using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOPure<A>(A Value) : InvokeSync<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOPureMap<A, B, B>(f, Value, IO.pure);

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOBind<A, B>(Value, f);

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOBindAsync2<A, B>(new ValueTask<A>(Value), f);

    public override A Invoke(EnvIO envIO) => 
        Value;
    
    public override string ToString() => 
        $"pure({Value})";
}
