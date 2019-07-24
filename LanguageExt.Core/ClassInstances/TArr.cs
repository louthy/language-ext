using LanguageExt;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct TArr<A> : 
        Monoid<Arr<A>>
    {
        public static readonly TArr<A> Inst = default(TArr<A>);

        static readonly Arr<A> emp = new A[0];

        [Pure]
        public Arr<A> Append(Arr<A> x, Arr<A> y) =>
            x.ConcatFast(y).ToArray();

        [Pure]
        public Arr<A> Empty() => emp;
    }
}
