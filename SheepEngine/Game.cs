
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Scenes;

namespace Runtime
{
    public class Game
    {
        public static int width = 640;
        public static int height = 480;
        public static void Start(string path)
        {
            // Set the correct directory
            Directory.SetCurrentDirectory(path);

            GameSettings gameSettings = Files.Load<GameSettings>("GameSettings.json");

            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
                Title = gameSettings.WindowTitle,
            };

            DedicatedSwitch.Switch();
            RenderCanvas window = new RenderCanvas(nativeWindowSettings);
            window.SetGraphicsPipeline(new DefaultGraphicsPipeline());

            Console.WriteLine("Loading Scene...");

            Scene.main = new Scene();
            window.Run();
        }

        public static Action finishedStartup;
    }

    class Program
    {
        public static void Main(string[] args)
        {
            Game.Start("C:\\Users\\zttde\\Desktop\\TestProject");
        }
    }
}