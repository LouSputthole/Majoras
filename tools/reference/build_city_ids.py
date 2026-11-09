"""Build shared/city/ids.json, the frozen ID registry, from the three city registers (PIPE-01).

Usage:  python -I tools/reference/build_city_ids.py
Reads docs/city/LOCATION_REGISTER.md (locations, rooms), PORTAL_REGISTER.md (directed portals) and
SOURCE_REGISTER.md (profiles, cameras). Rerun after a register changes; validate_city.py reads the result.
Rows whose mode starts with "none" are recorded as retired portals (kept so the name is never reused).
"""
import io
import json
import os
import re
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
CITY = os.path.join(ROOT, "docs", "city")
OUT = os.path.join(ROOT, "shared", "city", "ids.json")

PATTERNS = {
    "location": "^loc\\.[a-z][a-z0-9_]*$",
    "room": "^loc\\.[a-z][a-z0-9_]*\\.room_[0-9]{2}$",
    "portal": "^portal\\.[a-z][a-z0-9_]*\\.[a-z][a-z0-9_]*(\\.[a-z0-9][a-z0-9_]*)?$",
    "profile": "^prof\\.d[1-3]\\.[a-z]+\\.[a-z0-9]+$",
    "camera": "^cam\\.[a-z][a-z0-9_]*\\.[a-z][a-z0-9_]*$",
    "route": "^route\\.[a-z][a-z0-9_]*\\.[a-z][a-z0-9_]*$",
    "anchor": "^anchor\\.[a-z][a-z0-9]*\\.[a-z][a-z0-9_]*$",
    "water": "^water\\.[a-z][a-z0-9_]*\\.[a-z][a-z0-9_]*$",
    "external": "^ext\\.[a-z][a-z0-9_]*$",
    "componentSourceKey": "^Z2_[A-Z0-9_]+/h[0-9]+/(room_[0-9]{2}/a[0-9]+(/[a-z0-9-]+)?|ta[0-9]+(-ta[0-9]+)?|transition/ta[0-9]+)$",
}


def rd(name):
    return io.open(os.path.join(CITY, name), encoding="utf-8").read().splitlines()


def cells(line):
    return [c.strip() for c in line.strip().strip("|").split("|")]


def tick(s):
    m = re.search(r"`([^`]+)`", s)
    return m.group(1) if m else s


def rooms_from(cell):
    m = re.match(r"room_(\d\d) to room_(\d\d)", cell)
    if m:
        return [f"room_{i:02d}" for i in range(int(m.group(1)), int(m.group(2)) + 1)]
    return re.findall(r"room_\d\d", cell)


def main():
    locations, portals, retired, profiles, cameras, externals = {}, {}, {}, [], [], set()
    for line in rd("LOCATION_REGISTER.md"):
        if not line.startswith("| `loc."):
            continue
        c = cells(line)
        lid = tick(c[0])
        if re.match(r"^room_\d\d", c[1]):                      # room table row
            locations[lid]["rooms"] = [f"{lid}.{r}" for r in rooms_from(c[1])]
        elif c[2].startswith("`Z2_"):                           # location table row
            locations[lid] = {"label": c[1].split(" (")[0], "scene": tick(c[2]), "sceneEnum": c[2].split("/")[1].strip().strip("`"), "rooms": []}
    for line in rd("PORTAL_REGISTER.md"):
        if not line.startswith("| `portal."):
            continue
        c = cells(line)
        pid, mode, src = tick(c[0]), c[1].split(" ")[0], tick(c[2])
        if mode == "none":
            retired[pid] = c[1]
            continue
        if mode == "room":                                      # section 4.3 layout: Scene | Evidence | Rooms "room N -> room M"
            rm = re.match(r"room (\d+) -> room (\d+)", c[4])
            portals[pid] = {"mode": mode, "from": src, "to": src, "fromRoom": f"{src}.room_{int(rm.group(1)):02d}", "toRoom": f"{src}.room_{int(rm.group(2)):02d}"}
            continue
        dst = tick(c[6])
        portals[pid] = {"mode": mode, "from": src, "to": dst}
        if dst.startswith("ext."):
            externals.add(dst)
    for line in rd("SOURCE_REGISTER.md"):
        if line.startswith("| `prof."):
            p = tick(cells(line)[0])
            if p not in profiles:
                profiles.append(p)
        elif line.startswith("| `"):                           # camera ids live in two tables (per camera, per location)
            cameras.extend(re.findall(r"`(cam\.[a-z][a-z0-9_]*\.[a-z][a-z0-9_]*)`", line))
    for k, pat in [("location", "location"), ("portal", "portal"), ("profile", "profile"), ("camera", "camera")]:
        pool = {"location": locations, "portal": portals, "profile": profiles, "camera": cameras}[k]
        bad = [x for x in pool if not re.match(PATTERNS[pat], x)]
        assert not bad, f"{k} ids violate the frozen pattern: {bad}"
    for lid, loc in locations.items():
        assert loc["rooms"], f"{lid} has no room list"
    for pid, p in portals.items():
        assert p["from"] in locations, f"{pid}: from {p['from']} unknown"
        assert p["to"] in locations or p["to"] in externals, f"{pid}: to {p['to']} unknown"
        for k in ("fromRoom", "toRoom"):
            assert k not in p or p[k] in locations[src := p["from"]]["rooms"], f"{pid}: {k} {p[k]} is not a room of {p['from']}"
    try:
        head = subprocess.run(["git", "-C", ROOT, "rev-parse", "--short", "HEAD"], capture_output=True, text=True, check=True).stdout.strip()
    except Exception:
        head = "unknown"
    out = {
        "schemaVersion": 1,
        "frozen": "2026-11-08 (PIPE-01)",
        "generatedBy": "tools/reference/build_city_ids.py",
        "generatedFrom": {"commit": head, "files": ["docs/city/LOCATION_REGISTER.md", "docs/city/PORTAL_REGISTER.md", "docs/city/SOURCE_REGISTER.md"]},
        "legacyAliases": {"scene.south_clock_town": "loc.south_clock_town", "sct.*": "loc.south_clock_town (proxy component ids, kept valid)"},
        "patterns": PATTERNS,
        "locations": locations,
        "portals": portals,
        "retiredPortals": retired,
        "profiles": profiles,
        "cameras": sorted(set(cameras)),
        "externals": sorted(externals),
    }
    io.open(OUT, "w", encoding="utf-8", newline="\n").write(json.dumps(out, indent=2) + "\n")
    print(f"ids.json: {len(locations)} locations, {sum(len(l['rooms']) for l in locations.values())} rooms, "
          f"{len(portals)} portals (+{len(retired)} retired), {len(profiles)} profiles, {len(set(cameras))} cameras, {len(externals)} externals")
    return 0


if __name__ == "__main__":
    sys.exit(main())
