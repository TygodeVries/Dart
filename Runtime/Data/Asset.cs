namespace Runtime.Data
{
    public class Asset
    {
        private AssetDatabase assetDatabase;
        private string systempath;
        private string path;
        public Asset(AssetDatabase database, string path)
        {
            this.assetDatabase = database;
            this.systempath = database.GetAssetPath(path);
            this.path = path;

            if (path.Contains(":")) // In case we accedentally pass a system path
            {
                Asset asset = FromSystemPath(database, path);
                this.path = asset.path;
                this.systempath = asset.systempath;
            }
        }

        public static Asset FromSystemPath(AssetDatabase database, string path)
        {
            string relative = Path.GetRelativePath(database.GetFolder(), path);
            return new Asset(database, relative);
        }

        public string GetName()
        {
            return Path.GetFileName(GetSystemPath());
        }

        public Asset GetFolder()
        {
            string folderPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(folderPath))
                return null;

            return new Asset(assetDatabase, folderPath);
        }
        public AssetDatabase GetDatabase() { return assetDatabase; }

        public string GetPath()
        {
            return path;
        }

        public string GetSystemPath()
        {
            return systempath;
        }
    }
}
