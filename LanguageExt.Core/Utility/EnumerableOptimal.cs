using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static class EnumerableOptimal
    {
        public static IEnumerable<A> ConcatFast<A>(this IEnumerable<A> ma, IEnumerable<A> mb)
        {
            if (ma == null && mb == null) return new A[0];
            if (ma == null) return mb;
            if (mb == null) return ma;

            if (ma is ConcatEnum<A> ca && mb is ConcatEnum<A> cb)
            {
                var cs = new IEnumerable<A>[ca.count + cb.count];
                Array.Copy(ca.ms, cs, ca.count);
                Array.Copy(cb.ms, 0, cs, ca.count, cb.count);
                return new ConcatEnum<A>(cs, cs.Length);
            }
            else if (ma is ConcatEnum<A> ca2)
            {
                var cs = new IEnumerable<A>[ca2.count + 1];
                Array.Copy(ca2.ms, cs, ca2.count);
                cs[ca2.count] = mb;
                return new ConcatEnum<A>(cs, cs.Length);
            }
            else if (mb is ConcatEnum<A> cb2)
            {
                var cs = new IEnumerable<A>[cb2.count + 1];
                Array.Copy(cb2.ms, 0, cs, 1, cb2.count);
                cs[0] = mb;
                return new ConcatEnum<A>(cs, cs.Length);
            }
            else
            {
                return new ConcatEnum<A>(new[] { ma, mb }, 2);
            }
        }

        internal static IEnumerable<B> BindFast<A, B>(this IEnumerable<A> ma, Func<A, IEnumerable<B>> f) =>
            ma == null
                ? (IEnumerable<B>)(new B[0])
                : new BindEnum<A, B>(ma, f);

        internal static IEnumerable<B> BindFast<A, B>(this IEnumerable<A> ma, Func<A, Lst<B>> f) =>
            ma == null
                ? (IEnumerable<B>)(new B[0])
                : new BindEnum<A, B>(ma, a => f(a).AsEnumerable());

        internal static IEnumerable<B> BindFast<PredList, A, B>(this IEnumerable<A> ma, Func<A, Lst<PredList, B>> f)
            where PredList : struct, Pred<ListInfo> =>
            ma == null
                ? (IEnumerable<B>)(new B[0])
                : new BindEnum<A, B>(ma, a => f(a).AsEnumerable());

        internal static IEnumerable<B> BindFast<PredList, PredItemA, PredItemB, A, B>(this IEnumerable<A> ma, Func<A, Lst<PredList, PredItemB, B>> f)
            where PredList : struct, Pred<ListInfo>
            where PredItemA : struct, Pred<A>
            where PredItemB : struct, Pred<B> =>
            ma == null
                ? (IEnumerable<B>)(new B[0])
                : new BindEnum<A, B>(ma, a => f(a).AsEnumerable());

        internal static IEnumerable<B> BindFast<A, B>(this IEnumerable<A> ma, Func<A, Seq<B>> f) =>
            ma == null
                ? (IEnumerable<B>)(new B[0])
                : new BindEnum<A, B>(ma, a => f(a).AsEnumerable());

        class ConcatEnum<A> : IEnumerable<A>
        {
            internal readonly IEnumerable<A>[] ms;
            internal readonly int count;

            public ConcatEnum(IEnumerable<A>[] ms, int count)
            {
                this.ms = ms;
                this.count = count;
            }

            public IEnumerator<A> GetEnumerator() =>
                new ConcatIter<A>(ms, count);

            IEnumerator IEnumerable.GetEnumerator() =>
                new ConcatIter<A>(ms, count);
        }

        class BindEnum<A, B> : IEnumerable<B>
        {
            readonly IEnumerable<A> ma;
            readonly Func<A, IEnumerable<B>> f;

            public BindEnum(IEnumerable<A> ma, Func<A, IEnumerable<B>> f)
            {
                this.ma = ma;
                this.f = f;
            }

            public IEnumerator<B> GetEnumerator() =>
                new BindIter<A, B>(ma, f);

            IEnumerator IEnumerable.GetEnumerator() =>
                new BindIter<A, B>(ma, f);
        }

        class ConcatIter<A> : IEnumerator<A>
        {
            IEnumerable<A>[] ms;
            IEnumerator<A> iter;
            int count;
            int index;
            A current;

            public ConcatIter(IEnumerable<A>[] ms, int count)
            {
                this.ms = ms;
                this.count = count;
                this.index = 0;
                this.iter = ms[0].GetEnumerator();
            }

            public A Current => 
                current;

            object IEnumerator.Current => 
                current;

            public void Dispose() =>
                iter?.Dispose();

            public bool MoveNext()
            {
                if (iter.MoveNext())
                {
                    current = iter.Current;
                    return true;
                }
                else
                {
                    current = default;
                    index++;
                    while(index < count)
                    {
                        iter.Dispose();
                        iter = ms[index].GetEnumerator();
                        if (iter.MoveNext())
                        {
                            current = iter.Current;
                            return true;
                        }
                        else
                        {
                            index++;
                            continue;
                        }
                    }
                    iter.Dispose();
                    return false;
                }
            }

            public void Reset()
            {
                Dispose();
                index = 0;
                iter = ms[0].GetEnumerator();
            }
        }

        class BindIter<A, B> : IEnumerator<B>
        {
            readonly Func<A, IEnumerable<B>> f;
            readonly IEnumerable<A> ema;
            IEnumerator<A> ma;
            IEnumerator<B> mb;
            B current;

            public BindIter(IEnumerable<A> ma, Func<A, IEnumerable<B>> f)
            {
                this.ema = ma;
                this.ma = ema.GetEnumerator();
                this.f = f;
            }

            public B Current =>
                current;

            object IEnumerator.Current =>
                current;

            public void Dispose()
            {
                ma?.Dispose();
                mb?.Dispose();
            }

            public bool MoveNext()
            {
                if (ma == null) return false;
                if (mb == null)
                {
                    while (ma.MoveNext())
                    {
                        mb = f(ma.Current).GetEnumerator();
                        if (mb.MoveNext())
                        {
                            current = mb.Current;
                            return true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    ma.Dispose();
                    ma = null;
                    return false;
                }
                else
                {
                    if (mb.MoveNext())
                    {
                        current = mb.Current;
                        return true;
                    }
                    else
                    {
                        mb.Dispose();
                        mb = null;
                        while (ma.MoveNext())
                        {
                            mb = f(ma.Current).GetEnumerator();
                            if (mb.MoveNext())
                            {
                                current = mb.Current;
                                return true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        ma.Dispose();
                        ma = null;
                        return false;
                    }
                }
            }

            public void Reset()
            {
                Dispose();
                ma = ema.GetEnumerator();
            }
        }
    }
}
