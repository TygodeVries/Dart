
using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Data;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
using Runtime.Scenes;

namespace Runtime.Graphics.Materials
{
    [AssetReference(new string[] { ".material" }, nameof(LoadFromJson))]
    public class Material : AssetReference
    {

        public static Material CreateFallback()
        {
            ShaderProgram shaderProgram = new ShaderProgram("#version 330 core\r\n\r\nlayout(location = 0) in vec3 aPosition;\r\nlayout(location = 1) in vec3 normal;\r\nlayout(location = 2) in vec2 uv;\r\nlayout(location = 3) in vec4 tangent;\r\n\r\nuniform vec3 light_direction;\r\n\r\nuniform mat4 u_Model;\r\nuniform mat4 u_View;\r\nuniform mat4 u_Projection;\r\n\r\nout vec3 Pos;\r\nout vec3 Normal;\r\nout vec2 Uv;\r\nout vec4 Tangent;\r\nout vec3 light_direction_local;\r\n\r\nvoid main()\r\n{\r\n    vec3 temp = aPosition;\r\n    gl_Position = u_Projection * u_View * u_Model * vec4(temp, 1.0);\r\n\r\n    Pos = vec3(u_Model * vec4(aPosition, 1.0));\r\n    Normal = normal;\r\n    Uv = uv;\r\n    Tangent = tangent;\r\n    light_direction_local = normalize(mat3(inverse(u_Model)) * light_direction);\r\n}", "#version 330 core\r\n\r\nin vec3 Pos;\r\nin vec3 Normal;\r\nin vec2 Uv;\r\nin vec4 Tangent;\r\n\r\nout vec4 FragColor;\r\n\r\nvoid main()\r\n{\r\n   FragColor = vec4(1.0, 0, 1.0, 1.0);\r\n}");
            Material material = new Material(shaderProgram);
            return material;
        }

        public static Material CreateSimple()
        {
            ShaderProgram shaderProgram =
               new ShaderProgram(
   @"
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;
layout(location = 3) in vec4 tangent;

uniform vec3 light_direction;
uniform mat4 u_Model;
uniform mat4 u_View;
uniform mat4 u_Projection;

out vec3 Pos;
out vec3 Normal;
out vec2 Uv;
out vec4 Tangent;
out vec3 LightDir;

void main()
{
    gl_Position = u_Projection * u_View * u_Model * vec4(aPosition, 1.0);

    Pos = vec3(u_Model * vec4(aPosition, 1.0));

    mat3 normalMatrix = transpose(inverse(mat3(u_Model)));
    Normal = normalize(normalMatrix * normal);

    vec3 T = normalize(normalMatrix * tangent.xyz);
    Tangent = vec4(T, tangent.w);

    LightDir = normalize(light_direction);
    Uv = uv;
}
",
   @"
#version 330 core

in vec3 Pos;
in vec3 Normal;
in vec2 Uv;
in vec4 Tangent;

uniform sampler2D u_Texture;
uniform sampler2D u_NormalMap;
uniform sampler2D u_Rough;

uniform float u_shininess;
uniform vec3 u_camera_pos;

// Lighting (points)
uniform vec3 u_point_light_pos[16];
uniform vec3 u_point_light_col[16];
uniform vec3 u_point_light_data[16];
uniform int u_pointLight_Count;

// Lighting (direct)
uniform vec3 u_sun_Direction;
uniform vec3 u_sun_Color;

// Lighting (ambient)
uniform vec3 u_ambient_color;

out vec4 FragColor;

void main()
{
	vec4 col = texture(u_Texture, Uv);
	vec3 normalMap = texture(u_NormalMap, Uv).rgb * 2.0 - 1.0;
	vec3 rough = texture(u_Rough, Uv).rgb;


	vec3 viewDir = normalize(u_camera_pos - Pos);  // View direction

	vec3 T = normalize(Tangent.xyz);
	vec3 N = normalize(Normal);
	vec3 B = cross(N, T) * Tangent.w;

	mat3 TBN = mat3(T, B, N);
	vec3 normal = normalize(TBN * normalMap);

	// The resulting light
	vec3 light = u_ambient_color;

	// Calculate direct lighting
	light = light + u_sun_Color * max(dot(normal, -u_sun_Direction), 0.0);

	for (int i = 0; i < u_pointLight_Count; i++)
	{
		vec3 point_pos = u_point_light_pos[i];

		vec3 toLight = point_pos - Pos;
		float dist = length(toLight);
		vec3 lightDir = normalize(toLight);

		float dotV = max(dot(normal, lightDir), 0.0);

		float attenuation = clamp(1.0 - dist * (1 / u_point_light_data[i].x), 0.0, 1.0);

		vec3 diffuse = u_point_light_col[i] * dotV * attenuation * 10;

		vec3 halfDir = normalize(lightDir + viewDir);
		float dotNH = max(dot(normal, halfDir), 0.0);
		float spec = pow(dotNH, (1 - rough.x) * 16) * attenuation;

		vec3 specular = u_point_light_col[i] * spec * 10;

		light += diffuse + specular;
	}

	FragColor = vec4(col.rgb, col.a);
}
");

            Material material = new Material(shaderProgram);
            return material;
        }

        ShaderProgram shader;
        public Material(ShaderProgram shader)
        {
            this.shader = shader;
        }

        public bool matrixEnabled = true;

        public static Material LoadFromJson(Asset asset)
        {
            MaterialData settings = MaterialData.FromJson(asset);
            Material material = settings.CreateMaterial(asset.GetDatabase());
            material.SetAsset(asset);

            return material;
        }

        /// <summary>
        /// Start sending lighting information to any attached shaders
        /// </summary>
        public void EnableLightData()
        {
            Scene.main.GetLightManager().AddEffected(this);
        }

        Dictionary<string, MaterialField> materialFields = new Dictionary<string, MaterialField>();
        public void Use()
        {
            shader.Use();
            foreach (var field in materialFields.Values)
            {
                field.Upload(shader);
            }
        }

        // Permentantly set the value of the material
        public void SetMatrix4(string field, Matrix4 matrix)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is Matrix4MaterialField)
                {
                    ((Matrix4MaterialField)materialFields[field]).matrix = matrix;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Matrix4.");
                    return;
                }
            }
            else
            {
                Matrix4MaterialField materialField = new Matrix4MaterialField(field, matrix);
                materialFields.Add(field, materialField);
            }
        }

        public void Dispose()
        {
            Scene.main.GetLightManager().RemoveEffected(this);
        }

        public void SetFloat(string field, float f)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is FloatMaterialField)
                {
                    ((FloatMaterialField)materialFields[field]).f = f;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Float.");
                    return;
                }
            }
            else
            {
                FloatMaterialField materialField = new FloatMaterialField(field, f);
                materialFields.Add(field, materialField);
            }
        }

        public void SetVector3(string field, Vector3 vector)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is Vector3MaterialField)
                {
                    ((Vector3MaterialField)materialFields[field]).vector = vector;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Vector3.");
                    return;
                }
            }
            else
            {
                Vector3MaterialField materialField = new Vector3MaterialField(field, vector);
                materialFields.Add(field, materialField);
            }
        }

        public void SetVector4(string field, Vector4 vector)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is Vector4MaterialField)
                {
                    ((Vector4MaterialField)materialFields[field]).vector = vector;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Vector4.");
                    return;
                }
            }
            else
            {
                Vector4MaterialField materialField = new Vector4MaterialField(field, vector);
                materialFields.Add(field, materialField);
            }
        }

        public void SetVector3Array(string field, Vector3[] vectors)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is Vector3ArrayMaterialField)
                {
                    ((Vector3ArrayMaterialField)materialFields[field]).vectors = vectors;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Vector3[].");
                    return;
                }
            }
            else
            {
                Vector3ArrayMaterialField materialField = new Vector3ArrayMaterialField(field, vectors);
                materialFields.Add(field, materialField);
            }
        }

        public void SetIntArray(string field, int[] values)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is IntArrayMaterialField)
                {
                    ((IntArrayMaterialField)materialFields[field]).values = values;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be an int[].");
                    return;
                }
            }
            else
            {
                IntArrayMaterialField materialField = new IntArrayMaterialField(field, values);
                materialFields.Add(field, materialField);
            }
        }

        public void SetMatrix4Array(string field, Matrix4[] matrices)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is Matrix4ArrayMaterialField)
                {
                    ((Matrix4ArrayMaterialField)materialFields[field]).matrices = matrices;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Matrix4[].");
                    return;
                }
            }
            else
            {
                Matrix4ArrayMaterialField materialField = new Matrix4ArrayMaterialField(field, matrices);
                materialFields.Add(field, materialField);
            }
        }

        public void SetInt(string field, int i)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is IntMaterialField)
                {
                    ((IntMaterialField)materialFields[field]).i = i;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Int.");
                    return;
                }
            }
            else
            {
                IntMaterialField materialField = new IntMaterialField(field, i);
                materialFields.Add(field, materialField);
            }
        }


        Dictionary<string, int> textureId = new Dictionary<string, int>();
        public void SetTexture(string field, Texture texture)
        {
            if (!textureId.ContainsKey(field))
            {
                textureId.Add(field, textureId.Count);
            }


            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is TextureMaterialField)
                {
                    ((TextureMaterialField)materialFields[field]).texture = texture;
                    ((TextureMaterialField)materialFields[field]).id = Random.Shared.Next(100000);
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Texture.");
                    return;
                }
            }
            else
            {
                TextureMaterialField materialField = new TextureMaterialField(field, texture, textureId[field]);
                materialFields.Add(field, materialField);
            }
        }

        public void SetCubemapTexture(string field, CubemapTexture texture)
        {
            if (!textureId.ContainsKey(field))
            {
                textureId.Add(field, textureId.Count);
            }

            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is CubemapTextureMaterialField)
                {
                    ((CubemapTextureMaterialField)materialFields[field]).texture = texture;
                    ((CubemapTextureMaterialField)materialFields[field]).id = textureId[field];
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a CubemapTexture.");
                    return;
                }
            }
            else
            {
                CubemapTextureMaterialField materialField = new CubemapTextureMaterialField(field, texture, textureId[field]);
                materialFields.Add(field, materialField);
            }
        }
    }

    public abstract class MaterialField
    {
        public MaterialField(string field) { this.field = field; }
        public string field;
        public abstract void Upload(ShaderProgram shader);
    }

    class Vector3MaterialField : MaterialField
    {
        public Vector3 vector;
        public Vector3MaterialField(string field, Vector3 vector) : base(field)
        {
            this.vector = vector;
        }
        public override void Upload(ShaderProgram shader)
        {
            shader.SetVector3(field, vector);
        }
    }

    class Vector4MaterialField : MaterialField
    {
        public Vector4 vector;
        public Vector4MaterialField(string field, Vector4 vector) : base(field)
        {
            this.vector = vector;
        }
        public override void Upload(ShaderProgram shader)
        {
            shader.SetVector4(field, vector);
        }
    }

    class IntMaterialField : MaterialField
    {
        public int i;
        public IntMaterialField(string field, int i) : base(field)
        {
            this.i = i;
        }
        public override void Upload(ShaderProgram shader)
        {
            shader.SetInt(field, i);
        }
    }

    class FloatMaterialField : MaterialField
    {
        public float f;
        public FloatMaterialField(string field, float f) : base(field)
        {
            this.f = f;
        }
        public override void Upload(ShaderProgram shader)
        {
            shader.SetFloat(field, f);
        }
    }


    class Matrix4MaterialField : MaterialField
    {
        public Matrix4 matrix;
        public Matrix4MaterialField(string field, Matrix4 matrix) : base(field)
        {
            this.matrix = matrix;
        }
        public override void Upload(ShaderProgram shader)
        {
            shader.SetMatrix4(field, matrix);
        }
    }

    public class TextureMaterialField : MaterialField
    {
        public int id;
        public Texture texture;
        public TextureMaterialField(string field, Texture texture, int id) : base(field)
        {
            this.id = id;
            this.texture = texture;
        }

        public override void Upload(ShaderProgram shader)
        {
            if (texture == null)
            {
                Debug.Error("Texture is null!");
                return;
            }
            texture.Use((TextureUnit)(((Int64)TextureUnit.Texture0) + id));
            shader.SetTextureId(field, id);
        }
    }

    public class CubemapTextureMaterialField : MaterialField
    {
        public int id;
        public CubemapTexture texture;
        public CubemapTextureMaterialField(string field, CubemapTexture texture, int id) : base(field)
        {
            this.id = id;
            this.texture = texture;
        }

        public override void Upload(ShaderProgram shader)
        {
            if (texture == null)
            {
                Debug.Error("Texture is null!");
                return;
            }
            texture.Use((TextureUnit)(((Int64)TextureUnit.Texture0) + id));
            shader.SetTextureId(field, id);
        }
    }

    class Vector3ArrayMaterialField : MaterialField
    {
        public Vector3[] vectors;

        public Vector3ArrayMaterialField(string field, Vector3[] vectors) : base(field)
        {
            this.vectors = vectors;
        }

        public override void Upload(ShaderProgram shader)
        {
            shader.SetVector3Array(field, vectors);
        }
    }

    class IntArrayMaterialField : MaterialField
    {
        public int[] values;

        public IntArrayMaterialField(string field, int[] values) : base(field)
        {
            this.values = values;
        }

        public override void Upload(ShaderProgram shader)
        {
            shader.SetIntArray(field, values);
        }
    }
    class Matrix4ArrayMaterialField : MaterialField
    {
        public Matrix4[] matrices;

        public Matrix4ArrayMaterialField(string field, Matrix4[] matrices) : base(field)
        {
            this.matrices = matrices;
        }

        public override void Upload(ShaderProgram shader)
        {
            shader.SetMatrix4Array(field, matrices);
        }
    }
}
