using ImGuiNET;
using Runtime.DearImGUI.Backend;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Logging;
using Runtime.Plugins;

namespace Runtime.DearImGUI
{
    public class Main : Plugin
    {
        public Main()
        {

        }

        public override void Load()
        {
            RenderCanvas.main!.RenderPipelineSet += () =>
            {
                Debug.Log("Loaded into render pipeline...");
                IGraphicsPipeline? gp = RenderCanvas.main!.GetGraphicsPipeline();
                gp?.AddRenderPass(new ImGuiRenderPass());
            };
        }
    }
}
