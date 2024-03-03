using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Equivalent of `Either<Error, A>`
/// Called `Fin` because it is expected to be used as the concrete result of a computation
/// </summary>
public abstract record Fin<A> : 
    IEnumerable<A>,
    IComparable<Fin<A>>, 
    IComparable, 
    K<Fin, A>
{
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Succ(A value) => 
        new Fin.Succ<A>(value);

    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Fail(Error error) => 
        new Fin.Fail<A>(error);

    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Fail(string error) => 
        new Fin.Fail<A>(Error.New(error));

    /// <summary>
    /// Is the structure in a Success state?
    /// </summary>
    [Pure]
    public abstract bool IsSucc { get; }

    /// <summary>
    /// Is the structure in a Fail state?
    /// </summary>
    [Pure]
    public abstract bool IsFail { get; }

    /// <summary>
    /// Invokes the Succ or Fail function depending on the state of the structure
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Succ">Function to invoke if in a Succ state</param>
    /// <param name="Fail">Function to invoke if in a Fail state</param>
    /// <returns>The return value of the invoked function</returns>
    [Pure]
    public abstract B Match<B>(Func<A, B> Succ, Func<Error, B> Fail);

    /// <summary>
    /// Empty span
    /// </summary>
    [Pure]
    public abstract ReadOnlySpan<Error> FailSpan();

    /// <summary>
    /// Span of right value
    /// </summary>
    [Pure]
    public abstract ReadOnlySpan<A> SuccSpan();

    /// <summary>
    /// Compare this structure to another to find its relative ordering
    /// </summary>
    [Pure]
    public abstract int CompareTo<OrdA>(Fin<A> other)
        where OrdA : Ord<A>;    

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public abstract bool Equals<EqA>(Fin<A> other)
        where EqA : Eq<A>;    

    /// <summary>
    /// Unsafe access to the success value 
    /// </summary>
    internal abstract A SuccValue { get; }

    /// <summary>
    /// Unsafe access to the fail value 
    /// </summary>
    internal abstract Error FailValue { get; }

    /// <summary>
    /// Maps the value in the structure
    /// </summary>
    /// <param name="f">Map function</param>
    /// <returns>Mapped structure</returns>
    [Pure]
    public abstract Fin<B> Map<B>(Func<A, B> Succ);

    /// <summary>
    /// Bi-maps the structure
    /// </summary>
    /// <returns>Mapped Either</returns>
    [Pure]
    public abstract Fin<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <typeparam name="B">Resulting bound value</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Bound structure</returns>
    [Pure]
    public abstract Fin<B> Bind<B>(Func<A, Fin<B>> f);

    /// <summary>
    /// Bi-bind.  Allows mapping of both monad states
    /// </summary>
    [Pure]
    public abstract Fin<B> BiBind<B>(Func<A, Fin<B>> Succ, Func<Error, Fin<B>> Fail);

    /// <summary>
    /// Monoid empty
    /// </summary>
    [Pure]
    public static Fin<A> Empty { get; } = 
        new Fin.Fail<A>(Errors.None);

    /// <summary>
    /// Get hash code
    /// </summary>
    public override int GetHashCode() =>
        this switch
        {
            Fin.Succ<A> s => s.GetHashCode(),
            Fin.Fail<A> f => f.GetHashCode(),
            _             => 0
        };

    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(A value) =>
        new Fin.Succ<A>(value);
        
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(Error error) =>
        new Fin.Fail<A>(error);

    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(Either<Error, A> either) =>
        either switch
        {
            Either.Right<Error, A> (var r) => new Fin.Succ<A>(r),
            Either.Left<Error, A> (var l)  => new Fin.Fail<A>(l),
            _                              => throw new InvalidCastException()
        };
        
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(Pure<A> value) =>
        new Fin.Succ<A>(value.Value);
        
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Fin<A>(Fail<Error> value) =>
        new Fin.Fail<A>(value.Value);

    [Pure, MethodImpl(Opt.Default)]
    public static explicit operator A(Fin<A> ma) =>
        ma.SuccValue;

    [Pure, MethodImpl(Opt.Default)]
    public static explicit operator Error(Fin<A> ma) =>
        ma.FailValue;

    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> operator |(Fin<A> left, Fin<A> right) =>
        left.IsSucc ? left : right;

    [Pure, MethodImpl(Opt.Default)]
    public static bool operator true(Fin<A> ma) =>
        ma.IsSucc;

    [Pure, MethodImpl(Opt.Default)]
    public static bool operator false(Fin<A> ma) =>
        ma.IsFail;

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
    public static bool operator ==(Fin<A> lhs, Error rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Fin<A> lhs, A rhs) =>
        lhs.Equals((Fin<A>)rhs);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(A lhs, Fin<A> rhs) =>
        ((Fin<A>)lhs).Equals(rhs);

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

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(Fin<A> lhs, A rhs) =>
        !(lhs == rhs!);

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator !=(A lhs, Fin<A> rhs) =>
        !(lhs! == rhs);

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
    public int CompareTo(Fin<A>? other) =>
        other is null
            ? 1
            : CompareTo<OrdDefault<A>>(other);

    [Pure, MethodImpl(Opt.Default)]
    public virtual bool Equals(Fin<A>? other) =>
        other is not null && Equals<EqDefault<A>>(other);

    [Pure, MethodImpl(Opt.Default)]
    public IEnumerator<A> GetEnumerator()
    {
        foreach (var x in SuccSpan().ToArray()) 
            yield return x;
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    [Pure, MethodImpl(Opt.Default)]
    public int CompareTo(object? obj) =>
        obj is Fin<A> t ? CompareTo(t) : 1;

    [MethodImpl(Opt.Default)]
    public Unit Match(Action<A> Succ, Action<Error> Fail) =>
        Match(fun(Succ), fun(Fail));

    [Pure, MethodImpl(Opt.Default)]
    public A IfFail(Func<Error, A> Fail) =>
        Match(identity, Fail);

    [Pure, MethodImpl(Opt.Default)]
    public A IfFail(A alternative) =>
        Match(identity, _ => alternative);

    [MethodImpl(Opt.Default)]
    public Unit IfFail(Action<Error> Fail) =>
        Match(_ => { }, Fail);

    [MethodImpl(Opt.Default)]
    public Unit IfSucc(Action<A> Succ) =>
        Match(Succ, _ => { });

    [MethodImpl(Opt.Default)]
    public Unit Iter(Action<A> Succ) =>
        Match(Succ, _ => { });

    [Pure, MethodImpl(Opt.Default)]
    public S Fold<S>(S state, Func<S, A, S> f) =>
        Match(curry(f)(state), _ => state);    

    [Pure, MethodImpl(Opt.Default)]
    public S BiFold<S>(in S state, Func<S, A, S> Succ, Func<S, Error, S> Fail) =>
        Match(curry(Succ)(state), curry(Fail)(state));    

    [Pure, MethodImpl(Opt.Default)]
    public bool Exists(Func<A, bool> f) =>
        Match(f, _ => false);    

    [Pure, MethodImpl(Opt.Default)]
    public bool ForAll(Func<A, bool> f) =>
        Match(f, _ => true);    
    
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
    public Fin<B> Select<B>(Func<A, B> f) =>
        Map(f);

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Bind<B>(Func<A, K<Fin, B>> f) =>
        Bind(x => f(x).As());

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);

    [Pure, MethodImpl(Opt.Default)]
    public Fin<A> Bind(Func<A, Fail<Error>> f) =>
        Bind(x => Fail(f(x).Value));

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
        new(SuccSpan());

    [Pure, MethodImpl(Opt.Default)]
    public Seq<A> ToSeq() =>
        new(SuccSpan());

    [Pure, MethodImpl(Opt.Default)]
    public Arr<A> ToArray() =>
        new(SuccSpan());

    [Pure, MethodImpl(Opt.Default)]
    public Option<A> ToOption() =>
        IsSucc
            ? Option<A>.Some(SuccValue)
            : Option<A>.None;

    [Pure, MethodImpl(Opt.Default)]
    public Sum<Error, A> ToSum() =>
        IsSucc
            ? Sum<Error, A>.Right(SuccValue)
            : Sum<Error, A>.Left(FailValue);

    [Pure, MethodImpl(Opt.Default)]
    public Either<Error, A> ToEither() =>
        IsSucc
            ? Either<Error, A>.Right(SuccValue)
            : Either<Error, A>.Left(FailValue);

    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> ToEff() =>
        IsSucc
            ? Eff<A>.Pure(SuccValue)
            : Eff<A>.Fail(FailValue);

    public A ThrowIfFail()
    {
        if (IsFail)
        {
            FailValue.Throw();
        }
        return SuccValue;
    }
}
