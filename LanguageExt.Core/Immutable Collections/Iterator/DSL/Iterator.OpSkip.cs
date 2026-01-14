using System;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpSkip(Iterator<A> iter, int amount) : Iterator<A>
    {
        public override string ToString() => 
            $"Skip({amount}:{iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var remain = amount;
            var i      = iter;

            for (; i is (Exist<A>, var t) && remain > 0; i = t, remain--)
            {
                // loop
            }

            return remain == 0 && i is (Exist<A> head, var tail)
                       ? (head, tail)
                       : (Nil<A>.Default, Nil.Default);
        }

        public override void Dispose() =>
            iter.Dispose();
        
        public override Iterator<A> Using() =>
            new OpSkip(iter.Using(), amount);
    }
}
