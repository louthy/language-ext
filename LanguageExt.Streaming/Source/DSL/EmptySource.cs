using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record EmptySource<A> : Source<A>
{
    public static readonly Source<A> Default = new EmptySource<A>();
    
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token) =>
        Reduced.ContinueAsync(state);

    public override Source<B> Map<B>(Func<A, B> f) =>
        EmptySource<B>.Default;
    
    public override Source<B> Bind<B>(Func<A, Source<B>> f) => 
        EmptySource<B>.Default;

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        EmptySource<B>.Default;
}
