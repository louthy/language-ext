using System;
using System.Diagnostics.Contracts;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;
using System.Collections.Generic;
using LanguageExt.Common;

namespace LanguageExt
{
    public static partial class OptionalAsync
    {
        /// <summary>
        /// Invokes the f action if Option is in the Some state, otherwise nothing happens.
        /// </summary>
        public static Task<Unit> ifSomeAsync<OPT, OA, A>(OA opt, Action<A> f)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(opt, f, Optional.noneIgnore);

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public static Task<Unit> ifSomeAsync<OPT, OA, A>(OA opt, Func<A, Unit> f)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(opt, f, Optional.noneIgnoreF);

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public static Task<Unit> ifSomeAsync<OPT, OA, A>(OA opt, Func<A, Task<Unit>> f)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(opt, f, Optional.noneIgnoreF);

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public static Task<Unit> ifSomeAsync<OPT, OA, A>(OA opt, Func<A, Task> f)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(opt, async a => { await f(a).ConfigureAwait(false); return unit; } , Optional.noneIgnoreF);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public static Task<A> ifNoneAsync<OPT, OA, A>(OA opt, Func<A> None)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(opt, a => a, None);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public static Task<A> ifNoneAsync<OPT, OA, A>(OA opt, Func<Task<A>> None)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(opt, a => a, None);

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null noneValue</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public static Task<A> ifNoneAsync<OPT, OA, A>(OA opt, A noneValue)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(opt, a => a, () => noneValue);

        /// <summary>
        /// Pattern match operation
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static Task<R> matchAsync<OPT, OA, A, R>(OA ma, Func<A, R> Some, Func<R> None)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(ma,
                Some: x => Some(x),
                None: () => None());

        /// <summary>
        /// Pattern match operation
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static Task<R> matchAsync<OPT, OA, A, R>(OA ma, Func<A, Task<R>> SomeAsync, Func<R> None)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(ma,
                SomeAsync: async x => await SomeAsync(x).ConfigureAwait(false),
                None: () => None());

        /// <summary>
        /// Pattern match operation
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static Task<R> matchAsync<OPT, OA, A, R>(OA ma, Func<A, R> Some, Func<Task<R>> NoneAsync)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(ma,
                Some: x => Some(x),
                NoneAsync: async () => await NoneAsync().ConfigureAwait(false));

        /// <summary>
        /// Pattern match operation
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static Task<R> matchAsync<OPT, OA, A, R>(OA ma, Func<A, Task<R>> SomeAsync, Func<Task<R>> NoneAsync)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(ma,
                SomeAsync: async x => await SomeAsync(x).ConfigureAwait(false),
                NoneAsync: async () => await NoneAsync().ConfigureAwait(false));

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static Task<R> matchUntypedAsync<OPT, OA, A, R>(OA ma, Func<object, R> Some, Func<R> None)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match( ma,
                Some: x  => Some(x),
                None: () => None());

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static Task<R> matchUntypedAsync<OPT, OA, A, R>(OA ma, Func<object, Task<R>> SomeAsync, Func<R> None)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(ma,
                SomeAsync: async x => await SomeAsync(x).ConfigureAwait(false),
                None: () => None());

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static Task<R> matchUntypedAsync<OPT, OA, A, R>(OA ma, Func<object, R> Some, Func<Task<R>> NoneAsync)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(ma,
                Some: x => Some(x),
                NoneAsync: async () => await NoneAsync().ConfigureAwait(false));

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static Task<R> matchUntypedAsync<OPT, OA, A, R>(OA ma, Func<object, Task<R>> SomeAsync, Func<Task<R>> NoneAsync)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).MatchAsync(ma,
                SomeAsync: async x => await SomeAsync(x).ConfigureAwait(false),
                NoneAsync: async () => await NoneAsync().ConfigureAwait(false));

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static Task<Arr<A>> toArrayAsync<OPT, OA, A>(OA ma)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match( ma,
                Some: x  => Arr.create(x), 
                None: () => Arr.empty<A>());

        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public static Task<Lst<A>> toListAsync<OPT, OA, A>(OA ma)
            where OPT : struct, OptionalAsync<OA, A> =>
            toArrayAsync<OPT, OA, A>(ma).Map(Prelude.toList);

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static Task<IEnumerable<A>> asEnumerableAsync<OPT, OA, A>(OA ma)
            where OPT : struct, OptionalAsync<OA, A> =>
            toArrayAsync<OPT, OA, A>(ma).Map(x => (IEnumerable<A>)x.Value);

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        [Pure]
        public static EitherAsync<L, A> toEitherAsync<OPT, OA, L, A>(OA ma, L defaultLeftValue)
            where OPT : struct, OptionalAsync<OA, A> =>
            new EitherAsync<L, A>(
                default(OPT).Match(ma,
                    Some: x => new EitherData<L, A>(EitherStatus.IsRight, x, default(L)),
                    None: () => new EitherData<L, A>(EitherStatus.IsLeft, default(A), defaultLeftValue)));

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        [Pure]
        public static EitherAsync<L, A> toEitherAsync<OPT, OA, L, A>(OA ma, Func<L> Left)
            where OPT : struct, OptionalAsync<OA, A> =>
            new EitherAsync<L, A>(
                default(OPT).Match(ma,
                    Some: x => new EitherData<L, A>(EitherStatus.IsRight, x, default(L)),
                    None: () => new EitherData<L, A>(EitherStatus.IsLeft, default(A), Left())));

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        [Pure]
        public static Task<EitherUnsafe<L, A>> toEitherUnsafeAsync<OPT, OA, L, A>(OA ma, L defaultLeftValue)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(ma,
                Some: x  => RightUnsafe<L, A>(x),
                None: () => LeftUnsafe<L, A>(defaultLeftValue));

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        [Pure]
        public static Task<EitherUnsafe<L, A>> toEitherUnsafeAsync<OPT, OA, L, A>(OA ma, Func<L> Left)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(ma,
                Some: x  => RightUnsafe<L, A>(x),
                None: () => LeftUnsafe<L, A>(Left()));

        /// <summary>
        /// Convert the structure to a Option
        /// </summary>
        [Pure]
        public static OptionAsync<A> toOptionAsync<OPT, OA, A>(OA ma)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(ma,
                Some: x  => Optional(x),
                None: () => Option<A>.None).ToAsync();

        /// <summary>
        /// Convert the structure to a OptionUnsafe
        /// </summary>
        [Pure]
        public static Task<OptionUnsafe<A>> toOptionUnsafeAsync<OPT, OA, A>(OA ma)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(ma,
                Some: x  => SomeUnsafe(x),
                None: () => OptionUnsafe<A>.None);

        /// <summary>
        /// Convert the structure to a TryOptionAsync
        /// </summary>
        [Pure]
        public static TryOptionAsync<A> toTryOptionAsync<OPT, OA, A>(OA ma)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(ma,
                Some: x  => TryOption(x),
                None: () => TryOption(Option<A>.None)).ToAsync();

        /// <summary>
        /// Convert the structure to a TryAsync
        /// </summary>
        [Pure]
        public static TryAsync<A> toTryAsync<OPT, OA, A>(OA ma)
            where OPT : struct, OptionalAsync<OA, A> =>
            default(OPT).Match(ma,
                Some: x  => Try(x),
                None: () => Try<A>(BottomException.Default)).ToAsync();

    }
}
