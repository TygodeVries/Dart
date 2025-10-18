using Runtime.Graphics.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Graphics.Pipeline
{
    public interface IGraphicsPipeline
    {
        void Render();
        void Initialize() { }
        void AddRenderer(Renderer renderer) { }

        void AddRenderPass(RenderPass pass) { }
    }
}
