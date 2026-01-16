using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Add iterator
    /// </summary>
    internal class Add(Iterator<A> first, Seq<A> second) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            first.Next() is (Exist<A> (var head), var tail) 
                ? Head.Exist(head, new Add(tail, second))
                : second.ForwardIterator().Next();

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            first.NextIO() >> (f => f is (Exist<A> (var head), var tail)
                                        ? IO.pure(Head.Exist(head, new Add(tail, second)))
                                        : second.ForwardIterator().NextIO());
        
        public Add More(A value) =>
            new (first, second.Add(value));

        public override string ToString() => 
            $"{first}, {second.ToFullString()}";

        public override void Dispose() =>
            first.Dispose();

        public override Iterator<A> Using() =>
            new Add(first.Using(), second);

        public override Iterator<A> Strict() => 
            new Add(first.Strict(), second.Strict());
    }
}
