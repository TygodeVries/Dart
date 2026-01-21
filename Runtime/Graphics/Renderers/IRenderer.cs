using Runtime.Data;
using Runtime.Graphics.Materials;
using Runtime.Logging;
using Runtime.Objects;

namespace Runtime.Graphics.Renderers
{
    public abstract class Renderer : Objects.Component
    {
        private Material? _material;

        public int Order = 0;

        public void SetMaterial(Material material)
        {
            this.material = material;
        }

        [Inspectable]
        public Material? material
        {
            get
            {
                return _material;
            }
            set
            {
                if (this._material != null)
                {
                    this._material.Dispose();
                }

                _material = value;
            }
        }

        public Material? GetMaterial()
        {
            return _material;
        }

        public abstract void Render(bool useMaterial = true);

        public override void Load()
        {
            RenderCanvas.main!.GetGraphicsPipeline()?.AddRenderer(this);
            base.Load();
        }

        public override void Unload()
        {
            Material? material = GetMaterial();
            if (material != null)
            {

                Asset? asset = material.GetAsset();
                if (asset != null)
                {
                    Debug.Log($"Unloading renderer with material: {asset.GetSystemPath()}");
                }
                else
                {
                    Debug.Log("Unloading renderer with instance material.");
                }
            }
            RenderCanvas.main!.GetGraphicsPipeline()?.RemoveRenderer(this);
            base.Unload();
        }
    }
}
