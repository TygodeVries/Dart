using Runtime.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    public abstract class FileInspector
    {
        public virtual Texture GetIcon(string filepath)
        {
            return Defaults.GetFallbackTexture();
        }

        public virtual void ClearCache()
        {

        }

        public static void Reset()
        {
            foreach(FileInspector fileInspector in cache.Values)
            {
                fileInspector.ClearCache();
            }

            cache = new Dictionary<string, FileInspector>();
            fileInspectors = null;

            GC.Collect();
        }

        private static Dictionary<string, FileInspector> cache = new Dictionary<string, FileInspector>();
        private static IEnumerable<Type>? fileInspectors;
        public static FileInspector GetInspector(string filepath)
        {
            string fileType = Path.GetExtension(filepath).ToLower();
            var inspectorType = typeof(FileInspector);

            // Get all fileInspectors
            if (fileInspectors == null)
            {
                fileInspectors =  AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => inspectorType.IsAssignableFrom(t) && !t.IsAbstract);
            }

            if(cache.ContainsKey(fileType))
            {
                return cache[fileType];
            }

            foreach (var type in fileInspectors)
            {
                FileInspectorAttribute? attribute = type.GetCustomAttributes(typeof(FileInspectorAttribute), false)
                               .FirstOrDefault() as FileInspectorAttribute;

                if (attribute != null && attribute.FileExtension.ToLower() == fileType)
                {
                    FileInspector fileInspector = (FileInspector)Activator.CreateInstance(type)!;
                    cache.Add(fileType, fileInspector);
                    return fileInspector;
                }
            }

            return defaultInspector;
        }

        private static DefaultFileInspector defaultInspector = new DefaultFileInspector();
    }
}
