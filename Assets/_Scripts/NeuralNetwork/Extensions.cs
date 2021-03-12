using UnityEngine;
using System;
using GeneticNeuralNetwork;

public static class Extensions
{
    /// <summary>-1 to 1 range</summary>
    public static void ActivateSigmoid(this ref float x)
    {
        x = 1f / (1 + Mathf.Exp(-x)) * 2 - 1;
    }

    /// <summary>Anything positive range</summary>
    public static void ActivateReLU(this ref float x)
    {
        if (x < 0)
            x = 0;
    }

    public static T[] Copy<T>(this T[] original, int startIndex, int length)
    {
        if (length <= 0)
            return Array.Empty<T>();
        T[] result = new T[length];
        int maxIndex = original.Length - startIndex > length ? length : original.Length - startIndex;
        for (int i = 0; i < maxIndex; i++)
            result[i] = original[i + startIndex];
        return result;
    }

    public static T[] Copy<T>(this T[] original, int length)
    {
        return original.Copy(0, length);
    }

    public static Matrix Transposed(this Matrix original)
    {
        float[,] transposed = new float[original.Collums, original.Rows];
        for (int i = 0; i < original.Rows; i++)
            for (int j = 0; j < original.Collums; j++)
                transposed[j, i] = original[i, j];
        return new Matrix(transposed);
    }

    /// <summary>Caps a number capped in between two others.</summary>
    /// <param name="min">Minimum value of the number.</param>
    /// <param name="max">Maximum value of the number.</param>
    public static void Cap(this ref float number, float min = 0, float max = 1)
    {
        if (min > max)
            Swap(ref min, ref max);
        if (number > max)
            number = max;
        else if (number < min)
            number = min;
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

    public static Quaternion QuaternionFromDegrees(float degrees) => Quaternion.Euler(0, 0, degrees);

    public static float Average(float a, float b)
    {
        return (a + b) / 2;
    }
}