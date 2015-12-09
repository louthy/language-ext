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

#if !(NETFX_CORE || DNXCORE50)
    [TypeConverter(typeof(OptionalTypeConverter))] 
#endif
    public struct Some<T> : IOptional
    {
        readonly T value;
        readonly bool initialised;

        public Some(T value)
        {
            if (isnull(value))
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
            Value.ToString();

        public override int GetHashCode() =>
            Value.GetHashCode();

        public override bool Equals(object obj) =>
            Value.Equals(obj);

        public bool IsSome =>
            initialised;

        public bool IsNone =>
            !initialised;

        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            IsSome
                ? Some(value)
                : None();

        public Type GetUnderlyingType() =>
            typeof(T);
    }

    public static class Some
    {
        public static Some<T> Create<T>(T x) =>
            new Some<T>(x);
    }
}
