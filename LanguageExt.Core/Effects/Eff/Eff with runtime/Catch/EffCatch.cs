using System;
using LanguageExt.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public readonly struct EffCatch<RT, A>(Func<Error, Eff<RT, A>> fail)
    where RT : HasIO<RT, Error>
{
    public EffCatch(Func<Error, bool> predicate, Func<Error, Eff<RT, A>> fail) :
        this(e => predicate(e) ? fail(e) : Eff<RT, A>.Fail(e))
    { }

    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> Run(Error error) =>
        fail(error);
        
    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<RT, A> operator |(CatchValue<Error, A> ma, EffCatch<RT, A> mb) =>
        new (e => ma.Match(e) ? Pure(ma.Value(e)) : mb.Run(e));

    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<RT, A> operator |(CatchError<Error> ma, EffCatch<RT, A> mb) =>
        new (e => ma.Match(e) ? Fail(ma.Value(e)) : mb.Run(e));

    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, CatchValue<Error, A> mb) =>
        new (e => 
                 ma.Run(e)
                   .Match<Eff<RT, A>>(
                        Succ: Pure,
                        Fail: e1 => mb.Match(e1) 
                                        ? Pure(mb.Value(e1)) 
                                        : Fail(e1)).Flatten());

    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, CatchError<Error> mb) =>
        new (e => 
                 ma.Run(e)
                   .Match<Eff<RT, A>>(
                        Succ: Pure,
                        Fail: e1 => mb.Match(e1) 
                                        ? Fail(mb.Value(e1)) 
                                        : Fail(e1)).Flatten());

    [Pure, MethodImpl(Opt.Default)]
    public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, EffCatch<RT, A> mb) =>
        new (e => ma.Run(e) | mb.Run(e));
}
