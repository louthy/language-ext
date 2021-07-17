using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;

namespace LanguageExt.Common
{
    public static class Errors
    {
        public const string CancelledText = "cancelled";
        public const int CancelledCode = -2000000000;
        public static readonly Error Cancelled = (CancelledCode, CancelledText);

        public const string BottomText = "bottom";
        public const int BottomCode = -2000000001;
        public readonly static Error Bottom = (BottomCode, BottomText);

        public const string TimedOutText = "timed out";
        public const int TimedOutCode = -2000000002;
        public static readonly Error TimedOut = (TimedOutCode, TimedOutText);    
    }

    [Serializable]
    public readonly struct Error : ISerializable, IEquatable<Error>
    {
        readonly int code;
        readonly string message;
        readonly Option<Exception> exception;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Error(int code, string message, Option<Exception> exception)
        {
            this.code = code;
            this.message = message ?? throw new ArgumentNullException(nameof(message));
            this.exception = exception;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Error(SerializationInfo info, StreamingContext context)
        {
            code = (int)info.GetValue("Code", typeof(int));
            message = (string)info.GetValue("Message", typeof(string));
            exception = None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Code", Code);
            info.AddValue("Message", Message);
        }

        [Pure]
        public int Code
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => message == null ? Errors.BottomCode : code;
        }

        [Pure]
        public string Message
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => message ?? Errors.BottomText;
        }

        public Option<Exception> Exception =>
            exception;

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(int code, string message, Option<Exception> exception) => 
            new Error(code, message, exception);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(Exception exception) =>
            new Error(exception.HResult, exception.Message, exception);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(string message, Exception exception) =>
            new Error(exception.HResult, message, exception);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(string message) =>
            new Error(0, message, None);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Error New(int code, string message) =>
            new Error(code, message, None);

        [Pure]
        public static Error FromObject(object value) =>
            value switch
            {
                Error err          => err,
                Exception ex       => New(ex),
                string str         => New(str),
                Option<Error> oerr => oerr.IfNone(Errors.Bottom),
                _                  => Errors.Bottom
            };

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Exception ToException() =>
            Exception.IsSome
                ? (Exception)Exception
                : new ErrorException(Code, Message);

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
    }

    [Serializable]
    public class ErrorException : Exception
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ErrorException(int code, string message) : base(message) =>
            HResult = code;
    }
}
