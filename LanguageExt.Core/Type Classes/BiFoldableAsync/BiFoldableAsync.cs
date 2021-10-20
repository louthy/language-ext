using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass("BiFold*Async")]
    public interface BiFoldableAsync<F, A, B> : Typeclass
    {
        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFold<S>(F foldable, S state, Func<S, A, S> fa, Func<S, B, S> fb);

        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldAsync<S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, B, S> fb);

        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldAsync<S>(F foldable, S state, Func<S, A, S> fa, Func<S, B, Task<S>> fb);

        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldAsync<S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, B, Task<S>> fb);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldBack<S>(F foldable, S state, Func<S, A, S> fa, Func<S, B, S> fb);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldBackAsync<S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, B, S> fb);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldBackAsync<S>(F foldable, S state, Func<S, A, S> fa, Func<S, B, Task<S>> fb);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldBackAsync<S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, B, Task<S>> fb);
    }

    [Typeclass("BiFold*Async")]
    public interface BiFoldableAsync<F, A> : Typeclass
    {
        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFold<S>(F foldable, S state, Func<S, A, S> fa, Func<S, S> fb);

        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldAsync<S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, S> fb);

        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldAsync<S>(F foldable, S state, Func<S, A, S> fa, Func<S, Task<S>> fb);

        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldAsync<S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, Task<S>> fb);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldBack<S>(F foldable, S state, Func<S, A, S> fa, Func<S, S> fb);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldBackAsync<S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, S> fb);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldBackAsync<S>(F foldable, S state, Func<S, A, S> fa, Func<S, Task<S>> fb);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right.
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        Task<S> BiFoldBackAsync<S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, Task<S>> fb);
    }
}
