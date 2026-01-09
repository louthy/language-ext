using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Continuation iterator
    /// </summary>
    internal class Cont : Iterator<A>
    {
        readonly Func<(A Head, Iterator<A> Tail)> next;

        public Cont(Func<(A Head, Iterator<A> Tail)> next) =>
            this.next = next;

        protected override (Head<A> Head, Iterator<A> Tail) Next()
        {
            switch (next())
            {
                case (Nil<A>, _):
                    return (Nil<A>.Default, Nil.Default);

                case var (h, t):
                    return (new Exist<A>(h), t);
            }
        }

        public override string ToString() => 
            "...";
    }
}
