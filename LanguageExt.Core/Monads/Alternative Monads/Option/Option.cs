using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Runtime.Serialization;
using System.Collections;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Discriminated union type.  Can be in one of two states:
/// 
///     Some(a)
///     None
///     
/// </summary>
/// <typeparam name="A">Bound value</typeparam>
[Serializable]
public readonly struct Option<A> :
    IOptional,
    IEquatable<Option<A>>,
    IComparable<Option<A>>,
    IComparable,
    ISerializable,
    Fallible<Option<A>, Option, Unit, A>,
    Monoid<Option<A>>
{
    internal readonly A? Value;
    internal readonly bool isSome;

    /// <summary>
    /// None
    /// </summary>
    public static readonly Option<A> None =
        default;

    /// <summary>
    /// Constructor
    /// </summary>
    internal Option(A value)
    {
        Value  = value;
        isSome = true;
    }

    /// <summary>
    /// Ctor that facilitates serialisation
    /// </summary>
    /// <param name="option">None or Some A.</param>
    public Option(IEnumerable<A> option)
    {
        var first = option.Take(1).ToArray();
        isSome = first.Length == 1;
        Value = isSome
                    ? first[0]
                    : default;
    }

    Option(SerializationInfo info, StreamingContext context)
    {
        isSome = info.GetValue("IsSome", typeof(bool)) is true;
        if(isSome)
        {
            Value = info.GetValue("Value", typeof(A)) is A x ? x : throw new SerializationException();
        }
        else
        {
            Value = default;
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("IsSome", IsSome);
        if(IsSome) info.AddValue("Value", Value);
    }

    /// <summary>
    /// Reference version of option for use in pattern-matching
    /// </summary>
    /// <remarks>
    ///
    ///     Some = result is A
    ///     None = result is null
    ///
    /// </remarks>
    [Pure]
    public object? Case =>
        IsSome
            ? Value
            : default;

    /// <summary>
    /// Uses the `EqDefault` instance to do an equality check on the bound value.  
    /// To use anything other than the default call `oa.Equals〈EqA〉(ob)`
    /// where `EqA` is an instance derived from `Eq〈A〉`
    /// </summary>
    /// <remarks>
    /// This uses the `EqDefault` instance for comparison of the bound `A` values.  
    /// The `EqDefault` instance wraps up the .NET `EqualityComparer.Default`
    /// behaviour.  
    /// </remarks>
    /// <param name="other">The `Option` type to compare this type with</param>
    /// <returns>`True` if `this` and `other` are equal</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Option<A> other) =>
        Equals<EqDefault<A>>(other);

    /// <summary>
    /// Uses the `EqA` instance to do an equality check on the bound value.  
    /// </summary>
    /// <param name="other">The `Option` type to compare this type with</param>
    /// <returns>`True` if `this` and `other` are equal</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals<EqA>(Option<A> other) where EqA : Eq<A>
    {
        var yIsSome = other.IsSome;
        var xIsNone = !isSome;
        var yIsNone = !yIsSome;
        return xIsNone && yIsNone || isSome && yIsSome && EqA.Equals(Value!, other.Value!);
    }

    /// <summary>
    /// Uses the `OrdDefault` instance to do an ordering comparison on the bound 
    /// value.  To use anything other than the default call  `this.Compare〈OrdA〉(this, other)`, 
    /// where `OrdA` is an instance derived  from `Ord〈A〉`
    /// </summary>
    /// <param name="other">The `Option` type to compare `this` type with</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Option<A> other) =>
        CompareTo<OrdDefault<A>>(other);

    /// <summary>
    /// Uses the `Ord` instance provided to do an ordering comparison on the bound 
    /// value.  
    /// </summary>
    /// <param name="other">The `Option` type to compare `this` type with</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo<OrdA>(Option<A> other) where OrdA : Ord<A>
    {
        var yIsSome = other.IsSome;
        var xIsNone = !isSome;
        var yIsNone = !yIsSome;

        if (xIsNone && yIsNone) return 0;
        if (isSome  && yIsNone) return 1;
        if (xIsNone) return -1;

        return OrdA.Compare(Value!, other.Value!);
    }

    /// <summary>
    /// Explicit conversion operator from `Option〈A〉` to `A`
    /// </summary>
    /// <param name="a">None value</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator A(Option<A> ma)
    {
        var opExplicit = ma.IsSome
                             ? ma.Value
                             : throw new InvalidCastException("Option is not in a Some state");
        
        return opExplicit!;
    }

    /// <summary>
    /// Implicit conversion operator from A to Option〈A〉
    /// </summary>
    /// <param name="a">Unit value</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<A>(A? a) =>
        Optional(a);

    /// <summary>
    /// Implicit conversion operator from None to Option〈A〉
    /// </summary>
    /// <param name="a">None value</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<A>(Fail<Unit> a) =>
        default;

    /// <summary>
    /// Implicit conversion operator from None to Option〈A〉
    /// </summary>
    /// <param name="a">None value</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<A>(in Unit fail) => 
        default;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈 rhs</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Option<A> lhs, Option<A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs〈= rhs</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Option<A> lhs, Option<A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉rhs</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Option<A> lhs, Option<A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    /// <summary>
    /// Comparison operator
    /// </summary>
    /// <param name="lhs">The left hand side of the operation</param>
    /// <param name="rhs">The right hand side of the operation</param>
    /// <returns>True if lhs 〉= rhs</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Option<A> lhs, Option<A> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <remarks>
    /// This uses the EqDefault instance for comparison of the bound A values.  
    /// The EqDefault instance wraps up the .NET EqualityComparer.Default 
    /// behaviour.  For more control over equality you can call:
    /// 
    ///     equals〈EQ, A〉(lhs, rhs);
    ///     
    /// Where EQ is a struct derived from Eq〈A〉.  For example: 
    /// 
    ///     equals〈EqString, string〉(lhs, rhs);
    ///     equals〈EqArray〈int〉, int[]〉(lhs, rhs);
    ///     
    /// </remarks>
    /// <param name="lhs">Left hand side of the operation</param>
    /// <param name="rhs">Right hand side of the operation</param>
    /// <returns>True if the values are equal</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Option<A> lhs, Option<A> rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Non-equality operator
    /// </summary>
    /// <remarks>
    /// This uses the EqDefault instance for comparison of the A value.  
    /// The EqDefault trait wraps up the .NET EqualityComparer.Default 
    /// behaviour.  For more control over equality you can call:
    /// 
    ///     !equals〈EQ, A〉(lhs, rhs);
    ///     
    /// Where EQ is a struct derived from Eq〈A〉.  For example: 
    /// 
    ///     !equals〈EqString, string〉(lhs, rhs);
    ///     !equals〈EqArray〈int〉, int[]〉(lhs, rhs);
    ///     
    /// </remarks>
    /// <param name="lhs">Left hand side of the operation</param>
    /// <param name="rhs">Right hand side of the operation</param>
    /// <returns>True if the values are equal</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Option<A> lhs, Option<A> rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Coalescing operator
    /// </summary>
    /// <param name="lhs">Left hand side of the operation</param>
    /// <param name="rhs">Right hand side of the operation</param>
    /// <returns>if lhs is Some then lhs, else rhs</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Option<A> operator |(Option<A> lhs, Option<A> rhs) =>
        lhs.IsSome
            ? lhs
            : rhs;
    
    [Pure, MethodImpl(Opt.Default)]
    public static Option<A> operator |(K<Option, A> lhs, Option<A> rhs) =>
        lhs.Choose(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Option<A> operator |(Option<A> lhs, K<Option, A> rhs) =>
        lhs.Choose(rhs.As()).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Option<A> operator |(Option<A> ma, Pure<A> mb) =>
        ma.Choose(pure<Option, A>(mb.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Option<A> operator |(Option<A> ma, Fail<Unit> mb) =>
        ma.Choose(None).As();

    [Pure, MethodImpl(Opt.Default)]
    public static Option<A> operator |(Option<A> ma, CatchM<Unit, Option, A> mb) =>
        (ma.Kind() | mb).As(); 

    /// <summary>
    /// Truth operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Option<A> value) =>
        value.IsSome;

    /// <summary>
    /// Falsity operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Option<A> value) =>
        value.IsNone;

    /// <summary>
    /// DO NOT USE - Use the Structural equality variant of this method Equals〈EQ, A〉(y)
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) =>
        obj is Option<A> opt && Equals(opt);

    /// <summary>
    /// Calculate the hash-code from the bound value, unless the Option is in a None
    /// state, in which case the hash-code will be 0
    /// </summary>
    /// <returns>Hash-code from the bound value, unless the Option is in a None
    /// state, in which case the hash-code will be 0</returns>
    [Pure]
    public override int GetHashCode() =>
        isSome 
            ? Value?.GetHashCode() ?? 0 
            : 0;
        
    [Pure]
    public int CompareTo(object? obj) =>
        obj is Option<A> t ? CompareTo(t) : 1;

    /// <summary>
    /// Get a string representation of the Option
    /// </summary>
    /// <returns>String representation of the Option</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() =>
        isSome
            ? $"Some({Value?.ToString() ?? ""})"
            : "None";

    /// <summary>
    /// Is the option in a Some state
    /// </summary>
    [Pure]
    public bool IsSome =>
        isSome;

    /// <summary>
    /// Is the option in a None state
    /// </summary>
    [Pure]
    public bool IsNone =>
        !isSome;

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<A> Do(Action<A> f)
    {
        Iter(f);
        return this;
    }

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="f">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<B> Select<B>(Func<A, B> f) =>
        isSome
            ? Option.Some(f(Value!))
            : default;

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="f">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<B> Map<B>(Func<A, B> f) =>
        isSome
            ? Option.Some(f(Value!))
            : default;
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Option<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
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
    public K<M, Option<B>> TraverseM<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        M.Map(x => x.As(), Traversable.traverseM(f, this));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<B> Bind<B>(Func<A, Option<B>> f) =>
        isSome
            ? f(Value!)
            : default;

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<B> Bind<B>(Func<A, K<Option, B>> f) =>
        isSome
            ? f(Value!).As()
            : default;

    /// <summary>
    /// Bi-bind.  Allows mapping of both monad states
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<B> BiBind<B>(Func<A, Option<B>> Some, Func<Option<B>> None) =>
        isSome
            ? Some(Value!)
            : None();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<C> SelectMany<B, C>(
        Func<A, Option<B>> bind,
        Func<A, B, C> project)
    {
        if (IsNone) return default;
        var mb = bind(Value!);
        if (mb.IsNone) return default;
        return project(Value!, mb.Value!);
    }

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<C> SelectMany<C>(
        Func<A, Fail<Unit>> bind,
        Func<A, Unit, C> project) =>
        Option<C>.None;

    /// <summary>
    /// Match operation with an untyped value for Some. This can be
    /// useful for serialisation and dealing with the IOptional interface
    /// </summary>
    /// <typeparam name="R">The return type</typeparam>
    /// <param name="Some">Operation to perform if the option is in a Some state</param>
    /// <param name="None">Operation to perform if the option is in a None state</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R MatchUntyped<R>(Func<object?, R> Some, Func<R> None) =>
        IsSome
            ? Some(Value)
            : None();    

    /// <summary>
    /// Get the Type of the bound value
    /// </summary>
    /// <returns>Type of the bound value</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Type GetUnderlyingType() => 
        typeof(A);

    /// <summary>
    /// If the Option is in a `Some` state then the span will contain one itemm otherwise empty.
    /// </summary>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<A> ToSpan() =>
        IsSome
            ? new([Value!])
            : ReadOnlySpan<A>.Empty;
    
    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <returns>An enumerable of zero or one items</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> ToArray() =>
        isSome
            ? Arr.create(Value!)
            : [];

    /// <summary>
    /// Convert the Option to an immutable list of zero or one items
    /// </summary>
    /// <returns>An immutable list of zero or one items</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Lst<A> ToList() =>
        isSome
            ? List.create(Value!)
            : [];

    /// <summary>
    /// Convert the Option to an enumerable sequence of zero or one items
    /// </summary>
    /// <returns>An enumerable sequence of zero or one items</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> ToSeq() =>
        isSome
            ? [Value!]
            : [];

    /// <summary>
    /// Convert the `Option` to an enumerable of zero or one items
    /// </summary>
    /// <returns>An enumerable of zero or one items</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<A> AsEnumerable() =>
        IsSome ? [Value!] : [];

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <returns>An enumerable of zero or one items</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Iterable<A> ToIterable() =>
        IsSome ? [Value!] : [];
        
    /// <summary>
    /// Convert the structure to an Eff
    /// </summary>
    /// <returns>An Eff representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Eff<A> ToEff() =>
        ToEff(Errors.None);

    /// <summary>
    /// Convert to an Option transformer with embedded IO
    /// </summary>
    /// <returns></returns>
    [Pure]
    public OptionT<IO, A> ToIO() =>
        OptionT.lift<IO, A>(this);

    /// <summary>
    /// Convert the structure to an `Eff`
    /// </summary>
    /// <param name="Fail">Default value if the structure is in a None state</param>
    /// <returns>An Eff representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Eff<A> ToEff(Error Fail) =>
        isSome
            ? Pure(Value!)
            : Prelude.Fail(Fail);

    /// <summary>
    /// Convert the structure to a Fin
    /// </summary>
    /// <returns>A Fin representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Fin<A> ToFin() =>
        ToFin(Errors.None);

    /// <summary>
    /// Convert the structure to a Fin
    /// </summary>
    /// <param name="Fail">Default value if the structure is in a None state</param>
    /// <returns>A Fin representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Fin<A> ToFin(Error Fail) =>
        isSome
            ? Fin.Succ<A>(Value!)
            : Fin.Fail<A>(Fail);

    /// <summary>
    /// Convert the structure to an Either
    /// </summary>
    /// <param name="defaultLeftValue">Default value if the structure is in a None state</param>
    /// <returns>An Either representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Either<L, A> ToEither<L>(L defaultLeftValue) =>
        isSome
            ? Right<L, A>(Value!)
            : Left<L, A>(defaultLeftValue);

    /// <summary>
    /// Convert the structure to an Either
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Either<L, A> ToEither<L>() where L : Monoid<L> =>
        isSome
            ? Right<L, A>(Value!)
            : Left<L, A>(L.Empty);
    
    /// <summary>
    /// Convert the structure to an Either
    /// </summary>
    /// <param name="Left">Function to invoke to get a default value if the 
    /// structure is in a None state</param>
    /// <returns>An Either representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Either<L, A> ToEither<L>(Func<L> Left) =>
        isSome
            ? Right<L, A>(Value!)
            : Left<L, A>(Left());
    
    /// <summary>
    /// Convert the structure to a Validation
    /// </summary>
    /// <param name="Fail">Function to invoke to get a default value if the 
    /// structure is in a None state</param>
    /// <returns>An Validation representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Validation<L, A> ToValidation<L>(Func<L> Fail) where L : Monoid<L> =>
        isSome
            ? Success<L, A>(Value!)
            : Fail<L, A>(Fail());
    
    /// <summary>
    /// Convert the structure to a Validation
    /// </summary>
    /// <param name="Fail">Default value if the structure is in a None state</param>
    /// <returns>An Validation representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Validation<L, A> ToValidation<L>(L Fail) where L : Monoid<L> =>
        isSome
            ? Success<L, A>(Value!)
            : Fail<L, A>(Fail);
    
    /// <summary>
    /// Convert the structure to a Validation
    /// </summary>
    /// <returns>An Validation representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Validation<L, A> ToValidation<L>() where L : Monoid<L> =>
        isSome
            ? Success<L, A>(Value!)
            : Fail<L, A>(L.Empty);

    /// <summary>
    /// Fluent pattern matching.  Provide a Some handler and then follow
    /// on fluently with .None(...) to complete the matching operation.
    /// This is for dispatching actions, use Some〈A, B〉(...) to return a value
    /// from the match operation.
    /// </summary>
    /// <param name="f">The `Some(x)` match operation</param>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SomeUnitContext<A> Some(Action<A> f) =>
        new (this, f);

    /// <summary>
    /// Fluent pattern matching.  Provide a Some handler and then follow
    /// on fluently with .None(...) to complete the matching operation.
    /// This is for returning a value from the match operation, to dispatch
    /// an action instead, use Some〈A〉(...)
    /// </summary>
    /// <typeparam name="B">Match operation return value type</typeparam>
    /// <param name="f">The `Some(x)` match operation</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SomeContext<A, B> Some<B>(Func<A, B> f) =>
        new (this, f);

    /// <summary>
    /// Match the two states of the Option and return a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. Must not return null.</param>
    /// <param name="None">None match operation. Must not return null.</param>
    /// <returns>A non-null B</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Match<B>(Func<A, B> Some, Func<B> None) =>
        isSome
            ? Some(Value!)
            : None();

    /// <summary>
    /// Match the two states of the Option and return a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. Must not return null.</param>
    /// <param name="None">None match operation. Must not return null.</param>
    /// <returns>A non-null B</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Match<B>(Func<A, B> Some, B None) =>
        isSome
            ? Some(Value!)
            : None;

    /// <summary>
    /// Match the two states of the Option
    /// </summary>
    /// <param name="Some">Some match operation</param>
    /// <param name="None">None match operation</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit Match(Action<A> Some, Action None)
    {
        if(isSome)
        {
            Some(Value!);
        }
        else
        {
            None();
        }
        return default;
    }

    /// <summary>
    /// Invokes the action if Option is in the Some state, otherwise nothing happens.
    /// </summary>
    /// <param name="f">Action to invoke if Option is in the Some state</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit IfSome(Action<A> f)
    {
        if(isSome)
        {
            f(Value!);
        }
        return default;
    }

    /// <summary>
    /// Invokes the f function if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    /// <param name="f">Function to invoke if Option is in the Some state</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit IfSome(Func<A, Unit> f)
    {
        if (isSome)
        {
            f(Value!);
        }
        return default;
    }

    /// <summary>
    /// Returns the result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null return value from the None operation</remarks>
    /// <param name="None">Operation to invoke if the structure is in a None state</param>
    /// <returns>Result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public A IfNone(Func<A> None) =>
        isSome
            ? Value!
            : None();

    /// <summary>
    /// Invokes the action if Option is in the None state, otherwise nothing happens.
    /// </summary>
    /// <param name="f">Action to invoke if Option is in the None state</param>
    public Unit IfNone(Action None)
    {
        if (IsNone) None();
        return unit;
    }
        
    /// <summary>
    /// Returns the noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null noneValue</remarks>
    /// <param name="noneValue">Value to return if in a None state</param>
    /// <returns>noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public A IfNone(A noneValue) =>
        isSome
            ? Value!
            : noneValue;

    /// <summary>
    /// <para>
    /// Option types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para>
    /// <para>
    /// Note that, since the head of the resulting expression is produced by
    /// an application of the operator to the first element of the list,
    /// 'Fold' can produce a terminating expression from an infinite list.
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folder function, applied if Option is in a Some state</param>
    /// <returns>The aggregate state</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public S Fold<S>(S state, Func<S, A, S> folder) =>
        isSome
            ? folder(state, Value!)
            : state;

    /// <summary>
    /// <para>
    /// Option types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para>
    /// <para>
    /// Note that, since the head of the resulting expression is produced by
    /// an application of the operator to the first element of the list,
    /// 'Fold' can produce a terminating expression from an infinite list.
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folder function, applied if Option is in a Some state</param>
    /// <returns>The aggregate state</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public S FoldBack<S>(S state, Func<S, A, S> folder) =>
        isSome
            ? folder(state, Value!)
            : state;

    /// <summary>
    /// <para>
    /// Option types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para>
    /// <para>
    /// Note that, since the head of the resulting expression is produced by
    /// an application of the operator to the first element of the list,
    /// 'Fold' can produce a terminating expression from an infinite list.
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Folder function, applied if Option is in a Some state</param>
    /// <param name="None">Folder function, applied if Option is in a None state</param>
    /// <returns>The aggregate state</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public S BiFold<S>(S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
        isSome
            ? Some(state, Value!)
            : None(state, unit);

    /// <summary>
    /// <para>
    /// Option types are like lists of 0 or 1 items, and therefore follow the 
    /// same rules when folding.
    /// </para><para>
    /// In the case of lists, 'Fold', when applied to a binary
    /// operator, a starting value(typically the left-identity of the operator),
    /// and a list, reduces the list using the binary operator, from left to
    /// right:
    /// </para>
    /// <para>
    /// Note that, since the head of the resulting expression is produced by
    /// an application of the operator to the first element of the list,
    /// 'Fold' can produce a terminating expression from an infinite list.
    /// </para>
    /// </summary>
    /// <typeparam name="S">Aggregate state type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Folder function, applied if Option is in a Some state</param>
    /// <param name="None">Folder function, applied if Option is in a None state</param>
    /// <returns>The aggregate state</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public S BiFold<S>(S state, Func<S, A, S> Some, Func<S, S> None) =>
        isSome
            ? Some(state, Value!)
            : None(state);

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<B> BiMap<B>(Func<A, B> Some, Func<Unit, B> None) =>
        Check.NullReturn(
            isSome
                ? Some(Value!)
                : None(unit));

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<B> BiMap<B>(Func<A, B> Some, Func<B> None) =>
        isSome
            ? Some(Value!)
            : None();

    /// <summary>
    /// <para>
    /// Return the number of bound values in this structure:
    /// </para>
    /// <para>
    ///     None = 0
    /// </para>
    /// <para>
    ///     Some = 1
    /// </para>
    /// </summary>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Count() =>
        isSome ? 1 : 0;

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned (because the predicate applies for-all values).
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the Option is in a None state then True is returned (because 
    /// the predicate applies for-all values).  If the Option is in a Some state
    /// the value is the result of running applying the bound value to the 
    /// predicate supplied.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ForAll(Func<A, bool> pred) =>
        !isSome || pred(Value!);

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned if invoking None returns True.
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the Some predicate supplied.        
    /// </summary>
    /// <param name="Some">Predicate to apply if in a Some state</param>
    /// <param name="None">Predicate to apply if in a None state</param>
    /// <returns>If the Option is in a None state then True is returned if 
    /// invoking None returns True. If the Option is in a Some state the value 
    /// is the result of running applying the bound value to the Some predicate 
    /// supplied.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool BiForAll(Func<A, bool> Some, Func<Unit, bool> None) =>
        isSome
            ? Some(Value!)
            : None(unit);

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned if invoking None returns True.
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the Some predicate supplied.        
    /// </summary>
    /// <param name="Some">Predicate to apply if in a Some state</param>
    /// <param name="None">Predicate to apply if in a None state</param>
    /// <returns>If the Option is in a None state then True is returned if 
    /// invoking None returns True. If the Option is in a Some state the value 
    /// is the result of running applying the bound value to the Some predicate 
    /// supplied.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool BiForAll(Func<A, bool> Some, Func<bool> None) =>
        isSome
            ? Some(Value!)
            : None();

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then False is returned.
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the Option is in a None state then False is returned. If the Option is in a Some state the value 
    /// is the result of running applying the bound value to the predicate 
    /// supplied.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Exists(Func<A, bool> pred) =>
        isSome && pred(Value!);

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned if invoking None returns True.
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the Some predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the Option is in a None state then True is returned if 
    /// invoking None returns True. If the Option is in a Some state the value 
    /// is the result of running applying the bound value to the Some predicate 
    /// supplied.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool BiExists(Func<A, bool> Some, Func<Unit, bool> None) =>
        isSome
            ? Some(Value!)
            : None(unit);

    /// <summary>
    /// Apply a predicate to the bound value.  If the Option is in a None state
    /// then True is returned if invoking None returns True.
    /// If the Option is in a Some state the value is the result of running 
    /// applying the bound value to the Some predicate supplied.        
    /// </summary>
    /// <param name="pred"></param>
    /// <returns>If the Option is in a None state then True is returned if 
    /// invoking None returns True. If the Option is in a Some state the value 
    /// is the result of running applying the bound value to the Some predicate 
    /// supplied.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool BiExists(Func<A, bool> Some, Func<bool> None) =>
        isSome
            ? Some(Value!)
            : None();

    /// <summary>
    /// Invoke an action for the bound value (if in a Some state)
    /// </summary>
    /// <param name="Some">Action to invoke</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit Iter(Action<A> Some)
    {
        if(isSome)
        {
            Some(Value!);
        }
        return unit;
    }

    /// <summary>
    /// Invoke an action depending on the state of the Option
    /// </summary>
    /// <param name="Some">Action to invoke if in a Some state</param>
    /// <param name="None">Action to invoke if in a None state</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit BiIter(Action<A> Some, Action<Unit> None)
    {
        if (isSome)
        {
            Some(Value!);
        }
        else
        {
            None(unit);
        }
        return unit;
    }

    /// <summary>
    /// Invoke an action depending on the state of the Option
    /// </summary>
    /// <param name="Some">Action to invoke if in a Some state</param>
    /// <param name="None">Action to invoke if in a None state</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit BiIter(Action<A> Some, Action None)
    {
        if (isSome)
        {
            Some(Value!);
        }
        else
        {
            None();
        }
        return unit;
    }

    /// <summary>
    /// Apply a predicate to the bound value (if in a Some state)
    /// </summary>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Some(x) if the Option is in a Some state and the predicate
    /// returns True.  None otherwise.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<A> Filter(Func<A, bool> pred) =>
        isSome && pred(Value!)
            ? this
            : default;

    /// <summary>
    /// Apply a predicate to the bound value (if in a Some state)
    /// </summary>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Some(x) if the Option is in a Some state and the predicate
    /// returns True.  None otherwise.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<A> Where(Func<A, bool> pred) =>
        isSome && pred(Value!)
            ? this
            : default;

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<Func<B, C>> ParMap<B, C>(Func<A, B, C> func) =>
        Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<Func<B, Func<C, D>>> ParMap<B, C, D>(Func<A, B, C, D> func) =>
        Map(curry(func));
        
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    // `Pure` support
    //

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    [Pure]
    public Option<B> Bind<B>(Func<A, Pure<B>> f) =>
        IsSome  
            ? f(Value!).ToOption()
            : Option<B>.None;


    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    [Pure]
    public Option<B> Bind<B>(Func<A, Fail<Unit>> f) =>
        Option<B>.None;

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    [Pure]
    public Option<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        IsSome
            ? Option.Some(project(Value!, bind(Value!).Value))
            : Option<C>.None;

    [Pure]
    public static implicit operator Option<A>(Pure<A> mr) =>
        mr.Value is null ? None : Option.Some(mr.Value);

    /// <summary>
    /// Semigroup combine
    /// </summary>
    /// <param name="rhs">Alternative to return if this is None</param>
    /// <returns>This if in a Some state, `rhs` otherwise</returns>
    [Pure]
    public Option<A> Combine(Option<A> rhs) => 
        IsSome ? this : rhs;

    /// <summary>
    /// Monoid empty (aka None)
    /// </summary>
    [Pure]
    public static Option<A> Empty =>
        None;
}
