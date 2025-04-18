using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Pipes.Concurrent;

record ApplySource<A, B>(Source<Func<A, B>> FF, Source<A> FA) : Source<B>
{
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<B, S> reducer, CancellationToken token) => 
        FF.Zip(FA).Map(p => p.First(p.Second)).ReduceAsync(state, reducer, token);
}
