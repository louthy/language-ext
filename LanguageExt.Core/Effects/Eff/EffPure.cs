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
    public readonly struct Eff<A>
    {
        internal Thunk<A> Thunk => thunk ?? Thunk<A>.Fail(Errors.Bottom);
        readonly Thunk<A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Eff(Thunk<A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> Run() =>
            Thunk.Value();

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> ReRun() =>
            Thunk.ReValue();

        /// <summary>
        /// Clone the effect
        /// </summary>
        /// <remarks>
        /// If the effect had already run, then this state will be wiped in the clone, meaning it can be re-run
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public Eff<A> Clone() =>
            new Eff<A>(Thunk.Clone());        

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Throws on error
        /// </remarks>
        [MethodImpl(Opt.Default)]
        public Unit RunUnit() =>
            Thunk.Value().Case switch
            {
                A _     => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> EffectMaybe(Func<Fin<A>> f) =>
            new Eff<A>(Thunk<A>.Lazy(f));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Effect(Func<A> f) =>
            new Eff<A>(Thunk<A>.Lazy(() => Fin<A>.Succ(f())));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Success(A value) =>
            new Eff<A>(Thunk<A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Fail(Error error) =>
            new Eff<A>(Thunk<A>.Fail(error));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> operator |(Eff<A> ma, Eff<A> mb) => new Eff<A>(Thunk<A>.Lazy(
            () =>
            {
                var ra = ma.ReRun();
                return ra.IsSucc
                    ? ra
                    : mb.ReRun();
            }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> operator |(Eff<A> ma, EffCatch<A> mb) =>
            new Eff<A>(Thunk<A>.Lazy(
                () =>
                {
                    var ra = ma.ReRun();
                    return ra.IsSucc
                        ? ra
                        : mb.Run(ra.Error);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Eff<A> ma, AffCatch<A> mb) =>
            new Aff<A>(ThunkAsync<A>.Lazy(
                async () =>
                {
                    var ra = ma.ReRun();
                    return ra.IsSucc
                        ? ra
                        : await mb.Run(ra.Error).ConfigureAwait(false);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> operator |(Eff<A> ma, CatchValue<A> value) =>
            new Eff<A>(Thunk<A>.Lazy(
                           () =>
                           {
                               var ra = ma.ReRun();
                               return ra.IsSucc
                                          ? ra
                                          : value.Match(ra.Error)
                                              ? FinSucc(value.Value(ra.Error))
                                              : ra;
                           }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> operator |(Eff<A> ma, CatchError value) =>
            new Eff<A>(Thunk<A>.Lazy(
                           () =>
                           {
                               var ra = ma.ReRun();
                               return ra.IsSucc
                                          ? ra
                                          : value.Match(ra.Error)
                                              ? FinFail<A>(value.Value(ra.Error))
                                              : ra;
                           }));
        
        [Pure, MethodImpl(Opt.Default)]
        public Eff<RT, A> WithRuntime<RT>() where RT : struct 
        {
            var self = this;
            return Eff<RT, A>.EffectMaybe(e => self.ReRun());
        }
        
        [Pure, MethodImpl(Opt.Default)]
        public Aff<A> ToAff() 
        {
            var self = this;
            return Aff<A>.EffectMaybe(() => new ValueTask<Fin<A>>(self.ReRun()));
        }
        
        [Pure, MethodImpl(Opt.Default)]
        public Aff<RT, A> ToAffWithRuntime<RT>() where RT : struct, HasCancel<RT>
        {
            var self = this;
            return Aff<RT, A>.EffectMaybe(e => new ValueTask<Fin<A>>(self.ReRun()));
        }

        [Pure, MethodImpl(Opt.Default)]
        public S Fold<S>(S state, Func<S, A, S> f)
        {
            var r = Run();
            return r.IsSucc
                ? f(state, r.Value)
                : state;
        }

        [Pure, MethodImpl(Opt.Default)]
        public bool Exists(Func<A, bool> f)
        {
            var r = Run();
            return r.IsSucc && f(r.Value);
        }

        [Pure, MethodImpl(Opt.Default)]
        public bool ForAll(Func<A, bool> f)
        {
            var r = Run();
            return r.IsFail || (r.IsSucc && f(r.Value));
        }
    }
}
