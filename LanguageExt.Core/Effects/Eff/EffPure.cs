using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Synchronous IO monad
    /// </summary>
    public readonly struct Eff<A>
    {
        internal Func<Fin<A>> Thunk => thunk ?? (() => FinFail<A>(Errors.Bottom));
        readonly Func<Fin<A>> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Eff(Func<Fin<A>> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> Run()
        {
            try
            {
                return Thunk();
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
        public Unit RunUnit() =>
            Thunk().Case switch
            {
                A       => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

        /// <summary>
        /// Memoise the result, so subsequent calls don't invoke the side-effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Eff<A> Memo()
        {
            var thnk = Thunk;
            Fin<A> mr = default;

            return new Eff<A>(() =>
            {
                if (mr.IsSucc) return mr;
                mr = thnk();
                return mr;
            });
        }  
        
        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> EffectMaybe(Func<Fin<A>> f) =>
            new(f);

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Effect(Func<A> f) =>
            new(() => FinSucc(f()));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Success(A value) =>
            new (() => FinSucc(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Fail(Error error) =>
            new (() => FinFail<A>(error));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> operator |(Eff<A> ma, Eff<A> mb) =>
            new(() =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : mb.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> operator |(Eff<A> ma, EffCatch<A> mb) =>
            new(() =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : mb.Run(ra.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Eff<A> ma, AffCatch<A> mb) =>
            new(async () =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : await mb.Run(ra.Error).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> operator |(Eff<A> ma, CatchValue<A> value) =>
            new (() =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : value.Match(ra.Error)
                        ? FinSucc(value.Value(ra.Error))
                        : ra;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> operator |(Eff<A> ma, CatchError value) =>
            new(() =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : value.Match(ra.Error)
                        ? FinFail<A>(value.Value(ra.Error))
                        : ra;
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public Eff<RT, A> WithRuntime<RT>() where RT : struct 
        {
            var self = this;
            return Eff<RT, A>.EffectMaybe(_ => self.Run());
        }
        
        [Pure, MethodImpl(Opt.Default)]
        public Aff<A> ToAff() 
        {
            var self = this;
            return Aff<A>.EffectMaybe(() => new ValueTask<Fin<A>>(self.Run()));
        }
        
        [Pure, MethodImpl(Opt.Default)]
        public Aff<RT, A> ToAffWithRuntime<RT>() where RT : struct, HasCancel<RT>
        {
            var self = this;
            return Aff<RT, A>.EffectMaybe(e => new ValueTask<Fin<A>>(self.Run()));
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
            return r.IsFail || r.IsSucc && f(r.Value);
        }
    }
}
