using System.Globalization;

namespace Runtime.Calc
{
    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static Vector4 One = new Vector4(1, 1, 1, 1);
        public static Vector4 Zero => new Vector4(0, 0, 0, 0);

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Vector4 Parse(string text)
        {
            string[] args = text.Split(new char[] { ',', ' ', ';', ':', '|' }, StringSplitOptions.None);
            float x = float.Parse(args[0], CultureInfo.InvariantCulture);
            float y = float.Parse(args[1], CultureInfo.InvariantCulture);
            float z = float.Parse(args[2], CultureInfo.InvariantCulture);
            float w = float.Parse(args[3], CultureInfo.InvariantCulture);
            return new Vector4(x, y, z, w);
        }

        public override string ToString()
        {
            return $"{x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} {z.ToString(CultureInfo.InvariantCulture)} {w.ToString(CultureInfo.InvariantCulture)}";
        }

        public Vector4(OpenTK.Mathematics.Vector4 v)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = v.Z;
            this.w = v.W;
        }

        public Vector4(System.Numerics.Vector4 v)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = v.Z;
            this.w = v.W;
        }

        public Vector3 Xyz => new Vector3(x, y, z);

        public OpenTK.Mathematics.Vector4 ToOpenTK()
        {
            return new OpenTK.Mathematics.Vector4(x, y, z, w);
        }

        public System.Numerics.Vector4 ToNumerics()
        {
            return new System.Numerics.Vector4(x, y, z, w);
        }

        public static Vector4 operator /(Vector4 a, float b)
        {
            return new Vector4(a.x / b, a.y / b, a.z / b, a.w / b);
        }

        public static Vector4 operator *(Vector4 a, Matrix4 b)
        {
            return new Vector4(a.ToOpenTK() * b.ToOpenTK());
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public float Magnitude()
        {
            return MathF.Sqrt((x * x) + (y * y) + (z * z) + (w * w));
        }

        public Vector4 Normalize()
        {
            return this / Magnitude();
        }
    }
}
