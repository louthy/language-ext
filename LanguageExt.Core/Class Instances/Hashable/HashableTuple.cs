namespace LanguageExt.ClassInstances;

public struct HashableTuple<HashA, HashB, A, B> : Hashable<(A, B)>
    where HashA : Hashable<A>
    where HashB : Hashable<B>
{
    public static int GetHashCode((A, B) pair) =>
        FNV32.Next(HashA.GetHashCode(pair.Item1), 
                   HashB.GetHashCode(pair.Item2));
}
