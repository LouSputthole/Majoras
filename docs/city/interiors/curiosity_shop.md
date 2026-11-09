# Curiosity Shop (`loc.curiosity_shop`): source inventory dossier

Task **REF-09 / INT-03** (FULL_CITY_PLAN.md sections 6 and 12). Base commit `31b20dc65b5137de012fa60d373fb609c7af0aca` on `main` (REF-09 brief; HEAD moved to `7a7a08c` PIPE-01 while this packet ran, so section 10 is written to lift cleanly through `tools/reference/region_from_dossier.py`). Source pin: local zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639` (N64 US rev 0), ZAPD-extracted `Z2_AYASHIISHOP` C dated 2026-11-08. Written 2026-11-08 by source inspection only: no ZAPD, Blender, Unity or player run; no capture; no import. Selected profile for the static list is `prof.d1.noon.normal` (Day 1, 12:00, scene layer 0, fresh save); **the venue is closed at that profile** (section 6).

All paths below are relative to `local-reference/mm-decomp/` unless they start with `docs/`, `shared/` or `tools/`. `scene.c` = `extracted/n64-us/assets/scenes/Z2_AYASHIISHOP/Z2_AYASHIISHOP.c`; `r00.c` / `r01.c` = `.../Z2_AYASHIISHOP_room_00.c` / `.../Z2_AYASHIISHOP_room_01.c`. Collision polygon `i` is at `scene.c` line `400 + i`, collision vertex `j` at `585 + j`; room_00 actor `aNN` is at `r00.c` line `43 + 3 x NN`, room_01 actor `aNN` at `r01.c` line `34 + 3 x NN`. N64 world units, Y up. Unity `(x, y, z) = N64 (x, y, -z) / 100`, physical scale provisional. The interior has its own source frame (not registered to West or Laundry).

Evidence tags: **XML** declared in scene/object XML; **HDR** present in an extracted header/list as written; **C** decoded by me from extracted C or a decomp macro; **OVL** read from the actor overlay source; **INF** inferred (reason given); **NI** not inspected.

**Plain answer (C):** the Curiosity Shop back room is **`loc.curiosity_shop.room_01`, the second room file of this same scene** (`Z2_AYASHIISHOP_room_01`), not a separate scene. No Kafei scene ID exists or is needed. room_01 has its own door to Laundry Pool and no walkable or door link to the shop room_00; the only room_01 / room_00 changes are the peephole view (entrance 2) and its return (entrance 3), both full scene reloads (section 2.6).

## 1. Scene identity

| Field | Value | Evidence |
| --- | --- | --- |
| Scene symbol / enum | `Z2_AYASHIISHOP` / `SCENE_AYASHIISHOP` (comment "Curiosity Shop", ANN :34) | `include/tables/scene_table.h:35` |
| Scene-table index | `0x0D` | same line |
| Entrance-table scene index | `0x07` `ENTR_SCENE_CURIOSITY_SHOP`; row `SCENE_ENTRANCE(sCuriosityShopEntranceTable, "Z2_AYASHIISHOP")` | `include/z64scene.h:600`; `src/code/z_scene_table.c:2580` |
| Title / draw config / restrictions | `0x10E` / `SCENE_DRAW_CFG_MAT_ANIM` / `RESTRICTIONS_INDOORS` (field meaning NI, LOCATION_REGISTER U-13) | scene_table.h:35 |
| Persistent cycle flags | `PERSISTENT_CYCLE_FLAGS_NONE` | same |
| XML | scene file segment 2; room files `Z2_AYASHIISHOP_room_00` and `_room_01` segment 3; external `misc/scene_texture_02.xml`; declared DLs r00 `0xCF78`, `0x11820`; r01 `0x4F80`, `0x54F8` | XML `assets/xml/scenes/Z2_AYASHIISHOP/Z2_AYASHIISHOP.xml` |
| Rooms | 2: `SCENE_CMD_ROOM_LIST(2, ...)`; room list scene.c:52-55 | HDR scene.c:17 |
| Scene / room headers | default only: no `SCENE_CMD_ALTERNATE_HEADER_LIST` in scene.c:15-33, r00.c:15-24 or r01.c:15-24 | HDR (grep) |
| Sound | spec 5, ambience 19, seq 44 = `Sequence_44` `NA_BGM_MARINE_RESEARCH_LAB` (ordinal assumed from table order, U-14) | HDR scene.c:16; z64scene.h:845-846; `include/tables/sequence_table.h:108` |
| Region visited | `SCENE_CMD_SET_REGION_VISITED`; `SCENE_AYASHIISHOP` is in the Clock Town row of `gSceneIdsPerRegion` | HDR scene.c:19; `src/code/z_inventory.c:428-433`; handler `src/code/z_scene.c:504` |
| Special files | `NAVI_QUEST_HINTS_NONE`, `GAMEPLAY_FIELD_KEEP` | HDR scene.c:22 |
| Skybox | external texture file `0x02`, skybox 0, weather 0, lightMode `true` = `LIGHT_MODE_SETTINGS` (lights from `lightSetting`, not time of day) | HDR scene.c:25; z64scene.h:833-834; `include/z64environment.h:48-49` |
| Env light settings | 4 entries (values not transcribed) | HDR scene.c:27, :69-78 |
| Cutscene scripts | 1: `CutsceneData_000204`, nextEntrance `0xFFFF`, spawn `0xFF` (Kafei face reveal, section 5.7) | HDR scene.c:28, :274 |
| Actor cutscenes / cs cameras | 18 / 9 | HDR scene.c:29-30, :278-295, :338-346 |
| Paths | 5 (path 0 Kafei, paths 1-4 Sakon) | HDR scene.c:23, :116-120 |
| Minimap | 2 entries, mapId `0xFFFF`, centers (0, 0, -6) and (-84, 0, -390), scale 0 | HDR scene.c:298-305 |
| Animated materials | 2 two-texture scrolls: `{1, 1}` = segment 8, second layer step x -1; `{-2, 1}` = segment 9 (last entry), second layer step x -1, y -1; segment = abs + 7 | HDR scene.c:353-366; `src/code/z_scene_proc.c:403` |
| Room commands (both rooms) | echo 4; `ROOM_BEHAVIOR(0, 0, 0, 0, false, 0)` (storm state 0); `SKYBOX_DISABLES(true, true)`; `TIME_SETTINGS(255, 255, 3)` (no forced time, speed 3); cullable shape type 2 | HDR r00.c:16-20, r01.c:16-20; macros z64scene.h:804-807, :830-831 |
| room_00 / room_01 lists | 13 objects, 6 actors / 4 objects, 7 actors | HDR r00.c:21-22 / r01.c:21-22 |
| Transition actors | 2 (`EN_DOOR` x2) | HDR scene.c:18, :48-49 |

Counts agree with `docs/city/LOCATION_REGISTER.md` row `loc.curiosity_shop`: rooms 2, Alt 0, r00 13/6, r01 4/7, transitions 2, spawns 4, exits 2, entrances 4/4.

### 1.1 Headers and profiles

No alternate header exists, and each of the four `sCuriosityShopEntrance<N>` arrays has exactly one layer row (`z_scene_table.c:145, 149, 153, 157`), so every profile loads the default scene and room headers (C). Time of day changes this venue only through actor half-day masks, door schedules and overlay time tests (sections 4 and 5).

## 2. Spawns, entrances and exits

### 2.1 Start positions (HDR scene.c:36-44; decode C)

Player params: start mode `(params & 0xF00) >> 8`, bgCam `params & 0xFF` (`include/z64player.h:17-18`); yaw `(rawRotY >> 7) & 0x1FF`. Room comes from the entrance list (2.2). Door-local `(x, y, z)` uses `Actor_WorldToActorCoords` (rotate by -yaw).

| Spawn | Position | Raw rot X / Y / Z | Yaw | Params | Start mode | bgCam | Room | Arrival use (PORTAL_REGISTER) |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (1, 0, 227) | 0x0007 / 0x5A00 / 0x007F | 180 | 0x0EFF | 0xE: walk 120 u forward at speed 2 (`z_player.c:8683-8690`) | none | 0 | entrance 0 `0x0E00`, West exit [4]: `portal.west_clock_town.curiosity_shop`. TA0-local (1, 0, -25): interior side |
| 1 | (-262, -96, -619) | 0x0007 / 0x0000 / 0x007F | 0 | 0x0EFF | 0xE | none | 1 | entrance 1 `0x0E10`, Laundry exit [2]: `portal.laundry_pool.curiosity_shop`. TA1-local (-2, 0, +7): inside the door niche, on the door's front side (U-03) |
| 2 | (-75, 82, -170) | 0x0007 / 0x0000 / 0x007F | 0 | 0x0CFF | 0xC `PLAYER_START_MODE_TELESCOPE` (`z64player.h:33`) | none | 0 | `ENTRANCE(CURIOSITY_SHOP, 2)` = `0x0E20` from Obj_Nozoki (OVL `z_obj_nozoki.c:444`). 82 u above floor 42/43, behind the counter wall: a camera eye, player not drawn (2.6) |
| 3 | (-95, 30, -311) | 0x0007 / 0x0000 / 0x007F | 0 | 0x0D06 | 0xD (walk 180 u, `z_player.c:8677-8681`) | 6 (only 5 BgCamInfo exist, U-04) | 1 | `ENTRANCE(CURIOSITY_SHOP, 3)` = `0x0E30`, telescope B exit (`z_player.c:8636`). On the peephole step, polys 118/119 (Y 30) |

### 2.2 Entrances (HDR scene.c:57-61; `src/code/z_scene_table.c:144-165`)

| Entrance | Value | Table row | Flags: fade / arrival / bit 15 | EntranceEntry {spawn, room} | Reached by |
| --- | --- | --- | --- | --- | --- |
| 0 | `0x0E00` | :145 `{ SCENE_AYASHIISHOP, 0, 0x4102 }` | 0x02 / 0x02 / 0 | {0, 0} scene.c:58 | West Clock Town door (exit [4]) |
| 1 | `0x0E10` | :149 `{ ..., 1, 0x0102 }` | 0x02 / 0x02 / 0 | {1, 1} :59 | Laundry Pool door (exit [2]); map select "Rear Entrance" (ANN `z_select.c:357-358`) |
| 2 | `0x0E20` | :153 `{ ..., 2, 0x8A95 }` | 0x15 / 0x15 / 1 | {2, 0} :60 | Obj_Nozoki peephole; map select "Peephole" (ANN `z_select.c:360-361`) |
| 3 | `0x0E30` | :157 `{ ..., 3, 0x8102 }` | 0x02 / 0x02 / 1 | {3, 1} :61 | telescope B exit respawn |

Value = `ENTRANCE(scene, spawn)` (`include/z64scene.h:711`); flag fields per PORTAL_REGISTER section 1, transition names NI.

### 2.3 Exit list (HDR scene.c:64-66) and consumers

| Idx | Raw | Decode (index / entr / layer) | Destination row | Arrival (exterior dossier) | Consumers in this scene |
| --- | --- | --- | --- | --- | --- |
| 0 | `0xD440` | 0x6A `Z2_ICHIBA` / 4 / 0 | `sWestClockTownEntrance4` `z_scene_table.c:2277` `{ SCENE_ICHIBA, 4, 0x4102 }` | West spawn 4 (-1490, 45, -56) yaw 152 (`docs/city/districts/west_clock_town.md` 2.1) | surface type 4, polys 58/59 behind TA0; `EN_TIME_TAG` r00 a02, `params & 0x1F = 0` |
| 1 | `0xDA10` | 0x6D `Z2_ALLEY` / 1 / 0 | `sLaundryPoolEntrance1` `:2403` `{ SCENE_ALLEY, 1, 0x4A14 }` | Laundry spawn 1 (-2121, -120, -49) yaw 342 (`laundry_pool.md` 2.1) | surface type 12, polys 114/115 behind TA1; `EN_TIME_TAG` r01 a03 and a04, `params & 0x1F = 1` |

No trailing `0x0000`; both entries are referenced (C).

### 2.4 Transition actors and the room each connects (HDR scene.c:48-49; decode C)

Rules: params type `(p >> 7) & 7`, type var `p & 0x7F` (`ovl_En_Door/z_en_door.h:9-20`, `:95-96`); spawned params `TRANSITION_ACTOR_PARAMS(i, p) = (i << 10) + (p & 0x3FF)` (`include/z64door.h:22-23`); yaw `DEG_TO_BINANG((rotY >> 7) & 0x1FF)`, csId `rotY & 0x7F`, mask `HALFDAYBIT_ALL`, spawned only while the current or previous room equals a side room (`src/code/z_actor.c:3600-3625`). Opening: `doorDirection = +1` when the player's door-local z >= 0 (`z_en_door.c:496`); the schedule runs only when local z > 0 (`:537-541`); the player is placed `22 x doorDirection` along the door yaw (`z_player.c:6756-6757`) and the exit probe is `-22 x doorDirection` along it at door Y + 10 (`:6779-6783`). Non-framed doors load no collision mesh (`z_en_door.c:338-341`).

| TA | Position | rotY raw / yaw / cs | Sides (room, bgCam) front / back | Params (spawned) | Type / `ENDOOR_SCH_TYPE_*` | Door leaf | Probe from inside -> poly -> exit | Connects | Inside / outside use |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | (0, 0, 252) | -19329 = 0xB47F / 360 = 0 deg / none | 0, -1 / 0, -1 | 0x0284 (0x0284) | 5 `SCHEDULE` / 4 `CURIOSITY_SHOP` (`z_en_door.h:57`) | `sObjectInfo[21]` `object_wdor01` `gLotteryCuriosityShopMayorHouseDoorDL` 0x548 on `gDoorSkel` limb 4 (`z_en_door.c:216`, `:313-314`, `:638-660`) | (0, 10, 274) -> 58 -> exit+1 1 -> [0] `0xD440` | **room_00 <-> West Clock Town** | inside (local -Z, spawn 0 at -25): schedule skipped, always openable. Outside: West TA5 `0x0284` refuses outside 22:00-04:59 |
| 1 | (-264, -96, -612) | 23167 = 0x5A7F / 180 / none | 1, -1 / 1, -1 | 0x0285 (0x0685) | 5 `SCHEDULE` / 5 `LAUNDRY_POOL` (`z_en_door.h:58`) | `sObjectInfo[22]` gameplay_keep `gDoorLeftDL` 0x20BB8 / `gDoorRightDL` 0x20D00 by eye yaw (`z_en_door.c:217`, `:645-655`) | (-264, -86, -634) -> 114 -> exit+1 2 -> [1] `0xDA10` | **room_01 <-> Laundry Pool** | inside (local -Z, world z > -612): always openable. Outside: Laundry TA0 `0x0285` opens only Day 2 15:10-21:59 with `LETTER_TO_KAFEI_SCHEDULED_DAY_2` and Day 3 13:00-21:59 with `KAFEI_ENTRUSTED_LINK` (5.5) |

Both doors face out of the venue (local +Z = the exit pad), carry the same schedule type as their exterior twin, and change scene only when opened from inside: a probe from the pad side lands on poly 47 (TA0) or 146 (TA1), neither with an exit (C, scratch `col_out.txt`). Neither door has different rooms on its two sides, and the scene has no `EN_HOLL`, so **no transition actor joins room_00 and room_01** (C).

### 2.5 Collision surfaces carrying exits (C from scene.c:392-396 and the polygon list)

| Surface type | data[0] / data[1] | Exit idx | Polys | Extent x / y / z | Behind | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| 4 | 0x00000100 / 0x000017C2 | 0 West | 58, 59 | -60..60 / 0 / 270..375 | TA0, local z +18..+123 | flat stone floor, bgCam 0. The threshold (polys 52/53, z 240..270) is closed from inside by wall 38/39 at z 240, front facing -Z; only the door probe reaches the pad (C) |
| 12 | 0x00000204 / 0x000017C2 | 1 Laundry | 114, 115 | -364..-164 / -96 / -910..-630 | TA1, local z +18..+298 | flat stone floor, bgCam 4 (ROOM0). Kafei (r01 a01) and En_Test4 (r01 a05) stand on it |

Both exit surfaces are floors (no wall carries exit bits); no conveyor, no xpFlags, no vIB flags on any of the 182 polygons (C).

### 2.6 Shop, back room and private-entrance relationships (resolves LOCATION_REGISTER U-08)

- **Floor connectivity** (vertex-on-edge tolerance 1.5 u; scratch `connect.py`): 8 floor groups. Group 1 = shop room_00 + threshold + West pad (polys 42-47, 52, 53, 58, 59; Y 0; z -252..375). Group 0 = room_01 + stair passage + door niche + Laundry pad (102-105, 114, 115, 144-151, 172, 173; Y -96..0; z -910..-300). No floor of one group touches the other; the only riser adjacency found is between two room_01 furniture tops (groups 6/7, dy 17) (C).
- **Gap**: room_00's lowest collision z is -252 (floor 42/43) and its room DLs stop at z -216; room_01's peephole alcove wall (polys 108/109) is at z -288 and its DLs start at z -257. No polygon lies in between (C).
- **Private entrance**: the back room's only player entrance is the Laundry Pool door (TA1, exit [1], entrance 1); the shop's only entrance is the West door (TA0, exit [0], entrance 0) (C).
- **Peephole (special, scene reload).** In `SCENE_AYASHIISHOP`, Obj_Nozoki (r01 a02, (-96, 70, -288), at the alcove back wall) skips object, draw and dynapoly setup and runs `func_80BA3230` (OVL `z_obj_nozoki.c:73-76`). It offers talk only if `WEEKEVENTREG_CAN_USE_CURIOSITY_SHOP_PEEPHOLE` (64 0x20, `include/z64save.h:1249`) is set **and** `EN_TEST3` (Kafei) exists with a non-NULL draw (`:408-430`). Accepting sets `nextEntrance = ENTRANCE(CURIOSITY_SHOP, 2)` (`:442-445`) -> spawn 2, room 0, telescope start: player draw NULL, `ACTORCTX_FLAG_TELESCOPE_ON`, focus rot (0x9A6, 0x2102), actionVar2 2 (`z_player.c:8654-8670`); camera `CAM_SET_TELESCOPE` in `CAM_MODE_DEKUHIDE`, no zoom in this scene (`:8617-8619`, `:8649-8650`). B sets `respawn[RESPAWN_MODE_DOWN].entrance = ENTRANCE(CURIOSITY_SHOP, 3)`, `respawnFlag = -2`, circle transition (`:8625-8644`) -> spawn 3, room 1. On the first return (`curSpawn == 3`, `WEEKEVENTREG_64_40` clear) the peephole auto-talks text 0x297A and then sets 64 0x40 (`z_obj_nozoki.c:414-417`, `:436-440`).
- **Result**: room_00 is seen from room_01 only as a fixed telescope view; the peephole is not a walkable route (C: start mode 0xC plus disjoint floors).

### 2.7 Cross-check against the exterior dossiers and registers

| Item | Other document | This scene | Result |
| --- | --- | --- | --- |
| West door | west 2.4.1 TA5 (-1510, 45, -12), `0x0284` `CURIOSITY_SHOP`, closed except 22:00-05:00, text 0x180A | TA0 `0x0284`, same type; script `not if_time_range (22, 0, 5, 0)` returns 0xA (`ovl_En_Door/scheduleScripts.schl:124-130`) | agree; precise window 22:00 <= t <= 04:59 (`end - 1`, `src/code/z_schedule.c:33-42`; 06:00 shift `include/z64scripts.h:7`) |
| West -> shop | PORTAL_REGISTER row 186: `0x0E00`, spawn 0 (1, 0, 227) yaw 180 room 0 mode 0xE | 2.1 / 2.2 | agree |
| Shop -> West | row 216: exit [0] `0xD440`, surf 4, polys 58/59, "TA#0 70u" | probe from TA0 lands on poly 58 | agree; door association now decoded, not only by distance |
| Laundry door | laundry 2.5 TA0 `0x0285` `LAUNDRY_POOL`, locked all Day 1 | TA1 `0x0285`, same type | agree |
| Laundry -> back room | row 207: `0x0E10`, spawn 1 (-262, -96, -619) yaw 0 room 1 mode 0xE | 2.1 / 2.2 | agree |
| Back room -> Laundry | row 218 and laundry 2.5: exit [1] `0xDA10`, surf 12, polys 114/115, x -364..-164, z -910..-630; inside door always usable | same | agree |
| Kickouts | rows 217, 219: 05:00 GENERIC [D1d..D2n]; 22:00 BACK_1 [D2], BACK_2 [D3] | 5.1, 5.5 | agree |
| Transition lines | LOCATION_REGISTER 4.4 cites scene.c:47-50 | list declaration :47, entries :48-49 | minor citation offset only |
| Open hours | rows 186/207/216/218 "open hours UNKNOWN"; SOURCE_REGISTER U-10 | 5.1 and 5.5 | resolved here |

### 2.8 Portal IDs

Registered (frozen in `shared/city/ids.json`): `portal.curiosity_shop.west_clock_town` (door; TA0 + surface 4) and `portal.curiosity_shop.west_clock_town.kickout` (special; r00 a02), reverse `portal.west_clock_town.curiosity_shop` -> spawn 0; `portal.curiosity_shop.laundry_pool` (door; TA1 + surface 12) and `portal.curiosity_shop.laundry_pool.kickout` (special; r01 a03/a04), reverse `portal.laundry_pool.curiosity_shop` -> spawn 1.

Proposed, **not registered** (register owner adds them to PORTAL_REGISTER section 4.3 and reruns `build_city_ids.py`; frozen `.rN_rM` qualifier):

| Proposed ID | Mode | Trigger | Arrival | Condition |
| --- | --- | --- | --- | --- |
| `portal.curiosity_shop.curiosity_shop.r1_r0` | special | Obj_Nozoki talk (r01 a02) -> `0x0E20` | spawn 2, room 0, telescope view | 64 0x20 set and Kafei drawn |
| `portal.curiosity_shop.curiosity_shop.r0_r1` | special | B in telescope mode -> respawn `0x0E30` | spawn 3, room 1, peephole step | always, from spawn 2 |

No `room`-mode portal exists for this scene.

## 3. Object banks (IDs from `include/tables/object_table.h`)

An actor fails to spawn if its profile object is not loaded (`Actor_SpawnAsChildAndCutscene`, `src/code/z_actor.c:3517-3523`); En_GirlA kills itself when its item object is missing (OVL `z_en_girla.c:168-172`).

### 3.1 room_00 (HDR r00.c:26-40)

| # | Bank | Object ID (line) | Consumer | Day 1 static |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_FSN` | 0x1AB (:444) | En_Fsn a00 (profile object) | no (night mask) |
| 1 | `OBJECT_TEST3` | 0x01C (:45) | no room_00 actor (En_Test3 lives in room_01; U-05) | no |
| 2 | `OBJECT_GI_BOMBPOUCH` | 0x098 (:169) | En_GirlA `SI_BOMB_BAG_20_1/30_1/40` (`z_en_girla.c:107-112`), spawned by En_Fsn | no |
| 3 | `OBJECT_BOJ` | 0x0E3 (:244) | En_Suttari (Sakon) a01 | no |
| 4 | `OBJECT_OBJ_TOKEIDAI` | 0x18C (:413) | Obj_Tokeidai a05 | yes (already extracted) |
| 5 | `OBJECT_GI_BOTTLE` | 0x09E (:175) | En_GirlA `SI_BOTTLE` (`:139`) | no |
| 6 | `OBJECT_GI_SWORD_4` | 0x1FB (:524) | En_GirlA `SI_SWORD_GREAT_FAIRY` (`:141`) | no |
| 7 | `OBJECT_GI_SWORD_1` | 0x148 (:345) | En_GirlA `SI_SWORD_KOKIRI` (`:143`) | no |
| 8 | `OBJECT_GI_SWORD_2` | 0x1F9 (:522) | En_GirlA `SI_SWORD_RAZOR` (`:145`) | no |
| 9 | `OBJECT_GI_SWORD_3` | 0x1FA (:523) | En_GirlA `SI_SWORD_GILDED` (`:147`) | no |
| 10 | `OBJECT_GI_MASK06` | 0x265 (:630) | En_GirlA `SI_MASK_ALL_NIGHT` (`:105`) | no |
| 11 | `OBJECT_WDOR01` | 0x274 (:645) | En_Door TA0 leaf | yes |
| 12 | `OBJECT_HARIKO` | 0x1F2 (:515) | Obj_Hariko a03 | yes |

### 3.2 room_01 (HDR r01.c:26-31)

| # | Bank | Object ID | Consumer | Day 1 static |
| --- | --- | --- | --- | --- |
| 0 | `OBJECT_FSN` | 0x1AB | En_Fsn back room a00 | no (Day 3 mask) |
| 1 | `OBJECT_TEST3` | 0x01C | En_Test3 (Kafei) a01 | no (not drawn on Day 1) |
| 2 | `OBJECT_BOJ` | 0x0E3 | no room_01 actor (U-05) | no |
| 3 | `OBJECT_OBJ_TOKEIDAI` | 0x18C | Obj_Tokeidai a06 | yes |

Implicit: `gameplay_keep` 0x001 (En_Door, En_Time_Tag, En_Test4, Obj_Nozoki profiles; TA1 leaf) and `gameplay_field_keep` 0x002 (special file).

## 4. Actor tables

### 4.1 Decode rules (verified in `src/code/z_actor.c`)

- `Actor_SpawnEntry` (3628-3654): ID `id & 0x1FFF`; rotation `(raw >> 7) & 0x1FF` per axis; ID flag 0x8000 / 0x4000 / 0x2000 makes rotY / rotX / rotZ a plain integer (wrapped to -180..180); csId `rotY & 0x7F` (0x7F none, 3570-3574); half-day mask `((rotX & 7) << 7) | (rotZ & 0x7F)`, 0 = all (3576-3580). Room entries are written `SPAWN_ROT_FLAGS(rot, flags) = (rot << 7) | flags` (`include/z64scene.h:16`).
- `Actor_SpawnSetupActors` (2546-2577) spawns entries whose mask has the current bit; `Actor_InitHalfDaysBit` (2491-2499): Day 1 12:00 = 0x080. No ID here has bit 0x800, so the `EVENTINF_17` rule never applies.
- Only the loaded room's list spawns: room_00 after entrances 0/2, room_01 after 1/3 (2.2).
- `EN_TIME_TAG | 0xE000`: all three axes are integers; kickout hour = rot.x, minute = rot.y (`ovl_En_Time_Tag/z_en_time_tag.h:14-15`).

Classes: `static-mesh`, `posed-skeletal`, `npc-scheduled`, `billboard-effect`, `controller-invisible`, `door-transition` (frozen vocabulary). "D1d" = present at Day 1 12:00 by mask.

### 4.2 room_00 default header (6 entries, HDR r00.c:42-61)

| # | Actor (ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_FSN` (0x1C4) | (-27, 3, -132) | 0x0002 / 0x0009 / 0x0055 | 0 / 0 / 0 (shape +180, 4.4) | 0x155 D0n, D1n, D2n, D3n, D4n | 9 | 0x0000 shop | object_fsn | npc-scheduled | no |
| 1 | `ACTOR_EN_SUTTARI` (0x237) | (14, 0, 322) | 0x0000 / 0x5A10 / 0x0010 | 0 / 180 / 0 | 0x010 D2n | 16 | 0x83FF | object_boj | npc-scheduled | no |
| 2 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (50, 0, -126) | 0x0281 / 0x007F / 0x0070 | 5 / 0 / 0 (integers) | 0x0F0 D1d, D1n, D2d, D2n | none | 0x00C0 | gameplay_keep | controller-invisible | yes |
| 3 | `ACTOR_OBJ_HARIKO` (0x20A) | (-113, 48, -103) | 0x0007 / 0x6F7F / 0x007F | 0 / 222 / 0 | 0x3FF all | none | 0x0000 | object_hariko | static-mesh | yes |
| 4 | `ACTOR_EN_TEST4` (0x15A) | (-1, 0, 333) | 0x0007 / 0x000D / 0x007F | 0 / 0 / 0 | 0x3FF all | 13 | 0x0100 | gameplay_keep | controller-invisible | yes |
| 5 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-121, 75, -156) | 0x0007 / 0x2D7F / 0x007F | 0 / 90 / 0 | 0x3FF all | none | 0x907F (type 9) | object_obj_tokeidai | static-mesh | yes |

a01 and a04 stand on the West exit pad behind TA0 (door-local z +70 and +81); a00, a02, a03 and a05 are behind the counter wall (z < -48) (C).

### 4.3 room_01 default header (7 entries, HDR r01.c:33-55)

| # | Actor (ID) | Position | Raw rot X / Y / Z | Rot X / Y / Z | Mask | CS | Params | Bank | Class | D1d |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 0 | `ACTOR_EN_FSN` (0x1C4) | (-155, 0, -321) | 0x0000 / 0x4B7F / 0x000C | 0 / 150 / 0 | 0x00C D3d, D3n | none | 0x0001 back room | object_fsn | npc-scheduled | no |
| 1 | `ACTOR_EN_TEST3` (0x159) | (-260, -96, -779) | 0x0007 / 0x000F / 0x007F | 0 / 0 / 0 | 0x3FF all | 15 | 0x01E0 | object_test3 | npc-scheduled | yes by mask; draw NULL on Day 1 (4.4) |
| 2 | `ACTOR_OBJ_NOZOKI` (0x233) | (-96, 70, -288) | 0x0007 / 0x007F / 0x007F | 0 / 0 / 0 | 0x3FF all | none | 0x0000 | gameplay_keep (profile) | controller-invisible | yes |
| 3 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-17, 12, -471) | 0x0B00 / 0x007F / 0x0030 | 22 / 0 / 0 (integers) | 0x030 D2d, D2n | none | 0x0141 | gameplay_keep | controller-invisible | no |
| 4 | `ACTOR_EN_TIME_TAG \| 0xE000` (0x1AB) | (-83, -45, -551) | 0x0B00 / 0x007F / 0x000C | 22 / 0 / 0 | 0x00C D3d, D3n | none | 0x0161 | gameplay_keep | controller-invisible | no |
| 5 | `ACTOR_EN_TEST4` (0x15A) | (-264, -96, -685) | 0x0007 / 0x000E / 0x007F | 0 / 0 / 0 | 0x3FF all | 14 | 0x0500 | gameplay_keep | controller-invisible | yes |
| 6 | `ACTOR_OBJ_TOKEIDAI` (0x19C) | (-169, 82, -304) | 0x0007 / 0x5A7F / 0x007F | 0 / 180 / 0 | 0x3FF all | none | 0xA07F (type 10) | object_obj_tokeidai | static-mesh | yes |

a01 and a05 stand on the Laundry exit pad (TA1-local z +167 and +73); Kafei enters through TA1 along path 0 (5.5). The two EN_DOOR rows are in 2.4 (scene list, `HALFDAYBIT_ALL`).

### 4.4 Overlay findings per actor type (OVL; file = `src/overlays/actors/ovl_<Name>/z_<name>.c`)

| Actor | Init / draw | Presence and state gates |
| --- | --- | --- |
| En_Fsn (Curiosity Shop Man) | flex `gFsnSkel` (object_fsn 0x13320), limb max +1 bug (`z_en_fsn.h:14`); params bit 0: 0 shop, 1 back room (`z_en_fsn.h:8-9`). Shop: shape yaw + 180, attention off, cutscenes 9-12, `EnFsn_InitShop` (`:1480-1484`); back room: cylinder r18 h64, `EnFsn_IdleBackroom` (`:1485-1499`) | shop: night masks only. Back room: killed if `WEEKEVENTREG_RECOVERED_STOLEN_BOMB_BAG` (33 0x08) or `WEEKEVENTREG_SAKON_DEAD` (79 0x40) (`:1486-1488`). Mechanics in 5.2-5.5 |
| En_Suttari (Sakon) | flex `object_boj_Skel_00C240`, anim `object_boj_Anim_00071C` (`:965-967`); path index `(p & 0x7E00) >> 9 = 1` (`z_en_suttari.h:12`) -> paths 1-4 | in this scene killed if `RECOVERED_STOLEN_BOMB_BAG` (`:1013-1017`); otherwise action `func_80BAD7F8` with cs 16/17 (`:1018-1024`); schedule 5.4 |
| En_Time_Tag (kickout) | type `(p & 0xE000) >> 13 = 0` `TIMETAG_KICKOUT_DOOR`; text `(p & 0x1FE0) >> 5`; exit `p & 0x1F` (`z_en_time_tag.h:10-15`) | fires at the exact hour:minute when not in a cutscene, unless `WEEKEVENTREG_KICKOUT_WAIT` defers it (`z_en_time_tag.c:315-338`); text `0x1883 + text` (`:336`); exit `setupExitList[exit]` (`:285-296`) |
| Obj_Hariko (little cow statue head) | scale 0.1; draws `gHarikoBodyDL` 0x80 then `gHarikoFaceDL` 0x110 under head rotation (`z_obj_hariko.c:37-104`; XML `object_hariko.xml:4-5`) | always; bobs only while a quake is active (`:74-85`): static at rest |
| En_Test4 (three-day controller) | stars `(p >> 10) x 100`: 0 (r00), 100 (r01); bell volume index `(p >> 5) & 0xF` = 8 (`z_en_test4.h:8-9`); profile draw NULL | always; csId 13 / 14 feed the day/night transition list (`z_en_test4.c:326-337`) |
| Obj_Tokeidai type 9 / 10 | wall clock scale 0.02 / small wall clock scale 0.01, `ObjTokeidai_Clock_Draw`, `ObjTokeidai_Clock_Init`, idle `ObjTokeidai_WallClock_Idle` (`z_obj_tokeidai.c:247-259`, `:104-114`, `:719`) | always; time-posed like the tower clock but with `gWallClockClockFaceDL` 0xF518 (`:834-838`) |
| En_Test3 (Kafei) | Player-derived (`playerInit` with `gKafeiSkel`, object_test3 0xF7EC); `room = -1`; path `p & 0x1F = 0`, `(p >> 5) & 0xF = 0xF` -> follow schedule (`z_en_test3.h:10-11`, `z_en_test3.c:572-626`) | schedule `ovl_En_Test3/scheduleScripts.schl`: Day 1 has no `SCENE_AYASHIISHOP` branch -> result 0 -> `StopDrawing` sets draw NULL (`z_en_test3.c:1130-1155`, `:649-652`). Wears Keaton's Mask unless Day 3 with `KAFEI_ENTRUSTED_LINK` and no bomb bag recovered (`:603-606`) |
| Obj_Nozoki | in this scene: `unk_15C = 4`, action `func_80BA3230`, no draw, no dynapoly (`z_obj_nozoki.c:73-76`) | peephole controller, 2.6 |
| En_Door x2 | 2.4 | 2.4, 5.1, 5.5 |

### 4.5 Counts

| | room_00 | room_01 | Scene TAs |
| --- | --- | --- | --- |
| Entries | 6 | 7 | 2 |
| Day 1 daytime by mask | 4 (excluded a00, a01) | 4 (excluded a00, a03, a04) | 2 (one per loaded room) |
| static-mesh | 2 | 1 | 0 |
| npc-scheduled | 2 | 2 | 0 |
| controller-invisible | 2 | 4 | 0 |
| door-transition | 0 | 0 | 2 |

## 5. Venue mechanics index (INT-03 scope)

### 5.1 Opening hours and shop presence (night-only venue)

| Mechanism | Rule | Citation |
| --- | --- | --- |
| West door schedule (`ENDOOR_SCH_TYPE_CURIOSITY_SHOP`) | open (none) only for 22:00 <= t <= 04:59, any day; otherwise text 0x1800 + 0xA = 0x180A and the door refuses | `ovl_En_Door/scheduleScripts.schl:123-130`; table `z_en_door.c:54`; refusal `:537-547` |
| Inside door TA0 | no schedule from inside; always openable | `z_en_door.c:537` |
| Shopkeeper | En_Fsn a00 mask 0x155: every night half-day (D0-D4), never by day | r00.c:43-45; `z_actor.c:2562-2571` |
| 05:00 kickout | r00 a02 `0x00C0`: 05:00, text 6 `TIMETAG_KICKOUT_TEXT_GENERIC` (0x1889), exit [0] West; mask D1d-D2n, so **no 05:00 kickout on Night 3** | r00.c:49-51; `z_en_time_tag.h:49`; `z_en_time_tag.c:315-338` |
| Kickout deferral | `WEEKEVENTREG_KICKOUT_WAIT` (63 0x01) / `KICKOUT_TIME_PASSED` (63 0x02) hold the kickout for an external system | `z_en_time_tag.c:300-338`; `include/z64save.h:1227-1229` |
| Mask gate | interaction runs only with no mask; any mask -> welcome line then end (Deku 0x29FC, Goron/Zora 0x29FD, Kafei's Mask 0x2364, other 0x29FE; none 0x29CC) | `z_en_fsn.c:137-155`, `:822-842` |
| Counter talk box | offered only while the player stands in x -50..15, y > 0, z -35..-20 (the Y 12 counter step, polys 62/63); accepting snaps the player to (1, y, -34) | `z_en_fsn.c:770-780` |

### 5.2 Stock: items the shop sells (En_Fsn shop + En_GirlA)

| Step | Rule | Citation |
| --- | --- | --- |
| Has stock? | Day != 3: only if a stolen item is held; Day 3: a stolen item or the special item not yet bought. No stock -> straight to buying from Link | `z_en_fsn.c:348-362`, `:827-833` |
| Special item (Day 3 only, once per cycle) | `!WEEKEVENTREG_BOUGHT_CURIOSITY_SHOP_SPECIAL_ITEM` (33 0x04) and day 3: `SI_BOMB_BAG_30_1` if the bomb bag is not recovered and Sakon is not dead, else `SI_MASK_ALL_NIGHT` | `z_en_fsn.c:315-324`; flags `z64save.h:1008-1009`, `:1380` |
| Stolen items | `STOLEN_ITEM_1/2` = top bytes of `saveInfo.stolenItems` (`z64save.h:600-608`), set by En_Thiefbird (`ovl_En_Thiefbird/z_en_thiefbird.c:283-287`), cleared by `Sram_SaveEndOfCycle` (`src/code/z_sram_NES.c:611-612`). Mapping: bottle -> `SI_BOTTLE`; Great Fairy / Kokiri / Razor / Gilded sword -> `SI_SWORD_*` | `z_en_fsn.c:326-346` |
| Shelf | up to 3 En_GirlA at (-5, 35, -95), (13, 35, -95), (31, 35, -95), in order special, stolen 1, stolen 2 | `z_en_fsn.c:119-123`, `:364-405` |
| Item rows (`sShopItemEntries`) | All-Night Mask 0x13: object_gi_mask06, text 0x29D9 / 0x29DA, `GI_MASK_ALL_NIGHT` (`z_en_girla.c:105-106`); Bomb Bag 0x15: object_gi_bombpouch, 0x29DB / 0x29DC, `GI_BOMB_BAG_30` (`:109-110`); bottle 0x24: 0x29F8 / 0x29F9, `GI_BOTTLE_STOLEN` (`:139-140`); swords 0x25-0x28: 0x29F2 / 0x29F3 (Great Fairy), 0x29F4 / 0x29F5, `GI_SWORD_*_STOLEN` (`:141-148`) | enum `z_en_girla.h:83-104` |
| Can buy | All-Night: rupees >= price (`z_en_girla.c:272-277`); bomb bag: refused with 0x29DD if the bomb-bag upgrade is >= 2, else rupees (`:279-287`); bottle, swords: rupees (`:344-356`). Short of rupees -> 0x29F0 | `z_en_fsn.c:1190-1238` |
| Prices | `play->msgCtx.firstChoicePrice`, parsed from the item's choice message (`src/code/z_message.c:2073-2074`). **Values UNKNOWN here**: `assets/text/message_data.h` is a 492-byte stub (U-01) | `z_en_girla.c:186-287` |
| Purchase effects | special item sets 33 0x04 (`z_en_fsn.c:1194-1196`); stolen slot cleared (`:1214-1218`); shelf item hidden (`EnGirlA_Bought`, `z_en_girla.c:548-551`). Bomb bag: `EnGirlA_BuyBombBag` gives nothing (params 1-3 vs cases 20-22, `@bug`, `:446-466`); the bag arrives through `Actor_OfferGetItem(GI_BOMB_BAG_30)` (`z_en_fsn.c:1206`) | |
| Bombers' Notebook | All-Night Mask purchase queues `RECEIVED_ALL_NIGHT_MASK` | `z_en_fsn.c:1040-1046` |

### 5.3 Sell-back: the shop buying bottled goods from Link

| Step | Rule | Citation |
| --- | --- | --- |
| Offer | Link shows a C-button item; price = `gItemPrices[item]` for items below `ITEM_MOONS_TEAR`, else 0 | `z_en_fsn.c:956-988` (:966) |
| Price table (`gItemPrices`, `src/code/z_inventory.c:325-375`) | 20: red, green and blue potion, fairy, milk, fish, bug, spring water, hot spring water, Zora egg, Hylian loach (:345-364); 5: half milk, mushroom (:351, :361); 50: blue fire, poe (:354-355); 200: big poe, gold dust, Chateau Romani (:356, :360, :363); everything else 0 | OVL |
| Price 0 | text 0x29CF, the shopkeeper keeps asking | `z_en_fsn.c:970-973`, `:869-872` |
| Accept | payout as a rupee get-item: 5 blue, 10 `GI_RUPEE_10`, 20 red, 50 purple, 200 huge | `z_en_fsn.c:1004-1026` |
| Bottle | after the payout the held bottle becomes empty (`Player_UpdateBottleHeld(..., ITEM_BOTTLE, PLAYER_IA_BOTTLE_EMPTY)`) | `z_en_fsn.c:1047-1049` |

### 5.4 Sakon's Night 2 visit (room_00)

| Item | Rule | Citation |
| --- | --- | --- |
| Presence | a01 mask D2n only; killed if `RECOVERED_STOLEN_BOMB_BAG` | r00.c:46-48; `z_en_suttari.c:1013-1017` |
| Schedule (`save.day == 2`, i.e. after midnight of Night 2) | 00:25-00:30 result 8, 00:30-00:45 10, 00:45-01:00 2, 01:00-01:15 11, 01:15-01:20 9 (time paths); none if the bomb bag is recovered | `ovl_En_Suttari/scheduleScripts.schl:1-30` |
| Counter scene | result 2 places Sakon at (-16, 12, -16) and sets `ENFSN_HAGGLE` on the shopkeeper; talking to him sets `WEEKEVENTREG_81_04`; results 9/11 clear the haggle | `z_en_suttari.c:925-936`, `:1348-1357`, `:898-908`; `z_en_fsn.c:783-820` |
| Paths | path 1 (14, 0, 322) -> (3, 0, 169), chained 2 -> (-16, 12, -16), 3 -> (-3, 0, 141), 4 -> (9, 0, 323) (customValue 0-3) | scene.c:94-113, :117-120 |
| Sakon in room_00 outside 00:25-01:20 | not established | U-06 |

### 5.5 Back-room privacy and the Kafei / Anju chain (room_01)

| Gate | Rule | Citation |
| --- | --- | --- |
| Laundry door (`ENDOOR_SCH_TYPE_LAUNDRY_POOL`) | Day 2: open 15:10-21:59 only with `WEEKEVENTREG_LETTER_TO_KAFEI_SCHEDULED_DAY_2` (28 0x08); Day 3: open 13:00-21:59 only with `WEEKEVENTREG_KAFEI_ENTRUSTED_LINK` (51 0x08); every other time, all of Day 1: text 0x180C | `scheduleScripts.schl:132-157`; `z64save.h:953`, `:1116` |
| Kafei in the shop scene | Day 1: none -> not drawn. Day 2 with `LETTER_TO_KAFEI_SCHEDULED_TODAY` (51 0x02): 15:55-16:10 walks in (time path), 16:10-22:10 `MEETING_LINK_IN_CURIOSITY_SHOP`. Day 3 with the bomb bag recovered or Sakon dead, and `KAFEI_ENTRUSTED_LINK`: `WAITING_AT_CURIOSITY_SHOP`. Otherwise he is elsewhere (Ikana, Sakon's hideout) | `ovl_En_Test3/scheduleScripts.schl:1-74`; `z64save.h:1114` |
| Kafei path | path 0, 12 points: (-260, -96, -779) on the Laundry pad -> through TA1 -> passage -> (-136, 0, -340) on the upper floor; Kafei opens the door himself (`EnTest3_IsOpeningDoor`) | scene.c:80-92, :116; `z_en_test3.c:719-735` |
| Talks | recognises Link only with no mask, Bunny Hood, Postman's Hat, Keaton or Kafei's Mask, else 0x2968 (`z_en_test3.c:742-758`); first meeting 0x2969-0x296B, face-reveal cutscene (cs 15), pendant (`KAFEI_ENTRUSTED_LINK` set, `:1178-1191`), peephole enabled after the talk (`CAN_USE_CURIOSITY_SHOP_PEEPHOLE`, `:1203-1205`, effects table `:165-168`) | speak data `:366-389` |
| Back-room kickouts (22:00) | a03 `0x0141`: Day 2 (D2d+D2n), text 10 `CURIOSITY_SHOP_BACK_1` (0x188D); a04 `0x0161`: Day 3 (D3d+D3n), text 11 `BACK_2` (0x188E); both exit [1] Laundry | r01.c:43-48; `z_en_time_tag.h:53-54` |
| Shopkeeper in the back room (Day 3) | a00 mask D3; absent once the bomb bag is recovered or Sakon is dead (so he and the Day 3 Kafei are never both here). Talk: with `WEEKEVENTREG_RECEIVED_PRIORITY_MAIL` (80 0x10) -> 0x29E4 only; else 0x29E0, Keaton's Mask (`GI_MASK_KEATON`) if not owned, then Letter to Mama (`GI_LETTER_TO_MAMA`) and the flag | `z_en_fsn.c:157-220`, `:1422-1448`, `:1486-1488`; `z64save.h:1389` |
| Upstream letter chain (city, not this scene) | Anju offers `GI_LETTER_TO_KAFEI` (`ovl_En_An/z_en_an.c:588`); a postbox deposit sets `LETTER_TO_KAFEI_SENT_IN_TIME` / `_LATE` (`ovl_En_Pst/z_en_pst.c:636-656`); the postman pickup sets `_SCHEDULED_DAY_2` or `_DAY_3`, `_TODAY`, `POSTMAN_RECEIVED_LETTER_TO_KAFEI` (`ovl_En_Pm/z_en_pm.c:945-966`; `src/code/z_kankyo.c:1659-1674`); Kafei's Laundry bell flag `WEEKEVENTREG_HIT_LAUNDRY_POOL_BELL` (`ovl_En_Cha/z_en_cha.c:90-94`) | flags `z64save.h:943-954`, `:1112-1118`, `:1513`, `:1522-1523` |

Fresh save: every room_01 gate is closed on all three days, so the back room is never reachable without the letter chain (C).

### 5.6 Peephole (room_01 -> room_00 view)

Indexed in 2.6. Additionally: Kafei turns to the peephole during his talk (`ENTEST3_TALK_LOOK_AT_PEEPHOLE`, focus on Obj_Nozoki, `z_en_test3.c:516-527`); speak data [8]-[9] and [11] (0x2978, 0x297A) (`:375-378`). While the telescope is on, `ACTORCTX_FLAG_TELESCOPE_ON` suppresses En_Test4 day-change cutscenes (`z_en_test4.c:54`, `:75`). Which room_00 actors the view shows at each window: U-07.

### 5.7 Cameras and cutscenes

| Source | Entry | Setting / data | User |
| --- | --- | --- | --- |
| BgCamInfo 0 (scene.c:384, data :369-371) | CIRCLE3 0x25 (`z64camera.h:184`) | pos (110, 158, 194), rot Y -22391 (-123.0 deg), fov -1, 250, 30 | surface types 0, 1, 3, 4, 5: room_00 shell and West pad |
| BgCamInfo 1 (:385, :372-374) | CIRCLE3 | pos (-119, 82, -478), rot 0, fov -1 | types 7, 8, 9, 10, 13: room_01 upper floor |
| BgCamInfo 2 (:386, :375-377) | CIRCLE3 | pos (36, 67, -593), rot Y -16384 (-90 deg) | types 11, 15: room_01 passage and NE floor |
| BgCamInfo 3 (:387, :378-380) | CIRCLE0 0x0E (`:161`) | pos (37, 55, 65), rot Y -28399 (-156.0 deg) | type 6: counter step |
| BgCamInfo 4 (:388) | ROOM0 0x3F (`:210`), no data | | types 2, 12, 14, 16: behind the counter, door niche, Laundry pad, passage walls and ceilings |
| Actor cs cams 0-3 (:338-341; points :308-319) | FIXED1 0x1F (`:178`) | (-27, 40, -38), (13, 40, -38), (-27, 40, -38), (-27, 40, -16) | En_Fsn entries 9-12 chain (`z_en_fsn.c:1450-1455`): shopkeeper, shelf, back, buying |
| Actor cs cams 4 / 5 (:342-343) | FIXED1 | (-65, 41, -118) / (-152, 57, -366) | En_Test4 entries 13 / 14 (each its own additional entry) |
| Actor cs cam 6 (:344) | START2 0x2D (`:192`) | (-119, 82, -478), same as BgCam 1 | no entry of the 18 uses csCam 6 (U-09) |
| Actor cs cams 7 / 8 (:345-346) | FIXED1 | (-18, 42, -94) / (16, 38, -161) | Sakon entries 16 -> 17 |
| Cutscene entry 15 (:293) | script 0 = `CutsceneData_000204` (:123-271) | 568 frames; text 0x296C-0x2975; player cue at (-84, 0, -349); actor cues 506 at (-136, 0, -340) and 201 at (-95, 50, -349) | Kafei csId 15 (face reveal) |
| Entries 0-8 (:278-286) | negative csCamId (global settings) | | consumers NI (U-09) |

## 6. Day 1 noon static assembly list (`prof.d1.noon.normal`, default headers)

**Closed at this profile.** From West the door script returns 0xA at 12:00 (text 0x180A); from Laundry it returns 0xC all of Day 1 (text 0x180C) (5.1, 5.5). An owner capture inside the venue is impossible at this profile; an A1 reference render of the source state is still possible (SOURCE_REGISTER 4.6). State assumptions: fresh save (no week-event flags, `stolenItems` 0), Human form, no mask.

Present by mask and flags if a room were loaded at Day 1 12:00 (C): room_00 a02 (no draw), a03, a04 (no draw), a05, TA0; room_01 a01 (draw NULL, 4.4), a02 (no draw), a05 (no draw), a06, TA1. Only the loaded room's TA spawns (2.4). The shelf is empty on a fresh save at every Night 1 and Night 2 visit (5.2).

| Source key | Component / params | N64 position (Unity) | Yaw | Scale | DLs, bank, offset | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| room_00 shape | 20 cullable entries: 18 opaque + 10 XLU DLs; XML lists 0xCF78 (opaque) and 0x11820 (XLU) | scene frame | - | 1 | room_00 file | segment 8/9 scrolls (1 + 5 DL calls); a frame-0 still is an approximation |
| r00 a05 | wall clock `0x907F` (type 9) | (-121, 75, -156) (-1.21, 0.75, 1.56) | 90 | 0.02 | minute ring 0xCF28, center/hand 0xBEE8, wall face `gWallClockClockFaceDL` 0xF518, sun/moon panel 0xC368 (object_obj_tokeidai, seg 6) | noon pose as `docs/ROM_REFERENCE.md` steps 2-3 (identity ring and face; panel offset (0, -1112, -19.6) x 0.02) |
| r00 a03 | cow statue head `0x0000` | (-113, 48, -103) (-1.13, 0.48, 1.03) | 222 | 0.1 | `gHarikoBodyDL` 0x80, `gHarikoFaceDL` 0x110 (object_hariko) | head rotation identity at rest |
| TA0 | door `0x0284` | (0, 0, 252) (0.00, 0.00, -2.52) | 0 | 0.01 (Actor_Init default) | `gDoorSkel` gameplay_keep 0x22B28; limb 4 = object_wdor01 0x548 | closed pose |
| room_01 shape | 8 entries: 8 opaque + 3 XLU; XML 0x4F80 / 0x54F8 | scene frame | - | 1 | room_01 file | no animated material |
| r01 a06 | small wall clock `0xA07F` (type 10) | (-169, 82, -304) (-1.69, 0.82, 3.04) | 180 | 0.01 | same four DLs | as a05 |
| TA1 | door `0x0285` | (-264, -96, -612) (-2.64, -0.96, 6.12) | 180 | 0.01 | `gDoorSkel` + `gDoorLeftDL` 0x20BB8 / `gDoorRightDL` 0x20D00 | side DL by camera yaw |

**Excluded by half-day mask:** r00 a00 En_Fsn (nights), a01 Sakon (D2n); r01 a00 En_Fsn (D3), a03 and a04 time tags (D2, D3). **Excluded by schedule:** r01 a01 Kafei (draw NULL on Day 1). **No geometry:** r00 a02, a04; r01 a02, a05.

**First profiles in which the venue opens:** no registered Day 1 profile (`prof.d1.night.normal` is 21:00, before 22:00). The first registered profile with the shop open is `prof.d3.final.normal` (Day 3 01:00: En_Fsn present, special item on the shelf, no 05:00 kickout). Proposal for REF-03 / ids.json: `prof.d1.late.normal` = Day 1 23:00 (0xF555), bit 0x040 D1n, steady night light, shop open with an empty shelf, Sakon absent. room_01 opens only with story state: the first registered candidate is `prof.d2.night.normal` (Day 2 21:00) plus `LETTER_TO_KAFEI_SCHEDULED_DAY_2` and `_TODAY` (inside both the door window and Kafei's meeting window); the story-state ID qualifier is a REF-03 decision.

## 7. Collision summary (C from scene.c:384-769; scratch `col.py`, `connect.py`, `walls.py`)

One scene collision header serves both rooms: `Z2_AYASHIISHOPCollisionHeader_001B2C` (scene.c:761-769).

| Measure | Value |
| --- | --- |
| Vertices / polygons / surface types | 174 / 182 / 17 |
| Water boxes | 0 (`0, NULL`, scene.c:768) |
| Camera data | 5 BgCamInfo (5.7) |
| Header bounds | min (-364, -96, -910), max (137, 156, 375) |
| Vertex extents | min (-364, -96, -910), max (132, 156, 375); Unity x -3.64..1.32, y -0.96..1.56, z -3.75..9.10 |
| Polygon kinds | 38 floors (normal Y > 0.5), 126 walls, 18 ceilings (normal Y < -0.8) (`z_bgcheck.c:528-530`) |
| Slopes | 34 flat floors; 2 at 15-20 deg (ramp 148/149); 2 at 20-25 deg (ramp 144/145) |
| Materials | stone 100 polys (types 1-4, 8, 10-12, 14), wood 80 (0, 5, 6, 9, 13, 15, 16), carpet 2 (7) |
| Wall type 1 (`WALL_FLAG_0`) | types 0, 1, 8, 9, 14, 16 = 132 polys (`z_bgcheck.c:18-20`); the player's ledge-climb test reads flag 0 (`z_player.c:11835`), INF: no-climb walls |
| Other flags | floorType 0, floorProperty 0, lightSetting 31, echo 2 on all types; no hookshot, conveyor, soft or horse flags; no xpFlags; no vIB flags |
| Actor collision | no dynapoly: doors are non-framed (`z_en_door.c:338-341`) and Obj_Nozoki skips dynapoly here (`z_obj_nozoki.c:73-76`); En_Fsn back room has a cylinder r18 h64 (`z_en_fsn.c:99-117`) |
| Exits | 2 surface types, 4 floor polygons (2.5) |

### 7.1 room_00 region (INF split at z -276: 70 polys = 14 floors, 52 walls, 4 ceilings)

- Customer floor 44-47 (Y 0, x -132..132, z -48..240) inside irregular walls 0-33 (left x -84..-48, right x 21..72) and an outer box: 36/37 (x 132), 40/41 (x -132), 38/39 (z 240, full width, front -Z), 48/49 (z -48, full width, Y 0..156, front +Z = the counter line; the player cannot pass behind it) (C).
- Counter step 62/63 (Y 12, x -48..12, z -48..-24, wood, bgCam 3) with risers 60/61, 64-67: the En_Fsn talk position.
- Behind the counter (unreachable): floor 42/43 (Y 0, z -252..-48, bgCam 4) and a block 68/69 (Y 16, x -18..18, z -162..-132). The shopkeeper, shelf items, cow head, wall clock, time tag and the peephole eye (spawn 2) are all here.
- Threshold 52/53 (Y 0, x -30..30, z 240..270) with jambs 50/51, 54/55 and lintel ceiling 56/57 (Y 100); West pad 58/59 beyond. Ceiling 34/35 at Y 156 covers only x -84..84, z 0..192.
- No ladders, water, hookshot, conveyor or void-out surfaces.

### 7.2 room_01 region (112 polys = 24 floors, 74 walls, 14 ceilings)

- Upper room at Y 0: floors 102/103 (x -216..-48, z -480..-300, bgCam 1) and 104/105 (x -48..48, z -396..-300, bgCam 2); walls type 8 at x -216, -72, 48 and z -300, -396, -480; ceilings 84-87 at Y 132.
- Peephole alcove: walls 108-113 (x -120..-72, z -300..-288); step 118/119 (Y 30, wood) with risers 116/117, 120-123; Obj_Nozoki at the alcove back (z -288, Y 70); spawn 3 on the step.
- Furniture tops (INF from size): 70/71 (Y 16, carpet, x -216..-168, z -474..-390) with walls 72-79; 130/131 (Y 33, x -120..-72, z -481..-402); 138/139 (Y 16, x -156..-102, z -480..-427); risers 124-129, 132-137.
- Stair passage: ramp 144/145 (Y 0 down to -48 going south, z -396..-504), landing 150/151 (Y -48, x -48..48, z -600..-504), ramp 148/149 (Y -48 down to -96 going west, x -48..-216), lower floor 146/147 (Y -96, x -312..-216); walls type 14 (140-143, 152-165) and ceilings type 16 (166-169, 178-181); headroom 84 to 132 u.
- Door niche 172/173 (Y -96, x -294..-234, z -630..-600, bgCam 4) with walls 170/171, 174/175 and ceiling 176/177 (Y 4). Wall 158/159 at z -600 spans x -312..-216 with its front facing +Z, so the niche and spawn 1 lie behind its back face (U-02, U-03). Laundry pad 114/115 beyond.
- Path 0 Y values are not floor heights (for example (-234, -27, -528) is 69 u above floor 146): INF, Kafei follows XZ under gravity.

## 8. Materials and textures (C; counts and names only, nothing copied; scratch `mats.py`)

| Item | Count |
| --- | --- |
| Extracted PNGs | 33 |
| Scene file (shared by both rooms) | 8 ci8 textures + 2 rgba16 TLUTs (`Z2_AYASHIISHOPTex_001B60` .. `_002CA0`, `TLUT_0030A0`, `_0032A0`); all 10 referenced by both rooms |
| room_00 file | 20 textures (13 ci8, 3 ia16, 2 rgba16, 2 i8) + 1 rgba16 TLUT |
| room_01 file | 2 ci8 textures |
| External `scene_texture_02` | room_00 uses 10 (Tex_000000, 000400, 000600, 000700, 000B00, 000F00, 001300, 001700, 001800, 001C00); room_01 uses 6 of them (000600, 000700, 000B00, 000F00, 001300, 001800); not extracted |
| room_00 Gfx / Vtx / triangles | 49 DLs, 31 Vtx arrays, 618 `gsSP2Triangles` + 17 `gsSP1Triangle` = 1253 source triangles |
| room_01 Gfx / Vtx / triangles | 16 DLs, 9 Vtx arrays, 235 + 1 = 471 source triangles |
| Render modes | room_00: OPA_SURF2 x19, TEX_EDGE2 x8, XLU_DECAL2 x2, XLU_SURF2 x2; room_01: OPA_SURF2 x6, TEX_EDGE2 x3 (all with FOG_SHADE_A) |
| Animated material use | room_00 only: `gsSPDisplayList(0x08000000)` x1 (r00.c:999), `0x09000000` x5 (r00.c:1331, 1357, 1441, 1479, 1514) |
| Room DL bounds | room_00 x -212..176, y -13..162, z -216..270 (2229 vertices); room_01 x -312..51, y -96..144, z -630..-257 (862 vertices) |

## 9. Required extraction list (commands NOT executed)

Done: scene `Z2_AYASHIISHOP` (both rooms). Already extracted: `object_obj_tokeidai`. Binaries exist under `extracted/n64-us/baserom/` (checked by `stat`: object_hariko 1056, object_wdor01 5808, object_fsn 82720, object_test3 63488, object_boj 79808, gameplay_keep 599520, scene_texture_02 7424 bytes; the six object_gi_* banks 2288-6368).

| Priority | Inputs | Why |
| --- | --- | --- |
| A1 static | `object_hariko`, `object_wdor01`, `gameplay_keep` (door skeleton and leaves), `misc/scene_texture_02` | section 6 |
| Night shop (later) | `object_fsn`, `object_gi_mask06`, `object_gi_bombpouch`, `object_gi_bottle`, `object_gi_sword_1` .. `_4` | shopkeeper and state-dependent shelf |
| Story NPCs (later) | `object_test3`, `object_boj` | Kafei, Sakon |

Pattern from `tools/reference/README.md` (run from `local-reference/mm-decomp`, same ZAPD binary and flags as South):

```powershell
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
foreach ($bank in @('object_hariko','object_wdor01','gameplay_keep')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
& $zapd e -i "assets/xml/misc/scene_texture_02.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/misc/scene_texture_02" -osf "extracted/n64-us/assets/misc/scene_texture_02" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
```

`gameplay_keep` and the `misc/` output path have not been extracted with this tool before (NI); check output size and warnings separately. Message text (prices, door and kickout lines) needs a separate text extraction that this checkout lacks (U-01).

## 10. Proposed region record

**Proposal, schema not frozen (PIPE-01).** Same key set as North section 9, written so `tools/reference/region_from_dossier.py` maps it onto region schema 1 without edits; `sourceFrame.sharedFrame` is given directly (`not_shared`). Unregistered IDs (the two peephole portals, `prof.d1.late.normal`, extra cameras) are kept out of the JSON.

```json
{
  "_status": "proposal, schema not frozen (PIPE-01)",
  "schemaVersion": "proposal-0",
  "regionId": "loc.curiosity_shop",
  "displayLabel": "Curiosity Shop",
  "source": {
    "sceneSymbol": "Z2_AYASHIISHOP",
    "sceneEnum": "SCENE_AYASHIISHOP",
    "sceneTableIndex": "0x0D",
    "entranceTableSceneIndex": "0x07",
    "baseline": "N64 US rev 0",
    "decompRevision": "zeldaret/mm 3be2e24123f5e71fd13f939ecc0f9d7f73252639",
    "extractorSha256": "36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd",
    "fast64Revision": null
  },
  "selection": {
    "profile": "prof.d1.noon.normal",
    "sceneHeader": "default (sceneLayer 0; no alternates exist)",
    "rooms": { "loc.curiosity_shop.room_00": "default header", "loc.curiosity_shop.room_01": "default header" },
    "halfDayBit": "0x080",
    "stateAssumptions": ["fresh save: no week-event flags, stolenItems 0", "player form Human, no mask", "venue closed at this profile from both outside doors (texts 0x180A, 0x180C); record serves an A1 reference render only", "first open: prof.d3.final.normal (registered) or proposed prof.d1.late.normal for room_00; room_01 needs the Kafei letter flags (prof.d2.night.normal)"]
  },
  "localSourceInputs": [
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_AYASHIISHOP", "bytes": 13472, "sha256": "70650005834ecba853fc923b94a14275235bfba609edfaad3570135a51639cfc" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_AYASHIISHOP_room_00", "bytes": 72320, "sha256": "499284379a26e35e776a4a1fff16a503c79ed3cebcd2215f6f1ac11cace07ae3" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/baserom/Z2_AYASHIISHOP_room_01", "bytes": 21792, "sha256": "40cb9407143afcf2c4c951a826e074fd30d3bc1b74fb12498b44cc5d52ddf7fc" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_AYASHIISHOP/Z2_AYASHIISHOP.c", "bytes": 41919, "sha256": "e10e8fc86b1dfe716c558dd634a800f0eee373cc1e78f0b9d3bcbad61af019ae" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_AYASHIISHOP/Z2_AYASHIISHOP_room_00.c", "bytes": 105076, "sha256": "1df25587db85efba53dddfa08b488ce144a6094b691cf117c46fced1110ce1fd" },
    { "path": "local-reference/mm-decomp/extracted/n64-us/assets/scenes/Z2_AYASHIISHOP/Z2_AYASHIISHOP_room_01.c", "bytes": 35207, "sha256": "666d293414a20698de452c5717c778b54c5c53335976e86e4d0bb6747a9c5aae" },
    { "path": "local-reference/mm-decomp/assets/xml/scenes/Z2_AYASHIISHOP/Z2_AYASHIISHOP.xml", "bytes": 832, "sha256": "19f9023c8470a99eba35ae63a1a630fbad9c8f0dbf5c7d20b00b1a0049434c17" }
  ],
  "sourceFrame": { "units": "N64 world units", "up": "+Y", "sharedFrame": { "with": [], "status": "not_shared", "evidence": "interior scene with its own frame: TA0 at (0, 0, 252) pairs with West TA5 at (-1510, 45, -12) and TA1 at (-264, -96, -612) with Laundry TA0 at (-2097, -120, -112); joined only by portals" } },
  "conversionToUnity": { "matrix": [[0.01, 0, 0, 0], [0, 0.01, 0, 0], [0, 0, -0.01, 0], [0, 0, 0, 1]], "rule": "Unity (x, y, z) = N64 (x, y, -z) / 100" },
  "authoredRegistrationMatrix": null,
  "provisionalPhysicalScale": true,
  "bounds": {
    "sourceCollisionHeader": { "min": [-364, -96, -910], "max": [137, 156, 375] },
    "sourceCollisionVertices": { "min": [-364, -96, -910], "max": [132, 156, 375] },
    "unityCollisionVertices": { "min": [-3.64, -0.96, -3.75], "max": [1.32, 1.56, 9.10] }
  },
  "anchors": [
    { "id": "anchor.cshop.spawn0_west_door", "source": [1, 0, 227], "unity": [0.01, 0.00, -2.27], "status": "proposed, not surveyed" },
    { "id": "anchor.cshop.counter_talk", "source": [1, 12, -34], "unity": [0.01, 0.12, 0.34], "status": "proposed, not surveyed" },
    { "id": "anchor.cshop.spawn1_laundry_door", "source": [-262, -96, -619], "unity": [-2.62, -0.96, 6.19], "status": "proposed, not surveyed" },
    { "id": "anchor.cshop.peephole_step", "source": [-95, 30, -311], "unity": [-0.95, 0.30, 3.11], "status": "proposed, not surveyed" }
  ],
  "rooms": [
    { "id": "loc.curiosity_shop.room_00", "sourceRoom": "Z2_AYASHIISHOP_room_00", "roomShape": "cullable, 20 entries (18 opaque, 10 XLU)", "displayLists": 49 },
    { "id": "loc.curiosity_shop.room_01", "sourceRoom": "Z2_AYASHIISHOP_room_01", "roomShape": "cullable, 8 entries (8 opaque, 3 XLU)", "displayLists": 16 }
  ],
  "components": [
    { "sourceKey": "Z2_AYASHIISHOP/h0/room_00/a05", "actorId": "0x19C", "params": "0x907F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected, wall clock scale 0.02, noon pose, A1 render only" },
    { "sourceKey": "Z2_AYASHIISHOP/h0/room_00/a03", "actorId": "0x20A", "params": "0x0000", "class": "static-mesh", "dls": ["object_hariko:0x80", "object_hariko:0x110"], "status": "selected, A1 render only" },
    { "sourceKey": "Z2_AYASHIISHOP/h0/ta0", "actorId": "0x005", "params": "0x0284", "class": "door-transition", "dls": ["gameplay_keep:0x22B28", "object_wdor01:0x548"], "status": "selected, closed pose" },
    { "sourceKey": "Z2_AYASHIISHOP/h0/room_01/a06", "actorId": "0x19C", "params": "0xA07F", "class": "static-mesh", "dls": ["object_obj_tokeidai:0xCF28", "object_obj_tokeidai:0xBEE8", "object_obj_tokeidai:0xF518", "object_obj_tokeidai:0xC368"], "status": "selected, small wall clock scale 0.01, A1 render only" },
    { "sourceKey": "Z2_AYASHIISHOP/h0/ta1", "actorId": "0x005", "params": "0x0285", "class": "door-transition", "dls": ["gameplay_keep:0x22B28", "gameplay_keep:0x20BB8/0x20D00"], "status": "selected, closed pose" }
  ],
  "collisionOwnership": { "room": "Z2_AYASHIISHOPCollisionHeader_001B2C (182 polys, shared by room_00 and room_01, no water)", "actorDynapoly": [], "actorCylinders": "room_01 a00 En_Fsn r18 h64 (absent at profile); others NI" },
  "portalIds": [
    "portal.curiosity_shop.west_clock_town",
    "portal.curiosity_shop.west_clock_town.kickout",
    "portal.curiosity_shop.laundry_pool",
    "portal.curiosity_shop.laundry_pool.kickout"
  ],
  "materialBindings": null,
  "cameraIds": ["cam.curiosity_shop.entry", "cam.curiosity_shop.reverse_door", "cam.curiosity_shop.fixture", "cam.curiosity_shop.room_01"],
  "routeIds": [],
  "omissions": ["room_00 a00 En_Fsn and its En_GirlA shelf (night only)", "room_00 a01 Sakon (Night 2 only)", "room_01 a00 En_Fsn back room (Day 3)", "room_01 a01 Kafei (skeletal, schedule and flag gated)", "peephole telescope view (entrance 2) and return (entrance 3)", "Link and Tatl"],
  "excluded": { "halfDayMask": ["room_00/a00", "room_00/a01", "room_01/a00", "room_01/a03", "room_01/a04"], "stateOrSchedule": ["room_01/a01"], "noGeometry": ["room_00/a02", "room_00/a04", "room_01/a02", "room_01/a05"] },
  "unsupported": ["skeletal NPC poses", "door opening animation", "segment 8/9 texture scroll", "telescope camera mode", "time-driven clock pose beyond noon", "message-text prices and lines"],
  "evidence": ["docs/city/interiors/curiosity_shop.md"],
  "readiness": { "sourceInspection": "pass", "objectExtraction": "not_run", "import": "not_run", "controllerTraversal": "not_run", "editorRender": "not_run", "packagedRender": "not_run", "visualApproval": "not_run" }
}
```

Camera intent for the four registered IDs (proposals, REF-03): `entry` = spawn 0 eye looking -Z toward the counter; `reverse_door` = from the counter step looking +Z at TA0; `fixture` = the counter and shelf (source FIXED1 cs cam 0 at (-27, 40, -38), yaw 180); `room_01` = from just inside TA1 (no internal door exists, so SOURCE_REGISTER 4.6's "connecting door" is the Laundry door). Suggested extra ID for the register owner: `cam.curiosity_shop.peephole` (the spawn 2 telescope eye).

## 11. Unknowns, next source questions, checks

| ID | Unknown | Exact next source question |
| --- | --- | --- |
| U-01 | Sale prices (All-Night Mask, Big Bomb Bag, bottle, swords) and every message line cited (0x180A, 0x180C, 0x1889, 0x188D, 0x188E, 0x29xx) | Extract the N64 US message table (a full `assets/text/message_data.h`; the stub here is 492 bytes) and read the two price shorts after the choice command in 0x29DA, 0x29DC, 0x29F3, 0x29F5, 0x29F9 (`z_message.c:2061-2080` layout) |
| U-02 | Whether the player passes a wall from its back face (spawn 1 behind wall 158/159; the TA0 threshold behind 38/39) | Read the static wall test in `src/code/z_bgcheck.c` (`BgCheck_SphVsStaticWall` and its poly-side test) for back-face handling |
| U-03 | Spawn 1 starts on TA1's front side (local +7) inside the niche: confirm the start-mode-0xE walk carries the player into the passage without a door prompt | `func_80835324` and the action set for start mode 0xE in `z_player.c` (around 8536-8690); route test later |
| U-04 | Spawn 3 params `0x0D06`: bgCam 6 with 5 BgCamInfo, and start mode 0xD walking 180 u toward the alcove wall | How `Play_Init` handles `respawnFlag == -2` with `RESPAWN_MODE_DOWN` (`src/code/z_play.c`, `func_80169EFC`) and whether the respawn pose replaces spawn 3's start mode |
| U-05 | Consumers of `OBJECT_TEST3` in room_00 and `OBJECT_BOJ` in room_01 | Search `Actor_Spawn*` of `ACTOR_EN_TEST3` / `ACTOR_EN_SUTTARI` that can run in this scene, and whether Kafei (`room = -1`) survives the peephole reload |
| U-06 | Sakon's behavior in room_00 outside 00:25-01:20 on Night 2 (schedule result 0) | Read `func_80BAC2FC` and the draw gate in `z_en_suttari.c` for result 0 in `SCENE_AYASHIISHOP` |
| U-07 | Which room_00 actors the peephole shows (Day 2 16:10-21:59, Day 3 13:00-21:59) and what `WEEKEVENTREG_64_40` gates beyond the first return line | Apply the 4.2 masks per window; grep `WEEKEVENTREG_64_40` across `src/overlays` |
| U-08 | Cue channels 506 and 201 in `CutsceneData_000204` | grep `CS_CMD_ACTOR_CUE_506` / `_201` users (`Cutscene_GetCueChannel`) in `ovl_En_Test3`, `ovl_En_Fsn` |
| U-09 | Users of actor-cutscene entries 0-8 (global cameras) and of cs cam 6 (START2) | grep callers that index this scene's cutscene list by position (door, item get, literal `CutsceneManager_Queue` ids) |
| U-10 | `RESTRICTIONS_INDOORS` fields; lightSetting 31 under `LIGHT_MODE_SETTINGS` | `z_parameter.c` restriction table; `z_kankyo.c` handling of `lightSetting` 31 |
| U-11 | Shelf item draw pose and scale (GetItem_Draw per GID) for a static Night 3 shelf | `src/code/z_draw.c` `GetItem_Draw` entries for `GID_MASK_ALL_NIGHT`, `GID_BOMB_BAG_30`, `GID_BOTTLE`, `GID_SWORD_*`; En_GirlA draw scale |
| U-12 | Kafei's standing pose and spot during `MEETING_LINK_IN_CURIOSITY_SHOP` | `EnTest3_MoveTowardNextPoint` time path on path 0 (`z_en_test3.c:666-717`) |
| U-13 | Shopkeeper yaw: entry yaw 0 then `BINANG_ROT180` (faces -Z, away from the customer) | Confirm the skeleton's forward axis from object_fsn or a capture before posing |
| U-14 | `seqId` 44 name assumes the table ordinal | Count entries in `sequence_table.h` against the audio build's sequence enumeration |
| U-15 | Visual survey of both rooms | Capture `cam.curiosity_shop.*` at a profile where the venue is open (section 6), with the owner's capture mechanism |

**Not inspected:** room DL contents beyond counts, render modes, texture names and vertex bounds; light-setting values; minimap rendering; En_GirlA draw; En_Fsn cursor and UI code (`:445-685`, `:1528-1711`); Sakon's full schedule actions; Kafei's Ikana and Inn branches; message text; 3DS material; exterior reverse-side triggers beyond the cited dossiers.

**Checks run:** read the brief, HANDOFF, FULL_CITY_PLAN 5/6/11/12, the three registers, the North template, East 2.4, West 2.4.1 and Laundry 2.5, and (after HEAD moved) `shared/city/schema/README.md`, `region.schema.json`, `ids.json` and `tools/reference/region_from_dossier.py`; read `scene.c`, `r00.c`, `r01.c` (headers, lists, spawns, TAs, entrances, exits, paths, cutscene and camera lists, collision), the scene XML, the object, actor, scene and sequence tables, `z_actor.c`, `z_scene.c`, `z_scene_table.c`, `z_scene_proc.c`, `z_schedule.c`, `z_bgcheck.c` (class thresholds, wall flags), `z_inventory.c` (price table), `z_message.c` (price parse), `z_kankyo.c`, `z_sram_NES.c`, `z64scene.h`, `z64save.h`, `z64door.h`, `z64player.h`, `z64camera.h`, `z64bgcheck.h`, `z_player.c` (door, telescope, start modes, ledge test), and the overlays En_Fsn, En_GirlA, En_Door (+ schedule script), En_Time_Tag, En_Suttari (+ schedule), En_Test3 (+ schedule), En_Test4, Obj_Nozoki, Obj_Hariko, Obj_Tokeidai, plus the flag setters in En_An, En_Pst, En_Pm, En_Cha and En_Thiefbird. Scratch parsers run with `python -I` (`col.py`, `connect.py`, `walls.py`, `mats.py`; outputs `*_out.txt` in `C:\Users\cargi\AppData\Local\Temp\claude\9da0c5bc653b5e72\8e1b3883-6660-4971-9dc3-2e2f0d43bcef\scratchpad\ref09\curiosity_shop\`) for the 6/7/2 entry decode, surface decode, class and material counts, exit polygons, door-local coordinates and probes, floor groups and DL bounds; SHA-256 and sizes of the seven inputs; `stat` of 14 bank binaries; section 10 lifted with the repository's `region_from_dossier.py` functions into the scratch folder and checked with `validate_city.py`. Counts agree with LOCATION_REGISTER `loc.curiosity_shop`.

**NOT run:** ZAPD (no object extracted), Blender, Unity, any player or executable, captures, renders, controller routes, message extraction.

**Next dependency:** the register owner adds the two peephole portals and any accepted profile or camera IDs (`prof.d1.late.normal`, `cam.curiosity_shop.peephole`); the integrator lifts section 10 into `shared/city/regions/curiosity_shop.json`; then the section 9 extraction and an A1 room render; ROOM-series venue work waits on the INT-01 commercial pilot.
