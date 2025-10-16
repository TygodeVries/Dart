using Editor.ImGuiEditor;
using Runtime;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class TestKick
    {
        public TestKick()
        {
            Debug.Log("Running user code!");
            Game.onReady += Game_onReady;
        }

        private static void Game_onReady(object? sender, EventArgs e)
        {
            DefaultGraphicsPipeline pipe = (DefaultGraphicsPipeline)RenderCanvas.main.GetGraphicsPipeline();
            pipe.customRenderPasses.Add(new ImGuiRenderPass());
        }
    }
}
