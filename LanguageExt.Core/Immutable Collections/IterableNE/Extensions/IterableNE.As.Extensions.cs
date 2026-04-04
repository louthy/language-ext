#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IterableNEExtensions
{
    extension<A>(K<IterableNE, A> ma)
    {
        public IterableNE<A> As() =>
            (IterableNE<A>)ma;
    }

    extension<A>(IterableNE<A> ma)
    {
        /// <summary>
        /// Create an iterator from an `IterableNE` collection
        /// </summary>
        /// <returns>Iterator</returns>
        [Pure]
        public Iterator<A> AsIterator() =>
            ma.ForwardIterator();
        
        /// <summary>
        /// Create an iterable from an `IterableNE` collection
        /// </summary>
        /// <returns>Iterable</returns>
        [Pure]
        public Iterable<A> AsIterable() =>
            new(ma.AsIterator());

        /// <summary>
        /// Create a non-empty iterable from an `IterableNE` collection.
        /// </summary>
        /// <returns>IterableNE</returns>
        [Pure]
        public IterableNE<A> AsIterableNE() =>
            ma;

        /// <summary>
        /// Create an iterable from an `IterableNE` collection
        /// </summary>
        /// <returns>IterableIO</returns>
        public IterableIO<A> AsIterableIO() =>
            new(ma.AsIteratorIO());
        
        /// <summary>
        /// Create an iterator from an `IterableNE` collection
        /// </summary>
        /// <returns>IteratorIO</returns>
        [Pure]
        public IteratorIO<A> AsIteratorIO() =>
            IteratorIO.lift(ma.AsIterator());
    }
}
