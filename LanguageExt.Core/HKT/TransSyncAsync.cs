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

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).Return(MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner, f)));

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<NewInnerType>> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).Return(MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner, f)));

        public NewOuterType MapAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        default(NewOuterMonad).Return(
                            MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner,
                                a => default(NewInnerMonad).ReturnAsync(f(a).AsTask()))));

        public NewOuterType MapAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<B>> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        default(NewOuterMonad).Return(
                            MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner,
                                a => default(NewInnerMonad).ReturnAsync(f(a)))));

        public Task<S> FoldAsync<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.Fold(ma, state.AsTask(), (s, inner) =>
                MInner.FoldAsync(inner, s, async (ts, a) =>
                    {
                        var rs = await ts;
                        var ns = f(rs, a);
                        return ns;
                    })(unit).Flatten())(unit);

        public Task<S> FoldBackAsync<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.FoldBack(ma, state.AsTask(), (s, inner) =>
                MInner.FoldBackAsync(inner, s, async (ts, a) =>
                {
                    var rs = await ts;
                    var ns = f(rs, a);
                    return ns;
                })(unit).Flatten())(unit);

        public Task<S> FoldAsync<S>(OuterType ma, S state, Func<S, A, Task<S>> f) =>
            MOuter.Fold(ma, state.AsTask(), (s, inner) =>
                MInner.FoldAsync(inner, s, async (ts, a) =>
                {
                    var rs = await ts;
                    var ns = await f(rs, a);
                    return ns;
                })(unit).Flatten())(unit);

        public Task<S> FoldBackAsync<S>(OuterType ma, S state, Func<S, A, Task<S>> f) =>
            MOuter.FoldBack(ma, state.AsTask(), (s, inner) =>
                MInner.FoldBackAsync(inner, s, async (ts, a) =>
                {
                    var rs = await ts;
                    var ns = await f(rs, a);
                    return ns;
                })(unit).Flatten())(unit);

        public Task<int> CountAsync(OuterType ma) =>
            default(TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>)
                .FoldAsync(ma, 0, (s, x) => s + 1); // TODO: Find more efficient way

        public NewOuterType SequenceAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, A> =>
                TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, A>(ma, a => a);

        public NewOuterType TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Fold(ma, default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).Zero().AsTask()), (outerState, innerA) =>
                    TransAsyncSync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>.Inst.PlusAsync(outerState,
                        MInner.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                            default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).Return(f(a)).AsTask()))))(unit);

        public NewOuterType TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<B>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Fold(ma, default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).Zero().AsTask()), (outerState, innerA) =>
                    TransAsyncSync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>.Inst.PlusAsync(outerState,
                        MInner.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                            default(NewOuterMonad).ReturnAsync(async _ => default(NewInnerMonad).Return(await f(a))))))(unit);

        public OuterType PlusAsync(OuterType ma, OuterType mb) =>
            MOuter.Apply(MInner.PlusAsync, ma, mb);

        public OuterType ZeroAsync() =>
            MOuter.Return(MInner.ZeroAsync());
    }

    public struct TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A>
        where OuterMonad : struct, Monad<OuterType, InnerType>
        where InnerMonad : struct, MonadAsync<InnerType, A>
        where NumA : struct, Num<A>
    {
        public static readonly TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A> Inst;

        public Task<A> SumAsync(OuterType ma) =>
            default(TransSyncAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>).FoldAsync(ma,
                default(NumA).Empty(), (s, x) => default(NumA).Plus(s, x));
    }
}
