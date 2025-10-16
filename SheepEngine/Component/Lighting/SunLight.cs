using OpenTK.Mathematics;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Objects;
using Runtime.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Lighting
{
    /// <summary>
    /// A light, not from a point, but from a direction
    /// </summary>
    public class SunLight : IComponent
    {
        public override void OnLoad()
        {
            // Just like the point light, we need to keep track of ourselfs
            DefaultLightManager? lightManager = Scene.main.GetLightManager() as DefaultLightManager;
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
        public Vector3 direction = new Vector3(1, 1, 1);

        /// <summary>
        /// The color of the sun
        /// </summary>
        public Vector3 color = new Vector3(1, 1, 1);
    }
}
