using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    internal class SeqEmptyInternal<A> : ISeqInternal<A>
    {
        public static ISeqInternal<A> Default = new SeqEmptyInternal<A>();

        public A this[int index] => 
            throw new IndexOutOfRangeException();

        public A Head =>
            throw new InvalidOperationException("Sequence is empty");

        public ISeqInternal<A> Tail =>
            this;

        public bool IsEmpty => 
            true;

        public ISeqInternal<A> Init =>
            this;

        public A Last =>
            throw new InvalidOperationException("Sequence is empty");

        public int Count => 
            0;

        public ISeqInternal<A> Add(A value) =>
            SeqStrict<A>.FromSingleValue(value);

        public ISeqInternal<A> Cons(A value) =>
            SeqStrict<A>.FromSingleValue(value);

        public S Fold<S>(S state, Func<S, A, S> f) =>
            state;

        public S FoldBack<S>(S state, Func<S, A, S> f) =>
            state;

        public ISeqInternal<A> Skip(int amount) =>
            this;

        public ISeqInternal<A> Strict() =>
            this;

        public ISeqInternal<A> Take(int amount) =>
            this;
        
        public IEnumerator<A> GetEnumerator()
        {
            yield break;
        }
            
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }

        public Unit Iter(Action<A> f) =>
            default;

        public bool Exists(Func<A, bool> f) => 
            false;

        public bool ForAll(Func<A, bool> f) =>
            true;

        public SeqType Type => SeqType.Empty;

        public override int GetHashCode() =>
            FNV32.OffsetBasis;

        public int GetHashCode(int offsetBasis) =>
            offsetBasis;
    }
}
