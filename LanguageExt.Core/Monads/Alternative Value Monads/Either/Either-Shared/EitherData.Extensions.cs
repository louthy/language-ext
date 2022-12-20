using System;

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
    }
}
