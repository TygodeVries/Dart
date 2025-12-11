namespace Runtime.Calc
{
    public struct Vector2
    {
        public float x;
        public float y;

        public static Vector2 Zero => new Vector2(0, 0);

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static float Distance(Vector2 v1, Vector2 v2)
        {
            return MathF.Sqrt(((v2.x - v1.x) * (v2.x - v1.x)) + ((v2.y - v1.y) * (v2.y - v1.y)));
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }

        public Vector2(OpenTK.Mathematics.Vector2 vector3)
        {
            this.x = vector3.X;
            this.y = vector3.Y;
        }

        public Vector2(System.Numerics.Vector2 vector3)
        {
            this.x = vector3.X;
            this.y = vector3.Y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.x / b, a.y / b);
        }

        public OpenTK.Mathematics.Vector2 ToOpenTK()
        {
            return new OpenTK.Mathematics.Vector2(x, y);
        }

        public System.Numerics.Vector2 ToNumerics()
        {
            return new System.Numerics.Vector2(x, y);
        }
    }
}
