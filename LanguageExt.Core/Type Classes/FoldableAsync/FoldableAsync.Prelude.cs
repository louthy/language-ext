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
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static Task<S> foldAsync<FOLD, F, A, S>(F fa, S state, Func<S, A, S> f) where FOLD : FoldableAsync<F, A> =>
            default(FOLD).Fold(fa, state, f)(unit);

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
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static Task<S> foldAsync<FOLD, F, A, S>(F fa, S state, Func<S, A, Task<S>> f) where FOLD : FoldableAsync<F, A> =>
            default(FOLD).FoldAsync(fa, state, f)(unit);

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
        public static Task<S> foldBackAsync<FOLD, F, A, S>(F fa, S state, Func<S, A, S> f) where FOLD : FoldableAsync<F, A> =>
            default(FOLD).FoldBack(fa, state, f)(unit);

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
        public static Task<S> foldBackAsync<FOLD, F, A, S>(F fa, S state, Func<S, A, Task<S>> f) where FOLD : FoldableAsync<F, A> =>
            default(FOLD).FoldBackAsync(fa, state, f)(unit);

        /// <summary>
        /// Iterate the values in the foldable
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to perform the operation on</param>
        public static async Task<Unit> iterAsync<FOLD, F, A>(F fa, Action<A> action) where FOLD : FoldableAsync<F, A>
        {
            await Task.WhenAll((await toSeqAsync<FOLD, F, A>(fa)).Map(a => { action(a); return unit.AsTask(); })).ConfigureAwait(false);
            return unit;
        }

        /// <summary>
        /// Iterate the values in the foldable
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to perform the operation on</param>
        public static async Task<Unit> iterAsync<FOLD, F, A>(F fa, Func<A, Task<Unit>> action) where FOLD : FoldableAsync<F, A>
        {
            await Task.WhenAll((await toSeqAsync<FOLD, F, A>(fa)).Map(action)).ConfigureAwait(false);
            return unit;
        }

        /// <summary>
        /// Turn any foldable into a sequence
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Sequence of As</returns>
        [Pure]
        public static Task<Seq<A>> toSeqAsync<FOLD, F, A>(F fa) where FOLD : FoldableAsync<F, A> =>
            default(FOLD).FoldBack(fa, Seq<A>.Empty, (s, x) => x.Cons(s))(unit);


        /// <summary>
        /// Convert the foldable to a sequence (IEnumerable) performing a map operation
        /// on each item in the structure
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to performt the operation on</param>
        /// <returns>Sequence of As that represent the value(s) in the structure</returns>
        [Pure]
        public static Task<IEnumerable<B>> collectAsync<FOLD, F, A, B>(F self, Func<A, B> f) where FOLD : FoldableAsync<F, A> =>
            default(FOLD).FoldBack(self, Enumerable.Empty<B>(), (s, x) => f(x).Cons(s))(unit);

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable</returns>
        [Pure]
        public static async Task<A> headAsync<FOLD, F, A>(F fa) where FOLD : FoldableAsync<F, A> =>
            (await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false)).Head();

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static async Task<Option<A>> headOrNoneAsync<FOLD, F, A>(F fa) where FOLD : FoldableAsync<F, A> =>
            (await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false)).HeadOrNone();

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="fail">Fail case</param>
        /// <returns>First A produced by the foldable (Or Fail if no items produced)</returns>
        [Pure]
        public static async Task<Validation<FAIL, A>> headOrInvalidAsync<FOLD, F, FAIL, A>(F fa, FAIL fail) where FOLD : FoldableAsync<F, A> =>
            (await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false)).HeadOrInvalid(fail);

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="left">Left case</param>
        /// <returns>First A produced by the foldable (Or Left if no items produced)</returns>
        [Pure]
        public static async Task<Either<L, A>> headOrLeftAsync<FOLD, F, L, A>(F fa, L left) where FOLD : FoldableAsync<F, A> =>
            (await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false)).HeadOrLeft(left);

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable</returns>
        [Pure]
        public static async Task<A> lastAsync<FOLD, F, A>(F fa) where FOLD : FoldableAsync<F, A> =>
            (await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false)).Last();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static async Task<Option<A>> lastOrNoneAsync<FOLD, F, A>(F fa) where FOLD : FoldableAsync<F, A> =>
            (await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false))
                .Map(x => Prelude.Some(x))
                .DefaultIfEmpty(Option<A>.None)
                .LastOrDefault();

        /// <summary>
        /// Tests whether the foldable structure is empty
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static async Task<bool> isEmptyAsync<FOLD, F, A>(F fa) where FOLD : FoldableAsync<F, A> =>
            !(await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false)).Any();

        /// <summary>
        /// Find the length of a foldable structure 
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static Task<int> countAsync<FOLD, F, A>(F fa) where FOLD : FoldableAsync<F, A> =>
            default(FOLD).Count(fa)(unit);

        /// <summary>
        /// Does the element occur in the structure?
        /// </summary>
        /// <typeparam name="EQ">Eq<A> type-class</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="item">Item to test</param>
        /// <returns>True if item in the structure</returns>
        [Pure]
        public static async Task<bool> containsAsync<EQ, FOLD, F, A>(F fa, A item)
            where EQ : struct, Eq<A>
            where FOLD : FoldableAsync<F, A> =>
            (await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false)).Exists(x => equals<EQ, A>(x, item));

        /// <summary>
        /// The 'sum' function computes the sum of the numbers of a structure.
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Sum of the numbers in the structure</returns>
        [Pure]
        public static Task<A> sumAsync<NUM, FOLD, F, A>(F fa) 
            where FOLD : FoldableAsync<F, A> 
            where NUM : struct, Num<A> =>
                default(FOLD).Fold(fa, fromInteger<NUM, A>(0), (s, x) => plus<NUM, A>(s, x))(unit);

        /// <summary>
        /// The 'product' function computes the product of the numbers of a structure.
        /// </summary>
        /// <typeparam name="NUM">Foldable && NUM type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Product of the numbers in the structure</returns>
        [Pure]
        public static Task<A> productAsync<NUM, FOLD, F, A>(F fa)
            where FOLD : FoldableAsync<F, A>
            where NUM : struct, Num<A> =>
                default(FOLD).Fold(fa, fromInteger<NUM, A>(1), (s, x) => product<NUM, A>(s, x))(unit);

        /// <summary>
        /// Runs a predicate against the bound value(s).  If the predicate
        /// holds for all values then true is returned.  
        /// 
        /// NOTE: An empty structure will return true.
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>True if the predicate holds for all values</returns>
        [Pure]
        public static async Task<bool> forallAsync<FOLD, F, A>(F fa, Func<A,bool> pred) where FOLD : FoldableAsync<F, A>
        {
            foreach(var item in await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false))
            {
                if (!pred(item)) return false;
            }
            return true;
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
        public static async Task<bool> forallAsync<FOLD, F, A>(F fa, Func<A, Task<bool>> pred) where FOLD : FoldableAsync<F, A>
        {
            foreach (var item in await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false))
            {
                if (!(await pred(item).ConfigureAwait(false))) return false;
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
        public static async Task<bool> existsAsync<FOLD, F, A>(F fa, Func<A, bool> pred) where FOLD : FoldableAsync<F, A>
        {
            foreach (var item in await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false))
            {
                if (pred(item)) return true;
            }
            return false;
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
        public static async Task<bool> existsAsync<FOLD, F, A>(F fa, Func<A, Task<bool>> pred) where FOLD : FoldableAsync<F, A>
        {
            foreach (var item in await toSeqAsync<FOLD, F, A>(fa).ConfigureAwait(false))
            {
                if (await pred(item).ConfigureAwait(false)) return true;
            }
            return false;
        }
    }
}
