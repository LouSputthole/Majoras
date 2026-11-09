"""Validate shared/city records against the frozen schemas and the ID registry (PIPE-01).

Usage:  python -I tools/reference/validate_city.py [--selftest] [paths...]
With no paths every shared/city/regions/*.json is checked. Exit 1 on any error; every error names
the file and the JSON path. Stdlib only: a small JSON Schema subset (type, const, enum, pattern,
required, properties, additionalProperties, items, minItems, maxItems, minimum, maximum, minLength,
anyOf, $ref to #/$defs) is enough for the three frozen schemas, so no dependency is added.
# ponytail: subset validator; swap for the jsonschema package if a schema ever needs more keywords.
"""
import copy
import glob
import io
import json
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
SCHEMA_DIR = os.path.join(ROOT, "shared", "city", "schema")
IDS_PATH = os.path.join(ROOT, "shared", "city", "ids.json")
REGION_GLOB = os.path.join(ROOT, "shared", "city", "regions", "*.json")

TYPES = {"string": str, "integer": int, "number": (int, float), "boolean": bool, "array": list, "object": dict, "null": type(None)}


def load(p):
    return json.load(io.open(p, encoding="utf-8"))


def check(schema, data, root, path, errs):
    if "$ref" in schema:
        node = root
        for part in schema["$ref"].lstrip("#/").split("/"):
            node = node[part]
        check(node, data, root, path, errs)
        return
    if "anyOf" in schema:
        for alt in schema["anyOf"]:
            sub = []
            check(alt, data, root, path, sub)
            if not sub:
                return
        errs.append(f"{path}: matches none of the allowed alternatives")
        return
    if "const" in schema and data != schema["const"]:
        errs.append(f"{path}: must equal {json.dumps(schema['const'])}, got {json.dumps(data)}")
        return
    if "enum" in schema and data not in schema["enum"]:
        errs.append(f"{path}: {json.dumps(data)} not in {schema['enum']}")
        return
    if "type" in schema:
        types = schema["type"] if isinstance(schema["type"], list) else [schema["type"]]
        ok = any(isinstance(data, TYPES[t]) and not (t in ("integer", "number") and isinstance(data, bool)) for t in types)
        if not ok:
            errs.append(f"{path}: expected {types}, got {type(data).__name__}")
            return
    if isinstance(data, str):
        if "pattern" in schema and not re.search(schema["pattern"], data):
            errs.append(f"{path}: {json.dumps(data)} does not match {schema['pattern']}")
        if "minLength" in schema and len(data) < schema["minLength"]:
            errs.append(f"{path}: shorter than {schema['minLength']}")
    if isinstance(data, (int, float)) and not isinstance(data, bool):
        if "minimum" in schema and data < schema["minimum"]:
            errs.append(f"{path}: {data} < {schema['minimum']}")
        if "maximum" in schema and data > schema["maximum"]:
            errs.append(f"{path}: {data} > {schema['maximum']}")
    if isinstance(data, list):
        if "minItems" in schema and len(data) < schema["minItems"]:
            errs.append(f"{path}: needs at least {schema['minItems']} items, has {len(data)}")
        if "maxItems" in schema and len(data) > schema["maxItems"]:
            errs.append(f"{path}: at most {schema['maxItems']} items, has {len(data)}")
        if "items" in schema:
            for i, v in enumerate(data):
                check(schema["items"], v, root, f"{path}[{i}]", errs)
    if isinstance(data, dict):
        props = schema.get("properties", {})
        for k in schema.get("required", []):
            if k not in data:
                errs.append(f"{path}: missing required key '{k}'")
        extra = schema.get("additionalProperties", True)
        for k, v in data.items():
            if k in props:
                check(props[k], v, root, f"{path}.{k}", errs)
            elif extra is False:
                errs.append(f"{path}: unknown key '{k}'")
            elif isinstance(extra, dict):
                check(extra, v, root, f"{path}.{k}", errs)


def cross_check_region(rec, ids, errs):
    """Everything the schema cannot know: IDs must exist in the registry and belong to this region."""
    rid = rec.get("regionId")
    locs = ids["locations"]
    if rid not in locs:
        errs.append(f"$.regionId: {rid!r} is not a registered location")
        return
    expected_rooms = set(locs[rid]["rooms"])
    for i, room in enumerate(rec.get("rooms", [])):
        if room.get("id") not in expected_rooms:
            errs.append(f"$.rooms[{i}].id: {room.get('id')!r} is not a room of {rid} (registry: {sorted(expected_rooms)})")
    for i, pid in enumerate(rec.get("portalIds", [])):
        if pid in ids["retiredPortals"]:
            errs.append(f"$.portalIds[{i}]: {pid!r} is retired: {ids['retiredPortals'][pid]}")
        elif pid not in ids["portals"]:
            errs.append(f"$.portalIds[{i}]: {pid!r} is not a registered portal")
        elif ids["portals"][pid]["from"] != rid:
            errs.append(f"$.portalIds[{i}]: {pid!r} departs from {ids['portals'][pid]['from']}, not {rid}")
    missing = [p for p, v in ids["portals"].items() if v["from"] == rid and p not in rec.get("portalIds", [])]
    if missing:
        errs.append(f"$.portalIds: registry has outgoing portals this record omits: {missing}")
    prof = rec.get("selection", {}).get("profile")
    if prof not in ids["profiles"]:
        errs.append(f"$.selection.profile: {prof!r} is not a registered profile")
    for i, cid in enumerate(rec.get("cameraIds", [])):
        if cid not in ids["cameras"]:
            errs.append(f"$.cameraIds[{i}]: {cid!r} is not a registered camera")
    for i, loc in enumerate(rec.get("sourceFrame", {}).get("sharedFrame", {}).get("with", [])):
        if loc not in locs:
            errs.append(f"$.sourceFrame.sharedFrame.with[{i}]: {loc!r} is not a registered location")
    if rec.get("source", {}).get("sceneSymbol") != locs[rid]["scene"]:
        errs.append(f"$.source.sceneSymbol: {rec.get('source', {}).get('sceneSymbol')!r} != registry {locs[rid]['scene']!r}")
    stem = os.path.basename(rec.get("_file", "")).rsplit(".", 1)[0]
    if stem and stem != rid.split(".", 1)[1]:
        errs.append(f"file name {stem}.json does not match regionId {rid}")


def validate_region_file(path, schema, ids):
    errs = []
    try:
        rec = load(path)
    except Exception as e:  # malformed JSON must fail visibly, not crash the run
        return [f"not valid JSON: {e}"]
    check(schema, rec, schema, "$", errs)
    if not errs:
        rec["_file"] = path
        cross_check_region(rec, ids, errs)
    return errs


def selftest(schema, ids):
    """Negative cases: each mutation of a good record must be rejected."""
    good_path = sorted(glob.glob(REGION_GLOB))[0]
    good = load(good_path)
    assert not validate_region_file(good_path, schema, ids), "selftest needs one passing record"

    def mutate(fn):
        rec = copy.deepcopy(fn.__self__ if hasattr(fn, "__self__") else good)
        return rec

    cases = []
    r = copy.deepcopy(good); r["surpriseKey"] = 1; cases.append(("unknown top-level key", r))
    r = copy.deepcopy(good); r["schemaVersion"] = 2; cases.append(("wrong schemaVersion", r))
    r = copy.deepcopy(good); r["regionId"] = "loc.not_a_place"; cases.append(("unregistered regionId", r))
    r = copy.deepcopy(good); r["portalIds"] = r["portalIds"] + ["portal.east_clock_town.astral_observatory.p169"]; cases.append(("retired portal", r))
    r = copy.deepcopy(good); r["portalIds"] = r["portalIds"][1:]; cases.append(("omitted outgoing portal", r))
    r = copy.deepcopy(good); r["selection"]["profile"] = "prof.d9.noon.normal"; cases.append(("bad profile", r))
    r = copy.deepcopy(good); r["components"][0]["status"] = "selected, special rule"; cases.append(("free-text status", r))
    r = copy.deepcopy(good); r["readiness"]["import"] = "fail (see section 9)"; cases.append(("free-text readiness", r))
    r = copy.deepcopy(good); r["conversionToUnity"]["matrix"][2][2] = 0.01; cases.append(("wrong conversion matrix", r))
    r = copy.deepcopy(good); r["anchors"] = r["anchors"][:2]; cases.append(("fewer than three anchors", r))
    r = copy.deepcopy(good); r["localSourceInputs"][0]["sha256"] = "abc"; cases.append(("bad sha256", r))
    r = copy.deepcopy(good); r["rooms"][0]["id"] = good["regionId"] + ".room_99"; cases.append(("room not in registry", r))
    failed = 0
    for name, rec in cases:
        errs = []
        check(schema, rec, schema, "$", errs)
        if not errs:
            rec["_file"] = good_path
            cross_check_region(rec, ids, errs)
        status = "rejected" if errs else "ACCEPTED (BUG)"
        failed += not errs
        print(f"  selftest {name}: {status}" + (f" -> {errs[0]}" if errs else ""))
    print(f"selftest: {len(cases) - failed}/{len(cases)} negative cases rejected; truncated JSON: ", end="")
    bad_json = os.path.join(os.path.dirname(good_path), "_selftest_truncated.json")
    io.open(bad_json, "w", encoding="utf-8").write(io.open(good_path, encoding="utf-8").read()[:200])
    try:
        print("rejected" if validate_region_file(bad_json, schema, ids) else "ACCEPTED (BUG)")
    finally:
        os.remove(bad_json)
    return failed == 0


def main(argv):
    schema = load(os.path.join(SCHEMA_DIR, "region.schema.json"))
    ids = load(IDS_PATH)
    args = [a for a in argv if not a.startswith("--")]
    paths = args or sorted(glob.glob(REGION_GLOB))
    total = 0
    for p in paths:
        errs = validate_region_file(p, schema, ids)
        rel = os.path.relpath(p, ROOT)
        print(f"{rel}: {'OK' if not errs else str(len(errs)) + ' error(s)'}")
        for e in errs:
            print("   ", e)
        total += len(errs)
    ok = total == 0
    if "--selftest" in argv:
        ok = selftest(schema, ids) and ok
    print(f"validate_city: {len(paths)} record(s), {total} error(s)")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
