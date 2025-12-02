using System.Globalization;

namespace Runtime.Calc
{
    public class Format
    {
        static NumberFormatInfo f = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = 0 };
        public static string Number(int num)
        {
            return num.ToString("n", f);
        }
    }
}
