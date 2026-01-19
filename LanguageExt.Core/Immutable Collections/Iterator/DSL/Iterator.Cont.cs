using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Continuation iterator
    /// </summary>
    internal class Cont(Func<(A Head, Iterator<A> Tail)> next) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            switch (next())
            {
                case (Nil<A>, _):
                    return Head.Nil<A>();

                case var (h, t):
                    return Head.Exist(h, t);
            }
        }

        public override string ToString() => 
            "...";
    }
}
