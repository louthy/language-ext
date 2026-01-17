using System.Threading.Tasks;

namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpReverse(IteratorIO<A> iter) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Reverse({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO()
        {
            return IO.liftVAsync(go) >> (i => i.NextIO());
            
            async ValueTask<IteratorIO<A>> go(EnvIO e)
            {
                // Naive implementation, consider alternatives. 
                System.Collections.Generic.List<A> writer = new();
                for (var i = iter; await i.NextIO().RunAsync(e) is (Exist<A> (var head), var tail); i = tail)
                {
                    writer.Add(head);
                }
                writer.Reverse();
                return IteratorIO.forward(new Arr<A>(writer.ToArray()));
            }
        }

        public override void Dispose() => 
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpReverse(iter.Using());
    }
}
