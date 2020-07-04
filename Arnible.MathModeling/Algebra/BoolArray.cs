﻿using System;
using System.Collections.Generic;

namespace Arnible.MathModeling.Algebra
{
  readonly struct BoolArray : IComparable<BoolArray>
  {
    private readonly uint _valueCount;

    public BoolArray(IEnumerable<bool> values)
    {
      Values = values.ToValueArray();
      _valueCount = Values.Where(s => s).Count();
    }

    /*
     * IComparable
     */

    public int CompareTo(BoolArray other)
    {
      int byCount = _valueCount.CompareTo(other._valueCount);
      if (byCount != 0)
      {
        return byCount;
      }      

      for (uint i = Values.Length - 1; i >= 0; --i)
      {
        int byValue = Values[i].CompareTo(other.Values[i]);
        if (byValue != 0)
        {
          return byValue;
        }
      }

      return 0;
    }

    /*
     * Properties
     */

    public ValueArray<bool> Values { get; }
  }
}
