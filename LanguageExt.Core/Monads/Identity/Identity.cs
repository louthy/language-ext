using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
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
public readonly struct Identity<A> : 
    IEquatable<Identity<A>>, 
    IComparable<Identity<A>>, 
    IComparable,
    K<Identity, A>
{
    public static readonly Identity<A> Bottom = default;
    readonly A? value;

    Identity(A value) =>
        this. value = value;

    public static Identity<A> Pure(A value) =>
        new (value);
    
    [Pure]
    public A Value =>
        value switch
        {
            null  => throw new BottomException(),
            var x => x
        };
        
    public static bool operator ==(Identity<A> lhs, Identity<A> rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(Identity<A> lhs, Identity<A> rhs) =>
        !(lhs == rhs);

    public static bool operator >(Identity<A> lhs, Identity<A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    public static bool operator >=(Identity<A> lhs, Identity<A> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    public static bool operator <(Identity<A> lhs, Identity<A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    public static bool operator <=(Identity<A> lhs, Identity<A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    [Pure]
    public bool Equals(Identity<A> other) =>
        (value, other.value) switch
        {
            (null, null) => true,
            (_, null)    => false,
            (null, _)    => false,
            _            => EqDefault<A>.Equals(value, other.value)
        };

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Identity<A> other && Equals(other);

    [Pure]
    public override int GetHashCode() =>
        value is null ? 0 : HashableDefault<A>.GetHashCode(value);

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Identity<A> t ? CompareTo(t) : 1;

    [Pure]
    public int CompareTo(Identity<A> other) =>
        (value, other.value) switch
        {
            (null, null) => 0,
            (_, null)    => 1,
            (null, _)    => -1,
            _            => OrdDefault<A>.Compare(value, other.value)
        };

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
        value is null
            ? default
            : new(f(value));

    [Pure]
    public Identity<B> Select<B>(Func<A, B> f) =>
        value is null
            ? default
            : new(f(value));

    [Pure]
    public Identity<B> Bind<B>(Func<A, Identity<B>> f) =>
        value is null
            ? default
            : f(value);

    [Pure]
    public Identity<B> Bind<B>(Func<A, K<Identity, B>> f) =>
        value is null
            ? default
            : f(value).As();

    [Pure]
    public Identity<C> SelectMany<B, C>(Func<A, Identity<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure]
    public Identity<C> SelectMany<B, C>(Func<A, K<Identity, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).As().Map(y => project(x, y)));
}
