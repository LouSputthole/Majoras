# Art direction: South Clock Town and young Link

Status: production proposal, 2026-11-08. This document defines the intended result and review criteria. It does not certify that finished environment assets, a Link mesh, animation, or a Unity scene exist. The browser prototype is a composition and scale reviewer. A concept-generation attempt did not produce usable art; the Link concept sheet remains pending.

## Creative target

Rebuild the recognizable shapes, spatial compression, and strange civic character of South Clock Town with modern authored geometry, readable materials, expressive characters, and controlled lighting. The town should feel inhabited and festive while something is wrong above it. Young Link should remain an expressive, small hero within that oversized world.

The target is stylized modern fidelity: retain the original design decisions while supplying the depth, articulation, and surface treatment that the original technical limits could not show. Photographic masonry, an adult warrior, a generic medieval market, and a uniformly cheerful village would all miss the target. More polygons and texture resolution are tools; identity is the acceptance criterion.

## Reference hierarchy and unresolved evidence

1. Use the [South Clock Town screenshot](https://www.zeldadungeon.net/wp-content/uploads/2014/11/South_Clock_Town.png) as the present composition reference (local ignored copy: `docs/south-clock-town-reference.png`). Preserve its clock-tower silhouette, ochre stone, teal/red clock graphic, dark masonry recesses, angular parapets, banners, scaffolding, patterned ground, and painted construction objects.
2. Before final layout lock, collect matching in-game views from the chosen original release: a high view, all district thresholds, tower approach, doors, stairs, scaffold access, and ground-level reverse angles. Record camera, day, time, location, and release for every capture. The current single image cannot establish a complete map, dimensions, rear elevations, or occluded routes.
3. Working baseline: N64 layout and gameplay, with labeled 3DS imagery available as a visual reference for modern execution. This is an explicit interpretation, not evidence that every current proxy position matches either release. Do not silently average geometry, proportions, facial treatment, equipment, or lighting; document any adopted 3DS treatment. Record the selected baseline in the scene manifest before final modeling.
4. Use official character art and chosen-release screenshots to resolve Link's proportions and outfit. A generated model sheet can explore execution after those constraints exist; it cannot establish canonical anatomy or equipment by itself.
5. Record every deliberate departure in an asset's fidelity notes: what changed, why, which view proves it, and its gameplay consequence. Unsupported detail is a proposed addition, not recovered original detail.

The screenshot demonstrates appearance only. Neither it nor a browser recreation proves exact dimensions, collision, schedules, clock motion, or playable routes.

## Shape language and composition

The clock tower is the district's visual anchor. Preserve its stepped vertical mass, heavy dark stone cap, large circular/octagonal clock assembly, off-center graphic details, and small dark openings. Do not replace it with a familiar European steeple or a finely detailed realistic clock face. Its face must read at street scale before small ornament is added.

Buildings form a bounded civic bowl: thick walls, blunt roof edges, small openings, broad painted and stone planes. Keep the deliberate imbalance between huge civic architecture and human-scale stalls, doors, and workers. Buildings may acquire thickness and convincing joints without becoming neat, identical modular houses.

The major read order is tower and clock, district enclosure and openings, elevated scaffold and route shapes, hero/NPC silhouettes, then signs and market detail. In a thumbnail and a flat gray render, the tower must still dominate. Props must not fill every negative space or hide the tower approach and thresholds.

Keep asymmetry purposeful. Posts can lean slightly, wood can bow, and cloth can sag. Structural contact points must remain plausible; warped geometry is not permission for disconnected beams, floating crates, or impossible stairs. Primary building edges remain legible at gameplay distance.

Use three detail scales: large silhouette and mass, medium block/beam/panel rhythm, then restrained grain/chips/stitches. Do not cover each surface in equal-frequency noise. The original's bold painted symbols and color planes should survive a low-resolution thumbnail.

## Color script

These sRGB hex values are proposed swatches for authored assets, not sampled calibration values or exposure targets. Tune them together under the final renderer. Material base color and displayed lighting color are different quantities.

| Role | Starting swatches | Use and limit |
|---|---|---|
| Civic stone | `#A79761`, `#776A43`, `#46452F` | Warm ochre body, muted block variation, deep green-brown seams; avoid neutral gray castles. |
| Weathered timber | `#6F4C2C`, `#A17643`, `#3D3026` | Warm beams and scaffold; medium-scale grain, readable end cuts and joints. |
| Painted vermilion | `#A84B35`, `#D06B48` | Clock motifs, awnings, decorative panels; localized warm emphasis. |
| Oxidized teal | `#377F78`, `#65A8A1` | Clock face and selected painted accents; preserve contrast with ochre. |
| Festival cloth | `#E5D595`, `#80AC78`, `#7F729A` | Faded cream, green, violet banner families; separated symbols and edge binding. |
| Ground | `#B68C53`, `#C6B284`, `#744F39` | Warm path and tiled/patterned areas; avoid a featureless grass plane. |
| Link | `#3F7C3A`, `#79A64C`, `#D4B65D`, `#A56E48` | Green tunic/hat, golden hair, warm leather; final tones depend on selected references. |
| Dusk atmosphere | `#4D5578`, `#B36A5C` | Cool ambient and restrained dusty warm horizon; not a global purple color wash. |

Maintain material separation in grayscale: Link's face/hair, tunic, belt, boots, and shield silhouette must not merge. Stone should not saturate enough to compete with character color accents. Keep blacks colored and readable instead of crushing all recesses.

## Surface and material treatment

Use a restrained PBR foundation with authored, painted color design. Large color shifts and hand-shaped wear carry style; subtle roughness and normal detail provide depth. Avoid baked directional sunlight in base color, since time of day will move. Local cavity color and designed shading are acceptable when they do not contradict moving illumination.

| Family | Surface direction | Geometry versus texture |
|---|---|---|
| Tower and walls | Broad block groups, uneven ochre hues, dark joints, selective moss in moisture paths | Model the outer silhouette, cap, reveals, parapets and important seams; bake fine pits. |
| Clock assembly | Graphic teal, cream, rust red and dark linework; bold readable motif | Model rim thickness, hands/parts that must move, face relief affecting silhouette; paint fine marks. |
| Timber/scaffold | Cut ends, grain direction, worn contact edges, muted red paint remnants | Real beam joints, supports and walkable decking; texture shallow grain. |
| Plaster and paint | Broad matte fields, edge wear, occasional strokes | Keep graphic borders clean; avoid random grunge over symbols. |
| Cloth/thatch | Soft rough cloth; chunky grouped straw | Model sag and silhouette; alpha cutout only where needed, with tested shadows and sorting. |
| Link fabric/skin | Broad green folds, soft painted skin response, restrained specular | Cloth thickness at cuffs/hem; expression from mesh/rig, not a flat face image. |
| Metal/equipment | Worn stylized steel and painted wood; limited highlights | Edge silhouette and fit are modeled; do not turn every buckle into polished chrome. |

Normal intensity must preserve the smooth, broad planes of the style. Use roughness variation to explain a material, not to create wet-looking stone or oily skin. Keep texture channels and Unity remapping in [the pipeline](ASSET_PIPELINE.md).

## Lighting and atmosphere

First establish a neutral daylight review rig with visible contact shadows. Match the supplied screenshot's bright sky and warm civic stone as a visual study without baking its exact sky tint into every material. Then evaluate dawn, afternoon, dusk, and night. Each state needs a recorded exposure, sun direction, ambient color, fog settings, and renderer/version.

Threat emerges from composition, shadow shape, the moon, the clock, and changing atmosphere. Use a restrained cool/warm split, long afternoon shadows, isolated warm lamps at night, and increasingly oppressive sky framing where supported by the chosen scene state. Do not use a perpetual orange cinematic filter or dense fog that hides layout errors.

The moon is a major composition and identity asset, not a generic sphere. Its face, apparent scale, placement and visibility need their own references and day/time rules. Early placeholders must be labeled. Do not use a huge moon to compensate for missing architecture.

Distinguish a static art-lighting preset from implemented time progression. The first scene may offer review presets while gameplay clock, NPC schedules, weather and the three-day cycle remain later systems.

## Young Link: identity constraints

Create a genuinely modeled young Link: childlike head-to-body relationship, large expressive eyes, pointed ears, readable blond locks and long green cap, short tunic, compact limbs, and oversized but controlled hands/feet according to the selected baseline. Keep the silhouette agile and approachable. Avoid adult proportions, heavily muscular anatomy, gritty stubble, fashion-model facial realism, or photoreal skin pores.

Do not invent precise canonical height or head count from this screenshot. Establish a provisional metric stand-in for camera tests, then measure proportions as ratios against orthographic reference. Preserve the approved ratios through sculpt, retopology, rigging and LODs. Model the face for three-quarter dialogue and profile, not just the attractive front view.

Equipment is a separate reference decision. Record selected sword, shield, sheath, belt, strap routing, and handedness for the chosen release/state; do not mix convenient equipment from unrelated Zelda games. Test the silhouette with equipment both equipped and stowed. Establish sockets so later item variants do not require rebuilding the body.

Use original meshes and authored textures for the remake deliverables. An enlarged sprite, a textured camera-facing card, a painted render, and a concept/model sheet are image assets; none is a rigged 3D Link model. A stylized blockout is useful for scale but must remain labeled a placeholder until topology, rig, shading, and motion reviews pass.

## Link concept-to-runtime reviews

1. Assemble a labeled reference board for the selected baseline. Record front/side/rear evidence and unknowns before requesting concept variants.
2. Produce a consistent front/side/back model sheet, neutral three-quarter face, expression strip, equipment sheet, and material close-ups. Keep scale bars and landmark guides aligned. Generated images must be corrected for mismatched costume details, impossible straps, changing proportions, and inconsistent views. No usable sheet has been produced yet.
3. Make a simple 3D proportion sculpt and compare its orthographic views to the board and sheet. Turntable review must prove that the face and silhouette work all the way around. This is the earliest point to claim a 3D character blockout.
4. Refine primary anatomy, clothing thickness, hair groups, ears, cap and equipment; approve silhouette before pores, seams, or tiny accessories. Preserve child proportions while adding appealing expression.
5. Retopologize deformation areas, lay out UVs, bake and paint textures, and build rig and expression controls. Follow the explicit mesh/export gates in [ASSET_PIPELINE.md](ASSET_PIPELINE.md).
6. Review idle, walk/run, stop/turn, jump/landing and representative item use from the actual gameplay camera. Watch shoulders, elbows, knees, tunic hem, hair, cap, straps and shield intersections. A beautiful bind pose is insufficient.
7. Compare the same character asset in the neutral asset viewer and licensed Unity target. Render parity, interaction scale and readable motion are separate sign-offs. Until Unity import/runtime checks happen, label them pending.

## Environment production priorities

| Priority | Family / proposed ID | Dependencies | Acceptance before moving on |
|---|---|---|---|
| P0 | Reference/layout, `sct.layout.base` | Chosen release, reference capture set | District silhouette and thresholds match labeled views; unknown routes flagged; no invented map presented as exact. |
| P0 | Scale and hero proxy, `chr.link.young.proxy` | Layout and camera | Child-scale stand-in fits doors, stairs, parapets and scaffold clearances; placeholder visibly labeled. |
| P0 | Clock tower, `sct.env.clock_tower` | Reference/layout lock | Correct primary mass, cap, large clock motif, approach and openings from fixed views; clock assembly separable for later motion. |
| P0 | Ground and perimeter, `sct.env.ground`, `sct.env.boundary` | Layout and tower | Recognizable enclosed square, patterned ground and district openings; no open seams or hidden route obstruction. |
| P1 | Buildings/thresholds, `sct.env.facade_set` | Perimeter and movement envelope | Correct relative roofline, door/window/arch placement; wall thickness and believable returns; no arbitrary fantasy cottages. |
| P1 | Scaffold/worksite, `sct.env.scaffold_set` | Buildings, routes | Braced structures, supported decking, correct access and major painted construction forms; feet/beams contact surfaces. |
| P1 | Young Link hero, `chr.link.young` | Reference and proportions, export spike | Consistent full 3D silhouette; deformation and gameplay-camera tests pass; no image substituted for mesh. |
| P1 | Ground pattern/trim materials, `sct.mat.surface_set` | Neutral light, ground/facades | Rhythm reads at camera distance; no obvious repeated stains, scale mismatch, harsh seams or mip shimmer. |
| P2 | Banners and civic graphics, `sct.prop.banner_set`, `sct.prop.sign_set` | Locked anchor points and graphic references | Correct broad motifs, controlled sag, stable attachments; signs remain readable graphic shapes without invented canon text. |
| P2 | Stalls/awnings/crates, `sct.prop.market_set` | Ground, routes and material library | Dense enough for reference character, sparse enough for movement/readability; posts and feet grounded, awnings supported. |
| P2 | Moon/sky/light, `sct.sky.moon`, `sct.light.presets` | Fixed camera views and chosen day/time | Distinct face/silhouette and intentional framing; materials survive all presets; state labels truthful. |
| P3 | NPC visual prototypes, `chr.sct.worker_set` | Routes, scale and character pipeline | Reference-led proportions and costume, coherent scale, basic motion; static proxies do not imply schedules. |
| P3 | Small dressing, `sct.prop.dressing_set` | All composition/route reviews | Every cluster has a purpose; no silhouette clutter, unsupported props, duplicate coplanar surfaces or excessive detail noise. |
| P3 | Wind/dust/ambient detail, `sct.fx.ambient_set` | Stable lighting, attachment/export tests | Restrained motion, no clipping attachments or camera-obscuring noise; budget measured on target. |

Priorities order dependency risk, not estimated calendar duration. Final hero art should wait for an actual export/import spike so rig or shader incompatibilities do not invalidate days of polish.

## Faithful expansion rules

Faithful additions can explain existing forms: thickness behind a facade, a working clock support, timber joints, cloth stitching, a sheltered drain, wear around a doorway, or plausible unseen backs needed for a free camera. Match the surrounding design language and record the inferred part.

Optional expansion can add interiors, broader traversal, extra stalls, additional districts, denser crowds, or changed routes only as explicitly separated scope. Do not widen the square, move landmarks, add houses, or hide original routes under decorative assets while calling the result faithful. Keep a reference-layout review mode and an expansion mode if both directions are pursued.

Ground props with defined pivots, collision/support surfaces and contact shadows. Resolve beam ends, stair landings, skirt/ground contact and awning attachments in geometry. Avoid visually faking contact with oversized AO. Use physical separation for layered surfaces; reserve small depth bias for validated decals. Check contact and z-fighting with a moving camera at near and far distances, including dusk/night where shadows can conceal defects.

## Art acceptance packet

Save matching fixed views of the tower approach, central square reverse angle, district thresholds, scaffold, and Link front/profile/rear/three-quarter. Include neutral daylight, flat gray, material channels, wireframe and one intended final lighting state. Add a short comparison note listing retained forms, departures, unresolved evidence and actual renderer.

A family passes only when its silhouette, proportions, contact, materials and intended camera readability pass together. Browser screenshots establish the reviewer's displayed state. They do not prove Unity shader parity, a playable route, collision, skin deformation, performance or licensed build viability. Track each of those separately in the asset catalog.
