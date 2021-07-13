using System;
using static LanguageExt.Prelude;
using LanguageExt.Common;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    public struct AffCatch<A>
    {
        internal readonly Func<Error, Aff<A>> fail;

        public AffCatch(Func<Error, Aff<A>> fail) =>
            this.fail = fail;

        public AffCatch(Func<Error, bool> predicate, Func<Error, Aff<A>> fail) :
            this(e => predicate(e) ? fail(e) : FailEff<A>(e))
        { }

        public ValueTask<Fin<A>> Run(Error error) =>
            fail(error).Run();

        public static AffCatch<A> operator |(AffCatch<A> ma, AffCatch<A> mb) =>
            new AffCatch<A>(e => ma.fail(e) | mb.fail(e));

        public static AffCatch<A> operator |(AffCatch<A> ma, EffCatch<A> mb) =>
            new AffCatch<A>(e => ma.fail(e) | mb.fail(e));

        public static AffCatch<A> operator |(EffCatch<A> ma, AffCatch<A> mb) =>
            new AffCatch<A>(e => ma.fail(e) | mb.fail(e));
    }

    public struct AffCatch<RT, A> where RT : struct, HasCancel<RT>
    {
        internal readonly Func<Error, Aff<RT, A>> fail;

        public AffCatch(Func<Error, Aff<RT, A>> fail) =>
            this.fail = fail;

        public AffCatch(Func<Error, bool> predicate, Func<Error, Aff<RT, A>> fail) :
            this(e => predicate(e) ? fail(e) : FailEff<A>(e))
        { }

        public ValueTask<Fin<A>> Run(RT env, Error error) =>
            fail(error).Run(env);

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, AffCatch<RT, A> mb) =>
            new AffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, EffCatch<RT, A> mb) =>
            new AffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, EffCatch<A> mb) =>
            new AffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(EffCatch<RT, A> ma, AffCatch<RT, A> mb) =>
            new AffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(EffCatch<A> ma, AffCatch<RT, A> mb) =>
            new AffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<A> ma, AffCatch<RT, A> mb) =>
            new AffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));

        public static AffCatch<RT, A> operator |(AffCatch<RT, A> ma, AffCatch<A> mb) =>
            new AffCatch<RT, A>(e => ma.fail(e) | mb.fail(e));
    }
}
