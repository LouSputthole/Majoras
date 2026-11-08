// Launch the local review build from the Editor for an interactive review.
using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;
if (!Application.isEditor) throw new InvalidOperationException("Editor only.");
var build = Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds/CityMVP"));
var executable = Path.Combine(build, "MajorasCityMVP.exe");
if (!File.Exists(executable)) throw new FileNotFoundException(executable);
var log = Path.GetFullPath(Path.Combine(build, "../../../local-reference/south-clock-town/city-player-interactive.log"));
var process = Process.Start(new ProcessStartInfo(executable,
    "-screen-fullscreen 0 -screen-width 1280 -screen-height 720 -logFile \"" + log + "\"")
    { WorkingDirectory = build, UseShellExecute = true, WindowStyle = ProcessWindowStyle.Normal });
return new { processId = process.Id, executable, log };
