using LanguageExt.Attributes;
using System.Diagnostics.Contracts;

namespace LanguageExt.TypeClasses
{
    [Typeclass("Ord*")]
    public interface Ord<A> : Eq<A>, System.Collections.Generic.IComparer<A>
    {
    }
}
