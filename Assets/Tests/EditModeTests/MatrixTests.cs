using GeneticNeuralNetwork;
using NUnit.Framework;
using System.Collections.Generic;

public class MatrixTests
{
    [Test]
    public void ConstructorCreatesWithCorrectRows()
    {
        Matrix m = new Matrix(5, 10);
        Assert.AreEqual(5, m.array.GetLength(0));
    }

    [Test]
    public void ConstructorCreatesWithCorrectCollums()
    {
        Matrix m = new Matrix(5, 10);
        Assert.AreEqual(10, m.array.GetLength(1));
    }

    [Test]
    public void RowsGetsCorrectValue()
    {
        Matrix m = new Matrix(5, 10);
        Assert.AreEqual(5, m.Rows);
    }

    [Test]
    public void CollumsGetsCorrectValue()
    {
        Matrix m = new Matrix(5, 10);
        Assert.AreEqual(10, m.Collums);
    }

    [Test]
    public void LengthGetsCorrectValue()
    {
        Matrix m = new Matrix(5, 10);
        Assert.AreEqual(50, m.Length);
    }

    [Test]
    public void ConstructorWithNegativeRowsCreatesMatrixWithZeroRows()
    {
        Matrix m = new Matrix(-3, 5);
        Assert.AreEqual(0, m.Rows);
    }

    [Test]
    public void ConstructorWithNegativeCollumsCreatesMatrixWithZeroCollums()
    {
        Matrix m = new Matrix(5, -3);
        Assert.AreEqual(0, m.Collums);
    }

    [Test]
    public void ConstructorBasedOnMultidimensionalArrayWorks()
    {
        float[,] array = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix m = new Matrix(array);
        Assert.AreEqual(array, m.array);
    }

    [Test]
    public void ConstructorBasedOnArrayWorks()
    {
        float[] array = new float[3] { 1, 2, 3 };
        Matrix m = new Matrix(array);
        Assert.AreEqual(array, m.ToArray());
    }

    [Test]
    public void ConstructorBasedMatrixWorks()
    {
        float[,] array = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix original = new Matrix(array);
        Matrix copy = new Matrix(original);
        Assert.AreEqual(original, copy);
    }

    [Test]
    [TestCase(1, 2, 2, 2)]
    [TestCase(1, 2, 1, 1)]
    public void EqualsDifferentLengths(int rowA, int collumA, int rowB, int collumB)
    {
        Matrix a = new Matrix(rowA, collumA);
        Matrix b = new Matrix(rowB, collumB);
        Assert.IsFalse(a.Equals(b));
    }

    [Test]
    public void EqualsDifferentContent()
    {
        float[,] arrayA = new float[2, 3] { { 0, 2, 3 }, { 4, 5, 6 } };
        float[,] arrayB = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix a = new Matrix(arrayA);
        Matrix b = new Matrix(arrayB);
        Assert.IsFalse(a.Equals(b));
    }

    [Test]
    public void EqualsSameContent()
    {
        float[,] arrayA = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        float[,] arrayB = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix a = new Matrix(arrayA);
        Matrix b = new Matrix(arrayB);
        Assert.IsTrue(a.Equals(b));
    }

    [Test]
    [TestCase(0, 0, true)]
    [TestCase(2, 3, false)]
    [TestCase(1, 0, false)]
    [TestCase(4, 4, true)]
    public void IsSquaredWorks(int rows, int collums, bool expected)
    {
        Matrix m = new Matrix(rows, collums);
        Assert.AreEqual(expected, m.IsSquared);
    }

    private static IEnumerable<float[,]> IdentityMatrixes()
    {
        yield return new float[,] { { 1 } };
        yield return new float[,] { { 1, 0 }, { 0, 1 } };
        yield return new float[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
    }

    [Test, TestCaseSource(nameof(IdentityMatrixes))]
    public void IsIdentityWorksWithIdentityMatrixes(float[,] array)
    {
        Matrix m = new Matrix(array);
        Assert.AreEqual(true, m.IsIdentity);
    }

    private static IEnumerable<float[,]> NonIdentityMatrixes()
    {
        yield return new float[,] { { 0 } };
        yield return new float[,] { { 0 }, { 0 } };
        yield return new float[,] { { 1, 1 }, { 0, 1 } };
        yield return new float[,] { { 0, 1 }, { 1, 0 } };
        yield return new float[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }

    [Test, TestCaseSource(nameof(NonIdentityMatrixes))]
    public void IsIdentityWorksWithNonIdentityMatrixes(float[,] array)
    {
        Matrix m = new Matrix(array);
        Assert.AreEqual(false, m.IsIdentity);
    }

    [Test]
    public void MultiplicationWorks()
    {
        float[,] arrayA = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        float[,] arrayB = new float[3, 2] { { 7, 8 }, { 9, 10 }, { 11, 12 } };
        float[,] arrayE = new float[2, 2] { { 58, 64 }, { 139, 154 } };
        Matrix a = new Matrix(arrayA);
        Matrix b = new Matrix(arrayB);
        Assert.AreEqual(arrayE, (a * b).array);
    }

    [Test]
    public void ToArrayWorks()
    {
        float[,] arrayA = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix matrix = new Matrix(arrayA);
        float[] expected = new float[] { 1, 2, 3, 4, 5, 6 };
        Assert.AreEqual(expected, matrix.ToArray());
    }

    [Test]
    public void EqualsOperatorWorks()
    {
        float[,] array = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix matrixA = new Matrix(array);
        Matrix matrixB = new Matrix(array);
        Assert.IsTrue(matrixA == matrixB);
    }

    [Test]
    public void NotEqualOperatorWorks()
    {
        float[,] arrayA = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix matrixA = new Matrix(arrayA);
        float[,] arrayB = new float[2, 3] { { 1, 2, 3 }, { 4, 5, 0 } };
        Matrix matrixB = new Matrix(arrayB);
        Assert.IsTrue(matrixA != matrixB);
    }
}