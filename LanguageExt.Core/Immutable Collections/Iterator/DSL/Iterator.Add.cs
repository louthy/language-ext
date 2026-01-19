using System;

namespace LanguageExt;

public abstract partial class Iterator 
{
    /// <summary>
    /// Add iterator
    /// </summary>
    internal class Add<A>(Seq<A> first, Iterator<A> second, Seq<A> third) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            first.IsEmpty
                ? second.Next() is (Exist<A> (var head), var tail) 
                      ? Head.Exist(head, new Add<A>([], tail, third))
                      : third.ForwardIterator().Next()
                : Head.Exist(first[0], new Add<A>(first.Tail, second, third));

        public override string ToString() => 
            $"{first}, {second.ToFullString()}";

        public override Iterator<A> Strict() => 
            new Add<A>(first.Strict(), second.Strict(), third.Strict());

        public override Iterator<A> Append(A value) => 
            new Add<A>(first, second, third.Add(value));

        public override Iterator<A> Prepend(A value) => 
            new Add<A>(value.Cons(first), second, third);
    }
}
