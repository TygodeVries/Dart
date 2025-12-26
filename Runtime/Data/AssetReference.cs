using Runtime.Logging;
using System.Reflection;

namespace Runtime.Data
{
    public abstract class AssetReference
    {
        private Asset? asset;
        public Asset? GetAsset()
        {
            return asset;
        }

        public void SetAsset(Asset asset)
        {
            this.asset = asset;
        }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AssetReferenceAttribute : Attribute
    {
        public string[] filetype;
        public string createMethod;
        public AssetReferenceAttribute(string[] filetype, string createMethod)
        {
            this.createMethod = createMethod;
            this.filetype = filetype;
        }


        public object CreateInstance(Type type, Asset asset)
        {
            MethodInfo? method = type.GetMethod(createMethod);
            if (method == null)
            {
                Debug.Error($"AssetReference of type [{type.Name}] does not contain a method called {createMethod}");
                return null;
            }

            var info = method.GetParameters();
            if (info.Length != 1)
            {
                Debug.Error($"AssetReference of type [{type.Name}] create method [{createMethod}] contained more than 1, or none paramaters. It must always contain one, the file path.");
                return null;
            }

            if (!method.IsStatic)
            {
                Debug.Error($"AssetReference of type [{type.Name}] The creation method has to be static!");
            }

            // Get the created thing.
            return method.Invoke(null, new object[] { asset });
        }
    }
}
