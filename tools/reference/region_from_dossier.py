"""Lift the "Proposed region record" JSON block out of a dossier and normalize it to region schema 1 (PIPE-01).

Usage:  python -I tools/reference/region_from_dossier.py docs/city/districts/north_clock_town.md [more.md ...]
Writes shared/city/regions/<region>.json for each dossier. The dossiers keep their proposal-0 blocks as
written; this is the one place the proposal vocabulary is mapped onto the frozen enums. Run
validate_city.py afterwards; anything this script cannot map is left as-is so the validator reports it.
"""
import io
import json
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
OUT_DIR = os.path.join(ROOT, "shared", "city", "regions")

CLASS = {
    "static-mesh": ("static-mesh", None), "posed-skeletal": ("posed-skeletal", None), "npc-scheduled": ("npc-scheduled", None),
    "billboard/effect": ("billboard-effect", None), "controller/invisible": ("controller-invisible", None),
    "door/transition": ("door-transition", None),
    "static-mesh (camera-relative)": ("static-mesh", "camera-relative"), "static-mesh (state pickup)": ("static-mesh", "state-pickup"),
    "static-mesh (spawned)": ("static-mesh", "spawned"),
    "door/transition (posed-skeletal)": ("door-transition", "posed-skeletal"), "door/transition (posed skeleton)": ("door-transition", "posed-skeletal"),
}
SHARED_WITH = {
    "sharedWithSouth": ["loc.south_clock_town"],
    "sharedWithNorthWestEast": ["loc.north_clock_town", "loc.west_clock_town", "loc.east_clock_town"],
}
SHORT_LOC = {"South": "south_clock_town", "North": "north_clock_town", "West": "west_clock_town", "East": "east_clock_town", "Laundry": "laundry_pool"}
PRODUCED_BY = {"current committed scripts (inferred)": "inferred-committed-scripts", "earlier unrecorded export": "unrecorded"}
READINESS = ("not_run", "pass", "fail", "blocked", "not_applicable", "partial")


def split_status(text, enums):
    """'selected, yaw approximated' -> ('selected', 'yaw approximated'); 'partial (4 banks)' -> ('partial', '4 banks')."""
    head, sep, rest = text.partition(" ")
    word = head.rstrip(",")
    if word not in enums:
        return text, None
    note = rest.strip() or None
    if note and note.startswith("(") and note.endswith(")"):
        note = note[1:-1]
    if head.endswith(",") or sep:
        return word, note
    return word, None


def normalize(rec):
    rec = dict(rec)
    rec.pop("_status", None)
    rec["schemaVersion"] = 1
    sel = rec["selection"]
    m = re.search(r"sceneLayer (\d+)", sel.pop("sceneHeader", "sceneLayer 0"))
    sel["sceneLayer"] = int(m.group(1)) if m else 0
    room_headers = sel.pop("rooms", {})
    for room in rec["rooms"]:
        h = room_headers.get(room["id"], "default header")
        room["header"] = 0 if h.startswith("default") else int(re.search(r"\d+", h).group())
    sf = rec["sourceFrame"]
    for key in [k for k in sf if k.startswith("sharedWith")]:   # sharedWithSouth, sharedWithNorthWestEast, sharedWithEast, ...
        text = sf.pop(key)
        locs = SHARED_WITH.get(key) or [f"loc.{n}" for n in SHORT_LOC.values() if n.split("_")[0].capitalize() in re.findall(r"[A-Z][a-z]+", key[10:])]
        status = "verified" if text.lower().startswith("verified") else "inferred" if "INFERRED" in text or "inferred" in text else "unknown"
        sf["sharedFrame"] = {"with": locs, "status": status, "evidence": text}
    sf.setdefault("sharedFrame", {"with": [], "status": "unknown", "evidence": ""})
    if "cameraSourceHints" in rec:                              # camera poses belong in cameras/<region>.json (REF-03), not the region record
        rec.pop("cameraSourceHints")
        print(f"  note: dropped cameraSourceHints from {rec['regionId']} (stays in the dossier; goes into the camera records)")
    for a in rec["anchors"]:
        s = a["status"]
        if s.startswith("proposed"):
            a["status"] = "proposed"
        elif s.startswith("ground probe"):
            a["status"], a["note"] = "probed", s
        elif s.startswith("surveyed"):
            a["status"] = "surveyed"
    for c in rec["components"]:
        cls, variant = CLASS.get(c["class"], (c["class"], None))
        c["class"] = cls
        if variant:
            c["variant"] = variant
        word, note = split_status(c["status"], ("selected", "imported", "optional"))
        c["status"] = word
        if note:
            c["note"] = note
    co = rec["collisionOwnership"]
    if isinstance(co.get("actorCylinders"), str):
        co["note"] = "actorCylinders: " + co["actorCylinders"]
        co["actorCylinders"] = None
    if isinstance(co.get("waterBoxes"), list):                  # schema wants the count; the descriptions move to the note
        co["note"] = (co.get("note", "") + " waterBoxes: " + "; ".join(map(str, co["waterBoxes"]))).strip()
        co["waterBoxes"] = len(co["waterBoxes"])
    ex = rec["excluded"]
    if "stateOrDrawNull" in ex:
        ex["stateOrSchedule"] = ex.pop("stateOrDrawNull")
    notes = {}
    for k, v in rec["readiness"].items():
        word, note = split_status(v, READINESS)
        rec["readiness"][k] = word
        if note:
            notes[k] = note
    if notes:
        rec["readinessNotes"] = notes
    for name, exp in rec.get("localExports", {}).items():
        if "producedBy" in exp:                                 # a missing value stays missing so the validator reports it
            exp["producedBy"] = PRODUCED_BY.get(exp["producedBy"], exp["producedBy"])
    return rec


def extract_block(md_path):
    text = io.open(md_path, encoding="utf-8").read()
    m = re.search(r"^## \d+\. Proposed region record.*?```json\n(.*?)\n```", text, re.S | re.M)
    if not m:
        raise SystemExit(f"{md_path}: no 'Proposed region record' JSON block")
    return json.loads(m.group(1))


def main(argv):
    os.makedirs(OUT_DIR, exist_ok=True)
    for md in argv:
        rec = normalize(extract_block(md))
        out = os.path.join(OUT_DIR, rec["regionId"].split(".", 1)[1] + ".json")
        io.open(out, "w", encoding="utf-8", newline="\n").write(json.dumps(rec, indent=2) + "\n")
        print(f"{os.path.relpath(md, ROOT)} -> {os.path.relpath(out, ROOT)}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
