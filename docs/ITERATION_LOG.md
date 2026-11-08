# City iteration log and reusable findings

This is project-local learning requested for the city work. It does not update global memory. Record facts, source findings, assumptions, rejected directions and pending work separately. A source-code change is not a verified render until actual runtime evidence exists for that revision.

## 2026-11-08 — baseline before current exploration/art MVP

### Recorded verification, scoped to the existing run

Evidence: [rom-reference-verification.json](../evidence/rom-reference-verification.json), timestamp `2026-11-08T17:26:26Z`; details in [ROM_REFERENCE.md](ROM_REFERENCE.md) and [reference conversion instructions](../tools/reference/README.md).

| Fact | Evidence / limit |
| --- | --- |
| User-supplied local source is US N64 input with MD5 `2a0a8acb61538235bc1094d297fb6556` | Recorded source identity; original data stays ignored |
| Source/tool revisions pinned | zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639`; Fast64 `b13010949567e4a2bf44ba3ccc18b06427243153` |
| Static source reference imported in Blender 5.2.3 and Unity 6000.5.11f1 | Successful local route with compatibility adjustments; not general Fast64/MM/Blender compatibility certification |
| South scene has 21 visible meshes / 1,501 render triangles and one room collision mesh / 508 triangles | Recorded reference assembly, including clock/gears/counterweight/closed panel/doors/Day 1 platform |
| 158 source materials, 108 exported texture images, 316 Unity texture assets including sampler copies | Different counts measure different things; not an authored modern material library or GPU memory measurement |
| Play Mode advanced 540 frames in final probe | Reported simulation-time delta `0.833419442` seconds is not wall time and cannot be divided into a meaningful gameplay FPS result |
| Three original entrance ground probes passed | Ground raycast evidence only; no character/controller traversal acceptance |
| Final recorded interval had zero compile warnings/errors and zero new console warnings/errors | Applies to the recorded final import/check interval, not all later edits |
| Source reference had a Play Mode capture | Local path in evidence: `local-reference/south-clock-town/unity-reference-playmode.png`; fixed-camera match to original hardware remains unverified |
| This reference had not been packaged into a new Windows build | The older verified proxy Windows build is a different scene/result |

The recorded limitations were: no modern authored replacement art, Link/controls/original simulation, NPCs, moon/skybox/smoke or schedule reconstruction; approximate lighting/filtering/runtime LOD fraction; fixed billboard orientation; animated materials absent; actor collision/traversal, reference-camera match and physical meter calibration unverified. Do not relabel those omissions as complete because the room renders.

### Source findings and earlier corrections worth carrying forward

These entries describe implemented/documented fixes or constraints visible in the current toolchain. Their exact per-defect before/after visual captures are not supplied by the compact verification JSON; the aggregate imported result is recorded above.

| Finding / defect | Existing correction or rule | Reuse lesson |
| --- | --- | --- |
| Fast64's MM selector is disabled upstream even though MM importer branches exist | Local import process exposes the MM enum and registers the add-on | Pin and document the compatibility route; a successful import does not prove full MM support |
| Matrix-pop was unhandled; matrix multiplication could retain incorrect state | `import_clocktown.py` supplies a matrix stack and pop restoration in the local pinned parser | Test nested transforms and state restoration; fail clearly if upstream parser text/revision changes |
| Billboard matrix import repeatedly applied scene scaling | Source matrix parsing uses scale 1; each display-list root applies the unit conversion once | Keep local geometry scale and world-space unit conversion separate |
| MM object-bank/surface enum handling is incomplete in the OOT-oriented importer | Base-header import bypasses enum interpretation; room collision triangles are preserved directly | Preserve uninterpreted metadata; do not invent OOT semantics for MM values |
| Room-only scene assembly omits recognizable clock/platform/door actor pieces | Static Day 1 noon assembly explicitly adds documented separate display lists and actor transforms | Asset indexing must include initialization/draw logic and spawned pieces, not just scene mesh lists |
| Packed actor rotation words include metadata | `ROM_REFERENCE.md` documents `Actor_SpawnEntry` decoding, half-day masks and cutscene bits | Decode first; raw binary-angle interpretation can rotate/place otherwise valid geometry incorrectly |
| One source door actor spawns another leaf, with shifted world pivots | Both closed leaves are assembled at their documented distinct pivots | Scene-listed actor count is not visible component count |
| Platform and tower access differ by day/state | Day 1 pieces and closed panel selected; future/open stairs excluded | Alternate layers/time variants are not additional permanent scenery |
| Blended/cutout/culling and combiner inputs are material state, not just texture names | Exporter/importer retain two textures, combiner cycles, alpha, primitive/environment color, wrap and cull state | One Standard material per PNG cannot reproduce a source reference reliably |
| Blender color data and source combiner numeric space differ | Unity importer converts stored linear colors back to source/gamma numeric values; shader converts final output for linear project color space | Test colors through the whole path; do not compensate with unrelated lighting intensity |
| Two-cycle texture references require slot interpretation, including alpha | Unity importer swaps physical `TEXEL0`/`TEXEL1` sources for the second cycle | Multi-texture material parity needs explicit cycle semantics |
| Primitive LOD fraction is available as source material state | Exporter/importer/shader retain it; runtime `LOD_FRACTION` and other unmodeled inputs remain approximations surfaced by import result | Do not collapse `PRIM_LOD_FRAC` into generic runtime LOD or silently call the reference exact |
| A generated import can overwrite scene context | Import rejects Play Mode/compilation/dirty scenes and updates only its numbered generated asset paths | Preserve user edits and make ownership clear before broader region automation |

Primary project files for these lessons: [import_clocktown.py](../tools/reference/import_clocktown.py), [export_reference.py](../tools/reference/export_reference.py), [ImportRomReference.cs](../Unity/Assets/Editor/ImportRomReference.cs), [RomReference.shader](../Unity/Assets/Shaders/RomReference.shader). Consult source behavior and pinned dependencies when generalizing; this log is not a replacement implementation.

### Current source changes seen during planning — verification pending

At this planning read, the exporter includes UV basis dimensions, `gsSPTexture` scale and per-tile size/shift/origin export; the Unity importer/shader contain corresponding UV reconstruction. Tile origins use truncation compatible with the documented source conversion, including negative values. The working tree also showed an exporter change that skips Fast64 material-library sample objects.

These are source observations. This log does **not** claim current UV/sampler fixes were re-exported, rendered, compared or packaged in this iteration. Root-owned verification must record the exact revision and before/after output. Do not automatically inherit the 17:26 reference pass for changed exporter/shader code.

## Rejected directions and unsupported claims

| Direction / claim | Decision and reason |
| --- | --- |
| Replacing actual Clock Town with a generic procedural primitive village | Rejected production direction. Preserve original geography and distinctive hero shapes; use source assets as local reference before authored replacement |
| Treating the old boxes as modern city art | Rejected interpretation. They are engine/interchange smoke-test volumes |
| Assuming source import equals a modern remake or playable MVP | Rejected claim. Modern art, exploration, character, state fidelity and current-scene build each require evidence |
| Importing every display list / alternate actor layer at once | Rejected assembly method. Includes unused, other-scene, wrong-day or dynamically hidden components |
| Dividing actor positions by actor-local scale or repeatedly converting units | Rejected transform method. It breaks spacing and component contact even when individual meshes look plausible |
| Assigning OOT meanings to unsupported MM surface/object values | Rejected semantic shortcut. Retain source metadata and implement selected meanings deliberately |
| Raw skeletal limb display lists piled at an origin | Rejected character/effect shortcut. Correct hierarchy, rest transforms and runtime behavior must be reconstructed separately |
| Using more bloom/fog or a giant generic moon to hide weak geometry | Rejected art shortcut. Identity, routes, structure and same-camera A/B must pass first |
| Claiming an older proxy package verifies the new imported/modernized scene | Rejected evidence reuse. Package and run the current scene |
| Reporting frame count/simulation-time delta as performance FPS | Rejected measurement. Profile wall-clock frame times on named hardware and preset |

These are project decisions and ruled-out methods, not claims that every listed method was executed and failed in a tool run.

## MVP plan recorded before the editor verification pass

The user's direction is to keep iterating the MVP, make the city the priority and retain lessons for the remaining districts. This table preserves the earlier work plan. The dated outcome below supersedes blanket “pending” descriptions only for the checks it actually records; broader production gates remain open.

| ID | Intended work | Required evidence / pending limit |
| --- | --- | --- |
| MVP-01 | Correct remaining source UV/tile/material issues and exclude import-library samples | Re-export/import from pinned source, actual same-camera before/after, console summary; no silent carry-forward of old pass |
| MVP-02 | Establish the six reusable authored material families | Stone/plaster/wood/cloth/metal/ground sample cluster, proper channel settings and source-backed use; distinguish source material counts |
| MVP-03 | Refine visible hero architecture and route-facing kit | Tower/perimeter/ground/platform shape improvement in wireframe/flat/intended light; preserve anchors/routes |
| MVP-04 | Add bounded Unity exploration with safe spawn and camera | Actual walk/turn/orbit/recenter/pause/collision route clip; free fly alone is insufficient |
| MVP-05 | Keep source and candidate available for fixed-view comparison | Stored camera pose/lens/state, visible A/B or side-by-side, documented structural gains and deviations |
| MVP-06 | Verify current-scene runtime and package | Fresh compile/console, Play Mode evidence, Windows build/run and repeated core route; performance separately scoped |
| MVP-07 | Use the South pass to prepare other city districts | Region index first, actual source geometry next; no North/East/West extraction in this notes task |

MVP finish report must distinguish: source reference correction, authored visual improvement, exploration behavior, current-scene build/run, performance and user visual acceptance. Finished modern young Link remains a separate character deliverable until an actual mesh/rig/animation is imported and reviewed.

## 2026-11-08 — editor exploration/presentation MVP observed

Evidence: [city-mvp-editor.json](../evidence/city-mvp-editor.json), timestamp `2026-11-08T17:52:54.0577694Z`; recorded captures and bounded corrections below. The result is **partial MVP progress with targeted editor checks passed**. Windows build/run, physical input and the ramp/stair route are pending at this update; a later entry will record their final outcomes separately.

### Implemented candidate and source boundaries

- [BuildCityMvp.cs](../Unity/Assets/Editor/BuildCityMvp.cs) generates ignored `Assets/LocalReference/SouthClockTown/Enhanced/SouthClockTown_MVP.unity` separately from the source-reference scene. The original assembly is preserved from commit `15a46cd`.
- Four AI-assisted repaints: `stone_bricks.png` → source `texture_052.png`/`texture_097.png`; `plaza_pavers.png` → `041`; `wall_relief.png` → `057`; `golden_pavers.png` → `037`. They affect 33 slots; texture+sampler-wrap deduplication produces 149 enhanced texture assets with mipmaps/trilinear/aniso 8.
- Original UV basis/tile dimensions are retained despite 1254 px replacement artwork. Replacement image size must not be used to recompute original texture repetition. Color-derived relief samples neighboring replacement pixels separately.
- Built-in Standard lighting/shadows augment the original combiner's UV/color logic; a separate gradient/cloud/sun sky supports day/dusk profiles. Transparent source materials retain their reference renderer pending specific replacement.
- Ten authored detail renderers add approximately 8,000 triangles around the original 21 renderers/158 material records. This is a small hero-detail treatment, not a fully remodeled town.
- First-person walking uses a 1.1 m eye height. No Link mesh, rig or third-person character exists in this MVP.
- Eight preview boundaries map to source exit surfaces: North, Termina, two East approaches, two West approaches, Laundry and tower interior. Their source polygon/entrance mapping is in [CITY_PLAYBOOK.md](CITY_PLAYBOOK.md#eight-source-derived-day-1-preview-boundaries). Source mapping does not establish successful traversal of every boundary or a loaded neighboring district.
- Material production is partial: stone/ground repaints and plain wood/metal architectural materials are present; plaster/cloth remain source treatment. The six-family library is still a work plan.

### Editor checks actually recorded

| Check | Observed result | Scope limit |
| --- | --- | --- |
| Settled spawn | `(-5.7, 0.025, -4.5)` supported by `Source_Collision` | One known source-ground start |
| Flat-ground route | Forward 2.04 m, right 0.60 m, left 0.60 m, backward 2.04 m: **5.28 m total**, 88/88 steps grounded | Small flat loop only; no ramps/stairs/platform |
| Route return | 0 m position error | Public deterministic input hook, not physical device handling |
| Source-wall stop | 3.6 m commanded, 1.275 m actual; 39 side contacts; last ten steps 0 m; capsule clearance about 0.025 m | One wall surface only |
| Pause/reset | Pause displacement 0 m; reset error 0 m and source support restored | Targeted state hooks, not every menu/control flow |
| A/B | 21 material bindings, reference/enhanced selection and authored visibility verified | Actual visual comparison still requires captures/review |
| Lighting | Day/dusk presets differ | A preset toggle is not a three-day game clock |
| Frame observation | Frame 2235→2542; simulation time 0→0.5052153 during a 500 ms observation | Advancement evidence; not a performance benchmark |

The test retained its diagnostic distance and respawn increments. Do not interpret those counters as a fresh user session or extra traversal coverage.

### Corrections made during this pass

| Observed defect / rejected attempt | Correction | Reusable lesson |
| --- | --- | --- |
| CityEnhanced failed compilation with duplicate `_Cutoff` in generated surface passes | Removed the manual uniform; retained the property and `alphatest:_Cutoff` directive | Surface shader generation can add uniforms; inspect generated-pass errors before adding declarations by analogy |
| Initial sun intensity 1.15 plus ambient `#C1CCC4` made the enhanced scene overly bright | Day sun 0.78 / ambient `#7A8E99`; dusk sun 0.58 / ambient `#666B87` | Tune illumination against source/candidate captures; extra geometry/texture detail should remain readable without bleaching the palette |
| Inner clock bead floated about 21 cm because the annulus surface is conical | Corrected its sampled contact depth to Z 8.115–8.19 across the ring | Fit trim to actual source surface geometry, not one assumed flat clock plane |
| Brass trim left a visible gap | Added about 1 cm of intentional embedding at the contact | Small controlled overlap can secure contact; record it rather than hide a large unsupported gap with shading |

These are specific observed iteration outcomes reported during this pass, not hypothetical failure modes. The source reference remains available for comparison; the added architectural details remain visible only in the enhanced candidate.

### Captures and qualified visual review

Local images: `local-reference/south-clock-town/mvp-day.png`, `mvp-dusk.png`, `mvp-reference.png`, `mvp-tower-detail.png`, `mvp-menu.png`. Keep them with the exact scene/material revision and camera state. Current A/B swaps material/presentation state and enhanced-only geometry without requiring a different viewpoint; full hardware-reference camera calibration remains unverified.

Working review scores: layout **4/5**, palette **4/5**, art coherence **3/5**, detail **3/5**, presentation **3/5**, character **N/A**. These are subjective iteration assessments from the current review, not user approval, numerical fidelity measurements or an AAA claim. The next visible pass should target architectural/material coherence and detail rather than replace the already recognizable geography.

### Next bounded work

1. Complete the current-scene Windows build/run and physical input check; record results independently from the editor-hook pass.
2. Verify ramp/stair/platform movement with phase-correct collision; then test all configured boundary approaches.
3. Extend authored family samples beyond stone/ground and plain structural materials; retain source-specific cloth/plaster appearance until actually replaced.
4. Improve hero detail coherence with fixed-camera before/after and moving-camera contact checks; preserve source routes and geometry registration.
5. Continue the source-indexed North/West/East city phases only after South's reusable pipeline issues are understood.

Technical references used for this pass: Unity [CharacterController.Move](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/CharacterController.Move.html), which requires supplied motion/gravity handling, and [Built-in surface shaders](https://docs.unity3d.com/6000.0/Documentation/Manual/SL-SurfaceShaders.html), which generate lighting/shadow passes from surface declarations. These document implementation mechanics; project behavior is supported by the evidence run above.

## 2026-11-08 — remaining-city index inspection

Inspected the pinned local XML files only. Exact scene/room identifiers and discovery display-list offsets are recorded in [CITY_PLAYBOOK.md](CITY_PLAYBOOK.md#7-next-districts-exact-local-xml-index-future-work-only): North `Z2_BACKTOWN`, West `Z2_ICHIBA`, East `Z2_TOWN`, Laundry `Z2_ALLEY`. Each has one declared `room_00`. Their XMLs identify extraction metadata, not actor transforms, schedules or finished meshes.

Proposed priority: finish South's pipeline/exploration/A-B benchmark; index and reconstruct North; test commercial kits and N64 bank geography in West; rebuild destination-specific fronts in East; then integrate Laundry/portal boundaries. The priority is an implementation proposal, not source-game progression. No other district was extracted, imported or validated by this documentation pass.

## Append format for the next verified pass

Use one short entry per bounded iteration:

```text
Date/time and iteration ID:
Goal / observed defect:
Before artifact + camera/state:
Hypothesis (clearly labeled):
Change / exact files and revision:
Executed checks and observed output:
After artifact at same camera/state:
Result: pass / fail / partial / blocked / not run
What we learned and can reuse:
Remaining limitations / next bounded correction:
```

Keep failed or rejected attempts when they explain a reusable lesson. Avoid recording planned code as execution, a rendered frame as traversal, or “looks better” without a comparison artifact.

## 2026-11-08 — lockup recovery and full-city developer handoff

**Request:** after the computer locked up, continue the detailed full-city plan in the handoff notes so another developer can perform the reference and production legwork. This pass read surviving files and updated documentation. It did not restart Unity, launch a player, regenerate scenes, modify implementation or diagnose the lockup.

### Recovered source and evidence

- Checkout read as `main` at `15a46cd8cbd05395715b4042d4115f8a22667415`, with newer dirty/untracked city implementation, project settings, exporter edits, documentation and evidence. Preserve that state; the HEAD commit alone does not contain the complete current preview.
- The enhanced scene, executable, source `.blend`, repaint inputs and prior captures survive locally. Local source/generated inputs remain ignored and require a separate recovery/setup inventory.
- `city-mvp-routes-editor.json` records a passing 428-step ascent through **two successive side-ramp segments** to the Y=2 upper level, plus one lower East boundary stop/proximity check. It does not cover independent East/West ramp paths, every exit, the closed Day 1 tower stairs or roof.
- `city-mvp-windows-headless.json` and `city-mvp-routes-windows-headless.json` contain nested passing results at approximately 17:59 UTC. They establish recorded hook-driven checks, with target identity needing revalidation; their tool envelopes identify a generic `C:\Users\cargi` project path.
- `city-mvp-build.json` records a successful Windows build ending `2026-11-09T00:01:47Z`, with zero warnings/errors. This is later than the saved player checks; no artifact hash ties those earlier passes to that later build.
- `city-mvp-windows.json` records a failed visible-player connection: no Pipeline runtime instance at the build directory. Physical input and visible current-build graphics remain unverified.
- Saved graphics launch logs stop around initialization; the older proxy comparison also stops near graphics-device creation. The cause of the user's lockup is **unknown**. Preserve those logs for a separate bounded diagnosis; do not infer a driver or shader cause here.

### Documentation delivered

| Artifact | Purpose |
| --- | --- |
| [HANDOFF.md](HANDOFF.md) | Persistent entry point, recovered state, preservation steps, file map, developer ownership, suggested skills and next action |
| [FULL_CITY_PLAN.md](FULL_CITY_PLAN.md) | City A/B/C scope; five exterior production briefs; 17 venue/annex packets; proposed schemas; portal lifecycle; art, character, time, NPC, service, quest, save and QA work; dependency queue and first assignments |
| [MVP_REVIEW.md](MVP_REVIEW.md) | Restored previously missing README target, controls, exact saved evidence and unresolved visible-launch limits |
| Existing README/master/playbook/Unity notes | Linked to the new handoff and corrected stale recovery priorities and broad ramp/build descriptions |

Source inspection also confirmed the five exterior scene/room mappings and read the pinned scene-table names for the venue inventory. XMLs show multiple room files for the Inn (five), shared Fairy scene (five), Observatory (two) and Curiosity Shop (two). Those counts are discovery metadata; the correct town/header/door selection still requires the assigned dossiers. No other district/interior extraction or Unity assembly occurred in this pass.

### Decisions and next work

The full-city plan extends the earlier South-only scope without declaring the South/Link milestone complete. Complete the connected exteriors first, then all surveyed venues/annexes, while shared-state/character pilots proceed on their own dependencies. Preserve Built-in rendering for recovery and the North source pilot; qualify the production renderer/material contract separately before bulk art.

**Next:** REC-01 checkpoint preservation, then REF-01/02/03 and REF-05 for the next developer: full location/room/portal register, capture plan and North pilot dossier. Investigate visible launch as REC-02; source/index work can continue independently. No commit, push or external publication was performed by this notes update.

Documentation verification: all local Markdown file links in the eight touched documents resolved; code fences were balanced; no UTF-8 replacement characters were found; the new queue contained 61 unique task IDs; `git diff --check` passed. SHA-256 comparison against the start-of-pass snapshot found no changes to 61 existing implementation, tooling and settings files. No application tests were rerun because this pass changed documentation only.

## 2026-11-08 (evening) - REC-01 checkpoint, 22-scene extraction, REF-01/02/03/05

**Request:** the owner confirmed the fan project and asked to work the full-city plan, with Opus agents doing the legwork. The integrator did REC-01 and the extraction; four parallel agents each owned one new file under `docs/city/`. Source inspection only: no Unity, player, Blender or capture.

| Packet | Result | Evidence |
| --- | --- | --- |
| REC-01 | Done. All 38 dirty/untracked paths from the lockup checkpoint committed as `d0bc29f`; no ROM, extracted asset, generated scene or build included; not pushed | `git log -1 d0bc29f`, `git status` clean |
| Extraction | 21 remaining city scenes extracted with the recorded native ZAPD, 0 failures; all 22 now have readable scene/room C under ignored `extracted/n64-us/assets/scenes/` | `tools/reference/README.md` extraction note |
| REF-01 | Done. 22 scenes, 36 room files, 16 alternate scene headers, 60 room headers, 530 default-header actor entries, 88 spawns, 39 transition actors. Fountain scene: town room is room_00. Mayor's Residence has 4 rooms, tower interior 2, rooftop has no exit list. South default exit list has 10 values, not 8. 14 unknowns (U-01 to U-14) | [LOCATION_REGISTER.md](city/LOCATION_REGISTER.md); helper `tools/reference/city_index.py` |
| REF-02 | Done at source level. 95 directed portals (20 district, 29 door, 18 special, 10 external, 18 room); 25 mutual exit pairs plus 9 room pairs; all five exteriors reachable from South on foot; `EN_DOOR` params carry no exit index (the polygon behind the door does); 28 venue doors cross-confirmed by schedule type. **Correction:** the playbook's polygon column was transposed in 4 of 8 South rows (North 18/19, East lower 180/181, West upper 184/185, tower interior 8/9); exit values, entrance indices and the preview bounds were already correct. 14 unknowns | [PORTAL_REGISTER.md](city/PORTAL_REGISTER.md); corrected table in [CITY_PLAYBOOK.md](CITY_PLAYBOOK.md#eight-source-derived-day-1-preview-boundaries) |
| REF-03 | Done. Time model from source: `CLOCK_TIME`, speed 3 normal / 5 without Ocarina / 1 inverted, night at 18:00 and 06:00 (06:30 at `Play_Init`), half-day bits, `sceneLayer` from cutscene index. 9 profiles, 118 cameras, 14 existing PNGs indexed. Findings: `evidence/unity-proxy-windows.png` is byte-identical to the Play Mode capture; `mvp-dusk.png` is noon geometry under a Unity preset, not a source dusk state. Capture mechanism left as an owner decision | [SOURCE_REGISTER.md](city/SOURCE_REGISTER.md) |
| REF-05 | Done. North: 33 default / 32 alternate actors, 30 Day 1 daytime candidates, 19 object banks, 8 spawns, 8 exits, 312 collision polygons. Great Fairy access is collision exit surface type 4 (polys 118/119); Deku Playground is the `Door_Ana` grotto with params `0x0304` (exit 4). North lists the tower clock actors at the South coordinates, so a shared world frame is inferred (needs a collision overlay). Day 1 noon static list of 14 components written; object-bank extraction commands written, not run. 17 unknowns | [districts/north_clock_town.md](city/districts/north_clock_town.md) |

**Integrator checks run:** recomputed South exit polygons per exit index from `Z2_CLOCKTOWERCollisionHeader_005560` (matches REF-02 exactly; the rooftop exit covers 22 tower-top polygons and `0xD650` has none); read the cited lines for every REF-03 time constant; checked North's exit list, actor counts and grotto params against the C; checked ID consistency across the four files; relative-link check on the touched documents. **Not run:** Unity editor or player, Blender, any capture, any push, REC-02.

**Status:** G0 partially closed (checkpoint and inventory done; REC-02/REC-03 build identity still open). **Next:** PIPE-01 schema and ID freeze from the registers; REF-04 and REF-06/07/08 can run in parallel from the extracted C using the North dossier as the template; REF-09 interior dossiers after that. Owner decisions: capture mechanism, Ocarina in the baseline save, push, and whether REC-02 runs on this machine.

## 2026-11-08 (night) - REF-04, REF-06, REF-07, REF-08 dossiers

**Request:** same owner request as the evening entry. Four Opus agents each owned one new file under `docs/city/districts/`; the previous Claude Code process exited mid-run and all four were resumed by agent id. Source inspection only: no ZAPD, Blender, Unity, player, capture or push.

| Packet | Result | Evidence |
| --- | --- | --- |
| REF-04 South | Done. 50/12/9/29 actors per header (37 Day 1 daytime candidates in the default header), 1 transition actor (`EN_DOOR` at (1114,100,274) over floor poly 120, east of the East-lower trigger), 508 polys / 416 vertices / 21 surface types, 2 water boxes at Y -20 over Y -40 floors. 8 of 50 default actors are imported; 4 are covered by no doc. Rooftop exit = 22 polygons at Y 300-375; the three lower treads carry no exit. Actor collision gap: panel, door leaves, Day 1 platform, flower and chest have dynapoly in source; the preview gives colliders only to the door leaves. **Changed-input verdict:** the recorded Unity reference scene and `rom-reference-verification.json` (11:26) were built from an earlier, unrecorded exporter; `reference.json` (11:35) was re-exported by the committed scripts but the Unity scene was never regenerated; the Enhanced scene mixes both generations. Provenance is not established for any Unity artifact. 20 unknowns | [districts/south_clock_town.md](city/districts/south_clock_town.md) |
| REF-06 West | Done. 40 default / 22 alternate actors, 6 transition actors mapped to venues by schedule type, sign spots paired to doors. **Bank placement (plan 5.3):** the N64 bank is not a scene; it is the `En_Ginko_Man` counter actor at (-1559,133,-438), yaw 316, `OBJECT_BOJ` skeleton, present at Day 1 noon; no 3DS placement used. 19 unknowns | [districts/west_clock_town.md](city/districts/west_clock_town.md) |
| REF-07 East | Done. 7 transition actors mapped to venues; upper/lower levels and the Inn deck classified. **Ruling on REF-02 U3:** polygon 169 is one wall face of a six-face post (polys 164-169) under the Inn upper-deck walkway; `Player_HandleExitsAndVoids` only receives floor polygons (`z_player.c:12572` standing floor, `:6783` door probe), so its exit index is dead data and the observatory portal is the floor pair 116/117 only. 21 unknowns | [districts/east_clock_town.md](city/districts/east_clock_town.md) |
| REF-08 Laundry | Done. One water box `{-2400,-135,-200,600,1320}` (surface Y -135); no surface type carries a swim or dive flag, so all water behavior is the box. The scrolling 65 %-opaque surface sits exactly at the box height. The back-room door is `EN_DOOR` `0x0285` = `ENDOOR_SCH_TYPE_LAUNDRY_POOL`, both sides room 0, exit polys 40/41 behind it, black 2-triangle backdrop. 21 unknowns | [districts/laundry_pool.md](city/districts/laundry_pool.md) |

**Integrator checks run:** South spawns (11 positions and yaws), transition actor, exit list, water boxes, collision header bounds, 416/508/21 counts, poly 120 plane and vertices, and the per-header actor counts all read back from `Z2_CLOCKTOWER.c`/`_room_00.c`; the audit's git claims rechecked (`import_clocktown.py` sha identical at 15a46cd, d0bc29f, 98e554b; `export_reference.py` differs by exactly the two `sourceTexture`/`texture-sources.json` lines). West, East and Laundry exit lists, transition actors with params, actor counts, water box and polygon 169 normal read back from their scene C. The polygon-169 ruling was re-verified at `z_player.c:6405-6416`, `:6783` and `:12572` before editing PORTAL_REGISTER. **Corrections applied:** PORTAL_REGISTER `.p169` row relabeled as a non-portal data oddity, U3 resolved, "missing reciprocal" list reduced to the rooftop; SOURCE_REGISTER `cam.south_clock_town.east_lower_approach` polygons 184/185 -> 180/181 (REF-04 U-20). **Not run:** Unity, Blender, any capture, any push, REC-02, REC-03.

**Status:** REF-01 to REF-08 done; every exterior district has a source dossier and a proposed region record (section 10 of each). **Next:** PIPE-01 schema and ID freeze from those five records; REC-03 provenance rerun (needs Blender and Unity, owner go-ahead); REF-09 interiors. Owner decisions: capture mechanism, Ocarina in the baseline save, push, REC-02/REC-03 on this machine.
