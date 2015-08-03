using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Some T not initialised
    /// </summary>
    public class SomeNotInitialisedException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SomeNotInitialisedException(Type type)
            : 
            base("Unitialised Some<"+type.Name+">.")
        {
        }
    }

    /// <summary>
    /// Value is none
    /// </summary>
    public class ValueIsNoneException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException()
            : base("Value is none.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Value is null
    /// </summary>
    public class ValueIsNullException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNullException()
            : base("Value is null.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNullException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Result is null
    /// </summary>
    [Serializable]
    public class ResultIsNullException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException()
            : base("Result is null.  Not allowed with Option<T> or Either<R,L>.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Option T is none
    /// </summary>
    [Serializable]
    public class OptionIsNoneException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public OptionIsNoneException()
            : base("Option isn't set.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public OptionIsNoneException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public OptionIsNoneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Either is not right
    /// </summary>
    [Serializable]
    public class EitherIsNotRightException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public EitherIsNotRightException()
            : base("Either is not right.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public EitherIsNotRightException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public EitherIsNotRightException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Either is not left
    /// </summary>
    [Serializable]
    public class EitherIsNotLeftException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public EitherIsNotLeftException()
            : base("Either is not left.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public EitherIsNotLeftException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public EitherIsNotLeftException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Named process already exists
    /// </summary>
    [Serializable]
    public class NamedProcessAlreadyExistsException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public NamedProcessAlreadyExistsException()
            :
            base("Named process already exists")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public NamedProcessAlreadyExistsException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public NamedProcessAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        protected NamedProcessAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Invalid process name
    /// </summary>
    [Serializable]
    public class InvalidProcessNameException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public InvalidProcessNameException()
            :
            base("Invalid process name")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public InvalidProcessNameException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public InvalidProcessNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        protected InvalidProcessNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Invalid process ID
    /// </summary>
    [Serializable]
    public class InvalidProcessIdException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public InvalidProcessIdException()
            :
            base("Invalid process ID")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public InvalidProcessIdException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public InvalidProcessIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        protected InvalidProcessIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Kill process
    /// </summary>
    [Serializable]
    public class SystemKillActorException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SystemKillActorException()
            :
            base("SYS:Poison pill")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public SystemKillActorException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public SystemKillActorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        protected SystemKillActorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Value is bottom
    /// </summary>
    [Serializable]
    public class BottomException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public BottomException(string type = "Value")
            :
            base(type + " is in a bottom state and therefore not valid.  This can happen when the value was filterd and the predicate "+
                 "returned false and there was no valid state the value could be in.  If you are going to use the type in a filter "+
                 "you should check if the IsBottom flag is set before use.  This can also happen if the struct wasn't initialised properly and then used.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public BottomException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        protected BottomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
