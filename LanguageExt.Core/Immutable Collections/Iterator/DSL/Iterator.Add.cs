using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Add iterator
    /// </summary>
    internal class Add : Iterator<A>
    {
        readonly Iterator<A> first;
        readonly Seq<A> second;

        public Add(Iterator<A> first, Seq<A> second)
        {
            this.first = first;
            this.second = second;
        }

        protected override (Head<A> Head, Iterator<A> Tail) Next() =>
            first is (Exist<A> (var head), var tail) 
                ? (new Exist<A>(head), new Add(tail, second))
                : second.ForwardIterator().Next();
        
        public Add More(A value) =>
            new (first, second.Add(value));

        public override string ToString() => 
            $"{first}, {second.ToFullString()}";
    }
}
