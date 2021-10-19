using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using LanguageExt.Pipes;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Synchronous IO monad
    /// </summary>
    public readonly struct Eff<RT, A>
        where RT : struct 
    {
        internal Thunk<RT, A> Thunk => thunk ?? Thunk<RT, A>.Fail(Errors.Bottom);
        readonly Thunk<RT, A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Eff(Thunk<RT, A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> Run(RT env) =>
            Thunk.Value(env);

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> ReRun(RT env) =>
            Thunk.ReValue(env);

        /// <summary>
        /// Clone the effect
        /// </summary>
        /// <remarks>
        /// If the effect had already run, then this state will be wiped in the clone, meaning it can be re-run
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public Eff<RT, A> Clone() =>
            new Eff<RT, A>(Thunk.Clone());        

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Throws on error
        /// </remarks>
        [MethodImpl(Opt.Default)]
        public Unit RunUnit(RT env) =>
            Thunk.Value(env).Case switch
            {
                A _     => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> EffectMaybe(Func<RT, Fin<A>> f) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(f));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Effect(Func<RT, A> f) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(e => Fin<A>.Succ(f(e))));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Success(A value) =>
            new Eff<RT, A>(Thunk<RT, A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Fail(Error error) =>
            new Eff<RT, A>(Thunk<RT, A>.Fail(error));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, Eff<RT, A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                env =>
                {
                    var ra = ma.ReRun(env);
                    return ra.IsSucc
                        ? ra
                        : mb.ReRun(env);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, Eff<A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                e =>
                {
                    var ra = ma.ReRun(e);
                    return ra.IsSucc
                        ? ra
                        : mb.ReRun();
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<A> ma, Eff<RT, A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                e =>
                {
                    var ra = ma.ReRun();
                    return ra.IsSucc
                        ? ra
                        : mb.ReRun(e);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, EffCatch<RT, A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                env =>
                {
                    var ra = ma.ReRun(env);
                    return ra.IsSucc
                        ? ra
                        : mb.Run(env, ra.Error);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, EffCatch<A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                env =>
                {
                    var ra = ma.ReRun(env);
                    return ra.IsSucc
                        ? ra
                        : mb.Run(ra.Error);
                }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchValue<A> value) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                                env =>
                                {
                                    var ra = ma.ReRun(env);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinSucc(value.Value(ra.Error))
                                                   : ra;
                                }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchError value) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                                env =>
                                {
                                    var ra = ma.ReRun(env);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinFail<A>(value.Value(ra.Error))
                                                   : ra;
                                }));

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Eff<RT, A>(Eff<A> ma) =>
            EffectMaybe(env => ma.ReRun());
    }
}
