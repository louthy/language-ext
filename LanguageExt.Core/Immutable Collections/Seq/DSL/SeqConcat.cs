using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.UnsafeValueAccess;

namespace LanguageExt;

internal class SeqConcat<A>(Seq<ISeqInternal<A>> ms) : ISeqInternal<A>
{
    internal readonly Seq<ISeqInternal<A>> ms = ms;
    
    int selfHash;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<A> AsSpan() =>
        Strict().AsSpan();

    public A this[int index]
    {
        get
        {
            var r = At(index);
            if (r.IsSome) return r.Value!;
            throw new IndexOutOfRangeException();
        }
    }

    public Option<A> At(int index)
    {
        if (index < 0) return default;
        var ms1 = ms;
        while (!ms1.IsEmpty)
        {
            var head = ms1.Head.ValueUnsafe() ?? throw new InvalidOperationException();
            var r    = head.At(index);
            if (r.IsSome) return r;
            index -= head.Count;
            ms1 = ms1.Tail;
        }
        return default;
    }

    public SeqType Type =>
        SeqType.Concat;

    public A Head 
    {
        get 
        {
            foreach (var s in ms)
            {
                foreach (var a in s)
                {
                    return a;
                }
            } 
            throw Exceptions.SequenceEmpty;
        }
    }

    public ISeqInternal<A> Tail =>
        new SeqLazy<A>(Skip(1));

    public bool IsEmpty => 
        ms.ForAll(s => s.IsEmpty);

    public ISeqInternal<A> Init
    {
        get
        {
            var take = Count - 1;
            return take <= 0
                       ? SeqEmptyInternal<A>.Default
                       : Take(take);
        }
    }

    public A Last
    {
        get 
        {
            foreach (var s in ms.Reverse())
            {
                foreach (var a in s.Reverse())
                {
                    return a;
                }
            } 
            throw Exceptions.SequenceEmpty;
        }
    }

    public int Count => 
        ms.Sum(s => s.Count);

    public SeqConcat<A> AddSeq(ISeqInternal<A> ma) =>
        new (ms.Add(ma));

    public SeqConcat<A> AddSeqRange(Seq<ISeqInternal<A>> ma) =>
        new (ms.Concat(ma));

    public SeqConcat<A> ConsSeq(ISeqInternal<A> ma) =>
        new (ma.Cons(ms));

    public ISeqInternal<A> Add(A value)
    {
        var last = ms.Last.ValueUnsafe()?.Add(value) ?? throw new NotSupportedException();
        return new SeqConcat<A>(ms.Take(ms.Count - 1).Add(last));
    }

    public ISeqInternal<A> Cons(A value)
    {
        var head = ms.Head.ValueUnsafe()?.Cons(value) ?? throw new NotSupportedException();
        return new SeqConcat<A>(head.Cons(ms.Skip(1)));
    }

    public IEnumerator<A> GetEnumerator()
    {
        foreach(var s in ms)
        {
            foreach(var a in s)
            {
                yield return a;
            }
        }
    }

    public Unit Iter(Action<A> f)
    {
        foreach (var s in ms)
        {
            foreach (var a in s)
            {
                f(a);
            }
        }
        return default;
    }

    public ISeqInternal<A> Skip(int amount) =>
        new SeqLazy<A>(((IEnumerable<A>)this).Skip(amount));

    public ISeqInternal<A> Strict()
    {
        foreach(var s in ms)
        {
            s.Strict();
        }
        return this;
    }

    public ISeqInternal<A> Take(int amount)
    {
        IEnumerable<A> Yield()
        {
            using var iter = GetEnumerator();
            for(; amount > 0 && iter.MoveNext(); amount--)
            {
                yield return iter.Current;
            }
        }
        return new SeqLazy<A>(Yield());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InitFoldState(ref Seq.FoldState state) =>
        // ReSharper disable once GenericEnumeratorNotDisposed
        Seq.FoldState.FromEnumerator(ref state, GetEnumerator());

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var s in ms)
        {
            foreach (var a in s)
            {
                yield return a;
            }
        }
    }

    ISeqInternal<A> Flatten()
    {
        var total = 0;
        foreach (var s in ms)
        {
            s.Strict();
            total = s.Count;
        }

        var cap = 8;
        while(cap < total)
        {
            cap <<= 1;
        }

        var data    = new A[cap];
        var start   = (cap - total) >> 1;
        var current = start;

        foreach(var s in ms)
        {
            var strict = (SeqStrict<A>)s;
            Array.Copy(strict.data, strict.start, data, current, strict.count);
            current += strict.count;
        }
        return new SeqStrict<A>(data, start, total, 0, 0);
    }
        

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() =>
        selfHash == 0
            ? selfHash = GetHashCode(FNV32.OffsetBasis)
            : selfHash;        

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(int hash)
    {
        foreach (var seq in ms)
        {
            hash = seq.GetHashCode(hash);
        }
        return hash;
    }
}
