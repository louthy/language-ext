using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

record ApplySource<A, B>(Source<Func<A, B>> FF, Source<A> FA) : Source<B>
{
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<B, S> reducer, CancellationToken token) => 
        FF.Zip(FA).Map(p => p.First(p.Second)).ReduceAsync(state, reducer, token);
}

record ApplySource2<A, B>(Source<Func<A, B>> FF, Memo<Source, A> FA) : Source<B>
{
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<B, S> reducer, CancellationToken token) => 
        FF.Zip(FA.Value.As()).Map(p => p.First(p.Second)).ReduceAsync(state, reducer, token);
}
