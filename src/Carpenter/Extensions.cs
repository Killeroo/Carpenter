using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpenter
{
    public static class Extensions
    {
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

        public static string StripWhitespaces(this string str)
        {
            //str = str.Replace(" ", string.Empty);
            str = str.Replace(Environment.NewLine, string.Empty);
            str = str.Replace("\t", string.Empty);

            return str;
        }

        public static string GetTokenOrOptionValue(this string line)
        {
            return line.Split('=').Last().Split("``").First().StripWhitespaces();
        }

        public static string StripForwardSlashes(this string str)
        {
            int startIndex = str[0] == '/' ? 1 : 0;
            int endIndexOffset = str[str.Length - 1] == '/' ? 1 : 0;
            return str.Substring(startIndex, str.Length - endIndexOffset);
        }

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

        // Hehehehe... ew
        // I kind of just did this because I could...
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

        public static void AddOrUpdate<T, L>(this Dictionary<T, L> dict, T key, L value)
        {
            if (dict.TryAdd(key, value) == false)
            {
                dict[key] = value;
            }
        }

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
