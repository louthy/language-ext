using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.HKT;

namespace LanguageExt;

public static class Identity
{
    public static Identity<A> As<A>(this KStar<MIdentity, A> ma) =>
        (Identity<A>)ma;
}

/// <summary>
/// Identity monad
/// </summary>
public class MIdentity : Monad<MIdentity>
{
    public static KStar<MIdentity, A> Lift<A>(Transducer<Unit, A> f) => 
        new Identity<A>(f);
}

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
    KStar<MIdentity, A>
{
    public static readonly Identity<A> Bottom = default;
    readonly Transducer<Unit, A>? morphism;

    public Identity(A value) =>
        morphism = Transducer.pure(value);

    public Identity(Transducer<Unit, A> f) =>
        morphism = f;

    [Pure]
    public A Value =>
        morphism switch
        {
            null => throw new BottomException(),
            _    => morphism.Invoke1(default).ValueUnsafe
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
        (morphism, other.morphism) switch
        {
            (null, null) => true,
            (_, null)    => false,
            (null, _)    => false,
            _            => (from x in morphism
                             from y in other.morphism
                             select EqDefault<A>.Equals(x, y))
                            .Invoke1(default)
                            .ValueUnsafe
        };

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Identity<A> other && Equals(other);

    [Pure]
    public override int GetHashCode() =>
        Morphism.Map(HashableDefault<A>.GetHashCode)
                .Invoke1(default)
                .ValueUnsafe;

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Identity<A> t ? CompareTo(t) : 1;

    [Pure]
    public int CompareTo(Identity<A> other) =>
        (morphism, other.morphism) switch
        {
            (null, null) => 0,
            (_, null)    => 1,
            (null, _)    => -1,
            _            => (from x in morphism
                             from y in other.morphism
                             select OrdDefault<A>.Compare(x, y))
                            .Invoke1(default)
                            .ValueUnsafe
        };

    [Pure]
    public Identity<B> Map<B>(Func<A, B> f) =>
        Functor.map(this, f).As();

    [Pure]
    public Identity<B> Select<B>(Func<A, B> f) =>
        (Identity<B>)Functor.map(this, f);

    [Pure]
    public Identity<B> Bind<B>(Func<A, Identity<B>> f) =>
        Monad.bind<MIdentity, Identity<B>, A, B>(this, f);

    [Pure]
    public Identity<C> SelectMany<B, C>(Func<A, Identity<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure]
    public Transducer<Unit, A> Morphism =>
        morphism ?? throw new BottomException();
}
