using Runtime.Graphics.Materials;
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
    }
}
