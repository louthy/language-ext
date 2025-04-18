using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record BindSource<A, B>(Source<A> Source, Func<A, Source<B>> F) : Source<B>
{
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<B, S> reducer, CancellationToken token) => 
        Source.ReduceAsync(state, (s, x) => F(x).ReduceAsync(s, reducer, token), token);
}
