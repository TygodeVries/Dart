using System.Globalization;

namespace Runtime.Calc
{
    public class Encoder
    {
        public static string Get(System.Numerics.Vector4 vector4)
        {
            return $"{vector4.X.ToString(CultureInfo.InvariantCulture)} {vector4.Y.ToString(CultureInfo.InvariantCulture)} {vector4.Z.ToString(CultureInfo.InvariantCulture)} {vector4.W.ToString(CultureInfo.InvariantCulture)}";
        }

        public static System.Numerics.Vector4 NVec4(string text)
        {
            if (text == "default")
            {
                return default(System.Numerics.Vector4);
            }

            string[] args = text.Split(' ');
            return new System.Numerics.Vector4(float.Parse(args[0], CultureInfo.InvariantCulture), float.Parse(args[1], CultureInfo.InvariantCulture), float.Parse(args[2], CultureInfo.InvariantCulture), float.Parse(args[3], CultureInfo.InvariantCulture));
        }

        public static string Get(OpenTK.Mathematics.Vector4 vector4)
        {
            return $"{vector4.X.ToString(CultureInfo.InvariantCulture)} {vector4.Y.ToString(CultureInfo.InvariantCulture)} {vector4.Z.ToString(CultureInfo.InvariantCulture)} {vector4.W.ToString(CultureInfo.InvariantCulture)}";
        }

        public static OpenTK.Mathematics.Vector4 OVec4(string text)
        {
            if (text == "default")
            {
                return default(OpenTK.Mathematics.Vector4);
            }

            string[] args = text.Split(' ');
            return new OpenTK.Mathematics.Vector4(float.Parse(args[0], CultureInfo.InvariantCulture), float.Parse(args[1], CultureInfo.InvariantCulture), float.Parse(args[2], CultureInfo.InvariantCulture), float.Parse(args[3], CultureInfo.InvariantCulture));
        }

    }
}
