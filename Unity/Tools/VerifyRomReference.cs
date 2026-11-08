using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

var renderers = UnityEngine.Object.FindObjectsByType<MeshRenderer>();
var colliders = UnityEngine.Object.FindObjectsByType<MeshCollider>();
int renderTriangles = 0;
var uniqueMeshes = new HashSet<Mesh>();
foreach (var renderer in renderers)
{
    var mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
    // Static batching shares one combined mesh across many renderers in Play Mode.
    if (uniqueMeshes.Add(mesh)) renderTriangles += mesh.triangles.Length / 3;
    foreach (var material in renderer.sharedMaterials)
        if (material == null || material.shader == null || material.shader.name != "Majoras/ROM Reference Combiner")
            throw new Exception("Missing reference material: " + renderer.name);
}
Physics.SyncTransforms();
var probes = new[] { new Vector3(-2.79f, .5f, -6.27f), new Vector3(-7.8f, .5f, 1f), new Vector3(3.23f, .5f, -2.47f) };
var ground = new List<object>();
foreach (var point in probes)
{
    bool hit = Physics.Raycast(point, Vector3.down, out RaycastHit contact, 1);
    if (!hit || Mathf.Abs(contact.point.y) > .05f) throw new Exception("Original entrance collision probe failed: " + point);
    ground.Add(new { x = point.x, z = point.z, groundY = contact.point.y });
}
var frame = Time.frameCount; var time = Time.time;
await Task.Delay(500);
bool advancing = Application.isPlaying && Time.frameCount > frame && Time.time > time;
if (renderers.Length != 21 || colliders.Length != 1 || renderTriangles != 1501 || !advancing)
    throw new Exception($"Reference check: renderers={renderers.Length}, colliders={colliders.Length}, triangles={renderTriangles}, uniqueMeshes={uniqueMeshes.Count}, playing={Application.isPlaying}, frames={Time.frameCount-frame}, seconds={Time.time-time}.");
return new { passed = true, renderMeshes = renderers.Length, renderTriangles,
    collisionTriangles = colliders[0].sharedMesh.triangles.Length / 3,
    groundProbes = ground, framesAdvanced = Time.frameCount-frame, secondsAdvanced = Time.time-time,
    limit = "Static reference only; no actor collision, character traversal or original simulation verified." };
