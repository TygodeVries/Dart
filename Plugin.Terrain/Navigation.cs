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
		SortedDictionary<NavigationPiece, NavigationPiece?> exploredSet = 
			new SortedDictionary<NavigationPiece, NavigationPiece?>();
		Navigation terrain;
		public PathFinder(Navigation terrain)
		{
			this.terrain = terrain;
		}
		public override void Update()
		{
			terrain.Draw();
			Graphics.Pipeline.GizmoRenderPass gizmo = Runtime.Graphics.Pipeline.GizmoRenderPass.GetInstance();

			Stack<NavigationPiece> st = new Stack<NavigationPiece>();

			NavigationPiece? q = ClosestBoundaryPiece();
			if (null == q)
				return;
			st.Push(q);
			while (null != (q = Backtrace(q)))
				st.Push(q);

			NavigationPiece p = st.Pop();
			while (st.Count > 0)
			{
				NavigationPiece n = st.Pop();
				gizmo.AddLine(terrain.GetVector(p), terrain.GetVector(n));
				p = n;
			}
		}
		public NavigationPiece? Backtrace(NavigationPiece x)
		{
			return exploredSet[x];
		}
		public bool Init(NavigationPiece start, NavigationPiece end)
		{
			if (0 == start.CompareTo(end))
				return false;
			exploredSet.Clear();
			boundary.Clear();
			NavigationPiece[] nn = terrain.GetNeighbors(start);
			exploredSet.Add(start, null);
			boundary.Enqueue(start, 0);
			return true;
		}
		public enum NavigationStatus
		{
			Busy,
			Done,
			Stuck,
			Error
		};
		public NavigationStatus Step(NavigationPiece end)
		{
			if (null == terrain)
				return NavigationStatus.Error;
			if (boundary.Count == 0)
				return NavigationStatus.Stuck;
			NavigationPiece current = boundary.Dequeue();
			if (0 == current.CompareTo(end))
				return NavigationStatus.Done;
			NavigationPiece[] nn = terrain.GetNeighbors(current);
			foreach (NavigationPiece item in nn)
			{
				if (!exploredSet.ContainsKey(item))
				{
					exploredSet.Add(item, current);
					boundary.Enqueue(item, terrain.EstimateDistance(item, end) + terrain.TransitionCost(current, item));
				}
			}
			return NavigationStatus.Busy;
		}

		public NavigationPiece? ClosestBoundaryPiece()
		{
			if (boundary.Count > 0)
				return boundary.Peek();
			return null;
		}
		NavigationPiece[]? FindPath(NavigationPiece start, NavigationPiece end)
		{
			Init(start, end);
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
