using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Objects;
using Runtime.Objects.Prefabs;
using System.Text.Json;

namespace Runtime.Tests
{
    internal class PrefabRecordTest : Test
    {
        public override (TestResult, string) Start()
        {
            Vector3 startValue = new Vector3(0, 20, 5);
            GameObject gameObject = new GameObjectFactory()
                .AddComponent(new Transform()
                {
                    position = startValue
                })
                .Build();

            PrefabGameObject prefabValue = PrefabGameObject.FromGameObject(gameObject);
            string json = JsonSerializer.Serialize(prefabValue);

            PrefabGameObject prefabLoaded = JsonSerializer.Deserialize<PrefabGameObject>(json);

            GameObject gm = prefabLoaded.GetGameObject();

            Vector3 result = gm.GetComponent<Transform>().position;

            float distance = Vector3.Distance(result, startValue);

            if (distance < 0.1f)
            {
                return (TestResult.Success, "The values where close enough");
            }
            else
            {
                return (TestResult.Success, $"The values where not close enough {distance}");
            }
        }
    }
}
