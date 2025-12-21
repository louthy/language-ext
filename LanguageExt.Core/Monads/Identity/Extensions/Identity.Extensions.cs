using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IdentityExtensions
{
    extension<A>(K<Identity, A> ma)
    {
        public Identity<A> As() =>
            (Identity<A>)ma;

        public A Run() =>
            ((Identity<A>)ma).Value;
    }
}
