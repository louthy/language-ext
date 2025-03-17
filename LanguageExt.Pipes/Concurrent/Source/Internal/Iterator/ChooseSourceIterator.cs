using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSourceIterator<A>(SourceIterator<A> Left, SourceIterator<A> Right) : SourceIterator<A>
{
    internal override ValueTask<A> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<A>(Errors.Cancelled);
        var tl = Left.ReadValue(token);
        if (tl.IsCompleted)
        {
            if (tl.IsCompletedSuccessfully) return tl;
            return Right.ReadValue(token);
        }
        else
        {
            return ReadValueAsync(tl.AsTask(), token);
        }
    }

    async ValueTask<A> ReadValueAsync(Task<A> left, CancellationToken token)
    {
        try
        {
            return await left;
        }
        catch (Exception)
        {
            return await Right.ReadValue(token);
        }
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        SourceInternal.ReadyToRead([Left, Right], token);
}
