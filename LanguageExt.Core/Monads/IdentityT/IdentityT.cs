using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// IdentityT monad
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public record IdentityT<M, A>(K<M, A> Value) : K<IdentityT<M>, A>
    where M : Monad<M>, Choice<M>
{
    [Pure]
    public IdentityT<M, B> Map<B>(Func<A, B> f) =>
        new(M.Map(f, Value));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    public IdentityT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new(f(Value));
        
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
        new(M.Bind(Value, x => M.LiftIOMaybe(f(x))));

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
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static IdentityT<M, A> operator >> (IdentityT<M, A> lhs, IdentityT<M, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static IdentityT<M, A> operator >> (IdentityT<M, A> lhs, K<IdentityT<M>, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static IdentityT<M, A> operator >> (IdentityT<M, A> lhs, IdentityT<M, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));

    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static IdentityT<M, A> operator >> (IdentityT<M, A> lhs, K<IdentityT<M>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));

    /// <summary>
    /// Choice operator
    /// </summary>
    public static IdentityT<M, A> operator | (IdentityT<M, A> lhs, K<IdentityT<M>, A> rhs) =>
        lhs.Choose(rhs).As();

    /// <summary>
    /// Choice operator
    /// </summary>
    public static IdentityT<M, A> operator | (K<IdentityT<M>, A> lhs, IdentityT<M, A> rhs) =>
        lhs.Choose(rhs).As();

    /// <summary>
    /// Combine operator
    /// </summary>
    public static IdentityT<M, A> operator + (IdentityT<M, A> lhs, K<IdentityT<M>, A> rhs) =>
        lhs.Combine(rhs).As();

    /// <summary>
    /// Combine operator
    /// </summary>
    public static IdentityT<M, A> operator + (K<IdentityT<M>, A> lhs, IdentityT<M, A> rhs) =>
        lhs.Combine(rhs).As();
}
