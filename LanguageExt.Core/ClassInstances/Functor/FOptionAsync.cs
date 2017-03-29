using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FOptionAsync<A, B> : 
        Functor<OptionAsync<A>, OptionAsync<B>, A, B>,
        BiFunctor<OptionAsync<A>, OptionAsync<B>, A, Unit, B>
    {
        public static readonly FOptionAsync<A, B> Inst = default(FOptionAsync<A, B>);

        [Pure]
        public OptionAsync<B> BiMap(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            new OptionAsync<B>(OptionDataAsync.Lazy<B>(async () =>
                await ma.Match(
                    Some: x => fa(x),
                    None: () => fb(unit))));

        [Pure]
        public OptionAsync<B> Map(OptionAsync<A> ma, Func<A, B> f) =>
            MOptionAsync<A>.Inst.Bind<MOptionAsync<B>, OptionAsync<B>, B>(ma, a => MOptionAsync<B>.Inst.Return(f(a)));

        [Pure]
        public OptionAsync<B> Map(OptionAsync<A> ma, Func<A, Task<B>> f) =>
            MOptionAsync<A>.Inst.Bind<MOptionAsync<B>, OptionAsync<B>, B>(ma, a =>
                MOptionAsync<B>.Inst.IdAsync(async _ =>
                   MOptionAsync<B>.Inst.Return(await f(a))));
    }
}
