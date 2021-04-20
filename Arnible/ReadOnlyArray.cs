﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Arnible
{
  /// <summary>
  /// Read only array wrapper.
  /// Lightweight (and limited in some ways) replacement of ReadOnlySpan
  /// </summary>
  /// <remarks> 
  /// Features:
  /// * Equals returns true only if both arrays have the same size, and elements in each arrays are the same.
  /// * GetHashCode is calculated from array element's hash.
  /// Usage considerations:
  /// * Structure size is equal to IntPtr.Size, hence there is no need to return/receive structure instance by reference
  /// * Due to compatibility with IReadOnlyList array size is limited to ushort 
  /// </remarks>
  [Serializable]
  public readonly struct ReadOnlyArray<T> : IEquatable<ReadOnlyArray<T>> where T: IEquatable<T>
  {
    private readonly static T[] _emptyArray = new T[0];
    
    private readonly T[]? _src;

    /// <summary>
    /// Allow implicit casting of arrays
    /// </summary>
    private ReadOnlyArray(T[] items)
    {
      _src = items;
    }
    public static implicit operator ReadOnlyArray<T>(T[] v) => new(v);

    //
    // Basic casts
    //
    
    public static implicit operator ReadOnlySpan<T>(ReadOnlyArray<T> v) => v._src ?? _emptyArray;
    
    private IReadOnlyList<T> Src => _src ?? _emptyArray;
    public IReadOnlyList<T>AsList() => Src;
    public IEnumerator<T> GetEnumerator() => Src.GetEnumerator();

    //
    // Properties
    //
    
    public ushort Length => (ushort)Src.Count;
    public bool IsEmpty => Src.Count == 0;
    
    
    public ref T this[ushort pos] => ref (_src ?? throw new InvalidOperationException())[pos];

    public ref T First => ref (_src ?? throw new InvalidOperationException())[0]; 
    
    public ref T Last => ref (_src ?? throw new InvalidOperationException())[^1];

    //
    // Equals
    //

    public bool Equals(ReadOnlyArray<T> other)
    {
      if(_src is null || other._src is null)
      {
        // both needs to be null
        return _src == other._src;
      }
      else if(_src.Length != other._src.Length)
      {
        return false;
      }
      else
      {
        for(ushort i=0; i<_src.Length; ++i)
        {
          if(!_src[i].Equals(other._src[i]))
          {
            return false;
          }
        }
        return true;
      }
    }

    public override bool Equals(object? obj)
    {
      return obj is ReadOnlyArray<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
      HashCode hashCode = new HashCode();
      foreach(T item in Src)
      {
        hashCode.Add(item);
      }
      return hashCode.ToHashCode();
    }
    
    public static bool operator ==(ReadOnlyArray<T> a, ReadOnlyArray<T> b) => a.Equals(b);
    public static bool operator !=(ReadOnlyArray<T> a, ReadOnlyArray<T> b) => !a.Equals(b);
    
    //
    // Serialization
    //
    
    public ReadOnlyMemory<char> ToCharMemory(char separator = ',')
    {
      return ("[" + string.Join(separator, Src) + "]").AsMemory();
    }
    
    public override string ToString() => ToCharMemory().ToString();
  }
}