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
    public struct SeqTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A> : 
        MonadTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A>
            where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
            where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, A>
    {
        static readonly OuterMonad MOuter = new OuterMonad();
        static readonly InnerMonad MInner = new InnerMonad();

        public static readonly SeqTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A> Inst;

        public NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, Monad<OuterEnv, OuterOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<InnerEnv, InnerOut, NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).Return(_ =>
                        MInner.Bind<NewInnerMonad, NewInnerType, B>(inner, f)));

        // Here's how we're going to do this:
        // MO<MI<a>> =>(traverse) MI<MO<MO<MI<b>>>> =>(bind) MI<MO<MI<b>>> =>(sequence) MO<MI<MI<b>>> =>(bind) MO<MI<b>>
        public NewOuterType Bind<
            NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B,
            MI_O_NewOuterMonad, I_O_NewOuterType, MO_NewOuterMonad, O_NewOuterType,
            MI_NewOuterMonad, I_NewOuterType,
            MO_I_NewInnerMonad, O_I_NewInnerType, MI_NewInnerMonad, I_NewInnerType
        >(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, Monad<OuterEnv, OuterOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<InnerEnv, InnerOut, NewInnerType, B>
            where MI_O_NewOuterMonad : struct, Monad<InnerEnv, InnerOut, I_O_NewOuterType, O_NewOuterType>
            where MO_NewOuterMonad : struct, Monad<OuterEnv, OuterOut, O_NewOuterType, NewOuterType>
            where MI_NewOuterMonad : struct, Monad<InnerEnv, InnerOut, I_NewOuterType, NewOuterType>
            where MO_I_NewInnerMonad : struct, Monad<OuterEnv, OuterOut, O_I_NewInnerType, I_NewInnerType>
            where MI_NewInnerMonad : struct, Monad<InnerEnv, InnerOut, I_NewInnerType, NewInnerType> =>
                SeqTrans<OuterEnv, OuterOut, MO_I_NewInnerMonad, O_I_NewInnerType, InnerEnv, InnerOut, MI_NewInnerMonad, I_NewInnerType, NewInnerType>.Inst
                    .Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(
                        SeqTrans<InnerEnv, InnerOut, MI_NewOuterMonad, I_NewOuterType, OuterEnv, OuterOut, NewOuterMonad, NewOuterType, NewInnerType>.Inst
                            .Sequence<MO_I_NewInnerMonad, O_I_NewInnerType, MI_NewInnerMonad, I_NewInnerType>(
                                SeqTrans<InnerEnv, InnerOut, MI_O_NewOuterMonad, I_O_NewOuterType, OuterEnv, OuterOut, MO_NewOuterMonad, O_NewOuterType, NewOuterType>.Inst
                                    .Bind<MI_NewOuterMonad, I_NewOuterType, NewOuterMonad, NewOuterType, NewInnerType>(
                                        Traverse<MI_O_NewOuterMonad, I_O_NewOuterType, MO_NewOuterMonad, O_NewOuterType, NewOuterType>(ma, f),
                                        identity
                                    )
                            ),
                        identity
                    );

        public NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<OuterEnv, OuterOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<InnerEnv, InnerOut, NewInnerType, B> =>
                MOuter.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        default(NewOuterMonad).Return(_ =>
                            MInner.Bind<NewInnerMonad, NewInnerType, B>(inner,
                                a => default(NewInnerMonad).Return(__ => f(a)))));

        public S Fold<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.Fold(ma, state, (s, inner) =>
                MInner.Fold(inner, s, f)(default))(default);

        public S FoldBack<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.FoldBack(ma, state, (s, inner) =>
                MInner.FoldBack(inner, s, f)(default))(default);

        public int Count(OuterType ma) =>
            MOuter.Fold(ma, 0, (s, inner) =>
                s + MInner.Count(inner)(default))(default);

        public NewOuterType Sequence<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, Monad<InnerEnv, InnerOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<OuterEnv, OuterOut, NewInnerType, A> =>
                Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, A>(ma, identity);

        public NewOuterType Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<InnerEnv, InnerOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<OuterEnv, OuterOut, NewInnerType, B> =>
                MOuter.Fold(ma, default(NewOuterMonad).Zero(), (outerState, innerA) =>
                    default(NewOuterMonad).Plus(outerState,
                        MInner.Bind<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                            default(NewOuterMonad).Return(_ => default(NewInnerMonad).Return(__ => f(a))))))(default);

        public OuterType Plus(OuterType ma, OuterType mb) =>
            MOuter.Plus(ma, mb);

        public OuterType Zero() =>
            MOuter.Zero();
    }

    public struct SeqTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, NumA, A>
        where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, A>
        where NumA : struct, Num<A>
    {
        public static readonly SeqTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, NumA, A> Inst;
        public A Sum(OuterType ma) =>
            default(OuterMonad).Fold(ma, default(NumA).FromInteger(0), (s, inner) =>
                default(InnerMonad).Fold(inner, s, (ss, x) => default(NumA).Plus(ss, x))(default))(default);
    }
}
