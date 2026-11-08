# Clock Town source register (REF-03)

Task **REF-03** from [FULL_CITY_PLAN.md](../FULL_CITY_PLAN.md#12-dependency-ordered-work-queue). Base commit `d0bc29f3ccf38e53ed7f92559cb2d62c3ebd1e1b` on `main`. Written 2026-11-08 by source inspection only: no Unity editor, player, Blender, ZAPD or emulator was run and **no new capture was made**. Every profile and camera below is an index row for later packets, not a claim that a capture exists. Location and room facts belong to [LOCATION_REGISTER.md](LOCATION_REGISTER.md) (REF-01) and endpoints to `PORTAL_REGISTER.md` (REF-02). If those registers disagree with a row here, they win and this file gets corrected.

Profile and camera ID token rules, the record schema and the file layout in sections 3 to 5 are proposals: **schema not frozen, PIPE-01**.

Citation keys used below:

- `mm:` = `local-reference/mm-decomp/`, the ignored local zeldaret/mm checkout at `3be2e24123f5e71fd13f939ecc0f9d7f73252639`. Form: `mm:path:line`.
- `ext:` = `local-reference/mm-decomp/extracted/n64-us/assets/scenes/`, ignored ZAPD output for the 22 city scenes (folder mtimes 2026-11-08 14:04 to 14:05 local). Provenance: the "On 2026-11-08 the same `e` command was run for all 22 Clock Town scenes" paragraph of [tools/reference/README.md](../../tools/reference/README.md), an uncommitted edit when this file was written. Line numbers refer to those generated files.
- Status words: **source** (read at the citation), **derived** (arithmetic on cited constants), **proposed** (project choice), **UNKNOWN** (the next source question is in section 2.8).

## 1. Revisions

These are restated from existing records. Nothing was re-hashed for this table.

| Item | Identity | File that proves it |
| --- | --- | --- |
| Project base | `d0bc29f3ccf38e53ed7f92559cb2d62c3ebd1e1b`, `main` | `git rev-parse HEAD` at REF-03 start |
| Original ROM, compressed (N64 US baseline) | 33,554,432 bytes; MD5 `2a0a8acb61538235bc1094d297fb6556`; SHA-1 `d6133ace5afaa0882cf214cf88daba39e266c078` | [ROM_REFERENCE.md](../ROM_REFERENCE.md) "Verified inputs and tools"; MD5 also `romMd5` in [rom-reference-verification.json](../../evidence/rom-reference-verification.json) and `mm:baseroms/n64-us/checksum-compressed.md5` |
| Decompressed ROM | 49,283,072 bytes; **no hash recorded anywhere** | ROM_REFERENCE.md (gap noted, not filled here) |
| zeldaret/mm | `3be2e24123f5e71fd13f939ecc0f9d7f73252639` | ROM_REFERENCE.md; tools/reference/README.md; `mmSourceCommit` in rom-reference-verification.json; `sourceCommit` in `local-reference/zapd-build-provenance.json`. Local `git rev-parse` agrees. The checkout differs only by the four documented ZAPD patch files and the untracked `tools/ZAPD/native-msvc/` |
| Fast64 | `b13010949567e4a2bf44ba3ccc18b06427243153`, plus a local matrix-pop fix | ROM_REFERENCE.md; README; `fast64Commit` in rom-reference-verification.json; local `git rev-parse` agrees |
| ZAPD native extractor | SHA-256 `36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd`, 1,380,864 bytes, MSVC 19.50.35728.0; libpng.static 1.6.37 and zlib.static 1.2.5 package hashes recorded | README; `exeSha256` and `dependencies` in `local-reference/zapd-build-provenance.json` |
| Blender | 5.2.3 (`A:\SteamLibrary\steamapps\common\Blender\blender.exe`) | ROM_REFERENCE.md; README; `blender` in rom-reference-verification.json |
| Unity | 6000.5.11f1, revision `5c3a1087d8e4` | `Unity/ProjectSettings/ProjectVersion.txt`; README; `unity` in rom-reference-verification.json |
| 22 city scene extractions | ZAPD `e` output, one folder per `Z2_*` scene | README paragraph above; folder mtimes |

## 2. Time model from source

### 2.1 Units and state fields

The clock is `gSaveContext.save.time`, a `u16` that covers one day in 0x10000 units.

| Symbol | Definition or value | Citation | Status |
| --- | --- | --- | --- |
| `CLOCK_TIME(hr, min)` | `(s32)(((hr) * 60 + (min)) * 0x10000 / (24 * 60))`, integer truncation | `mm:include/macros.h:23` | source |
| `CLOCK_TIME_MINUTE` | `CLOCK_TIME(0, 1)` = 45 (0x2D); one exact minute is 45.51 units | `mm:include/macros.h:24` | source, derived |
| `CLOCK_TIME_HOUR` | 2730 (0xAAA) | `mm:include/macros.h:25` | derived |
| `DAY_LENGTH` | `CLOCK_TIME(24, 0)` = 0x10000; the `u16` wraps at midnight | `mm:include/macros.h:26` | source |
| `TIME_TO_MINUTES_F` | 0.021972656 game minutes per unit | `mm:include/macros.h:30` | source |
| Save fields | `cutsceneIndex` s32, `time` u16, `isNight` s32, `timeSpeedOffset` s32, `day` s32 ("totalday"), `playerForm` u8 | `mm:include/z64save.h:333, 334, 336, 337, 338, 340` | source |
| `CURRENT_DAY` / `CURRENT_TIME` | `save.day % 5` / `save.time` | `mm:include/z64save.h:518-519` | source |
| Day start | "The day begins at CLOCK_TIME(6, 0)" (moon-crash countdown offset) | `mm:include/z64save.h:521-524` | source |
| `sceneLayer` | separate save-context field; layout depends on version | `mm:include/z64save.h:366, 414` | source |
| `PLAYER_FORM_*` | FIERCE_DEITY 0, GORON 1, ZORA 2, DEKU 3, HUMAN 4 | `mm:include/z64player.h:70-75` | source |

Reference values (derived): 00:00 = 0x0000; 01:00 = 0x0AAA; 06:00 = 0x4000; 06:30 = 0x4555; 07:00 = 0x4AAA; 12:00 = 0x8000; 17:15 = 0xB800; 17:30 = 0xBAAA; 18:00 = 0xC000; 21:00 = 0xE000; 23:00 = 0xF555.

### 2.2 Clock rate

| Rule | Value | Citation |
| --- | --- | --- |
| Advance per environment update | `time += (u16)R_TIME_SPEED`; then, only when `R_TIME_SPEED != 0`, `time += timeSpeedOffset` | `mm:src/code/z_kankyo.c:1212-1215` |
| When it advances | not stopped, not paused, no game over, no message box except listed text IDs, no frame advance, no transition, no cutscene; called from `Environment_Update` only while pause is off | `mm:src/code/z_kankyo.c:1202-1211, 1767-1779` |
| Source of `R_TIME_SPEED` | header command `SCENE_CMD_TIME_SETTINGS(hour, min, speed)`; speed 0xFF means 0 | `mm:src/code/z_scene.c:387-397, 404-406`; `mm:include/z64scene.h:830-831` |
| Clock Town normal speed | all five exterior default room headers use `TIME_SETTINGS(255, 255, 3)`: no forced time, speed 3 | `ext:Z2_CLOCKTOWER/Z2_CLOCKTOWER_room_00.c:18`; `ext:Z2_BACKTOWN/Z2_BACKTOWN_room_00.c:18`; `ext:Z2_ICHIBA/Z2_ICHIBA_room_00.c:18`; `ext:Z2_TOWN/Z2_TOWN_room_00.c:18`; `ext:Z2_ALLEY/Z2_ALLEY_room_00.c:17` |
| No Ocarina in inventory | a nonzero scene speed is replaced by 5 ("Increase time speed during first cycle") | `mm:src/code/z_scene.c:399-402` |
| Inverted Song of Time | `timeSpeedOffset` toggles between 0 and -2; the HUD treats -2 as slowed | `mm:src/code/z_message.c:5688-5694`; `mm:src/code/z_parameter.c:4663` |
| Sun's Song | `R_TIME_SPEED = 400` until the next 06:00 or 18:00 boundary | `mm:src/code/z_parameter.c:7220-7244` |
| Effective speed helper | `R_TIME_SPEED + timeSpeedOffset` when `R_TIME_SPEED != 0` | `mm:src/code/z_kankyo.c:3404-3412` |
| Update cadence | framerate divisor 3, set at game-state init | `mm:src/code/game.c:40-44, 243` |

Derived units per update: normal 3 (910 updates per game hour, 21,845 per day); inverted 1 (2,730 per hour); no Ocarina 5 (546 per hour). Real seconds per update are **UNKNOWN** (U-01).

### 2.3 Day/night switches and day advance

| Rule | Condition | Citation |
| --- | --- | --- |
| `isNight` while playing and at environment init | t >= 18:00 OR t < 06:00 | `mm:src/code/z_kankyo.c:1229-1233, 582-586` |
| `isNight` at `Play_Init` | t >= 18:00 OR t < **06:30** (a different upper bound) | `mm:src/code/z_play.c:2202-2206` |
| Actor half-day is night | t < 06:00 OR t > 18:00 (strict: 18:00 itself still counts as day) | `mm:src/code/z_actor.c:2494-2496` |
| Day/night actor `En_Test4` | present in all five exterior default actor lists; swaps when (t minus 06:00 or 18:00) changes sign between updates | `ext:` `Z2_CLOCKTOWER_room_00.c:206`, `Z2_BACKTOWN_room_00.c:150`, `Z2_ICHIBA_room_00.c:171`, `Z2_TOWN_room_00.c:265`, `Z2_ALLEY_room_00.c:93`; `mm:src/overlays/actors/ovl_En_Test4/z_en_test4.c:405-408, 429` |
| Actor refresh on half-day change | non-matching actors killed; setup list re-armed; newly matching entries spawned | `mm:src/code/z_actor.c:3311-3330, 2546-2577`; `z_en_test4.c:561` |
| 06:00 on day 1 or 2 | `Sram_IncrementDay` (day++ and eventDayCount++ while CURRENT_DAY <= 3), time = 06:00, directly or through the DayTelop state | `z_en_test4.c:115-119, 448-470`; `mm:src/overlays/gamestates/ovl_daytelop/z_daytelop.c:86, 238-243`; `mm:src/code/z_sram_NES.c:655-659` |
| Pending day flag | `nextDayTime == NEXT_TIME_DAY` at `Play_Init` increments day | `mm:src/code/z_play.c:2230-2234`; `mm:include/z64environment.h:24-28` |
| 06:00 on day 3 | `Interface_StartMoonCrash`: day = 4, time = 06:00 + 10, entrance Termina Field | `z_en_test4.c:439-445`; `mm:src/code/z_parameter.c:5612-5624` |
| New cycle | end of cycle sets day 0 and time 06:00 - 1; `En_Test4` on day 0 before 06:01 starts DayTelop, otherwise sets day 1 | `mm:src/code/z_sram_NES.c:437-440`; `z_en_test4.c:347-361` |

### 2.4 Half-day mask bits

The current bit is `HALFDAYBIT_DAY0_DAWN >> (CURRENT_DAY * 2 + night)` (`mm:src/code/z_actor.c:2491-2498`). An actor entry's mask is `((rot.x & 7) << 7)` OR `(rot.z & 0x7F)`, and mask 0 means every half-day (`mm:src/code/z_actor.c:2562-2565, 3576-3580, 3651-3654`). An actor-list command resets the current bit to 0, so the first spawn takes every matching entry (`mm:src/code/z_scene.c:180-184`). Later half-days spawn only entries that did not match the previous half-day, with an `EVENTINF_17` exception for IDs flagged 0x800 (`mm:src/code/z_actor.c:2566-2571`). The macro names say DAWN, but those bits cover the whole daytime half.

| Half-day | CURRENT_DAY | Time window (from `z_actor.c:2494`) | Macro (`mm:include/z64actor.h:328-341`) | Bit |
| --- | --- | --- | --- | --- |
| Day 0 day / night | 0 | 06:00 <= t <= 18:00 / otherwise | `HALFDAYBIT_DAY0_DAWN` / `_NIGHT` | 0x200 / 0x100 |
| Day 1 day | 1 | 06:00 <= t <= 18:00 | `HALFDAYBIT_DAY1_DAWN` | 0x080 |
| Day 1 night | 1 | t > 18:00, then t < 06:00 until the day advances | `HALFDAYBIT_DAY1_NIGHT` | 0x040 |
| Day 2 day / night | 2 | same as day 1 | `HALFDAYBIT_DAY2_DAWN` / `_NIGHT` | 0x020 / 0x010 |
| Day 3 day / night | 3 | same as day 1; t < 06:00 on day 3 is the final-hours part | `HALFDAYBIT_DAY3_DAWN` / `_NIGHT` | 0x008 / 0x004 |
| Day 4 day / night | 4 | after the moon crash sets day 4 | `HALFDAYBIT_DAY4_DAWN` / `_NIGHT` | 0x002 / 0x001 |

### 2.5 How an entrance selects a header

1. Entrance bits: 0xFE00 = entrance-table scene index, 0x01F0 = spawn, 0x000F = layer, "stored separately in sceneLayer" (`mm:include/z64scene.h:706-711`). Cutscene entrances carry a layer that is applied to `nextCutsceneIndex` (`mm:include/z64scene.h:714-722`).
2. Lookup: `sSceneEntranceTable[e >> 9]`, then spawn array `[(e >> 4) & 0x1F]`, then layer entry `[e & 0xF]`. Each entry is `{sceneId, spawnNum, flags}` (`mm:src/code/z_scene_table.c:2688-2694`; `mm:include/z64scene.h:397-401`). For example, South spawn 0 lists four layer entries (`z_scene_table.c:2344-2349`).
3. `Play_Init` copies `nextCutsceneIndex` into `save.cutsceneIndex` (`mm:src/code/z_play.c:2188-2191`). If `cutsceneIndex >= 0xFFF0` (or the game mode is neither normal nor title), then `sceneLayer = (cutsceneIndex & 0xF) + 1` and the index is cleared. Otherwise `sceneLayer = 0` (`z_play.c:2210-2220`). The scene is spawned from `entrance + sceneLayer` (`z_play.c:2224-2226`). Transitions apply the same rule to `nextCutsceneIndex` (`z_play.c:579-581`).
4. When `sceneLayer != 0`, `Scene_CommandAltHeaderList` runs `altHeaders[sceneLayer - 1]` and ends the base list (`mm:src/code/z_scene.c:464-474`). Room files go through the same interpreter (`mm:src/code/z_room.c:613`; `z_scene.c:580`), so scene and room headers switch together.
5. If a header's `TIME_SETTINGS` hour and minute are not 0xFF, they overwrite the save time on load (`mm:src/code/z_scene.c:388-391`).

So `normal` below means sceneLayer 0, i.e. `cutsceneIndex < 0xFFF0` at load. Exterior header inventory:

| Location | Scene alt headers | Room alt headers | Room `TIME_SETTINGS` per layer (hour, min, speed) |
| --- | --- | --- | --- |
| `loc.south_clock_town` | 3 (`ext:Z2_CLOCKTOWER/Z2_CLOCKTOWER.c:35-39`) | 3 (`Z2_CLOCKTOWER_room_00.c:25-29`) | L0 (255,255,3) `:18`; L1 (2,0,1) `:270`; L2 (255,255,0) `:330`; L3 (6,1,3) `:383` |
| `loc.north_clock_town` | 1 (`ext:Z2_BACKTOWN/Z2_BACKTOWN.c:34-36`) | 1 (`Z2_BACKTOWN_room_00.c:25-27`) | L0 `:18`; L1 (255,255,3) `:204` |
| `loc.west_clock_town` | 1 (`ext:Z2_ICHIBA/Z2_ICHIBA.c:34-36`) | 1 (`Z2_ICHIBA_room_00.c:25-27`) | L0 `:18`; L1 (12,0,0) `:215` |
| `loc.east_clock_town` | 2 (`ext:Z2_TOWN/Z2_TOWN.c:35-38`) | 2 (`Z2_TOWN_room_00.c:25-28`) | L0 `:18`; L1 (21,1,3) `:316`; L2 (17,0,1) `:492` |
| `loc.laundry_pool` | none (`ext:Z2_ALLEY/Z2_ALLEY.c:13-30`) | none | L0 (255,255,3) `:17` |

Only one layer purpose is confirmed from source here: South layer 2 with cutscene script 0 runs the tower-opening setup (`mm:src/overlays/actors/ovl_Obj_Tokeidai/z_obj_tokeidai.c:122-124`). The purposes of the other layers are UNKNOWN (U-02).

### 2.6 Time-driven visual state

| Element | Rule | Citation |
| --- | --- | --- |
| Light row | day 0 or 1: lightConfig 0; day 2: 3; day 3: 4 | `mm:src/code/z_kankyo.c:612-628` |
| Light windows | 08:00-16:00 steady; 16:00-17:00 and 17:00-19:00 blends; 19:00-24:00 and 00:00-04:00 steady night; 04:00-08:00 blends. Steady day/night settings: row 0 = 1/3, row 3 = 13/15, row 4 = 17/19 | `mm:src/code/z_kankyo.c:96-159` (lines 101, 104, 128, 131, 137, 140) |
| Light mode | driven by time when the skybox command's light mode is 0; the South default scene header passes `false` | `mm:include/z64environment.h:48`; `ext:Z2_CLOCKTOWER/Z2_CLOCKTOWER.c:23` |
| Day >= 2 light delta | extra adjustment on day 2 and later; purpose UNKNOWN (U-04) | `mm:src/code/z_kankyo.c:1236-1256` |
| Weather | rain on day 2 from 07:00 until 17:30 while storm state is on; rain cleared from 17:30 to 23:00 | `z_en_test4.c:582-611, 632-643` |
| Storm state | room-behavior bit 12; set to 1 in all five exterior default room headers | `mm:src/code/z_scene.c:246-253`; `mm:include/z64scene.h:804-807`; `ext:` `Z2_CLOCKTOWER_room_00.c:16`, `Z2_BACKTOWN_room_00.c:16`, `Z2_ICHIBA_room_00.c:16`, `Z2_TOWN_room_00.c:16`, `Z2_ALLEY_room_00.c:15` |
| Moon (South LOD moon, type 6) | scale 0.16 x 1.2 / 2.4 / 3.6 / 3.6 on day 0/1/2/3; on day 3, Y drops by rel x 6700 x (scale x 6.25), where rel is the fraction of the day elapsed since 06:00 | `mm:src/overlays/actors/ovl_En_Fall/z_en_fall.c:80-118, 139-159, 270-276` |
| Tower opened | when (CURRENT_DAY == 3 and t < 06:00) or CURRENT_DAY >= 4: gears and clock enter opened idle, the rooftop staircase is drawn, and `WEEKEVENTREG_CLOCK_TOWER_OPENED` is set | `z_obj_tokeidai.c:125-129, 615-621, 689-694` |
| Final hours | `save.day == 3` and t < 06:00; the Final Hours sequence is requested | `mm:src/code/z_kankyo.c:3380-3385, 2544-2553` |
| Bells, screen shrink | bells at 17:30-17:54 and 05:30-05:54; before day 3 the screen scale shrinks from 05:36 to 06:00 (day 3 uses its own bell list) | `z_en_test4.c:274-319, 159-272` |

Derived day-3 moon drop at base scale 0.16: 1,675 units at 12:00, 4,187.5 at 21:00, and about 5,304 at 01:00 (rel 0.7917). Not captured. Moon values in this file are for the South moon actor; the other districts are U-09.

### 2.7 Capture-time rules (derived)

1. Never set a capture at exactly 18:00 (0xC000): the actor half-day says day while `isNight` says night.
2. Avoid 05:30 to 06:30 and 17:30 to 18:00 (bells, screen shrink, the 06:30 `Play_Init` bound and the day advance).
3. Use steady light windows: day 08:00 to 16:00; night 19:00 to 24:00 or 00:00 to 04:00. The dusk review time, 17:15, sits inside the 17:00 to 19:00 blend and before the bells.
4. Record Ocarina possession, `cutsceneIndex`, `sceneLayer` and any forced `TIME_SETTINGS`. Time freezes during cutscenes and most messages (2.2).

### 2.8 Unknowns and next source questions

| ID | UNKNOWN | Exact next source question |
| --- | --- | --- |
| U-01 | Real seconds per update | Which VI mode and refresh rate does the N64 US build set? Grep `osViSetMode`, `osViModeTable` and `osTvType` under `mm:src/boot` and `mm:src/code`, then combine the result with divisor 3 (`game.c:243`). |
| U-02 | Purpose of South L1/L3, North L1, West L1, East L1/L2 | What writes `nextCutsceneIndex` 0xFFF0+n, or a `CS_ENTRANCE` layer, for entrance scenes 0x69 to 0x6D (grep `mm:src/code/z_demo.c`, `mm:src/overlays` and the `ext:` cutscene lists)? Which actors in those scenes test `gSaveContext.sceneLayer`? |
| U-03 | What writes South `cutsceneIndex` 0xFFF1 (layer 2), and which spawn it uses | Grep `0xFFF1` and `CS_ENTRANCE(SOUTH_CLOCK_TOWN` across `mm:src` and the extracted cutscene data. |
| U-04 | Effect of the day >= 2 light delta on the exteriors | Which caller passes `arg1` 4 to 7 to `func_800F6CEC`? Do the `Z2_*` LightSettings arrays (South has 29, `ext:Z2_CLOCKTOWER/Z2_CLOCKTOWER.c:25`) cover settings 12 to 19? |
| U-05 | Initial weather and whether it persists across loads | Where is `gWeatherMode` reset (grep `gWeatherMode =` in `mm:src/code`)? Does `En_Test4` start at `THREEDAY_WEATHER_CLEAR` from a zeroed instance? |
| U-06 | Sky over time for skybox config 10 | Decode `sTimeBasedSkyboxConfigs[10]` (`mm:src/code/z_kankyo.c:171` onward). |
| U-07 | Ocarina possession in the baseline | OWNER/INTEGRATOR choice. Known effect: speed 5 versus 3 (`z_scene.c:399-402`). Next: grep `SLOT_OCARINA` in the overlays of the actors in the five default actor lists. |
| U-08 | Which flags define "no story progress beyond arrival" | Which weekEventReg/eventInf bits do new-file init and `Sram_ClearFlagsAtDawnOfTheFirstDay` (`z_daytelop.c:239-241`) set, and which of them do district actors read? |
| U-09 | Moon actor in North, West, East and Laundry | Grep `ACTOR_EN_FALL` in each `ext:` default room actor list (REF-05 to REF-08). |
| U-10 | Venue access at 12:00 on each day | Door and transition actor params and time predicates for each venue (REF-02, REF-09). |
| U-11 | `timeSpeedOffset == 0x12`, tested in `z_message.c:2661` | Grep `timeSpeedOffset =` across all of `mm:src`, including overlays. |
| U-12 | Which transition cutscenes `En_Test4` plays in each exterior (this selects the day-advance path) | Decode the cutscene ID of each exterior `En_Test4` entry and the scene actor-cutscene list that feeds `sCsIdList` (`z_en_test4.c:322-337`). |

## 3. Source profile register

ID tokens (proposed, PIPE-01): `prof.d<save.day>.<time>.<layer>`. Time tokens: `noon` 12:00 (0x8000), `dusk` 17:15 (0xB800), `night` 21:00 (0xE000), `final` 01:00 (0x0AAA, day 3 only), and `midnight` for a 00:00 cutscene entry. Layer tokens: `normal` = sceneLayer 0; `l<n>` = sceneLayer n. Link form is a field, so a capture in a non-human form needs its own profile ID (proposal: append `.<form>`). None is defined yet.

| Profile | Day | Clock | Half-day bit | Layer / header | Story / cutscene state | Link form | Weather (expected) | Moon, tower, light |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `prof.d1.noon.normal` (baseline) | 1 | 12:00, 0x8000 | 0x080 DAY1_DAWN | 0: default scene and room headers | `cutsceneIndex < 0xFFF0`, no cutscene; tower-opened flag clear; owl statue closed (ROM_REFERENCE choice); other flags U-08; Ocarina U-07 | human (4) | clear | moon 0.384 at home Y; tower closed; light row 0, setting 1 |
| `prof.d1.dusk.normal` | 1 | 17:15, 0xB800 | 0x080 | 0 | same as baseline | human | clear | same as baseline; light row 0, blend 2 to 3 |
| `prof.d1.night.normal` | 1 | 21:00, 0xE000 | 0x040 DAY1_NIGHT | 0 | same as baseline | human | clear | moon 0.384; closed; row 0, setting 3; `isNight` |
| `prof.d2.noon.normal` | 2 | 12:00 | 0x020 | 0 | one 06:00 advance | human | rain (07:00 to 17:30 branch) | moon 0.576 at home Y; closed; row 3, setting 13; South turret day-2 variant (ROM_REFERENCE) |
| `prof.d2.night.normal` | 2 | 21:00 | 0x010 | 0 | same as d2 | human | clear (rain cleared 17:30 to 23:00) | moon 0.576; closed; row 3, setting 15 |
| `prof.d3.noon.normal` | 3 | 12:00 | 0x008 | 0 | two advances | human | clear (no day-3 rain branch found) | moon 0.576, lowered 1,675; closed; row 4, setting 17; South turret day-3 variant; day-3 bell list |
| `prof.d3.night.normal` | 3 | 21:00 | 0x004 | 0 | before midnight | human | clear | moon lowered 4,187.5; closed (t >= 06:00); row 4, setting 19 |
| `prof.d3.final.normal` (tower open) | 3 | 01:00, 0x0AAA | 0x004 | 0 | final hours; loading on layer 0 gives the opened idle state, not the opening cutscene | human | clear | moon lowered about 5,304; tower opened, staircase drawn, flag set at init; row 4, setting 19; Final Hours sequence |
| `prof.d3.midnight.l2` (optional index) | 3 | 00:00 entry; L2 room speed 0 | 0x004 | South sceneLayer 2 (`Z2_CLOCKTOWER_room_00.c:330`) | `cutsceneIndex` 0xFFF1 by formula (writer is U-03); cutscene script 0 | human | clear | tower-opening sequence |

| Profile | Verified | Not verified |
| --- | --- | --- |
| `prof.d1.noon.normal` | Every field is source or derived from section 2. South static geometry (room plus clock, gears, doors, panel and day-1 platform) exists at this state (ROM_REFERENCE.md) | No image anywhere shows the full state: NPCs, moon, skybox, smoke and schedules are missing from every existing render. The four other exteriors and all interiors are unassembled. Open: U-07, U-08 |
| `prof.d1.dusk.normal` | Half-day bit and light blend window | Never rendered from source state. The MVP "Dusk" button applies a Unity lighting preset over noon geometry; it is not this profile |
| `prof.d1.night.normal` | Bit, light row and `isNight` rule | No render. Night rotation of the sun/moon panel is documented for South only (ROM_REFERENCE.md step 3) |
| `prof.d2.noon.normal` | Bit, light row, rain branch and moon scale | Rain never observed (U-05); turret day-2 variant documented for South only |
| `prof.d2.night.normal` | Bit, light row and rain-clearing branch | Clearing is derived, not observed |
| `prof.d3.noon.normal` | Bit, light row and moon formula | Moon drop derived, not observed; day-3 actor sets of the other districts not indexed |
| `prof.d3.night.normal` | Bit and tower-closed rule | Same as d3 noon |
| `prof.d3.final.normal` | Tower-opened rule, final-hours test and sequence request | Opened-tower transforms never assembled; rooftop access not surveyed (REF-02) |
| `prof.d3.midnight.l2` | Layer-2 tower-opening check | The event that writes 0xFFF1, and its spawn: U-03 |

## 4. Capture matrix

These conventions apply to every row:

- Camera ID is `cam.<loc_short>.<name>`. The Location column repeats the full location ID.
- **A/B = `A1/B +O`**. A1 is a same-camera Unity render of the imported local source reference (mechanism option a). B is the authored candidate at the identical stored pose (CITY_PLAYBOOK.md section 6). `+O` is an empty slot for an owner capture (option b), used only if the owner adopts it.
- **Unc = `U-TBD`**. These are the section 3.4 uncertainty fields `pose_uncertainty`, `lens_uncertainty`, `state_uncertainty` and `original_camera_match`, all TBD until the capture is made.
- No transform is recorded here; the pose is recorded with the first capture. The first profile listed is required first.
- A1 needs the region's reference assembly first. South already has one; North needs NCT-01, West WCT-01, East ECT-01, Laundry LP-01, and interiors ROOM-01 to ROOM-04.

### 4.1 South Clock Town (FULL_CITY_PLAN 5.1)

| Camera ID | Location | Intent | Profiles | A/B | Unc |
| --- | --- | --- | --- | --- | --- |
| `cam.south_clock_town.tower_approach` | `loc.south_clock_town` | Tower silhouette, face and day-1 platform from the square approach; G1 benchmark | `prof.d1.noon.normal`, `prof.d1.dusk.normal`, `prof.d1.night.normal`, `prof.d2.noon.normal`, `prof.d3.noon.normal`, `prof.d3.final.normal` | A1/B +O | U-TBD |
| `cam.south_clock_town.reverse_square` | `loc.south_clock_town` | Reverse from the tower base across the square | `prof.d1.noon.normal`, `prof.d1.night.normal` | A1/B +O | U-TBD |
| `cam.south_clock_town.upper_landing` | `loc.south_clock_town` | Upper landing reached by the recorded two-ramp route | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.south_clock_town.west_lower_approach` | `loc.south_clock_town` | Lower West boundary approach (exit 0xD410, polygons 165/166, CITY_PLAYBOOK section 3) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.south_clock_town.east_lower_approach` | `loc.south_clock_town` | Lower East boundary approach (exit 0xD210, polygons 180/181; corrected 2026-11-08 per REF-02) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.south_clock_town.stall_underside` | `loc.south_clock_town` | Stall underside: supports, joints and ground contact | `prof.d1.noon.normal`, `prof.d1.dusk.normal` | A1/B +O | U-TBD |
| `cam.south_clock_town.clock_closeup` | `loc.south_clock_town` | Pose of the ring, center/hand, face and sun/moon panel | `prof.d1.noon.normal`, `prof.d1.night.normal`, `prof.d3.final.normal` | A1/B +O | U-TBD |

### 4.2 North Clock Town (FULL_CITY_PLAN 4 step 2 and 5.2)

| Camera ID | Location | Intent | Profiles | A/B | Unc |
| --- | --- | --- | --- | --- | --- |
| `cam.north_clock_town.arrival` | `loc.north_clock_town` | Arrival view at the South-connected endpoint (REF-02 names it) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.departure` | `loc.north_clock_town` | Facing the South exit from inside North | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.main_silhouette` | `loc.north_clock_town` | Widest view of ground levels, enclosure, central landmark and vegetation | `prof.d1.noon.normal`, `prof.d1.night.normal`, `prof.d2.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.reverse` | `loc.north_clock_town` | Reverse of main_silhouette | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.upper_route` | `loc.north_clock_town` | Highest walkable level (REF-05 selects it) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.lower_route` | `loc.north_clock_town` | Lowest walkable level and perimeter edge | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.closeup` | `loc.north_clock_town` | Important close-up, chosen from source by REF-05 | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.great_fairy_approach` | `loc.north_clock_town` | Approach to the fountain entrance (North exit 0x4600, per REF-01) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.deku_playground_approach` | `loc.north_clock_town` | Approach to the Deku Playground access (North exit 0x3600, per REF-01) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.north_clock_town.foliage_detail` | `loc.north_clock_town` | Tree and grass silhouette and alpha edges at mid distance | `prof.d1.noon.normal`, `prof.d1.dusk.normal` | A1/B +O | U-TBD |

### 4.3 West Clock Town (FULL_CITY_PLAN 5.3)

The frontage grouping follows the 5.3 work brief. REF-02 reassigns any door it places elsewhere.

| Camera ID | Location | Intent | Profiles | A/B | Unc |
| --- | --- | --- | --- | --- | --- |
| `cam.west_clock_town.arrival` | `loc.west_clock_town` | Arrival at the primary South endpoint (REF-02) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.departure` | `loc.west_clock_town` | Facing back to South | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.main_silhouette` | `loc.west_clock_town` | Street rhythm and run of storefronts | `prof.d1.noon.normal`, `prof.d1.night.normal`, `prof.d2.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.reverse` | `loc.west_clock_town` | Reverse of main_silhouette | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.upper_route` | `loc.west_clock_town` | Upper South connection (paired with South exit 0xD420) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.lower_route` | `loc.west_clock_town` | Lower South connection (paired with South exit 0xD410) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.closeup` | `loc.west_clock_town` | Sign, overhang or threshold detail chosen by REF-06 | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.frontage_trading_post` | `loc.west_clock_town` | Trading Post exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.frontage_curiosity_shop` | `loc.west_clock_town` | Curiosity Shop exterior and door | `prof.d1.noon.normal`, `prof.d1.night.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.frontage_bomb_shop` | `loc.west_clock_town` | Bomb Shop exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.frontage_post_office` | `loc.west_clock_town` | Post Office exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.frontage_swordsmans_school` | `loc.west_clock_town` | Swordsman's School exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.frontage_lottery_shop` | `loc.west_clock_town` | Lottery Shop exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.bank_frontage` | `loc.west_clock_town` | Bank frontage at its N64 placement | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.west_clock_town.city_gate` | `loc.west_clock_town` | Out-of-city gate, shown as an honest boundary | `prof.d1.noon.normal` | A1/B +O | U-TBD |

### 4.4 East Clock Town (FULL_CITY_PLAN 5.4)

| Camera ID | Location | Intent | Profiles | A/B | Unc |
| --- | --- | --- | --- | --- | --- |
| `cam.east_clock_town.arrival` | `loc.east_clock_town` | Arrival at the primary South endpoint (REF-02) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.departure` | `loc.east_clock_town` | Facing back to South | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.main_silhouette` | `loc.east_clock_town` | Destination facades and vertical relationships | `prof.d1.noon.normal`, `prof.d1.night.normal`, `prof.d2.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.reverse` | `loc.east_clock_town` | Reverse of main_silhouette | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.upper_route` | `loc.east_clock_town` | Upper South connection (paired with South exit 0xD230) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.lower_route` | `loc.east_clock_town` | Lower South connection (paired with South exit 0xD210) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.closeup` | `loc.east_clock_town` | Doorway depth or balcony underside chosen by REF-07 | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.frontage_stock_pot_inn` | `loc.east_clock_town` | Inn exterior with both street doors (REF-01 4.1) | `prof.d1.noon.normal`, `prof.d1.night.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.frontage_mayors_residence` | `loc.east_clock_town` | Mayor's Residence exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.frontage_milk_bar` | `loc.east_clock_town` | Milk Bar exterior and door | `prof.d1.noon.normal`, `prof.d1.night.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.frontage_honey_darling` | `loc.east_clock_town` | Honey and Darling's exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.frontage_treasure_chest_shop` | `loc.east_clock_town` | Treasure Chest Shop exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.frontage_town_shooting_gallery` | `loc.east_clock_town` | Shooting Gallery exterior and door | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.observatory_route_access` | `loc.east_clock_town` | Access to the underground/observatory route (East exit to `Z2_TENMON_DAI`) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.east_clock_town.elevated_sightline` | `loc.east_clock_town` | Balcony or rooftop sightline that must survive art replacement | `prof.d1.noon.normal` | A1/B +O | U-TBD |

### 4.5 Laundry Pool (FULL_CITY_PLAN 5.5)

| Camera ID | Location | Intent | Profiles | A/B | Unc |
| --- | --- | --- | --- | --- | --- |
| `cam.laundry_pool.arrival` | `loc.laundry_pool` | Entry corridor from South (South exit 0xDA00) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.departure` | `loc.laundry_pool` | Facing back up the corridor | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.main_silhouette` | `loc.laundry_pool` | Basin, banks and enclosing walls | `prof.d1.noon.normal`, `prof.d1.night.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.reverse` | `loc.laundry_pool` | Reverse of main_silhouette | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.upper_route` | `loc.laundry_pool` | Upper walkable level; `not_applicable` if the source has only one level | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.lower_route` | `loc.laundry_pool` | Lowest bank and edge walk | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.closeup` | `loc.laundry_pool` | Bell or door fixture (5.5) | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.water_edge` | `loc.laundry_pool` | Water height against shore and masonry | `prof.d1.noon.normal`, `prof.d1.night.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.bridge_steps` | `loc.laundry_pool` | Bridge or steps if present; otherwise `not_applicable` | `prof.d1.noon.normal` | A1/B +O | U-TBD |
| `cam.laundry_pool.private_door` | `loc.laundry_pool` | Back-room door (Curiosity Shop room_01, per REF-01 4.4) | `prof.d1.noon.normal` | A1/B +O | U-TBD |

### 4.6 Interiors (FULL_CITY_PLAN 6)

The minimum set per venue is: `entry` (from just inside the entrance door), `reverse_door` (back toward that door), `fixture` (the main fixture named in the REF-09 dossier), and one `room_NN` for each declared room other than the entry room. Room counts are the `SCENE_CMD_ROOM_LIST` values in `ext:<SCENE>/<SCENE>.c` (line in brackets). The fountain cameras cover room_00 only, the town room per LOCATION_REGISTER.md 4.2; rooms 01 to 04 belong to other regions. The profile is `prof.d1.noon.normal` unless stated otherwise. Access at that time is U-10: an A1 render does not need access, but an owner capture does. Every row is A/B `A1/B +O` and Unc `U-TBD`.

| Location | Scene, rooms | Camera IDs |
| --- | --- | --- |
| `loc.trading_post` | `Z2_8ITEMSHOP`, 1 (:15) | `cam.trading_post.entry`, `cam.trading_post.reverse_door`, `cam.trading_post.fixture` |
| `loc.bomb_shop` | `Z2_BOMYA`, 1 (:15) | `cam.bomb_shop.entry`, `cam.bomb_shop.reverse_door`, `cam.bomb_shop.fixture` |
| `loc.curiosity_shop` | `Z2_AYASHIISHOP`, 2 (:17) | `cam.curiosity_shop.entry`, `cam.curiosity_shop.reverse_door`, `cam.curiosity_shop.fixture`, `cam.curiosity_shop.room_01` |
| `loc.post_office` | `Z2_POSTHOUSE`, 1 (:15) | `cam.post_office.entry`, `cam.post_office.reverse_door`, `cam.post_office.fixture` |
| `loc.swordsmans_school` | `Z2_DOUJOU`, 1 (:16) | `cam.swordsmans_school.entry`, `cam.swordsmans_school.reverse_door`, `cam.swordsmans_school.fixture` |
| `loc.lottery_shop` | `Z2_TAKARAKUJI`, 1 (:15) | `cam.lottery_shop.entry`, `cam.lottery_shop.reverse_door`, `cam.lottery_shop.fixture` |
| `loc.stock_pot_inn` | `Z2_YADOYA`, 5 (:21) | `cam.stock_pot_inn.entry`, `cam.stock_pot_inn.reverse_door`, `cam.stock_pot_inn.fixture`, `cam.stock_pot_inn.room_01`, `cam.stock_pot_inn.room_02`, `cam.stock_pot_inn.room_03`, `cam.stock_pot_inn.room_04`; add `prof.d1.night.normal` to `entry` |
| `loc.mayors_residence` | `Z2_SONCHONOIE`, 4 (:19) | `cam.mayors_residence.entry`, `cam.mayors_residence.reverse_door`, `cam.mayors_residence.fixture`, `cam.mayors_residence.room_01`, `cam.mayors_residence.room_02`, `cam.mayors_residence.room_03` |
| `loc.milk_bar` | `Z2_MILK_BAR`, 1 (:16) | `cam.milk_bar.entry`, `cam.milk_bar.reverse_door`, `cam.milk_bar.fixture`; add `prof.d1.night.normal` to `fixture` |
| `loc.honey_darling` | `Z2_BOWLING`, 1 (:15) | `cam.honey_darling.entry`, `cam.honey_darling.reverse_door`, `cam.honey_darling.fixture` |
| `loc.treasure_chest_shop` | `Z2_TAKARAYA`, 1 (:15) | `cam.treasure_chest_shop.entry`, `cam.treasure_chest_shop.reverse_door`, `cam.treasure_chest_shop.fixture` |
| `loc.town_shooting_gallery` | `Z2_SYATEKI_MIZU`, 1 (:15) | `cam.town_shooting_gallery.entry`, `cam.town_shooting_gallery.reverse_door`, `cam.town_shooting_gallery.fixture` |
| `loc.great_fairy_fountain_town` | `Z2_YOUSEI_IZUMI`, 5 declared, room_00 used (:20) | `cam.great_fairy_fountain_town.entry`, `cam.great_fairy_fountain_town.reverse_door`, `cam.great_fairy_fountain_town.fixture` |
| `loc.deku_playground` | `Z2_DEKUTES`, 1 (:15) | `cam.deku_playground.entry`, `cam.deku_playground.reverse_door`, `cam.deku_playground.fixture` |
| `loc.astral_observatory` | `Z2_TENMON_DAI`, 2 (:16) | `cam.astral_observatory.entry`, `cam.astral_observatory.reverse_door`, `cam.astral_observatory.fixture`, `cam.astral_observatory.room_01` |
| `loc.clock_tower_interior` | `Z2_INSIDETOWER`, 2 (:16) | `cam.clock_tower_interior.entry`, `cam.clock_tower_interior.reverse_door`, `cam.clock_tower_interior.fixture`, `cam.clock_tower_interior.room_01` |
| `loc.clock_tower_rooftop` | `Z2_OKUJOU`, 1 (:16) | `cam.clock_tower_rooftop.entry`, `cam.clock_tower_rooftop.reverse_door`, `cam.clock_tower_rooftop.fixture`; all on `prof.d3.final.normal` (the staircase is drawn only when the tower is open, `z_obj_tokeidai.c:615-621`) |

Intent per suffix: `entry` shows the room as first seen through the door; `reverse_door` shows the threshold, frame and return clearance; `fixture` shows the counter, stage, target set or other fixture named in the dossier; `room_NN` is that declared room's main view, taken from its connecting door where one exists. If the entry door is not in room_00, rename the cameras per REF-01 so that no `room_NN` duplicates the entry room.

### 4.7 Counts

| Region | Cameras | Profile-camera pairs |
| --- | --- | --- |
| South | 7 | 16 |
| North | 10 | 13 |
| West | 15 | 18 |
| East | 15 | 19 |
| Laundry | 10 | 12 |
| Interiors (17 venues) | 61 | 63 |
| **Total** | **118** | **141** |

## 5. Capture record schema and file naming

The fields come from FULL_CITY_PLAN 3.4 and the CITY_PLAYBOOK section 6 comparison protocol. REF-03 additions are marked "R".

| Field | Format | Origin |
| --- | --- | --- |
| `capture_id` | file stem (see below) | R |
| `camera_id`, `location_id`, `profile_id` | `cam.*`, `loc.*`, `prof.*` | 3.4 scene/profile |
| `scene` | Unity scene path, or source symbol + sceneLayer + room for an owner capture | 3.4 |
| `visible_state` | header, rooms, actor set and named omissions | playbook |
| `mode`, `pair_capture_id` | `A1`, `B`, `O` or `X` (third party); the paired capture | 3.4 A/B mode |
| `transform` | position (x, y, z) in Unity meters in the frame `(x,y,z) = N64 (x,y,-z)/100`; rotation quaternion (x, y, z, w) | 3.4 exact transform |
| `fov_deg`, `fov_axis` | number, `vertical` | 3.4 |
| `aspect`, `resolution_px` | ratio, w x h | 3.4, playbook |
| `near_clip`, `far_clip` | meters | 3.4 |
| `renderer` | Built-in today; the pipeline asset if that changes | 3.4 |
| `light_preset`, `exposure` | preset ID, value | 3.4 |
| `pose_uncertainty`, `lens_uncertainty`, `state_uncertainty`, `original_camera_match` | text or a numeric bound; `unknown` allowed | 3.4 uncertainty |
| `source_revision`, `dirty_fingerprint` | commit; hash of `git status` plus diff | 3.4 results |
| `input_hashes` | reference.json, exports, repaint inputs | 3.4 |
| `tool_versions` | Unity, Blender, Fast64, ZAPD (section 1) | 3.4 |
| `timestamp_utc`, `machine`, `quality_preset` | ISO 8601, host, preset | 3.4 |
| `file`, `sha256`, `bytes` | ignored path, hash, size | R |
| `outcomes` | `source_inspection`, `import`, `controller_traversal`, `editor_render`, `packaged_render` and `visual_approval`, each `not_run`, `pass`, `fail`, `blocked` or `not_applicable`; an omitted field is not a pass | 3.4 |
| `provenance_label` | for example "Unity render of imported reference, not original hardware" | R |
| `notes` | deviations, mismatches, approximated combiner lighting and filtering | playbook |

Naming proposal (schema not frozen, PIPE-01):

- Images (ignored): `local-reference/<region-dir>/captures/<camera_id>__<profile_id>__<mode>__<YYYYMMDDTHHMMSSZ>.png`, each with a sidecar `.json` that holds the record. `<region-dir>` keeps the existing `south-clock-town` folder and uses the same hyphenated style for the others (`north-clock-town`, `west-clock-town`, `east-clock-town`, `laundry-pool`, `stock-pot-inn`, and so on).
- Tracked index: `shared/city/cameras/<loc_short>.json` (for example `south_clock_town.json`). It holds camera definitions, profiles, A/B pairing, and capture references by `capture_id` and `sha256` only. It never holds images, ROM bytes or extracted data.

## 6. Capture mechanism decision (OWNER / INTEGRATOR)

The project has no original-hardware or emulator capture. The only image from outside the project is the ignored third-party `docs/south-clock-town-reference.png` (section 7), and its version, hardware, time and camera are all unknown. Two mechanisms are available:

- **(a)** same-camera Unity renders of the imported local source reference. These approximate lighting, filtering, fog and LOD, and omit NPCs, moon, skybox and smoke (tools/reference/README.md "Known limits").
- **(b)** the owner's own captures from their legally owned copy, with the profile state recorded as in section 5.

Choosing (a), (b) or both is an **OWNER/INTEGRATOR decision**, to be recorded here once made. Until then every A/B cell means A1 versus B, and `+O` stays empty.

## 7. Existing capture index

All files were read on 2026-11-08. The SHA-256 column shows the first 16 hex digits. No file stores a camera transform. By inspection, `mvp-first`, `mvp-day`, `mvp-dusk` and `mvp-reference` share one framing. "Partial d1 noon" means the static day-1 noon closed-tower assembly from ROM_REFERENCE.md, not the full `prof.d1.noon.normal` state.

| File | Pixels, SHA-256, mtime | What it shows | Profile | Label |
| --- | --- | --- | --- | --- |
| `local-reference/south-clock-town/mvp-first.png` | 1600x900, `8b84f26adf9a28c4`, 11:46 | MVP scene: tower and face, thatched stall, crate, covered stall, bunting, facade door at left. Paler and brighter than mvp-day. Not cited in any doc; possibly the over-bright first lighting pass described in ITERATION_LOG (inference) | partial d1 noon; Unity light | Unity render of imported reference / MVP, not original hardware |
| `.../mvp-day.png` | 1600x900, `fd751f7867d1198e`, 11:49 | Same framing: enhanced materials, blue sky with clouds, cast shadows, clock at the noon pose | partial d1 noon; Unity "Daylight" preset | same as above |
| `.../mvp-dusk.png` | 1600x900, `b365ec4c32bb4a9b`, 11:49 | Same framing: purple sky, dim warm light | mixed: noon geometry + Unity "Dusk" preset; **not** `prof.d1.dusk.normal` | same as above |
| `.../mvp-reference.png` | 1600x900, `b29268eb799fe6ba`, 11:49 | Same framing in N64 reference material mode, flat light-blue background | partial d1 noon | same as above |
| `.../mvp-tower-detail.png` | 1600x900, `a3f3f6b48ada0661`, 11:50 | Low close view of the face, side gear, platform rail, lanterns and emblem; doorway at right; purple sky | mixed: noon geometry + a dusk-like preset (inferred from the sky) | same as above |
| `.../mvp-menu.png` | 1280x720, `71cd3f719aa3acf1`, 11:49 | Pause overlay over a dimmed frame: Resume, Return to South entrance, View N64 reference, Daylight [1], Dusk [2] | UI only; not a comparison image | same as above |
| `.../unity-reference-front.png` | 1600x900, `8f1fc929f585924b`, 11:18 | Generated reference scene: low camera between the stalls facing the tower; two large textured planes cross the top of the frame (near-camera geometry, not identified); flat blue background | partial d1 noon | Unity render of imported reference, not original hardware |
| `.../unity-reference-playmode.png` | 1600x900, `9235572d43b5a727`, 11:26 | Generated reference scene in Play Mode: tower, face, stalls, bunting, notice board at right | partial d1 noon | same as above; cited by ROM_REFERENCE.md and the verification json |
| `.../unity-reference-plaza.png` | 1600x900, `9235572d43b5a727`, 11:23 | Byte-identical to unity-reference-playmode.png | same | duplicate; count it once |
| `evidence/south-clock-town-compact.png` | 700x850, `dd743d56fbee9e4a`, 02:17 | Three.js prototype viewer in compact layout: untextured proxy blockout ("17 proxy assemblies - no game data"), Dusk light study | none (proxy, unmeasured) | Three.js prototype render of the unmeasured proxy, no game data; not Unity, not original hardware |
| `evidence/south-clock-town-overview.png` | 1440x960, `3f74cadce35c32bb`, 02:17 | Same viewer in wide layout, with the "Unmeasured blockout" panel; used as the README hero image | none | same as above |
| `evidence/unity-proxy-playmode.png` | 1280x720, `ab7970315dcb13ce`, 10:20 | Older `SouthClockTown_Proxy` scene: pale boxes, cylinder tower, disc clock, default sky | none (proxy) | Unity render of the unmeasured proxy, no game data, not original hardware |
| `evidence/unity-proxy-windows.png` | 1280x720, `ab7970315dcb13ce`, 10:34 | Byte-identical to unity-proxy-playmode.png. Its runtime record is a batch-mode WindowsPlayer (`evidence/unity-windows-runtime.json`), so this file is not an independent player render | none | duplicate; provenance gap |
| `docs/south-clock-town-reference.png` (ignored; outside the two indexed folders) | 1366x768, `46cb24d3fb6964a2`, 01:55 | Third-party N64-style view of South with residents and stalls (SOUTH_CLOCK_TOWN.md:29, ART_DIRECTION.md:13) | unknown | third party; version, hardware, time and camera unverified |

## 8. Checks for this packet

**Run:** `git rev-parse` in the repository and in the decomp and Fast64 checkouts; `git status` of the decomp checkout; reads of every cited source and extracted line; integer arithmetic for the `CLOCK_TIME` values; SHA-256, dimensions and a visual read of the 14 PNGs above.

**Not run:** Unity, any player, Blender, ZAPD, any emulator, any new capture, schema validation, import, build, or any check against original hardware.
