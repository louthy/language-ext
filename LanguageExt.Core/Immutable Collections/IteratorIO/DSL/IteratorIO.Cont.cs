using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Continuation IteratorIO
    /// </summary>
    internal class Cont(Func<(A Head, IteratorIO<A> Tail)> next) : IteratorIO<A>
    {
        (Head<A> Head, IteratorIO<A> Tail) Next()
        {
            switch (next())
            {
                case (Nil<A>, _):
                    return Head.NilIO<A>();

                case var (h, t):
                    return Head.ExistIO(h, t);
            }
        }

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.lift(Next);

        public override string ToString() => 
            "...";

        public override IteratorIO<A> Using() =>
            this;
    }
}
