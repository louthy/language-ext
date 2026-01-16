namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpSkip(Iterator<A> iter, int amount) : Iterator<A>
    {
        public override string ToString() => 
            $"Skip({amount}:{iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var remain = amount;
            var i      = iter;

            for (; i is (Exist<A>, var t) && remain > 0; i = t, remain--)
            {
                // loop
            }

            return remain == 0 && i is (Exist<A> head, var tail)
                       ? (head, tail)
                       : Head.Nil<A>();
        }

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            iter.NextIO() >> (n => go(n, amount));

        static IO<(Head<A> Head, Iterator<A> Tail)> go((Head<A> Head, Iterator<A> Tail) ht, int remain) =>
            remain > 0
                ? ht switch
                  {
                      (Exist<A>, var tail) => tail.NextIO() >> (n => go(n, remain - 1)),
                      _                    => IO.pure(ht)
                  }
                : IO.pure(ht);

        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpSkip(iter.Using(), amount);
    }
}
