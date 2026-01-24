using Project.Editor.UI.Inspectors;
using Runtime.Data;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    public abstract class AssetManager
    {
        protected Asset? asset;

        public Asset? GetAsset()
        {
            return asset;
        }
        public virtual Texture GetIcon()
        {
            return DefaultsTextures.GetFallbackTexture();
        }

        public virtual void OnOpen() { }
        public virtual void ClearCache() { }

        public abstract Inspection GetInspection();

        public static void Clear()
        {
            foreach (AssetManager fileInspector in cache.Values)
            {
                fileInspector.ClearCache();
            }

            cache = new Dictionary<string, AssetManager>();
            assetManager = null;

            GC.Collect();
        }

        public static void Init()
        {
            UserCode.OnAttemptUnload += Clear;
        }

        private static Dictionary<string, AssetManager> cache = new Dictionary<string, AssetManager>();
        private static IEnumerable<Type>? assetManager;
        public static AssetManager GetAssetManager(Asset asset)
        {
            string fileType = Path.GetExtension(asset.GetPath()).ToLower();
            var inspectorType = typeof(AssetManager);

            // Get all fileInspectors
            if (assetManager == null)
            {
                assetManager = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => inspectorType.IsAssignableFrom(t) && !t.IsAbstract);
            }

            if (cache.ContainsKey(fileType))
            {
                cache[fileType].asset = asset;
                return cache[fileType];
            }

            foreach (var type in assetManager)
            {
                AssetManagerAttribute? attribute = type.GetCustomAttributes(typeof(AssetManagerAttribute), false)
                               .FirstOrDefault() as AssetManagerAttribute;

                if (attribute != null && attribute.FileExtension.ToLower() == fileType)
                {
                    AssetManager fileInspector = (AssetManager)Activator.CreateInstance(type)!;
                    ObjectTracker.Track(fileInspector);
                    cache.Add(fileType, fileInspector);
                    cache[fileType].asset = asset;
                    return fileInspector;
                }
            }

            return defaultInspector;
        }

        private static DefaultAssetManager defaultInspector = new DefaultAssetManager();
    }
}
