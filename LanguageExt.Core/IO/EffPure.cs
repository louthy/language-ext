using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Synchronous IO monad
    /// </summary>
    public struct EffPure<A>
    {
        internal Thunk<A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        internal EffPure(Thunk<A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<A> RunIO() =>
            thunk.Value();

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> EffectMaybe(Func<Fin<A>> f) =>
            new EffPure<A>(Thunk<A>.Lazy(f));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> Effect(Func<A> f) =>
            new EffPure<A>(Thunk<A>.Lazy(() => Fin<A>.Succ(f())));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> Success(A value) =>
            new EffPure<A>(Thunk<A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> Fail(Error error) =>
            new EffPure<A>(Thunk<A>.Fail(error));

        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> operator |(EffPure<A> ma, EffPure<A> mb) => new EffPure<A>(Thunk<A>.Lazy(
            () =>
            {
                var ra = ma.RunIO();
                return ra.IsSucc
                    ? ra
                    : mb.RunIO();
            }));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public Eff<Env, A> WithEnv<Env>()
        {
            var self = this;
            return Eff<Env, A>.EffectMaybe(e => self.RunIO());
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public Aff<Env, A> ToAsyncWithEnv<Env>() where Env : struct, HasCancel<Env>
        {
            var self = this;
            return Aff<Env, A>.EffectMaybe(e => new ValueTask<Fin<A>>(self.RunIO()));
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public S Fold<S>(S state, Func<S, A, S> f)
        {
            var r = RunIO();
            return r.IsSucc
                ? f(state, r.Value)
                : state;
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public bool Exists(Func<A, bool> f)
        {
            var r = RunIO();
            return r.IsSucc && f(r.Value);
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public bool ForAll(Func<A, bool> f)
        {
            var r = RunIO();
            return r.IsFail || (r.IsSucc && f(r.Value));
        }

        /// <summary>
        /// Clear the memoised value
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        public Unit Clear()
        {
            thunk = thunk.Clone();
            return default;
        }
    }
    
    public static partial class AffExtensions
    {
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> ToAsync<A>(this EffPure<A> ma) =>
            AffPure<A>.EffectMaybe(() => new ValueTask<Fin<A>>(ma.RunIO()));

        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<B> Map<A, B>(this EffPure<A> ma, Func<A, B> f) =>
            new EffPure<B>(ma.thunk.Map(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<B> BiMap<A, B>(this EffPure<A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
            new EffPure<B>(ma.thunk.BiMap(Succ, Fail));

        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<B> Match<A, B>(this EffPure<A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
            Eff(() => { 
                var r = ma.RunIO();
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> Filter<A>(this EffPure<A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Error.New(Thunk.CancelledText)));        

        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<B> Bind<A, B>(this EffPure<A> ma, Func<A, EffPure<B>> f) =>
            new EffPure<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> Bind<Env, A, B>(this EffPure<A> ma, Func<A, Eff<Env, B>> f) =>
            new Eff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> Bind<A, B>(this EffPure<A> ma, Func<A, AffPure<B>> f) =>
            new AffPure<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this EffPure<A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<B> BiBind<A, B>(this EffPure<A> ma, Func<A, EffPure<B>> Succ, Func<Error, EffPure<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> BiBind<Env, A, B>(this EffPure<A> ma, Func<A, Eff<Env, B>> Succ, Func<Error, Eff<Env, B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> BiBind<A, B>(this EffPure<A> ma, Func<A, AffPure<B>> Succ, Func<Error, AffPure<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this EffPure<A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
              .Flatten();

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> Flatten<A>(this EffPure<EffPure<A>> ma) =>
            new EffPure<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Flatten<Env, A>(this EffPure<Eff<Env, A>> ma) =>
            new Eff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> Flatten<A>(this EffPure<AffPure<A>> ma) =>
            new AffPure<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this EffPure<Aff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<B> Select<A, B>(this EffPure<A> ma, Func<A, B> f) =>
            Map(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<B> SelectMany<A, B>(this EffPure<A> ma, Func<A, EffPure<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> SelectMany<Env, A, B>(this EffPure<A> ma, Func<A, Eff<Env, B>> f)  =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> SelectMany<A, B>(this EffPure<A> ma, Func<A, AffPure<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this EffPure<A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
       
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<C> SelectMany<A, B, C>(this EffPure<A> ma, Func<A, EffPure<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, C> SelectMany<Env, A, B, C>(this EffPure<A> ma, Func<A, Eff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<C> SelectMany<A, B, C>(this EffPure<A> ma, Func<A, AffPure<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this EffPure<A> ma, Func<A, Aff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> Where<A>(this EffPure<A> ma, Func<A, bool> f) =>
            Filter(ma, f);
        
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<(A, B)> Zip<A, B>(EffPure<A> ma, EffPure<B> mb) =>
            new EffPure<(A, B)>(Thunk<(A, B)>.Lazy(() =>
            {
                var ta = ma.RunIO();
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.RunIO();
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(AffOpt.mops)]
        static Thunk<A> ThunkFromIO<A>(EffPure<A> ma) =>
            ma.thunk;
    }
}
