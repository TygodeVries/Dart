
using Runtime;
using Runtime.DearImGUI.Gui;
using Runtime.Plugins;
using System.Runtime.CompilerServices;

namespace FeatureTestProject
{
	[DartEntryPoint("Main")]
	public class EntryPoint
	{
		public EntryPoint()
		{

		}
		public static void Main()
		{
			GuiWindow.Enable(new AudioTestWindow());
		}
	}
}

