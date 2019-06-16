using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    internal interface ISeqInternal<A> : IEnumerable<A>
    {
        A this[int index] { get; }
        ISeqInternal<A> Add(A value);
        ISeqInternal<A> Cons(A value);
        A Head { get; }
        ISeqInternal<A> Tail { get; }
        bool IsEmpty { get; }
        A Last { get; }
        int Count { get; }
        S Fold<S>(S state, Func<S, A, S> f);
        S FoldBack<S>(S state, Func<S, A, S> f);
        ISeqInternal<A> Skip(int amount);
        ISeqInternal<A> Take(int amount);
        ISeqInternal<A> Strict();
    }
}
