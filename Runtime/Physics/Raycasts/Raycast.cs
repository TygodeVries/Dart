using OpenTK.Mathematics;
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
