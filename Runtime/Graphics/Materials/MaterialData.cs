using Runtime.Graphics.Shaders;
using Runtime.Logging;
using System.Text.Json;

namespace Runtime.Graphics.Materials
{
    public class MaterialData
    {
        public string FilePath;

        public string VertexShader { get; set; }

        public string FragmentShader { get; set; }

        public bool Lit { get; set; }

        public List<MaterialDataField> DataFields { get; set; }

        public Material CreateMaterial(string workingDir = "")
        {
            Material material = new Material(ShaderProgram.FromFile(Path.Combine(workingDir, VertexShader), Path.Combine(workingDir, FragmentShader)));

            if (Lit)
                material.EnableLightData();
            return material;
        }

        public static MaterialData FromJson(string file)
        {
            if (!File.Exists(file))
            {
                Debug.Error($"Tried to load a file, but the file did not exist {file}");
                // #TODO go to a backup material
            }

            string json = File.ReadAllText(file);
            MaterialData data = JsonSerializer.Deserialize<MaterialData>(json);
            data.FilePath = file;
            return data;
        }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this));
        }
    }

    public class MaterialDataField
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
