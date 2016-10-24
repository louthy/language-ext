using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Instances
{
    public struct TArray<A> : Monoid<A[]>
    {
        public static readonly TArray<A> Inst = default(TArray<A>);

        static readonly A[] emp = new A[0];

        public A[] Append(A[] x, A[] y) =>
            x.Concat(y).ToArray();

        public A[] Empty() => emp;
    }
}
