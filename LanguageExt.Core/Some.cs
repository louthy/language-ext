using System;
using System.ComponentModel;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    namespace SomeHelp
    {
        /// <summary>
        /// Extension method for Some T to help with the lack of covariance of generic
        /// parameters in structs (and therefore Some T)
        /// </summary>
        public static class SomeExt
        {
            /// <summary>
            /// Convert value to Some T.  Helps with the lack of covariance of generic
            /// parameters in structs (and therefore Some T)
            /// </summary>
            /// <typeparam name="T">Value type</typeparam>
            /// <param name="value">Value</param>
            /// <returns>Value wrapped in a Some T</returns>
            public static Some<T> ToSome<T>(this T value)
            {
                return new Some<T>(value);
            }
        }
    }

    /// <summary>
    /// Holds a value that is guaranteed not to be null.
    /// Use MatchUntyped or Value to access the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [TypeConverter(typeof(SomeTypeConverter))]
    public struct Some<T> : IOptional
    {
        readonly T value;
        readonly bool initialised;

        public Some(T value)
        {
            if (value == null)
            {
                throw new ValueIsNullException("Value is null when expecting Some(x)");
            }
            this.value = value;
            initialised = true;
        }

        public T Value => 
            CheckInitialised(value);

        private U CheckInitialised<U>(U value) =>
            initialised
                ? value
                : raise<U>( new SomeNotInitialisedException(typeof(T)) );

        public static implicit operator Option<T>(Some<T> value) =>
            Option<T>.Some(value.Value);

        public static implicit operator Some<T>(T value) => 
            new Some<T>(value);

        public static implicit operator T(Some<T> value) => 
            value.Value;

        public override string ToString() =>
            initialised
                ? value.ToString()
                : "(uninitialised)";        // ToString is not allowed to throw exceptions

        public override int GetHashCode() =>
            initialised
                ? value.GetHashCode()
                : 0;                        // GetHashCode is not allowed to throw exceptions

        public override bool Equals(object obj) =>
            initialised
                ? value.Equals(obj)
                : false;                    // Equals is not allowed to throw exceptions

        public bool IsSome =>
            CheckInitialised(true);

        public bool IsNone =>
            CheckInitialised(false);

        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            IsSome
                ? Some(value)
                : raise<R>(new InvalidOperationException("should never be None"));

        public Type GetUnderlyingType() =>
            CheckInitialised(typeof(T));
    }

    public static class Some
    {
        public static Some<T> Create<T>(T x) =>
            new Some<T>(x);
    }
}
