using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.DSL;

static class IODsl
{
    public static IODsl<A> Fail<A>(Error error) => 
        new IOFail<A>(error);
    
    public static IODsl<A> Lift<A>(Func<EnvIO, A> f) => 
        new IOLiftSync<A>(f);
    
    public static IODsl<A> Lift<A>(Func<EnvIO, Task<A>> f) => 
        new IOLiftAsync<A>(f);

    public static IODsl<B> Map<A, B>(A value, Func<A, B> f) =>
        new IOMap<A, B>(value, f);

    public static IODsl<B> MapAsync<A, B>(Task<A> value, Func<A, B> f) =>
        new IOMapAsync<A, B>(value, f);
    
    public static IODsl<B> Apply<A, B>(IO<Func<A, B>> ff, IO<A> fa) => 
        Apply(ff, fa, identity);
    
    public static IODsl<C> Apply<A, B, C>(IO<Func<A, B>> ff, IO<A> fa, Func<B, C> next) => 
        new IOApply<A, B, C>(ff, fa, next);
}

abstract record IODsl<A>
{
    public abstract IODsl<B> Map<B>(Func<A, B> f);
    public virtual bool IsAsync => false;
}

abstract record IODslAsync<A> : IODsl<A>
{
    public override bool IsAsync => true;
}
