#!/usr/bin/env python3
"""REF-01 helper: count header data in the ZAPD-extracted Clock Town scenes (read-only, stdlib only).

Usage: python -I tools/reference/city_index.py [path/to/local-reference/mm-decomp]
Reads scene_table.h, z_scene_table.c, scene XMLs and extracted scene/room .c files; writes nothing.
"""
import pathlib
import re
import sys

LOCS = [("loc.south_clock_town", "Z2_CLOCKTOWER"), ("loc.north_clock_town", "Z2_BACKTOWN"),
        ("loc.west_clock_town", "Z2_ICHIBA"), ("loc.east_clock_town", "Z2_TOWN"),
        ("loc.laundry_pool", "Z2_ALLEY"), ("loc.trading_post", "Z2_8ITEMSHOP"), ("loc.bomb_shop", "Z2_BOMYA"),
        ("loc.curiosity_shop", "Z2_AYASHIISHOP"), ("loc.post_office", "Z2_POSTHOUSE"),
        ("loc.swordsmans_school", "Z2_DOUJOU"), ("loc.lottery_shop", "Z2_TAKARAKUJI"),
        ("loc.stock_pot_inn", "Z2_YADOYA"), ("loc.mayors_residence", "Z2_SONCHONOIE"),
        ("loc.milk_bar", "Z2_MILK_BAR"), ("loc.honey_darling", "Z2_BOWLING"),
        ("loc.treasure_chest_shop", "Z2_TAKARAYA"), ("loc.town_shooting_gallery", "Z2_SYATEKI_MIZU"),
        ("loc.great_fairy_fountain_town", "Z2_YOUSEI_IZUMI"), ("loc.deku_playground", "Z2_DEKUTES"),
        ("loc.astral_observatory", "Z2_TENMON_DAI"), ("loc.clock_tower_interior", "Z2_INSIDETOWER"),
        ("loc.clock_tower_rooftop", "Z2_OKUJOU")]

ROOT = pathlib.Path(sys.argv[1] if len(sys.argv) > 1 else pathlib.Path(__file__).resolve().parents[2]
                    / "local-reference" / "mm-decomp")
EXT = ROOT / "extracted" / "n64-us" / "assets" / "scenes"


def array(text, sym):
    """Body of the C array named sym, or None (also when the header has no such command)."""
    if not sym:
        return None
    m = re.search(r"\b" + re.escape(sym) + r"\[\]\s*=\s*\{(.*?)\n\};", text, re.S)
    return m.group(1) if m else None


def commands(text, sym):
    """Ordered (command, args) pairs of a SceneCmd array."""
    body = array(text, sym) or ""
    return [(m.group(1), m.group(2)) for m in re.finditer(r"SCENE_CMD_(\w+)\((.*?)\),?\s*$", body, re.M)]


def headers(text, default_sym):
    """[('default', sym), ('alt1', sym|None), ...] following SCENE_CMD_ALTERNATE_HEADER_LIST."""
    out = [("default", default_sym)]
    for cmd, args in commands(text, default_sym):
        if cmd == "ALTERNATE_HEADER_LIST":
            body = array(text, args.strip()) or ""
            for i, e in enumerate([x.strip() for x in body.split(",") if x.strip()]):
                out.append(("alt%d" % (i + 1), None if e == "NULL" else e))
    return out


def arg(cmds, name, idx):
    for c, a in cmds:
        if c == name:
            return [x.strip().lstrip("&") for x in a.split(",")][idx]
    return None


def load_tables():
    st = (ROOT / "include" / "tables" / "scene_table.h").read_text(encoding="utf-8").splitlines()
    rows, last_comment = {}, ""
    for n, line in enumerate(st, 1):
        if line.startswith("//"):
            last_comment = line[2:].strip()
        m = re.match(r"/\* (0x[0-9A-F]+) \*/ DEFINE_SCENE\((\w+), (\w+), (\w+), (\w+), (RESTRICTIONS_\w+(?:\([^)]*\))?), (.*)\)$", line)
        if m:
            rows[m.group(2)] = dict(index=m.group(1), enum=m.group(3), text=m.group(4), draw=m.group(5),
                                    restr=m.group(6), cycle=m.group(7), name=last_comment, line=n)
    zst = (ROOT / "src" / "code" / "z_scene_table.c").read_text(encoding="utf-8")
    ent_index = {}
    for m in re.finditer(r"/\* (0x[0-9A-F]+) \*/ SCENE_ENTRANCE\((\w+), \"(\w+)\"\)", zst):
        ent_index[m.group(3)] = (int(m.group(1), 16), m.group(2))
    return rows, zst, ent_index


def entrance_table(zst, table_sym):
    """List of per-entrance lists of (sceneId, spawnNum, flags) for each layer."""
    body = array(zst, table_sym) or ""
    out = []
    for sub in re.findall(r"\w+", body):
        b = array(zst, sub) or ""
        out.append(re.findall(r"\{\s*(-?\w+),\s*(\d+),\s*(0x[0-9A-F]+)\s*\}", b))
    return out


def main():
    rows, zst, ent_index = load_tables()
    by_num = {v[0]: k for k, v in ent_index.items()}
    for loc, s in LOCS:
        r = rows[s]
        xml = (ROOT / "assets" / "xml" / "scenes" / s / (s + ".xml")).read_text(encoding="utf-8")
        xml_rooms = re.findall(r'<File Name="(\w+)" Segment="3"', xml)
        sc = (EXT / s / (s + ".c")).read_text(encoding="utf-8")
        ei = ent_index.get(s)
        print("## %s %s | %s | %s idx %s text %s | %s | %s | %s | scene_table.h:%d" % (
            loc, s, r["name"], r["enum"], r["index"], r["text"], r["draw"], r["restr"], r["cycle"], r["line"]))
        print("   xml rooms: %d %s | entrance-table idx %s" % (len(xml_rooms), ",".join(xml_rooms),
                                                         "0x%02X" % ei[0] if ei else "NONE"))
        if ei:
            et = entrance_table(zst, ei[1])
            print("   z_scene_table %s: %d entrances; layers per entrance %s; spawnNum(layer0) %s" % (
                ei[1], len(et), [len(e) for e in et], [e[0][1] if e else "-" for e in et]))
        for tag, sym in headers(sc, s + "Commands"):
            if sym is None:
                print("   scene %s: NULL (engine keeps default header, z_scene.c Scene_CommandAltHeaderList)" % tag)
                continue
            c = commands(sc, sym)
            has_exit = arg(c, "EXIT_LIST", 0) is not None
            exits = re.findall(r"0x[0-9A-F]+", array(sc, arg(c, "EXIT_LIST", 0)) or "")
            ents = re.findall(r"\{\s*(0x[0-9A-F]+),\s*(0x[0-9A-F]+)\s*\}", array(sc, arg(c, "ENTRANCE_LIST", 0) or "") or "")
            tr = re.findall(r"\{\s*(-?\d+),\s*(-?\d+),\s*(-?\d+),\s*(-?\d+),\s*(ACTOR_\w+)", array(sc, arg(c, "TRANSITION_ACTOR_LIST", 1) or "") or "")
            spawns = (array(sc, arg(c, "SPAWN_LIST", 1) or "") or "").count("ACTOR_PLAYER")
            dec = ["%s>%s e%d l%d" % (x, by_num.get(int(x, 16) >> 9, "?"), (int(x, 16) >> 4) & 0x1F, int(x, 16) & 0xF)
                   for x in exits]
            print("   scene %s %s: rooms %s trans %s(%d) spawns %s(%d) exits %s entrances %d col %s" % (
                tag, sym, arg(c, "ROOM_LIST", 0), arg(c, "TRANSITION_ACTOR_LIST", 0), len(tr),
                arg(c, "SPAWN_LIST", 0), spawns, len(exits) if has_exit else "NO_CMD", len(ents),
                arg(c, "COL_HEADER", 0)))
            print("      entrance->(spawn,room): %s" % " ".join("%d:(%d,%d)" % (i, int(a, 16), int(b, 16))
                                                          for i, (a, b) in enumerate(ents)))
            print("      exits: %s" % "; ".join(dec))
            if tr:
                print("      transition (front room, back room, actor): %s" % "; ".join(
                    "%s/%s %s" % (t[0], t[2], t[4].replace("ACTOR_", "")) for t in tr))
        for room in xml_rooms:
            p = EXT / s / (room + ".c")
            if not p.exists():
                print("   %s: NO EXTRACTED C" % room)
                continue
            rc = p.read_text(encoding="utf-8")
            parts = []
            for tag, sym in headers(rc, room + "Commands"):
                if sym is None:
                    parts.append("%s=NULL" % tag)
                    continue
                c = commands(rc, sym)
                no, na = arg(c, "OBJECT_LIST", 0), arg(c, "ACTOR_LIST", 0)
                ab = array(rc, arg(c, "ACTOR_LIST", 1) or "") or ""
                ob = array(rc, arg(c, "OBJECT_LIST", 1) or "") or ""
                chk = "" if (na is None or int(na) == ab.count("{ ACTOR_")) and (no is None or int(no) == ob.count("OBJECT_")) else "!"
                parts.append("%s=o%s/a%s%s" % (tag, no or 0, na or 0, chk))
            print("   %s: %d headers: %s" % (room, len(parts), " ".join(parts)))


if __name__ == "__main__":
    main()
