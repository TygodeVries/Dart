using Runtime.Components.Core;
using Runtime.Graphics.Renderers;

namespace Runtime.Graphics.Pipeline
{
    public interface IGraphicsPipeline
    {
        void Render();
        void Initialize() { }
        void AddRenderer(Renderer renderer) { }
        void RemoveRenderer(Renderer renderer) { }
        void AddCamera(Camera camera) { }
        void AddRenderPass(RenderPass pass) { }
    }
}
