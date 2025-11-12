using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Identity monad
/// </summary>
/// <remarks>
/// Simply carries the bound value through its bind expressions without imparting any additional behaviours.  It can
/// be constructed using:
///
///     Identity〈int〉 ma = Id(123);
/// 
/// </remarks>
/// <typeparam name="A">Bound value type</typeparam>
public record Identity<A>(A Value) : 
    K<Identity, A>,
    IComparable<Identity<A>>
{
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Identity<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<M, Identity<B>> TraverseM<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        M.Map(x => x.As(), Traversable.traverseM(f, this));

    [Pure]
    public virtual bool Equals(Identity<A>? other) =>
        other is not null && EqDefault<A>.Equals(Value, other.Value);

    [Pure]
    public override int GetHashCode() => 
        HashableDefault<A>.GetHashCode(Value);

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

    [Pure]
    public int CompareTo(Identity<A>? other) =>
        other is { } rhs
            ? OrdDefault<A>.Compare(Value, rhs.Value)
            : 1;
}
