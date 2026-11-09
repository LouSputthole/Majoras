# North Clock Town (`loc.north_clock_town`): source inventory dossier

Task **REF-05** (FULL_CITY_PLAN.md section 12). Base commit `d0bc29f3ccf38e53ed7f92559cb2d62c3ebd1e1b` on `main`. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_BACKTOWN` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_BACKTOWN/Z2_BACKTOWN.c`, `room.c` = `.../Z2_BACKTOWN_room_00.c`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_BACKTOWN` / `SCENE_BACKTOWN` | `include/tables/scene_table.h:225` |
| Scene-table index | `0x6E` (110) | same line |
| Entrance-table scene index | `0x6B` (`ENTR_SCENE_NORTH_CLOCK_TOWN`) | `src/code/z_scene_table.c:2680`, `include/z64scene.h:700` |
| Title card text / draw config / restrictions | `0x103` / `SCENE_DRAW_CFG_DEFAULT` / `RESTRICTIONS_NONE` | scene_table.h:225 |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_SET(0, 0, 0, (1 << 10))`: switch0, switch1, chest = 0; collectible bit 10 | scene_table.h:225; arg order `src/code/z_sram_NES.c:23`. INF: bit 10 is the heart piece's collectible flag `0x0A` (section 4.3) |
| XML files | Scene `Z2_BACKTOWN` segment 2; room `Z2_BACKTOWN_room_00` segment 3; three room DLs declared: `0x5E0`, `0x8148`, `0xA138` | XML `assets/xml/scenes/Z2_BACKTOWN/Z2_BACKTOWN.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)` in both scene headers) | HDR scene.c:16, :913 |
| Scene headers | default + 1 alternate (`Z2_BACKTOWNSet_002270`) | HDR scene.c:34-36 |
| Room headers | default + 1 alternate (`Z2_BACKTOWN_room_00Set_000370`) | HDR room.c:25-27 |
| Special files | `NAVI_QUEST_HINTS_NONE`, keep object `GAMEPLAY_FIELD_KEEP` | HDR scene.c:20 |
| Sound | spec 0, ambience 0, seq 29 (`Sequence_29` = `NA_BGM_CLOCK_TOWN_MAIN_SEQUENCE`, ordinal assumed from table order) | HDR scene.c:15; `include/tables/sequence_table.h:78` |
| Skybox | file 0, skybox 1, weather 10 (alt: 0), lightMode false | HDR scene.c:23, :920; macro `include/z64scene.h:833` |
| Env light settings | 29 entries per header (values differ by header; not transcribed) | HDR |
| Animated materials | single entry `{ 0, 6, NULL }`; segment 0 is skipped by `AnimatedMat_DrawMain` | HDR scene.c:356; `src/code/z_scene_proc.c:400` |
| Minimap | 1 entry `{0x0105, 0xFEC0, 0, 0xFC40, 0}` (mapId, centerX -320, floorY 0, centerZ -960), scale 52 | HDR scene.c:277-283 |
| Transition actors | none: no `SCENE_CMD_TRANSITION_ACTOR_LIST` in either header | HDR (grep) |
| Room commands | echo 4 (alt 0); `ROOM_BEHAVIOR(0, 0, 0, 0, false, 1)`; `SKYBOX_DISABLES(false, false)`; `TIME_SETTINGS(255, 255, 3)`; cullable room shape (type 2, 13 entries) | HDR room.c:15-21, :201-208 |

### 1.1 What scene layer 1 (alternate header) changes

Selection (C): `Scene_CommandAltHeaderList` uses `altHeaders[sceneLayer - 1]` when `sceneLayer != 0` (`src/code/z_scene.c:465-474`). `Play_Init` sets `sceneLayer = (cutsceneIndex & 0xF) + 1` only when the game mode is not normal/title or `cutsceneIndex >= 0xFFF0`, else 0 (`src/code/z_play.c:2210-2219`). The North entrance table has a layer-1 row only for entrance 0: `{ SCENE_BACKTOWN, 0, 0x458B }` (`z_scene_table.c:2306-2309`). So ordinary Day 1 play uses the **default** headers; layer 1 is a cutscene/special-mode layer (which cutscene uses it: UNKNOWN, U-01).

| Item | Default (layer 0) | Alternate (layer 1) |
| --- | --- | --- |
| Spawns / entrance list | 8 / 8 rows (entrance i -> spawn i, room 0) | 1 / 2 rows (both -> spawn 0); spawn 0 params `0x0E09` instead of `0x0E0B` |
| Exit list | 8 words | identical 8 words |
| Collision | `Z2_BACKTOWNCollisionHeader_002238` | same header |
| Cutscene scripts | 2 (both `CutsceneData_0004D4`: title card + camera spline; spawn 1 and 2, spawnFlags `0x15`) | none |
| Actor cutscene list / cs cameras | 22 / 15 | 20 / 12 |
| Paths | 5 | 5; path 2 and path 4 points differ slightly |
| Skybox weather | 10 | 0 |
| Room object list | 19 banks | same 19, same order |
| Room actor list | 33 | 32: **no tower clock** (default index 32); grotto `Door_Ana` at (-970, 292, -1676) instead of (-966, 292, -1703); sign at (-917, 292, -1651) instead of (-913, 292, -1678); both `En_Baba` params differ (`0x05FF`, `0xFFFF`); `En_Test4` params `0x0C20` (bell volume index 1) instead of `0x0C00`; list order differs |
| Room echo | 4 | 0 |

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:38-55; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam index `params & 0xFF` (`include/z64player.h:17-40`). Yaw = `(rawRotY >> 7) & 0x1FF` degrees.

| Spawn | Position | Raw rot X / Y / Z | Yaw | Params | Start mode | bgCam idx | Used by (C unless marked) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-473, 191, -2439) | 0x0007 / 0x0000 / 0x007F | 0 | 0x0E0B | 0xE | 0x0B | North entrance 0; INF arrival from Termina Field (2 to 80 units inside the TF exit floor, facing +Z) |
| 1 | (562, 193, -1785) | 0x0007 / 0x8700 / 0x007F | 270 | 0x0F09 | 0xF | 0x09 | Entrance 1 = `0xD610`, used by `Z2_TOWN` exit lists (cross-check) |
| 2 | (-265, 189, -1656) | 0x0007 / 0x5A00 / 0x007F | 180 | 0x0F0A | 0xF | 0x0A | Entrance 2 = `0xD620`, `Z2_CLOCKTOWER` exit list (playbook row "North") |
| 3 | (-1033, 292, -2008) | 0x0007 / 0x2D00 / 0x007F | 90 | 0x0E0C | 0xE | 0x0C | Entrance 3 = `0xD630`, `Z2_YOUSEI_IZUMI` exit index 0 (cross-check) |
| 4 | (-972, 292, -1702) | 0x0007 / 0x3B80 / 0x007F | 119 | 0x04FF | 0x4 `PLAYER_START_MODE_GROTTO` | none | Entrance 4 = `0xD640`, `Z2_DEKUTES` exit index 0 (cross-check); 6 units from the default grotto |
| 5 | (-666, 200, -2256) | 0x0007 / 0x5A00 / 0x007F | 180 | 0x0DFF | 0xD | none | Exit index 5 (`0xD650`) read by `En_Bomjimb` (OVL z_en_bomjimb.c:861); `0xD650` also appears in exit lists of ALLEY, CLOCKTOWER, ICHIBA, MILK_BAR, POSTHOUSE, TAKARAYA, TOWN, YADOYA |
| 6 | (-486, 188, -2004) | 0x0007 / 0x1D00 / 0x007F | 58 | 0x0DFF | 0xD | none | `En_Bomjima`: `Entrance_CreateFromSpawn(6)` (OVL z_en_bomjima.c:830) |
| 7 | (57, 190, -1808) | 0x0007 / 0x2D00 / 0x007F | 90 | 0x0DFF | 0xD | none | `ENTRANCE(NORTH_CLOCK_TOWN, 7)` literal in `En_Suttari` and `En_Baba` (OVL) |

Alternate header: one spawn, identical to spawn 0 except params `0x0E09`.

Entrance-table rows for North (C, z_scene_table.c:2306-2339): entrance 0 `{0, 0x4102}` + layer 1 `{0, 0x458B}`; 1 and 2 `0xC102`; 3 and 4 `0x4102`; 5, 6, 7 `0x8A14`. Flag semantics: NI (REF-02).

### 2.2 Exit list (HDR scene.c:72-81 and :944-953, identical) and consumers

Playbook decode, labeled **cross-check only** (REF-02 owns `PORTAL_REGISTER.md`): entrance-table scene = `value >> 9`, entrance = `(value >> 4) & 0x1F`, layer = `value & 0xF` (`Entrance_GetTableEntry`, z_scene_table.c:2688-2694).

| Exit idx | Raw | Cross-check decode | Destination row (z_scene_table.c) | Consumer in North |
| --- | --- | --- | --- | --- |
| 0 | `0x5480` | 0x2A `Z2_00KEIKOKU`, entr 8 | `sTerminaFieldEntrance8` (889): `SCENE_00KEIKOKU` spawn 8 | collision surface type 5 (2 polys) |
| 1 | `0xD250` | 0x69 `Z2_TOWN`, entr 5 | `sEastClockTownEntrance5` (2220): `SCENE_TOWN` spawn 5 | surface type 6 (2 polys) |
| 2 | `0xD840` | 0x6C `Z2_CLOCKTOWER`, entr 4 | `sSouthClockTownEntrance4` (2364): `SCENE_CLOCKTOWER` spawn 4 | surface type 3 (4 polys) |
| 3 | `0x4600` | 0x23 `Z2_YOUSEI_IZUMI`, entr 0 | `sFairyFountainEntrance0` (698): `SCENE_YOUSEI_IZUMI` spawn 0 | surface type 4 (2 polys) |
| 4 | `0x3600` | 0x1B `Z2_DEKUTES`, entr 0 | `sDekuScrubPlaygroundEntrance0` (524): `SCENE_DEKUTES` spawn 0 | `Door_Ana` params `0x0304` (default idx 0, alt idx 2) |
| 5 | `0xD650` | 0x6B North, entr 5 | spawn 5 | `En_Bomjimb` params `(p >> 8) & 0xFF = 0x05` (default idx 13, 26) |
| 6 | `0xD670` | 0x6B North, entr 7 | spawn 7 | no `setupExitList[6]` reader found in the 21 North overlays; the value equals the literal used by En_Suttari/En_Baba (U-06) |
| 7 | `0x0000` | n/a | n/a | none; trailing zero (LOCATION_REGISTER also marks it "last 0x0000") |

### 2.3 Collision surfaces carrying exits (C from scene.c:368-372 and the polygon list)

`exitIndexPlusOne` = bits 8-12 of `SurfaceType.data[0]` (`include/z64bgcheck.h:79-87`). All 10 exit polygons also carry vIB flag 1 (`COLPOLY_IS_FLOOR_CONVEYOR`, z64bgcheck.h:71) and conveyor speed 3 in `data[1]`; conveyor direction x 1024 = binang.

| Surface type | data[0] / data[1] | Exit idx | Polys (indices) | Trigger extent x / y / z | Conveyor dir | INF outward side |
| --- | --- | --- | --- | --- | --- | --- |
| 3 | 0x00000300 / 0x000C17C0 | 2 South | 4 (92, 95, 135, 136) | -320..-200 / 191..200 / -1599..-1280 | 0 (+Z) | toward South (+Z) |
| 4 | 0x00000400 / 0x060C17C0 | 3 Fairy Fountain | 2 (118, 119) | -1291..-1117 / 243..284 / -2073..-1953 | 48 (-X) | west end of the upper west level |
| 5 | 0x00000100 / 0x040C17C0 | 0 Termina Field | 2 (125, 126) | -560..-380 / 200 / -2773..-2507 | 32 (-Z) | north edge |
| 6 | 0x00000200 / 0x020C17C0 | 1 East | 2 (138, 139) | 613..880 / 200 / -1864..-1704 | 16 (+X) | east edge |

### 2.4 Great Fairy fountain and Deku Playground access (source finding)

- **Great Fairy fountain** = collision exit, not an actor. Surface type 4 (polys 118, 119; sloped floor, normal (-7515, 31894, 0)) -> exit index 3 -> `0x4600` -> fountain entrance 0 -> `Z2_YOUSEI_IZUMI` entrance list row 0 = spawn 0, **room 0** (`Z2_YOUSEI_IZUMI.c:72-75`). The fountain's exit index 0 is `0xD630` -> North spawn 3 at (-1033, 292, -2008), facing +X, about 90 units east of the trigger: reciprocal pair (cross-check; REF-02 / INT-13 confirm the room variant).
- **Deku Playground** = grotto hole `Door_Ana`, default index 0 at (-966, 292, -1703) (alt index 2 at (-970, 292, -1676)), params `0x0304`. OVL `z_door_ana.h`: type `params & 0x300 = 0x300` (`DOORANA_TYPE_VISIBLE_SCENE_EXIT`), exit index `params & 0x1F = 4`; `DoorAna_WaitOpen` sets `nextEntrance = setupExitList[4]` = `0x3600` (z_door_ana.c:135-138) -> `Z2_DEKUTES` entrance 0. `Z2_DEKUTES` exit 0 is `0xD640` -> North spawn 4 with `PLAYER_START_MODE_GROTTO` beside the hole: reciprocal (cross-check).
- Both openings are on the upper west level (Y about 243 to 292). Whether that level is reachable on foot from the main ground or needs the Deku flower at (-854, 250, -2352) or (-866, 292, -1853): UNKNOWN (U-03).

### 2.5 Proposed portal IDs (cross-reference `docs/city/PORTAL_REGISTER.md`, REF-02)

| Proposed ID | Source evidence | Arrival back into North |
| --- | --- | --- |
| `portal.north_clock_town.south_clock_town` | surface 3, exit 2 `0xD840` | spawn 2 via `0xD620` |
| `portal.north_clock_town.east_clock_town` | surface 6, exit 1 `0xD250` | spawn 1 via `0xD610` |
| `portal.north_clock_town.z2_00keikoku` (to `ext.z2_00keikoku`) | surface 5, exit 0 `0x5480` | spawn 0 (source of `0xD600` in Termina Field NI) |
| `portal.north_clock_town.great_fairy_fountain_town` | surface 4, exit 3 `0x4600` | spawn 3 via `0xD630` |
| `portal.north_clock_town.deku_playground` | `Door_Ana` h0 a00, exit 4 `0x3600` | spawn 4 via `0xD640` |

Spawns 5, 6, 7 are event respawns inside North (Bombers chase, Jim, Sakon/Bomb Shop Lady), not district portals.

## 3. Object banks (HDR room.c:29-49 and :211-231; IDs from `include/tables/object_table.h`)

Both headers list the same 19 banks in the same order. Actor spawn fails if the profile's object is not loaded (`Actor_SpawnAsChildAndCutscene`, `src/code/z_actor.c:3517-3523`).

| # | Bank | Object ID | Consumer (profile `objectId`, OVL) | Needed for Day 1 static list |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_F53_OBJ` | 0x16D | Obj_Entotu | billboard approximation only |
| 1 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai | yes (already extracted) |
| 2 | `OBJECT_BAL` | 0x185 | En_Bal | skeletal (later) |
| 3 | `OBJECT_CS` | 0x110 | En_Bombers, En_Bomjima, En_Bomjimb | skeletal (later) |
| 4 | `OBJECT_MM` | 0x107 | En_Pm | no (schedule-hidden at noon) |
| 5 | `OBJECT_SDN` | 0x1B6 | En_Stop_heishi | skeletal (later) |
| 6 | `OBJECT_WOOD02` | 0x061 | no listed North actor (En_Wood02 uses it) | no |
| 7 | `OBJECT_PST` | 0x1CB | En_Pst | skeletal (later) |
| 8 | `OBJECT_OS_ANIME` | 0x09D | no listed North actor (referenced by `z_en_hy_code.c`, En_Cne_01) | no |
| 9 | `OBJECT_BOJ` | 0x0E3 | En_Suttari | no (night) |
| 10 | `OBJECT_TOKEI_STEP` | 0x1A4 | Obj_Tokei_Step | yes (already extracted) |
| 11 | `OBJECT_TREE` | 0x20D | Obj_Tree | yes |
| 12 | `OBJECT_KIBAKO2` | 0x133 | no listed North actor (Obj_Kibako2 uses it) | no |
| 13 | `OBJECT_LODMOON` | 0x223 | En_Fall type 6 (by `Object_GetSlot`, z_en_fall.c:166) | yes (moon, special rule) |
| 14 | `OBJECT_BBA` | 0x0DF | En_Baba | no (night) |
| 15 | `OBJECT_KANBAN` | 0x0FC | En_Kanban | yes |
| 16 | `OBJECT_KITAN` | 0x264 | En_Kitan, spawned by En_Kusa2 on trigger | no |
| 17 | `OBJECT_GI_HEART` | 0x090 | En_Item00 recovery-heart draw (`src/code/z_en_item00.c:729`) | no |
| 18 | `OBJECT_FUSEN` | 0x280 | En_Bombal | yes |

Implicit (not in the list): `gameplay_keep` 0x001 (always loaded; Obj_Etcetera, En_Item00, En_Test4, sign board DL) and `gameplay_field_keep` 0x002 (scene special file; Door_Ana, En_Kusa2).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): actor ID `id & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF`, converted with `DEG_TO_BINANG` unless the ID carries the axis flag: `0x8000` rotY, `0x4000` rotX, `0x2000` rotZ. A flagged axis is passed as a plain integer (wrapped to -180..180), not an angle. csId = `rawRotY & 0x7F` (0x7F -> `CS_ID_NONE`, 3570-3574); half-day mask = `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> `HALFDAYBIT_ALL` (3576-3580).
- `Actor_SpawnSetupActors` (2546-2575): spawns an entry only if its mask intersects the current half-day bit; with `EVENTINF_17` set, entries whose ID has bit `0x800` are additionally suppressed for the previous half-day. No North entry has `0x800`.
- `Actor_InitHalfDaysBit` (2491-2499): bit = `HALFDAYBIT_DAY0_DAWN >> (day * 2 + night)`. Day 1 at 12:00 -> `0x080`. Bits (`include/z64actor.h:328-341`): D0d 0x200, D0n 0x100, D1d 0x080, D1n 0x040, D2d 0x020, D2n 0x010, D3d 0x008, D3n 0x004, D4d 0x002, D4n 0x001.
- `Actor_Init` (1166-1191) copies home to world and world rotation to shape rotation and sets default scale 0.01 before the overlay init; `Actor_Draw` (2809-2823) builds translate(world + yOffset x scale.y) x rotateYXZ(shape.rot) x scale.
- Special ID flag in North: `En_Stop_heishi | 0x2000` (both headers). Its rotZ (0) arrives as an integer; the overlay uses it as data, `unk_288 = rotZ * 40 + 50 = 50`, then zeroes `world.rot.z` (OVL z_en_stop_heishi.c:106-107).

Classes: `static-mesh`, `posed-skeletal`, `billboard/effect`, `controller/invisible`, `npc-scheduled` (skeletal NPC whose presence/pose is schedule- or flag-gated), `door/transition`. "D1d" = mask includes Day 1 daytime.

### 4.2 Default header (33 entries, HDR room.c:51-170, decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot deg X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_DOOR_ANA` (0x055) | (-966, 292, -1703) | 0x0007 / 0x430D / 0x007F | 0 / 134 / 0 | 0x3FF all | 13 | 0x0304 | gameplay_field_keep | door/transition | yes |
| 1 | `ACTOR_EN_SUTTARI` (0x237) | (383, 240, -2494) | 0x0002 / 0x8712 / 0x0040 | 0 / 270 / 0 | 0x140 D0n, D1n | 18 | 0x83FF | object_boj | npc-scheduled | no |
| 2 | `ACTOR_OBJ_ETCETERA` (0x183) | (-854, 250, -2352) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x007F | gameplay_keep | static-mesh | yes |
| 3 | `ACTOR_OBJ_ETCETERA` (0x183) | (-866, 292, -1853) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x007F | gameplay_keep | static-mesh | yes |
| 4 | `ACTOR_EN_ITEM00` (0x00E) | (-6, 384, -2335) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0A06 | gameplay_keep | static-mesh (state pickup) | yes |
| 5 | `ACTOR_OBJ_ENTOTU` (0x14D) | (319, 553, -1308) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_f53_obj | billboard/effect | yes |
| 6 | `ACTOR_OBJ_ENTOTU` (0x14D) | (-1080, 673, -1434) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_f53_obj | billboard/effect | yes |
| 7 | `ACTOR_OBJ_TREE` (0x229) | (-5, 240, -2335) | 0x0007 / 0x967F / 0x007F | 0 / 300 / 0 | 0x3FF all | none | 0x00FF | object_tree | static-mesh | yes |
| 8 | `ACTOR_EN_STOP_HEISHI \| 0x2000` (0x1C7) | (-473, 197, -2487) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 (Z raw) | 0x3FF all | none | 0x107F | object_sdn | npc-scheduled | yes |
| 9 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-68, 555, -1012) | 0x0007 / 0x2D7F / 0x007F | 0 / 90 / 0 | 0x3FF all | none | 0x007F | object_obj_tokeidai | static-mesh | yes |
| 10 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-492, 555, -1012) | 0x0007 / 0x877F / 0x007F | 0 / 270 / 0 | 0x3FF all | none | 0x007F | object_obj_tokeidai | static-mesh | yes |
| 11 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-280, 1103, -1020) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x307F | object_obj_tokeidai | static-mesh | yes |
| 12 | `ACTOR_OBJ_TOKEI_STEP` (0x1B8) | (-280, 255, -853) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_tokei_step | static-mesh | yes |
| 13 | `ACTOR_EN_BOMJIMB` (0x27F) | (455, 240, -2557) | 0x0007 / 0xA0FF / 0x007F | 0 / 321 / 0 | 0x3FF all | none | 0x050F | object_cs | npc-scheduled | yes |
| 14 | `ACTOR_EN_BOMJIMA` (0x27E) | (-491, 188, -2060) | 0x0007 / 0x238F / 0x007F | 0 / 71 / 0 | 0x3FF all | 15 | 0xFF00 | object_cs | npc-scheduled | yes |
| 15 | `ACTOR_EN_BOMBAL` (0x282) | (-154, 393, -1977) | 0x0007 / 0x998B / 0x007F | 0 / 307 / 0 | 0x3FF all | 11 | 0x0000 | object_fusen | static-mesh | yes |
| 16 | `ACTOR_EN_KUSA2` (0x171) | (54, 240, -2464) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0500 | gameplay_field_keep | controller/invisible (spawns static bushes) | yes |
| 17 | `ACTOR_EN_PST` (0x1F2) | (-28, 209, -1939) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0001 | object_pst | posed-skeletal | yes |
| 18 | `ACTOR_EN_BABA` (0x236) | (137, 190, -1806) | 0x0002 / 0x2D7F / 0x0040 | 0 / 90 / 0 | 0x140 D0n, D1n | none | 0x02FF | object_bba | npc-scheduled | no |
| 19 | `ACTOR_EN_BAL` (0x176) | (-307, 378, -2237) | 0x0005 / 0x9C0E / 0x002A | 0 / 312 / 0 | 0x2AA all daytimes | 14 | 0x0000 | object_bal | npc-scheduled | yes |
| 20 | `ACTOR_EN_PM` (0x1D5) | (-270, 199, -1543) | 0x0001 / 0x5A7F / 0x0028 | 0 / 180 / 0 | 0x0A8 D1d, D2d, D3d | none | 0x0000 | object_mm | npc-scheduled | yes |
| 21 | `ACTOR_OBJ_TREE` (0x229) | (-654, 200, -1780) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | object_tree | static-mesh | yes |
| 22 | `ACTOR_EN_BOMBERS` (0x280) | (-754, 200, -2334) | 0x0007 / 0x000F / 0x007F | 0 / 0 / 0 | 0x3FF all | 15 | 0x0010 | object_cs | npc-scheduled | yes |
| 23 | `ACTOR_EN_BABA` (0x236) | (137, 190, -1806) | 0x0002 / 0x877F / 0x0040 | 0 / 270 / 0 | 0x140 D0n, D1n | none | 0x7FFF | object_bba | npc-scheduled | no |
| 24 | `ACTOR_EN_BOMBERS` (0x280) | (-709, 200, -2334) | 0x0007 / 0x000F / 0x007F | 0 / 0 / 0 | 0x3FF all | 15 | 0x0011 | object_cs | npc-scheduled | yes |
| 25 | `ACTOR_EN_BOMBERS` (0x280) | (-665, 200, -2334) | 0x0007 / 0x000F / 0x007F | 0 / 0 / 0 | 0x3FF all | 15 | 0x0012 | object_cs | npc-scheduled | yes |
| 26 | `ACTOR_EN_BOMJIMB` (0x27F) | (-654, 200, -1780) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0513 | object_cs | npc-scheduled | yes |
| 27 | `ACTOR_EN_BOMBERS` (0x280) | (-622, 200, -2334) | 0x0007 / 0x000F / 0x007F | 0 / 0 / 0 | 0x3FF all | 15 | 0x0013 | object_cs | npc-scheduled | yes |
| 28 | `ACTOR_EN_TEST4` (0x15A) | (-280, 1103, -1020) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | 0x3FF all | 9 | 0x0C00 | gameplay_keep | controller/invisible | yes |
| 29 | `ACTOR_EN_BOMBERS` (0x280) | (-579, 200, -2334) | 0x0007 / 0x000F / 0x007F | 0 / 0 / 0 | 0x3FF all | 15 | 0x0014 | object_cs | npc-scheduled | yes |
| 30 | `ACTOR_EN_FALL` (0x17C) | (-280, 11000, -1020) | 0x1907 / 0x07FF / 0x007F | 50 / 15 / 0 | 0x3FF all | none | 0x037F | object_lodmoon | static-mesh (camera-relative moon) | yes |
| 31 | `ACTOR_EN_KANBAN` (0x0A8) | (-913, 292, -1678) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF all | none | 0x0011 | object_kanban | static-mesh | yes |
| 32 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-280, 555, -833) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x207F | object_obj_tokeidai | static-mesh | yes |

### 4.3 Alternate room header 1 (32 entries, HDR room.c:233-348, decode C)

"Twin" = default index with the same actor/position; only differences are spelled out. Masks, CS, banks and classes equal the twin's.

| # | Actor | Position | Rot deg X / Y / Z | Params | Twin / difference |
| --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_BABA` | (137, 190, -1806) | 0 / 90 / 0 | 0x05FF | 18; path index 5 (alt has paths 0-4 only, U-12) |
| 1 | `ACTOR_EN_BABA` | (137, 190, -1806) | 0 / 270 / 0 | 0xFFFF | 23; type `(p & 0xC000) >> 14 = 3`, which `EnBaba_FinishInit` kills in this scene (OVL z_en_baba.c:543-545) |
| 2 | `ACTOR_DOOR_ANA` | (-970, 292, -1676) | 0 / 134 / 0 | 0x0304 | 0; moved 27 units +Z, 4 units -X |
| 3 | `ACTOR_EN_SUTTARI` | (383, 240, -2494) | 0 / 270 / 0 | 0x83FF | 1 |
| 4 | `ACTOR_OBJ_ETCETERA` | (-854, 250, -2352) | 0 / 0 / 0 | 0x007F | 2 |
| 5 | `ACTOR_OBJ_ETCETERA` | (-866, 292, -1853) | 0 / 0 / 0 | 0x007F | 3 |
| 6 | `ACTOR_EN_ITEM00` | (-6, 384, -2335) | 0 / 0 / 0 | 0x0A06 | 4 |
| 7 | `ACTOR_EN_BAL` | (-307, 378, -2237) | 0 / 312 / 0 | 0x0000 | 19 |
| 8 | `ACTOR_EN_PM` | (-270, 199, -1543) | 0 / 180 / 0 | 0x0000 | 20 |
| 9 | `ACTOR_OBJ_ENTOTU` | (319, 553, -1308) | 0 / 0 / 0 | 0x0000 | 5 |
| 10 | `ACTOR_OBJ_ENTOTU` | (-1080, 673, -1434) | 0 / 0 / 0 | 0x0000 | 6 |
| 11 | `ACTOR_OBJ_TREE` | (-5, 240, -2335) | 0 / 300 / 0 | 0x00FF | 7 |
| 12 | `ACTOR_OBJ_TREE` | (-654, 200, -1780) | 0 / 0 / 0 | 0x0000 | 21 |
| 13 | `ACTOR_EN_STOP_HEISHI \| 0x2000` | (-473, 197, -2487) | 0 / 0 / 0 | 0x107F | 8 |
| 14 | `ACTOR_OBJ_TOKEIDAI` | (-68, 555, -1012) | 0 / 90 / 0 | 0x007F | 9 |
| 15 | `ACTOR_OBJ_TOKEIDAI` | (-492, 555, -1012) | 0 / 270 / 0 | 0x007F | 10 |
| 16 | `ACTOR_OBJ_TOKEIDAI` | (-280, 1103, -1020) | 0 / 0 / 0 | 0x307F | 11 |
| 17 | `ACTOR_OBJ_TOKEI_STEP` | (-280, 255, -853) | 0 / 0 / 0 | 0x0000 | 12 |
| 18 | `ACTOR_EN_BOMJIMB` | (455, 240, -2557) | 0 / 321 / 0 | 0x050F | 13 |
| 19 | `ACTOR_EN_BOMJIMA` | (-491, 188, -2060) | 0 / 71 / 0 | 0xFF00 | 14 |
| 20 | `ACTOR_EN_BOMJIMB` | (-654, 200, -1780) | 0 / 0 / 0 | 0x0513 | 26 |
| 21-25 | `ACTOR_EN_BOMBERS` x5 | x = -754, -709, -665, -622, -579; y 200; z -2334 | 0 / 0 / 0 | 0x0010-0x0014 | 22, 24, 25, 27, 29 |
| 26 | `ACTOR_EN_BOMBAL` | (-154, 393, -1977) | 0 / 307 / 0 | 0x0000 | 15 |
| 27 | `ACTOR_EN_KANBAN` | (-917, 292, -1651) | 0 / 180 / 0 | 0x0011 | 31; moved 27 units +Z, 4 units -X |
| 28 | `ACTOR_EN_KUSA2` | (54, 240, -2464) | 0 / 0 / 0 | 0x0500 | 16 |
| 29 | `ACTOR_EN_TEST4` | (-280, 1103, -1020) | 0 / 0 / 0 | 0x0C20 | 28; bell volume index 1 |
| 30 | `ACTOR_EN_PST` | (-28, 209, -1939) | 0 / 0 / 0 | 0x0001 | 17 |
| 31 | `ACTOR_EN_FALL` | (-280, 11000, -1020) | 50 / 15 / 0 | 0x037F | 30 |
| none | (absent) | | | | default 32, the tower clock, is not in layer 1 |

### 4.4 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init: scale, pivot, collision, spawned children | Draw | Presence gating at Day 1 noon |
| --- | --- | --- | --- |
| Door_Ana (grotto) | Init zeroes shape yaw/roll; visible types keep default scale 0.01 (steps toward 0.01) | XLU `gameplay_field_keep_DL_000C40`; Update sets shape yaw to camera yaw + 180 (billboard) | always; exit logic in 2.4 |
| En_Suttari (Sakon) | flex skeleton `object_boj_Skel_00C240`, scale 0.01; spawns `En_Nimotsu` child and `En_Clear_Tag` effect | `SkelAnime_DrawTransformFlexOpa` | night mask only |
| Obj_Etcetera (Deku flower) | type `(p & 0xFF80) >> 7 = 0` pink; scale (0.01, 0.02, 0.01); dynapoly `gPinkDekuFlowerCol` (gameplay_keep 0xE710) | idle: `gPinkDekuFlowerIdleDL` (0xED80) static DL; skeleton only while animating (lines 342-364) | always |
| En_Item00 (`src/code/z_en_item00.c`) | low byte 0x06 = `ITEM00_HEART_PIECE`, collectible flag `(p >> 8) & 0x7F = 0x0A`; if collected it becomes a recovery heart (85-95); scale 0.02 | XLU `gHeartPieceInteriorDL` (gameplay_keep 0x5AAB0); yaw += 0x3C0 per frame | save-state dependent; INF sits 144 units above tree 7's base (tree top collision) |
| Obj_Entotu (chimney) | no transform setup | billboard: yaw = camera + 180, scale (0.1, 0.1, 0); opaque `object_f53_obj_DL_000158`, XLU smoke `DL_001C00` with scrolling texture and vertex alpha | smoke on in 06-09, 11-14, 16-20 h windows with random minute edges (lines 30-80); at 12:00 the target alpha is 1 |
| Obj_Tree | params bit 15 clear: scale 0.1, dynapoly `gTreeTopCol` (0x1B2C); cylinder collider r28 h120 | opaque `gTreeBodyDL` (0x680), then `gTreeLeavesDL` (0x7C8) under the sway rotation (identity at rest) | always; sways only when struck |
| En_Stop_heishi (gate soldier) | flex `gSoldierSkel` (object_sdn 0xD640), anim `gSoldierStandHandOnHipAnim`, scale 0.01; switch flag `p & 0x3F = 0x3F` (killed if set); type nibble `p >> 12 = 1` | skeletal | present unless switch 0x3F; form/night/`WEEKEVENTREG_12_20` checks (lines 168-523, NI in detail) |
| Obj_Tokeidai | ICHAIN scale 0.1; type `p >> 12`: 0 gear, 3 counterweight, 2 clock; tower-open branches only for Day 3 night or later, or `SCENE_CLOCKTOWER`/`SCENE_00KEIKOKU` layer 2 (lines 117-204) | gear `gClockTowerExteriorGearDL` 0xBA78; counterweight 0xB208 + spotlight 0xB0C0 (intensity 0 by day); clock per ROM_REFERENCE.md | always; same params, anchors and yaws as South's instances |
| Obj_Tokei_Step | ICHAIN scale 0.1; dynapoly `gClocktowerPanelCol` (0x968) on Day 1 | `gClocktowerPanelDL` (0x88), closed | always |
| En_Bomjimb (chase Bombers) | flex `gBomberSkel`, scale 0.01; `(p >> 8) & 0xFF` = exit index 5 | skeletal | killed unless `WEEKEVENTREG_73_10` or `85_02` set and `75_40` clear (lines 176-221): absent on a fresh save (INF: flags unset) |
| En_Bomjima (Jim) | flex `gBomberSkel`, scale 0.01; clears `WEEKEVENTREG_83_04`; may spawn `En_Bombal` (line 632) | skeletal | killed if `75_40`, `73_10` or `85_02` (208-211): present on a fresh save |
| En_Bombal (Majora balloon) | scale approaches 0.1; vertical bob from `velocity.y = sin`; pops on hit (`En_Clear_Tag` + 100 sparkle effects) | opaque `gMajoraBalloonDL` (0xA00) + `gMajoraBalloonKnotDL` (0xD78) | no init-time removal in this overlay |
| En_Kusa2 (Keaton grass) | params bit 0 = 0: controller, `draw = NULL`; within 1600 units it spawns 9 `En_Kusa2` children (params 1): one at its position, eight at radius 80 on yaw steps of 45 deg starting at +Z, each with random yaw, Y snapped by floor raycast from +30 (lines 93-125, 252-268); ICHAIN scale 0.4; may spawn `En_Kitan` with params `0x05 << 9` | children: opaque `gKusaBushType1DL` (gameplay_field_keep 0x78A0); fade between 1200 and 1300 projected Z | always |
| En_Pst (postbox) | flex `gPostboxSkel` (0x1A80), idle anim, scale 0.02 | skeletal, drawn only while the schedule result is not none | script 1 returns "checked by postman" 10:03-10:07, otherwise available: drawn at 12:00 |
| En_Baba (Bomb Shop Lady) | flex `gBombShopLadySkel`, scale 0.01; path `(p & 0x3F00) >> 8`, type `(p & 0xC000) >> 14` | skeletal | night mask only |
| En_Bal (Tingle) | flex `gTingleSkel`, scale 0.02; map id `p & 0xFF = 0`; if magic is not acquired, Y is snapped to the floor and ground idle is used, else float idle (lines 193-229) | skeletal (balloon is a limb) | daytime mask; pose depends on save state |
| En_Pm (Postman) | flex `object_mm_Skel_0096E8`, scale 0.01 | skeletal, drawn only if schedule result != 0 (line 2032) | script 0 (fresh flags, lines 386-404) has North only 09:49-10:21 on Day 1 (`scheduleScripts.schl:37-45`): not drawn at 12:00 |
| En_Bombers (type 1) | flex `gBomberSkel`, scale 0.01; type `(p >> 4) & 0xF = 1`, number `p & 0xF` = 0-4 | skeletal | type 1 survives only with `73_10` or `85_02` plus its `76_xx` bit (lines 155-197): absent on a fresh save |
| En_Test4 | three-day controller; stars `(p >> 10) * 100 = 300`; bell volume index `(p >> 5) & 0xF` | profile draw `NULL` | always |
| En_Fall | type `(p & 0xF80) >> 7 = 6` LOD moon; Day 1 scale 0.16 x 2.4 = 0.384 (lines 80-158); beyond 9000 units from the eye it is redrawn at 9000 (818-827) | `gLodmoonEyesDL` 0x10E0 + `gLodmoonMoonDL` 0x1158 | always (see ROM_REFERENCE.md moon caveats) |
| En_Kanban (sign) | scale 0.01; textId `p \| 0x300 = 0x0311` | `gSignMaterialDL` (object_kanban 0xC30) then `gSignRectangularDL` (gameplay_keep 0x5AED0) after a model-space translate (0, 0, -100); draw sets `world.y = home.y - 15` for Human or Deku form (lines 966-971) | always |

### 4.5 Counts

| | Default | Alternate 1 |
| --- | --- | --- |
| Entries | 33 | 32 |
| Day 1 daytime candidates by mask | 30 (excluded: 1, 18, 23) | 29 (excluded: 0, 1, 3) |
| static-mesh (incl. heart piece, moon) | 13 | 12 |
| posed-skeletal | 1 | 1 |
| npc-scheduled | 14 | 14 |
| billboard/effect | 2 | 2 |
| controller/invisible | 2 | 2 |
| door/transition | 1 | 1 |

## 5. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

Assumptions to record in the profile: fresh save (no week-event, switch or collectible flags), Human form, magic not acquired. World positions are N64; `world = actorPos + actorRot x (actorScale x localVertex)` (ROM_REFERENCE.md). Unity position applies the documented conversion; rotation conversion under the Z mirror belongs to the importer (PIPE-01/02).

| Default # | Component / params | N64 position (Unity) | Yaw | Actor scale | Display list(s), bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| 32 | Tower clock, `0x207F` (type 2) | (-280, 555, -833) (-2.80, 5.55, 8.33) | 0 | 0.1 | minute ring 0xCF28, center/hand 0xBEE8, face 0xE818, sun/moon panel 0xC368 (object_obj_tokeidai, seg 6) | noon pose exactly as ROM_REFERENCE.md; visibility from North NI (U-04) |
| 9 | East exterior gear, `0x007F` | (-68, 555, -1012) (-0.68, 5.55, 10.12) | 90 | 0.1 | `gClockTowerExteriorGearDL` 0xBA78 | minute rotation 0 at noon |
| 10 | West exterior gear, `0x007F` | (-492, 555, -1012) (-4.92, 5.55, 10.12) | 270 | 0.1 | same DL | |
| 11 | Counterweight, `0x307F` | (-280, 1103, -1020) (-2.80, 11.03, 10.20) | 0 at init | 0.1 | `gClockTowerCounterweightDL` 0xB208 | omit spotlight 0xB0C0 (intensity 0 by day); phase convention as South |
| 12 | Closed rooftop panel, `0x0000` | (-280, 255, -853) (-2.80, 2.55, 8.53) | 0 | 0.1 | `gClocktowerPanelDL` 0x88 (object_tokei_step) | collision `gClocktowerPanelCol` 0x968 |
| 7 | Tree, `0x00FF` | (-5, 240, -2335) (-0.05, 2.40, 23.35) | 300 | 0.1 | `gTreeBodyDL` 0x680 + `gTreeLeavesDL` 0x7C8 (object_tree) | dynapoly `gTreeTopCol` 0x1B2C |
| 21 | Tree, `0x0000` | (-654, 200, -1780) (-6.54, 2.00, 17.80) | 0 | 0.1 | same | same |
| 2 | Pink Deku flower, `0x007F` | (-854, 250, -2352) (-8.54, 2.50, 23.52) | 0 | (0.01, 0.02, 0.01) | `gPinkDekuFlowerIdleDL` gameplay_keep 0xED80 (seg 4) | collision `gPinkDekuFlowerCol` 0xE710 |
| 3 | Pink Deku flower, `0x007F` | (-866, 292, -1853) (-8.66, 2.92, 18.53) | 0 | (0.01, 0.02, 0.01) | same | same |
| 16 | Keaton grass ring, 9 children of `0x0500` | center (54, 240, -2464) (0.54, 2.40, 24.64); ring XZ (54, -2384), (110.6, -2407.4), (134, -2464), (110.6, -2520.6), (54, -2544), (-2.6, -2520.6), (-26, -2464), (-2.6, -2407.4) | random per bush | 0.4 | `gKusaBushType1DL` gameplay_field_keep 0x78A0 (seg 5) | label: fixed yaw is an approximation; Y = floor raycast |
| 15 | Majora balloon, `0x0000` | (-154, 393, -1977) (-1.54, 3.93, 19.77) | 307 | 0.1 | `gMajoraBalloonDL` 0xA00 + `gMajoraBalloonKnotDL` 0xD78 (object_fusen) | rest pose at home Y; bob omitted |
| 31 | Sign, `0x0011` | (-913, 292, -1678); Human-form draw Y 277 (-9.13, 2.77, 16.78) | 180 | 0.01 | `gSignMaterialDL` object_kanban 0xC30 + `gSignRectangularDL` gameplay_keep 0x5AED0 | model-space (0, 0, -100) before draw |
| 4 | Heart piece, `0x0A06` (optional) | (-6, 384, -2335) (-0.06, 3.84, 23.35) | spins | 0.02 | `gHeartPieceInteriorDL` gameplay_keep 0x5AAB0 (XLU) | fresh-save only; fixed phase is an approximation |
| 30 | LOD moon, `0x037F` (special) | (-280, 11000, -1020) | X50 Y15 Z0 | 0.384 | `gLodmoonEyesDL` 0x10E0 + `gLodmoonMoonDL` 0x1158 (object_lodmoon) | reproduce the 9000-unit eye rule or label the approximation, as in South |

**Excluded by half-day mask (night only):** 1 En_Suttari, 18 En_Baba, 23 En_Baba.

**Excluded by state or schedule at Day 1 noon (fresh save):** 13 and 26 En_Bomjimb, 22/24/25/27/29 En_Bombers (flag-gated kill in init); 20 En_Pm (schedule none in North at 12:00).

**Unimplemented in a static assembly (present at noon, needs skeleton/pose or effect reconstruction):** 8 En_Stop_heishi (`gSoldierSkel`), 14 En_Bomjima (`gBomberSkel`), 17 En_Pst (`gPostboxSkel`), 19 En_Bal (`gTingleSkel`, ground pose without magic); billboards 5 and 6 Obj_Entotu (chimney DL 0x158 plus smoke 0x1C00; smoke is on at noon) and 0 Door_Ana (XLU 0xC40). A fixed-orientation billboard import must be labeled as such.

**No geometry:** 16 En_Kusa2 controller (children listed above), 28 En_Test4.

## 6. Collision summary (C from scene.c:363-905; tool output in the scratch folder)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 205 / 312 / 11 |
| Water boxes | 0 (`0, NULL`) |
| Camera data (`BgCamInfo`) | 2: `0x0001` `CAM_SET_NORMAL0`, `0x003D` `CAM_SET_DUNGEON3` (`include/z64camera.h:148, 208`); bgCam 1 only on surface type 8 |
| Header bounds | min (-2355, -40, -2773), max (880, 1183, -707); Unity x -23.55..8.80, y -0.40..11.83, z 7.07..27.73 |
| Actual vertex extents | x -1291..880, y 188..600, z -2773..-1280 (header box is larger: U-14) |
| Floor Y range | 188..480 (normal Y > 0.5) |
| Polygon kinds | 126 floors (normal Y > 0.5; 74 under 10 deg, 52 steeper), 177 walls (151 near vertical), 9 ceilings |
| Floor slope histogram (5-deg bins) | 0: 51, 5: 23, 10: 29, 15: 3, 20: 4, 30: 3, 35: 4, 40: 3, 45: 2, 50: 3, 55: 1 |
| Materials | grass (types 0, 2): 81; dirt (1, 3-6, 9): 192; wood (7, 8): 20; stone (10): 19 |
| Exit surfaces | 4 types, 10 polygons, all conveyor-flagged (section 2.3) |
| Floor property 9 | types 8 (2 wood floors, Y 352, x -812..-800, z -1933..-1694, bgCam 1) and 10 (19 stone sloped floors, x 155..395, Y 247..360, z -2278..-2118). OVL `z_player.c:7539, 7569`: leaving such a floor suppresses the auto-jump and the ledge-hang check |
| Other flags | floorEffect 1 on types 2, 9, 10; echo 2 on types 0-6 and 9, 0 on 7, 8, 10; no hookshot, wall type, floor type or soft flags; poly exclusion: 24 ignore-camera (type 1), 8 ignore-projectiles (type 1), 12 ignore camera + projectiles (types 7, 8) |

Floor elevation bands (centroid Y of floor polygons; x/z are union extents of the band, not one platform):

| Band | Floor polys | x | Y | z | Max slope | Surface types |
| --- | --- | --- | --- | --- | --- | --- |
| 188-205 main ground | 61 | -920..880 | 188..200 | -2773..-1280 | 16 deg | 0, 1, 3, 5, 6 |
| 206-235 | 8 | -920..520 | 200..250 | -2413..-1864 | 15 | 0, 1 |
| 235-262 | 10 | -1291..520 | 240..290 | -2653..-1953 | 16 | 0, 1, 4, 10 |
| 262-285 | 6 | -1291..322 | 243..294 | -2280..-1953 | 39 | 1, 4, 10 |
| 285-300 upper west | 13 | -1117..335 | 267..333 | -2198..-1600 | 53 | 1, 10 |
| 300-340 | 13 | 200..395 | 290..360 | -2533..-2118 | 53 | 7, 10 |
| 340-400 | 12 | -1040..424 | 316..375 | -2533..-1694 | 58 | 7, 8, 10 |
| 400-500 | 3 | -500..-440 | 465..480 | -2493..-2477 | 43 | 1 (INF: above the Termina Field gate, near cullable entry (-470, 420, -2481)) |

Survey basis: ground levels and perimeter can be measured from this collision once imported. Vegetation and landmark arrangement beyond the actor-placed trees, bushes, flowers, balloon and sign are **pending capture** (they live in room geometry or need visual review).

INF registration: North's tower actors (9-12, 32) and chimney 5 have the same params and coordinates as South's (ROM_REFERENCE.md tables; `Z2_CLOCKTOWER_room_00.c:88`). North and South appear to share one N64 world frame; South's collision z range (-1974..1200) overlaps North's (-2773..-707). Confirm by overlaying both collisions (U-05).

## 7. Materials and textures (C; counts only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 38: 33 textures + 5 TLUTs |
| By format | rgba16 25 (20 textures + 5 TLUTs), ci4 5, i4 4, i8 2, ia8 2 |
| Location | all in the scene file (segment 2); the room file holds none; all 38 are referenced by room DLs |
| Room `Gfx` display lists / `Vtx` arrays | 83 / 57 (57 `.vtx.inc` files) |
| Room shape | cullable, 13 entries: 11 opaque DLs, 7 XLU DLs (2 of them empty) |
| XML-declared DLs | `0x5E0` empty; `0x8148` calls the 11 opaque entry DLs; `0xA138` calls the 7 XLU entry DLs |
| Raw triangle commands | 345 `gsSP2Triangles` + 38 `gsSP1Triangle` = 728 triangles in source (DL reuse not resolved) |
| Animated materials | none (section 1) |

## 8. Required extraction list for the North pilot (commands NOT executed)

Done: scene `Z2_BACKTOWN` (C above). Already extracted for South: `object_obj_tokeidai`, `object_tokei_step`. Binaries for every bank below exist under `extracted/n64-us/baserom/` (checked by `stat`).

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_tree`, `object_fusen`, `object_kanban`, `object_lodmoon`, `gameplay_keep`, `gameplay_field_keep` | section 5 DLs and collision |
| Billboard approximation | `object_f53_obj` | chimneys |
| Later skeletal pass | `object_sdn`, `object_cs`, `object_pst`, `object_bal` | noon NPCs/postbox |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_tree','object_fusen','object_kanban','object_lodmoon','gameplay_keep','gameplay_field_keep','object_f53_obj')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

The keep files (`gameplay_keep` 599,520 bytes, `gameplay_field_keep`) have not been extracted with this tool before (NI); check output size and warnings separately.

## 9. Proposed region record

**Proposal, schema not frozen (PIPE-01).** Field set follows FULL_CITY_PLAN section 3.2; `null`/`"UNKNOWN"` where source does not settle a value.

```json
{
  "_status": "proposal, schema not frozen (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.north_clock_town",
  "displayLabel": "North Clock Town",
  "source": {
    "sceneSymbol": "Z2_BACKTOWN",
    "sceneEnum": "SCENE_BACKTOWN",
    "sceneTableIndex": "0x6E",
    "entranceTableSceneIndex": "0x6B",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0)",
    "rooms": { "loc.north_clock_town.room_00": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event, switch or collectible flags", "player form Human", "magic not acquired"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_BACKTOWN", "bytes": 69344, "sha256": "22468a076936183cd93741cd4d765ba8b7d7dd903f5347f95f202a5af3ca00c6" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_BACKTOWN_room_00", "bytes": 41344, "sha256": "480c42ace6fc8807acd8a40409ab6e4c15c9b012bbe8756d229705822a2ba651" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_BACKTOWN/Z2_BACKTOWN.c", "sha256": "bb483c82e669d23875a1ea86604d420aff419efb3796c950b3ce97f7c2717ab6" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_BACKTOWN/Z2_BACKTOWN_room_00.c", "sha256": "4732267915c1a43655a1b1d5a91c89149eba5c7c2beddf9fe19322d80a7ad7e7" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_BACKTOWN/Z2_BACKTOWN.xml", "sha256": "1f41cd190660782fd28e17e35fcbe4ce4aa8866748719e6c2ac72e9be1b8bdc3" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedWithSouth": "INFERRED (identical tower/chimney anchors), unverified" },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-2355, -40, -2773], "max": [880, 1183, -707] },
    "sourceCollisionVertices": { "min": [-1291, 188, -2773], "max": [880, 600, -1280] },
    "unityCollisionVertices": { "min": [-12.91, 1.88, 12.80], "max": [8.80, 6.00, 27.73] }
  },
  "anchors": [
    { "id": "anchor.nct.spawn0_tf_gate", "source": [-473, 191, -2439], "unity": [-4.73, 1.91, 24.39], "status": "proposed, not surveyed" },
    { "id": "anchor.nct.spawn1_east", "source": [562, 193, -1785], "unity": [5.62, 1.93, 17.85], "status": "proposed, not surveyed" },
    { "id": "anchor.nct.spawn3_fountain", "source": [-1033, 292, -2008], "unity": [-10.33, 2.92, 20.08], "status": "proposed, not surveyed" },
    { "id": "anchor.nct.tree_a07_base", "source": [-5, 240, -2335], "unity": [-0.05, 2.40, 23.35], "status": "proposed, not surveyed" }
  ],
  "rooms": [ { "id": "loc.north_clock_town.room_00", "sourceRoom": "Z2_BACKTOWN_room_00", "roomShape": "cullable, 13 entries", "displayLists": 83 } ],
  "components": [
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a32", "actorId": "0x19C", "params": "0x207F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "0xBEE8", "0xE818", "0xC368"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a09", "actorId": "0x19C", "params": "0x007F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xBA78"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a10", "actorId": "0x19C", "params": "0x007F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xBA78"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a11", "actorId": "0x19C", "params": "0x307F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xB208"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a12", "actorId": "0x1B8", "params": "0x0000", "class": "static-mesh", "dls": ["object_tokei_step:0x88"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a07", "actorId": "0x229", "params": "0x00FF", "class": "static-mesh", "dls": ["object_tree:0x680", "object_tree:0x7C8"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a21", "actorId": "0x229", "params": "0x0000", "class": "static-mesh", "dls": ["object_tree:0x680", "object_tree:0x7C8"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a02", "actorId": "0x183", "params": "0x007F", "class": "static-mesh", "dls": ["gameplay_keep:0xED80"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a03", "actorId": "0x183", "params": "0x007F", "class": "static-mesh", "dls": ["gameplay_keep:0xED80"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a16/child0-8", "actorId": "0x171", "params": "0x0001", "class": "static-mesh (spawned)", "dls": ["gameplay_field_keep:0x78A0"], "status": "selected, yaw approximated" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a15", "actorId": "0x282", "params": "0x0000", "class": "static-mesh", "dls": ["object_fusen:0xA00", "object_fusen:0xD78"], "status": "selected" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a31", "actorId": "0x0A8", "params": "0x0011", "class": "static-mesh", "dls": ["object_kanban:0xC30", "gameplay_keep:0x5AED0"], "status": "selected, Human-form Y" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a04", "actorId": "0x00E", "params": "0x0A06", "class": "static-mesh (state pickup)", "dls": ["gameplay_keep:0x5AAB0"], "status": "optional, fresh save" },
    { "sourceKey": "Z2_BACKTOWN/h0/room_00/a30", "actorId": "0x17C", "params": "0x037F", "class": "static-mesh (camera-relative)", "dls": ["object_lodmoon:0x10E0", "object_lodmoon:0x1158"], "status": "selected, special rule" }
  ],
  "collisionOwnership": { "room": "Z2_BACKTOWNCollisionHeader_002238 (312 polys)", "actorDynapoly": ["a02/a03 gPinkDekuFlowerCol", "a07/a21 gTreeTopCol", "a12 gClocktowerPanelCol"], "actorCylinders": "NI" },
  "portalIds": [
    "portal.north_clock_town.south_clock_town",
    "portal.north_clock_town.east_clock_town",
    "portal.north_clock_town.z2_00keikoku",
    "portal.north_clock_town.great_fairy_fountain_town",
    "portal.north_clock_town.deku_playground",
    "portal.north_clock_town.north_clock_town.bomjimb"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a08 En_Stop_heishi", "a14 En_Bomjima", "a17 En_Pst", "a19 En_Bal", "a05/a06 Obj_Entotu billboards and smoke", "a00 Door_Ana billboard", "Link and Tatl"],
  "excluded": { "halfDayMask": ["a01", "a18", "a23"], "stateOrSchedule": ["a13", "a26", "a22", "a24", "a25", "a27", "a29", "a20"], "noGeometry": ["a16 controller", "a28"] },
  "unsupported": ["skeletal NPC poses", "billboard orientation", "smoke vertex alpha and texture scroll", "conveyor exit floors", "floor property 9 behavior", "moon 9000-unit rule unless implemented"],
  "evidence": ["docs/city/districts/north_clock_town.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

## 10. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | What enters North on scene layer 1 | Which cutscene/transition sets `nextEntrance = ENTRANCE(NORTH_CLOCK_TOWN, 0)` with `nextCutsceneIndex >= 0xFFF0` (search cutscene destination commands and `z_demo.c`); what do entrance flags `0x458B` mean |
| U-02 | Spawn bgCam indices 0x09-0x0C exceed the 2-entry BgCamInfo list | Which code consumes `PLAYER_GET_BG_CAM_INDEX` (not referenced in `z_player.c`); do these index the 15-entry actor cutscene camera list |
| U-03 | Foot access to the upper west level (grotto, fountain exit, flower 3, sign) | Do floor polygons connect from Y 188-200 to Y 292 through slopes under the walkable limit, or is the Deku flower the only route; answer from collision edge connectivity, then a route capture |
| U-04 | Visibility of the tower clock (a32) and panel (a12) from North | Render room + actors or capture from spawn 2 facing +Z; the room shape includes cullable entries near the tower ((-204, 402, -940), (-424, 479, -1194)) |
| U-05 | Shared world frame with South | Overlay both collisions; check that North surface 3 and South's North exit polygons (8, 9) coincide |
| U-06 | Exit index 6 `0xD670` reader | Search all overlays and cutscene data that can run in North for `setupExitList[6]`, or confirm unused |
| U-07 | Entrance 0 source and entrance 5's many senders | Extract `Z2_00KEIKOKU` (exit `0xD600`); REF-02 to classify `0xD650` in eight scenes |
| U-08 | Gate soldier passage rule and switch flag 0x3F | Read `EnStopheishi` actions (z_en_stop_heishi.c:160-530): form, night and `WEEKEVENTREG_12_20` logic |
| U-09 | Tingle pose and position for the profile | Profile must declare magic state; read `EnBal_SetupGroundIdle` for the ground animation |
| U-10 | Meaning of `WEEKEVENTREG_73_10`, `85_02`, `75_40`, `76_xx` | Find where each is set (grep `SET_WEEKEVENTREG`) to name the Bombers phases |
| U-11 | Keaton bush yaw and exact Y | Choose a fixed-yaw convention (PIPE-03); compute Y by raycast in the importer |
| U-12 | Alternate-header En_Baba path 5 with 5 paths | `SubS_GetPathByIndex` has no bounds check (`z_sub_s.c:1115`); night-only, layer 1: low priority |
| U-13 | Conveyor exit floors | Read conveyor handling in `z_bgcheck.c`/`z_player.c` before the walker treats exit floors as plain ground |
| U-14 | Collision header bounds larger than vertex extents | Check how `BgCheck` uses `minBounds/maxBounds` (subdivision) before using them as region bounds |
| U-15 | Banks with no listed consumer (`object_wood02`, `object_kibako2`, `object_os_anime`) | Which runtime-spawned actors in North use them (e.g. children of En_Suttari: `En_Door`, `En_Fsn`, `En_Nimotsu`) |
| U-16 | Fountain room/variant | INT-13: confirm `Z2_YOUSEI_IZUMI` room 0 and its exit 0 trigger |
| U-17 | Visual survey | Capture arrival, perimeter, upper levels and foliage under the Day 1 noon profile (REF-03) |

**Not inspected:** room DL contents, combiners and geometry; light-setting values; cutscene data semantics and actor cutscene entries; minimap rendering; actor collider shapes beyond those quoted; En_Suttari, En_Baba, En_Bal, En_Pm behavior beyond init/draw/schedule gates; En_Kitan, En_Nimotsu, En_Bom_Bowl_Man; `Z2_00KEIKOKU`; reverse-side triggers in neighbor scenes beyond exit-list values; 3DS material.

**Checks run:** read the listed docs; read scene/room C, XML, scene/actor/object tables, `z_actor.c`, `z_scene.c`, `z_play.c`, `z_scene_table.c`, `z64bgcheck.h`, `z64player.h` and the 21 overlays named above; scratch Python parsers (`col.py`, `actors.py`, `bands.py`, outputs in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref05\`) for collision counts, bands and the 33/32/8/1 entry decode; SHA-256 of the five inputs; grep cross-checks of neighbor exit lists. Cross-check: counts agree with `docs/city/LOCATION_REGISTER.md` row `loc.north_clock_town` (19/33, 19/32, 8/1 spawns, 8/8 exits, no transition command).

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes, entrance decoding beyond the labeled cross-check.

**Next dependency:** REF-02 portal register entries for the five North portals and spawns 5-7; REF-03 capture profile; PIPE-01 schema review of section 9; then the section 8 extraction and NCT-01 assembly.
