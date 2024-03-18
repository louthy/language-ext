using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using LSeq = LanguageExt.Seq;
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Represents an empty sequence
    /// </summary>
    public static readonly SeqEmpty Empty = new();

    /// <summary>
    /// Construct a sequence from any value
    /// </summary>
    /// <typeparam name="A">Type of the items in the sequence</typeparam>
    /// <param name="head">Head item in the sequence</param>
    /// <returns></returns>
    [Pure]
    public static Seq<A> Cons<A>(this A head) =>
        LSeq.FromSingleValue(head);

    /// <summary>
    /// Construct a sequence from any value
    /// </summary>
    /// <typeparam name="A">Type of the items in the sequence</typeparam>
    /// <param name="head">Head item in the sequence</param>
    /// <returns></returns>
    [Pure]
    public static Seq<A> Cons<A>(this A head, SeqEmpty empty) =>
        LSeq.FromSingleValue(head);

    /// <summary>
    /// Construct a list from head and tail; head becomes the first item in 
    /// the list.  
    /// </summary>
    /// <typeparam name="A">Type of the items in the sequence</typeparam>
    /// <param name="head">Head item in the sequence</param>
    /// <param name="tail">Tail of the sequence</param>
    /// <returns></returns>
    [Pure]
    public static Seq<A> Cons<A>(this A head, Seq<A> tail) =>
        tail.Cons(head);

    /// <summary>
    /// Construct a list from head and tail; head becomes the first item in 
    /// the list.  
    /// </summary>
    /// <typeparam name="A">Type of the items in the sequence</typeparam>
    /// <param name="head">Head item in the sequence</param>
    /// <param name="tail">Tail of the sequence</param>
    /// <returns></returns>
    [Pure]
    public static Seq<A> Cons<A>(this A head, A[] tail)
    {
        if (tail.Length == 0)
        {
            return LSeq.FromSingleValue(head);
        }
        else
        {
            var data = new A[tail.Length + 1];
            System.Array.Copy(tail, 0, data, 1, tail.Length);
            data[0] = head;
            return LSeq.FromArray(data);
        }
    }

    /// <summary>
    /// Construct a list from head and tail; head becomes the first item in 
    /// the list.  
    /// </summary>
    /// <typeparam name="A">Type of the items in the sequence</typeparam>
    /// <param name="head">Head item in the sequence</param>
    /// <param name="tail">Tail of the sequence</param>
    /// <returns></returns>
    [Pure]
    public static Seq<A> Cons<A>(this A head, Arr<A> tail) =>
        Cons(head, tail.Value);

    /// <summary>
    /// Construct a list from head and tail; head becomes the first item in 
    /// the list.  
    /// </summary>
    /// <typeparam name="A">Type of the items in the sequence</typeparam>
    /// <param name="head">Head item in the sequence</param>
    /// <param name="tail">Tail of the sequence</param>
    /// <returns></returns>
    [Pure]
    public static Seq<A> Cons<A>(this A head, IEnumerable<A> tail) =>
        tail is Seq<A> seq 
            ? head.Cons(seq)
            : new Seq<A>(tail).Cons(head);

    /// <summary>
    /// Construct a list from head and tail; head becomes the first item in 
    /// the list.  
    /// </summary>
    /// <typeparam name="A">Type of the items in the sequence</typeparam>
    /// <param name="head">Head item in the sequence</param>
    /// <param name="tail">Tail of the sequence</param>
    /// <returns></returns>
    [Pure]
    public static Lst<A> Cons<A>(this A head, Lst<A> tail) =>
        tail.Insert(0, head);

    /// <summary>
    /// Provide a sorted enumerable
    /// </summary>
    [Pure]
    public static IEnumerable<A> Sort<OrdA, A>(this IEnumerable<A> xs) where OrdA : Ord<A> =>
        xs.OrderBy(identity, OrdComparer<OrdA, A>.Default);

    /// <summary>
    /// Provide a sorted Seq
    /// </summary>
    [Pure]
    public static Seq<A> Sort<OrdA, A>(this Seq<A> xs) where OrdA : Ord<A> =>
        xs.OrderBy(identity, OrdComparer<OrdA, A>.Default).AsEnumerableM().ToSeq();

    /// <summary>
    /// Provide a sorted Lst
    /// </summary>
    [Pure]
    public static Lst<A> Sort<OrdA, A>(this Lst<A> xs) where OrdA : Ord<A> =>
        xs.OrderBy(identity, OrdComparer<OrdA, A>.Default).AsEnumerableM().ToLst();

    /// <summary>
    /// Provide a sorted Arr
    /// </summary>
    [Pure]
    public static Arr<A> Sort<OrdA, A>(this Arr<A> xs) where OrdA : Ord<A> =>
        xs.OrderBy(identity, OrdComparer<OrdA, A>.Default).AsEnumerableM().ToArr();

    /// <summary>
    /// Provide a sorted array
    /// </summary>
    [Pure]
    public static A[] Sort<OrdA, A>(this A[] xs) where OrdA : Ord<A> =>
        xs.OrderBy(identity, OrdComparer<OrdA, A>.Default).ToArray();

    /// <summary>
    /// Lazy sequence of natural numbers up to Int32.MaxValue
    /// </summary>
    [Pure]
    public static EnumerableM<int> Naturals
    {
        get
        {
            return Go().AsEnumerableM();
            IEnumerable<int> Go()
            {
                for (var i = 0; i < int.MaxValue; i++)
                {
                    yield return i;
                }
            }
        }
    }

    /// <summary>
    /// Lazy sequence of natural numbers up to Int64.MaxValue
    /// </summary>
    [Pure]
    public static EnumerableM<long> LongNaturals
    {
        get
        {
            return Go().AsEnumerableM();
            IEnumerable<long> Go()
            {
                for (var i = 0L; i < long.MaxValue; i++)
                {
                    yield return i;
                }
            }
        }
    }

    /// <summary>
    /// Lazily generate a range of integers.  
    /// </summary>
    [Pure]
    public static Range<long> Range(long from, long count, long step = 1) =>
        LanguageExt.Range.fromCount(from, count, step);

    /// <summary>
    /// Lazily generate a range of integers.  
    /// </summary>
    [Pure]
    public static Range<int> Range(int from, int count, int step = 1) =>
        LanguageExt.Range.fromCount(from, count, step);

    /// <summary>
    /// Lazily generate a range of chars.  
    /// 
    ///   Remarks:
    ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
    /// </summary>
    [Pure]
    public static Range<char> Range(char from, char to) =>
        to > from
            ? LanguageExt.Range.fromMinMax(from, to, (char)1)
            : LanguageExt.Range.fromMinMax(from, to, (char)1) switch
              {
                  var r => r with { runRange = r.runRange.Reverse() }
              };

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<K, V> Map<K, V>() =>
        LanguageExt.Map.empty<K, V>();

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<K, V> Map<K, V>((K, V) head, params (K, V)[] tail) =>
        LanguageExt.Map.create(head, tail);

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<K, V> toMap<K, V>(IEnumerable<(K, V)> items) =>
        LanguageExt.Map.createRange(items);

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<K, V> toMap<K, V>(ReadOnlySpan<(K, V)> items) =>
        LanguageExt.Map.createRange(items);



    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<OrdK, K, V> Map<OrdK, K, V>() where OrdK : Ord<K> =>
        LanguageExt.Map.empty<OrdK, K, V>();

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<OrdK, K, V> Map<OrdK, K, V>((K, V) head, params (K, V)[] tail) where OrdK : Ord<K> =>
        LanguageExt.Map.create<OrdK, K, V>(head, tail);

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<OrdK, K, V> toMap<OrdK, K, V>(IEnumerable<(K, V)> items) where OrdK : Ord<K> =>
        LanguageExt.Map.createRange<OrdK, K, V>(items);

    /// <summary>
    /// Create an immutable map
    /// </summary>
    [Pure]
    public static Map<OrdK, K, V> toMap<OrdK, K, V>(ReadOnlySpan<(K, V)> items) where OrdK : Ord<K> =>
        LanguageExt.Map.createRange<OrdK, K, V>(items);



    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static HashMap<K, V> HashMap<K, V>() =>
        LanguageExt.HashMap.empty<K, V>();

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static HashMap<K, V> HashMap<K, V>((K, V) head, params (K, V)[] tail) =>
        LanguageExt.HashMap.create(head, tail);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static HashMap<K, V> toHashMap<K, V>(IEnumerable<(K, V)> items) =>
        LanguageExt.HashMap.createRange(items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static HashMap<K, V> toHashMap<K, V>(ReadOnlySpan<(K, V)> items) =>
        LanguageExt.HashMap.createRange(items);



    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static HashMap<EqK, K, V> HashMap<EqK, K, V>() where EqK : Eq<K> =>
        LanguageExt.HashMap.empty<EqK, K, V>();

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static HashMap<EqK, K, V> HashMap<EqK, K, V>((K, V) head, params (K, V)[] tail) where EqK : Eq<K> =>
        LanguageExt.HashMap.create<EqK, K, V>(head, tail);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static HashMap<EqK, K, V> toHashMap<EqK, K, V>(IEnumerable<(K, V)> items) where EqK : Eq<K> =>
        LanguageExt.HashMap.createRange<EqK, K, V>(items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static HashMap<EqK, K, V> toHashMap<EqK, K, V>(ReadOnlySpan<(K, V)> items) where EqK : Eq<K> =>
        LanguageExt.HashMap.createRange<EqK, K, V>(items);


        



    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K, V> TrackingHashMap<K, V>() =>
        LanguageExt.TrackingHashMap.empty<K, V>();

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K, V> TrackingHashMap<K, V>((K, V) head, params (K, V)[] tail) =>
        LanguageExt.TrackingHashMap.create(head, tail);

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K, V> toTrackingHashMap<K, V>(IEnumerable<(K, V)> items) =>
        LanguageExt.TrackingHashMap.createRange(items);

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<K, V> toTrackingHashMap<K, V>(ReadOnlySpan<(K, V)> items) =>
        LanguageExt.TrackingHashMap.createRange(items);



    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<EqK, K, V> TrackingHashMap<EqK, K, V>() where EqK : Eq<K> =>
        LanguageExt.TrackingHashMap.empty<EqK, K, V>();

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<EqK, K, V> TrackingHashMap<EqK, K, V>((K, V) head, params (K, V)[] tail) where EqK : Eq<K> =>
        LanguageExt.TrackingHashMap.create<EqK, K, V>(head, tail);

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<EqK, K, V> toTrackingHashMap<EqK, K, V>(IEnumerable<(K, V)> items) where EqK : Eq<K> =>
        LanguageExt.TrackingHashMap.createRange<EqK, K, V>(items);

    /// <summary>
    /// Create an immutable tracking hash-map
    /// </summary>
    [Pure]
    public static TrackingHashMap<EqK, K, V> toTrackingHashMap<EqK, K, V>(ReadOnlySpan<(K, V)> items) where EqK : Eq<K> =>
        LanguageExt.TrackingHashMap.createRange<EqK, K, V>(items);

        
        


    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<K, V> AtomHashMap<K, V>() =>
        LanguageExt.AtomHashMap<K, V>.Empty;

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<K, V> AtomHashMap<K, V>((K, V) head, params (K, V)[] tail) =>
        LanguageExt.HashMap.create(head, tail).ToAtom();

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<K, V> AtomHashMap<K, V>(HashMap<K, V> items) =>
        new (items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<K, V> toAtomHashMap<K, V>(IEnumerable<(K, V)> items) =>
        LanguageExt.HashMap.createRange(items).ToAtom();

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<K, V> toAtomHashMap<K, V>(ReadOnlySpan<(K, V)> items) =>
        LanguageExt.HashMap.createRange(items).ToAtom();


    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<EqK, K, V> AtomHashMap<EqK, K, V>() where EqK : Eq<K> =>
        LanguageExt.AtomHashMap<EqK, K, V>.Empty;

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<EqK, K, V> AtomHashMap<EqK, K, V>((K, V) head, params (K, V)[] tail) where EqK : Eq<K> =>
        LanguageExt.HashMap.create<EqK, K, V>(head, tail).ToAtom();

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<EqK, K, V> AtomHashMap<EqK, K, V>(HashMap<EqK, K, V> items) where EqK : Eq<K> =>
        new (items);

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<EqK, K, V> toAtomHashMap<EqK, K, V>(IEnumerable<(K, V)> items) where EqK : Eq<K> =>
        LanguageExt.HashMap.createRange<EqK, K, V>(items).ToAtom();

    /// <summary>
    /// Create an immutable hash-map
    /// </summary>
    [Pure]
    public static AtomHashMap<EqK, K, V> toAtomHashMap<EqK, K, V>(ReadOnlySpan<(K, V)> items) where EqK : Eq<K> =>
        LanguageExt.HashMap.createRange<EqK, K, V>(items).ToAtom();

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> List<T>() =>
        Lst<T>.Empty;

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> List<T>(T x, params T[] xs)
    {
        return new Lst<T>(Yield());

        IEnumerable<T> Yield()
        {
            yield return x;
            foreach(var item in xs)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> toList<T>(Arr<T> items) =>
        new (items.Value.AsSpan());

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> toList<T>(IEnumerable<T> items) =>
        items is Lst<T> lst
            ? lst
            : new Lst<T>(items);

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> toList<T>(ReadOnlySpan<T> items) =>
        new (items);


    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<T> Array<T>() =>
        Arr<T>.Empty;

    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<T> Array<T>(T x, params T[] xs) =>
        new (x.Cons(xs).ToArray());

    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<T> toArray<T>(IEnumerable<T> items) =>
        items is Arr<T> arr
            ? arr
            : new Arr<T>(items);

    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<T> toArray<T>(ReadOnlySpan<T> items) =>
        new (items);

    /// <summary>
    /// Create an immutable queue
    /// </summary>
    [Pure]
    public static Que<T> Queue<T>() =>
        Que<T>.Empty;

    /// <summary>
    /// Create an immutable queue
    /// </summary>
    [Pure]
    public static Que<T> Queue<T>(params T[] items)
    {
        var q = new QueInternal<T>();
        foreach (var item in items)
        {
            q = q.Enqueue(item);
        }
        return new Que<T>(q);
    }

    /// <summary>
    /// Create an immutable queue
    /// </summary>
    [Pure]
    public static Que<T> toQueue<T>(IEnumerable<T> items) =>
        new (items);

    /// <summary>
    /// Create an immutable queue
    /// </summary>
    [Pure]
    public static Que<T> toQueue<T>(ReadOnlySpan<T> items) =>
        new (items);

    /// <summary>
    /// Create an immutable stack
    /// </summary>
    [Pure]
    public static Stck<T> Stack<T>() =>
        new ();

    /// <summary>
    /// Create an immutable stack
    /// </summary>
    [Pure]
    public static Stck<T> Stack<T>(params T[] items) =>
        new (items.AsSpan());

    /// <summary>
    /// Create an immutable stack
    /// </summary>
    [Pure]
    public static Stck<T> toStack<T>(IEnumerable<T> items) =>
        items is Stck<T> s
            ? s
            : new Stck<T>(items);

    /// <summary>
    /// Create an immutable stack
    /// </summary>
    [Pure]
    public static Stck<T> toStackRev<T>(IEnumerable<T> items) =>
        new (items.Reverse());

    /// <summary>
    /// Create an immutable map, updating duplicates so that the final value of any key is retained
    /// </summary>
    [Pure]
    public static Map<K, V> toMapUpdate<K, V>(IEnumerable<(K, V)> keyValues) =>
        new(keyValues, false);

    /// <summary>
    /// Create an immutable map, updating duplicates so that the final value of any key is retained
    /// </summary>
    [Pure]
    public static Map<K, V> toMapUpdate<K, V>(ReadOnlySpan<(K, V)> keyValues) =>
        new(keyValues, false);

    /// <summary>
    /// Create an immutable map, ignoring duplicates so the first value of any key is retained
    /// </summary>
    [Pure]
    public static Map<K, V> toMapTry<K, V>(IEnumerable<(K, V)> keyValues) =>
        new(keyValues, false);

    /// <summary>
    /// Create an immutable map, ignoring duplicates so the first value of any key is retained
    /// </summary>
    [Pure]
    public static Map<K, V> toMapTry<K, V>(ReadOnlySpan<(K, V)> keyValues) =>
        new(keyValues, false);



    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<T> Set<T>() =>
        LanguageExt.Set.create<T>();

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<T> Set<T>(T head, params T[] tail) =>
        LanguageExt.Set.createRange(head.Cons(tail));

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<T> toSet<T>(IEnumerable<T> items) =>
        items is Set<T> s
            ? s
            : LanguageExt.Set.createRange(items);

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<T> toSet<T>(ReadOnlySpan<T> items) =>
        LanguageExt.Set.createRange(items);



    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<OrdT, T> Set<OrdT, T>() where OrdT : Ord<T> =>
        LanguageExt.Set.create<OrdT, T>();

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<OrdT, T> Set<OrdT, T>(T head, params T[] tail) where OrdT : Ord<T> =>
        LanguageExt.Set.createRange<OrdT, T>(head.Cons(tail));

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<OrdT, T> toSet<OrdT, T>(IEnumerable<T> items) where OrdT : Ord<T> =>
        items is Set<OrdT, T> s
            ? s
            : LanguageExt.Set.createRange<OrdT, T>(items);

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<OrdT, T> toSet<OrdT, T>(ReadOnlySpan<T> items) where OrdT : Ord<T> =>
        LanguageExt.Set.createRange<OrdT, T>(items);


    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<T> HashSet<T>() =>
        LanguageExt.HashSet.create<T>();

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<T> HashSet<T>(T head, params T[] tail) =>
        LanguageExt.HashSet.createRange(head.Cons(tail));

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<T> toHashSet<T>(IEnumerable<T> items) =>
        items is HashSet<T> hs
            ? hs
            : LanguageExt.HashSet.createRange(items);

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<T> toHashSet<T>(ReadOnlySpan<T> items) =>
        LanguageExt.HashSet.createRange(items);



    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqT, T> HashSet<EqT, T>() where EqT : Eq<T> =>
        LanguageExt.HashSet.create<EqT, T>();

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqT, T> HashSet<EqT, T>(T head, params T[] tail) where EqT : Eq<T> =>
        LanguageExt.HashSet.createRange<EqT, T>(head.Cons(tail));

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqT, T> toHashSet<EqT, T>(IEnumerable<T> items) where EqT : Eq<T> =>
        items is HashSet<EqT, T> hs
            ? hs
            : LanguageExt.HashSet.createRange<EqT, T>(items);

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqT, T> toHashSet<EqT, T>(ReadOnlySpan<T> items) where EqT : Eq<T> =>
        LanguageExt.HashSet.createRange<EqT, T>(items);




    /// <summary>
    /// Create a queryable
    /// </summary>
    [Pure]
    public static IQueryable<T> Query<T>(params T[] items) =>
        toQuery(items);

    /// <summary>
    /// Convert to queryable
    /// </summary>
    [Pure]
    public static IQueryable<T> toQuery<T>(IEnumerable<T> items) =>
        items.AsQueryable();

    /// <summary>
    /// Match empty list, or multi-item list
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="More">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    public static B match<A, B>(IEnumerable<A> list,
                                Func<B> Empty,
                                Func<Seq<A>, B> More) =>
        toSeq(list).Match(Empty, More);

    /// <summary>
    /// List pattern matching
    /// </summary>
    [Pure]
    public static B match<A, B>(IEnumerable<A> list,
                                Func<B> Empty,
                                Func<A, Seq<A>, B> More) =>
        toSeq(list).Match(Empty, More);

    /// <summary>
    /// List pattern matching
    /// </summary>
    [Pure]
    public static R match<T, R>(IEnumerable<T> list,
                                Func<R> Empty,
                                Func<T, R> One,
                                Func<T, Seq<T>, R> More) =>
        toSeq(list).Match(Empty, One, More);

    [Pure]
    public static R match<K, V, R>(Map<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
        match(LanguageExt.Map.find(map, key),
              Some,
              None );

    public static Unit match<K, V>(Map<K, V> map, K key, Action<V> Some, Action None) =>
        match(LanguageExt.Map.find(map, key),
              Some,
              None);

    [Pure]
    public static R match<K, V, R>(HashMap<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
        match(LanguageExt.HashMap.find(map, key),
              Some,
              None);

    public static Unit match<K, V>(HashMap<K, V> map, K key, Action<V> Some, Action None) =>
        match(LanguageExt.HashMap.find(map, key),
              Some,
              None);

    /// <summary>
    /// Construct an empty Seq
    /// </summary>
    [Pure]
    public static Seq<A> Seq<A>() =>
        Empty;
        
    /// <summary>
    /// Construct a sequence from any value
    ///
    ///     var list = Seq1(124);
    /// 
    /// </summary>
    [Pure]
    [Obsolete(Change.UseCollectionIntialiserSeq)]
    public static Seq<A> Seq1<A>(A value)
    {
        var arr = new A[4];
        arr[2] = value;
        return new Seq<A>(new SeqStrict<A>(arr, 2, 1, 0, 0));
    }
       
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
            Arr<A> arr          => LSeq.FromArray(arr.Value),
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
    public static Seq<A> toSeq<A>(IEnumerable<A>? value) =>
        value switch
        {
            null                => Empty,
            Seq<A> seq          => seq,
            Arr<A> arr          => LSeq.FromArray(arr.Value),
            A[] array           => toSeq(array),
            IList<A> list       => toSeq(list),
            ICollection<A> coll => toSeq(coll),
            _                   => new Seq<A>(value)
        };
        
    /// <summary>
    /// Construct a sequence from a nullable
    ///     HasValue == true  : [x]
    ///     HasValue == false : []
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(A? value) where A : struct =>
        value is null 
            ? Empty 
            : LSeq.FromSingleValue(value.Value);
        
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
            return LSeq.FromArray(data);
        }
    }

    /// <summary>
    /// Construct a sequence from an immutable array
    /// </summary>
    [Pure]
    public static Seq<A> toSeq<A>(Arr<A> value) =>
        toSeq(value.Value);

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
    /// Construct a sequence from an either
    ///     Right(x) : [x]
    ///     Left(y)  : []
    /// </summary>
    [Pure]
    public static Seq<R> toSeq<L, R>(Either<L, R> value) =>
        value.IsRight
            ? LSeq.FromSingleValue(value.RightValue)
            : Empty;

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

    /// <summary>
    /// Construct an empty AtomSeq
    /// </summary>
    [Pure]
    public static AtomSeq<A> AtomSeq<A>() =>
        new (SeqStrict<A>.Empty);

    /// <summary>
    /// Construct an AtomSeq
    /// </summary>
    [Pure]
    public static AtomSeq<A> AtomSeq<A>(params A[] items) =>
        new (LSeq.FromArray(items).Value);

    /// <summary>
    /// Construct an AtomSeq
    /// </summary>
    [Pure]
    public static AtomSeq<A> AtomSeq<A>(Seq<A> items) =>
        new (items.Value);

    /// <summary>
    /// Construct an AtomSeq
    /// </summary>
    [Pure]
    public static AtomSeq<A> AtomSeq<A>(IEnumerable<A> items) =>
        new (items);

    /// <summary>
    /// Compute the difference between two documents, using the Wagner-Fischer algorithm. 
    /// O(mn) time and space.
    /// 
    ///     apply(diff(d,e), d) == e
    ///     
    ///     diff(d, d) == Patch.empty
    ///     
    ///     apply(diff(d, e), d) == apply(inverse(diff(e, d)), d)
    ///     
    ///     apply(append(diff(a, b), diff(b, c), a) == apply(diff(a, c), a)
    ///     
    ///     applicable(diff(a, b) a)
    /// 
    /// </summary>
    public static Patch<EqA, A> Diff<EqA, A>(this IEnumerable<A> va, IEnumerable<A> vb) where EqA : Eq<A> =>
        Patch.diff<EqA, A>(va, vb);

    /// <summary>
    /// Apply the supplied patch to this collection
    /// </summary>
    public static IEnumerable<A> Apply<EqA, A>(this IEnumerable<A> va, Patch<EqA, A> patch) where EqA : Eq<A> =>
        Patch.apply(patch, va);

    /// <summary>
    /// Apply the supplied patch to this collection
    /// </summary>
    public static Lst<A> Apply<EqA, A>(this Lst<A> va, Patch<EqA, A> patch) where EqA : Eq<A> =>
        Patch.apply(patch, va);

    /// <summary>
    /// Apply the supplied patch to this collection
    /// </summary>
    public static Seq<A> Apply<EqA, A>(this Seq<A> va, Patch<EqA, A> patch) where EqA : Eq<A> =>
        Patch.apply(patch, va);

    /// <summary>
    /// Apply the supplied patch to this collection
    /// </summary>
    public static SpanArray<A> Apply<EqA, A>(this SpanArray<A> va, Patch<EqA, A> patch) where EqA : Eq<A> =>
        Patch.apply(patch, va);

    /// <summary>
    /// Apply the supplied patch to this collection
    /// </summary>
    public static A[] Apply<EqA, A>(this A[] va, Patch<EqA, A> patch) where EqA : Eq<A> =>
        Patch.apply(patch, va);

    /// <summary>
    /// Apply the supplied patch to this collection
    /// </summary>
    public static Arr<A> Apply<EqA, A>(this Arr<A> va, Patch<EqA, A> patch) where EqA : Eq<A> =>
        Patch.apply(patch, va);

    /// <summary>
    /// Apply the supplied patch to this collection
    /// </summary>
    public static List<A> Apply<EqA, A>(this List<A> va, Patch<EqA, A> patch) where EqA : Eq<A> =>
        Patch.apply(patch, va);

    /// <summary>
    /// Returns true if a patch can be safely applied to a document, that is,
    /// `applicable(p, d)` holds when `d` is a valid source document for the patch `p`.
    /// </summary>
    public static bool Applicable<EqA, A>(this IEnumerable<A> va, Patch<EqA, A> patch) where EqA : Eq<A> =>
        Patch.applicable(patch, va);
}
