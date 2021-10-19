#nullable enable
using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// <para>
    /// To create a vector clock, start from `Empty` or `Single` and `Insert`
    /// elements into it.  As a shortcut, `fromList` just inserts all the
    /// elements in a list, in order.
    /// </para>
    /// <code>
    ///     var vc = VectorClock&lt;char&gt;.Empty;
    ///     vc = vc.Insert('a', 1);
    ///     vc = vc.Insert('b', 2);
    ///     vc == VectorClock&lt;char&gt;.fromList(Seq(('a', 1), ('b', 2)))
    /// </code>
    /// <para>
    /// Note that, for different keys, the order of insertion does not
    /// matter:
    /// </para>
    /// <code>
    ///     fromList(Seq(('a', 1), ('b', 2)) == fromList(Seq(('b', 2), ('a', 1))
    /// </code>
    /// <para>
    /// Once you have a given vector clock, you can 'lookup' its fields,
    /// check that keys are 'member's, or convert it back 'toList' form.
    /// </para>
    /// <code>
    ///     vc.Lookup('a') == Some(1)
    ///     vc.Lookup('c') == None
    /// </code>
    /// <para>
    /// The main operations that you would do with a vector clock are to
    /// increment the entry corresponding to the current process and to
    /// update the process's vector clock with the 'max' of its and the
    /// received message's clocks.
    /// </para>
    /// <code>
    ///     vc.Inc('a') == Some [('a', 2), ('b', 2)]
    ///     VectorClock.max( [('a', 1), ('b', 2)], [('c', 3), ('b', 1)] ) == [('a', 1), ('b', 2), ('c' 3)]
    /// </code>
    /// <para>
    /// Finally, upon receiving different messages, you may wish to
    /// discover the relationship, if any, between them.  This
    /// information could be useful in determining the correct order to
    /// process the messages.
    /// </para>
    /// <code>
    ///     VectorClock.relation (fromList [('a', 1), ('b', 2)], fromList [('a', 2), ('b', 2)]) == Causes
    ///     VectorClock.relation (fromList [('a', 2), ('b', 2)], fromList [('a', 1), ('b', 2)]) == CausedBy
    ///     VectorClock.relation (fromList [('a', 2), ('b', 2)], fromList [('a', 1), ('b', 3)]) == Concurrent
    /// </code>
    /// <para>
    /// A vector clock is, conceptually, an associative list sorted by the
    /// value of the key, where each key appears only once.
    /// </para>
    /// </summary>
    public static class VectorClock
    {
        /// <summary>
        /// A vector clock with a single element
        /// </summary>
        public static VectorClock<A> Single<A>(A x, long y) where A : IComparable<A> =>
            VectorClock<A>.Single(x, y);

        /// <summary>
        /// A vector clock with a single element
        /// </summary>
        public static VectorClock<OrdA, NumB, A, B> Single<OrdA, NumB, A, B>(A x, B y)
            where OrdA : struct, Ord<A>
            where NumB : struct, Num<B> =>
            VectorClock<OrdA, NumB, A, B>.Single(x, y);

        /// <summary>
        /// Insert each entry in the list one at a time.
        /// </summary>
        public static VectorClock<A> fromList<A>(Seq<(A x, long y)> list) where A : IComparable<A> =>
            VectorClock<A>.fromList(list);

        /// <summary>
        /// Insert each entry in the list one at a time.
        /// </summary>
        public static VectorClock<OrdA, NumB, A, B> fromList<OrdA, NumB, A, B>(Seq<(A x, B y)> list) 
            where OrdA : struct, Ord<A>
            where NumB : struct, Num<B> =>
            VectorClock<OrdA, NumB, A, B>.fromList(list);

        /// <summary>
        /// Combine two vector clocks entry-by-entry
        /// </summary>
        /// <param name="f">a function that takes the /key/, the value of the entry in
        /// the left hand vector clock, if it exists, the value in the
        /// right hand vector clock, if it exists, and, if it wishes to
        /// keep a value for this /key/ in the resulting vector clock,
        /// returns it.</param>
        /// <param name="vc1">the left hand vector clock</param>
        /// <param name="vc2">he right hand vector clock</param>
        /// <returns></returns>
        public static VectorClock<A> combine<A>(
            Func<A, Option<long>, Option<long>, Option<long>> f,
            VectorClock<A> vc1,
            VectorClock<A> vc2) where A : IComparable<A> =>
            VectorClock<A>.combine(f, vc1, vc2);

        /// <summary>
        /// Combine two vector clocks entry-by-entry
        /// </summary>
        /// <param name="f">a function that takes the /key/, the value of the entry in
        /// the left hand vector clock, if it exists, the value in the
        /// right hand vector clock, if it exists, and, if it wishes to
        /// keep a value for this /key/ in the resulting vector clock,
        /// returns it.</param>
        /// <param name="vc1">the left hand vector clock</param>
        /// <param name="vc2">he right hand vector clock</param>
        /// <returns></returns>
        public static VectorClock<OrdA, NumB, A, B> combine<OrdA, NumB, A, B>(
            Func<A, Option<B>, Option<B>, Option<B>> f,
            VectorClock<OrdA, NumB, A, B> vc1,
            VectorClock<OrdA, NumB, A, B> vc2)
            where OrdA : struct, Ord<A>
            where NumB : struct, Num<B> =>
            VectorClock<OrdA, NumB, A, B>.combine(f, vc1, vc2);

        /// <summary>
        /// The maximum of the two vector clocks.
        /// </summary>
        public static VectorClock<A> max<A>(VectorClock<A> vc1, VectorClock<A> vc2)
            where A : IComparable<A> =>
            VectorClock<A>.max(vc1, vc2);

        /// <summary>
        /// The maximum of the two vector clocks.
        /// </summary>
        public static VectorClock<OrdA, NumB, A, B> max<OrdA, NumB, A, B>(VectorClock<OrdA, NumB, A, B> vc1, VectorClock<OrdA, NumB, A, B> vc2)
            where OrdA : struct, Ord<A>
            where NumB : struct, Num<B> =>
            VectorClock<OrdA, NumB, A, B>.max(vc1, vc2);

        /// <summary>
        /// The relation between the two vector clocks.
        /// </summary>
        public static Relation relation<A>(VectorClock<A> vc1, VectorClock<A> vc2)
            where A : IComparable<A> =>
            VectorClock<A>.relation(vc1, vc2);

        /// <summary>
        /// The relation between the two vector clocks.
        /// </summary>
        public static Relation relation<OrdA, NumB, A, B>(VectorClock<OrdA, NumB, A, B> vc1, VectorClock<OrdA, NumB, A, B> vc2)
            where OrdA : struct, Ord<A>
            where NumB : struct, Num<B> =>
            VectorClock<OrdA, NumB, A, B>.relation(vc1, vc2);

        /// <summary>
        /// Short-hand for relation(vc1, vc2) == Relation.Causes
        /// </summary>
        public static bool causes<A>(VectorClock<A> vc1, VectorClock<A> vc2)
            where A : IComparable<A> =>
            VectorClock<A>.causes(vc1, vc2);

        /// <summary>
        /// Short-hand for relation(vc1, vc2) == Relation.Causes
        /// </summary>
        public static bool causes<OrdA, NumB, A, B>(VectorClock<OrdA, NumB, A, B>vc1, VectorClock<OrdA, NumB, A, B> vc2)
            where OrdA : struct, Ord<A>
            where NumB : struct, Num<B> =>
            VectorClock<OrdA, NumB, A, B>.causes(vc1, vc2);

        /// <summary>
        /// If vc2 causes vc1, compute the smallest vc3
        /// </summary>
        /// <remarks>Note that the /first/ parameter is the newer vector clock.</remarks>
        public static Option<VectorClock<A>> diff<A>(VectorClock<A> vc1, VectorClock<A> vc2)
            where A : IComparable<A> =>
            VectorClock<A>.diff(vc1, vc2);

        /// <summary>
        /// If vc2 causes vc1, compute the smallest vc3
        /// </summary>
        /// <remarks>Note that the /first/ parameter is the newer vector clock.</remarks>
        public static Option<VectorClock<OrdA, NumB, A, B>> diff<OrdA, NumB, A, B>(VectorClock<OrdA, NumB, A, B> vc1, VectorClock<OrdA, NumB, A, B> vc2)
            where OrdA : struct, Ord<A>
            where NumB : struct, Num<B> =>
            VectorClock<OrdA, NumB, A, B>.diff(vc1, vc2);
    }
}
#nullable disable
