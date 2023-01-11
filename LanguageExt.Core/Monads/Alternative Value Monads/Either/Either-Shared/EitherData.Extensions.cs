#nullable enable
using System;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.DataTypes.Serialisation
{
    public static class EitherDataExtensions
    {
        public static Either<L, R> ToEither<L, R>(this EitherData<L, R> input) =>
            input.State switch
            {
                EitherStatus.IsBottom => Either<L, R>.Bottom,
                EitherStatus.IsRight => Either<L, R>.Right(input.Right),
                EitherStatus.IsLeft => Either<L, R>.Left(input.Left),
                _ => throw new InvalidOperationException($"Invalid {nameof(EitherStatus)} with value: {input.State}"),
            };

        public static EitherAsync<L, R> ToEitherAsync<L, R>(this EitherData<L, R> input) =>
            input.ToEither().ToAsync();

        public static async Task<EitherAsync<L, R>> ToEitherAsync<L, R>(this Task<EitherData<L, R>> input) =>
            (await input).ToEitherAsync();

        public static async Task<EitherAsync<L, R>> ToEitherAsync<L, R>(this ValueTask<EitherData<L, R>> input) =>
            (await input).ToEitherAsync();

        public static EitherUnsafe<L, R> ToEitherUnsafe<L, R>(this EitherData<L, R> input) =>
            input.State switch
            {
                EitherStatus.IsBottom => EitherUnsafe<L, R>.Bottom,
                EitherStatus.IsRight => EitherUnsafe<L, R>.Right(input.Right),
                EitherStatus.IsLeft => EitherUnsafe<L, R>.Left(input.Left),
                _ => throw new InvalidOperationException($"Invalid {nameof(EitherStatus)} with value: {input.State}"),
            };

        public static Fin<A> ToFin<E, A>(this EitherData<E, A> input) where E : Error =>
            input.State switch
            {
                EitherStatus.IsBottom => Fin<A>.Fail(BottomError.Default),
                EitherStatus.IsRight => Fin<A>.Succ(input.Right),
                EitherStatus.IsLeft => Fin<A>.Fail(input.Left),
                _ => throw new InvalidOperationException($"Invalid {nameof(EitherStatus)} with value: {input.State}"),
            };

        public static Try<A> ToTry<E, A>(this EitherData<E, A> input) where E : Exception =>
            input.State switch
            {
                EitherStatus.IsBottom => Prelude.TryFail<A>(BottomException.Default),
                EitherStatus.IsRight => Prelude.TrySucc(input.Right),
                EitherStatus.IsLeft => Prelude.TryFail<A>(input.Left),
                _ => throw new InvalidOperationException($"Invalid {nameof(EitherStatus)} with value: {input.State}"),
            };

        public static TryAsync<A> ToTryAsync<E, A>(this EitherData<E, A> input) where E : Exception =>
            input.ToTry().ToAsync();

        public static async Task<TryAsync<A>> ToTryAsync<E, A>(this Task<EitherData<E, A>> input) where E : Exception =>
            (await input).ToTryAsync();

        public static async Task<TryAsync<A>> ToTryAsync<E, A>(this ValueTask<EitherData<E, A>> input) where E : Exception =>
            (await input).ToTryAsync();
    }
}
