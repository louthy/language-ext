using System;
namespace LanguageExt;

public abstract partial class Iterator 
{
    /// <summary>
    /// Range iterator
    /// </summary>
    internal class IterRange<A>(A Current, bool LastWasEnd, A Stop, Func<A, A> Step, Func<A, A, bool> Eq) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            LastWasEnd
                ? Head.Nil<A>()
                : Head.Exist(Current, new IterRange<A>(Step(Current), Eq(Current, Stop), Stop, Step, Eq));

        public override string ToString() => 
            $"Range({Current}..{Stop})";
    }
}
