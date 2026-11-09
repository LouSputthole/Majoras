# Astral Observatory and underground route (`loc.astral_observatory`): source inventory dossier

Task **REF-09 / INT-15** (FULL_CITY_PLAN.md sections 6 and 12; special-space pilot candidate for ROOM-03). Base commit `31b20dc65b5137de012fa60d373fb609c7af0aca` on `main` (HEAD when written: `7a7a08c`, PIPE-01; section 10 follows the frozen `shared/city/` contract it added). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_TENMON_DAI` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save, Human form).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`, `shared/` or `tools/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_TENMON_DAI/Z2_TENMON_DAI.c`, `r00.c` / `r01.c` = `.../Z2_TENMON_DAI_room_00.c` / `.../Z2_TENMON_DAI_room_01.c`, `player.c` = `src/overlays/actors/ovl_player_actor/z_player.c`, `bji.c` = `src/overlays/actors/ovl_En_Bji_01/z_en_bji_01.c`; other overlays are `ovl_<Name>/z_<name>.c` under `src/overlays/actors/`. Collision polygon `i` is written at scene.c line `197 + i`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. Scratch scripts and outputs: `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\astral_observatory\`.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro/function; **OVL** read from the actor overlay source; **ANN** decomp-author comment or symbol name; **INF** inferred (reason given); **NI** not inspected.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_TENMON_DAI` / `SCENE_TENMON_DAI` | HDR `include/tables/scene_table.h:89` |
| Scene-table index | `0x29` | same line |
| Entrance-table scene index | `0x26` (`ENTR_SCENE_ASTRAL_OBSERVATORY`) | HDR `src/code/z_scene_table.c:2611`; `include/z64scene.h:631` |
| Entrance table | e0 `{SCENE_TENMON_DAI, 0, 0x0102}`, e1 `{.., 1, 0x4102}`, e2 `{.., 2, 0x8115}`; one layer each | HDR z_scene_table.c:761-777 |
| Title / draw config | `0x114` / `SCENE_DRAW_CFG_MAT_ANIM` | HDR scene_table.h:89 |
| Restrictions | `RESTRICTIONS_SET(0,0,0,0,0,0,0,3,3,0,0,0)`: field 8 `songOfSoaring` = 3 and field 9 `songOfStorms` = 3; all other fields 0 (usable) | C `src/code/z_parameter.c:60-71` (field order, "0 = usable") |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_NONE` | HDR scene_table.h:89 |
| XML files | scene seg 2; `Z2_TENMON_DAI_room_00` seg 3 with DLs `0xBCF8`, `0x11AF8`; `Z2_TENMON_DAI_room_01` seg 3 with DLs `0xC848`, `0x17580` | XML `assets/xml/scenes/Z2_TENMON_DAI/Z2_TENMON_DAI.xml` |
| Room count | 2 (`SCENE_CMD_ROOM_LIST(2, ...)`) | HDR scene.c:16, :48-51 |
| Scene / room headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene.c:14-31, r00.c:14-23 or r01.c:14-23 | HDR |
| Special files | `NAVI_QUEST_HINTS_NONE`, keep `GAMEPLAY_DANGEON_KEEP` | HDR scene.c:21 |
| Sound | spec 5, ambience 19, seq 59 = `Sequence_59` `NA_BGM_CAVERN`; room_01 `Obj_Sound` adds seq `0x3A` `NA_BGM_ASTRAL_OBSERVATORY` (4.4). Ordinal assumed from table order | HDR scene.c:15; `include/tables/sequence_table.h:136, 138` |
| Skybox | external file 0, skybox 0, weather 0, lightMode `true`; both rooms `SKYBOX_DISABLES(true, true)` | HDR scene.c:23, r00.c:17, r01.c:17; macros `include/z64scene.h:833-837` |
| Env light settings | 4 entries; floors select index 0 (room_00) or 1/2 (room_01) per surface (7.3) | HDR scene.c:65-74 |
| Region | `SCENE_CMD_SET_REGION_VISITED`; scene listed under `REGION_CLOCK_TOWN` | HDR scene.c:18; C `src/code/z_inventory.c:428-436` |
| Actor cutscenes / cs cameras | 16 entries / 5 cameras: cam 0 `CAM_SET_START0` (44), cams 1-4 `CAM_SET_FIXED1` (31) | HDR scene.c:76-93, :126-132; `include/z64camera.h:178, 191` |
| Minimap | 2 entries, both mapId `0xFFFF`; one chest icon (room 0, flag 0, at (-636, -579, 3088)) = room_00 a23 | HDR scene.c:95-106 |
| Animated materials | 6 entries, all type 1 (`AnimatedMat_DrawTwoTexScroll`), segments 1-6 drawn as 8-0xD (`segment + 7`); room_01 DLs call segments 8, 9, 0xA, 0xC, 0xD; room_00 calls 0xB | HDR scene.c:138-175; C `src/code/z_scene_proc.c:389-406`; grep of r00.c/r01.c |
| Transition actors | 2 (`EN_DOOR`, `EN_HOLL`), section 2.4 | HDR scene.c:43-46 |
| Room commands | room_00: echo 7, `ROOM_BEHAVIOR(0,0,0,0,true,0)` (point lights on), `TIME_SETTINGS(255,255,3)`, cullable shape 12 entries, 15 objects, 24 actors. room_01: identical except `enablePosLights` false, cullable 20 entries, 10 actors | HDR r00.c:14-23, r01.c:14-23; macro z64scene.h:804-807; `src/code/z_scene.c:246-253` |

### 1.1 Which room is which (C + INF)

| Room | Role | Evidence |
| --- | --- | --- |
| `loc.astral_observatory.room_00` | **Underground route**: east-end ramp, water channel, ladder, bomb wall, chest. It is the space behind East's Bombers gate | Entrance row 0 `{spawn 0, room 0}` is the East arrival (scene.c:54); polygons 0-656 (surface types 0-6) all lie at z > 440; every water box is `room 0`; torches, drips, Skulltulas, bomb wall, chest (HDR r00.c:43-116). The East guard's reaction set is named `FACE_REACTION_SET_BOMBERS_HIDEOUT_GUARD` (ANN, `ovl_En_Bombers2/z_en_bombers2.c:193`) |
| `loc.astral_observatory.room_01` | **Observatory chamber**: basement at Y -480, door level Y -129, telescope deck Y -84, upper structure to Y 143 | Rows 1 and 2 `{spawn 1/2, room 1}` (scene.c:55-56); polygons 657-1438 (surface types 7-27) all at z < 440; the Termina Field door (room 1 on both sides); Shikashi, scarecrow, cucco, Moon's Tear display, observatory BGM (HDR r01.c:43-74) |

The rooms meet at the `EN_HOLL` plane z = 440 inside a straight 120 u corridor (2.4). Both rooms load the same 15 object banks (section 3), so the room swap changes no bank (C). LOCATION_REGISTER 4.3 reached the same split by inference; the collision partition confirms it (C).

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:34-41; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`); yaw `(rawRotY >> 7) & 0x1FF`. The entrance list (scene.c:53-58) maps entrance n to `{spawn, room}`.

| Spawn | Position | Raw rot X / Y / Z | Yaw | Params | Mode | bgCam | Entrance -> room | Arrival use |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (602, -472, 4125) | 0x0007 / 0x5A00 / 0x007F | 180 | 0x0DFF | 0xD | none | e0 -> room 0 | East exit `0x4C00` (`Z2_TOWN.c:99` default, `:1729` alt 1). On ramp poly 239 (18.4 deg), 35 u before exit polys 233/234 (z 4160), facing -Z into the tunnel; `Player_StartMode_D` walk-in (player.c:8677-8681) |
| 1 | (-6, -129, 133) | 0x0007 / 0x5A00 / 0x007F | 180 | 0x0E00 | 0xE | 0x00 | e1 -> room 1 | `0x4C10` from Termina Field (sender NI). On poly 739 (Y -129), 55 u inside TA0, facing -Z; `Player_StartMode_E` walk-in at speed 2 (player.c:8683-8690) |
| 2 | (107, -84, -209) | 0x0007 / 0x2180 / 0x007F | 67 | 0x0DFF | 0xD | none | e2 -> room 1 | `0x4C20` telescope return (player.c:8630-8631); "Telescope entrance" case of Shikashi (ANN bji.c:397). On deck poly 709 (Y -84), 45 u from Shikashi (room_01 a00) |

The fourth entrance row `{0x00, 0x00}` ends at offset 0xF0 where the exit list begins and has no entrance-table row (only e0-e2, z_scene_table.c:773-777): padding (INF), matching LOCATION_REGISTER "4 (last pad?) / 3". No arrival lies inside its own departure trigger (spawn 0 is outside z 4160..4320; spawn 1 is outside z 188..286) (C).

### 2.2 Exit list (HDR scene.c:60-63) and consumers

The list is bounded by the LightSettings symbol at offset 0xF4, so it holds exactly 2 entries (C, symbol offsets 0xF0 and 0xF4).

| Exit idx | Raw | Decode (`Entrance_GetTableEntry`, z_scene_table.c:2688-2694) | Destination row | Consumer |
| --- | --- | --- | --- | --- |
| 0 | `0xD220` | 0x69 `Z2_TOWN`, entrance 2, layer 0 | `sEastClockTownEntrance2` `{SCENE_TOWN, 2, 0x4102}` (z_scene_table.c:2208-2210) -> East spawn 2 (1762, 80, -1793) yaw 270 params 0x0FFF (`Z2_TOWN.c:45-46`) | surface 3, polys 233/234, room_00 |
| 1 | `0x5490` | 0x2A `Z2_00KEIKOKU`, entrance 9, layer 0 | `sTerminaFieldEntrance9` (6 layer rows, z_scene_table.c:894-897); arrival pose NI (external scene) | surface 24, polys 1040/1041, room_01, behind TA0 |

### 2.3 Collision surfaces carrying exits (C from scene.c:186-194 and the polygon list)

| Surface | data[0] / data[1] | Exit+1 -> slot | Polys (scene.c line) | Kind | Extent x / y / z | Room | Floor effect | Flags |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 3 | 0x00000101 / 0x00001002 | 1 -> 0 East | 233, 234 (:430-431) | floor, flat | 570..630 / -460 / 4160..4320 | room_00 east end | 0 | none |
| 24 | 0x00000202 / 0x000050A2 | 2 -> 1 Termina Field | 1040, 1041 (:1237-1238) | floor, flat | -37..23 / -129 / 188..286 | room_01 outside TA0 | 2 | xp 1 (ignore camera) |

No other of the 28 surface types has exit bits. Neither exit floor carries the conveyor flag (vIB flag 1 occurs 0 times in this header), unlike East's walk-through exits. `FLOOR_EFFECT_2` on surface 24 makes `Player_HandleExitsAndVoids` pass `true` to `func_808354A4` (player.c:6441-6442), which stores the destination as the respawn-down entrance and sets `respawnFlag = -2` (player.c:6385-6389) (C).

### 2.4 Transition actors (HDR scene.c:43-46; decode C)

Sides are `{room, bgCamIndex}`, front then back (`include/z64scene.h:369-378`). Yaw `DEG_TO_BINANG((rotY >> 7) & 0x1FF)`, csId `rotY & 0x7F`, params `TRANSITION_ACTOR_PARAMS(index, p)` (`src/code/z_actor.c:3600-3625`).

| TA | Actor | Position | rotY raw -> yaw, cs | Front / back | Params decode | Behaviour (OVL) |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | `EN_DOOR` | (-7, -129, 188) | -19329 (0xB47F) -> 360 = 0 deg; cs none | room 1 cam 255 / room 1 cam 255 | `0x007F`: type `(p >> 7) & 7` = 0 `ENDOOR_TYPE_WHOLE_DAY`; type var 0x7F = text offset 0xF, half-day index 7 (`ovl_En_Door/z_en_door.h:43-52, 95-100`) | Closed only if `!((dawnBits >> 1 | nightBits) & D_801AED48[7])` (z_en_door.c:511-533); `D_801AED48[7]` holds all five night bits (`src/code/z_actor.c:2006-2015`), so the door is **open on every half-day of days 0-4** (C). The test runs from either side; only SCHEDULE doors are side-limited (z_en_door.c:537). Art: the scene lookup hits `{SCENE_TENMON_DAI, DOOR_DL_OBSERVATORY_LAB, OBJECT_DOR01}` (z_en_door.c:192, :351-362) -> `gObservatoryLabDoorDL` (object_dor01 0x448) on `gDoorSkel` (gameplay_keep 0x22B28) (z_en_door.c:309, :343, :643) |
| 1 | `EN_HOLL` | (60, -480, 440) | 127 (0x007F) -> 0 deg; cs none | room 1 cam 255 / room 0 cam 255 | `0x007F`: type `(p >> 7) & 7` = 0 `EN_HOLL_TYPE_DEFAULT` (visible hall); z-bitmask index `p & 7` = 7 (`ovl_En_Holl/z_en_holl.h:63-66`) | `playerSide` = BEHIND (0) if local z < 0 else BEFORE (1) (z_en_holl.c:106-111): world z < 440 -> `sides[0]` = room 1, z > 440 -> `sides[1]` = room 0, consistent with the collision split (C). Active within local y -50..200, abs(x) < 150, abs(z) < 200 (`z_en_holl.h:38-43`; .c:85, :174-179); 150 < abs(z) < 200 finishes the change to the near room, abs(z) < 150 requests the far room and fades the plane (:180-214). Never killed: index 7 covers every half-day (:152-160). Draws `gEnHollCentralPlaneDL` (overlay asset 0x0C30), black with prim alpha, turned 180 deg when behind (:318-347) |

**Door probe (C).** On opening, `doorDirection = +1` if the player's local z >= 0 (z_en_door.c:485-496); player.c:6756-6783 raycasts the floor at the door position moved `-doorDirection x 22` along the door yaw, at door Y + 10, and passes that polygon to `Player_HandleExitsAndVoids`. From inside the chamber (z < 188, direction -1) the probe point (-7, -119, 210) hits **poly 1041** (surface 24, exit slot 1, `0x5490` Termina Field) (scratch `col.py`). From the +Z side the probe (-7, -119, 166) hits poly 1038 (surface 16, no exit), but that side's floor is itself exit floor 1040/1041 (60 x 98 u), which fires from `actor.floorPoly` (player.c:12572) (C).

**Corridor (C).** The hall plane crosses a straight corridor at x 0..120, floor Y -480, ceiling Y -300: room_00 floors 630-633 (z 440..1008) and 625-629, 634 (z 1008..1058, ladder top); room_01 floors 1050/1051 (z 90..440); walls at x 0 and x 120 on both sides (polys 645-648, 1052-1057). These 12 floors form one shared-edge component (scratch `connect.py`).

### 2.5 Access from East and the cross-check against `east_clock_town.md` 2.5

| Item | East dossier / registers | This scene's data | Result |
| --- | --- | --- | --- |
| Inbound trigger | East floors 116/117, surface 4, exit+1 3 -> slot 2 `0x4C00`, centred near (1896, 80, -1794) | `0x4C00` = entrance 0 -> spawn 0, room 0 (2.1) | agrees |
| Return | `0xD220` -> East spawn 2 (1762, 80, -1793) facing -X | exit slot 0 on polys 233/234 (2.3) | agrees |
| Wall poly 169 | inert; `portal.east_clock_town.astral_observatory.p169` retired in `shared/city/ids.json` | no counterpart here; the player exit check reads floors only (player.c:6783, :12572) | agrees |
| Gate | `En_Bombers2` a47 blocks the corridor mouth until `WEEKEVENTREG_ENTERED_BOMBERS_CODE` | none of the 19 observatory overlays reads that flag or `bomberCode` (grep); the in-scene obstacle is the balloon (5.2) | complementary |
| Frame | East tunnel end x 1832..1960, Y 80, z -1824..-1764 | observatory east end x 570..630, Y -460, z 4160..4320 | **separate source frames** (C): a scene change, not a seam |
| TF door hours | PORTAL_REGISTER `portal.astral_observatory.z2_00keikoku`: "open hours UNKNOWN" | WHOLE_DAY index 7: always open (2.4) | resolvable; register not edited |
| Teleport-only list | PORTAL_REGISTER section 6: "Observatory 1-3" | only spawns 0-2 exist; external/teleport arrivals are spawns 1 and 2; entrance row 3 is padding | wording disagreement, reported only |

### 2.6 Portal IDs

| Portal ID | Status | Mode | Evidence | Arrival |
| --- | --- | --- | --- | --- |
| `portal.east_clock_town.astral_observatory.p116` | frozen (inbound) | district | East polys 116/117, slot 2 | spawn 0, room 0 |
| `portal.astral_observatory.east_clock_town` | frozen | district | polys 233/234, slot 0 `0xD220` | East spawn 2 |
| `portal.astral_observatory.z2_00keikoku` | frozen | external-boundary | TA0 door probe -> poly 1041, slot 1 `0x5490` | `ext.z2_00keikoku` spawn 9 (NI) |
| `portal.astral_observatory.astral_observatory.r0_r1` / `.r1_r0` | frozen | room | TA1 `EN_HOLL` at z 440 | room 0 -> 1 / room 1 -> 0 |
| `portal.east_clock_town.astral_observatory.p169` | retired | none | East wall poly | none |
| `portal.astral_observatory.z2_00keikoku.telescope` | **proposed, unregistered** | special | Shikashi choice 0 -> `ENTRANCE(TERMINA_FIELD, 10)` = `0x54A0` (bji.c:339-348) | TF spawn 10 in telescope mode (NI) |

Inbound-only arrivals from the external scene (outside REF-02 scope; no row proposed): Termina Field -> `0x4C10` (spawn 1); telescope B button -> `0x4C20` (spawn 2) (player.c:8626-8645). The scarecrow time skip reloads `ENTRANCE(ASTRAL_OBSERVATORY, 0)` with a stored pose (5.5): a same-scene reload, not a portal.

## 3. Object banks (HDR r00.c:25-41 and r01.c:25-41, identical; IDs from `include/tables/object_table.h`)

| # | Bank | Object ID | Consumer (profile `objectId`, OVL) | Needed for the Day 1 static list |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_OS_ANIME` | 0x09D | none among the 19 listed overlays (grep) | no |
| 1 | `OBJECT_BJI` | 0x0DE | En_Bji_01 (room_01 a00) | skeletal (later) |
| 2 | `OBJECT_SYOKUDAI` | 0x080 | Obj_Syokudai (room_00 a10-a15) | yes |
| 3 | `OBJECT_WATER_EFFECT` | 0x182 | En_Water_Effect (room_00 a04-a08) | effect only |
| 4 | `OBJECT_ST` | 0x020 | En_St (room_00 a01, a02) | skeletal (later) |
| 5 | `OBJECT_BOX` | 0x00C | En_Box (room_00 a23) | posed skeleton (closed) |
| 6 | `OBJECT_HAKUGIN_OBJ` | 0x190 | none listed (its users are Door_Shutter, Door_Spiral, Bg_Hakugin_*, En_Test2; none is in this scene) | no |
| 7 | `OBJECT_MKK` | 0x17A | En_Mkk, spawned by Obj_Tsubo only when home rot.z == 1 (`ovl_Obj_Tsubo/z_obj_tsubo.c:134-160`); all 7 pots have rot.z 0 (INF: unused) | no |
| 8 | `OBJECT_DOR01` | 0x230 | En_Door TA0 (z_en_door.c:192) | yes |
| 9 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai wall clocks (room_00 a22, room_01 a08) | yes (already extracted) |
| 10 | `OBJECT_GI_RESERVE00` | 0x1B1 | Obj_Moon_Stone (room_01 a03) | yes |
| 11 | `OBJECT_ASTR_OBJ` | 0x267 | Bg_Astr_Bombwall (room_00 a21) | yes |
| 12 | `OBJECT_NIW` | 0x00F | En_Niw (room_01 a02) | skeletal (later) |
| 13 | `OBJECT_KA` | 0x11D | En_Kakasi (room_01 a01) | skeletal (later) |
| 14 | `OBJECT_FUSEN` | 0x280 | En_Encount2 (room_00 a00) | yes |

Implicit: `gameplay_keep` (En_Light, Obj_Mure, Obj_Sound and En_Test4 profiles, `gDoorSkel`, `gEffFire1DL` 0x7D590) and `gameplay_dangeon_keep` (scene special file; pot type 0 DLs, z_obj_tsubo.c:67-69).

## 4. Actor tables

### 4.1 Decode rules

The brief's rules, verified at `src/code/z_actor.c:3628-3655` (`Actor_SpawnEntry`), `:2546-2577` (half-day filter) and `:2491-2499` (current bit). ID flags present: `OBJ_TSUBO | 0x2000` (rotZ passed as an integer, 0 here) and `EN_BOX | 0x6000` (rotX and rotZ as integers, both 0; `ENBOX_GET_SWITCH_FLAG` reads `world.rot.z`, `ovl_En_Box/z_en_box.h:42`). **Every entry in both rooms has raw rotX 0x0007 and rotZ 0x007F, so every mask is 0x3FF (all half-days)**: the actor lists carry no time-of-day variation (C, scratch `actors.py`). Classes use the frozen PIPE-01 vocabulary (`shared/city/schema/README.md`). "D1d" = mask includes Day 1 daytime.

### 4.2 room_00 default header (24 entries, HDR r00.c:43-116)

| # | Actor (ID) | Position | Raw rotY | Yaw | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `EN_ENCOUNT2` (0x05F) | (60, -490, 1090) | 0x007F | 0 | none | 0x0001 | object_fusen | static-mesh | yes |
| 1 | `EN_ST` (0x024) | (-169, -418, 2911) | 0x2D7F | 90 | none | 0x003F | object_st | posed-skeletal | yes |
| 2 | `EN_ST` (0x024) | (324, -440, 2110) | 0x2D7F | 90 | none | 0x0004 | object_st | posed-skeletal | yes |
| 3 | `OBJ_MURE` (0x04F) | (674, -603, 2884) | 0x007F | 0 | none | 0x3322 | gameplay_keep | controller-invisible | yes |
| 4-8 | `EN_WATER_EFFECT` (0x170) x5 | (-445, -320, 2884), (181, -320, 2250), (627, -320, 2252), (738, -320, 2430), (777, -320, 2812) | 0x007F | 0 | none | 0x00FF | object_water_effect | billboard-effect | yes |
| 9 | `EN_LIGHT` (0x004) | (780, -580, 2320) | 0x007F | 0 | none | 0x27F5 | gameplay_keep | billboard-effect | yes |
| 10-15 | `OBJ_SYOKUDAI` (0x039) x6 | (165, -630, 1095), (621, -560, 3121), (11, -580, 2036), (780, -600, 2320), (-47, -630, 1095), (-362, -580, 3140) | 0x007F | 0 | none | 0x287F | object_syokudai | static-mesh | yes |
| 16-19 | `OBJ_TSUBO` (0x082) \| 0x2000 x4 | (-119, -630, 1080) 0x400A; (-179, -626, 1080) 0x460A; (241, -630, 1079) 0x420E; (301, -630, 1080) 0x440E | 0x007F | 0 | none | as listed | gameplay_dangeon_keep | static-mesh | yes |
| 20 | `EN_TEST4` (0x15A) | (601, -460, 4288) | 0x000C | 0 | 12 | 0xFD00 | gameplay_keep | controller-invisible | yes |
| 21 | `BG_ASTR_BOMBWALL` (0x28D) | (-500, -580, 3090) | 0x2D0B | 90 | 11 | 0x0000 | object_astr_obj | static-mesh | yes |
| 22 | `OBJ_TOKEIDAI` (0x19C) | (720, -460, 3391) | 0x5A7F | 180 | none | 0x907F | object_obj_tokeidai | static-mesh | yes |
| 23 | `EN_BOX` (0x006) \| 0x6000 | (-636, -579, 3088) | 0x2D09 | 90 | 9 | 0x50C0 | object_box | posed-skeletal | yes |

### 4.3 room_01 default header (10 entries, HDR r01.c:43-74)

| # | Actor (ID) | Position | Raw rotY | Yaw | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `EN_BJI_01` (0x124) | (137, -84, -176) | 0x96FF | 301 | none | 0xFFFF | object_bji | npc-scheduled | yes |
| 1 | `EN_KAKASI` (0x0CA) | (-3, -471, -235) | 0x1E0E | 60 | 14 | 0x0001 | object_ka | npc-scheduled | yes |
| 2 | `EN_NIW` (0x011) | (-92, -400, -245) | 0x007F | 0 | none | 0xFFFF | object_niw | posed-skeletal | yes |
| 3 | `OBJ_MOON_STONE` (0x283) | (154, -44, -106) | 0x61FF | 195 | none | 0x0000 | object_gi_reserve00 | static-mesh | yes |
| 4-6 | `OBJ_TSUBO` (0x082) \| 0x2000 x3 | (-87, -480, -112) 0x4802; (-64, -480, -57) 0x4A02; (-117, -480, -66) 0x4C0E | 0x007F | 0 | none | as listed | gameplay_dangeon_keep | static-mesh | yes |
| 7 | `EN_TEST4` (0x15A) | (-10, -65, 342) | 0x000D | 0 | 13 | 0x0120 | gameplay_keep | controller-invisible | yes |
| 8 | `OBJ_TOKEIDAI` (0x19C) | (-130, -349, -39) | 0x3C7F | 120 | none | 0x907F | object_obj_tokeidai | static-mesh | yes |
| 9 | `OBJ_SOUND` (0x0F0) | (73, 4, -83) | 0x007F | 0 | none | 0x013A | gameplay_keep | controller-invisible | yes |

Transition actors (2.4): TA0 `EN_DOOR` and TA1 `EN_HOLL`, class door-transition. Support floors (scratch `probe2.py`): Shikashi on deck poly 710 (Y -84); the Moon's Tear display on poly 764 (Y -44, a fixture top 40 u above the deck); scarecrow on poly 1095 (Y -471); the wall clocks sit 131 u (room_01) and 120 u (room_00) above the floor below them (INF: wall-mounted); the Skulltulas hang 214 u and 140 u above the channel floor.

### 4.4 Overlay findings per actor type (OVL)

| Actor | Decode and init | Draw | Presence at Day 1 noon, fresh save |
| --- | --- | --- | --- |
| En_Encount2 ("Astral Observatory - Majora's Mask Balloon", ANN :4) | switch `p & 0x7F` = 1 (`z_en_encount2.h:10`); killed at init if switch 1 is set (:122-124); dynapoly `object_fusen_Colheader_002420` (:106-108); health 1; pops on an AC hit with damage effect 0xE (:154), which its table gives to Zora boomerang, normal/fire/ice/light arrows, hookshot, Deku bubble and thrown objects (:66-99); after the pop, a 30-frame timer, then switch 1 is set and the actor dies (:161-185) | opaque `gMajoraBalloonDL` 0xA00 + `gMajoraBalloonKnotDL` 0xD78 (:208-214); vertical bob `velocity.y = sin(angle += 1500)` (:150-151) | present. 28 u on the +Z side of climbable walls 653/654 (x 50..70, Y -630..-465) at Y -490, just under the ladder top (Y -480): its collision blocks the climb (INF, U-06) |
| En_St (Skulltula, large suspended; ANN :4) | switch `p & 0x3F` (`z_en_st.h:11`): a01 0x3F = none, a02 = 4; killed at init if that switch is set (:858-861); sets it on death (:631-632) | `object_st_Skel_005298` (XML) | both present |
| Obj_Mure | type `p & 0x1F` = 2 `OBJMURE_TYPE_FISH`; chNum `(p >> 12) & 0xF` = 3, ptn `(p >> 8) & 7` = 3, svNum `(p >> 5) & 3` = 1 (`z_obj_mure.h:10-21`) | fish children (NI) | present, 11 u below water box wb09's surface (Y -592) |
| En_Water_Effect | params 0xFF = `ENWATEREFFECT_TYPE_FALLING_WATER_SPAWNER` (`z_en_water_effect.h:23`); drips only while `Flags_GetSwitch(params)` is false (:157), and `Flags_GetSwitch` returns 0 for flags >= 0x80 (`src/code/z_actor.c:778-783`): always dripping | particles; all five at Y -320 | always |
| En_Light 0x27F5 | F = 5 (flame colours {255,200,0}/{255,0,0}, scale 75 x 0.0001), 0x800 set (flame update), 0x1000 clear (no switch hide), 0x2000 set (no-glow point light, radius 730), 0x4000 clear (light inserted) (`z_en_light.h:8-14`; .c:37-80, :104) | `gEffFire1DL` (:175, :196) | present, 20 u above torch a13 |
| Obj_Syokudai 0x287F | type `p >> 12` = 2 `NO_SWITCH`, start lit 0x800 set, group size 0, switch 0x7F none (`z_obj_syokudai.h:8-13, 27-29`); ICHAIN scale 1.0 (.c:72) | `gObjectSyokudaiTypeNoSwitchDL` (object_syokudai 0x870) + XLU `gEffFire1DL` (.c:298, :321) | present, lit |
| Obj_Tsubo | type `(p >> 7) & 3` = 0 -> gameplay_dangeon_keep pot `DL_017EA0`, shards `DL_018090`, scale 0.197 (.c:67-69, :181); drop index `p & 0x3F` through `D_801AE194` (`src/code/z_en_item00.c:1453-1461`): 0x0A recovery heart (r00 a16, a17), 0x0E small magic jar (r00 a18, a19; r01 a06), 0x02 blue rupee (r01 a04, a05); collectible flag `(p >> 9) & 0x7F` = 0x20, 0x23, 0x21, 0x22 / 0x24, 0x25, 0x26 (`z_obj_tsubo.h:12-18`; .c:121-131); rot.z 0 -> no Boe or Gold Skulltula (:155-160) | static | present |
| En_Test4 | one instance per scene load (`sIsLoaded`, `z_en_test4.c:35`, :339-343), `room = -1`; which of a20/a07 survives depends on the arrival room (INF, U-11); csId 12 / 13 -> cutscene entries whose `additionalCsId` is themselves (scene.c:89-90; .c:329-337); stars `(s16 params >> 10) * 100` = -100 (a20) and 0 (a07), ignored when <= 0 (.c:614-619); bell volume index `(p >> 5) & 0xF` = 8 / 9 (`z_en_test4.h:8-9`) | none | always |
| Bg_Astr_Bombwall | switch `p & 0x7F` = 0 (`z_bg_astr_bombwall.h:8`); killed at init if set (.c:104-107); dynapoly `gAstralObservatoryBombWallCol` (object_astr_obj 0x2498) (:101-102); tris collider AC mask 0x00000008 = `DMG_EXPLOSIVES` (.c:36-52; `include/z64collision_check.h:446`); on hit: switch 0 set, csId 11 (cam 1 FIXED1) via `SubS_StartCutscene`, fragments `gAstralObservatoryBombWallFragmentDL` (:161-190); ICHAIN scale 1.0 (:74-75) | opaque `gAstralObservatoryBombWallDL` 0x2380 + XLU `gAstralObservatoryBombWallCrackDL` 0x22E0 (:198-222) | present |
| En_Box 0x50C0 | type `(p >> 12) & 0xF` = 5 `ENBOX_TYPE_SMALL`; item `(p >> 5) & 0x7F` = 0x06 `GI_RUPEE_SILVER` (`include/z64item.h:421`) -> `ITEM_RUPEE_SILVER` = 100 rupees (`sRupeeRefillCounts` index 5, `src/code/z_parameter.c:2543, 2856-2857`); chest flag `p & 0x1F` = 0 (`z_en_box.h:39-44`; .c:210-212); csId 9 (scene.c:86) | `gBoxChestSkel` (object_box 0x66A0), closed pose | present; 136 u west of a21 at the same Z, both yaw 90 (INF: in the alcove the wall closes, U-16) |
| Obj_Tokeidai 0x907F | type `p >> 12` = 9 `OBJ_TOKEIDAI_TYPE_WALL_CLOCK`, scale 0.02, `ObjTokeidai_Clock_Draw` (`z_obj_tokeidai.h:6-19`; .c:247-252); `clockTime = CURRENT_TIME` at init (.c:215) | minute ring 0xCF28, centre/hand 0xBEE8, `gWallClockClockFaceDL` 0xF518, sun/moon panel 0xC368 (.c:810-844) | present in both rooms; hands follow the clock |
| En_Bji_01 (Professor Shikashi, ANN :4) | kept only if `save.entrance` is observatory 0, 1 or 2, else killed (bji.c:390-408); e0/e1 -> `SHIKASHI_TYPE_DEFAULT`, e2 -> `LOOKED_THROUGH_TELESCOPE` with auto-talk; idle anim `SHIKASHI_ANIM_LOOK_THROUGH_TELESCOPE` (`object_bji_Anim_000FDC`) (bji.c:66-71, :85-90); scale 0.01, cylinder r18 h64 (:37-55, :386); DEFAULT auto-starts talk within 60 u and 10 u height (:94-100) | `gShikashiSkel` (object_bji 0x578C), flex | present for every entrance into this scene (C) |
| En_Kakasi 0x0001 | summon distance `((p >> 8) & 0xFF) * 20` = 0 -> clamped 40; above-ground `p & 1` = 1 (`z_en_kakasi.h:45-47`; .c:160-171, :181-187); standing unless `WEEKEVENTREG_79_08`, which buries it until the scarecrow song is played within 80 u (.c:197-212, :1056-1065); `WEEKEVENTREG_83_01` -> time-skip dialogue first (.c:205-207, :313-330); csId 14 -> entry 14, additional 15 (scene.c:91-92) | `object_ka_Skel_0065B0`, flex | present, standing |
| En_Niw | params < 0 -> `NIW_TYPE_REGULAR` (`z_en_niw.c:92-98`); `gNiwSkel` (object_niw 0x2530), scale 0.01 | skeletal | present |
| Obj_Moon_Stone 0x0000 | type `(p & 0xF000) >> 12` = 0 -> health 0, attention + friendly; offers talk at 80 u when facing within 0x1555 (`z_obj_moon_stone.c:38-49, 65-95`); text 0x5E3 sets health 1 while open; scale 0.3, yOffset 25 | `gGiMoonsTearTexAnim` 0x1C60 + opaque `gGiMoonsTearItemDL` 0xD78 + billboarded XLU `gGiMoonsTearGlowDL` 0xC80 (object_gi_reserve00) (:143-157) | present and drawn: the observatory display, not the pickup (5.4) |
| Obj_Sound 0x013A | type `(p >> 8) & 0xFF` = 1 BGM, sfx bit 0, id 0x3A (`z_obj_sound.h:8-17`; .c:28-38); plays `Audio_PlayObjSoundBgm(projectedPos, 0x3A)` from its second update (:57-59) | none | always: positional observatory music over the scene's Cavern sequence |

### 4.5 Counts

| | room_00 | room_01 | Transition |
| --- | --- | --- | --- |
| Entries | 24 | 10 | 2 |
| Day 1 daytime candidates by mask | 24 | 10 | n/a |
| static-mesh | 13 (a00, a10-a15, a16-a19, a21, a22) | 5 (a03, a04-a06, a08) | - |
| posed-skeletal | 3 (a01, a02, a23) | 1 (a02) | - |
| npc-scheduled | 0 | 2 (a00, a01) | - |
| billboard-effect | 6 (a04-a08, a09) | 0 | - |
| controller-invisible | 2 (a03, a20) | 2 (a07, a09) | - |
| door-transition | - | - | 2 |

## 5. Venue mechanics index (INT-15 scope)

### 5.1 Bombers' code gate (East side; nothing in this scene re-checks it)

| Step | Implementation | Citation |
| --- | --- | --- |
| Code storage | `gSaveContext.save.saveInfo.bomberCode[5]` ("bombers_aikotoba_table", ANN) | `include/z64save.h:322` |
| Generation | 5 distinct digits 1-5 by rejection sampling in `Sram_GenerateRandomSaveFields` | `src/code/z_sram_NES.c:705-770` |
| When | `Sram_InitNewSave` and `Sram_InitDebugSave` (calls at :945 and :1165); no other caller in z_sram_NES.c (grep), so the code is per save file (INF) | `src/code/z_sram_NES.c:924-945, 1133-1165` |
| Shown to the player | message code reads the digits | `src/code/z_message.c:2814-2815`; `src/code/z_message_nes.c:1652` |
| Checker | `En_Bombers2` (East a47). Goron/Zora -> text index 1, no prompt; Deku/Human -> index 2, then 3 opens `TEXT_STATE_INPUT_BOMBER_CODE` | `ovl_En_Bombers2/z_en_bombers2.c:173-182, 289-296` |
| Match | slot i true when `msgCtx.codeGuessDigits[i] == bomberCode[i]`; all 5 needed; otherwise slots reset, `NA_SE_SY_ERROR`, text index 4 then 5 | :250-273, :298-315 |
| Success | text index 6 (0x072A), walk 50 u aside under the guard's csId, then `SET_WEEKEVENTREG(WEEKEVENTREG_ENTERED_BOMBERS_CODE)` (weekEventReg 73 bit 0x80) | :317-380; `include/z64save.h:1330` |
| Already open | init moves the guard aside if the flag is set or `save.entrance == ENTRANCE(EAST_CLOCK_TOWN, 2)` (the return from here) | :123-129 |
| Cycle reset | weekEventReg row 73 keeps only `73_08` across cycles, so the code must be re-entered each cycle (C) | `src/code/z_sram_NES.c:49, 230, 487` |
| Side effects | any talk sets `WEEKEVENTREG_TALKED_TO_BOMBERS_GUARD`; mask face reactions override the text (NI) | :193-199 |

### 5.2 Underground route obstacles (room_00)

| Mechanic | Source | Gate and item constants |
| --- | --- | --- |
| Balloon at the ladder | En_Encount2 a00 (4.4) | scene switch 1; ranged hit with damage effect 0xE; dynapoly while unpopped |
| Ladder | walls 653/654 `WALL_TYPE_2` (`WALL_FLAG_0 \| WALL_FLAG_1`), 655/656 `WALL_TYPE_3` (`WALL_FLAG_0 \| WALL_FLAG_2`) at x 50..70, z 1058..1062, Y -630..-460 | `src/code/z_bgcheck.c:18-26`; climb branch player.c:9612-9617 |
| Bomb wall | Bg_Astr_Bombwall a21 | scene switch 0; `DMG_EXPLOSIVES`; csId 11 |
| Chest | En_Box a23 | chest flag 0; `GI_RUPEE_SILVER` (100 rupees); csId 9 |
| Hazards / pickups | En_St a01 (switch none), a02 (switch 4); pots (collectible flags 0x20-0x23) | 4.4 |

Switch flags 0-3 here live in scene flags; the scene's persistent cycle flags are NONE (scene_table.h:89), so cycle carry-over is NI (LOCATION_REGISTER U-13).

### 5.3 Telescope (Shikashi in room_01 -> Termina Field view -> back)

| Step | Implementation | Citation |
| --- | --- | --- |
| Talk text | Deku: 0x5EC (sets `WEEKEVENTREG_17_10`) / 0x5E2 / 0x5F4 (`74_80`); Human: 0x5F5 (sets `74_10`) / 0x5F6, or 0x236A with Kafei's Mask; Goron/Zora: 0x5DC (sets `75_08`) / 0x5E4 | bji.c:118-162 |
| Choice | choice 0 -> `func_809CD634`; choice 1 -> 0x5F0 / 0x5F8 / 0x5E1 by form. Which message carries the choice is message data (NI, U-03) | bji.c:227-262 |
| Go | mute sfx banks, disable sequences, `nextEntrance = ENTRANCE(TERMINA_FIELD, 10)` (`0x54A0`), copy it to `respawn[RESPAWN_MODE_DOWN].entrance`, `func_80169EFC`, `respawnFlag = -2` | bji.c:339-348; `src/code/z_play.c:1918-1927` |
| Destination | `sTerminaFieldEntrance10` `{SCENE_00KEIKOKU, 10, 0x8A95}` plus 5 layers; spawn 10 pose and start mode NI (TF not extracted) | z_scene_table.c:899-902 |
| Telescope mode | start mode 0xC `PLAYER_START_MODE_TELESCOPE`: the spawn list loads `OBJECT_STK` instead of Link's objects; `Player_StartMode_Telescope` hides Link, sets focus rot (0x0BD8, -0x4D74) and a 20-frame delay in `SCENE_00KEIKOKU`, and sets `ACTORCTX_FLAG_TELESCOPE_ON` | `include/z64player.h:33`; `src/code/z_scene.c:161-166`; player.c:8654-8671, :11014-11016 |
| View | `Camera_ChangeSetting(CAM_SET_TELESCOPE)` every frame (`0x3E`, "Observatory telescope and Curiosity Shop Peep-Hole", `include/z64camera.h:209`); mode `CAM_MODE_FIRSTPERSON` during the delay, then `NORMAL`, `TARGET` while A is held (zoom); text 0x5E6 = controls overlay; pitch step +-0x12C clamped +-0x2EE0, yaw clamped +-0x3E80 from `shape.rot.y`; movement sfx in TF | player.c:8567-8650 |
| HUD, pause, time | clock drawn at full alpha; pause blocked; time keeps running while 0x5E6 is open (Dm_Stk); scene time-sequence logic skipped | `src/code/z_parameter.c:4579-4615`; `src/code/z_kaleido_setup.c:126-128`; `ovl_Dm_Stk/z_dm_stk.c:1866-1876`; `src/code/z_kankyo.c:2487` |
| Exit | B -> `respawn[DOWN].entrance = ENTRANCE(ASTRAL_OBSERVATORY, 2)` (`0x4C20`), `func_80169EFC`, `respawnFlag = -2`, circle transition | player.c:8626-8645 |
| Return pose | flag -2 selects respawn index DOWN without copying a stored position, so the spawn-list pose (spawn 2) is used | player.c:11263-11293 |
| Return talk | type `LOOKED_THROUGH_TELESCOPE`: auto-talk, sfx banks unmuted, sequences re-enabled; Deku 0x5F1 / 0x5F2 (`74_80`), Human 0x5F7, Goron/Zora 0x5E9 (day 1), 0x5EA (day 2), 0x5EB or 0x5E8 when `TIME_UNTIL_MOON_CRASH < CLOCK_TIME_F(1, 0)` (day 3) | bji.c:164-208, :397-403 |
| In this scene | no telescope actor and no `CAM_SET_TELESCOPE` bgCam (camera data 0x00, 0x11, 0x3F, 0x3D, 0x32, 0x4A, scene.c:177-184): the telescope in room_01 is room mesh only (INF; candidate area near spawn 2 and Shikashi, U-04) | HDR |

### 5.4 Skull Kid sighting and the Moon's Tear (Termina Field side; scene external, overlays inspected)

| Step | Implementation | Citation |
| --- | --- | --- |
| Skull Kid present | `Dm_Stk` in `SCENE_00KEIKOKU` layer 0 is killed unless the telescope flag is on; glowing-eyes mask, action `DmStk_WaitForTelescope` | `ovl_Dm_Stk/z_dm_stk.c:1138-1149` |
| Sighting test | while `WEEKEVENTREG_74_20` is clear: zoomed (`view.fovy < 25`) and his screen position inside x 70..(SCREEN_WIDTH - 70), y 30..(SCREEN_HEIGHT - 30) -> `Environment_StopTime`, start the telescope cutscene | z_dm_stk.c:1196-1208 |
| Cutscene choice | day < 3: actor csId; day 3: its additional csId; tower opened or (day 3 and before 06:00): the next additional | z_dm_stk.c:1213-1234 |
| After | fade-out sets `WEEKEVENTREG_12_04` | z_dm_stk.c:1670-1687 |
| Tear falls | `En_Fall` type 8 `EN_FALL_TYPE_MOONS_TEAR` exists only with the telescope on and a TF moon (`z_en_fall.c:279-292`); cue 517 id 2 spawns it at the moon's focus and moves it to home in 82 steps (:511-536); on landing: `SET_WEEKEVENTREG(74_80)` and `(74_20)`, child `En_Test`, large clear-tag explosion | `ovl_En_Fall/z_en_fall.h:7, 18`; z_en_fall.c:530-551 |
| Pickup | `Obj_Moon_Stone` type != 0 (the TF instance; params and position NI): killed if `74_40`; hidden until `74_80`; when drawn and within 25 u offers `GI_MOONS_TEAR` (0x96); on closing the text sets `WEEKEVENTREG_74_40` and dies | `z_obj_moon_stone.c:50-59, 97-132`; `include/z64item.h:565` |
| Downstream | `En_Scopenuts` reads `74_40` (NI) | `ovl_En_Scopenuts/z_en_scopenuts.c:728` |
| Seen from here | room_01 Obj_Moon_Stone type 0 is talk-only and always present; Shikashi turns to it while its text is open | z_obj_moon_stone.c:38-49; bji.c:106-110, :353-371 |
| Cycle reset | weekEventReg rows 17 and 74 have no persistent bits: `17_10`, `74_10`, `74_20`, `74_40`, `74_80` reset at the new cycle (C) | `src/code/z_sram_NES.c:97, 231` |
| Other telescope-gated TF actors (index only) | `En_Scopecoin`, `En_Scopecrow`, `En_Scopenuts`, `En_Ge2`, `En_Sth`, `En_Sth2`, `Obj_Tokeidai` TF types | grep of `ACTORCTX_FLAG_TELESCOPE_ON` |

### 5.5 Scarecrow (En_Kakasi, room_01 a01)

| Mechanic | Implementation | Citation |
| --- | --- | --- |
| Song teach | ending the teach buries it and sets `WEEKEVENTREG_79_08`; afterwards it starts underground and rises when the scarecrow song is played within 80 u | z_en_kakasi.c:1030-1047, :197-201, :1056-1065 |
| Dance offer | text 0x1650 "Shall we dance? No Yes" (ANN) after 0x1645 / 0x164E | :483-489, :495-499 |
| Player pin | during teach and dance the player is moved to x 60, z -190 in this scene | :262-265 |
| Time skip | case 14: respawn-down = `Entrance_CreateFromSpawn(0)` = `ENTRANCE(ASTRAL_OBSERVATORY, 0)` with the player's stored pose and start mode B; night -> time 06:00, `respawnFlag -4`, `EVENTINF_TRIGGER_DAYTELOP`; day -> 18:00, `respawnFlag -8`; sets `83_01` | :948-970; `src/code/z_scene.c:610-612` |
| Reload pose | flags -4 / -8 map to respawn index DOWN and copy the stored position, so the player returns to the dance spot, not spawn 0 (C); which room loads is NI (U-14) | player.c:11275-11293 |
| Afterwards | with `83_01`: "did you feel that?" 0x1653, then clears `83_01` | :318-330 |

### 5.6 Termina Field door and exit

TA0 is always open (2.4). Opening it from inside probes poly 1041 -> exit slot 1 `0x5490` -> Termina Field entrance 9 (2.2, 2.3), through the floor-effect-2 respawn-down path. Arrivals from Termina Field use `0x4C10` -> spawn 1 (mode E walk-in); which TF exit sends it is NI (U-01). No `EN_TIME_TAG` kickout exists in either room (HDR).

### 5.7 Time, day boundaries and music

| Item | Implementation | Citation |
| --- | --- | --- |
| Clock | both rooms `TIME_SETTINGS(255, 255, 3)`: no forced time, normal speed | r00.c:18, r01.c:18 |
| Day/night | En_Test4 (one survivor); with the telescope on at the night-to-day boundary it calls `Sram_IncrementDay`, sets 06:00 and shows the dawn card | z_en_test4.c:46-60 |
| Day 3 midnight | bell at 00:00 on day 3 with no Ocarina (or in South): from `SCENE_TENMON_DAI` the festival cutscene entry is `ENTRANCE(TERMINA_FIELD, 0)`; return params keep telescope mode if it was on | z_en_test4.c:494-518 |
| Moon crash | `Interface_StartMoonCrash` re-enables sequences if the telescope is on; entrance TF 12, day 4 | `src/code/z_parameter.c:5611-5624` |
| Music | scene seq `NA_BGM_CAVERN` plus room_01 `Obj_Sound` `NA_BGM_ASTRAL_OBSERVATORY`; Shikashi disables / re-enables sequences around the telescope | 1, 4.4, bji.c:339-348, :397-403 |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

Assumptions recorded for the profile: fresh save (no switch, chest, collectible or week-event flags), Human form, telescope off, arrival by entrance 0 or 1. **Access:** the venue has no time closure: the door is WHOLE_DAY index 7 (always open), no kickout exists, Shikashi has no schedule. Entry from East needs `WEEKEVENTREG_ENTERED_BOMBERS_CODE` (5.1) and climbing past the balloon (5.2): story and item gates, not time gates, so every profile in SOURCE_REGISTER section 3 is open by time. Entry from Termina Field is NI. A static render needs no access.

| Room # | Component / params | N64 position (Unity) | Yaw | Scale | Display list(s), bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| r00 a00 | Balloon, `0x0001` | (60, -490, 1090) (0.60, -4.90, -10.90) | 0 | 0.1 | `gMajoraBalloonDL` 0xA00 + `gMajoraBalloonKnotDL` 0xD78 (object_fusen) | collision `object_fusen_Colheader_002420`; bob omitted |
| r00 a10-a15 | Torches, `0x287F` | (165, -630, 1095), (621, -560, 3121), (11, -580, 2036), (780, -600, 2320), (-47, -630, 1095), (-362, -580, 3140) | 0 | 1.0 | `gObjectSyokudaiTypeNoSwitchDL` 0x870 (object_syokudai); flame `gEffFire1DL` gameplay_keep 0x7D590 (billboard) | lit |
| r00 a16-a19 | Pots | (-119, -630, 1080), (-179, -626, 1080), (241, -630, 1079), (301, -630, 1080) | 0 | 0.197 | `gameplay_dangeon_keep_DL_017EA0` | |
| r00 a21 | Bomb wall, `0x0000` | (-500, -580, 3090) (-5.00, -5.80, -30.90) | 90 | 1.0 | `gAstralObservatoryBombWallDL` 0x2380 + XLU crack 0x22E0 (object_astr_obj) | collision `gAstralObservatoryBombWallCol` 0x2498 |
| r00 a22 | Wall clock, `0x907F` | (720, -460, 3391) (7.20, -4.60, -33.91) | 180 | 0.02 | object_obj_tokeidai 0xCF28, 0xBEE8, 0xF518, 0xC368 | 12:00 pose as the tower clock rule |
| r00 a23 | Small chest, `0x50C0` | (-636, -579, 3088) (-6.36, -5.79, -30.88) | 90 | NI | `gBoxChestSkel` 0x66A0 (object_box), closed | optional: posed skeleton |
| r01 a03 | Moon's Tear display, `0x0000` | (154, -44, -106) (1.54, -0.44, 1.06); draw Y -36.5 (yOffset 25 x 0.3) | 195 | 0.3 | `gGiMoonsTearItemDL` 0xD78 + glow 0xC80 (billboard) + tex anim 0x1C60 (object_gi_reserve00) | |
| r01 a04-a06 | Pots | (-87, -480, -112), (-64, -480, -57), (-117, -480, -66) | 0 | 0.197 | `gameplay_dangeon_keep_DL_017EA0` | |
| r01 a08 | Wall clock, `0x907F` | (-130, -349, -39) (-1.30, -3.49, 0.39) | 120 | 0.02 | as r00 a22 | |
| TA0 | Door, `0x007F` | (-7, -129, 188) (-0.07, -1.29, -1.88) | 0 | default | `gObservatoryLabDoorDL` object_dor01 0x448 on `gDoorSkel` gameplay_keep 0x22B28 | closed rest pose |
| TA1 | Hall plane, `0x007F` | (60, -480, 440) (0.60, -4.80, -4.40) | 0 | default | `gEnHollCentralPlaneDL` (ovl_En_Holl asset 0x0C30) | opaque black beyond 150 u; omit in a both-rooms render |

**Present but unimplemented in a static assembly:** r00 a01/a02 En_St (`object_st_Skel_005298`), r01 a00 Shikashi (`gShikashiSkel`, telescope-looking anim), r01 a01 scarecrow (`object_ka_Skel_0065B0`, standing), r01 a02 cucco (`gNiwSkel`); effects r00 a04-a08 drips, a09 flame and light, torch flames; room_01's six cylindrical-billboard room DLs (8).

**No geometry:** r00 a03 Obj_Mure (fish children NI), r00 a20 / r01 a07 En_Test4, r01 a09 Obj_Sound.

**Excluded by half-day mask:** none. **Excluded by state on a fresh save:** none. Geometry for this list needs the banks in section 9; the extraction commands there are written, not executed.

## 7. Collision summary (C from scene.c:177-2599; scratch `col.py`, `idx.py`, `layout.py`, `connect.py`, `verts.py`, `water.py`)

### 7.1 Measures

| Measure | Whole header | room_00 (polys 0-656) | room_01 (polys 657-1438) |
| --- | --- | --- | --- |
| Vertices | 934 (903 referenced; indices 0-30 unreferenced, at Y -626 like the small water boxes, INF) | 382 | 525 (4 shared with room_00 at z 440) |
| Polygons | 1439 | 657 | 782 |
| Floors / walls / ceilings (normal Y > 0.5 / else / < -0.8, `src/code/z_bgcheck.c:528-531`) | 270 / 934 / 235 | 82 / 413 / 162 | 188 / 521 / 73 |
| Surface types | 28 | 0-6 | 7-27 |
| Header bounds | min (-810, -632, -497), max (900, 354, 4320) | | |
| Vertex extents | x -700..900, y -632..354, z -440..4320; Unity x -7.00..9.00, y -6.32..3.54, z -43.20..4.40 | x -700..900, y -632..-300, z 440..4320 | x -278..398, y -480..354, z -440..440 |
| Floor slopes (5-deg bins) | | 0: 78, 15: 2, 30: 2 | 0: 130, 20: 10, 25: 4, 30: 16, 35: 6, 40: 3, 45: 13, 50: 4, 55: 2 |
| Materials | | STONE 657 | STONE 645, WOOD 106, DIRT 31 |
| Water boxes | 13, all `room 0` | 13 | 0 |
| Exit surfaces | 2 types, 4 floors | surface 3 (233, 234) | surface 24 (1040, 1041) |

### 7.2 Camera, light and special flags

| Item | Value |
| --- | --- |
| BgCamInfo (scene.c:177-184) | [0] 0x0000 `CAM_SET_NONE`, [1] 0x0011 `CAM_SET_DUNGEON0` (all 82 room_00 floors), [2] 0x003F `CAM_SET_ROOM0` (most room_01), [3] 0x003D `CAM_SET_DUNGEON3` (surface 10, 2 ramp floors x 150..287), [4] 0x0032 `CAM_SET_DUNGEON2` (surface 12, 4 floors x -258..-64, Y -168..-129), [5] 0x004A `CAM_SET_NORMAL2` (surface 16, 22 door-level floors) (`include/z64camera.h:147-221`) |
| Light setting index (used by the player's floor, player.c:11730) | room_00: 0; room_01: 1 on lower surfaces 7-11, 13, 25, 26 and 2 on upper surfaces 12, 15-20, 23, 24, 27; 31 on surfaces 14, 21, 22 |
| Wall types | room_00: 2 `WALL_TYPE_2` (653/654), 2 `WALL_TYPE_3` (655/656) = the ladder (5.2); room_01: 54 walls `WALL_TYPE_1` (`WALL_FLAG_0`, which blocks the ledge climb at player.c:11835) on surfaces 25-27 |
| Floor effect | 2 on surfaces 23 and 24 (threshold and TF exit) |
| Not present | hookshot surfaces, conveyors, floor types, floor properties, soft or horse-blocked flags (all 0) |
| Poly flags | xp: room_00 4 camera-ignore walls (the ladder); room_01 213 camera-ignore, 84 camera + projectiles, 6 all, 5 projectiles; vIB flag 4 on 80 room_01 walls |
| Echo | 2 and 10 in room_00; 0, 2 and 10 in room_01 |

### 7.3 Floor levels

| Room | Level | Y | x / z | Surfaces | Notes |
| --- | --- | --- | --- | --- | --- |
| room_00 | East end | -460 | 570..630 / 4160..4320 | 3 | exit floor |
| room_00 | Ramp | -460 to -580, 18.4 deg | 570..630 / 3800..4160 | 4 (239, 240) | spawn 0 |
| room_00 | Channel banks | -580 / -600 | -700..900 / 1040..3800 | 2, 4 | water boxes wb06-08, wb10-12 lie 1 u over the -580 floor |
| room_00 | Channel bed | -630 / -632 | -660..900 / 1040..3000 | 2, 4 | wb09 surface -592 (40 u over -632); wb00-05 lie 4 u over -630 |
| room_00 | Ladder top + corridor | -480 | 0..120 / 440..1058 | 2, 4 | balloon a00 at the ladder |
| room_01 | Corridor + basement | -480 | -200..320 / -360..440 | 8, 10, 11 | scarecrow on -471; pots |
| room_01 | Stairs / ramps | -480 to -120, 28 to 35.5 deg faces | -258..320 / -360..86 | 7, 9-13 | links the basement (scarecrow, cucco) to door level |
| room_01 | Door level | -129 | -278..398 / -440..286 | 15, 16, 23, 24 | spawn 1, TA0, threshold |
| room_01 | Deck | -84 | -126..218 / -371..1 | 14, 15, 17, 19 | spawn 2, Shikashi |
| room_01 | Upper structure | -74 to 143, faces to 52 deg | -40..179 / -188..-65 | 17 | Moon's Tear display at -44; INF telescope / mount, U-04 |

Shared-edge floor components: 60 (scratch `connect.py`). Only the corridor component spans both rooms; other links (stairs, deck, upper structure) are edge-split and need a tolerant connectivity pass or a route capture before walkability is claimed (U-05).

### 7.4 Proposed camera anchors (REF-03 IDs; proposals only)

| Camera | Anchor and direction |
| --- | --- |
| `cam.astral_observatory.entry` | spawn 0 (602, -472, 4125) facing -Z down the ramp into the channel |
| `cam.astral_observatory.reverse_door` | ramp foot (600, -580, 3800) facing +Z to exit floor 233/234 (the East threshold; no door leaf) |
| `cam.astral_observatory.fixture` | spawn 2 (107, -84, -209) yaw 67 toward Shikashi, the Moon's Tear display and the telescope deck |
| `cam.astral_observatory.room_01` | spawn 1 (-6, -129, 133) facing -Z across the chamber |
| unregistered proposals | `ladder_balloon` (60, -600, 1300) facing -Z at a00 and walls 653/654; `bombwall` (-400, -560, 3090) facing -X at a21/a23; `tf_door` (-6, -129, 100) facing +Z at TA0 |

## 8. Materials and textures (C; counts only, nothing copied)

| Item | Scene file | room_00 | room_01 |
| --- | --- | --- | --- |
| Extracted PNGs | 5 (2 textures + 3 TLUTs) | 15 (11 textures + 4 TLUTs) | 31 (30 textures + 1 TLUT) |
| By format | ci4 2, rgba16 TLUT 3 | ci4 4, ci8 2, ia16 1, ia8 2, rgba16 2, rgba16 TLUT 4 | ci8 10, i4 4, i8 10, ia16 4, ia8 1, rgba16 1, rgba16 TLUT 1 |
| Use of the scene textures | all 5 referenced by both rooms' DLs (grep) | | |
| `Gfx` lists / `Vtx` arrays | | 26 / 15 | 37 / 31 |
| Triangle commands | | 589 `gsSP2Triangles` + 21 `gsSP1Triangle` = 1,199 | 648 + 30 = 1,326 |
| Cullable entries | | 12: 11 opaque, 4 XLU, 4 with negative radius (sorted last, `src/code/z_room.c:182-186`) | 20: 17 opaque, 4 XLU, 2 negative radius |
| Billboards | | none | six radius-14 entries (DLs 0x5C60, 0x60D0, 0x6540, 0x69B0, 0x6E20, 0x7290) each load a room `Mtx` then `gsSPMatrix(0x01000040)` = `billboardMtx + 1`, the camera-yaw cylindrical billboard (`src/code/z_play.c:1202-1210`) (U-12) |
| Animated materials | | segment 0xB | segments 8, 9, 0xA, 0xC, 0xD |

Totals: 51 PNGs = 43 textures + 8 TLUTs (ci4 6, ci8 12, i4 4, i8 10, ia16 5, ia8 3, rgba16 11 incl. TLUTs).

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_TENMON_DAI` (C above); `object_obj_tokeidai` already extracted for South. Binaries exist under `extracted/n64-us/baserom/` for every bank below (checked by `ls`).

| Priority | Banks | Why |
| --- | --- | --- |
| Static pilot | `object_fusen`, `object_syokudai`, `object_astr_obj`, `object_dor01`, `object_gi_reserve00`, `gameplay_dangeon_keep`, `gameplay_keep` | section 6 DLs and dynapoly collision |
| Posed / later skeletal | `object_box`, `object_bji`, `object_ka`, `object_niw`, `object_st` | chest, Shikashi, scarecrow, cucco, Skulltulas |
| Effects | `object_water_effect` | drips |
| External, not city scope | scene `Z2_00KEIKOKU` | telescope view, Moon's Tear pickup, door/exit endpoints (U-01, U-02) |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_fusen','object_syokudai','object_astr_obj','object_dor01','object_gi_reserve00','object_box','gameplay_dangeon_keep','gameplay_keep')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

The keep files have not been extracted with this tool before (NI; North section 8 notes the same); check output size and warnings separately. The `ovl_En_Holl` asset (`gEnHollCentralPlaneDL`) is overlay data, not an object bank (`assets/xml/overlays/ovl_En_Holl.xml`).

## 10. Proposed region record

Proposal-0 vocabulary as in North section 9, lifted by `tools/reference/region_from_dossier.py` onto region schema 1. A scratch lift of this block passed `tools/reference/validate_city.py` (section 11). `portalIds` lists only the registered outgoing portals; the telescope portal (2.6) waits for a registry edit.

```json
{
  "_status": "proposal for region schema 1 (PIPE-01); integrator lifts with region_from_dossier.py",
  "schemaVersion": "proposal-0",
  "regionId": "loc.astral_observatory",
  "displayLabel": "Astral Observatory",
  "source": {
    "sceneSymbol": "Z2_TENMON_DAI",
    "sceneEnum": "SCENE_TENMON_DAI",
    "sceneTableIndex": "0x29",
    "entranceTableSceneIndex": "0x26",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0)",
    "rooms": { "loc.astral_observatory.room_00": "default header", "loc.astral_observatory.room_01": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no switch, chest, collectible or week-event flags", "player form Human", "telescope off", "arrival by entrance 0 or 1", "no alternate headers exist"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TENMON_DAI", "bytes": 34688, "sha256": "a63da639fe2073d224645cf4b95e51fff02bb023e345a1641a44fd486039f728" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TENMON_DAI_room_00", "bytes": 74528, "sha256": "3e89e7c147bc9c91c9cd7210e03c3be2378f2fd8fe6054c2f14c1b7f2e0985b8" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TENMON_DAI_room_01", "bytes": 101040, "sha256": "2e8bd998e39690df1b02d26e100968ea2d06c6441fdcd50037fe4569d39f9477" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TENMON_DAI/Z2_TENMON_DAI.c", "bytes": 143218, "sha256": "90d369e5e82810af76a3d143b57d3d055e52c71673ed33d72b25d26333d6c043" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TENMON_DAI/Z2_TENMON_DAI_room_00.c", "bytes": 86945, "sha256": "dbf43101e39e7612cb29691ee078a7a4c27d1c4e0d65f99e52e92cecf44e6c52" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TENMON_DAI/Z2_TENMON_DAI_room_01.c", "bytes": 94734, "sha256": "ddf63e7010833cfaac10301aecfbd6f9eac0fcd5f25f3e78d82ea485b6be471b" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_TENMON_DAI/Z2_TENMON_DAI.xml", "bytes": 697, "sha256": "d16579c13198af41003f7b0702fbd938c69d961768e40a66d2ca5ce5e7b9cff5" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedFrame": { "with": [], "status": "not_shared", "evidence": "East tunnel exit floors x 1832..1960 z -1824..-1764 vs observatory east end x 570..630 z 4160..4320: separate frames; joined by scene change" } },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-810, -632, -497], "max": [900, 354, 4320] },
    "sourceCollisionVertices": { "min": [-700, -632, -440], "max": [900, 354, 4320] },
    "unityCollisionVertices": { "min": [-7.0, -6.32, -43.2], "max": [9.0, 3.54, 4.4] }
  },
  "water": [
    { "id": "water.astral_observatory.wb00", "sourceWaterBox": { "xMin": -300, "xMax": -220, "zMin": 1040, "zMax": 1200, "surfaceY": -626, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [-3.0, -2.2], "z": [-12.0, -10.4], "surfaceY": -6.26 }, "poolFloorY": [-630, -630], "behavior": "UNKNOWN: 4 u film over the channel bed" },
    { "id": "water.astral_observatory.wb01", "sourceWaterBox": { "xMin": -220, "xMax": -140, "zMin": 1040, "zMax": 1140, "surfaceY": -626, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [-2.2, -1.4], "z": [-11.4, -10.4], "surfaceY": -6.26 }, "poolFloorY": [-630, -630], "behavior": "UNKNOWN: 4 u film" },
    { "id": "water.astral_observatory.wb02", "sourceWaterBox": { "xMin": 280, "xMax": 420, "zMin": 1400, "zMax": 1640, "surfaceY": -626, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [2.8, 4.2], "z": [-16.4, -14.0], "surfaceY": -6.26 }, "poolFloorY": [-630, -630], "behavior": "UNKNOWN: 4 u film" },
    { "id": "water.astral_observatory.wb03", "sourceWaterBox": { "xMin": 380, "xMax": 420, "zMin": 1260, "zMax": 1400, "surfaceY": -626, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [3.8, 4.2], "z": [-14.0, -12.6], "surfaceY": -6.26 }, "poolFloorY": [-630, -630], "behavior": "UNKNOWN: 4 u film" },
    { "id": "water.astral_observatory.wb04", "sourceWaterBox": { "xMin": 220, "xMax": 280, "zMin": 1500, "zMax": 1640, "surfaceY": -626, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [2.2, 2.8], "z": [-16.4, -15.0], "surfaceY": -6.26 }, "poolFloorY": [-630, -630], "behavior": "UNKNOWN: 4 u film" },
    { "id": "water.astral_observatory.wb05", "sourceWaterBox": { "xMin": 160, "xMax": 220, "zMin": 1580, "zMax": 1640, "surfaceY": -626, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [1.6, 2.2], "z": [-16.4, -15.8], "surfaceY": -6.26 }, "poolFloorY": [-630, -630], "behavior": "UNKNOWN: 4 u film" },
    { "id": "water.astral_observatory.wb06", "sourceWaterBox": { "xMin": 720, "xMax": 820, "zMin": 3260, "zMax": 3400, "surfaceY": -579, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [7.2, 8.2], "z": [-34.0, -32.6], "surfaceY": -5.79 }, "poolFloorY": [-580, -580], "behavior": "UNKNOWN: 1 u film over the bank" },
    { "id": "water.astral_observatory.wb07", "sourceWaterBox": { "xMin": 680, "xMax": 720, "zMin": 3360, "zMax": 3400, "surfaceY": -579, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [6.8, 7.2], "z": [-34.0, -33.6], "surfaceY": -5.79 }, "poolFloorY": [-580, -580], "behavior": "UNKNOWN: 1 u film" },
    { "id": "water.astral_observatory.wb08", "sourceWaterBox": { "xMin": 800, "xMax": 820, "zMin": 3200, "zMax": 3260, "surfaceY": -579, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [8.0, 8.2], "z": [-32.6, -32.0], "surfaceY": -5.79 }, "poolFloorY": [-580, -580], "behavior": "UNKNOWN: 1 u film" },
    { "id": "water.astral_observatory.wb09", "sourceWaterBox": { "xMin": -660, "xMax": 900, "zMin": 2180, "zMax": 3000, "surfaceY": -592, "properties": "0x00000300", "bgCamIndex": 0, "lightIndex": 3, "room": 0, "disabled": false }, "unity": { "x": [-6.6, 9.0], "z": [-30.0, -21.8], "surfaceY": -5.92 }, "poolFloorY": [-632, -632], "bankFloorY": [-600, -580], "behavior": "UNKNOWN: main channel, 40 u deep over the -632 bed; fish spawner r00 a03 inside" },
    { "id": "water.astral_observatory.wb10", "sourceWaterBox": { "xMin": 420, "xMax": 470, "zMin": 3310, "zMax": 3400, "surfaceY": -579, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [4.2, 4.7], "z": [-34.0, -33.1], "surfaceY": -5.79 }, "poolFloorY": [-580, -580], "behavior": "UNKNOWN: 1 u film" },
    { "id": "water.astral_observatory.wb11", "sourceWaterBox": { "xMin": 420, "xMax": 440, "zMin": 3260, "zMax": 3310, "surfaceY": -579, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [4.2, 4.4], "z": [-33.1, -32.6], "surfaceY": -5.79 }, "poolFloorY": [-580, -580], "behavior": "UNKNOWN: 1 u film" },
    { "id": "water.astral_observatory.wb12", "sourceWaterBox": { "xMin": 470, "xMax": 510, "zMin": 3340, "zMax": 3400, "surfaceY": -579, "properties": "0x00000000", "bgCamIndex": 0, "lightIndex": 0, "room": 0, "disabled": false }, "unity": { "x": [4.7, 5.1], "z": [-34.0, -33.4], "surfaceY": -5.79 }, "poolFloorY": [-580, -580], "behavior": "UNKNOWN: 1 u film" }
  ],
  "anchors": [
    { "id": "anchor.ao.spawn0_east_arrival", "source": [602, -472, 4125], "unity": [6.02, -4.72, -41.25], "status": "proposed, not surveyed" },
    { "id": "anchor.ao.hall_plane", "source": [60, -480, 440], "unity": [0.6, -4.8, -4.4], "status": "proposed, not surveyed" },
    { "id": "anchor.ao.spawn1_tf_door", "source": [-6, -129, 133], "unity": [-0.06, -1.29, -1.33], "status": "proposed, not surveyed" },
    { "id": "anchor.ao.spawn2_telescope_deck", "source": [107, -84, -209], "unity": [1.07, -0.84, 2.09], "status": "proposed, not surveyed" },
    { "id": "anchor.ao.door_tf", "source": [-7, -129, 188], "unity": [-0.07, -1.29, -1.88], "status": "proposed, not surveyed" }
  ],
  "rooms": [
    { "id": "loc.astral_observatory.room_00", "sourceRoom": "Z2_TENMON_DAI_room_00", "roomShape": "cullable, 12 entries", "displayLists": 26 },
    { "id": "loc.astral_observatory.room_01", "sourceRoom": "Z2_TENMON_DAI_room_01", "roomShape": "cullable, 20 entries", "displayLists": 37 }
  ],
  "components": [
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a00", "actorId": "0x05F", "params": "0x0001", "class": "static-mesh", "dls": ["object_fusen:0xA00", "object_fusen:0xD78"], "status": "selected, bob omitted" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a10", "actorId": "0x039", "params": "0x287F", "class": "static-mesh", "dls": ["object_syokudai:0x870", "gameplay_keep:0x7D590"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a11", "actorId": "0x039", "params": "0x287F", "class": "static-mesh", "dls": ["object_syokudai:0x870", "gameplay_keep:0x7D590"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a12", "actorId": "0x039", "params": "0x287F", "class": "static-mesh", "dls": ["object_syokudai:0x870", "gameplay_keep:0x7D590"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a13", "actorId": "0x039", "params": "0x287F", "class": "static-mesh", "dls": ["object_syokudai:0x870", "gameplay_keep:0x7D590"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a14", "actorId": "0x039", "params": "0x287F", "class": "static-mesh", "dls": ["object_syokudai:0x870", "gameplay_keep:0x7D590"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a15", "actorId": "0x039", "params": "0x287F", "class": "static-mesh", "dls": ["object_syokudai:0x870", "gameplay_keep:0x7D590"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a16", "actorId": "0x082", "params": "0x400A", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a17", "actorId": "0x082", "params": "0x460A", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a18", "actorId": "0x082", "params": "0x420E", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a19", "actorId": "0x082", "params": "0x440E", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a21", "actorId": "0x28D", "params": "0x0000", "class": "static-mesh", "dls": ["object_astr_obj:0x2380", "object_astr_obj:0x22E0"], "status": "selected, intact (switch 0 clear)" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a22", "actorId": "0x19C", "params": "0x907F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected, 12:00 hand pose" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_00/a23", "actorId": "0x006", "params": "0x50C0", "class": "posed-skeletal", "dls": ["object_box:0x66A0 gBoxChestSkel"], "status": "optional, closed pose" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_01/a03", "actorId": "0x283", "params": "0x0000", "class": "static-mesh", "dls": ["object_gi_reserve00:0xD78", "object_gi_reserve00:0xC80", "object_gi_reserve00:0x1C60"], "status": "selected, glow is a billboard" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_01/a04", "actorId": "0x082", "params": "0x4802", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_01/a05", "actorId": "0x082", "params": "0x4A02", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_01/a06", "actorId": "0x082", "params": "0x4C0E", "class": "static-mesh", "dls": ["gameplay_dangeon_keep:0x17EA0"], "status": "selected" },
    { "sourceKey": "Z2_TENMON_DAI/h0/room_01/a08", "actorId": "0x19C", "params": "0x907F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected, 12:00 hand pose" },
    { "sourceKey": "Z2_TENMON_DAI/h0/transition/ta00", "actorId": "0x005", "params": "0x007F", "class": "door-transition", "dls": ["gameplay_keep:0x22B28 gDoorSkel", "object_dor01:0x448"], "status": "selected, rest pose" },
    { "sourceKey": "Z2_TENMON_DAI/h0/transition/ta01", "actorId": "0x018", "params": "0x007F", "class": "door-transition", "dls": ["ovl_En_Holl:0x0C30 gEnHollCentralPlaneDL"], "status": "optional, hall plane hidden in a both-rooms render" }
  ],
  "collisionOwnership": { "room": "Z2_TENMON_DAICollisionHeader_007514 (1439 polys: room_00 polys 0-656, room_01 polys 657-1438)", "waterBoxes": 13, "actorDynapoly": ["r00 a00 object_fusen_Colheader_002420", "r00 a21 gAstralObservatoryBombWallCol"], "actorCylinders": null, "note": "cylinders NI except r01 a00 r18 h64 (bji.c:54)" },
  "portalIds": [
    "portal.astral_observatory.east_clock_town",
    "portal.astral_observatory.z2_00keikoku",
    "portal.astral_observatory.astral_observatory.r0_r1",
    "portal.astral_observatory.astral_observatory.r1_r0",
    "portal.astral_observatory.z2_00keikoku.telescope"
  ],
  "materialBindings": null,
  "cameraIds": ["cam.astral_observatory.entry", "cam.astral_observatory.reverse_door", "cam.astral_observatory.fixture", "cam.astral_observatory.room_01"],
  "routeIds": [],
  "omissions": ["r00 a01/a02 En_St", "r01 a00 En_Bji_01", "r01 a01 En_Kakasi", "r01 a02 En_Niw", "r00 a04-a08 drips, a09 flame and light, torch flames", "room_01 six cylindrical-billboard room DLs", "telescope view (external Z2_00KEIKOKU)", "Link and Tatl"],
  "excluded": { "halfDayMask": [], "stateOrSchedule": [], "noGeometry": ["r00 a03 Obj_Mure controller", "r00 a20 En_Test4", "r01 a07 En_Test4", "r01 a09 Obj_Sound"] },
  "unsupported": ["skeletal NPC and enemy poses", "billboard orientation", "two-layer texture scroll on 6 segments", "hall plane fade and room swap", "ladder climbing", "water film and channel behaviour", "telescope camera mode", "door exit through the respawn-down path"],
  "evidence": ["docs/city/interiors/astral_observatory.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Termina Field side of the door: arrival for `0x5490` (TF spawn 9) and which TF exit sends `0x4C10` | Extract `Z2_00KEIKOKU` (outside the 22) and read its spawn 9 entry and the exit list index holding `0x4C10` with its collision surface |
| U-02 | Telescope destination: TF spawn 10 pose and start mode (0xC expected); Dm_Stk csIds; cutscene cue 517; TF `Obj_Moon_Stone` params and position | Same extraction: TF start position 10, the actor lists holding `ACTOR_DM_STK`, `ACTOR_EN_FALL` type 8 and `ACTOR_OBJ_MOON_STONE`, and the cutscene script with `CS_CMD_ACTOR_CUE_517` |
| U-03 | Which Shikashi message carries the yes/no telescope choice | Read message entries 0x5DC-0x5F8 and 0x236A in the extracted message data for the choice control code; confirm `choiceIndex` 0 = yes (bji.c:227-236) |
| U-04 | Where the telescope mesh is in room_01 | Inspect room_01 DLs of the cullable entries near (60, 54, -135) r386 XLU, (60, 35, -135) r498 and (38, 157, -127) r202 after an approved room import; capture `cam.astral_observatory.fixture` |
| U-05 | Walkability of room_01 stairs (28-35.5 deg faces) and of channel to corridor links | Run a tolerant (vertex-snap, step-height) floor connectivity pass, then a controller route from spawn 0 to spawn 2 |
| U-06 | Whether the balloon's dynapoly fully blocks the ladder | Extract `object_fusen` and read `object_fusen_Colheader_002420` extents at scale 0.1 against walls 653/654 |
| U-07 | Fish spawner behaviour (`OBJMURE_TYPE_FISH`, chNum 3, ptn 3, svNum 1) | Read `ovl_Obj_Mure/z_obj_mure.c` child spawn and swim code |
| U-08 | Spawn bgCam index 0 (spawn 1) vs 0xFF (spawns 0, 2) | Same as North U-02: which code consumes `PLAYER_GET_BG_CAM_INDEX` |
| U-09 | Entrance flags `0x0102`, `0x4102`, `0x8115` and TF 10 `0x8A95` | Decode transition-type bits 7-13 (`src/code/z_play.c:2275`) and fade bits 0-6 (`src/code/z_scene.c:540`) to names |
| U-10 | Users of cutscene entries 0-8 (player chain), 10 and cs camera 0 (`START0`) | Read `Play_AssignPlayerCsIdsFromScene` (player.c:11261) and search for csId 10 users among runtime-spawned actors |
| U-11 | Which En_Test4 survives per entrance (a20 room_00 or a07 room_01) and its bell volume | Read room/actor spawn order in `Room_` / `Actor_SpawnSetupActors` for the first room of each entrance |
| U-12 | What the six cylindrical billboards in room_01 depict | Inspect DLs 0x5C60-0x7290 textures after import, or capture |
| U-13 | Effect of surface light index 1/2/31 and `lightMode true` | Read `Environment_ChangeLightSetting` and the skybox light-mode branch in `src/code/z_kankyo.c` against the 4 LightSettings |
| U-14 | Scarecrow time-skip reload room and DayTelop path | Trace `respawn[DOWN].roomIndex` (`unk_3CE`) through `Play_Init` room selection with `respawnFlag -4` / `-8` |
| U-15 | Collision vertices 0-30 unreferenced | Compare with the binary collision header (vertex count field) to confirm they are authored leftovers, not a ZAPD artifact |
| U-16 | Chest a23 reachable only through the bomb wall | Map walls and floors around (-636, -580, 3088) to confirm a closed alcove behind a21 |
| U-17 | Consumers of `OBJECT_OS_ANIME`, `OBJECT_HAKUGIN_OBJ`, `OBJECT_MKK` | Search runtime-spawned children of the listed actors (`En_Test`, `En_Clear_Tag`, `En_Mkk`) for these banks |
| U-18 | Arrival behaviour after the TF door exit (floor effect 2, `respawnFlag -2`) | With U-01, read the TF spawn 9 start mode; confirm the spawn-list pose is used (player.c:11279-11281) |
| U-19 | Guard texts 0x0725-0x073C and mask reactions | Read the message entries and `FACE_REACTION_SET_BOMBERS_HIDEOUT_GUARD` table in `src/code/z_face_reaction.c` |
| U-20 | How the player learns the code in normal play | Read `En_Bomjima` / Bombers notebook flow in North (REF-05 U-10) and the `z_message.c:2814` caller context |

**Disagreements with registers (reported, not edited):** PORTAL_REGISTER section 6 says "Observatory 1-3" for teleport-only spawns; the scene has spawns 0-2 (2.5). PORTAL_REGISTER `portal.astral_observatory.z2_00keikoku` "open hours UNKNOWN" is resolvable to always open (2.4). LOCATION_REGISTER counts (2 rooms, r00 15/24, r01 15/10, 2 transitions, 3 spawns, 2 exits, 4 entrance rows with the last padding) all agree with the C. LOCATION_REGISTER U-09 is answered: exit 0 is room_00 (polys 233/234), exit 1 is room_01 behind TA0 (polys 1040/1041).

**Not inspected:** room DL contents, combiners and vertex geometry beyond counts; LightSettings values; cutscene scripts and camera point semantics; message text; `Z2_00KEIKOKU` and every TF-side actor placement; Obj_Mure, En_St, En_Niw and En_Kakasi behaviour beyond the cited init/draw/gate lines; mask face reactions; 3DS material.

**Checks run:** read the brief, `docs/HANDOFF.md`, FULL_CITY_PLAN sections 6, 11 and 12, LOCATION_REGISTER, PORTAL_REGISTER, SOURCE_REGISTER (sections 2-5), `districts/north_clock_town.md`, `districts/east_clock_town.md` (2.3-2.7), `shared/city/schema/README.md`, `region.schema.json` and `ids.json` entries; read scene.c, r00.c, r01.c and the scene XML; read the overlays and engine lines cited above (19 actor overlays plus En_Bombers2, Dm_Stk, En_Fall, player.c, z_scene.c, z_play.c, z_actor.c, z_bgcheck.c, z_sram_NES.c, z_parameter.c); scratch stdlib scripts `col.py`, `idx.py`, `layout.py`, `actors.py`, `connect.py`, `verts.py`, `water.py`, `water2.py`, `probe2.py` (run with `python -I <path>`; outputs `*_out.txt` and `water.json` in the scratch folder) for collision counts, exit polygons, door probes, floor bands, actor decode and water depths; SHA-256 and sizes of the seven inputs; ZAPD binary hash matches SOURCE_REGISTER; lifted this section 10 block with `region_from_dossier.normalize` into the scratch folder only and validated it with `python -I tools/reference/validate_city.py <scratch>/astral_observatory.json`.

**NOT run:** ZAPD (no object or TF scene extracted), Blender, Unity, any player or executable, captures, renders, controller routes; `region_from_dossier.py` against `shared/` (nothing under `shared/` was written).

**Next dependency:** integrator review of section 10 and of the proposed telescope portal (registry edit in PORTAL_REGISTER, then `build_city_ids.py`); section 9 extraction for the static pilot; ROOM-03 (special-space pilot) needs PORT-02 for the room hall and a decision on how the external telescope view (Termina Field) is presented under the FULL_CITY_PLAN section 6 external boundary policy.
