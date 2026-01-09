using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Graphics;
using Runtime.Physics.Raycasts;

namespace Runtime.Components.Core
{

    /// <summary>
    /// A camera is used to render the game image from.
    /// </summary>
    public class Camera : Objects.Component
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

        public override void Unload()
        {
            if (main == this)
                main = null;
        }


        /// <summary>
        /// Set this camera to be the main camera
        /// </summary>
        public void SetAsMain()
        {
            main = this;
        }

        /// <summary>
        /// Returns the projection matrix of the camera
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                OpenTK.Mathematics.MathHelper.DegreesToRadians(fieldOfView),
                Game.width / (float)Game.height,
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
            if (transform != null)
            {
                position = transform.position;
                direction = transform.GetForwards();
            }

            return Matrix4.LookAt(position, position + direction, Vector3.UnitY);
        }
        public Raycast? GetRaycastFromMouse()
        {
            if (null == RenderCanvas.main)
            {
                return null;
            }
            float[] viewport = new float[4];
            unsafe
            {
                fixed (float* vp = &viewport[0])
                {
                    GL.GetFloatv(GetPName.Viewport, vp);
                }
            }
            float x = (2f * Input.Mouse.current.position.x / viewport[2]) - 1f;
            float y = 1f - (2f * Input.Mouse.current.position.y / viewport[3]);

            Matrix4 q = GetViewMatrix() * GetProjectionMatrix();

            q.Invert();
            Vector4 position = new Vector4(0, 0, 0, 1) * q;
            Vector4 direction = new Vector4(x, y, 1, 1) * q;

            position /= position.w;
            direction /= direction.w;
            direction -= position;
            direction.Normalize();

            return new Raycast(position.Xyz, direction.Xyz);
        }
    }
}
