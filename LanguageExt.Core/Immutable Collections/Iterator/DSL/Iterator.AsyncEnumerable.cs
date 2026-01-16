using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Enumerable iterator
    /// </summary>
    internal class AsyncEnumerable(IAsyncEnumerable<A> enumerable) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var enumerator = enumerable.GetAsyncEnumerator();
            if (Async.await(enumerator.MoveNextAsync()))
            {
                return Head.Exist(enumerator.Current, new AsyncEnumeratorTail(new AsyncEn(enumerator)));
            }
            else
            {
                Async.await(enumerator.DisposeAsync());
                return Head.Nil<A>();
            }
        }

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() 
        {
            return IO.liftVAsync(go);
            async ValueTask<(Head<A> Head, Iterator<A> Tail)> go(EnvIO e)
            {
                var enumerator = enumerable.GetAsyncEnumerator(e.Token);
                if (await enumerator.MoveNextAsync())
                {
                    return Head.Exist(enumerator.Current, new AsyncEnumeratorTail(new AsyncEn(enumerator)));
                }
                else
                {
                    await enumerator.DisposeAsync();
                    return Head.Nil<A>();
                }
            }
        }

        public override string ToString() =>
            "...";

        public override Iterator<A> Using()
        {
            var enumerator = enumerable.GetAsyncEnumerator();
            return new AsyncEnumeratorTail(new AsyncEn(enumerator));
        }
    }
    
    /// <summary>
    /// Enumerator iterator
    /// </summary>
    internal class AsyncEnumeratorTail(AsyncEn enumerator) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            if (!enumerator.Disposed && Async.await(enumerator.Enumerator.MoveNextAsync()))
            {
                return Head.Exist(enumerator.Enumerator.Current, new AsyncEnumeratorTail(enumerator));
            }
            else
            {
                Async.await(enumerator.DisposeAsync());
                return Head.Nil<A>();
            }
        }

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO()
        {
            return IO.liftVAsync(go);
            async ValueTask<(Head<A> Head, Iterator<A> Tail)> go(EnvIO e)
            {
                if(e.Token.IsCancellationRequested) throw new OperationCanceledException();
                if (!enumerator.Disposed && await enumerator.Enumerator.MoveNextAsync())
                {
                    return Head.Exist(enumerator.Enumerator.Current, new AsyncEnumeratorTail(enumerator));
                }
                else
                {
                    await enumerator.DisposeAsync();
                    return Head.Nil<A>();
                }
            }
        }

        public override Iterator<A> Using() => 
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
