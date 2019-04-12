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
            return new ConcactEnum<A>(ma, mb);
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


        class ConcactEnum<A> : IEnumerable<A>
        {
            readonly IEnumerable<A> ma;
            readonly IEnumerable<A> mb;

            public ConcactEnum(IEnumerable<A> ma, IEnumerable<A> mb)
            {
                this.ma = ma;
                this.mb = mb;
            }

            public IEnumerator<A> GetEnumerator() =>
                new ConcatIter<A>(ma, mb);

            IEnumerator IEnumerable.GetEnumerator() =>
                new ConcatIter<A>(ma, mb);
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
            IEnumerable<A> ema;
            IEnumerable<A> emb;
            IEnumerator<A> ma;
            IEnumerator<A> mb;
            A current;

            public ConcatIter(IEnumerable<A> ma, IEnumerable<A> mb)
            {
                this.ema = ma;
                this.emb = mb;
                this.ma = ema.GetEnumerator();
                this.mb = emb.GetEnumerator();
            }

            public A Current =>
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
                if (ma == null)
                {
                    if (mb.MoveNext())
                    {
                        current = mb.Current;
                        return true;
                    }
                    else
                    {
                        current = default;
                        mb.Dispose();
                        return false;
                    }
                }
                else
                {
                    if (ma.MoveNext())
                    {
                        current = ma.Current;
                        return true;
                    }
                    else
                    {
                        ma.Dispose();
                        ma = null;

                        if (mb.MoveNext())
                        {
                            current = mb.Current;
                            return true;
                        }
                        else
                        {
                            current = default;
                            mb.Dispose();
                            return false;
                        }
                    }
                }
            }

            public void Reset()
            {
                Dispose();
                ma = ema.GetEnumerator();
                mb = emb.GetEnumerator();
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
