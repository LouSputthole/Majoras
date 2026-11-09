# Bomb Shop (`loc.bomb_shop`): source inventory dossier

Task **REF-09 / INT-02** (FULL_CITY_PLAN.md section 12 row REF-09, section 6 row INT-02). Base commit `31b20dc` on `main` (HEAD moved to `7a7a08c`, PIPE-01, while this packet ran; IDs below were checked against its `shared/city/ids.json`). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_BOMYA` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_BOMYA/Z2_BOMYA.c`, `room.c` = `.../Z2_BOMYA_room_00.c`. Line numbers count `\n` only (both files contain lone `\r` bytes: 556 CR in scene.c, 1214 in room.c, some inside entries). N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected. One value family is decoded from a baserom binary rather than C (shop prices, section 5.3); it is tagged **C-BIN** and the method is stated.

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01), `docs/city/PORTAL_REGISTER.md` (REF-02), `docs/city/SOURCE_REGISTER.md` (REF-03), `docs/city/districts/west_clock_town.md` section 2.4.1 (the street door TA3). Every value those files state for the Bomb Shop agrees with the C below; wording notes are in section 11.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_BOMYA` / `SCENE_BOMYA` | `include/tables/scene_table.h:213` |
| Scene-table index | `0x68` | same line |
| Entrance-table scene index | `0x65` (`ENTR_SCENE_BOMB_SHOP`) | `src/code/z_scene_table.c:2674`, `include/z64scene.h:694` |
| Title text / draw config / restrictions / cycle flags | `0x10D` / `SCENE_DRAW_CFG_DEFAULT` / `RESTRICTIONS_INDOORS` / `PERSISTENT_CYCLE_FLAGS_NONE` | scene_table.h:213 |
| XML files | scene `Z2_BOMYA` segment 2; room `Z2_BOMYA_room_00` segment 3 with DLs `0x7508` and `0x7EA8` | XML `assets/xml/scenes/Z2_BOMYA/Z2_BOMYA.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene.c:13-30 | HDR |
| Room headers | default only (room.c:13-22) | HDR |
| Sound | `SOUND_SETTINGS(5, 19, 0)`: spec 5, ambience 19, seq 0 (`Sequence_0` = `NA_BGM_GENERAL_SFX` by table order); shop music comes from actor a07 (section 5.9) | HDR scene.c:14; macro `include/z64scene.h:845-846`; `include/tables/sequence_table.h:20` |
| Skybox | `SKYBOX_SETTINGS(0x00, 0, 0, true)`: no skybox, weather 0, light mode 1 = `LIGHT_MODE_SETTINGS` (fixed light setting, not time of day) | HDR scene.c:22; `include/z64scene.h:833-834`; `include/z64environment.h:48-50`; `src/code/z_scene.c:376` |
| Env light settings | 4 entries; only the fog word differs (`0x07D4` in entry 0, `0x07E1` in 1-3); values not transcribed | HDR scene.c:24, :55-64 |
| Other scene commands | `SET_REGION_VISITED`; actor cutscene list 13; actor cs camera list 5; minimap `{0xFFFF, 0x00E1, 0xFFE2, 0x003C, 0}` (mapId none, centre x 225, floor y -30, centre z 60); animated material `{0, 6, NULL}` (segment 0 is skipped) | HDR scene.c:17, :25-28, :93-99, :127-129; `src/code/z_scene_proc.c:400` |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x00, 0x00, 0, 0, false, 0)` (storm state 0, unlike the exteriors' 1); `SKYBOX_DISABLES(true, true)`; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3); room shape cullable (type 2, 14 entries); 9 objects, 8 actors | HDR room.c:14-20, :67-88 |

### 1.1 Alternate headers

None. The scene has no alternate header list, so every `sceneLayer` loads the default header (`Scene_CommandAltHeaderList` only runs when the command exists, `src/code/z_scene.c:464-474`). The entrance table has no layer rows either (`z_scene_table.c:2124-2135`). Day and night variation inside the shop comes only from actor half-day masks (section 4) and overlay time checks (section 5).

## 2. Spawns, entrances and exits

### 2.1 Start position (HDR scene.c:32-35; decode C)

Player params: start mode `(params & 0xF00) >> 8`, camera index `params & 0xFF` (`include/z64player.h:17-18`). Yaw = `(rawRotY >> 7) & 0x1FF` degrees.

| Spawn | Position | Raw rot X / Y / Z | Yaw | Params | Start mode | Camera index | Used by |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (61, -30, 366) | 0x0007 / 0x5A06 / 0x007F | 180 (faces -Z, into the shop) | 0x0F03 | 0xF `PLAYER_START_MODE_F` | 0x03 | Entrance 0 `0xCA00` from West exit 6 and entrance 1 `0xCA10` (no sender found); both entrance rows are `{spawn 0, room 0}` |

- Camera index 3 is an **actor cutscene camera**, not a `BgCamInfo` index: `Play_Init` passes `PLAYER_GET_BG_CAM_INDEX` to `Camera_ChangeActorCsCamIndex` (`src/code/z_play.c:2326-2327`), which looks up `Play_GetActorCsCamSetting` because the value lacks `CAM_DATA_IS_BG` (bit 12) (`src/code/z_camera.c:767-777`, :7806-7817; `z_play.c:1816-1820`). Entry 3 of `Z2_BOMYAActorCsCamInfo_000268` is setting 45 `CAM_SET_START2` at (42, 9, 387), 25 u from the spawn (C, HDR scene.c:111-113, :123; `include/z64camera.h:192`). This also answers North dossier U-02 in kind (spawn indices above the BgCamInfo count).
- RotY low 7 bits `0x06` are passed as the player's csId by `Actor_SpawnEntry` (`src/code/z_actor.c:3653`); whether Player reads it: NI.

Entrance table (C, `z_scene_table.c:2124-2135`): entrance 0 `{SCENE_BOMYA, 0, 0x4102}`, entrance 1 `{SCENE_BOMYA, 1, 0x0102}`. Scene `EntranceEntry` list `{0x00, 0x00}, {0x00, 0x00}` (HDR scene.c:45-48): entrance 1's `spawnNum` 1 selects row 1, which is again start position 0 in room 0. Flag semantics: NI (REF-02).

### 2.2 Exit list (HDR scene.c:50-53) and consumers

| Exit idx | Raw | Decode (`Entrance_GetTableEntry`, `z_scene_table.c:2688-2694`) | Destination row | Consumer |
| --- | --- | --- | --- | --- |
| 0 | `0xD460` | 0x6A `Z2_ICHIBA`, entrance 6, layer 0 | `sWestClockTownEntrance6` `{SCENE_ICHIBA, 6, 0x4102}` (`z_scene_table.c:2284-2286`) -> West spawn 6 (-1946, 165, -508) yaw 113, params 0x0D07 (`Z2_ICHIBA.c:51-52`) | surface type 5, floor polys 76, 77 |
| 1 | `0x0000` | n/a | n/a | none: no `setupExitList` reader in any of the 10 overlays spawned here (grep of En_Sob1, En_GirlA, En_Test4, En_Talk, En_Baba, Obj_Tokeidai, En_S_Goro, Obj_Hariko, Obj_Sound, En_Door). INF: alignment padding (LOCATION_REGISTER U-02) |

Inbound: West `Z2_ICHIBAExitList_0001A0[6] = 0xCA00` (`Z2_ICHIBA.c:92`; also its alternate exit list, `Z2_ICHIBA.c:1555`). No other city scene contains `0xCA00` or `0xCA10` (grep of all 22 extracted scenes). The debug map select also uses `ENTRANCE(BOMB_SHOP, 0)` (`src/overlays/gamestates/ovl_select/z_select.c:325`), excluded as in REF-02.

### 2.3 Transition actor and door decode (HDR scene.c:37-39; C)

| TA | Sides (front / back: room, bgCam) | Position | rotY raw | Yaw | Spawned params | Type / schedule type | Door art (OVL) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 0 / 0, 255 / 255 (-1 = generic door camera, `include/z64scene.h:369-373`) | (60, -30, 405) | -19329 = 0xB47F | `(-19329 >> 7) & 0x1FF` = 360 -> `DEG_TO_BINANG(360)` truncates to 0 (`include/z64math.h:168-172`): door forward = +Z; csId `0x7F` none | `TRANSITION_ACTOR_PARAMS(0, 0x0286)` = 0x0286 (`include/z64door.h:23`; `src/code/z_actor.c:3613-3619`) | `(p >> 7) & 7` = 5 `ENDOOR_TYPE_SCHEDULE`, `p & 0x7F` = 6 `ENDOOR_SCH_TYPE_BOMB_SHOP` (`ovl_En_Door/z_en_door.h:9-20`, :61, :95-96) | `gDoorSkel` (gameplay_keep 0x22B28) with `sObjectInfo[17 + 6]` = `DOOR_OBJINFO_23` = `DOOR_DL_DEFAULT` = `gDoorLeftDL` 0x20BB8 / `gDoorRightDL` 0x20D00, side chosen by eye yaw (`z_en_door.c:152`, :218, :307; `assets/xml/objects/gameplay_keep.xml:931-941`) |

Door probe (C, my evaluation, scratch `col.py`): when a non-fake door is opened the player raycasts the floor at `door + (-doorDirection x 22)` along the door yaw at door Y + 10 and passes that polygon to `Player_HandleExitsAndVoids` (`ovl_player_actor/z_player.c:6756`, :6779-6783, `func_80835BF8` :6548-6551). `doorDirection` is +1 on the door's local +Z side (`z_en_door.c:496`).

| Player side | Door-local z | Probe point | Floor hit | Result |
| --- | --- | --- | --- | --- |
| Inside the shop (spawn 0 is at local (1, -39)) | < 0, `doorDirection` -1 | (60, -20, 427) | poly 77, surface 5, exit+1 = 1 | exit 0 `0xD460` -> West spawn 6 |
| Outside pad (z > 405, never a spawn) | > 0, `doorDirection` +1 | (60, -20, 383) | poly 46, surface 4, no exit | door opens, no scene change |

The schedule only runs from the +Z side (`z_en_door.c:537`), i.e. from the exit pad that the player never stands on; `sDoorSch_BombShop` is `return_none` anyway (`ovl_En_Door/scheduleScripts.schl:159-162`). Leaving is always possible.

### 2.4 Collision surfaces carrying exits (C from scene.c:140-143 and the polygon list)

| Surface type | data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Kind |
| --- | --- | --- | --- | --- | --- |
| 5 | 0x00000101 / 0x000017C2 | 0 (`0xD460`) | 76, 77 | 0..120 / -30 / 420..540 | 2 flat floors (normal +Y); also ignore-camera xp flag. No conveyor bits (`data[1]` bits 18-26 = 0), unlike the North exits |

The pad sits 15-135 u behind the door on its axis; centroid of the pair (60, -30, 480) is 75 u from TA0 and 114 u from spawn 0, the REF-02 values.

### 2.5 Door-to-venue cross-check with West Clock Town (REF-06 section 2.4.1)

| Item | West side (street) | Bomb Shop side | Agree |
| --- | --- | --- | --- |
| Door actor | TA3 (-1988, 165, -490), rotY 14463 = 0x387F -> yaw 112, params 0x0286 (`Z2_ICHIBA.c:63`) | TA0 (60, -30, 405), yaw 0, params 0x0286 | yes: same `BOMB_SHOP` schedule type on both leaves |
| Exit trigger | polys 204, 205, exit 6 `0xCA00` | polys 76, 77, exit 0 `0xD460` | yes (REF-02 rows) |
| Arrival | Bomb Shop spawn 0 (61, -30, 366) yaw 180, mode 0xF | West spawn 6 (-1946, 165, -508) yaw 113, mode 0xD | yes |
| Schedule at Day 1 12:00 | none = openable | none; never evaluated from inside | yes; resolves REF-02 "open hours UNKNOWN" for this venue: always open |
| Leaf art | default keep door | same | yes |

The two scenes use independent frames (West Y 165 versus interior Y -30); no shared-frame claim is made.

### 2.6 Portal IDs (registered in `docs/city/PORTAL_REGISTER.md` and `shared/city/ids.json`)

| ID | Source evidence | Arrival |
| --- | --- | --- |
| `portal.bomb_shop.west_clock_town` | surface 5, exit 0 `0xD460`, TA0 | West spawn 6 |
| `portal.west_clock_town.bomb_shop` (inbound, West owns it) | West surface 11, exit 6 `0xCA00`, West TA3 | spawn 0 here |

Single room, so no `portal.bomb_shop.room_NN.room_MM` rows. No kickout `EN_TIME_TAG` in the actor list (contrast Trading Post).

## 3. Object banks (HDR room.c:24-34; IDs from `include/tables/object_table.h`)

Actor spawn fails if the profile's object is not loaded (`Actor_SpawnAsChildAndCutscene`, `src/code/z_actor.c:3517-3523`).

| # | Bank | Object ID | Consumer (OVL) | Needed for Day 1 static list |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_RSN` | 0x12C | En_Sob1 shop type 2 main object (`z_en_sob1.c:78-83`, :432-436) | skeletal (later) |
| 1 | `OBJECT_GI_BOMB_2` | 0x0B0 | En_GirlA `SI_BOMBCHU` (`z_en_girla.c:117`) | yes |
| 2 | `OBJECT_GI_BOMB_1` | 0x0A5 | En_GirlA `SI_BOMB_1` (`z_en_girla.c:119`) | yes |
| 3 | `OBJECT_KIBAKO` | 0x16F | no listed actor (Obj_Kibako uses it, `ovl_Obj_Kibako/z_obj_kibako.c:70`) | no |
| 4 | `OBJECT_GI_BOMBPOUCH` | 0x098 | En_GirlA `SI_BOMB_BAG_20_2` / `_30_2` (`z_en_girla.c:113-116`) | yes |
| 5 | `OBJECT_BBA` | 0x0DF | En_Baba (profile object) | no (Day 2 onward) |
| 6 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai wall clock | yes (already extracted) |
| 7 | `OBJECT_OF1D_MAP` | 0x0A1 | En_S_Goro (profile object and `Object_GetSlot`, `z_en_s_goro.c:78`, :1290-1294) | yes (rolled-up DL) |
| 8 | `OBJECT_HARIKO` | 0x1F2 | Obj_Hariko | yes |

Implicit: `gameplay_keep` 0x001 (En_Door skeleton and leaves, En_Test4, En_Talk, Obj_Sound, En_Sob1 profile object).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): ID `id & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF`, converted with `DEG_TO_BINANG` unless the ID carries the axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), in which case it is a plain integer (wrapped to -180..180). csId = `rawRotY & 0x7F` (0x7F -> none, 3570-3574). Half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all (2562-2565, 3576-3580).
- `Actor_InitHalfDaysBit` (2491-2499): Day 1 at 12:00 -> `0x080`. Bits: D0d 0x200, D0n 0x100, D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001 (`include/z64actor.h:328-341`).
- `Actor_Draw`: translate(world + yOffset x scale.y) x rotateYXZ(shape.rot) x scale (`z_actor.c:2819-2823`); `Actor_Init` copies `profile->id` into `actor->id` (`z_actor.c:3540`).
- Special flags here: a02 `EN_TALK | 0xE000` (all three axes are integers).

Classes as in the brief; "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (8 entries, HDR room.c:36-61, decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_SOB1` (0x135); runs with id `ACTOR_EN_OSSAN` 0x02A (profile, `z_en_sob1.c:66-67`) | (100, -30, -139) | 0x0007 / 0x0000 / 0x007F | 0 / 0 / 0 | 0x3FF all | 0 | 0x0002 | object_rsn | npc-scheduled | yes |
| 1 | `ACTOR_EN_TEST4` (0x15A) | (61, 11, 484) | 0x0907 / 0x000A / 0x007F | 18 / 0 / 0 | 0x3FF all | 10 | 0x0100 | gameplay_keep | controller-invisible | yes |
| 2 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (460, 38, 306) | 0x0107 / 0x877F / 0x007F | int 2 / int -90 / int 0 | 0x3FF all | none | 0xFE08 | gameplay_keep | controller-invisible | yes |
| 3 | `ACTOR_EN_BABA` (0x236) | (288, -30, 8) | 0x0000 / 0xA57F / 0x003F | 0 / 330 / 0 | 0x03F D2d to D4n | none | 0x7FFF | object_bba | npc-scheduled | **no** |
| 4 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (320, 121, -214) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x907F | object_obj_tokeidai | static-mesh | yes |
| 5 | `ACTOR_EN_S_GORO` (0x242) | (325, -30, 210) | 0x0007 / 0x8DFF / 0x007F | 0 / 283 / 0 | 0x3FF all | none | 0x0009 | object_oF1d_map | static-mesh (NPC drawn as a rolled-up DL until talked to) | yes |
| 6 | `ACTOR_OBJ_HARIKO` (0x20A) | (272, 2, 73) | 0x0007 / 0x9DFF / 0x007F | 0 / 315 / 0 | 0x3FF all | none | 0x0000 | object_hariko | static-mesh | yes |
| 7 | `ACTOR_OBJ_SOUND` (0x0F0) | (438, 127, 200) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0144 | gameplay_keep | controller-invisible | yes |

Transition actor TA0 (`EN_DOOR` 0x005, door-transition, all half-days) is in section 2.3.

### 4.3 Runtime children of a00 (OVL `z_en_sob1.c:371-383`, :1406-1410, :1484)

`EnSob1_InitShop` spawns three `ACTOR_EN_GIRLA` (0x002) at rot (0, 0, 0) from `sShops[BOMB_SHOP]` (`z_en_sob1.c:114-118`). The shelf is indexed right to left (`items[3]` comment "2 1 0", `z_en_sob1.h:49`).

| Key | Position | Params (shop item) | Get item / draw (OVL) | Bank, DLs (XML) |
| --- | --- | --- | --- | --- |
| a00/item0 | (221, -7, 73) | 0x17 `SI_BOMB_BAG_20_2` on a fresh save; 0x18 `SI_BOMB_BAG_30_2` when `WEEKEVENTREG_RECOVERED_STOLEN_BOMB_BAG` (`z_en_sob1.c:1408-1410`) | `GI_BOMB_BAG_20` / `GI_BOMB_BAG_30`; `GetItem_DrawUpgrades` (`src/code/z_draw.c:169-172`) | object_gi_bombpouch: `gGiBombBagDL` 0xB90, bag colour 0xAD0 (20) or 0xAF0 (30), ring colour 0xB30 (20) or 0xB50 (30), `gGiBombBagRingDL` 0xD98 |
| a00/item1 | (203, -7, 69) | 0x19 `SI_BOMBCHU` | `GI_BOMBCHUS_10`; `GetItem_DrawBombchu` (`z_draw.c:203-204`) | object_gi_bomb_2 `gGiBombchuDL` 0x4B0 |
| a00/item2 | (185, -7, 65) | 0x1A `SI_BOMB_1` | `GI_BOMBS_10`; `GetItem_DrawOpa0` (`z_draw.c:183-184`) | object_gi_bomb_1 `gGiBombDL` 0x9A0 |

Each item: scale 0.25, `shape.yOffset` 24 (draw Y = pos Y + 6), shadow 4, gravity 0, shape rot X forced 0; when selected it spins `rotY += 0x1F4` per frame, otherwise eases back to 0 (`z_en_girla.c:563-616`). All three stand on the counter top (poly 36/37 at Y -8; C, scratch probe).

### 4.4 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init / draw | Presence gating at Day 1 noon |
| --- | --- | --- |
| En_Sob1 (bomb shopkeeper) | shop type `p & 0x1F` = 2 `BOMB_SHOP`, path `(p & 0x3E0) >> 5` = 0 (`z_en_sob1.h:17-18`, :87-92); flex `gBombShopkeeperSkel` (object_rsn 0x9220), walk anim at play speed 2.0, scale 0.01, cylinder radius 50, mass immovable (`z_en_sob1.c:1386-1391`, :1417-1418, :1483); scene path 0 has 4 points (100,-30,-140), (160,-30,-111), (189,-30,-21), (186,-30,-8) (HDR scene.c:66-75) | always (killed only if `object_rsn` is missing, :432-436); pose depends on the player's position (5.2) |
| En_GirlA x3 | see 4.3 | spawned with the shopkeeper |
| En_Test4 | three-day controller; stars `(p >> 10) * 100` = 0; bell volume index `(p >> 5) & 0xF` = 8 (`z_en_test4.h:8-9`); csId 10 feeds both night and day transitions (entry 10's `additionalCsId` is 10, `z_en_test4.c:326-337`) | always |
| En_Talk | invisible target; text `(p & 0x3F) + 0x1C00` = 0x1C08; attention range type `home.rot.x - 1` = 1; offered within 40 u while facing it (`z_en_talk.h:6-7`; `z_en_talk.c` Init and `func_80BDE090`) | always; 50 u above the corner dais (section 7). Text content NI |
| En_Baba (Bomb Shop Lady) | type `(p & 0xC000) >> 14` = 1, path `(p & 0x3F00) >> 8` = 0x3F none (`z_en_baba.h:12-15`); in `SCENE_BOMYA` she is always visible in the bagless idle anim and waits in `EnBaba_Idle` (`z_en_baba.c:507-511`) | half-day mask only: absent Day 1, present Day 2 day to Day 3 night |
| Obj_Tokeidai type 9 `WALL_CLOCK` | `(p & 0xF000) >> 12` = 9; scale 0.02; `ObjTokeidai_Clock_Draw` with the tower clock's minute ring 0xCF28, centre/hand 0xBEE8, sun/moon panel 0xC368 and `gWallClockClockFaceDL` 0xF518 instead of the tower face; follows `CURRENT_TIME` every frame (`z_obj_tokeidai.h:6-19`; `z_obj_tokeidai.c:247-252`, :104-115, :719-723, :811-848) | always |
| En_S_Goro type 9 `BOMBSHOP` | `p & 0xF` = 9 (`z_en_s_goro.h:7`, :55); starts rolled up, `shape.yOffset` 14, `EnSGoro_ShopGoron_Idle` (`z_en_s_goro.c:1073-1077`); rolled-up draw = `gGoronRolledUpDL` (object_oF1d_map 0x91A8) at translate(world + (0, 14, 0)) x rotY x scale 0.01, ignoring `Actor_Draw`'s matrix (`MTXMODE_NEW`, :1440-1467); unrolls with `gGoronUnrollAnim` on talk (:1152-1188) | always; no init kill for type 9 |
| Obj_Hariko (cow-head toy) | scale 0.1; DLs `gHarikoBodyDL` 0x80 + `gHarikoFaceDL` 0x110; head bobs only while a quake is active (`z_obj_hariko.c:36-105`; `assets/xml/objects/object_hariko.xml:4-5`) | always; sits on the counter |
| Obj_Sound | type `(p >> 8) & 0xFF` = 1 `OBJ_SOUND_TYPE_BGM`, id `p & 0x7F` = 0x44 = `NA_BGM_SHOP` (ordinal 68, `sequence_table.h:156`); from its second update it calls `Audio_PlayObjSoundBgm(projectedPos, 0x44)` (`z_obj_sound.h:8-17`; `z_obj_sound.c:28-70`) | always; positional shop music source |
| En_Door | section 2.3 | always |

### 4.5 Counts

| | Default header |
| --- | --- |
| Room entries / runtime children / transition actors | 8 / 3 (En_GirlA) / 1 |
| Day 1 daytime candidates by mask | 7 (excluded: a03) |
| static-mesh | 3 (a04, a05, a06) + 3 children |
| npc-scheduled | 2 (a00, a03) |
| controller-invisible | 3 (a01, a02, a07) |
| door-transition | 1 (TA0) |
| billboard-effect / posed-skeletal | 0 / 0 actors (5 billboard quads live in room DLs, section 8) |

## 5. Venue mechanics index (INT-02 scope)

Index and cite only; no Unity design.

### 5.1 Who does what

| Role | Actor | Mechanism |
| --- | --- | --- |
| Shop counter (3 goods) | a00 En_Sob1 shop type `BOMB_SHOP` + 3 En_GirlA | `sShops[2]`, browse/select/buy state machine, cutscene cameras |
| Stock change after the Night 1 robbery | flag `WEEKEVENTREG_RECOVERED_STOLEN_BOMB_BAG` (33, 0x08) | slot 0 becomes the Big Bomb Bag (5.5) |
| Owner dialogue, Day 2 onward | a03 En_Baba | flag-driven talk, sets 73_01 / 73_02 / 79_20 (5.6) |
| Powder Keg sale | a05 En_S_Goro type 9 | Goron form + keg already in its slot (5.7) |
| Time and music | a01 En_Test4, a07 Obj_Sound | day transitions inside; positional `NA_BGM_SHOP` |

### 5.2 Shopkeeper position and talk zones (OVL `z_en_sob1.c`)

| Step | Condition | Source |
| --- | --- | --- |
| Init | after objects load: `EnSob1_InitShop` gets path 0, sets action `EnSob1_SetupWalk` (bomb shop only), spawns goods | :1394-1491, :1422-1427 |
| Wait | stays at (100, -30, -139) yaw 0 playing the walk anim until the player is inside x 0..390, z 72..365 | `EnSob1_SetupWalk` :543-550; `SkelAnime_Update` every frame :1493-1513 |
| Walk | speed 2.0 along path 0; next waypoint when closer than 5 u; may be talked to while walking (`wasTalkedToWhileWalking`) | `EnSob1_Walk` :773-801, `EnSob1_Walking` :803-835 |
| Sit | approaches the last point at 0.5, then `SitAtCounterStart` and loop anims at (186, -30, -8) | `EnSob1_EndWalk` :741-760, `EnSob1_SetupIdle` :762-771 |
| Talk offer | player inside x 138..200, z 72..160 (`sPosXZRanges[2]`), range 400 | :126-131, `EnSob1_Idle` :552-584 |

Spawn 0 (z 366) is 1 u outside the walk box, so the walk starts on the first step into the shop (C, geometry).

Text selection (text content NI; IDs only):

| Function | Bomb Shop rule | Source |
| --- | --- | --- |
| Welcome | by mask: none/Bunny/Postman 0x644; Goron/Zora/Deku 0x64B; Truth, All-Night, Keaton, Romani, Great Fairy, Don Gero, Kamaro, Bremen, Scents 0x685; Garo, Circus Leader, Gibdo, Captain 0x686; Couple 0x687; Stone 0x688; Blast 0x689; Kafei 0x68A (ends the talk, :644-645) | `EnSob1_GetWelcome` :226-268 |
| Talk option | `save.day == 1` and time >= 06:00 -> 0x648; else flag `RECOVERED_STOLEN_BOMB_BAG` -> 0x649; else 0x64A | `EnSob1_GetTalkOption` :187-197 |
| Goodbye | day 1 0x64C; day 2 0x64D; day 3 not night 0x64E; else 0x64F | `EnSob1_GetGoodbye` :347-360 |
| Refusals | facing shopkeeper 0x0640; no room 0x0641; cannot get now 0x0645; need rupees 0x0646; bought 0x0647; already have 0x658; have better 0x659 | :85-101, :1047-1054 |

### 5.3 Shop item table and prices

| Slot | Shop item | Text desc / choice | Qty | Get item | Price (rupees) | Can-buy rule (OVL `z_en_girla.c`) |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | `SI_BOMB_BAG_20_2` | 0x0654 / 0x0655 | bag | `GI_BOMB_BAG_20` | 50 | `UPG_BOMB_BAG == 1` already have; `>= 2` have better; rupees < price need rupees; else `CANBUY_RESULT_SUCCESS_1` (:289-300) |
| 0 (alt) | `SI_BOMB_BAG_30_2` | 0x0656 / 0x0657 | bag | `GI_BOMB_BAG_30` | 90 | `== 2` already have; `== 3` have better; rupees; else `SUCCESS_1` (:302-313) |
| 1 | `SI_BOMBCHU` | 0x0652 / 0x0653 | 10 | `GI_BOMBCHUS_10` | 40 | no bag -> cannot get now; ammo full -> no room; rupees; `SUCCESS_1` if `Item_CheckObtainability(ITEM_BOMBCHU) == ITEM_NONE`, else `SUCCESS_2` (:315-329) |
| 2 | `SI_BOMB_1` | 0x0650 / 0x0651 | 10 | `GI_BOMBS_10` | 30 | no bag -> cannot get now; ammo full -> no room; rupees; else `SUCCESS_2` (:331-342) |

Entries: `sShopItemEntries[0x17..0x1A]` (`z_en_girla.c:113-120`). Price mechanism (C): every can-buy compares `playerData.rupees` with `msgCtx.firstChoicePrice`, which `Message` decoding copies from bytes 5-6 of the current message header (`src/code/z_message.c:2073-2074`; header layout `include/message_data_fmt_nes.h:126-127`). The choice text is shown before the check (`EnSob1_HasPlayerSelectedItem`, `z_en_sob1.c:908-929`).

**Price values are C-BIN.** `assets/text/message_data.h` in this checkout is a stub, so the numbers are not in C. Method (scratch `msgprice.py`, numbers only, no text read): locate `sMessageTableNES` in `extracted/n64-us/baserom/code` (4589 ascending 8-byte entries at 0x1210D8, layout `include/message_data_static.h:6-10`), then read the 11-byte header at each entry's offset in `baserom/message_data_static`. Description and choice headers agree for every item: 0x0650/51 = 30, 0x0652/53 = 40, 0x0654/55 = 50, 0x0656/57 = 90, Powder Keg 0x0670 = 50; Curiosity Shop Big Bomb Bag 0x29DB/DC = 100 (cross-reference for INT-03). Second-choice price is 0xFFFF in all of them.

Stock: unlimited. `EnGirlA_Bought` hides the item (`isOutOfStock`, draw NULL) and `restockFunc` restores it when the purchase text closes (`z_en_girla.c:548-557`; `z_en_sob1.c:1092-1103`, :1130-1144). Selecting an out-of-stock item plays the error sound (:915-926).

### 5.4 Purchase transaction order (what is and is not atomic)

| Path | Rupees | Inventory | Source |
| --- | --- | --- | --- |
| `SUCCESS_1` (bags; first Bombchus) | `buyFanfareFunc` = `EnGirlA_BuyFanfare` adds `-price` to `rupeeAccumulator` on the decision frame | `Actor_OfferGetItem(getItemId)` each frame until the player takes it (`Actor_HasParent`), then the get-item handler gives the item | `z_en_sob1.c:981-997`, :959-969, :1105-1112; `z_en_girla.c:533-535` |
| `SUCCESS_2` (bombs, later Bombchus) | `buyFunc` calls `Item_Give` and `Rupees_ChangeBy(-price)` in the same call | same call | `z_en_girla.c:467-496`; `z_en_sob1.c:999-1013` |
| Powder Keg (a05) | `Rupees_ChangeBy(-powderKegPrice)` only after `Actor_HasParent` | get-item `GI_POWDER_KEG` offered first | `z_en_s_goro.c:1225-1247` |
| `Rupees_ChangeBy` | only edits `gSaveContext.rupeeAccumulator` | | `src/code/z_parameter.c:3431-3433` |
| HUD drain | 1 rupee per update, or 10 while the accumulator is <= -50; clamps at 0 | | `z_parameter.c:6952-6980` |

Bomb bag upgrades are single `Item_Give` branches (`ITEM_BOMB_BAG_20`: upgrade 1, bomb slot, ammo = capacity 1; `ITEM_BOMB_BAG_30`: upgrade 2, ammo = capacity 2; `z_parameter.c:2643-2661`). `EnGirlA_BuyBombBag` would give nothing (params 1/2/3 versus cases 20/21/22, decomp `@bug` note, `z_en_girla.c:446-465`), but the Bomb Shop never calls it because bag can-buys return `SUCCESS_1`. INF, not run: can-buy reads `playerData.rupees`, not the pending accumulator, so a second purchase started before the drain finishes is checked against undrained rupees (U-08).

### 5.5 Big Bomb Bag stock condition and related flags

| Flag (pack) | Set by | Read here | Cycle persistence |
| --- | --- | --- | --- |
| `WEEKEVENTREG_RECOVERED_STOLEN_BOMB_BAG` (33, 0x08), `z64save.h:1009` | En_Suttari (Sakon) when stopped on the Night 1 chase: `func_80BAD380` sets it if `flags2 & 8`, then transitions to `ENTRANCE(NORTH_CLOCK_TOWN, 7)` (`ovl_En_Suttari/z_en_suttari.c:1226`, :1258-1265) | En_Sob1 slot 0 swap (`z_en_sob1.c:1408-1410`), talk option (:193), En_Baba dialogue (`z_en_baba.c:194`) | cleared each cycle: byte 33 mask 0 (`src/code/z_sram_NES.c:123`) |
| `WEEKEVENTREG_SAKON_DEAD` (79, 0x40), `z64save.h:1380` | En_Suttari (`z_en_suttari.c:1249`) | North-only En_Baba autotalk and `sakonDeadTimer` (`z_en_baba.c:166-170`, :702-704) | cleared (byte 79 not listed) INF |
| `WEEKEVENTREG_73_01`, `73_02` (`z64save.h:1320-1321`) | En_Baba in the shop (`z_en_baba.c:231`, :251) | En_Baba (:195, :205) | byte 73 keeps only 73_08 (`z_sram_NES.c:230`) |
| `WEEKEVENTREG_79_20` (`z64save.h:1379`) | En_Baba, Deku form (`z_en_baba.c:180-182`) | same | INF cleared |

- The swap writes the overlay's static `sShops[2][0]` in place; the file never writes it back. INF: it is reset when the overlay is reloaded from ROM (U-09).
- Curiosity Shop counterpart (INT-03 owns it): on Day 3, if neither `RECOVERED_STOLEN_BOMB_BAG` nor `SAKON_DEAD` is set and the special item was not bought, the special slot is `SI_BOMB_BAG_30_1` (100), else All-Night Mask (`ovl_En_Fsn/z_en_fsn.c:315-324`).
- Blast Mask: given only by En_Baba's North autotalk path (type 1 at `ENTRANCE(NORTH_CLOCK_TOWN, 7)`), text 0x2A34 -> 0x2A35 -> `GI_MASK_BLAST`, skipped if `INV_CONTENT(ITEM_MASK_BLAST)` is already set (`z_en_baba.c:158-176`, :260-272, :526-541, :636-644). Not given inside the shop. In this room, wearing it changes only En_Sob1's welcome (0x689); En_Baba, En_S_Goro, En_GirlA and En_Talk contain no mask test (grep).

### 5.6 Bomb Shop Lady in the shop (a03, Day 2 onward; OVL `z_en_baba.c:154-280`)

| Player state | Text chain | Flags set |
| --- | --- | --- |
| Deku form | 0x2A37 first time, else 0x2A38 | 79_20 |
| bag not recovered, first talk | 0x660 -> 0x661 -> 0x662 -> 0x663 (focus alternates with the shopkeeper via `Actor_ChangeFocus`) | 73_01 |
| bag not recovered, later | 0x662 -> 0x663 | |
| bag recovered, first talk | 0x65A -> 0x65B -> 0x65C -> 0x65D | 73_02 |
| bag recovered, later | 0x65E -> 0x65F | |

She talks only after finding the shopkeeper by id `ACTOR_EN_OSSAN` (`EnBaba_FindBombShopkeeper`, :142-152; `EnBaba_Idle` :566-582, offer within 100 u). `EnBaba_Talk` also has a `TEXT_STATE_DONE` branch that sets 81_02 and transitions to North 7 (:628-633); whether any shop message ends that way: U-04.

### 5.7 Powder Keg sale (a05 En_S_Goro type 9; OVL `z_en_s_goro.c:513-705`)

| Player state | Chain | Sale |
| --- | --- | --- |
| Goron form, keg in its slot (`INV_CONTENT(ITEM_POWDER_KEG) == ITEM_POWDER_KEG`) | first 0x66E -> 0x66F, later 0x679; Day 3 night 0x677 -> 0x678 / 0x67A; then choice 0x670 | yes |
| Goron form, no keg | 0x67B -> 0x67C -> 0x67D, later 0x681 -> 0x682; Day 3 night 0x67E..0x680 / 0x683 -> 0x684 | no |
| Human, Deku, Zora | 0x664 -> 0x665 -> 0x666 -> 0x667, later 0x66C; Day 3 night 0x668..0x66B / 0x66D | no |

Choice 0x670 "yes" (:675-703): `AMMO(ITEM_POWDER_KEG) != 0` -> 0x673 refused; `powderKegPrice = firstChoicePrice` (50, C-BIN) and rupees short -> 0x674; else 0x675 (Day 3 night 0x676) and `GI_POWDER_KEG` (:1224-1229). "Talked" memory is the actor field `bombbuyFlags`, lost when the scene unloads (`z_en_s_goro.h:21`; bits `z_en_s_goro.c:57-67`). Day 3 night test is `save.day == 3 && save.isNight`.

### 5.8 Door schedule and open hours

| Item | Value | Source |
| --- | --- | --- |
| Street door (West TA3) and interior door (TA0) | both `ENDOOR_SCH_TYPE_BOMB_SHOP`; script `return_none` | `scheduleScripts.schl:159-162`; `z_en_door.c:56` |
| Kickout | none: no `EN_TIME_TAG` in the actor list | HDR room.c:36-61 |
| Staff presence | shopkeeper and Goron in every half-day; Lady Day 2 day to Day 3 night | masks, section 4.2 |
| Result | **open in every half-day**, including `prof.d1.noon.normal`; only dialogue changes with day/night (5.2, 5.7) | C |

### 5.9 Cameras, cutscenes, lighting and sound

| Item | Value | Source |
| --- | --- | --- |
| Shop cutscenes | a00 csId 0 -> entry 0 (lookForward, cs cam 0), `additionalCsId` 1 (lookToShelf, cam 1) -> 2 (lookToShopkeeper, cam 2) | `EnSob1_GetCutscenes` `z_en_sob1.c:1315-1319`; HDR scene.c:78-80 |
| Cs cameras (setting; pos; rot as binang -> deg; third triple raw) | 0: `CAM_SET_FIXED1` (31); (151, -2, 116); (0, 30401, 0) -> yaw 167; (-1, -1, -1). 1: FIXED1; (190, -2, 125); same rot; (-1, 75, -1). 2: FIXED1; (151, -2, 116); same; (-1, 75, -1). 3: `CAM_SET_START2` (45); (42, 9, 387); (1638, 29673, 0) -> pitch 9, yaw 163; (-1, 10, -1). 4: FIXED1; (248, 63, 106); (-1092, 29309, 0) -> pitch -6, yaw 161; (-1, -1, -1) | HDR scene.c:101-125; `include/z64camera.h:178`, :192; field layout `include/z64bgcheck.h:131-142` |
| Cam use | 0-2 shop; 3 arrival (spawn camera index, 2.1); 4 = entry 10, En_Test4 day/night transition | C |
| Unreferenced cs entries | 3-9, 11, 12 use negative (global) csCamId; no room actor's csId reaches them | HDR scene.c:81-87, :89-90; user NI |
| BgCam list | 0 `0x0000` `CAM_SET_NONE` (walls, counter top); 1 `0x003F` `CAM_SET_ROOM0` (every reachable floor) | HDR scene.c:135-138; `z64camera.h:147`, :210 |
| Door camera | both sides bgCam -1 (generic door camera) | 2.3 |
| Lighting | `LIGHT_MODE_SETTINGS`: fixed setting, so noon and night light the same (INF from `z_kankyo.c:631` default `lightSetting = 0`); every surface has light index 31 | 1; C |
| Music | scene seq 0; Obj_Sound plays `NA_BGM_SHOP` from (438, 127, 200) | 1, 4.4 |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header)

Venue **open** at 12:00 (5.8). Assumptions: fresh save (no week-event flags, so slot 0 = Bomb Bag 20), Human form, no mask. World = actorPos + actorRot x (actorScale x localVertex) except where a custom draw overrides it.

| Key | Component / params | N64 position (Unity) | Yaw | Scale | DLs, bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room_00 | room shell, counter, shelves | room origin | | 1 | `Z2_BOMYA_room_00DL_007508` (9 opaque) + `0x7EA8` (6 XLU) | 5 billboard quads inside (section 8) |
| a04 | wall clock 0x907F | (320, 121, -214) (3.20, 1.21, 2.14) | 0 | 0.02 | object_obj_tokeidai 0xCF28 ring, 0xBEE8 centre/hand, 0xF518 wall face, 0xC368 sun/moon panel | noon pose per `docs/ROM_REFERENCE.md:55-56`; panel offset (0, -1112, -19.6) x 0.02 = (0, -22.24, -0.39) |
| a05 | rolled-up Goron 0x0009 | (325, -30, 210); DL origin Y -16 (3.25, -0.16, -2.10) | 283 | 0.01 | object_oF1d_map `gGoronRolledUpDL` 0x91A8 | translate +14 unscaled, custom matrix |
| a06 | cow-head toy 0x0000 | (272, 2, 73) (2.72, 0.02, -0.73) | 315 | 0.1 | object_hariko 0x80 + 0x110 | on the counter |
| a00/item0 | Bomb Bag 20 (0x17) | (221, -7, 73); draw Y -1 (2.21, -0.01, -0.73) | 0 | 0.25 | object_gi_bombpouch 0xB90, 0xAD0, 0xB30, 0xD98 | Big Bomb Bag colours 0xAF0/0xB50 only with the flag |
| a00/item1 | Bombchu (0x19) | (203, -7, 69); draw Y -1 (2.03, -0.01, -0.69) | 0 | 0.25 | object_gi_bomb_2 0x4B0 | |
| a00/item2 | Bombs (0x1A) | (185, -7, 65); draw Y -1 (1.85, -0.01, -0.65) | 0 | 0.25 | object_gi_bomb_1 0x9A0 | |
| ta0 | door 0x0286 | (60, -30, 405) (0.60, -0.30, -4.05) | 0 | default | gameplay_keep `gDoorSkel` 0x22B28, leaf 0x20BB8 / 0x20D00 | closed pose; leaf side by camera yaw |

**Excluded by half-day mask:** a03 En_Baba (0x03F).

**Present but unimplemented in a static pass:** a00 En_Sob1 (`gBombShopkeeperSkel`); two source poses: waiting at (100, -30, -139) yaw 0 in the walk anim, or seated at (186, -30, -8) in `gBombShopkeeperSitAtCounterLoopAnim` (object_rsn 0x87BC). A capture must name which.

**No geometry:** a01 En_Test4, a02 En_Talk, a07 Obj_Sound.

Extraction commands are in section 9 (not executed).

## 7. Collision summary (C from scene.c:135-405; scratch `col.py`, `walls.py`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 122 / 124 / 7 |
| Water boxes | 0 (`0, NULL`, scene.c:404) |
| Header bounds | min (-24, -33, -225), max (522, 264, 540); Unity x -0.24..5.22, y -0.33..2.64, z -5.40..2.25 |
| Vertex extents | x -15..465, y -30..225, z -225..540 |
| Polygon kinds | 20 floors (18 flat, 2 at 54 deg), 96 walls, 8 ceilings |
| Materials (data[1] & 0xF) | stone (types 0, 1, 5): 76; wood (2, 4): 38; dirt (3): 8; carpet (6): 2 |
| Exit surfaces | 1 type, 2 floor polys (2.4) |
| Other flags | no floor type, wall type, floor property, hookshot, conveyor or soft bits on any type; echo 2 on all but type 3 (0); poly exclusion: 10 ignore-camera (door jambs 70-73, lintel 68-69, threshold 74-75, exit pad 76-77), 2 ignore-projectiles (42, 43) |
| Camera data | 2 BgCamInfo (5.9); floors use index 1 except the counter top (index 0) |

Floor levels (centroid Y; INF readings in the last column only):

| Y | Polys | Surface | Extent x / z | Reading |
| --- | --- | --- | --- | --- |
| -30 | 46, 47 | 4 wood | -15..465 / 60..390 | customer floor (spawn, Goron, Lady) |
| -30 | 48, 49 | 1 stone | 0..450 / -225..60 | back floor (shopkeeper path, wall clock above); no perimeter walls in collision |
| -30 | 74, 75 / 76, 77 | 4 / 5 | 30..90 / 390..420; 0..120 / 420..540 | threshold under the door; exit pad behind it |
| -13 | 114, 115 | 1 | 345..397 / 142..180 | 17 u block with walls 116-123, beside the Goron |
| -12 | 98, 99 | 6 carpet | 405..465 / 277..390 | 18 u corner dais (wall 96, 97); En_Talk 50 u above it |
| -8 | 100, 101 | 3 | 422..448 / 277..304 | small step on the dais |
| -8 | 36, 37 | 3 | quad (150,0) (135,58) (302,100) (317,42) | **counter top**, 22 u high; goods and toy sit on it |
| -3 | 112, 113 | 4 | 324..371 / 309..356 | 27 u box (walls 104-111) |
| 150..225 | 50, 51 | 1 | 41..125 / -45..11 | 54 deg slope above the back left block (not walkable) |

Counter (fixture): front face polys 40-43 (surface 3) from (135, 58) to (302, 100), Y -30..150; the upper pair 42-43 (Y -8..150) carries `COLPOLY_IGNORE_PROJECTILES` (`include/z64bgcheck.h:66-68`), so the player cannot climb onto the counter but projectiles pass. Ends: 38-39 at x 135..142 and 44-45 at x 302..310; partitions continue to the west wall (90-95, 18-19) and to the east curve (86-89, 84-85 at x 390). Front wall z = 390 (60-67, Y -30..120) with a 60 u doorway at x 30..90 (lintel Y 70); the doorway itself is closed by wall 102-103 (surface 2, x 30..90, Y -30..70, facing the room), so the player leaves only through the door action, never by walking onto the pad. East boundary: a curved wall from (390, 150) through x 352..361 to (465, 277) (polys 4-17) enclosing the Goron's nook. Upper walls lean inward from Y 120 to 210 (78-83, normals 0.95); beam ceiling at Y 180 over z 60..90 (2-3) with face 0-1. Back-left block (52-59) x 31..132, z -15..22.

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 37: 33 textures + 4 TLUTs (`Z2_BOMYATLUT_00C530`, `_00C730`, `_00C750`, `_00C950`) |
| By format | ci8 14, rgba16 11 (7 textures + 4 TLUTs), i8 5, ci4 4, rgba32 2, ia4 1 |
| Location / use | all in the scene file (segment 2); all 37 referenced by room DLs |
| Room `Gfx` DLs / `Vtx` arrays | 22 / 14 (14 `.vtx.inc`) |
| Raw triangles | 317 `gsSP2Triangles` + 20 `gsSP1Triangle` = 654 |
| Render modes | `AA_ZB_OPA_SURF2` 6 DLs, `AA_ZB_TEX_EDGE2` 2 (0x2AE0, 0x7250), `ZB_XLU_SURF2` 6; the other 8 DLs are wrappers or empty (0x2D50, 0x7508, 0x77D0, 0x7950, 0x7AD0, 0x7C50, 0x7C60, 0x7EA8) |
| Billboards | 5 DLs multiply by segment 0x01 (billboard matrix) after their own `Mtx`: 0x2C68 at (272, 78, 76) (in the opaque list via 0x2D50), 0x7720 (376, 60, -57), 0x78A0 (362, 45, -44), 0x7A20 (381, 45, -21), 0x7BA0 (352, 49, -12); purpose NI |
| Animated materials | none (section 1) |

Cullable entries (HDR room.c:73-88; triangle counts C):

| # | Centre / radius | Opaque DL (tris) | XLU DL (tris) |
| --- | --- | --- | --- |
| 0 | (206, 71, -61) / 130 | 0x1C60 (46) | |
| 1 | (408, 35, 218) / 84 | 0x6A00 (124) | |
| 2, 3, 4, 6 | (376,60,-57), (381,60,-21), (362,60,-44), (352,49,-12) / 22 | | billboards 0x77D0, 0x7AD0, 0x7950, 0x7C50 (2 each) |
| 5 | (380, 15, 31) / 146 | 0x2808 (72) | |
| 7 | (302, 76, 82) / 360 | 0x47C0 (248, 17 textures) | |
| 8 | (225, 105, 97) / 416 | 0x0BB0 (80) | |
| 9 | (416, -29, 247) / 164 | | 0x7DA8 (6) |
| 10 | (226, -19, 50) / 94 | 0x2AE0 (4) | 0x7C60 (empty) |
| 11 | (295, 117, 72) / 360 | 0x7250 (22) | |
| 12 | (272, 82, 76) / 28 | 0x2D50 -> billboard 0x2C68 (1) | |
| 13 | (90, 97, -42) / 158 | 0x1658 (43) | |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_BOMYA` C; `object_obj_tokeidai`. Binaries present under `extracted/n64-us/baserom/` (checked by `ls`): object_rsn 37424 B, object_gi_bombpouch 3680, object_gi_bomb_1 3248, object_gi_bomb_2 1696, object_oF1d_map 85392, object_hariko 1056, object_bba 41120, object_kibako 6912.

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_gi_bombpouch`, `object_gi_bomb_1`, `object_gi_bomb_2`, `object_hariko`, `object_oF1d_map`, `gameplay_keep` | goods, toy, rolled Goron, door |
| Later skeletal pass | `object_rsn`, `object_bba` | shopkeeper, Lady (Day 2) |
| None | `object_kibako` | no consumer in this room |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_gi_bombpouch','object_gi_bomb_1','object_gi_bomb_2','object_hariko','object_oF1d_map','object_rsn','object_bba')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

`gameplay_keep` is shared with North's list (not yet extracted; size and warnings to be checked separately). Message text (`assets/text/message_data.h`) is not extracted; a text extraction would replace the C-BIN price decode (U-03).

## 10. Proposed region record

**Proposal, schema not frozen at base `31b20dc` (PIPE-01 froze schema 1 at `7a7a08c`).** Same key names as North section 9; `sourceFrame.sharedFrame` uses the frozen field directly because an interior shares no exterior frame. A scratch run of `region_from_dossier.normalize` plus `validate_city.py` on this block is reported in section 11.

```json
{
  "_status": "proposal, schema not frozen (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.bomb_shop",
  "displayLabel": "Bomb Shop",
  "source": {
    "sceneSymbol": "Z2_BOMYA",
    "sceneEnum": "SCENE_BOMYA",
    "sceneTableIndex": "0x68",
    "entranceTableSceneIndex": "0x65",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0); scene has no alternate headers",
    "rooms": { "loc.bomb_shop.room_00": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event flags (slot 0 = Bomb Bag 20)", "player form Human, no mask", "shopkeeper pose UNKNOWN (waiting at path start or seated; player-position driven)"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_BOMYA", "bytes": 51568, "sha256": "8b02d101e7fe3e7e46d72e64780402cf50381b63ef9e28bfba2b6f5f080f7c73" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_BOMYA_room_00", "bytes": 32480, "sha256": "852151e1eef6d6db9c4a2216194a5d59ff95cba39175ce63bf7105ad8417d43b" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_BOMYA/Z2_BOMYA.c", "sha256": "017de0f8c72ae46a0f5f8552c994b538d5dcc1c5e13120a1f078c404cfe996f7" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_BOMYA/Z2_BOMYA_room_00.c", "sha256": "936b43b15b410246ed6e25cc96ab71fab76cd6eebfaa72b94f607002086d1be1" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_BOMYA/Z2_BOMYA.xml", "sha256": "d0bae3c2a7643d1d95e8a31e7523a0380597c6774c786c56f22fe41babe0fcf1" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedFrame": { "with": [], "status": "not_shared", "evidence": "interior scene with its own origin; West door at Y 165 vs interior Y -30 (section 2.5)" } },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-24, -33, -225], "max": [522, 264, 540] },
    "sourceCollisionVertices": { "min": [-15, -30, -225], "max": [465, 225, 540] },
    "unityCollisionVertices": { "min": [-0.15, -0.30, -5.40], "max": [4.65, 2.25, 2.25] }
  },
  "anchors": [
    { "id": "anchor.bs.spawn0_inside_door", "source": [61, -30, 366], "unity": [0.61, -0.30, -3.66], "status": "proposed" },
    { "id": "anchor.bs.door_ta0", "source": [60, -30, 405], "unity": [0.60, -0.30, -4.05], "status": "proposed" },
    { "id": "anchor.bs.counter_seat", "source": [186, -30, -8], "unity": [1.86, -0.30, 0.08], "status": "proposed" },
    { "id": "anchor.bs.counter_top_item1", "source": [203, -8, 69], "unity": [2.03, -0.08, -0.69], "status": "proposed" }
  ],
  "rooms": [ { "id": "loc.bomb_shop.room_00", "sourceRoom": "Z2_BOMYA_room_00", "roomShape": "cullable, 14 entries", "displayLists": 22 } ],
  "components": [
    { "sourceKey": "Z2_BOMYA/h0/room_00/a04", "actorId": "0x19C", "params": "0x907F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected, wall clock scale 0.02, time-posed" },
    { "sourceKey": "Z2_BOMYA/h0/room_00/a05", "actorId": "0x242", "params": "0x0009", "class": "static-mesh", "dls": ["object_oF1d_map:0x91A8"], "status": "selected, rolled-up idle DL; unrolls to skeleton on talk" },
    { "sourceKey": "Z2_BOMYA/h0/room_00/a06", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80", "object_hariko:0x110"], "status": "selected" },
    { "sourceKey": "Z2_BOMYA/h0/room_00/a00/item0", "actorId": "0x002", "params": "0x0017", "class": "static-mesh (spawned)", "dls": ["object_gi_bombpouch:0xB90", "object_gi_bombpouch:0xAD0", "object_gi_bombpouch:0xB30", "object_gi_bombpouch:0xD98"], "status": "selected, fresh save; 0x0018 with WEEKEVENTREG_RECOVERED_STOLEN_BOMB_BAG" },
    { "sourceKey": "Z2_BOMYA/h0/room_00/a00/item1", "actorId": "0x002", "params": "0x0019", "class": "static-mesh (spawned)", "dls": ["object_gi_bomb_2:0x4B0"], "status": "selected" },
    { "sourceKey": "Z2_BOMYA/h0/room_00/a00/item2", "actorId": "0x002", "params": "0x001A", "class": "static-mesh (spawned)", "dls": ["object_gi_bomb_1:0x9A0"], "status": "selected" },
    { "sourceKey": "Z2_BOMYA/h0/transition/ta0", "actorId": "0x005", "params": "0x0286", "class": "door/transition (posed skeleton)", "dls": ["gameplay_keep:0x22B28", "gameplay_keep:0x20BB8", "gameplay_keep:0x20D00"], "status": "selected, closed pose" }
  ],
  "collisionOwnership": { "room": "Z2_BOMYACollisionHeader_000D84 (124 polys)", "actorDynapoly": [], "actorCylinders": "NI (En_Sob1 radius 50, En_Baba, En_S_Goro)" },
  "portalIds": ["portal.bomb_shop.west_clock_town"],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a00 En_Sob1 skeletal shopkeeper (two source poses)", "Link and Tatl"],
  "excluded": { "halfDayMask": ["a03"], "stateOrSchedule": [], "noGeometry": ["a01", "a02", "a07"] },
  "unsupported": ["skeletal NPC poses", "5 room billboard quads (segment 0x01 matrix)", "wall clock time-driven pose unless implemented", "shop goods spin on selection", "Obj_Hariko quake bob", "positional shop BGM"],
  "evidence": ["docs/city/interiors/bomb_shop.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Trailing exit `0x0000` (LOCATION_REGISTER U-02) | Does any cutscene command or global code index `setupExitList[1]` in `SCENE_BOMYA`? No reader in the 10 overlays here; record padding unless one is found |
| U-02 | Sender of entrance 1 (`0xCA10`, flags 0x0102) | Grep `0xCA10` and `ENTRANCE(BOMB_SHOP, 1)` across non-city scenes and cutscene data; only map select uses entrance 0 |
| U-03 | Prices are C-BIN, not C | Extract message data (`assets/text/message_data.h` is a stub here) and confirm the headers of 0x0650-0x0657 and 0x0670 |
| U-04 | En_Baba `TEXT_STATE_DONE` branch (`z_en_baba.c:628-633`) inside the shop | Which end type do 0x65D, 0x65F, 0x663, 0x2A37, 0x2A38 use (message data)? If any ends as DONE, talking to her in the shop would transition to North 7 |
| U-05 | Which room DLs are the counter, shelves and display fixtures; what the 5 billboards are | Render room DLs per cullable entry (REF-03 capture `cam.bomb_shop.fixture`); no inference from names |
| U-06 | Visibility of the back area and wall clock from the customer floor | Back floor 48-49 has no perimeter walls in collision; render from spawn 0 and from the talk zone |
| U-07 | Users of cs entries 3-9, 11, 12 | Grep `CutsceneManager_GetAdditionalCsId` / fixed scene cs indices in En_Baba, En_S_Goro, Player get-item and door code |
| U-08 | Rupee drain race | Can a second `SUCCESS_2` purchase be confirmed before `rupeeAccumulator` drains (`z_en_girla.c:331-342` reads `playerData.rupees`)? Needs frame timing of `EnSob1_CanBuy` versus `z_parameter.c:6952-6980`; NOT RUN |
| U-09 | Persistence of the `sShops[2][0]` write (`z_en_sob1.c:1409`) | Confirm the overlay is freed and reloaded from ROM on scene change (actor overlay load/unload in `z_actor.c` / `z_actor_dlftbls.c`) |
| U-10 | `Item_CheckObtainability(ITEM_BOMBCHU) == ITEM_NONE` meaning for first-purchase fanfare | Read `Item_CheckObtainability` in `src/code/z_parameter.c` |
| U-11 | Light setting actually used under `LIGHT_MODE_SETTINGS`, and surface light index 31 | Trace `envCtx.lightSetting` writes for indoor scenes and `SurfaceType_GetLightSetting` consumers |
| U-12 | Whether Player uses the spawn csId 6 | Grep `this->actor.csId` in `z_player.c` |
| U-13 | Exact Day 1 pose of a00 for captures | Capture policy choice (REF-03): before or after the player enters the walk box |
| U-14 | En_Door dynapoly and actor collider shapes | Read `KnobDoorActor` init in `z_en_door.c` and the cylinder inits of En_Sob1, En_Baba, En_S_Goro |
| U-15 | Visual survey | Captures `cam.bomb_shop.entry`, `.reverse_door`, `.fixture` under `prof.d1.noon.normal`, plus a Day 2 capture with the Lady |

Register wording notes (not edits): PORTAL_REGISTER "open hours UNKNOWN" on both Bomb Shop rows is resolved here (always open). Its teleport-only list names "Bomb Shop 1"; the scene has one start position, so that item is entrance 1, which lands on start position 0. LOCATION_REGISTER counts agree (9/8, 1 transition, 1 spawn, 2 exits with trailing 0x0000, 2/2 entrances).

**Not inspected:** room DL contents and combiners; light-setting values; minimap draw; message text; En_Sob1 cursor/prompt drawing; En_S_Goro sleeping and lullaby code paths outside type 9; Obj_Kibako; overlay load/unload; Day 2+ layouts beyond masks; 3DS material.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN sections 6/11/12, the three registers, North and East templates, West 2.4.1, `shared/city/schema/README.md` and `ids.json`; read scene/room C and XML, scene/actor/object/sequence tables, `z_actor.c`, `z_scene.c`, `z_scene_table.c`, `z_play.c`, `z_camera.c`, `z_parameter.c`, `z_message.c`, `z_sram_NES.c`, `z_draw.c`, `z64bgcheck.h`, `z64player.h`, `z64door.h`, and the overlays En_Sob1, En_GirlA, En_Baba, En_S_Goro, En_Door, Obj_Tokeidai, Obj_Hariko, Obj_Sound, En_Talk, En_Test4 (params), En_Suttari (flag writer), En_Fsn (cross-reference). Scratch scripts in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\bomb_shop\`: `col.py` (collision, door probe, actor floors), `walls.py`, `dls.py`, `msgprice.py` (C-BIN prices). SHA-256 of five inputs and the ZAPD binary; grep of all 22 scenes for `0xCA00`/`0xCA10`; grep of the 10 overlays for `setupExitList`. Region block validated in scratch (see packet summary).

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or build, captures, renders, controller routes, message text extraction.

**Next dependency:** REF-03 captures for the three `cam.bomb_shop.*` IDs; PIPE-01 acceptance of section 10; section 9 extraction; ROOM-01 commercial pilot pattern (INT-01) before building this room.
