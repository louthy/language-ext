using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// IdentityT monad
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public record IdentityT<M, A>(K<M, A> Value) : K<IdentityT<M>, A>
    where M : Monad<M>, SemiAlternative<M>
{
    public static IdentityT<M, A> Pure(A value) =>
        new (M.Pure(value));

    public static IdentityT<M, A> Lift(K<M, A> value) =>
        new (value);

    [Pure]
    public IdentityT<M, B> Map<B>(Func<A, B> f) =>
        new(M.Map(f, Value));

    [Pure]
    public IdentityT<M, B> Select<B>(Func<A, B> f) =>
        new(M.Map(f, Value));
    
    [Pure]
    public IdentityT<M, B> Bind<B>(Func<A, IdentityT<M, B>> f) =>
        new(M.Bind(Value, x => f(x).Value));

    [Pure]
    public IdentityT<M, B> Bind<B>(Func<A, K<IdentityT<M>, B>> f) =>
        new(M.Bind(Value, x => f(x).As().Value));

    [Pure]
    public IdentityT<M, B> Bind<B>(Func<A, Pure<B>> f) =>
        new(M.Map(x => f(x).Value, Value));

    [Pure]
    public IdentityT<M, B> Bind<B>(Func<A, IO<B>> f) =>
        new(M.Bind(Value, x => M.LiftIO(f(x))));

    [Pure]
    public IdentityT<M, C> SelectMany<B, C>(Func<A, IdentityT<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure]
    public IdentityT<M, C> SelectMany<B, C>(Func<A, K<IdentityT<M>, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).As().Map(y => project(x, y)));

    [Pure]
    public IdentityT<M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure]
    public IdentityT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}
