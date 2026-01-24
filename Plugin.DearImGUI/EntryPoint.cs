using Runtime.Calc;
using Runtime.DearImGUI.Backend;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Logging;

namespace Runtime.DearImGUI
{
    [Runtime.Plugins.DartEntryPoint("Main")]
    public class EntryPoint
    {
        public static void Main()
        {
            RenderCanvas.main!.RenderPipelineSet += () =>
            {
                Debug.Log("Loaded into render pipeline...");
                IGraphicsPipeline? gp = RenderCanvas.main!.GetGraphicsPipeline();
                gp?.AddRenderPass(new ImGuiRenderPass());
            };

            UserCode.OnAttemptUnload += () =>
            {
                MainThread.Run(() =>
                {
                    GuiWindow.DisableAll();
                });
            };

            UserCode.OnAttemptRestore += () =>
            {
                MainThread.Run(() =>
                {
                    GuiWindow.RestoreAll();
                });
            };
        }
    }
}
