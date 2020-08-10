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
    public struct SIO<A>
    {
        internal Thunk<A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(IO.mops)]
        internal SIO(Thunk<A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public Fin<A> RunIO() =>
            thunk.Value();

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> EffectMaybe(Func<Fin<A>> f) =>
            new SIO<A>(Thunk<A>.Lazy(f));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> Effect(Func<A> f) =>
            new SIO<A>(Thunk<A>.Lazy(() => Fin<A>.Succ(f())));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> Success(A value) =>
            new SIO<A>(Thunk<A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> Fail(Error error) =>
            new SIO<A>(Thunk<A>.Fail(error));

        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> operator |(SIO<A> ma, SIO<A> mb) => new SIO<A>(Thunk<A>.Lazy(
            () =>
            {
                var ra = ma.RunIO();
                return ra.IsSucc
                    ? ra
                    : mb.RunIO();
            }));
        
        [Pure, MethodImpl(IO.mops)]
        public SIO<Env, A> WithEnv<Env>()
        {
            var self = this;
            return SIO<Env, A>.EffectMaybe(e => self.RunIO());
        }

        [Pure, MethodImpl(IO.mops)]
        public IO<Env, A> ToAsyncWithEnv<Env>() where Env : struct, HasCancel<Env>
        {
            var self = this;
            return IO<Env, A>.EffectMaybe(e => new ValueTask<Fin<A>>(self.RunIO()));
        }

        [Pure, MethodImpl(IO.mops)]
        public S Fold<S>(S state, Func<S, A, S> f)
        {
            var r = RunIO();
            return r.IsSucc
                ? f(state, r.Value)
                : state;
        }

        [Pure, MethodImpl(IO.mops)]
        public bool Exists(Func<A, bool> f)
        {
            var r = RunIO();
            return r.IsSucc && f(r.Value);
        }

        [Pure, MethodImpl(IO.mops)]
        public bool ForAll(Func<A, bool> f)
        {
            var r = RunIO();
            return r.IsFail || (r.IsSucc && f(r.Value));
        }

        /// <summary>
        /// Clear the memoised value
        /// </summary>
        [MethodImpl(IO.mops)]
        public SIO<A> Clear()
        {
            thunk = thunk.Clone();
            return this;
        }
    }
    
    public static partial class IOExtensions
    {
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> ToAsync<A>(this SIO<A> ma) =>
            IO<A>.EffectMaybe(() => new ValueTask<Fin<A>>(ma.RunIO()));

        [Pure, MethodImpl(IO.mops)]
        public static SIO<B> Map<A, B>(this SIO<A> ma, Func<A, B> f) =>
            new SIO<B>(ma.thunk.Map(f));

        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> Filter<A>(this SIO<A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? IO.SuccessIO<A>(x) : IO.FailIO<A>(Error.New(Thunk.CancelledText)));        

        [Pure, MethodImpl(IO.mops)]
        public static SIO<B> Bind<A, B>(this SIO<A> ma, Func<A, SIO<B>> f) =>
            new SIO<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, B> Bind<Env, A, B>(this SIO<A> ma, Func<A, SIO<Env, B>> f) =>
            new SIO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<B> Bind<A, B>(this SIO<A> ma, Func<A, IO<B>> f) =>
            new IO<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this SIO<A> ma, Func<A, IO<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> Flatten<A>(this SIO<SIO<A>> ma) =>
            new SIO<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Flatten<Env, A>(this SIO<SIO<Env, A>> ma) =>
            new SIO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> Flatten<A>(this SIO<IO<A>> ma) =>
            new IO<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this SIO<IO<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());

        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<B> Select<A, B>(this SIO<A> ma, Func<A, B> f) =>
            Map(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<B> SelectMany<A, B>(this SIO<A> ma, Func<A, SIO<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, B> SelectMany<Env, A, B>(this SIO<A> ma, Func<A, SIO<Env, B>> f)  =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<B> SelectMany<A, B>(this SIO<A> ma, Func<A, IO<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this SIO<A> ma, Func<A, IO<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
       
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<C> SelectMany<A, B, C>(this SIO<A> ma, Func<A, SIO<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, C> SelectMany<Env, A, B, C>(this SIO<A> ma, Func<A, SIO<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<C> SelectMany<A, B, C>(this SIO<A> ma, Func<A, IO<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this SIO<A> ma, Func<A, IO<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> Where<A>(this SIO<A> ma, Func<A, bool> f) =>
            Filter(ma, f);
        
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<(A, B)> Zip<A, B>(SIO<A> ma, SIO<B> mb) =>
            new SIO<(A, B)>(Thunk<(A, B)>.Lazy(() =>
            {
                var ta = ma.RunIO();
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.RunIO();
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(IO.mops)]
        static Thunk<A> ThunkFromIO<A>(SIO<A> ma) =>
            ma.thunk;
    }
}
