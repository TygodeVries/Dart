namespace Runtime.Objects
{

    /// <summary>
    /// Allowed on: 
    /// bool, int, float, string, Vector2, Vector3, Vector4
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectableAttribute : Attribute { }
}
