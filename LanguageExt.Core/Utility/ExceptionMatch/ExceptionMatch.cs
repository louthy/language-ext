using System;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Pattern matching for exceptions.  This is to aid expression based error handling.
    /// </summary>
    /// <example>
    ///     ex.Match&lt;string&gt;()
    ///       .With&lt;SystemException&gt;(e =&gt; "It's a system exception")
    ///       .With&lt;ArgumentNullException&gt;(e =&gt; "Arg null")
    ///       .Otherwise("Not handled")
    /// </example>
    [Serializable]
    public class ExceptionMatch<R>
    {
        readonly Exception exception;
        bool valueSet;
        R value;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="e">Exception to match</param>
        internal ExceptionMatch(R value)
        {
            this.value = value;
            valueSet = true;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="e">Exception to match</param>
        internal ExceptionMatch(Exception e)
        {
            exception = e;
        }

        /// <summary>
        /// Matches a typed exception with a mapping function
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="map">Function to map the exception to a result value</param>
        /// <returns>Matching context - you must use 'Otherwise()' to invoke</returns>
        [Pure]
        public ExceptionMatch<R> With<TException>(Func<TException, R> map) where TException : Exception
        {
            if (exception is TException)
            {
                value = map(exception as TException);
                valueSet = true;
            }
            return this;
        }

        /// <summary>
        /// Invokes the match expression and provides a default value if nothing matches
        /// </summary>
        /// <returns>Result of the expression</returns>
        [Pure]
        public R OtherwiseReThrow() =>
            valueSet
                ? value
                : raise<R>(exception);

        /// <summary>
        /// Invokes the match expression and provides a default value if nothing matches
        /// </summary>
        /// <param name="otherwiseValue">Default value</param>
        /// <returns>Result of the expression</returns>
        [Pure]
        public R Otherwise(R otherwiseValue) =>
            valueSet
                ? value
                : otherwiseValue;

        /// <summary>
        /// Invokes the match expression and provides a default function to invoke if 
        /// nothing matches
        /// </summary>
        /// <param name="otherwise">Default value</param>
        /// <returns>Result of the expression</returns>
        [Pure]
        public R Otherwise(Func<R> otherwise) =>
            valueSet
                ? value
                : otherwise();

        /// <summary>
        /// Invokes the match expression and provides a default function to invoke if 
        /// nothing matches
        /// </summary>
        /// <param name="otherwiseMap">Default value</param>
        /// <returns>Result of the expression</returns>
        [Pure]
        public R Otherwise(Func<Exception, R> otherwiseMap) =>
            valueSet
                ? value
                : otherwiseMap(exception);
    }

    /// <summary>
    /// Exception extensions
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Pattern matching for exceptions.  This is to aid expression based error handling.
        /// </summary>
        /// <example>
        ///     ex.Match&lt;string&gt;()
        ///       .With&lt;SystemException&gt;(e =&gt; "It's a system exception")
        ///       .With&lt;ArgumentNullException&gt;(e =&gt; "Arg null")
        ///       .Otherwise("Not handled")
        /// </example>
        [Pure]
        public static LanguageExt.ExceptionMatch<R> Match<R>(this Exception self) =>
            new LanguageExt.ExceptionMatch<R>(self);

        /// <summary>
        /// Rethrows the exception whilst maintaining its original context
        /// </summary>
        /// <remarks>Unwraps any inner exceptions</remarks>
        /// <param name="exception">Exception to rethrow</param>
        public static Unit Rethrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
            return default;
        }

        /// <summary>
        /// Rethrows the exception whilst maintaining its original context
        /// </summary>
        /// <remarks>Unwraps any inner exceptions</remarks>
        /// <param name="exception">Exception to rethrow</param>
        public static A Rethrow<A>(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
            return default;
        }
    }
}
