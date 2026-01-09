using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Graphics;
using Runtime.Scenes;

namespace Runtime.Components.Lighting
{
    /// <summary>
    /// A light, not from a point, but from a direction
    /// </summary>
    public class SunLight : Objects.Component
    {
        public override void Load()
        {
            // Just like the point light, we need to keep track of ourselfs
            LightManager? lightManager = Scene.main.GetLightManager();
            if (lightManager == null)
            {
                Console.WriteLine("The SunLight component can not be used without the DefaultLightManager!");
                return;
            }

            lightManager.SetSunLight(this);
        }

        /// <summary>
        /// The direction the sun is facing
        /// </summary>
        public Vector3 GetDirection()
        {
            Transform transform = GetComponent<Transform>();
            if (transform == null)
                return new Vector3(1, 1, 1);

            return transform.GetForwards();
        }

        /// <summary>
        /// The color of the sun
        /// </summary>
        public Vector3 color = new Vector3(1, 1, 1);
    }
}
