#nullable enable
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    public readonly struct IOCatch<RT, E, A> 
        where RT : HasIO<RT, E> 
    {
        readonly Func<E, IO<RT, E, A>> fail;

        public IOCatch(Func<E, IO<RT, E, A>> fail) =>
            this.fail = fail;

        public IOCatch(Func<E, bool> predicate, Func<E, IO<RT, E, A>> fail) :
            this(e => predicate(e) ? fail(e) : Fail(e))
        { }

        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, A> Run(E error) =>
            fail(error);
        
        [Pure, MethodImpl(Opt.Default)]
        public static IOCatch<RT, E, A> operator |(CatchValue<E, A> ma, IOCatch<RT, E, A> mb) =>
            new (e => ma.Match(e) ? Pure(ma.Value(e)) : mb.Run(e));

        [Pure, MethodImpl(Opt.Default)]
        public static IOCatch<RT, E, A> operator |(CatchError<E> ma, IOCatch<RT, E, A> mb) =>
            new (e => ma.Match(e) ? Fail(ma.Value(e)) : mb.Run(e));

        [Pure, MethodImpl(Opt.Default)]
        public static IOCatch<RT, E, A> operator |(IOCatch<RT, E, A> ma, CatchValue<E, A> mb) =>
            new (e => 
                ma.Run(e)
                  .Match<IO<RT, E, A>>(
                      Succ: x => Pure(x),
                      Fail: e1 => mb.Match(e1) 
                          ? Pure(mb.Value(e1)) 
                          : Fail(e1)).Flatten());

        [Pure, MethodImpl(Opt.Default)]
        public static IOCatch<RT, E, A> operator |(IOCatch<RT, E, A> ma, CatchError<E> mb) =>
            new (e => 
                ma.Run(e)
                    .Match<IO<RT, E, A>>(
                        Succ: x => Pure(x),
                        Fail: e1 => mb.Match(e1) 
                            ? Fail(mb.Value(e1)) 
                            : Fail(e1)).Flatten());

        [Pure, MethodImpl(Opt.Default)]
        public static IOCatch<RT, E, A> operator |(IOCatch<RT, E, A> ma, IOCatch<RT, E, A> mb) =>
            new (e => ma.Run(e) | mb.Run(e));
    }
}
