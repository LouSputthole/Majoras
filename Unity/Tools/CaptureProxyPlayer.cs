// Run with script eval --file against the locally built Windows development player.
// Runtime Pipeline 0.10.0-exp.1 does not expose the Editor's capture command.
using System.IO;

if (Application.isEditor || Application.platform != RuntimePlatform.WindowsPlayer)
    throw new InvalidOperationException("Run against the Windows proxy player, not the Editor.");
var repo = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../.."));
if (!File.Exists(Path.Combine(repo, "shared/scenes/south-clock-town.blockout.json")))
    throw new InvalidOperationException("Expected the local Unity/Builds/Windows output layout.");
var output = Path.Combine(repo, "evidence/unity-proxy-windows.png");
var camera = Camera.main;
if (camera == null) throw new InvalidOperationException("No main camera in the built scene.");
var previousTarget = camera.targetTexture;
var previousActive = RenderTexture.active;
var previousAspect = camera.aspect;
var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.ARGB32);
Texture2D pixels = null;
try
{
    camera.targetTexture = target;
    camera.aspect = 1280f / 720f;
    camera.Render();
    RenderTexture.active = target;
    pixels = new Texture2D(1280, 720, TextureFormat.RGB24, false);
    pixels.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
    pixels.Apply();
    File.WriteAllBytes(output, ImageConversion.EncodeToPNG(pixels));
}
finally
{
    camera.targetTexture = previousTarget;
    camera.aspect = previousAspect;
    RenderTexture.active = previousActive;
    RenderTexture.ReleaseTemporary(target);
    if (pixels != null) Object.Destroy(pixels);
}
return new { savedPath = output, width = 1280, height = 720,
    source = "WindowsPlayer main camera off-screen render", batchMode = Application.isBatchMode };
