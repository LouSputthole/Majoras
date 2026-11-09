# Town Shooting Gallery (`loc.town_shooting_gallery`): source inventory dossier

Task **REF-09 / INT-12** (FULL_CITY_PLAN.md section 12 row REF-09, section 6 row INT-12). Base commit `c8b847e` on `main` (HEAD moved to `7ab0640` while this packet ran: registers batch edits `48a683e`, the message-text tool `560b88d`, INT-17; register cites below were re-read at `7ab0640`). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_SYATEKI_MIZU` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`, `shared/` or `tools/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_SYATEKI_MIZU/Z2_SYATEKI_MIZU.c`, `room.c` = `.../Z2_SYATEKI_MIZU_room_00.c`; `man.c` = `src/overlays/actors/ovl_En_Syateki_Man/z_en_syateki_man.c`, `okuta.c` = `src/overlays/actors/ovl_En_Syateki_Okuta/z_en_syateki_okuta.c`, `tag.c` = `src/overlays/actors/ovl_En_Time_Tag/z_en_time_tag.c`, `door.c` = `src/overlays/actors/ovl_En_Door/z_en_door.c`, `toke.c` = `src/overlays/actors/ovl_Obj_Tokeidai/z_obj_tokeidai.c`, `player.c` = `src/overlays/actors/ovl_player_actor/z_player.c`, `param.c` = `src/code/z_parameter.c`, `sram.c` = `src/code/z_sram_NES.c`. Both extracted files have no CR bytes, so line numbers are plain. Polygon `i` is on scene.c line `116 + i`; vertex `i` on line `311 + i`; actor entry `i` on room.c lines `31 + 3i` to `33 + 3i`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. The interior uses its own frame; it is not placed in the town frame.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro/function; **OVL** read from the actor overlay source (including `.schl` schedule scripts); **ANN** decomp-author comment or symbol name; **INF** inferred (reason given); **NI** not inspected. Message ids are cited, text is never copied; the short meanings given for ids are the decomp authors' comments in the overlays (ANN), not message data; header fields (prices, control codes) come from `local-reference/text/messages_nes.json` (tool `tools/reference/extract_messages.py`, read through my own scratch `msgs.py`).

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01), `docs/city/PORTAL_REGISTER.md` (REF-02), `docs/city/SOURCE_REGISTER.md` (REF-03), `docs/city/districts/east_clock_town.md` section 2.4 (street door TA#3). Every value those files state for this venue agrees with the C below; the open items they leave (trailing exit, entrance 1, open hours detail) are settled in 2.2, 2.5 and 11.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_SYATEKI_MIZU` / `SCENE_SYATEKI_MIZU`; comment "Town Shooting Gallery" | HDR `include/tables/scene_table.h:71`; ANN :70 |
| Scene-table index | `0x20` | same line |
| Entrance-table scene index | `0x1D` `sTownShootingGalleryEntranceTable` (2 slots): `Entrance0 = { SCENE_SYATEKI_MIZU, 0, 0x4102 }`, `Entrance1 = { SCENE_SYATEKI_MIZU, 1, 0x0102 }` | HDR `src/code/z_scene_table.c:2602`, :545-556 |
| Title text / draw config / cycle flags | `0x107` / `SCENE_DRAW_CFG_MAT_ANIM` / `PERSISTENT_CYCLE_FLAGS_NONE` | HDR scene_table.h:71 |
| Restrictions | `RESTRICTIONS_INDOORS` = `RESTRICTIONS_SET(0,1,0,0,0,3,0,0,3,0,0,1)`; field order hGauge, bButton, aButton, tradeItems, songOfTime, songOfDoubleTime, invSongOfTime, songOfSoaring, songOfStorms, masks, pictoBox, all | HDR; C `param.c:66-75` |
| Restriction finding | `bButton` 1 takes the `!= 0` branch: the generic path sets the B (sword) button to `BTN_DISABLED` (INF: the bow minigame uses the B-ammo path, 5.4); songOfDoubleTime and songOfStorms 3 (semantics NI) | C `param.c:2021-2039` |
| XML | scene `Z2_SYATEKI_MIZU` segment 2 (`Offset 0x0`); room `Z2_SYATEKI_MIZU_room_00` segment 3 plus DLs `0x5130` and `0x6128`; no external-file dependency | XML `assets/xml/scenes/Z2_SYATEKI_MIZU/Z2_SYATEKI_MIZU.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene / room headers | default only: no alternate-header command in scene.c:13-29 or room.c:13-21 | HDR |
| Special files | `NAVI_QUEST_HINTS_NONE`, keep `GAMEPLAY_FIELD_KEEP` (0x002); no actor of this scene uses a field-keep object (INF) | HDR scene.c:20 |
| Sound | spec 5, ambience 19, seq 0 = `Sequence_0` `NA_BGM_GENERAL_SFX` (no scene tune); the gallery tune comes from actor a14 (5.9) | HDR scene.c:14; `include/tables/sequence_table.h:20`; macro `include/z64scene.h:845-846` |
| Skybox | `SKYBOX_SETTINGS(0x00, 0, 0, true)`; room `SKYBOX_DISABLES(true, true)` | HDR scene.c:22, room.c:16 |
| Environment light | 4 rows, all different (table below); every collision surface selects row 0 (section 7), so rows 1-3 are not reached through the floor (how else they are selected: NI, U-08) | HDR scene.c:54-63; C `player.c:6603` |
| Region visited | `SCENE_CMD_SET_REGION_VISITED`; `SCENE_SYATEKI_MIZU` is in the `REGION_CLOCK_TOWN` list | HDR scene.c:17; C `src/code/z_inventory.c:428-436` |
| Paths | none (no `SCENE_CMD_PATH_LIST`); the keeper's `params` path index 0xFF is read but unused here (5.1) | HDR scene.c:13-29; OVL man.c:207, :239 |
| Actor cutscenes / cs cameras | 9 entries / none (no cutscene-camera command); all nine are the player's global-camera chain (2.1) | HDR scene.c:25, :65-75 |
| Minimap | 1 entry `{0xFFFF, 0, 0, 0, 0}`, list scale 0 | HDR scene.c:77-83 |
| Animated materials | one entry `{ -1, 1, params }`: segment `abs(-1) + 7 = 8`, type 1 = two-layer texture scroll, params `{-1, 0, 32, 32}` twice (x step -1 per game frame, no y; both layers identical) | HDR scene.c:89-96; C `src/code/z_scene_proc.c:388-408`, :85-89 |
| Transition actors | 1 (`EN_DOOR`) | HDR scene.c:16, :36-38 |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x02, 0x06, 0, 0, false, 0)`; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3); cullable shape | HDR room.c:14-18 |
| Room type 2 | `ROOM_TYPE_INDOORS`: reduces player run speed and blocks A-button attack and jump | C `include/z64scene.h:317`; `src/code/z_player_lib.c:634`; player.c:8245, :11573-11577; `src/code/z_camera.c:6995` |
| Room environment 6 | `ROOM_ENV_UNK_STRETCH_3`; consumed only as the player's idle-fidget selector | C `z64scene.h:330`, `src/code/z_scene.c:246-252`; player.c:10059, :10077 |
| Room shape | `RoomShapeCullable`, 19 entries: 18 opaque DLs, 3 XLU DLs (entries 13 and 14 carry both, entry 17 XLU only) | HDR room.c:82-109 |

Environment light rows (`EnvLightSettings`, field order `include/z64environment.h:205-215`; blend rate `(w >> 10) * 4`, fog near `w & 0x3FF`; HDR scene.c:54-63):

| Row | Ambient RGB | Light 1 dir / RGB | Light 2 dir / RGB | Fog RGB | blend / fog near / zFar |
| --- | --- | --- | --- | --- | --- |
| 0 | 100, 60, 60 | (44, 111, -6) / 240, 240, 180 | (-68, -98, 1) / 100, 110, 220 | 60, 60, 70 | 4 / 974 / 12800 |
| 1 | 105, 90, 90 | (69, 69, 69) / 255, 255, 240 | (-69, -69, -69) / 50, 50, 90 | 100, 100, 120 | 4 / 996 / 12800 |
| 2 | 120, 90, 0 | (69, 69, 69) / 250, 135, 50 | (-69, -69, -69) / 30, 30, 60 | 28, 20, 0 | 4 / 993 / 12800 |
| 3 | 40, 40, 70 | (69, 69, 69) / 20, 20, 35 | (-69, -69, -69) / 50, 50, 100 | 0, 0, 30 | 4 / 992 / 12800 |

### 1.1 Alternate headers

None. Every profile selects the default scene and room header. The entrance table has one layer-0 row per slot (z_scene_table.c:545-551), so a nonzero `sceneLayer` is not expected here (INF). Day and time variation is per-actor (half-day masks, section 4) and overlay logic (section 5).

## 2. Spawns, entrances and exits

### 2.1 Start position (HDR scene.c:31-34; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`); yaw `(rawRotY >> 7) & 0x1FF`; csId `rawRotY & 0x7F`.

| Spawn | Position | Raw rot X / Y / Z | Yaw | csId | Params | Start mode | bgCam | Used by |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-10, 0, 390) | 0x0007 / 0x5A00 / 0x007F | 180 (faces -Z, into the room) | 0 | 0x0EFF | 0xE `PLAYER_START_MODE_E` (`z64player.h:35`) | 0xFF | entrance 0 = `0x3A00` = East exit [8] (`portal.east_clock_town.town_shooting_gallery`); 30 u inside the door leaf (door z 420), 10 u inside the wall plane z 400 |

- **bgCam 0xFF** (C): `z_play.c:2326-2328` calls `Camera_ChangeActorCsCamIndex` only when the index is not 0xFF, so arrival keeps the floor camera: the floor under the spawn is poly 33 (surface 3, bgCam 0 = `CAM_SET_CIRCLE3`, section 5.9). Unlike the school (bgCam 0, an actor cs camera), no start camera is selected.
- **csId 0** (C): `Play_AssignPlayerCsIdsFromScene(play, 0)` (`player.c:11261`; `src/code/z_play.c:2016-2059`) walks `additionalCsId` 0 -> 1 -> ... -> 8 through the nine scene cutscene entries (scene.c:65-75). Their `csCamId` values are -3, -2, -4, -5, -7, -11, -8, -9, -6 = the global cameras ITEM_OCARINA (priority 700, lowered to 550 by `z_play.c:2051-2053`), ITEM_GET, ITEM_BOTTLE, ITEM_SHOW, WARP_PAD_MOON, MASK_TRANSFORMATION, DEATH, REVIVE, SONG_WARP (`include/z64cutscene.h:670-695`, struct :626-637). No actor of this room carries a cutscene id (all `csId` = none, section 4), so these entries only give Link his item-get, bottle, show, mask, death and revive cameras.
- **Entrance list** (HDR scene.c:44-47): two rows `{0x00, 0x00}`: entrance slots 0 and 1 both resolve to spawn 0, room 0 (`Entrance_GetTableEntry`, `z_scene_table.c:2688-2694`; spawn `z_scene.c:160`, room `z_room.c:549`). The list occupies 0xA0-0xA3 and the exit list starts at 0xA4 (symbol suffixes).
- **Entrance 1 (`0x3A10`) is not a second spawn.** `sTownShootingGalleryEntrance1` = `{ SCENE_SYATEKI_MIZU, 1, 0x0102 }` loads the same spawn 0 / room 0. Its flags differ from entrance 0 (`0x4102`) only in bit 14: exit fade `0x02`, arrival `(0x0102 >> 7) & 0x7F = 0x02` and bit 15 are identical (C `z_scene.c:540`, `z_play.c:583`, :2275); bit 14 (`0x4000`) gates the scene title card (`Message_DisplaySceneTitleCard(play, titleTextId 0x107)` when `scene->titleTextId != 0` and `showTitleCard`, C `src/code/z_demo.c:1580-1590`). So entrance 0 arrives with the "Town Shooting Gallery" title card, entrance 1 arrives silently. No sender exists for entrance 1: the only `ENTRANCE(TOWN_SHOOTING_GALLERY, n)` in `src/` is the debug map select with n = 0 (`src/overlays/gamestates/ovl_select/z_select.c:303`; ANN "50-0: Town Target Range"), and no exit list in the 22 extracted city scenes holds `0x3A10` (grep over `extracted/n64-us/assets/scenes`: no hit; non-city scenes are not extracted, so INF). PORTAL_REGISTER section 6 correctly drops "Shooting Gallery 1" as a spawn; the accurate statement is "entrance row 1 lands on spawn 0 without the title card".

### 2.2 Exit list (HDR scene.c:49-52) and consumers

| Exit idx | Raw | Decode (`>> 9`, `(>> 4) & 0x1F`, `& 0xF`) | Destination | Consumer in the gallery |
| --- | --- | --- | --- | --- |
| 0 | `0xD280` | 0x69 `Z2_TOWN`, entrance 8, layer 0 | `sEastClockTownEntrance8 = { SCENE_TOWN, 8, 0x4102 }` (z_scene_table.c:2233) -> `Z2_TOWNEntranceList0x0001D8[8] = {0x08, 0x00}` (Z2_TOWN.c:90) -> East spawn 8 (1095, 100, 257) yaw 235 (raw 0xEB), params 0x0F09 = start mode 0xF, bgCam 9, room 0 (Z2_TOWN.c:57-58) | surface 4, polys 36, 37 (door probe, 2.3); both `En_Time_Tag` kickouts (`params & 0x1F = 0`, section 4.3) |
| 1 | `0x0000` | would decode to Mayor's Residence entrance 0 | none | **padding** (C, closes LOCATION_REGISTER U-02 for this scene): no surface carries exit+1 = 2 (only surface 4 has exit bits), no `En_Time_Tag` index 1 (both read 0), no `Door_Ana`, `En_Warp_tag` or `En_Holl` exists in the room (actor list, section 4), and the list occupies 0xA4-0xA7 with the next block (`LightSettings`) at 0xA8, so the second u16 (0xA6) is consistent with alignment padding (INF) |

East's exit [8] = `0x3A00` in the default and alt1 lists (`Z2_TOWN.c:105`) is the entrance into this scene (entrance 0); East alt2's list holds it at index 7 (East dossier 2.2).

### 2.3 Transition actor and door decode (HDR scene.c:36-38; C)

Raw entry: `{ 0, 255, 0, 255, ACTOR_EN_DOOR, -10, 0, 420, 127, 0x0287 }` = front room 0 / bgCam 255, back room 0 / bgCam 255, position (-10, 0, 420), rotY 127 (0x007F), params 0x0287 (struct `include/z64scene.h:369-378`).

| Field | Value | Rule |
| --- | --- | --- |
| Yaw | `(127 >> 7) & 0x1FF = 0`: **yaw 0, faces +Z** | `src/code/z_actor.c:3613` |
| csId | `rotY & 0x7F = 0x7F` -> none; spawned with `HALFDAYBIT_ALL` | z_actor.c:3615-3619 |
| Spawned params | `TRANSITION_ACTOR_PARAMS(0, 0x287) = 0x0287` | `include/z64door.h:22-25` |
| Type / type var | `(p >> 7) & 7 = 5` `ENDOOR_TYPE_SCHEDULE`; `p & 0x7F = 7` `ENDOOR_SCH_TYPE_TOWN_SHOOTING_GALLERY` | OVL `ovl_En_Door/z_en_door.h:44-51`, :62, :95-96 |
| Sides | both room 0, bgCam -1 (generic door camera) | z64scene.h:371-372 |
| Leaf art | default pair `gDoorLeftDL` (gameplay_keep 0x20BB8) / `gDoorRightDL` (0x20D00) on `gDoorSkel` limb 4; same room on both sides, so the DL is chosen by camera yaw | OVL door.c:219 (`DOOR_OBJINFO_24`), :306-307, :638-657; XML `gameplay_keep.xml:931-932`, :941 |
| Leaving | the inside player is on the door's local -Z side, so `doorDirection = -1` (door.c:496); the player is placed 22 u in front: (-10, ?, 398); the probe is 22 u behind at (-10, 10, 442) (player.c:6756-6757, :6779-6781) -> floor poly 37 (surface 4, exit+1 = 1) -> exit 0 `0xD280` (player.c:6783, :6405-6416) | C; probe polygon from scratch `geom.py` |
| Schedule from inside | the schedule script runs only when the player is on local +Z (`playerPosRelToDoor.z > 0`, door.c:537), so **from inside the door always opens** | OVL |
| Geometry around the door | the wall plane z 400 has a doorway gap x -40..20, Y 0..100 (alcove walls polys 2-5 at x 20 and x -40, ceiling polys 0, 1 at Y 100, all ignore-camera); the leaf stands 20 u inside the alcove at z 420; floors 30, 31 (z 400..420, surface 2) are walkable up to the leaf, exit floors 36, 37 (z 420..486, surface 4) lie behind it and are reached only by the probe (INF that the player never stands there) | C (geometry) |

### 2.4 Collision surfaces carrying exits and door-to-venue cross-check

| Surface | data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Centroids | Normal | Flags | Material |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 4 | 0x24000100 / 0x00001000 | 0 (`0xD280`) | 36, 37 | -40..20 / 0 / 420..486 | (-20, 0, 464), (0, 0, 442) | +Y | xp 1 (ignore camera); floorProperty 9; no conveyor | dirt (0) |

No wall or ceiling has exit bits; nothing else in the scene is an exit.

| Door | Scene | Position / yaw | Params -> schedule | Probe -> poly -> exit | Arrival |
| --- | --- | --- | --- | --- | --- |
| East TA#3 | `Z2_TOWN` | (1114, 100, 274) / 225 (raw 28927) | 0x0287 -> SCHEDULE / 7 `TOWN_SHOOTING_GALLERY` | from the street: (1129.6, 110, 289.6) -> poly 480, surface 19, exit+1 = 9 -> slot 8 `0x3A00` | gallery spawn 0 (-10, 0, 390) yaw 180 |
| Gallery TA0 (this) | `Z2_SYATEKI_MIZU` | (-10, 0, 420) / 0 | 0x0287 -> same | from the room: (-10, 10, 442) -> poly 37, surface 4, exit+1 = 1 -> exit 0 `0xD280` | East start 8 (1095, 100, 257) yaw 235 |

Agreement (C): identical params 0x0287, identical default leaf pair, East `Z2_TOWN.c:71` raw row `{ ..., 1114, 100, 274, 28927, 0x0287 }` read directly. The frames are unrelated (door at (-10, 0, 420) here versus (1114, 100, 274) in East; East's return spawn is 25 u (XZ) from the East door leaf, the gallery's arrival is 30 u from its door leaf), so `sourceFrame` is `not_shared`. PORTAL_REGISTER rows 201, 237, 238 agree on every field I re-derived: surface 4 (exit+1 = 1), polys 36 and 37, arrival start 8 mode 0xF, the "33u" (= distance from the door to the mean of the two polygon centroids, (-10, 453)), kickout rows at room_00:31 and :34.

### 2.5 Open hours and kickout (settles the remaining "hours" cells of PORTAL_REGISTER for this venue)

| Rule | Value | Evidence |
| --- | --- | --- |
| Street door schedule | `sDoorSch_TownShootingGallery { if_since_time (22, 0) { return_s (0xF) } else { return_none } }`: closed with text `0x1800 + 0xF = 0x180F` from 22:00 until the 06:00-shifted clock wraps (just before 06:00: `SCRIPT_CONVERT_TIME` subtracts `0x10000 / 360 * 90 = 0x3FFC`, so the wrap is 4 clock units, about 5 game seconds, early); no day or flag test | OVL `ovl_En_Door/scheduleScripts.schl:165-171`; C `src/code/z_schedule.c:188-203` (`if_since_time` is the non-inverted CHECK_BEFORE_TIME, `tools/buildtools/schc.py:165-168`), `include/z64scripts.h:6-14` |
| Day 1 12:00 | not since 22:00 -> none -> **open** | C |
| Kickout a00 | `En_Time_Tag` type 0 `KICKOUT_DOOR`, text `0x1883 + 12 = 0x188F` (ANN `TIMETAG_KICKOUT_TEXT_TOWN_SHOOTING_GALLERY_1`, "it's time to close"), exit index 0, hour 22 minute 0 (home rot X 22, Y 0 integers); mask 0x3F0 = D0d, D0n, D1d, D1n, D2d, D2n | OVL `ovl_En_Time_Tag/z_en_time_tag.h:10-15`, :42-60; tag.c:315-339; C mask |
| Kickout a01 | same, text `0x1883 + 13 = 0x1890` (ANN `..._TOWN_SHOOTING_GALLERY_2`, "quick, it's closing time"); mask 0x00F = D3d, D3n, D4d, D4n | same |
| Kickout action | when `hour == 22 && minute == 0` and `!Play_InCsMode`: freeze Link, start the text, then `nextEntrance = setupExitList[0]`, transition trigger, door-open sound | OVL tag.c:333-338, :285-294 |
| Deferral while a game is paid for | paying sets `WEEKEVENTREG_KICKOUT_WAIT` (63/0x01) and clears `KICKOUT_TIME_PASSED` (63/0x02); at 22:00 the tag only sets TIME_PASSED and waits until both flags are clear, then kicks out; the keeper clears both when a talk ends and `Destroy` clears WAIT | OVL man.c:688-689, :775-776, :795-800, :809-814, :865-873, :984-992, :245-247; tag.c:300-306, :327-332; `z64save.h:1227-1229` |
| Result | enterable 06:00-21:59 every day; inside, kicked to East start 8 at 22:00 (after the current game's talk if one is in progress); door closed 22:00-05:59 | C |

Venue profiles: `prof.d1.noon.normal`, `prof.d1.dusk.normal` and `prof.d1.night.normal` (21:00) are open; `prof.d1.late.normal` (23:00) is closed from the street (the interior can only be assembled directly). The keeper exists D1d-D3n (mask 0x0FC), so Day 4 (after the moon crash) has no keeper.

### 2.6 Portal IDs (registered in `docs/city/PORTAL_REGISTER.md` and `shared/city/ids.json`; no new IDs)

| Portal ID | Mode | Source evidence | Arrival |
| --- | --- | --- | --- |
| `portal.town_shooting_gallery.east_clock_town` | door | TA0 + surface 4, exit 0 `0xD280` | East start 8 |
| `portal.town_shooting_gallery.east_clock_town.kickout` | special | `En_Time_Tag` a00 (0x0180) and a01 (0x01A0), exit 0 | East start 8 |
| `portal.east_clock_town.town_shooting_gallery` (inbound, owned by East) | door | East TA#3 + surface 19, exit [8] `0x3A00` | gallery spawn 0 |

One room: no internal room portal exists or is needed.

## 3. Object banks (HDR room.c:24-28; IDs from `include/tables/object_table.h`)

| # | Bank | Object ID | Consumer (profile `objectId`, OVL) | Needed for Day 1 static list |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_SHN` | 0x1AC (:445) | `En_Syateki_Man` a02 (man.c:94); head limb `gTownShootingGalleryManHeadDL` | posed keeper (later) |
| 1 | `OBJECT_OKUTA` | 0x005 (:22) | nine `En_Syateki_Okuta` a03-a11 (okuta.c:33) | no (hidden at rest) |
| 2 | `OBJECT_OBJ_TOKEIDAI` | 0x18C (:413) | `Obj_Tokeidai` a13 (wall clock) | yes (already extracted) |

Implicit (not in the list): `gameplay_keep` 0x001 (door skeleton and leaf DLs; profiles of `En_Time_Tag`, `Obj_Sound`, `En_Test4`), `gameplay_field_keep` 0x002 (scene special file, no consumer). Overlay asset data: `assets/xml/overlays/ovl_En_Syateki_Okuta.xml` (blue-material DL at 0x1640, cross and circle hit marks at 0x1A98 and 0x1B18, textures at 0x1658 and 0x1858), read by `okuta.c:564-587`. Every listed bank has a consumer; no unexplained bank (C).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): ID `id & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF` degrees, unless the ID carries `0x8000` (rotY), `0x4000` (rotX) or `0x2000` (rotZ), in which case that axis is a plain integer (>180 minus 360). csId `rawRotY & 0x7F` (0x7F -> none, :3570-3574); half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all (:3576-3580).
- `Actor_SpawnSetupActors` (2546-2577) spawns an entry only if its mask contains the current half-day bit; `Actor_InitHalfDaysBit` (2491-2499) gives `0x080` for Day 1 12:00. Bits (`include/z64actor.h:328-341`, SOURCE_REGISTER 2.4): D0d 0x200, D0n 0x100, D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001. No entry here has ID bit 0x800.
- Flagged ID here: `En_Time_Tag | 0xE000` (all three axes integers: X is the kickout hour 22, Y the minute 0).
- Positions are the raw s16 entries; `SPAWN_ROT_FLAGS(rot, flags) = (rot << 7) | flags` (`z64scene.h:16`).

Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled`, `billboard-effect`, `controller-invisible`, `door-transition`. "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (15 entries, HDR room.c:31-75; decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot deg X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-6, 0, 314) | 0x0B07 / 0x007F / 0x0070 | 22 / 0 / 0 (integers) | 0x3F0 D0d-D2n | none | 0x0180 | gameplay_keep | controller-invisible | yes |
| 1 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-21, 37, 47) | 0x0B00 / 0x007F / 0x000F | 22 / 0 / 0 (integers) | 0x00F D3d-D4n | none | 0x01A0 | gameplay_keep | controller-invisible | no |
| 2 | `ACTOR_EN_SYATEKI_MAN` (0x11D) | (145, 0, 290) | 0x0001 / 0x877F / 0x007C | 0 / 270 / 0 | 0x0FC D1d-D3n | none | 0xFF01 | object_shn | npc-scheduled | yes |
| 3 | `ACTOR_EN_SYATEKI_OKUTA` (0x14F) | (-150, 0, -320) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0xFFF0 | object_okuta | posed-skeletal | yes |
| 4 | same | (-20, 0, -320) | same | same | 0x3FF | none | 0xFFF1 | object_okuta | posed-skeletal | yes |
| 5 | same | (110, 0, -320) | same | same | 0x3FF | none | 0xFFF2 | object_okuta | posed-skeletal | yes |
| 6 | same | (-147, 0, -160) | same | same | 0x3FF | none | 0xFFF3 | object_okuta | posed-skeletal | yes |
| 7 | same | (-17, 0, -160) | same | same | 0x3FF | none | 0xFFF4 | object_okuta | posed-skeletal | yes |
| 8 | same | (113, 0, -160) | same | same | 0x3FF | none | 0xFFF5 | object_okuta | posed-skeletal | yes |
| 9 | same | (-150, 0, 0) | same | same | 0x3FF | none | 0xFFF6 | object_okuta | posed-skeletal | yes |
| 10 | same | (-20, 0, 0) | same | same | 0x3FF | none | 0xFFF7 | object_okuta | posed-skeletal | yes |
| 11 | same | (110, 0, 0) | same | same | 0x3FF | none | 0xFFF8 | object_okuta | posed-skeletal | yes |
| 12 | `ACTOR_EN_TEST4` (0x15A) | (-10, 0, 484) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0100 | gameplay_keep | controller-invisible | yes |
| 13 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (0, 210, 391) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF | none | 0x907F | object_obj_tokeidai | static-mesh | yes |
| 14 | `ACTOR_OBJ_SOUND` (0x0F0) | (0, 156, -83) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0146 | gameplay_keep | controller-invisible | yes |

Actor table IDs: `include/tables/actor_table.h:20` (En_Door 0x005), :259, :304, :354, :365, :431, :446. The nine Octoroks form a 3 x 3 grid, params low nibble = grid index `row * 3 + column` (`z_en_syateki_okuta.h:28-29`): row 0 back (z -320), row 1 center (z -160), row 2 front (z 0), columns left, center, right at x -150/-20/110 (center row -147/-17/113). The player stands at x -20, z 175, so index 7 (a10) is directly in front (man.c:54-60 comment).

### 4.3 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init: scale, collision, children | Draw | Presence / state gating |
| --- | --- | --- | --- |
| En_Syateki_Man (ANN "Shooting Gallery Man", man.c:1-8) | `ACTORCAT_NPC`; flags attention-enabled, friendly, update-culling-disabled, lock-on-disabled; flex `gBurlyGuySkel` with `gBurlyGuyHandsOnTableAnim` (looping) for the Town; scale 0.01; `colChkInfo.cylRadius` 100; no collider; talk offer radius 120; focus Y 70 (man.c:17-19, :204-243, :622-637, :1489-1490; XML `object_shn.xml:84`, :106) | `SkelAnime_DrawFlexOpa`; head limb `BURLY_GUY_LIMB_HEAD` replaced by `gTownShootingGalleryManHeadDL` (0xF2D0) for `SCENE_SYATEKI_MIZU`; eye texture pair open / closed (`gTownShootingGalleryManEyeOpenTex` 0xFB90, `...ClosedTex` 0x10390) through segments 8 and 9; blink counter 1 -> closed, 2 -> closed, 40 -> reset; head and torso track the player; **`draw = NULL` while a game runs** (man.c:1269, :1398, :1463-1482, :1497-1555) | a02 by mask D1d-D3n; path index `params >> 8` = 0xFF is computed but stored only in the Swamp (man.c:207, :238-242); low byte unused |
| En_Syateki_Okuta (ANN "Shooting Gallery Octorok", okuta.c:1-5) | `ACTORCAT_ENEMY`; flex `gOctorokSkel`; AC cylinder radius 20, height 40, y shift -30, accepts `AC_TYPE_PLAYER` with dmgFlags `0xF7CFFFFF` (every player damage type except normal shield, light ray and `DMG_UNK_0x1B`: arrows of all four types, sword, nuts, sticks, explosives, ...); red adds +10 radius and +15 height; Init floor-raycasts, looks up the water surface at (x, z) and kills itself without water or when the surface is not above the floor, else sets Y to the surface; every actor of this room has water, so Y = 37 (okuta.c:41-59, :87-114, :350-375; `include/z64collision_check.h:443-474`; XML overlay `ovl_En_Syateki_Okuta.xml`) | `gOctorokSkel`; red uses the actor setup DL, blue uses `gShootingGalleryOctorokBlueMaterialDL`; hit mark: cross (blue) or circle (red) at +30 Y, +20 Z fading from alpha 255 (okuta.c:555-592) | exists all day; invisible and idle (`draw = NULL`, `DoNothing`) until the keeper's wave signal (5.5) |
| En_Time_Tag | type `(p & 0xE000) >> 13 = 0`, text `(p & 0x1FE0) >> 5` (12, 13), exit `p & 0x1F = 0`; hour and minute from home rot X, Y; no draw | profile draw `NULL` (tag.c:41-50) | mask D0-D2 (a00), D3-D4 (a01); 2.5 |
| En_Test4 | day and half-day controller: stars `(p >> 10) * 100 = 0`, big-bell volume index `(p >> 5) & 0xF = 8` (`z_en_test4.h:8-9`) | none | always |
| Obj_Tokeidai type 9 | `OBJ_TOKEIDAI_TYPE_WALL_CLOCK`: scale 0.02, `ObjTokeidai_Clock_Draw`, `ObjTokeidai_Clock_Init` from the current time, action `WallClock_Idle` (`z_obj_tokeidai.h:6-20`; toke.c:104-115, :247-252, :719-723) | minute ring 0xCF28, center and hand 0xBEE8, `gWallClockClockFaceDL` 0xF518 (the wall face, not the tower face), sun and moon panel 0xC368 (toke.c:811-847; XML `object_obj_tokeidai.xml:47`, :50, :53, :67) | always; follows the clock every frame |
| Obj_Sound | type `(p >> 8) & 0xFF = 1` `OBJ_SOUND_TYPE_BGM`; sound id `p & 0x7F = 0x46` = `Sequence_70` `NA_BGM_SHOOTING_GALLERY` (`sequence_table.h:160`); plays positional BGM from its projected position from the second update; `Destroy` resets to `NA_BGM_GENERAL_SFX` (`z_obj_sound.h:8-18`; `z_obj_sound.c:28-66`) | none | always |
| En_Door TA0 | 2.3 | 2.3 | always; schedule 2.5 |

### 4.4 Counts

| | Default header |
| --- | --- |
| Entries / transition actors | 15 / 1 (LOCATION_REGISTER "d 3/15", 1 transition: agree) |
| Day 1 daytime candidates by mask | 14 (excluded: a01) |
| controller-invisible | 4 (a00, a01, a12, a14) |
| npc-scheduled | 1 (a02) |
| posed-skeletal | 9 (a03-a11, hidden at rest) |
| static-mesh | 1 (a13) |
| door-transition | 1 (TA0) |
| Runtime-spawned | none in this scene (the Octorok hit marks are part of the Octorok draw; rupee, quiver and heart-piece prizes are item offers, 5.7) |

## 5. Venue mechanics index (INT-12 scope: bow minigame, keeper, wall clock)

Index and citations only; no Unity design. Everything in 5.3-5.8 is `En_Syateki_Man` (Town branches, `play->sceneId == SCENE_SYATEKI_MIZU`) plus `En_Syateki_Okuta`; the Swamp branches of the same actor are out of scope.

### 5.1 Who does what

| Item | Value | Evidence |
| --- | --- | --- |
| Keeper | a02 at (145, 0, 290), yaw 270 (faces -X), standing in the right-hand alcove behind the wall x 80; runs idle, talk, the whole game state machine and the rewards | HDR room.c:37-39; OVL man.c:204-243, :622-1005, :1215-1461 |
| Targets | nine Octoroks, 3 x 3 grid in the water pool, signalled by `octorokFlags`; they attach to the keeper by scanning the NPC list for `ACTOR_EN_SYATEKI_MAN` | OVL okuta.c:182-194, :400-414; `z_en_syateki_man.h:30-46` |
| Fee, rupee gate, bow gate | 20 rupees; refused without a bow (`CUR_UPG_VALUE(UPG_QUIVER) == 0`) | OVL man.c:644, :655, :675 |
| States | `SG_GAME_STATE_*`: NONE, RUNNING, EXPLAINING_RULES (also the high-score text), NOT_PLAYING, ONE_MORE_GAME, GIVING_BONUS (Swamp), ENDED, MOVING_PLAYER; Octorok states APPEARING 0, APPEARED 1, INITIAL 70, HIDING 80 | `z_en_syateki_man.h:30-46` |
| Keeper talk flags (actor field, not saved) | bit 0 human spoke, 1 Deku, 2 Goron, 3 Zora, 4 rules explained | man.c:45-50 |

### 5.2 State storage

| Item | Value | Evidence |
| --- | --- | --- |
| High score | `HIGH_SCORE(HS_SHOOTING_GALLERY)` low 16 bits (Swamp score in the high 16 bits); new save and every cycle reset set it to **39** | `z64save.h:627-630`; sram.c:404-409; `Sram_SaveEndOfCycle` (sram.c:430-654) calls `Sram_ClearHighscores` at :637; new-save field generation (`Sram_GenerateRandomSaveFields`) calls it at :714 |
| Quiver prize flag | `WEEKEVENTREG_RECEIVED_TOWN_SHOOTING_GALLERY_QUIVER_UPGRADE` = 59/0x20; **persistent across the Song of Time** (row 59) | `z64save.h:1198`; sram.c:158-160 |
| Heart-piece prize flag | `WEEKEVENTREG_RECEIVED_TOWN_SHOOTING_GALLERY_HEART_PIECE` = 32/0x04; **persistent** (row 32) | `z64save.h:994`; sram.c:120-122 |
| Kickout deferral | `KICKOUT_WAIT` 63/0x01, `KICKOUT_TIME_PASSED` 63/0x02 (not persistent) | `z64save.h:1227-1229`; 2.5 |
| Minigame fields | `gSaveContext.minigameStatus` (ACTIVE during a run, END after), `minigameScore`, `interfaceCtx.minigamePoints` (+1 per red hit), `minigameAmmo` (20, never decremented here), timer `TIMER_ID_MINIGAME_1` | `param.c:2481-2492`, :7171-7175; okuta.c:431-434; player.c:4293-4297 |
| Scene chest word | the scene's `sceneFlags.chest` is used as a scratch number: set to the last score at the end of a run, incremented on each Human talk; read only by message control code `CHEST_FLAGS` (ids 0x401 and 0x403, "you got [score]") | OVL man.c:521, :1413; C `src/code/z_actor.c:820-829`, `src/code/z_message.c:2438-2440`; text-header check (`msgs2.py`) |
| Scene cycle flags | `PERSISTENT_CYCLE_FLAGS_NONE`: the chest word is cleared at the end of a cycle | HDR scene_table.h:71; sram.c:450, :461-462 |

### 5.3 Talk gating (checked in this order; man.c:622-637)

| Condition | Text ids | Result | Evidence |
| --- | --- | --- | --- |
| mask worn (not none, below `PLAYER_MASK_GIANT`) with a nonzero reaction entry | `0x236F` (most masks), `0x2370` (Kafei's Mask), `0x2371` (Captain's Hat), `0x2372` (Stone Mask); 0 (normal intro) for Bunny Hood and Postman's Hat | text only, no game | man.c:624-629; `src/code/z_face_reaction.c:3-22`, :85-87, :181-186 |
| Human, Day 1-2, first / repeat | `0x3E8` / `0x3E9` -> `0x3F6` (fee choice) | game offered | man.c:520-543 |
| Human, Day 3, first / repeat | `0x3EA` / `0x3EB` -> `0x3F6` | game offered | man.c:533-542 |
| Deku, Day 1-2 / Day 3 | first `0x3EC` -> `0x3ED`, repeat `0x3ED`; Day 3 first `0x3EE` -> `0x3EF`, repeat `0x3EF` | no game ("no bow" / "go home") | man.c:545-567, :727-737 |
| Zora, Day 1-2 / Day 3 | first `0x3F0` -> `0x3F1`, repeat `0x3F1`; Day 3 first `0x3F4` -> `0x3F5`, repeat `0x3F5` | no game | man.c:569-591, :739-755 |
| Goron, Day 1-2 / Day 3 | first `0x3F2` -> `0x3F3`, repeat `0x3F3`; Day 3 shares Zora's `0x3F4` / `0x3F5` | no game | man.c:593-615, :745-755 |
| Fierce Deity | no case in the intro switch: no text starts (INF: consistent with the form being unavailable in town; position constants for it exist, man.c:187) | softlock risk (INF) | man.c:617-618 |

Human talk also increments the scene chest word (man.c:521). Text ids ending in an event code (0x3E8-0x3EC, 0x3EE, 0x3F0, 0x3F2, 0x3F4, 0x3FD-0x407 variants) are the ones `HandleNormalMessage` reacts to (`msgs2.py`).

### 5.4 Fee and start sequence

| Step | What | Evidence |
| --- | --- | --- |
| 1 | choice message `0x3F6` (two choices; header price field 20); "Yes" = index 0 | man.c:723-724, :643; text header `firstChoicePrice 20` |
| 2 | Yes, no bow: error sound, `0x3F9` (Day 1-2) / `0x3FA` (Day 3), no charge | man.c:644-654 |
| 3 | Yes, rupees < 20: error sound, `0x3FB` / `0x3FC`, state NOT_PLAYING (if it was ONE_MORE_GAME also `minigameStatus = END` and `PLAYER_STATE3_400` cleared) | man.c:655-672 |
| 4 | Yes, enough: decide sound, `Rupees_ChangeBy(-20)`, state EXPLAINING_RULES, text `0x3FD` (rules, first time, sets talk bit 4) or `0x3FF` (reminder), set `KICKOUT_WAIT`, clear `KICKOUT_TIME_PASSED` | man.c:673-690 |
| 5 | No: cancel sound, `0x3F7` (Day 1-2) / `0x3F8` (Day 3), state NOT_PLAYING | man.c:691-709 |
| 6 | after `0x3FD` / `0x3FF`: first game -> close box, state MOVING_PLAYER, lock Link (`PLAYER_STATE1_20`), set WAIT again, action `Town_MovePlayerAndSayHighScore`; replay (ONE_MORE_GAME) -> straight to `0x3FE` (after `0x3FD`) or `0x400` (after `0x3FF`), no walk | man.c:757-779 |
| 7 | auto-walk: stick override toward (-20, 40, 175) (Fierce Deity (-20, 20, 198)), stick magnitude 80 beyond 30 u, 40 from 5 to 30 u, 10 under 5 u, done under 5 u (XZ); then `0x3FE` ("highest score is [HS]; break it for a prize", control code `HS_TOWN_SHOOTING_GALLERY`) or `0x400` | man.c:253-278, :186-188, :1215-1238; `z_message.c:2969-2971` |
| 8 | closing `0x3FE` / `0x400`: `Interface_InitMinigame` (status ACTIVE, score 0, ammo 20), `Player_SetBButtonAmmo(99)` (B ammo value 100 = 99 + 1), wave index 0, state RUNNING, sub-BGM `NA_BGM_TIMED_MINI_GAME`, action StartGame | man.c:781-792; `param.c:2481-2492`; `z_player_lib.c:1573-1577` |
| 9 | StartGame: first frame teleports Link to the station position, yaw -0x8000 (faces -Z), locked; 30-frame countdown with yaw held; then unlock, score 0, wave 0, Octorok state 70, **timer 75 s** (`Interface_StartTimer(TIMER_ID_MINIGAME_1, 75)`, HUD at (115, 80)), keeper `draw = NULL`, action RunGame | man.c:1240-1272; `param.c:836-852` |
| Ammo | unlimited: the arrow-fire path decrements `minigameAmmo` only outside `SCENE_SYATEKI_MIZU`, `SCENE_F01`, `SCENE_SYATEKI_MORI`; the ammo reader returns `minigameAmmo` (20) while the status is ACTIVE; the B ammo number is not drawn in the gallery scenes; B with `bButtonAmmoPlusOne > 0` draws the bow (INF: B is the shoot button) | player.c:4292-4297, :3984-3989, :4240-4241; `param.c:4259-4272` |
| HUD | score icon `gArcheryScoreIconTex` (24 x 16 rgba16, `parameter_static` 0x3FA0) at (20, 67 or 75) plus up to four digit sprites; timer digits | `param.c:6230-6298`, :6264-6270; XML `interface/parameter_static.xml:46`, :53 |
| Station | Y 40 is the top tier (floor poly 57, surface 7, bgCam 1 = `CAM_SET_ROOM1`); the Fierce Deity constant (z 198) lies on the Y 13, 26 and 40 tiers' XZ alike (z < 200), so its final tier depends on the floor raycast (NI) | C scratch `geom.py` |

### 5.5 Wave schedule and targets (RunGame, man.c:1342-1407)

| Rule | Value | Evidence |
| --- | --- | --- |
| Timer | real time: current = limit - elapsed `osGetTime` centiseconds; `timer = current * 0.1 + 1` (tenths of a second); the block runs when `timer <= 750` | `param.c:5927-5957`; man.c:1345-1348 |
| New wave | every 5.00 s: when `timer % 50 == sModFromLosingTime` and `octorokState >= 70`, `octorokFlags = sOctorokFlagsPerWave[flagsIndex++]`, sound `NA_SE_SY_FOUND`, state APPEARING (0); **15 waves** = 75 s | man.c:1383-1389, :1278-1340 |
| Hide | `waveTimer = current % 500` (shifted by 250 after a blue hit); when `< 100` (the last second of each 5 s cycle, four seconds after the wave began) state HIDING (80) | man.c:1355-1365 |
| Appear / float / hide | on APPEARING an Octorok whose grid cell holds a type > NONE sets its type, scale 0.01 and plays Appear (splash and jump sound on frames 2 and 15, suppressed when hidden behind another), then Float until the state is >= 70, then Hide (splash frame 4) | okuta.c:211-270, :400-414 |
| Occlusion | back-center (index 1) is hidden by an occupied center-center (4) or front-center (7); center-center (4) by an occupied front-center (7); other cells never; a blue Octorok that is occluded while appearing cannot be hit | okuta.c:143-166, :355-358 |
| Hit | `AC_HIT` while not Die / DoNothing: red -> sound, `minigamePoints++`, `score++`, `lastHitOctorokType = RED`; blue -> error sound, `lastHitOctorokType = BLUE`; then Die (scale pulse, smoke, bubbles, splash, ~shrink to 0, then DoNothing) | okuta.c:380-392, :416-447, :272-344 |
| Blue penalty | each blue hit subtracts **2.50 s** from the timer limit and shifts the wave grid by 25 tenths so the 5 s cadence is unchanged | man.c:1367-1374 |
| End of run | `timerCurTimes == 0`: wave index 0, state HIDING, timer STOP, Link locked, keeper drawn again, sub-BGM stopped, action EndGame; score 50 plays the get-item fanfare and the PERFECT letters | man.c:1391-1405; `param.c:5061-5075` |
| Perfect score | **50** = all red Octoroks of the 15 waves (77 targets: 50 red, 27 blue; scratch `waves.py`) | man.c:1401, :1278-1340 |

Waves (rows back / center / front, columns left to right; R red +1, B blue -2.5 s, . empty; counted by `waves.py`):

| Wave | Back / center / front | R | B | Wave | Back / center / front | R | B |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | R.R / .R. / ... | 3 | 0 | 8 | .R. / BRB / BRB | 3 | 4 |
| 1 | ..R / RBR / ... | 3 | 1 | 9 | B.R / R.B / B.R | 3 | 3 |
| 2 | RRB / ... / ..R | 3 | 1 | 10 | RBR / BRB / R.R | 5 | 3 |
| 3 | R.R / ... / R.R | 4 | 0 | 11 | RRR / B.B / B.B | 3 | 4 |
| 4 | .R. / BRB / ... | 2 | 2 | 12 | BBB / RRR / ... | 3 | 3 |
| 5 | ..R / R.. / .R. | 3 | 0 | 13 | R.R / R.R / BBB | 4 | 3 |
| 6 | R.R / .B. / ..R | 3 | 1 | 14 | RRR / .R. / .R. | 5 | 0 |
| 7 | RRR / ... / B.B | 3 | 2 | total | | 50 | 27 |

### 5.6 End of run: result flow

| Step | What | Evidence |
| --- | --- | --- |
| EndGame | while RUNNING: clear `octorokFlags`; after 15 frames (`talkWaitTimer`) and once the PERFECT letters are off, store the score in the scene chest word, then choose the text; from `talkWaitTimer < 5` on the B ammo is switched off (`bButtonAmmoPlusOne = -10`) | man.c:1409-1461 |
| No record, not perfect | `0x401` (Day 1-2) / `0x403` (Day 3), state ONE_MORE_GAME; no prize; high score unchanged | man.c:1438-1450 |
| Record or perfect | high score := score; state ENDED; text per 5.7 | man.c:1415-1437 |
| After the result text | `0x401` / `0x403`: if `KICKOUT_TIME_PASSED` -> close, clear WAIT and TIME_PASSED, state NONE, Idle; else `0x402` / `0x404` ("try again?") -> `0x3F6` with state ONE_MORE_GAME (fee choice again, no walk) | man.c:794-828 |
| Prize texts `0x405` / `0x406` / `0x407` | close the box, `minigameStatus = END`, action `Town_SetupGiveReward` | man.c:830-839 |
| Item offer | `Actor_OfferGetItem` (range 500, height 100) every frame until `Actor_HasParent`; Link is turned to -0x8000 and stopped meanwhile | man.c:942-979 |
| After the prize | Day 1-2: wait for the closing text, unlock Link, score 0, state NONE, clear both kickout flags, back to Idle (talk again to replay); Day 3: a talk offer then text `0x408`, same reset | man.c:981-1005 |

### 5.7 Prize decision (man.c:1415-1437, :942-979)

Inputs: `s` = score, `HS` = stored high score (39 on a fresh cycle), `Q` = quiver flag, `H` = heart-piece flag, `u` = quiver upgrade value (1 = 30 arrows; 0 means no bow, which cannot play).

| Case | Text | Item offered | Flag written when the item is taken |
| --- | --- | --- | --- |
| `s <= HS`, `s != 50` | `0x401` / `0x403` | none | none |
| `s > HS`, `Q` clear (any `s`, including 50) | `0x407` | if `u < 3`: `GI_QUIVER_30 + u` (u = 1 -> `GI_QUIVER_40`, u = 2 -> `GI_QUIVER_50`); else `GI_RUPEE_PURPLE` | set `Q` |
| `s > HS`, `Q` set, `s != 50` | `0x407` | `GI_RUPEE_PURPLE` = 50 rupees | none |
| `s > HS`, `Q` set, `s == 50` | `0x405` | `GI_HEART_PIECE` if `H` clear, else `GI_RUPEE_HUGE` = 200 rupees | set `H` on the heart piece |
| `s == 50`, `s <= HS` (HS already 50) | `0x406` | same as the row above | same |

Rupee amounts: `sRupeeRefillCounts = { 1, 5, 10, 20, 50, 100, 200 }` indexed by `item - ITEM_RUPEE_GREEN` (`param.c:2543`, :2857; `z64item.h:327-333`): purple (index 4) 50, huge (index 6) 200; `GI_*` enum `z64item.h:414-451`, table `player.c:2076-2172` (quiver rows map to `ITEM_BOW`, `ITEM_QUIVER_40`, `ITEM_QUIVER_50`). The high score is written inside the "record or perfect" branch (man.c:1436), so an equal or lower non-perfect run never changes it.

### 5.8 Repeat control and cycle reset

| Reward | Control | Evidence |
| --- | --- | --- |
| Quiver upgrade | once per save (flag 59/0x20 persistent); a later new record gives 50 rupees | 5.2, 5.7 |
| Heart piece | once per save (flag 32/0x04 persistent), needs 50 | 5.2, 5.7 |
| Purple rupee | every new record above the current high score; the high score resets to 39 each cycle, so records pay again after a Song of Time | 5.7; sram.c:637 |
| Huge rupee | **every** perfect run after the heart piece (fee 20): no cooldown, day limit or flag | 5.7 |
| Fee | 20 rupees per run, ONE_MORE_GAME loop keeps Link in the booth | 5.4, 5.6 |

### 5.9 Cameras, lighting, sound, hits

| Use | Setting | Source chain | Function data | Evidence |
| --- | --- | --- | --- | --- |
| Floor camera, every surface except surface 7 | bgCam 0 = `0x25 CAM_SET_CIRCLE3`: NORMAL mode `CAM_FUNC_FIXED2`; also Deku-shoot, bow-Z, first-person, boomerang, Zora fin, follow-target, talk, slingshot modes | `BgCamInfo[0]` `{0x0025, 3, pos data}` | eye (-190, 146, 306), rot (0, 0, 0), fov / unk_0E / unk_10 = -1 -1 -1 (defaults); `Camera_Fixed2` reads only `pos`, `fov`, `unk_0E` and `unk_10` (`rot` is not read) and falls back to the mode defaults for -1 (answers the school's U-06 for the FIXED2 data fields) | HDR scene.c:98-107; C `z_camera_data.inc.c:2494-2508`, :869-872, :3559; `z_camera.c:4799-4909`; `z64camera.h:184`; `z64bgcheck.h:133-142` |
| Top tier (Y 40, polys 56, 57) | bgCam 1 = `0x51 CAM_SET_ROOM1`: NORMAL `CAM_FUNC_NORMAL1`, bow aim `CAM_MODE_BOWARROW` -> `CAM_FUNC_SUBJECT1` first person | `BgCamInfo[1]` `{0x0051, 0, NULL}` | none | HDR scene.c:106; C `z_camera_data.inc.c:3275-3289`, :3603; `z64camera.h:228`; INF: the minigame station is on this surface |
| Water box camera | bgCamIndex 0 | water properties 0x00000000 | | C `z64bgcheck.h:33-46` |
| Door | generic door camera (bgCam -1 both sides) | TA0 sides | | HDR scene.c:38 |
| Arrival | no actor cs camera (bgCam 0xFF) | 2.1 | | `z_play.c:2326-2328` |
| Light | all nine surface types select light row 0 (`lightSetting` bits 6-10 = 0); echo 2 on every surface, room echo 4 | `SurfaceType_GetLightSettingIndex` | | C `z_bgcheck.c:4265`; player.c:6603; scratch `col.py` |
| Scene sound | seq 0; **a14 plays the gallery BGM `NA_BGM_SHOOTING_GALLERY` as a positional source at (0, 156, -83)**; during a run `Audio_PlaySubBgm(NA_BGM_TIMED_MINI_GAME)` plays and is stopped at the end; perfect plays `NA_BGM_GET_ITEM` or-ed with 0x900 | a14 params 0x0146 | | OVL man.c:790, :1399, :1402; `sequence_table.h:88`, :94, :160 |

The camera used by the first-person bow in the source is whichever floor setting the player stands on; the school's CIRCLE3 course camera was an actor camera, this one is a floor camera.

### 5.10 Wall clock `Obj_Tokeidai` type 9 (a13)

| Item | Value | Evidence |
| --- | --- | --- |
| Placement | (0, 210, 391), yaw 180, scale 0.02 (twice the school's small clock), on the entrance wall above the doorway (the wall plane is z 400) | HDR room.c:70-72; toke.c:247-252 |
| Draw | minute ring, center and hand, `gWallClockClockFaceDL`, sun and moon panel (4 DLs); the dossier's noon pose is the clock at 12:00 | toke.c:811-847 |
| Motion | every frame: `clockTime = CURRENT_TIME`; rotate on minute change, rotate on hour change | toke.c:719-723, :104-115 |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header)

Selection: default scene and room header (the only header), sceneLayer 0, half-day bit 0x080, fresh save, Human form. **The venue is open at Day 1 12:00** (2.5). World positions are N64; `world = actorPos + actorRot x (actorScale x localVertex)` (ROM_REFERENCE.md); rotation conversion under the Z mirror belongs to the importer.

| # | Component / params | N64 position (Unity) | Yaw | Actor scale | Display list(s), bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room | 19 cullable entries: 18 opaque, 3 XLU (room.c:89-109); DL 0x5130 aggregates the 18 opaque DLs, DL 0x6128 the 3 XLU | origin | n/a | 1 | `Z2_SYATEKI_MIZU_room_00DL_*` (26 Gfx arrays, 21 Vtx arrays) | what each entry draws is not inspected (U-01) |
| water | XLU entry 17 at (0, 37, -114) r 340: `DL_0052E0`, one texture twice with `0x08000000` scroll (segment 8) | (0.00, 0.37, 1.14) | n/a | 1 | `Z2_SYATEKI_MIZUTex_0013E0` rgba16 32 x 32, alpha 187 | water surface; scroll is the animated material (section 1) |
| TA0 | Door, 0x0287 | (-10, 0, 420) (-0.10, 0.00, -4.20) | 0 | 0.01 (default) | `gDoorSkel` gameplay_keep 0x22B28, limb 4 = `gDoorLeftDL` 0x20BB8 or `gDoorRightDL` 0x20D00 | closed rest pose; side by camera yaw (door.c:651-657) |
| a13 | Large wall clock, 0x907F | (0, 210, 391) (0.00, 2.10, -3.91) | 180 | 0.02 | object_obj_tokeidai 0xCF28, 0xBEE8, 0xF518, 0xC368 | noon pose per ROM_REFERENCE clock rules |

**Present at Day 1 12:00 but not assembled statically:** a02 keeper (145, 0, 290) yaw 270 (`gBurlyGuySkel` + `gBurlyGuyHandsOnTableAnim` + `gTownShootingGalleryManHeadDL`: posed-skeletal reconstruction); a03-a11 Octoroks at Y 37 (water surface), idle and not drawn until a wave: positions (Unity) back row (-1.50, 0.37, 3.20), (-0.20, 0.37, 3.20), (1.10, 0.37, 3.20); center row (-1.47, 0.37, 1.60), (-0.17, 0.37, 1.60), (1.13, 0.37, 1.60); front row (-1.50, 0.37, 0.00), (-0.20, 0.37, 0.00), (1.10, 0.37, 0.00). **Excluded by half-day mask:** a01 (D3d-D4n). **No geometry:** a00 (kickout tag), a12 (`En_Test4`), a14 (`Obj_Sound`). **Not present at rest:** hit marks (shown only while an Octorok dies).

Playable minigame (INT-12: a decorated booth alone is not a playable gallery) additionally needs: the bow, aim and arrow actors (`player.c:3984-4297`, B-ammo path), the nine Octorok actors with water (section 5.5), the keeper state machine (5.3-5.7), the 75 s timer and score HUD (5.4), rupee spend, the high score (default 39) and the two persistent prize flags (5.2), the floor-selected cameras (5.9), the invisible projectile-passing walls and the counter top rule (section 7), and the 22:00 kickout with its deferral (2.5).

## 7. Collision summary (C from scene.c:98-559; scratch `col.py`, `geom.py`, `unref.py`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 232 / 192 / 9 (LOCATION_REGISTER does not count these). 229 vertices are referenced; vertices 225-227 = (-209, 37, -413), (-209, 37, 153), (218, 37, 153) are the water-box corners and no polygon uses them |
| Water boxes | 1: `{ -209, 37, -413, 427, 566, 0x00000000 }` = x -209..218, z -413..153, surface Y 37 (`WaterBox` x, ySurface, z, xLength, zLength, properties; `z64bgcheck.h:140-149`); properties = bgCam 0, light 0, room 0, enabled |
| Camera data | 2: `0x0025 CAM_SET_CIRCLE3` (3 Vec3s) and `0x0051 CAM_SET_ROOM1` (NULL) |
| Header bounds | min (-260, -3, -413), max (260, 299, 502); referenced-vertex extents x -260..260, y 0..299, z -400..486; all-vertex extents z -413..486 (header max z 502 is 16 u past the vertices, U-07) |
| Unity extents (all vertices) | x -2.60..2.60, y 0..2.99, z -4.86..4.13 |
| Polygon kinds (normal Y > 0.5 floor, < -0.5 ceiling) | 20 floors, 96 walls, 76 ceilings (34 flat, 42 sloped roof panels) |
| Materials by polygon | dirt (0) 174, stone (2) 4, wood (10) 14 (`include/z64bgcheck.h:231-247`) |
| Exit surfaces | 1 type (surface 4), 2 floor polygons (2.4) |
| Poly exclusion | xp 1 ignore-camera: 10 (alcove ceilings 0, 1, alcove walls 2-5, floors 30, 31, 36, 37); xp 4 ignore-projectiles: 3 (polys 13-15, the z -400 back wall Y 69..291); xp 5 camera + projectiles: 2 (polys 190, 191, the invisible barrier z 160, Y 40..167); no vIB flags (no conveyor, no Deku flower) (`z64bgcheck.h:65-68`) |
| Other flags | all surfaces: lightSetting 0, echo 2, floorType 0, no hookshot, no conveyor, not soft; floorProperty 9 on surfaces 2, 3, 4, 6, 7 (INF reading: skips the ledge-jump and ledge-hang checks, player.c:7539, :7569), 8 on surface 5 (pins X and Z when the player leaves the ground, player.c:7504-7508), 0 on 0, 1, 8; wallType 1 on surface 1 (`WALL_FLAG_0`: no ledge climb on its 76 walls, `z_bgcheck.c:18-20`, :4206-4208; player.c:11835) |
| Special | no ladder, hookshot, conveyor, void or damage surfaces; one water box (the Octorok pool) |

| Surface | data[0] / data[1] | Material, flags | Polys (floor / wall / ceiling) | Where |
| --- | --- | --- | --- | --- |
| 0 | 0x00000000 / 0x00001000 | dirt | 0 / 14 / 64 | step risers (polys 50-55, 60-63, 68-71), roof panels |
| 1 | 0x00200000 / 0x00001000 | dirt, wallType 1 | 0 / 76 / 12 | room walls, alcove, counter faces, invisible barrier, flat underside strips |
| 2 | 0x24000000 / 0x00001000 | dirt, floorProp 9 | 6 / 0 / 0 | alcove floor (30, 31), pool floor (38, 39), back shelf Y 69 (184, 185) |
| 3 | 0x24000000 / 0x00001002 | stone, floorProp 9 | 4 / 0 / 0 | booth floor (32-35), Y 0, z 160..400 |
| 4 | 0x24000100 / 0x00001000 | dirt, exit 0 | 2 / 0 / 0 | exit floor behind the door (36, 37) |
| 5 | 0x20000000 / 0x0000100A | wood, floorProp 8 | 2 / 2 / 0 | counter top Y 52 (40, 41); keeper-alcove wall x 80, Y 40..167 (188, 189) |
| 6 | 0x24000000 / 0x0000100A | wood, floorProp 9 | 4 / 0 / 0 | tiers Y 13 (48, 49) and Y 26 (64, 65) |
| 7 | 0x24000001 / 0x0000100A | wood, bgCam 1 | 2 / 0 / 0 | top tier Y 40 (56, 57) |
| 8 | 0x00000000 / 0x0000100A | wood | 0 / 4 / 0 | tier risers z 200 and z 220 (58, 59, 66, 67) |

| Space | Extent x / y / z | Height | Polys |
| --- | --- | --- | --- |
| Booth floor (player area) | -200..80 / 0 / 160..400 (floor spans x -200..200; the keeper alcove is walled off at x 80) | roof to Y 299 | floors 32-35 |
| Steps | Y 13: x -80..40, z 160..240; Y 26: x -70..30, z 160..220; Y 40: x -60..20, z 160..200 | risers 13 / 13 / 14 | floors 48, 49, 64, 65, 56, 57; risers 50-55, 58-63, 66-71 |
| Counter | x -200..200, z 135..160, top Y 52 | 52 | top 40, 41; front face z 135 (44, 45); player-side face z 160 (42, 43) |
| Invisible barrier above the counter | x -200..200, z 160, Y 40..167 | | 190, 191 (xp 5): arrows pass, Link does not (INF that the player cannot lean over) |
| Keeper alcove | x 80..200, z 160..400 | | wall x 80: 46, 47 (Y 0..40), 188, 189 (Y 40..167); the keeper stands here |
| Target pool | floor x -200..200, z -387..160, Y 0; water x -209..218, z -413..153, surface 37 | depth 37 | floors 38, 39; side walls x +-200 (16-19), back walls z -380 and z -400 (13-15 pass arrows) |
| Back shelf | x -200..200, z -400..-380, Y 69 | | floors 184, 185 |
| Door alcove | x -40..20, z 400..486, Y 0..100 | 100 | walls 2-5; ceilings 0, 1; floors 30, 31, 36, 37 |

Probe results (scratch `geom.py`): spawn 0 (-10, 390) stands on poly 33; the door probe (-10, 442) lands on poly 37; the player station (-20, 175) stands over floors 34, 48, 57 and 65 (tiers Y 0, 13, 40, 26; the top is poly 57); all nine Octorok XZ positions lie inside the water box and over floors 38 or 39 (so each survives `Init`); the keeper (145, 290) stands on poly 32; `En_Time_Tag` a00 (-6, 314) on poly 34, a01 (-21, 47) in the pool over poly 38.

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 20: 17 textures + 3 TLUTs |
| By format | rgba16 8 (+3 TLUT PNGs), ia16 2, ia4 4, ci4 3 |
| Names | `Z2_SYATEKI_MIZUTex_` 0013E0, 001BE0, 0023E0, 0024E0, 002CE0, 002D60, 002E60, 003660, 004660, 005660, 005E60, 006660, 006A60, 006E60, 007660, 007E60, 008660; `Z2_SYATEKI_MIZUTLUT_` 009660, 009680, 0096A0 |
| Location | all 20 in the scene file; the room file holds none; the room DLs load all 17 textures (`gsDPLoadTextureBlock` 26 incl. 8 `_4b`, 1 `gsDPLoadMultiBlock`, 3 TLUT loads); no external texture file |
| Room `Gfx` / `Vtx` arrays | 26 / 21 (21 `.vtx.inc` files) |
| Room shape | cullable, 19 entries: 18 opaque, 3 XLU (entries 13, 14, 17; room.c:89-109); XML aggregates `0x5130` (opaque) and `0x6128` (XLU) (room.c:802-822, :977-983) |
| Triangles in source | 280 `gsSP2Triangles` + 7 `gsSP1Triangle` = 567 (DL reuse not resolved) |
| Render setup | 21 `gsDPSetCombineLERP`, 21 `gsDPSetRenderMode` |
| Animated materials | 1: the water plane (segment 8, two-layer scroll; room.c:846) |
| Keeper (object_shn) | Town head `gTownShootingGalleryManHeadDL` (0xF2D0) with 2 TLUTs, 2 rgba16 eye textures and 5 ci8 head textures (`object_shn.xml:109-120`); the body limbs, skeleton and animations are shared with the Swamp keeper |
| Octorok (object_okuta) | skeleton `gOctorokSkel`, 6 animations (4 used by this actor, shoot and hit unused: okuta.c:61-78), 11 textures in the object XML; blue-material DL and 2 hit-mark ia16 textures from the overlay XML |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_SYATEKI_MIZU` (C above); `object_obj_tokeidai` (South). Binaries for every input below exist under `extracted/n64-us/baserom/` (checked with `ls`/`wc`: object_shn 70,176; object_okuta 18,048; object_obj_tokeidai 72,832; gameplay_keep 599,520; gameplay_field_keep 37,520; ovl_En_Syateki_Okuta 7,904; parameter_static present).

| Priority | Inputs | Why |
| --- | --- | --- |
| Day 1 static | `gameplay_keep` | door skeleton and the default leaf pair (section 6) |
| Posed pass | `object_shn`, `object_okuta`, overlay assets `ovl_En_Syateki_Okuta` | keeper skeleton, Town head and eyes; Octorok skeleton and animations; blue material and hit marks |
| HUD (optional) | `interface/parameter_static` | score icon and digit textures (5.4) |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('gameplay_keep','object_shn','object_okuta')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
& $zapd e -i "assets/xml/overlays/ovl_En_Syateki_Okuta.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/overlays/ovl_En_Syateki_Okuta" -osf "extracted/n64-us/assets/overlays/ovl_En_Syateki_Okuta" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
if ($LASTEXITCODE -ne 0) { throw "Extraction failed: ovl_En_Syateki_Okuta" }
& $zapd e -i "assets/xml/interface/parameter_static.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/interface/parameter_static" -osf "extracted/n64-us/assets/interface/parameter_static" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
```

`gameplay_keep` has not been extracted with this tool before (NI); check size and warnings separately. The overlay and `interface` output paths are proposals (no overlay file has been extracted yet; the overlay XML carries a `BaseAddress` and `RangeStart`/`RangeEnd`, so the flags may need adjusting, U-04).

## 10. Proposed region record

Schema 1 (PIPE-01, `shared/city/schema/region.schema.json`). `portalIds` are exactly the two portals departing `loc.town_shooting_gallery` in `shared/city/ids.json`.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.town_shooting_gallery",
  "displayLabel": "Town Shooting Gallery",
  "source": {
    "sceneSymbol": "Z2_SYATEKI_MIZU",
    "sceneEnum": "SCENE_SYATEKI_MIZU",
    "sceneTableIndex": "0x20",
    "entranceTableSceneIndex": "0x1D",
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
      "fresh save: no week-event flags (quiver and heart-piece prize flags clear, kickout flags clear)",
      "player form Human, no mask, quiver value irrelevant to the static assembly",
      "clock 12:00: venue open (door schedule closed only 22:00-05:59)",
      "no game in progress: keeper drawn, Octoroks idle and not drawn, high score 39",
      "scene has no alternate headers"
    ]
  },
  "localSourceInputs": [
    {
      "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_SYATEKI_MIZU",
      "bytes": 38592,
      "sha256": "ac3046d43f9a671eb36539d04c914cd83b6f3da6e75a74914e33bcce9c5856cb"
    },
    {
      "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_SYATEKI_MIZU_room_00",
      "bytes": 24912,
      "sha256": "bd46d74bca1afb266498e36b89b1397ffcc633f49ce80cf87cde5a0809af6c54"
    },
    {
      "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_SYATEKI_MIZU/Z2_SYATEKI_MIZU.c",
      "bytes": 29274,
      "sha256": "dd9151ddd880bcda5aca72cdc35c334bdd5984ce993f33af23253b4daeee0aa0"
    },
    {
      "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_SYATEKI_MIZU/Z2_SYATEKI_MIZU_room_00.c",
      "bytes": 51199,
      "sha256": "f5ae263badf719e4f40feeecb0ad3d9cfe12e25f6a4723dedbb81a59b38c3fb3"
    },
    {
      "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_SYATEKI_MIZU/Z2_SYATEKI_MIZU.xml",
      "bytes": 428,
      "sha256": "abf9c6d4d0ed61efad3a91a5eac76947df165a369e4fa1031e98f036254e1cec"
    }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": [],
      "status": "not_shared",
      "evidence": "interior local frame (collision x -260..260, z -413..486); joined to East Clock Town only by the door transition (East TA#3 at (1114, 100, 274), gallery TA0 at (-10, 0, 420)); exit and arrival coordinates do not coincide (dossier 2.4)"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-260, -3, -413], "max": [260, 299, 502] },
    "sourceCollisionVertices": { "min": [-260, 0, -413], "max": [260, 299, 486] },
    "unityCollisionVertices": { "min": [-2.6, 0.0, -4.86], "max": [2.6, 2.99, 4.13] }
  },
  "water": [
    {
      "id": "water.town_shooting_gallery.pool",
      "sourceWaterBox": {
        "xMin": -209,
        "xMax": 218,
        "zMin": -413,
        "zMax": 153,
        "surfaceY": 37,
        "properties": "0x00000000",
        "bgCamIndex": 0,
        "lightIndex": 0,
        "room": 0,
        "disabled": false
      },
      "unity": { "x": [-2.09, 2.18], "z": [-1.53, 4.13], "surfaceY": 0.37 },
      "poolFloorY": [0, 0],
      "surfaceMesh": "room DL_0052E0 (XLU shape entry 17) at Y 37, two-layer scroll on segment 8",
      "behavior": "Octorok target pool: En_Syateki_Okuta Init needs this water to survive and spawns at the surface Y 37; the pool is walled off from the player (counter, invisible barrier)"
    }
  ],
  "anchors": [
    { "id": "anchor.tsg.spawn0_from_east", "source": [-10, 0, 390], "unity": [-0.1, 0.0, -3.9], "status": "proposed", "note": "spawn 0, yaw 180, floor poly 33" },
    { "id": "anchor.tsg.door_ta00", "source": [-10, 0, 420], "unity": [-0.1, 0.0, -4.2], "status": "proposed", "note": "TA0 door leaf, yaw 0; exit probe lands on poly 37" },
    { "id": "anchor.tsg.player_station", "source": [-20, 40, 175], "unity": [-0.2, 0.4, -1.75], "status": "proposed", "note": "sTownPlayerPos constant (man.c:188), top tier Y 40" },
    { "id": "anchor.tsg.keeper_a02", "source": [145, 0, 290], "unity": [1.45, 0.0, -2.9], "status": "proposed", "note": "keeper, yaw 270, behind the wall x 80" },
    { "id": "anchor.tsg.octorok_front_mid", "source": [-20, 37, 0], "unity": [-0.2, 0.37, 0.0], "status": "proposed", "note": "a10 grid index 7; actor-list Y is 0, runtime Y is the water surface 37" },
    { "id": "anchor.tsg.octorok_back_mid", "source": [-20, 37, -320], "unity": [-0.2, 0.37, 3.2], "status": "proposed", "note": "a04 grid index 1" },
    { "id": "anchor.tsg.wall_clock_a13", "source": [0, 210, 391], "unity": [0.0, 2.1, -3.91], "status": "proposed", "note": "large wall clock, yaw 180" }
  ],
  "rooms": [
    { "id": "loc.town_shooting_gallery.room_00", "sourceRoom": "Z2_SYATEKI_MIZU_room_00", "header": 0, "roomShape": "cullable, 19 entries (18 opaque, 3 XLU)", "displayLists": 26, "role": "venue room", "collisionLightIndex": 0 }
  ],
  "components": [
    { "sourceKey": "Z2_SYATEKI_MIZU/h0/transition/ta00", "actorId": "0x005", "params": "0x0287", "class": "door-transition", "variant": "posed-skeletal", "dls": ["gameplay_keep:0x22B28 gDoorSkel", "gameplay_keep:0x20BB8 gDoorLeftDL", "gameplay_keep:0x20D00 gDoorRightDL"], "status": "selected", "note": "closed rest pose; schedule type TOWN_SHOOTING_GALLERY (7); leaf side chosen by camera yaw" },
    { "sourceKey": "Z2_SYATEKI_MIZU/h0/room_00/a13", "actorId": "0x19C", "params": "0x907F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected", "note": "large wall clock (type 9), scale 0.02, noon pose" }
  ],
  "collisionOwnership": {
    "room": "Z2_SYATEKI_MIZUCollisionHeader_0013AC (192 polys, 1 water box)",
    "waterBoxes": 1,
    "actorDynapoly": [],
    "actorCylinders": ["a03-a11 Octorok AC cylinders r20 h40 yShift -30 (red +10 r +15 h), active only while shown"],
    "note": "keeper a02 has no collider (colChkInfo.cylRadius 100 only); counter-top pin (floorProperty 8), projectile-passing walls (xp 4/5) and the x 80 keeper wall are collision data, not actor logic"
  },
  "portalIds": [
    "portal.town_shooting_gallery.east_clock_town",
    "portal.town_shooting_gallery.east_clock_town.kickout"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": [
    "a02 En_Syateki_Man (gBurlyGuySkel, HandsOnTable loop, Town head DL, blink and head tracking)",
    "a03-a11 En_Syateki_Okuta (gOctorokSkel appear/float/hide/die, blue material, hit marks)",
    "Link and Tatl"
  ],
  "excluded": {
    "halfDayMask": ["a01"],
    "stateOrSchedule": ["a03", "a04", "a05", "a06", "a07", "a08", "a09", "a10", "a11"],
    "noGeometry": ["a00", "a12", "a14"]
  },
  "unsupported": [
    "skeletal keeper pose",
    "Octorok animation, wave schedule and hit marks",
    "door skeleton rest pose",
    "wall clock live time",
    "water two-layer texture scroll",
    "bow minigame: 20-rupee fee, 75 s timer, 50-point score, high score 39, quiver, heart piece, purple and huge rupees",
    "22:00 kickout and KICKOUT_WAIT deferral",
    "positional gallery BGM and timed-minigame sub-BGM",
    "counter-top pin and projectile-passing barrier walls",
    "floor-selected cameras (CIRCLE3 fixed eye, ROOM1)",
    "player cutscene id chain (csId 0)"
  ],
  "evidence": ["docs/city/interiors/town_shooting_gallery.md"],
  "readiness": {
    "sourceInspection": "pass",
    "objectExtraction": "not_run",
    "import": "not_run",
    "controllerTraversal": "not_run",
    "editorRender": "not_run",
    "packagedRender": "not_run",
    "visualApproval": "not_run"
  }
}
```

Camera proposals (registered IDs, no capture exists; left out of `cameraIds` until captured, as in the accepted school and lottery records): `cam.town_shooting_gallery.entry` at spawn 0 facing -Z (the source camera there is the CIRCLE3 fixed eye (-190, 146, 306), 5.9); `cam.town_shooting_gallery.reverse_door` from the booth toward the door alcove and the wall clock at z 391; `cam.town_shooting_gallery.fixture` = the station (-20, 40, 175) looking -Z at the 3 x 3 Octorok grid and the water, with the ROOM1 camera or the first-person bow view.

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Room visuals: booth, counter, pool basin, backdrop, props | Render the 19 shape entries (room.c:89-109) after the section 9 extraction; capture under `prof.d1.noon.normal` |
| U-02 | Whether the trailing exit `0x0000` is padding | Answered in 2.2 as padding (no reader, list ends at the next block). LOCATION_REGISTER U-02 can drop the Shooting Gallery from its "open" list |
| U-03 | Entrance 1 (`0x3A10`): any sender? | 2.1 shows no sender in `src/` and that it differs from entrance 0 only by the title-card bit. Confirm with a search of every exit list in non-city scenes if the register wants a hard negative |
| U-04 | ZAPD flags for overlay-asset XML (`ovl_En_Syateki_Okuta.xml` has `BaseAddress` and `RangeStart`/`RangeEnd`) and the `interface` XML | Read the decomp `Makefile` asset rule for `assets/xml/overlays/` and mirror it; the section 9 commands are patterns, not verified |
| U-05 | Whether Link can reach the counter top (Y 52) or the keeper alcove (x 80..200) | Route test after import: floor 57 -> 40/41 under the barrier 190/191 (Y 40..167) and the wall 188/189; edge connectivity and step-up limits |
| U-06 | `play->unk_18790(play, -0x8000)` called at the start of a run (man.c:1254) | Identify that function pointer in `z_play.c` / `z_camera.c` (camera yaw snap?) |
| U-07 | Header bounds larger than the vertices (z 502 vs 486; z -413 comes from the unreferenced water corners) | Same as North U-14: how `BgCheck` uses min / max before they are used as region bounds |
| U-08 | Environment light rows 1-3 | Find what selects rows besides the floor's `lightSetting` (`Environment_ChangeLightSetting`, `lightMode` true in `SKYBOX_SETTINGS`) in `z_kankyo.c`; collision always selects row 0 |
| U-09 | Fee text: the message header price field is 20 and the code constants are 20 | Resolved: `msgs.py` shows `firstChoicePrice 20` for `0x3F6` (text header `HEADER(0x0601, ..., 0x0014, ...)`); keep for the record |
| U-10 | Fierce Deity form at the keeper (no intro case, man.c:517-619) | Check whether the form can reach the town (mask restriction); if not, mark unreachable |
| U-11 | Real seconds for the 30-frame start lock and the 15-frame result wait | SOURCE_REGISTER U-01 (frame rate x divisor 3); the 75 s timer itself is real time (`osGetTime`) and does not depend on it |
| U-12 | Why Human talk increments the chest word (man.c:521) | Search for a reader of `Flags_GetAllTreasure` in this scene's actors (none found; only message code `CHEST_FLAGS`); likely harmless scratch use |
| U-13 | `RESTRICTIONS` bButton 1 versus the minigame's B bow | Read the remaining B-button branches in `param.c:1700-2040` for `bButtonAmmoPlusOne` handling during a run |
| U-14 | Which bow draw and release path accepts the shot (B-ammo mode versus C buttons) | Read `player.c:4240-4297`, :13379-13390 and the bow actions for `bButtonAmmoPlusOne` |
| U-15 | Ambience id 19 and spec 5 | Name them from `include/sequence.h:93-94` and the audio tables |
| U-16 | PORTAL_REGISTER section 1 lists entrance flag bits 0-6, 7-13 and 15 only | Add bit 14 (`0x4000`) = scene title card (`z_demo.c:1583-1587`): affects every venue entrance row with `0x4102` versus `0x0102` |

**Wording notes (no edits made):** PORTAL_REGISTER's teleport-only sentence ("their second entrance row is padding or lands on spawn 0") is exact for this scene: it lands on spawn 0 (2.1). LOCATION_REGISTER "Entr. 2 / 2" and "Exits 2 (last 0x0000)" agree with the C; the trailing exit is padding (2.2). The kickout row's text labels `TOWN_SG_1` / `TOWN_SG_2` are the decomp's `TIMETAG_KICKOUT_TEXT_TOWN_SHOOTING_GALLERY_1` / `_2` (ids `0x188F` / `0x1890`). No count in any register disagrees with the C (3 / 15, 1 spawn, 1 transition actor, 2 exits, 2 entrance rows, 2 / 2).

**Not inspected:** room DL contents and combiners (only counts); message text bodies (ids and header fields only); `Camera_Subject1` and the bow draw internals; the Octorok skeleton, animation and effect data; audio sequence content; 3DS material; light-row blending beyond the surface selection.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN sections 6, 11 and 12, LOCATION_REGISTER, PORTAL_REGISTER (all rows for this venue, re-read at `7ab0640`), SOURCE_REGISTER sections 2-4, the East dossier 2.4, the swordsman and lottery dossiers and the swordsman record as worked examples, the frozen schema README and `region.schema.json`, `shared/city/ids.json`, the laundry, milk-bar and trading-post records for water and omissions style; read scene and room C and XML, the scene, actor, object and sequence tables, `z_actor.c`, `z_scene_table.c`, `z_scene.c`, `z_play.c`, `z_demo.c`, `z_camera.c`, `z_camera_data.inc.c`, `z_parameter.c`, `z_sram_NES.c`, `z_schedule.c`, `z_scene_proc.c`, `z_bgcheck.c`, `z_message.c`, `z_face_reaction.c`, `z_player_lib.c`, `z64bgcheck.h`, `z64player.h`, `z64scene.h`, `z64cutscene.h`, `z64save.h`, `z64item.h`, `z64environment.h`, `z64collision_check.h`, the player door and exit code, and the overlays En_Syateki_Man, En_Syateki_Okuta, En_Time_Tag, En_Test4, Obj_Tokeidai, Obj_Sound, En_Door (with its schedule script); scratch Python `col.py` (collision, room DL and texture counts, SHA-256 of five inputs), `geom.py` (point-in-polygon probes and wall groups), `unref.py` (unreferenced vertices), `waves.py` (wave grid counts), `msgs.py` and `msgs2.py` (message header fields and control-code names, no text copied), `check.py` (ASCII, JSON parse and schema-key check of the section 10 block); outputs in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\town_shooting_gallery\`; grep cross-checks (`SCENE_SYATEKI_MIZU`, `TOWN_SHOOTING_GALLERY`, `showTitleCard`, `Entrance_GetTransitionFlags`, `Flags_GetAllTreasure`, `Sram_ClearHighscores`).

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes, `region_from_dossier.py`, `validate_city.py`, `extract_messages.py` itself.

**Next dependency:** the integrator lifts section 10 and runs `validate_city.py`; the section 9 extraction; then a gameplay pass that needs Link's bow and aim, the keeper state machine (5.3-5.7) and the Octorok actors before INT-12 can be called playable.
