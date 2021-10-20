using System;
using LanguageExt.Attributes;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass("M*TransAsyncSync")]
    public interface MonadTransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, A> : Typeclass
        where OuterMonad : struct, MonadAsync<OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerType, A>
    {
        NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        OuterType Zero();
        OuterType Plus(OuterType a, OuterType b);

        Task<S> Fold<S>(OuterType ma, S state, Func<S, A, S> f);
        Task<S> FoldBack<S>(OuterType ma, S state, Func<S, A, S> f);
    }
}
