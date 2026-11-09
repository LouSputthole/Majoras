# Lottery Shop (`loc.lottery_shop`): source inventory dossier

Task **REF-09 / INT-06** (FULL_CITY_PLAN.md section 12 row REF-09, section 6 row INT-06). Base commit `31b20dc` on `main` (HEAD moved to `1055c1c` while this packet ran; section 10 follows schema 1 as frozen at `7a7a08c`, PIPE-01, and the uncommitted additive working-tree edit seen at 2026-11-08 evening, and its IDs were checked against `shared/city/ids.json`). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_TAKARAKUJI` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_TAKARAKUJI/Z2_TAKARAKUJI.c`, `room.c` = `.../Z2_TAKARAKUJI_room_00.c`. Line numbers count `\n` only (both files are CRLF and also hold lone `\r` bytes: 2 in scene.c, 10 in room.c, some inside actor entries). Polygon `i` is on scene.c line `130 + i`; vertex `i` on line `233 + i`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. The interior uses its own frame around the origin; it is not placed in the town frame.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected. Quoted text meanings ("Pick 3 numbers") are the decomp authors' comments in the overlay (ANN), not message data.

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01), `docs/city/PORTAL_REGISTER.md` (REF-02), `docs/city/SOURCE_REGISTER.md` (REF-03), `docs/city/districts/west_clock_town.md` section 2.4.1 (street door TA2). Agreements and the one disagreement are in section 11.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_TAKARAKUJI` / `SCENE_TAKARAKUJI` | `include/tables/scene_table.h:120` |
| Scene-table index | `0x39` | same line |
| Entrance-table scene index | `0x36` (`ENTR_SCENE_LOTTERY_SHOP`) | `src/code/z_scene_table.c:2627`, `include/z64scene.h:647` |
| Title card text / draw config | `0x112` / `SCENE_DRAW_CFG_DEFAULT` | scene_table.h:120 |
| Restrictions | `RESTRICTIONS_SET(0,0,0,0,0,3,0,0,3,0,0,1)`: by argument name (`src/code/z_parameter.c:66-71`) songOfDoubleTime 3, songOfStorms 3, all 1, others 0. It differs from `RESTRICTIONS_INDOORS` (`:75`) only in bButton (0 here, 1 there). Meaning of value 3 and of `all`: NI | HDR, C |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_NONE` | scene_table.h:120 |
| XML files | Scene `Z2_TAKARAKUJI` segment 2; room `Z2_TAKARAKUJI_room_00` segment 3; room DLs declared `0x2E00`, `0x2E38` | XML `assets/xml/scenes/Z2_TAKARAKUJI/Z2_TAKARAKUJI.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene / room headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene.c:13-29 or room.c:13-22 | HDR |
| Transition actors | 1 `EN_DOOR` | HDR scene.c:16, :36-38 |
| Region visited | `SCENE_CMD_SET_REGION_VISITED` (scene.c:17); handler `Scene_CommandSetRegionVisitedFlag` (`src/code/z_scene.c:504`); the scene is in the Clock Town region list (`src/code/z_inventory.c:428-435`) | HDR, C |
| Special files | none (no `SCENE_CMD_SPECIAL_FILES`): no field or dungeon keep | HDR scene.c:13-29 |
| Sound | spec 5, ambience 19 (`AMBIENCE_ID_13`, `include/sequence.h:94`), seq 0 (`NA_BGM_GENERAL_SFX` by table ordinal, `include/tables/sequence_table.h:20`); arg order `include/z64scene.h:845-846`. Shop music is positional, from Obj_Sound a05 (section 4.3) | HDR scene.c:14, C |
| Skybox | file 0, skybox 0, weather 0, lightMode true = `LIGHT_MODE_SETTINGS` (`include/z64environment.h:48-49`); `SKYBOX_DISABLES(true, true)` in the room | HDR scene.c:21, room.c:16; macros z64scene.h:833-837 |
| Env light settings | 4 entries, all four identical (values not transcribed) | HDR scene.c:23, :54-63 |
| Actor cutscenes / cs cameras | 11 / 2 | HDR scene.c:24-25, :65-77, :87-99 |
| Cutscene scripts / paths | none / none (no `CUTSCENE_SCRIPT_LIST`, no `PATH_LIST`) | HDR |
| Minimap | 1 entry `{0xFFFF, 0x0000, 0x0000, 0x009E, 0x0000}`, scale 0 | HDR scene.c:79-85 |
| Animated materials | single entry `{ 0, 6, NULL }` (segment 0, skipped, as in North) | HDR scene.c:105-107 |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x00, 0x00, 0, 0, false, 0)` (type 0, environment 0, no positional lights, storm 0; macro z64scene.h:804-807); `TIME_SETTINGS(255, 255, 3)`: no forced time, the clock runs inside (speed 5 without Ocarina, `src/code/z_scene.c:399-402`); cullable room shape, 4 entries | HDR room.c:14-18 |

### 1.1 Alternate headers: none

The entrance table has one entrance with one layer: `sLotteryShopEntrance0[] = { { SCENE_TAKARAKUJI, 0, 0x4102 } }` and `sLotteryShopEntranceTable[] = { sLotteryShopEntrance0 }` (`z_scene_table.c:1254-1260`). Every day, time and profile therefore loads the same default scene and room header (C). Profiles differ only through the clock, flags and actor logic (section 5). Profiles that matter here: `prof.d1.noon.normal` (open, buy window) and `prof.d1.night.normal` (21:00, claim window), both on header 0.

## 2. Spawns, entrances and exits

### 2.1 Start position (HDR scene.c:31-34; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`); yaw `(rawRotY >> 7) & 0x1FF`.

| Start | Position | Raw rot X / Y / Z | Yaw | Params | Mode | bgCam | Reached by |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (0, 0, 320) | 0x0007 / 0x5A00 / 0x007F | 180 | 0x0EFF | 0xE | 0xFF none | Entrance 0 = `0x6C00`, West exit [8] (`Z2_ICHIBA.c:94`): arrival of `portal.west_clock_town.lottery_shop`. 25 u inside the door, 10 u in front of the back wall (z 330), facing -Z into the shop |

Entrance list (scene.c:44-47): row 0 `{0x00, 0x00}` = start 0, room 0. Row 1 `{0x00, 0x00}` has no declared entrance (only `sLotteryShopEntrance0` exists), so it is padding (INF; LOCATION_REGISTER U-14). Entrance flags `0x4102`: exit fade `0x02`, arrival `(0x4102 >> 7) & 0x7F = 0x02`, bit 15 clear (field meanings PORTAL_REGISTER section 1).

### 2.2 Exit list (HDR scene.c:49-52) and consumers

| Exit idx | Raw | Decode (scene / entrance / layer) | Destination | Consumer here |
| --- | --- | --- | --- | --- |
| 0 | `0xD480` | 0x6A `Z2_ICHIBA` / 8 / 0 | `sWestClockTownEntrance8[0] = { SCENE_ICHIBA, 8, 0x4102 }` (`z_scene_table.c:2292-2294`) -> `Z2_ICHIBAEntranceList0x00018C[8] = {0x08, 0x00}` (`Z2_ICHIBA.c:81`) -> West start 8 (-1453, 200, -814), yaw 229, params `0x0F05` (`Z2_ICHIBA.c:55-56`) | surface 1 (polys 24, 25) through the door probe (2.4); a01 En_Time_Tag kickout, `params & 0x1F = 0` |
| 1 | `0x0000` | n/a | n/a | none: no surface type has exit+1 = 2 and no actor in this scene reads index 1 (C). Trailing padding; answers LOCATION_REGISTER U-02 for this scene |

### 2.3 Collision surface carrying the exit (C)

`exitIndexPlusOne` = bits 8-12 of `SurfaceType.data[0]` (`include/z64bgcheck.h:79-87`). Surface type 1 = `{0x00000100, 0x00001002}` (scene.c:125): exit+1 = 1 -> exit 0. Polygons 24 and 25 (scene.c:154-155): one quad at Y 0, x -60..60, z 360..480, normal +Y, vIA flag 1 (`COLPOLY_IGNORE_CAMERA`, z64bgcheck.h:66), no conveyor, stone. Mean vertex (0, 0, 420): 75 u behind the door on its axis (PORTAL_REGISTER "75u" agrees). It is a floor, so it is a live exit (`Player_HandleExitsAndVoids`, `src/overlays/actors/ovl_player_actor/z_player.c:6405-6416`).

### 2.4 The door (transition actor ta00) and the return route (C)

`TransitionActorEntry { sides[2] {room, bgCamIndex}, id, pos, rotY, params }` (`include/z64scene.h:369-378`); spawn `src/code/z_actor.c:3600-3625`.

| Field | Value | Evidence |
| --- | --- | --- |
| Raw entry | `{ 0, 255, 0, 255, ACTOR_EN_DOOR, 0, 0, 345, -19329, 0x0282 }` | HDR scene.c:37 |
| Sides | front room 0, bgCam -1 (generic door camera); back room 0, bgCam -1 | z64scene.h:371-374 |
| Position | (0, 0, 345); Unity (0.00, 0.00, -3.45) | HDR |
| Yaw | rotY -19329 = `0xB47F`; `(rotY >> 7) & 0x1FF` = 360; `DEG_TO_BINANG(360)` = `TRUNCF_BINANG(65536.0)` = 0 (`z_actor.c:3613`; `include/z64math.h:168, 172`). Effective yaw 0: local +Z = world +Z, toward the exit stub. The West doors write 0 as `0x007F`; this one writes a full turn (C, same result) | C |
| csId / mask | `0xB47F & 0x7F = 0x7F` none; mask always `HALFDAYBIT_ALL` (`z_actor.c:3619`) | C |
| Params | spawned `TRANSITION_ACTOR_PARAMS(0, 0x0282) = 0x0282` (`include/z64door.h:23`): type `(p >> 7) & 7 = 5` `ENDOOR_TYPE_SCHEDULE`, type var 2 = `ENDOOR_SCH_TYPE_LOTTERY_SHOP` (`ovl_En_Door/z_en_door.h:9-20`, `:57`). Identical params to West TA2 (`Z2_ICHIBA.c:62`) | C |
| Leaf art | `sObjectInfo[DOOR_OBJKIND_SCHEDULE + 2]` = `DOOR_OBJINFO_19` `{ -1, DOOR_DL_LOTTERY_CURIOSITY_SHIP_MAYOR_HOUSE, OBJECT_WDOR01 }` (`z_en_door.c:347`, `:148`, `:214`); both side DLs are `gLotteryCuriosityShopMayorHouseDoorDL` object_wdor01 `0x548` (`z_en_door.c:313-314`); drawn as `gDoorSkel` (gameplay_keep `0x22B28`, `assets/xml/objects/gameplay_keep.xml:941`) limb 4 (`z_en_door.c:638-660`); bank loaded by the room list (room.c:27); default actor scale 0.01 | OVL |
| Schedule side | the schedule runs only for `playerPosRelToDoor.z > 0` (`z_en_door.c:537`), i.e. on the exit-stub side. A player inside the shop is on local -Z, so leaving never meets a closed message (C) | OVL, C |
| Probe from inside | `doorDirection = -1` (`z_en_door.c:496`); player placed 22 u on its own side at (0, 0, 323) (`z_player.c:6756-6757`); floor probe 22 u on the far side at (0, 10, 367) (`z_player.c:6779-6781`; `func_80835BF8` `:6548-6551`) -> polygon 25, surface 1, exit 0 -> `0xD480` (`:6783`). Scratch `analyze.py` | C |
| Probe from the stub side | (0, 10, 323) -> polygon 0, no exit. Not reachable in play: standing on the stub already fires exit 0 through the standing-floor call (`z_player.c:12572`) | C, INF |

The back wall (polygons 12, 13, z 330) spans the full width x -105..105, Y 0..180, so collision has no walkable opening into the door alcove (z 330..360, x -30..30). The player reaches the door only through the door action, which places Link at z 323 (C).

### 2.5 Cross-check with the West door (west_clock_town.md 2.4.1, PORTAL_REGISTER rows)

| Item | West side (`Z2_ICHIBA`) | Lottery side | Result |
| --- | --- | --- | --- |
| Door | TA2 (-1432, 200, -796), raw rotY 28927 = `0x70FF`, yaw 225, params `0x0282` (`Z2_ICHIBA.c:62`) | ta00 (0, 0, 345), yaw 0, params `0x0282` | same schedule type, same leaf bank |
| Exit trigger | polys 370, 371, surface 17 `0x00000900` (exit+1 = 9 -> exit 8 `0x6C00`), 36 u behind TA2 on its local -Z (`Z2_ICHIBA.c:812`, `:1186-1187`) | polys 24, 25, surface 1, exit 0 `0xD480`, 75 u from ta00 on its local +Z | agrees with the West dossier and PORTAL_REGISTER |
| Schedule-checked side | front = street (West dossier) | front = exit stub | the closed message is seen only when entering from the street |
| Arrival | Lottery start 0 (0, 0, 320) yaw 180 mode 0xE | West start 8 (-1453, 200, -814) yaw 229 mode 0xF bgCam 5 | agrees with PORTAL_REGISTER rows `portal.west_clock_town.lottery_shop` and `portal.lottery_shop.west_clock_town` |
| Open hours | West: "none = openable (06:00-23:00)" | same script | PORTAL_REGISTER says "open hours UNKNOWN" in both door rows: resolved here, 06:00-23:00 every day (5.5) |

### 2.6 Portal IDs (registered in `shared/city/ids.json`; no new IDs)

| Portal ID | Mode | Source evidence | Arrival |
| --- | --- | --- | --- |
| `portal.lottery_shop.west_clock_town` | door | ta00 + surface 1 (polys 24, 25), exit 0 `0xD480` | West start 8 |
| `portal.lottery_shop.west_clock_town.kickout` | special | a01 En_Time_Tag type 0, exit 0, 23:00, text `0x1887` | West start 8 |
| Inbound `portal.west_clock_town.lottery_shop` | door | West TA2 + surface 17, exit 8 `0x6C00` | start 0 here |

One room and one transition actor with both sides in room 0: no internal room portal exists or is needed.

## 3. Object banks (HDR room.c:24-29; IDs from `include/tables/object_table.h`)

| # | Bank | Object ID | Consumer (OVL) | Needed for Day 1 static |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_OBJ_TOKEIDAI` | 0x18C (`:413`) | a02 Obj_Tokeidai small wall clock (profile `z_obj_tokeidai.c:67`) | yes (already extracted for South) |
| 1 | `OBJECT_KUJIYA` | 0x263 (`:628`) | a03 En_Kujiya (profile `z_en_kujiya.c:39`) | yes |
| 2 | `OBJECT_WDOR01` | 0x274 (`:645`) | ta00 En_Door leaf (`z_en_door.c:214`) | yes |
| 3 | `OBJECT_HARIKO` | 0x1F2 (`:515`) | a04 Obj_Hariko (profile `z_obj_hariko.c:28`) | yes |

Implicit: `gameplay_keep` (always loaded): door skeleton `gDoorSkel` and the profiles of En_Test4 (`z_en_test4.c:27`), En_Time_Tag (`z_en_time_tag.c:42`) and Obj_Sound (`z_obj_sound.c:20`). All four listed banks have a consumer.

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): id `& 0x1FFF`; each rotation `(raw >> 7) & 0x1FF`, converted with `DEG_TO_BINANG` unless the id carries the axis flag (`0x8000` rotY, `0x4000` rotX, `0x2000` rotZ), in which case it is passed as a plain integer (values above 180 wrap by -360). csId = `rawRotY & 0x7F` (0x7F = none, `:3653`); half-day mask = `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 = all (`:3654`; `:3576-3580`).
- `Actor_SpawnSetupActors` (2546-2575) spawns an entry only if its mask meets the current half-day bit; `Actor_InitHalfDaysBit` (2491-2499) gives Day 1 day = `0x080`. No entry here has id bit `0x800`.
- Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled`, `billboard-effect`, `controller-invisible`, `door-transition` (schema 1 vocabulary). "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (6 entries, HDR room.c:31-50, decode C; scratch `actors.py`)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_TEST4` (0x15A) | (0, 0, 441) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | 0x3FF all | 9 | 0xFD00 | gameplay_keep | controller-invisible | yes |
| 1 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-17, 0, 225) | 0x0B87 / 0x007F / 0x007F | 23 / 0 / 0 (all raw integers) | 0x3FF all | none | 0x0080 | gameplay_keep | controller-invisible | yes |
| 2 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (0, 125, 321) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF all | none | 0xA07F | object_obj_tokeidai | static-mesh | yes |
| 3 | `ACTOR_EN_KUJIYA` (0x239) | (0, 0, 105) | 0x0007 / 0x000A / 0x007F | 0 / 0 / 0 (shape yaw replaced at init) | 0x3FF all | 10 | 0x00FF | object_kujiya | static-mesh (state-rotated) | yes |
| 4 | `ACTOR_OBJ_HARIKO` (0x20A) | (80, 50, 130) | 0x0007 / 0xAF7F / 0x007F | 0 / 350 / 0 | 0x3FF all | none | 0x0000 | object_hariko | static-mesh | yes |
| 5 | `ACTOR_OBJ_SOUND` (0x0F0) | (-4, 154, 119) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0144 | gameplay_keep | controller-invisible | yes |

Plus the transition actor ta00 (section 2.4): `ACTOR_EN_DOOR` (0x005), (0, 0, 345), yaw 0, params `0x0282`, class door-transition, always present.

Placement notes (C from collision, section 7): a03 sits at z 105, behind the full-width booth wall at z 135; a04 is 50 u up at z 130, also behind it; a02 hangs on the back wall 25 u above the door lintel (Y 100), facing -Z; a00 lies in the exit-stub zone at z 441, outside the room shell; a01 is mid-room; a05 is 154 u up behind the booth wall.

### 4.3 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init / params | Draw | Presence and state at Day 1 noon |
| --- | --- | --- | --- |
| En_Test4 (a00) | three-day controller; csId 9 -> scene cutscene entry 9 `{900, -1, csCamId 0, -1, additionalCsId 9, ...}` (scene.c:75), so day and night transitions both use csId 9 (`z_en_test4.c:322-337`); stars `(params >> 10) * 100` with s16 params `0xFD00` = -768 gives -100 and is skipped by `numStars > 0` (`z_en_test4.h:8`, `.c:614-619`; params is s16, `include/z64actor.h:103`); bell volume index `(p >> 5) & 0xF = 8`, used as `& 7 = 0` (`z_en_test4.h:9`; `src/audio/code_8019AF00.c:4451-4453`) | profile draw NULL (`z_en_test4.c:32`) | always; runs the 18:00 day/night swap while the player is inside (`:404-440`) |
| En_Time_Tag (a01) | type `(p & 0xE000) >> 13 = 0` `TIMETAG_KICKOUT_DOOR`; text `(p & 0x1FE0) >> 5 = 4`; exit `p & 0x1F = 0`; hour = `home.rot.x` = 23, minute = `home.rot.y` = 0 (`z_en_time_tag.h:10-15`, `:23-30`); no init branch for type 0 (`.c:50-89`) | profile draw NULL (`.c:47`) | always; 23:00 kickout (5.5) |
| Obj_Tokeidai (a02) | type `(p & 0xF000) >> 12 = 0xA` `OBJ_TOKEIDAI_TYPE_SMALL_WALL_CLOCK` (`z_obj_tokeidai.h:6-20`); scale 0.01, draw `ObjTokeidai_Clock_Draw`, `ObjTokeidai_Clock_Init`, idle `WallClock_Idle` (`.c:254-258`); follows `CURRENT_TIME` every frame (`:719-723`) | minute ring `0xCF28`, center/hand `0xBEE8`, `gWallClockClockFaceDL` `0xF518` (wall-clock face, chosen for types 9 and 10), sun/moon panel `0xC368` (`.c:811-845`; offsets `assets/xml/objects/object_obj_tokeidai.xml:47-67`) | always; time-posed. ROM_REFERENCE.md lines 54-60 give the noon clock rules and already name `0xF518` "a different wall-clock face": this is its user |
| En_Kujiya (a03) | `ACTORCAT_NPC`, flags attention + friendly + lock-on-disabled (`z_en_kujiya.c:12`, `:35-45`); scale 0.1 (`:58`); attention flag cleared, range type 6 (`:60-61`); focus = pos + 30 Y (`:63-64`); shape yaw 0 if the current day's ticket is held and 06:00 <= t < 18:00, else `0x7555` (`:66-70`); no collider or dynapoly (grep) | `AnimatedMat_Draw(gLotteryShopTexAnim)` then 10 opaque DLs: cylinder `0x2A80`, back sign `0x3030`, open box `0x3248`, scrolling effects `0x3358`, red/green/blue rupees `0x34A8`/`0x35B8`/`0x36B0`, target arrow and sign `0x37C0`, scrolling arrow `0x3C80`, mask `0x3D58` (billboard mtx + `gLotteryShopMaskPosMtx` `0x2A40`) (`:366-388`; XML `object_kujiya.xml`) | always; fresh save -> yaw `0x7555` (165.0 deg). Mechanics in section 5 |
| Obj_Hariko (a04) | "Little Cow Statue Head" (`z_obj_hariko.c:4`, ANN); scale 0.1; head rotation 0 at init (`:36-46`); bobs only while a quake is active (`:58-85`) | `gHarikoBodyDL` `0x80` + `gHarikoFaceDL` `0x110` under the head rotation (`:87-105`) | always; rest pose |
| Obj_Sound (a05) | type `(p >> 8) & 0xFF = 1` `OBJ_SOUND_TYPE_BGM`, sfxType `(p >> 7) & 1 = 0`, id `p & 0x7F = 0x44` (`z_obj_sound.h:8-17`; `.c:28-38`) = `NA_BGM_SHOP` (`Sequence_68`, `sequence_table.h:156`, ordinal 68); from its second update plays positional BGM at the actor (`.c:48-66`); destroy resets BGM (`:40-46`) | profile draw NULL (`.c:25`); type 1 sets no draw | always |
| En_Door (ta00) | schedule door: no dynapoly (only framed doors load a mesh, `z_en_door.c:337-341`), no collider (grep) | section 2.4 | always (`HALFDAYBIT_ALL`) |

### 4.4 Counts

| | Default header |
| --- | --- |
| Room actor entries | 6 (+ 1 transition actor) |
| Day 1 daytime candidates by mask | 6 of 6 (every mask is 0x3FF) |
| static-mesh | 3 (a02, a03, a04) |
| controller-invisible | 3 (a00, a01, a05) |
| door-transition | 1 (ta00) |
| npc-scheduled / posed-skeletal / billboard-effect | 0 / 0 / 0 (the mask DL is a billboard part of a03) |
| Actor cutscene entries used by room actors | 2 of 11 (9 by a00, 10 by a03) |

## 5. Venue mechanics index (INT-06 scope)

Scope (FULL_CITY_PLAN section 6): "Entry/draw/reward rules, day rollover and repeat-claim prevention based on source." The dispatch's guess `ovl_En_Takaraya` is the Treasure Chest Shop; the lottery is `ovl_En_Kujiya` ("Clock Town - Lottery Shop", `z_en_kujiya.c:4`, ANN). Index only; no Unity design.

### 5.1 Who does what

| Part | Role | Source |
| --- | --- | --- |
| a03 En_Kujiya | sale, code entry, announcement, comparison, payout, cylinder turn | `ovl_En_Kujiya/z_en_kujiya.c` (388 lines) |
| Message system | 3-digit entry box; prints winning and guessed codes | `z_message.c:714-758` (`Message_ControlLotteryCodeInput`), `:3735-3736`, `:5106-5111`; `z_message_nes.c:805-806`, `:1513-1529`, `:1599-1607` |
| `Inventory_SaveLotteryCodeGuess` | packs the three digits | `src/code/z_inventory.c:730-737` |
| `Sram_GenerateRandomSaveFields` | creates the winning codes | `src/code/z_sram_NES.c:705-724` |
| ta00 / West TA2 En_Door | open hours | `ovl_En_Door/scheduleScripts.schl:105-112` |
| a01 En_Time_Tag | 23:00 kickout | `ovl_En_Time_Tag/z_en_time_tag.c:285-339` |
| a00 En_Test4 | day/night swap inside, day advance | `ovl_En_Test4/z_en_test4.c:404-440` |

### 5.2 State storage

| State | Location | Written | Read | Reset |
| --- | --- | --- | --- | --- |
| Winning codes | `gSaveContext.save.saveInfo.lotteryCodes[3][3]`, s8, save offset 0xFC8 (`include/z64save.h:320`), indexed `[CURRENT_DAY - 1][digit]` | `Sram_GenerateRandomSaveFields`: each digit `Rand_S16Offset(0, 10)` = 0..9 (`z_sram_NES.c:716-724`; `src/code/z_lib.c:268-270`); only callers `Sram_InitNewSave` (`:945`) and `Sram_InitDebugSave` (`:1165`) | `CHECK_LOTTERY_NUMBERS` (`z_en_kujiya.c:47-53`); message code `MESSAGE_LOTTERY_CODE` (`z_message_nes.c:1599-1603`) | never: no other writer in `src/` (grep). The three daily codes are fixed for the life of the file and repeat every cycle (C) |
| Player's guess | `HS_LOTTERY_GUESS` = `highScores[5]`, low 16 bits, digits in nibbles 8-11, 4-7, 0-3 (`z64save.h:179`, `:308`, `:624-625`) | after the entry box (`z_en_kujiya.c:202-207`; `z_inventory.c:730-737`) | `CHECK_LOTTERY_NUMBERS`; `MESSAGE_LOTTERY_CODE_GUESS` (`z_message_nes.c:1604-1607`) | not touched by `Sram_ClearHighscores` (`z_sram_NES.c:404-414`, called at cycle end `:636`). One slot for all days |
| Ticket held | `WEEKEVENTREG_33_10` day 1, `_33_20` day 2, `_33_40` day 3 (`z64save.h:1010-1012`) | `EnKujiya_SetBoughtTicket` (`z_en_kujiya.c:263-280`) | `EnKujiya_CheckBoughtTicket`, current day only (`:237-261`) | `EnKujiya_UnsetBoughtTicket` at claim (`:285-302`); all of weekEventReg[33] at cycle end (`sPersistentCycleWeekEventRegs[33] = 0`, `z_sram_NES.c:123`; loop `:486-495`) |
| Rupees | `saveInfo.playerData.rupees` | `Rupees_ChangeBy(-10)` on purchase (`z_en_kujiya.c:116`); +50 on a win (5.4) | `< 10` test (`:110`) | 0 at cycle end (`z_sram_NES.c:645`) |

No other code reads the three ticket flags or `lotteryCodes` (grep of `src/` and `include/`, C).

### 5.3 Entry rule: buying a ticket (06:00 <= t < 18:00, current day's ticket not held)

| Step | Text id (ANN meaning) | Code |
| --- | --- | --- |
| 1 | `0x2B5C` "Pick 3 numbers" | talk accepted, `CLOCK_TIME(6,0) <= t < CLOCK_TIME(18,0)`, no ticket (`z_en_kujiya.c:82-90`); talk offered only within xz 100 u of (0, 105) (`:102-104`) |
| 2 | `0x2B5D` "Buy or not" (choice) | `:139-142` |
| 3a | `0x2B62` "Not enough Rupees" | choice 0 with rupees < 10: `NA_SE_SY_ERROR` (`:109-113`) |
| 3b | `0x2B5F` "Enter number" | choice 0 with rupees >= 10: decide sfx, **`Rupees_ChangeBy(-10)`** (`:114-119`) |
| 3c | `0x2B5E` "Too bad" | any other choice: cancel sfx (`:120-124`) |
| 4 | (entry box) | `0x2B5F` ends in `MESSAGE_INPUT_LOTTERY_CODE` 0xD5 (`include/message_data_fmt_nes.h:73`) -> `TEXTBOX_ENDTYPE_INPUT_LOTTERY_CODE` (`z_message_nes.c:805-806`) -> `TEXT_STATE_INPUT_LOTTERY_CODE` (`z_message.c:3735-3736`). Digits start 1-1-1 (`z_message_nes.c:1513-1529`); stick up/down cycles the selected digit 0-9 with wrap, left/right moves among 3 digits (`z_message.c:714-758`) |
| 5 | `0x2B60` "Will announce winning numbers after 6" | on advance: `Inventory_SaveLotteryCodeGuess` (`:202-207`) |
| 6 | close | on advance of `0x2B60`: `EnKujiya_SetBoughtTicket`, close, `TurnToClosed` (`:144-148`): yaw smooth-steps to 0 (step 0x16C, windmill sfx) under csId 10, holds 20 frames, stops the cutscene (`:332-358`) |
| later | `0x2B61` "Come back tomorrow" | ticket held in daytime (`:84-86`): one ticket per day |

Order (C): the 10 rupees leave at step 3b; the ticket flag is set only at step 6. Whether the entry box can be backed out of between them is NI (U-03). The overlay has no player-form test (grep).

### 5.4 Draw and reward rules (t >= 18:00 or t < 06:00; reachable 18:00-23:00)

- There is no draw event: the "winning numbers" are the file's fixed `lotteryCodes[CURRENT_DAY - 1]`; after 18:00 the shop reveals and compares them (C).
- Cylinder: inside at 18:00 with the ticket held and yaw 0 -> `TurnToOpen` (`:100-101`, `:304-330`), yaw to `0x7555` under csId 10. Entering after 18:00: init already picks `0x7555` (`:66-70`). The shop tests `t >= 18:00`, so 18:00 itself counts as night here while actor half-days still call it day (`z_actor.c:2494-2496`).
- Claim: talk, night, ticket -> `0x2B64` "Announce winning numbers" (`:91-93`) -> advance: **`EnKujiya_UnsetBoughtTicket` first**, timer 20, `0x2B65` "Your numbers vs winning numbers" (`:150-155`) -> 20-frame hold (`:130-135`) -> advance: all three digits equal in order -> `0x2B66` "Won 50 Rupees", else `0x2B67` "Lost, come back tomorrow" (`:157-165`).
- Reward: `0x2B66` advance -> close, `Actor_OfferGetItem(GI_RUPEE_PURPLE, 500, 100)` every frame until `Actor_HasParent` (`:166-171`, `:219-225`), then wait for the text to end (`:231-235`). `GI_RUPEE_PURPLE` (`include/z64item.h:420`) -> `ITEM_RUPEE_PURPLE` 0x88 (`z_player.c:2085-2087`; `z64item.h:331`) -> `Rupees_ChangeBy(sRupeeRefillCounts[0x88 - 0x84] = 50)` (`src/code/z_parameter.c:2543`, `:2856-2857`). Net +40 per winning ticket.
- Night talk with no ticket: `0x2B63` "Exchanging winning tickets" (`:94-97`), no state change.
- Odds (C from the digit ranges): 1 in 1000 for a blind guess. Because the codes never change for a file (5.2), a known code wins every day of every later cycle (INF).

### 5.5 Open hours, door schedule and kickout

| Rule | Value | Source |
| --- | --- | --- |
| Door schedule | `if_time_range (6, 0, 23, 0) { return_none } else { return_s (0x1C) }`, no day branch | `ovl_En_Door/scheduleScripts.schl:105-112` |
| Range test | `start <= now <= end - 1` in script time (06:00-shifted) | `src/code/z_schedule.c:25-45`; `include/z64scripts.h:7-12` |
| Closed response | text `0x1800 + 0x1C = 0x181C`, door stays shut (`PLAYER_DOORTYPE_TALKING`) | `z_en_door.c:537-546` |
| Where checked | front side only: the West street side of TA2; never from inside (2.4) | `z_en_door.c:537` |
| Day 1 12:00 | open; open 06:00-23:00 on Days 1-3 | C |
| Kickout | a01 at hour 23, minute 0, not in cutscene mode: halts the player, text `0x1883 + 4 = 0x1887`, then `nextEntrance = setupExitList[0]` = `0xD480` with door sfx | `z_en_time_tag.c:315-339`, `:285-294`, `:303` |
| Kickout hold | `WEEKEVENTREG_KICKOUT_WAIT` path; set only by En_Aob_01, En_Fu, En_Mm3, En_Syateki_Man, En_Takaraya (grep), none in this scene | `z_en_time_tag.c:297-312`, `:327-332` |
| Claim window (INF) | 18:00 to 23:00 of the purchase day. From 23:00 to 06:00 the door is shut; at 06:00 CURRENT_DAY advances and the old day's bit is never read again | 5.2, 5.6 |

### 5.6 Day rollover and cycle reset

- `CURRENT_DAY = save.day % 5` (`z64save.h:518`); day++ at 06:00 (`z_en_test4.c:115-119`, `z_sram_NES.c:655-659`; SOURCE_REGISTER 2.3). Each day reads its own ticket bit and its own code row (`z_en_kujiya.c:237-261`, `:47-53`).
- An unclaimed ticket lapses silently at the day change: its bit stays set but unread until the cycle reset clears register 33 (C from the code paths; that no other path reads it is from grep).
- Cycle reset (`Sram_SaveEndOfCycle`, `z_sram_NES.c:430` onward): ticket bits and rupees cleared; `lotteryCodes` and `HS_LOTTERY_GUESS` kept.
- The clock runs inside the shop (room `TIME_SETTINGS(255, 255, 3)`), so 18:00 can pass indoors; a00 then runs its swap with csId 9 (`z_en_test4.c:404-440`).

### 5.7 Repeat-claim prevention (summary)

| Case | Outcome | Source |
| --- | --- | --- |
| Second purchase same day | refused with `0x2B61` while the ticket is held; after a claim the buy window (06:00-18:00) of that day is over | `z_en_kujiya.c:83-86` |
| Second claim same night | ticket bit cleared before the comparison, so the next talk gives `0x2B63` | `:150-152`, `:91-97` |
| Lost ticket | same clear, no payout | `:150-165` |
| Ticket from an earlier day | never read (current-day bit only) | `:237-261` |
| Payout atomicity | the bit is cleared at `0x2B64`; the 50 rupees arrive only when the get-item is accepted. Behavior if that is interrupted: NI (U-04) | `:150-152`, `:219-225` |

### 5.8 Cameras, cutscenes, lighting, sound

| Item | Value | Source |
| --- | --- | --- |
| Room camera (bgCam 0) | `CAM_SET_CIRCLE3` 0x25 (`include/z64camera.h:184`); data pos (-93, 31, 298), rot (0, 17840 = 98.0 deg, 0), fov -1 | scene.c:113-121 |
| bgCam 1 | `CAM_SET_NONE` (0); only on surface type 4 (wall ornaments) | scene.c:121, :126 |
| Actor cs camera 0 | `CAM_SET_FIXED1` 0x1F (`z64camera.h:178`), pos (-16, 105, 320), rot (-1820, -22938, 0) = (-10.0, -126.0, 0) deg; entry 9 (a00) | scene.c:87-90, :97, :75 |
| Actor cs camera 1 | `CAM_SET_FIXED1`, pos (-49, 2, 321), rot (-3823, 30219, 0) = (-21.0, 166.0, 0) deg; entry 10 (a03 turn cutscenes, `z_en_kujiya.c:310-316`, `:338-344`) | scene.c:91-93, :98, :76 |
| Cutscene entries 0-8 | chain 0 -> 1 -> ... -> 8, negative csCamIds (global camera settings); no room actor uses csId 0-8 | scene.c:66-74; U-05 |
| Door camera | bgCam -1 both sides (generic door camera, `Camera_ChangeDoorCam`, `z_player.c:6791-6795`) | scene.c:37 |
| Lighting | `LIGHT_MODE_SETTINGS`, 4 identical settings: indoor light does not change with time (C) | scene.c:21, :54-63 |
| Sound | scene seq 0, ambience 19; positional `NA_BGM_SHOP` from a05 at (-4, 154, 119); windmill sfx during cylinder turns | sections 1, 4.3 |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default header)

The venue is **open** at Day 1 12:00 (door schedule returns none, 5.5), so the baseline is also the first open profile. Assumptions: fresh save (`WEEKEVENTREG_33_10` clear, so a03 shape yaw `0x7555`), Human form, clock 12:00 (wall clock noon pose). All 6 room actors pass the mask; none is removed by state. World placement: `world = actorPos + actorRot x (actorScale x localVertex)` (ROM_REFERENCE.md).

| # | Component / params | N64 position (Unity) | Yaw | Scale | Display lists (bank, offset) | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room | `Z2_TAKARAKUJI_room_00` | origin | 0 | 1 | opaque `0x8E0`, `0x18B0`, `0x2178`, `0x2C00` (cullable entries, or wrapper `0x2E00`); XLU `0x2E30` empty (wrapper `0x2E38`) | 287 triangles, section 8 |
| a03 | Lottery booth, `0x00FF` | (0, 0, 105) (0.00, 0.00, -1.05) | `0x7555` = 165.0 (shape) | 0.1 | object_kujiya `0x2A80`, `0x3030`, `0x3248`, `0x3358`, `0x34A8`, `0x35B8`, `0x36B0`, `0x37C0`, `0x3C80`, `0x3D58` | texture scroll `gLotteryShopTexAnim` `0x6198` and the billboard mask: approximate and label |
| a02 | Small wall clock, `0xA07F` | (0, 125, 321) (0.00, 1.25, -3.21) | 180 | 0.01 | object_obj_tokeidai `0xCF28`, `0xBEE8`, `0xF518`, `0xC368` | noon pose per ROM_REFERENCE.md steps 2-3 (same draw function, `z_obj_tokeidai.c:811-845`) |
| a04 | Cow statue head, `0x0000` | (80, 50, 130) (0.80, 0.50, -1.30) | 350 | 0.1 | object_hariko `0x80`, `0x110` | rest pose |
| ta00 | Door, `0x0282` | (0, 0, 345) (0.00, 0.00, -3.45) | 0 | 0.01 | `gDoorSkel` gameplay_keep `0x22B28`, limb 4 = object_wdor01 `0x548` | closed rest pose approximated |

**Excluded by half-day mask:** none. **Excluded by state or schedule:** none. **State-dependent pose:** a03 yaw (0 when the Day 1 ticket is held before 18:00). **No geometry:** a00 En_Test4, a01 En_Time_Tag, a05 Obj_Sound. **Not part of the static list:** Link, Tatl, talk and turn cutscenes, cow-head bob, positional BGM.

## 7. Collision summary (C from scene.c:113-361; scratch `analyze.py`, `clusters.py`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 118 / 100 / 5 |
| Water boxes | 0 (`0, NULL`, scene.c:360) |
| Camera data (`BgCamInfo`) | 2 (5.8) |
| Header bounds | min (-110, 0, -15), max (114, 180, 480); equal to the vertex extents. Unity x -1.10..1.14, y 0..1.80, z -4.80..0.15 |
| Polygon kinds | 7 floors (normal Y > 0.5), 91 walls, 2 ceilings |
| Floors | 0, 1: room floor Y 0, x -105..105, z 105..330 (z 105-135 lies behind the booth wall); 10, 11: door alcove Y 0, x -30..30, z 330..360; 24, 25: exit stub (2.3); 55: a 54 deg facet of a wall ornament at Y 131-138 (unreachable) |
| Walls | side walls x = -105 / +105 (2, 3 / 14, 15; z -15..330, Y 0..180); back wall z = 330, full width (12, 13); booth wall z = 135, full width, surface 2 wood (26, 27); booth front, surface 3 wood (28-51): facets from (+-75, 135) via (+-65, 143) and (+-38, 170) to (0, 180), in two bands Y 0-30 and 45-180 with no polygon between 30 and 45 and no top surface; two corner blocks x +-75..105, z 285..330, 45 u high, front and inner faces only (16-23); alcove side walls x = +-30, Y 0..100 (6-9) |
| Ceilings | alcove lintel Y 100 (4, 5); no room ceiling polygon (shell walls end at Y 180) |
| Surface types | 0 `{0x0, 0x1002}` stone; 1 `{0x100, 0x1002}` stone, exit 0; 2 `{0x0, 0x100A}` wood; 3 `{0x0, 0x17CA}` wood, lightSetting 31; 4 `{0x1, 0x17C2}` stone, bgCam 1, lightSetting 31 (scene.c:125-126; decode z64bgcheck.h:79-97) |
| Materials | stone (types 0, 1, 4): 74 polys; wood (types 2, 3): 26 |
| Flags | ignore-camera on polys 4-11 (alcove) and 24-25 (stub); echo 2 on every type; no conveyor, hookshot, wall type, floor type, floor property, soft or horse-block bits |
| Walkable area | one level, Y 0: z 135..330 between the side walls, minus the booth front (out to z 180 at x 0) and the corner blocks. No ladder, water, void, slope or climbable surface |
| Talk zone (INF from 5.3) | xz within 100 u of (0, 105): in front of the booth, z 180-205 at x 0 and z 135-171 at x +-75 |

Wall-ornament clusters (surface 4, 8 polygons each, purpose NI, U-07):

| Polys | Side | x | Y | z |
| --- | --- | --- | --- | --- |
| 52-59 | left | -105..-85 | 95..160 | 270..312 |
| 92-99 | left | -110..-85 | 98..158 | 217..259 |
| 76-83 | left | -105..-85 | 99..151 | 165..207 |
| 60-67 | right | 88..114 | 85..153 | 260..313 |
| 84-91 | right | 88..108 | 88..154 | 211..253 |
| 68-75 | right | 88..107 | 93..149 | 160..202 |

Proposed camera anchors (registered IDs, `SOURCE_REGISTER.md` 4.6; proposals, not captures):

| Camera ID | Proposed eye / target | Shows |
| --- | --- | --- |
| `cam.lottery_shop.entry` | eye at start 0 (0, 0, 320) plus eye height, yaw 180 | booth front and a03 as first seen after the door |
| `cam.lottery_shop.reverse_door` | eye (0, eye height, 200), facing +Z | door ta00, back wall, wall clock a02, corner blocks |
| `cam.lottery_shop.fixture` | eye (0, eye height, 230), target a03 focus (0, 30, 105) | booth DLs, rupee displays, cow head a04 |

A source-faithful alternative for `entry` is the room camera data point (-93, 31, 298), rot Y 98 deg (5.8); a fixed shot does not reproduce its CIRCLE3 behavior (INF).

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 13: 11 textures + 2 TLUTs |
| By format | ci8 10 (`Tex_000BD0` 8x8, `000C10` 16x32, `000E10` 16x16, `000F10` 32x32, `001310` 16x16, `001410` 16x64, `001810` 16x32, `001A10` 16x32, `001C10` 16x32, `001E10` 32x32); i8 1 (`000B90` 8x8); rgba16 2 (`TLUT_002210`, `TLUT_002410`, 256-entry palettes) |
| Location | all in the scene file (segment 2); the room file holds none; all 13 are referenced by room DLs |
| Room `Gfx` / `Vtx` arrays | 7 Gfx (4 content, 2 wrappers, 1 empty XLU) / 4 Vtx arrays, 126 + 160 + 71 + 128 = 485 vertices |
| Room shape | cullable, 4 entries (centers (-1, 88, 211) r148, (0, 47, 210) r152, (0, 90, 187) r210, (2, 122, 236) r134): 4 opaque DLs, 1 empty XLU DL (room.c:56-68) |
| Triangles | 141 `gsSP2Triangles` + 5 `gsSP1Triangle` = 287 |
| Material state | 18 texture loads; 1 combiner (2-cycle texel x shade, then x primitive); 6 distinct primitive colors (values not copied); render modes 3 opaque-surface, 1 texture-edge (cutout); 7 geometry-mode loads, 4 back-face culled, 3 two-sided; fog on all |
| Object textures (XML names) | object_kujiya 9: sign text i8 64x16, mask rgba16 16x32, gradient i8 4x4, cylinder top/bottom rgba16 16x32, middle stripes rgba16 16x32, sign border rgba16 16x64, box base rgba16 16x16, rainbow rgba16 4x32, rupees rgba16 32x32, plus `gLotteryShopTexAnim`; object_hariko 2 (rgba16 8x8, 16x16); object_wdor01 1 (rgba16 32x64) |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_TAKARAKUJI` (C above); `object_obj_tokeidai` (extracted for South). Binaries for every bank below exist under `extracted/n64-us/baserom/` (checked by `ls`: object_kujiya 25008 B, object_hariko 1056 B, object_wdor01 5808 B, gameplay_keep 599520 B).

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_kujiya`, `object_hariko`, `object_wdor01` | a03 booth, a04 head, door leaf |
| Door skeleton | `gameplay_keep` | `gDoorSkel` 0x22B28 (shared with every venue door; extract once for all REF-09 packets) |

Pattern from `tools/reference/README.md:29-32` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_kujiya','object_hariko','object_wdor01','gameplay_keep')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

`gameplay_keep` has not been extracted with this tool before (North section 8); check output size and warnings separately. `object_kujiya` carries a `Mtx` and a `TextureAnimation` node (XML); confirm ZAPD emits both.

## 10. Proposed region record

Written directly in schema 1 (`shared/city/schema/region.schema.json`, PIPE-01 `7a7a08c`). `portalIds` are exactly the two portals whose `from` is `loc.lottery_shop` in `shared/city/ids.json`. The interior shares no frame with the exteriors (`not_shared`). Not run through `region_from_dossier.py` or `validate_city.py` (section 11).

```json
{
  "schemaVersion": 1,
  "regionId": "loc.lottery_shop",
  "displayLabel": "Lottery Shop",
  "source": {
    "sceneSymbol": "Z2_TAKARAKUJI",
    "sceneEnum": "SCENE_TAKARAKUJI",
    "sceneTableIndex": "0x39",
    "entranceTableSceneIndex": "0x36",
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
      "fresh save: no week-event flags",
      "WEEKEVENTREG_33_10 clear (no Day 1 ticket): a03 shape yaw 0x7555",
      "player form Human",
      "clock 12:00: venue open (door schedule 06:00-23:00), wall clock a02 in noon pose",
      "scene has no alternate headers"
    ]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TAKARAKUJI", "bytes": 9744, "sha256": "0dac00574b646c70833f24776c3f3cf06f41782d1423342a1d221c0de3cde814" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_TAKARAKUJI_room_00", "bytes": 11856, "sha256": "d667f3accfc24d6c34449e9d379afb48bfae5f0aef917b0f18a077710dbdc615" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TAKARAKUJI/Z2_TAKARAKUJI.c", "bytes": 18022, "sha256": "bdbbb29510fc89f10d43b92abd4a1063f852afc82e1a3e90478ee2963f6ce0aa" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_TAKARAKUJI/Z2_TAKARAKUJI_room_00.c", "bytes": 21241, "sha256": "3610cf955d6041578d517755a50d86ec78b8c17c8626fc7d062ab9e1977677e4" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_TAKARAKUJI/Z2_TAKARAKUJI.xml", "bytes": 416, "sha256": "0e97c34d0d4af35f441fea5090d178e4604e033b79251b15eaa680e1dc15e786" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": [],
      "status": "not_shared",
      "evidence": "interior local frame around the origin (collision x -110..114, z -15..480); joined to West Clock Town only by the door transition (West TA2 at (-1432, 200, -796))"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-110, 0, -15], "max": [114, 180, 480] },
    "sourceCollisionVertices": { "min": [-110, 0, -15], "max": [114, 180, 480] },
    "unityCollisionVertices": { "min": [-1.1, 0.0, -4.8], "max": [1.14, 1.8, 0.15] }
  },
  "anchors": [
    { "id": "anchor.lot.spawn0_from_west", "source": [0, 0, 320], "unity": [0.0, 0.0, -3.2], "status": "proposed" },
    { "id": "anchor.lot.door_ta00", "source": [0, 0, 345], "unity": [0.0, 0.0, -3.45], "status": "proposed" },
    { "id": "anchor.lot.booth_a03", "source": [0, 0, 105], "unity": [0.0, 0.0, -1.05], "status": "proposed", "note": "behind the booth wall at z 135" },
    { "id": "anchor.lot.wall_clock_a02", "source": [0, 125, 321], "unity": [0.0, 1.25, -3.21], "status": "proposed" },
    { "id": "anchor.lot.exit_stub", "source": [0, 0, 420], "unity": [0.0, 0.0, -4.2], "status": "proposed", "note": "mean vertex of exit polygons 24, 25" }
  ],
  "rooms": [
    { "id": "loc.lottery_shop.room_00", "sourceRoom": "Z2_TAKARAKUJI_room_00", "header": 0, "roomShape": "cullable, 4 entries", "displayLists": 7 }
  ],
  "components": [
    { "sourceKey": "Z2_TAKARAKUJI/h0/room_00/a03", "actorId": "0x239", "params": "0x00FF", "class": "static-mesh", "dls": ["object_kujiya:0x2A80", "object_kujiya:0x3030", "object_kujiya:0x3248", "object_kujiya:0x3358", "object_kujiya:0x34A8", "object_kujiya:0x35B8", "object_kujiya:0x36B0", "object_kujiya:0x37C0", "object_kujiya:0x3C80", "object_kujiya:0x3D58"], "status": "selected", "note": "shape yaw 0x7555 with no Day 1 ticket; texture scroll and billboard mask approximated" },
    { "sourceKey": "Z2_TAKARAKUJI/h0/room_00/a02", "actorId": "0x19C", "params": "0xA07F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected", "note": "small wall clock, scale 0.01, noon pose" },
    { "sourceKey": "Z2_TAKARAKUJI/h0/room_00/a04", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80", "object_hariko:0x110"], "status": "selected", "note": "rest pose" },
    { "sourceKey": "Z2_TAKARAKUJI/h0/transition/ta00", "actorId": "0x005", "params": "0x0282", "class": "door-transition", "dls": ["gameplay_keep:0x22B28 gDoorSkel", "object_wdor01:0x548"], "status": "selected", "note": "rest pose approximated" }
  ],
  "collisionOwnership": {
    "room": "Z2_TAKARAKUJICollisionHeader_000B60 (100 polys, 0 water boxes)",
    "waterBoxes": 0,
    "actorDynapoly": [],
    "actorCylinders": [],
    "note": "no collider or dynapoly in En_Kujiya, Obj_Tokeidai, Obj_Hariko or the schedule En_Door; the booth front is room collision"
  },
  "portalIds": [
    "portal.lottery_shop.west_clock_town",
    "portal.lottery_shop.west_clock_town.kickout"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a03 texture scroll gLotteryShopTexAnim", "a03 billboard mask orientation", "a03 yaw 0 ticket-held variant", "Link and Tatl"],
  "excluded": {
    "halfDayMask": [],
    "stateOrSchedule": [],
    "noGeometry": ["a00 En_Test4", "a01 En_Time_Tag", "a05 Obj_Sound"]
  },
  "unsupported": ["door skeleton rest pose", "texture scroll", "billboard DL", "positional BGM (a05)", "time-driven booth turn and cutscenes", "lottery rules (section 5)"],
  "evidence": ["docs/city/interiors/lottery_shop.md"],
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

## 11. Unknowns, next source questions, checks

Register cross-check (C): the LOCATION_REGISTER rows for `loc.lottery_shop` agree with the C on every count (scene 0x39, entrance index 0x36, title 0x112, DEFAULT draw, the `RESTRICTIONS_SET` value, no cycle flags; room_00 only, no alternates, 4 objects / 6 actors, 1 transition actor, 1 spawn, 2 exits with trailing `0x0000`, entrance list 2 / declared 1). The PORTAL_REGISTER rows `portal.west_clock_town.lottery_shop`, `portal.lottery_shop.west_clock_town` and `.kickout` agree on exit values, decode lines, polygons, distances, arrival poses and the 23:00 kickout; their "open hours UNKNOWN" is resolved by 5.5. The West dossier's TA2 row agrees (2.5). SOURCE_REGISTER U-10 for this venue: open at 12:00 on Days 1-3.

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | **Disagreement:** PORTAL_REGISTER section 6 ("Teleport-only spawns") lists "Lottery 1", but `Z2_TAKARAKUJI` has one spawn (`SCENE_CMD_SPAWN_LIST(1, ...)`, scene.c:20) and one declared entrance (`z_scene_table.c:1254-1260`); entrance-list row 1 (scene.c:46) is padding | REF-02 owner: was "Lottery 1" meant as the padding entrance-list row? If so, reword the register (do not add a spawn) |
| U-02 | Text content of `0x2B5C`-`0x2B67`, `0x181C`, `0x1887` (meanings here are overlay comments) | Read the message table entries for these ids to confirm meaning and the choice count of `0x2B5D`; record ids only, not text |
| U-03 | Can the player back out of the code entry (or the choice) after the 10 rupees are taken (5.3 step 3b vs step 6)? | In `z_message.c`, does B or any input end `TEXT_STATE_INPUT_LOTTERY_CODE` other than advance (`:5106-5111`, `Message_ShouldAdvance`)? |
| U-04 | Payout if the get-item offer is interrupted after the ticket bit is cleared | Read `Actor_OfferGetItem` and the player's get-item acceptance in talk state (`z_actor.c`, `z_player.c`); then a capture |
| U-05 | Actor cutscene entries 0-8 (scene.c:66-74) have no room-actor user | Search runtime-spawned actors and player/door code for scene csIds 0-8 in this scene; else record unused |
| U-06 | Wallet overflow on the 50-rupee prize | Read `Rupees_ChangeBy` and the accumulator clamp in `z_parameter.c` |
| U-07 | What the 6 surface-4 clusters, the booth's 30-45 u gap and the topless corner blocks model | Review room DLs `0x8E0`, `0x18B0`, `0x2178`, `0x2C00` against the cluster boxes in section 7; capture `cam.lottery_shop.reverse_door` |
| U-08 | Placement of the booth DLs relative to the collision booth front (object_kujiya not extracted) | Extract `object_kujiya` (section 9) and compare its vertex extents at scale 0.1 and yaw `0x7555` with the z 135-180 facets |
| U-09 | Kickout skipped if the player is in cutscene mode for the whole 23:00 minute (equality test, `z_en_time_tag.c:333-338`) | Which states hold `Play_InCsMode` for 45+ clock units here; edge case, capture only |
| U-10 | Talk rules beyond distance (facing, height) and whether the 100 u zone suits every form | Read `Actor_OfferTalk` in `z_actor.c`; the overlay has no form test |
| U-11 | `RESTRICTIONS_SET` value 3 and `all` semantics (LOCATION_REGISTER U-13) | Read the users of `RESTRICTIONS_GET_*` in `z_parameter.c` |
| U-12 | Scene seq 0 with a positional BGM actor: what plays on entry and on exit | Read `Scene_CommandSoundSettings` (`z_scene.c:449`) and `Audio_PlayObjSoundBgm` |
| U-13 | Visual survey | Capture `cam.lottery_shop.entry`, `.reverse_door`, `.fixture` under `prof.d1.noon.normal`, plus `.fixture` under `prof.d1.night.normal` with a ticket held (both booth yaw states) |

**Not inspected:** room DL contents beyond texture loads, combiner, render and geometry modes; light-setting values; En_Test4 behavior beyond init, events and the swap entry; message data; `object_kujiya`, `object_hariko`, `object_wdor01`, `gameplay_keep` contents (not extracted); `Actor_OfferTalk` and `Actor_OfferGetItem` internals; 3DS material.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN sections 6, 11 and 12, the three registers' lottery rows, the North and West dossiers (template; West 2.4.1), the PIPE-01 schema README, `region.schema.json`, `laundry_pool.json`, `south_clock_town.json` and the `ids.json` portal and camera entries; read scene.c, room.c, the scene XML, object XMLs (kujiya, hariko, wdor01, tokeidai), `scene_table.h`, `z_scene_table.c`, `z_actor.c`, `z_player.c` (door and exit code), `z_en_door.c` and its schedule, and the overlays En_Kujiya, En_Time_Tag, En_Test4, Obj_Tokeidai, Obj_Hariko and Obj_Sound; `z_sram_NES.c`, `z_inventory.c`, `z_message.c`, `z_message_nes.c`, `z_parameter.c`, `z64save.h`, `z64item.h`. Scratch scripts (stdlib, read-only) in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\lottery_shop\`: `analyze.py` (collision counts, surface decode, door probe, DL and texture counts, SHA-256 of 8 inputs), `actors.py` (6-entry decode), `clusters.py` (ornament clusters, shell walls), `ids_check.py` (portals departing `loc.lottery_shop`), `crcount.py` (CR counts), `check_region.py` (section 10 block against a subset of the schema and the registry). Greps for every flag, field and scene id named in section 5.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes; `region_from_dossier.py` and `validate_city.py` (they write outside this packet's path; the integrator lifts and validates section 10).

**Next dependency:** integrator lifts section 10 and runs `validate_city.py`; REF-02 owner resolves U-01; extraction of the section 9 banks (`gameplay_keep` shared with the other venue packets); captures for U-13; the ROOM-01 pattern (INT-01 pilot) before any Lottery assembly.
