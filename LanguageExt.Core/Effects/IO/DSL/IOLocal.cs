using System;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

abstract record IOLocal<A>(Func<EnvIO, EnvIO> MapEnvIO) : IO<A>
{
    public abstract IO<A> MakeOperation();
}

record IOLocal<X, A>(Func<EnvIO, EnvIO> MapEnvIO, K<IO, X> Operation, Func<X, K<IO, A>> Next) : IOLocal<A>(MapEnvIO) 
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOLocal<X, B>(MapEnvIO, Operation, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOLocal<X, B>(MapEnvIO, Operation, x => Next(x).Bind(f));

    public override IO<A> MakeOperation() =>
        Operation.As().Bind(x => new IOLocalRestore<A>(Next(x)));
}

record IOLocalOnFailOnly<X, A>(Func<EnvIO, EnvIO> MapEnvIO, K<IO, X> Operation, Func<X, K<IO, A>> Next) : IOLocal<A>(MapEnvIO) 
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOLocal<X, B>(MapEnvIO, Operation, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOLocal<X, B>(MapEnvIO, Operation, x => Next(x).Bind(f));

    public override IO<A> MakeOperation() =>
        Operation.As().Bind(Next);
}

record IOLocalRestore<A>(K<IO, A> Next) : IO<A> 
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOLocalRestore<B>(Next.Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOLocalRestore<B>(Next.Bind(f));
} 
