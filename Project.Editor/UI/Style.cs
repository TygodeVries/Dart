using ImGuiNET;
using System.Numerics;

namespace Project.Editor.UI
{
    public class Style
    {
        public static void Apply()
        {
            var style = ImGui.GetStyle();
            var colors = style.Colors;

            ImGui.StyleColorsDark();

            style.WindowRounding = 0;
            style.FrameRounding = 8f;
            style.GrabRounding = 8f;
            style.ScrollbarRounding = 8f;
            style.TabRounding = 8f;

            style.FramePadding = new Vector2(6f, 4f);
            style.ItemSpacing = new Vector2(8f, 6f);
            style.WindowPadding = new Vector2(10f, 10f);


            var io = ImGui.GetIO();

            io.Fonts.AddFontFromFileTTF("Assets/Fonts/Roboto-Medium.ttf", 18f);
        }
    }
}
