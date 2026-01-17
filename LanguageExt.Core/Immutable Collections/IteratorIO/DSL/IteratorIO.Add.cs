using System;

namespace LanguageExt;

public abstract partial class IteratorIO 
{
    /// <summary>
    /// Add IteratorIO
    /// </summary>
    internal class Add<A>(IteratorIO<A> first, Seq<A> second) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            first.NextIO() >> (f => f is (Exist<A> (var head), var tail)
                                        ? IO.pure(Head.ExistIO(head, new Add<A>(tail, second)))
                                        : new IterSeq(second).NextIO());
        
        public override string ToString() => 
            $"{first}, {second.ToFullString()}";

        public override void Dispose() =>
            first.Dispose();

        public override IteratorIO<A> Using() =>
            new Add<A>(first.Using(), second);

        public override IteratorIO<A> Strict() => 
            new Add<A>(first.Strict(), second.Strict());

        public override IteratorIO<A> Append(A value) => 
            new Add<A>(first, second.Add(value));
    }
}
