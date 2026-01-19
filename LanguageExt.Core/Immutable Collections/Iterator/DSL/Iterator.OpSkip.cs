using System;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpSkip(Iterator<A> iter, long amount) : Iterator<A>
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
                       : Head.Nil<A>();
        }
    }
    
    internal sealed class OpSkipWhile(Iterator<A> iter, Func<A, bool> predicate) : Iterator<A>
    {
        public override string ToString() => 
            $"SkipWhile({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var i = iter;

            for (; i is (Exist<A> (var h), var t) && predicate(h); i = t)
            {
                // loop
            }

            return i is (Exist<A> head, var tail)
                       ? (head, tail)
                       : Head.Nil<A>();
        }
    }
        
    internal sealed class OpSkipUntil(Iterator<A> iter, Func<A, bool> predicate) : Iterator<A>
    {
        public override string ToString() => 
            $"SkipUntil({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var i = iter;

            for (; i is (Exist<A> (var h), var t) && !predicate(h); i = t)
            {
                // loop
            }

            return i is (Exist<A> head, var tail)
                       ? (head, tail)
                       : Head.Nil<A>();
        }
    }
}
