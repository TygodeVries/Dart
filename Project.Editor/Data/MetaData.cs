using Runtime.Logging;
using System.Globalization;
using System.Numerics;
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
        public static MetaData Get(string path)
        {
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

            MetaData metaData = new MetaData(metaDataFilePath);
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
                vector4.X = float.Parse(args[0], CultureInfo.InvariantCulture);
                vector4.Y = float.Parse(args[1], CultureInfo.InvariantCulture);
                vector4.Z = float.Parse(args[2], CultureInfo.InvariantCulture);
                vector4.W = float.Parse(args[3], CultureInfo.InvariantCulture);
                return vector4;
            }

            return def;
        }

        public void SetVector4(string path, Vector4 val)
        {
            if (data == null)
                Load();

            data![path] = $"{val.X.ToString(CultureInfo.InvariantCulture)} {val.Y.ToString(CultureInfo.InvariantCulture)} {val.Z.ToString(CultureInfo.InvariantCulture)} {val.W}";
        }

        string path;
        public MetaData(string path)
        {
            this.path = path;
        }

        private Dictionary<string, string>? data;

        public void Load()
        {
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
            File.WriteAllText(path, JsonSerializer.Serialize(data));
        }
    }

    public enum MetaDataType
    {
        Vector4
    }
}
