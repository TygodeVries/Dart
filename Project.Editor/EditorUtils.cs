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
                UseShellExecute = true
            };

            gameProcess = System.Diagnostics.Process.Start(startInfo);
            if (gameProcess == null)
            {
                Debug.Error("Could not start game!");
                return;
            }

            gameProcess.EnableRaisingEvents = true;
            gameProcess.Exited += GameProcess_Exited;
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
