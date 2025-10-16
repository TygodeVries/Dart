using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Calc
{
    /// <summary>
    /// A 2 dimensional circle todo math with
    /// </summary>
    public class Circle2D
    {
        public float radius;
        public Vector2 center;

        public Circle2D(float radius, Vector2 center)
        {
            this.radius = radius;
            this.center = center;
        }

        public Circle2D(Vector2 x0, Vector2 x1, Vector2 x2)
        {
            Vector2 y0 = (x0 + x1) / 2f;
            Vector2 y1 = (x1 + x2) / 2f;
            Vector2 d0 = (x1 - x0);
            Vector2 d1 = (x2 - x1);

            Vector3 l0 = new Vector3(
                d0.X,
                d0.Y,
                -y0.X * d0.X - y0.Y * d0.Y
                );

            Vector3 l1 = new Vector3(
                d1.X,
                d1.Y,
                -y1.X * d1.X - y1.Y * d1.Y
                );


            Vector3 xm = Vector3.Cross(l0, l1);

            xm.X /= xm.Z;
            xm.Y /= xm.Z;

            center = new Vector2(xm.X, xm.Y);
            radius = MathF.Sqrt(Vector2.Dot(center - x0, center - x0));
        }

        /// <summary>
        /// Check if a specific point is inside the circle
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool PointInCircle(Vector2 point)
        {
            return Vector2.Distance(center, point) < radius;
        }
    }
}
