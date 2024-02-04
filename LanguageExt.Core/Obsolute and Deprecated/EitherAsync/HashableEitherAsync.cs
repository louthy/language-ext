using System;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Either type hash
/// </summary>
[Obsolete(Change.UseEffMonadInstead)]
public struct HashableEitherAsync<L, R>
{
    public static Task<int> GetHashCodeAsync(EitherAsync<L, R> x) =>
        HashableEitherAsync<HashableDefault<L>, HashableDefault<R>, L, R>.GetHashCodeAsync(x);
}
    
/// <summary>
/// Either type hash
/// </summary>
[Obsolete(Change.UseEffMonadInstead)]
public struct HashableEitherAsync<HashL, HashR, L, R>
    where HashL : Hashable<L>
    where HashR : Hashable<R>
{
    public static async Task<int> GetHashCodeAsync(EitherAsync<L, R> x)
    {
        var d = await x.Data.ConfigureAwait(false);
        return d.State switch
               {
                   EitherStatus.IsRight => HashL.GetHashCode(d.LeftValue),
                   EitherStatus.IsLeft  => HashR.GetHashCode(d.RightValue),
                   _                    => 0
               };
    }
}
