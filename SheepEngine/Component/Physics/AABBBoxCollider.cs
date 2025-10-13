using OpenTK.Mathematics;
using Runtime.Component.Core;
using Runtime.Objects;
using Runtime.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Physics
{
    public class AABBBoxCollider : ICollider
    {
        public override void OnLoad()
        {
            Scene.main.physicsSolver.colliders.Add(this);
            base.OnLoad();
        }


        public Vector3 GetCenter()
        {
            Transform transform = GetComponent<Transform>();
            if (transform != null)
                return transform.position;
            return Vector3.Zero;
        }

        public Vector3 Size;

        public override bool HasOverlap(Vector3 point)
        {
            Vector3 center = GetCenter();
            return point.X < center.X + Size.X * 0.5f && point.X > center.X - Size.X * 0.5f &&
                point.Y < center.Y + Size.Y * 0.5f && point.Y > center.Y - Size.Y * 0.5f &&
                point.Z < center.Z + Size.Z * 0.5f && point.Z > center.Z - Size.Z * 0.5f;
        }

        public override bool HasOverlap(ICollider other)
        {
            Vector3 centerA = GetCenter();
            Vector3 centerB = ((AABBBoxCollider) other).GetCenter();

            Vector3 sizeA = Size * 0.5f;
            Vector3 sizeB = ((AABBBoxCollider)other).Size * 0.5f;

            bool overlapX = centerA.X - sizeA.X < centerB.X + sizeB.X &&
                            centerA.X + sizeA.X > centerB.X - sizeB.X;

            bool overlapY = centerA.Y - sizeA.Y < centerB.Y + sizeB.Y &&
                            centerA.Y + sizeA.Y > centerB.Y - sizeB.Y;

            bool overlapZ = centerA.Z - sizeA.Z < centerB.Z + sizeB.Z &&
                            centerA.Z + sizeA.Z > centerB.Z - sizeB.Z;

            return overlapX && overlapY && overlapZ;
        }
    }
}
