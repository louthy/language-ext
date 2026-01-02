using System;
using System.Collections.Generic;

namespace LanguageExt;

internal enum SeqType
{
    Empty,
    Lazy,
    Strict,
    Concat
}

internal interface ISeqInternal<A> : IEnumerable<A>
{
    SeqType Type { get; }
    A this[int index] { get; }
    Option<A> At(int index);
    ISeqInternal<A> Add(A value);
    ISeqInternal<A> Cons(A value);
    A Head { get; }
    ISeqInternal<A> Tail { get; }
    bool IsEmpty { get; }
    ISeqInternal<A> Init { get; }
    A Last { get; }
    int Count { get; }
    ISeqInternal<A> Skip(int amount);
    ISeqInternal<A> Take(int amount);
    ISeqInternal<A> Strict();
    int GetHashCode(int offsetBasis);
    ReadOnlySpan<A> AsSpan();
    void InitFoldState(ref Seq.FoldState state);
}
