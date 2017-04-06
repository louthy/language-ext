using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances.Pred
{
    public struct Equal<EQ, A, CONST> : Pred<A>
        where EQ    : struct, Eq<A>
        where CONST : struct, Const<A>
    {
        public static readonly Equal<EQ, A, CONST> Is = default(Equal<EQ, A, CONST>);

        [Pure]
        public bool True(A value) =>
            default(EQ).Equals(value, default(CONST).Value);
    }
}
