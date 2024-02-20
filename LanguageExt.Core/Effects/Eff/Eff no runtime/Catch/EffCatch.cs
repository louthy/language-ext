using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public readonly struct EffCatch<A> 
{
    readonly Func<Error, Eff<A>> fail;

    public EffCatch(Func<Error, Eff<A>> fail) =>
        this.fail = fail;

    public EffCatch(Func<Error, bool> predicate, Func<Error, Eff<A>> fail) :
        this(e => predicate(e) ? fail(e) : Eff<A>.Fail(e))
    { }

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
