using System;

namespace LanguageExt;

record EmptySource<A> : Source<A>
{
    public static readonly Source<A> Default = new EmptySource<A>();
    
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<A, S> reducer) =>
        IO.pure(Reduced.Continue(state));

    public override Source<B> Map<B>(Func<A, B> f) =>
        EmptySource<B>.Default;
    
    public override Source<B> Bind<B>(Func<A, Source<B>> f) => 
        EmptySource<B>.Default;

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        EmptySource<B>.Default;
}
