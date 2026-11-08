# Laundry Pool (`loc.laundry_pool`): source inventory dossier

Task **REF-08** (FULL_CITY_PLAN.md section 12). Base commit `98e554b3c1fc778894fd2c66de64ff040a90bb71` on `main`. Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_ALLEY` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save, Human form; `docs/city/SOURCE_REGISTER.md` section 3).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_ALLEY/Z2_ALLEY.c`, `room.c` = `.../Z2_ALLEY_room_00.c`, `shop.c` = `extracted/n64-us/assets/scenes/Z2_AYASHIISHOP/Z2_AYASHIISHOP.c`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. Collision polygon `i` is at `scene.c` line `410 + i`; room actor entry `aNN` is at `room.c` line `42 + 3 x NN`.

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **ANN** decomp-author annotation (comment, symbol or enum name); **INF** inferred (reason given); **NI** not inspected.

Cross-references, not redone: `docs/city/LOCATION_REGISTER.md` (REF-01) rows `loc.laundry_pool` and section 4.4 (Curiosity Shop room split); `docs/city/PORTAL_REGISTER.md` (REF-02) rows 173, 206, 207, 218, 219; `docs/city/SOURCE_REGISTER.md` (REF-03) profile and `cam.laundry_pool.*`. Every count and endpoint below agrees with those registers unless section 11 says otherwise.

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_ALLEY` / `SCENE_ALLEY` (ANN comment "Laundry Pool") | `include/tables/scene_table.h:228-229` |
| Scene-table index | `0x70` | scene_table.h:229 |
| Entrance-table scene index | `0x6D` (`sLaundryPoolEntranceTable`) | `src/code/z_scene_table.c:2682` |
| Title card text / draw config / restrictions | `0x105` / `SCENE_DRAW_CFG_MAT_ANIM` / `RESTRICTIONS_NONE` | scene_table.h:229 |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_SET(0, 0, 1, 0)`: switch0 0, switch1 0, chest bit 0, collectible 0 | scene_table.h:229; arg order `src/code/z_sram_NES.c:23`. No chest actor exists in this scene (section 4); meaning UNKNOWN (U-10) |
| XML files | Scene `Z2_ALLEY` segment 2; room `Z2_ALLEY_room_00` segment 3; two room DLs declared: `0x7390` (opaque), `0x8CC0` (XLU) | XML `assets/xml/scenes/Z2_ALLEY/Z2_ALLEY.xml` |
| Room count | 1 (`SCENE_CMD_ROOM_LIST(1, ...)`) | HDR scene.c:15 |
| Scene / room headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene or room | HDR scene.c:13-31, room.c:13-22 (agrees with REF-01 "0 alternates") |
| Cutscene scripts | none (no `SCENE_CMD_CUTSCENE_SCRIPT_LIST`) | HDR scene.c:13-31 |
| Special files | `NAVI_QUEST_HINTS_NONE`, keep object `GAMEPLAY_FIELD_KEEP` | HDR scene.c:20 |
| Sound | spec 0, ambience 0, seq 29 (same value as North) | HDR scene.c:14; macro `include/z64scene.h:845` |
| Skybox | file 0, skybox 1, weather 10, lightMode false | HDR scene.c:23; macro z64scene.h:833 |
| Env light settings | 29 entries (values not transcribed) | HDR scene.c:25, :60-119 |
| Actor cutscenes / cs cameras | 14 / 4 | HDR scene.c:26-27, :339-354, :379-384 |
| Paths | 11 (section 7.3) | HDR scene.c:21, :325-337 |
| Minimap | 1 entry `{0x0105, 0xFEC0, 0, 0xFC40, 0}`, scale 52; identical values to North's entry | HDR scene.c:356-362 |
| Animated materials | 1 entry `{ -1, 1, TexScrollParams }` (section 1.1) | HDR scene.c:390-397 |
| Transition actors | 1 (`EN_DOOR`, section 2.5) | HDR scene.c:16, :40-42 |
| Room commands | echo 4; `ROOM_BEHAVIOR(0x04, 0x00, 0, 0, false, 1)`: room type 4, environment 0, lens 0, posLights off, storm state 1; `SKYBOX_DISABLES(false, false)`; `TIME_SETTINGS(255, 255, 3)` | HDR room.c:14-17; decode `src/code/z_scene.c:246-253`; type 4 = `ROOM_TYPE_4` "prevents switching to CAM_SET_HORSE" (ANN `include/z64scene.h:319`) |
| Room shape | cullable (type 2), 19 entries | HDR room.c:132-158 |

### 1.1 Animated material (`SCENE_DRAW_CFG_MAT_ANIM`)

| Step | Finding | Evidence |
| --- | --- | --- |
| Draw config | `Scene_DrawConfigMatAnim` calls `AnimatedMat_Draw(play, play->sceneMaterialAnims)`, which runs `AnimatedMat_DrawMain(..., step = play->gameplayFrames, flags = 3)` (OPA and XLU) | `src/code/z_scene_proc.c:498-500`, `:413-415` |
| Entry | `{ segment -1, type 1, params }`; segment `ABS(-1) + 7 = 8`; a negative segment marks the last entry | HDR scene.c:395-397; C `z_scene_proc.c:400-406` |
| Type 1 | `AnimatedMat_DrawTwoTexScroll`, two-layer texture scroll into segment 8 | `z_scene_proc.c:85-108`, handler table `:389-391` |
| Params | layer 0 `{xStep 0, yStep 1, 32, 32}`; layer 1 `{xStep 1, yStep 1, 32, 32}` | HDR scene.c:390-393 |
| Effect | `Gfx_TwoTexScroll` sets tile 0 origin to `(0, -frames)` and tile 1 origin to `(+frames, -frames)` in 10.2 fixed point, each `% 2048`: tile 0 moves 1/4 texel per gameplay frame along -T, tile 1 moves 1/4 texel per frame along +S and -T | C `z_scene_proc.c:85-90`; `src/code/z_rcp.c:1396-1412` |
| Target | the only `gsSPDisplayList(0x08000000)` in the scene/room is in `Z2_ALLEY_room_00DL_008B58` (room.c:1295) | grep: 1 hit in room.c, 0 in scene.c |
| Target DL | `DL_008B58` (room.c:1281-1310): XLU (`G_RM_AA_ZB_XLU_SURF2`), prim color `(255, 255, 255, 166)`, prim LOD frac 0x80, texture `Z2_ALLEYTex_0028E0` (rgba16 32x32) loaded into both tiles (`gsDPLoadTextureBlock` + `gsDPLoadMultiBlock`), combiner lerps TEXEL1/TEXEL0 by PRIM_LOD_FRAC; 22 triangles, **every vertex at Y = -135**, vertex bounds x -2577..-1329, z -262..1109 | C (room.c; scratch `dl_out.txt`) |
| Draw path | `DL_008B58` is the 7th call of XLU `DL_008C80` (room.c:1312), the XLU DL of cullable entry 2 (center (-1953, -2, 423), radius 928); `DL_008C80` is also called by XML-declared `DL_008CC0` (room.c:1323) | HDR |
| Meaning | INF: a scrolling, 65 % opaque two-layer surface at exactly the water-box surface height (section 5), i.e. the visible water surface | Y equality with the water box (scene.c:1080) |

**Not reproducible by a static import.** A static import gets one frame of this material (tile offsets at frame 0) and must be labeled as such; the scroll speed in seconds also depends on SOURCE_REGISTER U-01 (frames per second).

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:33-38; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam index `params & 0xFF` (`include/z64player.h:17-18`). Yaw = `(rawRotY >> 7) & 0x1FF` degrees.

| Spawn | Position | Raw rot X / Y / Z | Yaw | Params | Start mode | bgCam | Support floor | Used by |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (-1426, -8, 397) | 0x0007 / 0x7C00 / 0x007F | 248 (faces (-0.93, -0.38) in XZ: down the corridor) | 0x0EFF | 0xE (`PLAYER_START_MODE_E`, unnamed, ANN z64player.h:35) | none (0xFF) | poly 136 at Y -8.5 (dirt, 28 deg) | Laundry entrance 0 `0xDA00` from South exit [6] (polys 158/159), REF-02 row 173 |
| 1 | (-2121, -120, -49) | 0x0007 / 0xAB00 / 0x007F | 342 (faces (-0.31, 0.95): away from the door) | 0x0EFF | 0xE | none | poly 174 at Y -120 (dirt) | Laundry entrance 1 `0xDA10` from Curiosity Shop exit [1], REF-02 row 218 |

Entrance list (HDR scene.c:48-51): `{0x00, 0x00}`, `{0x01, 0x00}` (entrance i -> spawn i, room 0). Entrance-table rows (C, z_scene_table.c:2398-2413): e0 `{SCENE_ALLEY, 0, 0xCA14}`, e1 `{SCENE_ALLEY, 1, 0x4A14}`, e2 `{SCENE_ALLEY, 2, 0x4102}`. e2 points at spawn 2, which does not exist (REF-01 U-04); grep found no `0xDA20` in any extracted scene and no `ENTRANCE(LAUNDRY_POOL, 2)` in `src/` (U-09). Flags: e0 and e1 both have exit fade 0x14 and arrival transition `(f >> 7) & 0x7F = 0x14`; bit 15 set on e0 only (bit meanings per PORTAL_REGISTER section 1; transition names NI).

### 2.2 Exit list (HDR scene.c:53-58) and consumers

Decode is a **cross-check only** (REF-02 owns endpoints): entrance-table scene = `value >> 9`, entrance = `(value >> 4) & 0x1F`, layer = `value & 0xF`.

| Exit idx | Raw | Cross-check decode | Destination row | Consumer in Laundry |
| --- | --- | --- | --- | --- |
| 0 | `0xD860` | 0x6C `Z2_CLOCKTOWER`, entr 6 | `sSouthClockTownEntrance6` (z_scene_table.c:2372-2374) `{SCENE_CLOCKTOWER, 6, 0xCA14}` -> South spawn 6 (-840, 100, 560), yaw 90, params 0x0F06 (`Z2_CLOCKTOWER.c:54-55`) | surface type 6, polys 130/131 (section 2.3) |
| 1 | `0xD650` | 0x6B `Z2_BACKTOWN`, entr 5 | North spawn 5 (North dossier section 2.1) | none found: no collision surface, transition actor or room actor in the 18 overlays used here reads `setupExitList[1]` (grep, section 11 U-08; PORTAL_REGISTER U4) |
| 2 | `0x0E10` | 0x07 `Z2_AYASHIISHOP`, entr 1 | `sCuriosityShopEntrance1` (z_scene_table.c:148-150) `{SCENE_AYASHIISHOP, 1, 0x0102}` -> shop entrance list [1] `{0x01, 0x01}` = spawn 1, **room 1** (shop.c:59) | surface type 3, polys 40/41 behind the transition-actor door (section 2.5) |
| 3 | `0x0000` | n/a | n/a | none; trailing zero (REF-01: "last 0x0000"; U-02 there) |

### 2.3 Collision surfaces carrying exits (C from scene.c:404-407 and the polygon list)

`exitIndexPlusOne` = bits 8-12 of `SurfaceType.data[0]` (`include/z64bgcheck.h:79-87`). Unlike North, **no polygon in this scene carries a vIB flag** (no conveyor or Deku-flower floors).

| Surface type | data[0] / data[1] | Exit idx | Polys | Trigger extent x / y / z | Vertex-mean centroid | Normal / slope | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 6 | 0x00000100 / 0x000017C0 | 0 `0xD860` South | 130, 131 (scene.c:540-541) | -1275..-1120 / 50..100 / 355..435 | (-1196, 75, 395) | (-10546, 31006, -1055) and (-10060, 31185, 0): 19 and 18 deg, rising toward +X | dirt; agrees with PORTAL_REGISTER row 206 |
| 3 | 0x00000300 / 0x000017C2 | 2 `0x0E10` Curiosity room_01 | 40, 41 (scene.c:450-451) | -2126..-2016 / -120 / -283..-102 | (-2071, -120, -193) | (0, 32767, 0) flat | stone; both polys have xpFlag 1 (`COLPOLY_IGNORE_CAMERA`, vIA `0x202E`); agrees with row 207 |

### 2.4 South entry corridor (C; floors from scratch `corridor_out.txt`)

| Segment (east to west) | Polys | x | Y | z | Slope | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| South-side stub inside the Laundry collision | 125-129 (dirt, flat) | -1120..-800 | 100 | 355..600 | 0 | C |
| Exit trigger to South | 130, 131 | -1275..-1120 | 50..100 | 355..435 | 18-19 deg | C |
| Upper ramp | 132-135 | -1431..-1267 | -7..50 | 355..435 | 20-35 deg | C |
| Lower ramp | 136-141 | -1653..-1414 | -127..0 | 267..435 | 27-40 deg | C |
| Bank foot | 142-147, 149, 150, 152 | -1764..-1622 | -129..-120 | 250..424 | 3-33 deg | C |

The corridor falls 220 units (Y 100 to -120) over about 530 units of X. Arrival from South (`0xDA00`) lands on spawn 0, 151 units west of the trigger's west edge and below it, facing down the ramp: the arrival is outside the departure trigger (graph check passes, C). Whether the 27-40 degree segments are walkable for the player or only for NPC paths: UNKNOWN (U-01). Source NPC paths 0, 1, 2, 4 and 10 run along this corridor (section 7.3), which shows the authored route but is not a player traversal test.

INF registration: the stub floors 125-129 (Y 100, x -1120..-800, z 355..600) overlap South's own Laundry exit polygons 158/159, AABB (-1120, 100, 435)..(-859, 100, 600) (PORTAL_REGISTER 3.3), and the Postman, Anju and Kafei anchors (a06, a08, a09, about (-1050, 100, 530)) stand inside that South AABB. Laundry and South appear to share one N64 frame with a short overlap at the seam; confirm by overlaying both collisions (U-07).

### 2.5 The back-room door (Curiosity Shop room_01)

**Transition actor** (HDR scene.c:41): `{ 0, 255, 0, 255, ACTOR_EN_DOOR, -2097, -120, -112, -21761, 0x0285 }`.

| Field | Value | Evidence |
| --- | --- | --- |
| Sides | front room 0, bgCam -1; back room 0, bgCam -1 ("-1 for generic door") | C `include/z64scene.h:369-378` |
| Position / yaw | (-2097, -120, -112); rotY -21761 = 0xAAFF, yaw `(0xAAFF >> 7) & 0x1FF = 341` deg; csId `0x7F` none | C `src/code/z_actor.c:3600-3625` (yaw :3613, params :3618) |
| Params at spawn | `TRANSITION_ACTOR_PARAMS(0, 0x285) = 0x0285` (transition index 0) | C `include/z64door.h:22-23` |
| Type | `ENDOOR_GET_TYPE = (0x285 >> 7) & 7 = 5` `ENDOOR_TYPE_SCHEDULE`; type var `0x05` = `ENDOOR_SCH_TYPE_LAUNDRY_POOL` | OVL `ovl_En_Door/z_en_door.h:49, :60, :95-96` |
| Model | `sObjectInfo[17 + 5]` = `DOOR_OBJINFO_22` `{ -1, DOOR_DL_DEFAULT, GAMEPLAY_KEEP }`: skeleton `gDoorSkel` (gameplay_keep 0x22B28); limb 4 draws `gDoorLeftDL` (0x20BB8) or `gDoorRightDL` (0x20D00), chosen by camera yaw because both sides are room 0 | OVL z_en_door.c:217, :343-347, :638-660; XML gameplay_keep.xml:931-941 |
| Exit polygon behind it | polys 40/41 (section 2.3). In door-local coordinates (`Actor_WorldToActorCoords`, z_actor.c:1356-1372) their front edge is at local z +0.3 to +0.9 (the door plane) and they extend to local z -171, 60 units wide (local x -33..+30) | C (scratch `door_out.txt`) |
| Backdrop | `DL_0064B8` (room.c:870): 2 triangles, prim color black, x -2113..-2056, y -120..-20, z -159..-140, i.e. local z -39, 60 wide and 100 tall behind the door | C; INF: the dark void seen through the open door |
| Door frame ceiling | ceilings 47, 48, 83, 84 at Y -20 over x -2115..-2074, z -217..-77 | C |
| Laundry-side approach | spawn 1 is at local (-2, 0, +67), the stone floors 38/39/171/172 at local z +14..+34: the walkable Laundry side is local **+Z** (front) | C |

**Destination** `0x0E10` -> `loc.curiosity_shop.room_01`: spawn 1 (-262, -96, -619), yaw 0, params 0x0EFF (start mode 0xE, no bgCam), room 1 (shop.c:39-40, :59). The shop's matching door is transition actor 1 (shop.c:49) `{1, 255, 1, 255, ACTOR_EN_DOOR, -264, -96, -612, 23167, 0x0285}`: rooms 1/1, yaw 180, **the same type 5 / `ENDOOR_SCH_TYPE_LAUNDRY_POOL` params**. In that door's local frame the arrival spawn is at local z +7 and the shop's exit floor at local z +18..+298, so the room_01 interior is that door's local -Z side (C).

**Return exit** `0xDA10` (shop exit list [1], shop.c:66): surface type 12 (`data[0] 0x204`: bgCam 4, exit+1 = 2), polys 114/115, flat floor at Y -96, x -364..-164, z -910..-630 (C from shop collision; scratch `ayashii_col_out.txt`) -> `sLaundryPoolEntrance1` `{SCENE_ALLEY, 1, 0x4A14}` -> Laundry spawn 1 above. Kickout to the same exit by `EN_TIME_TAG` in room_01 at 22:00 on Days 2 and 3: REF-02 row 219, not re-read. room_01 has no walkable link to room_00 (REF-01 4.4).

**Door state at Day 1 noon (OVL, decoded):**

1. `EnDoor_Idle` only offers the door when the player is within |x| < 20, |y| < 20, |z| < 50 of it in door-local space and faces it within 0x3000 (z_en_door.c:485-497).
2. For `ENDOOR_TYPE_SCHEDULE` it runs the schedule **only when `playerPosRelToDoor.z > 0`** (z_en_door.c:537-547). A non-none result sets `textId = result + 0x1800` and `player->doorType = PLAYER_DOORTYPE_TALKING`: a message instead of opening ("A schedule returning none means the door can be used normally", ANN z_en_door.c:45-48).
3. `sDoorSch_LaundryPool` (`ovl_En_Door/scheduleScripts.schl:133-157`): Day 2 returns none (open) only from 15:10 to 22:00 and only with `WEEKEVENTREG_LETTER_TO_KAFEI_SCHEDULED_DAY_2`; Day 3 returns none from 13:00 to 22:00 only with `WEEKEVENTREG_KAFEI_ENTRUSTED_LINK`; **every other case, including all of Day 1, returns `0xC`**.
4. Result at `prof.d1.noon.normal`: from the Laundry side (local +Z) the door does **not** open and shows text `0x180C` (content UNKNOWN: `assets/text/message_data.h` is a 492-byte stub in this checkout, U-05). From inside room_01 (shop door's local -Z) the schedule check is skipped, so that door is always usable from inside (C). An NPC can still open a schedule door through `openTimer` (`EnDoor_OpenScheduleActor`, z_en_door.c:569-589); Kafei's Day 1 06:50-07:20 path ends behind the door (section 7.3).

### 2.6 Proposed portal IDs (cross-reference PORTAL_REGISTER.md, REF-02)

| Proposed ID | Source evidence | Arrival back into Laundry |
| --- | --- | --- |
| `portal.laundry_pool.south_clock_town` | surface 6, polys 130/131, exit 0 `0xD860` | spawn 0 via South `0xDA00` (`portal.south_clock_town.laundry_pool`) |
| `portal.laundry_pool.curiosity_shop` | TA0 `EN_DOOR` 0x0285 + surface 3, polys 40/41, exit 2 `0x0E10`; Day 1 locked from this side | spawn 1 via shop `0xDA10` (`portal.curiosity_shop.laundry_pool`) |

Exit 1 (`0xD650`) has no Laundry trigger found, exit 3 is padding: neither gets a portal row here.

## 3. Object banks (HDR room.c:24-39; IDs from `include/tables/object_table.h`)

Actor spawn fails if the profile's object is not loaded (`Actor_SpawnAsChildAndCutscene`, `src/code/z_actor.c:3517-3523`).

| # | Bank | Object ID | Consumer (profile `objectId` or slot lookup, OVL) | Needed for Day 1 static list |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_SYOKUDAI` | 0x080 | Obj_Syokudai a01, a02 | yes (a01, unlit) |
| 1 | `OBJECT_WOOD02` | 0x061 | no consumer among the 18 overlays here (grep) | no (U-16) |
| 2 | `OBJECT_KIBAKO` | 0x16F | Obj_Kibako a07 (bank index 1, `sObjectIds`, z_obj_kibako.c:70) | yes |
| 3 | `OBJECT_OS_ANIME` | 0x09D | no consumer among the 18 overlays here (grep) | no (U-16) |
| 4 | `OBJECT_MM` | 0x107 | En_Pm a06 | no (not drawn at noon) |
| 5 | `OBJECT_LODMOON` | 0x223 | En_Fall a03 | yes (moon, special rule) |
| 6 | `OBJECT_AN2` | 0x0E0 | En_An extra slot (`an2ObjectSlot`, z_en_an.c:882) | no |
| 7 | `OBJECT_TEST3` | 0x01C | En_Test3 a09 | no (not drawn at noon) |
| 8 | `OBJECT_AN1` | 0x0E2 | En_An a08 | no (not drawn at noon) |
| 9 | `OBJECT_CHA` | 0x243 | En_Cha a11 | yes |
| 10 | `OBJECT_FR` | 0x0BC | En_Minifrog a18 | skeletal (later) |
| 11 | `OBJECT_FU` | 0x0FF | En_Guruguru a12 | no (night) |
| 12 | `OBJECT_MASK_KERFAY` | 0x1FF | En_An extra slot (z_en_an.c:880) | no |
| 13 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai a16 | yes (already extracted) |

Implicit (not in the list): `gameplay_keep` 0x001 (En_Door default door, Obj_Mure, En_Fish, En_Elforg, En_Insect, En_Talk, En_Test4, En_Item00) and `gameplay_field_keep` 0x002 (scene special file; En_Kusa type 0 bushes, `sObjectIds`, z_en_kusa.c:72).

## 4. Actor tables

### 4.1 Decode rules

Same rules as the North dossier section 4.1 (verified again here): `SPAWN_ROT_FLAGS(rotation, flags) = (rotation << 7) | flags` (`include/z64scene.h:16`); `Actor_SpawnEntry` (z_actor.c:3628-3655): ID `id & 0x1FFF`, angles `(raw >> 7) & 0x1FF`, axis flags `0x8000` Y, `0x4000` X, `0x2000` Z pass a raw integer; csId `rawRotY & 0x7F`; half-day mask `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, 0 -> all. Day 1 at 12:00 -> half-day bit `0x080` (D1d). No entry has ID bit `0x800`, so the `EVENTINF_17` filter does not apply. Transition actors spawn with `HALFDAYBIT_ALL` (z_actor.c:3612-3618).

Classes: `static-mesh`, `posed-skeletal`, `billboard/effect`, `controller/invisible`, `npc-scheduled` (skeletal NPC whose presence/pose is schedule- or flag-gated), `door/transition`. "D1d" = mask includes Day 1 daytime. "Water" = position relative to the water box (section 5).

### 4.2 Transition actor (HDR scene.c:41)

| # | Actor | Position | Yaw | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- |
| TA0 | `ACTOR_EN_DOOR` (0x005) | (-2097, -120, -112) | 341 | 0x0285 (schedule, Laundry Pool) | gameplay_keep | door/transition | yes (always spawned; closed from this side on Day 1, section 2.5) |

### 4.3 Room actors, default header (28 entries, HDR room.c:41-127, decode C)

| # | Actor (table ID) | Position | Raw rot X / Y / Z | Rot deg X / Y / Z | Mask | CS | Params | Bank | Class | D1d | Water |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_OBJ_MURE` (0x04F) | (-2090, -162, 508) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x3322 | gameplay_keep | controller/invisible (fish group) | yes | 27 below surface |
| 1 | `ACTOR_OBJ_SYOKUDAI` (0x039) | (-2220, -120, 610) | 0x0005 / 0x007F / 0x002A | 0 / 0 / 0 | 0x2AA all daytimes | none | 0x207F | object_syokudai | static-mesh | yes | bank, 15 above |
| 2 | `ACTOR_OBJ_SYOKUDAI` (0x039) | (-2220, -120, 610) | 0x0002 / 0x007F / 0x0055 | 0 / 0 / 0 | 0x155 all nights | none | 0x287F | object_syokudai | static-mesh | no | bank, 15 above |
| 3 | `ACTOR_EN_FALL` (0x17C) | (-320, 11000, -959) | 0x1907 / 0x07FF / 0x007F | 50 / 15 / 0 | 0x3FF all | none | 0x037F | object_lodmoon | static-mesh (camera-relative moon) | yes | |
| 4 | `ACTOR_EN_ELFORG` (0x1B0) | (-2080, -111, 446) | 0x0005 / 0x2D7F / 0x002A | 0 / 90 / 0 | 0x2AA all daytimes | none | 0xFE03 | gameplay_keep | billboard/effect (stray fairy) | yes | 24 above, over the basin |
| 5 | `ACTOR_EN_INSECT` (0x016) | (-1761, -120, 621) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | gameplay_keep | posed-skeletal (ambient bug) | yes | |
| 6 | `ACTOR_EN_PM` (0x1D5) | (-1054, 100, 538) | 0x0007 / 0x607F / 0x007F | 0 / 192 / 0 | 0x3FF all | none | 0x0000 | object_mm | npc-scheduled | yes | |
| 7 | `ACTOR_OBJ_KIBAKO` (0x081) | (-1668, -120, 615) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0xA002 | object_kibako | static-mesh | yes | |
| 8 | `ACTOR_EN_AN` (0x202) | (-1061, 100, 542) | 0x0007 / 0x66FF / 0x007F | 0 / 205 / 0 | 0x3FF all | none | 0x8001 | object_an1 | npc-scheduled | yes | |
| 9 | `ACTOR_EN_TEST3` (0x159) | (-1041, 100, 515) | 0x0001 / 0x647F / 0x0070 | 0 / 200 / 0 | 0x0F0 D1d, D1n, D2d, D2n | none | 0x01E2 | object_test3 | npc-scheduled | yes | |
| 10 | `ACTOR_EN_TALK \| 0xE000` (0x261) | (-1961, -77, 537) | 0x0087 / 0x007F / 0x007F | 1 / 0 / 0 (all raw) | 0x3FF all | none | 0xFE07 | gameplay_keep | controller/invisible | yes | 58 above |
| 11 | `ACTOR_EN_CHA` (0x270) | (-1961, -123, 530) | 0x0007 / 0x2D0C / 0x007F | 0 / 90 / 0 | 0x3FF all | 12 | 0x000F | object_cha | static-mesh (bell, swings when hit) | yes | 12 above |
| 12 | `ACTOR_EN_GURUGURU` (0x248) | (-1746, -120, 554) | 0x0000 / 0x5A7F / 0x0050 | 0 / 180 / 0 | 0x050 D1n, D2n | none | 0x0001 | object_fu | npc-scheduled | no | |
| 13 | `ACTOR_EN_KUSA` (0x090) | (-1900, -120, 316) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0500 | gameplay_field_keep | static-mesh (bush) | yes | bank, 15 above |
| 14 | `ACTOR_EN_KUSA` (0x090) | (-1921, -120, 228) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0C00 | gameplay_field_keep | static-mesh (bush) | yes | bank, 15 above |
| 15 | `ACTOR_EN_KUSA` (0x090) | (-1864, -120, 216) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0C00 | gameplay_field_keep | static-mesh (bush) | yes | bank, 15 above |
| 16 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-280, 555, -833) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x207F | object_obj_tokeidai | static-mesh (tower clock) | yes | |
| 17 | `ACTOR_EN_TEST4` (0x15A) | (-893, 174, 265) | 0x0007 / 0x0009 / 0x007F | 0 / 0 / 0 | 0x3FF all | 9 | 0x0C00 | gameplay_keep | controller/invisible | yes | |
| 18 | `ACTOR_EN_MINIFROG` (0x022) | (-1885, -120, 251) | 0x0007 / 0x7D0D / 0x007F | 0 / 250 / 0 | 0x3FF all | 13 | 0x0004 | object_fr | posed-skeletal (white frog) | yes | bank, 15 above |
| 19 | `ACTOR_EN_ITEM00` (0x00E) | (-1984, -208, -62) | 0x0000 / 0x007F / 0x0020 | 0 / 0 / 0 | 0x020 D2d | none | 0x0302 | gameplay_keep | static-mesh (state pickup) | no | pool floor, 73 below |
| 20 | `ACTOR_EN_ITEM00` (0x00E) | (-2104, -208, 204) | 0x0000 / 0x007F / 0x0010 | 0 / 0 / 0 | 0x010 D2n | none | 0x0302 | gameplay_keep | static-mesh (state pickup) | no | pool floor |
| 21 | `ACTOR_EN_ITEM00` (0x00E) | (-2134, -208, 707) | 0x0000 / 0x007F / 0x000A | 0 / 0 / 0 | 0x00A D3d, D4d | none | 0x0302 | gameplay_keep | static-mesh (state pickup) | no | pool floor, under the south arch |
| 22 | `ACTOR_EN_ITEM00` (0x00E) | (-1956, -208, -42) | 0x0000 / 0x007F / 0x0020 | 0 / 0 / 0 | 0x020 D2d | none | 0x0402 | gameplay_keep | static-mesh (state pickup) | no | pool floor |
| 23 | `ACTOR_EN_ITEM00` (0x00E) | (-2066, -208, 220) | 0x0000 / 0x007F / 0x0010 | 0 / 0 / 0 | 0x010 D2n | none | 0x0402 | gameplay_keep | static-mesh (state pickup) | no | pool floor |
| 24 | `ACTOR_EN_ITEM00` (0x00E) | (-2090, -208, 707) | 0x0000 / 0x007F / 0x000A | 0 / 0 / 0 | 0x00A D3d, D4d | none | 0x0402 | gameplay_keep | static-mesh (state pickup) | no | pool floor |
| 25 | `ACTOR_EN_ITEM00` (0x00E) | (-1927, -208, -19) | 0x0000 / 0x007F / 0x0020 | 0 / 0 / 0 | 0x020 D2d | none | 0x0502 | gameplay_keep | static-mesh (state pickup) | no | pool floor |
| 26 | `ACTOR_EN_ITEM00` (0x00E) | (-2026, -208, 236) | 0x0000 / 0x007F / 0x0010 | 0 / 0 / 0 | 0x010 D2n | none | 0x0502 | gameplay_keep | static-mesh (state pickup) | no | pool floor |
| 27 | `ACTOR_EN_ITEM00` (0x00E) | (-2049, -208, 707) | 0x0000 / 0x007F / 0x000A | 0 / 0 / 0 | 0x00A D3d, D4d | none | 0x0502 | gameplay_keep | static-mesh (state pickup) | no | pool floor |

Support floors under each anchor (C, floor-triangle containment, scratch `support_out.txt`): a01/a02 on Y -120 (poly 175), a04 and a00 over the pool floor (Y -208), a05/a07/a12 on Y -120 (polys 329/330), a06/a08/a09 on poly 127 at Y 100 (corridor stub), a10/a11 over poly 313 at Y -124.1, a13-a15/a18 on Y -120 (polys 181/182), a19-a27 exactly on the pool floor (Y -208, polys 121-123, 373-375), a17 has no floor below it.

### 4.4 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init: params, scale, collision, children | Draw | Presence at Day 1 noon (fresh save, Human) |
| --- | --- | --- | --- |
| En_Door (TA0) | type 5, schedule 5; no dynapoly (not framed); focus 35 | `gDoorSkel` + limb 4 side DL (section 2.5) | spawned; closed from the Laundry side (text 0x180C) |
| Obj_Mure (a00) | `0x3322`: chNum `(p >> 12) & 0xF = 3`, ptn `(p >> 8) & 7 = 3`, svNum `(p >> 5) & 3 = 1`, type `p & 0x1F = 2` `OBJMURE_TYPE_FISH` (z_obj_mure.h:10-21); spawns 3 `ACTOR_EN_FISH` (gameplay_keep, `FISH_PARAMS(ENFISH_MINUS1)`) when `abs(projectedPos.z) < 1000`, kills them beyond 1040 (z_obj_mure.c:37-66, :273-279, :395-404) | profile draw `NULL`; fish draw NI | controller present; fish are ambient, camera-distance gated |
| Obj_Syokudai (a01, a02) | type `p >> 12 = 2` `NO_SWITCH`, switch flag 0x7F none, group 0; start-lit bit `0x800`: clear on a01, set on a02 (z_obj_syokudai.h:8-13); ICHAIN scale 1.0; stand cylinder r12 h45; point-glow light at Y +70 (z_obj_syokudai.c:71-116) | `gObjectSyokudaiTypeNoSwitchDL` (object_syokudai 0x870, XML); flame `gEffFire1DL` only while `snuffTimer != 0` (:298-321); actor memory is zeroed at spawn (z_actor.c:3538), so a01 starts unlit | a01 present, unlit; a02 night only (lit) |
| En_Fall (a03) | type `(p & 0xF80) >> 7 = 6` `EN_FALL_TYPE_LODMOON` (z_en_fall.h:6-16); same params and rotation as North a30, home differs ((-320, 11000, -959) vs (-280, 11000, -1020)) | as North dossier 4.4 (Day 1 scale 0.384, 9000-unit eye rule) | always; answers SOURCE_REGISTER U-09 for Laundry |
| En_Elforg (a04) | `0xFE03`: type `p & 0xF = 3` `STRAY_FAIRY_TYPE_CLOCK_TOWN`, area `(p & 0x1C0) >> 6 = 0`, flag 0x7F; killed if `WEEKEVENTREG_08_80`; flex `gStrayFairySkel` (gameplay_keep 0x2CA98), scale 0.01, random yaw, `EnElforg_FreeFloating` (z_en_elforg.h:8-30; .c:66-150) | skeletal glow (draw NI in detail) | present on a fresh save (INF: flag clear); position not fixed |
| En_Insect (a05) | random yaw; skeleton `gameplay_keep_Skel_0527A0`, ICHAIN scale 0.01; random start action (z_en_insect.c:123-166) | `SkelAnime_DrawOpa` (:503-509) | present; moves |
| En_Pm (a06) | flex `object_mm_Skel_0096E8`; path `p & 0xFF = 0` (z_en_pm.h:12); script `D_80AFB900[func_80AF7B40()]`, index 0 on a fresh save (z_en_pm.c:386-404, :1843) | drawn only if schedule result != 0 (:2032) | script `D_80AFAD80` Day 1 12:00-13:01 returns a value only in `SCENE_POSTHOUSE` (scheduleScripts.schl:70-75); Laundry appears only on Day 2 with `LETTER_TO_KAFEI_SCHEDULED_DAY_2` (:183-205): **not drawn** |
| Obj_Kibako (a07) | `0xA002`: bank index `(p >> 15) & 1 = 1` (`OBJECT_KIBAKO`), drop id `p & 0x3F = 2`, drop flag `(p >> 8) & 0x7F = 0x20`, bomber-hide bit 0 (z_obj_kibako.h:6-9); scale 0.15; cylinder r15 h30 (z_obj_kibako.c:50-70, :134-158) | `gSmallCrateDL` (object_kibako 0x1180) once the object is loaded (:257, :427-429) | present |
| En_An (a08) | outside `SCENE_YADOYA` bit 0x8000 is cleared, path `p & 0xFF = 1`; killed if `WEEKEVENTREG_COUPLES_MASK_CUTSCENE_FINISHED` (z_en_an.h:15-18; .c:3378-3411) | drawn only if schedule result != none or `forceDraw` (:3501-3504) | Anju's script has a `SCENE_ALLEY` branch only on Day 2 without the midnight meeting (13:15-15:55, `ANJU_SCH_LAUNDRY_POOL_SIT` 13:45-15:25, scheduleScripts.schl:68-183): **not drawn** |
| En_Test3 (a09, Kafei) | path `p & 0x1F = 2`, `(p >> 5) & 0xF = 0xF` -> follows schedule (z_en_test3.h:10-11; .c:572-625); one instance only (`sExists`) | `draw = NULL` for result none (`KAFEI_SCH_ACTION_STOP_DRAWING`, .c:341-342, :649-652) | Day 1 Laundry: 06:50-07:20 runs to the shop backdoor (path 2), none before 07:30, after that `KAFEI_SCH_HEARD_BELL_RING` only if `WEEKEVENTREG_HIT_LAUNDRY_POOL_BELL` (scheduleScripts.schl:2-11): **not drawn** at noon on a fresh save |
| En_Talk (a10) | rotX raw 1 -> attention range `1 - 1 = 0`; text `(p & 0x3F) + 0x1C00 = 0x1C07`; offers talk within 40 XZ when faced (z_en_talk.h:6-7; .c:29-62) | profile draw `NULL` | present (no geometry); INF: 7 units XZ from the bell, so it is the bell's talk/inspect spot; text content NI (U-05) |
| En_Cha (a11, bell) | "Laundry Pool Bell" (ANN z_en_cha.c:4); cylinder r10 h40, AC from player; scale 0.01; clears `WEEKEVENTREG_60_04`; at rest `home.rot.x = home.rot.z = 0` (:51-63) | `object_cha_DL_000710` ("cha_ki_model", ANN XML), then model-space translate (-1094, 4950, 9) and `RotateX(home.rot.x)`, then `object_cha_DL_000958` ("cha_kane_model") (:112-119) | present at rest. When hit: bell sfx, swing (`home.rot.z = 0x7D0`, damped), and on the first hit sets `WEEKEVENTREG_HIT_LAUNDRY_POOL_BELL` and queues cutscene 12 (:76-101). That flag makes Kafei appear (row a09): state branch, not static |
| En_Guruguru (a12) | params 1: killed in daytime, active at night (z_en_guruguru.c:86-110) | flex `gGuruGuruSkel` | night mask and night-only init |
| En_Kusa (a13-a15) | type `p & 3 = 0` `ENKUSA_TYPE_BUSH`; random-collectible id `(p >> 8) & 0xF` = 5 (a13), 0xC (a14, a15); bugs bit 0 (z_en_kusa.h:10-31); ICHAIN scale 0.4; random yaw because rot.y = 0; snapped to floor, killed if no floor (.c:360-400) | `gKusaBushType1DL` (gameplay_field_keep 0x78A0, XML) within 1200 projected Z, fade to 1300 (:703-730) | present |
| Obj_Tokeidai (a16) | type `p >> 12 = 2` tower clock (z_obj_tokeidai.h:6-11); same params and anchor as North a32 and South | per ROM_REFERENCE.md | always; visibility from the Laundry UNKNOWN (U-06) |
| En_Test4 (a17) | three-day controller; stars `(p >> 10) * 100 = 300`; bell volume index `(p >> 5) & 0xF = 0` (z_en_test4.h:8-9); csId 9 | profile draw `NULL` | always |
| En_Minifrog (a18) | `p & 0xF = 4` `FROG_WHITE` (ANN "Laundry Pool", z_en_minifrog.h:18); returned bit `(p & 0xF0) >> 4 = 0`; killed if `WEEKEVENTREG_33_02`; grow-in 30 frames then `EnMinifrog_Idle`, jumps; talk only with the Don Gero mask (.c:79-150, :307-331) | flex `gFrogSkel`, env color (190, 190, 190) (:648, :651-662) | present on a fresh save (INF: flag clear); animated |
| En_Item00 (a19-a27) | low byte `0x02` `ITEM00_RUPEE_RED`, scale 0.015; collectible flag `(p & 0x7F00) >> 8` = 3, 4, 5 (each shared by three entries); killed if collected (`src/code/z_en_item00.c:82-113`; enum `include/z_en_item00.h:7-19`) | rupee DL (NI) | none on Day 1: masks D2d, D2n, D3d + D4d only. Three rupees per half-day group, each group at a different pool-floor spot |

### 4.5 Counts

| | Default header |
| --- | --- |
| Room actor entries | 28 (+ 1 transition actor, + 2 spawns) |
| Day 1 daytime candidates by mask | 17 (excluded: a02, a12, a19-a27) |
| static-mesh (incl. moon, bell, rupees) | 18 |
| posed-skeletal | 2 (a05, a18) |
| billboard/effect | 1 (a04) |
| npc-scheduled | 4 (a06, a08, a09, a12) |
| controller/invisible | 3 (a00, a10, a17) |
| door/transition | 1 (TA0, transition list) |

## 5. Water: box, basin, banks and swim data

### 5.1 Water box (HDR scene.c:1079-1081; layout `include/z64bgcheck.h:33-46, :144-149`)

| Field | Value | Evidence |
| --- | --- | --- |
| Raw | `{ -2400, -135, -200, 600, 1320, 0x00000101 }` | HDR |
| Bounds | x -2400..-1800, z -200..1120 (600 x 1320) | C (`minPos` + lengths) |
| Surface Y | -135 ("y = water surface. Water is effectively infinitely deep.", ANN z64bgcheck.h:145) | HDR |
| Properties 0x101 | bgCam index 1 (`BgCamInfo[1]` = `0x0032` `CAM_SET_DUNGEON2`), light index 1, room 0, not disabled (bit 19 clear) | C `WATERBOX_PROPERTIES`, z64bgcheck.h:34-46; camera names `include/z64camera.h:197, :228` |
| Unity | x -24.00..-18.00, z -11.20..2.00, surface Y -1.35 | C |
| Room gating | active only when the current room is 0 (`WATERBOX_ROOM`, `src/code/z_bgcheck.c:4385-4392`); the Laundry has only room 0 | C |
| Depth definition | `depthInWater = waterSurfaceY - actor.y`; negative means the water is below (`include/z64actor.h:124`; `src/code/z_actor.c:1758`) | C |

INF from collision vertices: the scene has 264 vertices but only 261 are used by polygons; the unused ones carry the extreme x -2400 and z 1120, which match the box's corners. The box was probably authored from that geometry. The box is larger than the basin and also covers banks, the torch, bushes, frog and spawn 1; every one of those stands at Y -120 or higher, so its `depthInWater` is negative there.

### 5.2 Surfaces against the water height (C; scratch `col_out.txt`, `islands_out.txt`)

| Surface | Polys | x / z | Y | Relation to Y -135 |
| --- | --- | --- | --- | --- |
| Pool floor (one connected island) | 21 floors (113-124, 373-379, 392, 393) | -2177..-1827 / -177..790 | -208..-200 | 65-73 below |
| Submerged wooden slab top | 91, 92 (wood) | -2005..-1915 / 390..510 | -152 | 17 below; slab underside ceilings 105/106 at -164; its west face (walls 87/88, x -2005, Y -164..-152) is the only wall type 4 (section 7.1) |
| Wooden step beside it | 95, 96 (wood) | -1915..-1875 / 390..510 | -132 | 3 above (lowest dry floor); shares the x -1915 edge with the slab and is part of the main walkable island, so bank, step (-132) and slab (-152) form a descent into the water (C; INF: washing steps) |
| Banks around the basin | 58 floors with a vertex in the box XZ and centroid Y -132..-120 | -2257..-1622 / -283..630 | -132..-120 | 3-15 above |
| Stone bridge deck | 363-368 (stone) | -2177..-1977 / 538..630 | -120 at both ends, -100 at the crest (x -2117..-2037) | 15-35 above; underside ceilings 351-356 span Y -148..-128: 7 above the surface under the crest, 13 below it at the ends |
| South arch over the basin | ceilings 380-385, walls 369-372 | -2177..-1977 / 683..737 | underside -148..-128; no floor on top in collision | rupees a21/a24/a27 lie on the pool floor below it |
| North overhang | ceilings 107-112 | -2015..-1893 / -125..25 | -113..-100 | 22-35 above; no floor on top in collision |
| Door exit floor | 40, 41 | section 2.3 | -120 | 15 above |

Counts for polygons with at least one vertex inside the box XZ: **47 entirely below the surface** (23 floors, 22 walls, 2 ceilings; polys 87-92, 97-106, 113-124, 369-379, 388, 389, 392, 393, 396-399), **35 straddling it** (27 walls, 8 ceilings; no floor straddles), 227 above it. 52 floors lie fully outside the box XZ.

Swim and dive surface flags: **none**. No surface type uses `SURFACE_MATERIAL_WATER_SHALLOW` (4) or `WATER_DEEP` (5) (`include/z64bgcheck.h:232-247`); materials are grass 8 (169 polys), dirt 0 (117), wood 10 (101), stone 2 (13). No floor type, floor property, soft or hookshot bits are set. All water behavior comes from the water box.

### 5.3 What the source gives and what stays a decision

| Item | Source | Status |
| --- | --- | --- |
| Water surface height | -135 (box) and the scrolling surface mesh at -135 | source |
| Bank heights | -120 to -132 (3-15 above); door floor -120; bridge -120 to -100 | source |
| Basin depth | 65-73 to the pool floor, 17 on the wooden slab | source |
| Human-form water thresholds | `sPlayerAgeProperties[PLAYER_FORM_HUMAN]` `unk_24 22.0`, `unk_28 32.4`, `unk_2C 32.0`, `unk_30 48.0` (`ovl_player_actor/z_player.c:1127-1153`; form index 4, `include/z64player.h:70-74`); swim entry when `unk_2C < depthInWater` (z_player.c:8974) | read; INF: Human swims over the pool floor (depth 65-73 > 32) and wades on the slab (17 < 32). The rest of the swim/climb-out state machine (z_player.c:8941-9140) NI |
| Camera in water | bgCam 1 `CAM_SET_DUNGEON2` from the box properties | source; how the camera system consumes it NI |
| Visible surface vs volume | the surface mesh spans x -2577..-1329, z -262..1109, wider than the box in X (section 1.1) | source; INF: the overhang is hidden under banks (U-14) |
| Swimming, diving, drowning, current | none implied by materials | **gameplay decision** (FULL_CITY_PLAN 5.5): build water collision/swim as explicit gameplay, not from the material |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

Assumptions to record in the profile: fresh save (no week-event, switch or collectible flags), Human form, bell never hit. World positions are N64; `world = actorPos + actorRot x (actorScale x localVertex)` (ROM_REFERENCE.md). Unity position applies the documented conversion; rotation conversion under the Z mirror belongs to the importer (PIPE-01/02).

| # | Component / params | N64 position (Unity) | Yaw | Actor scale | Display list(s), bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| TA0 | Back-room door, `0x0285` | (-2097, -120, -112) (-20.97, -1.20, 1.12) | 341 | 0.01 (default; not set by En_Door, INF) | `gDoorSkel` gameplay_keep 0x22B28, limb 4 = `gDoorLeftDL` 0x20BB8 or `gDoorRightDL` 0x20D00 | closed; rest pose needs the skeleton (U-18); side DL depends on camera, so a fixed choice is an approximation |
| 11 | Bell, `0x000F` | (-1961, -123, 530) (-19.61, -1.23, -5.30) | 90 | 0.01 | `object_cha_DL_000710`, then model translate (-1094, 4950, 9), `RotateX(0)`, `object_cha_DL_000958` (object_cha, seg 6) | rest pose; swing omitted |
| 1 | Torch, `0x207F` (unlit) | (-2220, -120, 610) (-22.20, -1.20, -6.10) | 0 | 1.0 (ICHAIN) | `gObjectSyokudaiTypeNoSwitchDL` object_syokudai 0x870 | no flame, no light at noon |
| 7 | Small crate, `0xA002` | (-1668, -120, 615) (-16.68, -1.20, -6.15) | 0 | 0.15 | `gSmallCrateDL` object_kibako 0x1180 | |
| 13 | Bush, `0x0500` | (-1900, -120, 316) (-19.00, -1.20, -3.16) | random | 0.4 | `gKusaBushType1DL` gameplay_field_keep 0x78A0 (seg 5) | fixed yaw is an approximation; Y = floor snap (-120) |
| 14 | Bush, `0x0C00` | (-1921, -120, 228) (-19.21, -1.20, -2.28) | random | 0.4 | same | same |
| 15 | Bush, `0x0C00` | (-1864, -120, 216) (-18.64, -1.20, -2.16) | random | 0.4 | same | same |
| 16 | Tower clock, `0x207F` (type 2) | (-280, 555, -833) (-2.80, 5.55, 8.33) | 0 | 0.1 | as North a32: minute ring 0xCF28, center/hand 0xBEE8, face 0xE818, sun/moon panel 0xC368 (object_obj_tokeidai) | noon pose per ROM_REFERENCE.md; visibility from the Laundry UNKNOWN (U-06) |
| 3 | LOD moon, `0x037F` (special) | (-320, 11000, -959) | X50 Y15 Z0 | 0.384 | `gLodmoonEyesDL` 0x10E0 + `gLodmoonMoonDL` 0x1158 (object_lodmoon) | reproduce the 9000-unit eye rule or label the approximation |
| room | Water surface `DL_008B58` | Y -135 | n/a | n/a | room DL, `Z2_ALLEYTex_0028E0` x2 tiles, segment 8 | frame-0 tile offsets only; scroll is not static (section 1.1) |

**Excluded by half-day mask:** a02 torch (nights), a12 Guru-Guru (D1n, D2n), a19-a27 red rupees (D2d, D2n, D3d + D4d).

**Excluded by schedule or state at Day 1 noon (fresh save):** a06 En_Pm, a08 En_An, a09 En_Test3 (Kafei; appears only after the bell flag). Do not place them.

**Unimplemented in a static assembly (present at noon, needs skeleton/pose, motion or effect reconstruction):** a18 white frog (`gFrogSkel`, jumping), a04 stray fairy (`gStrayFairySkel`, free flight), a05 bug (`gameplay_keep_Skel_0527A0`, random motion), the 3 `En_Fish` children of a00, and the scrolling water material.

**No geometry:** a00 Obj_Mure controller, a10 En_Talk, a17 En_Test4.

## 7. Collision summary (C from scene.c:399-1092; tool output in the scratch folder)

### 7.1 Measures

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 264 / 400 / 7 |
| Water boxes | 1 (section 5.1) |
| Camera data (`BgCamInfo`) | 2: `0x0051` `CAM_SET_ROOM1` ("certain rooms eg. some rooms in Stock Pot Inn", ANN z64camera.h:228), `0x0032` `CAM_SET_DUNGEON2` (:197); every surface type uses bgCam 0; bgCam 1 only through the water box |
| Header bounds | min (-2577, -224, -960), max (-240, 635, 1120); Unity x -25.77..-2.40, y -2.24..6.35, z -11.20..9.60 |
| Vertex extents (all / used by polygons) | x -2400..-800, y -224..560, z -596..1120 / x -2356..-800, y -224..560, z -596..875 (header box is larger: U-17) |
| Floor Y range | -208..560 (normal Y > 0.5) |
| Polygon kinds | 169 floors (92 under 10 deg), 206 walls (162 near vertical), 25 ceilings |
| Floor slope histogram (5-deg bins) | 0: 82, 5: 10, 10: 5, 15: 12, 20: 20, 25: 12, 30: 7, 35: 8, 40: 2, 45: 3, 50: 5, 55: 3 |
| Surface types | 0 grass (167 polys), 1 dirt (115), 2 stone (11), 3 stone + exit 2 (2), 4 grass + wall type 4 (2), 5 wood (101), 6 dirt + exit 0 (2); all with light setting 31 and echo 2 |
| Wall type 4 | surface 4, walls 87/88 (scene.c:497-498), west face of the submerged slab. `sWallFlags[4] = WALL_FLAG_3` (`src/code/z_bgcheck.c:23`); z_player.c:7425-7445 uses `WALL_FLAG_3` to choose a climb-down (`Fclimb_startB`) instead of a fall when stepping off an edge above such a wall. Its role underwater NI (U-13) |
| Poly exclusion | xpFlag 1 (`COLPOLY_IGNORE_CAMERA`) on 106 polys (74 walls, 29 floors, 3 ceilings) in x -2132..-1765, y -120..101, z -283..370: types 0 (19), 2 (2), 3 (2, the exit floor), 5 (83, the wood cluster). No ignore-entity or ignore-projectile flags |
| vIB flags | none |

### 7.2 Floor islands (shared-vertex connectivity, C; scratch `islands_out.txt`)

| Island | Floors | x | Y | z | Max slope | Note |
| --- | --- | --- | --- | --- | --- | --- |
| Main walkable network | 104 | -2257..-800 | -132..100 | -283..630 | 40 | corridor, banks, door floor, both exit triggers, bridge (363-368), wooden step (95/96) |
| Pool floor | 21 | -2177..-1827 | -208..-200 | -177..790 | 23 | under water |
| Y 120 level | 10 | -2297..-1431 | 120 | -156..670 | 0 | not vertex-connected to the main network; INF: wall tops. Reachability UNKNOWN (U-02) |
| Wood cluster (7 small islands) | 25 (21 wood, 4 grass) | -1943..-1765 | -33..95 | 221..370 | 55 | mostly 30-55 deg wood planes with small flat parts at 11, 39 and 61; INF: roof or awning surfaces; reachability UNKNOWN (U-02) |
| High slopes over the corridor mouth | 4 | -1431..-800 | 400..560 | 435..875 | 57 | INF: roof surfaces, out of reach |
| Submerged slab | 2 | -2005..-1915 | -152 | 390..510 | 0 | section 5.2 |
| Guru-Guru platform | 2 (216, 217, wood) | -1787..-1708 | -104 | 560..589 | 0 | 16 above the bank; room DL_004FA8 covers the same box |
| Door lintel top | 1 (80, stone) | -2152..-2081 | 0 | -124..-64 | 0 | INF: top of the door frame |

Bridges and steps present (answers `cam.laundry_pool.bridge_steps`, not `not_applicable`): the stone bridge (363-368), the wooden step and slab (95/96, 91/92). An upper walkable level is not established: `cam.laundry_pool.upper_route` stays pending U-02.

### 7.3 Paths (HDR scene.c:121-337; `Path {count, additionalPathIndex, customValue, points}`, `include/z64path.h:13-18`)

| Path | Points | Next | Custom | Start -> end | Used by (C) |
| --- | --- | --- | --- | --- | --- |
| 0 | 10 | 3 | 31 | (-1054, 100, 538) -> (-1939, -127, 540) | En_Pm a06 (path index 0), chained with `SubS_GetAdditionalPath` (`src/code/z_sub_s.c:485-499`) |
| 1 | 11 | 4 | 3 | (-1061, 100, 542) -> (-1743, -120, 551) | En_An a08 (path index 1) |
| 2 | 32 | 5 | 2 | (-1040, 100, 505) -> (-2058, -120, -217) | En_Test3 a09 (path index 2): corridor, west bank, then through the door; ends at door-local z -112, on the exit floor |
| 3 | 10 | 9 | 32 | (-1939, -127, 540) -> (-1054, 100, 538) | chain of 0 |
| 4 | 11 | none | 4 | (-1743, -120, 551) -> (-1061, 100, 542) | chain of 1 |
| 5 | 34 | 6 | 3 | (-2065, -120, -222) -> (-2110, -100, 584) | chain of 2: door to bridge crest along x -2219 |
| 6 | 30 | 7 | 4 | (-2110, -100, 584) -> (-2069, -120, -222) | chain of 2 |
| 7 | 3 | 8 | 5 | (-2073, -120, -188) -> (-2127, -120, -34) | chain of 2: through the door outward (local z -80 to +84); this is the path `KAFEI_SCH_HEARD_BELL_RING` selects (`{BELL_RUNG, 4, 0}` -> limit 3: 2 -> 5 -> 6 -> 7, z_en_test3.c:352, :674-677) |
| 8 | 3 | none | 6 | (-2124, -120, -33) -> (-2072, -119, -188) | chain of 2: back through the door |
| 9 | 5 | 10 | 55 | (-1939, -127, 540) -> (-2050, -100, 584) | chain of 0: onto the bridge crest |
| 10 | 22 | none | 56 | (-2050, -100, 584) -> (-1041, 100, 538) | chain of 0: back up the corridor |

### 7.4 Elevation bands (centroid Y of floors, 25-unit bins)

| Band | Floors | x | Y | z | Max slope | Surface types |
| --- | --- | --- | --- | --- | --- | --- |
| -225..-200 | 21 | -2177..-1827 | -208..-200 | -177..790 | 23 | 0, 1 |
| -175..-150 | 2 | -2005..-1915 | -152 | 390..510 | 0 | 5 |
| -150..-125 | 29 | -1977..-1627 | -132..-120 | 271..630 | 37 | 0, 1, 5 |
| -125..-100 | 58 | -2257..-1622 | -129..-100 | -283..630 | 33 | 0, 1, 2, 3, 5 |
| -100..-25 | 8 | -2117..-1414 | -127..0 | 267..630 | 40 | 1, 2 |
| -25..100 | 32 | -2152..-1120 | -33..100 | -124..435 | 55 | 0, 1, 2, 5, 6 |
| 100..125 | 15 | -2297..-800 | 100..120 | -156..670 | 0 | 0, 1 |
| 450..475 | 4 | -1431..-800 | 400..560 | 435..875 | 57 | 0 |

Survey basis: ground levels, banks, water height and the perimeter can be measured from this collision once imported. Masonry, foliage and dressing beyond the actor-placed torch, crate, bushes and bell are **pending capture** (room geometry or visual review).

## 8. Materials and textures (C; counts only, nothing copied)

| Item | Count |
| --- | --- |
| Extracted scene PNGs | 36: 29 textures + 7 TLUTs |
| By format | rgba16 16 (9 textures + 7 TLUTs), ci8 14, ci4 4, i4 1, ia8 1 |
| Location | all in the scene file (segment 2); the room file holds none; all 29 textures are loaded by room DLs |
| Room `Gfx` display lists / `Vtx` arrays | 37 / 28 (28 `.vtx.inc` files) |
| Room shape | cullable, 19 entries: 18 with an opaque DL, 5 with an XLU DL (4 of them empty: `DL_007430`, `007438`, `007440`, `007448`) |
| XML-declared DLs | `0x7390` calls 18 opaque DLs; `0x8CC0` calls the 4 empty XLU DLs and `DL_008C80` |
| XLU geometry | `DL_008C80` calls 7 DLs: 6 translucent `G_RM_ZB_XLU_SURF2` DLs (prim alpha 185 or 242; textures `Tex_006AE0` ci4, `Tex_008100` ci8; x -2253..-1653, y -120..132) and the water surface `DL_008B58`; plus one `G_RM_AA_ZB_XLU_DECAL2` DL, `DL_0072F0` (prim black, alpha 127, Y -120, same footprint as the Guru-Guru platform; INF: a shadow decal), called from the opaque list `DL_007390` |
| Raw triangle commands | 362 `gsSP2Triangles` + 17 `gsSP1Triangle` = 741 triangles in source (DL reuse not resolved) |
| Animated materials | 1 (two-layer scroll on segment 8, section 1.1) |

## 9. Required extraction list for the Laundry pilot (commands NOT executed)

Done: scene `Z2_ALLEY` (C above). Already extracted: `object_obj_tokeidai`. Binaries for every bank below exist under `extracted/n64-us/baserom/` (checked by `stat`): `object_cha` 7,696 B, `object_syokudai` 15,504, `object_kibako` 6,912, `object_lodmoon` 15,904, `object_fr` 46,416, `object_fu` 27,808, `object_mm` 55,904, `object_an1` 81,456, `object_an2` 17,120, `object_test3` 63,488, `object_mask_kerfay` 5,568, `object_wood02` 38,848, `object_os_anime` 11,408, `gameplay_keep` 599,520, `gameplay_field_keep` 37,520.

| Priority | Banks | Why |
| --- | --- | --- |
| Day 1 static | `object_cha`, `object_syokudai`, `object_kibako`, `object_lodmoon`, `gameplay_keep` (door skeleton and side DLs), `gameplay_field_keep` (bush) | section 6 DLs |
| Later skeletal pass | `object_fr` (frog); `gameplay_keep` skeletons for stray fairy, bug and fish | present at noon, animated |
| Later profiles | `object_mm`, `object_an1`, `object_an2`, `object_mask_kerfay`, `object_test3`, `object_fu` | Day 2/3, night, bell state |

Pattern from `tools/reference/README.md:29-32` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South and North):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_cha','object_syokudai','object_kibako','object_lodmoon','gameplay_keep','gameplay_field_keep','object_fr')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

The keep files have not been extracted with this tool before (NI, same note as the North dossier); check output size and warnings separately. North's list also includes `object_lodmoon`, `gameplay_keep` and `gameplay_field_keep`: extract once and share.

## 10. Proposed region record

**Proposal, schema not frozen (PIPE-01).** Field set follows FULL_CITY_PLAN section 3.2; `null` or `"UNKNOWN"` where source does not settle a value.

```json
{
  "_status": "proposal, schema not frozen (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.laundry_pool",
  "displayLabel": "Laundry Pool",
  "source": {
    "sceneSymbol": "Z2_ALLEY",
    "sceneEnum": "SCENE_ALLEY",
    "sceneTableIndex": "0x70",
    "entranceTableSceneIndex": "0x6D",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0; no alternates exist)",
    "rooms": { "loc.laundry_pool.room_00": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event, switch or collectible flags", "player form Human", "bell not hit (WEEKEVENTREG_HIT_LAUNDRY_POOL_BELL clear)"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_ALLEY", "bytes": 55392, "sha256": "c73517b309943976fb2ef562d715e8e878a5ff31c4dfec422ee14ce94316f0a8" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_ALLEY_room_00", "bytes": 36080, "sha256": "a82e90431561812b8a5d6f023748b7de7ff477105b0dcdca3940fe93bde03f3d" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_ALLEY/Z2_ALLEY.c", "bytes": 57206, "sha256": "f3babac54684f99b2d2c3aaa27f9f64f606e6e188af943b854ed31244199b9d8" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_ALLEY/Z2_ALLEY_room_00.c", "bytes": 67628, "sha256": "3ad10d2d4b42f2106f3f1492f6a2a60922909cbb3df0b2f88996838c1f46fc7a" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_ALLEY/Z2_ALLEY.xml", "bytes": 386, "sha256": "4b941e3041ac1abef144858a13e4e1c382c2d33edc072944764f4d46c1336e53" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedWithSouth": "INFERRED (corridor stub overlaps South's Laundry exit AABB), unverified" },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-2577, -224, -960], "max": [-240, 635, 1120] },
    "sourceCollisionVertices": { "min": [-2400, -224, -596], "max": [-800, 560, 1120] },
    "unityCollisionVertices": { "min": [-24.00, -2.24, -11.20], "max": [-8.00, 5.60, 5.96] }
  },
  "water": [
    {
      "id": "water.laundry_pool.basin",
      "sourceWaterBox": { "xMin": -2400, "xMax": -1800, "zMin": -200, "zMax": 1120, "surfaceY": -135, "properties": "0x00000101", "bgCamIndex": 1, "lightIndex": 1, "room": 0, "disabled": false },
      "unity": { "x": [-24.00, -18.00], "z": [-11.20, 2.00], "surfaceY": -1.35 },
      "poolFloorY": [-208, -200],
      "bankFloorY": [-132, -120],
      "surfaceMesh": "room DL_008B58 at Y -135, two-layer scroll on segment 8",
      "behavior": "UNKNOWN: gameplay decision (FULL_CITY_PLAN 5.5)"
    }
  ],
  "anchors": [
    { "id": "anchor.lp.spawn0_from_south", "source": [-1426, -8, 397], "unity": [-14.26, -0.08, -3.97], "status": "proposed, not surveyed" },
    { "id": "anchor.lp.spawn1_from_shop", "source": [-2121, -120, -49], "unity": [-21.21, -1.20, 0.49], "status": "proposed, not surveyed" },
    { "id": "anchor.lp.door_ta0", "source": [-2097, -120, -112], "unity": [-20.97, -1.20, 1.12], "status": "proposed, not surveyed" },
    { "id": "anchor.lp.bell_a11", "source": [-1961, -123, 530], "unity": [-19.61, -1.23, -5.30], "status": "proposed, not surveyed" },
    { "id": "anchor.lp.bridge_crest", "source": [-2077, -100, 584], "unity": [-20.77, -1.00, -5.84], "status": "proposed, not surveyed" }
  ],
  "rooms": [ { "id": "loc.laundry_pool.room_00", "sourceRoom": "Z2_ALLEY_room_00", "roomShape": "cullable, 19 entries", "displayLists": 37 } ],
  "components": [
    { "sourceKey": "Z2_ALLEY/h0/transition/ta00", "actorId": "0x005", "params": "0x0285", "class": "door/transition", "dls": ["gameplay_keep:0x22B28 gDoorSkel", "gameplay_keep:0x20BB8", "gameplay_keep:0x20D00"], "status": "selected, rest pose approximated" },
    { "sourceKey": "Z2_ALLEY/h0/room_00/a11", "actorId": "0x270", "params": "0x000F", "class": "static-mesh", "dls": ["object_cha:0x710", "object_cha:0x958"], "status": "selected, rest pose" },
    { "sourceKey": "Z2_ALLEY/h0/room_00/a01", "actorId": "0x039", "params": "0x207F", "class": "static-mesh", "dls": ["object_syokudai:0x870"], "status": "selected, unlit" },
    { "sourceKey": "Z2_ALLEY/h0/room_00/a07", "actorId": "0x081", "params": "0xA002", "class": "static-mesh", "dls": ["object_kibako:0x1180"], "status": "selected" },
    { "sourceKey": "Z2_ALLEY/h0/room_00/a13", "actorId": "0x090", "params": "0x0500", "class": "static-mesh", "dls": ["gameplay_field_keep:0x78A0"], "status": "selected, yaw approximated" },
    { "sourceKey": "Z2_ALLEY/h0/room_00/a14", "actorId": "0x090", "params": "0x0C00", "class": "static-mesh", "dls": ["gameplay_field_keep:0x78A0"], "status": "selected, yaw approximated" },
    { "sourceKey": "Z2_ALLEY/h0/room_00/a15", "actorId": "0x090", "params": "0x0C00", "class": "static-mesh", "dls": ["gameplay_field_keep:0x78A0"], "status": "selected, yaw approximated" },
    { "sourceKey": "Z2_ALLEY/h0/room_00/a16", "actorId": "0x19C", "params": "0x207F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "0xBEE8", "0xE818", "0xC368"], "status": "selected, visibility unverified" },
    { "sourceKey": "Z2_ALLEY/h0/room_00/a03", "actorId": "0x17C", "params": "0x037F", "class": "static-mesh (camera-relative)", "dls": ["object_lodmoon:0x10E0", "object_lodmoon:0x1158"], "status": "selected, special rule" }
  ],
  "collisionOwnership": { "room": "Z2_ALLEYCollisionHeader_0028A8 (400 polys, 1 water box)", "actorDynapoly": [], "actorCylinders": ["a01 stand r12 h45", "a07 r15 h30", "a11 r10 h40"] },
  "portalIds": [
    "portal.laundry_pool.south_clock_town",
    "portal.laundry_pool.curiosity_shop"
  ],
  "materialBindings": null,
  "cameraIds": [],
  "routeIds": [],
  "omissions": ["a18 En_Minifrog", "a04 En_Elforg", "a05 En_Insect", "a00 En_Fish children", "water texture scroll", "Link and Tatl"],
  "excluded": { "halfDayMask": ["a02", "a12", "a19", "a20", "a21", "a22", "a23", "a24", "a25", "a26", "a27"], "stateOrSchedule": ["a06", "a08", "a09"], "noGeometry": ["a00 controller", "a10", "a17"] },
  "unsupported": ["skeletal NPC and critter poses", "door skeleton rest pose", "two-layer texture scroll", "bell swing", "water box swim/dive behavior", "moon 9000-unit rule unless implemented"],
  "evidence": ["docs/city/districts/laundry_pool.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Player walkability of the 27-40 deg corridor ramps (polys 132-141) | Which floor-normal threshold makes the player slide (search `z_player.c` for comparisons on `floorPoly->normal.y` / floor pitch); then a controller route from spawn 0 to the bank |
| U-02 | Reachability of the Y 120 island, the wood cluster (30-55 deg) and the high slopes | Do any edges connect them to the main network within the ledge-climb heights (`unk_14/18/1C` vs `yDistToLedge`)? Then a route capture |
| U-03 | Exact swim, climb-out and underwater behavior on banks 3-15 above the surface | Read `z_player.c:8941-9140` and the climb-out-of-water branch; this informs, but does not replace, the 5.5 gameplay decision |
| U-04 | Purpose of water-box light index 1 | Grep `WaterBox_GetLightIndex` (or the light field of `WATERBOX_PROPERTIES`) consumers in `src/code/z_kankyo.c` and `z_bgcheck.c` |
| U-05 | Text of `0x180C` (door refusal) and `0x1C07` (En_Talk at the bell) | `assets/text/message_data.h` is a stub here; extract the message table (not run) or read the ROM message archive |
| U-06 | Visibility of the tower clock (a16) and the moon from inside the Laundry | Render room + actors or capture from spawn 0 and the bank |
| U-07 | Shared world frame with South | Overlay both collisions; check that Laundry floors 125-129 and South polys 158/159 coincide at Y 100 |
| U-08 | Reader of exit index 1 `0xD650` | Search cutscene data and non-room actors that can run in `SCENE_ALLEY` for `setupExitList[1]`; else record unused (PORTAL_REGISTER U4) |
| U-09 | Laundry entrance 2 (`0xDA20`, spawn 2 missing) | Check cutscene destination commands (`CS_ENTRANCE`) and `z_demo.c` tables for Laundry entrance 2; extracted scenes and `src/` have no hit |
| U-10 | Persistent cycle flag chest bit 0 with no chest actor | Grep `Flags_GetTreasure`/`Flags_SetTreasure` users that can run in `SCENE_ALLEY`; read the persistence code after `z_sram_NES.c:39` |
| U-11 | Cullable room entries 9 and 10 with radius 44 and -44 | Read the cullable draw in `src/code/z_room.c` for the meaning of a negative radius |
| U-12 | Bell-hit state: cutscene 12 (priority 900, length 60, cs camera 2 = `CAM_SET_FIXED1` at (-2080, -63, 219)) and Kafei's `HEARD_BELL_RING` pose and path | Read `EnTest3_ProcessSchedule_BellRung` / `HandleSchedule_BellRung`; record as a separate later profile |
| U-13 | Wall type 4 underwater | Does any water-state branch read `WALL_FLAG_3`? (grep `SurfaceType_GetWallFlags` in `z_player.c`) |
| U-14 | Visible water mesh wider than the box | Check in an import that the parts beyond x -1800 are hidden under banks; never derive the water volume from the mesh |
| U-15 | `PLAYER_START_MODE_E` behavior for both spawns | Read the start-mode handler table in `z_player.c` (`Player_Init` start-mode switch) |
| U-16 | Banks with no listed consumer (`object_wood02`, `object_os_anime`) | Which runtime-spawned actors in the Laundry use them (children of En_An, En_Pm, En_Test3; `z_en_hy_code.c`)? |
| U-17 | Collision header bounds larger than vertex extents | As North U-14: check how `BgCheck` uses `minBounds/maxBounds` before using them as region bounds |
| U-18 | Door rest pose | Extract `gameplay_keep` and evaluate `gDoorSkel` at frame 0 of `gDoorFierceDeityZoraOpenLeftAnim` (the `SkelAnime_Init` animation, z_en_door.c:343-344) |
| U-19 | Bush yaw, fish count/positions, fairy and bug motion | Fixed-yaw convention (PIPE-03); critters stay omitted or need a motion decision |
| U-20 | Day 2/3 and night states (Anju sitting, Postman, Kafei, Guru-Guru, rupees, lit torch, door open windows) | Separate profiles from SOURCE_REGISTER; reuse sections 2.5 and 4.4 |
| U-21 | Visual survey | Capture `cam.laundry_pool.*` under the Day 1 noon profile (REF-03); `bridge_steps` applies (section 7.2) |

**Register notes (no edits made; for the register owners):**

- PORTAL_REGISTER rows 207 and 218 list the door state as "open hours UNKNOWN". Source gives more: on Day 1 the Laundry-side approach is refused with text `0x180C` (schedule `0xC`), the shop's room_01 side has no schedule check, and the Day 2/Day 3 open windows are flag-gated (section 2.5). This refines rather than contradicts the rows.
- SOURCE_REGISTER U-09 (moon in Laundry): yes, a03 `En_Fall 0x037F` at (-320, 11000, -959), a different home from North's.
- SOURCE_REGISTER U-10 (venue access at noon) for this door: closed on Day 1 from the Laundry side.
- SOURCE_REGISTER U-12 (En_Test4 cutscenes): Laundry En_Test4 csId 9 -> entry 9 (priority 850, cs camera 0 `CAM_SET_FIXED1` at (-1769, -103, 306), additional cs 10 -> cs camera 1). Partial; the `sCsIdList` use is NI.
- REF-01/REF-02 counts all agree: 14 objects, 28 actors, 1 transition actor, 2 spawns, 4 exits (last `0x0000`), 2 entrance entries / 3 declared, exit polygons 130/131 and 40/41, arrival poses for `0xDA00` and `0xDA10`.

**Not inspected:** room DL contents beyond render modes, textures and vertex bounds; light-setting values; cutscene entries beyond the three used csIds; minimap rendering; rupee, fish and stray-fairy draw code; En_Pm, En_An, En_Test3 behavior beyond init, draw gates and the Laundry schedule branches; Curiosity Shop room_01 beyond its door, spawn 1 and exit polygons 114/115; reverse-side triggers in South beyond REF-02 values; 3DS material.

**Checks run:** read the listed docs and registers; read scene/room C, scene XML, scene/actor/object tables, `z_actor.c`, `z_scene.c`, `z_scene_proc.c`, `z_rcp.c`, `z_scene_table.c`, `z_bgcheck.c` (wall flags, water rooms), `z64bgcheck.h`, `z64scene.h`, `z64player.h`, `z64camera.h`, `z_sub_s.c`, the player age properties, and the 18 overlays named above (incl. `En_Door` schedule scripts and `En_Fish` profile); `Z2_AYASHIISHOP.c` door, spawn, entrance and exit data. Scratch Python parsers run with `python -I` (`col.py`, `islands.py`, `room.py`, `dlbounds.py`, `door.py`, `actors.py`, `support.py`, `corridor.py`, `q.py`; outputs `*_out.txt` in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref08\`) for the 28/2/1 entry decode, collision counts, water classification, floor islands, door-local coordinates, DL bounds and texture counts; SHA-256 and sizes of the five inputs; `stat` of 15 bank binaries; greps for exit-list readers and Laundry entrance senders; JSON validity of section 10.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes, text extraction, entrance decoding beyond the labeled cross-check.

**Next dependency:** REF-02 owner to fold the section 2.5 door state into rows 207/218; REF-03 Laundry capture set under `prof.d1.noon.normal`; PIPE-01 schema review of section 10 (incl. the new `water` field); then the section 9 extraction and LP-01 assembly, with the water behavior decided explicitly per FULL_CITY_PLAN 5.5.
