# Deku Scrub Playground (`loc.deku_playground`): source inventory dossier

Task **REF-09 / INT-14** (FULL_CITY_PLAN.md section 6 row INT-14, section 12 row REF-09). Base commit `c8b847e` on `main` (the REF-09 brief names `31b20dc`; HEAD observed while writing is `c8b847e`, which did not change the three registers this file cross-checks). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_DEKUTES` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save), room_00, the only room.

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_DEKUTES/Z2_DEKUTES.c`, `room.c` = `.../Z2_DEKUTES_room_00.c`. Overlays live in `src/overlays/actors/ovl_<Name>/z_<name>.c`: `liftnuts.c` = En_Lift_Nuts, `lupylift.c` = Obj_Lupygamelift, `gamelupy.c` = En_Gamelupy, `etcetera.c` = Obj_Etcetera, `warptag.c` = En_Warp_tag, `syokudai.c` = Obj_Syokudai, `tokeidai.c` = Obj_Tokeidai, `test4.c` = En_Test4, `player.c` = `ovl_player_actor/z_player.c`. Line numbers count `\n` only (lone `\r` bytes: 3 in scene.c, 31 in room.c). N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro/function; **OVL** read from the actor overlay source; **ANN** decomp-author comment or symbol name; **INF** inferred (reason given); **NI** not inspected. Message text ids are cited, the text is not transcribed (ROM content); where I name what a text id says, the role comes from reading its message-data preview in scratch (header fields decoded as in the Swordsman dossier, tagged **C-BIN**, text not copied).

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01), `PORTAL_REGISTER.md` (REF-02), `SOURCE_REGISTER.md` (REF-03), `docs/city/districts/north_clock_town.md` section 2.4 (the grotto). Every count in the LOCATION_REGISTER row (1 room, 0 alternates, 4 object banks, 27 actors, no transition command, 2 spawns, 2 exits with last `0x0000`, 2 / 2 entrances) agrees with the C below; the open item U-02 (exit `0x0000`) is answered in 2.3.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_DEKUTES` / `SCENE_DEKUTES`; comment "Deku Scrub Playground" | HDR `include/tables/scene_table.h:66-67` |
| Scene-table index | `0x1E` (the register's "(67)" is the line) | same line |
| Entrance-table scene index | `0x1B` `sDekuScrubPlaygroundEntranceTable` (2 slots, each one layer-0 row); `ENTR_SCENE_DEKU_SCRUB_PLAYGROUND` | HDR `src/code/z_scene_table.c:524-535`, :2600; `include/z64scene.h:620` |
| Title card / draw config / cycle flags | title text `0` (no title card), `SCENE_DRAW_CFG_MAT_ANIM`, `PERSISTENT_CYCLE_FLAGS_NONE` | HDR scene_table.h:67 |
| Restrictions | `RESTRICTIONS_SET(0, 1, 0, 0, 0, 3, 0, 0, 0, 0, 0, 1)`; field order hGauge, bButton, aButton, tradeItems, songOfTime, songOfDoubleTime, invSongOfTime, songOfSoaring, songOfStorms, masks, pictoBox, all | HDR; C `src/code/z_parameter.c:66-71`, assigned :2459-2471 |
| Restriction finding | differs from `RESTRICTIONS_INDOORS` (`0,1,0,0,0,3,0,0,3,0,0,1`, z_parameter.c:75) only in **songOfStorms 0 vs 3**. Decoded: B restricted branch (1), Double Song of Time blocked (3), `all` 1 (C-button items other than quest items, bottles, Ocarina, masks and the pictograph box are disabled, :2158-2170). Song of Time, Soaring and Storms are not restricted. During a run the HUD branch below overrides the buttons | C, INF; HUD `z_parameter.c:1813-1840` |
| XML files | scene `Z2_DEKUTES` segment 2; room `Z2_DEKUTES_room_00` segment 3 with DLs `0x3FB0` (opaque dispatch, 16 calls) and `0x4350` (XLU dispatch, 1 call); no external dependency | XML `assets/xml/scenes/Z2_DEKUTES/Z2_DEKUTES.xml`; HDR room.c:753-775, :813-816 |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene / room headers | **default only**: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene.c:13-29 or room.c:13-21; both entrance rows are layer 0, so no profile selects a layer other than 0 (INF) | HDR |
| Special files | `NAVI_QUEST_HINTS_NONE`, keep object `GAMEPLAY_DANGEON_KEEP`; no room actor uses a dangeon-keep symbol directly; the Obj_Syokudai XML lists `gameplay_dangeon_keep` as an external file (consumer INF) | HDR scene.c:19; XML `object_syokudai.xml:3` |
| Sound | spec 6, ambience 19, seq 59 = `Sequence_59` `NA_BGM_CAVERN`; the run adds sub-BGM `NA_BGM_TIMED_MINI_GAME` | HDR scene.c:14; `include/tables/sequence_table.h:138`; OVL liftnuts.c:817 |
| Skybox | `SKYBOX_SETTINGS(0x00, 0, 0, true)`: external texture file 0, skybox 0, weather 0, `lightMode` true (settings mode, not time of day); room `SKYBOX_DISABLES(false, false)` | HDR scene.c:22, room.c:16; macros `include/z64scene.h:833-837` |
| Env light settings | 4 rows; rows 0 and 1 identical, 2 and 3 differ (values not transcribed). Collision surfaces select index 0 or 31; 31 >= `LIGHT_SETTING_MAX` (31) and is clamped to 0 in settings mode, so rows 1-3 are selected by no floor | HDR scene.c:24, :53-62; C `include/z64environment.h:30`, `z_kankyo.c:2186-2190`, :631 (initial 0), `player.c:11723-11735` |
| Region visited | `SCENE_CMD_SET_REGION_VISITED`; `SCENE_DEKUTES` is in the `REGION_CLOCK_TOWN` list | HDR scene.c:16; C `src/code/z_scene.c:504-530`, `src/code/z_inventory.c:435` |
| Paths | 20 paths (scene.c:200-221): 18 lift rails (6 per day) and 2 one-point scrub waypoints; table in 4.4 | HDR scene.c:20, :64-221 |
| Actor cutscenes / cs cameras | 11 entries, 2 `FIXED1` cameras; no `SCENE_CMD_CUTSCENE_SCRIPT_LIST`, so no scripted cutscene | HDR scene.c:25-26, :223-235, :245-257 |
| Minimap | 1 entry `{0xFFFF, 0, 0, 0, 0}`, list value 0 | HDR scene.c:237-243 |
| Animated materials | one entry `{-1, 1, params}`: segment `abs(-1) + 7 = 8`, type 1 two-texture scroll, params rows `{1, 0, 16, 64}` and `{-1, 0, 16, 64}`; a negative segment ends the list | HDR scene.c:259-266; C `src/code/z_scene_proc.c:388-407` |
| Transition actors | none (no `SCENE_CMD_TRANSITION_ACTOR_LIST`) | HDR |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x00, 0x00, 0, 0, false, 0)` = `ROOM_TYPE_NORMAL`, `ROOM_ENV_DEFAULT`, storm 0; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3); cullable shape with 17 entries; 4 objects; 27 actors | HDR room.c:14-21; `include/z64scene.h:314-330`, :804-807 |
| Persistence | no persistent cycle flags; the venue's own state is `weekEventReg[14]` bits and `dekuPlaygroundHighScores` (5.5, 5.6) | C |

### 1.1 Alternate headers

None. Every profile (`prof.d1.noon.normal`, `prof.d3.night.normal`, ...) selects the default scene and room header. All day variation is per actor: half-day masks on the 18 lift entries (4.3) and the time test in En_Lift_Nuts (5.1).

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:32-37; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`); yaw `(rawRotY >> 7) & 0x1FF`; csId `rawRotY & 0x7F`. Both entries have raw rot X / Y / Z `0x0007 / 0x5A00 / 0x007F`: yaw 180 (faces -Z), csId 0.

| Spawn | Position (Unity) | Params | Start mode | bgCam | Used by |
| --- | --- | --- | --- | --- | --- |
| 0 | (-28, 400, 1939) (-0.28, 4.00, -19.39); floor under it is poly 131 at Y 40, so the drop is 360 u | 0x0D00 | 0xD `PLAYER_START_MODE_D` | 0 | North grotto `0x3600` (entrance 0). 71.1 u from the exit tag a16 (-24, 40, 2010), which needs 30 u |
| 1 | (-1, 40, 941) (-0.01, 0.40, -9.41); floor poly 156 (start platform, Y 40) | 0x0DFF | 0xD | 0xFF (none) | the minigame reload `ENTRANCE(DEKU_SCRUB_PLAYGROUND, 1)` (`liftnuts.c:905`, 5.4). 97.6 u from scrub a14, 99.4 u from a15 |

- **Start mode D** (C): `Player_StartMode_D` calls `func_8083A878(play, this, 180.0f)`, which (no water here) starts `Player_Action_35`, an auto-walk toward the point 180 u ahead along the spawn yaw, and sets `PLAYER_STATE1_20000000` (`player.c:8677-8681`, :8536-8556, :6315-6327; table entry :11069). Effect with the player halted at spawn 1 (5.4): U-08.
- **bgCam** (C): `Play_Init` calls `Camera_ChangeActorCsCamIndex(mainCamera, bgCam)` unless the value is `0xFF` (`src/code/z_play.c:2325-2327`); without `CAM_DATA_IS_BG` the index selects the actor cs camera list (`z_camera.c:767-777`, :783-789). Spawn 0 therefore starts with `actorCsCamList[0]` = setting 31 `CAM_SET_FIXED1` (5.9); spawn 1 has no arrival camera (floor camera data applies).
- **csId 0** (C): `Play_AssignPlayerCsIdsFromScene(play, 0)` (`player.c:11261`; `z_play.c:2031-2059`) walks `additionalCsId` 0 -> 1 -> 2 -> ... -> 8 (scene.c:224-232); entries 0-8 carry the nine global cs cameras `-3, -2, -4, -5, -7, -11, -8, -9, -6`, which map to the player cs ids Ocarina, item get, bottle, item show, moon pad, mask transformation, death, revive, song warp. `PLAYER_CS_ID_WARP_PAD_ENTRANCE` (camera -16) is not present and stays `CS_ID_NONE`.

### 2.2 Entrance table to room (C)

| Entr | Table row (`z_scene_table.c`) | Scene row (scene.c:43-46) | Spawn / room | Sender |
| --- | --- | --- | --- | --- |
| 0 | `{ SCENE_DEKUTES, 0, 0x0102 }` :525 | `{0x00, 0x00}` | spawn 0, room 0 | North `DOOR_ANA` exit [4] `0x3600` (PORTAL_REGISTER); developer map select (`ovl_select/z_select.c:427`, excluded) |
| 1 | `{ SCENE_DEKUTES, 1, 0x0102 }` :529 | `{0x01, 0x00}` | spawn 1, room 0 | En_Lift_Nuts only: `ENTRANCE(DEKU_SCRUB_PLAYGROUND, 1)` = `0x3610` (`liftnuts.c:905`) |

Flags `0x0102`: exit fade `0x02`, arrival `(0x0102 >> 7) & 0x7F = 0x02`, bit 15 clear (decode per PORTAL_REGISTER section 1). The city scenes use `0x4102`; bit 14 meaning NI. All `ENTRANCE(DEKU_SCRUB_PLAYGROUND, n)` call sites in `src/`: `liftnuts.c:905` and `z_select.c:427` (C, grep). Exit lists of the 22 extracted city scenes carry `0x3600` only in North (default `Z2_BACKTOWN.c:77`, alternate :949) and `0x3610` nowhere (C, grep); scenes outside the 22 are NI (U-09).

### 2.3 Exit list (HDR scene.c:48-51) and consumers

| Exit idx | Raw | Decode (`>> 9`, `(>> 4) & 0x1F`, `& 0xF`) | Destination | Consumer |
| --- | --- | --- | --- | --- |
| 0 | `0xD640` | 0x6B `Z2_BACKTOWN`, entrance 4, layer 0: `sNorthClockTownEntrance4[0] = { SCENE_BACKTOWN, 4, 0x4102 }` (`z_scene_table.c:2322-2324`, row 2323) | `loc.north_clock_town` spawn 4 (-972, 292, -1702) yaw 119 params 0x04FF = mode 4 `PLAYER_START_MODE_GROTTO`, bgCam 0xFF (`Z2_BACKTOWN.c:47-48`) | a16 `EN_WARP_TAG` `0x83C0`: exit index `0x83C0 & 0x3F = 0` (`warptag.c:241`, header :27) |
| 1 | `0x0000` | n/a | none | **padding** (C): none of the 7 surface types has exit bits (every `exitPlus1` is 0, 7.1), no transition actor exists, and the only exit-list reader in the room is a16 with index 0. The list spans `0xAC..0xB0` (4 bytes) around one live u16; the register's "last `0x0000`" is alignment (answers LOCATION_REGISTER U-02 for this scene) |

The player's collision exit check (`player.c:6405-6416`) therefore never fires here: **the only way out is the tag**.

### 2.4 Transition actors and room links

None: no transition-actor command (HDR). One room, so no internal room portal exists or is proposed.

### 2.5 Access from North Clock Town: cross-check with `north_clock_town.md` 2.4 and PORTAL_REGISTER

| Item | North dossier / PORTAL_REGISTER | This scene's data | Result |
| --- | --- | --- | --- |
| Trigger in | `DOOR_ANA` `0x0304` (type `0x300` visible scene exit, exit index 4, csId 13) at (-966, 292, -1703) in the default header, (-970, 292, -1676) in the alternate (`Z2_BACKTOWN_room_00.c:52`, :240); `z_door_ana.c:126-162` (nextEntrance at :135-138) | no door actor here | agree |
| Day, time, form gate on the hole | PORTAL_REGISTER: none stated | `DoorAna_WaitOpen` tests only distance (xz <= 20, height -50..15), not in a cutscene, not swimming (`z_door_ana.c:130-162`); mask `0x3FF` (all half-days, North dossier 4.2) | **open every half-day, any form**; Human noon works |
| Arrival | entrance 0 -> spawn 0 (-28, 400, 1939) yaw 180 mode 0xD (PORTAL_REGISTER row `portal.north_clock_town.deku_playground`) | 2.1, 2.2 | agree |
| Trigger out | `EN_WARP_TAG 0x83C0` "room_00:96" | a16 at (-24, 40, 2010), params `0x83C0`: `& 0x3C0 = 0x3C0` (`WARPTAG_3C0_MAX`), sign bit set = invisible grotto-return variant, exit index 0 (`warptag.h:25-30`, `warptag.c:56-62`). **The register's "96" is the source line (room.c:96); the actor index is 16** (component key `Z2_DEKUTES/h0/room_00/a16`) | agree; naming note |
| Return | North spawn 4 (-972, 292, -1702) yaw 119 mode 4 (row `portal.deku_playground.north_clock_town`) | 2.3 | agree. The register cites the closing brace (L2324) of `sNorthClockTownEntrance4`; the row is line 2323 |
| Exit trigger geometry | tag fires when `xzDistToPlayer <= 30` and `playerHeightRel <= 10` and the player is not in a cutscene mode (`warptag.c:96-107`) | tag stands on cave floor poly 130 (Y 40), 71 u beyond spawn 0 on the +Z side; arrival faces -Z, away from it | no bounce on arrival |
| Exit sequence | none stated | `Player_SetCsActionWithHaltedActors(PLAYER_CSACTION_81)`, then `EnWarpTag_GrottoReturn`: after 10 frames `nextEntrance = setupExitList[0]`, exit fade, BGM and ambience muted (`warptag.c:231-249`); csId of a16 is `0x7F` = none | C |
| Foot access to the hole in North | North U-03 UNKNOWN | not this scene | dependency stays with North |

### 2.6 Portal IDs (registered names, `shared/city/ids.json`)

| Portal ID | Mode | Source evidence | Arrival |
| --- | --- | --- | --- |
| `portal.north_clock_town.deku_playground` (incoming, owned by North) | special | North `DOOR_ANA` a00, exit 4 `0x3600` | spawn 0 |
| `portal.deku_playground.north_clock_town` | special | a16 `EN_WARP_TAG`, exit 0 `0xD640` | North spawn 4 |
| (not a portal) minigame reload | same-scene reload | a14/a15 `EnLiftNuts_EndGame`, `ENTRANCE(DEKU_SCRUB_PLAYGROUND, 1)`; `respawnFlag -2` | spawn 1 |

`portalIds` in the record is exactly the one portal departing this venue (`shared/city/ids.json:577-581`). PORTAL_REGISTER section 6 lists "Deku Playground 1" as teleport-only: it is reached by the in-scene reload above (the integrator may register a `.reload` row; not done here). Section 6 of that register is otherwise right: no exit of any city scene lands on spawn 1.

## 3. Object banks (HDR room.c:24-29; IDs from `include/tables/object_table.h`)

| # | Bank | Object ID | Consumer (OVL) | Baserom bytes | Needed for Day 1 static |
| --- | --- | --- | --- | --- | --- |
| 0 | `OBJECT_RAILLIFT` | 0x163 | Obj_Lupygamelift profile object (`lupylift.c:27`): DL `object_raillift_DL_0071B8` (0x71B8), collision `object_raillift_Colheader_0048D0` (0x48D0) | 32,432 | yes |
| 1 | `OBJECT_DNT` | 0x1E5 | En_Lift_Nuts (`liftnuts.c:56`): `gBusinessScrubSkel` (flex, 0xAC70), 19 animation-info rows (`liftnuts.c:64-108`) | 45,776 | pose only |
| 2 | `OBJECT_SYOKUDAI` | 0x080 | Obj_Syokudai: `gObjectSyokudaiTypeNoSwitchDL` (0x870) | 15,504 | yes |
| 3 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai wall clock (type 9) | 72,832 | yes (already extracted, South) |

Implicit (not in the list): `gameplay_keep` (599,520 bytes) for Obj_Etcetera (`gPinkDekuFlowerCol` 0xE710, `gPinkDekuFlowerIdleDL` 0xED80, `gPinkDekuFlowerSkel`, bounce 0xEB7C and rustle 0x117A8 animations), En_Gamelupy (`gRupeeDL` 0x622C0, `gRupeeGreenTex` 0x61FC0, `gRupeeBlueTex` 0x61FE0), the torch flame (`gEffFire1DL` 0x7D590), En_Warp_tag and En_Test4 profiles; and `gameplay_dangeon_keep` (the scene keep object, 144,000 bytes). Offsets from `assets/xml/objects/gameplay_keep.xml:809-838, :1300-1306, :1429` (XML).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- Entry id `raw & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF`, converted to binang unless the id carries the axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), in which case it is an integer, values over 180 wrapped by -360 (`Actor_SpawnEntry`, z_actor.c:3628-3655). All 18 lift entries carry `0x2000`, so rotZ arrives as an integer and is the lift's speed input (4.3). csId `rawY & 0x7F` (0x7F none, :3570). Half-day mask `((rawX & 7) << 7) | (rawZ & 0x7F)`, 0 = all (:2562-2564, :3576-3580); bits `DAY1_DAWN` 0x080 (daytime), `DAY1_NIGHT` 0x040, `DAY2_DAWN` 0x020, `DAY2_NIGHT` 0x010, `DAY3_DAWN` 0x008, `DAY3_NIGHT` 0x004 (`include/z64actor.h:328-337`); the current bit is `HALFDAYBIT_DAY0_DAWN >> (day * 2 + night)` (:2491-2498). Spawn test :2567-2572. A child spawned with a parent inherits the parent's mask (:3597); a plain `Actor_Spawn` gets `HALFDAYBIT_ALL` (:3576-3580).
- Entries `{ACTOR_ID, pos, {rotX, rotY, rotZ}, params}`; entry `i` of room.c starts on line `32 + 4i` (room.c:32-139).
- Switch flags: `Flags_GetSwitch(flag)` indexes `switches[flag >> 5]`; flags 0x40-0x42 live in `switches[2]` (flags 64-95), zeroed at scene init (`z_actor.c:2508-2523`) but restored from `respawn[RESPAWN_MODE_DOWN].tempSwitchFlags` by a respawn load (`player.c:11295`), which is how the run result survives the reload (5.4).

Classes as in North; "D1d" = mask includes Day 1 daytime.

### 4.2 room_00 default header (27 entries, HDR room.c:31-139, decode C; scratch `rooms.py`)

| # | Line | Actor (table ID) | Position N64 | Raw rot X / Y / Z | Mask | CS | Params | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 32 | `ACTOR_OBJ_ETCETERA` (0x183) | (90, 40, 901) | 0x0007 / 0x007F / 0x007F | 0x3FF | none | 0x007F | static-mesh (Deku flower, dynapoly) | yes |
| 1 | 36 | `ACTOR_OBJ_ETCETERA` | (-90, 40, 901) | same | 0x3FF | none | 0x007F | static-mesh | yes |
| 2-7 | 40-60 | `ACTOR_OBJ_LUPYGAMELIFT \| 0x2000` (0x1CD) | 4.3 | rotX 0, rotZ low 7 bits 0x30 | **0x030** (Day 2) | none | 0x8000-0x9005 | static-mesh (moving platform, dynapoly) | no |
| 8-13 | 64-84 | `ACTOR_OBJ_LUPYGAMELIFT \| 0x2000` | 4.3 | rotX 1, rotZ low 7 bits 0x40 | **0x0C0** (Day 1) | none | 0x8008-0x900D | static-mesh | yes |
| 14 | 88 | `ACTOR_EN_LIFT_NUTS` (0x1C9) | (-90, 40, 901) | 0x0007 / 0x007F / 0x007F | 0x3FF | none | 0x06FF (path 6) | posed-skeletal NPC (employee scrub) | yes |
| 15 | 92 | `ACTOR_EN_LIFT_NUTS` | (90, 40, 901) | same | 0x3FF | none | 0x07FF (path 7) | posed-skeletal NPC | yes |
| 16 | 96 | `ACTOR_EN_WARP_TAG` (0x116) | (-24, 40, 2010) | 0x0007 / 0x007F / 0x007F | 0x3FF | none | 0x83C0 | controller-invisible (exit trigger) | yes |
| 17 | 100 | `ACTOR_EN_TEST4` (0x15A) | (-23, 1550, 1999) | 0x0007 / 0x0009 / 0x007F | 0x3FF | 9 | 0x0100 | controller-invisible | yes |
| 18-23 | 104-124 | `ACTOR_OBJ_LUPYGAMELIFT \| 0x2000` | 4.3 | rotX 0, rotZ low 7 bits 0x0C | **0x00C** (Day 3) | none | 0x800E-0x8013 | static-mesh | no |
| 24 | 128 | `ACTOR_OBJ_SYOKUDAI` (0x039) | (-60, 40, 1440) | 0x0007 / 0x007F / 0x007F | 0x3FF | none | 0x287F | static-mesh + billboard flame | yes |
| 25 | 132 | `ACTOR_OBJ_SYOKUDAI` | (60, 40, 1083) | same | 0x3FF | none | 0x287F | static-mesh + billboard flame | yes |
| 26 | 136 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (0, 260, -741) | same | 0x3FF | none | 0x907F | static-mesh (wall clock) | yes |

All rotations decode to 0 except the lifts' rotZ (integer speed input, 4.3) and a17's csId 9 (rotY). Unity: a00/a01 (0.90 / -0.90, 0.40, -9.01); a14/a15 same; a16 (-0.24, 0.40, -20.10); a17 (-0.23, 15.50, -19.99); a24 (-0.60, 0.40, -14.40); a25 (0.60, 0.40, -10.83); a26 (0.00, 2.60, 7.41).

### 4.3 The 18 lift entries (OVL `lupylift.h:6-8`: path index `p & 0x7F`, start point `(p >> 7) & 0x1F`, rupee colour `(p >> 12) & 1`)

Speed = `|home.rot.z| * 0.1` (`lupylift.c:59-61`) with rotZ the wrapped integer (raw 0x12C -> -60 -> 6.0). Start point is 0 for all. "Pt 0" is the first waypoint the lift moves toward.

| # | Day (mask) | Home N64 (Unity) | Params | Path | Rupee | Speed | Pt 0 vs home |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 2 | 2 (0x030) | (-100, -50, -50) (-1.00, -0.50, 0.50) | 0x8000 | 0 | green | 6.0 | same |
| 3 | 2 | (300, -50, -250) | 0x8001 | 1 | green | 8.0 | same |
| 4 | 2 | (500, -50, 550) | 0x8002 | 2 | green | 10.0 | same |
| 5 | 2 | (100, -50, 150) | 0x8003 | 3 | green | 6.0 | same |
| 6 | 2 | (-300, -50, 350) | 0x8004 | 4 | green | 8.0 | same |
| 7 | 2 | (-500, -50, -450) | 0x9005 | 5 | **blue** | 10.0 | same |
| 8 | 1 (0x0C0) | (-100, -200, 150) (-1.00, -2.00, -1.50) | 0x8008 | 8 | green | 6.0 | 40 u below pt 0 (-160) |
| 9 | 1 | (100, 0, -50) (1.00, 0.00, 0.50) | 0x8009 | 9 | green | 6.0 | same |
| 10 | 1 | (-200, 40, -250) (-2.00, 0.40, 2.50) | 0x800A | 10 | green | 6.0 | same |
| 11 | 1 | (200, -161, 350) (2.00, -1.61, -3.50) | 0x800B | 11 | green | 6.0 | 1 u below pt 0 |
| 12 | 1 | (-500, -210, 350) (-5.00, -2.10, -3.50) | 0x800C | 12 | green | 6.0 | 50 u below pt 0 |
| 13 | 1 | (500, 90, -250) (5.00, 0.90, 2.50) | 0x900D | 13 | **blue** | 6.0 | same |
| 18 | 3 (0x00C) | (-100, 50, -50) | 0x800E | 14 | green | 6.0 | same |
| 19 | 3 | (100, 50, 150) | 0x800F | 15 | green | 6.0 | same |
| 20 | 3 | (-300, -150, 250) | 0x8010 | 16 | green | 6.0 | same |
| 21 | 3 | (300, 50, -150) | 0x8011 | 17 | green | 6.0 | same |
| 22 | 3 | (-500, -150, -450) | 0x9012 | 18 | **blue** | 12.0 | 110 u below pt 0 (-40) |
| 23 | 3 | (500, -150, 550) | 0x8013 | 19 | green | 12.0 | 110 u below pt 0 |

Each day has exactly 6 lifts, 5 green and 1 blue rupee (the six-rupee total is the win condition, 5.4). Day 1 = six vertical shuttles scattered over x -500..500, z -250..350. Day 2 = three concentric rectangular rails at Y -50, two counter-rotating lifts on each. Day 3 = a small rail at Y 50 (2 lifts), two vertical shuttles and a fast big rail at Y -40 (2 lifts, speed 12).

### 4.4 Paths (HDR scene.c:64-221, `Path {count, additionalPathIndex -1, customValue -1, points}`)

| Path | Count | Points (N64) | Used by (entry) | Nominal frames per lap (INF) |
| --- | --- | --- | --- | --- |
| 0 / 3 | 5, 5 | rectangle x +-100, z -50..150, Y -50; path 3 is path 0 reversed | 2 / 5 (Day 2) | 800 u at 6.0 = 133 |
| 1 / 4 | 5, 5 | rectangle x +-300, z -250..350, Y -50; reversed | 3 / 6 (Day 2) | 2400 u at 8.0 = 300 |
| 2 / 5 | 5, 5 | rectangle x +-500, z -450..550, Y -50; reversed | 4 / 7 (Day 2) | 4000 u at 10.0 = 400 |
| 6 / 7 | 1, 1 | (-42, 40, 900) / (40, 40, 900) | scrubs a14 / a15 waypoints | n/a |
| 8-13 | 3 each | vertical there-and-back: x/z (-100,150), (100,-50), (-200,-250), (200,350), (-500,350), (500,-250); Y -160..0, 0..-160, 40..-160, -160..40, -160..90, 90..-160 | 8-13 (Day 1) | 320, 320, 400, 400, 500, 500 u at 6.0 = 53-83 |
| 14 / 15 | 5, 5 | rectangle x +-100, z -50..150, Y 50; reversed | 18 / 19 (Day 3) | 800 u at 6.0 = 133 |
| 16 / 17 | 3, 3 | vertical (-300, 250) Y -150..50; (300, -150) Y 50..-150 | 20 / 21 (Day 3) | 400 u at 6.0 = 67 |
| 18 / 19 | 5, 5 | rectangle x +-500, z -450..550, Y -40; reversed | 22 / 23 (Day 3) | 4000 u at 12.0 = 333 |

Frames are per actor update (rate NI, SOURCE_REGISTER U-01); the 30 u ease-down to speed 5 at each waypoint (`lupylift.c:136-163`) is ignored. The path-point lists are scene.c:64-198; path records :200-221.

### 4.5 Overlay findings per actor type (OVL)

| Actor | Init, pose, children | Draw | Presence / state at Day 1 noon, fresh save |
| --- | --- | --- | --- |
| Obj_Etcetera (a00, a01; also spawned x6) | type `(p & 0xFF80) >> 7` = 0 pink (all room entries and all children are pink); scale (0.01, 0.02, 0.01); dynapoly `gPinkDekuFlowerCol`; AC cylinder r20 h14; waits for gameplay_keep in `ObjEtcetera_Setup` (`etcetera.c:69-97`, :241-319, header :7-16) | no actor draw until set: idle `gPinkDekuFlowerIdleDL` (0xED80); skeleton and animations only while rustling or bouncing (:342-364) | present, idle (burrow flag clear) |
| Obj_Lupygamelift (a08-a13 at noon) | scale (0.1, 0.15, 0.1); dynapoly `DYNA_TRANSFORM_POS` (carries the player and anything on its floor, `src/code/z_bg_collect.c:88-117`, `z_actor.c:1695-1697`); spawns child **Obj_Etcetera** (params 0) at its position and an **En_Gamelupy** at its **home** position, then waits 5 frames and moves; the child's position is copied every frame; speed eases to 5 within 30 u of a waypoint (`lupylift.c:42-84`, :118-163) | `object_raillift_DL_0071B8` (:171-173); flags update/draw culling disabled (:11) | present: 6 lifts at their home pose for the first 5 frames |
| En_Gamelupy (spawned x6) | params = colour (0 green, 1 blue); scale 0.03, gravity -0.5, OC cylinder r10 h30 player-only; finds the shared score cell, then idles spinning; on player overlap adds **50 points** and 1 rupee (green) or 5 (blue), scales away over 30 frames (`gamelupy.c:64-83`, :112-135, :144-158, header :8) | `gRupeeDL` with the green or blue texture | present from scene load; its score cell exists only once an En_Lift_Nuts has run Init (:112-121) |
| En_Lift_Nuts (a14, a15) | flex skeleton `gBusinessScrubSkel`; cylinder r25 h75; path index `(p & 0xFF00) >> 8` -> single waypoint; shared score cell and state variables are overlay statics; switch 0x41 clear: state NONE, `IdleHidden` with the burrow pose (`liftnuts.c:114-132`, :269-317) | always drawn; limbs hat, hair, scalp, both bag limbs nulled; eye DL by text id (:1076-1107) | present, burrowed; rises when the player is within 100 u after 40 frames, burrows again beyond 120 u (:332-338, :378-385) |
| En_Warp_tag (a16) | `WARPTAG_GET_INVISIBLE` -> `EnWarpTag_WaitForPlayer`; no skeleton (`warptag.c:50-74`) | profile draw NULL | present, invisible |
| En_Test4 (a17) | stars `(p >> 10) * 100 = 0`, bell volume index `(p >> 5) & 0xF = 8` (`test4.h:8-9`); csId list `[night] = 9`, `[day] = additionalCsId of entry 9 = 10` (`test4.c:322-337`; scene.c:233-234) | NULL | always; three-day controller (dusk/dawn cuts, bells, midnight DayTelop) |
| Obj_Syokudai (a24, a25) | `0x287F`: type `>> 12` = 2 `NO_SWITCH`, start-lit bit 0x800 set, group size 0, switch flag 0x7F none; `snuffTimer = -1` (never) at init; point light at Y +70, radius 250 (`syokudai.c:88-110`, header :8-13, :58, :56) | `gObjectSyokudaiTypeNoSwitchDL` + `gEffFire1DL` flame billboard scale 0.0027 (`syokudai.c:288-320`) | present, lit; the room has no water box that could snuff it |
| Obj_Tokeidai (a26) | type `(p & 0xF000) >> 12` = 9 `WALL_CLOCK`; scale 0.02; clock time = `CURRENT_TIME` every frame (`tokeidai.c:247-252`, :719-723) | minute ring 0xCF28, clock center and hand 0xBEE8, `gWallClockClockFaceDL` 0xF518, sun and moon panel 0xC368 (`tokeidai.c:811-842`) | present; noon pose shows 12:00 |

### 4.6 Counts

| | Value |
| --- | --- |
| Objects / actors (HDR) | 4 / 27 (matches LOCATION_REGISTER) |
| By mask | 9 all-day (a00, a01, a14-a17, a24-a26), 6 Day 1, 6 Day 2, 6 Day 3 |
| Setup actors at Day 1 noon | 15 (9 + 6); runtime-spawned 12 (6 child flowers, 6 rupees) |
| Drawn at rest at noon | 25: 8 flowers (2 start + 6 lift children), 6 lifts, 6 rupees, 2 scrubs, 2 torches (+2 flames), 1 clock; invisible controllers 2 (a16, a17) |
| Excluded by mask at noon | 12 (a02-a07, a18-a23) |
| Day 2 / Day 3 noon | their own 6 lifts replace the Day 1 set (+12 spawned each); the 9 all-day actors stay |

## 5. Venue mechanics index (INT-14 scope: Deku flower platform game)

Index and citations only; no Unity design. "Deku Scrub Playground" is the whole scene: a pit crossed on moving platforms, with the employee scrubs, the rupee score, the timer, the reward and the form gate listed below.

### 5.1 What gates play (the venue has no door)

| Gate | Value | Evidence |
| --- | --- | --- |
| Door / open hours | none: no door actor, no schedule; the grotto hole is the only entrance and is open every half-day (2.5). The scene is enterable 24 h | C (2.5) |
| Day | Days 1-3 each have their own lift set, record and win flag; `CURRENT_DAY = save.day % 5` (`z64save.h:518`). Switch cases cover 1, 2, 3 only | OVL `liftnuts.c:389-441`, :482-500; mask 4.3 |
| Time | play is refused when `CURRENT_TIME > CLOCK_TIME(23, 30)` (64170) **or** `<= CLOCK_TIME(6, 0)` (16384) on every day: **playable 06:00 < t <= 23:30** (text `0x27F7`, then the scrub burrows). Day 1 12:00 (32768) is open | OVL liftnuts.c:391, :404, :425, :712-721; `include/macros.h:23` |
| Form | **Deku only**: `GET_PLAYER_FORM == PLAYER_FORM_DEKU` at talk time; any other form gets text `0x27D8` (refusal) and the scrub burrows | OVL liftnuts.c:386-387, :502-505; `z64save.h:526` |
| Form (movement) | the Deku flower burrow and launch tests `transformation == PLAYER_FORM_DEKU` at both call sites, so no other form can use a flower; the 1350 u gap from the start platform to the far platform is not walkable (INF) | C `player.c:7090-7100`, :8228-8231, :6980-6993 |
| Fee | **10 rupees** per run (hardcoded; text `0x27E2` has no price field in its header, C-BIN): "Yes" with >= 10 rupees -> `Rupees_ChangeBy(-10)` and `0x27E5`; with fewer, error sfx and `0x27E4` | OVL liftnuts.c:517-541 |
| Earlier win today | changes the text only (`0x27D9/0x27DB/0x27DE` "already won" versus `0x27DA/0x27DC/0x27DD` first attempt), never blocks play | OVL liftnuts.c:394-440 |
| All three days won | `GetNumDaysWon() == 3`, Deku form, player within 150 u: the scrubs hide permanently (`0x27F6` once) and the game is no longer offered (until the cycle resets the flags) | OVL liftnuts.c:230-267, :980-1008 |
| Scene-level | `RESTRICTIONS` blocks C-button items and Double Song of Time (section 1); the run's HUD overrides it (5.8) | C |

### 5.2 Cast and fixtures

| Fixture | Where | Role |
| --- | --- | --- |
| Employee scrubs a14 (left, path 6) and a15 (right, path 7) | burrowed in the two start flowers (-90 / 90, 40, 901) | conversation, fee, rules, result, reward; both exist, but after a run only the first to Init (a14) takes the result talk (`sIsAutotalkOn` static, liftnuts.c:162-193, :307-316) |
| Start flowers a00, a01 | same two spots | where the run begins: the player burrows (A) and launches |
| Lift flowers (6 spawned) | on each lift | landing and relaunch points |
| Rupees (6 spawned) | at each lift's home position (not at its moving position); carried afterwards only if the rupee rests on the lift's floor (INF, U-03) | score |
| Start platform, tunnel, cave | 7.2 | entry and safe ground (floor Y 40) |
| Pit floor Y -260 and the side ramps | 7.2 | failure surface (`y < 20` on static floor) |
| Wall clock a26, torches a24/a25 | far wall, tunnel | props; the clock shows live time |

### 5.3 Conversation flow (talk accepted, `EnLiftNuts_Idle`, liftnuts.c:386-506)

Talk offer: 100 u and `playerHeightRel >= -13`, only while no autotalk is active (:507-513). Texts by id; roles from C-BIN previews.

| Condition (in this order) | Text | Next |
| --- | --- | --- |
| not Deku | `0x27D8` | any choice ends in burrow (:552-554); text done -> burrow (:712-721) |
| Deku, state NONE, time closed | `0x27F7` | burrow |
| Deku, state NONE, open | Day 1: won -> `0x27D9`, else `0x27DA`. Day 2: won -> `0x27DB`; else won D1 -> `0x27DC`; else `0x27DD`. Day 3: won -> `0x27DE`; else 2 won -> `0x27DF`; 1 won -> `0x27E0`; else `0x27E1` | event handler chains every one of these to `0x27E2` (:564-575) |
| `0x27E2` choice | Yes (index 0): >= 10 rupees -> `0x27E5`; < 10 -> `0x27E4` -> burrow. No -> `0x27E3` -> burrow | :517-541, :577-580 |
| `0x27E5` closes | textbox closed, player halted (`PLAYER_STATE1_20`), `EnLiftNuts_SetupMove`: autotalk on, state STARTING; the scrub walks to its waypoint at speed 2, then the player is auto-walked to the scrub's `home.pos` (stick magnitude 80 at >= 30 u, 40 at 5-30 u, 10 under 5 u) until within 5 u | :582-586, :757-814 |
| state STARTING, talk (auto-accepted, range 200) | first time this visit (`switch 0x42` clear): rules `0x27E6`, then the day text `0x27E7/8/9`; otherwise the day text directly. `0x27E7-9` announce the day's best time (control codes 0xFA-0xFC, `message_data_fmt_nes.h:103-105`); then `0x27FA` -> textbox closed, player released, `SetupStartGame` | :477-501, :588-622 |
| state AFTER (post-run, 5.4) | see 5.4 | :446-475 |
| orphan | `case 0x238D` ("Yes" -> `SetupStartGameImmediately`) has no `Message_StartTextbox(0x238D)` in the overlay and no message-table entry (C-BIN header scan): unreachable unless message data jumps to it (U-01) | :543-550 |

### 5.4 The run, its end conditions and the reload

| Step | Rule | Evidence |
| --- | --- | --- |
| Start | `EnLiftNuts_StartGame` waits for `player->stateFlags3 & PLAYER_STATE3_200` (launching out of a flower), then: `SET_EVENTINF(EVENTINF_34)`, `Interface_StartTimer(TIMER_ID_MINIGAME_2, 0)` (count up from 0), state RUNNING, sfx `NA_SE_SY_FOUND`; the scrub's attention flag is cleared while RUNNING | OVL liftnuts.c:816-832, :1071-1073; `z_parameter.c:836-852` |
| Per frame | three tests in `EnLiftNuts_RunGame` (below) | OVL liftnuts.c:858-879 |
| **Fail: fell** | player on the ground, `floorBgId == BGCHECK_SCENE` (static floor, not a lift or flower) **and** `pos.y < 20`: the pit floor (Y -260) and all but the top 20 u of the side ramps. The start and far platforms (Y 40) and the lifts do not count | liftnuts.c:861-863; `include/z64bgcheck.h:24-26` |
| **Fail: time** | `timerCurTimes >= 120 s` (12000 centiseconds; the HUD also caps the value at 12000 in this scene) | liftnuts.c:863; `z_parameter.c:6009-6011`, :5899-5901 |
| **Success** | shared score == `ENGAMELUPY_POINTS * 6` = **300** (all six rupees): if `timerCurTimes < dekuPlaygroundHighScores[day - 1]` set switch 0x40 (new record) | liftnuts.c:869-878; `gamelupy.h:8` |
| End | on either path: `Flags_SetSwitch(0x41)`, `player` halted, sfx, timer state 6 (stop, minigame checks) | liftnuts.c:864-866, :876-886; `z64save.h:111` |
| Result text (frame 10) | `time > record` -> `0x27EA` (time up); else score 300 -> `0x27F8`; else `0x27EC` (fell) | liftnuts.c:888-902 |
| Reload (frame 30) | `CLEAR_EVENTINF(EVENTINF_34)`; `respawn[DOWN].entrance = ENTRANCE(DEKU_SCRUB_PLAYGROUND, 1)` (`0x3610`); `nextCutsceneIndex = 0`; `func_80169EFC` (saves `switches[2]` into the respawn data and loads that entrance); `respawnFlag = -2`; `transitionType = TRANS_TYPE_64`, next fade black | liftnuts.c:903-911; `z_play.c:1918-1927`; `player.c:11263-11295` |
| After the reload | scene starts at **spawn 1**; switch 0x41 is set again, so the first scrub to Init (a14) takes state AFTER, halts the player, sets autotalk and rises; a15 stays hidden | liftnuts.c:304-316; `player.c:11295` |
| AFTER talk | switch 0x40 set -> clear it, `Inventory_SaveDekuPlaygroundHighScore` (record time and the file name into the day's slot), then `0x27F4` if the player has won Days 1 and 2 and it is Day 3, else `0x27EE`. Switch 0x40 clear: `time >= record` and `time < 120 s` -> `0x27F9` (then `0x27ED`); `time >= record` and at the 120 s cap -> `0x27EB`; otherwise (`time < record` but not all rupees) `0x27ED`. Then clear switch 0x41 | liftnuts.c:446-476; `z_inventory.c:694-705`; handler :660-663 |

### 5.5 Score, timer and the per-day records ("target score")

| Item | Value | Evidence |
| --- | --- | --- |
| Rupee score | 50 points per rupee, six rupees = 300; the shared score cell is a static `s16` owned by En_Lift_Nuts and found by En_Gamelupy through `Actor_FindSharedMemoryEntry` | OVL liftnuts.c:136-154; gamelupy.c:112-121 |
| **Target time per day** | `dekuPlaygroundHighScores[day - 1]`, centiseconds, default **7500 / 7500 / 7600** (`SECONDS_TO_TIMER(75, 75, 76)`: Day 1 75 s, Day 2 75 s, Day 3 76 s) set by `Sram_ClearHighscores`; a prize needs all six rupees in **strictly less** time; each new record overwrites the slot | C `src/code/z_sram_NES.c:404-414`; `z64save.h:79`, :295; liftnuts.c:872-875 |
| Reset | `Sram_ClearHighscores` is called from `Sram_SaveEndOfCycle`, so the three records return to 75 / 75 / 76 s when the Song of Time resets the cycle; the stored names (`dekuPlaygroundPlayerName[3][8]`) are not cleared by that function (U-07) | C z_sram_NES.c:430, :637; `z64save.h:221` |
| Timer clock | wall-clock based: `osGetTime()` minus start and paused times, so real seconds, independent of game-clock speed | C `z_parameter.c:5990-5995` |
| Timer HUD | position (115, 80); white until 9 s before the record, **yellow** from `record - 9 s`, **orange** at or after the record; a beep once per second from `record - 9 s` | C `z_parameter.c:6027-6034`, :6066-6075, :836-852 |
| Record shown in text | message control codes `HS_DEKU_PLAYGROUND_DAY_n` (0xFA-0xFC) and name codes (0xFD-0xFF) read the slot | C `message_data_fmt_nes.h:103-108`; `z_message_nes.c:1754-1766`, :1852-1863 |

### 5.6 Rewards, rupee economy and save flags

| Item | Value | Evidence |
| --- | --- | --- |
| Rupees collected | green +1, blue +5: all six = **+10**, equal to the fee | gamelupy.c:126-131; liftnuts.c:524-528 |
| Record prize | `GI_RUPEE_PURPLE` = `ITEM_RUPEE_PURPLE` = **50 rupees** (`sRupeeRefillCounts[4]`), offered at range 500 / 100 | liftnuts.c:919-933; `z_parameter.c:2543`, :2856-2857; `z64item.h:327-331`, :420; `player.c:2086` |
| Heart piece | `GI_HEART_PIECE` offered instead of the purple rupee when text `0x27F4` is active (record on **Day 3** after wins on Days 1 and 2) **and** `WEEKEVENTREG_RECEIVED_DEKU_PLAYGROUND_HEART_PIECE` is clear; the flag is set when the item is taken | liftnuts.c:922-932; `z64save.h:815`; `player.c:2106` |
| Win flags | `WEEKEVENTREG_WON_DEKU_PLAYGROUND_DAY_1/2/3` (reg 14 bits 0x10 / 0x20 / 0x40) are set in `ResumeConversation` by `CURRENT_DAY` after the reward is taken | liftnuts.c:942-974; `z64save.h:812-814` |
| Follow-up talk | `0x27F5` after the Day 3 triple (then the scrubs may hide), `0x27F0` or `0x27F1 -> 0x27F2 -> 0x27F3` otherwise | liftnuts.c:943-957, :636-658 |
| Persistence | only the heart-piece bit of reg 14 survives the Song of Time (`z_sram_NES.c:94`); win flags, records (5.5) and switches 0x40-0x42 reset | C |
| Prize rule summary | a prize requires a Deku-form run that collects all six rupees without touching static floor below Y 20, in under the day's record time; the first Day 3 record after two earlier wins in the same cycle is the one heart piece (once per file) | derived from the rows above |

### 5.7 Lifts and rupees per day (what changes with the day)

Only the lift set changes (4.3, 4.4); the room shell, flowers, scrubs and fee do not. At each 06:00 dawn (Day 2, Day 3) the mask test replaces the set (the 18:00 half-day change keeps it, since each set's mask covers both halves of its day): `Actor_KillAllOnHalfDayChange` kills actors whose mask lacks the new bit (`z_actor.c:3311-3330`) and `Actor_SpawnSetupActors` spawns entries newly in range (:2546-2575); whether a half-day change is processed while the player stays in the scene is U-06.

### 5.8 Deku flower, launch and HUD (Player side, indexed only)

| Mechanic | Where |
| --- | --- |
| Burrow: A on a flower floor as Deku (`SurfaceType_IsFloorDekuFlower`: poly `flags_vIB & COLPOLY_IS_FLOOR_DEKU_FLOWER` (1 << 1), or no collision header) -> `Player_Action_93` | `z_bgcheck.c:4330-4340`; `z64bgcheck.h:72`; `player.c:6980-6993`, :7090-7100, :8228-8231, :18920 |
| Charge and launch: release A; held >= 10 frames -> launch power 2700, else 1450; gold flowers (`params & 0x100`) scale speed (not used here, all flowers pink) | `player.c:18972-18978`, :18996-19004 |
| Launch sets `PLAYER_STATE3_200`; a long charge also sets `PLAYER_STATE3_2000` (hover phase); this is the run's start trigger and the flower's bounce trigger | `player.c:19015-19020`; liftnuts.c:824; etcetera.c:136-146 |
| Flight: `Player_Action_94`; range constants `D_8085D958 = {600, 960}` (semantics NI) | `player.c:19103-19200` |
| Flower reacts: oscillation timer, rustle, bounce animations | etcetera.c:111-239 |
| HUD during a run (`EVENTINF_34`): C buttons disabled; while hovering B shows the Deku Nut if owned, else disabled; HUD `A_B_MINIMAP` | `z_parameter.c:1813-1840`; cleared at `Interface_Init` (:7361-7364) |

### 5.9 Camera settings and cutscene data

| Use | Setting | Data | Source |
| --- | --- | --- | --- |
| Cave floors (surface 0: polys 0-9 camera-only Y 60, polys 133-142 Y 40) | `0x43 CAM_SET_INBUSH` | `BgCamInfo[0]`, data NULL | HDR scene.c:268-271; `include/z64camera.h:214` |
| Everything else (surfaces 1-6) | `0x11 CAM_SET_DUNGEON0` | `BgCamInfo[1]`, data NULL | same; z64camera.h:164 |
| Arrival at spawn 0 | `actorCsCamList[0]`: setting 31 `CAM_SET_FIXED1` | eye (85, 139, 2057) (0.85, 1.39, -20.57); rot (4369, -27125, 0) binang = pitch 24.0 deg, yaw -149.0 deg (sign convention NI); fov -1, then 20, -1 (`BgCamFuncData`, z64bgcheck.h:133-142) | HDR scene.c:245-257; C |
| Dawn cut (night -> day) | csId 9 (a17): priority 900, length -1, `csCamId 1` -> `actorCsCamList[1]` FIXED1, `hudVisibility 1`, endCam 0, letterbox 32 | eye (166, 286, -493) (1.66, 2.86, 4.93); rot (2549, -27307, 0) = pitch 14.0 deg, yaw -150.0 deg; fov -1 twice | HDR scene.c:233, :245-257; the eye looks toward the wall clock a26 within about 4 deg of yaw (INF) |
| Dusk cut (day -> night) | csId 10 (`additionalCsId` of 9): priority 1000, no camera, no letterbox | n/a | HDR scene.c:234; `test4.c:54-62` semantics (index = previous state) |
| Player-global cs | entries 0-8 (2.1) | global cs cameras | HDR scene.c:224-232 |

`cam.deku_playground.*` proposals (SOURCE_REGISTER 4.6; no capture exists): `entry` = the source arrival camera above from spawn 0; `reverse_door` = there is no door, so this is the tunnel mouth (about z 1050) looking +Z along the tunnel toward the cave and exit tag; `fixture` = from the start platform (about (0, 90, 1000)) looking -Z across the pit at the Day 1 lifts and the far platform.

### 5.10 Day and time variants

| Window | Effect | Evidence |
| --- | --- | --- |
| Day 1 06:00-23:30 | game offered; Day 1 lifts a08-a13 | 5.1, 4.3 |
| Day 2 / Day 3 06:00-23:30 | game offered; Day 2 lifts a02-a07 / Day 3 lifts a18-a23 | same |
| 23:30-06:00 every day | scrub refuses (`0x27F7`); lifts of the current day still exist | liftnuts.c:391, :404, :425 |
| Day 3 after 06:00 Day 4 | cycle ends; `day % 5 = 4` has no switch case | `z64save.h:518`; liftnuts.c:482-500 |
| Dusk/dawn cuts | En_Test4 inside the scene (2 cutscene entries above) | 5.9 |

### 5.11 Other room mechanics

Torches: lit, no interaction needed (4.5). Wall clock: live time. Obj_Etcetera AC cylinders and the Obj_Syokudai colliders accept the player's hits; no reward or switch is attached. No water, ladder, hookshot, conveyor or void surface exists (7.1).

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header, room_00)

Selection: default headers (the only ones), sceneLayer 0, half-day bit 0x080, fresh save (no week-event, switch or scene flags; records 75 / 75 / 76 s), Human form with Ocarina and Deku Mask held per the profile. **The venue is reachable and the game is open at Day 1 12:00** (no door; 5.1); at Human form the scrubs only refuse (`0x27D8`). Playing needs a Deku-form profile, which does not exist yet (U-02); the static geometry is independent of form.

| # | Component / params | N64 position (Unity) | Actor scale | Display list(s), bank, offset | Notes |
| --- | --- | --- | --- | --- | --- |
| shell | room_00 cullable entries 0-8, 10-16 (opaque) and 9 (XLU) | world-space vertices | n/a | room DLs `0x3FB0` (16 opaque calls) and `0x4350` (`DL_004220`, 22 vertices, segment-8 two-texture scroll) | entry 10 `DL_003EF8` is a decal-mode XLU DL in an opaque slot (radius -128, drawn last); INF: a scrolling XLU column over the cave (entry 9 centered (-37, 360, 2014), radius 326) |
| a00, a01 | Deku flower, 0x007F | (90, 40, 901), (-90, 40, 901) ((0.90, 0.40, -9.01), (-0.90, 0.40, -9.01)) | (0.01, 0.02, 0.01) | `gPinkDekuFlowerIdleDL` gameplay_keep 0xED80; collision 0xE710 | start flowers; idle |
| a08-a13 | lifts, 4.3 | home positions of 4.3 | (0.1, 0.15, 0.1) | `object_raillift_DL_0071B8` 0x71B8; collision 0x48D0 | rest pose at home (they move after 5 frames) |
| a08-a13 / child0 | pink Deku flower, params 0 | on each lift | (0.01, 0.02, 0.01) | as a00 | spawned child |
| a08-a13 / child1 | rupee, params 0 (5 green) or 1 (a13 blue) | at the lift home; settles under gravity -0.5 | 0.03 | `gRupeeDL` gameplay_keep 0x622C0 + green 0x61FC0 / blue 0x61FE0 | spawned; spinning; rest height INF U-03 |
| a14, a15 | scrub, 0x06FF / 0x07FF | (-90, 40, 901), (90, 40, 901) | default | `gBusinessScrubSkel` object_dnt 0xAC70, burrowed end pose of `gBusinessScrubBurrowAnim` | hat, hair, scalp and bag limbs not drawn |
| a24, a25 | torches, 0x287F | (-60, 40, 1440), (60, 40, 1083) | 1.0 (init chain 1000/1000) | `gObjectSyokudaiTypeNoSwitchDL` object_syokudai 0x870 + flame `gEffFire1DL` 0x7D590 | lit; point light |
| a26 | wall clock, 0x907F | (0, 260, -741) (0.00, 2.60, 7.41) | 0.02 | object_obj_tokeidai 0xCF28, 0xBEE8, 0xF518, 0xC368 | shows 12:00 |

**Excluded by half-day mask:** a02-a07 (Day 2), a18-a23 (Day 3). **No geometry:** a16 `En_Warp_tag`, a17 `En_Test4`. **Present, needs behavior, not a static mesh:** the scrub conversation and run, lift motion, rupee pickup, flower launch, timer HUD. Extraction commands for the banks are in section 9 (not executed).

## 7. Collision summary (C from scene.c:268-570; scratch `col.py`, `floors.py`)

### 7.1 Header and surfaces

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 108 / 175 / 7 (LOCATION_REGISTER does not count these) |
| Water boxes | 0 (`0, NULL`, scene.c:567-575) |
| Camera data | 2 `BgCamInfo`, both data NULL: `0x43 CAM_SET_INBUSH`, `0x11 CAM_SET_DUNGEON0` |
| Header bounds | min (-750, -260, -750), max (750, 880, 2240); Unity x -7.50..7.50, y -2.60..8.80, z -22.40..7.50 |
| Vertex extents | x -750..750, y -260..680, z -750..2240; Unity y -2.60..6.80 (header Y max is 200 u past the vertices) |
| Polygon kinds (normal Y > 0.5 floor, < -0.8 ceiling, else wall; `z_bgcheck.c:3029-3034`) | 52 floors, 106 walls, 17 ceilings |
| Floor slopes (10-deg bins) | 0 deg: 46, 30 deg: 4 (the four ramps, 39.8 deg), 50 deg: 2 (polys 92 and 95, 55.4 and 56.0 deg, tunnel side) |
| Exit surfaces | none: every surface has `exitPlus1` 0 (2.3) |
| Poly exclusion (`vIA >> 13`) | 24 polys xp 6 (ignore entity and projectiles, camera only): cave lid floor polys 0-9 (Y 60) and rim walls 10-23; 10 polys xp 4 (ignore projectiles): cave floor polys 133-142 (Y 40); no vIB flags (no conveyor, no Deku-flower floor in the static scene) |
| Special | no ladder, hookshot, conveyor, void, water or damage surface; floor property 9 on every platform floor (type 6): suppresses auto-jump and the ledge check (`player.c:7539`, :7569); no wall types |

| Type | data[0] / data[1] | bgCam | Material | Light | Echo | Polys (floor / wall / ceiling) | Where |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | 0x00000000 / 0x000017C6 | 0 INBUSH | 6 tall grass | 31 | 2 | 20 / 0 / 0 | cave floor ring (polys 0-9 lid Y 60, 133-142 Y 40) |
| 1 | 0x00000001 / 0x000017C8 | 1 DUNGEON0 | 8 grass | 31 | 2 | 8 / 14 / 0 | gate and cave floor (polys 125-132, Y 40), camera-only rim walls 10-23 |
| 2 | 0x00000001 / 0x00005002 | 1 | 2 stone | 0 | 10 | 0 / 48 / 13 | outer walls z -750 and z 1050 (tunnel doorway x +-90), tunnel walls and roof, cave ceiling |
| 3 | 0x00000001 / 0x00001002 | 1 | 2 stone | 0 | 2 | 6 / 20 / 0 | tunnel floor (polys 48, 49, 62, 63, Y 40), cave and gate walls, polys 92 and 95 |
| 4 | 0x00000001 / 0x000057C2 | 1 | 2 stone | 31 | 10 | 4 / 24 / 4 | pit floor Y -260 (polys 167-170), pit walls x +-750 and +-270, ceiling Y 340..640 |
| 5 | 0x00000001 / 0x00001008 | 1 | 8 grass | 0 | 2 | 2 / 0 / 0 | gate floor (polys 123, 124, Y 40) |
| 6 | 0x24000001 / 0x000057C2 | 1 | 2 stone | 31 | 10 | 12 / 0 / 0 | start platform, landings, ramps, far platform; floor property 9 |

Surface decode: `SURFACETYPE0/1` (`z64bgcheck.h:79-98`); materials `include/z64bgcheck.h:232-246`. Polygon `i` is on scene.c line `279 + i`; surfaces :274-275; vertices :457-564.

### 7.2 Walkable spaces

| Space | Extent x / y / z | Polys | Notes |
| --- | --- | --- | --- |
| Entry cave | -227..173 / 40 / 1887..2240 (Unity -2.27..1.73, 0.40, -22.40..-18.87); ceiling Y 680 | floors 128-142 (plus camera-only lid polys 0-9 at Y 60); ceilings 74-77 | spawn 0 and the exit tag a16 |
| Gate | -152..92 (floor 125 spans -227..173) / 40 / 1590..1887 | floors 123-127 | domed gateway (ceilings 85-89, Y 301..356), arch walls |
| Tunnel | -90..90 / 40 / 1050..1590; walls to Y 220, roof Y 280 (x +-30) with 45 deg chamfers | floors 48, 49, 62, 63 | torches a24 (-60, 40, 1440) and a25 (60, 40, 1083) stand in it |
| Start platform | -150..150 / 40 / 840..1050 and landings +-(150..270) / 40 / 990..1050 | floors 147-150, 155, 156 (type 6) | flowers and scrubs at z 901; spawn 1 |
| Side ramps | +-(150..270) / -260..40 / 630..990, 39.8 deg | floors 143, 144, 153, 154 (type 6) | climb from the pit floor to the platform (INF: walkable, U-04); during a run they count as failure below Y 20 |
| Pit | -750..750 / -260 / -750..1050; ceiling Y 340..640 | floors 167-170 | failure surface |
| Far platform | -150..150 / 40 / -750..-510 | floors 159, 160 (type 6) | far wall at z -750 (wall clock a26 above it at Y 260) |

Everything between the start and far platforms is empty in the static collision: it is crossed on the lifts (dynapoly, `object_raillift_Colheader_0048D0`, NI) and flower launches. Coplanar duplicates: the cave floor has two Y 40 sets (polys 128-132 type 1, polys 133-142 type 0) with different camera data and exclusion flags (U-05).

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 20: 19 textures + 1 TLUT (`Z2_DEKUTESTLUT_008C90`, loaded as a 256-entry palette by the ci8 DLs) |
| By format | rgba16 9, ci8 6, ia16 2, i4 2 (+ the rgba16 TLUT) |
| Location | all in the scene file; the room file holds none; all 19 are loaded by room DLs |
| Room `Gfx` / `Vtx` arrays | 19 / 17 (17 `.vtx.inc` files, 679 vertices) |
| Room shape | cullable, 17 entries: 16 opaque-slot DLs (one is the decal DL) and 1 XLU DL (entry 9) (room.c:146-170) |
| XML-declared DLs | `0x3FB0` calls the 16 opaque DLs; `0x4350` calls `DL_004220` (room.c:753-776, :813-816) |
| Triangles in source | 199 `gsSP2Triangles` + 6 `gsSP1Triangle` = 404 (DL reuse not resolved) |
| Render setup | 17 `gsDPSetCombineLERP`, 17 `gsDPSetRenderMode`: 15 `G_RM_FOG_SHADE_A, G_RM_AA_ZB_OPA_SURF2`, 1 `G_RM_PASS, G_RM_AA_ZB_XLU_DECAL2`, 1 `G_RM_PASS, G_RM_ZB_XLU_SURF2`; 15 DLs use fog |
| Animated materials | 1 (segment 8, two-texture scroll; the XLU DL's `gsSPDisplayList(0x08000000)`, room.c:795) |
| Object textures (later) | `object_raillift` 13 textures (XML), `object_dnt` 11 textures plus eyes DLs, `object_syokudai` 5 textures |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_DEKUTES` and room_00 (C above); `object_obj_tokeidai` (South). Binaries exist under `extracted/n64-us/baserom/` (checked by `stat`: object_raillift 32,432, object_dnt 45,776, object_syokudai 15,504, object_obj_tokeidai 72,832, gameplay_dangeon_keep 144,000, gameplay_keep 599,520 bytes).

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_raillift`, `object_syokudai`, `gameplay_keep` | lifts and their collision, torches, flowers, rupees, flame (section 6) |
| Static, pose | `object_dnt` | scrub skeleton and burrowed pose |
| Optional | `gameplay_dangeon_keep` | only if the torch DL references it (U-10) |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_raillift','object_syokudai','object_dnt','gameplay_keep','gameplay_dangeon_keep')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

`gameplay_keep` and `gameplay_dangeon_keep` have not been extracted with this tool before (NI); check size and warnings separately. Dynapoly collision of the lifts and flowers comes from `object_raillift_Colheader_0048D0` and `gPinkDekuFlowerCol` once extracted.

## 10. Proposed region record

Schema 1 (`shared/city/schema/region.schema.json`, PIPE-01 `7a7a08c`). Not lifted or validated by the integrator's tools in this packet; the block was checked in scratch with a hand-written subset validator (section 11) and against `shared/city/ids.json` by string.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.deku_playground",
  "displayLabel": "Deku Scrub Playground",
  "source": {
    "sceneSymbol": "Z2_DEKUTES",
    "sceneEnum": "SCENE_DEKUTES",
    "sceneTableIndex": "0x1E",
    "entranceTableSceneIndex": "0x1B",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "halfDayBit": "0x080",
    "stateAssumptions": [
      "fresh save: no week-event, switch or scene flags; WEEKEVENTREG_WON_DEKU_PLAYGROUND_DAY_1..3 and ..._HEART_PIECE clear",
      "dekuPlaygroundHighScores at their defaults 75 s / 75 s / 76 s",
      "player form Human per profile; En_Lift_Nuts refuses play unless Deku form (dossier 5.1, U-02)",
      "Day 1 12:00: scrub game open (closed 23:30 to 06:00)"
    ],
    "sceneLayer": 0
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_DEKUTES", "bytes": 36496, "sha256": "cc5517da3014884a1360f96ac778ccdac75b585e0be96041605a94eb2bc7900b" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_DEKUTES_room_00", "bytes": 17248, "sha256": "a8a295652e66fcfbc77e4296d97d7d4428c6783e39bd22d52d2b570e2be1978d" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_DEKUTES/Z2_DEKUTES.c", "bytes": 27952, "sha256": "adef0623c8cfc4ac87c92d103e44f7bd4c6b148b50f3d6b142701321d944145c" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_DEKUTES/Z2_DEKUTES_room_00.c", "bytes": 46236, "sha256": "c1baf8b4103f9a1ca84b5b8cc20d75262d35992bdcd77faa1ff8a17878e7b090" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_DEKUTES/Z2_DEKUTES.xml", "bytes": 398, "sha256": "e979e36dda44a8d1cbb4326de2f3bd2666a6b6e03301d0d5beadd8ff40f6a5e4" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": { "with": [], "status": "not_shared", "evidence": "interior local frame: arrival spawn 0 (-28, 400, 1939) versus the North grotto hole (-966, 292, -1703); the grotto pair is entrance-based, no coordinates coincide" }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-750, -260, -750], "max": [750, 880, 2240] },
    "sourceCollisionVertices": { "min": [-750, -260, -750], "max": [750, 680, 2240] },
    "unityCollisionVertices": { "min": [-7.5, -2.6, -22.4], "max": [7.5, 6.8, 7.5] }
  },
  "water": [],
  "anchors": [
    { "id": "anchor.dkp.spawn0_from_north", "source": [-28, 400, 1939], "unity": [-0.28, 4.0, -19.39], "status": "proposed", "note": "grotto arrival, 360 u above the cave floor" },
    { "id": "anchor.dkp.warp_tag", "source": [-24, 40, 2010], "unity": [-0.24, 0.4, -20.1], "status": "proposed", "note": "a16 exit trigger, 30 u radius" },
    { "id": "anchor.dkp.spawn1_reload", "source": [-1, 40, 941], "unity": [-0.01, 0.4, -9.41], "status": "proposed", "note": "post-run reload spawn on the start platform" },
    { "id": "anchor.dkp.flower_east", "source": [90, 40, 901], "unity": [0.9, 0.4, -9.01], "status": "proposed", "note": "start flower a00 and scrub a15" },
    { "id": "anchor.dkp.far_platform", "source": [0, 40, -630], "unity": [0.0, 0.4, 6.3], "status": "proposed", "note": "centre of the far platform, floor polys 159-160" },
    { "id": "anchor.dkp.pit_floor", "source": [0, -260, 100], "unity": [0.0, -2.6, -1.0], "status": "proposed", "note": "failure floor, polys 167-170" }
  ],
  "rooms": [
    { "id": "loc.deku_playground.room_00", "sourceRoom": "Z2_DEKUTES_room_00", "header": 0, "roomShape": "cullable, 17 entries", "displayLists": 19, "role": "play field: entry cave, tunnel, start platform, pit, far platform" }
  ],
  "components": [
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a00", "actorId": "0x183", "params": "0x007F", "class": "static-mesh", "dls": ["gameplay_keep:gPinkDekuFlowerIdleDL 0xED80"], "status": "selected", "note": "start flower, dynapoly gPinkDekuFlowerCol 0xE710, scale (0.01, 0.02, 0.01)" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a01", "actorId": "0x183", "params": "0x007F", "class": "static-mesh", "dls": ["gameplay_keep:gPinkDekuFlowerIdleDL 0xED80"], "status": "selected", "note": "start flower, same as a00" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a08", "actorId": "0x1CD", "params": "0x8008", "class": "static-mesh", "dls": ["object_raillift:0x71B8"], "status": "selected", "note": "Day 1 lift, path 8, rest pose at home; dynapoly 0x48D0; scale (0.1, 0.15, 0.1)" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a09", "actorId": "0x1CD", "params": "0x8009", "class": "static-mesh", "dls": ["object_raillift:0x71B8"], "status": "selected", "note": "Day 1 lift, path 9" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a10", "actorId": "0x1CD", "params": "0x800A", "class": "static-mesh", "dls": ["object_raillift:0x71B8"], "status": "selected", "note": "Day 1 lift, path 10" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a11", "actorId": "0x1CD", "params": "0x800B", "class": "static-mesh", "dls": ["object_raillift:0x71B8"], "status": "selected", "note": "Day 1 lift, path 11" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a12", "actorId": "0x1CD", "params": "0x800C", "class": "static-mesh", "dls": ["object_raillift:0x71B8"], "status": "selected", "note": "Day 1 lift, path 12" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a13", "actorId": "0x1CD", "params": "0x900D", "class": "static-mesh", "dls": ["object_raillift:0x71B8"], "status": "selected", "note": "Day 1 lift, path 13, blue rupee" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a08/child0", "actorId": "0x183", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gPinkDekuFlowerIdleDL 0xED80"], "status": "selected", "note": "pink flower riding lift a08, position copied from the lift every frame" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a08/child1", "actorId": "0x1D2", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gRupeeDL 0x622C0"], "status": "selected", "note": "green rupee (+1) at the lift home, spinning" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a09/child0", "actorId": "0x183", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gPinkDekuFlowerIdleDL 0xED80"], "status": "selected", "note": "pink flower riding lift a09, position copied from the lift every frame" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a09/child1", "actorId": "0x1D2", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gRupeeDL 0x622C0"], "status": "selected", "note": "green rupee (+1) at the lift home, spinning" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a10/child0", "actorId": "0x183", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gPinkDekuFlowerIdleDL 0xED80"], "status": "selected", "note": "pink flower riding lift a10, position copied from the lift every frame" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a10/child1", "actorId": "0x1D2", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gRupeeDL 0x622C0"], "status": "selected", "note": "green rupee (+1) at the lift home, spinning" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a11/child0", "actorId": "0x183", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gPinkDekuFlowerIdleDL 0xED80"], "status": "selected", "note": "pink flower riding lift a11, position copied from the lift every frame" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a11/child1", "actorId": "0x1D2", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gRupeeDL 0x622C0"], "status": "selected", "note": "green rupee (+1) at the lift home, spinning" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a12/child0", "actorId": "0x183", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gPinkDekuFlowerIdleDL 0xED80"], "status": "selected", "note": "pink flower riding lift a12, position copied from the lift every frame" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a12/child1", "actorId": "0x1D2", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gRupeeDL 0x622C0"], "status": "selected", "note": "green rupee (+1) at the lift home, spinning" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a13/child0", "actorId": "0x183", "params": "0x0000", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gPinkDekuFlowerIdleDL 0xED80"], "status": "selected", "note": "pink flower riding lift a13, position copied from the lift every frame" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a13/child1", "actorId": "0x1D2", "params": "0x0001", "class": "static-mesh", "variant": "spawned", "dls": ["gameplay_keep:gRupeeDL 0x622C0"], "status": "selected", "note": "blue rupee (+5) at the lift home, spinning" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a14", "actorId": "0x1C9", "params": "0x06FF", "class": "posed-skeletal", "variant": "posed-skeletal", "dls": ["object_dnt:gBusinessScrubSkel 0xAC70"], "status": "optional", "note": "left employee scrub, burrowed pose; hat, hair, scalp and bag limbs nulled by the overlay" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a15", "actorId": "0x1C9", "params": "0x07FF", "class": "posed-skeletal", "variant": "posed-skeletal", "dls": ["object_dnt:gBusinessScrubSkel 0xAC70"], "status": "optional", "note": "right employee scrub, burrowed pose" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a24", "actorId": "0x039", "params": "0x287F", "class": "static-mesh", "dls": ["object_syokudai:0x870", "gameplay_keep:gEffFire1DL 0x7D590"], "status": "selected", "note": "lit torch, flame billboard, point light radius 250" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a25", "actorId": "0x039", "params": "0x287F", "class": "static-mesh", "dls": ["object_syokudai:0x870", "gameplay_keep:gEffFire1DL 0x7D590"], "status": "selected", "note": "lit torch, same as a24" },
    { "sourceKey": "Z2_DEKUTES/h0/room_00/a26", "actorId": "0x19C", "params": "0x907F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected", "note": "wall clock type 9, scale 0.02, noon pose" }
  ],
  "collisionOwnership": {
    "room": "Z2_DEKUTESCollisionHeader_001260 (175 polys, 108 vertices, 7 surface types)",
    "waterBoxes": 0,
    "actorDynapoly": ["a00/a01 and every spawned lift flower: gPinkDekuFlowerCol (gameplay_keep 0xE710)", "a08-a13 (Day 1; a02-a07 and a18-a23 on other days): object_raillift_Colheader_0048D0"],
    "actorCylinders": ["a14/a15 scrubs r25 h75 (OC)", "spawned En_Gamelupy rupees r10 h30 (OC, player only)", "Obj_Etcetera AC cylinders r20 h14", "Obj_Syokudai stand and flame cylinders"],
    "note": "no collision exit: the only way out is the a16 En_Warp_tag trigger; failure surface is static floor below Y 20 (pit Y -260 and the lower ramps)"
  },
  "portalIds": ["portal.deku_playground.north_clock_town"],
  "materialBindings": null,
  "cameraIds": ["cam.deku_playground.entry", "cam.deku_playground.reverse_door", "cam.deku_playground.fixture"],
  "routeIds": [],
  "omissions": ["Link and Tatl", "scrub conversation, fee, state machine and rewards", "lift motion along paths 8-13 and the Day 2 and Day 3 lift sets with their spawned flowers and rupees", "rupee collection and the 300-point score", "Deku flower burrow, launch, hover and flight", "run timer HUD and per-day record times", "reload at spawn 1"],
  "excluded": {
    "halfDayMask": ["a02-a07 Day 2 lifts (mask 0x030)", "a18-a23 Day 3 lifts (mask 0x00C)"],
    "stateOrSchedule": [],
    "noGeometry": ["a16 En_Warp_tag (invisible exit trigger, portal source)", "a17 En_Test4 (three-day controller)"]
  },
  "unsupported": ["two-texture scroll animated material on the cave light shaft", "torch flame billboard and flicker light", "wall clock live time", "lift dynapoly carrying actors and the 5-frame wait before movement", "rupee spin and pickup", "scrub burrow, rise and talk animations", "camera-only polygons (exclusion flags 6 and 4)", "floor property 9 behavior", "INBUSH and DUNGEON0 camera settings and the FIXED1 arrival camera", "per-day record time persistence"],
  "evidence": ["docs/city/interiors/deku_playground.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" },
  "readinessNotes": { "sourceInspection": "message roles come from a scratch header and preview scan of message_data_static (C-BIN, text not copied); play needs Deku form, which prof.d1.noon.normal does not provide (dossier U-02)" }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Text id `0x238D` has no message-table entry and no `Message_StartTextbox` call: dead branch or reached by a jump inside message data; also the role of `0x27F3` | Scan `message_data_static` control codes (jump/next opcodes, `include/message_data_fmt_nes.h`) for `0x238D` and for the ids that follow `0x27F2`/`0x27F3`; the table lookup is `z_message.c:116-121` |
| U-02 | Profile form: the baseline `prof.d1.noon.normal` is Human, so En_Lift_Nuts only refuses; the frozen profile grammar `prof.d<1-3>.<time>.<layer>` has no form slot (SOURCE_REGISTER 3 proposes a `.form` suffix) | OWNER / INTEGRATOR: add a Deku-form profile id for INT-14 playtests, or declare the game out of scope for static import |
| U-03 | Whether a rupee that landed on a lift rides it (carried actor) or stays; the raillift top Y and size | Extract `object_raillift_Colheader_0048D0` (0x48D0); run Day 1 and log the rupee and lift positions at frames 5, 60, 600; check `Actor_UpdateBgCheckInfo` floor flags for `FLAG_4 \| FLAG_8` (`gamelupy.c`, `z_actor.c:1690-1700`) |
| U-04 | Walkability of the 39.8 deg ramps and the 55-56 deg polys 92/95 (climb out of the pit outside a run; are lower ramp steps a failure inside a run) | Read the slope-slide limit in `player.c` (search `floorPoly->normal` / `Player_...Slope`), then a route test from the pit floor to the platform |
| U-05 | Two coplanar Y 40 floor sets in the cave (polys 128-132 type 1, 133-142 type 0): which one the floor raycast returns and which camera data applies | `BgCheck_RaycastFloor` ordering in `z_bgcheck.c` for static polys; an A1 capture of spawn 0 |
| U-06 | Behavior when the half-day changes while the player is in the scene: lift set swap, scrub state, En_Test4 cut | Read how `numSetupActors` is re-armed on a half-day change (`z_actor.c:2546-2575`, `Actor_KillAllOnHalfDayChange` callers) and run Day 1 18:00 in the scene |
| U-07 | Whether `dekuPlaygroundPlayerName` is cleared at cycle end | `Sram_SaveEndOfCycle` (`z_sram_NES.c:430-654`) for the `inventory` name arrays |
| U-08 | Start mode D at spawn 1 while the AFTER-state scrub halts the player (auto-walk 180 u toward z 761, off the platform edge at z 840); `Player_Action_35` details | Read `Player_Action_35` (`player.c`, search the action table) and the `actionVar2 = -20` wait; capture the reload |
| U-09 | Senders of entrances `0x3600` / `0x3610` outside the 22 extracted city scenes | Grep all scene exit lists and cutscene `nextEntrance` words (non-city scenes are not extracted) for `0x3600`, `0x3610` |
| U-10 | Which torch DL refers to `gameplay_dangeon_keep` (scene special file); LightSettings rows 1-3 consumers and values | Decode `object_syokudai` DL after extraction; `LightSettings` fields (`include/z64environment.h`) for scene.c:53-62 |
| U-11 | Real seconds per actor update (lift speeds are units per frame; the timer is wall clock) | SOURCE_REGISTER U-01 follow-up: `R_UPDATE_RATE` / framerate divisor for the N64 US build |
| U-12 | Meaning of the `0x0102` versus `0x4102` entrance flag word and `TRANS_TYPE_64` at the reload | `Entrance_GetTransitionFlags` users (`z_play.c:583`, :2275; `z_scene.c:540`) and the transition table |
| U-13 | Constants and semantics of the Deku flight (`D_8085D958`, hover phase) for a later controller packet | `Player_Action_94` in full (`player.c:19108-19260`) |

Register cross-checks (not edited): LOCATION_REGISTER agrees on every count and its U-02 is answered for this scene (exit 1 is padding). PORTAL_REGISTER agrees on both rows (its "room_00:96" is a source line, actor index 16; the L2324 cite is the closing brace, row 2323); its "special-only" classification holds: the only inbound is the North `DOOR_ANA` and the only outbound is the a16 tag. PORTAL_REGISTER U12 (hardcoded `ENTRANCE`) is settled for this scene: one runtime site, `liftnuts.c:905` -> spawn 1. The dispatch's "FULL_CITY_PLAN section 5" row is in section 6 of the current plan.

**Checks run:** read `docs/HANDOFF.md`, `REF09_BRIEF.md`, FULL_CITY_PLAN sections 6, 11, 12, the three registers' rows for this venue, the North dossier headings and 2.4, the Great Fairy and Swordsman dossiers and records, `shared/city/schema/README.md` and `region.schema.json`, `shared/city/ids.json` entries for this venue; read the whole scene C and room C, the scene XML and the object XMLs (`object_raillift`, `object_dnt`, `object_syokudai`, `object_obj_tokeidai`, gameplay_keep symbols), `scene_table.h`, `z_scene_table.c` rows, `z_actor.c`, `z_play.c`, `z_parameter.c`, `z_inventory.c`, `z_sram_NES.c`, `z_scene.c`, `z_scene_proc.c`, `z_bgcheck.c`, `z_bg_collect.c`, `z64bgcheck.h`, `z64save.h`, `z64item.h`, and the overlays En_Lift_Nuts (all), Obj_Lupygamelift (all), En_Gamelupy (all), Obj_Etcetera (all), En_Warp_tag (all), Door_Ana, Obj_Syokudai, Obj_Tokeidai (wall clock paths), En_Test4 (init); player functions cited above. Scratch stdlib scripts in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\deku_playground\` (`col.py` -> `col_out.txt`, `rooms.py` -> `rooms_out.txt`, `floors.py` -> `floors_out.txt`, `msg.py` -> `msg_out.txt`, `check_record.py`); SHA-256 and sizes of the inputs in section 10; the section 10 block parsed as JSON and checked against the schema's structure, enums and patterns and the registered ids by `check_record.py` (not `validate_city.py`).

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes; `region_from_dossier.py` and `validate_city.py`; message text transcription; LightSettings value decoding; the lift and flower collision headers (not extracted); scenes outside the 22; 3DS material.

**Next dependency:** integrator review of section 10 (lift into `shared/city/regions/` is the integrator's step); U-02 profile decision; then ROOM-03 (special-space work) only after ROOM-01. The minigame (rupee collection, timer, records, Deku flight) is a later mechanics packet and stays explicitly pending in City C.
