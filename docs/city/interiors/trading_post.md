# Trading Post (`loc.trading_post`): interior source dossier

Task **REF-09 / INT-01** (FULL_CITY_PLAN.md sections 6 and 12). Brief base commit `31b20dc65b5137de012fa60d373fb609c7af0aca` on `main`; HEAD when written was `7a7a08ce5d5589e4f80d65ece40e7ddfe1d47017` (PIPE-01 added `shared/city/`; nothing this dossier relies on changed). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_8ITEMSHOP` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, text extraction, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`, `shared/` or `tools/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_8ITEMSHOP/Z2_8ITEMSHOP.c`, `room.c` = `.../Z2_8ITEMSHOP_room_00.c`; neither file contains `\r`. `ovl/X` = `src/overlays/actors/ovl_X/`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected.

Cross-references, not redone: `docs/city/LOCATION_REGISTER.md` (REF-01 counts), `docs/city/PORTAL_REGISTER.md` (REF-02 rows), `docs/city/districts/west_clock_town.md` section 2.4.1 (the street side of the door). Every value below that those files also state agrees with them (checked); refinements are listed in section 11.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_8ITEMSHOP` / `SCENE_8ITEMSHOP`, display comment "Trading Post" (ANN) | `include/tables/scene_table.h:109-110` |
| Scene-table index | `0x34` | same line |
| Entrance-table scene index | `0x31` `ENTR_SCENE_TRADING_POST` | `src/code/z_scene_table.c:2622`; `include/z64scene.h:642` |
| Title text / draw config | `0x10F` / `SCENE_DRAW_CFG_MAT_ANIM` | scene_table.h:110 |
| Restrictions | `RESTRICTIONS_SET(0,0,0,0,0,3,0,0,3,0,0,1)`: equals `RESTRICTIONS_INDOORS` except bButton 0 instead of 1; field order hGauge, bButton, aButton, tradeItems, songOfTime, songOfDoubleTime, invSongOfTime, songOfSoaring, songOfStorms, masks, pictoBox, all | C: `src/code/z_parameter.c:66-75`; value semantics NI (U-08) |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_NONE` (no scene flag survives a cycle reset) | scene_table.h:10, :110 |
| XML | scene segment 2; room `Z2_8ITEMSHOP_room_00` segment 3 with DLs `0xC040`, `0xC9F0` | XML `assets/xml/scenes/Z2_8ITEMSHOP/Z2_8ITEMSHOP.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene or room | HDR scene.c:13-29, room.c:13-22 |
| Sound | `SOUND_SETTINGS(5, 19, 0)`: spec 5, ambience 19, seq 0 (`Sequence_0` = `NA_BGM_GENERAL_SFX` by table order, INF); shop music comes from actor a14 (section 4.4) | HDR scene.c:14; macro `include/z64scene.h:845-846`; `include/tables/sequence_table.h:20` |
| Skybox | `SKYBOX_SETTINGS(0x00, 0, 0, true)`: lightMode true, i.e. light setting not time-driven (SOURCE_REGISTER 2.6); room `SKYBOX_DISABLES(true, true)` | HDR scene.c:21, room.c:16; `include/z64scene.h:833-837` |
| Env light settings | 4 entries (not transcribed) | HDR scene.c:23, :54-63 |
| Region flag | `SCENE_CMD_SET_REGION_VISITED()` | HDR scene.c:17 |
| Special files | none (no `SCENE_CMD_SPECIAL_FILES`): no field/dungeon keep | HDR scene.c:13-29 |
| Actor cutscenes / cams | 16 entries / 7 cam infos (section 5.6) | HDR scene.c:24-25, :65-82, :116-124 |
| Minimap | one entry `{0xFFFF, 0, 0, 0, 0}`, scale 0 (no map) | HDR scene.c:84-90 |
| Animated materials | 2 entries, both type 1 (two-texture scroll): segment `1 + 7 = 8` params `{-1,-3,32,32},{2,-2,32,32}`; segment `-2 -> 9` (last) params `{-1,0,32,32},{0,0,32,32}` | HDR scene.c:130-143; C `src/code/z_scene_proc.c:389-406` |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x03, 0x00, 0, 0, false, 0)`: room type 3 `ROOM_TYPE_3` (not `ROOM_TYPE_INDOORS` = 2), storm 0; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3); cullable room shape, 12 entries | HDR room.c:14-18; C `include/z64scene.h:314-321`, `src/code/z_scene.c` `Scene_CommandRoomBehavior` |

`ROOM_TYPE_3` is never named in `src/`; the only `curRoom.type` tests compare against NORMAL, DUNGEON, INDOORS, 4 and BOSS (grep), so the INDOORS run-speed/attack/jump limits (z64scene.h:317) do not apply here (C); `z_camera.c:6984` falls to its default branch for type 3 (C, behavior of that branch NI).

## 2. Spawns, entrances and exits

### 2.1 Start position (HDR scene.c:31-34; decode C)

| Spawn | Position (Unity) | Raw rot X / Y / Z | Yaw | Params | Start mode | bgCam | Used by |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-1, 15, 189) (-0.01, 0.15, -1.89) | 0x0007 / 0x5A00 / 0x007F | 180 (faces -Z, into the room) | 0x0E06 | 0xE `PLAYER_START_MODE_E` (`include/z64player.h:18`, :35) | 6 | entrance 0 (West exit `0x6200`, `portal.west_clock_town.trading_post`) and entrance 1 (no city sender) |

bgCam 6 exceeds the 2-entry `BgCamInfo` list; `Play_Init` passes it to `Camera_ChangeActorCsCamIndex` (`src/code/z_play.c:2326-2327`), which without `CAM_DATA_IS_BG` reads the actor-cutscene camera list (`src/code/z_camera.c:772-778`, :7806-7832): cam info 6 = `CAM_SET_START0` 0x2C at (-30, 102, 209) (C; HDR scene.c:123; `include/z64camera.h:191`). This also answers North U-02's general question for interiors.

Entrance table (C, `z_scene_table.c:1028-1039`): entrance 0 `{SCENE_8ITEMSHOP, 0, 0x4102}`, entrance 1 `{SCENE_8ITEMSHOP, 1, 0x0102}`. Scene `EntranceEntry` list (HDR scene.c:44-47): `{0,0}`, `{0,0}`: both entrances land on spawn 0 in room 0. Entrance 1 = `0x6210`: no exit list among the 22 extracted scenes contains it and no `ENTRANCE(TRADING_POST, 1)` exists in `src/` (grep); the map-select row uses entrance 0 (`src/overlays/gamestates/ovl_select/z_select.c:331`).

### 2.2 Exit list (HDR scene.c:49-52) and consumers

| Exit idx | Raw | Decode (C) | Destination | Consumers in this scene |
| --- | --- | --- | --- | --- |
| 0 | `0xD450` | table 0x6A West, entrance 5, layer 0 | `sWestClockTownEntrance5` (`z_scene_table.c:2280-2282`) -> West spawn 5 at (-1760, 105, -244), yaw 125, params `0x0D08` (mode 0xD, bgCam 8), room 0 (`Z2_ICHIBA.c:49-50`, :78) | surface 4 floor polys 35, 36 (door probe and standing floor); a09 `En_Time_Tag` kickout, exit index `params & 0x1F = 0` |
| 1 | `0x0000` | would be Mayor's Residence e0 | none | no surface carries exit+1 = 2 and no actor reads index 1. The list starts at offset 0xA4 and the light settings at 0xA8, so the 4 bytes hold one real word plus alignment: **padding** (C for the absence of readers; INF for padding). Resolves LOCATION_REGISTER U-02 for this scene |

### 2.3 Collision surface carrying the exit (C from scene.c:150-153 and polygon lines 156 + index)

| Surface | data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Kind | Other flags |
| --- | --- | --- | --- | --- | --- | --- |
| 4 | 0x00000100 / 0x000017CA | 0 | 35, 36 (scene.c:191-192) | -60..60 / 15 / 240..360 | floor, normal (0, 1, 0) | wood; vIA xpFlag 1 (ignore camera); no conveyor bit |

The player reads exits only from floors (`ovl/player_actor/z_player.c:6405-6416`, call sites :6783 door probe and :12572 standing floor), so these two floors are the only exit trigger (C).

### 2.4 Door into the venue (transition actor; HDR scene.c:36-38; cross-check with West 2.4.1)

`EN_DOOR` params: type `(p >> 7) & 7`, type var `p & 0x7F` (`ovl/En_Door/z_en_door.h:95-96`); spawned params `TRANSITION_ACTOR_PARAMS(i, p) = (i << 10) + (p & 0x3FF)` (`include/z64door.h:22-23`); yaw `DEG_TO_BINANG((rotY >> 7) & 0x1FF)`, csId `rotY & 0x7F`, half-day mask always `HALFDAYBIT_ALL` (`src/code/z_actor.c:3600-3625`).

| Side | TA | Position | rotY raw / yaw | Rooms (front / back, bgCam) | Params (spawned) -> type / schedule | Leaf art | Exit polys (door-local) | Return start (door-local, yaw) |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Interior | `Z2_8ITEMSHOP` TA#0 | (0, 15, 225) | -19329 (0xB47F): 360 deg -> 0x10000 truncated to binang 0 (`include/z64math.h:168`, :172), i.e. yaw 0; csId none | 0 / 0, -1 / -1 (generic door camera, `include/z64scene.h:369-378`) | 0x0283 (0x0283) -> 5 `ENDOOR_TYPE_SCHEDULE` / 3 `ENDOOR_SCH_TYPE_TRADING_POST` (`z_en_door.h:49`, :58) | `object_wdor02` `gPostOfficeTradingPostDoorDL` 0x548 (`z_en_door.c:215`, :315; XML `object_wdor02.xml:5`) | 35, 36 at forward +15..+135 (centroid +75) | spawn 0 at (-1, 0, -36), yaw 180 |
| Street | `Z2_ICHIBA` TA#4 (`Z2_ICHIBA.c:64`) | (-1796, 105, -204) | 0x43FF / 135 | 0 / 0, -1 / -1 | 0x0283 (0x1283), same type | same DL | 206, 207 at (0, 0, -33) | West spawn 5 at (3, 0, +54), yaw 125 |

Mechanism (C): opening from the side with local z < 0 gives `doorDirection = -1` (`z_en_door.c:496`); the player is placed at door + 22 x doorDirection and the floor is probed at door - 22 x doorDirection, 10 above the door (`z_player.c:6756`, :6779-6783). From inside that probe is (0, 25, 247) and lands on poly 36 (scratch `probe.py`): the door exits only when opened from the room (C). The schedule runs only for `playerPosRelToDoor.z > 0` (`z_en_door.c:537`), which inside is the exit-pad side the player never stands on, so **the interior door always opens**; the street door uses the schedule (section 5.1). `ENDOOR_SCH_TYPE_TRADING_POST` names this venue on both sides (REF-02 independent check confirmed).

Door frame (C, polys 43-50): floor Y 15 at z 210..240, side walls x = +-30, ceiling Y 115: an opening 60 wide, 100 high, 30 deep. No wall crosses z 225 and schedule doors load no dynapoly (`z_en_door.c:338-341`; no collider in the overlay, grep): what stops a walk through the closed doorway onto the exit floor is U-02.

### 2.5 Portal IDs (all exist in `docs/city/PORTAL_REGISTER.md` and `shared/city/ids.json`)

| Portal ID | Mode | Source evidence | Arrival |
| --- | --- | --- | --- |
| `portal.west_clock_town.trading_post` (inbound) | door | West exit [5] `0x6200`, surface 12, polys 206/207, TA#4 | spawn 0 (-1, 15, 189) yaw 180 room 0 |
| `portal.trading_post.west_clock_town` | door | exit [0] `0xD450`, surface 4, polys 35/36, TA#0 | West spawn 5 (-1760, 105, -244) yaw 125 |
| `portal.trading_post.west_clock_town.kickout` | special | a09 `En_Time_Tag` params 0x0100 (room.c:72), exit [0] | same as above; one-way |

Single room: no internal `room_NN` portals. Entrance 1 is teleport-only (PORTAL_REGISTER section 6).

## 3. Object banks (HDR room.c:24-42; IDs from `include/tables/object_table.h`)

Actor spawn fails if the profile's object is not loaded (`src/code/z_actor.c:3517-3523`, as North).

| # | Bank | ID | Consumer (OVL) | Day 1 noon |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_FSN` | 0x1AB | a01 En_Ossan type 0 (`sObjectIds[0]`, `ovl/En_Ossan/z_en_ossan.c:134`) | skeletal (later) |
| 1 | `OBJECT_GI_LIQUID` | 0x0C1 | En_GirlA red/green potions (`ovl/En_GirlA/z_en_girla.c:77`, :87) | yes (item models) |
| 2 | `OBJECT_GI_SHIELD_2` | 0x0B3 | En_GirlA Hero's Shield (:79) | yes |
| 3 | `OBJECT_GI_NUTS` | 0x094 | En_GirlA Deku Nuts (:85) | yes |
| 4 | `OBJECT_GI_STICK` | 0x09F | En_GirlA Deku Stick (:81) | yes |
| 5 | `OBJECT_GI_ARROW` | 0x0AF | En_GirlA arrows (:75, :83) | yes |
| 6 | `OBJECT_KIBAKO` | 0x16F | no listed consumer | no (U-04) |
| 7 | `OBJECT_CS` | 0x110 | no listed consumer | no (U-04) |
| 8 | `OBJECT_WOOD02` | 0x061 | a02-a05 En_Wood02 (profile object, `ovl/En_Wood02/z_en_wood02.c:44`) | yes |
| 9 | `OBJECT_OS_ANIME` | 0x09D | no listed consumer | no (U-04) |
| 10 | `OBJECT_ANI` | 0x0C2 | a07 En_Ossan type 1 (`sObjectIds[1]`) | no (killed at noon) |
| 11 | `OBJECT_TSUBO` | 0x0F9 | a08 Obj_Tsubo type 2 (`ovl/Obj_Tsubo/z_obj_tsubo.c:72`) | yes |
| 12 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | a11 wall clock (`ovl/Obj_Tokeidai/z_obj_tokeidai.c:67`) | yes (already extracted) |
| 13 | `OBJECT_KA` | 0x11D | a12 En_Kakasi (`ovl/En_Kakasi/z_en_kakasi.c:76`) | skeletal (later) |
| 14 | `OBJECT_GI_BOTTLE_04` | 0x272 | En_GirlA fairy (`z_en_girla.c:73`) | yes |
| 15 | `OBJECT_WDOR02` | 0x275 | TA#0 door leaf (`z_en_door.c:215`) | yes |
| 16 | `OBJECT_HARIKO` | 0x1F2 | a13 Obj_Hariko | yes |

Implicit: `gameplay_keep` 0x001 (En_Test4, En_Time_Tag, En_Talk, Obj_Sound, En_Fish skeleton `gameplay_keep_Skel_02F028`, door skeleton `gDoorSkel`).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): ID `id & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF`, converted by `DEG_TO_BINANG` unless the ID carries the axis flag (0x8000 rotY, 0x4000 rotX, 0x2000 rotZ), in which case it is passed as a plain integer (values > 180 minus 360). csId `rawRotY & 0x7F` (0x7F = none); half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 = all (3570-3580).
- `Actor_InitHalfDaysBit` (2491-2499) and `Actor_SpawnSetupActors` (2546-2577): Day 1 12:00 -> bit 0x080; no entry here has ID bit 0x800.
- Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled`, `billboard-effect`, `controller-invisible`, `door-transition` (frozen vocabulary, `shared/city/schema/README.md`). "D1d" = mask includes Day 1 daytime.

### 4.2 Default header, room_00 (15 entries, HDR room.c:44-90, entry i at line 45 + 3i; decode C; scratch `parse_out.txt`)

| # | Actor (ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `EN_TEST4` (0x15A) | (-4, 15, 334) | 0x0007 / 0x000D / 0x007F | 0 / 0 / 0 | all | 13 | 0x0100 | gameplay_keep | controller-invisible | yes |
| 1 | `EN_OSSAN` (0x02A) | (4, 25, -146) | 0x0001 / 0x5A09 / 0x0078 | 0 / 180 / 0 | 0x0F8 D1d D1n D2d D2n D3d | 9 | 0x0000 | object_fsn | npc-scheduled | yes |
| 2 | `EN_WOOD02 \| 0x2000` (0x041) | (-190, 0, -50) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 (Z int) | all | none | 0xFF0B | object_wood02 | static-mesh | yes |
| 3 | `EN_WOOD02 \| 0x2000` | (-190, 0, -120) | same | same | all | none | 0xFF0B | object_wood02 | static-mesh | yes |
| 4 | `EN_WOOD02 \| 0x2000` | (177, 0, 89) | same | same | all | none | 0xFF0B | object_wood02 | static-mesh | yes |
| 5 | `EN_WOOD02 \| 0x2000` | (136, 0, 183) | same | same | all | none | 0xFF0B | object_wood02 | static-mesh | yes |
| 6 | `EN_FISH` (0x017) | (-120, -10, 23) | 0x0007 / 0x2D7F / 0x007F | 0 / 90 / 0 | all | none | 0x0001 | gameplay_keep | posed-skeletal (swimming) | yes |
| 7 | `EN_OSSAN` (0x02A) | (4, 25, -154) | 0x0001 / 0x0009 / 0x007C | 0 / 0 / 0 | 0x0FC D1d..D3n | 9 | 0x0001 | object_ani | npc-scheduled | yes by mask, killed by state |
| 8 | `OBJ_TSUBO \| 0x2000` (0x082) | (105, 43, 105) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 (Z int) | all | none | 0x030A | object_tsubo | static-mesh | yes |
| 9 | `EN_TIME_TAG \| 0xE000` (0x1AB) | (-101, 25, -101) | 0x0A87 / 0x007F / 0x007F | 21 / 0 / 0 (all int) | all | none | 0x0100 | gameplay_keep | controller-invisible | yes |
| 10 | `EN_TALK \| 0xE000` (0x261) | (177, 60, -92) | 0x0107 / 0x877F / 0x007F | 2 / -90 / 0 (all int) | all | none | 0xFE09 | gameplay_keep | controller-invisible | yes |
| 11 | `OBJ_TOKEIDAI` (0x19C) | (-98, 143, -206) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0xA07F | object_obj_tokeidai | static-mesh (time-posed) | yes |
| 12 | `EN_KAKASI` (0x0CA) | (-120, -13, 162) | 0x0007 / 0x2D0E / 0x007F | 0 / 90 / 0 | all | 14 | 0x0001 | object_ka | npc-scheduled | yes |
| 13 | `OBJ_HARIKO` (0x20A) | (34, 35, -157) | 0x0007 / 0xA6FF / 0x007F | 0 / 333 / 0 | all | none | 0x0000 | object_hariko | static-mesh | yes |
| 14 | `OBJ_SOUND` (0x0F0) | (12, 210, -169) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0x0144 | gameplay_keep | controller-invisible | yes |

Transition actor TA#0 `EN_DOOR` (0x005): section 2.4. Spawned children: the surviving En_Ossan spawns 8 `EN_GIRLA` (0x002) shop items (section 5.2).

### 4.3 Overlay findings per actor type (OVL)

| Actor | Init: params, scale, collision, children | Draw | Presence / state at Day 1 noon |
| --- | --- | --- | --- |
| En_Ossan (shopkeeper; file header "Trading Post Shop") | type `params`: 0 `ENOSSAN_CURIOSITY_SHOP_MAN` (object_fsn), 1 `ENOSSAN_PART_TIME_WORKER` (object_ani) (`z_en_ossan.h:69-72`, `.c:134`); scale 0.01 (:138, :1572); circle shadow 20 (:1514); `cylRadius` 50, immovable (:1523); spawns 8 En_GirlA from `sShops[params]` (:252-263, :1575) | flex skeleton `gFsnSkel` / `gAniSkel` (:1391-1401; XML `object_fsn.xml:105`, `object_ani.xml:24`); idle anim index 1 = scratch back / standing loop (:1573-1574) | `EnOssan_CheckValidSpawn` at Init (:206-233): Day 1-2, 06:00 < t <= 21:30 kills type 1, else kills type 0; Day 3 kills type 0 always and type 1 if t > 22:00 or t < 06:00. At 12:00 Day 1: a01 stays, a07 is killed |
| En_GirlA (shop item) | `params` = `EnGirlAShopItemId` into `sShopItemEntries` (`z_en_girla.c:66-153`); object from the entry (:159-176 `EnGirlA_InitObjIndex`); scale 0.25, yOffset 24, gravity 0, floorHeight = home Y (:563-601) | `GetItem_Draw(getItemDrawId)` after the entry's draw helper, Y-rotating while selected (:603-631) | drawn until bought (section 5.2) |
| En_Wood02 | `params & 0xFF = 0x0B` `WOOD_BUSH_GREEN_SMALL`; `(p >> 8) = 0xFF` -> `unk_144 = -1` (:180-188); no cylinder for types >= small bush (:193); scale stays 1.0 (:171, :291) | drawType 3 `WOOD_DRAW_BUSH_GREEN` (:275-289); `D_808C4D70[3]` is NULL so one XLU DL `object_wood02_DL_000090` (:81-84, :505-510; XML `object_wood02.xml:6`) | always |
| En_Fish | params 1 `ENFISH_1`: gravity 0, swim action `func_8091F344` (:217-218, :768-780); skeleton `gameplay_keep_Skel_02F028` (:205) | skeletal | always; swims in water box 1 (fish Y -10, floor -15, surface -6) |
| Obj_Tsubo | type `(p >> 7) & 3 = 2`: `OBJECT_TSUBO`, scale 0.197, `gPotDL`, collider r12 h32 (`z_obj_tsubo.c:72`, :173-212; XML `object_tsubo.xml:7`); drops: `P003F = 0x0A` -> `D_801AE194[10] = ITEM00_RECOVERY_HEART` with collectible flag `(p >> 9) & 0x7F = 1`, plus random table `P000F << 4` (:116-131, :483-489; `src/code/z_en_item00.c:1453-1461`) | opaque `gPotDL` 0x17C0 (:703) | always; breakable, flag 1 resets with the cycle (no persistent flags) |
| En_Time_Tag | type `(p & 0xE000) >> 13 = 0` `TIMETAG_KICKOUT_DOOR`, text `(p & 0x1FE0) >> 5 = 8` `TIMETAG_KICKOUT_TEXT_TRADING_POST`, exit `p & 0x1F = 0`; hour = home.rot.x = 21, minute = home.rot.y = 0 (`z_en_time_tag.h:10-15`, :51) | none | always (section 5.1) |
| En_Talk ("green target spot") | text `(p & 0x3F) + 0x1C00 = 0x1C09`; attention range type `home.rot.x - 1 = 1`; scale 1.0 (`z_en_talk.h:6-7`; `.c:29-40`) | none (profile draw NULL) | always; offers talk within 40 u when facing (`.c:57`); text content NI (U-05) |
| Obj_Tokeidai | type `(p & 0xF000) >> 12 = 10` `OBJ_TOKEIDAI_TYPE_SMALL_WALL_CLOCK`: scale 0.01, `ObjTokeidai_Clock_Draw`, `ObjTokeidai_Clock_Init` from `CURRENT_TIME` (`z_obj_tokeidai.h:8-21`; `.c:205-259`, :104-115) | minute ring 0xCF28, center/hand 0xBEE8, `gWallClockClockFaceDL` 0xF518 (instead of the tower face), sun/moon panel 0xC368 (`.c:811-850`; XML `object_obj_tokeidai.xml:47-67`) | always; follows the clock each frame (`ObjTokeidai_WallClock_Idle`, :719-723) |
| En_Kakasi (scarecrow) | above-ground `params & 1 = 1`; summon distance `(p >> 8) = 0`; scale 0.01; flex `object_ka_Skel_0065B0` (`z_en_kakasi.h:45-47`; `.c:160-210`) | skeletal | `WEEKEVENTREG_79_08` clear and `83_01` clear on a fresh save (INF) -> standing idle in the pool (`.c:196-210`) |
| Obj_Hariko ("little cow statue head") | scale 0.1; bobs its head while a quake is active (`z_obj_hariko.c:36-85`) | `gHarikoBodyDL` 0x80 + `gHarikoFaceDL` 0x110 (`.c:87-105`; XML `object_hariko.xml:4-5`) | always |
| Obj_Sound | type `(p >> 8) = 1` `OBJ_SOUND_TYPE_BGM`, id `p & 0x7F = 0x44` (`z_obj_sound.h:8-17`; `.c:28-38`) | none; `Audio_PlayObjSoundBgm(projectedPos, 0x44)` from the second update (`.c:48-66`); 0x44 = `Sequence_68` `NA_BGM_SHOP` (`sequence_table.h:155-156`, by table order) | always |
| En_Test4 | three-day controller; stars `(p >> 10) x 100 = 0`, bell volume index `(p >> 5) & 0xF = 8`; uses csId 13 for both night and day transitions because entry 13's additional cs is 13 (`z_en_test4.h:8-9`; `.c:322-337`) | none | always |
| En_Door | section 2.4 | `gDoorSkel` with limb 4 replaced by the venue DL (West 2.4.1) | always |

### 4.4 Counts

| | Default (only) header |
| --- | --- |
| Entries / transition actors / spawned children | 15 / 1 / 8 En_GirlA |
| Day 1 daytime candidates by mask | 15 of 15 |
| Present at Day 1 noon after state checks | 14 (a07 killed by `EnOssan_CheckValidSpawn`) |
| static-mesh / posed-skeletal / npc-scheduled / controller-invisible / door-transition | 7 / 1 / 3 / 4 / 1 |

## 5. Venue mechanics index (INT-01 scope: door/return, shop transaction, time conditions)

### 5.1 Open hours and time conditions

| Condition | Rule | Citation |
| --- | --- | --- |
| Street door schedule | `sDoorSch_TradingPost`: `if_time_range (21, 0, 22, 0)` -> `return_s (0xB)` (door closed, text 0x1800 + 0xB = 0x180B), else `return_none` (openable). Range test is `start <= now <= end - 1`, so closed 21:00 through 21:59 | OVL `ovl/En_Door/scheduleScripts.schl:114-121`; `z_en_door.c:53`, :537-541; `src/code/z_schedule.c:33-42` |
| Interior door | always openable (schedule only from local +z, section 2.4) | `z_en_door.c:537` |
| Kickout | when `hour == 21 && minute == 0` and not in cutscene mode: player held, text 0x1883 + 8 = 0x188B, then `nextEntrance = setupExitList[0]` (0xD450) with door SFX. If `WEEKEVENTREG_KICKOUT_WAIT` is set the kickout is deferred until both wait flags clear; no Trading Post actor sets that flag (setters: En_Aob_01, En_Fu, En_Mm3, En_Syateki_Man, En_Takaraya, grep) | `z_en_time_tag.c:50-90`, :285-306, :315-339; `include/z64save.h:1227-1229` |
| Shopkeeper roster | masks (a01 D1d-D3d, a07 D1d-D3n) plus `EnOssan_CheckValidSpawn` at Init only (table below) | room.c:48-50, :66-68; `z_en_ossan.c:206-233` |
| Clock in the room | `TIME_SETTINGS(255, 255, 3)`: normal speed 3 (5 without Ocarina); time stops while most message boxes are open | room.c:17; SOURCE_REGISTER 2.2 (`src/code/z_kankyo.c:1202-1215`, `z_scene.c:399-402`) |
| Scarecrow time skip | after its dance: time set to 06:00 (with DayTelop) if `t > 18:00 or t < 06:00`, else 18:00; respawn at spawn 0 (`Entrance_CreateFromSpawn(0)`, `RESPAWN_MODE_DOWN`); sets `WEEKEVENTREG_83_01` | `z_en_kakasi.c:953-970` |

Derived roster and door state (C from the rules above; INF where two rules interact; not observed):

| Window | Door from street | Present at counter (if loaded then) | Stock |
| --- | --- | --- | --- |
| Day 1-2, 06:00 < t < 21:00 | open | Curiosity Shop Man (a01) | `sShops[0]` |
| Day 1-3, 21:00-21:59 | closed (0x180B); occupants are kicked out at 21:00 | - | - |
| Day 1-2, 22:00 to 06:00 | open | Part-time worker (a07): t > 21:30 or t <= 06:00 | `sShops[1]` |
| Day 3, 06:00 < t < 21:00 | open | Part-time worker (a07); a01 is killed on day 3 | `sShops[1]` |
| Day 3, 22:00 to 06:00 (final hours) | open | nobody (a07 killed after 22:00 or before 06:00) | none |

**Day 1 noon (`prof.d1.noon.normal`): open** (12:00 is outside 21:00-21:59), Curiosity Shop Man present, day stock. Closes SOURCE_REGISTER U-10 for this venue.

### 5.2 Shop item tables (`sShops`, `z_en_ossan.c:140-158`; entries `z_en_girla.c:66-153`)

Spawn positions are world coordinates (`Actor_Spawn` with `spawnPos`, :252-263). Shelf slots per `z_en_ossan.h:63-67`: odd = upper row (Y 92), even = lower row (Y 68); slots 0-3 at x 50/80 (right shelf, player's right when facing -Z), 4-7 at x -50/-80 (left). All at z -195, behind the counter, over the Y 25 platform.

| Slot | Position | Day (`sShops[0]`, type 0) | Night (`sShops[1]`, type 1) | Object / draw ID | Qty param | Day text desc / choice | Night text desc / choice | GetItem | CanBuy / Buy |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (50, 68, -195) | `SI_NUTS_1` 0x09 | `SI_NUTS_2` 0x0F | gi_nuts / `GID_DEKU_NUTS` | 10 | 0x06B0 / 0x06B1 | 0x06CD / 0x06CE | `GI_DEKU_NUTS_10` | Nuts / BuyNuts |
| 1 | (50, 92, -195) | `SI_STICK_1` 0x07 | `SI_STICK_2` 0x10 | gi_stick / `GID_DEKU_STICK` | 1 | 0x06B4 / 0x06B5 | 0x06D1 / 0x06D2 | `GI_DEKU_STICKS_1` | Stick / BuyStick |
| 2 | (80, 68, -195) | `SI_ARROWS_LARGE_1` 0x04 | `SI_ARROWS_LARGE_2` 0x0D | gi_arrow / `GID_ARROWS_LARGE` | 50 | 0x06BA / 0x06BB | 0x06D7 / 0x06D8 | `GI_ARROWS_40` | Arrows / BuyArrows |
| 3 | (80, 92, -195) | `SI_ARROWS_MEDIUM_1` 0x08 | `SI_ARROWS_MEDIUM_2` 0x0C | gi_arrow / `GID_ARROWS_MEDIUM` | 30 | 0x06B8 / 0x06B9 | 0x06D5 / 0x06D6 | `GI_ARROWS_30` | Arrows / BuyArrows |
| 4 | (-50, 68, -195) | `SI_FAIRY_1` 0x03 | `SI_FAIRY_2` 0x0B | gi_bottle_04 / `GID_FAIRY` | 1 | 0x06B6 / 0x06B7 | 0x06D3 / 0x06D4 | `GI_FAIRY` | Fairy / BuyBottleItem |
| 5 | (-50, 92, -195) | `SI_POTION_GREEN_2` 0x05 | `SI_POTION_GREEN_3` 0x0E | gi_liquid / `GID_POTION_GREEN` | 1 | 0x06B2 / 0x06B3 | 0x06CF / 0x06D0 | `GI_POTION_GREEN` | PotionGreen / BuyBottleItem |
| 6 | (-80, 68, -195) | `SI_SHIELD_HERO_1` 0x06 | `SI_SHIELD_HERO_2` 0x11 | gi_shield_2 / `GID_SHIELD_HERO` | 1 | 0x06AE / 0x06AF | 0x06CB / 0x06CC | `GI_SHIELD_HERO` | ShieldHero / BuyShieldHero |
| 7 | (-80, 92, -195) | `SI_POTION_RED_2` 0x0A | `SI_POTION_RED_3` 0x12 | gi_liquid / `GID_POTION_RED` | 1 | 0x06AC / 0x06AD | 0x06C9 / 0x06CA | `GI_POTION_RED` | PotionRed / BuyBottleItem |

Day and night stock are the same eight item kinds at the same slots; only the SI ids and message ids differ. Selected items lerp to (35, 68, -130) (right shelf) or (-35, 68, -130) (left) (`EnOssan_PositionSelectedItem`, :1207-1225; tween :1235-1260).

### 5.3 Funds, capacity, stock (OVL `z_en_girla.c` unless noted)

| Mechanic | Implementation |
| --- | --- |
| Price | **not a C constant**: `play->msgCtx.firstChoicePrice` is read from the item's choice message control bytes (`src/code/z_message.c:2073-2074`). The message text is not extracted in this checkout (`assets/text/message_data.h` only includes the generated `text/message_data.h`; binary `extracted/n64-us/baserom/message_data_static` exists): price values NI (U-01) |
| Funds | every CanBuy compares `gSaveContext.save.saveInfo.playerData.rupees` (`include/z64save.h:276`) with `firstChoicePrice` -> `CANBUY_RESULT_NEED_RUPEES` (e.g. :191) |
| Payment | `Rupees_ChangeBy(-firstChoicePrice)` in every Buy function (:407, :412, :428, :433, :438, :534) adds to `gSaveContext.rupeeAccumulator` (`src/code/z_parameter.c:3431-3433`), which the HUD drains later (:6954 onward): the wallet changes over several frames, not in one write |
| Capacity checks | Nuts: full only if capacity != 0 and `AMMO >= capacity` (:236-247); Stick: same (:259-270); Arrows: no quiver -> `CANNOT_GET_NOW_2`, full -> `NO_ROOM_2` (:223-234); Fairy and potions: need an empty bottle (`Inventory_HasEmptyBottle`, `z_parameter.c:3200`) (:187-205, :368-376); Hero's Shield: any shield equipped -> `NO_ROOM` (:249-257) |
| Grant | `SUCCESS_2` (potions, fairy, arrows; nuts/sticks once obtainable): `buyFunc` gives the item at once (`Item_Give` or `Inventory_ChangeAmmo`, arrows clamp to quiver capacity, `z_parameter.c:3463-3470`) and the shopkeeper shows the buy-success text. `SUCCESS_1` (Hero's Shield; nuts/sticks when `Item_CheckObtainability == ITEM_NONE`): `buyFanfareFunc` takes the rupees, then `Actor_OfferGetItem` runs the get-item fanfare (`z_en_ossan.c:1015-1040`, :993-1003, :1116-1123) |
| Stock | `EnGirlA_Bought`: `isOutOfStock = true`, `draw = NULL` (:548-551); `EnGirlA_Restock` reverses it (:553-556), called on the next text advance (`z_en_ossan.c:1110` after SUCCESS_2; :1151, :1174 after SUCCESS_1). Stock is unlimited; no save field, week-event, switch or collectible flag records a purchase (both overlays grepped; only the TALKED flags of 5.4 are set) |
| Reload | every load re-runs `EnOssan_CheckValidSpawn` and respawns all 8 items from `sShops` (:265-283, :1575); rupees and inventory persist in `gSaveContext` |

### 5.4 Transaction flow, cancel and repeat (OVL `z_en_ossan.c`)

| Step | Function (lines) | Notes |
| --- | --- | --- |
| Talk offer | `EnOssan_Idle` (:365-390) | only while `xzDistToPlayer < 100` and the player stands in x -40..40, z -91..-60 (:381-384), i.e. 14-45 u in front of the counter wall at z -105 |
| Greeting | `BeginInteraction` (:392-459), `GetWelcome` (:1403-1472), `SetTalked` (:1474-1503) | a worn mask with a face reaction (`FACE_REACTION_SET_CURIOSITY_SHOP_MAN` 0x2F / `_PART_TIMER` 0x36, `include/z64message.h:162-165`) sets `END_INTERACTION`: greeting only. Human: 0x06A4 / 0x06C1. Goron, Zora, Deku: first-time vs repeat text by `WEEKEVENTREG_TALKED_CURIOSITY_SHOP_MAN_AS_*` (18 0x04/0x08/0x10) and `_PART_TIMER_AS_*` (55 0x04/0x08/0x10) (`include/z64save.h:842-844`, :1157-1159) |
| Form gate | `EnOssan_Hello` (:565-597) | Deku form: interaction ends after the greeting (:578-580); part-timer plays an apology loop for Zora (:589-592) |
| Prompt | `StartShopping` (:351-357, text 0x640), `FaceShopkeeper` (:627-675) | stick left -> left shelf (slots 4-7), right -> right shelf (0-3); choice 0 = talk text 0x06AB / 0x06C8, choice 1 = leave (:599-625) |
| Browse | `LookToLeftShelf` / `LookToRightShelf` (:689-743), `BrowseLeftShelf` / `BrowseRightShelf` (:853-967), `CursorUpDown` (:745-828) | cursor skips NULL slots (:505-563) |
| Select / confirm | `SelectItem` (:1072-1094), `HandleCanBuyItem` (:1015-1070) | result texts: success 0x06BF / 0x06DC; no room 0x06BE / 0x06DB (also used for `CANNOT_GET_NOW`); need bottle 0x06BC / 0x06D9; need rupees 0x06BD / 0x06DA; cannot get now (`_2`) 0x06C0 / 0x06DD (:176-184) |
| Cancel | `TestCancelOption` (:333-340), choice 1 (:1085-1088), `TestEndInteraction` (:325-331), `EndInteraction` (:300-323) | B at the confirm returns to browsing; B at the prompt ends the interaction |
| Repeat | `ItemPurchased` (:1183-1205, text 0x642), `ContinueShopping` (:1141-1181) | after a fanfare purchase: choice 0 restarts shopping (player turned 180), choice 1 ends; the same slot can be bought again after restock |
| UI | `DrawCursor`, `DrawStickDirectionPrompts` (:1609-1720) | screen-space cursor and stick arrows; colors set in `InitShop` (:1528-1564) |

### 5.5 Other venue mechanics

| Item | Rule | Citation |
| --- | --- | --- |
| Scarecrow song teach / dance | player pinned to (-50, 155) in this scene during song teach and dance; dig-away spawns water splashes here and sets `WEEKEVENTREG_79_08` | `z_en_kakasi.c:256-268`, :1028-1031, :1041 |
| Water current | surface 6 (channel floor) has conveyor speed 1, direction 48 -> binang 0xC000 (-X), no floor-conveyor bit; non-floor conveyors act only when `depthInWater > 20`; the channel is 9 deep, so the player is never carried | C `include/z64bgcheck.h:265`; `z_player.c:11736-11747` |
| Swimming | Human swims only when `depthInWater > 32.0` (`unk_2C`); max depth here is 9 | `z_player.c:1127-1153` (`:1151`), :8973 |
| Ledges | surface 1 (`WALL_TYPE_1` -> `WALL_FLAG_0`) on the 60-high right-hand box blocks the climb-up ledge types | `src/code/z_bgcheck.c:18-26`, :4206-4208; `z_player.c:11835` |

### 5.6 Cameras (HDR scene.c:65-82, :116-124, :145-148; `include/z64cutscene.h:626-637`)

| Cutscene id | Cam (csCam) | Setting | Pos / rot (raw) | User |
| --- | --- | --- | --- | --- |
| 9 | 0 | 0x1F `CAM_SET_FIXED1` (`z64camera.h:178`) | (0, 70, -80) / (0, -32768, 0) | En_Ossan look to shopkeeper (actor csId) |
| 10 | 2 | FIXED1 | (-35, 70, -80) / (0, -32768, 0) | look to left shelf (additional of 9) |
| 11 | 3 | FIXED1 | (35, 70, -80) / (0, -32768, 0) | look to right shelf |
| 12 | 1 | FIXED1 | (0, 70, -80) / (0, -32768, 0) | shopkeeper from shelf (`EnOssan_GetCutscenes`, :1584-1589) |
| 13 | 4 | FIXED1 | (-72, 47, 193) / (-1638, -22391, 0) | En_Test4 day/night transition |
| 14 -> 15 | -1 (global) -> 5 | FIXED1 for 5 | (-27, 143, 43) / (7464, -7282, 0); entry 15 length 60, end SFX `TRE_BOX_APPEAR` | En_Kakasi (csId 14, chain read at `z_en_kakasi.c:186-194`) |
| - | 6 | 0x2C `CAM_SET_START0` | (-30, 102, 209) / (4005, 30948, 0) | spawn 0 arrival camera (section 2.1) |
| 0 -> 8 | -2..-11 (global) | - | - | chain 0-8: no consumer found among the room actors (U-03) |

Room bgCams: 0 = 0x3F `CAM_SET_ROOM0` (every surface except 2; both water boxes also name bgCam 0), 1 = 0x4A `CAM_SET_NORMAL2` on surface 2, the shop platform floor polys 22/23 (`z64camera.h:210`, :221). Proposed capture poses for the registered cameras (proposals only, no capture): `cam.trading_post.entry` = cam info 6; `cam.trading_post.fixture` = cutscene 9 pose; `cam.trading_post.reverse_door` has no source pose (INF: from the bridge toward +Z).

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header)

Open at this profile (section 5.1). Assumptions: fresh save (no week-event, switch or collectible flags), Human form, no mask worn, Ocarina possession per SOURCE_REGISTER U-07 (clock rate only). Positions N64 (Unity).

| # | Component / params | Position (Unity) | Yaw | Scale | Display lists, bank | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room | room_00 geometry | origin | - | 1 | opaque `DL_00C040` (9 DLs) and XLU `DL_00C9F0` (7 DLs incl. water `DL_00C150`, `DL_00C2E8`) | XML-declared; water scroll on segments 8/9 |
| ta0 | door, 0x0283 | (0, 15, 225) (0.00, 0.15, -2.25) | 0 | skeleton default | `gDoorSkel` (gameplay_keep) + `gPostOfficeTradingPostDoorDL` object_wdor02 0x548 | closed rest pose; side DL by camera yaw |
| a11 | wall clock, 0xA07F | (-98, 143, -206) (-0.98, 1.43, 2.06) | 0 | 0.01 | object_obj_tokeidai 0xCF28, 0xBEE8, 0xF518, 0xC368 | noon pose rules as ROM_REFERENCE tower clock |
| a13 | cow statue head, 0x0000 | (34, 35, -157) (0.34, 0.35, 1.57) | 333 | 0.1 | object_hariko 0x80, 0x110 | rest pose (no quake) |
| a08 | pot, 0x030A | (105, 43, 105) (1.05, 0.43, -1.05) | 0 | 0.197 | object_tsubo `gPotDL` 0x17C0 | on the Y 43 box |
| a02 | bush, 0xFF0B | (-190, 0, -50) (-1.90, 0.00, 0.50) | 0 | 1.0 | object_wood02 `DL_000090` (XLU) | |
| a03 | bush, 0xFF0B | (-190, 0, -120) (-1.90, 0.00, 1.20) | 0 | 1.0 | same | |
| a04 | bush, 0xFF0B | (177, 0, 89) (1.77, 0.00, -0.89) | 0 | 1.0 | same | |
| a05 | bush, 0xFF0B | (136, 0, 183) (1.36, 0.00, -1.83) | 0 | 1.0 | same | |
| a01/items0-7 | 8 En_GirlA from `sShops[0]` | section 5.2 positions; e.g. slot 0 (0.50, 0.68, 1.95), slot 7 (-0.80, 0.92, 1.95) | 0 | 0.25, yOffset 24 | `GetItem_Draw` models from gi_nuts, gi_stick, gi_arrow (x2), gi_bottle_04, gi_liquid (x2), gi_shield_2 | spawned at runtime; GID model DLs NI (U-11) |

**Excluded by state at Day 1 noon:** a07 part-time worker (`EnOssan_CheckValidSpawn`). **Excluded by mask:** none.

**Unimplemented in a static assembly (present at noon, needs skeleton/pose or motion):** a01 Curiosity Shop Man (`gFsnSkel`, scratch-back idle, facing yaw 180 toward the shelves), a12 scarecrow (`object_ka_Skel_0065B0`), a06 fish (`gameplay_keep_Skel_02F028`, swimming), the water texture scroll, door skeleton pose.

**No geometry:** a00 En_Test4, a09 En_Time_Tag, a10 En_Talk, a14 Obj_Sound (positional BGM `NA_BGM_SHOP`).

## 7. Collision summary (C from scene.c:145-620; scratch `parse_out.txt`, `probe_out.txt`, `dump_out.txt`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 205 (199 used) / 241 / 7 |
| Kinds | 76 floors (65 under 10 deg), 161 walls (all vertical), 4 ceilings (Y 115 door frame, Y 220 room) |
| Header bounds | min (-310, -41, -385), max (310, 280, 360) |
| Used-vertex extents | x -210..210, y -40..220, z -210..360 (Unity x -2.10..2.10, y -0.40..2.20, z -3.60..2.10); the 6 unused vertices are the corners of the two water boxes |
| Materials | wood (surfaces 0, 1, 2, 4): 177 polys; dirt (5, 6): 58; stone (3): 6 (outer walls z 210 and x +-210) |
| Surface specials | 1: wall type 1 (no-climb) on the right box; 2: bgCam 1 `NORMAL2` (shop platform floor); 4: exit 0; 6: water current (section 5.5). Echo 2 on all; light setting 31 on all; no hookshot, floor type, floor property or soft bits |
| Poly exclusion | 30 polys with xpFlag 1 (ignore camera): exit pad 35/36, door frame 43-50, two posts 107-126 |
| Water boxes | 2, properties 0x00001F00 (bgCam 0, light index 0x1F none, room 0, enabled; `include/z64bgcheck.h:34-46`): box 1 x -240..240, z -385..95, surface Y -6; box 2 x -158..-90, z 128..204, surface Y -6 (scene.c:607-610) |
| Camera data | 2 `BgCamInfo` (section 5.6) |

Floor levels (centroid Y; INF meaning from shape and actor positions):

| Level | Polys | x | Y | z | Surface | Meaning (INF) |
| --- | --- | --- | --- | --- | --- | --- |
| Channel bed | 93, 94 | -210..210 | -15 | 0..45 | 6 | water channel across the room (box 1); fish a06 |
| Channel banks | 91, 92, 95, 96 | -210..210 | -15..0 | -15..0, 45..60 | 5 | 45-degree banks |
| Pool bed | 223, 224 | -158..-90 | -13 | 128..204 | 5 | scarecrow pool (box 2), walls 225-232 |
| Main floor | 37, 38, 71, 72, 89, 90, 97, 98, 105, 106, 221, 222, 233-238 | -210..210 | 0 | -210..210 | 5 | sunken floor; bushes a02-a05 |
| Mound | 30-32 | -142..-112 | 0..11 | 68..98 | 0 | small three-faced mound |
| Entry walkway | 99, 100 | -30..30 | 15 | 90..210 | 0 | from the door, 15 above the floor (walls 101-104) |
| Door frame / exit pad | 43, 44 / 35, 36 | -30..30 / -60..60 | 15 | 210..240 / 240..360 | 0 / 4 | section 2.4 |
| Bridge | 137-144 | -30..30 | 15 rising to 30, down to 25 | -15..90 | 0 | crossing over the channel (side walls 127-156) |
| Shop platform | 22, 23, 145, 146 | -150..150 | 25 | -210..-15 | 2, 0 | counter level; posts 107-126 (Y 0..60) flank the bridge landing |
| Left ledges | 58-60 / 51, 52 | -210..-177 | 20 / 26 | 101..180 | 5 | two-step ledge on the left wall |
| Display box / low steps | 157-164 / 187-191, 216-220 | 60..150 / 45..150 | 43 / 18 | 62..148 / 131..180 | 0 | pot a08 on top |
| Right box | 239, 240 | 150..210 | 60 | -180..-45 | 5 | no-climb walls 6-9; talk spot a10 above it |

Counter (C from normals, INF for the reading): a full-height wall (Y 25-220) at z -105 for x -105..105 faces the player (polys 16/17), with side faces at x -105 (18/19) and x 105 (14/15) and further walls at x 79-82 / z -135..-180 (10-13), z -180 (69/70) and z -130..-157 on the left (0-5). The shopkeeper zone and both shelves (z about -105 to -210) are out of reach on foot; the platform floor runs to z -210 with no wall across the back (the back wall is room DL only, NI). The player's talk box (z -91..-60) is on the platform floor, 14-45 u in front of that wall.

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 33: 31 textures + 2 TLUTs; all referenced by the room file; the room file holds none |
| By format | ci8 22, i8 3, ia16 2, rgba16 2 textures + 2 TLUTs, rgba32 1, i4 1 |
| Names | ci8 `Tex_0027F0` `Tex_002830` `Tex_002E30` `Tex_003030` `Tex_003930` `Tex_004130` `Tex_004930` `Tex_004A30` `Tex_004E30` `Tex_005230` `Tex_005430` `Tex_005530` `Tex_006930` `Tex_0069B0` `Tex_007DB0` `Tex_008230` `Tex_008270` `Tex_008670` `Tex_008C70` `Tex_009470` `Tex_009870` `Tex_009C70`; i8 `Tex_001770` `Tex_0027B0` `Tex_005930`; i4 `Tex_003130`; ia16 `Tex_002C30` `Tex_0081B0`; rgba16 `Tex_006DB0` `Tex_008A70`, TLUTs `TLUT_009E70` `TLUT_00A070`; rgba32 `Tex_0017B0` (all prefixed `Z2_8ITEMSHOP`) |
| Room `Gfx` / `Vtx` arrays | 18 / 14 (10 `.vtx.inc` files) |
| Room shape | cullable, 12 entries (room.c:102-115): 9 with an opaque DL, 7 with an XLU DL (4 entries have both) |
| Raw triangle commands | 558 `gsSP2Triangles` + 24 `gsSP1Triangle` = 1140 triangles (DL reuse not resolved) |
| Render modes | `G_RM_AA_ZB_OPA_SURF2` 5, `G_RM_AA_ZB_TEX_EDGE2` 5, `G_RM_AA_ZB_XLU_SURF2` 3, `G_RM_ZB_XLU_SURF2` 1, `G_RM_FOG_SHADE_A` 14 |
| Animated materials | 2 two-texture scrolls: segment 8 used once, by `DL_00C150` (cullable entry 7, centre (0, -6, 22), r 244: the channel water); segment 9 once, by `DL_00C2E8` (entry 8, centre (-124, -6, 166), r 52: the pool water) (room.c:1603, :1628) |
| Display goods | the 8 shop items are actors (5.2); any other shelf goods, signs and counter dressing are room geometry, unidentified until a render (U-10) |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_8ITEMSHOP` (C above). Already extracted: `object_obj_tokeidai`. Binaries and XMLs exist for every bank below (checked by `stat`).

| Priority | Banks / data | Why |
| --- | --- | --- |
| Day 1 static | `object_wood02`, `object_tsubo`, `object_hariko`, `object_wdor02`, `gameplay_keep` | section 6 DLs, door skeleton |
| Shop goods | `object_gi_nuts`, `object_gi_stick`, `object_gi_arrow`, `object_gi_liquid`, `object_gi_bottle_04`, `object_gi_shield_2` | GetItem models of the 8 items |
| Later skeletal pass | `object_fsn` (XML note: `gFsnSkel` declares 18 limbs, 17 exist; `object_fsn.xml:103`), `object_ani`, `object_ka` | shopkeepers, scarecrow; fish uses `gameplay_keep` |
| Prices (U-01) | message text, into an ignored scratch folder | `firstChoicePrice` of the 16 choice messages in 5.2 |

```powershell
# run from local-reference/mm-decomp; same ZAPD binary and flags as tools/reference/README.md
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_wood02','object_tsubo','object_hariko','object_wdor02','gameplay_keep','object_gi_nuts','object_gi_stick','object_gi_arrow','object_gi_liquid','object_gi_bottle_04','object_gi_shield_2','object_fsn','object_ani','object_ka')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
# prices: decomp text extractor, output kept out of the repo (no -I: the tool imports tools/version and tools/text)
python tools/extract_text.py extracted/n64-us/baserom <scratch>/text -v n64-us
```

`gameplay_keep` has not been extracted with this tool before (North section 8); check its size and warnings separately. The text output is ROM-derived: transcribe only the price numbers. Argument shape read from `tools/extract_text.py:7-35`; the tool itself was not run.

## 10. Proposed region record

**Proposal in the proposal-0 vocabulary that `tools/reference/region_from_dossier.py` lifts; checked against frozen schema 1 (section 11).**

```json
{
  "_status": "proposal-0 block; region_from_dossier.py lifts it into schema 1 (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.trading_post",
  "displayLabel": "Trading Post",
  "source": {
    "sceneSymbol": "Z2_8ITEMSHOP",
    "sceneEnum": "SCENE_8ITEMSHOP",
    "sceneTableIndex": "0x34",
    "entranceTableSceneIndex": "0x31",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0)",
    "rooms": { "loc.trading_post.room_00": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event, switch or collectible flags", "player form Human, no mask worn", "venue open: door schedule none at 12:00, Curiosity Shop Man present, day stock sShops[0]"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_8ITEMSHOP", "bytes": 41584, "sha256": "5e1fce4d894017b2c1136575485b5b7c07695de0d5e3c1724035c9a933bed912" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_8ITEMSHOP_room_00", "bytes": 51760, "sha256": "8a0372ba6771df1b3c7228f54f04d14af669e96dfb74dd6589b92c9aeaa42de8" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_8ITEMSHOP/Z2_8ITEMSHOP.c", "bytes": 34580, "sha256": "c6e1547829f682046881015673885041212f518b6b29c08ea8630e6244870007" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_8ITEMSHOP/Z2_8ITEMSHOP_room_00.c", "bytes": 95701, "sha256": "ba3735bac600de0e3e786c6954e595559d9a542b952b137a445b49200f3a2b72" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_8ITEMSHOP/Z2_8ITEMSHOP.xml", "bytes": 410, "sha256": "b5fe3dae2d553bf3d9333cd28263b5da7c3e6924cfd1ac11eb71b9ae863d900b" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedFrame": { "with": [], "status": "not_shared", "evidence": "interior local frame: door TA#0 at (0, 15, 225) vs the West street door at (-1796, 105, -204); no shared geometry" } },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-310, -41, -385], "max": [310, 280, 360] },
    "sourceCollisionVertices": { "min": [-210, -40, -210], "max": [210, 220, 360] },
    "unityCollisionVertices": { "min": [-2.10, -0.40, -3.60], "max": [2.10, 2.20, 2.10] }
  },
  "water": [
    {
      "id": "water.trading_post.channel",
      "sourceWaterBox": { "xMin": -240, "xMax": 240, "zMin": -385, "zMax": 95, "surfaceY": -6, "properties": "0x00001F00", "bgCamIndex": 0, "lightIndex": 31, "room": 0, "disabled": false },
      "unity": { "x": [-2.40, 2.40], "z": [-0.95, 3.85], "surfaceY": -0.06 },
      "poolFloorY": [-15, -15],
      "bankFloorY": [0, 0],
      "surfaceMesh": "room DL_00C150 at Y -6, two-texture scroll on segment 8",
      "behavior": "depth at most 9: wading only (Human swims above 32); surface 6 current inactive below depth 20"
    },
    {
      "id": "water.trading_post.scarecrow_pool",
      "sourceWaterBox": { "xMin": -158, "xMax": -90, "zMin": 128, "zMax": 204, "surfaceY": -6, "properties": "0x00001F00", "bgCamIndex": 0, "lightIndex": 31, "room": 0, "disabled": false },
      "unity": { "x": [-1.58, -0.90], "z": [-2.04, -1.28], "surfaceY": -0.06 },
      "poolFloorY": [-13, -13],
      "bankFloorY": [0, 0],
      "surfaceMesh": "room DL_00C2E8 at Y -6, two-texture scroll on segment 9",
      "behavior": "depth 7: wading only; scarecrow a12 stands in it"
    }
  ],
  "anchors": [
    { "id": "anchor.tp.spawn0_entry", "source": [-1, 15, 189], "unity": [-0.01, 0.15, -1.89], "status": "proposed, not surveyed" },
    { "id": "anchor.tp.door_ta0", "source": [0, 15, 225], "unity": [0.00, 0.15, -2.25], "status": "proposed, not surveyed" },
    { "id": "anchor.tp.counter_talk_center", "source": [0, 25, -75], "unity": [0.00, 0.25, 0.75], "status": "proposed, not surveyed" },
    { "id": "anchor.tp.wall_clock", "source": [-98, 143, -206], "unity": [-0.98, 1.43, 2.06], "status": "proposed, not surveyed" }
  ],
  "rooms": [ { "id": "loc.trading_post.room_00", "sourceRoom": "Z2_8ITEMSHOP_room_00", "roomShape": "cullable, 12 entries", "displayLists": 18 } ],
  "components": [
    { "sourceKey": "Z2_8ITEMSHOP/h0/ta0", "actorId": "0x005", "params": "0x0283", "class": "door/transition (posed skeleton)", "dls": ["gameplay_keep:gDoorSkel", "object_wdor02:0x548"], "status": "selected, rest pose approximated" },
    { "sourceKey": "Z2_8ITEMSHOP/h0/room_00/a11", "actorId": "0x19C", "params": "0xA07F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected, noon pose from clock time" },
    { "sourceKey": "Z2_8ITEMSHOP/h0/room_00/a13", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80", "object_hariko:0x110"], "status": "selected" },
    { "sourceKey": "Z2_8ITEMSHOP/h0/room_00/a08", "actorId": "0x082", "params": "0x030A", "class": "static-mesh", "dls": ["object_tsubo:0x17C0"], "status": "selected" },
    { "sourceKey": "Z2_8ITEMSHOP/h0/room_00/a02", "actorId": "0x041", "params": "0xFF0B", "class": "static-mesh", "dls": ["object_wood02:0x90"], "status": "selected" },
    { "sourceKey": "Z2_8ITEMSHOP/h0/room_00/a03", "actorId": "0x041", "params": "0xFF0B", "class": "static-mesh", "dls": ["object_wood02:0x90"], "status": "selected" },
    { "sourceKey": "Z2_8ITEMSHOP/h0/room_00/a04", "actorId": "0x041", "params": "0xFF0B", "class": "static-mesh", "dls": ["object_wood02:0x90"], "status": "selected" },
    { "sourceKey": "Z2_8ITEMSHOP/h0/room_00/a05", "actorId": "0x041", "params": "0xFF0B", "class": "static-mesh", "dls": ["object_wood02:0x90"], "status": "selected" },
    { "sourceKey": "Z2_8ITEMSHOP/h0/room_00/a01/items0-7", "actorId": "0x002", "params": "0x0009,0x0007,0x0004,0x0008,0x0003,0x0005,0x0006,0x000A", "class": "static-mesh (spawned)", "dls": ["GID_DEKU_NUTS", "GID_DEKU_STICK", "GID_ARROWS_LARGE", "GID_ARROWS_MEDIUM", "GID_FAIRY", "GID_POTION_GREEN", "GID_SHIELD_HERO", "GID_POTION_RED"], "status": "selected, GetItem_Draw models spawned by a01 from sShops[0]" }
  ],
  "collisionOwnership": { "room": "Z2_8ITEMSHOPCollisionHeader_001738 (241 polys, 2 water boxes)", "waterBoxes": 2, "actorDynapoly": [], "actorCylinders": ["a08 Obj_Tsubo r12 h32"], "note": "schedule door loads no dynapoly (z_en_door.c:338-341); NPC and fish colliders NI" },
  "portalIds": [
    "portal.trading_post.west_clock_town",
    "portal.trading_post.west_clock_town.kickout"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a01 En_Ossan (gFsnSkel)", "a12 En_Kakasi (object_ka skeleton)", "a06 En_Fish (swimming skeleton)", "door skeleton animation", "water texture scroll", "Link and Tatl"],
  "excluded": { "halfDayMask": [], "stateOrSchedule": ["a07"], "noGeometry": ["a00", "a09", "a10", "a14"] },
  "unsupported": ["skeletal NPC poses", "shop transaction UI, cursor and cutscene cameras", "prices (message data)", "time-driven wall clock rotation", "positional BGM", "water texture scroll", "inactive water current"],
  "evidence": ["docs/city/interiors/trading_post.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Price of each of the 16 shop entries (day and night) | Run `tools/extract_text.py` (section 9) into scratch and read the `firstChoicePrice` field (`z_message.c:2073-2074` order) of the day choice messages 0x06B1, 0x06B5, 0x06BB, 0x06B9, 0x06B7, 0x06B3, 0x06AF, 0x06AD and night 0x06CE, 0x06D2, 0x06D8, 0x06D6, 0x06D4, 0x06D0, 0x06CC, 0x06CA (slots 0-7, section 5.2); record whether night prices differ |
| U-02 | What stops a walk through the closed doorway onto exit floors 35/36 (no dynapoly, no collider, no wall at z 225) | Read the player's door handling around `z_player.c:6839` and its wall/ACTORCAT_DOOR push code, or test with a controller route once a reference assembly exists |
| U-03 | Consumer of actor-cutscene entries 0-8 (global camera chain) | Which code queues csId 0 in this scene: grep `CutsceneManager_Queue`/`GetAdditionalCsId` callers that take a scene index (door transition, item get, player) |
| U-04 | Banks with no listed consumer: `object_kibako`, `object_cs`, `object_os_anime` | Grep `OBJECT_KIBAKO`, `OBJECT_CS`, `OBJECT_OS_ANIME` in overlays that can be spawned here (children, event actors) |
| U-05 | En_Talk a10 text 0x1C09 and what it labels (above the Y 60 box) | Message extraction (U-01) plus a render of room DLs near (177, 60, -92) |
| U-06 | Cycle-reset persistence of `WEEKEVENTREG_TALKED_*` (18, 55), `79_08`, `83_01` | Read the end-of-cycle week-event clearing in `src/code/z_sram_NES.c` |
| U-07 | Exact shop browsing camera framing (third Vec3s of cam infos 1-3 is `(-1, 75, -1)`) | Read `Camera_Fixed1` use of `BgCamFuncData` fov / `unk_0E` in `src/code/z_camera.c` |
| U-08 | Semantics of `ROOM_TYPE_3` and of each `RESTRICTIONS_SET` field | Grep the restriction table consumers in `z_parameter.c`; confirm no numeric `type == 3` test in `src/` beyond the grep done |
| U-09 | Start mode 0xE arrival behavior | Read the start-mode dispatch table in `z_player.c` for mode 0xE |
| U-10 | Counter, shelves, signs and non-actor display goods | A1 render of the room DLs (needs ROOM-01); capture `cam.trading_post.fixture` |
| U-11 | Model DLs behind each `GID_*` of the shop items | Read `src/code/z_draw.c` `sDrawItemTable` rows for the 8 GIDs |
| U-12 | Scarecrow dialogue branches (day 3, night) and the respawn path after the time skip | Read `z_en_kakasi.c:313-560` and `func_80169EFC` in `src/code/z_play.c` |
| U-13 | Whether the clock can pass 21:00 during shopping (kickout needs the exact minute and no cutscene mode) | List the text IDs exempt from the time stop in `z_kankyo.c:1202-1211` and compare with the shop texts |
| U-14 | Senders of entrance 1 (`0x6210`) | Search non-city scenes and cutscene destination data |

Refinements to other files (not edited): PORTAL_REGISTER rows `portal.west_clock_town.trading_post` and `portal.trading_post.west_clock_town` say "open hours UNKNOWN": source gives closed 21:00-21:59 from the street (text 0x180B), interior always openable (5.1). West 2.4.1 "message only 21:00-22:00" agrees; the exact end is 21:59. LOCATION_REGISTER U-02: the trailing `0x0000` here is padding (2.2). SOURCE_REGISTER U-10: open at Day 1 12:00. No count disagrees: 17 objects, 15 actors, 1 spawn, 1 transition actor, 2 exit words, 2 entrances all match LOCATION_REGISTER.

**Checks run:** read the brief, `docs/HANDOFF.md`, FULL_CITY_PLAN sections 3, 4, 6, 11, 12, the three registers, the North template, East 2.4, West 2.4.1, `shared/city/schema/README.md`, `region.schema.json`, `tools/reference/region_from_dossier.py`, `validate_city.py`; scene/room C and XML; scene, actor, object and sequence tables; `z_scene_table.c`; `z_actor.c`, `z_scene.c`, `z_play.c`, `z_camera.c`, `z_bgcheck.c`, `z_schedule.c`, `z_parameter.c`, `z_message.c`, `z_en_item00.c`, `z_scene_proc.c` at the lines cited; overlays En_Ossan, En_GirlA, En_Door (+ `scheduleScripts.schl`), En_Time_Tag, En_Kakasi, En_Fish, En_Wood02, Obj_Tsubo, Obj_Tokeidai, Obj_Hariko, Obj_Sound, En_Talk, En_Test4 (init), player (door, exit, conveyor, swim, ledge lines). Scratch scripts in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\trading_post\` (`parse.py`, `probe.py`, `dump.py`, `bounds.py`, `region_check.py`; outputs `*_out.txt`): actor/spawn decode, collision counts, floor probes, polygon dump, used-vertex bounds; SHA-256 of the five inputs; grep cross-checks (entrance users, `ROOM_TYPE_3`, kickout-wait setters, stock flags). The section 10 block was lifted with `region_from_dossier.extract_block` + `normalize` into scratch only (`region_check.py`, bytecode writing off) and `python -I tools/reference/validate_city.py <scratch>/lifted/trading_post.json` printed `OK`, `1 record(s), 0 error(s)`. A first run without bytecode suppression left `tools/reference/__pycache__/region_from_dossier.cpython-314.pyc`; that file and folder were deleted, so the tree is as found.

**NOT run:** ZAPD (no object extracted), text extraction, Blender, Unity, any player or executable, captures, renders, controller routes; `region_from_dossier.py` in its writing mode; `validate_city.py --selftest` (it writes into `shared/city/regions/`).

**Next dependency:** U-01 prices before SYS-02; section 9 extraction and ROOM-01 reference assembly; integrator lifts section 10 into `shared/city/regions/trading_post.json`.
