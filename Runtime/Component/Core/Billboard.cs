using Runtime.Calc;
using Runtime.Objects;

namespace Runtime.Component.Core
{
    public class Billboard : IComponent
    {
        public override void OnLoad()
        {
            alwaysUpdate = true;
        }

        public override void Update()
        {
            Vector3 goal = Camera.main.GetComponent<Transform>().position;
            Vector3 direction = goal - GetComponent<Transform>().position;

            direction.y = 0;

            GetComponent<Transform>().SetForwards(direction);
        }
    }
}
