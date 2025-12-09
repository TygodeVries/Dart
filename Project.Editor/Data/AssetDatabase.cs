using Runtime.Calc;
using Runtime.Logging;

namespace Project.Editor.Data
{
    public class AssetDatabase
    {
        private static Dictionary<string, string> assets = new Dictionary<string, string>();

        private static double lastRefreshTime = 0;

        public static void Refresh()
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
            List<string> assets = new List<string>();

            var content = GetAllAssets();
            lock (content)
            {
                foreach (KeyValuePair<string, string> pairs in content)
                {
                    if (pairs.Value == type)
                    {
                        assets.Add(pairs.Key);
                    }
                }
            }
            return assets;
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
                    Debug.Log($"{args.FullPath} was changed {args.ChangeType}");
                });
            };
            watcher.EnableRaisingEvents = true;

            Debug.Log("Started Watching...");
        }
    }
}
