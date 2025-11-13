using Project.Editor.UI.Inspectors;
using Runtime.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    public abstract class AssetManager
    {
        public virtual Texture GetIcon(string filepath)
        {
            return Defaults.GetFallbackTexture();
        }

        public virtual void ClearCache()
        {

        }

        public abstract Inspection GetInspection();

        public static void Reset()
        {
            foreach(AssetManager fileInspector in cache.Values)
            {
                fileInspector.ClearCache();
            }

            cache = new Dictionary<string, AssetManager>();
            assetManager = null;

            GC.Collect();
        }

        private static Dictionary<string, AssetManager> cache = new Dictionary<string, AssetManager>();
        private static IEnumerable<Type>? assetManager;
        public static AssetManager GetAssetManager(string filepath)
        {
            string fileType = Path.GetExtension(filepath).ToLower();
            var inspectorType = typeof(AssetManager);

            // Get all fileInspectors
            if (assetManager == null)
            {
                assetManager =  AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => inspectorType.IsAssignableFrom(t) && !t.IsAbstract);
            }

            if(cache.ContainsKey(fileType))
            {
                return cache[fileType];
            }

            foreach (var type in assetManager)
            {
                AssetManagerAttribute? attribute = type.GetCustomAttributes(typeof(AssetManagerAttribute), false)
                               .FirstOrDefault() as AssetManagerAttribute;

                if (attribute != null && attribute.FileExtension.ToLower() == fileType)
                {
                    AssetManager fileInspector = (AssetManager)Activator.CreateInstance(type)!;
                    cache.Add(fileType, fileInspector);
                    return fileInspector;
                }
            }

            return defaultInspector;
        }

        private static DefaultAssetManager defaultInspector = new DefaultAssetManager();
    }
}
