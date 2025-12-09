using OpenTK.Mathematics;
using Runtime.Component.Core;
using Runtime.Physics.Raycasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Physics
{
	public class SphereCollider : ICollider
	{
		public float radius = 1;
		public Vector3 GetCenter()
		{
			Vector3 center = new Vector3(0, 0, 0);
			if (GetComponent<Transform>() is Transform t)
			{
				center = t.position;
			}
			return center;
		}
		public override bool HasOverlap(Vector3 point)
		{
			return (GetCenter() - point).LengthFast <= radius;
		}

		public override bool HasOverlap(ICollider collider)
		{
			throw new NotImplementedException();
		}
		float Inner(Vector3 a, Vector3 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}
		public override float Raycast(Raycast raycast)
		{
			Vector3 mu = raycast.position - GetCenter();
			Vector3 d = raycast.direction;

			float ud = Inner(mu, d);
			float dd = Inner(d, d);
			float uu = Inner(mu, mu);
			float det = ud * ud - dd * (uu - radius * radius);
			if (det < 0)
				return -1;
			det = MathF.Sqrt(det) / dd;
			float e = -ud;
			float i = e - det;
			if (i >= 0)
				return i;
			i = e + det;
			return i;
		}
	}
}
