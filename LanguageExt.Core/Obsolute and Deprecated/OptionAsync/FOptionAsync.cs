using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    [Obsolete(Change.UseEffMonadInstead)]
    public struct FOptionAsync<A, B> : 
        FunctorAsync<OptionAsync<A>, OptionAsync<B>, A, B>
    {
        public static readonly FOptionAsync<A, B> Inst = default(FOptionAsync<A, B>);

        [Pure]
        public OptionAsync<B> Map(OptionAsync<A> ma, Func<A, B> f) =>
            default(MOptionAsync<A>).Bind<MOptionAsync<B>, OptionAsync<B>, B>(ma,
                a => default(MOptionAsync<B>).ReturnAsync(f(a).AsTask()));

        [Pure]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, Task<B>> f) =>
            default(MOptionAsync<A>).Bind<MOptionAsync<B>, OptionAsync<B>, B>(ma,
                a => default(MOptionAsync<B>).ReturnAsync(f(a)));
    }
}
