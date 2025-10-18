using Runtime.Graphics.Materials;
using Runtime.Graphics.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Runtime.Data
{
    internal class MaterialSettings
    {
        public static MaterialSettings? LoadFromFile(string file)
        {
            string fileContent = File.ReadAllText(file);
            return JsonSerializer.Deserialize<MaterialSettings>(fileContent);
        }

        public Material GetMaterial()
        {
            ShaderProgram shaderProgram = ShaderProgram.FromFile(Vertex, Fragment);
            return new Material(shaderProgram);
        }

      private MaterialSettings()
      {
         Fragment = Vertex = "";
      }

        public string Vertex {  get; set; }
        public string Fragment { get; set; }
    }
}
