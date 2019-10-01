using System.Globalization;

namespace VideoGameSales
{
    public static class NumberUtil
    {
        public static double ConvertDouble(string str)
        {
            double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double result);
            return result;
        }
    }
}