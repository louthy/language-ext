using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTryOptionAsync<A, B> : 
        Functor<TryOptionAsync<A>, TryOptionAsync<B>, A, B>,
        BiFunctor<TryOptionAsync<A>, TryOptionAsync<B>, A, Unit, B>
    {
        public static readonly FTryOptionAsync<A, B> Inst = default(FTryOptionAsync<A, B>);

        [Pure]
        public TryOptionAsync<B> BiMap(TryOptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FOptional<MTryOptionAsync<A>, MTryOptionAsync<B>, TryOptionAsync<A>, TryOptionAsync<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryOptionAsync<B> Map(TryOptionAsync<A> ma, Func<A, B> f) =>
            FOptional<MTryOptionAsync<A>, MTryOptionAsync<B>, TryOptionAsync<A>, TryOptionAsync<B>, A, B>.Inst.Map(ma, f);
    }
}
