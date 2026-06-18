using System;

namespace LanguageExt;

public partial class Iterator
{
    internal sealed class OpSort<A>(Iterator<A> xs, Comparison<A> comparer) : Iterator<A>
    {
        public override string ToString() => 
            $"OrderBy({xs})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var ys = ArrayWriterRef<A>.Init();
            foreach (var x in xs)
            {
                ys.Add(x);
            }
            ys.MutableView.Sort(comparer);
            return ys.ToArr().ForwardIterator().Next();
        }
    }
    
    internal sealed class OpSort<A, K>(Iterator<A> xs, Func<A, K> key, Comparison<K> comparer) : Iterator<A>
    {
        public override string ToString() => 
            $"OrderBy({xs})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var ks = ArrayWriterRef<K>.Init();
            var ys = ArrayWriterRef<A>.Init();
            foreach (var x in xs)
            {
                ks.Add(key(x));
                ys.Add(x);
            }
            ks.MutableView.Sort(ys.MutableView, comparer);
            return ys.ToArr().ForwardIterator().Next();
        }
    } 
}
