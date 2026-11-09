# Stock Pot Inn (`loc.stock_pot_inn`): source inventory dossier

Task **REF-09 / INT-07** (FULL_CITY_PLAN.md sections 6 and 12; the multi-room interior pilot). Base commit `31b20dc` on `main`. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_YADOYA` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save, Human form; `docs/city/SOURCE_REGISTER.md`).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_YADOYA/Z2_YADOYA.c`, `rNN.c` = `.../Z2_YADOYA_room_NN.c`, `player.c` = `src/overlays/actors/ovl_player_actor/z_player.c`, `door.c` = `src/overlays/actors/ovl_En_Door/z_en_door.c`, `door.schl` = `.../ovl_En_Door/scheduleScripts.schl`, `an.c` = `src/overlays/actors/ovl_En_An/z_en_an.c`. Overlay files are `src/overlays/actors/ovl_<Name>/z_<name>.c`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. Line numbers in room files count `\n` only (the files also hold stray `\r`; text-mode readers that split on `\r` drift). Cross-references: `docs/city/LOCATION_REGISTER.md` (REF-01), `PORTAL_REGISTER.md` (REF-02), `SOURCE_REGISTER.md` (REF-03), `docs/city/districts/east_clock_town.md` (REF-07, door side), `north_clock_town.md` (template).

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro/function; **OVL** read from the actor overlay source (including its `.schl` schedule script); **ANN** decomp-author comment or symbol name; **INF** inferred (reason given); **NI** not inspected.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum / index | `Z2_YADOYA` / `SCENE_YADOYA` / `0x61` | HDR `include/tables/scene_table.h:199` |
| Entrance-table scene index | `0x5E` (`sStockPotInnEntranceTable`, rows `z_scene_table.c:2000-2029`) | `src/code/z_scene_table.c:2667` |
| Title / draw / restrictions / cycle flags | `0x10A` / `SCENE_DRAW_CFG_MAT_ANIM` / `RESTRICTIONS_INDOORS` / `PERSISTENT_CYCLE_FLAGS_NONE` | HDR scene_table.h:199 |
| XML | scene `Z2_YADOYA` segment 2; five room files `Z2_YADOYA_room_00..04` segment 3, four DLs declared per room; dependency `misc/scene_texture_01.xml` | XML `assets/xml/scenes/Z2_YADOYA/Z2_YADOYA.xml` |
| Room count | 5 in both scene headers | HDR scene.c:21, :4090 |
| Headers | scene default (`Z2_YADOYACommands`, :18-38) + alt1 (`Z2_YADOYASet_00A050`, :4088-4107); every room has default + alt1 (`r00.c:30-32`, :159; `r01.c` :103; `r02.c` :111; `r03.c` :100; `r04.c` :121) | HDR |
| Sound | spec 5, ambience 18, seq 31 = `NA_BGM_INSIDE_A_HOUSE` (alt1: 5, 19, 118 = `NA_BGM_TITLE_THEME`) | HDR scene.c:20, :4089; `include/tables/sequence_table.h:82, :256` |
| Skybox / lighting | `SKYBOX_SETTINGS(0x01, 0, 0, true)`: lightMode true, i.e. not the time-driven light path (SOURCE_REGISTER 2.6); 5 `LightSettings` per header, byte-identical between headers (values not transcribed) | HDR scene.c:28, :92-103, :4143-4154 (diff run) |
| Special files | none: no `SCENE_CMD_SPECIAL_FILES`, so no sub-keep (`gameplay_field_keep`) is loaded (`z_scene.c:234-237`) | HDR (grep) |
| Transition actors | 6 (`Z2_YADOYATransitionActorList_0000FC`, :59-66; alt1 :4114-4121) | HDR |
| Paths / actor cutscenes / cs cameras | 42 / 30 / 22 (alt1: 41 / 10 / 9) | HDR scene.c:26, :32-33, :4095, :4101-4102 |
| Cutscene scripts | default 2 (`CutsceneData_000D34`, `_002454`, 1804 frames each, next entrance `0xFFFF`); alt1 1 (`_00AC44`, 552 frames, next entrance `0xD201`) | HDR scene.c:1764-1767, :4866-4868 |
| Animated material | 1 entry, segment 8 (`-1` + 7), type 1 two-texture scroll (-1,0)/(0,-1), 32x32; used by the room_00 water quad at Y -15 (x -495..-390, z 210..375) | HDR scene.c:1913-1920; `src/code/z_scene_proc.c:400-406`; `r00.c:3086` |
| Minimap | 5 entries (one per room, INF), compass list 2 chests: room 2 flag 1 (-358, 210, 24), room 3 flag 0 (-452, 210, 326) | HDR scene.c:1802-1817 |
| Room commands (all rooms) | echo 4; `SKYBOX_DISABLES(true, true)`; `TIME_SETTINGS(255, 255, 3)` default, `(16, 11, 3)` in every alt1 room header (forced 16:11); `ROOM_BEHAVIOR(0, 0, 0, 0, enablePosLights, 0)` with enablePosLights false in room_00, true in rooms 01-04 | HDR `rNN.c:20-23` and alt lines; macro `include/z64scene.h:804-807`; `z_scene.c:251` |
| Room shapes | cullable: r00 12 entries, r01 8, r02 6, r03 8, r04 8 | HDR `rNN.c` RoomShapeCullable |

### 1.1 What scene layer 1 (alt1) changes

Selection rule (C, SOURCE_REGISTER 2.5): `altHeaders[sceneLayer - 1]` when `sceneLayer != 0`. The Inn entrance table has a layer-1 row only for entrance 0: `{ SCENE_YADOYA, 0, 0x8102 }` (`z_scene_table.c:2000-2003`). The only extracted writer is South alt3's cutscene script `{ ..., 0xBC01, ... }` (`Z2_CLOCKTOWER.c:3452-3454`), and the Inn alt1 script leads on to East layer 1 (`0xD201`, scene.c:4867). INF: the title/attract chain (East U-02). Ordinary play uses the default headers.

| Item | Default (layer 0) | Alt1 (layer 1) |
| --- | --- | --- |
| Spawns / entrance list | 6 / 6 | 1 / 2 (both spawn 0, room 0): (300, 15, 145) yaw 270, params `0x0F03` (scene.c:4109-4112, :4131-4134) |
| Exit list | 4 words (:85-90) | identical 4 words (:4136-4141) |
| Transition actors | as section 2.4 | same positions and params; side camera indices remapped to the 9-entry alt cs-camera list, each pointing at the same pose (default 16/12 -> 8/4, 15/7 -> 7/0, 14/8 -> 6/1, 13/9 -> 5/2; checked point by point) |
| Room object lists | 18/16/18/16/16 | 9/8/9/9/8; adds `OBJECT_MAG`, drops most NPC banks |
| Room actor lists | 26/8/12/7/14 | 21/9/7/6/12; adds `EN_MAG` ("Title Logo", ANN) in room_00 and `DM_AN` ("Anju (cutscene)") in room_02 |
| Room time | free (255, 255), speed 3 | forced 16:11, speed 3 |

### 1.2 Which room is which (C where cited, role names INF)

| Room | Role | Evidence |
| --- | --- | --- |
| room_00 | lobby and reception (Y 0), kitchen with boiling pot and water basin (west, Y -8/-23), toilet under the stair ramp, 41-degree ramp stair, upper corridor (Y 210) with both street doors | spawns 0, 1, 3 land here (scene.c:77-82); reception counter collision x 30..150, z 0..180; `ANJU_SCH_RECEPTIONIST_IDLE` uses path chain step 10 = path 18, which ends at (101, 0, 103), her room_00 entry point (OVL an.c:164-200, :2683-2725; scene.c:265-300, :666); `Obj_Sound` sfx index 17 = `NA_SE_EV_BOILED_WATER_S` (`src/audio/code_8019AF00.c:1925-1937`); hearth `En_Light 0x07F4`; toilet floor surface 5 uses `CAM_SET_FIXED3` ("Stock Pot Inn Toilet", ANN `include/z64camera.h:186`); `En_Bjt` toilet hand |
| room_01 | Granny's room (ground floor, west) | TA0 schedule type 13 `INN_GRANNYS`; `En_Nb` "Anju's Grandma" (ANN) at (-326, 0, -75) |
| room_02 | staff room (Anju's room, upper floor) | TA1 type 14 `INN_STAFF_ROOM`; `En_Drs` "Anju's Wedding Dress Mannequin"; `Dm_Char05` Couple's Mask prop; chest flag 1 |
| room_03 | Knife Chamber (rentable room, upper floor) | TA2 type 15 `INN_KNIFE_CHAMBER` (Room Key); chest flag 0; `Dm_Tag` eavesdrop tag |
| room_04 | Large Suite (upper floor, bunk beds with climbable ends) | TA3 type 16 `INN_LARGE_SUITE`; Guru-Guru, Rosa sisters, jugglers; floors at Y 300-329 over Y 225 |

Room footprints from collision (proxy: each room's surfaces carry their own light index, section 7): room_01 x -480..-150, z -300..0, Y 0; room_02 x -600..-270, z -240..60; room_03 x -600..-270, z 90..390; room_04 x -602..-270, z 420..960 (all Y 210+); room_00 is everything else (x -464..330, Y -30..390).

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:44-57; decode C)

Player params: start mode `(params & 0xF00) >> 8`, start camera `params & 0xFF` (`include/z64player.h:17-18`). A start camera other than `0xFF` is passed to `Camera_ChangeActorCsCamIndex`, i.e. it indexes the scene's actor-cutscene camera list, not `BgCamInfo` (`src/code/z_play.c:2326-2327`). This also answers North U-02 for that dossier's owner.

| Spawn | Position | Yaw | Params | Mode / cam | Entrance -> room | Used by |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | (149, 0, 303) | 180 | 0x0F0B | 0xF / cs cam 11 `CAM_SET_START2` at (251, 38, 306) | 0 -> room_00 | East exit `0xBC00` (East TA0, lower street door); 65 u inside TA5 |
| 1 | (-63, 210, -46) | 180 | 0x0D0A | 0xD / cs cam 10 `START2` at (-195, 343, -137) | 1 -> room_00 | East exit `0xBC10` (East TA1, upper deck door); 46 u inside TA4 |
| 2 | (-337, 0, -135) | 16 | 0x0DFF | 0xD / none | 2 -> room_01 | `ENTRANCE(STOCK_POT_INN, 2)` after Granny's story (OVL `ovl_En_Nb/z_en_nb.c:475`); equals Inn exit [3] `0xBC20` |
| 3 | (-254, 0, 227) | 90 | 0x0DFF | 0xD / none | 3 -> room_00 | `ENTRANCE(STOCK_POT_INN, 3)` after the midnight meeting (OVL an.c:1591-1600) |
| 4 | (-450, 210, 10) | 180 | 0x0DFF | 0xD / none | 4 -> room_02 | `ENTRANCE(STOCK_POT_INN, 4)` from the Knife Chamber tag (OVL `ovl_Dm_Tag/z_dm_tag.c:202`) |
| 5 | (-345, 210, 129) | 180 | 0x0DFF | 0xD / none | 5 -> room_03 | `ENTRANCE(STOCK_POT_INN, 5)` back from the staff room (OVL z_dm_tag.c:183) |

Entrance list `{spawn, room}` = (0,0) (1,0) (2,1) (3,0) (4,2) (5,3) (HDR scene.c:76-83). Entrance-table rows: 0 `{0, 0x4102}` + layer 1 `{0, 0x8102}`; 1 `0x4102`; 2-5 `0x0102` (`z_scene_table.c:2000-2022`); flag semantics NI. The debug map select also uses entrance 0 (`ovl_select/z_select.c:313`, excluded). Cs cameras: `Z2_YADOYAActorCsCamInfo_003FD0[k]` at scene.c:1889+k, points at :1820+3k.

### 2.2 Exit list (HDR scene.c:85-90; identical alt1 :4136-4141) and consumers

| Exit idx | Raw | Decode (C) | Destination | Consumer in the Inn |
| --- | --- | --- | --- | --- |
| 0 | `0xD290` | 0x69 East, entr 9, layer 0 | East spawn 9 (669, 100, -846) | collision surface 3 (TA5 lower door); kickout `En_Time_Tag` exit index `0x0040 & 0x1F = 0` in all 5 rooms (OVL `z_en_time_tag.c:287`, macros `.h:10-15`) |
| 1 | `0xD2A0` | 0x69 East, entr 10 | East spawn 10 (477, 280, -1270), Inn upper deck | collision surface 8 (TA4 upper door) |
| 2 | `0xD650` | 0x6B North, entr 5 | North spawn 5 (Bombers) | none: no surface has exit+1 = 3 and no Inn actor reads `setupExitList` except `En_Time_Tag` (grep of the 30 Inn room-actor overlays plus En_Door) |
| 3 | `0xBC20` | 0x5E Inn, entr 2 | Inn spawn 2, room_01 | none in data; `En_Nb` builds the same value with the `ENTRANCE` macro instead (z_en_nb.c:475) |

### 2.3 Collision surfaces carrying exits (C from scene.c:1933-1942 and the polygon list; poly i is at scene.c:1945+i)

| Surface | data[0] / data[1] | Exit+1 -> slot | Polys | Kind | Extent x / y / z | Flags |
| --- | --- | --- | --- | --- | --- | --- |
| 3 | 0x00000101 / 0x00001082 | 1 -> 0 `0xD290` | 181, 182 | floor, stone | 80..280 / 0 / 360..640 | xp 1 (ignore camera) |
| 8 | 0x00000201 / 0x00001082 | 2 -> 1 `0xD2A0` | 344, 345 | floor, stone | -120..0 / 210 / 0..120 | xp 1 |

Both are camera-ignored floors hidden behind a street door; no conveyor, no wall carries exit bits. Every other surface has exit+1 = 0 (col.py over 29 surface types).

### 2.4 The 6 transition actors (all `EN_DOOR`, HDR scene.c:59-66; decode C)

Mechanism (C): `TransitionActorEntry {sides[2] {room, bgCamIndex}, id, pos, rotY, params}` (`include/z64scene.h:369-378`); spawned while either side's room is the current or previous room (`src/code/z_actor.c:3600-3625`); yaw `(rotY >> 7) & 0x1FF`. `EN_DOOR` params: type `(p >> 7) & 7` = 5 `ENDOOR_TYPE_SCHEDULE`, type var `p & 0x7F` = `ENDOOR_SCH_TYPE_*` (`ovl_En_Door/z_en_door.h:9-20, :54-88, :95-96`). The player on the door's local +Z side gets `doorDirection = +1` (door.c:494-496) and goes to `sides[0].room`; on the -Z side to `sides[1].room` (player.c:6841-6847). The door camera is `sides[...].bgCamIndex`, -1 (255) = generic `CAM_SET_DOORC` (`src/code/z_camera.c:7977-7978`), otherwise an index into the actor-cutscene camera list (`z_camera.c:767-779`). The probe floor is 22 u past the door on the far side at door Y + 10 and decides a scene exit (player.c:6779-6783, :6548-6551; scratch `col2.py`). The schedule script runs only from the +Z side (door.c:537).

| TA | Position | Yaw | Params -> schedule type | Front (sides[0]) room / cam | Back (sides[1]) room / cam | Probe from -Z side | Probe from +Z side | Art (door.c:189-245, :306-321) |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-150, 0, -150) | 90 | 0x028D -> 13 `INN_GRANNYS` | room_01 / 16 FIXED1 (-412, 100, -258) | room_00 / 12 FIXED1 (-107, 93, -302) | poly 6 (room_00, no exit) | poly 362 (room_01, no exit) | objinfo 30 = `gameplay_field_keep`, not loaded, so fallback objinfo 15: `gameplay_keep` `gDoorLeftDL` 0x20BB8 / `gDoorRightDL` 0x20D00 (door.c:370-376) |
| 1 | (-270, 210, -90) | 90 | 0x028E -> 14 `INN_STAFF_ROOM` | room_02 / 15 (-526, 337, 21) | room_00 / 7 (-217, 368, -138) | poly 296 (room_00) | poly 481 (room_02) | same fallback |
| 2 | (-270, 210, 240) | 90 | 0x028F -> 15 `INN_KNIFE_CHAMBER` | room_03 / 14 (-553, 308, 379) | room_00 / 8 (-136, 302, 365) | poly 298 | poly 701 (room_03) | same fallback |
| 3 | (-270, 210, 510) | 90 | 0x0290 -> 16 `INN_LARGE_SUITE` | room_04 / 13 (-566, 364, 464) | room_00 / 9 (-141, 311, 744) | poly 306 | poly 713 (room_04) | same fallback |
| 4 | (-60, 210, 0) | 0 | 0x028C -> 12 `INN_UPPER_ENTRANCE` | room_00 / -1 | room_00 / -1 | **poly 345, exit+1 2 -> `0xD2A0`** | poly 314 (no exit) | `object_wdor03` `gInnSchoolDoorDL` 0x548 (objinfo 29; bank in room_00 list) |
| 5 | (180, 0, 360) | 0 | 0x028B -> 11 `INN_MAIN_ENTRANCE` | room_00 / -1 | room_00 / -1 | **poly 182, exit+1 1 -> `0xD290`** | poly 246 (no exit) | `object_wdor03` `gInnSchoolDoorDL` 0x548 (objinfo 28) |

For yaw 90 the local +Z side is world +X, i.e. room_00's corridor/lobby side: rooms 1-4 are entered from +Z and their schedule gates apply only on entry from room_00; leaving any room is never gated (C). For the street doors the interior is the -Z side, so leaving the Inn is never schedule-gated; the `INN_MAIN_ENTRANCE` gate is the East-side copy (East TA0). This resolves LOCATION_REGISTER U-10 (C): TA4 is the upper deck door, TA5 the lower street door, matching East 2.4 (East TA0 `0x028B` -> `0xBC00` -> spawn 0; East TA1 `0x028C` -> `0xBC10` -> spawn 1). No disagreement with the East table or the PORTAL_REGISTER rows.

### 2.5 Proposed portal IDs (reuse of `docs/city/PORTAL_REGISTER.md` names; conditions added)

| Portal ID (frozen in REF-02) | Source | Condition from this dossier |
| --- | --- | --- |
| `portal.east_clock_town.stock_pot_inn.lower` (inbound) | East TA0 -> `0xBC00` -> spawn 0 | `INN_MAIN_ENTRANCE` from the street: Days 1-2 open 08:00-20:30; Day 3 open from 08:00 on (incl. the final night); always open with the Room Key in inventory (door.schl:213-234; time base 06:00, `include/z64scripts.h:7`) |
| `portal.east_clock_town.stock_pot_inn.upper` (inbound) | East TA1 -> `0xBC10` -> spawn 1 | always open (door.schl:237-239) |
| `portal.stock_pot_inn.east_clock_town.lower` | TA5 -> poly 182 -> `0xD290` | never gated from inside (C, 2.4) |
| `portal.stock_pot_inn.east_clock_town.upper` | TA4 -> poly 345 -> `0xD2A0` | never gated from inside; the only Inn route to the East upper deck (roof/deck entry, 5.2) |
| `portal.stock_pot_inn.east_clock_town.kickout` | 5 x `En_Time_Tag 0x0040` | 20:30 on half-days D0d..D2n; skipped in this scene with `ITEM_ROOM_KEY` (z_en_time_tag.c:319-338); text `0x1883 + 2` |
| `portal.stock_pot_inn.stock_pot_inn.r0_r1` / `.r1_r0` | TA0 | both always open (door.schl:242-244) |
| `portal.stock_pot_inn.stock_pot_inn.r0_r2` / `.r2_r0` | TA1 | r0 -> r2 only Day 3 from 18:00, else text `0x1814` (door.schl:247-258); r2 -> r0 ungated |
| `portal.stock_pot_inn.stock_pot_inn.r0_r3` / `.r3_r0` | TA2 | r0 -> r3 needs `ITEM_ROOM_KEY` in inventory (`z_schedule.c:106`), else `0x1816` (door.schl:261-267); r3 -> r0 ungated |
| `portal.stock_pot_inn.stock_pot_inn.r0_r4` / `.r4_r0` | TA3 | both always open (door.schl:270-272) |

Special internal relocations (not portals in the register): Granny's story -> spawn 2; midnight meeting -> spawn 3; eavesdrop Knife Chamber -> spawn 4 (room_02) -> spawn 5 (room_03) (section 5.4). Proposed IDs if PIPE-01 wants them: `portal.stock_pot_inn.stock_pot_inn.story_r1`, `.midnight_r0`, `.eavesdrop_r2`, `.eavesdrop_r3`.

## 3. Object banks (HDR room lists; IDs `include/tables/object_table.h`)

| Header (line) | n | Banks in load order (id) |
| --- | --- | --- |
| r00 default (r00.c:34) | 18 | AN1 0x0E2, AN2 0x0E0, AN3 0x0DD, AH 0x007, BOJ 0x0E3, FU 0x0FF, DAI 0x1D5, RZ 0x24B, TEST3 0x01C, IN2 0x248, YADO_OBJ 0x26E, OBJ_TOKEIDAI 0x18C, HARIKO 0x1F2, MM 0x107, DRS 0x018, MSMO 0x016, BJT 0x24F, WDOR03 0x276 |
| r01 default (r01.c:34) | 16 | AN1..IN2 (same first 10), YADO_OBJ, OBJ_TOKEIDAI, HARIKO, NB 0x004, DRS, MSMO |
| r02 default (r02.c:34) | 18 | AN1..TEST3 (first 9), YADO_OBJ, OBJ_TOKEIDAI, HARIKO, AN4 0x01A, DRS, MSMO, BOX 0x00C, GI_MASK13 0x282, DMASK 0x213 |
| r03 default (r03.c:34) | 16 | AN1..IN2, YADO_OBJ, OBJ_TOKEIDAI, HARIKO, BOX, DRS, MSMO |
| r04 default (r04.c:34) | 16 | AN1..IN2, YADO_OBJ, OBJ_TOKEIDAI, HARIKO, MM, DRS, MSMO |
| r00 alt1 (r00.c:170) | 9 | AN1, DAI, RZ, YADO_OBJ, OBJ_TOKEIDAI, HARIKO, MSMO, WDOR03, MAG 0x115 |
| r01 alt1 (r01.c:114) | 8 | AN1, DAI, RZ, YADO_OBJ, OBJ_TOKEIDAI, HARIKO, MSMO, MAG |
| r02 alt1 (r02.c:122) | 9 | AN1, DAI, RZ, YADO_OBJ, OBJ_TOKEIDAI, HARIKO, MSMO, BOX, MAG |
| r03 alt1 (r03.c:111) | 9 | AN1, DAI, RZ, YADO_OBJ, OBJ_TOKEIDAI, HARIKO, BOX, MSMO, MAG |
| r04 alt1 (r04.c:132) | 8 | AN1, DAI, RZ, YADO_OBJ, OBJ_TOKEIDAI, HARIKO, MSMO, MAG |

Room-change rule (C): a new room's object list keeps loaded slots only while they match position by position; at the first mismatch every later slot is invalidated and actors using them are killed (`src/code/z_scene.c:277-290`, `Actor_KillAllWithMissingObject`). The nine-bank prefix AN1..TEST3 is identical in all five default lists, so the cross-room NPC banks (Anju AN1/AN2/AN3, Anju's mother AH, jugglers BOJ, Guru-Guru FU, Goron DAI, Rosa RZ, Kafei TEST3) survive every room change; room_00 <-> room_01/03 keep 13 slots, room_00 <-> room_04 keeps 16, room_00 <-> room_02 keeps 9. Actor spawn fails when the profile's object is not loaded (`src/code/z_actor.c:3516-3522`); entries marked NOT LOADED in section 4 never spawn. Implicit: `gameplay_keep` (lights, fish, insects, tags, fallback doors). No sub-keep (section 1).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (:3628-3655): id `& 0x1FFF`; rotation `(raw >> 7) & 0x1FF` degrees unless the id carries the axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), which passes a plain integer (marked `i` below); csId = `rawRotY & 0x7F` (0x7F none); half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 = all. `SPAWN_ROT_FLAGS(r, f) = (r << 7) | f` (`include/z64scene.h:16`).
- `Actor_SpawnSetupActors` (:2546-2575) spawns entries whose mask holds the current half-day bit (Day 1 daytime 0x080, :2491-2498). Room actors are per room: `Room_ProcessRoomRequest` runs the new room's commands (`src/code/z_room.c:606-618`) and `Room_FinishRoomChange` kills every actor with `room >= 0` that is neither the current nor the previous room (`z_room.c:655-668`, `z_actor.c:3278-3300`). Actors that set `room = -1` persist across rooms.
- Masks used below: 0x3f8 D0d..D3d (no D3n/D4); 0x3f0 D0d..D2n; 0x2aa all daytimes; 0x3c0 D0d..D1n; 0x0a0 D1d+D2d; 0x0ff D1d..D4n; 0x0fc D1d..D3n; 0x03f D2d..D4n; 0x3df all but D2d; 0x3fb all but D3n; 0x170 D0n+D1n+D2d+D2n; 0x060 D1n+D2d; 0x054 D1n+D2n+D3n; 0x050 D1n+D2n; 0x028 D2d+D3d; 0x020 D2d; 0x010 D2n; 0x00c D3d+D3n; 0x005 D3n+D4n.
- Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled`, `billboard/effect`, `controller/invisible`, `door/transition` (transition actors are section 2.4). "D1d" = mask includes Day 1 daytime; presence at 12:00 is section 6.

### 4.2 r00 default (26 entries, `Z2_YADOYA_room_00ActorEntry_000070` line 55)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 56 | EN_LIGHT (0x004) | (-112, 100, 15) | 0/0/0 | 0x3f8 | - | 0x83f0 | gameplay_keep | billboard/effect | yes |
| 1 | 59 | EN_LIGHT (0x004) | (278, 100, -292) | 0/0/0 | 0x3f8 | - | 0x83f0 | gameplay_keep | billboard/effect | yes |
| 2 | 62 | EN_AH (0x253) | (100, 0, 67) | 0/74/0 | 0x028 | - | 0x0000 | object_ah | npc-scheduled | no |
| 3 | 65 | OBJ_KINOKO (0x23b) | (101, 4, -86) | 0/0/0 | all | - | 0x0006 | gameplay_keep | controller/invisible | yes |
| 4 | 68 | EN_GURUGURU (0x248) | (-513, 210, 610) | 0/0/0 | 0x0a0 | - | 0x0002 | object_fu | controller/invisible | yes |
| 5 | 71 | EN_FISH (0x017) | (-425, -19, 235) | 0/347/0 | all | - | 0x0001 | gameplay_keep | posed-skeletal | yes |
| 6 | 74 | EN_TIME_TAG \|0xe000 (0x1ab) | (134, 0, 197) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 7 | 77 | OBJ_YADO (0x294) | (-449, 58, 157) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 8 | 80 | OBJ_YADO (0x294) | (-449, 58, 252) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 9 | 83 | OBJ_YADO (0x294) | (-120, 269, 330) | 0/270/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 10 | 86 | OBJ_YADO (0x294) | (-120, 269, 675) | 0/270/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 11 | 89 | OBJ_YADO (0x294) | (210, 285, -30) | 0/180/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 12 | 92 | OBJ_HARIKO (0x20a) | (117, 70, 18) | 0/32/0 | all | - | 0x0000 | object_hariko | static-mesh | yes |
| 13 | 95 | EN_LIGHT (0x004) | (-330, -24, 76) | 0/0/0 | 0x2aa | - | 0x07f4 | gameplay_keep | billboard/effect | yes |
| 14 | 98 | EN_HIT_TAG \|0xe000 (0x265) | (314, 113, 180) | 0i/270i/0i | all | - | 0xfe00 | gameplay_keep | controller/invisible | yes |
| 15 | 101 | EN_INSECT (0x016) | (-277, 0, 241) | 0/107/0 | all | - | 0x0000 | gameplay_keep | posed-skeletal | yes |
| 16 | 104 | EN_TEST4 (0x15a) | (-926, 629, 676) | 0/0/0 | all | 10 | 0x0100 | gameplay_keep | controller/invisible | yes |
| 17 | 107 | OBJ_TOKEIDAI (0x19c) | (34, 93, 231) | 0/90/0 | all | - | 0xa07f | object_obj_tokeidai | static-mesh | yes |
| 18 | 110 | EN_BJT (0x27d) | (107, 0, -90) | 0/270/0 | 0x054 | - | 0x0000 | object_bjt | npc-scheduled | no |
| 19 | 113 | EN_AN (0x202) | (101, 0, 103) | 0/74/0 | 0x0ff | 21 | 0x0002 | object_an1 | npc-scheduled | yes |
| 20 | 116 | EN_GM (0x0a4) | (278, 0, 223) | 0/270/0 | 0x3df | - | 0x0003 | object_in2 | npc-scheduled | yes |
| 21 | 119 | EN_TEST3 (0x159) | (173, 0, 412) | 0/180/0 | 0x00c | 20 | 0x01e6 | object_test3 | npc-scheduled | no |
| 22 | 122 | EN_PM (0x1d5) | (185, 0, 428) | 0/180/0 | all | - | 0x0001 | object_mm | npc-scheduled | yes |
| 23 | 125 | EN_IG (0x276) | (180, 0, 420) | 0/180/0 | 0x3c0 | - | 0x0004 | object_dai | npc-scheduled | yes |
| 24 | 128 | EN_RZ (0x27b) | (41, 148, -113) | 0/90/0 | 0x0a0 | - | 0x0a00 | object_rz | npc-scheduled | yes |
| 25 | 131 | OBJ_SOUND (0x0f0) | (-328, 23, 73) | 0/0/0 | 0x3fb | - | 0x0091 | gameplay_keep | controller/invisible | yes |

### 4.3 r00 alt1 (21 entries, `Z2_YADOYA_room_00ActorEntry_000334` line 182)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 183 | EN_MAG (0x0c5) | (0, 0, 0) | 0/0/0 | all | - | 0x0000 | object_mag | controller/invisible | yes |
| 1 | 186 | EN_LIGHT (0x004) | (-112, 100, 15) | 0/0/0 | 0x3f8 | - | 0x83f0 | gameplay_keep | billboard/effect | yes |
| 2 | 189 | EN_LIGHT (0x004) | (278, 100, -292) | 0/0/0 | 0x3f8 | - | 0x83f0 | gameplay_keep | billboard/effect | yes |
| 3 | 192 | OBJ_KINOKO (0x23b) | (101, 4, -86) | 0/0/0 | all | - | 0x0006 | gameplay_keep | controller/invisible | yes |
| 4 | 195 | EN_RZ (0x27b) | (41, 148, -113) | 0/90/0 | 0x0a0 | - | 0x0a00 | object_rz | npc-scheduled | yes |
| 5 | 198 | EN_FISH (0x017) | (-425, -19, 235) | 0/347/0 | all | - | 0x0001 | gameplay_keep | posed-skeletal | yes |
| 6 | 201 | EN_TIME_TAG \|0xe000 (0x1ab) | (134, 0, 197) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 7 | 204 | OBJ_YADO (0x294) | (-449, 58, 157) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 8 | 207 | OBJ_YADO (0x294) | (-449, 58, 252) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 9 | 210 | OBJ_YADO (0x294) | (-120, 269, 330) | 0/270/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 10 | 213 | OBJ_YADO (0x294) | (-120, 269, 675) | 0/270/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 11 | 216 | OBJ_YADO (0x294) | (210, 285, -30) | 0/180/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 12 | 219 | OBJ_HARIKO (0x20a) | (117, 70, 18) | 0/32/0 | all | - | 0x0000 | object_hariko | static-mesh | yes |
| 13 | 222 | EN_LIGHT (0x004) | (-330, -24, 76) | 0/0/0 | 0x2aa | - | 0x07f4 | gameplay_keep | billboard/effect | yes |
| 14 | 225 | EN_HIT_TAG \|0xe000 (0x265) | (314, 113, 180) | 0i/270i/0i | all | - | 0xfe00 | gameplay_keep | controller/invisible | yes |
| 15 | 228 | EN_INSECT (0x016) | (-277, 0, 241) | 0/107/0 | all | - | 0x0000 | gameplay_keep | posed-skeletal | yes |
| 16 | 231 | EN_IG (0x276) | (190, 0, 428) | 0/180/0 | 0x3c0 | - | 0x0004 | object_dai | npc-scheduled | yes |
| 17 | 234 | OBJ_TOKEIDAI (0x19c) | (34, 93, 231) | 0/90/0 | all | - | 0xa07f | object_obj_tokeidai | static-mesh | yes |
| 18 | 237 | EN_TEST4 (0x15a) | (-926, 629, 676) | 0/0/0 | all | - | 0x0100 | gameplay_keep | controller/invisible | yes |
| 19 | 240 | EN_AN (0x202) | (101, 0, 103) | 0/74/0 | 0x0ff | - | 0x0002 | object_an1 | npc-scheduled | yes |
| 20 | 243 | OBJ_SOUND (0x0f0) | (-328, 23, 73) | 0/0/0 | 0x3fb | - | 0x0091 | gameplay_keep | controller/invisible | yes |

### 4.4 r01 default (8 entries, `Z2_YADOYA_room_01ActorEntry_00006C` line 53)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 54 | EN_TALK \|0xe000 (0x261) | (-413, 33, -98) | 1i/98i/0i | 0x3c0 | - | 0xfe05 | gameplay_keep | controller/invisible | yes |
| 1 | 57 | EN_TALK \|0xe000 (0x261) | (-413, 33, -98) | 1i/98i/0i | 0x03f | - | 0xfe16 | gameplay_keep | controller/invisible | no |
| 2 | 60 | EN_TIME_TAG \|0xe000 (0x1ab) | (-334, 0, -264) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 3 | 63 | OBJ_HARIKO (0x20a) | (-203, 53, -12) | 0/243/0 | all | - | 0x0000 | object_hariko | static-mesh | yes |
| 4 | 66 | EN_LIGHT (0x004) | (-329, 0, -6) | 0/0/0 | 0x3f8 | - | 0x07f5 | gameplay_keep | billboard/effect | yes |
| 5 | 69 | EN_NB (0x243) | (-326, 0, -75) | 0/191/0 | 0x3f8 | 22 | 0x0000 | object_nb | npc-scheduled | yes |
| 6 | 72 | EN_TEST4 (0x15a) | (-926, 629, 677) | 0/0/0 | all | 11 | 0x0100 | gameplay_keep | controller/invisible | yes |
| 7 | 75 | EN_AN (0x202) | (-257, 0, -195) | 0/0/0 | 0x0fc | 21 | 0x0002 | object_an1 | npc-scheduled | yes |

### 4.5 r01 alt1 (9 entries, `Z2_YADOYA_room_01ActorEntry_0001D0` line 125)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 126 | EN_AN (0x202) | (-257, 0, -195) | 0/0/0 | 0x0fc | - | 0x0002 | object_an1 | npc-scheduled | yes |
| 1 | 129 | EN_GM (0x0a4) | (278, 0, 223) | 0/270/0 | 0x3df | - | 0x0003 | object_in2 (NOT LOADED) | npc-scheduled | yes |
| 2 | 132 | EN_PM (0x1d5) | (180, 0, 428) | 0/180/0 | all | - | 0x0001 | object_mm (NOT LOADED) | npc-scheduled | yes |
| 3 | 135 | EN_GURUGURU (0x248) | (-513, 210, 610) | 0/0/0 | 0x0a0 | - | 0x0002 | object_fu (NOT LOADED) | controller/invisible | yes |
| 4 | 138 | EN_TALK \|0xe000 (0x261) | (-413, 33, -98) | 1i/98i/0i | all | - | 0xfe05 | gameplay_keep | controller/invisible | yes |
| 5 | 141 | EN_TIME_TAG \|0xe000 (0x1ab) | (-334, 0, -264) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 6 | 144 | OBJ_HARIKO (0x20a) | (-203, 53, -12) | 0/243/0 | all | - | 0x0000 | object_hariko | static-mesh | yes |
| 7 | 147 | EN_LIGHT (0x004) | (-329, 0, -6) | 0/0/0 | 0x3f8 | - | 0x07f5 | gameplay_keep | billboard/effect | yes |
| 8 | 150 | EN_TEST4 (0x15a) | (-926, 629, 677) | 0/0/0 | all | - | 0x0100 | gameplay_keep | controller/invisible | yes |

### 4.6 r02 default (12 entries, `Z2_YADOYA_room_02ActorEntry_000070` line 55)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 56 | DM_CHAR05 (0x197) | (-468, 244, -53) | 0/0/0 | all | - | 0x0004 | object_dmask | controller/invisible | yes |
| 1 | 59 | EN_AH (0x253) | (-395, 210, -162) | 0/268/0 | 0x010 | - | 0x0000 | object_ah | npc-scheduled | no |
| 2 | 62 | EN_DRS (0x2a1) | (-450, 210, -208) | 0/0/0 | all | - | 0x0000 | object_drs | posed-skeletal | yes |
| 3 | 65 | EN_TIME_TAG \|0xe000 (0x1ab) | (-373, 210, -68) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 4 | 68 | OBJ_YADO (0x294) | (-600, 267, -26) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 5 | 71 | OBJ_YADO (0x294) | (-599, 267, -140) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 6 | 74 | DM_TAG (0x2a6) | (-452, 210, -35) | 0/0/0 | all | 24 | 0x0000 | gameplay_keep | controller/invisible | yes |
| 7 | 77 | EN_DS2N (0x1c3) | (-450, 210, -160) | 0/59/0 | 0x005 | - | 0x0000 | object_ds2n (NOT LOADED) | posed-skeletal | no |
| 8 | 80 | EN_TEST4 (0x15a) | (-926, 630, 736) | 0/0/0 | all | 12 | 0x0100 | gameplay_keep | controller/invisible | yes |
| 9 | 83 | EN_BOX \|0x6000 (0x006) | (-358, 210, 24) | 0i/180/0i | all | - | 0x50c1 | object_box | posed-skeletal | yes |
| 10 | 86 | EN_AN (0x202) | (-420, 210, -160) | 0/0/0 | all | - | 0x80ff | object_an1 | npc-scheduled | yes |
| 11 | 89 | EN_AN (0x202) | (-508, 210, -162) | 0/90/0 | 0x0ff | 21 | 0x0002 | object_an1 | npc-scheduled | yes |

### 4.7 r02 alt1 (7 entries, `Z2_YADOYA_room_02ActorEntry_0001F4` line 134)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 135 | DM_AN (0x29e) | (-509, 210, -162) | 0/91/0 | all | - | 0x0000 | object_an1 | npc-scheduled | yes |
| 1 | 138 | EN_TIME_TAG \|0xe000 (0x1ab) | (-373, 210, -68) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 2 | 141 | OBJ_YADO (0x294) | (-600, 267, -26) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 3 | 144 | OBJ_YADO (0x294) | (-599, 267, -140) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 4 | 147 | EN_BOX \|0x6000 (0x006) | (-358, 210, 24) | 0i/180/0i | all | - | 0x50c1 | object_box | posed-skeletal | yes |
| 5 | 150 | EN_TEST4 (0x15a) | (-926, 630, 736) | 0/0/0 | all | - | 0x0100 | gameplay_keep | controller/invisible | yes |
| 6 | 153 | EN_AN (0x202) | (-508, 210, -162) | 0/90/0 | 0x0ff | - | 0x00ff | object_an1 | npc-scheduled | yes |

### 4.8 r03 default (7 entries, `Z2_YADOYA_room_03ActorEntry_00006C` line 53)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 54 | EN_TIME_TAG \|0xe000 (0x1ab) | (-382, 210, 207) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 1 | 57 | OBJ_YADO (0x294) | (-599, 267, 184) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 2 | 60 | OBJ_YADO (0x294) | (-599, 267, 290) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 3 | 63 | EN_LIGHT (0x004) | (-451, 210, 93) | 0/0/0 | 0x170 | - | 0x07f5 | gameplay_keep | billboard/effect | no |
| 4 | 66 | DM_TAG (0x2a6) | (-346, 247, 92) | 0/0/0 | all | - | 0x0000 | gameplay_keep | controller/invisible | yes |
| 5 | 69 | EN_TEST4 (0x15a) | (-948, 649, 648) | 0/0/0 | all | 13 | 0x0100 | gameplay_keep | controller/invisible | yes |
| 6 | 72 | EN_BOX \|0x6000 (0x006) | (-452, 210, 326) | 0i/180/0i | all | - | 0x50c0 | object_box | posed-skeletal | yes |

### 4.9 r03 alt1 (6 entries, `Z2_YADOYA_room_03ActorEntry_0001C4` line 123)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 124 | EN_TIME_TAG \|0xe000 (0x1ab) | (-382, 210, 207) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 1 | 127 | OBJ_YADO (0x294) | (-599, 267, 184) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 2 | 130 | OBJ_YADO (0x294) | (-599, 267, 290) | 0/90/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 3 | 133 | EN_LIGHT (0x004) | (-451, 210, 93) | 0/0/0 | 0x170 | - | 0x07f5 | gameplay_keep | billboard/effect | no |
| 4 | 136 | EN_BOX \|0x6000 (0x006) | (-452, 210, 326) | 0i/180/0i | all | - | 0x50c0 | object_box | posed-skeletal | yes |
| 5 | 139 | EN_TEST4 (0x15a) | (-948, 649, 648) | 0/0/0 | all | - | 0x0100 | gameplay_keep | controller/invisible | yes |

### 4.10 r04 default (14 entries, `Z2_YADOYA_room_04ActorEntry_00006C` line 53)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 54 | EN_LIGHT (0x004) | (-491, 261, 534) | 0/0/0 | 0x3f8 | - | 0x83f0 | gameplay_keep | billboard/effect | yes |
| 1 | 57 | EN_GM (0x0a4) | (-334, 225, 903) | 0/180/0 | 0x020 | - | 0x00ff | object_in2 | npc-scheduled | no |
| 2 | 60 | EN_JA (0x244) | (-539, 214, 581) | 0/99/0 | 0x050 | - | 0x0001 | object_boj | npc-scheduled | no |
| 3 | 63 | EN_JA (0x244) | (-471, 214, 495) | 0/301/0 | 0x050 | - | 0x0000 | object_boj | npc-scheduled | no |
| 4 | 66 | OBJ_KINOKO (0x23b) | (-336, 225, 899) | 0/0/0 | all | - | 0x0005 | gameplay_keep | controller/invisible | yes |
| 5 | 69 | EN_RZ (0x27b) | (-544, 225, 690) | 0/90/0 | 0x060 | - | 0x7e01 | object_rz | npc-scheduled | no |
| 6 | 72 | EN_RZ (0x27b) | (-544, 225, 723) | 0/90/0 | 0x060 | - | 0xfe01 | object_rz | npc-scheduled | no |
| 7 | 75 | EN_GURUGURU (0x248) | (-533, 210, 600) | 0/53/0 | 0x0a0 | - | 0x0000 | object_fu | npc-scheduled | yes |
| 8 | 78 | EN_TIME_TAG \|0xe000 (0x1ab) | (-361, 210, 593) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 9 | 81 | OBJ_YADO (0x294) | (-486, 296, 959) | 0/180/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 10 | 84 | OBJ_YADO (0x294) | (-414, 297, 959) | 0/180/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 11 | 87 | EN_INSECT (0x016) | (-480, 210, 925) | 0/180/0 | all | - | 0x0000 | gameplay_keep | posed-skeletal | yes |
| 12 | 90 | EN_TEST4 (0x15a) | (-925, 632, 737) | 0/0/0 | all | 14 | 0x0100 | gameplay_keep | controller/invisible | yes |
| 13 | 93 | EN_RZ (0x27b) | (-465, 210, 684) | 0/0/0 | 0x0a0 | - | 0x8000 | object_rz | npc-scheduled | yes |

### 4.11 r04 alt1 (12 entries, `Z2_YADOYA_room_04ActorEntry_000230` line 143)

| # | L | Actor (id) | Position | Rot X/Y/Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 144 | EN_LIGHT (0x004) | (-491, 261, 534) | 0/0/0 | 0x3f8 | - | 0x83f0 | gameplay_keep | billboard/effect | yes |
| 1 | 147 | EN_GM (0x0a4) | (-334, 225, 903) | 0/180/0 | 0x020 | - | 0x00ff | object_in2 (NOT LOADED) | npc-scheduled | no |
| 2 | 150 | EN_JA (0x244) | (-539, 214, 581) | 0/99/0 | 0x050 | - | 0x0001 | object_boj (NOT LOADED) | npc-scheduled | no |
| 3 | 153 | EN_JA (0x244) | (-471, 214, 495) | 0/301/0 | 0x050 | - | 0x0000 | object_boj (NOT LOADED) | npc-scheduled | no |
| 4 | 156 | OBJ_KINOKO (0x23b) | (-336, 225, 899) | 0/0/0 | all | - | 0x0005 | gameplay_keep | controller/invisible | yes |
| 5 | 159 | EN_RZ (0x27b) | (-534, 210, 690) | 0/90/0 | 0x060 | - | 0x7e01 | object_rz | npc-scheduled | no |
| 6 | 162 | EN_RZ (0x27b) | (-465, 210, 684) | 0/0/0 | 0x0a0 | - | 0x8000 | object_rz | npc-scheduled | yes |
| 7 | 165 | EN_RZ (0x27b) | (-534, 210, 723) | 0/90/0 | 0x060 | - | 0xfe01 | object_rz | npc-scheduled | no |
| 8 | 168 | EN_TIME_TAG \|0xe000 (0x1ab) | (-361, 210, 593) | 20i/30i/0i | 0x3f0 | - | 0x0040 | gameplay_keep | controller/invisible | yes |
| 9 | 171 | OBJ_YADO (0x294) | (-486, 296, 959) | 0/180/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 10 | 174 | OBJ_YADO (0x294) | (-414, 297, 959) | 0/180/0 | all | - | 0x0002 | object_yado_obj | static-mesh | yes |
| 11 | 177 | EN_TEST4 (0x15a) | (-925, 632, 737) | 0/0/0 | all | - | 0x0100 | gameplay_keep | controller/invisible | yes |

### 4.12 Overlay findings per actor type (OVL)

| Actor | Init / params | Presence gating (Day 1 12:00, fresh save unless stated) |
| --- | --- | --- |
| En_An (Anju) | flex `gAnju1Skel`, scale 0.01; path index `p & 0xFF` (= 2); `ENAN_8000` variant only after `WEEKEVENTREG_COUPLES_MASK_CUTSCENE_FINISHED` (51_40), ordinary one only before; an ordinary Anju kills itself if another live ordinary Anju exists, else sets `room = -1` (an.c:3378-3411, helper :931-953) | schedule (an.c `scheduleScripts.schl:1-67`): Day 1 12:00-12:15 `ANJU_SCH_GIVE_LUNCH_TO_GRANNY`, placed at the last point of path chain step 4 (path 12, (-429, 0, -128), yaw 332) with `ENAN_ANIM_WAIT_WITH_TRAY` and the tray drawn, and only if `En_Nb` is loaded (an.c:2325-2353, limits :164, chain `src/code/z_sub_s.c:485-499`); drawn only while the schedule result is not none (an.c:3501-3503) |
| En_Nb (Granny) | flex `gNbSkel`, scale 0.01; stays at her entry position | `.schl:19-29`: Day 1 08:00-12:00 SCH_1, 12:00-12:15 SCH_2 (needs `En_An` loaded, z_en_nb.c:679-689), then SCH_1; mask excludes D3n |
| En_Bjt (toilet hand) | appears/vanishes, scale to 0.017 | `.schl:1-11`: 00:00-06:00 only, mask D1n+D2n+D3n: absent |
| En_Ah (Anju's mother) | | `.schl:1-29`: nothing in the Inn on Day 1; masks D2d+D3d (r00), D2n (r02): absent |
| En_Gm (Gorman) | object IN2 | Day 1 Inn 06:00-09:15 only (`.schl:3-12`); r04 entry D2d: absent at noon |
| En_Pm (Postman) | | Day 1 12:00-13:01 in the Post Office; Inn 13:56-14:30 (`.schl:66-125`): absent |
| En_Ig (Link the Goron) | | Day 1 Inn 15:45-16:55 (`.schl:23-44`, branches on `WEEKEVENTREG_RECEIVED_ROOM_KEY`): absent |
| En_Test3 (Kafei) | | mask D3d+D3n; Inn only Day 3 04:10-06:00 (`.schl:91-101`): absent |
| En_Ja (jugglers) | object BOJ | night only in the Inn (`.schl:1-17`), mask D1n+D2n: absent |
| En_Rz (Rosa sisters) | sister `p & 0x8000` (0 Judo, 1 Marilla), type `p & 0xF`, path `(p & 0x7E00) >> 9` (`z_en_rz.h:7-11`); type 0 walks a path and dies if `WEEKEVENTREG_77_04`; type 1 sits, needs 77_04 (z_en_rz.c:118-153) | r00 a24 Judo walks path 5 (lobby, ramp, corridor loop, scene.c:580-605); r04 a13 Marilla walks path 0 (Large Suite); sitting entries excluded by mask and flag |
| En_Guruguru | params 0 day form, 1 night form, 2 invisible day audio source (`draw = NULL`, plays `NA_BGM_SONG_OF_STORMS` sub-BGM at its position while the player is within 100 u in Y) (z_en_guruguru.c:94-112, :336-357) | r04 a07 (p 0) visible in the Large Suite by day; r00 a04 (p 2, same suite coordinates) is the room_00 audio stand-in, so the music is heard from the corridor without a second visible Guru-Guru |
| En_Test4 | static singleton: writes the per-scene day/night cutscene list from its own csId before the singleton check, then dies if one exists, else `room = -1` (z_en_test4.c:322-343) | every room carries one; csIds 10-14 select per-room transition cameras cs cam 0-4 (cutscene entries 10-14, scene.c:1780-1784); the latest room's csId wins |
| En_Time_Tag | type 0 kickout, text `(p & 0x1FE0) >> 5` = 2, exit `p & 0x1F` = 0, fires at home rot X:Y = 20:30 (`.h:10-15`, `.c:315-338`) | one per room, masks D0d..D2n; no-op in `SCENE_YADOYA` with the Room Key |
| En_Light | `p < 0`: small flame `gameplay_keep_DL_01ACF0`, light radius 100; else `gEffFire1DL` 0x7D590, radius 300; color/scale row `p & 0xF` (z_en_light.c:36-82, :161-199) | `0x83F0` candles (r00 a00/a01, r04 a00); `0x07F4` hearth (r00 a13, daytime halves only); `0x07F5` fires (r01 a04; r03 a03 night/Day 2) |
| Obj_Yado ("2nd Floor Window") | ICHAIN scale 0.1; opaque `object_yado_obj_DL_000430` + XLU `DL_000320`, animated material `Matanimheader_0012E8`; day/night prim colors from `isNight` (z_obj_yado.c:30-71) | always |
| Obj_Hariko ("Little Cow Statue Head") | scale 0.1; `gHarikoBodyDL` 0x80 + `gHarikoFaceDL` 0x110; head bobs during quakes | always |
| Obj_Tokeidai `0xA07F` | type 10 `SMALL_WALL_CLOCK`, scale 0.01, tells time (`z_obj_tokeidai.h:9-19`, `.c:254-259`); DLs minute ring 0xCF28, center/hand 0xBEE8, `gWallClockClockFaceDL` 0xF518, sun/moon panel 0xC368 (`.c:817-845`) | always; noon pose as South's tower clock rules (ROM_REFERENCE) |
| Obj_Kinoko | magic mushroom; drawn and collectible only while the Mask of Scents is worn; collectible flag `p & 0x7F` (6 r00, 5 r04) (z_obj_kinoko.c:29-62) | no geometry without the mask |
| En_Hit_Tag | invisible; on hit drops 3 green rupees; switch flag `(p & 0xFE00) >> 9` = 0x7F | always, no geometry |
| Obj_Sound `0x0091` | type 0 SFX, flag 1, sfx index 17 boiling water | always (mask excludes D3n) |
| En_Talk | talk spot, text `(p & 0x3F) + 0x1C00`: 0x1C05 (D0d..D1n), 0x1C16 (D2d..D4n) | Granny's room; contents NI |
| En_Fish `p 1` / En_Insect `p 0` | gameplay_keep fish in the room_00 basin (water box) / free bug | always |
| En_Drs | needs `OBJECT_MSMO` loaded (yes in r02), then flex `gWeddingDressMannequinSkel` idle | always (room_02) |
| Dm_Char05 `p 4` | Couple's Mask item draw (`GID_MASK_COUPLE`) only after cutscene cue 564 (z_dm_char05.c:202-215, :764-777) | no draw |
| Dm_Tag | killed if `WEEKEVENTREG_85_04`; in room_02 survives only when `curSpawn == 4`; elsewhere a talk tag (z_dm_tag.c:265-295) | r02 a06 killed; r03 a04 talk tag (section 5.4) |
| En_Box | type `(p >> 12) & 0xF` = 5 small, item `(p >> 5) & 0x7F` = 0x06 `GI_RUPEE_SILVER`, chest flag `p & 0x1F` (`z_en_box.h:39-41`; `include/z64item.h:421`) | both chests present, closed |
| En_Ds2n | "Potion Shop Owner from OoT (unused)" (ANN); `OBJECT_DS2N` 0x1AA is in no Inn object list | never spawns |
| En_Mag / Dm_An | title logo / cutscene Anju | alt1 only |

### 4.13 Counts

| | r00 d | r00 a1 | r01 d | r01 a1 | r02 d | r02 a1 | r03 d | r03 a1 | r04 d | r04 a1 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Entries (= LOCATION_REGISTER) | 26 | 21 | 8 | 9 | 12 | 7 | 7 | 6 | 14 | 12 |
| D1d by mask | 23 | 21 | 7 | 9 | 10 | 7 | 6 | 5 | 9 | 7 |
| static-mesh | 7 | 7 | 1 | 1 | 2 | 2 | 2 | 2 | 2 | 2 |
| posed-skeletal | 2 | 2 | 0 | 0 | 3 | 1 | 1 | 1 | 1 | 0 |
| npc-scheduled | 8 | 3 | 2 | 3 | 3 | 2 | 0 | 0 | 7 | 6 |
| billboard/effect | 3 | 3 | 1 | 1 | 0 | 0 | 1 | 1 | 1 | 1 |
| controller/invisible | 6 | 6 | 4 | 4 | 4 | 2 | 3 | 2 | 3 | 3 |

Totals: 67 default entries, 55 alt1, plus 6 transition actors per header. Object counts 18/16/18/16/16 and 9/8/9/9/8 also equal the register row.

## 5. Venue mechanics index (INT-07 scope)

### 5.1 Room access and door gating

| Door | Gate (OVL door.schl) | Item/flag | Message |
| --- | --- | --- | --- |
| Lower street door, from East (East TA0) | Days 1-2 closed outside 08:00-20:30; Day 3 closed only 06:00-08:00 | `SCHEDULE_CHECK_MISC_ROOM_KEY` = `INV_CONTENT(ITEM_ROOM_KEY) == ITEM_ROOM_KEY` opens it at any time (`src/code/z_schedule.c:106`, `include/z64schedule.h:55`) | `0x1800 + 0x13` |
| Upper deck door, from East (East TA1) | never | | |
| Granny's door TA0 / Large Suite TA3 | never | | |
| Staff room TA1 | open only on Day 3 from 18:00 | | `0x1814` "Only employees" (ANN) |
| Knife Chamber TA2 | Room Key in inventory | `ITEM_ROOM_KEY` | `0x1816` |

Script times are measured from 06:00 (`SCRIPT_CONVERT_TIME`, `include/z64scripts.h:7-17`), so "Day 3 before 08:00" is 06:00-08:00 and the final night (day 3, 00:00-06:00) counts as open. NPCs open schedule doors themselves through `openTimer` (door.c:476-479, :569-590; Anju's door results map to schedule types in an.c:959-995).

### 5.2 Open hours, kickout, Room Key and the roof/deck entry

- **Kickout (C/OVL):** one `En_Time_Tag 0x0040` per room (r00 a06, r01 a02, r02 a03, r03 a00, r04 a08; alt1 likewise) at 20:30 on D0d..D2n: halts the player, text `0x1885`, then `nextEntrance = setupExitList[0]` = `0xD290`, East spawn 9 (z_en_time_tag.c:284-338). Skipped while the Room Key is held in this scene (:319-322). No Day 3 kickout (mask).
- **Closing routine (OVL):** Anju walks to the front door 19:50-20:20 and waits 20:20-20:30, then walks to the staff room (an.c `.schl:40-47`).
- **Room Key / reservation (OVL):** `sAnjuMsgScript_ReceptionistDay1` (an.c:391-425), chosen on Day 1 when Anju is `ANJU_SCH_RECEPTIONIST_IDLE` and the player is not Deku and not wearing Kafei's Mask (an.c:1669-1786). The key branch runs only after 11:00 (`CHECK_BEFORE_TIME(11, 0)` skips when now <= 11:00, `src/code/z_msgevent.c:718-731`) and only while `WEEKEVENTREG_32_20`, `RECEIVED_ROOM_KEY` (75_10), `55_01` and `55_02` ("Link the Goron claims his reservation", ANN `include/z64save.h:1154-1155`) are clear; a text choice then offers `GI_ROOM_KEY` and sets `WEEKEVENTREG_RECEIVED_ROOM_KEY` and switch flag 0x41. The second choice (INF: declining) sets 55_01 instead (an.c:402, :433). Anju's desk windows on Day 1: 06:00-10:55, 13:50-14:08, 14:18-16:10, 16:30-19:50.
- **Roof/deck entry (C):** the upper door TA4 is always open from both sides and is the only source-backed walking link to the East upper deck (East L8, spawn 10); East has no foot link from the plaza (East 6.4). Route inside: spawn 0 -> lobby -> ramp polys 8/9 (scene.c:1953-1954, slope 41.2 deg, rising from x 210 / Y 0 to x -30 / Y 210 over z -150..-30) -> landing -> TA4. All six door thresholds, both exit floors, the ramp and every spawn floor are one floor-adjacency island (scratch `conn.py`, edges within 2 u, steps up to 30 u: INF, not a controller test). This answers East U-05.

### 5.3 Loading rooms without duplicated actors (C)

| Mechanism | Source |
| --- | --- |
| Two rooms are resident during a door pass: the room buffer is sized for front + back room of any transition actor; the old room and its actors stay until `Room_FinishRoomChange` | `z_room.c:515-527`, `:576-598`, `:655-668` |
| Actors with `room >= 0` not in the current/previous room are deleted on finish | `z_actor.c:3278-3300` |
| Transition actors spawn once (id negated) while either side's room is loaded | `z_actor.c:3600-3625`; door destroy restores the id (door.c:393-405) |
| Anju appears in all five default lists (r00 a19, r01 a07, r02 a10/a11) but only the first ordinary instance survives, with `room = -1`; the `0x80FF` copy exists only after the Couple's Mask cutscene | an.c:3378-3411 |
| `En_Test4` in every room; singleton via static `sIsLoaded`, `room = -1`; each spawn still updates the transition-cutscene list | z_en_test4.c:322-343 |
| Guru-Guru split: visible NPC only in room_04 (p 0); room_00 holds an invisible audio copy (p 2) | z_en_guruguru.c:94-112, :349-357 |
| Kickout tags, windows, lights, chests are room-local (no cross-room instance) | room lists |
| Banks of persistent NPCs survive room changes through the shared nine-bank prefix | `z_scene.c:277-290` (section 3) |
| Room-only interactions: Dm_Tag in room_02 only for spawn 4; Granny and Anju's lunch need both actors loaded at once | z_dm_tag.c:281-282; z_en_nb.c:679-689; an.c:2335-2353 |

Room geometry overlap (INF): each room's cullable list also draws volumes outside its footprint, e.g. room_00 entries at (-435, 300, -83/233/690) r 246/246/330, room_01 at (90, 86, -135) r 308, room_02 at (30, 195, -38) r 376, room_03 at (-195, 300, 225), room_04 at (-195, 300, 510) (HDR room shape lists). These look like through-door views; whether they duplicate geometry of the neighbouring room when both are resident is U-08.

### 5.4 Residents, schedules and city quest dependencies

| Thread | Inn actors and rooms | Flags / items / time (OVL unless marked) |
| --- | --- | --- |
| Anju's day | En_An, all rooms | Day 1 desk, 10:55 kitchen, 11:10-11:30 cooking, 11:50 Granny's door, 12:00-12:15 lunch, 12:55-13:20 staff room, 14:08-14:18 receives Kafei's letter from the Postman, 16:10-16:30 attends the Goron; Day 2 branches on `WEEKEVENTREG_HAD_MIDNIGHT_MEETING` (50_20) (Laundry Pool trip 11:03-17:55 otherwise); Day 3 sweeping the Large Suite 06:00-11:00 (`.schl:1-233`, enum an.c:30-160) |
| Midnight meeting | En_An, kitchen (room_00) | talking to Anju on Day 1 after 14:18 in Kafei's Mask runs `sAnjuMsgScript_80B58AC4`, which sets `WEEKEVENTREG_PROMISED_MIDNIGHT_MEETING` (50_08) (an.c:1723-1746, :716-726); Day 1 00:00-06:00 `ANJU_SCH_MIDNIGHT_MEETING` in the kitchen; the meeting script sets `HAD_MIDNIGHT_MEETING` (an.c:566-585) and ends with `ENTRANCE(STOCK_POT_INN, 3)` (an.c:1591-1600, :1749-1752) |
| Granny's stories | En_Nb, room_01 | the story callback shows a story only with the All-Night Mask worn, otherwise skips to the time jump; the jump sets 08:00 + `Sram_IncrementDay` unless `EVENTINF_42` (then +2 h) and reloads spawn 2 (z_en_nb.c:410-480); the quiz scripts check the mask and offer heart pieces gated by `WEEKEVENTREG_50_04` (long story) and `50_02` (short) (z_en_nb.c:70-160) |
| Toilet hand | En_Bjt, room_00 toilet | 00:00-06:00; takes Letter to Kafei, any title deed or Letter to Mama; first time `GI_HEART_PIECE` + `WEEKEVENTREG_73_08`, later `GI_RUPEE_BLUE`; sets `90_80` (z_en_bjt.c:32-76, :183-212) |
| Link the Goron | En_Ig, enters by the lower door | Day 1 15:45-16:55; schedule forks on `RECEIVED_ROOM_KEY`; 55_02 presumably his claim (ANN "Unconfirmed") |
| Eavesdrop | Dm_Tag r03 -> spawn 4 (room_02) -> spawn 5 | Day 2 21:30-23:00 (z_dm_tag.c:212-232, :119-210); Anju talks with her mother then (`ANJU_SCH_TALKING_WITH_MOM`) |
| Couple's Mask | En_Test3 (Kafei, Day 3 04:10-06:00, needs `WEEKEVENTREG_DELIVERED_PENDANT_OF_MEMORIES`), En_An `0x80FF`, Dm_Char05 | flag `COUPLES_MASK_CUTSCENE_FINISHED` 51_40 (set by Kafei, ANN `include/z64save.h:1120-1121`); Kafei csId 20 -> cutscene entry 20 (script 0) -> additional 23 (script 1) (scene.c:1790, :1793) |
| Large Suite residents | Guru-Guru (day), Rosa sisters (77_04 switches walking/sitting), jugglers (night), Gorman (Day 2 day) | as section 4.12 |
| Chests | En_Box r02 flag 1, r03 flag 0 | silver rupee each; behind the staff-room and Room-Key gates |

### 5.5 Cameras and lighting (C)

| Item | Value | Source |
| --- | --- | --- |
| Collision cameras | `BgCamInfo[0]` `0x0027` `CAM_SET_FIXED3` at (33, 60, -156), rot (4551, 5279) binang; `[1]` `0x0051` `CAM_SET_ROOM1`. Only surface 5 (toilet floor: polys 271/272 x 30..90, z -150..-30; 277/278 the seat at Y 6) uses bgCam 0; everything else bgCam 1 | scene.c:1922-1931; `include/z64camera.h:186, :228` |
| Actor-cutscene cameras | 22 entries; settings 31 `FIXED1` (19), 45 `START2` (cs cams 10, 11), 59 `CIRCLE8` (cs cam 6); each 3 x Vec3s (pos, rot, third vector NI) | scene.c:1819-1911; `z64camera.h:178, 192, 206` |
| Who uses them | spawns 0/1 -> 11/10; door sides 7-9, 12-16; En_Test4 csId 10-14 -> cs cam 0-4 (one per room: (191,70,259), (-375,80,-87), (-484,231,-155), (-416,260,185), (-495,224,732)); Anju csId 21 -> cs cam 5 (141, 40, 43) at the desk; Granny csId 22 -> cs cam 6; Dm_Tag csId 24 -> cs cams 17-21 (room_02) | scene.c:1769-1800 (`CutsceneEntry` fields `include/z64cutscene.h:626-637`) |
| Per-room light | the player's floor surface selects the env light setting (`player.c:11729-11730`, `z_bgcheck.c:4265-4267`): room_00 light 2, room_01 0, room_02 4, room_03 1, room_04 3 | surface types, section 7 |
| Point lights | `En_Light` flames add point lights (radius 100/300); room behavior enables positional lights for rooms 01-04 only | z_en_light.c:47-82; section 1 |
| Water | one water box x -600..-390, y -15, z 210..420, props `0x201` (bgCam 1, light 2, room 0); basin floor at Y -23 (polys 80/81, depth 8) with `En_Fish` | scene.c:4070-4072; macros `include/z64bgcheck.h:34-46` |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

The venue is **open** at 12:00 on Day 1: East TA0 `INN_MAIN_ENTRANCE` is open 08:00-20:30 (door.schl:224-228); arrival is spawn 0 in room_00. Rooms 01 and 04 are reachable (ungated doors); room_02 is closed until Day 3 18:00 and room_03 needs the Room Key (absent on a fresh save), but their static contents are listed so the venue can be assembled. Assumptions: fresh save (no week-event, switch, collectible or chest flags), Human form, no Room Key, no mask worn.

| Room / # | Component, params | N64 position (Unity) | Yaw | Scale | Display lists (bank, offset) | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| all rooms | room geometry | | | 1 | room cullable DL lists (section 8) | five rooms, each its own room file |
| r00 TA5, TA4 | street doors `0x028B`, `0x028C` | (180, 0, 360) (1.80, 0.00, -3.60); (-60, 210, 0) (-0.60, 2.10, 0.00) | 0 | 0.01 (actor default, INF) | `gInnSchoolDoorDL` object_wdor03 0x548 on `gDoorSkel` limb 4 | closed pose |
| r00/r01-r04 TA0-3 | room doors `0x028D-0x0290` | section 2.4 | 90 | 0.01 (INF) | `gDoorLeftDL` / `gDoorRightDL` gameplay_keep 0x20BB8 / 0x20D00 | fallback art (no field keep) |
| r00 7-11 | windows `0x0002` | (-449,58,157), (-449,58,252), (-120,269,330), (-120,269,675), (210,285,-30) | 90, 90, 270, 270, 180 | 0.1 | object_yado_obj 0x430 opa + 0x320 xlu; matanim 0x12E8 | day prim colors |
| r00 12 | cow statue head `0x0000` | (117, 70, 18) (1.17, 0.70, -0.18) | 32 | 0.1 | object_hariko 0x80 + 0x110 | |
| r00 17 | small wall clock `0xA07F` | (34, 93, 231) (0.34, 0.93, -2.31) | 90 | 0.01 | object_obj_tokeidai 0xCF28, 0xBEE8, 0xF518, 0xC368 | hands at 12:00 (ROM_REFERENCE convention) |
| r00 0, 1, 13 | flames `0x83F0` x2, hearth `0x07F4` | (-112,100,15), (278,100,-292), (-330,-24,76) | camera-facing | 0.0075 / 0.004 | gameplay_keep 0x1ACF0 / `gEffFire1DL` 0x7D590 (XLU) | billboard approximation; point lights |
| r01 3 | cow statue head | (-203, 53, -12) | 243 | 0.1 | object_hariko | |
| r01 4 | fire `0x07F5` | (-329, 0, -6) | camera-facing | 0.0075 | `gEffFire1DL` | billboard |
| r02 4, 5 / r03 1, 2 / r04 9, 10 | windows | (-600,267,-26), (-599,267,-140) / (-599,267,184), (-599,267,290) / (-486,296,959), (-414,297,959) | 90 / 90 / 180 | 0.1 | object_yado_obj | |
| r02 9, r03 6 | small chests `0x50C1`, `0x50C0` | (-358, 210, 24), (-452, 210, 326) | 180 | NI | object_box (skeletal, closed) | chest flags 1 / 0 clear |
| r04 0 | candle `0x83F0` | (-491, 261, 534) | camera-facing | 0.0075 | gameplay_keep 0x1ACF0 | |

**Present at noon but needing skeleton/pose work:** r01 Anju (persistent instance) at (-429, 0, -128) yaw 332 in the lunch state, only when room_01 is loaded; r01 a05 Granny (-326, 0, -75) yaw 191 (SCH_2); r00 a24 Judo on path 5 and r04 a13 Marilla on path 0 (positions depend on walk progress); r04 a07 Guru-Guru (-533, 210, 600) yaw 53; r02 a02 dress mannequin; r00 a05 fish, r00 a15 and r04 a11 insects.

**Excluded by half-day mask:** r00 a02 En_Ah, a18 En_Bjt, a21 En_Test3; r01 a01 En_Talk; r02 a01 En_Ah, a07 En_Ds2n; r03 a03 En_Light; r04 a01 En_Gm, a02/a03 En_Ja, a05/a06 En_Rz (sitting).

**Excluded by schedule, flag or duplicate rule:** r00 a19 Anju (not drawn in room_00 at 12:00; her lunch position is in room_01), a20 Gorman, a22 Postman, a23 Goron; r01 a07 and r02 a11 (duplicate Anju), r02 a10 (`0x80FF`, needs 51_40), r02 a06 Dm_Tag, r02 a00 Dm_Char05 (no cue).

**No geometry:** kickout tags, En_Test4 x5, En_Hit_Tag, Obj_Sound, En_Talk, r03 a04 Dm_Tag, Obj_Kinoko x2 (Mask of Scents only), r00 a04 Guru-Guru audio copy.

## 7. Collision summary (C from scene.c:1922-4082; scratch `col.py`, `col2.py`, `r0floors.py`, `conn.py`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 975 / 1145 / 29 (`Z2_YADOYACollisionHeader_00A018`, shared by both headers) |
| Header bounds | min (-605, -30, -300), max (337, 390, 960); vertex extents (-602, -30, -300)..(330, 390, 960); Unity x -6.02..3.30, y -0.30..3.90, z -9.60..3.00 |
| Kinds | 282 floors (276 under 10 deg, ramp 8/9 at 41.2 deg, 4 polys at 13 deg on a room_01 prop), 791 walls (781 near vertical), 72 ceilings |
| Materials (polys) | stone 664, wood 445, carpet 36 (`SURFACE_MATERIAL_*`, `include/z64bgcheck.h:232-246`) |
| Exits | surfaces 3 and 8, 4 floor polys (section 2.3) |
| Wall types | type 1 (`WALL_FLAG_0`, no ledge climb, player.c:11835): 42 polys (toilet partition, reception, room_01 furniture, room_02 shelf, room_04 x -569); type 2 (`FLAG_0 + FLAG_1`) 8 polys at x -531/-371 and type 3 (`FLAG_0 + FLAG_2`) 8 polys at x -535/-367, z 741..941: bunk-bed ends that enter the climb branch (player.c:9612-9617; `z_bgcheck.c:18-26`) |
| Echo | 2 on most surfaces, 3 on corridor surface 7, 4 on room_04 surfaces 20, 22, 23, 25, 27 |
| Other flags | no conveyor, hookshot, floor-type, floor-property or soft flags; xp 1 (ignore camera) on 546 polys incl. both exit floors; no void-out surface |

| Room (light idx) | Polys F/W/C | Surface types | Floor levels (centroid Y) | Notable |
| --- | --- | --- | --- | --- |
| room_00 (2) | 354: 85/239/30 | 0-8 | -23 basin, -8 kitchen floor (x -450..-360, z 75..330), 0 lobby (x -390..330, z -300..640 incl. exit), 6 toilet seat, 15 (x 285..315, z 120..270), 30/60/90 furniture and counter, 70/140 ramp, 210 corridor x -270..0, z -150..885 | water box; toilet camera; both exits; ramp |
| room_01 (0) | 109: 31/74/4 | 9-11 | 0 floor x -480..-150, z -300..-15; 20-47 furniture | wall type 1 furniture block x -448..-365 |
| room_02 (4) | 128: 36/88/4 | 12-15 | 210 floor x -600..-270, z -240..60; 230-254 furniture; 312 shelf | layout mirrors room_03 |
| room_03 (1) | 122: 34/84/4 | 16-18 | 210 floor z 90..390; 230-254 furniture | |
| room_04 (3) | 432: 96/306/30 | 19-28 | 210 floor z 420..960; 225 lower bunks, 300 upper bunks, 316-329 bunk frames (four 14-poly islands), 230-254 furniture | climbable bunk ends |

Floor connectivity (INF, `conn.py`): one 178-poly island holds every walkable room floor, threshold, ramp and exit; the four upper-bunk islands and nine furniture tops are separate. The Inn interior is its own source frame: TA4 - TA5 = (-240, 210, -360) in the Inn, but East TA1 - TA0 = (-191, 180, -425), so the interior is not a rigid copy of the East facade (C arithmetic; U-10).

## 8. Materials and textures (C; counts only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted PNGs in the scene folder | 43: scene file 15 (ci8 7, i8 1, rgba16 2, rgba16 TLUT 5); room_00 17 (ci8 13, rgba16 3, rgba32 1); room_01 5 ci8; room_02 1 ci8; room_03 1 ci8; room_04 4 (ci8 2, rgba16 2) |
| Shared external textures | `scene_texture_01` (XML dependency): 30 distinct textures referenced across the five rooms (r00 29, r01 23, r02 19, r03 18, r04 9); not extracted (baserom file present) |
| Room Gfx / Vtx arrays / source triangles | r00 85 / 48 / 1602; r01 24 / 14 / 569; r02 18 / 9 / 470; r03 16 / 9 / 382; r04 16 / 9 / 684 (DL reuse not resolved) |
| Cullable entries (opa / xlu) | r00 12 (12/7), r01 8 (8/2), r02 6 (6/1), r03 8 (8/2), r04 8 (8/2) |
| Texture loads in room_00 | 190 `gsDPLoadTextureBlock`, 168 `gsDPLoadTLUT_pal`, 1 `gsDPLoadMultiBlock` (the animated water) |
| Animated material | 1 (section 1) |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_YADOYA` C; `object_obj_tokeidai` (South). Binaries for everything below exist under `extracted/n64-us/baserom/` (sizes checked by `mats.py`); all XMLs exist.

| Priority | Inputs | Why |
| --- | --- | --- |
| Day 1 static | `misc/scene_texture_01`, `object_yado_obj`, `object_hariko`, `object_wdor03`, `object_box`, `gameplay_keep` | room textures, windows, cow heads, street doors, chests, room doors and flames |
| Later skeletal pass | `object_an1`, `object_nb`, `object_rz`, `object_fu`, `object_drs`, `object_msmo` | noon NPCs and mannequin |
| Other states | `object_an2`, `object_an3`, `object_an4`, `object_ah`, `object_in2`, `object_dai`, `object_mm`, `object_bjt`, `object_boj`, `object_test3`, `object_dmask`, `object_gi_mask13` | night, Day 2/3 and quest scenes |

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
$cfg = @('-gsf','1','-rconf','tools/ZAPDConfigs/MM/Config.xml','--cs-float','both','-Wno-hardcoded-pointer')
& $zapd e -i 'assets/xml/misc/scene_texture_01.xml' -b extracted/n64-us/baserom -o 'extracted/n64-us/assets/misc/scene_texture_01' -osf 'extracted/n64-us/assets/misc/scene_texture_01' @cfg
foreach ($bank in @('object_yado_obj','object_hariko','object_wdor03','object_box','gameplay_keep')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" @cfg
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

Run from `local-reference/mm-decomp` as in `tools/reference/README.md`. `gameplay_keep` (599,520 bytes) has not been extracted with this tool before (NI, as North notes).

## 10. Proposed region record

**Proposal, schema not frozen (PIPE-01).** Keys follow North section 9; `rooms` gains a `role` field and `components` include doors because this is the multi-room pilot.

```json
{
  "_status": "proposal, schema not frozen (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.stock_pot_inn",
  "displayLabel": "Stock Pot Inn",
  "source": {
    "sceneSymbol": "Z2_YADOYA", "sceneEnum": "SCENE_YADOYA", "sceneTableIndex": "0x61", "entranceTableSceneIndex": "0x5E",
    "baseline": "N64 US rev 0", "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd", "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0)",
    "rooms": {
      "loc.stock_pot_inn.room_00": "default header", "loc.stock_pot_inn.room_01": "default header",
      "loc.stock_pot_inn.room_02": "default header", "loc.stock_pot_inn.room_03": "default header",
      "loc.stock_pot_inn.room_04": "default header"
    },
    "halfDayBit": "0x080",
    "stateAssumptions": [
      "fresh save: no week-event, switch, collectible or chest flags", "player form Human", "no Room Key", "no mask worn",
      "arrival spawn 0 through East TA0 (open 08:00-20:30)"
    ]
  },
  "localSourceInputs": [
    {"path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_YADOYA", "bytes": 62592, "sha256": "b49451b01030f336070f8cbe78768115c0b61410e50e3f86c4c42107cf968777"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_YADOYA_room_00", "bytes": 99520, "sha256": "f5caef9f074b855c3fb02d44aff5f9c976bbeacf9bc56b6e5f860ff4b735cd9f"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_YADOYA_room_01", "bytes": 33584, "sha256": "9d5e0bec9c69696144cba14e0f0f12696727ef35e1bd59a275e51ba104d3865d"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_YADOYA_room_02", "bytes": 24288, "sha256": "bd845f1f3aceb068be7faf0d79767f88cdd02845875ce56bd6289ddb3524b6b9"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_YADOYA_room_03", "bytes": 20672, "sha256": "12aec8621ba7b2cec2a3278b6af61b3a136537c5186213b92ca6420478bc4235"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_YADOYA_room_04", "bytes": 30000, "sha256": "1e9851105ce43d7b997a38a767db24701f4921cdde62b8ed93a6ef068d271830"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_YADOYA/Z2_YADOYA.c", "sha256": "4b8b2c5da23479a2277de0cdcd318ce52de457e2c51d4e624919a07662c0e368"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_YADOYA/Z2_YADOYA_room_00.c", "sha256": "9c3469fa3b53ae6f32db8392c52387335efd05a7436d28b97f0101e76e30d43a"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_YADOYA/Z2_YADOYA_room_01.c", "sha256": "8631e38be29e8646144418fb1f77703ac50f705e78daff8297eabf466f9edda0"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_YADOYA/Z2_YADOYA_room_02.c", "sha256": "cc5b66bd6149778c3e4ddec33ba5a55eda557946c65a07f218fed4194ab74fad"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_YADOYA/Z2_YADOYA_room_03.c", "sha256": "26606bfc51a91a1f313bb5493f8e52163491dea04e9bc3d0cdf796d82d859f8b"},
    {"path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_YADOYA/Z2_YADOYA_room_04.c", "sha256": "4bcc5fbbb2c63389b7b550c3972417368d39161bf85d2fe3142691232a8629c1"},
    {"path": "local-reference/mm-decomp/assets/xml/scenes/Z2_YADOYA/Z2_YADOYA.xml", "sha256": "e7c7e0c1ccabc34ff42a32f1cba7132a76aa8a0d61f5e25ec20d57f4ed77703a"}
  ],
  "sourceFrame": {"units": "N64 world units", "up": "+Y", "sharedWithEast": "no: interior frame, door offsets differ from the East facade (U-10)"},
  "conversionToUnity": {"matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"},
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": {"min": [-605, -30, -300], "max": [337, 390, 960]},
    "sourceCollisionVertices": {"min": [-602, -30, -300], "max": [330, 390, 960]},
    "unityCollisionVertices": {"min": [-6.02, -0.3, -9.6], "max": [3.3, 3.9, 3.0]}
  },
  "anchors": [
    {"id": "anchor.spi.spawn0_lower_door", "source": [149, 0, 303], "unity": [1.49, 0.0, -3.03], "status": "proposed, not surveyed"},
    {"id": "anchor.spi.spawn1_upper_door", "source": [-63, 210, -46], "unity": [-0.63, 2.1, 0.46], "status": "proposed, not surveyed"},
    {"id": "anchor.spi.door_lower_ta5", "source": [180, 0, 360], "unity": [1.8, 0.0, -3.6], "status": "proposed, not surveyed"},
    {"id": "anchor.spi.door_upper_ta4", "source": [-60, 210, 0], "unity": [-0.6, 2.1, 0.0], "status": "proposed, not surveyed"},
    {"id": "anchor.spi.ramp_foot", "source": [210, 0, -90], "unity": [2.1, 0.0, 0.9], "status": "proposed, not surveyed"},
    {"id": "anchor.spi.ramp_top", "source": [-30, 210, -90], "unity": [-0.3, 2.1, 0.9], "status": "proposed, not surveyed"},
    {"id": "anchor.spi.wall_clock", "source": [34, 93, 231], "unity": [0.34, 0.93, -2.31], "status": "proposed, not surveyed"}
  ],
  "rooms": [
    {"id": "loc.stock_pot_inn.room_00", "sourceRoom": "Z2_YADOYA_room_00", "roomShape": "cullable, 12 entries", "displayLists": 85, "role": "lobby, reception, kitchen, toilet, ramp stair, upper corridor (INF)", "collisionLightIndex": 2},
    {"id": "loc.stock_pot_inn.room_01", "sourceRoom": "Z2_YADOYA_room_01", "roomShape": "cullable, 8 entries", "displayLists": 24, "role": "Granny's room", "collisionLightIndex": 0},
    {"id": "loc.stock_pot_inn.room_02", "sourceRoom": "Z2_YADOYA_room_02", "roomShape": "cullable, 6 entries", "displayLists": 18, "role": "staff room (Anju)", "collisionLightIndex": 4},
    {"id": "loc.stock_pot_inn.room_03", "sourceRoom": "Z2_YADOYA_room_03", "roomShape": "cullable, 8 entries", "displayLists": 16, "role": "Knife Chamber", "collisionLightIndex": 1},
    {"id": "loc.stock_pot_inn.room_04", "sourceRoom": "Z2_YADOYA_room_04", "roomShape": "cullable, 8 entries", "displayLists": 16, "role": "Large Suite", "collisionLightIndex": 3}
  ],
  "components": [
    {"sourceKey": "Z2_YADOYA/h0/ta5", "actorId": "0x005", "params": "0x028B", "class": "door/transition", "dls": ["object_wdor03:0x548"], "status": "selected, closed pose"},
    {"sourceKey": "Z2_YADOYA/h0/ta4", "actorId": "0x005", "params": "0x028C", "class": "door/transition", "dls": ["object_wdor03:0x548"], "status": "selected, closed pose"},
    {"sourceKey": "Z2_YADOYA/h0/ta0", "actorId": "0x005", "params": "0x028D", "class": "door/transition", "dls": ["gameplay_keep:0x20BB8", "gameplay_keep:0x20D00"], "status": "selected, fallback art"},
    {"sourceKey": "Z2_YADOYA/h0/ta1", "actorId": "0x005", "params": "0x028E", "class": "door/transition", "dls": ["gameplay_keep:0x20BB8", "gameplay_keep:0x20D00"], "status": "selected, fallback art"},
    {"sourceKey": "Z2_YADOYA/h0/ta2", "actorId": "0x005", "params": "0x028F", "class": "door/transition", "dls": ["gameplay_keep:0x20BB8", "gameplay_keep:0x20D00"], "status": "selected, fallback art"},
    {"sourceKey": "Z2_YADOYA/h0/ta3", "actorId": "0x005", "params": "0x0290", "class": "door/transition", "dls": ["gameplay_keep:0x20BB8", "gameplay_keep:0x20D00"], "status": "selected, fallback art"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a07", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a08", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a09", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a10", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a11", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a12", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80", "object_hariko:0x110"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a17", "actorId": "0x19C", "params": "0xA07F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected, noon pose"},
    {"sourceKey": "Z2_YADOYA/h0/room_01/a03", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80", "object_hariko:0x110"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_02/a04", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_02/a05", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_02/a09", "actorId": "0x006", "params": "0x50C1", "class": "posed-skeletal", "dls": ["object_box:NI"], "status": "selected, closed chest, skeleton pending"},
    {"sourceKey": "Z2_YADOYA/h0/room_03/a01", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_03/a02", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_03/a06", "actorId": "0x006", "params": "0x50C0", "class": "posed-skeletal", "dls": ["object_box:NI"], "status": "selected, closed chest, skeleton pending"},
    {"sourceKey": "Z2_YADOYA/h0/room_04/a09", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_04/a10", "actorId": "0x294", "params": "0x0002", "class": "static-mesh", "dls": ["object_yado_obj:0x430", "object_yado_obj:0x320"], "status": "selected"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a00", "actorId": "0x004", "params": "0x83F0", "class": "billboard/effect", "dls": ["gameplay_keep:0x1ACF0"], "status": "optional, billboard approximation"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a01", "actorId": "0x004", "params": "0x83F0", "class": "billboard/effect", "dls": ["gameplay_keep:0x1ACF0"], "status": "optional, billboard approximation"},
    {"sourceKey": "Z2_YADOYA/h0/room_00/a13", "actorId": "0x004", "params": "0x07F4", "class": "billboard/effect", "dls": ["gameplay_keep:0x7D590"], "status": "optional, billboard approximation"},
    {"sourceKey": "Z2_YADOYA/h0/room_01/a04", "actorId": "0x004", "params": "0x07F5", "class": "billboard/effect", "dls": ["gameplay_keep:0x7D590"], "status": "optional, billboard approximation"},
    {"sourceKey": "Z2_YADOYA/h0/room_04/a00", "actorId": "0x004", "params": "0x83F0", "class": "billboard/effect", "dls": ["gameplay_keep:0x1ACF0"], "status": "optional, billboard approximation"}
  ],
  "collisionOwnership": {
    "room": "Z2_YADOYACollisionHeader_00A018 (1145 polys, one header for all five rooms; per-room split by surface light index)",
    "actorDynapoly": [],
    "actorCylinders": "NI",
    "waterBoxes": ["x -600..-390, y -15, z 210..420, room 0"]
  },
  "portalIds": [
    "portal.stock_pot_inn.east_clock_town.lower", "portal.stock_pot_inn.east_clock_town.upper",
    "portal.stock_pot_inn.east_clock_town.kickout", "portal.stock_pot_inn.stock_pot_inn.r0_r1", "portal.stock_pot_inn.stock_pot_inn.r1_r0",
    "portal.stock_pot_inn.stock_pot_inn.r0_r2", "portal.stock_pot_inn.stock_pot_inn.r2_r0", "portal.stock_pot_inn.stock_pot_inn.r0_r3",
    "portal.stock_pot_inn.stock_pot_inn.r3_r0", "portal.stock_pot_inn.stock_pot_inn.r0_r4", "portal.stock_pot_inn.stock_pot_inn.r4_r0"
  ],
  "materialBindings": null,
  "cameraIds": [
    "cam.stock_pot_inn.entry", "cam.stock_pot_inn.reverse_door", "cam.stock_pot_inn.fixture", "cam.stock_pot_inn.room_01",
    "cam.stock_pot_inn.room_02", "cam.stock_pot_inn.room_03", "cam.stock_pot_inn.room_04"
  ],
  "cameraSourceHints": {
    "entry": "cs cam 11 START2 (251, 38, 306)", "fixture": "cs cam 5 FIXED1 (141, 40, 43), Anju csId 21, reception desk",
    "room_01": "cs cam 16 (-412, 100, -258)", "room_02": "cs cam 15 (-526, 337, 21)", "room_03": "cs cam 14 (-553, 308, 379)",
    "room_04": "cs cam 13 (-566, 364, 464)", "toilet": "BgCamInfo 0 FIXED3 (33, 60, -156)"
  },
  "routeIds": [],
  "omissions": [
    "room_01 Anju lunch pose (persistent instance)", "room_01 a05 En_Nb", "room_00 a24 En_Rz Judo walking",
    "room_04 a13 En_Rz Marilla walking", "room_04 a07 En_Guruguru", "room_02 a02 En_Drs", "room_00 a05 En_Fish",
    "room_00 a15 and room_04 a11 En_Insect", "Link and Tatl"
  ],
  "excluded": {
    "halfDayMask": [
      "room_00/a02", "room_00/a18", "room_00/a21", "room_01/a01", "room_02/a01", "room_02/a07", "room_03/a03", "room_04/a01", "room_04/a02",
      "room_04/a03", "room_04/a05", "room_04/a06"
    ],
    "stateOrSchedule": [
      "room_00/a19 (drawn in room_01 at noon)", "room_00/a20", "room_00/a22", "room_00/a23", "room_01/a07 duplicate",
      "room_02/a11 duplicate", "room_02/a10 needs 51_40", "room_02/a06 Dm_Tag killed", "room_02/a00 cue only"
    ],
    "noGeometry": [
      "kickout tags x5", "En_Test4 x5", "room_00/a14", "room_00/a25", "room_01/a00", "room_03/a04",
      "room_00/a03 and room_04/a04 Mask of Scents", "room_00/a04 audio copy"
    ]
  },
  "unsupported": [
    "two resident rooms during door passes", "schedule-driven NPC placement", "per-floor environment light index", "fixed toilet camera",
    "water box and scrolling water texture", "Mask of Scents mushrooms", "audio-only Guru-Guru", "climbable bunk walls",
    "door schedule gates and kickout"
  ],
  "evidence": ["docs/city/interiors/stock_pot_inn.md"],
  "readiness": {
    "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run",
    "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run"
  }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Third Vec3s of each actor-cutscene camera triple (fov? roll?) | Read how `Camera_Fixed1` / `Camera_Unique4` parse `actorCsCamFuncData` in `src/code/z_camera.c` before turning cs cams 5, 10-16 into stored camera poses |
| U-02 | Meaning of `WEEKEVENTREG_32_20` and who sets `55_01`/`55_02` | grep `SET_WEEKEVENTREG(WEEKEVENTREG_32_20)` and `55_02` across `src/overlays` (En_Ig message script `z_en_ig.c:30-60`) |
| U-03 | Granny's SCH_2 pose (Anju's lunch pose is settled: `ENAN_ANIM_WAIT_WITH_TRAY`, an.c:2418-2431) | Read `EnNb_ChangeAnim` index `EN_NB_ANIM_0` in `func_80BC0B98` (z_en_nb.c:679-689) against `object_nb` animations |
| U-04 | Rosa sisters' positions at 12:00 | Does `EnRz_Walk` advance by frames or by `SubS` time path (z_en_rz.c path setup)? A static profile needs a declared waypoint |
| U-05 | Who is drawn if the player loads straight into room_01 at 12:00 (spawn 2 after a story) | Order of `EnAn_Init`/`EnNb` lunch checks when both spawn in the same frame (an.c:877-884 room slot init) |
| U-06 | Which En_Test4 transition camera is used after several room changes | Confirm `sCsIdList` overwrite order (z_en_test4.c:326-337) against room load order; capture a dusk transition in two rooms |
| U-07 | Readers of exit [2] `0xD650` and [3] `0xBC20` | Same as PORTAL_REGISTER U4/U7: cutscene `nextEntrance` data and hardcoded `setupExitList[2]` uses |
| U-08 | Whether room_00's far cull entries (-435, 300, ...) and the rooms' through-door entries overlap the neighbour room when both are resident | Dump vertex extents of `r00 DL_010200/012760/013BC0` and `r01 DL_006DF8` etc. against the room footprints in section 1.2 |
| U-09 | Purpose of alt1 beyond the title chain | Read the title/opening game state for `0xBC01` (South alt3 script) and `Z2_YADOYACutsceneData_00AC44` actor cues for `Dm_An` |
| U-10 | Registration of the Inn interior to the East facade | Survey both door frames; the two door offsets differ (section 7), so seamless stitching needs a measured transform |
| U-11 | Ramp polys 8/9 walk behaviour (41.2 deg, floor type 0, no slide effect) | Controller test on the import; check player slope speed handling in `player.c` near :11944 |
| U-12 | `scene_texture_01` contents and which are shared with other interiors | Extract once (section 9) and diff the reference lists of the other interior rooms |
| U-13 | Exact lighting with `lightMode = true` and per-floor light index | Read `Environment_ChangeLightSetting` and the lightMode branch in `src/code/z_kankyo.c` |
| U-14 | LOCATION_REGISTER says transition actors are identical in every header | Positions and params are identical; side camera indices differ (consistent remap, section 1.1). Owner to reword the register line, not edit here |
| U-15 | Visual survey | Capture `cam.stock_pot_inn.*` (REF-03) under `prof.d1.noon.normal` and `entry` also under `prof.d1.night.normal` |

**Not inspected:** room DL contents and combiners; LightSettings values; cutscene command semantics beyond destinations and texts; minimap rendering; En_Fish/En_Insect behaviour; En_Gm, En_Ja, En_Ah, En_Test3, En_Pm, En_Ig beyond schedules and draw gates; message text bodies; `Z2_TOWN` beyond the East dossier; 3DS material.

**Checks run:** read the brief docs; read scene/room C, XML, scene/actor/object/sequence tables, `z_actor.c`, `z_room.c`, `z_scene.c`, `z_play.c`, `z_camera.c`, `z_bgcheck.c`, `z_schedule.c`, `z_msgevent.c`, `z64scripts.h`, `z64door.h`, `z64scene.h`, `z64cutscene.h` and the 30 Inn room-actor overlays plus En_Door at the lines cited (En_Fish, En_Insect, En_Mag, Dm_An, En_Ds2n headers and params only); scratch Python (`actors.py`, `tables.py`, `col.py` from REF-08, `col2.py`, `r0floors.py`, `conn.py`, `mats.py`; outputs in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\stock_pot_inn\`); SHA-256 of 13 inputs and of the ZAPD binary; JSON parse of section 10; ASCII-only and line-count check of this file. Cross-checks: all ten room-header object/actor counts, spawns 6/1, exits 4/4, entrances 6, transition actors 6 equal `docs/city/LOCATION_REGISTER.md`; door mapping equals East 2.4 and the PORTAL_REGISTER Inn rows.

**NOT run:** ZAPD (no object or texture extracted), Blender, Unity, any player or executable, captures, renders, controller routes, schedule simulation.

**Next dependency:** PIPE-01 review of section 10 (room `role`, door components, special relocations); REF-02 owner to fill the room-portal conditions from 2.5 and close U-10 in LOCATION_REGISTER; section 9 extraction; then ROOM-02 (INT-07 multi-room pilot) after ROOM-01.
