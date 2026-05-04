using System;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static class MaintainerExtensions
{
    extension<A>(A)
        where A : Maintainer<A>
    {
        public static Option<A> FindM(Func<A, bool> fa) =>
            Prelude.findM(fa);

        public static A Find(Func<A, bool> fa) =>
            Prelude.find(fa);
    }

    extension<A>(A a)
        where A : Maintainer<A>
    {
        public bool Is(A b) => a.Equals(b);
        public bool IsNot(A b) => !a.Equals(b);
    }

}

