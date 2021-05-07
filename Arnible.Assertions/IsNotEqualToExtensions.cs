using System;
using System.Runtime.CompilerServices;

namespace Arnible.Assertions
{
  public static class IsNotEqualToExtensions
  {
    public static T AssertIsNotEqualTo<T>(this T actual, in T expected) where T: IEquatable<T>
    {
      if(expected.Equals(actual))
      {
        throw new AssertException($"Not expected {expected}");
      }
      return actual;
    }
    
    public static T AssertIsNotEqualToEnum<T>(this T actual, T expected) where T: Enum
    {
      int actualValue = Unsafe.As<T, int>(ref actual);
      int expectedValue = Unsafe.As<T, int>(ref expected);
      if(actualValue == expectedValue)
      {
        throw new AssertException($"Not expected {expected}");
      }
      return actual;
    }
  }
}