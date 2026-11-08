# South Clock Town acceptance plan

Status: **proposed acceptance specification; no test below is recorded as passed by this document**. Prepared 2026-11-08. Scope and evidence sources are defined in [SOUTH_CLOCK_TOWN.md](SOUTH_CLOCK_TOWN.md).

## 1. What each kind of evidence means

The Three.js viewer can establish that a manifest loads, a proxy renders, review controls work and an export contains expected objects. Unity editor tests can establish controller/system behavior in the editor. A standalone Windows build requires its own input, traversal and performance run. Original-game fidelity requires platform-matched references and comparative review. Keep these four results separate in status reports.

Use `PASS`, `FAIL`, `BLOCKED`, `NOT RUN` or `DEFERRED` for each case. `BLOCKED` needs the missing dependency and the next action. A deferred requirement is outside the signed scope and cannot silently count toward completion. Current geometry is **UNMEASURED** until the M0 survey is complete.

Every evidence record must contain:

- Case ID, result, tester, UTC timestamp and concise observed result.
- Git commit and dirty-state note; engine/browser version and executable/build hash where applicable.
- Manifest, survey and reference-profile revisions.
- Scene, spawn, story/form profile, cycle day/time, inventory and flags.
- Input device, resolution, quality preset, frame cap, machine CPU/GPU/RAM and driver for performance cases.
- Reproduction steps, expected result, actual result, artifact path and any development/debug controls used.

Proposed evidence location is `evidence/<milestone>/<run-id>/` with a result index, screenshots/videos and relevant logs. Keep reference captures/provenance distinct from project renders. If third-party images are stored locally for comparison, record their source and do not include them in a release bundle by accident.

## 2. Milestone gate summary

| Gate | Required case groups | Completion definition |
| --- | --- | --- |
| M0 | REF, SUR, GEO-01 | Reference profile and all anchors/routes inventoried; measurement uncertainties explicit. Without direct survey, approve proxy work only |
| M1 | MAN, GEO, MOVE, CAM, PORT | Graybox can be traversed and reviewed; state/coordinate contracts stable |
| M2 | ART-01 through ART-05, CHAR, existing M1 regressions | Character, tower and representative kit approved before full environment production |
| M3 | All ART, LIGHT, relevant PERF smoke run, M1 regressions | Complete exterior art works from gameplay cameras in required lighting states |
| M4 | INT, NPC, TIME, TOWER, RESET, SAVE where enabled | Local systems produce correct outcomes through boundaries, re-entry and resets |
| M5 | BUILD, PERF, SOAK, all enabled groups, separate fidelity review | Reproducible standalone candidate with no blocking defect and explicit remaining scope |

## 3. Reference and survey gates

| ID | Setup / action | Pass condition |
| --- | --- | --- |
| REF-01 | Open the reference register | Every used capture has platform, known build/revision, aspect, day/time, form/story state and source. Unknowns are stated |
| REF-02 | Compare N64 and 3DS source lists | N64 geography is primary; imported 3DS differences have explicit decisions. No N64 South bank is introduced by implication |
| REF-03 | Replay the documented reference route | Another reviewer can reproduce the relevant viewpoint/state. A single third-party screenshot cannot pass the whole route survey |
| REF-04 | Review local screenshot and release inputs | Source URL/version uncertainty recorded; screenshot not treated as floor plan; only intended files enter distributable package |
| SUR-01 | Inspect coordinate/anchor register | Origin, axes, scale method and datum defined. Unknown dimensions remain null/unmeasured; proxy sizes labeled separately |
| SUR-02 | Overlay at least three independent surveyed views | Tower, passage and perimeter anchors agree within a recorded uncertainty. A fit from only one camera is insufficient |
| SUR-03 | Compare route widths, elevations and travel durations | Scale choice fits character/door/plaza/tower relationships together; any deviations listed with reason and reviewer |
| SUR-04 | Review construction, NPC and tower captures across states | State-dependent geometry/occupancy has evidence or a named blocker; no single-time screenshot stands for all days |
| SUR-05 | Lock survey revision | Each measured field has provenance and method; reviewer signs measured fidelity separately from art preference |

Default survey tolerance must be chosen from evidence quality before measurement review. Do not claim a centimeter-level match from perspective frames. Tighter render alignment is useful only when camera calibration and coordinate evidence support it.

## 4. Manifest and interchange

| ID | Setup / action | Pass condition |
| --- | --- | --- |
| MAN-01 | Validate manifest | IDs unique; references resolve; all numbers finite; bounds and variant references valid; unknown survey coordinates explicit |
| MAN-02 | Load canonical asymmetric test fixture into viewer and Unity | +X is east, +Z north and Y up in canonical data; viewer adapter preserves intended placement/orientation; labels do not mirror |
| MAN-03 | Export/reimport fixture with rotated non-symmetric mesh | Positions, dimensions, pivots, normals and facing remain correct; no inverted faces or double Z conversion |
| MAN-04 | Replace tower proxy with authored asset | Stable ID and anchors survive; interactions, review cameras, colliders and clock binding do not depend on mesh order |
| MAN-05 | Request each day/tower variant | Only intended instances/colliders change; no duplicate platforms or invisible blocking geometry |
| MAN-06 | Remove an asset or supply invalid manifest in development | Clear actionable validation failure; no silent placement at world origin or apparently valid empty scene |
| MAN-07 | Open any review build | Build/manifest/reference revision and unmeasured-proxy status visible where applicable |

Viewer-only acceptance includes orbit, zoom, reset view, wireframe, day/night preview, landmark labels and GLB export if those controls are shipped. A day/night preview switch is not an implemented game cycle. The viewer needs no Link controller, gameplay interactions or simulated schedules.

## 5. Geography, traversal and portals

The following route IDs describe test intent. Bind them to surveyed anchors before recording measured route results. Each route is performed walking and running, in both directions where meaningful, at least once with keyboard/mouse and once with controller in the standalone candidate.

| Route | Traversal |
| --- | --- |
| R01 | South spawn → tower front threshold → return |
| R02 | South approach → west passage → west boundary → return |
| R03 | South approach → east passage → east boundary → return |
| R04 | Tower front → left/right tower circuit → north boundary → return |
| R05 | Plaza → laundry passage boundary → return |
| R06 | Tower front → flower/raised approach → reachable ledge under selected movement profile |
| R07 | Tower approach before opening → blocked rooftop boundary; repeat after opening → enabled boundary |
| R08 | Each NPC/fixture approach → interaction exit → nearest main route |
| R09 | Every stair, curb, narrow corner, canopy support and platform edge on the supported route network |

| ID | Setup / action | Pass condition |
| --- | --- | --- |
| GEO-01 | Inspect plan and landmark list | Original features/routes accounted for or explicitly blocked; current proxy is never called surveyed layout |
| GEO-02 | Compare six locked reference cameras | Tower silhouette, skyline, opening placement and construction masses match the selected survey or have approved deviations |
| GEO-03 | Walk R01–R09 with collision debug enabled | No route-blocking clutter, missing ground, invisible wall, unintended shortcut or out-of-bounds escape |
| GEO-04 | Compare all active geometry and colliders | Day/tower state visible geometry and collision agree; decorative ropes/banners do not obstruct unless intentionally physical |
| MOVE-01 | Start, stop and turn at low and full input | Speed/acceleration predictable, no input spike, idle drift or turn-in-place sliding beyond approved animation tolerance |
| MOVE-02 | Run the route suite at 30, 60 and uncapped frame rates | Equivalent reachability and movement distance within the declared simulation tolerance; diagonal input is not faster |
| MOVE-03 | Traverse supported stairs/curbs/ramps from shallow and direct angles | No snagging, bobbing into geometry, perpetual falling or unapproved auto-climb |
| MOVE-04 | Roll/stop near walls, posts and ledges | No tunneling, collider shrinking or clipping; unsupported ledges remain unreachable |
| MOVE-05 | Disconnect/reconnect controller; change keyboard focus | Input clears safely, pause/menu navigation remains usable and movement does not stick |
| MOVE-06 | Reach a safe fallback after invalid spawn/fall | Player recovers deterministically without losing declared persistent progress or advancing a reward twice |
| PORT-01 | Approach every portal from reachable sides | Trigger matches physical opening and correct destination name; no transition through adjacent wall |
| PORT-02 | Cross and return through each stub portal | One transition per crossing; correct safe arrival, facing and camera; slice boundary clearly identified |
| PORT-03 | Attempt guarded exit with allowed and denied profile | Correct source-backed allow/deny result; denial does not trap input or falsely complete a quest |
| PORT-04 | Hold input during transition / immediately reverse direction | No repeated load loop, double spawn or re-trigger until player clears the receiving trigger |

Proposed controller tolerances are project requirements, not original measurements: no visible sustained wall penetration, no unsupported route escape and no reproducible snag on intended traversal. Numeric epsilon, maximum step, slope limit and capsule size must be recorded after character-scale tuning rather than scattered as undocumented constants.

## 6. Camera and character acceptance

| ID | Setup / action | Pass condition |
| --- | --- | --- |
| CAM-01 | Full orbit at every route corner | No near-plane view into voids, wall clipping or camera escape outside the finished shell |
| CAM-02 | Back into wall, pass a pillar and leave it | Collision correction keeps Link readable; recovery has no repeated zoom pumping or abrupt viewpoint flip |
| CAM-03 | Walk under canopy and beside tower base | Camera does not force unsupported player movement or hide the active prompt; fallback framing remains controllable |
| CAM-04 | Recenter while moving in all cardinal directions | Heading is predictable and does not invert input or snap through an obstruction |
| CAM-05 | Test minimum/maximum sensitivity and inverted Y | Mouse/controller controls honor settings; settings survive intended session persistence |
| CAM-06 | Enter/cancel/finish dialogue near obstruction | Both subjects frame sensibly; input ownership returns; camera resolves to a valid gameplay pose |
| CAM-07 | 16:9 at 1080p and one narrower/wider supported aspect | Link, HUD, prompts, tower cues and moon remain readable; no clipped UI |
| CHAR-01 | Front/side/back neutral character turntable in scene light | Young Link identity, green silhouette, proportions and expression approved; no adult-proportion drift |
| CHAR-02 | Idle/walk/run/turn/stop/roll loop under gameplay camera | Animation transitions have weight, plausible foot placement and no major clipping or scale pop |
| CHAR-03 | Change art LOD and shadow distance | Silhouette and animation hold; accessories do not detach or collapse visibly |
| CHAR-04 | Compare character to door, stall, stair and tower base | Approved scale relationships hold across the area; controller capsule matches the agreed movement envelope |

## 7. Art, lighting and sound

Lock six review cameras during M0/M1: south approach toward tower, tower-front low/medium view, west passage toward plaza, east passage toward plaza, rear/north approach and raised construction/tower approach. Store actual surveyed poses/lenses; these names do not supply coordinates. Add a moon visibility view if none of those frames serves it.

| ID | Setup / action | Pass condition |
| --- | --- | --- |
| ART-01 | Compare tower silhouette and face at six cameras | Immediately recognizable organization; new geometry supports original forms; no unrelated cathedral/generic clock substitution |
| ART-02 | Review representative material cluster in day/night | Stone, timber, thatch/cloth, paint and metal have distinct coherent responses; saturation/value grouping follows approved target |
| ART-03 | Review street-scale construction details | Bevels, attachments and material thickness hold near gameplay camera; no floating beams, paper-thin structural forms or obviously repeating hero detail |
| ART-04 | Review Link/environment composition strip | Tower prominence and character scale approved; expanded detail does not overwhelm the youthful figure or change route hierarchy |
| ART-05 | Toggle low/high detail and wireframe | Structural richness is actual authored geometry where promised; texture polish is not used to conceal incorrect shape |
| ART-06 | Inspect front/back/side of stalls and perimeter | All gameplay-visible surfaces finished, sensible depth and no exposed voids |
| ART-07 | Compare all banner/sign anchor groups | Rhythmic color and readability remain; cloth motion does not intersect roofs repeatedly or hide landmarks |
| ART-08 | Inspect each day construction state | Correct state recorded in manifest; clutter and scaffolds reflect reference or approved deviation |
| ART-09 | Asset/provenance audit | Every shipped asset has author/source and intended-use record; no accidental extracted game files or reference screenshots in package |
| LIGHT-01 | Day/dusk/night/final-hours capture at same cameras | Distinct atmosphere, stable readable routes and preserved colorful/eerie target; no unapproved uniform red/gray wash |
| LIGHT-02 | Advance continuously across lighting keys | No exposure flash, abrupt fog wall, shadow pop or fixture state mismatch |
| LIGHT-03 | Inspect tower spotlight and sun/moon panel | State follows shared clock, not independent viewer/UI timer; matching reference behavior reviewed separately |
| LIGHT-04 | Look at moon from required routes at all review times | Direction, scale and occlusion are deliberate and compared against reference; no foreground building intersection |
| LIGHT-05 | Disable optional expensive effects | Critical navigation/cues still visible; quality settings do not remove clock readability |
| AUDIO-01 | Approach tower, worksite and quiet rear passage | Original project ambience/cues locate plausibly, respect volume controls and avoid clipping/stacked duplicate events |
| AUDIO-02 | Re-enter area or skip across cue boundaries | No runaway sound accumulation; one event cue per intended crossing; seek reconstruction does not replay every skipped bell |

Music composition, original soundtrack reproduction, full voice acting and final cinematic sound are deferred unless separately commissioned. Placeholder sound must be labeled in the candidate report.

## 8. Interactions and NPC schedule cases

| ID | Setup / action | Pass condition |
| --- | --- | --- |
| INT-01 | Approach each enabled fixture/NPC | Prompt only at valid range/facing/visibility; walls and distant targets cannot steal interaction |
| INT-02 | Start, advance, cancel and repeat dialogue | Correct input ownership, no stuck camera/cursor or unintended movement; cancellation does not grant outcomes |
| INT-03 | Offer missing or wrong item to scrub | No deed, consumption or completion flag; clear bounded response |
| INT-04 | Offer Moon's Tear from labeled debug fixture | Exactly one valid exchange; inventory/ownership flags change atomically; departure state reached |
| INT-05 | Hold confirm, spam input, repeat exchange after completion | No duplicate item, state drift, overlapping dialogue or repeated departure event |
| INT-06 | Reset/re-enter during exchange boundary states | Defined recoverable result; no half-consumed item and missing reward combination |
| INT-07 | Inspect disabled/deferred feature | Honest inspection or clear unavailable boundary; no claim that save/soaring/Deku flight already works |
| NPC-01 | Review all schedule rows | Each enabled temporal claim has actor/day/time/story/source/path mapping; unverified rows labeled provisional |
| NPC-02 | Test one tick before, at and after each authored boundary | Actor state/path/visibility matches interval policy; no duplicate or briefly missing identity |
| NPC-03 | Observe local path in both directions / obstruct path | No wall walking, permanent player blockade or visible teleport; recovery follows declared policy |
| NPC-04 | Enter district mid-route and after route completion | Correct sampled state; no replay of all prior actions or duplicate reward |
| NPC-05 | Fast-forward across multiple schedule intervals | Same final state as natural advance for equivalent clock/story state; skipped one-shot effects handled deliberately |
| NPC-06 | Actor leaves via out-of-scope district boundary | Correct portal handoff/absence; actor not visible in two places or respawned immediately behind camera |
| NPC-07 | Toggle relevant story/form flag from test fixture | Re-evaluates intended state; unrelated NPCs unchanged; source dependencies recorded |

The postman is optional for M4 until local scene-path mapping is available. A smaller verified roster can pass the slice. Replace “all NPC schedules work” with the exact roster and intervals actually exercised.

## 9. Cycle clock, tower opening and resets

Use deterministic test fixtures for boundary tests and a standalone natural-advance run for integration. Boundary values below are test intentions; final tick mapping must follow the selected reference build.

| ID | Setup / action | Pass condition |
| --- | --- | --- |
| TIME-01 | Start post-intro fixture at Day 1 06:00 | HUD, scene state, tower, lighting and NPC evaluation agree on one authoritative clock |
| TIME-02 | Advance over midnight on nights 1 and 2 | No premature dawn/day transition; calendar-display policy documented and consistent |
| TIME-03 | Advance over 06:00 day boundaries and 18:00 light boundaries | Correct day/night and authored state changes once; no skipped/duplicated transition |
| TIME-04 | Test 30/60/uncapped frame rates with equal simulated duration | Same final clock and event/state results within declared tolerance; no frame-count dependent cycle |
| TIME-05 | Pause/menu/dialogue/cutscene fixtures | Clock obeys the documented policy; differences from original pause exceptions listed instead of claimed exact |
| TIME-06 | Seek forward/backward in development panel | State reconstructs consistently; no negative remaining time or reward replay; debug use recorded |
| TIME-07 | Natural advance vs accelerated advance from same fixture | Same logical end state for every enabled system; presentation-only cues follow their explicit skip policy |
| TIME-08 | Compare original and project real-time route/cycle timing | Measured speed/pause parity within agreed tolerance or explicit deviation; BLOCKED until reference timing exists |
| TOWER-01 | Observe hour/two-minute display cues and dawn/dusk | Mechanisms follow sourced/captured character and shared clock; no unrelated clock hands or generic spin substituted |
| TOWER-02 | Day 3 final night just before/at/after midnight | Closed → opening → stable open exactly once; correct clock, visual cue and access state |
| TOWER-03 | Re-enter after opening or load open-state fixture | Correct pose/collision immediately; no second opening cinematic or temporary closed blocker |
| TOWER-04 | Stand at each reachable moving-part boundary during opening | No embedding, crushing, launch or unsupported fall; player-control policy explicit and safe |
| TOWER-05 | Approach rooftop portal before and after opening | Locked/enabled state matches tower and prerequisites; visible doorway and trigger agree |
| TOWER-06 | Reset cycle after opening | Closed geometry, collision and access restored once; no leftover stair collider |
| RESET-01 | Reach expiration of third night | Scoped failure/reset behavior occurs once; input/camera recovered at start fixture |
| RESET-02 | Trigger development reset after exchange | Temporary inventory, ownership, NPC and tower state reset per declared contract |
| RESET-03 | Reset with enabled permanent collectible/settings | Declared persistent data survives; no duplicate collectible reward |
| RESET-04 | Run three complete accelerated cycles | No accumulated actors, colliders, lights, audio cues, event listeners or increasingly divergent state |
| RESET-05 | Reset while paused, in dialogue or on a portal boundary | Single controlled transition; no orphan UI, stuck input or split scene/clock state |
| SAVE-01 | If local save is enabled: save/reload mid-cycle and after reset | Exact declared state restored; format versioned; no unintended full-game persistence claims |
| SAVE-02 | If enabled: interrupted/corrupt/older local save fixture | Safe explicit recovery; user data not silently overwritten as a successful load |

Full Song of Time input/performance, complete bank persistence, the entire inventory, rooftop Skull Kid encounter, moon-crash cinematic and full-game quest save parity are deferred. They cannot be counted as implemented because a developer reset button exists.

## 10. Build, performance and repeatability

Set the reference machine before M3 profiling. Proposed candidate target is **1920×1080, 60 fps on a named Windows desktop configuration**, subject to recording the actual CPU/GPU/RAM and agreeing the quality preset. An unspecified “midrange PC” cannot pass the gate. If no target machine is selected, functional build review can proceed and performance acceptance remains BLOCKED.

| ID | Setup / action | Pass condition |
| --- | --- | --- |
| BUILD-01 | Fresh documented checkout/build with declared engine/tool versions | Reproducible build or actionable missing dependency list; no reliance on an undocumented personal asset path |
| BUILD-02 | Run standalone on target machine | Scene loads, input/settings/pause/quit work; no missing material/shader/asset or unhandled error |
| BUILD-03 | Review packaged contents | Correct build/profile/revision label; reference images, temporary debug exports and unrelated files excluded |
| BUILD-04 | Run with save/config folder absent | Safe defaults and successful first launch; no manual editor step required |
| PERF-01 | Warm-up then 120-second fixed route through all principal views | Proposed 60 fps target: p95 frame time ≤16.7 ms and p99 ≤25 ms, measured and reported with CPU/GPU breakdown; targets approved before recording |
| PERF-02 | Stress tower view at night with full required effects/NPCs | Meets agreed frame budget; no uncontrolled shadow/light cost or sustained hitching |
| PERF-03 | Record first traversal, warmed traversal and state changes | Loading/shader-compilation hitches reported separately; no masked first-run failures |
| PERF-04 | Inspect profiler, LOD and culling transitions | No visible hero-asset collapse, missing shadow or disappearance from valid gameplay camera; draw/triangle/memory counts recorded |
| PERF-05 | Compare baseline and low quality profile | Both usable; no critical cue/interaction disappears; performance change measured |
| SOAK-01 | 30-minute standalone run including three accelerated cycles | No crash, progressive memory growth, accumulating entities, input loss or stuck transition |
| SOAK-02 | Repeat manifest load/area enter/exit ten times | Stable counts after cleanup; no duplicate NPC/system instances or multiplied cues |

The frame-time numbers are proposed product budgets, not evidence about this machine or a current build. Set platform-specific budgets if the target changes. Mobile, web gameplay and consoles are outside this slice.

## 11. Review records and blocker policy

Required review artifacts at M5:

1. One uninterrupted playable route video showing build label, keyboard/mouse or controller input, interaction, night transition, tower opening and reset. Shorter targeted clips supplement it.
2. Six paired reference/project cameras at declared matching states, with all lens/position uncertainty disclosed.
3. Top-down route/collision plan and the source-backed landmark/portal checklist.
4. Boundary-test result table for clock, selected NPCs, exchange, opening, re-entry and reset.
5. Performance report with target hardware, preset, route, frame-time distribution and profiler captures.
6. Build instructions, executable identity, asset provenance index, deviations and known-limitations list.

Severity definitions: `P0` crash/data loss/unusable build; `P1` broken intended route, wrong version geography, duplicated outcome, clock/state disagreement or failed agreed performance target; `P2` noticeable visual/camera/animation defect without blocking the intended route; `P3` minor polish. M5 has zero open P0/P1 issues. Remaining P2 issues require explicit recorded acceptance; visual fidelity remains a separate sign-off.

Missing reference access blocks a **fidelity claim**, not useful authored proxy work. Missing Unity/playable implementation blocks gameplay acceptance, even if the browser viewer looks good. Missing target-hardware profiling blocks performance acceptance. Keep each blocker attached to the specific requirement and state what evidence resolves it.

## 12. Current baseline record

- Reference planning sources have been identified and one local visual reference inspected.
- The original geometry, collision mesh, calibrated dimensions, exact camera poses and full NPC route survey have not been supplied or validated.
- The shared layout is an authored, unmeasured composition proxy.
- This document records no executed gameplay, standalone, fidelity or performance passes.
- Milestone case results remain NOT RUN unless a separate dated evidence run supplies them.
