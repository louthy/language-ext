using System;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// As an enumerable
        /// </summary>
        public static IEnumerable<int> AsEnumerable(this (int from, int count) range) =>
            Range(range.from, range.count);
        
        /// <summary>
        /// Convert to a lazy seq
        /// </summary>
        public static Seq<int> ToSeq(this (int from, int count) range) =>
            range.AsEnumerable().ToSeq();
        
        /// <summary>
        /// Functor map
        /// </summary>
        public static IEnumerable<B> Map<B>(this (int from, int count) range, Func<int, B> f) =>
            range.AsEnumerable().Map(f);

        /// <summary>
        /// Functor map
        /// </summary>
        public static IEnumerable<B> Select<B>(this (int from, int count) range, Func<int, B> f) =>
            range.AsEnumerable().Map(f);

        /// <summary>
        /// Monadic bind
        /// </summary>
        public static IEnumerable<B> Bind<B>(this (int from, int count) range, Func<int, IEnumerable<B>> f) =>
            range.AsEnumerable().Bind(f);

        /// <summary>
        /// Monadic bind
        /// </summary>
        public static IEnumerable<B> SelectMany<B>(this (int from, int count) range, Func<int, IEnumerable<B>> f) =>
            range.AsEnumerable().Bind(f);

        /// <summary>
        /// Combination of monadic bind and functor map
        /// </summary>
        public static IEnumerable<C> SelectMany<B, C>(this (int from, int count) range, Func<int, IEnumerable<B>> bind,
            Func<int, B, C> project)
        {
            foreach (var x in range.AsEnumerable())
            {
                foreach (var y in bind(x))
                {
                    yield return project(x, y);
                }
            }
        }

        /// <summary>
        /// Removes items from the sequence that return false when the predicate is applied
        /// </summary>
        public static IEnumerable<int> Filter(this (int from, int count) range, Func<int, bool> f) =>
            range.AsEnumerable().Filter(f);

        /// <summary>
        /// Removes items from the sequence that return false when the predicate is applied
        /// </summary>
        public static IEnumerable<int> Where(this (int from, int count) range, Func<int, bool> f) =>
            range.AsEnumerable().Filter(f);

        /// <summary>
        /// Creates an aggregate value from all items in the sequence, starting at the first item
        /// </summary>
        public static S Fold<S>(this (int from, int count) range, S state, Func<S, int, S> f) =>
            range.AsEnumerable().Fold(state, f);

        /// <summary>
        /// Creates an aggregate value from all items in the sequence, starting at the last item
        /// </summary>
        public static S FoldBack<S>(this (int from, int count) range, S state, Func<S, int, S> f) =>
            range.AsEnumerable().FoldBack(state, f);

        /// <summary>
        /// Does the predicate hold for any item
        /// </summary>
        public static bool Exists(this (int from, int count) range, Func<int, bool> f) =>
            range.AsEnumerable().Exists(f);

        /// <summary>
        /// Does the predicate hold for all items
        /// </summary>
        public static bool ForAll(this (int from, int count) range, Func<int, bool> f) =>
            range.AsEnumerable().ForAll(f);
    }
}
