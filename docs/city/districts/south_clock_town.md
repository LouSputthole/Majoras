# South Clock Town (`loc.south_clock_town`): source inventory, collision and changed-input audit

Task **REF-04** (FULL_CITY_PLAN.md section 12). Base commit `98e554b3c1fc778894fd2c66de64ff040a90bb71` on `main`. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_CLOCKTOWER` C dated 2026-11-08 11:11. Written 2026-11-08 by source and project-file inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save). Aliases `scene.south_clock_town` and `sct.*` stay valid; room ID `loc.south_clock_town.room_00`.

This dossier extends [ROM_REFERENCE.md](../../ROM_REFERENCE.md) and does not repeat its tower, platform, door and landmark tables; it cites them. The template is [north_clock_town.md](north_clock_town.md). Portal IDs and exit decoding are owned by [PORTAL_REGISTER.md](../PORTAL_REGISTER.md) and are only cross-referenced here.

Paths are relative to `local-reference/mm-decomp/` unless they start with `docs/`, `tools/`, `Unity/`, `evidence/` or `local-reference/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_CLOCKTOWER/Z2_CLOCKTOWER.c`, `room.c` = `.../Z2_CLOCKTOWER_room_00.c`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in XML; **HDR** present in an extracted header or list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from overlay source; **PRJ** read from a project file (script, local export, evidence JSON, file timestamp or hash); **INF** inferred (reason given); **NI** not inspected.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum / index | `Z2_CLOCKTOWER` / `SCENE_CLOCKTOWER` / `0x6F` | `include/tables/scene_table.h:227` |
| Entrance-table scene index | `0x6C` (`ENTR_SCENE_SOUTH_CLOCK_TOWN`) | `src/code/z_scene_table.c:2681`, `include/z64scene.h:701` |
| Title card / draw config / restrictions | `0x104` / `SCENE_DRAW_CFG_MAT_ANIM` / `RESTRICTIONS_NONE` | scene_table.h:227 |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_SET((1 << 20), 0, 0, (1 << 10))`: switch0 bit 20, collectible bit 10 | scene_table.h:227. INF: bit 10 matches the heart piece's collectible flag `0x0A` (default actor 5). Consumer of switch bit 20: UNKNOWN (U-13) |
| XML | scene file segment 2; room `Z2_CLOCKTOWER_room_00` segment 3; two room DLs declared, `DL_011708` (calls the 6 opaque shape DLs) and `DL_013880` (calls the 4 XLU shape DLs) | XML `assets/xml/scenes/Z2_CLOCKTOWER/Z2_CLOCKTOWER.xml`; HDR room.c:3926-3934, :4655-4661 |
| Rooms / headers | 1 room; default + 3 alternate scene headers (`Set_005590`, `Set_005F40`, `Set_006920`); default + 3 alternate room headers (`Set_000420`, `Set_000530`, `Set_000610`) | HDR scene.c:14-39, room.c:14-29 |
| Room shape | cullable, 6 entries: 6 opaque DLs, 4 XLU DLs (`DL_013870` is empty) | HDR room.c:248-262 |
| Collision header | `Z2_CLOCKTOWERCollisionHeader_005560`, shared by all 4 scene headers | HDR scene.c:18, :2293, :2592, :2862 |
| Animated materials | 2 entries, both type 1 (`AnimatedMat_DrawTwoTexScroll`): segment 1 -> `0x08`, segment -2 (last) -> `0x09` | HDR scene.c:1310-1323; C `src/code/z_scene_proc.c:389-405` (`ABS(segment) + 7`) |
| Minimap / compass chests | 1 entry `{0x0105, 0xFEC0, 0, 0xFC40, 0}` scale 52; 2 chest icons (default), 1 (alt3) | HDR scene.c:1211-1222, :2874 |

### 1.1 Scene and room headers (HDR scene.c and room.c at the cited lines)

Selection rule is the same as North (`docs/city/districts/north_clock_town.md` section 1.1; `src/code/z_play.c:2210-2219`, `src/code/z_scene.c:465-474`): ordinary Day 1 play uses layer 0. South layer 2 with cutscene script 0 is the tower-opening setup (SOURCE_REGISTER; OVL `z_obj_tokeidai.c:121-124`, `z_obj_tokei_step.c:196-199`).

| Item | Default (L0) | Alt 1 (L1) | Alt 2 (L2) | Alt 3 (L3) |
| --- | --- | --- | --- | --- |
| Scene set (line) | `Commands` (13) | `Set_005590` (2289) | `Set_005F40` (2588) | `Set_006920` (2857) |
| Spawns / entrance rows | 11 / 12 (last `{0,0}`) | 2 / 2, both at (-1937, 199, -1272) | 1 / 2 at (-143, 0, 459) | 1 / 2 at (-250, 160, 222) |
| Exit list | 10 words | same 10 | none | same 10 |
| Transition actors | 1 (`EN_DOOR`) | same | same | same |
| Sound (spec, ambience, seq) | (1, 0, 29) | (0, 19, 118) | (0, 0, 0) | (11, 19, 118) |
| Skybox (file, skybox, weather, lightMode) | (0, 1, 10, false) | (0, 1, 7, true) | (0, 1, 7, false) | (0, 1, 7, false) |
| Env light settings | 29 | 4 | 20 | 29 |
| Cutscene scripts / actor cutscenes / cs cameras | 2 / 20 / 19 | 2 / 13 / 3 | 1 / 10 / none | 1 / 12 / 3 |
| Paths | 24 (`Pathway_000A8C`) | none | none | 20 (`Pathway_00721C`) |
| Room echo / behavior / `TIME_SETTINGS` | 4 / `(0,0,0,0,false,1)` / (255, 255, 3) | 0 / `(1,0,0,0,false,0)` / (2, 0, 1) | 0 / `(1,0,0,0,false,0)` / (255, 255, 0) | 4 / `(0,0,0,0,false,1)` / (6, 1, 3) |
| Room objects / actors | 23 / 50 | 7 / 12 | 5 / 9 | 19 / 29 |

INF on layer purposes (labels only, U-01): L1 holds `EN_MAG` (overlay description "Title Logo") and `EN_FALL` type 1 (`EN_FALL_TYPE_TITLE_SCREEN_MOON`, `ovl_En_Fall/z_en_fall.h:11`) plus `DM_STK`; L3's spawn (-250, 160, 222) lies on the Day 1 platform top floor (section 6.6). Default cutscene scripts: `CutsceneData_000B4C` with spawn `0x0A` and `CS_SPAWN_FLAG_ALWAYS`, `CutsceneData_0020EC` with spawn `0xFF` and `CS_SPAWN_FLAG_NONE` (HDR scene.c:1183-1186); semantics NI.

## 2. Spawns, exits and the transition actor

### 2.1 Start positions (HDR scene.c:41-64; decode C as North section 2.1)

| Spawn | Position | Yaw | Params | Start mode | bgCam idx | Arrival use (cross-check, PORTAL_REGISTER section 4) |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | (-278, 0, -801) | 0 | 0x0E11 | 0xE | 0x11 | from Clock Tower interior `0xD800` |
| 1 | (-279, 0, 627) | 180 | 0x0F04 | 0xF | 0x04 | teleport-only per PORTAL_REGISTER section 6 (Termina Field side, external NI) |
| 2 | (180, 100, -770) | 270 | 0x0F05 | 0xF | 0x05 | from East upper `0xD820` |
| 3 | (-862, 100, -1094) | 90 | 0x0F09 | 0xF | 0x09 | from West upper `0xD830` |
| 4 | (-260, 200, -1380) | 0 | 0x0E07 | 0xE | 0x07 | from North `0xD840` |
| 5 | (-780, 0, -100) | 90 | 0x0F08 | 0xF | 0x08 | from West lower `0xD850` |
| 6 | (-840, 100, 560) | 90 | 0x0F06 | 0xF | 0x06 | from Laundry Pool `0xD860` |
| 7 | (323, 0, 247) | 270 | 0x0F0A | 0xF | 0x0A | from East lower `0xD870` |
| 8 | (-275, 240, -751) | 0 | 0x0D0D | 0xD | 0x0D | teleport-only; on the Y240 tower-front ledge (poly 243, section 6.3) |
| 9 | (-614, 100, -1100) | 90 | 0x06FF | 0x6 (`OWL`) | none | owl warp, 74 units west of the owl statue (default actor 32) |
| 10 | (-278, 0, -801) | 0 | 0x0F12 | 0xF | 0x12 | default cutscene script 0 spawn `0x0A` (HDR scene.c:1184) |

bgCam indices 0x04 to 0x12 exceed the 3-entry `BgCamInfo` list (`0x0001` NORMAL0, `0x0000` NONE, `0x003D` DUNGEON3; HDR scene.c:1325-1329, `include/z64camera.h:147-148, 208`): U-02, same question as North U-02.

### 2.2 Exits (HDR scene.c:89-100)

Ten exit words: `0xC010`, `0x5460`, `0xD230`, `0xD420`, `0xD620`, `0xD410`, `0xDA00`, `0xD210`, `0x2C00`, `0xD650`. Decoding, destinations, reverse links and polygon ownership are in PORTAL_REGISTER section 3.3 and section 4 (rows `portal.south_clock_town.*`); the corrected polygon pairs are in CITY_PLAYBOOK section 3. Re-checked here from the surface list (section 6.2): every exit index 1 to 9 has the polygons PORTAL_REGISTER lists; index 10 (`0xD650`) has no surface and no South actor that reads `setupExitList[9]` was found (PORTAL U4).

### 2.3 Transition actor (HDR scene.c:66-68; identical in all 4 scene headers)

| TA | Actor | Position | Raw rotY / yaw | csId | Params | Sides | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_DOOR` (0x005) | (1114, 100, 274) | 28927 = 0x70FF / 225 | 0x7F none | 0x007F: type `(p >> 7) & 7 = 0` `ENDOOR_TYPE_WHOLE_DAY`, type var 0x7F | room 0 / room 0, bgCam 255 both | C `ovl_En_Door/z_en_door.h:44, 95-96`; spawn rule `src/code/z_actor.c:3600-3624`. Non-framed type: no door dynapoly (OVL `z_en_door.c:337-340`). New finding for PORTAL U8: the point lies over South floor poly 120 (Y 100, x 720..1200, z -200..360), which is east of the East-lower exit trigger polys 180/181 (x 347..720); a walker reaches it only after crossing that trigger (INF) |

## 3. Object banks (HDR room.c:31-55, :277-285, :337-343, :390-410; IDs from `include/tables/object_table.h`)

| # | Bank | ID | Default consumers (actor index) | Alt 1 | Alt 2 | Alt 3 |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | `OBJECT_F53_OBJ` | 0x16D | 8 | 9 | 4 | 6 |
| 1 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | 13-16, 18 | 2-5, 10 | 0-3, 7 | 9-12, 14 |
| 2 | `OBJECT_TOKEI_TOBIRA` | 0x197 | 19 | 7 | 5 | 15 |
| 3 | `OBJECT_TOKEI_STEP` | 0x1A4 | 17 | 8 | 6 | 13 |
| 4 | `OBJECT_MM` | 0x107 | 6 | | | |
| 5 | `OBJECT_DOG` | 0x132 | 11 | | | 7 |
| 6 | `OBJECT_DAIKU` | 0x0F1 | 33-39 | | | 19-21 |
| 7 | `OBJECT_SDN` | 0x1B6 | 12 | | | 8 |
| 8 | `OBJECT_PST` | 0x1CB | 46, 47 | | | 25, 26 |
| 9 | `OBJECT_TOKEI_TURRET` | 0x205 | 20-26 | | | 16, 17 |
| 10 | `OBJECT_DNT` | 0x1E5 | 0, 41, 42 | | | 0, 23 |
| 11 | `OBJECT_SEK` | 0x170 | 32 | | | 18 |
| 12 | `OBJECT_LODMOON` | 0x223 | none by profile; En_Fall type 6 loads it by slot (as North section 3, INF) | | | yes |
| 13 | `OBJECT_AN2` | 0x0E0 | none listed (NI) | | | |
| 14 | `OBJECT_TEST3` | 0x01C | 2 | | | 1 |
| 15 | `OBJECT_AN1` | 0x0E2 | 1 | | | |
| 16 | `OBJECT_OS_ANIME` | 0x09D | none listed (NI) | | | yes |
| 17 | `OBJECT_STH` | 0x26A | none by profile; `En_Test3` asks for its slot (OVL `z_en_test3.c:121-123`) | | | yes |
| 18 | `OBJECT_TORYO` | 0x0F0 | 7 | | | yes |
| 19 | `OBJECT_SYOKUDAI` | 0x080 | 30, 31 | | | |
| 20 | `OBJECT_GI_HEART` | 0x090 | none by profile (En_Item00 recovery-heart draw, as North) | | | yes |
| 21 | `OBJECT_BOX` | 0x00C | 43, 44 | | | 24 |
| 22 | `OBJECT_KANBAN` | 0x0FC | none listed (NI) | | | |

Alternate-only banks: alt1 `OBJECT_FALL` (0x188, no listed consumer), `OBJECT_STK` (0x192, Dm_Stk), `OBJECT_MAG` (0x115, En_Mag); alt2 `OBJECT_F52_OBJ` (0x16C, no listed consumer); alt3 `OBJECT_MAG`. Implicit `gameplay_keep` serves 17 default entries (flower, heart piece, controllers, sign, moon by profile).

## 4. Actor tables

### 4.1 Decode rules

Identical to North section 4.1 (`src/code/z_actor.c:3628-3655` spawn decode, `:2546-2575` half-day filter, `:2491-2499` half-day bit; Day 1 at 12:00 is bit `0x080`). ZAPD writes rotations as `SPAWN_ROT_FLAGS(value, flags)` = `(value << 7) | flags` (`include/z64scene.h:16`). Axis flags in South: `| 0xE000` (all three axes plain integers: En_Talk, Elf_Msg6, En_Hit_Tag), `| 0x6000` (rotX and rotZ integers: En_Box; `ENBOX_GET_SWITCH_FLAG` reads `world.rot.z`, `ovl_En_Box/z_en_box.h:42`), `| 0x2000` (rotZ integer: En_Stop_heishi). Integer axes above 180 become value - 360. No South ID carries bit `0x800`.

Columns: Cov = coverage by current tooling and docs: **IMP** assembled by `tools/reference/import_clocktown.py`; **DOC** placement and params documented in ROM_REFERENCE.md but not imported; **GEN** ROM_REFERENCE names it only generically ("Anju, Kafei-related/scheduled actors, the postman, guard, dog, Moon's Tear/deed-related actors and interaction controllers", or "skip Day2/3-only placements"); **NONE** neither. Classes as North section 4.1.

### 4.2 Default room header (50 entries, HDR room.c:57-242; decode C)

| # | Actor (ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask (half-days) | CS | Params | Bank | Class | D1d | Cov | Init / presence notes (OVL unless marked) |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `EN_AKINDONUTS` (0x274) | (-692, 0, -348) | 0x0007/0x2D7F/0x007F | 0/90/0 | 0x3FF all | none | 0xFC04 | object_dnt | npc-scheduled | yes | DOC | Trade-quest Business Scrub; bit 2 set, so the path check is skipped (`z_en_akindonuts.c:1704-1710`); cylinder type 1; skeletal; D1 noon pose NI (U-15) |
| 1 | `EN_AN` (0x202) | (889, 100, -20) | 0x0007/0xAC7F/0x007F | 0/344/0 | all | none | 0x8003 | object_an1 | npc-scheduled | yes | GEN | Anju; cylinder; anchor over poly 121 (Y 100) east of the East-lower trigger (INF); schedule NI (U-12) |
| 2 | `EN_TEST3` (0x159) | (-1043, 100, 466) | 0x0001/0x077F/0x0040 | 0/14/0 | 0x0C0 D1d, D1n | none | 0x01E4 | object_test3 | npc-scheduled | yes | GEN | Kafei; profile draw NULL; schedule NI |
| 3 | `EN_STH` (0x20B) | (-343, 160, 194) | 0x0001/0x5A7F/0x0040 | 0/180/0 | 0x0C0 D1d, D1n | none | 0xFE03 | gameplay_keep, then waits for a slot (`EnSth_UpdateWaitForObject`) | npc-scheduled | yes | NONE | "Guy looking at moon in South Clock Town"; cylinder; anchor height equals the Day 1 platform top floor Y 160 (INF, section 6.6) |
| 4 | `OBJ_ETCETERA` (0x183) | (-692, 0, -348) | 0x0007/0x2D7F/0x007F | 0/90/0 | all | none | 0x017F | gameplay_keep | static-mesh | yes | DOC | gold flower type 2; dynapoly `gGoldDekuFlowerCol` + cylinder r20 h14 (`z_obj_etcetera.c:244-264`, :50) |
| 5 | `EN_ITEM00` (0x00E) | (-279, 240, -755) | 0x0007/0x007F/0x007F | 0/0/0 | all | none | 0x0A06 | gameplay_keep | static-mesh (state pickup) | yes | NONE | heart piece, collectible flag 0x0A (decode as North section 4.4); pickup cylinder; on the Y240 ledge poly 243; fresh-save only |
| 6 | `EN_PM` (0x1D5) | (-1580, 200, -1325) | 0x0007/0x2D7F/0x007F | 0/90/0 | all | none | 0x0002 | object_mm | npc-scheduled | yes | GEN | Postman; cylinder + sphere; anchor over poly 97 (Y about 200) west of the West-upper trigger (INF); schedule NI |
| 7 | `EN_MUTO` (0x26B) | (-289, 0, 38) | 0x0000/0x5A7F/0x0004 | 0/180/0 | 0x004 D3n | none | 0x0000 | object_toryo | npc-scheduled | no | NONE | Mutoh, Day 3 night only |
| 8 | `OBJ_ENTOTU` (0x14D) | (319, 553, -1308) | 0x0007/0x007F/0x007F | 0/0/0 | all | none | 0x0000 | object_f53_obj | billboard/effect | yes | DOC | chimney + smoke (ROM_REFERENCE) |
| 9 | `EN_TALK \| 0xE000` (0x261) | (195, 66, 64) | 0x0387/0x877F/0x007F | 7/-90/0 (ints) | all | none | 0xFE06 | gameplay_keep | controller/invisible | yes | GEN | "Green Target Spot"; profile draw NULL |
| 10 | `EN_TALK \| 0xE000` | (-457, 42, -173) | 0x0387/0x2D7F/0x007F | 7/90/0 (ints) | all | none | 0xFE18 | gameplay_keep | controller/invisible | yes | GEN | as 9 |
| 11 | `EN_DG` (0x0E2) | (-254, 0, -262) | 0x0007/0x8EFF/0x005A | 0/285/0 | 0x3DA (not D2d, D3n, D4n) | none | 0x03FF | object_dog | npc-scheduled | yes | GEN | dog; cylinder; scale 0.0075 |
| 12 | `EN_STOP_HEISHI \| 0x2000` (0x1C7) | (-280, 0, 680) | 0x0007/0x5A7F/0x007F | 0/180/0 (Z int) | all | none | 0x007F | object_sdn | npc-scheduled | yes | GEN | "Gate-Blocking Soldier"; cylinder r50 h260 (`z_en_stop_heishi.c:52`); stands on the south edge (z 680) of Termina Field exit polys 176/177; passage rule NI (U-10) |
| 13 | `OBJ_TOKEIDAI` (0x19C) | (-280, 555, -833) | 0x0007/0x000C/0x007F | 0/0/0 | all | 12 | 0x207F | object_obj_tokeidai | static-mesh | yes | IMP | tower clock (ROM_REFERENCE) |
| 14 | `OBJ_TOKEIDAI` | (-492, 555, -1012) | 0x0007/0x877F/0x007F | 0/270/0 | all | none | 0x007F | object_obj_tokeidai | static-mesh | yes | IMP | west gear |
| 15 | `OBJ_TOKEIDAI` | (-68, 555, -1012) | 0x0007/0x2D7F/0x007F | 0/90/0 | all | none | 0x007F | object_obj_tokeidai | static-mesh | yes | IMP | east gear |
| 16 | `OBJ_TOKEIDAI` | (-280, 1103, -1020) | 0x0007/0x007F/0x007F | 0/0/0 | all | none | 0x307F | object_obj_tokeidai | static-mesh | yes | IMP | counterweight |
| 17 | `OBJ_TOKEI_STEP` (0x1B8) | (-280, 255, -853) | 0x0007/0x007F/0x007F | 0/0/0 | all | none | 0x0000 | object_tokei_step | static-mesh | yes | IMP | closed panel; on Day 1 loads dynapoly `gClocktowerPanelCol` (`z_obj_tokei_step.c:204-206`) |
| 18 | `OBJ_TOKEIDAI` | (-280, 255, -853) | 0x0007/0x007F/0x007F | 0/0/0 | all | none | 0xB000 | object_obj_tokeidai | static-mesh | yes (draw NULL) | DOC | rooftop staircase type 11; draw only if Day 3 before 06:00, day >= 4 or `WEEKEVENTREG_CLOCK_TOWER_OPENED` (`z_obj_tokeidai.c:615-622`); no collision in this overlay |
| 19 | `OBJ_TOKEI_TOBIRA` (0x1A2) | (-280, 0, -807) | 0x0007/0x0010/0x007F | 0/0/0 | all | 16 | 0x0000 | object_tokei_tobira | door/transition | yes | IMP | swinging doors; spawns the type-1 leaf; dynapoly per leaf; first-entry cutscene if `WEEKEVENTREG_59_04` clear on layer 0 (`z_obj_tokei_tobira.c:55-84`) |
| 20 | `OBJ_TOKEI_TURRET` (0x221) | (-290, 0, 160) | 0x0000/0x007F/0x000C | 0/0/0 | 0x00C D3d, D3n | none | 0x0002 | object_tokei_turret | static-mesh | no | DOC | flags tier |
| 21 | `OBJ_TOKEI_TURRET` | (-290, 80, 160) | 0x0001/0x007F/0x0040 | 0/0/0 | 0x0C0 D1d, D1n | none | 0x0001 | object_tokei_turret | static-mesh | yes | IMP | Day 1 top; dynapoly `gClockTownTurretPlatformCol` (`z_obj_tokei_turret.c:43-50`) |
| 22 | `OBJ_TOKEI_TURRET` | (-290, 240, 160) | 0x0000/0x007F/0x000C | 0/0/0 | 0x00C D3 | none | 0x0001 | object_tokei_turret | static-mesh | no | DOC | Day 3 top |
| 23 | `OBJ_TOKEI_TURRET` | (-290, 160, 160) | 0x0000/0x007F/0x0030 | 0/0/0 | 0x030 D2 | none | 0x0001 | object_tokei_turret | static-mesh | no | DOC | Day 2 top |
| 24 | `OBJ_TOKEI_TURRET` | (-290, 0, 160) | 0x0001/0x007F/0x007C | 0/0/0 | 0x0FC D1d to D3n | none | 0x0000 | object_tokei_turret | static-mesh | yes | IMP | base; dynapoly `gClockTownTurretBaseCol` |
| 25 | `OBJ_TOKEI_TURRET` | (-290, 80, 160) | 0x0000/0x007F/0x003C | 0/0/0 | 0x03C D2d to D3n | none | 0x0000 | object_tokei_turret | static-mesh | no | DOC | second base |
| 26 | `OBJ_TOKEI_TURRET` | (-290, 160, 160) | 0x0000/0x007F/0x000C | 0/0/0 | 0x00C D3 | none | 0x0000 | object_tokei_turret | static-mesh | no | DOC | third base |
| 27 | `ELF_MSG6 \| 0xE000` (0x232) | (-280, 240, -858) | 0x0F07/0x007F/0x007F | 30/0/0 (ints) | all | none | 0x0002 | gameplay_keep | controller/invisible | yes | GEN | Tatl hint; draw NULL |
| 28 | `ELF_MSG6 \| 0xE000` | (-278, 0, -797) | 0x0007/0x0013/0x007F | 0/0/0 (ints) | all | 19 | 0x0000 | gameplay_keep | controller/invisible | yes | GEN | Tatl hint at the doors |
| 29 | `EN_HIT_TAG \| 0xE000` (0x265) | (-279, 490, -756) | 0x0007/0x007F/0x007F | 0/0/0 (ints) | all | none | 0xFE00 | gameplay_keep | controller/invisible | yes | GEN | "Invisible hitbox that can spawn rupees"; cylinder |
| 30 | `OBJ_SYOKUDAI` (0x039) | (-447, 0, 67) | 0x0002/0x007F/0x0055 | 0/0/0 | 0x155 all nights | none | 0x287F | object_syokudai | static-mesh | no | DOC | torch; stand + flame cylinders |
| 31 | `OBJ_SYOKUDAI` | (-131, 0, 70) | 0x0002/0x007F/0x0055 | 0/0/0 | 0x155 all nights | none | 0x287F | object_syokudai | static-mesh | no | DOC | torch |
| 32 | `OBJ_WARPSTONE` (0x223) | (-540, 100, -1099) | 0x0007/0x870E/0x007F | 0/270/0 | all | 14 | 0x0004 | object_sek | static-mesh | yes | DOC | owl warp id `p & 0xF = 4`; cylinder r20 h60, hookable; opened model only if activated (`z_obj_warpstone.c:35-77`) |
| 33 | `EN_DAIKU` (0x09C) | (105, 0, 39) | 0x0007/0x2D7F/0x0078 | 0/90/0 | 0x3F8 D0d to D3d | none | 0x0001 | object_daiku | npc-scheduled | yes | DOC | carpenter; cylinder; killed on Day 3 night (`z_en_daiku.c:110-121`) |
| 34 | `EN_DAIKU` | (-686, 100, 555) | 0x0000/0x46FF/0x0008 | 0/141/0 | 0x008 D3d | none | 0x0002 | object_daiku | npc-scheduled | no | GEN | |
| 35 | `EN_DAIKU` | (-666, 20, 224) | 0x0000/0x2D7F/0x0030 | 0/90/0 | 0x030 D2 | none | 0x0002 | object_daiku | npc-scheduled | no | GEN | |
| 36 | `EN_DAIKU` | (-440, 0, 207) | 0x0001/0x2D7F/0x0040 | 0/90/0 | 0x0C0 D1 | none | 0x0002 | object_daiku | npc-scheduled | yes | DOC | |
| 37 | `EN_DAIKU` | (-277, 0, 384) | 0x0001/0x877F/0x0040 | 0/270/0 | 0x0C0 D1 | none | 0x0503 | object_daiku | npc-scheduled | yes | DOC | FF 3, path index 5 |
| 38 | `EN_DAIKU` | (-254, 200, 185) | 0x0000/0x007F/0x0030 | 0/0/0 | 0x030 D2 | none | 0x0603 | object_daiku | npc-scheduled | no | GEN | path index 6 |
| 39 | `EN_DAIKU` | (-253, 300, 185) | 0x0000/0x007F/0x0008 | 0/0/0 | 0x008 D3d | none | 0x0603 | object_daiku | npc-scheduled | no | GEN | |
| 40 | `EN_TEST4` (0x15A) | (-280, 1103, -1020) | 0x0007/0x0009/0x007F | 0/0/0 | all | 9 | 0x0C00 | gameplay_keep | controller/invisible | yes | GEN | three-day controller (SOURCE_REGISTER) |
| 41 | `EN_SELLNUTS` (0x1BD) | (-692, 0, -348) | 0x0007/0x2D0B/0x007F | 0/90/0 | all | 11 | 0x07E2 | object_dnt | npc-scheduled | yes | GEN | "Business Scrub Carrying Bags"; killed if land title deed received or `61_10`; fresh save: init sets draw NULL (`z_en_sellnuts.c:990-1058`), later visibility NI (U-14) |
| 42 | `EN_SELLNUTS` | (-279, 301, -796) | 0x0007/0x0011/0x007F | 0/0/0 | all | 17 | 0x1FFF | object_dnt | npc-scheduled | yes | GEN | params bit 0 set: other init branch, also draw NULL at init |
| 43 | `EN_BOX \| 0x6000` (0x006) | (249, 374, 565) | 0x0007/0x5A7F/0x007F | 0/180/0 (X, Z ints) | all | none | 0x5080 | object_box | posed-skeletal | yes | DOC | chest type 5, item 0x04, chest flag 0 (`z_en_box.h:39-41`); dynapoly `gBoxChestCol` + `gBoxChestSkel`; no room floor at Y 374 here (highest below: Y 220, poly 125; U-09) |
| 44 | `EN_BOX \| 0x6000` | (-290, 573, 160) | 0x0000/0x007F/0x000C | 0/0/0 (X, Z ints) | 0x00C D3 | none | 0x50A1 | object_box | posed-skeletal | no | DOC | Day 3 only; item 0x05, flag 1 |
| 45 | `EN_A_OBJ` (0x026) | (-39, 100, -821) | 0x0007/0x877F/0x007F | 0/270/0 | all | none | 0x360A | gameplay_keep | static-mesh | yes | NONE | directional signpost: type `(p & 0xFF) - 9 = 1`, textId `0x336`, `gSignDirectionalDL`, cylinder r25 h60 (`src/code/z_en_a_keep.c:54-104`, `include/z_en_a_keep.h:19-24`) |
| 46 | `EN_PST` (0x1F2) | (176, 100, -945) | 0x0007/0x877F/0x007F | 0/270/0 | all | none | 0x0000 | object_pst | posed-skeletal | yes | DOC | postbox; cylinder r28 h72; scale 0.02 |
| 47 | `EN_PST` | (-616, 0, 321) | 0x0007/0x2D7F/0x007F | 0/90/0 | all | none | 0x0004 | object_pst | posed-skeletal | yes | DOC | postbox |
| 48 | `EN_FALL` (0x17C) | (-280, 11000, -1020) | 0x1907/0x07FF/0x007F | 50/15/0 | all | none | 0x037F | gameplay_keep (LOD moon by slot) | static-mesh (camera-relative) | yes | DOC | type 6 LOD moon |
| 49 | `OBJ_SOUND` (0x0F0) | (-290, 0, 160) | 0x0001/0x007F/0x0078 | 0/0/0 | 0x0F8 D1d to D3d | none | 0x0090 | gameplay_keep | controller/invisible | yes | GEN | invisible point sound source at the platform base |

### 4.3 Alternate room header 1 (12 entries, HDR room.c:287-324)

All masks `0x3FF`, no cutscene ids. "Twin" = default entry with the same actor and position.

| # | Actor | Position | Rot X / Y / Z | Params | Twin / note |
| --- | --- | --- | --- | --- | --- |
| 0 | `DM_STK` | (-517, 0, -655) | 0/0/0 | 0x0000 | none; Skull Kid |
| 1 | `EN_MAG` | (-992, 600, -1464) | 0/0/0 | 0x0000 | none; "Title Logo" |
| 2 | `OBJ_TOKEIDAI` | (-280, 555, -833) | 0/0/0 | 0x207F | 13 (no cs) |
| 3 | `OBJ_TOKEIDAI` | (-68, 555, -1012) | 0/90/0 | 0x007F | 15 |
| 4 | `OBJ_TOKEIDAI` | (-492, 555, -1012) | 0/270/0 | 0x007F | 14 |
| 5 | `OBJ_TOKEIDAI` | (-280, 1103, -1020) | 0/180/0 | 0x307F | 16, yaw 180 instead of 0 |
| 6 | `EN_FALL` | (-1477, 2274, -413) | 20/123/20 | 0x00FF | none; type 1 `TITLE_SCREEN_MOON` |
| 7 | `OBJ_TOKEI_TOBIRA` | (-280, 0, -807) | 0/0/0 | 0x0000 | 19 (no cs) |
| 8 | `OBJ_TOKEI_STEP` | (-280, 255, -853) | 0/0/0 | 0x0000 | 17 |
| 9 | `OBJ_ENTOTU` | (319, 553, -1308) | 0/0/0 | 0x0000 | 8 |
| 10 | `OBJ_TOKEIDAI` | (-280, 255, -853) | 0/0/0 | 0xB000 | 18 |
| 11 | `EN_WEATHER_TAG` | (-300, 0, -126) | 0/0/0 | 0x0A04 | none; "Local weather changes" |

### 4.4 Alternate room header 2 (9 entries, HDR room.c:345-373; tower-opening layer)

All masks `0x3FF`, no cutscene ids: 0 clock `0x207F`, 1 east gear, 2 west gear, 3 counterweight (yaw 0), 4 `OBJ_ENTOTU`, 5 `OBJ_TOKEI_TOBIRA`, 6 `OBJ_TOKEI_STEP`, 7 staircase `0xB000`, 8 `EN_WEATHER_TAG` (-269, 0, -256) `0x0A04`. Positions equal the default twins.

### 4.5 Alternate room header 3 (29 entries, HDR room.c:412-519)

28 entries are default twins with identical position, mask and params unless noted; 1 is new.

| Alt 3 # | Default twin | Difference |
| --- | --- | --- |
| 0, 2, 4-14, 16-18, 20, 22, 25-28 | 0, 3, 4, 5, 8, 11, 12, 13, 14, 15, 16, 17, 18, 21, 24, 32, 36, 40, 46, 47, 48, 49 | owl (18) has no cs (default 14) |
| 1 | 2 `EN_TEST3` | params 0x01E2 (default 0x01E4) |
| 3 | none | `EN_MAG` at (0, 0, 0), params 0x0000 |
| 15 | 19 `OBJ_TOKEI_TOBIRA` | no cs (default 16) |
| 19 | 33 `EN_DAIKU` | mask 0x3FF all (default 0x3F8) |
| 21 | 37 `EN_DAIKU` | params 0x0303, path index 3 (default 0x0503) |
| 23 | 41 `EN_SELLNUTS` | no cs (default 11) |
| 24 | 43 `EN_BOX` | position (270, 374, 563), 21 units +X and 2 units +Z |
| absent | 1, 6, 7, 9, 10, 20, 22, 23, 25, 26, 27, 28, 29, 30, 31, 34, 35, 38, 39, 42, 44, 45 | 22 default entries not in alt 3 |

### 4.6 Counts and coverage

| | Default | Alt 1 | Alt 2 | Alt 3 |
| --- | --- | --- | --- | --- |
| Entries | 50 | 12 | 9 | 29 |
| Day 1 daytime candidates by mask | 37 (excluded 7, 20, 22, 23, 25, 26, 30, 31, 34, 35, 38, 39, 44) | 12 | 9 | 29 |
| static-mesh | 20 | 7 | 6 | 12 |
| posed-skeletal | 4 | 0 | 0 | 3 |
| npc-scheduled | 17 | 1 (Dm_Stk) | 0 | 9 |
| billboard/effect | 1 | 3 (chimney, title logo, weather) | 2 | 2 |
| controller/invisible | 7 | 0 | 0 | 2 |
| door/transition | 1 (+ TA#0) | 1 | 1 | 1 |
| Imported by `import_clocktown.py` | 8 (13, 14, 15, 16, 17, 19 with its spawned leaf, 21, 24) | 0 | 0 | 0 |
| Documented, not imported (DOC) | 20 (12 of them D1d: 0, 4, 8, 18, 32, 33, 36, 37, 43, 46, 47, 48) | | | |
| Generic mention only (GEN) | 18 (14 D1d) | | | |
| Neither (NONE) | 4: 3 En_Sth, 5 heart piece, 7 Mutoh, 45 signpost (3 D1d) | all 12 not imported | all 9 not imported | all 29 not imported |

The importer disables alternate room and scene headers (`room_header.parseAlternateRoomHeaders`, `scene_header.parseAlternateSceneHeaders` set to no-ops, `tools/reference/import_clocktown.py:59-61`, PRJ) and disables actors, cutscenes, lights, cameras, paths, water boxes and animated materials (`:46-49`). Its 12 actor meshes are hand-placed calls (`:82-92`), not a decode of the actor list.

## 5. Day 1 noon assembly status (`prof.d1.noon.normal`, default headers)

Already assembled (IMP, per ROM_REFERENCE tables): clock 4 DLs (sun/moon panel placed at (-280, 443.8, -834.96), PRJ `import_clocktown.py:84`), two gears, counterweight (yaw 0 phase), closed panel, two door leaves at their shifted pivots, Day 1 platform base and top.

Static additions still missing at Day 1 noon (DOC/NONE, D1d, non-skeletal), with source DL where known:

| Default # | Component | Source DL (bank) | Status |
| --- | --- | --- | --- |
| 4 | gold Deku flower, yaw 90, scale (0.01, 0.02, 0.01) | `gGoldDekuFlowerIdleDL` 0x11BD0 (gameplay_keep) | not extracted |
| 32 | owl statue, closed | `gOwlStatueClosedDL` 0x1D0 (object_sek) | not extracted |
| 45 | directional signpost, yaw 270 | `gSignDirectionalDL` (gameplay_keep, offset NI) | not extracted |
| 5 | heart piece (optional, fresh save) | `gHeartPieceInteriorDL` (gameplay_keep 0x5AAB0, as North) | not extracted |
| 48 | LOD moon (special 9000-unit rule) | `gLodmoonEyesDL` 0x10E0 + `gLodmoonMoonDL` 0x1158 | not extracted |
| 8 | chimney billboard + smoke | object_f53_obj 0x158, 0x1C00 | billboard approximation only |

Skeletal or state-driven and present by mask at noon (reconstruction later): 0 Business Scrub, 43 chest (closed pose), 46/47 postboxes, 1 Anju, 2 Kafei, 3 En_Sth, 6 Postman, 11 dog, 12 gate soldier, 33/36/37 carpenters; 41/42 Sellnuts are draw NULL at init on a fresh save. No geometry: 9, 10, 27, 28, 29, 40, 49 and hidden 18.

## 6. Collision

### 6.1 Room collision summary (C from scene.c:1325-2284; scratch parser output)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 416 / 508 / 21 |
| Water boxes | 2: `{-760, -20, -1000, 127, 600, 0x101}` and `{-200, -20, -600, 200, 200, 0x101}`; their floors are polys 190/191 and 194/195 at Y -40 (20 units of water) |
| Camera data | 3: `0x0001` NORMAL0, `0x0000` NONE, `0x003D` DUNGEON3 |
| Header bounds | min (-2360, -424, -1974), max (1600, 1183, 1200) |
| Vertex extents | x -2040..1200, y -424..995, z -1600..1000 (Unity x -20.40..12.00, y -4.24..9.95, z -10.00..16.00) |
| Polygon kinds | 146 floors (normal Y > 0.5; 93 under 5 degrees, 53 steeper), 335 walls (275 with abs(normal Y) < 0.1), 27 ceilings |
| Floor slope histogram (5-degree bins) | 0: 93, 10: 6, 15: 4, 20: 4, 25: 5, 30: 6, 35: 2, 40: 4, 45: 16, 50: 2, 55: 4 |
| Materials (data[1] & 0xF) | stone 320 polys, wood 182 (types 6, 17), dirt 6 (types 8, 9, 20) |
| Exit-carrying polygons | 11 surface types, 38 polygons (28 floors with vIB conveyor flag `COLPOLY_IS_FLOOR_CONVEYOR`, plus 10 rooftop risers without it); list in 6.2 |
| Poly exclusion (xpFlags) | ignore-camera: 18 on type 0 + 57 on type 6; ignore-projectiles: 4 on type 0 + 2 on type 6; both: 4 on type 18 |
| vIB flag 4 (`COLPOLY_VIB_FLAG_4`) | 6 walls (types 18, 20) |
| Absent flags | no floor types (bits 13-17 all 0), no hookshot bit, no horse-block, no wall types other than 1, no ladder wall type |

### 6.2 Surface types (C; data[0] / data[1] decoded with `include/z64bgcheck.h:78-97`)

| Type | data[0] / data[1] | Decoded (nonzero fields) | Material | Polys | Use |
| --- | --- | --- | --- | --- | --- |
| 0 | 0x00000000 / 0x000017C2 | echo 2 | stone | 256 (41 floors, 205 walls, 10 ceilings) | general masonry |
| 1 | 0x00000100 / 0x040C17C2 | exit+1 1; conveyor speed 3 dir 32 | stone | 2 floors (8, 9) | exit 0 `0xC010`, tower interior, Y 0, behind the door leaves |
| 2 | 0x00000500 / 0x040C17C2 | exit+1 5; speed 3 dir 32 | stone | 2 floors (18, 19) | exit 4 `0xD620`, North, Y 200 |
| 3 | 0x00000300 / 0x020C17C2 | exit+1 3; speed 3 dir 16 | stone | 2 floors (24, 25) | exit 2 `0xD230`, East upper, Y 100 |
| 4 | 0x00000900 / 0x040C17C2 | exit+1 9; speed 3 dir 32 | stone | 10 floors (46, 47, 50, 51, 54, 55, 58, 59, 62, 63) | exit 8 `0x2C00`, rooftop treads Y 300..360 |
| 5 | 0x00000900 / 0x000017C2 | exit+1 9 | stone | 10 walls (48, 49, 52, 53, 56, 57, 60, 61, 374, 375) | rooftop risers Y 300..375 |
| 6 | 0x00000000 / 0x000017CA | echo 2 | wood | 180 (62 floors, 101 walls, 17 ceilings) | timber: ramps, landings, stall canopies |
| 7 | 0x00000002 / 0x000017C2 | bgCam 2 (DUNGEON3) | stone | 2 floors (125, 126) | Y 220 ledge, x 200..1000, z 360..600 |
| 8 | 0x00000700 / 0x060C57C0 | exit+1 7; echo 10; speed 3 dir 48 | dirt | 2 floors (158, 159) | exit 6 `0xDA00`, Laundry Pool, Y 100 |
| 9 | 0x00000000 / 0x000017C0 | echo 2 | dirt | 2 floors | Y 100 next to the Laundry exit |
| 10 | 0x00000600 / 0x060C17C2 | exit+1 6; speed 3 dir 48 | stone | 2 floors (165, 166) | exit 5 `0xD410`, West lower, Y 0 |
| 11 | 0x00000200 / 0x000C57C2 | exit+1 2; echo 10; speed 3 dir 0 | stone | 2 floors (176, 177) | exit 1 `0x5460`, Termina Field, Y 0 |
| 12 | 0x00000800 / 0x020C17C2 | exit+1 8; speed 3 dir 16 | stone | 2 floors (180, 181), 15-degree ramp Y 0..100 | exit 7 `0xD210`, East lower |
| 13 | 0x00000400 / 0x060C17C2 | exit+1 4; speed 3 dir 48 | stone | 2 floors (184, 185), 27-degree ramp Y 100..200 | exit 3 `0xD420`, West upper |
| 14 | 0x00000001 / 0x000017C2 | bgCam 1 | stone | 1 wall | x -240..-200, y 220..240 |
| 15 | 0x00200000 / 0x000017C2 | wall type 1 | stone | 12 walls | tower base, x -513..-47, y 0..200, z -1253..-787 |
| 16 | 0x24000000 / 0x000017D2 | floor property 9; floor effect 1 | stone | 10 floors (46.6-degree skirt) | tower base skirt, Y 200..255 |
| 17 | 0x00000900 / 0x040C17CA | exit+1 9; speed 3 dir 32 | wood | 2 floors (368, 369) | rooftop top tread Y 375, z -1053..-993 |
| 18 | 0x00200001 / 0x000017C2 | bgCam 1; wall type 1 | stone | 4 walls (xp 5, vIB 4) | x -207..-127, y 200..250 |
| 19 | 0x40000000 / 0x000017C2 | soft | stone | 1 floor (505) | Y 101, x -1220..-800, z -1420..-980 |
| 20 | 0x00000001 / 0x000007C0 | bgCam 1; echo 0 | dirt | 2 walls (vIB 4) | x -859, y 180..540, z 460..620 |

Flag semantics read from source (OVL/C): wall type 1 maps to `WALL_FLAG_0` (`src/code/z_bgcheck.c:20`), and the player only starts a ledge climb when `!SurfaceType_CheckWallFlag0` (`ovl_player_actor/z_player.c:11835`): the tower base walls (types 15, 18) refuse ledge climbs. Floor property 9 suppresses the leave-floor auto-jump and the ledge-hang landing (`z_player.c:7539`, `:7569`). Soft floors lower the floor raycast by 1 unit (`z_bgcheck.c:1733-1735`). Conveyor behavior on exit floors: NI (U-13 of North applies).

### 6.3 Floor elevation levels (C; flat = slope under 5 degrees; extents are band unions)

| Level | Y | Flat floor polys | x / z extents | Notes |
| --- | --- | --- | --- | --- |
| water-channel beds | -40 | 4 (190, 191, 194, 195) | x -760..-633, z -1000..-400; x -200..0, z -600..-400 | under the two water boxes |
| ground | 0 | 22 | x -1000..347, z -967..1000 | square; includes exits 8/9, 165/166, 176/177 |
| low steps and counters | 16, 40 | 2 + 6 | Y 40 blocks 40 x 140 at x -504..-464 and -131..-91 (z -274..-123), x -62..78 (z 472..512) | INF: stall counters under the canopies; ignore-camera |
| terraces | 100 (+ soft 101) | 22 + 1 | x -1120..1200, z -1420..600 | East upper exit 24/25, Laundry exit 158/159, owl, postbox 46, sign 45, Anju strip 120/121 |
| tower-front landing | 180 | 2 (287, 288) | x -400..-160, z -787..-728 | reached by ramp 257/258 (section 6.4) |
| north upper and west strip | 198-200 | 2 + 8 | x -393..200, z -1600..-1253 (North exit 18/19); x -1400..-1200 and x -2040..-1400 beyond the West-upper trigger | upper level of the tested route |
| south-east ledge | 220 | 2 (125, 126) | x 200..1000, z 360..600 | bgCam 2; chest 43 is 154 units above it |
| tower-front ledge | 240 | 4 (243, 244, 271, 272) | x -360..-200, z -826..-680 | heart piece, spawn 8; overlaps the Y 180 landing in plan |
| stair, no exit | 255, 270, 285 | 6 (355, 356, 38, 39, 42, 43) | x -340..-220, z -893..-839 | 15-unit treads behind the closed panel |
| tower top (rooftop exit) | 300-375 | 12 (types 4, 17) | x -340..-220, z -1053..-893 | 22-polygon exit, 6.5 |
| sloped floors | 0-320 | 53 | | 16 ramp polys, 12 tower skirt polys (46.6 degrees), 23 stall-canopy polys (433-441, 452-461, 472-481), 2 steep polys 117/118 (55.7 degrees, Y 260..320) |

### 6.4 Ramps between levels (C; rise direction from the floor normal)

| Ramp polys | Type | Y | x / z | Rises toward | Connects |
| --- | --- | --- | --- | --- | --- |
| 251, 252 | 6 | 0..100 | x -633..-513, z -1000..-787 (25 degrees) | -Z | square to west terrace; first ramp of the tested route |
| 246, 247 | 6 | 100..200 | x -640..-393, z -1400..-1253 (22 degrees) | +X | west terrace to north upper; second ramp of the tested route |
| 259, 260 | 6 | 0..100 | x -200..-47, z -747..-600 (33 degrees) | +X | square to east terrace |
| 268, 269 | 6 | 100..200 | x -47..53, z -1253..-1027 (24 degrees) | -Z | east terrace to north upper |
| 257, 258 | 6 | 100..180 | x -160..-47, z -787..-747 (35 degrees) | -X | east terrace to the Y 180 landing |
| 178, 179 | 0 | 0..100 | x -806..-640, z 160..480 (17 degrees) | +Z | square to the Laundry corridor terrace |
| 180, 181 | 12 | 0..100 | x 347..720 (15 degrees) | +X | East-lower exit trigger itself |
| 184, 185 | 13 | 100..200 | x -1200..-1000 (27 degrees) | -X | West-upper exit trigger itself |

Endpoint heights match the flat levels exactly (C). Walkability of the 33- and 35-degree ramps and the 46.6-degree skirt for the player: NI (U-08).

### 6.5 Rooftop exit (exit index 8, `0x2C00`)

22 polygons (PORTAL_REGISTER): 10 floors of type 4 (treads Y 300, 315, 330, 345, 360), 10 walls of type 5 (risers Y 300..375), 2 floors of type 17 (top tread Y 375, z -1053..-993). Y range 300..375, x -340..-220, z -1053..-893. The three lower treads (Y 255, 270, 285) are type 0 and carry no exit. The closed panel `gClocktowerPanelCol` is one vertical plane 120 x 120 at (-280, 255, -853), covering x -340..-220, Y 255..375 at the first tread's front edge z -853 (C: object collision extents x 0.1 scale; INF that it closes the stair).

### 6.6 Room collision versus actor collision (OVL; object collision extents C from `extracted/n64-us/assets/objects/<bank>/<bank>.c`)

Current import (PRJ): Blender keeps the 508 room triangles as `Source_Collision` without surface semantics (`import_clocktown.py:93-103`); every actor mesh is marked `ignore_collision` (`:78`) and exported with `collision: false`; the Unity reference scene gives only `Source_Collision` a non-convex `MeshCollider` (`Unity/Assets/Editor/ImportRomReference.cs:141-145`). The enhanced preview adds a `MeshCollider` to renderers whose name starts with `object_tokei_step` or `object_tokei_tobira` (`Unity/Assets/Editor/BuildCityMvp.cs:102-104`); the panel's GameObject is named `gClocktowerPanelDL` (mesh name in `reference.json`), so only the two door leaves get one (INF from names).

| Owner (default #) | Source collision | Day 1 noon | Unity reference scene | Enhanced preview | Gap |
| --- | --- | --- | --- | --- | --- |
| Room `CollisionHeader_005560` | 508 tris, 21 surface types | yes | MeshCollider, no metadata | same + 8 preview boxes | exits, conveyor, floor property 9, wall type 1, soft, bgCam, material, echo all unimplemented |
| Water boxes (2) | surface Y -20 over Y -40 floors | yes | not imported (`includeWaterBoxes` false) | none | water volume and swim/wade behavior |
| 19 door leaves | 2 dynapolys 80 x 140 x 20, pivots (-360, 0, -807) and (-200, 0, -807); pushed open by the player (`z_obj_tokei_tobira.c:110-160`) | yes | render only | static MeshCollider from the closed render mesh | push-swing; interior exit polys 8/9 lie behind the leaves, so the preview box is unreachable on foot (INF) |
| 17 closed panel | dynapoly vertical 120 x 120 | yes | render only | none (name filter miss) | stair closure missing in the preview |
| 21 + 24 Day 1 platform | base: 8 wall polys, 136 x 136 x 80; top: 8 walls + floor at world Y 160 (x -358..-222, z 92..228) | yes | render only | none | the preview platform has no collision: walk-through base, no top floor (INF) |
| 4 gold flower | dynapoly `gGoldDekuFlowerCol` + cylinder r20 h14 | yes | not imported | none | mesh and collision |
| 43 chest | dynapoly `gBoxChestCol` | yes | not imported | none | mesh and collision |
| 32 owl statue | cylinder r20 h60, OC solid, hookable | yes | not imported | none | |
| 46, 47 postboxes | cylinder r28 h72 | yes | not imported | none | |
| 45 signpost | cylinder r25 h60 | yes | not imported | none | |
| 12 gate soldier | cylinder r50 h260 at the Termina Field exit | yes | not imported | none | blocking rule NI |
| 0, 1, 2, 3, 6, 11, 33, 36, 37, 41, 42 NPCs | cylinders (Pm also a sphere) | yes by mask | not imported | none | |
| 5 heart piece, 29 hit tag | interaction cylinders | yes | not imported | none | |
| TA#0 `EN_DOOR` | no dynapoly for type 0 | yes | not imported | none | beyond the East-lower trigger |
| `Obj_Tokeidai` (13-16, 18) | none in the overlay | yes | render only | render only | none |

## 7. Routes

Evidence: [city-mvp-editor.json](../../../evidence/city-mvp-editor.json) (17:52:54Z) and [city-mvp-routes-editor.json](../../../evidence/city-mvp-routes-editor.json) (17:58:02Z), both editor-hook tests in `SouthClockTown_MVP`; [MVP_REVIEW.md](../../MVP_REVIEW.md). Tested route waypoints converted to N64: square (-370, 0, -654) -> (-568, 0, -660) -> ramp to (-570, 100, -1026) -> (-570, 100, -1206) -> (-678, 100, -1210) -> (-680, 100, -1318) -> ramp to (-374, 200, -1320), 428 steps, all grounded, no side contacts (PRJ). The Enhanced scene was regenerated at 12:01:27 local, after both tests (section 9), so these results belong to an earlier generation of the same scene.

| ID | Route | Source path | Status | Evidence / blocker |
| --- | --- | --- | --- | --- |
| R1 | Flat loop near the start (5.28 m) | ground Y 0 | tested | city-mvp-editor.json segments, return error 0 |
| R2 | Source wall stop | ground wall at Unity z -6.0 | tested | wallCollision 1.275 m of 3.6 m |
| R3 | West ramps up, Y 0 -> 100 -> 200 | 251/252, then 246/247 | tested | city-mvp-routes-editor.json, upperLevel Y 2.025 |
| R4 | East ramps up, Y 0 -> 100 -> 200 | 259/260, then 268/269 | not_run | |
| R5 | East branch to the Y 180 landing | 257/258 | not_run | |
| R6 | Reverse descent, west side | 246/247, 251/252 | not_run | |
| R7 | Reverse descent, east side | 268/269, 259/260 | not_run | |
| R8 | North approach | polys 18/19, Y 200 | not_run | |
| R9 | Termina Field approach | polys 176/177, Y 0 | not_run | gate soldier (12) at the trigger edge; rule NI |
| R10 | East upper approach | polys 24/25, Y 100 | not_run | |
| R11 | East lower approach | ramp polys 180/181 | tested (stop only) | eastLower: preview box stop at x 324.5, before the source trigger at x 347; no transition |
| R12 | West upper approach | ramp polys 184/185 | not_run | |
| R13 | West lower approach | polys 165/166, Y 0 | not_run | |
| R14 | Laundry Pool approach | polys 158/159, Y 100 | not_run | |
| R15 | Clock Tower interior approach | polys 8/9 behind the leaves | not_run | preview leaves are static colliders (6.6) |
| R16 | Laundry corridor traversal | ramp 178/179 to the Y 100 terrace | not_run | |
| R17 | Tower stair to the rooftop | treads Y 255..375, exit polys (22) | blocked by state | closed panel dynapoly and hidden staircase on Day 1; open only Day 3 before 06:00, day >= 4, or tower-opened flag (`z_obj_tokei_step.c:191-208`, `z_obj_tokeidai.c:615-622`) |
| R18 | Day 1 platform top | turret top floor Y 160 | not_run | no room floor or ramp near it above Y 0; access path UNKNOWN (U-06) |
| R19 | Tower-front ledge Y 240 | 243/244/271/272 from the Y 180 landing | not_run | connectivity UNKNOWN (U-07) |
| R20 | Water channels | water boxes over Y -40 | not_run | water not imported |

Totals: tested 4, not_run 15, blocked by state 1. Regions behind exit triggers (x 720..1200 at Y 100, x -2040..-1200 at Y 200) are not routes for South: a walker crosses an exit polygon first (INF).

## 8. Room geometry, materials and textures (counts and names only)

### 8.1 Room display lists and what the import parsed (C; PRJ `local-reference/blender-import.log`, `import-report.json`)

| Item | Value |
| --- | --- |
| Room `Gfx` DLs / `Vtx` arrays | 239 / 148 |
| `gsSPBranchLessZraw` LOD commands | 53, to 43 distinct targets; 12 DLs carry two branches, the first to an empty DL (`DL_000A10` or `DL_011740`) |
| Triangles reachable from the shape following every branch | 1134 |
| Triangles Fast64 parsed (log "Vertices/Triangles" per shape DL) | 1048: 2, 2, 140, 0, 85, 8, 358, 12, 417, 24 for 011508, 011668, 011430, 013870, 00EF00, 013820, 00D990, 0133E8, 0073E8, 012400 |
| Imported room polygons (import-report) | 1033: 00D990 has 343 after Blender's vertex merge (15 lost, U-04) |
| Not parsed | 86 triangles in the 12 second-branch targets: 006680, 006908, 006C20, 006EA8 (under 0073E8), 012928, 012B40, 012D58, 012F70, 013188 (under 0133E8), 011C80, 011E98, 0122C0 (under 012400). INF: Fast64 follows the first `BranchLessZ` target as a jump; the per-entry counts match this model exactly |
| Animated-material DLs | `DL_012090` calls segment 0x08 (imported, scroll not reproduced); `DL_0122C0` calls segment 0x09 (second branch, not imported) |
| Import totals | 21 render meshes (9 room, 12 actor), 1501 render triangles (1033 room, 468 actor), 1 collision mesh of 508 |

### 8.2 Textures by format (C; PRJ `Unity/Assets/LocalReference/SouthClockTown/texture-sources.json`)

| Source | rgba16 | ci4 | i4 | i8 | ia8 | TLUT (rgba16) | Total |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Extracted scene PNGs (all in the scene file; room file holds none) | 75 | 3 | 7 | 1 | 1 | 3 | 90 |
| Exported from the scene (texture-sources.json) | 61 | 3 | 7 | 1 | 1 | 3 | 76 |
| Exported from actor banks | 25 entries | 4 | 3 | | | | 32 entries = 26 unique images (gear sides x3, gear face x2, turret text x2, tobira `tex_000800` x2, `tex_000000` x2) |
| Export total | | | | | | | 108 `texture_000..107.png` |

The 158 exported materials all bind both slots (two-cycle, lighting on, 7 transparent, 7 using `LOD_FRACTION`); the Unity reference makes 316 per-material sampler copies (2 x 158). The 14 never-exported scene textures are all rgba16 mip levels loaded with `gsDPLoadMultiBlock` on tiles 2 and 3 under `G_TL_LOD`: `Tex_00D340`, `00D380`, `010080`, `010100`, `010300`, `010380`, `010580`, `010600`, `010800`, `010880`, `010A80`, `010B00`, `011D00`, `011D80`, in DLs 009170, 009458, 0096D0, 009920, 009D00, 00A700 (all under 00A958) and 00AA90, all children of `DL_00D990` (C, room.c:2252-2433). Their geometry is imported; their mip chains are not (INF: Fast64 binds tiles 0/1 only; U-05).

### 8.3 Repaints (PRJ `evidence/city-art-manifest.json`, `BuildCityMvp.cs:22-27`, `reference.json`)

Manifest hashes re-checked against the local files: all four match.

| Repaint file (sha256 prefix) | Replaces export | Source image | Materials (slots) | Mesh | Slots |
| --- | --- | --- | --- | --- | --- |
| `stone_bricks.png` (5ee7e278) | `texture_052.png` | `Z2_CLOCKTOWERTex_015000` rgba16 | 86-97, 119 | room 0073E8 | 16 |
| `stone_bricks.png` | `texture_097.png` | `clock_tower_bricks` rgba16 (object_obj_tokeidai) | 147, 148 | counterweight | 3 |
| `plaza_pavers.png` (707b2087) | `texture_041.png` | `Z2_CLOCKTOWERTex_00CB40` rgba16 | 55 | room 00D990 | 2 |
| `wall_relief.png` (db24f97f) | `texture_057.png` | `Z2_CLOCKTOWERTex_00C340` i4 | 98, 99, 102, 103, 105-110 | room 0073E8 | 10 |
| `golden_pavers.png` (856c63e9) | `texture_037.png` | `Z2_CLOCKTOWERTex_00D880` rgba16 | 51 | room 00D990 | 2 |

Totals: 4 files, 5 source images, 27 materials, 33 slots (matches MVP_REVIEW). Untouched: 103 of 108 exported images (72 scene entries including the 3 TLUTs, 31 actor-bank entries) plus the 14 never-exported mip textures. `Tex_00CB40` is also the tile-0 texture of the mip-mapped `DL_009D00` material. The Enhanced `Samplers` folder holds 465 sampler assets while the playbook records 149 unique samplers per run; `Store()` never deletes, so older files remain (INF).

## 9. Changed-input audit (PRJ: git history, file hashes and modification times, all 2026-11-08 local time, UTC-6)

| Input / output | State | Evidence |
| --- | --- | --- |
| `tools/reference/import_clocktown.py` | unchanged since its first commit `15a46cd` (11:33:55): sha256 `fd4231e3...` in 15a46cd, d0bc29f and 98e554b; file mtime 11:27:08 | `git show <c>:<path> \| sha256sum` |
| `tools/reference/export_reference.py` | 15a46cd version (4541 bytes, sha256 `8f9f47eb...`) lacks `sourceTexture0/1` and the `texture-sources.json` write; d0bc29f = 98e554b = working tree (4731 bytes, `6f5b1755...`), mtime 11:35:10 | `git diff 15a46cd 98e554b` (2 added lines) |
| `local-reference/import_clocktown.py`, `export_reference.py` (11:14:39) | older generation: absolute paths, no tile, UV-basis or LOD fields, triangle normals; not called by the committed script, which execs `tools/reference/export_reference.py` (`import_clocktown.py:110`) | `diff` against the committed files |
| Last Blender run | `blender-import.log`, `import-report.json`, `.blend`, `reference.json` and `texture-sources.json` all written 11:35:21; 108 texture PNGs rewritten 11:35; log `REFERENCE_EXPORT {"meshes": 22, "materials": 158, "textures": 108, "triangles": 2009}`; previous save `.blend1` 11:22:03 | mtimes; log text |
| Current local export | `reference.json` sha256 `ada0fd31...` (1,264,011 bytes) contains `sourceTexture0/1` on all 158 materials; `texture-sources.json` `f0db4d9f...` exists. Only the current export script writes these, and it was saved 11 s before the run: **produced by the current committed scripts** (INF; no hash was recorded at run time) | file content and mtimes |
| Unity reference scene and its verification | `Generated/` materials, meshes, textures and `SouthClockTown_RomReference.unity` (sha256 `408da692...`) written 11:25-11:26:00; `unity-import.json` 11:26:00; `evidence/rom-reference-verification.json` `verifiedUtc` 17:26:26Z = 11:26:26 local. All precede the 11:35 export: **built from an earlier export** (the run saved by `.blend1` at 11:22, INF) made by a pre-commit `export_reference.py` whose text is not recorded. `ImportRomReference.cs` (mtime 11:25:33, unchanged since 15a46cd) tolerates exports without UV-basis/tile fields, so its success does not reveal which version ran | mtimes; `ImportRomReference.cs:185` comment |
| Recorded verification content | tool revisions (ROM MD5, mm 3be2e24, fast64 b130109, Blender 5.2.3, Unity 6000.5.11f1) and counts (21, 1501, 1, 508, 158, 108, 316) only: no script hash, no input or export hash, no export timestamp, no mention of the local fast64 patch. Counts equal the 11:35 export, which shows count agreement only | evidence file |
| fast64 checkout | HEAD b130109 with `fast64_internal/f3d/f3d_parser.py` modified (matrix-pop patch applied at 11:12:59; sha256 `0f61a839...`); the import script patches this file in place when needed (`import_clocktown.py:7-14`) | `git status` in `local-reference/fast64` |
| Extracted inputs | `Z2_CLOCKTOWER.c` and room C 11:11:04, object C files 11:11:22, binaries 11:07:16: all before both Blender runs | mtimes |
| Enhanced MVP | `SouthClockTown_MVP.unity` and Materials/Meshes/Geometry written 12:01:25-27 by `BuildCityMvp.cs` (mtime 12:01:17). It opens the 11:26 Generated scene and materials by index but reads texture names from the 11:35 `reference.json` and PNGs from the 11:35 `Textures/`: two export generations mixed, correct only if material order did not change (unverified). The editor tests (11:52:54, 11:58:02) ran before this regeneration | `BuildCityMvp.cs:42-64`; mtimes |

**Verdict:** the recorded reference scene and `rom-reference-verification.json` were produced from an earlier, unrecorded exporter version, not from the committed scripts; the local `reference.json` and textures were later re-exported by the current committed scripts, but the Unity reference scene was never regenerated from them, and the Enhanced scene and route evidence belong to different generations. Provenance is not established for any Unity artifact.

To re-establish it (NOT run here; Blender/Unity are excluded from this packet): (1) record sha256 of the scene/room C, the four object C files, both scripts, the patched `f3d_parser.py` and the ZAPD exe; (2) rerun `import_clocktown.py` in Blender 5.2.3 (it execs the exporter) and hash `reference.json`, `texture-sources.json`, the 108 PNGs and `import-report.json`; (3) rerun `ImportRomReference.Generate()`; (4) rerun `BuildCityMvp.Generate()` after clearing stale `Samplers`; (5) rerun the editor smoke and route checks and the ground probes against that exact scene, writing a verification record that includes script, input and output hashes and timestamps (REC-03).

## 10. Proposed region record

**Proposal, schema not frozen (PIPE-01).** Same field set as North section 9.

```json
{
  "_status": "proposal, schema not frozen (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.south_clock_town",
  "aliases": ["scene.south_clock_town", "sct.*"],
  "displayLabel": "South Clock Town",
  "source": {
    "sceneSymbol": "Z2_CLOCKTOWER",
    "sceneEnum": "SCENE_CLOCKTOWER",
    "sceneTableIndex": "0x6F",
    "entranceTableSceneIndex": "0x6C",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": "b13010949567e4a2bf44ba3ccc18b06427243153 plus local f3d_parser.py patch (sha256 0f61a839138c40cf6935a762b78bde89a8a904d5a596784c321bfc0b223041d4)"
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0)",
    "rooms": { "loc.south_clock_town.room_00": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event, switch or collectible flags", "player form Human", "owl statue not activated", "WEEKEVENTREG_59_04 clear (door cutscene pending)"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_CLOCKTOWER", "bytes": 140896, "sha256": "64da8491df8c143fbf3bd6b4523dede22a45b5a0b181c91403ac94af81320706" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_CLOCKTOWER_room_00", "bytes": 80048, "sha256": "c9b8754c74786f1553d2a667e1c52fcbb140b65d729ffb7a0e8f7bbf64f3efd5" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_CLOCKTOWER/Z2_CLOCKTOWER.c", "bytes": 209318, "sha256": "ff08dbfb319cca148a4260eab9462d4b06c5c05c97392a7ea35f56487283aaef" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_CLOCKTOWER/Z2_CLOCKTOWER_room_00.c", "bytes": 231563, "sha256": "88a093d637ea0a4a8b82725cced8e3cbbacce18070dab1e119ea5220ea09a92b" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_CLOCKTOWER/Z2_CLOCKTOWER.xml", "bytes": 418, "sha256": "f065cbde259cef0ec3b4221696ab3c7d6625b45d37bd6633fd94c466626d0968" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/objects/object_obj_tokeidai/object_obj_tokeidai.c", "sha256": "4e05dadb0fd8bf084820cf006321274b4f6d3d5fe389f7db9d162e2ef8384f8a" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/objects/object_tokei_turret/object_tokei_turret.c", "sha256": "f8319a02976e7d3fce1632b44f489f2b84313ccb3524c32c87a9822855f36a43" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/objects/object_tokei_step/object_tokei_step.c", "sha256": "dc1dd8ff1785b7b42265e13278155800c98636f14df8503917331c728e9793f6" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/objects/object_tokei_tobira/object_tokei_tobira.c", "sha256": "919936cfc3a92e24f48a05607bab85d0deb1eb5bfdf083dcdbf2a184321df6c2" }
  ],
  "localExports": {
    "referenceJson": { "path": "Unity/Assets/LocalReference/SouthClockTown/reference.json", "sha256": "ada0fd31667547d4a57e0d14556fc68b97a52eaacda28cf58523a30640d0c9e5", "writtenLocal": "2026-11-08T11:35:21", "producedBy": "current committed scripts (inferred)" },
    "textureSourcesJson": { "path": "Unity/Assets/LocalReference/SouthClockTown/texture-sources.json", "sha256": "f0db4d9f7806b5281004842cb2aec97c9114658a0887b12358298596a8887770" },
    "unityReferenceScene": { "path": "Unity/Assets/LocalReference/SouthClockTown/Generated/SouthClockTown_RomReference.unity", "sha256": "408da692dd4addd73be050ff62cb77cec87a39ab63152e32b22c5ce3b519d7da", "writtenLocal": "2026-11-08T11:26:00", "producedBy": "earlier unrecorded export" }
  },
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedWithNorthWestEast": "INFERRED (PORTAL_REGISTER section 3.2), unverified" },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-2360, -424, -1974], "max": [1600, 1183, 1200] },
    "sourceCollisionVertices": { "min": [-2040, -424, -1600], "max": [1200, 995, 1000] },
    "unityImportRecorded": { "min": [-23.6, -4.24, -10.2], "max": [16.0, 13.03, 16.0] }
  },
  "anchors": [
    { "id": "anchor.sct.spawn0_tower_door", "source": [-278, 0, -801], "unity": [-2.78, 0.0, 8.01], "status": "ground probe recorded in unity-playmode-verification.json, not surveyed" },
    { "id": "anchor.sct.spawn4_north_upper", "source": [-260, 200, -1380], "unity": [-2.60, 2.00, 13.80], "status": "proposed, not surveyed" },
    { "id": "anchor.sct.spawn7_east_lower", "source": [323, 0, 247], "unity": [3.23, 0.0, -2.47], "status": "ground probe recorded, not surveyed" },
    { "id": "anchor.sct.owl_statue", "source": [-540, 100, -1099], "unity": [-5.40, 1.00, 10.99], "status": "proposed, not surveyed" },
    { "id": "anchor.sct.clock_center", "source": [-280, 555, -833], "unity": [-2.80, 5.55, 8.33], "status": "proposed, not surveyed" }
  ],
  "rooms": [ { "id": "loc.south_clock_town.room_00", "sourceRoom": "Z2_CLOCKTOWER_room_00", "roomShape": "cullable, 6 entries", "displayLists": 239, "lodBranchesNotImported": 12 } ],
  "components": [
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a13", "actorId": "0x19C", "params": "0x207F", "class": "static-mesh", "status": "imported" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a14", "actorId": "0x19C", "params": "0x007F", "class": "static-mesh", "status": "imported" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a15", "actorId": "0x19C", "params": "0x007F", "class": "static-mesh", "status": "imported" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a16", "actorId": "0x19C", "params": "0x307F", "class": "static-mesh", "status": "imported, yaw 0 phase" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a17", "actorId": "0x1B8", "params": "0x0000", "class": "static-mesh", "status": "imported, collision missing" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a19", "actorId": "0x1A2", "params": "0x0000", "class": "door/transition", "status": "imported with spawned type-1 leaf, push behavior missing" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a21", "actorId": "0x221", "params": "0x0001", "class": "static-mesh", "status": "imported, collision missing" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a24", "actorId": "0x221", "params": "0x0000", "class": "static-mesh", "status": "imported, collision missing" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a04", "actorId": "0x183", "params": "0x017F", "class": "static-mesh", "dls": ["gameplay_keep:0x11BD0"], "status": "selected, not extracted" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a32", "actorId": "0x223", "params": "0x0004", "class": "static-mesh", "dls": ["object_sek:0x1D0"], "status": "selected, not extracted" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a45", "actorId": "0x026", "params": "0x360A", "class": "static-mesh", "dls": ["gameplay_keep:gSignDirectionalDL"], "status": "selected, not extracted" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a05", "actorId": "0x00E", "params": "0x0A06", "class": "static-mesh (state pickup)", "status": "optional, fresh save" },
    { "sourceKey": "Z2_CLOCKTOWER/h0/room_00/a48", "actorId": "0x17C", "params": "0x037F", "class": "static-mesh (camera-relative)", "status": "selected, special rule" }
  ],
  "collisionOwnership": {
    "room": "Z2_CLOCKTOWERCollisionHeader_005560 (508 polys, 21 surface types)",
    "waterBoxes": 2,
    "actorDynapoly": ["a17 gClocktowerPanelCol", "a19 + child object_tokei_tobira_Colheader_0012B0/001590", "a21 gClockTownTurretPlatformCol", "a24 gClockTownTurretBaseCol", "a04 gGoldDekuFlowerCol", "a43 gBoxChestCol"],
    "actorCylinders": ["a32 owl", "a46/a47 postboxes", "a45 sign", "a12 soldier", "NPC cylinders"]
  },
  "portalIds": [
    "portal.south_clock_town.clock_tower_interior",
    "portal.south_clock_town.z2_00keikoku",
    "portal.south_clock_town.east_clock_town.upper",
    "portal.south_clock_town.west_clock_town.upper",
    "portal.south_clock_town.north_clock_town",
    "portal.south_clock_town.west_clock_town.lower",
    "portal.south_clock_town.laundry_pool",
    "portal.south_clock_town.east_clock_town.lower",
    "portal.south_clock_town.clock_tower_rooftop"
  ],
  "materialBindings": { "sourceMaterials": 158, "exportedImages": 108, "repaintFiles": 4, "repaintedSlots": 33 },
  "cameraIds": [],
  "routeIds": ["R1", "R2", "R3", "R11"],
  "omissions": ["a00 Business Scrub", "a01 Anju", "a02 Kafei", "a03 En_Sth", "a06 Postman", "a11 dog", "a12 gate soldier", "a33/a36/a37 carpenters", "a43 chest", "a46/a47 postboxes", "a08 chimney billboard and smoke", "86 LOD-branch triangles", "14 mip-level textures", "Link and Tatl"],
  "excluded": { "halfDayMask": ["a07", "a20", "a22", "a23", "a25", "a26", "a30", "a31", "a34", "a35", "a38", "a39", "a44"], "stateOrDrawNull": ["a18", "a41", "a42"], "noGeometry": ["a09", "a10", "a27", "a28", "a29", "a40", "a49"] },
  "unsupported": ["surface semantics", "water boxes", "actor collision", "door push", "LOD branches", "mip LOD", "texture scroll", "skeletal poses", "billboards", "moon 9000-unit rule"],
  "evidence": ["docs/city/districts/south_clock_town.md", "evidence/rom-reference-verification.json", "evidence/city-mvp-routes-editor.json"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "partial (4 banks)", "import": "fail (provenance not established, section 9)", "controllerTraversal": "partial (4 of 20 routes)", "editorRender": "not_run on current inputs", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Purpose of layers 1 and 3 | Which code writes `nextCutsceneIndex` 0xFFF0 or 0xFFF2 for an `ENTRANCE(SOUTH_CLOCK_TOWN, n)` (SOURCE_REGISTER U-02/U-03); do title-screen code paths load South layer 1 |
| U-02 | Spawn bgCam indices 0x04-0x12 vs 3 `BgCamInfo` entries | Which code consumes `PLAYER_GET_BG_CAM_INDEX`; do they index the 19-entry actor cutscene camera list |
| U-03 | LOD branch policy | Read `gsSPBranchLessZraw` in `include/PR/gbi.h` for the raw z unit, and Fast64 `f3d_parser.py` handling of `G_BRANCH_Z`; decide whether the reference should carry near, far or both LODs |
| U-04 | 15 triangles lost in `DL_00D990` | Which triangles collapse in Fast64 `importMeshC` vertex merge (compare parsed index list with the .blend mesh) |
| U-05 | Mip chains (14 textures) | Does `ImportRomReference` or a future shader need tiles 2/3; Fast64 material import of `gsDPLoadMultiBlock` tile 2+ |
| U-06 | Day 1 platform access | Which surface or actor lets the player reach the turret top floor at Y 160: read all 10 `gClockTownTurretPlatformCol` and 8 base polygons, and En_Sth/En_Daiku path data in `Pathway_000A8C` |
| U-07 | Y 180 landing to Y 240 ledge to stair | Wall polygons and types between Y 180 and 240 at x -400..-160, z -826..-680; player ledge-climb heights (`ageProperties` in `z_player.c`) |
| U-08 | Walkable slope limit | Which normal-Y threshold makes a floor slide or wall for the player (`z_bgcheck.c` floor classification, `z_player.c` slope handling) for the 33, 35 and 46.6-degree surfaces |
| U-09 | Chest 43 support at Y 374 | Room DL geometry near (249, 374, 565) and En_Box type-5 movement flags (`z_en_box.c:19-39`, init) |
| U-10 | Gate soldier at the Termina Field exit | `EnStopheishi` actions (`z_en_stop_heishi.c:160-530`), params 0x007F |
| U-11 | TA#0 door context | Overlay East `Z2_TOWN` collision at (1114, 100, 274) and confirm the shared frame; is the South door ever drawn or used (PORTAL U8) |
| U-12 | Anju and Postman anchors beyond exit triggers | Schedule scripts and path indices in `z_en_an.c` and `z_en_pm.c` that move them into the square at 12:00 |
| U-13 | Switch flag bit 20 | Which South actor or cutscene sets/tests switch 0x14 |
| U-14 | Sellnuts visibility at noon | `func_80ADC5A4` and the non-flying branch after init: when is `draw` set |
| U-15 | Business Scrub noon pose | `func_80BEE938` / `func_80BEEB20` for params 0xFC04 |
| U-16 | Water box properties 0x101 | `WaterBox_GetBgCamIndex` / `GetLightSettingIndex` (`z_bgcheck.c:4555-4602`) and room bits; player behavior in 20 units of water |
| U-17 | Route evidence vs current scene | Rerun R1-R3, R11 on the regenerated scene (REC-03) |
| U-18 | Depth-band geometry | What the 12 second-branch DLs draw (render or capture under the profile) |
| U-19 | Header bounds larger than vertex extents | As North U-14 |
| U-20 | Doc drift | SOURCE_REGISTER `cam.south_clock_town.east_lower_approach` still cites polygons 184/185; source and the corrected playbook say 180/181 (owner: SOURCE_REGISTER) |

**Not inspected:** room DL contents beyond triangle/texture/LOD counting; combiners; light-setting values; cutscene data and actor cutscene entries; minimap rendering; path point data; NPC schedules and behavior beyond the init lines cited; En_Talk/Elf_Msg6/En_Hit_Tag params meaning; En_Mag, Dm_Stk, En_Weather_Tag behavior; Fast64 source; `.blend` contents; Unity `Library` state; captures; 3DS material.

**Checks run:** read the listed docs, scripts, local exports and evidence JSON; read scene/room C, XML, actor/object/scene tables, `z_actor.c`, `z_bgcheck.c`, `z64bgcheck.h`, `z_scene_proc.c`, `z_scene_table.c` and the overlays cited; scratch parsers `actors.py`, `col.py`, `ramps.py`, `probe.py`, `objcol.py`, `dl2.py`, `mats.py`, `tex.py`, `ev.py`, `classes.py` in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref04\`; sha256 of inputs, scripts at each commit, exports and repaint files; git history and status of the repo, fast64 and decomp checkouts. Cross-check: counts agree with LOCATION_REGISTER (`r00 d 23/50, a1 7/12, a2 5/9, a3 19/29`, 1 transition actor, spawns 11/2/1/1, exits 10/10/none/10) and with ROM_REFERENCE (80,048-byte room, 50 actors, 23 objects, alternates at 0x420/0x530/0x610).

**NOT run:** ZAPD, Blender, Unity, any player or executable, captures, renders, controller routes, object extraction, hashing of the 108 export PNGs.

**Next dependency:** PIPE-01 schema review of section 10; REC-03 provenance rerun (section 9 steps); SCT-01 route checks R4-R20; REF-03 capture profile for U-06, U-07, U-18.
