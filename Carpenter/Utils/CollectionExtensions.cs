using System;
using System.Collections.Generic;

namespace Carpenter
{
    public static class CollectionExtensions
    {
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
        public static T? GetKeyOfValue<T, L>(this Dictionary<T, L> dict, L value)
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
        public static string[] RemoveSection(this List<string> array, int start, int end)
        {
            string[] result = new string[array.Count - (end - start)];

            int destinationCount = 0;
            for (int i = 0; i < array.Count; i++)
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

        public static T Add_GetRef<T>(this List<T> list) where T : class, new()
        {
            T newItem = new();
            list.Add(newItem);
            return newItem;
        }
    }
}
