using FeatureTestProject;
using ImGuiNET;
using Runtime.Audio;
using Runtime.Component.Core;
using Runtime.DearImGUI.Gui;
using Runtime.Objects;
using Runtime.Plugin.Terrain;
using Runtime.WindowsNative;
using Runtime.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runtime.Input;

namespace Project.Example.Windows
{
	internal class FireWindow: GuiWindow
	{
		Navigation terrain;
		NavigationPiece start, end;
		PathFinder pathfinder;
		Random r;
		public
		FireWindow(PathFinder finder, Navigation terrain, NavigationPiece start, NavigationPiece end)
		{
			r = new Random();
			pathfinder = finder;
			this.terrain = terrain;
			this.start = start;
			this.end = end;
		}
		int steps_taken = 0;
		bool reset = true;
		
		public override void Render()
		{
			Runtime.Graphics.Pipeline.GizmoRenderPass gizmo = Runtime.Graphics.Pipeline.GizmoRenderPass.GetInstance();

			gizmo.AddLine
			(
				terrain.GetVector(start),
				terrain.GetVector(end),
				new Runtime.Calc.Vector4(1, 0, 0, 0),
				new Runtime.Calc.Vector4(0,1,0,0)
			);

			if (terrain.FromRayCast(Camera.main!.GetRaycastFromMouse()!) is GraphNavigation.GraphNavigationPiece p)
			{
				Vector4 pos = terrain.GetVector(p);
				gizmo.AddLine(pos, pos + new Vector4(0, 1, 0, 0));
				if (Mouse.current.middlePressed)
				{
					end = p;
					pathfinder.Init(start, end);
				}
			}

			ImGui.Begin("Path");

			if (ImGui.Button("Init"))
			{
				steps_taken = 0;
				pathfinder.Init(start, end);
			}
			if (ImGui.Button("Step"))
			{

				steps_taken++;
				if (pathfinder.Step(end) == PathFinder.NavigationStatus.Done)
				{
					Runtime.Logging.Debug.Log("Arrived!");
					start = end;
				}
			}
			ImGui.End();
		}

	}
}
