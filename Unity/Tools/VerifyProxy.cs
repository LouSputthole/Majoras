// Run through Unity Pipeline: script eval --file Tools/VerifyProxy.cs.
// Kept outside Assets: this is an editor inspection script, not game code.
using System.IO;
using Majoras.Foundation;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

var failures = new List<string>();
void Check(bool condition, string message) { if (!condition) failures.Add(message); }
bool Near(Vector3 a, Vector3 b) => Vector3.Distance(a, b) < 0.002f;
Vector3 Vector(JToken value) => new Vector3((float)value[0], (float)value[1], (float)value[2]);

var manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath,
    "../../shared/scenes/south-clock-town.blockout.json"));
var manifest = JObject.Parse(File.ReadAllText(manifestPath));
var nodes = (JArray)manifest["nodes"];
var scene = SceneManager.GetActiveScene();
Check(scene.path == "Assets/Scenes/SouthClockTown_Proxy.unity", "Unexpected active scene");
var identities = scene.GetRootGameObjects()
    .SelectMany(root => root.GetComponentsInChildren<ProxyAssetIdentity>(true)).ToArray();
Check(identities.Length == nodes.Count, "Identity count differs from manifest");
Check(identities.Select(item => item.assetId).Distinct().Count() == nodes.Count,
    "Duplicate or missing stable IDs");
var proxyRoots = scene.GetRootGameObjects().Where(root =>
    root.name == (string)manifest["id"] + " [UNMEASURED PROXY]").ToArray();
Check(proxyRoots.Length == 1, "Proxy root missing or duplicated");
var proxyRoot = proxyRoots.FirstOrDefault();
if (proxyRoot != null)
{
    Check(Near(proxyRoot.transform.position, Vector3.zero), "Proxy root moved");
    Check(Quaternion.Angle(proxyRoot.transform.rotation, Quaternion.identity) < 0.001f,
        "Proxy root rotated");
    Check(Near(proxyRoot.transform.localScale, Vector3.one), "Proxy root scaled");
}

Physics.SyncTransforms();
var samples = new List<object>();
foreach (var node in nodes)
{
    var id = (string)node["id"];
    var matches = identities.Where(item => item.assetId == id).ToArray();
    if (matches.Length != 1) { failures.Add(id + ": missing/duplicate identity"); continue; }
    var item = matches[0];
    var size = Vector(node["size"]);
    var position = Vector(node["position"]);
    if ((string)node["kind"] == "shelter") position.y += size.y / 2f;
    Check(Near(item.transform.position, position), id + ": position differs");
    var rotation = Quaternion.Euler(0, (float)node["yawDegrees"], 0);
    var scale = size;
    if ((string)node["kind"] == "clockFace")
    {
        rotation *= Quaternion.Euler(90, 0, 0);
        scale = new Vector3(size.x, size.z / 2f, size.y);
    }
    else if ((string)node["kind"] == "cylinder") scale.y /= 2f;
    Check(Quaternion.Angle(item.transform.rotation, rotation) < 0.01f, id + ": rotation differs");
    Check(Near(item.transform.localScale, scale), id + ": local scale differs");
    Check(item.provenance == (string)node["provenance"], id + ": provenance differs");
    Check(item.collisionIntent == (bool)node["collision"], id + ": collision intent differs");
    Check(proxyRoot != null && item.transform.parent == proxyRoot.transform, id + ": wrong parent");
    var colliders = item.GetComponents<Collider>();
    Check(colliders.Length == ((bool)node["collision"] ? 1 : 0), id + ": collider count differs");
    Check(colliders.All(collider => collider is BoxCollider), id + ": unexpected collider shape");
    var renderer = item.GetComponent<Renderer>();
    Check(renderer != null && renderer.sharedMaterial != null, id + ": missing renderer/material");
    if (renderer != null && renderer.sharedMaterial != null)
    {
        var material = renderer.sharedMaterial;
        Check(material.shader != null && material.shader.isSupported, id + ": unsupported shader");
        Check(material.name == (string)node["material"], id + ": wrong material");
        // Independent world envelope check includes Unity's cylinder height and face rotation.
        var yaw = (float)node["yawDegrees"] * Mathf.Deg2Rad;
        var worldSize = new Vector3(Mathf.Abs(Mathf.Cos(yaw)) * size.x + Mathf.Abs(Mathf.Sin(yaw)) * size.z,
            size.y, Mathf.Abs(Mathf.Sin(yaw)) * size.x + Mathf.Abs(Mathf.Cos(yaw)) * size.z);
        Check(Near(renderer.bounds.size, worldSize), id + ": renderer world envelope differs");
        foreach (var collider in colliders)
        {
            Check(collider.enabled && !collider.isTrigger, id + ": collision disabled or trigger-only");
            Check(Near(collider.bounds.center, renderer.bounds.center), id + ": collider center differs");
            Check(Near(collider.bounds.size, worldSize), id + ": collider world envelope differs");
        }
        var surface = manifest["materials"].Single(value => (string)value["id"] == material.name);
        Color expectedColor;
        ColorUtility.TryParseHtmlString((string)surface["color"], out expectedColor);
        Check(Vector4.Distance(material.color, expectedColor) < 0.002f, id + ": material color differs");
        var shaderName = material.shader == null ? "missing" : material.shader.name;
        var smoothnessProperty = shaderName == "Standard" ? "_Glossiness" : "_Smoothness";
        Check(material.HasProperty(smoothnessProperty) &&
            Mathf.Abs(material.GetFloat(smoothnessProperty) - (1f - (float)surface["roughness"])) < 0.002f,
            id + ": material roughness differs");
        samples.Add(new { id, position = new[] { item.transform.position.x, item.transform.position.y, item.transform.position.z },
            bounds = new[] { renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z },
            colliderCount = colliders.Length, shader = shaderName });
    }
}

// Fixed anchors catch mirrored axes and incorrect collision top surfaces.
var ground = identities.FirstOrDefault(item => item.assetId == "sct.env.ground");
Check(ground != null && ground.GetComponent<Collider>() != null &&
    Mathf.Abs(ground.GetComponent<Collider>().bounds.max.y) < 0.002f, "Ground top is not Y=0");
RaycastHit spawnHit;
var hitGround = Physics.Raycast(new Vector3(0, 3, -20), Vector3.down, out spawnHit, 5f);
Check(hitGround && spawnHit.collider.gameObject == (ground == null ? null : ground.gameObject) &&
    Mathf.Abs(spawnHit.point.y) < 0.002f, "South courtyard ground raycast failed");
var tower = identities.FirstOrDefault(item => item.assetId == "sct.env.clock_tower");
Check(tower != null && Near(tower.transform.position, new Vector3(0, 5, 15)), "Tower axis anchor differs");
var camera = Camera.main;
Check(camera != null && camera.enabled, "Main camera missing or disabled");

return new { passed = failures.Count == 0, editorVersion = Application.unityVersion,
    scene = scene.path, stage = (string)manifest["stage"], identityCount = identities.Length,
    colliderCount = identities.Sum(item => item.GetComponents<Collider>().Length),
    groundRaycastPassed = hitGround, failures, samples,
    limit = "Proxy contract and collision inspection only; no measured fidelity or gameplay validation" };
