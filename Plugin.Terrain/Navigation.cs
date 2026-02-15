using Runtime.Physics.Raycasts;
using System.Diagnostics.CodeAnalysis;
using Runtime.Calc;
using Runtime.Graphics.Renderers;
using OpenTK.Graphics.OpenGL;
using Runtime.Graphics.Shaders;
using Runtime.Objects;

namespace Runtime.Plugin.Navigation
{
	public interface NavigationPiece : IComparable
	{
		public String ToString();
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
	public class PathFinder : Component
	{
		PriorityQueue<NavigationPiece, double> boundary = new PriorityQueue<NavigationPiece, double>();
		SortedDictionary<NavigationPiece, (NavigationPiece?, double)> exploredSet = 
			new SortedDictionary<NavigationPiece, (NavigationPiece?, double)>();
		Navigation terrain;
		public PathFinder(Navigation terrain)
		{
			this.terrain = terrain;
		}
		public override void Update()
		{
			terrain.Draw();
		}
		public List<NavigationPiece>? GetPathFrom(NavigationPiece? x)
		{
			if (null == x)
				return null;
			List<NavigationPiece> path = new List<NavigationPiece>();

			NavigationPiece? q = x;
			do
			{
				path.Add(q);
			} while (null != (q = Backtrace(q)));

			path.Reverse();
			
			return path;
		}
		public NavigationPiece? Backtrace(NavigationPiece x)
		{
			(NavigationPiece? parent, double q) = exploredSet[x];
			return parent;
		}
		public NavigationStatus Init(NavigationPiece start, NavigationPiece end)
		{
			if (0 == start.CompareTo(end))
				return currentStatus = NavigationStatus.Done;
			exploredSet.Clear();
			boundary.Clear();
			NavigationPiece[] nn = terrain.GetNeighbors(start);
			exploredSet.Add(start, (null,0));
			boundary.Enqueue(start, 0);
			return currentStatus = NavigationStatus.Busy;
		}
		public enum NavigationStatus
		{
			Busy,
			Done,
			Stuck,
			Error
		};
		NavigationStatus currentStatus = NavigationStatus.Error;
		public NavigationStatus Step(NavigationPiece end)
		{
			if (currentStatus != NavigationStatus.Busy)
				return currentStatus;
			if (null == terrain)
				return currentStatus = NavigationStatus.Error;
			if (boundary.Count == 0)
				return currentStatus = NavigationStatus.Stuck;
			NavigationPiece current = boundary.Dequeue();
			if (0 == current.CompareTo(end))
			{
				boundary.Enqueue(current, 0);
				return currentStatus = NavigationStatus.Done;
			}
			NavigationPiece[] nn = terrain.GetNeighbors(current);
			(NavigationPiece? _, double cur_distance) = exploredSet[current];
			foreach (NavigationPiece item in nn)
			{
				if (!exploredSet.ContainsKey(item))
				{
					exploredSet.Add(item, (current, cur_distance + terrain.TransitionCost(current, item)));
					boundary.Enqueue(item, terrain.EstimateDistance(item, end) + terrain.TransitionCost(current, item));
				}
				else
				{
					(NavigationPiece? _, double prev_distance) = exploredSet[item];
					if (cur_distance + terrain.TransitionCost(current, item) < prev_distance)
					{
						exploredSet[item] = (current, cur_distance + terrain.TransitionCost(current, item));
					}

				}
			}
			return currentStatus = NavigationStatus.Busy;
		}

		public NavigationPiece? ClosestBoundaryPiece()
		{
			if (boundary.Count > 0)
				return boundary.Peek();
			return null;
		}
		public NavigationPiece[]? FindPath(NavigationPiece start, NavigationPiece end)
		{
			if (Init(start, end) == NavigationStatus.Done)
				return null;
			
			NavigationStatus status = NavigationStatus.Error;
			while ((status = Step(end)) == NavigationStatus.Busy) ;
			if (status != NavigationStatus.Done)
				return null;
			return GetPathFrom(end)?.ToArray();
		}
	}
}
