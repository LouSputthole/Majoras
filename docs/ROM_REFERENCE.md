# Local ROM reference: South Clock Town, Day 1 noon

Status: extracted, imported into Blender, and rendered in Unity on 2026-11-08. The local scene contains 21 visible meshes (1,501 triangles), the source room collision (508 triangles), 158 materials and 108 unique exported texture images. The separate clock, gears, counterweight, closed doors/panel and Day 1 platform are assembled. Play Mode advanced 540 frames during the final probe; three original entrance ground checks passed. The final import/check interval produced no new Unity warnings or errors. This remains a static original-asset reference with approximate lighting/LOD and omitted actors/effects, not modern replacement art or a playable remake. See [conversion instructions](../tools/reference/README.md) and [verification](../evidence/rom-reference-verification.json).

## Verified inputs and tools

| Input | Verified identity / role |
|---|---|
| Original compressed ROM | US N64 baseline, 33,554,432 bytes; local `local-reference/mm-decomp/baseroms/n64-us/baserom.z64`. |
| Compressed ROM MD5 | `2a0a8acb61538235bc1094d297fb6556`; rehashed locally and matches the pinned decomp's `checksum-compressed.md5`. |
| Compressed ROM SHA-1 | `d6133ace5afaa0882cf214cf88daba39e266c078`; rehashed locally. These hashes identify the compressed input, not its decompressed derivative. |
| Decompressed ROM | Local `baserom-decompressed.z64`, 49,283,072 bytes. Individual scene/object binary files are present under `extracted/n64-us/baserom/`. |
| Decomp source | [zeldaret/mm](https://github.com/zeldaret/mm/tree/3be2e24123f5e71fd13f939ecc0f9d7f73252639), local HEAD `3be2e24123f5e71fd13f939ecc0f9d7f73252639`. |
| Import tooling | [Fast-64/fast64](https://github.com/Fast-64/fast64/tree/b13010949567e4a2bf44ba3ccc18b06427243153), local HEAD `b13010949567e4a2bf44ba3ccc18b06427243153`. |
| Blender | Existing `A:\SteamLibrary\steamapps\common\Blender\blender.exe`, version 5.2.3. Source import and editable `.blend` save passed with the documented local compatibility adjustments. |

The Fast64 [game selector](https://github.com/Fast-64/fast64/blob/b13010949567e4a2bf44ba3ccc18b06427243153/__init__.py#L81) has the MM entry commented out; Z64 importer branches for MM-exclusive commands exist in [scene_header.py](https://github.com/Fast-64/fast64/blob/b13010949567e4a2bf44ba3ccc18b06427243153/fast64_internal/z64/importer/scene_header.py). Exposing the entry locally is an experimental import route, not proof of full MM compatibility. Importer success must be checked against this actor mapping and actual images.

ROMs, decompressed files, extracted textures/meshes, local scripts/tool clones and generated reference exports belong under ignored `local-reference/`. Unity reference copies belong under ignored `Unity/Assets/LocalReference/`. These ignore rules are present. Commit reference notes and reconstruction code separately from copyrighted game data; do not commit dumps or extracted assets accidentally.

## Scene selection and packed actor data

Use [assets/xml/scenes/Z2_CLOCKTOWER/Z2_CLOCKTOWER.xml](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/assets/xml/scenes/Z2_CLOCKTOWER/Z2_CLOCKTOWER.xml). The scene is `Z2_CLOCKTOWER` (segment 2), with one room `Z2_CLOCKTOWER_room_00` (segment 3). The XML identifies room display lists at `0x11708` and `0x13880`. Import the referenced room shape and its lists; two discovered display lists alone are not a complete scene assembly.

The local room binary is 80,048 bytes. Its default header is at offset `0x0`, with 50 actor entries at segmented pointer `0x03000084` and 23 object IDs at `0x03000054`. Its alternate-header pointer table at `0x48` references headers at `0x420`, `0x530` and `0x610`; these contain different actor sets. Use the default layer for normal Day 1 gameplay, rather than collecting every alternate layer's actors into one scene.

The 16-byte [ActorEntry](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/include/z64scene.h) stores position, packed rotation/metadata and params. Follow [Actor_SpawnEntry](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/code/z_actor.c) instead of interpreting every raw `rot` word as a binary angle:

- Actor ID is `entry.id & 0x1FFF`.
- Rotation values are decoded from `(rawRot >> 7) & 0x1FF`; ordinary actors use those values as degrees before conversion to game angles. ID flags alter raw-value treatment for special actor types; preserve that behavior when generalizing the parser.
- Low bits hold other data: the half-day mask is `((rawRotX & 7) << 7) | (rawRotZ & 0x7F)`, and `rawRotY & 0x7F` is the cutscene ID.
- Zero half-day mask means all half-days. Day 1 daytime is mask bit `0x80`, Day 1 night `0x40`, Day 2 daytime/night `0x20/0x10`, Day 3 daytime/night `0x08/0x04`. The definitions are in [z64actor.h](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/include/z64actor.h).

All positions below are original N64 world coordinates, with Y vertical. Object display-list vertices have their own local coordinates. Compute `world = actorPosition + actorRotation * (actorScale * localVertex)`, then apply one common import unit/axis conversion to the assembled scene. Actor scale `0.1` is not the scene's conversion to meters. In particular, do not shrink actor positions by `0.1` just because their geometry has that scale.

Fast64's Blender scale setting must be recorded and applied consistently to room meshes, actor translations and local geometry. Verify with known actor anchors before renaming units as meters. Do not mix an already-scaled imported room with unconverted actor coordinates.

## Clock Tower: required separate actor geometry

The tower's static masonry belongs with the room geometry. Its decorated clock, side gears, top counterweight and doors are actor geometry. A room-only export therefore omits the most recognizable moving landmark pieces.

Use [object_obj_tokeidai.xml](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/assets/xml/objects/object_obj_tokeidai.xml), binary `object_obj_tokeidai`, object ID `0x18C`, segment 6. Actor `Obj_Tokeidai` has ID `0x19C`; component type is `(params & 0xF000) >> 12`. The source behavior and exact matrix ordering are in [z_obj_tokeidai.c](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_Obj_Tokeidai/z_obj_tokeidai.c).

| Default actor index | Component / params | World position | Yaw | Actor scale | Required display list(s), segment-6 offset |
|---|---|---|---|---|---|
| 13 | Clock, `0x207F` (type 2) | `(-280, 555, -833)` | 0 degrees | 0.1 | Minute ring `gClockTowerMinuteRingDL` `0xCF28`; center/hand `gClockTowerClockCenterAndHandDL` `0xBEE8`; face `gClockTowerClockFaceDL` `0xE818`; sun/moon panel `gClockTowerSunAndMoonPanelDL` `0xC368`. |
| 14 | West side gear, `0x007F` (type 0) | `(-492, 555, -1012)` | 270 degrees | 0.1 | `gClockTowerExteriorGearDL` `0xBA78`. |
| 15 | East side gear, `0x007F` (type 0) | `(-68, 555, -1012)` | 90 degrees | 0.1 | Same gear DL, separate instance/rotation. |
| 16 | Counterweight, `0x307F` (type 3) | `(-280, 1103, -1020)` | 0 degrees at actor initialization | 0.1 | `gClockTowerCounterweightDL` `0xB208`. Its nighttime spotlight is `gClockTowerSpotlightDL` `0xB0C0`. |
| 18 | Rooftop staircase, `0xB000` (type 11) | `(-280, 255, -853)` | 0 degrees | 0.1 | `gClockTowerStaircaseToRooftopDL` `0xD8E8`, **hidden on normal Day 1**. |

For a static Day 1 12:00 reference pose:

1. Keep the tower closed: no final-night translation, tilt, collapse state or opened-staircase geometry.
2. The ring's local Z rotation is `-minuteRingRotation`. The center/hand remains in the clock actor's local basis. Then the face rotates about local Z by `-2 * clockFaceRotation`. The initial `clockFaceRotation` represents `floor(hours) * (65536/24)` game-angle units. At exactly noon, ring and face are both effectively identity rotations. At 06:00, the face instead rotates 180 degrees; use the recorded time consistently.
3. The sun/moon panel inherits the face's matrix, then translates by object-local `(0, -1112, -19.6)`, then rotates around its Y by 0 during daytime or 180 degrees at night. With scale 0.1, the noon translation contributes `(0, -111.2, -1.96)` world units. Do not place the panel at the clock's center.
4. Gears use their actor yaw plus their local Z minute rotation; the minute rotation is zero at noon.
5. Counterweight yaw changes by `-0x40` game-angle units per update, so its exact phase depends on elapsed frames. A static initialization phase of yaw 0 is explicit and reproducible; noon alone does not determine that phase. Daylight spotlight intensity is zero, so omit its visible beam in this static daytime assembly.

Do not import `gClockTowerUnusedTowerWallDL` (`0xD388`), `gClockTowerTerminaFieldWallsDL` (`0x9A08`), or `gWallClockClockFaceDL` (`0xF518`) as missing South Clock Town components. They are an unused piece, the Termina Field exterior, and a different wall-clock face respectively. Empty companion display lists are not additional geometry.

## Day 1 construction platform and doors

| Object XML / actor | Day 1 geometry and placement | Variant / assembly pitfall |
|---|---|---|
| [object_tokei_turret.xml](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/assets/xml/objects/object_tokei_turret.xml), `Obj_Tokei_Turret` `0x221` | Binary `object_tokei_turret`, segment 6. Actor index 24, params 0: `gClockTownTurretPlatformBaseDL` `0x2508` at `(-290,0,160)`. Index 21, params 1: `gClockTownTurretPlatformTopDL` `0x2A88` at `(-290,80,160)`. Both yaw 0, scale 0.1. | These are the two Day 1 pieces. Day 2 adds a base at Y80 and raises the top to Y160. Day 3 adds bases at Y80/Y160 and puts the top at Y240. `gClockTownFlagsDL` `0x3038`, params 2, is the Day 3 turret flags, not a required Day 1 flag instance. Preserve any bunting already in room geometry separately. |
| [object_tokei_step.xml](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/assets/xml/objects/object_tokei_step.xml), `Obj_Tokei_Step` `0x1B8` | Binary `object_tokei_step`, segment 6. Actor index 17: `gClocktowerPanelDL` `0x88` at `(-280,255,-853)`, yaw 0, scale 0.1. | Day 1 uses the closed panel. Do not duplicate the seven lowered/open-step panels from the final-night draw function. Distinct from the hidden type-11 staircase above. |
| [object_tokei_tobira.xml](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/assets/xml/objects/object_tokei_tobira.xml), `Obj_Tokei_Tobira` `0x1A2` | Binary `object_tokei_tobira`, segment 6. Default actor index 19 starts at `(-280,0,-807)`, yaw 0, scale 0.1. Its type-0 leaf draws `object_tokei_tobira_DL_001108` `0x1108`; it spawns a type-1 leaf drawing `object_tokei_tobira_DL_0013E8` `0x13E8`. | Initialization shifts their pivots to `(-360,0,-807)` and `(-200,0,-807)` respectively, both closed at yaw 0. The +/-80 offsets are world units. Importing only scene-listed actors gives half a door; ignoring the pivot shift puts both leaves at the center. |

Source logic: [turret](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_Obj_Tokei_Turret/z_obj_tokei_turret.c), [closed/open panel](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_Obj_Tokei_Step/z_obj_tokei_step.c), [spawned door leaf and pivot offset](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_Obj_Tokei_Tobira/z_obj_tokei_tobira.c).

## Other omitted visuals to track explicitly

Room geometry plus the preceding landmark pack is still an incomplete original scene. The following verified scene actors matter to a populated reference:

| Visual | Verified reference and placement | Integration caveat |
|---|---|---|
| Gold Deku Flower | `Obj_Etcetera` at `(-692,0,-348)`, yaw90, params `0x017F` selects type 2. `gameplay_keep.xml`: `gGoldDekuFlowerIdleDL` `0x11BD0`. Idle scale `(0.01,0.02,0.01)`. | Its origin/scale differs from tower actors; use the static idle DL for this reference. |
| Business Scrub / deed flower owner | `En_Akindonuts` at the same flower anchor, yaw90, params `0xFC04`; `object_dnt.xml`, `gBusinessScrubSkel` and standing animation, actor scale 0.01. | Skeletal animation, custom limb transforms, merchant state and scene progress determine his exact pose/presence. Do not draw a pile of raw limb DLs at one origin. |
| Owl statue | `Obj_Warpstone` at `(-540,100,-1099)`, yaw270, params4. `object_sek.xml`: closed `gOwlStatueClosedDL` `0x1D0`, opened `gOwlStatueOpenedDL` `0x3770`. | Choice depends on activation save flag, not day alone. For an unactivated reference choose closed explicitly. Actor keeps the engine's default scale, 0.01; confirm import against source bounds. |
| Postboxes | `En_Pst`: `(176,100,-945)`, yaw270, params0; `(-616,0,321)`, yaw90, params4. `object_pst.xml`: `gPostboxSkel` `0x1A80`, `gPostboxIdleAnim` `0x18`, actor scale 0.02. | Frame DL `0x4A0` and slot DL `0x638` are skeleton components; preserve limb transforms. |
| Gameplay moon | `En_Fall` at `(-280,11000,-1020)`, rotations X50/Y15/Z0 degrees, params `0x037F` selects type-6 LOD moon. `object_lodmoon.xml`: `gLodmoonEyesDL` `0x10E0` plus `gLodmoonMoonDL` `0x1158`. Day1 scale is `0.16*2.4 = 0.384`. | Runtime moves its drawn position to 9000 units from the camera when farther away; this changes the apparent image. Reproduce that rule or label the static physical-position approximation. Eyes also use runtime primitive color; mesh-only export can alter their appearance. Do not substitute the high-detail Termina Field/cutscene moon while claiming an exact South Clock Town baseline. |
| Smoking chimney effect | `Obj_Entotu` at `(319,553,-1308)`; `object_f53_obj.xml`: DL `0x158` and smoke DL `0x1C00`. | Billboard matrices use scale `(0.1,0.1,0)`; smoke uses generated vertex alpha and animated texture segments. A normal static mesh import will not reproduce it faithfully. |
| Day1 carpenters | `En_Daiku` / `object_daiku.xml`: `(105,0,39)` yaw90 params1; `(-440,0,207)` yaw90 params2; `(-277,0,384)` yaw270 params `0x0503`. | Skeletal poses, work motions and paths need separate reconstruction. Skip Day2/3-only actor placements when assembling Day1. |
| Other NPCs / actors | The default list also includes Anju, Kafei-related/scheduled actors, the postman, guard, dog, Moon's Tear/deed-related actors and interaction controllers. | A half-day filter establishes only an initial actor candidate set. Schedules, inventory/week-event flags, cutscene state and actor initialization can hide, relocate or change them. Scene list presence is not proof they stand visibly at their editor anchor at noon. |
| Link and Tatl | The player and companion are not supplied as finished characters by this room mesh extraction. | Add the correct original player/companion assets separately when needed; their animated rendering, fairy billboard effects and state require their own pipeline. No remade young Link model exists by virtue of extracting the environment. |

The two `Obj_Syokudai` instances at `(-447,0,67)` and `(-131,0,70)` have mask `0x155`, which excludes Day1 daytime; do not show their nighttime variant automatically. The high turret chest at `(-290,573,160)` is Day3-only. The chest at `(249,374,565)` exists in the all-days candidate set, with state-dependent contents/visibility to assess separately.

Additional pinned sources: [Deku Flower](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_Obj_Etcetera/z_obj_etcetera.c), [Business Scrub](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_En_Akindonuts/z_en_akindonuts.c), [owl](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_Obj_Warpstone/z_obj_warpstone.c), [postbox](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_En_Pst/z_en_pst.c), [moon scale and camera placement](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_En_Fall/z_en_fall.c), [chimney](https://github.com/zeldaret/mm/blob/3be2e24123f5e71fd13f939ecc0f9d7f73252639/src/overlays/actors/ovl_Obj_Entotu/z_obj_entotu.c).

## Next reconstruction order and verification

1. Finish the actual room mesh/material/collision import. Save the editable source, import report, tool versions and a view that establishes its actual rendered result. Record failures without substituting a generic village.
2. Import clock assembly, gears and counterweight with the transforms above. Validate tower silhouette, clock face/texture palette and noon panel location from a street-level camera.
3. Add the two closed swinging-door leaves, closed rooftop panel and the two Day1 platform pieces. Confirm contact, scale and absence of overlapping Day2/3 variants.
4. Add the flower, owl and postboxes, then the moon with its camera-distance rule. Keep omitted NPCs, effects and state-dependent assets listed until actually reconstructed.
5. Match reference camera/lighting as an original-asset baseline and save images. An extracted textured room is a reference milestone, not a completed populated game scene, and it is not modern remake art.
6. Only after the source-based reference renders correctly, measure landmark spacing and silhouettes and replace asset families with original modern authored models one at a time. Keep before/after cameras fixed. Build the genuine young Link mesh/rig through the art pipeline separately; a proxy or image sheet does not satisfy that deliverable.

Format conversion, object import, material fidelity, population/state fidelity, playable collision and Unity runtime behavior remain separate checks. Current local visual evidence is `local-reference/south-clock-town/unity-reference-playmode.png`; full reference-camera matching and gameplay fidelity remain unverified.
