using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.UnsafeValueAccess
{
    public static class UnsafeValueAccessExtensions
    {
        public static A ValueUnsafe<A>(this Option<A> option) =>
            option.IsSome
                ? option.Value
                : default(A);

        public static A Value<A>(this Option<A> option) where A : struct =>
            option.IsSome
                ? option.Value
                : default(A);

        public static A ValueUnsafe<A>(this OptionUnsafe<A> option) =>
            option.IsSome
                ? option.Value
                : default(A);

        public static A Value<A>(this OptionUnsafe<A> option) where A : struct =>
            option.IsSome
                ? option.Value
                : default(A);

        public static Task<A> ValueUnsafe<A>(this OptionAsync<A> option) =>
            option.Value;

        public static Task<A> Value<A>(this OptionAsync<A> option) where A : struct =>
            option.Value;

        public static R Value<L, R>(this Either<L, R> either) where R : struct =>
            either.IsRight
                ? either.RightValue
                : default(R);

        public static R Value<L, R>(this EitherUnsafe<L, R> either) where R : struct =>
            either.IsRight
                ? either.RightValue
                : default(R);

        public static R ValueUnsafe<L, R>(this Either<L, R> either) =>
            either.IsRight
                ? either.RightValue
                : default(R);

        public static R ValueUnsafe<L, R>(this EitherUnsafe<L, R> either) =>
            either.IsRight
                ? either.RightValue
                : default(R);
    }
}
