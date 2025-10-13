using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Objects
{
    public class GameObjectFactory
    {
        GameObject gameObject;
        public GameObjectFactory()
        {
            gameObject = new GameObject();
        }
        public GameObjectFactory AddComponent(IComponent component)
        {
            gameObject.AddComponent(component);
            return this;
        }

        public GameObject Build()
        {
            return gameObject;
        }
    }
}
