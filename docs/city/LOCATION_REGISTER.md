# Clock Town location register (REF-01)

Produced 2026-11-08 for task REF-01 at base commit `d0bc29f3ccf38e53ed7f92559cb2d62c3ebd1e1b`. Source: local pinned zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639`, N64 US, using the ZAPD output already present under `local-reference/mm-decomp/extracted/n64-us/assets/scenes/`. This register is an inventory of scenes, rooms and header counts only. It does not establish actor visibility, schedules, door mapping, collision or traversal. Portal endpoints belong to REF-02 (`PORTAL_REGISTER.md`).

Paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `tools/`. "Extracted C" means `extracted/n64-us/assets/scenes/<SCENE>/<file>.c`.

## 1. Evidence labels and decoding rules

| Label | Meaning |
| --- | --- |
| XML | Declared in `assets/xml/scenes/<SCENE>/<SCENE>.xml` |
| HDR | Present as a command argument in a scene/room header in the extracted C (for example the `n` of `SCENE_CMD_ACTOR_LIST(n, ...)`) |
| C | Decoded from extracted C or engine source by a rule cited below |
| ANN | Decomp-author annotation (comment, map-select label, symbol name), not original game data |
| INF | Inferred; not proven by a single source line |
| NI | Not inspected |

Rules used (all C):

- **Three namespaces.** Scene-table index is the `/* 0xNN */` comment in `include/tables/scene_table.h`. Entrance-table index is the row of `sSceneEntranceTable` in `src/code/z_scene_table.c:2572-2681`. Exit-list values are entrance values. They are different numbers for the same scene (South: scene 0x6F, entrance table 0x6C).
- **Entrance value.** `Entrance_GetTableEntry` (`src/code/z_scene_table.c:2688-2694`): entrance-table index = `value >> 9`, entrance number = `(value >> 4) & 0x1F`, layer = `value & 0xF`. The `EntranceTableEntry` (`include/z64scene.h:397-401`) gives `spawnNum`; the scene's `EntranceEntry {spawn, room}` list (`include/z64scene.h:392-395`) indexed by that number gives the spawn-list index and the **room**. Engine use: `src/code/z_scene.c:160` (spawn), `src/code/z_room.c:549` (room). Spawn `ActorEntry`s carry no room index themselves.
- **Alternate headers.** `Scene_CommandAltHeaderList` (`src/code/z_scene.c:464-474`) runs `altHeaders[sceneLayer - 1]`; a NULL entry keeps the default header. `sceneLayer = (cutsceneIndex & 0xF) + 1` only for cutscene entries `>= 0xFFF0` or non-normal game modes, else 0 (`src/code/z_play.c:2210-2219`). Room files go through the same command executor (`src/code/z_room.c:613`), so room alternate k pairs with scene alternate k. Day/half-day variation is a separate per-actor mask (see `docs/ROM_REFERENCE.md`), not an alternate header.
- **List lengths.** `SCENE_CMD_ENTRANCE_LIST` and `SCENE_CMD_EXIT_LIST` carry no count (`include/z64scene.h:798`, `:839`). Their lengths below are what ZAPD emitted. A trailing exit `0x0000` or a trailing entrance `{0x00, 0x00}` past the spawn count is consistent with 4-byte alignment padding (INF). `0x0000` also decodes to Mayor's Residence entrance 0, because that scene owns entrance-table index 0x00.
- **Object/actor counts** are the HDR `n` values. `tools/reference/city_index.py` checked each against the number of emitted array entries: 0 mismatches across 60 room headers.

## 2. Scene identity

Display names are the comment line above each `DEFINE_SCENE` row (ANN). Columns 4 to 9 are HDR values from that row (`include/tables/scene_table.h`, line given). Draw config drops the `SCENE_DRAW_CFG_` prefix; restrictions and cycle flags drop `RESTRICTIONS_` / `PERSISTENT_CYCLE_FLAGS_`. Field meanings of `RESTRICTIONS_SET` and the cycle-flag words are NI (`z_parameter.c` per the header comment).

| Location ID | Display name | Scene / enum | Scene idx (line) | Entr. idx | Title text | Draw | Restrictions | Persistent cycle flags | Plan packet |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `loc.south_clock_town` (alias `scene.south_clock_town`, `sct.*`) | South Clock Town | `Z2_CLOCKTOWER` / `SCENE_CLOCKTOWER` | 0x6F (227) | 0x6C | 0x104 | MAT_ANIM | NONE | SET((1 << 20), 0, 0, (1 << 10)) | 5.1 |
| `loc.north_clock_town` | North Clock Town | `Z2_BACKTOWN` / `SCENE_BACKTOWN` | 0x6E (225) | 0x6B | 0x103 | DEFAULT | NONE | SET(0, 0, 0, (1 << 10)) | 5.2 |
| `loc.west_clock_town` | West Clock Town | `Z2_ICHIBA` / `SCENE_ICHIBA` | 0x6D (223) | 0x6A | 0x102 | DEFAULT | NONE | NONE | 5.3 |
| `loc.east_clock_town` | East Clock Town | `Z2_TOWN` / `SCENE_TOWN` | 0x6C (221) | 0x69 | 0x101 | DEFAULT | NONE | SET(1, 0, 0, 0) | 5.4 |
| `loc.laundry_pool` | Laundry Pool | `Z2_ALLEY` / `SCENE_ALLEY` | 0x70 (229) | 0x6D | 0x105 | MAT_ANIM | NONE | SET(0, 0, 1, 0) | 5.5 |
| `loc.trading_post` | Trading Post | `Z2_8ITEMSHOP` / `SCENE_8ITEMSHOP` | 0x34 (110) | 0x31 | 0x10F | MAT_ANIM | SET(0,0,0,0,0,3,0,0,3,0,0,1) | NONE | INT-01 |
| `loc.bomb_shop` | Bomb Shop | `Z2_BOMYA` / `SCENE_BOMYA` | 0x68 (213) | 0x65 | 0x10D | DEFAULT | INDOORS | NONE | INT-02 |
| `loc.curiosity_shop` | Curiosity Shop | `Z2_AYASHIISHOP` / `SCENE_AYASHIISHOP` | 0x0D (35) | 0x07 | 0x10E | MAT_ANIM | INDOORS | NONE | INT-03 |
| `loc.post_office` | Post Office | `Z2_POSTHOUSE` / `SCENE_POSTHOUSE` | 0x2E (99) | 0x2B | 0x111 | DEFAULT | SET(0,0,0,0,0,3,0,0,3,0,0,1) | SET(3, 0, 0, 0) | INT-04 |
| `loc.swordsmans_school` | Swordsman's School | `Z2_DOUJOU` / `SCENE_DOUJOU` | 0x54 (173) | 0x51 | 0x110 | DEFAULT | SET(0,0,0,0,0,3,0,0,3,0,0,1) | NONE | INT-05 |
| `loc.lottery_shop` | Lottery Shop | `Z2_TAKARAKUJI` / `SCENE_TAKARAKUJI` | 0x39 (120) | 0x36 | 0x112 | DEFAULT | SET(0,0,0,0,0,3,0,0,3,0,0,1) | NONE | INT-06 |
| `loc.stock_pot_inn` | Stock Pot Inn | `Z2_YADOYA` / `SCENE_YADOYA` | 0x61 (199) | 0x5E | 0x10A | MAT_ANIM | INDOORS | NONE | INT-07 |
| `loc.mayors_residence` | The Mayor's Residence | `Z2_SONCHONOIE` / `SCENE_SONCHONOIE` | 0x12 (43) | 0x00 | 0x10B | DEFAULT | INDOORS | NONE | INT-08 |
| `loc.milk_bar` | Milk Bar | `Z2_MILK_BAR` / `SCENE_MILK_BAR` | 0x15 (49) | 0x12 | 0x10C | MAT_ANIM | INDOORS | NONE | INT-09 |
| `loc.honey_darling` | Honey & Darling's Shop | `Z2_BOWLING` / `SCENE_BOWLING` | 0x11 (41) | 0x04 | 0x108 | MAT_ANIM | INDOORS | NONE | INT-10 |
| `loc.treasure_chest_shop` | Treasure Chest Shop | `Z2_TAKARAYA` / `SCENE_TAKARAYA` | 0x17 (53) | 0x14 | 0x109 | DEFAULT | INDOORS | SET(2, 0, 0, 0) | INT-11 |
| `loc.town_shooting_gallery` | Town Shooting Gallery | `Z2_SYATEKI_MIZU` / `SCENE_SYATEKI_MIZU` | 0x20 (71) | 0x1D | 0x107 | MAT_ANIM | INDOORS | NONE | INT-12 |
| `loc.great_fairy_fountain_town` | Fairy's Fountain (shared scene; town = room_00, section 4.2) | `Z2_YOUSEI_IZUMI` / `SCENE_YOUSEI_IZUMI` | 0x26 (83) | 0x23 | 0x13E | MAT_ANIM | NONE | SET((1 << 10), 0, 0, 0) | INT-13 |
| `loc.deku_playground` | Deku Scrub Playground | `Z2_DEKUTES` / `SCENE_DEKUTES` | 0x1E (67) | 0x1B | 0 | MAT_ANIM | SET(0,1,0,0,0,3,0,0,0,0,0,1) | NONE | INT-14 |
| `loc.astral_observatory` | Astral Observatory | `Z2_TENMON_DAI` / `SCENE_TENMON_DAI` | 0x29 (89) | 0x26 | 0x114 | MAT_ANIM | SET(0,0,0,0,0,0,0,3,3,0,0,0) | NONE | INT-15 |
| `loc.clock_tower_interior` | Clock Tower Interior | `Z2_INSIDETOWER` / `SCENE_INSIDETOWER` | 0x63 (203) | 0x60 | 0 | MAT_ANIM | SET(0,0,0,0,3,3,3,3,3,0,0,0) | SET(1 << 0, 0, 0, 0) | INT-16 |
| `loc.clock_tower_rooftop` | Clock Tower Rooftop | `Z2_OKUJOU` / `SCENE_OKUJOU` | 0x19 (57) | 0x16 | 0 | DEFAULT | SET(0,0,0,0,0,3,3,3,3,3,0,0) | NONE | INT-17 |

Entrance-table rows are in `src/code/z_scene_table.c:2572-2681`; each scene's `s*EntranceTable[]` pointer array starts at line: Mayor 37, Honey 76, Curiosity 160, Milk Bar 359, Treasure 384, Rooftop 426, Deku 532, Shooting 553, Fountain 739, Observatory 773, Post 934, Trading 1036, Lottery 1258, Swordsman 1801, Inn 2026, Interior 2067, Bomb 2132, East 2252, West 2300, North 2339, South 2392, Laundry 2410.

## 3. Rooms, headers and counts

Room IDs are `<location ID>.room_NN`, one per XML room file (XML). In every scene the `SCENE_CMD_ROOM_LIST` count (HDR) equals the XML room-file count, in the default and every alternate header. "Alt" = alternate headers in the scene's `AlternateHeaders` array; no city scene or room has a NULL entry (24 arrays checked). Each room has the same number of alternates as its scene. Objects/actors are `objects/actors` per header (d = default, a1.. = alternates). Spawns and exits are per scene header in the same order; "none" = no `SCENE_CMD_EXIT_LIST` in that header (`play->setupExitList` starts NULL, `src/code/z_play.c:1574`). Entr. = scene `EntranceEntry` count (ZAPD length, default header) / entrances declared in `z_scene_table.c`. Transition actors are identical in every header of a scene. "U-nn" refers to section 6.

| Location ID | XML room files | Alt | Objects/actors per room header (HDR) | Trans. | Spawns | Exits (ZAPD) | Entr. | Unknowns |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `loc.south_clock_town` | room_00 | 3 | r00 d 23/50, a1 7/12, a2 5/9, a3 19/29 | 1 | 11/2/1/1 | 10/10/none/10 | 12 (last pad?) / 11 | U-01, U-03, U-05, U-07 |
| `loc.north_clock_town` | room_00 | 1 | r00 d 19/33, a1 19/32 | 0 (no command) | 8/1 | 8/8 (last 0x0000) | 8 / 8 | U-01, U-02, U-03, U-05 |
| `loc.west_clock_town` | room_00 | 1 | r00 d 18/40, a1 9/22 | 6 | 9/1 | 10/10 | 10 / 10 | U-01, U-05 |
| `loc.east_clock_town` | room_00 | 2 | r00 d 25/64, a1 17/37, a2 11/12 | 7 | 12/1/1 | 14/14/10 | 12 / 13 | U-01, U-02, U-04, U-05 |
| `loc.laundry_pool` | room_00 | 0 | r00 d 14/28 | 1 | 2 | 4 (last 0x0000) | 2 / 3 | U-01, U-04 |
| `loc.trading_post` | room_00 | 0 | r00 d 17/15 | 1 | 1 | 2 (last 0x0000) | 2 / 2 | U-02 |
| `loc.bomb_shop` | room_00 | 0 | r00 d 9/8 | 1 | 1 | 2 (last 0x0000) | 2 / 2 | U-02 |
| `loc.curiosity_shop` | room_00, room_01 | 0 | r00 d 13/6; r01 d 4/7 | 2 | 4 | 2 | 4 / 4 | U-08 |
| `loc.post_office` | room_00 | 0 | r00 d 4/12 | 1 | 1 | 2 | 2 / 1 | U-03 |
| `loc.swordsmans_school` | room_00 | 0 | r00 d 7/13 | 1 | 1 | 2 (last 0x0000) | 2 / 1 | U-02 |
| `loc.lottery_shop` | room_00 | 0 | r00 d 4/6 | 1 | 1 | 2 (last 0x0000) | 2 / 1 | U-02 |
| `loc.stock_pot_inn` | room_00 to room_04 | 1 | r00 d 18/26, a1 9/21; r01 d 16/8, a1 8/9; r02 d 18/12, a1 9/7; r03 d 16/7, a1 9/6; r04 d 16/14, a1 8/12 | 6 | 6/1 | 4/4 | 6 / 6 | U-05, U-10 |
| `loc.mayors_residence` | room_00 to room_03 | 0 | r00 d 12/9; r01 d 11/9; r02 d 11/6; r03 d 7/4 | 4 | 2 | 2 | 2 / 3 | U-04, U-11 |
| `loc.milk_bar` | room_00 | 4 | r00 d 14/18, a1 4/4, a2 9/13, a3 11/9, a4 10/18 | 1 | 1/1/1/1/1 | 2/none/none/none/none | 2 / 1 | U-03, U-05, U-07 |
| `loc.honey_darling` | room_00 | 0 | r00 d 3/5 | 1 | 1 | 2 (last 0x0000) | 2 / 1 | U-02 |
| `loc.treasure_chest_shop` | room_00 | 0 | r00 d 4/5 | 1 | 2 | 2 | 2 / 2 | U-03 |
| `loc.town_shooting_gallery` | room_00 | 0 | r00 d 3/15 | 1 | 1 | 2 (last 0x0000) | 2 / 2 | U-02 |
| `loc.great_fairy_fountain_town` | room_00 to room_04 | 1 | r00 d 2/5, a1 2/5; r01, r02, r03 d 1/3, a1 1/0; r04 d 2/4, a1 2/0 | 0 (no command) | 10/1 | 6/6 (last 0x0000) | 10 / 10 | U-02, U-06 |
| `loc.deku_playground` | room_00 | 0 | r00 d 4/27 | 0 (no command) | 2 | 2 (last 0x0000) | 2 / 2 | U-02 |
| `loc.astral_observatory` | room_00, room_01 | 0 | r00 d 15/24; r01 d 15/10 | 2 | 3 | 2 | 4 (last pad?) / 3 | U-09 |
| `loc.clock_tower_interior` | room_00, room_01 | 0 | r00 d 6/12; r01 d 3/2 | 1 | 7 | 2 | 8 (last pad?) / 7 | U-12 |
| `loc.clock_tower_rooftop` | room_00 | 3 | r00 d 6/17, a1 8/4, a2 8/12, a3 6/13 | 0 (no command) | 2/1/3/1 | none in any header | 2 (alt2: 4) / 3 | U-04, U-05, U-07 |

Cross-check (C): for every scene with alternates, the first entrance in `z_scene_table.c` has exactly 1 + alt layers (South 4, North 2, West 2, East 3, Inn 2, Milk Bar 5, Fountain 2, Rooftop 4), matching the header count. Every scene uses one collision header symbol in all of its headers (HDR); collision content is NI.

Totals: 22 scenes, 36 room files (31 if fountain rooms 01 to 04 are excluded as non-city, section 4.2), 16 alternate scene headers, 0 NULL entries, 60 room headers, 530 default-header actor entries (816 over all headers), 88 default-header spawns, 39 transition actors.

## 4. Multi-room and shared scenes

"Lands in" is C: entrance number -> `z_scene_table.c` spawnNum (equal to the entrance number at layer 0 for every scene below) -> scene `EntranceEntry {spawn, room}` (default header). Positions are the spawn `ActorEntry` in N64 units. "Reached by" lists exit values found in other city scenes' default exit lists (C) and engine/actor call sites (C plus ANN where a comment is quoted). Which polygon or door uses each exit index is NI (U-01). Transition actors are written front room / back room (`include/z64scene.h:369-378`).

### 4.1 Stock Pot Inn, `Z2_YADOYA` (5 rooms)

| Entrance | Spawn (pos) | Lands in | Reached by |
| --- | --- | --- | --- |
| 0 | 0 (149, 0, 303) | `loc.stock_pot_inn.room_00` | East exit `0xBC00` |
| 1 | 1 (-63, 210, -46) | room_00 | East exit `0xBC10` |
| 2 | 2 (-337, 0, -135) | room_01 | Inn's own exit index 3 `0xBC20`; `ovl_En_Nb/z_en_nb.c:475` |
| 3 | 3 (-254, 0, 227) | room_00 | `ovl_En_An/z_en_an.c:1597` |
| 4 | 4 (-450, 210, 10) | room_02 | `ovl_Dm_Tag/z_dm_tag.c:202` |
| 5 | 5 (-345, 210, 129) | room_03 | `ovl_Dm_Tag/z_dm_tag.c:183` |

Transition actors (`Z2_YADOYA.c:59-66`, all `EN_DOOR`): 1/0, 2/0, 3/0, 4/0, plus two 0/0 doors at (-60, 210, 0) and (180, 0, 360). Rooms 1 to 4 each join room_00; room_04 has no spawn and is reached only by its door (C). The two 0/0 doors sit next to spawns 1 and 0, so they are probably the upper and lower street doors (INF). Inn exits: `0xD290` East e9, `0xD2A0` East e10, `0xD650` North e5, `0xBC20` Inn e2. Alt1 (`Z2_YADOYASet_00A050`, line 4088) has 1 spawn at (300, 15, 145) in room_00 and is selected by `sStockPotInnEntrance0` layer 1.

### 4.2 Fairy's Fountain, `Z2_YOUSEI_IZUMI` (5 rooms): the Clock Town room is room_00

| Entrance | Spawn (pos) | Lands in | Reached by / label |
| --- | --- | --- | --- |
| 0 | 0 (2400, 20, 360) | `loc.great_fairy_fountain_town.room_00` | North exit `0x4600` (default and alt1); `z_demo.c:703`; map select "Fairy Fountain - Clock Town" (ANN, `ovl_select/z_select.c:429`) |
| 1 to 4 | 1 to 4 (x = 1200, 0, -1200, -2400) | room_01 to room_04 | Map select labels Woodfall, Snowhead, Great Bay Coast, Ikana Canyon (ANN, `z_select.c:432-441`); no city scene exits here |
| 5 to 9 | 5 to 9 (same x, y -15, z -454) | room_00 to room_04 | No `ENTRANCE(FAIRY_FOUNTAIN, 5..9)` literal in `src/` (U-06) |

Source chain for the town room (C): `Z2_BACKTOWN.c:72` exit index 3 = `0x4600` -> entrance table 0x23, entrance 0, layer 0 -> `sFairyFountainEntrance0[0]` = spawnNum 0 (`z_scene_table.c:698-701`) -> `Z2_YOUSEI_IZUMIEntranceList0x000154[0]` = spawn 0, room 0 (`Z2_YOUSEI_IZUMI.c:72`). Reverse (C): fountain exit index 0 is `0xD630` = North entrance 3 (`Z2_YOUSEI_IZUMI.c:85`). The other exits (`0x8620`, `0xB220`, `0x6A50`, `0x20B0`) decode to Woodfall, Snowhead, Zora Cape and Ikana Canyon entrance tables. Not settled: that room_00's exit polygon uses exit index 0 (collision NI), and which Great Fairy `BG_DY_YOSEIZO` draws in each room (params NI). In alt1, which the credits cutscene selects (`z_demo.c:701-704`, cutscene index `0xFFF0` -> layer 1), only room_00 has actors (5); rooms 01 to 04 have 0 (HDR). The scene has no transition actor command, so no transition actor links the five rooms (C).

### 4.3 Astral Observatory, `Z2_TENMON_DAI` (2 rooms)

| Entrance | Spawn (pos) | Lands in | Reached by / label |
| --- | --- | --- | --- |
| 0 | 0 (602, -472, 4125) | `loc.astral_observatory.room_00` | East exit `0x4C00`; comment "ECT entrance" (ANN, `ovl_En_Bji_01/z_en_bji_01.c:391`) |
| 1 | 1 (-6, -129, 133) | room_01 | Comment "TF door entrance" (ANN, `z_en_bji_01.c:392`); no city exit |
| 2 | 2 (107, -84, -209) | room_01 | Comment "Telescope entrance" (ANN, `z_en_bji_01.c:397`); set as the B-button return entrance when the scene is `SCENE_00KEIKOKU`, `ovl_player_actor/z_player.c:8630-8631` |

Transitions (`Z2_TENMON_DAI.c:43-46`): `EN_DOOR` 1/1 at (-7, -129, 188), next to spawn 1; `EN_HOLL` 1/0 at (60, -480, 440) joining the two rooms. Exits: `0xD220` East e2, `0x5490` Termina Field e9. room_00 is the East-side arrival room, so it is the plan's "underground route" (INF from the arrival plus actor names such as `BG_ASTR_BOMBWALL` and `EN_WATER_EFFECT` x5). room_01 holds `EN_BJI_01` and `OBJ_MOON_STONE` (HDR). No separate sewer scene exists among the city scenes (C).

### 4.4 Curiosity Shop, `Z2_AYASHIISHOP` (2 rooms)

| Entrance | Spawn (pos) | Lands in | Reached by / label |
| --- | --- | --- | --- |
| 0 | 0 (1, 0, 227) | `loc.curiosity_shop.room_00` | West exit `0x0E00` |
| 1 | 1 (-262, -96, -619) | room_01 | Laundry exit `0x0E10`; map select "Maniac Mart - Rear Entrance" (ANN, `z_select.c:357`) |
| 2 | 2 (-75, 82, -170) | room_00 | `ovl_Obj_Nozoki/z_obj_nozoki.c:444`; map select "Peephole" (ANN, `z_select.c:360`) |
| 3 | 3 (-95, 30, -311) | room_01 | Peephole exit return, `z_player.c:8635-8636` |

Transitions (`Z2_AYASHIISHOP.c:47-50`): `EN_DOOR` 0/0 at (0, 0, 252) and `EN_DOOR` 1/1 at (-264, -96, -612). **No transition actor joins room_00 and room_01** (C). Exits: `0xD440` West e4, `0xDA10` Laundry e1. room_00 is the West-side shop and room_01 is the Laundry-side back room. Entrances 2 and 3 switch between them for the peephole sequence (C for the call sites; that this is a view rather than a walkable route is INF, U-08). `OBJ_NOZOKI` is in room_01 (HDR).

### 4.5 Multi-room scenes the plan did not size

- **Mayor's Residence, `Z2_SONCHONOIE`, 4 rooms.** Entrance 0 -> spawn 0 (3, 0, 304), room_00; entrance 1 -> spawn 1 (-165, 0, -289), room_01 (also reached by the scene's own exit `0x0010` and `ovl_En_Dt/z_en_dt.c:530`). Doors (`Z2_SONCHONOIE.c:43-48`): 1/0, 2/0, 2/3, 0/0. room_03 is reachable only through room_02 (C). East's default exit index 7 is `0x0000`, mid-list, so it is probably the East door to entrance 0 (INF). Entrance 2 is declared (`z_scene_table.c:33`) but has no `EntranceEntry`: the list at 0x100 ends where the exit list starts at 0x104 (U-04).
- **Clock Tower Interior, `Z2_INSIDETOWER`, 2 rooms.** Entrance 0 -> spawn 0 (11, -296, -2317), room_01; entrances 1 to 6 -> spawns 1 to 6, room_00. South exit `0xC010` = entrance 1, room_00. `EN_HOLL` 1/0 at (0, -300, -905) joins them. Exits: `0x2E10` -> `Z2_OPENINGDAN` e1 (scene-table comment "Before Clock Town", `scene_table.h:58`, not a city scene) and `0xD800` -> South e0. `ovl_En_Osn/z_en_osn.c:915-925` tests entrances 2, 3 and 6 (C; behavior NI).

## 5. Coverage check against FULL_CITY_PLAN sections 5 and 6

Every one of the 22 scenes is the destination of at least one decoded exit value in another city scene's default header (C; trigger NI):

| Plan location | Scene / room | Decoded inbound exit value(s) |
| --- | --- | --- |
| 5.1 South | `Z2_CLOCKTOWER` room_00 | North `0xD840`; West `0xD850`, `0xD830`; East `0xD870`, `0xD820`; Laundry `0xD860`; Interior `0xD800` |
| 5.2 North; Great Fairy and Deku Playground access | `Z2_BACKTOWN` room_00 | South `0xD620`, `0xD650`; East `0xD610`; Fountain `0xD630`; Deku `0xD640`. North -> Fountain `0x4600`, -> Deku `0x3600` |
| 5.3 West; six venues; out-of-city gate | `Z2_ICHIBA` room_00 | South `0xD420`, `0xD410`; venues `0xD430` to `0xD480`. West -> Termina Field `0x5400` |
| 5.4 East; seven destinations | `Z2_TOWN` room_00 | South `0xD230`, `0xD210`; North `0xD250`; venues `0xD220` to `0xD2B0`. East -> Termina Field `0x5470` |
| 5.5 Laundry; private back room | `Z2_ALLEY` room_00 | South `0xDA00`; Curiosity `0xDA10`. Laundry -> Curiosity room_01 `0x0E10` |
| INT-01 Trading Post | `Z2_8ITEMSHOP` room_00 | West `0x6200` |
| INT-02 Bomb Shop | `Z2_BOMYA` room_00 | West `0xCA00` |
| INT-03 Curiosity Shop + secondary room | `Z2_AYASHIISHOP` room_00, room_01 | West `0x0E00`, Laundry `0x0E10` (section 4.4) |
| INT-04 Post Office | `Z2_POSTHOUSE` room_00 | West `0x5600` |
| INT-05 Swordsman's School | `Z2_DOUJOU` room_00 | West `0xA200` |
| INT-06 Lottery Shop | `Z2_TAKARAKUJI` room_00 | West `0x6C00` |
| INT-07 Stock Pot Inn | `Z2_YADOYA` room_00 to room_04 | East `0xBC00`, `0xBC10` (section 4.1) |
| INT-08 Mayor's Residence | `Z2_SONCHONOIE` room_00 to room_03 | East index 7 `0x0000` (INF, section 4.5) |
| INT-09 Milk Bar | `Z2_MILK_BAR` room_00 | East `0x2400` |
| INT-10 Honey & Darling's | `Z2_BOWLING` room_00 | East `0x0800` |
| INT-11 Treasure Chest Shop | `Z2_TAKARAYA` room_00 | East `0x2800` |
| INT-12 Town Shooting Gallery | `Z2_SYATEKI_MIZU` room_00 | East `0x3A00` |
| INT-13 Town Great Fairy | `Z2_YOUSEI_IZUMI` room_00 only | North `0x4600` (section 4.2) |
| INT-14 Deku Playground | `Z2_DEKUTES` room_00 | North `0x3600` |
| INT-15 Underground route + Observatory | `Z2_TENMON_DAI` room_00 + room_01 | East `0x4C00` (section 4.3) |
| INT-16 Clock Tower interior | `Z2_INSIDETOWER` room_00, room_01 | South `0xC010` |
| INT-17 Clock Tower rooftop | `Z2_OKUJOU` room_00 | South `0x2C00`; `ovl_En_Fall/z_en_fall.c:346` |

Plan items with no scene of their own (accounted for elsewhere):

- **Bank (5.3):** no scene. Actor `EN_GINKO_MAN` is in the West default list (`Z2_ICHIBA_room_00.c:127`); calling it the bank is a name-based INF. Belongs to REF-06.
- **Underground route (INT-15):** room_00 of `Z2_TENMON_DAI` (section 4.3), not a scene.
- **Out-of-city gates / Termina Field:** destination `Z2_00KEIKOKU` (entrance table 0x2A, scene 0x2D), outside the 22 and not extracted. South, North, West, East and the Observatory exit to it.
- **Kafei / private room (INT-03):** `Z2_AYASHIISHOP` room_01; no other scene.
- **South worksite, tower opening, residents:** actor-level and state-level items (ROM_REFERENCE, REF-04), not locations.

Scene features the plan did not anticipate:

1. `Z2_SONCHONOIE` has 4 rooms with room_03 behind room_02. `Z2_INSIDETOWER` has 2 rooms, arrival in room_01 for entrance 0, and an exit to the non-city scene `Z2_OPENINGDAN`, which no packet covers.
2. `Z2_OKUJOU` has no exit list in any of its 4 headers. Its entrance table uses negative scene IDs (`-SCENE_OKUJOU`, `z_scene_table.c:407-424`), which `z_play.c:592` treats specially (NI).
3. `Z2_YOUSEI_IZUMI` packs five fountains into one scene and collision header; only room_00 is city scope. IDs `loc.great_fairy_fountain_town.room_01` to `room_04` are registered here for completeness and flagged for PIPE-01 naming.
4. The Curiosity Shop's two rooms are not door-linked (section 4.4).
5. South's default exit list has 10 values; the playbook's eight boundaries omit `0x2C00` (rooftop e0) and `0xD650` (North e5).
6. `0xD650` (North entrance 5) appears in 9 scenes: South, North, West, East, Laundry, Post Office, Inn, Milk Bar, Treasure Chest Shop (U-03).
7. Milk Bar has 4 alternates (5 room headers) and none of them has an exit list. East alt2 has a different 10-entry exit list, so exit indices differ between East headers.
8. Eight scenes use `RESTRICTIONS_SET(...)`: four West venues share one value (Trading Post, Post Office, Swordsman's School, Lottery Shop), and Deku Playground, Observatory, Interior and Rooftop each have their own. The other interiors use `RESTRICTIONS_INDOORS`, except the Fountain (`RESTRICTIONS_NONE`, like the exteriors). Eight scenes have non-empty persistent cycle flags: South, North, East, Laundry, Post Office, Treasure Chest Shop, Fountain, Interior. Semantics are NI (U-13).

## 6. Unknowns and exact next source questions

| ID | Unknown | Next source question |
| --- | --- | --- |
| U-01 | Which polygon or door uses each exit index, in every scene and header | Decode `exitIndexPlusOne` (surface bits 8-12, as done for South in `docs/CITY_PLAYBOOK.md` section 3) from each `<SCENE>CollisionHeader_*` polygon/surface array, and the exit field of `EN_DOOR` / `EN_HOLL` params in `src/overlays/actors/ovl_En_Door`, `ovl_En_Holl` (REF-02) |
| U-02 | Whether a trailing exit `0x0000` is padding or a real Mayor's Residence e0 exit | U-01 for the last exit index of North, East (index 13), Laundry, Trading Post, Bomb Shop, Swordsman, Lottery, Honey, Shooting Gallery, Fountain, Deku. If nothing references it, record padding |
| U-03 | Trigger and purpose of `0xD650` (North e5) in 9 scenes | U-01 in each scene, plus `Z2_BACKTOWNStartPositionList0x000094[5]` player params (`Z2_BACKTOWN.c:38`) and any `ENTRANCE(NORTH_CLOCK_TOWN, 5)` in `src/` |
| U-04 | Entrances declared in `z_scene_table.c` with no scene `EntranceEntry`: Laundry e2, Mayor e2, East e12, Rooftop e2 (default header) | Search for `ENTRANCE(<scene>, n)` and exit values that reference them. If none, mark unused. For Rooftop, check whether only alt2 (4 entries) is reachable through `sClockTowerRooftopEntrance2` layer 2 |
| U-05 | What state selects each alternate header (South 3, North 1, West 1, East 2, Inn 1, Milk Bar 4, Rooftop 3) | For each scene, find the call sites that pair `ENTRANCE(<scene>, n)` with `nextCutsceneIndex = 0xFFFn`, as `z_demo.c:701-704` does for the fountain. Read the alternate's `SCENE_CMD_CUTSCENE_SCRIPT_LIST` |
| U-06 | Which Great Fairy appears per fountain room; who uses fountain entrances 5 to 9 | Decode `BG_DY_YOSEIZO` params in each `Z2_YOUSEI_IZUMI_room_0N.c` against `ovl_Bg_Dy_Yoseizo`; search computed entrances on `ENTR_SCENE_FAIRY_FOUNTAIN` |
| U-07 | Exit behavior where a header has no exit list (South alt2, Milk Bar alt1 to alt4, all Rooftop headers) | Check whether any surface in those collision headers has a nonzero exit index, and which actors set `play->nextEntrance` in those scenes |
| U-08 | Whether Curiosity Shop room_00 and room_01 share any walkable link | Read `ovl_Obj_Nozoki/z_obj_nozoki.c` around line 444 and `z_player.c:8620-8640`. Confirm that room_00 collision has no room-switch surface |
| U-09 | Which observatory exit (`0xD220`, `0x5490`) belongs to which room | U-01 for `Z2_TENMON_DAICollisionHeader_007514` and the 1/1 `EN_DOOR` params `0x007F` |
| U-10 | Inn upper vs lower street door mapping; trigger of Inn exit index 3 (`0xBC20`) | `EN_DOOR` params `0x028B`, `0x028C` and U-01 for `Z2_YADOYACollisionHeader_00A018` |
| U-11 | Mayor's Residence entry from East is only INF | U-01 for East exit index 7 (door params among `Z2_TOWNTransitionActorList_000160`) |
| U-12 | Clock Tower interior exit to `Z2_OPENINGDAN` and use of entrances 2 to 6 | U-01 for `Z2_INSIDETOWERCollisionHeader_007F10`; read `ovl_En_Osn/z_en_osn.c:900-930`. `Z2_OPENINGDAN` is not extracted |
| U-13 | Meaning of each `RESTRICTIONS_SET` field and of the persistent cycle flags | Read the `RESTRICTIONS_SET` definition and its users (`z_parameter.c`) and the cycle-flag reset code |
| U-14 | Whether the ZAPD-emitted list lengths are exact | Compare each list's start offset with the next declared offset (done only for Mayor's Residence and Laundry Pool, where the exit list starts right after the entrance list) |

## 7. How this was produced

- **Read:** `docs/HANDOFF.md`; `docs/FULL_CITY_PLAN.md` sections 3, 5, 6, 11 and 12; `docs/CITY_PLAYBOOK.md` sections 1 to 3 and 7; `docs/ROM_REFERENCE.md`. Source: `include/tables/scene_table.h` (22 rows plus comments); `src/code/z_scene_table.c` (22 entrance tables, `sSceneEntranceTable`, `Entrance_GetTableEntry`); `include/z64scene.h` (structs, `ENTRANCE` macro line 711, command macros); `src/code/z_scene.c`, `z_play.c` and `z_room.c` at the lines cited; the 22 `assets/xml/scenes/Z2_*/Z2_*.xml`; the 22 scene and 36 room `.c` files under `extracted/n64-us/assets/scenes/` (command arrays, alternate-header arrays, spawn, transition, entrance and exit lists, object/actor list counts, and actor symbol names only for the multi-room rooms in section 4); the decomp call sites and comments cited in section 4.
- **Helper:** `tools/reference/city_index.py` (stdlib, read-only, 147 lines). Run: `python -I tools/reference/city_index.py local-reference/mm-decomp`. It prints, per scene, the scene-table row, XML rooms, entrance-table data, per-header room/transition/spawn/exit/entrance counts with decoded exit destinations, and per-room object/actor counts. Output was kept in scratch, not committed. Decoded exit names are entrance-table segment names, not English labels.
- **Extraction:** the ZAPD C output already existed (extracted earlier today). This task did not run ZAPD, Unity, Blender or any player, and changed nothing under `local-reference/` or `Unity/`.
- **Not inspected:** collision polygons, surface types and camera data; room shapes, display lists, vertices and textures; actor params, half-day masks, overlay behavior (including `EN_DOOR` / `EN_HOLL` door semantics) and visibility; cutscene, path, light, minimap and animated-material data; player start-mode params; equivalence of the C to the ROM binaries; non-city scenes (`Z2_00KEIKOKU`, `Z2_OPENINGDAN`, other-region fountains); restriction and cycle-flag semantics; any 3DS material; in-game captures.
