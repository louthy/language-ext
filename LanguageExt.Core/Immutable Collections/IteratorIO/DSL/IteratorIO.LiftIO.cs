using System;

namespace LanguageExt;

public abstract partial class IteratorIO 
{
    /// <summary>
    /// Lift an IO Iterator into an IteratorIO
    /// </summary>
    internal class LiftIO<A>(IO<IteratorIO<A>> lifted) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            lifted >> (iter => iter.NextIO());
        
        public override string ToString() => 
            "LiftIO";

        public override IteratorIO<A> Using() =>
            new LiftIO<A>(lifted * (i => i.Using()));

        public override IteratorIO<A> Strict() => 
            new LiftIO<A>(lifted * (i => i.Strict()));

        public override IteratorIO<A> Append(A value) => 
            new LiftIO<A>(lifted * (i => i.Append(value)));
    }
}
