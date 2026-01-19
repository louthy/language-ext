using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class Iterator
{
    /// <summary>
    /// Distinct iterator
    /// </summary>
    internal class OpDistinct<EqA, A>(Iterator<A> iter, HashSet<EqA, A> seen) : Iterator<A>
        where EqA : Eq<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            for (var i = iter; i is (Exist<A> (var h), var t); i = t)
            {
                if(seen.Contains(h)) continue;
                return Head.Exist(h, new OpDistinct<EqA, A>(t, seen.Add(h)));
            }
            return Head.Nil<A>();
        }

        public override string ToString() => 
            $"Distinct({iter})";

        public override Iterator<A> Strict() => 
            new OpDistinct<EqA, A>(iter.Strict(), seen);
    }
}
