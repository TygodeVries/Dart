using Runtime.Calc;
using Runtime.Logging;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Project.Editor.Data
{
    public class AssetDatabase
    {
        private static Dictionary<string, string> assets = new Dictionary<string, string>();

        private static double lastRefreshTime = 0;

        private static Timer timer;
        public static void Refresh()
        {
            // Stop the old timer
            if (timer != null)
                timer.Stop();

            // Start the new timer
            timer = new Timer();
            timer.AutoReset = false;
            timer.Elapsed += RefreshLater;
            timer.Interval = 1000; // Wait one second
            timer.Start();
        }


        private static void RefreshLater(object? sender, ElapsedEventArgs args)
        {
            double currentTime = DateTime.Now.Subtract(DateTime.UnixEpoch).TotalSeconds;
            Debug.Log($"Ticks since last refresh: {currentTime - lastRefreshTime}");
            if (currentTime - lastRefreshTime < 1) // Avoid spam
            {
                return;
            }

            lastRefreshTime = currentTime;

            string projectPath = Editor.projectPath;

            if (!Directory.Exists(projectPath))
            {
                Debug.Error($"Project path not found: {projectPath}");
                return;
            }

            string[] files = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories);

            lock (assets)
            {
                assets.Clear();

                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();

                    if (ext == ".meta" || ext == ".mtl")
                        continue;

                    assets.Add(file, ext);
                }
            }

            DatabaseRefreshed?.Invoke();
        }

        public static IReadOnlyDictionary<string, string> GetAllAssets()
        {
            lock (assets)
            {
                return assets.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }

        /// <summary>
        /// #TODO make this more efficient
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetAllAssetsOfType(string type)
        {
            List<string> result = new List<string>();

            var content = GetAllAssets();
            lock (content)
            {
                foreach (var pair in content)
                {
                    if (pair.Value == type)
                    {
                        // Convert absolute → relative
                        string relativePath =
                            Path.GetRelativePath(Editor.projectPath, pair.Key);

                        result.Add(relativePath);
                    }
                }
            }

            return result;
        }


        public static event Action? DatabaseRefreshed;

        static double lastEditTime = DateTime.UnixEpoch.Ticks;

        static FileSystemWatcher? watcher;
        public static void Start()
        {
            watcher = new FileSystemWatcher(Editor.projectPath);

            watcher.IncludeSubdirectories = true;

            watcher.Created += (sender, args) => { MainThread.Run(() => { Refresh(); }); };
            watcher.Changed += (sender, args) =>
            {
                MainThread.Run(() =>
                {
                    Refresh();
                });
            };
            watcher.EnableRaisingEvents = true;

            Debug.Log("Started Watching...");
        }
    }
}
