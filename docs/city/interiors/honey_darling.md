# Honey & Darling's Shop (`loc.honey_darling`): source inventory dossier

Task **REF-09 / INT-10** (FULL_CITY_PLAN.md section 12 row REF-09, section 6 row INT-10). Base commit `c8b847e` on `main` (dispatch override of the brief's `31b20dc`; HEAD checked with `git log`). Section 10 follows schema 1 as frozen at `7a7a08c` (PIPE-01); its IDs were checked against `shared/city/ids.json`. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_BOWLING` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_BOWLING/Z2_BOWLING.c`, `room.c` = `.../Z2_BOWLING_room_00.c`. Line numbers count `\n` only: scene.c has 426 LF and 428 CR, room.c 537 LF and 545 CR (lone `\r` bytes inside some entries); the overlay files are plain CRLF, so their line numbers are unambiguous. Polygon `i` is on scene.c line `188 + i`, vertex `i` on line `312 + i`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. The interior uses its own frame around the origin; it is not placed in the town frame. Overlay files are cited as `z_<name>.c:line` under `src/overlays/actors/ovl_<Name>/` (`z_en_fu.c` is `ovl_En_Fu`, and so on); a bare `:line` inside a table row refers to the file named earlier in that row or section.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected; **C-BIN** decoded from a baserom object binary (collision, skeleton and animation headers, numbers only; method in section 7.3). Role names for the En_Fu fields (`gameKind`, `score`, ...) are mine; the decomp calls them `unk_542`, `unk_548`, ...

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01), `docs/city/PORTAL_REGISTER.md` (REF-02), `docs/city/SOURCE_REGISTER.md` (REF-03), `docs/city/districts/east_clock_town.md` section 2.4 (street door TA4). Agreements and the wording notes are in sections 2.5 and 11.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_BOWLING` / `SCENE_BOWLING` | `include/tables/scene_table.h:41` |
| Scene-table index | `0x11` | same line |
| Entrance-table scene index | `0x04`; row `sHoneyAndDarlingsShopEntranceTable` | `src/code/z_scene_table.c:2577`, rows `:72-78` |
| Title text / draw config / restrictions / cycle flags | `0x108` / `SCENE_DRAW_CFG_MAT_ANIM` / `RESTRICTIONS_INDOORS` / `PERSISTENT_CYCLE_FLAGS_NONE` | scene_table.h:41 |
| XML files | scene `Z2_BOWLING` segment 2; room `Z2_BOWLING_room_00` segment 3; one manual `DList Z2_BOWLING_room_00DL_004CF0` at `0x4CF0` | XML `assets/xml/scenes/Z2_BOWLING/Z2_BOWLING.xml:2-8` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene.c:14-29 | HDR |
| Room headers | default only (room.c:14-21) | HDR |
| Transition actors | 1 `EN_DOOR` | HDR scene.c:16, :37-39 |
| Region visited | `SET_REGION_VISITED` (scene.c:17); `SCENE_BOWLING` is in the Clock Town list (`src/code/z_inventory.c:431`) | HDR, C |
| Special files / cutscene scripts | none / none (no `SPECIAL_FILES`, no `CUTSCENE_SCRIPT_LIST`) | HDR scene.c:14-29 |
| Sound | `SOUND_SETTINGS(5, 19, 0)`: spec 5, ambience 19, seq 0 (`NA_BGM_GENERAL_SFX` by table order); the room music comes from a04 (5.11) | HDR scene.c:14; macro `include/z64scene.h:845-846`; `include/tables/sequence_table.h:20` |
| Skybox / light mode | `SKYBOX_SETTINGS(0x00, 0, 0, true)`: no skybox, `LIGHT_MODE_SETTINGS` (fixed settings, not time of day); room `SKYBOX_DISABLES(true, true)` | HDR scene.c:22, room.c:16; `include/z64environment.h:48-50` |
| Env light settings | 4 entries, all different (not transcribed; the Bomb Shop's differ only in fog); every scene-floor surface selects index 0 (5.11) | HDR scene.c:24, :55-64 |
| Actor cutscenes / cs cameras / paths | 10 / 1 / 3 (16, 8 and 8 points) | HDR scene.c:20, :25-26, :66-110, :113-124, :140-142 |
| Minimap | one entry `{0xFFFF, 0, 0, 0, 0}`, scale 0 | HDR scene.c:27, :126-132 |
| Animated materials | 5 entries, all `type 1` (two-tile scroll) on segments 8 to 12 (`ABS(segment) + 7`; last entry negative) | HDR scene.c:28, :144-175; `src/code/z_scene_proc.c:95-109, :388-408` |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x02, 0x00, 0, 0, false, 0)`: room type 2 = `ROOM_TYPE_INDOORS`, environment 0, storm 0 (the Bomb Shop's first argument is 0x00); `SKYBOX_DISABLES(true, true)`; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3, 5 without Ocarina); cullable shape (type 2) with 10 entries; 3 objects, 5 actors | HDR room.c:14-20, :52-56; `src/code/z_scene.c:246-253, :399-402` |
| Room-type consumers | run speed limit set to 500 (`R_RUN_SPEED_LIMIT`); camera default for the type is `CAM_SET_ROOM0` | C `src/code/z_player_lib.c:634-635`; `src/code/z_camera.c:6995-6997`; enum comment (ANN) `z64scene.h:317` |

### 1.1 Alternate headers: none

`z_scene_table.c:72-78` declares one entrance with one layer (`sHoneyAndDarlingsShopEntrance0[] = { { SCENE_BOWLING, 0, 0x4102 } }`), so every profile loads the default scene and room header (C). Day and time variation inside the shop comes only from actor half-day masks (section 4) and overlay logic (section 5). Profiles of interest: `prof.d1.noon.normal` (open, Honey & Darling present), `prof.d1.night.normal` (21:00, still open), `prof.d1.late.normal` (23:00, door closed, section 5.10), `prof.d3.night.normal` (21:00 Day 3: door open but no Honey & Darling, section 4.2). All select header 0.

### 1.2 Room list

`loc.honey_darling.room_00` = declared file `Z2_BOWLING_room_00` (XML segment 3), default header 0: the only room, an octagonal hall with the rotating platform and the entrance corridor behind the door. LOCATION_REGISTER row `r00 d 3/5`, 1 transition, 1 spawn, 2 exits (last `0x0000`), entrances 2 / 1: all agree with HDR (scene.c:15-21, :45-53; room.c:19-20).

## 2. Spawns, entrances and exits

### 2.1 Start position (HDR scene.c:32-35; decode C)

Player params: start mode `(params & 0xF00) >> 8`, camera index `params & 0xFF` (`include/z64player.h:17-18`); yaw `(rawRotY >> 7) & 0x1FF`.

| Spawn | Position | Raw rot X / Y / Z | Yaw | Params | Mode | Camera index | Used by |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (0, 0, 300) | 0x0007 / 0x5A00 / 0x007F | 180 (faces -Z, toward the platform) | 0x0EFF | 0xE `PLAYER_START_MODE_E` | 0xFF: no spawn camera change (`z_play.c:2326`) | Entrance 0 = `0x0800`, East exit index 6: arrival of `portal.east_clock_town.honey_darling` |

- 42 u in front of the door TA0 (door-local (0, -42)), on floor poly 36 (surface 1, Y 0), 128.9 u radially outside the platform deck edge (radius 171.1, C-BIN) (C, scratch `col.py`, `objcol.py`).
- RotY low 7 bits are 0, so `Actor_SpawnEntry` passes csId 0 to the player (`src/code/z_actor.c:3653`); whether Player reads it: NI. Cutscene entry 0 exists (scene.c:114) but no actor here uses it.
- Entrance table (C, `z_scene_table.c:72-78`): entrance 0 `{SCENE_BOWLING, 0, 0x4102}`. Scene `EntranceEntry` list `{0x00, 0x00}, {0x00, 0x00}` (HDR scene.c:45-48): row 0 = spawn 0, room 0; row 1 has no declared entrance, so it is padding (INF, 2.2).

### 2.2 Exit list (HDR scene.c:50-53) and consumers

| Exit idx | Raw | Decode (`Entrance_GetTableEntry`, `z_scene_table.c:2688-2694`) | Destination row | Consumer |
| --- | --- | --- | --- | --- |
| 0 | `0xD260` | 0x69 `Z2_TOWN`, entrance 6, layer 0 | `sEastClockTownEntrance6 = { SCENE_TOWN, 6, 0x4102 }` (`z_scene_table.c:2224-2226`) -> East spawn 6 (1395, 100, -274) yaw 270, params 0x0F08 (East dossier 2.1) | surface type 3 floor polys 91, 92 through the door probe (2.3, 2.4); a00 `En_Time_Tag` kickout (`params & 0x1F` = 0, 5.10) |
| 1 | `0x0000` | n/a | n/a | none (below) |

**U-02 for this scene: padding (INF, with C evidence).** Layout (HDR symbol offsets): entrance list `0x0000A8` (4 bytes), exit list `0x0000AC` (4 bytes), light settings `0x0000B0`, so each list is one used 2-byte entry plus 2 bytes of alignment. Readers: of the nine overlays spawned here (En_Time_Tag, En_Door, En_Fu, Bg_Fu_Kaiten, En_Fu_Mato, En_Fu_Kago, Bg_Fu_Mizu, En_Test4, Obj_Sound) only En_Time_Tag reads `setupExitList` (`ovl_En_Time_Tag/z_en_time_tag.c:287`, index `params & 0x1F` = 0); En_Test4 sets `nextEntrance` itself for the Day 3 midnight case (`z_en_test4.c:513-522`) and Player reads exits only from collision (`SurfaceType_GetSceneExitIndex`, `ovl_player_actor/z_player.c:6416`). No surface type has exit+1 = 2 (2.4). No `EntranceEntry` 1 reader exists and only one entrance row is declared.

Inbound: East `Z2_TOWNExitList_0001F0[6] = 0x0800` (PORTAL_REGISTER row; East dossier 2.2 exit table). The debug map select is excluded as in REF-02.

### 2.3 Transition actor and door decode (HDR scene.c:37-39; C)

| TA | Sides (front / back: room, bgCam) | Position | rotY raw | Yaw | Spawned params | Type / schedule type | Door art (OVL) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 0 / 0, 255 / 255 (-1 = generic door camera, `include/z64scene.h:369-373`) | (0, 0, 342) | 127 = 0x7F | `(127 >> 7) & 0x1FF` = 0: door +Z = world +Z; csId `0x7F` none | `TRANSITION_ACTOR_PARAMS(0, 0x0289)` = 0x0289 (`include/z64door.h:22-23`; `z_actor.c:3613-3619`) | `(p >> 7) & 7` = 5 `ENDOOR_TYPE_SCHEDULE`, `p & 0x7F` = 9 `ENDOOR_SCH_TYPE_HONEY_DARLING_SHOP` (`ovl_En_Door/z_en_door.h:64`) | `gDoorSkel` with `sObjectInfo[17 + 9]` = `DOOR_OBJINFO_26` = `{ -1, DOOR_DL_DEFAULT, GAMEPLAY_KEEP }` -> `gDoorLeftDL` / `gDoorRightDL` (`z_en_door.c:221, :307, :347`) |

Door probe (C, my evaluation, scratch `col.py`): opening a non-fake door raycasts the floor at `door + (-doorDirection x 22)` along the door yaw at door Y + 10 and passes that polygon to `Player_HandleExitsAndVoids` (`z_player.c:6779-6783`; East dossier 2.4 for the mechanism). `doorDirection` is +1 on the door's local +Z side, else -1 (`z_en_door.c:496`).

| Player side | Door-local z | Probe point | Floor hit | Result |
| --- | --- | --- | --- | --- |
| Inside the hall (spawn 0 is at local (0, -42)) | < 0, `doorDirection` -1 | (0, 10, 364) | poly 92, surface 3, exit+1 = 1 | exit 0 `0xD260` -> East spawn 6 |
| Corridor pad side (z > 342, never a spawn) | > 0, `doorDirection` +1 | (0, 10, 320) | poly 36, surface 1, no exit | door opens, no scene change |

The schedule runs only from the +Z side (`z_en_door.c:537`), the pad the player never stands on, so leaving is always possible and the interior door never evaluates `sDoorSch_HoneyDarlingShop`.

Geometry around the door (C): wall polys 95, 96 close the corridor mouth at z = 322 (x -30..30, Y 0..100, normal -Z); corridor floor polys 93, 94 (surface 4) z 322..342; the door sits at z = 342, 20 u beyond that wall; exit pad floor polys 91, 92 (surface 3) z 342..442, x -30..30; end wall 53, 54 at z = 442; ceilings 20, 21 and 71, 72 at Y 100; side walls 18, 19, 22, 23, 69, 70, 73, 74. Whether the player is carried through the wall by the door action: INF, U-05.

### 2.4 Collision surfaces carrying exits (C from scene.c:182-185 and the polygon list)

| Surface type | data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Kind |
| --- | --- | --- | --- | --- | --- |
| 3 | 0x00000100 / 0x00001002 | 0 (`0xD260`) | 91, 92 | -30..30 / 0 / 342..442 | 2 flat floors (normal +Y); xp flag 1 (`COLPOLY_IGNORE_CAMERA`); bgCam 0; no conveyor |

The pair centroid (0, 0, 392) is 50 u from TA0 and 92 u from spawn 0 (the REF-02 values "50u" and "92u").

### 2.5 Door-to-venue cross-check with East Clock Town (East dossier 2.4, TA4 row)

| Item | East side (street) | This scene | Agree |
| --- | --- | --- | --- |
| Door actor | TA4 (1420, 100, -274) yaw 270, params 0x0289 | TA0 (0, 0, 342) yaw 0, params 0x0289 | yes: same `HONEY_DARLING_SHOP` schedule type on both leaves |
| Exit trigger | polys 477, 478, East exit index 6 `0x0800` | polys 91, 92, exit index 0 `0xD260` | yes (REF-02 rows) |
| Arrival | Honey & Darling spawn 0 (0, 0, 300) yaw 180, mode 0xE | East spawn 6 (1395, 100, -274) yaw 270, mode 0xF | yes |
| Schedule at Day 1 12:00 | open | never evaluated from inside | yes |
| Leaf art | gameplay_keep default pair | same | yes |

`0x0800` decodes to table 0x04, entrance 0, layer 0 (`0x0800 >> 9`, `(0x0800 >> 4) & 0x1F`, `& 0xF`). The two scenes use independent frames (East Y 100, interior Y 0); no shared-frame claim is made. REF-02's "open hours UNKNOWN" is resolved in 5.10: closed from about 21:59 to 05:59 from the street. No disagreement with the East dossier; one wording note is in section 11.

### 2.6 Portal IDs (registered in `docs/city/PORTAL_REGISTER.md` and `shared/city/ids.json`; no new IDs)

| ID | Source evidence | Arrival |
| --- | --- | --- |
| `portal.honey_darling.east_clock_town` | surface 3 polys 91, 92, exit 0 `0xD260`, TA0 | East spawn 6 |
| `portal.honey_darling.east_clock_town.kickout` | a00 `En_Time_Tag` `0x0060`, exit 0, 22:00 on Days 0 to 2 (5.10) | East spawn 6 |
| `portal.east_clock_town.honey_darling` (inbound; East owns it) | East surface 18 polys 477, 478, TA4 | spawn 0 here |

Single room, so no `portal.honey_darling.room_NN.room_MM` rows.

## 3. Object banks (HDR room.c:24-28; IDs from `include/tables/object_table.h`)

An actor whose profile object is not loaded fails to spawn (`Actor_SpawnAsChildAndCutscene`, `z_actor.c:3517-3523`, as read for the Bomb Shop dossier).

| # | Bank | Object ID | Binary size (C-BIN) | Consumer (OVL) | Content (XML) | Needed for Day 1 static list |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | `OBJECT_FU_KAITEN` (`object_table.h:433`) | 0x1A0 | 14384 | Bg_Fu_Kaiten profile object (`z_bg_fu_kaiten.c:21`); Bg_Fu_Mizu profile object (`z_bg_fu_mizu.c:21`) | platform DL `0x5D0` ("dai_model"), platform collision `0x2D30`, water DLs `0x2EB0`, `0x2FB0`, `0x2FC0`, water collision `0x37F8`, water matanim `0x37D8`; 1 TLUT (rgba16 4x4), 1 ci4 64x64, 4 rgba16 (16x64, 3 of 32x32) | yes (platform) |
| 1 | `OBJECT_FU_MATO` (`:434`) | 0x1A1 | 12880 | En_Fu_Mato and En_Fu_Kago profile object (`z_en_fu_mato.c:32`, `z_en_fu_kago.c:33`) | 6 basket DLs `0x6A0`..`0x9C0`, basket collision `0x15C0`, 6 target-shard DLs `0x1C80`..`0x2108`, target collision `0x23D4`, target DL `0x2720`; 3 rgba16 (32x32, 16x16, 16x64) | yes (Day 1 targets) |
| 2 | `OBJECT_MU` (`:337`) | 0x140 | 47840 | En_Fu profile object (`z_en_fu.c:61`) | Honey and Darling: 6 animations, 1 flex skeleton (20 limbs), 1 TLUT + 11 textures (9 ci8, 2 i8), heart DLs `0xB0A0`, `0xB0E0` | yes (skeleton, deferred) |

Implicit: `gameplay_keep` 0x001 (En_Door skeleton and leaves, En_Time_Tag, En_Test4, Obj_Sound; heart texture `gDropRecoveryHeartTex`, `z_en_fu.c:1500`).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): ID `id & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF`, converted with `DEG_TO_BINANG` unless the ID carries that axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), then it is a plain integer (wrapped to -180..180). csId = `rawRotY & 0x7F` (0x7F -> none, 3570-3574). Half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all (2562-2565, 3576-3580). `home.rot` keeps the decoded rotations (3563-3568).
- Bits (`include/z64actor.h:328-337`): D0d 0x200, D0n 0x100, D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001. `Actor_InitHalfDaysBit` (2491-2499): Day 1 at 12:00 -> 0x080. `Actor_SpawnSetupActors` kills entries whose mask lacks the current half-day (`Actor_KillAllOnHalfDayChange`, `z_actor.c:2555`, `:3311-3318`) and is called from `Actor_UpdateAll` (`:2714`).
- Special flags here: a00 is `ACTOR_EN_TIME_TAG | 0xE000`: all three rotations are integers. `SPAWN_ROT_FLAGS(rot, flags)` = `(rot << 7) | flags` (`include/z64scene.h:16`).
- Actors spawned with plain `Actor_Spawn` get `HALFDAYBIT_ALL` and `CS_ID_NONE` (`z_actor.c:3451-3455`).

### 4.2 Default header (5 entries, HDR room.c:30-46, decode C; scratch `col.py`)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| a00 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (40, 0, 233) | 0x0B07 / 0x007F / 0x0070 | int 22 / int 0 / int 0 | 0x3F0 = D0d..D2n | none | 0x0060 | gameplay_keep | controller-invisible | yes |
| a01 | `ACTOR_BG_FU_KAITEN` (0x1AE) | (0, 0, 0) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x000F | object_fu_kaiten | static-mesh (dyna, moving) | yes |
| a02 | `ACTOR_EN_FU` (0x0B5) | (0, 49, 0) | 0x0007 / 0x0009 / 0x0078 | 0 / 0 / 0 | 0x3F8 = D0d..D3d | 9 | 0x0000 | object_mu | npc-scheduled (posed-skeletal) | yes |
| a03 | `ACTOR_EN_TEST4` (0x15A) | (-1, 0, 410) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0100 | gameplay_keep | controller-invisible | yes |
| a04 | `ACTOR_OBJ_SOUND` (0x0F0) | (3, 382, -272) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0146 | gameplay_keep | controller-invisible | yes |

Transition actor TA0 (`EN_DOOR` 0x005, door-transition, all half-days) is in 2.3. Findings from the masks (C):

- a00: hour = `home.rot.x` = 22, minute = `home.rot.y` = 0 (`TIMETAG_KICKOUT_HOUR/MINUTE`, `z_en_time_tag.h:14-15`); type `(0x0060 & 0xE000) >> 13` = 0 `TIMETAG_KICKOUT_DOOR`; text `(0x0060 & 0x1FE0) >> 5` = 3 `TIMETAG_KICKOUT_TEXT_HONEY_AND_DARLING`; exit index `0x0060 & 0x1F` = 0. The mask excludes Day 3, so there is no 22:00 kickout on Day 3.
- a02: mask 0x3F8 excludes D3n, D4d, D4n: Honey and Darling are removed at the Day 3 18:00 half-day change (INF from `z_actor.c:2555-2571`, `:3311-3318`) and so are absent from `prof.d3.night.normal` on. Their children (targets, baskets) are spawned with `HALFDAYBIT_ALL` (4.1) and are not removed by the mask. a02 csId 9 selects cutscene entry 9 (5.11).
- a03: `THREEDAY_GET_SKYBOX_NUM_STARS` = `(0x0100 >> 10) * 100` = 0; bell volume index `(0x0100 >> 5) & 0xF` = 8 (`ovl_En_Test4/z_en_test4.h:8-9`); csId none: `sCsIdList` entries are `CS_ID_NONE`, so day/night swaps in this room play no transition cutscene (`z_en_test4.c:326-337`, `:54`, `:129`).
- a04: type `(0x0146 >> 8) & 0xFF` = 1 `OBJ_SOUND_TYPE_BGM`; id `0x0146 & 0x7F` = 0x46 = ordinal 70 in `sequence_table.h` = `NA_BGM_SHOOTING_GALLERY` (by table order, `sequence_table.h:160`; scratch `seq.py`); from its second update it calls `Audio_PlayObjSoundBgm(&projectedPos, 0x46)` (`ovl_Obj_Sound/z_obj_sound.c:28-66`); on destroy it restores `NA_BGM_GENERAL_SFX` (`:43-45`).
- Spawn order: a01 precedes a02, and a02 looks up the first `ACTOR_BG_FU_KAITEN` in the BG list at Init (`z_en_fu.c:199-210`); if none is found, En_Fu does nothing at all (`:212`).

### 4.3 Runtime children of a02 (spawned in `EnFu_Init` -> `func_809619D0` -> `func_809616E0`, OVL `z_en_fu.c:164-197`, `:123-162`)

The day selects the path by following `additionalPathIndex` from path `(params >> 8) & 0xFF` = 0 (`ENFU_GET_PATH_INDEX`, `z_en_fu.h:12`) for `gameKind` steps (`z_en_fu.c:166, :190-192`). Path array `{16, 1, -1, list 0x138}, {8, 2, -1, list 0x108}, {8, -1, -1, list 0x198}` (HDR scene.c:107-111). Each point spawns one actor at the point with rotation (0, yaw toward a02, 0) and params = point index; the yaw is `Math_Vec3f_Yaw`-form (`Math_Atan2S_XY(dz, dx)`, `z_en_fu.c:136-142`; `z_lib.c:427-432`); the whole degrees below are my values.

| CURRENT_DAY | gameKind | Path | Actor spawned | Count | Notes |
| --- | --- | --- | --- | --- | --- |
| 1 (also 0, 4 and any other: default branch) | 2 | Path[2] (8 pts) | `ACTOR_EN_FU_MATO` (0x1B3) | 8 | stationary until hit |
| 2 | 1 | Path[1] (8 pts) | `ACTOR_EN_FU_KAGO` (0x1B4) + `ACTOR_BG_FU_MIZU` (0x1BC) at the platform's home position, params 0 | 8 + 1 | water actor spawned at Init (`:235-241`) |
| 3 | 0 | Path[0] (16 pts) | `ACTOR_EN_FU_MATO` | 16; 8 when `GET_PLAYER_FORM == DEKU` (only odd indices spawn, `:130-132, :140`) | targets move along the path |

Path points (HDR scene.c:66-105; radius = distance from the origin, C):

| idx | Path[2] Day 1 (x, y, z) / yaw | Path[1] Day 2 (x, y, z) / yaw | Path[0] Day 3 (x, y, z) / yaw |
| --- | --- | --- | --- |
| 0 | (-2, 140, 297) / 180 | (125, 140, 297) / 203 | (126, 139, -293) / 337 |
| 1 | (209, 200, 211) / 225 | (-125, 80, 297) / 157 | (210, 200, -210) / 315 |
| 2 | (292, 140, 2) / 270 | (-293, 120, 127) / 113 | (295, 137, -124) / 293 |
| 3 | (212, 200, -208) / 314 | (-297, 80, -125) / 67 | (295, 200, 0) / 270 |
| 4 | (0, 140, -290) / 0 | (-125, 100, -295) / 23 | (295, 138, 127) / 247 |
| 5 | (-212, 200, -210) / 45 | (127, 80, -293) / 337 | (207, 200, 214) / 224 |
| 6 | (-294, 140, 2) / 90 | (295, 100, -123) / 293 | (126, 137, 294) / 203 |
| 7 | (-212, 200, 208) / 134 | (297, 120, 130) / 246 | (0, 200, 298) / 180 |
| 8 to 15 | | | (-125, 141, 296) / 157; (-214, 200, 208) / 134; (-292, 140, 127) / 114; (-294, 200, 1) / 90; (-294, 140, -125) / 67; (-211, 200, -208) / 45; (-126, 139, -294) / 23; (0, 200, -293) / 0 |
| radius | 290..298 | 319..324 | 293..321 |
| closed-loop length | 1871 | 1984 | 2190 (1095 updates per lap at speed 2.0) |

Path[2] points sit at Y 140 and 200 alternately on the wall side of the hall (radius 290..298, inside the lower wall at 322); the Day 3 Deku subset (odd indices) is all at Y 200. Heights are above the hall floor (Y 0).

### 4.4 Overlay findings per actor type (OVL)

| Actor | Init / draw / state | Presence gating |
| --- | --- | --- |
| En_Time_Tag (a00) | profile `ACTORCAT_ITEMACTION`, draw NULL (`z_en_time_tag.c:38-48`); type 0 keeps `EnTimeTag_KickOut_WaitForTime` (`:50-55`, `:315-339`) | mask 0x3F0 |
| Bg_Fu_Kaiten (a01) | `ACTORCAT_BG`, scale 1.0, `DYNA_TRANSFORM_POS \| DYNA_TRANSFORM_ROT_Y`, collision `object_fu_kaiten:0x2D30`; `rotationSpeed`, `bounceSpeed`, `bounceHeight`, `elevation`, `bounce` all 0 at Init (`z_bg_fu_kaiten.c:29-48`); culling disabled | always (mask all) |
| En_Fu (a02) | `ACTORCAT_NPC`, flex skeleton `gHoneyAndDarlingSkel`, scale 0.01, mass immovable, gravity -0.2, shadow circle 36, attention flag cleared at Init, attention range type 6, yaw +0x4000, cylinder radius 20 / height 50 (AC on only on Day 3) (`z_en_fu.c:199-244`, `:103-121`, `:1360-1366`) | mask 0x3F8; needs a01 first |
| En_Fu_Mato | `ACTORCAT_BG`, dyna `object_fu_mato:0x23D4`, scale 0.1, sphere collider radius 17 (Day 3), 30 and `ELEM_MATERIAL_UNK0` (Day 1); killed if no En_Fu exists (`z_en_fu_mato.c:69-111`) | spawned by a02 |
| En_Fu_Kago | `ACTORCAT_BG`, dyna `object_fu_mato:0x15C0`, scale 0.1, sphere radius 10, health 1; killed if no En_Fu (`z_en_fu_kago.c:81-112`) | spawned by a02 on Day 2 |
| Bg_Fu_Mizu | `ACTORCAT_BG`, scale 1.0, dyna `object_fu_kaiten:0x37F8` (no polygons, one water box), Y = home Y - 10 at Init (`z_bg_fu_mizu.c:29-40`) | spawned by a02 on Day 2 |
| En_Test4 (a03) | generic three-day controller (5.11) | always |
| Obj_Sound (a04) | positional BGM source (4.2) | always |
| En_Door (TA0) | 2.3 | always |

### 4.5 Counts

| | Default header |
| --- | --- |
| Room entries / transition actors | 5 / 1 |
| Runtime children (Day 1 / Day 2 / Day 3) | 8 / 9 / 16 (8 for Deku) |
| Day 1 daytime candidates by mask | 5 of 5 |
| static-mesh | 1 (a01) + 8 children on Day 1 |
| npc-scheduled | 1 (a02) |
| controller-invisible | 3 (a00, a03, a04) |
| door-transition | 1 (TA0) |
| billboard-effect | 0 actors (heart particles are drawn by a02, 5.9) |

## 5. Venue mechanics index (INT-10 scope)

Scope: the daily games, fee, timer, win flags and heart piece, Honey and Darling's poses, the rotating platform. Index and cite only; no Unity design. The decomp calls the three games after the camera settings: `CAM_SET_HONEY_AND_DARLING_1` "BOMBBASKET" (0x3A), `CAM_SET_HONEY_AND_DARLING_2` "BOMBBOWL" (0x4B) (`include/z64camera.h:205, :222`). By day: Day 1 Bombchus at stationary targets, Day 2 bombs into baskets over rising water, Day 3 arrows (Human) or Deku bubbles (Deku) at moving targets.

### 5.1 Who does what

| Role | Actor | Mechanism |
| --- | --- | --- |
| Talk, fee, game state machine, reward | a02 En_Fu | `actionFunc` chain `func_809622FC` -> ... (`z_en_fu.c:390-967`) |
| Platform motion | a01 Bg_Fu_Kaiten | En_Fu writes its speed fields through `actor.child` (`z_en_fu.c:202-210`, `:657-663`) |
| Targets | En_Fu_Mato | score when hit (`z_en_fu_mato.c:355-382`) |
| Baskets | En_Fu_Kago | score when a bomb's fuse reaches 1 inside (`z_en_fu_kago.c:145-176`) |
| Water (Day 2) | Bg_Fu_Mizu | height follows `unk_160` and basket state (`z_bg_fu_mizu.c:48-81`) |
| Kickout, player hooks | a00 En_Time_Tag; Player and HUD | 5.10; 5.12 |

### 5.2 State storage

| Store | Meaning (role) | Written | Cleared | Read | Cycle persistence |
| --- | --- | --- | --- | --- | --- |
| `WEEKEVENTREG_22_10` (byte 22, 0x10; `z64save.h:895`) | Day 1 game won as Human | `z_en_fu.c:947` (after the reward talk, Human only) | cycle reset | day texts (`:1049, :1073, :1082, :1107, :1114, :1123`), heart piece gate (`:408-409, :922`) | no: byte 22 keeps only 0x02 and 0x80 (`z_sram_NES.c:102-103`, loop `:486-496`) |
| `WEEKEVENTREG_22_20` (0x20; `:896`) | Day 2 game won as Human | `:951` | cycle reset | same | no |
| `WEEKEVENTREG_22_40` (0x40; `:897`) | Day 3 game won as Human | `:955` | cycle reset | `:410, :924, :1106` | no |
| `WEEKEVENTREG_RECEIVED_HONEY_AND_DARLING_HEART_PIECE` (byte 22, 0x80; `:898`) | the piece has been given | `:928` (during the talk after the reward) | never | `:905, :927` | **yes** (`z_sram_NES.c:102-103`) |
| `WEEKEVENTREG_08_01` (byte 8, 0x01; `:731`) | a game is running (HUD/ammo gate) | `z_en_fu.c:761` (`func_80962F10`) | `:851`, En_Fu `Destroy` (`:250`) | `z_parameter.c:2209, :2221, :2290, :2347, :4259`; `z_player.c:3977, :4039` | no (byte 8 keeps 0x20 only, `z_sram_NES.c:88`) |
| `WEEKEVENTREG_KICKOUT_WAIT` (byte 63, 0x01; `:1227`) | holds the 22:00 kickout | `z_en_fu.c:553` (start text 0x287D) | `:626` and `Destroy` (`:249`) | `z_en_time_tag.c:301, :327` | no (byte 63 keeps 0x20 only, `z_sram_NES.c:164`) |
| `WEEKEVENTREG_KICKOUT_TIME_PASSED` (0x02; `:1229`) | 22:00 passed while held | En_Time_Tag (`:331`) | `z_en_fu.c:554, :627` | `z_en_time_tag.c:301, :328` | no |
| Actor fields `unk_53E`, `unk_540` | "already told the long speech" (Human / Deku) | `z_en_fu.c:1056, :1078, ...`, `:1180` | scene unload | `:1052, :1176` | none (actor memory) |

### 5.3 The three days side by side

| | Day 1 (also Day 0 / 4) | Day 2 | Day 3 |
| --- | --- | --- | --- |
| `gameKind` (`unk_542`, `z_en_fu.c:168-188`) | 2 | 1 | 0 |
| Playable form / gear | Human, `UPG_BOMB_BAG != 0` (else text 0x2853); other forms 0x286F (`:1043-1064`) | same (`:1066-1093`) | Human with `UPG_QUIVER != 0` (else 0x284B); Deku needs `isMagicAcquired` (else 0x2843; 0x2845 first, 0x2847 later); other forms 0x2841 (`:1095-1140`, `:1174-1188`) |
| Target | 8 `En_Fu_Mato`, stationary, sphere 30 | 8 `En_Fu_Kago` | 16 `En_Fu_Mato` moving (8 for Deku), sphere 17 |
| Score trigger | any overlap of the target's OC sphere (`ocFlags1 & OC1_HIT`), `fu->gameKind == 2` (`z_en_fu_mato.c:362-363`) | bomb enters the basket sphere (OC hit by `ACTOR_EN_BOM`), fuse set to 20; score when the fuse reads 1 (`z_en_fu_kago.c:125-137, :145-176`); a basket touched by the Player breaks without score (`:130-132`) | AC hit on the target (`z_en_fu_mato.c:362`); Human: target breaks in two bursts (`:198-245`); Deku: target falls (`:166-196`, `:370-373`) |
| B button item (HUD) | `ITEM_BOMBCHU` (Day 1) | `ITEM_BOMB` (Day 2) | `ITEM_BOW` (otherwise, Day 3); Deku keeps B as is (`z_parameter.c:2247-2258, :2304-2312`, 5.12) |
| Free ammo | `play->unk_1887D = 30` every update while playing (Bombchu count; `:780-781`) | `unk_1887E = 30` every update (bomb count; `:776-778`) | Human: `PLAYER_STATE3_400` every update (`:770-773`); Deku: magic filled at start (`:676-678`) and bubbles free here (`z_player.c:3975-3979, :4038-4040`) |
| Invulnerable while playing | `PLAYER_STATE3_400000` (`:736`, cleared `:805`) | same (`:705`) | no |
| Spin-up (before the timer starts) | rotation 100, bounce speed 600, bounce height 40 (`func_80962D60`, `:723-747`) | rotation 100, bounce speed 600, bounce height 30, elevation 10 (`func_80962BCC`, `:691-716`) | rotation 300, bounce speed 600, bounce height 40 (`func_80962A10`, `:655-681`) |
| Rotation ramp by time left | > 40 s: spin-up value; 40..20 s: 200; < 20 s: 300 | same as Day 1 | > 40 s: 300; 40..20 s: 400; < 20 s: 500 (`D_80964B00 = {300, 100, 100}` indexed by `gameKind`, +100 then +200, `:69, :788-796`) |
| Camera while playing | actor cs cam 0 via csId 9, re-applied every update (`:749-756`, `:832`) | same | floor bgCam 1 `CAM_SET_HONEY_AND_DARLING_2` (no override: `:750`, 5.11) |
| After the game | `Camera_ChangeSetting(HONEY_AND_DARLING_2)`, `unk_1887D = 0` (`:1199-1204`) | `unk_1887E = 0` | none |
| Win flag | `22_10` | `22_20` | `22_40` |
| Reward | purple rupee | purple rupee | purple rupee; heart piece on a qualifying first win (5.6) |
| Water | none | Bg_Fu_Mizu (5.8) | none |

### 5.4 Fee, start sequence and replay

| Step | Rule | Source |
| --- | --- | --- |
| Talk offer | idle: `Actor_OfferTalk` range 100; with `phase == 2` the talk is auto-accepted at range 500 (flag `ACTOR_FLAG_TALK_OFFER_AUTO_ACCEPTED`) | `z_en_fu.c:395-441` |
| Opening text | 0x283C, then the day chain (`func_809639D0`, `:1042-1145`) | `:433-435`, `:476-478` |
| Fee choice | text 0x2871, choice 0 = yes: needs `rupees >= 10`, then `Audio_PlaySfx_MessageDecide`, `Rupees_ChangeBy(-10)` and the "ready" text by day (0x2875 / 0x2877 Day 3 non-Human / Human, 0x2879 Day 2, 0x287B Day 1). Short of rupees: error sound and 0x2873. No: 0x2872 | `:445-465`, `:1147-1172` |
| Fee amount | 10 rupees, hard-coded; charged before the round, never refunded | `:451-453` |
| Start text 0x287D | sets `KICKOUT_WAIT`, clears `KICKOUT_TIME_PASSED`, closes the text, halts the player (`PLAYER_STATE1_20`), `HONEY_DARLING_ANIM_GAME_DANCE`, `Audio_PlaySubBgm(NA_BGM_TIMED_MINI_GAME)`; on a replay the old targets/baskets are marked to shrink away (`func_80961EC8` / `func_80961F00`); then the day's spin-up function | `:552-576`, `:299-319` |
| Round ready | when platform speed, bounce height and bounce speed reach the day's values: `NA_SE_SY_FOUND`, un-halt, `Interface_StartTimer(TIMER_ID_MINIGAME_2, 60)`; first round sets `playedBefore`, later rounds respawn the targets (`func_809616E0`) | `:655-681`, `:691-716`, `:723-747` |
| Timer | 60 s countdown (`SECONDS_TO_TIMER(s) = s * 100`), HUD position (115, 80); no bonus time | `src/code/z_parameter.c:836-852`; `include/z64save.h:79` |
| Hit-the-host text | Day 3 AC hit or Day 1 Bombchu overlap on En_Fu starts text 0x288B; no state change (content NI) | `:254-268`, `:798-800` |
| Explosive pull (Day 2) | grounded bombs on a basket's floor are eased toward the basket centre each update | `:270-288` |

### 5.5 Game loop and end conditions (`func_80962F4C`, `z_en_fu.c:765-833`)

| Condition | Result |
| --- | --- |
| `score == targetCount` (`unk_548 == unk_54C`) | win: clear invulnerability, kill all explosives (`func_80961E88`), halt the player, stop the sub BGM, timer set to 0 and stopped, fanfare `NA_BGM_GET_ITEM \| 0x900`, `Interface_SetPerfectLetters(PERFECT_LETTERS_TYPE_1)`, `phase = 3` (`:821-830`) |
| timer <= 0 | lose: text 0x2885 (`:808-811`) |
| the Player is not above the platform and stands on ground | lose: text 0x2888 (`:802-815`) |
| any of the three | the same clean-up as a win first (`:805-807`), then `func_809632D0` (`:846-861`): `HUD_VISIBILITY_ALL` for Deku, `08_01` cleared, water released, En_Fu attention re-enabled |

`targetCount` is the number of spawned targets/baskets (`unk_54C`, `:143`); a Day 2 basket broken by the Player (no score) makes a win impossible (INF from `z_en_fu_kago.c:130-132`). After the end the platform winds down to rest while `func_809638F8` subtracts 25 per update from `envCtx.adjLightSettings` ambient and light-1 colours; nothing in this scene raises them above 0 (zeroed at `z_kankyo.c:667-676`), so the fade is a no-op in a fresh state (INF, U-07). When everything is at rest, `func_80963F88` restores the camera (5.3) and `EnFu_MovePlayerToPos` walks the player (forced input, stick magnitude 10 / 40 / 80 by distance) to within 80 u of En_Fu (`:863-893`, `:969-993`); then En_Fu goes idle with the auto-accepted talk (`phase == 2`, range 500).

### 5.6 Result texts, reward and the heart piece

After a **loss**, `phase` goes 0 -> 2 once the text is dismissed (`:867-873`), the auto-talk picks 0x2886 (time out) or 0x2889, then 0x2887 / 0x288A (DONE list, clears both kickout flags, `:616-634`). After a **win** the auto-talk branches on the unchanged `textId == 0x287D` (`:404-420`):

| Condition at the auto-talk | Text | Reward given (`func_80963560`, `:901-911`) |
| --- | --- | --- |
| player form is Deku | 0x287E | `GI_RUPEE_PURPLE` |
| Day 3, `22_10` and `22_20` set, `22_40` clear (and not Deku) | 0x2880 | `GI_HEART_PIECE` if `RECEIVED_HONEY_AND_DARLING_HEART_PIECE` is clear, else `GI_RUPEE_PURPLE` |
| Day 3, `22_10`, `22_20` and `22_40` set | 0x2883 | `GI_RUPEE_PURPLE` |
| anything else | 0x287E | `GI_RUPEE_PURPLE` |

Reward values (C): `GI_RUPEE_PURPLE` (`include/z64item.h:420`) -> `ITEM_RUPEE_PURPLE` 0x88 (`:331`) -> `sRupeeRefillCounts[0x88 - 0x84]` = 50 (`z_parameter.c:2543, :2856-2857`); `GI_HEART_PIECE` (`z64item.h:427`) -> table entry `ITEM_HEART_PIECE_2` (`z_player.c:2106`). Offered with `Actor_OfferGetItem(..., 500.0f, 100.0f)`.

After the item is taken (`Actor_HasParent`), `func_80963630` waits for a talk (`Actor_OfferTalkExchangeEquiCylinder`, range 500, `:918-967`). If `22_10`, `22_20`, Day 3 and Human: `22_40` set -> 0x2884; else the piece flag clear -> **set the flag**, 0x2882; else 0x2881; any other state -> 0x287F. Then, Human only, the day's flag is set (Day 1 `22_10`, Day 2 `22_20`, Day 3 `22_40`; `:944-961`). The DONE texts 0x287F, 0x2881, 0x2882, 0x2884, 0x2887, 0x288A clear `KICKOUT_WAIT` and `KICKOUT_TIME_PASSED` (`:620-629`).

Consequences (C unless marked): the piece needs three Human wins in one cycle (Day 1, Day 2, then Day 3) because `22_10`/`22_20`/`22_40` are cleared by every cycle reset and the item gate reads them; it is given once per save (flag persists); a Deku-form win never sets a day flag and never gives the piece; the three flags are set only after the reward is taken and the follow-up talk accepted (leaving the scene earlier loses the flag, INF); every win on a non-qualifying round repays the 10 rupee fee five-fold (50 rupees) and may be repeated.

### 5.7 The rotating platform (Bg_Fu_Kaiten, OVL `z_bg_fu_kaiten.c`)

| Item | Value | Source |
| --- | --- | --- |
| Update | each update: `shape.rot.y += rotationSpeed` (s16 binang); if `rotationSpeed > 0` plays `NA_SE_EV_WOOD_GEAR` at frequency `rotationSpeed * 0.002`; `bounce += bounceSpeed`; `world.y = home.y + elevation + bounceHeight - bounceHeight * cos(bounce)` | `:48-66` |
| Resulting height | `home.y + [elevation, elevation + 2 * bounceHeight]`: Day 1 and Day 3 [0, 80]; Day 2 [10, 70]; bounce period 65536 / 600 = 109.2 updates | derived from `:62-66`, `z_en_fu.c:655-747` |
| Rider transform | `DYNA_TRANSFORM_POS \| DYNA_TRANSFORM_ROT_Y`: standing actors are carried and turned | `:34` (Init) |
| Rotation per update | 100 = 0.55 deg, 200 = 1.10 deg, 300 = 1.65 deg, 400 = 2.20 deg, 500 = 2.75 deg | derived |
| Controller | En_Fu through `actor.child`: spin-up `Math_SmoothStepToS` (10, 5, 5) for rotation, (20, 10, 10) for bounce speed, `Math_SmoothStepToF` for heights; wind-down targets 0 (`:882-885`) | `z_en_fu.c:655-747`, `:863-894` |
| Geometry (C-BIN, 7.3) | deck floor at local Y 20, radius 171.1; pedestal top at local Y 49, radius 46.1 (En_Fu stands on it at Y 49); skirt walls down to Y -80 below the hall floor | `objcol.py` |

### 5.8 Targets, baskets and water

| Item | Rule | Source |
| --- | --- | --- |
| Day 3 target motion | each update: yaw and pitch toward the next path point, speed 2.0, next point when within 10 u (wraps to 0); faces En_Fu; only when `gameKind == 0` and `phase == 0` (during the round) | `z_en_fu_mato.c:124-164` |
| Target break | Human: 2 groups of 3 shards (`func_80ACE850`, `EffectSsHahen` burst, `NA_SE_EV_WOODPLATE_BROKEN`), shrinks after 60 updates; Deku or Day 1: spins and falls with gravity -0.5 (`func_80ACE680`), `playedBefore = 1`; `NA_SE_SY_TRE_BOX_APPEAR` on score | `:166-245`, `:281-353`, `:355-382` |
| Basket | catches a bomb: bomb fuse = 20, bomb pinned to basket x/z until fuse reads 1, then score and break (6 pieces, splash and ripple effects, collision disabled, shrink after 60 updates) | `z_en_fu_kago.c:145-345` |
| Water (Day 2) | target height = home Y + 25 while `unk_160 == 1` (round and spin-up), else home Y + 10 if any basket has health 0, else home Y - 10; smoothing 0.05; `NA_SE_EV_WATER_LEVEL_DOWN_FIX` while moving. The water is a dynapoly with one water box (-400, 0, -400), 800 x 880, properties 0x7FF00 = `WATERBOX_PROPERTIES(0, 0x1F, 0x3F, 0)` (bgCam 0, light none, all rooms, enabled) (C-BIN) | `z_bg_fu_mizu.c:48-81`; `z_en_fu.c:683-689, :851-857`; `include/z64bgcheck.h:33-46` |
| Wading / swimming at the water's height | NI | U-10 |

### 5.9 Honey and Darling: actor and poses (a02, OVL `z_en_fu.c`)

One actor draws both characters from one flex skeleton (`gHoneyAndDarlingSkel`, `object_mu` offset `0xB2B0`: 20 limbs, 17 display lists, C-BIN from scratch `mu.py`): man limbs (torso, two arms of 3 limbs each, head) and woman limbs (root, torso, two arms of 4 limbs each, head), plus legs. Position (0, 49, 0) = the pedestal top (5.7). Poses switch when the player advances past the listed text (`Message_ShouldAdvance`, `z_en_fu.c:1244-1246`).

| Pose | Animation (`object_mu` offset, frames C-BIN) | Morph | Selected when |
| --- | --- | --- | --- |
| rest and default | `CUP_CHEEKS` `gHoneyAndDarlingCupCheeksLoopAnim` (0x1F74, 90) loop | -4 | Init, every return to idle (`:390-393`); texts 0x2842, 0x2844, 0x2848 (`:1247-1251`) |
| holding hands | `HOLD_HANDS_MORPH` `...HoldHandsLoopAnim` (0x5304, 30) loop | -8 | texts 0x2840, 0x2841, 0x2843, 0x2847, 0x2849, 0x284B, 0x284D, 0x284F, 0x2851, 0x2853, 0x2855, 0x2857, 0x2859, 0x285B, 0x285D, 0x2861, 0x2863, 0x2865, 0x2867, 0x2869, 0x286B, 0x286D, 0x2871 (`:1253-1277`) |
| holding hands (no blend) | `HOLD_HANDS` (same clip) | 0 | text 0x2860 (`:1279-1281`) |
| surprise | `SURPRISE` `gHoneyAndDarlingSurpiseAnim` (0xBAC4, 24) once | 0 | text 0x285F (`:1283-1285`) |
| hug | `HUG` `...HugLoopAnim` (0x2F64, 49) loop | -4 | rewards: texts 0x287E, 0x2880, 0x2883 (`:1287-1291`) |
| game dance | `GAME_DANCE` `...GameDanceLoopAnim` (0x4904, 80) loop | 0 | start text 0x287D (`:558`); runs through the round and wind-down |
| idle clip | `IDLE` `gHoneyAndDarlingIdleAnim` (0x53E0, 2) | -4 | never selected (the enum and table only; grep `HONEY_DARLING_ANIM_IDLE`, `:76, :87`) |

| Behaviour | Rule | Source |
| --- | --- | --- |
| Yaw at rest | spawn yaw 0 + 0x4000; settles to platform yaw - 0x4000 (-90 deg) with `Math_SmoothStepToS(..., 10, 3000, 0x64)` | `:222, :442` |
| Yaw while talking | toward the player minus 0x4000, `(..., 5, 0x3E8, 0x64)` | `:1239-1242` |
| Yaw while the round runs | `+= 0x400` per update in the five game states (they turn with the platform) | `:1230-1237` |
| Heads | man's head tracks the player while `headFlags & 1`, woman's while `& 2`; set per text id (27 man texts, 18 woman texts); tracking clamp 0x38E3 (80 deg), released beyond 0x6AAA (150 deg, man) or 0x5555 (120 deg, woman); pivots in `EnFu_OverrideLimbDraw` | `:321-348, :1299-1358, :1383-1412` |
| Focus point | (0, 60, -8), (0, 55, 12) or (0, 60, 0) by which head is active, rotated by yaw | `:71-73, :371-387` |
| Hearts | every 12 updates a heart particle spawns at position + (0, 62, 0) (+ 4 along yaw), 10-slot pool, rises 1.5 per update for 16 updates, sways, drawn billboard at scale 0.01 with `gHoneyAndDarlingHeartModelDL` and `gDropRecoveryHeartTex`; runs in every state | `:1218-1228, :1435-1507` |

### 5.10 Door schedule, open hours and kickout

| Item | Value | Source |
| --- | --- | --- |
| Door type | TA0 `ENDOOR_TYPE_SCHEDULE`, var 9 -> `sDoorSch_HoneyDarlingShop` | 2.3; `z_en_door.c:49-82` (entry at `:59`) |
| Script | `if_since_time (22, 0) { return_s (0x11) } else { return_none }` | `ovl_En_Door/scheduleScripts.schl:183-189` |
| Time test | `if_since_time` compiles to `SCHEDULE_CMD_CHECK_BEFORE_TIME_*` (branch to `else` when `now < 22:00`); `now = (u16)(CURRENT_TIME - 16380)` and the test time is `(u16)(60060 - 16380)` = 43680 (`SCRIPT_CONVERT_TIME` subtracts `0x10000 / 360 * 90` = 16380) | `tools/buildtools/schc.py:167, :179`; `src/code/z_schedule.c:188-203`; `include/z64scripts.h:6-17` |
| Result | open for `CURRENT_TIME` 16380 .. 60059; **closed for 60060 .. 65535 and 0 .. 16379** (about 21:59:41 to 05:59:55; the u16 wrap closes it through the small hours, not only until midnight) | derived (scratch arithmetic) |
| Closed response | text `0x1800 + 0x11` = 0x1811, door stays shut (`PLAYER_DOORTYPE_TALKING`) | `z_en_door.c:537-546` |
| Where checked | street side only (East TA4); never from inside (2.3) | `z_en_door.c:537` |
| Day 1 12:00 | **open**; first closed profile is `prof.d1.late.normal` (23:00) | C |
| Kickout | a00 at hour 22, minute 0, not in cutscene mode: halts the player (`Player_SetCsActionWithHaltedActors(PLAYER_CSACTION_WAIT)`), text `0x1883 + 3` = 0x1886, then in `TEXT_STATE_EVENT` `nextEntrance = setupExitList[0]` = `0xD260`, door sfx | `z_en_time_tag.c:333-338, :285-294, :303` |
| Kickout window | a single clock minute (45.5 units; about 15 updates at speed 3): `hour == 22 && minute == 0`; if the player is in cutscene mode the whole minute can be missed and no kickout happens (INF from the equality test) | `z_en_time_tag.c:324-338` |
| Kickout hold | while `KICKOUT_WAIT` is set (a round started, 5.4): 22:00 only sets `KICKOUT_TIME_PASSED`; the tag then switches to `WaitForTrigger` and ejects as soon as En_Fu clears both flags (DONE texts, 5.6) | `z_en_time_tag.c:297-312, :327-332`; `z_en_fu.c:616-634` |
| Staff presence | Honey and Darling D0d..D3d; kickout tag D0d..D2n | masks, 4.2 |
| Result | open every half-day on Days 0 to 3 until 21:59; from Day 3 18:00 the room is open but empty of En_Fu; closed 22:00 to 06:00 | C, INF |

### 5.11 Cameras, cutscenes, lighting, sound, time

| Item | Value | Source |
| --- | --- | --- |
| BgCam list | 0: `0x0000` `CAM_SET_NONE` (walls, ceilings, corridor, exit pad); 1: `0x004B` `CAM_SET_HONEY_AND_DARLING_2` "BOMBBOWL", used by every hall floor poly (surface types 1 and 2) | HDR scene.c:177-180; `z64camera.h:147, :222` |
| Actor cs camera 0 | `CAM_SET_HONEY_AND_DARLING_1` (0x3A) "BOMBBASKET", 3 points: (-1, 199, -2), (0, 0, 0), (-1, -1, -1); used by cutscene entry 9 (a02 csId 9) | HDR scene.c:134-142; `z64camera.h:205`; `z_en_fu.c:749-756` |
| Cutscene entry 9 | `{ priority 900, length -1, csCamId 0, scriptIndex -1, additionalCsId -1, endSfx none, customValue 255, hudVisibility 0, endCam 0, letterbox 0 }` (field order `include/z64cutscene.h:626-637`) | HDR scene.c:123 |
| Cutscene entries 0 to 8 | priorities 700/600/700/700/500/400/100/200/800, csCamIds -3, -2, -4, -5, -7, -11, -8, -9, -6 (global camera settings), `additionalCsId` chain 1..8 and -1; no room actor has csId 0 to 8 (the player spawn rotY gives csId 0, 2.1: Player use NI) | HDR scene.c:114-122; U-04 |
| Lighting | `LIGHT_MODE_SETTINGS`; every surface type has light index 0 (`data[1] >> 6 & 0x1F`); `Environment_ChangeLightSetting` is called only for scene floors, not for platform surfaces (`z_player.c:11728-11731`), so the setting stays at index 0 of the 4 (INF; settings 1 to 3 are unreferenced by scene data) | HDR scene.c:182-185; `z_bgcheck.c:4265-4267`; `z_kankyo.c:2169-2191` |
| Music | scene seq 0; Obj_Sound plays `NA_BGM_SHOOTING_GALLERY` from (3, 382, -272); `NA_BGM_TIMED_MINI_GAME` as sub BGM during rounds | 4.2; `z_en_fu.c:559` |
| Three-day controller | a03: day/night swap with no cutscene (csId none); on Day 3, when the 00:00 bell is due and the Ocarina slot is empty, it sends the player to South Clock Town with `nextCutsceneIndex` 0xFFF1 (not read further) | `z_en_test4.c:495-526` |

### 5.12 Player-side hooks (for the index only)

| Hook | Behaviour | Source |
| --- | --- | --- |
| Free explosives | when `unk_1887D`/`unk_1887E` is nonzero the counter is decremented instead of the item ammo; active explosive cap 5 (3 normally) | `z_player.c:3432-3444, :4557-4563` |
| Free arrows | `PLAYER_STATE3_400`: ammo reads 1 and is not decremented | `z_player.c:3981-3982, :4292`, cleared `:12845` |
| Deku bubbles | magic check bypassed in `SCENE_BOWLING` while `08_01` is set | `z_player.c:3975-3979, :4038-4040` |
| Invulnerability | `PLAYER_STATE3_400000` makes the damage function return early | `z_player.c:5711-5713` |
| HUD | in `SCENE_BOWLING` the B button item is forced by day (`CURRENT_DAY` 1 `ITEM_BOMBCHU`, 2 `ITEM_BOMB`, otherwise `ITEM_BOW`) and the C buttons are disabled (`z_parameter.c:2247-2258, :2304-2312`); `08_01` alone disables C and shows A, B and minimap (`:2290-2294, :2347-2351`); Deku with `08_01` here: B enabled, C disabled, `HUD_VISIBILITY_B_MAGIC` (`:2221-2237`); the B ammo count is not drawn in this scene (`:4265-4272`) | `z_parameter.c:2209-2312, :4259-4272` |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header)

The venue is **open** at Day 1 12:00 (the schedule runs only from the street and returns none before 21:59, 5.10), so the baseline is also the first open profile. Assumptions: fresh save (all flags clear, so the piece is available and `22_10` is clear), Human form, no mask, platform at rest (rotation 0, bounce 0, elevation 0), En_Fu settled at its rest yaw. All five room actors pass the mask; the three controller actors have no geometry. World = actorPos + actorRot x (actorScale x localVertex) except where noted.

| Key | Component / params | N64 position (Unity) | Yaw | Scale | DLs, bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room_00 | room shell | origin | 0 | 1 | opaque `0x210`, `0xB50`, `0xEF8`, `0x1218`, `0x1728`, `0x1E40`, `0x2378`, `0x2610`, `0x4900`, `0x528` (cullable entries; wrapper `0x4CF0` is unreferenced) | 397 triangles, no XLU; animated materials on segments 8-12 (section 8) |
| a01 | platform, `0x000F` | (0, 0, 0) (0.00, 0.00, 0.00) | 0 | 1.0 | `object_fu_kaiten` `0x5D0`; dyna collision `0x2D30` | deck Y 20, pedestal Y 49 at rest; rotation and bounce unsupported until a controller exists |
| a02 | Honey and Darling, `0x0000` | (0, 49, 0) (0.00, 0.49, 0.00) | rest yaw -90 (270) | 0.01 | `object_mu` skeleton `0xB2B0`, `CUP_CHEEKS` loop frame 0 | skeletal pose deferred; hearts optional |
| a02/mato0..mato7 | 8 targets, params 0..7, yaw toward the centre | Path[2] points 0..7: (-2, 140, 297) (-0.02, 1.40, -2.97); (209, 200, 211) (2.09, 2.00, -2.11); (292, 140, 2) (2.92, 1.40, -0.02); (212, 200, -208) (2.12, 2.00, 2.08); (0, 140, -290) (0.00, 1.40, 2.90); (-212, 200, -210) (-2.12, 2.00, 2.10); (-294, 140, 2) (-2.94, 1.40, -0.02); (-212, 200, 208) (-2.12, 2.00, -2.08) | 180, 225, 270, 314, 0, 45, 90, 134 | 0.1 | `object_fu_mato` `0x2720`; dyna collision `0x23D4` | intact state (draw `:472-473`); spawned at Init, present before any play |
| ta0 | door `0x0289` | (0, 0, 342) (0.00, 0.00, -3.42) | 0 | default | gameplay_keep `gDoorSkel` `0x22B28`, leaves `0x20BB8` / `0x20D00` | closed pose; leaf side by eye yaw |

**Excluded by mask:** none at Day 1 daytime. **No geometry:** a00 En_Time_Tag, a03 En_Test4, a04 Obj_Sound. **Not present on Day 1:** the 8 baskets and Bg_Fu_Mizu (Day 2), the 16 moving targets (Day 3).

Other profiles on header 0 (same room shell): `prof.d1.night.normal` (21:00) identical to this list; `prof.d1.late.normal` (23:00) door closed, kickout already fired (the player is outside); `prof.d2.noon.normal` replaces the 8 targets by 8 baskets (Path[1]) plus Bg_Fu_Mizu at water height -10; `prof.d3.noon.normal` has 16 moving targets (Path[0]) and no kickout tag; `prof.d3.night.normal` has the platform only (no En_Fu, no targets).

Extraction commands are in section 9 (not executed).

## 7. Collision summary (C from scene.c:182-395; scratch `col.py`, `col2.py`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 71 / 121 / 5 |
| Water boxes | 0 (`0, NULL`, scene.c:392) |
| Header bounds | min (-322, 0, -322), max (322, 427, 442); Unity x -3.22..3.22, y 0..4.27, z -4.42..3.22 |
| Vertex extents | x -322..322, y 0..427, z -322..442 (same as the header) |
| Polygon kinds | 45 floors (all flat, Y 0), 48 walls (20 vertical octagon walls Y 0..140, 12 vertical corridor walls Y 0..100, 16 inward-leaning upper walls Y 140..427), 28 ceilings (24 dome ceiling polys at Y 408, 4 corridor ceilings at Y 100) |
| Materials (`data[1] & 0xF`) | dirt (type 0): 76 polys (walls and ceilings); stone (types 1, 3, 4): 33 floors; carpet (type 2): 12 floors |
| Exit surfaces | 1 type, 2 floor polys (2.4) |
| Other flags | no floor type, wall type, floor property, hookshot, conveyor or soft bits on any type; echo 2 and light index 0 on all five; poly exclusion: 18 ignore-camera polys (corridor walls 18, 19, 22, 23, 53, 54, 69-74, ceilings 20, 21, 71, 72, floors 91-94) |
| Camera data | 2 BgCamInfo (5.11); floors of types 1 and 2 use index 1, everything else index 0 |

### 7.1 Floor layout (C)

| Type | Surface | Polys | Radius range (from the origin) | xz area |
| --- | --- | --- | --- | --- |
| 1 | stone, bgCam 1 | 29 (24-29, 33-37, 44-46, 50-52, 77-88) | 0..350 | 241,284 u^2 |
| 2 | carpet, bgCam 1 | 12 (30-32, 38-43, 47-49) | 207..350 | 105,740 u^2 |
| 3 | stone, bgCam 0, exit 0 | 2 (91, 92) | 343..443 | 6,000 u^2 |
| 4 | stone, bgCam 0 | 2 (93, 94) | 323..343 | 1,200 u^2 |

The hall floor is one flat octagon at Y 0 (apothem 322, circumradius 350; corner vertices (+-138, +-322), (+-322, +-138)) with an inner ring of vertices at radius 205..208; stone and carpet tile it. The platform sits over the centre (7.3). Floor level under every room actor's XZ is Y 0 (poly 28 under a01 and a02, poly 91 under a03, poly 45 under a04; a00 lies inside the octagon).

### 7.2 Walls, ceilings, special features

- Lower octagon: 20 vertical wall polys, Y 0..140 (`0-17, 89, 90`: 7 sides of 2 polys and the z = 322 side as 6 polys, 12-17, with a doorway opening x -30..30 up to Y 100, lintel 14, 15 at Y 100..140). The corridor mouth is closed again by 95, 96.
- Upper dome: 16 leaning wall polys (55-68, 75, 76; normals with |ny| about 0.10) from the octagon top (Y 140) to a ring of 8 vertices at Y 427 (radius 320), then 24 ceiling polys at Y 408 (97-120).
- Corridor: 12 vertical polys (18, 19, 22, 23, 53, 54, 69, 70, 73, 74 and the closing wall 95, 96), section 2.3. No ladders, hookshot surfaces, conveyors, void-out polys or sloped floors.
- The mouth wall 95, 96 (z = 322) is the only collision between the hall and the door (2.3).

### 7.3 Dynapoly owners and object collision headers (C-BIN; owners: a01 Bg_Fu_Kaiten always, Bg_Fu_Mizu Day 2, En_Fu_Mato Days 1 and 3, En_Fu_Kago Day 2 and disabled when broken, `z_en_fu_kago.c:229`; method: scratch `objcol.py`, `walls.py` read the baserom object binaries at the XML offsets, `CollisionHeader` layout `include/z64bgcheck.h:291-302`; numbers only)

| Header | Bounds min / max | Vertices / polys | Floors / walls / ceilings | Notes |
| --- | --- | --- | --- | --- |
| platform `object_fu_kaiten:0x2D30` | (-165, -80, -165) / (165, 49, 165) | 41 / 58 | 18 / 40 / 0 | deck floor Y 20, 10 polys, radius up to 171.1; pedestal top Y 49, 8 polys, radius up to 46.1; pedestal walls Y 20..49 at radius 46 (16 polys, `wallType` 1 = `WALL_FLAG_0`, `z_bgcheck.c:20`); skirt walls Y -80..20 at radius 171 (24 polys); wood, light 31, echo 0 |
| water `object_fu_kaiten:0x37F8` | (-400, 0, -400) / (400, 0, 480) | 0 / 0 | n/a | 1 water box (-400, 0, -400), x 800, z 880, properties 0x7FF00 |
| basket `object_fu_mato:0x15C0` (scale 0.1) | (-200, 0, -223) / (200, 240, 223) | 24 / 44 | 16 / 24 / 4 | basket floor Y 40 (4 polys, radius up to 183.4); rim ring at Y 240 |
| target `object_fu_mato:0x23D4` (scale 0.1) | (-173, -200, 0) / (173, 200, 120) | 14 / 24 | 4 / 16 / 4 | |

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 8 textures, 0 TLUTs: `000CA0` (rgba16 32x32), `0014A0` (rgba16 16x16), `0016A0` (rgba16 32x32), `001EA0` (i4 32x32), `0020A0` (i4 64x64), `0028A0` (rgba16 16x64), `0030A0` (rgba16 32x32), `0038A0` (rgba16 32x32) |
| By format | rgba16 6, i4 2 |
| Location / use | all in the scene file (segment 2); all 8 referenced by room DLs; sizes from the `gsDPLoad*` calls |
| Room `Gfx` DLs / `Vtx` arrays | 11 / 10 (10 `.vtx.inc`, 983 vertex entries by line count) |
| Raw triangles | 397 (2 + 24 + 48 + 16 + 15 + 32 + 42 + 32 + 6 + 180 per DL) |
| Render modes | all ten drawing DLs `AA_ZB_OPA_SURF2`; wrapper `0x4CF0` has none; no XLU DL |
| Animated materials (5, two-tile scroll) | segment 8: tile 1 (64x64) y -2 per update, used by `0x528`; segment 9: tile 1 x +1, used by `0xB50`, `0xEF8`, `0x1218`, `0x1728`; segment 10: tile 1 y -1, tile 0 is 16x64, used by `0x1E40`; segment 11: tile 1 (32x32) y +4, used by `0x2378`; segment 12: tile 0 (16x16) y -1, used by `0x4900` (HDR scene.c:144-175; per-DL segment use from `gsSPDisplayList(0x0N000000)` calls) |
| Billboards | none in the room DLs (hearts are actor-drawn, 5.9) |
| Object-bank textures (XML) | fu_kaiten 1 TLUT + 5 textures (1 ci4, 4 rgba16, incl. water); fu_mato 3 rgba16; mu 1 TLUT + 11 (9 ci8, 2 i8) |

Cullable entries (HDR room.c:58-69; triangle counts C):

| # | Centre / radius | DL (tris) |
| --- | --- | --- |
| 0 | (0, 50, 442) / 60 | `0x210` (2) corridor floor |
| 1 | (0, 0, 0) / 208 | `0xB50` (48) |
| 2 | (0, 0, 0) / 352 | `0xEF8` (16) |
| 3 | (0, 0, 60) / 408 | `0x1218` (15) |
| 4 | (0, 283, 0) / 380 | `0x1728` (32) |
| 5 | (0, 70, 10) / 368 | `0x1E40` (42) |
| 6 | (0, 213, 0) / 390 | `0x2378` (32) |
| 7 | (0, 50, 392) / 78 | `0x2610` (6) |
| 8 | (0, 342, 0) / 52 | `0x4900` (180) |
| 9 | (0, 408, 0) / 330 | `0x528` (24) |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_BOWLING` C (all textures, DLs, collision). Binaries present under `extracted/n64-us/baserom/` (checked by `ls`): object_fu_kaiten 14384 B, object_fu_mato 12880, object_mu 47840. `gameplay_keep` is shared with the other venues (not yet extracted).

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_fu_kaiten`, `object_fu_mato`, `gameplay_keep` | platform, targets, door, heart texture |
| Later skeletal pass | `object_mu` | Honey and Darling skeleton and 6 animations |
| Day 2 / Day 3 variants | `object_fu_mato` (baskets), `object_fu_kaiten` (water) | already in the same banks |
| Text (U-02) | `assets/text/message_data.h` | texts 0x283C..0x288B, 0x1886, 0x1811 (stub in this checkout) |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_fu_kaiten','object_fu_mato','object_mu')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

## 10. Proposed region record

Schema 1 (frozen at `7a7a08c`). Interior with its own frame, so `sharedFrame` is `not_shared`. `portalIds` are exactly the registry's departing portals of `loc.honey_darling`. Scratch checks of this block (JSON parse, key set and patterns against `region.schema.json`, ID membership in `ids.json`) are listed in section 11; the integrator's `region_from_dossier.py` and `validate_city.py` were not run.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.honey_darling",
  "displayLabel": "Honey & Darling's Shop",
  "source": {
    "sceneSymbol": "Z2_BOWLING",
    "sceneEnum": "SCENE_BOWLING",
    "sceneTableIndex": "0x11",
    "entranceTableSceneIndex": "0x04",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "halfDayBit": "0x080",
    "stateAssumptions": [
      "fresh save: no week-event flags (heart piece available, 22_10 clear)",
      "player form Human, no mask",
      "platform at rest: rotation 0, bounce 0, elevation 0",
      "Honey and Darling pose UNKNOWN until captured (CUP_CHEEKS loop, settled yaw 270)",
      "no alternate scene header exists; Day 2 swaps targets for baskets and adds water, Day 3 uses 16 moving targets"
    ],
    "sceneLayer": 0
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_BOWLING", "bytes": 16544, "sha256": "352be13629c2b29f1c1c64638c4f82f6344c33c574f8b1c74d74c4b61909c20a" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_BOWLING_room_00", "bytes": 19792, "sha256": "d9c0024f74f589cc9f61039b6d3bb472c3fc076c5fa8038e31cf49ea67d0c0b6" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_BOWLING/Z2_BOWLING.c", "sha256": "55a0e260b9948712bbfb70493b55bb605216e100f663452a5be3898922efa7ec" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_BOWLING/Z2_BOWLING_room_00.c", "sha256": "0dffcab482d20b3bd920f581345d412342fb1d74f6d1c6b3914b05c105b5ed9d" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_BOWLING/Z2_BOWLING.xml", "sha256": "919755e1aa1285d82d2cedc2e7eab035c9abf2323bfd55742e9667876185cad9" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/object_fu_kaiten", "bytes": 14384, "sha256": "822d8dbe84c2330fabf2cdc0398635a7f622be3e249861b8dd6c2b8f22bcc807" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/object_fu_mato", "bytes": 12880, "sha256": "c752d2467802596d49dcbf4aab8672d15efef942c42d51195b7de0d1cf695d4c" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/object_mu", "bytes": 47840, "sha256": "c1e53f61e4933218d8b036616a492b2c1a59a11f39308ab7aab5d3e5678642bf" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": [],
      "status": "not_shared",
      "evidence": "interior scene with its own origin; East door at Y 100 vs interior floor Y 0 (section 2.5)"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-322, 0, -322], "max": [322, 427, 442] },
    "sourceCollisionVertices": { "min": [-322, 0, -322], "max": [322, 427, 442] },
    "unityCollisionVertices": { "min": [-3.22, 0, -4.42], "max": [3.22, 4.27, 3.22] }
  },
  "anchors": [
    { "id": "anchor.hd.spawn0_from_east", "source": [0, 0, 300], "unity": [0, 0, -3.0], "status": "proposed" },
    { "id": "anchor.hd.door_ta00", "source": [0, 0, 342], "unity": [0, 0, -3.42], "status": "proposed" },
    { "id": "anchor.hd.exit_pad_centroid", "source": [0, 0, 392], "unity": [0, 0, -3.92], "status": "proposed", "note": "centroid of exit polys 91, 92" },
    { "id": "anchor.hd.pedestal_top", "source": [0, 49, 0], "unity": [0, 0.49, 0], "status": "proposed", "note": "En_Fu a02 position; platform at rest" },
    { "id": "anchor.hd.deck_rim", "source": [171, 20, 0], "unity": [1.71, 0.2, 0], "status": "proposed", "note": "platform deck edge at rest (C-BIN radius 171.1)" }
  ],
  "rooms": [
    {
      "id": "loc.honey_darling.room_00",
      "sourceRoom": "Z2_BOWLING_room_00",
      "header": 0,
      "roomShape": "cullable, 10 entries",
      "displayLists": 11,
      "role": "only room: octagonal hall with the rotating platform, corridor and door behind",
      "collisionLightIndex": 0
    }
  ],
  "components": [
    {
      "sourceKey": "Z2_BOWLING/h0/room_00/a01",
      "actorId": "0x1AE",
      "params": "0x000F",
      "class": "static-mesh",
      "dls": ["object_fu_kaiten:0x5D0"],
      "status": "selected",
      "note": "rotating platform, dyna collision 0x2D30; rest pose only (rotation and bounce are driven by En_Fu)"
    },
    {
      "sourceKey": "Z2_BOWLING/h0/room_00/a02",
      "actorId": "0x0B5",
      "params": "0x0000",
      "class": "npc-scheduled",
      "dls": ["object_mu:0xB2B0 gHoneyAndDarlingSkel"],
      "status": "optional",
      "note": "Honey and Darling, one flex skeleton, mask D0d..D3d; skeletal pose deferred; spawns the day's targets"
    },
    { "sourceKey": "Z2_BOWLING/h0/room_00/a02/mato0", "actorId": "0x1B3", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["object_fu_mato:0x2720"], "status": "selected", "note": "Day 1 target at path 2 point 0 (-2, 140, 297), yaw 180" },
    { "sourceKey": "Z2_BOWLING/h0/room_00/a02/mato1", "actorId": "0x1B3", "params": "0x0001", "class": "static-mesh", "variant": "spawned", "dls": ["object_fu_mato:0x2720"], "status": "selected", "note": "Day 1 target at (209, 200, 211), yaw 225" },
    { "sourceKey": "Z2_BOWLING/h0/room_00/a02/mato2", "actorId": "0x1B3", "params": "0x0002", "class": "static-mesh", "variant": "spawned", "dls": ["object_fu_mato:0x2720"], "status": "selected", "note": "Day 1 target at (292, 140, 2), yaw 270" },
    { "sourceKey": "Z2_BOWLING/h0/room_00/a02/mato3", "actorId": "0x1B3", "params": "0x0003", "class": "static-mesh", "variant": "spawned", "dls": ["object_fu_mato:0x2720"], "status": "selected", "note": "Day 1 target at (212, 200, -208), yaw 314" },
    { "sourceKey": "Z2_BOWLING/h0/room_00/a02/mato4", "actorId": "0x1B3", "params": "0x0004", "class": "static-mesh", "variant": "spawned", "dls": ["object_fu_mato:0x2720"], "status": "selected", "note": "Day 1 target at (0, 140, -290), yaw 0" },
    { "sourceKey": "Z2_BOWLING/h0/room_00/a02/mato5", "actorId": "0x1B3", "params": "0x0005", "class": "static-mesh", "variant": "spawned", "dls": ["object_fu_mato:0x2720"], "status": "selected", "note": "Day 1 target at (-212, 200, -210), yaw 45" },
    { "sourceKey": "Z2_BOWLING/h0/room_00/a02/mato6", "actorId": "0x1B3", "params": "0x0006", "class": "static-mesh", "variant": "spawned", "dls": ["object_fu_mato:0x2720"], "status": "selected", "note": "Day 1 target at (-294, 140, 2), yaw 90" },
    { "sourceKey": "Z2_BOWLING/h0/room_00/a02/mato7", "actorId": "0x1B3", "params": "0x0007", "class": "static-mesh", "variant": "spawned", "dls": ["object_fu_mato:0x2720"], "status": "selected", "note": "Day 1 target at (-212, 200, 208), yaw 134" },
    {
      "sourceKey": "Z2_BOWLING/h0/transition/ta0",
      "actorId": "0x005",
      "params": "0x0289",
      "class": "door-transition",
      "variant": "posed-skeletal",
      "dls": ["gameplay_keep:0x22B28", "gameplay_keep:0x20BB8", "gameplay_keep:0x20D00"],
      "status": "selected",
      "note": "closed pose; schedule type HONEY_DARLING_SHOP, closed from the street 22:00 to 06:00"
    }
  ],
  "collisionOwnership": {
    "room": "Z2_BOWLINGCollisionHeader_000C74 (121 polys)",
    "waterBoxes": 0,
    "actorDynapoly": [
      "a01 Bg_Fu_Kaiten object_fu_kaiten:0x2D30 (58 polys, moving)",
      "Bg_Fu_Mizu object_fu_kaiten:0x37F8 (no polys, 1 water box, Day 2 only)",
      "En_Fu_Mato object_fu_mato:0x23D4 (24 polys each, Days 1 and 3)",
      "En_Fu_Kago object_fu_mato:0x15C0 (44 polys each, Day 2)"
    ],
    "actorCylinders": [
      "En_Fu cylinder radius 20 height 50 (AC on Day 3 only)",
      "En_Fu_Mato sphere radius 30 (Day 1) or 17 (Day 3)",
      "En_Fu_Kago sphere radius 10"
    ],
    "note": "dyna collision headers decoded from the baserom object binaries (C-BIN), not from extracted C"
  },
  "portalIds": [
    "portal.honey_darling.east_clock_town",
    "portal.honey_darling.east_clock_town.kickout"
  ],
  "materialBindings": null,
  "cameraIds": [
    "cam.honey_darling.entry",
    "cam.honey_darling.reverse_door",
    "cam.honey_darling.fixture"
  ],
  "routeIds": [],
  "omissions": [
    "a02 skeletal Honey and Darling (6 animations)",
    "platform rotation, bounce and wood-gear sound",
    "heart particles around a02",
    "animated materials on segments 8-12",
    "Link and Tatl"
  ],
  "excluded": {
    "halfDayMask": [],
    "stateOrSchedule": [
      "En_Fu_Kago x8 and Bg_Fu_Mizu (Day 2 only)",
      "En_Fu_Mato x16 moving targets (Day 3, 8 for Deku)",
      "target break, basket break and fall animations"
    ],
    "noGeometry": ["a00", "a03", "a04"]
  },
  "unsupported": [
    "three daily games: fee, 60 s timer, score, win flags, heart piece and purple rupee rewards",
    "free Bombchus, bombs, arrows and Deku bubbles; invulnerability during rounds",
    "22:00 kickout (En_Time_Tag) and door schedule closed 22:00 to 06:00",
    "positional BGM NA_BGM_SHOOTING_GALLERY and sub BGM NA_BGM_TIMED_MINI_GAME",
    "actor cs camera 0 (BOMBBASKET) and bgCam 1 (BOMBBOWL) behaviour",
    "En_Test4 three-day controller"
  ],
  "evidence": ["docs/city/interiors/honey_darling.md"],
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
    "sourceInspection": "scene and room C read in full; overlay mechanics read for En_Fu, En_Fu_Mato, En_Fu_Kago, Bg_Fu_Kaiten, Bg_Fu_Mizu, En_Time_Tag, En_Door schedule; object collision decoded from baserom binaries (C-BIN)"
  }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Exit index 1 `0x0000` and entrance row 1 are padding (INF) | Does any cutscene command or global code index `setupExitList[1]` or `EntranceEntry[1]` for `SCENE_BOWLING`? No reader in the nine overlays here (2.2); record padding unless one is found |
| U-02 | Text content for En_Fu ids 0x283C..0x288B, kickout 0x1886 and door-closed 0x1811; `assets/text/message_data.h` is a stub here | Extract the message data and confirm that 0x2871 is the 10 rupee choice and what 0x288B, 0x1886, 0x1811 say; confirm each DONE text's end type (5.6) |
| U-03 | Camera poses of `CAM_SET_HONEY_AND_DARLING_1` (0x3A, data (-1, 199, -2)) and `_2` (0x4B, no data) | Read `sCameraSettings` entries 0x3A and 0x4B and their mode functions in `src/code/z_camera.c` to turn them into `cam.honey_darling.*` hints |
| U-04 | Users of cutscene entries 0 to 8 and of the player's spawn csId 0 | Grep `CutsceneManager_*` with scene cs ids in En_Fu and Player; check whether Player reads `this->actor.csId` at spawn |
| U-05 | How the player passes the mouth wall polys 95, 96 (z = 322) to reach the door at z = 342 | Read the door-open trigger range and Player door action (`z_en_door.c:485-536`, `z_player.c:6756-6790`) against the 20 u gap; capture `cam.honey_darling.reverse_door` |
| U-06 | Exact En_Fu pose to capture (animation frame, yaw at the capture instant) | Capture policy choice (REF-03): frame 0 of `CUP_CHEEKS` at settled yaw 270, or later |
| U-07 | Whether `envCtx.adjLightSettings` is ever nonzero here (the post-game fade loop `func_809638F8`) | Grep writers of `adjLightSettings` reachable in `SCENE_BOWLING` (`z_demo.c:173-257` runs in cutscenes only; none found in the scene's actors) |
| U-08 | Which colliders count as a Day 1 target hit (`OC1_HIT`, any OC type) | Read `CollisionCheck_OC` pairing rules and the Bombchu collider (`ovl_En_Bom_Chu`); confirm only Bombchus and not the Player or bombs reach the targets |
| U-09 | Day 3 18:00 removal of En_Fu and survival of its children | Confirm `Actor_KillAllOnHalfDayChange` deletes a02 while targets (mask ALL, `Actor_Spawn`) stay, and what `EnFu_Destroy` clears (`z_en_fu.c:246-252`) |
| U-10 | Player behaviour in the Day 2 water (height up to +25 over the hall floor) and `WALL_TYPE_1` meaning on the pedestal | `z_bgcheck.c` dyna water, `z_player.c` water depth and wade thresholds; `sWallFlags[1]` consumers |
| U-11 | Who clears `actorCtx.isOverrideInputOn` after the post-game walk (`z_en_fu.c:841, :986`) | Full grep of `isOverrideInputOn` including `z_actor.c` and `z_play.c` |
| U-12 | Meaning of `PLAYER_STATE1_20` as used for the spin-up halt | Read the flag's consumers in `z_player.c` (it appears in `:12903`) |
| U-13 | `Obj_Sound` id 0x46 = `NA_BGM_SHOOTING_GALLERY` is by table order | Confirm `Audio_PlayObjSoundBgm` indexes `gSequenceTable` ordinal, not a sub-table |
| U-14 | Where the form's B-button item, overwritten with Bombchu / bomb / bow while `08_01` holds (`z_parameter.c:2247-2258, :2304-2312`), is restored after the round | Trace `BUTTON_ITEM_EQUIP` writers and the restore path when `WEEKEVENTREG_08_01` clears (`z_parameter.c` HUD update, `z_en_fu.c:851`) |

Register wording notes (not edits): PORTAL_REGISTER "open hours UNKNOWN" on both Honey and Darling door rows is resolved here (closed 21:59 to 05:59 from the street, open otherwise; 5.10). The East dossier's "open (closed since 22:00, :183-189)" is correct but incomplete: the script's `u16` wrap keeps the door closed through the small hours until 06:00. LOCATION_REGISTER counts agree (3/5, 1 transition, 1 spawn, 2 exits with trailing 0x0000, 2/1 entrances) and its U-02 is answered for this scene (2.2: padding).

**Not inspected:** room DL contents and combiners; light-setting values; minimap draw; message text; En_Fu `EnFu_PostLimbDraw` pivots in detail; heart particle motion beyond 5.9; the Day 3 midnight festival path; collision-check pairing details; camera function bodies; 3DS material.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN sections 6/11/12, the three registers, North and East templates, the Bomb Shop and Lottery Shop dossiers and the Bomb Shop record, `shared/city/schema/README.md` and `region.schema.json`, `ids.json`; read scene/room C and XML, object XMLs, scene/actor/object/sequence tables, `z_actor.c`, `z_scene.c`, `z_scene_table.c`, `z_play.c`, `z_camera.c`, `z_parameter.c`, `z_sram_NES.c`, `z_schedule.c`, `z_scene_proc.c`, `z_bgcheck.c`, `z_kankyo.c` (adjustment and light calls), `z64bgcheck.h`, `z64player.h`, `z64door.h`, `z64scripts.h`, `z64cutscene.h` and the overlays En_Fu, En_Fu_Mato, En_Fu_Kago, Bg_Fu_Kaiten, Bg_Fu_Mizu, En_Time_Tag, En_Test4, Obj_Sound, En_Door (schedule, object info) plus player hooks. Scratch scripts in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\honey_darling\`: `col.py` (collision, door probe, actor floors, paths), `col2.py` (floor types, wall groups, flag polys), `dls.py` (DL and texture inventory), `objcol.py` and `walls.py` (object collision headers, C-BIN), `mu.py` (skeleton and animation headers, C-BIN), `paths.py` (path geometry), `seq.py` (sequence ordinals), `poly_dump.py` (doorway polys), `check.py` (record check: ASCII clean, JSON parses, 0 errors against a subset reading of `region.schema.json`, regionId, rooms, profile and cameras found in `ids.json`, `portalIds` equal to the registry's departing portals). SHA-256 of eight inputs and the ZAPD binary recorded in section 10; grep of the nine overlays for `setupExitList`; grep of `HONEY_DARLING_ANIM_IDLE`, `adjLightSettings`, `isOverrideInputOn`.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or build, captures, renders, controller routes, message text extraction, `region_from_dossier.py`, `validate_city.py`, `build_city_ids.py`.

**Next dependency:** REF-03 captures for `cam.honey_darling.entry`, `.reverse_door`, `.fixture` (and Day 2 / Day 3 variants); integrator lift and validation of section 10; section 9 extraction; ROOM-01 commercial pilot pattern (INT-01) before building this room; a controller for the platform, timer and reward logic belongs to a later gameplay packet.
