using System;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public static Seq<A> get<A>()
      where A : Maintainer<A> =>
      A.All;

    public static Option<A> findM<A>(Func<A, bool> fa)
      where A : Maintainer<A> =>
      get<A>().Find(fa);

    public static A find<A>(Func<A, bool> fa)
        where A : Maintainer<A> =>
        findM(fa).Case switch
        {
            A a => a,
            _ => throw new InvalidOperationException("Option was None")
        };
}
