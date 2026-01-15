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
using Runtime.Logging;
using Runtime.Graphics.Renderers;

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
			MeshRenderer renderer = new MeshRenderer();
			Mesh mesh = Mesh.FromFileObj(new Runtime.Data.Asset(Runtime.Game.GetAssetDatabase(), "Assets\\Models\\insane.obj"));
			renderer.SetMesh(mesh);

			Scene.main.Instantiate(
				new GameObjectFactory()
				.AddComponent(renderer)
				.Build());

			GraphNavigation? terrain = GraphNavigation.FromFile(
				new Runtime.Data.Asset(
					Runtime.Game.GetAssetDatabase(), "Assets\\Models\\insane.obj")); ;
			if (null == terrain)
			{
				Debug.Error("Could not load terrain");
				return;
			}
			PathFinder finder = new PathFinder(terrain);
			GraphNavigation.GraphNavigationPiece start = new GraphNavigation.GraphNavigationPiece();
			GraphNavigation.GraphNavigationPiece end = new GraphNavigation.GraphNavigationPiece();
			start.vertex_index = 1000;

			end.vertex_index = 2000;

			window = new FireWindow(finder, terrain, start, end);
			GuiWindow.Enable(window);
			Runtime.Scenes.Scene.main.Instantiate(
				new GameObjectFactory().AddComponent(finder).Build());
		}
	}
}

