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
        public static R with<T, R>(T value, Func<T, R> project) =>
            project(value);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        public static R with<T1, T2, R>(T1 value1, T2 value2, Func<T1, T2, R> project) =>
            project(value1,value2);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        public static R with<T1, T2, T3, R>(T1 value1, T2 value2, T3 value3, Func<T1, T2, T3, R> project) =>
            project(value1, value2, value3);

        /// <summary>
        /// Generates an identity function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<T,T> identity<T>() => (T id) => id;

        public static Action failwith(string message) =>
            () => { throw new Exception(message); };

        public static R failwith<R>(string message)
        {
            throw new Exception(message);
        }

        public static R raise<R>(Exception ex)
        {
            throw ex;
        }

        public static bool exceptionIs<E>(Exception e)
        {
            if (e is E) return true;
            if (e.InnerException == null) return false;
            return exceptionIs<E>(e.InnerException);
        }
    }
}