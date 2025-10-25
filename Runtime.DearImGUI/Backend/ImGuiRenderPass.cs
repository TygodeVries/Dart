using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Input;
using Runtime.Logging;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Runtime.DearImGUI.Backend
{
    public class ImGuiRenderPass : RenderPass
    {
        ImGuiIOPtr io;
        nint context;
        public override void Start()
        {
            instance = this;

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
        public static List<GuiWindow> guiWindows = new List<GuiWindow>();
        public override void Pass()
        {
            InputData();

            ImguiImplOpenGL3.NewFrame();
            ImGui.NewFrame();

            foreach(GuiWindow guiWindow in guiWindows)
            {
                guiWindow.Render();
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
        }

        void InputData()
        {
            // Mouse Position
            Vector2 mousePos = Mouse.current.position;
            io.MousePos = new System.Numerics.Vector2(mousePos.X, mousePos.Y);

            io.MouseDown[0] = Mouse.current.leftPressed;
            io.MouseDown[1] = Mouse.current.rightPressed;
            io.MouseDown[2] = Mouse.current.middlePressed;

            Keys[] keys = (Keys[]) Enum.GetValues(typeof(Keys));
            Keyboard.current.IsPressed(Keys.A);

            foreach (var v in keyMap)
            {
                bool isPressedThisFrame = Keyboard.current.IsPressedThisFrame(v.Key);
                if(isPressedThisFrame)
                {
                    io.AddKeyEvent(v.Value, true);
                }

                bool isReleasedThisFrame = Keyboard.current.IsReleasedThisFrame(v.Key);
                if (isReleasedThisFrame)
                {
                    io.AddKeyEvent(v.Value, false);
                }
            }

            // Update modifier keys manually (important)
            io.KeyCtrl = Keyboard.current.IsPressed(Keys.LeftControl) || Keyboard.current.IsPressed(Keys.RightControl);
            io.KeyShift = Keyboard.current.IsPressed(Keys.LeftShift) || Keyboard.current.IsPressed(Keys.RightShift);
            io.KeyAlt = Keyboard.current.IsPressed(Keys.LeftAlt) || Keyboard.current.IsPressed(Keys.RightAlt);
            io.KeySuper = Keyboard.current.IsPressed(Keys.LeftSuper) || Keyboard.current.IsPressed(Keys.RightSuper);

        }

        Dictionary<Keys, ImGuiKey> keyMap = new Dictionary<Keys, ImGuiKey>
{
    // Letters
    { Keys.A, ImGuiKey.A },
    { Keys.B, ImGuiKey.B },
    { Keys.C, ImGuiKey.C },
    { Keys.D, ImGuiKey.D },
    { Keys.E, ImGuiKey.E },
    { Keys.F, ImGuiKey.F },
    { Keys.G, ImGuiKey.G },
    { Keys.H, ImGuiKey.H },
    { Keys.I, ImGuiKey.I },
    { Keys.J, ImGuiKey.J },
    { Keys.K, ImGuiKey.K },
    { Keys.L, ImGuiKey.L },
    { Keys.M, ImGuiKey.M },
    { Keys.N, ImGuiKey.N },
    { Keys.O, ImGuiKey.O },
    { Keys.P, ImGuiKey.P },
    { Keys.Q, ImGuiKey.Q },
    { Keys.R, ImGuiKey.R },
    { Keys.S, ImGuiKey.S },
    { Keys.T, ImGuiKey.T },
    { Keys.U, ImGuiKey.U },
    { Keys.V, ImGuiKey.V },
    { Keys.W, ImGuiKey.W },
    { Keys.X, ImGuiKey.X },
    { Keys.Y, ImGuiKey.Y },
    { Keys.Z, ImGuiKey.Z },

    // Numbers (top row)
    { Keys.D0, ImGuiKey._0 },
    { Keys.D1, ImGuiKey._1 },
    { Keys.D2, ImGuiKey._2 },
    { Keys.D3, ImGuiKey._3 },
    { Keys.D4, ImGuiKey._4 },
    { Keys.D5, ImGuiKey._5 },
    { Keys.D6, ImGuiKey._6 },
    { Keys.D7, ImGuiKey._7 },
    { Keys.D8, ImGuiKey._8 },
    { Keys.D9, ImGuiKey._9 },

    // Function keys
    { Keys.F1, ImGuiKey.F1 },
    { Keys.F2, ImGuiKey.F2 },
    { Keys.F3, ImGuiKey.F3 },
    { Keys.F4, ImGuiKey.F4 },
    { Keys.F5, ImGuiKey.F5 },
    { Keys.F6, ImGuiKey.F6 },
    { Keys.F7, ImGuiKey.F7 },
    { Keys.F8, ImGuiKey.F8 },
    { Keys.F9, ImGuiKey.F9 },
    { Keys.F10, ImGuiKey.F10 },
    { Keys.F11, ImGuiKey.F11 },
    { Keys.F12, ImGuiKey.F12 },

    // Modifiers
    { Keys.LeftShift, ImGuiKey.ModShift },
    { Keys.RightShift, ImGuiKey.ModShift },
    { Keys.LeftControl, ImGuiKey.ModCtrl },
    { Keys.RightControl, ImGuiKey.ModCtrl },
    { Keys.LeftAlt, ImGuiKey.ModAlt },
    { Keys.RightAlt, ImGuiKey.ModAlt },
    { Keys.LeftSuper, ImGuiKey.ModSuper },
    { Keys.RightSuper, ImGuiKey.ModSuper },

    // Navigation
    { Keys.Up, ImGuiKey.UpArrow },
    { Keys.Down, ImGuiKey.DownArrow },
    { Keys.Left, ImGuiKey.LeftArrow },
    { Keys.Right, ImGuiKey.RightArrow },

    // Symbols and misc
    { Keys.Enter, ImGuiKey.Enter },
    { Keys.Escape, ImGuiKey.Escape },
    { Keys.Tab, ImGuiKey.Tab },
    { Keys.Backspace, ImGuiKey.Backspace },
    { Keys.Insert, ImGuiKey.Insert },
    { Keys.Delete, ImGuiKey.Delete },
    { Keys.PageUp, ImGuiKey.PageUp },
    { Keys.PageDown, ImGuiKey.PageDown },
    { Keys.Home, ImGuiKey.Home },
    { Keys.End, ImGuiKey.End },
    { Keys.Space, ImGuiKey.Space },
    { Keys.Minus, ImGuiKey.Minus },
    { Keys.Equal, ImGuiKey.Equal },
    { Keys.LeftBracket, ImGuiKey.LeftBracket },
    { Keys.RightBracket, ImGuiKey.RightBracket },
    { Keys.Backslash, ImGuiKey.Backslash },
    { Keys.Semicolon, ImGuiKey.Semicolon },
    { Keys.Apostrophe, ImGuiKey.Apostrophe },
    { Keys.Comma, ImGuiKey.Comma },
    { Keys.Period, ImGuiKey.Period },
    { Keys.Slash, ImGuiKey.Slash },
    { Keys.GraveAccent, ImGuiKey.GraveAccent },
};

    }
}
