# Proxy verification

`VerifyProxy.cs` is an inspection script for Unity Pipeline, kept outside `Assets` so it is not compiled into the game. On 2026-11-08 it compiled and passed against Unity 6000.5.11f1 after initial generation, saving/reopening and regeneration. Reports are in `../../evidence/unity-proxy-*.json`.

After generating the proxy and confirming a ready editor connection, run from the repository root:

```powershell
unity command script eval --project-path "$PWD/Unity" --format json -- --file Tools/VerifyProxy.cs --dry-run true
unity command script eval --project-path "$PWD/Unity" --format json -- --file Tools/VerifyProxy.cs
```

Inspect the returned `passed` value and `failures` list. CLI execution success alone does not mean the inspection passed. Save and reopen the generated scene, then repeat the inspection to check persistence.

The inspection checks stable IDs, transforms, material assignments and values, render/collision envelopes, ground contact and a camera. It does not generate or change scene assets. It synchronizes physics transforms before reading collision bounds and raycasting.

A passing report covers the authored **unmeasured proxy contract** only. Play Mode progression, visible rendering, a Windows build, measured layout fidelity and player movement require separate evidence.

`CaptureProxyPlayer.cs` was executed against the local instrumented Windows development build. It renders the main camera off-screen and saves `evidence/unity-proxy-windows.png`, restoring the camera and render-target state afterward. Use `script eval --file <absolute script path>` with `--runtime-path <repo>/Unity/Builds/Windows`; the runtime path must be a directory. The script requires the local build layout and rejects Editor execution. It is needed because this Pipeline version exposes its capture commands only in the Editor. See `../README.md` for the smoke-test scope and temporary instrumentation settings.
