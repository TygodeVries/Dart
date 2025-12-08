using Runtime.Logging;

namespace Project.Editor.Data
{
    public class AssetDatabase
    {
        private static Dictionary<string, string> assets = new Dictionary<string, string>();

        public static void Refresh()
        {
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

        public static event Action DatabaseRefreshed;

        static double lastEditTime = DateTime.UnixEpoch.Ticks;

        static FileSystemWatcher watcher;
        public static void Start()
        {
            watcher = new FileSystemWatcher(Editor.projectPath);

            watcher.IncludeSubdirectories = true;

            watcher.Created += (sender, args) => { Refresh(); };
            watcher.Changed += (sender, args) => { Refresh(); };
            watcher.EnableRaisingEvents = true;

            Debug.Log("Started Watching...");
        }
    }
}
