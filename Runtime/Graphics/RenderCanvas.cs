using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Runtime.Calc;
using Runtime.Graphics.Pipeline;
using Runtime.Input;
using Runtime.Logging;
using Runtime.Scenes;

namespace Runtime.Graphics
{
    public class RenderCanvas : GameWindow
    {
        IGraphicsPipeline? graphicsPipeline = null;
        public void SetGraphicsPipeline(IGraphicsPipeline graphicsPipeline)
        {
            this.graphicsPipeline = graphicsPipeline;
            RenderPipelineSet?.Invoke();
            this.graphicsPipeline.Initialize();
        }

        public Action? RenderPipelineSet;

        public IGraphicsPipeline? GetGraphicsPipeline()
        {
            return graphicsPipeline;
        }

        public static RenderCanvas? main;
        public RenderCanvas(NativeWindowSettings settings)
         : base(GameWindowSettings.Default, settings)
        {
            if (main == null) main = this;
            Unload += () =>
            {
                Debug.Log("Unloading...");
            };
        }

        protected override void OnLoad()
        {
            Keyboard.current.EndOfFrame();
            Mouse.current.EndOfFrame();
            Mouse.current.scroll = new Vector2();

            Debug.Log("Loading render canvas...");
            base.OnLoad();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left)
                Mouse.current.leftPressed = true;

            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right)
                Mouse.current.rightPressed = true;

            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle)
                Mouse.current.middlePressed = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left)
                Mouse.current.leftPressed = false;

            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right)
                Mouse.current.rightPressed = false;

            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle)
                Mouse.current.middlePressed = false;
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Mouse.current.mouseDelta = new Vector2(e.Delta);
            Mouse.current.position = new Vector2(e.Position);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            Game.width = e.Width;
            Game.height = e.Height;
            base.OnResize(e);
        }


        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            Keyboard.current.SetKeyState(e.Key, true);
            base.OnKeyDown(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            Mouse.current.scroll.x = e.OffsetX;
            Mouse.current.scroll.y = e.OffsetY;
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            Keyboard.current.SetKeyState(e.Key, false);
            base.OnKeyUp(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            MainThread.Update();
            if (args.Time < 0.2f)
            {
                Time.deltaTime = args.Time;
            }
            else
            {
                Time.deltaTime = 0.2f;
                Console.WriteLine("Frame dropped!");
            }
            Scene.main.Update();
        }

        int i = 0;
        double[] frames = new double[500];
        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            graphicsPipeline?.Render();
            frames[i % 500] = args.Time;

            if (i == 500 - 1)
            {
                double avr = 0;
                for (int a = 0; a < 500; a++)
                {
                    avr += frames[a];
                }

                avr /= frames.Length;

                Time.frameRate = 1.0 / avr;
                i = 0;
            }

            i++;
            SwapBuffers();

            Mouse.current.EndOfFrame();
            Keyboard.current.EndOfFrame();
        }
    }
}
