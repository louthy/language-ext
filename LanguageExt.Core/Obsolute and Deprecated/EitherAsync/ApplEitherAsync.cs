using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

[Obsolete(Change.UseEffMonadInstead)]
public struct ApplEitherAsync<L, A, B>
{
    [Pure]
    public static EitherAsync<L, B> Action(EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
        from a in fa
        from b in fb
        select b;

    [Pure]
    public static EitherAsync<L, B> Apply(EitherAsync<L, Func<A, B>> fab, EitherAsync<L, A> fa) =>
        from f in fab
        from a in fa
        select f(a);

    [Pure]
    public static EitherAsync<L, B> Map(EitherAsync<L, A> ma, Func<A, B> f) =>
        ma.Map(f);

    [Pure]
    public static EitherAsync<L, B> MapAsync(EitherAsync<L, A> ma, Func<A, Task<B>> f) =>
        ma.MapAsync(f);

    [Pure]
    public static EitherAsync<L, A> PureAsync(Task<A> x) =>
        EitherAsync<L, A>.RightAsync(x);
}

[Obsolete(Change.UseEffMonadInstead)]
public struct ApplEitherAsync<L, A, B, C>
{
    public static EitherAsync<L, Func<B, C>> Apply(EitherAsync<L, Func<A, Func<B, C>>> fabc, EitherAsync<L, A> fa) =>
        from f in fabc
        from a in fa
        select f(a);

    public static EitherAsync<L, C> Apply(EitherAsync<L, Func<A, Func<B, C>>> fabc, EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
        from f in fabc
        from a in fa
        from b in fb
        select f(a)(b);

    public static EitherAsync<L, A> PureAsync(Task<A> x) =>
        EitherAsync<L, A>.RightAsync(x);
}
