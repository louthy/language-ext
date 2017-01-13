using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static System.Threading.Tasks.Task;

namespace LanguageExt.DataTypes.Conditional
{
    /// <summary>
    /// Cond is a construct used in many LISPs in the same manner as if/else if/else is used in C#
    /// This class is designed to emulate the spirit of contruct in a fluent style.
    /// The class implements the Optional Interface as unless an else clause is provided,
    /// the structure may not have a result.      
    ///     
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public struct Conditional<A, B> : Optional<Conditional<A, B>, B>, IOptional
    {
        /// <summary>
        /// This is set to Some when the predicate evaluates to true
        /// </summary>
        private readonly Option<B> result;

        /// <summary>
        /// This is the seed value which is passed to each predicate and result functions
        /// </summary>
        private readonly A subject;


        internal Conditional(Some<A> subject)
        {
            this.subject = subject;
            result = Option<B>.None;            
        }

        internal Conditional(Some<A> subject, Some<B> result)
        {
            this.subject = subject;
            this.result = result;
        }


        /// <summary>
        /// If no other conditions were matched, then the result falls through to the else statement
        /// </summary>
        /// <param name="None">Function which produces the value if none of the previous predicates were true</param>
        /// <returns></returns>
        [Pure]
        public B Else(Func<A, B> None) => map(this, continuation => continuation.result.Match(Some: identity, None: () => None(continuation.subject)));


        /// <summary>
        /// If no other conditions were matched, then the result falls through to the else statement
        /// </summary>
        /// <param name="None">Asynchronous function which produces the value if none of the previous predicates were true</param>
        /// <returns></returns>
        [Pure]
        public Task<B> Else(Func<A, Task<B>> None) =>
            map(this, continuation => continuation.result.Match(Some: FromResult, None: () => None(continuation.subject)));

        /// <summary>
        /// If no previous predicate has evaulated to true, then the current predicate is evaluated. If the predicate 
        /// matches, then the return function is invoked and the result set
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="returns"></param>
        /// <returns></returns>
        public Conditional<A, B> Cond(Func<A, bool> predicate, Func<A, B> returns) =>
            map(this, continuation =>
                continuation.result.Match
                (
                    Some: (_) => continuation,
                    None: () => predicate(continuation.subject)
                        ? new Conditional<A, B>(continuation.subject, returns(continuation.subject))
                        : new Conditional<A, B>(continuation.subject)
                ));

        /// <summary>
        /// If no previous predicate has evaulated to true, then the current predicate is evaluated. If the predicate 
        /// matches, then the return function is invoked and the result set
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="returns"></param>
        /// <returns></returns>
        public Task<Conditional<A, B>> Cond(Func<A, Task<bool>> predicate, Func<A, B> returns) =>
            map(this, continuation =>
                continuation.result.Match
                (
                    Some: (_) => FromResult(continuation),
                    None: async () => await predicate(continuation.subject)
                        ? new Conditional<A, B>(continuation.subject, returns(continuation.subject))
                        : new Conditional<A, B>(continuation.subject)
                ));


        /// <summary>
        /// If no previous predicate has evaulated to true, then the current predicate is evaluated. If the predicate 
        /// matches, then the return function is invoked and the result set
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="returns"></param>
        /// <returns></returns>
        [Pure]
        public Task<Conditional<A, B>> Cond(Func<A, bool> predicate, Func<A, Task<B>> returns) =>
            map(this, continuation =>
                continuation.result.Match
                (
                    Some: _ => FromResult(continuation),
                    None: async () => predicate(continuation.subject)
                        ? new Conditional<A, B>(continuation.subject, await returns(continuation.subject))
                        : new Conditional<A, B>(continuation.subject)
                ));

        /// <summary>
        /// If no previous predicate has evaulated to true, then the current predicate is evaluated. If the predicate 
        /// matches, then the return function is invoked and the result set
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="returns"></param>
        /// <returns></returns>
        [Pure]
        public Task<Conditional<A, B>> Cond(Func<A, Task<bool>> predicate, Func<A, Task<B>> returns) =>
            map(this, continuation =>
                continuation.result.Match
                (
                    Some: (_) => FromResult(continuation),
                    None: async () => await predicate(continuation.subject)
                        ? new Conditional<A, B>(continuation.subject, await returns(continuation.subject))
                        : new Conditional<A, B>(continuation.subject)
                ));



        [Pure]
        public C Match<C>(Func<B, C> Some, Func<C> None) => result.Match(Some: Some, None: None);

        [Pure]
        public C MatchUnsafe<C>(Func<B, C> Some, Func<C> None) => result.MatchUnsafe(Some: Some, None: None);


        /// <summary>
        /// Implicitely converts a conditional type into an option
        /// </summary>
        /// <param name="self"></param>
        [Pure]
        public static implicit operator Option<B>(Conditional<A, B> self) =>
            self.result.Match
            (
                Some: Optional,
                None: () => None
            );



        [Pure]
        public bool IsSome => result.IsSome;

        [Pure]
        public bool IsNone => result.IsNone;

        [Pure]
        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) => result.Match(Some: (result) => Some(result), None: None);

        [Pure]
        public Type GetUnderlyingType() => result.GetUnderlyingType();

        [Pure]
        public bool IsUnsafe(Conditional<A, B> opt) => false;

        [Pure]
        bool Optional<Conditional<A, B>, B>.IsSome(Conditional<A, B> opt) => opt.IsSome;

        [Pure]
        bool Optional<Conditional<A, B>, B>.IsNone(Conditional<A, B> opt) => opt.IsNone;

        [Pure]
        public C Match<C>(Conditional<A, B> opt, Func<B, C> Some, Func<C> None) => opt.Match(Some: Some, None: None);

        [Pure]
        public C MatchUnsafe<C>(Conditional<A, B> opt, Func<B, C> Some, Func<C> None) => opt.MatchUnsafe<C>(Some: Some, None: None);

        [Pure]
        public Unit Match(Conditional<A, B> opt, Action<B> Some, Action None) => opt.result.Match(Some: Some, None: None);
    }
}