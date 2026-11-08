using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Majoras.City
{
    [DisallowMultipleComponent]
    public sealed class CityPresentation : MonoBehaviour
    {
        [Serializable]
        public sealed class MaterialBinding
        {
            public Renderer renderer;
            public Material[] referenceMaterials;
            public Material[] enhancedMaterials;
        }
        public CityExploration exploration;
        public Camera viewCamera;
        public Light sun;
        public GameObject enhancedOnly;
        public MaterialBinding[] bindings = new MaterialBinding[0];
        public Material daySkybox, duskSkybox;
        public bool enhanced = true;
        public bool hudVisible = true;
        public int lightingPreset;
        public bool referenceUsesOriginalLighting = true;
        public string areaLabel = "SOUTH CLOCK TOWN";
        public event Action<bool> ComparisonChanged;
        public event Action<int> LightingChanged;
        public bool Enhanced => enhanced;
        public int LightingPreset => lightingPreset;

        [SerializeField] bool captured;
        bool beganExploring;
        [SerializeField] bool referenceFog;
        [SerializeField] FogMode referenceFogMode;
        [SerializeField] Color referenceBackground, referenceAmbient, referenceFogColor;
        [SerializeField] float referenceFogStart, referenceFogEnd, referenceFogDensity, referenceSunIntensity;
        [SerializeField] AmbientMode referenceAmbientMode;
        [SerializeField] Material referenceSkybox;
        [SerializeField] Color referenceSunColor;
        [SerializeField] Quaternion referenceSunRotation;
        GUIStyle eyebrow, body, title, button, hint;
        float noticeUntil;
        string notice;
        string nearbyDistrict;
        float nextThresholdCheck;

        void Awake()
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            if (exploration == null) exploration = FindAnyObjectByType<CityExploration>();
            if (viewCamera == null) viewCamera = exploration != null ? exploration.viewCamera : Camera.main;
            if (exploration != null)
            {
                exploration.handleClickToResume = false;
                exploration.PauseChanged += OnPauseChanged;
            }
            if (!captured) CaptureReferenceEnvironment();
        }
        void Start() { ApplyPresentation(); }
        void OnDestroy()
        {
            if (exploration != null) exploration.PauseChanged -= OnPauseChanged;
        }
        void OnPauseChanged(bool paused) { if (!paused) beganExploring = true; }
        void Update()
        {
            if (Time.unscaledTime >= nextThresholdCheck)
            {
                nearbyDistrict = CityThreshold.Nearby(exploration);
                nextThresholdCheck = Time.unscaledTime + .2f;
            }
            if (Input.GetKeyDown(KeyCode.Tab)) ToggleComparison();
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetLightingPreset(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetLightingPreset(1);
            if (Input.GetKeyDown(KeyCode.F1)) hudVisible = !hudVisible;
            if (Input.GetKeyDown(KeyCode.R) && exploration != null)
            {
                exploration.Respawn(); ShowNotice("Returned to the south entrance");
            }
        }
        public void Configure(CityExploration player, Camera camera, Light directionalLight,
            GameObject enhancedRoot, MaterialBinding[] materialBindings)
        {
            if (exploration != null) exploration.PauseChanged -= OnPauseChanged;
            exploration = player; viewCamera = camera; sun = directionalLight;
            enhancedOnly = enhancedRoot; bindings = materialBindings ?? new MaterialBinding[0];
            if (exploration != null)
            {
                exploration.handleClickToResume = false;
                exploration.PauseChanged += OnPauseChanged;
            }
            captured = false;
        }
        public void CaptureReferenceEnvironment()
        {
            referenceBackground = viewCamera != null ? viewCamera.backgroundColor : CityPalette.DaySky;
            referenceAmbientMode = RenderSettings.ambientMode; referenceAmbient = RenderSettings.ambientLight;
            referenceFog = RenderSettings.fog; referenceFogMode = RenderSettings.fogMode;
            referenceFogColor = RenderSettings.fogColor; referenceFogStart = RenderSettings.fogStartDistance;
            referenceFogEnd = RenderSettings.fogEndDistance; referenceFogDensity = RenderSettings.fogDensity;
            referenceSkybox = RenderSettings.skybox;
            if (sun != null)
            {
                referenceSunIntensity = sun.intensity; referenceSunColor = sun.color;
                referenceSunRotation = sun.transform.rotation;
            }
            captured = true;
        }
        public void ToggleComparison() { SetEnhanced(!enhanced); }
        public void SetEnhanced(bool value)
        {
            enhanced = value; ApplyPresentation(); ComparisonChanged?.Invoke(value);
            ShowNotice(value ? "Enhanced presentation" : "N64 reference presentation");
            Debug.Log("[CityPresentation] Comparison: " + (value ? "enhanced" : "reference"));
        }
        public void SetLightingPreset(int preset)
        {
            if (preset < 0 || preset > 1) throw new ArgumentOutOfRangeException(nameof(preset), "0 = day, 1 = dusk.");
            lightingPreset = preset; ApplyPresentation(); LightingChanged?.Invoke(preset);
            ShowNotice(!enhanced && referenceUsesOriginalLighting ? "Reference retains source noon lighting" : preset == 0 ? "Daylight" : "Dusk");
            Debug.Log("[CityPresentation] Lighting preset: " + preset);
        }
        public void SetHudVisible(bool visible) { hudVisible = visible; }
        public void ApplyPresentation()
        {
            if (!captured) CaptureReferenceEnvironment();
            if (enhancedOnly != null) enhancedOnly.SetActive(enhanced);
            foreach (var binding in bindings ?? new MaterialBinding[0])
            {
                if (binding == null || binding.renderer == null) continue;
                var selection = enhanced ? binding.enhancedMaterials : binding.referenceMaterials;
                if (selection != null && selection.Length > 0) binding.renderer.sharedMaterials = selection;
            }
            if (!enhanced && referenceUsesOriginalLighting) RestoreReferenceEnvironment();
            else ApplyLighting(lightingPreset == 1);
        }
        void RestoreReferenceEnvironment()
        {
            if (viewCamera != null) viewCamera.backgroundColor = referenceBackground;
            RenderSettings.skybox = referenceSkybox;
            RenderSettings.ambientMode = referenceAmbientMode; RenderSettings.ambientLight = referenceAmbient;
            RenderSettings.fog = referenceFog; RenderSettings.fogMode = referenceFogMode;
            RenderSettings.fogColor = referenceFogColor; RenderSettings.fogStartDistance = referenceFogStart;
            RenderSettings.fogEndDistance = referenceFogEnd; RenderSettings.fogDensity = referenceFogDensity;
            if (sun != null)
            {
                sun.intensity = referenceSunIntensity; sun.color = referenceSunColor;
                sun.transform.rotation = referenceSunRotation;
            }
        }
        void ApplyLighting(bool dusk)
        {
            if (viewCamera != null) viewCamera.backgroundColor = dusk ? CityPalette.DuskSky : CityPalette.DaySky;
            RenderSettings.skybox = dusk ? duskSkybox : daySkybox;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = dusk ? CityPalette.DuskAmbient : CityPalette.DayAmbient;
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = dusk ? CityPalette.DuskSky : CityPalette.DaySky;
            RenderSettings.fogStartDistance = dusk ? 28 : 38; RenderSettings.fogEndDistance = dusk ? 85 : 115;
            if (sun != null)
            {
                sun.color = dusk ? new Color(1, .67f, .44f) : new Color(1, .92f, .77f);
                sun.intensity = dusk ? .58f : .78f;
                sun.transform.rotation = Quaternion.Euler(dusk ? 18 : 48, dusk ? -58 : -35, 0);
            }
            // Enhanced materials must be distinct assets from their reference counterparts.
            // Only set optional properties supported by the root-authored shader.
            foreach (var binding in bindings ?? new MaterialBinding[0])
            {
                if (binding == null || binding.enhancedMaterials == null) continue;
                foreach (var material in binding.enhancedMaterials)
                {
                    if (material == null) continue;
                    if (material.HasProperty("_Ambient")) material.SetFloat("_Ambient", dusk ? .4f : .68f);
                    if (material.HasProperty("_Diffuse")) material.SetFloat("_Diffuse", dusk ? .35f : .32f);
                    if (material.HasProperty("_LightDirection"))
                    {
                        Vector3 direction = sun != null ? -sun.transform.forward : new Vector3(-.4f, .8f, -.3f).normalized;
                        material.SetVector("_LightDirection", new Vector4(direction.x, direction.y, direction.z, 0));
                    }
                    if (material.HasProperty("_LightTint")) material.SetColor("_LightTint", dusk ? new Color(1, .76f, .65f) : Color.white);
                }
            }
        }
        void ShowNotice(string value) { notice = value; noticeUntil = Time.unscaledTime + 2.4f; }

        void BuildStyles()
        {
            eyebrow = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            eyebrow.normal.textColor = CityPalette.Brass;
            title = new GUIStyle(GUI.skin.label) { fontSize = 32, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            title.normal.textColor = CityPalette.Paper;
            body = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true, alignment = TextAnchor.UpperLeft };
            body.normal.textColor = CityPalette.Muted;
            hint = new GUIStyle(body) { fontSize = 12, alignment = TextAnchor.MiddleCenter, wordWrap = false };
            button = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            button.normal.textColor = CityPalette.Paper;
        }
        void OnGUI()
        {
            if (eyebrow == null) BuildStyles();
            var oldMatrix = GUI.matrix; var oldColor = GUI.color;
            float scale = Mathf.Clamp(Mathf.Min(Screen.width / 1440f, Screen.height / 900f), .72f, 1.35f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            float width = Screen.width / scale, height = Screen.height / scale;
            try
            {
                bool paused = exploration != null && exploration.IsPaused;
                if (hudVisible && !paused)
                {
                    Rect panel = new Rect(28, 25, 302, 68);
                    Fill(panel, CityPalette.Alpha(CityPalette.Ink, .84f));
                    Fill(new Rect(panel.x, panel.y, 3, panel.height), CityPalette.Brass);
                    GUI.Label(new Rect(46, 34, 268, 23), areaLabel, eyebrow);
                    GUI.Label(new Rect(46, 58, 268, 23), enhanced ? "Enhanced  /  " + (lightingPreset == 0 ? "Daylight" : "Dusk") : "N64 reference  /  source noon", body);
                    string controls = width > 1050 ? "WASD  move     MOUSE  look     SHIFT  run     SPACE  jump     TAB  compare     1 / 2  light     R  return     ESC  pause"
                        : "WASD move  ·  MOUSE look  ·  SHIFT run  ·  TAB compare  ·  ESC pause";
                    Fill(new Rect(24, height - 59, width - 48, 34), CityPalette.Alpha(CityPalette.Ink, .78f));
                    GUI.Label(new Rect(32, height - 59, width - 64, 34), controls, hint);
                    Fill(new Rect(width * .5f - 1, height * .5f - 1, 2, 2), CityPalette.Alpha(CityPalette.Paper, .65f));
                    if (!string.IsNullOrEmpty(notice) && Time.unscaledTime < noticeUntil)
                        GUI.Label(new Rect(width * .5f - 240, height - 106, 480, 28), notice, hint);
                    else if (!string.IsNullOrEmpty(nearbyDistrict))
                    {
                        Fill(new Rect(width * .5f - 245, height - 108, 490, 32), CityPalette.Alpha(CityPalette.Ink, .9f));
                        GUI.Label(new Rect(width * .5f - 240, height - 106, 480, 28), nearbyDistrict + "  |  Beyond this preview", hint);
                    }
                }
                if (paused)
                {
                    Fill(new Rect(0, 0, width, height), CityPalette.Alpha(CityPalette.Ink, .68f));
                    float panelWidth = Mathf.Min(510, width - 48);
                    Rect panel = new Rect((width - panelWidth) / 2, Mathf.Max(24, (height - 408) / 2), panelWidth, 408);
                    Fill(panel, CityPalette.Alpha(CityPalette.Deep, .98f));
                    Fill(new Rect(panel.x, panel.y, panel.width, 2), CityPalette.Brass);
                    float x = panel.x + 30, y = panel.y + 25, content = panel.width - 60;
                    GUI.Label(new Rect(x, y, content, 24), beganExploring ? "EXPLORATION PAUSED" : "A WALK THROUGH TERMINA", eyebrow);
                    GUI.Label(new Rect(x, y + 30, content, 48), "South Clock Town", title);
                    GUI.Label(new Rect(x, y + 83, content, 54), "Explore the square, look up at the tower, and compare the original scene with its enhanced presentation.", body);
                    if (Button(new Rect(x, y + 151, content, 43), beganExploring ? "RESUME EXPLORING" : "BEGIN EXPLORING", true)) Resume();
                    if (Button(new Rect(x, y + 205, content, 39), "RETURN TO SOUTH ENTRANCE", false)) { exploration.Respawn(); Resume(); }
                    if (Button(new Rect(x, y + 255, content, 39), enhanced ? "VIEW N64 REFERENCE" : "VIEW ENHANCED", false)) ToggleComparison();
                    if (Button(new Rect(x, y + 307, content * .48f, 34), "DAYLIGHT  [1]", false)) SetLightingPreset(0);
                    if (Button(new Rect(x + content * .52f, y + 307, content * .48f, 34), "DUSK  [2]", false)) SetLightingPreset(1);
                    GUI.Label(new Rect(x, y + 351, content, 22), "ESC to resume  ·  click the scene to explore", hint);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !panel.Contains(Event.current.mousePosition))
                    { Resume(); Event.current.Use(); }
                }
            }
            finally { GUI.matrix = oldMatrix; GUI.color = oldColor; }
        }
        public void Resume()
        {
            beganExploring = true;
            if (exploration != null) exploration.SetPaused(false);
        }
        static void Fill(Rect rect, Color color)
        {
            var previous = GUI.color; GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = previous;
        }
        bool Button(Rect rect, string text, bool primary)
        {
            bool hover = rect.Contains(Event.current.mousePosition);
            Fill(rect, primary ? CityPalette.Alpha(CityPalette.Teal, hover ? .75f : .52f) : CityPalette.Alpha(CityPalette.Ink, hover ? .8f : .45f));
            Fill(new Rect(rect.x, rect.yMax - 1, rect.width, 1), CityPalette.Alpha(primary ? CityPalette.Teal : CityPalette.Muted, .42f));
            return GUI.Button(rect, text, button);
        }
    }
}
