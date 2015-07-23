using System;
using System.Collections.Generic;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Prelude' to your code.
    /// </summary>
    public static partial class Prelude
    {
        /// <summary>
        /// 'No value' state of Option<T>.  
        /// </summary>
        public static OptionNone None => 
            OptionNone.Default;

        public static Unit unit =>
            Unit.Default;

        public static Unit ignore<T>(T anything) =>
            unit;

        /// <summary>
        /// Projects a value into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        public static R map<T, R>(T value, Func<T, R> project) =>
            project(value);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        public static R map<T1, T2, R>(T1 value1, T2 value2, Func<T1, T2, R> project) =>
            project(value1,value2);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        public static R map<T1, T2, T3, R>(T1 value1, T2 value2, T3 value3, Func<T1, T2, T3, R> project) =>
            project(value1, value2, value3);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        public static R map<T1, T2, T3, T4, R>(T1 value1, T2 value2, T3 value3, T4 value4, Func<T1, T2, T3, T4, R> project) =>
            project(value1, value2, value3, value4);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        public static R map<T1, T2, T3, T4, T5, R>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, Func<T1, T2, T3, T4, T5, R> project) =>
            project(value1, value2, value3, value4, value5);

        /// <summary>
        /// Identity function
        /// </summary>
        public static T identity<T>(T x) => x;

        /// <summary>
        /// Raises a lazy Exception with the message provided
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <returns>Action that when executed throws</returns>
        public static Action failwith(string message) =>
            () => { throw new Exception(message); };

        /// <summary>
        /// Raises an Exception wigth the messge provided
        /// </summary>
        /// <typeparam name="R">The return type of the expression this function is being used in.
        /// This allows exceptions to be thrown in ternary operators, or LINQ expressions for
        /// example</typeparam>
        /// <param name="message">Exception message</param>
        /// <returns>Throws an exception</returns>
        public static R failwith<R>(string message)
        {
            throw new Exception(message);
        }

        /// <summary>
        /// Raise an exception
        /// </summary>
        /// <typeparam name="R">The return type of the expression this function is being used in.
        /// This allows exceptions to be thrown in ternary operators, or LINQ expressions for
        /// example</typeparam>
        /// <param name="ex">Exception to throw</param>
        /// <returns>Throws an exception</returns>
        public static R raise<R>(Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// Identifies an exception as being of type E
        /// </summary>
        /// <typeparam name="E">Type to match</typeparam>
        /// <param name="e">Exception to test</param>
        /// <returns>True if e is of type E</returns>
        public static bool exceptionIs<E>(Exception e)
        {
            if (e is E) return true;
            if (e.InnerException == null) return false;
            return exceptionIs<E>(e.InnerException);
        }
   }
}