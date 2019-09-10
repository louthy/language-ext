using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FResource<A, B> :
        Functor<Resource<A>, Resource<B>, A, B>
    {
        public static readonly FResource<A, B> Inst = default;

        [Pure]
        public Resource<B> Map(Resource<A> ma, Func<A, B> f) =>
            MResource<A>.Inst.Bind<MResource<B>, Resource<B>, B>(ma, a => 
                MResource<B>.Inst.Return(_ => f(a)));
    }
}
