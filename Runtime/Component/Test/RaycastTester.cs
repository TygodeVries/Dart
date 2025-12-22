using Runtime.Component.Core;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Physics.Raycasts;
namespace Runtime.Component.Test
{
    public class RaycastTester : IComponent
    {
        GameObject m;
        public RaycastTester(GameObject target)
        {
            m = target;
        }
        public override void Update()
        {
            Raycast? cast = Camera.main?.GetRaycastFromMouse();

            RaycastResult? result = cast?.CastInMainScene();

            if (result != null)
            {
                m.GetComponent<Transform>().position = result.hit;
                Debug.Log(result.hit.ToString());
            }
        }

    }
}
