using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace LanguageExt
{
    /// <summary>
    /// A `Patch` is a collection of edits performed to a _document_, in this case an 'IEnumerable&lt;A&gt;'. 
    /// They are implemented as a list of 'Edit', and can be converted to and from raw lists of edits using 
    /// `toList` and `fromList` respectively.
    /// 
    /// Patches form a groupoid (a 'Monoid' with inverses, and a partial composition relation), where the 
    /// inverse element can be computed with 'inverse' and the groupoid operation is a _composition_ of 
    /// patches.  Applying `append(p1, p2)` is the same as applying `p1` _then_
    /// `p2` (see `Patch.apply`). This composition operator may produce structurally different patches depending 
    /// on associativity, however the patches are guaranteed to be _equivalent_ in the sense that the resultant 
    /// document will be the same when they are applied.
    /// 
    /// For convenience, we make our composition operator here total, to fit the `Monoid` typeclass, but provide 
    /// some predicates (`Patch.composable` and `Patch.applicable`) to determine if the operation can be validly 
    /// used.
    /// </summary>
    public static class Patch
    {
        /// <summary>
        /// Convert a list of edits to a patch, making sure to eliminate conflicting edits 
        /// and sorting by index.
        /// </summary>
        public static Patch<EqA, A> unsafeFromSeq<EqA, A>(Seq<Edit<EqA, A>> edits) where EqA : struct, Eq<A> =>
            new Patch<EqA, A>(edits);

        /// <summary>
        /// Convert a list of edits to a patch, making sure to eliminate conflicting edits 
        /// and sorting by index.
        /// </summary>
        public static Patch<EqA, A> fromSeq<EqA, A>(Seq<Edit<EqA, A>> edits) where EqA : struct, Eq<A> =>
            new Patch<EqA, A>(
                toSeq((from es in normalise(edits)
                       group es by es.Position into eg
                       from ed in eg.Distinct()
                       orderby ed.Position
                       select ed).ToArray()));

        /// <summary>
        /// Internal: Eliminate conflicting edits
        /// </summary>
        internal static Seq<Edit<EqA, A>> normalise<EqA, A>(Seq<Edit<EqA, A>> edits) where EqA : struct, Eq<A>
        {
            var (inss, dels, repls) = partition3(edits);
            return normalise1(inss, dels, repls);
            
            static (Seq<Edit<EqA, A>.Insert> inserts, Seq<Edit<EqA, A>.Delete> deletes, Seq<Edit<EqA, A>.Replace> replaces) partition3(Seq<Edit<EqA, A>> grp)
            {
                if (grp.IsEmpty) return (Seq<Edit<EqA, A>.Insert>(), Seq<Edit<EqA, A>.Delete>(), Seq<Edit<EqA, A>.Replace>());
                if (grp.Head is Edit<EqA, A>.Insert ins)
                {
                    var (i, d, r) = partition3(grp.Tail);
                    return (ins.Cons(i), d, r);
                }
                else if (grp.Head is Edit<EqA, A>.Delete del)
                {
                    var (i, d, r) = partition3(grp.Tail);
                    return (i, del.Cons(d), r);
                }
                else if (grp.Head is Edit<EqA, A>.Replace repl)
                {
                    var (i, d, r) = partition3(grp.Tail);
                    return (i, d, repl.Cons(r));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            static Seq<Edit<EqA, A>> normalise1(Seq<Edit<EqA, A>.Insert> inserts, Seq<Edit<EqA, A>.Delete> deletes, Seq<Edit<EqA, A>.Replace> replaces)
            {
                if (!inserts.IsEmpty && !deletes.IsEmpty) return normalise1(inserts.Tail, deletes.Tail, Edit<EqA, A>.Replace.New(deletes.Head.Position, deletes.Head.Element, inserts.Head.Element).Cons(replaces));
                if (deletes.IsEmpty) return toSeq(inserts.Map(i => i as Edit<EqA, A>).ConcatFast(replaces.Take(1)).ToArray());
                if (inserts.IsEmpty) return Seq1(deletes.Head as Edit<EqA, A>);
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Monoid append: produces a patch is a merged version of both provided
        /// patches.  
        /// </summary>
        public static Patch<EqA, A> append<EqA, A>(Patch<EqA, A> px, Patch<EqA, A> py) where EqA : struct, Eq<A> =>
            default(MPatch<EqA, A>).Append(px, py);

        /// <summary>
        /// Compute the inverse of a patch
        /// </summary>
        public static Patch<EqA, A> inverse<EqA, A>(Patch<EqA, A> patch) where EqA : struct, Eq<A>
        {
            return new Patch<EqA, A>(PatchInternal.mapAccumL(go, 0, patch.Edits).Item2);
            
            (int, Edit<EqA, A>) go(int off, Edit<EqA, A> edit) =>
                edit switch
                {
                    Edit<EqA, A>.Insert ins  => (off + 1, Edit<EqA, A>.Delete.New(off + ins.Position, ins.Element) as Edit<EqA, A>),
                    Edit<EqA, A>.Delete del  => (off - 1, Edit<EqA, A>.Insert.New(off + del.Position, del.Element) as Edit<EqA, A>),
                    Edit<EqA, A>.Replace rpl => (off, Edit<EqA, A>.Replace.New(off + rpl.Position, rpl.ReplaceElement, rpl.Element) as Edit<EqA, A>),
                    _                        => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// Returns true if a patch can be safely applied to a document, that is,
        /// `applicable(p, d)` holds when `d` is a valid source document for the patch `p`.
        /// </summary>
        public static bool applicable<EqA, A>(Patch<EqA, A> pa, IEnumerable<A> va) where EqA : struct, Eq<A>
        {
            var i = SpanArray<A>.New(va);

            return pa.Edits.ForAll(e =>
                e switch
                {
                    Edit<EqA, A>.Insert ins => ins.Position <= i.Count,
                    Edit<EqA, A>.Delete del => i.Elem(del.Position).Match(
                        Some: ci => default(EqA).Equals(del.Element, ci),
                        None: () => false),
                    _ => e is Edit<EqA, A>.Replace repl && i.Elem(repl.Position).Match(
                             Some: ci => default(EqA).Equals(repl.Element, ci),
                             None: () => false)
                });
        }

        /// <summary>
        /// Returns true if a patch can be validly composed with another.
        /// That is, `composable(p, q)` holds if `q` can be validly applied after `p`.
        /// </summary>
        public static bool composable<EqA, A>(Patch<EqA, A> pa, Patch<EqA, A> pb) where EqA : struct, Eq<A>
        {
            return go(pa.Edits, pb.Edits, 0);
            
            static bool go(Seq<Edit<EqA, A>> ea, Seq<Edit<EqA, A>> eb, int off)
            {
                if (ea.IsEmpty || eb.IsEmpty) return true;
                var x = ea.Head;
                var xs = ea.Tail;
                var y = eb.Head;
                var ys = eb.Tail;
                var yi = y.Index(i => i + off);
                var xi = x.Position;
                if (xi < yi.Position)
                {
                    return go(xs, eb, off + offset(x));
                }
                else if (xi > yi.Position)
                {
                    return go(ea, ys, off);
                }
                else
                {
                    switch (x)
                    {
                        case Edit<EqA, A>.Delete del1 when yi is Edit<EqA, A>.Insert ins1:
                            return go(xs, ys, off + offset(x));
                        case Edit<EqA, A>.Delete del2:
                            return go(xs, eb, off + offset(x));
                        default:
                        {
                            if (yi is Edit<EqA, A>.Insert ins2)
                            {
                                return go(ea, ys, off);
                            }
                            else
                                return x switch
                                       {
                                           Edit<EqA, A>.Replace replA1 when yi is Edit<EqA, A>.Replace replB1 => 
                                               default(EqA).Equals(replA1.ReplaceElement, replB1.Element) && go(xs, ys, off),
                                           
                                           Edit<EqA, A>.Replace replA2 when yi is Edit<EqA, A>.Delete del3 => 
                                               default(EqA).Equals(replA2.ReplaceElement, del3.Element) && go(xs, ys, off),
                                           
                                           Edit<EqA, A>.Insert ins3 when yi is Edit<EqA, A>.Replace replB2 => 
                                               default(EqA).Equals(ins3.Element, replB2.Element) && go(xs, ys, off + offset(x)),
                                           
                                           Edit<EqA, A>.Insert ins4 when yi is Edit<EqA, A>.Delete del4 => 
                                               default(EqA).Equals(ins4.Element, del4.Element) && go(xs, ys, off + offset(x)),
                                           
                                           _  => throw new NotSupportedException()
                                       };
                        }
                    }
                }

            }

            static int offset(Edit<EqA, A> edit) =>
                edit switch
                {
                    Edit<EqA, A>.Insert  => -1,
                    Edit<EqA, A>.Delete  => 1,
                    Edit<EqA, A>.Replace => 0,
                    _                    => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// Build the default parameters for building a patch
        /// </summary>
        static PatchParams<A, Edit<EqA, A>, int> parms<EqA, A>() where EqA : struct, Eq<A> =>
            new PatchParams<A, Edit<EqA, A>, int>(
                default(EqA).Equals,
                Edit<EqA, A>.Delete.New,
                Edit<EqA, A>.Insert.New,
                Edit<EqA, A>.Replace.New,
                static _ => 1,
                static x => x is Edit<EqA, A>.Delete ? 0 : 1);

        /// <summary>
        /// Returns the delta of the document's size when a patch is applied.
        /// Essentially the number of `Insert` minus the number of `Delete`.
        /// </summary>
        public static int sizeChange<EqA, A>(Patch<EqA, A> patch) where EqA : struct, Eq<A>
        {
            var count = 0;
            foreach (var item in patch.Edits)
            {
                switch (item)
                {
                    case Edit<EqA, A>.Delete:
                        count--;
                        break;
                    case Edit<EqA, A>.Insert:
                        count++;
                        break;
                }
            }
            return count;
        }

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public static Lst<A> apply<EqA, A>(Patch<EqA, A> patch, Lst<A> va) where EqA : struct, Eq<A> =>
            toList(apply(patch, va.AsEnumerable()));

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public static Seq<A> apply<EqA, A>(Patch<EqA, A> patch, Seq<A> va) where EqA : struct, Eq<A> =>
            toSeq(apply(patch, va.AsEnumerable()));

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public static Arr<A> apply<EqA, A>(Patch<EqA, A> patch, Arr<A> va) where EqA : struct, Eq<A> =>
            apply(patch, va.AsEnumerable()).ToArr();

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public static A[] apply<EqA, A>(Patch<EqA, A> patch, A[] va) where EqA : struct, Eq<A> =>
            apply(patch, va.AsEnumerable()).ToArray();

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public static SpanArray<A> apply<EqA, A>(Patch<EqA, A> patch, SpanArray<A> va) where EqA : struct, Eq<A> =>
            SpanArray<A>.New(apply(patch, va.AsEnumerable()));

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public static List<A> apply<EqA, A>(Patch<EqA, A> patch, List<A> va) where EqA : struct, Eq<A> =>
            new List<A>(apply(patch, va.AsEnumerable()));

        /// <summary>
        /// Apply a patch to a document, returning the transformed document.
        /// </summary>
        public static IEnumerable<A> apply<EqA, A>(Patch<EqA, A> patch, IEnumerable<A> va) where EqA : struct, Eq<A>
        {
            if (patch.Edits.Count == 0) return va;
            var i = SpanArray<A>.New(va);

            var dlength = i.Count + sizeChange(patch);
            var d = SpanArray<A>.New(dlength);

            go(patch.Edits, i, d, 0);
            return d;
 
            static Unit go(Seq<Edit<EqA, A>> edits, SpanArray<A> src, SpanArray<A> dest, int si)
            {
                while (true)
                {
                    if (edits.IsEmpty)
                    {
                        return dest.Count > 0
                                   ? src.UnsafeCopy(dest)
                                   : unit;
                    }
                    else
                    {
                        var x = edits.Head.Position - si;
                        if (x > 0)
                        {
                            src.Take(x).UnsafeCopy(dest.Take(x));
                            src  = src.Skip(x);
                            dest = dest.Skip(x);
                            si   = si + x;
                            continue;
                        }
                        else
                        {
                            switch (edits.Head)
                            {
                                case Edit<EqA, A>.Insert insert:
                                    dest[0] = insert.Element;
                                    edits   = edits.Tail;
                                    dest    = dest.Tail;
                                    continue;

                                case Edit<EqA, A>.Delete delete:
                                    edits = edits.Tail;
                                    src   = src.Tail;
                                    si    = si + 1;
                                    continue;

                                case Edit<EqA, A>.Replace replace:
                                    dest[0] = replace.ReplaceElement;
                                    edits   = edits.Tail;
                                    src     = src.Tail;
                                    dest    = dest.Tail;
                                    si      = si + 1;
                                    continue;

                                default:
                                    throw new NotSupportedException();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Empty patch
        /// </summary>
        public static Patch<EqA, A> empty<EqA, A>() where EqA : struct, Eq<A> =>
            Patch<EqA, A>.Empty;

        /// <summary>
        /// Resolve a conflict by always using the left-hand side
        /// </summary>
        public static A ours<A>(A x, A y) =>
            x;

        /// <summary>
        /// Resolve a conflict by always using the right-hand side
        /// </summary>
        public static A theirs<A>(A x, A y) =>
            y;

        /// <summary>
        /// A convenience version of `transformWith` which resolves conflicts using `append`.
        /// </summary>
        public static (Patch<MonoidEqA, A> a, Patch<MonoidEqA, A> b) transform<MonoidEqA, A>(Patch<MonoidEqA, A> p, Patch<MonoidEqA, A> q)
            where MonoidEqA : struct, Monoid<A>, Eq<A> =>
                transformWith(default(MonoidEqA).Append, p, q);

        /// <summary>
        /// Given two diverging patches `p` and `q`, `transform(m, p, q)` returns
        /// a pair of updated patches `(np, nq)` such that `append(q, np)` and
        /// `append(p, nq)` are equivalent patches that incorporate the changes
        /// of _both_ `p` and `q`, up to merge conflicts, which are handled by
        /// the provided function `m`.
        /// 
        /// This is the standard `transform` function of Operational Transformation
        /// patch resolution techniques, and can be thought of as the pushout
        /// of two diverging patches within the patch groupoid.
        /// </summary>
        public static (Patch<EqA, A> a, Patch<EqA, A> b) transformWith<EqA, A>(Func<A, A, A> conflict, Patch<EqA, A> p, Patch<EqA, A> q) where EqA : struct, Eq<A>
        {
            var (pi, qi) = go(p.Edits, 0, q.Edits, 0, conflict);
            return (new Patch<EqA, A>(pi), new Patch<EqA, A>(qi));
 
            static (Seq<Edit<EqA, A>> a, Seq<Edit<EqA, A>> b) go(Seq<Edit<EqA, A>> xs, int a, Seq<Edit<EqA, A>> ys, int b, Func<A, A, A> conflict)
            {
                if (xs.IsEmpty && ys.IsEmpty) return (xs, ys);
                if (ys.IsEmpty) return (xs.Map(e => e.Index(i => i + a)), ys);
                if (xs.IsEmpty) return (xs, ys.Map(e => e.Index(i => i + b)));
                var x = xs.Head;
                var y = ys.Head;
                var ord = x.Position.CompareTo(y.Position);
                switch (ord)
                {
                    case < 0:
                    {
                        var (_1, _2) = go(xs.Tail, a, ys, b + offset(x), conflict);
                        return (x.Index(i => i + a).Cons(_1), _2);
                    }
                    case > 0:
                    {
                        var (_1, _2) = go(xs, a + offset(y), ys.Tail, b, conflict);
                        return (_1, y.Index(i => i + b).Cons(_2));
                    }
                    default:
                    {
                        if (x == y)
                        {
                            return go(xs.Tail, a + offset(y), ys.Tail, b + offset(x), conflict);
                        }
                        else switch (x)
                        {
                            case Edit<EqA, A>.Insert ins1 when y is Edit<EqA, A>.Insert ins2:
                            {
                                var n = conflict(ins1.Element, ins2.Element);
                                return cons2(
                                    (Edit<EqA, A>.Replace.New(ins1.Position + a, ins2.Element, n),
                                     Edit<EqA, A>.Replace.New(ins1.Position + b, ins1.Element, n)),
                                    go(xs.Tail, a + offset(y), ys.Tail, b + offset(x), conflict));
                            }
                            case Edit<EqA, A>.Replace repl1 when y is Edit<EqA, A>.Replace repl2:
                            {
                                var n = conflict(repl1.ReplaceElement, repl2.ReplaceElement);
                                return cons2(
                                    (Edit<EqA, A>.Replace.New(repl1.Position + a, repl2.ReplaceElement, n),
                                     Edit<EqA, A>.Replace.New(repl1.Position + b, repl1.ReplaceElement, n)),
                                    go(xs.Tail, a, ys.Tail, b, conflict));
                            }
                            case Edit<EqA, A>.Insert ins3:
                            {
                                var (_1, _2) = go(xs.Tail, a, ys, b + offset(x), conflict);
                                return (x.Index(i => i + a).Cons(_1), _2);
                            }
                            default:
                            {
                                if (y is Edit<EqA, A>.Insert ins4)
                                {
                                    var (_1, _2) = go(xs, a + offset(y), ys.Tail, b, conflict);
                                    return (_1, y.Index(i => i + b).Cons(_2));
                                }
                                else switch (x)
                                {
                                    case Edit<EqA, A>.Replace repl3 when y is Edit<EqA, A>.Delete del1:
                                    {
                                        var (_1, _2) = go(xs.Tail, a + offset(y), ys.Tail, b, conflict);
                                        return (_1, (Edit<EqA, A>.Delete.New(repl3.Position, repl3.ReplaceElement)).Index(i => i + b).Cons(_2));
                                    }
                                    case Edit<EqA, A>.Delete del3 when y is Edit<EqA, A>.Replace repl4:
                                    {
                                        var (_1, _2) = go(xs.Tail, a, ys.Tail, b + offset(x), conflict);
                                        return ((Edit<EqA, A>.Delete.New(repl4.Position, repl4.ReplaceElement)).Index(i => i + a).Cons(_1), _2);
                                    }
                                    case Edit<EqA, A>.Delete del4 when y is Edit<EqA, A>.Delete del5:
                                        return go(xs.Tail, a + offset(y), ys.Tail, b + offset(x), conflict);
                                    default:
                                        throw new NotSupportedException();
                                }
                            }
                        }
                    }
                }
            }

            static int offset(Edit<EqA, A> edit) =>
                edit switch
                {
                    Edit<EqA, A>.Insert  => 1,
                    Edit<EqA, A>.Delete  => -1,
                    Edit<EqA, A>.Replace => 0,
                    _                    => throw new NotSupportedException()
                };

            static (Seq<Edit<EqA, A>>, Seq<Edit<EqA, A>>) cons2((Edit<EqA, A> cx, Edit<EqA, A> cy) head, (Seq<Edit<EqA, A>> cxs, Seq<Edit<EqA, A>> cys) tail) =>
                (head.cx.Cons(tail.cxs), head.cy.Cons(tail.cys));
        }

        /// <summary>
        /// Compute the difference between two documents, using the Wagner-Fischer algorithm. 
        /// O(mn) time and space.
        /// 
        ///     apply(diff(d,e), d) == e
        ///     
        ///     diff(d, d) == Patch.empty
        ///     
        ///     apply(diff(d, e), d) == apply(inverse(diff(e, d)), d)
        ///     
        ///     apply(append(diff(a, b), diff(b, c), a) == apply(diff(a, c), a)
        ///     
        ///     applicable(diff(a, b) a)
        /// 
        /// </summary>
        public static Patch<EqA, A> diff<EqA, A>(IEnumerable<A> va, IEnumerable<A> vb) where EqA : struct, Eq<A>
        {
            var (_, s) = PatchInternal.leastChanges<TInt, A, Edit<EqA, A>, int>(parms<EqA, A>(), SpanArray<A>.New(va), SpanArray<A>.New(vb));
            return new Patch<EqA, A>(adjust(0, s));

            static Seq<Edit<EqA, A>> adjust(int o, Seq<Edit<EqA, A>> list) =>
                list.IsEmpty
                    ? Seq<Edit<EqA, A>>()
                    : list.Head is Edit<EqA, A>.Insert ia ? Edit<EqA, A>.Insert.New(ia.Position + o, ia.Element).Cons(adjust(o - 1, list.Tail))
                    : list.Head is Edit<EqA, A>.Delete da ? Edit<EqA, A>.Delete.New(da.Position + o, da.Element).Cons(adjust(o + 1, list.Tail))
                    : list.Head is Edit<EqA, A>.Replace ra ? Edit<EqA, A>.Replace.New(ra.Position + o, ra.Element, ra.ReplaceElement).Cons(adjust(o, list.Tail))
                    : throw new NotSupportedException();
        }
    }
}
