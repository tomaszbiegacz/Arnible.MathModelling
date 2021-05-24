﻿using Arnible.Assertions;
using Arnible.Linq;
using Arnible.Linq.Algebra;
using Arnible.MathModeling.Algebra;
using Arnible.MathModeling.Test;
using Xunit;

namespace Arnible.MathModeling.Geometry.Test
{
  public class HypersphericalCoordinateOnAxisViewTests
  {
    [Fact]
    public void ConversationCircle()
    {
      CartesianCoordinate cc = new CartesianCoordinate(1, 2, 3, 4);

      HypersphericalCoordinateOnAxisView view = cc.ToSphericalView();
      view.Coordinates.AssertIsEqualTo(cc.Coordinates);
      view.DimensionsCount.AssertIsEqualTo(cc.DimensionsCount);

      HypersphericalCoordinate hc = view;
      hc.R.AssertIsEqualTo(view.R);
      hc.Angles.AssertIsEqualTo(view.Angles);
      hc.DimensionsCount.AssertIsEqualTo(view.DimensionsCount);

      HypersphericalCoordinateOnAxisView view2 = hc.ToCartesianView();
      view2.R.AssertIsEqualTo(hc.R);
      view2.Angles.AssertIsEqualTo(hc.Angles);
      view2.DimensionsCount.AssertIsEqualTo(hc.DimensionsCount);

      CartesianCoordinate cc2 = view2;
      cc2.Coordinates.AssertIsEqualTo(cc.Coordinates);
      cc2.DimensionsCount.AssertIsEqualTo(cc.DimensionsCount);
    }

    [Fact]
    public void GetRectangularView()
    {
      CartesianCoordinate cc = new CartesianCoordinate(1, 2, 3, 4);
      HypersphericalCoordinateOnAxisView view = cc.ToSphericalView();

      var rcView = view.GetRectangularView(1, 3);
      rcView.R.AssertIsEqualTo(view.R);
      rcView.X.AssertIsEqualTo(2);
      rcView.Y.AssertIsEqualTo(4);
    }

    [Fact]
    public void GetLineView()
    {
      CartesianCoordinate cc = new CartesianCoordinate(1, 2, 3, 4);
      HypersphericalCoordinateOnAxisView view = cc.ToSphericalView();

      var rcView = view.GetLineView(1);
      rcView.R.AssertIsEqualTo(view.R);
      rcView.X.AssertIsEqualTo(2d);
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(2u)]
    [InlineData(3u)]
    [InlineData(4u)]
    [InlineData(5u)]
    [InlineData(6u)]
    [InlineData(7u)]
    [InlineData(8u)]
    public void GetIdentityVector(ushort dimensionsCount)
    {
      NumberVector vector = HypersphericalCoordinateOnAxisView.GetIdentityVector(dimensionsCount);
      vector.Length.AssertIsEqualTo(dimensionsCount);

      vector.AllWithDefault(v => v > 0 && v <= 1).AssertIsTrue();
      NumberVector.Repeat(HypersphericalCoordinateOnAxisView.GetIdentityVectorRatio(dimensionsCount), dimensionsCount).GetInternalEnumerable().AssertSequenceEqualsTo(vector.GetInternalEnumerable().ToArray());      
      vector.Select(v => v * v).SumDefensive().AssertIsEqualTo(1d);
    }
  }
}
