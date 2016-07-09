using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt.TypeClass
{
    public static partial class Prelude
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
        public static S fold<FOLD, A, S>(Foldable<A> fa, S state, Func<S, A, S> f) where FOLD : struct, Foldable<A> =>
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
        public static S foldBack<FOLD, A, S>(Foldable<A> fa, S state, Func<S, A, S> f) where FOLD : struct, Foldable<A> =>
            default(FOLD).FoldBack(fa, state, f);

        /// <summary>
        /// Turn any foldable into a sequence
        /// </summary>
        /// <typeparam name="FOLD">Foldable type</typeparam>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Sequence of As</returns>
        public static IEnumerable<A> seq<FOLD, A>(Foldable<A> fa) where FOLD : struct, Foldable<A> =>
            fold<FOLD, A, IEnumerable<A>>(fa, new A[0], (s, x) => x.Cons(s));

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="FOLD">Foldable type</typeparam>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable</returns>
        public static A head<FOLD, A>(Foldable<A> fa) where FOLD : struct, Foldable<A> =>
            seq<FOLD, A>(fa).Head();

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="FOLD">Foldable type</typeparam>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable (Or None if no items produced)</returns>
        public static Option<A> headOrNone<FOLD, A>(Foldable<A> fa) where FOLD : struct, Foldable<A> =>
            seq<FOLD, A>(fa).HeadOrNone();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="FOLD">Foldable type</typeparam>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable</returns>
        public static A last<FOLD, A>(Foldable<A> fa) where FOLD : struct, Foldable<A> =>
            seq<FOLD, A>(fa).Last();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="FOLD">Foldable type</typeparam>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable (Or None if no items produced)</returns>
        public static Option<A> lastOrNone<FOLD, A>(Foldable<A> fa) where FOLD : struct, Foldable<A> =>
            seq<FOLD, A>(fa)
                .Map(x => Some(x))
                .DefaultIfEmpty(Option<A>.None)
                .LastOrDefault();

        /// <summary>
        /// Tests whether the foldable structure is empty
        /// </summary>
        /// <typeparam name="FOLD">Foldable type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        public static bool isEmpty<FOLD, A>(Foldable<A> fa) where FOLD : struct, Foldable<A> =>
            fold<FOLD, A, bool>(fa, true, (_, __) => false);

        /// <summary>
        /// Find the length of a foldable structure 
        /// </summary>
        /// <typeparam name="FOLD">Foldable type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        public static int length<FOLD, A>(Foldable<A> fa) where FOLD : struct, Foldable<A> =>
            fold<FOLD, A, int>(fa, 0, (s, _) => s + 1);

        /// <summary>
        /// Does the element occur in the structure?
        /// </summary>
        /// <remarks>This won't early out when the element is found</remarks>
        /// <typeparam name="FOLDEQ">Foldable && EQ type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="item">Item to test</param>
        /// <returns>True if item in the structure</returns>
        public static bool elem<FOLDEQ, A>(Foldable<A> fa, A item)
            where FOLDEQ : struct, Foldable<A>, Eq<A> =>
                fold<FOLDEQ, A, bool>(fa, false, (s, x) => s || equals<FOLDEQ, A>(x, item));

        /// <summary>
        /// The 'sum' function computes the sum of the numbers of a structure.
        /// </summary>
        /// <typeparam name="FOLDNUM">Foldable && NUM type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Sum of the numbers in the structure</returns>
        public static A sum<FOLDNUM, A>(Foldable<A> fa)
            where FOLDNUM : struct, Foldable<A>, Num<A> =>
                fold<FOLDNUM, A, A>(fa, fromInteger<FOLDNUM, A>(0), (s, x) => add<FOLDNUM, A>(s, x));

        /// <summary>
        /// The 'product' function computes the product of the numbers of a structure.
        /// </summary>
        /// <typeparam name="FOLDNUM">Foldable && NUM type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Product of the numbers in the structure</returns>
        public static A product<FOLDNUM, A>(Foldable<A> fa)
            where FOLDNUM : struct, Foldable<A>, Num<A> =>
                fold<FOLDNUM, A, A>(fa, fromInteger<FOLDNUM, A>(0), (s, x) => product<FOLDNUM, A>(s, x));
    }
}
