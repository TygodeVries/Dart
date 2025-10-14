
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Runtime.Graphics;
using Runtime.Graphics.Shaders;
using Runtime.Scenes;
using System;


namespace Runtime.Graphics.Materials
{
    public class Material
    {
        ShaderProgram shader;
        public Material(ShaderProgram shader)
        {
            this.shader = shader;
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
        public void SetTexture(string field, Texture texture, int id)
        {
            if (materialFields.ContainsKey(field))
            {
                if (materialFields[field] is TextureMaterialField)
                {
                    ((TextureMaterialField)materialFields[field]).texture = texture;
                    ((TextureMaterialField)materialFields[field]).id = id;
                }
                else
                {
                    Console.WriteLine($"{field} is already set as {materialFields[field].GetType()}. It can not also be a Texture.");
                    return;
                }
            }
            else
            {
                TextureMaterialField materialField = new TextureMaterialField(field, texture, id);
                materialFields.Add(field, materialField);
            }
        }
    }

    abstract class MaterialField
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

    class TextureMaterialField : MaterialField
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
            texture.Use((TextureUnit) (((Int64) TextureUnit.Texture0) + id));
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
