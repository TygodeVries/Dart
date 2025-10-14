using Runtime.Graphics.Renderers;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Soil.Types
{
    public class SoilTerrainShape : SoilType
    {
        public override GameObject Build(SoilData data)
        {

            Mesh mesh = GenerateMesh();

            GameObject gameObject = new GameObject();
            gameObject.AddComponent(new MeshRenderer()
            {
                mesh = mesh
            });

            return gameObject;
        }

        Mesh GenerateMesh()
        {
            
        }
    }
}
