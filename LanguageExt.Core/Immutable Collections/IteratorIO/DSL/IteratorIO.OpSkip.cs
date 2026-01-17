namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpSkip(IteratorIO<A> iter, int amount) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Skip({amount}:{iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO() >> (n => go(n, amount));

        static IO<(Head<A> Head, IteratorIO<A> Tail)> go((Head<A> Head, IteratorIO<A> Tail) ht, int remain) =>
            remain > 0
                ? ht switch
                  {
                      (Exist<A>, var tail) => tail.NextIO() >> (n => go(n, remain - 1)),
                      _                    => IO.pure(ht)
                  }
                : IO.pure(ht);

        public override void Dispose() =>
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpSkip(iter.Using(), amount);
    }
}
