using System;
using LanguageExt.Attributes;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> : Typeclass
        where OuterMonad : struct, MonadAsync<OuterType, InnerType>
        where InnerMonad : struct, MonadAsync<InnerType, A>
    {
        NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B>;

        NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B>;

        NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<NewOuterType>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B>;

        NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<NewInnerType>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B>;

        NewOuterType MapAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B>;

        NewOuterType MapAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<B>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B>;

        NewOuterType TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B>;

        NewOuterType TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<B>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B>;

        NewOuterType SequenceAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, A>;

        OuterType ZeroAsync();
        OuterType PlusAsync(OuterType a, OuterType b);

        Task<S> FoldAsync<S>(OuterType ma, S state, Func<S, A, S> f);

        Task<S> FoldAsync<S>(OuterType ma, S state, Func<S, A, Task<S>> f);

        Task<S> FoldBackAsync<S>(OuterType ma, S state, Func<S, A, S> f);

        Task<S> FoldBackAsync<S>(OuterType ma, S state, Func<S, A, Task<S>> f);
    }
}
