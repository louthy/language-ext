using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class IteratorIO
{
    /// <summary>
    /// Distinct iterator
    /// </summary>
    internal class OpDistinct<EqA, A>(IteratorIO<A> iter, HashSet<EqA, A> seen) : IteratorIO<A>
        where EqA : Eq<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO()
        {
            return +Monad.recur(iter, go);

            IO<Next<IteratorIO<A>, (Head<A> Head, IteratorIO<A> Tail)>> go(IteratorIO<A> xs) =>
                xs.NextIO()
                  .Map(n => n is (Exist<A> (var head), var tail)
                                ? seen.Contains(head) switch
                                  {
                                      false =>
                                          Next.Done<IteratorIO<A>, (Head<A> Head, IteratorIO<A> Tail)>(
                                              Head.ExistIO(head, new OpDistinct<EqA, A>(tail, seen.Add(head)))),

                                      _ => Next.Loop<IteratorIO<A>, (Head<A> Head, IteratorIO<A> Tail)>(tail)
                                  }
                                : Next.Done<IteratorIO<A>, (Head<A> Head, IteratorIO<A> Tail)>(Head.NilIO<A>()));
        }
        public override IteratorIO<A> Using() => 
            new OpDistinct<EqA, A>(iter.Using(), seen);

        public override IteratorIO<A> Strict() => 
            new OpDistinct<EqA, A>(iter.Strict(), seen);

        public override string ToString() => 
            $"Distinct({iter})";
    }
}
