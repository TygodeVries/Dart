using Project.Editor.Code;
using Project.Editor.UI;
using Runtime.Calc;
using Runtime.Data;
using Runtime.Logging;

namespace Project.Editor
{
    public class EditorUtils
    {
        public static string projectPath = "D:\\Games\\Dart\\Assets.Example";
        public static string exeLocation = "D:\\Games\\Dart\\Runtime\\bin\\Debug\\net8.0\\runtime.exe";

        public static AssetDatabase GetAssetDatabase()
        {
            return assets;
        }
        public static void ExportGame()
        {
            Job job = new Job("Exporting Game...");

            MainThread.Run(() =>
            {
                string runtime = Path.GetDirectoryName(exeLocation);

                CopyDirectory(runtime, $"{projectPath}/Game Export/runtime/");
                CopyDirectory(projectPath, $"{projectPath}/Game Export/runtime/assets/");

                // Create Play.bat
                File.WriteAllText($"{projectPath}/Game Export/Play.bat", "runtime\\Runtime.exe \"runtime/assets\"");

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = $"{projectPath}/Game Export",
                    UseShellExecute = true, // Important for opening folders
                    Verb = "open"           // Optional, opens with default action
                });
            });

            job.Done();
        }

        static void CopyDirectory(string source, string destination)
        {

            if (source.Contains($"Game Export"))
                return;
            // #TODO This will cause issues, but it will work for now...
            if (source.Contains($"Scripts"))
                return;

            if (source.Contains(".git"))
                return;

            if (source.Contains(".vs"))
                return;

            Debug.Log($"Exporting {source}");
            Directory.CreateDirectory(destination);

            foreach (var file in Directory.GetFiles(source))
            {
                var destFile = Path.Combine(destination, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (var dir in Directory.GetDirectories(source))
            {
                var destDir = Path.Combine(destination, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }


        static AssetDatabase assets;
        public static void LoadAssetDatabase()
        {
            assets = new AssetDatabase(Directory.GetCurrentDirectory());
            assets.Start();
        }

        /// <summary>
        /// #TODO: Add settings
        /// </summary>
        public static void OpenScriptEditor()
        {
            System.Diagnostics.Process.Start("CMD.exe", $"/c start \"\" \"{projectPath}/scripts/Game.csproj\"");
        }

        static System.Diagnostics.Process? gameProcess;

        /// <summary>
        /// Load up user's game executable.
        /// </summary>
        public static void StartGame()
        {
            Compiler.ScheduleBuild(() =>
            {
                Job job = new Job("Starting Game...");
                Debug.Log("Starting Game...");

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = exeLocation,
                    Arguments = projectPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                gameProcess = new System.Diagnostics.Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                // Subscribe to output events
                gameProcess.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                        return;
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.Log("[Game] " + e.Data);

                    // Very safe!

                    if (e.Data.Contains("Opening window"))
                        job.Done();
                };

                gameProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.Error("[Game Error] " + e.Data);
                };

                gameProcess.Exited += GameProcess_Exited;

                if (!gameProcess.Start())
                {
                    Debug.Error("Could not start game!");
                    return;
                }

                // Begin async reading of stdout/stderr
                gameProcess.BeginOutputReadLine();
                gameProcess.BeginErrorReadLine();
            });

        }

        /// <summary>
        /// Get's called from the game's running executable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void GameProcess_Exited(object? sender, EventArgs e)
        {
            Debug.Log("Game Closed. (Process Exited)");
            gameProcess?.Dispose();
            gameProcess = null;
        }

        /// <summary>
        /// Stop the game if its running
        /// </summary>
        public static void StopGame()
        {
            gameProcess?.Kill();
            gameProcess?.Dispose();
            gameProcess = null;
        }

        /// <summary>
        /// If the user's game is currently running
        /// </summary>
        /// <returns>True if its running, false if its not.</returns>
        public static bool IsGameRunning()
        {
            return gameProcess == null;
        }
    }
}
