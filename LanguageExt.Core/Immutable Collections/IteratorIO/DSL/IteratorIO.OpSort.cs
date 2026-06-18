using System;
using System.Threading.Tasks;

namespace LanguageExt;

public partial class IteratorIO
{
    internal sealed class OpSort<A>(IteratorIO<A> xs, Comparison<A> comparer) : IteratorIO<A>
    {
        public override string ToString() => 
            $"OrderBy({xs})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO()
        {
            async ValueTask<Arr<A>> go()
            {
                var ys = ArrayWriter<A>.Init();
                await foreach (var x in xs)
                {
                    ys.Add(x);
                }
                ys.MutableView.Sort(comparer);
                return ys.ToArr();
            }

            return IO.liftVAsync(go) >> (arr => arr.AsIteratorIO().NextIO());
        }

        public override IteratorIO<A> Using() => 
            new OpSort<A>(xs.Using(), comparer);
    }
    
    internal sealed class OpSort<A, K>(IteratorIO<A> xs, Func<A, K> key, Comparison<K> comparer) : IteratorIO<A>
    {
        public override string ToString() => 
            $"OrderBy({xs})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO()
        {
            async ValueTask<Arr<A>> go()
            {
                var ks = ArrayWriter<K>.Init();
                var ys = ArrayWriter<A>.Init();
                await foreach (var x in xs)
                {
                    ks.Add(key(x));
                    ys.Add(x);
                }
                ks.MutableView.Sort(ys.MutableView, comparer);
                return ys.ToArr();
            }

            return IO.liftVAsync(go) >> (arr => arr.AsIteratorIO().NextIO());
        }

        public override IteratorIO<A> Using() => 
            new OpSort<A, K>(xs.Using(), key, comparer);
    } 
}
