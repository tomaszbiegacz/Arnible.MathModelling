﻿using Arnible.MathModeling.Algebra;
using System;
using System.Collections.Generic;

namespace Arnible.MathModeling.Geometry
{
  public readonly struct HypersphericalAngleQuantified : IEquatable<HypersphericalAngleQuantified>
  {
    internal class Factory
    {
      private uint _id = 0;
      private readonly byte _resolution;

      public Factory(byte resolution)
      {
        if (resolution == 0)
          throw new ArgumentException(nameof(resolution));

        _resolution = resolution;
      }

      public sbyte LowestAngle => (sbyte)(-1 * _resolution + 1);

      public sbyte RightAngle => (sbyte)_resolution;

      public IEnumerable<sbyte> AnglesWithoutRightAngle()
      {
        for (sbyte angle = LowestAngle; angle < RightAngle; ++angle)
        {
          yield return angle;
        }
      }

      public IEnumerable<sbyte> Axis(uint angleCount)
      {
        if (angleCount == 0)
        {
          throw new ArgumentException(nameof(angleCount));
        }
        for (uint i = 1; i < angleCount; ++i)
        {
          yield return 0;
        }
        yield return RightAngle;
      }

      public HypersphericalAngleQuantified Create(IEnumerable<sbyte> angles) => new HypersphericalAngleQuantified(angles, _resolution, ++_id);
    }

    /// <summary>
    /// Return possible directions in given resolution (2 for 45 degres resolution).
    /// </summary>    
    public static IEnumerable<HypersphericalAngleQuantified> GetQuantifiedDirections(uint anglesCount, uint resolution)
    {
      if (resolution == 0 || resolution > byte.MaxValue)
      {
        throw new ArgumentException(nameof(resolution));
      }
      var factory = new QuantifiedDirectionsFactory(new Factory((byte)resolution), anglesCount);
      return factory.Angles;
    }

    /// <summary>
    /// Return possible directions in given resolution (2 for 45 degres resolution), but not along one cartesian axis.
    /// </summary>    
    public static IEnumerable<HypersphericalAngleQuantified> GetQuantifiedDirectionsNotOrthogonal(uint anglesCount, uint resolution)
    {
      return GetQuantifiedDirections(anglesCount, resolution).Where(a => a.UsedCartesianDirectionsCount > 1);
    }

    private readonly byte _rightAngleResolution;
    private readonly sbyte[] _angles;

    private HypersphericalAngleQuantified(IEnumerable<sbyte> angles, byte rightAngleResolution, uint id)
    {
      Id = id;
      _rightAngleResolution = rightAngleResolution;
      _angles = angles.ToArray();
      if (_angles.Where(a => a == rightAngleResolution).Count() > 1)
      {
        throw new ArgumentException(nameof(angles));
      }
      UsedCartesianDirectionsCount = GetUsedCartesianDirectionsCount(_angles, rightAngleResolution);
    }

    private static byte GetUsedCartesianDirectionsCount(sbyte[] angles, byte rightAngleResolution)
    {
      byte result = 0;
      if (angles.Length > 0)
      {
        uint anglePos;
        uint? firstAnglePos = angles.IndexOf(a => a == rightAngleResolution);
        if (firstAnglePos > 0)
        {
          result = 1;
          anglePos = firstAnglePos.Value + 1;
        }
        else
        {
          sbyte firstAngle = angles[0];
          if (firstAngle == 0 || firstAngle == rightAngleResolution)
          {
            result = 1;
          }
          else
          {
            result = 2;
          }
          anglePos = 1;
        }
        result += (byte)angles.SkipExactly(anglePos).Where(a => a != 0).Count();
      }
      return result;
    }

    public uint Id { get; }

    public byte UsedCartesianDirectionsCount { get; }

    public IEnumerable<sbyte> Angles => _angles ?? LinqEnumerable.Empty<sbyte>();

    public bool Equals(HypersphericalAngleQuantified other) => Id == other.Id;

    public override bool Equals(object obj)
    {
      if (obj is HypersphericalAngleQuantified objCast)
        return Equals(objCast);
      else
        return false;
    }

    public static bool operator ==(HypersphericalAngleQuantified a, HypersphericalAngleQuantified b) => a.Equals(b);
    public static bool operator !=(HypersphericalAngleQuantified a, HypersphericalAngleQuantified b) => !a.Equals(b);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString()
    {
      return "[" + string.Join(" ", Angles) + "]";
    }

    public HypersphericalAngleVector ToVector()
    {
      if (Angles.Any())
      {
        Number step = Angle.RightAngle / _rightAngleResolution;
        return new HypersphericalAngleVector(_angles.Select(v => v * step).ToVector());
      }
      else
      {
        return default;
      }
    }
  }
}
