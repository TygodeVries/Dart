using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Components.Test;
using Runtime.Graphics;
using Runtime.Objects;
using Runtime.Scenes;
using static System.MathF;

using Runtime.Plugin.Navigation;
using Runtime.Logging;
using Runtime.Graphics.Renderers;
using Project.Example.Windows;
using Runtime.DearImGUI.Gui;
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

			GraphNavigation? terrain = GraphNavigation.FromFile(
				new Runtime.Data.Asset(
					Runtime.Game.GetAssetDatabase(), "Assets\\Models\\untitled.obj")); ;
			if (null == terrain)
			{
				Debug.Error("Could not load terrain");
				return;
			}
			PathFinder finder = new PathFinder(terrain);
			GraphNavigation.GraphNavigationPiece start = new GraphNavigation.GraphNavigationPiece();
			GraphNavigation.GraphNavigationPiece end = new GraphNavigation.GraphNavigationPiece();
			start.vertex_index = 0;


			end.vertex_index = 2;


			window = new FireWindow(finder, terrain, start, end);
			GuiWindow.Enable(window);
			Runtime.Scenes.Scene.main.Instantiate(
				new GameObjectFactory().AddComponent(finder).Build());
		}
	}
}

