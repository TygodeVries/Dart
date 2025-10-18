
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;
using Runtime;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Input;
using Runtime.Logging;
using System.Reflection;

namespace Runtime.DearImGUI.Backend
{
    public class ImGuiRenderPass : RenderPass
    {
        ImGuiIOPtr io;
        nint context;
        public override void Start()
        {
            instance = this;

            GuiWindow.Enable(new GuiDemoWindow());

            Debug.Log("Start");
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            context = ImGui.CreateContext();
            io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            io.DisplaySize = new System.Numerics.Vector2(RenderCanvas.main!.FramebufferSize.X, RenderCanvas.main!.FramebufferSize.Y);
            ImGui.StyleColorsDark();

            ImGuiStylePtr style = ImGui.GetStyle();
            if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
            {
                style.WindowRounding = 1.0f;
                style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
            }

            ImguiImplOpenGL3.Init();
            RenderCanvas.main.FramebufferResize += Main_FramebufferResize;
        }

        private void Main_FramebufferResize(OpenTK.Windowing.Common.FramebufferResizeEventArgs obj)
        {
            io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(RenderCanvas.main.FramebufferSize.X, RenderCanvas.main.FramebufferSize.Y);

            io.DisplayFramebufferScale = new System.Numerics.Vector2(
                (float)RenderCanvas.main.FramebufferSize.X / RenderCanvas.main.Size.X,
                (float)RenderCanvas.main.FramebufferSize.Y / RenderCanvas.main.Size.Y
            );
        }

        public static ImGuiRenderPass? instance;
        public List<GuiWindow> guiWindows = new List<GuiWindow>();
        public override void Pass()
        {

            ImguiImplOpenGL3.NewFrame();
            ImGui.NewFrame();

            foreach(GuiWindow window in guiWindows)
            {
                window.Render();
            }

            ImGui.Render();
            GL.Viewport(0, 0, RenderCanvas.main!.FramebufferSize.X, RenderCanvas.main!.FramebufferSize.Y);
            
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

            // Mouse Position
            Vector2 mousePos = Mouse.current.position;
            io.MousePos = new System.Numerics.Vector2(mousePos.X, mousePos.Y);

            io.MouseDown[0] = Mouse.current.leftPressed;
            io.MouseDown[1] = Mouse.current.rightPressed;
            io.MouseDown[2] = Mouse.current.middlePressed;
        }
    }
}
