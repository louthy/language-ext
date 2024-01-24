#nullable enable
using System;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

[Trait("M*Trans")]
public interface MonadTrans<OuterType, A> : Trait
{
    public static abstract NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
        where NewOuterMonad : Monad<NewOuterType, NewInnerType>;

    public static abstract NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
        where NewOuterMonad : Monad<NewOuterType, NewInnerType>
        where NewInnerMonad : Monad<NewInnerType, B>;

    public static abstract NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
        where NewOuterMonad : Monad<NewOuterType, NewInnerType>
        where NewInnerMonad : Monad<NewInnerType, B>;

    public static abstract NewOuterType Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
        where NewOuterMonad : Monad<NewOuterType, NewInnerType>
        where NewInnerMonad : Monad<NewInnerType, B>;

    public static abstract NewOuterType Sequence<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
        where NewOuterMonad : Monad<NewOuterType, NewInnerType>
        where NewInnerMonad : Monad<NewInnerType, A>;

    public static abstract OuterType Zero();
    public static abstract OuterType Plus(OuterType a, OuterType b);

    public static abstract S Fold<S>(OuterType ma, S state, Func<S, A, S> f);
    public static abstract S FoldBack<S>(OuterType ma, S state, Func<S, A, S> f);
}
