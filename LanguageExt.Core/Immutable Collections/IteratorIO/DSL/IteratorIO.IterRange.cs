using System;
namespace LanguageExt;

public abstract partial class IteratorIO 
{
    /// <summary>
    /// Range IteratorIO
    /// </summary>
    internal class IterRange<A>(A Current, bool LastWasEnd, A Stop, Func<A, A> Step, Func<A, A, bool> Eq) : IteratorIO<A>
    {
        (Head<A> Head, IteratorIO<A> Tail) Next() =>
            LastWasEnd
                ? Head.NilIO<A>()
                : Head.ExistIO(Current, new IterRange<A>(Step(Current), Eq(Current, Stop), Stop, Step, Eq));

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Range({Current}..{Stop})";

        public override IteratorIO<A> Using() =>
            this;
    }
}
