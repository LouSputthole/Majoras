# Asset pipeline: source to review viewer to Unity

Status: proposed production contract, 2026-11-08. The target is an authored 3D remake; the current Three.js work is a thin scene reviewer. This document specifies future assets and gates, not completed character art or a working Unity game. Unity 6000.5.11f1 now generates the JSON proxy, passes transform/material/collision inspection, runs in Play Mode, and produces a Windows development build that passed a local batch-mode smoke test. Real GLB/FBX, skinning, animation and production-material import remain unverified; see `../evidence/environment.json`.

Read [ART_DIRECTION.md](ART_DIRECTION.md) for visual constraints and the prioritized South Clock Town backlog. Use portable glTF/GLB deliverables for review, editable Blender sources for authorship, and explicitly exported Unity interchange files. A successful image generation or concept review is not a delivered mesh.

## Tool and engine boundary

Blender is the proposed DCC for modeling, UVs, texture bakes, rigging and animation. Record and pin the actual Blender/exporter version after an export spike. Do not assume a tool is installed or that a saved file has been validated.

Use GLB as the compact review package and keep source files independent of Three.js. glTF defines runtime asset exchange; custom Blender node networks, control rigs and engine-specific shaders need baking or separate reconstruction. The [Blender 3.6 glTF manual](https://docs.blender.org/manual/en/3.6/addons/import_export/scene_gltf2.html) is a versioned reference for supported material/animation concepts, not a declaration of the installed tool version; use the matching manual when pinning the actual exporter.

Unity is the intended production target, with render pipeline and editor/package versions to be recorded before final materials are authored. Start by evaluating URP for the stylized desktop slice; that is a proposal, not a selected or configured render pipeline. The Three.js viewer cannot determine Unity lighting, physics, navigation, shader compilation or animation-controller behavior.

The first Unity calibration spike compares pinned glTFast import against explicit FBX plus texture exports from the same Blender source. Prefer the canonical GLB for both consumers if skin, clips, materials and reimport pass; use generated FBX for Unity if that route proves more reliable, with the decision recorded before final rigging. Unity documents [FBX as its primary supported model format](https://docs.unity.com/en-us/engine/6000.6/manual/assets-and-media/asset-types/models/importing/importing-model-files); do not rely on installed Blender for automatic `.blend` conversion. Pin any selected glTF importer and test its actual supported extensions. GLB review success is not proof of Unity import support. Keep both exports derived from one editable source.

## Repository layout and retention

The paths below are proposed destinations to create as each asset arrives, not a claim that their contents exist:

```text
art/
  catalog.json                     stable asset metadata and status
  sources/environment/<asset>/     editable .blend, texture source, bake presets
  sources/characters/link_young/   sculpt, low poly, rig, animation sources
  textures/<asset>/                lossless authored maps with channel metadata
  exports/review/<asset>/          GLB plus validation report and export manifest
  exports/unity/<asset>/           FBX plus textures and export manifest
  reference-index.json             labels, origin and local reference pointers
evidence/art/<asset>/<revision>/  screenshots, comparisons, measurements, reports
Unity/Assets/Art/                  approved imported production copies
Unity/Assets/Prefabs/              scene/runtime assembly, colliders and sockets
```

Keep the current review app's public assets as derived copies, with catalog IDs and source revisions. The catalog and scene manifest must reference the same asset ID; the manifest owns placement, the catalog owns provenance/export metadata. Do not rename existing manifest IDs without updating all references atomically.

Do not commit local game dumps, ROMs, extracted game meshes/textures/audio, account information, or extraction tools' bulk output. Use a user-owned local reference directory outside tracked source, record a non-sensitive label/hash where appropriate, and keep `.gitignore` aligned before acquiring material. The supplied screenshot is a reference artifact; its exact origin and release remain to be recorded. Do not imply that access to reference material grants distribution rights to a finished remake.

Use lossless source images and explicit exports. Pin sources before generation/re-export so an export can be traced to a specific revision. Large `.blend`, high-resolution texture, sculpt and animation files will eventually require a deliberate Git LFS or external artifact-storage decision; do not silently add large binary history during groundwork.

## Asset identity and provenance

Use durable, engine-neutral IDs such as `sct.env.clock_tower`, `sct.prop.banner_set`, `chr.link.young`, and `chr.link.young.anim.run`. These are proposed examples; reconcile them with the actual scene manifest before adding the catalog. Human-readable filenames may evolve while IDs stay stable.

The review scene's initial ID contract is below. Scene-instance IDs and asset-family IDs are related but distinct: both market-stall instances may eventually share one authored asset. All current proxy placements are unmeasured; stable naming does not establish accurate geography.

| Scene ID | Asset relationship |
|---|---|
| `scene.south_clock_town` | District assembly root; not a single flattened mesh. |
| `sct.env.clock_tower` | Clock-tower assembly, with separately pivoted clock parts later. |
| `sct.env.perimeter_west`, `sct.env.perimeter_east`, `sct.env.perimeter_north`, `sct.env.perimeter_south` | Boundary/facade instances replaced without changing district identity. |
| `sct.prop.market_stall_west`, `sct.prop.market_stall_east` | Market-family instances with explicit placement and support bounds. |
| `sct.prop.construction_platform` | Scaffold/worksite assembly; later components retain attachment relationships. |
| `sct.prop.bunting` | Banner-family assembly with per-end attachment anchors. |
| `marker.spawn_south`, `marker.exit_west`, `marker.exit_east`, `marker.exit_north`, `marker.exit_south` | Logical reference markers, not proof of implemented transitions or exact original thresholds. |
| `chr.link.young` | Stable hero asset ID reserved for authored character work; any proxy is labeled separately. |

Suggested names: `sct_env_clock_tower_lod0.glb`, `chr_link_young_rig.fbx`, `chr_link_young_basecolor.png`, `chr_link_young_normal.png`. Use ASCII lowercase and underscores in files. Avoid names such as `final_final2`. Store the revision in version control/export metadata rather than editing every scene reference for every export.

Every catalog record must contain:

| Field | Required meaning |
|---|---|
| `id`, `family`, `label` | Stable identity and visual grouping. |
| `state` | `planned`, `reference_ready`, `blockout`, `concept_ready`, `mesh_ready`, `rig_ready`, `review_validated`, `unity_validated`, or `accepted`. Use only applicable states, with independent gate flags below. |
| `author`, `createdAt`, `updatedAt`, `sourceRevision` | Actual origin and edit history; do not invent a person or approval. |
| `provenance[]` | Source URL/local reference label, creator/provider, date, source type, license/usage notes, selected game release when applicable. |
| `generation` | If generated: tool/model when disclosed, prompt, input references, output hash and manual corrections. Set pending/absent when generation failed; do not create a fictional output record. |
| `fidelityNotes`, `unknowns[]` | Reference-backed choices, inferred hidden parts and intentional departures. |
| `source`, `reviewExport`, `unityExport` | Relative paths, SHA-256 hashes, tool versions and export preset IDs. |
| `dependencies[]` | Other asset/material/rig IDs and their required revision or compatible schema version. |
| `boundsMeters`, `pivot`, `forward`, `up`, `sockets[]` | Explicit scale and placement contract, including source-versus-export coordinate conventions. |
| `materials[]`, `textures[]`, `lods[]`, `animations[]` | Slot IDs, channel/color-space metadata, exported triangle counts, texture dimensions, clip names/durations, rig compatibility. |
| `gates` | Separate reference, visual, mesh, deformation, GLB validation, browser, Unity import/runtime, collision and performance results. Each has status, evidence path, tool/version and date. |
| `review` | Reviewer and actual decision/date if supplied; otherwise `pending`. |

An asset can pass GLB validation and fail art review. A concept can pass visual review while no mesh exists. Do not collapse these facts into a single `done` boolean. A failed export returns to the relevant gate without erasing earlier evidence.

## Scale, transforms, pivots and attachment

Use meters as the common authored scale and include a one-meter calibration cube and a labeled hero-height guide in the export test scene. The [glTF specification](https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html#coordinate-system-and-units) defines a right-handed coordinate system and meters; Unity model scale must be checked with that calibration object, not corrected by arbitrary scene scaling. Unity's [model preparation guidance](https://docs.unity.cn/2022.3/Documentation/Manual/models-preparing.html) describes its one-unit-per-meter expectation.

Blender source convention: Z up, character faces negative Y; glTF export convention: Y up, inspect actual forward after conversion. Unity assembly convention: Y up, character prefab faces positive Z. Let pinned exporters/importers handle coordinate conversion and verify with an asymmetrical forward marker. Do not manually negate a scale axis to repair a flipped character. Record a deliberate prefab rotation if the tested interchange route needs it.

Apply source mesh rotation/scale before rigging, baking and final export. Keep the authored armature rest pose and unit scale stable after animation starts. Repeatedly applying transforms to a rigged asset can invalidate bind poses and clips. Use identity scene-instance scale unless an explicit modular variation requires otherwise.

Grounded props use a base-center pivot on their support plane; modular wall pieces use a consistent grid corner; doors use hinge pivots; clock hands use the clock axle; Link uses a stable root at ground level. Store sockets for sword grip, shield, sheath and any effect anchors. Use named attachment transforms rather than arbitrary offsets copied into each scene.

Each placeable family needs support bounds and optional simplified collision geometry. Validate contact points after rotation and scale, including all four crate feet, scaffold posts, stair landings and building bases. Small intentional embedding into uneven ground is acceptable when recorded. Floating bases and intersecting walkable surfaces fail review. Contact shadows supplement geometry and do not establish collision.

For ground layering, make paths part of the ground mesh or use an explicitly supported decal route. Avoid coplanar duplicate planes. Give banners, signs and trim real thickness or tested geometric offset. Establish the camera clipping range from scene scale and check depth stability at maximum review distance before increasing tiny offsets blindly.

## Young Link production sequence

| Stage | Work | Exit evidence |
|---|---|---|
| 1. Reference baseline | Choose release/state; collect labeled body, face, hair, costume and equipment views; note unknowns and handedness. | Board with sources, proportions as ratios, selected outfit/equipment; no mesh claim. |
| 2. Concept/model sheet | Front/side/back, three-quarter, expressions, material swatches and equipment; repair inconsistent views. | Coherent sheet assessed against reference. A sheet remains a 2D image. Current state: pending. |
| 3. Proportion sculpt | Original full 3D body, tunic, hat, hair and gear blockout; compare orthographic views and turntable. | Actual `.blend`/mesh plus front/profile/back and silhouette renders; camera-height test. |
| 4. Refined sculpt | Primary/secondary forms, tunic thickness and folds, grouped hair, equipment structure, expressive face. | Turntable and close-ups with proportion overlay; detail does not change approved age/silhouette. |
| 5. Retopology | Original runtime mesh, planned face/joint loops, garment thickness, hair and gear separation, stable triangulation. | Wireframe, exported counts, no unintended non-manifold/open body holes, normals correct. |
| 6. UV and bake | UV0 layout, deliberate mirrored/shared regions, bake cage, normal/AO and texture painting; separate any unique facial detail. | UV checker at target density, seam checks, clean bake without ray misses or intersecting-shell artifacts. |
| 7. Rig and skin | Root/pelvis chain, spine/head, limbs/fingers as needed, facial/eye controls, gear sockets, cap/hair secondary controls. Separate animator control rig from deform/export rig. | Named deformation skeleton, clean bind pose, normalized weights, extreme-pose sheet; no disconnected influences. |
| 8. Animation | Idle, walk, run, start/stop, turning, jump/land and reference-led item pose. Bake procedural/control motion for export. | Loop boundaries, foot contact, stride distance, root-motion/in-place policy and clip events documented. |
| 9. LOD and materials | Reduce secondary shapes while retaining head/ears/cap silhouette; preserve skeleton/material contract. | Fixed-camera comparison at switch distances, no large shading/pop or equipment disappearance. |
| 10. Runtime integration | GLB reviewer, Unity FBX import, prefab, sockets, colliders, camera framing and animation controller. | Separate browser and Unity recordings/checks, actual hardware/frame timing and unresolved issue list. |

Use deformation poses before final painting: raised arms, elbow/knee bends, crouch, running stride, head turn, wide blink and facial expressions. Check tunic/legs, ears/hair, cap/shoulders and sword/shield intersections. Do not solve every intersection by excessive body deletion or hiding missing forms from the default camera.

Start with a Generic rig for a faithful custom proportion/animation spike; evaluate Humanoid only if retargeting is a concrete need and the actual avatar mapping passes. This is a production choice to validate, not a claim that either mode already works. Keep root motion explicit per clip. The viewer's orbit/turntable and a running animation are different tests.

## Provisional authoring budgets

These are starting caps for a desktop vertical slice to guide scope, not measurements, proven platform limits, or promises of frame rate. Count triangles after export, because UV seams, normal splits and modifiers change runtime data. Profile an integrated representative scene before locking budgets; increase or decrease them based on evidence.

| Family | LOD0 starting range | Material/texture starting envelope | Reduction direction |
|---|---|---|---|
| Young Link, body/clothes/hair excluding removable equipment | 30k–55k triangles | 2–4 material slots; 2K primary set, optional separate 1K face/eyes | LOD1 around 50–60% of LOD0, LOD2 around 20–30%; retain facial/head/hat silhouette. |
| Link equipment set | 4k–12k triangles | 1–2 slots; 1K–2K shared set | Keep distinctive shield/sword outline; simplify rear-facing microdetail. |
| Clock tower including clock assembly | 35k–80k triangles | 3–5 slots; 2K trim/tile sets plus a unique clock graphic if needed | Preserve clock rim and skyline; simplify hidden back detail. |
| Typical facade module | 2k–12k triangles | 1–3 slots; shared 1K–2K tile/trim textures | Instance reusable parts without erasing deliberate facade variation. |
| Scaffold/market cluster | 5k–20k triangles | 2–4 shared slots; shared 1K–2K sets | Simplify internal beams/thatch; preserve posts, joints and cloth shape. |
| Small prop | 150–3k triangles | Prefer 1 shared slot and atlas/trim reuse | Cull tiny detail before flattening important silhouette. |

Begin Link's deform skeleton around 60–90 bones including limited secondary/facial bones, with four influences per vertex as an interoperability target. This is provisional: confirm expressive needs and exporter/runtime support before freezing the rig. More bones and influences require measured justification, not automatic rejection or acceptance.

Start environment texel density around 256 pixels/meter on ordinary surfaces and 512 pixels/meter on close-read hero graphics, adjusted after gameplay-camera inspection. Tiny props should share sheets where practical. The target hardware, resolution, memory budget and frame-time cap still need definition. Do not report browser FPS as the Unity production benchmark or add all texture files' compressed sizes and call that GPU residency.

## Materials, textures and portable shading

Author base color in sRGB; treat normal, metallic, roughness, occlusion and masks as data with explicit color-space settings. Use a metallic/roughness glTF review workflow. For packed glTF channels, document occlusion in R when shared, roughness in G, metallic in B; retain original separate maps so Unity shaders can be repacked deterministically. Never assume Unity URP consumes the glTF pack directly: a selected shader's metallic/smoothness and occlusion inputs must be matched deliberately.

Record normal-map convention, texture orientation and tangent basis in export metadata; test a strongly directional calibration normal in both runtimes. Bake maps with the final triangulation and use compatible imported tangents. A green-channel inversion is acceptable only when demonstrated by the chosen import/shader path, not as a blanket fix.

Keep the portable baseline to ordinary opaque PBR materials and limited alpha-cutout cloth/hair where needed. Any custom toon ramp, outlines, wind vertex animation, eye shader, animated UVs or emissive enhancement needs a separate viewer and Unity implementation with parity evidence. Bake supported textures from complex Blender nodes; do not assume a procedural node graph survives GLB export.

Avoid transparent blended surfaces for solid objects. Test cutout edges and mipmaps at distance for shimmer/halos. Use shared material instances where the art permits; unique colors do not automatically require a new shader. Keep material IDs stable across LODs and revisions.

## Export spike before production scale-up

Create a small deliberately diagnostic asset pack first: one asymmetrical meter cube, a beveled stone block with UV checker and normal map, a painted cutout banner, a simple skinned limb with two named clips, and two test attachment sockets. Export both GLB and FBX from the same source revision.

Record the exporter preset, unit/axis settings, modifier policy, texture embedding/extraction, tangent export, animation sampling, clip naming, root-motion policy and file hashes. Reimport into a new empty Blender file and compare scale, orientation, materials, skinning, clip lengths and sockets. Then inspect GLB in the viewer. Finally inspect FBX plus rebuilt materials in Unity once the editor is runnable. A rigged hero should not be the first experiment with this pipeline.

For GLB, include only intended export collections, meshes, deform bones and baked clips. Exclude cameras, concept planes, hidden reference geometry and unused high-poly sculpt. Keep source and exported LOD naming explicit. Do not export all animation actions accidentally or rely on an unnamed default take.

## Validation gates and evidence

| Gate | Required checks | Evidence and failure handling |
|---|---|---|
| Source audit | Editable source opens; provenance complete; correct selected reference; no private dumps in tracked package | Catalog record and source revision. Missing provenance remains unresolved. |
| Geometry | Expected bounds/orientation/pivots; clean normals; no NaN/invalid transforms; final triangle count; intended open surfaces; attachment contact | Measurement report, wireframe/normal view and calibration scene. Repair in source. |
| Texture/material | Texture references complete, channel/color-space assignments correct, UV padding/mips/seams checked, calibrated normal test | Channel renders and material manifest. A pretty lit render alone cannot prove channels. |
| Rig/animation | Bind pose, hierarchy, weights, clips, loops, root motion, socket alignment, extreme poses | Pose sheet plus actual exported animation playback. Repair source or pinned export preset. |
| GLB format | Run pinned Khronos validator; zero errors; review every warning and record exceptions | Validator JSON with asset hash/tool version. Passing validates the file, not its fidelity or artistic quality. |
| Browser review | Clean load, correct scale/materials, no missing files/console errors, working orbit/fixed views, clips and relevant sockets | Browser/version, screenshot/video and console capture. Mark shader limitations explicitly. |
| Unity import | Correct scale, rig/rest pose, material rebuild, normals, texture compression, clips, prefab assembly | Editor/package versions, import presets and scene evidence; pending until actual execution. |
| Runtime integration | Collision/contact, camera framing, animation transitions/equipment fit, lighting presets, LOD switches, no z-fighting | Recorded walkthrough and defect log in the intended runtime. |
| Performance | Representative populated scene on named hardware/resolution; CPU/GPU frame times, memory and draw/triangle counts | Profiler capture and conditions. No numerical performance claim before measurement. |

The official [Khronos glTF Validator](https://github.com/KhronosGroup/glTF-Validator) checks format/schema, references, buffer values and animation data and emits a JSON report. It cannot check art accuracy, foot sliding, believable rigging, scale relative to the intended world, or Unity behavior. Keep the validator report beside the exact exported asset revision.

Run review captures with identical camera transforms and labeled lighting. At minimum inspect tower approach, square reverse view, each district threshold, scaffold access, and Link front/profile/rear/three-quarter. Add a moving-camera pass near layered surfaces: static frames can conceal depth shimmer and floating props.

## Handoff package and next executable order

For each accepted asset, hand off editable source, export manifest and hashes, GLB, Unity FBX/textures, catalog record, material channel notes, bounds/pivots/sockets, collision guidance, animation list, LOD policy, validation evidence and known limitations. Include tool versions and a deterministic re-export recipe. Do not deliver only a screenshot or an engine-specific scene with missing sources.

The next work order is: complete the chosen-release reference board and missing layout views; lock stable IDs and placement contracts; run the diagnostic export spike; replace tower/ground/perimeter placeholders; create Link's coherent concept sheet and actual proportion sculpt; test his rig early; then develop the other P1 families, materials and smaller dressing. Unity import and runtime validation become a required gate as soon as licensing permits editor execution. Until that happens, identify every browser-only result as browser evidence.
