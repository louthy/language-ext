using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Effects;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public readonly struct EffCatch<A> 
{
    readonly Func<Error, Eff<A>> fail;

    public EffCatch(Func<Error, Eff<A>> fail) =>
        this.fail = fail;

    public EffCatch(Func<Error, bool> predicate, Func<Error, Eff<A>> fail) :
        this(e => predicate(e) ? fail(e) : Eff<A>.Fail(e))
    { }

    /*
    public IOCatch<MinRT, Error, A> As()
    {
        var f = fail;
        return new IOCatch<MinRT, Error, A>(e => f(e).Morphism);
    }

    public IOCatch<RT, Error, A> As<RT>() where RT : HasIO<RT, Error>
    {
        var f = fail;
        return new IOCatch<RT, Error, A>(e => Transducer.compose(MinRT.convert<RT>(), f(e).Morphism));
    }
    */

    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> Run(Error error) =>
        fail(error);
        
    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<A> operator |(CatchValue<Error, A> ma, EffCatch<A> mb) =>
        new (e => ma.Match(e) ? Pure(ma.Value(e)) : mb.Run(e));

    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<A> operator |(CatchError<Error> ma, EffCatch<A> mb) =>
        new (e => ma.Match(e) ? Fail(ma.Value(e)) : mb.Run(e));

    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<A> operator |(EffCatch<A> ma, CatchValue<Error, A> mb) =>
        new (e => 
                 ma.Run(e)
                   .Match<Eff<A>>(
                        Succ: x => Pure(x),
                        Fail: e1 => mb.Match(e1) 
                                        ? Pure(mb.Value(e1)) 
                                        : Fail(e1)).Flatten());

    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<A> operator |(EffCatch<A> ma, CatchError<Error> mb) =>
        new (e => 
                 ma.Run(e)
                   .Match<Eff<A>>(
                        Succ: x => Pure(x),
                        Fail: e1 => mb.Match(e1) 
                                        ? Fail(mb.Value(e1)) 
                                        : Fail(e1)).Flatten());

    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<A> operator |(EffCatch<A> ma, EffCatch<A> mb) =>
        new (e => ma.Run(e) | mb.Run(e));
}
