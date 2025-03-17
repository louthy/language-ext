 using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record BindSourceIterator<A, B>(SourceIterator<A> Source, Func<A, SourceIterator<B>> F) : SourceIterator<B>
{
    SourceIterator<B>? Current = null;
    
    internal override async ValueTask<B> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) Errors.Cancelled.Throw();
        if (Current is null)
        {
            if (await ReadyToRead(token))
            {
                return await Current!.ReadValue(token);
            }
            else
            {
                throw new InvalidOperationException("Call `ReadyToRead` before `ReadValue`");
            }
        }
        else
        {
            return await Current.ReadValue(token);
        }
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if (Current is null)
        {
            while (true)
            {
                if (await Source.ReadyToRead(token))
                {
                    Current = F(await Source.ReadValue(token));
                    if (await Current.ReadyToRead(token))
                    {
                        return true;
                    }
                }
                else
                {
                    Current = null;
                    return false;
                }
            }
        }
        else
        {
            var res = await Current.ReadyToRead(token);
            Current = res ? Current : null;
            return res;
        }
    }
}
