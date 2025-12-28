using Runtime.Graphics.Renderers;

namespace Runtime.Graphics.Pipeline
{
    public interface IGraphicsPipeline
    {
        void Render();
        void Initialize() { }
        void AddRenderer(Renderer renderer) { }
        void RemoveRenderer(Renderer renderer) { }

        void AddRenderPass(RenderPass pass) { }
    }
}
