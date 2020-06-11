using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageExt
{
    internal class SeqConcat<A> : ISeqInternal<A>
    {
        internal readonly Seq<ISeqInternal<A>> ms;

        public SeqConcat(Seq<ISeqInternal<A>> ms)
        {
            this.ms = ms;
        }

        public A this[int index] => 
            throw new NotSupportedException();

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
                throw new InvalidOperationException("Sequence is empty");
            }
        }

        public ISeqInternal<A> Tail =>
            new SeqLazy<A>(this.Skip(1));

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
                throw new InvalidOperationException("Sequence is empty");
            }
        }

        public int Count => 
            ms.Sum(s => s.Count);

        public SeqConcat<A> AddSeq(ISeqInternal<A> ma) =>
            new SeqConcat<A>(ms.Add(ma));

        public SeqConcat<A> AddSeqRange(Seq<ISeqInternal<A>> ma) =>
            new SeqConcat<A>(ms.Concat(ma));

        public SeqConcat<A> ConsSeq(ISeqInternal<A> ma) =>
            new SeqConcat<A>(ma.Cons(ms));

        public ISeqInternal<A> Add(A value)
        {
            var last = ms.Last.Add(value);
            return new SeqConcat<A>(ms.Take(ms.Count - 1).Add(last));
        }

        public ISeqInternal<A> Cons(A value)
        {
            var head = ms.Head.Cons(value);
            return new SeqConcat<A>(head.Cons(ms.Skip(1)));
        }

        public bool Exists(Func<A, bool> f) =>
            ms.Exists(s => s.Exists(f));
        
        public S Fold<S>(S state, Func<S, A, S> f) =>
            ms.Fold(state, (s, x) => x.Fold(s, f));

        public S FoldBack<S>(S state, Func<S, A, S> f) =>
            ms.FoldBack(state, (s, x) => x.FoldBack(s, f));

        public bool ForAll(Func<A, bool> f) =>
            ms.ForAll(s => s.ForAll(f));

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
                var iter = GetEnumerator();
                for(; amount > 0 && iter.MoveNext(); amount--)
                {
                    yield return iter.Current;
                }
            }
            return new SeqLazy<A>(Yield());
        }

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
                cap = cap << 1;
            }

            var data = new A[cap];
            var start = (cap - total) >> 1;
            var current = start;

            foreach(var s in ms)
            {
                var strict = (SeqStrict<A>)s;
                Array.Copy(strict.data, strict.start, data, current, strict.count);
                current += strict.count;
            }
            return new SeqStrict<A>(data, start, total, 0, 0);
        }

        public int GetHashCode(int hash)
        {
            foreach (var seq in ms)
            {
                hash = seq.GetHashCode(hash);
            }
            return hash;
        }
    }
}
