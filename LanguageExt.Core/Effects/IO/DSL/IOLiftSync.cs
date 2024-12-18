using System;
using System.Threading.Tasks;

namespace LanguageExt.DSL;

record IOLiftSync<A>(Func<EnvIO, A> F) : IODsl<A>
{
    public override IODsl<B> Map<B>(Func<A, B> f) =>
        new IOLiftSync<B>(x => f(F(x)));
}
