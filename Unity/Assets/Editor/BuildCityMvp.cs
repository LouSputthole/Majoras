using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Majoras.City;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Majoras.Editor
{
    // All ROM-derived and generated art remains local, including the review scene.
    public static class BuildCityMvp
    {
        const string Source = "Assets/LocalReference/SouthClockTown";
        public const string Output = Source + "/Enhanced";
        public const string ScenePath = Output + "/SouthClockTown_MVP.unity";
        [Serializable] class Document { public Surface[] materials; }
        [Serializable] class Surface { public string texture0, texture1; public bool transparent; }
        static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>
        {
            {"texture_052.png", "stone_bricks.png"}, {"texture_097.png", "stone_bricks.png"},
            {"texture_041.png", "plaza_pavers.png"}, {"texture_057.png", "wall_relief.png"},
            {"texture_037.png", "golden_pavers.png"}
        };

        [MenuItem("Majoras/Build Local City MVP")]
        public static object Generate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                throw new InvalidOperationException("Generation requires a stopped, compiled editor.");
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty) throw new InvalidOperationException("Save dirty scenes first.");
            var shader = Shader.Find("Majoras/City Enhanced");
            if (shader == null || ShaderUtil.ShaderHasError(shader)) throw new InvalidOperationException("Enhanced shader is not ready.");
            PlayerSettings.productName = "Majoras - South Clock Town";
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.resizableWindow = true;
            var data = JsonUtility.FromJson<Document>(File.ReadAllText(Source + "/reference.json"));
            foreach (string folder in new[] {"Materials", "Meshes", "Samplers", "Geometry"}) Folder(Output + "/" + folder);
            var scene = EditorSceneManager.OpenScene(Source + "/Generated/SouthClockTown_RomReference.unity");
            EditorSceneManager.SaveScene(scene, ScenePath);
            var replacements = new Dictionary<Material, Material>();
            var samplers = new Dictionary<string, Texture2D>();
            int repaintedSlots = 0;
            for (int i = 0; i < data.materials.Length; i++)
            {
                var original = AssetDatabase.LoadAssetAtPath<Material>(Source + "/Generated/Materials/" + i.ToString("D3") + ".mat");
                var mat = new Material(original);
                // Alpha blended source effects retain their original combiner until individually authored.
                if (!data.materials[i].transparent) mat.shader = shader;
                mat.SetFloat("_BakedShadeStrength", .65f);
                mat.SetFloat("_Smoothness", .12f); mat.SetFloat("_ReliefStrength", 0);
                for (int slot = 0; slot < 2; slot++)
                {
                    string file = slot == 0 ? data.materials[i].texture0 : data.materials[i].texture1;
                    if (string.IsNullOrEmpty(file)) continue;
                    file = Path.GetFileName(file);
                    string replacement;
                    bool upgraded = Replacements.TryGetValue(file, out replacement);
                    string path = upgraded ? Output + "/Textures/" + replacement : Source + "/Textures/" + file;
                    var oldTexture = original.GetTexture("_Tex" + slot);
                    string key = Path.GetFileNameWithoutExtension(path) + "_" + oldTexture.wrapModeU + "_" + oldTexture.wrapModeV;
                    Texture2D texture;
                    if (!samplers.TryGetValue(key, out texture))
                    {
                        texture = new Texture2D(2, 2, TextureFormat.RGBA32, true, true);
                        if (!ImageConversion.LoadImage(texture, File.ReadAllBytes(path))) throw new IOException(path);
                        texture.wrapModeU = oldTexture.wrapModeU; texture.wrapModeV = oldTexture.wrapModeV;
                        texture.filterMode = FilterMode.Trilinear; texture.anisoLevel = 8;
                        texture.Apply(true, false);
                        texture = Store(texture, Output + "/Samplers/" + key + ".asset");
                        samplers[key] = texture;
                    }
                    mat.SetTexture("_Tex" + slot, texture);
                    if (upgraded)
                    {
                        repaintedSlots++;
                        mat.SetFloat("_ReliefStrength", replacement == "wall_relief.png" ? .12f : .2f);
                        mat.SetFloat("_ReliefSlot", slot);
                    }
                }
                replacements[original] = Store(mat, Output + "/Materials/" + i.ToString("D3") + ".mat");
            }
            var renderers = UnityEngine.Object.FindObjectsByType<MeshRenderer>();
            var bindings = new List<CityPresentation.MaterialBinding>();
            foreach (var renderer in renderers)
            {
                var originalMaterials = renderer.sharedMaterials;
                var enhancedMaterials = originalMaterials.Select(x => replacements[x]).ToArray();
                bindings.Add(new CityPresentation.MaterialBinding { renderer = renderer,
                    referenceMaterials = originalMaterials, enhancedMaterials = enhancedMaterials });
                renderer.sharedMaterials = enhancedMaterials;
                renderer.shadowCastingMode = ShadowCastingMode.TwoSided; renderer.receiveShadows = true;
                var filter = renderer.GetComponent<MeshFilter>();
                var mesh = UnityEngine.Object.Instantiate(filter.sharedMesh); mesh.RecalculateTangents();
                string originalName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(filter.sharedMesh));
                filter.sharedMesh = Store(mesh, Output + "/Meshes/" + originalName + ".asset");
                // Actor door and platform collision were separate banks in the original engine.
                if (renderer.name.StartsWith("object_tokei_step") || renderer.name.StartsWith("object_tokei_tobira"))
                    renderer.gameObject.AddComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
            }
            var enhancedRoot = new GameObject("Enhanced city artwork");
            var stone = Standard("Warm limestone", new Color(.7f,.57f,.36f), .14f);
            var wood = Standard("Dark carved timber", new Color(.24f,.12f,.055f), .16f);
            var metal = Standard("Aged bronze", new Color(.3f,.24f,.12f), .3f, .55f);
            var trim = Standard("Brass clock bead", new Color(.72f,.5f,.17f), .34f, .5f);
            CityArchitecture.Build(enhancedRoot.transform, stone, wood, metal, trim);
            var sun = new GameObject("City sunlight").AddComponent<Light>();
            sun.type = LightType.Directional; sun.shadows = LightShadows.Soft;
            sun.shadowStrength = .8f; sun.shadowBias = .025f; sun.shadowNormalBias = .12f;
            RenderSettings.sun = sun;
            QualitySettings.shadows = ShadowQuality.All; QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.shadowDistance = 65; QualitySettings.shadowCascades = 4; QualitySettings.antiAliasing = 4;
            var camera = Camera.main; camera.clearFlags = CameraClearFlags.Skybox;
            camera.allowHDR = true; camera.allowMSAA = true; camera.fieldOfView = 65;
            camera.nearClipPlane = .06f;
            var player = new GameObject("City explorer").AddComponent<CityExploration>();
            player.Configure(camera, new Vector3(-5.7f,.02f,-4.5f), 12, -10);
            player.startPaused = true;
            Threshold("North Clock Town", new Vector3(-3.2f,2,14.32f), new Vector3(-2,2,16));
            Threshold("Termina Field", new Vector3(-3.7f,0,-10), new Vector3(-1.9f,0,-6.8f));
            Threshold("East Clock Town", new Vector3(2.32f,1,7.15f), new Vector3(4,1,8.35f));
            Threshold("East Clock Town", new Vector3(3.47f,0,-3.6f), new Vector3(7.2f,1,-1.2f));
            Threshold("West Clock Town", new Vector3(-12,1,10), new Vector3(-10,2,12));
            Threshold("West Clock Town", new Vector3(-10,0,.4f), new Vector3(-8.32f,0,1.6f));
            Threshold("Laundry Pool", new Vector3(-11.2f,1,-6), new Vector3(-8.59f,1,-4.35f));
            Threshold("Clock Tower interior", new Vector3(-3.6f,0,8.07f), new Vector3(-2,0,9.67f));
            var presentation = new GameObject("City presentation").AddComponent<CityPresentation>();
            presentation.Configure(player, camera, sun, enhancedRoot, bindings.ToArray());
            presentation.daySkybox = Sky("Day sky", false); presentation.duskSkybox = Sky("Dusk sky", true);
            // Keep a true source environment snapshot; ApplyPresentation runs in Start after Awake.
            RenderSettings.skybox = null; RenderSettings.fog = false;
            RenderSettings.ambientMode = AmbientMode.Flat; RenderSettings.ambientLight = Color.gray;
            presentation.CaptureReferenceEnvironment();
            presentation.ApplyPresentation();
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            return new { scene = ScenePath, sourceRenderers = bindings.Count, sourceMaterials = replacements.Count,
                repaintedTextureSlots = repaintedSlots, uniqueEnhancedSamplers = samplers.Count,
                authoredRenderers = enhancedRoot.GetComponentsInChildren<Renderer>().Length,
                player = "First-person environment review", renderer = "Built-in, source combiner plus shadowed Standard lighting" };
        }
        static Material Sky(string name, bool dusk)
        {
            var mat = new Material(Shader.Find("Majoras/City Sky"));
            mat.SetColor("_Zenith", dusk ? new Color(.14f,.16f,.3f) : new Color(.17f,.42f,.7f));
            mat.SetColor("_Horizon", dusk ? new Color(.74f,.48f,.43f) : new Color(.7f,.84f,.87f));
            mat.SetColor("_CloudColor", dusk ? new Color(.8f,.62f,.58f) : new Color(.97f,.95f,.88f));
            mat.SetFloat("_CloudCoverage", .32f); mat.SetFloat("_CloudOpacity", .55f);
            mat.SetFloat("_SunEnabled", 0);
            return Store(mat, Output + "/Materials/" + name + ".mat");
        }
        static void Threshold(string destination, Vector3 min, Vector3 max)
        {
            var go = new GameObject("Preview boundary - " + destination);
            var bounds = new Bounds(); bounds.SetMinMax(min - Vector3.up * .05f, max + Vector3.up * 3);
            go.transform.position = bounds.center;
            var box = go.AddComponent<BoxCollider>(); box.size = bounds.size;
            var label = go.AddComponent<CityThreshold>(); label.destination = destination; label.stop = box;
        }
        static Material Standard(string name, Color color, float smoothness, float metal = 0)
        {
            var mat = new Material(Shader.Find("Standard")); mat.color = color;
            mat.SetFloat("_Glossiness", smoothness); mat.SetFloat("_Metallic", metal);
            return Store(mat, Output + "/Materials/" + name + ".mat");
        }
        static void Folder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            Folder(Path.GetDirectoryName(path).Replace('\\','/'));
            AssetDatabase.CreateFolder(Path.GetDirectoryName(path).Replace('\\','/'), Path.GetFileName(path));
        }
        static T Store<T>(T value, string path) where T : UnityEngine.Object
        {
            value.name = Path.GetFileNameWithoutExtension(path);
            var old = AssetDatabase.LoadAssetAtPath<T>(path);
            if (old == null) { AssetDatabase.CreateAsset(value, path); return value; }
            EditorUtility.CopySerialized(value, old); UnityEngine.Object.DestroyImmediate(value); return old;
        }
    }
}
