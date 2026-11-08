# West Clock Town (`loc.west_clock_town`): source inventory dossier

Task **REF-06** (FULL_CITY_PLAN.md section 12). Base commit `98e554b3c1fc778894fd2c66de64ff040a90bb71` on `main`. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_ICHIBA` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_ICHIBA/Z2_ICHIBA.c`, `room.c` = `.../Z2_ICHIBA_room_00.c`. Line numbers count `\n` only (both files contain stray lone `\r` characters that some editors treat as line breaks). N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected.

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01: IDs, counts), `docs/city/PORTAL_REGISTER.md` (REF-02: every West portal row, entrance decoding, door-to-venue check), `docs/city/SOURCE_REGISTER.md` (REF-03: profile, half-day bits, `cam.west_clock_town.*`). Every West value below that those registers also state agrees with them (checked); wording notes are in section 10.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_ICHIBA` / `SCENE_ICHIBA` | `include/tables/scene_table.h:223` |
| Scene-table index | `0x6D` (109) | same line |
| Entrance-table scene index | `0x6A` (`ENTR_SCENE_WEST_CLOCK_TOWN`) | `src/code/z_scene_table.c:2679`, `include/z64scene.h:699` |
| Title card text / draw config / restrictions | `0x102` / `SCENE_DRAW_CFG_DEFAULT` / `RESTRICTIONS_NONE` | scene_table.h:223 |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_NONE` | scene_table.h:223 |
| XML files | Scene `Z2_ICHIBA` segment 2; room `Z2_ICHIBA_room_00` segment 3; three room DLs declared: `0x530` (empty), `0x8CF0`, `0xAAB8` | XML `assets/xml/scenes/Z2_ICHIBA/Z2_ICHIBA.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)` in both scene headers) | HDR scene.c:16, :1509 |
| Scene headers | default + 1 alternate (`Z2_ICHIBASet_003A60`) | HDR scene.c:34-36, :1507 |
| Room headers | default + 1 alternate (`Z2_ICHIBA_room_00Set_000370`) | HDR room.c:25-27, :211 |
| Transition actors | 6 `EN_DOOR` in both headers (identical lists) | HDR scene.c:59-66, :1530-1537 |
| Sound | spec 0, ambience 0, seq 29 (alt: ambience 19, seq 0) | HDR scene.c:15, :1508 |
| Skybox | file 0, skybox 1, weather 10, lightMode false (alt: weather 0, lightMode true) | HDR scene.c:23, :1515 |
| Env light settings | 29 entries (alt 4); values not transcribed | HDR scene.c:25, :1517 |
| Animated materials | single entry `{ 0, 6, NULL }` (segment 0, skipped, as in North) | HDR scene.c:794-796 |
| Minimap | 1 entry `{0x0105, 0xFEC0, 0, 0xFC40, 0}`, scale 52; the same values as North's entry (shared town map, INF) | HDR scene.c:732-738 |
| Cutscene scripts / actor cutscenes / cs cameras | 4 / 15 / 11 (alt: 1 / 1 / none) | HDR scene.c:26-28 |
| Paths | 9 (alt: none, no `SCENE_CMD_PATH_LIST`) | HDR scene.c:264-274 |
| Room commands (default) | echo 4; `ROOM_BEHAVIOR(0x04, 0x00, 0, 0, true, 1)` = room type 4 (`ROOM_TYPE_4`, `include/z64scene.h:319`), environment 0, positional lights enabled, storm state 1 (`src/code/z_scene.c:245-252`); `SKYBOX_DISABLES(false, false)`; `TIME_SETTINGS(255, 255, 3)`; cullable room shape, 7 entries | HDR room.c:13-22, :193 |
| Room commands (alt) | same, except `TIME_SETTINGS(12, 0, 0)`: clock forced to 12:00, speed 0 | HDR room.c:211-220 |

Differences from North (REF-05): West's room type is 4 and enables positional lights (North: type 0, false). The torches insert point lights and `Bg_Market_Step` carries `ACTOR_FLAG_UCODE_POINT_LIGHT_ENABLED` (OVL), so the reference lighting path must honor them (INF).

### 1.1 What scene layer 1 (alternate header) is

Selection (C): sceneLayer 1 requires `cutsceneIndex >= 0xFFF0` at load (rules cited in North 1.1). The West entrance table has a layer-1 row only for entrance 0: `{ SCENE_ICHIBA, 0, 0x8102 }` (`z_scene_table.c:2259-2262`). The only `ENTRANCE(WEST_CLOCK_TOWN, 0)` call site outside the debug map select is the credits branch `CS_CREDITS_MASK_BLAST`: when the Blast Mask is in the inventory it sets `nextEntrance = ENTRANCE(WEST_CLOCK_TOWN, 0)` and `nextCutsceneIndex = 0xFFF0` (`src/code/z_demo.c:722-730`). The layer-1 cutscene `Z2_ICHIBACutsceneData_003BC0` is a camera spline that ends in `CS_CHOOSE_CREDITS_SCENES(CS_CREDITS_MASK_BUNNY, 10, 3000)` with text `0x4E35` (HDR scene.c:1572-1630). So **layer 1 is a credits vignette** (C). Ordinary Day 1 play uses the default headers. This answers LOCATION_REGISTER U-05 and SOURCE_REGISTER U-02 for West.

| Item | Default (layer 0) | Alternate (layer 1) |
| --- | --- | --- |
| Spawns / entrance list | 9 / 10 rows | 1 / 2 rows (both -> start 0); start 0 params `0x0FFF` (mode 0xF, bgCam 0xFF) and rotY raw `0x2D7F` instead of `0x0F02` / `0x2D00` |
| Transition actors | 6 `EN_DOOR` | identical 6 |
| Exit list | 10 words | same 10 values (REF-02 section 5) |
| Collision | `Z2_ICHIBACollisionHeader_003A28` | same header |
| Room object list | 18 banks | 9 banks: drops cs, mm, sdn, lodmoon, boj, rz, sek, wdor01-03, hariko; adds bba, rsn |
| Room actor list | 40 | 22: no banker, cow statue, sign spots, gate soldier, NPCs, owl statue, moon or `En_Test4`; adds `Obj_Shutter` at the bank, `En_Baba` (`0x8000`) and `En_Rsn` at (-1940, 165, -513), five torch pairs at new positions |

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:38-57; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam index `params & 0xFF` (`include/z64player.h:17-18`). Yaw = `(rawRotY >> 7) & 0x1FF` degrees. Facing vector in N64 XZ is `(sin yaw, cos yaw)`.

| Start | Position | Raw rot X / Y / Z | Yaw | Params | Mode | bgCam | Reached by (C, cross-check of REF-02) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-2076, 200, -1086) | 0x0007 / 0x2D00 / 0x007F | 90 | 0x0F02 | 0xF | 0x02 | Entrance 0 (`0xD400`, sender on the Termina Field side, external NI) and entrance 9 (`EntranceList` row 9 = `{0x00, 0x00}`, scene.c:82); 39 u in front of the gate soldier |
| 1 | (-1007, 0, -107) | 0x0007 / 0x8580 / 0x007F | 267 | 0x0E0A | 0xE | 0x0A | Entrance 1 `0xD410` = South exit [5], **lower** approach |
| 2 | (-1253, 200, -1095) | 0x0007 / 0x8700 / 0x007F | 270 | 0x0F06 | 0xF | 0x06 | Entrance 2 `0xD420` = South exit [3], **upper** approach |
| 3 | (-1870, 200, -1389) | 0x0007 / 0x0000 / 0x007F | 0 | 0x0F03 | 0xF | 0x03 | `0xD430` from Swordsman's School |
| 4 | (-1490, 45, -56) | 0x0007 / 0x4C00 / 0x007F | 152 | 0x0D09 | 0xD | 0x09 | `0xD440` from Curiosity Shop |
| 5 | (-1760, 105, -244) | 0x0007 / 0x3E80 / 0x007F | 125 | 0x0D08 | 0xD | 0x08 | `0xD450` from Trading Post |
| 6 | (-1946, 165, -508) | 0x0007 / 0x3880 / 0x007F | 113 | 0x0D07 | 0xD | 0x07 | `0xD460` from Bomb Shop |
| 7 | (-1609, 200, -1368) | 0x0007 / 0x0000 / 0x007F | 0 | 0x0F04 | 0xF | 0x04 | `0xD470` from Post Office |
| 8 | (-1453, 200, -814) | 0x0007 / 0x7280 / 0x007F | 229 | 0x0F05 | 0xF | 0x05 | `0xD480` from Lottery Shop |

Entrance-table flags (C, `z_scene_table.c:2259-2297`): entrance 0 `0x4102` (+ layer 1 `0x8102`); 1 `0xC102`; 2 `0xCA14`; 3 to 8 `0x4102`; 9 `0x4A14`. Every bgCam index 0x02-0x0A exceeds the 2-entry `BgCamInfo` list (section 6), the same open question as North U-02 (U-02 here).

### 2.2 Exit list (HDR scene.c:85-96) and consumers

Decode is a **cross-check only** (REF-02 owns it): scene `value >> 9`, entrance `(value >> 4) & 0x1F`, layer `value & 0xF`.

| Exit idx | Raw | Cross-check decode | Destination | Consumer in West |
| --- | --- | --- | --- | --- |
| 0 | `0x5400` | 0x2A `Z2_00KEIKOKU`, entr 0 | `ext.z2_00keikoku` | surface 15, polys 333, 334 (gate) |
| 1 | `0xD850` | 0x6C South, entr 5 | `loc.south_clock_town` start 5 | surface 18, polys 374, 375 (lower) |
| 2 | `0xD830` | 0x6C South, entr 3 | `loc.south_clock_town` start 3 | surface 19, polys 380, 381 (upper) |
| 3 | `0xA200` | 0x51 Swordsman's School, entr 0 | `loc.swordsmans_school` | surface 7, polys 54, 55 (behind TA1) |
| 4 | `0x0E00` | 0x07 Curiosity Shop, entr 0 | `loc.curiosity_shop` | surface 13, polys 208, 209 (behind TA5) |
| 5 | `0x6200` | 0x31 Trading Post, entr 0 | `loc.trading_post` | surface 12, polys 206, 207 (behind TA4) |
| 6 | `0xCA00` | 0x65 Bomb Shop, entr 0 | `loc.bomb_shop` | surface 11, polys 204, 205 (behind TA3) |
| 7 | `0x5600` | 0x2B Post Office, entr 0 | `loc.post_office` | surface 8, polys 56, 57 (behind TA0) |
| 8 | `0x6C00` | 0x36 Lottery Shop, entr 0 | `loc.lottery_shop` | surface 17, polys 370, 371 (behind TA2) |
| 9 | `0xD650` | 0x6B North, entr 5 | `loc.north_clock_town` start 5 | `En_Bomjimb` #11 params `0x0910`: `(p >> 8) & 0xFF = 9` (OVL `z_en_bomjimb.c:162`, `:861`) |

No exit word is a trailing `0x0000`; all 10 have a consumer.

### 2.3 Collision surfaces carrying exits (C from scene.c:807-1205)

`exitIndexPlusOne` = bits 8-12 of `SurfaceType.data[0]` (`include/z64bgcheck.h:79-87`). Polygon `i` is on scene.c line `816 + i`. Conveyor direction x 1024 = binang (0x4000 = +X).

| Surface | data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Centroid | Normal | Conveyor | Material |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 7 | 0x00000400 / 0x000017C2 | 3 Swordsman's | 54, 55 | -1900..-1840 / 200 / -1513..-1400 | (-1870, 200, -1456) | +Y | none | stone |
| 8 | 0x00000800 / 0x000017C2 | 7 Post Office | 56, 57 | -1640..-1580 / 200 / -1509..-1400 | (-1610, 200, -1454) | +Y | none | stone |
| 11 | 0x00000700 / 0x000017C2 | 6 Bomb Shop | 204, 205 | -2074..-1958 / 165 / -526..-431 | (-2016, 165, -478) | +Y | none | stone |
| 12 | 0x00000600 / 0x000017C2 | 5 Trading Post | 206, 207 | -1878..-1761 / 105 / -239..-122 | (-1819, 105, -180) | +Y | none | stone |
| 13 | 0x00000500 / 0x000017C2 | 4 Curiosity Shop | 208, 209 | -1570..-1474 / 45 / -42..79 | (-1522, 45, 18) | +Y | none | stone |
| 15 | 0x00000100 / 0x060C2FC0 | 0 Termina Field | 333, 334 | -2360..-2114 / 200..201 / -1178..-991 | (-2237, 200, -1084) | (136, 32767, -4) | speed 3, dir 48 (-X) | dirt |
| 17 | 0x00000900 / 0x000017C2 | 8 Lottery Shop | 370, 371 | -1467..-1345 / 200 / -831..-709 | (-1406, 200, -770) | +Y | none | stone |
| 18 | 0x00000200 / 0x020C07C0 | 1 South lower | 374, 375 | -968..-800 / 0 / -160..-40 | (-884, 0, -100) | +Y | speed 3, dir 16 (+X) | dirt |
| 19 | 0x00000300 / 0x020C2FC2 | 2 South upper | 380, 381 | -1200..-1000 / 100..200 / -1208..-992 | (-1100, 150, -1099) | (14654, 29308, 0): 26.6 deg, falls toward +X | speed 3, dir 16 (+X) | stone |

The three walk-in exits (15, 18, 19) carry the conveyor flag (vIB bit 1) on both polygons; the six door exits do not, and carry the ignore-camera flag (vIA bit 1) instead. All values agree with PORTAL_REGISTER section 3.3.

### 2.4 Access findings

#### 2.4.1 The six venue doors (HDR scene.c:59-66; decode C; schedule OVL)

`EN_DOOR` params: type `(p >> 7) & 7`, type var `p & 0x7F` (`ovl_En_Door/z_en_door.h:9-20`, `:95-96`); all six are type 5 `ENDOOR_TYPE_SCHEDULE`. Spawned params are `TRANSITION_ACTOR_PARAMS(i, p) = (i << 10) + (p & 0x3FF)` (`include/z64door.h:23`); yaw `(rotY >> 7) & 0x1FF`, csId `rotY & 0x7F`, half-day mask always `HALFDAYBIT_ALL` (`src/code/z_actor.c:3597-3620`). Both sides of every door are room 0 with bgCam -1 (generic door camera, `include/z64scene.h:369-378`). Door-local frame: forward = door yaw; `local (lateral, up, forward)`.

| TA | Position | rotY raw / yaw | Params (spawned) | Schedule type | Exit polys behind (door-local) | Return start (local, yaw) | Venue | Door leaf (bank, DL) | Day 1 12:00 schedule result |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-1610, 200, -1420) | 0x007F / 0 | 0x0281 (0x0281) | `POST_OFFICE` (1) | 56, 57 at (0, 0, -34) | 7 (1, 0, +52), yaw 0 | `loc.post_office` | `object_wdor02` `gPostOfficeTradingPostDoorDL` 0x548 | none = openable (Day 1 branch "open 12:00-13:00"; `if_before_time(12, 0)` is false at 12:00 because the test is `now < time`, `z_schedule.c:198`) |
| 1 | (-1870, 200, -1420) | 0x007F / 0 | 0x0280 (0x0680) | `SWORDSMANS_SCHOOL` (0) | 54, 55 at (0, 0, -36) | 3 (0, 0, +31), yaw 0 | `loc.swordsmans_school` | `object_wdor03` `gInnSchoolDoorDL` 0x548 | none = openable (closed only Day 3 23:00-00:00) |
| 2 | (-1432, 200, -796) | 0x70FF / 225 | 0x0282 (0x0A82) | `LOTTERY_SHOP` (2) | 370, 371 at (0, 0, -36) | 8 (2, 0, +28), yaw 229 | `loc.lottery_shop` | `object_wdor01` `gLotteryCuriosityShopMayorHouseDoorDL` 0x548 | none = openable (06:00-23:00) |
| 3 | (-1988, 165, -490) | 0x387F / 112 | 0x0286 (0x0E86) | `BOMB_SHOP` (6) | 204, 205 at (0, 0, -31) | 6 (1, 0, +46), yaw 113 | `loc.bomb_shop` | `gameplay_keep` `gDoorLeftDL` 0x20BB8 / `gDoorRightDL` 0x20D00 (`sObjectInfo[23]` = default door) | none = openable (script always none) |
| 4 | (-1796, 105, -204) | 0x43FF / 135 | 0x0283 (0x1283) | `TRADING_POST` (3) | 206, 207 at (0, 0, -33) | 5 (3, 0, +54), yaw 125 | `loc.trading_post` | `object_wdor02` 0x548 (shared with Post Office) | none = openable (message only 21:00-22:00) |
| 5 | (-1510, 45, -12) | 0x4F7F / 158 | 0x0284 (0x1684) | `CURIOSITY_SHOP` (4) | 208, 209 at (0, 0, -33) | 4 (-2, 0, +48), yaw 152 | `loc.curiosity_shop` | `object_wdor01` 0x548 (shared with Lottery) | **closed**: returns `0xA`, text `0x180A` (open only 22:00-05:00) |

Schedules: `ovl_En_Door/scheduleScripts.schl:13-163` (OVL); script times are shifted by 06:00 so 22:00-05:00 is one range (`include/z64scripts.h:7`); `if_time_range` is `start <= now <= end - 1` (`z_schedule.c:33-42`). The schedule only runs while the player stands on the door's front side (`playerPosRelToDoor.z > 0`, `z_en_door.c:537`). In every row the exit polygons lie 31-36 u behind the door on its axis and the return start 28-54 u in front, so front = street side and the venue trigger is behind the leaf (C, geometry). Distances agree with REF-02's 30-37 u door association within 1 u.

Door leaf drawing (OVL): every `EN_DOOR` is `gDoorSkel` (gameplay_keep 0x22B28, standard skeleton) with limb 4's DL replaced by the venue DL from `sObjectInfo[17 + schType]` (`z_en_door.c:189-243`, `:346-347`, `:637-660`). When both sides are the same room the side DL is chosen by the camera-to-door yaw (`:645-655`). The object list's `object_wdor01/02/03` are explained by these doors (C). **Shared leaf DLs:** Post Office and Trading Post share `object_wdor02`; Lottery and Curiosity share `object_wdor01`; Swordsman's uses `object_wdor03` (also the Inn doors, `sObjectInfo[28-29]`); the Bomb Shop uses the generic keep door. Whether facades, frames and stoops are shared between venues lives in room DLs and in `Bg_Market_Step`'s single market DL (section 4.5): **not inspectable here** (NI, U-06).

Sign spots (`En_Talk`, no draw; text `(params & 0x3F) + 0x1C00`, `ovl_En_Talk/z_en_talk.h:6`): one per venue, 23-123 u from its door and 74-115 u above it, plus a second Swordsman's spot and one at the bank (positions C; the venue pairing is INF by nearest door; text content NI, U-01).

| Room # | Position | Text | Nearest door (xz u; door-local) | INF frontage |
| --- | --- | --- | --- | --- |
| 19 | (-1610, 315, -1397) | 0x1C0D | TA0 (23; 0, 115, +23) | Post Office, above the door |
| 5 | (-1930, 277, -1396) | 0x1C0A | TA1 (65; -60, 77, +24) | Swordsman's, one side of the door |
| 10 | (-1809, 274, -1396) | 0x1C0A | TA1 (66; 61, 74, +24) | Swordsman's, other side (same text) |
| 25 | (-1398, 302, -914) | 0x1C13 | TA2 (123; -107, 102, +59) | Lottery Shop |
| 2 | (-2000, 264, -574) | 0x1C0E | TA3 (85; 82, 99, +20) | Bomb Shop |
| 9 | (-1845, 218, -282) | 0x1C10 | TA4 (92; 90, 113, +21) | Trading Post |
| 22 | (-1557, 138, -55) | 0x1C0F | TA5 (64; 60, 93, +22) | Curiosity Shop |
| 23 | (-1495, 176, -376) | 0x1C14 | banker 89 u (TA4 347 u) | bank |

#### 2.4.2 The bank (N64 placement; answers FULL_CITY_PLAN 5.3)

**This is the placement the N64 source gives. No 3DS placement is used or implied anywhere in this dossier.** The bank is not a scene and has no door or exit: it is a counter on the street served by one actor (REF-01 section 5).

| Part | Source | Value | Evidence |
| --- | --- | --- | --- |
| Banker | default room #21 `ACTOR_EN_GINKO_MAN` (0x177), room.c:127 | (-1559, 133, -438), raw rot `0x0007 / 0x9E7F / 0x007F`, yaw 316, csId none, mask all (`0x3FF`), params `0x00FF` | HDR, C |
| Identity | bank dialogue: `HS_GET_BANK_RUPEES()` on talk, deposit/withdraw choice constants, bank reward flags `WEEKEVENTREG_RECEIVED_BANK_WALLET_UPGRADE` / `_HEART_PIECE`; XML names the skeleton `object_ginko_skeleton` and the idle `Boj_ginkowait` ("ginko" = "bank") | REF-01's name-based INF is now confirmed | OVL `z_en_ginko_man.c:85-100`, `:150-195`, `:546-582`, `z_en_ginko_man.h:28-34`; XML `object_boj.xml:13`, `:78` |
| Object / draw | profile object `OBJECT_BOJ` (0x0E3); flex `object_boj_Skel_00C240` (0xC240), init anim `object_boj_Anim_0043F0`; scale 0.01; limb 0x0F DL replaced by `object_boj_DL_00B1D8`; env color (50, 80, 0) on segments 8 and 9 | **posed-skeletal**, not a static mesh | OVL `:31-41`, `:60-75`, `:652-690` |
| Presence | params never read; no kill and no schedule in Init; mask all half-days | present at Day 1 noon | OVL, C |
| Pose | `GINKO_ANIM_SITTING` at init; `ADVERTISING` loop while the player is over 160 u away, `AMAZED` within 160 u (`EnGinkoMan_SwitchAnimation`) | profile must choose (U-18) | OVL `:629-642` |
| Facing | yaw 316 faces (-0.695, 0, 0.719), toward the surface-14 landing whose centroid is 54 u ahead and 28 u lower | INF | C |
| Cow statue head | default #20 `ACTOR_OBJ_HARIKO`, room.c:125: (-1589, 143, -464), yaw 343, params 0x0000, 40 u lateral of the banker and 10 u higher; `object_hariko`, scale 0.1, `gHarikoBodyDL` 0x80 + `gHarikoFaceDL` 0x110; bobs only during quakes | static-mesh; source comment "Little Cow Statue Head", XML "Clock Town cow statues"; on the counter (INF) | HDR, OVL `z_obj_hariko.c:1-90`, XML `object_hariko.xml` |
| Sign spot | default #23 `En_Talk` text 0x1C14 (above) | no geometry | HDR, OVL |
| Bank advertisements and door | **actor geometry**, not room geometry: `Bg_Market_Step` #37 (day, params 0) draws `gWestClockTownMarketBankAdvertisementsAndDoorDayDL` (object_market_obj 0x1EF10); #38 (night, params 1) draws the night DL 0x18C60. XML comment: "The door is not interactive, and it's not visible in-game." | static-mesh at the origin, scale 1.0 | OVL `z_bg_market_step.c:1-45`; XML `object_market_obj.xml:6-9` |
| Counter floor | surface 16, polys 356, 357: floor at Y 133, x -1608..-1481, z -520..-393 (banker stands on it, 23 u from its centroid); polys 358, 359 are its walls (Y 103..143) | room collision | C |
| Customer landing | surface 14, polys 230-232: floor at Y 105, x -1679..-1515, z -485..-320; the only surface with bgCam 1 = `0x003F CAM_SET_ROOM0` (`include/z64camera.h:210`) | standing here switches the camera setting | C |
| Counter barrier | polys 364, 365 (surface 9): vertical wall, Y 133..193, x -1593..-1524, z -476..-407, within 4 u of the banker's XZ and perpendicular to his facing; vIB bit 4 (non-player actors ignore it, `z_bgcheck.c:566-568`) and vIA ignore-projectiles | keeps the player, not the banker, off the counter floor (INF) | C, OVL |
| Shutter | `ACTOR_OBJ_SHUTTER` exists **only in alternate room header 1** (credits), room.c:238: (-1553, 133, -448), yaw 316; `object_f53_obj` `gBankShutterDL` 0x11E0, scale 0.1; schedule 10:00-20:00 result 1 raises it 80 u, otherwise it drops | absent from Day 1 play; source comment "Unused West Clock Town bank closing shutter" | HDR, OVL `z_obj_shutter.c:1-80`, `scheduleScripts.schl:1-7` |

#### 2.4.3 The two South approaches (kept distinct)

| | Lower | Upper |
| --- | --- | --- |
| West exit (to South) | exit [1] `0xD850`, surface 18, polys 374, 375; flat Y 0; x -968..-800, z -160..-40; conveyor +X | exit [2] `0xD830`, surface 19, polys 380, 381; 26.6 deg ramp from Y 200 at x -1200 to Y 100 at x -1000; z -1208..-992; conveyor +X |
| Arrives in South | start 5 (-780, 0, -100) yaw 90 (REF-02) | start 3 (-862, 100, -1094) yaw 90 (REF-02) |
| South exit into West | South [5] `0xD410`, polys 165, 166, AABB (-1000, 0, -160)..(-832, 0, -40) | South [3] `0xD420`, polys 184, 185, AABB (-1200, 100, -1200)..(-1000, 200, -1000) |
| Arrives in West | start 1 (-1007, 0, -107) yaw 267, 39 u west of the exit floor | start 2 (-1253, 200, -1095) yaw 270, 53 u west of the ramp top |
| Enclosure (C) | a covered passage: ceiling polys 48, 49 at Y 160 over x -1000..-800, z -160..-40; end wall polys 376, 377 at x -800, Y 0..160 (ignore-projectiles); polys 372, 373 (surface 9, Y 0) join the exit floor to x -1000 | open ramp; top floor polys 382, 383 (surface 2, Y 200, x -1400..-1200) are edge-connected to the street |
| NPC path (HDR) | path 4 starts at (-837, 0, -96), inside this passage | path 2 crosses (-1000, 100, -1075) to (-580, 53, -900) in South |

The West exit polygons occupy the same AABBs as their South counterparts (REF-02 3.2 corroboration), consistent with a shared world frame.

#### 2.4.4 Out-of-city gate

| Part | Value | Evidence |
| --- | --- | --- |
| Exit | [0] `0x5400` -> `ext.z2_00keikoku` entrance 0; surface 15, polys 333, 334, near-flat floor Y 200..201 at x -2360..-2114, z -1178..-991; conveyor speed 3 toward -X | C |
| Enclosure | ceiling polys 40, 41 (surface 3) at Y 480-481 over x -2360..-2034, z -1180..-991; end wall polys 378, 379 at x -2360..-2355, Y 201..481 (ignore-projectiles) | C: a roofed gate passage |
| Gate soldier | default #1 `ACTOR_EN_STOP_HEISHI \| 0x2000` (0x1C7), room.c:55: (-2115, 200, -1087), raw rot `0x0007 / 0x2D7F / 0x007F`, yaw 90 (facing into town), rotZ raw integer 0 -> `unk_288 = 50`; mask all; params `0x207F`: switch flag `p & 0x3F = 0x3F` (actor killed if set; INF clear on a fresh save), type nibble `p >> 12 = 2` (North's is 1; it indexes the leave-message table, `z_en_stop_heishi.c:434-448`); `object_sdn` `gSoldierSkel` (0xD640) | HDR, C, OVL `:95-123` |
| Soldier placement | at the town-side edge of the exit floor (x -2114); the exit centroid is 122 u behind him; start 0 is 39 u in front | C |
| Passage rules | the overlay sets form-dependent states (Human: only with `WEEKEVENTREG_12_20` or a stolen-sword case; Goron 2, Zora 3, Deku 4) and night/Day 3 variants (`z_en_stop_heishi.c:375-415`, `:505-523`). How these block or release the player on the exit floor: **UNKNOWN** (U-03) | OVL partial |
| Night resident | default #0 `En_Suttari` at (-2243, 201, -1062), inside the passage, mask `0x010` (Day 2 night only) | HDR, C |

Treat the gate as an honest external boundary (`ext.z2_00keikoku` is not extracted). Which forms or items pass on Day 1 noon is not settled by this pass.

### 2.5 Proposed portal IDs (as in `docs/city/PORTAL_REGISTER.md`)

| Portal ID | Source evidence | Arrival back into West |
| --- | --- | --- |
| `portal.west_clock_town.south_clock_town.lower` | surface 18, exit 1 `0xD850` | start 1 via `0xD410` |
| `portal.west_clock_town.south_clock_town.upper` | surface 19, exit 2 `0xD830` | start 2 via `0xD420` |
| `portal.west_clock_town.z2_00keikoku` (to `ext.z2_00keikoku`) | surface 15, exit 0 `0x5400` | start 0 (external sender NI) |
| `portal.west_clock_town.post_office` | TA0 + surface 8, exit 7 `0x5600` | start 7 via `0xD470` |
| `portal.west_clock_town.swordsmans_school` | TA1 + surface 7, exit 3 `0xA200` | start 3 via `0xD430` |
| `portal.west_clock_town.lottery_shop` | TA2 + surface 17, exit 8 `0x6C00` | start 8 via `0xD480` |
| `portal.west_clock_town.bomb_shop` | TA3 + surface 11, exit 6 `0xCA00` | start 6 via `0xD460` |
| `portal.west_clock_town.trading_post` | TA4 + surface 12, exit 5 `0x6200` | start 5 via `0xD450` |
| `portal.west_clock_town.curiosity_shop` | TA5 + surface 13, exit 4 `0x0E00` | start 4 via `0xD440` |
| `portal.west_clock_town.north_clock_town.bomjimb` (special, one-way) | `En_Bomjimb` #11, exit 9 `0xD650` | n/a |

Inbound: `portal.south_clock_town.west_clock_town.upper` / `.lower` and the six venue `*.west_clock_town` rows (plus their kickout rows) in REF-02 section 4. The bank has no portal.

## 3. Object banks (HDR room.c:29-48 and :222-232; IDs from `include/tables/object_table.h`)

| # | Bank | ID | Consumer (OVL profile or selection) | Day 1 static need |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_F53_OBJ` | 0x16D | Obj_Entotu (chimneys); Obj_Shutter (alt only) | billboard approximation |
| 1 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai | yes (already extracted) |
| 2 | `OBJECT_OS_ANIME` | 0x09D | no listed West actor (`En_Cne_01` uses it) | no |
| 3 | `OBJECT_SYOKUDAI` | 0x080 | Obj_Syokudai (torches) | yes |
| 4 | `OBJECT_KIBAKO` | 0x16F | no listed West actor (`Obj_Kibako` uses it) | no |
| 5 | `OBJECT_CS` | 0x110 | En_Bombers, En_Bomjimb | skeletal (later) |
| 6 | `OBJECT_MARKET_OBJ` | 0x1B0 | Bg_Market_Step (market scenery, bank advertisements) | **yes (hero geometry)** |
| 7 | `OBJECT_MM` | 0x107 | En_Pm | no (schedule-hidden at noon) |
| 8 | `OBJECT_SDN` | 0x1B6 | En_Stop_heishi | skeletal (later) |
| 9 | `OBJECT_TOKEI_STEP` | 0x1A4 | Obj_Tokei_Step | yes (already extracted) |
| 10 | `OBJECT_LODMOON` | 0x223 | En_Fall type 6 | yes (moon, special rule) |
| 11 | `OBJECT_BOJ` | 0x0E3 | En_Ginko_Man (banker), En_Suttari | skeletal (banker, later) |
| 12 | `OBJECT_RZ` | 0x24B | En_Rz | no (night) |
| 13 | `OBJECT_SEK` | 0x170 | Obj_Warpstone | optional (statue east of the exits) |
| 14 | `OBJECT_WDOR01` | 0x274 | EN_DOOR Lottery, Curiosity | door leaves |
| 15 | `OBJECT_WDOR02` | 0x275 | EN_DOOR Post Office, Trading Post | door leaves |
| 16 | `OBJECT_WDOR03` | 0x276 | EN_DOOR Swordsman's School | door leaves |
| 17 | `OBJECT_HARIKO` | 0x1F2 | Obj_Hariko | yes |

Alternate list (9): `f53_obj`, `obj_tokeidai`, `os_anime`, `syokudai`, `kibako`, `market_obj`, `tokei_step`, `object_bba` 0x0DF (En_Baba), `object_rsn` 0x12C (En_Rsn). Implicit: `gameplay_keep` 0x001 (En_Talk, En_Test4, the Bomb Shop door, `gDoorSkel`, `gEffFire1DL`); `gameplay_field_keep` 0x002 is not named in the West scene header (special-files command NI). Actor spawn fails when the profile's object is absent (as in North section 3).

## 4. Actor tables

### 4.1 Decode rules

As in North 4.1 (`Actor_SpawnEntry`, `src/code/z_actor.c:3622-3655`): ID `id & 0x1FFF`; rotation `(raw >> 7) & 0x1FF` degrees unless the ID carries the axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), in which case it is a plain integer wrapped to -180..180 (marked `i`); csId `rawRotY & 0x7F`; half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all. Day 1 12:00 bit `0x080`. West flags: `En_Stop_heishi | 0x2000` and every `En_Talk | 0xE000` (all three axes raw). No West entry has ID bit `0x800`.

Classes as in North: `static-mesh`, `posed-skeletal`, `billboard/effect`, `controller/invisible`, `npc-scheduled`, `door/transition`. "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (40 entries, HDR room.c:50-190, decode C)

| # | Actor (ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d | Line |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `EN_SUTTARI` (0x237) | (-2243, 201, -1062) | 0x0000 / 0x2C7F / 0x0010 | 0 / 88 / 0 | 0x010 D2n | none | 0x83FF | object_boj | npc-scheduled | no | 51 |
| 1 | `EN_STOP_HEISHI \| 0x2000` (0x1C7) | (-2115, 200, -1087) | 0x0007 / 0x2D7F / 0x007F | 0 / 90 / 0i | all | none | 0x207F | object_sdn | npc-scheduled | yes | 55 |
| 2 | `EN_TALK \| 0xE000` (0x261) | (-2000, 264, -574) | 0x0387 / 0x39FF / 0x007F | 7i / 115i / 0i | all | none | 0xFE0E | gameplay_keep | controller/invisible | yes | 59 |
| 3 | `EN_BOMBERS` (0x280) | (-1974, 200, -1280) | 0x0000 / 0x007F / 0x0030 | 0 / 0 / 0 | 0x030 D2d, D2n | none | 0x0000 | object_cs | npc-scheduled | no | 63 |
| 4 | `EN_BOMBERS` | (-1943, 200, -1292) | 0x0000 / 0x007F / 0x000C | 0 / 0 / 0 | 0x00C D3d, D3n | none | 0x0000 | object_cs | npc-scheduled | no | 67 |
| 5 | `EN_TALK \| 0xE000` | (-1930, 277, -1396) | 0x0387 / 0x007F / 0x007F | 7i / 0i / 0i | all | none | 0xFE0A | gameplay_keep | controller/invisible | yes | 71 |
| 6 | `OBJ_SYOKUDAI` (0x039) | (-1902, 135, -407) | 0x0005 / 0x0BFF / 0x002A | 0 / 23 / 0 | 0x2AA all dawns | none | 0x207F | object_syokudai | static-mesh | yes | 75 |
| 7 | `OBJ_SYOKUDAI` | (-1902, 135, -407) | 0x0002 / 0x0BFF / 0x0055 | 0 / 23 / 0 | 0x155 all nights | none | 0x287F | object_syokudai | static-mesh + flame | no | 79 |
| 8 | `EN_BOMBERS` | (-1894, 200, -1271) | 0x0001 / 0x007F / 0x0040 | 0 / 0 / 0 | 0x0C0 D1d, D1n | none | 0x0000 | object_cs | npc-scheduled | yes | 83 |
| 9 | `EN_TALK \| 0xE000` | (-1845, 218, -282) | 0x0387 / 0x42FF / 0x007F | 7i / 133i / 0i | all | none | 0xFE10 | gameplay_keep | controller/invisible | yes | 87 |
| 10 | `EN_TALK \| 0xE000` | (-1809, 274, -1396) | 0x0387 / 0x007F / 0x007F | 7i / 0i / 0i | all | none | 0xFE0A | gameplay_keep | controller/invisible | yes | 91 |
| 11 | `EN_BOMJIMB` (0x27F) | (-1741, 200, -1388) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0x0910 | object_cs | npc-scheduled | yes | 95 |
| 12 | `OBJ_SYOKUDAI` | (-1618, 200, -780) | 0x0005 / 0x007F / 0x002A | 0 / 0 / 0 | 0x2AA all dawns | none | 0x207F | object_syokudai | static-mesh | yes | 99 |
| 13 | `OBJ_SYOKUDAI` | (-1618, 200, -780) | 0x0002 / 0x877F / 0x0055 | 0 / 270 / 0 | 0x155 all nights | none | 0x287F | object_syokudai | static-mesh + flame | no | 103 |
| 14 | `EN_RZ` (0x27B) | (-1706, 200, -902) | 0x0000 / 0x130B / 0x0050 | 0 / 38 / 0 | 0x050 D1n, D2n | 11 | 0x7E02 | object_rz | npc-scheduled | no | 105 |
| 15 | `OBJ_SYOKUDAI` | (-1682, 75, -157) | 0x0005 / 0x9DFF / 0x002A | 0 / 315 / 0 | 0x2AA all dawns | none | 0x207F | object_syokudai | static-mesh | yes | 109 |
| 16 | `OBJ_SYOKUDAI` | (-1682, 75, -157) | 0x0002 / 0x9DFF / 0x0055 | 0 / 315 / 0 | 0x155 all nights | none | 0x287F | object_syokudai | static-mesh + flame | no | 111 |
| 17 | `EN_RZ` | (-1642, 200, -932) | 0x0000 / 0x098B / 0x0050 | 0 / 19 / 0 | 0x050 D1n, D2n | 11 | 0xFE02 | object_rz | npc-scheduled | no | 113 |
| 18 | `EN_PM` (0x1D5) | (-1610, 200, -1436) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0x0000 | object_mm | npc-scheduled | yes | 117 |
| 19 | `EN_TALK \| 0xE000` | (-1610, 315, -1397) | 0x0107 / 0x007F / 0x007F | 2i / 0i / 0i | all | none | 0xFE0D | gameplay_keep | controller/invisible | yes | 121 |
| 20 | `OBJ_HARIKO` (0x20A) | (-1589, 143, -464) | 0x0007 / 0xABFF / 0x007F | 0 / 343 / 0 | all | none | 0x0000 | object_hariko | static-mesh | yes | 125 |
| 21 | `EN_GINKO_MAN` (0x177) | (-1559, 133, -438) | 0x0007 / 0x9E7F / 0x007F | 0 / 316 / 0 | all | none | 0x00FF | object_boj | posed-skeletal | yes | 127 |
| 22 | `EN_TALK \| 0xE000` | (-1557, 138, -55) | 0x0387 / 0x4CFF / 0x007F | 7i / 153i / 0i | all | none | 0xFE0F | gameplay_keep | controller/invisible | yes | 129 |
| 23 | `EN_TALK \| 0xE000` | (-1495, 176, -376) | 0x0387 / 0x9D7F / 0x007F | 7i / -46i / 0i | all | none | 0xFE14 | gameplay_keep | controller/invisible | yes | 133 |
| 24 | `EN_BOMBERS` | (-1401, 50, -270) | 0x0001 / 0x77FF / 0x0040 | 0 / 239 / 0 | 0x0C0 D1d, D1n | none | 0x0001 | object_cs | npc-scheduled | yes | 135 |
| 25 | `EN_TALK \| 0xE000` | (-1398, 302, -914) | 0x0107 / 0x877F / 0x007F | 2i / -90i / 0i | all | none | 0xFE13 | gameplay_keep | controller/invisible | yes | 139 |
| 26 | `EN_BOMBERS` | (-1253, 0, -39) | 0x0000 / 0x33FF / 0x000C | 0 / 103 / 0 | 0x00C D3d, D3n | none | 0x0001 | object_cs | npc-scheduled | no | 141 |
| 27 | `EN_BOMBERS` | (-1132, 0, -205) | 0x0000 / 0x827F / 0x0030 | 0 / 260 / 0 | 0x030 D2d, D2n | none | 0x0001 | object_cs | npc-scheduled | no | 145 |
| 28 | `OBJ_ENTOTU` (0x14D) | (-1080, 673, -1434) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0x0000 | object_f53_obj | billboard/effect | yes | 147 |
| 29 | `OBJ_WARPSTONE` (0x223) | (-562, 100, -1100) | 0x0007 / 0x877F / 0x007F | 0 / 270 / 0 | all | none | 0x000F | object_sek | static-mesh | yes | 151 |
| 30 | `OBJ_TOKEIDAI` (0x19C) | (-492, 555, -1012) | 0x0007 / 0x877F / 0x007F | 0 / 270 / 0 | all | none | 0x007F | object_obj_tokeidai | static-mesh | yes | 153 |
| 31 | `OBJ_TOKEIDAI` | (-280, 555, -833) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0x207F | object_obj_tokeidai | static-mesh | yes | 155 |
| 32 | `OBJ_TOKEIDAI` | (-280, 1103, -1020) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0x307F | object_obj_tokeidai | static-mesh | yes | 159 |
| 33 | `OBJ_TOKEI_STEP` (0x1B8) | (-280, 255, -853) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0x0000 | object_tokei_step | static-mesh | yes | 163 |
| 34 | `EN_FALL` (0x17C) | (-280, 11000, -1020) | 0x1907 / 0x07FF / 0x007F | 50 / 15 / 0 | all | none | 0x037F | object_lodmoon | static-mesh (camera-relative moon) | yes | 167 |
| 35 | `EN_TEST4` (0x15A) | (-227, 555, -934) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | all | 9 | 0x0C20 | gameplay_keep | controller/invisible | yes | 171 |
| 36 | `OBJ_TOKEIDAI` | (-68, 555, -1012) | 0x0007 / 0x2D7F / 0x007F | 0 / 90 / 0 | all | none | 0x007F | object_obj_tokeidai | static-mesh | yes | 175 |
| 37 | `BG_MARKET_STEP` (0x1CC) | (0, 0, 0) | 0x0005 / 0x007F / 0x002A | 0 / 0 / 0 | 0x2AA all dawns | none | 0x0000 | object_market_obj | static-mesh | yes | 179 |
| 38 | `BG_MARKET_STEP` | (0, 0, 0) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 all nights | none | 0x0001 | object_market_obj | static-mesh | no | 183 |
| 39 | `OBJ_ENTOTU` | (319, 553, -1308) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | all | none | 0x0000 | object_f53_obj | billboard/effect | yes | 187 |

"All dawns" = D0d, D1d, D2d, D3d, D4d (`0x2AA`); "all nights" = `0x155`. Tower actors #30-#33, #36, moon #34 and chimneys #28, #39 have the same params and coordinates as North's and South's (cross-check), so these exteriors share one frame for them (INF, as North U-05).

### 4.3 Alternate room header 1 (22 entries, HDR room.c:234-301, decode C)

| # | Actor | Position | Rot deg X / Y / Z | Mask | Params | Note |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | `EN_BABA` | (-1940, 165, -513) | 0 / 113 / 0 | all | 0x8000 | type `(p & 0xC000) >> 14 = 2`, path 0 (`ovl_En_Baba/z_en_baba.h:12-13`); 7 u from the Bomb Shop return start 6 |
| 1 | `OBJ_SHUTTER` | (-1553, 133, -448) | 0 / 316 / 0 | all | 0x0000 | bank shutter (2.4.2) |
| 2, 3 | `BG_MARKET_STEP` x2 | (0, 0, 0) | 0 / 0 / 0 | dawns / nights | 0x0000 / 0x0001 | as default 37, 38 |
| 4, 5 | `OBJ_ENTOTU` x2 | (319, 553, -1308), (-1080, 673, -1434) | 0 / 0 / 0 | all | 0x0000 | as default 39, 28 |
| 6-9 | `OBJ_TOKEIDAI` x4 | as default 31, 36, 30, 32 | as default | all | 0x207F, 0x007F, 0x007F, 0x307F | |
| 10 | `OBJ_TOKEI_STEP` | (-280, 255, -853) | 0 / 0 / 0 | all | 0x0000 | |
| 11-15 | `OBJ_SYOKUDAI` x5 day | (-1916, 165, -461) yaw 23; (-1434, 45, -42) yaw 337; (-1740, 200, -1358) yaw 0; (-1717, 105, -201) yaw 315; (-1529, 200, -790) yaw 0 | | dawns | 0x207F | positions differ from the default torches |
| 16-20 | `OBJ_SYOKUDAI` x5 night | same five positions and yaws | | nights | 0x287F | |
| 21 | `EN_RSN` | (-1940, 165, -513) | 0 / 113 / 0 | all | 0x0000 | `object_rsn` `gBombShopkeeperSkel` (OVL) |

### 4.4 Transition actors

The six `EN_DOOR` rows are in 2.4.1 (identical in both headers). Class `door/transition`; each is also a posed skeleton (`gDoorSkel`) for drawing.

### 4.5 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

Types already read for North (`En_Suttari`, `En_Bomjimb`, `En_Pm`, `En_Test4`, `En_Fall`, `Obj_Tokeidai`, `Obj_Tokei_Step`, `Obj_Entotu`) behave as in North section 4.4; only West-specific facts are repeated.

| Actor | Init: scale, pivot, collision, children | Draw | Presence at Day 1 noon (fresh save) |
| --- | --- | --- | --- |
| Bg_Market_Step | ICHAIN scale 1000/1000 = **1.0** (not the default 0.01); culling volume 1; no collision; source comment "West Clocktown - most scenery" | `Gfx_DrawDListOpa` (current actor matrix, `src/code/z_cheap_proc.c:6-16`): `sMarketDLs[p & 1]` then `sBankAdvertisementsAndDoorDLs[p & 1]`; day 0x1F050 + 0x1EF10, night 0x18DA0 + 0x18C60 (object_market_obj). At the origin with scale 1.0 and no rotation the DL vertices are world coordinates (INF from the matrix) | #37 by mask; #38 night only |
| Obj_Syokudai (torch) | ICHAIN scale 1.0; params `0x207F`: type `p >> 12 = 2` `NO_SWITCH`, start-lit bit `0x800` clear, group 0, switch `0x7F` none; night `0x287F` sets start-lit; stand and flame cylinders; inserts a point light above the stand | stand DL `sDLists[type]` = `gObjectSyokudaiTypeNoSwitchDL` (object_syokudai 0x870); flame `gEffFire1DL` (gameplay_keep 0x7D590, XLU, scrolling) only while lit | day variants unlit (no flame); night variants lit |
| Obj_Hariko | see 2.4.2 | | present |
| En_Ginko_Man | see 2.4.2 | | present |
| En_Talk | scale 1.0; text `(p & 0x3F) + 0x1C00`; attention range `home.rot.x - 1` (6 for rotX 7, 1 for rotX 2); talk offered within 40 u when facing | profile draw `NULL` | present, no geometry |
| Obj_Warpstone (owl statue) | ICHAIN attention only, default scale 0.01; cylinder collider; params `p & 0xF = 0xF` owl id 15, past `OWL_WARP_MAX` 0xB (`include/z64scene.h:553-567`); closed model unless `owlActivationFlags` bit 15 is set | `gOwlStatueClosedDL` (object_sek 0x1D0) or `gOwlStatueOpenedDL` 0x3770 | INF closed. It sits 22 u west of South's statue (-540, 100, -1099, params `0x0004` `OWL_WARP_CLOCK_TOWN`, `Z2_CLOCKTOWER_room_00.c:178`) and east of both West exit triggers: a West-side stand-in for South's statue (INF, U-08) |
| En_Stop_heishi | see 2.4.4 | skeletal | present |
| En_Bombers | West rows are type `(p >> 4) & 0xF = 0` `ENBOMBERS_TYPE_NPC` (`z_en_bombers.h:22`), killed only if `WEEKEVENTREG_73_10` or `85_02` (`z_en_bombers.c:155-159`); number `p & 0xF` + 1; flex `gBomberSkel` (object_cs 0xF82C), scale 0.01 | skeletal, NPC behavior (moves) | #8, #24 present (D1d mask, flags clear); #3, #4, #26, #27 other days |
| En_Bomjimb | exit index 9 (2.2); killed unless `73_10` or `85_02` and `75_40` clear (`z_en_bomjimb.c:218-220`) | skeletal | absent on a fresh save |
| En_Pm | Day 1 12:00-13:01 branch returns a state only in `SCENE_POSTHOUSE` (`ovl_En_Pm/scheduleScripts.schl:70-75`); script selection as North (`z_en_pm.c:386-404`) | drawn only for a non-none result | not drawn in West at 12:00 |
| En_Rz | params `0x7E02` / `0xFE02`: sister bit `0x8000`, path `0x3F` none, type 2 (`z_en_rz.h:7-11`); flex `object_rz_Skel_00D8D8`, scale 0.01 | skeletal | night masks only |
| En_Suttari | as North; mask here is Day 2 night only | skeletal | absent |
| En_Test4 | `0x0C20`: stars 300, bell volume index 1 (North 4.4) | none | present, no geometry |
| Obj_Shutter (alt) | see 2.4.2 | | not in default header |
| En_Baba / En_Rsn (alt) | flex `gBombShopLadySkel` / `gBombShopkeeperSkel`; credits layer only | skeletal | not in default header |

### 4.6 Counts

| | Default | Alternate 1 |
| --- | --- | --- |
| Entries | 40 | 22 |
| Day 1 daytime candidates by mask | 29 (excluded: 0, 3, 4, 7, 13, 14, 16, 17, 26, 27, 38) | 16 (excluded: 3, 16-20) |
| static-mesh (incl. torches, moon, market, owl) | 16 | 18 |
| posed-skeletal | 1 (banker) | 0 |
| npc-scheduled | 12 | 2 |
| billboard/effect | 2 | 2 |
| controller/invisible | 9 (8 En_Talk, En_Test4) | 0 |
| door/transition (scene list, not room) | 6 | 6 |

## 5. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

Assumptions to record in the profile: fresh save (no week-event, switch, owl or collectible flags), Human form. World positions N64; `world = actorPos + actorRot x (actorScale x localVertex)` (ROM_REFERENCE.md). Unity position by the documented conversion; rotation conversion under the Z mirror belongs to the importer (PIPE-01/02).

| Default # | Component / params | N64 position (Unity) | Yaw | Actor scale | Display list(s), bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| 37 | Market scenery, day, `0x0000` | (0, 0, 0) (0, 0, 0) | 0 | **1.0** | `gWestClockTownMarketDayDL` 0x1F050 + `gWestClockTownMarketBankAdvertisementsAndDoorDayDL` 0x1EF10 (object_market_obj, seg 6) | hero street geometry outside the room DLs; bank adverts included; contains a non-interactive door (XML comment) |
| 6 | Torch stand, `0x207F` | (-1902, 135, -407) (-19.02, 1.35, 4.07) | 23 | 1.0 | `gObjectSyokudaiTypeNoSwitchDL` 0x870 (object_syokudai) | unlit by day |
| 12 | Torch stand, `0x207F` | (-1618, 200, -780) (-16.18, 2.00, 7.80) | 0 | 1.0 | same | |
| 15 | Torch stand, `0x207F` | (-1682, 75, -157) (-16.82, 0.75, 1.57) | 315 | 1.0 | same | |
| 20 | Cow statue head, `0x0000` | (-1589, 143, -464) (-15.89, 1.43, 4.64) | 343 | 0.1 | `gHarikoBodyDL` 0x80 + `gHarikoFaceDL` 0x110 (object_hariko) | bank counter; rest pose |
| 31 | Tower clock, `0x207F` | (-280, 555, -833) (-2.80, 5.55, 8.33) | 0 | 0.1 | 0xCF28, 0xBEE8, 0xE818, 0xC368 (object_obj_tokeidai) | as North; visibility from West NI (U-17) |
| 30 | West exterior gear | (-492, 555, -1012) (-4.92, 5.55, 10.12) | 270 | 0.1 | `gClockTowerExteriorGearDL` 0xBA78 | |
| 36 | East exterior gear | (-68, 555, -1012) (-0.68, 5.55, 10.12) | 90 | 0.1 | same | |
| 32 | Counterweight, `0x307F` | (-280, 1103, -1020) (-2.80, 11.03, 10.20) | 0 | 0.1 | 0xB208 | omit spotlight 0xB0C0 by day |
| 33 | Closed rooftop panel | (-280, 255, -853) (-2.80, 2.55, 8.53) | 0 | 0.1 | `gClocktowerPanelDL` 0x88 (object_tokei_step) | |
| 29 | Owl statue, closed, `0x000F` (optional) | (-562, 100, -1100) (-5.62, 1.00, 11.00) | 270 | 0.01 | `gOwlStatueClosedDL` 0x1D0 (object_sek) | beyond the exit triggers; include only if visible (U-08) |
| 34 | LOD moon, `0x037F` (special) | (-280, 11000, -1020) | X50 Y15 Z0 | 0.384 | `gLodmoonEyesDL` 0x10E0 + `gLodmoonMoonDL` 0x1158 | moon rules as North/South |

Door leaves (present; transition actors; class door/transition; need the `gDoorSkel` rest pose; DLs in 2.4.1): TA0-TA5 at the positions and yaws in 2.4.1, default scale 0.01 (`EnDoor_Init` sets no scale, OVL). A fixed-leaf import must be labeled as an approximation of the skeleton's rest pose.

**Excluded by half-day mask:** 0 En_Suttari, 3/4/26/27 En_Bombers (Day 2/3), 7/13/16 lit torches, 14/17 En_Rz, 38 night market.

**Excluded by state or schedule at Day 1 noon (fresh save):** 11 En_Bomjimb (flag-gated kill), 18 En_Pm (no West state 12:00-13:01).

**Unimplemented in a static assembly (present at noon):** 21 En_Ginko_Man (`object_boj_Skel_00C240`, pose U-18), 1 En_Stop_heishi (`gSoldierSkel`), 8 and 24 En_Bombers (`gBomberSkel`, moving NPCs), the six door skeletons, chimney billboards 28 and 39 (DL 0x158 plus smoke 0x1C00; smoke on at noon as in North).

**No geometry:** 8 En_Talk spots, 35 En_Test4.

## 6. Collision summary (C from scene.c:802-1501; tool output in the scratch folder)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 284 / 388 / 20 |
| Water boxes | 0 (`0, NULL`) |
| Camera data (`BgCamInfo`) | 2: `0x0001` `CAM_SET_NORMAL0`, `0x003F` `CAM_SET_ROOM0` (`include/z64camera.h:148`, `:210`); bgCam 1 only on surface 14 (bank landing) |
| Header bounds | min (-2360, -40, -1669), max (820, 1183, 520); Unity x -23.60..8.20, y -0.40..11.83, z -5.20..16.69 |
| Actual vertex extents | x -2360..-424, y -40..995, z -1600..79 (header box larger: U-13) |
| Floor Y range | 0..255 (normal Y > 0.5) |
| Polygon kinds | 124 floors (91 under about 10 deg, 33 steeper), 260 walls (238 near vertical), 4 ceilings |
| Floor slope histogram (5-deg bins) | 0: 87, 5: 4, 10: 23, 20: 2, 25: 6, 45: 2 |
| Materials (`SURFACE_MATERIAL_*`, `z64bgcheck.h:232-242`) | dirt (types 0, 2, 5, 9, 14, 15, 18): 303; stone (3, 4, 6-8, 11-13, 16, 17, 19): 48; wood (1, 10): 37 |
| Exit surfaces | 9 types, 18 polygons (section 2.3); conveyor on the 6 walk-in exit polygons |
| Surface flags | no surface sets floor type, wall type, floor property, soft, horse-block, hookshot or wall damage: **no stair or ramp flags exist; stairs are geometry only**. Echo 2 (0, 1, 3, 7, 8, 11-13, 17), 5 (2, 4, 15, 19), 10 (5, 6, 10, 14), 0 (9, 16, 18); light setting 31 on all; floor effect 0 on all |
| Polygon flags | ignore-camera (vIA 1): 42 (23 of surface 1, 7 of surface 10, the 12 door-exit floors); ignore-projectiles (vIA 4): 364, 365, 376-379; conveyor (vIB 1): 6; vIB 4 (player only): 364, 365 |
| Wood walls | surfaces 1 and 10 (37 walls) span x -1855..-1265, Y 31..517, z -1400..-96; 30 of them ignore the camera (INF: timber facade parts the camera may pass through) |

Floor elevation bands (centroid Y of floor polygons; x/z are union extents of the band, not one platform; rows whose x reaches -513..-461 include polygons east of both South exit triggers):

| Band | Floor polys | x | Y | z | Max slope | Surface types | Thresholds and features |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Y 0 lower street | 9 | -1343..-800 | 0 | -269..35 | 0 | 5, 9, 18 | lower South exit, covered passage |
| 15-35 | 12 | -1470..-513 | 0..100 | -1000..35 | 25 | 3, 5 | treads and ramps; polys 10, 11 (x -633..-513) beyond the exits |
| 45-60 | 13 | -1661..-513 | 0..100 | -1000..79 | 25 | 3, 5, 13 | **Curiosity Shop** threshold Y 45 |
| 75-95 | 10 | -1732..-1468 | 60..105 | -398..-96 | 26 | 5 | treads and ramps |
| 105-120 | 17 | -1903..-513 | 95..135 | -1253..-122 | 26 | 4, 5, 12, 14 | **Trading Post** threshold Y 105; **bank landing** Y 105 |
| 133-150 | 11 | -1936..-1000 | 100..200 | -1200..-339 | 27 | 5, 16, 19 | **bank counter floor** Y 133 (behind the barrier) |
| 165-180 | 14 | -2074..-1000 | 100..200 | -1208..-419 | 27 | 5, 11, 19 | **Bomb Shop** threshold Y 165 |
| 195-201 upper street | 30 | -2360..-1200 | 180..201 | -1513..-657 | 12 | 0, 2, 7, 8, 9, 15, 17 | **Post Office**, **Swordsman's**, **Lottery** thresholds Y 200; gate floor; top of the upper South ramp |
| 205-255 | 8 | -1879..-461 | 200..255 | -1376..-647 | 47 | 0, 3, 5 | raised block polys 6, 7 at Y 216 (U-16); polys 16, 17 (47 deg, beyond the exits) |

Routes between levels (C unless marked):

- **Continuous floor route.** West of x = -800, 53 floor polygons form one edge-connected group from the lower street (Y 0, x -1343..-1000) to the upper street (Y 200), through ramps of 9-14 deg (polys 235-241, 217-221) and four steeper ramps of 23.5-26.3 deg (233, 234 into the bank landing; 222, 223 out of it), the bank landing (surface 14) and a Y 105 platform (176-179); the same group contains the gate floor (333, 334) and the upper South exit ramp (380, 381). Whether 23-27 deg floors are walkable without sliding: UNKNOWN (U-05).
- **Stepped descent.** 94 low walls (risers of at most 25 u, mostly 15 u) separate treads at Y 15, 30, ..., 180 across x -2035..-1175, z -769..35. Climbing a 15 u riser as a step: UNKNOWN (U-04).
- **Thresholds outside the edge-connected group** (connection by T-junction or riser: INF, confirm in WCT-01): Curiosity Shop Y 45 level (134, 150, 151, 157, 208, 209); Bomb Shop Y 165 level (133, 194-196, 204, 205); the Y 200 door floors 54-57, 370, 371; the Trading Post floor 206, 207 overlaps the Y 105 platform's XZ; the lower South passage floor 372-375 meets the Y 0 street at x = -1000.
- **NPC paths** (HDR scene.c:159-274; consumers NI) follow these routes: path 4 (22 points) from (-837, 0, -96) to (-1598, 200, -1367) in front of the Post Office; path 1 (16) from (-2038, 200, -1045) down to (-1493, 45, -39) at the Curiosity Shop; path 7 (12) back up to (-2273, 201, -1055) in the gate passage.
- No route in source requires a form or item, except the gate soldier logic (U-03). Every other form or item restriction: UNKNOWN until WCT-01 walks it.

Survey basis: ground levels, thresholds and perimeter can be measured from this collision once imported. Facades, signs, awnings and overhangs are render geometry in the 7 room DL entries and in `Bg_Market_Step`: **pending capture** (U-06, U-17).

## 7. Materials and textures (C; counts only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 50: 42 textures + 8 TLUTs |
| By format | rgba16 37 (29 textures + 8 TLUTs), ci4 8, i4 3, i8 1, ia8 1 |
| Location | all in the scene file (segment 2); the room file holds none; all 50 are referenced by `room.c` |
| Room `Gfx` display lists / `Vtx` arrays | 145 / 89 (89 `.vtx.inc` files) |
| Room shape | cullable, 7 entries: 6 opaque DLs (entry 0 has none), 7 XLU DLs |
| XML-declared DLs | `0x530` empty; `0x8CF0` calls the 6 opaque entry DLs; `0xAAB8` calls the 7 XLU entry DLs |
| Raw triangle commands | 301 `gsSP2Triangles` + 16 `gsSP1Triangle` = 618 triangles in source (DL reuse not resolved) |
| Actor-side geometry | `object_market_obj` declares 58 textures and 4 DLs (XML); 134,608 bytes |
| Animated materials | none (section 1) |

West's room file is smaller than North's (618 vs 728 source triangles); most street scenery is in `object_market_obj` (INF from the actor's own comment).

## 8. Required extraction list for the West pilot (commands NOT executed)

Done: scene `Z2_ICHIBA` (C above). Already extracted: `object_obj_tokeidai`, `object_tokei_step`. Binaries for every bank below exist under `extracted/n64-us/baserom/` (checked by `stat`; bytes in parentheses).

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_market_obj` (134,608), `object_syokudai` (15,504), `object_hariko` (1,056), `object_lodmoon` (15,904), `gameplay_keep` (599,520) | section 5 DLs; `gDoorSkel`, keep door DLs, `gEffFire1DL` |
| Door leaves | `object_wdor01`, `object_wdor02`, `object_wdor03` (5,808 each) | venue leaf DLs at 0x548 |
| Optional / billboard | `object_sek` (19,056), `object_f53_obj` (14,960) | owl statue; chimneys |
| Later skeletal pass | `object_boj` (79,808), `object_sdn` (56,464), `object_cs` (68,480) | banker, gate soldier, Bombers |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_market_obj','object_syokudai','object_hariko','object_lodmoon','gameplay_keep','object_wdor01','object_wdor02','object_wdor03','object_sek','object_f53_obj')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

`gameplay_keep` is shared with North's list (extract once). `object_market_obj` is the largest West input and has not been extracted before: check output size and warnings separately (NI).

## 9. Proposed region record

**Proposal, schema not frozen (PIPE-01).** Field set follows FULL_CITY_PLAN section 3.2 and North's proposal; `null` or `"UNKNOWN"` where source does not settle a value.

```json
{
  "_status": "proposal, schema not frozen (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.west_clock_town",
  "displayLabel": "West Clock Town",
  "source": {
    "sceneSymbol": "Z2_ICHIBA",
    "sceneEnum": "SCENE_ICHIBA",
    "sceneTableIndex": "0x6D",
    "entranceTableSceneIndex": "0x6A",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0)",
    "rooms": { "loc.west_clock_town.room_00": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event, switch, owl or collectible flags", "player form Human", "banker pose UNKNOWN (distance-driven)"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_ICHIBA", "bytes": 89664, "sha256": "c77e9020c6e4506471b3806f4eeccefdea10db606258aee4d424f6803a84a67c" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_ICHIBA_room_00", "bytes": 43776, "sha256": "1612d4f23bd374bad4b2222344003089d8c8e5235448b77bcaa83ebeb5be594b" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_ICHIBA/Z2_ICHIBA.c", "bytes": 101022, "sha256": "6039359726019489c9f9b988ca4e9970cd8ecf62e5d466e31623ccd14c8614ac" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_ICHIBA/Z2_ICHIBA_room_00.c", "bytes": 124312, "sha256": "cf4d562a60adad815ee5a690fb1473c02f00ecdecfe257029c06fe0b6d235efb" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_ICHIBA/Z2_ICHIBA.xml", "bytes": 459, "sha256": "967540b42d99aeed2aad40be02c52a8cf8282898c1f116e156ffc0801935bdd9" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedWithSouth": "INFERRED (identical tower/chimney/moon anchors; exit AABBs coincide), unverified" },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-2360, -40, -1669], "max": [820, 1183, 520] },
    "sourceCollisionVertices": { "min": [-2360, -40, -1600], "max": [-424, 995, 79] },
    "unityCollisionVertices": { "min": [-23.60, -0.40, -0.79], "max": [-4.24, 9.95, 16.00] }
  },
  "anchors": [
    { "id": "anchor.wct.start0_gate", "source": [-2076, 200, -1086], "unity": [-20.76, 2.00, 10.86], "status": "proposed, not surveyed" },
    { "id": "anchor.wct.start1_south_lower", "source": [-1007, 0, -107], "unity": [-10.07, 0.00, 1.07], "status": "proposed, not surveyed" },
    { "id": "anchor.wct.start2_south_upper", "source": [-1253, 200, -1095], "unity": [-12.53, 2.00, 10.95], "status": "proposed, not surveyed" },
    { "id": "anchor.wct.banker", "source": [-1559, 133, -438], "unity": [-15.59, 1.33, 4.38], "status": "proposed, not surveyed" },
    { "id": "anchor.wct.door_curiosity_ta5", "source": [-1510, 45, -12], "unity": [-15.10, 0.45, 0.12], "status": "proposed, not surveyed" }
  ],
  "rooms": [ { "id": "loc.west_clock_town.room_00", "sourceRoom": "Z2_ICHIBA_room_00", "roomShape": "cullable, 7 entries", "displayLists": 145 } ],
  "components": [
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a37", "actorId": "0x1CC", "params": "0x0000", "class": "static-mesh", "dls": ["object_market_obj:0x1F050", "object_market_obj:0x1EF10"], "status": "selected, actor scale 1.0" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a06", "actorId": "0x039", "params": "0x207F", "class": "static-mesh", "dls": ["object_syokudai:0x870"], "status": "selected, unlit" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a12", "actorId": "0x039", "params": "0x207F", "class": "static-mesh", "dls": ["object_syokudai:0x870"], "status": "selected, unlit" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a15", "actorId": "0x039", "params": "0x207F", "class": "static-mesh", "dls": ["object_syokudai:0x870"], "status": "selected, unlit" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a20", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80", "object_hariko:0x110"], "status": "selected" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a31", "actorId": "0x19C", "params": "0x207F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "0xBEE8", "0xE818", "0xC368"], "status": "selected" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a30", "actorId": "0x19C", "params": "0x007F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xBA78"], "status": "selected" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a36", "actorId": "0x19C", "params": "0x007F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xBA78"], "status": "selected" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a32", "actorId": "0x19C", "params": "0x307F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xB208"], "status": "selected" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a33", "actorId": "0x1B8", "params": "0x0000", "class": "static-mesh", "dls": ["object_tokei_step:0x88"], "status": "selected" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a29", "actorId": "0x223", "params": "0x000F", "class": "static-mesh", "dls": ["object_sek:0x1D0"], "status": "optional, beyond exit triggers" },
    { "sourceKey": "Z2_ICHIBA/h0/room_00/a34", "actorId": "0x17C", "params": "0x037F", "class": "static-mesh (camera-relative)", "dls": ["object_lodmoon:0x10E0", "object_lodmoon:0x1158"], "status": "selected, special rule" },
    { "sourceKey": "Z2_ICHIBA/h0/ta0-ta5", "actorId": "0x005", "params": "0x0281,0x0280,0x0282,0x0286,0x0283,0x0284", "class": "door/transition (posed skeleton)", "dls": ["object_wdor02:0x548", "object_wdor03:0x548", "object_wdor01:0x548", "gameplay_keep:0x20BB8/0x20D00", "object_wdor02:0x548", "object_wdor01:0x548"], "status": "selected, rest pose approximated" }
  ],
  "collisionOwnership": { "room": "Z2_ICHIBACollisionHeader_003A28 (388 polys)", "actorDynapoly": [], "actorCylinders": "NI (torches, owl statue, NPCs)" },
  "portalIds": [
    "portal.west_clock_town.south_clock_town.lower",
    "portal.west_clock_town.south_clock_town.upper",
    "portal.west_clock_town.z2_00keikoku",
    "portal.west_clock_town.post_office",
    "portal.west_clock_town.swordsmans_school",
    "portal.west_clock_town.lottery_shop",
    "portal.west_clock_town.bomb_shop",
    "portal.west_clock_town.trading_post",
    "portal.west_clock_town.curiosity_shop",
    "portal.west_clock_town.north_clock_town.bomjimb"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a21 En_Ginko_Man", "a01 En_Stop_heishi", "a08/a24 En_Bombers", "a28/a39 Obj_Entotu billboards and smoke", "door skeleton animation", "Link and Tatl"],
  "excluded": { "halfDayMask": ["a00", "a03", "a04", "a07", "a13", "a14", "a16", "a17", "a26", "a27", "a38"], "stateOrSchedule": ["a11", "a18"], "noGeometry": ["a02", "a05", "a09", "a10", "a19", "a22", "a23", "a25", "a35"] },
  "unsupported": ["skeletal NPC poses", "door skeleton pose", "billboard orientation", "smoke vertex alpha and texture scroll", "conveyor exit floors", "camera setting switch on surface 14", "player-only barrier polys 364/365", "positional lights", "moon 9000-unit rule unless implemented"],
  "evidence": ["docs/city/districts/west_clock_town.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

## 10. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Sign texts 0x1C0A-0x1C14 (which frontage each `En_Talk` names) | The tree has no extracted message text (`assets/text/message_data.h` includes a generated file). Extract `message_data_static` and read entries 0x1C0A, 0x1C0D-0x1C10, 0x1C13, 0x1C14 |
| U-02 | Start bgCam indices 0x02-0x0A exceed the 2-entry `BgCamInfo` list | As North U-02: what consumes `PLAYER_GET_BG_CAM_INDEX`; do they index the 11-entry actor cutscene camera list |
| U-03 | Gate passage for Human on Day 1 noon, and for other forms | Read `func_80AE7E9C` and the action functions in `ovl_En_Stop_heishi/z_en_stop_heishi.c:175-530`: how the soldier blocks or pushes the player on surface 15 and what `unk_265` / `WEEKEVENTREG_12_20` release |
| U-04 | 15 u risers in the stepped descent | Player step-up / ledge thresholds in `ovl_player_actor/z_player.c` and the `z_bgcheck.c` floor checks; then a WCT-01 walk |
| U-05 | 23-27 deg floors (polys 222, 223, 233, 234, 380, 381) | Player slope-slide threshold in `z_player.c`; then a WCT-01 walk up the upper South ramp and through the bank landing |
| U-06 | Facades, frames, awnings, overhangs, signs, and which venues share facade geometry | Render the 7 room DL entries and `Bg_Market_Step` 0x1F050 / 0x1EF10 after extraction; map each surface to the nearest door |
| U-07 | `Bg_Market_Step` contains a door the XML calls non-interactive and invisible | After extraction, locate that door in 0x1EF10 and confirm no transition actor or exit is near it |
| U-08 | Owl statue copy `0x000F` (owl id 15) 22 u from South's statue | Does any code set `owlActivationFlags` bit 15; is (-562, 100, -1100) visible from inside West's exits (render check) |
| U-09 | Venue hours on other days and kickout interplay | REF-02 U9 / REF-09: run each `sDoorSch_*` per profile (Day 1 noon is settled in 2.4.1) |
| U-10 | `En_Bomjimb` #11 trigger | REF-02 U11 (`z_en_bomjimb.c:859`) |
| U-11 | Floor polygons east of both South exits (polys 10-13, 16, 17, surfaces 2-4 near x -1000..-461) | Overlay with South's collision in the shared frame; confirm they are unreachable edge geometry |
| U-12 | Threshold floors not edge-connected (Curiosity Y 45, Bomb Shop Y 165, door floors, lower passage) | Edge connectivity with T-junction tolerance on import, then WCT-01 routes to each threshold |
| U-13 | Header bounds larger than vertex extents (max x 820 vs -424) | As North U-14: how `BgCheck` uses min/max bounds before using them as region bounds |
| U-14 | Light settings, cutscene texts 0x2927-0x2929, actor cutscene entries, the title-card script for starts 1 and 2 | Read `Z2_ICHIBACutsceneData_000650`, `_0014B0`, `_001670` and the 15-entry actor cutscene list |
| U-15 | Banks with no listed consumer (`object_kibako`, `object_os_anime`) | Which runtime-spawned actors in West use them (children of listed actors, cutscene actors) |
| U-16 | Raised block, polys 0-7 at x -1441..-1400, Y 200..216, z -1376..-1297 | Identify the render geometry above it (room DL or market DL) |
| U-17 | Visual survey and tower visibility | Capture arrival (both South approaches), departure, main silhouette, reverse, bank frontage, each storefront and the gate under the Day 1 noon profile (REF-03 `cam.west_clock_town.*`) |
| U-18 | Banker pose for the static profile | Choose sitting (init) or advertising (player beyond 160 u) in the profile; read `object_boj_Anim_004A7C` after extraction |
| U-19 | Door rest pose | Confirm frame 0 of `gDoorFierceDeityZoraOpenLeftAnim` is closed after `gameplay_keep` extraction |

Register notes (no register file edited): (a) PORTAL_REGISTER section 6 "Teleport-only spawns ... West 0, 9": West has start positions 0-8 only; "9" is entrance slot 9, whose `EntranceEntry` row 9 lands on start position 0 (scene.c:82). (b) LOCATION_REGISTER U-05 and SOURCE_REGISTER U-02 can close for West (section 1.1). (c) SOURCE_REGISTER U-09: West's default list has the moon (`En_Fall` #34); the alternate list has none. (d) LOCATION_REGISTER's name-based bank INF now has OVL and XML evidence (2.4.2). No value disagreement found with REF-01, REF-02 or REF-03.

**Not inspected:** room DL contents, combiners and geometry; `object_market_obj` contents; light-setting values; cutscene data semantics; minimap rendering; actor collider shapes; En_Suttari, En_Rz, En_Baba, En_Rsn and En_Pm behavior beyond init/draw/schedule gates; gate soldier action logic in full; message text; `Z2_00KEIKOKU`; any 3DS material.

**Checks run:** read the listed docs and North's template; read scene/room C, scene XML, actor/object/scene tables, `z_actor.c`, `z_scene.c`, `z_scene_table.c`, `z_demo.c`, `z_schedule.c`, `z_cheap_proc.c`, `z64scripts.h`, `z64bgcheck.h`, `z64player.h`, `z64door.h`, `z64scene.h`, `z64camera.h`, object XMLs (`object_market_obj`, `object_hariko`, `object_f53_obj`, `object_syokudai`, `object_sek`, `object_boj`, `object_wdor01-03`, `gameplay_keep`, `object_sdn`, `object_cs`) and the overlays named above; scratch Python parsers (`actors.py`, `col.py`, `bands.py`, `bands2.py`, `main_group.py`, `doors.py`, outputs in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref06\`) for the 40/22/9/1/6 entry decode, collision counts, bands, edge connectivity and door-local geometry; SHA-256 of the five inputs and the extractor; `stat` of the bank binaries. Cross-check: counts agree with `docs/city/LOCATION_REGISTER.md` row `loc.west_clock_town` (18/40, 9/22, 6 transitions, 9/1 spawns, 10/10 exits, 10 entrances) and every West row of `docs/city/PORTAL_REGISTER.md`.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes, entrance decoding beyond the labeled cross-check, message extraction.

**Next dependency:** REF-03 capture profile with the banker-pose and owl-flag fields (U-18, U-08); PIPE-01 schema review of section 9; then the section 8 extraction and WCT-01 assembly, whose first walk tests are U-04, U-05 and U-12.
