# South Clock Town: first faithful vertical slice

Status: **planning specification; current layout is an UNMEASURED proxy**. Prepared 2026-11-08. This document describes a future Unity playable slice and the reference work needed to substantiate it. The lightweight Three.js review viewer is an authoring aid for composition, materials and export; it does not establish playable-game or original-game fidelity.

## 1. The result we are aiming for

Build a compact, walkable South Clock Town that someone familiar with the Nintendo 64 game recognizes from the approach, silhouettes, routes and changing atmosphere. Keep the strange, colorful theatrical character: a huge decorated clock, weathered enclosed streets, carnival preparations, warm materials and a moon whose presence becomes uncomfortable. Give those forms richer geometry, coherent materials, expressive animation and contemporary lighting. Expanded detail should explain the existing town rather than change its geography.

The first gameplay profile is a **post-introduction exploration sandbox with young human Link**. This makes character and camera evaluation useful immediately. It is not a recreation of the opening, which begins under different story and transformation conditions. A separate first-cycle story profile and Deku gameplay require a later, explicit scope decision.

The first complete slice contains the exterior district, a small set of local interactions, a bounded local schedule demonstration, the three-day clock and the exterior tower opening. Neighboring districts, the tower interior and rooftop are transition boundaries. A correct-looking boundary is sufficient here; a finished neighboring area is not required.

## 2. Reference hierarchy and version contract

Use **N64 layout and gameplay as the primary baseline**. Use 3DS references selectively for construction and material ideas, with every adopted difference recorded. Modernization can improve rendering and controls while preserving route structure, landmark relationships and deliberate restrictions.

Every factual survey record must name platform, region/revision where known, day, time, story state, form, camera position and source. Do not label an image simply “original.” N64 US is a proposed reference target because the decompilation provides a readable behavioral source; selecting and recording the exact reference build is an M0 gate.

| Evidence | What it supports | What it does not establish |
| --- | --- | --- |
| [zeldaret/mm scene table](https://github.com/zeldaret/mm/blob/main/include/tables/scene_table.h) | South Clock Town is `Z2_CLOCKTOWER` / `SCENE_CLOCKTOWER` (`0x6F`); the tower interior and rooftop are separate scene entries | Scene dimensions, door transforms or the full exit graph |
| [Clock Tower actor](https://github.com/zeldaret/mm/blob/main/src/overlays/actors/ovl_Obj_Tokeidai/z_obj_tokeidai.c) | Distinct face, minute ring, exterior gears, sun/moon panel, counterweight and rooftop stairs; opening at Final Day midnight | A measured modern rig, animation timing acceptance or matching visuals |
| [Town Business Scrub actor](https://github.com/zeldaret/mm/blob/main/src/overlays/actors/ovl_En_Sellnuts/z_en_sellnuts.c) | Moon's Tear exchange and Land Title Deed grant are real behavior, not invented set dressing | The town flower's surveyed transform or an implemented Deku launch mechanic |
| [Carpenter actor](https://github.com/zeldaret/mm/blob/main/src/overlays/actors/ovl_En_Daiku/z_en_daiku.c) | Actor variation, path and dialogue logic deserve a state-specific survey | That every worker is present in every screenshot or at all times |
| [Postman schedule](https://github.com/zeldaret/mm/blob/main/src/overlays/actors/ovl_En_Pm/scheduleScripts.schl) and [schedule documentation](https://github.com/zeldaret/mm/blob/main/docs/schedule_scripting_language.md) | Schedule decisions depend on scene, time, day and story flags; the postman has South Clock Town states | A generic NPC loop being an accurate schedule; the community-readable script format being Nintendo's original authoring language |
| [Cycle reset implementation](https://github.com/zeldaret/mm/blob/main/src/code/z_sram_NES.c) | Cycle reset selectively preserves flags and removes temporary resources | That a new save system already reproduces the complete game's persistence rules |
| [South Clock Town reference article](https://www.zeldawiki.wiki/wiki/South_Clock_Town) and [bank reference](https://www.zeldadungeon.net/wiki/index.php?mobileaction=toggle_view_desktop&title=Clock_Town_Bank) | Secondary corroboration of version-specific geography, including the relocated bank | A substitute for direct platform-matched captures or metric survey |

The local `south-clock-town-reference.png` is a third-party N64-style visual reference from [Zelda Dungeon](https://www.zeldadungeon.net/wp-content/uploads/2014/11/South_Clock_Town.png). It shows the decorated face, wall treatment, stalls, banners, platform and workers from one viewpoint. It is not a floor plan, all-day roster or calibrated survey. Keep its source and version uncertainty in the reference register. Publication of our planning material does not require redistributing this image.

### Version differences that must remain explicit

| Subject | N64 baseline | 3DS reference | Slice rule |
| --- | --- | --- | --- |
| Bank | West Clock Town | Relocated to an alcove in the rear of the Clock Tower in South Clock Town | Do not put a functioning bank in the N64 South district. A future 3DS/hybrid mode must be labeled and authored separately |
| Plaza and stairs | Survey the original enclosed circulation and level changes | Expanded plaza and altered/symmetrical tower-side stair treatment are reported in the secondary reference | Do not silently adopt the expanded 3DS footprint |
| Owl statue | Survey N64 position and activation/save semantics | Position and save behavior differ between versions | Reserve the feature, then author against the chosen version; a generic “save anywhere” prompt is not fidelity |
| Bombers and incidental placements | Version- and state-specific capture required | Do not assume matching hiding places or props | Excluded from first local schedule until surveyed |
| Color and surface detail | Preserve N64 color relationships, value grouping and eerie theatrical tone | Useful additional construction detail | Borrow craft ideas without inheriting all brighter palette/lighting choices |

## 3. Area inventory and route graph

Treat this as a survey work list. Only the identities and broad adjacency are established; positions, widths, counts and exact state-dependent arrangements remain unmeasured.

| Stable feature | Design responsibility | Required survey proof |
| --- | --- | --- |
| Clock Tower exterior | Dominant silhouette, decorated face, mechanisms, base entrance, elevated access and final-hours configuration | Front/rear/side views, orthographic or reconstructed outline, door and ledge heights, moving component pivots |
| Plaza ground and perimeter | Preserve enclosure, corners, surface patterns, route hierarchy and tower sightlines | Ground outline, elevations, gate/passages and wall tops from multiple views |
| Carnival construction | Distinct platform/scaffold states, timber, ropes and work space | Per-day geometry, collision and worker occupancy; do not assume one scaffold state for all three days |
| Stalls, thatch/cloth canopies and crates | Secondary silhouettes and human-scale clutter | Exact occupied footprints and which props obstruct movement |
| Banners and painted/carnival signs | Color rhythm, local graphic language and controlled animation | Anchor positions, original motifs, front/back appearance and readable distance |
| Town Deku flower and Business Scrub | Landmark plus bounded dialogue/exchange hook | Position, owner state, departure path, nearby ledge relation |
| Owl statue and mailbox | Recognizable interaction fixtures | Chosen-version transforms, activation state and approach clearance |
| South gate/guard | Town boundary and form/story-sensitive restriction hook | Gate throat, guard location, allowed and denied cases |
| Dog and passing townspeople | Signs of a living district | Selected actors' paths, collision, time and story dependencies |
| Sky, moon, tower spotlight and atmosphere | Visible pressure of the cycle without hiding navigation | Same camera across day/night/final hours; moon direction/apparent size and light behavior |

Proposed route graph to verify at M0: South Clock Town exterior connects to East Clock Town, West Clock Town, North Clock Town, Laundry Pool, the southern Termina Field gate, and the tower's base interior entrance. The raised tower approach leads to the rooftop boundary only under the appropriate tower state. Scene table entries support the destination identities, but **do not prove the physical graph or approach coordinates**; direct traversal capture must confirm each edge.

Use stable portal IDs such as `portal.east`, `portal.west`, `portal.north`, `portal.south_field`, `portal.laundry`, `portal.tower_interior`, `portal.tower_rooftop`. A stub portal may fade, display the actual destination name, and return to a safe exterior spawn. It must visibly communicate the slice boundary; it must not lead into an invented district.

Design the promenade as connected walkable space, not a row of buildings on a flat square. Test the tower circuit, side passages, south approach, construction detours and the raised approach separately. Modern clutter may not narrow a route, block a destination, add a convenient shortcut or turn a deliberate ledge into an ordinary stair without an approved deviation entry.

## 4. Coordinate survey before metric fidelity

### Canonical project convention

Use meters, Y-up, `+X = east`, `+Z = north` in the shared Unity-oriented manifest. Set the canonical origin to a clearly named surveyed point; proposed point is the ground projection of the tower's front/base entrance center. Record its exact definition before any coordinate lock. Y=0 is the survey's chosen plaza datum, not a claim that every plaza surface is level.

The Three.js review adapter flips Z when mapping world positions. A reflection also affects orientation and triangle winding; convert rotations, normals and any handedness-sensitive data through one tested adapter. Do not let viewer-specific signs enter the manifest. An asymmetric east/west marker fixture and a rotated prop must verify the round trip before real assets are accepted.

### Survey sequence

1. Record the reference build and reproducible start state. Capture the full traversal at an unmodified resolution/aspect and retain original frames.
2. Capture a top-down view or reconstruct a plan from at least three overlapping views. A single perspective screenshot cannot determine depth or world scale.
3. If a user-provided, authorized local reference setup supplies collision/scene coordinates, record their provenance and transform; otherwise measure normalized relationships from captures and mark uncertainty.
4. Choose a project scale only after comparing character height, door clearance, tower footprint, route width and travel time together. Do not assume an undocumented N64-unit-to-meter conversion.
5. Survey anchors first, then boundaries, ground elevations, passage widths, ledges, stairs, interaction points, NPC paths and camera test poses. Keep coordinates in a machine-readable table.
6. Fit the proxy against the survey with a uniform transform. Record every intentional local exception rather than stretching different sections until one camera looks convincing.
7. Review plan overlay, silhouette pairs and traversal duration before locking geometry. Retain the unmeasured proxy as a separate revision for comparison.

Proposed future survey row:

```json
{
  "id": "anchor.tower_front_threshold",
  "referenceVersion": "N64_US_REVISION_PENDING",
  "sourceCapture": "capture-id-pending",
  "day": 1,
  "time": "12:00",
  "storyProfile": "post_intro_exploration",
  "positionMeters": null,
  "positionOriginalUnits": null,
  "measurementStatus": "unmeasured",
  "uncertaintyMeters": null,
  "notes": "Origin candidate; do not supply invented coordinates."
}
```

Keep the following cells unknown until measured: tower width/height/depth; front/rear thresholds; main portal centers; laundry passage; plaza outline; all ground levels; platform state footprints; ledge and stair heights; flower, owl, mailbox and NPC anchors; moon transforms; original camera/FOV; original route times. Unknown values are `null`/`unmeasured`, not zero. An artist's practical authoring size belongs in a separate `proxyTransform` field.

## 5. Scene manifest proposal

The manifest is the shared content contract for the viewer and future Unity importer. Rendering, collision, portals, interactions, schedules and light cues reference stable IDs. Do not generate these independently from building names or mesh indices.

### Existing proxy identifiers to preserve

| ID | Meaning | Current evidence class |
| --- | --- | --- |
| `sct.env.clock_tower` | Tower mass and clock assembly proxy | Unmeasured authoring proxy |
| `sct.env.perimeter_west` / `east` / `north` / `south` | Perimeter composition proxies | Unmeasured; not original wall counts |
| `sct.prop.market_stall_west` / `east` | Stall composition proxies | Unmeasured; names do not prove original placement |
| `sct.prop.construction_platform` | First platform mass | Unmeasured; per-day variants required later |
| `marker.spawn_south` | Review/player approach marker | Proposed authoring location |
| `marker.exit_west` / `east` / `north` / `south` | Navigation reference markers | Proposed locations; no operational game transitions implied |

### Data to add as the slice matures

| Collection | Required fields / behavior |
| --- | --- |
| `referenceProfile` | Platform/build, source citations, capture set, survey revision, fidelity status and approved deviations |
| `assets` | Stable ID, authored source, provenance/license record, prefab/mesh, pivot, bounds, LODs and replacement status |
| `instances` | Stable ID, asset ID, parent, canonical transform, state variant, measurement status, source anchors and render layer |
| `collision` | Separate simplified collider ID, owning instance, collision role, walkable surface and state variant |
| `portals` | Source trigger volume, destination ID, arrival anchor, guard conditions, stub status and safe fallback |
| `interactions` | Actor/fixture ID, approach anchor, facing/range/visibility rules, action set, state prerequisites and localized text key |
| `schedules` | Actor ID, source-backed time/day/story predicates, state result, path ID, arrival/departure policy and evidence reference |
| `paths` | Stable route/point IDs, canonical points, speed/timing policy, occupancy and boundary ownership |
| `clockRig` | Named pivots for face/ring/gears/panel/counterweight/stairs, motion curves, cue events and final-hours pose |
| `lightingProfiles` | Time keys, directional/ambient/fog/exposure/fixture settings and source/reference capture IDs |
| `reviewCameras` | Canonical pose, lens, output resolution, intended comparison and validated survey revision |
| `testRoutes` | Ordered anchors, input script/profile, allowed transitions, expected end state and approved tolerance |

Unknown survey transforms remain unmeasured even when a proxy is renderable. The importer should reject duplicate IDs, dangling references, invalid bounds, nonfinite transforms and a `measured` label without evidence. Emit a visible build label containing manifest revision, reference profile and “UNMEASURED PROXY” where applicable.

## 6. Traversal, collision and camera

### Movement scope

Implement a controllable young human Link with idle, walk, run, turn, stop, roll and contextual step/ledge traversal sufficient for this district. Use analog speed, predictable acceleration and camera-relative movement. Keep the physical capsule and movement controller independent of the final character mesh so art revisions do not silently change reachable space. Decide free jump versus contextual jumping from the fidelity target before coding; the default plan preserves contextual traversal rather than adding a universal jump button.

Deku flower launch, transformations, swimming, combat, items and the opening escape are deferred. The flower remains owned/unowned scenery plus interaction state until the Deku movement slice exists. Do not fake flight by teleporting human Link onto the tower.

### Collision ownership

Author ground, steps/ramps, walls, parapets, canopy supports, stalls and platform colliders separately from decorative meshes. Classify surfaces as walkable, blocking, camera-only or noncolliding detail. Use simple stable geometry for normal traversal; reserve exact mesh collision for static shapes that truly require it.

Check concave corners, shallow curb edges, stair tops/bottoms, transitions between ground sections, the rear tower passage, canopy posts and the south gate throat. Never repair a bad route by shrinking Link's collision capsule locally. A moving tower component needs state-aware collision and safe player placement: an opening animation must not leave Link embedded, pinched or standing on removed ground.

### Modern third-person camera

Provide orbit, recenter, shoulder-safe near-wall behavior, adjustable sensitivity/invert controls and controller support. Keep Link's full movement silhouette readable and leave room above him for the tower and moon. Survey-comparison cameras are separate locked lenses; a default modern camera is not proof of matching the original lens.

Proposed first tuning range is a 55–65 degree vertical FOV and a distance near 2.5–3.5 Link heights, explicitly an authoring starting point. Resolve collisions with a volume sweep, retain a stable target near Link's upper torso, and damp recovery so a pillar does not cause repeated zoom pumping. Near-plane clipping must not expose voids through building shells. A dialogue camera may reframe only after confirming both subjects and a clear exit from the conversation.

## 7. Interactions and local life

| Feature | First complete slice | Deferred / prerequisite |
| --- | --- | --- |
| Directional sign / notice | Approach, contextual action, short original project text, close/cancel and return control | Exact copyrighted dialogue reproduction is unnecessary for this planning milestone |
| Carpenter | Talk, work/idle animation and a day-dependent local presence state once surveyed | Whole Carnival Committee and cross-district story scenes |
| Business Scrub | Recognizable actor, talk and local Moon's Tear → Land Title Deed state transition using a clearly labeled debug fixture for the prerequisite | Observatory quest, telescope event and flower flight |
| Owl statue | Visual landmark and inspect; activation/save semantics only after profile survey | Full save/soar network and version-specific save lifecycle |
| Mailbox | Inspect / limited local interaction after survey | Complete mail delivery and Anju/Kafei quest |
| South guard | Talk and one source-backed allow/deny rule appropriate to the selected form/story profile | All forms, inventory and progression combinations |
| Dog | Optional short local ambient loop with clear provisional label until original path survey | Full behavior parity |
| Postman | Source-backed local segment with boundary handoff; optional if source path coordinates remain unavailable | Complete cross-scene schedule and quest outcomes |

Every interaction must have an accessible approach, readable prompt, cancellation, exclusive ownership of player input and deterministic state outcome. A held button or duplicated callback may not grant an item twice. Wrong-item and already-completed cases must be explicit. Put debug inventory/time controls in a development panel, never in story dialogue disguised as original gameplay.

### Schedule discipline

Choose a small roster after M0 rather than populate arbitrary NPCs everywhere. A schedule row carries actor, day/time interval, story predicates, state, path and source. Do not equate “a worker animates” with “the original schedule works.” The available postman script offers an auditable source for local temporal states, but mapping result values to paths still requires actor-code and scene-path verification.

Schedules are evaluated from authoritative cycle time, not wall-clock timers or a random wandering loop. Define interval boundaries consistently. On fast-forward, load, re-entry or reset, evaluate current state directly and avoid replaying every skipped animation or reward. Keep one actor identity across a local route and portal handoff. Out-of-scope travel should leave through the correct boundary and become absent; it should not teleport in view or silently appear in two districts.

## 8. Clock, tower and three-day scope

Use one authoritative `CycleClock` shared by HUD, tower, schedules, lighting, moon and event triggers. Store cycle-relative time as deterministic ticks or a sufficiently precise scalar plus an explicit day/time projection. Keep simulation clock and presentation clock distinct so an animation cannot alter event truth.

Model the ordinary cycle from Day 1 06:00 to the end of the third night at the following dawn. A calendar date rollover at midnight is not the dawn-of-next-day event. Verify the exact original boundary rules and speed/pause behavior against the selected build before claiming parity. Provide developer-only set-time, pause and accelerate controls and record their use in evidence.

Required local behavior:

1. HUD, lighting, moon presentation, selected NPC states and tower display read the same time.
2. Clock face, surrounding ring, side gears, sun/moon panel and nighttime counterweight light are separately authored mechanisms; they are not generic continuously spinning clock hands. The source describes hour/two-minute updates and final-hours alternate states; reproduce the captured movement character during rig authoring.
3. Final Day midnight starts an idempotent exterior opening sequence, ends in a stable open pose and enables the raised rooftop portal only when its prerequisites are met.
4. Entering the area after the opening time creates the correct open configuration directly. Crossing backward using debug time uses an explicit reconstruction path, not a reverse story event.
5. Cycle expiration produces a clearly scoped failure/reset presentation, then returns to the configured start. Full moon-crash cinematics and the Skull Kid rooftop encounter are deferred.
6. A development reset demonstrates temporary state clearing and explicitly declared persistent local state. Full Song of Time music input, save serialization and the entire game's persistence table remain separate work.

Proposed slice persistence contract: local exchange ownership, actor positions, tower-open state and temporary inventory reset with the cycle; settings and explicit completed permanent collectible flags survive. The final list must be checked against the original before shipping. Do not automatically persist every `completed` boolean or clear all progress indiscriminately. If the Piece of Heart is enabled later, collection permanence and duplicate prevention become mandatory tests.

The first clock pass can use a documented development speed. “Three days work” means all authored boundaries and reset rules work under natural advance and accelerated replay; it does not mean the original real-time duration has been matched. Original speed and all pause exceptions are a separate measured-fidelity gate.

## 9. Visual expansion without losing the area

Author recognizable primary silhouettes first, then structural construction, then wear and small props. Tower silhouette and face motifs, wall/roof profiles, route openings, construction masses, banner rhythm and the moon must read at gameplay distance before texture polish.

Use bevels, inset masonry joints, layered timber, thick canopy edges, rope attachments, dimensional clock decoration and restrained material response. Keep the clock's graphic organization readable; additional ornamental geometry must not turn it into an unrelated realistic cathedral clock. Ground wear should follow actual routes, stalls should have plausible backs, and every camera-reachable wall needs believable depth.

Keep stone warm or moss-tinted where the reference supports it; reserve saturated reds, teals, greens and carnival accents for strong readable groupings. Daytime should remain colorful with unease under the surface. Night needs warm local light against a cooler environment, legible routes and deliberate pools of darkness. Final hours should increase pressure through moon scale/presence, sound and controlled environmental change, without washing everything into an indistinct red filter.

Modern young Link must retain youth, recognizable green silhouette, expressive face and large readable poses while gaining proper cloth layering, hands, hair, material detail and animation weight. Lock character height and silhouette with the environment survey. An adult-proportioned warrior in generic realistic armor would change the scene's emotional scale.

Approve a six-camera composition strip and day/night pair before full prop production. Approve one complete material sample cluster—stone, timber, thatch/cloth, paint and metal—under all required lighting profiles before multiplying it across the area.

## 10. Milestones and stop conditions

These are ordered gates, not a claim of work already completed. Detailed case IDs and evidence requirements are in [ACCEPTANCE.md](ACCEPTANCE.md).

| Milestone | Deliverable | Objective exit gate | Stop condition |
| --- | --- | --- | --- |
| M0 — reference and layout truth | Version contract, capture register, route graph, anchor survey and deviation ledger | All primary landmarks/routes accounted for; measured and unknown values separated; source captures reproducible | No original capture/coordinate access: proceed only with clearly labeled composition proxy; no fidelity approval |
| M1 — controlled graybox | Shared manifest, complete exterior circulation, proxy character, collision and modern camera | Every route test passes in Unity; coordinate round trip and state reconstruction pass; silhouette review recorded | Wrong route graph, lost tower prominence or collision workaround that changes scale |
| M2 — character and hero architecture | Young Link rig/movement, tower, representative perimeter/stall/construction kit and first lighting pair | Character/camera review plus six matched cameras; structural geometry and material sample approved | Character identity/scale or tower form unapproved; do not mass-produce detail |
| M3 — authored environment | Entire exterior with LOD/collision, fixtures, banners, night treatment and moon presentation | Every landmark traceable to reference or approved deviation; all camera/route tests repeat successfully | Decoration changes travel routes, unreadable nighttime navigation or unresolved silhouette mismatch |
| M4 — local playable systems | Bounded interactions, selected source-backed NPC states, shared cycle, exterior opening and reset | Boundary, negative-path, reset and re-entry cases pass; natural and accelerated time agree | Generic wandering presented as exact schedules; duplicated rewards; tower state disagrees with clock |
| M5 — candidate handoff | Standalone Windows build, reproducible evidence set, performance capture and known-limitations report | No blocking functional/visual defects; agreed hardware targets pass; reference fidelity signed off separately | Missing reference survey, unknown performance machine or screenshot-only “playable” evidence |

Every milestone review should say separately: implemented, tested in editor, tested in standalone, visually compared, original fidelity validated, and blocked. A build export or green automated test is not any of the other claims.

## 11. Reference capture checklist and current blockers

- [ ] Exact N64 reference build, display aspect, capture route and starting story/form state recorded.
- [ ] Front, rear, east and west tower views plus top-down/reconstructed district plan.
- [ ] Each portal approached and crossed in both directions; tower interior/rooftop transitions distinguished.
- [ ] Ground levels, stair/ledge heights, route clearances, collision-blocking props and supported traversal recorded.
- [ ] Same six camera poses on Day 1 daytime, nighttime, Day 2, Day 3, Final Night before opening and after opening.
- [ ] Construction changes and local NPC roster captured by day/time/story state.
- [ ] Scrub exchange, invalid/missing item, completed exchange, departure and cycle reset observed.
- [ ] Owl/mailbox/guard behavior captured for the chosen version and profile.
- [ ] Clock hour change, ring/gears update, dawn/dusk panel/light change and Final Day midnight opening captured frame-by-frame.
- [ ] Moon direction/apparent size and skyline compared from repeatable camera poses across the cycle.
- [ ] Route timing captured with declared movement/input conditions.
- [ ] 3DS references kept in a separate register; bank/stair/plaza differences marked before borrowing detail.

No original ROM, extracted scene/collision asset set, calibrated camera set, original scene-path coordinates or verified complete NPC survey was supplied for this planning pass. The public decompilation is useful behavioral evidence, but extraction metadata and actor code alone do not supply a measured district mesh. Geometry, dimensions, paths, exact animation timing, reference speed and all unobserved schedules remain blockers to a fidelity claim. Do not download a game image or assume decompiled names convey permission to redistribute game assets. Continue authored work with sourced references and honest proxy labels while these gaps are resolved.
