using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FEnumerable<A, B> :
        Functor<IEnumerable<A>, IEnumerable<B>, A, B>
    {
        public static readonly FEnumerable<A, B> Inst = default(FEnumerable<A, B>);

        [Pure]
        public IEnumerable<B> Map(IEnumerable<A> ma, Func<A, B> f)
        {
            foreach (var a in ma)
                yield return f(a);
        }
    }
}
