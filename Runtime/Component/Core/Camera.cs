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

    /// <summary>
    /// A camera is used to render the game image from.
    /// </summary>
    public class Camera : IComponent
    {
        /// <summary>
        /// The camera that is rendering the final image
        /// </summary>
        public static Camera? main;

        /// <summary>
        /// The field of view of the camera
        /// </summary>
        public float fieldOfView = 60f;

        /// <summary>
        /// The color used to clear the background
        /// </summary>
        public Vector3 backgroundColor = new Vector3(0.1f, 0.77f, 0.78f);

        /// <summary>
        /// Create a new camera, if there is no main camera, auto assign this
        /// </summary>
        public Camera()
        {
            if (main == null)
                main = this;
        }

        /// <summary>
        /// Returns the projection matrix of the camera
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(fieldOfView),
                (float)Game.width / (float) Game.height,
                0.1f, 4000.0f
            );
        }

        /// <summary>
        /// Returns the view matrix of the camera
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetViewMatrix()
        {
            Transform? transform = GetComponent<Transform>();

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
