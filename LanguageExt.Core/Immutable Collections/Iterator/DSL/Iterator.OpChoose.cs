using System;
using LanguageExt.Traits;
using L = LanguageExt;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpChoose<X>(Iterator<X> iter, Func<X, Option<A>> f) : Iterator<A>
    {
        public override string ToString() => 
            $"Choose({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            for (var i = iter; i is (Exist<X> (var h), var t); i = t)
            {
                var option = f(h);
                if (option.IsSome) return Head.Exist(option.Value!, t.Choose(f));
            }
            return Head.Nil<A>();
        }

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO()
        {
            return +Monad.recur(iter, go);

            IO<Next<Iterator<X>, (Head<A> Head, Iterator<A> Tail)>> go(Iterator<X> xs) =>
                xs is (Exist<X> (var head), var tail)
                    ? f(head) switch
                      {
                          { IsSome: true, Case: A value } =>
                              IO.pure(L.Next.Done<Iterator<X>, (Head<A> Head, Iterator<A> Tail)>(
                                          Head.Exist(value, new OpChoose<X>(tail, f)))),

                          _ =>
                              IO.pure(L.Next.Loop<Iterator<X>, (Head<A> Head, Iterator<A> Tail)>(tail))
                      }
                    : IO.pure(L.Next.Done<Iterator<X>, (Head<A> Head, Iterator<A> Tail)>(Head.Nil<A>()));
        }

        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpChoose<X>(iter.Using(), f);
    }
}
