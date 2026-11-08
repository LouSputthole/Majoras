# Local N64 reference conversion

This pipeline imports the user's existing US N64 ROM assets as a local reconstruction reference. It does not generate modern replacement art or port gameplay. ROMs, extracted assets, Blender files, generated Unity assets and reference screenshots stay in ignored directories.

Verified sources: zeldaret/mm `3be2e24123f5e71fd13f939ecc0f9d7f73252639`; Fast-64/fast64 `b13010949567e4a2bf44ba3ccc18b06427243153`. See [scene/version research](../../docs/ROM_REFERENCE.md). Native Blender 5.2.3 and Unity 6000.5.11f1 were used locally; Fast64's README declares support through 5.1.2, so this successful import is specific evidence, not a general compatibility promise.

## Prepared local layout

```
local-reference/
  .venv/                    Python with zeldaret/mm requirements
  roms/mm-us.z64             MD5 2a0a8acb61538235bc1094d297fb6556
  mm-decomp/                pinned source and extracted/n64-us/
  fast64/                   pinned Blender addon, with local matrix-pop fix
  south-clock-town/         editable .blend, import report, Unity captures
Unity/Assets/LocalReference/SouthClockTown/
  reference.json            explicit meshes, UVs, colors, material state
  Textures/                 local extracted PNGs
  Generated/                Unity assets and reference scene
```

## Extraction

Place the verified ROM at `local-reference/mm-decomp/baseroms/n64-us/baserom.z64`. From `local-reference/mm-decomp`, using the isolated Python environment:

```powershell
& '..\.venv\Scripts\python.exe' tools/decompress_baserom.py -v n64-us
& '..\.venv\Scripts\python.exe' tools/extract_baserom.py baseroms/n64-us/baserom-decompressed.z64 extracted/n64-us/baserom -v n64-us
$zapd = '.\tools\ZAPD\native-msvc\build\Release\ZAPDNative.exe'
& $zapd e -i assets/xml/scenes/Z2_CLOCKTOWER/Z2_CLOCKTOWER.xml -b extracted/n64-us/baserom -o extracted/n64-us/assets/scenes/Z2_CLOCKTOWER -osf extracted/n64-us/assets/scenes/Z2_CLOCKTOWER -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
foreach ($bank in @('object_obj_tokeidai','object_tokei_turret','object_tokei_step','object_tokei_tobira')) {
    & $zapd e -i "assets/xml/objects/$bank.xml" -b extracted/n64-us/baserom -o "extracted/n64-us/assets/objects/$bank" -osf "extracted/n64-us/assets/objects/$bank" -gsf 1 -rconf tools/ZAPDConfigs/MM/Config.xml --cs-float both -Wno-hardcoded-pointer
    if ($LASTEXITCODE -ne 0) { throw "Extraction failed: $bank" }
}
```

The prepared native extractor passed a synthetic PNG round trip and these five extractions. Its SHA256 is `36cdc50b83a3c3e3c414317f2d65cdc04c0be4125125ebe6b88e1d83122409bd`. Build provenance is local at `local-reference/zapd-build-provenance.json`.

On 2026-11-08 the same `e` command was run for all 22 Clock Town scenes (the five exteriors plus the 17 venue/annex scenes in [FULL_CITY_PLAN.md](../../docs/FULL_CITY_PLAN.md#6-interiors-underground-routes-and-city-annex)), substituting each `Z2_*` name into the scene command above. Output is readable C under `extracted/n64-us/assets/scenes/<SCENE>/` (scene commands, actor/object lists, exit/spawn lists, transition actors, collision) and is the input for the `docs/city/` registers. It stays ignored; only object banks needed by an assembly are extracted separately.

For native rebuilding, copy `zapd-windows/CMakeLists.txt` into the pinned decomp's `tools/ZAPD/native-msvc/`, apply `zapd-windows/windows-path-fixes.patch` from the decomp root (check whether already applied), and restore the solution's packages with MSBuild `/t:Restore /p:RestorePackagesConfig=true`. The wrapper uses the restored libpng.static 1.6.37 and zlib.static 1.2.5 sources. Configure with the existing VS18 BuildTools CMake, generator `Visual Studio 18 2026`, x64, then build Release. No WSL is required. Upstream source/licenses remain in the local checkouts; these patches are not a bundled extractor binary.

## Blender and Unity

From the Majoras root:

```powershell
& 'A:\SteamLibrary\steamapps\common\Blender\blender.exe' --background --factory-startup --python-exit-code 1 --python tools/reference/import_clocktown.py
```

The script registers Fast64 only for this process, enables its existing MM branches, imports the base scene header, assembles the documented static Day 1 noon clock/platform/door actors, preserves collision triangles without interpreting them as OOT surfaces, saves a `.blend`, and exports the local Unity interchange. It excludes Fast64's material-library sample object.

Then in the ready Unity Editor use **Majoras > Import Local ROM Reference**, or the Unity CLI's `script eval` with `return Majoras.Editor.ImportRomReference.Generate();`. Save any user-edited scene first; the importer rejects dirty scenes. It writes `Assets/LocalReference/SouthClockTown/Generated/SouthClockTown_RomReference.unity` and returns counts/bounds. Re-import updates only its own numbered generated assets.

Coordinate conversion is Unity `(x,y,z) = N64 (x,y,-z)/100`. The ratio preserves source proportions but remains a provisional physical meter calibration. Winding and normals are converted once. The original room collision contains 508 triangles; actor collision/interaction semantics require separate work.

## Known limits

- Static base-header reference: no Link/NPCs, moon, skybox, smoke, schedules, input or game simulation.
- Fast64 MM object-bank/surface enums are incomplete. Gameplay metadata is bypassed; it is not replaced with OOT meanings.
- The source importer needed matrix-stack pop handling and one scale conversion instead of repeated scaling during matrix multiplication. Camera-facing billboards are retained at a fixed import orientation.
- The Unity combiner preserves texture/color inputs and primitive LOD fraction; lighting, hardware filtering, per-pixel LOD, fog and runtime animated material state remain approximate.
- A rendered reference does not establish final art quality, camera matching, traversal, collision gameplay or performance.
