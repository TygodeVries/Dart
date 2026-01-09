namespace Runtime.Objects
{
    public abstract class Component
    {
        public bool AlwaysUpdate { get; set; } = false;
        public GameObject? gameObject { get; set; }
        public T? GetComponent<T>() where T : Component
        {
            return gameObject?.GetComponent<T>();
        }

        public virtual void Load() { }
        public virtual void Update() { }
        public virtual void Unload() { }
        public virtual void DrawGizmos() { }
        public virtual string? GetGizmosPath()
        {
            return null;
        }

    }
}
