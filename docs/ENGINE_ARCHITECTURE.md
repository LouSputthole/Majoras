# Engine architecture and implementation decisions

Status: proposed architecture, grounded in the local bootstrap evidence and primary documentation checked on 2026-11-08. This document is a build plan; it does not claim a playable Unity project, a working source-game bridge, or gameplay parity already exists.

## 1. Recommended direction

Build the game in Unity. Use the existing Three.js/Vite folder only as a fast, browser-accessible asset and composition review tool. Both consumers load the same exported art, but Unity owns movement, collision, interaction, NPC schedules, dialogue, inventory, time, saving, and resets. Do not maintain a second browser gameplay implementation.

The initial production architecture is a modular C# reimplementation, developed one measured behavior at a time against a fixed reference version of the original game. Use 2 Ship 2 Harkinian (2Ship) as a separately built reference and a possible source of understood behavior, with provenance recorded for any reused code or data. A native source-simulation bridge is a research alternative with a bounded feasibility gate, not an assumed shortcut. An enhanced 2Ship port is a third, distinct product direction if preserving the existing simulation becomes more important than Unity tooling and a fully rebuilt presentation.

South Clock Town is the first complete vertical slice. Its first reviewable milestone is a recognizable, correctly proportioned environment with a working camera and traversal; its gameplay-complete milestone includes the selected town behaviors, time transitions, scene exits, and reset/save semantics defined in the reference specification. A beautiful environment alone does not establish a faithful remake.

## 2. Evidence and current constraints

| Observation | Evidence | Consequence |
| --- | --- | --- |
| The workspace has a `Unity/Assets` directory, but successful project initialization has not been demonstrated. | Local directory inspection during this task. | Treat Unity settings, imported packages, scene compilation, and build output as unverified. |
| The bootstrap attempt used an editor under `6000.5.0f1` and terminated with `No valid Unity Editor license found` and return code 198. | Root `bootstrap.log`, inspected during this task. | Editor compilation, play mode, package compatibility, rendering, and player builds are blocked until a valid local editor session is available. Authentication and license activation remain user actions. |
| The assigned environment inventory reports editors `6000.4.11f1` and `6000.5.0f1`. | Parent task's local inventory; not independently repeated here. | Do not silently install another editor or let multiple editors rewrite the project. Select and pin one verified version. |
| `prototype/package.json` pins Three.js `0.181.1`, Vite `7.3.7`, and Playwright `1.56.1`. | Local manifest; reviewer test results are in `evidence/verification.json`. | Browser checks establish the reviewer only, not Unity gameplay. |
| Unity exposes native C/C++ integration, but that is a language boundary, not a ready-made game integration. | [Unity native plug-ins](https://docs.unity3d.com/6000.0/Documentation/Manual/plug-ins-native.html). | A 2Ship bridge would require substantial adaptation and verification. |
| 2Ship has its own C/C++ CMake build, `libultraship` dependency, and asset-extraction workflow. Its `PlayState` code also references graphics, interaction, and interpolation systems. | [2Ship build instructions](https://github.com/2ship2harkinian/2ship2harkinian/blob/develop/docs/BUILDING.md), [CMakeLists](https://github.com/2ship2harkinian/2ship2harkinian/blob/develop/CMakeLists.txt), [play-state source](https://github.com/2ship2harkinian/2ship2harkinian/blob/develop/mm/src/code/z_play.c). | Source cannot simply be dropped into Unity as C# gameplay. No stable headless stepping/snapshot ABI has been established in this project. |
| 2Ship documents replacement asset archives and requires a supported user-provided game copy; it does not include the original copyrighted assets. | [2Ship README](https://github.com/2ship2harkinian/2ship2harkinian/blob/develop/README.md). | A port experiment uses its documented asset workflow. Art review GLBs and Unity prefabs are a separate pipeline; they are not automatically valid 2Ship replacement assets. |

Public documentation changes. Record exact upstream commits and actual installed package versions when implementing; the links above are discovery references, not immutable dependency locks.

Current seed: `Unity/ProjectSettings/ProjectVersion.txt` provisionally targets installed `6000.5.0f1`. `Unity/Assets/Editor/ImportSouthClockTown.cs` reads `shared/scenes/south-clock-town.blockout.json` and creates proxy envelopes using a built-in Standard material fallback; URP is not installed/configured. `Unity/Assets/Scripts/ProxyAssetIdentity.cs` retains IDs and provenance. The seed is source-reviewed, not compiled/imported. A sanitized failure record is in `evidence/environment.json`; the full local bootstrap log is ignored. The proposed production tree below is future work, separate from these actual seed paths.

## 3. Architecture decisions

### ADR-001: Unity is the production runtime

**Proposed decision:** Unity 6, Universal Render Pipeline, Windows desktop first. Use conventional GameObjects, prefabs, scenes, C# assemblies, and editor tooling. Avoid DOTS/ECS, networking, procedural city generation, and runtime mod-loading infrastructure until a demonstrated requirement needs them.

**Why:** a character-driven, authored adventure benefits from integrated animation, camera tooling, lighting, scene editing, profiling, and controller support. Those are production workflow reasons, not evidence that Unity automatically reproduces the original game's feel.

**Version gate:** once licensing is available, test the installed `6000.5.0f1` with the smallest URP project, glTF import, input, a character, and a Windows build. Pin `ProjectVersion.txt`, package manifest, package lock, and a successful smoke-build record. If choosing a currently supported LTS editor instead, make that an explicit recorded change before significant scene authoring. The Unity documentation currently labels `6000.3` as LTS and `6000.4`/`6000.5` as legacy; verify support again at version selection. Do not equate an installed editor with the best long-term version. [Unity versioned documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/plug-ins-native.html)

**Exit condition:** a fresh checkout opens without import/compile errors and produces a runnable Windows development build with the pinned editor. Until then the engine choice is architectural intent, not a verified toolchain.

### ADR-002: One gameplay authority

**Proposed decision:** a Unity-hosted game session owns authoritative state. The web viewer is read-only with respect to the game. It may scrub an art-review time-of-day slider, play an animation clip, display authored collision proxies, and move a review camera. These are inspection controls; they do not implement NPC schedules, quests, save files, or player physics.

If a browser walkthrough is needed during the license block, label its controls and output as environment preview. Keep any temporary navigation code isolated and disposable. Do not grow it into the game's baseline controller.

**Exit condition:** the browser can review the same asset revision Unity imports, while no gameplay state or rule is implemented twice.

### ADR-003: Start with C# reimplementation; gate source reuse

| Route | Strength | Main technical cost | Decision |
| --- | --- | --- | --- |
| Reimplement the selected behavior in C# | Native Unity authoring; controllable presentation; testable domain state; straightforward debugging. | Every implemented mechanic needs measured reference behavior, including timing and obscure state dependencies. Full-game fidelity is a long program, not a South Clock Town task. | Default for the first slice. Expand only after parity evidence. |
| Run adapted 2Ship simulation behind Unity presentation | Potentially retains existing schedules, flags, puzzle logic, and other behavior rather than rewriting it. | Engine/platform coupling; renderer separation; authoritative collision and animation ownership; actor identity; native ABI; audio/cutscene hooks; save/restore; interpolation; upstream maintenance. | A bounded feasibility experiment only, outside the production branch until its gates pass. |
| Enhance 2Ship itself | Existing native port, game simulation, and documented replacement-asset system provide a practical base for an enhanced port. | Must work within or extend that renderer/resource/actor pipeline; Unity scenes, shader graphs, and prefabs are not directly usable. A radically different rendering stack is still engineering work. | Viable fallback/product fork, not a Unity implementation detail. |

Revisit this decision before building broad quests, combat, transformations, or dungeons. Do not pay for both a complete C# implementation and a production source bridge simultaneously. A source bridge that passes its gate replaces the relevant C# authority; it does not become an additional authority.

### ADR-004: glTF is the art interchange, not the game format

Use `.blend` or another agreed DCC source for editable artwork; export `.glb` for mesh, texture, skin, and animation review. Store gameplay definitions, spawn IDs, collision semantics, schedules, interactions, and save policy in explicit versioned data. A mesh name is not a durable quest or save identifier.

Unity imports the art once in the editor and wraps it in authored prefabs. Candidate importer: Unity glTFast, package ID `com.unity.cloud.gltfast`; select and pin a compatible release after the editor gate. Three.js uses its `GLTFLoader`. Neither importer proves custom Unity stylized materials will appear identically in the other renderer. [glTFast](https://github.com/atteneder/glTFast), [Three.js GLTFLoader](https://threejs.org/docs/pages/GLTFLoader.html)

## 4. Runtime boundaries and state flow

```text
Input devices
    -> Input adapter -> tick-stamped commands
    -> Session coordinator -> domain state and ordered events
        -> player motor / collision adapter
        -> clock and schedule evaluator
        -> interactions, dialogue, inventory, event flags
        -> scene-transition coordinator
        -> save/reset service
    -> read-only presentation snapshot
        -> character/NPC views, animation, camera, audio, UI, lighting

Authored definitions -> import/validation -> domain definitions + Unity bindings
Art source -> GLB export -> Unity imported assets + Three.js review
Reference fixtures -> parity tests and review captures
```

The core is ordinary C# with no scene searches, `MonoBehaviour` dependencies, `GameObject` references, or Unity serialization objects. A small `System.Numerics`-based geometry vocabulary or project-owned vector structs can be used where necessary. Unity adapters translate engine vectors and collision-query results. Keep engine physics out of the pure state tests rather than pretending PhysX is deterministic across platforms.

Use assembly boundaries to enforce direction:

| Assembly | Responsibility | Allowed dependencies |
| --- | --- | --- |
| `Majoras.Core` | IDs, commands, clock arithmetic, inventory/flags, schedule evaluation, reset rules, snapshot/save models. | .NET and project primitives only. |
| `Majoras.Content` | Authored definition assets and validated conversion into core definitions. | Core, Unity serialization/editor support as appropriate. |
| `Majoras.Runtime` | Session lifetime, Unity collision/motor adapter, scene binding, input adaptation, presentation coordination. | Core, Content, engine packages. |
| `Majoras.Presentation` | Views, animation binding, camera, UI, audio, environment. | Core snapshots/events and Unity; no independent quest/time authority. |
| `Majoras.Editor` | Import rules, asset validation, fixture capture tools, scene audit. | Editor-only; never included in the player. |
| `Majoras.Tests.*` | Pure state tests, editor import tests, play-mode integration tests. | Only the code under test plus approved test packages. |

Start with direct references and explicit composition in a bootstrap object. A large dependency-injection framework, global event bus, or service locator is unnecessary for the slice. Events carry stable IDs, simulation tick, cycle generation, and sequence so the consumer can distinguish a new event from a repeated snapshot.

Example interfaces to define before implementation:

```text
IGameSession.Step(InputFrame, CollisionFrame) -> SessionSnapshot + events
IClock.Advance(ticks, mode) -> crossed boundaries
ISchedule.Resolve(actorId, cycleTime, flags) -> intended state
ISceneCatalog.Resolve(sceneId, entranceId) -> content binding
ISaveStore.Write/Read(versioned DTO) -> explicit result
IReferenceTrace.Read(scenarioId) -> expected observations
```

These describe responsibilities, not a commitment to exact method signatures. Keep asynchronous disk and scene operations outside the synchronous `Step` call.

## 5. World coordinates, scale, and authored data

### Conventions to freeze before environment production

| Item | Project convention |
| --- | --- |
| Unity distance | 1 world unit = 1 meter. All movement speeds and collision tolerances name their units. |
| Unity axes | +Y up, +Z project north, +X east. Character prefab forward is +Z. |
| South Clock Town origin | Current proxy: rectangular footprint center. Future survey proposal: tower doorway ground projection. Rebase through a versioned transform; never silently reinterpret old coordinates. |
| Scene origin | Each scene has a local survey origin; entrances and actors use scene-local coordinates and a stable `SceneId`. No open-world floating origin is needed for this slice. |
| Rotations | Quaternion storage for transforms; human-facing yaw degrees in Unity tools; explicit radians conversion at glTF/data boundaries. |
| Actor pivot | Feet/root on the ground; visual rig below a dedicated model child; collision capsule independent from rig proportions. |
| Static mesh pivots | Building/prop pivot at a documented foundation or attachment point. Large environment chunks share the scene survey origin when useful. |
| IDs | Authored, stable, unique IDs for actors, entrances, triggers, schedule segments, and persistent objects. Never Unity instance IDs or display names. |

Unity uses a left-handed coordinate system. glTF uses a right-handed system with +Y up, a front-facing asset convention along +Z, and meter units. Importers must handle that difference. Do not apply an extra axis flip because a model looks wrong before inspecting the importer and export settings. [Unity orientation](https://docs.unity3d.com/6000.0/Documentation/Manual/QuaternionAndEulerRotationsInUnity.html), [glTF coordinates and units](https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html#coordinate-system-and-units)

The initial shared scene JSON uses the Unity world convention above: meters, Y up, +Z north, +X east. Unity consumes those transforms directly. The Three.js review adapter reflects Z once to render them in its right-handed space. Express that conversion as a basis matrix and derive rotations through the corresponding basis change; do not scatter hand-negated Euler angles or point components across tools. The schema must declare its coordinate convention so later exporters cannot silently reinterpret it.

GLB assets have their own glTF coordinate convention. Do not reflect them a second time simply because the JSON adapter reflects Z. Record whether an export already contains the review adapter's conversion, test the pinned Unity importer's actual conversion, and apply any required orientation correction at one documented wrapper boundary. A Unity import and a Three.js export can each perform legitimate handedness conversion while still requiring a known asset-forward adjustment between them. The E1 fixture must prove the end-to-end result rather than relying on an assumed importer mapping.

An original-game coordinate is a separate `ReferenceGameSpace`, with source version, units, origin, and transform attached. Do not assume that one original unit is a centimeter or that one screenshot supplies a metric scale. Measure several anchor distances, establish the conversion, then validate doorway widths, stair heights, and player-relative proportions. A stylized replacement should preserve gameplay clearance and spatial relationships even when its surface detail changes.

Schema v1 clarification: `collision` currently means boolean envelope intent, not browser physics. Future gameplay schemas need collision categories. `size` is full authoring-envelope dimensions; the shelter pivot is at ground level and other primitives use a center pivot. Decorative child geometry is not a measured bound. The implemented shared validator/conversion is `shared/scene-contract.mjs`.

### Calibration fixture

Create one small owned calibration GLB before importing the town: a one-meter cube, labeled positive axes, an asymmetric arrow, an off-center pivot, a skinned joint chain, one animation, a normal map, an alpha-cutout card, and separate collision proxy geometry. Check it in both renderers. Pass criteria: expected dimensions, correct winding, no mirrored skeleton, correct tangent-space lighting, matching animation endpoints, and known transform/pivot values. This catches errors that a symmetric building hides.

### Data ownership

Maintain an asset manifest with `assetId`, source path, exported path, revision/hash, exporter version/preset, bounds, material profile, animation list, provenance, and review status. Maintain scene definitions separately with `sceneId`, entrances, spawn bindings, exits, interaction volumes, actor routes, camera zones, and collision categories. Commit generated manifests only when they can be reproduced.

The initial proxy-node contract is `id`, `kind`, `position`, `size`, `yawDegrees`, `material`, `collision`, and `provenance`. IDs are unique within an explicit scene namespace and retained when geometry changes. `material` resolves through a shared palette/profile table; it is not an arbitrary shader name. Define `collision` as a validated category rather than overloading visible shape names. `size` must state whether it means full bounds or another dimension convention. Nodes marked `provenance: "proxy-unmeasured"` are layout hypotheses, even if their names match real landmarks. Do not promote them into surveyed reference data or final collision without review. Future schema revisions can add actor/entrance semantics without turning this visual blockout format into the entire gameplay model.

Keep large binary art in Git LFS once the team agrees to the storage setup; do not introduce it blindly during initial bootstrap. Exclude local game dumps, extracted game archives, caches, local save files, editor credentials, and generated build output from source control. Any copied upstream files need recorded source commit and applicable notices; the top-level upstream license alone does not inventory all dependencies or authorize original game assets.

Interchange decision gate: compare pinned glTFast against explicit FBX exports from the same Blender source. Prefer canonical GLB for both consumers if skin, clips, materials and reimport pass; otherwise generate FBX plus textures for Unity from the same source revision. No independently edited browser/Unity models. Today's viewer generates JSON proxies and exports GLB; loading authored GLB is a later task, not implemented yet.

## 6. Art and glTF pipeline

1. **Survey and blockout:** use the reference specification to define landmark transforms, visible silhouettes, terrain elevations, entrances, and walkable clearances. Review a simple Unity blockout against fixed reference camera views before high-detail modeling.
2. **Author:** use meter-scaled DCC files with documented export settings. Apply intentional transforms before rigging/export. Keep visual surfaces, collision proxies, sockets, and animation clips separately named and identifiable.
3. **Export:** produce versioned `.glb` assets, initially without exotic extensions or mesh/texture compression. Prefer a conservative metallic-roughness or unlit review material baseline. Bake unsupported procedural DCC materials into textures where needed.
4. **Validate:** run the Khronos validator, check missing textures, external URI policy, bounds, scale, finite transforms, required skeleton joints, allowed material slots, and manifest consistency. glTF validation establishes format correctness, not correct art or collision. [Khronos validator](https://github.com/KhronosGroup/glTF-Validator)
5. **Import into Unity:** use the pinned importer to produce imported assets; create wrapper prefabs containing stable bindings, colliders, LODs, interaction sockets, and production materials. Do not edit generated imported hierarchy data manually. Reimport must preserve the wrapper's durable bindings through explicit mapping and validation.
6. **Review in Three.js:** use the identical GLB hash, a meter grid, orbit/free camera, fixed composition cameras, lighting presets, clip playback, bounds, and optional collision-proxy overlay. Report missing assets loudly. Dispose replaced scene resources correctly when swapping models.
7. **Production rendering:** map approved material IDs to the Unity stylized shader profile. Validate the final appearance in Unity, including shadows and post-processing; the browser proxy is not final shader approval.
8. **Optimize after approval:** introduce LODs, texture compression, atlases, instancing, or mesh compression against a measured target. Record which optimizations alter asset output and re-run the import/calibration checks.

Start with URP, linear lighting, a restrained stylized material library, authored palettes, readable silhouettes, controlled shadow softness, and deliberate fog. Use painted/albedo detail and selective normal maps rather than treating extra geometric density as fidelity. Keep day/night parameter curves in data and layer them over a stable lighting baseline. Unity shader code and post-processing should not be recreated in the viewer simply to chase screenshot identity.

## 7. Simulation, time, and reference fidelity

### Three distinct clocks

1. **Presentation time:** camera smoothing, particles, UI transitions, and visual interpolation. Runs per rendered frame and can continue while the simulation is paused where the design allows.
2. **Simulation time:** ordered integer ticks for authoritative commands, motor updates, interactions, and events. Proposed initial movement step: 60 Hz, subject to feel and performance verification.
3. **World-cycle time:** an integer representation of progress through the three-day cycle with a declared epoch, day boundaries, speed mode, and pause rules. It advances through explicit conversion from simulation ticks, using integer/rational arithmetic rather than accumulated floating-point seconds.

Do not assume the reference game's regional timing, gameplay tick rate, clock speed, or pause behavior from memory. Choose the target version first and measure them. A 60 Hz motor is a proposal for smooth presentation, not a claim that the original simulation ran at 60 Hz. If a behavior depends on a reference update cadence, reproduce that cadence through an explicit gameplay substep/accumulator and document it. Frame interpolation cannot repair different underlying decisions.

Unity's fixed step and catch-up behavior are configurable; its default 0.02-second physics interval should not silently determine this game's timing. Configure a measured step and a catch-up limit, and log overloads. [Unity fixed-timestep documentation](https://docs.unity.com/en-us/engine/6000.5/manual/physics-section/physics-overview/physics-optimization/cpu/frequency)

### Tick order and overload policy

Sample input per rendered frame, retain button edges until consumed, and assign commands to ticks. In each authoritative tick: consume commands; advance approved clocks; process boundary events; evaluate schedules; resolve movement/collision and interactions in a fixed order; commit state; publish events/snapshot. Finalize the precise order with reference tests because interaction-versus-clock-boundary ordering can change outcomes.

Use one coordinator rather than relying on incidental ordering among many `Update`/`FixedUpdate` methods. Interpolate view transforms between committed snapshots; snap across teleports, resets, and scene loads. On a long render stall, cap catch-up work and record the discontinuity. Do not advance quest time using wall-clock catch-up while character/world logic ran fewer ticks. Handle application suspension as an explicit pause/resume policy. The remake does not need real-world offline progression.

World-cycle slowdown changes the world clock's conversion rate, not all Unity animation/physics through `Time.timeScale`. Explicit pause reasons include menu, dialogue, cutscene, scene transition, reset, and debug inspection; the reference specification decides which clocks each reason stops. Support nested pause reasons so closing one menu does not accidentally resume a cutscene.

### Schedules and crossed boundaries

Represent schedule segments with stable IDs, conditions, half-open time intervals, intended location/route/action, and transition policy. Derive an actor's intended state from `(cycle time, flags, scene)` rather than advancing a coroutine for seventy-two in-game hours. Use authored paths and schedule anchors where fidelity matters; runtime NavMesh routing alone can choose different paths and arrival times.

Process a time interval's crossed boundaries, not just `currentTime == eventTime`. This must work for normal progression, a multi-minute skip, day rollover, and reset. A jump does not necessarily replay every transient animation or line; each event declares whether it must apply state, emit presentation, or be skipped during a seek. Seeking a debug clock must be visibly a debug operation with documented state reconciliation.

Use deterministic ordering and an explicit seeded random stream for logic that needs reproducible randomness. Do not claim bitwise determinism for Unity collision or floating-point motion. Reference tests compare discrete outcomes exactly and motion/camera observations within measured tolerances.

## 8. Player, camera, collision, and animation

### Player and collision

Begin with a kinematic capsule motor backed by Unity casts, sweeps, and ground probes. Avoid an uncontrolled dynamic rigidbody for the main character. Keep movement parameters in a versioned profile: acceleration, braking, turn behavior, slope limit, step handling, ledge rules, gravity, jump/fall response, and form-specific dimensions. Every numeric value starts provisional until measured.

Separate render meshes from collision. Prefer simple authored collision for stairs, walls, platforms, and tight doorways; static concave meshes are appropriate where needed, while moving pieces use suitable simple/convex shapes. Give surfaces semantic data such as walkable, climbable, water, interaction, camera obstruction, and transition. Do not infer climbability from a decorative texture.

Define a collision-layer matrix early: player/world, actor/world, interaction sensors, camera blockers, triggers, and cosmetic objects. Validate corner sliding, slopes, steps, seams, dropped frames, high-speed movement, and entrance thresholds. Preserve collision clearances when replacing an approved blockout mesh with final art.

For a source-simulation bridge, these decisions change: source collision must remain authoritative unless deliberately replaced and revalidated. Unity colliders would then support presentation/camera queries only, and cannot independently move the player or NPCs. This is a key architecture gate, not a minor adapter detail.

### Camera

Own a small explicit camera state machine: follow, recenter, target/interaction, authored shot, transition, and debug review. Evaluate a pinned Cinemachine package if it reduces implementation, but keep gameplay mode and target selection outside package-specific components. Measure reference framing, orbit/recenter timing, occlusion handling, target transitions, and input behavior; a generic third-person rig is only a bootstrap.

Run camera presentation after the player view update. Use dedicated obstruction tests with a tunable shoulder/target composition, bounded near clipping, and recovery smoothing. Capture fixed camera presets for South Clock Town's entrance, tower, stairs, and major sightlines so composition changes are reviewable.

### Animation and cutscenes

Core state owns movement/action transitions and hit/interaction outcomes. Presentation selects clips and blends from that state. Prefer in-place locomotion driven by the motor initially; root motion is opt-in for authored actions with a documented reconciliation policy. Cosmetic animation events may trigger sounds or particles, but must not be the sole authority for durable inventory, quest flags, or damage.

Define a stable rig/skeleton contract, clip naming, loop flags, root-motion policy, sockets, and contact markers. Retargeting may be useful for development, but verify final body proportions and foot placement. Interpolate movement independently of intentionally stepped or stylized pose timing. Cutscenes coordinate input locks, camera, animation, dialogue, and clock policy through a named sequence and cancellation/restore rules.

## 9. Saves, cycle resets, and scene transitions

### State classes

The exact persistence rules must be derived from the chosen reference version; the following classes are the proposed implementation vocabulary, not a claim that all original items follow one simple rule.

| State class | Examples of architectural contents | Lifetime |
| --- | --- | --- |
| Permanent progression | Stable progress records, persistent item ownership, unlocked features as individually specified. | Retained by declared reset rules. |
| Cycle state | Current cycle time, cycle-local flags, schedule consequences, temporary objects/inventory entries as specified. | Rebuilt or transformed by a cycle reset. |
| Session state | Active scene, player transform/velocity, active dialogue, view state, loaded handles. | Runtime; selectively captured only for a defined save mode. |
| User settings | Controls, audio/display/accessibility settings. | Independent from game progression and cycle reset. |
| Debug state | Inspection camera, cheats, accelerated time, fixture identity. | Clearly separate from normal saves and parity runs. |

Use versioned save DTOs with schema version, game/content version, slot ID, target-reference profile, stable content IDs, and validation. Never serialize live Unity objects, native pointers, coroutines, or an arbitrary scene hierarchy. Differentiate a manual/normal save, a suspend snapshot, and a debug fixture; implement only the reference-supported modes being claimed. A convenience suspend feature must be declared as an enhancement if it changes behavior.

Write to a temporary file, flush, validate/read back, then replace the prior slot through the platform save-store adapter. Retain a recoverable previous generation and reject incomplete or incompatible data explicitly. Test interrupted writes and migration with actual disk files. Keep saves out of the repository.

### Reset transaction

1. Enter a reset state; block gameplay input and prevent concurrent save/scene transitions.
2. Capture the pre-reset state needed for recovery and apply an explicit per-field persistence table.
3. Create the next cycle state, reset clock epoch, assign a new cycle generation, and resolve the target entrance.
4. Rebuild schedule-dependent actors/objects from definitions plus the retained progression. Cancel prior-generation delayed work, events, audio loops, and scene callbacks.
5. Load/bind the target scene, place the player, settle collision, initialize the camera, and publish the first valid snapshot.
6. Commit the corresponding persistent save only at the defined safe point; unlock input after the scene is coherent. A failed transition returns an explicit error/recovery state rather than partially retaining both cycles.

The reset should be idempotent under duplicate commands and reload recovery. Event deduplication keys include cycle generation so a legitimate event can occur again in the next cycle. Rehydrating state must not replay rewards, duplicate NPCs, or award items merely because a view was spawned.

### Scene transitions

Use a small always-loaded bootstrap scene plus additive content scenes. For South Clock Town, keep authored static environment and bindings in one content scene until size/ownership warrants more division. A transition record contains source/target `SceneId`, `EntranceId`, clock policy, and sequence ID. Disable new interactions, load target, resolve persistent/cycle state, place actors/player, set the camera, unload the previous scene, and then return control. Do not let scene lifecycle callbacks independently grant rewards or advance schedules.

For the initial slice, out-of-scope exits may produce a clearly labeled development boundary. They must not pretend to be completed adjacent areas. Later implement interiors/adjacent areas as separate scenes through the same entrance contract.

## 10. A bounded 2Ship simulation-bridge experiment

This experiment is optional and must run before broad C# content implementation if it is to influence the production choice. Do not start it as an unlimited parallel rewrite.

**Research budget proposal:** five focused engineering days after the reference build works, with a written stop/go review. This is a timebox, not a promise of feasibility.

**Stage A — pin and build reference:** choose upstream commit and supported game revision, inventory native dependencies/licenses, build the unchanged port, capture reference South Clock Town behavior, and preserve an unmodified baseline. No game data is committed.

**Stage B — test separation:** map initialization, per-step logic, input, resources, graphics submission, audio, save storage, and shutdown. Determine whether the scene can initialize and advance without the original window/render loop while retaining required engine services. Do not assume disabling draw calls removes all gameplay/render coupling.

**Stage C — minimal ABI:** prototype a C ABI with versioned fixed-layout structs and explicit ownership. Candidate calls: `Create`, `LoadScene`, `SubmitInput`, `Step`, `ReadSnapshot`, `ReadEvents`, `Save`, `Load`, `Reset`, `Destroy`. No raw actor pointers or Unity-managed objects cross the boundary. Return errors explicitly. Own all simulation mutation on one thread and copy snapshots at a committed boundary.

**Stage D — visual binding:** mirror only the player, one static mesh, one scheduled NPC, clock, and one interaction in Unity. Map persistent logical IDs to prefab views; extract or adapt world transforms, pose/skeleton state, action state, dialogue/interaction state, camera intent, and discrete audio events as needed. Establish the source-to-meter transform and exact tick rate. Source owns authoritative movement/collision and save state; Unity owns rendered views and agreed camera effects.

**Stage E — lifecycle proof:** repeat scene unload/load, time skip, pause, reset, save/load, and DLL unload/restart without invalid memory, duplicate events, leaked actors, or mixed-generation state. Separate render interpolation from authoritative simulation ticks.

**Go criteria:** repeatable scripted inputs produce the reference observations; the whole lifecycle above works; a clean-machine Windows build packages native dependencies; Unity and native debugging are usable; required data/pose hooks are understood; performance/memory overhead is measured; the maintenance and asset pipeline costs are accepted.

**Stop criteria:** no reliable single-step boundary, substantial gameplay dependence on original graphics side effects without a bounded solution, unmanageable pointer/global-state lifetimes, inability to reproduce save/reset lifecycle, or art replacement requiring effectively a second renderer reimplementation. Document the failed assumption and continue with the C# route or explicitly choose an enhanced native port.

The upstream [play-state implementation](https://github.com/2ship2harkinian/2ship2harkinian/blob/develop/mm/src/code/z_play.c) is a starting point for this dependency map, not proof of a supported embedding API. Unity's [native plug-in support](https://docs.unity3d.com/6000.0/Documentation/Manual/plug-ins-native.html) establishes only that the interop mechanism exists.

## 11. Proposed repository layout

```text
Majoras/
  docs/                         # Scope, reference specification, art bible, architecture
  evidence/                     # Dated review captures, manifests, measurements, test summaries
  art-source/                   # Editable owned art; large binaries under agreed LFS policy
  shared/
    assets/                     # Exported review GLBs and textures, identified by manifest
    manifests/                  # Asset IDs, hashes, bounds, provenance, export settings
    schemas/                    # Versioned scene/asset/review data schemas
    reference-fixtures/         # Observations and input traces; no game dumps
  tools/
    export/                     # Reproducible DCC export support
    validate/                   # Asset/schema/ID checks
  Unity/
    Assets/Majoras/
      Art/                      # Imported/shared-art staging and production material bindings
      Audio/
      Content/                  # Definitions, routes, dialogues, environment profiles
      Prefabs/
      Scenes/                   # Bootstrap, SouthClockTown, test scenes
      Scripts/{Core,Content,Runtime,Presentation,Editor}/
      Tests/{EditMode,PlayMode}/
    Packages/                   # Manifest and lock committed after successful initialization
    ProjectSettings/            # Editor version, input/physics/layer/render settings
  prototype/                    # Thin Three.js asset/composition reviewer
    src/
    public/                     # Generated or linked review staging; avoid hand-maintained copies
    tests/
    scripts/
  native-spike/                  # Only if the bridge experiment is approved as a direction
```

This tree is proposed, not a list of existing implementation. Keep shared export artifacts in one canonical location and use a preparation script to stage them into Unity/viewer as required. The staging manifest records hashes so stale copies fail validation. Never make artists manually update two asset directories.

## 12. Implementation sequence and acceptance gates

| Gate | Deliverable | Evidence needed before advancing |
| --- | --- | --- |
| E0: Toolchain | Licensed, pinned Unity/URP project and thin viewer skeleton. | Clean import/compile, Windows player launches, viewer production build loads, version/package record. |
| E1: Coordinate and import | Calibration fixture, export preset, manifest/schema, single staging path. | Correct meter dimensions, orientation, normals, skin/clip endpoints in Unity and viewer; reimport preserves bindings. |
| E2: Traversal blockout | Surveyed South Clock Town geometry, player motor, camera, collision, entrance IDs. | Reference-view overlays and landmark tolerances; complete traversable route; no fall-through/stair/doorway blockers; gamepad/keyboard smoke check. |
| E3: Time and actors | Integer cycle clock, pause policy, one full NPC route/interaction and relevant boundary behavior. | Trace comparisons at normal, reduced, and irregular render rates; skipped-time boundaries; exact event/flag outcomes. |
| E4: Lifecycle | Scene transition, save/load, cycle reset, actor rehydration. | Interrupted-write recovery; repeated resets/loads; no duplicate rewards, actors, or prior-cycle callbacks; defined persistent fields match reference. |
| E5: Art integration | Approved town environment and character pass, production materials, audio/lighting pass. | Unity captures at named cameras/times; traversal still passes after art replacement; performance profile on named hardware. |
| E6: Slice acceptance | Packaged South Clock Town build and reference matrix with explicit scope. | Fresh-checkout build, clean-user save path, reproducible acceptance run, known differences documented, user visual/feel review. |

Do not advance to large world production because E1 looks attractive. E2-E4 determine whether the foundation can support a faithful adventure. If conducting the native bridge experiment, insert its decision gate before E3 and revise the ownership contracts accordingly.

## 13. Verification and provisional budgets

Pure state tests should cover clock rollover, rational-speed accumulation, nested pause reasons, interval boundary crossing, conditional schedule selection, reset persistence, duplicate-command handling, event generation, save schema validation, and migrations. Test outcomes that can break player progress, not getters that mirror implementation.

Play-mode tests should cover actual collision and scene binding, input-edge consumption, transition input locks, rig/prefab contract, animation alignment for important interactions, and reset/load with real views. A deterministic input replay at several render caps should produce identical discrete domain outcomes and bounded movement/camera differences. Set those tolerances from measured behavior before calling a test a fidelity pass.

Visual acceptance needs fixed reference profiles: game version, location, time, flags, player form, camera transform/FOV, output aspect ratio, and asset revision. Compare landmark silhouette, apparent scale, entrance position, elevation, route clearance, framing, and day/night readability. Store screenshots and review notes; screenshot similarity alone cannot validate hidden flags or schedules.

Provisional performance target: 60 fps at 1920x1080 on a named Windows reference machine, with separate CPU/GPU frame timings, memory, loading, and shader-warmup measurements. No hardware class or measured frame rate is established in this document. Establish the machine and budgets at E0, profile at E2, then constrain art at E5. The viewer receives its own lightweight loading/render budget; it is not a proxy for Unity performance.

## 14. Open decisions, risks, and immediate spikes

| Risk or open decision | Next bounded action | Exit criterion |
| --- | --- | --- |
| Unity unavailable due to license state. | User activates/opens the intended editor; then run E0 smoke checks. Continue reference survey and asset schemas meanwhile. | Real editor import and Windows player evidence, not another directory creation. |
| Reference target is ambiguous. | Choose original release/region/revision, supported copy, baseline settings, and whether enhancements are enabled. Record source-game hash privately if needed. | One reproducible reference profile; N64 and 3DS behavior are not mixed accidentally. |
| Reimplementation scope grows without parity. | Implement one route/interaction/reset chain before a broad framework. | Its reference trace and lifecycle checks pass. |
| Source bridge is mistaken for ready-made integration. | Execute only the bounded experiment in section 10 if pursuing it. | Explicit stop/go decision with measured prototype evidence. |
| Export/import produces mirrors or wrong scale. | Build E1 calibration fixture before town final art. | Dimensions, orientation, skin, materials, pivots, and sidecar transform tests pass. |
| Final art changes traversability. | Separate collision from render art and validate clearances after every replacement. | E2 traversal tests still pass on the final environment. |
| Time skips/resets duplicate state. | Implement state classes, generation IDs, interval events, and transactional reset before broad quests. | Repeated skip/reset/save/load yields expected state with no duplicated one-shot effects. |
| Generic Unity movement feels wrong. | Measure reference acceleration, turning, camera, slopes, and interaction distances; tune in a small test scene. | Reviewed traces and player-feel acceptance for the selected form. |
| Two renderers create a maintenance trap. | Limit browser controls to art/composition review; share exports/manifests. | No duplicate quest/movement/schedule logic and no manual asset-copy workflow. |
| Material parity consumes art time. | Approve Unity as final rendering authority; use a documented browser proxy material profile. | Reviewer understands which differences are proxy rendering versus asset defects. |
| Save compatibility becomes accidental. | Version DTOs and stable content IDs from the first working save. | Old fixture migration and corrupt-write recovery work; incompatible data fails clearly. |

The next implementation work should therefore be E0/E1, a reference-grounded traversal blockout, and one end-to-end gameplay/lifecycle chain. Broad asset production and full-game promises should follow that evidence.
