using Runtime.Data;
using Runtime.Graphics;
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

        static AssetDatabase assets;
        public static void LoadAssetDatabase()
        {
            assets = new AssetDatabase(Directory.GetCurrentDirectory());
            assets.Start();
        }

        static System.Diagnostics.Process? gameProcess;

        /// <summary>
        /// Load up user's game executable.
        /// </summary>
        public static void StartGame()
        {
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
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.Log("[Game] " + e.Data);
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
        }

        public static void ShowProgressBar()
        {
            .SetValue(RenderCanvas.main.Context.WindowPtr, 50, 100);
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
