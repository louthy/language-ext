using System;
using LanguageExt.Attributes;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface MonadTransAsyncSync<OuterMonad, OuterType, InnerMonad, InnerType, A> : Typeclass
        where OuterMonad : struct, MonadAsync<OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerType, A>
    {
        NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        NewOuterType MapAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>;

        OuterType ZeroAsync();
        OuterType PlusAsync(OuterType a, OuterType b);

        Task<S> FoldAsync<S>(OuterType ma, S state, Func<S, A, S> f);
        Task<S> FoldBackAsync<S>(OuterType ma, S state, Func<S, A, S> f);
    }
}
