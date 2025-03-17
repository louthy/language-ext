using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes.Concurrent;

record ApplySource<A, B>(Source<Func<A, B>> FF, Source<A> FA) : Source<B>
{
    internal override SourceIterator<B> GetIterator() =>
        new ApplySourceIterator<A, B>(FF.GetIterator(), FA.GetIterator());

    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<B, S> reducer, CancellationToken token) =>
        FF.ReduceAsync(state,
                       (s, f) => token.IsCancellationRequested
                                     ? ValueTask.FromException<Reduced<S>>(Errors.Cancelled)
                                     : FA.ReduceAsync(s, (s1, x) => token.IsCancellationRequested
                                                                        ? ValueTask.FromException<Reduced<S>>(Errors.Cancelled)
                                                                        : reducer(s1, f(x)), token),
                       token);
}
