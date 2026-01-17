using System;
using LanguageExt.Traits;
using L = LanguageExt;

namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpChoose<X>(IteratorIO<X> iter, Func<X, Option<A>> f) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Choose({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO()
        {
            return +Monad.recur(iter, go);

            IO<Next<IteratorIO<X>, (Head<A> Head, IteratorIO<A> Tail)>> go(IteratorIO<X> xs) =>
                xs.NextIO()
                  .Map(n => n is (Exist<X> (var head), var tail)
                                ? f(head) switch
                                  {
                                      { IsSome: true, Case: A value } =>
                                          Next.Done<IteratorIO<X>, (Head<A> Head, IteratorIO<A> Tail)>(
                                              Head.ExistIO(value, new OpChoose<X>(tail, f))),

                                      _ =>
                                          Next.Loop<IteratorIO<X>, (Head<A> Head, IteratorIO<A> Tail)>(tail)
                                  }
                                : Next.Done<IteratorIO<X>, (Head<A> Head, IteratorIO<A> Tail)>(Head.NilIO<A>()));
        }

        public override void Dispose() =>
            iter.Dispose();
        
        public override IteratorIO<A> Using() =>
            new OpChoose<X>(iter.Using(), f);
    }
}
