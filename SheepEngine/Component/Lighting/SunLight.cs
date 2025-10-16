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
    public class SunLight : IComponent
    {
        public override void OnLoad()
        {
            DefaultLightManager? lightManager = Scene.main.GetLightManager() as DefaultLightManager;
            if (lightManager == null)
            {
                Console.WriteLine("The SunLight component can not be used without the DefaultLightManager!");
                return;
            }

            lightManager.SetSunLight(this);
        }

        public Vector3 direction;
        public Vector3 color;
    }
}
