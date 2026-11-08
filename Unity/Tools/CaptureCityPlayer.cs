// Runtime Pipeline capture for the locally built city player; excludes IMGUI.
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

if (Application.isEditor || SceneManager.GetActiveScene().name != "SouthClockTown_MVP")
    throw new InvalidOperationException("Expected the built city MVP player.");
var repo = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../.."));
if (!File.Exists(Path.Combine(repo, "Unity/Assets/Editor/BuildCityMvp.cs")))
    throw new InvalidOperationException("Expected the local Unity/Builds/CityMVP layout.");
var output = Path.Combine(repo, "local-reference/south-clock-town/mvp-windows.png");
var camera = Camera.main;
var previousTarget = camera.targetTexture;
var previousActive = RenderTexture.active;
float previousAspect = camera.aspect;
var target = RenderTexture.GetTemporary(1600, 900, 24, RenderTextureFormat.ARGB32);
Texture2D pixels = null;
try
{
    camera.targetTexture = target; camera.aspect = 1600f / 900;
    camera.Render(); RenderTexture.active = target;
    pixels = new Texture2D(1600, 900, TextureFormat.RGB24, false);
    pixels.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); pixels.Apply();
    File.WriteAllBytes(output, ImageConversion.EncodeToPNG(pixels));
}
finally
{
    camera.targetTexture = previousTarget; camera.aspect = previousAspect;
    RenderTexture.active = previousActive; RenderTexture.ReleaseTemporary(target);
    if (pixels != null) UnityEngine.Object.Destroy(pixels);
}
return new { savedPath = output, width = 1600, height = 900, batchMode = Application.isBatchMode,
    scene = SceneManager.GetActiveScene().name, source = "WindowsPlayer main camera" };
