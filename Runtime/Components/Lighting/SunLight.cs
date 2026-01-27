using Runtime.Calc;
using Runtime.Graphics;
using Runtime.Objects;
using Runtime.Scenes;

namespace Runtime.Components.Lighting
{
    /// <summary>
    /// A light, not from a point, but from a direction
    /// </summary>
    public class SunLight : Objects.Component
    {
        public override string? GetGizmosPath()
        {
            return "assets/textures/gizmos/sunlight.png";
        }
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
            return direction;
        }

        [Inspectable]
        public Vector3 direction = new Vector3(1, 1, 1);

        /// <summary>
        /// The color of the sun
        /// </summary>
        [Inspectable]
        public Vector3 color = new Vector3(1, 1, 1);
    }
}
