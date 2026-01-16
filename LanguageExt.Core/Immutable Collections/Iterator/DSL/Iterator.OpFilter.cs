using System;
using LanguageExt.Traits;
using L = LanguageExt;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpFilter(Iterator<A> iter, Func<A, bool> f) : Iterator<A>
    {
        public override string ToString() => 
            $"Filter({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            for (var i = iter; i is (Exist<A> h, var t); i = t)
            {
                if (f(h.Value)) return (h, t.Filter(f));
            }
            return Head.Nil<A>();
        }

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO()
        {
            return +Monad.recur(iter, go);

            IO<Next<Iterator<A>, (Head<A> Head, Iterator<A> Tail)>> go(Iterator<A> xs) =>
                xs is (Exist<A> (var head), var tail)
                    ? f(head) switch
                      {
                          true =>
                              IO.pure(L.Next.Done<Iterator<A>, (Head<A> Head, Iterator<A> Tail)>(
                                          Head.Exist(head, new OpFilter(tail, f)))),

                          _ =>
                              IO.pure(L.Next.Loop<Iterator<A>, (Head<A> Head, Iterator<A> Tail)>(tail))
                      }
                    : IO.pure(L.Next.Done<Iterator<A>, (Head<A> Head, Iterator<A> Tail)>(Head.Nil<A>()));
        }

        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpFilter(iter.Using(), f);
    }
}
