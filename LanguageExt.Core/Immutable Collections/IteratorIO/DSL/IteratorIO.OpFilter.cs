using System;
using LanguageExt.Traits;
using L = LanguageExt;

namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpFilter(IteratorIO<A> iter, Func<A, bool> f) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Filter({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO()
        {
            return +Monad.recur(iter, go);

            IO<Next<IteratorIO<A>, (Head<A> Head, IteratorIO<A> Tail)>> go(IteratorIO<A> xs) =>
                xs.NextIO()
                  .Map(n => n is (Exist<A> (var head), var tail)
                                ? f(head) switch
                                  {
                                      true =>
                                          Next.Done<IteratorIO<A>, (Head<A> Head, IteratorIO<A> Tail)>(
                                              Head.ExistIO(head, new OpFilter(tail, f))),

                                      _ => Next.Loop<IteratorIO<A>, (Head<A> Head, IteratorIO<A> Tail)>(tail)
                                  }
                                : Next.Done<IteratorIO<A>, (Head<A> Head, IteratorIO<A> Tail)>(Head.NilIO<A>()));
        }

        public override void Dispose() =>
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpFilter(iter.Using(), f);
    }
}
