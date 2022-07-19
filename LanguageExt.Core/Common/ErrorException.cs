#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt.Common;

/// <summary>
/// Error value
/// </summary>
/// <remarks>
/// Unlike exceptions, `Error` can be either:
/// 
/// * `Exceptional`         - representing an unexpected error
/// * `LabelledExceptional` - representing an unexpected error with additional context (a message)
/// * `Expected`            - representing an expected error
/// * `ManyErrors`          - representing an many errors
///
/// i.e. it is either created from an exception or it isn't.  This allows for expected errors to be represented
/// without throwing exceptions.  
/// </remarks>
public abstract class ErrorException : Exception, IEnumerable<ErrorException>
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
    public abstract ErrorException Append(ErrorException error);
    
    /// <summary>
    /// Append an error to this error
    /// </summary>
    /// <remarks>Single errors will be converted to `ManyErrors`;  `ManyErrors` will have their collection updated</remarks>
    [Pure]
    public static ErrorException operator+(ErrorException lhs, ErrorException rhs) =>
        lhs.Append(rhs);

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
        new ManyExceptions(errors.ToSeq());

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
public sealed class ExpectedException : ErrorException
{
    public ExpectedException(string message, int code, Option<ErrorException> inner) : base(code) =>
        (Message, Code, Inner) = (message, code, inner);
    
    /// <summary>
    /// Error code
    /// </summary>
    [Pure]
    public override int Code { get; } 
    
    /// <summary>
    /// Error message
    /// </summary>
    [Pure]
    public override string Message { get; }

    /// <summary>
    /// Inner error
    /// </summary>
    [Pure]
    public override Option<ErrorException> Inner { get; }
    
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
    public override ErrorException Append(ErrorException error) =>
        error is ManyExceptions m
            ? new ManyExceptions(error.Cons(m.Errors))
            : new ManyExceptions(Seq(this, error));
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
    public override ErrorException Append(ErrorException error) =>
        error is ManyExceptions m
            ? new ManyExceptions(error.Cons(m.Errors))
            : new ManyExceptions(Seq(this, error));
}

/// <summary>
/// Represents multiple errors
/// </summary>
/// <param name="Errors">Errors</param>
public sealed class ManyExceptions : ErrorException
{
    public ManyExceptions(Seq<ErrorException> errors) : base(0) =>
        Errors = errors;

    public readonly Seq<ErrorException> Errors;

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
    /// This type can contain zero or more errors.  If `IsEmpty` is `true` then this is like `None` in `Option`:  still
    /// an error, but without any specific information about the error.
    /// </summary>
    [Pure]
    public override bool IsEmpty =>
        Errors.IsEmpty;

    /// <summary>
    /// True if any of the the errors are exceptional
    /// </summary>
    [Pure]
    public override bool IsExceptional =>
        Errors.Exists(static e => e.IsExceptional);

    /// <summary>
    /// True if all of the the errors are expected
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
    public override ErrorException Append(ErrorException error) =>
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
public class BottomException : ExceptionalException
{
    public static readonly BottomException Default;

    static BottomException()
    {
        Default = new();
    }
    
    public BottomException() : base(Errors.BottomText, Errors.BottomCode)
    {
    }
    
    public override string Message =>
        Errors.BottomText;

    public override int Code =>
        Errors.BottomCode;
    
    public override Option<ErrorException> Inner =>
        default;
    
    public override Error ToError() => 
        BottomError.Default;
    
    public override ErrorException Append(ErrorException error) => throw new NotImplementedException();
}
