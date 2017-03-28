using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTryAsync<A, B> : 
        Functor<TryAsync<A>, TryAsync<B>, A, B>,
        BiFunctor<TryAsync<A>, TryAsync<B>, Unit, A, B>
    {
        public static readonly FTryAsync<A, B> Inst = default(FTryAsync<A, B>);

        [Pure]
        public TryAsync<B> BiMap(TryAsync<A> ma, Func<Unit, B> fa, Func<A, B> fb) =>
            FOptional<MTryAsync<A>, MTryAsync<B>, TryAsync<A>, TryAsync<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryAsync<B> Map(TryAsync<A> ma, Func<A, B> f) =>
            FOptional<MTryAsync<A>, MTryAsync<B>, TryAsync<A>, TryAsync<B>, A, B>.Inst.Map(ma, f);
    }
}
