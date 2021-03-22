using FlappyPlane;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FlappyExtensionsTests
{
    #region PingPong Tests

    [Test, Category("Edge case")]
    public void PingPong_SameMinMax_Works()
    {
        int number = 5;
        int actual = number.PingPong(0);
        Assert.That(actual, Is.EqualTo(0));
    }

    [Test, Category("Edge case")]
    public void PingPong_InvertedMinMax_Works()
    {
        int number = 5;
        int actual = number.PingPong(0, 4);
        Assert.That(actual, Is.EqualTo(3));
    }

    [Test]
    [TestCase(5, 0, 1, ExpectedResult = 1)]
    [TestCase(0, 0, 2, ExpectedResult = 0)]
    [TestCase(1, 0, 2, ExpectedResult = 1)]
    [TestCase(2, 0, 2, ExpectedResult = 2)]
    [TestCase(3, 0, 2, ExpectedResult = 1)]
    [TestCase(8, 0, 2, ExpectedResult = 0)]
    [TestCase(5, 0, 4, ExpectedResult = 3)]
    [TestCase(9, -3, 4, ExpectedResult = -1)]
    [TestCase(-2, 0, 5, ExpectedResult = 2)]
    public int PingPong_RegularCases_Works(int number, int min, int max)
    {
        return number.PingPong(max, min);
    }

    #endregion PingPong Tests

    #region ToStringLong Tests

    [Test]
    [TestCase(new int[] { }, ExpectedResult = "")]
    [TestCase(new int[] { 0 }, ExpectedResult = "0")]
    [TestCase(new int[] { 0, 1 }, ExpectedResult = "0, 1")]
    [TestCase(new int[] { 0, 1, 2, 3, 4 }, ExpectedResult = "0, 1, 2, 3, 4")]
    public string ToString_RegularCases_Works(int[] array)
    {
        return array.ToStringLong();
    }

    #endregion ToStringLong Tests

    #region Capped Tests

    [Test, Category("Edge case")]
    public void Capped_SameMinMax_Works()
    {
        float number = 0;
        float actual = number.Capped(5, 5);
        Assert.That(actual, Is.EqualTo(5));
    }

    [Test, Category("Edge case")]
    public void Capped_InvertedMinMax_Works()
    {
        float number = 0;
        float actual = number.Capped(-2, -10);
        Assert.That(actual, Is.EqualTo(-2));
    }

    [Test]
    [TestCase(10, 3, 7, ExpectedResult = 7)]
    [TestCase(5, 6, 9, ExpectedResult = 6)]
    [TestCase(3, 0, 10, ExpectedResult = 3)]
    [TestCase(-10, -8, -3, ExpectedResult = -8)]
    [TestCase(-10, -13, -3, ExpectedResult = -10)]
    [TestCase(-1, -10, -3, ExpectedResult = -3)]
    public float Capped_RegularCases_Works(float number, float min, float max)
    {
        return number.Capped(min, max);
    }

    #endregion Capped Tests

    #region Swap Tests

    [Test]
    public void Swap_RegularCase_Works()
    {
        int a = 2, b = 3;
        Extensions.Swap(ref a, ref b);
        Assert.That(a, Is.EqualTo(3));
        Assert.That(b, Is.EqualTo(2));
    }

    #endregion Swap Tests

    #region PushAt Tests

    [Test, Category("Edge case")]
    public void PushAt_IndexOutOfBounds_LogsError()
    {
        int[] array = new int[] { 0 };
        int value = 4, index = 2;
        array.PushAt(index, value);
        LogAssert.Expect(LogType.Error, "Index (2) out of range (1)");
    }

    [Test, Category("Edge case")]
    public void PushAt_IndexOutOfBounds_DoesNotChangeArray()
    {
        int[] array = new int[] { 0 };
        int[] expected = new int[] { 0 };
        int value = 4, index = 2;

        array.PushAt(index, value);

        LogAssert.Expect(LogType.Error, "Index (2) out of range (1)");
        Assert.AreEqual(expected, array);
    }

    [Test]
    public void PushAt_RegularCases_Works()
    {
        int[] array = new int[] { 0, 1, 2, 3 };
        int[] expected = new int[] { 0, 1, 4, 2 };
        int value = 4, index = 2;

        array.PushAt(index, value);

        Assert.AreEqual(expected, array);
    }

    #endregion PushAt Tests
}