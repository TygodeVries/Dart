using Runtime.Calc;
using Runtime.Scenes;

namespace Runtime.Physics.Raycasts
{
    public class Raycast
    {
        public Vector3 position;
        public Vector3 direction;
        public Raycast(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }

         public float MinimumQuadranceToPoint(Vector3 point)
         {
            float i = -Vector3.Dot((position - point), direction) / Vector3.Dot(direction, direction);

            return ((position + i * direction) - point).Quadrance();
         }

        /// <summary>
        /// Shoot the ray in a specific scene
        /// </summary>
        /// <param name="scene">The scene to shoot the ray in</param>
        /// <returns>The result if something was hit, null if nothing was hit</returns>
        public RaycastResult? CastInScene(Scene scene)
        {
            return scene.physicsSolver.ShootRaycast(this);
        }

        /// <summary>
        /// Shoot the ray in the current main scene
        /// </summary>
        /// <returns>The result if something was hit, null if nothing was hit</returns>
        public RaycastResult? CastInMainScene()
        {
            return CastInScene(Scene.main);
        }
    }
}
