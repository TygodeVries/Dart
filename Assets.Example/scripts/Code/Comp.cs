using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Objects;
using System;

namespace Game.Code
{
    public class EpicWaveTest : Component
    {
        float timer = 0;
        public override void Update()
        {
            Transform transform = GetComponent<Transform>();
            timer += (float)Time.deltaTime;
            transform.position.y = MathF.Cos(timer);
        }
    }
}
