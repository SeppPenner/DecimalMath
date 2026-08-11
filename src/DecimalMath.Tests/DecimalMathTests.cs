// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalMathTests.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the DecimalMath class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DecimalMath.Tests;

/// <summary>
/// A class to test the DecimalMath class. The functions are Taylor series and Newton iterations over
/// <see cref="decimal"/>, so most results are compared with a tolerance. Only the results that the implementation
/// reaches by multiplication alone are compared exactly.
/// </summary>
[TestClass]
public class DecimalMathTests
{
    /// <summary>
    /// The tolerance for a comparison against <see cref="Math"/>. A <see cref="double"/> carries 15 to 17
    /// significant digits, everything below that is not comparable at all.
    /// </summary>
    private const double DoubleTolerance = 1e-9;

    /// <summary>
    /// The tolerance for a comparison of two <see cref="decimal"/> values, three digits above the smallest
    /// <see cref="decimal"/> to leave room for the rounding of the series.
    /// </summary>
    private const decimal DecimalTolerance = 0.0000000000000000000000001M;

    /// <summary>
    /// The arguments in radians that the trigonometric functions are checked with. The values beyond two PI check
    /// the range reduction, the negative ones the symmetry.
    /// </summary>
    private static readonly decimal[] TrigonometryValues = [0M, 0.1M, 0.5M, 1M, -1M, 1.5M, 3M, 3.5M, 6M, 7M, 100M, -100M];

    /// <summary>
    /// The arguments that the inverse trigonometric functions are checked with, the full definition range of
    /// arcus sinus and arcus cosinus.
    /// </summary>
    private static readonly decimal[] InverseTrigonometryValues = [-1M, -0.5M, 0M, 0.5M, 0.9M, 1M];

    /// <summary>
    /// Checks whether the public constants carry the value of their mathematical counterpart.
    /// </summary>
    [TestMethod]
    public void ConstantsCarryTheirMathematicalValue()
    {
        // The comparison goes through the helper for the plain constants as well, a direct
        // Assert.AreEqual of two compile time constants is rejected by MSTEST0032.
        AssertMatchesSystemMath(Math.E, DecMath.E);
        AssertMatchesSystemMath(Math.PI, DecMath.Pi);
        AssertMatchesSystemMath(0.0, DecMath.Zero);
        AssertMatchesSystemMath(1.0, DecMath.One);
        AssertMatchesSystemMath(1e-19, DecMath.Epsilon);
    }

    /// <summary>
    /// Checks whether the absolute value is returned for a negative, a positive and the smallest value.
    /// </summary>
    [TestMethod]
    public void AbsReturnsTheAbsoluteValue()
    {
        Assert.AreEqual(5.5M, DecMath.Abs(-5.5M));
        Assert.AreEqual(5.5M, DecMath.Abs(5.5M));
        Assert.AreEqual(0M, DecMath.Abs(0M));
        Assert.AreEqual(decimal.MaxValue, DecMath.Abs(decimal.MinValue));
    }

    /// <summary>
    /// Checks whether the absolute value of zero is a zero without the sign bit. Negating a zero returns a
    /// <see cref="decimal"/> that <see cref="decimal.IsNegative"/> reports as negative, which is what the
    /// comparison with the smaller or equal operator produced before.
    /// </summary>
    [TestMethod]
    public void AbsReturnsAZeroWithoutTheSignBit()
    {
        Assert.IsFalse(decimal.IsNegative(DecMath.Abs(0.0M)));
        Assert.IsFalse(decimal.IsNegative(DecMath.Abs(DecMath.Zero)));
    }

    /// <summary>
    /// Checks whether the sign of a negative value, of zero and of a positive value is returned.
    /// </summary>
    [TestMethod]
    public void SignReturnsTheSignOfTheValue()
    {
        Assert.AreEqual(-1, DecMath.Sign(-0.5M));
        Assert.AreEqual(0, DecMath.Sign(0M));
        Assert.AreEqual(1, DecMath.Sign(0.5M));
    }

    /// <summary>
    /// Checks whether sinus and cosinus match <see cref="Math"/> over the whole set of test arguments, the
    /// values outside the interval of two PI included.
    /// </summary>
    [TestMethod]
    public void SinAndCosMatchSystemMath()
    {
        foreach (var value in TrigonometryValues)
        {
            AssertMatchesSystemMath(Math.Sin((double)value), DecMath.Sin(value));
            AssertMatchesSystemMath(Math.Cos((double)value), DecMath.Cos(value));
        }
    }

    /// <summary>
    /// Checks whether the known values at zero, PI/2 and PI are exact. The multiplication and the square root
    /// behind them do not lose a digit, unlike the series.
    /// </summary>
    [TestMethod]
    public void SinAndCosReturnTheKnownValuesExactly()
    {
        Assert.AreEqual(0M, DecMath.Sin(0M));
        Assert.AreEqual(1M, DecMath.Cos(0M));
        Assert.AreEqual(1M, DecMath.Sin(DecMath.Pi / 2));
        Assert.AreEqual(0M, DecMath.Sin(DecMath.Pi));
        Assert.AreEqual(-1M, DecMath.Cos(DecMath.Pi));
    }

    /// <summary>
    /// Checks whether the Pythagorean identity holds for an argument that hits neither a known value nor a
    /// symmetry.
    /// </summary>
    [TestMethod]
    public void SinAndCosKeepThePythagoreanIdentity()
    {
        var sin = DecMath.Sin(0.7M);
        var cos = DecMath.Cos(0.7M);

        AssertIsCloseTo(1M, (sin * sin) + (cos * cos), DecimalTolerance);
    }

    /// <summary>
    /// Checks whether the tangens matches <see cref="Math"/> and stays the quotient of sinus and cosinus.
    /// </summary>
    [TestMethod]
    public void TanMatchesSystemMath()
    {
        foreach (var value in new decimal[] { 0M, 0.5M, 1M, -1M, 1.5M })
        {
            AssertMatchesSystemMath(Math.Tan((double)value), DecMath.Tan(value));
            AssertIsCloseTo(DecMath.Sin(value) / DecMath.Cos(value), DecMath.Tan(value), DecimalTolerance);
        }
    }

    /// <summary>
    /// Checks whether the tangens of PI/2 returns a very large value instead of throwing. The guard in the
    /// function only triggers on a cosinus of exactly zero, and Cos(PI/2) returns -2E-28 here, because the
    /// constant PI is a rounded decimal.
    /// </summary>
    [TestMethod]
    public void TanOfHalfPiReturnsALargeValueInsteadOfThrowing()
    {
        Assert.IsTrue(DecMath.Abs(DecMath.Tan(DecMath.Pi / 2)) > 1000000000000000000000000M);
    }

    /// <summary>
    /// Checks whether arcus sinus, arcus cosinus and arcus tangens match <see cref="Math"/> over the whole
    /// definition range.
    /// </summary>
    [TestMethod]
    public void AsinAcosAndAtanMatchSystemMath()
    {
        foreach (var value in InverseTrigonometryValues)
        {
            AssertMatchesSystemMath(Math.Asin((double)value), DecMath.Asin(value));
            AssertMatchesSystemMath(Math.Acos((double)value), DecMath.Acos(value));
            AssertMatchesSystemMath(Math.Atan((double)value), DecMath.Atan(value));
        }
    }

    /// <summary>
    /// Checks whether an argument outside the interval from minus one to one is rejected by the arcus sinus and,
    /// through it, by the arcus cosinus.
    /// </summary>
    [TestMethod]
    public void AsinAndAcosThrowOutsideTheirDefinitionRange()
    {
        var exception = Assert.ThrowsExactly<ArgumentException>(() => { _ = DecMath.Asin(2M); });
        Assert.AreEqual("x must be in [-1,1]", exception.Message);
        Assert.ThrowsExactly<ArgumentException>(() => { _ = DecMath.Asin(-2M); });
        Assert.ThrowsExactly<ArgumentException>(() => { _ = DecMath.Acos(2M); });
    }

    /// <summary>
    /// Checks whether the angle of all four quadrants and of the four axes matches <see cref="Math"/>.
    /// </summary>
    [TestMethod]
    public void Atan2ReturnsTheAngleOfEveryQuadrant()
    {
        foreach (var (y, x) in new (decimal Y, decimal X)[] { (1M, 1M), (1M, -1M), (-1M, -1M), (-1M, 1M), (1M, 0M), (-1M, 0M), (0M, 1M), (0M, -1M) })
        {
            AssertMatchesSystemMath(Math.Atan2((double)y, (double)x), DecMath.Atan2(y, x));
        }
    }

    /// <summary>
    /// Checks whether both arguments being zero throws. This differs from <see cref="Math.Atan2"/>, which
    /// returns zero for it, and it is kept because a consumer may catch it.
    /// </summary>
    [TestMethod]
    public void Atan2ThrowsForBothArgumentsZero()
    {
        var exception = Assert.ThrowsExactly<ArgumentException>(() => { _ = DecMath.Atan2(0M, 0M); });

        Assert.AreEqual("invalid atan2 arguments", exception.Message);
        Assert.AreEqual(0.0, Math.Atan2(0.0, 0.0));
    }

    /// <summary>
    /// Checks whether the exponential function matches <see cref="Math"/> for positive and negative arguments.
    /// </summary>
    [TestMethod]
    public void ExpMatchesSystemMath()
    {
        foreach (var value in new decimal[] { 0M, 0.5M, 1M, 2M, 10M, -1M, -5M })
        {
            AssertMatchesSystemMath(Math.Exp((double)value), DecMath.Exp(value));
        }
    }

    /// <summary>
    /// Checks whether the logarithm and the logarithm to base ten match <see cref="Math"/>.
    /// </summary>
    [TestMethod]
    public void LogAndLog10MatchSystemMath()
    {
        foreach (var value in new decimal[] { 0.5M, 1M, 2M, 10M, 100M, 10000000000M })
        {
            AssertMatchesSystemMath(Math.Log((double)value), DecMath.Log(value));
            AssertMatchesSystemMath(Math.Log10((double)value), DecMath.Log10(value));
        }
    }

    /// <summary>
    /// Checks whether the logarithm of one is exactly zero. The series returned 1E-28 for it, because the
    /// rounded constants E and EInverted do not multiply to exactly one.
    /// </summary>
    [TestMethod]
    public void LogOfOneIsExactlyZero()
    {
        Assert.AreEqual(0M, DecMath.Log(1M));
        Assert.AreEqual(0M, DecMath.Log10(1M));
    }

    /// <summary>
    /// Checks whether zero and a negative argument are rejected by the logarithm.
    /// </summary>
    [TestMethod]
    public void LogThrowsForZeroAndNegativeValues()
    {
        var exception = Assert.ThrowsExactly<ArgumentException>(() => { _ = DecMath.Log(0M); });
        Assert.AreEqual("x must be greater than zero", exception.Message);
        Assert.ThrowsExactly<ArgumentException>(() => { _ = DecMath.Log(-1M); });
    }

    /// <summary>
    /// Checks whether the exponential function and the logarithm cancel each other out within the tolerance.
    /// </summary>
    [TestMethod]
    public void ExpAndLogAreInverseToEachOther()
    {
        AssertIsCloseTo(7M, DecMath.Exp(DecMath.Log(7M)), DecimalTolerance);
        AssertIsCloseTo(3M, DecMath.Log10(1000M), DecimalTolerance);
    }

    /// <summary>
    /// Checks whether an integer power returns an exact result. The negative base is the interesting one, it
    /// went through Exp and Log before and returned -8.000000000000000000000000008 for the first case.
    /// </summary>
    [TestMethod]
    public void PowerReturnsExactResultsForIntegerPowers()
    {
        Assert.AreEqual(-8M, DecMath.Power(-2M, 3M));
        Assert.AreEqual(4M, DecMath.Power(-2M, 2M));
        Assert.AreEqual(1024M, DecMath.Power(2M, 10M));
        Assert.AreEqual(0.25M, DecMath.Power(2M, -2M));
        Assert.AreEqual(100000000000000000000M, DecMath.Power(10M, 20M));
    }

    /// <summary>
    /// Checks whether a non-integer power still goes through the exponential function and the logarithm and
    /// therefore only matches <see cref="Math"/> within the tolerance.
    /// </summary>
    [TestMethod]
    public void PowerMatchesSystemMathForNonIntegerPowers()
    {
        AssertMatchesSystemMath(Math.Pow(2.0, 0.5), DecMath.Power(2M, 0.5M));
        AssertMatchesSystemMath(Math.Pow(9.0, 0.5), DecMath.Power(9M, 0.5M));
        AssertMatchesSystemMath(Math.Pow(2.0, 1.5), DecMath.Power(2M, 1.5M));
    }

    /// <summary>
    /// Checks whether the special cases of the base and of the power are answered without a calculation.
    /// </summary>
    [TestMethod]
    public void PowerAnswersTheSpecialCases()
    {
        Assert.AreEqual(1M, DecMath.Power(12.34M, 0M));
        Assert.AreEqual(12.34M, DecMath.Power(12.34M, 1M));
        Assert.AreEqual(1M, DecMath.Power(1M, 12345M));
        Assert.AreEqual(0M, DecMath.Power(0M, 2M));
        Assert.AreEqual(0.5M, DecMath.Power(2M, -1M));
    }

    /// <summary>
    /// Checks whether a zero base with a negative power and a negative base with a non-integer power are
    /// rejected. Both throw the base exception type, which is kept for the consumers that catch it.
    /// </summary>
    [TestMethod]
    public void PowerThrowsForTheUndefinedCombinations()
    {
        var zeroBase = Assert.ThrowsExactly<Exception>(() => { _ = DecMath.Power(0M, -1M); });
        Assert.AreEqual("Invalid Operation: zero base and negative power", zeroBase.Message);

        var negativeBase = Assert.ThrowsExactly<Exception>(() => { _ = DecMath.Power(-2M, 0.5M); });
        Assert.AreEqual("Invalid Operation: negative base and non-integer power", negativeBase.Message);
    }

    /// <summary>
    /// Checks whether a representable result is returned instead of an overflow. The squaring of the running
    /// base happened once more than needed, so the last squaring left the range of <see cref="decimal"/>
    /// although the result itself fits into it. Two to the power of 96 is one more than
    /// <see cref="decimal.MaxValue"/> and still has to throw.
    /// </summary>
    [TestMethod]
    public void PowerNReturnsRepresentableResultsWithoutAnOverflow()
    {
        Assert.AreEqual(100000000000000000000M, DecMath.PowerN(10M, 20));
        Assert.AreEqual(39614081257132168796771975168M, DecMath.PowerN(2M, 95));
        Assert.ThrowsExactly<OverflowException>(() => { _ = DecMath.PowerN(2M, 96); });
    }

    /// <summary>
    /// Checks whether an integer power is calculated for a negative power and for the power of zero.
    /// </summary>
    [TestMethod]
    public void PowerNHandlesNegativeAndZeroPowers()
    {
        Assert.AreEqual(0.125M, DecMath.PowerN(2M, -3));
        Assert.AreEqual(1M, DecMath.PowerN(12.34M, 0));
        Assert.AreEqual(-32M, DecMath.PowerN(-2M, 5));
        Assert.AreEqual(16M, DecMath.PowerN(-2M, 4));
    }

    /// <summary>
    /// Checks whether the square root matches <see cref="Math"/> up to the largest <see cref="decimal"/>, which
    /// is also the check that the Newton iteration terminates without the epsilon parameter.
    /// </summary>
    [TestMethod]
    public void SqrtMatchesSystemMath()
    {
        foreach (var value in new decimal[] { 0M, 0.1M, 0.5M, 1M, 2M, 7M, 10M, 123456.789M, 10000000000M })
        {
            AssertMatchesSystemMath(Math.Sqrt((double)value), DecMath.Sqrt(value));
        }

        AssertMatchesSystemMath(Math.Sqrt((double)decimal.MaxValue), DecMath.Sqrt(decimal.MaxValue), 1e-3);
    }

    /// <summary>
    /// Checks whether the square root squared returns the argument again and whether zero is answered directly.
    /// </summary>
    [TestMethod]
    public void SqrtSquaredReturnsTheArgument()
    {
        var root = DecMath.Sqrt(2M);

        AssertIsCloseTo(2M, root * root, DecimalTolerance);
        Assert.AreEqual(0M, DecMath.Sqrt(0M));
        Assert.AreEqual(3M, DecMath.Sqrt(9M));
    }

    /// <summary>
    /// Checks whether a larger epsilon stops the iteration earlier without leaving the tolerance it was given.
    /// </summary>
    [TestMethod]
    public void SqrtRespectsTheEpsilonParameter()
    {
        AssertMatchesSystemMath(Math.Sqrt(2.0), DecMath.Sqrt(2M, 0.001M), 1e-3);
    }

    /// <summary>
    /// Checks whether a negative argument is rejected. The type is an <see cref="OverflowException"/> and not an
    /// <see cref="ArgumentException"/>, which is kept for the consumers that catch it.
    /// </summary>
    [TestMethod]
    public void SqrtThrowsForANegativeValue()
    {
        var exception = Assert.ThrowsExactly<OverflowException>(() => { _ = DecMath.Sqrt(-1M); });

        Assert.AreEqual("Cannot calculate square root from a negative number", exception.Message);
    }

    /// <summary>
    /// Checks whether the hyperbolic functions match <see cref="Math"/>.
    /// </summary>
    [TestMethod]
    public void SinhCoshAndTanhMatchSystemMath()
    {
        foreach (var value in new decimal[] { 0M, 0.5M, 1M, -1M, 1.5M })
        {
            AssertMatchesSystemMath(Math.Sinh((double)value), DecMath.Sinh(value));
            AssertMatchesSystemMath(Math.Cosh((double)value), DecMath.Cosh(value));
            AssertMatchesSystemMath(Math.Tanh((double)value), DecMath.Tanh(value));
        }
    }

    /// <summary>
    /// Checks whether the hyperbolic identity holds, the counterpart of the Pythagorean identity.
    /// </summary>
    [TestMethod]
    public void CoshAndSinhKeepTheHyperbolicIdentity()
    {
        var cosh = DecMath.Cosh(1M);
        var sinh = DecMath.Sinh(1M);

        AssertIsCloseTo(1M, (cosh * cosh) - (sinh * sinh), DecimalTolerance);
    }

    /// <summary>
    /// Asserts that a result matches the <see cref="double"/> result of <see cref="Math"/>.
    /// </summary>
    /// <param name="expected">The result of the counterpart in <see cref="Math"/>.</param>
    /// <param name="actual">The result of the function under test.</param>
    /// <param name="tolerance">The allowed difference, <see cref="DoubleTolerance"/> if not given.</param>
    private static void AssertMatchesSystemMath(double expected, decimal actual, double tolerance = DoubleTolerance)
    {
        var difference = Math.Abs((double)actual - expected);

        Assert.IsTrue(
            difference <= tolerance,
            $"Expected {expected}, but got {actual}. The difference {difference:E2} is larger than the tolerance {tolerance:E2}.");
    }

    /// <summary>
    /// Asserts that two <see cref="decimal"/> results do not differ by more than the given tolerance.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The result of the function under test.</param>
    /// <param name="tolerance">The allowed difference.</param>
    private static void AssertIsCloseTo(decimal expected, decimal actual, decimal tolerance)
    {
        var difference = DecMath.Abs(actual - expected);

        Assert.IsTrue(
            difference <= tolerance,
            $"Expected {expected}, but got {actual}. The difference {difference} is larger than the tolerance {tolerance}.");
    }
}
