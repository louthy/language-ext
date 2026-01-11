using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LE = LanguageExt;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct an empty Seq
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>() =>
        Empty;
       
    /// <summary>
    /// Construct a singleton sequence from any value
    ///
    ///     var list = Seq(1);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A value)
    {
        var arr = new A[4];
        arr[2] = value;
        return new Seq<A>(new SeqStrict<A>(arr, 2, 1, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq(1, 2);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A a, A b)
    {
        var arr = new A[4];
        arr[2] = a;
        arr[3] = b;
        return new Seq<A>(new SeqStrict<A>(arr, 2, 2, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq(1, 2, 3);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A a, A b, A c)
    {
        var arr = new A[8];
        arr[2] = a;
        arr[3] = b;
        arr[4] = c;
        return new Seq<A>(new SeqStrict<A>(arr, 2, 3, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq(1, 2, 3, 4);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A a, A b, A c, A d)
    {
        var arr = new A[8];
        arr[2] = a;
        arr[3] = b;
        arr[4] = c;
        arr[5] = d;
        return new Seq<A>(new SeqStrict<A>(arr, 2, 4, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq(1, 2, 3, 4, 5);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A a, A b, A c, A d, A e)
    {
        var arr = new A[8];
        arr[2] = a;
        arr[3] = b;
        arr[4] = c;
        arr[5] = d;
        arr[6] = e;
        return new Seq<A>(new SeqStrict<A>(arr, 2, 5, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq(1, 2, 3, 4, 5, 6);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A a, A b, A c, A d, A e, A f)
    {
        var arr = new A[16];
        arr[4] = a;
        arr[5] = b;
        arr[6] = c;
        arr[7] = d;
        arr[8] = e;
        arr[9] = f;
        return new Seq<A>(new SeqStrict<A>(arr, 4, 6, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq(1, 2, 3, 4, 5, 6, 7);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A a, A b, A c, A d, A e, A f, A g)
    {
        var arr = new A[16];
        arr[4] = a;
        arr[5] = b;
        arr[6] = c;
        arr[7] = d;
        arr[8] = e;
        arr[9] = f;
        arr[10] = g;
        return new Seq<A>(new SeqStrict<A>(arr, 4, 7, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq(1, 2, 3, 4, 5, 6, 7, 8);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A a, A b, A c, A d, A e, A f, A g, A h)
    {
        var arr = new A[16];
        arr[4]  = a;
        arr[5]  = b;
        arr[6]  = c;
        arr[7]  = d;
        arr[8]  = e;
        arr[9]  = f;
        arr[10] = g;
        arr[11] = h;
        return new Seq<A>(new SeqStrict<A>(arr, 4, 8, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq(1, 2, 3, 4);
    /// 
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(A a, A b, A c, A d, A e, A f, A g, A h, params A[] tail)
    {
        var arr = new A[16 + tail.Length];
        arr[4]  = a;
        arr[5]  = b;
        arr[6]  = c;
        arr[7]  = d;
        arr[8]  = e;
        arr[9]  = f;
        arr[10] = g;
        arr[11] = h;

        System.Array.Copy(tail, 0, arr, 12, tail.Length);
        return new Seq<A>(new SeqStrict<A>(arr, 4, 8 + tail.Length, 0, 0));
    }
        
    /// <summary>
    /// Construct a sequence from an Enumerable
    /// Deals with `value == null` by returning `[]` and also memoizes the
    /// items in the enumerable as they're being consumed.
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(ReadOnlySpan<A> value) =>
        new (value);
        
    /// <summary>
    /// Construct a sequence from an Enumerable
    /// Deals with `value == null` by returning `[]` and also memoizes the
    /// items in the enumerable as they're being consumed.
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>(IEnumerable<A>? value) =>
        value switch
        {
            null                => Empty,
            Seq<A> seq          => seq,
            Arr<A> arr          => toSeq(arr),
            A[] array           => toSeq(array),
            IList<A> list       => toSeq(list),
            ICollection<A> coll => toSeq(coll),
            _                   => new Seq<A>(value)
        };

    /// <summary>
    /// Construct a sequence from an Enumerable
    /// Deals with `value == null` by returning `[]` and also memoizes the
    /// items in the enumerable as they're being consumed.
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(Iterator<A>? value) =>
        value switch
        {
            null                  => Empty,
            Iterator<A>.IterSeq s => s.Items,
            _                     => new Seq<A>(value)
        };

    /// <summary>
    /// Construct a sequence from an Enumerable
    /// Deals with `value == null` by returning `[]` and also memoizes the
    /// items in the enumerable as they're being consumed.
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(IEnumerable<A>? value) =>
        value switch
        {
            null                => Empty,
            Seq<A> seq          => seq,
            Arr<A> arr          => toSeq(arr.ToArray()),
            A[] array           => toSeq(array),
            IList<A> list       => toSeq(list),
            ICollection<A> coll => toSeq(coll),
            _                   => new Seq<A>(value)
        };
        
    /// <summary>
    /// Construct a sequence from an array
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(A[]? value)
    {
        if (value is null || value.Length == 0)
        {
            return Empty;
        }
        else
        {
            var length = value.Length;
            var data   = new A[length];
            System.Array.Copy(value, data, length);
            return LE.Seq.FromArray(data);
        }
    }

    /// <summary>
    /// Construct a sequence from a list
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(IList<A>? value) =>
        toSeq(value?.ToArray());

    /// <summary>
    /// Construct a sequence from a list
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(ICollection<A>? value) =>
        toSeq(value?.ToArray());

    /// <summary>
    /// Construct a sequence from a tuple
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(ValueTuple<A> tup) =>
        [tup.Item1];

    /// <summary>
    /// Construct a sequence from a tuple
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(ValueTuple<A, A> tup) =>
        [tup.Item1, tup.Item2];

    /// <summary>
    /// Construct a sequence from a tuple
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(ValueTuple<A, A, A> tup) =>
        [tup.Item1, tup.Item2, tup.Item3];

    /// <summary>
    /// Construct a sequence from a tuple
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(ValueTuple<A, A, A, A> tup) =>
        [tup.Item1, tup.Item2, tup.Item3, tup.Item4];

    /// <summary>
    /// Construct a sequence from a tuple
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(ValueTuple<A, A, A, A, A> tup) =>
        [tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5];

    /// <summary>
    /// Construct a sequence from a tuple
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(ValueTuple<A, A, A, A, A, A> tup) =>
        [tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6];

    /// <summary>
    /// Construct a sequence from a tuple
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(ValueTuple<A, A, A, A, A, A, A> tup) =>
        [tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6, tup.Item7];
       
}
