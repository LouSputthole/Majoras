# Town Great Fairy fountain (`loc.great_fairy_fountain_town`): source inventory dossier

Task **REF-09 / INT-13** (FULL_CITY_PLAN.md section 6 row INT-13 and section 12 rows REF-09, ROOM-03; this is the special-space pilot). Base commit `31b20dc` on `main` per the REF-09 brief; HEAD observed while writing is `7a7a08c` (PIPE-01), which froze `shared/city/` but did not change the three registers this file cross-checks. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_YOUSEI_IZUMI` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save), **room_00 only**.

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_YOUSEI_IZUMI/Z2_YOUSEI_IZUMI.c`, `room0.c` .. `room4.c` = `.../Z2_YOUSEI_IZUMI_room_00.c` .. `_room_04.c`. Overlays are `src/overlays/actors/ovl_<Name>/z_<name>.c`; `elfgrp.c`, `elforg.c`, `yoseizo.c`, `getitem.c`, `elfmsg3.c` abbreviate En_Elfgrp, En_Elforg, Bg_Dy_Yoseizo, Demo_Getitem and Elf_Msg3. Line numbers count `\n` only. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_YOUSEI_IZUMI` / `SCENE_YOUSEI_IZUMI` | `include/tables/scene_table.h:83` (HDR) |
| Scene-table index | `0x26` | same line |
| Entrance-table scene index | `0x23` (`ENTR_SCENE_FAIRY_FOUNTAIN`) | `src/code/z_scene_table.c:2608`, `include/z64scene.h:628` |
| Title card / draw config / restrictions | `0x13E` / `SCENE_DRAW_CFG_MAT_ANIM` / `RESTRICTIONS_NONE` | scene_table.h:83 |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_SET((1 << 10), 0, 0, 0)`: switch0 bit 10 survives the cycle reset (arg order `src/code/z_sram_NES.c:16-23`, mask applied `:451`, `:457-458`). Its only consumer in this scene is Elf_Msg3 switch flag `0x0A` (section 5.7) | HDR, C |
| XML files | scene segment 2; five room files `Z2_YOUSEI_IZUMI_room_00` .. `_04`, segment 3, each with two declared DLs (room_00: `0x3DF0`, `0x50B0`) | XML `assets/xml/scenes/Z2_YOUSEI_IZUMI/Z2_YOUSEI_IZUMI.xml` |
| Room count | 5 (`SCENE_CMD_ROOM_LIST(5, ...)` in both headers) | HDR scene.c:20, :10230 |
| Scene headers | default + 1 alternate (`Z2_YOUSEI_IZUMISet_01BE20`) | HDR scene.c:37-39 |
| Room headers | every room: default + 1 alternate | HDR room0.c:29-31 (same in room1-4) |
| Special files | `NAVI_QUEST_HINTS_NONE`, keep `GAMEPLAY_FIELD_KEEP` | HDR scene.c:24 |
| Sound | spec 3, ambience 19, seq 40 = `NA_BGM_FAIRY_FOUNTAIN` (row index 40 of the `DEFINE_SEQUENCE*` rows, counting from 0, `include/tables/sequence_table.h:100`); alt header seq 0 | HDR scene.c:19, :10229; C ordinal |
| Skybox | `(0x00, 0, 0, true)`: lightMode 1 = `LIGHT_MODE_SETTINGS`, lights come from light settings, not from time of day | HDR scene.c:26; `include/z64environment.h:47-50`; `src/code/z_scene.c:373-377` |
| Env light settings | 17 (default), 10 (alt); values not transcribed. Surfaces select index 0 (corridor) and 5 (chamber) in room_00 (section 7) | HDR scene.c:28, :94-129, :10238 |
| Animated materials | 3 entries, all type 1 (`AnimatedMat_DrawTwoTexScroll`) on segments 8, 9, 10; shared by both headers | HDR scene.c:6628-6647, :33, :10242; `src/code/z_scene_proc.c:389-404` |
| Minimap | 5 entries, mapId `0xFFFF`, centerX 2400 / 1200 / 0 / -1200 / -2400 (one per room) | HDR scene.c:6586-6596 |
| Transition actors | none (no `SCENE_CMD_TRANSITION_ACTOR_LIST` in either header) | HDR |
| Cutscene tables | default: 24 scripts, 38 actor cutscenes, 5 actor-cutscene cameras; alt: 1, 1, none | HDR scene.c:29-31, :10239-10240 |
| Room commands (all rooms, both headers) | echo 7; `ROOM_BEHAVIOR(0x00, 0x06, 0, 0, false, 0)` = environment 6 `ROOM_ENV_UNK_STRETCH_3`, storm 0; `SKYBOX_DISABLES(true, true)`; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3); cullable shape, 9 entries | HDR room0.c:19-23, :78-82; `include/z64scene.h:323-331`, `:804-807` |

### 1.1 What the alternate header (scene layer 1) changes

Selection (C): layer 1 needs `cutsceneIndex >= 0xFFF0` at load (`src/code/z_play.c:2210-2219`; `src/code/z_scene.c:464-474`). The only writer found is the credits branch `CS_CREDITS_MASK_GREAT_FAIRY`: with the Great Fairy's Mask in inventory it sets `ENTRANCE(FAIRY_FOUNTAIN, 0)` and `nextCutsceneIndex = 0xFFF0` (`src/code/z_demo.c:701-704`). Layer 1 is a credits presentation, outside city scope.

| Item | Default (layer 0) | Alternate (layer 1) |
| --- | --- | --- |
| Spawns / entrance rows | 10 / 10 | 1 at (2400, 20, -1021) yaw 180 / 2 rows, both spawn 0 room 0 (scene.c:10246-10262) |
| Exit list | 6 words | identical 6 words (scene.c:10264-10271) |
| Collision, animated materials | `CollisionHeader_01BDE8`, `TexAnim_0119F8` | same |
| BGM | seq 40 | seq 0 |
| Light settings | 17 | 10 |
| Scripts / actor cs / cs cams | 24 / 38 / 5 | 1 (credits, spawn 0 `CS_SPAWN_FLAG_ALWAYS`, `CS_CHOOSE_CREDITS_SCENES`) / 1 / none (scene.c:10296-10395) |
| room_00 objects / actors | 2 / 5 (shop actors, section 4.2) | 2 / 5: five `Bg_Dy_Yoseizo`, one of each type, at z -1764..-1800 behind the collision's back wall (z min -1121) (room0.c:93-109) |
| room_01 .. room_04 actors | 3, 3, 3, 4 | none: the alt room headers have no `SCENE_CMD_ACTOR_LIST` (room1.c:70-78 etc.) |

### 1.2 Room list and which room is Clock Town's

| Room | XML file | Origin x (minimap / cull centers / spawns) | Great Fairy type (default `Bg_Dy_Yoseizo` params) | Room exit | Region by spawn (`sGreatFairySpawnRegions`) |
| --- | --- | --- | --- | --- | --- |
| `loc.great_fairy_fountain_town.room_00` | `Z2_YOUSEI_IZUMI_room_00` | 2400 | `0x0000`: type 0 `GREAT_FAIRY_TYPE_MAGIC`, switch flag 0 | 0 `0xD630` North Clock Town | spawn 0 and 5: `REGION_CLOCK_TOWN` |
| `.room_01` | `_room_01` | 1200 | `0x0201`: type 1 POWER, flag 1 | 1 `0x8620` Woodfall | spawn 1, 6: WOODFALL |
| `.room_02` | `_room_02` | 0 | `0x0402`: type 2 WISDOM, flag 2 | 2 `0xB220` Snowhead | spawn 2, 7: SNOWHEAD |
| `.room_03` | `_room_03` | -1200 | `0x0603`: type 3 COURAGE, flag 3 | 3 `0x6A50` Zora Cape | spawn 3, 8: GREAT_BAY |
| `.room_04` | `_room_04` | -2400 | `0x0804`: type 4 KINDNESS, flag 4 | 4 `0x20B0` Ikana Canyon | spawn 4, 9: IKANA_CANYON |

Type and flag macros: `GREAT_FAIRY_GET_TYPE = p & 0xF`, `GREAT_FAIRY_GET_SWITCHFLAG = (p & 0xFE00) >> 9` (`ovl_Bg_Dy_Yoseizo/z_bg_dy_yoseizo.h:12-22`).

**Proof that room_00 is the town fountain (C/OVL, independent lines):**

1. North exit index 3 is `0x4600` (`Z2_BACKTOWN.c:76`) -> entrance table 0x23, entrance 0, layer 0 -> `sFairyFountainEntrance0[0] = { SCENE_YOUSEI_IZUMI, 0, 0x4102 }` (`z_scene_table.c:698-700`) -> `EntranceList0x000154[0] = { 0x00, 0x00 }` = spawn 0, **room 0** (scene.c:73). The engine loads that room (`src/code/z_room.c:549`).
2. Room_00 is the only room whose collision exit polygons carry exit+1 = 1, and exit 0 is `0xD630` = North entrance 3 (scene.c:86; section 2.4).
3. Room_00's Great Fairy and fairy group are type 0 `GREAT_FAIRY_TYPE_MAGIC`; `En_Elfgrp` documents type 0 as Clock Town (24 fairies in the fountain, the 25th scattered in town) (elfgrp.c:6-28, :177-219; `z_en_elfgrp.h:15-22`).
4. The pause map maps fountain spawns 0 and 5 to `REGION_CLOCK_TOWN` (`ovl_kaleido_scope/z_kaleido_map.c:532-535`, used at `:780-783` when `sceneId == SCENE_YOUSEI_IZUMI`).
5. Water box 0 is the only one with room index 0, spanning x 2101..2699 (scene.c:10207; section 7). ANN: map select "Fairy Fountain - Clock Town" = `ENTRANCE(FAIRY_FOUNTAIN, 0)` (`ovl_select/z_select.c:429-430`).

Rooms 01 to 04 sit 1200 units apart on x with the same shell (same 406-poly collision block, same DL layout, section 7) and are registered for completeness only. No transition actor or collision surface links any two rooms, and none of their exits reaches a city scene.

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:41-62; decode C)

All ten entries have raw rot `0x0007 / 0x5A00 / 0x007F` (yaw `(0x5A00 >> 7) & 0x1FF` = 180, facing -Z) and params `0x0FFF`: start mode `(p & 0xF00) >> 8 = 0xF` (`PLAYER_START_MODE_F`), bgCam `p & 0xFF = 0xFF` (none) (`include/z64player.h:17-38`).

| Spawn | Position | Room (entrance row) | Used by | Unity |
| --- | --- | --- | --- | --- |
| 0 | (2400, 20, 360) | 0 | North `0x4600` (walk-in); 24 u in front of the exit floor edge z 384, facing away from it | (24.00, 0.20, -3.60) |
| 1 | (1200, 20, 360) | 1 | external (NI) | n/a |
| 2 | (0, 20, 360) | 2 | external (NI) | n/a |
| 3 | (-1200, 20, 360) | 3 | external (NI) | n/a |
| 4 | (-2400, 20, 360) | 4 | external (NI) | n/a |
| 5 | (2400, -15, -454) | 0 | cutscene destination of script 0 (section 2.2); floor under it Y -15.2 (poly 79, pool edge), 7 u outside the water box | (24.00, -0.15, 4.54) |
| 6 .. 9 | (1200 / 0 / -1200 / -2400, -15, -454) | 1 .. 4 | cutscene destinations (6, 8) or none found (7, 9) | n/a |
| alt 0 | (2400, 20, -1021) | 0 | credits only (section 1.1) | n/a |

### 2.2 Entrance table to room (C)

| Entr | Table rows (`z_scene_table.c`) | Scene row (scene.c:73-82) | Room | Sender | Arrival cutscene (scene script, actor csId) |
| --- | --- | --- | --- | --- | --- |
| 0 | layer 0 `{.., 0, 0x4102}` L699; layer 1 `{.., 0, 0x8102}` L700 | `{0x00, 0x00}` | **0** | North exit [3] `0x4600` (layer 0); credits `z_demo.c:701-704` (layer 1) | default: none; alt: credits script (spawn 0, always) |
| 1 .. 4 | `{.., n, 0x4102}` L704, 708, 712, 716 | `{n, n}` | 1 .. 4 | other regions' scenes (NI); map select (ANN) | none |
| 5 | `{.., 5, 0x8183}` L720 | `{0x05, 0x00}` | **0** | script 0 (`nextEntrance 0x4650`, `CS_DESTINATION_DEFAULT` at frame 640, scene.c:6519, :131-408) | script 20 (`spawn 0x05`, `CS_SPAWN_FLAG_ALWAYS`, scene.c:6539), actor cs 29 |
| 6 | `{.., 6, 0x858B}` L724 | `{0x06, 0x01}` | 1 | script 1 (`0x4660`, destination frame 620) | script 17, cs 11 |
| 7 | L728 | `{0x07, 0x02}` | 2 | none found: script 2 carries `0x4670` but has no destination command | script 21, cs 30 |
| 8 | L732 | `{0x08, 0x03}` | 3 | script 3 (`0x4680`, destination frame 620) | script 18, cs 12 |
| 9 | L736 | `{0x09, 0x04}` | 4 | none found (no script carries `0x4690`) | script 22, cs 31 |

Mechanism (C): a `CS_DESTINATION_DEFAULT` command copies `scriptList[scriptIndex].nextEntrance` into `play->nextEntrance` (`src/code/z_demo.c:585-621`, line 601). On arrival, `CutsceneManager_FindEntranceCsId` picks the actor cutscene whose script's `spawn` equals `play->curSpawn` (`src/code/z_eventmgr.c:516-540`), and `Cutscene_HandleEntranceTriggers` starts it every time for `CS_SPAWN_FLAG_ALWAYS` (`z_demo.c:1546-1563`; struct `include/z64cutscene.h:593-607`). Entrance flag words beyond PORTAL_REGISTER's notes: NI.

### 2.3 Exit list (HDR scene.c:85-92, alt :10264-10271, identical) and consumers

| Exit idx | Raw | Decode (`>> 9` / `(>> 4) & 0x1F` / `& 0xF`) | Destination | Consumer |
| --- | --- | --- | --- | --- |
| 0 | `0xD630` | 0x6B / 3 / 0, `sNorthClockTownEntrance3` `{ SCENE_BACKTOWN, 3, 0x4102 }` (`z_scene_table.c:2319-2321`) | `loc.north_clock_town` spawn 3 (-1033, 292, -2008) yaw 90 params `0x0E0C` (`Z2_BACKTOWN.c:45`) | room_00 surface types 5, 6, polys 44-49 |
| 1 | `0x8620` | 0x43 / 2 / 0 | `ext.z2_21miturinmae` | room_01 types 14, 15, polys 450-455 |
| 2 | `0xB220` | 0x59 / 2 / 0 | `ext.z2_12hakuginmae` | room_02 type 23, polys 856, 857 |
| 3 | `0x6A50` | 0x35 / 5 / 0 | `ext.z2_31misaki` | room_03 type 32, polys 1262, 1263 |
| 4 | `0x20B0` | 0x10 / 11 / 0 | `ext.z2_ikana` | room_04 type 41, polys 1668, 1669 |
| 5 | `0x0000` | n/a | none | no surface has exit+1 = 6 and no fountain room actor reads `setupExitList`: padding (C) |

### 2.4 Collision surfaces carrying exits (C from scene.c:6655-8701; scratch `col.py`, `rooms.py`)

`exitIndexPlusOne` = bits 8-12 of `SurfaceType.data[0]`; conveyor speed = `data[1]` bits 18-20, direction bits 21-26 (`include/z64bgcheck.h:79-98`). All exit polygons are floors (normal (0, 32767, 0)), so the player exit check reads them (`ovl_player_actor/z_player.c:6405-6416`).

| Room | Surface type: data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Conveyor |
| --- | --- | --- | --- | --- | --- |
| 0 | 5: `0x00000101` / `0x000C1000` | 0 | 44, 45 (vIB flag 1 conveyor) | 2298..2498 / 20 / 384..542 | speed 3, dir 0 (binang 0, +Z = outward along the corridor) |
| 0 | 6: `0x00000101` / `0x00001000` | 0 | 46, 47, 48, 49 | 2305..2490 / 20 / 542..857 | none |
| 1 | 14 / 15 | 1 | 450-455 | 1098..1298 / 20 / 384..857 | type 14 speed 3 |
| 2 | 23 | 2 | 856, 857 | -102..98 / 20 / 384..542 | speed 3 |
| 3 | 32 | 3 | 1262, 1263 | -1302..-1102 / 20 / 384..542 | speed 3 |
| 4 | 41 | 4 | 1668, 1669 | -2502..-2302 / 20 / 384..542 | speed 3 |

Room_00's exit field covers the outer end of the entry corridor from z 384 to the corridor end at z 857; Unity x 22.98..24.98, z -8.57..-3.84, Y 0.20.

### 2.5 Transition actors and internal room links

None: no transition-actor command in either header (HDR). The five rooms never load together through a door; the room is fixed by the entrance row (C, `z_room.c:549`). There are no internal room portals to register.

### 2.6 Access from North Clock Town: cross-check with `north_clock_town.md` 2.4 and PORTAL_REGISTER

| Item | North dossier / PORTAL_REGISTER | This scene's data | Result |
| --- | --- | --- | --- |
| Trigger | surface type 4, polys 118, 119, exit 3 `0x4600`, floor, conveyor dir 48 (-X), x -1291..-1117, y 243..284 (`Z2_BACKTOWN.c:370`) | no door actor anywhere in this scene | agree: collision exit, no door, no schedule |
| Arrival | fountain entrance 0 -> spawn 0, room 0, (2400, 20, 360) yaw 180, start mode 0xF (PORTAL_REGISTER row `portal.north_clock_town.great_fairy_fountain_town`) | section 2.1, 2.2 row 0 | agree |
| Return | fountain exit 0 `0xD630`, surf 5, 6, polys 44-49 -> North spawn 3 (-1033, 292, -2008) yaw 90, mode 0xE (PORTAL_REGISTER row `portal.great_fairy_fountain_town.north_clock_town`) | section 2.3, 2.4 | agree; settles LOCATION_REGISTER 4.2 "exit polygon NI" |
| Arrival inside trigger | PORTAL_REGISTER 6: none | spawn 0 z 360 vs exit floor z >= 384 | agree (24 u clearance) |
| Upper-west-level foot access in North | North U-03 UNKNOWN | not this scene | dependency stays with North |

### 2.7 Portal IDs (registered names, `shared/city/ids.json`)

| Portal ID | Source evidence | Arrival back here |
| --- | --- | --- |
| `portal.north_clock_town.great_fairy_fountain_town` (incoming, owned by North) | North surface 4, exit 3 `0x4600` | spawn 0, room 0 |
| `portal.great_fairy_fountain_town.north_clock_town` | room_00 surf 5, 6, exit 0 `0xD630` | North spawn 3 |
| `portal.great_fairy_fountain_town.z2_21miturinmae`, `.z2_12hakuginmae`, `.z2_31misaki`, `.z2_ikana` | rooms 01-04, exits 1-4 | not city scope (external boundary) |

## 3. Object banks (HDR roomN.c object lists; IDs from `include/tables/object_table.h`)

| Room / header | Banks | Consumer (profile `objectId`, OVL) |
| --- | --- | --- |
| room_00 default and alt (room0.c:33-36, :88-91) | `OBJECT_DY_OBJ` 0x008; `OBJECT_GI_MASK14` 0x20A | Bg_Dy_Yoseizo (yoseizo.c:30) and its En_Dy_Extra beam child (`ovl_En_Dy_Extra/z_en_dy_extra.c:24`); Demo_Getitem mask draw via `Object_GetSlot` (getitem.c:32, :59-63) |
| room_01, room_02, room_03, both headers | `OBJECT_DY_OBJ` | Bg_Dy_Yoseizo |
| room_04, both headers | `OBJECT_DY_OBJ`; `OBJECT_GI_SWORD_4` 0x1FB | Bg_Dy_Yoseizo; Demo_Getitem sword (`0xFE01`) |

Implicit: `gameplay_keep` (En_Elfgrp, En_Elforg and its `gStrayFairySkel` at 0x2CA98, Elf_Msg3, En_Test4, Demo_Getitem and Demo_Effect profiles) and `gameplay_field_keep` (scene special file; no room_00 consumer found). XML: `object_dy_obj` = flex `gGreatFairySkel` (27 limbs), 10 animations, 30 DLs, 20 textures, 1 texture animation `gGreatFairyAppearenceTexAnim`; `object_gi_mask14` = 2 DLs, 4 textures (`assets/xml/objects/object_dy_obj.xml`, `object_gi_mask14.xml`).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): actor ID `id & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF`, converted to binang unless the ID carries the axis flag (`0x8000` rotY, `0x4000` rotX, `0x2000` rotZ), in which case it arrives as a plain integer; csId `rawRotY & 0x7F` (0x7F none, 3570-3574); half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 means all (3576-3580). Macro `SPAWN_ROT_FLAGS(r, f) = (r << 7) | f` (`include/z64scene.h:16`).
- Day 1 noon bit `0x080` (`z_actor.c:2491-2499`). Every room actor in this scene has mask `0x3FF`, so presence never depends on day or time; it depends on save state only.
- Switch flags: `Flags_GetSwitch` indexes `switches[flag >> 5]` (`z_actor.c:778-783`). `switches[0..1]` reload from the cycle flags; the whole actor context, including `switches[2]` (flags 64-95), is zeroed at scene init (`z_actor.c:2508`, `:2521-2523`), so flag 64 lasts one visit.

Classes as in North; "D1d" = mask includes Day 1 daytime.

### 4.2 room_00 default header (5 entries, HDR room0.c:38-54, decode C)

| # | Actor (table ID) | Position (Unity) | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_DEMO_GETITEM` (0x165) | (2403, -20, -567) (24.03, -0.20, 5.67) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0xFE0F | static-mesh (cutscene prop; draw NULL at rest) | yes, not drawn |
| 1 | `ACTOR_BG_DY_YOSEIZO` (0x130) | (2400, -20, -750) (24.00, -0.20, 7.50) | 0x0007 / 0x000E / 0x007F | 0 / 0 / 0 | 0x3FF | 14 | 0x0000 | posed-skeletal (cutscene only) | yes, not drawn |
| 2 | `ACTOR_EN_ELFGRP \| 0x2000` (0x153) | (2400, -20, -750) | 0x0007 / 0x000E / 0x207F | 0 / 0 / Z = integer 64 | 0x3FF | 14 | 0x0000 | controller-invisible (spawns stray fairies) | yes |
| 3 | `ACTOR_EN_TEST4` (0x15A) | (2401, 20, 717) (24.01, 0.20, -7.17) | 0x0007 / 0x0020 / 0x007F | 0 / 0 / 0 | 0x3FF | 32 | 0x0100 | controller-invisible | yes |
| 4 | `ACTOR_ELF_MSG3 \| 0xE000` (0x146) | (2400, 20, -560) (24.00, 0.20, 5.60) | 0x3207 / 0x007F / 0x327F | X = 100, Y = 0, Z = 100 (integers) | 0x3FF | none | 0x0A1B | controller-invisible (Tatl trigger) | yes |

### 4.3 room_00 alternate header 1 (5 entries, HDR room0.c:93-109) - credits only

All `ACTOR_BG_DY_YOSEIZO`, raw rot `0x0007 / 0x007F / 0x007F`, mask 0x3FF, no csId. Params / position: `0x0004` (type 4, flag 0) (2229, -20, -1764); `0x0203` (type 3, flag 1) (2308, -18, -1781); `0x0402` (type 2, flag 2) (2398, -23, -1800); `0x0601` (type 1, flag 3) (2477, -12, -1789); `0x0800` (type 0, flag 4) (2563, -20, -1776). Excluded from city scope.

### 4.4 rooms 01-04 (default headers; alt headers have no actor list)

| Room (lines) | Entries | Notes |
| --- | --- | --- |
| room_01 (room1.c:37-47) | Bg_Dy_Yoseizo `0x0201` cs 16 and En_Elfgrp\|0x2000 `0x0201` cs 16 at (1200, -20, -750); En_Test4 `0x0160` cs 33 at (1194, 20, 688) | rotZ raw 0x007F -> Elfgrp rot switch 0 (none) |
| room_02 (room2.c:37-47) | same pattern, `0x0402`, cs 18; Test4 cs 34 at (-6, 20, 694) | |
| room_03 (room3.c:37-47) | same, `0x0603`, cs 20; Test4 cs 35 at (-1206, 20, 668) | |
| room_04 (room4.c:38-51) | same, `0x0804`, cs 22; Test4 cs 36 at (-2403, 20, 661); Demo_Getitem `0xFE01` (sword) at (-2277, -20, -569) | |

### 4.5 Overlay findings per actor type (OVL)

| Actor | Init: scale, pivot, children | Draw | Presence / visibility gating at Day 1 noon, fresh save |
| --- | --- | --- | --- |
| Demo_Getitem | `p & 0xF = 0xF` (not 1) -> mask item: `OBJECT_GI_MASK14`, `GID_MASK_GREAT_FAIRY`, cue type `CS_CMD_ACTOR_CUE_110`; scale 0.25 (getitem.c:32-66) | profile draw NULL; drawn only on cue 110 ids 2 or 4, translated by the cue, id 4 bobs 15 u (getitem.c:79-112) | present, invisible |
| Bg_Dy_Yoseizo | flex `gGreatFairySkel`, start anim ShowingItem, **scale 0**, action `func_80A0BB08` (yoseizo.c:63-80) | profile draw NULL (yoseizo.c:35); draw is set only on cue 103 id 2 (appear), 7 (laid down, scale 0.035, Y +40) or 9 (scale 0.01, spin-attack teacher) (yoseizo.c:511-537); type 0 uses the orange appearance row (yoseizo.c:565-607) | present, invisible until a cutscene |
| En_Elfgrp | type `p & 0xF = 0` MAGIC; switch flag `(p & 0xFE00) >> 9 = 0`; rot switch `home.rot.z = 64` (`z_en_elfgrp.h:11-13`) | none | fresh save: spawns 24 stray fairies, talk mode (section 5.3) |
| En_Elforg (spawned x24 by Elfgrp) | params `STRAY_FAIRY_PARAMS(0, 0, 1) = 0x0001` (type 1 FAIRY_FOUNTAIN, area 0 Clock Town); spawned at Elfgrp pos +20 Y, +-10 x/z; home (2400, 0, -750) (elfgrp.c:283-317); orbit radius 50..150 u, speed 1..3 (elforg.c:115-120, :209-235); scale 0.01 | flex `gStrayFairySkel` (gameplay_keep 0x2CA98), billboard, XLU, Clock Town texture animation (elforg.c:631-667) | present at noon; skeletal billboard effect |
| En_Test4 | three-day controller; stars `(p >> 10) * 100 = 0`, bell volume index `(p >> 5) & 0xF = 8` (room_00) / 11 (rooms 1-4) (`z_en_test4.h:8-9`); csId 32 is its night cut, day cut = `additionalCsId` of entry 32 = 32 (`z_en_test4.c:322-337`) | NULL | always |
| Elf_Msg3 | rot X/Z integers 100 -> scale (4, 4, 4); trigger box `abs(dx) < 400`, `0 <= dy < 400`, `abs(dz) < 400` around its position = x 2000..2800, y 20..420, z -960..-160 (elfmsg3.c:69-88, :101-109) | NULL | killed at init if switch 10 is set (elfmsg3.c:58-65); fresh save: armed |

### 4.6 Counts

| | r00 d | r00 a1 | r01 d | r02 d | r03 d | r04 d | r01-r04 a1 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Objects / actors (HDR) | 2 / 5 | 2 / 5 | 1 / 3 | 1 / 3 | 1 / 3 | 2 / 4 | 1-2 / 0 |
| Day 1 daytime by mask | 5 | 5 | 3 | 3 | 3 | 4 | 0 |
| Drawn at rest (no cutscene) | 0 (+24 spawned fairies) | 0 | 0 (+fairies) | 0 | 0 | 0 | n/a |

Counts agree with LOCATION_REGISTER (r00 d 2/5, a1 2/5; r01-r03 d 1/3, a1 1/0; r04 d 2/4, a1 2/0; 10/1 spawns; 6/6 exits, last 0x0000; 10/10 entrances).

## 5. Venue mechanics index (INT-13 scope)

Naming note: the dispatch names `ovl_En_Elf` as the stray fairy. In the decomp the stray fairy is `ovl_En_Elforg`; `ovl_En_Elf` is Tatl and ordinary healing fairies and enters here only as the Tatl-hint consumer (5.7). No `ovl_Dm_*` actor appears in any fountain room list; the Great Fairy is `ovl_Bg_Dy_Yoseizo`, her group manager `ovl_En_Elfgrp`, the reward prop `ovl_Demo_Getitem`.

### 5.1 State that drives the town fountain

| State | Meaning | Set / cleared | Persists across Song of Time? |
| --- | --- | --- | --- |
| `WEEKEVENTREG_08_80` (`include/z64save.h:748`) | Clock Town stray fairy caught and held | set when its pickup text closes (elforg.c:432-438) | no: weekEventReg[8] mask keeps only the doggy-race heart piece (`z_sram_NES.c:88`) |
| `permanentSceneFlags[SCENE_YOUSEI_IZUMI].unk_14` bit 0 | town fountain restored (25 fairies) | set/cleared by `EnElfgrp_SetFountainFairiesCount` (elfgrp.c:265-276); read at :246-252 | no: `unk_14 = 0` for every scene at end of cycle (`z_sram_NES.c:466`) |
| `weekEventReg[9]` bit `1 << type` (`WEEKEVENTREG_09_01` for town, `z64save.h:751`) | talked to the fairy group once | elfgrp.c:591, :613, :632 | no (`z_sram_NES.c:89`, mask 0) |
| switch0 flag 0 (Elfgrp/Great Fairy flag, params `>> 9`) | "award already given this visit" gate for the magic grant | unset before a return cutscene (elfgrp.c:481), set before a restored-visit cutscene (:525); read yoseizo.c:320 | not in the persistence mask (only bit 10 is) |
| switch flag 64 (Elfgrp `home.rot.z`) | restored cutscene already played this visit | set :531-533; read :184-186 | per visit (`switches[2]` zeroed at scene init) |
| switch0 flag 10 (Elf_Msg3 `(p & 0x7F00) >> 8`) | Tatl fountain message already given | elfmsg3.c:135-138; kill check :58-65 | **yes** (scene persistent mask `1 << 10`) |
| `isMagicAcquired`, `magicFillTarget` | Magic Power | yoseizo.c:322-326 | save data (not reset by the cycle code read here) |
| `INV_CONTENT(ITEM_MASK_DEKU)`, `INV_CONTENT(ITEM_MASK_GREAT_FAIRY)` | first-cycle test and mask ownership | read elfgrp.c:187-205; mask given by `Item_Give` :483-485, :527-529 | inventory |
| `GET_PLAYER_FORM` | Deku auto-talk | elfgrp.c:626 | n/a |

Totals: `STRAY_FAIRY_TOTAL 25`, `STRAY_FAIRY_SCATTERED_TOTAL 15` (`z64save.h:1760-1761`). For town the count in the fountain is 24 or 25 only (elfgrp.c:246-252); `EnElfgrp_GetHeldFairiesCount` returns 0 for type 0 (:227-236), so "held" for town is `WEEKEVENTREG_08_80` alone.

### 5.2 The Clock Town stray fairy (outside this scene)

| Where | Entry | Mask | Params | Rule |
| --- | --- | --- | --- | --- |
| Laundry Pool | `Z2_ALLEY_room_00.c:54-56`, (-2080, -111, 446) | 0x2AA every daytime | `0xFE03` | type `p & 0xF = 3` `STRAY_FAIRY_TYPE_CLOCK_TOWN`, area 0 (`z_en_elforg.h:8-31`) |
| East Clock Town | `Z2_TOWN_room_00.c:63-66` (also alt1 :344-347), (663, 199, -335) | 0x155 every night | `0xFE03` | same |

Killed at init when `WEEKEVENTREG_08_80` is set (elforg.c:79-85). Pickup when `xzDist < 30` and `-68 < dy < 12`: `Health_ChangeBy(0x30)` (3 hearts), `Flags_SetSwitch(0x7F)` (a temporary flag: `switches[3]` is cleared on room change, `z_actor.c:3303`), message `0x579`, then `EnElforg_ClockTownFairyCollected` freezes the player and sets `WEEKEVENTREG_08_80` when the text closes (elforg.c:470-496, :424-449). With the Great Fairy's Mask worn it homes on the player at speed 5 (elforg.c:458-465, :513-521). Cross-reference: `docs/city/districts/laundry_pool.md` a04, `east_clock_town.md` a1.

### 5.3 En_Elfgrp town state machine (type 0; elfgrp.c:177-219)

The csId offset is applied by walking `additionalCsId` from the actor's csId 14 (`EnElfgrp_SetCutscene`, elfgrp.c:87-96; `z_eventmgr.c:484-489`). Chain in `CutsceneEntryList_0116BC` (scene.c:6545-6584): 14 -> 15 -> 10 -> 9 -> 13 -> 24 -> 37 -> none.

| Condition at Elfgrp init | Action | Offset -> csId -> script | Award / side effect | Script content (scan) |
| --- | --- | --- | --- | --- |
| restored (`unk_14 & 1`) and switch 64 set | `EnElfgrp_DoNothing` | none | none (same visit) | n/a |
| restored, Great Fairy's Mask owned | `func_80A3A520`: queue when `xzDist < 350`, set switch 0 (:519-537) | +4 -> 13 -> 7 | heal and magic refill only (switch 0 set) | 550 frames, texts 0x591, 0x592 |
| restored, no Deku Mask (first cycle) | same | +5 -> 24 -> 8 | heal only | 550 frames, texts 0x593, 0x594 |
| restored, Deku Mask, no Great Fairy's Mask | same, `ELFGRP_STATE_1` | +6 -> 37 -> 23 | `Item_Give(ITEM_MASK_GREAT_FAIRY)` at start (:527-529) | 1174 frames, `NA_BGM_GET_NEW_MASK`, mask prop cues, texts 0x593, 0x58F, 0x590, 0x86, 0x592 |
| fairy held (`08_80`), Deku Mask and Great Fairy's Mask owned | spawn 24, `func_80A3A398` (queue < 350, unset switch 0, :477-499) | +2 -> 10 -> 6 | magic if not yet acquired | 874 frames, texts 0x57E, 0x57F |
| held, Deku Mask, no Great Fairy's Mask | same, `ELFGRP_STATE_1` | +3 -> 9 -> 19 | Great Fairy's Mask at start (:483-485); magic if missing | 1174 frames, mask cues, texts 0x58E, 0x58F, 0x590, 0x86, 0x592 |
| held, no Deku Mask, magic acquired | same | +1 -> 15 -> 5 | none new (INF: unreachable in normal play, section 5.6) | 874 frames, texts 0x57C, 0x57D |
| held, no Deku Mask, no magic (first-cycle return) | same | +0 -> 14 -> 0 | **Magic Power**; destination entrance 5 | 1023 frames; text 0x57A; `CS_DESTINATION` frame 640 -> fountain entrance 5 -> script 20 replays the grant tail (texts 0xC8, 0x57B, light settings 15/16/10) |
| neither held nor restored (fresh save) | spawn 24, `func_80A3A8F8`, talk enabled (:209-218) | none | talk heals: `Magic_Add(MAGIC_FILL_TO_CAPACITY)`, `healthAccumulator = 0x140` (:542-560) | text 0x578 first time, 0x580 after `WEEKEVENTREG_09_01` |

Return cutscene cues (`CS_CMD_ACTOR_CUE_100`, elfgrp.c:445-475): cue 2 spawns 1 returning fairy at the player for town (:454-456) and sets the count to 25 (:461); cue 3 summons the fairies, then they shrink and vanish and `Demo_Effect` light `type + DEMO_EFFECT_TYPE_LIGHT_BASE` spawns (:403-443). A restored-path Elfgrp that sees cue 103 already running (the script-20 entrance cutscene) goes idle instead of queuing again (:520-521).

Talk geometry (fresh save): the group offers talk only when `xzDist < 30` from (2400, -750); Deku form gets the textbox automatically and is frozen; other forms get `Actor_OfferTalk(100)` (elfgrp.c:625-639).

### 5.4 Great Fairy (Bg_Dy_Yoseizo) cue table and awards

| Cue 103 id | Effect | Lines |
| --- | --- | --- |
| 2 | appear: draw on, spin-lay anim, 30 fast radiant particles | yoseizo.c:512-516, :429-436 |
| 4 | finish growing to scale 0.035 at home Y +40, lay-down transition | :414-427, :210-220 |
| 5 | start giving: anim, spawn `En_Dy_Extra` beam at player Y +200 (params = type), timer 120 | :395-404, :367-385 |
| timer 60 | `if (!Flags_GetSwitch(flag 0))`: type 0 sets `isMagicAcquired = true`, `magicFillTarget = MAGIC_NORMAL_METER`; HUD visibility 9 | :315-347 |
| timer 50 | `healthAccumulator = 0x140`, `Magic_Add(MAGIC_FILL_TO_CAPACITY)` | :356-359 |
| 7 / 8 | laying-down / showing-item loops | :287-313, :518-527 |
| 6 | vanish: shrink, white-out, `Demo_Effect` light | :259-270, :222-257 |

Types 2 and 3 grant double magic and double defense in their own rooms (:329-341); the spin attack (type 1) and sword (type 4) are given by Elfgrp (elfgrp.c:132-158, :487-493). Out of city scope.

### 5.5 Great Fairy's Mask

Given only by the town fountain, only when the Deku Mask is owned (5.3 rows +3 and +6), by `Item_Give` when the cutscene starts. The prop is room_00 a00 Demo_Getitem (`OBJECT_GI_MASK14`), drawn on cue 110 ids 2 and 4 (scripts 19 and 23 move it from (2427, 169, -701) to (2400, 90, -454)). Worn, it pulls free stray fairies toward Link (5.2) and plays `NA_SE_SY_FAIRY_MASK_SUCCESS` (elforg.c:513-517). Credits: owning it selects the layer-1 credits shot here (1.1).

### 5.6 Deku versus Human form

| Test | Source | Effect |
| --- | --- | --- |
| `GET_PLAYER_FORM == PLAYER_FORM_DEKU` | elfgrp.c:626-632 | fresh-save group talk is automatic within 30 u; other forms must accept a talk offer |
| `INV_CONTENT(ITEM_MASK_DEKU) != ITEM_MASK_DEKU` | elfgrp.c:189, :199 | stands in for "first cycle": chooses scripts 0/5 (return) and 8 (restored) and withholds the mask |
| Form during cutscenes | player cue lists, e.g. `PLAYER_CUEID_55` in script 0 (scene.c:131-408) | animation per form: NI |

INF: the inventory test makes "no Deku Mask but magic acquired" (+1, script 5) unreachable without debug state, since the Deku Mask arrives with the Ocarina at the end of the first cycle. No source line establishes this; it is game-progression inference.

### 5.7 Tatl messages

| Source | Rule | Text |
| --- | --- | --- |
| Elf_Msg3 a04 (`0x0A1B`) | `tatlTextId = -0x200 - (p & 0xFF)` (bit 0x8000 clear) (elfmsg3.c:93-99); negative id makes Tatl's talk offer auto-accepted (`ovl_En_Elf/z_en_elf.c:1472-1474`; `z_player.c:7993-8005`); talking sets switch 10 and kills the trigger (elfmsg3.c:133-139) | `0x21B` once per file (flag 10 persists) |
| `QuestHint_GetTatlTextId` (`src/code/z_elf_message.c:19-52`) | first-cycle hints (no Ocarina, day >= 1): fairy not held -> `0x21E` if `WEEKEVENTREG_09_01`, else none inside the fountain, else `0x21D`; held but magic missing -> `0x21F` | ids only |

Message text is not transcribed (ROM content).

### 5.8 Camera and cutscene setup

| Item | Value | Source |
| --- | --- | --- |
| `BgCamInfo` | 3: [0] `0x0000` `CAM_SET_NONE`, [1] `0x003F` `CAM_SET_ROOM0`, [2] `0x0011` `CAM_SET_DUNGEON0` (room_02 only) | scene.c:6649-6653; `include/z64camera.h:147, 164, 210` |
| room_00 floor cameras | every walkable floor type (2, 4, 5, 6, 7) uses bgCam 1 ROOM0; types 0, 3, 8 use bgCam 0, which `Camera_GetBgCamIndex` treats as "no camera data" | section 7; `src/code/z_camera.c:794-805` |
| Actor-cutscene cameras | 5 x setting 31 `CAM_SET_FIXED1` (`z64camera.h:178`), 3 points each; room_00: eye (2328, 67, -257), rot (-1820, 31130, 0) binang = pitch -10.0 deg, yaw 171.0 deg, fov -1 (default) | scene.c:6598-6622; `include/z64bgcheck.h:133-142`; `z_camera.c:4720-4724` |
| Who uses cs camera 0 | actor cutscene 32 (`csCamId 0`), En_Test4's day/night transition cut in room_00 | scene.c:6578; `z_en_test4.c:326-336` |
| Town actor cutscenes | 14, 15, 10, 9, 13, 24, 37 (Elfgrp chain; priorities 500-505, entry 37 3700; letterbox 32; all scripted; entry 14 at scene.c:6560), 29 (entrance-5 arrival), 32 (Test4) | scene.c:6545-6584 (entry n at line 6546 + n) |
| Town scripts | 0, 5, 6, 19 (return), 7, 8, 23 (restored), 20 (arrival at spawn 5); 3 to 17 camera splines each, scripted fade/destination as listed in 5.3 | scene.c:6518-6543; scratch `cs_out.txt` |
| Scripted cue channels | 100 (Elfgrp), 103 (Great Fairy), 110 (mask prop), player cues | cue handlers above |
| Unused here | actor cutscenes 0-8 (global camera data, no script) have no room_00 actor pointing at them (INF unused or runtime) | HDR |

### 5.9 Time behaviour

No schedule, door or time predicate gates the fountain (no transition actor; North's exit surface has no actor). Every room actor has mask 0x3FF. Room `TIME_SETTINGS(255, 255, 3)` keeps the normal clock (with the no-Ocarina speed rule of `z_scene.c:399-402`). Lighting is in settings mode, so time of day does not change the room light (1; INF from the light-mode enum). En_Test4 runs the 06:00/18:00 transitions inside the room with the fixed camera above. The only time dependence of the venue's story is outside it: the Clock Town fairy is in Laundry Pool by day and East Clock Town by night (5.2).

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers, room_00)

The fountain is open at Day 1 noon (no door, no schedule; section 5.9). Assumptions to record in the profile: fresh save (no weekEventReg, scene or switch flags; `unk_14 = 0`), Clock Town fairy not held, no Deku Mask and no Great Fairy's Mask in inventory, magic not acquired. Form: the baseline profile says Human, but Human with no Deku Mask item is not a reachable first-cycle state; the static geometry is the same either way (U-07).

| Item | Source | N64 / Unity | Geometry needed | Notes |
| --- | --- | --- | --- | --- |
| Room shell, opaque | room_00 cullable entries 1, 2, 6, 7, 8: `DL_0012D0`, `DL_003D38`, `DL_000980`, `DL_002AB8`, `DL_0039D0` (all called by XML DL `0x3DF0`) | world-space vertices, no transform | room DLs + scene textures (already extracted) | entry 2 has radius -146 (sorted last, `z_room.c:182-186`) |
| Room shell, XLU | entries 0, 3, 4, 5: `DL_004260`, `DL_004760` (seg 8), `DL_004CD0` (seg 9), `DL_004FB0` (seg 10) (XML DL `0x50B0`) | same | same | entry 5 is centered at (2400, -20, -760) r 312, the water surface height: INF water surface |
| Animated materials | 3 two-texture scrolls on segments 8-10 | n/a | scroll params scene.c:6628-6647 | static import must label them frozen at t = 0 |
| Water box | (2101..2699, -20, -1059..-461) | Unity x 21.01..26.99, z 4.61..10.59, Y -0.20 | collision only | pool floor Y -30..10 |
| 24 stray fairies | Elfgrp a02 children, `0x0001` | around (2400, 0, -750) (24.00, 0.00, 7.50), radius 50-150 | `gStrayFairySkel` + Clock Town tex anim (gameplay_keep) | unimplemented in a static pass; any proxy must be labeled |

**Present but not drawn without a cutscene:** a00 Demo_Getitem (draw NULL), a01 Bg_Dy_Yoseizo (scale 0, draw NULL). **No geometry:** a02 Elfgrp, a03 En_Test4, a04 Elf_Msg3. **Excluded by mask:** none. Extraction for the shell: none beyond the existing scene C (section 9 lists the later banks).

## 7. Collision summary (C from scene.c:6649-10222; scratch `col.py`, `rooms.py`, `floors.py`)

One collision header serves all five rooms: 1500 vertices, 2030 polygons, 45 surface types (9 per room), 5 water boxes, 3 camera entries; header bounds (-2772, -32, -1121)..(2770, 369, 857). Polygons split by x into five identical-size blocks of 406: room_00 = polys 0-405 (room_01 406-811, room_02 812-1217, room_03 1218-1623, room_04 1624-2029; INF ordering from centroids, verified by count).

### 7.1 room_00

| Measure | Value |
| --- | --- |
| Polys / vertices referenced | 406 / 297 |
| Vertex extents | (2028, -32, -1121)..(2770, 369, 857); Unity x 20.28..27.70, y -0.32..3.69, z -8.57..11.21 |
| Kinds (floor ny > 0.5, ceiling ny < -0.8) | 121 floors, 235 walls, 50 ceilings |
| Floor slopes (10-deg bins) | 0: 68, 10: 24, 20: 2, 30: 2, 40: 24, 50: 1 |
| Materials | stone (2): 400; dirt (0): 6 (types 5 and 6, the exit floors) |
| Water box | 1: x 2101..2699, z -1059..-461, surface Y -20, props `0x00000A00` (bgCam 0, light 10, room 0) (`include/z64bgcheck.h:34-46`) |
| Exit surfaces | types 5, 6, polys 44-49 (section 2.4) |
| Poly flags | 156 ignore-camera (vIA 1); polys 44, 45 conveyor (vIB 1) |
| Floor property 9 | type 7, 22 floors at Y 32..42 around the rim (x 2059..2740, z -1090..-414): leaving them suppresses auto-jump and the ledge check (`z_player.c:7539`, `:7569`) |
| Wall type 1 | type 8, 4 walls at Y 2..42, z -479..-379 (the front step faces): `WALL_FLAG_0`, which blocks ledge climbing (`src/code/z_bgcheck.c:18-27`, `:4206-4207`; `z_player.c:11835`) |
| Light index per surface | types 0, 2, 7, 8: 5; types 1, 3, 4, 5, 6: 0 |
| No | hookshot, soft, horse-block, floor-type, deku-flower flags |

| Floor group (centroid Y) | Polys | Types | Extent x / y / z | Reading |
| --- | --- | --- | --- | --- |
| Pool (< 0) | 58 | 2 | 2028..2770 / -30..10 / -1121..-379 | basin under the water box, about 10 u deep at the bottom |
| Corridor and step (0-25) | 14 | 0, 2, 4, 5, 6 | 2232..2569 / 6..20 / -420..857 | entry corridor Y 20 from z 857 to about -219, step down at z -379..-420 |
| Rim walkway (25-60) | 44 | 2, 7 | 2028..2770 / 32..42 / -1120..-379 | raised ring around the pool |
| High (>= 60) | 5 | 2 | 2172..2506 / 141..181 / -699..-537 | small sloped tops (25-54 deg) on the fountain sculpture: INF not a route |

Corridor shell: type 3 walls (18) and type 1 ceilings (8) at x 2298..2498, Y 20..260, z -219..857; chamber ceilings type 2 (42) at Y 123..168 over z -976..-537.

### 7.2 rooms 01-04

Same counts per room (406 polys; 121/235/50; slopes and materials identical), shifted by -1200 x per room. Differences: room_02 uses bgCam 2 (DUNGEON0) where the others use 0, and wall type 2 instead of 1; rooms 02-04 exit fields are 2 polys (z 384..542) where rooms 00 and 01 have 6 (z 384..857).

## 8. Materials and textures (C; counts only, nothing copied)

| Item | Count |
| --- | --- |
| Scene PNGs | 10, all in the scene file: rgba16 6, i8 2, i4 1, ia8 1; no TLUT |
| Room textures | none; every room references the same 10 scene textures |
| Room `Gfx` lists / `Vtx` arrays | 11 / 9 per room (9 cullable entry DLs + 2 XML DLs) |
| Room shape | cullable, 9 entries: 5 opaque, 4 XLU |
| Raw triangles in source | room_00: 291 `gsSP2Triangles` + 3 `gsSP1Triangle` = 585 |
| Render modes (room_00) | 4 `G_RM_FOG_SHADE_A, G_RM_AA_ZB_OPA_SURF2`; 1 `G_RM_PASS, G_RM_AA_ZB_XLU_DECAL2`; 1 `G_RM_FOG_SHADE_A, G_RM_ZB_XLU_SURF2`; 3 `G_RM_FOG_SHADE_A, G_RM_AA_ZB_XLU_SURF2` |
| Animated materials | 3 (segments 8, 9, 10) |
| `object_dy_obj` (later) | 20 textures: ci8 7, rgba16 9, i8 3, i4 1 (XML) |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_YOUSEI_IZUMI` and its 5 rooms (C above). The Day 1 noon shell needs nothing more. Binaries exist under `extracted/n64-us/baserom/` for `object_dy_obj` (116,928 bytes) and `object_gi_mask14` (8,064 bytes) (checked by `stat`).

| Priority | Banks | Why |
| --- | --- | --- |
| Optional for Day 1 noon | `gameplay_keep` | stray fairy skeleton and texture animation (24 fairies) |
| Mechanics pass (ROOM-03) | `object_dy_obj`, `object_gi_mask14` | Great Fairy, beam, particle DLs; mask prop |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as North section 8):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_dy_obj','object_gi_mask14','gameplay_keep')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

`gameplay_keep` has not been extracted with this tool before (North section 8 notes the same); check its output size and warnings separately.

## 10. Proposed region record

**Proposal, schema not frozen when the REF-09 brief was written; PIPE-01 has since frozen schema 1.** Field names follow North section 9 so `tools/reference/region_from_dossier.py` can lift the block. Checked in scratch: lifted with that script's `normalize()` and validated with `validate_city.py`'s `validate_region_file()` against the committed schema and `ids.json` (output in scratch, nothing written under `shared/`).

```json
{
  "_status": "proposal, schema not frozen at REF-09 brief; lifts to schema 1 via region_from_dossier.py",
  "schemaVersion": "proposal-0",
  "regionId": "loc.great_fairy_fountain_town",
  "displayLabel": "Fairy's Fountain (Clock Town)",
  "source": {
    "sceneSymbol": "Z2_YOUSEI_IZUMI",
    "sceneEnum": "SCENE_YOUSEI_IZUMI",
    "sceneTableIndex": "0x26",
    "entranceTableSceneIndex": "0x23",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0)",
    "rooms": { "loc.great_fairy_fountain_town.room_00": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event, switch or scene flags (unk_14 = 0)", "Clock Town stray fairy not held (WEEKEVENTREG_08_80 clear)", "no Deku Mask and no Great Fairy's Mask in inventory", "magic not acquired", "player form per profile (Human); first-cycle play is Deku (dossier U-07)"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_YOUSEI_IZUMI", "bytes": 144320, "sha256": "dec3850e4e685e2553a6ffdbe75424560192f9c0540ba2144f94ff6b9429676d" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_YOUSEI_IZUMI_room_00", "bytes": 20704, "sha256": "4def5b3cd8c493af90048ecf29c28d9a0c3e807cf549d3da480a6e4b3ee639a6" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_YOUSEI_IZUMI/Z2_YOUSEI_IZUMI.c", "bytes": 748454, "sha256": "32b696d0c11e21897aca2ad7720e1b2e27515c10910df69a695efda31d488a32" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_YOUSEI_IZUMI/Z2_YOUSEI_IZUMI_room_00.c", "bytes": 34103, "sha256": "f13242b1a0e511db3cd302f7133b55b482b2b9dc1d8f2273f8e52f24e365d3ba" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_YOUSEI_IZUMI/Z2_YOUSEI_IZUMI.xml", "bytes": 1576, "sha256": "3c093a1f8750a7e4f8295adc667f05821d2ada4cc6a8e40c03b15af69e75a7a1" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedFrame": { "with": [], "status": "not_shared", "evidence": "own scene; five fountain rooms laid out 1200 u apart on x (room_00 at x 2400); no exterior shares this frame" } },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-2772, -32, -1121], "max": [2770, 369, 857] },
    "sourceCollisionVertices": { "min": [2028, -32, -1121], "max": [2770, 369, 857] },
    "unityCollisionVertices": { "min": [20.28, -0.32, -8.57], "max": [27.70, 3.69, 11.21] }
  },
  "water": [
    {
      "id": "water.great_fairy_fountain_town.pool",
      "sourceWaterBox": { "xMin": 2101, "xMax": 2699, "zMin": -1059, "zMax": -461, "surfaceY": -20, "properties": "0x00000A00", "bgCamIndex": 0, "lightIndex": 10, "room": 0, "disabled": false },
      "unity": { "x": [21.01, 26.99], "z": [4.61, 10.59], "surfaceY": -0.20 },
      "poolFloorY": [-30, 10],
      "bankFloorY": [32, 42],
      "surfaceMesh": "INFERRED: room XLU entry 5 DL_004FB0 centered at Y -20, segment 10 two-texture scroll",
      "behavior": "UNKNOWN: shallow wading (about 10 u over the deepest floor); gameplay decision"
    }
  ],
  "anchors": [
    { "id": "anchor.gff.spawn0_entry", "source": [2400, 20, 360], "unity": [24.00, 0.20, -3.60], "status": "proposed, not surveyed" },
    { "id": "anchor.gff.spawn5_pool_edge", "source": [2400, -15, -454], "unity": [24.00, -0.15, 4.54], "status": "proposed, not surveyed" },
    { "id": "anchor.gff.fairy_center", "source": [2400, -20, -750], "unity": [24.00, -0.20, 7.50], "status": "proposed, not surveyed" },
    { "id": "anchor.gff.exit_field_edge", "source": [2398, 20, 384], "unity": [23.98, 0.20, -3.84], "status": "proposed, not surveyed" }
  ],
  "rooms": [ { "id": "loc.great_fairy_fountain_town.room_00", "sourceRoom": "Z2_YOUSEI_IZUMI_room_00", "roomShape": "cullable, 9 entries", "displayLists": 11 } ],
  "components": [
    { "sourceKey": "Z2_YOUSEI_IZUMI/h0/room_00/a02/child0-23", "actorId": "0x1B0", "params": "0x0001", "class": "billboard/effect", "dls": ["gameplay_keep:gStrayFairySkel 0x2CA98"], "status": "optional, 24 runtime En_Elforg around a02; skeletal billboard, approximation must be labeled" }
  ],
  "collisionOwnership": { "room": "Z2_YOUSEI_IZUMICollisionHeader_01BDE8 polys 0-405 of 2030 (room_00 slice; one header for all five fountains)", "waterBoxes": 1, "actorDynapoly": [], "actorCylinders": "NI" },
  "portalIds": [
    "portal.great_fairy_fountain_town.north_clock_town",
    "portal.great_fairy_fountain_town.z2_21miturinmae",
    "portal.great_fairy_fountain_town.z2_12hakuginmae",
    "portal.great_fairy_fountain_town.z2_31misaki",
    "portal.great_fairy_fountain_town.z2_ikana"
  ],
  "materialBindings": null,
  "cameraIds": ["cam.great_fairy_fountain_town.entry", "cam.great_fairy_fountain_town.reverse_door", "cam.great_fairy_fountain_town.fixture"],
  "routeIds": [],
  "omissions": ["a02 children: 24 En_Elforg stray fairies (optional proxy)", "rooms 01-04 (other regions' fountains) and their four external exits", "alternate header 1 (credits)", "Great Fairy, beam, particles and reward cutscenes", "Link and Tatl"],
  "excluded": { "halfDayMask": [], "stateOrSchedule": ["a00 Demo_Getitem (draw only on cue 110)", "a01 Bg_Dy_Yoseizo (scale 0, draw only on cue 103)"], "noGeometry": ["a02 En_Elfgrp controller", "a03 En_Test4", "a04 Elf_Msg3 trigger"] },
  "unsupported": ["two-texture scroll animated materials (3)", "stray fairy flight, sparkle and billboard", "conveyor exit floor", "floor property 9 and wall type 1 behavior", "light-mode settings (per-surface light index)", "Great Fairy cutscenes and awards"],
  "evidence": ["docs/city/interiors/great_fairy_fountain_town.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

Camera intent for the three registered IDs (SOURCE_REGISTER 4.6; poses are proposals, no capture exists): `entry` = from spawn 0 (2400, 20, 360) facing -Z down the corridor toward the pool; `reverse_door` = there is no door, so this is the corridor mouth from the rim (about z -380) looking +Z at the exit field (z 384..857); `fixture` = the source's own room_00 fixed camera, eye (2328, 67, -257), pitch -10.0 deg, yaw 171.0 deg (5.8), framing the pool and fairy group.

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Senders of fountain entrances 1-4, 7, 9 | Grep the external scenes' exit lists (Woodfall, Snowhead, Zora Cape, Ikana Canyon) for `0x4610`-`0x4640`, and all cutscene data for `0x4670`/`0x4690`; none is a city scene |
| U-02 | Flag bits of entrance words `0x4102`, `0x8102`, `0x8183`, `0x858B` beyond PORTAL_REGISTER 1 | Read `Entrance_GetTransitionFlags` users (`z_play.c:583`, `:2275`; `z_scene.c:540`) and the transition-type table |
| U-03 | Whether the shared exterior frame question matters here | None: the fountain does not share the exterior frame (`not_shared`); the North arrival/return pair is entrance-based |
| U-04 | Real reach of the 350 u cutscene trigger and the 30 u talk radius over this geometry | Route test: walk spawn 0 to the rim and pool and log `xzDistToPlayer` against (2400, -750) under `prof.d1.noon.normal` and with `WEEKEVENTREG_08_80` set |
| U-05 | Which XLU entry is the water surface and what segments 8-9 animate | Inspect `DL_004760`, `DL_004CD0`, `DL_004FB0` combiners and vertex Y; then an A1 render |
| U-06 | Ambience 19 and fog values of light settings 0 and 5 | Decode `LightSettings` fields (`include/z64environment.h`) for scene.c:94-129 entries 0, 5, 10, 15, 16 |
| U-07 | Profile form contradiction: baseline is Human, but Human without the Deku Mask item is not a first-cycle state, and Elfgrp branches on that item | OWNER/INTEGRATOR: declare Deku Mask inventory and form in `prof.d1.noon.normal`, or add `.deku` per SOURCE_REGISTER 3 |
| U-08 | Behavior of `PLAYER_START_MODE_F` and the outward conveyor during arrival/exit | Read the start-mode table in `z_player.c` and conveyor handling in `z_bgcheck.c` (shared with North U-13) |
| U-09 | Player cue ids per form in scripts 0/19/23 (`PLAYER_CUEID_15/16/17/19/22/52/55`) | Map cue ids to player actions in `z_player.c` cutscene handler; check form-specific branches |
| U-10 | Whether actor cutscenes 0-8 (global camera, no script) are used by any runtime actor in this scene | Grep spawned actors (Elforg, Dy_Extra, Demo_Effect) for `csId` use; else mark unused |
| U-11 | En_Dy_Extra and Demo_Effect light drawing | Read `ovl_En_Dy_Extra` and `ovl_Demo_Effect` light types (`DEMO_EFFECT_TYPE_LIGHT_BASE + 0`) before ROOM-03 |
| U-12 | Respawn on void-out restores `switches[2]` (`z_play.c:1894`, `:1919`) | Confirm switch 64 survives a void respawn inside the fountain (affects repeat of the restored cutscene) |

Register cross-checks (not edited): LOCATION_REGISTER row and 4.2 agree on every count; this dossier settles its two "not settled" items (room_00 exit polygons 44-49; Great Fairy type per room) and resolves U-06 in part (entrances 5, 6, 8 are cutscene destinations; 7 and 9 have no sender found). PORTAL_REGISTER section 6 lists "Great Fairy 1-9" as teleport-only spawns; 5, 6 and 8 are reached by in-scene cutscene destinations. LOCATION_REGISTER U-13's fountain persistent flag is switch0 bit 10, the Elf_Msg3 flag. North dossier U-16 is answered. The dispatch's "FULL_CITY_PLAN section 5" row is in section 6 of the current plan.

**Checks run:** read `docs/HANDOFF.md`, FULL_CITY_PLAN sections 6, 11, 12, the three registers' fountain rows, North dossier (whole) and East 2.4, the PIPE-01 schema README, `region.schema.json`, `ids.json` fountain IDs and `tools/reference/region_from_dossier.py`; read scene and room C (all five rooms, both headers), scene XML, object XMLs, `scene_table.h`, `z_scene_table.c` fountain and North rows, `z_actor.c`, `z_demo.c`, `z_eventmgr.c`, `z_elf_message.c`, `z_sram_NES.c`, `z_camera.c`, `z_room.c`, `z_scene_proc.c`, `z_bgcheck.c`, `z64bgcheck.h`, `z64cutscene.h`, `z64save.h`, `z64player.h`, `z_kaleido_map.c`, and the overlays En_Elfgrp, En_Elforg, Bg_Dy_Yoseizo, Demo_Getitem, Elf_Msg3, En_Test4 (init and cs list), En_Elf (tatl text). Scratch stdlib parsers in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\great_fairy_fountain_town\` (`cs.py` -> `cs_out.txt`, `col.py` -> `col_out.txt`, `rooms.py` -> `rooms_out.txt`, `floors.py` -> `floors_out.txt`, `lift_check.py` -> `lift_out.txt`); SHA-256 and sizes of the inputs in section 10; region block lift and schema validation in scratch.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes; message text decoding; LightSettings value decoding; external fountain scenes; 3DS material.

**Next dependency:** integrator review of section 10 (lift into `shared/city/regions/` is the integrator's step), U-07 profile decision, then ROOM-03 (special-space pilot) after ROOM-01.
