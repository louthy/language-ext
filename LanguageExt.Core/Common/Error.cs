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
    /// Error value
    /// </summary>
    /// <remarks>
    /// Unlike exceptions, Error can be either exceptional or non-exceptional, i.e. it is either created from an
    /// exception or it isn't.  This allows for expected errors to be represented without throwing exceptions.  
    /// </remarks>
    [Serializable]
    public readonly struct Error : ISerializable, IEquatable<Error>
    {
        readonly int code;
        readonly string message;
        internal readonly Exception exception;
        internal readonly ErrorException inner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Error(int code, string message, Exception exception, ErrorException inner)
        {
            this.code      = code;
            this.message   = message ?? throw new ArgumentNullException(nameof(message));
            this.exception = exception;
            this.inner     = inner;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Error(SerializationInfo info, StreamingContext context)
        {
            code      = (int)info.GetValue("Code", typeof(int));
            message   = (string)info.GetValue("Message", typeof(string));
            exception = null;
            inner     = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
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
        public string Message
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
                : (Error)inner;
            
        /// <summary>
        /// If this error represents an exceptional error, then this will return that exception, otherwise it will
        /// generate a new ErrorException that contains the code, message, and inner of this Error.
        /// </summary>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Exception ToException() =>
            exception ?? new ErrorException(Code, Message, null, inner);

        /// <summary>
        /// If this error represents an exceptional error, then this will return that exception, otherwise None
        /// </summary>
        [Pure]
        public Option<Exception> Exception =>
            Optional(exception);

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
        public static Error New(int code, string message, Exception thisException) => 
            new Error(code, message, thisException, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(Exception thisException) =>
            new Error(thisException.HResult, thisException.Message, thisException, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="thisException">The exception this error represents</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(string message, Exception thisException) =>
            new Error(thisException.HResult, message, thisException, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(string message) =>
            new Error(0, message, null, null);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(int code, string message) =>
            new Error(code, message, null, null);
        
        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="thisException">The exception this error represents</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(int code, string message, Error inner) => 
            new Error(code, message, null, (ErrorException)inner);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(Exception thisException, Error inner) =>
            new Error(thisException.HResult, thisException.Message, thisException, (ErrorException)inner);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="thisException">The exception this error represents</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(string message, Exception thisException, Error inner) =>
            new Error(thisException.HResult, message, thisException, (ErrorException)inner);

        /// <summary>
        /// Create a new error 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="inner">The inner error to this error</param>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(string message, Error inner) =>
            new Error(0, message, null, (ErrorException)inner);

        /// <summary>
        /// Attempt to recover an error from an object.
        /// Will accept Error, ErrorException, Exception, string, Option<Error>
        /// If it fails, Errors.Bottom is returned
        /// </summary>
        [Pure]
        public static Error FromObject(object value) =>
            value switch
            {
                Error err          => err,
                ErrorException ex  => ex.ToError(),
                Exception ex       => New(ex),
                string str         => New(str),
                Option<Error> oerr => oerr.IfNone(Errors.Bottom),
                _                  => Errors.Bottom
            };

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() =>
            Message;

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Error(string e) =>
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

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Error other) =>
            Code == 0
                ? Message == other.Message
                : Code == other.Code;

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) =>
            obj is Error other && Equals(other);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Error lhs, Error rhs) =>
            lhs.Equals(rhs);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Error lhs, Error rhs) =>
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
        
        [Pure]
        internal static Option<FAIL> Convert<FAIL>(object err) => err switch
        {
            // Messy, but we're doing our best to recover an error rather than return Bottom
                
            FAIL fail                                                   => fail,
            Exception e     when typeof(FAIL) == typeof(Common.Error)   => (FAIL)(object)Common.Error.New(e),
            Exception e     when typeof(FAIL) == typeof(string)         => (FAIL)(object)e.Message,
            Common.Error e  when typeof(FAIL) == typeof(Exception)      => (FAIL)(object)e.ToException(),
            Common.Error e  when typeof(FAIL) == typeof(string)         => (FAIL)(object)e.ToString(),
            string e        when typeof(FAIL) == typeof(Exception)      => (FAIL)(object)new Exception(e),
            string e        when typeof(FAIL) == typeof(Common.Error)   => (FAIL)(object)Common.Error.New(e),
            _ => None
        };

        /// <summary>
        /// Throw the error as an exception
        /// </summary>
        public Unit Throw() =>
            ToException().Rethrow();
    }
}
