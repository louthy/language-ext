/*
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects;
using static LanguageExt.Prelude;

namespace LanguageExt;

public readonly struct IOCatch<E, A>(Func<E, IO<E, A>> fail)
{
    public IOCatch(Func<E, bool> predicate, Func<E, IO<E, A>> fail) :
        this(e => predicate(e) ? fail(e) : Fail(e))
    { }

    public IOCatch<MinRT<E>, E, A> As =>
        new (Fail);
    
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Run(E error) =>
        fail(error);
        
    [Pure, MethodImpl(Opt.Default)]
    public static IOCatch<E, A> operator |(CatchValue<E, A> ma, IOCatch<E, A> mb) =>
        new (e => ma.Match(e) ? Pure(ma.Value(e)) : mb.Run(e));

    [Pure, MethodImpl(Opt.Default)]
    public static IOCatch<E, A> operator |(CatchError<E> ma, IOCatch<E, A> mb) =>
        new (e => ma.Match(e) ? Fail(ma.Value(e)) : mb.Run(e));

    [Pure, MethodImpl(Opt.Default)]
    public static IOCatch<E, A> operator |(IOCatch<E, A> ma, CatchValue<E, A> mb) =>
        new (e => 
                 ma.Run(e)
                   .Match<IO<E, A>>(
                        Succ: Pure,
                        Fail: e1 => mb.Match(e1) 
                                        ? Pure(mb.Value(e1)) 
                                        : Fail(e1)).Flatten());

    [Pure, MethodImpl(Opt.Default)]
    public static IOCatch<E, A> operator |(IOCatch<E, A> ma, CatchError<E> mb) =>
        new (e => 
                 ma.Run(e)
                   .Match<IO<E, A>>(
                        Succ: Pure,
                        Fail: e1 => mb.Match(e1) 
                                        ? Fail(mb.Value(e1)) 
                                        : Fail(e1)).Flatten());

    [Pure, MethodImpl(Opt.Default)]
    public static IOCatch<E, A> operator |(IOCatch<E, A> ma, IOCatch<E, A> mb) =>
        new (e => ma.Run(e) | mb.Run(e));
}
*/
