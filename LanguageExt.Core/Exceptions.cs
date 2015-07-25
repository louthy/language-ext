using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class SomeNotInitialisedException : Exception
    {
        public SomeNotInitialisedException(Type type)
            : 
            base("Unitialised Some<"+type.Name+">.")
        {
        }
    }

    public class ValueIsNoneException : Exception
    {
        public ValueIsNoneException()
            : base("Value is none.")
        {
        }

        public ValueIsNoneException(string message) : base(message)
        {
        }

        public ValueIsNoneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ValueIsNullException : Exception
    {
        public ValueIsNullException()
            : base("Value is null.")
        {
        }

        public ValueIsNullException(string message) : base(message)
        {
        }

        public ValueIsNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class ResultIsNullException : Exception
    {
        public ResultIsNullException()
            : base("Result is null.  Not allowed with Option<T> or Either<R,L>.")
        {
        }

        public ResultIsNullException(string message) : base(message)
        {
        }

        public ResultIsNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class OptionIsNoneException : Exception
    {
        public OptionIsNoneException()
            : base("Option isn't set.")
        {
        }

        public OptionIsNoneException(string message) : base(message)
        {
        }

        public OptionIsNoneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }



    [Serializable]
    public class EitherIsNotRightException : Exception
    {
        public EitherIsNotRightException()
            : base("Either is not right.")
        {
        }

        public EitherIsNotRightException(string message) : base(message)
        {
        }

        public EitherIsNotRightException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class EitherIsNotLeftException : Exception
    {
        public EitherIsNotLeftException()
            : base("Either is not left.")
        {
        }

        public EitherIsNotLeftException(string message) : base(message)
        {
        }

        public EitherIsNotLeftException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class EitherNotInitialisedException : Exception
    {
        public EitherNotInitialisedException()
            : base("Unitialised Either<T> in class member declaration.")
        {
        }

        public EitherNotInitialisedException(string message) : base(message)
        {
        }

        public EitherNotInitialisedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class NamedProcessAlreadyExistsException : Exception
    {
        public NamedProcessAlreadyExistsException()
            :
            base("Named process already exists")
        {
        }

        public NamedProcessAlreadyExistsException(string message) : base(message)
        {
        }

        public NamedProcessAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NamedProcessAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidProcessNameException : Exception
    {
        public InvalidProcessNameException()
            :
            base("Invalid process name")
        {
        }

        public InvalidProcessNameException(string message) : base(message)
        {
        }

        public InvalidProcessNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidProcessNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidProcessIdException : Exception
    {
        public InvalidProcessIdException()
            :
            base("Invalid process ID")
        {
        }

        public InvalidProcessIdException(string message) : base(message)
        {
        }

        public InvalidProcessIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidProcessIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class SystemKillActorException : Exception
    {
        public SystemKillActorException()
            :
            base("SYS:Poison pill")
        {
        }

        public SystemKillActorException(string message) : base(message)
        {
        }

        public SystemKillActorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SystemKillActorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
