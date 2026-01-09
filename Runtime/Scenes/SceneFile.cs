using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Data;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Objects.Prefabs;

namespace Runtime.Scenes
{
    internal class SceneFile
    {
        public List<SceneObject> sceneObjects { get; set; } = new List<SceneObject>();

        public void LoadSceneAssets(AssetDatabase assetDatabase, Scene scene)
        {
            foreach (SceneObject obj in sceneObjects)
            {
                Asset asset = assetDatabase.GetAsset(obj.path);
                GameObject gameObject = PrefabGameObject.FromFile(asset).GetGameObject();

                Transform? transform = gameObject.GetComponent<Transform>();
                if (transform != null)
                {
                    transform.position = Vector3.Parse(obj.position);
                    Debug.Log(obj.position);
                    transform.rotation = Vector3.Parse(obj.rotation);
                }

                scene.Instantiate(gameObject);
                Debug.Log($"Loaded an object for scene from {asset.GetSystemPath()}");
            }
        }
    }

    internal class SceneObject
    {
        public string path { get; set; }
        public string? position { get; set; }
        public string? rotation { get; set; }
    }
}
