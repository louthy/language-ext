using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A>
{
    internal sealed class OpTake(IteratorIO<A> iter, long remain) : IteratorIO<A>
    {
        public override string ToString() => 
            $"Take({remain}:{iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            remain <= 0
                ? IO.pure(Head.NilIO<A>())
                : iter.NextIO() * (ht => ht switch
                                          {
                                              (Exist<A> head, var tail) => (head, new OpTake(tail, remain - 1)),
                                              _                         => Head.NilIO<A>()
                                          });

        public override void Dispose() =>
            iter.Dispose();

        public override IteratorIO<A> Using() =>
            new OpTake(iter.Using(), remain);
    }
    
    internal sealed class OpTakeWhile(IteratorIO<A> iter, Func<A, bool> pred) : IteratorIO<A>
    {
        public override string ToString() => 
            $"TakeWhile({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO() * (ht => ht switch
                                   {
                                       (Exist<A> head, var tail) when pred(head.Value) =>
                                           (head, new OpTakeWhile(tail, pred)),
                                       
                                       _ => Head.NilIO<A>()
                                   });

        public override void Dispose() =>
            iter.Dispose();

        public override IteratorIO<A> Using() =>
            new OpTakeWhile(iter.Using(), pred);
    }
        
    internal sealed class OpTakeUntil(IteratorIO<A> iter, Func<A, bool> pred) : IteratorIO<A>
    {
        public override string ToString() => 
            $"TakeUntil({iter})";

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() =>
            iter.NextIO() * (ht => ht switch
                                   {
                                       (Exist<A> head, var tail) when !pred(head.Value) =>
                                           (head, new OpTakeWhile(tail, pred)),
                                       
                                       _ => Head.NilIO<A>()
                                   });

        public override void Dispose() =>
            iter.Dispose();

        public override IteratorIO<A> Using() =>
            new OpTakeWhile(iter.Using(), pred);
    }
}
