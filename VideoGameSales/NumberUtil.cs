using System.Globalization;

namespace VideoGameSales
{
    public static class NumberUtil
    {
        /// <summary>
        /// Converts a string to double. Returns 0 if the string can not be converted.
        /// </summary>
        /// <param name="str">String to be converted</param>
        /// <returns></returns>
        public static double ConvertDouble(string str)
        {
            double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double result);
            return result;
        }
    }
}