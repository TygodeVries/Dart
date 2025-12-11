namespace Runtime.Calc
{
    public struct Matrix4
    {
        public float m00;
        public float m01;
        public float m02;
        public float m03;

        public float m10;
        public float m11;
        public float m12;
        public float m13;

        public float m20;
        public float m21;
        public float m22;
        public float m23;

        public float m30;
        public float m31;
        public float m32;
        public float m33;

        public void Invert()
        {
            var m = this.ToOpenTK();
            m.Invert();
            this = new Matrix4(m);
        }

        public static Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            return new Matrix4(OpenTK.Mathematics.Matrix4.LookAt(eye.ToOpenTK(), target.ToOpenTK(), up.ToOpenTK()));
        }

        public static Matrix4 MultiplicativeIdentity => new Matrix4(OpenTK.Mathematics.Matrix4.MultiplicativeIdentity);

        public OpenTK.Mathematics.Matrix4 ToOpenTK()
        {
            return new OpenTK.Mathematics.Matrix4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33); ;
        }

        public static Matrix4 CreateRotationX(float angle)
        {
            OpenTK.Mathematics.Matrix4 matrix;
            OpenTK.Mathematics.Matrix4.CreateRotationX(angle, out matrix);
            return new Matrix4(matrix);
        }

        public static Matrix4 CreateTranslation(Vector3 vector)
        {
            return CreateTranslation(vector.x, vector.y, vector.z);
        }

        public static Matrix4 CreateTranslation(float x, float y, float z)
        {
            OpenTK.Mathematics.Matrix4 matrix;
            OpenTK.Mathematics.Matrix4.CreateTranslation(x, y, z, out matrix);
            return new Matrix4(matrix);
        }

        public static Matrix4 CreateRotationY(float angle)
        {
            OpenTK.Mathematics.Matrix4 matrix;
            OpenTK.Mathematics.Matrix4.CreateRotationY(angle, out matrix);
            return new Matrix4(matrix);
        }

        public static Matrix4 CreateRotationZ(float angle)
        {
            OpenTK.Mathematics.Matrix4 matrix;
            OpenTK.Mathematics.Matrix4.CreateRotationZ(angle, out matrix);
            return new Matrix4(matrix);
        }

        public static Matrix4 CreatePerspectiveFieldOfView(float fovy, float aspect, float depthNear, float depthFar)
        {
            OpenTK.Mathematics.Matrix4 matrix;
            OpenTK.Mathematics.Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, depthNear, depthFar, out matrix);
            return new Matrix4(matrix);
        }

        public Matrix4(OpenTK.Mathematics.Matrix4 matrix)
        {
            this.m00 = matrix.M11;
            this.m01 = matrix.M12;
            this.m02 = matrix.M13;
            this.m03 = matrix.M14;

            this.m10 = matrix.M21;
            this.m11 = matrix.M22;
            this.m12 = matrix.M23;
            this.m13 = matrix.M24;

            this.m20 = matrix.M31;
            this.m21 = matrix.M32;
            this.m22 = matrix.M33;
            this.m23 = matrix.M34;

            this.m30 = matrix.M41;
            this.m31 = matrix.M42;
            this.m32 = matrix.M43;
            this.m33 = matrix.M44;
        }

        public static Matrix4 operator *(Matrix4 a, Matrix4 b)
        {
            return new Matrix4(a.ToOpenTK() * b.ToOpenTK());
        }

        public static Vector4 operator *(Matrix4 m, Vector4 v)
        {
            return new Vector4(m.ToOpenTK() * v.ToOpenTK());
        }

    }
}
