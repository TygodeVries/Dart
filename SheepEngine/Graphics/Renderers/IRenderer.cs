using Runtime.Graphics.Materials;
using Runtime.Graphics.Shaders;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Graphics.Renderers
{
    public abstract class Renderer : IComponent
    {
        protected Material material;

        public Material GetMaterial()
        {
            return material;
        }

        public abstract void Render();

        public override void OnLoad()
        {
            RenderCanvas.main.GetGraphicsPipeline().AddRenderer(this);
            base.OnLoad();
        }
    }
}
