using System;
using System.Collections.Generic;
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
