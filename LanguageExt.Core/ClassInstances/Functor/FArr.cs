using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FArr<A, B> : 
        Functor<Arr<A>, Arr<B>, A, B>
    {
        public static readonly FArr<A, B> Inst = default(FArr<A, B>);

        [Pure]
        public Arr<B> Map(Arr<A> ma, Func<A, B> f)
        {
            var bs = new B[ma.Count];
            var iter = ma.GetEnumerator();
            for (int i = 0; iter.MoveNext(); i++)
            {
                bs[i] = f(iter.Current);
            }
            return bs;
        }
    }
}
