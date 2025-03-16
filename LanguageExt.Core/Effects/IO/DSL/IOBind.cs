using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOBind<A, B>(A Value, Func<A, K<IO, B>> F) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOBindMap<A, B, C>(Value, F, f);

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOBindBind<A, B, C>(Value, F, f);

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) =>
        new IOBindBindAsync2<A, B, C>(Value.AsValueTask(), x => F(x).AsValueTask(), f);

    public override IO<B> Invoke(EnvIO envIO) =>
        F(Value).As();
    
    public override string ToString() => 
        "IO bind";
}
