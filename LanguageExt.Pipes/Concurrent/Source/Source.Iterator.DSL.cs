using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

public abstract record SourceIterator<A>
{
    public IO<A> Read() => 
        IO.liftVAsync(e => ReadValue(e.Token));
    
    internal abstract ValueTask<A> ReadValue(CancellationToken token);
    internal abstract ValueTask<bool> ReadyToRead(CancellationToken token);
}

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

record SingletonSourceIterator<A>(A Value) : SourceIterator<A>
{
    volatile int read;

    internal override ValueTask<A> ReadValue(CancellationToken token) =>
        Interlocked.CompareExchange(ref read, 1, 0) == 0
            ? new (Value)
            : ValueTask.FromException<A>(Errors.SourceClosed);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(read == 0);
}

record ForeverSourceIterator<A>(A Value) : SourceIterator<A>
{
    internal override ValueTask<A> ReadValue(CancellationToken token) =>
        new (Value);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(true);
}

record EmptySourceIterator<A> : SourceIterator<A>
{
    public static readonly SourceIterator<A> Default = new EmptySourceIterator<A>();

    internal override ValueTask<A> ReadValue(CancellationToken token) => 
        ValueTask.FromException<A>(Errors.SourceClosed);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(false);
}

record ReaderSourceIterator<A>(ChannelReader<A> Reader, string Label) : SourceIterator<A>
{
    internal override ValueTask<A> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<A>(Errors.Cancelled);
        var tf = Reader.WaitToReadAsync(token);
        if (tf.IsCompleted)
        {
            if (tf.Result)
            {
                return Reader.ReadAsync(token);
            }
            else
            {
                return ValueTask.FromException<A>(Errors.SourceClosed);
            }
        }
        return ReadValueAsync(tf, token);
    }

    async ValueTask<A> ReadValueAsync(ValueTask<bool> tflag, CancellationToken token)
    {
        var f = await tflag;
        if (f)
        {
            return await Reader.ReadAsync(token);
        }
        else
        {
            throw Errors.SourceClosed;
        }
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Reader.WaitToReadAsync(token);
}

record ApplySourceIterator<A, B>(SourceIterator<Func<A, B>> FF, SourceIterator<A> FA) : SourceIterator<B>
{
    internal override ValueTask<B> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<B>(Errors.Cancelled);
        var ta = FA.ReadValue(token);
        var tf = FF.ReadValue(token);
        if(ta.IsCompleted && tf.IsCompleted) return new (tf.Result(ta.Result));
        return ReadValue(tf.AsTask(), ta.AsTask());
    }

    static async ValueTask<B> ReadValue(Task<Func<A, B>> tf, Task<A> ta)
    {
        await Task.WhenAll(tf, ta);
        return tf.Result(ta.Result);
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        await FA.ReadyToRead(token) && await FF.ReadyToRead(token);
}

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
record CombineSourceIterator<A>(Seq<SourceIterator<A>> Sources) : SourceIterator<A>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if (Sources.Count == 0) return new ValueTask<bool>(false);
        if (Sources.Count == 1) return Sources[0].ReadyToRead(token);
        return SourceInternal.ReadyToRead(Sources, token);
    }

    internal override ValueTask<A> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<A>(Errors.Cancelled);
        if (Sources.Count == 0) return ValueTask.FromException<A>(Errors.SourceClosed);
        if (Sources.Count == 1) return Sources[0].ReadValue(token);
        return SourceInternal.Read(Sources, token);
    }
}

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

record Zip2SourceIterator<A, B>(SourceIterator<A> SourceA, SourceIterator<B> SourceB)
    : SourceIterator<(A First, B Second)>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var ta = SourceA.ReadyToRead(token);
        var tb = SourceB.ReadyToRead(token);

        if (ta.IsCompleted && tb.IsCompleted)
            return new(ta.Result && tb.Result);

        return ReadyToReadAsync(ta.AsTask(), tb.AsTask());
    }

    async ValueTask<bool> ReadyToReadAsync(Task<bool> ta, Task<bool> tb)
    {
        await Task.WhenAll(ta, tb);
        return ta.Result && tb.Result;
    }

    internal override ValueTask<(A First, B Second)> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<(A First, B Second)>(Errors.Cancelled);
        var ta = SourceA.ReadValue(token);
        var tb = SourceB.ReadValue(token);

        if (ta.IsCompleted && tb.IsCompleted)
            return new ValueTask<(A First, B Second)>((ta.Result, tb.Result));

        return ReadValueAsync(ta.AsTask(), tb.AsTask());
    }

    async ValueTask<(A First, B Second)> ReadValueAsync(Task<A> ta, Task<B> tb)
    {
        await Task.WhenAll(ta, tb);
        return (ta.Result, tb.Result);
    }
}

record Zip3SourceIterator<A, B, C>(SourceIterator<A> SourceA, SourceIterator<B> SourceB, SourceIterator<C> SourceC) :
    SourceIterator<(A First, B Second, C Third)>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var ta = SourceA.ReadyToRead(token);
        var tb = SourceB.ReadyToRead(token);
        var tc = SourceC.ReadyToRead(token);

        if (ta.IsCompleted && tb.IsCompleted && tc.IsCompleted)
            return new(ta.Result && tb.Result && tc.Result);

        return ReadyToReadAsync(ta.AsTask(), tb.AsTask(), tc.AsTask());
    }

    async ValueTask<bool> ReadyToReadAsync(Task<bool> ta, Task<bool> tb, Task<bool> tc)
    {
        await Task.WhenAll(ta, tb, tc);
        return ta.Result && tb.Result && tc.Result;
    }

    internal override ValueTask<(A First, B Second, C Third)> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<(A First, B Second, C Third)>(Errors.Cancelled);
        var ta = SourceA.ReadValue(token);
        var tb = SourceB.ReadValue(token);
        var tc = SourceC.ReadValue(token);

        if (ta.IsCompleted && tb.IsCompleted && tc.IsCompleted)
            return new ValueTask<(A First, B Second, C Third)>((ta.Result, tb.Result, tc.Result));

        return ReadValueAsync(ta.AsTask(), tb.AsTask(), tc.AsTask());
    }

    async ValueTask<(A First, B Second, C Third)> ReadValueAsync(Task<A> ta, Task<B> tb, Task<C> tc)
    {
        await Task.WhenAll(ta, tb, tc);
        return (ta.Result, tb.Result, tc.Result);
    }
}

record Zip4SourceIterator<A, B, C, D>(SourceIterator<A> SourceA, SourceIterator<B> SourceB, SourceIterator<C> SourceC, SourceIterator<D> SourceD)
    : SourceIterator<(A First, B Second, C Third, D Fourth)>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var ta = SourceA.ReadyToRead(token);
        var tb = SourceB.ReadyToRead(token);
        var tc = SourceC.ReadyToRead(token);
        var td = SourceD.ReadyToRead(token);

        if (ta.IsCompleted && tb.IsCompleted && tc.IsCompleted && td.IsCompleted)
            return new(ta.Result && tb.Result && tc.Result && td.Result);

        return ReadyToReadAsync(ta.AsTask(), tb.AsTask(), tc.AsTask(), td.AsTask());
    }

    async ValueTask<bool> ReadyToReadAsync(Task<bool> ta, Task<bool> tb, Task<bool> tc, Task<bool> td)
    {
        await Task.WhenAll(ta, tb, tc, td);
        return ta.Result && tb.Result && tc.Result && td.Result;
    }

    internal override ValueTask<(A First, B Second, C Third, D Fourth)> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<(A First, B Second, C Third, D Fourth)>(Errors.Cancelled);
        var ta = SourceA.ReadValue(token);
        var tb = SourceB.ReadValue(token);
        var tc = SourceC.ReadValue(token);
        var td = SourceD.ReadValue(token);

        if (ta.IsCompleted && tb.IsCompleted && tc.IsCompleted && td.IsCompleted)
            return new ValueTask<(A First, B Second, C Third, D Fourth)>((ta.Result, tb.Result, tc.Result, td.Result));

        return ReadValueAsync(ta.AsTask(), tb.AsTask(), tc.AsTask(), td.AsTask());
    }

    async ValueTask<(A First, B Second, C Third, D Fourth)> ReadValueAsync(Task<A> ta, Task<B> tb, Task<C> tc, Task<D> td)
    {
        await Task.WhenAll(ta, tb, tc, td);
        return (ta.Result, tb.Result, tc.Result, td.Result);
    }
}

record IteratorSyncSourceIterator<A> : SourceIterator<A>
{
    internal required Iterator<A> Src;

    internal override ValueTask<A> ReadValue(CancellationToken token)
    {
        if (token.IsCancellationRequested) return ValueTask.FromException<A>(Errors.Cancelled);
        var state = Src.Clone();
        if (state.IsEmpty) return ValueTask.FromException<A>(Errors.SourceClosed);
        Src = state.Tail.Split();
        return new(state.Head);
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new (!Src.IsEmpty);
}

record IteratorAsyncSourceIterator<A> : SourceIterator<A>
{
    internal required IteratorAsync<A> Src;

    internal override async ValueTask<A> ReadValue(CancellationToken token)
    {
        if (token.IsCancellationRequested) throw Errors.Cancelled;
        var state = Src.Clone();
        if (await state.IsEmpty) throw Errors.SourceClosed;
        Src = (await state.Tail).Split();
        return await state.Head;
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !await Src.IsEmpty;
}
