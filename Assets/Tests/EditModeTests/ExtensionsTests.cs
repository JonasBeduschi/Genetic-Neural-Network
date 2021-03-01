using NUnit.Framework;
using GeneticNeuralNetwork;

public class ExtensionsTests
{
    #region Copy Tests

    [Test]
    [TestCase(new int[] { 0, 1, 2, 3, 4 }, new int[] { }, 0, 0)]
    [TestCase(new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1 }, 0, 2)]
    [TestCase(new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 }, 0, 5)]
    [TestCase(new int[] { 0, 1, 2, 3, 4 }, new int[] { 2, 3, 4 }, 2, 3)]
    [TestCase(new int[] { 0, 1, 2, 3, 4 }, new int[] { 2, 3, 4, 0, 0 }, 2, 5)]
    public void Copy_CopiesArray(int[] original, int[] expected, int startIndex, int length)
    {
        int[] actual = original.Copy(startIndex, length);

        Assert.AreEqual(expected, actual);
    }

    #endregion Copy Tests

    #region Transposed Tests

    [Test]
    public void Transposed_EmptyMatrix_ReturnsEmptyMatrix()
    {
        Matrix original = new Matrix(new float[,] { });
        Matrix expected = new Matrix(new float[,] { });

        Matrix transposed = original.Transposed();

        Assert.AreEqual(expected, transposed);
    }

    [Test]
    public void Transposed_SingleValueMatrix_ReturnsTransposedMatrix()
    {
        Matrix original = new Matrix(new float[,] { { 5 } });
        Matrix expected = new Matrix(new float[,] { { 5 } });

        Matrix transposed = original.Transposed();

        Assert.AreEqual(expected, transposed);
    }

    [Test]
    public void Transposed_NonSquareMatrix_ReturnsTransposedMatrix()
    {
        Matrix original = new Matrix(new float[,] { { 0, 1, 2 }, { 3, 4, 5 } });
        Matrix expected = new Matrix(new float[,] { { 0, 3 }, { 1, 4 }, { 2, 5 } });

        Matrix transposed = original.Transposed();

        Assert.AreEqual(expected, transposed);
    }

    [Test]
    public void Transposed_SquareMatrix_ReturnsTransposedMatrix()
    {
        Matrix original = new Matrix(new float[,] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 } });
        Matrix expected = new Matrix(new float[,] { { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 } });

        Matrix transposed = original.Transposed();

        Assert.AreEqual(expected, transposed);
    }

    #endregion Transposed Tests

    #region Average Tests

    [Test]
    [TestCase(1, 2, 1.5f)]
    [TestCase(0, 0, 0)]
    [TestCase(-20, -40, -30)]
    [TestCase(99, .5f, 49.75f)]
    [TestCase(99.999f, .001f, 50)]
    [TestCase(200_000_000, 0, 100_000_000)]
    public void Average_RegularInput_Works(float a, float b, float expected)
    {
        float actual = Extensions.Average(a, b);

        Assert.AreEqual(expected, actual);
    }

    #endregion Average Tests

    #region Swap Tests

    [Test]
    [TestCase(0, 0, 0, 0)]
    [TestCase(0, 9, 9, 0)]
    [TestCase(-15, 0, 0, -15)]
    [TestCase(-20, 11, 11, -20)]
    [TestCase(12, 16, 16, 12)]
    [TestCase(-4, -2, -2, -4)]
    public void Swap_RegularCase_Works(int originalA, int originalB, int expectedA, int expectedB)
    {
        Extensions.Swap(ref originalA, ref originalB);
        Assert.That(originalA, Is.EqualTo(expectedA));
        Assert.That(originalB, Is.EqualTo(expectedB));
    }

    #endregion Swap Tests
}