#nullable enable
using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt
{
    /// <summary>
    /// Synchronous IO monad
    /// </summary>
    public readonly struct IO<RT, E, A>
        where RT : struct, HasCancel<RT>
    {
        Transducer<RT, Sum<E, A>> Thunk => thunk ?? Transducer.Fail<RT, Sum<E, A>>(Errors.Bottom);
        readonly Transducer<RT, Sum<E, A>> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal IO(Transducer<RT, Sum<E, A>> thunk) =>
            this.thunk = thunk;

        [MethodImpl(Opt.Default)]
        internal IO(Func<RT, Sum<E, A>> thunk) =>
            this.thunk = Transducer.lift(thunk);

        [MethodImpl(Opt.Default)]
        internal IO(Func<RT, A> thunk) : this(rt => Sum<E, A>.Right(thunk(rt)))
        { }

        [MethodImpl(Opt.Default)]
        internal IO(Transducer<RT, A> thunk) 
            : this(Transducer.compose(thunk, Transducer.lift<A, Sum<E, A>>(x => Sum<E, A>.Right(x))))
        { }

        [MethodImpl(Opt.Default)]
        internal IO(Func<RT, Either<E, A>> thunk) : this(rt => thunk(rt).ToSum())
        { }

        [MethodImpl(Opt.Default)]
        internal IO(Transducer<RT, Either<E, A>> thunk) 
            : this(Transducer.compose(thunk, Transducer.lift<Either<E, A>, Sum<E, A>>(x => x.ToSum())))
        { }

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Either<E, A> Run(RT env, Func<Error, Either<E, A>>? errorMap = null)
        {
            return Thunk.Invoke1(env, default(RT).CancellationToken).ToEither(errorMap ?? Throw);
            static Either<E, A> Throw(Error e) => e.ToException().Rethrow<Either<E, A>>();
        }

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Throws on error
        /// </remarks>
        [MethodImpl(Opt.Default)]
        public Unit RunUnit(RT env)
        {
            return ignore(Run(env, Throw));
            static Either<E, A> Throw(Error e) => e.ToException().Rethrow<Either<E, A>>();
        }

        /// <summary>
        /// Memoise the result, so subsequent calls don't invoke the side-IOect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, A> Memo() =>
            new(Transducer.memoStream(Thunk));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> Lift(Func<RT, Either<E, A>> f) =>
            new (f);

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> Lift(Func<RT, Sum<E, A>> f) =>
            new (f);

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> Lift(Func<RT, A> f) =>
            new (f);

        /// <summary>
        /// Lift a asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> LiftIO(Func<RT, Task<A>> f) =>
            new (Transducer.liftIO<RT, A>((_, rt) => f(rt)));

        /// <summary>
        /// Lift a asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> LiftIO(Func<RT, Task<Sum<E, A>>> f) =>
            new (Transducer.liftIO<RT, Sum<E, A>>((_, rt) => f(rt)));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> Pure(A value) =>
            new (_ => Sum<E, A>.Right(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> Fail(E error) =>
            new (_ => Sum<E, A>.Left(error));

        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> operator |(IO<RT, E, A> ma, IO<RT, E, A> mb) =>
            new(Transducer.choice(ma.Thunk, mb.Thunk));

        /*
         
         TODO
         
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, A> operator |(IO<RT, E, A> ma, Eff<E, A> mb) =>
            new (e =>
            {
                var ra = ma.Run(e);
                return ra.IsSucc
                    ? ra
                    : mb.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, A> operator |(IO<A> ma, IO<RT, A> mb) =>
            new(e =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : mb.Run(e);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, A> operator |(IO<RT, A> ma, IOCatch<RT, A> mb) =>
            new(env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : mb.Run(env, ra.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, A> operator |(IO<RT, A> ma, IOCatch<A> mb) =>
            new(env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : mb.Run(ra.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, A> operator |(IO<RT, A> ma, CatchValue<A> value) =>
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
        public static IO<RT, A> operator |(IO<RT, A> ma, CatchError value) =>
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
        /// Implicit conversion from pure IO
        /// </summary>
        public static implicit operator IO<RT, E, A>(IO<E, A> ma) =>
            IOectMaybe(_ => ma.Run());
            */
        
        //
        // Map and map-left
        //

        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> Map<B>(Func<A, B> f) =>
            new(Transducer.mapRight(Thunk, Transducer.lift(f)));

        [Pure, MethodImpl(Opt.Default)]
        public  IO<RT, E, A> MapFail(Func<E, E> f) =>
            new(Transducer.mapLeft(Thunk, Transducer.lift(f)));

        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> MapAsync<B>(Func<CancellationToken, A, Task<B>> f) =>
            new(Transducer.mapRight(Thunk, Transducer.liftIO(f)));
        
        
        //
        // Bi-map
        //

        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> BiMap<B>(Func<A, B> Succ, Func<E, E> Fail) =>
            new(Transducer.bimap(Thunk, Transducer.lift(Fail), Transducer.lift(Succ)));
        

        //
        // Match
        //

        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> Match<B>(Func<A, B> Succ, Func<E, B> Fail) =>
            new(Transducer.compose(
                Transducer.bimap(Thunk, Transducer.lift(Fail), Transducer.lift(Succ)),
                Transducer.lift<Sum<B, B>, Sum<E, B>>(s => s switch
                {
                    SumRight<B, B> r => Sum<E, B>.Right(r.Value),
                    SumLeft<B, B> l => Sum<E, B>.Right(l.Value),
                    _ => throw new BottomException()
                })));



    }
}
