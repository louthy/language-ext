using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    [Typeclass("BiF*Async")]
    public interface BiFunctorAsync<FAB, FR, A, B, R> : Typeclass
    {
        /// <summary>
        /// Projection from one bi-functor to another.  This operation
        /// should map only one of the items (A, or B).  The type R
        /// should match A, or B depending on which item is being mapped.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FR">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="R">Target item value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FR BiMapAsync(FAB ma, Func<A, R> fa, Func<B, R> fb);

        /// <summary>
        /// Projection from one bi-functor to another.  This operation
        /// should map only one of the items (A, or B).  The type R
        /// should match A, or B depending on which item is being mapped.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FR">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="R">Target item value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FR BiMapAsync(FAB ma, Func<A, Task<R>> fa, Func<B, R> fb);

        /// <summary>
        /// Projection from one bi-functor to another.  This operation
        /// should map only one of the items (A, or B).  The type R
        /// should match A, or B depending on which item is being mapped.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FR">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="R">Target item value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FR BiMapAsync(FAB ma, Func<A, R> fa, Func<B, Task<R>> fb);

        /// <summary>
        /// Projection from one bi-functor to another.  This operation
        /// should map only one of the items (A, or B).  The type R
        /// should match A, or B depending on which item is being mapped.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FR">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="R">Target item value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FR BiMapAsync(FAB ma, Func<A, Task<R>> fa, Func<B, Task<R>> fb);

    }

    /// <summary>
    /// Functor type-class
    /// </summary>
    [Typeclass("BiF*Async")]
    public interface BiFunctorAsync<FAB, FUV, A, B, U, V> : Typeclass
    {
        /// <summary>
        /// Projection from one value to another.  Both elements of
        /// the bi-functor can will be mapped to a new result value.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FUV">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="U">Target item 1 value type</typeparam>
        /// <typeparam name="V">Target item 2 value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FUV BiMapAsync(FAB ma, Func<A, U> fa, Func<B, V> fb);

        /// <summary>
        /// Projection from one value to another.  Both elements of
        /// the bi-functor can will be mapped to a new result value.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FUV">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="U">Target item 1 value type</typeparam>
        /// <typeparam name="V">Target item 2 value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FUV BiMapAsync(FAB ma, Func<A, Task<U>> fa, Func<B, V> fb);

        /// <summary>
        /// Projection from one value to another.  Both elements of
        /// the bi-functor can will be mapped to a new result value.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FUV">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="U">Target item 1 value type</typeparam>
        /// <typeparam name="V">Target item 2 value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FUV BiMapAsync(FAB ma, Func<A, U> fa, Func<B, Task<V>> fb);

        /// <summary>
        /// Projection from one value to another.  Both elements of
        /// the bi-functor can will be mapped to a new result value.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FUV">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="U">Target item 1 value type</typeparam>
        /// <typeparam name="V">Target item 2 value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FUV BiMapAsync(FAB ma, Func<A, Task<U>> fa, Func<B, Task<V>> fb);
    }
}
