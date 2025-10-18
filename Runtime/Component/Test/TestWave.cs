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
            GetComponent<Transform>()?.Rotate(0, (float)Time.deltaTime * 100, 0);
        }
    }
}
