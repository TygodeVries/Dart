using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Graphics;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Scenes;

namespace Runtime.Component.Lighting
{
    /// <summary>
    /// A light with a source from 1 point.
    /// </summary>
    public class PointLight : IComponent
    {
        public override void OnLoad()
        {
            // Try to add ourselfs to the scene's light manager, we need to keep track of this to send this data to our renderers.
            LightManager defaultLightManager = Scene.main.GetLightManager();
            if (null != defaultLightManager)
            {
                defaultLightManager.GetPointLights().Add(this);
            }
            else
                Debug.Warning("PointLight used without DefaultLightManager");
        }

        public override void Unload()
        {
            LightManager defaultLightManager = Scene.main.GetLightManager();
            if (null != defaultLightManager)
            {
                defaultLightManager.GetPointLights().Remove(this);
            }
            else
                Debug.Warning("PointLight used without DefaultLightManager");
        }

        /// <summary>
        /// Get the position of the light
        /// (Defaults to (0, 0, 0), unless a transform is attached)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            Transform? transform = GetComponent<Transform>();
            if (transform == null)
                return Vector3.Zero;

            return transform.position;
        }

        /// <summary>
        /// The color of the light
        /// In the range 0 - 1
        /// </summary>
        [Inspectable] public Vector3 color = new Vector3(1, 1, 1);

        /// <summary>
        /// The intensity of the light
        /// </summary>
        [Inspectable] public float intensity = 4;

        /// <summary>
        /// Get camera data as a simple vector
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDataAsVector()
        {
            // For now just intensity, but we might want to pass other data later.
            return new Vector3(intensity, 0, 0);
        }
    }
}
