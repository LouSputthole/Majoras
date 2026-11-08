# East Clock Town (`loc.east_clock_town`): source inventory dossier

Task **REF-07** (FULL_CITY_PLAN.md section 12). Base commit `98e554b3c1fc778894fd2c66de64ff040a90bb71` on `main`. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_TOWN` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save, Human form; `docs/city/SOURCE_REGISTER.md`).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_TOWN/Z2_TOWN.c`, `room.c` = `.../Z2_TOWN_room_00.c`, `player.c` = `src/overlays/actors/ovl_player_actor/z_player.c`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. Cross-references: `docs/city/LOCATION_REGISTER.md` (REF-01), `docs/city/PORTAL_REGISTER.md` (REF-02), `docs/city/SOURCE_REGISTER.md` (REF-03), `docs/city/districts/north_clock_town.md` (REF-05, template).

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro/function; **OVL** read from the actor overlay source (including its `.schl` schedule script); **ANN** decomp-author comment or symbol name; **INF** inferred (reason given); **NI** not inspected.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_TOWN` / `SCENE_TOWN` | `include/tables/scene_table.h:221` |
| Scene-table index | `0x6C` | same line |
| Entrance-table scene index | `0x69` (`ENTR_SCENE_EAST_CLOCK_TOWN`) | `include/z64scene.h:698`; `src/code/z_scene_table.c:2678` |
| Title card text / draw config / restrictions | `0x101` / `SCENE_DRAW_CFG_DEFAULT` / `RESTRICTIONS_NONE` | scene_table.h:221 |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_SET(1, 0, 0, 0)`: switch0 = 1, others 0 (arg order per REF-05, `src/code/z_sram_NES.c:23`); meaning NI | scene_table.h:221 |
| XML files | Scene `Z2_TOWN` segment 2; room `Z2_TOWN_room_00` segment 3; room DLs declared `0x960`, `0x12D98`, `0x17668` | XML `assets/xml/scenes/Z2_TOWN/Z2_TOWN.xml` |
| Room count | 1 in all three headers | HDR scene.c:16, :1684, :2298 |
| Scene headers | default + 2 alternates (`Z2_TOWNSet_003A00`, `Z2_TOWNSet_004910`) | HDR scene.c:35-38 |
| Room headers | default + 2 alternates (`Z2_TOWN_room_00Set_000500`, `Set_0007C0`) | HDR room.c:25-28 |
| Special files | **none**: no `SCENE_CMD_SPECIAL_FILES` in any East header, so no sub-keep object is loaded (`src/code/z_scene.c:234-237` runs only for that command). North has one | HDR (grep: 0 hits; Z2_BACKTOWN.c: 2) |
| Sound | spec 1, ambience 0, seq 29 (default); alt1 (1, 19, 118); alt2 (0, 0, 86) | HDR scene.c:15, :1683, :2297 |
| Skybox | (0x00, 1, 10, false) default; alt1 weather 7; alt2 weather 0, last arg true | HDR scene.c:22, :1691, :2304 |
| Env light settings | 29 / 29 / 4 entries (values not transcribed) | HDR |
| Animated materials | single `{ 0, 6, NULL }` shared by all headers; segment 0 is skipped by `AnimatedMat_DrawMain` (per REF-05) | HDR scene.c:738-740 |
| Minimap | default `{0x0105, 0xFEC0, 0, 0xFC40, 0}` scale 52; alternates mapId `0xFFFF`, scale 0 | HDR scene.c:656-662, :2270-2276, :2468-2474 |
| Minimap chest | `{0x0000, 0x000A, 0x01DF, 0x00DC, 0x01EF}`: room 0, chest flag 0x0A, (479, 220, 495), the En_Box in section 4 | HDR scene.c:664-666 |
| Transition actors | 7 `ACTOR_EN_DOOR`, all rooms 0/0, cams 255/255 (section 2.4) | HDR scene.c:67-75 |
| Paths | 37 (default), 31 (alt1), none (alt2) | HDR scene.c:526-564, :2100 |
| Cutscene scripts | default 3 (one data block, spawns 1, 3, 5, spawnFlags `0x13`, nextEntrance `0xFFFF`) | HDR scene.c:631-635 |
| Actor cutscenes / cs cameras | 16 / 15 (default); 12 / 2 (alt1); 10 / none (alt2) | HDR |
| Room commands (default) | echo 4; `ROOM_BEHAVIOR(0x04, 0x00, 0, 0, false, 1)` (storm state 1); `SKYBOX_DISABLES(false, false)`; `TIME_SETTINGS(255, 255, 3)`; cullable room shape, 6 entries | HDR room.c:14-21, :295-308 |

### 1.1 What scene layers 1 and 2 change

Selection rule (C, per REF-01/REF-05): `altHeaders[sceneLayer - 1]` is used when `sceneLayer != 0` (`src/code/z_scene.c:465-474`), and `sceneLayer` is nonzero only for cutscene entries (`src/code/z_play.c:2210-2219`). East entrance 0 has three table rows: layer 0 `0x4102`, layer 1 `0x8102`, layer 2 `0x058B` (`z_scene_table.c:2198-2202`).

| Item | Default (layer 0) | Alt1 (`Set_003A00`, layer 1) | Alt2 (`Set_004910`, layer 2) |
| --- | --- | --- | --- |
| How it is entered | normal play | `Z2_YADOYA` alt1 cutscene script `{ ..., 0xD201, 0, CS_SPAWN_FLAG_ALWAYS }` (`Z2_YADOYA.c:4866-4868`): East entrance 0 layer 1 (C) | UNKNOWN: `0xD202` appears in no extracted scene and no `src/` file (grep), U-01 |
| Where its cutscene leads | n/a | `nextEntrance 0xD801` = South entrance 0 layer 1 (scene.c:2251-2253) | `nextEntrance 0x2401` = Milk Bar entrance 0 layer 1 (scene.c:2451-2453) |
| Purpose | Day 1 play | INF: title/attract chain (Inn alt1 -> East alt1 -> South alt1) and the room list adds `En_Mag`, whose file header says "Title Logo" (ANN, `src/overlays/actors/ovl_En_Mag/z_en_mag.c:4`) | INF: cutscene-only layer (fixed time, slow clock, one spawn outside the plaza) |
| Spawns / entrance list | 12 / 12 rows (entrance i -> spawn i, room 0) | 1 / 2 rows (both -> spawn 0): (1050, 420, -955) yaw 270 params `0x0FFF`, on the Milk Bar roof top (section 6) | 1 / 2 rows: (-573, 0, -336) yaw 0 params `0x0DFF`, on the South-frame strip west of the lower South exit |
| Exit list | 14 words | identical 14 words | **10 words, remapped** (section 2.2) |
| Transition actors | 7 schedule doors | identical | same positions, every params `0x01FF` = `ENDOOR_TYPE_NIGHT`, type var `0x7F` (half-day index 7, text offset 15) (C, `z_en_door.h:9-20`) |
| Collision | `Z2_TOWNCollisionHeader_0039C8` | same | same |
| Room object list | 25 banks | 17: drops `OBJECT_MM`, `BOJ`, `AN2`, `TEST3`, `AN1`, `IN2`, `DAI`, `BOX`, `KANBAN`; adds `OBJECT_MAG` | 11: F53, F52, OBJ_TOKEIDAI, OS_ANIME, BOJ, NIW, MK, TOKEI_STEP, CNE, MA2, DAIKU |
| Room actor list | 64 | 37 (section 4.3) | 12 (section 4.4) |
| Room time / behavior | (255, 255, 3); type 0x04, storm 1; echo 4 | (21, 1, 3); same behavior; echo 4 | (17, 0, 1); type 0x01, storm 0; echo 0 |

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:40-65; decode C)

Start mode `(params & 0xF00) >> 8`, bgCam index `params & 0xFF` (`include/z64player.h:17-40`); yaw `(rawRotY >> 7) & 0x1FF` (raw = `SPAWN_ROT_FLAGS(rot, flags)` = `(rot << 7) | flags`, `include/z64scene.h:16`). The "used by" column is the reverse of REF-02 rows (cross-check only).

| Spawn | Position | Yaw | Params | Mode / bgCam | Used by | Sits on (section 6) |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | (1346, 78, -708) | 270 | 0x0F02 | 0xF / 0x02 | no city exit (REF-02 teleport-only list); INF external Termina Field return | slope 294/295, gate level L2 |
| 1 | (697, 94, 205) | 90 | 0x0F03 | 0xF / 0x03 | South `0xD210` (lower) | ramp 467/468, L1 |
| 2 | (1762, 80, -1793) | 270 | 0x0FFF | 0xF / none | Observatory `0xD220`; alt2 exit [2] | tunnel floor 118/119, L3 |
| 3 | (406, 100, -777) | 90 | 0x0F04 | 0xF / 0x04 | South `0xD230` (upper) | plaza 283/284, L4 |
| 4 | (610, 100, -173) | 180 | 0x0F0C | 0xF / 0x0C | Treasure Chest Shop `0xD240` + kickout | plaza, L4 |
| 5 | (807, 200, -1776) | 90 | 0x0F05 | 0xF / 0x05 | North `0xD250` | north terrace, L5 |
| 6 | (1395, 100, -274) | 270 | 0x0F08 | 0xF / 0x08 | Honey & Darling `0xD260` + kickout; alt2 exit [6]; `En_Bom_Bowl_Man` `Entrance_CreateFromSpawn(6)` (OVL `z_en_bom_bowl_man.c:395`) | plaza, L4 |
| 7 | (1099, 200, -1992) | 0 | 0x0F0A | 0xF / 0x0A | Mayor's Residence `0xD270` + kickout | north terrace, L5 |
| 8 | (1095, 100, 257) | 235 | 0x0F09 | 0xF / 0x09 | Shooting Gallery `0xD280` + kickout | plaza, L4 |
| 9 | (669, 100, -846) | 0 | 0x0F06 | 0xF / 0x06 | Stock Pot Inn `0xD290` + kickout | plaza, L4 |
| 10 | (477, 280, -1270) | 0 | 0x0F07 | 0xF / 0x07 | Stock Pot Inn `0xD2A0` | Inn upper deck F13, L8 |
| 11 | (1036, 100, -902) | 0 | 0x0E0B | 0xE / 0x0B | Milk Bar `0xD2B0` + kickout | plaza, L4 |

Entrance-table flags (C, `z_scene_table.c:2198-2251`): entrance 0 `0x4102` / `0x8102` / `0x058B` (three layers); 1 `0xCA14`; 3 and 5 `0xC102`; 2, 4, 6-12 `0x4102`. Entrance 12 is declared (`sEastClockTownEntrance12`, spawn 12) but the scene has only 12 spawns and 12 `EntranceEntry` rows (REF-01 U-04).

### 2.2 Exit lists and consumers (HDR scene.c:96-111, :1726-1741, :2338-2349)

Decode is a cross-check of REF-02 (entrance scene `v >> 9`, entrance `(v >> 4) & 0x1F`, layer `v & 0xF`, `z_scene_table.c:2688-2694`). "Consumer" is what in East indexes the slot; collision consumers are `exitIndexPlusOne = slot + 1` (bits 8-12 of `SurfaceType` word 0, `include/z64bgcheck.h:79-87`).

| Idx | Default value | Destination (REF-02 decode) | Consumer in East (default) | Alt2 value at same idx |
| --- | --- | --- | --- | --- |
| 0 | `0x5470` | Termina Field entr 7 (external) | surface 10, polys 299/300 (gate floor, L2) | `0x5470` |
| 1 | `0xD870` | South entr 7, (323, 0, 247) | surface 13, polys 461/462 (lower South ramp, L1) | `0xD870` |
| 2 | `0x4C00` | Observatory entr 0, (602, -472, 4125) room 0 | surface 4, floors 116/117 (tunnel end, L3); surface 5, **wall** 169 (inert, 2.5) | `0xD220` East spawn 2 (self) |
| 3 | `0xD820` | South entr 2, (180, 100, -770) | surface 9, polys 290/291 (upper South passage, L4) | `0xD820` |
| 4 | `0x2800` | Treasure Chest Shop entr 0 | surface 20, polys 481/482 behind TA2 | `0x2800` |
| 5 | `0xD610` | North entr 1, (562, 193, -1785) | surface 8, polys 286/287 (L5) | `0xD610` |
| 6 | `0x0800` | Honey & Darling's entr 0 | surface 18, polys 477/478 behind TA4 | `0xD260` East spawn 6 (self) |
| 7 | **`0x0000`** | **Mayor's Residence entr 0** -> `sMayorsResidenceEntrance0[0]` spawn 0 (3, 0, 304) room 0 | **surface 17, polys 475/476 (exit+1 = 8) behind TA5**; the door probe lands on poly 476 (2.4), so slot 7 is a real exit, not padding (C) | `0x3A00` Shooting Gallery |
| 8 | `0x3A00` | Town Shooting Gallery entr 0 | surface 19, polys 479/480 behind TA3 | `0x0800` Honey & Darling's |
| 9 | `0xBC00` | Stock Pot Inn entr 0, spawn 0 (149, 0, 303) room 0 | surface 14, polys 469/470 behind TA0 | `0xD880` South entr 8 |
| 10 | `0xBC10` | Stock Pot Inn entr 1, spawn 1 (-63, 210, -46) room 0 | surface 16, polys 473/474 behind TA1 | (list ends at 9) |
| 11 | `0xD650` | North entr 5, (-666, 200, -2256) | `En_Bomjimb` a48 `0x0B22`, a49 `0x0B11`: `(p >> 8) & 0xFF = 0x0B` (OVL `z_en_bomjimb.c:161`); both killed on a fresh save (4.5) | (out of range) |
| 12 | `0x2400` | Milk Bar entr 0, (-200, 152, 360) room 0 | surface 15, polys 471/472 behind TA6 | (out of range) |
| 13 | `0x0000` | (would be Mayor's entr 0) | none: no surface has exit+1 = 14 and no East actor reads slot 13; INF trailing padding (REF-01 U-02) | (out of range) |

Alt1 uses the identical 14 words. In alt1 only `En_Bomjimb` alt1 #27/#28 (`0x0B22`/`0x0B11`, room.c:446, :448) read a slot (11). **REF-02 U13 answer:** no actor list in any East header references slot 13; slot 11 is read only by `En_Bomjimb` in the default and alt1 lists, which have 14 entries. Alt2's 12 actors include none of the 15 overlays that index `setupExitList` (grep: Door_Ana, Door_Warp1, En_Bomjimb, En_Ge2, En_Ge3, En_Holl, En_Hs, En_Js, En_Kaizoku, En_Kgy, En_Time_Tag, En_Warp_tag, En_Zod, En_Zov, player); its `En_Ma_Yto` sets hardcoded `ENTRANCE(TERMINA_FIELD, 13)` / `ENTRANCE(ROMANI_RANCH, 8)` (OVL `z_en_ma_yto.c:1132-1134`). So in alt2 the only out-of-range readers are the shared collision surfaces 16 (exit+1 = 11) and 15 (exit+1 = 13), reachable only by opening TA1 or TA6 (2.4). Whether the player can reach them in alt2: UNKNOWN (U-12); the alt2 spawn is west of the lower South exit trigger, whose alt2 slot 1 (`0xD870`) is valid.

### 2.3 Collision surfaces carrying exits (C from scene.c:751-758 and the polygon list)

Level IDs refer to section 6.3. vIB flag 1 = `COLPOLY_IS_FLOOR_CONVEYOR` (`include/z64bgcheck.h:71`); conveyor direction x 1024 = binang (`CONVEYOR_DIRECTION_TO_BINANG`, z64bgcheck.h:265). xp 1 = `COLPOLY_IGNORE_CAMERA`.

| Surface | data[0] / data[1] | Exit+1 -> slot | Polys | Kind | Extent x / y / z | Level | Conveyor (speed / dir) | Flags |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 10 | 0x00000101 / 0x020C17C2 | 1 -> 0 TF | 299, 300 | floor | 1480..1720 / 60 / -800..-620 | L2 gate | 3 / 16 (+X, outward east) | vIB 1 |
| 13 | 0x00000201 / 0x060C17C2 | 2 -> 1 South lower | 461, 462 | floor, 15.0 deg | 347..680 / 0..89 / 120..360 | L1 | 3 / 48 (-X, toward South) | vIB 1 |
| 4 | 0x00000301 / 0x020C17C2 | 3 -> 2 Observatory | 116, 117 | floor | 1832..1960 / 80 / -1824..-1764 | L3 tunnel | 3 / 16 (+X) | vIB 1 |
| 5 | 0x00000301 / 0x000017CA | 3 -> 2 Observatory | 169 | **wall**, n (0.97, -0.03, 0.24) | 594..599 / 100..245 / -640..-628 | L4 (under L8) | none | xp 1 |
| 9 | 0x00000401 / 0x060C17C2 | 4 -> 3 South upper | 290, 291 | floor | 200..368 / 100 / -835..-715 | L4 | 3 / 48 (-X) | vIB 1 |
| 20 | 0x00000501 / 0x000017C2 | 5 -> 4 Treasure | 481, 482 | floor | 580..640 / 100 / -160..-80 | L4 | none | xp 1 |
| 8 | 0x00000601 / 0x060C17C2 | 6 -> 5 North | 286, 287 | floor | 520..768 / 200 / -1864..-1704 | L5 | 3 / 48 (-X) | vIB 1 |
| 18 | 0x00000701 / 0x000017C2 | 7 -> 6 Honey & Darling | 477, 478 | floor | 1400..1480 / 100 / -304..-244 | L4 | none | xp 1 |
| 17 | 0x00000801 / 0x000017C2 | 8 -> 7 Mayor's | 475, 476 | floor | 1070..1130 / 200 / -2080..-2000 | L5 | none | xp 1 |
| 19 | 0x00000901 / 0x000017C0 | 9 -> 8 Gallery | 479, 480 | floor | 1079..1178 / 100 / 239..338 | L4 | none | xp 1 |
| 14 | 0x00000A01 / 0x000017C0 | 10 -> 9 Inn lower | 469, 470 | floor | 638..698 / 100 / -935..-855 | L4 | none | xp 1 |
| 16 | 0x00000B01 / 0x000017C2 | 11 -> 10 Inn upper | 473, 474 | floor | 447..507 / 280 / -1360..-1280 | L8 | none | xp 1 |
| 15 | 0x00000D01 / 0x000017C0 | 13 -> 12 Milk Bar | 471, 472 | floor | 1008..1068 / 100 / -991..-911 | L4 | none | xp 1 |

Totals: 13 surface types, 25 polygons (24 floors + 1 wall). The five walk-through district/observatory exits are conveyor floors; the seven door exits are camera-ignored floors hidden behind doors. No surface uses exit+1 = 12 or 14.

### 2.4 Door-to-venue mapping: the 7 transition actors (C)

Mechanism (C): `EN_DOOR` params hold only transition index, type and type var (`z_en_door.h:9-20`; `TRANSITION_ACTOR_PARAMS`, `include/z64door.h:22-25`). When the player opens a non-fake door, `player.c:6779-6783` computes a point `22 * doorDirection` behind the door along its yaw (`func_80835BF8`, player.c:6548-6551), raycasts the floor at door Y + 10 and passes that floor polygon to `Player_HandleExitsAndVoids`. `doorDirection = +1` when the player is on the door's local +Z side (`z_en_door.c:485-496`); the schedule check runs only from that side (`z_en_door.c:537`). The "probe" column is my evaluation of that raycast against the East floors (scratch `doors.py`). Transition rotY is decoded as `(rotY >> 7) & 0x1FF` degrees, low 7 bits csId (`src/code/z_actor.c:3613-3619`). Door art: `sObjectInfo[17 + schType]` and `sDoorDLists` (OVL `z_en_door.c:189-245`, :306-321), drawn on `gDoorSkel` limb 4 (`z_en_door.c:640-660`).

| TA | Position | Yaw (raw) | Rooms (front / back, cam) | Params -> type / schedule type | Door art | Day 1 12:00 schedule (OVL `scheduleScripts.schl`) | Probe from the front -> poly | Exit+1 -> slot -> value | Venue (`loc.*`) and arrival | Return into East |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (668, 100, -875) | 0 (0x007F) | 0 / 0, 255 / 255 | 0x028B -> SCHEDULE / 11 `INN_MAIN_ENTRANCE` | object_wdor03 `gInnSchoolDoorDL` 0x548 | open (closed outside 08:00-20:30 on Days 1-2 without the Room Key, :213-234) | (668, 110, -897) -> 470 | 10 -> 9 -> `0xBC00` | `loc.stock_pot_inn` spawn 0 (149, 0, 303) room 0, **lower street door** | spawn 9 (669, 100, -846), 29 u in front |
| 1 | (477, 280, -1300) | 0 (0x007F) | 0 / 0 | 0x028C -> SCHEDULE / 12 `INN_UPPER_ENTRANCE` | object_wdor03 `gInnSchoolDoorDL` | always open (`return_none`, :237-239) | (477, 290, -1322) -> 474 | 11 -> 10 -> `0xBC10` | `loc.stock_pot_inn` spawn 1 (-63, 210, -46) room 0, **upper deck door at Y 280** | spawn 10 (477, 280, -1270) |
| 2 | (610, 100, -140) | 180 (0x5A7F) | 0 / 0 | 0x0288 -> SCHEDULE / 8 `TREASURE_CHEST_SHOP` | gameplay_keep `gDoorLeftDL` 0x20BB8 / `gDoorRightDL` 0x20D00 (side by eye yaw) | open (closed since 22:00, :174-180) | (610, 110, -118) -> 482 | 5 -> 4 -> `0x2800` | `loc.treasure_chest_shop` spawn 0 (-70, 0, 161) | spawn 4 (610, 100, -173) |
| 3 | (1114, 100, 274) | 225 (0x70FF) | 0 / 0 | 0x0287 -> SCHEDULE / 7 `TOWN_SHOOTING_GALLERY` | gameplay_keep default pair | open (closed since 22:00, :165-171) | (1129.6, 110, 289.6) -> 480 | 9 -> 8 -> `0x3A00` | `loc.town_shooting_gallery` spawn 0 (-10, 0, 390) | spawn 8 (1095, 100, 257) |
| 4 | (1420, 100, -274) | 270 (0x877F) | 0 / 0 | 0x0289 -> SCHEDULE / 9 `HONEY_DARLING_SHOP` | gameplay_keep default pair | open (closed since 22:00, :183-189) | (1442, 110, -274) -> 478 | 7 -> 6 -> `0x0800` | `loc.honey_darling` spawn 0 (0, 0, 300) | spawn 6 (1395, 100, -274) |
| 5 | (1100, 200, -2020) | 0 (0x007F) | 0 / 0 | 0x0291 -> SCHEDULE / 17 `MAYORS_RESIDENCE_MAIN_ENTRANCE` | object_wdor01 `gLotteryCuriosityShopMayorHouseDoorDL` 0x548 | open (Days 1-2 10:00-20:00, :275-294) | (1100, 210, -2042) -> 476 | 8 -> **7** -> `0x0000` | `loc.mayors_residence` spawn 0 (3, 0, 304) room 0 | spawn 7 (1099, 200, -1992) |
| 6 | (1038, 100, -931) | 0 (0x007F) | 0 / 0 | 0x028A -> SCHEDULE / 10 `MILK_BAR` | object_wdor04 `gMilkBarDoorDL` 0x508 | open (10:00-21:00, :192-210) | (1038, 110, -953) -> 472 | 13 -> 12 -> `0x2400` | `loc.milk_bar` spawn 0 (-200, 152, 360) room 0 | spawn 11 (1036, 100, -902) |

From the back side every probe lands on an ordinary plaza/deck floor with no exit (469 vs 283, 474 vs 133, etc.), so each door transitions only when opened from the street side (C). Every door's `ENDOOR_SCH_TYPE_*` names the venue its probed exit decodes to, matching REF-02's independent check. **The two Inn doors are distinct**: TA0 at plaza level (Y 100, exit slot 9, Inn spawn 0 at Y 0) and TA1 on the upper deck (Y 280, slot 10, Inn spawn 1 at Y 210). REF-01's inference that slot 7 `0x0000` is the Mayor's door is confirmed (C).

### 2.5 Observatory route and wall polygon 169 (REF-02 U3)

- **Walk-in route (C):** north terrace L5 (Y 200) -> corridor mouth polys 112/113 (Y 200, x 1400..1480, z -1824..-1764, 60 u wide) -> ramp 106/107 (26.6 deg, Y 200 down to Y 80, x 1480..1720) -> tunnel floor 118/119 (Y 80) -> exit floors 116/117 (surface 4, x 1832..1960) -> `0x4C00` -> Observatory spawn 0 room 0. Ceilings 122/123 at Y 200 cover x 1800..1960 (INF: the underground mouth). The Observatory's return `0xD220` lands on spawn 2 (1762, 80, -1793) on 118/119, facing -X (back up the ramp).
- **Gate at the corridor mouth (OVL):** `En_Bombers2` a47 at (1388, 200, -1795) yaw 270, 12 u west of the mouth, centred on it (corridor z centre -1794), cylinder radius 35 height 30 (`z_en_bombers2.c` sCylinderInit). Init keeps it unless `csId == 0` (default csId 11), and if `WEEKEVENTREG_ENTERED_BOMBERS_CODE` is set or the entrance is `ENTRANCE(EAST_CLOCK_TOWN, 2)` it moves 50 u along `home.rot.y + 0xC100` (`z_en_bombers2.c:123-128`), which for yaw 270 is about -Z, out of the corridor (C). The flag is set by its own talk flow (`:373-375`). INF: on a fresh save the 60 u corridor is blocked by a 70 u collider until the code is entered: a story restriction, not geometry. `En_Bom_Bowl_Man` a46 (1340, 200, -1750) params `0x0510` is killed unless the entrance is East 2 with the code set and no Notebook (`z_en_bom_bowl_man.c:157-167`): absent on a normal Day 1 entry.
- **Wall polygon 169 (C + INF):** poly 169 (surface 5, exit+1 = 3, WOOD, xp 1) is one face of a six-face narrow column, polys 164-169 (all walls, surfaces 3 and 5, xp 1), about 15 x 13 u in plan around (592, -634), rising from the plaza (Y 100) to Y 243-245. It stands directly under the Inn upper-deck walkway (floors 171/172 span x 400..614, z -855..-603, Y 239..280; the walkway underside is ceilings 173-175). INF: it is a support post of that walkway. Surface 5 differs from the post's other faces (surface 3, `0x00000001 / 0x000017CA`) only by the exit bits in word 0 (`0x301`).
- **Is it a usable exit? (C):** the player passes only floor polygons to `Player_HandleExitsAndVoids`: `this->actor.floorPoly` (player.c:12572) and the door floor raycast (player.c:6779-6783). Of the other `SurfaceType_GetSceneExitIndex` callers (Bg_Tobira01, En_Bomjimb, En_Dns, En_Mt_tag, En_Rg, En_Trt2) all read floor polygons, and only En_Bomjimb is in East. INF: poly 169's exit is never read in play; it is not an observatory access. This disagrees with REF-02's district row `portal.east_clock_town.astral_observatory.p169` (U-03).
- **Actors near poly 169 (HDR, all headers, within 250 u XZ):** `En_Bomjimb` a49 (491, 267, -678) params `0x0B11` on the walkway above, 115 u (killed on a fresh save); `En_Gm` a3 (731, 100, -677), 141 u; `En_Ja` a8 (625, 100, -470) D2 only, 166 u; `Obj_Kibako` a11 (759, 100, -458), 239 u. **No `BG_*` or `OBJ_*` actor is at the post**; the nearest object actors are the crate (239 u) and the Deku flower a9 (347 u).

### 2.6 The two South approaches (keep distinct)

| | Upper (`0xD230` from South) | Lower (`0xD210` from South) |
| --- | --- | --- |
| East trigger | slot 3 `0xD820`, surface 9, polys **290, 291** | slot 1 `0xD870`, surface 13, polys **461, 462** |
| Extent x / y / z | 200..368 / 100 / -835..-715 | 347..680 / 0..89 / 120..360 |
| Shape | flat, seamless with 292/293 and the plaza (L4) | 15 deg ramp descending west from Y 89 to Y 0 (L1), then 467/468 (Y 89..100) up to the plaza |
| Overhead | ceilings 152/153 at Y 260 over x 200..400, z -835..-715 (160 u headroom: a covered passage, INF) | none |
| Arrival into East | spawn 3 (406, 100, -777) yaw 90 | spawn 1 (697, 94, 205) yaw 90 |
| South-side trigger (REF-02) | South polys 24/25, (316, 100, -775) | South polys 180/181, (534, 50, 240) |
| Separation | about 1,000 u apart in Z; upper is level with the plaza, lower drops 100 u toward South | |

West of x 347 the lower ramp continues into 9 floor polys at South coordinates (380-388, x -1120..347, Y 0..100), the "South-frame strip" (L0). INF: duplicated South ground for alt2, whose spawn (-573, 0, -336) is on it (U-13).

### 2.7 Proposed portal IDs (cross-reference `docs/city/PORTAL_REGISTER.md`, REF-02)

| Portal ID (REF-02) | Source evidence (this doc) | Arrival back into East |
| --- | --- | --- |
| `portal.east_clock_town.south_clock_town.upper` | surface 9, polys 290/291, slot 3 | spawn 3 via `0xD230` |
| `portal.east_clock_town.south_clock_town.lower` | surface 13, polys 461/462, slot 1 | spawn 1 via `0xD210` |
| `portal.east_clock_town.north_clock_town` | surface 8, polys 286/287, slot 5 | spawn 5 via `0xD250` |
| `portal.east_clock_town.z2_00keikoku` (to `ext.z2_00keikoku`) | surface 10, polys 299/300, slot 0 | UNKNOWN (external; spawn 0 is a candidate, U-14) |
| `portal.east_clock_town.astral_observatory.p116` | surface 4, polys 116/117, slot 2; gated by a47 | spawn 2 via `0xD220` |
| `portal.east_clock_town.astral_observatory.p169` | wall poly 169: **inert, propose retiring** (2.5, U-03) | none |
| `portal.east_clock_town.stock_pot_inn.lower` | TA0 -> poly 470, slot 9 | spawn 9 via `0xD290` |
| `portal.east_clock_town.stock_pot_inn.upper` | TA1 -> poly 474, slot 10 | spawn 10 via `0xD2A0` |
| `portal.east_clock_town.mayors_residence` | TA5 -> poly 476, slot 7 | spawn 7 via `0xD270` |
| `portal.east_clock_town.milk_bar` | TA6 -> poly 472, slot 12 | spawn 11 via `0xD2B0` |
| `portal.east_clock_town.honey_darling` | TA4 -> poly 478, slot 6 | spawn 6 via `0xD260` |
| `portal.east_clock_town.treasure_chest_shop` | TA2 -> poly 482, slot 4 | spawn 4 via `0xD240` |
| `portal.east_clock_town.town_shooting_gallery` | TA3 -> poly 480, slot 8 | spawn 8 via `0xD280` |
| `portal.east_clock_town.north_clock_town.bomjimb` | a48/a49, slot 11, one-way, inactive on a fresh save | North spawn 5 (not East) |

## 3. Object banks (HDR room.c:30-56; IDs from `include/tables/object_table.h`)

Actor spawn fails if the profile's object is not loaded (`Actor_SpawnAsChildAndCutscene`, `src/code/z_actor.c:3517-3523`, per REF-05).

| # | Bank | ID | Consumer (profile `objectId` or `Object_GetSlot`, OVL) | Needed for Day 1 static list |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_F53_OBJ` | 0x16D | Obj_Entotu a13 | billboard approximation only |
| 1 | `OBJECT_F52_OBJ` | 0x16C | Obj_Bell a45 | yes |
| 2 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai a27-a29 | yes (already extracted) |
| 3 | `OBJECT_NIW` | 0x00F | no listed East actor (En_Niw, En_Attack_Niw, En_Nwc reference it) | no |
| 4 | `OBJECT_KIBAKO` | 0x16F | Obj_Kibako a10/a11, bank index 1 (`z_obj_kibako.c:70`, :139-155) | yes |
| 5 | `OBJECT_CS` | 0x110 | En_Bom_Bowl_Man, En_Bombers2, En_Bomjimb, En_Bombers | skeletal (later) |
| 6 | `OBJECT_MM` | 0x107 | En_Pm a12 | no (schedule none at noon) |
| 7 | `OBJECT_OS_ANIME` | 0x09D | no listed East actor | no |
| 8 | `OBJECT_SDN` | 0x1B6 | En_Stop_heishi a26 | skeletal (later) |
| 9 | `OBJECT_BOJ` | 0x0E3 | En_Ja a5-a8 | skeletal (later) |
| 10 | `OBJECT_PST` | 0x1CB | En_Pst a60/a61 | posed skeletal |
| 11 | `OBJECT_TOKEI_STEP` | 0x1A4 | Obj_Tokei_Step a30 | yes (already extracted) |
| 12 | `OBJECT_TOKEI_TURRET` | 0x205 | Obj_Tokei_Turret a31-a37 | yes (already extracted) |
| 13 | `OBJECT_LODMOON` | 0x223 | En_Fall type 6 a62 | yes (moon, special rule) |
| 14 | `OBJECT_AN2` | 0x0E0 | En_An secondary slot (`z_en_an.c:882`) | no |
| 15 | `OBJECT_TEST3` | 0x01C | En_Test3 a2 | no (Day 3) |
| 16 | `OBJECT_AN1` | 0x0E2 | En_An a0 | no |
| 17 | `OBJECT_IN2` | 0x248 | En_Gm a3/a4 | no |
| 18 | `OBJECT_DAI` | 0x1D5 | En_Ig a57 | no |
| 19 | `OBJECT_WDOR01` | 0x274 | En_Door TA5 (schedule type 17 -> `DOOR_OBJINFO_34`) | yes (door) |
| 20 | `OBJECT_WDOR02` | 0x275 | no East door (Post Office / Trading Post types) | no |
| 21 | `OBJECT_WDOR03` | 0x276 | En_Door TA0, TA1 (types 11, 12) | yes (doors) |
| 22 | `OBJECT_WDOR04` | 0x27B | En_Door TA6 (type 10) | yes (door) |
| 23 | `OBJECT_BOX` | 0x00C | En_Box a59 | posed skeletal |
| 24 | `OBJECT_KANBAN` | 0x0FC | En_Kanban a56 | yes |

Implicit: `gameplay_keep` 0x001 (En_Talk, En_Hit_Tag, En_Light, En_Elforg, Obj_Etcetera, En_Test4, Obj_Sound, the default door pair and `gDoorSkel` for TA2-TA4, the sign rectangle DL). No sub-keep: East has no special-files command (section 1).

## 4. Actor tables

### 4.1 Decode rules

As REF-05 section 4.1 (verified again in `src/code/z_actor.c:3628-3654`, :2546-2575, :3560-3580): ID `id & 0x1FFF`; rotation `(raw >> 7) & 0x1FF` degrees, except axes flagged in the ID (`0x8000` Y, `0x4000` X, `0x2000` Z) which arrive as plain integers wrapped to -180..180 (marked "int"); csId `rawRotY & 0x7F`; half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all; Day 1 12:00 -> bit `0x080`. No East ID carries `0x800`. Classes as REF-05. "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (64 entries, HDR room.c:58-289, decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_AN` (0x202) | (674, 100, -913) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x8001 | object_an1 | npc-scheduled | yes |
| 1 | `ACTOR_EN_ELFORG` (0x1B0) | (663, 199, -335) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 D0n, D1n, D2n, D3n, D4n | none | 0xFE03 | gameplay_keep | billboard/effect | no |
| 2 | `ACTOR_EN_TEST3` (0x159) | (1713, 60, -722) | 0x0000 / 0x877F / 0x000C | 0 / 270 / 0 | 0x00C D3d, D3n | none | 0x01E2 | object_test3 | npc-scheduled | no |
| 3 | `ACTOR_EN_GM` (0x0A4) | (731, 100, -677) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0003 | object_in2 | npc-scheduled | yes |
| 4 | `ACTOR_EN_GM` (0x0A4) | (686, 100, -907) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0003 | object_in2 | npc-scheduled | yes |
| 5 | `ACTOR_EN_JA` (0x244) | (1075, 100, -269) | 0x0001 / 0x96FF / 0x0000 | 0 / 301 / 0 | 0x080 D1d | none | 0x0001 | object_boj | npc-scheduled | yes |
| 6 | `ACTOR_EN_JA` (0x244) | (460, 100, -332) | 0x0000 / 0x407F / 0x0020 | 0 / 128 / 0 | 0x020 D2d | none | 0x0001 | object_boj | npc-scheduled | no |
| 7 | `ACTOR_EN_JA` (0x244) | (919, 100, -164) | 0x0001 / 0x407F / 0x0000 | 0 / 128 / 0 | 0x080 D1d | none | 0x0000 | object_boj | npc-scheduled | yes |
| 8 | `ACTOR_EN_JA` (0x244) | (625, 100, -470) | 0x0000 / 0xA17F / 0x0020 | 0 / 322 / 0 | 0x020 D2d | none | 0x0000 | object_boj | npc-scheduled | no |
| 9 | `ACTOR_OBJ_ETCETERA` (0x183) | (350, 120, -390) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x007F | gameplay_keep | static-mesh | yes |
| 10 | `ACTOR_OBJ_KIBAKO` (0x081) | (1100, 100, -460) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x8181 | object_kibako (bank 1) | static-mesh | yes |
| 11 | `ACTOR_OBJ_KIBAKO` (0x081) | (759, 100, -458) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x828A | object_kibako (bank 1) | static-mesh | yes |
| 12 | `ACTOR_EN_PM` (0x1D5) | (540, 200, -1800) | 0x0007 / 0x2D7F / 0x007F | 0 / 90 / 0 | 0x3FF all | none | 0x0000 | object_mm | npc-scheduled | yes |
| 13 | `ACTOR_OBJ_ENTOTU` (0x14D) | (319, 553, -1308) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_f53_obj | billboard/effect | yes |
| 14 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (1398, 164, -431) | 0x0387 / 0x877F / 0x007F | 7 int / -90 int / 0 int | 0x3FF all | none | 0xFE00 | gameplay_keep | controller/invisible | yes |
| 15 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (1398, 254, -1639) | 0x0387 / 0x877F / 0x007F | 7 int / -90 int / 0 int | 0x3FF all | none | 0xFE00 | gameplay_keep | controller/invisible | yes |
| 16 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (805, 256, -1568) | 0x0387 / 0x2D7F / 0x007F | 7 int / 90 int / 0 int | 0x3FF all | none | 0xFE00 | gameplay_keep | controller/invisible | yes |
| 17 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (721, 186, -47) | 0x0387 / 0x2D7F / 0x007F | 7 int / 90 int / 0 int | 0x3FF all | none | 0xFE01 | gameplay_keep | controller/invisible | yes |
| 18 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (1151, 168, -900) | 0x0387 / 0x007F / 0x007F | 7 int / 0 int / 0 int | 0x3FF all | none | 0xFE02 | gameplay_keep | controller/invisible | yes |
| 19 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (511, 166, -164) | 0x0387 / 0x5A7F / 0x007F | 7 int / 180 int / 0 int | 0x3FF all | none | 0xFE03 | gameplay_keep | controller/invisible | yes |
| 20 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (720, 156, 50) | 0x0387 / 0x2D7F / 0x007F | 7 int / 90 int / 0 int | 0x3FF all | none | 0xFE03 | gameplay_keep | controller/invisible | yes |
| 21 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (1279, 161, -555) | 0x0387 / 0x877F / 0x007F | 7 int / -90 int / 0 int | 0x3FF all | none | 0xFE06 | gameplay_keep | controller/invisible | yes |
| 22 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (1352, 219, -273) | 0x0407 / 0x877F / 0x007F | 8 int / -90 int / 0 int | 0x3FF all | none | 0xFE0C | gameplay_keep | controller/invisible | yes |
| 23 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (1098, 364, -1982) | 0x0407 / 0x007F / 0x007F | 8 int / 0 int / 0 int | 0x3FF all | none | 0xFE12 | gameplay_keep | controller/invisible | yes |
| 24 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (1049, 248, 205) | 0x0407 / 0x6BFF / 0x007F | 8 int / -145 int / 0 int | 0x3FF all | none | 0xFE15 | gameplay_keep | controller/invisible | yes |
| 25 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (720, 175, -854) | 0x0387 / 0x007F / 0x007F | 7 int / 0 int / 0 int | 0x3FF all | none | 0xFE11 | gameplay_keep | controller/invisible | yes |
| 26 | `ACTOR_EN_STOP_HEISHI \| 0x2000` (0x1C7) | (1402, 60, -703) | 0x0007 / 0x877F / 0x007F | 0 / 270 / 0 int | 0x3FF all | none | 0x307F | object_sdn | npc-scheduled | yes |
| 27 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-280, 555, -833) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x207F | object_obj_tokeidai | static-mesh | yes |
| 28 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-68, 555, -1012) | 0x0007 / 0x2D7F / 0x007F | 0 / 90 / 0 | 0x3FF all | none | 0x007F | object_obj_tokeidai | static-mesh | yes |
| 29 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-280, 1103, -1020) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x307F | object_obj_tokeidai | static-mesh | yes |
| 30 | `ACTOR_OBJ_TOKEI_STEP` (0x1B8) | (-280, 255, -853) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_tokei_step | static-mesh | yes |
| 31 | `ACTOR_OBJ_TOKEI_TURRET` (0x221) | (-290, 0, 160) | 0x0000 / 0x007F / 0x000C | 0 / 0 / 0 | 0x00C D3d, D3n | none | 0x0002 | object_tokei_turret | static-mesh | no |
| 32 | `ACTOR_OBJ_TOKEI_TURRET` (0x221) | (-290, 80, 160) | 0x0001 / 0x007F / 0x0070 | 0 / 0 / 0 | 0x0F0 D1d, D1n, D2d, D2n | none | 0x0001 | object_tokei_turret | static-mesh | yes |
| 33 | `ACTOR_OBJ_TOKEI_TURRET` (0x221) | (-290, 160, 160) | 0x0000 / 0x007F / 0x0030 | 0 / 0 / 0 | 0x030 D2d, D2n | none | 0x0001 | object_tokei_turret | static-mesh | no |
| 34 | `ACTOR_OBJ_TOKEI_TURRET` (0x221) | (-290, 240, 160) | 0x0000 / 0x007F / 0x000C | 0 / 0 / 0 | 0x00C D3d, D3n | none | 0x0001 | object_tokei_turret | static-mesh | no |
| 35 | `ACTOR_OBJ_TOKEI_TURRET` (0x221) | (-290, 0, 160) | 0x0001 / 0x007F / 0x007C | 0 / 0 / 0 | 0x0FC D1d, D1n, D2d, D2n, D3d, D3n | none | 0x0000 | object_tokei_turret | static-mesh | yes |
| 36 | `ACTOR_OBJ_TOKEI_TURRET` (0x221) | (-290, 80, 160) | 0x0000 / 0x007F / 0x003C | 0 / 0 / 0 | 0x03C D2d, D2n, D3d, D3n | none | 0x0000 | object_tokei_turret | static-mesh | no |
| 37 | `ACTOR_OBJ_TOKEI_TURRET` (0x221) | (-290, 160, 160) | 0x0000 / 0x007F / 0x000C | 0 / 0 / 0 | 0x00C D3d, D3n | none | 0x0000 | object_tokei_turret | static-mesh | no |
| 38 | `ACTOR_EN_LIGHT` (0x004) | (934, 406, -460) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 D0n, D1n, D2n, D3n, D4n | none | 0x07F5 | gameplay_keep | billboard/effect | no |
| 39 | `ACTOR_EN_LIGHT` (0x004) | (1146, 451, -1712) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 D0n, D1n, D2n, D3n, D4n | none | 0x07F5 | gameplay_keep | billboard/effect | no |
| 40 | `ACTOR_EN_LIGHT` (0x004) | (934, 120, -460) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 D0n, D1n, D2n, D3n, D4n | none | 0x27F5 | gameplay_keep | billboard/effect | no |
| 41 | `ACTOR_EN_LIGHT` (0x004) | (1146, 200, -1712) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 D0n, D1n, D2n, D3n, D4n | none | 0x27F5 | gameplay_keep | billboard/effect | no |
| 42 | `ACTOR_EN_HIT_TAG \| 0xE000` (0x265) | (1389, 250, -346) | 0x0007 / 0x877F / 0x007F | 0 int / -90 int / 0 int | 0x3FF all | none | 0xFE00 | gameplay_keep | controller/invisible | yes |
| 43 | `ACTOR_EN_HIT_TAG \| 0xE000` (0x265) | (1388, 250, -199) | 0x0007 / 0x877F / 0x007F | 0 int / -90 int / 0 int | 0x3FF all | none | 0xFE00 | gameplay_keep | controller/invisible | yes |
| 44 | `ACTOR_EN_HIT_TAG \| 0xE000` (0x265) | (1381, 212, -272) | 0x0007 / 0x007F / 0x007F | 0 int / 0 int / 0 int | 0x3FF all | none | 0xFE00 | gameplay_keep | controller/invisible | yes |
| 45 | `ACTOR_OBJ_BELL` (0x14E) | (633, 284, -966) | 0x0007 / 0x097F / 0x007F | 0 / 18 / 0 | 0x3FF all | none | 0x0000 | object_f52_obj | static-mesh | yes |
| 46 | `ACTOR_EN_BOM_BOWL_MAN \| 0x2000` (0x11C) | (1340, 200, -1750) | 0x0007 / 0x000C / 0x017F | 0 / 0 / 2 int | 0x3FF all | 12 | 0x0510 | object_cs | npc-scheduled | yes |
| 47 | `ACTOR_EN_BOMBERS2` (0x281) | (1388, 200, -1795) | 0x0007 / 0x870B / 0x007F | 0 / 270 / 0 | 0x3FF all | 11 | 0x0000 | object_cs | npc-scheduled | yes |
| 48 | `ACTOR_EN_BOMJIMB` (0x27F) | (1291, 300, -808) | 0x0007 / 0xA07F / 0x007F | 0 / 320 / 0 | 0x3FF all | none | 0x0B22 | object_cs | npc-scheduled | yes |
| 49 | `ACTOR_EN_BOMJIMB` (0x27F) | (491, 267, -678) | 0x0007 / 0x177F / 0x007F | 0 / 46 / 0 | 0x3FF all | none | 0x0B11 | object_cs | npc-scheduled | yes |
| 50 | `ACTOR_EN_BOMBERS` (0x280) | (505, 100, -266) | 0x0001 / 0x007F / 0x0040 | 0 / 0 / 0 | 0x0C0 D1d, D1n | none | 0x0002 | object_cs | npc-scheduled | yes |
| 51 | `ACTOR_EN_BOMBERS` (0x280) | (1112, 372, -1132) | 0x0000 / 0x007F / 0x0030 | 0 / 0 / 0 | 0x030 D2d, D2n | none | 0x0002 | object_cs | npc-scheduled | no |
| 52 | `ACTOR_EN_BOMBERS` (0x280) | (514, 280, -1093) | 0x0000 / 0x007F / 0x000C | 0 / 0 / 0 | 0x00C D3d, D3n | none | 0x0002 | object_cs | npc-scheduled | no |
| 53 | `ACTOR_EN_BOMBERS` (0x280) | (1110, 372, -1148) | 0x0001 / 0x007F / 0x0040 | 0 / 0 / 0 | 0x0C0 D1d, D1n | none | 0x0003 | object_cs | npc-scheduled | yes |
| 54 | `ACTOR_EN_BOMBERS` (0x280) | (1056, 100, -549) | 0x0000 / 0x007F / 0x0030 | 0 / 0 / 0 | 0x030 D2d, D2n | none | 0x0003 | object_cs | npc-scheduled | no |
| 55 | `ACTOR_EN_BOMBERS` (0x280) | (912, 100, 211) | 0x0000 / 0x007F / 0x000C | 0 / 0 / 0 | 0x00C D3d, D3n | none | 0x0003 | object_cs | npc-scheduled | no |
| 56 | `ACTOR_EN_KANBAN` (0x0A8) | (1130, 372, -1163) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x003F | object_kanban | static-mesh | yes |
| 57 | `ACTOR_EN_IG` (0x276) | (1710, 60, -750) | 0x0007 / 0x877F / 0x007F | 0 / 270 / 0 | 0x3FF all | none | 0x0004 | object_dai | npc-scheduled | yes |
| 58 | `ACTOR_EN_TEST4` (0x15A) | (-280, 1103, -1020) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | 0x3FF all | 9 | 0x0C00 | gameplay_keep | controller/invisible | yes |
| 59 | `ACTOR_EN_BOX \| 0x6000` (0x006) | (479, 220, 495) | 0x0007 / 0x2D7F / 0x007F | 0 int / 90 / 0 int | 0x3FF all | none | 0x50CA | object_box | posed-skeletal | yes |
| 60 | `ACTOR_EN_PST` (0x1F2) | (1028, 200, -1401) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF all | none | 0x0002 | object_pst | posed-skeletal | yes |
| 61 | `ACTOR_EN_PST` (0x1F2) | (1243, 100, -64) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF all | none | 0x0003 | object_pst | posed-skeletal | yes |
| 62 | `ACTOR_EN_FALL` (0x17C) | (-280, 11000, -1020) | 0x1907 / 0x07FF / 0x007F | 50 / 15 / 0 | 0x3FF all | none | 0x037F | object_lodmoon (by slot) | static-mesh (camera-relative moon) | yes |
| 63 | `ACTOR_OBJ_SOUND` (0x0F0) | (-290, 0, 160) | 0x0001 / 0x007F / 0x0078 | 0 / 0 / 0 | 0x0F8 D1d, D1n, D2d, D2n, D3d | none | 0x0090 | gameplay_keep | controller/invisible | yes |

Only one tower gear is listed (a28, the east one); the west gear at (-492, 555, -1012) appears in alt2 only (INF: not visible from East in play, U-20). Tower, panel, turret and chimney coordinates and params equal South's and North's instances (ROM_REFERENCE.md; REF-05 4.2), consistent with REF-02's shared world frame.

### 4.3 Alternate room header 1 (37 entries, HDR room.c:343-482, decode C)

"Twin" = default index with the same actor, params and position; only differences are written out. Masks, banks and classes equal the twin's.

| Alt1 # | Actor | Twin / difference |
| --- | --- | --- |
| 0 | `EN_ELFORG` | 1 |
| 1, 2 | `EN_JA` x2 | 5, 7; **`OBJECT_BOJ` is not in the alt1 object list, so their spawn fails** (z_actor.c:3517-3523) |
| 3 | `OBJ_ETCETERA` (326, 120, -409) | 9 moved 24 u -X, 19 u -Z |
| 4 | `OBJ_ETCETERA` (454, 280, -930) | **new**: a second pink Deku flower on the Inn upper deck F13 (Y 280) |
| 5, 6 | `OBJ_KIBAKO` | 10, 11 |
| 7 | `OBJ_ENTOTU` | 13 |
| 8 | `EN_STOP_HEISHI \| 0x2000` | 26 |
| 9-11 | `OBJ_TOKEIDAI` x3 | 27-29 |
| 12 | `OBJ_TOKEI_STEP` | 30 |
| 13-19 | `OBJ_TOKEI_TURRET` x7 | 31-37 |
| 20-23 | `EN_LIGHT` x4 | 38-41 |
| 24 | `OBJ_BELL` | 45 |
| 25 | `EN_BOM_BOWL_MAN \| 0x2000` | 46; params `0x0010` (path 0) instead of `0x0510`, csId none instead of 12 |
| 26 | `EN_BOMBERS2` | 47; csId none instead of 11 |
| 27 | `EN_BOMJIMB` `0x0B22` | 48 |
| 28 | `EN_BOMJIMB` `0x0B11` (1076, 100, 75) yaw 332 | 49 moved from the walkway (491, 267, -678) yaw 46 to the plaza |
| 29, 30 | `EN_BOMBERS` `0x0002`, `0x0003` | 50, 53 |
| 31 | `EN_TEST4` | 58; params `0x0C20` instead of `0x0C00` (bell volume index 1, per REF-05) |
| 32, 33 | `EN_PST` | 60, 61 |
| 34 | `EN_FALL` | 62 |
| 35 | `EN_MAG` (0, 0, 0), params 0, `OBJECT_MAG` | **new**; "Title Logo" (ANN) |
| 36 | `OBJ_SOUND` | 63 |
| absent | | default 0 (En_An), 2 (En_Test3), 3-4 (En_Gm), 6 and 8 (D2 En_Ja), 12 (En_Pm), 14-25 (12 En_Talk), 42-44 (En_Hit_Tag), 51, 52, 54, 55 (Day 2/3 En_Bombers), 56 (sign), 57 (En_Ig), 59 (chest) |

### 4.4 Alternate room header 2 (12 entries, HDR room.c:513-550, decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_MK` (0x0AE) | (1143, 100, -106) | 0x0007 / 0x6BFF / 0x007F | 0 / 215 / 0 | 0x3FF all | none | 0xFFFF | object_mk | npc-scheduled |
| 1 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-280, 555, -833) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x207F | object_obj_tokeidai | static-mesh |
| 2 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-68, 555, -1012) | 0x0007 / 0x2D7F / 0x007F | 0 / 90 / 0 | 0x3FF all | none | 0x007F | object_obj_tokeidai | static-mesh |
| 3 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-492, 555, -1012) | 0x0007 / 0x877F / 0x007F | 0 / 270 / 0 | 0x3FF all | none | 0x007F | object_obj_tokeidai | static-mesh |
| 4 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-280, 1103, -1020) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x307F | object_obj_tokeidai | static-mesh |
| 5 | `ACTOR_OBJ_TOKEI_STEP` (0x1B8) | (-280, 255, -853) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_tokei_step | static-mesh |
| 6 | `ACTOR_OBJ_BELL` (0x14E) | (633, 284, -966) | 0x0007 / 0x097F / 0x007F | 0 / 18 / 0 | 0x3FF all | none | 0x0000 | object_f52_obj | static-mesh |
| 7 | `ACTOR_EN_JA` (0x244) | (1202, 100, -127) | 0x0007 / 0x76FF / 0x007F | 0 / 237 / 0 | 0x3FF all | none | 0x0001 | object_boj | npc-scheduled |
| 8 | `ACTOR_EN_JA` (0x244) | (1103, 100, -103) | 0x0007 / 0x70FF / 0x007F | 0 / 225 / 0 | 0x3FF all | none | 0x0001 | object_boj | npc-scheduled |
| 9 | `ACTOR_OBJ_ENTOTU` (0x14D) | (319, 553, -1308) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_f53_obj | billboard/effect |
| 10 | `ACTOR_OBJ_ENTOTU` (0x14D) | (-1080, 593, -1434) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_f53_obj | billboard/effect |
| 11 | `ACTOR_EN_MA_YTO` (0x220) | (1185, 100, -155) | 0x0007 / 0x79FF / 0x007F | 0 / 243 / 0 | 0x3FF all | none | 0x00FF | object_ma2 | npc-scheduled |

Alt2 chimney #10 is at Y 593, while North's chimney at the same X/Z is at Y 673 (REF-05 a06): a real source difference, purpose NI. Banks `OBJECT_OS_ANIME`, `NIW`, `CNE`, `DAIKU` have no listed alt2 consumer.

### 4.5 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init / draw facts used here | Presence at Day 1 12:00, fresh save |
| --- | --- | --- |
| En_An (a0) | `gAnju1Skel`, scale 0.01 (:3244-3256); params bit `0x8000` (`ENAN_8000`) | Day 1 schedule has no `SCENE_TOWN` branch (`scheduleScripts.schl:2-67`; at 12:00-12:15 it returns `ANJU_SCH_GIVE_LUNCH_TO_GRANNY` in `SCENE_YADOYA`): **absent** |
| En_Gm (a3, a4) | `object_in2_Skel_0078B0`, scale 0.01 (:1806-1813) | Day 1 `SCENE_TOWN` windows 09:10-10:05 and 12:15-12:55; 12:00-12:15 is `SCENE_SONCHONOIE` (`schl:13-49`): **absent at 12:00** |
| En_Ja (a5, a7) | `object_boj_Skel_00C240`, scale 0.01, no init kill (:419-438); schedule result 0 clears shadow/attention (:370-385) | Days 1-2 06:00-18:00 in `SCENE_TOWN` returns 1 (`schl:1-17`): **present** |
| En_Pm (a12) | `object_mm_Skel_0096E8`, scale 0.01; script by week-event flags (:1909, :380-398) | fresh script 0: 12:00-13:01 only `SCENE_POSTHOUSE` (`schl:70-75`): **absent** |
| En_Ig (a57) | `object_dai_Skel_0130D0`, scale 0.01; draw only if `scheduleResult != 0` (:1096) | Day 1 `SCENE_TOWN` starts 15:25 (`schl:1-20`): **absent** |
| En_Stop_heishi (a26) | `gSoldierSkel`, idle `gSoldierStandHandOnHipAnim`; type nibble 3; switch flag `p & 0x3F = 0x3F` kills if set; Z int 0 -> `unk_288 = 50` (:96-120) | **present** at (1402, 60, -703), 78 u west of the Termina Field exit floor; passage rule NI (U-10) |
| En_Bombers2 (a47) | `gBomberSkel`, scale 0.008, cylinder r35 h30 (2.5) | **present**, blocking the observatory corridor |
| En_Bom_Bowl_Man (a46) | `gBomberSkel`, scale 0.01; F0 = 1, path 5 (:136-167) | killed unless entrance East 2 + code + no Notebook: **absent** |
| En_Bomjimb (a48, a49) | `gBomberSkel`, scale 0.01; slot `(p >> 8) & 0xFF = 11` | killed unless `WEEKEVENTREG_73_10` or `85_02` and not `75_40` (:175-216): **absent** (same gate as North, REF-05 U-10) |
| En_Bombers (a50, a53) | `gBomberSkel`, scale 0.01; type `(p >> 4) & 0xF = 0` (`ENBOMBERS_TYPE_NPC`), number `p & 0xF` + 1 (:150-160) | NPC type killed only if `73_10` or `85_02`: **present**; a53 stands on the Milk Bar roof step F24 (Y 372) |
| En_Test3 (a2) | `KAFEI_GET_PATH_INDEX` macros (ANN); mask D3 only | excluded by mask |
| En_Talk x12 (a14-a25) | scale 1, text id `(p & 0x3F) + 0x1C00` (0x1C00-0x1C15), attention type `rot.x - 1` (6, or 7 which is out of range and ignored); offers talk within 40 u when faced (:29-58, `z_en_talk.h:6-7`) | present, invisible |
| En_Hit_Tag x3 (a42-a44) | scale 1, cylinder; killed if switch `(p & 0xFE00) >> 9 = 0x7F` set; on hit plays the rupee sfx and kills itself (:50-75) | present, invisible; all three cluster at (1381-1389, 212-250, -346..-199), above the Honey & Darling door TA4 |
| Obj_Kibako (a10, a11) | bank 1 `OBJECT_KIBAKO`, scale 0.15, opaque `gSmallCrateDL` 0x1180; collectible `p & 0x3F` = 0x01 / 0x0A, flag `(p >> 8) & 0x7F` = 1 / 2, "bomber can hide" bit 7 set on both (`z_obj_kibako.h:6-9`, .c:139-155, :427-428) | present |
| Obj_Bell (a45) | dynapoly `object_f52_obj_Colheader_001BA8`, scale 0.08; bell DLs 0x698, 0x8D0, 0x960, 0x7A8 under a swing matrix (local Y 2600 pivot, rot.x = 0 at rest); post 0x570 with shape yaw; XLU shadow 0x840 at Y - 4 (:218-270) | present, on the Inn upper deck F13 |
| Obj_Etcetera (a9) | type 0 pink flower, scale (0.01, 0.02, 0.01), `gPinkDekuFlowerIdleDL` 0xED80, `gPinkDekuFlowerCol` 0xE710 (per REF-05) | present, on plinth F03 (Y 120) |
| En_Kanban (a56) | scale 0.01; text id `p \| 0x300 = 0x033F` (:147-163); draw as REF-05 (`gSignMaterialDL` 0xC30 + `gSignRectangularDL` 0x5AED0, Human-form Y offset) | present, on F24 (Y 372) |
| En_Box (a59) | `ENBOX_GET_TYPE = 5` small, item `(p >> 5) & 0x7F = 0x06` `GI_RUPEE_SILVER` (`include/z64item.h:421`), chest flag 0x0A; small chest scale 0.0075; shape yaw += 0x8000 (90 -> 270); rot.z zeroed (`z_en_box.h:24-44`, .c:265-279); `gBoxChestSkel` 0x66A0, base 0x6F0, lid 0x12E8, `gBoxChestCol` 0x80E8 | present (contents state-dependent), on the southern rooftop F05 (Y 220) |
| En_Pst (a60, a61) | `gPostboxSkel` 0x1A80, scale 0.02 (per REF-05); script by params: 2 -> checked 10:35-10:39, 3 -> 10:53-10:57 (`schl`; `z_en_pst.c:617-623`) | available at 12:00: **drawn** |
| Obj_Tokei_Turret (a32, a35) | Day 1 pieces: base params 0 at Y 0 (`0x2508`), top params 1 at Y 80 (`0x2A88`) (ROM_REFERENCE.md) | present; South-district geometry listed in East |
| Obj_Tokeidai, Obj_Tokei_Step, Obj_Entotu, En_Fall, En_Test4 | as REF-05 4.4 and ROM_REFERENCE.md | present |
| En_Light (a38-a41), En_Elforg (a1) | night masks | excluded by mask |
| Obj_Sound (a63) | params 0x0090 at the turret base | present, no geometry |
| En_Door (TA0-TA6) | `gDoorSkel` 0x22B28 (gameplay_keep), limb 4 swapped for the style DL (2.4); front/back DL chosen by eye yaw (`z_en_door.c:640-660`) | present; all seven open at 12:00 (2.4) |

### 4.6 Counts

| | Default | Alt1 | Alt2 |
| --- | --- | --- | --- |
| Entries | 64 | 37 | 12 |
| Day 1 daytime candidates by mask | 47 (excluded: 1, 2, 6, 8, 31, 33, 34, 36, 37, 38-41, 51, 52, 54, 55) | 27 (excluded: 0, 13, 15, 16, 18, 19, 20-23) | 12 |
| static-mesh (incl. moon) | 17 | 17 | 6 |
| posed-skeletal | 3 | 2 | 0 |
| npc-scheduled | 21 | 9 | 4 |
| billboard/effect | 6 | 7 (incl. En_Mag) | 2 |
| controller/invisible | 17 | 2 | 0 |
| door/transition (scene list) | 7 | 7 | 7 (night type) |

## 5. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

Assumptions to record in the profile: fresh save (no week-event, switch or chest flags), Human form, no Room Key, normal entry (not East entrance 2). `world = actorPos + actorRot x (actorScale x localVertex)` (ROM_REFERENCE.md); Unity position by the documented conversion; rotation handedness belongs to the importer (PIPE-01/02).

| Source | Component / params | N64 position (Unity) | Yaw | Actor scale | Display list(s), bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| a27 | Tower clock `0x207F` | (-280, 555, -833) (-2.80, 5.55, 8.33) | 0 | 0.1 | ring 0xCF28, center/hand 0xBEE8, face 0xE818, sun/moon panel 0xC368 (object_obj_tokeidai) | noon pose per ROM_REFERENCE; visibility from East NI |
| a28 | East exterior gear `0x007F` | (-68, 555, -1012) (-0.68, 5.55, 10.12) | 90 | 0.1 | `gClockTowerExteriorGearDL` 0xBA78 | only gear listed |
| a29 | Counterweight `0x307F` | (-280, 1103, -1020) (-2.80, 11.03, 10.20) | 0 at init | 0.1 | 0xB208 | omit spotlight by day |
| a30 | Closed rooftop panel `0x0000` | (-280, 255, -853) (-2.80, 2.55, 8.53) | 0 | 0.1 | `gClocktowerPanelDL` 0x88 (object_tokei_step) | |
| a35 | Turret base `0x0000` | (-290, 0, 160) (-2.90, 0.00, -1.60) | 0 | 0.1 | `gClockTownTurretPlatformBaseDL` 0x2508 (object_tokei_turret) | South geometry |
| a32 | Turret top `0x0001` | (-290, 80, 160) (-2.90, 0.80, -1.60) | 0 | 0.1 | `gClockTownTurretPlatformTopDL` 0x2A88 | South geometry |
| a9 | Pink Deku flower `0x007F` | (350, 120, -390) (3.50, 1.20, 3.90) | 0 | (0.01, 0.02, 0.01) | `gPinkDekuFlowerIdleDL` gameplay_keep 0xED80 | collision 0xE710 |
| a10 | Crate `0x8181` | (1100, 100, -460) (11.00, 1.00, 4.60) | 0 | 0.15 | `gSmallCrateDL` object_kibako 0x1180 | breakable |
| a11 | Crate `0x828A` | (759, 100, -458) (7.59, 1.00, 4.58) | 0 | 0.15 | same | breakable |
| a45 | Bell `0x0000` | (633, 284, -966) (6.33, 2.84, 9.66) | 18 (post) | 0.08 | object_f52_obj 0x698, 0x8D0, 0x960, 0x7A8 (bell, rest pose), 0x570 (post), XLU 0x840 (shadow) | dynapoly 0x1BA8 |
| a56 | Sign `0x003F` | (1130, 372, -1163); Human-form draw Y 357 (11.30, 3.57, 11.63) | 0 | 0.01 | `gSignMaterialDL` object_kanban 0xC30 + `gSignRectangularDL` gameplay_keep 0x5AED0 | text 0x033F |
| a59 | Small chest `0x50CA` (posed) | (479, 220, 495) (4.79, 2.20, -4.95) | 270 (shape) | 0.0075 | `gBoxChestSkel` 0x66A0 closed frame: base 0x6F0, lid 0x12E8 (object_box) | label: posed skeleton; contents flag 0x0A |
| a60 | Postbox `0x0002` (posed) | (1028, 200, -1401) (10.28, 2.00, 14.01) | 180 | 0.02 | `gPostboxSkel` 0x1A80 idle (object_pst) | as South/North |
| a61 | Postbox `0x0003` (posed) | (1243, 100, -64) (12.43, 1.00, 0.64) | 180 | 0.02 | same | |
| TA0-TA6 | Seven closed doors (posed) | 2.4 positions | 2.4 yaws | NI | `gDoorSkel` + style DL per door (2.4) | side DL depends on camera |
| a62 | LOD moon `0x037F` (special) | (-280, 11000, -1020) | X50 Y15 Z0 | 0.384 | `gLodmoonEyesDL` 0x10E0 + `gLodmoonMoonDL` 0x1158 (object_lodmoon) | 9000-unit eye rule or label the approximation |

**Excluded by half-day mask:** a1, a2, a6, a8, a31, a33, a34, a36, a37, a38-a41, a51, a52, a54, a55.

**Excluded by state or schedule at 12:00 (fresh save):** a0 En_An, a3/a4 En_Gm, a12 En_Pm, a57 En_Ig (schedule none in `SCENE_TOWN`); a46 En_Bom_Bowl_Man, a48/a49 En_Bomjimb (init kill).

**Present but unimplemented in a static assembly:** skeletal NPCs a5 and a7 En_Ja (`object_boj_Skel_00C240`), a26 En_Stop_heishi (`gSoldierSkel`), a47 En_Bombers2 and a50/a53 En_Bombers (`gBomberSkel`); billboard a13 Obj_Entotu (chimney 0x158, smoke 0x1C00, smoke timing per REF-05 4.4).

**No geometry:** a14-a25 En_Talk, a42-a44 En_Hit_Tag, a58 En_Test4, a63 Obj_Sound.

## 6. Collision summary and vertical structure (C from scene.c:746-1676; scratch `col.py`, `connect.py`, `doors.py`)

### 6.1 Measures

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 413 / 489 / 21 |
| Water boxes | 0 (`0, NULL`) |
| Camera data (`BgCamInfo`) | 2: index 0 = `0x003D` `CAM_SET_DUNGEON3`, index 1 = `0x0001` `CAM_SET_NORMAL0` (`include/z64camera.h:148, 208`); index 0 is used only by surface types 0, 11, 12 (the 25 Milk Bar roof-deck floors), all other surfaces use index 1 (order is reversed relative to North) |
| Header bounds | min (-1225, 0, -2673), max (1960, 1183, 1020); Unity x -12.25..19.60, y 0..11.83, z -10.20..26.73 |
| Actual vertex extents | x -1120..1960, y 0..760, z -2653..600; Unity x -11.20..19.60, y 0..7.60, z -6.00..26.53 (header box is larger, U-18) |
| Polygon kinds | 148 floors (normal Y > 0.5; 117 under 10 deg, 31 steeper), 310 walls (297 with abs(nY) < 0.1), 31 ceilings |
| Floor slope histogram (5-deg bins) | 0: 117, 10: 4, 15: 8, 20: 2, 25: 2, 35: 7, 40: 4, 55: 4 |
| Materials (`data[1] & 0xF`) | STONE 180, DIRT 160, WOOD 149 polys |
| Exit surfaces | 13 types, 25 polys (section 2.3) |
| Floor property 9 | surface 0 only: 4 floors at Y 420 (x 1040..1200, z -1197..-911), the Milk Bar roof top. Leaving such a floor suppresses the auto-jump and the ledge-hang check (player.c:7539, 7569) |
| Wall type 1 (`WALL_FLAG_0`, `z_bgcheck.c:20`) | surfaces 6, 7: walls 216, 217 (x 1181..1294, z 95..208), 220, 221 (x 935..1048, z 341..454), both Y 220..308, and 238, 239 (x 360..793, z 380, Y 220..340). `WALL_FLAG_0` prevents a ledge-climb classification (player.c:11835) |
| Other flags | floorType 0, floorEffect 0, soft 0, horse 0, canHookshot 0 on all 21 types (**no hookshot surface in static collision**); echo 2, light 31 on all; conveyor only on surfaces 4, 8, 9, 10, 13; poly exclusion: 68 ignore-camera (surfaces 1, 3, 5, 14-20), 8 ignore-projectiles (1-3), 32 camera + projectiles (0, 1, 3, 11) |
| Slopes | with floorEffect 0 everywhere, `Player_HandleSlopes` never starts the slope slide (player.c:9385-9388); uphill pushback is `(1 - nY) x 40` (:9402-9409). Walkability of the 36.9-55.7 deg faces is UNKNOWN until a controller test (U-07) |

### 6.2 Floor vertex Y histogram (all 148 floors, 444 vertex references, 20 u bins)

| Y bin | 0 | 60 | 80 | 100 | 120 | 200 | 220 | 240 | 260 | 280 | 300 | 320 | 360 | 380 | 420 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Count | 18 | 15 | 21 | 117 | 6 | 72 | 31 | 1 | 11 | 41 | 28 | 18 | 29 | 12 | 24 |

### 6.3 Distinct floor levels

Floors were joined when an edge sample 3 u outward lands on another floor within 2 u in Y (seamless surface); that gives 26 surfaces (F00-F25). F00 (76 polys) is one seamless walking surface that spans several levels through ramps; it is split below by elevation. "Reach" uses the human-form ledge numbers from `sPlayerAgeProperties` (player.c:742+, entry 4: `unk_0C` 71, `unk_14` 49, `unk_18` 39, `unk_1C` 27) and the ledge test (player.c:11810-11850): a rise under 17.8 u is no ledge, 17.8 to about 71 u is a climbable ledge unless the wall is type 1, above about 71 u is not climbable (INF from code, to be confirmed by controller test, U-08).

| Level | Y | Floors (count) | Extent x / z | Joined by | Exits on it |
| --- | --- | --- | --- | --- | --- |
| L0 South-frame strip | 0 and 100 | 380-388 (9) | -1120..347 / -440..600 | seamless with L1 at x 347 | none; alt2 spawn |
| L1 lower South ramp | 0..100 | 461, 462, 467, 468 (4) | 347..720 / 120..360 | 15 deg ramp to L4 | slot 1 (461/462) |
| L2 Termina Field gate | 60 | 294, 295 (18.4 deg from L4), 297-300 (6) | 1280..1720 / -800..-620 | ramp from L4; ceilings 70/71 at Y 300 overhead | slot 0 (299/300) |
| L3 observatory tunnel | 80..200 | 112, 113, 106, 107 (26.6 deg), 118, 119, 116, 117 (8) | 1400..1960 / -1824..-1764 | corridor from L5 (Y 200) | slot 2 (116/117) |
| L4 main plaza | 100 | 279-285, 290-293, 296, 301-306, 469-472, 477-482 (28) | 200..1480 / -991..360 | hub | slots 3, 4, 6, 8, 9, 12; wall 169 |
| L4a plinths | 116, 120 | F01 330/331 (x 400..434), F02 322/323 (x 1345..1375), F03 192/193 (x 200..400, z -480..-160) (6) | | F01/F02: 16 u step; F03: 20 u ledge from L4 | none; Deku flower a9 on F03 |
| L5 north terrace | 200 | 286-289, 307-313, 475, 476, ramp 463/464 (15.2 deg, x 760..960) (15) | 520..1480 / -2080..-1280 | ramp from L4 | slots 5 (286/287), 7 (475/476) |
| L6 east walkway | 200..300 | ramp 465/466 (20.1 deg, x 1280..1400, z -1377..-1103), 78-80 (Y 300, z -1103..-800) (5) | 1280..1400 / -1377..-800 | ramp from L5 | none; En_Bomjimb a48 (absent) |
| L7 Milk Bar roof deck | 300..420 | 428 + F17-F20 (36.9 deg faces, flat top Y 360), F23 (Y 360..388 lip, 45 deg), F24 (Y 372), F25 (Y 420) (25) | 960..1280 / -1377..-898 | L6 -> 428 -> F20 (+2) -> F24 (+12 step) -> F25 (+48 ledge) | none; sign a56 and Bomber a53 on F24; alt1 spawn on F25 |
| L8 Inn upper deck | 239..328 | F08 170-172 (ramp Y 239..280), F13 133, 134, 483, 488, 473, 474 (Y 280), F22 158, 159, 162, 163 (Y 328) (13) | 400..760 / -1360..-603 | F08 <-> F13 6 u step; F13 -> F22 +48 ledge; no floor link from L4 (rise 144-180 u) | slot 10 (473/474) behind TA1; bell a45 |

Unreached tops (no seamless, step or under-71 u link from any level above; balcony/roof candidates only):

| Surface | Y | Floors | Extent x / z | Rise from L4 | Note |
| --- | --- | --- | --- | --- | --- |
| R1 (F04, F07, F14, F15) | 214, 234, 281 | 30-33, 46, 51, 42, 43, 58, 59, 64, 65, 15, 16, 24, 25 (16) | 1354..1402 / -371..-175 | 114-181 | above the Honey & Darling door; En_Hit_Tag a42-a44 and En_Talk a22 are here; ceilings 34-37 at Y 200 below |
| R2 south band (F05, F06, F16) | 220, 220, 288 | 199, 200, 210, 211, 201-203 (7) | 200..1400 / -40..600 | 120 (F16 188) | **chest a59 on F05**; F05/F06 -> F16 (+68) is blocked by type-1 walls 216/217, 220/221; ceilings 224/225 (Y 228) and 252/253 (Y 308) over the gallery door area |
| R3 (F09-F12) | 260..320 | 188-191 (4), 55.7 deg | 200..720 / -160..120 | 160-220 | paired roof faces over the footprint that holds the Treasure Chest Shop exit 481/482 (INF) |
| R4 (F21) | 300 | 76, 77 (2) | 1280..1400 / -620..-500 | 200 | not joined to the L6 walkway (78-80 end before z -620) |

Small ceilings 332-349 (Y 328..342, around (934, -460)) and 356-373 (Y 372..387, around (1146, -1712)) sit under the night `En_Light` pairs a38/a40 and a39/a41 (INF: lamp heads).

### 6.4 Balcony, roof and access summary (only what collision or actors show)

- **Inn upper deck (L8):** the Inn's second street door TA1 and the bell are on it; it has no foot link to the plaza in collision, and its ramp end F08 (Y 239) is a 144-177 u drop. INF: reached from the street through the Inn interior (enter TA0, leave by the Inn's upper door), or by alt1's second Deku flower (alt1 only). Inn interior stair connectivity is INT-07 work (U-05).
- **Milk Bar roof deck (L7):** reached on foot from the north terrace through the east walkway (L6) if the 36.9 deg faces are walkable (U-07); uses the DUNGEON3 camera; the Milk Bar door TA6 (z -931) and its exit (z -991..-911) sit under the deck's south edge (INF: the deck is the Milk Bar roof).
- **Southern rooftop chest (R2):** no foot route, no hookshot surface, and type-1 walls stop ledge climbs onto F16. Route UNKNOWN (U-06).
- **Form/story restrictions, separate from movement:** the corridor gate a47 (Bombers code); the Termina Field gate soldier a26 (rule NI); the Deku flower a9 (Deku form launch, destination UNKNOWN); door schedules (all open at 12:00; closures in 2.4); Inn kickout and other kickouts (REF-02).
- Everything else (facades, signage, balcony railings, rooftop dressing, awnings): **visual survey pending capture**.

### 6.5 Proposed camera anchors (REF-03 IDs, proposals only)

| Camera ID | Source anchor |
| --- | --- |
| `cam.east_clock_town.upper_route` / `arrival` | spawn 3 (406, 100, -777) facing +X, under ceilings 152/153 |
| `cam.east_clock_town.lower_route` | spawn 1 (697, 94, 205) facing +X, top of ramp 461/462 |
| `cam.east_clock_town.closeup` | Inn walkway underside: ceilings 173-175 over posts 164-169 at (592, 100..245, -634) (also documents the poly-169 site) |
| `cam.east_clock_town.observatory_route_access` | corridor mouth 112/113 (x 1400..1480, Y 200) facing +X, with a47 |
| `cam.east_clock_town.elevated_sightline` | Milk Bar roof top F25 (Y 420) or Inn deck F13 (Y 280) facing the plaza |
| `cam.east_clock_town.frontage_*` | each door TA0-TA6 (2.4), from its street side |

## 7. Materials and textures (C; counts only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 87: 78 textures + 9 TLUTs |
| By format | rgba16 68 (59 textures + 9 TLUTs), ci4 9, i4 7, i8 1, ia8 1, ia16 1 |
| Location | all in the scene file (segment 2); the room file holds none; all 87 are referenced by the room DLs (grep) |
| Room `Gfx` display lists / `Vtx` arrays | 297 / 163 (163 `.vtx.inc` files) |
| Room shape | cullable, 6 entries: 6 opaque DLs, 5 XLU DLs (entry 0 has none) |
| XML-declared DLs | `0x960` empty; `0x12D98` calls the 6 opaque entry DLs; `0x17668` calls the 5 XLU entry DLs |
| Raw triangle commands | 739 `gsSP2Triangles` + 55 `gsSP1Triangle` = 1,533 triangles in source (DL reuse not resolved) |
| Animated materials | none effective (section 1) |

## 8. Required extraction list for the East pass (commands NOT executed)

Done: scene `Z2_TOWN` (C above). Already extracted under `extracted/n64-us/assets/objects/`: `object_obj_tokeidai`, `object_tokei_step`, `object_tokei_tobira`, `object_tokei_turret`. Binaries for every bank below exist under `extracted/n64-us/baserom/` (checked by `stat`; sizes in bytes): object_kibako 6,912; object_f52_obj 7,136; object_kanban 6,064; object_lodmoon 15,904; object_box 33,056; object_pst 6,800; object_wdor01 5,808; object_wdor03 5,808; object_wdor04 5,744; gameplay_keep 599,520; object_f53_obj 14,960; object_boj 79,808; object_sdn 56,464; object_cs 68,480.

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_kibako`, `object_f52_obj`, `object_kanban`, `object_lodmoon`, `gameplay_keep` | section 5 DLs and collision |
| Doors and posed pieces | `object_wdor01`, `object_wdor03`, `object_wdor04`, `object_box`, `object_pst` (gameplay_keep covers TA2-TA4 and `gDoorSkel`) | venue doors, chest, postboxes |
| Billboard approximation | `object_f53_obj` | chimney |
| Later skeletal pass | `object_boj`, `object_sdn`, `object_cs` | noon NPCs |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_kibako','object_f52_obj','object_kanban','object_lodmoon','gameplay_keep','object_wdor01','object_wdor03','object_wdor04','object_box','object_pst','object_f53_obj')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

`gameplay_keep` is shared with the North list (REF-05 section 8); extract it once and check output size and warnings separately.

## 9. Proposed region record

**Proposal, schema not frozen (PIPE-01).** Field set follows FULL_CITY_PLAN section 3.2; `null` where source does not settle a value.

```json
{
  "_status": "proposal, schema not frozen (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.east_clock_town",
  "displayLabel": "East Clock Town",
  "source": {
    "sceneSymbol": "Z2_TOWN",
    "sceneEnum": "SCENE_TOWN",
    "sceneTableIndex": "0x6C",
    "entranceTableSceneIndex": "0x69",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0)",
    "rooms": { "loc.east_clock_town.room_00": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event, switch or chest flags", "player form Human", "no Room Key", "entrance is not ENTRANCE(EAST_CLOCK_TOWN, 2)"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TOWN", "bytes": 124784, "sha256": "88283336c683365400b325ec8d7acc50e5e1ddab625a44e9318efc72fbe20625" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TOWN_room_00", "bytes": 95904, "sha256": "600cffd3555d0936ddbb5b6852fd7358c4065daba18d0b1841cf9258f20c03b0" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TOWN/Z2_TOWN.c", "sha256": "44ae67320d625b2346600a47fe91083c20d870242fd123356ef1fac9308d25d8" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TOWN/Z2_TOWN_room_00.c", "sha256": "1d697543e05154d3359b462552150d3bbed850b9be75f1a320d89ec6489b130f" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_TOWN/Z2_TOWN.xml", "sha256": "7fa1b533bcd31a5f8b5dc0f7bbea4558c0e0808743f76f88a2cbb9c13d53ced3" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedWithSouth": "INFERRED (identical tower/turret/chimney anchors; REF-02 exit overlap), unverified" },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-1225, 0, -2673], "max": [1960, 1183, 1020] },
    "sourceCollisionVertices": { "min": [-1120, 0, -2653], "max": [1960, 760, 600] },
    "unityCollisionVertices": { "min": [-11.20, 0.00, -6.00], "max": [19.60, 7.60, 26.53] }
  },
  "anchors": [
    { "id": "anchor.ect.spawn3_south_upper", "source": [406, 100, -777], "unity": [4.06, 1.00, 7.77], "status": "proposed, not surveyed" },
    { "id": "anchor.ect.spawn1_south_lower", "source": [697, 94, 205], "unity": [6.97, 0.94, -2.05], "status": "proposed, not surveyed" },
    { "id": "anchor.ect.spawn2_tunnel", "source": [1762, 80, -1793], "unity": [17.62, 0.80, 17.93], "status": "proposed, not surveyed" },
    { "id": "anchor.ect.spawn10_inn_deck", "source": [477, 280, -1270], "unity": [4.77, 2.80, 12.70], "status": "proposed, not surveyed" },
    { "id": "anchor.ect.ta5_mayor_door", "source": [1100, 200, -2020], "unity": [11.00, 2.00, 20.20], "status": "proposed, not surveyed" }
  ],
  "rooms": [ { "id": "loc.east_clock_town.room_00", "sourceRoom": "Z2_TOWN_room_00", "roomShape": "cullable, 6 entries", "displayLists": 297 } ],
  "components": [
    { "sourceKey": "Z2_TOWN/h0/room_00/a27", "actorId": "0x19C", "params": "0x207F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "0xBEE8", "0xE818", "0xC368"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a28", "actorId": "0x19C", "params": "0x007F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xBA78"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a29", "actorId": "0x19C", "params": "0x307F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xB208"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a30", "actorId": "0x1B8", "params": "0x0000", "class": "static-mesh", "dls": ["object_tokei_step:0x88"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a35", "actorId": "0x221", "params": "0x0000", "class": "static-mesh", "dls": ["object_tokei_turret:0x2508"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a32", "actorId": "0x221", "params": "0x0001", "class": "static-mesh", "dls": ["object_tokei_turret:0x2A88"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a09", "actorId": "0x183", "params": "0x007F", "class": "static-mesh", "dls": ["gameplay_keep:0xED80"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a10", "actorId": "0x081", "params": "0x8181", "class": "static-mesh", "dls": ["object_kibako:0x1180"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a11", "actorId": "0x081", "params": "0x828A", "class": "static-mesh", "dls": ["object_kibako:0x1180"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a45", "actorId": "0x14E", "params": "0x0000", "class": "static-mesh", "dls": ["object_f52_obj:0x698", "0x8D0", "0x960", "0x7A8", "0x570", "0x840"], "status": "selected, rest pose" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a56", "actorId": "0x0A8", "params": "0x003F", "class": "static-mesh", "dls": ["object_kanban:0xC30", "gameplay_keep:0x5AED0"], "status": "selected, Human-form Y" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a59", "actorId": "0x006", "params": "0x50CA", "class": "posed-skeletal", "dls": ["object_box:0x6F0", "object_box:0x12E8"], "status": "selected, closed pose" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a60", "actorId": "0x1F2", "params": "0x0002", "class": "posed-skeletal", "dls": ["object_pst:gPostboxSkel 0x1A80"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a61", "actorId": "0x1F2", "params": "0x0003", "class": "posed-skeletal", "dls": ["object_pst:gPostboxSkel 0x1A80"], "status": "selected" },
    { "sourceKey": "Z2_TOWN/h0/ta0-ta6", "actorId": "0x005", "params": "0x0287-0x0291", "class": "door/transition (posed-skeletal)", "dls": ["gameplay_keep:gDoorSkel 0x22B28", "per-door style DL (section 2.4)"], "status": "selected, closed" },
    { "sourceKey": "Z2_TOWN/h0/room_00/a62", "actorId": "0x17C", "params": "0x037F", "class": "static-mesh (camera-relative)", "dls": ["object_lodmoon:0x10E0", "object_lodmoon:0x1158"], "status": "selected, special rule" }
  ],
  "collisionOwnership": { "room": "Z2_TOWNCollisionHeader_0039C8 (489 polys)", "actorDynapoly": ["a09 gPinkDekuFlowerCol", "a45 object_f52_obj_Colheader_001BA8", "a59 gBoxChestCol", "TA0-TA6 door dynapoly (NI)"], "actorCylinders": "NI" },
  "portalIds": [
    "portal.east_clock_town.south_clock_town.upper",
    "portal.east_clock_town.south_clock_town.lower",
    "portal.east_clock_town.north_clock_town",
    "portal.east_clock_town.z2_00keikoku",
    "portal.east_clock_town.astral_observatory.p116",
    "portal.east_clock_town.stock_pot_inn.lower",
    "portal.east_clock_town.stock_pot_inn.upper",
    "portal.east_clock_town.mayors_residence",
    "portal.east_clock_town.milk_bar",
    "portal.east_clock_town.honey_darling",
    "portal.east_clock_town.treasure_chest_shop",
    "portal.east_clock_town.town_shooting_gallery",
    "portal.east_clock_town.north_clock_town.bomjimb"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a05/a07 En_Ja", "a26 En_Stop_heishi", "a47 En_Bombers2", "a50/a53 En_Bombers", "a13 Obj_Entotu billboard and smoke", "Link and Tatl"],
  "excluded": { "halfDayMask": ["a01", "a02", "a06", "a08", "a31", "a33", "a34", "a36", "a37", "a38", "a39", "a40", "a41", "a51", "a52", "a54", "a55"], "stateOrSchedule": ["a00", "a03", "a04", "a12", "a57", "a46", "a48", "a49"], "noGeometry": ["a14-a25", "a42-a44", "a58", "a63"] },
  "unsupported": ["skeletal NPC poses", "door side-DL selection by camera", "billboard orientation", "smoke vertex alpha and texture scroll", "conveyor exit floors", "floor property 9 behavior", "DUNGEON3 camera on the Milk Bar roof deck", "moon 9000-unit rule unless implemented"],
  "evidence": ["docs/city/districts/east_clock_town.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

`portal.east_clock_town.astral_observatory.p169` is left out of `portalIds` on the evidence in section 2.5; the integrator decides (U-03).

## 10. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | What selects East layer 2 | Which cutscene script or CS destination outside the 22 extracted city scenes holds `nextEntrance 0xD202` (or a cutscene index `0xFFF1` with `ENTRANCE(EAST_CLOCK_TOWN, 0)`)? Extract or grep the remaining scenes' cutscene lists |
| U-02 | Purpose of layer 1 | Confirm the chain South alt -> Inn alt1 (`0xBC01`/`0x5E01` in `Z2_CLOCKTOWER.c`) -> East alt1 (`0xD201`) -> South alt1 (`0xD801`) is the title attract loop: read that South cutscene entry and `ovl_title`/`z_opening.c` |
| U-03 | **Disagreement with REF-02:** wall poly 169 listed as portal `...astral_observatory.p169` | Section 2.5 finds no player path that tests a wall poly for an exit. Confirm with `BgCheck` poly classification in `z_bgcheck.c` (floor/wall split for nY = -0.03) and that no actor in any East header passes a wall poly to `Player_HandleExitsAndVoids`; then REF-02's owner retires or keeps the row |
| U-04 | Spawn bgCam indices 0x02-0x0C exceed the 2-entry `BgCamInfo` list | Same as REF-05 U-02: which code consumes `PLAYER_GET_BG_CAM_INDEX`; do these index the 15-entry actor cutscene camera list |
| U-05 | Foot access to the Inn upper deck (L8) | INT-07: does `Z2_YADOYA` room_00 collision connect spawn 0 (Y 0) to the upper door at (-60, 210, 0) by stairs; any other East route (dynapoly, flower) at 12:00 |
| U-06 | Access to the southern rooftop chest (R2, a59) | Is any hookshot or Deku launch target near (479, 220, 495) (actors in other headers, the flower a9 trajectory)? Then a route capture |
| U-07 | Walkability of the 36.9 deg Milk Bar roof faces and 55.7 deg roof faces | Controller test on the reference collision; read `Player_HandleSlopes` pushback limits against human run speed |
| U-08 | Human ledge thresholds (17.8 / 71 u) are inferred from code | Controller test of the 20 u (F03), 48 u (F13 -> F22, F24 -> F25) and 60-68 u rises |
| U-09 | En_Bombers2 talk flow and when it steps aside | Read `z_en_bombers2.c:140-410` (code entry, `hasMovedAside`) for conditions beyond the flag |
| U-10 | Termina Field gate soldier rule (a26, type 3) | Same as REF-05 U-08: read `EnStopheishi` actions for form/time checks with type nibble 3 |
| U-11 | Door hours beyond 12:00 | `if_since_time` end bound in the schedule interpreter (`src/code/z_schedule.c` NI); full open-hour table per door for the other REF-03 profiles |
| U-12 | Alt2 reachability of exit+1 = 11 / 13 | Read alt2 cutscene data `Z2_TOWNCutsceneData_004A80` (player cues) to see whether the player is ever free in alt2 |
| U-13 | South-frame strip polys 380-388 | Overlay with South collision (shared frame): do they coincide with South floors near (-573, 0, -336); confirm only alt2 uses them |
| U-14 | Spawn 0 (1346, 78, -708) sender | Extract `Z2_00KEIKOKU` and find `0xD200`; REF-02 lists East 0 as teleport-only |
| U-15 | Entrance 12 declared without an `EntranceEntry` | REF-01 U-04: search `ENTRANCE(EAST_CLOCK_TOWN, 12)` and `0xD2C0` |
| U-16 | Banks with no East consumer (`NIW`, `OS_ANIME`, `WDOR02`; `AN2` secondary) | Which runtime-spawned actors in East load them (`En_Niw` children, schedule NPC children) |
| U-17 | Visual survey | Capture all REF-03 `cam.east_clock_town.*` views under `prof.d1.noon.normal`: facades, signage, balcony railings, roofs, the Inn deck and the tunnel mouth |
| U-18 | Collision header bounds larger than vertex extents | Same as REF-05 U-14 |
| U-19 | What the 12 En_Talk spots and 3 En_Hit_Tag targets are attached to | Text 0x1C00-0x1C15 in the message data (NI) and capture |
| U-20 | Only the east tower gear in the default list | Compare South's list (two gears) and check visibility from East spawns |
| U-21 | Persistent cycle flags `SET(1, 0, 0, 0)` and chest flag 0x0A semantics | Read `z_sram_NES.c` cycle-flag handling for switch0 bit 0 and whether chest 0x0A persists |

Register cross-checks (no edits made): counts agree with `docs/city/LOCATION_REGISTER.md` row `loc.east_clock_town` (25/64, 17/37, 11/12 objects/actors; 12/1/1 spawns; 14/14/10 exits; 7 transition actors; 12 / 13 entrances). REF-01's inference for slot 7 (Mayor's) is confirmed by the door probe (C). REF-02's door associations ("nearest EN_DOOR 20u") are confirmed by the probe for all seven doors. REF-02 section 3.3 lists exit+1 = 3 with one centroid over two separate sites; this doc splits them (2.3, 2.5). REF-02 U8's orphan South door at (1114, 100, 274) coincides exactly with East TA3, consistent with the shared frame. SOURCE_REGISTER room time settings for East L1 (21, 1, 3) and L2 (17, 0, 1) agree.

**Not inspected:** room DL contents, combiners and geometry; light-setting values; cutscene data semantics (all three headers) and actor cutscene entries; path usage per actor; En_An, En_Gm, En_Ig, En_Pm behavior beyond schedule gates; En_Mk, En_Ma_Yto, En_Mag behavior; door dynapoly and actor collider shapes beyond those quoted; message text; `Z2_00KEIKOKU`; venue interiors (INT-07 to INT-12, INT-15); 3DS material.

**Checks run:** read the listed docs and registers; read scene/room C, XML, scene/actor/object tables, `z_actor.c`, `z_scene.c`, `z_scene_table.c` (East rows), `z64bgcheck.h`, `z_bgcheck.c` (wall flags, exit getters), `z64player.h`, `player.c` (exit handler, door probe, slopes, ledges, age properties), `z_en_door.c/.h` and its `scheduleScripts.schl`, and the overlays named in section 4.5 including the En_An, En_Gm, En_Ja, En_Ig, En_Pm and En_Pst schedule scripts; scratch Python (`col.py`, `levels.py`, `connect.py`, `actors.py`, `doors.py`, `near169.py`, `misc.py`, `f00.py`, `rows.py`, outputs in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref07\`) for collision counts, floor surfaces and relations, the 64/37/12 entry decode, the door floor probe and the poly-169 neighborhood; SHA-256 of the five inputs; grep for `setupExitList` readers, `EAST_CLOCK_TOWN` and `0xD201`/`0xD202`.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes; no file outside this document was modified.

**Next dependency:** REF-02 owner to rule on p169 (U-03); INT-07 Inn interior connectivity (U-05); REF-03 capture of the East camera set (U-17); PIPE-01 schema review of section 9; then the section 8 extraction and ECT-01 assembly.
