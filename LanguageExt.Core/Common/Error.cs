using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Common;

/// <summary>
/// Abstract error value
/// </summary>
[DataContract]
public abstract record Error : Monoid<Error>
{
    /// <summary>
    /// Error code
    /// </summary>
    [Pure]
    [DataMember]
    public virtual int Code =>
        0;

    /// <summary>
    /// Error message
    /// </summary>
    [Pure]
    [DataMember]
    public abstract string Message { get; }

    /// <summary>
    /// Inner error
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public virtual Option<Error> Inner =>
        None;

    /// <summary>
    /// If this error represents an exceptional error, then this will return true if the exceptional error is of type E
    /// </summary>
    [Pure]
    public virtual bool HasException<E>() where E : Exception =>
        false;

    /// <summary>
    /// Return true if an `Error` with `Code` equalling `code` is contained within 
    /// </summary>
    [Pure]
    public virtual bool HasCode(int code) =>
        Code == code;
    
    /// <summary>
    /// Return true if this error contains or *is* the `error` provided
    /// </summary>
    [Pure]
    public virtual bool IsType<E>() where E : Error =>
        this is E;

    /// <summary>
    /// Return true if this error contains or *is* the `error` provided
    /// </summary>
    [Pure]
    public virtual bool Is(Error error) =>
        error is ManyErrors errors
            ? errors.Errors.Exists(Is) 
            : Code == 0
                ? Message == error.Message
                : Code == error.Code;

    /// <summary>
    /// Filter the error(s) so that only errors of type `E` are left
    /// </summary>
    /// <remarks>
    /// If no errors match, then returns `Errors.None`.
    /// </remarks>
    [Pure]
    public virtual Error Filter<E>() where E : Error =>
        this is E ? this : Errors.None;
    
    /// <summary>
    /// For each error in this structure, invoke the `f` function and
    /// monad bind it with the subsequent value.
    ///
    /// If any `f` invocation yields a failure, then subsequent error will
    /// not be processed.
    /// monad
    /// </summary>
    /// <remarks>
    /// The final `A` is returned.
    /// </remarks>
    /// <param name="f">Function</param>
    /// <returns>The final `A` is returned.</returns>
    [Pure]
    public virtual K<M, A> ForAllM<M, A>(Func<Error, K<M, A>> f) 
        where M : Monad<M>, Fallible<Error, M> =>
        f(this);
    
    /// <summary>
    /// For each error in this structure, invoke the `f` function and
    /// monoid combine it with the subsequent value.
    ///
    /// If any `f` invocation yields a failure, then subsequent error
    /// values may be processed.  This is dependent on the `M` monoid. 
    /// </summary>
    /// <remarks>
    /// The aggregated `K<M, A>` is returned.
    /// </remarks>
    /// <param name="f">Function</param>
    /// <returns>The aggregated `K<M, A>` is returned.</returns>
    [Pure]
    public virtual K<M, A> FoldM<M, A>(Func<Error, K<M, A>> f) 
        where M : MonoidK<M> =>
        f(this);

    /// <summary>
    /// True if the error is exceptional
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public abstract bool IsExceptional { get; }

    /// <summary>
    /// True if the error is expected
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public abstract bool IsExpected { get; }

    /// <summary>
    /// Get the first error (this will be `Errors.None` if there are zero errors)
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public virtual Error Head =>
        this;

    /// <summary>
    /// Get the errors with the head removed (this may be `Errors.None` if there are zero errors in the tail)
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public virtual Error Tail =>
        Errors.None;

    /// <summary>
    /// This type can contain zero or more errors.
    ///
    /// If `IsEmpty` is `true` then this is like `None` in `Option`: still an error, but without any specific
    /// information about the error.
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public virtual bool IsEmpty =>
        false;

    /// <summary>
    /// This type can contain zero or more errors.  This property returns the number of information carrying errors.
    ///
    /// If `Count` is `0` then this is like `None` in `Option`: still an error, but without any specific information
    /// about the error.
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public virtual int Count =>
        1;

    /// <summary>
    /// If this error represents an exceptional error, then this will return that exception, otherwise it will
    /// generate a new ErrorException that contains the code, message, and inner of this Error.
    /// </summary>
    [Pure]
    public virtual Exception ToException() =>
        ToErrorException();

    /// <summary>
    /// If this error represents an exceptional error, then this will return that exception, otherwise `None`
    /// </summary>
    [Pure]
    public Option<Exception> Exception =>
        IsExceptional
            ? Some(ToException())
            : None;

    /// <summary>
    /// Convert to an `ErrorException` which is like `Error` but derived from the `Exception` hierarchy
    /// </summary>
    [Pure]
    public abstract ErrorException ToErrorException();
    
    /// <summary>
    /// Append an error to this error
    /// </summary>
    /// <remarks>Single errors will be converted to `ManyErrors`;  `ManyErrors` will have their collection updated</remarks>
    /// <param name="error">Error</param>
    /// <returns></returns>
    [Pure]
    public Error Combine(Error error) =>
        (this, error) switch
        {
            ({IsEmpty: true}, var e)       => e,
            (var e, {IsEmpty: true})       => e,
            (ManyErrors e1, ManyErrors e2) => new ManyErrors(e1.Errors + e2.Errors), 
            (ManyErrors e1, var e2)        => new ManyErrors(e1.Errors.Add(e2)), 
            (var e1,        ManyErrors e2) => new ManyErrors(e1.Cons(e2.Errors)), 
            (var e1,        var e2)        => new ManyErrors(Seq(e1, e2)) 
        };

    [Pure]
    public static Error Empty =>
        ManyErrors.Empty;
    
    /// <summary>
    /// Append an error to this error
    /// </summary>
    /// <remarks>Single errors will be converted to `ManyErrors`;  `ManyErrors` will have their collection updated</remarks>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error operator+(Error lhs, Error rhs) =>
        lhs.Combine(rhs);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual Iterable<Error> AsIterable()
    {
        return Iterable.createRange(go());
        IEnumerable<Error> go() 
        {
            yield return this;
        }
    }

    /// <summary>
    /// Convert the error to a string
    /// </summary>
    [Pure]
    public override string ToString() => 
        Message;

    /// <summary>
    /// Create an `Exceptional` error 
    /// </summary>
    /// <param name="thisException">Exception</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error New(Exception? thisException) =>
        thisException switch
        {
            null                       => Errors.None,
            ErrorException e           => e.ToError(),
            TaskCanceledException      => Errors.Cancelled,
            OperationCanceledException => Errors.Cancelled,
            TimeoutException           => Errors.TimedOut,
            AggregateException a       => ManyErrors.FromAggregate(a),
            var e                      => new Exceptional(e)
        };
        

    /// <summary>
    /// Create a `Exceptional` error with an overriden message.  This can be useful for sanitising the display message
    /// when internally we're carrying the exception. 
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="thisException">Exception</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error New(string message, Exception? thisException) =>
        new Exceptional(message, thisException ?? new BottomException());

    /// <summary>
    /// Create an `Expected` error 
    /// </summary>
    /// <param name="message">Error message</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error New(string message) =>
        new Expected(message, 0, None);

    /// <summary>
    /// Create an `Expected` error 
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error New(int code, string message) =>
        new Expected(message, code, None);
    
    /// <summary>
    /// Create an `Expected` error 
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    /// <param name="inner">The inner error to this error</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error New(int code, string message, Error inner) =>
        new Expected(message, code, inner);

    /// <summary>
    /// Create an `Expected` error 
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="inner">The inner error to this error</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error New(string message, Error inner) =>
        new Expected(message, 0, inner);

    /// <summary>
    /// Create a `ManyErrors` error 
    /// </summary>
    /// <remarks>Collects many errors into a single `Error` type, called `ManyErrors`</remarks>
    /// <param name="code">Error code</param>
    /// <param name="inner">The inner error to this error</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error Many(params Error[] errors) =>
        errors.Length == 0
            ? Errors.None
            : errors.Length == 1
                ? errors[0]
                : new ManyErrors(errors.AsIterable().ToSeq());

    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="inner">The inner error to this error</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error Many(Seq<Error> errors) =>
        errors.IsEmpty
            ? Errors.None
            : errors.Tail.IsEmpty
                ? (Error)errors.Head
                : new ManyErrors(errors);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Error(string e) =>
        New(e);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Error((int Code, string Message) e) =>
        New(e.Code, e.Message);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Error(Exception e) =>
        New(e);

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Exception(Error e) =>
        e.ToException();
    
    /// <summary>
    /// Attempt to recover an error from an object.
    /// Will accept Error, ErrorException, Exception, string, Option<Error>
    /// If it fails, Errors.Bottom is returned
    /// </summary>
    [Pure]
    public static Error FromObject(object? value) =>
        value switch
        {
            Error err          => err,
            ErrorException ex  => ex.ToError(),
            Exception ex       => New(ex),
            string str         => New(str),
            Option<Error> oerr => oerr.IfNone(Errors.Bottom),
            _                  => Errors.Bottom
        };
    
    [Pure]
    internal static Option<FAIL> Convert<FAIL>(object? err) => 
        err switch
    {
        // Messy, but we're doing our best to recover an error rather than return Bottom
            
        FAIL fail                                           => fail,
        Exception e  when typeof(FAIL) == typeof(Error)     => (FAIL)(object)New(e),
        Exception e  when typeof(FAIL) == typeof(string)    => (FAIL)(object)e.Message,
        Error e      when typeof(FAIL) == typeof(Exception) => (FAIL)(object)e.ToException(),
        Error e      when typeof(FAIL) == typeof(string)    => (FAIL)(object)e.ToString(),
        string e     when typeof(FAIL) == typeof(Exception) => (FAIL)(object)new Exception(e),
        string e     when typeof(FAIL) == typeof(Error)     => (FAIL)(object)New(e),
        _ => None
    };

    /// <summary>
    /// Throw the error as an exception
    /// </summary>
    public Unit Throw()
    {
        ExceptionDispatchInfo.Capture(ToException()).Throw();
        return default;
    }

    /// <summary>
    /// Throw the error as an exception
    /// </summary>
    public R Throw<R>()
    {
        ExceptionDispatchInfo.Capture(ToException()).Throw();
        return default;
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append("Message = ");
        builder.Append(Message);
        if (Code != 0)
        {
            builder.Append(", Code = ");
            builder.Append(Code);
        }
        return true;
    }
}

/// <summary>
/// Contains the following:
/// 
/// * `Code` - an integer value
/// * `Message` - display text
/// * `Option<Error>` - a nested inner error
/// 
/// It returns `false` when `IsExceptional` is called against it; and `true` when `IsExpected` is
/// called against it.
/// 
/// Equality is done via the `Code`, so any two `Expected` errors with the same `Code` will be considered
/// the same.  This is useful when using the `@catch` behaviours with the `Aff` and `Eff` monads.  If the
/// `Code` is `0` then equality is done by comparing `Message`.
/// 
/// > This allows for localised error messages where the message is ignored when matching/catching
/// </summary>
/// <param name="Message">Error message</param>
/// <param name="Code">Error code</param>
/// <param name="Inner">Optional inner error</param>
[DataContract]
public record Expected(string Message, int Code, Option<Error> Inner = default) : Error
{
    /// <summary>
    /// Error message
    /// </summary>
    [Pure]
    [DataMember] 
    public override string Message { get; } = 
        Message;

    /// <summary>
    /// Error code
    /// </summary>
    [Pure]
    [DataMember] 
    public override int Code { get; } = 
        Code;
    
    /// <summary>
    /// Inner error
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override Option<Error> Inner { get; } = 
        Inner;
    
    [Pure]
    public override string ToString() => 
        Message;
    
    /// <summary>
    /// Generates a new `ErrorException` that contains the `Code`, `Message`, and `Inner` of this `Error`.
    /// </summary>
    [Pure]
    public override ErrorException ToErrorException() => 
        new WrappedErrorExpectedException(this);

    /// <summary>
    /// Returns false because this type isn't exceptional
    /// </summary>
    [Pure]
    public override bool HasException<E>() =>
        false;

    /// <summary>
    /// True if the error is exceptional
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsExceptional =>
        false;

    /// <summary>
    /// True if the error is expected
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsExpected =>
        true;
}

/// <summary>
/// This contains an `Exception` is the classic sense.  This also returns `true` when `IsExceptional` is 
/// called against it; and `false` when `IsExpected` is called against it.  
/// </summary>
/// <remarks>
/// If this record is constructed via deserialisation, or the default constructor then the internal `Exception`
/// will be `null`.  This is intentional to stop exceptions leaking over application boundaries.  The type will
/// gracefully handle that, but all stack-trace information (and the like) will be erased.  It is still considered
/// an exceptional error, however.
/// </remarks>
[DataContract]
public record Exceptional(string Message, int Code) : Error
{
    /// <summary>
    /// Internal exception.  If this record is constructed via deserialisation, or the default constructor then this
    /// value will be `null`.  This is intentional to stop exceptions leaking over application boundaries. 
    /// </summary>
    [IgnoreDataMember]
    readonly Exception? Value;
    
    /// <summary>
    /// Construct from an exception
    /// </summary>
    /// <param name="value">Exception</param>
    internal Exceptional(Exception value) : this(value.Message, value.HResult) =>
        Value = value;

    /// <summary>
    /// Construct from an exception, but override the message
    /// </summary>
    /// <param name="message">Message to override with</param>
    /// <param name="value">Exception</param>
    internal Exceptional(string message, Exception value) : this(message, value.HResult) =>
        Value = value;

    [DataMember]
    public override string Message { get; } = Message;

    [DataMember]
    public override int Code { get; } = Code;

    public override string ToString() => 
        Message;

    /// <summary>
    /// Returns the inner exception as an `Error` (if one exists), `None` otherwise
    /// </summary>
    [Pure]
    public override Option<Error> Inner => 
        Value?.InnerException == null
            ? None
            : New(Value.InnerException);
    
    /// <summary>
    /// Gets the `Exception`
    /// </summary>
    /// <returns></returns>
    [Pure]
    public override Exception ToException() => 
        Value ?? new ExceptionalException(Message, Code);
    
    /// <summary>
    /// Gets the `ErrorException`
    /// </summary>
    /// <returns></returns>
    [Pure]
    public override ErrorException ToErrorException() =>
        Value == null
            ? new WrappedErrorExceptionalException(this)
            : new ExceptionalException(Value);

    /// <summary>
    /// Return true if the exceptional error is of type E
    /// </summary>
    [Pure]
    public override bool HasException<E>() =>
        Value is E;

    [Pure]
    public override bool Is(Error error) =>
        error is ManyErrors errors
            ? errors.Errors.Exists(Is) 
            : Value == null
                ? error.IsExceptional && Code == error.Code && Message == error.Message 
                : error.IsExceptional && Value.GetType().IsInstanceOfType(error.ToException());

    /// <summary>
    /// True if the error is exceptional
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsExceptional =>
        true;

    /// <summary>
    /// True if the error is expected
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsExpected =>
        false;
}

/// <summary>
/// Bottom error
/// </summary>
[DataContract]
public sealed record BottomError() : Exceptional(BottomException.Default)
{
    public static readonly Error Default = new BottomError();
    
    [DataMember]
    public override int Code => 
        Errors.BottomCode; 

    [DataMember]
    public override string Message => 
        Errors.BottomText;

    public override string ToString() => 
        Message;
    
    /// <summary>
    /// Gets the Exception
    /// </summary>
    /// <returns></returns>
    public override Exception ToException() => 
        BottomException.Default;
    
    /// <summary>
    /// Gets the `ErrorException`
    /// </summary>
    /// <returns></returns>
    public override ErrorException ToErrorException() => 
        BottomException.Default;

    /// <summary>
    /// Return true if the exceptional error is of type E
    /// </summary>
    [Pure]
    public override bool HasException<E>() =>
        BottomException.Default is E;

    /// <summary>
    /// Return true this error contains or *is* the `error` provided
    /// </summary>
    [Pure]
    public override bool Is(Error error) =>
        error is ManyErrors errors
            ? errors.Errors.Exists(Is) 
            : error is BottomError;

    /// <summary>
    /// True if the error is exceptional
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsExceptional =>
        true;

    /// <summary>
    /// True if the error is expected
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsExpected =>
        false;
}

/// <summary>
/// `ManyErrors` allows for zero or more errors to be collected.  This is useful for applicative behaviours
/// like validation.  It effectively turns the `Error` type into a monoid, with 'zero' being `Errors.None`,
/// and 'append' coming from the `Append` method or use of `operator+` 
/// </summary>
/// <param name="Errors">Errors</param>
[DataContract]
public sealed record ManyErrors([property: DataMember] Seq<Error> Errors) : Error
{
    public new static Error Empty { get; } =
        new ManyErrors(Seq.empty<Error>()); 

    public override int Code => 
        Common.Errors.ManyErrorsCode;

    public override string Message { get; } =
        Errors.ToFullArrayString();

    public override string ToString() => 
        Errors.ToFullArrayString();

    /// <summary>
    /// Gets the `Exception`
    /// </summary>
    public override Exception ToException() => 
        new AggregateException(Errors.Map(static e => e.ToException()));

    /// <summary>
    /// Gets the `ErrorException`
    /// </summary>
    public override ErrorException ToErrorException() =>
        new ManyExceptions(Errors.Map(static e => e.ToErrorException()));

    /// <summary>
    /// Return true if an exception of type E is contained within 
    /// </summary>
    [Pure]
    public override bool HasException<E>() =>
        Errors.Exists(static e => e.HasException<E>());

    /// <summary>
    /// Return true if an `Error` with `Code` equalling `code` is contained within 
    /// </summary>
    [Pure]
    public override bool HasCode(int code) =>
        Errors.Exists(e => e.Code == code);
    
    [Pure]
    public override bool IsType<E>() =>
        Errors.Exists(static e => e.IsType<E>());
    
    /// <summary>
    /// Return true this error contains or *is* the `error` provided
    /// </summary>
    [Pure]
    public override bool Is(Error error) =>
        Errors.Exists(e => e.Is(error));

    /// <summary>
    /// Filter the error(s) so that only errors of type `E` are left
    /// </summary>
    /// <remarks>
    /// If no errors match, then returns `Errors.None`.
    /// </remarks>
    [Pure]
    public override Error Filter<E>() =>
        Errors.Choose(e => e.IsType<E>() ? Some(e.Filter<E>()) : None) switch
        {
            []     => Common.Errors.None,
            var es => Many(es)
        };

    /// <summary>
    /// For each error in this structure, invoke the `f` function and
    /// monad bind it with the subsequent value.
    ///
    /// If any `f` invocation yields a failure, then subsequent error will
    /// not be processed.
    /// monad
    /// </summary>
    /// <remarks>
    /// The final `A` is returned.
    /// </remarks>
    /// <param name="f">Function</param>
    /// <returns>The final `A` is returned.</returns>
    [Pure]
    public override K<M, A> ForAllM<M, A>(Func<Error, K<M, A>> f)
    {
        if(Errors.IsEmpty) return M.Fail<A>(this);
        var result = Errors[0].ForAllM(f);
        foreach (var e in Errors.Tail)
        {
            result = result.Bind(_ => e.ForAllM(f));
        }
        return result;
    }
    
    /// <summary>
    /// For each error in this structure, invoke the `f` function and
    /// monoid combine it with the subsequent value.
    ///
    /// If any `f` invocation yields a failure, then subsequent error
    /// values may be processed.  This is dependent on the `M` monoid. 
    /// </summary>
    /// <remarks>
    /// The aggregated `K<M, A>` is returned.
    /// </remarks>
    /// <param name="f">Function</param>
    /// <returns>The aggregated `K<M, A>` is returned.</returns>
    [Pure]
    public override K<M, A> FoldM<M, A>(Func<Error, K<M, A>> f)
    {
        var result = M.Empty<A>();
        foreach (var e in Errors)
        {
            result = result.Combine(e.FoldM(f));
        }
        return result;
    }

    /// <summary>
    /// True if any errors are exceptional
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsExceptional =>
        Errors.Exists(static e => e.IsExceptional);

    /// <summary>
    /// True if all the errors are expected
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsExpected =>
        Errors.ForAll(static e => e.IsExpected);

    /// <summary>
    /// Get the first error (this may be `Errors.None` if there are zero errors)
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override Error Head =>
        Errors.IsEmpty
            ? Common.Errors.None
            : (Error)Errors.Head;

    /// <summary>
    /// Get the errors with the head removed (this may be `Errors.None` if there are zero errors in the tail)
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override Error Tail =>
        Errors.Tail.IsEmpty
            ? Common.Errors.None
            : this with {Errors = Errors.Tail};

    /// <summary>
    /// This type can contain zero or more errors.  If `IsEmpty` is `true` then this is like `None` in `Option`:  still
    /// an error, but without any specific information about the error.
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override bool IsEmpty =>
        Errors.IsEmpty;

    /// <summary>
    /// This type can contain zero or more errors.  This property returns the number of information carrying errors.
    ///
    /// If `Count` is `0` then this is like `None` in `Option`: still an error, but without any specific information
    /// about the error.
    /// </summary>
    [Pure]
    [IgnoreDataMember]
    public override int Count =>
        Errors.Count;

    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Iterable<Error> AsIterable() =>
        Errors.AsIterable();

    [Pure]
    internal static Error FromAggregate(AggregateException? e)
    {
        if (e is null) return Common.Errors.None;
        var errs = e.InnerExceptions.Bind(x => New(x).AsIterable()).AsIterable().ToSeq();
        if (errs.Count == 0) return Common.Errors.None;
        if (errs.Count == 1) return (Error)errs.Head;
        return Many(errs);
    }
} 
