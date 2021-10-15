#nullable enable
using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// The relations two vector clocks may find themselves in.
    /// </summary>
    public enum Relation
    {
        Causes,
        CausedBy,
        Concurrent
    }

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

    /// <summary>
    /// A vector clock is, conceptually, an associative list sorted by the
    /// value of the key, where each key appears only once.
    /// </summary>
    public record VectorClock<A>(Seq<(A, long)> Entries) 
        where A : IComparable<A>
    {
        public static readonly VectorClock<A> Empty = new(Seq<(A, long)>());

        public virtual bool Equals(VectorClock<A>? rhs) =>
            rhs is not null &&
            GetHashCode() == rhs.GetHashCode() &&
            Count == rhs.Count &&
            Entries.Zip(rhs.Entries).ForAll(p => TypeClass.equals<OrdDefault<A>, A>(p.Left.Item1, p.Right.Item1)) &&
            Entries.Zip(rhs.Entries).ForAll(p => p.Left.Item2 == p.Right.Item2);

        public override int GetHashCode() =>
            Entries.GetHashCode();

        /// <summary>
        /// A vector clock with a single element
        /// </summary>
        public static VectorClock<A> Single(A x, long y) =>
            fromList(Seq1((x, y)));

        /// <summary>
        /// Insert each entry in the list one at a time.
        /// </summary>
        public static VectorClock<A> fromList(Seq<(A x, long y)> list) =>
            list.FoldBack(Empty, (vc, pair) => vc.Insert(pair.x, pair.y));

        /// <summary>
        /// All the entries in the vector clock.  Note that this is /not/ the inverse of 'fromList'
        /// </summary>
        public Seq<(A x, long y)> ToSeq() =>
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
        public (Option<long> Value, VectorClock<A> Clock) Extract(A index)
        {
            Option<long> value = default;
            return (value, new VectorClock<A>(go(Entries).ToSeq()));

            IEnumerable<(A x, long y)> go(Seq<(A x, long y)> zs)
            {
                foreach (var z in zs)
                {
                    if (default(OrdDefault<A>).Equals(z.x, index))
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
        public Option<long> Lookup(A index)
        {
            foreach (var z in Entries)
            {
                if (default(OrdDefault<A>).Equals(z.Item1, index)) return z.Item2;
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
        public VectorClock<A> Remove(A index) =>
            new VectorClock<A>(Entries.Filter(e => !default(OrdDefault<A>).Equals(e.Item1, index)).ToSeq());

        /// <summary>
        /// Insert or replace the entry for a key.
        /// </summary>
        public VectorClock<A> Insert(A index, long value)
        {
            return new VectorClock<A>(go(Entries));

            Seq<(A, long)> go(Seq<(A, long)> entries) =>
                entries.IsEmpty
                    ? Seq1((index, value))
                    : entries.Head switch
                      {
                          (var x1, _) xy when default(OrdDefault<A>).Compare(x1, index) < 0 => xy.Cons(go(entries.Tail)),
                          (var x1, _) xy when default(OrdDefault<A>).Equals(x1, index)      => (index, value).Cons(entries.Tail),
                          var xy                                                            => (index, value).Cons(xy.Cons(entries.Tail)),
                      };
        }

        /// <summary>
        /// Increment the entry for a key by 1
        /// </summary>
        public Option<VectorClock<A>> Inc(A index) =>
            Lookup(index).Map(y => Insert(index, y + 1));

        /// <summary>
        /// Increment the entry for a key by 1
        /// </summary>
        public VectorClock<A> Inc(A index, long defaultValue) =>
            Lookup(index).Case switch
            {
                long y => Insert(index, y + 1),
                _      => Insert(index, defaultValue + 1)
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
        public static VectorClock<A> combine(
            Func<A, Option<long>, Option<long>, Option<long>> f,
            VectorClock<A> vc1,
            VectorClock<A> vc2)
        {
            return new VectorClock<A>(go(vc1.Entries, vc2.Entries).Somes());
            
            Seq<Option<(A, long)>> go(Seq<(A, long)> es1, Seq<(A, long)> es2) =>
                (es1.IsEmpty, es2.IsEmpty) switch
                {
                    (true,  _) => es2.Map(xy => mk(xy.Item1, f(xy.Item1, None, Some(xy.Item2)))),
                    (_,  true) => es1.Map(xy => mk(xy.Item1, f(xy.Item1, Some(xy.Item2), None))),
                    _          => default(OrdDefault<A>).Compare(es1.Head.Item1, es2.Head.Item1) switch
                                  {
                                      var c when c < 0  => mk(es1.Head.Item1, f(es1.Head.Item1, Some(es1.Head.Item2), None)).Cons(go(es1.Tail, es2)),
                                      var c when c == 0 => mk(es1.Head.Item1, f(es1.Head.Item1, Some(es1.Head.Item2), Some(es2.Head.Item2))).Cons(go(es1.Tail, es2.Tail)),
                                      _                 => mk(es2.Head.Item1, f(es2.Head.Item1, None, Some(es2.Head.Item2))).Cons(go(es1, es2.Tail)),
                                  }
                };

            static Option<(A, long)> mk(A x, Option<long> v) =>
                v.Map(y => (x, y)); 
        }

        /// <summary>
        /// The maximum of the two vector clocks.
        /// </summary>
        public VectorClock<A> Max(VectorClock<A> vc2) =>
            max(this, vc2);

        /// <summary>
        /// The maximum of the two vector clocks.
        /// </summary>
        public static VectorClock<A> max(VectorClock<A> vc1, VectorClock<A> vc2)
        {
            return combine(maxEntry, vc1, vc2);

            static Option<long> maxEntry(A _, Option<long> ea, Option<long> eb) =>
                (ea.Case, eb.Case) switch
                {
                    (null, null)     => None,
                    (long x, null)   => Some(x),
                    (null, long y)   => Some(y),
                    (long x, long y) => Some(Math.Max(x, y)),
                    _                => None
                };
        }

        /// <summary>
        /// The relation between the two vector clocks.
        /// </summary>
        public Relation Relation(VectorClock<A> vc2) =>
            relation(this, vc2);

        /// <summary>
        /// The relation between the two vector clocks.
        /// </summary>
        public static Relation relation(VectorClock<A> vc1, VectorClock<A> vc2)
        {
            return go(vc1.Entries, vc2.Entries);

            static Relation go(Seq<(A, long)> es1, Seq<(A, long)> es2) =>
                (es1.IsEmpty, es2.IsEmpty) switch
                {
                    (false, false) => TypeClass.equals<OrdDefault<A>, A>(es1.Head.Item1, es2.Head.Item1)
                                          ? es1.Head.Item2 == es2.Head.Item2
                                                ? go(es1.Tail, es2.Tail)
                                                : es1.Head.Item2 < es2.Head.Item2
                                                    ? checkCauses(es1.Tail, es2.Tail) ? LanguageExt.Relation.Causes : LanguageExt.Relation.Concurrent
                                                    : checkCauses(es2.Tail, es1.Tail)
                                                        ? LanguageExt.Relation.CausedBy
                                                        : LanguageExt.Relation.Concurrent
                                          : TypeClass.lessThan<OrdDefault<A>, A>(es1.Head.Item1, es2.Head.Item1)
                                              ? checkCauses(es2, es1.Tail) ? LanguageExt.Relation.CausedBy : LanguageExt.Relation.Concurrent
                                              : TypeClass.greaterThan<OrdDefault<A>, A>(es1.Head.Item1, es2.Head.Item1)
                                                  ? checkCauses(es1, es2.Tail) ? LanguageExt.Relation.Causes : LanguageExt.Relation.Concurrent
                                                  : LanguageExt.Relation.Concurrent,
                    (true, _) => LanguageExt.Relation.Causes,
                    (_, true) => LanguageExt.Relation.CausedBy,
                };

            static bool checkCauses(Seq<(A, long)> es1, Seq<(A, long)> es2) =>
                (es1.IsEmpty, es2.IsEmpty) switch
                {
                    (false, false) => TypeClass.equals<OrdDefault<A>, A>(es1.Head.Item1, es2.Head.Item1)
                                          ? es1.Head.Item2 <= es2.Head.Item2 && checkCauses(es1.Tail, es2.Tail)
                                          : !TypeClass.lessThan<OrdDefault<A>, A>(es1.Head.Item1, es2.Head.Item1) && checkCauses(es1, es2.Tail),
                    (true, _) => true,
                    _         => false
                };
        }

        /// <summary>
        /// Short-hand for relation(vc1, vc2) == Relation.Causes
        /// </summary>
        public bool Causes(VectorClock<A> vc2) =>
            causes(this, vc2);

        /// <summary>
        /// Short-hand for relation(vc1, vc2) == Relation.Causes
        /// </summary>
        public static bool causes(VectorClock<A> vc1, VectorClock<A> vc2) =>
            relation(vc1, vc2) == LanguageExt.Relation.Causes;

        /// <summary>
        /// If vc2 causes vc1, compute the smallest vc3
        /// </summary>
        /// <remarks>Note that the /first/ parameter is the newer vector clock.</remarks>
        public Option<VectorClock<A>> Diff(VectorClock<A> vc2) =>
            diff(this, vc2);

        /// <summary>
        /// If vc2 causes vc1, compute the smallest vc3
        /// </summary>
        /// <remarks>Note that the /first/ parameter is the newer vector clock.</remarks>
        public static Option<VectorClock<A>> diff(VectorClock<A> vc1, VectorClock<A> vc2)
        {
            return vc1 == vc2
                       ? Some(Empty)
                       : causes(vc2, vc1)
                           ? Some(combine(diffOne, vc1, vc2))
                           : None;

            static Option<long> diffOne(A _, Option<long> ox, Option<long> oy) =>
                (ox.Case, oy.Case) switch
                {
                    (null, null)     => None,
                    (long x, null)   => Some(x),
                    (null, long y)   => throw new InvalidOperationException("diff broken"),
                    (long x, long y) => x == y ? None : Some(x),
                    _                => None
                };
        }

        bool? valid;
        public bool Valid 
        { 
            get
            {
                if (valid.HasValue) return valid.Value;
                var keys     = Entries.Map(e => e.Item1);
                var sorted   = keys.Sort<OrdDefault<A>, A>() == keys;
                var distinct = Entries.Distinct<EqTuple<OrdDefault<A>, TLong, A, long>, (A, long)>().Count == Entries.Count;
                valid = sorted && distinct;
                return valid.Value;
            }
        }
    }
    
    /// <summary>
    /// A vector clock is, conceptually, an associative list sorted by the
    /// value of the key, where each key appears only once.
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
            Entries.Zip(rhs.Entries).ForAll(p => TypeClass.equals<OrdA, A>(p.Left.Item1, p.Right.Item1)) &&
            Entries.Zip(rhs.Entries).ForAll(p => TypeClass.equals<NumB, B>(p.Left.Item2, p.Right.Item2));

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
            list.FoldBack(Empty, (vc, pair) => vc.Insert(pair.x, pair.y));

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
                    if (default(OrdA).Equals(z.x, index))
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
                if (default(OrdA).Equals(z.Item1, index)) return z.Item2;
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
                          (var x1, _) xy when default(OrdA).Compare(x1, index) < 0 => xy.Cons(go(entries.Tail)),
                          (var x1, _) xy when default(OrdA).Equals(x1, index)      => (index, value).Cons(entries.Tail),
                          var xy                                                   => (index, value).Cons(xy.Cons(entries.Tail)),
                      };
        }

        /// <summary>
        /// Increment the entry for a key by 1
        /// </summary>
        public Option<VectorClock<OrdA, NumB, A, B>> Inc(A index) =>
            Lookup(index).Map(y => Insert(index, default(NumB).Plus(y, default(NumB).FromInteger(1))));

        /// <summary>
        /// Increment the entry for a key by 1
        /// </summary>
        public VectorClock<OrdA, NumB, A, B> Inc(A index, B defaultValue) =>
            Lookup(index).Case switch
            {
                B y => Insert(index, default(NumB).Plus(y, default(NumB).FromInteger(1))),
                _   => Insert(index, default(NumB).Plus(defaultValue, default(NumB).FromInteger(1))),
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
                    _          => default(OrdA).Compare(es1.Head.Item1, es2.Head.Item1) switch
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
                    (B x, B y)   => Some(TypeClass.max<NumB, B>(x, y)),
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
                var distinct = Entries.Distinct<EqTuple<OrdA, NumB, A, B>, (A, B)>().Count == Entries.Count;
                valid = sorted && distinct;
                return valid.Value;
            }
        }
    }
}
#nullable disable
