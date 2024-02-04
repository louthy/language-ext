using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Either type ordering
/// </summary>
[Obsolete(Change.UseEffMonadInstead)]
public struct OrdEitherAsync<L, A>
{
    public static Task<int> CompareAsync(EitherAsync<L, A> x, EitherAsync<L, A> y) =>
        OrdEitherAsync<OrdDefault<L>, OrdDefault<A>, L, A>.CompareAsync(x, y);

    public static Task<bool> EqualsAsync(EitherAsync<L, A> x, EitherAsync<L, A> y) =>
        EqEitherAsync<EqDefault<L>, EqDefault<A>, L, A>.EqualsAsync(x, y);

    public static Task<int> GetHashCodeAsync(EitherAsync<L, A> x) =>
        HashableEitherAsync<L, A>.GetHashCodeAsync(x);
}

/// <summary>
/// Either type ordering
/// </summary>
[Obsolete(Change.UseEffMonadInstead)]
public struct OrdEitherAsync<OrdL, OrdA, L, A> 
    where OrdL : Ord<L>
    where OrdA : Ord<A>
{
    public static async Task<int> CompareAsync(EitherAsync<L, A> x, EitherAsync<L, A> y)
    {
        var dx = await x.Data.ConfigureAwait(false);
        var dy = await y.Data.ConfigureAwait(false);
            
        return dx.State switch
               {
                   EitherStatus.IsRight => dy.State switch
                                           {
                                               EitherStatus.IsRight => OrdA.Compare(dx.RightValue, dy.RightValue),
                                               EitherStatus.IsLeft => 1,
                                               _ => 1
                                           },
                   EitherStatus.IsLeft => dy.State switch
                                          {
                                              EitherStatus.IsLeft => OrdL.Compare(dx.LeftValue, dy.LeftValue),
                                              EitherStatus.IsRight => -1,
                                              _ => 1
                                          },
                   EitherStatus.IsBottom => dy.State == EitherStatus.IsBottom ? 0 : -1,
                   _                     => 0
               };
    }

    public static Task<bool> EqualsAsync(EitherAsync<L, A> x, EitherAsync<L, A> y) =>
        EqEitherAsync<OrdL, OrdA, L, A>.EqualsAsync(x, y);

    public static Task<int> GetHashCodeAsync(EitherAsync<L, A> x) =>
        HashableEitherAsync<OrdL, OrdA, L, A>.GetHashCodeAsync(x);
}
