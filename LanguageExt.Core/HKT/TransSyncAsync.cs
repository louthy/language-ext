using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Default monad transformer, can nest any two monadic types and provide the
    /// correct default behaviour based on their Bind operations.
    /// 
    ///     i.e.
    ///     
    ///         TransSyncAsync<MSeq<TryAsync<int>>, Seq<TryAsync<int>>, MTryAsync<int>, TryAsync<int>, int>
    /// 
    /// </summary>
    public struct TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> : 
        MonadTransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>
            where OuterMonad : struct, Monad<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, A>
    {
        static readonly OuterMonad MOuter = new OuterMonad();
        static readonly InnerMonad MInner = new InnerMonad();

        public static readonly TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> Inst;

        public NewOuterC SelectManyAsync<NewOuterMonadB, NewOuterB, NewInnerMonadB, NewInnerB, B, NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(
            OuterType ma,
            Func<A, NewInnerB> bind,
            Func<A, B, C> project)
            where NewOuterMonadB : struct, Monad<NewOuterB, NewInnerB>
            where NewInnerMonadB : struct, MonadAsync<NewInnerB, B>
            where NewOuterMonadC : struct, Monad<NewOuterC, NewInnerC>
            where NewInnerMonadC : struct, MonadAsync<NewInnerC, C> =>
            default(TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>).Bind<NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(ma, a =>
            {
                return default(NewInnerMonadB).Bind<NewInnerMonadC, NewInnerC, C>(bind(a), b =>
                {
                    return default(NewInnerMonadC).ReturnAsync(project(a, b).AsTask());
                });
            });

        public NewOuterC SelectManyAsync<NewOuterMonadB, NewOuterB, NewInnerMonadB, NewInnerB, B, NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(
            OuterType ma,
            Func<A, Task<NewInnerB>> bind,
            Func<A, B, C> project)
            where NewOuterMonadB : struct, Monad<NewOuterB, NewInnerB>
            where NewInnerMonadB : struct, MonadAsync<NewInnerB, B>
            where NewOuterMonadC : struct, Monad<NewOuterC, NewInnerC>
            where NewInnerMonadC : struct, MonadAsync<NewInnerC, C> =>
            default(TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>).BindAsync<NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(ma, async a =>
            {
                return default(NewInnerMonadB).Bind<NewInnerMonadC, NewInnerC, C>(await bind(a), b =>
                {
                    return default(NewInnerMonadC).ReturnAsync(project(a, b).AsTask());
                });
            });

        public NewOuterC SelectManyAsync<NewOuterMonadB, NewOuterB, NewInnerMonadB, NewInnerB, B, NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(
            OuterType ma,
            Func<A, NewInnerB> bind,
            Func<A, B, Task<C>> project)
            where NewOuterMonadB : struct, Monad<NewOuterB, NewInnerB>
            where NewInnerMonadB : struct, MonadAsync<NewInnerB, B>
            where NewOuterMonadC : struct, Monad<NewOuterC, NewInnerC>
            where NewInnerMonadC : struct, MonadAsync<NewInnerC, C> =>
            default(TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>).Bind<NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(ma, a =>
            {
                return default(NewInnerMonadB).Bind<NewInnerMonadC, NewInnerC, C>(bind(a), b =>
                {
                    return default(NewInnerMonadC).ReturnAsync(project(a, b));
                });
            });

        public NewOuterC SelectManyAsync<NewOuterMonadB, NewOuterB, NewInnerMonadB, NewInnerB, B, NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(
            OuterType ma,
            Func<A, Task<NewInnerB>> bind,
            Func<A, B, Task<C>> project)
            where NewOuterMonadB : struct, Monad<NewOuterB, NewInnerB>
            where NewInnerMonadB : struct, MonadAsync<NewInnerB, B>
            where NewOuterMonadC : struct, Monad<NewOuterC, NewInnerC>
            where NewInnerMonadC : struct, MonadAsync<NewInnerC, C> =>
            default(TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>).BindAsync<NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(ma, async a =>
            {
                return default(NewInnerMonadB).Bind<NewInnerMonadC, NewInnerC, C>(await bind(a), b =>
                {
                    return default(NewInnerMonadC).ReturnAsync(project(a, b));
                });
            });

        public NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).Return(MInner.Bind<NewInnerMonad, NewInnerType, B>(inner, f)));

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<NewInnerType>> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).Return(MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner, f)));

        public NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        default(NewOuterMonad).Return(
                            MInner.Bind<NewInnerMonad, NewInnerType, B>(inner,
                                a => default(NewInnerMonad).ReturnAsync(f(a).AsTask()))));

        public NewOuterType MapAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<B>> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        default(NewOuterMonad).Return(
                            MInner.Bind<NewInnerMonad, NewInnerType, B>(inner,
                                a => default(NewInnerMonad).ReturnAsync(f(a)))));

        public Task<S> Fold<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.Fold(ma, state.AsTask(), (s, inner) =>
                MInner.Fold(inner, s, async (ts, a) =>
                    {
                        var rs = await ts;
                        var ns = f(rs, a);
                        return ns;
                    })(unit).Flatten())(unit);

        public Task<S> FoldBack<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.FoldBack(ma, state.AsTask(), (s, inner) =>
                MInner.FoldBack(inner, s, async (ts, a) =>
                {
                    var rs = await ts;
                    var ns = f(rs, a);
                    return ns;
                })(unit).Flatten())(unit);

        public Task<S> FoldAsync<S>(OuterType ma, S state, Func<S, A, Task<S>> f) =>
            MOuter.Fold(ma, state.AsTask(), (s, inner) =>
                MInner.Fold(inner, s, async (ts, a) =>
                {
                    var rs = await ts;
                    var ns = await f(rs, a);
                    return ns;
                })(unit).Flatten())(unit);

        public Task<S> FoldBackAsync<S>(OuterType ma, S state, Func<S, A, Task<S>> f) =>
            MOuter.FoldBack(ma, state.AsTask(), (s, inner) =>
                MInner.FoldBack(inner, s, async (ts, a) =>
                {
                    var rs = await ts;
                    var ns = await f(rs, a);
                    return ns;
                })(unit).Flatten())(unit);

        public Task<int> Count(OuterType ma) =>
            default(TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>)
                .Fold(ma, 0, (s, x) => s + 1); // TODO: Find more efficient way

        public NewOuterType Sequence<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, A> =>
                Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, A>(ma, a => a);

        public NewOuterType Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Fold(ma, default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).Zero().AsTask()), (outerState, innerA) =>
                    TransAsyncSync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>.Inst.Plus(outerState,
                        MInner.Bind<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                            default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).Return(f(a)).AsTask()))))(unit);

        public NewOuterType TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<B>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Fold(ma, default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).Zero().AsTask()), (outerState, innerA) =>
                    TransAsyncSync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>.Inst.Plus(outerState,
                        MInner.Bind<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                            default(NewOuterMonad).ReturnAsync(async _ => default(NewInnerMonad).Return(await f(a))))))(unit);

        public OuterType Plus(OuterType ma, OuterType mb) =>
            MOuter.Apply(MInner.Plus, ma, mb);

        public OuterType Zero() =>
            MOuter.Return(MInner.Zero());
    }

    public struct TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A>
        where OuterMonad : struct, Monad<OuterType, InnerType>
        where InnerMonad : struct, MonadAsync<InnerType, A>
        where NumA : struct, Num<A>
    {
        public static readonly TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A> Inst;

        public Task<A> Sum(OuterType ma) =>
            default(TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>).Fold(ma,
                default(NumA).Empty(), (s, x) => default(NumA).Plus(s, x));
    }
}
