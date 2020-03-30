using System;
using System.Runtime.ExceptionServices;
using static LanguageExt.Prelude;

namespace LanguageExt.Common
{
    public struct Error : IEquatable<Error>
    {
        public readonly static Error Bottom = new Error(666, "Bottom", None);

        readonly int code;
        readonly string message;
        public readonly Option<Exception> Exception;

        Error(int code, string message, Option<Exception> exception)
        {
            this.code = code;
            this.message = message ?? throw new ArgumentNullException(nameof(message));
            Exception = exception;
        }

        public int Code =>
            message == null
                ? 666
                : code;

        public string Message =>
            message ?? "Bottom";

        public static Error New(int code, string message, Option<Exception> exception) => 
            new Error(code, message, exception);

        public static Error New(Exception exception) =>
            new Error(exception.HResult, exception.Message, exception);

        public static Error New(string message, Exception exception) =>
            new Error(exception.HResult, message, exception);

        public static Error New(string message) =>
            new Error(0, message, None);

        public static Error New(int code, string message) =>
            new Error(code, message, None);

        public static Error FromObject(object value) =>
            value is Error err          ? err
          : value is Exception ex       ? New(ex)
          : value is string str         ? New(str)
          : value is Option<Error> oerr ? oerr.IfNone(Bottom)
          : Bottom;

        public Exception ToException() =>
            Exception.IsSome
                ? (Exception)Exception
                : Message == null
                    ? new BottomException()
                    : new Exception(Message);

        /// <summary>
        /// Throw as an exception
        /// </summary>
        public Unit Throw()
        {
            ExceptionDispatchInfo.Capture(ToException()).Throw();
            return default;
        }

        public override string ToString() =>
            message;

        public string StackTrace =>
            Exception.IsSome
                ? ((Exception)Exception).StackTrace
                : "";
        
        public static implicit operator Error(Exception e) =>
            New(e);

        public static implicit operator Exception(Error e) =>
            e.ToException();

        public static bool operator ==(Error x, Error y) =>
            x.Equals(y);

        public static bool operator !=(Error x, Error y) =>
            !(x == y);
        
        public bool Equals(Error other) => 
            message == other.message;

        public override bool Equals(object obj) => obj is Error other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (code * 397) ^ (message != null ? message.GetHashCode() : 0);
            }
        }
        
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
}
