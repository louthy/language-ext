using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Conditional computation.  Represents a computation that could succeed
    /// or fail.  The result of the computation is an Option data-type.
    /// </summary>
    /// <typeparam name="A">Input type to the conditional computation</typeparam>
    /// <typeparam name="B">Resulting value type on success</typeparam>
    /// <param name="input">Input value</param>
    /// <returns>Optional result</returns>
    public delegate Option<B> Cond<A, B>(A input);

    public static partial class Prelude
    {
        /// <summary>
        /// Conditional computation constructor.  Represents a computation that could 
        /// succeed or fail.  The result of the computation is an Option data-type.  
        /// Use the fluent API methods of Then and Else to extract the monadic value.
        /// </summary>
        /// <typeparam name="A">Input type to the conditional computation</typeparam>
        /// <param name="pred">Predicate to apply to the input value</param>
        /// <returns>Conditional computation</returns>
        [Pure]
        public static Cond<A, A> Cond<A>(Func<A, bool> pred) =>
            input =>
                pred(input)
                    ? Optional(input)
                    : None;

        /// <summary>
        /// Conditional computation constructor.  Represents a computation that could 
        /// succeed or fail.  The result of the computation is an Option data-type.  
        /// Use the fluent API methods of Then and Else to extract the monadic value.
        /// </summary>
        /// <typeparam name="A">Input type to the conditional computation</typeparam>
        /// <param name="pred">Predicate to apply to the input value</param>
        /// <returns>Conditional computation</returns>
        [Pure]
        public static Cond<A, A> Subj<A>() =>
            input =>
                Optional(input);
    }

    /// <summary>
    /// Extensions to the Cond type
    /// </summary>
    public static class CondExt
    {
        /// <summary>
        /// Apply a value as the first argument to the function provided.  
        /// </summary>
        /// <remarks>This is a general case apply function, however it is especially 
        /// useful for fluently applying a value to the result of the Cond.Else() 
        /// extension method.
        /// </remarks>
        /// <typeparam name="A">Type of the value to apply to the function</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="a">Value to apply to the function</param>
        /// <param name="f">Function to apply the value to</param>
        /// <returns>The result of applying the value to the function</returns>
        [Pure]
        public static B Apply<A, B>(this A a, Func<A, B> f) =>
            f(a);

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="f">The 'then' computation</param>
        /// <returns>A conditional computation</returns>
        [Pure]
        public static Cond<A, B> Then<A, B>(this Cond<A, A> self, Func<A, B> f) =>
            self.Select(f);

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="f">The 'then' computation</param>
        /// <returns>A conditional computation</returns>
        [Pure]
        public static Cond<A, B> Then<A, B>(this Cond<A, A> self, Func<B> f) =>
            self.Select(_ => f());

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="value">The 'then' value</param>
        /// <returns>A conditional computation</returns>
        [Pure]
        public static Cond<A, B> Then<A, B>(this Cond<A, A> self, B value) =>
            self.Select(_ => value);

        /// <summary>
        /// A conditional computation where the input value can match any of the predicates
        /// provided to return a positive Some/True state.  This is like an if(a || b || c ...)
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="predicates">The predicates to test the bound value with</param>
        /// <returns>A conditional computation</returns>
        [Pure]
        public static Cond<A, B> Any<A, B>(this Cond<A, B> self, params Func<B, bool>[] predicates) =>
            env =>
                self(env).Bind(
                    input =>
                    {
                        foreach (var pred in predicates)
                        {
                            if (pred(input))
                            {
                                return Optional(input);
                            }
                        }
                        return None;
                    });

        /// <summary>
        /// A conditional computation where the input value must match all of the predicates
        /// provided to return a positive Some/True state.  This is like an if(a && b && c ...)
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="predicates">The predicates to test the bound value with</param>
        /// <returns>A conditional computation</returns>
        [Pure]
        public static Cond<A, B> All<A, B>(this Cond<A, B> self, params Func<B, bool>[] predicates) =>
            env =>
                self(env).Bind(
                    input =>
                    {
                        foreach (var pred in predicates)
                        {
                            if (!pred(input))
                            {
                                return None;
                            }
                        }
                        return Optional(input);
                    });

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="f">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        [Pure]
        public static Func<A, B> Else<A, B>(this Cond<A, B> self, Func<A, B> f) =>
            input =>
                self(input).IfNone(() => f(input));

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="f">The else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        [Pure]
        public static Func<A, B> Else<A, B>(this Cond<A, B> self, Func<B> f) =>
            input =>
                self(input).IfNone(f);

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="value">The else value to use if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        [Pure]
        public static Func<A, B> Else<A, B>(this Cond<A, B> self, B value) =>
            input =>
                self(input).IfNone(value);

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="f">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        [Pure]
        public static Func<Task<A>, Task<B>> ElseAsync<A, B>(this Cond<A, B> self, Func<A, B> f) =>
            input =>
                from a in input
                select self(a).IfNone(() => f(a));

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="f">The else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        [Pure]
        public static Func<Task<A>, Task<B>> ElseAsync<A, B>(this Cond<A, B> self, Func<B> f) =>
            input =>
                from a in input
                select self(a).IfNone(f);

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="value">The else value to use if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        [Pure]
        public static Func<Task<A>, Task<B>> ElseAsync<A, B>(this Cond<A, B> self, B value) =>
            input =>
                from a in input
                select self(a).IfNone(value);

        /// <summary>
        /// Functor map of the conditional computation
        /// </summary>
        /// <typeparam name="A">Source computation input type</typeparam>
        /// <typeparam name="B">Source computation output type</typeparam>
        /// <typeparam name="C">Mapped computation output type</typeparam>
        /// <param name="self">The conditional computation to map</param>
        /// <param name="map">Functor mapping function</param>
        /// <returns>A mapped conditional computation</returns>
        [Pure]
        public static Cond<A, C> Map<A, B, C>(this Cond<A, B> self, Func<B, C> map) =>
            Select(self, map);

        /// <summary>
        /// Functor map of the conditional computation
        /// </summary>
        /// <typeparam name="A">Source computation input type</typeparam>
        /// <typeparam name="B">Source computation output type</typeparam>
        /// <typeparam name="C">Mapped computation output type</typeparam>
        /// <param name="self">The conditional computation to map</param>
        /// <param name="map">Functor mapping function</param>
        /// <returns>A mapped conditional computation</returns>
        [Pure]
        public static Cond<A, C> Select<A, B, C>(this Cond<A, B> self, Func<B, C> map) =>
            input =>
                self(input).Map(map);

        /// <summary>
        /// Monadic bind for conditional computations.  Allows nesting of computations 
        /// that follow the rules of Cond, namely that a None/False result cancels the
        /// operation until an Else is encountered.
        /// </summary>
        [Pure]
        public static Cond<A, D> SelectMany<A, B, C, D>(
            this Cond<A, B> self,
            Func<B, Cond<A, C>> bind,
            Func<B, C, D> project) =>
                input =>
                    from b in self(input)
                    from c in bind(b)(input)
                    select project(b, c);

        /// <summary>
        /// Filter the conditional computation.  This is the equivalent of the predicate
        /// in an If statement.
        /// </summary>
        /// <typeparam name="A">The input type of the conditional computation</typeparam>
        /// <typeparam name="B">The output type of the conditional computation</typeparam>
        /// <param name="self">The conditional computation to test</param>
        /// <param name="pred">The predicate function</param>
        /// <returns>A conditional computation</returns>
        [Pure]
        public static Cond<A, B> Filter<A, B>(this Cond<A, B> self, Func<B, bool> pred) =>
            Where(self, pred);

        /// <summary>
        /// Filter the conditional computation.  This is the equivalent of the predicate
        /// in an If statement.
        /// </summary>
        /// <typeparam name="A">The input type of the conditional computation</typeparam>
        /// <typeparam name="B">The output type of the conditional computation</typeparam>
        /// <param name="self">The conditional computation to test</param>
        /// <param name="pred">The predicate function</param>
        /// <returns>A conditional computation</returns>
        [Pure]
        public static Cond<A, B> Where<A, B>(this Cond<A, B> self, Func<B, bool> pred) =>
            input =>
                self(input).Where(pred);
    }
}
