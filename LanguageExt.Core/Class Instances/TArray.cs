using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct TArray<A> : Monoid<A[]>
    {
        public static readonly TArray<A> Inst = default(TArray<A>);

        static readonly A[] emp = new A[0];

        [Pure]
        public A[] Append(A[] x, A[] y) =>
            x.ConcatFast(y).ToArray();

        [Pure]
        public A[] Empty() => emp;
    }
}
