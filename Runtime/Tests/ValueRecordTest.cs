using Runtime.Calc;
using Runtime.Data;
using System.Text.Json;

namespace Runtime.Tests
{
    public class ValueRecordTest : Test
    {
        public override (TestResult, string) Start()
        {
            object[] values = new object[]
            {
                "test",
                7,
                1.2f,
                new Vector2(10, 4),
                new Vector3(5, 3, 1),
                new Vector4(10, 4, 2, 4)
            };

            for (int i = 0; i < values.Length; i++)
            {
                object testItem = values[i];
                ValueRecord valueRecord = new ValueRecord("test", testItem);

                string json = JsonSerializer.Serialize(valueRecord);

                ValueRecord record = JsonSerializer.Deserialize<ValueRecord>(json);
                if (record.GetValue().GetType() == testItem.GetType())
                {
                    continue;
                }
                else
                {
                    return (TestResult.Failure, "The value type was not the same.");
                }
            }

            return (TestResult.Success, "All values where serialized and deserialized as expected!");
        }

    }
}
