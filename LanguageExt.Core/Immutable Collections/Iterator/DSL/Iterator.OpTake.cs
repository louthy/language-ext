using System;

namespace LanguageExt;

public abstract partial class Iterator<A>
{
    internal sealed class OpTake(Iterator<A> iter, long remain) : Iterator<A>
    {
        public override string ToString() => 
            $"Take({remain}:{iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remain <= 0
                ? Head.Nil<A>()
                : iter is (Exist<A> head, var tail)
                    ? (head, new OpTake(tail, remain - 1))
                    : Head.Nil<A>();
    }
    
    internal sealed class OpTakeWhile(Iterator<A> iter, Func<A, bool> pred) : Iterator<A>
    {
        public override string ToString() => 
            $"TakeWhile({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            iter is (Exist<A> head, var tail) && pred(head.Value)
                ? (head, new OpTakeWhile(tail, pred))
                : Head.Nil<A>();
    }
        
    internal sealed class OpTakeUntil(Iterator<A> iter, Func<A, bool> pred) : Iterator<A>
    {
        public override string ToString() => 
            $"TakeWhile({iter})";

        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            iter is (Exist<A> head, var tail) && !pred(head.Value)
                ? (head, new OpTakeWhile(tail, pred))
                : Head.Nil<A>();
    }
}
