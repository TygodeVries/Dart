using Runtime.Calc;
using Runtime.Logging;
using Timer = System.Timers.Timer;

namespace Runtime.Data
{
    public class AssetDatabase
    {
        string activeFolder;

        /// <summary>
        /// Get the root folder of the asset database.
        /// </summary>
        /// <returns></returns>
        public string GetFolder()
        {
            return activeFolder;
        }

        /// <summary>
        /// Get the system path of an asset, from a relative path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetAssetPath(string path)
        {
            return Path.Join(GetFolder(), path);
        }

        /// <summary>
        /// Get an asset from the asset database
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Asset GetAsset(string path)
        {
            return new Asset(this, path);
        }

        public AssetDatabase(string activeFolder)
        {
            this.activeFolder = activeFolder;
        }

        private Dictionary<string, string> assets = new Dictionary<string, string>();
        private double lastRefreshTime = 0;
        private Timer timer;

        public void Refresh()
        {
            // Stop the old timer
            if (timer != null)
                timer.Stop();

            // Start the new timer
            timer = new Timer();
            timer.AutoReset = false;
            timer.Elapsed += (e, a) => { MainThread.Run(() => { RefreshNow(); }); };
            timer.Interval = 1000; // Wait one second
            timer.Start();
        }


        public void RefreshNow()
        {
            if (timer != null)
                timer.Stop();
            double currentTime = DateTime.Now.Subtract(DateTime.UnixEpoch).TotalSeconds;
            Debug.Log($"Ticks since last refresh: {currentTime - lastRefreshTime}");
            if (currentTime - lastRefreshTime < 1) // Avoid spam
            {
                return;
            }

            lastRefreshTime = currentTime;


            if (!Directory.Exists(activeFolder))
            {
                Debug.Error($"Project path not found: {activeFolder}");
                return;
            }

            string[] files = Directory.GetFiles(activeFolder, "*.*", SearchOption.AllDirectories);

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
            changes.Clear();
        }

        public IReadOnlyDictionary<string, string> GetAllAssets()
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
        public List<Asset> GetAllAssetsOfType(string type)
        {
            List<Asset> result = new List<Asset>();

            var content = GetAllAssets();
            lock (content)
            {
                foreach (var pair in content)
                {
                    if (pair.Value == type)
                    {
                        result.Add(Asset.FromSystemPath(this, pair.Key));
                    }
                }
            }

            return result;
        }


        public event Action? DatabaseRefreshed;
        FileSystemWatcher? watcher;
        public void Start()
        {
            watcher = new FileSystemWatcher(activeFolder);

            watcher.IncludeSubdirectories = true;

            watcher.Created += (sender, args) =>
            {
                MainThread.Run(() =>
                {
                    changes.Add(args.FullPath);
                    Refresh();
                });
            };
            watcher.Changed += (sender, args) =>
            {
                MainThread.Run(() =>
                {
                    changes.Add(args.FullPath);
                    Refresh();
                });
            };
            watcher.EnableRaisingEvents = true;

            Debug.Log("Started Watching...");
            Refresh();

        }

        public List<string> changes = new List<string>();
    }
}
