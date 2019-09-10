using System;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface MonadTrans<OuterEnv, OuterOut, OuterMonad, OuterType, InnerEnv, InnerOut, InnerMonad, InnerType, A> : Typeclass
        where OuterMonad : struct, Monad<OuterEnv, OuterOut, OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerEnv, InnerOut, InnerType, A>
    {
        NewOuterType Bind<
            NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B,
            B_A_NewOuterMonad, B_A_NewOuterType, A_NewOuterMonad, A_NewOuterType,
            B_NewOuterMonad, B_NewOuterType,
            A_B_NewInnerMonad, A_B_NewInnerType, B_NewInnerMonad, B_NewInnerType
        >(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, Monad<OuterEnv, OuterOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<InnerEnv, InnerOut, NewInnerType, B>
            where B_A_NewOuterMonad : struct, Monad<InnerEnv, InnerOut, B_A_NewOuterType, A_NewOuterType>
            where A_NewOuterMonad : struct, Monad<OuterEnv, OuterOut, A_NewOuterType, NewOuterType>
            where B_NewOuterMonad : struct, Monad<InnerEnv, InnerOut, B_NewOuterType, NewOuterType>
            where A_B_NewInnerMonad : struct, Monad<OuterEnv, OuterOut, A_B_NewInnerType, B_NewInnerType>
            where B_NewInnerMonad : struct, Monad<InnerEnv, InnerOut, B_NewInnerType, NewInnerType>;

        NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, Monad<OuterEnv, OuterOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<InnerEnv, InnerOut, NewInnerType, B>;

        NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<OuterEnv, OuterOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<InnerEnv, InnerOut, NewInnerType, B>;

        NewOuterType Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<InnerEnv, InnerOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<OuterEnv, OuterOut, NewInnerType, B>;

        NewOuterType Sequence<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, Monad<InnerEnv, InnerOut, NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<OuterEnv, OuterOut,NewInnerType, A>;

        OuterType Zero();
        OuterType Plus(OuterType a, OuterType b);

        S Fold<S>(OuterType ma, S state, Func<S, A, S> f);

        S FoldBack<S>(OuterType ma, S state, Func<S, A, S> f);
    }
}
