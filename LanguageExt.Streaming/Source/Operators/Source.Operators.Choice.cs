using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceExtensions
{
    extension<A>(K<Source, A> self)
    {
        public static Source<A> operator |(K<Source, A> lhs, K<Source, A> rhs) =>
            +lhs.Choose(rhs);

        public static Source<A> operator |(K<Source, A> lhs, Pure<A> rhs) =>
            +lhs.Choose(Source.pure(rhs.Value));
    }
}
