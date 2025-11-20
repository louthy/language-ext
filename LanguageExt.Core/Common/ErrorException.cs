using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Common;

/// <summary>
/// Error value
/// </summary>
/// <remarks>
/// This is a pair to the `Error` type, to allow `Error` to be converted to-and-from a classic `Exception`.
///
/// This allows code that can't handle the `Error` type to still throw something that keeps the fidelity of the `Error`
/// type, and can be converted directly to an `Error` in a `catch` block. 
/// </remarks>
/// <remarks>
/// Unlike exceptions, `Error` can be either:
/// 
/// * `ExceptionalException`    - representing an unexpected error
/// * `ExpectedException`       - representing an expected error
/// * `ManyExceptions`          - representing many errors
///
/// i.e. it is either created from an exception or it isn't.  This allows for expected errors to be represented
/// without throwing exceptions.  
/// </remarks>
public abstract class ErrorException : Exception, IEnumerable<ErrorException>, Monoid<ErrorException>
{
    protected ErrorException(int code) =>
        HResult = code;
    
    /// <summary>
    /// Error code
    /// </summary>
    [Pure]
    public abstract int Code { get; }

    /// <summary>
    /// Inner error
    /// </summary>
    [Pure]
    public abstract Option<ErrorException> Inner { get; }

    /// <summary>
    /// Convert to an `Error`
    /// </summary>
    [Pure]
    public abstract Error ToError();

    /// <summary>
    /// This type can contain zero or more errors.  If `IsEmpty` is `true` then this is like `None` in `Option`:  still
    /// an error, but without any specific information about the error.
    /// </summary>
    [Pure]
    public virtual bool IsEmpty =>
        false;

    /// <summary>
    /// True if the error is exceptional
    /// </summary>
    [Pure]
    public abstract bool IsExceptional { get; }

    /// <summary>
    /// True if the error is expected
    /// </summary>
    [Pure]
    public abstract bool IsExpected { get; }

    /// <summary>
    /// Append an error to this error
    /// </summary>
    /// <remarks>Single errors will be converted to `ManyErrors`;  `ManyErrors` will have their collection updated</remarks>
    [Pure]
    public abstract ErrorException Combine(ErrorException error);

    [Pure]
    public static ErrorException Empty => 
        ManyExceptions.Empty;
    
    /// <summary>
    /// Append an error to this error
    /// </summary>
    /// <remarks>Single errors will be converted to `ManyErrors`;  `ManyErrors` will have their collection updated</remarks>
    [Pure]
    public static ErrorException operator+(ErrorException lhs, ErrorException rhs) =>
        lhs.Combine(rhs);

    [Pure]
    public virtual IEnumerator<ErrorException> GetEnumerator()
    {
        yield return this;
    }

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Convert the error to a string
    /// </summary>
    [Pure]
    public override string ToString() => 
        Message;


    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="message">Error message</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ErrorException New(Exception thisException) =>
        new ExceptionalException(thisException);

    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="thisException">The exception this error represents</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ErrorException New(string message, Exception thisException) =>
        new ExceptionalException(message, thisException);

    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="message">Error message</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ErrorException New(string message) =>
        new ExpectedException(message, 0, None);

    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ErrorException New(int code, string message) =>
        new ExpectedException(message, code, None);
    
    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    /// <param name="inner">The inner error to this error</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ErrorException New(int code, string message, ErrorException inner) =>
        new ExpectedException(message, code, inner);

    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="inner">The inner error to this error</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ErrorException New(string message, ErrorException inner) =>
        new ExpectedException(message, 0, inner);

    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="inner">The inner error to this error</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ErrorException Many(params ErrorException[] errors) =>
        new ManyExceptions(errors.AsIterable().ToSeq());

    /// <summary>
    /// Create a new error 
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="inner">The inner error to this error</param>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ErrorException Many(Seq<ErrorException> errors) =>
        new ManyExceptions(errors);
}

/// <summary>
/// Represents expected errors
/// </summary>
public class ExpectedException(string message, int code, Option<ErrorException> inner = default) : ErrorException(code)
{
    /// <summary>
    /// Error code
    /// </summary>
    [Pure]
    public override int Code { get; } = code;

    /// <summary>
    /// Error message
    /// </summary>
    [Pure]
    public override string Message { get; } = message;

    /// <summary>
    /// Convert the error to a string
    /// </summary>
    [Pure]
    public override string ToString() => 
        Message;

    /// <summary>
    /// Inner error
    /// </summary>
    [Pure]
    public override Option<ErrorException> Inner { get; } = inner;

    /// <summary>
    /// Generates a new `Error` that contains the `Code`, `Message`, and `Inner` of this `ErrorException`.
    /// </summary>
    [Pure]
    public override Error ToError() => 
        new Expected(Message, Code, Inner.Map(static e => e.ToError()));

    /// <summary>
    /// True if the error is exceptional
    /// </summary>
    [Pure]
    public override bool IsExceptional =>
        false;

    /// <summary>
    /// True if the error is expected
    /// </summary>
    [Pure]
    public override bool IsExpected =>
        true;

    /// <summary>
    /// Append an error to this error
    /// </summary>
    /// <remarks>Single errors will be converted to `ManyErrors`;  `ManyErrors` will have their collection updated</remarks>
    [Pure]
    public override ErrorException Combine(ErrorException error) =>
        error is ManyExceptions m
            ? new ManyExceptions(error.Cons(m.Errors))
            : new ManyExceptions(Seq(this, error));
}

/// <summary>
/// Wraps an `Error` maintaining its type for subsequent conversion back to an `Error` later
/// </summary>
/// <param name="Error"></param>
public sealed class WrappedErrorExpectedException(Error Error) : 
    ExpectedException(Error.Message, Error.Code, Error.Inner.Map(e => e.ToErrorException()))
{
    /// <summary>
    /// Convert back to an `Error`
    /// </summary>
    /// <returns></returns>
    public override Error ToError() => 
        Error;
    
    /// <summary>
    /// Convert the error to a string
    /// </summary>
    [Pure]
    public override string ToString() => 
        Error.ToString();
}

/// <summary>
/// Represents an exceptional (unexpected) error
/// </summary>
/// <param name="Exception">Exceptional error</param>
public class ExceptionalException : ErrorException
{
    public ExceptionalException(Exception Exception) : base(Exception.HResult)
    {
        this.Exception = Exception;
        Code = Exception.HResult;
        Message = Exception.Message;
    }

    public ExceptionalException(string Message, int Code) : base(Code)
    {
        this.Code = Code;
        this.Message = Message;
    }

    public ExceptionalException(string Message, Exception Exception) : base(Exception.HResult)
    {
        Code = Exception.HResult;
        this.Message = Message;
    }

    public readonly Exception? Exception;
    public override int Code { get; }
    public override string Message { get; }
    
    /// <summary>
    /// Convert the error to a string
    /// </summary>
    [Pure]
    public override string ToString() => 
        Message;

    /// <summary>
    /// Returns the inner exception as an `Error` (if one exists), None otherwise
    /// </summary>
    [Pure]
    public override Option<ErrorException> Inner => 
        Exception?.InnerException == null
            ? None
            : New(Exception.InnerException);

    /// <summary>
    /// Gets the `Error`
    /// </summary>
    /// <returns></returns>
    public override Error ToError() =>
        Exception == null
            ? new Exceptional(Message, Code)
            : new Exceptional(Exception);

                /// <summary>
    /// True if the error is exceptional
    /// </summary>
    [Pure]
    public override bool IsExceptional =>
        true;

    /// <summary>
    /// True if the error is expected
    /// </summary>
    [Pure]
    public override bool IsExpected =>
        false;

    /// <summary>
    /// Append an error to this error
    /// </summary>
    /// <remarks>Single errors will be converted to `ManyErrors`;  `ManyErrors` will have their collection updated</remarks>
    /// <param name="error">Error</param>
    /// <returns></returns>
    [Pure]
    public override ErrorException Combine(ErrorException error) =>
        error is ManyExceptions m
            ? new ManyExceptions(error.Cons(m.Errors))
            : new ManyExceptions(Seq(this, error));
}

/// <summary>
/// Wraps an `Error` maintaining its type for subsequent conversion back to an `Error` later
/// </summary>
/// <param name="Error"></param>
public sealed class WrappedErrorExceptionalException(Error Error) : 
    ExceptionalException(Error.Message, Error.Code)
{
    /// <summary>
    /// Convert back to an `Error`
    /// </summary>
    /// <returns></returns>
    public override Error ToError() => 
        Error;
    
    /// <summary>
    /// Convert the error to a string
    /// </summary>
    [Pure]
    public override string ToString() => 
        Message;
}

/// <summary>
/// Represents multiple errors
/// </summary>
/// <param name="Errors">Errors</param>
public sealed class ManyExceptions(Seq<ErrorException> errors) : ErrorException(0)
{
    public new static ErrorException Empty { get; } = new ManyExceptions([]);

    public readonly Seq<ErrorException> Errors = errors;

    public override int Code => 
        Common.Errors.ManyErrorsCode;

    public override string Message =>
        Errors.ToFullArrayString();

    /// <summary>
    /// Returns the inner exception as an `Error` (if one exists), None otherwise
    /// </summary>
    [Pure]
    public override Option<ErrorException> Inner => 
        None;
    
    /// <summary>
    /// Gets the Exception
    /// </summary>
    public override Error ToError() => 
        new ManyErrors(Errors.Map(static e => e.ToError()));
    
    /// <summary>
    /// Convert the error to a string
    /// </summary>
    [Pure]
    public override string ToString() => 
        Errors.ToFullArrayString();

    /// <summary>
    /// This type can contain zero or more errors.  If `IsEmpty` is `true` then this is like `None` in `Option`:  still
    /// an error, but without any specific information about the error.
    /// </summary>
    [Pure]
    public override bool IsEmpty =>
        Errors.IsEmpty;

    /// <summary>
    /// True if any of the errors are exceptional
    /// </summary>
    [Pure]
    public override bool IsExceptional =>
        Errors.Exists(static e => e.IsExceptional);

    /// <summary>
    /// True if all the errors are expected
    /// </summary>
    [Pure]
    public override bool IsExpected =>
        Errors.ForAll(static e => e.IsExpected);

    /// <summary>
    /// Append an error to this error
    /// </summary>
    /// <remarks>Single errors will be converted to `ManyErrors`;  `ManyErrors` will have their collection updated</remarks>
    /// <param name="error">Error</param>
    /// <returns></returns>
    [Pure]
    public override ErrorException Combine(ErrorException error) =>
        error is ManyExceptions m
            ? new ManyExceptions(Errors + m.Errors)
            : new ManyExceptions(Seq(this, error));

    [Pure]
    public override IEnumerator<ErrorException> GetEnumerator() =>
        Errors.GetEnumerator();

}


/// <summary>
/// Value is bottom
/// </summary>
[Serializable]
public class BottomException() : 
    ExceptionalException(Errors.BottomText, Errors.BottomCode)
{
    public static readonly BottomException Default;

    static BottomException() => 
        Default = new();

    public override string Message =>
        Errors.BottomText;

    public override int Code =>
        Errors.BottomCode;
    
    public override Option<ErrorException> Inner =>
        default;
    
    public override Error ToError() => 
        BottomError.Default;
    
    public override ErrorException Combine(ErrorException error) => throw new NotImplementedException();
}

public static class ExceptionExtensions
{
    /// <summary>
    /// Throw the error as an exception
    /// </summary>
    public static Unit Rethrow(this Exception e)
    {
        ExceptionDispatchInfo.Capture(e).Throw();
        return default;
    }

    /// <summary>
    /// Throw the error as an exception
    /// </summary>
    public static R Rethrow<R>(this Exception e)
    {
        ExceptionDispatchInfo.Capture(e).Throw();
        return default;
    }
}
