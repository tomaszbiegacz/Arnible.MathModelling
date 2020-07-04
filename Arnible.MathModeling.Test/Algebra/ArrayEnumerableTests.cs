﻿using static Arnible.MathModeling.xunit.AssertNumber;

namespace Arnible.MathModeling.Algebra.Test
{
  public abstract class ArrayEnumerableTests<TEnumerator, TValue>
    where TEnumerator : class, IArrayEnumerable<TValue>
    where TValue : struct
  {
    protected static void Verify(TEnumerator list, params TValue[] signs)
    {
      AreEqual(signs.Length, list.Length);
      for (uint i = 0; i < signs.Length; ++i)
      {
        IsTrue(signs[i].Equals(list[i]));
      }
    }

    protected static void VerifyAndMove(TEnumerator list, params TValue[] signs)
    {
      Verify(list, signs);
      IsTrue(list.MoveNext());
    }

    protected static void VerifyAndFinish(TEnumerator list, params TValue[] signs)
    {
      Verify(list, signs);
      IsFalse(list.MoveNext());
      Verify(list, signs);
    }
  }
}
