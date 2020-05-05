﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Arnible.MathModeling
{
  public readonly struct Polynomial : IEquatable<PolynomialDivision>, IEquatable<Polynomial>, IPolynomialOperation, IEnumerable<PolynomialTerm>
  {
    const char InPlaceVariableReplacement = '$';

    private readonly IEnumerable<PolynomialTerm> _terms;

    internal Polynomial(params PolynomialTerm[] terms)      
    {
      _terms = PolynomialTerm.Simplify(terms);
    }

    private Polynomial(IEnumerable<PolynomialTerm> terms)
    {
      _terms = PolynomialTerm.Simplify(terms.ToArray());
    }

    private Polynomial(double v)
    {
      if(v == 0)
      {
        _terms = LinqEnumerable.Empty<PolynomialTerm>();
      }
      else
      {
        _terms = LinqEnumerable.Yield<PolynomialTerm>(v);
      }
    }

    private Polynomial(char v)
    {      
      _terms = LinqEnumerable.Yield<PolynomialTerm>(v);      
    }

    public static implicit operator Polynomial(PolynomialTerm v) => new Polynomial(v);
    public static implicit operator Polynomial(double value) => new Polynomial(value);
    public static implicit operator Polynomial(char name) => new Polynomial(name);

    private IEnumerable<PolynomialTerm> Terms => _terms ?? LinqEnumerable.Empty<PolynomialTerm>();

    public bool Equals(Polynomial other) => (other - this).IsZero;

    public bool Equals(PolynomialDivision other) => other.IsPolynomial ? Equals((Polynomial)other) : false;

    public override int GetHashCode()
    {
      int result = 0;
      foreach (var v in Terms)
      {
        result ^= v.GetHashCode();
      }
      return result;
    }

    public override bool Equals(object obj)
    {
      if (obj is PolynomialDivision pd)
      {
        return Equals(pd);
      }
      else if (obj is Polynomial v)
      {
        return Equals(v);
      }
      else
      {
        return false;
      }
    }

    public string ToString(CultureInfo cultureInfo)
    {
      if (IsZero)
      {
        return "0";
      }
      else
      {
        var result = new StringBuilder();
        bool printOperator = false;
        foreach (var variable in Terms)
        {
          if (printOperator && variable.HasPositiveCoefficient)
          {
            result.Append("+");
          }
          result.Append(variable.ToString(cultureInfo));

          printOperator = true;
        }
        return result.ToString();
      }
    }

    public override string ToString() => ToString(CultureInfo.InvariantCulture);


    public static bool operator ==(Polynomial a, Polynomial b) => a.Equals(b);
    public static bool operator !=(Polynomial a, Polynomial b) => !a.Equals(b);

    /*
     * Properties
     */

    public bool IsZero => !Terms.Any();

    public bool HasOneTerm => Terms.Count() < 2;

    public bool IsConstant
    {
      get
      {
        if (!HasOneTerm)
          return false;
        else
          return Terms.SingleOrDefault().IsConstant;
      }
    }

    /*
     * Operators
     */

    public static explicit operator PolynomialTerm(Polynomial v) => v.Terms.SingleOrDefault();

    public static explicit operator double(Polynomial v) => (double)v.Terms.SingleOrDefault();

    // +

    public static Polynomial operator +(Polynomial a, Polynomial b)
    {
      return new Polynomial(a.Terms.Concat(b.Terms));
    }

    public static Polynomial operator +(Polynomial a, double b)
    {
      return new Polynomial(a.Terms.Append(b));
    }

    public static Polynomial operator +(double a, Polynomial b) => b + a;

    // -

    public static Polynomial operator -(Polynomial a, Polynomial b)
    {
      return new Polynomial(a.Terms.Concat(b.Terms.Select(v => -1 * v)));
    }

    public static Polynomial operator -(Polynomial a, double b)
    {
      return new Polynomial(a.Terms.Append(-1 * b));
    }

    public static Polynomial operator -(double a, Polynomial b) => -1 * b + a;

    // *

    private static IEnumerable<PolynomialTerm> MultiplyVariables(Polynomial a, Polynomial b)
    {
      foreach (var v1 in a.Terms)
      {
        foreach (var v2 in b.Terms)
        {
          yield return v1 * v2;
        }
      }
    }

    public static Polynomial operator *(Polynomial a, Polynomial b)
    {
      return new Polynomial(MultiplyVariables(a, b));
    }

    public static Polynomial operator *(PolynomialTerm a, Polynomial b)
    {
      return new Polynomial(b.Terms.Select(t => a * t));
    }

    public static Polynomial operator *(Polynomial b, PolynomialTerm a) => a * b;

    public static Polynomial operator *(double a, Polynomial b)
    {
      return new Polynomial(b.Terms.Select(t => a * t));
    }

    public static Polynomial operator *(Polynomial b, double a) => a * b;

    // /

    public static PolynomialDivision operator /(Polynomial a, Polynomial b)
    {
      return new PolynomialDivision(a, b);
    }

    public static Polynomial operator /(Polynomial a, double denominator) => a * (1 / denominator);    

    public static PolynomialDivision operator /(double a, Polynomial denominator)
    {
      return new PolynomialDivision(a, denominator);
    }

    /*
     * Other operators
     */

    public static Polynomial operator %(Polynomial a, Polynomial b)
    {
      a.ReduceBy(b, out Polynomial reminder);
      return reminder;
    }

    private static Polynomial TryReduce(
      Polynomial a,
      PolynomialTerm denominator,
      Polynomial denominatorSuffix,
      List<PolynomialTerm> result)
    {
      if (a == 0)
      {
        // we are done, there is nothing more to extract from
        return 0;
      }

      foreach (PolynomialTerm aTerm in a.Terms)
      {
        if (aTerm.TryDivide(denominator, out PolynomialTerm remainder))
        {
          result.Add(remainder);

          // let's see what is left and continue reducing on it
          Polynomial remaining = a - aTerm - denominatorSuffix * remainder;
          return TryReduce(remaining, denominator, denominatorSuffix, result);
        }
      }

      // we weren't able to reduce it
      return a;
    }

    public Polynomial ToPower(uint power)
    {
      switch (power)
      {
        case 0:
          return 1;
        case 1:
          return this;
        default:
          {
            uint power2 = power / 2;
            var power2Item = this.ToPower(power2);
            var power2Item2 = power2Item * power2Item;
            if (power % 2 == 0)
              return power2Item2;
            else
              return power2Item2 * this;
          }
      }
    }

    public Polynomial ReduceBy(Polynomial b, out Polynomial reminder)
    {
      if (b.IsConstant)
      {
        reminder = default;
        return this / (double)b;
      }
      if (IsZero)
      {
        reminder = default;
        return 0;
      }

      PolynomialTerm denominator = b.Terms.First();
      Polynomial denominatorSuffix = new Polynomial(b.Terms.SkipExactly(1));
      var resultTerms = new List<PolynomialTerm>();
      reminder = TryReduce(this, denominator, denominatorSuffix, resultTerms);
      return new Polynomial(resultTerms);
    }

    public Polynomial ReduceBy(Polynomial b)
    {
      Polynomial result = ReduceBy(b, out Polynomial reminder);
      if (reminder != 0)
      {
        throw new InvalidOperationException($"Cannot reduce [{this}] with [{b}].");
      }
      return result;
    }

    /*
     * Derivative
     */

    public Polynomial DerivativeBy(char name)
    {
      return new Polynomial(Terms.SelectMany(v => v.DerivativeBy(name)));
    }

    public Polynomial DerivativeBy(PolynomialTerm name) => DerivativeBy((char)name);

    /*
     * Composition
     */

    private IEnumerable<PolynomialTerm> CompositionIngredients(char variable, Polynomial replacement)
    {
      List<PolynomialTerm> remaining = new List<PolynomialTerm>();
      foreach (PolynomialTerm term in Terms)
      {
        if (term.Variables.Any(v => v == variable))
        {
          foreach (var replacedTerm in term.Composition(variable, replacement))
          {
            yield return replacedTerm;
          }
        }
        else
        {
          yield return term;
        }
      }
    }

    public Polynomial Composition(char variable, Polynomial replacement)
    {
      if (replacement.Variables.Any(v => v == variable))
      {
        if (variable == InPlaceVariableReplacement)
        {
          throw new InvalidOperationException("Something went wrong with in-place variables replacement.");
        }

        // special case for in-place variables replacement
        Polynomial temporaryReplacement = replacement.Composition(variable, InPlaceVariableReplacement);
        return Composition(variable, temporaryReplacement).Composition(InPlaceVariableReplacement, variable);
      }

      return new Polynomial(CompositionIngredients(variable, replacement));
    }

    public PolynomialDivision Composition(char variable, PolynomialDivision replacement)
    {
      List<PolynomialTerm> remaining = new List<PolynomialTerm>();

      PolynomialDivision result = 0;
      foreach (PolynomialTerm term in Terms)
      {
        if (term.Variables.Any(v => v == variable))
        {
          result += term.Composition(variable, replacement);
        }
        else
        {
          remaining.Add(term);
        }
      }

      if (remaining.Count > 0)
      {
        result += new Polynomial(remaining);
      }

      return result;
    }

    public Polynomial Composition(PolynomialTerm variable, Polynomial replacement) => Composition((char)variable, replacement);

    public PolynomialDivision Composition(PolynomialTerm variable, PolynomialDivision replacement) => Composition((char)variable, replacement);

    /*
     * IPolynomialOperation
     */

    public IEnumerable<char> Variables => Terms.SelectMany(kv => kv.Variables);

    public double Value(IReadOnlyDictionary<char, double> x)
    {
      if (IsZero)
      {
        return 0;
      }
      else
      {
        return _terms.Select(t => t.Value(x)).SumDefensive();
      }
    }

    /*
     * IEnumerator<PolynomialTerm>
     */

    public IEnumerator<PolynomialTerm> GetEnumerator() => Terms.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Terms.GetEnumerator();
  }
}
