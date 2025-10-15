using OpenTK.Mathematics;
using Runtime.Component.Core;
using Runtime.Graphics;
using Runtime.Objects;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runtime.Scenes;

namespace Runtime.Component.Lighting
{
    public class PointLight : IComponent
    {
        public override void OnLoad()
        {
            DefaultLightManager defaultLightManager = Scene.main.GetLightManager() as DefaultLightManager;
            if (null != defaultLightManager)
            {
               defaultLightManager.pointLights.Add(this);
            }
            else
               Debug.Log("PointLight used without DefaultLightManager");
        }

        public Vector3 GetPosition()
        {
            Transform? transform = GetComponent<Transform>();
            if(transform == null)
               return Vector3.Zero;

            return transform.position;
        }

        public Vector3 color;
        public float intensity;
        public Vector3 GetDataAsVector()
        {
            return new Vector3(intensity, 0, 0);
        }
    }
}
