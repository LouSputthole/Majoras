# Post Office (`loc.post_office`): interior source dossier

Task **REF-09 / INT-04** (FULL_CITY_PLAN.md sections 6 and 12). Brief base commit `31b20dc65b5137de012fa60d373fb609c7af0aca` on `main`; HEAD when written was `7a7a08ce5d5589e4f80d65ece40e7ddfe1d47017` (PIPE-01 added `shared/city/`; nothing this dossier relies on changed). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_POSTHOUSE` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, text extraction, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save); the venue is open at that time (section 5.5).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`, `shared/` or `tools/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_POSTHOUSE/Z2_POSTHOUSE.c`, `room.c` = `.../Z2_POSTHOUSE_room_00.c` (line numbers are `grep -n` lines). `ovl/X` = `src/overlays/actors/ovl_X/`; `pm.c` = `ovl/En_Pm/z_en_pm.c`, `pm.schl` = `ovl/En_Pm/scheduleScripts.schl`, `mm3.c` = `ovl/En_Mm3/z_en_mm3.c`, `door.schl` = `ovl/En_Door/scheduleScripts.schl`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected. Original animation/DL names quoted from XML comments are decomp annotations (ANN).

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_POSTHOUSE` / `SCENE_POSTHOUSE` | `include/tables/scene_table.h:99` |
| Scene-table index | `0x2E` | same line |
| Entrance-table scene index | `0x2B` (`ENTR_SCENE_POST_OFFICE`) | `src/code/z_scene_table.c:2616`, `include/z64scene.h:636` |
| Entrance table | one entrance, one layer: `sPostOfficeEntrance0[] = { SCENE_POSTHOUSE, 0, 0x4102 }` | HDR `z_scene_table.c:930-936` |
| Title text / draw config | `0x111` / `SCENE_DRAW_CFG_DEFAULT` | scene_table.h:99 |
| Restrictions | `RESTRICTIONS_SET(0,0,0,0,0,3,0,0,3,0,0,1)`: Song of Double Time 3, Song of Storms 3, `all` 1; differs from `RESTRICTIONS_INDOORS` only by B button 0 (indoors = 1) | C, macro `src/code/z_parameter.c:66-77` |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_SET(3, 0, 0, 0)`: scene switch flags 0 and 1 survive the cycle reset | C, `src/code/z_sram_NES.c:15-33`, applied in `Sram_SaveEndOfCycle` `:430-464`; consumers in section 5.4 |
| XML files | scene segment 2; room `Z2_POSTHOUSE_room_00` segment 3 with DLs `0x7C90`, `0x88B8` | XML `assets/xml/scenes/Z2_POSTHOUSE/Z2_POSTHOUSE.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene / room headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene.c:13-31 or room.c:13-22 | HDR |
| Commands (scene) | sound settings spec 5, ambience 18, seq 31 (`Sequence_31` = `NA_BGM_INSIDE_A_HOUSE`, `include/tables/sequence_table.h:82`); 1 transition actor; `SET_REGION_VISITED`; special files `NAVI_QUEST_HINTS_NONE, GAMEPLAY_FIELD_KEEP`; 3 paths; 1 spawn; skybox `(0x00, 0, 0, true)`; 4 env light settings; 10 actor cutscenes; 2 actor cs cameras; minimap 1 empty entry (`0xFFFF`); animated material `{ 0, 6, NULL }` | HDR scene.c:14-29, :109, :131 |
| Light mode | skybox command `lightMode` true = `LIGHT_MODE_SETTINGS` (indexed light settings, not time-driven) | C, `include/z64environment.h:48-51`, `src/code/z_scene.c:376` |
| Animated materials | segment 0 entry is skipped by `AnimatedMat_DrawMain` | C, `src/code/z_scene_proc.c:400` |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x00, 0x00, 0, 0, false, 0)`; `SKYBOX_DISABLES(true, true)`; `TIME_SETTINGS(255, 255, 3)`; cullable shape (31 entries); 4 objects; 12 actors | HDR room.c:14-20 |

Selection note (C): with no alternate header, every load uses the default header. A cutscene entry would set `sceneLayer >= 1` (`src/code/z_play.c:2210-2219`), but the entrance table has only layer 0, so no layer-1 lookup exists for this scene (INF from `z_scene_table.c:930-936`).

Register cross-check: LOCATION_REGISTER row `loc.post_office` (r00 d 4/12, 1 transition actor, 1 spawn, 2 exits, entrances 2 / 1) agrees with the C on every count.

## 2. Spawns, entrances and exits

### 2.1 Start position (HDR scene.c:33-36; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`); yaw `(rawRotY >> 7) & 0x1FF`.

| Spawn | Position (Unity) | Raw rot X / Y / Z | Yaw | Params | Start mode | bgCam | Used by |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-10, 0, 265) (-0.10, 0.00, -2.65) | 0x0007 / 0x5A00 / 0x007F | 180 (faces -Z, into the room) | 0x0E01 | 0xE `PLAYER_START_MODE_E` | 0x01 (list has 1 entry, U-01) | Entrance 0 = `0x5600`, West exit index 7 (`Z2_ICHIBA.c:93`, alt list `:1556`); map select `ovl_select/z_select.c:337` |

Entrance list (HDR scene.c:46-49): two rows `{0x00, 0x00}`, `{0x00, 0x00}`; the list sits at 0xB0 and the exit list starts at 0xB4. Only entrance 0 is declared in `z_scene_table.c`, so row 1 cannot be selected through the entrance table (C); whether it is real data or padding is U-03.

### 2.2 Exit list (HDR scene.c:51-54) and consumers

| Exit idx | Raw | Decode (`Entrance_GetTableEntry`, `z_scene_table.c:2688-2694`) | Destination row | Consumer in this scene |
| --- | --- | --- | --- | --- |
| 0 | `0xD470` | 0x6A `Z2_ICHIBA`, entrance 7, layer 0 | `sWestClockTownEntrance7` `z_scene_table.c:2288-2290`: `SCENE_ICHIBA` spawn 7 -> (-1609, 200, -1368) yaw 0 params 0x0F04 (`Z2_ICHIBA.c:53-54`) | collision surface type 3 (polys 58, 59) behind the door; all three `EN_TIME_TAG` kickouts (`params & 0x1F = 0`) |
| 1 | `0xD650` | 0x6B `Z2_BACKTOWN`, entrance 5 (Bombers arrival) | North spawn 5 | none found: no room actor, door or surface indexes [1] (U-02; PORTAL_REGISTER U4) |

### 2.3 Collision surface carrying the exit (C from scene.c:142-145 and polygon line 149 + index)

| Surface type | data[0] / data[1] | Exit idx | Polys | Vertices (all Y 0) | Kind / flags |
| --- | --- | --- | --- | --- | --- |
| 3 | 0x00000100 / 0x00001002 | 0 -> `0xD470` | 58, 59 (scene.c:207-208) | x -40..20, z 300..391 (centroid of the pair (-10, 0, 345.5)) | floor, normal +Y; vIA exclusion flag 1 (`COLPOLY_IGNORE_CAMERA`, `include/z64bgcheck.h:66`); material stone, no conveyor |

The exit floor starts exactly at the door plane (z = 300) and lies wholly on the door's +Z side, away from the room (C).

### 2.4 Door into the venue (transition actor HDR scene.c:38-40; cross-check with West 2.4.1)

Mechanism (C): `EN_DOOR` params carry only type and type var (`ovl/En_Door/z_en_door.h:95-96`), spawned as `TRANSITION_ACTOR_PARAMS(i, p)` (`include/z64door.h:23`, `src/code/z_actor.c:3600-3625`). Opening a door places the player `22 * doorDirection` in front (`z_player.c:6756-6757`), probes the floor `22` units on the far side at door Y + 10 (`:6779-6781`, `func_80835BF8` `:6548-6551`) and passes it to `Player_HandleExitsAndVoids` (`:6783`). `doorDirection = +1` on the door's local +Z side (`z_en_door.c:496`); the schedule check runs only from that side (`:537`), text `0x1800 + result` (`:541`).

| Side | TA | Position | Yaw (raw) | Rooms front/back, cam | Params -> type / schedule | Door art | Day 1 12:00 schedule | Probe from the walkable side -> poly | Exit+1 -> idx -> value | Arrival |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| West (inbound) | `Z2_ICHIBA` TA0 | (-1610, 200, -1420) | 0 (0x007F) | 0/0, 255/255 | 0x0281 -> 5 `SCHEDULE` / 1 `POST_OFFICE` | `object_wdor02` `gPostOfficeTradingPostDoorDL` 0x548 | none = openable (`door.schl:36-53`) | street side (+Z) -> (-1610, 210, -1442) -> poly 57 (my probe; polys 56/57 type 8) | 8 -> 7 -> `0x5600` | Post Office spawn 0 (-10, 0, 265), 35 u inside the interior door |
| Post Office (return) | TA0 (`scene.c:39`) | (-10, 0, 300) | 0 (`127` = 0x007F) | 0/0, 255/255 | 0x0281 -> 5 `SCHEDULE` / 1 `POST_OFFICE` | same DL (`z_en_door.c:147`, `:213`, `:315`, `:347`) | schedule never runs from inside: the room is the door's -Z side (INF from geometry) | room side (-Z) -> (-10, 10, 322) -> poly 58 | 1 -> 0 -> `0xD470` | West spawn 7 (-1609, 200, -1368), 52 u in front of the West door, yaw 0 |

From the non-walkable side each probe lands on a non-exit floor (interior: (-10, 10, 278) -> poly 83, type 2), so each door transitions only from its walkable side (C). Both doors have yaw 0 with the building on their -Z side, so the interior and the West facade share orientation but not origin (offset West minus interior = (-1600, +200, -1720)) (INF). Agreement with `docs/city/districts/west_clock_town.md` 2.4.1 and PORTAL_REGISTER rows: schedule type, exit polygons (56/57 and 58/59), return spawn 7 and "open 12:00-13:00" all match. One West-side observation for that dossier's owner: polys 56/57 reach z -1400, 20 u in front of the West door plane (z -1420) (U-19).

### 2.5 Portal IDs (all exist in `docs/city/PORTAL_REGISTER.md` and `shared/city/ids.json`)

| Portal ID | Evidence | Reverse / arrival |
| --- | --- | --- |
| `portal.west_clock_town.post_office` (inbound) | West exit [7] `0x5600`, polys 56/57, TA0 | arrives at spawn 0 |
| `portal.post_office.west_clock_town` | exit [0] `0xD470`, surface 3, polys 58/59, TA0 | West spawn 7 |
| `portal.post_office.west_clock_town.kickout` | exit [0] via `EN_TIME_TAG 0x00C0` x3 (room.c:47, :50, :53) | West spawn 7 (one-way) |

Single room: no room portal is needed. Exit [1] `0xD650` has no consumer and no portal row (U-02).

## 3. Object banks (HDR room.c:19, :24-29; IDs from `include/tables/object_table.h`)

| # | Bank | Object ID | Consumer (OVL) | Needed for Day 1 static |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_MM` | 0x107 (`object_table.h:280`) | En_Pm profile (`pm.c:300`); En_Mm3 spawned child (`mm3.c:33`) | skeletal (later) |
| 1 | `OBJECT_OBJ_TOKEIDAI` | 0x18C (`:413`) | Obj_Tokeidai wall clocks (profile `ovl/Obj_Tokeidai/z_obj_tokeidai.c:67`) | yes (already extracted) |
| 2 | `OBJECT_WDOR02` | 0x275 (`:646`) | En_Door leaf DL `sObjectInfo[18]` (`z_en_door.c:213`) | yes |
| 3 | `OBJECT_POSTHOUSE_OBJ` | 0x208 (`:537`) | Bg_Breakwall types 4 and 5 by `Object_GetSlot`; the actor is killed if the bank is absent (`ovl/Bg_Breakwall/z_bg_breakwall.c:76-78`, `:219-224`) | yes |

Implicit: `gameplay_keep` (En_Talk, En_Time_Tag, En_Test4, En_Mm2 and En_Door profiles; door skeleton `gDoorSkel` 0x22B28) and `gameplay_field_keep` (scene special file, no listed consumer here).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): actor ID `id & 0x1FFF`; each axis `(raw >> 7) & 0x1FF` degrees, converted with `DEG_TO_BINANG` unless the ID carries `0x8000` (rotY), `0x4000` (rotX) or `0x2000` (rotZ), in which case the value is passed as a plain integer (wrapped to -180..180). csId = `rawRotY & 0x7F` (0x7F none, 3570-3574); half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all (3576-3580).
- `Actor_SpawnSetupActors` (2546-2577) spawns an entry only when its mask has the current bit; `Actor_InitHalfDaysBit` (2491-2499): Day 1 12:00 -> 0x080. Bits: D0d 0x200, D0n 0x100, D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001. No entry has ID bit 0x800.
- Flag users here: `En_Talk | 0xE000` reads `home.rot.x - 1` as attention range type (`ovl/En_Talk/z_en_talk.h:6-7`); `En_Time_Tag | 0xE000` reads `home.rot.x` / `home.rot.y` as kickout hour / minute (`ovl/En_Time_Tag/z_en_time_tag.h:12-15`).

Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled`, `billboard-effect`, `controller-invisible`, `door-transition`. "D1d" = mask includes Day 1 daytime.

### 4.2 Default header, room_00 (12 entries, HDR room.c:31-67, entry i at line 32 + 3i; decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_PM` (0x1D5) | (116, 26, -219) | 0x0007 / 0x87FF / 0x007F | 0 / 271 / 0 | 0x3FF all | none | 0x0000 (path 0) | object_mm | npc-scheduled | yes |
| 1 | `ACTOR_BG_BREAKWALL` (0x036) | (-120, 0, -280) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0005 (type 5, switch 0) | object_posthouse_obj | static-mesh | yes |
| 2 | `ACTOR_BG_BREAKWALL` (0x036) | (-80, 0, -310) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0204 (type 4, switch 1) | object_posthouse_obj | static-mesh | yes |
| 3 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (199, 92, -77) | 0x0107 / 0x877F / 0x0078 | int 2 / -90 / 0 | 0x3F8 (all but D3n, D4d, D4n) | none | 0xFE0B (text 0x1C0B) | gameplay_keep | controller-invisible | yes |
| 4 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (-194, 59, -12) | 0x0107 / 0x2D7F / 0x007F | int 2 / 90 / 0 | 0x3FF all | none | 0xFE0B (text 0x1C0B) | gameplay_keep | controller-invisible | yes |
| 5 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-28, 0, 164) | 0x0687 / 0x007F / 0x007F | int 13 / 0 / 0 | 0x3FF all | none | 0x00C0 | gameplay_keep | controller-invisible | yes |
| 6 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-88, 0, 99) | 0x0007 / 0x007F / 0x0070 | int 0 / 0 / 0 | 0x3F0 D0d-D2n | none | 0x00C0 | gameplay_keep | controller-invisible | yes |
| 7 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-81, 0, 188) | 0x0780 / 0x007F / 0x003F | int 15 / 0 / 0 | 0x03F D2d-D4n | none | 0x00C0 | gameplay_keep | controller-invisible | no |
| 8 | `ACTOR_EN_TEST4` (0x15A) | (-9, 0, 365) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | 0x3FF all | 9 | 0xFD00 | gameplay_keep | controller-invisible | yes |
| 9 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (191, 105, 176) | 0x0007 / 0x877F / 0x007F | 0 / 270 / 0 | 0x3FF all | none | 0x907F (type 9) | object_obj_tokeidai | static-mesh | yes |
| 10 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (134, 84, -275) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x907F (type 9) | object_obj_tokeidai | static-mesh | yes |
| 11 | `ACTOR_EN_MM2` (0x0FE) | (114, 20, -182) | 0x0000 / 0x007F / 0x0004 | 0 / 0 / 0 | 0x004 D3n only | none | 0x0000 | gameplay_keep (overlay DL) | static-mesh | no |

Transition actor (all headers): TA0 `ACTOR_EN_DOOR` (0x005), params 0x0281, mask `HALFDAYBIT_ALL` (`z_actor.c:3618-3620`), class door-transition (section 2.4).

### 4.3 Overlay findings per actor type (OVL)

| Actor | Init: scale, collision, children | Draw | Presence / state at Day 1 noon (fresh save) |
| --- | --- | --- | --- |
| En_Pm (Postman) | flex `object_mm_Skel_0096E8`, scale 0.01; cylinder r14 h62 and sphere r26 colliders (`pm.c:308-347`, init `:1897-1914`); schedule script chosen at init (`:1909`) and advanced per frame (`:1932`) | skeletal via `SubS_DrawTransformFlex`, only if `scheduleResult != 0` (`:2024-2047`); cap 0x85C8 / bag 0x83E0 / letter 0x8348 limbs gated by `unk_356` bits (`:875-899`, `:1955-1992`) | present; script 0 result 9 = asleep on the bed, no cap or bag (section 5.1, 5.3) |
| Bg_Breakwall type 5 (`z2_post_cloth_model`, ANN) | ICHAIN scale 1, then x scale 0.1 while switch 0 is clear (`z_bg_breakwall.c:174-179`); dynapoly `object_posthouse_obj_Colheader_002948` (`:77`, `:258-261`); update eases x scale to 1.0 when switch 0 is set, back to 0.1 when clear (`:279-288`) | opaque `object_posthouse_obj_DL_001870` (`:428-433`) | present, folded (x 0.1): INF curtain |
| Bg_Breakwall type 4 (`z2_post_cap_model`, ANN) | scale 0.1; draw NULL if switch 1 set (`:166-172`); update re-checks switch 1 every frame (`:271-277`) | opaque `object_posthouse_obj_DL_000A50` | present (switch 1 clear): INF the postman's spare cap on its hook |
| En_Talk | scale 1; text `(p & 0x3F) + 0x1C00` = 0x1C0B; attention type 1 (`ovl/En_Talk/z_en_talk.c:29-40`) | none | present; talk within 40 u while facing, offer 120 (`:57-60`); text NI (U-05) |
| En_Time_Tag (kickout) | type `(p & 0xE000) >> 13 = 0` `KICKOUT_DOOR`, text 6 `GENERIC`, exit 0 (`z_en_time_tag.h:10-13`, `:24`, `:49`) | none | a05, a06 present, a07 masked out (section 5.6) |
| En_Test4 | three-day controller; params read as s16: stars `(p >> 10) * 100` = -100, bell volume index `(p >> 5) & 0xF` = 8 (`ovl/En_Test4/z_en_test4.h:8-9`, U-14) | none | present; Day 3 00:00 first-cycle transition (section 5.7) |
| Obj_Tokeidai type 9 `WALL_CLOCK` | scale 0.02; `ObjTokeidai_Clock_Init`; action `ObjTokeidai_WallClock_Idle` tracks `CURRENT_TIME` (`z_obj_tokeidai.c:104-115`, `:247-252`, `:719-723`) | `ObjTokeidai_Clock_Draw`: minute ring 0xCF28, center/hand 0xBEE8, wall face `gWallClockClockFaceDL` 0xF518, sun/moon panel 0xC368 (`:811-847`; XML `object_obj_tokeidai.xml:47-67`) | present, noon pose |
| En_Mm2 (Postman's letter to himself) | scale 0.015; talk within 60 u while facing, offer 110, text 0x277B (`ovl/En_Mm2/z_en_mm2.c:33-75`) | opaque overlay DL `sEnMm2DL` (`:83-91`; XML `assets/xml/overlays/ovl_En_Mm2.xml`) | masked out (Day 3 night only) |
| En_Door | `gDoorSkel` with limb DL from `sObjectInfo[DOOR_OBJKIND_SCHEDULE + schType]` (`z_en_door.c:329-347`); only framed doors load collision (`:338-340`) | skeletal leaf | present |

### 4.4 Counts

| | Default (only) header |
| --- | --- |
| Entries / transition actors | 12 / 1 |
| Day 1 daytime candidates by mask | 10 (excluded: 7, 11) |
| static-mesh | 5 (1, 2, 9, 10, 11) |
| npc-scheduled | 1 (0) |
| controller-invisible | 6 (3-8) |
| posed-skeletal / billboard-effect | 0 / 0 |
| door-transition | 1 (TA0) |

## 5. Venue mechanics index (INT-04 scope)

### 5.1 Postman schedule in this room (`ovl_En_Pm`)

Script selection (OVL): `func_80AF7B40` (`pm.c:386-404`), run in `EnPm_Init` (`:1909`) and advanced live by `func_80AF7BAC` (`:406-438`, called at `:1932`), picks `D_80AFB900[]` (`:1829-1831`): `WEEKEVENTREG_90_01` -> 4 `D_80AFB41C`, `89_40` -> 3 `D_80AFB3FC`, `89_08` -> 2 `D_80AFB3C0`, `86_01` -> 1 `D_80AFB30C`, else 0 `D_80AFAD80`. Scripts 1-4 run on a relative clock `CURRENT_TIME - D_801F4E78` (`:1836-1841`, `:1855-1858`); `D_801F4E78` is a non-saved global (`src/code/z_kankyo.c:39`) set when each flag appears (`pm.c:406-438`, `z_kankyo.c:1751-1764`). Only scripts 0 and 1 name `SCENE_POSTHOUSE`. Time tests: `if_time_range` is `start <= now <= end - 1` and `return_time` stores `end - 1` (`src/code/z_schedule.c:25-49`, `:169-186`), in script time shifted by 06:00 (`include/z64scripts.h:7`).

Script 0, `SCENE_POSTHOUSE` windows (OVL `pm.schl`; outside them the result is none and the postman is neither drawn nor updated, `pm.c:1843-1858`, `:1933`, `:2032`):

| Day | Flag condition | Window (source literal) | Result | Lines |
| --- | --- | --- | --- | --- |
| 1 | any | 12:00-13:01 | 9 | 70-75 |
| 1 | any | 15:00-00:05 | 8 | 130-135 |
| 2 | `LETTER_TO_KAFEI_SCHEDULED_DAY_2` (28 0x08) | 12:00-13:01 | 21 | 140, 143-148 |
| 2 | same | 17:00-00:05 | 8 | 210-215 |
| 2 | not set | 12:00-13:01 | 20 | 221-226 |
| 2 | not set | 13:01-15:00 | 14 (`return_l`, no time pair) | 227-232 |
| 3 | `..._DAY_2` | 12:00-13:01 / 18:00-06:00 | 20 (`return_l`) / 24 | 267-283 |
| 3 | `LETTER_TO_KAFEI_SCHEDULED_DAY_3` (28 0x10) | 12:00-13:01 / 18:00-06:00 | 21 / 24 | 287-303 |
| 3 | neither | 12:00-13:01 / 18:00-06:00 | 22 / 24 | 309-314, 370-375 |

Door crossings in West that bracket these windows (script 0, `SCENE_ICHIBA`, results 1 = leave, 2 = enter; both open the Post Office door, `pm.c:465-495`): Day 1 09:00-09:03 (1), 11:57-12:00 (2), 13:01-13:04 (1), 14:56-15:00 (2) (`pm.schl:6-13`, `:78-85`); Day 2 with flag 13:01-13:04 (1), 16:57-17:00 (2) (`:152-159`). Inside the room there is no walk-in: results 9/8 place the postman directly (INF from the handlers below).

Script 1 (`D_80AFB30C`, after `WEEKEVENTREG_86_01`, relative clock T; `pm.schl:384-396`): Post Office T 00:00-00:10 result 82, 00:10-00:13 result 27, 00:13-00:23 result 83, 00:23-00:26 result 32; then West, South, East and Milk Bar results 33-36, 84-88, 28 (`:397-430`).

Result handlers (dispatch `func_80AF9BF8`, `pm.c:1410-1541`; per-frame `func_80AFA438` `:1725-1826`; message choice `EnPm_GetMsgScript` `:765-831`):

| Result | Setup | Placement | Anim (`sAnimationInfo`, `pm.c:369-384`; ANN name) | Talk | Message (text) | Side effects |
| --- | --- | --- | --- | --- | --- | --- |
| 9, 20, 21, 22 | `func_80AF992C` `:1334-1354` | (116, 26, -219), rot `0xC0BA` (271 deg), gravity -1 | 8 `0xFC4` "mm_neru" | range 80, attention type 6 | 9 0x2775 (`:93-98`), 20 0x2777 (`:100`), 21 0x2781 (`:121`), 22 0x277A (`:128`); Kafei's Mask -> 0x2369 (`:797-799`, `:267-272`) | snore `NA_SE_VO_NP_SLEEP_OUT` on frame 0 (`:1709-1716`); sphere collider instead of cylinder (`:562-584`, center on limb 08 `:1977-1985`) |
| 14 | same | same | 13 `0xC640` "mm_yoru" (`:1346-1348`) | only while the player is within 90 deg of his facing (`:1693-1701`) | 0x2778 (`:107`) | |
| 8 | `func_80AF98A0` `:1323-1332` | spawns `ACTOR_EN_MM3` at (116, 26, -219), rotY `-0x3F46`, then kills itself | | | | counting game, section 5.2 |
| 24 | `func_80AF94AC` `:1203-1234` | path 0 point 0 (32, 0, -165) facing point 1 | 9 `0xA8D8` "mm_uzukumaru" | range 60, facing only | `D_80AFB658` (`:150-196`) | clears switches 0 and 1 (`:1224-1226`) |
| 82 | `func_80AF91E8` `:1121-1201` | time-path on path 0, (32, 0, -165) -> (-81, 0, -317) | 0 run | none | | |
| 27 | `func_80AF95E8` `:1236-1321` | path 0 last point (-81, 0, -317) | 3 wait | onscreen offer | default 0x2774 (`:86-91`) | curtain SFX, set switch 0 (`:1279-1284`); 20 frames later set switch 1 (`:1703-1706`) |
| 83 | `func_80AF91E8` | path 1, (-81, 0, -317) -> (-11, 0, 255) | 0 | none (`:1165-1177`) | | previous result 27 -> curtain SFX, clear switch 0 (`:1159-1162`); cap and bag drawn (`0x9000`) |
| 32 | `func_80AF9008` `:1073-1119` | path 2, (-11, 0, 255) -> (-9, 0, 366) | 0 | attention off (`:1107`) | | opens the `POST_OFFICE` door (`door->openTimer`, `:1544-1556`; `z_en_door.c:476-480`) |

Path list (HDR scene.c:67-93): path 0 = 4 points, next 1, custom 43; path 1 = 7 points, next 2, custom 44; path 2 = 2 points, none, custom 45. Result-to-path table `D_80AFB430` (`pm.c:25-29`) gives 24, 27, 82 -> path 0, 83 -> 1, 32 -> 2 via `SubS_GetAdditionalPath` (`src/code/z_sub_s.c:485-499`) (C, scratch `paths.py`).

### 5.2 Ten-second counting game (`ovl_En_Mm3`, "Counting Game Postman")

| Item | Source fact | Cite |
| --- | --- | --- |
| Availability | only while En_Pm result 8 holds: Day 1 15:00-00:05; Day 2 17:00-00:05 with `LETTER_TO_KAFEI_SCHEDULED_DAY_2` | `pm.schl:130-135`, `:210-215` |
| Actor | En_Mm3, profile `OBJECT_MM`, spawned as En_Pm's child at (116, 26, -219); Y snapped to the floor; looping anim 5 `0xCD90` "uttekure" | `mm3.c:30-39`, `:92-113`, `:132-137` |
| Talk | offer 100 while the player is in front (90 deg); first text 0x278A | `mm3.c:122-152` |
| First play per form (free) | per-instance bits `unk_2B2`: Human 0x01, Human + Bunny Hood 0x10, Deku 0x02, Zora 0x04, Goron 0x08; unseen form -> 0x278B (or 0x27A0 with Bunny Hood) -> 0x278C -> 0x278D -> 0x278E choice | `mm3.c:211-261`, `:478-541` |
| Repeat in the same form (paid) | 0x279D -> 0x279E sets 0x20 -> 0x279F -> 0x278E; choice 0 pays `msgCtx.firstChoicePrice` (else 0x279C, error SFX); choice 1 -> 0x278F | `mm3.c:154-209`, `:245-261`; price value NI (U-04) |
| Start | 0x2790 done -> `PLAYER_STATE1_20`, `Interface_StartPostmanTimer(0, bunny)`: count-up from 0 | `mm3.c:354-367`; `src/code/z_parameter.c:854-872` |
| Timer display | drawn always with Bunny Hood; without it only while under 3.00 s | `z_parameter.c:5653-5658` |
| Stop | A press read in `Interface_PostmanTimerCallback` stores `osGetTime()`; elapsed = stop OS time - start - pauses (frame-rate independent); forced stop above 15.00 s | `z_parameter.c:823-834`, `:5714-5735`; `mm3.c:414-417` |
| Snap window | 9.95-10.05 s (centiseconds 995-1005, `XREG(16)`/`XREG(17)` = 0) becomes exactly 10.00 | `mm3.c:403-413`; `include/z64save.h:79-85`; `src/code/z_debug.c:18-19` |
| Verdict | ==10.00 -> 0x2792 win (fanfare `:317-326`); >=15.00 -> 0x2797; 9.50-10.50 -> 0x2795; else 0x2796; losses -> 0x2798 -> 0x2799 -> 0x279A paid retry | `mm3.c:263-312` |
| Reward | `GI_HEART_PIECE` until `WEEKEVENTREG_RECEIVED_POSTMAN_COUNTING_GAME_HEART_PIECE` (77 0x01, set on receipt), then `GI_RUPEE_PURPLE`; notebook `MET_POSTMAN`, `RECEIVED_POSTMAN_HP` | `mm3.c:444-476`; `include/z64save.h:1355`; flag kept across cycles (`sPersistentCycleWeekEventRegs`, `src/code/z_sram_NES.c:43-49`, `:234`) |
| Kickout interplay | start sets `WEEKEVENTREG_KICKOUT_WAIT` (63 0x01), clears `KICKOUT_TIME_PASSED` (63 0x02); after a loss with time passed -> 0x279B instead of retry; both cleared when the talk ends and on destroy | `mm3.c:391-398`, `:294-301`, `:368-371`, `:115-120` |
| Audio | player/item/env/enemy/ocarina/voice banks muted, BGM to 0 during counting; tick SFX | `mm3.c:391-394`, `:432-436` |

### 5.3 Bed and sleep state

| Item | Source fact | Tag |
| --- | --- | --- |
| Asleep | results 9, 20, 21, 22 at (116, 26, -219): anim "mm_neru", snore SFX, sphere collider | OVL (5.1) |
| Lying down for the game | En_Mm3 plays `NA_SE_EV_LIE_DOWN_ON_BED` on frame 8 of its talk anim (`mm3.c:384-388`) at the same spot | OVL |
| Bed surface | collision type 10 (carpet, wall type 1) at Y 16, x 40..177, z -280..-166 (polys 303-306), with a wood frame (types 1, 8) at x 177..200, Y 34..45; En_Pm home and En_Mm2 lie inside it | C; "bed" is INF from the SFX and positions (U-09) |
| Day 3 night | result 24 crouches at (32, 0, -165), not on the bed; the letter En_Mm2 (D3n) sits on the bed at (114, 20, -182) | OVL / HDR |

### 5.4 Delivery and state flow

- **Express mail to Mama (Day 3 night, result 24):** `D_80AFB658` checks `ITEM_LETTER_MAMA`; without it 0x277C (first, sets `WEEKEVENTREG_82_80`) or 0x277D; with it the player may present `PLAYER_IA_LETTER_MAMA` -> 0x277E, sets `82_80` and `86_01`, deletes the item (`pm.c:150-196`). `func_80AF7BAC`, called every update (`:1932`), sees `86_01`, switches the live instance to script 1 and starts the relative clock (`:406-412`); a fresh init would also pick script 1 (`:386-404`): in-room sequence 82 -> 27 -> 83 -> 32 (5.1), then the Madame Aroma scene in the Milk Bar (`D_80AFB6BC`, sets `89_08`, `:198-239`). Later stages `85_80` -> `89_08` -> `89_40` -> `90_01` advance in `Environment_UpdatePostmanEvents` (`z_kankyo.c:1750-1764`); the hat (`GI_MASK_POSTMAN`, `90_04`) is given outside, East result 29 (`pm.c:241-265`, `pm.schl:458-466`). Out of room scope beyond the door.
- **Letter to Kafei:** postbox checks set `LETTER_TO_KAFEI_SCHEDULED_DAY_2/3`, `..._TODAY` and `POSTMAN_RECEIVED_LETTER_TO_KAFEI` (`pm.c:941-972`; off-screen `z_kankyo.c:1660-1706`, day catch-up `:1708-1734`; received flag cleared outside 09:00-12:00 `:1736-1738`). In this room they select results 21 vs 20/22, move the Day 2 counting game to 17:00 and change door hours (5.5); the carried letter DL 0x8348 shows while the received flag is set (`pm.c:1108-1110`, `:1188-1190`).
- **Scene switches:** switch 0 = curtain (Bg_Breakwall type 5), switch 1 = cap hidden (type 4); written only by En_Pm (`pm.c:1159-1162`, `:1224-1226`, `:1279-1284`, `:1703-1706`). Both survive a cycle reset (section 1), so after a completed delivery the wall cap stays hidden in later cycles until a Day 3 night result 24 clears it (INF from the code paths).
- Messages tied to other results seen elsewhere (16, 17, 18, 25, 28, 29) are not reachable in this scene (results never returned for `SCENE_POSTHOUSE`).

### 5.5 Door open hours (`sDoorSch_PostOffice`, `door.schl:28-102`; exterior door only)

Result none = openable; else text `0x1800 + result` (`z_en_door.c:537-546`): 0x1808 "out delivering mail", 0x1809 closed (comments in the script; text content NI). `if_before_time` is `now < t` (`z_schedule.c:188-203`); `if_since_time` is its negation (`tools/buildtools/schc.py:167-168`, `:473-474`).

| Day | 06:00-08:59 | 09:00-11:59 | 12:00-12:59 | 13:00-... | Evening / night |
| --- | --- | --- | --- | --- | --- |
| 1 | closed 0x9 | delivering 0x8 | open | 13:00-14:59 delivering 0x8 | 15:00-23:59 open; 00:00-05:59 closed 0x9 |
| 2, `..._DAY_2` set | closed | delivering | open | 13:00-16:59 delivering | 17:00-23:59 open (script comment says 15:00); 00:00-05:59 closed |
| 2, not set | closed | delivering | open | closed 0x9 from 13:00 to 05:59 (`@bug` comment, `:76-80`) | closed |
| 3 | closed | delivering | open | 13:00-17:59 closed 0x9 (`:87-95`) | 18:00-05:59 open |

The interior door never runs this schedule (2.4). En_Pm opens either door itself through `openTimer` during results 1, 2, 32, 33 (`pm.c:1544-1556`).

### 5.6 Kickout (`portal.post_office.west_clock_town.kickout`)

| Entry | Position | Fires at | Mask | Text | Exit |
| --- | --- | --- | --- | --- | --- |
| a05 | (-28, 0, 164) | 13:00 | all half-days | `0x1883 + 6` = 0x1889 (`GENERIC`) | [0] `0xD470` -> West spawn 7 |
| a06 | (-88, 0, 99) | 00:00 | D0d-D2n (0x3F0) | 0x1889 | [0] |
| a07 | (-81, 0, 188) | 15:00 | D2d-D4n (0x03F) | 0x1889 | [0] |

Behavior (OVL `z_en_time_tag.c:282-339`): fires only when hour and minute equal the tag exactly and no cutscene runs; with `KICKOUT_WAIT` set it records `KICKOUT_TIME_PASSED` and waits for both flags to clear; then text, `nextEntrance = setupExitList[0]`, door SFX. Day 3 has no 00:00 tag, so the player may stay from 18:00 to 06:00 (INF from masks). Purpose of a07 when 13:00 already expels the player is U-07.

### 5.7 Other room mechanics

| Item | Source fact | Cite |
| --- | --- | --- |
| Day 3 midnight, first cycle | En_Test4 (any scene holding it): at the 00:00 bell on Day 3 with no Ocarina, sets return respawn, `nextEntrance = ENTRANCE(SOUTH_CLOCK_TOWN, 0)`, `nextCutsceneIndex = 0xFFF1` | OVL `z_en_test4.c:499-525`; no portal ID (U-08) |
| Wall clocks | two clocks follow `CURRENT_TIME` (hour/minute rotation) | OVL 4.3 |
| Signs | two invisible talk spots, both text 0x1C0B; a03 at Y 92 on the east wall is absent D3n-D4 | HDR / OVL |
| Letter to himself | En_Mm2 on the bed, Day 3 night only, text 0x277B | HDR mask 0x004 |

### 5.8 Cameras (HDR scene.c:95-106, :116-128, :138-140; `include/z64cutscene.h:626-637`, `include/z64camera.h:159, 178, 191`)

| Item | Value |
| --- | --- |
| BgCamInfo | 1 entry: setting 0x000C `CAM_SET_NORMAL1`; every surface type uses bgCam 0; spawn 0 asks for bgCam 1 (U-01); door sides bgCam 255 (generic door camera) |
| Actor cs cameras | [0] setting 31 `CAM_SET_FIXED1`, pos (-18, 34, -194), rot (-4187, 24394, 0); [1] setting 44 `CAM_SET_START0`, pos (119, 133, 250), rot (6918, -19843, 0) |
| Actor cutscene list | 10 entries; 0-8 chain through `additionalCsId` 1..8 with global camera ids -3, -2, -4, -5, -7, -11, -8, -9, -6; entry 9 (priority 900, csCamId 0 -> cs camera [0], additional 9) is En_Test4's csId 9. No entry uses cs camera [1]; consumers of 0-8 NI |
| Registered capture IDs | `cam.post_office.entry` (from spawn 0 facing -Z), `cam.post_office.reverse_door` (toward TA0 at z 300), `cam.post_office.fixture` (proposed fixture: the counter partition with the bed behind it, INF identities) |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header)

Open at this time (5.5). Assumptions: fresh save (no week-event, switch or collectible flags), Human form, En_Pm script 0. World = actorPos + actorRot x (actorScale x localVertex) (ROM_REFERENCE.md); rotation under the Z mirror belongs to the importer.

| # | Component / params | N64 position (Unity) | Yaw | Scale | Display list(s), bank | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room | room_00 DLs (cullable, 31 entries) + collision `Z2_POSTHOUSECollisionHeader_002158` | as written | | 1 | scene/room C (already extracted) | 22 textures + 2 TLUTs in the scene file |
| TA0 | Door leaf, 0x0281 | (-10, 0, 300) (-0.10, 0.00, -3.00) | 0 | default | `gameplay_keep` `gDoorSkel` 0x22B28 + `object_wdor02` 0x548 on limb 4 | rest pose approximated; same leaf as the Trading Post |
| 1 | Curtain, 0x0005 | (-120, 0, -280) (-1.20, 0.00, 2.80) | 0 | (0.1, 1, 1) | `object_posthouse_obj` 0x1870 | dynapoly 0x2948 at the same scale (INF, U-12) |
| 2 | Cap on hook, 0x0204 | (-80, 0, -310) (-0.80, 0.00, 3.10) | 0 | 0.1 | `object_posthouse_obj` 0xA50 | drawn because switch 1 is clear |
| 9 | Wall clock, 0x907F | (191, 105, 176) (1.91, 1.05, -1.76) | 270 | 0.02 | `object_obj_tokeidai` 0xCF28, 0xBEE8, 0xF518, 0xC368 | noon pose via `ObjTokeidai_Clock_Init` (U-15) |
| 10 | Wall clock, 0x907F | (134, 84, -275) (1.34, 0.84, 2.75) | 0 | 0.02 | same | same |

**Present but skeletal (deferred):** 0 En_Pm asleep at (116, 26, -219) (1.16, 0.26, 2.19), yaw 271, anim `object_mm` 0xFC4, skeleton 0x96E8, no cap/bag; resting on the bed top (Y 16) under gravity (INF).

**Excluded by half-day mask:** 7 (15:00 kickout, D2-D4), 11 En_Mm2 (D3n).

**No geometry:** 3, 4 En_Talk; 5, 6 En_Time_Tag; 8 En_Test4.

Registered profiles that show the counting game (result 8, En_Mm3 on the bed): `prof.d1.dusk.normal` (17:15) and `prof.d1.night.normal` (21:00). Day 3 night (`prof.d3.night.normal`) shows result 24 and, on the bed, the letter a11.

## 7. Collision summary (C from scene.c:138-828; scratch `col.py`, `col_out.txt`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 285 / 381 / 12 (all vertices referenced) |
| Water boxes | 0 (`0, NULL`, scene.c:827) |
| Header bounds = vertex extents | min (-200, 0, -360), max (200, 200, 391); Unity x -2.00..2.00, y 0.00..2.00, z -3.91..3.60 |
| Polygon kinds | 95 floors (normal Y > 0.5), 256 walls, 30 ceilings |
| Floor slopes | 83 under 5 deg; 12 at 35-50 deg (small trim pieces) |
| Materials (surface data[1] & 0xF) | stone 2: 65 floors, 152 walls, 24 ceilings; wood 10: 10 floors, 56 walls; dirt 0: 12 floors, 48 walls, 6 ceilings; carpet 13: 8 floors |
| Wall type 1 (`WALL_FLAG_0`, blocks the ledge climb at `z_player.c:11835`) | types 0, 1, 7, 10; 192 walls |
| Exit surfaces | type 3 only, polys 58/59 (2.3) |
| Exclusion flags | 116 polys with `COLPOLY_IGNORE_CAMERA` (types 0, 2, 3, 5, 6); no vIB flags; no conveyor, hookshot, floor-property, soft or horse flags |
| Light index (data[1] >> 6 & 0x1F) | 0 on all floors except types 11 (shelf at Y 44) and the type 5/7/9 walls (31) |
| Echo | 2 on every type except 11 (0) |
| Camera | 1 BgCamInfo (5.8) |

Floor levels (centroid Y; INF identities from shape and actors, U-10):

| Y | Polys | Extent x / z | Types | INF identity |
| --- | --- | --- | --- | --- |
| 0 | 31 | -200..200 / -340..391 | 2, 3 (exit), 4 (carpet rug x -48..120, z -160..-16) | main floor |
| 16 | 6 | 40..177 / -280..-166 (carpet, polys 303-306) and 85..103 / 8..30 (wood) | 10, 8 | bed top; small wooden box |
| 34-45 | 8 | 147..200 / -280..-41 | 1, 2, 8 | bed frame (x 177..200) and a ledge by the east wall |
| 40-41 | 12 | -80..200 / 40..80, 160..200 / 40..280, box top 103..107 / 8..30 | 1, 2, 8 | counter top (L-shaped along the east wall) with small raised items |
| 52-64 | 12 | 190..197 / -99..-82 and 0..16 / 43..56 | 2 | small sloped pieces (east-wall fixture; item on the counter) |
| 96-100 | 14 | -72..184 / 56..64 | 2, 6 | partition above the counter: posts at x -72..-64, 31..39, 83..91, 176..184 reach Y 100; openings between them with sills at Y 44 (outer two) and Y 65 (middle) |
| 20-32, 44, 112, 128 | 12 | west and north walls | 2, 11 | niche step (Y 20) and lintel (Y 128), west-wall shelves |

Total 95 floors. Ceilings: Y 160 over z -280..40, Y 200 over z 40..280, Y 120 over a north-wall niche (x -120..-40, z -360..-272), Y 100 over the door alcove (x -40..20, z 280..391). The niche holds the cap (a02) and path 0's end point (-81, 0, -317); the curtain (a01) stands at its west edge (x -120), so INF: the niche is the changing alcove that result 27 closes. The postman's path 1 passes the open west end of the counter (x -142), so the area behind the counter is walkable from the door side at x -200..-80 (INF from paths and floor extents).

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 24: 22 textures + 2 TLUTs (`Z2_POSTHOUSETLUT_009590`, `_0095B0`) |
| By format | ci8 13, i4 3, ia4 2, rgba16 1 (+2 TLUTs), i8 1, ia8 1, ci4 1 |
| Location | all in the scene file (segment 2); the room file declares none; all 24 are referenced by room DLs |
| Room `Gfx` arrays / `Vtx` arrays | 40 / 32 (32 `.vtx.inc` files, 1,422 vertices, x -313..330, y 0..200, z -360..391; wider than collision in x, U-13) |
| Room shape | cullable, 31 entries: 28 with an opaque DL, 9 with an XLU DL |
| Raw triangle commands | 367 `gsSP2Triangles` + 4 `gsSP1Triangle` = 738 triangles in source (DL reuse not resolved) |
| Actor-side materials (not extracted) | `object_posthouse_obj`: 4 rgba16 textures (8x8, 16x16, 32x32, 64x32) for the cap and cloth (XML); `object_wdor02`: 1 rgba16 32x64; `object_mm`: 11 ci8 + 1 TLUT + 3 rgba16 + 1 i8; `ovl_En_Mm2`: i8 32x32 envelope, rgba16 16x16 letter |

## 9. Required extraction list (commands NOT executed)

Already extracted: scene `Z2_POSTHOUSE`; `object_obj_tokeidai` (South pilot). Binaries for every bank below exist under `extracted/n64-us/baserom/` (checked by `os.path.exists`: object_posthouse_obj 10,624 bytes, object_wdor02 5,808, object_mm 55,904, gameplay_keep 599,520, ovl_En_Mm2, ovl_En_Mm3).

| Priority | Inputs | Why |
| --- | --- | --- |
| Day 1 static | `object_posthouse_obj`, `object_wdor02`, `gameplay_keep` | curtain, cap, door leaf and skeleton (section 6) |
| Later skeletal pass | `object_mm` | postman (asleep, counting game, Day 3 crouch) |
| State variants | `assets/xml/overlays/ovl_En_Mm2.xml`, `ovl_En_Mm3.xml` | Day 3 letter; counting-game segment 0x0C DL |
| Text (owner decision) | `tools/extract_text.py` | game price, signs 0x1C0B, door 0x1808/0x1809, kickout 0x1889, postman 0x2774-0x27AE |

```powershell
# run from local-reference/mm-decomp; same ZAPD binary and flags as tools/reference/README.md:29-32
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_posthouse_obj','object_wdor02','gameplay_keep','object_mm')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
# overlay assets: same flags, unverified for overlay XML (BaseAddress/RangeStart attributes)
foreach ($ovl in @('ovl_En_Mm2','ovl_En_Mm3')) {
    & $zapd e -i "assets/xml/overlays/$ovl.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/overlays/$ovl" -osf "extracted/n64-us/assets/overlays/$ovl" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
}
```

`gameplay_keep` has not been extracted with this tool before (NI); the text extractor's arguments and output location are NI and its output must stay out of the repository.

## 10. Proposed region record

Schema 1 (PIPE-01, `shared/city/schema/region.schema.json`); lifted and validated by the integrator, not by this packet.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.post_office",
  "displayLabel": "Post Office",
  "source": {
    "sceneSymbol": "Z2_POSTHOUSE",
    "sceneEnum": "SCENE_POSTHOUSE",
    "sceneTableIndex": "0x2E",
    "entranceTableSceneIndex": "0x2B",
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
      "player form Human",
      "En_Pm schedule script 0 (WEEKEVENTREG_86_01, 89_08, 89_40, 90_01 clear)",
      "scene switch flags 0 and 1 clear (they persist across cycles)"
    ]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_POSTHOUSE", "bytes": 38832, "sha256": "1c88d04972274d36fdab3d9416c4fb84a738c9edb2cbb22e355b570523055630" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_POSTHOUSE_room_00", "bytes": 35088, "sha256": "6dbf593ed508ffea807e9fe66f6bb6ff8821158231b0a746efa7e1902c703cd3" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_POSTHOUSE/Z2_POSTHOUSE.c", "bytes": 45385, "sha256": "baf4928b390af6ccac15edbaeced96abb69dd3b637dcc21d5f347f0943be6804" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_POSTHOUSE/Z2_POSTHOUSE_room_00.c", "bytes": 69466, "sha256": "4c9cea1eb91a8ee73a9b982021bd8764a4179c5a5fab17b8c95354f35f765878" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_POSTHOUSE/Z2_POSTHOUSE.xml", "bytes": 410, "sha256": "0f83b376e84b9bd6a94940ca11ad9af7b87468d8476f267258d4e63a99c3caff" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": [],
      "status": "not_shared",
      "evidence": "separate interior scene; door TA0 (-10,0,300) vs West TA0 (-1610,200,-1420); both doors yaw 0 with the building on -Z (INF orientation match only)"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-200, 0, -360], "max": [200, 200, 391] },
    "sourceCollisionVertices": { "min": [-200, 0, -360], "max": [200, 200, 391] },
    "unityCollisionVertices": { "min": [-2.0, 0.0, -3.91], "max": [2.0, 2.0, 3.6] }
  },
  "anchors": [
    { "id": "anchor.po.spawn0_from_west", "source": [-10, 0, 265], "unity": [-0.1, 0.0, -2.65], "status": "proposed" },
    { "id": "anchor.po.door_ta0", "source": [-10, 0, 300], "unity": [-0.1, 0.0, -3.0], "status": "proposed" },
    { "id": "anchor.po.postman_bed", "source": [116, 26, -219], "unity": [1.16, 0.26, 2.19], "status": "proposed", "note": "En_Pm and En_Mm3 home; bed top Y 16 (collision type 10)" },
    { "id": "anchor.po.counter_east_corner", "source": [200, 40, 40], "unity": [2.0, 0.4, -0.4], "status": "proposed", "note": "counter-top vertex, polys 219-222" },
    { "id": "anchor.po.wall_clock_a10", "source": [134, 84, -275], "unity": [1.34, 0.84, 2.75], "status": "proposed" }
  ],
  "rooms": [
    { "id": "loc.post_office.room_00", "sourceRoom": "Z2_POSTHOUSE_room_00", "header": 0, "roomShape": "cullable, 31 entries", "displayLists": 40 }
  ],
  "components": [
    { "sourceKey": "Z2_POSTHOUSE/h0/transition/ta00", "actorId": "0x005", "params": "0x0281", "class": "door-transition", "variant": "posed-skeletal", "dls": ["gameplay_keep:0x22B28 gDoorSkel", "object_wdor02:0x548"], "status": "selected", "note": "rest pose approximated" },
    { "sourceKey": "Z2_POSTHOUSE/h0/room_00/a01", "actorId": "0x036", "params": "0x0005", "class": "static-mesh", "dls": ["object_posthouse_obj:0x1870"], "status": "selected", "note": "curtain, x scale 0.1 while switch 0 clear; dynapoly 0x2948" },
    { "sourceKey": "Z2_POSTHOUSE/h0/room_00/a02", "actorId": "0x036", "params": "0x0204", "class": "static-mesh", "dls": ["object_posthouse_obj:0xA50"], "status": "selected", "note": "cap on hook, hidden once switch 1 is set" },
    { "sourceKey": "Z2_POSTHOUSE/h0/room_00/a09", "actorId": "0x19C", "params": "0x907F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected", "note": "wall clock, scale 0.02, noon pose" },
    { "sourceKey": "Z2_POSTHOUSE/h0/room_00/a10", "actorId": "0x19C", "params": "0x907F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected", "note": "wall clock, scale 0.02, noon pose" }
  ],
  "collisionOwnership": {
    "room": "Z2_POSTHOUSECollisionHeader_002158 (381 polys, 0 water boxes)",
    "waterBoxes": 0,
    "actorDynapoly": ["a01 object_posthouse_obj:0x2948 curtain (x scale 0.1 while switch 0 clear)"],
    "actorCylinders": ["a00 En_Pm cylinder r14 h62, sphere r26 while asleep"]
  },
  "portalIds": [
    "portal.post_office.west_clock_town",
    "portal.post_office.west_clock_town.kickout"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": [
    "a00 En_Pm asleep on the bed (object_mm skeleton, schedule result 9)",
    "door leaf skeleton pose",
    "Link and Tatl"
  ],
  "excluded": {
    "halfDayMask": ["a07", "a11"],
    "stateOrSchedule": [],
    "noGeometry": ["a03", "a04", "a05", "a06", "a08"]
  },
  "unsupported": [
    "skeletal NPC pose and sleep loop",
    "curtain scale animation and cross-cycle switch persistence",
    "wall clock hand motion",
    "counting-game timer UI and OS-time A-button stop",
    "door schedule and kickout timing"
  ],
  "evidence": [
    "docs/city/interiors/post_office.md"
  ],
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

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Spawn 0 asks for bgCam 1; BgCamInfo has 1 entry | Which code consumes `PLAYER_GET_BG_CAM_INDEX` for the start camera, and what an out-of-range index selects (same question as North U-02) |
| U-02 | Exit [1] `0xD650` (North spawn 5) has no reader here | Search every overlay that can run in `SCENE_POSTHOUSE` (including cutscene `nextEntrance` and the Bombers notebook flow) for `setupExitList[1]`, or record it unused (PORTAL_REGISTER U4) |
| U-03 | PORTAL_REGISTER section 6 lists "Post Office 1" as a teleport-only spawn; the scene has 1 spawn and entrance row 1 has no `z_scene_table.c` entrance | Register owner: confirm from the binary layout (0xB0-0xB3) whether row 1 is padding and drop it from the teleport-only list |
| U-04 | Counting-game price (`msgCtx.firstChoicePrice`) | Read the price control code in text 0x278E and 0x279A via `tools/extract_text.py` (owner decision, output outside the repo) |
| U-05 | Text of 0x1C0B, 0x1808, 0x1809, 0x1889, 0x2369, 0x2774-0x27AE | Same text extraction; which wall object the two 0x1C0B spots label |
| U-06 | `unk_356` bits 0x2000 / 0x4000 that switch result 24 to `D_80AFB74C` / `D_80AFB764` (`pm.c:781-791`) | Where are they set: `SubS_Offer` item exchange or `MsgEvent_RunScript` (`src/code/z_sub_s.c`, `z_msgevent.c`)? |
| U-07 | Purpose of the 15:00 kickout (a07, D2-D4) | Which path keeps the player inside past 13:00 (for example a talk or cutscene spanning 13:00, since the tag requires `!Play_InCsMode`, `z_en_time_tag.c:333-334`)? Needs a capture |
| U-08 | Day 3 00:00 first-cycle transition from this room (`z_en_test4.c:499-525`) has no portal ID | Integrator: does PORTAL_REGISTER need one global special row for En_Test4, or is it out of the per-venue graph? |
| U-09 | Bed identity | Render cullable entries (120, 22, -223) r102 and (108, 17, -223) r90 against polys 303-306 |
| U-10 | Counter and partition identity | Render the DLs over Y 40-100 at z 40-80 |
| U-11 | Pose meaning of "mm_neru", "mm_yoru", "mm_uzukumaru", "uttekure" (ANN) | Extract `object_mm` and view the animations |
| U-12 | Curtain collision follows actor x scale | Confirm in the DynaPoly SRT update (`src/code/z_bgcheck.c`, `DynaPoly_ExpandSRT` and callers) |
| U-13 | Room vertices reach x -313..330, collision only -200..200 | Map each cullable entry's DL to its vertex extents (door alcove, exterior trim?) |
| U-14 | En_Test4 params 0xFD00 as s16 (stars -100, bell volume index 8) | Confirm `params` signedness in `THREEDAY_GET_*` users and that stars are unused with the skybox disabled |
| U-15 | Wall clock noon pose | Apply `ObjTokeidai_Clock_Init` (`z_obj_tokeidai.c:104-115`) at 12:00 as ROM_REFERENCE.md does for the tower clock |
| U-16 | Light index 31 on some surfaces | What `Environment_ChangeLightSetting` does with 31 when the player stands on the type 11 shelf |
| U-17 | Script 1 relative clock `D_801F4E78` is not saved | Does a save/reload during the delivery reset the sequence (`z_kankyo.c:39`, `:1751-1764`)? |
| U-18 | Visual survey | Capture `cam.post_office.entry`, `.reverse_door`, `.fixture` on `prof.d1.noon.normal`; counting game on `prof.d1.night.normal` |
| U-19 | West polys 56/57 extend to z -1400, 20 u in front of the West door plane | West dossier owner: is a wall at z -1420 keeping the player off that strip, or can the floor exit fire without opening the door (`z_player.c:12572`)? |

Register notes for their owners (not edited here): PORTAL_REGISTER rows `portal.west_clock_town.post_office` and `portal.post_office.west_clock_town` say "open hours UNKNOWN"; section 5.5 answers them. SOURCE_REGISTER U-10 for this venue: open 12:00-12:59 on all three days.

**Not inspected:** room DL contents and combiners; light-setting values; minimap; cutscene entries 0-8 consumers; `object_posthouse_obj` curtain collision contents; En_Pm results outside this scene beyond the lines cited; message data; 3DS material.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN sections 6, 11, 12, the three registers, North and East/West dossier templates and the schema README, `region.schema.json`, `laundry_pool.json`; read scene/room C, XML, scene/actor/object/sequence tables, `z_actor.c`, `z_schedule.c`, `z_scene_table.c`, `z64bgcheck.h`, `z64player.h`, `z_player.c` door/exit lines, `z_kankyo.c` postman events, `z_parameter.c` timer code, `z_sram_NES.c` persistence, and overlays En_Pm, En_Mm2, En_Mm3, Bg_Breakwall, En_Talk, En_Time_Tag, En_Test4, Obj_Tokeidai, En_Door. Scratch scripts (stdlib, read-only) in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\post_office\`: `col.py` (collision counts, levels, probes), `mat.py` (texture/DL counts, SHA-256 of the five inputs), `paths.py` (result-to-path decode), `west_check.py` (West polys 56/57 and probe). Counts agree with LOCATION_REGISTER; portal rows agree with PORTAL_REGISTER and `shared/city/ids.json`.

**NOT run:** ZAPD (no object or text extracted), Blender, Unity, any player or executable, captures, renders, controller routes, `region_from_dossier.py`, `validate_city.py`.

**Next dependency:** integrator lifts section 10 and runs `validate_city.py`; owner decision on text extraction (U-04/U-05); section 9 extraction before ROOM-04 assembly; NPC-01 for the postman schedule and SYS-03 for the counting game.
