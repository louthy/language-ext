using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class SomeNotInitialisedException : Exception
    {
        public SomeNotInitialisedException(Type type)
            : 
            base("Unitialised Some<"+type.Name+"> in class member declaration.")
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
}
