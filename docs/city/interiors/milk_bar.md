# Milk Bar (`loc.milk_bar`): source inventory dossier

Task **REF-09 / INT-09** (FULL_CITY_PLAN.md section 12 row REF-09, section 6 row INT-09; the dispatch calls the INT table "section 5", it is section 6). Base commit `31b20dc` on `main` (HEAD is `7a7a08c`, PIPE-01; IDs below were checked against its `shared/city/ids.json`). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_MILK_BAR` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save, Human form, no mask worn; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_MILK_BAR/Z2_MILK_BAR.c`, `room.c` = `.../Z2_MILK_BAR_room_00.c`, `player.c` = `src/overlays/actors/ovl_player_actor/z_player.c`; an overlay `z_<name>.c` lives in `src/overlays/actors/ovl_<Name>/`. Line numbers count `\n` only (scene.c holds 10 lone `\r` bytes and room.c 110, some inside actor entries; a parser that treats `\r` as a line break drifts by up to 4 lines). N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro/function; **OVL** read from the actor overlay source (including its `.schl` schedule script); **ANN** decomp-author comment or symbol name; **INF** inferred (reason given); **NI** not inspected.

Cross-references, not redone here: `docs/city/LOCATION_REGISTER.md` (REF-01), `docs/city/PORTAL_REGISTER.md` (REF-02), `docs/city/SOURCE_REGISTER.md` (REF-03), `docs/city/districts/east_clock_town.md` section 2.4 (street door TA6). Every count those files state for the Milk Bar matches the C; one wording disagreement (transition-actor params differ by header) is U-17.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_MILK_BAR` / `SCENE_MILK_BAR` | HDR `include/tables/scene_table.h:49` |
| Scene-table index | `0x15` | same line |
| Entrance-table scene index | `0x12` (`ENTR_SCENE_MILK_BAR`) | HDR `src/code/z_scene_table.c:2591`; `include/z64scene.h:611` |
| Title / draw / restrictions / cycle flags | `0x10C` / `SCENE_DRAW_CFG_MAT_ANIM` / `RESTRICTIONS_INDOORS` / `PERSISTENT_CYCLE_FLAGS_NONE` | HDR scene_table.h:49 |
| XML files | scene `Z2_MILK_BAR` segment 2; room `Z2_MILK_BAR_room_00` segment 3 with DLs `0x93C0`, `0xAF48` | XML `assets/xml/scenes/Z2_MILK_BAR/Z2_MILK_BAR.xml` |
| Rooms | 1 (`SCENE_CMD_ROOM_LIST(1, ...)` in all five scene headers) | HDR scene.c:16, :1142, :1354, :1503, :1764 |
| Scene headers | default + 4 alternates (`Set_002D50`, `Set_003460`, `Set_0038B0`, `Set_0041A0`) | HDR scene.c:34-39 |
| Room headers | default + 4 alternates (`Set_000230`, `Set_0002C0`, `Set_0003F0`, `Set_0004E0`) | HDR room.c:25-30 |
| Object heap | 1580 KB for this scene (default 1380, four exteriors 1530) | C `src/code/z_scene.c:45-53` |
| Region | `SCENE_CMD_SET_REGION_VISITED`; scene is in the REGION_CLOCK_TOWN list | HDR scene.c:18; C `src/code/z_inventory.c:432` |
| Sound | spec 5 (alt3/alt4: 12), ambience 19 = `AMBIENCE_ID_13`, seq 0 = `NA_BGM_GENERAL_SFX` (no scene BGM) | HDR scene.c:15; macro `include/z64scene.h:845`; `include/sequence.h:94`; `include/tables/sequence_table.h:20` |
| Skybox | file 0, skybox 0, weather 0, lightMode true; room `SKYBOX_DISABLES(true, true)` | HDR scene.c:23; room.c:17 |
| Env light settings | 4 per header; default entry 0, alt2 entry 1 and alt4 entry 0 differ (values not transcribed) | HDR scene.c:64, :1175, :1387, :1537, :1797 |
| Animated materials | `{ 0, 6, NULL }` (segment 0, skipped by the draw loop per REF-05) | HDR scene.c:383-385 |
| Minimap | 1 entry `0xFFFF` (no map) in every header | HDR scene.c:341-347 |
| Collision | `Z2_MILK_BARCollisionHeader_002D1C` in all five headers | HDR scene.c:19, :1145, :1357, :1506, :1767 |
| Default room commands | echo 10; `ROOM_BEHAVIOR(0x00, ...)` = `ROOM_TYPE_NORMAL`; `TIME_SETTINGS(255, 255, 3)`; cullable shape, 8 entries | HDR room.c:15-19; enum `include/z64scene.h:315-320` |
| Paths | 6, default header only | HDR scene.c:21, :125-132 |
| Default cutscene data | 2 scripts, 17 actor cutscenes, 6 actor-cutscene cameras | HDR scene.c:26-28 |

### 1.1 Header inventory (all five headers)

| Item | Default (layer 0) | Alt1 (layer 1) | Alt2 (layer 2) | Alt3 (layer 3) | Alt4 (layer 4) |
| --- | --- | --- | --- | --- | --- |
| Scene set (line) | `Commands` (13) | `Set_002D50` (1140) | `Set_003460` (1352) | `Set_0038B0` (1501) | `Set_0041A0` (1762) |
| Sound spec | 5 | 5 | 5 | 12 | 12 |
| Spawn | (-200, 152, 360) `0x0EFF` | (-200, 160, 360) `0x0EFF` | same as alt1 | (-200, 160, 360) `0x0E00` | same as alt1 |
| TA0 `EN_DOOR` params | `0x028A` | `0x007F` | `0x007F` | `0x007F` | `0x007F` |
| Exit list | 2 words (scene.c:59-62) | none | none | none | none |
| Paths | 6 | none | none | none | none |
| Script list (nextEntrance, flag) | 2 x (`0xFFFF`, NONE) (316-319) | 1 (`0xFFFF`, ALWAYS) (1336-1338) | 1 (`0xFFFF`, ALWAYS) (1485-1487) | 1 (`0x8A01`, ALWAYS) (1732-1734) | 1 (`0xFFFF`, ALWAYS) (1879-1881) |
| Actor cs / cs cams | 17 / 6 | 1 / 0 | 1 / 0 | 1 / 1 (`CAM_SET_START0`, 1748-1756) | 1 / 0 |
| Credits command (line) | none | `CS_CREDITS_MASK_GREAT_FAIRY` (1325) | `CS_CREDITS_MASK_BREMEN` (1469) | `CS_CREDITS_DESTINATION` (1691) | `CS_CREDITS_IKANA` (1866) |
| Script text ids | 0x2B32-0x2B3B | 0x4E31 | 0x4E38 | 0x4E23-0x4E25 | 0x4E39 |
| Room set (room.c line) | `Commands` (13) | `Set_000230` (129) | `Set_0002C0` (170) | `Set_0003F0` (239) | `Set_0004E0` (289) |
| Room echo / type / time | 10 / NORMAL / (255,255,3) | 7 / INDOORS / (255,255,0) | same as alt1 | same as alt1 | same as alt1 |
| Objects / actors (HDR) | 14 / 18 | 4 / 4 | 9 / 13 | 11 / 9 | 10 / 18 |
| Distinct content | bar NPCs, stage, milk jars, kickout tags, BGM sources | Rosa Sisters, soldier | Indigo-Go's, Gorman, 5 carpenters, 2 Zoras | Rosa Sisters, Indigo-Go's, Toto, Honey and Darling | soldiers, carpenter, Guru-Guru, 5 cuccos, 3 Zoras, Zubora, Gabora |

Counts match LOCATION_REGISTER (`r00 d 14/18, a1 4/4, a2 9/13, a3 11/9, a4 10/18`; 1 spawn and 1 transition actor per header; exits 2/none x4).

### 1.2 What selects each alternate: all four are end-credits layers (C)

Selection: `sceneLayer = (cutsceneIndex & 0xF) + 1` when `cutsceneIndex >= 0xFFF0` (`src/code/z_play.c:2210-2219`); `altHeaders[sceneLayer - 1]` (`src/code/z_scene.c:464-474`); a script entry whose `nextEntrance` has layer bits n sets `nextCutsceneIndex = n + 0xFFEF` (`src/code/z_demo.c:600-619`). `sMilkBarEntrance0` has five layer rows, all `SCENE_MILK_BAR` spawn 0 (`z_scene_table.c:354-357`).

| Layer | cutsceneIndex | Writer found (C) | Cutscene on spawn (`CS_SPAWN_FLAG_ALWAYS`) | Next |
| --- | --- | --- | --- | --- |
| 1 | 0xFFF0 | `CS_CREDITS_MASK_KAMARO` with Kamaro's Mask owned: `ENTRANCE(MILK_BAR, 0)` + 0xFFF0 (`z_demo.c:689-697`); also the East alt2 script `{..., 0x2401, ...}` after its `CS_CREDITS_DESTINATION` (`Z2_TOWN.c:2441`, `:2452`) | `CutsceneData_002E44` (scene.c:1186-1330): Rosa Sister cues 550/551 at (-14, 22, -402) and (-159, 22, -409) (stage) | Great Fairy's Mask owned: fountain + 0xFFF0, else `ENTRANCE(CUTSCENE, 1)` (`z_demo.c:699-707`) |
| 2 | 0xFFF1 | `CS_CREDITS_MASK_CIRCUS_LEADER` with Circus Leader's Mask owned (`z_demo.c:733-741`) | `CutsceneData_003554` (scene.c:1398-1479): camera only, `CS_LIGHT_SETTING` 0 then 1 (:1472-1474) | Bremen Mask owned: Milk Bar + 0xFFF3 (layer 4), else `ENTRANCE(CUTSCENE, 6)` (`z_demo.c:743-751`) |
| 3 | 0xFFF2 | **none found** in `src/` or the 22 extracted city scenes (grep `0xFFF2`, `0x2403`; U-01) | `CutsceneData_0039AC` (scene.c:1548-1726): cues Japas 500, Evan 501, Lulu 504, Tatl 201; Link cues at (7, 22, -445) and `CS_MISC_PLAYER_FORM_HUMAN` at frame 200 (:1706-1714) | `CS_CREDITS_DESTINATION` uses the script's `0x8A01` = Goron Village (spring) entrance 0 layer 1 (`z_scene_table.c:2642`) |
| 4 | 0xFFF3 | `CS_CREDITS_MASK_BREMEN` with Bremen Mask owned (`z_demo.c:743-751`), i.e. from alt2 | `CutsceneData_004294` (scene.c:1808-1872): camera only | `CS_CREDITS_IKANA`: Ikana Canyon 0 + 0xFFF3 (`z_demo.c:753-757`) |

Credit transitions wait for a music marker (`func_801A3950(SEQ_PLAYER_BGM_MAIN, true) != 0xFF`, `z_demo.c:683`). None of the alternates is a gameplay band performance; the playable Ballad of the Wind Fish sound check runs in the default header (section 5.5). Actor names in the alternates are ANN (overlay file headers, e.g. `z_en_ending_hero5.c:4` "Carpenters watching moon disappearance and Indigo-Go's").

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:41-44, :1157-1160, :1369-1372, :1519-1522, :1779-1782; decode C)

Player params: start mode `(p & 0xF00) >> 8`, bgCam `p & 0xFF` (`include/z64player.h:17-18`). Yaw `(rotY >> 7) & 0x1FF`.

| Header | Position (Unity) | Raw rot X / Y / Z | Yaw | Params | Start mode / bgCam | Used by |
| --- | --- | --- | --- | --- | --- | --- |
| Default | (-200, 152, 360) (-2.00, 1.52, -3.60) | 0x0007 / 0x5A00 / 0x007F | 180 (faces -Z, into the bar) | 0x0EFF | 0xE `PLAYER_START_MODE_E` / none | entrance 0 layer 0 = `0x2400`, East exit [12] (`portal.east_clock_town.milk_bar`); 50 u inside door TA0 |
| Alt1, alt2, alt4 | (-200, 160, 360) | same | 180 | 0x0EFF | 0xE / none | credits layers 1, 2, 4 |
| Alt3 | (-200, 160, 360) | same | 180 | 0x0E00 | 0xE / bgCam 0 | credits layer 3 |

Every header's `EntranceEntry` list is `{0,0},{0,0}` (spawn 0, room 0; scene.c:54-57 and twins); the second row is not addressed by any entrance (INF padding). Entrance rows: layer 0 flags `0x4102`, layers 1-4 `0x8102` (`z_scene_table.c:355-356`; flag semantics per REF-02).

### 2.2 Exit list (HDR scene.c:59-62, default header only) and consumers

| Idx | Raw | Decode (entrance-table scene, entrance, layer) | Destination | Consumers in this scene |
| --- | --- | --- | --- | --- |
| 0 | `0xD2B0` | 0x69 `Z2_TOWN`, 11, 0 | East spawn 11 (1036, 100, -902) yaw 0 params `0x0E0B` (East dossier 2.1) | collision surface 5 (polys 126, 127) via the door probe; `EN_TIME_TAG` a09 and a10 (exit `p & 0x1F = 0`) |
| 1 | `0xD650` | 0x6B `Z2_BACKTOWN`, 5, 0 | North spawn 5 (Bombers arrival) | **none**: no surface has exit+1 = 2 and no default-header actor reads index 1 (C; answers PORTAL_REGISTER U4 for this scene) |

Alternates have no `SCENE_CMD_EXIT_LIST`, so `play->setupExitList` stays NULL (`z_play.c:1569-1574`). Exit readers in alternates (PORTAL_REGISTER U14, decoded here): `En_Zov 0xFE0F` index `(p & 0xFE00) >> 9 = 127`, read only after talk text 0x1039 (`z_en_zov.c:430-446`); `En_Zod 0xFE0F` index 127, read in `EnZod_Rehearse`, reached from a talk (`z_en_zod.c:450-478`); `En_Kgy 0xFE1F` index `p & 0x1F = 31`, read after text 0xC43 (`z_en_kgy.c:284-291`, `:540-548`). All three need a player talk exchange; the credits cutscene runs from spawn and the exit check also requires `csAction == NONE` (`player.c:6409-6410`), so INF none is reached (U-12). `En_Zos 0xFE0F` and `En_Kbt 0xFE1F` have no exit reader.

### 2.3 Transition actor and door decode (HDR scene.c:46-48 and alt twins; C)

`{ 0, 255, 0, 255, ACTOR_EN_DOOR, -200, 152, 410, 127, params }`: front room 0 / cam 255, back room 0 / cam 255; position (-200, 152, 410) (Unity (-2.00, 1.52, -4.10)); rotY 127 = yaw 0, csId 0x7F none (`src/code/z_actor.c:3613-3619`). Params decode `ENDOOR_GET_TYPE = (p >> 7) & 7`, type var `p & 0x7F` (`z_en_door.h:95-100`).

| Header | Params | Type / var | Art (OVL `z_en_door.c`) | Opening rule |
| --- | --- | --- | --- | --- |
| Default | `0x028A` | 5 `ENDOOR_TYPE_SCHEDULE` / 10 `ENDOOR_SCH_TYPE_MILK_BAR` | `sObjectInfo[17 + 10]` = `DOOR_OBJINFO_27` `{DOOR_DL_MILK_BAR, OBJECT_WDOR04}` (:222, :346-347); `gMilkBarDoorDL` on both sides (:317; XML `object_wdor04.xml:4`, offset 0x508) | `sDoorSch_MilkBar`, from the +Z side only (:537); section 5.1 |
| Alt1-alt4 | `0x007F` | 0 `ENDOOR_TYPE_WHOLE_DAY` / half-day index 7, text offset 15 | no `SCENE_MILK_BAR` row in `DOOR_OBJKIND_DEFAULT`, so `DOOR_OBJINFO_15` gameplay_keep default door (:189-206, :349-361); INF field keep not loaded (no `SPECIAL_FILES` command) | `D_801AED48[7]` = all nights, so open every day (`z_actor.c:2006-2016`; `z_en_door.c:511-536`) |

Door sides (C): door yaw 0, so local +Z = world +Z = the street side (exit floor z 420..520). A player inside (z < 410) gets `doorDirection = -1` (`z_en_door.c:496`), the schedule check is skipped (:537), and the probe is `-(doorDirection x 22) = +22` along the door yaw (`player.c:6756`, `:6779-6781`): point (-200, 162, 432); the floor raycast hits poly 127 (scratch `col.py`), exit+1 = 1, exit 0 = `0xD2B0` (`player.c:6783`, `:6405-6441`). Leaving the bar never consults the schedule.

### 2.4 Collision surfaces carrying exits (C from scene.c:402-406 and the polygon list)

| Surface | data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Notes |
| --- | --- | --- | --- | --- | --- |
| 5 | `0x00000101` / `0x0000500A` | 0 (`0xD2B0`) | 126, 127 (floors, normal +Y) | -230..-170 / 152 / 420..520 | wood, bgCam 1, echo 10, no conveyor; both polys flagged ignore-camera (vIA bit 0); centroid 60 u past the door plane. Shared by every header; alternates have no exit list (U-12) |

### 2.5 Door-to-venue cross-check with East Clock Town (REF-07 section 2.4; PORTAL_REGISTER rows 205, 231, 232)

| Check | East side | Milk Bar side | Verdict |
| --- | --- | --- | --- |
| Street door | TA6 (1038, 100, -931) yaw 0, `0x028A` SCHEDULE / 10 MILK_BAR, `gMilkBarDoorDL` | TA0 `0x028A`, same schedule type and art | agree |
| Inbound | probe (1038, 110, -953) -> poly 472, exit+1 13 -> slot 12 `0x2400` | `0x2400` = entrance 0 layer 0 -> spawn 0, room 0, facing into the bar | agree |
| Return | spawn 11 (1036, 100, -902), 29 u south of TA6, yaw 0 | exit [0] `0xD2B0` = East entrance 11 | agree |
| Hours | East "open (10:00-21:00)" | free 10:00 to before 21:00, **members 22:00 to before 05:00 with Romani's Mask worn**, otherwise closed (5.1) | East states only the free window; PORTAL_REGISTER "open hours UNKNOWN" is resolved here |
| Kickout | PORTAL row 232: 05:00 GENERIC (D0-D2), 21:00 MILK_BAR (all) | a10 05:00 text 0x1889, a09 21:00 text 0x188C (5.2) | agree (the register's labels are its own) |

East's DUNGEON3 camera on the Milk Bar roof deck (East dossier section 6) is exterior data in `Z2_TOWN`; no part of it is in this scene.

### 2.6 Portal IDs (registered in `docs/city/PORTAL_REGISTER.md` and `shared/city/ids.json`)

| Portal ID | Mode | Evidence | Arrival |
| --- | --- | --- | --- |
| `portal.milk_bar.east_clock_town` | door | TA0 probe -> polys 126/127, exit 0 `0xD2B0` | East spawn 11 |
| `portal.milk_bar.east_clock_town.kickout` | special | a09 (21:00) and a10 (05:00) `EN_TIME_TAG`, exit 0 | East spawn 11 |
| `portal.east_clock_town.milk_bar` (inbound, departs East) | door | East TA6 -> slot 12 `0x2400` | spawn 0 (-200, 152, 360) |

Single room: no `portal.milk_bar.room_NN.room_MM`. Exit [1] `0xD650` gets no portal (unreferenced).

## 3. Object banks (HDR room.c:32-47, :140-145, :181-191, :250-262, :300-311; IDs from `include/tables/object_table.h`)

A setup actor spawns only if its profile object is loaded (`src/code/z_actor.c:3516-3522`).

| # | Default bank | ID | Consumer (profile `objectId`, OVL) |
| --- | --- | --- | --- |
| 0 | `OBJECT_IN2` | 0x248 | En_Gm (a07) |
| 1 | `OBJECT_OS_ANIME` | 0x09D | none in this scene (only `ovl_En_Cne_01` references it; U-06) |
| 2 | `OBJECT_MILKBAR` | 0x212 | Dm_Char07 (a04; Toto's spotlight spawn) |
| 3 | `OBJECT_AL` | 0x00D | En_Al (a06) |
| 4 | `OBJECT_TAB` | 0x013 | En_Tab (a03, a08) |
| 5 | `OBJECT_ZM` | 0x23A | En_Toto (a02) |
| 6-9 | `OBJECT_LINK_GORON`, `_ZORA`, `_NUTS`, `_CHILD` | 0x14C, 0x14D, 0x154, 0x011 | player form objects (`src/code/object_table.c:3-9`); INF loaded for the sound-check Link clones (`z_en_toto.c:696-701`) |
| 10 | `OBJECT_MBAR_OBJ` | 0x202 | only `Bg_Mbar_Chair`, which no actor list or code spawns (grep; U-05) |
| 11 | `OBJECT_WDOR04` | 0x27B | En_Door TA0 art |
| 12 | `OBJECT_MM` | 0x107 | En_Pm (a12) |
| 13 | `OBJECT_OBJ_MILK_BIN` | 0x261 | Obj_Milk_Bin (a13-a15) |
| missing | `OBJECT_BG` 0x129, `OBJECT_SHN` 0x1AC | | En_Takaraya (a00) and En_Syateki_Man (a01) can never spawn here (C; U-04) |

| Alt | Banks (in order) |
| --- | --- |
| 1 | MILKBAR, MBAR_OBJ, RZ 0x24B, SDN 0x1B6 |
| 2 | MILKBAR, MBAR_OBJ, ZOV 0x22B, ZOS 0x220, ZOB 0x211, ZOD 0x216, IN2, DAIKU 0x0F1, ZO 0x0D0 |
| 3 | MILKBAR, MBAR_OBJ, ZOV, ZOS, ZOB, ZOD, ZM, KBT 0x1CF, KGY 0x1D6, MU 0x140, RZ (KBT and KGY have no actor in alt3) |
| 4 | MILKBAR, MBAR_OBJ, MU, FU 0x0FF, NIW 0x00F, DAIKU, ZO, KBT, KGY, SDN |

No alternate loads `OBJECT_WDOR04` (hence the keep-door fallback, 2.3). Implicit: `gameplay_keep` (door skeleton, En_Time_Tag, En_Test4, En_Gakufu, Obj_Sound).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): id `raw & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF` degrees, passed as a plain integer (wrapped above 180) when the id carries `0x8000` (Y), `0x4000` (X) or `0x2000` (Z); csId `rotY & 0x7F` (0x7F none, :3570-3574); half-day mask `((rotX & 7) << 7) | (rotZ & 0x7F)` (:3651-3654), 0 means all.
- `Actor_SpawnSetupActors` (2546-2575, mask at :2562) and `Actor_InitHalfDaysBit` (2491-2499): Day 1 12:00 -> bit `0x080`. Bits: D0d 0x200, D0n 0x100, D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001. No entry here carries id bit 0x800.
- Flagged ids here: `EN_TIME_TAG | 0xE000` (all three axes integers: rotX = kickout hour, rotY = minute), `EN_STOP_HEISHI | 0x2000`, `EN_ZOT | 0x2000`.
- Classes per `shared/city/schema/README.md`; credits performers (cutscene-cue driven) are labeled `posed-skeletal`. "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (18 entries, HDR room.c:49-104, decode C)

| # | Actor (ID) | Position | Rot deg X/Y/Z | Mask | CS | Params | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `EN_TAKARAYA` (0x1C1) | (71, -18, 75) | 0/90/0 | 0x040 D1n | none | 0xFFFF | npc-scheduled (never spawns: no OBJECT_BG) | no |
| 1 | `EN_SYATEKI_MAN` (0x11D) | (69, -6, 195) | 0/90/0 | 0x050 D1n, D2n | none | 0xFF0F | npc-scheduled (never spawns: no OBJECT_SHN) | no |
| 2 | `EN_TOTO` (0x234) | (-27, -8, -275) | 0/180/0 | 0x150 D0n, D1n, D2n | 11 | 0x3FFF | npc-scheduled | no |
| 3 | `EN_TAB` (0x263) Mr. Barten | (-28, -8, -195) | 0/0/0 | 0x2AA all daytimes | none | 0x0000 | npc-scheduled | yes |
| 4 | `DM_CHAR07` (0x199) | (0, 0, 0) | 0/0/0 | all | none | 0x0000 | static-mesh (stage + dynapoly) | yes |
| 5 | `EN_GAKUFU` (0x1D6) | (3, 94, -556) | 0/0/0 | all | none | 0xFE01 | billboard-effect (hidden until timer) | yes |
| 6 | `EN_AL` (0x262) Madame Aroma | (61, 0, 35) | 0/103/0 | 0x004 D3n | 16 | 0x0000 | npc-scheduled | no |
| 7 | `EN_GM` (0x0A4) Gorman | (-200, 152, 479) | 0/180/0 | 0x0F0 D1d, D1n, D2d, D2n | none | 0x0000 | npc-scheduled | yes |
| 8 | `EN_TAB` (0x263) | (161, 0, -10) | 0/270/0 | 0x15D D0n, D1n, D2n, D3d, D3n, D4n | none | 0x0000 | npc-scheduled | no |
| 9 | `EN_TIME_TAG \| 0xE000` (0x1AB) | (-67, -8, -40) | 21/0/0 (raw) | all | none | 0x0120 | controller-invisible (21:00 kickout) | yes |
| 10 | `EN_TIME_TAG \| 0xE000` (0x1AB) | (-110, -8, 3) | 5/0/0 (raw) | 0x3F0 D0d..D2n | none | 0x00C0 | controller-invisible (05:00 kickout) | yes |
| 11 | `EN_TEST4` (0x15A) | (-202, 152, 503) | 0/0/0 | all | 9 | 0xFD00 | controller-invisible | yes |
| 12 | `EN_PM` (0x1D5) Postman | (-206, 152, 465) | 0/180/0 | all | none | 0x0001 | npc-scheduled | yes |
| 13 | `OBJ_MILK_BIN` (0x28B) | (100, 32, 282) | 0/338/0 | all | none | 0x0002 | static-mesh (state-gated) | yes |
| 14 | `OBJ_MILK_BIN` (0x28B) | (75, 32, 282) | 0/13/0 | all | none | 0x0002 | static-mesh (state-gated) | yes |
| 15 | `OBJ_MILK_BIN` (0x28B) | (28, 32, 282) | 0/343/0 | all | none | 0x0002 | static-mesh (state-gated) | yes |
| 16 | `OBJ_SOUND` (0x0F0) | (204, 299, -6) | 0/0/0 | 0x2AA all daytimes | none | 0x013C | controller-invisible (BGM source) | yes |
| 17 | `OBJ_SOUND` (0x0F0) | (0, 326, -526) | 0/0/0 | 0x155 all nights | none | 0x0156 | controller-invisible (BGM source) | no |

Plus TA0 `EN_DOOR` (0x005) from the scene list (2.3), class door-transition.

### 4.3 Alternate 1 (4 entries, HDR room.c:147-164; all masks all, CS none)

| # | Actor | Position | Yaw | Params | Note |
| --- | --- | --- | --- | --- | --- |
| 0 | `EN_RZ` (0x27B) | (-14, 22, -402) | 0 | 0x7E02 | type `p & 0xF = 2`, path `(p & 0x7E00) >> 9 = 63` (`z_en_rz.h:7-9`); cue 550 |
| 1 | `EN_RZ` | (-159, 22, -409) | 0 | 0xFE02 | sister bit `0x8000` set; cue 551 |
| 2 | `DM_CHAR07` | (0, 0, 0) | 0 | 0x0001 | `DMCHAR07_CREDITS_STAGE` |
| 3 | `EN_STOP_HEISHI \| 0x2000` (0x1C7) | (54, -8, -283) | 214 | 0x007F | switch `p & 0x3F = 0x3F` |

### 4.4 Alternate 2 (13 entries, HDR room.c:193-233)

| # | Actor | Position | Yaw | Params |
| --- | --- | --- | --- | --- |
| 0 | `DM_CHAR07` | (0, 0, 0) | 0 | 0x0001 |
| 1 | `EN_ZOV` (0x252) Lulu | (-14, 22, -402) | 0 | 0xFE0F |
| 2 | `EN_ZOB` (0x231) Japas | (-164, 22, -409) | 0 | 0x000F |
| 3 | `EN_ZOD` (0x238) Tijo | (122, 22, -451) | 0 | 0xFE0F |
| 4 | `EN_ZOS` (0x241) Evan | (-91, 22, -523) | 0 | 0xFE0F |
| 5 | `EN_GM` (0x0A4) Gorman | (154, 0, 119) | 232 | 0x00FF |
| 6-10 | `EN_ENDING_HERO5` (0x2AB) x5 | (50, -8, -236), (-40, -8, -194), (42, 0, -78), (76, 0, -8), (-6, -8, -303) | 197, 179, 185, 94, 180 | 0x0000-0x0004 |
| 11-12 | `EN_ZOT \| 0x2000` (0x228) x2 | (-91, -8, -246), (-131, -8, -259) | 168, 143 | 0xFC13 |

### 4.5 Alternate 3 (9 entries, HDR room.c:264-283)

| # | Actor | Position | Yaw | Params |
| --- | --- | --- | --- | --- |
| 0-1 | `EN_RZ` x2 | (197, -8, -287), (120, -8, -285) | 197, 197 | 0x7E02, 0xFE02 |
| 2 | `DM_CHAR07` | (0, 0, 0) | 0 | 0x0001 |
| 3-6 | `EN_ZOV`, `EN_ZOB`, `EN_ZOD`, `EN_ZOS` | same four stage positions as alt2 | 0 | 0xFE0F, 0x000F, 0xFE0F, 0xFE0F |
| 7 | `EN_TOTO` | (43, 0, -46) | 180 | 0x3FFF (CS none) |
| 8 | `EN_TG` (0x0E9) Honey and Darling | (-113, -8, -90) | 0 | 0x0000 |

### 4.6 Alternate 4 (18 entries, HDR room.c:313-374)

| # | Actor | Position | Yaw | Params |
| --- | --- | --- | --- | --- |
| 0 | `DM_CHAR07` | (0, 0, 0) | 0 | 0x0001 |
| 1 | `EN_STOP_HEISHI \| 0x2000` | (-40, 22, -58) | 341 | 0x007F |
| 2 | `EN_TG` | (-119, -8, -38) | 0 | 0x0000 |
| 3-4 | `EN_GURUGURU` (0x248) x2 | (-67, -8, 121), (-68, -8, 122) | 180 | 0x0000, 0x0001 |
| 5-9 | `EN_NIW` (0x011) x5 | (-78, -8, 90), (-57, -8, 100), (-88, -8, 129), (-55, -8, 129), (-88, -8, 104) | 0 | 0xFFFF |
| 10-12 | `EN_ZOT \| 0x2000` x3 | (-126, -8, 19), (-49, -8, -8), (-97, -8, 19) | 23, 351, 11 | 0xFC13 |
| 13 | `EN_ENDING_HERO5` | (-3, -8, -52) | 334 | 0x0000 |
| 14-15 | `EN_ENDING_HERO4` (0x2AA) x2 | (-111, -8, -95), (-26, -8, 24) | 8, 349 | 0x0000 |
| 16 | `EN_KBT` (0x1F6) Zubora | (107, 32, -100) | 264 | 0xFE1F |
| 17 | `EN_KGY` (0x1FF) Gabora | (9, -8, -112) | 341 | 0xFE1F |

### 4.7 Overlay findings per actor type (OVL)

| Actor | Init / draw / gating relevant to presence and pose |
| --- | --- |
| En_Tab (Barten) | flex `gBartenSkel`; draws only when `scheduleResult != 0` (`z_en_tab.c:757-779`); schedule and positions in 5.3 |
| En_Gm (Gorman) | kills a duplicate En_Gm, `actor.room = -1`, scale 0.01 (`z_en_gm.c:1796-1818`); draws only with `scheduleResult != 0` and an OBJECT_IN2 slot (`:1960-1980`); layer-2 credits force result 255 (`:884-892`) |
| En_Toto | killed in the Milk Bar from 06:00 to before 21:30 (`z_en_toto.c:279-283`); flex `gTotoSkel`; section 5.5 |
| En_Al (Madame Aroma) | schedule `D_80BDFC70`: Day 3 18:00-06:00 result 2 (`ovl_En_Al/scheduleScripts.schl:2-6`); mask D3n only |
| En_Pm (Postman) | script index from `WEEKEVENTREG_90_01/89_40/89_08/86_01` (`z_en_pm.c:386-404`); only scripts 1 and 2 list the Milk Bar (`ovl_En_Pm/scheduleScripts.schl:421-430`, `:445-452`); path index `p & 0xFF = 1` (`z_en_pm.h:12`); fresh save: no Milk Bar window |
| Dm_Char07 | type `p & 0xFF`, spotlight flags `(p >> 8) & 0x1F` (`z_dm_char07.h:10-23`); STAGE: scale 0.1, dynapoly `object_milkbar_Colheader_006688`, DLs opa 0x2CD0 + xlu 0x2BA0; CREDITS_STAGE: AnimatedMat 0x105F8 + DLs 0x7918/0x76A0 and all spotlight pairs (`z_dm_char07.c:35-50`, `:70-160`) |
| En_Gakufu | type 1 `GAKUFU_MILK_BAR`: draw NULL, waits; drawn while `home.rot.x` counts down (`z_en_gakufu.c:138-142`, `:165-175`) |
| Obj_Milk_Bin | type 2: draw and collider off unless `WEEKEVENTREG_ESCORTED_CREMIA` (`z_obj_milk_bin.c:59-61`, `:81-87`); DL `gMilkBinMilkJarDL` 0x4B0 (XML); yOffset 1100; cylinder r12 h30 |
| Obj_Sound | type `(p >> 8) & 0xFF = 1` BGM; id `p & 0x7F` (`z_obj_sound.h`); `Audio_PlayObjSoundBgm(projectedPos, id)` (`z_obj_sound.c:57-60`): a16 seq 0x3C `NA_BGM_MILK_BAR`, a17 seq 0x56 `NA_BGM_MILK_BAR_DUPLICATE` (`sequence_table.h:140`, `:192`) |
| En_Time_Tag | section 5.2 |
| En_Test4 | three-day controller; stars `(p >> 10) x 100 = 6300`, bell index `(p >> 5) & 0xF = 8` (`z_en_test4.h:8-9`); csId 9 |
| En_Takaraya / En_Syateki_Man | profile objects OBJECT_BG / OBJECT_SHN absent: `Actor_SpawnAsChildAndCutscene` returns NULL (`z_actor.c:3516-3522`) |
| Credits actors | Zov/Zob/Zos read cues 504/500/501 (`z_en_zov.c:347`, `z_en_zob.c:303`, `z_en_zos.c:282`); Zov and Zod type 0xF kill themselves if `WEEKEVENTREG_CLEARED_GREAT_BAY_TEMPLE` (Zov also `53_20`) (`z_en_zov.c:134-139`, `z_en_zod.c:138-141`; U-11); Rosa Sisters cue 550/551 (`z_en_rz.c:158-160`) |

### 4.8 Counts

| | Default | Alt1 | Alt2 | Alt3 | Alt4 |
| --- | --- | --- | --- | --- | --- |
| Entries | 18 | 4 | 13 | 9 | 18 |
| D1d candidates by mask | 12 (excluded: 0, 1, 2, 6, 8, 17) | 4 | 13 | 9 | 18 |
| static-mesh | 4 (a04, a13-a15) | 1 | 1 | 1 | 1 |
| npc-scheduled | 8 | 0 | 0 | 0 | 0 |
| posed-skeletal (cutscene) | 0 | 3 | 12 | 8 | 17 |
| billboard-effect | 1 | 0 | 0 | 0 | 0 |
| controller-invisible | 5 | 0 | 0 | 0 | 0 |
| door-transition (scene list) | 1 | 1 | 1 | 1 | 1 |

## 5. Venue mechanics index (INT-09 scope: membership/time, services, performance state)

Index and cite only. No music, mixing or performance behavior is claimed beyond the calls cited.

### 5.1 Opening hours and the membership gate (OVL `ovl_En_Door/scheduleScripts.schl:192-210`)

| Script time (same every day) | Result | Door text (`result + 0x1800`, `z_en_door.c:541`) | Effect |
| --- | --- | --- | --- |
| 10:00 to before 21:00 | `return_none` | none | opens normally |
| 22:00 to before 05:00, Romani's Mask **worn** | `return_s(0x21)` | 0x1821 | on accept sets `sDoorIsMilkBarMember` (`z_en_door.c:457-461`), which bypasses the 20/20/50 u proximity box (:486) and makes the door `PLAYER_DOORTYPE_PROXIMITY` (:543-548) |
| 22:00 to before 05:00, mask not worn | `return_s(0x22)` | 0x1822 ("Show you are a member", ANN :201) | talking door, no entry |
| 21:00-21:59 and 05:00-09:59 | `return_s(0x12)` | 0x1812 | closed message |

Mechanics: `SCHEDULE_CHECK_MISC_MASK_ROMANI` tests `Player_GetMask(play) == PLAYER_MASK_ROMANI` (worn, not owned) (`src/code/z_schedule.c:106-111`); ranges are `start <= now <= end - 1` (`z_schedule.c:25-49`) on script time `time - 0x3FFC` (about a 6 h shift, so 22:00-05:00 does not wrap) (`include/z64scripts.h:7-17`). Checked only from the street side (`z_en_door.c:537`). `sDoorIsMilkBarMember` is a static assigned only at :460 (never cleared in the overlay; U-15).

### 5.2 Kickouts (OVL `z_en_time_tag.c`, `.h:10-29`)

| Tag | Params decode | Fires | Text | Mask |
| --- | --- | --- | --- | --- |
| a09 | type `(p & 0xE000) >> 13 = 0` KICKOUT_DOOR, text `(p & 0x1FE0) >> 5 = 9`, exit `p & 0x1F = 0` | at 21:00 (`hour == rotX`, `minute == rotY`, not in a cutscene) | 0x1883 + 9 = 0x188C | all half-days |
| a10 | type 0, text 6, exit 0 | at 05:00 | 0x1883 + 6 = 0x1889 | D0d..D2n (no 05:00 kickout on the Day 3 night) |

`EnTimeTag_KickOut_WaitForTime` / `_WaitForTrigger` / `_Transition` (`z_en_time_tag.c:285-338`): halts the player, shows the text, then `nextEntrance = setupExitList[0]` (`0xD2B0`) with door SFX; deferred while `WEEKEVENTREG_KICKOUT_WAIT` is set (`KICKOUT_TIME_PASSED` records the missed minute). The Room Key exemption is Inn-only (:319-322).

### 5.3 Mr. Barten: schedule, positions, membership talk, milk and Chateau Romani (OVL `ovl_En_Tab`)

| Day | Script time window | Result | Pose (OVL) |
| --- | --- | --- | --- |
| 1, 2 | 09:50 to 21:05 | 1 | (-28, -8, -195) yaw 0, `gBartenIdleAnim`; requires an En_Gm actor to exist (`z_en_tab.c:506-525`) |
| 1, 2 | 21:55 to 05:05 | 2 | (161, 0, -10) yaw 0xC000 (270), `gBartenIdleBarCounterAnim` (`z_en_tab.c:527-541`) |
| 3 | 09:50 to 18:01 / 18:00 to 06:00 | 1 / 2 | same two poses; other Day-3 times `return_l(0)` (not drawn) |

Schedule `D_80BE18D0` (`ovl_En_Tab/scheduleScripts.schl:1-25`). Message selection `EnTab_GetMsgScript` (`z_en_tab.c:472-504`):

| Condition | Script (line) | Flags / texts |
| --- | --- | --- |
| Deku form (any result) | `D_80BE1984` (:88) | `WEEKEVENTREG_80_01`; 0x2B3F/0x2B40 |
| Result 1, Romani's Mask worn | `D_80BE1940` (:49) | `WEEKEVENTREG_63_04`; 0x2B3D/0x2B3E then 0x2AFA..0x2AFE |
| Result 1, Kafei's Mask worn | `D_80BE1970` (:76) | `WEEKEVENTREG_58_01`; 0x2AFF/0x2B00 |
| Result 1, otherwise | `D_80BE1914` (:24) | `WEEKEVENTREG_58_02`; 0x2AF9..0x2AFE, branch on `WEEKEVENTREG_ESCORTED_CREMIA` |
| Result 2, Romani's Mask not worn | `D_80BE1998` (:100) | 0x2B09 |
| Result 2, worn, Days 1-2 / Day 3 | `D_80BE19A0` (:106) / `D_80BE1A0C` (:160) | sale menu below |

Sale (both menus, `z_en_tab.c:106-158` and twin): empty-bottle callback `Inventory_HasEmptyBottle` (`:453-455`) via `CHECK_CALLBACK_CONTINUE` (:111; else 0x2B0E); menu 0x2B0B with three choices (:116-118). Milk: `CHECK_RUPEES(20)` (:119) -> `CHANGE_RUPEES(-20)`, `OFFER_ITEM(GI_MILK)`, `SET_COLLECTIBLE(0x0092)` (:143-145). Chateau Romani: `CHECK_RUPEES(200)` (:121) -> `CHANGE_RUPEES(-200)`, `OFFER_ITEM(GI_CHATEAU)`, `SET_COLLECTIBLE(0x0091)` (:135-137). Short funds -> error SFX + 0x2B0C; decline -> 0x2B0D; first-prompt "no" -> 0x2B0F (Day 3: 0x2B10) or 0x2B11 by `ESCORTED_CREMIA` (:150-155). `CHECK_RUPEES` branches when rupees >= amount (`src/code/z_msgevent.c:197-206`); `SET_COLLECTIBLE` sets a scene collectible flag (`z_msgevent.c:435-444`). Script order deducts rupees before the item offer (C; atomicity is a future engineering question). Derived (C): on Day 3 result 2 starts at 18:00 while the door is still free until 21:00 (U-16).

### 5.4 Gorman (OVL `ovl_En_Gm`)

| Day | Milk Bar window (`scheduleScripts.schl`) | Result | Behaviour (OVL) |
| --- | --- | --- | --- |
| 1 | 12:55-13:00 / 13:00-13:25 (:56-59) | 20 / 30 | walks from path index `p & 0xFF = 0`: `D_80951A0C[20] = 0` -> path 0 (door, 2 points), `[30] = 1` -> path 2 (8 points down the stairs to (40, 0, -122)) (`z_en_gm.c:23-25`; `src/code/z_sub_s.c:485-499`) |
| 1 | 13:25-21:05 (:60-61) | 3 | (64, 0, -122) beside Barten; needs En_Tab (`z_en_gm.c:1224-1243`) |
| 1 | 21:55-05:05 (:62-63) | 5 | path 2 end, or (64, 0, -122) on a fresh arrival (`z_en_gm.c:1280-1320`) |
| 2 | only if `WEEKEVENTREG_HELPED_GORMAN_DAY_1` is clear: 21:30-21:45 / 21:45-22:10 / 22:10-06:00 (:71, :105-113) | 20 / 30 / 5 | as Day 1; when set he stays in the Inn (results 7, 8) |
| 3 | none | | |

Talk (`EnGm_GetMsgScript`, `z_en_gm.c:808-850`): result 3 `D_80951B98` (:169-208) is a two-way scene with Barten (`FOCUS_TO_CHILD`), texts 0x2B01-0x2B07, sets `WEEKEVENTREG_75_01`, notebook `MET_GORMAN`; repeat 0x2B08. Result 5 `D_80951BE8` (:211-240): `HELPED_GORMAN_DAY_1` or `_DAY_2` -> 0x2B12; else first time 0x2B13 and `WEEKEVENTREG_75_02`, later 0x2B14-0x2B18. Barten turns toward Gorman during 0x2B01-0x2B07 unless `75_01` is set, Gorman is not drawn, or he is beyond 160 u (`z_en_tab.c:564-590`).

### 5.5 Toto and the Ballad of the Wind Fish sound check (default header, layer 0; OVL `z_en_toto.c`)

| Item | Source |
| --- | --- |
| Presence | a02 mask D0n/D1n/D2n; Init kill 06:00 to before 21:30 (:279-283); talk offered only outside 06:00..22:13 (+7 units), radius 50 (:350-357) |
| Dialog | `sDialogSpeakData` (:130-150): 0x2B23 first time, 0x2B24 ask (choice), 0x2B25 decline, 0x2B21 after completion; switch `p & 0x7F = 0x7F` set on talk (:342) |
| Walk to stage | override-input points (-186, -8, -192), (-223, -3, -252), (-156, 22, -386) (:202-206, :557-572), up the stage-left ramp in collision; spawns `DM_CHAR07` params 0xF02 = spotlights, flags 0xF (:571) |
| Spotlights (form: prompt / wrong / right text) | Goron (-106, 22, -490) 0x2B2F/30/31; Zora (114, 22, -452) 0x2B26/27/28; Deku (-153, 22, -402) 0x2B29/2A/2B; Human (-15, 22, -396) 0x2B2C/2D/2E (:195-200); leaving at `z > -270` cancels (:615-621); the correct light snaps the player, yaw 0 (:624-630) |
| Played flags | `WEEKEVENTREG_PLAYED_WIND_FISH_AS_HUMAN/_DEKU/_ZORA/_GORON` on `OCARINA_MODE_END` (:659-675); staff prompt `OCARINA_ACTION_PROMPT_WIND_FISH_*` (:207-212, :653-657) |
| Playback | earlier-played forms spawn as extra `ACTOR_PLAYER` at their spotlight (:683-701); `Audio_PlayFanfareWithPlayerIOCustomPort(NA_BGM_BALLAD_OF_THE_WIND_FISH, 4, formsPlayed ^ ALL)` (:711-713); `player.c:17226-17228` special-cases this sequence in the Milk Bar |
| Completion | all four played: `WEEKEVENTREG_HELPED_GORMAN_DAY_1` (Day 1) else `_DAY_2` (:737-742); otherwise the next additional cutscene, "Gorman booing" (ANN :744). `ENTOTO_HELPED_GORMAN` (:11-12) switches later talks to the replay path |
| Reward | while cue channel `CS_CMD_ACTOR_CUE_525` is active, cue id 4 -> `Actor_OfferGetItem(GI_MASK_CIRCUS_LEADER)` (:799-828); notebook `RECEIVED_CIRCUS_LEADERS_MASK`, `MET_TOTO`, `MET_GORMAN` (:432-437) |

Cutscene ids (HDR scene.c:321-338; `CutsceneEntry` layout `include/z64cutscene.h:626-637`): Toto csId 11 (room.c:56-58). Chain 11 (cam 2) -> 12 (cam 3) -> 13 (cam 4) -> 14 (script 0) -> 15 (script 1). Script 0 = `CutsceneData_00021C` (scene.c:134-251, 350 frames): Toto cues 3, 2, 1 and **4 at frames 319-349** (:231), Gorman cue 526 at (107, 0, 18), Link cue path (-206, 11, -292) -> (-16, -8, -130), texts 0x2B34-0x2B3B. Script 1 = `CutsceneData_0007AC` (scene.c:257-310, 122 frames): cues 3, 2, 1 only (no mask), Gorman moves to (-95, 50, -349) (:301), texts 0x2B32-0x2B33. INF: script 0 is the reward scene and script 1 the boo; which branch advances to 14 versus 15 is U-02. The Indigo-Go's are not in the default header; in gameplay the "band" is Link's four forms.

### 5.6 Madame Aroma and the Postman (Day 3 night touchpoints)

En_Al result 2 (Day 3 18:00-06:00): with `WEEKEVENTREG_85_80` set and `89_08` clear, `D_80BDFCBC` (`z_en_al.c:23-66`) plays the Postman exchange and sets `89_08`; Letter to Mama exchanges (`PLAYER_IA_LETTER_MAMA`) offer `GI_CHATEAU_BOTTLE` (:220, :305, :353) or `GI_RUPEE_PURPLE` once `WEEKEVENTREG_57_08` is set (:219, :227); selection `EnAl_GetMsgScript` (:744-790). Postman scripts 1/2 enter via scene path 1 (door) -> 3 -> 4 -> 5 (scene.c:125-132), timed relative to a stored start time (`z_en_pm.c:1828-1843`).

### 5.7 Cameras, lighting, acoustics and music

| Item | Value | Evidence |
| --- | --- | --- |
| bgCam 0 | `CAM_SET_FIXED0` ("milk bar stage", ANN `include/z64camera.h:162`): pos (-17, 244, -188), rot (9284, -32768, 0) = pitch 51.0 deg, yaw 180 (faces the stage), third triplet -1 | HDR scene.c:391-400; used only by surfaces 1 and 3 (stage deck and lip, 4 floors) |
| bgCam 1 | `CAM_SET_ROOM1` (`z64camera.h:228`), no data | every other surface |
| Actor cs cams | 6 entries, settings `CAM_SET_FIXED1` (31) / `FIXED2` (32): [0] and [1] (-151, 100, -404); [2] (-17, 244, -188); [3] (-22, 110, -129); [4] (50, 123, -81); [5] (-4, 75, -68) | HDR scene.c:349-377; used by csIds 9-10 (En_Test4), 11-13 (Toto), 16 (En_Al) |
| Actor cs 0-8 | priorities 100-800, global camera settings -2..-11, chained 0 -> 8 | HDR scene.c:322-330; no consumer found (U-03) |
| Lighting | indoor, skybox and sun/moon disabled, lightMode true (INF: not time-driven); 4 env settings | HDR scene.c:23-25; room.c:17 |
| Echo | room 10 (alternates 7); every surface type echo 10 | HDR room.c:15, :130; C surface data[1] bits 11-16 |
| Music | scene seq 0; positional BGM from a16 (day half-days) and a17 (night half-days) | section 4.7 |
| Room type | default `ROOM_TYPE_NORMAL`; alternates `ROOM_TYPE_INDOORS` ("reduces run speed, blocks attack/jump", ANN) | room.c:16, :131; `z64scene.h:317` |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

Open at 12:00: `sDoorSch_MilkBar` returns none between 10:00 and 21:00 (5.1). Fresh save, Human form, no mask.

| Source | Component | N64 position (Unity) | Yaw | Scale | DLs / bank | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room_00 | room geometry | origin | | 1 | opa master 0x93C0 (0x31F0, 0x4BA8, 0x8A80, 0x90E8), xlu master 0xAF48 (0x9AE0, 0x9AF0 empty, 0xA0E8, 0xA8D0, 0xAB58, 0xAE60) | already extracted (room.c) |
| a04 | stage set `0x0000` | (0, 0, 0) (0, 0, 0) | 0 | 0.1 | `object_milkbar` 0x2CD0 (opa), 0x2BA0 (xlu) | dynapoly `object_milkbar_Colheader_006688` |
| TA0 | street door `0x028A` | (-200, 152, 410) (-2.00, 1.52, -4.10) | 0 | ICHAIN NI | `object_wdor04` `gMilkBarDoorDL` 0x508 on `gameplay_keep` `gDoorSkel` limb 4 | closed pose |
| a13-a15 | milk jars `0x0002` (optional) | (100, 32, 282), (75, 32, 282), (28, 32, 282) | 338, 13, 343 | 0.01 | `object_obj_milk_bin` 0x4B0 | hidden on a fresh save (`ESCORTED_CREMIA` clear) |

**Excluded by half-day mask:** a00, a01, a02, a06, a08, a17. **Excluded by schedule/state:** a07 Gorman (Day 1 Milk Bar windows start 12:55), a12 Postman (script 0 has no Milk Bar entry). **Present but unimplemented (skeletal):** a03 Mr. Barten at (-28, -8, -195), yaw 0, `gBartenSkel` idle (result 1; En_Gm a07 exists, so the precondition holds). **No geometry:** a05 (hidden), a09, a10, a11, a16 (BGM only).

First open night profile: members only, from 22:00, with Romani's Mask worn. The registered `prof.d1.night.normal` (21:00) is inside the closed hour and is the exact a09 kickout minute, so SOURCE_REGISTER's "add `prof.d1.night.normal` to `fixture`" cannot show the bar open (U-09).

## 7. Collision summary (C from scene.c:391-1134; scratch `col.py`, output `col.out`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 304 / 408 / 9 |
| Water boxes | 0 (`0, NULL`, scene.c:1133) |
| Header bounds | min (-260, -16, -600), max (260, 500, 520) (scene.c:1127-1128) |
| Vertex extents | x -260..228, y -16..500, z -600..520 (Unity x -2.60..2.28, y -0.16..5.00, z -5.20..6.00) |
| Polygon kinds | 125 floors (normal Y > 0.5), 271 walls, 12 ceilings |
| Materials | stone (types 0, 1, 6): 20 floors, 250 walls, 12 ceilings; wood (2, 3, 5, 8): 85 floors, 21 walls; carpet (4, 7): 20 floors |
| Exit surface | type 5 only, polys 126/127 (2.4) |
| Wall type 1 | surface types 6, 7, 8 (type 6: 42 walls; type 8: 8 walls and 4 floors; type 7: 14 floors): `sWallFlags[1] = WALL_FLAG_0`, which blocks the ledge climb (`src/code/z_bgcheck.c:18-20`; `player.c:11835`) |
| Poly exclusion | ignore-camera: 12 walls + 4 ceilings (type 0), 2 floors (type 2), 2 exit floors; ignore-projectiles: 4 walls (type 0) + 2 walls (type 8); camera + projectiles: 4 walls (type 0), 6 walls and 4 sloped floors (type 8) |
| Not present | conveyors, hookshot surfaces, floor property, soft or horse flags, void floors |

Floor levels (C extents; furniture labels are INF from heights, positions and actor anchors, U-14):

| Level | Polys (surface) | Extent x / z | Reading |
| --- | --- | --- | --- |
| Y 152 landing | 108-109 (4 carpet), 124-125 (2), 126-127 (5 exit) | -260..-160 / 164..400; -230..-170 / 400..520 | entry landing behind the door (spawn 0 on poly 108) |
| Y 152 -> -8 | 106-107 (4), 35.5 deg | -260..-160 / -60..164 | stairs as a ramp (Gorman/Postman paths follow it); INF handrail tops 331-332 and 335-336 (8) at x -160..-140 |
| Y -8 main floor | 0-9 (0 stone) | -260..228 / -320..181 | hall floor; Barten day (-28, -8, -195), Toto (-27, -8, -275) |
| Y 0 | 12-15 (0) | -140..88 / -192..261 | raised central area, 38.7 deg bevels 28-33 |
| Y 0 behind counter | 16-19 (2 wood) | 121..228 / -150..400 | Barten night anchor (161, 0, -10) is on poly 16/17; low ceilings Y 160 / 220 above (74-77, 84-85) |
| Y 24 | 273-322, 14 floors (7 carpet) | seven 32-37 u squares: x 51..88 at z -142, -62, 18, 98, 178; z 224..261 at x -3..29 and -83..-51 | INF bar stools beside the counter arms |
| Y 32 | 247-252 (2 wood) | x 88..140 / z -202..313; x 88..228 / -202..-150; x -140..140 / 261..313 | INF L-shaped counter top; milk jars a13-a15 sit on 251/252 |
| Y 22 stage | 10-11 (1, bgCam 0) + lip 22-23 (3, 45 deg) | -260..228 / -600..-320 | stage deck (FIXED0 camera); left ramp 110-111 (4 carpet, 20.6 deg) to Y -8 at x -260..-160 |
| Y 47, 59, 192-311 | 337-339; 135-136; floors among 151-233 and 372-407 | small shelf at x 141..179; ledges and slopes along z 313..401 | INF shelving and cornice above the entrance wall (not a route) |
| Ceiling | 72-73 Y 500; entry ceiling 116-119 Y 252 | | |

The `Dm_Char07` stage dynapoly (`object_milkbar_Colheader_006688`) adds collision whose content is NI until `object_milkbar` is extracted (U-10).

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Scene textures (segment 2, PNGs) | 32: i8 15, i4 8, ia16 5, rgba16 4; no TLUT, no CI |
| Referenced by room DLs | all 32 (`Z2_MILK_BARTex_004550` .. `Z2_MILK_BARTex_00C3D0`) |
| Room file | 26 `Gfx` DLs, 19 `Vtx` arrays (19 `.vtx.inc`), 646 `gsSP2Triangles` + 9 `gsSP1Triangle` = 1301 source triangles |
| Room shape | cullable, 8 entries: 4 opa entry DLs, 6 xlu entry DLs (one empty, 0x9AF0) |
| Object-bank materials | `object_milkbar` (XML: 5 spotlight DL pairs, stage DLs, TextureAnimation 0x105F8), `object_wdor04`, `object_obj_milk_bin`: NI |

## 9. Required extraction list (commands NOT executed)

Binaries exist under `extracted/n64-us/baserom/` (`stat`): `object_milkbar` 71856, `object_wdor04` 5744, `object_obj_milk_bin` 6656, `gameplay_keep` 599520, `object_tab` 34496, `object_in2` 49232, `object_zm` 51344, `object_al` 56304, `object_mm` 55904; none is extracted (only the four tower banks are).

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_milkbar`, `object_wdor04`, `gameplay_keep`, `object_obj_milk_bin` | section 6 DLs, stage collision, door |
| Residents (later skeletal) | `object_tab`, `object_in2`, `object_zm`, `object_al`, `object_mm` | Barten, Gorman, Toto, Madame Aroma, Postman |
| Sound check | `object_link_goron`, `object_link_zora`, `object_link_nuts`, `object_link_child` | Link clones (5.5) |
| Credits layers (deferred) | `object_rz`, `object_zov`, `object_zos`, `object_zob`, `object_zod`, `object_daiku`, `object_zo`, `object_mu`, `object_fu`, `object_niw`, `object_kbt`, `object_kgy`, `object_sdn` | alt1-alt4 only |

```powershell
# run from local-reference/mm-decomp, same ZAPD binary and flags as South/North (tools/reference/README.md)
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_milkbar','object_wdor04','object_obj_milk_bin','gameplay_keep')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

`gameplay_keep` has not been extracted with this tool before (REF-05 note); check output size and warnings separately.

## 10. Proposed region record

Schema 1 (`shared/city/schema/region.schema.json`, PIPE-01 `7a7a08c`). Not lifted or validated by this packet.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.milk_bar",
  "displayLabel": "Milk Bar",
  "source": {
    "sceneSymbol": "Z2_MILK_BAR",
    "sceneEnum": "SCENE_MILK_BAR",
    "sceneTableIndex": "0x15",
    "entranceTableSceneIndex": "0x12",
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
      "fresh save: no week-event, switch or collectible flags (WEEKEVENTREG_ESCORTED_CREMIA clear)",
      "player form Human, no mask worn",
      "12:00 inside the free door window (sDoorSch_MilkBar returns none 10:00-21:00)"
    ]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_MILK_BAR", "bytes": 54224, "sha256": "8e65185a4415d599692e8a107d19a5b9affb150124c590df3a617394ec3dcd80" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_MILK_BAR_room_00", "bytes": 44928, "sha256": "ce13695d88bf45b355ed88cb9500705a8a45846198cffbef44b2230a91c2b402" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_MILK_BAR/Z2_MILK_BAR.c", "sha256": "24649989273d7eb561568ee0e559031bccfa90d242645a0f1e039f4f1cd27e45" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_MILK_BAR/Z2_MILK_BAR_room_00.c", "sha256": "52cd941e324d763552edc0c3de22352d564ff33a52eedbec676e67bf71f4b2e9" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_MILK_BAR/Z2_MILK_BAR.xml", "sha256": "0aeed311726256c92e53c85039d87846ee7dc315875dc2082ab0f42fffd0902a" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": [],
      "status": "not_shared",
      "evidence": "interior scene with its own origin; East street door TA6 at (1038, 100, -931) vs interior door TA0 at (-200, 152, 410) (section 2.5)"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-260, -16, -600], "max": [260, 500, 520] },
    "sourceCollisionVertices": { "min": [-260, -16, -600], "max": [228, 500, 520] },
    "unityCollisionVertices": { "min": [-2.6, -0.16, -5.2], "max": [2.28, 5.0, 6.0] }
  },
  "anchors": [
    { "id": "anchor.mb.spawn0_landing", "source": [-200, 152, 360], "unity": [-2.0, 1.52, -3.6], "status": "proposed" },
    { "id": "anchor.mb.door_ta0", "source": [-200, 152, 410], "unity": [-2.0, 1.52, -4.1], "status": "proposed" },
    { "id": "anchor.mb.barten_day_floor", "source": [-28, -8, -195], "unity": [-0.28, -0.08, 1.95], "status": "proposed", "note": "main floor Y -8" },
    { "id": "anchor.mb.barten_counter_back", "source": [161, 0, -10], "unity": [1.61, 0.0, 0.1], "status": "proposed", "note": "behind-counter floor Y 0" },
    { "id": "anchor.mb.stage_human_spotlight", "source": [-15, 22, -396], "unity": [-0.15, 0.22, 3.96], "status": "proposed", "note": "stage deck Y 22" }
  ],
  "rooms": [
    { "id": "loc.milk_bar.room_00", "sourceRoom": "Z2_MILK_BAR_room_00", "header": 0, "roomShape": "cullable, 8 entries", "displayLists": 26 }
  ],
  "components": [
    { "sourceKey": "Z2_MILK_BAR/h0/room_00/a04", "actorId": "0x199", "params": "0x0000", "class": "static-mesh", "dls": ["object_milkbar:0x2CD0", "object_milkbar:0x2BA0"], "status": "selected", "note": "Dm_Char07 stage, scale 0.1, dynapoly object_milkbar_Colheader_006688" },
    { "sourceKey": "Z2_MILK_BAR/h0/transition/ta0", "actorId": "0x005", "params": "0x028A", "class": "door-transition", "variant": "posed-skeletal", "dls": ["object_wdor04:0x508"], "status": "selected", "note": "closed pose on gameplay_keep gDoorSkel limb 4; alternates use 0x007F with gameplay_keep door DLs" },
    { "sourceKey": "Z2_MILK_BAR/h0/room_00/a13", "actorId": "0x28B", "params": "0x0002", "class": "static-mesh", "dls": ["object_obj_milk_bin:0x4B0"], "status": "optional", "note": "drawn only with WEEKEVENTREG_ESCORTED_CREMIA" },
    { "sourceKey": "Z2_MILK_BAR/h0/room_00/a14", "actorId": "0x28B", "params": "0x0002", "class": "static-mesh", "dls": ["object_obj_milk_bin:0x4B0"], "status": "optional", "note": "drawn only with WEEKEVENTREG_ESCORTED_CREMIA" },
    { "sourceKey": "Z2_MILK_BAR/h0/room_00/a15", "actorId": "0x28B", "params": "0x0002", "class": "static-mesh", "dls": ["object_obj_milk_bin:0x4B0"], "status": "optional", "note": "drawn only with WEEKEVENTREG_ESCORTED_CREMIA" }
  ],
  "collisionOwnership": {
    "room": "Z2_MILK_BARCollisionHeader_002D1C (408 polys)",
    "waterBoxes": 0,
    "actorDynapoly": ["a04 object_milkbar_Colheader_006688 (Dm_Char07 stage)"],
    "actorCylinders": null,
    "note": "actorCylinders NI except Obj_Milk_Bin r12 h30 (section 4.7)"
  },
  "portalIds": [
    "portal.milk_bar.east_clock_town",
    "portal.milk_bar.east_clock_town.kickout"
  ],
  "materialBindings": null,
  "cameraIds": ["cam.milk_bar.entry", "cam.milk_bar.reverse_door", "cam.milk_bar.fixture"],
  "routeIds": [],
  "omissions": [
    "a03 En_Tab Mr. Barten skeletal (gBartenSkel, schedule result 1 at (-28, -8, -195))",
    "alternate headers 1-4 (end-credits layers, sections 1.2 and 4.3-4.6)",
    "Link and Tatl"
  ],
  "excluded": {
    "halfDayMask": ["a00", "a01", "a02", "a06", "a08", "a17"],
    "stateOrSchedule": ["a07", "a12"],
    "noGeometry": ["a05", "a09", "a10", "a11", "a16"]
  },
  "unsupported": [
    "skeletal NPC poses",
    "door schedule, Romani's Mask membership gate and 21:00/05:00 kickouts",
    "CAM_SET_FIXED0 stage camera (bgCam 0)",
    "Dm_Char07 stage dynapoly until object_milkbar is extracted",
    "positional Obj_Sound BGM",
    "sound-check cutscene chain, spotlights, Link clones and Ballad playback",
    "WALL_TYPE_1 no-ledge-climb walls"
  ],
  "evidence": ["docs/city/interiors/milk_bar.md"],
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

Camera proposals (REF-03 IDs, not records): `cam.milk_bar.entry` from spawn 0 facing yaw 180 down the stairs; `cam.milk_bar.reverse_door` on the Y 152 landing facing +Z at TA0; `cam.milk_bar.fixture` at the source bgCam 0 pose (-17, 244, -188), pitch 51 deg, yaw 180, toward the stage.

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Writer of Milk Bar layer 3 (`cutsceneIndex 0xFFF2`, or a script `nextEntrance 0x2403`) | Extract the non-city credits scenes (or grep their baserom script lists for `0x2403`); `src/` and the 22 city scenes have none |
| U-02 | Which sound-check branch reaches actor cutscene 14 (script 0, mask) versus 15 (script 1, boo) | Count `EnToto_SetupTalk_StartAdditionalCutscene` calls per path (`z_en_toto.c:487-494`, `:703-706`, `:744`, the `ENTOTO_TALK_START_GORMAN_CUTSCENE` handler) from csId 11 |
| U-03 | Consumer of actor cutscenes 0-8 (scene.c:322-330) | Which Milk Bar actor or engine path starts csId 0; grep `CutsceneManager_` users whose `actor.csId` can be 0 here; else unused |
| U-04 | En_Takaraya a00 / En_Syateki_Man a01 placed without their objects | Confirm no code loads `OBJECT_BG`/`OBJECT_SHN` in this scene; record as dead entries |
| U-05 | `OBJECT_MBAR_OBJ` loaded in all headers, `Bg_Mbar_Chair` never spawned | grep `ACTOR_BG_MBAR_CHAIR` in non-city scene actor lists and in code |
| U-06 | `OBJECT_OS_ANIME` consumer here | Which runtime-spawned actor uses it (only `ovl_En_Cne_01` references it) |
| U-07 | Who sets `En_Gakufu` `home.rot.x` (score display timer) | grep `ACTOR_EN_GAKUFU` and `home.rot.x =` in `ovl_En_Toto` and `ovl_En_Gm` |
| U-08 | Contents of texts 0x1812, 0x1821, 0x1822, 0x1889, 0x188C, 0x2Bxx | Message data (NI); only ids are cited |
| U-09 | No registered profile shows the bar open at night | Integrator: register a 22:00-04:59 Day 1/2 profile with Romani's Mask worn (for example `prof.d1.late.normal`) and re-point `cam.milk_bar.fixture`'s night variant |
| U-10 | Stage dynapoly collision content and its overlap with the scene stage deck (Y 22) | Extract `object_milkbar`; compare `Colheader_006688` with polys 10-11 |
| U-11 | Credits Lulu/Tijo self-kill on `CLEARED_GREAT_BAY_TEMPLE` | Which flags hold during the ending (credits save state); read the ending entry path |
| U-12 | Alternates keep exit surface 5 with no exit list | Confirm the player stays in a cutscene action for the whole credits layer (`player.c:6409-6410`) |
| U-13 | Meaning of bgCam 0's third triplet (-1, -1, -1) | Read the `CAM_FUNC_FIXED*` data parse in `src/code/z_camera.c` |
| U-14 | Counter, stools, ledges are height-based INF | Render or capture `cam.milk_bar.fixture` and a top view (REF-03) |
| U-15 | Lifetime of `sDoorIsMilkBarMember` (never cleared in the overlay) | Check overlay reload / BSS clear on scene change (`Actor_LoadOverlay`) |
| U-16 | Day 3 18:00-21:00: Barten at the counter while the door is free | Runtime check; text/menu path `D_80BE1A0C` |
| U-17 | LOCATION_REGISTER says transition actors are identical in every header | Milk Bar alternates change TA0 params `0x028A` -> `0x007F` (scene.c:47 vs :1163, :1375, :1525, :1785); register owner to reword |
| U-18 | Two writers of layer 1 (Kamaro's Mask credits and East alt2's destination) | Which credits sequence passes through East alt2 (East U-01) |
| U-19 | Exit [1] `0xD650` unreferenced here | Same question as PORTAL_REGISTER U4 in the other eight scenes |

**Checks run:** read the brief, `docs/HANDOFF.md`, FULL_CITY_PLAN sections 6/11/12, LOCATION/PORTAL/SOURCE registers (Milk Bar rows), the North template, East sections 1.1 and 2.4, `shared/city/schema/README.md`, `region.schema.json`, `ids.json` and the `bomb_shop`/`laundry_pool`/`south_clock_town` records; scene.c, room.c and the XML; `scene_table.h`, `z_scene_table.c`, `z_actor.c`, `z_scene.c`, `z_play.c`, `z_demo.c`, `z_schedule.c`, `z64scripts.h`, `z_msgevent.c`, `z_sub_s.c`, `z64bgcheck.h`, `z_bgcheck.c`, `z64camera.h`, `z64player.h`, `player.c` at the cited lines; overlays En_Door, En_Tab, En_Gm, En_Toto, En_Al, En_Pm, En_Time_Tag, En_Test4, En_Gakufu, Dm_Char07, Obj_Milk_Bin, Obj_Sound, Bg_Mbar_Chair and the credits actors at the cited lines; scratch parsers `actors.py` (5 actor and 5 object lists), `cs.py` (cutscene commands), `col.py` (collision counts, exit polys, door probe, floor bands), outputs in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\milk_bar\`; SHA-256 of the five inputs and the ZAPD binary; `stat` of object binaries; greps for `0xFFF2`, `0x2403`, `ENTRANCE(MILK_BAR`, `SCENE_MILK_BAR`, `GI_MASK_CIRCUS_LEADER`, `ACTOR_BG_MBAR_CHAIR`, `OBJECT_OS_ANIME`.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes, `region_from_dossier.py`, `validate_city.py`; formal schema validation of section 10 (left to the integrator); message text, light-setting values, room DL contents, object-bank geometry, non-city credits scenes.

**Next dependency:** integrator lifts and validates section 10; U-09 profile decision for the night fixture; section 9 extraction, then a ROOM pass that reuses the ROOM-01 door/return pattern.
