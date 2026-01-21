using System;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt;

[Serializable]
class QueInternal<A> : IEnumerable<A>
{
    public static readonly QueInternal<A> Empty = new ();

    readonly Stck<A> forward;
    readonly Stck<A> backward;
    Stck<A>? backwardRev;
    int hashCode;

    internal QueInternal()
    {
        forward = Stck<A>.Empty;
        backward = Stck<A>.Empty;
    }

    internal QueInternal(IEnumerable<A> items)
    {
        var q = new QueInternal<A>();
        foreach(var item in items)
        {
            q = q.Enqueue(item);
        }
        forward = q.forward;
        backward = q.backward;
        backwardRev = q.backwardRev;
    }

    internal QueInternal(ReadOnlySpan<A> items)
    {
        var q = new QueInternal<A>();
        foreach(var item in items)
        {
            q = q.Enqueue(item);
        }
        forward = q.forward;
        backward = q.backward;
        backwardRev = q.backwardRev;
    }

    QueInternal(Stck<A> f, Stck<A> b)
    {
        forward = f;
        backward = b;
    }

    Stck<A> BackwardRev =>
        backwardRev ??= backward.Reverse();

    public long Count =>
        forward.Count + backward.Count;

    public bool IsEmpty =>
        forward.IsEmpty && backward.IsEmpty;

    public QueInternal<A> Clear() =>
        Empty;

    public Option<A> Peek() =>
        forward.Peek();

    public A PeekUnsafe() =>
        forward.PeekUnsafe();

    public QueInternal<A> Dequeue()
    {
        var f = forward.Pop();
        if (!f.IsEmpty)
        {
            return new QueInternal<A>(f, backward);
        }
        if (backward.IsEmpty)
        {
            return Empty;
        }
        return new QueInternal<A>(BackwardRev, Stck<A>.Empty);
    }

    public QueInternal<A> Dequeue(out A outValue)
    {
        var ov = Peek();
        if (ov.IsSome)
        {
            outValue = (A)ov.Value!;
            return Dequeue();
        }
        else
        {
            throw new InvalidOperationException("Queue is empty");
        }
    }

    public (QueInternal<A>, Option<A>) TryDequeue() =>
        forward.TryPeek(out var x)
            ? (Dequeue(), Some(x))
            : (this, Option<A>.None);

    public bool TryPeek(out A value) =>
        forward.TryPeek(out value);

    public QueInternal<A> Enqueue(A value) =>
        IsEmpty
            ? new QueInternal<A>(Stck<A>.Empty.Push(value), Stck<A>.Empty)
            : new QueInternal<A>(forward, backward.Push(value));

    public Seq<A> ToSeq() =>
        toSeq(forward.AsIterable().ConcatFast(BackwardRev));

    public Iterable<A> AsIterable() =>
        forward.AsIterable().ConcatFast(BackwardRev).AsIterable();

    public IEnumerator<A> GetEnumerator() =>
        forward.AsIterable().ConcatFast(BackwardRev).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        forward.AsIterable().ConcatFast(BackwardRev).GetEnumerator();

    public Iterator<A> GetIterator() =>
        forward.ForwardIterator() + backward.Reverse().ForwardIterator();

    public static QueInternal<A> operator +(QueInternal<A> lhs, QueInternal<A> rhs) =>
        lhs.Combine(rhs);

    public QueInternal<A> Combine(QueInternal<A> rhs)
    {
        var self = this;
        foreach (var item in rhs)
        {
            self = self.Enqueue(item);
        }
        return self;
    }

    public override int GetHashCode() =>
        hashCode == 0
            ? hashCode = FNV32.Hash<HashableDefault<A>, A>(this)
            : hashCode;
}
