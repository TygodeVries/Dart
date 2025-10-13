using OpenTK.Mathematics;
using Runtime;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Core
{
    public class Camera : IComponent
    {
        public static Camera main;
        public float fieldOfView = 60f;


        public Camera()
        {
            if (main == null)
                main = this;
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(fieldOfView),
                (float)Game.width / (float) Game.height,
                0.1f, 4000.0f
            );
        }

        public Matrix4 GetViewMatrix()
        {
            Transform transform = GetComponent<Transform>();

            Vector3 position = new Vector3(0, 0, 0);
            Vector3 direction = new Vector3(0, 0, -1);
            if(transform != null)
            {
                position = transform.position;
                direction = transform.GetForwards();
            }

            return Matrix4.LookAt(position, position + direction, Vector3.UnitY);
        }
    }
}
