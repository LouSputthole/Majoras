using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Majoras.Foundation;

namespace Majoras.Editor
{
    // Unvalidated until the Unity activation gate is cleared. Creates proxy envelopes,
    // not finished meshes. Canonical JSON is already Unity-handed; do NOT reflect Z.
    public static class ImportSouthClockTown
    {
        [Serializable] private class SceneData { public int schemaVersion; public string id; public string units; public Node[] nodes; public Surface[] materials; }
        [Serializable] private class Node { public string id, name, kind, material, provenance; public float[] position, size; public float yawDegrees; public bool collision; }
        [Serializable] private class Surface { public string id, color; public float roughness; }

        [MenuItem("Majoras/Generate South Clock Town Proxy")]
        public static void Generate()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var path = Path.GetFullPath(Path.Combine(Application.dataPath, "../../shared/scenes/south-clock-town.blockout.json"));
            var data = JsonUtility.FromJson<SceneData>(File.ReadAllText(path));
            if (data == null || data.schemaVersion != 1 || data.units != "meters" || data.nodes == null) throw new InvalidDataException("Unsupported scene contract");
            var ids = new HashSet<string>();
            foreach (var node in data.nodes)
            {
                if (string.IsNullOrEmpty(node.id) || !ids.Add(node.id)) throw new InvalidDataException("Duplicate/missing asset ID");
                Vector(node.position); var size = Vector(node.size);
                if (size.x <= 0 || size.y <= 0 || size.z <= 0) throw new InvalidDataException("Non-positive size: " + node.id);
            }
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject(data.id + " [UNMEASURED PROXY]");
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null) throw new InvalidOperationException("No supported material shader");
            Directory.CreateDirectory("Assets/Generated/Materials");
            AssetDatabase.Refresh();
            var materials = new Dictionary<string, Material>();
            foreach (var surface in data.materials)
            {
                var assetPath = "Assets/Generated/Materials/" + surface.id + ".mat";
                var mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (mat == null) { mat = new Material(shader); AssetDatabase.CreateAsset(mat, assetPath); }
                if (ColorUtility.TryParseHtmlString(surface.color, out var color)) mat.color = color;
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 1f - surface.roughness);
                if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 1f - surface.roughness);
                EditorUtility.SetDirty(mat);
                materials.Add(surface.id, mat);
            }
            foreach (var node in data.nodes)
            {
                var cylinder = node.kind == "cylinder" || node.kind == "clockFace";
                var go = GameObject.CreatePrimitive(cylinder ? PrimitiveType.Cylinder : PrimitiveType.Cube);
                go.name = node.id; go.transform.SetParent(root.transform);
                go.transform.position = Vector(node.position); go.transform.rotation = Quaternion.Euler(0, node.yawDegrees, 0);
                var scale = Vector(node.size);
                if (node.kind == "clockFace") { scale = new Vector3(scale.x, scale.z / 2f, scale.y); go.transform.rotation *= Quaternion.Euler(90, 0, 0); }
                else if (cylinder) scale.y /= 2f;
                // Shelter/bunting are envelopes in the Unity seed, not detailed viewer meshes.
                if (node.kind == "shelter") go.transform.position += Vector3.up * scale.y / 2f;
                go.transform.localScale = scale;go.GetComponent<Renderer>().sharedMaterial = materials[node.material];
                if (!node.collision) UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
                else if (cylinder)
                {
                    // Unity cylinders come with capsule colliders. Use an explicit
                    // envelope so a flattened crown does not gain a spherical collider.
                    UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
                    go.AddComponent<BoxCollider>().size = new Vector3(1, 2, 1);
                }
                var identity = go.AddComponent<ProxyAssetIdentity>();identity.assetId = node.id;identity.provenance = node.provenance;identity.collisionIntent = node.collision;
            }
            var sun = new GameObject("Sun").AddComponent<Light>();sun.type = LightType.Directional;sun.transform.rotation = Quaternion.Euler(45, -30, 0);sun.intensity = 1.4f;
            var camera = new GameObject("Main Camera").AddComponent<Camera>();camera.tag = "MainCamera";camera.transform.position = new Vector3(40, 32, -45);camera.transform.LookAt(new Vector3(0, 6, 4));camera.farClipPlane = 300;
            Directory.CreateDirectory("Assets/Scenes");AssetDatabase.Refresh();
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/SouthClockTown_Proxy.unity");AssetDatabase.SaveAssets();
            Debug.Log("Created unmeasured proxy scene. No gameplay or final art is present.");
        }
        private static Vector3 Vector(float[] v)
        {
            if (v == null || v.Length != 3) throw new InvalidDataException("Expected vector3");
            foreach (var n in v) if (float.IsNaN(n) || float.IsInfinity(n)) throw new InvalidDataException("Non-finite coordinate");
            return new Vector3(v[0], v[1], v[2]);
        }
    }
}
