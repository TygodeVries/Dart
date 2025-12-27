using Runtime.Calc;
using Runtime.Physics.Raycasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Plugin.Terrain
{
	public class QuadTerrain : Terrain
	{
		public class QuadTerrainPiece : TerrainPiece
		{
			public int x, y;

			public int CompareTo(object? obj)
			{
				QuadTerrainPiece? o = obj as QuadTerrainPiece;
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
		public double EstimateDistance(TerrainPiece x, TerrainPiece y)
		{
			QuadTerrainPiece? xx = x as QuadTerrainPiece;
			QuadTerrainPiece? yy = y as QuadTerrainPiece;
			if (xx != null && yy != null)
			{
				return (xx.x - yy.x) * (xx.x - yy.x) + (xx.y - yy.y) * (xx.y - yy.y);
			}
			return Double.PositiveInfinity;
		}

		public TerrainPiece? FromRayCast(Raycast ray)
		{
			throw new NotImplementedException();
		}

		public TerrainPiece[] GetNeighbors(TerrainPiece x)
		{
			QuadTerrainPiece? xx = x as QuadTerrainPiece;
			if (null == xx)
				return new TerrainPiece[0];
			QuadTerrainPiece[] yy = new QuadTerrainPiece[4];

			yy[0] = new QuadTerrainPiece();
			yy[0].x = xx.x;
			yy[0].y = xx.y - 1;
			yy[1] = new QuadTerrainPiece();
			yy[1].x = xx.x;
			yy[1].y = xx.y + 1;
			yy[2] = new QuadTerrainPiece();
			yy[2].x = xx.x - 1;
			yy[2].y = xx.y;
			yy[3] = new QuadTerrainPiece();
			yy[3].x = xx.x + 1;
			yy[3].y = xx.y;

			return yy;
		}

		public double TransitionCost(TerrainPiece x, TerrainPiece y)
		{
			return 1;
		}

		public Vector4 GetVector(TerrainPiece x)
		{
			QuadTerrainPiece? xx = x as QuadTerrainPiece;
			return new Vector4(xx.x, xx.y, 0, 1);

		}
	}
}
