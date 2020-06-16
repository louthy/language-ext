using System;
using System.Net;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;

namespace LanguageExt.Common
{
    [Serializable]
    public struct Error : ISerializable, IEquatable<Error>
    {
        public readonly static Error Bottom = new Error(666, "Bottom", None);

        int code;
        string message;
        Option<Exception> exception;

        Error(int code, string message, Option<Exception> exception)
        {
            this.code = code;
            this.message = message ?? throw new ArgumentNullException(nameof(message));
            this.exception = exception;
        }
        
        Error(SerializationInfo info, StreamingContext context)
        {
            code = (int)info.GetValue("Code", typeof(int));
            message = (string)info.GetValue("Message", typeof(string));
            exception = None;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Code", Code);
            info.AddValue("Message", Message);
        }

        public int Code
        {
            get => message == null ? 666 : code;
            private set => code = value;
        }

        public string Message
        {
            get => message ?? "Bottom";
            private set => message = value;
        }

        public Option<Exception> Exception
        {
            get => exception;
            private set => exception = value;
        }

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
            Message;

        public static implicit operator Error(Exception e) =>
            New(e);

        public static implicit operator Exception(Error e) =>
            e.ToException();

        public bool Equals(Error other) =>
            message == other.message;

        public override bool Equals(object obj) =>
            obj is Error other && Equals(other);

        public static bool operator ==(Error lhs, Error rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Error lhs, Error rhs) =>
            !(lhs == rhs);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = code;
                hashCode = (hashCode * 397) ^ (message != null ? message.GetHashCode() : 0);
                return hashCode;
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
