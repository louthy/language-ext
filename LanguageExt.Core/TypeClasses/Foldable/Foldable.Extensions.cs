using System;
using System.Linq;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// 
        /// Fold([x1, x2, ..., xn] == x1 `f` (x2 `f` ... (xn `f` z)...)
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
        public static S Fold<FOLD, F, A, S>(this F fa, S state, Func<S, A, S> f) where FOLD : struct, Foldable<F, A> =>
            default(FOLD).Fold(fa, state, f);

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// 
        /// FoldBack( [x1, x2, ..., xn]) == (...((z `f` x1) `f` x2) `f`...) `f` xn
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static S FoldBack<FOLD, F, A, S>(this F fa, S state, Func<S, A, S> f) where FOLD : struct, Foldable<F, A> =>
            default(FOLD).FoldBack(fa, state, f);

        /// <summary>
        /// Iterate the values in the foldable
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to perform the operation on</param>
        public static Unit Iter<FOLD, F, A>(this F fa, Action<A> action) where FOLD : struct, Foldable<F, A>
        {
            foreach (var item in toSeq<FOLD, F, A>(fa))
            {
                action(item);
            }
            return unit;
        }

        /// <summary>
        /// Turn any foldable into a sequence
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Sequence of As</returns>
        [Pure]
        public static IEnumerable<A> ToSeq<FOLD, F, A>(this F fa) where FOLD : struct, Foldable<F, A> =>
            default(FOLD).FoldBack(fa, new A[0].AsEnumerable(), (s, x) => x.Cons(s));

        /// <summary>
        /// Convert the foldable to a sequence (IEnumerable) performing a map operation
        /// on each item in the structure
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to performt the operation on</param>
        /// <returns>Sequence of As that represent the value(s) in the structure</returns>
        [Pure]
        public static IEnumerable<B> Collect<FOLD, F, A, B>(this F self, Func<A, B> f) where FOLD : struct, Foldable<F, A> =>
            default(FOLD).FoldBack(self, new B[0].AsEnumerable(), (s, x) => f(x).Cons(s));

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable</returns>
        [Pure]
        public static A Head<FOLD, F, A>(this F fa) where FOLD : struct, Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa).Head();

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<A> HeadOrNone<FOLD, F, A>(this F fa) where FOLD : struct, Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa).HeadOrNone();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable</returns>
        [Pure]
        public static A Last<FOLD, F, A>(this F fa) where FOLD : struct, Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa).Last();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<A> LastOrNone<FOLD, F, A>(this F fa) where FOLD : struct, Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa)
                .Map(x => Option<A>.Some(x))
                .DefaultIfEmpty(Option<A>.None)
                .LastOrDefault();

        /// <summary>
        /// Tests whether the foldable structure is empty
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static bool IsEmpty<FOLD, F, A>(this F fa) where FOLD : struct, Foldable<F, A> =>
            !toSeq<FOLD, F, A>(fa).Any();

        /// <summary>
        /// Does the element occur in the structure?
        /// </summary>
        /// <typeparam name="EQ">Eq<A> type-class</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="item">Item to test</param>
        /// <returns>True if item in the structure</returns>
        [Pure]
        public static bool Elem<EQ, FOLD, F, A>(this F fa, A item)
            where EQ : struct, Eq<A>
            where FOLD : struct, Foldable<F, A>
        {
            foreach (var x in toSeq<FOLD, F, A>(fa))
            {
                if (equals<EQ, A>(x, item)) return true;
            }
            return false;
        }

        /// <summary>
        /// The 'sum' function computes the sum of the numbers of a structure.
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Sum of the numbers in the structure</returns>
        [Pure]
        public static A Sum<NUM, FOLD, F, A>(this F fa)
            where FOLD : struct, Foldable<F, A>
            where NUM  : struct, Num<A> =>
                default(FOLD).Fold(fa, fromInteger<NUM, A>(0), (s, x) => plus<NUM, A>(s, x));

        /// <summary>
        /// The 'product' function computes the product of the numbers of a structure.
        /// </summary>
        /// <typeparam name="NUM">Foldable && NUM type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Product of the numbers in the structure</returns>
        [Pure]
        public static A Product<NUM, FOLD, F, A>(this F fa)
            where FOLD : struct, Foldable<F, A>
            where NUM : struct, Num<A> =>
                default(FOLD).Fold(fa, fromInteger<NUM, A>(0), (s, x) => product<NUM, A>(s, x));

        /// <summary>
        /// Runs a predicate against the bound value(s).  If the predicate
        /// holds for all values then true is returned.  
        /// 
        /// NOTE: An empty structure will return true.
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>True if the predicate holds for all values</returns>
        [Pure]
        public static bool ForAll<FOLD, F, A>(this F fa, Func<A, bool> pred) where FOLD : struct, Foldable<F, A>
        {
            foreach (var item in toSeq<FOLD, F, A>(fa))
            {
                if (!pred(item)) return false;
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
        public static bool Exists<FOLD, F, A>(this F fa, Func<A, bool> pred) where FOLD : struct, Foldable<F, A>
        {
            foreach (var item in toSeq<FOLD, F, A>(fa))
            {
                if (pred(item)) return true;
            }
            return false;
        }
    }
}
