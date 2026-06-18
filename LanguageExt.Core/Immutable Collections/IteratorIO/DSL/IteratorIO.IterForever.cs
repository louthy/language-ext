using System;

namespace LanguageExt;

public abstract partial class IteratorIO 
{
    /// <summary>
    /// Yield a value forever
    /// </summary>
    internal class IterForever<A>(A value) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Head.ExistIO(value, this));

        public override string ToString() => 
            $"Forever({value})";

        public override IteratorIO<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Yields an IO operation forever, if it fails, the yielding stops
    /// </summary>
    internal class IterForeverIO<A>(IO<A> value) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            value.Map(x => Head.ExistIO(x, this)) | IO.pure(Head.NilIO<A>());

        public override string ToString() => 
            $"Forever({value})";

        public override IteratorIO<A> Using() =>
            this;
    }
}
