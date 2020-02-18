using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
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
        ISeqInternal<A> Add(A value);
        ISeqInternal<A> Cons(A value);
        A Head { get; }
        ISeqInternal<A> Tail { get; }
        bool IsEmpty { get; }
        ISeqInternal<A> Init { get; }
        A Last { get; }
        int Count { get; }
        S Fold<S>(S state, Func<S, A, S> f);
        S FoldBack<S>(S state, Func<S, A, S> f);
        ISeqInternal<A> Skip(int amount);
        ISeqInternal<A> Take(int amount);
        ISeqInternal<A> Strict();
        ISeqInternal<A> Filter(Func<A, bool> f);
        ISeqInternal<B> Map<B>(Func<A, B> f);
        Unit Iter(Action<A> f);
        bool Exists(Func<A, bool> f);
        bool ForAll(Func<A, bool> f);
        int GetHashCode(int offsetBasis);
    }
}
