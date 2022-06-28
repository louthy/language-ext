using System;
using LanguageExt.Thunks;
using LanguageExt.Common;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public readonly struct AffCatch<A>
    {
        internal readonly Func<Error, Aff<A>> fail;

        public AffCatch(Func<Error, Aff<A>> fail) =>
            this.fail = fail;

        public AffCatch(Func<Error, bool> predicate, Func<Error, Aff<A>> fail) :
            this(e => predicate(e) ? fail(e) : FailEff<A>(e))
        { }

        public ValueTask<Fin<A>> Run(Error error) =>
            fail(error).Run();

        public static AffCatch<A> operator |(CatchValue<A> ma, AffCatch<A> mb) =>
            new (e => ma.Match(e) ? SuccessEff(ma.Value(e)) : mb.fail(e));

        public static AffCatch<A> operator |(CatchError ma, AffCatch<A> mb) =>
            new (e => ma.Match(e) ? FailEff<A>(ma.Value(e)) : mb.fail(e));

        public static AffCatch<A> operator |(AffCatch<A> ma, CatchValue<A> mb) =>
            new (e => ma.fail(e).MatchAff(Succ: SuccessAff,
                                          Fail: e => mb.Match(e) 
                                                       ? SuccessAff(mb.Value(e)) 
                                                       : FailAff<A>(e)));

        public static AffCatch<A> operator |(AffCatch<A> ma, CatchError mb) =>
            new (e => ma.fail(e).MatchAff(Succ: SuccessAff,
                                          Fail: e => mb.Match(e) 
                                                        ? FailAff<A>(mb.Value(e)) 
                                                        : FailAff<A>(e)));

        public static AffCatch<A> operator |(AffCatch<A> ma, AffCatch<A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static AffCatch<A> operator |(AffCatch<A> ma, EffCatch<A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static AffCatch<A> operator |(EffCatch<A> ma, AffCatch<A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));
    }

    public readonly struct AffCatch<RT, A> where RT : struct, HasCancel<RT>
    {
        internal readonly Func<Error, Aff<RT, A>> fail;

        public AffCatch(Func<Error, Aff<RT, A>> fail) =>
            this.fail = fail;

        public AffCatch(Func<Error, bool> predicate, Func<Error, Aff<RT, A>> fail) :
            this(e => predicate(e) ? fail(e) : FailEff<A>(e))
        { }

        public ValueTask<Fin<A>> Run(RT env, Error error) =>
            fail(error).Run(env);

        public static AffCatch<RT, A> operator |(CatchValue<A> ma, AffCatch<RT, A> mb) =>
            new (e => ma.Match(e) ? SuccessEff(ma.Value(e)) : mb.fail(e));

        public static AffCatch<RT, A> operator |(CatchError ma, AffCatch<RT, A> mb) =>
            new (e => ma.Match(e) ? FailEff<A>(ma.Value(e)) : mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, CatchValue<A> mb) =>
            new (e => ma.fail(e).MatchAff(Succ: SuccessAff<RT, A>,
                                          Fail: e => mb.Match(e) 
                                                       ? SuccessAff<RT, A>(mb.Value(e))
                                                       : FailAff<RT, A>(e)));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, CatchError mb) =>
            new (e => ma.fail(e).MatchAff(Succ: SuccessAff<RT, A>,
                                          Fail: e => mb.Match(e) 
                                                       ? FailAff<RT, A>(mb.Value(e))
                                                       : FailAff<RT, A>(e)));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, AffCatch<RT, A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, EffCatch<RT, A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, EffCatch<A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(EffCatch<RT, A> ma, AffCatch<RT, A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(EffCatch<A> ma, AffCatch<RT, A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<A> ma, AffCatch<RT, A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, AffCatch<A> mb) =>
            new (e => ma.fail(e) | mb.fail(e));

        public static Aff<RT, A> operator |(Aff<A> ma, AffCatch<RT, A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run().ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(env, ra.Error).ConfigureAwait(false);
            });

        public static Aff<RT, A> operator |(Eff<A> ma, AffCatch<RT, A> mb) =>
            new(async env =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : await mb.Run(env, ra.Error).ConfigureAwait(false);
            });

        public static Aff<RT, A> operator |(Eff<RT, A> ma, AffCatch<RT, A> mb) =>
            new(async env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(env, ra.Error).ConfigureAwait(false);
            });
    }
}
