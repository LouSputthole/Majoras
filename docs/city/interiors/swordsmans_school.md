# Swordsman's School (`loc.swordsmans_school`): source inventory dossier

Task **REF-09 / INT-05** (FULL_CITY_PLAN.md section 12 row REF-09, section 6 row INT-05). Base commit `31b20dc` on `main` (HEAD moved to `7a7a08c`, PIPE-01, then `1055c1c`, REF-09 INT-01/02/13, while this packet ran; IDs below were checked against `shared/city/ids.json` and the current `region.schema.json`). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_DOUJOU` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_DOUJOU/Z2_DOUJOU.c`, `room.c` = `.../Z2_DOUJOU_room_00.c`, `js.c` = `src/overlays/actors/ovl_En_Kendo_Js/z_en_kendo_js.c`, `maruta.c` = `src/overlays/actors/ovl_En_Maruta/z_en_maruta.c`, `player.c` = `src/overlays/actors/ovl_player_actor/z_player.c`. Line numbers count `\n` only (lone `\r` bytes: 3 in scene.c, 20 in room.c). N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro/function; **OVL** read from the actor overlay source (including `.schl` schedule scripts); **ANN** decomp-author comment or symbol name; **INF** inferred (reason given); **NI** not inspected. One value family (the two course fees, section 5.2) is decoded from a baserom binary header rather than C; it is tagged **C-BIN** and the method is stated.

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01), `docs/city/PORTAL_REGISTER.md` (REF-02), `docs/city/SOURCE_REGISTER.md` (REF-03), `docs/city/districts/west_clock_town.md` section 2.4.1 (street door TA1). Every value those files state for this venue agrees with the C below; the one open item they leave (open hours) is settled in 2.5.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_DOUJOU` / `SCENE_DOUJOU`; comment "Swordsman's School" | HDR `include/tables/scene_table.h:173`; ANN :172 |
| Scene-table index | `0x54` | same line |
| Entrance-table scene index | `0x51` `sSwordmansSchoolEntranceTable` (one slot) -> `sSwordmansSchoolEntrance0 = { SCENE_DOUJOU, 0, 0x4102 }` | HDR `src/code/z_scene_table.c:2654`, :1801-1803, :1797-1799 |
| Title text / draw config / cycle flags | `0x110` / `SCENE_DRAW_CFG_DEFAULT` / `PERSISTENT_CYCLE_FLAGS_NONE` | HDR scene_table.h:173 |
| Restrictions | `RESTRICTIONS_SET(0,0,0,0,0,3,0,0,3,0,0,1)`; field order hGauge, bButton, aButton, tradeItems, songOfTime, songOfDoubleTime, invSongOfTime, songOfSoaring, songOfStorms, masks, pictoBox, all | HDR; C `src/code/z_parameter.c:66-71` |
| Restriction finding | differs from `RESTRICTIONS_INDOORS` (`0,1,0,0,0,3,0,0,3,0,0,1`) only in **bButton 0 vs 1**; bButton 0 takes the unrestricted branch, so the B (sword) button is not restricted here | C `z_parameter.c:75`, :1979, :2021; exact effect of value 1: INF |
| XML files | scene `Z2_DOUJOU` segment 2; room `Z2_DOUJOU_room_00` segment 3; room DLs `0x62B0`, `0x64C0`; external dependency `misc/scene_texture_01.xml` | XML `assets/xml/scenes/Z2_DOUJOU/Z2_DOUJOU.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:16 |
| Scene / room headers | default only: no alternate-header command in scene.c:14-31 or room.c:14-23 | HDR |
| Special files | `NAVI_QUEST_HINTS_NONE`, keep `GAMEPLAY_DANGEON_KEEP` (pots) | HDR scene.c:21 |
| Sound | spec 5, ambience 19, seq 80 = `Sequence_80` `NA_BGM_SWORD_TRAINING_HALL` | HDR scene.c:15; `include/tables/sequence_table.h:180`; macro `include/z64scene.h:845-846` |
| Skybox | externalTextureFileId 1, skybox 0, weather 0, lightMode true; room `SKYBOX_DISABLES(true, true)` | HDR scene.c:24, room.c:17; macros z64scene.h:833-837 |
| Env light settings | 4 rows, all four identical | HDR scene.c:57-66 |
| Region visited | `SCENE_CMD_SET_REGION_VISITED`; `SCENE_DOUJOU` is in the `REGION_CLOCK_TOWN` list | HDR scene.c:18; C `src/code/z_inventory.c:429-435` |
| Paths | 1 path, 7 points (the log pads, 5.6) | HDR scene.c:68-80 |
| Actor cutscenes / cs cameras | 10 / 2 | HDR scene.c:27-28, :82-93, :112-115 |
| Minimap | 1 entry `{0xFFFF, 0, 0, 0x69, 0}`, list scale 0 | HDR scene.c:95-101 |
| Animated materials | single `{ 0, 6, NULL }` (segment 0 is skipped, as in North) | HDR scene.c:121-123 |
| Transition actors | 1 (`EN_DOOR`) | HDR scene.c:17, :39-41 |
| Room commands | echo 7; `ROOM_BEHAVIOR(0x03, 0x00, 0, 0, false, 0)`: type 3 = `ROOM_TYPE_3`, storm 0; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3); cullable shape, 11 entries | HDR room.c:15-19; `include/z64scene.h:314-321`, macro :804-807 |
| Room type 3 effect | not `ROOM_TYPE_INDOORS` (2), so the indoor run-speed cap and the A-button attack/jump blocks do not apply; no `ROOM_TYPE_3` user found by grep | C `src/code/z_player_lib.c:634`; player.c:8245, :11573; `src/code/z_camera.c:6995-7003` |

### 1.1 Alternate headers

None. Every profile (`prof.d1.noon.normal`, `prof.d3.night.normal`, ...) selects the default scene and room header; the one entrance-table slot has a single layer-0 row (z_scene_table.c:1797-1799), so a nonzero `sceneLayer` is not expected here (INF). All day/time variation is per-actor (half-day masks, overlay time checks, section 5.10).

## 2. Spawns, entrances and exits

### 2.1 Start position (HDR scene.c:34-37; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`); yaw `(rawRotY >> 7) & 0x1FF`; csId `rawRotY & 0x7F`.

| Spawn | Position | Raw rot X / Y / Z | Yaw | csId | Params | Start mode | bgCam | Used by |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-1, 0, 380) | 0x0007 / 0x5A03 / 0x007F | 180 (faces -Z, into the room) | 3 | 0x0E00 | 0xE `PLAYER_START_MODE_E` | 0 | entrance 0 = `0xA200`, West exit [3] (`portal.west_clock_town.swordsmans_school`); 40 u inside the door |

- **csId 3** (C): the player passes it to `Play_AssignPlayerCsIdsFromScene` (player.c:11261; `src/code/z_play.c:2036-2059`), which walks `additionalCsId` 3 -> 2 -> 0 -> 1 -> 4 -> 9 -> 6 -> 8 -> 5 through the scene cutscene list (scene.c:83-92); all nine have negative (global) csCamIds. Entry 7 is not in the chain; it belongs to the swordsman (5.7).
- **bgCam 0** (C): `z_play.c:2326-2327` passes the spawn bgCam to `Camera_ChangeActorCsCamIndex`; without `CAM_DATA_IS_BG` (`z_camera.c:767-777`) it indexes the actor cs camera list, so arrival uses `actorCsCamList[0]` = setting 44 = `0x2C CAM_SET_START0` (5.7). Side finding for other dossiers: spawn bgCam values index the actor cs camera list, not `BgCamInfo` (bears on North U-02).
- Entrance list (HDR scene.c:47-50): two rows `{0x00, 0x00}`; entrance 0 -> spawn 0, room 0. Row 1 has no entrance-table slot (1 declared): padding (INF), matching LOCATION_REGISTER "2 / 1". Entrance flags `0x4102`: exit fade 0x02, arrival `(0x4102 >> 7) & 0x7F = 0x02`, bit 15 clear (decode per PORTAL_REGISTER section 1).

### 2.2 Exit list (HDR scene.c:52-55) and consumers

| Exit idx | Raw | Decode (`>> 9`, `(>> 4) & 0x1F`, `& 0xF`) | Destination | Consumer in the school |
| --- | --- | --- | --- | --- |
| 0 | `0xD430` | 0x6A `Z2_ICHIBA`, entrance 3, layer 0 | `sWestClockTownEntrance3 = { SCENE_ICHIBA, 3, 0x4102 }` (z_scene_table.c:2272-2274) -> West start 3 (-1870, 200, -1389) yaw 0 | surface 0, polys 0, 1 (door probe, 2.3); `En_Time_Tag` a05 exit index `0x01C0 & 0x1F = 0` |
| 1 | `0x0000` | would be Mayor's Residence e0 | none | no surface carries exit+1 = 2 (only surface 0 has exit bits) and no actor reads index 1: **padding** (C). Answers LOCATION_REGISTER U-02 for this scene |

### 2.3 Transition actor and door decode (HDR scene.c:39-41; C)

Raw entry: `{ 0, 255, 0, 255, ACTOR_EN_DOOR, 0, 0, 420, -19329, 0x0280 }` = front room 0 / bgCam 255, back room 0 / bgCam 255, position (0, 0, 420), rotY -19329, params 0x0280 (struct `include/z64scene.h:369-378`).

| Field | Value | Rule |
| --- | --- | --- |
| Yaw | rotY -19329 = 0xB47F; `(rotY >> 7) & 0x1FF = 360`; `DEG_TO_BINANG(360)` wraps to 0: **yaw 0, faces +Z** | `src/code/z_actor.c:3613` |
| csId | `rotY & 0x7F = 0x7F` -> none; spawned with `HALFDAYBIT_ALL` | z_actor.c:3619 |
| Spawned params | `TRANSITION_ACTOR_PARAMS(0, 0x280) = 0x0280` | `include/z64door.h:23` |
| Type / type var | `(p >> 7) & 7 = 5` `ENDOOR_TYPE_SCHEDULE`; `p & 0x7F = 0` `ENDOOR_SCH_TYPE_SWORDSMANS_SCHOOL` | OVL `ovl_En_Door/z_en_door.h:9-20`, :48-50, :95-96 |
| Sides | both room 0; bgCam -1 (generic door camera) both sides | z64scene.h:371-372 |
| Leaf art | `gDoorSkel` (gameplay_keep 0x22B28) limb 4 replaced by `sObjectInfo[17]` = `object_wdor03` `gInnSchoolDoorDL` 0x548 (shared with the Inn doors) | OVL `z_en_door.c:212`, :316, :637-660; XML `object_wdor03.xml:5`, `gameplay_keep.xml:941` |
| Leaving | the inside player is on the door's local -Z side, so `doorDirection = -1` (`z_en_door.c:496`); the player is placed 22 u in front (z 398) and the probe is 22 u behind: (0, 10, 442) (player.c:6756-6757, :6779-6783) -> floor polys 0/1 (surface 0, exit+1 = 1) -> exit 0 `0xD430` | C |
| Schedule from inside | the schedule script runs only when the player is on local +Z (`playerPosRelToDoor.z > 0`), so it never runs from inside: **the door always opens from inside** | OVL `z_en_door.c:537-549` |
| Wall behind the door | the front wall polys 12, 13 (surface 2, z 405, x -195..195, Y 0..180, normal -Z) span the full room width including the doorway; vestibule floors 18/19 (z 405..435) and exit floors 0/1 (z 435..555) are reached only by the probe | C (geometry); INF that the player never stands on them |

### 2.4 Collision surfaces carrying exits and door-to-venue cross-check

| Surface | data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Centroid | Normal | Flags | Material |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 0x00000100 / 0x000017C0 | 0 (`0xD430`) | 0, 1 | -60..60 / 0 / 435..555 | (0, 0, 495), 75 u behind the door | +Y | vIA xp 1 (ignore camera); no conveyor | dirt (0) |

Polygon `i` is on scene.c line `139 + i`; surfaces at :133-136. No wall or ceiling has exit bits; nothing else in the scene is an exit.

| Door | Scene | Position / yaw | Params -> schedule | Leaf | Probe -> poly -> exit | Arrival |
| --- | --- | --- | --- | --- | --- | --- |
| West TA1 (REF-06) | `Z2_ICHIBA` | (-1870, 200, -1420) / 0 | 0x0280 -> SCHEDULE / 0 `SWORDSMANS_SCHOOL` | object_wdor03 0x548 | from the street (+Z): polys 54, 55 (local z -36), surface 7, exit 3 `0xA200` | school spawn 0 (-1, 0, 380) yaw 180 |
| School TA0 (this) | `Z2_DOUJOU` | (0, 0, 420) / 0 | 0x0280 -> same | same | from the room (-Z): (0, 10, 442) -> polys 0, 1, surface 0, exit 0 `0xD430` | West start 3 (-1870, 200, -1389) yaw 0, 31 u on the street side |

Agreement (C): same params, same leaf DL, both yaw 0. In West the venue trigger is behind the leaf (-Z) and the street in front (+Z); in the school the room is on -Z and the exit floor on +Z. The two frames are unrelated (door at (0, 0, 420) here versus (-1870, 200, -1420) in West), so `sourceFrame` is `not_shared`. PORTAL_REGISTER's rows (polys 0/1, 75 u, `0xD430`, start 3, `0xA200` -> spawn 0 mode 0xE) all agree.

### 2.5 Open hours and kickout (settles PORTAL_REGISTER "open hours UNKNOWN" and SOURCE_REGISTER U-10 for this venue)

| Rule | Value | Evidence |
| --- | --- | --- |
| Street door schedule | closed only when `CURRENT_DAY == 3` and the time is in neither 06:00-22:59 nor 00:00-05:59, i.e. **Day 3 23:00-23:59**, text `0x1800 + 7 = 0x1807` (ANN comment: teacher "focusing his spirit"); every other time returns none (openable) | OVL `ovl_En_Door/scheduleScripts.schl:13-25`; ranges are `start <= now <= end - 1` on 06:00-shifted time (`src/code/z_schedule.c:25-49`, `include/z64scripts.h:7`) |
| Day 1 12:00 | not Day 3 -> none -> **open** | C |
| Kickout | a05 `En_Time_Tag` type 0 `KICKOUT_DOOR`, text `0x1883 + 0x0E = 0x1891`, exit 0, fires when hour == 23 and minute == 0 (home rot X / Y are raw integers 23 / 0); only spawned on Day 3 and Day 4 (mask 0x00F) | OVL `ovl_En_Time_Tag/z_en_time_tag.h:10-15`; `.c:285-294`, :315-339; C mask |
| Kickout deferral | `En_Kendo_Js` never sets `WEEKEVENTREG_KICKOUT_WAIT` (setters: En_Aob_01, En_Fu, En_Mm3 only, grep), so a course in progress at 23:00 is not protected; the tag only waits while `Play_InCsMode` | C (grep); z_en_time_tag.c:333-334 |
| Result | open 24 h on Days 1 and 2 and on Day 3 until 23:00; kicked out at Day 3 23:00; closed 23:00-23:59; open again 00:00-05:59 (Day 3 final hours), when the instructor is replaced (5.10) | C |

### 2.6 Portal IDs (registered in `docs/city/PORTAL_REGISTER.md` and `shared/city/ids.json`)

| Portal ID | Mode | Source evidence | Arrival |
| --- | --- | --- | --- |
| `portal.swordsmans_school.west_clock_town` | door | TA0 + surface 0, exit 0 `0xD430` | West start 3 |
| `portal.swordsmans_school.west_clock_town.kickout` | special | a05 `En_Time_Tag` 0x01C0, exit 0 | West start 3 |
| `portal.west_clock_town.swordsmans_school` (inbound, owned by West) | door | West TA1 + surface 7, exit 3 `0xA200` | school spawn 0 |

One room: no internal room portal exists or is needed.

## 3. Object banks (HDR room.c:25-33; IDs from `include/tables/object_table.h`)

| # | Bank | Object ID | Consumer (profile `objectId`, OVL) | Needed for Day 1 static list |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_JS` | 0x10F | En_Kendo_Js a00, a01 (js.c:43) | skeletal (later) |
| 1 | `OBJECT_MARUTA` | 0x1D1 | En_Maruta, spawned by a00 during a course (js.c:761; maruta.c:41) | no (course only) |
| 2 | `OBJECT_DORA` | 0x1EE | Obj_Dora a03 and Obj_Kendo_Kanban a02 (`z_obj_dora.c:40`; `z_obj_kendo_kanban.c:49`) | yes |
| 3 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai a04 (wall clock) | yes (already extracted) |
| 4 | `OBJECT_KANBAN` | 0x0FC | En_Kanban spawned by a00 on Day 3 after 23:00 or before 06:00 (js.c:149-154) | no |
| 5 | `OBJECT_WDOR03` | 0x276 | EN_DOOR TA0 leaf (`z_en_door.c:212`) | yes |
| 6 | `OBJECT_HARIKO` | 0x1F2 | Obj_Hariko a12 | yes |

Implicit (not in the list): `gameplay_keep` 0x001 (door skeleton, En_Time_Tag and En_Test4 profiles) and `gameplay_dangeon_keep` 0x003 (scene special file; Obj_Tsubo type 0 pot, `z_obj_tsubo.c:67-69`). Every listed bank has a consumer; no unexplained bank (C).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): ID `id & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF` degrees, unless the ID carries `0x8000` (rotY), `0x4000` (rotX) or `0x2000` (rotZ), in which case that axis is passed as a plain integer. csId `rawRotY & 0x7F` (0x7F -> none, :3570-3574); half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all (:3576-3580).
- `Actor_SpawnSetupActors` (2546-2577) spawns an entry only if its mask contains the current half-day bit; `Actor_InitHalfDaysBit` (2491-2499) gives `0x080` for Day 1 12:00. Bits (`include/z64actor.h:328-341`): D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001. No entry here has ID bit 0x800.
- Flagged IDs here: `En_Time_Tag | 0xE000` (all three axes integer: X 23 = kickout hour, Y 0 = minute) and `Obj_Tsubo | 0x2000` (rotZ integer 0 = pot mode, `OBJ_TSUBO_ZROT`).

Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled`, `billboard-effect`, `controller-invisible`, `door-transition`. "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (13 entries, HDR room.c:35-75, entry `i` on line `36 + 3i`; decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot deg X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_KENDO_JS` (0x1EF) | (-3, 30, -45) | 0x0001 / 0x0007 / 0x007C | 0 / 0 / 0 | 0x0FC D1d-D3n | 7 | 0x0000 | object_js | npc-scheduled | yes |
| 1 | `ACTOR_EN_KENDO_JS` (0x1EF) | (89, 45, -410) | 0x0000 / 0x40FF / 0x0004 | 0 / 129 / 0 | 0x004 D3n | none | 0xFF01 | object_js | npc-scheduled | no |
| 2 | `ACTOR_OBJ_KENDO_KANBAN` (0x209) | (0, 45, -255) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_dora | static-mesh | yes |
| 3 | `ACTOR_OBJ_DORA` (0x207) | (-114, 78, 55) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_dora | static-mesh | yes |
| 4 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-1, 136, 401) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF all | none | 0xA07F | object_obj_tokeidai | static-mesh | yes |
| 5 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (71, 0, 171) | 0x0B80 / 0x007F / 0x000F | 23 / 0 / 0 (integers) | 0x00F D3d, D3n, D4d, D4n | none | 0x01C0 | gameplay_keep | controller-invisible | no |
| 6 | `ACTOR_OBJ_TSUBO \| 0x2000` (0x082) | (94, 45, -346) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 (Z integer) | 0x3FF all | none | 0x0202 | gameplay_dangeon_keep | static-mesh | yes |
| 7 | `ACTOR_OBJ_TSUBO \| 0x2000` (0x082) | (34, 45, -345) | same | same | 0x3FF all | none | 0x0602 | gameplay_dangeon_keep | static-mesh | yes |
| 8 | `ACTOR_OBJ_TSUBO \| 0x2000` (0x082) | (34, 45, -405) | same | same | 0x3FF all | none | 0x0802 | gameplay_dangeon_keep | static-mesh | yes |
| 9 | `ACTOR_OBJ_TSUBO \| 0x2000` (0x082) | (-25, 45, -405) | same | same | 0x3FF all | none | 0x0A02 | gameplay_dangeon_keep | static-mesh | yes |
| 10 | `ACTOR_OBJ_TSUBO \| 0x2000` (0x082) | (-26, 45, -345) | same | same | 0x3FF all | none | 0x0C02 | gameplay_dangeon_keep | static-mesh | yes |
| 11 | `ACTOR_EN_TEST4` (0x15A) | (-1, 0, 519) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0100 | gameplay_keep | controller-invisible | yes |
| 12 | `ACTOR_OBJ_HARIKO` (0x20A) | (35, 40, -24) | 0x0007 / 0xAC7F / 0x007F | 0 / 344 / 0 | 0x3FF all | none | 0x0000 | object_hariko | static-mesh | yes |

### 4.3 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init: scale, collision, children | Draw | Presence / state gating |
| --- | --- | --- | --- |
| En_Kendo_Js (ANN "Swordsman", js.c:1-5) | `ACTORCAT_NPC`; flex `gSwordmasterSkel` (object_js 0x6990) with `gSwordmasterDefaultAnim` (0xF4C), default scale 0.01; cylinder r18 h30, OC only; shadow circle 36; params location `p & 0xFF` (1 = back room), path `(p >> 8) & 0xFF` (js.c:39-69, :136-179; `z_en_kendo_js.h:11-14`; XML `object_js.xml:7`, :42) | `SkelAnime_DrawFlexOpa`; head limb 0x0C turned by player tracking (js.c:784, :789-807) | a00 by mask D1d-D3n; on Day 3 after 23:00 or before 06:00 a00 spawns `En_Kanban` params 0x10 at home z-10 and kills itself, a01 plays the cowering anim; otherwise a01 kills itself (js.c:149-160). Details 5.1-5.10 |
| En_Maruta (log, spawned) | `ACTORCAT_PROP`; scale 0.1; Y -4; cylinder r12 h65, AC from player, hookable; health 1 (maruta.c:37-47, :177-195, :236-285) | `gPracticeLogWholeDL` 0x2EC0 or 8 chunk DLs (maruta.c:64-72, :730-769; XML `object_maruta.xml:6-14`) | exists only during a course (5.6) |
| Obj_Kendo_Kanban (ANN "Cuttable Board at back of school") | scale 0.1; cylinder r33 h80 offset 20 u back; tris collider x -300..300, y 10..850 model units at z 40 (60 x 84 u world) (`z_obj_kendo_kanban.c:88-144`, :186-236, :476-478) | intact `gKendoKanbanDL` (object_dora 0x180); after a cut, two halves from four quarter DLs 0x2080-0x2380 (:494-525; XML `object_dora.xml:4-10`) | always; a sword cut splits it (vertical for forward slash or jump-slash finish, else horizontal, :252-300); no flag, item or reward in the file (grep) |
| Obj_Dora (ANN "Gong") | scale 0.1; 6-element tris collider (`z_obj_dora.c:49-166`, :168-206) | rest: `gDoraGongDL` 0x3FD0 then `gDoraChainDL` 0x4160; swinging: chain then gong with X rotations (:317-352; XML `object_dora.xml:13-14`) | always; rupee mechanic 5.11 |
| Obj_Tokeidai type 0xA | `OBJ_TOKEIDAI_TYPE_SMALL_WALL_CLOCK`: scale 0.01, `ObjTokeidai_Clock_Draw`, `ObjTokeidai_Clock_Init` from `CURRENT_TIME` (`z_obj_tokeidai.h:6-20`; `.c:104-115`, :254-259) | minute ring 0xCF28, center/hand 0xBEE8, `gWallClockClockFaceDL` 0xF518 (not the tower face), sun/moon panel 0xC368 (`.c:811-847`; XML `object_obj_tokeidai.xml:47-67`) | always; follows the clock every frame (`.c:719-723`) |
| En_Time_Tag (kickout) | type `(p & 0xE000) >> 13 = 0`, text `(p & 0x1FE0) >> 5 = 0x0E`, exit `p & 0x1F = 0`; hour/minute from home rot X/Y | profile draw `NULL` | mask Day 3-4 only (2.5) |
| Obj_Tsubo (pot) | type `(p >> 7) & 3 = 0`: gameplay_dangeon_keep pot, scale 0.197, cylinder r12 h32; drop index `p & 0x3F = 2` = `ITEM00_RUPEE_BLUE`; collectible flag `(p >> 9) & 0x7F` = 1, 3, 4, 5, 6 for a06-a10 (`z_obj_tsubo.h:12-18`; `.c:67-75`, :122-131, :175-205; `src/code/z_en_item00.c:1453-1469`) | `gameplay_dangeon_keep_DL_017EA0` (`.c:703`; XML `gameplay_dangeon_keep.xml:57`) | always; behind the training-area clamp (5.9) |
| En_Test4 | stars `(p >> 10) * 100 = 0`, big-bell volume index `(p >> 5) & 0xF = 8` (`z_en_test4.h:8-9`) | none | always; day/half-day controller |
| Obj_Hariko (cow statue head) | scale 0.1 (`z_obj_hariko.c:39`) | `gHarikoBodyDL` 0x80 + `gHarikoFaceDL` 0x110 (:92-100; XML `object_hariko.xml`) | always; bobs only during quakes (:74-80) |
| En_Door TA0 | 2.3 | 2.3 | always; schedule 2.5 |

### 4.4 Counts

| | Default header |
| --- | --- |
| Entries / transition actors | 13 / 1 |
| Day 1 daytime candidates by mask | 11 (excluded: a01, a05) |
| npc-scheduled | 2 (a00, a01) |
| static-mesh | 9 (a02, a03, a04, a06-a10, a12) |
| controller-invisible | 2 (a05, a11) |
| posed-skeletal / billboard-effect | 0 / 0 |
| door-transition | 1 (TA0) |
| Runtime-spawned | En_Maruta logs (course) and their cut pieces, En_Kanban (Day 3 late), En_Item00 rupees (gong, pots) |

## 5. Venue mechanics index (INT-05 scope: training room, target locations, camera, collision space)

Index and citations only; no Unity design.

### 5.1 The instructor and where training happens

| Item | Value | Evidence |
| --- | --- | --- |
| Instructor | a00 `En_Kendo_Js` at (-3, 30, -45) on the dais, yaw 0 (facing the door); csId 7; path 0 | HDR, C |
| Back-room variant | a01 at (89, 45, -410), yaw 129, location 1; talk gives only `0x273C` | js.c:203-204 |
| Talk offer | 100 u; lock-on and attention disabled | js.c:244, :170; FLAGS :10-12 |
| Training floor | main hall floor Y 0, z 25..405 (clamp 5.9), with the 7 log pads (5.6) | C |
| Novice start | auto-walk to a00 + (0, 0, 200) = XZ (-3, 155) | js.c:519-537 |
| Expert start | auto-walk to a00 + (0, 0, 300) = XZ (-3, 255) | js.c:614-625 |
| Auto-walk | `isOverrideInputOn`, stick toward target, magnitude 80 / 40 / 10 beyond 40 / 20 / under 20 u; done under 20 u | js.c:725-749 |

### 5.2 Talk gating, course choice and fees

| Condition (checked in this order) | Result | Evidence |
| --- | --- | --- |
| back-room instance | `0x273C` | js.c:203-204 |
| not Human form | `0x2731`-`0x2739` by form (Deku, Goron, Zora) and day; no course | js.c:205-224, :94-98 |
| a mask worn (not none, below `PLAYER_MASK_GIANT`) | `0x273C + mask`; no course | js.c:225-231 |
| Human, no mask | `0x2710`-`0x2715` (first/repeat x day 1-3, `dayIndex = CURRENT_DAY - 1`), then choice `0x2716` | js.c:197-201, :232-240, :100-103, :300-307 |
| choice 0 (Novice) | no sword equipped (`EQUIP_VALUE_SWORD_NONE`) -> `0x272C` + flailing; rupees < `firstChoicePrice` -> `0x2718`; else `Rupees_ChangeBy(-firstChoicePrice)`, `0x2719` | js.c:251-263 |
| choice 1 (Expert) | same checks with `secondChoicePrice`; then `0x273A` -> `0x273B` -> course | js.c:266-278, :320-329 |
| choice 2 | `0x2717`, no charge | js.c:281-283 |

**Fees (C-BIN):** message `0x2716` header gives `firstChoicePrice = 1` and `secondChoicePrice = 10`: **Novice 1 rupee, Expert 10 rupees**. Method: header layout `include/message_data_fmt_nes.h:126-127`, parse order `src/code/z_message.c:2070-2077`; the `code` file's message table entry for 0x2716 (offset 0x1281B0, neighbours 0x2715 / 0x2717 in sequence) points to segment 0x08053220; bytes 5-8 at offset 0x53220 of `extracted/n64-us/baserom/message_data_static`. Scratch `msg.py`. No message text is copied here.

### 5.3 Novice course (lessons, no score, no prize)

| Step | Detail | Evidence |
| --- | --- | --- |
| Setup | player locked (`PLAYER_STATE1_20`), auto-walked to (-3, 155); log at path point 0; text `0x271A` | js.c:309-313, :519-537 |
| Start | `SET_WEEKEVENTREG(WEEKEVENTREG_PLAYING_SWORDSMAN_MINIGAME)` (82 & 0x08, `include/z64save.h:1407`); round 0, course active | js.c:539-546 |
| Lessons in order | side jump; roll; backflip; horizontal slash on the log (`PLAYER_MWA_RIGHT_SLASH_1H` / `RIGHT_COMBO_1H`); forward slash (`FORWARD_SLASH_1H` / `FORWARD_COMBO_1H`; hint `0x272B` while locked on); thrust (`STAB_1H`); jump attack (`JUMPSLASH_START` / `FINISH`) | js.c:119-128, :383-480, :502-506 |
| Wrong action | error sfx, `0x2729`, flailing, lesson-specific retry text | js.c:575-581, :109-111, :552-561 |
| Correct | sfx, progress text, next lesson | js.c:568-573, :105-107, :482-485 |
| Log respawn | if no log stands, a new one at point 0 after 30 frames | js.c:584-590 |
| End | after lesson 7 the minigame flag is cleared; nothing awarded | js.c:599-602 |

### 5.4 Expert course rules (time limit and scoring)

| Rule | Value | Evidence |
| --- | --- | --- |
| HUD state | `Interface_InitMinigame`: `minigameStatus = ACTIVE`, `minigameScore = 0` | js.c:326; `z_parameter.c:2481-2492` |
| Start state | flag set, timer 120, round 0 | js.c:606-612 |
| Waves | when timer >= 140 and round < 5: spawn one log at a random path point 1-3 and one at 4-6, timer 0, round++; at timer 120 all logs retract | js.c:627-653, :765-776 |
| Time limit | frame based, no clock time: 20-frame lead-in, then 5 waves of 140 frames, each pair standing until frame 120 of its wave: 720 frames, 10 logs | C (arithmetic on js.c:608, :630, :648) |
| Seconds | about 36 s if one actor update = 1/20 s (INF; real seconds per update are SOURCE_REGISTER U-01) | INF |
| Points per cut | jump attack 3, `STAB_1H` 2, any other cutting animation 1; added each frame while ACTIVE | js.c:655-666; `z_parameter.c:7171-7174` |
| Win | after wave 5: score exactly 30 (all ten logs by jump attack) -> `0x272D`; otherwise `0x272E` and `minigameStatus = END` when it closes | js.c:630-641, :365-367 |
| Last-cut lock | a jump-attack cut with score >= 27 locks the player | js.c:659-661 |
| Leaving | Destroy clears the minigame flag | js.c:181-186 |

### 5.5 Reward and save flags

| Item | Value | Evidence |
| --- | --- | --- |
| Trigger | closing `0x272D`: `minigameStatus = END`, prize offered | js.c:331-336 |
| First clear | `WEEKEVENTREG_RECEIVED_SWORDSMANS_SCHOOL_HEART_PIECE` (63 & 0x20) clear -> `Actor_OfferGetItem(GI_HEART_PIECE, 800, 100)` | js.c:677-686; `include/z64save.h:1234`; `include/z64item.h:427` |
| Repeat clear | flag set -> `GI_RUPEE_RED` (`z64item.h:419`); 20 rupees if it gives `ITEM_RUPEE_RED` (`sRupeeRefillCounts[3]`, `z_parameter.c:2543`, :2856-2857; GI -> item mapping INF) | OVL, INF |
| Flag write | set after the item is taken, at the follow-up talk (`0x272F` first, `0x2730` repeat) | js.c:692-707 |
| Persistence | the heart-piece flag survives the Song of Time reset (`sPersistentCycleWeekEventRegs` row 63); other week-event bits are cleared in `Sram_SaveEndOfCycle` | `src/code/z_sram_NES.c:164`, :430, :486-496 |
| Scene flags | `PERSISTENT_CYCLE_FLAGS_NONE` | scene_table.h:173 |

### 5.6 Log targets (`En_Maruta`) and target locations

| Behavior | Value | Evidence |
| --- | --- | --- |
| Spawn | `Actor_SpawnAsChild(ACTOR_EN_MARUTA)` at a path point, rot 0, params 0 (shape whole, type 0) | js.c:756-763; `z_en_maruta.h:10-13` |
| Rise | starts 630 u below home, smooth-steps up | maruta.c:319-328 |
| Seated | turns 60 deg toward the player when off by more than 30 deg; reports lock-on to a00 | maruta.c:299-317 |
| Cut | on an AC hit while seated, `sShapesAfterMeleeWeaponAnimations[meleeWeaponAnimation]`; a non-whole shape spawns the cut piece, sets a00 `isSlashingLog`, `numLogs--` | maruta.c:86-121, :551-596 (flag at :582) |
| Animations that do not cut | flip slash start, jump slash start, Zora jump kick, back slashes, Goron and Zora punches map to whole | maruta.c:103-116 |
| Retract | gravity -2 down to floor - 630, then killed; a cut bottom half sinks after 30 frames, killed at 40 | maruta.c:330-372 |

| Path point (scene.c:68-76) | Position | Collision pad (surface 4) | Pad extent x / z | Used by | Distance from course start |
| --- | --- | --- | --- | --- | --- |
| 0 | (0, 4, 92) | polys 24, 25 | -15..15 / 75..105 | Novice | 63 u from (-3, 155) |
| 1 | (-37, 4, 286) | 30, 31 | -52..-22 / 270..300 | Expert left (rand 1-3) | 46 u from (-3, 255) |
| 2 | (-105, 4, 250) | 28, 29 | -120..-90 / 233..263 | Expert left | 102 u |
| 3 | (-135, 4, 181) | 26, 27 | -150..-120 / 165..195 | Expert left | 151 u |
| 4 | (37, 4, 287) | 32, 33 | 22..52 / 270..300 | Expert right (rand 4-6) | 51 u |
| 5 | (106, 4, 251) | 34, 35 | 90..120 / 233..263 | Expert right | 109 u |
| 6 | (135, 4, 182) | 36, 37 | 120..150 / 165..195 | Expert right | 156 u |

Pads are 30 x 30 u wood floors at Y 4 with `isSoft` (floor raycasts return Y - 1, `src/code/z_bgcheck.c:1733-1735`); a log's home Y is 4 - 4 = 0 (C). Whether the pads are drawn as trap doors is room-DL content (NI, U-04).

### 5.7 Camera settings

| Use | Setting | Source chain | Function data (`BgCamFuncData`, `include/z64bgcheck.h:133-142`) | Mode function |
| --- | --- | --- | --- | --- |
| Room (every floor) | `0x003F CAM_SET_ROOM0` | `BgCamInfo[0]`; all 7 surfaces have bgCam 0 | NULL | `CAM_FUNC_NORMAL1` |
| Arrival | `0x2C CAM_SET_START0` | spawn bgCam 0 -> `actorCsCamList[0]` (2.1) | pos (-108, 53, 376), rot (546, 26214, 0), fov -1, then 5, -1 | `CAM_FUNC_UNIQUE0` |
| Expert course (set every frame) | `0x25 CAM_SET_CIRCLE3` | a00 csId 7 -> cutscene entry 7 csCamId 1 -> `actorCsCamList[1]` via `Camera_ChangeActorCsCamIndex` | pos (-1, 125, -31), rot (0, 0, 0), fov -1, then 260, 100 | `CAM_FUNC_FIXED2` |
| Novice course | no change (room camera) | `EnKendoJs_SetExpertCourseCamera` is called only from the expert loop | | |
| Door | generic door camera (bgCam -1 both sides) | TA0 sides | | |

Citations: scene.c:129-131 (BgCamInfo), :90 (entry 7), :103-115 (points, cs cam list); js.c:670, :709-714; `src/code/z_eventmgr.c:118-125`; `src/code/z_camera.c:767-777`, :7806-7832; `src/code/z_camera_data.inc.c:2494-2495`, :2571-2572, :2860-2861, :3559, :3566, :3585; enum `include/z64camera.h:184`, :191, :210. INF: the CIRCLE3 eye sits 14 u in front of the instructor and 95 u above the dais, looking at the player; FIXED2 data semantics beyond pos are U-06.

### 5.8 HUD and input during a course

| Item | Value | Evidence |
| --- | --- | --- |
| While the minigame flag is set | C buttons disabled; HUD `A_B_HEARTS_MAGIC_MINIMAP` | `z_parameter.c:1764-1772`, :2285-2287 |
| Expert score icon | `gSwordTrainingLogIconTex` (parameter_static 0xBE0, ia8 24x16), drawn for `SCENE_DOUJOU` while ACTIVE | `z_parameter.c:6249-6253`; XML `interface/parameter_static.xml:15` |
| Player locks | `PLAYER_STATE1_20` during choice-to-start, results and lesson feedback | js.c:311, :327, :571, :578, :637, :703 |
| Required player actions | side jump, roll, backflip, B-button sword slashes, thrust, jump attack, Z-target lock-on; the room keeps B usable (section 1) | js.c:383-480; maruta.c:86-121 |

### 5.9 Training-area clamp and reachable space

- Every frame a00 clamps the player to `z >= a00.z + 70 = 25` (js.c:716-723, called at :786; a01 is exempt). With a00 present (every half-day of Days 1-3 except Day 3 after 23:00 / before 06:00) the dais (z -195..0), the cow statue (z -24), the board (z -255), the back room and the five pots (z -442..-285) are **unreachable**; the gong frame (z 30..71) and all seven pads (z 75..300) are inside the allowed space (C, arithmetic).
- The board a02 sits centred on the 60 u passage mouth (x -30..30, Y 45..120, walls 93/94 and 99/100); its 60 x 84 u tris face and r33 cylinder close it (INF: it must be cut to pass).
- When a00 is absent (Day 3 00:00-05:59, door open per 2.5) an `En_Kanban` stands at (-3, 30, -55) and a01 cowers in the back room; whether a player then reaches the back room is U-03.

### 5.10 Day and time variants

| Window | Instructor | Other | Evidence |
| --- | --- | --- | --- |
| Days 1-2, all day; Day 3 06:00-23:00 | a00 teaches | a01 absent; a05 present on Day 3 only | masks; js.c:149-160 |
| Day 3 23:00 | kickout to West start 3 (text 0x1891) | door closed until 00:00 | 2.5 |
| Day 3 00:00-05:59 | a00 replaced by `En_Kanban` params 0x10 (text `p \| 0x300 = 0x0310`) | a01 cowering, talk `0x273C` | js.c:149-156 |
| Day text | `0x2710 + 2 x dayIndex` (first talk) / `0x2711 + 2 x dayIndex` (repeat) | | js.c:100-103 |

### 5.11 Other room mechanics

| Fixture | Mechanic | Evidence |
| --- | --- | --- |
| Gong a03 | sword, stick, thrown objects, arrows, hookshot: light hit; Goron punch: strong; any hit swings it and quakes; if the clock is within 1 game minute of :00 or :30, a blue rupee drops (40-frame cooldown). 12:00 qualifies | `z_obj_dora.c:130-166`, :252-308 |
| Board a02 | cuttable, physics only; no reward | 4.3 |
| Pots a06-a10 | blue rupee each, collectible flags 1, 3, 4, 5, 6 | 4.3 |
| Wall clock a04 | live time display above the door | 4.3 |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header)

Selection: default scene and room header (the only header), sceneLayer 0, half-day bit 0x080, fresh save, Human form. **The venue is open at Day 1 12:00** (2.5). World positions are N64; `world = actorPos + actorRot x (actorScale x localVertex)` (ROM_REFERENCE.md); rotation conversion under the Z mirror belongs to the importer.

| # | Component / params | N64 position (Unity) | Yaw | Actor scale | Display list(s), bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| TA0 | Door, 0x0280 | (0, 0, 420) (0.00, 0.00, -4.20) | 0 | 0.01 (default) | `gDoorSkel` gameplay_keep 0x22B28, limb 4 = `gInnSchoolDoorDL` object_wdor03 0x548 | closed rest pose; same-room side DL chosen by camera yaw |
| a04 | Small wall clock, 0xA07F | (-1, 136, 401) (-0.01, 1.36, -4.01) | 180 | 0.01 | object_obj_tokeidai 0xCF28, 0xBEE8, 0xF518, 0xC368 | above the door alcove, facing -Z; noon pose per ROM_REFERENCE clock rules |
| a03 | Gong, 0x0000 | (-114, 78, 55) (-1.14, 0.78, -0.55) | 0 | 0.1 | object_dora 0x3FD0 + 0x4160 | rest pose, inside the gong frame (7) |
| a02 | Board, 0x0000 | (0, 45, -255) (0.00, 0.45, 2.55) | 0 | 0.1 | object_dora 0x180 | intact |
| a12 | Cow statue head, 0x0000 | (35, 40, -24) (0.35, 0.40, 0.24) | 344 | 0.1 | object_hariko 0x80 + 0x110 | on the dais, 10 u above its floor (support NI, U-13) |
| a06 | Pot, 0x0202 | (94, 45, -346) (0.94, 0.45, 3.46) | 0 | 0.197 | gameplay_dangeon_keep 0x17EA0 | back room |
| a07 | Pot, 0x0602 | (34, 45, -345) (0.34, 0.45, 3.45) | 0 | 0.197 | same | back room |
| a08 | Pot, 0x0802 | (34, 45, -405) (0.34, 0.45, 4.05) | 0 | 0.197 | same | back room |
| a09 | Pot, 0x0A02 | (-25, 45, -405) (-0.25, 0.45, 4.05) | 0 | 0.197 | same | back room |
| a10 | Pot, 0x0C02 | (-26, 45, -345) (-0.26, 0.45, 3.45) | 0 | 0.197 | same | back room |

**Excluded by half-day mask:** a01 (D3n), a05 (Day 3-4). **Present but unimplemented in a static assembly:** a00 `En_Kendo_Js` (`gSwordmasterSkel` + `gSwordmasterDefaultAnim`; posed-skeletal reconstruction). **No geometry:** a11 `En_Test4`. **Not present at rest:** logs (course only), `En_Kanban` (Day 3 late).

Playable training (INT-05: a decorated room alone is not playable training) additionally needs: the Link sword actions listed in 5.8, the log actor (object_maruta, 5.6), the CIRCLE3 course camera (5.7), the z >= 25 clamp (5.9), the score HUD (5.8), rupee spend and the persistent heart-piece flag (5.2, 5.5).

## 7. Collision summary (C from scene.c:129-386; scratch `col.py`, `walls.py`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 113 / 121 / 7 (LOCATION_REGISTER does not count these) |
| Water boxes | 0 (`0, NULL`, scene.c:385) |
| Camera data | 1: `0x003F CAM_SET_ROOM0` |
| Header bounds | min (-195, 0, -525), max (196, 187, 555) |
| Vertex extents | x -195..195, y 0..187, z -442..555; Unity x -1.95..1.95, y 0..1.87, z -5.55..4.42 (header z min is 83 u past the vertices, U-05) |
| Polygon kinds (normal Y > 0.5 floor, < -0.5 ceiling) | 40 floors, 70 walls, 11 ceilings |
| Materials by polygon | dirt (0) 20, carpet (13) 2, wood (10) 73, stone (2) 26 (`include/z64bgcheck.h:232-246`) |
| Exit surfaces | 1 type, 2 floor polygons (2.4) |
| Poly exclusion | xp 1 ignore-camera: 10 (exit 0/1, door alcove 16-23); xp 5 ignore-camera + projectiles: 8 (gong frame walls 47-52, 63, 64); no vIB flags (no conveyor, no Deku flower) |
| Other flags | all surfaces: lightSetting 31, echo 2, floorType 0, floorProperty 0, no hookshot, no conveyor; surface 4 `isSoft`; surface 5 wallType 1 = `WALL_FLAG_0`, which skips the player's ledge-climb branch (`z_bgcheck.c:18-20`, :4206-4208; player.c:11835) |
| Special | no ladder, hookshot, conveyor, void, water or damage surfaces |

| Surface | data[0] / data[1] | Material, flags | Polys (floor / wall / ceiling) | Where |
| --- | --- | --- | --- | --- |
| 0 | 0x00000100 / 0x000017C0 | dirt, exit 0 | 2 / 0 / 0 | exit floor beyond the door |
| 1 | 0x00000000 / 0x000017CD | carpet | 2 / 0 / 0 | dais top, Y 30 |
| 2 | 0x00000000 / 0x000017C0 | dirt | 4 / 10 / 4 | hall floor, side walls x +-195, front wall z 405, door alcove, hall ceiling Y 180, alcove ceiling Y 100 |
| 3 | 0x00000000 / 0x000017CA | wood | 18 / 20 / 7 | dais riser, gong base, front-right fixture, back landing, passage, back room |
| 4 | 0x40000000 / 0x000017CA | wood, soft | 14 / 0 / 0 | the 7 log pads, Y 4 |
| 5 | 0x00200000 / 0x000017CA | wood, wallType 1 | 0 / 14 / 0 | gong frame (8), front-right fixture (6) |
| 6 | 0x00000000 / 0x000017C2 | stone | 0 / 26 / 0 | back wall z -195 and the landing, passage and back-room walls |

| Space | Extent x / y / z | Height | Polys |
| --- | --- | --- | --- |
| Main hall | -195..195 / 0 / 0..405 | ceiling Y 180 | floors 14, 15; ceilings 4, 5; walls 6, 7, 10-13 |
| Dais | -195..195 / 30 / -195..0 | 180 | floors 2, 3; riser 8, 9 |
| Back opening and landing | opening x -45..60 over a 15 u step (115, 116); landing -45..60 / 45 / -255..-195 | 180 | floors 107, 108; walls 103-106, 113, 114, 117, 118 |
| Passage | -30..30 / 45 / -285..-255 | 120 | floors 97, 98; ceilings 95, 96; walls 93, 94, 99, 100 |
| Back room | -120..120 / 45 / -442..-285, with a walled block at x -120..-49, z -442..-330 | 180 | floors 75, 76; ceilings 91, 92; walls 69-90 |
| Door alcove | -30..30 / 0 / 405..435 | 100 | floors 18, 19; walls 16, 17, 20, 21; ceilings 22, 23 |
| Gong frame | walls x -139..-71, z 40..61, Y 6..180; base slopes to x -146..-64, z 30..71 (40.6 deg, Y 0..6); top Y 180 | | 47-68 |
| Front-right fixture | 134..195 / 0..85 / 355..405 | top Y 85 | floors 39, 40; walls 41-46; ceiling 38 (INF furniture; room shape entry (166, 42, 375) r50) |

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 21: 18 textures + 3 TLUTs |
| By format | rgba16 6 (3 textures + 3 TLUTs), ci8 13, i8 2 |
| External texture | `scene_texture_01_Tex_002D00` (i8 8x8, `assets/xml/misc/scene_texture_01.xml:18`); not extracted |
| Location | all 21 in the scene file; the room file holds none; all 18 scene textures are loaded by room DLs |
| Room `Gfx` / `Vtx` arrays | 14 / 11 (11 `.vtx.inc` files) |
| Room shape | cullable, 11 entries: 10 opaque DLs, 2 XLU DLs (entries 2 and 7) (room.c:77-95) |
| XML-declared DLs | `0x62B0` calls the 10 opaque entry DLs; `0x64C0` calls the 2 XLU DLs (0x6310, 0x6418) (room.c:848-898) |
| Triangles in source | 334 `gsSP2Triangles` + 7 `gsSP1Triangle` = 675 (DL reuse not resolved) |
| Render setup | 10 `gsDPSetCombineLERP`, 11 `gsDPSetRenderMode` |
| Animated materials | none |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_DOUJOU` (C above); `object_obj_tokeidai` (South). Binaries for every input below exist under `extracted/n64-us/baserom/` (checked by `stat`; object_js 27,168, object_maruta 12,256, object_dora 21,008, object_hariko 1,056, object_wdor03 5,808, object_kanban 6,064, gameplay_dangeon_keep 144,000, gameplay_keep 599,520, scene_texture_01 19,712, parameter_static 47,904 bytes).

| Priority | Inputs | Why |
| --- | --- | --- |
| Day 1 static | `object_dora`, `object_hariko`, `object_wdor03`, `gameplay_dangeon_keep`, `gameplay_keep`, `misc/scene_texture_01` | section 6 DLs, door skeleton, the room DL's external texture |
| Training pass | `object_js`, `object_maruta`; optional `interface/parameter_static` (score icon) | instructor skeleton, logs |
| Day 3 late variant | `object_kanban` | En_Kanban sign |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_dora','object_hariko','object_wdor03','gameplay_dangeon_keep','gameplay_keep','object_js','object_maruta','object_kanban')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
& $zapd e -i "assets/xml/misc/scene_texture_01.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/misc/scene_texture_01" -osf "extracted/n64-us/assets/misc/scene_texture_01" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
if ($LASTEXITCODE -ne 0) { throw "Extraction failed: scene_texture_01" }
```

`gameplay_keep` and `gameplay_dangeon_keep` have not been extracted with this tool before (NI); check size and warnings separately. The `misc` output path is a proposal (no misc file has been extracted yet).

## 10. Proposed region record

Schema 1 (PIPE-01, `shared/city/schema/region.schema.json`). `portalIds` are exactly the two portals departing `loc.swordsmans_school` in `shared/city/ids.json`.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.swordsmans_school",
  "displayLabel": "Swordsman's School",
  "source": {
    "sceneSymbol": "Z2_DOUJOU",
    "sceneEnum": "SCENE_DOUJOU",
    "sceneTableIndex": "0x54",
    "entranceTableSceneIndex": "0x51",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneLayer": 0,
    "halfDayBit": "0x080",
    "stateAssumptions": [
      "fresh save: no week-event, switch or collectible flags",
      "player form Human, no mask",
      "WEEKEVENTREG_RECEIVED_SWORDSMANS_SCHOOL_HEART_PIECE clear",
      "no course in progress (no En_Maruta logs)"
    ]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_DOUJOU", "bytes": 30000, "sha256": "8fa7f225794f65282a96bc385662c6f38724850dd5d98307479c2af3188559a7" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_DOUJOU_room_00", "bytes": 25824, "sha256": "daf0bc0ceb2fd8cf54a5e9e783ccd711f4e36c1f2b1d3cde18f88c81f5b50da2" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_DOUJOU/Z2_DOUJOU.c", "bytes": 20192, "sha256": "10da327ef29da660d57fb27eb6be1be32f4b1cb902b20f9cab51c379fbea1efb" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_DOUJOU/Z2_DOUJOU_room_00.c", "bytes": 47014, "sha256": "eb393d32995885cb5cb4d29800b54c527324bc7730968d8214208625aad3507c" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_DOUJOU/Z2_DOUJOU.xml", "bytes": 518, "sha256": "8b7d6f207e62f11e96efc4f5159d91db4472875c53bf3036b5b44c522eaf261a" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": [],
      "status": "not_shared",
      "evidence": "interior local frame: school door TA0 at (0, 0, 420) versus West TA1 at (-1870, 200, -1420); exit and arrival coordinates do not coincide (dossier 2.4)"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-195, 0, -525], "max": [196, 187, 555] },
    "sourceCollisionVertices": { "min": [-195, 0, -442], "max": [195, 187, 555] },
    "unityCollisionVertices": { "min": [-1.95, 0.0, -5.55], "max": [1.95, 1.87, 4.42] }
  },
  "anchors": [
    { "id": "anchor.sws.spawn0_from_west", "source": [-1, 0, 380], "unity": [-0.01, 0.0, -3.8], "status": "proposed" },
    { "id": "anchor.sws.door_ta0", "source": [0, 0, 420], "unity": [0.0, 0.0, -4.2], "status": "proposed" },
    { "id": "anchor.sws.instructor_a00", "source": [-3, 30, -45], "unity": [-0.03, 0.3, 0.45], "status": "proposed", "note": "dais top Y 30" },
    { "id": "anchor.sws.log_point0", "source": [0, 4, 92], "unity": [0.0, 0.04, -0.92], "status": "proposed", "note": "novice log pad, path 0 point 0" },
    { "id": "anchor.sws.expert_start", "source": [-3, 0, 255], "unity": [-0.03, 0.0, -2.55], "status": "proposed", "note": "a00 + (0, 0, 300) on the hall floor" },
    { "id": "anchor.sws.back_room_pot_a06", "source": [94, 45, -346], "unity": [0.94, 0.45, 3.46], "status": "proposed", "note": "back-room floor Y 45" }
  ],
  "rooms": [
    { "id": "loc.swordsmans_school.room_00", "sourceRoom": "Z2_DOUJOU_room_00", "header": 0, "roomShape": "cullable, 11 entries", "displayLists": 14 }
  ],
  "components": [
    { "sourceKey": "Z2_DOUJOU/h0/transition/ta00", "actorId": "0x005", "params": "0x0280", "class": "door-transition", "variant": "posed-skeletal", "dls": ["gameplay_keep:0x22B28 gDoorSkel", "object_wdor03:0x548"], "status": "selected", "note": "closed rest pose; schedule type SWORDSMANS_SCHOOL" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a04", "actorId": "0x19C", "params": "0xA07F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected", "note": "small wall clock, scale 0.01, noon pose" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a03", "actorId": "0x207", "params": "0x0000", "class": "static-mesh", "dls": ["object_dora:0x3FD0", "object_dora:0x4160"], "status": "selected", "note": "gong rest pose" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a02", "actorId": "0x209", "params": "0x0000", "class": "static-mesh", "dls": ["object_dora:0x180"], "status": "selected", "note": "intact board at the passage mouth" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a12", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80", "object_hariko:0x110"], "status": "selected" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a06", "actorId": "0x082", "params": "0x0202", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected", "note": "pot, scale 0.197, behind the training clamp" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a07", "actorId": "0x082", "params": "0x0602", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected", "note": "pot" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a08", "actorId": "0x082", "params": "0x0802", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected", "note": "pot" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a09", "actorId": "0x082", "params": "0x0A02", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected", "note": "pot" },
    { "sourceKey": "Z2_DOUJOU/h0/room_00/a10", "actorId": "0x082", "params": "0x0C02", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected", "note": "pot" }
  ],
  "collisionOwnership": {
    "room": "Z2_DOUJOUCollisionHeader_000CB8 (121 polys, 0 water boxes)",
    "waterBoxes": 0,
    "actorDynapoly": [],
    "actorCylinders": ["a00 instructor r18 h30", "a02 board r33 h80 plus 2 tris", "a03 gong 6 tris", "a06-a10 pots r12 h32", "En_Maruta logs r12 h65 (course only)"],
    "note": "training-area clamp z >= 25 is actor logic (a00), not collision"
  },
  "portalIds": [
    "portal.swordsmans_school.west_clock_town",
    "portal.swordsmans_school.west_clock_town.kickout"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a00 En_Kendo_Js (gSwordmasterSkel, default anim)", "En_Maruta logs and cut pieces (course only)", "Link and Tatl"],
  "excluded": { "halfDayMask": ["a01", "a05"], "stateOrSchedule": [], "noGeometry": ["a11"] },
  "unsupported": ["skeletal instructor pose", "door skeleton rest pose", "wall clock live time", "gong swing and half-hour rupee", "log rise, cut and retract", "board tumble", "course rules, score HUD and CIRCLE3 course camera", "z >= 25 training-area clamp"],
  "evidence": ["docs/city/interiors/swordsmans_school.md"],
  "readiness": {
    "sourceInspection": "pass",
    "objectExtraction": "not_run",
    "import": "not_run",
    "controllerTraversal": "not_run",
    "editorRender": "not_run",
    "packagedRender": "not_run",
    "visualApproval": "not_run"
  }
}
```

Camera proposals (registered IDs, no capture exists): `cam.swordsmans_school.entry` at spawn 0 facing -Z; `cam.swordsmans_school.reverse_door` from the pads toward the door and wall clock; `cam.swordsmans_school.fixture` = the training floor and seven pads, from the CIRCLE3 eye (-1, 125, -31) if the capture tool allows. Left out of `cameraIds` until captured, as in the accepted records.

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Course fees are C-BIN, not C | Confirm `firstChoicePrice 1` / `secondChoicePrice 10` of message `0x2716` with a decomp text-extraction output (`assets/text/message_data.h` generated at build) or an owner capture of the choice box |
| U-02 | Real-time length of the 720-frame expert course | SOURCE_REGISTER U-01 (VI rate x divisor 3); confirm that actor Update runs once per game frame here |
| U-03 | Whether the back room is ever walkable | With a00 killed (Day 3 00:00-05:59), check floor edge connectivity hall -> dais (riser 30 u, polys 8/9) -> step 115/116 -> landing, and whether the board's r33 cylinder plus tris block the 60 u passage until cut; then a route capture |
| U-04 | Room visuals: dais decoration, pad art, wall racks, the front-right fixture | Render the room DLs (cullable entries room.c:83-95) after the section 9 extraction; capture under `prof.d1.noon.normal` |
| U-05 | Header bounds larger than vertices (z -525 vs -442) | Same as North U-14: how `BgCheck` uses min/max bounds before using them as region bounds |
| U-06 | START0 / CIRCLE3 function data beyond pos (fov -1, 5, -1 and 260, 100) | Read `Camera_Unique0` and `Camera_Fixed2` in `src/code/z_camera.c` for which `BgCamFuncData` fields they consume |
| U-07 | `GI_RUPEE_RED` -> 20 rupees | Read the get-item table row for `GI_RUPEE_RED` (`include/tables/get_item_table.h` or the player's get-item table) |
| U-08 | Pot rupee persistence (collectible flags 1, 3-6) within a cycle | `Flags_GetCollectible` storage for scene `0x54` and the En_Item00 init kill for a set flag; low priority (pots are behind the clamp) |
| U-09 | Lesson, result and closed-door text content | Out of scope (ROM text); ids are listed in sections 2.5 and 5 |
| U-10 | `RESTRICTIONS` value 3 semantics (songOfDoubleTime, songOfStorms) | Read the users of `restrictions.songOfDoubleTime` / `songOfStorms` in `z_parameter.c` (LOCATION_REGISTER U-13) |
| U-11 | `ROOM_TYPE_3` behavior | Grep numeric room-type comparisons (`curRoom.type` with `>=`, `!=`, switch defaults) across `src/`; only the `z_camera.c:7000-7003` default branch is known |
| U-12 | Arrival START0 framing | Capture at spawn 0 to see what the UNIQUE0 camera at (-108, 53, 376) frames |
| U-13 | Cow statue support (Y 40 on a Y 30 dais) | Visual: room DL or another prop under it (U-04) |

**Wording notes (no edits made):** PORTAL_REGISTER rows for `portal.west_clock_town.swordsmans_school` and `portal.swordsmans_school.west_clock_town` say "open hours UNKNOWN"; 2.5 settles them (closed Day 3 23:00-23:59 from the street; kickout Day 3 23:00). The kickout row's text label can be the id `0x1891`. LOCATION_REGISTER U-02 resolves to padding for this scene. No count in any register disagrees with the C (7/13, 1 spawn, 1 TA, 2 exits with trailing `0x0000`, entrances 2 / 1).

**Not inspected:** room DL contents, combiners and geometry; light-setting values beyond the identical-row check; cutscene entries other than 7 and the player chain; minimap rendering; message text (only two header prices read); En_Kanban, En_Item00 and Obj_Tsubo break/drop behavior beyond the cited lines; camera function internals (U-06); 3DS material.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN sections 6/11/12, LOCATION_REGISTER, PORTAL_REGISTER (all rows for this venue), SOURCE_REGISTER sections 2-3, the North, East 2.4 and West 2.4.1 dossiers, the frozen schema README and `region.schema.json`, `shared/city/ids.json` and `shared/city/regions/laundry_pool.json`; read scene/room C and XML, the scene/actor/object/sequence tables, `z_actor.c`, `z_scene_table.c`, `z_play.c`, `z_camera.c`, `z_camera_data.inc.c`, `z_eventmgr.c`, `z_parameter.c`, `z_sram_NES.c`, `z_schedule.c`, `z_message.c`, `z_bgcheck.c`, `z64bgcheck.h`, `z64player.h`, `z64scene.h`, the player door code, and the overlays En_Kendo_Js, En_Maruta, Obj_Kendo_Kanban, Obj_Dora, Obj_Tokeidai, Obj_Tsubo, Obj_Hariko, En_Time_Tag, En_Test4, En_Door (with its schedule script); scratch Python `col.py` (collision, room DL and texture counts, SHA-256 of five inputs), `walls.py` (wall listing), `msg.py` (message header prices), outputs in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\swordsmans_school\`; grep cross-checks (`SCENE_DOUJOU`, `WEEKEVENTREG_KICKOUT_WAIT`, `ROOM_TYPE_3`, `PLAYER_GET_BG_CAM_INDEX`); JSON parse of the section 10 block (see below).

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes, `region_from_dossier.py`, `validate_city.py`.

**Next dependency:** the integrator lifts section 10 and runs `validate_city.py`; the section 9 extraction; then a training pass, which needs Link's sword move set (5.8) before INT-05 can be called playable.
