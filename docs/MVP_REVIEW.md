# South Clock Town MVP: recovered review record

Updated 2026-11-08 by reading surviving files after a reported computer lockup. This file repairs an existing README link whose target had not been saved. No new Unity execution or visual acceptance occurred in this documentation pass. Use [HANDOFF.md](HANDOFF.md) to resume and [FULL_CITY_PLAN.md](FULL_CITY_PLAN.md) for the next developer's assignments.

## What the current preview contains

The local `SouthClockTown_MVP` scene combines the imported N64 Day 1 South reference with four texture repaints, ten added architectural renderers, Built-in lighting/shadows, day/dusk presets, a first-person explorer and eight solid preview boundaries. The four repaint families affect 33 source texture slots. Most geometry and many material inputs remain source-derived. No Link mesh, third-person character, NPC schedules, three-day simulation, quests, save system or loaded neighboring district is implemented by this preview.

Source: [BuildCityMvp.cs](../Unity/Assets/Editor/BuildCityMvp.cs), [CityArchitecture.cs](../Unity/Assets/Editor/CityArchitecture.cs), [CityExploration.cs](../Unity/Assets/Scripts/CityExploration.cs), [CityPresentation.cs](../Unity/Assets/Scripts/CityPresentation.cs), [CityThreshold.cs](../Unity/Assets/Scripts/CityThreshold.cs). These files include current uncommitted work; preserve them before regeneration.

## Local locations and controls

- Scene: `Unity/Assets/LocalReference/SouthClockTown/Enhanced/SouthClockTown_MVP.unity`.
- Source scene: `Unity/Assets/LocalReference/SouthClockTown/Generated/SouthClockTown_RomReference.unity`.
- Build: `Unity/Builds/CityMVP/MajorasCityMVP.exe`; retain the entire adjacent data/DLL directory.
- Generator menu: **Majoras > Build Local City MVP**. It requires a stopped, compiled editor and saved scenes. It regenerates disposable enhanced assets from local source data and repaint inputs; it is not an ordinary nonmutating scene-open command.
- Code-defined controls: Begin exploring; WASD/mouse; Shift run; Space jump; Tab source/enhanced; 1/2 day/dusk; R reset; Esc pause; F1 HUD. Physical handling still needs verification in a visible current build.

Opening and running the player belongs to the separate recovery/verification packet. Do not repeatedly launch players to reproduce the reported lockup without first preserving logs and identifying any still-running process.

## Evidence ledger

| Evidence | Saved result | Accurate interpretation |
| --- | --- | --- |
| [Editor smoke](../evidence/city-mvp-editor.json) | `passed: true`, 17:52:54 UTC | 5.28 m flat loop, all 88 steps grounded, zero return error; one source wall stop; pause/reset; A/B material and lighting state |
| [Editor routes](../evidence/city-mvp-routes-editor.json) | `passed: true`, 17:58:02 UTC | 428 route steps via two successive side-ramp segments to Y=2; eight boundary boxes inspected; only lower East stop/proximity exercised |
| [Headless player smoke](../evidence/city-mvp-windows-headless.json) | Nested `passed: true`, 17:59:08 UTC | Targeted hook-driven smoke; does not prove graphics or device input |
| [Headless player routes](../evidence/city-mvp-routes-windows-headless.json) | Nested `passed: true`, 17:59:30 UTC | Same bounded route evidence; target identity must be tied to a specific executable/package on rerun |
| [Windows build](../evidence/city-mvp-build.json) | Succeeded; 0 warnings/errors; 204,332,779 reported bytes; `2026-11-09T00:01:47Z` | Saved successful packaging result, later than the player-check timestamps; no hash demonstrates those tests exercised this exact build |
| [Visible-player check](../evidence/city-mvp-windows.json) | Failed: no Pipeline runtime instance found | No successful interactive Windows verification in this file |

The headless tool envelopes name a generic `C:\Users\cargi` project path and loopback target. This is a provenance gap, not proof the tests hit the wrong process. Next verification must include runtime PID, executable path, active scene, source/dirty-state fingerprint and build hash. Inspect nested `passed`/failures, not only transport success.

The tested ascent is a route through two successive side ramps on the way to the north upper level. It is not proof that every side route, the closed Day 1 tower stairway, roof, platform or all eight exits work. The saved route script also reports obsolete API warnings in its editor diagnostic envelope; these are separate from the build report's zero warnings.

## Captures and unresolved launch

Surviving local captures: `local-reference/south-clock-town/mvp-day.png`, `mvp-dusk.png`, `mvp-reference.png`, `mvp-tower-detail.png` and `mvp-menu.png`. Earlier subjective scores and corrections are retained in [ITERATION_LOG.md](ITERATION_LOG.md); they are not owner approval. This handoff checked their presence, not a new render comparison.

Several graphics launch logs remain in that folder. The interactive log ends during engine/subsystem initialization; the proxy graphics-control log reaches graphics-device creation. The lockup cause is unknown. Keep the original logs and machine-event timestamps, run one bounded comparison at a time, and do not infer a cause or claim the later executable is safe/verified from a no-graphics test.

## Next acceptance packet

1. Preserve the dirty checkpoint and local input/build/log inventory.
2. Confirm exact editor/player identity and a stable visible launch, with one target process at a time.
3. Capture a fresh same-build smoke/route result and actual visible keyboard/mouse exploration.
4. Exercise every enabled exit approach and intended platform route separately; retain closed-state exclusions.
5. Repeat same-camera A/B and moving-camera contact checks. Profile only with named hardware, resolution and preset; frame count divided by recorded simulation time is not an FPS measurement.
6. Record remaining defects, artifact hashes and owner visual decision independently.
