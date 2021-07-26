using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// This creates a Seq from an Array without any copying of data, so it's super fast
        /// However because the input array is mutable it weakens the guarantees of the immutable Seq, so this is not
        /// advised unless you know what you're doing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<A> ToSeqUnsafe<A>(this A[] value) =>
            Seq.FromArray(value);

        /// <summary>
        /// This creates a Seq from an Array without any copying of data, so it's super fast
        /// However because the input array is mutable it weakens the guarantees of the immutable Seq, so this is not
        /// advised unless you know what you're doing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<A> ToSeqUnsafe<A>(this A[] value, int length) =>
            Seq.FromArray(value, length);

        /// <summary>
        /// This creates a Seq from an Array without any copying of data, so it's super fast
        /// However because the input array is mutable it weakens the guarantees of the immutable Seq, so this is not
        /// advised unless you know what you're doing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SeqLoan<A> ToSeqLoanUnsafe<A>(this A[] value, ArrayPool<A> pool) =>
            new SeqLoan<A>(value, pool, 0, value.Length);

        /// <summary>
        /// This creates a Seq from an Array without any copying of data, so it's super fast
        /// However because the input array is mutable it weakens the guarantees of the immutable Seq, so this is not
        /// advised unless you know what you're doing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SeqLoan<A> ToSeqLoanUnsafe<A>(this A[] value, int length, ArrayPool<A> pool) =>
            new SeqLoan<A>(value, pool, 0, length);
    }
}
