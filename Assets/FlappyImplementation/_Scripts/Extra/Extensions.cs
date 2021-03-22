using System.Text;
using UnityEngine;

namespace FlappyPlane
{
    public static class Extensions
    {
        /// <summary>Returns a number capped in between two others.</summary>
        /// <param name="min">Minimum value of the number.</param>
        /// <param name="max">Maximum value of the number.</param>
        public static float Capped(this float number, float min = 0, float max = 1)
        {
            float result = number;
            if (max < min)
                Swap(ref min, ref max);

            if (result > max)
                result = max;
            else if (result < min)
                result = min;
            return result;
        }

        /// <summary>Swaps two values or references.</summary>
        /// <param name="a">The first value or reference.</param>
        /// <param name="b">The second value or reference.</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        /// <summary>Inserts a value into an array, pushing out all the other (higher index) values as needed.</summary>
        /// <param name="index">Where to insert the value.</param>
        /// <param name="value">Value to be inserted.</param>
        public static void PushAt<T>(this T[] array, int index, T value)
        {
            if (index >= array.Length || index < 0) {
                Debug.LogError($"Index ({index}) out of range ({array.Length})");
                return;
            }

            for (int i = array.Length - 1; i > index; i--)
                array[i] = array[i - 1];

            array[index] = value;
        }

        /// <summary>Formats a whole array into a string, comma separated. Useful for debugging.</summary>
        public static string ToStringLong<T>(this T[] array)
        {
            if (array.Length <= 0)
                return string.Empty;

            const string coma = ", ";
            StringBuilder builder = new StringBuilder();
            builder.Append(array[0].ToString());
            for (int i = 1; i < array.Length; i++) {
                builder.Append(coma);
                builder.Append(array[i].ToString());
            }

            return builder.ToString();
        }

        /// <summary>Overflows (wraps) an integer between a min and a max.</summary>
        /// <param name="number"></param>
        /// <param name="max">Maximum value (inclusive).</param>
        /// <param name="min">Minimum value (inclusive).</param>
        public static void Wrap(this ref int number, int max, int min = 0)
        {
            // If min equals max, that is the answer
            if (max == min) {
                number = max;
                return;
            }
            // Make sure max is grater than min
            if (max < min)
                Swap(ref min, ref max);

            int difference = max - min;
            while (number > max)
                number -= difference;
            while (number < min)
                number += difference;
        }

        /// <summary>Ping-pongs an integer in between two others.</summary>
        /// <param name="max">Maximum value (inclusive).</param>
        /// <param name="min">Minimum value (inclusive).</param>
        public static int PingPong(this ref int number, int max, int min = 0)
        {
            // If min equals max, that is the answer
            if (max == min)
                return max;

            // Make sure max is grater than min
            if (max < min)
                Swap(ref min, ref max);

            int difference = max - min;
            while (number > max + difference)
                number -= difference * 2;
            while (number < min)
                number += difference * 2;

            int result = number;
            if (result > max)
                result -= 2 * (result - max);
            return result;
        }
    }
}