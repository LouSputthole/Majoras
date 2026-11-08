using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Majoras.Editor
{
    // Local reference inspection only. ROM-derived inputs and all generated assets stay ignored.
    public static class ImportRomReference
    {
        const string Input = "Assets/LocalReference/SouthClockTown";
        const string Output = Input + "/Generated";
        [Serializable] class Document { public int version; public string coordinateSystem; public Surface[] materials; public Geometry[] meshes; }
        [Serializable] class Surface
        {
            public string name, texture0, texture1;
            public Color prim, env;
            public float primLodFrac;
            public Vector2 uvBasisSize, texScale;
            public Tile tile0, tile1;
            public string[] combiner1, combiner2, wrap0, wrap1;
            public bool twoCycle, lighting, transparent, cullBack;
        }
        [Serializable] class Tile { public Vector2 size, shift, low; }
        [Serializable] class Geometry
        {
            public string name;
            public bool collision;
            public Vector3[] vertices, normals;
            public Color[] colors;
            public Vector2[] uv;
            public Part[] submeshes;
        }
        [Serializable] class Part { public int material; public int[] indices; }
        static readonly Dictionary<string, int> Sources = new Dictionary<string, int>
        {
            {"0",0},{"1",1},{"COMBINED",2},{"TEXEL0",3},{"TEXEL1",4},
            {"PRIMITIVE",5},{"SHADE",6},{"ENVIRONMENT",7},{"COMBINED_ALPHA",8},
            {"TEXEL0_ALPHA",9},{"TEXEL1_ALPHA",10},{"PRIMITIVE_ALPHA",11},
            {"SHADE_ALPHA",12},{"ENV_ALPHA",13},{"LOD_FRACTION",14},
            {"PRIM_LOD_FRAC",15},{"NOISE",16},{"CENTER",17},{"SCALE",18},{"K4",19},{"K5",20}
        };

        [MenuItem("Majoras/Import Local ROM Reference")]
        static void GenerateMenu() { Generate(); }

        public static object Generate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                throw new InvalidOperationException("Stop Play Mode and wait for compilation before importing the reference.");
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty)
                    throw new InvalidOperationException("Save or close all dirty scenes before importing the local reference.");
            var sourcePath = DiskPath(Input + "/reference.json");
            var data = JsonUtility.FromJson<Document>(File.ReadAllText(sourcePath));
            Validate(data);
            var shader = Shader.Find("Majoras/ROM Reference Combiner");
            if (shader == null) throw new InvalidOperationException("ROM Reference Combiner shader has not imported.");
            EnsureFolder(Output + "/Meshes");
            EnsureFolder(Output + "/Materials");
            EnsureFolder(Output + "/Textures");
            var materials = new Material[data.materials.Length];
            var approximatedInputs = new HashSet<string>();
            int textureCount = 0;
            for (int i = 0; i < materials.Length; i++)
            {
                var source = data.materials[i];
                var material = new Material(shader) { name = source.name };
                // Blender Color values are linear. Fast64 converts Prim/Env to sRGB before
                // evaluating the N64 combiner. Keep PNG samples in the same numeric space.
                material.SetVector("_PrimColor", ToSourceColor(source.prim));
                material.SetVector("_EnvColor", ToSourceColor(source.env));
                material.SetFloat("_PrimLodFrac", source.primLodFrac);
                SetTextureCoordinates(material, source);
                SetCycle(material, source.combiner1, 1, approximatedInputs);
                SetCycle(material, source.combiner2, 2, approximatedInputs);
                material.SetFloat("_TwoCycle", source.twoCycle ? 1 : 0);
                material.SetFloat("_Lighting", source.lighting ? 1 : 0);
                material.SetFloat("_Cull", source.cullBack ? (float)CullMode.Back : (float)CullMode.Off);
                material.SetFloat("_SrcBlend", source.transparent ? (float)BlendMode.SrcAlpha : (float)BlendMode.One);
                material.SetFloat("_DstBlend", source.transparent ? (float)BlendMode.OneMinusSrcAlpha : (float)BlendMode.Zero);
                material.SetFloat("_ZWrite", source.transparent ? 0 : 1);
                material.SetOverrideTag("RenderType", source.transparent ? "Transparent" : "TransparentCutout");
                material.renderQueue = source.transparent ? (int)RenderQueue.Transparent : (int)RenderQueue.AlphaTest;
                for (int slot = 0; slot < 2; slot++)
                {
                    string filename = slot == 0 ? source.texture0 : source.texture1;
                    if (string.IsNullOrEmpty(filename)) continue;
                    var bytes = File.ReadAllBytes(TexturePath(filename));
                    var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
                    if (!ImageConversion.LoadImage(texture, bytes, false))
                        throw new InvalidDataException("Could not decode texture " + filename);
                    texture.name = source.name + " texture " + slot;
                    var wrap = slot == 0 ? source.wrap0 : source.wrap1;
                    texture.wrapModeU = Wrap(wrap[0]); texture.wrapModeV = Wrap(wrap[1]);
                    texture.filterMode = FilterMode.Bilinear;
                    texture.anisoLevel = 0;
                    texture = Store(texture, Output + "/Textures/" + i.ToString("D3") + "_" + slot + ".asset");
                    material.SetTexture(slot == 0 ? "_Tex0" : "_Tex1", texture);
                    textureCount++;
                }
                materials[i] = Store(material, Output + "/Materials/" + i.ToString("D3") + ".mat");
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("South Clock Town [LOCAL N64 REFERENCE]");
            var renderRoot = new GameObject("Render meshes"); renderRoot.transform.SetParent(root.transform, false);
            var collisionRoot = new GameObject("Original collision"); collisionRoot.transform.SetParent(root.transform, false);
            long vertexCount = 0, triangleCount = 0;
            int renderCount = 0, collisionCount = 0;
            Bounds bounds = default(Bounds); bool haveBounds = false;
            for (int i = 0; i < data.meshes.Length; i++)
            {
                var source = data.meshes[i];
                var mesh = new Mesh { name = source.name,
                    indexFormat = source.vertices.Length > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16 };
                // Exporter has already converted coordinates, winding, normals and scale.
                mesh.vertices = source.vertices;
                var normals = new Vector3[source.normals.Length];
                for (int n = 0; n < normals.Length; n++) normals[n] = source.normals[n].normalized;
                mesh.normals = normals;
                mesh.uv = source.uv;
                var colors = new Color[source.colors.Length];
                for (int c = 0; c < colors.Length; c++) colors[c] = ToSourceColor(source.colors[c]);
                mesh.colors = colors;
                mesh.subMeshCount = source.submeshes.Length;
                var bindings = new Material[source.submeshes.Length];
                for (int s = 0; s < source.submeshes.Length; s++)
                {
                    mesh.SetTriangles(source.submeshes[s].indices, s, false);
                    triangleCount += source.submeshes[s].indices.Length / 3;
                    if (!source.collision) bindings[s] = materials[source.submeshes[s].material];
                }
                mesh.RecalculateBounds();
                mesh = Store(mesh, Output + "/Meshes/" + i.ToString("D3") + ".asset");
                var go = new GameObject(source.name); go.isStatic = true;
                go.transform.SetParent(source.collision ? collisionRoot.transform : renderRoot.transform, false);
                if (source.collision)
                {
                    var collider = go.AddComponent<MeshCollider>(); collider.convex = false; collider.sharedMesh = mesh;
                    collisionCount++;
                }
                else
                {
                    go.AddComponent<MeshFilter>().sharedMesh = mesh;
                    go.AddComponent<MeshRenderer>().sharedMaterials = bindings;
                    renderCount++;
                }
                vertexCount += source.vertices.Length;
                if (!haveBounds) { bounds = mesh.bounds; haveBounds = true; }
                else bounds.Encapsulate(mesh.bounds);
            }
            var camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.tag = "MainCamera"; camera.transform.position = new Vector3(-1f, 2.3f, -4.5f);
            camera.transform.LookAt(new Vector3(-2.8f, 4.3f, 8.3f));
            camera.fieldOfView = 65; camera.nearClipPlane = .05f; camera.farClipPlane = 150;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.39f, .66f, .85f, 1);
            string scenePath = Output + "/SouthClockTown_RomReference.unity";
            if (!EditorSceneManager.SaveScene(scene, scenePath)) throw new IOException("Could not save " + scenePath);
            AssetDatabase.SaveAssets();
            var summary = new { scene = scenePath, meshes = data.meshes.Length, renderMeshes = renderCount,
                collisionMeshes = collisionCount, materials = materials.Length, textures = textureCount,
                vertices = vertexCount, triangles = triangleCount,
                boundsMin = new[] { bounds.min.x, bounds.min.y, bounds.min.z },
                boundsMax = new[] { bounds.max.x, bounds.max.y, bounds.max.z },
                approximatedCombinerInputs = new List<string>(approximatedInputs).ToArray(),
                limits = "Local static source reference; approximate lighting/filtering, no original gameplay, fog, animated material state or actor behavior." };
            Debug.Log("ROM reference imported: " + renderCount + " render meshes, " + collisionCount + " collision meshes.");
            return summary;
        }

        static Color ToSourceColor(Color color)
        {
            var result = color.gamma; result.a = color.a; return result;
        }
        static void SetTextureCoordinates(Material material, Surface surface)
        {
            // convertF3DUV stores ((signed ST / 32 + .5) / basis), with V flipped.
            // Undo that normalization in the shader before applying gsSPTexture and each tile.
            // Missing new fields preserve the previous identity UV behavior for old exports.
            if (surface.uvBasisSize.x <= 0 || surface.uvBasisSize.y <= 0) return;
            material.SetVector("_UVBasis", new Vector4(surface.uvBasisSize.x, surface.uvBasisSize.y,
                surface.texScale.x, surface.texScale.y));
            for (int slot = 0; slot < 2; slot++)
            {
                var tile = slot == 0 ? surface.tile0 : surface.tile1;
                var size = tile == null ? surface.uvBasisSize : tile.size;
                var shift = tile == null ? Vector2.zero : tile.shift;
                var low = tile == null ? Vector2.zero : tile.low;
                material.SetVector("_Tile" + slot, new Vector4(size.x, size.y, shift.x, shift.y));
                // Match Fast64's trunc_10_2, including negative origins.
                material.SetVector("_TileLow" + slot, new Vector4(
                    (float)Math.Truncate(low.x * 4) / 4, (float)Math.Truncate(low.y * 4) / 4, 0, 0));
            }
        }
        static void SetCycle(Material material, string[] source, int cycle, HashSet<string> approximated)
        {
            var values = new float[8];
            for (int i = 0; i < 8; i++)
            {
                string key = source[i];
                // Fast64 swaps physical texture slots in cycle two, including alpha sources.
                if (cycle == 2)
                    key = key == "TEXEL0" ? "TEXEL1" : key == "TEXEL1" ? "TEXEL0" :
                        key == "TEXEL0_ALPHA" ? "TEXEL1_ALPHA" : key == "TEXEL1_ALPHA" ? "TEXEL0_ALPHA" : key;
                values[i] = Sources[key];
                if (values[i] >= 14 && key != "PRIM_LOD_FRAC") approximated.Add(key);
            }
            material.SetVector("_Color" + cycle, new Vector4(values[0], values[1], values[2], values[3]));
            material.SetVector("_Alpha" + cycle, new Vector4(values[4], values[5], values[6], values[7]));
        }
        static TextureWrapMode Wrap(string value)
        {
            switch (value) { case "Clamp": return TextureWrapMode.Clamp; case "Mirror": return TextureWrapMode.Mirror;
                case "Repeat": return TextureWrapMode.Repeat; default: throw new InvalidDataException("Unknown wrap " + value); }
        }
        static string TexturePath(string filename)
        {
            if (filename != Path.GetFileName(filename) || filename.Contains("/") || filename.Contains("\\"))
                throw new InvalidDataException("Texture must be a local filename: " + filename);
            string path = DiskPath(Input + "/Textures/" + filename);
            if (!File.Exists(path)) throw new FileNotFoundException("Missing reference texture", path);
            return path;
        }
        static string DiskPath(string assetPath)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
        }
        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = path.Substring(0, path.LastIndexOf('/'));
            EnsureFolder(parent); AssetDatabase.CreateFolder(parent, path.Substring(path.LastIndexOf('/') + 1));
        }
        static T Store<T>(T asset, string path) where T : UnityEngine.Object
        {
            asset.name = Path.GetFileNameWithoutExtension(path);
            var existing = AssetDatabase.LoadMainAssetAtPath(path);
            if (existing != null)
            {
                if (!(existing is T)) throw new IOException("Unexpected asset type at generated path " + path);
                EditorUtility.CopySerialized(asset, existing); EditorUtility.SetDirty(existing);
                UnityEngine.Object.DestroyImmediate(asset); return (T)existing;
            }
            AssetDatabase.CreateAsset(asset, path); return asset;
        }
        static void Validate(Document data)
        {
            if (data == null || data.version != 1 || data.meshes == null || data.materials == null || data.meshes.Length == 0)
                throw new InvalidDataException("Unsupported or empty reference export.");
            foreach (var surface in data.materials)
            {
                if (surface == null) throw new InvalidDataException("Null reference material.");
                if (surface.uvBasisSize.x > 0 && surface.uvBasisSize.y > 0)
                {
                    foreach (float value in new[] { surface.uvBasisSize.x, surface.uvBasisSize.y, surface.texScale.x, surface.texScale.y })
                        if (float.IsNaN(value) || float.IsInfinity(value)) throw new InvalidDataException("Non-finite UV basis: " + surface.name);
                    foreach (var tile in new[] { surface.tile0, surface.tile1 })
                    {
                        if (tile == null) continue;
                        if (tile.size.x <= 0 || tile.size.y <= 0) throw new InvalidDataException("Non-positive tile size: " + surface.name);
                        foreach (float value in new[] { tile.size.x,tile.size.y,tile.shift.x,tile.shift.y,tile.low.x,tile.low.y })
                            if (float.IsNaN(value) || float.IsInfinity(value)) throw new InvalidDataException("Non-finite tile transform: " + surface.name);
                    }
                }
                foreach (var cycle in new[] { surface.combiner1, surface.combiner2 })
                {
                    if (cycle == null || cycle.Length != 8) throw new InvalidDataException("Expected eight combiner inputs: " + surface.name);
                    foreach (string input in cycle)
                        if (input == null || !Sources.ContainsKey(input)) throw new InvalidDataException("Unknown combiner input: " + input);
                }
                foreach (var wrap in new[] { surface.wrap0, surface.wrap1 })
                {
                    if (wrap == null || wrap.Length != 2) throw new InvalidDataException("Expected two wrap axes.");
                    Wrap(wrap[0]); Wrap(wrap[1]);
                }
                if (!string.IsNullOrEmpty(surface.texture0)) TexturePath(surface.texture0);
                if (!string.IsNullOrEmpty(surface.texture1)) TexturePath(surface.texture1);
            }
            foreach (var mesh in data.meshes)
            {
                if (mesh == null || mesh.vertices == null || mesh.vertices.Length == 0 || mesh.submeshes == null || mesh.submeshes.Length == 0)
                    throw new InvalidDataException("Empty reference geometry.");
                int count = mesh.vertices.Length;
                if (mesh.normals == null || mesh.normals.Length != count || mesh.colors == null || mesh.colors.Length != count || mesh.uv == null || mesh.uv.Length != count)
                    throw new InvalidDataException("Vertex attributes differ in length: " + mesh.name);
                for (int i = 0; i < count; i++)
                {
                    var p = mesh.vertices[i]; var n = mesh.normals[i]; var t = mesh.uv[i]; var c = mesh.colors[i];
                    foreach (float value in new[] { p.x,p.y,p.z,n.x,n.y,n.z,t.x,t.y,c.r,c.g,c.b,c.a })
                        if (float.IsNaN(value) || float.IsInfinity(value)) throw new InvalidDataException("Non-finite vertex attribute: " + mesh.name);
                }
                foreach (var part in mesh.submeshes)
                {
                    if (part == null || part.indices == null || part.indices.Length % 3 != 0)
                        throw new InvalidDataException("Invalid triangle list: " + mesh.name);
                    if (!mesh.collision && (part.material < 0 || part.material >= data.materials.Length))
                        throw new InvalidDataException("Missing render material: " + mesh.name);
                    foreach (int index in part.indices)
                        if (index < 0 || index >= count) throw new InvalidDataException("Triangle index outside mesh: " + mesh.name);
                }
            }
        }
    }
}
