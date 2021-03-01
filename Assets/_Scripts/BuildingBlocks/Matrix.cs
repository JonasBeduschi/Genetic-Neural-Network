using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GeneticNeuralNetwork
{
    public struct Matrix
    {
        public float[,] array { get; private set; }
        public float this[int row, int collum]
        {
            get => array[row, collum];
            set => array[row, collum] = value;
        }
        public int Rows { get => array.GetLength(0); }
        public int Collums { get => array.GetLength(1); }
        public int Length { get => Rows * Collums; }
        public bool IsSquared { get => Rows.Equals(Collums); }

        public bool IsIdentity
        {
            get
            {
                if (!IsSquared)
                    return false;
                for (int r = 0; r < Rows; r++) {
                    for (int c = 0; c < Collums; c++) {
                        if (r.Equals(c)) {
                            if (!array[r, c].Equals(1))
                                return false;
                        }
                        else if (!array[r, c].Equals(0))
                            return false;
                    }
                }
                return true;
            }
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(a.Rows, b.Collums);

            if (a.Collums != b.Rows) {
                Debug.LogError("Matrix cannot be multiplied");
                Debug.Log($"a[{a.Rows},{a.Collums}] and b[{b.Rows},{b.Collums}]");
                return result;
            }
            float temp;
            for (int r = 0; r < result.Rows; r++) {
                for (int c = 0; c < result.Collums; c++) {
                    temp = 0;
                    for (int k = 0; k < a.Collums; k++) {
                        temp += a.array[r, k] * b.array[k, c];
                    }
                    result.array[r, c] = temp;
                }
            }
            return result;
        }

        public Matrix(float[,] matrix)
        {
            array = new float[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    array[i, j] = matrix[i, j];
                }
            }
        }

        public Matrix(float[] array)
        {
            this.array = new float[array.Length, 1];
            for (int i = 0; i < array.Length; i++)
                this.array[i, 0] = array[i];
        }

        public Matrix(Matrix original)
        {
            array = new float[original.Rows, original.Collums];
            for (int i = 0; i < original.Rows; i++) {
                for (int j = 0; j < original.Collums; j++) {
                    array[i, j] = original.array[i, j];
                }
            }
        }

        public Matrix(int rows, int collums)
        {
            if (rows < 0) {
                rows = 0;
                Debug.LogWarning("Creating a Matrix with zero rows");
            }
            if (collums < 0) {
                collums = 0;
                Debug.LogWarning("Creating a Matrix with zero collums");
            }
            array = new float[rows, collums];
        }

        public static Matrix RandomMatrix(int rows, int collums, float min = -.5f, float max = .5f)
        {
            float[,] array = new float[rows, collums];
            for (int r = 0; r < rows; r++) {
                for (int c = 0; c < collums; c++) {
                    array[r, c] = Random.Range(min, max);
                }
            }
            return new Matrix(array);
        }

        public float[] ToArray()
        {
            float[] result = new float[Length];
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Collums; j++)
                    result[Collums * i + j] = array[i, j];
            return result;
        }

        public void Activate()
        {
            for (int r = 0; r < Rows; r++) {
                for (int c = 0; c < Collums; c++) {
                    array[r, c].ActivateSigmoid();
                }
            }
        }

        public override bool Equals(object obj)
        {
            Matrix other = (Matrix)obj;
            if (other == null)
                return false;
            if (other.Collums != Collums || other.Rows != Rows)
                return false;
            for (int i = 0; i < Rows; i++) {
                for (int j = 0; j < Collums; j++) {
                    if (other.array[i, j] != array[i, j])
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            return !(left == right);
        }
    }
}