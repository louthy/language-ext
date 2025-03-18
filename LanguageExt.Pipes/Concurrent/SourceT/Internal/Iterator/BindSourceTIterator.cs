using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record BindSourceTIterator<M, A, B>(SourceTIterator<M, A> SourceT, Func<A, SourceTIterator<M, B>> F) : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    K<M, SourceTIterator<M, B>>? Current = null;

    public override K<M, B> Read()
    {
        return IO.token.Bind(go);
        
        K<M, B> go(CancellationToken token)
        {
            if (Current is null)
            {
                var ta = SourceT.Read();
                Current = ta.Map(F);
            }

            // Use of GetAwaiter().GetResult() here is knowing that we will already be in the middle of 
            // an async/await, so this shouldn't block.  You can instead use the commented the code below,
            // which is more 'correct', but will be less efficient:
            //
            //      Current.Bind(c => c.ReadValue())
            //             .Choose(() => M.LiftIO(IO.liftVAsync(e => MoveNext(e.Token))).Flatten());
            //
            return Current.Bind(c => c.Read())
                          .Choose(() => MoveNext(token).GetAwaiter().GetResult());
        }
    }

    async ValueTask<K<M, B>> MoveNext(CancellationToken token)
    {
        if(!await SourceT.ReadyToRead(token)) return M.Empty<B>();
        Current = SourceT.Read().Map(F);
        
        // Use of GetAwaiter().GetResult() here is knowing that we will already be in the middle of 
        // an async/await, so this shouldn't block.  You can instead use the commented the code below,
        // which is more 'correct', but will be less efficient:
        //
        //     Current.Bind(c => c.ReadValue(e.Token))
        //            .Choose(() => M.LiftIO(IO.liftVAsync(e => MoveNext(e.Token))).Flatten());
        //
        return Current.Bind(c => c.Read())
                      .Choose(() => MoveNext(token).GetAwaiter().GetResult());
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await SourceT.ReadyToRead(token);
}
