using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VideoGameSales
{
    public class StringUtil
    {
        static readonly string[] InvalidWords = new string[] { "an", "at", "by", "in", "is", "my", "ni", "no", "of", "the", "to" };


        /// <summary>
        /// Checks if word if searchable
        /// </summary>
        /// <param name="word">Word to validate</param>
        /// <returns></returns>
        private static bool IsValidWord(string word) => word.Length > 1 && !InvalidWords.Contains(word);

        /// <summary>
        /// Removes special characters and accents and transforms string to lowercase
        /// </summary>
        /// <param name="word">String to normalize</param>
        /// <returns></returns>
        public static string NormalizeString(string word)
        {
            return new string(
            word.Normalize(System.Text.NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        /// <summary>
        /// Get all valid and normalized words for a given name
        /// </summary>
        /// <param name="name">String to get words from</param>
        /// <returns></returns>
        public static string[] GetGameNameWords(string name)
        {
            string[] words = Regex.Split(name.ToLower(), "[^A-Za-z0-9]");

            var results = words.Where(IsValidWord).Select(NormalizeString).Distinct().ToArray();

            return results;
        }

        /// <summary>
        /// Gets a hash integer from a string
        /// </summary>
        /// <param name="word">String to hash</param>
        /// <returns></returns>
        public static int GetWordHash(string word) => NumberUtil.Hash(GetWordValue(word));


        /// <summary>
        /// Gets a numeric value for a given string
        /// </summary>
        /// <param name="word">String to get the value from</param>
        /// <returns></returns>
        private static int GetWordValue(string word)
        {
            int value = 0;
            char[] chars = word.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                value += chars[i] + i;
            }

            return value;
        }
    }
}
