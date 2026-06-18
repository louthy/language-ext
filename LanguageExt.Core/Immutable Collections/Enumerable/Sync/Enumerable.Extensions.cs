#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt;

public static partial class EnumerableExtensions
{
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<A>(IEnumerable<A> ma)
    {
        /// <summary>
        /// Create an iterable from an `IEnumerable` collection
        /// </summary>
        /// <remarks>This forces evaluation of the `IEnumerable` to construct an `Iterable` with an fixed sized array
        /// instead of the lazy sequence.  This is to avoid the use of impure enumerators in the resulting `Iterable`.
        ///
        /// If you need `IEnumerable` to be lazy, use `AsIterableIO` instead.
        /// </remarks>
        /// <returns>An iterable</returns>
        public Iterable<A> AsIterableStrict() =>
            Iterable.create(ma.ToArray());
        
        /// <summary>
        /// Create an iterable from an `IEnumerable` collection
        /// </summary>
        /// <returns>IterableIO</returns>
        public IterableIO<A> AsIterableIO() =>
            IterableIO.createRange(ma);

        /// <summary>
        /// Create an iterable from an `IEnumerable` collection
        /// </summary>
        /// <remarks>This forces evaluation of the `IEnumerable` to construct an `Iterable` with an fixed sized array
        /// instead of the lazy sequence.  This is to avoid the use of impure enumerators in the resulting `Iterable`.
        ///
        /// If you need `IEnumerable` to be lazy, use `AsIterableIO` instead.
        /// </remarks>
        /// <returns>An iterable</returns>
        public Iterator<A> AsIteratorStrict() =>
            Iterator.forward(ma.ToArray());
        
        /// <summary>
        /// Create an iterator from an `IEnumerable` collection
        /// </summary>
        /// <returns>IteratorIO</returns>
        [Pure]
        public IteratorIO<A> AsIteratorIO() =>
            new IteratorIO<A>.Enumerable(ma);
        
        /// <summary>
        /// Convert an IEnumerable to a `Seq`
        /// </summary>
        [Pure]
        public Seq<A> AsSeq() =>
            Seq.createRange(ma);
        
        /// <summary>
        /// Convert an IEnumerable to an `Arr`
        /// </summary>
        [Pure]
        public Arr<A> AsArr() =>
            Arr.createRange(ma);
        
        /// <summary>
        /// Convert an IEnumerable to a `Lst`
        /// </summary>
        [Pure]
        public Lst<A> AsLst() =>
            Lst.createRange(ma);
        
        /// <summary>
        /// Convert an IEnumerable to a `Set`
        /// </summary>
        [Pure]
        public Set<A> AsSet() =>
            Set.createRange(ma);
        
        /// <summary>
        /// Convert an IEnumerable to a `HashMap`
        /// </summary>
        [Pure]
        public HashSet<A> AsHashSet() =>
            HashSet.createRange(ma);        
    }
    
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<A>(IList<A> ma)
    {
        /// <summary>
        /// Create an iterator from an `IEnumerable` collection
        /// </summary>
        /// <returns>Iterator</returns>
        [Pure]
        public Iterator<A> AsIterator() =>
            new Iterator<A>.IterGenList(ma, 0, ma.Count);
        
        /// <summary>
        /// Create an iterator from an `IEnumerable` collection
        /// </summary>
        /// <returns>Iterator</returns>
        [Pure]
        public IteratorIO<A> AsIteratorIO() =>
            ma.AsIterator().AsIteratorIO();
        /// <summary>
        /// Create an iterator from an `IEnumerable` collection
        /// </summary>
        /// <returns>Iterator</returns>
        [Pure]
        public Iterable<A> AsIterable() =>
            ma.AsIterator().AsIterable();
        
        /// <summary>
        /// Create an iterator from an `IEnumerable` collection
        /// </summary>
        /// <returns>Iterator</returns>
        [Pure]
        public IterableIO<A> AsIterableIO() =>
            ma.AsIteratorIO().AsIterable();
    }
    
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<K, V>(IEnumerable<(K, V)> ma)
    {
        /// <summary>
        /// Convert an IEnumerable to a `Map`
        /// </summary>
        [Pure]
        public Map<K, V> AsMap() =>
            Map.createRange(ma);
        
        /// <summary>
        /// Convert an IEnumerable to a `HashMap`
        /// </summary>
        [Pure]
        public HashMap<K, V> AsHashMap() =>
            HashMap.createRange(ma);
    }
    
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<K1, K2, V>(IEnumerable<(K1, K2, V)> ma)
    {
        /// <summary>
        /// Convert an IEnumerable to a `Map`
        /// </summary>
        [Pure]
        public Map<(K1, K2), V> AsMap() =>
            new (ma.Select(x => ((x.Item1, x.Item2), x.Item3)));
        
        /// <summary>
        /// Convert an IEnumerable to a `HashMap`
        /// </summary>
        [Pure]
        public HashMap<(K1, K2), V> AsHashMap() =>
            new (ma.Select(x => ((x.Item1, x.Item2), x.Item3)));
    }
    
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<K1, K2, K3, V>(IEnumerable<(K1, K2, K3, V)> ma)
    {
        /// <summary>
        /// Convert an IEnumerable to a `Map`
        /// </summary>
        [Pure]
        public Map<(K1, K2, K3), V> AsMap() =>
            new (ma.Select(x => ((x.Item1, x.Item2, x.Item3), x.Item4)));
        
        /// <summary>
        /// Convert an IEnumerable to a `HashMap`
        /// </summary>
        [Pure]
        public HashMap<(K1, K2, K3), V> AsHashMap() =>
            new (ma.Select(x => ((x.Item1, x.Item2, x.Item3), x.Item4)));
    }
    
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<K1, K2, K3, K4, V>(IEnumerable<(K1, K2, K3, K4, V)> ma)
    {
        /// <summary>
        /// Convert an IEnumerable to a `Map`
        /// </summary>
        [Pure]
        public Map<(K1, K2, K3, K4), V> AsMap() =>
            new (ma.Select(x => ((x.Item1, x.Item2, x.Item3, x.Item4), x.Item5)));
        
        /// <summary>
        /// Convert an IEnumerable to a `HashMap`
        /// </summary>
        [Pure]
        public HashMap<(K1, K2, K3, K4), V> AsHashMap() =>
            new (ma.Select(x => ((x.Item1, x.Item2, x.Item3, x.Item4), x.Item5)));
    }
}
