using Runtime.Calc;
using Runtime.Components.Core;

namespace Runtime.Components.Test
{
    internal class TestWave : Component
    {
        double time;
        public override void Update()
        {
            time += Time.deltaTime;
            GetComponent<Transform>()?.Rotate(0, (float)Time.deltaTime * 100, 0);
        }
    }
}
