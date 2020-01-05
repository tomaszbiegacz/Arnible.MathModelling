﻿using Arnible.MathModeling.Geometry;
using Xunit;
using static Arnible.MathModeling.Term;

namespace Arnible.MathModeling.FormalTest.Test.Geometry
{
  public class CoordinatesExtensionTests
  {
    [Fact]
    public void ToSpherical_Generalizes_ToPolar()
    {      
      Number expression = (x - y).ToPower(2);

      var rc = new RectangularCoordianate(x, y);
      var pc = new PolarCoordinate(r, φ);
      var expected = expression.ToPolar(rc, pc);

      var cc = new CartesianCoordinate(x, y);
      var hc = new HypersphericalCoordinate(r, φ);
      var actual = expression.ToSpherical(cc, hc);

      AssertFormal.Equal(expected, actual);
    }
  }
}