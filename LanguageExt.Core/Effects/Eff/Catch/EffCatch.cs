using System;
using static LanguageExt.Prelude;
using LanguageExt.Common;
using LanguageExt.Thunks;

namespace LanguageExt
{
    public readonly struct EffCatch<A>
    {
        internal readonly Func<Error, Eff<A>> fail;

        public EffCatch(Func<Error, Eff<A>> fail) =>
            this.fail = fail;

        public EffCatch(Func<Error, bool> predicate, Func<Error, Eff<A>> fail) :
            this(e => predicate(e) ? fail(e) : FailEff<A>(e))
        { }

        public Fin<A> Run(Error error) =>
            fail(error).Run();

        public static EffCatch<A> operator |(EffCatch<A> ma, EffCatch<A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static EffCatch<A> operator |(CatchValue<A> ma, EffCatch<A> mb) =>
            new (e => ma.Match(e) ? SuccessEff(ma.Value(e)) : mb.fail(e));

        public static EffCatch<A> operator |(CatchError ma, EffCatch<A> mb) =>
            new (e => ma.Match(e) ? FailEff<A>(ma.Value(e)) : mb.fail(e));

        public static EffCatch<A> operator |(EffCatch<A> ma, CatchValue<A> mb) =>
            new (e => ma.fail(e).MatchEff(Succ: SuccessEff,
                                                     Fail: e => mb.Match(e) 
                                                                    ? SuccessEff(mb.Value(e)) 
                                                                    : FailEff<A>(e)));

        public static EffCatch<A> operator |(EffCatch<A> ma, CatchError mb) =>
            new (e => ma.fail(e).MatchEff(Succ: SuccessEff,
                                          Fail: e => mb.Match(e) 
                                                         ? FailEff<A>(mb.Value(e)) 
                                                         : FailEff<A>(e)));
    }

    public readonly struct EffCatch<RT, A> where RT : struct 
    {
        internal readonly Func<Error, Eff<RT, A>> fail;

        public EffCatch(Func<Error, Eff<RT, A>> fail) =>
            this.fail = fail;

        public EffCatch(Func<Error, bool> predicate, Func<Error, Eff<RT, A>> fail) :
            this(e => predicate(e) ? fail(e) : FailEff<A>(e))
        { }

        public Fin<A> Run(RT env, Error error) =>
            fail(error).Run(env);
        
        public static EffCatch<RT, A> operator |(CatchValue<A> ma, EffCatch<RT, A> mb) =>
            new (e => ma.Match(e) ? SuccessEff(ma.Value(e)) : mb.fail(e));

        public static EffCatch<RT, A> operator |(CatchError ma, EffCatch<RT, A> mb) =>
            new (e => ma.Match(e) ? FailEff<A>(ma.Value(e)) : mb.fail(e));

        public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, CatchValue<A> mb) =>
            new (e => ma.fail(e).MatchEff(Succ: SuccessEff<RT, A>,
                                          Fail: e => mb.Match(e) 
                                                         ? SuccessEff<RT, A>(mb.Value(e))
                                                         : FailEff<RT, A>(e)));

        public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, CatchError mb) =>
            new (e => ma.fail(e).MatchEff(Succ: SuccessEff<RT, A>,
                                          Fail: e => mb.Match(e) 
                                                         ? FailEff<RT, A>(mb.Value(e))
                                                         : FailEff<RT, A>(e)));

        public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, EffCatch<RT, A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static EffCatch<RT, A> operator |(EffCatch<A> ma, EffCatch<RT, A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, EffCatch<A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static Eff<RT, A> operator |(Eff<A> ma, EffCatch<RT, A> mb) =>
            new(env =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : mb.Run(env, ra.Error);
            });
    }
}
