using System;

namespace LanguageExt;

public abstract partial class Iterator 
{
    /// <summary>
    /// Add iterator
    /// </summary>
    internal class Add<A>(Iterator<A> first, Seq<A> second) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            first.Next() is (Exist<A> (var head), var tail) 
                ? Head.Exist(head, new Add<A>(tail, second))
                : second.ForwardIterator().Next();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            first.NextIO() >> (f => f is (Exist<A> (var head), var tail)
                                        ? IO.pure(Head.Exist(head, new Add<A>(tail, second)))
                                        : second.ForwardIterator().NextIO());
        
        public override string ToString() => 
            $"{first}, {second.ToFullString()}";

        public override void Dispose() =>
            first.Dispose();

        public override Iterator<A> Using() =>
            new Add<A>(first.Using(), second);

        public override Iterator<A> Strict() => 
            new Add<A>(first.Strict(), second.Strict());

        public override Iterator<A> Append(A value) => 
            new Add<A>(first, second.Add(value));
    }
}
