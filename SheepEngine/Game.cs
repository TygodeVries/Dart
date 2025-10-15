
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Scenes;
using static Runtime.Logging.Debug;

namespace Runtime
{
    public class Game
    {
        public static int width = 640;
        public static int height = 480;
        public static void Start(string path)
        {
            Log("Starting Dart v0.1...");
            Log($"Working from {path}");
            Directory.SetCurrentDirectory(path);

            Log($"Loading {Path.Join(path, "GameSettings.json")}...");
            GameSettings? gameSettings = Files.Load<GameSettings>("GameSettings.json");

            if (null == gameSettings)
            {
               Log("GameSettings.json not loaded");
               return;
            }

            Log("Attempting to switch to dedicated graphics card (If present)");
            DedicatedSwitch.Switch();

            Log($"Creating window of size {width}, {height}");
            Log($"Setting window title to {gameSettings?.WindowTitle}");
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(width, height),
                Title = gameSettings?.WindowTitle,
            };

            RenderCanvas window = new RenderCanvas(nativeWindowSettings);

            IGraphicsPipeline graphicsPipeline = new DefaultGraphicsPipeline();
            Log($"Using graphicsPipeline: {graphicsPipeline}.");            
            window.SetGraphicsPipeline(graphicsPipeline);

            Log($"Opening window...");
            window.Run(); // Keeps the thread blocked until closed.
            Log($"Cleaning up...");
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            Game.Start("TestProject");
        }
    }
}