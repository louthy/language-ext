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
    public struct Eff<Env, A>
    {
        internal Thunk<Env, A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        internal Eff(Thunk<Env, A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<A> RunIO(Env env) =>
            thunk.Value(env);

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        public Unit RunUnitIO(Env env) =>
            ignore(thunk.Value(env));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> EffectMaybe(Func<Env, Fin<A>> f) =>
            new Eff<Env, A>(Thunk<Env, A>.Lazy(f));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Effect(Func<Env, A> f) =>
            new Eff<Env, A>(Thunk<Env, A>.Lazy(e => Fin<A>.Succ(f(e))));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Success(A value) =>
            new Eff<Env, A>(Thunk<Env, A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Fail(Error error) =>
            new Eff<Env, A>(Thunk<Env, A>.Fail(error));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> operator |(Eff<Env, A> ma, Eff<Env, A> mb) =>
            new Eff<Env, A>(Thunk<Env, A>.Lazy(
                env =>
                {
                    var ra = ma.RunIO(env);
                    return ra.IsSucc
                        ? ra
                        : mb.RunIO(env);
                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> operator |(Eff<Env, A> ma, LanguageExt.EffPure<A> mb) =>
            new Eff<Env, A>(Thunk<Env, A>.Lazy(
                e =>
                {
                    var ra = ma.RunIO(e);
                    return ra.IsSucc
                        ? ra
                        : mb.RunIO();
                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> operator |(LanguageExt.EffPure<A> ma, Eff<Env, A> mb) =>
            new Eff<Env, A>(Thunk<Env, A>.Lazy(
                e =>
                {
                    var ra = ma.RunIO();
                    return ra.IsSucc
                        ? ra
                        : mb.RunIO(e);
                }));


        /// <summary>
        /// Clear the memoised value
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        public Unit Clear()
        {
            thunk = thunk.Clone();
            return default;
        }

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Eff<Env, A>(EffPure<A> ma) =>
            EffectMaybe(env => ma.RunIO());
    }

    public static partial class AffExtensions
    {
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> ToAsync<Env, A>(this Eff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            Aff<Env, A>.EffectMaybe(e => new ValueTask<Fin<A>>(ma.RunIO(e)));

 
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> Map<Env, A, B>(this Eff<Env, A> ma, Func<A, B> f) =>
            new Eff<Env, B>(ma.thunk.Map(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> MapFail<Env, A>(this Eff<Env, A> ma, Func<Error, Error> f) =>
            ma.BiMap(identity, f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> BiMap<Env, A, B>(this Eff<Env, A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
            new Eff<Env, B>(ma.thunk.BiMap(Succ, Fail));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> Match<Env, A, B>(this Eff<Env, A> ma, Func<A, B> Succ, Func<Error, B> Fail) where Env : struct, HasCancel<Env> =>
            Eff<Env, B>(env => { 
            
                var r = ma.RunIO(env);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Filter<Env, A>(this Eff<Env, A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Error.New(Thunk.CancelledText)));        


        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> Bind<Env, A, B>(this Eff<Env, A> ma, Func<A, Eff<Env, B>> f) =>
            new Eff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> Bind<Env, A, B>(this Eff<Env, A> ma, Func<A, EffPure<B>> f) =>
            new Eff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Eff<Env, A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Eff<Env, A> ma, Func<A, AffPure<B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Flatten<Env, A>(this Eff<Env, Eff<Env, A>> ma) =>
            new Eff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Flatten<Env, A>(this Eff<Env, EffPure<A>> ma) =>
            new Eff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Eff<Env, Aff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Eff<Env, AffPure<A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> Select<Env, A, B>(this Eff<Env, A> ma, Func<A, B> f) =>
            Map(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> SelectMany<Env, A, B>(this Eff<Env, A> ma, Func<A, Eff<Env, B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> SelectMany<Env, A, B>(this Eff<Env, A> ma, Func<A, EffPure<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Eff<Env, A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Eff<Env, A> ma, Func<A, AffPure<B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
       
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, C> SelectMany<Env, A, B, C>(this Eff<Env, A> ma, Func<A, Eff<Env, B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, C> SelectMany<Env, A, B, C>(this Eff<Env, A> ma, Func<A, EffPure<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Eff<Env, A> ma, Func<A, Aff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Eff<Env, A> ma, Func<A, AffPure<B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Where<Env, A>(this Eff<Env, A> ma, Func<A, bool> f) =>
            Filter(ma, f);

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, (A, B)> Zip<Env, A, B>(Eff<Env, A> ma, Eff<Env, B> mb) =>
            new Eff<Env, (A, B)>(Thunk<Env, (A, B)>.Lazy(e =>
            {
                var ta = ma.RunIO(e);
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.RunIO(e);
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, (A, B)> Zip<Env, A, B>(EffPure<A> ma, Eff<Env, B> mb) =>
            new Eff<Env, (A, B)>(Thunk<Env, (A, B)>.Lazy(e =>
            {
                var ta = ma.RunIO();
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.RunIO(e);
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, (A, B)> Zip<Env, A, B>(Eff<Env, A> ma, EffPure<B> mb) =>
            new Eff<Env, (A, B)>(Thunk<Env, (A, B)>.Lazy(e =>
            {
                var ta = ma.RunIO(e);
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.RunIO();
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));           
       
        [Pure, MethodImpl(AffOpt.mops)]
        static Thunk<Env, A> ThunkFromIO<Env, A>(Eff<Env, A> ma) =>
            ma.thunk;
    }    
}
