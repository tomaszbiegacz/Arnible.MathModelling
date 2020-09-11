﻿using Arnible.MathModeling.Export;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Arnible.MathModeling
{
  /// <summary>
  /// Immutable value array.  
  /// </summary>
  /// <remarks> 
  /// Features:
  /// * Equals returns true only if both arrays have the same size, and elements in each arrays are the same.
  /// * GetHashCode is calculated from array length and each element's hash.
  /// Usage considerations:
  /// * Structure size is equal to IntPtr.Size, hence there is no need to return/receive structure instance by reference
  /// </remarks>  
  [Serializable]
  public readonly struct ValueArray<T> : 
    IEquatable<ValueArray<T>>, 
    IValueArray<T>, 
    IValueObject 
    where T : struct, IValueObject
  {
    private static IEnumerable<T> _empty = LinqEnumerable.Empty<T>().ToReadOnlyList();
    private readonly T[] _values;

    internal ValueArray(params T[] items)
    {
      _values = items;
    }

    public static implicit operator ValueArray<T>(in T[] v) => new ValueArray<T>(v);

    public static implicit operator ValueArray<T>(in T v) => new ValueArray<T>(new[] { v });

    public override string ToString()
    {
      return "[" + string.Join(" ", GetInternalEnumerable().Select(v => v.ToStringValue())) + "]";
    }
    public string ToStringValue() => ToString();

    public override int GetHashCode()
    {
      int hc = Length.GetHashCode();
      foreach (T v in GetInternalEnumerable())
      {
        hc = unchecked(hc * 314159 + v.GetHashCodeValue());
      }
      return hc;
    }
    public int GetHashCodeValue() => GetHashCode();
    
    //
    // Serializer
    //

    class Serializer : IRecordWriter<ValueArray<T>>
    {
      private readonly IRecordWriterReadOnlyCollection<T> _serializer;

      public Serializer(
        in IRecordFieldSerializer serializer, 
        in Func<IRecordFieldSerializer, IRecordWriter<T>> writerFactory)
      {
        _serializer = serializer.GetReadOnlyCollectionSerializer(string.Empty, in writerFactory);
      }
      
      public void Write(in ValueArray<T> record)
      {
        _serializer.Write(record._values);
      }

      public void WriteNull()
      {
        // intentionally empty
      }
    }
    
    public static IRecordWriter<ValueArray<T>> CreateSerializer(
      IRecordFieldSerializer serializer,
      Func<IRecordFieldSerializer, IRecordWriter<T>> writerFactory)
    {
      return new Serializer(in serializer, in writerFactory);
    }
    
    //
    // IEquatable
    //    

    public bool Equals(in ValueArray<T> other) => GetInternalEnumerable().SequenceEqual(other.GetInternalEnumerable());

    public bool Equals(ValueArray<T> other) => Equals(in other);

    public override bool Equals(object obj)
    {
      if (obj is ValueArray<T> v)
      {
        return Equals(in v);
      }
      else
      {
        return false;
      }
    }

    public static bool operator ==(in ValueArray<T> a, in ValueArray<T> b) => a.Equals(in b);
    public static bool operator !=(in ValueArray<T> a, in ValueArray<T> b) => !a.Equals(in b);

    //
    // IArray
    //
    
    internal IEnumerable<T> GetInternalEnumerable() => _values ?? _empty;

    public ref readonly T this[in uint index]
    {
      get
      {
        if (_values == null || index >= _values.Length)
        {
          throw new ArgumentException(nameof(index));
        }
        return ref _values[index];
      }
    }

    public uint Length => (uint)(_values?.Length ?? 0);

    public ref readonly T First => ref _values[0];
    public ref readonly T Last => ref _values[^1];

    public IEnumerator<T> GetEnumerator() => GetInternalEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetInternalEnumerable().GetEnumerator();

    //
    // Operations
    //

    public IEnumerable<uint> Indexes()
    {
      return LinqEnumerable.RangeUint(Length);
    }

    public IEnumerable<uint> IndexesWhere(Func<T, bool> predicate)
    {
      for (uint i = 0; i < Length; ++i)
      {
        if (predicate(_values[i]))
        {
          yield return i;
        }
      }
    }

    public ValueArray<T> SubsetFromIndexes(in IReadOnlyCollection<uint> indexes)
    {
      T[] result = new T[indexes.Count];

      uint i = 0;
      foreach (uint index in indexes)
      {
        if (index >= Length)
        {
          throw new ArgumentException(nameof(indexes));
        }
        
        result[i] = _values[index];
        i++;
      }

      return new ValueArray<T>(result);
    }
    
    //
    // IEnumerable implementation (to avoid boxing)
    //
    
    public IEnumerable<TOutput> AggregateCombinations<TOutput>(
      in uint groupSize,
      in Func<IEnumerable<T>, TOutput> aggregator)
    {
      return GetInternalEnumerable().AggregateCombinations(in groupSize, in aggregator);
    }
    
    public bool All(in Func<T, bool> predicate)
    {
      return GetInternalEnumerable().All(predicate);
    }

    public bool Any(in Func<T, bool> predicate)
    {
      return GetInternalEnumerable().Any(predicate);
    }

    public IEnumerable<T> ExcludeAt(in uint pos)
    {
      return GetInternalEnumerable().ExcludeAt(pos);
    }

    public IEnumerable<TResult> Select<TResult>(in Func<T, TResult> selector)
    {
      return GetInternalEnumerable().Select(selector);
    }

    public IEnumerable<TResult> Select<TResult>(in Func<uint, T, TResult> selector)
    {
      return GetInternalEnumerable().Select(selector);
    }

    public T Single()
    {
      return GetInternalEnumerable().Single();
    }
    
    public IEnumerable<T> Where(in Func<T, bool> predicate)
    {
      return GetInternalEnumerable().Where(predicate);
    }
  }
}
