using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;

namespace LanguageExt.Common
{
    /// <summary>
    /// Error exception
    /// This works in tandem with the Error struct to allow for nested errors that easily convert between each other
    /// </summary>
    /// <remarks>
    /// Unlike regular exceptions, ErrorException can be either exceptional or non-exceptional, i.e. it is either
    /// created from an exception or it isn't.  This allows for expected errors to be represented without throwing
    /// exceptions.  
    /// </remarks>
    [Serializable]
    public class ErrorException : Exception, ISerializable, IEquatable<Error>, IEquatable<ErrorException>
    {
        readonly int code;
        readonly string message;
        readonly Exception exception;
        readonly ErrorException inner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ErrorException(int code, string message, Exception exception, ErrorException inner)
        {
            this.code      = code;
            this.message   = message ?? throw new ArgumentNullException(nameof(message));
            this.exception = exception;
            this.inner     = inner;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ErrorException(int code, string message)
        {
            this.code      = code;
            this.message   = message ?? throw new ArgumentNullException(nameof(message));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ErrorException(SerializationInfo info, StreamingContext context)
        {
            code = (int)info.GetValue("Code", typeof(int));
            message = (string)info.GetValue("Message", typeof(string));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Code", Code);
            info.AddValue("Message", Message);
        }

        /// <summary>
        /// Error code
        /// </summary>
        [Pure]
        public int Code
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => message == null ? Errors.BottomCode : code;
        }

        /// <summary>
        /// Error message
        /// </summary>
        [Pure]
        public override string Message
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => message ?? Errors.BottomText;
        }

        /// <summary>
        /// Inner error
        /// </summary>
        [Pure]
        public Option<Error> Inner =>
            inner is null 
                ? None 
                : new Error(inner.Code, inner.Message, inner.exception, inner.inner);

        /// <summary>
        /// Convert this ErrorException into an Error
        /// </summary>
        [Pure]
        public Error ToError() =>
            new Error(Code, Message, exception, inner);

        /// <summary>
        /// If this error represents an exceptional error, then this will return that exception, otherwise None
        /// </summary>
        [Pure]
        public Option<Exception> Exception =>
            exception;

        /// <summary>
        /// Convert this error to an exception.  If the error represents an exceptional error, then that will be returned.
        /// Otherwise a new Exception will be generated that wraps up the code, message, and inner of this object
        /// </summary>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Exception ToException() =>
            exception ?? new ErrorException(code, message, null, inner);

        /// <summary>
        /// If this error represents an exceptional error, then this will return true if the exceptional error is of type E
        /// </summary>
        [Pure]
        public bool Is<E>() where E : Exception =>
            exception is E; 
        
        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="thisException">The exception this error represents</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(int code, string message, Exception thisException) => 
            new ErrorException(code, message, thisException, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="thisException">The exception this error represents</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(Exception thisException) =>
            new ErrorException(thisException.HResult, thisException.Message, thisException, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="thisException">The exception this error represents</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(string message, Exception thisException) =>
            new ErrorException(thisException.HResult, message, thisException, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(string message) =>
            new ErrorException(0, message, null, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(int code, string message) =>
            new ErrorException(code, message, null, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="thisException">The exception this error represents</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(int code, string message, Exception thisException, Error inner) => 
            new ErrorException(code, message, thisException, inner);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(Exception thisException, Error inner) =>
            new ErrorException(thisException.HResult, thisException.Message, thisException, inner);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="thisException">The exception this error represents</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(string message, Exception thisException, Error inner) =>
            new ErrorException(thisException.HResult, message, thisException, inner);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(string message, Error inner) =>
            new ErrorException(0, message, null, inner);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ErrorException New(int code, string message, Error inner) =>
            new ErrorException(code, message, null, inner);

        /// <summary>
        /// Attempt to recover an error from an object.
        /// Will accept Error, ErrorException, Exception, string
        /// If it fails, Errors.Bottom is returned
        /// </summary>
        [Pure]
        public static ErrorException FromObject(object value) =>
            value switch
            {
                ErrorException ex => ex,
                Error err         => new ErrorException(err.Code, err.Message, err.exception, err.inner),
                Exception ex      => New(ex),
                string str        => New(str),
                _                 => new ErrorException(Errors.BottomCode, Errors.BottomText)
            };

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() =>
            Message;

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ErrorException(string e) =>
            New(e);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ErrorException((int Code, string Message) e) =>
            New(e.Code, e.Message);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ErrorException(Error e) =>
            New(e.Code, e.Message, e.exception, e.inner);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Error (ErrorException e) =>
            new Error(e.Code, e.Message, e.exception, e.inner);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ErrorException other) =>
            other is not null &&
            (Code == 0
                 ? Message == other.Message
                 : Code == other.Code);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Error other) =>
            Code == 0
                ? Message == other.Message
                : Code == other.Code;

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) =>
            (obj is ErrorException other1 && Equals(other1)) ||
            (obj is Error other2 && Equals(other2));

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ErrorException lhs, ErrorException rhs) =>
            lhs?.Equals(rhs) ?? false;

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ErrorException lhs, ErrorException rhs) =>
            !(lhs == rhs);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ErrorException lhs, Error rhs) =>
            lhs?.Equals(rhs) ?? false;

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ErrorException lhs, Error rhs) =>
            !(lhs == rhs);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Error lhs, ErrorException rhs) =>
            lhs.Equals(rhs);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Error lhs, ErrorException rhs) =>
            !(lhs == rhs);
        
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return Code == 0
                           ? Message.GetHashCode()
                           : Code;
            }
        }
    }
}
