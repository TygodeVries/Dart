using OpenTK.Mathematics;
using Runtime.Component.Lighting;
using Runtime.Graphics.Materials;
namespace Runtime.Graphics
{
    public class LightManager
    {

        List<Material> materials = new List<Material>();
        public LightManager()
        {
        }

        /// <summary>
        /// Make a material effected by lighting.
        /// </summary>
        /// <param name="material"></param>
        public void AddEffected(Material material)
        {
            materials.Add(material);
        }

        public void RemoveEffected(Material material)
        {
            materials.Remove(material);
        }


        List<PointLight> pointLights = new List<PointLight>();
        public List<PointLight> GetPointLights()
        {
            return pointLights;
        }

        public void UploadAll()
        {
            Vector3[] positions = new Vector3[pointLights.Count];
            Vector3[] colors = new Vector3[pointLights.Count];
            Vector3[] data = new Vector3[pointLights.Count];

            for (int i = 0; i < pointLights.Count; i++)
            {
                positions[i] = pointLights[i].GetPosition();
                colors[i] = pointLights[i].color;
                data[i] = pointLights[i].GetDataAsVector();
            }

            lock (materials)
            {
                foreach (Material material in materials)
                {
                    if (sunLight != null)
                    {
                        material.SetVector3("u_sun_Direction", sunLight.GetDirection());
                        material.SetVector3("u_sun_Color", sunLight.color);
                    }

                    material.SetVector3("u_ambient_color", ambient);


                    material.SetVector3Array("u_point_light_pos", positions);
                    material.SetVector3Array("u_point_light_col", colors);
                    material.SetVector3Array("u_point_light_data", data);

                    material.SetInt("u_pointLight_Count", positions.Length);
                }
            }
        }

        Vector3 ambient = new Vector3(0.1f, 0.1f, 0.1f);
        SunLight? sunLight = null;
        public void SetSunLight(SunLight sunLight)
        {
            this.sunLight = sunLight;
        }


    }



}
