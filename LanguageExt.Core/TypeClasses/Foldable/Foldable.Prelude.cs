using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

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
        public static S fold<FOLD, F, A, S>(F fa, S state, Func<S, A, S> f) where FOLD : struct, Foldable<F, A> =>
            default(FOLD).Fold(fa, state, f)(unit);

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
        public static S foldBack<FOLD, F, A, S>(F fa, S state, Func<S, A, S> f) where FOLD : struct, Foldable<F, A> =>
            default(FOLD).FoldBack(fa, state, f)(unit);

        /// <summary>
        /// Iterate the values in the foldable
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to perform the operation on</param>
        public static Unit iter<FOLD, F, A>(F fa, Action<A> action) where FOLD : struct, Foldable<F, A>
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
        public static Seq<A> toSeq<FOLD, F, A>(F fa) where FOLD : struct, Foldable<F, A> =>
            default(FOLD).FoldBack(fa, Seq<A>.Empty, (s, x) => x.Cons(s))(unit);


        /// <summary>
        /// Convert the foldable to a sequence (IEnumerable) performing a map operation
        /// on each item in the structure
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to performt the operation on</param>
        /// <returns>Sequence of As that represent the value(s) in the structure</returns>
        [Pure]
        public static Seq<B> collect<FOLD, F, A, B>(F self, Func<A, B> f) where FOLD : struct, Foldable<F, A> =>
            default(FOLD).FoldBack(self, Seq<B>.Empty, (s, x) => f(x).Cons(s))(unit);

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable</returns>
        [Pure]
        public static A head<FOLD, F, A>(F fa) where FOLD :struct,  Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa).Head();

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<A> headOrNone<FOLD, F, A>(F fa) where FOLD :struct,  Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa).HeadOrNone();

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="fail">Fail case</param>
        /// <returns>First A produced by the foldable (Or Fail if no items produced)</returns>
        [Pure]
        public static Validation<FAIL, A> headOrInvalid<FOLD, F, FAIL, A>(F fa, FAIL fail) where FOLD : struct, Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa).HeadOrInvalid(fail);

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="left">Left case</param>
        /// <returns>First A produced by the foldable (Or Left if no items produced)</returns>
        [Pure]
        public static Either<L, A> headOrLeft<FOLD, F, L, A>(F fa, L left) where FOLD : struct, Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa).HeadOrLeft(left);

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable</returns>
        [Pure]
        public static A last<FOLD, F, A>(F fa) where FOLD : struct, Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa).Last();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<A> lastOrNone<FOLD, F, A>(F fa) where FOLD : struct, Foldable<F, A> =>
            toSeq<FOLD, F, A>(fa)
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
        public static bool isEmpty<FOLD, F, A>(F fa) where FOLD : struct, Foldable<F, A> =>
            !toSeq<FOLD, F, A>(fa).Any();

        /// <summary>
        /// Find the length of a foldable structure 
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static int count<FOLD, F, A>(F fa) where FOLD : struct, Foldable<F, A> =>
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
        public static bool contains<EQ, FOLD, F, A>(F fa, A item)
            where EQ : struct, Eq<A>
            where FOLD : struct, Foldable<F, A>
        {
            foreach(var x in toSeq<FOLD, F, A>(fa))
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
        public static A sum<NUM, FOLD, F, A>(F fa) 
            where FOLD : struct, Foldable<F, A> 
            where NUM : struct, Num<A> =>
                default(FOLD).Fold(fa, fromInteger<NUM, A>(0), (s, x) => plus<NUM, A>(s, x))(unit);

        /// <summary>
        /// The 'product' function computes the product of the numbers of a structure.
        /// </summary>
        /// <typeparam name="NUM">Foldable && NUM type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Product of the numbers in the structure</returns>
        [Pure]
        public static A product<NUM, FOLD, F, A>(F fa)
            where FOLD : struct, Foldable<F, A>
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
        public static bool forall<FOLD, F, A>(F fa, Func<A,bool> pred) where FOLD : struct, Foldable<F, A>
        {
            foreach(var item in toSeq<FOLD, F, A>(fa))
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
        public static bool exists<FOLD, F, A>(F fa, Func<A, bool> pred) where FOLD : struct, Foldable<F, A>
        {
            foreach (var item in toSeq<FOLD, F, A>(fa))
            {
                if (pred(item)) return true;
            }
            return false;
        }
    }
}
