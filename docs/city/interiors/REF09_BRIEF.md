# REF-09 shared brief: interior venue dossiers (one packet per INT-01..17)

You are one of several agents each writing ONE interior dossier for the Majora's Mask Clock Town Unity fan remake at `C:\Users\cargi\code\Majoras`. The owner has full permission for this fan project. Your packet id, venue, scene and owned file are in your dispatch message. Everything else is here.

## Baseline and pins

- Repo `C:\Users\cargi\code\Majoras`, branch `main`, base commit `31b20dc` (REF-01..08 done). Do not commit, stage, checkout or modify any tracked file. Do not push.
- Decomp: `local-reference/mm-decomp/` = zeldaret/mm pinned at `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US). Ignored path, read-only for you.
- Extracted readable C for every city scene: `local-reference/mm-decomp/extracted/n64-us/assets/scenes/<SCENE>/<SCENE>.c` (scene header, alternate headers, spawns, exits, transition actors, collision header, water boxes, cutscene data) and `<SCENE>_room_NN.c` (room headers, OBJECT_LIST, ACTOR_LIST, display lists). Only four object banks are extracted (tower, turret, step, tobira); for others read `assets/xml/objects/*.xml` and the overlay C.
- Entrance tables: `src/code/z_scene_table.c` (entrance-table index != scene-table enum). Actor overlays: `src/overlays/actors/ovl_<Name>/z_<name>.c`. Object ids: `include/tables/object_table.h`. Actor ids: `include/tables/actor_table.h`. Collision macros: `include/z64bgcheck.h`. Door: `include/z64door.h`, `ovl_En_Door/z_en_door.h`.

## Read first (in this order, skim where long)

1. `docs/HANDOFF.md` (entry point) and `docs/FULL_CITY_PLAN.md` section 5 (your INT row), section 11 (packet return format), section 12 (REF-09 row).
2. `docs/city/LOCATION_REGISTER.md`: your `loc.*` row(s) and the room table row. Counts there are the contract; if you disagree, say so with evidence, do not edit it.
3. `docs/city/PORTAL_REGISTER.md`: every row whose from or to is your `loc.*`. The portal ids there are frozen names; reuse them.
4. `docs/city/SOURCE_REGISTER.md`: time model, `prof.*` profiles, `cam.*` naming.
5. Template: `docs/city/districts/north_clock_town.md` (single room) and `docs/city/districts/east_clock_town.md` section 2.4 (door-to-venue table). Your dossier mirrors their section structure and evidence discipline. For a multi-room scene add one subsection per room under spawns/actors/collision; never inventory only room_00.
6. The exterior dossier holding the door INTO your venue (the district named in your dispatch): read its transition-actor table and cross-check the door schedule type, exit polygons and return spawn against your scene's data. Report disagreements under Unknowns; do not edit the other file.

## Decode rules (cite these lines, verify them yourself if you rely on more)

- Actor entry id = `raw & 0x1FFF`; rotation degrees = `((raw >> 7) & 0x1FF)` per axis; half-day mask = `((rotX & 7) << 7) | (rotZ & 0x7F)`, 0 = every half-day, Day 1 daytime bit = 0x80 (`src/code/z_actor.c:2491-2498, 2562-2571, 3576-3580, 3651-3654`); cutscene id = `rotY & 0x7F` (0x7F = none); transition actor spawn `src/code/z_actor.c:3600-3625`.
- Entrance word: `value >> 9` = entrance-table index, `(value >> 4) & 0x1F` = spawn slot, `value & 0xF` = layer (`Entrance_GetTableEntry`, `src/code/z_scene_table.c:2688-2694`).
- Exit from collision: `SurfaceType` word0 bits 8-12 = exit index + 1; `CollisionPoly {type, vIA|flags, vIB|flags, vIC, nx, ny, nz, dist}`; the player passes only FLOOR polygons to `Player_HandleExitsAndVoids` (`src/code/z_player.c:6405-6416`, call sites `:6783` door probe and `:12572` standing floor). Walls with exit bits are dead data.
- `EN_DOOR` params carry schedule type only (`ENDOOR_GET_TYPE = (p >> 7) & 7`, type var `p & 0x7F`, `ENDOOR_SCH_TYPE_*` names the venue); the exit polygon behind the door changes scene.
- Time: `CLOCK_TIME(h,m)` (`include/macros.h:23`); clock speed 3 normal, 5 without Ocarina, 1 inverted; night is >= 18:00 or < 06:00 (`z_kankyo.c:1229`), 06:30 at `Play_Init` (`z_play.c:2202`); `sceneLayer = (cutsceneIndex & 0xF) + 1` when >= 0xFFF0 (`z_play.c:2210-2219`); alternate header = `altHeaders[layer - 1]`.
- Coordinates: N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

## Fixed ID namespaces (reuse, do not invent new families)

`loc.<place>` (your loc id is given), rooms `<loc>.room_NN`, `portal.<from_short>.<to_short>[.qualifier]`, profiles `prof.<day>.<time>.<layer>` (baseline `prof.d1.noon.normal`; interiors may need `prof.d1.night.normal` etc., say which header a profile selects), cameras `cam.<loc_short>.<name>`, outside destinations `ext.<scene_lower>`.

## Required sections of your dossier

Header paragraph exactly like North's (task id, base commit, pins, "source inspection only", relative-path convention, evidence tags XML/HDR/C/OVL/INF/NI).

1. Scene identity: scene-table entry, config, draw config, restriction flags, every scene header (default + alternates) and what each alternate changes; room list with each declared room file.
2. Spawns, entrances and exits: start positions with params decode and arrival use (reverse-link to PORTAL_REGISTER); exit list with decode and destinations; transition actors (all rooms) with params, schedule type, sides, which rooms they connect; collision surfaces carrying exits per room; proposed portal ids (must match PORTAL_REGISTER; add internal room portals as `portal.<loc_short>.room_NN.room_MM` if the register lacks them).
3. Object banks per room header, with ids and names.
4. Actor tables: decode rules you relied on; one table per room header (default and each alternate) with index, actor, position, rot/yaw, half-day mask, csId, params, class (static-mesh / posed-skeletal / npc-scheduled / billboard-effect / controller-invisible / door-transition), Day 1 daytime candidate yes/no; overlay findings per actor type that gate presence, pose or state; counts table.
5. Venue mechanics index (this is what distinguishes interiors): for the mechanics named in your INT row's scope column, list the overlay files, functions, flags (`WEEKEVENTREG_*`, `gSaveContext` fields, `HS_*`), item/rupee constants, time windows and camera settings that implement them, with line cites. Index and cite; do not design the Unity implementation.
6. Day 1 noon static assembly list for `prof.d1.noon.normal` (which header, which actors present by mask and by flag on a fresh save, what geometry each needs, extraction commands written but NOT executed). If the venue is closed at Day 1 noon, say so with the door schedule cite and name the first profile in which it is open.
7. Collision summary per room: vertices/polys/surface types, water boxes, exit surfaces, floor/wall/ceiling counts, materials, camera data, anything special (ladders, hookshot, conveyors, void-out).
8. Materials and textures: counts by format and names only; nothing copied.
9. Required extraction list (commands NOT executed).
10. Proposed region record: a fenced JSON block. The schema is now FROZEN (PIPE-01, commit `7a7a08c`): read `shared/city/schema/README.md` and `shared/city/schema/region.schema.json` and write the block directly in schema 1 (`"schemaVersion": 1`, enum values only for `components[].class`/`status`, `anchors[].status`, `readiness.*`; free text goes in `note`/`readinessNotes`; `sourceFrame.sharedFrame` object; `selection.sceneLayer` integer; one `rooms[]` entry per room with its `header` index; `portalIds` must be exactly the portals departing from your `loc.*` in `shared/city/ids.json`, including `.kickout` and room swaps). Use the accepted `shared/city/regions/laundry_pool.json` and `south_clock_town.json` as worked examples. Do NOT add keys the schema does not list (the validator rejects unknown keys): camera pose hints go in your section 7 prose, not the record; `collisionOwnership.waterBoxes` is an integer count; `sourceFrame.sharedFrame` is the only shared-frame field; optional `rooms[].role` and `rooms[].collisionLightIndex` are allowed. Do NOT run `region_from_dossier.py` or `validate_city.py` yourself (they write outside your owned path); the integrator lifts and validates your block.
11. Unknowns, next source questions, checks: table of `U-NN | unknown | exact next source question`; then "Checks run" and "NOT run" lists.

## Hard rules

- Source inspection only. Do NOT run ZAPD, Blender, Unity, any player or build, any capture. Do NOT touch `Unity/`, `evidence/` or anything outside your owned file and your scratch folder.
- Python: NEVER run `python -` or `python -I -` or any form that reads a program from stdin (it hangs the machine's Python 3.14). Write scripts to your scratch folder and run `python -I <absolute path>.py`. Bash `cd` state leaks between calls: use absolute paths in every command.
- Plain ASCII in the dossier: no em dashes, no smart quotes, no non-ASCII. Markdown tables. Cite `file:line` for every non-obvious claim and tag each with XML/HDR/C/OVL/INF/NI.
- Numbers you report must come from the C you read, not from memory of the game. If LOCATION_REGISTER disagrees with the C, report both numbers.
- Nothing from the ROM (textures, display-list bytes, audio) goes into the dossier; names, counts and coordinates are fine.
- Keep the file under about 700 lines. Prefer tables over prose.

## Return format (FULL_CITY_PLAN section 11, reference packet)

```
Task ID and one region/venue:
Baseline commit and source/tool revisions:
Owned paths:
Source scene/header/room/profile:
Inventory: rooms, actors, object banks, collision, paths, doors/exits, materials:
Source citations: file + symbol + selected condition, not just a generic URL:
Capture list and reproducible state:
Known unknowns and the exact next source question:
Produced register/manifest/camera/route records:
Checks run and what remains NOT RUN:
Review request / next dependency:
```

Keep the returned summary under about 60 lines; the detail lives in the dossier. State plainly anything you could not verify.
