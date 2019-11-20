﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Arnible.MathModeling
{
  public struct PolynomialDivision : IEquatable<PolynomialDivision>, IEquatable<Polynomial>, IPolynomialOperation
  {
    public Polynomial Numerator { get; }
    public Polynomial Denominator { get; }

    internal PolynomialDivision(Polynomial numerator, Polynomial denominator)
    {
      if (denominator.IsZero)
      {
        throw new DivideByZeroException();
      }

      if(numerator.TryDivide(denominator, out Polynomial reducedNumerator))
      {
        // let's simplify it if we can
        Numerator = reducedNumerator;
        Denominator = 1;
      }
      else
      {
        Numerator = numerator;
        Denominator = denominator;
      }      
    }

    public bool Equals(PolynomialDivision other) => IsZero ? other.IsZero : other.Numerator == Numerator && other.Denominator == Denominator;

    public bool Equals(Polynomial other) => IsPolynomial ? other.Equals((Polynomial)this) : false;

    public override int GetHashCode() => IsZero ? 0 : Numerator.GetHashCode() * Denominator.GetHashCode();

    public override bool Equals(object obj)
    {
      if (obj is PolynomialDivision pd)
      {
        return Equals(pd);
      }
      else if (obj is Polynomial p)
      {
        return Equals(p);
      }
      else
      {
        return false;
      }
    }

    public override string ToString()
    {
      if (IsZero)
        return "0";
      if (IsNaN)
        return "NaN";

      string numerator = Numerator.HasOneTerm ? Numerator.ToString() : $"({Numerator})";
      string denominator = Denominator.HasOneTerm ? Denominator.ToString() : $"({Denominator})";
      return $"{numerator}/{denominator}";
    }

    public static bool operator ==(PolynomialDivision a, PolynomialDivision b) => a.Equals(b);
    public static bool operator !=(PolynomialDivision a, PolynomialDivision b) => !a.Equals(b);

    public static bool operator ==(PolynomialDivision a, Polynomial b) => a.Equals(b);
    public static bool operator !=(PolynomialDivision a, Polynomial b) => !a.Equals(b);

    public static bool operator ==(Polynomial a, PolynomialDivision b) => a.Equals(b);
    public static bool operator !=(Polynomial a, PolynomialDivision b) => !a.Equals(b);

    /*
     * Properties
     */

    public bool IsNaN => Numerator.IsZero && Denominator.IsZero;

    public bool IsZero => Numerator.IsZero && !Denominator.IsZero;

    public bool IsPolynomial => Denominator == 1;

    /*
     * Operators
     */

    public static explicit operator Polynomial(PolynomialDivision v) => v.Numerator / (double)v.Denominator;    

    public static PolynomialDivision operator *(PolynomialDivision a, double numerator)
    {
      return new PolynomialDivision(numerator * a.Numerator, a.Denominator);
    }

    public static PolynomialDivision operator *(double numerator, PolynomialDivision a) => a * numerator;

    public PolynomialDivision DerivativeBy(char name)
    {
      Polynomial denominatorDerivative = Denominator.DerivativeBy(name);
      Polynomial numeratorDerivative = Numerator.DerivativeBy(name);
      if (denominatorDerivative.IsZero)
      {
        // this is necessary, since library doesn't support yet polynomials reduction/division
        return new PolynomialDivision(numeratorDerivative, Denominator);
      }
      else
      {
        Polynomial numerator = numeratorDerivative * Denominator - Numerator * denominatorDerivative;
        Polynomial denominator = Denominator * Denominator;
        return new PolynomialDivision(numerator, denominator);
      }
    }

    public PolynomialDivision DerivativeBy(PolynomialTerm name) => DerivativeBy((char)name);

    public PolynomialDivision Derivative2By(char name)
    {
      Polynomial denominatorDerivative = Denominator.DerivativeBy(name);
      Polynomial numeratorDerivative = Numerator.DerivativeBy(name);
      if (denominatorDerivative.IsZero)
      {
        // this is necessary, since library doesn't support yet polynomials reduction/division
        return new PolynomialDivision(numeratorDerivative, Denominator).DerivativeBy(name);
      }
      else
      {
        Polynomial numerator1 = numeratorDerivative * Denominator - Numerator * denominatorDerivative;
        Polynomial denominator1 = Denominator * Denominator;

        Polynomial numerator2 = numerator1.DerivativeBy(name) * Denominator - 2 * numerator1 * denominatorDerivative;
        Polynomial denominator2 = denominator1 * Denominator;
        return new PolynomialDivision(numerator2, denominator2);
      }
    }

    public PolynomialDivision Derivative2By(PolynomialTerm name) => Derivative2By((char)name);

    public PolynomialDivision Composition(char variable, Polynomial replacement) => new PolynomialDivision(
        numerator: Numerator.Composition(variable, replacement),
        denominator: Denominator.Composition(variable, replacement));

    public PolynomialDivision Composition(PolynomialTerm variable, Polynomial replacement) => Composition((char)variable, replacement);

    /*
     * IPolynomialOperation
     */

    public IEnumerable<char> Variables => Numerator.Variables.Union(Denominator.Variables);

    public double Value(IReadOnlyDictionary<char, double> x)
    {
      if(IsNaN)
      {
        throw new InvalidOperationException("Cannot calculate value from NaN");
      }

      if (IsZero)
      {
        return 0;
      }
      else
      {
        return Numerator.Value(x) / Denominator.Value(x);
      }
    }    
  }
}
