using Runtime.Calc;

namespace Runtime.Components.Core
{
    public class Billboard : Objects.Component
    {
        public override void Load()
        {
            AlwaysUpdate = true;
        }

        public override void Update()
        {

            Transform? thisTransform = GetComponent<Transform>();
            if (thisTransform == null)
            {
                return;
            }

            Vector3 goal = Camera.main.GetComponent<Transform>().position;
            Vector3 direction = goal - thisTransform.position;

            direction.y = 0;

            thisTransform.SetForwards(direction);
        }
    }
}
