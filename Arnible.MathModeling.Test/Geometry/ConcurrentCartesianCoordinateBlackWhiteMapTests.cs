using Arnible.Assertions;
using Xunit;

namespace Arnible.MathModeling.Geometry.Test
{
  public class ConcurrentCartesianCoordinateBlackWhiteMapTests
  {
    [Fact]
    public void Precision_One()
    {
      var map = new ConcurrentCartesianCoordinateBlackWhiteMap(
        leftBottomMapCorner: new Number[] {0, -1, -2},
        rightTopMapCorner: new Number[] {2, 3, 4},
        precision: 1);
      
      map.DimensionsCount.AssertIsEqualTo(3);
      map.MarkedPointsCount.AssertIsEqualTo(0);
      map.IsMarked(new Number[] {0, -1, -2}).AssertIsFalse();
      map.IsMarked(new Number[] { 0, 0, 0 }).AssertIsFalse();
      map.IsMarked(new Number[] { 1, 1, 1 }).AssertIsFalse();
      map.IsMarked(new Number[] { 2, 3, 4 }).AssertIsFalse();
      
      map.MarkPoint(new Number[] { 0, 0, 0 }).AssertIsTrue();
      
      map.MarkedPointsCount.AssertIsEqualTo(1);
      map.IsMarked(new Number[] {0, -1, -2}).AssertIsTrue();
      map.IsMarked(new Number[] { 0, 0, 0 }).AssertIsTrue();
      map.IsMarked(new Number[] { 1, 1, 1 }).AssertIsTrue();
      map.IsMarked(new Number[] { 2, 3, 4 }).AssertIsTrue();
    }
    
    [Fact]
    public void Precision_Two()
    {
      var map = new ConcurrentCartesianCoordinateBlackWhiteMap(
        leftBottomMapCorner: new Number[] {0, -1, -2},
        rightTopMapCorner: new Number[] {2, 3, 4},
        precision: 2);
      // middle: new Number[] {1, 1, 1}
      
      map.DimensionsCount.AssertIsEqualTo(3);
      map.MarkedPointsCount.AssertIsEqualTo(0);
      map.IsMarked(new Number[] {0, -1, -2}).AssertIsFalse();
      map.IsMarked(new Number[] { 0, 0, 0 }).AssertIsFalse();
      map.IsMarked(new Number[] { 1, 1, 1 }).AssertIsFalse();
      map.IsMarked(new Number[] { 2, 3, 4 }).AssertIsFalse();
      
      map.MarkPoint(new Number[] { 0, 0, 0 }).AssertIsTrue();
      
      map.MarkedPointsCount.AssertIsEqualTo(1);
      map.IsMarked(new Number[] {0, -1, -2}).AssertIsTrue();
      map.IsMarked(new Number[] { 0, 0, 0 }).AssertIsTrue();
      map.IsMarked(new Number[] { 1, 1, 1 }).AssertIsFalse();
      map.IsMarked(new Number[] { 2, 3, 4 }).AssertIsFalse();
    }
    
    [Fact]
    public void Precision_Three()
    {
      var map = new ConcurrentCartesianCoordinateBlackWhiteMap(
        leftBottomMapCorner: new Number[] {0, -1, -2},
        rightTopMapCorner: new Number[] {2, 3, 4},
        precision: 3);

      map.DimensionsCount.AssertIsEqualTo(3);
      map.MarkedPointsCount.AssertIsEqualTo(0);
      map.IsMarked(new Number[] {0, -1, -2}).AssertIsFalse();
      map.IsMarked(new Number[] { 0, 0, 0 }).AssertIsFalse();
      map.IsMarked(new Number[] { 1, 1, 1 }).AssertIsFalse();
      map.IsMarked(new Number[] { 2, 3, 4 }).AssertIsFalse();
      
      map.MarkPoint(new Number[] { 0, 0, 0 }).AssertIsTrue();
      
      map.MarkedPointsCount.AssertIsEqualTo(1);
      map.IsMarked(new Number[] {0, -1, -2}).AssertIsFalse();
      map.IsMarked(new Number[] { 0, 0, 0 }).AssertIsTrue();
      map.IsMarked(new Number[] { 0.1, 0.1, 0.1 }).AssertIsTrue();
      map.IsMarked(new Number[] { 1, 1, 1 }).AssertIsFalse();
      map.IsMarked(new Number[] { 2, 3, 4 }).AssertIsFalse();
      
      map.MarkPoint(new Number[] { 0.1, 0.1, 0.1 }).AssertIsFalse();
    }
  }
}