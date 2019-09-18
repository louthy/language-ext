using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class EnumerableOptimal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<A> ConcatFast<A>(this IEnumerable<A> ma, IEnumerable<A> mb) =>
            ma is ConcatEnum<A> ca
                ? ca.Concat(mb)
                : new ConcatEnum<A>(Seq(ma, mb));

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

        internal class ConcatEnum<A> : IEnumerable<A>
        {
            internal readonly Seq<IEnumerable<A>> ms;

            public ConcatEnum(Seq<IEnumerable<A>> ms)
            {
                this.ms = ms;
            }

            public IEnumerator<A> GetEnumerator() =>
                new ConcatIter<A>(ms);

            public ConcatEnum<A> Concat(IEnumerable<A> cb) =>
                new ConcatEnum<A>(ms.Add(cb));

            IEnumerator IEnumerable.GetEnumerator() =>
                new ConcatIter<A>(ms);
        }

        internal class BindEnum<A, B> : IEnumerable<B>
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
            Seq<IEnumerable<A>> ms;
            IEnumerator<A> iter;
            int index;
            A current;

            public ConcatIter(Seq<IEnumerable<A>> ms)
            {
                this.ms = ms;
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
                    while(index < ms.Count)
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
