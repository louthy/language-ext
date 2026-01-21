using System;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class Iterator 
{
    /// <summary>
    /// Monad.Recur iterator
    /// </summary>
    internal class Recur<A, B>(Func<A, K<Iterator, Next<A, B>>> f, Stck<Iterator<Next<A, B>>> cont) : Iterator<B>
    {
        public override (Head<B> Head, Iterator<B> Tail) Next()
        {
            var local = cont;
            while (!local.IsEmpty)
            {
                if (local.PeekUnsafe() is (Exist<Next<A, B>>(var next), var tail))
                {
                    if (next.IsDone)
                    {
                        local = local.Pop()
                                     .Push(tail);
                        
                        return Head.Exist(next.Done, new Recur<A, B>(f, local) );
                    }
                    else
                    {
                        local = local.Pop()
                                     .Push(tail)
                                     .Push(+f(next.Loop));
                    }
                }
                else
                {
                    local = local.Pop();
                }
            }
            return Head.Nil<B>();
        }

        public override string ToString() => 
            "Recur";

        public override Iterator<B> Strict() => 
            this;
    }
}
