# Proxy verification

`VerifyProxy.cs` is an inspection script for Unity Pipeline, kept outside `Assets` so it is not compiled into the game. It has been source-reviewed but has not yet run in Unity; the editor patch is downloaded and installation is awaiting Windows elevation.

After generating the proxy and confirming a ready editor connection, run from the repository root:

```powershell
unity command script eval --project-path "$PWD/Unity" --format json -- --file Tools/VerifyProxy.cs --dry-run true
unity command script eval --project-path "$PWD/Unity" --format json -- --file Tools/VerifyProxy.cs
```

Inspect the returned `passed` value and `failures` list. CLI execution success alone does not mean the inspection passed. Save and reopen the generated scene, then repeat the inspection to check persistence.

The inspection checks stable IDs, transforms, material assignments and values, render/collision envelopes, ground contact and a camera. It does not generate or change scene assets. It synchronizes physics transforms before reading collision bounds and raycasting.

A passing report covers the authored **unmeasured proxy contract** only. Play Mode progression, visible rendering, a Windows build, measured layout fidelity and player movement require separate evidence.
