using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.HKT;

namespace LanguageExt;

public static class IdentityExt
{
    public static Identity<A> As<A>(this Monad<Identity, A> ma) =>
        (Identity<A>)ma;
    
    public static Identity<A> As<A>(this Applicative<Identity, A> ma) =>
        (Identity<A>)ma;
    
    public static Identity<A> As<A>(this Functor<Identity, A> ma) =>
        (Identity<A>)ma;
}

/// <summary>
/// Identity monad
/// </summary>
public class Identity : Monad<Identity>
{
    static Applicative<Identity, A> Applicative<Identity>.Pure<A>(A value) => 
        new Identity<A>(value);

    public static Applicative<Identity, B> Apply<A, B>(Applicative<Identity, Transducer<A, B>> mf, Applicative<Identity, A> ma) => 
        from f in mf.As()
        from a in ma.As()
        from r in f.Invoke(a)
        select r;

    public static Applicative<Identity, B> Action<A, B>(Applicative<Identity, A> ma, Applicative<Identity, B> mb) => 
        from _ in ma.As()
        from b in mb.As()
        select b;

    public static Monad<Identity, B> Bind<A, B>(Monad<Identity, A> ma, Transducer<A, Monad<Identity, B>> f) =>
        new Identity<B>(
            Transducer.compose(ma.As().ToTransducer(), f)
                      .Map(mb => mb.As().ToTransducer()).Flatten());
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
    Monad<Identity, A>
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
            _    => morphism.Run1(default).ValueUnsafe
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
                            .Run1(default)
                            .ValueUnsafe
        };

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Identity<A> other && Equals(other);

    [Pure]
    public override int GetHashCode() =>
        ToTransducer().Map(HashableDefault<A>.GetHashCode)
                      .Run1(default)
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
                            .Run1(default)
                            .ValueUnsafe
        };

    [Pure]
    public Identity<B> Map<B>(Func<A, B> f) =>
        Functor.map(this, f).AsMonad().As();

    [Pure]
    public Identity<B> Map<B>(Transducer<A, B> f) =>
        Functor.map(this, f).AsMonad().As();

    [Pure]
    public Identity<B> Select<B>(Func<A, B> f) =>
        Functor.map(this, f).AsMonad().As();

    [Pure]
    public Identity<B> Bind<B>(Func<A, Identity<B>> f) =>
        Monad.bind<Identity, Identity<B>, A, B>(this, f);

    [Pure]
    public Identity<B> Bind<B>(Func<A, Transducer<Unit, B>> f) =>
        Monad.bind<Identity, Identity<B>, A, B>(this, x => new Identity<B>(f(x)));

    [Pure]
    public Identity<B> Bind<B>(Transducer<A, Identity<B>> f) =>
        Monad.bind<Identity, Identity<B>, A, B>(this, f);

    [Pure]
    public Identity<C> SelectMany<B, C>(Func<A, Identity<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure]
    public Identity<C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> bind, Func<A, B, C> project) =>
        Bind(x => new Identity<C>(bind(x).Map(y => project(x, y))));

    [Pure]
    public Transducer<Unit, A> ToTransducer() =>
        morphism ?? throw new BottomException();
}
