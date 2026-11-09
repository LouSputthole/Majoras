# Clock Tower Rooftop (`loc.clock_tower_rooftop`): source inventory dossier

Task **REF-09 / INT-17** (FULL_CITY_PLAN.md section 12 row REF-09, section 6 row INT-17: "Roof and access presentation tied to the opening state, skyline and safe boundaries"; acceptance "Opening/final-night behavior supported; final boss/moon campaign remains external to city scope"). Base commit `c8b847e` on `main`; IDs below were checked against its `shared/city/ids.json`. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_OKUJOU` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. **The venue is closed at `prof.d1.noon.normal`** (section 6); the static list is written for the first open profile, `prof.d3.final.normal` (Day 3, 01:00, scene layer 0), with the Ocarina held (SOURCE_REGISTER section 3 baseline owner decision).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_OKUJOU/Z2_OKUJOU.c`, `room.c` = `.../Z2_OKUJOU_room_00.c`, `player.c` = `src/overlays/actors/ovl_player_actor/z_player.c`; an overlay `z_<name>.c` lives in `src/overlays/actors/ovl_<Name>/`. Line numbers count `\n` only (`grep -n`; neither file holds a `\r`). N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro/function; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected. Senders that live in non-extracted scenes (`SPOT00`, `Z2_00KEIKOKU`) come from a read-only scan of scene-header command words in `extracted/n64-us/baserom/` (cutscene-script list `0x17` and actor-cutscene list `0x1B`; numbers only, nothing copied); they are tagged **C-BIN**. The same scan of `Z2_OKUJOU` reproduces the extracted C exactly (check, section 11).

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_OKUJOU` / `SCENE_OKUJOU` | HDR `include/tables/scene_table.h:57` |
| Scene-table index | `0x19` | same line |
| Entrance-table scene index | `0x16` (`ENTR_SCENE_CLOCK_TOWER_ROOFTOP`); table `sClockTowerRooftopEntranceTable` | `include/z64scene.h:615`; `src/code/z_scene_table.c:407-431, 2595` |
| Title text / draw config | `0` / `SCENE_DRAW_CFG_DEFAULT` | scene_table.h:57 |
| Restrictions | `RESTRICTIONS_SET(0,0,0,0,0,3,3,3,3,3,0,0)`: hGauge, B, A, trade items, **Song of Time 0**; Double Time, Inverted Time, Soaring, Storms, **masks 3**; picto box, all 0 | field order C `src/code/z_parameter.c:66-71`; meaning of 3 is LOCATION_REGISTER U-13 (INF: blocked) |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_NONE` | scene_table.h:57 |
| Negative scene id | every entrance row uses `-SCENE_OKUJOU`; `Entrance_GetSceneIdAbsolute` strips the sign; in the final hours a negative destination mutes the BGM on the transition even if the row lacks flag `0x8000` | `z_scene_table.c:408-425, 2706-2713`; C `src/code/z_play.c:583-595` (resolves LOCATION_REGISTER note 2) |
| XML files | scene seg 2; `room_00` seg 3, extra DL `0x2F50` (wrapper of the 5 room DLs) | XML `assets/xml/scenes/Z2_OKUJOU/Z2_OKUJOU.xml`; room.c:707-714 |
| Rooms | 1 (`SCENE_CMD_ROOM_LIST(1, ...)` in all four headers) | HDR scene.c:16, 3868, 4110, 6767 |
| Collision | one header `Z2_OKUJOUCollisionHeader_00AC00` in all four headers | HDR scene.c:19, 3871, 4113, 6770 |
| Transition actors | none (no `TRANSITION_ACTOR_LIST` command in any header) | HDR scene.c:13-31, 3866-3882, 4108-4124, 6765-6780 |
| Exit list | **none in any header** (no `EXIT_LIST` command) | same lines |
| Path list | default only: 1 path, 2 points (0, 4000, -200) -> (0, 3000, -200); no reader found (U-09) | HDR scene.c:22, 122-129 |
| Point lights | 3 `LIGHT_POINT_NOGLOW`, radius 400: (-320, 110, 180) blue, (0, 110, -360) red, (320, 110, 180) green; same in every header | HDR scene.c:46-50; `include/z64light.h:75` |
| Env light settings | 29 per header (values not transcribed) | HDR scene.c:25, 3876, 4118, 6775 |
| Minimap / animated material | minimap count 0; animated material `{ 0, 6, NULL }` = segment 0, which `AnimatedMat_DrawMain` skips (no texture animation) | HDR scene.c:3590-3604; C `src/code/z_scene_proc.c:399-406` |
| Room commands (all 4 room headers) | echo 4; `ROOM_BEHAVIOR(0x01, 0x00, 0, 0, false, 0)`; `SKYBOX_DISABLES(false, false)` (sky, sun and moon drawn); `TIME_SETTINGS(255, 255, 0)`: no forced time, **time speed 0**; cullable shape, 5 entries | HDR room.c:14-19, 111-115, 148-152, 209-213; C `src/code/z_scene.c:386-405` |

### 1.1 Header inventory (all four headers; HDR scene.c and room.c line ranges)

| Header | Layer / `nextCutsceneIndex` | Scene cmds (line) | Sound (spec, ambience, seq) | Skybox (file, id, config, lightMode) | Spawns | Entrance entries | Scripts / actor cutscenes | Room objects / actors |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| default `Z2_OKUJOUCommands` | 0 / any `< 0xFFF0` | 13-31 | 2, 19, 0 (`NA_BGM_GENERAL_SFX` = no area BGM) | 0, 1, 0, false (time-driven light) | 2 | 2 (:57-58) | 8 / 17 | 6 / 17 (room.c:13-91) |
| alt1 `Z2_OKUJOUSet_00AC30` | 1 / `0xFFF0` | 3866-3882 | 2, 19, 0 | 0, 1, **9**, **true** (`LIGHT_MODE_SETTINGS`) | 1 | 2 (:3900-3901; [1] pad, INF) | 1 / 10 | 8 / 4 (room.c:110-145) |
| alt2 `Z2_OKUJOUSet_00B5A0` | 2 / `0xFFF1` | 4108-4124 | 2, 19, 0 | 0, 1, 0, false | 3 | 4 (:4146-4149; [3] pad, INF) | 3 / 12 | 8 / 12 (room.c:147-206) |
| alt3 `Z2_OKUJOUSet_012CC0` | 3 / `0xFFF2` | 6765-6780 | 2, 19, **56** (`NA_BGM_MINI_BOSS`) | 0, 1, 0, false | 1 | 2 (:6798-6799; [1] pad, INF) | **none** / 9 | 6 / 13 (room.c:208-265) |

Sequence ids from `include/tables/sequence_table.h:20, 132` (0-based order). Skybox fields C `src/code/z_scene.c:373-377`; `include/z64environment.h:48-49, 96`. Pads: each list would otherwise end 2 bytes short of the 4-aligned light-settings block that follows it (INF from symbol offsets `0xACEC`/`0xACF0`, `0xB67C`/`0xB684`, `0x12D74`/`0x12D78`).

What each alternate changes (C, from the lists in section 4): **alt1** keeps only Skull Kid, the camera-attached Eff_Stk, the star tag and the clock-tower moon; adds `OBJECT_DMASK` and `OBJECT_FALL2` banks that no alt1 actor uses (INF: needed by the cutscene's actors elsewhere, U-10). **alt2** is the moon-warp layer: Majora's Mask prop (Dm_Char05), success giants, the warp beam (En_Fall2), a stopped closed-mouth moon and an open-mouth moon. **alt3** is the giants-failed layer: failure giants at Y -720, a falling-debris effect, a Tatl hint tag at the stair head, the mini-boss BGM and no cutscene script list.

### 1.2 What selects each alternate (LOCATION_REGISTER U-05; `sceneLayer = (cutsceneIndex & 0xF) + 1` for `>= 0xFFF0`, `src/code/z_play.c:2210-2219`; a cutscene destination word with layer n writes `nextCutsceneIndex = n + 0xFFEF`, `src/code/z_demo.c:599-619`)

| Header | Writer of the entrance + layer (source) | Entrance word | Story trigger |
| --- | --- | --- | --- |
| default | South exit list [8] `0x2C00` (floor exit, section 2.3) | `0x2C00` e0 L0 | walk up the opened tower stair |
| default | `SPOT00` alt3 script 0, spawn 0, `CS_SPAWN_FLAG_ALWAYS` (C-BIN); that `SPOT00` layer is written by the Ocarina get-item: `ENTRANCE(CUTSCENE, 0)` + `0xFFF2` when the get-item text closes (`player.c:13675-13684`) | `0x2C10` e1 L0 | return to the roof after the Song of Time lesson (section 5.3) |
| alt1 | `SPOT00` alt9 script 1, spawn 1 ALWAYS (C-BIN); `SPOT00` L9 spawn 1 is the destination `0x1C19` of interior script 2, the Deku Mask cutscene (`clock_tower_interior.md` 5.2) | `0x2C01` e0 L1 | flashback chain after the Deku Mask: alt1 script 0 (ALWAYS, spawn 0) sends on to `0x1C29` = `SPOT00` e2 L9, whose script 2 enters the interior at `0xC060` (C-BIN, INT-16 5.2) |
| alt2 | `Z2_00KEIKOKU` alt1 csId 11 = script 2 (C-BIN), started by En_Fall when all four boss remains are held and `WEEKEVENTREG_93_04` is clear (sets it) | `0x2C02` e0 L2 | first moon-warp cutscene: rooftop alt2 script 0 (spawn 0) |
| alt2 | `Z2_00KEIKOKU` alt1 csId 12 = script 3 (C-BIN), same test with `WEEKEVENTREG_93_04` set (`z_en_fall.c:331-338`) | `0x2C12` e1 L2 | repeat moon-warp cutscene: alt2 script 1 (spawn 1) |
| alt2 | `Z2_00KEIKOKU` alt1 csId 13 = script 4 (C-BIN); csId 13 is only reachable as csId 12's additional cutscene; no `CutsceneManager_*(13` call in `src/` (U-02) | `0x2C22` e2 L2 | alt2 script 2 (spawn 2) |
| alt3 | `Z2_00KEIKOKU` alt1 script 0 (giants cutscene, spawn 0 ALWAYS) destination `0x2C03` (C-BIN), and En_Fall's hardcoded `ENTRANCE(CLOCK_TOWER_ROOFTOP, 0)` + `0xFFF2` after frame 1600 of that script when not all four remains are held | `0x2C03` / `0x2C00`+`0xFFF2` | giants fail to stop the moon (`z_en_fall.c:317-356`) |

The Termina Field giants cutscene is reached from the rooftop default header: Oath to Order -> csId 12 = script 3 -> `0x5401` = `Z2_00KEIKOKU` e0 L1 (section 5.4). The scan covered all 103 baserom scene files (`scan.py`); no other `0x2Cxx` sender exists. Only `ENTRANCE(CLOCK_TOWER_ROOFTOP, 0)` appears in `src/` (Dm_Stk compare, En_Fall, map select).

## 2. Spawns, entrances and exits

### 2.1 Start positions and entrance rows (HDR scene.c:39-44, 3884-3887, 4126-4133, 6782-6785; `z_scene_table.c:407-425`; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`; modes 0xD and 0xF unnamed). Yaw = `(rawRotY >> 7) & 0x1FF`.

| Spawn | Position (Unity) | Yaw | Params (mode) | In headers (line) | Floor (C, collision) |
| --- | --- | --- | --- | --- | --- |
| 0 | (0, 20, 222) (0.00, 0.20, -2.22) | 180 (faces -Z, into the roof) | 0x0DFF (0xD) | default :40, alt1 :3885, alt2 :4127, alt3 :6783 | stair-head landing polys 100-107, Y 20 |
| 1 | (0, 0, 0) (0.00, 0.00, 0.00) | 180 | 0x0DFF (0xD) | default :42, alt2 :4129 | roof center, polys 118/120/121/128-131 meet at Y 0 |
| 2 | (1, 9, 95) (0.01, 0.09, -0.95) | 180 | 0x0FFF (0xF) | alt2 :4131 only | polys 129/130 (Y 0..15) |

| Entrance row | Layer 0 | Layer 1 | Layer 2 | Layer 3 |
| --- | --- | --- | --- | --- |
| `sClockTowerRooftopEntrance0` (L408) spawn 0 | `0x0102` default | `0x8102` alt1 | `0x0102` alt2 | `0x0102` alt3 |
| `sClockTowerRooftopEntrance1` (L415) spawn 1 | `0x0102` default | `0x0102` alt1 (spawn list has 1 entry; entrance entry [1] is `{0,0}`, a pad that would place spawn 0, INF) | `0x0102` alt2 | - |
| `sClockTowerRooftopEntrance2` (L421) spawn 2 | `0x858B` default: **gap** (2-entry list; [2] reads the first light-setting bytes) | `0x858B` alt1: **gap** | `0x8102` alt2: valid | - |

**LOCATION_REGISTER U-04 (rooftop e2):** entrance 2 is valid only on layer 2 (`0x2C22`, alt2 spawn 2). No sender writes `0x2C20` or `0x2C21` (C-BIN scan of 103 scenes, grep of `src/` and all 22 extracted scenes). Mark e2 L0/L1 unused.

### 2.2 Exit list: none, and what moves the player off the roof (PORTAL_REGISTER U2, LOCATION_REGISTER U-07)

No header has an `EXIT_LIST` (section 1), and no surface type carries an exit index (all three have exit+1 = 0, section 7). The stair head (Y 11..27, z 115..330) is a dead end: the room collision stops at Y 0 and holds no descending tread (C). The roof is left only by code:

| # | Site (source) | Header / condition | Writes | Destination | Notes |
| --- | --- | --- | --- | --- | --- |
| X1 | default script 3 `CS_DESTINATION_DEFAULT` frames 675-701 (scene.c:1450, list :3563) | default; csId 12, started by the Oath to Order time tag (5.4) | `0x5401` -> `nextCutsceneIndex 0xFFF0` | `Z2_00KEIKOKU` e0 L1 (giants cutscene) | `ENTR_SCENE_TERMINA_FIELD` 0x2A (`z64scene.h:635`) |
| X2 | moon-crash timer stop, `TIMER_STATE_STOP` for `TIMER_ID_MOON_CRASH` (`src/code/z_parameter.c:5868-5876`) | any header except alt3 | `day = 4`; `Interface_StartMoonCrash`: `ENTRANCE(TERMINA_FIELD, 12)`, time 06:00+10, fade white (`:5612-5624`) | `Z2_00KEIKOKU` e12 | the 5-minute (default) timer runs out |
| X3 | same site, alt3 branch (`z_parameter.c:5870-5873`) | alt3 (`sceneLayer == 3`) | `day = 4`; `ENTRANCE(TERMINA_FIELD, 1)` + `0xFFF0` | `Z2_00KEIKOKU` e1 L1 | the 1-minute timer runs out after the giants fail |
| X4 | Song of Time, `OCARINA_MODE_APPLY_SOT` (`player.c:17472-17486`) | any header; restriction allows it (section 1) | `ENTRANCE(CUTSCENE, 1)` if `threeDayResetCount == 1`, else `ENTRANCE(CUTSCENE, 0)`; `0xFFF7` | `SPOT00` e1 or e0, L8 (new cycle) | generic, not rooftop-specific |
| X5 | Ocarina get-item closes (`player.c:13675-13684`) | default, Dm_Char02 present (no Ocarina) | `ENTRANCE(CUTSCENE, 0)` + `0xFFF2`, fade white | `SPOT00` e0 L3 | that `SPOT00` script returns to `0x2C10` (1.2) |
| X6 | alt1 script 0 destination frames 67-68 (scene.c:4073, list :4080) | alt1, spawn 0 ALWAYS | `0x1C29` -> `0xFFF8` | `SPOT00` e2 L9 | flashback chain (1.2) |
| X7 | alt2 scripts 0/1/2 destinations (scene.c:5296, 6033, 6706; list :6737-6739) | alt2, spawn 0/1/2 ALWAYS | `0xC800` | `Z2_SOUGEN` (The Moon, `ENTR_SCENE_THE_MOON` 0x64, `z64scene.h:693`) e0 L0 | out of city scope (FULL_CITY_PLAN INT-17) |
| - | default scripts 4, 5 (`0x6E00`, scene.c:3564-3565) | none: no actor-cutscene row uses scripts 4 or 5 (rows list scripts 0, 1, 2, 3, 6, 7) | `0x6E00` = scene 0x37 `SCENE_ENTRANCE_NONE` (`z_scene_table.c:2628`) | invalid | unreferenced copies (INF; script 4 matches alt2 script 0 in length, command count and destination frames) |

**Disagreement with the dispatch:** `ovl_En_Fall/z_en_fall.c:346` does not move the player off the roof; it sends the player from Termina Field **onto** the roof (alt3). `ovl_Dm_Stk/z_dm_stk.c:1093` is a comparison of the arrival entrance, not a writer. No site in `src/` writes a `SOUTH_CLOCK_TOWN` entrance from the rooftop; X4 reaches South only through the new-cycle `SPOT00` sequence (NI where it lands).

### 2.3 Way in from South Clock Town (cross-check: `south_clock_town.md` 2.1, 6.5, R17; PORTAL_REGISTER rows 128, 175, U1)

| Item | Value | Evidence |
| --- | --- | --- |
| Trigger | South exit list [8] `0x2C00` (default :98, alt1 :2337, alt3 :2905 of `Z2_CLOCKTOWER.c`); 22 polys with exit+1 = 9 (surface types 4, 5, 17), treads Y 300..375, x -340..-220, z -1053..-893 | PORTAL_REGISTER row 175 (not re-decoded here) |
| Destination | e0 L0 `{ -SCENE_OKUJOU, 0, 0x0102 }` -> default header spawn 0 (0, 20, 222), yaw 180, mode 0xD | `z_scene_table.c:408`; scene.c:40 |
| Gate 1: closed panel | `Obj_Tokei_Step` loads `gClocktowerPanelCol` (a vertical 120 x 120 plane at the first tread) unless `(CURRENT_DAY == 3 && t < 06:00) || day >= 4`, or South layer 2 script 0 (opening) | OVL `ovl_Obj_Tokei_Step/z_obj_tokei_step.c:191-208` |
| Gate 2: staircase draw | `Obj_Tokeidai` type 11 draws only when Day 3 before 06:00, day >= 4, or `WEEKEVENTREG_CLOCK_TOWER_OPENED`; it has no collision | OVL `z_obj_tokeidai.c:615-621`; `south_clock_town.md` row 18 |
| Agreement | PORTAL_REGISTER row 175 and the South dossier agree on exit index, polygons and arrival; this resolves PORTAL_REGISTER U1 to the panel dynapoly (gate 1) | C |

### 2.4 Portal IDs (`shared/city/ids.json` at `c8b847e`)

| Portal ID (frozen) | Mode | Direction | Source |
| --- | --- | --- | --- |
| `portal.south_clock_town.clock_tower_rooftop` | special | inbound only | 2.3 |

**No portal departs this venue in `ids.json`**, so `portalIds` is `[]` (section 10; the schema allows an empty list: `"portalIds": { "type": "array", ... }` with no `minItems`). Proposed rows for the PORTAL_REGISTER owner (`special` mode, hardcoded, not edited here):

| Proposed ID | Site | Destination | Comment |
| --- | --- | --- | --- |
| `portal.clock_tower_rooftop.z2_00keikoku.oath` | X1 | `ext.z2_00keikoku` e0 L1 | cutscene destination |
| `portal.clock_tower_rooftop.z2_00keikoku.moon_crash` | X2 | `ext.z2_00keikoku` e12 | timer |
| `portal.clock_tower_rooftop.z2_00keikoku.giants_failed` | X3 | `ext.z2_00keikoku` e1 L1 | alt3 timer |
| `portal.clock_tower_rooftop.south_clock_town.song_of_time` | X4 | via `SPOT00` new cycle (needs an `ext.spot00` or a South alias decision) | the dispatch's `south_clock_town.<qualifier>` form; the only exit that ends in town |
| `portal.clock_tower_rooftop.spot00.ocarina` | X5 | `SPOT00` e0 L3, returns to rooftop e1 | needs `ext.spot00` |
| `portal.clock_tower_rooftop.spot00.flashback` | X6 | `SPOT00` e2 L9 | needs `ext.spot00` |
| `portal.clock_tower_rooftop.z2_sougen.moon` | X7 | The Moon | needs `ext.z2_sougen`; out of scope |

## 3. Object banks (HDR room.c:31-38, 121-130, 158-167, 219-226; IDs `include/tables/object_table.h`)

| Bank | ID | default | alt1 | alt2 | alt3 | Consumer (OVL) |
| --- | --- | --- | --- | --- | --- | --- |
| `OBJECT_STK` | 0x192 | 0 | 0 | 0 | 0 | Dm_Stk skeleton `gSkullKidSkel` 0x13328, call-down-moon anims (`z_dm_stk.c:307-323`) |
| `OBJECT_FALL` | 0x188 | 1 | 1 | 1 | 1 | En_Fall types 7, 9, 4 (`gMoonDL` 0x77F0, debris) (`z_en_fall.c:161-181`) |
| `OBJECT_RACETSUBO` | 0x19C | 2 | 2 | 2 | 2 | Obj_Tsubo type 1 magic pot `gMagicPotDL` 0x278 (`z_obj_tsubo.c:67-75`) |
| `OBJECT_STK2` | 0x1BE | 3 | 3 | 3 | 3 | Dm_Stk floating anims; Dm_Char02 Ocarina skeleton 0xAD68; Eff_Stk DLs |
| `OBJECT_DELF` | 0x1C8 | 4 | 4 | 4 | 4 | Dm_Char00 Tatl/Tael anims |
| `OBJECT_GIANT` | 0x1B8 | 5 | 5 | 5 | 5 | En_Giant `gGiantSkel` 0x79B0 |
| `OBJECT_DMASK` | 0x213 | - | 6 | 7 | - | Dm_Char05 type 3 (alt2) |
| `OBJECT_FALL2` | 0x269 | - | 7 | 6 | - | En_Fall2, En_Fall type 10 (alt2) |

Implicit: `gameplay_keep` (En_Time_Tag, En_Weather_Tag, Elf_Msg6 profile object), `gameplay_field_keep` (special file, scene.c:21). Dm_Stk also asks for `OBJECT_STK3` (giants-era anims, `z_dm_stk.c:1068-1073, 313-314`), which no rooftop list loads (INF: those anims are not used here).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): ID `id & 0x1FFF`; each axis `(raw >> 7) & 0x1FF` degrees unless the ID carries the axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), which passes that axis as a plain integer; csId `rawRotY & 0x7F` (0x7F = none, 3570-3574); half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 = all (3576-3580); Day 1 12:00 bit `0x080`, Day 3 01:00 bit `0x004` (2491-2499). Raw rot = `(a << 7) | b` for `SPAWN_ROT_FLAGS(a, b)`.
- Every entry in all four lists has mask `0x3FF` (all half-days): presence is decided by header, flags and cutscenes, never by time.
- Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled` (skeletal, gated by flag/cutscene), `billboard-effect`, `controller-invisible`, `door-transition`. **D3f** = drawn at `prof.d3.final.normal`, default header, entrance 0, Ocarina held, `WEEKEVENTREG_25_02` clear, after the arrival cutscene.

### 4.2 Default header (17 entries, HDR room.c:40-91; decode C; objects 6)

| # | Line | Actor (ID) | Position | Raw rot X / Y / Z | Deg X/Y/Z | CS | Params | Class | D3f |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 41 | `DM_CHAR02` (0x194) Ocarina | (2, 14, -163) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0x0000 | posed-skeletal | no: killed when the Ocarina is held |
| 1 | 44 | `DM_STK` (0x191) Skull Kid | (0, 9, -110) | 0x0007/0x0009/0x007F | 0/0/0 | 9 | 0x0000 | posed-skeletal (cutscene-driven) | yes, floating ~Y 160 |
| 2 | 47 | `EFF_STK` (0x275) | (44, 15, -164) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0x000F | billboard-effect (camera-relative) | unknown (U-06) |
| 3 | 50 | `OBJ_TSUBO \| 0x2000` (0x082) | (208, 26, -212) | 0x0007/0x007F/0x007F | 0/0/Z int 0 | none | 0x808F | static-mesh (breakable pot) | yes |
| 4 | 53 | `OBJ_TSUBO \| 0x2000` | (-212, 26, -208) | same | 0/0/0 | none | 0x828F | static-mesh | yes |
| 5 | 56 | `OBJ_TSUBO \| 0x2000` | (-212, 26, 212) | same | 0/0/0 | none | 0x848F | static-mesh | yes |
| 6 | 59 | `OBJ_TSUBO \| 0x2000` | (216, 26, 212) | same | 0/0/0 | none | 0x868F | static-mesh | yes |
| 7 | 62 | `EN_GIANT` (0x1DB) | (0, -1050, -1200) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0xFE04 type 4 | npc-scheduled | no: needs `WEEKEVENTREG_25_02` |
| 8 | 65 | `EN_GIANT` | (-1000, -1050, -200) | 0x0007/0x2D7F/0x007F | 0/90/0 | none | 0xFE05 type 5 | npc-scheduled | no |
| 9 | 68 | `EN_GIANT` | (0, -1050, 800) | 0x0007/0x5A7F/0x007F | 0/180/0 | none | 0xFE06 type 6 | npc-scheduled | no |
| 10 | 71 | `EN_GIANT` | (1000, -1050, -200) | 0x0007/0x877F/0x007F | 0/270/0 | none | 0xFE07 type 7 | npc-scheduled | no |
| 11 | 74 | `EN_TIME_TAG \| 0xE000` (0x1AB) | (-2, 0, -2) | 0x0007/0x000C/0x007F | all int 0 | 12 | 0x5FFF type 2 `TIMETAG_ROOFTOP_OATH` | controller-invisible | no geometry |
| 12 | 77 | `DM_CHAR00` (0x192) Tatl | (-50, 14, -113) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0x0000 | npc-scheduled (cutscene-only) | no |
| 13 | 80 | `DM_CHAR00` Tael | (44, 11, -110) | same | 0/0/0 | none | 0x0001 | npc-scheduled (cutscene-only) | no |
| 14 | 83 | `EN_FALL` (0x17C) | (0, 3250, -200) | 0x1E07/0x9DFF/0x007F | 60/315/0 | none | 0x03FF type 7 stopped closed-mouth moon | static-mesh | no: needs `WEEKEVENTREG_25_02` |
| 15 | 86 | `EN_FALL` | (0, 4000, -200) | 0x1E07/0x9DFF/0x007F | 60/315/0 | none | 0x04FF type 9 clock-tower moon | static-mesh | yes |
| 16 | 89 | `EN_WEATHER_TAG` (0x0BC) | (-1, 5, -64) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0x0A04 type 4 skybox stars, 0x0A x 100 = 1000 | controller-invisible | no geometry |

### 4.3 Alternate 1 (4 entries, HDR room.c:132-145; objects 8)

| # | Line | Actor | Position | Raw rot | Deg | CS | Params | Class |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 133 | `DM_STK` | (0, 9, -110) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0x0000 | posed-skeletal |
| 1 | 136 | `EFF_STK` | (-326, 27, 2) | same | 0/0/0 | none | 0x000F | billboard-effect |
| 2 | 139 | `EN_WEATHER_TAG` | (-1, 5, -64) | same | 0/0/0 | none | 0x0A04 | controller-invisible |
| 3 | 142 | `EN_FALL` | (0, 4000, -200) | 0x1E07/0x9DFF/0x007F | 60/315/0 | none | 0x04FF type 9 | static-mesh |

### 4.4 Alternate 2 (12 entries, HDR room.c:169-206; objects 8)

| # | Line | Actor | Position | Raw rot | Deg | CS | Params | Class |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 170 | `DM_CHAR05` (0x197) Majora's Mask | (-148, 19, -127) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0x0003 | posed-skeletal (cutscene-only) |
| 1 | 173 | `DM_STK` | (0, 9, -110) | same | 0/0/0 | none | 0x0000 | posed-skeletal |
| 2-5 | 176-185 | `EN_GIANT` x4 | (0, -1050, -1200), (-1000, -1050, -200), (0, -1050, 800), (1000, -1050, -200) | Y 0x007F/0x2D7F/0x5A7F/0x877F | Y 0/90/180/270 | none | 0xFE04-0xFE07 types 4-7 (success) | npc-scheduled |
| 6 | 188 | `EN_FALL2` (0x29C) warp beam | (-1, 4, 44) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0x0000 | billboard-effect (cutscene) |
| 7 | 191 | `EN_WEATHER_TAG` | (-1, 5, -64) | same | 0/0/0 | none | 0x0A04 | controller-invisible |
| 8 | 194 | `DM_CHAR00` Tatl | (-50, 14, -113) | same | 0/0/0 | none | 0x0000 | npc-scheduled |
| 9 | 197 | `DM_CHAR00` Tael | (44, 11, -110) | same | 0/0/0 | none | 0x0001 | npc-scheduled |
| 10 | 200 | `EN_FALL` | (0, 3250, -200) | 0x1E07/0xA2FF/0x007F | 60/325/0 | none | 0x03FF type 7 | static-mesh |
| 11 | 203 | `EN_FALL` | (0, 3250, -200) | 0x1E07/0xA2FF/0x007F | 60/325/0 | none | 0x057F type 10 open-mouth moon | static-mesh (draw on cue 133 id 4) |

### 4.5 Alternate 3 (13 entries, HDR room.c:228-265; objects 6)

| # | Line | Actor | Position | Raw rot | Deg | CS | Params | Class |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 229 | `DM_STK` | (0, 9, -110) | 0x0007/0x007F/0x007F | 0/0/0 | none | 0x0000 | posed-skeletal |
| 1-4 | 232-241 | `OBJ_TSUBO \| 0x2000` x4 | as default a03-a06 | same | 0/0/0 | none | 0x808F, 0x828F, 0x848F, 0x868F | static-mesh |
| 5-8 | 244-253 | `EN_GIANT` x4 | (0, -720, -1200), (-1000, -720, -200), (0, -720, 800), (1000, -720, -200) | Y 0x007F/0x2D7F/0x5A7F/0x877F | Y 0/90/180/270 | none | 0xFE0C-0xFE0F types 12-15 (failure) | npc-scheduled |
| 9 | 256 | `EN_FALL` | (0, 3250, -200) | 0x1E07/0x9DFF/0x007F | 60/315/0 | none | 0x04FF type 9 | static-mesh |
| 10 | 259 | `EN_WEATHER_TAG` | (-1, 5, -64) | same | 0/0/0 | none | 0x0A04 | controller-invisible |
| 11 | 262 | `EN_FALL` | (0, 0, 0) | 0x0087/0x007F/0x007F | 1/0/0 | none | 0x027F type 4 rising debris | billboard-effect |
| 12 | 265 | `ELF_MSG6 \| 0xE000` (0x232) | (-1, 20, 222) | 0x0007/0x007F/0x007F | all int 0 | none | 0xFE04: type 4, switch 0x7F (none) | controller-invisible |

### 4.6 Overlay findings per actor type (OVL)

| Actor | Init / presence | Draw / state |
| --- | --- | --- |
| Dm_Stk | needs `OBJECT_STK` else killed; in `SCENE_OKUJOU` alpha 255, cylinder r14 h38 (`z_dm_stk.c:181`), every damage type has effect 0xF (`:186-222`). Entrance `ENTRANCE(CLOCK_TOWER_ROOFTOP, 0)`: L0 starts a 300 s moon-crash timer and intro cs 9 (no Ocarina) or cs 11 (Ocarina); L3 starts a 60 s timer, Y 120, `Idle`; L1/L2 do nothing, floating. Any other entrance: Deku-pipes state ready, Y 120, `Idle` (`:1084-1135`) | flex `gSkullKidSkel`; bank chosen per anim (`:307-323`). Idle bob `Y -> 160 + 10 sin`, faces the player, pitch 0x1B58 (`:1296-1312`) |
| Dm_Char02 | killed unless `items[SLOT_OCARINA] == ITEM_NONE`; skeleton `gClockTowerOcarinaOfTimeSkel`, scale 0.01 (`z_dm_char02.c:82-96`) | drawn when no cutscene and Y < 100, or on Dm_Stk cue 107 ids 0x17/0x1C/0x26 (`:172-194`); offers `GI_OCARINA_OF_TIME` at xz 30, y 80 (`:153`) |
| Eff_Stk | scale 0.2; profile draw `EffStk_Draw` (`z_eff_stk.c:18-37`) | cue 512 ids 2/3 grow/shrink; other cue ids set draw NULL; drawn at the camera eye with a billboard (`:41-101`) |
| Obj_Tsubo | type `(p >> 7) & 3 = 1` magic pot, scale 0.2955, cylinder r18 h45, bank `OBJECT_RACETSUBO`; drop index `p & 0x3F = 0x0F` -> `ITEM00_MAGIC_JAR_BIG`; collectible flag `(p >> 9) & 0x7F` = 0x40..0x43 (`z_obj_tsubo.c:67-75, 120-131, 178-191`; `src/code/z_en_item00.c:1453-1469`) | `gMagicPotDL` (`:703`) |
| En_Giant | scale 0.32, draw NULL at init. Types 4-7 (success): killed unless `WEEKEVENTREG_25_02`, raised-arms pose. Types 12-15 (failure): struggle loop, killed if that giant is still imprisoned (its boss remains not held) (`z_en_giant.c:116-160, 162-230`) | `gGiantSkel` |
| En_Time_Tag type 2 | `EnTimeTag_RooftopOath_Wait`, updates during ocarina (`z_en_time_tag.c:66-69`) | section 5.4 |
| Dm_Char00 | cue type `CS_CMD_ACTOR_CUE_113 + params` (Tatl 113, Tael 114) (`z_dm_char00.c:691`); scene SFX (`:605-612`) | returns before drawing unless a cutscene runs (`:1057-1061`) |
| En_Fall | scale 0.16 by default (`z_en_fall.c:139-159`). Type 7: x3, killed unless `WEEKEVENTREG_25_02`. Type 9: x3, killed if `WEEKEVENTREG_25_02`. Type 10: x3, draw NULL until cue 133 id 4. Type 4: debris effect, scale 1 (`:209-301`) | `gMoonDL` (`object_fall` 0x77F0, `:766`); moon-fall SFX during rooftop cutscenes (`:474-478`); type 7 moon SFX on alt2 scripts 0/1 (`:422-470`) |
| En_Weather_Tag type 4 | sets `gSkyboxNumStars = 1000` (`z_en_weather_tag.c:357-362`) | none |
| Dm_Char05 type 3 | `object_dmask_Skel_013D0` (`z_dm_char05.c:114-121`) | drawn only on cue 559 ids other than 1 and 4; id 4 draws `object_dmask_DL_001E70` offset -600 (`:667-686`) |
| En_Fall2 | scale 0.02, cue 561 (`z_en_fall2.c:34-42`) | cutscene warp beam |
| Elf_Msg6 type 4 | xz radius `100 x 0.4 = 40`, height 0..40 (`z_elf_msg6.c:129-140, 236-238`); text 0x255; killed on talk (`:212-215, 375-384`) | none (Tatl prompt) |

### 4.7 Counts

| | default | alt1 | alt2 | alt3 |
| --- | --- | --- | --- | --- |
| Entries / objects | 17 / 6 | 4 / 8 | 12 / 8 | 13 / 6 |
| static-mesh | 6 (pots 4, moons 2) | 1 | 2 | 5 |
| posed-skeletal | 2 | 1 | 2 | 1 |
| npc-scheduled | 6 (giants 4, fairies 2) | 0 | 6 | 4 |
| billboard-effect | 1 | 1 | 1 | 1 |
| controller-invisible | 2 | 1 | 1 | 2 |
| door-transition | 0 | 0 | 0 | 0 |
| D3f drawn (Ocarina held, no 25_02) | 6: Skull Kid, 4 pots, moon a15 (+ Eff_Stk U-06) | - | - | - |

LOCATION_REGISTER (`r00 d 6/17, a1 8/4, a2 8/12, a3 6/13`, `0 (no command)`, `2/1/3/1`, `none in any header`, `2 (alt2: 4) / 3`) agrees with the C on every count.

## 5. Venue mechanics index (INT-17 scope)

Index and cite only; the Unity implementation is not designed here.

### 5.1 Skull Kid confrontation (OVL `z_dm_stk.c`; default header, entrance 0, layer 0)

| Step | Lines | Condition | Result |
| --- | --- | --- | --- |
| Arrival | 1093-1107 | moon-crash timer off | `Interface_StartTimer(TIMER_ID_MOON_CRASH, 300)` at (115, 200) |
| Intro v1 | 1104-1106, 1238-1254 | no Ocarina | cs 9 = script 0 (1580 frames, stop at 1030; texts 0x2008-0x2010; `NA_BGM_MAJORAS_THEME` at 290, `NA_BGM_MINI_BOSS` at 900); then call-down-moon loop holding the Ocarina, `IdleWithOcarina` |
| Intro v2 | 1107-1108, 1256-1271 | Ocarina held | cs 11 = script 2 (3000 frames, stop at 565; texts with `BOSSES_REMAINS` variants 0x2011, 0x2012); then floating arms crossed, `Idle` |
| Idle with Ocarina | 1764-1789 | first cycle | faces player, lock-on; Tatl text 0x2014 every 800 frames; any hit (effect 0xF) -> drop cutscene |
| Drop Ocarina | 1273-1294 | hit | cs 10 = script 1 (1118 frames, stop at 140; end SFX `CS_END_SFX_TRE_BOX_APPEAR`); Dm_Char02 bounce SFX frames 95-112 (`z_dm_char02.c:61-80`) |
| Idle | 1791-1807, 1315-1353 | after either intro | hits only deflect; every third hit text 0x2013 unless the player plays |
| Deku pipes | 1123-1132, 1838-1863 | arrival by any entrance other than e0 (e.g. `0x2C10`) | ocarina interaction -> cs 16 = script 7 (300 frames, text 0x2015) |

### 5.2 Ocarina get-item (OVL `z_dm_char02.c`, `player.c`)

`DmChar02_Update` offers `GI_OCARINA_OF_TIME` (range 30 xz, 80 y) every frame until taken, then sets `playerForm = PLAYER_FORM_HUMAN` and kills itself (`z_dm_char02.c:143-160`). When the get-item text closes, `player.c:13675-13684` sends `ENTRANCE(CUTSCENE, 0)` with `nextCutsceneIndex 0xFFF2` (fade white). `SPOT00` layer 3 script 0 (ALWAYS) returns to `0x2C10`, rooftop spawn 1, where script 6 (csId 15, ALWAYS on spawn 1; texts 0x2021, 0x2022; white fills 0-1 and 20-40; mini-boss BGM at 190; stop at 200) plays (scene.c:3343-3475, :3566) (C-BIN for `SPOT00`).

### 5.3 Song of Time and the moon crash

| Item | Implementation | Citation |
| --- | --- | --- |
| Song of Time | allowed (restriction 0); `OCARINA_MODE_APPLY_SOT` -> `SPOT00` e1 (first reset) or e0, `0xFFF7` | section 1; `player.c:17472-17486` |
| Moon-crash timer | 300 s on default L0 arrival, 60 s on alt3 arrival; stopped by the Oath (time tag) | `z_dm_stk.c:1095-1117`; `z_en_time_tag.c:103, 121` |
| Timer runs out | day 4; alt3 -> TF e1 L1; else `Interface_StartMoonCrash` -> TF e12, 06:00 + 10 | `z_parameter.c:5868-5876, 5612-5624` |
| Moon visual | type 9 at (0, 4000, -200), rot X 60 Y 315, scale 0.48; moon-fall SFX while any rooftop cutscene runs | `z_en_fall.c:229-236, 474-478` |
| Unused path | (0, 4000, -200) -> (0, 3000, -200) matches the moon X/Z | scene.c:122-129 (reader U-09) |

### 5.4 Oath to Order rooftop time tag (`0x5FFF`, `TIMETAG_ROOFTOP_OATH`)

| Step | Lines (`z_en_time_tag.c`) | Result |
| --- | --- | --- |
| Init | 66-69 | `RooftopOath_Wait`; `ACTOR_FLAG_UPDATE_DURING_OCARINA` |
| Wait | 114-125 | when `ocarinaMode == OCARINA_MODE_EVENT` and `lastPlayedSong == OCARINA_SONG_OATH` and csId != none: queue csId 12, stop the moon-crash timer, `ocarinaMode = OCARINA_MODE_END` |
| Cutscene | 98-112 | `CutsceneManager_StartWithPlayerCs(12)`; timer off; sets `WEEKEVENTREG_25_02` only if all four boss remains are held |
| csId 12 | scene.c:3583 (row 12: priority 5, script 3, additional none) | script 3: 771 frames; `CS_MISC_EARTHQUAKE_MEDIUM` 61-271, stop 271-351; Skull Kid cues 30-35; destination `0x5401` at 675 (scene.c:1200-1457) |

### 5.5 Giants (`z_en_giant.c`; `z_en_fall.c:317-356`)

Termina Field alt1 script 0 (C-BIN) decides: all four remains -> csId 11 (first, sets `WEEKEVENTREG_93_04`) or 12 (repeat) -> rooftop alt2 e0/e1 (moon warp, success giants types 4-7 need `WEEKEVENTREG_25_02`, set at the Oath); otherwise -> rooftop alt3 (failure giants 12-15, each killed while imprisoned; 60 s timer; Elf_Msg6 hint at the stair head). Default-header giants and moon a14 appear only if the default header loads with `WEEKEVENTREG_25_02` already set in the cycle, and then moon a15 is killed (INF from `z_en_fall.c:221-237`; when that happens is U-03).

### 5.6 Cutscene index (default scene.c:3559-3588; alt1 :4079-4094; alt2 :6736-6755; alt3 :6863-6873)

`CutsceneEntry { priority, length, csCamId, scriptIndex, additionalCsId, endSfx, customValue, hudVisibility, endCam, letterboxSize }` (`include/z64cutscene.h:626-637`). Rows 0-8 in every header are global-camera rows (csCam -3..-11, no script, chained 0->...->8), identical to South's.

| Header | csId | Script (data, line) | Frames / stop or destination | Started by | Key contents (HDR, cs.py) |
| --- | --- | --- | --- | --- | --- |
| default | 9 (-> 10) | 0 `000380` (134) | 1580 / stop 1030 | Dm_Stk intro v1 | cues 107 Skull Kid, 113/114 fairies, 131 Ocarina, 512/513 Eff_Stk, 201 Elf; texts 0x2008-0x2010 |
| default | 10 (-> 11) | 1 `001BF0` (667) | 1118 / stop 140 | Dm_Stk hit | cues 107, 114, 131 (fall) |
| default | 11 (-> 13) | 2 `002280` (806) | 3000 / stop 565 | Dm_Stk intro v2 | as script 0, remains-branch texts |
| default | 12 | 3 `0034B0` (1200) | 771 / dest 675 `0x5401` | time tag (Oath) | earthquake 61-351 |
| default | 13 (-> 14), 14 (-> 16) | none | - | no starter found (U-02) | - |
| default | 15 | 6 `0095B0` (3343) | 1000 / stop 200 | spawn 1 ALWAYS | texts 0x2021, 0x2022 |
| default | 16 | 7 `009C20` (3481) | 300 / stop 80 | Dm_Stk Deku pipes | text 0x2015; `CS_MISC_DISABLE_PLAYER_CSACTION_START_POS` |
| default | - | 4 `003FE0` (1463), 5 `007420` (2627) | 3000 / dest `0x6E00` | none | unreferenced (2.2) |
| alt1 | 9 | 0 `00AF70` (3970) | 3000 / dest 67 `0x1C29` | spawn 0 ALWAYS | text 0x1FBF; black fills 0-65; light setting 0x13 |
| alt2 | 9 (-> 10) | 0 `00B904` (4222) | 3000 / dest 1925 `0xC800` | spawn 0 ALWAYS | moon cue 133, mask 559, beam 561, giants 453; player cue 68 frames 640-700; `NA_BGM_INTO_THE_MOON` 760 |
| alt2 | 10 | 1 `00ED44` (5386) | 3000 / dest 960 | spawn 1 ALWAYS | shorter variant |
| alt2 | 11 | 2 `010F04` (6106) | 3000 / dest 380 | spawn 2 ALWAYS | texts 0x2036, `ALL_NORMAL_MASKS` 0x204E |
| alt3 | - | no script list | - | - | 9 global rows only |

Player cue 68 on the rooftop lets the player act during the alt2 cutscene (`player.c:20972-20978`, OVL; purpose U-07).

### 5.7 The final-hours fixed clock, light and music

| Item | Implementation | Citation |
| --- | --- | --- |
| Clock | room `TIME_SETTINGS(255, 255, 0)`: scene time speed 0 in every header, so the three-day clock stops on the roof; the first-cycle speed-5 rule leaves 0 at 0 | room.c:18, 114, 151, 212; `z_scene.c:386-405` |
| Countdown | the moon-crash timer replaces the clock (5.3) | `z_dm_stk.c:1095-1117` |
| Final Hours BGM | requested when day 3 and t < 06:00 under the usual guards; the negative scene id mutes BGM on entry in the final hours; default header seq 0 | `src/code/z_kankyo.c:2544-2553`; `z_play.c:583-595` |
| Light | default/alt2/alt3 light by time (`lightMode false`); alt1 by setting (config 9, `LIGHT_MODE_SETTINGS`); all surfaces carry light setting 31 (none); 3 colored point lights | 1.1; section 7 |
| Sky | skybox 1 drawn (`SKYBOX_DISABLES(false, false)`); 1000 stars via the weather tag | room.c:17; 4.6 |
| Cameras | `BgCamInfo` 0 = `0x0001` `CAM_SET_NORMAL0` (all floors), 1 = `0x0000` `CAM_SET_NONE` (all walls) | scene.c:3610-3613; `include/z64camera.h:147-148` |
| Flags read / written | read: `save.entrance`, `sceneLayer`, `items[SLOT_OCARINA]`, `timerStates[TIMER_ID_MOON_CRASH]`, quest remains, `WEEKEVENTREG_25_02`, `93_04`, `CLOCK_TOWER_OPENED` (Elf_Msg6 type 2 only); written: `WEEKEVENTREG_25_02` (time tag), `12_04` (Dm_Stk fade, `z_dm_stk.c:1680`), `93_04` (En_Fall, TF), `playerForm` (Dm_Char02), `day = 4` | lines above |
| Rupees, shops, highscores | none (C: no `Rupees_`, `HS_` or shop actor in these overlays; pots drop a big magic jar) | grep |

## 6. Static assembly list

**Day 1 noon (`prof.d1.noon.normal`): closed.** The only way in is South exit+1 = 9 (2.3); at Day 1 12:00 `Obj_Tokei_Step` holds the closed panel dynapoly in front of the stair and the staircase is not drawn (`z_obj_tokei_step.c:201-206`; `z_obj_tokeidai.c:615-621`). No warp, door or cutscene enters the roof on Day 1 (1.2). **First open profile: `prof.d3.final.normal`** (Day 3, 01:00, layer 0), where SOURCE_REGISTER section 3 and 4.6 already place `cam.clock_tower_rooftop.entry`, `.reverse_door` and `.fixture`.

At `prof.d3.final.normal`: default scene and room headers, arrival `0x2C00` spawn 0, Ocarina held, `WEEKEVENTREG_25_02` clear, after cs 11 ends (clock frozen at the arrival time, 5.7). World placement: `world = actorPos + actorRot x (actorScale x localVertex)`; rotation conversion under the Z mirror belongs to the importer.

| # | Component / params | N64 position (Unity) | Rot | Scale | DL / bank / offset | Collision | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| room_00 | cullable, 5 entries | origin | - | 1 | DLs 002E18, 001548, 0019E0, 002AE0, 000C70 (wrapper 002F50) | scene collision (section 7) | all opaque |
| a03 | magic pot `0x808F` | (208, 26, -212) (2.08, 0.26, 2.12) | 0 | 0.2955 | `object_racetsubo` `gMagicPotDL` 0x278 | cylinder r18 h45 | breakable |
| a04 | magic pot `0x828F` | (-212, 26, -208) (-2.12, 0.26, 2.08) | 0 | 0.2955 | same | same | |
| a05 | magic pot `0x848F` | (-212, 26, 212) (-2.12, 0.26, -2.12) | 0 | 0.2955 | same | same | |
| a06 | magic pot `0x868F` | (216, 26, 212) (2.16, 0.26, -2.12) | 0 | 0.2955 | same | same | |
| a15 | clock-tower moon `0x04FF` | (0, 4000, -200) (0.00, 40.00, 2.00) | X 60, Y 315 | 0.48 | `object_fall` `gMoonDL` 0x77F0 | none | skyline element |

**Present but needs skeleton/pose:** a01 Skull Kid at (0, ~160, -110) (0.00, ~1.60, 1.10), facing the player, pitch 0x1B58, scale 0.01, `object_stk` `gSkullKidSkel` 0x13328 + `object_stk2` `gSkullKidFloatingArmsCrossedAnim` 0x29A04; cylinder r14 h38. **No geometry:** a11 time tag, a16 star tag (sets 1000 stars). **Excluded by state:** a00 Ocarina (killed: Ocarina held; first-cycle profile would show it in Skull Kid's hand), a07-a10 giants and a14 moon (no `25_02`), a12/a13 Tatl/Tael (cutscene-only). **Unknown:** a02 Eff_Stk (camera-relative effect; draw outside cue 512 is U-06). **Excluded by half-day mask:** none.

## 7. Collision summary (C from scene.c:3610-3860; scratch `parse.py`, output `parse_out.txt`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 81 (all referenced) / 146 / 3 |
| Header bounds | min (-483, -45, -480), max (477, 139, 480) (scene.c:3852-3860) |
| Vertex extents | x -483..477, Y 0..139, z -480..480; Unity x -4.83..4.77, y 0.00..1.39, z -4.80..4.80 |
| Polygon kinds | 110 floors (normal Y > 0.5), 36 walls, 0 ceilings |
| Water boxes | 0 (`0, NULL`, scene.c:3859) |
| Exit surfaces | **none** (exit+1 = 0 in all 3 types) |
| Materials | all `SURFACE_MATERIAL_WOOD` (10) (`include/z64bgcheck.h:242`) |
| Other flags | no conveyor, hookshot, floor type, floor property, soft or horse flag; no vIA/vIB poly flags; light setting 31 and echo 10 on all types |
| Camera data | 2 `BgCamInfo` (5.7) |

| Surface type | data[0] / data[1] | Polys | Use |
| --- | --- | --- | --- |
| 0 | 0x00000000 / 0x000057CA | 110 F | all floors, bgCam 0 |
| 1 | 0x00000001 / 0x000057CA | 12 W | stair-head side walls Y 11..27, bgCam 1 |
| 2 | 0x00200001 / 0x000057CA | 24 W | wall type 1, ring risers Y 30..88, bgCam 1 |

Floor bands (C; x/z are band unions):

| Band | Polys | Y | Radius / extent | Slope |
| --- | --- | --- | --- | --- |
| roof center (dish) | 24 (110-133) | 0..30 | x, z -360..360 | 3-7 deg, rising outward |
| inner ring transition | 14 (108, 109, 134-145) | 18..30 | x -312..312, z -360..360 | 4-6 deg |
| stair head landing and treads | 24 (0, 1, 3, 4, 9, 10, 12, 13, 15-18, 20, 21, 26, 27, 100-107) | 11..27 | x -107..107, z 115..330 | 0-10 deg; spawn 0 on 100-107 |
| flat ring walkway | 24 (40-87 floors) | 30 | octagonal band about r 312..381 | 0 |
| outer rim slope | 24 (28-39, 88-99) | 88..139 | to r 483 | 27.4 deg, rising outward to the edge |

Between the Y 30 walkway and the rim slope stand the 24 wall-type-1 risers (Y 30..88). Beyond the rim top (Y 139, r about 480) there is no collision: falling off is a void case (behavior NI, U-08). Actor collision: Skull Kid cylinder r14 h38, pot cylinders r18 h45; no dynapoly actor in any header (C).

Proposed camera poses for the registered IDs (proposals only, REF-03 owns them): `cam.clock_tower_rooftop.entry` from spawn 0 (0, 20, 222) looking -Z and up across the dish to Skull Kid (0, ~160, -110) and the moon (0, 4000, -200); `cam.clock_tower_rooftop.reverse_door` from the center (0, 30, -150) looking +Z at the stair head (z 115..330), the roof's only arrival (it has no door); `cam.clock_tower_rooftop.fixture` low from the ring (0, 40, 300) up at Skull Kid with the moon behind. All three need `prof.d3.final.normal`; the Ocarina-free (first cycle) and alt layers are separate states.

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 18 (17 textures + 1 TLUT), all in the scene file (scene.c:6887 onward), used by room DLs |
| By format | rgba16 9 (Tex_0138A0, 0140A0, 0148A0, 0150A0, 0158A0, 0160A0, 0168A0, 0170A0, 0178A0), ci8 7 (0188A0, 018CA0, 0194A0, 019CA0, 01A4A0, 01A5A0, 01A9A0), i4 1 (0130A0); TLUT rgba16 1 (TLUT_01ADA0) |
| room_00 DLs / Vtx | 17 `Gfx` (incl. the XML wrapper 002F50), 12 `Vtx` arrays, 435 vertex lines in `.vtx.inc` |
| Triangles | 109 `gsSP2Triangles` = 218, no `gsSP1Triangle` (DL reuse not resolved) |
| Animated material | none (segment-0 terminator, section 1) |
| Actor banks (XML counts) | `object_stk` 31 tex, 40 DL, 15 anim, 1 skel; `object_stk2` 8 / 4 / 55 / 1; `object_fall` 14 / 10 / 4 / 0; `object_racetsubo` 3 / 4 / 0 / 0; `object_giant` 12 / 16 / 16 / 1; `object_delf` 0 / 0 / 78 / 0; `object_dmask` 8 / 2 / 6 / 4 skel; `object_fall2` 10 / 11 / 3 / 0 |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_OKUJOU` (C above). Binaries for every bank below exist under `extracted/n64-us/baserom/` (NI: not re-listed in this packet).

| Priority | Banks / files | Why |
| --- | --- | --- |
| d3 final static | `object_racetsubo`, `object_fall` | pots and moon |
| Skeletal pass | `object_stk`, `object_stk2` | Skull Kid skeleton and floating anim; Ocarina skeleton |
| Layer / cutscene only | `object_giant`, `object_delf`, `object_dmask`, `object_fall2` | giants, Tatl/Tael anims, Majora's Mask, moon mouth and beam |
| Source questions only | scenes `SPOT00`, `Z2_00KEIKOKU` | confirm the C-BIN senders (U-01) |

```powershell
# run from local-reference/mm-decomp, same ZAPD binary and flags as South (tools/reference/README.md)
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_racetsubo','object_fall','object_stk','object_stk2','object_giant','object_delf','object_dmask','object_fall2')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
foreach ($scene in @('SPOT00','Z2_00KEIKOKU')) {
    & $zapd e -i "assets/xml/scenes/$scene/$scene.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/scenes/$scene" -osf "extracted/n64-us/assets/scenes/$scene" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $scene" }
}
```

## 10. Proposed region record

Schema 1 (PIPE-01, `shared/city/schema/region.schema.json`), written directly; the integrator lifts and validates it. `portalIds` is `[]` because `ids.json` has no portal departing this venue; the schema allows an empty array (no `minItems`). `cameraIds` is `[]` as in the accepted interior record; the three registered `cam.clock_tower_rooftop.*` IDs are listed in section 7.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.clock_tower_rooftop",
  "displayLabel": "Clock Tower Rooftop",
  "source": {
    "sceneSymbol": "Z2_OKUJOU",
    "sceneEnum": "SCENE_OKUJOU",
    "sceneTableIndex": "0x19",
    "entranceTableSceneIndex": "0x16",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d3.final.normal",
    "sceneLayer": 0,
    "halfDayBit": "0x004",
    "stateAssumptions": [
      "venue closed at prof.d1.noon.normal: Obj_Tokei_Step closed panel and hidden staircase until Day 3 before 06:00 (z_obj_tokei_step.c:191-208, z_obj_tokeidai.c:615-621)",
      "arrival by South exit 0x2C00, entrance 0 layer 0, spawn 0, default headers",
      "Ocarina held (SOURCE_REGISTER baseline owner decision): Dm_Char02 killed, Skull Kid intro cs 11 then floating idle",
      "WEEKEVENTREG_25_02 clear: success giants and stopped moon a14 killed, clock-tower moon a15 drawn",
      "scene time speed 0: clock frozen at the arrival time; 300 s moon-crash timer running"
    ]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_OKUJOU", "bytes": 110496, "sha256": "332445961fc8b267e72510a83a993d3646aaa31ccf2d1e17bfc72c4da4d01ef7" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_OKUJOU_room_00", "bytes": 12160, "sha256": "38ce3e75223f68fbca0db2baf1df53e0711ada3d5956f0f6a367c2b4e31a9c07" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_OKUJOU/Z2_OKUJOU.c", "bytes": 544886, "sha256": "83034b16ecd9606ac03630b2f759e456be5f6207b28a3308113414939b03ec63" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_OKUJOU/Z2_OKUJOU_room_00.c", "bytes": 39137, "sha256": "1e7cafa724213b4a7265c17370d9eded25acc8459cad36ae4243d5c8bfba7bc6" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_OKUJOU/Z2_OKUJOU.xml", "bytes": 324, "sha256": "131e23060ba2e4870ec7b96df948777bc3e253fb1b5bfe4930b4ac266a16da48" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": ["loc.south_clock_town"],
      "status": "not_shared",
      "evidence": "rooftop spawn 0 at (0, 20, 222) on a roof centered at the origin vs the South tower-top exit treads x -340..-220, Y 300..375, z -1053..-893; the scenes are joined only by exit 0x2C00 (C)"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-483, -45, -480], "max": [477, 139, 480] },
    "sourceCollisionVertices": { "min": [-483, 0, -480], "max": [477, 139, 480] },
    "unityCollisionVertices": { "min": [-4.83, 0.0, -4.8], "max": [4.77, 1.39, 4.8] }
  },
  "anchors": [
    { "id": "anchor.ctr.spawn0_stair_head", "source": [0, 20, 222], "unity": [0.0, 0.2, -2.22], "status": "proposed" },
    { "id": "anchor.ctr.spawn1_center", "source": [0, 0, 0], "unity": [0.0, 0.0, 0.0], "status": "proposed" },
    { "id": "anchor.ctr.pot_a03", "source": [208, 26, -212], "unity": [2.08, 0.26, 2.12], "status": "proposed" },
    { "id": "anchor.ctr.skull_kid_a01_home", "source": [0, 9, -110], "unity": [0.0, 0.09, 1.1], "status": "proposed", "note": "idle hover Y about 160" },
    { "id": "anchor.ctr.moon_a15", "source": [0, 4000, -200], "unity": [0.0, 40.0, 2.0], "status": "proposed", "note": "skyline, not a ground point" }
  ],
  "rooms": [
    { "id": "loc.clock_tower_rooftop.room_00", "sourceRoom": "Z2_OKUJOU_room_00", "header": 0, "roomShape": "cullable, 5 entries", "displayLists": 17, "role": "tower roof; one room for all four headers", "collisionLightIndex": 31 }
  ],
  "components": [
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a03", "actorId": "0x082", "params": "0x808F", "class": "static-mesh", "variant": "state-pickup", "dls": ["object_racetsubo:0x278 gMagicPotDL"], "status": "selected", "note": "magic pot, breakable; drops a big magic jar" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a04", "actorId": "0x082", "params": "0x828F", "class": "static-mesh", "variant": "state-pickup", "dls": ["object_racetsubo:0x278 gMagicPotDL"], "status": "selected", "note": "magic pot" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a05", "actorId": "0x082", "params": "0x848F", "class": "static-mesh", "variant": "state-pickup", "dls": ["object_racetsubo:0x278 gMagicPotDL"], "status": "selected", "note": "magic pot" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a06", "actorId": "0x082", "params": "0x868F", "class": "static-mesh", "variant": "state-pickup", "dls": ["object_racetsubo:0x278 gMagicPotDL"], "status": "selected", "note": "magic pot" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a15", "actorId": "0x17C", "params": "0x04FF", "class": "static-mesh", "dls": ["object_fall:0x77F0 gMoonDL"], "status": "selected", "note": "clock-tower moon, scale 0.48, rot X 60 Y 315; killed if WEEKEVENTREG_25_02" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a01", "actorId": "0x191", "params": "0x0000", "class": "posed-skeletal", "variant": "posed-skeletal", "dls": ["object_stk:0x13328 gSkullKidSkel", "object_stk2:0x29A04 floating anim"], "status": "optional", "note": "hovers near Y 160 facing the player after the arrival cutscene" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a00", "actorId": "0x194", "params": "0x0000", "class": "posed-skeletal", "dls": ["object_stk2:0xAD68 gClockTowerOcarinaOfTimeSkel"], "status": "optional", "note": "first cycle only (no Ocarina held); get-item GI_OCARINA_OF_TIME" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a02", "actorId": "0x275", "params": "0x000F", "class": "billboard-effect", "variant": "camera-relative", "status": "optional", "note": "draw outside cue 512 unknown (dossier U-06)" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a14", "actorId": "0x17C", "params": "0x03FF", "class": "static-mesh", "dls": ["object_fall:0x77F0 gMoonDL"], "status": "optional", "note": "stopped moon at Y 3250; only with WEEKEVENTREG_25_02" },
    { "sourceKey": "Z2_OKUJOU/h0/room_00/a07", "actorId": "0x1DB", "params": "0xFE04", "class": "npc-scheduled", "dls": ["object_giant:0x79B0 gGiantSkel"], "status": "optional", "note": "success giant; only with WEEKEVENTREG_25_02 (a08-a10 same rule)" }
  ],
  "collisionOwnership": {
    "room": "Z2_OKUJOUCollisionHeader_00AC00 (146 polys, 3 surface types, no exits; shared by all four headers)",
    "waterBoxes": 0,
    "actorDynapoly": [],
    "actorCylinders": ["a01 Dm_Stk r14 h38", "a03-a06 Obj_Tsubo type 1 r18 h45"],
    "note": "beyond the rim top (Y 139, r about 480) there is no collision; void behavior unknown"
  },
  "portalIds": [],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a01 Skull Kid skeletal pose and hover", "Tatl companion (En_Elf) and Link", "moon-crash timer HUD", "skybox and 1000 stars", "three colored point lights"],
  "excluded": {
    "halfDayMask": [],
    "stateOrSchedule": ["a00 Dm_Char02 (killed: Ocarina held)", "a07-a10 En_Giant success types (need WEEKEVENTREG_25_02)", "a14 En_Fall type 7 (needs WEEKEVENTREG_25_02)", "a12, a13 Dm_Char00 (cutscene-only)", "alt1-alt3 actor lists (cutscene layers 1-3)"],
    "noGeometry": ["a11 En_Time_Tag Oath to Order controller", "a16 En_Weather_Tag star count"]
  },
  "unsupported": ["venue gate (South Obj_Tokei_Step panel and Obj_Tokeidai staircase)", "skeletal NPC pose", "cutscene scripts 0-7 and alternate layers 1-3", "Oath to Order time tag", "moon-crash timer and its exits", "Ocarina get-item", "code-driven exits (no exit list)", "scene time speed 0 (clock frozen)", "breakable pots and drops", "void beyond the rim"],
  "evidence": ["docs/city/interiors/clock_tower_rooftop.md"],
  "readiness": {
    "sourceInspection": "pass",
    "objectExtraction": "not_run",
    "import": "not_run",
    "controllerTraversal": "not_run",
    "editorRender": "not_run",
    "packagedRender": "not_run",
    "visualApproval": "not_run"
  },
  "readinessNotes": { "sourceInspection": "selected profile is prof.d3.final.normal because the venue is closed at Day 1 noon; SPOT00 and Z2_00KEIKOKU senders come from a scratch baserom header scan (dossier 1.2, C-BIN)" }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | `SPOT00` and `Z2_00KEIKOKU` senders are known only from a binary header scan | Extract both scenes (section 9); read `SPOT00` alt3/alt9 scripts and `Z2_00KEIKOKU` alt1 scripts 0, 2, 3, 4 and its actor-cutscene list |
| U-02 | Who starts default csIds 13/14 (no script) and `Z2_00KEIKOKU` alt1 csId 13 (script 4 -> `0x2C22`, the only e2 sender) | Grep the TF alt1 actors' overlays for `CutsceneManager_GetAdditionalCsId` chains from csId 12; check whether `CutsceneManager` auto-plays an additional csId (`src/code/z_eventmgr.c`) |
| U-03 | When the default header loads with `WEEKEVENTREG_25_02` already set (showing success giants a07-a10 and moon a14, hiding a15) | Find writers/clearers of `WEEKEVENTREG_25_02` beyond `z_en_time_tag.c:107` and whether any entrance returns to rooftop L0 after the Oath in the same cycle |
| U-04 | Interior `0x1C19` and the alt1 chain purpose (alt1 loads `OBJECT_DMASK`, `OBJECT_FALL2` with no consumer) | After U-01, read `SPOT00` alt9 actors; text 0x1FBF in the message table |
| U-05 | Where the new cycle lands after X4 (`SPOT00` L8) | Extract `SPOT00`; read its layer-8 script destinations |
| U-06 | Eff_Stk a02 draws via its profile before any cue 512 (`unk144` alpha step 0); visible or not | Read `AnimatedMat_DrawAlphaStep` with step 0 on `object_stk2_Matanimheader_009F60` (`src/code/z_scene_proc.c`) |
| U-07 | Player cue 68 on the roof (`player.c:20972-20978`) and `Player_ActionHandler_13` | Read `Player_ActionHandler_13` and `sPlayerCueToCsActionMap` |
| U-08 | Falling off the rim (no collision beyond r about 480, header min Y -45) | Read the player void/fall respawn logic (`z_player.c` `Player_HandleExitsAndVoids`, `func_80833xxx` void checks) for scenes without a void plane |
| U-09 | Path 0 (moon line) has no reader found | Grep overlays for `play->setupPathList` use in `SCENE_OKUJOU` (En_Fall, Dm_Stk) |
| U-10 | Restriction value 3 (masks, songs) | LOCATION_REGISTER U-13 (`z_parameter.c` users of the restriction table) |
| U-11 | Room DL content (rim, clock-face edge, stair head) | Render room_00 DLs (after REC-03 provenance) or capture `cam.clock_tower_rooftop.*` |
| U-12 | Mode 0xD/0xF start behavior (spawns 0-2) | Read the start-mode table in `Player_Init` |

Register notes (not edited): LOCATION_REGISTER counts all agree (4.7). LOCATION_REGISTER U-04 (rooftop e2): resolved, e2 valid only on layer 2 (2.1). U-05 (rooftop): resolved for all three alternates (1.2), except the csId 13 starter (U-02). U-07 (rooftop): resolved, no exit surfaces; code exits X1-X7 (2.2). LOCATION_REGISTER note 2 (`z_play.c:592`): negative scene id mutes BGM on final-hours transitions (section 1). PORTAL_REGISTER U1: resolved to the `Obj_Tokei_Step` panel dynapoly (2.3). PORTAL_REGISTER U2: answered by 2.2; the dispatch's two citations are an inbound sender (`z_en_fall.c:346`) and an arrival test (`z_dm_stk.c:1093`). PORTAL_REGISTER row 175 "MISSING: no return exit in dest default header" stands; proposed `special` rows are in 2.4.

**Not inspected:** room DL contents and combiners; light-setting values; cutscene camera splines and most cue semantics; En_Elf; `SPOT00`, `Z2_00KEIKOKU` beyond the header scan; South exit polygons (taken from PORTAL_REGISTER and the South dossier); 3DS material; captures.

**Checks run:** read the brief, LOCATION_REGISTER, PORTAL_REGISTER, SOURCE_REGISTER sections 2.6, 3, 4.6, the South dossier rows for the rooftop, the accepted `clock_tower_interior.md`, `milk_bar.md` structure and `shared/city/regions/clock_tower_interior.json` format, `shared/city/schema/README.md`, `region.schema.json`, `ids.json`; read scene/room C and XML, `scene_table.h`, `z_scene_table.c`, `z_play.c`, `z_demo.c`, `z_actor.c`, `z_scene.c`, `z_scene_proc.c`, `z_parameter.c`, `z_kankyo.c`, `z_message.c`, `z_en_item00.c`, `player.c` (13665-13690, 17462-17500, 20965-20980), `z64scene.h`, `z64bgcheck.h`, `z64camera.h`, `z64environment.h`, `z64light.h` and the overlays Dm_Stk, Dm_Char00, Dm_Char02, Dm_Char05, Eff_Stk, Obj_Tsubo, En_Giant, En_Time_Tag, En_Fall, En_Fall2, En_Weather_Tag, Elf_Msg6, Obj_Tokeidai, Obj_Tokei_Step; scratch scripts in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\clock_tower_rooftop\`: `parse.py` (actors, spawns, collision, hashes; rerun output identical to `parse_out.txt`), `cs.py` (cutscene summary, `cs_out.txt`), `scan.py` (103 baserom scene files, cutscene-script destinations into 0x16; rerun identical), `tfcs.py` (actor-cutscene lists of `Z2_00KEIKOKU` alt1 and `Z2_OKUJOU` default; the latter matches scene.c:3559-3588); SHA-256 of the ZAPD binary; grep of `src/` and all 22 extracted scenes for rooftop entrance values.

**NOT run:** ZAPD (no object or scene extracted), Blender, Unity, any player or build, captures, renders, controller routes, `region_from_dossier.py`, `validate_city.py`.

**Next dependency:** integrator lifts section 10 and runs `validate_city.py`; the PORTAL_REGISTER owner decides whether to register the 2.4 `special` rows (and `ext.spot00` / `ext.z2_sougen`); REF-03 places the three rooftop cameras on `prof.d3.final.normal`; then extract `object_racetsubo` and `object_fall` for the static assembly.
