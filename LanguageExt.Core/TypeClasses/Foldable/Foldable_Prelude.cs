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
        public static S fold<A, S>(Foldable<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(fa, state, f);

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
        public static S foldBack<A, S>(Foldable<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(fa, state, f);

        /// <summary>
        /// Iterate the values in the foldable
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to perform the operation on</param>
        public static Unit iter<A>(Foldable<A> fa, Action<A> action)
        {
            foreach (var item in toSeq(fa))
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
        public static IEnumerable<A> toSeq<A>(Foldable<A> fa) =>
            foldBack<A, IEnumerable<A>>(fa, new A[0], (s, x) => x.Cons(s));


        /// <summary>
        /// Convert the foldable to a sequence (IEnumerable) performing a map operation
        /// on each item in the structure
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to performt the operation on</param>
        /// <returns>Sequence of As that represent the value(s) in the structure</returns>
        [Pure]
        public static IEnumerable<B> collect<A, B>(Foldable<A> self, Func<A, B> f) =>
            foldBack(self, new B[0].AsEnumerable(), (s, x) => f(x).Cons(s));

        /// <summary>
        /// Convert the foldable to a sequence (IEnumerable) performing a map operation
        /// on each item in the structure
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to performt the operation on</param>
        /// <returns>Sequence of As that represent the value(s) in the structure</returns>
        [Pure]
        public static IEnumerable<C> collectT<A, B, C>(Foldable<A> self, Func<B, C> f) where A : Foldable<B> =>
            foldBack(self, new C[0].AsEnumerable(), (s, x) => collect(x, f).Concat(s));

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable</returns>
        [Pure]
        public static A head<A>(Foldable<A> fa) =>
            toSeq(fa).Head();

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<A> headOrNone<A>(Foldable<A> fa) =>
            toSeq(fa).HeadOrNone();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable</returns>
        [Pure]
        public static A last<A>(Foldable<A> fa) =>
            toSeq(fa).Last();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<A> lastOrNone<A>(Foldable<A> fa) =>
            toSeq(fa)
                .Map(x => Some(x))
                .DefaultIfEmpty(Option<A>.None)
                .LastOrDefault();

        /// <summary>
        /// Tests whether the foldable structure is empty
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static bool isEmpty<A>(Foldable<A> fa) =>
            fold(fa, true, (_, __) => false);

        /// <summary>
        /// Find the length of a foldable structure 
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static int count<A>(Foldable<A> fa) =>
            fold(fa, 0, (s, _) => s + 1);

        /// <summary>
        /// Does the element occur in the structure?
        /// </summary>
        /// <typeparam name="EQ">Eq<A> type-class</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="item">Item to test</param>
        /// <returns>True if item in the structure</returns>
        [Pure]
        public static bool elem<EQ, A>(Foldable<A> fa, A item)
            where EQ : struct, Eq<A>
        {
            foreach(var x in toSeq(fa))
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
        public static A sum<NUM, A>(Foldable<A> fa)
            where NUM : struct, Num<A> =>
                fold(fa, fromInteger<NUM, A>(0), (s, x) => add<NUM, A>(s, x));

        /// <summary>
        /// The 'product' function computes the product of the numbers of a structure.
        /// </summary>
        /// <typeparam name="NUM">Foldable && NUM type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Product of the numbers in the structure</returns>
        [Pure]
        public static A product<NUM, A>(Foldable<A> fa)
            where NUM : struct, Num<A> =>
                fold(fa, fromInteger<NUM, A>(0), (s, x) => product<NUM, A>(s, x));

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
        public static S foldT<A, B, S>(Foldable<A> fa, S state, Func<S, B, S> f) where A : Foldable<B> =>
            fa.Fold(fa, state, (s, x) => fold(x, s, f));

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
        public static S foldBackT<A, B, S>(Foldable<A> fa, S state, Func<S, B, S> f) where A : Foldable<B> =>
            fa.FoldBack(fa, state, (s, x) => foldBack(x, s, f));

        /// <summary>
        /// Iterate the values in the foldable
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="self">Foldable to perform the operation on</param>
        public static Unit iterT<A, B>(Foldable<A> fa, Action<B> action) where A : Foldable<B>
        {
            foreach (var item in toSeqT<A, B>(fa))
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
        public static IEnumerable<B> toSeqT<A, B>(Foldable<A> fa) where A : Foldable<B> =>
            fold<A, IEnumerable<B>>(fa, new B[0], (s, x) => s.Concat(toSeq<B>(x)));

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable</returns>
        [Pure]
        public static B headT<A, B>(Foldable<A> fa) where A : Foldable<B> =>
            toSeqT<A, B>(fa).Head();

        /// <summary>
        /// Get the first item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>First A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<B> headOrNoneT<A, B>(Foldable<A> fa) where A : Foldable<B> =>
            toSeqT<A, B>(fa).HeadOrNone();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable</returns>
        [Pure]
        public static B lastT<A, B>(Foldable<A> fa) where A : Foldable<B> =>
            toSeqT<A, B>(fa).Last();

        /// <summary>
        /// Get the last item in a foldable structure
        /// </summary>
        /// <typeparam name="A">Sequence item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>Last A produced by the foldable (Or None if no items produced)</returns>
        [Pure]
        public static Option<B> lastOrNoneT<A, B>(Foldable<A> fa) where A : Foldable<B> =>
            toSeqT<A, B>(fa)
                .Map(x => Some(x))
                .DefaultIfEmpty(Option<B>.None)
                .LastOrDefault();

        /// <summary>
        /// Tests whether the foldable structure is empty
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static bool isEmptyT<A, B>(Foldable<A> fa) where A : Foldable<B> =>
            foldT<A, B, bool>(fa, true, (_, __) => false);

        /// <summary>
        /// Find the length of a foldable structure 
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <returns>True if empty, False otherwise</returns>
        [Pure]
        public static int countT<A, B>(Foldable<A> fa) where A : Foldable<B> =>
            foldT<A, B, int>(fa, 0, (s, _) => s + 1);

        /// <summary>
        /// Does the element occur in the structure?
        /// </summary>
        /// <typeparam name="EQ">Eq<A> type-class</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <param name="fa">Foldable</param>
        /// <param name="item">Item to test</param>
        /// <returns>True if item in the structure</returns>
        [Pure]
        public static bool elemT<EQ, A, B>(Foldable<A> fa, B item)
            where A : Foldable<B>
            where EQ : struct, Eq<B>
        {
            foreach (var x in toSeqT<A, B>(fa))
            {
                if (equals<EQ, B>(x, item)) return true;
            }
            return false;
        }

        /// <summary>
        /// The 'sum' function computes the sum of the numbers of a structure.
        /// </summary>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Sum of the numbers in the structure</returns>
        [Pure]
        public static B sumT<NUM, A, B>(Foldable<A> fa)
            where A   : Foldable<B>
            where NUM : struct, Num<B> =>
                foldT<A, B, B>(fa, fromInteger<NUM, B>(0), (s, x) => add<NUM, B>(s, x));

        /// <summary>
        /// The 'product' function computes the product of the numbers of a structure.
        /// </summary>
        /// <typeparam name="NUM">Foldable && NUM type</typeparam>
        /// <typeparam name="A">Foldable item type</typeparam>
        /// <returns>Product of the numbers in the structure</returns>
        [Pure]
        public static B productT<NUM, A, B>(Foldable<A> fa)
            where A   : Foldable<B>
            where NUM : struct, Num<B> =>
                foldT<A, B, B>(fa, fromInteger<NUM, B>(0), (s, x) => product<NUM, B>(s, x));

        /// <summary>
        /// Runs a predicate against the bound value(s).  If the predicate
        /// holds for all values then true is returned.  
        /// 
        /// NOTE: An empty structure will return true.
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>True if the predicate holds for all values</returns>
        [Pure]
        public static bool forall<A>(Foldable<A> fa, Func<A,bool> pred)
        {
            foreach(var item in toSeq(fa))
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
        public static bool forallT<A, B>(Foldable<A> fa, Func<B, bool> pred) where A : Foldable<B> 
        {
            foreach (var item in toSeqT<A,B>(fa))
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
        public static bool exists<A>(Foldable<A> fa, Func<A, bool> pred)
        {
            foreach (var item in toSeq(fa))
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
        /// <returns>True if the predicate holds for any value</returns>
        [Pure]
        public static bool existsT<A, B>(Foldable<A> fa, Func<B, bool> pred) where A : Foldable<B>
        {
            foreach (var item in toSeqT<A,B>(fa))
            {
                if (pred(item)) return true;
            }
            return false;
        }
    }
}
