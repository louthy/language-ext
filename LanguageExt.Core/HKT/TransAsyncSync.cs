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
    public struct TransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, A> : 
        MonadTransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, A>
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerType, A>
    {
        static readonly OuterMonad MOuter = new OuterMonad();
        static readonly InnerMonad MInner = new InnerMonad();

        public static readonly TransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, A> Inst;

        public NewOuterC SelectManyAsync<NewOuterMonadB, NewOuterB, NewInnerMonadB, NewInnerB, B, NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(
            OuterType ma,
            Func<A, NewOuterB> bind,
            Func<A, B, C> project)
            where NewOuterMonadB : struct, MonadAsync<NewOuterB, NewInnerB>
            where NewInnerMonadB : struct, Monad<NewInnerB, B>
            where NewOuterMonadC : struct, MonadAsync<NewOuterC, NewInnerC>
            where NewInnerMonadC : struct, Monad<NewInnerC, C> =>
            default(TransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, A>).BindAsync<NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(ma, a =>
            default(TransAsyncSync<NewOuterMonadB, NewOuterB, NewInnerMonadB, NewInnerB, B>).BindAsync<NewOuterMonadC, NewOuterC, NewInnerMonadC, NewInnerC, C>(bind(a), b =>
            default(NewOuterMonadC).ReturnAsync(default(NewInnerMonadC).Return(project(a, b)).AsTask())));

        public NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).ReturnAsync(MInner.Bind<NewInnerMonad, NewInnerType, B>(inner, f).AsTask()));

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    MInner.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(inner, f).AsTask());

        public NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(ma, a => default(NewInnerMonad).Return(f(a)));

        public Task<S> Fold<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.Fold(ma, state, (s, inner) =>
                MInner.Fold(inner, s, (s2, a) => f(s2, a))(unit))(unit);

        public Task<S> FoldBack<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.FoldBack(ma, state, (s, inner) =>
                MInner.FoldBack(inner, s, (s2, a) => f(s2, a))(unit))(unit);

        public Task<int> Count(OuterType ma) =>
            default(TransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, A>)
                .Fold(ma, 0, (s, x) => s + 1); // TODO: Find more efficient way

        public OuterType Plus(OuterType ma, OuterType mb) =>
            MOuter.Apply(MInner.Plus, ma, mb);

        public OuterType Zero() =>
            MOuter.ReturnAsync(MInner.Zero().AsTask());
    }

    public struct TransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A>
        where OuterMonad : struct, MonadAsync<OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerType, A>
        where NumA : struct, Num<A>
    {
        public static readonly TransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A> Inst;

        public Task<A> Sum(OuterType ma) =>
            default(TransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, A>).Fold(ma,
                default(NumA).Empty(), (s, x) => default(NumA).Plus(s, x));
    }
}
