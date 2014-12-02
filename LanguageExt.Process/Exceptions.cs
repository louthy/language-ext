using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
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
            base("Invalid process path")
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

    [Serializable]
    public class TimeoutException : Exception
    {
        public TimeoutException()
            :
            base("The mailbox operation timed out.")
        {
        }

        public TimeoutException(string message) : base(message)
        {
        }

        public TimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
