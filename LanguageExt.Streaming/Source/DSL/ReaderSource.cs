using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LanguageExt;

record ReaderSource<A>(Channel<A> Channel) : Source<A>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token)
    {
        while(await Channel.Reader.WaitToReadAsync(token))
        {
            var item = await Channel.Reader.ReadAsync(token);
            switch (await reducer(state, item))
            {
                case { Continue: true, Value: var value }:
                    state = value;
                    break;
                
                case { Value: var value }:
                    return Reduced.Done(value);
            }
        }
        return Reduced.Continue(state);
    }
}
