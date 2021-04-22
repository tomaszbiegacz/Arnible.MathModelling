﻿using Arnible.Assertions;
using Arnible.Linq;
using Arnible.MathModeling.Algebra.Polynomials;
using Xunit;
using static Arnible.MathModeling.Algebra.Polynomials.MetaMath;
using static Arnible.MathModeling.Algebra.Polynomials.Term;
using static Arnible.MathModeling.xunit.AssertNumber;

namespace Arnible.MathModeling.Geometry.Test
{
  public class HypersphericalCoordinateOnAxisViewTests
  {
    [Fact]
    public void DerivativeByR()
    {
      CartesianCoordinate cartesianPoint = new NumberVector(x, y, z);
      var sphericalPoint = new HypersphericalCoordinate(r, new HypersphericalAngleVector(θ, φ));
      HypersphericalCoordinateOnAxisView view = sphericalPoint.ToCartesianView();

      var derivatives = view.DerivativeByR().ToArray();
      AreEqual(cartesianPoint.DimensionsCount, derivatives.Length);
      for (ushort dimensionPos = 0; dimensionPos < cartesianPoint.DimensionsCount; ++dimensionPos)
      {
        var symbol = (PolynomialDivision)cartesianPoint.Coordinates[dimensionPos];
        AreEqual(symbol.ToSpherical(cartesianPoint, sphericalPoint), r * derivatives[dimensionPos].First);
      }
    }

    [Fact]
    public void GetCartesianAxisViewsRatiosDerivativesByAngle()
    {
      var sphericalPoint = new HypersphericalCoordinate(r, new HypersphericalAngleVector(α, β, γ, δ));
      HypersphericalCoordinateOnAxisView view = sphericalPoint.ToCartesianView();

      for (ushort anglePos = 0; anglePos < sphericalPoint.Angles.Length; ++anglePos)
      {
        PolynomialTerm angleTerm = (PolynomialTerm)sphericalPoint.Angles[anglePos];
        var derivatives = view.GetCartesianAxisViewsRatiosDerivativesByAngle(anglesCount: 4, anglePos: anglePos).ToArray();
        AreEqual(view.DimensionsCount, derivatives.Length);
        for (ushort coordinatePos = 0; coordinatePos < view.Coordinates.Length; ++coordinatePos)
        {
          PolynomialDivision coordinate = (PolynomialDivision)view.Coordinates[coordinatePos];
          PolynomialDivision expected = coordinate.DerivativeBy(angleTerm);
          derivatives[coordinatePos].First.AssertEqualTo(expected);
        }
      }
    }

    [Fact]
    public void GetRectangularViewDerivativeByAngle()
    {
      var sphericalPoint = new HypersphericalCoordinate(r, new HypersphericalAngleVector(α, β, γ));
      HypersphericalCoordinateOnAxisView view = sphericalPoint.ToCartesianView();

      HypersphericalCoordinateOnRectangularViewWithDerivative recView = view
        .GetAngleDerivativesView(anglesCount: 3, anglePos: 2)
        .GetRectangularViewDerivativeByAngle(axisA: 0, axisB: 2);
      AreEqual(r, recView.R);

      AreEqual(Cos(γ) * Cos(β) * Cos(α), recView.RatioX);
      AreEqual(r * Cos(γ) * Cos(β) * Cos(α), recView.X);
      AreEqual(-1 * r * Sin(γ) * Cos(β) * Cos(α), recView.DerivativeForX.First);

      AreEqual(Sin(β) * Cos(γ), recView.RatioY);
      AreEqual(r * Sin(β) * Cos(γ), recView.Y);
      AreEqual(-1 * r * Sin(γ) * Sin(β), recView.DerivativeForY.First);
    }

    [Fact]
    public void GetRectangularViewDerivativeByAngle_Zero()
    {
      var sphericalPoint = new HypersphericalCoordinate(r, new HypersphericalAngleVector(α, β, γ));
      HypersphericalCoordinateOnAxisView view = sphericalPoint.ToCartesianView();

      HypersphericalCoordinateOnRectangularViewWithDerivative recView = view
        .GetAngleDerivativesView(anglesCount: 8, anglePos: 2)
        .GetRectangularViewDerivativeByAngle(axisA: 0, axisB: 8);
      AreEqual(r, recView.R);

      AreEqual(Cos(γ) * Cos(β) * Cos(α), recView.RatioX);
      AreEqual(r * Cos(γ) * Cos(β) * Cos(α), recView.X);
      AreEqual(-1 * r * Sin(γ) * Cos(β) * Cos(α), recView.DerivativeForX.First);

      AreEqual(0, recView.RatioY);
      AreEqual(0, recView.Y);
      AreEqual(0, recView.DerivativeForY.First);
    }
  }
}
