using OpenTK.Mathematics;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Core
{

    /// <summary>
    /// A transform changes where an object is rendered at
    /// </summary>
    public class Transform : IComponent
    {
        /// <summary>
        /// The position of the object
        /// </summary>
        public Vector3 position = Vector3.Zero;

        /// <summary>
        /// The rotation, in euler angles (degrees) of the object
        /// </summary>
        public Vector3 rotation;

        /// <summary>
        /// The forwards facing direction of the object.
        /// </summary>
        /// <returns>A vector of magnitude 1 </returns>
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

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// The direction of the right side
        /// </summary>
        /// <returns>A vector with magnitude 1</returns>
        public Vector3 GetRight()
        {
            Vector3 forward = GetForwards();
            Vector3 up = Vector3.UnitY;
            Vector3 right = Vector3.Cross(forward, up);
            return right.Normalized();
        }


        /// <summary>
        /// Rotate the object by the specified amount.
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        public void Rotate(float yaw, float pitch, float roll)
        {
            Rotate(new Vector3(yaw, pitch, roll));
        }

        /// <summary>
        /// Rotate in yaw, pitch, roll
        /// </summary>
        /// <param name="v"></param>
        public void Rotate(Vector3 v)
        {
            rotation += v;
        }

        /// <summary>
        /// Get the matrix of this transform
        /// </summary>
        /// <returns></returns>
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
