using System.Threading.Tasks;
using LanguageExt;

namespace Contoso.Core
{
    public static class LanguageExtensions
    {
        public static Either<Error, R> ToEither<R>(this Validation<Error, R> validation) =>
            validation.ToEither().MapLeft(errors => errors.Join());

        public static Task<Either<Error, R>> ToEither<R>(this Task<Validation<Error, R>> validation) =>
            validation.Map(v => v.ToEither<R>());

        public static Task<Either<Error, R>> ToEitherAsync<R>(this Validation<Error, Task<R>> validation) =>
            validation.ToEither()
                .MapLeft(errors => errors.Join())
                .MapAsync<Error, Task<R>, R>(e => e);
    }
}
