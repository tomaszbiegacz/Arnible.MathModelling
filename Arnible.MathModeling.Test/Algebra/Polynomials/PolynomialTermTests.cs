﻿using System;
using Arnible.Assertions;
using Arnible.Linq;
using Arnible.MathModeling.Analysis;
using Xunit;

namespace Arnible.MathModeling.Algebra.Polynomials.Tests
{
  public class PolynomialTermTests
  {
    [Fact]
    public void Constructor_Default()
    {
      PolynomialTerm v = default;

      ConditionExtensions.AssertIsTrue(v == 0);
      ConditionExtensions.AssertIsTrue(v.IsConstant);
      IsEqualToExtensions.AssertIsEqualTo("0", v.ToString());

      IsEqualToExtensions.AssertIsEqualTo(0u, v.PowerSum);
      IsEqualToExtensions.AssertIsEqualTo(0, v.GreatestPowerIndeterminate.Variable);
      IsEqualToExtensions.AssertIsEqualTo(0u, v.GreatestPowerIndeterminate.Power);

      IsEqualToExtensions.AssertIsEqualTo(0, v);
      IsEqualToExtensions.AssertIsEqualTo(0, (double)v);
      ConditionExtensions.AssertIsFalse(1 == v);

      IsEmptyExtensions.AssertIsEmpty(v.DerivativeBy('a'));

      IsEqualToExtensions.AssertIsEqualTo(0, 2 * v);
      IsEqualToExtensions.AssertIsEqualTo(0, v / 2);

      IsEqualToExtensions.AssertIsEqualTo<double>(0, v.GetOperation().Value());
    }

    [Fact]
    public void Constructor_Constant()
    {
      PolynomialTerm v = 2;

      ConditionExtensions.AssertIsFalse(v == 0);
      ConditionExtensions.AssertIsTrue(v.IsConstant);
      IsEqualToExtensions.AssertIsEqualTo("2", v.ToString());

      IsEqualToExtensions.AssertIsEqualTo(0u, v.PowerSum);
      IsEqualToExtensions.AssertIsEqualTo(0, v.GreatestPowerIndeterminate.Variable);
      IsEqualToExtensions.AssertIsEqualTo(0u, v.GreatestPowerIndeterminate.Power);

      IsEqualToExtensions.AssertIsEqualTo(2, v);
      IsEqualToExtensions.AssertIsEqualTo<double>(2, (double)v);
      ConditionExtensions.AssertIsFalse(1 == v);
      ConditionExtensions.AssertIsFalse(0 == v);

      IsEmptyExtensions.AssertIsEmpty(v.DerivativeBy('a'));

      IsEqualToExtensions.AssertIsEqualTo(4, 2 * v);
      IsEqualToExtensions.AssertIsEqualTo(1, v / 2);

      IsEqualToExtensions.AssertIsEqualTo<double>(2, v.GetOperation().Value());
    }

    [Fact]
    public void Constructor_Variable()
    {
      PolynomialTerm v = 'a';
      ConditionExtensions.AssertIsFalse(v == 0);
      ConditionExtensions.AssertIsFalse(v.IsConstant);

      IsEqualToExtensions.AssertIsEqualTo(1u, v.PowerSum);
      IsEqualToExtensions.AssertIsEqualTo('a', v.GreatestPowerIndeterminate.Variable);
      IsEqualToExtensions.AssertIsEqualTo(1u, v.GreatestPowerIndeterminate.Power);

      IsEqualToExtensions.AssertIsEqualTo("a", v.ToString());
      ConditionExtensions.AssertIsFalse('b' == v);

      IsEqualToExtensions.AssertIsEqualTo(1, v.DerivativeBy('a').Single());
      IsEmptyExtensions.AssertIsEmpty(v.DerivativeBy('b'));

      IsEqualToExtensions.AssertIsEqualTo(2 * Term.a, 2 * v);
      IsEqualToExtensions.AssertIsEqualTo(0.5 * Term.a, v / 2);

      IsEqualToExtensions.AssertIsEqualTo<double>(5, v.GetOperation('a').Value(5));
    }

    [Fact]
    public void Constructor_PolynomialVariable()
    {
      PolynomialTerm v = 2.1 * Term.a * Term.c.ToPower(3);

      ConditionExtensions.AssertIsFalse(v == 0);
      ConditionExtensions.AssertIsFalse(v.IsConstant);
      IsEqualToExtensions.AssertIsEqualTo("2.1ac³", v.ToString());

      IsEqualToExtensions.AssertIsEqualTo(4u, v.PowerSum);
      IsEqualToExtensions.AssertIsEqualTo('c', v.GreatestPowerIndeterminate.Variable);
      IsEqualToExtensions.AssertIsEqualTo(3u, v.GreatestPowerIndeterminate.Power);      

      IsEqualToExtensions.AssertIsEqualTo(2.1 * Term.c.ToPower(3), v.DerivativeBy('a').Single());
      IsEmptyExtensions.AssertIsEmpty(v.DerivativeBy('b'));
      IsEqualToExtensions.AssertIsEqualTo(6.3 * Term.c.ToPower(2) * Term.a.ToPower(1), v.DerivativeBy('c').Single());

      IsEqualToExtensions.AssertIsEqualTo(6.3 * Term.c.ToPower(2), v.DerivativeBy('a').DerivativeBy('c').Single());

      IsEqualToExtensions.AssertIsEqualTo(-4.2 * Term.a * Term.c.ToPower(3), -2 * v);

      IsEqualToExtensions.AssertIsEqualTo(2.1 * 5 * 8, v.GetOperation('a', 'c').Value(5, 2));
    }

    [Fact]
    public void Multiply_Inline()
    {
      PolynomialTerm x = 'x';
      IsEqualToExtensions.AssertIsEqualTo(4 * Term.x.ToPower(2), 4 * x * x);
      IsEqualToExtensions.AssertIsEqualTo("4x²", (4 * x * x).ToString());

      IsEqualToExtensions.AssertIsEqualTo(16, (x * x).GetOperation('x').Value(4));
    }

    [Fact]
    public void Multiply_Variables()
    {
      PolynomialTerm v1 = 2 * Term.a * Term.c.ToPower(3);
      PolynomialTerm v2 = 3 * Term.a.ToPower(2) * Term.d.ToPower(2);
      IsEqualToExtensions.AssertIsEqualTo(6 * Term.a.ToPower(3) * Term.d.ToPower(2) * Term.c.ToPower(3), v1 * v2);
    }

    [Fact]
    public void Simplify_To_Zero()
    {
      PolynomialTerm v1 = 2 * Term.a * Term.c.ToPower(3);
      PolynomialTerm v2 = -2 * Term.a * Term.c.ToPower(3);
      PolynomialTerm.Simplify(new[] { v1, v2 }).AsList().AssertIsEmpty();
    }

    [Fact]
    public void Simplify_To_Sum()
    {
      PolynomialTerm a = 'a';
      PolynomialTerm b = 'b';
      PolynomialTerm c = 'c';

      var expected = new PolynomialTerm[] { a * b * c, a * a, b * b, a * b, a, 2 * b, 3 };
      var before   = new PolynomialTerm[] { 1, b, 2, b, a, a * b, a * b * c, a * a, b * b };
      IsEqualToExtensions.AssertIsEqualTo(expected, PolynomialTerm.Simplify(before));
    }

    [Fact]
    public void Power_ByZero()
    {
      PolynomialTerm x = 'x';
      IsEqualToExtensions.AssertIsEqualTo(1, x.ToPower(0));
    }

    [Fact]
    public void Power_ByOne()
    {
      PolynomialTerm x = 'x';
      IsEqualToExtensions.AssertIsEqualTo(x, x.ToPower(1));
    }

    [Fact]
    public void Power_ByTwo()
    {
      PolynomialTerm x = 'x';
      IsEqualToExtensions.AssertIsEqualTo(x * x, x.ToPower(2));
    }

    [Fact]
    public void TryDivide_ByZero()
    {
      PolynomialTerm x = 'x';
      ConditionExtensions.AssertIsFalse(x.TryDivide(0, out _));
    }

    [Fact]
    public void TryDivide_ByConstant()
    {
      PolynomialTerm x = 'x';
      ConditionExtensions.AssertIsTrue((2 * x).TryDivide(2, out PolynomialTerm r));
      IsEqualToExtensions.AssertIsEqualTo(x, r);
    }

    [Fact]
    public void TryDivide_ByTerm_BySupersetVariables_xy_y()
    {
      PolynomialTerm x = 'x';
      PolynomialTerm y = 'y';
      ConditionExtensions.AssertIsFalse(x.TryDivide(x * y, out _));
    }

    [Fact]
    public void TryDivide_ByTerm_BySupersetVariables_9x_x()
    {
      PolynomialTerm x = 'x';
      ConditionExtensions.AssertIsTrue((9 * x).TryDivide(x, out PolynomialTerm r));
      IsEqualToExtensions.AssertIsEqualTo(9, r);
    }

    [Fact]
    public void TryDivide_ByTerm_BySupersetPowers()
    {
      PolynomialTerm x = 'x';
      ConditionExtensions.AssertIsFalse(x.TryDivide(x * x, out _));
    }

    [Fact]
    public void TryDivide_BySubsetTerm()
    {
      PolynomialTerm x = 'x';
      PolynomialTerm y = 'y';
      ConditionExtensions.AssertIsTrue((2 * x.ToPower(3) * y.ToPower(2)).TryDivide(0.5 * x, out PolynomialTerm r));
      IsEqualToExtensions.AssertIsEqualTo(4 * x.ToPower(2) * y.ToPower(2), r);
    }

    [Fact]
    public void Composition_SinWithZero()
    {
      var sinExpression = MetaMath.Sin(Term.α) + 1;
      IsEqualToExtensions.AssertIsEqualTo(1, sinExpression.Composition(Term.α, 0));
    }

    [Fact]
    public void Composition_CosWithZero()
    {
      var sinExpression = MetaMath.Cos(Term.α) + 1;
      IsEqualToExtensions.AssertIsEqualTo(2, sinExpression.Composition(Term.α, 0));
    }

    [Fact]
    public void Composition_SinCosWithZero()
    {
      var sinExpression = MetaMath.Sin(Term.α)* MetaMath.Cos(Term.α) + 1;
      IsEqualToExtensions.AssertIsEqualTo(1, sinExpression.Composition(Term.α, 0));
    }

    [Fact]
    public void Sin_Variable()
    {
      IsEqualToExtensions.AssertIsEqualTo(IndeterminateExpression.Sin('a'), PolynomialTerm.Sin(Term.a));
    }

    [Fact]
    public void Sin_Constant()
    {
      PolynomialTerm term = Math.PI / 2;
      IsEqualToExtensions.AssertIsEqualTo(1, PolynomialTerm.Sin(term));
    }

    [Fact]
    public void Cos_Variable()
    {
      IsEqualToExtensions.AssertIsEqualTo(IndeterminateExpression.Cos('a'), PolynomialTerm.Cos(Term.a));
    }

    [Fact]
    public void Cos_Constant()
    {
      PolynomialTerm term = 0;
      IsEqualToExtensions.AssertIsEqualTo(1, PolynomialTerm.Cos(term));
    }
  }
}
