
namespace LanguageExt
{
    internal static class CastExtensions
    {
        public static A Cast<A>(this Option<A> ma) => (A)ma;
        public static L CastLeft<L, R>(this Either<L, R> ma) => (L)ma;
        public static R CastRight<L, R>(this Either<L, R> ma) => (R)ma;
    }
}
