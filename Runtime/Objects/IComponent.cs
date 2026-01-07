namespace Runtime.Objects
{
    public abstract class IComponent
    {
        public bool alwaysUpdate = false;
        public GameObject? gameObject;
        public T? GetComponent<T>() where T : IComponent
        {
            return gameObject?.GetComponent<T>();
        }

        public virtual void OnLoad() { }
        public virtual void Update() { }
        public virtual bool OverrideParse(string key, string value) { return false; }
        public virtual void Unload() { }

    }
}
