using System;

namespace LanguageExt;

public abstract partial class Iterator
{
    internal sealed class OpUnion<A, B>(
        Iterator<A> xs, 
        Iterator<A> ys, 
        Func<A, Option<B>> choose, 
        Func<A, A, Option<B>> join) : Iterator<B>
    {
        public override string ToString() => 
            $"Union({xs}, {ys})";

        public override (Head<B> Head, Iterator<B> Tail) Next()
        {
            var iterator = (xs.Next(), ys.Next()) switch
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
                           };

            return iterator.Next();
        }
    }
}
