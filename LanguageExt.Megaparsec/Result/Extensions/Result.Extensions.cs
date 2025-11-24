using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public static class ResultExtensions
{
    extension<S, E, A>(K<Result<S, E>, A> self)
    {
        public Result<S, E, A> As() =>
            (Result<S, E, A>)self;
        
        public static Result<S, E, A> operator+(K<Result<S, E>, A> ma) =>
            (Result<S, E, A>)ma;
    }
}
