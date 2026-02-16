using ImGuiNET;
using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;

namespace Project.Editor.UI.Scenes
{
    public class CameraWindow : GuiWindow
    {
        RenderTexture? renderTexture;
        public void RenderCamera(Camera camera)
        {
            camera.SetRenderTexture(new RenderTexture(1024, 1024));
            renderTexture = camera.renderTexture;
        }

        public override void Render()
        {
            Vector2 size = new Vector2(ImGui.GetWindowSize());
            ImGui.Image(renderTexture.ColorTexture, size.ToNumerics());
        }
    }
}
