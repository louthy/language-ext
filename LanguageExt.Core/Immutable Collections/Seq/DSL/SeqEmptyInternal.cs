using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt.Common;

namespace LanguageExt;

class SeqEmptyInternal<A> : ISeqInternal<A>
{
    public static readonly ISeqInternal<A> Default = new SeqEmptyInternal<A>();

    public ReadOnlySpan<A> AsSpan() =>
        ReadOnlySpan<A>.Empty;

    public Seq.FoldState InitFoldState() => 
        Seq.FoldState.FromSpan(AsSpan());

    public A this[long index] => 
        throw new IndexOutOfRangeException();

    public Option<A> At(long index) => 
        default;

    public A Head =>
        throw Exceptions.SequenceEmpty;

    public ISeqInternal<A> Tail =>
        this;

    public bool IsEmpty => 
        true;

    public ISeqInternal<A> Init =>
        this;

    public A Last =>
        throw Exceptions.SequenceEmpty;

    public long Count => 
        0;

    public ISeqInternal<A> Add(A value) =>
        SeqStrict<A>.FromSingleValue(value);

    public ISeqInternal<A> Cons(A value) =>
        SeqStrict<A>.FromSingleValue(value);

    public ISeqInternal<A> Skip(long amount) =>
        this;

    public ISeqInternal<A> Strict() =>
        this;

    public ISeqInternal<A> Take(long amount) =>
        this;

    public Iterator<A> GetIterator() => 
        Iterator<A>.Empty;

    public IteratorEnumerator<A> GetEnumerator() => 
        new (Iterator<A>.Empty);

    public SeqType Type => SeqType.Empty;

    public override int GetHashCode() =>
        FNV32.OffsetBasis;

    public int GetHashCode(int offsetBasis) =>
        offsetBasis;
}
