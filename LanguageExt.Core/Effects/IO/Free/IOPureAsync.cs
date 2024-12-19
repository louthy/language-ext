using System;
using System.Threading.Tasks;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOPureAsync<A>(Task<A> Value) : IO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        IO<B>.Lift(IODsl.MapAsync(Value, f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOBindAsync<A, B>(Value, f);
}
