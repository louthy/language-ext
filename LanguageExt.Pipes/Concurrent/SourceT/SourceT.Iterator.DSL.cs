using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

public abstract record SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public abstract K<M, A> Read();
    internal abstract ValueTask<bool> ReadyToRead(CancellationToken token);
}

record TransformSourceTIterator<M, A, B>(SourceTIterator<M, A> SourceT, Transducer<A, B> Transducer) 
    : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, B> Read()
    {
        var mvalue = SourceT.Read();
        var result = mvalue.Bind(v => Transducer.ReduceM<M, Option<B>>(reduce)(None, v))
                           .Bind(mb => mb.IsSome ? M.Pure((B)mb) : M.Empty<B>()); 
        return result;
        
        static K<M, Option<B>> reduce(Option<B> _, B x) => 
            M.Pure(Optional(x));
    }
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        SourceT.ReadyToRead(token);
}

record SingletonSourceTIterator<M, A>(A Value) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    volatile int read;

    public override K<M, A> Read() =>
        Interlocked.CompareExchange(ref read, 1, 0) == 0
            ? M.Pure(Value)
            : M.Empty<A>();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(read == 0);
}

record LiftSourceTIterator<M, A>(K<M, A> Value) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    volatile int read;

    public override K<M, A> Read() =>
        Interlocked.CompareExchange(ref read, 1, 0) == 0
            ? Value
            : M.Empty<A>();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(read == 0);
}

record ForeverSourceTIterator<M, A>(K<M, A> Value) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, A> Read() =>
        Value;

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(true);
}

record EmptySourceTIterator<M, A> : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public static readonly SourceTIterator<M, A> Default = new EmptySourceTIterator<M, A>();

    public override K<M, A> Read() => 
        M.Empty<A>();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(false);
}

record ReaderSourceTIterator<M, A>(ChannelReader<K<M, A>> Reader) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, A> Read()
    {
        return IO.token.BindAsync(go);
        
        async ValueTask<K<M, A>> go(CancellationToken token)
        {
            if(await Reader.WaitToReadAsync(token))
            {
                return await Reader.ReadAsync(token);
            }
            else
            {
                return M.Empty<A>();
            }
        }
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Reader.WaitToReadAsync(token);
}

record ReaderPureSourceTIterator<M, A>(ChannelReader<A> Reader) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, A> Read()
    {
        return IO.token.BindAsync(go);
        
        async ValueTask<K<M, A>> go(CancellationToken token)
        {
            if(await Reader.WaitToReadAsync(token))
            {
                return M.Pure(await Reader.ReadAsync(token));
            }
            else
            {
                return M.Empty<A>();
            }
        }
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Reader.WaitToReadAsync(token);
}

record ApplySourceTIterator<M, A, B>(SourceTIterator<M, Func<A, B>> FF, SourceTIterator<M, A> FA) : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, B> Read() =>
        FF.Read().Apply(FA.Read());

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        await FA.ReadyToRead(token) && await FF.ReadyToRead(token);
}

record BindSourceTIterator<M, A, B>(SourceTIterator<M, A> SourceT, Func<A, SourceTIterator<M, B>> F) : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    K<M, SourceTIterator<M, B>>? Current = null;

    public override K<M, B> Read()
    {
        return IO.token.BindAsync(go);
        
        async ValueTask<K<M, B>> go(CancellationToken token)
        {
            if (token.IsCancellationRequested) throw new TaskCanceledException();
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

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        SourceT.ReadyToRead(token);
}
record CombineSourceTIterator<M, A>(Seq<SourceTIterator<M, A>> SourceTs) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if (SourceTs.Count == 0) return new(false);
        if (SourceTs.Count == 1) return SourceTs[0].ReadyToRead(token);
        return SourceTInternal.ReadyToRead(SourceTs, token);
    }

    public override K<M, A> Read()
    {
        if (SourceTs.Count == 0) return M.Empty<A>();
        if (SourceTs.Count == 1) return SourceTs[0].Read();
        return IO.token.BindAsync(t => SourceTInternal.Read(SourceTs, t));
    }
}

record ChooseSourceTIterator<M, A>(SourceTIterator<M, A> Left, SourceTIterator<M, A> Right) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, A> Read()
    {
        try
        {
            return Left.Read().Choose(() => Right.Read());
        }
        catch
        {
            return Right.Read();
        }
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        SourceTInternal.ReadyToRead([Left, Right], token);
}

record Zip2SourceTIterator<M, A, B>(SourceTIterator<M, A> SourceTA, SourceTIterator<M, B> SourceTB)
    : SourceTIterator<M, (A First, B Second)>
    where M : Monad<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var a = await SourceTA.ReadyToRead(token);
        var b = await SourceTB.ReadyToRead(token);
        return a && b;
    }

    public override K<M, (A First, B Second)> Read() =>
        SourceTA.Read().Zip(SourceTB.Read());
}

record Zip3SourceTIterator<M, A, B, C>(
    SourceTIterator<M, A> SourceTA, 
    SourceTIterator<M, B> SourceTB, 
    SourceTIterator<M, C> SourceTC) :
    SourceTIterator<M, (A First, B Second, C Third)>
    where M : Monad<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var a = await SourceTA.ReadyToRead(token);
        var b = await SourceTB.ReadyToRead(token);
        var c = await SourceTC.ReadyToRead(token);
        return a && b && c;
    }

    public override K<M, (A First, B Second, C Third)> Read() => 
        SourceTA.Read().Zip(SourceTB.Read(), SourceTC.Read());
}

record Zip4SourceTIterator<M, A, B, C, D>(
    SourceTIterator<M, A> SourceTA, 
    SourceTIterator<M, B> SourceTB, 
    SourceTIterator<M, C> SourceTC, 
    SourceTIterator<M, D> SourceTD)
    : SourceTIterator<M, (A First, B Second, C Third, D Fourth)>
    where M : Monad<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var a = await SourceTA.ReadyToRead(token);
        var b = await SourceTB.ReadyToRead(token);
        var c = await SourceTC.ReadyToRead(token);
        var d = await SourceTD.ReadyToRead(token);

        return a && b && c && d;
    }

    public override K<M, (A First, B Second, C Third, D Fourth)> Read() =>
        SourceTA.Read().Zip(SourceTB.Read(), SourceTC.Read(), SourceTD.Read());
}

record IteratorSyncSourceTIterator<M, A> : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal required Iterator<K<M, A>> Src;

    public override K<M, A> Read()
    {
        if (Src.IsEmpty) return M.Empty<A>();
        var head = Src.Head;
        Src = Src.Tail.Split();
        return head;
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new (!Src.IsEmpty);
}

record IteratorAsyncSourceTIterator<M, A> : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal required IteratorAsync<K<M, A>> Src;

    public override K<M, A> Read()
    {
        return IO.token.BindAsync(go);
        async ValueTask<K<M, A>> go(CancellationToken token)
        {
            if (token.IsCancellationRequested) throw Errors.Cancelled;
            if (await Src.IsEmpty) return M.Empty<A>();
            var head = await Src.Head;
            Src = (await Src.Tail).Split();
            return head;
        }
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !await Src.IsEmpty;
}
