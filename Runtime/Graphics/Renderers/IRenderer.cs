using Runtime.Data;
using Runtime.Graphics.Materials;
using Runtime.Logging;
using Runtime.Objects;

namespace Runtime.Graphics.Renderers
{
    public abstract class Renderer : IComponent
    {
        private Material? _material;

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

        public abstract void Render();

        public override void OnLoad()
        {
            RenderCanvas.main!.GetGraphicsPipeline()?.AddRenderer(this);
            base.OnLoad();
        }

        public override void Unload()
        {
            Asset asset = GetMaterial().GetAsset();
            if (asset != null)
            {
                Debug.Log($"Unloading renderer with material: {asset.GetSystemPath()}");
            }
            else
            {
                Debug.Log("Unloading renderer with instance material.");
            }
            RenderCanvas.main!.GetGraphicsPipeline()?.RemoveRenderer(this);
            base.Unload();
        }
    }
}
