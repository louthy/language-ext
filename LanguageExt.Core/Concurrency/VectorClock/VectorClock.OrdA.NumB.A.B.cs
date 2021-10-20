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
    public record VectorClock<OrdA, NumB, A, B>(Seq<(A, B)> Entries)
        where OrdA : struct, Ord<A>
        where NumB : struct, Num<B>
    {
        /// <summary>
        /// Empty vector clock
        /// </summary>
        public static readonly VectorClock<OrdA, NumB, A, B> Empty = new(Seq<(A, B)>());

        public virtual bool Equals(VectorClock<OrdA, NumB, A, B>? rhs) =>
            rhs is not null &&
            GetHashCode() == rhs.GetHashCode() &&
            Count == rhs.Count &&
            Entries.Zip(rhs.Entries).ForAll(p => equals<OrdA, A>(p.Left.Item1, p.Right.Item1)) &&
            Entries.Zip(rhs.Entries).ForAll(p => equals<NumB, B>(p.Left.Item2, p.Right.Item2));

        public override int GetHashCode() =>
            Entries.GetHashCode();

        /// <summary>
        /// A vector clock with a single element
        /// </summary>
        public static VectorClock<OrdA, NumB, A, B> Single(A x, B y) =>
            fromList(Seq1((x, y)));

        /// <summary>
        /// Insert each entry in the list one at a time.
        /// </summary>
        public static VectorClock<OrdA, NumB, A, B> fromList(Seq<(A x, B y)> list) =>
            list.Fold(Empty, (vc, pair) => vc.Insert(pair.x, pair.y));

        /// <summary>
        /// All the entries in the vector clock.  Note that this is /not/ the inverse of 'fromList'
        /// </summary>
        public Seq<(A x, B y)> ToList() =>
            Entries;

        /// <summary>
        /// Is the vector clock empty?
        /// </summary>
        public bool IsEmpty =>
            Entries.IsEmpty;

        /// <summary>
        /// The number of entries in the vector clock.
        /// </summary>
        public int Count =>
            Entries.Count;

        /// <summary>
        /// Lookup the value for a key in the vector clock and remove the corresponding entry
        /// </summary>
        public (Option<B> Value, VectorClock<OrdA, NumB, A, B> Clock) Extract(A index)
        {
            Option<B> value = default;
            return (value, new VectorClock<OrdA, NumB, A, B>(go(Entries).ToSeq()));

            IEnumerable<(A x, B y)> go(Seq<(A x, B y)> zs)
            {
                foreach (var z in zs)
                {
                    if (equals<OrdA, A>(z.x, index))
                    {
                        value = z.y;
                    }
                    else
                    {
                        yield return z;
                    }
                }
            }
        }

        /// <summary>
        /// Lookup the value for a key in the vector clock.
        /// </summary>
        public Option<B> Lookup(A index)
        {
            foreach (var z in Entries)
            {
                if (equals<OrdA, A>(z.Item1, index)) return z.Item2;
            }

            return None;
        }

        /// <summary>
        /// Is a member?
        /// </summary>
        public bool Contains(A index) =>
            Lookup(index).IsSome;

        /// <summary>
        /// Delete
        /// </summary>
        public VectorClock<OrdA, NumB, A, B> Remove(A index) =>
            new VectorClock<OrdA, NumB, A, B>(Entries.Filter(e => !default(OrdA).Equals(e.Item1, index)).ToSeq());


        /// <summary>
        /// Insert or replace the entry for a key.
        /// </summary>
        public VectorClock<OrdA, NumB, A, B> Insert(A index, B value)
        {
            return new VectorClock<OrdA, NumB, A, B>(go(Entries));

            Seq<(A, B)> go(Seq<(A, B)> entries) =>
                entries.IsEmpty
                    ? Seq1((index, value))
                    : entries.Head switch
                      {
                          (var x1, _) xy when lessThan<OrdA, A>(x1, index) => xy.Cons(go(entries.Tail)),
                          (var x1, _) xy when equals<OrdA, A>(x1, index)   => (index, value).Cons(entries.Tail),
                          var xy                                           => (index, value).Cons(xy.Cons(entries.Tail)),
                      };
        }

        /// <summary>
        /// Increment the entry for a key by 1
        /// </summary>
        public Option<VectorClock<OrdA, NumB, A, B>> Inc(A index) =>
            Lookup(index).Map(y => Insert(index, plus<NumB, B>(y, fromInteger<NumB, B>(1))));

        /// <summary>
        /// Increment the entry for a key by 1
        /// </summary>
        public VectorClock<OrdA, NumB, A, B> Inc(A index, B defaultValue) =>
            Lookup(index).Case switch
            {
                B y => Insert(index, plus<NumB, B>(y, fromInteger<NumB, B>(1))),
                _   => Insert(index, plus<NumB, B>(defaultValue, fromInteger<NumB, B>(1))),
            };

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
        public static VectorClock<OrdA, NumB, A, B> combine(
            Func<A, Option<B>, Option<B>, Option<B>> f,
            VectorClock<OrdA, NumB, A, B> vc1,
            VectorClock<OrdA, NumB, A, B> vc2)
        {
            return new VectorClock<OrdA, NumB, A, B>(go(vc1.Entries, vc2.Entries).Somes());
            
            Seq<Option<(A, B)>> go(Seq<(A, B)> es1, Seq<(A, B)> es2) =>
                (es1.IsEmpty, es2.IsEmpty) switch
                {
                    (true,  _) => es2.Map(xy => mk(xy.Item1, f(xy.Item1, None, Some(xy.Item2)))),
                    (_,  true) => es1.Map(xy => mk(xy.Item1, f(xy.Item1, Some(xy.Item2), None))),
                    _          => compare<OrdA, A>(es1.Head.Item1, es2.Head.Item1) switch
                                  {
                                      var c when c < 0  => mk(es1.Head.Item1, f(es1.Head.Item1, Some(es1.Head.Item2), None)).Cons(go(es1.Tail, es2)),
                                      var c when c == 0 => mk(es1.Head.Item1, f(es1.Head.Item1, Some(es1.Head.Item2), Some(es2.Head.Item2))).Cons(go(es1.Tail, es2.Tail)),
                                      _                 => mk(es2.Head.Item1, f(es2.Head.Item1, None, Some(es2.Head.Item2))).Cons(go(es1, es2.Tail)),
                                  }
                };

            static Option<(A, B)> mk(A x, Option<B> v) =>
                v.Map(y => (x, y)); 
        }

        /// <summary>
        /// The maximum of the two vector clocks.
        /// </summary>
        public static VectorClock<OrdA, NumB, A, B> max(VectorClock<OrdA, NumB, A, B> vc1, VectorClock<OrdA, NumB, A, B> vc2)
        {
            return combine(maxEntry, vc1, vc2);

            static Option<B> maxEntry(A _, Option<B> ea, Option<B> eb) =>
                (ea.Case, eb.Case) switch
                {
                    (null, null) => None,
                    (B x, null)  => Some(x),
                    (null, B y)  => Some(y),
                    (B x, B y)   => Some(max<NumB, B>(x, y)),
                    _            => None
                };
        }

        /// <summary>
        /// The relation between the two vector clocks.
        /// </summary>
        public Relation Relation(VectorClock<OrdA, NumB, A, B> vc1, VectorClock<OrdA, NumB, A, B> vc2) =>
            relation(this, vc2);

        /// <summary>
        /// The relation between the two vector clocks.
        /// </summary>
        public static Relation relation(VectorClock<OrdA, NumB, A, B> vc1, VectorClock<OrdA, NumB, A, B> vc2)
        {
            return go(vc1.Entries, vc2.Entries);

            static Relation go(Seq<(A, B)> es1, Seq<(A, B)> es2) =>
                (es1.IsEmpty, es2.IsEmpty) switch
                {
                    (false, false) => TypeClass.equals<OrdA, A>(es1.Head.Item1, es2.Head.Item1)
                                          ? TypeClass.equals<NumB, B>(es1.Head.Item2, es2.Head.Item2)
                                                ? go(es1.Tail, es2.Tail)
                                                : TypeClass.lessThan<NumB, B>(es1.Head.Item2, es2.Head.Item2)
                                                    ? checkCauses(es1.Tail, es2.Tail) ? LanguageExt.Relation.Causes : LanguageExt.Relation.Concurrent
                                                    : checkCauses(es2.Tail, es1.Tail)
                                                        ? LanguageExt.Relation.CausedBy
                                                        : LanguageExt.Relation.Concurrent
                                          : TypeClass.lessThan<OrdA, A>(es1.Head.Item1, es2.Head.Item1)
                                              ? checkCauses(es2, es1.Tail) ? LanguageExt.Relation.CausedBy : LanguageExt.Relation.Concurrent
                                              : TypeClass.greaterThan<OrdA, A>(es1.Head.Item1, es2.Head.Item1)
                                                  ? checkCauses(es1, es2.Tail) ? LanguageExt.Relation.Causes : LanguageExt.Relation.Concurrent
                                                  : LanguageExt.Relation.Concurrent,
                    (true, _) => LanguageExt.Relation.Causes,
                    (_, true) => LanguageExt.Relation.CausedBy,
                };

            static bool checkCauses(Seq<(A, B)> es1, Seq<(A, B)> es2) =>
                (es1.IsEmpty, es2.IsEmpty) switch
                {
                    (false, false) => TypeClass.equals<OrdA, A>(es1.Head.Item1, es2.Head.Item1)
                                          ? TypeClass.lessOrEq<NumB, B>(es1.Head.Item2, es2.Head.Item2) && checkCauses(es1.Tail, es2.Tail)
                                          : !TypeClass.lessThan<OrdA, A>(es1.Head.Item1, es2.Head.Item1) && checkCauses(es1, es2.Tail),
                    (true, _) => true,
                    _         => false
                };
        }

        /// <summary>
        /// Short-hand for relation(vc1, vc2) == Relation.Causes
        /// </summary>
        public bool Causes(VectorClock<OrdA, NumB, A, B> vc2) =>
            causes(this, vc2);

        /// <summary>
        /// Short-hand for relation(vc1, vc2) == Relation.Causes
        /// </summary>
        public static bool causes(VectorClock<OrdA, NumB, A, B> vc1, VectorClock<OrdA, NumB, A, B> vc2) =>
            relation(vc1, vc2) == LanguageExt.Relation.Causes;

        /// <summary>
        /// If vc2 causes vc1, compute the smallest vc3
        /// </summary>
        /// <remarks>Note that the /first/ parameter is the newer vector clock.</remarks>
        public Option<VectorClock<OrdA, NumB, A, B>> Diff(VectorClock<OrdA, NumB, A, B> vc2) =>
            diff(this, vc2);
        
        /// <summary>
        /// If vc2 causes vc1, compute the smallest vc3
        /// </summary>
        /// <remarks>Note that the /first/ parameter is the newer vector clock.</remarks>
        public static Option<VectorClock<OrdA, NumB, A, B>> diff(VectorClock<OrdA, NumB, A, B> vc1, VectorClock<OrdA, NumB, A, B> vc2)
        {
            return vc1 == vc2
                       ? Some(Empty)
                       : causes(vc2, vc1)
                           ? Some(combine(diffOne, vc1, vc2))
                           : None;

            static Option<B> diffOne(A _, Option<B> ox, Option<B> oy) =>
                (ox.Case, oy.Case) switch
                {
                    (null, null) => None,
                    (B x, null)  => Some(x),
                    (null, B y)  => throw new InvalidOperationException("diff broken"),
                    (B x, B y)   => TypeClass.equals<NumB, B>(x, y) ? None : Some(x),
                    _            => None
                };
        }

        bool? valid;
        public bool Valid 
        { 
            get
            {
                if (valid.HasValue) return valid.Value;
                var keys     = Entries.Map(e => e.Item1);
                var sorted   = keys.Sort<OrdA, A>() == keys;
                var distinct = Entries.Distinct<EqTuple2<OrdA, NumB, A, B>, (A, B)>().Count == Entries.Count;
                valid = sorted && distinct;
                return valid.Value;
            }
        }
    }
}
#nullable disable
