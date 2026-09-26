using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Carpenter
{
    /// <summary>
    /// Extension methods to different core types used throughout Carpenter.
    /// Mainly used to shorthand code that would have be consistently repeated or put in random methods or util classes
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Copy and return characters from a string till a certain character is hit.
        /// </summary>
        public static string CopyTill(this string str, char till)
        {
            char[] chars = str.ToCharArray();

            string copy = string.Empty;
            int index = 0;
            while (chars[index] != till)
            {
                copy += chars[index];
                index++;
            }

            return copy;
        }

        /// <summary>
        /// Removes all white spaces (including tabs) from a string)
        /// </summary>
        public static string StripWhitespaces(this string str)
        {
            //str = str.Replace(" ", string.Empty);
            str = str.Replace(Environment.NewLine, string.Empty);
            str = str.Replace("\t", string.Empty);

            return str;
        }

        /// <summary>
        /// Retrieves a value for a token or option from a line in a config file. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string GetTokenOrOptionValue(this string line)
        {
            return line.Split('=').Last().Split("``").First().StripWhitespaces();
        }

        /// <summary>
        /// Strips forward slashes from the beginning and end of the string if they are present
        /// </summary>
        public static string StripForwardSlashes(this string str)
        {
            int startIndex = str[0] == '/' ? 1 : 0;
            int endIndexOffset = str[str.Length - 1] == '/' ? 1 : 0;
            return str.Substring(startIndex, str.Length - endIndexOffset);
        }

        /// <summary>
        /// Checks if a string contains any letters
        /// </summary>
        public static bool ContainsLetters(this string str)
        {
            foreach (char character in str)
            {
                if (char.IsLetter(character))
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Capitalizes the first letter of the string
        /// </summary>
        public static string Capitalize(this string str)
        {
            if (!string.IsNullOrEmpty(str)) 
                return str.Substring(0, 1).ToUpper() + str.Substring(1);
            else
                return str;
        }

        public static List<int> GetIndexesOf(this string str, string value)
        {
            int currentIndex = 0;
            List<int> indexes = new();
            while (currentIndex != -1)
            {
                currentIndex = str.IndexOf(value, currentIndex + value.Length, StringComparison.Ordinal);
                if (currentIndex != -1)
                    indexes.Add(currentIndex);
            }

            return indexes;
        }

        public static string ToLiteral(this string str)
        {
            return SymbolDisplay.FormatLiteral(str, true);
        }

        public static bool IsEmptyOrNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
