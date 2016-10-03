using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Convert the foldable to a sequence (IEnumerable)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to perform the operation on</param>
        /// <returns>Sequence of As that represent the value(s) in the structure</returns>
        [Pure]
        public static IEnumerable<A> ToSeq<A>(this Foldable<A> self) =>
            TypeClass.toSeq(self);

        /// <summary>
        /// Iterate the values in the foldable
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to perform the operation on</param>
        public static Unit Iter<A>(this Foldable<A> self, Action<A> action) =>
            TypeClass.iter(self, action);

        /// <summary>
        /// Convert the foldable to a sequence (IEnumerable) performing a map operation
        /// on each item in the structure
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to performt the operation on</param>
        /// <returns>Sequence of As that represent the value(s) in the structure</returns>
        [Pure]
        public static IEnumerable<B> Collect<A, B>(this Foldable<A> self, Func<A, B> f) =>
            TypeClass.collect(self,f);

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
        public static S Fold<A, S>(this Foldable<A> fa, S state, Func<S, A, S> f) =>
            TypeClass.fold(fa, state, f);

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
        public static S FoldBack<A, S>(this Foldable<A> fa, S state, Func<S, A, S> f) =>
            TypeClass.foldBack(fa, state, f);

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable</returns>
        [Pure]
        public static A Head<A>(this Foldable<A> fa) =>
            TypeClass.head(fa);

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<A> HeadOrNone<A>(this Foldable<A> fa) =>
            TypeClass.headOrNone(fa);

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable</returns>
        [Pure]
        public static A Last<A>(this Foldable<A> fa) =>
            TypeClass.last(fa);

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<A> LastOrNone<A>(this Foldable<A> fa) =>
            TypeClass.lastOrNone(fa);

        /// <summary>
        /// Tests whether the foldable structure is empty
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static bool IsEmpty<A>(this Foldable<A> fa) =>
            TypeClass.isEmpty(fa);

        /// <summary>
        /// Find the length of a foldable structure 
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static int Count<A>(this Foldable<A> fa) =>
            TypeClass.count(fa);

        /// <summary>
        /// Does the element occur in the structure?
        /// </summary>
        /// <typeparam name="EQ">Eq<A> type-class</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="item">Item to test</param>
        /// <returns>True if item in the structure</returns>
        [Pure]
        public static bool Elem<EQ, A>(this Foldable<A> fa, A item) where EQ : struct, Eq<A> =>
            TypeClass.elem<EQ, A>(fa, item);

        /// <summary>
        /// The 'sum' function computes the sum of the numbers of a structure.
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Sum of the numbers in the structure</returns>
        [Pure]
        public static A Sum<NUM, A>(this Foldable<A> fa) where NUM : struct, Num<A> =>
            TypeClass.sum<NUM, A>(fa);

        /// <summary>
        /// The 'product' function computes the product of the numbers of a structure.
        /// </summary>
        /// <typeparam name="NUM">NUM type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Product of the numbers in the structure</returns>
        [Pure]
        public static A Product<NUM, A>(this Foldable<A> fa)
            where NUM : struct, Num<A> =>
            TypeClass.product<NUM, A>(fa);

        /// <summary>
        /// Runs a predicate against the bound value(s).  If the predicate
        /// holds for all values then true is returned.  
        /// 
        /// NOTE: An empty structure will return true.
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>True if the predicate holds for all values</returns>
        [Pure]
        public static bool ForAll<A>(this Foldable<A> fa, Func<A, bool> pred) =>
            TypeClass.forall(fa, pred);

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
        public static bool Exists<A>(this Foldable<A> fa, Func<A, bool> pred) =>
            TypeClass.exists(fa, pred);
    }
}
