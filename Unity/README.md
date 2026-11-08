# Unity seed — scripts compiled; scene checks pending

This directory is the production-engine starting point, not a runnable remake. On 2026-11-08, Unity CLI `1.0.0-beta.17` confirmed an active Unity Personal license. The project opened in `6000.5.0f1` and compiled both seed assemblies without C# compiler errors. The earlier license failure is resolved.

Pipeline `0.10.0-exp.1` is installed. The current blocker is `STATUS_PIPELINE_LOAD_PENDING`: the CLI specifically identifies Unity `6000.5.0f1` as incompatible with starting the Pipeline server and requires `6000.5.3f1` or later. Waiting or refocusing the editor does not fix this version defect. The proposed side-by-side update is `6000.5.11f1`, pending user approval. This patch proposal does not establish a long-term support choice.

1. After approval, install the compatible patch, preserve the existing editors, and reopen this `Unity` project with the chosen version. Record its exact revision.
2. Run `unity status --until-ready --project-path <repo>/Unity` and confirm a ready connection. Check compilation and console errors again after the version change.
3. Invoke `Majoras.Editor.ImportSouthClockTown.Generate()` through the connected editor, or choose **Majoras → Generate South Clock Town Proxy**. The tool offers to save any modified scene before replacing the active scene.
4. The importer reads `../shared/scenes/south-clock-town.blockout.json` relative to this project and creates `Assets/Scenes/SouthClockTown_Proxy.unity` plus generated materials.
5. Inspect scale, orientation, IDs, palette and collider envelopes, then record actual results. Add URP/input and a player only through the planned toolchain gate.
6. Verify play mode with advancing simulation time and an inspected capture, then produce and launch a Windows smoke build. None of these scene/runtime/build checks has run yet.

The CLI executable in this local session is `C:\Users\cargi\AppData\Local\Unity\bin\unity.exe`; use the full path if the terminal has not picked up the installer PATH change. Target this project explicitly when multiple editors are open. Sanitized results are in `../evidence/environment.json`; raw logs remain local and ignored.

The importer uses Unity coordinates directly. Do not reflect Z again. The browser adapter reflects Z once when producing right-handed Three.js/glTF geometry. Real GLB imports need a pinned importer and the calibration spike; Unity does not automatically load this viewer's GLB as a configured game scene.

Shelters and bunting are simple **envelopes** in this Unity seed, while the browser shows decorative proxy components. Neither representation establishes finished art/collision. Cylinder envelope colliders are explicitly boxes instead of Unity's default capsule collider. Built-in Standard roughness maps to `_Glossiness`; future URP materials map to `_Smoothness`.

The generated proxy is disposable. Final measured scenes/prefabs must live outside the ignored generated filenames. Unity-generated source `.meta` files, project settings and the package lock are tracked for reproducibility. Their presence proves project initialization, not successful proxy generation or runtime validation. The menu importer is editor-only; `ProxyAssetIdentity` stores metadata, not gameplay state.
