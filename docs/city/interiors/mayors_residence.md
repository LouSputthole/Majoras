# The Mayor's Residence (`loc.mayors_residence`): source inventory dossier

Task **REF-09 / INT-08** (FULL_CITY_PLAN.md section 6 INT-08 row, section 12 REF-09 row). Base commit `31b20dc` on `main`; the frozen schemas were read at working-tree HEAD `7a7a08c` (PIPE-01). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_SONCHONOIE` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save, Human form, no mask worn; `docs/city/SOURCE_REGISTER.md`).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_SONCHONOIE/Z2_SONCHONOIE.c`; `r00.c` .. `r03.c` = `.../Z2_SONCHONOIE_room_00.c` .. `_room_03.c`; `ovl:<Name>` = `src/overlays/actors/ovl_<Name>/z_<name>.c` (`<Name>.schl` = that folder's `scheduleScripts.schl`); `player.c` = `src/overlays/actors/ovl_player_actor/z_player.c`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. Line numbers in the extracted C count `\n` only (the ZAPD output carries stray `\r` after some actor names; a reader that treats `\r` as a newline shows larger numbers). Scratch tools and outputs: `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\mayors_residence\` (`actors.py`, `col.py`, `col2.py`, `probe.py`, `surfq.py`, `wall2.py`, `mats.py`, `vtxbox.py`, `*_out.txt`).

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or engine source; **OVL** read from the actor overlay source (including `.schl` schedule scripts and message scripts); **ANN** decomp-author comment or symbol name; **INF** inferred (reason given); **NI** not inspected.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_SONCHONOIE` / `SCENE_SONCHONOIE`, scene-table index `0x12` | HDR `include/tables/scene_table.h:43` |
| Entrance-table scene index | `0x00` (`ENTR_SCENE_MAYORS_RESIDENCE`), table `sMayorsResidenceEntranceTable` | C `src/code/z_scene_table.c:2573`, `:37-41`; `include/z64scene.h:593` |
| Title card / draw / restrictions / cycle flags | `0x10B` / `SCENE_DRAW_CFG_DEFAULT` / `RESTRICTIONS_INDOORS` / `PERSISTENT_CYCLE_FLAGS_NONE` | HDR scene_table.h:43 |
| XML | scene segment 2; four room files segment 3, 4 DLs declared per room; dependency `misc/scene_texture_01.xml` | XML `assets/xml/scenes/Z2_SONCHONOIE/Z2_SONCHONOIE.xml` |
| Room list | 4: `Z2_SONCHONOIE_room_00` .. `_room_03` (`SCENE_CMD_ROOM_LIST(4, ...)`) | HDR scene.c:19, :50-55 |
| Scene and room headers | **default only**: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in the scene or any room, so every `sceneLayer` loads the default header | HDR scene.c:17-34, r0N.c:17-26; C `src/code/z_scene.c:464-474` |
| Entrance-table layers | entrances 0, 1, 2 each have one row, flags `0x4102` | C z_scene_table.c:25-35 |
| Sound | spec 5, ambience 19, seq 31 = `NA_BGM_INSIDE_A_HOUSE` | HDR scene.c:18; `include/tables/sequence_table.h:82` (ordinal counted) |
| Skybox | `SKYBOX_SETTINGS(0x01, 1, 0, false)` (file 1, skybox 1, weather 0, time-driven light); rooms `SKYBOX_DISABLES(false, true)` (sun/moon off) | HDR scene.c:26, r0N.c:20; `include/z64scene.h:833-837` |
| Env light settings | 20 entries = 4 distinct rows repeated 5 times (values not transcribed) | HDR scene.c:28, :67-108 |
| Actor cutscenes / cameras | 29 `CutsceneEntry` (:185-215); 26 `ActorCsCamInfo` (:309-334) over 78 points (:228-307) | HDR |
| Paths | 9 paths, 36 points, all Y 0, chained `additionalPathIndex n -> n+1` (path 8 ends with -1) | HDR scene.c:110-183 |
| Minimap | 4 entries, all mapId `0xFFFF`; per-room centre: r0 (0,0,0), r1 (-165,0,-323), r2 (165,0,-330), r3 (525,0,-225) | HDR scene.c:217-226 |
| Animated materials | `{ 0, 6, NULL }` only; segment 0 is skipped | HDR scene.c:342-344; `src/code/z_scene_proc.c:400` |
| Room commands (identical in all four rooms) | echo 4; `ROOM_BEHAVIOR(0, 0, 0, 0, false, 0)` (pos lights off, storm 0); `TIME_SETTINGS(255, 255, 3)`: the clock runs inside (speed 3, or 5 without Ocarina) | HDR r0N.c:18-21; `include/z64scene.h:804-807`; SOURCE_REGISTER 2.2 |
| Room shapes | cullable: r00 6 entries, r01 8, r02 9, r03 7 | HDR r00.c:77-91, r01.c:76-92, r02.c:67-84, r03.c:53-68 |

**Room roles (C + ANN).** room_00 = lobby and reception: arrival room of entrance 0, `En_Recepgirl` ("Mayor's receptionist", ANN). room_01 = the Mayor's office, where the meeting is held: `En_Dt` ("Mayor Dotour"), `En_Muto`, `En_Baisen`; door schedule `MAYORS_RESIDENCE_MAYOR_DOTOUR`. room_02 = Madame Aroma's drawing room: `En_Al` ("Madame Aroma"), `En_Toto`; door `MAYORS_RESIDENCE_MADAME_AROMA`; receptionist ANN "drawing room on the right" (ovl:En_Recepgirl:165-169). room_03 = bedroom (door `MAYORS_RESIDENCE_BEDROOM`), reachable only through room_02. Whose bedroom it is is not stated in source (U-05).

**Plan from collision (C, scratch `wall2.py`).** Walls are 60 u thick and the doors sit mid-wall. Lobby: x -225..225, back wall z -120, exit floor out to z 480. Office: x -315..-15, z -480..-180. Drawing room: x 15..315, z -480..-180. Bedroom: x 375..675, z -480..30. Facing the receptionist (-Z), the office is on the left (-X) and the drawing room on the right (+X), as the ANN text comments say (ovl:En_Recepgirl:130, :165-169).

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:36-41; decode C)

Start mode `(params & 0xF00) >> 8`, cam byte `params & 0xFF` (`include/z64player.h:17-18`). The cam byte indexes the **actor cutscene camera list** (`Camera_ChangeActorCsCamIndex`, `src/code/z_play.c:2326-2327`). Yaw `(rawRotY >> 7) & 0x1FF`.

| Spawn | Position (Unity) | Yaw | Params | Mode / cam | Room (EntranceEntry) | Used by |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | (3, 0, 304) (0.03, 0.00, -3.04) | 180 (faces -Z, into the lobby) | `0x0E11` | 0xE / csCam 17 `CAM_SET_START2` at (26, 61, 324) | 0 (`{0x00, 0x00}`, scene.c:58) | entrance 0 = `0x0000`, East exit slot 7 behind East TA5 (`portal.east_clock_town.mayors_residence`); 41 u inside the main door TA3 |
| 1 | (-165, 0, -289) (-1.65, 0.00, 2.89) | 180 | `0x0DFF` | 0xD / none | 1 (`{0x01, 0x01}`, scene.c:59) | `ENTRANCE(MAYORS_RESIDENCE, 1)` literal set by En_Dt at the end of the Couple's Mask scene (ovl:En_Dt:530); 139 u inside the office door TA0 |

Entrance 2 (`sMayorsResidenceEntrance2`, spawn 2, z_scene_table.c:33-35) has no `EntranceEntry` (the list at 0x100 has 2 rows; the exit list starts at 0x104) and the scene has no spawn 2. A grep of `src/` and of all 22 extracted exit lists finds no `ENTRANCE(MAYORS_RESIDENCE, 2)` and no exit value `0x0020` (C): unused, which answers LOCATION_REGISTER U-04 for this scene. `ENTRANCE(MAYORS_RESIDENCE, 0)` otherwise appears only in the debug map select (`src/overlays/gamestates/ovl_select/z_select.c:316`).

### 2.2 Exit list (HDR scene.c:62-65) and consumers

| Slot | Value | Decode (z_scene_table.c:2688-2694) | Destination | Consumer in this scene |
| --- | --- | --- | --- | --- |
| 0 | `0xD270` | entrance scene 0x69 `Z2_TOWN`, entrance 7, layer 0 -> `sEastClockTownEntrance7[0] = { SCENE_TOWN, 7, 0x4102 }` (:2228-2230) | East spawn 7 (1099, 200, -1992) yaw 0, room 0 (East dossier 2.1) | floor surface 2, polys 60/61 (exit+1 = 1) behind TA3; all four `En_Time_Tag` kickouts (`params & 0x1F = 0`, ovl:En_Time_Tag:287) |
| 1 | `0x0010` | entrance scene 0x00 (this scene), entrance 1 -> spawn 1, room 1 | the office (2.1) | **none**: no surface has exit+1 = 2, and of the 16 overlays placed here only En_Time_Tag reads `setupExitList` (grep), and only slot 0. En_Dt reaches spawn 1 with a literal `ENTRANCE()` instead (C). Answers REF-02 U7 for this scene |

### 2.3 Collision surfaces carrying exits (C from scene.c:374-381 and the polygon list)

| Room | Surface | data[0] / data[1] | Exit+1 -> slot | Polys | Kind | Extent x / y / z | Flags |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 00 | 2 | `0x00000104` / `0x000017C2` | 1 -> 0 `0xD270` | 60, 61 | floor | -60..60 / 0 / 360..480 | xp 1 (ignore camera); bgCam 4; stone; no conveyor |
| 01, 02, 03 | none | | | | | | |

Only 1 of 22 surface types carries an exit. The player passes only floor polygons to `Player_HandleExitsAndVoids` (`player.c:6405-6416`; call sites `:6783` door probe and `:12572` standing floor).

### 2.4 Transition actors (HDR scene.c:43-48; decode C)

`TransitionActorEntry { sides[2] {room, bgCamIndex}, id, pos, rotY, params }`, sides[0] = front (`include/z64scene.h:369-378`). Spawned while the current or previous room equals either side (`src/code/z_actor.c:3600-3625`); yaw `(rotY >> 7) & 0x1FF`, csId `rotY & 0x7F`. Opening from the door's local +Z side sets `doorDirection = +1` (ovl:En_Door:496); the player then requests room `sides[doorDirection > 0 ? 0 : 1]` and uses that side's camera (`player.c:6791-6796`, `:6842-6848`), so **sides[0] is the room on the door's local -Z side**. The floor probe sits 22 u past the door (`player.c:6756-6783`, `func_80835BF8` :6548-6551; scratch `col.py`). A door bgCamIndex without bit 12 indexes the actor cutscene camera list (`src/code/z_camera.c:767-790`, `:7982`, `:7442`).

| TA | Position (Unity) | Yaw | Params -> type / schedule | Door art (ovl:En_Door) | Front room / cam ; back room / cam | Probe, player on local +Z | Probe, player on local -Z | Hours (`En_Door.schl`) |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-165, 0, -150) (-1.65, 0, 1.50) | 0 | `0x0292` -> 5 SCHEDULE / 18 `MAYORS_RESIDENCE_MAYOR_DOTOUR` | `DOOR_OBJINFO_35` -> gameplay_keep `gDoorLeftDL`/`gDoorRightDL` (:230, :307) | 1 / csCam 16 `CIRCLE3` (-36,154,-198) ; 0 / csCam 19 `FIXED1` (39,94,132) | player in lobby -> poly 146 (office floor, no exit): swap 0 -> 1 | player in office -> poly 30 (lobby): swap 1 -> 0 | always open (`return_none`, :296-299) |
| 1 | (165, 0, -150) (1.65, 0, 1.50) | 0 | `0x0293` -> SCHEDULE / 19 `MAYORS_RESIDENCE_MADAME_AROMA` | `DOOR_OBJINFO_36`, default pair (:231) | 2 / csCam 18 `CIRCLE3` (32,94,-383) ; 0 / csCam 13 `FIXED1` (-49,86,139) | lobby -> poly 199 (drawing room): swap 0 -> 2 | drawing room -> poly 28: swap 2 -> 0 | always open (:301-304) |
| 2 | (345, 0, -435) (3.45, 0, 4.35) | 90 (raw 11647 = `0x2D7F`) | `0x0294` -> SCHEDULE / 20 `MAYORS_RESIDENCE_BEDROOM` | `DOOR_OBJINFO_37`, default pair (:232) | 2 / csCam 18 ; 3 / csCam 24 `CIRCLE3` (661,146,-196) | bedroom (x > 345) -> poly 214 (drawing room): swap 3 -> 2 | drawing room -> poly 345 (bedroom): swap 2 -> 3 | always open (:306-309) |
| 3 | (0, 0, 345) (0, 0, -3.45) | 0 | `0x0291` -> SCHEDULE / 17 `MAYORS_RESIDENCE_MAIN_ENTRANCE` | `DOOR_OBJINFO_34` -> object_wdor01 `gLotteryCuriosityShopMayorHouseDoorDL` 0x548 (:229, :313-314) | 0 / 255 ; 0 / 255 (generic `CAM_SET_DOORC`, z_camera.c:7977-7978) | outside (z > 345) -> poly 69, lobby floor | **inside -> poly 61, surface 2, slot 0 `0xD270` -> East spawn 7** | 5.1; the schedule runs only from local +Z (ovl:En_Door:537), so never for a player leaving |

All four doors are `ENDOOR_TYPE_SCHEDULE`, not framed, so none loads door collision (ovl:En_Door:338-341). Inner-door probes land on plain floors: they swap rooms and never change scene (C).

### 2.5 Cross-check with East Clock Town (`docs/city/districts/east_clock_town.md` 2.2-2.4) and the registers

| Item | East dossier / register | This scene | Result |
| --- | --- | --- | --- |
| Inbound door | East TA5 (1100, 200, -2020) yaw 0, `0x0291` schedule 17, object_wdor01 0x548, probe -> poly 476, slot 7 `0x0000` | entrance 0 -> spawn 0 (3, 0, 304) yaw 180 room 0; TA3 has the same schedule type and door art | agrees |
| Return | `0xD270` -> East spawn 7 (1099, 200, -1992) yaw 0, 28 u in front of TA5 | TA3 inside probe -> poly 61 -> slot 0 `0xD270` | agrees |
| Door hours | East: open Days 1-2 10:00-20:00 (`En_Door.schl`:275-294); PORTAL_REGISTER rows say "open hours UNKNOWN" | same script; Day 3 opens at 10:00 and never closes (:279-285) | register rows can now carry the hours (not edited here) |
| Room portals | PORTAL_REGISTER rows 266-271: TA0 1<->0, TA1 2<->0, TA2 2<->3, "hours UNKNOWN" | sides and probes as 2.4; all three inner schedules `return_none` | agrees; hours = always open |
| Kickout | `EN_TIME_TAG 0x00C0 x4 (room_00:53, room_01:49, room_02:52, room_03:42)`, 20:00, [D1d+D1n+D2d+D2n] | same lines and decode (4.6) | agrees |
| Counts | LOCATION_REGISTER: r00 12/9, r01 11/9, r02 11/6, r03 7/4; 4 transition actors; 2 spawns; 2 exits; entrances 2/3 | HDR r0N.c:23-24, scene.c:19-27 | agrees |
| Exit `0x0010` | LOCATION_REGISTER section 4: entrance 1 is "also reached by the scene's own exit `0x0010`" | slot 1 has no reader (2.2) | wording only: the value equals En_Dt's literal, but nothing reads slot 1 |

No disagreement with the East dossier was found.

### 2.6 Portal IDs (frozen in `shared/city/ids.json`; reused, none invented)

| Portal ID | Mode | Source evidence | Arrival | Reverse |
| --- | --- | --- | --- | --- |
| `portal.east_clock_town.mayors_residence` (inbound, East's record) | door | East TA5 -> East slot 7 `0x0000` | spawn 0, room 0, csCam 17 | `portal.mayors_residence.east_clock_town` |
| `portal.mayors_residence.east_clock_town` | door | TA3 -> poly 61, slot 0 `0xD270` | East spawn 7 | inbound row |
| `portal.mayors_residence.east_clock_town.kickout` | special | 4 x En_Time_Tag type 0, slot 0, 20:00, Days 1-2 | East spawn 7 | one-way |
| `portal.mayors_residence.mayors_residence.r0_r1` / `.r1_r0` | room | TA0 | room 1, cam 16 / room 0, cam 19 | each other |
| `portal.mayors_residence.mayors_residence.r0_r2` / `.r2_r0` | room | TA1 | room 2, cam 18 / room 0, cam 13 | each other |
| `portal.mayors_residence.mayors_residence.r2_r3` / `.r3_r2` | room | TA2 | room 3, cam 24 / room 2, cam 18 | each other |

Not a portal: the Couple's Mask reload `ENTRANCE(MAYORS_RESIDENCE, 1)` (ovl:En_Dt:530) re-enters this scene at spawn 1, room 1 (story event, 5.2).

## 3. Object banks per room header (HDR r0N.c:28-41; IDs from `include/tables/object_table.h`)

| Bank | ID | r00 slot | r01 | r02 | r03 | Consumer (profile `objectId`) |
| --- | --- | --- | --- | --- | --- | --- |
| `OBJECT_DAIKU` | 0x0F1 | 0 | 0 | 0 | 0 | En_Daiku (r01) |
| `OBJECT_IN2` | 0x248 | 1 | 1 | 1 | 1 | En_Gm, Gorman (r00, r02) |
| `OBJECT_ZM` | 0x23A | 2 | 2 | 2 | 2 | En_Toto (r02) |
| `OBJECT_AL` | 0x00D | 3 | 3 | 3 | 3 | En_Al (r02) |
| `OBJECT_SDN` | 0x1B6 | 4 | 4 | 4 | 4 | En_Heishi (r00, r01) |
| `OBJECT_OBJ_TOKEIDAI` | 0x18C | 5 | 5 | 5 | - | Obj_Tokeidai wall clock (r00); already extracted |
| `OBJECT_DT` | 0x241 | 6 | 6 | 6 | - | En_Dt (r01) |
| `OBJECT_BAI` | 0x247 | 7 | 7 | 7 | - | En_Baisen (r00, r01) |
| `OBJECT_TORYO` | 0x0F0 | 8 | 8 | 8 | - | En_Muto (r01) |
| `OBJECT_HARIKO` | 0x1F2 | 9 | 9 | 9 | 5 | Obj_Hariko (r00, r01) |
| `OBJECT_BG` | 0x129 | 10 | - | - | - | En_Recepgirl (r00 only) |
| `OBJECT_WDOR01` | 0x274 | 11 | 10 | 10 | 6 | main door TA3 (`DOOR_OBJINFO_34`) |
| Count | | 12 | 11 | 11 | 7 | matches LOCATION_REGISTER |

Implicit: `gameplay_keep` 0x001 (En_Light, En_Time_Tag, En_Test4, En_Talk, the three inner doors, `gDoorSkel`). An actor whose profile bank is not loaded fails to spawn (`src/code/z_actor.c:3517-3523`). The NPC banks sit at the same slot indices in rooms 00-02, so INF they stay resident across those room swaps (object reload rules NI).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): id `& 0x1FFF`; rotation `(raw >> 7) & 0x1FF` per axis, passed as degrees unless the id carries the axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), in which case the value is a plain integer. csId `rawRotY & 0x7F` (0x7F = none). Half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all.
- `Actor_SpawnSetupActors` (2546-2577) spawns entries whose mask contains the current half-day bit; `Actor_InitHalfDaysBit` (2491-2499) gives Day 1 12:00 -> `0x080`. Bits: D0d 0x200, D0n 0x100, D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001.
- `ACTOR_EN_TIME_TAG | 0xE000` and `ACTOR_EN_TALK | 0xE000` carry all three axis flags, so their rotations are data (kickout hour:minute; En_Talk attention range).
- Columns: "D1d" = mask contains Day 1 daytime; "Noon" = present at 12:00 on a fresh save after the overlay's init gates. Classes as in the brief.

### 4.2 room_00 lobby (9 entries, HDR r00.c:43-70)

| # (line) | Actor (id) | Position | Raw rot X / Y / Z | Rot | Mask | CS | Params | Bank | Class | D1d | Noon |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 (44) | `ACTOR_EN_LIGHT` (0x004) | (-64, 53, -49) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 all nights | none | 0x83F0 | gameplay_keep | billboard-effect | no | no (mask) |
| 1 (47) | `ACTOR_EN_GM` (0x0A4) | (0, 0, 448) | 0x0007 / 0x5A1C / 0x007F | 0 / 180 / 0 | 0x3FF | 28 | 0x0000 | object_in2 | npc-scheduled | yes | yes, walking path 7 (5.5) |
| 2 (50) | `ACTOR_EN_HEISHI` (0x26D) | (-154, 0, 51) | 0x0007 / 0x877F / 0x007F | 0 / 270 / 0 | 0x3FF | none | 0x0000 | object_sdn | npc-scheduled | yes | no (init kill, 4.6) |
| 3 (53) | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-16, 0, 197) | 0x0A01 / 0x007F / 0x0070 | int 20 / 0 / 0 | 0x0F0 D1d, D1n, D2d, D2n | none | 0x00C0 | gameplay_keep | controller-invisible | yes | yes |
| 4 (56) | `ACTOR_OBJ_HARIKO` (0x20A) | (-65, 48, -53) | 0x0007 / 0x09FF / 0x007F | 0 / 19 / 0 | 0x3FF | none | 0x0000 | object_hariko | static-mesh | yes | yes |
| 5 (59) | `ACTOR_EN_BAISEN` (0x26C) | (-195, 0, 49) | 0x0007 / 0x6BFF / 0x007F | 0 / 215 / 0 | 0x3FF | none | 0x0000 | object_bai | npc-scheduled | yes | no (init kill) |
| 6 (62) | `ACTOR_EN_RECEPGIRL` (0x290) | (-7, 0, -85) | 0x0007 / 0x007F / 0x0070 | 0 / 0 / 0 | 0x3F0 D0d-D2n | none | 0x0005 | object_bg | npc-scheduled | yes | yes |
| 7 (65) | `ACTOR_EN_TEST4` (0x15A) | (31, 0, 101) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | 0x3FF | 9 | 0xFD00 | gameplay_keep | controller-invisible | yes | yes |
| 8 (68) | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (0, 114, -116) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0xA07F | object_obj_tokeidai | static-mesh | yes | yes |

### 4.3 room_01 office (9 entries, HDR r01.c:42-69)

| # (line) | Actor (id) | Position | Raw rot X / Y / Z | Rot | Mask | CS | Params | Bank | Class | D1d | Noon |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 (43) | `ACTOR_EN_LIGHT` | (-142, 54, -345) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 nights | none | 0x83F0 | gameplay_keep | billboard-effect | no | no (mask) |
| 1 (46) | `ACTOR_EN_DT` (0x26F) | (-160, 0, -430) | 0x0007 / 0x0011 / 0x007F | 0 / 0 / 0 | 0x3FF | 17 | 0x0000 | object_dt | npc-scheduled | yes | yes |
| 2 (49) | `ACTOR_EN_TIME_TAG \| 0xE000` | (-174, 0, -281) | 0x0A01 / 0x007F / 0x0070 | int 20 / 0 / 0 | 0x0F0 | none | 0x00C0 | gameplay_keep | controller-invisible | yes | yes |
| 3 (52) | `ACTOR_OBJ_HARIKO` | (-138, 49, -348) | 0x0007 / 0x977F / 0x007F | 0 / 302 / 0 | 0x3FF | none | 0x0000 | object_hariko | static-mesh | yes | yes |
| 4 (55) | `ACTOR_EN_TEST4` | (-100, 0, -240) | 0x0007 / 0x000B / 0x007F | 0 / 0 / 0 | 0x3FF | 11 | 0xFD00 | gameplay_keep | controller-invisible | yes | singleton kill if already loaded |
| 5 (58) | `ACTOR_EN_MUTO` (0x26B) | (-245, 0, -314) | 0x0007 / 0x2D7F / 0x0078 | 0 / 90 / 0 | 0x3F8 D0d-D3d | none | 0x0001 | object_toryo | npc-scheduled | yes | yes |
| 6 (61) | `ACTOR_EN_HEISHI` | (-53, 0, -286) | 0x0007 / 0x877F / 0x0078 | 0 / 270 / 0 | 0x3F8 | none | 0x0001 | object_sdn | npc-scheduled | yes | yes |
| 7 (64) | `ACTOR_EN_BAISEN` | (-84, 0, -315) | 0x0007 / 0x877F / 0x0078 | 0 / 270 / 0 | 0x3F8 | none | 0x0001 | object_bai | npc-scheduled | yes | yes |
| 8 (67) | `ACTOR_EN_DAIKU` (0x09C) | (-276, 0, -284) | 0x0007 / 0x2D7F / 0x0078 | 0 / 90 / 0 | 0x3F8 | none | 0x0000 | object_daiku | npc-scheduled | yes | yes |

### 4.4 room_02 drawing room (6 entries, HDR r02.c:42-60)

| # (line) | Actor (id) | Position | Raw rot X / Y / Z | Rot | Mask | CS | Params | Bank | Class | D1d | Noon |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 (43) | `ACTOR_EN_AL` (0x262) | (118, 0, -416) | 0x0007 / 0x077F / 0x0070 | 0 / 14 / 0 | 0x3F0 D0d-D2n | none | 0x0000 | object_al | npc-scheduled | yes | yes (schedule result 1) |
| 1 (46) | `ACTOR_EN_GM` | (239, 0, -292) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0000 | object_in2 | npc-scheduled | yes | no: singleton kill, Gorman already spawned from r00 a01 |
| 2 (49) | `ACTOR_EN_TOTO` (0x234) | (83, 11, -329) | 0x0001 / 0x3D7F / 0x0020 | 0 / 122 / 0 | 0x0A0 D1d, D2d | none | 0x050B | object_zm | npc-scheduled | yes | yes (seated) |
| 3 (52) | `ACTOR_EN_TIME_TAG \| 0xE000` | (223, 0, -251) | 0x0A01 / 0x007F / 0x0070 | int 20 / 0 / 0 | 0x0F0 | none | 0x00C0 | gameplay_keep | controller-invisible | yes | yes |
| 4 (55) | `ACTOR_EN_LIGHT` | (298, 0, -330) | 0x0007 / 0x007F / 0x0070 | 0 / 0 / 0 | 0x3F0 | none | 0x07F4 | gameplay_keep | billboard-effect | yes | yes |
| 5 (58) | `ACTOR_EN_TEST4` | (197, 0, -318) | 0x0007 / 0x000D / 0x007F | 0 / 0 / 0 | 0x3FF | 13 | 0xFD00 | gameplay_keep | controller-invisible | yes | singleton kill |

### 4.5 room_03 bedroom (4 entries, HDR r03.c:38-50)

| # (line) | Actor (id) | Position | Raw rot X / Y / Z | Rot | Mask | CS | Params | Bank | Class | D1d | Noon |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 (39) | `ACTOR_EN_TALK \| 0xE000` (0x261) | (452, 34, -4) | 0x0107 / 0x5A7F / 0x007F | int 2 / 180 / 0 | 0x3FF | none | 0xFE04 | gameplay_keep | controller-invisible | yes | yes |
| 1 (42) | `ACTOR_EN_TIME_TAG \| 0xE000` | (461, 0, -254) | 0x0A01 / 0x007F / 0x0070 | int 20 / 0 / 0 | 0x0F0 | none | 0x00C0 | gameplay_keep | controller-invisible | yes | yes |
| 2 (45) | `ACTOR_EN_LIGHT` | (391, 0, -328) | 0x0007 / 0x007F / 0x0070 | 0 / 0 / 0 | 0x3F0 | none | 0x07F4 | gameplay_keep | billboard-effect | yes | yes |
| 3 (48) | `ACTOR_EN_TEST4` | (530, 0, -188) | 0x0007 / 0x000F / 0x007F | 0 / 0 / 0 | 0x3FF | 15 | 0xFD00 | gameplay_keep | controller-invisible | yes | singleton kill |

### 4.6 Overlay findings per actor type (OVL)

| Actor | Init: scale, collider, skeleton | Draw | Presence / state gates |
| --- | --- | --- | --- |
| En_Recepgirl (ovl:En_Recepgirl:46-70, :100-171) | flex `object_bg_Skel_011B60` (object_bg 0x11B60), shape yOffset -60, default scale 0.01; **no collider** | skeletal, eye texture swap | mask only; switch flag = params = 5 ("heard directions", set at :150); text 0x2AD9 / 0x2ADC; after 0x2AD9 branches on `WEEKEVENTREG_RESOLVED_MAYOR_MEETING` (:153: 0x2ADF ended / 0x2ADA ongoing); Kafei's Mask worn -> 0x2367 (:108-109) |
| En_Heishi (ovl:En_Heishi:53-80, :151-163) | flex `gSoldierSkel` (object_sdn 0xD640), stand-hand-on-hip loop; cylinder r20 h60, p1 r30 h60 | skeletal | p0 (lobby) killed unless `RESOLVED_MAYOR_MEETING` or (day 3 and night) (:63-68); p1 (office) killed if either holds (:73-76), and again in Update on day 3 night (:161-162) |
| En_Baisen (ovl:En_Baisen:77-108, :138-253) | flex `object_bai_Skel_007908`; p1 cylinder r30 h60 | skeletal | p0 (lobby) same gate as Heishi p0 (:85-90); finds En_Heishi and runs a four-line exchange that alternates focus (text 0x2AD5..0x2AD8, :224-253). p1 (office) killed like Heishi p1 (:95-98); text 0x2ABD, 0x2ABB after `WEEKEVENTREG_ATTENDED_MAYOR_MEETING` (:171), 0x2AC6 with the Couple's Mask worn (:174); talking sets `cutsceneState = 1`, which hands the scene to En_Dt (:187-200) |
| En_Muto (ovl:En_Muto:55-90, :165-185) | flex `object_toryo_Skel_007150`; p1 r30 h60 | skeletal | p1 `isInMayorsRoom` (:63): killed if RESOLVED or day 3 night (:79-82); text 0x2ABD / 0x2ABB attended / 0x2AC6 Couple's Mask (:174-180). The p0 variant (final night, South Clock Town `Z2_CLOCKTOWER_room_00.c:84`) is not in this venue |
| En_Daiku p0 (ovl:En_Daiku:95-143, :174-195) | flex `object_daiku_Skel_00A850`, animation 0 (types 0 and 1, :177-180); r30 h60, lock-on disabled | skeletal | killed if RESOLVED or day 3 night (:110-118); text by day (:194-195) |
| En_Dt (ovl:En_Dt:234-258, :354-398, :772-805) | flex `object_dt_Skel_00B0CC`, scale 0.01; cylinder r25 h70; collects 11 csIds from csId 17 (:251-255) | skeletal with eye/brow textures | always (mask 0x3FF); appearance by flags (5.2); day 3 night switches to the final-night state (:247-248, :779-781) |
| En_Al (ovl:En_Al:1106-1121, :1074-1088, :1230-1256) | flex `gMadameAromaSkel` (object_al 0xA0D8), scale 0.01; cylinder r14 h62 | drawn only while `scheduleResult != 0` (:1238) | schedule `En_Al.schl`:1-25 (5.3) |
| En_Gm (ovl:En_Gm:1797-1820, :1715-1731, :1960-1979) | **killed if another En_Gm exists** (:1800-1803); `actor.room = -1` (:1814); flex `object_in2_Skel_0078B0`; r14 h62 plus sphere | drawn only if `scheduleResult != 0` and OBJECT_IN2 loaded (:1967) | schedule `En_Gm.schl`:1-69 (5.5) |
| En_Toto (ovl:En_Toto:274-291, :316-322, :350-370) | flex `gTotoSkel` (object_zm 0xA978); sitting idle in `SCENE_SONCHONOIE` (:286-288); r20 h60 | skeletal | mask D1d, D2d; Milk Bar time kill does not apply here (:279-283); talk offered when the player is within 0x2000 of his home yaw |
| En_Talk (ovl:En_Talk:29-62; `z_en_talk.h:6-7`) | no draw; scale 1.0 | none | text `0x1C00 + (params & 0x3F)` = 0x1C04; attention range `home.rot.x - 1` = 1; offers talk within 40 u when faced or locked on (:57-59). Text content NI |
| En_Time_Tag type 0 kickout (`z_en_time_tag.h:10-15`; ovl:En_Time_Tag:280-338) | controller | none | at hour:minute = `home.rot.x : home.rot.y` = 20:00 shows text `0x1883 + 6` = 0x1889, then `nextEntrance = setupExitList[0]` (:287, :303, :333-336); deferral via `WEEKEVENTREG_KICKOUT_WAIT` / `_TIME_PASSED` (:300-338); the Room Key exception is `SCENE_YADOYA` only (:319) |
| En_Test4 (ovl:En_Test4:322-345) | singleton (`sIsLoaded`, :339-343), `room = -1` | none | day/night transition controller; per-room csIds 9, 11, 13, 15 (4.7) |
| En_Light (ovl:En_Light:48-76, :97-115, :161-196) | 0x83F0: params < 0 -> small flame `gameplay_keep_DL_01ACF0` (:182), light radius 100 at Y + 1; 0x07F4: `gEffFire1DL` gameplay_keep 0x7D590 (:175), colour row 4 (255,200,0)/(255,0,0), scale 40 x 0.0001 = 0.004, light radius 300, torch SFX (:104, :112-114) | XLU billboard turned to the camera yaw, two-texture scroll | mask only (candles nights, fires Days 0-2) |
| Obj_Hariko (ovl:Obj_Hariko:36-45, :74-104) | scale 0.1; no collider | `gHarikoBodyDL` 0x80 + `gHarikoFaceDL` 0x110 (object_hariko) | always; the head bobs only while a quake is active (:74-78). ANN "Little Cow Statue Head" |
| Obj_Tokeidai type 10 `SMALL_WALL_CLOCK` (`z_obj_tokeidai.h:6, :18`; ovl:Obj_Tokeidai:254-259, :811-845) | scale 0.01; clock state from the current time | minute ring 0xCF28, centre/hand 0xBEE8, `gWallClockClockFaceDL` 0xF518, sun/moon panel 0xC368 (panel turned 0x8000 at night, :88-93) | always |
| En_Door | section 2.4 | | |

### 4.7 Actor cutscene and camera use (HDR scene.c:185-215, :309-334; C)

| Consumer | csId chain -> csCam indices (setting, first point) |
| --- | --- |
| En_Dt r01 a01 (meeting) | 17-27 (11 entries, `EN_DT_CUTSCENE_COUNT`) -> cams 5, 3, 6, 7, 12, 9, 2, 11, 8, 4, 10: `FIXED1` / `CIRCLE8` points inside the office (x -220..-118, z -402..-157) |
| En_Test4 r00 / r01 / r02 / r03 | 9->10, 11->12, 13->14, 15->16 -> cams (14, 15) lobby (-43,69,279); (0, 1) office (-223,45,-382); (20, 21) drawing room (190,43,-368); (22, 23) bedroom (475,55,-458) |
| En_Gm r00 a01 (receptionist talk) | 28 -> cam 25 `FIXED1` (48, 51, -89), in front of the desk |
| Spawn 0 | cam 17 `START2` (26, 61, 324) |
| Doors | cams 13, 16, 18, 19, 24 (2.4) |
| Entries 0-8 | chained 0 -> 8, priorities 100-800, negative csCam ids; no placed actor uses csId 0-8 (U-06) |

### 4.8 Counts

| | r00 | r01 | r02 | r03 | Total |
| --- | --- | --- | --- | --- | --- |
| Entries (HDR) | 9 | 9 | 6 | 4 | 28 |
| D1d by mask | 8 | 8 | 6 | 4 | 26 |
| Present at Day 1 noon, fresh save | 6 | 7 (+ Test4 singleton kill, U-04) | 4 (+ Gorman persisting from r00; duplicate and Test4 killed) | 3 (+ Test4 killed) | |
| static-mesh / npc-scheduled / controller / billboard | 2 / 4 / 2 / 1 | 1 / 5 / 2 / 1 | 0 / 3 / 2 / 1 | 0 / 0 / 3 / 1 | 3 / 12 / 9 / 4 |
| Transition actors (scene) | | | | | 4 doors |

## 5. Venue mechanics index (INT-08: audience, time and story predicates, group dialogue, stable placement)

### 5.1 Access window

| Predicate | Rule | Evidence |
| --- | --- | --- |
| Enter from East | East TA5 schedule 17 returns none on Days 1-2 for 10:00 <= t < 20:00 and on Day 3 for t >= 10:00; otherwise `return_s (0x15)` -> text 0x1815 and the door stays shut | OVL `En_Door.schl`:275-294; ovl:En_Door:537-548; schedule range semantics `src/code/z_schedule.c:25-47` (end exclusive) |
| Leave | always: the schedule test runs only from the door's local +Z side (outside) | ovl:En_Door:537 |
| Inner doors | always open (`return_none`) | `En_Door.schl`:296-309 |
| Kickout | 20:00 on Days 1-2: one En_Time_Tag per room (mask 0x0F0), text 0x1889, to `0xD270` East spawn 7. No kickout on Day 3 | HDR r00.c:53, r01.c:49, r02.c:52, r03.c:42; OVL 4.6 |
| Clock | keeps running inside; half-day refresh at 06:00/18:00 kills actors whose mask lacks the new bit | HDR r0N.c:21; `src/code/z_actor.c:3311-3330` (SOURCE_REGISTER 2.3) |
| Day 1 noon | **open** (Day 1, 12:00 is inside 10:00-20:00) | as above |

### 5.2 Mayor's office (room_01): the Mutoh vs Viscen meeting and the Couple's Mask resolution (ovl:En_Dt unless noted)

Seating (C positions; arrangement INF): Mayor (-160, 0, -430) yaw 0, at the far (-Z) end facing the door, on a seat floor Y 18 (poly 182); Mutoh (-245, 0, -314) and the carpenter (-276, 0, -284) on the west side facing +X; Viscen (-84, 0, -315) and the soldier (-53, 0, -286) on the east side facing -X.

| Mechanic | Implementation | Evidence |
| --- | --- | --- |
| Participants found | En_Dt walks the NPC list for `ACTOR_EN_MUTO` and `ACTOR_EN_BAISEN`; focus targets switch per line through `sCutsceneFocusTargetTable` | :339-352, :115-148, :309-337 |
| Trigger | talk to the Mayor (range 150) or to Mutoh/Viscen, whose `cutsceneState = 1` makes En_Dt start the scene | :400-443 (:432-435); ovl:En_Baisen:187-200; ovl:En_Muto:198 |
| Text flow | `sTextIds` (27 ids, :109-112): index 0-8 first meeting, 9-10 repeat after attending, 11-22 Couple's Mask resolution, 23 after the reward, 24-26 final night; text -> cutscene table `sStringIdCsIndexTable` (:152-165) | :486-628 |
| Attended flag | `WEEKEVENTREG_ATTENDED_MAYOR_MEETING` (60, 0x08) set at the end of each meeting dialogue (index 8, 10, 22, 23); notebook person `MAYOR_DOTOUR` queued at the end of the first meeting | :542-560, :656; `include/z64save.h:1204` |
| Couple's Mask | `Player_GetMask(play) == PLAYER_MASK_COUPLE` (worn) -> index 11; Mutoh line 4 and Viscen line 6 = 0x2AC6; appearance `RESOLUTION_TALK` if already attended, else `RESOLVED` | :418-424, :460-468 |
| Resolution | at index 20: `nextEntrance = ENTRANCE(MAYORS_RESIDENCE, 1)`, exit fade, `TRANS_TRIGGER_START`, `SET_WEEKEVENTREG(WEEKEVENTREG_RESOLVED_MAYOR_MEETING)` (63, 0x80) | :526-540; z64save.h:1238 |
| Reward | on reload (spawn 1, room 1) with RESOLVED and no heart piece yet: text 21 starts at once with the player held (`PLAYER_CSACTION_WAIT`); then `GI_HEART_PIECE`; `WEEKEVENTREG_RECEIVED_MAYOR_HEART_PIECE` (60, 0x10); notebook `RECEIVED_MAYOR_HP` + person | :357-369, :685-716; z64save.h:1205 |
| After resolution | office Mutoh/Viscen/soldier/carpenter killed at init; lobby Viscen + soldier (p0) appear; receptionist says the meeting ended (0x2ADF); Mayor with Kafei's Mask worn -> 0x2368 | 4.6; :438-440, :745-747 |
| Final night (day 3, isNight) | office NPCs absent (mask 0x3F8 has no D3n, plus init kills); Mayor tap-desk state, text 24 then 25 once (`WEEKEVENTREG_TALKED_MAYOR_NIGHT_3`, 60, 0x40), else 26 | :718-770; z64save.h:1207 |
| Audio | while unresolved and not day 3 night: `NA_BGM_MAYORS_OFFICE` as sub-BGM at range 1000 and `NA_SE_EV_CROWD` | :783-786; sequence_table.h:118 |
| Day 1 noon appearance | fresh save: phase DEFAULT -> distress loop, look-down eyes, no blink; then `PRE_MEETING` | :296-307, :378-389 |

### 5.3 Madame Aroma's drawing room (room_02): schedule, Kafei's Mask, group scene, Letter to Mama

| Mechanic | Implementation | Evidence |
| --- | --- | --- |
| Aroma schedule (Days 1-2) | 08:00-10:00 result 1; 10:00-10:55 1; 10:55-11:45 **3 on Day 1**, 1 on Day 2; 11:45-20:00 1; else none (not drawn). Day 3: 18:00-06:00 result 2, else none | `En_Al.schl`:1-25; ovl:En_Al:1074-1088 |
| Result 1 talk | Deku form -> `D_80BDFDD0` (sets `WEEKEVENTREG_92_08`); **no Kafei's Mask in inventory** -> `D_80BDFE8C`; wearing it -> `D_80BDFE7C` (0x2A9C); carrying it -> `D_80BDFE84` (0x2A9D) | ovl:En_Al:744-765 |
| Kafei's Mask | `D_80BDFE8C`: intro 0x2A9E-0x2AA0 (skipped once `WEEKEVENTREG_57_02` is set), choice; accept -> `OFFER_ITEM(GI_MASK_KAFEIS_MASK)`, `SET_COLLECTIBLE(0x008F)` (meaning NI), notebook `RECEIVED_KAFEIS_MASK` + `MET_MADAME_AROMA`; decline -> 0x2AA1, sets `WEEKEVENTREG_57_02` | ovl:En_Al:259-290 (:275, :276, :279, :288); z64save.h:1175 |
| Group scene: Aroma + Gorman + Toto | **Day 1 10:55-11:45 only** (Aroma result 3, Gorman result 2). Aroma 3 needs En_Gm and En_Toto alive (ovl:En_Al:958-970); Gorman 2 needs En_Toto and En_Al (ovl:En_Gm:1258-1278). Shared script text 0x2AA6..0x2AB8 (ovl:En_Al:67-148, ovl:En_Gm:73-160), then `WEEKEVENTREG_86_20` (z64save.h:1459) and notebook `MET_MADAME_AROMA`, `MET_TOTO`, `MET_GORMAN`; repeat lines 0x2AEA/0x2AEB | as cited |
| Toto | mask D1d + D2d, seated idle in this scene, greeting 0x2A94; in Deku form the spoken flag is switch `home.rot.x` = 0 | ovl:En_Toto:140, :286-288, :342-345, :361-369 |
| Letter to Mama | handled only in Aroma's result 2 scripts (`PLAYER_IA_LETTER_MAMA`; `DELETE_ITEM(ITEM_LETTER_MAMA)`; first time `GI_CHATEAU_BOTTLE`, else `GI_RUPEE_PURPLE`; sets `WEEKEVENTREG_57_04`, `57_08`), and result 2 is Day 3 18:00-06:00. This room's En_Al has mask 0x3F0 (no Day 3); the Milk Bar En_Al (`Z2_MILK_BAR_room_00.c:68`, mask 0x004 D3n) is the one that runs it. **Not reachable in this venue** (C) | ovl:En_Al:188-243, :292-370, :767-790 |
| Fireplace | En_Light 0x07F4 at (298, 0, -330); no floor under it (inside a recess, INF); the bedroom has a matching fire at (391, 0, -328) across the 60 u wall | HDR r02.c:55, r03.c:45; scratch `probe.py` |

### 5.4 Lobby (room_00): receptionist, fixtures and lobby conversations

| Mechanic | Implementation | Evidence |
| --- | --- | --- |
| Reception desk | U-shaped raised floor Y 40 (polys 90-93, 106-107, x -90..45, z -120..-45), wall type 1 sides (surfaces 4, 5) so the player cannot ledge-climb it; receptionist inside at (-7, 0, -85) facing +Z (the entrance) | C collision; `src/code/z_bgcheck.c:18-27`; `player.c:11835` |
| Fixtures | cow statue on the desk at (-65, 48, -53) (desk top Y 40); small wall clock at (0, 114, -116), yaw 0, 4 u in front of the back wall plane z -120; night candle at (-64, 53, -49) | 4.2; collision poly 40-43 |
| Receptionist directions | switch flag 5 and texts (4.6); mask D0d-D2n, so absent on Day 3 | ovl:En_Recepgirl:65-70, :100-171 |
| Gorman + receptionist | Day 1 10:10-10:30: Gorman result 1 stands at path 1 end (-1, 0, -22) only if En_Recepgirl exists; script `D_80951A98` (sets `WEEKEVENTREG_86_40`, focus to child), cs 28 / cam 25 | ovl:En_Gm:1245-1256, :41-72, :570-611 |
| Viscen + soldier | after RESOLVED or on Day 3 night: Viscen p0 (-195, 0, 49) yaw 215 and soldier p0 (-154, 0, 51) yaw 270 talk to each other | 4.6 |

### 5.5 Gorman's Day 1 route (stable placement across rooms; `En_Gm.schl`:29-54, `if_scene (SCENE_SONCHONOIE)`)

Path index = `D_80951A0C[result]` (ovl:En_Gm:23-25) via `SubS_GetAdditionalPath` from base path `params & 0xFF` = 0 (`src/code/z_sub_s.c:485-499`). Door transits find the door by schedule type (ovl:En_Gm:369-404, `SubS_FindScheduleDoor`); if the needed partner or door actor is missing, the handler fails, `scheduleResult` becomes 0 and Gorman is not drawn until a later re-evaluation succeeds (ovl:En_Gm:1715-1731, :1967).

| Day 1 window | Result | Path (scene.c) | Handler | Needs | Where |
| --- | --- | --- | --- | --- | --- |
| 10:00-10:05 | 16 | 0: (0,450) -> (1,306) | door transit (:1132-1176) | TA3 | enters through the main door (East side uses result 10 on the same door, ovl:En_Gm:378-383) |
| 10:05-10:10 | 25 | 1 | timed path walk (:1178-1223) | | lobby, to the desk |
| 10:10-10:30 | 1 | end of 1: (-1, 0, -22) | receptionist talk (:1245-1256) | En_Recepgirl | in front of the desk |
| 10:30-10:40 | 26 | 2 | walk | | to the drawing-room door |
| 10:40-10:45 | 18 | 3: (176,-106) -> (179,-206) | door transit | TA1 | through TA1 |
| 10:45-10:55 | 27 | 4 | walk | | into the drawing room |
| 10:55-11:45 | 2 | end of 4: (137, 0, -352) | group scene (:1258-1278) | En_Toto, En_Al | drawing room |
| 11:45-11:55 | 28 | 5 | walk | | back to the door |
| 11:55-12:00 | 19 | 6: (174,-206) -> (171,-105) | door transit | TA1 | through TA1 into the lobby |
| **12:00-12:15** | **29** | **7: (171,-105) .. (10,306), 9 points** | walk | | **lobby, toward the main door (state at `prof.d1.noon.normal`)** |
| 12:15-12:20 | 17 | 8: (10,306) -> (10,420) | door transit | TA3 | leaves; East continues 12:15-12:55 (`En_Gm.schl`:20-25) |

Day 2 and Day 3 have no `SCENE_SONCHONOIE` branch: Gorman never appears here on those days (`En_Gm.schl`:70-120).

### 5.6 Presence by half-day (fresh save; access limits from 5.1 apply)

| Actor / group | D1 day | D1 night (18-20) | D2 day | D2 night (18-20) | D3 day (>= 10:00) | D3 night |
| --- | --- | --- | --- | --- | --- | --- |
| Receptionist | yes | yes | yes | yes | no | no |
| Mayor | yes | yes | yes | yes | yes | yes, final-night state |
| Office group: Mutoh, Viscen p1, soldier p1, carpenter | yes | yes | yes | yes | yes | no |
| Lobby Viscen p0 + soldier p0 | only if RESOLVED | same | same | same | same | yes |
| Aroma | yes (10:55-11:45 group) | yes until 20:00 | yes | yes until 20:00 | no | no (Milk Bar) |
| Toto | yes | no | yes | no | no | no |
| Gorman | 10:00-12:20 only | no | no | no | no | no |
| Candles r00, r01 | no | yes | no | yes | no | yes |
| Fires r02, r03 | yes | yes | yes | yes | no | no |
| Kickout armed | yes | fires 20:00 | yes | fires 20:00 | no | no |
| Cow statues, wall clock, En_Talk | yes | yes | yes | yes | yes | yes |

### 5.7 Stable placement rules (C/OVL)

- One Gorman: the room_02 entry dies at init when the room_00 Gorman exists (ovl:En_Gm:1800-1803); `room = -1` keeps him alive across room swaps (:1814); on a room change his draw waits for OBJECT_IN2 (:444-469).
- One day/night controller: En_Test4 singleton (ovl:En_Test4:339-343). The first room loaded supplies the transition cutscene ids (room_00 on entrance 0, room_01 on entrance 1).
- Doors exist only while an adjacent room is current or previous (`z_actor.c:3600-3625`). TA2 is therefore absent while the player is in the lobby, and Gorman's door results need TA1/TA3, which exist whenever the lobby is loaded.
- Every room has its own kickout tag, so the 20:00 kickout fires in whichever room is loaded.

### 5.8 Camera settings and proposed REF-03 camera bindings

BgCam list (HDR scene.c:350-372; `include/z64camera.h:161-228`): 0 `CIRCLE3` (-36,154,-198) office floors; 1 `CIRCLE3` (32,94,-383) drawing-room floors; 2 `CIRCLE0` (661,146,-196) and 3 `CIRCLE0` (661,147,-122) bedroom floors; 4 `ROOM1` (no data) lobby floors; 5 `NONE` on two drawing-room walls (surface 12).

| Registered camera id | Proposed source camera | Status |
| --- | --- | --- |
| `cam.mayors_residence.entry` | csCam 17 `START2` (26, 61, 324), the spawn 0 arrival camera | proposal |
| `cam.mayors_residence.reverse_door` | no source camera faces TA3 from inside; capture from about (0, 60, 200) toward +Z | proposal |
| `cam.mayors_residence.fixture` | reception desk with cow statue and wall clock: csCam 25 `FIXED1` (48, 51, -89) | proposal |
| `cam.mayors_residence.room_01` | bgCam 0 / csCam 16 (-36, 154, -198) | proposal |
| `cam.mayors_residence.room_02` | bgCam 1 / csCam 18 (32, 94, -383) | proposal |
| `cam.mayors_residence.room_03` | bgCam 2 / csCam 24 (661, 146, -196) | proposal |

The entry door is in room_00, so no rename is needed (SOURCE_REGISTER 4.6).

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

Header: the only header (scene layer 0). Arrival: entrance 0, spawn 0, room_00. Each room is assembled from its own file; all four rooms are listed because each is entered at noon. World position = actor position; rotation conversion under the Z mirror belongs to the importer (PIPE-02). State: fresh save, Human, no mask, Kafei's Mask not owned.

| Room / # | Component, params | N64 position (Unity) | Yaw | Scale | Display lists, bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| all | room meshes | r00..r03 cullable entries | | 1 | 24 / 23 / 27 / 21 `Gfx` arrays (section 8) | LOD-branched neighbour copies in r00 and r02 (U-01) |
| scene TA3 | main door `0x0291` | (0, 0, 345) (0, 0, -3.45) | 0 | 0.01 | object_wdor01 0x548 on `gDoorSkel` gameplay_keep 0x22B28 | closed rest pose (approximation) |
| scene TA0, TA1, TA2 | inner doors `0x0292`, `0x0293`, `0x0294` | (-165,0,-150), (165,0,-150), (345,0,-435) | 0, 0, 90 | 0.01 | gameplay_keep `gDoorLeftDL` 0x20BB8 / `gDoorRightDL` 0x20D00 on `gDoorSkel` | left/right DL chosen by eye side (East dossier 2.4) |
| r00 a04 | cow statue `0x0000` | (-65, 48, -53) (-0.65, 0.48, 0.53) | 19 | 0.1 | object_hariko 0x80 + 0x110 | on the desk |
| r00 a08 | small wall clock `0xA07F` | (0, 114, -116) (0.00, 1.14, 1.16) | 0 | 0.01 | object_obj_tokeidai 0xCF28, 0xBEE8, 0xF518, 0xC368 | hands at 12:00, panel day side; pose rules as the South tower clock (ROM_REFERENCE.md) |
| r01 a03 | cow statue `0x0000` | (-138, 49, -348) (-1.38, 0.49, 3.48) | 302 | 0.1 | object_hariko 0x80 + 0x110 | stands on furniture mesh without collision |
| r02 a04, r03 a02 | fire `0x07F4` (optional) | (298, 0, -330), (391, 0, -328) | camera | 0.004 | gameplay_keep `gEffFire1DL` 0x7D590 (XLU) | billboard plus point light r300; label any fixed-orientation import |

**Present at noon but skeletal (later pass, posed from rest or idle):** r00 a06 receptionist (object_bg 0x11B60); r00 a01 Gorman (object_in2 0x78B0, moving on path 7, at about (171, 0, -105) at 12:00); r01 a01 Mayor (object_dt 0xB0CC, distress loop), a05 Mutoh (object_toryo 0x7150), a06 soldier (object_sdn 0xD640), a07 Viscen (object_bai 0x7908), a08 carpenter (object_daiku 0xA850); r02 a00 Aroma (object_al 0xA0D8), a02 Toto (object_zm 0xA978, seated).

**Excluded by half-day mask:** r00 a00, r01 a00 (night candles).

**Excluded by state at noon (fresh save):** r00 a02 soldier p0 and r00 a05 Viscen p0 (killed until RESOLVED or Day 3 night); r02 a01 Gorman duplicate (singleton).

**No geometry:** En_Time_Tag x4, En_Test4 x4, r03 a00 En_Talk.

## 7. Collision summary (C from scene.c:350-1428; scratch `col.py`, `col2.py`, `surfq.py`, `wall2.py`)

One scene-level collision header (`Z2_SONCHONOIECollisionHeader_0036D4`, scene.c:1419-1428) covers all four rooms; per-room figures below assign each polygon by its centroid to the room footprints of section 1 (C; no polygon falls on a boundary).

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 495 / 535 / 22 |
| Header bounds | min (-315, 0, -495), max (675, 182, 480) |
| Vertex extents | x -315..675, y 0..180, z -480..480 (Unity x -3.15..6.75, y 0..1.80, z -4.80..4.80) |
| Kinds | 154 floors (144 under 5 deg; 10 at 20-59 deg), 349 walls, 32 ceilings |
| Water boxes | 0 (`0, NULL`) |
| Exit surfaces | 1 (surface 2, polys 60/61) |
| Special | wall type 1 (`WALL_FLAG_0`, blocks ledge climbing, `player.c:11835`) on surfaces 4, 5 (desk sides), 19, 20, 21 (bedroom furniture); poly exclusion: 10 ignore-camera (main door recess polys 46-53 and exit floor 60-61), 19 ignore-projectiles (drawing-room walls among polys 264-283, x 30..211, z -480..-391); no conveyor, hookshot, soft or floor-property flags; echo 2 on every type |
| Per-floor light | every surface type has light setting 31 = `LIGHT_SETTING_MAX` (`include/z64environment.h:30`), which `Environment_ChangeLightSetting` treats as "no per-floor change" (`src/code/z_kankyo.c:2169-2178`, called from `player.c:11728-11730`), so no room selects its own environment light; the rooms cannot be split by light index (unlike the Inn) |

| Room | Polys (floor / wall / ceiling) | Floor levels (centroid Y: count; furniture labels INF) | Materials (floor) | Floor bgCam | Notes |
| --- | --- | --- | --- | --- | --- |
| 00 lobby | 114 (32 / 72 / 10) | 0: 24, 15: 2 (bench-like block x 60..90, z 120..270), 40: 6 (desk top) | stone 22, wood 6, carpet 4 | 4 `ROOM1` | ceilings Y 100 (door recess) and 180; back wall z -120 full width |
| 01 office | 76 (24 / 46 / 6) | 0: 14, 18: 2 (Mayor's seat), 36: 4 (desk sides), 123-124: 4 (shelf tops on both side walls) | stone 22, carpet 2 | 0 | front wall z -180 |
| 02 drawing room | 111 (36 / 67 / 8) | 0: 22, 6-21: 11 (dais and seat slopes near Aroma, x 99..133), 11-16: 4 (sofa under Toto), 36-42: 2 | stone 21, wood 9, carpet 6 | 1 | east wall x 315; Aroma's home Y 0 sits under a Y 10 floor (poly 294) |
| 03 bedroom | 234 (62 / 164 / 8) | 0: 28, 16: 12 (bed-like platforms x 560..660), 29-40: 16, 52: 2 | stone 26, wood 26, carpet 10 | 2 and 3 | west wall x 375; En_Talk on a Y 34 surface (poly 451) |

Paths (HDR scene.c:110-183; `include/z64path.h` layout as North): 9 paths of 2, 6, 5, 2, 4, 4, 2, 9 and 2 points, all on Y 0, used only by Gorman (5.5).

## 8. Materials and textures (C; counts only, nothing copied; scratch `mats.py`)

| Item | Count |
| --- | --- |
| Distinct textures loaded by the four rooms | 49: CI8 27, RGBA16 18, I8 2, IA16 1, RGBA32 1 |
| Of these, in this scene's own files | 20: scene file 6 (rgba16 x5, ia16 x1) + room_00 2, room_01 3, room_02 3, room_03 6 |
| External (`scene_texture_01`, not extracted) | 29: CI8 26, RGBA16 2, I8 1 |
| Extracted PNGs in the scene folder | scene 9 (6 textures + 3 TLUTs); r00 2; r01 3; r02 3; r03 7 (6 textures + 1 TLUT) |
| `Gfx` arrays per room (non-empty) | r00 24 (18), r01 23 (18), r02 27 (22), r03 21 (16) |
| Raw triangle commands per room | r00 678, r01 455, r02 777, r03 752 (DL reuse not resolved) |
| Render modes | opaque surf, tex-edge and a few XLU per room (`G_RM_FOG_SHADE_A` first cycle) |
| `gsSPBranchLessZraw` LOD branches | r00 3 (z < 0x190) to DLs `0056E8`, `007470`, `0077A8` = 307 triangles whose vertices lie in the drawing-room footprint (x 4..345, z -508..-150); r02 2 (z < 0xC8) to `006718`, `0073E8` = 139 triangles in the lobby footprint (x -225..301, z -196..401); r01, r03 none |
| Animated materials | none |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_SONCHONOIE` (C above); `object_obj_tokeidai` (already extracted for South). All banks below exist under `extracted/n64-us/baserom/` (checked by `stat`): object_hariko 1,056 B, object_wdor01 5,808, gameplay_keep 599,520, scene_texture_01 19,712, object_bg 72,560, object_dt 46,352, object_toryo 29,024, object_bai 33,200, object_sdn 56,464, object_daiku 54,080, object_al 56,304, object_in2 49,232, object_zm 51,344.

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `scene_texture_01` (misc), `object_hariko`, `object_wdor01`, `gameplay_keep` | 29 external room textures; cow statues; main door; inner doors, `gDoorSkel`, flames |
| Later skeletal pass | `object_bg`, `object_dt`, `object_toryo`, `object_bai`, `object_sdn`, `object_daiku`, `object_al`, `object_in2`, `object_zm` | the nine NPCs present at noon |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
& $zapd e -i assets/xml/misc/scene_texture_01.xml -b extracted/n64-us/baserom -o extracted/n64-us/assets/misc/scene_texture_01 -osf extracted/n64-us/assets/misc/scene_texture_01 -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
foreach ($bank in @('object_hariko','object_wdor01','gameplay_keep','object_bg','object_dt','object_toryo','object_bai','object_sdn','object_daiku','object_al','object_in2','object_zm')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

The misc output path follows the scene XML's `ExternalFile OutPath="assets/misc/scene_texture_01/"` (INF; the misc extraction has not been run with this tool before). `gameplay_keep` is also needed by other packets: extract once and share.

## 10. Proposed region record

Schema 1 (PIPE-01, `shared/city/schema/region.schema.json`, including the optional `rooms[].role` and `rooms[].collisionLightIndex` from the README change list), written directly; the integrator lifts and validates it. `cameraIds` cites the six cameras registered for this venue (SOURCE_REGISTER 4.6); their source bindings are proposals (5.8). `collisionLightIndex` 31 is `LIGHT_SETTING_MAX` on every surface (section 7).

```json
{
  "schemaVersion": 1,
  "regionId": "loc.mayors_residence",
  "displayLabel": "The Mayor's Residence",
  "source": {
    "sceneSymbol": "Z2_SONCHONOIE",
    "sceneEnum": "SCENE_SONCHONOIE",
    "sceneTableIndex": "0x12",
    "entranceTableSceneIndex": "0x00",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneLayer": 0,
    "halfDayBit": "0x080",
    "stateAssumptions": [
      "fresh save: no week-event, switch or collectible flags (WEEKEVENTREG_ATTENDED_MAYOR_MEETING, WEEKEVENTREG_RESOLVED_MAYOR_MEETING, WEEKEVENTREG_86_20 clear)",
      "player form Human, no mask worn, Kafei's Mask not owned",
      "arrival by entrance 0 from East Clock Town, room_00 loaded first",
      "Day 1 12:00: Gorman on path 7 (schedule result 29), Aroma result 1, meeting unresolved"
    ]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_SONCHONOIE", "bytes": 29952, "sha256": "d5b204bf68129ba888c4f83152b7963e71702fd539b4884fae8fa2ec9063b43b" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_SONCHONOIE_room_00", "bytes": 38352, "sha256": "df594cec98f21cc151425f5812a07db4de4d0c74c09999b85c028bc140ac8999" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_SONCHONOIE_room_01", "bytes": 28544, "sha256": "5d5e3da1a45b0bab2b3f783596cd5321f28dc0757cd69a96c49613ea2bb0f92b" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_SONCHONOIE_room_02", "bytes": 41328, "sha256": "90a03d827c35688b13c747e6ca917e196f0ede10ff188d2c21e35a9c43461dff" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_SONCHONOIE_room_03", "bytes": 40304, "sha256": "74b1933d8931c742b677875498a406073332c3a2af37d70fa25a4e5f28b82d1f" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_SONCHONOIE/Z2_SONCHONOIE.c", "bytes": 72105, "sha256": "d65903442f6d2b62d721a4293942408a8f59db241ac9cc3bbd16bc282573acd6" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_SONCHONOIE/Z2_SONCHONOIE_room_00.c", "bytes": 64894, "sha256": "f47abfc183ba1d25b62a357e361de3ddbec23c67766e22dacbe5468b9ffdd138" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_SONCHONOIE/Z2_SONCHONOIE_room_01.c", "bytes": 44432, "sha256": "133fa49dc903145c4d0923dd62405d22a0b513da853360dbcaf1684cac653a91" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_SONCHONOIE/Z2_SONCHONOIE_room_02.c", "bytes": 63566, "sha256": "5d6a2a4c440ec53e6c5d2e2e41cf268e4c5d62c8bde9418cbd9e17706e7bf661" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_SONCHONOIE/Z2_SONCHONOIE_room_03.c", "bytes": 56968, "sha256": "096914f06a19a61128787bb662b2075ef21cb6e56b5bc8a8efa9db47d8cb6521" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_SONCHONOIE/Z2_SONCHONOIE.xml", "bytes": 1951, "sha256": "ad009ec42e6ca2371bada01dfa520820d76a760b17278420d4c4260b1571ab13" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": [],
      "status": "not_shared",
      "evidence": "C: interior frame near the origin (main door TA3 at (0, 0, 345)); East's door TA5 is at (1100, 200, -2020); the link is the entrance pair 0x0000 / 0xD270, not shared coordinates"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-315, 0, -495], "max": [675, 182, 480] },
    "sourceCollisionVertices": { "min": [-315, 0, -480], "max": [675, 180, 480] },
    "unityCollisionVertices": { "min": [-3.15, 0.0, -4.8], "max": [6.75, 1.8, 4.8] }
  },
  "water": [],
  "anchors": [
    { "id": "anchor.mr.spawn0_entry", "source": [3, 0, 304], "unity": [0.03, 0.0, -3.04], "status": "proposed" },
    { "id": "anchor.mr.main_door_ta3", "source": [0, 0, 345], "unity": [0.0, 0.0, -3.45], "status": "proposed" },
    { "id": "anchor.mr.office_door_ta0", "source": [-165, 0, -150], "unity": [-1.65, 0.0, 1.5], "status": "proposed" },
    { "id": "anchor.mr.drawing_room_door_ta1", "source": [165, 0, -150], "unity": [1.65, 0.0, 1.5], "status": "proposed" },
    { "id": "anchor.mr.bedroom_door_ta2", "source": [345, 0, -435], "unity": [3.45, 0.0, 4.35], "status": "proposed" },
    { "id": "anchor.mr.spawn1_office", "source": [-165, 0, -289], "unity": [-1.65, 0.0, 2.89], "status": "proposed" },
    { "id": "anchor.mr.desk_cow_statue", "source": [-65, 48, -53], "unity": [-0.65, 0.48, 0.53], "status": "proposed", "note": "on the Y 40 reception desk top (poly 90)" },
    { "id": "anchor.mr.wall_clock", "source": [0, 114, -116], "unity": [0.0, 1.14, 1.16], "status": "proposed", "note": "elevation anchor" },
    { "id": "anchor.mr.bedroom_talk_spot", "source": [452, 34, -4], "unity": [4.52, 0.34, 0.04], "status": "proposed", "note": "En_Talk on the Y 34 surface (poly 451)" }
  ],
  "rooms": [
    { "id": "loc.mayors_residence.room_00", "sourceRoom": "Z2_SONCHONOIE_room_00", "header": 0, "roomShape": "cullable, 6 entries", "displayLists": 24, "role": "lobby and reception (arrival room)", "collisionLightIndex": 31 },
    { "id": "loc.mayors_residence.room_01", "sourceRoom": "Z2_SONCHONOIE_room_01", "header": 0, "roomShape": "cullable, 8 entries", "displayLists": 23, "role": "Mayor's office, meeting room", "collisionLightIndex": 31 },
    { "id": "loc.mayors_residence.room_02", "sourceRoom": "Z2_SONCHONOIE_room_02", "header": 0, "roomShape": "cullable, 9 entries", "displayLists": 27, "role": "Madame Aroma's drawing room", "collisionLightIndex": 31 },
    { "id": "loc.mayors_residence.room_03", "sourceRoom": "Z2_SONCHONOIE_room_03", "header": 0, "roomShape": "cullable, 7 entries", "displayLists": 21, "role": "bedroom (occupant not stated in source)", "collisionLightIndex": 31 }
  ],
  "components": [
    { "sourceKey": "Z2_SONCHONOIE/h0/transition/ta3", "actorId": "0x005", "params": "0x0291", "class": "door-transition", "variant": "posed-skeletal", "dls": ["object_wdor01:0x548 gLotteryCuriosityShopMayorHouseDoorDL", "gameplay_keep:0x22B28 gDoorSkel"], "status": "selected", "note": "main door, closed rest pose approximated; exit floor polys 60/61 behind it" },
    { "sourceKey": "Z2_SONCHONOIE/h0/ta0-ta2", "actorId": "0x005", "params": "0x0292,0x0293,0x0294", "class": "door-transition", "variant": "posed-skeletal", "dls": ["gameplay_keep:0x20BB8 gDoorLeftDL", "gameplay_keep:0x20D00 gDoorRightDL", "gameplay_keep:0x22B28 gDoorSkel"], "status": "selected", "note": "inner room-swap doors, always open" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_00/a04", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80 gHarikoBodyDL", "object_hariko:0x110 gHarikoFaceDL"], "status": "selected", "note": "cow statue on the reception desk, scale 0.1, yaw 19" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_00/a08", "actorId": "0x19C", "params": "0xA07F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518 gWallClockClockFaceDL", "object_obj_tokeidai:0xC368"], "status": "selected", "note": "small wall clock (type 10), scale 0.01, hands at 12:00" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_01/a03", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80 gHarikoBodyDL", "object_hariko:0x110 gHarikoFaceDL"], "status": "selected", "note": "cow statue in the office, scale 0.1, yaw 302" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_02/a04", "actorId": "0x004", "params": "0x07F4", "class": "billboard-effect", "dls": ["gameplay_keep:0x7D590 gEffFire1DL"], "status": "optional", "note": "fireplace flame, scale 0.004, point light r300, Days 0-2 mask" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_03/a02", "actorId": "0x004", "params": "0x07F4", "class": "billboard-effect", "dls": ["gameplay_keep:0x7D590 gEffFire1DL"], "status": "optional", "note": "fireplace flame, scale 0.004, point light r300, Days 0-2 mask" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_00/a06", "actorId": "0x290", "params": "0x0005", "class": "npc-scheduled", "dls": ["object_bg:0x11B60 object_bg_Skel_011B60"], "status": "optional", "note": "receptionist, present Days 0-2; skeletal pose deferred" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_00/a01", "actorId": "0x0A4", "params": "0x0000", "class": "npc-scheduled", "dls": ["object_in2:0x78B0 object_in2_Skel_0078B0"], "status": "optional", "note": "Gorman, Day 1 10:00-12:20 only; walking path 7 at 12:00; singleton" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_01/a01", "actorId": "0x26F", "params": "0x0000", "class": "npc-scheduled", "dls": ["object_dt:0xB0CC object_dt_Skel_00B0CC"], "status": "optional", "note": "Mayor Dotour, all half-days; distress loop before the meeting" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_01/a05", "actorId": "0x26B", "params": "0x0001", "class": "npc-scheduled", "dls": ["object_toryo:0x7150 object_toryo_Skel_007150"], "status": "optional", "note": "Mutoh, until RESOLVED_MAYOR_MEETING or Day 3 night" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_01/a06", "actorId": "0x26D", "params": "0x0001", "class": "npc-scheduled", "dls": ["object_sdn:0xD640 gSoldierSkel"], "status": "optional", "note": "soldier, same gate as Mutoh" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_01/a07", "actorId": "0x26C", "params": "0x0001", "class": "npc-scheduled", "dls": ["object_bai:0x7908 object_bai_Skel_007908"], "status": "optional", "note": "Viscen, same gate as Mutoh" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_01/a08", "actorId": "0x09C", "params": "0x0000", "class": "npc-scheduled", "dls": ["object_daiku:0xA850 object_daiku_Skel_00A850"], "status": "optional", "note": "carpenter, same gate as Mutoh" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_02/a00", "actorId": "0x262", "params": "0x0000", "class": "npc-scheduled", "dls": ["object_al:0xA0D8 gMadameAromaSkel"], "status": "optional", "note": "Madame Aroma, Days 1-2 08:00-20:00; schedule result 1 at 12:00" },
    { "sourceKey": "Z2_SONCHONOIE/h0/room_02/a02", "actorId": "0x234", "params": "0x050B", "class": "npc-scheduled", "dls": ["object_zm:0xA978 gTotoSkel"], "status": "optional", "note": "Toto seated, Day 1 and Day 2 daytime mask" }
  ],
  "collisionOwnership": {
    "room": "Z2_SONCHONOIECollisionHeader_0036D4 (495 vertices, 535 polys, 22 surface types); one scene-level header covers rooms 00-03",
    "waterBoxes": 0,
    "actorDynapoly": [],
    "actorCylinders": [
      "room_00/a01 En_Gm r14 h62 plus sphere",
      "room_01/a01 En_Dt r25 h70",
      "room_01/a05 En_Muto r30 h60",
      "room_01/a06 En_Heishi r30 h60",
      "room_01/a07 En_Baisen r30 h60",
      "room_01/a08 En_Daiku r30 h60",
      "room_02/a00 En_Al r14 h62",
      "room_02/a02 En_Toto r20 h60"
    ],
    "note": "En_Recepgirl and Obj_Hariko have no collider; the four doors load no collision (not framed)"
  },
  "portalIds": [
    "portal.mayors_residence.east_clock_town",
    "portal.mayors_residence.east_clock_town.kickout",
    "portal.mayors_residence.mayors_residence.r0_r1",
    "portal.mayors_residence.mayors_residence.r1_r0",
    "portal.mayors_residence.mayors_residence.r0_r2",
    "portal.mayors_residence.mayors_residence.r2_r0",
    "portal.mayors_residence.mayors_residence.r2_r3",
    "portal.mayors_residence.mayors_residence.r3_r2"
  ],
  "materialBindings": null,
  "cameraIds": [
    "cam.mayors_residence.entry",
    "cam.mayors_residence.reverse_door",
    "cam.mayors_residence.fixture",
    "cam.mayors_residence.room_01",
    "cam.mayors_residence.room_02",
    "cam.mayors_residence.room_03"
  ],
  "routeIds": [],
  "omissions": [
    "skeletal NPC poses and animation (9 NPCs present at noon)",
    "flame point lights and texture scroll",
    "Link and Tatl",
    "LOD-branch neighbour-room copies (room_00 3 branches, room_02 2)",
    "door opening animation"
  ],
  "excluded": {
    "halfDayMask": ["room_00/a00 En_Light night candle", "room_01/a00 En_Light night candle"],
    "stateOrSchedule": ["room_00/a02 En_Heishi p0 (until RESOLVED_MAYOR_MEETING or Day 3 night)", "room_00/a05 En_Baisen p0 (same gate)", "room_02/a01 En_Gm duplicate (singleton kill)"],
    "noGeometry": ["room_00/a03 En_Time_Tag", "room_00/a07 En_Test4", "room_01/a02 En_Time_Tag", "room_01/a04 En_Test4", "room_02/a03 En_Time_Tag", "room_02/a05 En_Test4", "room_03/a00 En_Talk", "room_03/a01 En_Time_Tag", "room_03/a03 En_Test4"]
  },
  "unsupported": [
    "schedule-driven NPC movement and NPC door opening (Gorman)",
    "door hours and the 20:00 kickout",
    "meeting and Couple's Mask cutscenes and cameras",
    "gsSPBranchLessZraw LOD branches",
    "wall type 1 no-ledge-climb flag",
    "billboard flames",
    "scene_texture_01 external textures (not extracted)"
  ],
  "evidence": ["docs/city/interiors/mayors_residence.md"],
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
    "sourceInspection": "4 rooms, 28 room actors, 4 doors, collision, schedules and story flags indexed; no capture",
    "objectExtraction": "only object_obj_tokeidai exists locally; section 9 lists the rest"
  }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Why room_00 carries 307 LOD-branched triangles in the drawing-room footprint and room_02 carries 139 in the lobby footprint | Decode `gsSPBranchLessZraw` (`include/PR/gbi.h`) and its reference vertices (r00 `Vtx_007190` at (260, 0, 240); r02 `Vtx_006ED8` at (165, 0, -370)); then render each room alone from spawn 0 and from inside the drawing room to see whether the copy is visible through a window or only during a door transition |
| U-02 | Exact text of the IDs cited (0x2AD9.., 0x2ABB.., 0x2A9C.., 0x1889, 0x1C04) | Messages are not extracted locally; extract the message table (`assets/text`) and read the IDs. Text stays out of tracked files |
| U-03 | NPC static poses for an assembly (seated Mayor at home Y 0 over a Y 18 seat; Aroma at home Y 0 under a Y 10 dais; Toto Y 11) | For each skeleton, take frame 0 of the init animation (`gDotourWaitAnim` then distress, `ENAL_ANIM_1`/`ENAL_ANIM_0`, `gTotoIdleSittingAnim`) after the section 9 extraction; check root-limb translation against the floors |
| U-04 | Whether En_Test4 in the room loaded second is killed or kept | Trace `sIsLoaded` reset in `EnTest4_Destroy` and room-change actor kills (`src/code/z_actor.c` room unload path) |
| U-05 | Whose bedroom room_03 is | No overlay or text names it; read message 0x1C04 (U-02) and any cutscene that enters room 3 |
| U-06 | Actor cutscene entries 0-8 (chained, negative csCam ids) | Search overlays for `CutsceneManager` calls with literal ids 0-8 in this scene, and decode negative `csCamId` values (`include/z64cutscene.h` global cameras) |
| U-07 | `MSCRIPT_CMD_SET_COLLECTIBLE(0x008F)` after the Kafei's Mask offer | Read the message-script command handler (`src/code/z_msgevent.c`) for SET_COLLECTIBLE semantics |
| U-08 | Object reload on room swaps (does OBJECT_BG stay loaded when leaving the lobby) | Read `Scene_CommandObjectList` / `Object_LoadAll` in `src/code/z_scene.c` for slot reuse when the next room's list differs |
| U-09 | Gorman's exact position at 12:00 on path 7 | Evaluate `SubS_TimePathing_*` (`src/code/z_sub_s.c`) with `timePathElapsedTime = 0` and 9 points; record the point and yaw for a fixed noon pose |
| U-10 | Visual survey | Capture the six registered cameras (5.8) at `prof.d1.noon.normal`; add `prof.d1.night.normal` for the candles and `prof.d3.night.normal` for the lobby Viscen and the final-night Mayor |

**Not inspected:** room DL contents beyond counts and the LOD branches; light-setting values; minimap rendering; En_Al, En_Gm and En_Toto behavior beyond schedule, init, draw and message-script selection; En_Daiku dialogue text table; the Milk Bar side of the Letter to Mama exchange; `SubS` time-pathing math; door skeleton animation; 3DS material.

**Checks run:** read the brief and listed docs (HANDOFF, FULL_CITY_PLAN sections 6, 11, 12, LOCATION/PORTAL/SOURCE registers, North and East dossiers, schema README, region schema, `laundry_pool.json`, `south_clock_town.json`, `ids.json` portal rows); read scene/room C, XML, scene/actor/object/sequence tables, `z_actor.c`, `z_scene.c`, `z_play.c`, `z_scene_table.c`, `z_schedule.c`, `z64scripts.h`, `z64bgcheck.h`, `z_bgcheck.c`, `z_camera.c`, `z64player.h`, the player door code and the 16 overlays above; scratch Python (`python -I <file>`): actor decode (28 entries, line numbers checked against grep), collision parse (495/535/22), room partition (114/76/111/234, no boundary polygons), door and actor floor probes, wall planes, material counts, vertex-buffer extents; SHA-256 and sizes of the 11 source inputs; grep for `ENTRANCE(MAYORS_RESIDENCE`, exit values `0x0010`/`0x0020` across all 22 exit lists, `setupExitList` readers among the placed overlays, and other placements of these NPCs (`ACTOR_EN_AL`, `EN_GM`, `EN_TOTO`, `EN_MUTO`); ASCII check of this file; JSON parse of section 10. Counts agree with `docs/city/LOCATION_REGISTER.md` row `loc.mayors_residence` and with PORTAL_REGISTER rows 200, 229-230, 266-271.

**NOT run:** ZAPD (no object or misc extraction), Blender, Unity, any player or executable, captures, renders, controller routes, `region_from_dossier.py`, `validate_city.py`.

**Next dependency:** integrator lifts section 10 with `region_from_dossier.py` and runs `validate_city.py`; PORTAL_REGISTER rows for this venue can take the door hours from 2.5; section 9 extraction (shared `gameplay_keep` and `scene_texture_01`) before ROOM-0x assembly; U-02 message extraction before any dialogue work.
