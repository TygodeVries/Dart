using System.Globalization;

namespace Runtime.Calc
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public static Vector3 Zero => new Vector3(0, 0, 0);

        public static Vector3 UnitY => new Vector3(0, 1, 0);
        public static Vector3 Up => new Vector3(0, 1, 0);

        public static float Distance(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;

            return MathF.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
        }


        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
        (a.y * b.z) - (a.z * b.y),
        (a.z * b.x) - (a.x * b.z),
        (a.x * b.y) - (a.y * b.x)
    );
        }

        public Vector3(OpenTK.Mathematics.Vector3 vector3)
        {
            this.x = vector3.X;
            this.y = vector3.Y;
            this.z = vector3.Z;
        }

        public Vector3(System.Numerics.Vector3 vector3)
        {
            this.x = vector3.X;
            this.y = vector3.Y;
            this.z = vector3.Z;
        }

        public float this[int ind]
        {
            get
            {
                if (ind == 0)
                    return this.x;
                if (ind == 1)
                    return this.y;
                if (ind == 2)
                    return this.z;
                throw new IndexOutOfRangeException();
            }

            set
            {
                if (ind == 0)
                    this.x = value;
                else if (ind == 1)
                    this.y = value;
                else if (ind == 2)
                    this.z = value;
                else throw new IndexOutOfRangeException();
            }
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        public OpenTK.Mathematics.Vector3 ToOpenTK()
        {
            return new OpenTK.Mathematics.Vector3(x, y, z);
        }

        public System.Numerics.Vector3 ToNumerics()
        {
            return new System.Numerics.Vector3(x, y, z);
        }

        public float Magnitude()
        {
            return MathF.Sqrt((x * x) + (y * y) + (z * z));
        }

        public Vector3 Normalize()
        {
            return this / Magnitude();
        }

        public Vector3 Normalized()
        {
            return Normalize();
        }

        public static Vector3 Normalize(Vector3 vector3)
        {
            return vector3.Normalize();
        }

        public static float Dot(Vector3 a, Vector3 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }

        public static Vector3 Parse(string text)
        {
            string[] args = text.Split(' ');
            float x = float.Parse(args[0], CultureInfo.InvariantCulture);
            float y = float.Parse(args[1], CultureInfo.InvariantCulture);
            float z = float.Parse(args[2], CultureInfo.InvariantCulture);
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return $"{x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} {z.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
