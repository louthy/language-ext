using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Lazy iterator
    /// </summary>
    internal class Lazy(Func<Iterator<A>> next) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            next() switch
            {
                var (h, t) => (h, t)
            };

        public override string ToString() => 
            "...";
    }
}
