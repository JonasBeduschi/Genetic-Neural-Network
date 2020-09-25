using UnityEngine;

public static class Extensions
{
    public static float[,] DotMultiply(this float[,] a, float[,] b)
    {
        float[,] matrix = new float[a.GetLength(0), b.GetLength(1)];
        if (a.GetLength(1) != b.GetLength(0)) {
            Debug.LogError("Matrix cannot be multiplied");
            Debug.Log($"a[{a.GetLength(0)},{a.GetLength(1)}] and b[{b.GetLength(0)},{b.GetLength(1)}]");
            return matrix;
        }

        float temp;

        for (int row = 0; row < matrix.GetLength(0); row++) {
            for (int collum = 0; collum < matrix.GetLength(1); collum++) {
                temp = 0;
                for (int i = 0; i < a.GetLength(1); i++) {
                    temp += a[row, i] * b[i, collum];
                }
                matrix[row, collum] = temp;
            }
        }
        return matrix;
    }

    public static float[,] NormalMultiply(this float[,] a, float[,] b)
    {
        if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1)) {
            Debug.LogError("Matrices are not the same size");
            return a;
        }
        for (int row = 0; row < a.GetLength(0); row++)
            for (int collum = 0; collum < a.GetLength(1); collum++)
                a[row, collum] += a[row, collum] * b[row, collum];
        return a;
    }

    public static float[,] RandomMatrix(int row, int collum, float min = -.5f, float max = .5f)
    {
        float[,] matrix = new float[row, collum];
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                matrix[i, j] = Random.Range(min, max);
            }
        }
        return matrix;
    }

    public static float[] Subtract(this float[] a, float[] b)
    {
        if (a.Length != b.Length)
            Debug.LogError("Cannot subtract arrays of different length");
        else
            for (int i = 0; i < a.Length; i++)
                a[i] -= b[i];
        return a;
    }

    /// <summary>0 to 1 range</summary>
    public static void Activate(this float x)
    {
        x = 1f / (1 + Mathf.Exp(-x));
    }

    public static float[,] Activate(this float[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
            for (int j = 0; j < matrix.GetLength(1); j++)
                matrix[i, j].Activate();
        return matrix;
    }

    public static float[] ToArray(this float[,] matrix)
    {
        float[] array = new float[matrix.GetLength(0)];
        for (int i = 0; i < array.Length; i++)
            array[i] = matrix[i, 0];
        return array;
    }

    public static float[,] ToMatrix(this float[] array)
    {
        float[,] matrix = new float[array.Length, 1];
        for (int i = 0; i < array.Length; i++)
            matrix[i, 0] = array[i];
        return matrix;
    }

    public static float[,] Transpose(this float[,] matrix)
    {
        float[,] transposed = new float[matrix.GetLength(1), matrix.GetLength(0)];
        for (int i = 0; i < matrix.GetLength(0); i++)
            for (int j = 0; j < matrix.GetLength(1); j++)
                transposed[j, i] = matrix[i, j];
        return transposed;
    }

    public static float[,] Copy(this float[,] original)
    {
        float[,] matrix = new float[original.GetLength(0), original.GetLength(1)];
        for (int i = 0; i < matrix.GetLength(0); i++)
            for (int j = 0; j < matrix.GetLength(1); j++)
                matrix[i, j] = original[i, j];
        return matrix;
    }

    public static float Power(this float number, int power)
    {
        if (power == 0)
            return 1f;
        if (power < 0)
            return Mathf.Pow(number, power);
        float result = number;
        while (power > 1) {
            result *= number;
            power--;
        }
        return result;
    }
}