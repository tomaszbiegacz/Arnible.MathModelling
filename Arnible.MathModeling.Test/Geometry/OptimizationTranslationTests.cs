﻿using Arnible.MathModeling.Algebra;
using System;
using Xunit;
using static Arnible.MathModeling.xunit.AssertNumber;

namespace Arnible.MathModeling.Geometry.Test
{
  public class OptimizationTranslationTests
  {
    [Fact]
    public void ForMinimumEquals0_Absolute()
    {
      AreEqual(3d, OptimizationTranslation.ForMinimumEquals0(value: 6, new Derivative1Value(-2)));
    }

    [Fact]
    public void CartesianForMinimumEquals0_DirectedPosive_Hyperspherical()
    {
      AreEqual(new NumberTranslationVector(0, 3), OptimizationTranslation.CartesianForMinimumEquals0(value: 6, new HypersphericalAngleVector(Angle.RightAngle), new Derivative1Value(-2)));
    }

    [Fact]
    public void CartesianForMinimumEquals0_DirectedNegative_Hyperspherical()
    {
      double delta = -3 / Math.Sqrt(2);
      AreEqual(new NumberTranslationVector(delta, delta), OptimizationTranslation.CartesianForMinimumEquals0(value: 6, new HypersphericalAngleVector(Angle.RightAngle / 2), new Derivative1Value(2)));
    }

    [Fact]
    public void CartesianForMinimumEquals0_DirectedPosive()
    {
      AreEqual(new NumberTranslationVector(0, 3), OptimizationTranslation.CartesianForMinimumEquals0(value: 6, cartesiaxAxisNumber: 1, new Derivative1Value(-2)));
    }

    [Fact]
    public void HypersphericalForMinimumEquals0_DirectedPosive()
    {
      AreEqual(new HypersphericalAngleTranslationVector(0, 0.25), OptimizationTranslation.HypersphericalForMinimumEquals0(value: 0.5, anglePos: 1, new Derivative1Value(-2)));
    }

    [Fact]
    public void CartesianForMinimumEquals0_DirectedNegative()
    {      
      AreEqual(new NumberTranslationVector(-3), OptimizationTranslation.CartesianForMinimumEquals0(value: 6, cartesiaxAxisNumber: 0, new Derivative1Value(2)));
    }

    [Fact]
    public void CartesianForMinimumEquals0_AngleTranslation()
    {
      HypersphericalCoordinate hc = new HypersphericalCoordinate(2, new HypersphericalAngleVector(Angle.HalfRightAngle, Angle.HalfRightAngle));
      HypersphericalCoordinateOnAxisView view = hc.ToCartesianView();

      Number value = Angle.FullCycle;
      Derivative1Value dv = new Derivative1Value(2);

      HypersphericalCoordinate hc2 = new HypersphericalCoordinate(2, new HypersphericalAngleVector(Angle.HalfRightAngle, -1 * Angle.HalfRightAngle));
      HypersphericalCoordinateOnAxisView view2 = hc2.ToCartesianView();

      NumberTranslationVector expected = new NumberTranslationVector(view2.Coordinates - view.Coordinates);

      AreEqual(expected, OptimizationTranslation.CartesianForMinimumEquals0(value, view, 1, dv));
    }
  }
}