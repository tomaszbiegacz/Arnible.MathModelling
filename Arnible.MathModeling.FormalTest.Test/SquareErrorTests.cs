﻿using Arnible.MathModeling.Geometry;
using Arnible.MathModeling.Test;
using Xunit;
using static Arnible.MathModeling.Term;

namespace Arnible.MathModeling.FormalTest.Test
{
  public class SquareErrorTests
  {
    private readonly SquareError _error = new SquareError();

    [Fact]
    public void Value()
    {
      AssertFormal.Equal((x - y).ToPower(2), _error.Value(x, y));
    }

    [Fact]
    public void DerivativeByX()
    {
      var p = new RectangularCoordianate(x, y);
      AssertFormal.VerifyDerivatives(_error.Value(x, y), x, _error.DerivativeByX(p));
    }

    [Fact]
    public void DerivativeByY()
    {
      var p = new RectangularCoordianate(x, y);
      AssertFormal.VerifyDerivatives(_error.Value(x, y), y, _error.DerivativeByY(p));
    }

    [Fact]
    public void DerivativeByR()
    {
      var cartesianPoint = new RectangularCoordianate(x, y);
      var polarPoint = new PolarCoordinate(r, φ);
      var valueInPolar =  _error.Value(x, y).ToPolar(cartesianPoint, polarPoint);
      AssertFormal.VerifyDerivatives(valueInPolar, r, _error.DerivativeByR(polarPoint));
    }

    [Fact]
    public void DerivativeByΦ()
    {
      var cartesianPoint = new RectangularCoordianate(x, y);
      var polarPoint = new PolarCoordinate(r, φ);
      var valueInPolar = _error.Value(x, y).ToPolar(cartesianPoint, polarPoint);
      AssertFormal.VerifyDerivatives(valueInPolar, φ, _error.DerivativeByΦ(polarPoint));
    }
  }
}