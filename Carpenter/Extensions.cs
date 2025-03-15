using System;
using System.Collections.Generic;
using System.Linq;

namespace Carpenter
{
    /// <summary>
    /// Extension methods to different core types used throughout Carpenter.
    /// Mainly used to shorthand code that would have be consistently repeated or put in random methods or util classes
    /// </summary>
    public static class Extensions
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
        /// Finds the key closest to the inputted int value in a dictionary
        /// </summary>
        public static int FindClosestKey(this Dictionary<int, string> dict, int value)
        {
            int closestKey = int.MaxValue;
            foreach (int key in dict.Keys)
            {
                int diff = Math.Abs(value - key);
                if (diff < closestKey)
                {
                    closestKey = key;
                }
            }

            return closestKey;
        }

        /// <summary>
        /// Returns the key that a particular value is stored at
        /// </summary>
        /// <remarks>
        /// Hehehehe... ew
        /// I kind of just did this because I could...
        /// </remarks>
        public static T? GetKeyOfValue<T, L> (this Dictionary<T, L> dict, L value)
        {
            foreach (KeyValuePair<T, L> keyPair in dict)
            {
                if (keyPair.Value == null)
                {
                    continue;
                }

                if (keyPair.Value.Equals(value))
                {
                    return keyPair.Key;
                }
            }

            return default;
        }

        /// <summary>
        /// Add a new key with a given value to a dictionary or update the key if it already exists
        /// </summary>
        public static void AddOrUpdate<T, L>(this Dictionary<T, L> dict, T key, L value)
        {
            if (dict.TryAdd(key, value) == false)
            {
                dict[key] = value;
            }
        }

        /// <summary>
        /// Removes a section of an array from the supplied start to the end index
        /// </summary>
        public static string[] RemoveSection(this string[] array, int start, int end)
        {
            string[] result = new string[array.Length - (end - start)];

            int destinationCount = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (i >= start && i <= end)
                {
                    continue;
                }
                result[destinationCount] = array[i];
                destinationCount++;
            }

            return result;
        }

        /// <summary>
        /// Basic array search that returns the index that has the current string value
        /// </summary>
        public static int FindIndexWhichContainsValue(this string[] array, string value)
        {
            for (int i = 0; i < array?.Length; i++)
            {
                string element = array[i];

                if (element.Contains(value))
                {
                    return i;
                }
            }

            return -1;
        }

    }
}
