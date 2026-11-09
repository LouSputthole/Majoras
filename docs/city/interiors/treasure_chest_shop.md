# Treasure Chest Shop (`loc.treasure_chest_shop`): source inventory dossier

Task **REF-09 / INT-11** (FULL_CITY_PLAN.md section 12 row REF-09; the INT-11 row is in section 6, not section 5 as REF09_BRIEF.md says). Base commit `c8b847e` on `main` (HEAD at dispatch; `main` moved to `7ab0640` while this packet ran: register batch edits, `tools/reference/extract_messages.py` and the INT-17 dossier; everything below was checked against that tree, and section 10 against its `shared/city/ids.json`). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_TAKARAYA` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`, `shared/` or `tools/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_TAKARAYA/Z2_TAKARAYA.c`, `room.c` = `.../Z2_TAKARAYA_room_00.c`. Line numbers count `\n` only (scene.c holds 4 lone `\r` bytes, room.c 9). Polygon `i` is on scene.c line `125 + i`, vertex `i` on line `225 + i`, surface types on lines 119-121. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. The interior uses its own frame; it is not placed in the town frame.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected; **MSG** header numbers and control-code skeleton read from the decoded message table (`tools/reference/extract_messages.py` output, ignored local JSON, read-only; no text is reproduced here, only ids, numbers and macro names; the same prices were decoded independently from the baserom binary by scratch `msgprice.py`); **SIM** statistical port of the maze generator in scratch (uniform Python RNG, not the game's).

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01), `docs/city/PORTAL_REGISTER.md` (REF-02), `docs/city/SOURCE_REGISTER.md` (REF-03), `docs/city/districts/east_clock_town.md` section 2.4 (the street door TA#2). Agreements and wording notes are in section 11.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_TAKARAYA` / `SCENE_TAKARAYA` | `include/tables/scene_table.h:53` |
| Scene-table index | `0x17` | same line |
| Entrance-table scene index | `0x14` (`ENTR_SCENE_TREASURE_CHEST_SHOP`) | `src/code/z_scene_table.c:2593`, `include/z64scene.h:613` |
| Title card text / draw config | `0x109` / `SCENE_DRAW_CFG_DEFAULT` | scene_table.h:53 |
| Restrictions | `RESTRICTIONS_INDOORS` = `RESTRICTIONS_SET(0, 1, 0, 0, 0, 3, 0, 0, 3, 0, 0, 1)`: by argument name bButton 1, songOfDoubleTime 3, songOfStorms 3, all 1; value meanings NI | scene_table.h:53; `src/code/z_parameter.c:60-75` |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_SET(2, 0, 0, 0)`: switch0 mask 2 = switch flag 1 only; chest, collectible and switch1 masks 0 (section 5.7) | scene_table.h:53; `src/code/z_sram_NES.c:15-33` |
| XML files | scene `Z2_TAKARAYA` segment 2; room `Z2_TAKARAYA_room_00` segment 3 with DLs `0x77B0` and `0x7E68` | XML `assets/xml/scenes/Z2_TAKARAYA/Z2_TAKARAYA.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene.c:13-29 | HDR |
| Room headers | default only (room.c:13-21) | HDR |
| Transition actors | 1 `EN_DOOR` | HDR scene.c:16, :39-41 |
| Region visited | `SET_REGION_VISITED` (scene.c:17); the scene is in the Clock Town region list | HDR; `src/code/z_inventory.c:428-435` |
| Special files | `SCENE_CMD_SPECIAL_FILES(NAVI_QUEST_HINTS_NONE, GAMEPLAY_DANGEON_KEEP)`: the dungeon keep is loaded as the persistent sub-keep in segment 5; none of the Bomb Shop or Lottery Shop scenes has this command. No segment-5 reference in the room DLs (grep); consumer not traced (U-05) | HDR scene.c:20; `src/code/z_scene.c:226-238` |
| Sound | spec 5, ambience 19, seq 0 (`NA_BGM_GENERAL_SFX` by table order); the shop music is positional from a04 and a sub-BGM plays during a game (section 5.11) | HDR scene.c:14; `include/z64scene.h:845-846`; `include/tables/sequence_table.h:20` |
| Skybox | file 0, skybox 0, weather 0, lightMode true = `LIGHT_MODE_SETTINGS`; room `SKYBOX_DISABLES(true, true)` | HDR scene.c:22, room.c:16; z64scene.h:833-837 |
| Env light settings | 4 entries; entries 0 and 1 are identical except the `blendRateAndFogNear` word (`0x07CA` versus `0x07D9`); entries 2 and 3 differ and no collision surface selects them (section 7); values not transcribed | HDR scene.c:57-66 |
| Actor cutscenes / cs cameras | 11 entries / 1 entry | HDR scene.c:25-26, :68-80, :96-98 |
| Cutscene scripts / paths | none / none | HDR |
| Minimap | 1 entry `{0xFFFF, 0xFCB8, 0x0000, 0x01E0, 0}`: no map id, x -840, y 0, z 480 (the midpoints of the collision bounds x -1680..0 and z 0..960, INF), scale 0 | HDR scene.c:82-88 |
| Animated materials | single placeholder `{0, 6, NULL}`, skipped because segment is 0 | HDR scene.c:104-106; `src/code/z_scene_proc.c:400` |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x00, 0x00, 0, 0, false, 0)`; `SKYBOX_DISABLES(true, true)`; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3, 5 without the Ocarina); cullable shape type 2 with 16 entries; 4 objects; 5 actors | HDR room.c:14-20; `src/code/z_scene.c:399-402` |

### 1.1 Alternate headers: none

The entrance table has two entrances, each with a single layer row (`sTreasureChestShopEntrance0[]`, `sTreasureChestShopEntrance1[]`, `z_scene_table.c:376-387`), and the scene has no alternate list, so every `sceneLayer` loads the default scene and room header (`Scene_CommandAltHeaderList` only runs when the command exists, `src/code/z_scene.c:464-474`). Day, night and flag variation comes only from actor half-day masks (section 4) and overlay logic (section 5). Profiles that matter, all on header 0: `prof.d1.noon.normal` (open, gal present, kickout armed), `prof.d1.night.normal` (21:00, open, one hour before the door closes), `prof.d1.late.normal` (23:00, door closed, section 5.8) and `prof.d3.night.normal` (no gal, section 4.2).

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:32-37; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam index `params & 0xFF` (`include/z64player.h:17-18`). Yaw = `(rawRotY >> 7) & 0x1FF`; csId = `rawRotY & 0x7F`.

| Spawn | Position (Unity) | Raw rot X / Y / Z | Yaw | csId | Params | Start mode | bgCam | Used by |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-70, 0, 161) (-0.70, 0.00, -1.61) | 0x0007 / 0x0000 / 0x007F | 0 (faces +Z, into the shop) | 0 | 0x0EFF | 0xE `Player_StartMode_E` | 0xFF none | Entrance 0 = `0x2800`, East exit index 4 (`Z2_TOWN.c:101`): arrival of `portal.east_clock_town.treasure_chest_shop` |
| 1 | (-91, 0, 744) (-0.91, 0.00, -7.44) | 0x0007 / 0x0000 / 0x007F | 0 (faces +Z, toward the gal) | 0 | 0x0DFF | 0xD `Player_StartMode_D` | 0xFF none | Entrance 1 = `0x2810`, set only by En_Takaraya when a game ends (below) |

- bgCam `0xFF` skips the actor-cs camera override at spawn (`src/code/z_play.c:2326-2327`); the floor's BgCam then applies (section 7). Mode E sets walk speed 2.0 and a 120-unit walk-in, mode D a 180-unit walk-in (`ovl_player_actor/z_player.c:8677-8690`; the walk helper `func_8083A878` NI). RotY low 7 bits are passed to the player actor as csId 0 (`z_actor.c:3653`); whether Player reads it: NI (U-03).
- Entrance rows (C): `sTreasureChestShopEntrance0[0] = {SCENE_TAKARAYA, 0, 0x4102}` (`z_scene_table.c:376-378`), `sTreasureChestShopEntrance1[0] = {SCENE_TAKARAYA, 1, 0x0102}` (:380-382), table :384-387. Scene `EntranceEntry` rows `{0x00, 0x00}`, `{0x01, 0x00}` (HDR scene.c:47-50): entrance 0 -> start 0 room 0, entrance 1 -> start 1 room 0. Flags: exit fade `0x02`, arrival `(flags >> 7) & 0x7F = 2` in both; bit 14 is set only in entrance 0 (meaning NI, U-04); bit 15 clear.
- **What entrance 1 / spawn 1 is for (C, OVL):** the post-game return point. When a game ends, `EnTakaraya` sets `play->nextEntrance = ENTRANCE(TREASURE_CHEST_SHOP, 1)` (= `0x2810`, `z_en_takaraya.c:343`) and the reloaded actor tests `gSaveContext.save.entrance == ENTRANCE(TREASURE_CHEST_SHOP, 1)` (:119). It lands the player 105 units in front of the gal (-85, 0, 849). No city exit uses it: `0x2810` occurs nowhere in `src/` or in the 22 extracted scene files (grep), and `ENTRANCE(TREASURE_CHEST_SHOP` appears only at z_en_takaraya.c:119 and :343 plus the debug map select for entrance 0 (`ovl_select/z_select.c:310`, excluded as in REF-02). This is PORTAL_REGISTER's "Treasure Chest Shop 1".

### 2.2 Exit list (HDR scene.c:52-55) and consumers

| Exit idx | Raw | Decode (`Entrance_GetTableEntry`, `z_scene_table.c:2688-2694`) | Destination row | Consumer |
| --- | --- | --- | --- | --- |
| 0 | `0xD240` | 0x69 East Clock Town, entrance 4, layer 0 | `sEastClockTownEntrance4[0] = {SCENE_TOWN, 4, 0x4102}` (`z_scene_table.c:2216-2218`) -> East start 4 (610, 100, -173) yaw 180, params 0x0F0C (East dossier 2.1) | surface 6, floor polys 69, 70 through the door probe (2.4); a03 En_Time_Tag kickout, `params & 0x1F = 0` (5.8) |
| 1 | `0xD650` | 0x6B North Clock Town, entrance 5, layer 0 | North start 5 (the Bombers arrival per PORTAL_REGISTER U4) | none found: no surface has exit+1 = 2 (section 7), En_Time_Tag reads index 0, and of the 15 overlays that index `setupExitList` (list in East dossier section 2.2) only En_Time_Tag and Player (via collision) occur here. INF: dead data (U-01) |

Inbound: East `Z2_TOWNExitList_0001F0[4] = 0x2800` (`Z2_TOWN.c:101`). The second exit is not padding (`0xD650` is non-zero), consistent with LOCATION_REGISTER's "2" without "(last 0x0000)".

### 2.3 Collision surface carrying the exit (C)

`exitIndexPlusOne` = bits 8-12 of `SurfaceType` word 0 (`include/z64bgcheck.h:79-87`). Surface type 6 = `{0x00000100, 0x00001002}` (scene.c:120, third entry): exit+1 = 1 -> exit 0. Polygons 69 and 70 (scene.c:194-195): one quad at Y 0, x -100..-40, z 20..100, normal +Y, `COLPOLY_IGNORE_CAMERA`, stone, no conveyor. Mean vertex (-70, 0, 60): 40.0 units behind the door on its axis (PORTAL_REGISTER "40u" agrees). A floor, so a live exit (`Player_HandleExitsAndVoids`, `ovl_player_actor/z_player.c:6405-6416`). No other surface carries an exit bit.

### 2.4 The door (transition actor ta0) and the return route (C)

`TransitionActorEntry { sides[2] {room, bgCamIndex}, id, pos, rotY, params }` (`include/z64scene.h:369-378`); spawn `src/code/z_actor.c:3600-3625`.

| Field | Value | Evidence |
| --- | --- | --- |
| Raw entry | `{ 0, 255, 0, 255, ACTOR_EN_DOOR, -70, 0, 100, 23167, 0x0288 }` | HDR scene.c:40 |
| Sides | front room 0, bgCam -1 (generic door camera); back room 0, bgCam -1 | z64scene.h:371-374 |
| Position | (-70, 0, 100); Unity (-0.70, 0.00, -1.00) | HDR |
| Yaw | rotY 23167 = `0x5A7F`; `(rotY >> 7) & 0x1FF` = 180; `DEG_TO_BINANG(180)` = `0x8000`. Door forward (local +Z) = world -Z, toward the exit pad (`z_actor.c:3613`; `include/z64math.h:168-172`) | C |
| csId / mask | `0x5A7F & 0x7F = 0x7F` none; mask always `HALFDAYBIT_ALL` | C |
| Params | `TRANSITION_ACTOR_PARAMS(0, 0x0288)` = 0x0288 (`include/z64door.h:23`): type `(p >> 7) & 7` = 5 `ENDOOR_TYPE_SCHEDULE`, type var 8 = `ENDOOR_SCH_TYPE_TREASURE_CHEST_SHOP` (`ovl_En_Door/z_en_door.h:9-20`, :63). Identical to East TA#2 | C |
| Leaf art | `sObjectInfo[DOOR_OBJKIND_SCHEDULE + 8]` = `DOOR_OBJINFO_25` `{ -1, DOOR_DL_DEFAULT, GAMEPLAY_KEEP }` = `gDoorLeftDL` 0x20BB8 / `gDoorRightDL` 0x20D00 on `gDoorSkel` 0x22B28 (side by eye yaw) | OVL `z_en_door.c:154`, :220, :307, :347; `assets/xml/objects/gameplay_keep.xml:931-941` |
| Schedule side | the schedule runs only for `playerPosRelToDoor.z > 0` (`z_en_door.c:537`), the exit-pad side. A player inside the shop is on local -Z (spawn 0 at door-local z = -61), so leaving never meets the closed message | OVL, C |

Door probe (C, scratch `analyze.py`): opening a non-fake door raycasts the floor 22 units behind it along its yaw at door Y + 10 and passes that polygon to `Player_HandleExitsAndVoids` (`z_player.c:6779-6783`; `func_80835BF8` :6548-6551).

| Player side | `doorDirection` | Probe point | Floor hit | Result |
| --- | --- | --- | --- | --- |
| Inside the shop (z > 120) | -1 (`z_en_door.c:496`) | (-70, 10, 78) | poly 70, surface 6, exit+1 = 1 | exit 0 `0xD240` -> East spawn 4 |
| Exit-pad side (never reachable) | +1 | (-70, 10, 122) | poly 64, surface 4, no exit | door opens, no scene change |

The doorway at z = 120 is closed by collision wall polys 22-23 (x -100..-40, Y 0..100, normal +Z), so the pad cannot be walked onto; the player leaves only through the door action (C).

### 2.5 Door-to-venue cross-check with East Clock Town (east_clock_town.md 2.4)

| Item | East side | Treasure Chest Shop side | Result |
| --- | --- | --- | --- |
| Door actor | TA#2 (610, 100, -140), raw rotY `0x5A7F` yaw 180, params 0x0288 | ta0 (-70, 0, 100), raw rotY `0x5A7F` yaw 180, params 0x0288 | same schedule type, same leaf art |
| Exit trigger | surface 20, polys 481, 482, exit index 4 `0x2800` | surface 6, polys 69, 70, exit index 0 `0xD240` | agrees with East dossier and PORTAL_REGISTER |
| Arrival | Treasure Chest Shop spawn 0 (-70, 0, 161) yaw 0 mode 0xE | East spawn 4 (610, 100, -173) yaw 180 mode 0xF bgCam 0x0C | agrees |
| Schedule-checked side | the street (front) | the exit stub | the closed message is seen only when entering from the street |
| Open hours | East: "closed since 22:00" (`scheduleScripts.schl:174-180`), end bound open (East U-11) | same script | resolved in 5.8: open 06:00-21:59, closed 22:00-05:59, every day |

Frames are independent (East Y 100, interior Y 0); no shared-frame claim is made.

### 2.6 Portal IDs (registered in `shared/city/ids.json`; no new IDs)

| Portal ID | Mode | Source evidence | Arrival |
| --- | --- | --- | --- |
| `portal.treasure_chest_shop.east_clock_town` | door | ta0 + surface 6 (polys 69, 70), exit 0 `0xD240` | East start 4 |
| `portal.treasure_chest_shop.east_clock_town.kickout` | special | a03 En_Time_Tag type 0, exit 0, 22:00, text `0x1892`, mask D0d..D2n | East start 4 |
| Inbound `portal.east_clock_town.treasure_chest_shop` (East owns it) | door | East TA#2 + surface 20, exit 4 `0x2800` | start 0 here |

One room and one transition actor with both sides in room 0: no internal room portal exists or is needed. The game-end return `ENTRANCE(TREASURE_CHEST_SHOP, 1)` is an in-scene reload, not a city portal; it has no ID (section 5.9).

## 3. Object banks (HDR room.c:24-29; IDs from `include/tables/object_table.h`)

Actor spawn fails if the profile's object is not loaded (`Actor_SpawnAsChildAndCutscene`, `src/code/z_actor.c:3517-3523`), which is why the runtime-spawned wall and chest need their banks declared here.

| # | Bank | Object ID | Consumer (OVL) | Needed for Day 1 static list |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_BOX` | 0x00C (`:29`) | En_Box chest, spawned by the wall actor after payment (`z_en_box.c:61`) | no (runtime) |
| 1 | `OBJECT_BG` | 0x129 (`:314`) | a02 En_Takaraya profile object, `gTreasureChestShopGalSkel` (`z_en_takaraya.c:38`) | skeletal (later) |
| 2 | `OBJECT_TAKARAYA_OBJECTS` | 0x1B4 (`:453`) | Obj_Takaraya_Wall profile object, wall cell DLs (`z_obj_takaraya_wall.c:43`) | no (runtime) |
| 3 | `OBJECT_OBJ_TOKEIDAI` | 0x18C (`:413`) | a01 Obj_Tokeidai wall clock | yes (already extracted) |

Implicit: `gameplay_keep` (door skeleton and leaves; profiles of En_Test4, En_Time_Tag, Obj_Sound) and the scene sub-keep `GAMEPLAY_DANGEON_KEEP` (section 1). All four listed banks have a consumer.

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): id `& 0x1FFF`; each rotation `(raw >> 7) & 0x1FF`, converted with `DEG_TO_BINANG` unless the id carries the axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), in which case it is a plain integer. csId = `rawRotY & 0x7F`, 0x7F = none (3570-3574, :3653); half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all (2562-2571, :3576-3580, :3654).
- `Actor_InitHalfDaysBit` (2491-2499): Day 1 at 12:00 -> `0x080`. Bits: D0d 0x200, D0n 0x100, D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001 (`include/z64actor.h:328-341`).
- `Actor_Init` (`z_actor.c:1166-1171`) sets scale 0.01 before the actor's own Init; En_Takaraya never changes it.
- Classes as in the brief; "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (5 entries, HDR room.c:32-46, decode C; scratch `analyze.py`)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_TEST4` (0x15A) | (-95, 0, -67), outside the collision bounds (z < 0) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0100 | gameplay_keep | controller-invisible | yes |
| 1 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-300, 160, 831) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF all | none | 0x907F | object_obj_tokeidai | static-mesh | yes |
| 2 | `ACTOR_EN_TAKARAYA` (0x1C1) | (-85, 0, 849) | 0x0007 / 0x5A0A / 0x0078 | 0 / 180 / 0 | **0x3F8 = D0d..D3d** | **10** | 0x0000 | object_bg | npc-scheduled (flex skeleton) | yes |
| 3 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-171, 0, 417) | 0x0B07 / 0x007F / 0x0070 | int 22 / 0 / 0 | **0x3F0 = D0d..D2n** | none | 0x01E0 | gameplay_keep | controller-invisible | yes |
| 4 | `ACTOR_OBJ_SOUND` (0x0F0) | (47, 260, 462) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0146 | gameplay_keep | controller-invisible | yes |

Transition actor ta0 (`EN_DOOR` 0x005, door-transition, all half-days) is in section 2.4. a02 is absent from Day 3 night onward (bits 0x004, 0x002, 0x001 are outside 0x3F8) and a03 from Day 3 onward (0x3F0), so `prof.d3.night.normal` has neither gal nor kickout. a03's id carries all three axis flags (`0xE000`), so its rotations are plain integers: hour = `home.rot.x` = 22, minute = `home.rot.y` = 0 (`z_en_time_tag.h:14-15`).

### 4.3 Runtime-spawned actors (not in any list)

| Key | Spawner | Actor | Position | Rot X / Y / Z | Params | csId | When |
| --- | --- | --- | --- | --- | --- | --- | --- |
| a02/wall | En_Takaraya `EnTakaraya_SpawnWalls` (`z_en_takaraya.c:191-202`) | `ACTOR_OBJ_TAKARAYA_WALL` 0x1BB, `ACTORCAT_BG` | (0, 0, 0) | 0 / 0 / 5 (rotZ = switch flag 5) | `(ENBOX_TYPE_BIG_SWITCH_FLAG << 12) + (getItemId << 5) + 0`: 0xB0A0 purple rupee, 0xB180 Heart Piece, 0xB080 red rupee, 0xB540 10 nuts, 0xB520 5 nuts | 10 | on payment |
| a02/chest | Obj_Takaraya_Wall Init (`z_obj_takaraya_wall.c:294-297`) | `ACTOR_EN_BOX` 0x006, `ACTORCAT_CHEST` | (-1635, 0, 60 + 120 c), c = random column 0..7 | 0 / 0x4000 / 5 | same as the wall | 10 | wall Init |

GetItem ids 0x05 `GI_RUPEE_PURPLE`, 0x0C `GI_HEART_PIECE`, 0x04 `GI_RUPEE_RED`, 0x2A `GI_DEKU_NUTS_10`, 0x29 `GI_DEKU_NUTS_5` (`include/z64item.h:419-457`). Item values: red 20, purple 50 rupees (`sRupeeRefillCounts`, `z_parameter.c:2543`, :2857; table rows `ovl_player_actor/z_player.c:2084`, :2086); nuts 5 / 10 (`z_parameter.c:2540`, :2744-2749; rows z_player.c:2184, :2186); Heart Piece row :2106.

### 4.4 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init / draw | Presence and state at Day 1 noon |
| --- | --- | --- |
| En_Takaraya (a02) | `ACTORCAT_NPC`; flags attention, friendly, update and draw culling disabled; object `OBJECT_BG`; `gTreasureChestShopGalSkel` (Flex, object_bg 0x8FC8) with idle loop `object_bg_Anim_009890` ("bg_newwait"); shadow yOffset -60; attention range 6; params `& 0xFF` = 0 (treasure flag bit 0), switch flag = `(params >> 8) & 0xFF` = 0 (`z_en_takaraya.c:10-12, :34-44, :99-138`; `z_en_takaraya.h:11-12`). No collider and no dynapoly (grep). Eyes: Deku and Human use the eyes-down textures, the other forms eyes-up; 4-step blink, 2 percent chance per frame (:46-58, :150-159, :427-431). Head tracks the player (:393-419) | present (mask 0x3F8); state machine in 5.3 |
| Obj_Takaraya_Wall | `ACTORCAT_BG`, draw/update culling disabled, object `OBJECT_TAKARAYA_OBJECTS`, scale 0.1; two OC cylinders (init radius 40, height 120); 11 x 8 board (`z_obj_takaraya_wall.c:30-96`) | not present until payment; 5.4 |
| En_Box (chest) | type `(p >> 12) & 0xF` = 11 `ENBOX_TYPE_BIG_SWITCH_FLAG`: big chest, appears when its switch flag is set; item `(p >> 5) & 0x7F`; chest flag `p & 0x1F` = 0; switch flag = `world.rot.z` = 5 (`z_en_box.h:35-42`); FLAGS 0, so the wall sets `cullingVolumeDistance` 2000 (`z_obj_takaraya_wall.c:302`); big because `Actor_IsSmallChest` is false for type 11 (`z_actor.c:5053-5060`) | not present until payment; 5.5 |
| En_Test4 (a00) | three-day controller; csId none -> `CLEAR_EVENTINF(EVENTINF_HAS_DAYTIME_TRANSITION_CS)`, so the 18:00 and 06:00 swaps run without a cutscene (`z_en_test4.c:322-337`, :54, :115, :446-452); stars `(p >> 10) * 100` = 0; bell volume index `(p >> 5) & 0xF` = 8 (`z_en_test4.h:8-9`) | always |
| Obj_Tokeidai (a01) | type `(p & 0xF000) >> 12` = 9 `OBJ_TOKEIDAI_TYPE_WALL_CLOCK`; scale 0.02, `ObjTokeidai_Clock_Draw`, `ObjTokeidai_WallClock_Idle` follows `CURRENT_TIME` every frame (`z_obj_tokeidai.c:247-252, :104-115, :719-723, :811-848`; `z_obj_tokeidai.h:6-19`). DLs object_obj_tokeidai 0xCF28 minute ring, 0xBEE8 centre/hand, 0xF518 wall-clock face, 0xC368 sun/moon panel (XML `object_obj_tokeidai.xml:47-67`) | always; time-posed |
| En_Time_Tag (a03) | type `(p & 0xE000) >> 13` = 0 `TIMETAG_KICKOUT_DOOR`; text `(p & 0x1FE0) >> 5` = 15 `TIMETAG_KICKOUT_TEXT_TREASURE_CHEST_SHOP`; exit `p & 0x1F` = 0; 22:00 (`z_en_time_tag.h:10-15, :58`); draw NULL | present Day 0 to Day 2 only (D0d..D2n); section 5.8 |
| Obj_Sound (a04) | type `(p >> 8) & 0xFF` = 1 `OBJ_SOUND_TYPE_BGM`, sfxType 0, id `p & 0x7F` = 0x46 = ordinal 70 = `NA_BGM_SHOOTING_GALLERY` (`z_obj_sound.h:8-17`; `z_obj_sound.c:28-70`; `sequence_table.h:160`); from its second update plays positional BGM at the actor; destroy resets BGM | always |
| En_Door (ta0) | schedule door; no dynapoly for schedule doors, no collider | section 2.4 |

### 4.5 Counts

| | Default header |
| --- | --- |
| Room entries / runtime children / transition actors | 5 / 2 (wall, chest) / 1 |
| Day 1 daytime candidates by mask | 5 of 5 |
| static-mesh | 1 (a01), plus runtime wall and chest |
| npc-scheduled | 1 (a02) |
| controller-invisible | 3 (a00, a03, a04) |
| door-transition | 1 (ta0) |
| billboard-effect / posed-skeletal | 0 / 0 actors (no billboard DL in the room) |
| Actor cutscene entries used | 2 of 11 (csId 10 by a02 and the chest, csId 9 by the chest) |

## 5. Venue mechanics index (INT-11 scope)

Scope (FULL_CITY_PLAN section 6): "Source-backed activity rules/conditions, prize eligibility and save/reset handling"; the dispatch adds fee, timer, per-form layout and prizes, the keeper, the chests and the second spawn. Index and cite only; no Unity design.

**Correction to the brief's wording:** the maze layout is **not** per form. One generator runs on every payment (5.4); the form selects only the fee, the prize pair, the texts, the eyes and the chest contents (5.2).

### 5.1 Who does what

| Part | Role | Source |
| --- | --- | --- |
| a02 En_Takaraya | keeper: talk, fee, state machine, timer start, win and time-up detection, return transition | `ovl_En_Takaraya/z_en_takaraya.c` (437 lines) |
| Obj_Takaraya_Wall | board manager: maze generation, wall rise and fall, chest spawn | `ovl_Obj_Takaraya_Wall/z_obj_takaraya_wall.c` (502 lines) |
| En_Box | the prize chest (big, appears on switch flag 5), open animation and get-item | `ovl_En_Box/z_en_box.c` |
| a03 En_Time_Tag | 22:00 kickout, deferred while a game runs | `ovl_En_Time_Tag/z_en_time_tag.c:285-339` |
| ta0 / East TA#2 En_Door | door schedule | `ovl_En_Door/scheduleScripts.schl:174-181` |
| HUD timer | `TIMER_ID_MINIGAME_2`, 45 s | `z_parameter.c:836-851`, :5740-5960 |
| Face reactions | mask texts for the gal | `src/code/z_face_reaction.c:77-80, :181-186` |

### 5.2 Per-form fee, prize and texts

Arrays are indexed by `GET_PLAYER_FORM` (0 Fierce Deity, 1 Goron, 2 Zora, 3 Deku, 4 Human; `z64player.h:70-75`): intro `D_80ADFB2C`, prize pair `D_80ADFB38`, decline `D_80ADFB44`, choice `D_80ADFB50` (`z_en_takaraya.c:67-97`). The fee is `msgCtx.firstChoicePrice`, copied from the choice message's header when it is shown (`z_message.c:2073-2074`) and compared with `playerData.rupees` at the choice (`z_en_takaraya.c:237`).

| Form | Intro text | Choice text (fee, MSG) | Decline text | First-win prize | Repeat prize | Form switch flag | Survives the cycle reset |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Fierce Deity | 0x76D | 0x771 (**20**) | 0x775 | purple rupee (50) | purple rupee | 0 | no |
| Goron | 0x76E | 0x772 (**30**) | 0x776 | **Heart Piece** | purple rupee (50) | 1 | **yes** (mask bit 1) |
| Zora | 0x76F | 0x773 (**5**) | 0x777 | red rupee (20) | red rupee | 2 | no |
| Deku | 0x76C | 0x770 (**10**) | 0x774 | Deku Nuts 10 | Deku Nuts 5 | 3 | no |
| Human | 0x76D | 0x771 (**20**) | 0x775 | purple rupee (50) | purple rupee | 4 | no |

MSG skeleton: each intro (0x76C-0x76F) ends `CONTINUE` into its choice message through `nextTextId`; each choice message is `TWO_CHOICE` with the fee in `firstChoicePrice` (0x0A, 0x14, 0x1E, 0x05) and `textBoxProperties` 0x0601; decline texts have no chain. Other ids: 0x778 (rules, one `BOX_BREAK`, `CONTINUE` to 0x779), 0x77A (win, ends `EVENT`), 0x77B (not enough rupees), 0x77C (post-game, ends `EVENT`), 0x77D (time up). "First win" means the form's switch flag is not yet set; the flag is set only on a win (5.3), so a lost game never advances the prize.

### 5.3 En_Takaraya state machine (OVL `z_en_takaraya.c`)

| State (function, lines) | Trigger | Effect |
| --- | --- | --- |
| Init (99-138) | spawn | if `save.entrance == ENTRANCE(TREASURE_CHEST_SHOP, 1)` the return branch (5.9), else `EnTakaraya_SetupWait` |
| Wait `EnTakaraya_Wait` (168-189) | idle | loop anim; facing the player within `0x2000`: offered text = mask reaction (5.10) or the form intro; `formSwitchFlag = form + switchFlag` every frame; `Actor_OfferTalk(120)`. On accept: if no mask reaction, play `object_bg_Anim_00A280` ("bg_tokune"); -> Talk |
| Talk `EnTakaraya_Talk` (208-261) | message states | on the choice (`TEXT_STATE_CHOICE`): choice 0 with rupees < fee -> text 0x77B, `NA_SE_SY_ERROR`; choice 0 otherwise -> decide sfx, `Rupees_ChangeBy(-fee)`, `EnTakaraya_SpawnWalls`, text 0x778; any other choice -> cancel sfx, decline text. On `TEXT_STATE_CLOSING`/`DONE`: textId 0x778 -> start (below); else clear both kickout flags and back to Wait |
| SpawnWalls (191-202) | payment | prize = `D_80ADFB38[form][Flags_GetSwitch(formSwitchFlag) ? 1 : 0]`; spawns Obj_Takaraya_Wall (4.3) |
| `func_80ADF2D4` (263-270) | 0x778 closed | `NA_SE_SY_FOUND`, sub-BGM `NA_BGM_TIMED_MINI_GAME`, `timer = 145`, **set `WEEKEVENTREG_KICKOUT_WAIT`**, clear `KICKOUT_TIME_PASSED` |
| `func_80ADF338` (272-299) | chest cutscene | while csId 10 is current and a chest exists: each frame `timer--` and the cutscene sub-camera sits on the chest-to-player line 250 units from the chest, 120 above the player, looking toward the chest, and over the last 25 frames the eye slides out to the player's distance; when the cutscene ends and `timer < 145` -> next |
| `func_80ADF4E0` (301-305) | | `Interface_StartTimer(TIMER_ID_MINIGAME_2, 45)`; `timer = 1` |
| `func_80ADF520` (307-323) | every frame outside cutscenes | **win** if `Flags_GetTreasure(params = 0)`: `Flags_SetSwitch(formSwitchFlag)`, `sceneFlags.chest &= ~(1 << 0)`, `timer = 0`, `timerStates = TIMER_STATE_6`; **time up** if `timerCurTimes == 0`: `timer = 50`, textbox 0x77D, `timerStates = TIMER_STATE_STOP`. Both -> `func_80ADF608` |
| `func_80ADF608` / `func_80ADF654` (325-346) | | player halted (`PLAYER_CSACTION_WAIT`), `unk2AD = true`; after `timer` frames: win -> `TRANS_TYPE_80` and `FADE_WHITE`, time up -> `TRANS_TYPE_64` and `FADE_BLACK`; `nextCutsceneIndex = 0`; `nextEntrance = ENTRANCE(TREASURE_CHEST_SHOP, 1)` |
| Destroy (140-148) | scene unload | always `Flags_UnsetSwitch(play, 5)`; if `unk2AD` is false (the scene unloads mid-game, for example through the door): clear `KICKOUT_WAIT`, `timerStates = TIMER_STATE_STOP` |

### 5.4 The board (OVL `z_obj_takaraya_wall.c`)

| Item | Value | Source |
| --- | --- | --- |
| Grid | 11 rows x 8 columns, 120-unit cells (88 cells); row index follows +X, column index +Z; cell centre `(i * 120 - 1620, j * 120 + 60)`; Unity cell 1.20; board x -1680..-360, z 0..960 = exactly the maze-floor polys 71, 72 | :75-76, :100-108, :466-472; section 7 |
| Back / front | row 0 (x -1620) is the back wall side, where the chest column is chosen; row 10 (x -420) is the front, bordering the shop floor at x -360. The board diagram comment lists columns 7..0 left to right facing the back | :7-25 |
| Cell state | `sTakarayaWallHeights`: 0.0 closed (a wall that may rise), -10.0 open path (never draws, never rises) | :96-97, :113-120 |
| Generation | all cells closed; `column = rand 0..7`; chest at (-1635, 0, 120 column + 60); cell [0][column] opened; randomized depth-first carve: from the stack top choose uniformly among the legal directions (back = row - 1, right = column - 1, front = row + 1, left = column + 1); a direction is legal when the adjacent cell is in range and not open and the two cells beside it and the cell two steps ahead are not open; when none is legal the top is popped; the loop ends when the stack depth falls to 1 (the root is never revisited). Then if no column 1..6 of row 10 is open, row 10 column 1 (if column 0 is open) or column 6 is opened ("the scene blocks direct access to the front corners") | :133-334 |
| SIM (20000 boards, uniform RNG) | open cells min 21, mean 56.3, max 61 of 88; every board reaches row 10 columns 1..6 from the chest; row 10 always has at least 2 open cells; the corner fix-up fired in 3 of 20000 boards | scratch `sim.py` |
| Rise rule | per frame, with the player's cell and the offset from its centre `((x + 1800) % 120) - 60`, `((z + 120) % 120) - 60`: offset > 20 on an axis selects the neighbour cell on that side (+X side row + 1, -X side row - 1; likewise column), < -20 the other side; a closed cell in the player's column (row neighbours) or row (column neighbours) rises 15 units per frame to 120; every other closed cell except the player's own falls with `Math_SmoothStepToF(0.2, 5)` toward 0 | :351-409 |
| Colliders | two OC cylinders (init radius 40, height 120), radius forced to 200 each update, moved onto the risen wall of the active axis; parked 240 below the actor when inactive; `OC1_ON \| OC1_TYPE_ALL`, mass immovable | :51-69, :411-437, :290-292 |
| Draw | cells with height > 0 only: matrix x `i * 120 - 1620`, y `height + (actor.y - 120)`, z `j * 120 + 60`; `(i + j) % 2 != 0` -> `gTreasureChestShopWallWhiteDL` (object 0xD60) else `...BlackDL` (0xB70); rising and falling cells play `NA_SE_EV_ROCK_CUBE_RISING` / `_FALL` at their projected position | :450-502 |

Route visibility consequence (C): the layout is invisible until approached; only closed cells next to the player appear, and open cells are never drawn. The state is rebuilt on every payment (Init resets all heights, :284-288).

### 5.5 The chest (OVL `z_en_box.c`)

| Step | Behavior | Source |
| --- | --- | --- |
| Init | not yet opened and switch flag 5 not yet set -> `EnBox_AppearSwitchFlag`: collision disabled, y = home - 50, alpha 0; yaw 0x4000 (90 deg, facing +X toward the front) then `shape.rot.y += 0x8000` | :176-254, :93 |
| Appear | the wall sets switch flag 5 right after the spawn (`z_obj_takaraya_wall.c:298`); the chest requests csId1 = 10 (`CutsceneManager_StartWithPlayerCs`), rises 1.25 per frame for 40 frames, fades in over 20, then `EnBox_WaitOpen` and `CutsceneManager_Stop` | :377-387, :404-438 |
| csIds | the loop over `additionalCsId` takes the entry with custom value 1 as csId2 (entry 9, 135 frames, the open cutscene) and the other as csId1 (entry 10, 150 frames, priority 310) | :107-118; HDR scene.c:78-79 |
| Open | offered when the player stands within z -50..0, x +-20 of the chest in its local frame and faces it; opening plays the per-form big-chest animation (Human child, Goron, Deku, Fierce Deity and Zora adult), sets the chest flag bit 0 (`Flags_SetTreasure`) and, for a Heart Piece, collectible flag 0; the item is `-getItemId` through `Actor_OfferGetItemNearby`; a Heart Piece already marked collected is swapped for a Recovery Heart | :441-515 |

### 5.6 Timer

| Item | Value | Source |
| --- | --- | --- |
| Start | `Interface_StartTimer(TIMER_ID_MINIGAME_2, 45)`: 4500 centiseconds counting down (`SECONDS_TO_TIMER` = x 100), state `TIMER_STATE_START` | `z_en_takaraya.c:302`; `z_parameter.c:836-851`; `z64save.h:79, :85-111` |
| HUD preface | with no minigame state set, START -> HOLD (20 frames) -> MOVING (20 frames) -> COUNTING; the countdown clock `timerStartOsTimes` starts at COUNTING, so about 40 frames of HUD motion precede the 45 s | `z_parameter.c:5744-5812` |
| Clock | real time (`osGetTime`), not the game clock; paused time is subtracted | `z_parameter.c:5931-5953` |
| Expiry | `timerCurTimes` = 0 and state `TIMER_STATE_STOP`; En_Takaraya tests `timerCurTimes == TIMER_STATE_OFF` (0) | `z_parameter.c:5956-5958`; `z_en_takaraya.c:316` |
| Win | `TIMER_STATE_6` freezes the count (`timerStopTimes`) and moves to `TIMER_STATE_7`; its extra checks concern the Ranch and the Deku Playground, not this scene | `z_parameter.c:5886-5915` |
| Place | timer id 4 is shared with other minigames (En_Az, En_Fu, ...); not persisted by the shop | `z64save.h:92` |

### 5.7 State, eligibility and reset

| State | Location | Written | Read | Cycle reset |
| --- | --- | --- | --- | --- |
| Form-won switch flags 0..4 | `sceneFlags.switches[0]` -> `cycleSceneFlags[SCENE_TAKARAYA].switch0` | `Flags_SetSwitch(formSwitchFlag)` on a win (`z_en_takaraya.c:311`) | prize pick at payment (:194) | `sPersistentCycleSceneFlags[SCENE_TAKARAYA].switch0 = 2` keeps **only bit 1 (Goron)**: the Heart Piece is a once-per-file prize, every other form's pair restarts each cycle (`z_sram_NES.c:450-466`; `scene_table.h:53`) |
| Chest opened bit 0 | `sceneFlags.chest` | `Flags_SetTreasure` on opening (`z_en_box.c:491`) | En_Box Init (:212), win test (`z_en_takaraya.c:310`) | cleared by En_Takaraya on a win (:312) so the next chest is closed; mask 0, cleared at cycle reset |
| Collectible bit 0 | `sceneFlags.collectible[0]` | `Flags_SetCollectible` when a Heart Piece chest opens (`z_en_box.c:489`) | swaps a repeat Heart Piece for a Recovery Heart (:500-503) | mask 0, cleared each cycle; the Goron switch flag normally stops a second Heart Piece first |
| Switch flag 5 | `switches[0]` bit 5 | wall Init (`z_obj_takaraya_wall.c:298`) | En_Box appear | unset in En_Takaraya Destroy (:143); not persistent |
| `WEEKEVENTREG_KICKOUT_WAIT` (63, 0x01), `_TIME_PASSED` (63, 0x02) | `weekEventReg[63]` | En_Takaraya, En_Time_Tag | both | cleared at cycle reset (byte 63 keeps only 0x20, `z_sram_NES.c:164`; `z64save.h:1227-1234`) |
| Rupees | `playerData.rupees` | `Rupees_ChangeBy(-fee)` at payment (`z_en_takaraya.c:245`); prize by get-item | `< fee` test (:237) | 0 at cycle end (`z_sram_NES.c:645`) |

Scene flags are copied to `gSaveContext.cycleSceneFlags[SCENE_TAKARAYA]` after all actors are destroyed (`Actor_CleanupContext`, `z_actor.c:3363`; `Play_SaveCycleSceneFlags`, `z_play.c:1870-1884`) and loaded at `Actor_InitContext` (`z_actor.c:2507, :2521-2527`). No payment is refunded on any path. Nothing in the overlay stops the player from walking back to the door during a game: the exit then unloads the scene, and Destroy only stops the timer and clears the kickout wait (`z_en_takaraya.c:140-148`; INF, the route was not run). Eligibility rules in the overlay: any form pays its own fee; no flag, item or mask is required; no daily limit exists (the flags only choose the prize); a worn mask from the 19-entry list ends the talk with a reaction text instead (5.10); the Giant, Fierce Deity, Goron, Zora and Deku masks are outside the table (`z_face_reaction.c:181-186`), so transformation masks do not block.

### 5.8 Door schedule and kickout

| Item | Value | Source |
| --- | --- | --- |
| Door (street TA#2 and interior ta0) | `sDoorSch_TreasureChestShop`: `if_since_time (22, 0)` -> `return_s (0x10)` else `return_none`; text `0x1800 + 0x10` = `0x1810` (MSG: type-6 box, no choice) | `scheduleScripts.schl:174-181`; `z_en_door.c:536-546`; `z_schedule.c:188-203` |
| Schedule clock | script time = `CURRENT_TIME - 0x3FFC` truncated to `u16`, so the schedule day starts at 06:00; `if_since_time` is the negation of `now < t` | `include/z64scripts.h:7-17`; `tools/buildtools/schc.py:167-168, :473-474` |
| Result | **open 06:00-21:59, closed 22:00-05:59, every day**; no day, flag or mask term. Resolves REF-02 "open hours UNKNOWN" and East U-11 | C |
| Kickout | a03 at hour 22 minute 0 (equality test, `z_en_time_tag.c:333`), when not in a cutscene: player halted, text 0x1892 (`0x1883 + 15`, MSG: ends `EVENT2`), then `nextEntrance = setupExitList[0] = 0xD240` and the door sfx | `z_en_time_tag.c:285-294, :315-339` |
| Presence | a03 mask 0x3F0 = D0d..D2n: no kickout on Day 3 (the door still closes at 22:00 on Day 3) | HDR room.c:41-43 |
| Deferral while a game runs | KICKOUT_WAIT set (5.3): at 22:00 the tag sets `KICKOUT_TIME_PASSED` and waits in `WaitForTrigger` until both flags are clear, then halts the player and shows 0x1892 | `z_en_time_tag.c:300-306, :327-332` |
| Flag clears by En_Takaraya | Talk closing (:227-228), the win text 0x77A when `TIME_PASSED` is set (:375-379), Init of a lost game when `TIME_PASSED` is set (:127-130), Destroy mid-game (:143-147) | OVL |
| Other users of the same pair | En_Aob_01, En_Fu, En_Mm3, En_Syateki_Man (grep) | cross-reference only |
| Quirk (INF, U-02) | after a lost game that ended after 22:00 Init clears both flags before En_Time_Tag's first update (actor order a02 then a03), and the tag fires only inside the 22:00 minute, so no kickout follows | `z_en_takaraya.c:127-130`; `z_en_time_tag.c:333` |

### 5.9 Game-end return (entrance 1)

| Case | Init action (`z_en_takaraya.c:119-137`) | Then |
| --- | --- | --- |
| Win (`timerStates == TIMER_STATE_6`) | `Audio_StopSubBgm`; textId 0x77A; `timerStates = TIMER_STATE_STOP`; `func_80ADF6DC` | gal plays `object_bg_Anim_001384` loop with `ACTOR_FLAG_TALK_OFFER_AUTO_ACCEPTED`; auto-talk within 120; on the 0x77A event advance: if `TIME_PASSED` close the box and clear the flags (the tag then kicks out), else text 0x77C then the choice text `D_80ADFB50[form]` and back to Talk (:348-391) |
| Time up | textId 0x77C; if `KICKOUT_TIME_PASSED`: clear both flags and `SetupWait`; else `func_80ADF6DC` | auto-talk 0x77C, then the choice text (play again) |

The player arrives at spawn 1 (mode D walk-in). The wall and chest are gone (scene reloaded); a new payment builds a new board.

### 5.10 Mask reactions

`Text_GetFaceReaction(play, FACE_REACTION_SET_TREASURE_CHEST_SHOP_GAL = 0x2D)` returns a non-zero text id for every mask from Truth to Scents (`z_face_reaction.c:77-80, :181-186`; `z64message.h:161`). When non-zero it replaces the form intro and, since the id is not 0x778, the talk ends without a game (`z_en_takaraya.c:176-188, :223-230`). MSG: all 13 distinct reaction texts 0x233B-0x2347 have `nextTextId` 0xFFFF and no choice. Column order Truth, Kafei, All-Night, Bunny, Keaton, Garo, Romani, Circus Leader, Postman, Couple, Great Fairy, Gibdo, Don Gero, Kamaro, Captain, Stone, Bremen, Blast, Scents maps to `233B, 233C, 233D, 233E, 233F, 233B, 2340, 2341, 2342, 2343, 2344, 2345, 2346, 233B, 2345, 2347, 233D, 233B, 233D`. The Giant, Fierce Deity, Goron, Zora and Deku masks are not in the table.

### 5.11 Cameras, cutscenes, lighting and sound

| Item | Value | Source |
| --- | --- | --- |
| Cutscene entries (priority, length, csCamId, additional) | 0: 700, -1, -3, +1; 1: 600, -1, -2, +2; 2: 700, -1, -4, +3; 3: 700, -1, -5, +4; 4: 500, -1, -7, +5; 5: 400, -1, -11, +6; 6: 100, -1, -8, +7; 7: 200, -1, -9, +8; 8: 800, -1, -6, none (a chain 0-8, unreferenced by any room actor, U-07); **9: 300, 135, -10, none, custom value 1 (chest-open cutscene)**; **10: 310, 150, -1, +9 (chest-appear cutscene, En_Takaraya and the chest)** | HDR scene.c:68-80; `include/z64cutscene.h:626-637` |
| Actor cs camera | 1 entry `{37, 3, points}` = `CAM_SET_CIRCLE3` at (-108, 57, 683); no cutscene entry has `csCamId >= 0`, so it is unreferenced (U-07) | HDR scene.c:90-98; `include/z64camera.h:184` |
| BgCam list | 0 `0x0000` `CAM_SET_NONE`; 1 `0x003F` `CAM_SET_ROOM0` (shop floor); 2 `0x0039` `CAM_SET_TREASURE_CHEST_MINIGAME` (maze floor, "TRESURE1"); all with count 0 and no data (U-06) | HDR scene.c:112-116; `z64camera.h:147, :204, :210` |
| Door camera | both sides bgCam -1 (generic door camera) | 2.4 |
| Lighting | `LIGHT_MODE_SETTINGS`: the floor under the player selects the light setting; floor surfaces use index 0 (shop floor, threshold, pad, counter top) and 1 (maze floor, counter end); a wall index of 31 would be remapped to 0; indices 2 and 3 are unreferenced | scene.c:119-121; `src/code/z_kankyo.c:2186-2194`; `z_player.c:11729-11731` |
| Music | scene seq 0; a04 plays `NA_BGM_SHOOTING_GALLERY` positionally from (47, 260, 462); during a game `NA_BGM_TIMED_MINI_GAME` (`sequence_table.h:94`) is the sub-BGM, stopped on the return | 4.4; `z_en_takaraya.c:265, :120` |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header)

Venue **open** at 12:00 (5.8). Assumptions: fresh save (no form switch flag set), Human form, no mask. World = actorPos + actorRot x (actorScale x localVertex) except where a custom draw overrides it.

| Key | Component / params | N64 position (Unity) | Yaw | Scale | DLs, bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room_00 | room shell: shop hall, gal counter, board hall, vault roof | room origin | | 1 | opaque wrapper `Z2_TAKARAYA_room_00DL_0077B0` (calls 15 entry DLs) and XLU wrapper `0x7E68` (calls `0x7D20`) | 21 `Gfx`, 16 cullable entries (section 8) |
| a01 | wall clock 0x907F | (-300, 160, 831) (-3.00, 1.60, -8.31) | 180 | 0.02 | object_obj_tokeidai 0xCF28 ring, 0xBEE8 centre/hand, 0xF518 face, 0xC368 sun/moon panel | noon pose per `docs/ROM_REFERENCE.md:55-56`; panel offset (0, -1112, -19.6) x 0.02 (`z_obj_tokeidai.c:841`) |
| a02 (optional) | gal 0x0000 | (-85, 0, 849) (-0.85, 0.00, -8.49) | 180 | 0.01 | object_bg 0x8FC8 `gTreasureChestShopGalSkel`, idle `object_bg_Anim_009890` | present at D1d; skeletal pass deferred; floor under her is poly 63 at Y 0 |
| ta0 | door 0x0288 | (-70, 0, 100) (-0.70, 0.00, -1.00) | 180 | default | gameplay_keep `gDoorSkel` 0x22B28, leaf 0x20BB8 / 0x20D00 | closed pose; leaf side by camera yaw |

**Excluded by half-day mask:** none at D1d (all five entries are present). **No geometry:** a00 En_Test4, a03 En_Time_Tag, a04 Obj_Sound. **Not present until payment:** Obj_Takaraya_Wall and the En_Box chest (4.3); nothing of the board is drawn at idle (5.4).

Profile notes: `prof.d1.night.normal` (21:00, bit 0x040) keeps all five entries (0x3F8 and 0x3F0 contain 0x040); `prof.d1.late.normal` (23:00) shares that bit but the door is closed, so the interior is reachable only by direct entrance; `prof.d3.noon.normal` (bit 0x008) has a02 but not a03; `prof.d3.night.normal` has neither. Extraction commands are in section 9 (not executed).

## 7. Collision summary (C from scene.c:112-306; scratch `analyze.py`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 72 / 97 / 9 |
| Water boxes | 0 (`0, NULL`, scene.c:306) |
| Header bounds | min (-1680, 0, 0), max (80, 572, 964); Unity x -16.80..0.80, y 0..5.72, z -9.64..0 |
| Vertex extents | x -1680..80, y 0..484, z 0..960 |
| Polygon kinds (normal y > 0.5 floor, < -0.8 ceiling, `z_bgcheck.c:526-534`) | 17 floors (all flat), 72 walls, 8 ceilings |
| Materials (`data[1] & 0xF`) | floors all stone (types 4-8); wall and ceiling types 0-3 are dirt |
| Exit surfaces | 1 type, 2 floor polys (2.3) |
| Other flags | echo 2 on every surface; no floor type, floor property, hookshot, conveyor, soft or horse bits; wall type 1 (`WALL_FLAG_0`, `z_bgcheck.c:20`, meaning NI) on surface 3; poly exclusion `COLPOLY_IGNORE_CAMERA`: 12 polys (0-5, 20, 21, 67-70); `IGNORE_CAMERA \| IGNORE_PROJECTILES` plus vIB flag 4: 6 polys (73-78) |
| Camera data | 3 BgCamInfo (5.11): surface 4 -> bgCam 1, surface 7 -> bgCam 2, all others bgCam 0 |

Surface types (scene.c:119-121):

| Type | data[0] / data[1] | bgCam | Light | Polys | Kinds | Reading |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | 0x00000000 / 0x00001000 | 0 | 0 | 38 | 36 wall, 2 ceiling | shop and vestibule walls |
| 1 | 0x00000000 / 0x00001040 | 0 | 1 | 34 | 28 wall, 6 ceiling | board-hall perimeter, upper bands, flat roof |
| 2 | 0x00000000 / 0x000017C0 | 0 | 31 | 4 | wall | back wall z = 960 |
| 3 | 0x00200000 / 0x00001040 | 0 | 1 | 4 | 4 sloped (normal y -0.73) | roof slopes, wall type 1 |
| 4 | 0x00000001 / 0x00001002 | 1 | 0 | 5 | floor | shop floor |
| 5 | 0x00000000 / 0x00001002 | 0 | 0 | 6 | floor | threshold (67, 68) and counter top at Y 40 (79, 80, 83, 84) |
| 6 | 0x00000100 / 0x00001002 | 0 | 0 | 2 | floor, exit 0 | exit pad (69, 70) |
| 7 | 0x00000002 / 0x00001042 | 2 | 1 | 2 | floor | board floor (71, 72) |
| 8 | 0x00000000 / 0x00001042 | 0 | 1 | 2 | floor | counter end at Y 40 (81, 82) |

Floor levels and walkable regions:

| Y | Polys | Surface | Extent x / z | Reading (INF) |
| --- | --- | --- | --- | --- |
| 0 | 62-66 | 4 | -360..80 / 120..960 | shop floor; spawns 0 and 1 and the gal stand here (spawn 0 on poly 64, spawn 1 and the gal on poly 63) |
| 0 | 67, 68 | 5 | -100..-40 / 100..120 | threshold under the door |
| 0 | 69, 70 | 6 | -100..-40 / 20..100 | exit pad |
| 0 | 71, 72 | 7 | -1680..-360 / 0..960 | board floor, 11 x 8 cells; the chest row (x -1635) and all cell centres land on it |
| 40 | 79, 80, 83, 84 | 5 | -175..80 / 811..870 | counter top |
| 40 | 81, 82 | 8 | -240..-175 / 811..870 | counter end |

Fixture readings (INF from polygon extents; poly numbers are scene.c indices): vestibule x -100..-40, z 20..120 with ceiling Y 100 (0, 1), side walls (2-5), lintel Y 100..120 at z = 120 (6, 7), outer end wall z = 20 (20, 21), and the doorway closed by wall 22, 23. The shop front wall at z = 120 spans x -360..80 (12-15 and 44, 45 up to Y 120, then 38, 39 up to Y 358). East wall x = 80 over z 120..960 (8, 9, 24-27, 30, 31). The board hall opens onto the shop floor through a 720-unit gap (z 120..840) between two corner blocks at x about -360..-240: z 0..120 (42-45) and z 840..960 (46-51); the wall clock a01 hangs on the z = 840 face of the second block (46, 47). Hall walls: z = 0 (10, 11, 36, 37), x = -1680 (16, 17, 28, 29, 32-35), z = 960 (18, 19, 40, 41). Roof: slopes (52, 53, 60, 61) from Y 240 at z = 0 and 960 to Y 420 at z = 192 and 768, ceiling bands to a flat Y 484 for z 384..576 (54-59), spanning x -1680..80. Gal booth: a solid block z 811..841 below the counter top at Y 40 (85-96 lower walls), an upper wall at z = 811 from Y 40 to 154 with bay-angled sides (73-78, camera- and projectile-ignoring), and the gal in the pocket behind it at z = 849; spawn 1 is 67 units in front of the block.

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 17: 16 textures + 1 TLUT (`Z2_TAKARAYATLUT_009860`) |
| By format | rgba16 10 (9 textures + the TLUT), i8 3, i4 2, rgba32 1, ci4 1 |
| Texture names | `000A60` i4, `001260` i8, `001660` rgba16, `001E60` rgba16, `002060` rgba32, `003060` rgba16, `003860` rgba16, `004860` rgba16, `005860` rgba16, `005A60` rgba16, `005E60` i8, `006E60` i8, `007660` ci4 (uses the TLUT, `G_TT_RGBA16`), `007E60` rgba16, `008660` rgba16, `009660` i4 |
| Location / use | all in the scene file (segment 2); all 17 referenced by room DLs |
| Room `Gfx` DLs / `Vtx` arrays / vertices | 21 / 18 / 1537 |
| Raw triangles | 432 `gsSP2Triangles` + 6 `gsSP1Triangle` = 870 (755 in opaque entries, 40 in the XLU entry, 75 in the three sub-DLs of `0x2098`) |
| Render modes | 16 DLs `G_RM_AA_ZB_OPA_SURF2` with fog; `0x76F0` (10 tris) `G_RM_AA_ZB_XLU_DECAL2` drawn from an opaque slot; `0x7D20` (40 tris) `G_RM_AA_ZB_XLU_SURF2` / `G_RM_PASS`; three wrappers (`0x2098`, `0x77B0`, `0x7E68`) with no triangles |
| Combiner / prim-colour / geometry-mode loads | 6 distinct combiners, 6 prim colours, 18 geometry-mode loads (all `G_CULL_BACK`, 17 with `G_FOG`, 1 with `G_LIGHTING`) |
| Billboards | none (no segment-1 matrix) |
| Animated materials | none (section 1) |
| Runtime banks (XML names only) | object_takaraya_objects: wall white TLUT (rgba16 4x4), black texture (rgba16 32x32), white texture (ci4 32x32), 2 DLs; object_bg gal block: 12 ci8 textures (hair, hair 2, five eye states, mouth, ear and belly button, skin, clothes, fingers) + 1 TLUT, 16 limb DLs; object_box chest: 6 textures (lock and corner in three finishes), 6 DLs |

Cullable entries (HDR room.c:60-75; triangle counts C from scratch `dls.py`; readings INF from centres):

| # | Centre / radius | Opaque DL (tris) | XLU DL (tris) | Reading |
| --- | --- | --- | --- | --- |
| 0, 1 | (-80, 18 and 38, 840) / 164 | 0x22B8 (20), 0x2760 (36) | | gal booth |
| 2, 3, 4, 14 | (12, 54, 928), (9, 54, 940), (-170, 54, 932), (-31, 120, 949) / 94, 78, 88, 16 | 0x2B10 (16), 0x3DB0 (136), 0x4FE8 (63), 0x44E8 (66) | | back wall of the booth area |
| 5 | (-800, 0, 480) / 1004 | 0x2098 (wrapper: 0x1910 47, 0x1C48 8, 0x1F80 20) | | board floor |
| 6, 12 | (-1020 and -1000, 286, 480) / 866, 882 | 0x08A0 (62), 0x5AE8 (80) | | board-hall walls |
| 7, 8, 9, 15 | (-80, 120, 490), (-300, 120, 480), (-80, 60, 540), (-120, 240, 540) / 512, 500, 454, 466 | 0x0E90 (34), 0x2DD8 (10), 0x3110 (20), 0x1280 (42) | | shop hall |
| 10 / 11 | (-751, 399 / 361, 570) / 776, 768 | 0x7180 (160) | 0x7D20 (40) | roof and a translucent layer |
| 13 | (-80, 0, 540) / **-450** | 0x76F0 (10) | | floor decal; a negative radius sets `boundsNearZ = FLT_MAX` so it sorts last (`z_room.c:168, :182-183`) |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_TAKARAYA` C; `object_obj_tokeidai`. Binaries present under `extracted/n64-us/baserom/` (checked by `ls`): object_box 33056 B, object_bg 72560, object_takaraya_objects 3664, gameplay_dangeon_keep 144000, gameplay_keep 599520.

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `gameplay_keep` | door skeleton and leaves (shared with North's list) |
| Later skeletal pass | `object_bg` | the gal (the XML also holds other `object_bg` skeletons) |
| Minigame pass | `object_takaraya_objects`, `object_box` | wall cells, chest |
| Optional | `gameplay_dangeon_keep` | only to resolve U-05 |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_bg','object_takaraya_objects','object_box')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

Message text for the ids in this dossier is already decoded locally by `tools/reference/extract_messages.py` (ignored output); nothing from it is copied here.

## 10. Proposed region record

Schema 1 as frozen at `7a7a08c` (PIPE-01). `sourceFrame.sharedFrame` uses the frozen field because an interior shares no exterior frame. The integrator lifts and validates this block; I did not run `region_from_dossier.py` or `validate_city.py`. A read-only scratch check of this block against `region.schema.json` and `ids.json` is reported in section 11.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.treasure_chest_shop",
  "displayLabel": "Treasure Chest Shop",
  "source": {
    "sceneSymbol": "Z2_TAKARAYA",
    "sceneEnum": "SCENE_TAKARAYA",
    "sceneTableIndex": "0x17",
    "entranceTableSceneIndex": "0x14",
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
      "fresh save: no form switch flag set (prize pair index 0 for every form)",
      "player form Human, no mask",
      "scene has no alternate headers; the default header serves every layer",
      "gal skeletal pose UNKNOWN beyond the idle loop"
    ]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TAKARAYA", "bytes": 39040, "sha256": "871d64bf855cf90fa8a0400e452e3aa9b8aa4ada50163eb34147c7be1e3a1233" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TAKARAYA_room_00", "bytes": 32384, "sha256": "f815bec34debaa2dc637d2f6a1d0efa9c172d2e7f76f47c679003c264b068f50" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TAKARAYA/Z2_TAKARAYA.c", "sha256": "68de693bb565a3b820e69c4518857234a55b99accadab33944cf448f9c8956fd" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TAKARAYA/Z2_TAKARAYA_room_00.c", "sha256": "72f2341ccf3dfac12711c39954dc2d204a499cc3269b5737dfecb8602bc8f577" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_TAKARAYA/Z2_TAKARAYA.xml", "sha256": "bf338ed7261f1f1b75f1e5df4694575f1508622338b6e61ef0faffb75f967466" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": [],
      "status": "not_shared",
      "evidence": "interior scene with its own origin; East door at Y 100 vs interior Y 0 (section 2.5)"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-1680, 0, 0], "max": [80, 572, 964] },
    "sourceCollisionVertices": { "min": [-1680, 0, 0], "max": [80, 484, 960] },
    "unityCollisionVertices": { "min": [-16.8, 0, -9.6], "max": [0.8, 4.84, 0] }
  },
  "anchors": [
    { "id": "anchor.tcs.spawn0_inside_door", "source": [-70, 0, 161], "unity": [-0.7, 0, -1.61], "status": "proposed" },
    { "id": "anchor.tcs.door_ta0", "source": [-70, 0, 100], "unity": [-0.7, 0, -1], "status": "proposed" },
    { "id": "anchor.tcs.spawn1_game_return", "source": [-91, 0, 744], "unity": [-0.91, 0, -7.44], "status": "proposed", "note": "entrance 1 arrival after a game ends" },
    { "id": "anchor.tcs.gal_stand", "source": [-85, 0, 849], "unity": [-0.85, 0, -8.49], "status": "proposed" },
    { "id": "anchor.tcs.counter_top", "source": [-79.5, 40, 826], "unity": [-0.795, 0.4, -8.26], "status": "proposed", "note": "mid of floor polys 79 and 80, Y 40" },
    { "id": "anchor.tcs.board_center", "source": [-1020, 0, 480], "unity": [-10.2, 0, -4.8], "status": "proposed", "note": "centre of the 11 x 8 cell board, floor polys 71 and 72" },
    { "id": "anchor.tcs.wall_clock", "source": [-300, 160, 831], "unity": [-3, 1.6, -8.31], "status": "proposed", "note": "elevation anchor" }
  ],
  "rooms": [
    {
      "id": "loc.treasure_chest_shop.room_00",
      "sourceRoom": "Z2_TAKARAYA_room_00",
      "header": 0,
      "roomShape": "cullable, 16 entries",
      "displayLists": 21
    }
  ],
  "components": [
    {
      "sourceKey": "Z2_TAKARAYA/h0/room_00/a01",
      "actorId": "0x19C",
      "params": "0x907F",
      "class": "static-mesh",
      "dls": [
        "object_obj_tokeidai:0xCF28 gClockTowerMinuteRingDL",
        "object_obj_tokeidai:0xBEE8 gClockTowerClockCenterAndHandDL",
        "object_obj_tokeidai:0xF518 gWallClockClockFaceDL",
        "object_obj_tokeidai:0xC368 gClockTowerSunAndMoonPanelDL"
      ],
      "status": "selected",
      "note": "wall clock scale 0.02 yaw 180, time-posed"
    },
    {
      "sourceKey": "Z2_TAKARAYA/h0/room_00/a02",
      "actorId": "0x1C1",
      "params": "0x0000",
      "class": "npc-scheduled",
      "dls": [
        "object_bg:0x8FC8 gTreasureChestShopGalSkel"
      ],
      "status": "optional",
      "note": "gal, present D0d to D3d (mask 0x3F8), idle loop object_bg_Anim_009890, scale 0.01 default; skeletal pose deferred"
    },
    {
      "sourceKey": "Z2_TAKARAYA/h0/transition/ta0",
      "actorId": "0x005",
      "params": "0x0288",
      "class": "door-transition",
      "variant": "posed-skeletal",
      "dls": [
        "gameplay_keep:0x22B28 gDoorSkel",
        "gameplay_keep:0x20BB8 gDoorLeftDL",
        "gameplay_keep:0x20D00 gDoorRightDL"
      ],
      "status": "selected",
      "note": "closed pose; schedule TREASURE_CHEST_SHOP; exit floor polys 69 and 70 behind it"
    },
    {
      "sourceKey": "Z2_TAKARAYA/h0/room_00/a02/wall",
      "actorId": "0x1BB",
      "params": "0xB0A0,0xB180,0xB080,0xB540,0xB520",
      "class": "static-mesh",
      "variant": "spawned",
      "dls": [
        "object_takaraya_objects:0xB70 gTreasureChestShopWallBlackDL",
        "object_takaraya_objects:0xD60 gTreasureChestShopWallWhiteDL"
      ],
      "status": "optional",
      "note": "runtime child of the gal, spawned at (0, 0, 0) on payment; params = prize (purple, heart piece, red, 10 nuts, 5 nuts); up to 88 cells of 120 units rise only next to the player"
    },
    {
      "sourceKey": "Z2_TAKARAYA/h0/room_00/a02/chest",
      "actorId": "0x006",
      "params": "0xB0A0,0xB180,0xB080,0xB540,0xB520",
      "class": "static-mesh",
      "variant": "spawned",
      "dls": [
        "object_box:0x66A0 gBoxChestSkel"
      ],
      "status": "optional",
      "note": "big chest at (-1635, 0, 60 + 120 column), column random 0..7, spawned by the wall actor; prize chosen by form"
    }
  ],
  "collisionOwnership": {
    "room": "Z2_TAKARAYACollisionHeader_000A30 (97 polys)",
    "waterBoxes": 0,
    "actorDynapoly": [
      "a02/chest En_Box gBoxChestCol (runtime, after payment)"
    ],
    "actorCylinders": [
      "a02/wall Obj_Takaraya_Wall columnCollider and rowCollider (OC cylinders, init radius 40 height 120, radius 200 while active)"
    ],
    "note": "En_Takaraya has no collider; the board floor is room collision polys 71 and 72 and the walls are OC cylinders, not collision polys"
  },
  "portalIds": [
    "portal.treasure_chest_shop.east_clock_town",
    "portal.treasure_chest_shop.east_clock_town.kickout"
  ],
  "materialBindings": null,
  "cameraIds": [
    "cam.treasure_chest_shop.entry",
    "cam.treasure_chest_shop.reverse_door",
    "cam.treasure_chest_shop.fixture"
  ],
  "routeIds": [],
  "omissions": [
    "a02 En_Takaraya skeletal pose, head tracking and blink",
    "Link and Tatl"
  ],
  "excluded": {
    "halfDayMask": [],
    "stateOrSchedule": [],
    "noGeometry": [
      "a00",
      "a03",
      "a04"
    ]
  },
  "unsupported": [
    "skeletal NPC poses",
    "payment, 45 s timer, win and time-up states and the return to entrance 1",
    "random 11 x 8 maze generation and the rising wall cells",
    "per-form fee, prize pair and the persistent Goron Heart Piece flag",
    "door schedule closed 22:00-05:59 and the 22:00 kickout with its game deferral",
    "BgCam 2 CAM_SET_TREASURE_CHEST_MINIGAME camera and the chest cutscenes 9 and 10",
    "positional shop BGM and the game sub-BGM",
    "wall clock time-driven pose unless implemented"
  ],
  "evidence": [
    "docs/city/interiors/treasure_chest_shop.md"
  ],
  "readiness": {
    "sourceInspection": "pass",
    "objectExtraction": "not_run",
    "import": "not_run",
    "controllerTraversal": "not_run",
    "editorRender": "not_run",
    "packagedRender": "not_run",
    "visualApproval": "not_run"
  },
  "readinessNotes": {
    "sourceInspection": "fee, prize and text numbers come from the decoded message table (MSG) and an independent baserom decode; maze statistics are a scratch simulation"
  }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Exit index 1 `0xD650` (LOCATION_REGISTER U-03, PORTAL_REGISTER U4): no reader found in this scene | Does any cutscene `nextEntrance` data or global code index `setupExitList[1]` in `SCENE_TAKARAYA`? Collision, En_Time_Tag (index 0) and the other overlays here do not; the scene has no cutscene script list |
| U-02 | After a lost game that ends after 22:00 the kickout appears to be skipped (5.8) | Confirm by trace or capture: actor spawn order a02 then a03 (`room.c:38-43`), `z_en_takaraya.c:127-130`, and the minute-equality test at `z_en_time_tag.c:333`; also confirm the intended text 0x1892 flow when the win text clears the flags |
| U-03 | Whether Player reads its spawn csId (0 here) | Grep `this->actor.csId` in `ovl_player_actor/z_player.c`; shared with Bomb Shop U-12 |
| U-04 | Meaning of entrance flag bit 14 (set in `0x4102`, clear in `0x0102`) | Read `z_scene.c:540`, `z_play.c:2275` and `:583` consumers of `EntranceTableEntry.flags` for bit 14 |
| U-05 | Consumer of the scene sub-keep `GAMEPLAY_DANGEON_KEEP` (segment 5) | Search En_Box, Demo_Tre_Lgt and the chest effects for segment-5 DLs; the room DLs have none |
| U-06 | Behaviour of BgCam setting 0x39 `CAM_SET_TREASURE_CHEST_MINIGAME` and bgCam 1 `CAM_SET_ROOM0` with no data | Read the camera setting handlers in `src/code/z_camera.c` for settings 0x39 and 0x3F; needed for `cam.treasure_chest_shop.*` |
| U-07 | Cutscene entries 0-8 (chain) and the actor cs camera `{37, 3, points}` are unreferenced; the spawns carry csId 0 | Does Player's csId 0 start entry 0's chain (U-03)? Any other `CutsceneManager_*` caller with csId 0..8 in this scene |
| U-08 | Rise, fall and push behaviour of the wall OC cylinders on Player (blocking versus pushing) | `CollisionCheck_SetOC` consumers and Player's OC response (`z_collision_check.c`, `z_player.c`), mass immovable |
| U-09 | Game RNG state: SIM used a uniform RNG; real boards depend on `Rand_ZeroFloat` state | `src/code/z_lib.c` RNG and seeding, only if exact board reproduction is wanted |
| U-10 | Frame timing of the 45 s window versus the 40-frame HUD preface and the 145-frame chest cutscene | Count frames from `func_80ADF2D4` to COUNTING (`z_parameter.c:5744-5812`) in a run |
| U-11 | Which room DLs are the counter, shelves, pillars and board-hall walls | Render room DLs per cullable entry (REF-03 `cam.treasure_chest_shop.fixture`); section 8 readings are INF |
| U-12 | Visual survey | Captures `cam.treasure_chest_shop.entry`, `.reverse_door`, `.fixture` under `prof.d1.noon.normal`, plus a mid-game capture with risen walls |

Register wording notes (not edits): PORTAL_REGISTER "open hours UNKNOWN" on both Treasure Chest Shop door rows is resolved here (open 06:00-21:59, closed 22:00-05:59); its teleport-only entry "Treasure Chest Shop 1 (INT-11 pending)" is the game-end return spawn of section 2.1. LOCATION_REGISTER counts agree (4 objects / 5 actors, 1 transition actor, 2 spawns, 2 exits, 2 / 2 entrances) and its U-03 (`0xD650`) is only partly answered here (U-01). The brief names FULL_CITY_PLAN section 5 for the INT row; it is section 6.

**Not inspected:** room DL contents and combiners; light-setting values; minimap draw; En_Takaraya head-tracking maths; Player walk-in helper; Player OC response; the cutscene camera handlers; message text beyond ids and control-code skeletons; 3DS material; Day 2 and later layouts beyond masks.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN sections 6/11/12, the three registers, the Bomb Shop, Lottery Shop and Milk Bar dossiers and records, North and East templates, `shared/city/schema/README.md`, `region.schema.json` and `ids.json`; read scene and room C and XML, scene/actor/object/sequence tables, `z_actor.c`, `z_scene.c`, `z_scene_table.c`, `z_play.c`, `z_sram_NES.c`, `z_parameter.c`, `z_room.c`, `z_kankyo.c`, `z_face_reaction.c`, `z_schedule.c`, `z_bgcheck.c`, `z64bgcheck.h`, `z64player.h`, `z64door.h`, `z64scripts.h`, `z64cutscene.h`, and the overlays En_Takaraya, Obj_Takaraya_Wall, En_Box, En_Time_Tag, En_Door (script and schedule use), En_Test4, Obj_Tokeidai, Obj_Sound. Scratch scripts in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\treasure_chest_shop\`: `analyze.py` (collision decode, door probe, actors, DL and texture counts, SHA-256; re-run and diffed identical), `dls.py`, `sim.py` (maze port), `msgprice.py` and `msgprice2.py` (baserom price and header decode, numbers only), `msgs.py` (structure-only read of the local message JSON), `check_region.py` (section 10 against the schema and registry, writes nothing). SHA-256 of the five inputs and the ZAPD binary (matches the 17 accepted records); grep of the 22 scenes and `src/` for `0x2810`, of `src/` for `ENTRANCE(TREASURE_CHEST_SHOP` and for `setupExitList` readers.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or build, captures, renders, controller routes, `region_from_dossier.py`, `validate_city.py`.

**Next dependency:** REF-03 captures for the three `cam.treasure_chest_shop.*` IDs; integrator acceptance of section 10; section 9 extraction; the minigame pass (payment, board, timer, chest, return) is a separate engineering packet after the room shell exists.
