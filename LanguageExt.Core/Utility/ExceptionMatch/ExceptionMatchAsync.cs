using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt.Common;

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
    public class ExceptionMatchAsync<R>
    {
        readonly Task<Result<R>> Value;
        List<Func<Exception, (R Value, bool IsSet)>> actions = new List<Func<Exception, (R Value, bool IsSet)>>();

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="e">Exception to match</param>
        internal ExceptionMatchAsync(Task<Result<R>> value) =>
            Value = value;

        /// <summary>
        /// Matches a typed exception with a mapping function
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="map">Function to map the exception to a result value</param>
        /// <returns>Matching context - you must use 'Otherwise()' to invoke</returns>
        [Pure]
        public ExceptionMatchAsync<R> With<TException>(Func<TException, R> map) where TException : Exception
        {
            actions.Add(ex =>
            {
                if (ex is TException)
                {
                    return (map(ex as TException), true);
                }
                else
                {
                    return (default(R), false);
                }
            });
            return this;
        }

        Task<(R Result, bool IsSet, Exception Exception)> Expr =>
            from res in Value
            let tup = res.IsFaulted
                ? actions.Fold((default(R), false), (state, action) => state.Item2 ? state : action(res.Exception))
                : (res.Value, true)
            select (tup.Item1, tup. Item2, res.Exception);

        /// <summary>
        /// Invokes the match expression and provides a default value if nothing matches
        /// </summary>
        /// <returns>Result of the expression</returns>
        [Pure]
        public Task<R> OtherwiseReThrow() =>
            from tup in Expr
            select tup.IsSet
                ? tup.Result
                : throw new InnerException(tup.Exception);

        /// <summary>
        /// Invokes the match expression and provides a default value if nothing matches
        /// </summary>
        /// <param name="otherwiseValue">Default value</param>
        /// <returns>Result of the expression</returns>
        [Pure]
        public Task<R> Otherwise(R otherwiseValue) =>
            from tup in Expr
            select tup.IsSet
                ? tup.Result
                : otherwiseValue;

        /// <summary>
        /// Invokes the match expression and provides a default function to invoke if 
        /// nothing matches
        /// </summary>
        /// <param name="otherwise">Default value</param>
        /// <returns>Result of the expression</returns>
        [Pure]
        public Task<R> Otherwise(Func<R> otherwise) =>
            from tup in Expr
            select tup.IsSet
                ? tup.Result
                : otherwise();

        /// <summary>
        /// Invokes the match expression and provides a default function to invoke if 
        /// nothing matches
        /// </summary>
        /// <param name="otherwiseMap">Default value</param>
        /// <returns>Result of the expression</returns>
        [Pure]
        public Task<R> Otherwise(Func<Exception, R> otherwiseMap) =>
            from tup in Expr
            select tup.IsSet
                ? tup.Result
                : otherwiseMap(tup.Exception);
    }

    /// <summary>
    /// Exception extensions
    /// </summary>
    public static class ExceptionAsyncExtensions
    {
        /// <summary>
        /// Pattern matching for exceptions.  This is to aid expression based error handling.
        /// </summary>
        /// <example>
        ///     tr.Match&lt;string&gt;()
        ///       .With&lt;SystemException&gt;(e =&gt; "It's a system exception")
        ///       .With&lt;ArgumentNullException&gt;(e =&gt; "Arg null")
        ///       .Otherwise("Not handled")
        /// </example>
        [Pure]
        public static LanguageExt.ExceptionMatchAsync<R> Match<R>(this Task<Result<R>> self) =>
            new LanguageExt.ExceptionMatchAsync<R>(self);
    }
}
