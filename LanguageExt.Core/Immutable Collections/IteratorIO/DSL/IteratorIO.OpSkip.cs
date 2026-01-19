using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpSkip(IteratorIO<A> iter, long amount) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Skip({amount}:{iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO() >> (n => go(n, amount));

        static IO<(Head<A> Head, IteratorIO<A> Tail)> go((Head<A> Head, IteratorIO<A> Tail) ht, long remain) =>
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
    
    internal sealed class OpSkipWhile(IteratorIO<A> iter, Func<A, bool> predicate) : IteratorIO<A>
    {
        public override string ToString() => 
            $"SkipWhile({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO() >> (n => go(n, predicate));

        static IO<(Head<A> Head, IteratorIO<A> Tail)> go((Head<A> Head, IteratorIO<A> Tail) ht, Func<A, bool> f) =>
            ht switch
            {
                (Exist<A> (var h), var tail) when f(h) => tail.NextIO() >> (n => go(n, f)),
                _                                      => IO.pure(ht)
            };

        public override void Dispose() =>
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpSkipWhile(iter.Using(), predicate);
    }
        
    internal sealed class OpSkipUntil(IteratorIO<A> iter, Func<A, bool> predicate) : IteratorIO<A>
    {
        public override string ToString() => 
            $"SkipWhile({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO() >> (n => go(n, predicate));

        static IO<(Head<A> Head, IteratorIO<A> Tail)> go((Head<A> Head, IteratorIO<A> Tail) ht, Func<A, bool> f) =>
            ht switch
            {
                (Exist<A> (var h), var tail) when !f(h) => tail.NextIO() >> (n => go(n, f)),
                _                                       => IO.pure(ht)
            };

        public override void Dispose() =>
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpSkipWhile(iter.Using(), predicate);
    }
}
