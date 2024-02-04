using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Equivalent of `Either<Error, A>`
/// Called `Fin` because it is expected to be used as the concrete result of a computation
/// </summary>
[Serializable]
public readonly struct Fin<A> : 
    IComparable<Fin<A>>, 
    IComparable, 
    IEquatable<Fin<A>>,
    IEnumerable<A>,
    ISerializable
{
    readonly Either<Error, A> either;

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
        get => either.IsRight;
    }

    [Pure]
    public bool IsFail
    {
        [MethodImpl(Opt.Default)]
        get => either.IsLeft;
    }

    [Pure]
    public bool IsBottom
    {
        [MethodImpl(Opt.Default)]
        get => either.IsBottom;
    }
        
    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    [Pure]
    public object? Case =>
        either.Case;

    /// <summary>
    /// Equality
    /// </summary>
    public override bool Equals(object? obj) =>
        obj is Fin<A> ma && Equals(ma);

    /// <summary>
    /// Get hash code
    /// </summary>
    public override int GetHashCode() =>
        IsSucc
            ? either.GetHashCode()
            : FNV32.OffsetBasis;

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
        ma.IsSucc
            ? ma.Value
            : throw new EitherIsNotRightException();

    [Pure, MethodImpl(Opt.Default)]
    public static explicit operator Error(Fin<A> ma) =>
        ma.IsFail
            ? ma.Error
            : throw new EitherIsNotLeftException();

    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> operator |(Fin<A> left, Fin<A> right) =>
        left.IsSucc
            ? left
            : right;

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
        lhs < FinSucc(rhs);

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(Fin<A> lhs, A rhs) =>
        lhs <= FinSucc(rhs);

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(Fin<A> lhs, A rhs) =>
        lhs > FinSucc(rhs);

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(Fin<A> lhs, A rhs) =>
        lhs >= FinSucc(rhs);

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <(A lhs, Fin<A> rhs) =>
        FinSucc(lhs) < rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator <=(A lhs, Fin<A> rhs) =>
        FinSucc(lhs) <= rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >(A lhs, Fin<A>rhs) =>
        FinSucc(lhs) > rhs;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs < rhs</returns>
    [Pure]
    public static bool operator >=(A lhs, Fin<A>  rhs) =>
        FinSucc(lhs) >= rhs;

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
        lhs.Equals(FinFail<A>(rhs));

    /// <summary>
    /// Equality operator override
    /// </summary>
    [Pure]
    public static bool operator ==(Error lhs, Fin<A>  rhs) =>
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
        get => either.RightValue;  
    }

    internal Error Error 
    { 
        [MethodImpl(Opt.Default)]
        get => either.LeftValue;  
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
        IsSucc
            ? convert<B>(Value)
             .Map(Fin<B>.Succ)
             .IfNone(() => Fin<B>.Fail(Error.New($"Can't cast success value of `{nameof(A)}` to `{nameof(B)}` ")))
            : IsFail
                ? Fin<B>.Fail(either.LeftValue)
                : default;

    [Pure, MethodImpl(Opt.Default)]
    public int CompareTo(Fin<A> other) =>
        either.CompareTo(other.either);

    [Pure, MethodImpl(Opt.Default)]
    public bool Equals(Fin<A> other) =>
        either.Equals(other.either);

    [Pure, MethodImpl(Opt.Default)]
    public IEnumerator<A> GetEnumerator()
    {
        if (IsSucc)
        {
            yield return Value;
        }
    }

    [Pure, MethodImpl(Opt.Default)]
    public override string ToString() =>
        IsSucc
            ? $"Succ({Value})"
            : IsFail
                ? $"Fail({Error})"
                : "Bottom";

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("State", IsSucc);
        if (IsSucc)
        {
            info.AddValue("Succ", either.RightValue);
        }
        else if(IsFail)
        {
            info.AddValue("Fail", either.LeftValue);
        }
        else 
        {
            info.AddValue("Fail", Errors.Bottom);
        }
    }
        
    Fin(SerializationInfo info, StreamingContext context)
    {
        var isSucc = (bool?)info.GetValue("State", typeof(bool)) ?? throw new SerializationException();
        if (isSucc)
        {
            either = (A?)info.GetValue("Succ", typeof(A)) ?? throw new SerializationException();
        }
        else
        {
            either = (Error?)info.GetValue("Fail", typeof(Error)) ?? throw new SerializationException();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    [Pure, MethodImpl(Opt.Default)]
    public int CompareTo(object? obj) =>
        obj is Fin<A> t ? CompareTo(t) : 1;

    [Pure, MethodImpl(Opt.Default)]
    public B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        IsSucc
            ? Succ(Value)
            : Fail(Error);

    [MethodImpl(Opt.Default)]
    public Unit Match(Action<A> Succ, Action<Error> Fail)
    {
        if (IsSucc)
        {
            Succ(Value);
        }
        else if (IsFail)
        {
            Fail(Error);
        }
        return default;
    }

    [Pure, MethodImpl(Opt.Default)]
    public A IfFail(Func<Error, A> Fail) =>
        IsSucc
            ? Value
            : Fail(Error);

    [Pure, MethodImpl(Opt.Default)]
    public A IfFail(in A alternative) =>
        IsSucc
            ? Value
            : alternative;

    [MethodImpl(Opt.Default)]
    public Unit IfFail(Action<Error> Fail)
    {
        if (IsFail)
        {
            Fail(Error);
        }
        return default;
    }

    [MethodImpl(Opt.Default)]
    public Unit IfSucc(Action<A> Succ)
    {
        if (IsSucc)
        {
            Succ(Value);
        }
        return default;
    }

    [MethodImpl(Opt.Default)]
    public Unit Iter(Action<A> Succ)
    {
        if (IsSucc)
        {
            Succ(Value);
        }
        return default;
    }

    [Pure, MethodImpl(Opt.Default)]
    public Fin<A> Do(Action<A> Succ)
    {
        if (IsSucc)
        {
            Succ(Value);
        }
        return this;
    }

    [Pure, MethodImpl(Opt.Default)]
    public S Fold<S>(in S state, Func<S, A, S> f) =>
        IsSucc
            ? f(state, Value)
            : state;

    [Pure, MethodImpl(Opt.Default)]
    public S BiFold<S>(in S state, Func<S, A, S> Succ, Func<S, Error, S> Fail) =>
        IsSucc
            ? Succ(state, Value)
            : Fail(state, Error);

    [Pure, MethodImpl(Opt.Default)]
    public bool Exists(Func<A, bool> f) =>
        IsSucc && f(Value);

    [Pure, MethodImpl(Opt.Default)]
    public bool ForAll(Func<A, bool> f) =>
        IsFail || (IsSucc && f(Value));

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Map<B>(Func<A, B> f) =>
        IsSucc
            ? Fin<B>.Succ(f(Value))
            : Fin<B>.Fail(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
        IsSucc
            ? Fin<B>.Succ(Succ(Value))
            : Fin<B>.Fail(Fail(Error));

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> BiMap<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        IsSucc
            ? Fin<B>.Succ(Succ(Value))
            : Fin<B>.Succ(Fail(Error));

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Select<B>(Func<A, B> f) =>
        IsSucc
            ? Fin<B>.Succ(f(Value))
            : Fin<B>.Fail(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Bind<B>(Func<A, Fin<B>> f) =>
        IsSucc
            ? f(Value)
            : Fin<B>.Fail(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Bind<B>(Func<A, Pure<B>> f) =>
        IsSucc
            ? f(Value)
            : Fin<B>.Fail(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> Bind<B>(Func<A, Fail<Error>> f) =>
        IsSucc
            ? f(Value)
            : Fin<B>.Fail(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Fin<B> BiBind<B>(Func<A, Fin<B>> Succ, Func<Error, Fin<B>> Fail) =>
        IsSucc
            ? Succ(Value)
            : Fail(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Fin<C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project)
    {
        if(IsSucc)
        {
            var mb = bind(Value);
            return mb.IsSucc
                       ? project(either.RightValue, mb.Value)
                       : Fin<C>.Fail(mb.Error);
        }
        else
        {
            return Fin<C>.Fail(Error);
        }
    }

    [Pure, MethodImpl(Opt.Default)]
    public Fin<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project)
    {
        if(IsSucc)
        {
            var mb = bind(Value);
            return project(either.RightValue, mb.Value);
        }
        else
        {
            return Fin<C>.Fail(Error);
        }
    }

    [Pure, MethodImpl(Opt.Default)]
    public Fin<C> SelectMany<B, C>(Func<A, Fail<Error>> bind, Func<A, B, C> project)
    {
        if(IsSucc)
        {
            return bind(Value);
        }
        else
        {
            return Fin<C>.Fail(Error);
        }
    }

    /// <summary>
    /// Convert to a `Transducer`
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BottomException">If the either is in a bottom state then this exception will be thrown</exception>
    [Pure]
    public Transducer<Unit, Sum<Error, A>> ToTransducer()
    {
        var sum = IsSucc
                      ? Sum<Error, A>.Right(Value)
                      : IsFail
                          ? Sum<Error, A>.Left(Error)
                          : throw new BottomException();

        return lift<Unit, Sum<Error, A>>(_ => sum);
    }

    [Pure, MethodImpl(Opt.Default)]
    public Lst<A> ToList() =>
        IsSucc
            ? List(Value)
            : Empty;

    [Pure, MethodImpl(Opt.Default)]
    public Seq<A> ToSeq() =>
        IsSucc
            ? [Value]
            : Empty;

    [Pure, MethodImpl(Opt.Default)]
    public Arr<A> ToArr() =>
        IsSucc
            ? Array(Value)
            : Empty;

    [Pure, MethodImpl(Opt.Default)]
    public A[] ToArray() =>
        IsSucc
            ? [Value]
            : System.Array.Empty<A>();

    [Pure, MethodImpl(Opt.Default)]
    public Option<A> ToOption() =>
        IsSucc
            ? Some(Value)
            : None;

    [Pure, MethodImpl(Opt.Default)]
    [Obsolete(Change.UseEffMonadInstead)]
    public OptionAsync<A> ToOptionAsync() =>
        IsSucc
            ? SomeAsync(Value)
            : None;

    [Pure, MethodImpl(Opt.Default)]
    public Sum<Error, A> ToSum() =>
        IsSucc
            ? Sum<Error, A>.Right(Value)
            : Sum<Error, A>.Left(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Either<Error, A> ToEither() =>
        IsSucc
            ? Right<Error, A>(Value)
            : Left<Error, A>(Error);

    [Pure, MethodImpl(Opt.Default)]
    [Obsolete(Change.UseEffMonadInstead)]
    public EitherAsync<Error, A> ToEitherAsync() =>
        IsSucc
            ? RightAsync<Error, A>(Value)
            : LeftAsync<Error, A>(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> ToEff() =>
        IsSucc
            ? Pure(Value)
            : Fail(Error);

    [Pure, MethodImpl(Opt.Default)]
    public Aff<A> ToAff() =>
        IsSucc
            ? SuccessAff(Value)
            : FailAff<A>(Error);

    public A ThrowIfFail()
    {
        if (IsFail)
        {
            Error.Throw();
        }

        return Value;
    }
}
