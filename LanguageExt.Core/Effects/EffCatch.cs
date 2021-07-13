using System;
using static LanguageExt.Prelude;
using LanguageExt.Common;

namespace LanguageExt
{
    public struct EffCatch<A>
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
            new EffCatch<A>(e => ma.fail(e) | mb.fail(e));
    }

    public struct EffCatch<RT, A>
    {
        internal readonly Func<Error, Eff<RT, A>> fail;

        public EffCatch(Func<Error, Eff<RT, A>> fail) =>
            this.fail = fail;

        public EffCatch(Func<Error, bool> predicate, Func<Error, Eff<RT, A>> fail) :
            this(e => predicate(e) ? fail(e) : FailEff<A>(e))
        { }

        public Fin<A> Run(RT env, Error error) =>
            fail(error).Run(env);

        public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, EffCatch<RT, A> mb) =>
            new EffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));

        public static EffCatch<RT, A> operator |(EffCatch<A> ma, EffCatch<RT, A> mb) =>
            new EffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));

        public static EffCatch<RT, A> operator |(EffCatch<RT, A> ma, EffCatch<A> mb) =>
            new EffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));
    }
}
