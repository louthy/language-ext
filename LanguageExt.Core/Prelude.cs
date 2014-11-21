using System;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using TvdP.Collections;

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
        /// Generates an identity function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<T,T> identity<T>() => (T id) => id;

        public static Action failaction(string message) =>
            () => { throw new Exception(message); };

        public static R failwith<R>(string message)
        {
            throw new Exception(message);
        }

        public static R raise<R>(Exception ex)
        {
            throw ex;
        }
    }
}