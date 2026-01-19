#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class EnumerableExtensions
{
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<A>(IEnumerable<A> ma)
    {
        /// <summary>
        /// Create an iterator from an `IEnumerable` collection
        /// </summary>
        /// <returns>Iterator</returns>
        [Pure]
        public Iterator<A> AsIterator() =>
            new Iterator<A>.Enumerable(ma);
        
        /// <summary>
        /// Create an iterable from an `IEnumerable` collection
        /// </summary>
        /// <returns>Iterable</returns>
        [Pure]
        public Iterable<A> AsIterable() =>
            Iterable.createRange(ma);
        
        /// <summary>
        /// Create a non-empty iterable from an `IEnumerable` collection.  You must provide the head
        /// value due to the possibility of an empty collection.
        /// </summary>
        /// <param name="head">Head value</param>
        /// <returns>IterableNE</returns>
        [Pure]
        public IterableNE<A> AsIterableNE(A head) =>
            IterableNE.create(head, Iterator.forward(ma));

        /// <summary>
        /// Create an iterable from an `IEnumerable` collection
        /// </summary>
        /// <returns>IterableIO</returns>
        public IterableIO<A> AsIterableIO() =>
            IterableIO.createRange(ma);
        
        /// <summary>
        /// Create an iterator from an `IEnumerable` collection
        /// </summary>
        /// <returns>IteratorIO</returns>
        [Pure]
        public IteratorIO<A> AsIteratorIO() =>
            new IteratorIO<A>.Enumerable(ma);
    }
}
