using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Common
{
    public struct Error
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

        internal Exception ToException() =>
            Exception.IsSome
                ? (Exception)Exception
                : Message == null
                    ? new Exception("Bottom")
                    : new Exception(Message);

        public override string ToString() =>
            code == 0 
                ? message
                : $"{code}: {message}";

        public static implicit operator Error(Exception e) =>
            New(e);

        public static implicit operator Exception(Error e) =>
            e.ToException();
    }
}
