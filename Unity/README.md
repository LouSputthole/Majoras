# Unity seed — not yet compiled

This directory is the production-engine starting point, not a runnable remake. Installed Unity `6000.5.0f1` failed activation during project initialization, so no Unity compilation, import, play-mode or player-build result exists.

1. Activate a valid editor license in Unity Hub yourself.
2. Add/open this `Unity` folder with the provisional installed version, or record a deliberate supported-version change first.
3. Let Unity generate its settings and `.meta` files; check the console before proceeding.
4. Choose **Majoras → Generate South Clock Town Proxy**. The tool offers to save any modified scene before replacing the active scene.
5. It reads `../shared/scenes/south-clock-town.blockout.json` relative to this project and creates `Assets/Scenes/SouthClockTown_Proxy.unity` plus generated materials.
6. Inspect scale, orientation, IDs, palette and collider envelopes, then record actual results. Add URP/input and a player only through the planned toolchain gate.

The importer uses Unity coordinates directly. Do not reflect Z again. The browser adapter reflects Z once when producing right-handed Three.js/glTF geometry. Real GLB imports need a pinned importer and the calibration spike; Unity does not automatically load this viewer's GLB as a configured game scene.

Shelters and bunting are simple **envelopes** in this Unity seed, while the browser shows decorative proxy components. Neither representation establishes finished art/collision. Cylinder envelope colliders are explicitly boxes instead of Unity's default capsule collider. Built-in Standard roughness maps to `_Glossiness`; future URP materials map to `_Smoothness`.

The generated proxy is disposable. Final measured scenes/prefabs must live outside the ignored generated filenames. Commit stable `.meta` files and the package lock after the first successful import, together with the recorded editor version. The menu importer is editor-only; `ProxyAssetIdentity` stores metadata, not gameplay state.
