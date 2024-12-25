using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOPureAsync<A>(Task<A> Value) : InvokeAsync<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOPureMapAsync<A, B, B>(f, Value, IO.pure);

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOBindAsync<A, B>(Value, f);

    public override async ValueTask<A> Invoke(EnvIO envIO) => 
        await Value;
    
    public override string ToString() => 
        "IO pure async";
}
