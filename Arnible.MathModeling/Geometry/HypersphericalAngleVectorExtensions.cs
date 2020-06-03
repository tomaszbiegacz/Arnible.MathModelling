﻿using Arnible.MathModeling.Algebra;
using System.Collections.Generic;

namespace Arnible.MathModeling.Geometry
{
  public static class HypersphericalAngleVectorExtensions
  {
    public static HypersphericalAngleVector ToAngleVector(this IEnumerable<Number> numbers)
    {
      return HypersphericalAngleVector.Create(numbers);
    }

    public static HypersphericalAngleVector ToAngleVector(this IEnumerable<double> numbers)
    {
      return HypersphericalAngleVector.Create(numbers.Select(n => (Number)n));
    }

    public static HypersphericalAngleVector Sum(this IEnumerable<HypersphericalAngleVector> x)
    {
      HypersphericalAngleVector? current = null;
      foreach (var v in x)
      {
        if (current.HasValue)
        {
          current = current.Value + v;
        }
        else
        {
          current = v;
        }
      }
      return current.Value;
    }

    public static HypersphericalAngleVector Average(this IEnumerable<HypersphericalAngleVector> angles)
    {
      return angles.Select(v => (NumberVector)v).Average().ToAngleVector();
    }
  }
}
