// Run via Pipeline script eval --file Tools/VerifyCityMvp.cs after starting Play Mode.
// Does not start/stop Play Mode or save assets. This is a local, bounded traversal smoke test.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Majoras.City;

void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException("City MVP verification: " + message);
}
float[] V(Vector3 value) => new[] { value.x, value.y, value.z };
float Horizontal(Vector3 a, Vector3 b) => new Vector2(a.x - b.x, a.z - b.z).magnitude;

Require(Application.isPlaying, "Start the intended Play Mode session first.");
var scene = SceneManager.GetActiveScene();
Require(scene.name == "SouthClockTown_MVP", "Unexpected active scene: " + scene.name);
var players = UnityEngine.Object.FindObjectsByType<CityExploration>();
var presentations = UnityEngine.Object.FindObjectsByType<CityPresentation>();
Require(players.Length == 1 && presentations.Length == 1, "Expected one exploration controller and one presentation controller.");
var player = players[0];
var presentation = presentations[0];
var controller = player.GetComponent<CharacterController>();
Require(player.enabled && controller != null && controller.enabled, "Exploration controller is disabled or missing.");
Require(player.viewCamera != null && player.viewCamera.enabled, "Player camera is missing/disabled.");
Require(presentation.enabled && presentation.viewCamera != null, "Presentation or its camera is missing/disabled.");
Require(presentation.exploration == player, "Presentation is bound to a different player.");
Require(!player.TestInputActive, "Clear an existing test-input override before running this check.");

var savedPosition = player.transform.position;
float savedYaw = player.Yaw, savedPitch = player.Pitch, savedTimeScale = Time.timeScale;
bool savedPaused = player.IsPaused, savedEnhanced = presentation.Enhanced, savedHud = presentation.hudVisible;
int savedPreset = presentation.LightingPreset;
var savedCursor = Cursor.lockState;
bool savedCursorVisible = Cursor.visible;
var savedCameraPosition = player.viewCamera.transform.localPosition;
var savedCameraRotation = player.viewCamera.transform.localRotation;
var segments = new List<object>();
var groundSamples = new List<object>();
int initialRespawns = player.RespawnCount;
float initialTravel = player.HorizontalDistanceTravelled;
object report = null;

RaycastHit GroundAt(Vector3 feet)
{
    Physics.SyncTransforms();
    // Explicitly exclude the player's own capsule; a hit on it is not ground evidence.
    var hits = Physics.RaycastAll(feet + Vector3.up * .35f, Vector3.down, .85f,
        Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
        .Where(hit => hit.collider != controller && !hit.collider.transform.IsChildOf(player.transform))
        .OrderBy(hit => hit.distance).ToArray();
    Require(hits.Length > 0, "No external support below feet at " + feet);
    Require(hits[0].normal.y > .45f, "Support probe hit a wall/steep surface at " + feet);
    return hits[0];
}
void SetInput(Vector2 move)
{
    player.SetTestInput(move, false);
    // Immediately establishes manual stepping, so Update cannot add movement during awaits.
    player.SimulateTestStep(.02f);
    Physics.SyncTransforms();
}
void Settle()
{
    SetInput(Vector2.zero);
    for (int i = 0; i < 35; i++) { player.SimulateTestStep(.02f); Physics.SyncTransforms(); }
    Require(player.IsGrounded, "Player did not settle onto the scene collision.");
    GroundAt(player.transform.position);
}
void Walk(string label, Vector2 input, int totalSteps, float expectedDistance)
{
    Require(totalSteps > 0 && totalSteps <= 100, "Walk step budget exceeded.");
    var before = player.transform.position;
    int respawns = player.RespawnCount, sideContacts = 0, groundedSteps = 0;
    player.SetTestInput(input, false);
    for (int i = 0; i < totalSteps; i++)
    {
        player.SimulateTestStep(.02f); Physics.SyncTransforms();
        if ((player.LastCollisionFlags & CollisionFlags.Sides) != 0) sideContacts++;
        if (player.IsGrounded) groundedSteps++;
        if (i % 8 == 0 || i == totalSteps - 1)
        {
            var hit = GroundAt(player.transform.position);
            groundSamples.Add(new { segment = label, step = i + 1, feet = V(player.transform.position),
                support = hit.collider.name, supportY = hit.point.y, normalY = hit.normal.y });
        }
    }
    var after = player.transform.position;
    float distance = Horizontal(before, after);
    Require(player.RespawnCount == respawns, label + " unexpectedly triggered fall recovery.");
    Require(player.IsGrounded && groundedSteps >= totalSteps - 3, label + " lost sustained ground contact.");
    Require(Mathf.Abs(after.y - before.y) < .35f, label + " changed elevation unexpectedly.");
    Require(distance >= expectedDistance * .8f && distance <= expectedDistance * 1.15f,
        label + " travelled " + distance.ToString("F3") + " m; expected about " + expectedDistance.ToString("F3") + " m.");
    segments.Add(new { label, from = V(before), to = V(after), distanceMeters = distance,
        expectedMeters = expectedDistance, steps = totalSteps, stepSeconds = .02f, sideContacts, groundedSteps });
}
void CheckMaterials(bool enhanced)
{
    Require(presentation.bindings != null && presentation.bindings.Length > 0, "No material comparison bindings.");
    int changedBindings = 0;
    foreach (var binding in presentation.bindings)
    {
        Require(binding != null && binding.renderer != null, "Missing comparison renderer.");
        var expected = enhanced ? binding.enhancedMaterials : binding.referenceMaterials;
        Require(expected != null && expected.Length > 0 && expected.All(material => material != null),
            "Missing comparison materials for " + binding.renderer.name);
        var actual = binding.renderer.sharedMaterials;
        Require(actual.Length == expected.Length && actual.Zip(expected, (a, b) => a == b).All(equal => equal),
            "Comparison material assignment differs for " + binding.renderer.name);
        if (binding.referenceMaterials != null && binding.enhancedMaterials != null &&
            !binding.referenceMaterials.SequenceEqual(binding.enhancedMaterials)) changedBindings++;
    }
    Require(changedBindings > 0, "Reference/enhanced bindings point at identical material arrays.");
}

try
{
    player.SetPaused(false); Time.timeScale = 1;
    presentation.SetHudVisible(false);
    Require(!presentation.hudVisible, "HUD visibility setter failed.");
    presentation.SetEnhanced(true); presentation.SetLightingPreset(0);
    Require(presentation.enhancedOnly != null && presentation.enhancedOnly.activeInHierarchy,
        "Enhanced authored-geometry root is missing/inactive.");
    int authoredRenderers = presentation.enhancedOnly.GetComponentsInChildren<Renderer>(true).Length;
    Require(authoredRenderers > 0, "Enhanced root contains no authored renderers.");

    // Source-coordinate route selected for this scene: southern square, away from tower/platform.
    // This checks a small route only; it does not qualify all walls, portals or platform paths.
    var routeOrigin = new Vector3(-5.7f, .08f, -4.5f);
    player.SetPosition(routeOrigin); player.SetLookAngles(0, 0); Settle();
    var settledSpawn = player.transform.position;
    var spawnGround = GroundAt(settledSpawn);
    Require(Mathf.Abs(settledSpawn.y) < .15f, "Selected southern route did not settle near source floor Y=0.");
    Walk("forward", Vector2.up, 34, player.walkSpeed * .02f * 34);
    Walk("strafe-right", Vector2.right, 10, player.walkSpeed * .02f * 10);
    Walk("strafe-left", Vector2.left, 10, player.walkSpeed * .02f * 10);
    Walk("backward", Vector2.down, 34, player.walkSpeed * .02f * 34);
    float returnError = Horizontal(settledSpawn, player.transform.position);
    Require(returnError < .25f, "Route return drift exceeded .25 m: " + returnError);

    // The exported source collision contains a broad south-facing boundary at Unity Z=-6,
    // normal +Z, with ground-to-four-meter faces across this X coordinate. Confirm the
    // actual loaded surface before driving the capsule into it; do not infer a wall test
    // from a stationary position or from CharacterController grounding alone.
    player.SetPosition(routeOrigin); player.SetLookAngles(180, 0); Settle();
    var wallStart = player.transform.position;
    var wallHits = Physics.RaycastAll(wallStart + Vector3.up * (controller.height * .5f),
        Vector3.back, 2.2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
        .Where(hit => hit.collider != controller && !hit.collider.transform.IsChildOf(player.transform))
        .OrderBy(hit => hit.distance).ToArray();
    Require(wallHits.Length > 0, "Known south wall was absent at torso height.");
    var wallHit = wallHits[0];
    Require(Mathf.Abs(wallHit.point.z + 6) < .04f && wallHit.normal.z > .9f,
        "Wall probe found a different surface: " + wallHit.collider.name + " at " + wallHit.point);
    int wallSideContacts = 0, wallGroundedSteps = 0, wallRespawns = player.RespawnCount;
    Vector3 wallPositionBeforeFinalSteps = wallStart;
    const int wallSteps = 60, wallTailSteps = 10;
    player.SetTestInput(Vector2.up, false);
    for (int i = 0; i < wallSteps; i++)
    {
        if (i == wallSteps - wallTailSteps) wallPositionBeforeFinalSteps = player.transform.position;
        player.SimulateTestStep(.02f); Physics.SyncTransforms();
        if ((player.LastCollisionFlags & CollisionFlags.Sides) != 0) wallSideContacts++;
        if (player.IsGrounded) wallGroundedSteps++;
    }
    var wallEnd = player.transform.position;
    float wallDistance = Horizontal(wallStart, wallEnd);
    float commandedWallDistance = player.walkSpeed * .02f * wallSteps;
    float wallTailDistance = Horizontal(wallPositionBeforeFinalSteps, wallEnd);
    float capsuleClearance = wallEnd.z - controller.radius - wallHit.point.z;
    Require(wallSideContacts >= 10, "Wall push did not produce repeated CharacterController side contacts.");
    Require(wallDistance > .8f && wallDistance < 1.6f && wallDistance < commandedWallDistance * .6f,
        "Wall did not bound movement: actual " + wallDistance + " m, commanded " + commandedWallDistance + " m.");
    Require(wallTailDistance < .025f, "Player continued sliding/moving during the final wall push: " + wallTailDistance);
    Require(capsuleClearance >= -.035f, "Capsule crossed the known wall plane: clearance " + capsuleClearance);
    Require(wallEnd.z < -5.5f && Mathf.Abs(wallEnd.x - wallStart.x) < .08f,
        "Wall push stopped elsewhere or slid sideways.");
    Require(player.RespawnCount == wallRespawns && player.IsGrounded && wallGroundedSteps >= wallSteps - 3,
        "Wall push lost ground or invoked fall recovery.");
    GroundAt(wallEnd);
    var wallCollision = new { surface = wallHit.collider.name, surfacePoint = V(wallHit.point),
        surfaceNormal = V(wallHit.normal), from = V(wallStart), to = V(wallEnd),
        actualDistanceMeters = wallDistance, commandedDistanceMeters = commandedWallDistance,
        finalTenStepsDistanceMeters = wallTailDistance, capsuleClearanceMeters = capsuleClearance,
        steps = wallSteps, stepSeconds = .02f, sideContacts = wallSideContacts, groundedSteps = wallGroundedSteps };

    // Pause input must point into the open square, not into the wall just tested.
    player.SetPosition(settledSpawn + Vector3.up * .04f); player.SetLookAngles(0, 0); Settle();

    player.SetTestInput(Vector2.up, false);
    player.SetPaused(true);
    var pausePosition = player.transform.position;
    float pausedTime = Time.time;
    for (int i = 0; i < 10; i++) player.SimulateTestStep(.02f);
    await Task.Delay(150);
    float pauseDistance = Vector3.Distance(pausePosition, player.transform.position);
    Require(pauseDistance < .001f, "Player moved while paused: " + pauseDistance);
    if (player.freezeTimeWhenPaused)
        Require(Mathf.Abs(Time.time - pausedTime) < .01f, "Scaled game time advanced while paused.");
    player.SetPaused(false);

    player.Respawn(); Settle();
    float resetDistance = Horizontal(player.spawnPosition, player.transform.position);
    Require(resetDistance < .08f, "Reset did not return to configured entrance.");
    Require(Mathf.Abs(Mathf.DeltaAngle(player.Yaw, player.spawnYaw)) < .1f &&
        Mathf.Abs(player.Pitch - player.spawnPitch) < .1f, "Reset look angles differ from configured entrance.");
    var resetPosition = player.transform.position;
    var resetGround = GroundAt(resetPosition);

    presentation.SetEnhanced(false); CheckMaterials(false);
    Require(!presentation.Enhanced && !presentation.enhancedOnly.activeSelf, "Reference mode retained enhanced-only geometry.");
    presentation.ToggleComparison(); CheckMaterials(true);
    Require(presentation.Enhanced && presentation.enhancedOnly.activeInHierarchy, "Enhanced mode did not restore authored geometry.");

    presentation.SetLightingPreset(0);
    Color dayBackground = presentation.viewCamera.backgroundColor, dayAmbient = RenderSettings.ambientLight;
    Color daySun = presentation.sun != null ? presentation.sun.color : Color.white;
    float dayIntensity = presentation.sun != null ? presentation.sun.intensity : 0;
    presentation.SetLightingPreset(1);
    Color duskBackground = presentation.viewCamera.backgroundColor, duskAmbient = RenderSettings.ambientLight;
    Color duskSun = presentation.sun != null ? presentation.sun.color : Color.white;
    float duskIntensity = presentation.sun != null ? presentation.sun.intensity : 0;
    Require(presentation.LightingPreset == 1 && Vector4.Distance(dayBackground, duskBackground) > .1f &&
        Vector4.Distance(dayAmbient, duskAmbient) > .05f, "Day/dusk environment presets did not differ.");
    if (presentation.sun != null)
        Require(Vector4.Distance(daySun, duskSun) > .1f && Mathf.Abs(dayIntensity - duskIntensity) > .1f,
            "Day/dusk sun settings did not differ.");

    SetInput(Vector2.zero);
    int beforeFrame = Time.frameCount; float beforeTime = Time.time;
    await Task.Delay(500);
    int afterFrame = Time.frameCount; float afterTime = Time.time;
    Require(!player.IsPaused && afterFrame > beforeFrame && afterTime > beforeTime,
        "Unpaused frames/scaled time did not advance during 500 ms observation.");

    report = new { passed = true, editorVersion = Application.unityVersion, scene = scene.name,
        timestampUtc = DateTime.UtcNow.ToString("O"), settledSpawn = V(settledSpawn),
        spawnSupport = spawnGround.collider.name, segments, groundSamples, routeReturnErrorMeters = returnError,
        wallCollision,
        pauseDistanceMeters = pauseDistance, resetPosition = V(resetPosition), resetDistanceMeters = resetDistance,
        resetSupport = resetGround.collider.name, materialBindings = presentation.bindings.Length, authoredRenderers,
        referenceAndEnhancedMaterialsVerified = true, authoredVisibilityVerified = true, lightingPresetsDiffer = true,
        frameObservation = new { beforeFrame, afterFrame, beforeTime, afterTime, waitMilliseconds = 500 },
        distanceCounterIncrease = player.HorizontalDistanceTravelled - initialTravel,
        respawnCounterIncrease = player.RespawnCount - initialRespawns,
        limits = new[] { "Small flat-ground route and one confirmed source-wall stop; not a complete collision/traversal audit.",
            "Tower stair/platform traversal NOT RUN: phase-correct route and collision still need dedicated verification.",
            "Public API hooks do not verify physical keyboard/mouse handling or visual appearance.",
            "Traversal distance/respawn diagnostic counters retain this test's increments." } };
}
finally
{
    // All restoring operations are synchronous; no subsequent test frame observes partial state.
    if (player != null)
    {
        player.SetPaused(true); player.ClearTestInput();
        player.SetPosition(savedPosition); player.SetLookAngles(savedYaw, savedPitch);
        if (player.viewCamera != null)
        {
            player.viewCamera.transform.localPosition = savedCameraPosition;
            player.viewCamera.transform.localRotation = savedCameraRotation;
        }
    }
    if (presentation != null)
    {
        presentation.SetLightingPreset(savedPreset); presentation.SetEnhanced(savedEnhanced);
        presentation.SetHudVisible(savedHud);
    }
    if (player != null) player.SetPaused(savedPaused);
    Time.timeScale = savedTimeScale;
    Cursor.lockState = savedCursor; Cursor.visible = savedCursorVisible;
    Physics.SyncTransforms();
}
return report;
