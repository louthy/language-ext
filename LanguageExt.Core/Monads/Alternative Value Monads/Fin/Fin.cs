using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Equivalent of `Either<Error, A>`
/// Called `Fin` because it is expected to be used as the concrete result of a computation
/// </summary>
public readonly struct Fin<A> : 
    IComparable<Fin<A>>, 
    IComparable, 
    IEquatable<Fin<A>>,
    IEnumerable<A>,
    K<Fin, A>
{
    readonly Either<Error, A>? either;

    /// <summary>
    /// Ctor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal Fin(in Error error) =>
        either = error;

    /// <summary>
    /// Ctor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal Fin(in A value) =>
        either = value;

    /// <summary>
    /// Ctor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal Fin(in Either<Error, A> either) =>
        this.either = either;

    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Succ(A value) => 
        new (value);

    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Fail(Error error) => 
        new (error);

    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Fail(string error) => 
        new (Error.New(error));

    [Pure]
    public bool IsSucc
    {
        [MethodImpl(Opt.Default)]
        get => either?.IsRight ?? false;
    }

    [Pure]
    public bool IsFail
    {
        [MethodImpl(Opt.Default)]
        get => either?.IsLeft ?? true;
    }

    [Pure]
    public bool IsBottom
    {
        [MethodImpl(Opt.Default)]
        get => either is null;
    }

    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    [Pure]
    public object? Case =>
        either switch
        {
            Right<Error, A> r => r.Value,
            Left<Error, A> l  => l.Value,
            _                 => null
        };

    /// <summary>
    /// Equality
    /// </summary>
    public override bool Equals(object? obj) =>
        obj is Fin<A> ma && Equals(ma);

    /// <summary>
    /// Get hash code
    /// </summary>
    public override int GetHashCode() =>
        either?.GetHashCode() ?? 0;

    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(A value) =>
        Succ(value);
        
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(Error error) =>
        Fail(error);
        
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(Either<Error, A> either) =>
        new(either);
        
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(Pure<A> value) =>
        new(value.Value);
        
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(Fail<Error> value) =>
        new(value.Value);

    [Pure, MethodImpl(Opt.Default)]
    public static explicit operator A(Fin<A> ma) =>
        ma.either is { } e
          ? e.RightValue
          : throw new InvalidCastException();

    [Pure, MethodImpl(Opt.Default)]
    public static explicit operator Error(Fin<A> ma) =>
        ma.either is { } e
            ? e.LeftValue
            : throw new InvalidCastException();

    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> operator |(Fin<A> left, Fin<A> right) =>
        (left.either, right.either) switch
        {
            (null, null)  => Fail(Errors.Bottom),
            (var l, null) => l,
            (null, var r) => r,
            var (l, r)    => l | r
        };

    [Pure, MethodImpl(Opt.Default)]
    public static bool operator true(Fin<A> ma) =>
        ma.IsSucc;

    [Pure, MethodImpl(Opt.Default)]
    public static bool operator false(Fin<A> ma) =>
        !ma.IsSucc;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Fin<A> lhs, A rhs) =>
        lhs < (Fin<A>)rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Fin<A> lhs, A rhs) =>
        lhs <= (Fin<A>)rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Fin<A> lhs, A rhs) =>
        lhs > (Fin<A>)rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Fin<A> lhs, A rhs) =>
        lhs >= (Fin<A>)rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(A lhs, Fin<A> rhs) =>
        (Fin<A>)lhs < rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(A lhs, Fin<A> rhs) =>
        (Fin<A>)lhs <= rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(A lhs, Fin<A>rhs) =>
        (Fin<A>)lhs > rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(A lhs, Fin<A>  rhs) =>
        (Fin<A>)lhs >= rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(Fin<A> lhs, Fin<A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs <= rhs</returns>
    [Pure]
    public static bool operator <=(Fin<A> lhs, Fin<A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs > rhs</returns>
    [Pure]
    public static bool operator >(Fin<A> lhs, Fin<A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs >= rhs</returns>
    [Pure]
    public static bool operator >=(Fin<A> lhs, Fin<A> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Fin<A> lhs, Fin<A> rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Fin<A> lhs, Fin<A> rhs) =>
        !(lhs == rhs);
        
    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Fin<A> lhs, Error rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Error lhs, Fin<A> rhs) =>
        FinFail<A>(lhs).Equals(rhs);

    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Fin<A> lhs, Error rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Non-equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Error lhs, Fin<A> rhs) =>
        !(lhs == rhs);

    internal A Value 
    { 
        [MethodImpl(Opt.Default)]
        get => (A)this;  
    }

    internal Error Error 
    { 
        [MethodImpl(Opt.Default)]
        get => (Error)this;  
    }
        
    [Pure, MethodImpl(Opt.Default)]
    static Option<T> convert<T>(in object? value)
    {
        if (value == null)
        {
            return None;
        }

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return None;
        }
    }

    [Pure, MethodImpl(Opt.Default)]
    internal Fin<B> Cast<B>() =>
        Bind(x => convert<B>(x)
                    .Map(Fin<B>.Succ)
                    .IfNone(() => Fin<B>.Fail(Error.New($"Can't cast success value of `{nameof(A)}` to `{nameof(B)}` "))));

    [Pure, MethodImpl(Opt.Default)]
    public int CompareTo(Fin<A> other) =>
        (either, other.either) switch
        {
            (null, null) => 0,
            (null, _)    => -1,
            (_, null)    => 1,
            var (l, r)   => l.CompareTo(r)
        };

    [Pure, MethodImpl(Opt.Default)]
    public bool Equals(Fin<A> other) =>
        (either, other.either) switch
        {
            (null, null) => true,
            (null, _)    => false,
            (_, null)    => false,
            var (l, r)   => l.Equals(r)
        };

    [Pure, MethodImpl(Opt.Default)]
    public IEnumerator<A> GetEnumerator() =>
        either is { } e
            ? e.GetEnumerator()
            : Seq<A>.Empty.GetEnumerator();    

    [Pure, MethodImpl(Opt.Default)]
    public override string ToString() =>
        IsSucc
            ? $"Succ({Value})"
            : IsFail
                ? $"Fail({Error})"
                : "Bottom";

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    [Pure, MethodImpl(Opt.Default)]
    public int CompareTo(object? obj) =>
        obj is Fin<A> t ? CompareTo(t) : 1;

    [Pure, MethodImpl(Opt.Default)]
    public B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        either is { } e
            ? e.Match(Fail, Succ)
            : Fail(Errors.Bottom);    

    [MethodImpl(Opt.Default)]
    public Unit Match(Action<A> Succ, Action<Error> Fail) =>
        either is { } e
            ? e.Match(Fail, Succ)
            : unit;    

    [Pure, MethodImpl(Opt.Default)]
    public A IfFail(Func<Error, A> Fail) =>
        either is { } e
            ? e.IfLeft(Fail)
            : Fail(Errors.Bottom);    

    [Pure, MethodImpl(Opt.Default)]
    public A IfFail(in A alternative) =>
        either is { } e
            ? e.IfLeft(alternative)
            : alternative;    

    [MethodImpl(Opt.Default)]
    public Unit IfFail(Action<Error> Fail) =>
        either is { } e
            ? e.IfLeft(Fail)
            : unit;    

    [MethodImpl(Opt.Default)]
    public Unit IfSucc(Action<A> Succ) =>
        either is { } e
            ? e.IfRight(Succ)
            : unit;    

    [MethodImpl(Opt.Default)]
    public Unit Iter(Action<A> Succ) =>
        either is { } e
            ? e.IfRight(Succ)
            : unit;    

    [Pure, MethodImpl(Opt.Default)]
    public Fin<A> Do(Action<A> Succ) =>
        either is { } e
            ? e.Do(Succ)
            : this;    

    [Pure, MethodImpl(Opt.Default)]
    public S Fold<S>(in S state, Func<S, A, S> f) =>
        either is { } e
            ? e.Fold(state, f)
            : state;    

    [Pure, MethodImpl(Opt.Default)]
    public S BiFold<S>(in S state, Func<S, A, S> Succ, Func<S, Error, S> Fail) =>
        either is { } e
            ? e.BiFold(state, Fail, Succ)
            : state;    

    [Pure, MethodImpl(Opt.Default)]
    public bool Exists(Func<A, bool> f) =>
        either is { } e && e.Exists(f);    

    [Pure, MethodImpl(Opt.Default)]
    public bool ForAll(Func<A, bool> f) =>
        either is not { } e || e.ForAll(f);    
    
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
    public K<F, Fin<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));
    
    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Map<B>(Func<A, B> f) =>
        either is { } e
            ? e.Map(f)
            : Errors.Bottom;    

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
        either is { } e
            ? e.BiMap(Fail, Succ)
            : Fail(Errors.Bottom);    

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Select<B>(Func<A, B> f) =>
        either is { } e
            ? e.Select(f)
            : Errors.Bottom;

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Bind<B>(Func<A, Fin<B>> f) =>
        either switch
        {
            Right<Error, A> r => f(r.Value),
            Left<Error, A> l  => l.Value,
            _                 => Errors.Bottom
        };

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Bind<B>(Func<A, K<Fin, B>> f) =>
        Bind(x => f(x).As());

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Bind<B>(Func<A, Pure<B>> f) =>
        either switch
        {
            Right<Error, A> r => f(r.Value),
            Left<Error, A> l  => l.Value,
            _                 => Errors.Bottom
        };

    [Pure, MethodImpl(Opt.Default)]
    public Fin<A> Bind(Func<A, Fail<Error>> f) =>
        either switch
        {
            Right<Error, A> r => f(r.Value),
            Left<Error, A> l  => l.Value,
            _                 => Errors.Bottom
        };

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> BiBind<B>(Func<A, Fin<B>> Succ, Func<Error, Fin<B>> Fail) =>
        either switch
        {
            Right<Error, A> r => Succ(r.Value),
            Left<Error, A> l  => Fail(l.Value),
            _                 => Errors.Bottom
        };

    [Pure, MethodImpl(Opt.Default)]
    public Fin<C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure, MethodImpl(Opt.Default)]
    public Fin<C> SelectMany<B, C>(Func<A, K<Fin, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).As().Map(y => project(x, y)));

    [Pure, MethodImpl(Opt.Default)]
    public Fin<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure, MethodImpl(Opt.Default)]
    public Fin<Unit> SelectMany(Func<A, Fail<Error>> bind, Func<A, Error, Unit> project) =>
        Map(x => ignore(bind(x)));
    
    [Pure, MethodImpl(Opt.Default)]
    public Lst<A> ToList() =>
        either is { } e
            ? e.ToList()
            : [];    

    [Pure, MethodImpl(Opt.Default)]
    public Seq<A> ToSeq() =>
        either is { } e
            ? e.ToSeq()
            : [];    

    [Pure, MethodImpl(Opt.Default)]
    public Arr<A> ToArray() =>
        either is { } e
            ? e.ToArr()
            : [];    

    [Pure, MethodImpl(Opt.Default)]
    public Option<A> ToOption() =>
        either is { } e
            ? e.ToOption()
            : [];    

    [Pure, MethodImpl(Opt.Default)]
    public Sum<Error, A> ToSum() =>
        either is { } e
            ? e.ToSum()
            : throw new BottomException();    

    [Pure, MethodImpl(Opt.Default)]
    public Either<Error, A> ToEither() =>
        either ?? throw new BottomException();

    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> ToEff() =>
        either is { } e
            ? e.ToEff()
            : throw new BottomException();    

    public A ThrowIfFail()
    {
        if (IsBottom)
        {
            throw new BottomException();
        }
        if (IsFail)
        {
            Error.Throw();
        }
        return Value;
    }
}
