using Runtime.Component.Core;
using Runtime.Calc;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Component.Test
{
    internal class TestWave : IComponent
    {
        double time;
        public override void Update()
        {
            time += Time.deltaTime;
            GetComponent<Transform>().position.Y = MathF.Cos((float) time + + GetComponent<Transform>().position.Z) + 2;
            GetComponent<Transform>().position.X = MathF.Cos((float)time * 3 + GetComponent<Transform>().position.Z) + 1;
        }
    }
}
