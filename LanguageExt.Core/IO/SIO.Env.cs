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
    public struct SIO<Env, A>
    {
        internal readonly Thunk<Env, A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(IO.mops)]
        internal SIO(Thunk<Env, A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public Fin<A> RunIO(Env env) =>
            thunk.Value(env ?? throw new ArgumentNullException(nameof(env)));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [MethodImpl(IO.mops)]
        public Unit RunUnitIO(Env env) =>
            ignore(thunk.Value(env ?? throw new ArgumentNullException(nameof(env))));
 
        /// <summary>
        /// Lift an asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, Unit> Effect(Func<Env, ValueTask> f) =>
            new SIO<Env, Unit>(Thunk<Env, Unit>.Lazy(e =>
            {
                f(e);
                return Fin<Unit>.Succ(default);
            }));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> EffectMaybe(Func<Env, Fin<A>> f) =>
            new SIO<Env, A>(Thunk<Env, A>.Lazy(f));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Effect(Func<Env, A> f) =>
            new SIO<Env, A>(Thunk<Env, A>.Lazy(e => Fin<A>.Succ(f(e))));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Success(A value) =>
            new SIO<Env, A>(Thunk<Env, A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Fail(Error error) =>
            new SIO<Env, A>(Thunk<Env, A>.Fail(error));

        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> operator |(SIO<Env, A> ma, SIO<Env, A> mb) => new SIO<Env, A>(Thunk<Env, A>.Lazy(
            env =>
            {
                var ra = ma.RunIO(env);
                return ra.IsSucc
                    ? ra
                    : mb.RunIO(env);
            }));

        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> operator |(SIO<Env, A> ma, SIO<A> mb) => new SIO<Env, A>(Thunk<Env, A>.Lazy(
            e =>
            {
                var ra = ma.RunIO(e);
                return ra.IsSucc
                    ? ra
                    : mb.RunIO();
            }));

        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> operator |(SIO<A> ma, SIO<Env, A> mb) => new SIO<Env, A>(Thunk<Env, A>.Lazy(
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
        [MethodImpl(IO.mops)]
        public SIO<Env, A> Clear()
        {
            thunk.Flush();
            return this;
        }
    }
    
    public static partial class IOExtensions
    {
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> ToAsync<Env, A>(this SIO<Env, A> ma) where Env : Cancellable =>
            IO<Env, A>.EffectMaybe(e => new ValueTask<Fin<A>>(ma.RunIO(e)));

 
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, B> Map<Env, A, B>(this SIO<Env, A> ma, Func<A, B> f) =>
            new SIO<Env, B>(ma.thunk.Map(f));

        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Filter<Env, A>(this SIO<Env, A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SIO.SuccessIO<A>(x) : SIO.FailIO<A>(Error.New(Thunk.CancelledText)));        


        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, B> Bind<Env, A, B>(this SIO<Env, A> ma, Func<A, SIO<Env, B>> f) =>
            new SIO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, B> Bind<Env, A, B>(this SIO<Env, A> ma, Func<A, SIO<B>> f) =>
            new SIO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this SIO<Env, A> ma, Func<A, IO<Env, B>> f) where Env : Cancellable =>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this SIO<Env, A> ma, Func<A, IO<B>> f) where Env : Cancellable =>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Flatten<Env, A>(this SIO<Env, SIO<Env, A>> ma) =>
            new SIO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Flatten<Env, A>(this SIO<Env, SIO<A>> ma) =>
            new SIO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this SIO<Env, IO<Env, A>> ma) where Env : Cancellable =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this SIO<Env, IO<A>> ma) where Env : Cancellable =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());

        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, B> Select<Env, A, B>(this SIO<Env, A> ma, Func<A, B> f) =>
            Map(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, B> SelectMany<Env, A, B>(this SIO<Env, A> ma, Func<A, SIO<Env, B>> f)  =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, B> SelectMany<Env, A, B>(this SIO<Env, A> ma, Func<A, SIO<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this SIO<Env, A> ma, Func<A, IO<Env, B>> f) where Env : Cancellable =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this SIO<Env, A> ma, Func<A, IO<B>> f) where Env : Cancellable =>
            Bind(ma, f);
       
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, C> SelectMany<Env, A, B, C>(this SIO<Env, A> ma, Func<A, SIO<Env, B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, C> SelectMany<Env, A, B, C>(this SIO<Env, A> ma, Func<A, SIO<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this SIO<Env, A> ma, Func<A, IO<Env, B>> bind, Func<A, B, C> project) where Env : Cancellable =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this SIO<Env, A> ma, Func<A, IO<B>> bind, Func<A, B, C> project) where Env : Cancellable =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Where<Env, A>(this SIO<Env, A> ma, Func<A, bool> f) =>
            Filter(ma, f);

        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, (A, B)> Zip<Env, A, B>(SIO<Env, A> ma, SIO<Env, B> mb) =>
            new SIO<Env, (A, B)>(Thunk<Env, (A, B)>.Lazy(e =>
            {
                var ta = ma.RunIO(e);
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.RunIO(e);
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, (A, B)> Zip<Env, A, B>(SIO<A> ma, SIO<Env, B> mb) =>
            new SIO<Env, (A, B)>(Thunk<Env, (A, B)>.Lazy(e =>
            {
                var ta = ma.RunIO();
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.RunIO(e);
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, (A, B)> Zip<Env, A, B>(SIO<Env, A> ma, SIO<B> mb) =>
            new SIO<Env, (A, B)>(Thunk<Env, (A, B)>.Lazy(e =>
            {
                var ta = ma.RunIO(e);
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.RunIO();
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));           
       
        [Pure, MethodImpl(IO.mops)]
        static Thunk<Env, A> ThunkFromIO<Env, A>(SIO<Env, A> ma) =>
            ma.thunk;
    }    
}