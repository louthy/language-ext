using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class TypeClass
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
        public static Task<S> biFoldAsync<FOLD, F, A, B, S>(F foldable, S state, Func<S, A, S> fa, Func<S, B, S> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            default(FOLD).BiFold(foldable, state, fa, fb);

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
        public static Task<S> biFoldAsync<FOLD, F, A, B, S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, B, S> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            default(FOLD).BiFoldAsync(foldable, state, fa, fb);

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
        public static Task<S> biFoldAsync<FOLD, F, A, B, S>(F foldable, S state, Func<S, A, S> fa, Func<S, B, Task<S>> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            default(FOLD).BiFoldAsync(foldable, state, fa, fb);

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
        public static Task<S> biFoldAsync<FOLD, F, A, B, S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, B, Task<S>> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            default(FOLD).BiFoldAsync(foldable, state, fa, fb);

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
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static Task<S> biFoldBackAsync<FOLD, F, A, B, S>(F foldable, S state, Func<S, A, S> fa, Func<S, B, S> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            default(FOLD).BiFoldBack(foldable, state, fa, fb);

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
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static Task<S> biFoldBackAsync<FOLD, F, A, B, S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, B, S> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            default(FOLD).BiFoldBackAsync(foldable, state, fa, fb);

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
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static Task<S> biFoldBackAsync<FOLD, F, A, B, S>(F foldable, S state, Func<S, A, S> fa, Func<S, B, Task<S>> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            default(FOLD).BiFoldBackAsync(foldable, state, fa, fb);

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
        /// <param name="fa">Folder function, applied for each item in foldable</param>
        /// <param name="fb">Folder function, applied for each item in foldable</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static Task<S> biFoldBackAsync<FOLD, F, A, B, S>(F foldable, S state, Func<S, A, Task<S>> fa, Func<S, B, Task<S>> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            default(FOLD).BiFoldBackAsync(foldable, state, fa, fb);

        /// <summary>
        /// Iterate the values in the foldable
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="foldable">Foldable to perform the operation on</param>
        public static Task<Unit> biIterAsync<FOLD, F, A, B>(F foldable, Action<A> fa, Action<B> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            biFoldAsync<FOLD, F, A, B, Unit>(foldable, unit, (s,x) => { fa(x); return unit; }, (s, x) => { fb(x); return unit; });

        /// <summary>
        /// Turn any foldable into a sequence
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="foldable">Foldable</param>
        /// <returns>Sequence of As</returns>
        [Pure]
        public static Task<IEnumerable<Either<A, B>>> toBiSeqAsync<FOLD, F, A, B>(F foldable)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            biFoldBackAsync<FOLD, F, A, B, IEnumerable<Either<A, B>>>(foldable, new Either<A, B>[0], (s, x) => Left<A,B>(x).Cons(s), (s, x) => Right<A, B>(x).Cons(s));

        /// <summary>
        /// Convert the foldable to a sequence (IEnumerable) performing a map operation
        /// on each item in the structure
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to performt the operation on</param>
        /// <returns>Sequence of As that represent the value(s) in the structure</returns>
        [Pure]
        public static Task<IEnumerable<C>> collectAsync<FOLD, F, A, B, C>(F foldable, Func<A, C> fa, Func<B, C> fb)
            where FOLD : struct, BiFoldableAsync<F, A, B> =>
            biFoldBackAsync<FOLD, F, A, B, IEnumerable<C>>(foldable, Enumerable.Empty<C>(), (s, x) => fa(x).Cons(s), (s, x) => fb(x).Cons(s));

        /// <summary>
        /// Does the element occur in the structure?
        /// </summary>
        /// <typeparam name="EQ">Eq<A> type-class</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="foldable">Foldable</param>
        /// <param name="item">Item to test</param>
        /// <returns>True if item in the structure</returns>
        [Pure]
        public static async Task<bool> containsAsync<EQ, FOLD, F, A, B>(F foldable, A item)
            where FOLD : struct, BiFoldableAsync<F, A, B>
            where EQ   : struct, Eq<A>
        {
            foreach (var x in (await toBiSeqAsync<FOLD, F, A, B>(foldable).ConfigureAwait(false)).Lefts())
            {
                if (equals<EQ, A>(x, item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Does the element occur in the structure?
        /// </summary>
        /// <typeparam name="EQ">Eq<B> type-class</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="foldable">Foldable</param>
        /// <param name="item">Item to test</param>
        /// <returns>True if item in the structure</returns>
        [Pure]
        public static async Task<bool> containsAsync<EQ, FOLD, F, A, B>(F foldable, B item)
            where FOLD : struct, BiFoldableAsync<F, A, B>
            where EQ   : struct, Eq<B>
        {
            foreach (var x in (await toBiSeqAsync<FOLD, F, A, B>(foldable).ConfigureAwait(false)).Rights())
            {
                if (equals<EQ, B>(x, item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Runs a predicate against the bound value(s).  If the predicate
        /// holds for all values then true is returned.  
        /// 
        /// NOTE: An empty structure will return true.
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>True if the predicate holds for all values</returns>
        [Pure]
        public static async Task<bool> biForAllAsync<FOLD, F, A, B>(F foldable, Func<A, bool> preda, Func<B, bool> predb)
            where FOLD : struct, BiFoldableAsync<F, A, B>
        {
            foreach (var item in await toBiSeqAsync<FOLD, F, A, B>(foldable).ConfigureAwait(false))
            {
                if (!item.Match(Left: preda, Right: predb)) return false;
            }
            return true;
        }

        /// <summary>
        /// Runs a predicate against the bound value(s).  If the predicate
        /// returns true for any item then the operation immediately returns
        /// true.  False is returned if no items in the structure match the
        /// predicate.
        /// 
        /// NOTE: An empty structure will return false.
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>True if the predicate holds for all values</returns>
        [Pure]
        public static async Task<bool> biExistsAsync<FOLD, F, A, B>(F foldable, Func<A, bool> preda, Func<B, bool> predb)
            where FOLD : struct, BiFoldableAsync<F, A, B>
        {
            foreach (var item in await toBiSeqAsync<FOLD, F, A, B>(foldable).ConfigureAwait(false))
            {
                if (item.Match(Left: preda, Right: predb)) return true;
            }
            return false;
        }
    }
}
