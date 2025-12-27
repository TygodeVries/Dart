using FeatureTestProject;
using ImGuiNET;
using Runtime.Audio;
using Runtime.DearImGUI.Gui;
using Runtime.Objects;
using Runtime.Plugin.Terrain;
using Runtime.WindowsNative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Example.Windows
{
	internal class FireWindow: GuiWindow
	{
		Terrain terrain;
		TerrainPiece start, end;
		PathFinder pathfinder;
		public
		FireWindow(PathFinder finder, Terrain terrain, TerrainPiece start, TerrainPiece end)
		{
			pathfinder = finder;
			this.terrain = terrain;
			this.start = start;
			this.end = end;
		}
		public override void Render()
		{
			ImGui.Begin("Path");

			if (ImGui.Button("Init"))
			{
				pathfinder.Init(terrain, start, end);
			}
			if (ImGui.Button("Step"))
			{
				pathfinder.Step(terrain, end);
			}
			ImGui.End();
		}

	}
}
