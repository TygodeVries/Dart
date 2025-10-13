using OpenTK.Mathematics;
using Runtime.Component.Core;
using Runtime.Graphics;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Lighting
{
    public class PointLight : IComponent
    {
        public override void OnLoad()
        {
            DefaultLightManager.current.pointLights.Add(this);
        }

        public Vector3 GetPosition()
        {
            Transform transform = GetComponent<Transform>();
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
