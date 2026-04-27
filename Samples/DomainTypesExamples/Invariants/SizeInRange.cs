using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace DomainTypesExamples.Invariants;

public sealed class SizeEqualsTo<V, A> : Rule<SizeEqualsTo<V, A>, A>
    where V : Const<A>
{
    public A Value => V.Value;
    
    public static bool Check(A value) =>
        Equals(V.Value, value);
}
