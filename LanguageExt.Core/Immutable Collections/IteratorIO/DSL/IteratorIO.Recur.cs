using System;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class IteratorIO
{
    /// <summary>
    /// Monad.Recur iterator
    /// </summary>
    internal class Recur<A, B>(Func<A, K<IteratorIO, Next<A, B>>> f, Stck<IteratorIO<Next<A, B>>> cont) : IteratorIO<B>
    {
        public override IO<(Head<B> Head, IteratorIO<B> Tail)> NextIO()
        {
            return go(cont);

            IO<(Head<B> Head, IteratorIO<B> Tail)> go(Stck<IteratorIO<Next<A, B>>> local) =>
                local.IsEmpty
                    ? IO.pure(Head.NilIO<B>())
                    : local.PeekUnsafe().NextIO() >>
                      (n => n is (Exist<Next<A, B>>(var next), var tail)
                                ? next.IsDone
                                      ? IO.pure(Head.ExistIO(next.Done, new Recur<A, B>(f, local.Pop().Push(tail))))
                                      : go(local.Pop()
                                                .Push(tail)
                                                .Push(+f(next.Loop)))
                                : go(local.Pop()));
        }

        public override IteratorIO<B> Using() => 
            this;

        public override string ToString() => 
            "Recur";

        public override IteratorIO<B> Strict() => 
            this;
    }
}
