using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Synchronous IO monad
    /// </summary>
    public readonly struct Eff<RT, A>
        where RT : struct 
    {
        internal Func<RT, Fin<A>> Thunk => thunk ?? (_ => FinFail<A>(Errors.Bottom));
        readonly Func<RT, Fin<A>> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Eff(Func<RT, Fin<A>> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> Run(RT env)
        {
            try
            {
                return Thunk(env);
            }
            catch (Exception e)
            {
                return FinFail<A>(e);
            }
        }

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Throws on error
        /// </remarks>
        [MethodImpl(Opt.Default)]
        public Unit RunUnit(RT env) =>
            Thunk(env).Case switch
            {
                A       => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

        /// <summary>
        /// Memoise the result, so subsequent calls don't invoke the side-effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Eff<RT, A> Memo()
        {
            var thnk = Thunk;
            Fin<A> mr = default;

            return new Eff<RT, A>(rt =>
            {
                if (mr.IsSucc) return mr;
                mr = thnk(rt);
                return mr;
            });
        }        

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> EffectMaybe(Func<RT, Fin<A>> f) =>
            new (f);

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Effect(Func<RT, A> f) =>
            new (rt => FinSucc(f(rt)));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Success(A value) =>
            new (_ => FinSucc(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Fail(Error error) =>
            new (_ => FinFail<A>(error));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, Eff<RT, A> mb) =>
            new(env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : mb.Run(env);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, Eff<A> mb) =>
            new (e =>
            {
                var ra = ma.Run(e);
                return ra.IsSucc
                    ? ra
                    : mb.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<A> ma, Eff<RT, A> mb) =>
            new(e =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : mb.Run(e);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, EffCatch<RT, A> mb) =>
            new(env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : mb.Run(env, ra.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, EffCatch<A> mb) =>
            new(env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : mb.Run(ra.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchValue<A> value) =>
            new(env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : value.Match(ra.Error)
                        ? FinSucc(value.Value(ra.Error))
                        : ra;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchError value) =>
            new(env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : value.Match(ra.Error)
                        ? FinFail<A>(value.Value(ra.Error))
                        : ra;
            });

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Eff<RT, A>(Eff<A> ma) =>
            EffectMaybe(_ => ma.Run());
    }
}
