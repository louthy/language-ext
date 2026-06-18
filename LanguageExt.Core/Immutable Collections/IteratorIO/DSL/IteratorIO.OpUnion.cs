using System;

namespace LanguageExt;

public abstract partial class IteratorIO
{
    internal sealed class OpUnion<A, B>(
        IteratorIO<A> xs, 
        IteratorIO<A> ys, 
        Func<A, Option<B>> choose, 
        Func<A, A, Option<B>> join) : IteratorIO<B>
    {
        public override string ToString() => 
            $"Union({xs}, {ys})";

        public override IO<(Head<B> Head, IteratorIO<B> Tail)> NextIO()
        {
            var operation = (((Head<A> Head, IteratorIO<A> Tail) x, (Head<A> Head, IteratorIO<A> Tail) y) =>
                                 (x, y) switch
                                 {
                                     ((Exist<A> (var lh), var lt), (Exist<A> (var rh), var rt)) =>
                                         join(lh, rh)
                                            .Match(Some: uh => cons(uh, lt.Union(rt, choose, join)),
                                                   None: () => lt.Union(rt, choose, join)),

                                     ((Exist<A> (var lh), var lt), _) =>
                                         choose(lh).Match(Some: uh => cons(uh, lt.Choose(choose)),
                                                          None: () => lt.Choose(choose)),

                                     (_, (Exist<A> (var rh), var rt)) =>
                                         choose(rh).Match(Some: uh => cons(uh, rt.Choose(choose)),
                                                          None: () => rt.Choose(choose)),

                                     _ => empty<B>()
                                 })
                          * xs.NextIO()
                          * ys.NextIO();

            return from iterator in operation
                   from next in iterator.NextIO()
                   select next;
        }

        public override void Dispose()
        {
            xs.Dispose();
            ys.Dispose();
        }
        
        public override IteratorIO<B> Using() =>
            new OpUnion<A, B>(xs.Using(), ys.Using(), choose, join);
    }
}
