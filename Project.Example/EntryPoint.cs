using Project.Example.Windows;
using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Component.Test;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Objects;
using Runtime.Scenes;
using static System.MathF;

using Runtime.Plugin.Terrain;

namespace FeatureTestProject
{
    [Runtime.Plugins.DartEntryPoint("Main")]
    public class EntryPoint
    {
         static FireWindow window;
        static EntryPoint()
        {
        }
        public static void Main()
        {
            Runtime.Scenes.Scene.main.Instantiate(
                new GameObjectFactory()
                .AddComponent<Camera>()
                .AddComponent<Transform>()
                .AddComponent<FlightCamera>()
                .Build());


         QuadTerrain terrain = new QuadTerrain();
         PathFinder finder = new PathFinder();
         QuadTerrain.QuadTerrainPiece start = new QuadTerrain.QuadTerrainPiece();
         QuadTerrain.QuadTerrainPiece end = new QuadTerrain.QuadTerrainPiece();
         start.x = 0;
         start.y = 0;

         end.x = 10;
         end.y = 20;

		 window = new FireWindow(finder, terrain, start, end);
		 GuiWindow.Enable(window);
         Runtime.Scenes.Scene.main.Instantiate(
            new GameObjectFactory().AddComponent(finder).Build());
		}
	}
}

