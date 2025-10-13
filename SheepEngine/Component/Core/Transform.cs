using OpenTK.Mathematics;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Core
{
    public class Transform : IComponent
    {
        public Vector3 position = Vector3.Zero;
        public Vector3 rotation;
        public Vector3 scale = new Vector3(1, 1, 1);


        public Vector3 GetForwards()
        {
            Vector3 radians = new Vector3(
                MathHelper.DegreesToRadians(rotation.X),
                MathHelper.DegreesToRadians(rotation.Y),
                MathHelper.DegreesToRadians(rotation.Z));

            float pitch = radians.X;
            float yaw = radians.Y;

            float x = MathF.Cos(pitch) * MathF.Sin(yaw);
            float y = -MathF.Sin(pitch);
            float z = MathF.Cos(pitch) * MathF.Cos(yaw);

            return new Vector3(x, y, z).Normalized();
        }

        public Vector3 GetRight()
        {
            Vector3 forward = GetForwards();
            Vector3 up = Vector3.UnitY;
            Vector3 right = Vector3.Cross(forward, up);
            return right.Normalized();
        }


        public void Rotate(float yaw, float pitch, float roll)
        {
            Rotate(new Vector3(yaw, pitch, roll));
        }

        public void Rotate(Vector3 v)
        {
            rotation += v;
        }

        public Matrix4 GetMatrix()
        {
            Vector3 radians = new Vector3(
                MathHelper.DegreesToRadians(rotation.X),
                MathHelper.DegreesToRadians(rotation.Y),
                MathHelper.DegreesToRadians(rotation.Z));

            Matrix4 rotX = Matrix4.CreateRotationX(radians.X);
            Matrix4 rotY = Matrix4.CreateRotationY(radians.Y);
            Matrix4 rotZ = Matrix4.CreateRotationZ(radians.Z);

            Matrix4 rotationMatrix = rotZ * rotX * rotY;

            Matrix4 translationMatrix = Matrix4.CreateTranslation(position);

            return rotationMatrix * translationMatrix;
        }
    }
}
