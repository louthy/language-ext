using System.Numerics;
using LanguageExt.Traits.Domain;

namespace DomainTypesExamples.Invariants;

public sealed class NonNegative<A> : Rule<NonNegative<A>, A>
    where A : INumber<A>
{
    public A Zero => A.Zero;

    public static bool Check(A value) =>
        value >= A.Zero;
}
