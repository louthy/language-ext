using System;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    [Typeclass("M*Trans")]
    public interface MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, A> : Typeclass
        where OuterMonad : struct, Monad<OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerType, A>
    {
        NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        NewOuterType Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        NewOuterType Sequence<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, A>;

        OuterType Zero();
        OuterType Plus(OuterType a, OuterType b);

        S Fold<S>(OuterType ma, S state, Func<S, A, S> f);

        S FoldBack<S>(OuterType ma, S state, Func<S, A, S> f);
    }
}
