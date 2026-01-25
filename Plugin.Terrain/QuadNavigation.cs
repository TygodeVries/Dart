using Runtime.Calc;
using Runtime.Physics.Raycasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Plugin.Navigation
{
	public class QuadNavigation : Navigation
	{
		public void Draw()
		{
		}
		public class QuadNavigationPiece : NavigationPiece
		{
			public int x, y;

			public int CompareTo(object? obj)
			{
				QuadNavigationPiece? o = obj as QuadNavigationPiece;
				if (null == o)
					return 1;
				if (x < o.x)
					return -1;
				if (x > o.x)
					return 1;
				if (y < o.y)
					return -1;
				if (y > o.y)
					return 1;
				return 0;

			}
		}
		public double EstimateDistance(NavigationPiece x, NavigationPiece y)
		{
			if (x is not QuadNavigationPiece xx || y is not QuadNavigationPiece yy)
				return Double.PositiveInfinity;
			return (xx.x - yy.x) * (xx.x - yy.x) + (xx.y - yy.y) * (xx.y - yy.y);
		}

		public NavigationPiece? FromRayCast(Raycast ray)
		{
			throw new NotImplementedException();
		}

		public NavigationPiece[] GetNeighbors(NavigationPiece x)
		{
			if (x is not QuadNavigationPiece xx)
				return new NavigationPiece[0];
			QuadNavigationPiece[] yy = new QuadNavigationPiece[4];

			yy[0] = new QuadNavigationPiece();
			yy[0].x = xx.x;
			yy[0].y = xx.y - 1;
			yy[1] = new QuadNavigationPiece();
			yy[1].x = xx.x;
			yy[1].y = xx.y + 1;
			yy[2] = new QuadNavigationPiece();
			yy[2].x = xx.x - 1;
			yy[2].y = xx.y;
			yy[3] = new QuadNavigationPiece();
			yy[3].x = xx.x + 1;
			yy[3].y = xx.y;

			return yy;
		}

		public double TransitionCost(NavigationPiece x, NavigationPiece y)
		{
			return 1;
		}

		public Vector4 GetVector(NavigationPiece x)
		{
			if (x is not QuadNavigationPiece xx)
				return new Vector4(0, 0, 0, 1);

			return new Vector4(xx.x, xx.y, 0, 1);
		}
	}
}
