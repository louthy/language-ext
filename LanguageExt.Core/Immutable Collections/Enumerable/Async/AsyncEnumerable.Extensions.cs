#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="ma">Enumerable to inject values into</param>
    /// <typeparam name="A">Bound type</typeparam>
    extension<A>(IAsyncEnumerable<A> ma)
    {
        /// <summary>
        /// Create an iterator from an `IAsyncEnumerable` collection
        /// </summary>
        /// <returns>IteratorIO</returns>
        [Pure]
        public IteratorIO<A> AsIteratorIO() =>
            IteratorIO.forward(ma);

        /// <summary>
        /// Create an iterable from an `IAsyncEnumerable` collection
        /// </summary>
        /// <returns>IterableIO</returns>
        [Pure]
        public IterableIO<A> AsIterableIO() =>
            IterableIO.createRange(ma);
    }
}
