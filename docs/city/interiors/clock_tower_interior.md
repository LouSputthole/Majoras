# Clock Tower Interior (`loc.clock_tower_interior`): source inventory dossier

Task **REF-09 / INT-16** (FULL_CITY_PLAN.md section 12 row REF-09, section 6 row INT-16; the dispatch called it "section 5", the INT rows are in section 6). Base commit `31b20dc` on `main` (HEAD moved to `7a7a08c`, PIPE-01, while this packet ran; IDs below were checked against its `shared/city/ids.json`). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_INSIDETOWER` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0) in the **post-introduction** state defined in section 5.4.

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/` or `shared/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_INSIDETOWER/Z2_INSIDETOWER.c`, `r00.c` / `r01.c` = `.../Z2_INSIDETOWER_room_00.c` / `.../Z2_INSIDETOWER_room_01.c`. Line numbers count `\n` only (`grep -n`); scene.c has 9 lone `\r` bytes that some editors count as extra lines. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected. Five sender values (section 2.1) come from a scratch scan of the scene-header commands of the non-extracted baserom files `Z2_OPENINGDAN` and `SPOT00` (exit list `0x13` and cutscene-script list `0x17`; numbers only, nothing copied); they are tagged **C-BIN**.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_INSIDETOWER` / `SCENE_INSIDETOWER` | HDR `include/tables/scene_table.h:203` |
| Scene-table index | `0x63` | same line |
| Entrance-table scene index | `0x60` (`ENTR_SCENE_CLOCK_TOWER_INTERIOR`) | `src/code/z_scene_table.c:2669`, `include/z64scene.h:689` |
| Title text / draw config | `0` (no title card) / `SCENE_DRAW_CFG_MAT_ANIM` | scene_table.h:203 |
| Restrictions | `RESTRICTIONS_SET(0,0,0,0,3,3,3,3,3,0,0,0)`: hGauge, B, A, trade items 0; Song of Time, Double Time, Inverted Time, Soaring, Storms 3; masks, picto box, all 0 | field order C `src/code/z_parameter.c:66-71`; the meaning of value 3 is LOCATION_REGISTER U-13 (INF: those songs are blocked) |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_SET(1 << 0, 0, 0, 0)`: **switch flag 0 survives the three-day reset** | scene_table.h:203; mask applied in `Sram_SaveEndOfCycle`, `src/code/z_sram_NES.c:430, 451, 457-458` |
| XML files | scene seg 2; `room_00` seg 3 (DLs `0x6B10`, `0xC9E0`); `room_01` seg 3 (DL `0x198`) | XML `assets/xml/scenes/Z2_INSIDETOWER/Z2_INSIDETOWER.xml` |
| Rooms | 2 (`SCENE_CMD_ROOM_LIST(2, ...)`) | HDR scene.c:16, :55-58 |
| Scene headers | **default only**: no `SCENE_CMD_ALTERNATE_HEADER_LIST`; each of the 7 entrance-table rows has one layer-0 entry | HDR scene.c:14-32; `z_scene_table.c:2039-2065` |
| Room headers | default only, both rooms | HDR r00.c:14-23, r01.c:14-23 |
| Sound | spec 5, ambience 19, seq 5 = `NA_BGM_CLOCK_TOWER` | HDR scene.c:15; macro `include/z64scene.h:845-846`; `include/tables/sequence_table.h:30` |
| Special files | `NAVI_QUEST_HINTS_NONE`, `GAMEPLAY_FIELD_KEEP` | HDR scene.c:21 |
| Skybox | file 0, skybox 0, weather 0, lightMode `true` = `LIGHT_MODE_SETTINGS` (lights by setting index, not by time) | HDR scene.c:23; `include/z64environment.h:47-50` |
| Env light settings | 11 entries (values not transcribed) | HDR scene.c:25, :76-99 |
| Cutscene scripts / actor cutscenes / cs cameras | 8 / 18 / 1 | HDR scene.c:26-28 (section 5.2) |
| Minimap | 2 entries, both mapId `0xFFFF` (no map) | HDR scene.c:1914-1921 |
| Animated material | 1 entry `{ -1, 1, ... }`: segment 8 (`abs(-1) + 7`), type 1 two-texture scroll, steps (1, 7) and (-1, 5), 32x32 | HDR scene.c:1937-1944; C `src/code/z_scene_proc.c:400-406` |
| Transition actors | 1 (`EN_HOLL`) | HDR scene.c:17, :51-53 |
| Spawns / entrance rows / exits | 7 / 8 (row 7 = pad, INF) / 2 | HDR scene.c:22, :34-49, :60-69, :71-74 |
| Room commands (both rooms) | echo 10; `ROOM_BEHAVIOR(0x01, 0x00, 0, 0, false, 0)` = `ROOM_TYPE_DUNGEON`, environment 0, storm 0; `SKYBOX_DISABLES(true, true)`; `TIME_SETTINGS(255, 255, 0)`: no forced time, **time speed 0 (the clock does not advance inside)**; cullable room shape | HDR r00.c:15-19, r01.c:15-19; C `src/code/z_scene.c:246-253, 387-406`; `include/z64scene.h:315-316` |

### 1.1 Header selection (C)

`sceneLayer = (cutsceneIndex & 0xF) + 1` only for `cutsceneIndex >= 0xFFF0` (`src/code/z_play.c:2210-2219`). The interior has no alternate header list and no entrance row with a layer above 0, so **intro versus post-introduction is not a header choice**. It is selected at run time by the arrival entrance (cutscene-script spawn triggers), SCENE_INSIDETOWER switch flag 0, Ocarina possession, the Song of Healing and the Deku Mask (section 5). LOCATION_REGISTER (`Alt 0`) agrees.

### 1.2 Rooms

| Room | File | Shape (HDR) | Objects / actors (HDR) | Geometry (C) | Role |
| --- | --- | --- | --- | --- | --- |
| `loc.clock_tower_interior.room_00` | `Z2_INSIDETOWER_room_00` | cullable, 15 entries (12 opaque, 5 XLU; entries 5 and 11 have a negative radius, which sorts them last, `src/code/z_room.c:182-186`) | 6 / 12 | 23 `Gfx`, 19 `Vtx`, 336 `gsSP2Triangles` + 12 `gsSP1Triangle` = 684 triangles; 17 textures + 3 TLUTs | tower chamber: base floor Y -300, water channel, central platform Y -30, door landing Y 45; Happy Mask Salesman (HMS) |
| `loc.clock_tower_interior.room_01` | `Z2_INSIDETOWER_room_01` | cullable, 1 entry, center (180, 540, -905), radius 58 | 3 / 2 | 2 `Gfx`, 1 `Vtx`, **1 triangle** at x 140..220, Y 500..580, z -905, untextured (r01.c:51-78 and its `.vtx.inc`) | the twisting corridor from `Z2_OPENINGDAN`; its visible geometry is the two `Bg_Ctower_Rot` corridor actors (INF from the one-triangle room DL) |

LOCATION_REGISTER (`r00 d 6/12; r01 d 3/2`, `Trans. 1`, `Spawns 7`, `Exits 2`, `Entr. 8 (last pad?) / 7`) agrees with the C. **Disagreement with the dispatch:** it said "1 spawn, 7 transition actors"; the C has 7 spawns and 1 transition actor (scene.c:22 `SPAWN_LIST(7, ...)`, :17 `TRANSITION_ACTOR_LIST(1, ...)`).

## 2. Spawns, entrances and exits

### 2.1 Start positions and entrances (HDR scene.c:34-49, :60-69; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`; modes 0xD and 0xF are unnamed, `:34-36`). Yaw = `(rawRotY >> 7) & 0x1FF`. Entrance n = `0xC000 | (n << 4)`; every row maps slot n to spawn n (`z_scene_table.c:2039-2065`). The entrance cutscene is the actor cutscene whose script `spawn` equals `play->curSpawn` (`src/code/z_eventmgr.c:516-540`), started immediately when its `spawnFlags` is `CS_SPAWN_FLAG_ALWAYS` (`src/code/z_demo.c:1544-1565`).

| Spawn / entrance | Room | Position | Yaw | Params (mode) | Row flags (line) | Sender | On arrival |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 / `0xC000` | 1 | (11, -296, -2317) | 0 | 0x0DFF (0xD) | 0x0102 (2040) | `Z2_OPENINGDAN` exit [1] = `0xC000` in h0 and alt1 (C-BIN) | cs 14 = script 4 (ALWAYS): 257-frame arrival, motion blur 1-181 |
| 1 / `0xC010` | 0 | (1, 30, 182) | 180 | 0x0DFF (0xD) | 0x0102 (2044) | South exit [0] `0xC010` (`portal.south_clock_town.clock_tower_interior`) | none; with switch 0 clear the `Obj_Demo` trigger a11 is already in range (5.3) |
| 2 / `0xC020` | 0 | (36, -30, 64) | 0 | 0x0FFF (0xF) | 0x058B (2048) | `SPOT00` layer 1 script 0 (spawn 0, ALWAYS) (C-BIN); interior script 1 sends the player to that `SPOT00` entry (`0x1C01`) | cs 12 = script 2 (ALWAYS): Deku Mask given; En_Osn special conversation |
| 3 / `0xC030` | 0 | (-34, -30, 64) | 0 | 0x0FFF (0xF) | 0x0102 (2052) | `SPOT00` layer 9 script 0 (C-BIN) | no spawn cutscene; En_Osn queues cs 11/17/13 by inventory (5.1) |
| 4 / `0xC040` | 0 | (-93, -30, -33) | 197 | 0x0DFF (0xD) | 0x058B (2056) | `Z2_OPENINGDAN` h0 script 3 (spawn 4, ALWAYS) (C-BIN) | cs 15 = script 5 (ALWAYS): white fill, then destination `0xD8A0` = South spawn 10 |
| 5 / `0xC050` | 0 | (0, -300, -604) | 0 | 0x0DFF (0xD) | 0x0102 (2060) | none found (U-01) | none; 61 u in front of the passage mouth (z -665) |
| 6 / `0xC060` | 0 | (-75, -175, 1) | 210 | 0x0FFF (0xF) | 0x858B (2064) | `SPOT00` layer 9 script 2 (C-BIN) | cs 16 = script 6 (ALWAYS); En_Osn special conversation |

Entrance row 7 `{0x00, 0x00}` has no entrance-table slot (7 pointers, `z_scene_table.c:2067-2071`) and the list ends exactly where the exit list starts (`0x118 + 8 x 2 = 0x128`): alignment pad (INF). Spawn floors (C, scratch `col.py`): spawn 0 poly 549 (Y -296); 1 poly 120 (Y 30, door step); 2, 3, 4 Y -30 (polys 365, 367); 5 poly 514 (Y -300); 6 poly 493 (wood, Y -175). Among the 22 extracted city scenes only South (`0xC010`) sends here, and no `src/` code builds `ENTRANCE(CLOCK_TOWER_INTERIOR, n)` except En_Osn's comparisons (C, grep).

### 2.2 Exit list (HDR scene.c:71-74) and consumers

| Exit idx | Raw | Decode (scene / entr / layer) | Destination row | Consumer |
| --- | --- | --- | --- | --- |
| 0 | `0x2E10` | `0x17` `Z2_OPENINGDAN` / 1 / 0 | `sOpeningDungeonEntrance1` `{ -SCENE_OPENINGDAN, 1, 0x0102 }` (`z_scene_table.c:437-439`) | surface type 12, floors 551, 552 (room_01 end) |
| 1 | `0xD800` | `0x6C` `Z2_CLOCKTOWER` / 0 / 0 | `sSouthClockTownEntrance0[0]` `{ SCENE_CLOCKTOWER, 0, 0x4102 }` (`z_scene_table.c:2344-2345`) | surface type 5, floors 382, 383 (door tunnel) |

The list is exactly 2 words: it starts at `0x128` and the light settings start at `0x12C` (symbol offsets, C). No room actor reads `setupExitList` (among these overlays only `EnHoll` type 4 does, `z_en_holl.c:190-191`; this hall is type 0).

### 2.3 Transition actors: one loading hall, no doors (HDR scene.c:51-53; decode C)

`TransitionActorEntry { sides[2] {room, bgCamIndex}, id, pos, rotY, params }` (`include/z64scene.h:369-378`), spawned with params `(index << 10) + (params & 0x3FF)`, yaw `(rotY >> 7) & 0x1FF`, csId `rotY & 0x7F`, half-day `HALFDAYBIT_ALL` (`src/code/z_actor.c:3600-3625`, `include/z64door.h:22-23`).

| TA | Actor | Position | Raw rotY / yaw / cs | Params / type (OVL) | Sides | Half-day gate | Behavior (OVL `z_en_holl.c`) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_HOLL` (0x018) | (0, -300, -905) | `0x007F` / 0 / none | `0x007F`: id 0, type `(p >> 7) & 7 = 0` `EN_HOLL_TYPE_DEFAULT` (visible room hall), bitmask index `p & 7 = 7` (`z_en_holl.h:63-69`) | `sides[0]` room 1 for player local z < 0 (`EN_HOLL_BEHIND`), `sides[1]` room 0 for local z >= 0; bgCam 255 (-1, generic) both (`:106-111, 184, 197`) | index 7 = every night bit (`z_actor.c:2006-2016`); `((cur & DAWNS) >> 1) | (cur & NIGHTS)` is never 0, so it lives every half-day (`:151-158`) | inside 200 u: requests the far room; 150..200 u: finishes the swap; box half-width 150, height -50..200 (`:85-94, 167-200`; `z_en_holl.h:38-43`). Draws black `gEnHollCentralPlaneDL` (`ovl_En_Holl` 0xC30): opaque, XLU inside 100 u, hidden inside 50 u (`:318-348`) |

**Every door-like element, accounted for.** There is exactly one transition actor (the hall above, the room_00 / room_01 swap plane). The other door-like elements are ordinary room_00 actors: the push-swing front leaves `Obj_Tokei_Tobira` a02 (+ spawned child) in front of the South exit floor, and the sliding stone doors `Bg_Ctower_Rot` a08/a09 that close the passage to the hall. The exterior link is the floor exit behind a02 (surface 5); the `Z2_OPENINGDAN` link is the floor exit at the far end of room_01 (surface 12). There is no `EN_DOOR` in this scene (C), hence no `ENDOOR_SCH_TYPE_*` schedule.

### 2.4 Collision surfaces carrying exits (C from scene.c:1951-1956 and the polygon list)

| Surface type | data[0] / data[1] | Exit | Polys (lines) | Kind | Extent x / Y / z | Conveyor | Room side (INF, by the hall plane z -905) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 12 | 0x00000100 / 0x00001002 | idx 0 `0x2E10` | 551, 552 (scene.c:2510-2511) | floor, normal +Y | -49..71 / -296 / -2500..-2380 | none | room_01, directly behind spawn 0 (z -2317, facing +Z) |
| 5 | 0x00000200 / 0x000C1002 | idx 1 `0xD800` | 382, 383 (scene.c:2341-2342) | floor, normal +Y | -80..80 / 45 / 270..445 | vIB conveyor flag; speed 3, direction 0 (+Z, outward) | room_00, door tunnel behind a02 |

Both are FLOOR polygons, which the player reads (`z_player.c:6405-6416`, standing floor `:12572`); no wall carries an exit. Door tunnel: opening x -80..80, Y 45..185 in the z 270 wall (polys 356-363), side walls x +-80 (378-381), ceiling Y 185 (375, 376). Collision is one scene mesh for both rooms (scene.c:19); the room split is inferred from the hall plane.

### 2.5 Door to South Clock Town and return (cross-check: `south_clock_town.md` 2.1, 2.3, 6.6; PORTAL_REGISTER rows 167, 248)

| Item | South side | Interior side |
| --- | --- | --- |
| Door actor | a19 `Obj_Tokei_Tobira` (-280, 0, -807), yaw 0, csId 16 | a02 `Obj_Tokei_Tobira` (0, 45, 288), yaw 180, csId none |
| First-entry cutscene | if `WEEKEVENTREG_59_04` clear, `SCENE_CLOCKTOWER`, layer 0 (`z_obj_tokei_tobira.c:78-82`) | never (the scene test fails) |
| Leaf pivots (Init `:72-76`: rotY(-0x4000 or +0x4000 plus home yaw) x (0, 0, 80)) | (-360, 0, -807), (-200, 0, -807) | type 0 (80, 45, 288); spawned type 1 (-80, 45, 288) |
| Leaf collision | 80 x 140 x 20 (`object_tokei_tobira_Colheader_0012B0` verts x 0..800, Y 0..1400, z +-100, scale 0.1) | same: x -80..80, Y 45..185, z 278..298 |
| Exit floor | polys 8, 9 (-360..-200, 0, -967..-807), exit+1 1 -> `0xC010` | polys 382, 383 (-80..80, 45, 270..445), exit+1 2 -> `0xD800` |
| Arrival | South spawn 0 (-278, 0, -801), yaw 0, mode 0xE, from `0xD800` | spawn 1 (1, 30, 182), yaw 180, mode 0xD, from `0xC010`; 106 u in front of the leaf line |
| Opening hours | none: no `EN_DOOR`, no time test in `ObjTokeiTobira_Update` (`:102-175`) | same |

All values agree with the South dossier and PORTAL_REGISTER. Push rule (OVL `z_obj_tokei_tobira.c:111-170`): while the player touches a leaf from its local +Z side more than 20 u from the pivot along the leaf, the leaf gains `(d - 20) x sign x 16/3` binang per frame (past 48 u the partner is also flagged), damped by 0.87 per frame and clamped to -1000..14336 binang (about -5.5..78.75 deg). A yaw-180 leaf has its local +Z on world -Z, the room side (C). The exit floor starts 8 u in front of the closed leaf face (z 270 vs 278): U-05.

### 2.6 The `Z2_OPENINGDAN` side (Deku-form intro route)

Route in source order: spawn 0 (room_01, Y -296) -> corridor (the room_01 scene collision has only two boxed stubs, z -2500..-2260 holding the exit floor and z -1145..-905 holding floors 541/542; the 1,115 u between them is floored only by the corridor dynapoly `gClockTowerCorridorCol`, INF from the gap) -> hall plane z -905 -> passage x -60..60, Y -300..-180, z -905..-665 (polys 0-7, 523-528) -> stone doors a08/a09 at z -710 -> base floor Y -300. The doors start open (each moved 80 u to x +-125) only when the player is on their local -Z side at init, and close behind with cs 9 once the player is more than 30 u past them (OVL `z_bg_ctower_rot.c:54-70, 113-134`). Arriving from South (z 182) they stay closed at home (`:68-69`), so the passage, the hall and room_01 are shut in the post-intro profile (INF; door collision extents NI, U-02). `Z2_OPENINGDAN` exit [1] `0xC000` (back to spawn 0) is the reciprocal of interior exit 0 (C-BIN); PORTAL_REGISTER lists that reverse as UNKNOWN.

### 2.7 Portal IDs (registered in `docs/city/PORTAL_REGISTER.md` and `shared/city/ids.json`)

| Portal ID (frozen) | Registry mode | Source evidence | Arrival |
| --- | --- | --- | --- |
| `portal.clock_tower_interior.south_clock_town` | district | surface 5, exit 1 `0xD800` (behind leaves a02; INF: "door" would mirror the South side; not changed) | South spawn 0 |
| `portal.clock_tower_interior.z2_openingdan` | external-boundary | surface 12, exit 0 `0x2E10` | `ext.z2_openingdan` entrance 1 (NI) |
| `portal.clock_tower_interior.clock_tower_interior.r0_r1` | room | TA0 `EN_HOLL`, room 0 -> room 1 | room_01 |
| `portal.clock_tower_interior.clock_tower_interior.r1_r0` | room | TA0 `EN_HOLL`, room 1 -> room 0 | room_00 |

Inbound: `portal.south_clock_town.clock_tower_interior` (door) lands on spawn 1. No kickout, grotto or scene-changer hall. Cutscene arrivals (entrances 2, 3, 4, 6) come from `SPOT00` / `Z2_OPENINGDAN` scripts and are not portals in the frozen registry.

## 3. Object banks (HDR r00.c:25-32, r01.c:25-29; IDs from `include/tables/object_table.h`)

| Room | # | Bank | Object ID | Consumer (OVL) | Needed for Day 1 noon |
| --- | --- | --- | --- | --- | --- |
| 00, 01 | 0 | `OBJECT_CTOWER_ROT` | 0x088 | `Bg_Ctower_Gear`, `Bg_Ctower_Rot` | yes |
| 00, 01 | 1 | `OBJECT_TOKEI_TOBIRA` | 0x197 | `Obj_Tokei_Tobira` | yes (already extracted) |
| 00, 01 | 2 | `OBJECT_BB` | 0x051 | no listed actor (users are `En_Bb`, `En_Bbfall`) | no (U-04) |
| 00 | 3 | `OBJECT_OSN` | 0x1A3 | `En_Osn`; `Dm_Char03` (the Deku Mask skeleton lives in `object_osn`) | skeletal HMS (later) |
| 00 | 4 | `OBJECT_GI_NUTSMASK` | 0x1BD | `Dm_Char03` get-item draw via `Object_GetSlot` (`z_dm_char03.c:73-80`) | no (cutscene) |
| 00 | 5 | `OBJECT_DELF` | 0x1C8 | `Dm_Char00` (Tatl/Tael cutscene animations; skeleton `gameplay_keep_Skel_02AF58`) | no (cutscene) |

Implicit: `gameplay_keep` (Obj_Demo and En_Holl profile object, fairy skeleton) and `gameplay_field_keep` (special file). INF: listing banks 0-2 in both rooms keeps them loaded across the room swap.

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3655): ID `id & 0x1FFF`; each rotation `(raw >> 7) & 0x1FF` degrees, unless the ID carries the axis flag (`0x8000` Y, `0x4000` X, `0x2000` Z), which passes that axis as a plain integer. csId `rawRotY & 0x7F`, `0x7F` = none (3570-3574). Half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 = all (3576-3580); spawn filter 2546-2577; Day 1 12:00 bit `0x080` (2491-2499).
- `Actor_Init` sets default scale 0.01 before the overlay init (1171).
- Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled` (skeletal, presence gated by schedule, flag or cutscene), `billboard-effect`, `controller-invisible`, `door-transition`. D1d = mask includes Day 1 daytime.

### 4.2 room_00 default header (12 entries, HDR r00.c:34-71; decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Deg X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_DM_CHAR03` (0x195) | (0, -30, -29) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0000 | object_osn | posed-skeletal (cutscene-only) | yes |
| 1 | `ACTOR_EN_OSN` (0x1B5) | (-106, -30, -79) | 0x0007 / 0x078B / 0x007F | 0 / 15 / 0 | 0x3FF | 11 | 0x0000 | object_osn | npc-scheduled (switch-gated) | yes |
| 2 | `ACTOR_OBJ_TOKEI_TOBIRA` (0x1A2) | (0, 45, 288) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF | none | 0x0000 | object_tokei_tobira | door-transition | yes |
| 3 | `ACTOR_BG_CTOWER_GEAR` (0x1B6) | (0, -30, -200) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0003 organ | object_ctower_rot | static-mesh (cutscene-only) | yes |
| 4 | `ACTOR_BG_CTOWER_GEAR` | (155, 390, 0) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0000 ceiling cog | object_ctower_rot | static-mesh | yes |
| 5 | `ACTOR_BG_CTOWER_GEAR` | (-155, 390, 0) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF | none | 0x0000 ceiling cog | object_ctower_rot | static-mesh | yes |
| 6 | `ACTOR_BG_CTOWER_GEAR` | (0, 80, 0) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0001 center cog | object_ctower_rot | static-mesh | yes |
| 7 | `ACTOR_BG_CTOWER_GEAR` | (0, -270, -56) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0002 water wheel | object_ctower_rot | static-mesh | yes |
| 8 | `ACTOR_BG_CTOWER_ROT` (0x061) | (-45, -300, -710) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | 0x3FF | 9 | 0x0002 stone door | object_ctower_rot | static-mesh | yes |
| 9 | `ACTOR_BG_CTOWER_ROT` | (45, -300, -710) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | 0x3FF | 9 | 0x0001 main stone door | object_ctower_rot | static-mesh | yes |
| 10 | `ACTOR_DM_CHAR00` (0x192) | (0, 45, 218) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF | none | 0x0000 Tatl | object_delf | npc-scheduled (cutscene-only) | yes |
| 11 | `ACTOR_OBJ_DEMO \| 0x6000` (0x0D8) | (0, 60, 290) | 0x1687 / 0x000A / 0x007F | X 45 (integer) / 0 / Z 0 (integer) | 0x3FF | 10 | 0x0000 (switch 0) | gameplay_keep | controller-invisible | yes |

### 4.3 room_01 default header (2 entries, HDR r01.c:31-38; decode C)

| # | Actor | Position | Raw rot X / Y / Z | Deg | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_BG_CTOWER_ROT` (0x061) | (2, -251, -2320) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0000 corridor | object_ctower_rot | static-mesh | yes |
| 1 | `ACTOR_BG_CTOWER_ROT` | (0, -250, -2320) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF | none | 0x0000 corridor | object_ctower_rot | static-mesh | yes |

Two corridor instances 2 u apart, both loading `gClockTowerCorridorCol` and both drawing (C); whether that is visible as z-fighting is U-08.

### 4.4 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init: scale, collision, children | Draw | Presence / state at Day 1 noon, post-intro |
| --- | --- | --- | --- |
| En_Osn (HMS) | flex `gHappyMaskSalesmanSkel` (object_osn 0x202F0), idle anim 0x201BC; default scale 0.01; cylinder r30 h40, OC on, `MASS_IMMOVABLE` (`:120-140, 901-911`); type `params & 3 = 0` `OSN_TYPE_CHOOSE` (`z_en_osn.h:10-17`) | `SkelAnime_DrawFlex`; eye/mouth textures by anim; backpack limb hidden during organ anims; Majora's Mask DL 0x192A0 in the left hand during mask-look anims (`:989-1069`) | drawn and talkable only while switch 0 is set (`:966-984`): yes. Idle anim, head tracks the player (`:986`) |
| Obj_Tokei_Tobira | scale 0.1; dynapoly per leaf, `DYNA_TRANSFORM_POS | ROT_Y`; type 0 spawns the type-1 leaf (`:55-76`) | `DL_001108` (type 0) / `DL_0013E8` (type 1) (`:50-53, 177-179`) | closed at rest; push-swing (2.5) |
| Bg_Ctower_Gear | scale 0.1 (`:117`); type `params & 3` (`.h:9-16`): 0 ceiling cog, 1 center cog, 2 water wheel (dynapoly `gClockTowerWaterWheelCol` 0x18588), 3 organ (dynapoly `gClockTowerOrganCol` 0x16E70, collision disabled, `draw = NULL`) (`:113-135`) | `sDLists`: ceiling cog 0x10828, center cog 0x17018, water wheel 0x18118 (`:63, 186-188`); organ 0x160A0 + XLU pipes 0x15F30 (`:190-201`) | cogs and wheel rotate 0x1F4 binang (2.75 deg) per frame: ceiling cog `rot.x -=`, center cog `rot.y +=` with windmill SFX, wheel `rot.z -=` with splash effects (`:65-111, 146-159`). Organ hidden: shown only on cue 104 id 2, hidden on id 1, killed on id 3 (`:161-184`) |
| Bg_Ctower_Rot | scale 0.1 (`:34-36`); type = params: 0 corridor (`gClockTowerCorridorCol` 0x142E8), 1 main stone door (`gClockTowerStoneDoorMainCol` 0x17410, slides +X), 2 stone door (`gClockTowerStoneDoorCol` 0x17650, slides -X) (`:40-71`; `.h:10-15`) | corridor 0x12DA0 + XLU foliage 0x129D0; doors 0x17220 / 0x174E0 (`:38, 142-149`) | doors closed at home when entered from South (2.6). Corridor roll `rot.z = clamp(1100 - local z of player, 0, 1000) x 16.384` binang, i.e. 90 deg at the `Z2_OPENINGDAN` end to 0 deg 1,000 u later; forces `CAM_SET_DUNGEON0` every frame (`:79-92`) |
| Dm_Char03 (Deku Mask) | flex `gDekuMaskSkel` (object_osn 0x20550), anim `gDekuMaskFallOverAnim` 0x20530, scale 0.01 (`:57-68`) | drawn only while cue 136 is active and not id 1; after id 4 draws `GID_MASK_DEKU` at 0.2 scale, spinning (`:170-198`) | not drawn. Cue 136 id 4 calls `Item_Give(play, ITEM_MASK_DEKU)` (`:120-123`) |
| Dm_Char00 (Tatl) | fairy skeleton `gameplay_keep_Skel_02AF58`, scale 0.01 (`:663-685`); cue type `CS_CMD_ACTOR_CUE_113 + params` (`:691`) | returns early unless a cutscene runs (the exception is `SCENE_OPENINGDAN` room 0 only) (`:1057-1061`) | not drawn; scene-specific SFX in script 0 (`:505-520, 645-649`) |
| Obj_Demo | `params & 0xFF` = switch 0; killed at init if set (`:30-37`); xz range `rot.x x 4 = 180`, y range 60 (`:42-55`) | none (profile draw `NULL`, `:27`) | killed (switch 0 set) |
| En_Holl (TA0) | section 2.3 | black hall plane | present, behind closed doors |

### 4.5 Counts

| | room_00 | room_01 | Transition |
| --- | --- | --- | --- |
| Entries | 12 | 2 | 1 |
| Day 1 daytime by mask | 12 | 2 | 1 (`HALFDAYBIT_ALL`) |
| static-mesh | 7 (a03-a09; a03 cutscene-only) | 2 | |
| posed-skeletal | 1 (a00, cutscene-only) | | |
| npc-scheduled | 2 (a01; a10 cutscene-only) | | |
| controller-invisible | 1 (a11) | | |
| door-transition | 1 (a02 + spawned child) | | 1 |
| Drawn at Day 1 noon, post-intro, entered from South | 8 (a01, a02 + child, a04-a09) | 0 (room not loaded, INF) | hall drawn but occluded by the closed doors (INF) |

## 5. Venue mechanics index (INT-16 scope)

INT-16 scope: interior geometry, mechanisms, entry/return and presentation; preserve the post-introduction profile; the introductory story and character behavior are separate scoped scenarios. This section indexes; it does not design the Unity implementation.

### 5.1 Happy Mask Salesman state machine (OVL `z_en_osn.c`)

| Step | Lines | Condition | Result |
| --- | --- | --- | --- |
| Init, type 0 | 913-934 | `save.entrance` is interior entrance 2, 3 or 6 | sets `OSN_STATE_SPECIAL_CONVERSTATION` |
| | | in `SCENE_INSIDETOWER`: entrance 2 or 6 | `EnOsn_HandleCutscene` (driven by the spawn cutscene) |
| | | entrance 3 | `EnOsn_InitCutscene` then `EnOsn_StartCutscene` |
| | | any other entrance | `EnOsn_ChooseAction` |
| ChooseAction | 705-716 | switch 0 clear / set | wait for cue 130 (`HandleCutscene`) / `EnOsn_Idle` |
| Update | 966-984 | switch 0 set / clear | attention on, collider, drawn / `draw = NULL`, attention off |
| Idle | 718-737 | Ocarina in `items[SLOT_OCARINA]` and **not** `CHECK_QUEST_ITEM(QUEST_SONG_HEALING)` | talk offer (xz < 100 or locked on, facing +-90 deg) starts **cs 11 = script 1** (Song of Healing lesson) |
| | | otherwise | talk offer -> `EnOsn_GetInitialText` -> `EnOsn_Talk` |
| InitCutscene | 692-703 | no Ocarina, no Deku Mask / no Ocarina, has Deku Mask / has both / Ocarina only | cs 17 (script 7) / cs 13 (script 3) / cs 13 (script 3) / cs 11 (script 1) |
| HandleCutscene | 751-881 | cue 130 present | cue id -> anim (1 bow, 2 arms out, 3 shake head, 4 reminisce, 5 think, 6 believe, 7 hands clasped, 8 idle, 10 organ talk, 11 organ play, 13 shake, 15 choke, 16 despair, 17 fast bows, 18 hand out, 19-24 mask look, turn, walk away, fade-out kill); `Cutscene_ActorTranslateAndYaw` |
| | | no cue | `ChooseAction` |
| GetInitialText | 441-531 | needs Ocarina **and** Song of Healing, else `0x1FAE` (`0x2004` on Day 3 05:00-06:00) | special conversation: `0x1FCD` / `0x2006` if Ocarina and Deku Mask, else `0x1FAF`. Deku form: first `0x1FC8`, then `0x1FCD`. Goron: `0x1FCE` once (sets `WEEKEVENTREG_76_20`), then `0x1FC8`. Zora: `0x1FD0` once (sets `WEEKEVENTREG_76_40`). Human, no mask: first `0x1FC8`. Mask worn: `EnOsn_GetInitialMaskText` (`0x1FD1`/`0x1FD2` once per mask per actor instance, `maskTextFlags`, `:289-439`) |
| HandleConversation / Talk | 533-690, 883-896 | text chains `0x1FC8`..`0x1FCC`, masks `0x1FD6`..`0x1FFE` -> `0x1FD3`/`0x1FD4`/`0x1FD5`; Day 3 05:00-06:00 swaps `0x1FCB` for `0x2007` | ends back in `Idle` |

### 5.2 Cutscene index (HDR scene.c:101-1891 scripts, :1882-1891 script list, :1893-1912 actor cutscenes)

`CutsceneEntry { priority, length, csCamId, scriptIndex, additionalCsId, endSfx, customValue, hudVisibility, endCam, letterboxSize }` (`include/z64cutscene.h:626-637`); script entry `{ script, nextEntrance, spawn, spawnFlags }` (`:602-607`). A default destination copies `nextEntrance` and turns its layer into `nextCutsceneIndex = layer + 0xFFEF` (`src/code/z_demo.c:585-619`). `CS_MISC_STOP_CUTSCENE` ends the script at its frame (`z_demo.c:192-196`).

| csId (row) | Script (data, line) | Frames / ends | Started by | Key contents (HDR) | Destination |
| --- | --- | --- | --- | --- | --- |
| 9 | none; csCam 0 = `CAM_SET_FIXED1` (31) at (-132, -62, -431) (scene.c:1923-1931) | - | stone doors a08/a09 closing (`z_bg_ctower_rot.c:120, 128`) | - | - |
| 10 | 0 `CutsceneData_000220` (102) | 980 / 600 | `Obj_Demo` a11 when the player is within 180 u xz and 60 u Y of (0, 60, 290) | HMS cue 130 x11; Tatl cue 113 x6; En_Elf cue 201; texts `0x1FA4`-`0x1FAD`; BGM `NA_BGM_HAPPY_MASK_SALESMAN` at 300, `NA_BGM_CLOCK_TOWER` at 599 | none |
| 11 (-> 17 -> 13) | 1 `0011D0` (452) | 1148 / destination 475 | HMS talk with Ocarina, no Song of Healing; entrance 3 with Ocarina, no mask | HMS cues 10/11 organ talk/play; organ cue 104 id 2 at frame 79; texts `0x1FB0`-`0x1FB2`, `OCARINA_ACTION_DEMONSTRATE_HEALING`, `OCARINA_ACTION_PROMPT_HEALING`; `CS_MISC_PLAYER_FORM_HUMAN` at 470 (sets Human, removes mask, `z_demo.c:314-318`); black fill 460-470 | `0x1C01` = `SPOT00` entr 0 layer 1 |
| 12 | 2 `001C10` (687) | 3810 / destination 770 | entrance 2 (spawn 2, ALWAYS) | Deku Mask cue 136: id 4 at frame 310 gives `ITEM_MASK_DEKU`; `NA_BGM_GET_NEW_MASK` at 330; HMS cues incl. 15 choke, 16 despair; texts `0x1FB3`, `0x1FB4`, `0x0078`, `0x1FB5`-`0x1FBE` | `0x1C19` = `SPOT00` entr 1 layer 9 |
| 13 | 3 `003100` (1164) | 1527 / 70 | En_Osn chain (has the Deku Mask) | text `0x1FAF`; HMS cue 7; En_Elf cue | none |
| 14 | 4 `0033C0` (1232) | 257 / 216 | entrance 0 (spawn 0, ALWAYS) | player cues only; motion blur 1-181 | none |
| 15 | 5 `003540` (1274) | 669 / destination 70 | entrance 4 (spawn 4, ALWAYS) | white fills 0-6 and 60-70; light settings 0x09 then 0x08; motion blur 60-100; HMS cue x14 | `0xD8A0` = South spawn 10 (layer 0) |
| 16 | 6 `003B30` (1400) | 722 / 365 | entrance 6 (spawn 6, ALWAYS) | black fill in/out 0-20; texts `0x1FC4`-`0x1FC7`, `0x1FFF`-`0x2002`; HMS cues incl. 16, 17, 18 | none |
| 17 (-> 13) | 7 `0045D0` (1646) | 1322 / 440 | En_Osn chain (no Ocarina, no mask) | texts `0x1FA5`-`0x1FAD` (script 0's speech without `0x1FA4`); light settings 0x09, 0x08, 0x0A | none |
| 0-8 | none; csCam -3 .. -11 (global camera data), chained 0 -> 1 -> ... -> 8 | - | no actor in either list uses them (C); same nine rows open South's list (`Z2_CLOCKTOWER.c:1188-1197`) | - | - |

Chains found by the C-BIN scan: script 1 -> `SPOT00` L1 script 0 -> **entrance 2** (script 2) -> `SPOT00` L9 spawn 1 -> `0x2C01` (rooftop entr 0, layer 1). `SPOT00` L9 spawn 0 -> entrance 3, spawn 2 -> entrance 6; who reaches those two `SPOT00` spawns is U-01. `Z2_OPENINGDAN` script 3 (its spawn 4) -> entrance 4 -> South spawn 10, where South's own script 0 runs (ALWAYS, `south_clock_town.md` 2.1).

### 5.3 Intro versus post-introduction: what selects each

| State | Selector (source) | Interior result |
| --- | --- | --- |
| Deku-form intro arrival | entrance 0 from `Z2_OPENINGDAN` exit [1] (C-BIN) | room_01, script 4; corridor roll and camera; stone doors open behind, close with cs 9 (2.6) |
| First meeting ("intro talk") | switch 0 clear; player reaches the door steps (Y 0..120 inside 180 u of (0, 60, 290)); no form or item test (`z_obj_demo.c:66-104`) | cs 10 = script 0; `Obj_Demo` sets switch 0 when the cutscene starts (its additional csId is -1) and kills itself (`:92-97`); HMS becomes drawn (`z_en_osn.c:975-983`) |
| Song of Healing lesson | Ocarina held, Song of Healing not held; talk to HMS (`z_en_osn.c:721-729`) | cs 11 = script 1 -> `SPOT00` -> entrance 2 -> script 2 (Deku Mask) |
| Return cutscenes | entrances 3, 4, 6 (C-BIN senders) | 5.1 / 5.2 |
| **Post-introduction idle** | any arrival with switch 0 set and no spawn cutscene (entrance 1 or 5); Ocarina/Healing/form only change the talk branch | HMS idle at (-106, -30, -79), yaw 15; `Obj_Demo` gone; Tatl, Deku Mask and organ hidden |

Persistence: switch 0 is cycle-persistent (section 1). The debug/title save sets exactly this state: `cycleSceneFlags[SCENE_INSIDETOWER].switch0 = 1` and the permanent copy, with `WEEKEVENTREG_15_20`, `59_04`, `31_04` (`src/code/z_sram_NES.c:1133-1166`), a source example of a post-intro save. Where the Song of Healing quest bit is granted is U-03.

### 5.4 Baseline profile (`prof.d1.noon.normal`, post-introduction)

Default scene and room headers (the only ones), `sceneLayer` 0, arrival by entrance 1 (`0xC010`, from South), SCENE_INSIDETOWER switch flag 0 set, `respawnFlag` 0, player form Human, half-day bit `0x080`. Time speed 0 holds the clock at the entry time (12:00) for the whole visit; lighting is by setting index, not time (section 1), so every `prof.*` shows the same interior except En_Osn's Day 3 05:00-06:00 texts. Ocarina and Song of Healing follow the owner's choice (SOURCE_REGISTER U-07): with the Ocarina but without the song, talking starts cs 11; either way HMS is drawn idle. The venue is open at Day 1 noon: no door schedule exists on either side (2.5).

### 5.5 Mechanisms, presentation and constants index

| Item | Implementation | Citation |
| --- | --- | --- |
| Gears and wheel | 0x1F4 binang per frame on rot.x / rot.y / rot.z; wheel splash spawns in rot.z windows `0x1388..0x1B58` and `< 0x1F4`, SFX `NA_SE_EV_WATERWHEEL_LEVEL`; center cog `NA_SE_EV_WINDMILL_LEVEL` | `z_bg_ctower_gear.c:65-111, 146-159` |
| Organ | hidden, collision off; cue 104 id 2 (script 1, frame 79) shows it and enables `gClockTowerOrganCol` | `z_bg_ctower_gear.c:120-134, 161-184` |
| Stone doors | open only if the player starts on their -Z side; close over `Math_SmoothStepToF` with SFX `NA_SE_EV_STONE_STATUE_OPEN` / `STONEDOOR_STOP`, cs 9 | `z_bg_ctower_rot.c:54-70, 97-134` |
| Corridor | roll from player local z; `CAM_SET_DUNGEON0`; SFX `NA_SE_SY_SPIRAL_DASH` at cutscene frame 132 | `z_bg_ctower_rot.c:79-92` |
| Front leaves | push-swing, SFX `NA_SE_OC_WOOD_GATE_OPEN` | `z_obj_tokei_tobira.c:102-175` |
| Room swap | `EN_HOLL` type 0 planes 200 / 150 / 100 / 50 | `z_en_holl.c:85-94, 146-200` |
| Cameras | `BgCamInfo` 0 = `0x003F` `CAM_SET_ROOM0` ("inside the clock tower"), 1 = `0x0000` `CAM_SET_NONE`; spawns bgCam 0xFF; cs 9 `CAM_SET_FIXED1` | scene.c:1946-1949; `include/z64camera.h:147, 164, 178, 210` |
| Music / echo / sky | seq `NA_BGM_CLOCK_TOWER`; echo 10; sky and sun/moon disabled | scene.c:15; r00.c:15, :17 |
| Lighting | `LIGHT_MODE_SETTINGS`; the floor surface picks the setting (`SurfaceType_GetLightSettingIndex`); all floor types here carry light 0 | scene.c:23; `src/code/z_bgcheck.c:4265`; `z_player.c:11730` |
| Clock | scene time speed 0 (no first-cycle speed 5, because 0 stays 0) | `src/code/z_scene.c:387-406` |
| Save fields read | `save.entrance`, `save.day`, `CURRENT_TIME`, `inventory.items[SLOT_OCARINA]`, quest `QUEST_SONG_HEALING`, `INV_CONTENT(ITEM_MASK_DEKU)`, `Player_GetMask`, `player->transformation`, `sceneLayer` (Dm_Char00) | `z_en_osn.c:435-531, 692-703, 913-934`; `include/z64save.h:533, 553` |
| Flags written | SCENE_INSIDETOWER switch 0 (`Obj_Demo`); `WEEKEVENTREG_76_20`, `76_40` (En_Osn); `ITEM_MASK_DEKU` (Dm_Char03); player form Human (script 1); South's `WEEKEVENTREG_59_04` belongs to the South door | `z_obj_demo.c:94-96`; `z_en_osn.c:486, 505`; `z_dm_char03.c:121`; `z_demo.c:314-318` |
| Rupees, shop items, highscores | none in this venue (C: no `Rupees_`, `HS_` or shop actor in the 9 overlays) | grep of the overlays |

### 5.6 Mask shop props

No mask display, shelf or pedestal actor exists in either room list (C, r00.c:34-71, r01.c:31-38). The mask visuals in this venue are: the HMS backpack limb (hidden during organ anims, `z_en_osn.c:996-999`); Majora's Mask in his left hand during mask-look anims (`gHappyMaskSalesmanMajorasMaskDL` object_osn 0x192A0, `:1012-1024`); the cutscene Deku Mask a00 (`gDekuMaskSkel`) and its get-item model (`gGiDekuMaskDL` object_gi_nutsmask 0xB50). Any masks modeled into the room DLs are NI (U-11).

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, post-intro, default headers)

Room_00 only (room_01 is behind the closed stone doors, INF). World placement as South/North: `world = actorPos + actorRot x (actorScale x localVertex)`. Unity rotation conversion under the Z mirror belongs to the importer.

| # | Component / params | N64 position (Unity) | Yaw | Scale | Display list(s), bank, offset | Collision | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| room_00 | room shape, 15 cullable entries | origin | - | 1 | opaque wrapper DL_006B10 (12 DLs), XLU wrapper DL_00C9E0 (5 DLs) | scene collision (section 7) | water surface DL_00C8F8 at Y -330 uses the segment-8 scroll |
| a02 + child | front leaves `0x0000` / type 1 | pivots (80, 45, 288) (0.80, 0.45, -2.88) and (-80, 45, 288) (-0.80, 0.45, -2.88) | 180 | 0.1 | `object_tokei_tobira` 0x1108 / 0x13E8 | Colheader 0x12B0 / 0x1590 | closed rest pose |
| a04 | ceiling cog `0x0000` | (155, 390, 0) (1.55, 3.90, 0.00) | 0 | 0.1 | `gClockTowerCeilingCogDL` 0x10828 | none | rot.x frozen at 0 (approximation) |
| a05 | ceiling cog `0x0000` | (-155, 390, 0) (-1.55, 3.90, 0.00) | 180 | 0.1 | same | none | same |
| a06 | center cog `0x0001` | (0, 80, 0) (0.00, 0.80, 0.00) | 0 | 0.1 | `gClockTowerCenterCogDL` 0x17018 | none | rot.y frozen |
| a07 | water wheel `0x0002` | (0, -270, -56) (0.00, -2.70, 0.56) | 0 | 0.1 | `gClockTowerWaterWheelDL` 0x18118 | `gClockTowerWaterWheelCol` 0x18588 | rot.z frozen; no splash |
| a08 | stone door `0x0002` | (-45, -300, -710) (-0.45, -3.00, 7.10) | 0 | 0.1 | `gClockTowerStoneDoorDL` 0x174E0 | `gClockTowerStoneDoorCol` 0x17650 | closed at home |
| a09 | main stone door `0x0001` | (45, -300, -710) (0.45, -3.00, 7.10) | 0 | 0.1 | `gClockTowerStoneDoorMainDL` 0x17220 | `gClockTowerStoneDoorMainCol` 0x17410 | closed at home |

**Present but needs skeleton/pose (optional):** a01 HMS at (-106, -30, -79) (-1.06, -0.30, 0.79), yaw 15, scale 0.01, `gHappyMaskSalesmanSkel` 0x202F0 + `gHappyMaskSalesmanIdleAnim` 0x201BC; cylinder r30 h40. TA0 hall plane (0, -300, -905) (0.00, -3.00, 9.05), `gEnHollCentralPlaneDL`, occluded.

**Excluded by state (not drawn at noon, post-intro):** a00 Dm_Char03, a03 organ, a10 Dm_Char00 (cutscene-only); a11 Obj_Demo (killed). **Excluded by half-day mask:** none. **Not loaded:** room_01 a00/a01 corridors (`gClockTowerCorridorDL` 0x12DA0 + XLU `gClockTowerCorridorFoliageDL` 0x129D0; collision 0x142E8); for a room_01 capture use an entrance-0 arrival (script 4 plays first) with the corridor roll at the camera's z.

**No geometry:** a11 controller; Tatl companion (En_Elf, not in the lists).

## 7. Collision summary (C from scene.c:1946-2970; scratch `col.py`, `col2.py`, `col3.py`, `col4.py`)

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 441 (438 referenced; 420-422 are unreferenced water-box corners) / 553 / 13 |
| Header bounds | min (-1665, -1379, -2500), max (1665, 720, 1465) (scene.c:2962-2970) |
| Referenced vertex extents | x -1065..1065, Y -420..585, z -2500..445; Unity x -10.65..10.65, y -4.20..5.85, z -4.45..25.00 |
| Polygon kinds | 143 floors (normal Y > 0.5), 336 walls, 74 ceilings |
| By room side (hall plane z -905, INF) | room_00 side 137 / 328 / 70 (x -1065..1065, z -905..445); room_01 side 6 / 8 / 4 (x -60..71, z -2500..-905) |
| Floor slope histogram (5-deg bins) | 0: 97, 5: 10, 10: 7, 15: 3, 20: 1, 25: 22, 30: 2, 35: 1 |
| Materials (per poly) | stone 440, dirt 59, wood 54 |
| Water box | 1: min (-765, -330, -135), x 1600, z 1600, props `0x00001F01` = bgCam 1, light 0x1F (none), room 0, enabled (scene.c:2958-2960; `include/z64bgcheck.h:34-46`) |
| Exit surfaces | 2 types, 4 floors (2.4) |
| Conveyors | type 1: 6 channel-bed floors at Y -420, x -1065..1065, z -105..105, speed 2, direction 48 (48 x 1024 = 0xC000); type 5 exit floors speed 3 direction 0; vIB conveyor flag only on 382, 383 |
| Other flags | wall type 1 on types 2 and 3 (20 walls, door-step sides x +-80, z 90..270); bgCam 1 (`CAM_SET_NONE`) on 63 walls/ceilings (types 3, 6, 8, 11); 50 polys ignore the camera (vIA flag 1); no hookshot, floor property, floor type, soft or horse flags |
| Camera data | 2 `BgCamInfo` (5.5) |

| Surface type | data[0] / data[1] | Polys | Use |
| --- | --- | --- | --- |
| 0 | 0x0 / 0x1002 | 411 (94 F, 247 W, 70 C) | stone, general |
| 1 | 0x0 / 0x06081002 | 6 F | channel bed, conveyor |
| 2, 3 | 0x200000 / 0x1002; 0x200001 / 0x7C0 | 8 W; 12 W | door-step side walls (wall type 1; type 3 also bgCam 1, dirt) |
| 4 | 0x0 / 0x1000 | 24 F | dirt ramps, x -195..195, z -665..-270 |
| 5 | 0x200 / 0x000C1002 | 2 F | exit to South |
| 6, 8, 11 | 0x1 / 0x7C0; 0x1 / 0x17C2; 0x1 / 0x7CA | 21 (17 W, 4 C); 11 W; 19 W | bgCam 1 walls (dirt, stone, wood) |
| 7 | 0x0 / 0x0 | 2 F | x 110..130, Y -255..-183 |
| 9, 10 | 0x0 / 0x100A; 0x0 / 0xA | 34 (12 F, 22 W); 1 F | wood |
| 12 | 0x100 / 0x1002 | 2 F | exit to `Z2_OPENINGDAN` |

Floor elevation bands (centroid Y; x/z are band unions, not one platform):

| Band | Floor polys | x | Y | z | Max slope | Types |
| --- | --- | --- | --- | --- | --- | --- |
| channel bed | 6 | -1065..1065 | -420 | -105..105 | 0 | 1 |
| channel banks | 24 | -1065..1065 | -315..-300 | -150..150 | 27 | 0 |
| base floor | 22 | -1065..1065 | -300 | -905..270 | 0 | 0 |
| corridor stubs (room_01 side) | 6 | -60..71 | -296 | -2500..-905 | 0 | 0, 12 |
| ramps -285..-235 | 44 | -270..270 | -300..-217 | -665..270 | 35 | 0, 4, 9 |
| wood structure -230..-150 | 15 | -270..250 | -255..-150 | -150..270 | 16 | 0, 7, 9, 10 |
| wood ramp -110..-70 | 2 | -270..-150 | -150..-30 | -90..150 | 27 | 9 |
| central platform | 16 | -270..270 | -30..15 | -270..270 | 27 | 0 |
| door steps and landing | 8 | -80..80 | 15, 30, 45 | 150..445 | 0 | 0, 5 |

Actor collision: dynapolys of a02 + child, a07, a08, a09 (and a03 when shown, room_01 a00/a01); En_Osn cylinder r30 h40. Special: water channel under the base floor (swim/dive behavior NI); conveyor channel bed; no ladders, hookshot targets or void planes found in the surface data (C).

Proposed camera anchors for the registered IDs (proposals only, REF-03 owns them): `cam.clock_tower_interior.entry` from spawn 1 (1, 30, 182) looking -Z over the platform; `cam.clock_tower_interior.reverse_door` from (0, -30, 64) toward the leaves (0, 45, 288); `cam.clock_tower_interior.fixture` on HMS (-106, -30, -79) with the center cog (0, 80, 0) and water wheel in frame; `cam.clock_tower_interior.room_01` from spawn 0 (11, -296, -2317) looking +Z, which needs an entrance-0 arrival (2.6). The entry room is room_00, so no camera rename is needed (SOURCE_REGISTER 4.6).

## 8. Materials and textures (C; counts and names only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 20 (17 textures + 3 TLUTs), all in the room_00 file; scene file and room_01 hold none |
| By format | ci8 7 (Tex_006B78, 006F78, 007378, 008778, 00A378, 00A978, 00B178), ci4 5 (007778, 007F78, 008B78, 009378, 009B78), rgba32 2 (00CA10, 00DA10), ia16 2 (00EA10, 00F210), ia4 1 (00A578); TLUTs rgba16 3 (00B978, 00B998, 00BB98) |
| room_00 DLs / Vtx / Mtx | 23 `Gfx` (incl. 2 XML wrappers), 19 `Vtx`, 4 `Mtx` (used by DL_00BF50..00C370 under XLU DL_00C420) |
| room_01 | 2 `Gfx`, 1 `Vtx`, 1 triangle, no texture |
| Raw triangle commands | room_00 684, room_01 1 (DL reuse not resolved) |
| Animated material | segment 8 two-layer scroll, referenced once (r00.c:1109, water DL_00C8F8) |
| Actor banks (XML counts) | `object_ctower_rot` 42 textures incl. 5 TLUTs (ci8 13, rgba16 9, ia16 7, i4 4, ci4 3, ia8 3, ia4 2, i8 1), 12 DLs; `object_osn` 50 textures, 19 DLs, 23 animations; `object_gi_nutsmask` 3; `object_tokei_tobira` 2 (extracted) |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_INSIDETOWER` (C above); `object_tokei_tobira` (for South). Binaries for every bank below exist under `extracted/n64-us/baserom/` (checked by `ls`).

| Priority | Banks / files | Why |
| --- | --- | --- |
| Day 1 static | `object_ctower_rot` | cogs, wheel, stone doors (and corridor, organ) DLs and collision |
| Later skeletal pass | `object_osn` | HMS skeleton, idle anim, Majora's Mask DL, Deku Mask skeleton |
| Cutscene-only | `object_gi_nutsmask`, `object_delf`, `gameplay_keep` (fairy skeleton) | Deku Mask get-item, Tatl |
| Hall plane | `assets/xml/overlays/ovl_En_Holl.xml` | `gEnHollCentralPlaneDL` 0xC30 |
| Source questions only | scenes `Z2_OPENINGDAN`, `SPOT00` | confirm the C-BIN senders (U-01, U-14) |
| Skip | `object_bb` | no consumer (U-04) |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_ctower_rot','object_osn','object_gi_nutsmask','object_delf')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
foreach ($scene in @('Z2_OPENINGDAN','SPOT00')) {
    & $zapd e -i "assets/xml/scenes/$scene/$scene.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/scenes/$scene" -osf "extracted/n64-us/assets/scenes/$scene" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $scene" }
}
```

The overlay XML (`ovl_En_Holl`) and `gameplay_keep` have not been run through this tool before (NI): check output path, size and warnings separately.

## 10. Proposed region record

Schema 1 (PIPE-01, `shared/city/schema/region.schema.json`), written directly; the integrator lifts and validates it.

```json
{
  "schemaVersion": 1,
  "regionId": "loc.clock_tower_interior",
  "displayLabel": "Clock Tower Interior",
  "source": {
    "sceneSymbol": "Z2_INSIDETOWER",
    "sceneEnum": "SCENE_INSIDETOWER",
    "sceneTableIndex": "0x63",
    "entranceTableSceneIndex": "0x60",
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
      "post-introduction: SCENE_INSIDETOWER switch flag 0 set (cycle-persistent; Obj_Demo killed, En_Osn drawn)",
      "arrival by entrance 1 (0xC010, from South): stone doors a08/a09 closed at home, room_01 not loaded",
      "player form Human",
      "scene time speed 0: clock frozen at the entry time (12:00)",
      "Ocarina and Song of Healing per owner choice (SOURCE_REGISTER U-07); changes En_Osn talk only"
    ]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_INSIDETOWER", "bytes": 32576, "sha256": "3c0d865934f5c730058638b2965b0ebde08911138401e4c859c440e26f7ce6b4" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_INSIDETOWER_room_00", "bytes": 64016, "sha256": "5a76e16b03958d39d51b05032395795b638368e326d2423bc13382c7a16d0a41" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_INSIDETOWER_room_01", "bytes": 432, "sha256": "75c75307607646e8c0a547310657b7217f8c876871e6608d7de2f3cae39b7440" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_INSIDETOWER/Z2_INSIDETOWER.c", "bytes": 206209, "sha256": "ee9c6384a5bb07dd4352f5414693bb3898b033a5809ff83ea2340483fc48ccd9" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_INSIDETOWER/Z2_INSIDETOWER_room_00.c", "bytes": 57206, "sha256": "57c46092e6a649ad0f74d120d62249e0dfdc0ba440c78dd1c70677f52a44b783" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_INSIDETOWER/Z2_INSIDETOWER_room_01.c", "bytes": 3172, "sha256": "714f71190de50ed1cc85698cf7392a55b685a5b16aeada2cbc4e8691a40535d4" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_INSIDETOWER/Z2_INSIDETOWER.xml", "bytes": 631, "sha256": "e9a6d98a69790b10b0d67f1ddadbb8a8d434d2043c60528bccfafeb1e33cc2ba" }
  ],
  "sourceFrame": {
    "units": "N64 world units",
    "up": "+Y",
    "sharedFrame": {
      "with": ["loc.south_clock_town"],
      "status": "not_shared",
      "evidence": "interior door a02 at (0, 45, 288) vs South door a19 at (-280, 0, -807); the scenes are joined only by exit 0xD800 / entrance 0xC010 (C)"
    }
  },
  "conversionToUnity": {
    "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]],
    "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100"
  },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-1665, -1379, -2500], "max": [1665, 720, 1465] },
    "sourceCollisionVertices": { "min": [-1065, -420, -2500], "max": [1065, 585, 445] },
    "unityCollisionVertices": { "min": [-10.65, -4.2, -4.45], "max": [10.65, 5.85, 25.0] }
  },
  "water": [
    {
      "id": "water.clock_tower_interior.channel",
      "sourceWaterBox": { "xMin": -765, "xMax": 835, "zMin": -135, "zMax": 1465, "surfaceY": -330, "properties": "0x00001F01", "bgCamIndex": 1, "lightIndex": 31, "room": 0, "disabled": false },
      "unity": { "x": [-7.65, 8.35], "z": [-14.65, 1.35], "surfaceY": -3.3 },
      "poolFloorY": [-420, -420],
      "bankFloorY": [-315, -300],
      "surfaceMesh": "room_00 DL_00C8F8 at Y -330, two-layer scroll on segment 8",
      "behavior": "UNKNOWN: gameplay decision; channel bed type 1 is a conveyor (speed 2, direction 48)"
    }
  ],
  "anchors": [
    { "id": "anchor.cti.spawn1_from_south", "source": [1, 30, 182], "unity": [0.01, 0.3, -1.82], "status": "proposed" },
    { "id": "anchor.cti.door_a02", "source": [0, 45, 288], "unity": [0.0, 0.45, -2.88], "status": "proposed" },
    { "id": "anchor.cti.hms_a01", "source": [-106, -30, -79], "unity": [-1.06, -0.3, 0.79], "status": "proposed" },
    { "id": "anchor.cti.spawn5_base_floor", "source": [0, -300, -604], "unity": [0.0, -3.0, 6.04], "status": "proposed" },
    { "id": "anchor.cti.spawn0_corridor", "source": [11, -296, -2317], "unity": [0.11, -2.96, 23.17], "status": "proposed", "note": "room_01" }
  ],
  "rooms": [
    { "id": "loc.clock_tower_interior.room_00", "sourceRoom": "Z2_INSIDETOWER_room_00", "header": 0, "roomShape": "cullable, 15 entries", "displayLists": 23, "role": "tower chamber, entry room from South (spawns 1-6)" },
    { "id": "loc.clock_tower_interior.room_01", "sourceRoom": "Z2_INSIDETOWER_room_01", "header": 0, "roomShape": "cullable, 1 entry (one placeholder triangle)", "displayLists": 2, "role": "twisting corridor from Z2_OPENINGDAN (spawn 0); closed off post-intro" }
  ],
  "components": [
    { "sourceKey": "Z2_INSIDETOWER/h0/room_00/a02", "actorId": "0x1A2", "params": "0x0000", "class": "door-transition", "dls": ["object_tokei_tobira:0x1108", "object_tokei_tobira:0x13E8"], "status": "selected", "note": "with spawned type-1 leaf; push swing not modeled" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_00/a04", "actorId": "0x1B6", "params": "0x0000", "class": "static-mesh", "dls": ["object_ctower_rot:0x10828"], "status": "selected", "note": "ceiling cog; rotation frozen" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_00/a05", "actorId": "0x1B6", "params": "0x0000", "class": "static-mesh", "dls": ["object_ctower_rot:0x10828"], "status": "selected", "note": "ceiling cog, yaw 180; rotation frozen" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_00/a06", "actorId": "0x1B6", "params": "0x0001", "class": "static-mesh", "dls": ["object_ctower_rot:0x17018"], "status": "selected", "note": "center cog; rotation frozen" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_00/a07", "actorId": "0x1B6", "params": "0x0002", "class": "static-mesh", "dls": ["object_ctower_rot:0x18118"], "status": "selected", "note": "water wheel; rotation and splashes frozen" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_00/a08", "actorId": "0x061", "params": "0x0002", "class": "static-mesh", "dls": ["object_ctower_rot:0x174E0"], "status": "selected", "note": "stone door closed at home (arrival from South)" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_00/a09", "actorId": "0x061", "params": "0x0001", "class": "static-mesh", "dls": ["object_ctower_rot:0x17220"], "status": "selected", "note": "main stone door closed at home" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_00/a01", "actorId": "0x1B5", "params": "0x0000", "class": "npc-scheduled", "dls": ["object_osn:0x202F0 gHappyMaskSalesmanSkel", "object_osn:0x201BC idle anim"], "status": "optional", "note": "drawn only with switch 0 set; skeletal pose needed" },
    { "sourceKey": "Z2_INSIDETOWER/h0/transition/ta00", "actorId": "0x018", "params": "0x007F", "class": "door-transition", "dls": ["ovl_En_Holl:0xC30 gEnHollCentralPlaneDL"], "status": "optional", "note": "room hall; occluded by the closed stone doors in this profile" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_01/a00", "actorId": "0x061", "params": "0x0000", "class": "static-mesh", "dls": ["object_ctower_rot:0x12DA0", "object_ctower_rot:0x129D0"], "status": "optional", "note": "corridor; room_01 loads only on an entrance-0 arrival; roll depends on player z" },
    { "sourceKey": "Z2_INSIDETOWER/h0/room_01/a01", "actorId": "0x061", "params": "0x0000", "class": "static-mesh", "dls": ["object_ctower_rot:0x12DA0", "object_ctower_rot:0x129D0"], "status": "optional", "note": "duplicate corridor 2 u from a00" }
  ],
  "collisionOwnership": {
    "room": "Z2_INSIDETOWERCollisionHeader_007F10 (553 polys, 13 surface types; one scene mesh for both rooms)",
    "waterBoxes": 1,
    "actorDynapoly": ["a02 + child object_tokei_tobira_Colheader_0012B0/001590", "a07 gClockTowerWaterWheelCol", "a08 gClockTowerStoneDoorCol", "a09 gClockTowerStoneDoorMainCol", "a03 gClockTowerOrganCol (disabled until cue 104 id 2)", "room_01 a00/a01 gClockTowerCorridorCol"],
    "actorCylinders": ["a01 En_Osn r30 h40"]
  },
  "portalIds": [
    "portal.clock_tower_interior.south_clock_town",
    "portal.clock_tower_interior.z2_openingdan",
    "portal.clock_tower_interior.clock_tower_interior.r0_r1",
    "portal.clock_tower_interior.clock_tower_interior.r1_r0"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a01 En_Osn skeletal pose", "Tatl companion (En_Elf) and Link", "gear, wheel and corridor animation", "water texture scroll"],
  "excluded": {
    "halfDayMask": [],
    "stateOrSchedule": ["a00 Dm_Char03 (cue 136 only)", "a03 organ (cue 104 only)", "a10 Dm_Char00 (cutscene only)", "room_01 a00/a01 (room not loaded)"],
    "noGeometry": ["a11 Obj_Demo controller (killed by switch 0)"]
  },
  "unsupported": ["skeletal NPC pose", "push-swing door leaves", "stone door open/close and cs 9", "corridor roll driven by player position", "cutscene scripts 0-7", "conveyor floors (exit and channel)", "water box swim/dive behavior", "scene time speed 0 (clock frozen)", "EN_HOLL room loading"],
  "evidence": ["docs/city/interiors/clock_tower_interior.md"],
  "readiness": {
    "sourceInspection": "pass",
    "objectExtraction": "not_run",
    "import": "not_run",
    "controllerTraversal": "not_run",
    "editorRender": "not_run",
    "packagedRender": "not_run",
    "visualApproval": "not_run"
  },
  "readinessNotes": { "sourceInspection": "senders of entrances 0, 2, 3, 4, 6 come from a scratch baserom header scan (dossier 2.1, C-BIN)" }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Senders of entrances 2, 3, 4, 6 are known only from a binary header scan; who reaches `SPOT00` layer 9 spawns 0 and 2; entrance 5 has no sender | Extract `SPOT00` and `Z2_OPENINGDAN` (section 9); read their actor-cutscene lists for scripts with spawn 0/2 (layer 9) and 4; grep `src/` and all extracted scenes for `0xC050` and `Entrance_CreateFromSpawn(5)` |
| U-02 | Whether the closed stone doors seal the passage, so room_01 never loads post-intro | Extract `object_ctower_rot`; read `gClockTowerStoneDoorCol` 0x17650 / `MainCol` 0x17410 extents against the passage (x -60..60, Y -300..-180) and the hall activation distance 200 u from z -905 |
| U-03 | Where `QUEST_SONG_HEALING` is granted | Read the ocarina teach/prompt flow in `src/code/z_message.c` for `OCARINA_ACTION_PROMPT_HEALING` (script 1 text list) |
| U-04 | `OBJECT_BB` has no consumer in either list | Search for runtime spawns of `ACTOR_EN_BB` / `ACTOR_EN_BBFALL` reachable in this scene; otherwise record as unused |
| U-05 | The exit floor starts at z 270, 8 u in front of the closed leaf faces (z 278) | Read the player wall/collider radius in `z_player.c` (`ageProperties` wall check) to see whether the exit fires before the leaves swing |
| U-06 | Player start modes 0xD and 0xF (spawns 0-6) | Read the start-mode function table in `Player_Init` (`z_player.c`) |
| U-07 | Story purpose of scripts 2, 3, 5, 6, 7 beyond the source facts | Look up text IDs `0x1FA4`-`0x2007` and `0x0078` in the message table (names only) |
| U-08 | Two overlapping corridor actors in room_01 | Capture room_01 on an entrance-0 arrival; check for z-fighting or a hidden pairing rule |
| U-09 | Meaning of restriction value 3 | LOCATION_REGISTER U-13 (`z_parameter.c` users of the restriction table) |
| U-10 | Water channel: conveyor flow direction and whether the player can enter it from the base floor | Read conveyor handling in `z_bgcheck.c`/`z_player.c`; check bank edge connectivity (channel banks Y -315..-300) |
| U-11 | Room DL content: decor, organ alcove, any modeled masks | Render room_00 DLs (after REC-03 provenance) or capture `cam.clock_tower_interior.*` |
| U-12 | Persistence of `WEEKEVENTREG_76_20` / `76_40` across cycles | Read `Sram_SaveEndOfCycle` weekEventReg persistence (`z_sram_NES.c:430-480`) |
| U-13 | Actor cutscenes 0-8 (no script, global cameras) | Which global door/talk systems index them (`CutsceneManager` global csIds) |
| U-14 | `Z2_OPENINGDAN` arrival for interior exit 0 (its entrance 1) and its exit [1] trigger polygon | Extract `Z2_OPENINGDAN`; decode its collision exit surfaces and spawn 1 |

Register disagreements (not edited): the dispatch's "1 spawn, 7 transition actors" is reversed (7 spawns, 1 TA, section 1.2). PORTAL_REGISTER section 6 lists "Tower interior 0, 2-7" as teleport-only spawns: there is no spawn 7 (spawns 0-6; row 7 is a pad), and spawn 0 is reached by the non-city `Z2_OPENINGDAN` exit `0xC000` (C-BIN). PORTAL_REGISTER row 247 has the `Z2_OPENINGDAN` reverse as UNKNOWN; it is `0xC000` (C-BIN). The registry mode `district` for `portal.clock_tower_interior.south_clock_town` is legitimate (floor exit) but the floor sits behind push leaves like South's `door` row (INF). LOCATION_REGISTER and the South dossier agree with every count and endpoint checked.

**Not inspected:** room DL contents and combiners; light-setting values; cutscene camera splines and player cue semantics; En_Elf; `Z2_OPENINGDAN`, `SPOT00` and rooftop data beyond the header scan; collider details beyond those quoted; 3DS material; captures.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN sections 6, 11, 12, LOCATION_REGISTER, PORTAL_REGISTER, SOURCE_REGISTER, the North, East 2.4 and South 2.1-2.3 / 6.6 dossiers, `shared/city/schema/README.md`, `region.schema.json`, `ids.json`, the Laundry and South records; read scene/room C and XML, `scene_table.h`, `z_scene_table.c`, `z_actor.c`, `z_scene.c`, `z_play.c`, `z_demo.c`, `z_eventmgr.c`, `z_sram_NES.c`, `z_room.c`, `z64scene.h`, `z64cutscene.h`, `z64bgcheck.h`, `z64player.h`, `z64door.h`, `z64camera.h` and the overlays En_Osn, Obj_Demo, Dm_Char00, Dm_Char03, Bg_Ctower_Gear, Bg_Ctower_Rot, Obj_Tokei_Tobira, En_Holl; scratch scripts in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\clock_tower_interior\` (`col.py`, `col2.py`, `col3.py`, `col4.py` collision; `cs.py` cutscenes; `actors.py` actor decode; `senders.py` baserom header scan, outputs `senders_*.txt`); SHA-256 of the seven inputs and of the ZAPD binary; grep of all 22 extracted scenes and `src/` for interior entrance values.

**NOT run:** ZAPD (no object or scene extracted), Blender, Unity, any player or build, captures, renders, controller routes, `region_from_dossier.py`, `validate_city.py`.

**Next dependency:** integrator lifts section 10 and runs `validate_city.py`; REF-03 decides the room_01 capture state (entrance-0 arrival); owner decision on Ocarina possession (SOURCE_REGISTER U-07) fixes which HMS talk branch the baseline shows; then the section 9 extraction of `object_ctower_rot` for a ROOM-series static assembly.
