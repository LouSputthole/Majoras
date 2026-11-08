# Clock Town portal register (REF-02)

Task REF-02. Base commit `d0bc29f3ccf38e53ed7f92559cb2d62c3ebd1e1b` on `main`. Written 2026-11-08 from the local pinned zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639`; source paths below are relative to `local-reference/mm-decomp/` unless stated. Scene data is the ZAPD-extracted C in `extracted/n64-us/assets/scenes/<SCENE>/<SCENE>.c` and `<SCENE>_room_NN.c`.

This register maps source endpoints only. Nothing was walked, loaded or launched: every row's traversal status is `not_run`. It covers the default scene header of all 22 city scenes; alternate headers are in section 5. Location IDs are the fixed REF-01 set; `scene.south_clock_town` remains a valid alias of `loc.south_clock_town`.

| Evidence class | Meaning in this document |
| --- | --- |
| present in header | Read directly from the extracted scene or room C (symbol plus index or line) |
| decoded from C | Header values run through cited decomp code (entrance tables, player exit code, actor param macros) |
| inferred | An association this register makes: door-to-polygon proximity, reverse pairing, `upper`/`lower` qualifiers, mode classification; distances in N64 units (u) are given |
| not inspected | Not read in this pass; listed in sections 7 and 8 |

## 1. Entrance encoding (decoded from C)

**Layout.** `include/z64scene.h:706-711` documents the u16 and defines `ENTRANCE(scene, spawn)` (`:711`); `CS_ENTRANCE` adds a layer (`:722`); `Entrance_Create` builds `(scene << 9) | (spawn << 4) | layer` (`src/code/z_scene.c:603-606`).

| Bits | Mask | Meaning |
| --- | --- | --- |
| 15-9 | `0xFE00` | Index into `sSceneEntranceTable` (`src/code/z_scene_table.c:2572`): the ENTRANCE scene index (`EntranceSceneId`, `z64scene.h:592-704`), not the scene-table enum |
| 8-4 | `0x01F0` | Slot in that scene's `s<Name>EntranceTable[]`, an array of `EntranceTableEntry*` |
| 3-0 | `0x000F` | Index into the selected `s<Name>Entrance<N>[]` ("layer") |

**Lookup.** `Entrance_GetTableEntry` (`z_scene_table.c:2688-2694`) returns `&sSceneEntranceTable[e >> 9].table[(e >> 4) & 0x1F][e & 0xF]`, an `EntranceTableEntry { s8 sceneId; s8 spawnNum; u16 flags; }` (`z64scene.h:397-401`). `sceneId` is a `SCENE_*` scene-table enum (`include/tables/scene_table.h`); some are negative and loading uses the absolute value (`z_scene_table.c:2708-2712`); negative IDs also alter the final-hours music test (`src/code/z_play.c:592`, `:605`).

- The slot is not defined as the spawn. Checked: all 117 `EntranceTableEntry` rows in the 22 city scenes' tables have `spawnNum == slot` and `sceneId ==` the scene's own enum, so for these scenes slot equals destination spawn (decoded from C).
- Layer: at load the engine adds `gSaveContext.sceneLayer` before lookup (`z_play.c:2224-2226`). It is 0 unless `cutsceneIndex >= 0xFFF0` (`z_play.c:2210-2219`); nonzero selects alternate header `sceneLayer - 1` (`z_scene.c:465-474`). Every exit value in the 22 default headers has layer bits 0.
- `flags`: bits 0-6 exit fade (`z_scene.c:540`), bits 7-13 arrival transition (`z_play.c:2275`), bit 15 feeds the BGM-mute test (`z_play.c:583`). Transition-type names: not inspected.

**Spawn to pose.** `spawnNum` becomes `curSpawn`; `setupEntranceList[curSpawn]` is `EntranceEntry { u8 spawn; u8 room; }` (`z64scene.h:392-395`): `.spawn` selects the start-position entry (`z_scene.c:159-160`), `.room` the first room (`src/code/z_room.c:549`). The player is spawned by `Actor_SpawnEntry` (`src/code/z_actor.c:2535`), so yaw is `(rot.y >> 7) & 0x1FF` degrees (`z_actor.c:3629-3634`); start mode is `(params & 0xF00) >> 8` and bgCam `params & 0xFF` (`include/z64player.h:17-18`). Poses are N64 world units, Y up. Unity's source mapping is `(x, y, -z) / 100`.

**Triggers that read the exit list** (`play->setupExitList`, set only by `Scene_CommandExitList`, `z_scene.c:440-442`; reset to NULL in `Play_InitScene`, `z_play.c:1569-1574`):

- Collision: surface word 0 bits 8-12 hold `exitIndexPlusOne` (`include/z64bgcheck.h:79-87`, `:277`; `SurfaceType_GetSceneExitIndex`, `src/code/z_bgcheck.c:4183-4185`). A polygon's surface is `surfaceTypes[poly->type]` (`z_bgcheck.c:4038`). The player reads it (`src/overlays/actors/ovl_player_actor/z_player.c:6416`) and calls `func_808354A4(play, exitIndexPlusOne - 1, ...)` (`:6441`), which sets `nextEntrance = setupExitList[exitIndex]` (`:6372`); `0xFFFF` is respawn and `>= 0xFE00` a return group (`:6374-6384`).
- Doors: `EN_DOOR` params hold transition index, type and type var only (`ovl_En_Door/z_en_door.h:9-20`, `:95-96`); the scene change is the exit polygon behind the door. Every city scene-exit door lists the same room on both sides. Framed doors read an exit from `rot.x/rot.z` (`z_player.c:15895-15900`); no city door is framed.
- Actors: kickout `EN_TIME_TAG` types 0/4/5, `params & 0x1F`, fires at `home.rot.x:home.rot.y` (`ovl_En_Time_Tag/z_en_time_tag.h:10-15`, `.c:287`, `:315-338`); scene-exit grotto `DOOR_ANA` type `0x300`, `params & 0x1F` (`ovl_Door_Ana/z_door_ana.h:20-25`, `.c:136-138`); `EN_WARP_TAG` `params & 0x3F` (`ovl_En_Warp_tag/z_en_warp_tag.h:27`, `.c:241`); `EN_BOMJIMB` `(params >> 8) & 0xFF` (`ovl_En_Bomjimb/z_en_bomjimb.c:162`, `:861`); `EN_HOLL` only for type 4 (`ovl_En_Holl/z_en_holl.h:64-65`, `.c:190-191`), and all city EnHoll are type 0. Room actors also carry a spawn half-day mask `((rot.x & 7) << 7) | (rot.z & 0x7F)` (room spawn filter `z_actor.c:2562-2570`, 0 means all; bit names `include/z64actor.h:328-337`).

**Worked example, South to North `0xD620`** (matches the playbook row):

| Step | Value | Source |
| --- | --- | --- |
| Trigger | Polygons 18, 19 have `type 0x0002`; `SurfaceType[2] = {0x00000500, ...}`; `(0x500 >> 8) & 0x1F = 5` | `Z2_CLOCKTOWER.c:1359-1360`, `:1332` |
| Exit index | `5 - 1 = 4`; `Z2_CLOCKTOWERExitList_000184[4] = 0xD620` | `Z2_CLOCKTOWER.c:94` |
| Scene field | `0xD620 >> 9 = 0x6B` -> `sNorthClockTownEntranceTable`, "Z2_BACKTOWN" | `z_scene_table.c:2680` |
| Slot, layer | `(0xD620 >> 4) & 0x1F = 2`, `0xD620 & 0xF = 0` -> `sNorthClockTownEntrance2[0] = { SCENE_BACKTOWN, 2, 0xC102 }` | `z_scene_table.c:2315-2317` |
| Enum | `SCENE_BACKTOWN = 0x6E`, not `0x6B` | `scene_table.h:225` |
| Spawn 2 | `Z2_BACKTOWNEntranceList0x00011C[2] = {0x02, 0x00}`: start 2, room 0 | `Z2_BACKTOWN.c:64` |
| Pose | `(-265, 189, -1656)`, yaw `0xB4` = 180, params `0x0F0A` (start mode 0xF, bgCam 0x0A) | `Z2_BACKTOWN.c:43-44` |
| Flags | `0xC102`: exit fade `0x02`, arrival `(0xC102 >> 7) & 0x7F = 0x02`, bit 15 set | as above |

## 2. Entrance-table index per scene (present in header + decoded from C)

All 22 city scenes plus every other scene they exit to. "Table" is the `sSceneEntranceTable` row in `z_scene_table.c`.

| Entrance index | Scene symbol | Table (line) | Scene-table enum (line) | Register ID |
| --- | --- | --- | --- | --- |
| `0x00` | `Z2_SONCHONOIE` | `sMayorsResidenceEntranceTable` L2573 | `SCENE_SONCHONOIE` = `0x12` (scene_table.h:43) | `loc.mayors_residence` |
| `0x04` | `Z2_BOWLING` | `sHoneyAndDarlingsShopEntranceTable` L2577 | `SCENE_BOWLING` = `0x11` (scene_table.h:41) | `loc.honey_darling` |
| `0x07` | `Z2_AYASHIISHOP` | `sCuriosityShopEntranceTable` L2580 | `SCENE_AYASHIISHOP` = `0x0D` (scene_table.h:35) | `loc.curiosity_shop` |
| `0x10` | `Z2_IKANA` | `sIkanaCanyonEntranceTable` L2589 | `SCENE_IKANA` = `0x13` (scene_table.h:45) | `ext.z2_ikana` |
| `0x12` | `Z2_MILK_BAR` | `sMilkBarEntranceTable` L2591 | `SCENE_MILK_BAR` = `0x15` (scene_table.h:49) | `loc.milk_bar` |
| `0x14` | `Z2_TAKARAYA` | `sTreasureChestShopEntranceTable` L2593 | `SCENE_TAKARAYA` = `0x17` (scene_table.h:53) | `loc.treasure_chest_shop` |
| `0x16` | `Z2_OKUJOU` | `sClockTowerRooftopEntranceTable` L2595 | `SCENE_OKUJOU` = `0x19` (scene_table.h:57) | `loc.clock_tower_rooftop` |
| `0x17` | `Z2_OPENINGDAN` | `sOpeningDungeonEntranceTable` L2596 | `SCENE_OPENINGDAN` = `0x1A` (scene_table.h:59) | `ext.z2_openingdan` |
| `0x1B` | `Z2_DEKUTES` | `sDekuScrubPlaygroundEntranceTable` L2600 | `SCENE_DEKUTES` = `0x1E` (scene_table.h:67) | `loc.deku_playground` |
| `0x1D` | `Z2_SYATEKI_MIZU` | `sTownShootingGalleryEntranceTable` L2602 | `SCENE_SYATEKI_MIZU` = `0x20` (scene_table.h:71) | `loc.town_shooting_gallery` |
| `0x23` | `Z2_YOUSEI_IZUMI` | `sFairyFountainEntranceTable` L2608 | `SCENE_YOUSEI_IZUMI` = `0x26` (scene_table.h:83) | `loc.great_fairy_fountain_town` |
| `0x26` | `Z2_TENMON_DAI` | `sAstralObservatoryEntranceTable` L2611 | `SCENE_TENMON_DAI` = `0x29` (scene_table.h:89) | `loc.astral_observatory` |
| `0x2A` | `Z2_00KEIKOKU` | `sTerminaFieldEntranceTable` L2615 | `SCENE_00KEIKOKU` = `0x2D` (scene_table.h:97) | `ext.z2_00keikoku` |
| `0x2B` | `Z2_POSTHOUSE` | `sPostOfficeEntranceTable` L2616 | `SCENE_POSTHOUSE` = `0x2E` (scene_table.h:99) | `loc.post_office` |
| `0x31` | `Z2_8ITEMSHOP` | `sTradingPostEntranceTable` L2622 | `SCENE_8ITEMSHOP` = `0x34` (scene_table.h:110) | `loc.trading_post` |
| `0x35` | `Z2_31MISAKI` | `sZoraCapeEntranceTable` L2626 | `SCENE_31MISAKI` = `0x38` (scene_table.h:118) | `ext.z2_31misaki` |
| `0x36` | `Z2_TAKARAKUJI` | `sLotteryShopEntranceTable` L2627 | `SCENE_TAKARAKUJI` = `0x39` (scene_table.h:120) | `loc.lottery_shop` |
| `0x43` | `Z2_21MITURINMAE` | `sWoodfallEntranceTable` L2640 | `SCENE_21MITURINMAE` = `0x46` (scene_table.h:145) | `ext.z2_21miturinmae` |
| `0x51` | `Z2_DOUJOU` | `sSwordmansSchoolEntranceTable` L2654 | `SCENE_DOUJOU` = `0x54` (scene_table.h:173) | `loc.swordsmans_school` |
| `0x59` | `Z2_12HAKUGINMAE` | `sSnowheadEntranceTable` L2662 | `SCENE_12HAKUGINMAE` = `0x5C` (scene_table.h:189) | `ext.z2_12hakuginmae` |
| `0x5E` | `Z2_YADOYA` | `sStockPotInnEntranceTable` L2667 | `SCENE_YADOYA` = `0x61` (scene_table.h:199) | `loc.stock_pot_inn` |
| `0x60` | `Z2_INSIDETOWER` | `sClockTowerInteriorEntranceTable` L2669 | `SCENE_INSIDETOWER` = `0x63` (scene_table.h:203) | `loc.clock_tower_interior` |
| `0x65` | `Z2_BOMYA` | `sBombShopEntranceTable` L2674 | `SCENE_BOMYA` = `0x68` (scene_table.h:213) | `loc.bomb_shop` |
| `0x69` | `Z2_TOWN` | `sEastClockTownEntranceTable` L2678 | `SCENE_TOWN` = `0x6C` (scene_table.h:221) | `loc.east_clock_town` |
| `0x6A` | `Z2_ICHIBA` | `sWestClockTownEntranceTable` L2679 | `SCENE_ICHIBA` = `0x6D` (scene_table.h:223) | `loc.west_clock_town` |
| `0x6B` | `Z2_BACKTOWN` | `sNorthClockTownEntranceTable` L2680 | `SCENE_BACKTOWN` = `0x6E` (scene_table.h:225) | `loc.north_clock_town` |
| `0x6C` | `Z2_CLOCKTOWER` | `sSouthClockTownEntranceTable` L2681 | `SCENE_CLOCKTOWER` = `0x6F` (scene_table.h:227) | `loc.south_clock_town` |
| `0x6D` | `Z2_ALLEY` | `sLaundryPoolEntranceTable` L2682 | `SCENE_ALLEY` = `0x70` (scene_table.h:229) | `loc.laundry_pool` |

## 3. Exterior collision exits

### 3.1 Eight source-derived Day 1 preview boundaries (CITY_PLAYBOOK, verbatim)

| Destination / approach | Source collision polygons | Exit-list value | Entrance-table scene index |
| --- | --- | --- | --- |
| North Clock Town | 8, 9 | `0xD620` | `0x6B` |
| Termina Field | 176, 177 | `0x5460` | `0x2A` |
| East Clock Town, upper | 24, 25 | `0xD230` | `0x69` |
| East Clock Town, lower | 184, 185 | `0xD210` | `0x69` |
| West Clock Town, upper | 18, 19 | `0xD420` | `0x6A` |
| West Clock Town, lower | 165, 166 | `0xD410` | `0x6A` |
| Laundry Pool | 158, 159 | `0xDA00` | `0x6D` |
| Clock Tower interior | 180, 181 | `0xC010` | `0x60` |

### 3.2 Source re-check of that table (decoded from C)

The exit values and entrance indices above all agree with source. The polygon column does not for four rows: it is permuted across North, East lower, West upper and the tower interior. Raw check: polygon `type` field, then `SurfaceType[type]` word 0 bits 8-12, then `ExitList_000184[exit+1 - 1]`.

| Playbook row | Playbook polygons | Source polygons for that exit value | Raw evidence (`Z2_CLOCKTOWER.c`) |
| --- | --- | --- | --- |
| North Clock Town `0xD620` | 8, 9 | 18, 19 | Polys 18/19 (`:1359-1360`) type 2, `0x500`, exit+1 = 5, list[4] = `0xD620` |
| East Clock Town, lower `0xD210` | 184, 185 | 180, 181 | Polys 180/181 (`:1521-1522`) type 12, `0x800`, exit+1 = 8, list[7] = `0xD210` |
| West Clock Town, upper `0xD420` | 18, 19 | 184, 185 | Polys 184/185 (`:1525-1526`) type 13, `0x400`, exit+1 = 4, list[3] = `0xD420` |
| Clock Tower interior `0xC010` | 180, 181 | 8, 9 | Polys 8/9 (`:1349-1350`) type 1, `0x100`, exit+1 = 1, list[0] = `0xC010` |

Independent corroboration (inferred): the exteriors share one world frame. West polys 380/381 sit at (-1100,150,-1099), on South 184/185; East polys 461/462 at (514,44,240), on South 180/181; North polys 92/95/135/136 at (-264,196,-1466), on South 18/19; South 8/9 end at z = -807, the tower door leaf line in ROM_REFERENCE. The MVP's eight `Threshold(...)` bounds (`Unity/Assets/Editor/BuildCityMvp.cs:124-131`, read only) convert back to exactly the source polygons for each label, so the preview labels are correct; only the playbook's polygon column needs correcting by the doc owner. The four other rows and both East and both West approaches stay as listed.

### 3.3 All exterior exit polygons, default collision (present in header + decoded from C)

Centroid and AABB are computed from the polygons' vertices (N64 u). Kind: floor (all normals near +Y), wall, or mixed.

| Scene | exit+1 | List index, value | Destination | Surface type(s) | Polygons | Centroid | AABB | Kind |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `Z2_CLOCKTOWER` | 1 | [0] `0xC010` | `loc.clock_tower_interior` | 1 | 8, 9 | (-280,0,-887) | (-360,0,-967)..(-200,0,-807) | floor |
| `Z2_CLOCKTOWER` | 2 | [1] `0x5460` | `ext.z2_00keikoku` | 11 | 176, 177 | (-280,0,840) | (-370,0,680)..(-190,0,1000) | floor |
| `Z2_CLOCKTOWER` | 3 | [2] `0xD230` | `loc.east_clock_town` | 3 | 24, 25 | (316,100,-775) | (232,100,-835)..(400,100,-715) | floor |
| `Z2_CLOCKTOWER` | 4 | [3] `0xD420` | `loc.west_clock_town` | 13 | 184, 185 | (-1100,150,-1100) | (-1200,100,-1200)..(-1000,200,-1000) | floor |
| `Z2_CLOCKTOWER` | 5 | [4] `0xD620` | `loc.north_clock_town` | 2 | 18, 19 | (-260,200,-1516) | (-320,200,-1600)..(-200,200,-1432) | floor |
| `Z2_CLOCKTOWER` | 6 | [5] `0xD410` | `loc.west_clock_town` | 10 | 165, 166 | (-916,0,-100) | (-1000,0,-160)..(-832,0,-40) | floor |
| `Z2_CLOCKTOWER` | 7 | [6] `0xDA00` | `loc.laundry_pool` | 8 | 158, 159 | (-990,100,523) | (-1120,100,435)..(-859,100,600) | floor |
| `Z2_CLOCKTOWER` | 8 | [7] `0xD210` | `loc.east_clock_town` | 12 | 180, 181 | (534,50,240) | (347,0,120)..(720,100,360) | floor |
| `Z2_CLOCKTOWER` | 9 | [8] `0x2C00` | `loc.clock_tower_rooftop` | 4, 5, 17 | 46-63, 368, 369, 374, 375 (22) | (-280,338,-955) | (-340,300,-1053)..(-220,375,-893) | mixed |
| `Z2_BACKTOWN` | 1 | [0] `0x5480` | `ext.z2_00keikoku` | 5 | 125, 126 | (-470,200,-2640) | (-560,200,-2773)..(-380,200,-2507) | floor |
| `Z2_BACKTOWN` | 2 | [1] `0xD250` | `loc.east_clock_town` | 6 | 138, 139 | (746,200,-1784) | (613,200,-1864)..(880,200,-1704) | floor |
| `Z2_BACKTOWN` | 3 | [2] `0xD840` | `loc.south_clock_town` | 3 | 92, 95, 135, 136 | (-264,196,-1466) | (-320,191,-1599)..(-200,200,-1280) | floor |
| `Z2_BACKTOWN` | 4 | [3] `0x4600` | `loc.great_fairy_fountain_town` | 4 | 118, 119 | (-1204,264,-2013) | (-1291,243,-2073)..(-1117,284,-1953) | floor |
| `Z2_ICHIBA` | 1 | [0] `0x5400` | `ext.z2_00keikoku` | 15 | 333, 334 | (-2237,200,-1084) | (-2360,200,-1178)..(-2114,201,-991) | floor |
| `Z2_ICHIBA` | 2 | [1] `0xD850` | `loc.south_clock_town` | 18 | 374, 375 | (-884,0,-100) | (-968,0,-160)..(-800,0,-40) | floor |
| `Z2_ICHIBA` | 3 | [2] `0xD830` | `loc.south_clock_town` | 19 | 380, 381 | (-1100,150,-1099) | (-1200,100,-1208)..(-1000,200,-992) | floor |
| `Z2_ICHIBA` | 4 | [3] `0xA200` | `loc.swordsmans_school` | 7 | 54, 55 | (-1870,200,-1456) | (-1900,200,-1513)..(-1840,200,-1400) | floor |
| `Z2_ICHIBA` | 5 | [4] `0x0E00` | `loc.curiosity_shop` | 13 | 208, 209 | (-1522,45,18) | (-1570,45,-42)..(-1474,45,79) | floor |
| `Z2_ICHIBA` | 6 | [5] `0x6200` | `loc.trading_post` | 12 | 206, 207 | (-1819,105,-180) | (-1878,105,-239)..(-1761,105,-122) | floor |
| `Z2_ICHIBA` | 7 | [6] `0xCA00` | `loc.bomb_shop` | 11 | 204, 205 | (-2016,165,-478) | (-2074,165,-526)..(-1958,165,-431) | floor |
| `Z2_ICHIBA` | 8 | [7] `0x5600` | `loc.post_office` | 8 | 56, 57 | (-1610,200,-1454) | (-1640,200,-1509)..(-1580,200,-1400) | floor |
| `Z2_ICHIBA` | 9 | [8] `0x6C00` | `loc.lottery_shop` | 17 | 370, 371 | (-1406,200,-770) | (-1467,200,-831)..(-1345,200,-709) | floor |
| `Z2_TOWN` | 1 | [0] `0x5470` | `ext.z2_00keikoku` | 10 | 299, 300 | (1600,60,-710) | (1480,60,-800)..(1720,60,-620) | floor |
| `Z2_TOWN` | 2 | [1] `0xD870` | `loc.south_clock_town` | 13 | 461, 462 | (514,44,240) | (347,0,120)..(680,89,360) | floor |
| `Z2_TOWN` | 3 | [2] `0x4C00` | `loc.astral_observatory` | 4, 5 | 116, 117, 169 | (1463,103,-1407) | (594,80,-1824)..(1960,245,-628) | mixed |
| `Z2_TOWN` | 4 | [3] `0xD820` | `loc.south_clock_town` | 9 | 290, 291 | (284,100,-775) | (200,100,-835)..(368,100,-715) | floor |
| `Z2_TOWN` | 5 | [4] `0x2800` | `loc.treasure_chest_shop` | 20 | 481, 482 | (610,100,-120) | (580,100,-160)..(640,100,-80) | floor |
| `Z2_TOWN` | 6 | [5] `0xD610` | `loc.north_clock_town` | 8 | 286, 287 | (644,200,-1784) | (520,200,-1864)..(768,200,-1704) | floor |
| `Z2_TOWN` | 7 | [6] `0x0800` | `loc.honey_darling` | 18 | 477, 478 | (1440,100,-274) | (1400,100,-304)..(1480,100,-244) | floor |
| `Z2_TOWN` | 8 | [7] `0x0000` | `loc.mayors_residence` | 17 | 475, 476 | (1100,200,-2040) | (1070,200,-2080)..(1130,200,-2000) | floor |
| `Z2_TOWN` | 9 | [8] `0x3A00` | `loc.town_shooting_gallery` | 19 | 479, 480 | (1128,100,288) | (1079,100,239)..(1178,100,338) | floor |
| `Z2_TOWN` | 10 | [9] `0xBC00` | `loc.stock_pot_inn` | 14 | 469, 470 | (668,100,-895) | (638,100,-935)..(698,100,-855) | floor |
| `Z2_TOWN` | 11 | [10] `0xBC10` | `loc.stock_pot_inn` | 16 | 473, 474 | (477,280,-1320) | (447,280,-1360)..(507,280,-1280) | floor |
| `Z2_TOWN` | 13 | [12] `0x2400` | `loc.milk_bar` | 15 | 471, 472 | (1038,100,-951) | (1008,100,-991)..(1068,100,-911) | floor |
| `Z2_ALLEY` | 1 | [0] `0xD860` | `loc.south_clock_town` | 6 | 130, 131 | (-1196,75,395) | (-1275,50,355)..(-1120,100,435) | floor |
| `Z2_ALLEY` | 3 | [2] `0x0E10` | `loc.curiosity_shop` | 3 | 40, 41 | (-2071,-120,-193) | (-2126,-120,-283)..(-2016,-120,-102) | floor |

Exit-list entries with no exterior surface: North [4] (`DOOR_ANA`), [5] (`EN_BOMJIMB`), [6], [7]; West [9] (`EN_BOMJIMB`); East [11] (`EN_BOMJIMB`), [13]; South [9]; Laundry Pool [1], [3]. East exit+1 = 3 has two separate sites: floor polys 116/117 near (1896,80,-1794) and wall poly 169 at (597,148,-634); the wall site is never read by the player (see U3, resolved). South exit+1 = 9 is a 22-polygon run of alternating treads and risers at x -340..-220, y 300-375, z -1053..-893.

## 4. Directed portals, default headers

Column notes. **Mode** (inferred): `district` = walk-in collision exit with no door actor; `door` = collision exit behind an `EN_DOOR` transition actor or the tower door; `room` = transition-actor room swap inside one scene; `special` = actor-driven exit or a state-gated surface; `external-boundary` = destination outside the 22 scenes (ID uses the lowercase scene symbol, loc column `ext.<symbol>`). **Evidence** (present in header): exit-list symbol and index, surface type and polygons, or actor params with `room_NN:line`; door association is inferred by nearest `EN_DOOR` (distance in u). **Decode** (decoded from C): scene index / slot / layer, entry array, `z_scene_table.c` line. **Arrival** (decoded from C): destination default-header pose, room, start-position index and start mode. **Reverse** (inferred): the destination portal whose arrival lands nearest this trigger; the two distances are (that arrival to this trigger, that trigger to this arrival). **Conditions**: only what source shows; door type names are `ENDOOR_SCH_TYPE_*` (`z_en_door.h:54-87`); kickout half-days use `HALFDAYBIT_DAYn` names (D0-D4, d = dawn/day, n = night).

### 4.1 Exteriors (41 rows)

| Portal ID | Mode | From | Evidence | Raw | Decode | Destination | Spawn | Arrival | Reverse | Conditions | Traversal |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `portal.south_clock_town.clock_tower_interior` | door | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[0]; surf 1 (exit+1=1); polys 8,9; behind Obj_Tokei_Tobira z=-807 (inferred) | `0xC010` | 0x60/1/0 `sClockTowerInteriorEntrance1`[0] L2044 | `loc.clock_tower_interior` | 1 | (1,30,182) yaw 180 room 0 start1 mode 0xD | `portal.clock_tower_interior.south_clock_town` (inferred: 86u/177u) | UNKNOWN | not_run |
| `portal.south_clock_town.z2_00keikoku` | external-boundary | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[1]; surf 11 (exit+1=2); polys 176,177 | `0x5460` | 0x2A/6/0 `sTerminaFieldEntrance6`[0] L880 | `ext.z2_00keikoku` | 6 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.south_clock_town.east_clock_town.upper` | district | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[2]; surf 3 (exit+1=3); polys 24,25 | `0xD230` | 0x69/3/0 `sEastClockTownEntrance3`[0] L2213 | `loc.east_clock_town` | 3 | (406,100,-777) yaw 90 room 0 start3 mode 0xF | `portal.east_clock_town.south_clock_town.upper` (inferred: 136u/122u) | UNKNOWN | not_run |
| `portal.south_clock_town.west_clock_town.upper` | district | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[3]; surf 13 (exit+1=4); polys 184,185 | `0xD420` | 0x6A/2/0 `sWestClockTownEntrance2`[0] L2269 | `loc.west_clock_town` | 2 | (-1253,200,-1095) yaw 270 room 0 start2 mode 0xF | `portal.west_clock_town.south_clock_town.upper` (inferred: 243u/161u) | UNKNOWN | not_run |
| `portal.south_clock_town.north_clock_town` | district | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[4]; surf 2 (exit+1=5); polys 18,19 | `0xD620` | 0x6B/2/0 `sNorthClockTownEntrance2`[0] L2316 | `loc.north_clock_town` | 2 | (-265,189,-1656) yaw 180 room 0 start2 mode 0xF | `portal.north_clock_town.south_clock_town` (inferred: 136u/190u) | UNKNOWN | not_run |
| `portal.south_clock_town.west_clock_town.lower` | district | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[5]; surf 10 (exit+1=6); polys 165,166 | `0xD410` | 0x6A/1/0 `sWestClockTownEntrance1`[0] L2265 | `loc.west_clock_town` | 1 | (-1007,0,-107) yaw 267 room 0 start1 mode 0xE | `portal.west_clock_town.south_clock_town.lower` (inferred: 136u/123u) | UNKNOWN | not_run |
| `portal.south_clock_town.laundry_pool` | district | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[6]; surf 8 (exit+1=7); polys 158,159 | `0xDA00` | 0x6D/0/0 `sLaundryPoolEntrance0`[0] L2399 | `loc.laundry_pool` | 0 | (-1426,-8,397) yaw 248 room 0 start0 mode 0xE | `portal.laundry_pool.south_clock_town` (inferred: 154u/245u) | UNKNOWN | not_run |
| `portal.south_clock_town.east_clock_town.lower` | district | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[7]; surf 12 (exit+1=8); polys 180,181 | `0xD210` | 0x69/1/0 `sEastClockTownEntrance1`[0] L2205 | `loc.east_clock_town` | 1 | (697,94,205) yaw 90 room 0 start1 mode 0xF | `portal.east_clock_town.south_clock_town.lower` (inferred: 217u/193u) | UNKNOWN | not_run |
| `portal.south_clock_town.clock_tower_rooftop` | special | `loc.south_clock_town` | `Z2_CLOCKTOWERExitList_000184`[8]; surf 4,5,17 (exit+1=9); polys 22 polys 46-63,368,369,374,375 | `0x2C00` | 0x16/0/0 `sClockTowerRooftopEntrance0`[0] L408 | `loc.clock_tower_rooftop` | 0 | (0,20,222) yaw 180 room 0 start0 mode 0xD | MISSING: no return exit in dest default header | tower-top stair polys; reachability state UNKNOWN | not_run |
| `portal.north_clock_town.z2_00keikoku` | external-boundary | `loc.north_clock_town` | `Z2_BACKTOWNExitList_00012C`[0]; surf 5 (exit+1=1); polys 125,126 | `0x5480` | 0x2A/8/0 `sTerminaFieldEntrance8`[0] L890 | `ext.z2_00keikoku` | 8 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.north_clock_town.east_clock_town` | district | `loc.north_clock_town` | `Z2_BACKTOWNExitList_00012C`[1]; surf 6 (exit+1=2); polys 138,139 | `0xD250` | 0x69/5/0 `sEastClockTownEntrance5`[0] L2221 | `loc.east_clock_town` | 5 | (807,200,-1776) yaw 90 room 0 start5 mode 0xF | `portal.east_clock_town.north_clock_town` (inferred: 184u/163u) | UNKNOWN | not_run |
| `portal.north_clock_town.south_clock_town` | district | `loc.north_clock_town` | `Z2_BACKTOWNExitList_00012C`[2]; surf 3 (exit+1=3); polys 92,95,135,136 | `0xD840` | 0x6C/4/0 `sSouthClockTownEntrance4`[0] L2365 | `loc.south_clock_town` | 4 | (-260,200,-1380) yaw 0 room 0 start4 mode 0xE | `portal.south_clock_town.north_clock_town` (inferred: 190u/136u) | UNKNOWN | not_run |
| `portal.north_clock_town.great_fairy_fountain_town` | district | `loc.north_clock_town` | `Z2_BACKTOWNExitList_00012C`[3]; surf 4 (exit+1=4); polys 118,119 | `0x4600` | 0x23/0/0 `sFairyFountainEntrance0`[0] L699 | `loc.great_fairy_fountain_town` | 0 | (2400,20,360) yaw 180 room 0 start0 mode 0xF | `portal.great_fairy_fountain_town.north_clock_town` (inferred: 173u/260u) | UNKNOWN | not_run |
| `portal.north_clock_town.deku_playground` | special | `loc.north_clock_town` | `Z2_BACKTOWNExitList_00012C`[4]; DOOR_ANA 0x0304 x1 (room_00:52) | `0x3600` | 0x1B/0/0 `sDekuScrubPlaygroundEntrance0`[0] L525 | `loc.deku_playground` | 0 | (-28,400,1939) yaw 180 room 0 start0 mode 0xD | `portal.deku_playground.north_clock_town` (inferred: 6u/367u) | DOOR_ANA type 0x300 scene-exit grotto; entry trigger z_door_ana.c:129-138 | not_run |
| `portal.north_clock_town.north_clock_town.bomjimb` | special | `loc.north_clock_town` | `Z2_BACKTOWNExitList_00012C`[5]; EN_BOMJIMB 0x050F/0x0513 x2 (room_00:98, room_00:142) | `0xD650` | 0x6B/5/0 `sNorthClockTownEntrance5`[0] L2328 | `loc.north_clock_town` | 5 | (-666,200,-2256) yaw 180 room 0 start5 mode 0xD | one-way (actor-driven) | EN_BOMJIMB func_80C02CA4 (z_en_bomjimb.c:859); trigger state UNKNOWN | not_run |
| `portal.west_clock_town.z2_00keikoku` | external-boundary | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[0]; surf 15 (exit+1=1); polys 333,334 | `0x5400` | 0x2A/0/0 `sTerminaFieldEntrance0`[0] L846 | `ext.z2_00keikoku` | 0 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.west_clock_town.south_clock_town.lower` | district | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[1]; surf 18 (exit+1=2); polys 374,375 | `0xD850` | 0x6C/5/0 `sSouthClockTownEntrance5`[0] L2369 | `loc.south_clock_town` | 5 | (-780,0,-100) yaw 90 room 0 start5 mode 0xF | `portal.south_clock_town.west_clock_town.lower` (inferred: 123u/136u) | UNKNOWN | not_run |
| `portal.west_clock_town.south_clock_town.upper` | district | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[2]; surf 19 (exit+1=3); polys 380,381 | `0xD830` | 0x6C/3/0 `sSouthClockTownEntrance3`[0] L2361 | `loc.south_clock_town` | 3 | (-862,100,-1094) yaw 90 room 0 start3 mode 0xF | `portal.south_clock_town.west_clock_town.upper` (inferred: 161u/243u) | UNKNOWN | not_run |
| `portal.west_clock_town.swordsmans_school` | door | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[3]; surf 7 (exit+1=4); polys 54,55; TA#1 EN_DOOR 36u | `0xA200` | 0x51/0/0 `sSwordmansSchoolEntrance0`[0] L1798 | `loc.swordsmans_school` | 0 | (-1,0,380) yaw 180 room 0 start0 mode 0xE | `portal.swordsmans_school.west_clock_town` (inferred: 67u/115u) | EN_DOOR SCHEDULE/SWORDSMANS_SCHOOL; open hours UNKNOWN | not_run |
| `portal.west_clock_town.curiosity_shop` | door | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[4]; surf 13 (exit+1=5); polys 208,209; TA#5 EN_DOOR 32u | `0x0E00` | 0x07/0/0 `sCuriosityShopEntrance0`[0] L145 | `loc.curiosity_shop` | 0 | (1,0,227) yaw 180 room 0 start0 mode 0xE | `portal.curiosity_shop.west_clock_town` (inferred: 81u/95u) | EN_DOOR SCHEDULE/CURIOSITY_SHOP; open hours UNKNOWN | not_run |
| `portal.west_clock_town.trading_post` | door | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[5]; surf 12 (exit+1=6); polys 206,207; TA#4 EN_DOOR 33u | `0x6200` | 0x31/0/0 `sTradingPostEntrance0`[0] L1029 | `loc.trading_post` | 0 | (-1,15,189) yaw 180 room 0 start0 mode 0xE | `portal.trading_post.west_clock_town` (inferred: 87u/111u) | EN_DOOR SCHEDULE/TRADING_POST; open hours UNKNOWN | not_run |
| `portal.west_clock_town.bomb_shop` | door | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[6]; surf 11 (exit+1=7); polys 204,205; TA#3 EN_DOOR 30u | `0xCA00` | 0x65/0/0 `sBombShopEntrance0`[0] L2125 | `loc.bomb_shop` | 0 | (61,-30,366) yaw 180 room 0 start0 mode 0xF | `portal.bomb_shop.west_clock_town` (inferred: 76u/114u) | EN_DOOR SCHEDULE/BOMB_SHOP; open hours UNKNOWN | not_run |
| `portal.west_clock_town.post_office` | door | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[7]; surf 8 (exit+1=8); polys 56,57; TA#0 EN_DOOR 34u | `0x5600` | 0x2B/0/0 `sPostOfficeEntrance0`[0] L931 | `loc.post_office` | 0 | (-10,0,265) yaw 180 room 0 start0 mode 0xE | `portal.post_office.west_clock_town` (inferred: 86u/81u) | EN_DOOR SCHEDULE/POST_OFFICE; open hours UNKNOWN | not_run |
| `portal.west_clock_town.lottery_shop` | door | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[8]; surf 17 (exit+1=9); polys 370,371; TA#2 EN_DOOR 37u | `0x6C00` | 0x36/0/0 `sLotteryShopEntrance0`[0] L1255 | `loc.lottery_shop` | 0 | (0,0,320) yaw 180 room 0 start0 mode 0xE | `portal.lottery_shop.west_clock_town` (inferred: 64u/100u) | EN_DOOR SCHEDULE/LOTTERY_SHOP; open hours UNKNOWN | not_run |
| `portal.west_clock_town.north_clock_town.bomjimb` | special | `loc.west_clock_town` | `Z2_ICHIBAExitList_0001A0`[9]; EN_BOMJIMB 0x0910 x1 (room_00:95) | `0xD650` | 0x6B/5/0 `sNorthClockTownEntrance5`[0] L2328 | `loc.north_clock_town` | 5 | (-666,200,-2256) yaw 180 room 0 start5 mode 0xD | one-way (actor-driven) | EN_BOMJIMB func_80C02CA4 (z_en_bomjimb.c:859); trigger state UNKNOWN | not_run |
| `portal.east_clock_town.z2_00keikoku` | external-boundary | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[0]; surf 10 (exit+1=1); polys 299,300 | `0x5470` | 0x2A/7/0 `sTerminaFieldEntrance7`[0] L885 | `ext.z2_00keikoku` | 7 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.east_clock_town.south_clock_town.lower` | district | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[1]; surf 13 (exit+1=2); polys 461,462 | `0xD870` | 0x6C/7/0 `sSouthClockTownEntrance7`[0] L2377 | `loc.south_clock_town` | 7 | (323,0,247) yaw 270 room 0 start7 mode 0xF | `portal.south_clock_town.east_clock_town.lower` (inferred: 193u/217u) | UNKNOWN | not_run |
| `portal.east_clock_town.astral_observatory.p116` | district | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[2]; surf 4,5 (exit+1=3); polys 116,117 | `0x4C00` | 0x26/0/0 `sAstralObservatoryEntrance0`[0] L762 | `loc.astral_observatory` | 0 | (602,-472,4125) yaw 180 room 0 start0 mode 0xD | `portal.astral_observatory.east_clock_town` (inferred: 134u/116u) | UNKNOWN | not_run |
| `portal.east_clock_town.astral_observatory.p169` | none (data oddity, not a portal) | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[2]; surf 4,5 (exit+1=3); poly 169, a WALL face of a six-sided post under the Inn upper deck (REF-07) | `0x4C00` | 0x26/0/0 `sAstralObservatoryEntrance0`[0] L762 | `loc.astral_observatory` | 0 | (602,-472,4125) yaw 180 room 0 start0 mode 0xD | not applicable: `Player_HandleExitsAndVoids` only receives floor polygons (`z_player.c:6783` door probe, `:12572` standing floor), so this wall can never fire; the real observatory portal is the `.p116` row | not applicable | not_applicable |
| `portal.east_clock_town.south_clock_town.upper` | district | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[3]; surf 9 (exit+1=4); polys 290,291 | `0xD820` | 0x6C/2/0 `sSouthClockTownEntrance2`[0] L2357 | `loc.south_clock_town` | 2 | (180,100,-770) yaw 270 room 0 start2 mode 0xF | `portal.south_clock_town.east_clock_town.upper` (inferred: 122u/136u) | UNKNOWN | not_run |
| `portal.east_clock_town.treasure_chest_shop` | door | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[4]; surf 20 (exit+1=5); polys 481,482; TA#2 EN_DOOR 20u | `0x2800` | 0x14/0/0 `sTreasureChestShopEntrance0`[0] L377 | `loc.treasure_chest_shop` | 0 | (-70,0,161) yaw 0 room 0 start0 mode 0xE | `portal.treasure_chest_shop.east_clock_town` (inferred: 53u/101u) | EN_DOOR SCHEDULE/TREASURE_CHEST_SHOP; open hours UNKNOWN | not_run |
| `portal.east_clock_town.north_clock_town` | district | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[5]; surf 8 (exit+1=6); polys 286,287 | `0xD610` | 0x6B/1/0 `sNorthClockTownEntrance1`[0] L2312 | `loc.north_clock_town` | 1 | (562,193,-1785) yaw 270 room 0 start1 mode 0xF | `portal.north_clock_town.east_clock_town` (inferred: 163u/184u) | UNKNOWN | not_run |
| `portal.east_clock_town.honey_darling` | door | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[6]; surf 18 (exit+1=7); polys 477,478; TA#4 EN_DOOR 20u | `0x0800` | 0x04/0/0 `sHoneyAndDarlingsShopEntrance0`[0] L73 | `loc.honey_darling` | 0 | (0,0,300) yaw 180 room 0 start0 mode 0xE | `portal.honey_darling.east_clock_town` (inferred: 45u/92u) | EN_DOOR SCHEDULE/HONEY_DARLING_SHOP; open hours UNKNOWN | not_run |
| `portal.east_clock_town.mayors_residence` | door | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[7]; surf 17 (exit+1=8); polys 475,476; TA#5 EN_DOOR 20u | `0x0000` | 0x00/0/0 `sMayorsResidenceEntrance0`[0] L26 | `loc.mayors_residence` | 0 | (3,0,304) yaw 180 room 0 start0 mode 0xE | `portal.mayors_residence.east_clock_town` (inferred: 48u/116u) | EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MAIN_ENTRANCE; open hours UNKNOWN | not_run |
| `portal.east_clock_town.town_shooting_gallery` | door | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[8]; surf 19 (exit+1=9); polys 479,480; TA#3 EN_DOOR 20u | `0x3A00` | 0x1D/0/0 `sTownShootingGalleryEntrance0`[0] L546 | `loc.town_shooting_gallery` | 0 | (-10,0,390) yaw 180 room 0 start0 mode 0xE | `portal.town_shooting_gallery.east_clock_town` (inferred: 45u/63u) | EN_DOOR SCHEDULE/TOWN_SHOOTING_GALLERY; open hours UNKNOWN | not_run |
| `portal.east_clock_town.stock_pot_inn.lower` | door | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[9]; surf 14 (exit+1=10); polys 469,470; TA#0 EN_DOOR 20u | `0xBC00` | 0x5E/0/0 `sStockPotInnEntrance0`[0] L2002 | `loc.stock_pot_inn` | 0 | (149,0,303) yaw 180 room 0 start0 mode 0xF | `portal.stock_pot_inn.east_clock_town.lower` (inferred: 49u/199u) | EN_DOOR SCHEDULE/INN_MAIN_ENTRANCE; open hours UNKNOWN | not_run |
| `portal.east_clock_town.stock_pot_inn.upper` | door | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[10]; surf 16 (exit+1=11); polys 473,474; TA#1 EN_DOOR 20u | `0xBC10` | 0x5E/1/0 `sStockPotInnEntrance1`[0] L2007 | `loc.stock_pot_inn` | 1 | (-63,210,-46) yaw 180 room 0 start1 mode 0xD | `portal.stock_pot_inn.east_clock_town.upper` (inferred: 50u/106u) | EN_DOOR SCHEDULE/INN_UPPER_ENTRANCE; open hours UNKNOWN | not_run |
| `portal.east_clock_town.north_clock_town.bomjimb` | special | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[11]; EN_BOMJIMB 0x0B11/0x0B22 x2 (room_00:229, room_00:231) | `0xD650` | 0x6B/5/0 `sNorthClockTownEntrance5`[0] L2328 | `loc.north_clock_town` | 5 | (-666,200,-2256) yaw 180 room 0 start5 mode 0xD | one-way (actor-driven) | EN_BOMJIMB func_80C02CA4 (z_en_bomjimb.c:859); trigger state UNKNOWN | not_run |
| `portal.east_clock_town.milk_bar` | door | `loc.east_clock_town` | `Z2_TOWNExitList_0001F0`[12]; surf 15 (exit+1=13); polys 471,472; TA#6 EN_DOOR 20u | `0x2400` | 0x12/0/0 `sMilkBarEntrance0`[0] L355 | `loc.milk_bar` | 0 | (-200,152,360) yaw 180 room 0 start0 mode 0xE | `portal.milk_bar.east_clock_town` (inferred: 49u/110u) | EN_DOOR SCHEDULE/MILK_BAR; open hours UNKNOWN | not_run |
| `portal.laundry_pool.south_clock_town` | district | `loc.laundry_pool` | `Z2_ALLEYExitList_0000C4`[0]; surf 6 (exit+1=1); polys 130,131 | `0xD860` | 0x6C/6/0 `sSouthClockTownEntrance6`[0] L2373 | `loc.south_clock_town` | 6 | (-840,100,560) yaw 90 room 0 start6 mode 0xF | `portal.south_clock_town.laundry_pool` (inferred: 245u/154u) | UNKNOWN | not_run |
| `portal.laundry_pool.curiosity_shop` | door | `loc.laundry_pool` | `Z2_ALLEYExitList_0000C4`[2]; surf 3 (exit+1=3); polys 40,41; TA#0 EN_DOOR 85u | `0x0E10` | 0x07/1/0 `sCuriosityShopEntrance1`[0] L149 | `loc.curiosity_shop` | 1 | (-262,-96,-619) yaw 0 room 1 start1 mode 0xE | `portal.curiosity_shop.laundry_pool` (inferred: 152u/151u) | EN_DOOR SCHEDULE/LAUNDRY_POOL; open hours UNKNOWN | not_run |

### 4.2 Venues and annexes (36 rows)

| Portal ID | Mode | From | Evidence | Raw | Decode | Destination | Spawn | Arrival | Reverse | Conditions | Traversal |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `portal.trading_post.west_clock_town` | door | `loc.trading_post` | `Z2_8ITEMSHOPExitList_0000A4`[0]; surf 4 (exit+1=1); polys 35,36; TA#0 EN_DOOR 75u | `0xD450` | 0x6A/5/0 `sWestClockTownEntrance5`[0] L2281 | `loc.west_clock_town` | 5 | (-1760,105,-244) yaw 125 room 0 start5 mode 0xD | `portal.west_clock_town.trading_post` (inferred: 111u/87u) | EN_DOOR SCHEDULE/TRADING_POST; open hours UNKNOWN | not_run |
| `portal.trading_post.west_clock_town.kickout` | special | `loc.trading_post` | `Z2_8ITEMSHOPExitList_0000A4`[0]; EN_TIME_TAG 0x0100 x1 (room_00:72) | `0xD450` | 0x6A/5/0 `sWestClockTownEntrance5`[0] L2281 | `loc.west_clock_town` | 5 | (-1760,105,-244) yaw 125 room 0 start5 mode 0xD | one-way (kickout); door row is the inbound | kickout at 21:00 text TRADING_POST [all half-days] | not_run |
| `portal.bomb_shop.west_clock_town` | door | `loc.bomb_shop` | `Z2_BOMYAExitList_0000AC`[0]; surf 5 (exit+1=1); polys 76,77; TA#0 EN_DOOR 75u | `0xD460` | 0x6A/6/0 `sWestClockTownEntrance6`[0] L2285 | `loc.west_clock_town` | 6 | (-1946,165,-508) yaw 113 room 0 start6 mode 0xD | `portal.west_clock_town.bomb_shop` (inferred: 114u/76u) | EN_DOOR SCHEDULE/BOMB_SHOP; open hours UNKNOWN | not_run |
| `portal.curiosity_shop.west_clock_town` | door | `loc.curiosity_shop` | `Z2_AYASHIISHOPExitList_000108`[0]; surf 4 (exit+1=1); polys 58,59; TA#0 EN_DOOR 70u | `0xD440` | 0x6A/4/0 `sWestClockTownEntrance4`[0] L2277 | `loc.west_clock_town` | 4 | (-1490,45,-56) yaw 152 room 0 start4 mode 0xD | `portal.west_clock_town.curiosity_shop` (inferred: 95u/81u) | EN_DOOR SCHEDULE/CURIOSITY_SHOP; open hours UNKNOWN | not_run |
| `portal.curiosity_shop.west_clock_town.kickout` | special | `loc.curiosity_shop` | `Z2_AYASHIISHOPExitList_000108`[0]; EN_TIME_TAG 0x00C0 x1 (room_00:49) | `0xD440` | 0x6A/4/0 `sWestClockTownEntrance4`[0] L2277 | `loc.west_clock_town` | 4 | (-1490,45,-56) yaw 152 room 0 start4 mode 0xD | one-way (kickout); door row is the inbound | kickout at 05:00 text GENERIC [D1d+D1n+D2d+D2n] | not_run |
| `portal.curiosity_shop.laundry_pool` | door | `loc.curiosity_shop` | `Z2_AYASHIISHOPExitList_000108`[1]; surf 12 (exit+1=2); polys 114,115; TA#1 EN_DOOR 158u | `0xDA10` | 0x6D/1/0 `sLaundryPoolEntrance1`[0] L2403 | `loc.laundry_pool` | 1 | (-2121,-120,-49) yaw 342 room 0 start1 mode 0xE | `portal.laundry_pool.curiosity_shop` (inferred: 151u/152u) | EN_DOOR SCHEDULE/LAUNDRY_POOL; open hours UNKNOWN | not_run |
| `portal.curiosity_shop.laundry_pool.kickout` | special | `loc.curiosity_shop` | `Z2_AYASHIISHOPExitList_000108`[1]; EN_TIME_TAG 0x0141/0x0161 x2 (room_01:43, room_01:46) | `0xDA10` | 0x6D/1/0 `sLaundryPoolEntrance1`[0] L2403 | `loc.laundry_pool` | 1 | (-2121,-120,-49) yaw 342 room 0 start1 mode 0xE | one-way (kickout); door row is the inbound | kickout at 22:00 text CURIOSITY_BACK_1 [D2d+D2n], 22:00 text CURIOSITY_BACK_2 [D3d+D3n] | not_run |
| `portal.post_office.west_clock_town` | door | `loc.post_office` | `Z2_POSTHOUSEExitList_0000B4`[0]; surf 3 (exit+1=1); polys 58,59; TA#0 EN_DOOR 46u | `0xD470` | 0x6A/7/0 `sWestClockTownEntrance7`[0] L2289 | `loc.west_clock_town` | 7 | (-1609,200,-1368) yaw 0 room 0 start7 mode 0xF | `portal.west_clock_town.post_office` (inferred: 81u/86u) | EN_DOOR SCHEDULE/POST_OFFICE; open hours UNKNOWN | not_run |
| `portal.post_office.west_clock_town.kickout` | special | `loc.post_office` | `Z2_POSTHOUSEExitList_0000B4`[0]; EN_TIME_TAG 0x00C0 x3 (room_00:47, room_00:50, room_00:53) | `0xD470` | 0x6A/7/0 `sWestClockTownEntrance7`[0] L2289 | `loc.west_clock_town` | 7 | (-1609,200,-1368) yaw 0 room 0 start7 mode 0xF | one-way (kickout); door row is the inbound | kickout at 00:00 text GENERIC [D0d+D0n+D1d+D1n+D2d+D2n], 13:00 text GENERIC [all half-days], 15:00 text GENERIC [D2d+D2n+D3d+D3n+D4d+D4n] | not_run |
| `portal.swordsmans_school.west_clock_town` | door | `loc.swordsmans_school` | `Z2_DOUJOUExitList_0000B4`[0]; surf 0 (exit+1=1); polys 0,1; TA#0 EN_DOOR 75u | `0xD430` | 0x6A/3/0 `sWestClockTownEntrance3`[0] L2273 | `loc.west_clock_town` | 3 | (-1870,200,-1389) yaw 0 room 0 start3 mode 0xF | `portal.west_clock_town.swordsmans_school` (inferred: 115u/67u) | EN_DOOR SCHEDULE/SWORDSMANS_SCHOOL; open hours UNKNOWN | not_run |
| `portal.swordsmans_school.west_clock_town.kickout` | special | `loc.swordsmans_school` | `Z2_DOUJOUExitList_0000B4`[0]; EN_TIME_TAG 0x01C0 x1 (room_00:51) | `0xD430` | 0x6A/3/0 `sWestClockTownEntrance3`[0] L2273 | `loc.west_clock_town` | 3 | (-1870,200,-1389) yaw 0 room 0 start3 mode 0xF | one-way (kickout); door row is the inbound | kickout at 23:00 text SWORDSMANS_SCHOOL [D3d+D3n+D4d+D4n] | not_run |
| `portal.lottery_shop.west_clock_town` | door | `loc.lottery_shop` | `Z2_TAKARAKUJIExitList_0000A4`[0]; surf 1 (exit+1=1); polys 24,25; TA#0 EN_DOOR 75u | `0xD480` | 0x6A/8/0 `sWestClockTownEntrance8`[0] L2293 | `loc.west_clock_town` | 8 | (-1453,200,-814) yaw 229 room 0 start8 mode 0xF | `portal.west_clock_town.lottery_shop` (inferred: 100u/64u) | EN_DOOR SCHEDULE/LOTTERY_SHOP; open hours UNKNOWN | not_run |
| `portal.lottery_shop.west_clock_town.kickout` | special | `loc.lottery_shop` | `Z2_TAKARAKUJIExitList_0000A4`[0]; EN_TIME_TAG 0x0080 x1 (room_00:35) | `0xD480` | 0x6A/8/0 `sWestClockTownEntrance8`[0] L2293 | `loc.west_clock_town` | 8 | (-1453,200,-814) yaw 229 room 0 start8 mode 0xF | one-way (kickout); door row is the inbound | kickout at 23:00 text LOTTERY [all half-days] | not_run |
| `portal.stock_pot_inn.east_clock_town.lower` | door | `loc.stock_pot_inn` | `Z2_YADOYAExitList_000190`[0]; surf 3 (exit+1=1); polys 181,182; TA#5 EN_DOOR 140u | `0xD290` | 0x69/9/0 `sEastClockTownEntrance9`[0] L2237 | `loc.east_clock_town` | 9 | (669,100,-846) yaw 0 room 0 start9 mode 0xF | `portal.east_clock_town.stock_pot_inn.lower` (inferred: 199u/49u) | EN_DOOR SCHEDULE/INN_MAIN_ENTRANCE; open hours UNKNOWN | not_run |
| `portal.stock_pot_inn.east_clock_town.kickout` | special | `loc.stock_pot_inn` | `Z2_YADOYAExitList_000190`[0]; EN_TIME_TAG 0x0040 x5 (room_00:74, room_01:60, room_02:65, room_03:54, room_04:78) | `0xD290` | 0x69/9/0 `sEastClockTownEntrance9`[0] L2237 | `loc.east_clock_town` | 9 | (669,100,-846) yaw 0 room 0 start9 mode 0xF | one-way (kickout); door row is the inbound | kickout at 20:30 text STOCK_POT_INN [D0d+D0n+D1d+D1n+D2d+D2n]; skipped with Room Key (z_en_time_tag.c:319) | not_run |
| `portal.stock_pot_inn.east_clock_town.upper` | door | `loc.stock_pot_inn` | `Z2_YADOYAExitList_000190`[1]; surf 8 (exit+1=2); polys 344,345; TA#4 EN_DOOR 60u | `0xD2A0` | 0x69/10/0 `sEastClockTownEntrance10`[0] L2241 | `loc.east_clock_town` | 10 | (477,280,-1270) yaw 0 room 0 start10 mode 0xF | `portal.east_clock_town.stock_pot_inn.upper` (inferred: 106u/50u) | EN_DOOR SCHEDULE/INN_UPPER_ENTRANCE; open hours UNKNOWN | not_run |
| `portal.mayors_residence.east_clock_town` | door | `loc.mayors_residence` | `Z2_SONCHONOIEExitList_000104`[0]; surf 2 (exit+1=1); polys 60,61; TA#3 EN_DOOR 75u | `0xD270` | 0x69/7/0 `sEastClockTownEntrance7`[0] L2229 | `loc.east_clock_town` | 7 | (1099,200,-1992) yaw 0 room 0 start7 mode 0xF | `portal.east_clock_town.mayors_residence` (inferred: 116u/48u) | EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MAIN_ENTRANCE; open hours UNKNOWN | not_run |
| `portal.mayors_residence.east_clock_town.kickout` | special | `loc.mayors_residence` | `Z2_SONCHONOIEExitList_000104`[0]; EN_TIME_TAG 0x00C0 x4 (room_00:53, room_01:49, room_02:52, room_03:42) | `0xD270` | 0x69/7/0 `sEastClockTownEntrance7`[0] L2229 | `loc.east_clock_town` | 7 | (1099,200,-1992) yaw 0 room 0 start7 mode 0xF | one-way (kickout); door row is the inbound | kickout at 20:00 text GENERIC [D1d+D1n+D2d+D2n] | not_run |
| `portal.milk_bar.east_clock_town` | door | `loc.milk_bar` | `Z2_MILK_BARExitList_0000CC`[0]; surf 5 (exit+1=1); polys 126,127; TA#0 EN_DOOR 60u | `0xD2B0` | 0x69/11/0 `sEastClockTownEntrance11`[0] L2245 | `loc.east_clock_town` | 11 | (1036,100,-902) yaw 0 room 0 start11 mode 0xE | `portal.east_clock_town.milk_bar` (inferred: 110u/49u) | EN_DOOR SCHEDULE/MILK_BAR; open hours UNKNOWN | not_run |
| `portal.milk_bar.east_clock_town.kickout` | special | `loc.milk_bar` | `Z2_MILK_BARExitList_0000CC`[0]; EN_TIME_TAG 0x00C0/0x0120 x2 (room_00:77, room_00:80) | `0xD2B0` | 0x69/11/0 `sEastClockTownEntrance11`[0] L2245 | `loc.east_clock_town` | 11 | (1036,100,-902) yaw 0 room 0 start11 mode 0xE | one-way (kickout); door row is the inbound | kickout at 05:00 text GENERIC [D0d+D0n+D1d+D1n+D2d+D2n], 21:00 text MILK_BAR [all half-days] | not_run |
| `portal.honey_darling.east_clock_town` | door | `loc.honey_darling` | `Z2_BOWLINGExitList_0000AC`[0]; surf 3 (exit+1=1); polys 91,92; TA#0 EN_DOOR 50u | `0xD260` | 0x69/6/0 `sEastClockTownEntrance6`[0] L2225 | `loc.east_clock_town` | 6 | (1395,100,-274) yaw 270 room 0 start6 mode 0xF | `portal.east_clock_town.honey_darling` (inferred: 92u/45u) | EN_DOOR SCHEDULE/HONEY_DARLING_SHOP; open hours UNKNOWN | not_run |
| `portal.honey_darling.east_clock_town.kickout` | special | `loc.honey_darling` | `Z2_BOWLINGExitList_0000AC`[0]; EN_TIME_TAG 0x0060 x1 (room_00:31) | `0xD260` | 0x69/6/0 `sEastClockTownEntrance6`[0] L2225 | `loc.east_clock_town` | 6 | (1395,100,-274) yaw 270 room 0 start6 mode 0xF | one-way (kickout); door row is the inbound | kickout at 22:00 text HONEY_AND_DARLING [D0d+D0n+D1d+D1n+D2d+D2n] | not_run |
| `portal.treasure_chest_shop.east_clock_town` | door | `loc.treasure_chest_shop` | `Z2_TAKARAYAExitList_0000BC`[0]; surf 6 (exit+1=1); polys 69,70; TA#0 EN_DOOR 40u | `0xD240` | 0x69/4/0 `sEastClockTownEntrance4`[0] L2217 | `loc.east_clock_town` | 4 | (610,100,-173) yaw 180 room 0 start4 mode 0xF | `portal.east_clock_town.treasure_chest_shop` (inferred: 101u/53u) | EN_DOOR SCHEDULE/TREASURE_CHEST_SHOP; open hours UNKNOWN | not_run |
| `portal.treasure_chest_shop.east_clock_town.kickout` | special | `loc.treasure_chest_shop` | `Z2_TAKARAYAExitList_0000BC`[0]; EN_TIME_TAG 0x01E0 x1 (room_00:41) | `0xD240` | 0x69/4/0 `sEastClockTownEntrance4`[0] L2217 | `loc.east_clock_town` | 4 | (610,100,-173) yaw 180 room 0 start4 mode 0xF | one-way (kickout); door row is the inbound | kickout at 22:00 text TREASURE_CHEST_SHOP [D0d+D0n+D1d+D1n+D2d+D2n] | not_run |
| `portal.town_shooting_gallery.east_clock_town` | door | `loc.town_shooting_gallery` | `Z2_SYATEKI_MIZUExitList_0000A4`[0]; surf 4 (exit+1=1); polys 36,37; TA#0 EN_DOOR 33u | `0xD280` | 0x69/8/0 `sEastClockTownEntrance8`[0] L2233 | `loc.east_clock_town` | 8 | (1095,100,257) yaw 235 room 0 start8 mode 0xF | `portal.east_clock_town.town_shooting_gallery` (inferred: 63u/45u) | EN_DOOR SCHEDULE/TOWN_SHOOTING_GALLERY; open hours UNKNOWN | not_run |
| `portal.town_shooting_gallery.east_clock_town.kickout` | special | `loc.town_shooting_gallery` | `Z2_SYATEKI_MIZUExitList_0000A4`[0]; EN_TIME_TAG 0x0180/0x01A0 x2 (room_00:31, room_00:34) | `0xD280` | 0x69/8/0 `sEastClockTownEntrance8`[0] L2233 | `loc.east_clock_town` | 8 | (1095,100,257) yaw 235 room 0 start8 mode 0xF | one-way (kickout); door row is the inbound | kickout at 22:00 text TOWN_SG_1 [D0d+D0n+D1d+D1n+D2d+D2n], 22:00 text TOWN_SG_2 [D3d+D3n+D4d+D4n] | not_run |
| `portal.great_fairy_fountain_town.north_clock_town` | district | `loc.great_fairy_fountain_town` | `Z2_YOUSEI_IZUMIExitList_000168`[0]; surf 5,6 (exit+1=1); polys 44,45,46,47,48,49 | `0xD630` | 0x6B/3/0 `sNorthClockTownEntrance3`[0] L2320 | `loc.north_clock_town` | 3 | (-1033,292,-2008) yaw 90 room 0 start3 mode 0xE | `portal.north_clock_town.great_fairy_fountain_town` (inferred: 260u/173u) | UNKNOWN | not_run |
| `portal.great_fairy_fountain_town.z2_21miturinmae` | external-boundary | `loc.great_fairy_fountain_town` | `Z2_YOUSEI_IZUMIExitList_000168`[1]; surf 14,15 (exit+1=2); polys 450,451,452,453,454,455 | `0x8620` | 0x43/2/0 `sWoodfallEntrance2`[0] L1491 | `ext.z2_21miturinmae` | 2 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.great_fairy_fountain_town.z2_12hakuginmae` | external-boundary | `loc.great_fairy_fountain_town` | `Z2_YOUSEI_IZUMIExitList_000168`[2]; surf 23 (exit+1=3); polys 856,857 | `0xB220` | 0x59/2/0 `sSnowheadEntrance2`[0] L1928 | `ext.z2_12hakuginmae` | 2 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.great_fairy_fountain_town.z2_31misaki` | external-boundary | `loc.great_fairy_fountain_town` | `Z2_YOUSEI_IZUMIExitList_000168`[3]; surf 32 (exit+1=4); polys 1262,1263 | `0x6A50` | 0x35/5/0 `sZoraCapeEntrance5`[0] L1225 | `ext.z2_31misaki` | 5 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.great_fairy_fountain_town.z2_ikana` | external-boundary | `loc.great_fairy_fountain_town` | `Z2_YOUSEI_IZUMIExitList_000168`[4]; surf 41 (exit+1=5); polys 1668,1669 | `0x20B0` | 0x10/11/0 `sIkanaCanyonEntrance11`[0] L243 | `ext.z2_ikana` | 11 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.deku_playground.north_clock_town` | special | `loc.deku_playground` | `Z2_DEKUTESExitList_0000AC`[0]; EN_WARP_TAG 0x83C0 x1 (room_00:96) | `0xD640` | 0x6B/4/0 `sNorthClockTownEntrance4`[0] L2324 | `loc.north_clock_town` | 4 | (-972,292,-1702) yaw 119 room 0 start4 mode 0x4 | `portal.north_clock_town.deku_playground` (inferred: 367u/6u) | invisible tag; xz <= 30 and height <= 10 (z_en_warp_tag.c:96-101) | not_run |
| `portal.astral_observatory.east_clock_town` | district | `loc.astral_observatory` | `Z2_TENMON_DAIExitList_0000F0`[0]; surf 3 (exit+1=1); polys 233,234 | `0xD220` | 0x69/2/0 `sEastClockTownEntrance2`[0] L2209 | `loc.east_clock_town` | 2 | (1762,80,-1793) yaw 270 room 0 start2 mode 0xF | `portal.east_clock_town.astral_observatory.p116` (inferred: 116u/134u) | UNKNOWN | not_run |
| `portal.astral_observatory.z2_00keikoku` | external-boundary | `loc.astral_observatory` | `Z2_TENMON_DAIExitList_0000F0`[1]; surf 24 (exit+1=2); polys 1040,1041; TA#0 EN_DOOR 49u | `0x5490` | 0x2A/9/0 `sTerminaFieldEntrance9`[0] L895 | `ext.z2_00keikoku` | 9 | not inspected (external) | UNKNOWN (external scene not inspected) | EN_DOOR WHOLE_DAY/0x7F; open hours UNKNOWN | not_run |
| `portal.clock_tower_interior.z2_openingdan` | external-boundary | `loc.clock_tower_interior` | `Z2_INSIDETOWERExitList_000128`[0]; surf 12 (exit+1=1); polys 551,552 | `0x2E10` | 0x17/1/0 `sOpeningDungeonEntrance1`[0] L438 | `ext.z2_openingdan` | 1 | not inspected (external) | UNKNOWN (external scene not inspected) | UNKNOWN | not_run |
| `portal.clock_tower_interior.south_clock_town` | district | `loc.clock_tower_interior` | `Z2_INSIDETOWERExitList_000128`[1]; surf 5 (exit+1=2); polys 382,383 | `0xD800` | 0x6C/0/0 `sSouthClockTownEntrance0`[0] L2345 | `loc.south_clock_town` | 0 | (-278,0,-801) yaw 0 room 0 start0 mode 0xE | `portal.south_clock_town.clock_tower_interior` (inferred: 177u/86u) | UNKNOWN | not_run |

`Z2_OKUJOU` has no `EXIT_LIST` in its default header or alt1-alt3, so the rooftop has no exit-list portal. Its one listed time tag (`0x5FFF`, `room_00:74`) is type 2 `TIMETAG_ROOFTOP_OATH`, not a kickout.

### 4.3 Room portals (18 rows, 9 transition actors)

Both sides of each actor are separate directed rows; neither loads a scene. `EN_HOLL` params `0x007F` are type 0 loading planes (`z_en_holl.h:64`).

| Portal ID | Mode | Scene | Evidence | Rooms | Reverse | Conditions | Traversal |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `portal.stock_pot_inn.stock_pot_inn.r1_r0` | room | `loc.stock_pot_inn` | `Z2_YADOYATransitionActorList_0000FC`[0] EN_DOOR SCHEDULE/INN_GRANNYS at (-150,0,-150) yaw 90 | room 1 -> room 0 | `portal.stock_pot_inn.stock_pot_inn.r0_r1` | EN_DOOR SCHEDULE/INN_GRANNYS; hours UNKNOWN | not_run |
| `portal.stock_pot_inn.stock_pot_inn.r0_r1` | room | `loc.stock_pot_inn` | `Z2_YADOYATransitionActorList_0000FC`[0] EN_DOOR SCHEDULE/INN_GRANNYS at (-150,0,-150) yaw 90 | room 0 -> room 1 | `portal.stock_pot_inn.stock_pot_inn.r1_r0` | EN_DOOR SCHEDULE/INN_GRANNYS; hours UNKNOWN | not_run |
| `portal.stock_pot_inn.stock_pot_inn.r2_r0` | room | `loc.stock_pot_inn` | `Z2_YADOYATransitionActorList_0000FC`[1] EN_DOOR SCHEDULE/INN_STAFF_ROOM at (-270,210,-90) yaw 90 | room 2 -> room 0 | `portal.stock_pot_inn.stock_pot_inn.r0_r2` | EN_DOOR SCHEDULE/INN_STAFF_ROOM; hours UNKNOWN | not_run |
| `portal.stock_pot_inn.stock_pot_inn.r0_r2` | room | `loc.stock_pot_inn` | `Z2_YADOYATransitionActorList_0000FC`[1] EN_DOOR SCHEDULE/INN_STAFF_ROOM at (-270,210,-90) yaw 90 | room 0 -> room 2 | `portal.stock_pot_inn.stock_pot_inn.r2_r0` | EN_DOOR SCHEDULE/INN_STAFF_ROOM; hours UNKNOWN | not_run |
| `portal.stock_pot_inn.stock_pot_inn.r3_r0` | room | `loc.stock_pot_inn` | `Z2_YADOYATransitionActorList_0000FC`[2] EN_DOOR SCHEDULE/INN_KNIFE_CHAMBER at (-270,210,240) yaw 90 | room 3 -> room 0 | `portal.stock_pot_inn.stock_pot_inn.r0_r3` | EN_DOOR SCHEDULE/INN_KNIFE_CHAMBER; hours UNKNOWN | not_run |
| `portal.stock_pot_inn.stock_pot_inn.r0_r3` | room | `loc.stock_pot_inn` | `Z2_YADOYATransitionActorList_0000FC`[2] EN_DOOR SCHEDULE/INN_KNIFE_CHAMBER at (-270,210,240) yaw 90 | room 0 -> room 3 | `portal.stock_pot_inn.stock_pot_inn.r3_r0` | EN_DOOR SCHEDULE/INN_KNIFE_CHAMBER; hours UNKNOWN | not_run |
| `portal.stock_pot_inn.stock_pot_inn.r4_r0` | room | `loc.stock_pot_inn` | `Z2_YADOYATransitionActorList_0000FC`[3] EN_DOOR SCHEDULE/INN_LARGE_SUITE at (-270,210,510) yaw 90 | room 4 -> room 0 | `portal.stock_pot_inn.stock_pot_inn.r0_r4` | EN_DOOR SCHEDULE/INN_LARGE_SUITE; hours UNKNOWN | not_run |
| `portal.stock_pot_inn.stock_pot_inn.r0_r4` | room | `loc.stock_pot_inn` | `Z2_YADOYATransitionActorList_0000FC`[3] EN_DOOR SCHEDULE/INN_LARGE_SUITE at (-270,210,510) yaw 90 | room 0 -> room 4 | `portal.stock_pot_inn.stock_pot_inn.r4_r0` | EN_DOOR SCHEDULE/INN_LARGE_SUITE; hours UNKNOWN | not_run |
| `portal.mayors_residence.mayors_residence.r1_r0` | room | `loc.mayors_residence` | `Z2_SONCHONOIETransitionActorList_0000A0`[0] EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MAYOR_DOTOUR at (-165,0,-150) yaw 0 | room 1 -> room 0 | `portal.mayors_residence.mayors_residence.r0_r1` | EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MAYOR_DOTOUR; hours UNKNOWN | not_run |
| `portal.mayors_residence.mayors_residence.r0_r1` | room | `loc.mayors_residence` | `Z2_SONCHONOIETransitionActorList_0000A0`[0] EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MAYOR_DOTOUR at (-165,0,-150) yaw 0 | room 0 -> room 1 | `portal.mayors_residence.mayors_residence.r1_r0` | EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MAYOR_DOTOUR; hours UNKNOWN | not_run |
| `portal.mayors_residence.mayors_residence.r2_r0` | room | `loc.mayors_residence` | `Z2_SONCHONOIETransitionActorList_0000A0`[1] EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MADAME_AROMA at (165,0,-150) yaw 0 | room 2 -> room 0 | `portal.mayors_residence.mayors_residence.r0_r2` | EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MADAME_AROMA; hours UNKNOWN | not_run |
| `portal.mayors_residence.mayors_residence.r0_r2` | room | `loc.mayors_residence` | `Z2_SONCHONOIETransitionActorList_0000A0`[1] EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MADAME_AROMA at (165,0,-150) yaw 0 | room 0 -> room 2 | `portal.mayors_residence.mayors_residence.r2_r0` | EN_DOOR SCHEDULE/MAYORS_RESIDENCE_MADAME_AROMA; hours UNKNOWN | not_run |
| `portal.mayors_residence.mayors_residence.r2_r3` | room | `loc.mayors_residence` | `Z2_SONCHONOIETransitionActorList_0000A0`[2] EN_DOOR SCHEDULE/MAYORS_RESIDENCE_BEDROOM at (345,0,-435) yaw 90 | room 2 -> room 3 | `portal.mayors_residence.mayors_residence.r3_r2` | EN_DOOR SCHEDULE/MAYORS_RESIDENCE_BEDROOM; hours UNKNOWN | not_run |
| `portal.mayors_residence.mayors_residence.r3_r2` | room | `loc.mayors_residence` | `Z2_SONCHONOIETransitionActorList_0000A0`[2] EN_DOOR SCHEDULE/MAYORS_RESIDENCE_BEDROOM at (345,0,-435) yaw 90 | room 3 -> room 2 | `portal.mayors_residence.mayors_residence.r2_r3` | EN_DOOR SCHEDULE/MAYORS_RESIDENCE_BEDROOM; hours UNKNOWN | not_run |
| `portal.astral_observatory.astral_observatory.r1_r0` | room | `loc.astral_observatory` | `Z2_TENMON_DAITransitionActorList_0000B8`[1] EN_HOLL type 0 loading plane at (60,-480,440) yaw 0 | room 1 -> room 0 | `portal.astral_observatory.astral_observatory.r0_r1` | UNKNOWN | not_run |
| `portal.astral_observatory.astral_observatory.r0_r1` | room | `loc.astral_observatory` | `Z2_TENMON_DAITransitionActorList_0000B8`[1] EN_HOLL type 0 loading plane at (60,-480,440) yaw 0 | room 0 -> room 1 | `portal.astral_observatory.astral_observatory.r1_r0` | UNKNOWN | not_run |
| `portal.clock_tower_interior.clock_tower_interior.r1_r0` | room | `loc.clock_tower_interior` | `Z2_INSIDETOWERTransitionActorList_0000F8`[0] EN_HOLL type 0 loading plane at (0,-300,-905) yaw 0 | room 1 -> room 0 | `portal.clock_tower_interior.clock_tower_interior.r0_r1` | UNKNOWN | not_run |
| `portal.clock_tower_interior.clock_tower_interior.r0_r1` | room | `loc.clock_tower_interior` | `Z2_INSIDETOWERTransitionActorList_0000F8`[0] EN_HOLL type 0 loading plane at (0,-300,-905) yaw 0 | room 0 -> room 1 | `portal.clock_tower_interior.clock_tower_interior.r1_r0` | UNKNOWN | not_run |

## 5. Alternate headers (by header index; present in header)

Alternate headers load only when `sceneLayer` is nonzero (cutscene entry, section 1). All share their scene's default collision header. Exit-capable room actors are listed per room-file alternate header of the same index.

| Scene | Header (line) | Spawns / entrances | Exit list (line) | Difference from default | Exit-capable room actors |
| --- | --- | --- | --- | --- | --- |
| Z2_CLOCKTOWER | alt1 `Z2_CLOCKTOWERSet_005590` (2289) | 2 / 2 | `Z2_CLOCKTOWERExitList_00564C` (2328) | same 10 values | none |
| Z2_CLOCKTOWER | alt2 `Z2_CLOCKTOWERSet_005F40` (2588) | 1 / 2 | none | `setupExitList` stays NULL (section 1) | none |
| Z2_CLOCKTOWER | alt3 `Z2_CLOCKTOWERSet_006920` (2857) | 1 / 2 | `Z2_CLOCKTOWERExitList_0069DC` (2896) | same 10 values | none |
| Z2_BACKTOWN | alt1 `Z2_BACKTOWNSet_002270` (911) | 1 / 2 | `Z2_BACKTOWNExitList_00230C` (944) | same 8 values | `DOOR_ANA 0x0304` (`room_00:240`), `EN_BOMJIMB 0x050F/0x0513` (`:296`, `:302`) |
| Z2_ICHIBA | alt1 `Z2_ICHIBASet_003A60` (1507) | 1 / 2 | `Z2_ICHIBAExitList_003B54` (1548) | same 10 values | none |
| Z2_TOWN | alt1 `Z2_TOWNSet_003A00` (1682) | 1 / 2 | `Z2_TOWNExitList_003B14` (1726) | same 14 values | `EN_BOMJIMB 0x0B22/0x0B11` (`room_00:446`, `:448`) |
| Z2_TOWN | alt2 `Z2_TOWNSet_004910` (2296) | 1 / 2 | `Z2_TOWNExitList_004A14` (2338), 10 entries | [2] `0xD220` (East spawn 2) for `0x4C00`; [6] `0xD260` (East spawn 6) for `0x0800`; [7] `0x3A00` for `0x0000`; [8] `0x0800` for `0x3A00`; [9] `0xD880` (South spawn 8) for `0xBC00`; shared collision exit+1 = 11 and 13 index past the list | none |
| Z2_YADOYA | alt1 `Z2_YADOYASet_00A050` (4088) | 1 / 2 | `Z2_YADOYAExitList_00A17C` (4136) | same 4 values | 5 kickout `EN_TIME_TAG 0x0040` (`room_00:201`, `room_01:141`, `room_02:138`, `room_03:124`, `room_04:168`) |
| Z2_MILK_BAR | alt1-alt4 `Set_002D50`, `Set_003460`, `Set_0038B0`, `Set_0041A0` (1140, 1352, 1501, 1762) | 1 / 2 each | none | `setupExitList` stays NULL | alt2/alt3 `EN_ZOV`, `EN_ZOD 0xFE0F` (`room_00:197`, `:203`, `:271`, `:275`); alt4 `EN_KGY 0xFE1F` (`:372`); exit macros not decoded |
| Z2_YOUSEI_IZUMI | alt1 `Z2_YOUSEI_IZUMISet_01BE20` (10228) | 1 / 2 | `Z2_YOUSEI_IZUMIExitList_01BED4` (10264) | same 6 values | none |
| Z2_OKUJOU | alt1-alt3 `Set_00AC30`, `Set_00B5A0`, `Set_012CC0` (3866, 4108, 6765) | 1 / 2, 3 / 4, 1 / 2 | none | default has none either | none |

The other 11 scenes declare no alternate header.

## 6. Graph checks (FULL_CITY_PLAN section 3.3)

Run over the generated rows; no runtime involved.

| Check | Result |
| --- | --- |
| IDs unique | Pass: 95 IDs (77 exit-list portals, 18 room portals), 0 duplicates |
| Destination exists or is unavailable | 67 exit-list portals resolve to one of the 22 locations with an in-range destination spawn; 10 resolve to 6 external scenes (`ext.z2_00keikoku` x5, `ext.z2_openingdan`, `ext.z2_21miturinmae`, `ext.z2_12hakuginmae`, `ext.z2_31misaki`, `ext.z2_ikana`), all unavailable |
| Enabled endpoints resolve | Not applicable: no endpoint is enabled |
| Reciprocal pairs | Found: 25 mutual exit-list pairs (50 rows), every reverse arrival within 367 u of its trigger; 9 room-portal pairs by construction. Missing: `portal.south_clock_town.clock_tower_rooftop` (no rooftop exit list). The `.p169` row is a wall face that the player exit check never reads (REF-07, integrator-verified at `z_player.c:6783` and `:12572`), so it is not a missing pair. One-way by design: 12 kickout rows, 3 Bombers rows. Unknown: 10 external rows |
| No arrival inside the departure trigger | Pass at source level: none of the 67 city arrival points lies inside any exit triangle of its destination (xz point-in-triangle, abs(dy) <= 60). Not a controller test |
| All five exteriors reachable from South | Pass: South reaches North, West (upper and lower), East (upper and lower) and Laundry Pool directly by `district` rows. Walk modes (`district`, `door`, `room`) reach 30 of 36 scene-room nodes |
| Isolated rooms | Great Fairy rooms 1-4: no edge from the 22 scenes reaches them (their exits lead only to external scenes). Curiosity Shop room 1 has no internal link to room 0; it is reached only through the Laundry Pool door. Inn rooms 1-4 and Mayor's rooms 1-3 are reached only through room doors |
| Special-only | Deku Playground room 0 (only by the North `DOOR_ANA` grotto) and Clock Tower rooftop room 0 (only by South exit+1 = 9). With `special` rows 32 of 36 nodes are reached |
| Teleport-only spawns | No city exit lands on: South 1, 8, 9 (start mode 6 `OWL`), 10, 11; North 0, 6, 7; West 0, 9; East 0; Trading Post 1; Bomb Shop 1; Curiosity Shop 2 (mode 0xC `TELESCOPE`), 3; Post Office 1; Swordsman's School 1; Lottery 1; Inn 2-5; Mayor's 1; Milk Bar 1; Honey and Darling 1; Treasure Chest Shop 1; Shooting Gallery 1; Great Fairy 1-9; Deku Playground 1; Observatory 1-3; Tower interior 0, 2-7; Rooftop 1. Reached by external, owl, cutscene or hardcoded entrances: not inspected. The debug map select (`src/overlays/gamestates/ovl_select/z_select.c`) is the developer teleport and is excluded |

Independent door check (present in header): all 28 venue doors are `ENDOOR_TYPE_SCHEDULE`, and on both sides each door's `ENDOOR_SCH_TYPE_*` names the decoded destination venue (for example West TA#4 `TRADING_POST` -> `0x6200` -> `Z2_8ITEMSHOP`).

## 7. Unresolved edges and the next source question

| # | Item | Known | Next source question |
| --- | --- | --- | --- |
| U1 | South to rooftop reachability | exit+1 = 9 stair polys at the tower top; `0x2C00` -> rooftop spawn 0 | Which actor or state makes these polygons reachable? Read the type-11 staircase actor in `Z2_CLOCKTOWER_room_00.c` (default list) and its spawn/draw condition in `ovl_Obj_Tokeidai` / `ovl_Obj_Tokei_Step` |
| U2 | Rooftop return | No exit list in any `Z2_OKUJOU` header | Which code moves the player off the rooftop? Read the hardcoded `ENTRANCE(CLOCK_TOWER_ROOFTOP/SOUTH_CLOCK_TOWN, n)` sites (`ovl_Dm_Stk/z_dm_stk.c:1093`, `ovl_En_Fall/z_en_fall.c:346`) and `Z2_OKUJOU` cutscene destinations |
| U3 | East wall poly 169 -> observatory | RESOLVED 2026-11-08 by REF-07: polys 164-169 are a six-sided wooden post under the Inn upper-deck walkway at about (592, 100..245, -634); no actor there. The player only passes floor polygons to the exit check (`z_player.c:6783`, `:12572`), so the wall's exit index is dead data | None; the `.p169` row is retained as a labeled oddity |
| U4 | Unreferenced `0xD650` (North spawn 5, the Bombers arrival) | South [9], Laundry [1], Post Office [1], Inn [2], Milk Bar [1], Treasure Chest Shop [1] | Which code indexes `setupExitList` with these indices in these scenes, including cutscene `nextEntrance` commands? |
| U5 | `0x0000` entries | Trailing in 11 lists; East [7] is a real use (Mayor's) | Exit lists have no count; confirm each list's true length from the binary header layout or ZAPD XML before treating trailing `0x0000` as Mayor's Residence spawn 0 |
| U6 | North [6] `0xD670` | Equals `ENTRANCE(NORTH_CLOCK_TOWN, 7)`, hardcoded in `ovl_En_Baba/z_en_baba.c:514` (and 5 more) and `ovl_En_Suttari/z_en_suttari.c:992`, `:1265` | Does any North actor index exit [6]? Read the `Z2_BACKTOWN_room_00.c` lists for those actors |
| U7 | Self entries Inn [3] `0xBC20` (spawn 2, room 1), Mayor's [1] `0x0010` (spawn 1, room 1) | No consumer found | Which actor or cutscene in `Z2_YADOYA` / `Z2_SONCHONOIE` (all headers) uses them? |
| U8 | Orphan `EN_DOOR` in South, TA#0 at (1114,100,274), `WHOLE_DAY/0x7F` | Nearest South exit polygon 583 u away; East's shooting-gallery door sits at the identical point | Is (1114,100,274) inside South's walkable collision, or overlap with East geometry? |
| U9 | Door open hours | 28 schedule doors | Decode `src/overlays/actors/ovl_En_Door/scheduleScripts.schl` per `ENDOOR_SCH_TYPE` |
| U10 | External endpoints | 10 rows, 6 scenes | Parse those scenes' default headers for arrival pose and the return exit to each city spawn |
| U11 | Bombers trigger | `EN_BOMJIMB` rows in North, West, East | Which state reaches `func_80C02CA4` (`z_en_bomjimb.c:859`)? |
| U12 | Hardcoded city entrances | About 80 `ENTRANCE(<city scene>, n)` call sites, e.g. observatory (`ovl_En_Bji_01/z_en_bji_01.c:391-397`, `z_player.c:8631`), Curiosity Shop (`ovl_Obj_Nozoki/z_obj_nozoki.c:444`, `z_player.c:8636`), Deku Playground (`ovl_En_Lift_Nuts/z_en_lift_nuts.c:905`) | Classify each site (actor, condition, destination spawn) as a `special` row; this likely covers most teleport-only spawns |
| U13 | East alt2 out-of-range exits | exit+1 = 11, 13 exceed the 10-entry alt2 list | Which cutscene index selects East alt2, and can the player touch those surfaces in it? |
| U14 | Milk Bar alt-header actors | `EN_ZOV`, `EN_ZOD`, `EN_KGY` with no exit list in those headers | Decode their exit-index macros and confirm they never index the NULL list |

## 8. Not run, not inspected, reproduction

- Not run: Unity, any player or executable, Blender, ZAPD; no controller, load or render test. `not_run` on every row is literal.
- Not inspected: external scene files; door schedules; cutscene destination commands; hardcoded `ENTRANCE()` call sites (counted only); room geometry beyond exit polygons; actors outside the exit-capable set in section 1; transition-type names; ZAPD XML list lengths.
- Method: a scratch parser read the 22 scene files, their room files and `z_scene_table.c`, decoded every exit value through the three-level table, clustered exit polygons, attached doors within 200 u, paired reverses (threshold 400 u) and ran the checks above. Scripts live outside the repository (session scratchpad `ref02\parse.py`, `gen.py`, `tables.py`) and wrote nothing under `local-reference/` or `Unity/`.
