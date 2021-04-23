using System;
using Arnible.Assertions;
using Arnible.MathModeling.Analysis.Optimization;
using Arnible.MathModeling.Test;
using Arnible.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Arnible.MathModeling.Optimization.Test
{
  public class UnimodalSecantTests : TestsWithLogger
  {
    public UnimodalSecantTests(ITestOutputHelper output) : base(output)
    {
    }
    
    [Fact]
    public void Unimodal_Square_Optimum()
    {
      var f = new SquareTestFunction();
      var a = f.ValueWithDerivative(-1);
      var b = f.ValueWithDerivative(2);
      
      var method = new UnimodalSecant(f: f, a: a, b: b, Logger);
      EqualExtensions.AssertEqualTo<double>(2, (double)method.X);
      EqualExtensions.AssertEqualTo<double>(4, (double)method.Y);
      
      ConditionExtensions.AssertIsTrue(method.MoveNext());
      EqualExtensions.AssertEqualTo(1, method.X);
      EqualExtensions.AssertEqualTo(3, method.Y);
      
      ConditionExtensions.AssertIsTrue(method.IsOptimal);
      ConditionExtensions.AssertIsFalse(method.MoveNext());
    }
    
    /*
     * Unimodal square reversed
     */
    
    [Fact]
    public void Unimodal_SquareReversed()
    {
      var f = new SquareReversedTestFunction();
      var a = f.ValueWithDerivative(-1);
      var b = f.ValueWithDerivative(2);
      
      var method = new UnimodalSecant(f: f, a: a, b: b, Logger);
      EqualExtensions.AssertEqualTo<double>(-1, (double)method.X);
      EqualExtensions.AssertEqualTo<double>(-1, (double)method.Y);

      ConditionExtensions.AssertIsFalse(method.IsOptimal);
      ConditionExtensions.AssertIsFalse(method.IsPolimodal);
      
      ConditionExtensions.AssertIsFalse(method.MoveNext());

      ConditionExtensions.AssertIsFalse(method.IsOptimal);
      ConditionExtensions.AssertIsTrue(method.IsPolimodal);
    }
    
    /*
     * Unimodal sin
     */

    [Fact]
    public void Unimodal_Sin_Optimum()
    {
      var f = new SinTestFunction();
      var a = f.ValueWithDerivative(-1.3 * Math.PI);
      var b = f.ValueWithDerivative(0.4 * Math.PI);
      
      var method = new UnimodalSecant(f: f, a: a, b: b, Logger);
      EqualExtensions.AssertEqualTo<double>((double)a.X, (double)method.X);
      EqualExtensions.AssertEqualTo<double>((double)a.Y, (double)method.Y);
      
      Number width = method.Width;
      Number value = method.Y;

      uint i = OptimizationHelper.FindOptimal(method);
      
      EqualExtensions.AssertEqualTo(2, method.Y);
      EqualExtensions.AssertEqualTo(6, i);
    }
    
    [Fact]
    public void Unimodal_Sin_CornerDecreasing()
    {
      var f = new SinTestFunction();
      var a = f.ValueWithDerivative(0.3 * Math.PI);
      var b = f.ValueWithDerivative(Math.PI);
      
      var method = new UnimodalSecant(f: f, a: a, b: b, Logger);
      EqualExtensions.AssertEqualTo<double>((double)b.X, (double)method.X);
      EqualExtensions.AssertEqualTo<double>((double)b.Y, (double)method.Y);
      IsLowerThanExtensions.AssertIsLowerThan(b.Y, a.Y);

      ConditionExtensions.AssertIsFalse(method.IsOptimal);
      ConditionExtensions.AssertIsFalse(method.IsPolimodal);
      
      ConditionExtensions.AssertIsFalse(method.MoveNext());

      ConditionExtensions.AssertIsFalse(method.IsOptimal);
      ConditionExtensions.AssertIsTrue(method.IsPolimodal);
    }
    
    [Fact]
    public void Unimodal_Sin_CornerIncreasing()
    {
      var f = new SinTestFunction();
      var a = f.ValueWithDerivative(0);
      var b = f.ValueWithDerivative(0.7 * Math.PI);
      
      var method = new UnimodalSecant(f: f, a: a, b: b, Logger);
      EqualExtensions.AssertEqualTo<double>((double)a.X, (double)method.X);
      EqualExtensions.AssertEqualTo<double>((double)a.Y, (double)method.Y);
      IsLowerThanExtensions.AssertIsLowerThan(a.Y, b.Y);

      ConditionExtensions.AssertIsFalse(method.IsOptimal);
      ConditionExtensions.AssertIsFalse(method.IsPolimodal);
      
      ConditionExtensions.AssertIsFalse(method.MoveNext());

      ConditionExtensions.AssertIsFalse(method.IsOptimal);
      ConditionExtensions.AssertIsTrue(method.IsPolimodal);
    }
    
    /*
     * Polimodal sin
     */
    
    [Fact]
    public void Polimodal_Sin()
    {
      var f = new SinTestFunction();
      var a = f.ValueWithDerivative(1.4 * Math.PI);
      var b = f.ValueWithDerivative(4.4 * Math.PI);
      
      IsGreaterThanExtensions.AssertIsGreaterThan(b.Y, a.Y);
      IsGreaterThanExtensions.AssertIsGreaterThan(0, a.First);
      IsLowerThanExtensions.AssertIsLowerThan(0, b.First);
      
      var method = new UnimodalSecant(f: f, a: a, b: b, Logger);
      EqualExtensions.AssertEqualTo<double>((double)a.X, (double)method.X);
      EqualExtensions.AssertEqualTo<double>((double)a.Y, (double)method.Y);

      ConditionExtensions.AssertIsFalse(method.IsOptimal);
      ConditionExtensions.AssertIsFalse(method.IsPolimodal);
      
      ConditionExtensions.AssertIsFalse(method.MoveNext());

      ConditionExtensions.AssertIsFalse(method.IsOptimal);
      ConditionExtensions.AssertIsTrue(method.IsPolimodal);
    }
  }
}