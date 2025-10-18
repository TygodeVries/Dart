using ImGui_OpenTK.Backends;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Runtime;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Input;
using Runtime.Logging;
using System.Reflection;

namespace Editor.ImGuiEditor
{
    public class ImGuiRenderPass : RenderPass
    {
        nint context;
        public override void Start()
        {
            Debug.Log("Start");
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            context = ImGui.CreateContext();
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            io.DisplaySize = new System.Numerics.Vector2(RenderCanvas.main.FramebufferSize.X, RenderCanvas.main.FramebufferSize.Y);
            ImGui.StyleColorsDark();

            ImGuiStylePtr style = ImGui.GetStyle();
            if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
            {
                style.WindowRounding = 1.0f;
                style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
            }

            ImguiImplOpenGL3.Init();
        }

        public override void Pass()
        {
            ImguiImplOpenGL3.NewFrame();
            ImGui.NewFrame();

          //  ImGui.DockSpaceOverViewport();

            ImGui.ShowDemoWindow();

            ImGui.Render();
            GL.Viewport(0, 0, RenderCanvas.main.FramebufferSize.X, RenderCanvas.main.FramebufferSize.Y);
            
            ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

            if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
            {
                ImGui.UpdatePlatformWindows();
                ImGui.RenderPlatformWindowsDefault();

                ImGui.SetCurrentContext(context);
            }

            InputData();
            // (Do the same for keyboard if needed)
        }

        void InputData()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            // Mouse Position
            Vector2 mousePos = Mouse.current.position;
            io.MousePos = new System.Numerics.Vector2(mousePos.X, mousePos.Y);

            io.MouseDown[0] = Mouse.current.leftPressed;
            io.MouseDown[1] = Mouse.current.rightPressed;
            io.MouseDown[2] = Mouse.current.middlePressed;
        }
    }
}
