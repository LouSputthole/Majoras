// Pipeline script eval --file Tools/VerifyCityRoutes.cs, in the owned MVP Play Mode session.
// Bounded source-backed side-ramp route plus one preview-boundary stop. No scene/asset writes.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Majoras.City;

void Require(bool value, string message)
{
    if (!value) throw new InvalidOperationException("City route verification: " + message);
}
float[] V(Vector3 value) => new[] { value.x, value.y, value.z };
float Planar(Vector3 a, Vector3 b) => new Vector2(a.x-b.x, a.z-b.z).magnitude;
Require(Application.isPlaying && SceneManager.GetActiveScene().name == "SouthClockTown_MVP",
    "Run in the existing SouthClockTown_MVP Play Mode session.");
var player = UnityEngine.Object.FindFirstObjectByType<CityExploration>();
var presentation = UnityEngine.Object.FindFirstObjectByType<CityPresentation>();
Require(player != null && presentation != null && player.enabled, "Missing active MVP components.");
Require(!player.TestInputActive, "Clear the existing test-input override first.");
var controller = player.GetComponent<CharacterController>();
Require(controller != null && controller.enabled && player.viewCamera != null, "Missing capsule/camera.");
var savedPosition = player.transform.position;
float savedYaw = player.Yaw, savedPitch = player.Pitch, savedScale = Time.timeScale;
bool savedPaused = player.IsPaused, savedEnhanced = presentation.Enhanced, savedHud = presentation.hudVisible;
int savedPreset = presentation.LightingPreset;
var savedCursor = Cursor.lockState; bool savedCursorVisible = Cursor.visible;
var savedCameraPosition = player.viewCamera.transform.localPosition;
var savedCameraRotation = player.viewCamera.transform.localRotation;
int initialRespawns = player.RespawnCount;
float initialDistance = player.HorizontalDistanceTravelled;
var route = new List<object>();
var samples = new List<object>();
int routeStepCount = 0;
object report = null;

RaycastHit Probe(Vector3 expectedFeet)
{
    Physics.SyncTransforms();
    var hits = Physics.RaycastAll(expectedFeet + Vector3.up * .55f, Vector3.down, 1.15f,
        Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
        .Where(hit => hit.collider != controller && !hit.collider.transform.IsChildOf(player.transform))
        .OrderBy(hit => hit.distance).ToArray();
    Require(hits.Length > 0, "No support near " + expectedFeet);
    Require(hits[0].normal.y > .65f, "Probe found a non-walkable surface near " + expectedFeet);
    return hits[0];
}
void Settle()
{
    player.SetTestInput(Vector2.zero, false);
    for (int i = 0; i < 35; i++) { player.SimulateTestStep(.02f); Physics.SyncTransforms(); }
    Require(player.IsGrounded, "Player did not settle onto source collision.");
    Probe(player.transform.position);
}
void WalkTo(string name, Vector3 target)
{
    var expectedSupport = Probe(target);
    Require(Mathf.Abs(expectedSupport.point.y-target.y) < .15f,
        name + " source waypoint floor differs: expected " + target.y + ", hit " + expectedSupport.point.y);
    var start = player.transform.position;
    int beforeRespawns = player.RespawnCount, steps = 0, grounded = 0, stuck = 0, sides = 0;
    int budget = Mathf.Min(650, Mathf.CeilToInt(Planar(start,target)/(player.walkSpeed*.02f)*3)+30);
    while (Planar(player.transform.position, target) > .075f)
    {
        Require(++steps <= budget && ++routeStepCount <= 1400, "Route step budget exceeded at " + name);
        var before = player.transform.position;
        Vector2 direction = new Vector2(target.x-before.x, target.z-before.z);
        float magnitude = Mathf.Min(1, direction.magnitude/(player.walkSpeed*.02f));
        player.SetTestInput(direction.normalized*magnitude, false);
        player.SimulateTestStep(.02f); Physics.SyncTransforms();
        if (player.IsGrounded) grounded++;
        if ((player.LastCollisionFlags & CollisionFlags.Sides) != 0) sides++;
        stuck = Planar(before,player.transform.position) < .002f ? stuck+1 : 0;
        Require(stuck < 18, "Blocked for 18 steps before " + name + " at " + player.transform.position);
        Require(player.RespawnCount == beforeRespawns, "Fall recovery occurred before " + name);
        if (steps % 8 == 0)
        {
            var support = Probe(player.transform.position);
            Require(Mathf.Abs(player.transform.position.y-support.point.y) < .35f,
                "Feet lost support near " + name);
            samples.Add(new { waypoint = name, step = steps, feet = V(player.transform.position),
                surface = support.collider.name, supportY = support.point.y, normalY = support.normal.y });
        }
    }
    Settle();
    var end = player.transform.position;
    Require(Planar(end,target) < .1f && Mathf.Abs(end.y-target.y) < .18f,
        name + " was not reached: " + end + ", expected " + target);
    route.Add(new { name, from = V(start), to = V(end), target = V(target), steps,
        actualPlanarMeters = Planar(start,end), elevationChange = end.y-start.y, groundedSteps = grounded, sideContacts = sides });
}

try
{
    player.SetPaused(false); Time.timeScale = 1;
    player.SetPosition(new Vector3(-5.7f,.08f,-4.5f)); player.SetLookAngles(0,0); Settle();
    var start = player.transform.position;
    // Go east of the raised central object, then use the left-side ramp. Source triangle
    // 251/252: X[-6.33,-5.13], Z[7.87,10], Y[0,1], normal (0,.905,-.425).
    WalkTo("clear south square", new Vector3(-3.7f,0,-4.5f));
    WalkTo("clear central object", new Vector3(-3.7f,0,6.6f));
    WalkTo("west ramp approach", new Vector3(-5.7f,0,6.6f));
    WalkTo("lower ramp landing", new Vector3(-5.7f,1,10.3f));
    WalkTo("west upper approach", new Vector3(-5.7f,1,12.1f));
    // Enter the SECOND ramp at its west base. Entering at X=-5.7 across Z=12.53
    // would instead hit the ramp's .283m SIDE lip and is not the intended ascent.
    WalkTo("second ramp west base", new Vector3(-6.8f,1,12.1f));
    WalkTo("second ramp alignment", new Vector3(-6.8f,1,13.2f));
    // Source triangles246/247: X[-6.4,-3.93], Z[12.53,14], Y[1,2], normal(-.375,.927,0).
    WalkTo("north upper level", new Vector3(-3.7f,2,13.2f));
    var upper = player.transform.position;
    Require(upper.y-start.y > 1.8f && upper.y-start.y < 2.2f, "Route did not gain the two-meter upper level.");

    var thresholds = UnityEngine.Object.FindObjectsByType<CityThreshold>();
    Require(thresholds.Length == 8, "Expected eight preview boundaries, found " + thresholds.Length);
    foreach (var threshold in thresholds)
    {
        Require(!string.IsNullOrWhiteSpace(threshold.destination), "Unlabeled preview boundary.");
        Require(threshold.stop is BoxCollider && threshold.stop.enabled && !threshold.stop.isTrigger &&
            threshold.stop.gameObject.activeInHierarchy, "Boundary does not have an active solid box: " + threshold.destination);
        var size = threshold.stop.bounds.size;
        Require(size.x > .1f && size.y > .1f && size.z > .1f, "Degenerate boundary: " + threshold.destination);
    }
    var eastCandidates = thresholds.Where(t => t.destination == "East Clock Town" && t.stop.bounds.center.z < 0).ToArray();
    Require(eastCandidates.Length == 1, "Missing/ambiguous lower East Clock Town boundary.");
    var east = eastCandidates[0];
    Require(Mathf.Abs(east.stop.bounds.min.x-3.47f) < .05f, "Lower east boundary moved from source-floor edge X=3.47.");
    player.SetPosition(new Vector3(2.57f,.08f,-2.4f)); player.SetLookAngles(90,0); Settle();
    var exitStart = player.transform.position;
    var exitHits = Physics.RaycastAll(exitStart+Vector3.up*.65f, Vector3.right, 1.8f,
        Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
        .Where(hit => hit.collider != controller && !hit.collider.transform.IsChildOf(player.transform))
        .OrderBy(hit => hit.distance).ToArray();
    Require(exitHits.Length > 0 && exitHits[0].collider == east.stop, "Exit approach does not lead to the selected boundary box.");
    int sideContacts = 0, exitRespawns = player.RespawnCount;
    Vector3 beforeTail = exitStart;
    player.SetTestInput(Vector2.up,false);
    for (int i = 0; i < 50; i++)
    {
        if (i == 40) beforeTail = player.transform.position;
        player.SimulateTestStep(.02f); Physics.SyncTransforms();
        if ((player.LastCollisionFlags & CollisionFlags.Sides) != 0) sideContacts++;
    }
    var exitEnd = player.transform.position;
    float exitTravel = Planar(exitStart,exitEnd), exitTailTravel = Planar(beforeTail,exitEnd);
    float exitClearance = east.stop.bounds.min.x-(exitEnd.x+controller.radius);
    Require(sideContacts >= 10 && exitTravel > .35f && exitTravel < 1.1f,
        "Lower east boundary did not stop the walker; travel " + exitTravel + ", side contacts " + sideContacts);
    Require(exitTailTravel < .025f && exitClearance >= -.035f && Mathf.Abs(exitEnd.z+2.4f) < .08f,
        "Walker crossed/slid past lower east boundary.");
    Require(player.RespawnCount == exitRespawns && player.IsGrounded, "Exit stop lost support or triggered recovery.");
    player.SetTestInput(Vector2.zero,false); player.SimulateTestStep(.02f);
    await Task.Delay(250);
    string nearby = CityThreshold.Nearby(player);
    Require(nearby == "East Clock Town", "Nearby label does not identify the blocked exit: " + nearby);
    report = new { passed = true, scene = SceneManager.GetActiveScene().name,
        timestampUtc = DateTime.UtcNow.ToString("O"), start = V(start), upperLevel = V(upper),
        elevationGainMeters = upper.y-start.y, routeStepCount, route, samples,
        thresholds = thresholds.Select(t => new { destination = t.destination,
            center = V(t.stop.bounds.center), size = V(t.stop.bounds.size) }).ToArray(),
        eastLower = new { label = nearby, from = V(exitStart), to = V(exitEnd),
            actualDistanceMeters = exitTravel, commandedDistanceMeters = player.walkSpeed*.02f*50,
            finalTenStepsDistanceMeters = exitTailTravel, capsuleClearanceMeters = exitClearance, sideContacts },
        diagnosticDistanceIncrease = player.HorizontalDistanceTravelled-initialDistance,
        diagnosticRespawnIncrease = player.RespawnCount-initialRespawns,
        limits = new[] { "Side ramps to the north upper level; closed Day 1 clock-tower stairs/roof are not traversed.",
            "Eight boundary labels/solid boxes inspected; proximity and physical stop exercised at the lower east exit only.",
            "Direct waypoint input verifies collision/movement, not physical keyboard input or the rendered HUD label.",
            "Pose/presentation restored; diagnostic distance counters retain test increments." } };
}
finally
{
    if (player != null)
    {
        player.SetPaused(true); player.ClearTestInput(); player.SetPosition(savedPosition);
        player.SetLookAngles(savedYaw,savedPitch);
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
    Time.timeScale = savedScale; Cursor.lockState = savedCursor; Cursor.visible = savedCursorVisible;
    Physics.SyncTransforms();
}
return report;
