using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Lazy IteratorIO
    /// </summary>
    internal class Lazy(Func<IteratorIO<A>> next) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            next().NextIO();

        public override string ToString() => 
            "...";

        public override IteratorIO<A> Using() =>
            this;
    }
}
