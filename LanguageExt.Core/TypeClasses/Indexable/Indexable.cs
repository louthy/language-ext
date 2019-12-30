using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using static LanguageExt.Prelude;

namespace LanguageExt.TypeClasses
{
    [Typeclass("Ix*")]
    public interface Indexable<A, KEY, VALUE>
    {
        Option<VALUE> TryGet(A ma, KEY key);
        VALUE Get(A ma, KEY key);
    }
}
