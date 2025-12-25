namespace Runtime.Data
{
    public abstract class AssetReference
    {
        private string? path;
        public string? GetFilePath()
        {
            return path;
        }

        public void SetFilePath(string path)
        {
            this.path = path;
        }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AssetReferenceAttribute : Attribute
    {
        public string[] filetype;
        public string createMethod;
        public AssetReferenceAttribute(string[] filetype, string createMethod)
        {
            this.createMethod = createMethod;
            this.filetype = filetype;
        }
    }
}
