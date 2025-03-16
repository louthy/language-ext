using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record EmptySourceT<M, A> : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    public static readonly SourceT<M, A> Default = new EmptySourceT<M, A>();
    
    internal override SourceTIterator<M, A> GetIterator() =>
        EmptySourceTIterator<M, A>.Default;

    public override SourceT<M, B> Map<B>(Func<A, B> f) =>
        EmptySourceT<M, B>.Default;

    public override SourceT<M, B> Bind<B>(Func<A, SourceT<M, B>> f) => 
        EmptySourceT<M, B>.Default;

    public override SourceT<M, B> ApplyBack<B>(SourceT<M, Func<A, B>> ff) =>
        EmptySourceT<M, B>.Default;

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer) =>
        M.Pure(state);
}
