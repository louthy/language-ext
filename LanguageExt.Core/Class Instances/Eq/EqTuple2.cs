using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

public struct EqTuple2<EqA, EqB, A, B> : Eq<(A, B)>
    where EqA : Eq<A>
    where EqB : Eq<B>
{
    public static int GetHashCode((A, B) pair) =>
        FNV32.Next(EqA.GetHashCode(pair.Item1), 
                   EqB.GetHashCode(pair.Item2));

    public static bool Equals((A, B) x, (A, B) y) =>
        EqA.Equals(x.Item1, y.Item1) &&
        EqB.Equals(x.Item2, y.Item2);
}
