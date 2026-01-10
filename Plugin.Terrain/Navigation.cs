using Runtime.Physics.Raycasts;
using System.Diagnostics.CodeAnalysis;
using Runtime.Calc;
using Runtime.Graphics.Renderers;
using OpenTK.Graphics.OpenGL;
using Runtime.Graphics.Shaders;
using Runtime.Objects;

namespace Runtime.Plugin.Terrain
{
	public interface NavigationPiece : IComparable
	{

	}
	public interface Navigation
	{
		NavigationPiece? FromRayCast(Raycast ray);
		double EstimateDistance(NavigationPiece x, NavigationPiece y);
		double TransitionCost(NavigationPiece x, NavigationPiece y);
		NavigationPiece[] GetNeighbors(NavigationPiece x);
		Vector4 GetVector(NavigationPiece x);
		void Draw();
	}
	public class PathFinder : IComponent
	{
		PriorityQueue<NavigationPiece, double> boundary = new PriorityQueue<NavigationPiece, double>();
		SortedDictionary<NavigationPiece, NavigationPiece> exploredSet = new SortedDictionary<NavigationPiece, NavigationPiece>();
		Navigation? terrain;
		public PathFinder()
		{

		}
		public override void Update()
		{
			if (null == terrain)
				return;
			terrain.Draw();
			Graphics.Pipeline.GizmoRenderPass gizmo = Runtime.Graphics.Pipeline.GizmoRenderPass.GetInstance();

			NavigationPiece[] keys = exploredSet.Keys.ToArray<NavigationPiece>();
			NavigationPiece[] values = exploredSet.Values.ToArray<NavigationPiece>();

			for (int cx = 0; cx < exploredSet.Keys.Count; cx++)
			{
				Vector4 a = terrain.GetVector(keys[cx]);
				Vector4 b = terrain.GetVector(values[cx]);

				gizmo.AddLine(a, b);
			}
		}
		public NavigationPiece? Backtrace(NavigationPiece x)
		{
			return exploredSet[x];
		}
		public void Init(Navigation terrain, NavigationPiece start, NavigationPiece end)
		{
			this.terrain = terrain;
			exploredSet.Clear();
			boundary.Clear();
			NavigationPiece[] nn = terrain.GetNeighbors(start);
			foreach (NavigationPiece item in nn)
			{
				exploredSet.Add(item, start);
				boundary.Enqueue(item, terrain.EstimateDistance(item, end) + terrain.TransitionCost(start, item));
			}
		}

		public bool Step(Navigation terrain, NavigationPiece end)
		{
			if (boundary.Count == 0)
				return false;
			NavigationPiece current = boundary.Dequeue();
			if (0 == current.CompareTo(end))
				return false;
			NavigationPiece[] nn = terrain.GetNeighbors(current);
			foreach (NavigationPiece item in nn)
			{
				if (!exploredSet.ContainsKey(item))
				{
					exploredSet.Add(item, current);
					boundary.Enqueue(item, terrain.EstimateDistance(item, end) + terrain.TransitionCost(current, item));
				}
			}
			return true;
		}
		NavigationPiece[]? FindPath(Navigation terrain, NavigationPiece start, NavigationPiece end)
		{
			Init(terrain, start, end);
			while (boundary.Count > 0)
			{
				NavigationPiece current = boundary.Dequeue();
				if (current == end)
					break;
				NavigationPiece[] nn = terrain.GetNeighbors(current);
				foreach (NavigationPiece item in nn)
				{
					if (!exploredSet.ContainsKey(item))
					{
						exploredSet.Add(item, current);

						boundary.Enqueue(item, terrain.EstimateDistance(item, end));
					}
				}
			}
			return null;
		}
	}
}
