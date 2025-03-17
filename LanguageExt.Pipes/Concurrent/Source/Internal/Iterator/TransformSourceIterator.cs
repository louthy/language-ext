using System.Threading;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record TransformSourceIterator<A, B>(SourceIterator<A> Source, Transducer<A, B> Transducer) 
    : SourceIterator<B>
{
    SourceIterator<A>? src;
    B? Value;

    internal override ValueTask<B> ReadValue(CancellationToken token) => 
        Value is null
            ? ValueTask.FromException<B>(Errors.SourceClosed)
            : new(Value);

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        src = src ?? Source;
        while (true)
        {
            if (!await src.ReadyToRead(token)) return false;
            var value  = await Source.ReadValue(token);
            var result = await Transducer.Reduce<Option<B>>(reduce)(None, value);
            if (!result.Continue) src = EmptySourceIterator<A>.Default;
            if (result.Value.IsNone) continue;
            Value = (B)result.Value;
            return true;
        }

        static ValueTask<Reduced<Option<B>>> reduce(Option<B> _, B x) => 
            Reduced.ContinueAsync(Optional(x));
    }
}
