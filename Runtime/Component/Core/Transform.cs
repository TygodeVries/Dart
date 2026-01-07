using Runtime.Calc;
using Runtime.Objects;

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
        [Inspectable] public Vector3 position = Vector3.Zero;

        /// <summary>
        /// The rotation, in euler angles (degrees) of the object
        /// </summary>
        [Inspectable] public Vector3 rotation;

        /// <summary>
        /// The forwards facing direction of the object.
        /// </summary>
        /// <returns>A vector of magnitude 1 </returns>
        public Vector3 GetForwards()
        {
            Vector3 radians = new Vector3(
                OpenTK.Mathematics.MathHelper.DegreesToRadians(rotation.x),
                OpenTK.Mathematics.MathHelper.DegreesToRadians(rotation.y),
                OpenTK.Mathematics.MathHelper.DegreesToRadians(rotation.z));

            // Yaw (Y), Pitch (X)
            float yaw = radians.y;
            float pitch = radians.x;

            float x = MathF.Cos(pitch) * MathF.Sin(yaw);
            float y = MathF.Sin(pitch);
            float z = MathF.Cos(pitch) * MathF.Cos(yaw);

            return new Vector3(x, y, z).Normalized();
        }

        public void SetForwards(Vector3 forwards)
        {
            forwards = forwards.Normalized();

            float pitch = MathF.Asin(forwards.y);
            float yaw = MathF.Atan2(forwards.x, forwards.z);

            rotation.x = OpenTK.Mathematics.MathHelper.RadiansToDegrees(pitch);
            rotation.y = OpenTK.Mathematics.MathHelper.RadiansToDegrees(yaw);
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
        /// The direction of the up side
        /// </summary>
        /// <returns>A vector with magnitude 1</returns>
        public Vector3 GetUp()
        {
            Vector3 forward = GetForwards();
            Vector3 right = GetRight();

            Vector3 up = Vector3.Cross(right, forward);
            return up.Normalized();
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
                OpenTK.Mathematics.MathHelper.DegreesToRadians(rotation.x),
                OpenTK.Mathematics.MathHelper.DegreesToRadians(rotation.y),
                OpenTK.Mathematics.MathHelper.DegreesToRadians(rotation.z));

            Matrix4 rotationMatrix =
                Matrix4.CreateRotationY(radians.y) *
                Matrix4.CreateRotationX(radians.x) *
                Matrix4.CreateRotationZ(radians.z);

            Matrix4 translationMatrix = Matrix4.CreateTranslation(position);

            return rotationMatrix * translationMatrix;
        }

    }
}
