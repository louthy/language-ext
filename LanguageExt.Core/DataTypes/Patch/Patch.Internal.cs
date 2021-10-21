using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Linq;

namespace LanguageExt
{
    internal static class PatchInternal
    {
        public static (A, Seq<C>) mapAccumR<A, B, C>(Func<A, B, (A, C)> f, A state, Seq<B> t)
        {
            if (t.IsEmpty) return (state, Seq<C>());
            var (a1, c1) = mapAccumR(f, state, t.Tail);
            var (a, c) = f(a1, t.Head);
            return (a, c.Cons(c1));
        }

        public static (A, Seq<C>) mapAccumL<A, B, C>(Func<A, B, (A, C)> f, A state, Seq<B> t)
        {
            if (t.IsEmpty) return (state, Seq<C>());
            var (a, c) = f(state, t.Head);
            var (a1, c1) = mapAccumL(f, a, t.Tail);
            return (a1, c.Cons(c1));
        }

        public static (C, Seq<O>) leastChanges<MonoidC, V, O, C>(PatchParams<V, O, C> p, SpanArray<V> ss, SpanArray<V> tt)
            where MonoidC : struct, Monoid<C>, Ord<C>
        {
            var rawChanges = rawChanges<MonoidC, V, O, C>(p, ss, tt);
            var changes = rawChanges.Last;
            var newlst = changes.Map(pair => toSeq(pair.Item2.Somes().Reverse()));
            return (changes.Item1, newlst);
        }

        static (int quot, int rem) quotRem(int x, int y) =>
            (x / y, x % y);

        static A minimumBy<A>(Func<A, A, int> compare, Lst<A> list) =>
            list.Count == 0
                ? throw new Exception("List is empty")
                : list.Fold(list[0], (x, y) => compare(x, y) > 0 ? y : x);

        static Func<A, A, C> on<A, B, C>(Func<B, B, C> on, Func<A, B> f) =>
            (A x, A y) => on(f(x), f(y));

        static SpanArray<A> constructN<A>(int n, Func<SpanArray<A>, A> f)
        {
            var vector = SpanArray<A>.New(n);
            for (int i = 0; i < n; i++)
            {
                var slice = vector.Slice(0, i);
                var x = f(slice);
                vector[i] = x;
            }
            return vector;
        }

        public static SpanArray<(C, Seq<Option<O>>)> rawChanges<MonoidC, V, O, C>(PatchParams<V, O, C> p, SpanArray<V> src, SpanArray<V> dst)
            where MonoidC : struct, Monoid<C>, Ord<C>
        {
            var lenX = 1 + dst.Count;
            var lenY = 1 + src.Count;
            var lenN = lenX * lenY;

            int ix(int x, int y) => (x * lenY) + y;

            (C, Seq<Option<O>>) get(SpanArray<(C, Seq<Option<O>>)> m, int x, int y)
            {
                int i = ix(x, y);
                return i < m.Count
                    ? m[i]
                    : throw new Exception($"Unable to get ({x},{y}) from change matrix");
            }

            int position(Seq<Option<O>> seq) =>
                seq.Map(oo => oo.Map(p.positionOffset).IfNone(1)).Sum();

            (C, Seq<Option<O>>) ctr(SpanArray<(C, Seq<Option<O>>)> v)
            {
                var (quot, rem) = quotRem(v.Count, lenY);

                if (quot == 0 && rem == 0)
                {
                    return (default(MonoidC).Empty(), Seq<Option<O>>());
                }
                else if (quot == 0)
                {
                    var y = rem - 1;
                    var o = p.delete(0, src[y]);
                    var (pc, po) = get(v, 0, y);
                    return (default(MonoidC).Append(p.cost(o), pc), Some(o).Cons(po));
                }
                else if (rem == 0)
                {
                    var x = quot - 1;
                    var o = p.insert(x, dst[x]);
                    var (pc, po) = get(v, x, 0);
                    return (default(MonoidC).Append(p.cost(o), pc), Some(o).Cons(po));
                }
                else
                {
                    var y = rem - 1;
                    var x = quot - 1;
                    var s = src[y];
                    var d = dst[x];
                    var tl = get(v, x, y);
                    var top = get(v, x + 1, y);
                    var left = get(v, x, y + 1);
                    if (p.equivalent(s, d))
                    {
                        return (tl.Item1, Prelude.Cons(None, tl.Item2));
                    }
                    else
                    {
                        var c1 = p.delete(position(top.Item2), s);
                        var item1 = (default(MonoidC).Append(p.cost(c1), top.Item1), Prelude.Cons(Some(c1), top.Item2));

                        var c2 = p.insert(position(left.Item2), d);
                        var item2 = (default(MonoidC).Append(p.cost(c2), left.Item1), Prelude.Cons(Some(c2), left.Item2));

                        var c3 = p.substitute(position(tl.Item2), s, d);
                        var item3 = (default(MonoidC).Append(p.cost(c3), tl.Item1), Prelude.Cons(Some(c3), tl.Item2));

                        Func<(C, Seq<Option<O>>), (C, Seq<Option<O>>), int> compare =
                            default(OrdTupleFirst<MonoidC, C, Seq<Option<O>>>).Compare;

                        return minimumBy(compare, List(item1, item2, item3));
                    }
                }
            }
            return constructN<(C, Seq<Option<O>>)>(lenN, ctr);
        }
    }
}
