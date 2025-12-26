using Runtime.Calc;
using Runtime.Data;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
using System.Globalization;
using System.Text.Json;

namespace Runtime.Graphics.Materials
{
    public class MaterialData
    {
        public required string FilePath;

        public required string VertexShader { get; set; }

        public required string FragmentShader { get; set; }

        public required bool Lit { get; set; }

        public required List<MaterialDataField> DataFields { get; set; }

        public Material CreateMaterial(AssetDatabase database)
        {
            Material material = new Material(ShaderProgram.FromFile(database.GetAsset(VertexShader), database.GetAsset(FragmentShader)));

            if (Lit)
                material.EnableLightData();

            int textureIds = 0;
            foreach (MaterialDataField field in DataFields)
            {
                if (field.Type == "vec4")
                {
                    material.SetVector4(field.Name, Vector4.Parse(field.Value));
                }
                else if (field.Type == "vec3")
                {
                    material.SetVector3(field.Name, Vector3.Parse(field.Value));
                }
                else if (field.Type == "float")
                {
                    material.SetFloat(field.Name, float.Parse(field.Value, CultureInfo.InvariantCulture));
                }
                else if (field.Type == "sampler2D")
                {
                    material.SetTexture(field.Name, Texture.LoadFromPng(database.GetAsset(field.Value)), textureIds);
                    textureIds++;
                }
            }

            return material;
        }

        public static MaterialData FromJson(Asset asset)
        {
            string file = asset.GetSystemPath();
            if (!File.Exists(file))
            {
                Debug.Error($"Tried to load a file, but the asset file did not exist {file}");
                // #TODO go to a backup material
            }

            string json = File.ReadAllText(file);
            MaterialData? data = JsonSerializer.Deserialize<MaterialData>(json);

            if (data == null)
                Debug.Error("Could not load MaterialData: Null!");

            data!.FilePath = file;
            return data;
        }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this));
        }
    }


    // #TODO maybe later replace this with ValueRecord?
    public class MaterialDataField
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Value { get; set; }
    }
}
