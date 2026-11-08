# Majora's / full Clock Town developer handoff

Updated 2026-11-08 (evening) after REC-01 and the first four reference packets. **Start here.** This is the persistent project handoff requested by the owner. The detailed work specification is [FULL_CITY_PLAN.md](FULL_CITY_PLAN.md); the reusable source/art procedure is [CITY_PLAYBOOK.md](CITY_PLAYBOOK.md). This update plans and documents work; it does not implement additional districts or repeat a Unity run.

## 1. Owner direction and immediate assignment

The owner wants the entire faithful, stylized Clock Town developed, and will assign another developer much of the reference, indexing, asset preparation and repetitive production work. The city and recognizable source geography are the priority. Continue the existing South work; do not replace it with a generic procedural town or restart from the older boxes.

**REC-01, REF-01, REF-02, REF-03 and REF-05 were completed on 2026-11-08** (source inspection only, no Unity run): the checkpoint is committed, all 22 city scenes are extracted to readable C, and `docs/city/` holds the location, portal and source registers plus the North dossier. Later the same day REF-04 (South audit) and REF-06/07/08 (West, East, Laundry dossiers) landed, so all five exterior districts have a source dossier with a proposed region record. The next developer continues the [execution queue](FULL_CITY_PLAN.md#12-dependency-ordered-work-queue) with PIPE-01 (freeze IDs and schemas from the five region records), REC-03 (the South audit found that no Unity artifact has established provenance; rerun the Blender, Unity import and MVP build with hashes recorded) and the REF-09 interior dossiers. Do not begin by extracting every object bank, bulk-generating final art, changing render pipelines, or implementing a second game in the browser.

## 2. Where everything lives

| Item | Current location |
| --- | --- |
| Working repository | `C:\Users\cargi\code\Majoras` |
| This handoff | `C:\Users\cargi\code\Majoras\docs\HANDOFF.md` |
| Detailed full-city execution plan | [FULL_CITY_PLAN.md](FULL_CITY_PLAN.md) |
| City registers (REF-01/02/03) and the five exterior dossiers (REF-04/05/06/07/08) | [docs/city/LOCATION_REGISTER.md](city/LOCATION_REGISTER.md), [PORTAL_REGISTER.md](city/PORTAL_REGISTER.md), [SOURCE_REGISTER.md](city/SOURCE_REGISTER.md), [districts/south_clock_town.md](city/districts/south_clock_town.md), [north](city/districts/north_clock_town.md), [west](city/districts/west_clock_town.md), [east](city/districts/east_clock_town.md), [laundry_pool](city/districts/laundry_pool.md); header-count helper `tools/reference/city_index.py` |
| Extracted readable C for all 22 city scenes | `local-reference/mm-decomp/extracted/n64-us/assets/scenes/<SCENE>/` (ignored; recipe in [tools/reference/README.md](../tools/reference/README.md)) |
| Current preview, controls and verification limits | [MVP_REVIEW.md](MVP_REVIEW.md) |
| Existing master plan | [PROJECT_PLAN.md](../PROJECT_PLAN.md); retains original South/Link milestone IDs |
| Source/import/art lessons | [CITY_PLAYBOOK.md](CITY_PLAYBOOK.md), [ITERATION_LOG.md](ITERATION_LOG.md), [ROM_REFERENCE.md](ROM_REFERENCE.md) |
| Technical contracts | [ENGINE_ARCHITECTURE.md](ENGINE_ARCHITECTURE.md), [ASSET_PIPELINE.md](ASSET_PIPELINE.md), [ART_DIRECTION.md](ART_DIRECTION.md), [ACCEPTANCE.md](ACCEPTANCE.md) |
| Current local scene | `Unity/Assets/LocalReference/SouthClockTown/Enhanced/SouthClockTown_MVP.unity` |
| Preserved local source scene | `Unity/Assets/LocalReference/SouthClockTown/Generated/SouthClockTown_RomReference.unity` |
| Current local executable | `Unity/Builds/CityMVP/MajorasCityMVP.exe`, with its complete adjacent build directory |
| Original proxy | `Unity/Assets/Scenes/SouthClockTown_Proxy.unity`; a different, older smoke-test scene |
| Local reference inputs | `local-reference/` and `Unity/Assets/LocalReference/`, intentionally ignored |

These are local files, not a newly published web page or a pushed handoff. Share the notes and a reviewed source checkpoint with the next developer. A repository URL or `git clone` by itself will not transfer the current uncommitted files, generated scenes, four repaint inputs, ROM, extracted assets or local build.

## 3. Recovered checkpoint: facts and limits

State after REC-01 (2026-11-08): branch `main`, HEAD `d0bc29f3ccf38e53ed7f92559cb2d62c3ebd1e1b` (`Checkpoint South city MVP code, docs and evidence (REC-01)`) followed by the registers commit recorded in [ITERATION_LOG.md](ITERATION_LOG.md). Everything that was dirty on top of `15a46cd` is now committed; nothing is pushed. The table below describes what that checkpoint contains. Refresh `git status` before touching it.

| Area | What survived | What that establishes |
| --- | --- | --- |
| Source import | Existing importer/exporter, ignored source scene and local `.blend` | South reference assembly exists; not final authored city art |
| New preview code | `BuildCityMvp`, `CityArchitecture`, `CityExploration`, `CityPresentation`, `CityPalette`, `CityThreshold`, two new shaders and four verification/launch tools | Source is present; much of it is untracked in Git |
| Preview scene/build inputs | Enhanced scene, source scene, repaint inputs and executable found on disk | Files exist; this handoff did not open or execute them |
| Editor smoke test | `city-mvp-editor.json`, `2026-11-08T17:52:54Z`, `passed: true` | Small flat loop, one wall stop, reset/pause, presentation bindings |
| Editor route test | `city-mvp-routes-editor.json`, `2026-11-08T17:58:02Z`, `passed: true` | Two successive side-ramp segments reach Y=2; lower East boundary stops movement; not two independent west/east route passes |
| Headless player checks | Two `*-windows-headless.json` reports with nested `passed: true` at 17:59 UTC | Recorded movement/hook checks, with runtime-target provenance needing rechecking; no visible render or physical-input acceptance |
| Build | `city-mvp-build.json`: `Succeeded`, zero warnings/errors, report timestamp `2026-11-09T00:01:47Z` | Build report exists; it is later than the headless reports and lacks an artifact-hash link to them |
| Visible player check | `city-mvp-windows.json`: failed, no Pipeline runtime instance | Successful visible Windows launch is not established |
| Other districts | Source XML indices for North, West, East, Laundry | Inventory groundwork only; no additional district import was verified here |

The build's UTC timestamp falls on November 8 in Chicago. More significantly, it is later than the saved player tests. Do not attach those earlier tests to the later executable without an exact artifact identity. The headless reports also record a generic target project path `C:\Users\cargi`; retain the evidence but establish PID, executable path, scene, source revision and package hash in the next run.

The user's machine lockup has **no diagnosed cause** in these notes. Several saved graphics launch logs stop around initialization; the old proxy control launch also stops near graphics-device creation. This is a useful lead for a separate diagnosis, not proof of a driver, GPU, shader or application defect. No editor/player was launched during this handoff.

## 4. Preserve before resuming

1. Record `git status --short`, branch, HEAD and a tracked diff. Separately inventory untracked files; a diff does not contain them.
2. Preserve the uncommitted documentation, code, shader and tool files plus their `.meta` files. Preserve existing `ProjectSettings.asset`, `QualitySettings.asset` and exporter edits. Do not reset, clean, stash destructively, or regenerate scenes over unknown edits.
3. Keep the generated scene, editable `.blend`, repaint inputs, raw logs and local output available as recovery material. They are ignored and will not be recreated by checking out HEAD alone.
4. Review and checkpoint the implementation changes deliberately before giving a second developer a branch. A worktree based only on HEAD excludes current dirty/untracked work. Transfer those changes intentionally or use a reviewed commit; do not assume Git has saved them.
5. Give each developer a separate checkout/worktree and explicit file ownership. Use one Unity Editor per project path. Never let concurrent editors, asset importers or generators write the same `Assets/LocalReference` tree.
6. On another machine, follow [reference setup](../tools/reference/README.md) with that developer's permitted local inputs. Do not send a ROM, extracted asset bundle or reference-derived build through the public repository.

## 5. Read order and governing decisions

Read `AGENTS.md`, this document, the full-city plan, then the specialist document needed for the assigned packet. The new city plan extends the original South-only milestone scope; it does not turn the incomplete South/Link milestone into a completed one.

- N64 US revision 0 is the baseline. The starting reference profile is Day 1 noon, normal room header, closed tower. Label 3DS material and every alternate state.
- Unity `6000.5.11f1` is pinned. The current preview uses Built-in rendering and legacy keyboard/mouse input. URP/new Input System remain separately gated production decisions, not the first recovery task.
- Blender `5.2.3` was used for the local reference import; that successful patched path does not qualify the future authored-character/export pipeline.
- Source Unity mapping is `(x,y,z) = N64 (x,y,-z)/100`; physical meter calibration is provisional. Older proxy coordinates are a different frame. Never reuse proxy placement as measured source placement.
- Original source geometry/materials are local reference. Four repaints and added details do not make the whole district newly authored.
- Current first-person exploration is an environment-review tool. Finished young Link, third-person movement, NPCs, time, saves, quests and connected districts are still future work.
- Preserve shared IDs. Freeze proposed city schemas with the integration owner before producing dozens of records that depend on them.
- Use transitioned districts first. Seamless city stitching needs a separate surveyed alignment and streaming experiment; it is not a prerequisite to useful city exploration.

## 6. Split of responsibility

**Next developer:** reference register; per-room/actor/door inventories; source citations; labeled cameras; portal endpoint survey; material and prop catalog; district-local source assembly after the North pilot; approved kit variations; per-district route/capture evidence. Work through the numbered queue in [FULL_CITY_PLAN.md](FULL_CITY_PLAN.md). Keep unknowns explicit and return small reviewable batches.

**Integration owner:** checkpoint review; schema and coordinate contracts; general importer; renderer/input decision; transition lifecycle; state/time/save architecture; Link integration; global shader/settings changes; cross-district regression and candidate package. A second developer can implement a specifically assigned engineering packet, but does not independently redefine these contracts.

**Owner/art reviewer:** accept the tower/material/character benchmarks and intentional geography deviations; review concrete captures or a playable package when one is ready. Ordinary inventory and reversible preparation can continue while art review is pending.

## 7. Required return from each work packet

Return the task ID, base commit, exact owned files, source citations, produced artifacts, checks actually run, result, known omissions and next dependency. For art, include the same-camera before/after, flat-light or wireframe evidence, contact/clearance checks and editable source/export identity. For a scene, include all room variants selected and omitted, actor omissions, entry/exit pairs and the actual controller route. For systems, include state-before/after and repeated/negative boundary cases.

Do not report `done` from a screenshot, a compiler pass, `success: true` on the tool wrapper, or an old result. Inspect the nested result and its limits. Record implementation, editor run, packaged run, visible review and owner approval separately.

## 8. Suggested skills

Invoke only what the packet needs; inspect each skill before using it.

- `handoff`: keep this entry point and the iteration log useful to the next developer.
- `diagnose`: a bounded investigation of the unresolved visible Windows launch or any reproducible importer defect.
- `unity-cli` / `unity-pipeline`: discover current editor commands and validate the exact Unity project/runtime target before running tests.
- `asset-transformer-toolkit`: evaluate only if its import workflow fits the approved authored asset pipeline.
- `craft-game-floors`: environment polish after source registration, collision and art benchmarks are stable.
- `urp-postprocessing` or `migrate-birp-to-urp`: only for an assigned renderer migration; do not migrate the working preview as routine setup.
- `imagegen`: texture/concept work when requested by an asset packet; generated images are not meshes or rigged characters.

## 9. End-of-session rule

Update [ITERATION_LOG.md](ITERATION_LOG.md) with results and failures, update task state in the city plan, and leave one exact next action here. **Current next action: PIPE-01 (freeze `loc.*`/`portal.*`/`prof.*`/`cam.*` IDs and the region/portal/camera schemas from the three registers and the five district region records, section 10 of each dossier).** REC-03 (provenance rerun of Blender import, `ImportRomReference`, `BuildCityMvp`, smoke and route checks with hashes recorded) is the first task that needs Blender and Unity, so it waits for the owner's go-ahead on this machine. Owner decisions pending: capture mechanism (SOURCE_REGISTER section 6), whether the baseline save holds the Ocarina (clock speed 3 versus 5), whether to push, and whether to run REC-02 (visible player launch) at all on this machine.
