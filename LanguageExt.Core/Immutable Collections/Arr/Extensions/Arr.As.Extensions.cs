#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ArrExtensions
{
    extension<A>(K<Arr, A> ma)
    {
        public Arr<A> As() =>
            (Arr<A>)ma;
    }

    extension<A>(Arr<A> ma)
    {
        /// <summary>
        /// Create an iterator from an `Arr` collection
        /// </summary>
        /// <returns>Iterator</returns>
        [Pure]
        public Iterator<A> AsIterator() =>
            ma.ForwardIterator();
        
        /// <summary>
        /// Create an iterable from an `Arr` collection
        /// </summary>
        /// <returns>Iterable</returns>
        [Pure]
        public Iterable<A> AsIterable() =>
            new (ma.AsIterator());
        
        /// <summary>
        /// Create a non-empty iterable from an `Arr` collection.  You must provide the head
        /// value due to the possibility of an empty collection.
        /// </summary>
        /// <param name="head">Head value</param>
        /// <returns>IterableNE</returns>
        [Pure]
        public IterableNE<A> AsIterableNE(A head) =>
            IterableNE.create(head, ma.AsIterator());

        /// <summary>
        /// Create an iterable from an `Arr` collection
        /// </summary>
        /// <returns>IterableIO</returns>
        public IterableIO<A> AsIterableIO() =>
            new(ma.AsIteratorIO());
        
        /// <summary>
        /// Create an iterator from an `Arr` collection
        /// </summary>
        /// <returns>IteratorIO</returns>
        [Pure]
        public IteratorIO<A> AsIteratorIO() =>
            IteratorIO.lift(ma.AsIterator());
    }
}
