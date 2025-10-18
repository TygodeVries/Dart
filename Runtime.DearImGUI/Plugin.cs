using Runtime.DearImGUI.Backend;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Plugins;

namespace Runtime.DearImGUI
{
    public class Main : Plugin
    {
        public override void Load()
        {
            if(RenderCanvas.main!.GetGraphicsPipeline() is DefaultGraphicsPipeline pipeline)
            {
                pipeline.AddRenderPass(new ImGuiRenderPass());
            }
        }
    }
}
