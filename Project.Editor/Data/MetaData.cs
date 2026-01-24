using Project.Editor.UI.Inspectors;
using Runtime.Calc;
using Runtime.Data;
using Runtime.Logging;
using System.Globalization;
using System.Text.Json;

namespace Project.Editor.Data
{
    /// <summary>
    /// #TODO Optimize this class more, this is a temp version to start on other things, but this will be called often, and should be better then this.
    /// After writing most of this class, it might be better todo everything on a byte scale, but that might cause terrible git merges
    /// </summary>
    public class MetaData
    {
        private static Dictionary<string, MetaData> metaDataCache = new Dictionary<string, MetaData>();
        public static MetaData GetAssetMeta(Asset asset)
        {

            string path = asset.GetSystemPath();
            if (metaDataCache.ContainsKey(path))
            {
                return metaDataCache[path];
            }

            FileAttributes fileAttributes = File.GetAttributes(path);

            string metaDataFilePath;
            if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                metaDataFilePath = Path.Join(path, "folder.meta");
            }
            else
            {
                metaDataFilePath = path + ".meta";
            }

            MetaData metaData = new MetaData(Asset.FromSystemPath(asset.GetDatabase(), metaDataFilePath));
            metaData.Load();
            return metaData;
        }

        public Vector4 GetVector4(string key, Vector4 def = default)
        {
            if (data == null)
                Load();

            if (data!.ContainsKey(key))
            {
                string value = data[key];
                string[] args = value.Split(' ');
                Vector4 vector4 = new Vector4();
                vector4.x = float.Parse(args[0], CultureInfo.InvariantCulture);
                vector4.y = float.Parse(args[1], CultureInfo.InvariantCulture);
                vector4.z = float.Parse(args[2], CultureInfo.InvariantCulture);
                vector4.w = float.Parse(args[3], CultureInfo.InvariantCulture);
                return vector4;
            }

            return def;
        }

        public void SetVector4(string path, Vector4 val)
        {
            if (data == null)
                Load();

            data![path] = $"{val.x.ToString(CultureInfo.InvariantCulture)} {val.y.ToString(CultureInfo.InvariantCulture)} {val.z.ToString(CultureInfo.InvariantCulture)} {val.w}";
        }

        Asset asset;
        public MetaData(Asset asset)
        {
            this.asset = asset;
        }

        private Dictionary<string, string>? data;

        public void Load()
        {
            string path = asset.GetSystemPath();
            data = null;
            if (File.Exists(path))
            {
                string fileContent = File.ReadAllText(path);
                try
                {
                    data = JsonSerializer.Deserialize<Dictionary<string, string>>(fileContent);
                }
                catch (Exception ex)
                {
                    Debug.Error($"Failed to load metadata: {ex}. Resetting, sorry!");
                    data = new Dictionary<string, string>();
                    Save();
                }
            }

            if (data == null)
            {
                data = new Dictionary<string, string>();
                Save();
                Debug.Log($"Created new metadata on {path}");
            }
        }

        public void Save()
        {
            string path = asset.GetSystemPath();
            try
            {
                File.WriteAllText(path, JsonSerializer.Serialize(data));
            }
            catch (Exception e)
            {
                Debug.Error("Failed to save meta data for: " + path + " because " + e);
                InspectorWindow.GetActive().SetInspection(null);
            }
        }
    }

    public enum MetaDataType
    {
        Vector4
    }
}
