
using Runtime;
using Runtime.Component.Core;
using Runtime.DearImGUI.Gui;
using Runtime.Objects;
using Runtime.Plugins;
using System.Runtime.CompilerServices;

namespace FeatureTestProject
{
	[Runtime.Plugins.DartEntryPoint("Main")]
	public class EntryPoint
	{
	
		static EntryPoint()
		{
		}
		public static void Main()
		{
			GuiWindow.Enable(new AudioTestWindow());
			GuiWindow.Enable(new GUIPerformanceWindow());

			Runtime.Scenes.Scene.main.Instantiate(
				new GameObjectFactory().AddComponent<Camera>().Build());

			Runtime.Scenes.Scene.main.Instantiate(
				new GameObjectFactory()
					.AddComponent<Box2D>()
					.AddComponent<Box2DRenderer>()
					.AddComponent<Box2DCollider>()
					.AddComponent<Box2DRigidBody>()
					.Build()
			);
		}
	}
}

