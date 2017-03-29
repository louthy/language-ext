using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct FTryAsync<A, B> : 
        Functor<TryAsync<A>, TryAsync<B>, A, B>,
        BiFunctor<TryAsync<A>, TryAsync<B>, A, Unit, B>
    {
        public static readonly FTryAsync<A, B> Inst = default(FTryAsync<A, B>);

        [Pure]
        public TryAsync<B> BiMap(TryAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) => () =>
            ma.Match(
                Succ: x => new Result<B>(fa(x)),
                Fail: _ => new Result<B>(fb(unit)));

        [Pure]
        public TryAsync<B> Map(TryAsync<A> ma, Func<A, B> f) => () =>
            ma.Match(
                Succ: x => new Result<B>(f(x)),
                Fail: e => new Result<B>(e));
    }
}
