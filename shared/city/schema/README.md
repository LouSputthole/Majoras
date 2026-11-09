# City data contracts (PIPE-01 freeze, 2026-11-08)

This folder freezes what `docs/FULL_CITY_PLAN.md` section 3 proposed. Everything under `shared/city/` is tracked and must hold only IDs, citations, hashes, counts and reviewed placements. ROM-derived bulk (geometry, texture bytes, actor arrays) stays under ignored `local-reference/` and `Unity/Assets/LocalReference/`.

## Files

| Path | Owner | Written by | Read by |
| --- | --- | --- | --- |
| `schema/region.schema.json`, `portal.schema.json`, `camera.schema.json` | integrator | hand-edited; a change bumps `schemaVersion` and is logged in `docs/ITERATION_LOG.md` | `tools/reference/validate_city.py`, future importer (PIPE-02) and Unity bindings |
| `ids.json` | integrator | `python -I tools/reference/build_city_ids.py` from the three registers in `docs/city/` | validator, importer, evidence writers |
| `regions/<region>.json` | the dossier's author proposes, the integrator accepts | `python -I tools/reference/region_from_dossier.py docs/city/<dir>/<file>.md` lifts the dossier's "Proposed region record" block and maps it onto the frozen enums | importer (PIPE-02), evidence, saves |
| `portals.json` | integrator | not yet generated (PORT-01 exports it from `docs/city/PORTAL_REGISTER.md`) | transition system |
| `cameras/<region>.json`, `routes/<region>.json` | capture / route owners | not yet generated (REF-03 captures, SCT-01 routes) | evidence comparison |

Validate before every commit that touches this folder:

```
python -I tools/reference/validate_city.py --selftest
```

Exit 1 and a `file: $.json.path: reason` line for every problem. The self-test proves the validator still rejects an unknown key, a wrong version, an unregistered or retired ID, a free-text enum, a wrong conversion matrix, too few anchors, a bad hash, a foreign room and truncated JSON. A record that is not listed as OK is not accepted; an omitted field is never a pass.

## Frozen ID grammar

| Namespace | Pattern | Count frozen | Notes |
| --- | --- | --- | --- |
| location | `loc.<snake>` | 22 | the 5 exteriors and 17 venues of `LOCATION_REGISTER.md`; `scene.south_clock_town` and `sct.*` stay valid aliases of `loc.south_clock_town` |
| room | `loc.<snake>.room_NN` | 36 | one per declared room file; `NN` is the source room index |
| portal | `portal.<from_short>.<to_short>[.<qualifier>]` | 94 live, 1 retired | directed; `<from_short>` is the location without `loc.`; qualifier distinguishes `.upper`/`.lower`, `.kickout`, `.rN_rM` room swaps, `.bomjimb`, `.p116`; `portal.east_clock_town.astral_observatory.p169` is retired (wall polygon never read by the player exit check) and the name is never reused |
| profile | `prof.d<1-3>.<time>.<layer>` | 9 | baseline `prof.d1.noon.normal`; `layer` is `normal` (scene layer 0) or `lN` |
| camera | `cam.<loc_short>.<name>` | 118 | every `cam.*` id in a `SOURCE_REGISTER.md` table row (per-camera and per-location tables); a record may only cite a registered camera |
| route | `route.<loc_short>.<name>` | 0 | South's legacy labels `R1`..`R20` from `evidence/` are accepted as-is in `routeIds` until SCT-01 renames them |
| anchor | `anchor.<loc_abbrev>.<name>` | per record | `loc_abbrev` is the dossier's short code (`sct`, `nct`, `wct`, `ect`, `lp`); at least three per region, elevation included |
| water | `water.<loc_short>.<name>` | per record | one per source water box |
| external | `ext.<scene_lower>` | 6 | destinations outside the 22 scenes (`ext.z2_00keikoku`, ...); never a region record |
| component source key | `Z2_<SCENE>/h<header>/room_NN/a<index>[/child]`, `.../h<header>/ta<N>[-ta<M>]`, `.../h<header>/transition/ta<N>` | per record | the durable identity of a source actor or transition actor; never a material index, filename, Unity instance id or English label |

All patterns are machine-readable in `ids.json` under `patterns`. New IDs are added by editing the register that owns them and rerunning `build_city_ids.py`; a record that cites an ID the registry lacks fails validation.

## Coordinate contract

- Source frame: N64 world units, +Y up, as written in the extracted C. Exterior scenes are assumed to share one world frame (`sourceFrame.sharedFrame.status` is `inferred` until a collision overlay verifies it).
- Conversion: `Unity (x, y, z) = N64 (x, y, -z) / 100`, matrix `[[0.01,0,0,0],[0,0.01,0,0],[0,0,-0.01,0],[0,0,0,1]]`. The schema pins this constant; a different mapping is a schema change, not a per-record choice.
- `provisionalPhysicalScale` stays `true` until a surveyed anchor set fixes the scale; `authoredRegistrationMatrix` is `null` until authored art is registered against the reference.

## Vocabularies

| Field | Values |
| --- | --- |
| `components[].class` | `static-mesh`, `posed-skeletal`, `npc-scheduled`, `billboard-effect`, `controller-invisible`, `door-transition` |
| `components[].variant` | `camera-relative`, `state-pickup`, `spawned`, `posed-skeletal` |
| `components[].status` | `selected` (in the profile's static list), `imported` (present in the current Unity import), `optional` (state-dependent) + free-text `note` |
| `anchors[].status` | `proposed`, `probed` (a ground probe exists in evidence), `surveyed` |
| `readiness.*` and portal/camera `results` | `not_run`, `pass`, `fail`, `blocked`, `not_applicable`, `partial` (region readiness only); explanations go in `readinessNotes` |
| portal `mode` | `district`, `door`, `room`, `special`, `external-boundary` |
| `localExports[].producedBy` | `committed-scripts`, `inferred-committed-scripts`, `unrecorded` |

## Source-output ownership

| Artifact | Produced by | Provenance fields that must be recorded |
| --- | --- | --- |
| extracted scene/room C | native ZAPD (`tools/reference/README.md`) | `source.extractorSha256`, `localSourceInputs[].sha256` |
| `reference.json`, textures, `texture-sources.json` | `tools/reference/import_clocktown.py` running `export_reference.py` in Blender | `localExports.referenceJson.sha256`, script hashes, `writtenLocal` |
| `Unity/Assets/LocalReference/<region>/Generated/*` | `ImportRomReference.Generate()` | `localExports.unityReferenceScene`; `readiness.import` is `fail` until REC-03 records input and output hashes together |
| `Unity/Assets/Scenes/*_MVP.unity` | `BuildCityMvp.Generate()` | consumes one export generation only; mixing generations is a `fail` |
| `evidence/**` | editor tests, captures | every result names source revision, dirty-state fingerprint, input/export hashes, tool versions, scene, profile, timestamp |
