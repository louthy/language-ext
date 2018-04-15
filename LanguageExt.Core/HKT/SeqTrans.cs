using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;

namespace LanguageExt
{
    /// <summary>
    /// Default monad transformer, can nest any two monadic types and provide the
    /// correct default behaviour based on their Bind operations.
    /// </summary>
    public struct SeqTrans<OuterMonad, OuterType, InnerMonad, InnerType, A> : 
        MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, A>
            where OuterMonad : struct, Monad<OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerType, A>
    {
        static readonly OuterMonad MOuter = new OuterMonad();
        static readonly InnerMonad MInner = new InnerMonad();

        public static readonly SeqTrans<OuterMonad, OuterType, InnerMonad, InnerType, A> Inst;

        public NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).Return(
                        MInner.Bind<NewInnerMonad, NewInnerType, B>(inner, f)));

        public NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    MInner.Bind<NewOuterMonad, NewOuterType, NewInnerType>(inner, a =>
                        f(a)));

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    MInner.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(inner, a =>
                        f(a)));

        public NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        default(NewOuterMonad).Return(
                            MInner.Bind<NewInnerMonad, NewInnerType, B>(inner,
                                a => default(NewInnerMonad).Return(f(a)))));

        public S Fold<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.Fold(ma, state, (s, inner) =>
                MInner.Fold(inner, s, f)(unit))(unit);

        public S FoldBack<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.FoldBack(ma, state, (s, inner) =>
                MInner.FoldBack(inner, s, f)(unit))(unit);

        public int Count(OuterType ma) =>
            MOuter.Fold(ma, 0, (s, inner) =>
                s + MInner.Count(inner)(unit))(unit);

        public NewOuterType Sequence<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, A> =>
                Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, A>(ma, identity);

        public NewOuterType Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
                MOuter.Fold(ma, default(NewOuterMonad).Zero(), (outerState, innerA) =>
                    SeqTrans<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>.Inst.Plus(outerState,
                        MInner.Bind<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                            default(NewOuterMonad).Return(default(NewInnerMonad).Return(f(a))))))(unit);

        public OuterType Plus(OuterType ma, OuterType mb) =>
            MOuter.Plus(ma, mb);

        public OuterType Zero() =>
            MOuter.Zero();
    }

    public struct SeqTrans<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A>
        where OuterMonad : struct, Monad<OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerType, A>
        where NumA : struct, Num<A>
    {
        public static readonly SeqTrans<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A> Inst;

        public A Sum(OuterType ma) =>
            default(OuterMonad).Fold(ma, default(NumA).FromInteger(0), (s, inner) =>
                default(InnerMonad).Fold(inner, s, (ss, x) => default(NumA).Plus(ss, x))(unit))(unit);
    }
}
