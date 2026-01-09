using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Lazy iterator
    /// </summary>
    internal class Lazy : Iterator<A>
    {
        readonly Func<Iterator<A>> next;

        public Lazy(Func<Iterator<A>> next) =>
            this.next = next;

        protected override (Head<A> Head, Iterator<A> Tail) Next() =>
            next() switch
            {
                var (h, t) => (h, t)
            };
    }
}
