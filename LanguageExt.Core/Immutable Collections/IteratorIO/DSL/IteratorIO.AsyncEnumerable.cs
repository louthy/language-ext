#pragma warning disable CS0660, CS0661
using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Enumerable IteratorIO
    /// </summary>
    internal class AsyncEnumerable(IAsyncEnumerable<A> enumerable) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() 
        {
            return IO.liftVAsync(go);
            async ValueTask<(Head<A> Head, IteratorIO<A> Tail)> go(EnvIO e)
            {
                var enumerator = enumerable.GetAsyncEnumerator(e.Token);
                if (await enumerator.MoveNextAsync())
                {
                    return Head.ExistIO(enumerator.Current, new AsyncEnumeratorTail(new AsyncEn(enumerator)));
                }
                else
                {
                    await enumerator.DisposeAsync();
                    return Head.NilIO<A>();
                }
            }
        }

        public override string ToString() =>
            "...";

        public override IteratorIO<A> Using()
        {
            var enumerator = enumerable.GetAsyncEnumerator();
            return new AsyncEnumeratorTail(new AsyncEn(enumerator));
        }
    }
    
    /// <summary>
    /// Enumerator IteratorIO
    /// </summary>
    internal class AsyncEnumeratorTail(AsyncEn enumerator) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO()
        {
            return IO.liftVAsync(go);
            async ValueTask<(Head<A> Head, IteratorIO<A> Tail)> go(EnvIO e)
            {
                if(e.Token.IsCancellationRequested) throw new OperationCanceledException();
                if (!enumerator.Disposed && await enumerator.Enumerator.MoveNextAsync())
                {
                    return Head.ExistIO(enumerator.Enumerator.Current, new AsyncEnumeratorTail(enumerator));
                }
                else
                {
                    await enumerator.DisposeAsync();
                    return Head.NilIO<A>();
                }
            }
        }

        public override IteratorIO<A> Using() => 
            this;

        public override void Dispose() =>
            Async.await(enumerator.DisposeAsync());

        public override string ToString() => 
            "...";
    }

    /// <summary>
    /// Simple type to carry the enumerator and handle disposal. It allows `Dispose` to be
    /// called many times (because there could be umpteen references to it, so let the
    /// devs be overzealous with their clean-up)
    /// </summary>
    internal class AsyncEn(IAsyncEnumerator<A> enumerator) : IAsyncDisposable
    {
        int disposed;
        public readonly IAsyncEnumerator<A> Enumerator = enumerator;

        public bool Disposed =>
            disposed == 1;

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
            {
                await Enumerator.DisposeAsync();
            }
        }
    }
}
