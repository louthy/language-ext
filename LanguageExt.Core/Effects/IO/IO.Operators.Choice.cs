using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A>(K<IO, A> self)
    {
        public static IO<A> operator |(K<IO, A> lhs, CatchM<Error, IO, A> rhs) =>
            lhs.Catch(rhs).As();

        public static IO<A> operator |(K<IO, A> lhs, Fail<Error> rhs) =>
            lhs.Catch(rhs).As();
    }
}
