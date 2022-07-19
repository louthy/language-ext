using System;
using LanguageExt.Common;

namespace LanguageExt
{
    /// <summary>
    /// Some T not initialised
    /// </summary>
    [Serializable]
    public class SomeNotInitialisedException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SomeNotInitialisedException(Type type)
            :
            base($"Unitialised Some<{type.Name}>.")
        {
        }
    }

    /// <summary>
    /// Value is none
    /// </summary>
    [Serializable]
    public class ValueIsNoneException : Exception
    {
        public static readonly ValueIsNoneException Default = new ValueIsNoneException();

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
    [Serializable]
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
            : base("Result is null.")
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

    [Serializable]
    public class NotAppendableException : Exception
    {
        public NotAppendableException(Type t)
            : base($"Type '{t.Name}' not appendable: It's neither a CLR numeric-type, a string nor derived from IAppendable")
        {
        }
    }

    [Serializable]
    public class NotSubtractableException : Exception
    {
        public NotSubtractableException(Type t)
            : base($"Type '{t.Name}' not subtractable: It's neither a CLR numeric-type, nor derived from ISubtractable")
        {
        }
    }

    [Serializable]
    public class NotMultiplicableException : Exception
    {
        public NotMultiplicableException(Type t)
            : base($"Type '{t.Name}' not multiplicable: It's neither a CLR numeric-type, nor derived from IMultiplicable")
        {
        }
    }

    [Serializable]
    public class NotDivisibleException : Exception
    {
        public NotDivisibleException(Type t)
            : base($"Type '{t.Name}' not divisible: It's neither a CLR numeric-type, nor derived from IDivisible")
        {
        }
    }

    [Serializable]
    public class RefValidationFailedException : Exception
    {
        public RefValidationFailedException() :
            base("Ref validation failed")
        {
        }
    }

    [Serializable]
    public class DeadlockException : Exception
    {
        public DeadlockException() : base("Deadlock occured during atomic update") { }
    }

}
