using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Identity monad
/// </summary>
/// <remarks>
/// Simply carries the bound value through its bind expressions without imparting any additional behaviours.  It can
/// be constructed using:
///
///     Identity<int> ma = Id(123);
/// 
/// </remarks>
/// <typeparam name="A">Bound value type</typeparam>
public record Identity<A>(A Value) : 
    K<Identity, A>
{
    public static Identity<A> Pure(A value) =>
        new (value);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// </remarks>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Identity<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));
    
    [Pure]
    public Identity<B> Map<B>(Func<A, B> f) =>
        new(f(Value));

    [Pure]
    public Identity<B> Select<B>(Func<A, B> f) =>
        new(f(Value));

    [Pure]
    public Identity<B> Bind<B>(Func<A, Identity<B>> f) =>
        f(Value);

    [Pure]
    public Identity<B> Bind<B>(Func<A, K<Identity, B>> f) =>
        f(Value).As();

    [Pure]
    public Identity<C> SelectMany<B, C>(Func<A, Identity<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure]
    public Identity<C> SelectMany<B, C>(Func<A, K<Identity, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).As().Map(y => project(x, y)));
}
