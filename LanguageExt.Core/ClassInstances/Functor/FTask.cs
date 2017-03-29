using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTask<A, B> : 
        Functor<Task<A>, Task<B>, A, B>,
        BiFunctor<Task<A>, Task<B>, A, Unit, B>
    {
        public static readonly FTask<A, B> Inst = default(FTask<A, B>);

        [Pure]
        public Task<B> BiMap(Task<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FOptional<MTask<A>, MTask<B>, Task<A>, Task<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Task<B> Map(Task<A> ma, Func<A, B> f) =>
            FOptional<MTask<A>, MTask<B>, Task<A>, Task<B>, A, B>.Inst.Map(ma, f);
    }
}
