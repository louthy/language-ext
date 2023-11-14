#nullable enable
using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt
{
    /// <summary>
    /// Transducer based IO monad
    /// </summary>
    /// <typeparam name="RT">Runtime struct</typeparam>
    /// <typeparam name="E">Error value type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    public readonly struct IO<RT, E, A>
        where RT : struct, HasIO<RT, E>
    {
        static readonly Func<Error, Either<E, A>> errorMap = 
            e => default(RT).FromError(e); 
        
        readonly Transducer<RT, Sum<E, A>> thunk;
        internal Transducer<RT, Sum<E, A>> Thunk => 
            thunk ?? Transducer.Fail<RT, Sum<E, A>>(Errors.Bottom);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Constructors
        //
        
        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal IO(Transducer<RT, Sum<E, A>> thunk) =>
            this.thunk = thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        IO(Func<RT, Sum<E, A>> thunk) =>
            this.thunk = Transducer.lift(thunk);

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        IO(Func<RT, A> thunk) : this(rt => Sum<E, A>.Right(thunk(rt)))
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        IO(Transducer<RT, A> thunk) 
            : this(Transducer.compose(thunk, Transducer.lift<A, Sum<E, A>>(x => Sum<E, A>.Right(x))))
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        IO(Func<RT, Either<E, A>> thunk) : this(rt => thunk(rt).ToSum())
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        IO(Transducer<RT, Either<E, A>> thunk) 
            : this(Transducer.compose(thunk, Transducer.lift<Either<E, A>, Sum<E, A>>(x => x.ToSum())))
        { }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Invoking
        //
        
        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Either<E, A> Run(RT env) =>
            Thunk.Invoke1(env, default(RT).CancellationToken)
                 .ToEither(errorMap);

        /// <summary>
        /// Invoke the effect (which could produce multiple values).  Run a reducer for
        /// each value yielded.
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<S> RunMany<S>(RT env, S initialState, Func<S, Either<E, A>, TResult<S>> reducer) =>
            Thunk.Invoke(
                     env,
                     initialState,
                     Reducer.from<Sum<E, A>, S>(
                         (_, s, sv) => sv switch
                         {
                             SumRight<E, A> r => reducer(s, Either<E, A>.Right(r.Value)),
                             SumLeft<E, A> l => reducer(s, Either<E, A>.Left(l.Value)),
                             _ => TResult.Complete(s)
                         }),
                     default(RT).CancellationToken)
                 .ToFin();

        /// <summary>
        /// Invoke the effect (which could produce multiple values).  Collect those results in a `Seq`
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Either<E, Seq<A>> RunMany(RT env) =>
            RunMany(env,
                    Either<E, Seq<A>>.Right(Seq<A>()),
                    (s, v) =>
                        (s.IsRight, v.IsRight) switch
                        {
                            (true, true) => TResult.Continue(Either<E, Seq<A>>.Right(((Seq<A>)s).Add((A)v))),
                            (true, false) => TResult.Complete(Either<E, Seq<A>>.Left((E)v)),
                            _ => TResult.Complete(s),
                        })
                .Match(
                    Succ: v => v,
                    Fail: e => default(RT).FromError(e));

        /// <summary>
        /// Invoke the effect asynchronously
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Task<Either<E, A>> RunAsync(RT env) =>
            Thunk.Invoke1Async(env, default(RT).CancellationToken)
                 .Map(r => r.ToEither(errorMap));

        /// <summary>
        /// Invoke the effect (which could produce multiple values).  Run a reducer for
        /// each value yielded.
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Task<Fin<S>> RunManyAsync<S>(RT env, S initialState, Func<S, Either<E, A>, TResult<S>> reducer) =>
            Thunk.InvokeAsync(
                     env,
                     initialState,
                     Reducer.from<Sum<E, A>, S>(
                         (_, s, sv) => sv switch
                         {
                             SumRight<E, A> r => reducer(s, Either<E, A>.Right(r.Value)),
                             SumLeft<E, A> l => reducer(s, Either<E, A>.Left(l.Value)),
                             _ => TResult.Complete(s)
                         }),
                     default(RT).CancellationToken)
                 .Map(r => r.ToFin());

        /// <summary>
        /// Invoke the effect (which could produce multiple values).  Collect those results in a `Seq`
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Task<Either<E, Seq<A>>> RunManyAsync(RT env) =>
            RunManyAsync(env,
                    Either<E, Seq<A>>.Right(Seq<A>()),
                    (s, v) =>
                        (s.IsRight, v.IsRight) switch
                        {
                            (true, true) => TResult.Continue(Either<E, Seq<A>>.Right(((Seq<A>)s).Add((A)v))),
                            (true, false) => TResult.Complete(Either<E, Seq<A>>.Left((E)v)),
                            _ => TResult.Complete(s)
                        })
                .Map(r => r.Match(
                    Succ: v => v,
                    Fail: e => default(RT).FromError(e)));
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Lifting
        //
        
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
        public static IO<RT, E, A> Lift(Transducer<RT, Either<E, A>> f) =>
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
        public static IO<RT, E, A> Lift(Transducer<RT, Sum<E, A>> f) =>
            new (f);

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> Lift(Func<RT, A> f) =>
            new (f);

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> Lift(Transducer<RT, A> f) =>
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
        /// Memoise the result, so subsequent calls don't invoke the side-IOect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, A> Memo() =>
            new(Transducer.memo(Thunk));
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Map and map-left
        //

        /// <summary>
        /// Maps the IO monad if it's in a success state
        /// </summary>
        /// <param name="f">Function to map the success value with</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> Map<B>(Func<A, B> f) =>
            Map(Transducer.lift(f));

        /// <summary>
        /// Maps the IO monad if it's in a success state
        /// </summary>
        /// <param name="f">Function to map the success value with</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> Select<B>(Func<A, B> f) =>
            Map(Transducer.lift(f));

        /// <summary>
        /// Maps the IO monad if it's in a success state
        /// </summary>
        /// <param name="f">Function to map the success value with</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> Map<B>(Transducer<A, B> f) =>
            new(Transducer.mapRight(Thunk, f));

        /// <summary>
        /// Maps the IO monad if it's in a success state
        /// </summary>
        /// <param name="f">Function to map the success value with</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public  IO<RT, E, A> MapFail(Func<E, E> f) =>
            MapFail(Transducer.lift(f));

        /// <summary>
        /// Maps the IO monad if it's in a success state
        /// </summary>
        /// <param name="f">Function to map the success value with</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public  IO<RT, E, A> MapFail(Transducer<E, E> f) =>
            new(Transducer.mapLeft(Thunk, f));

        /// <summary>
        /// Maps the IO monad if it's in a success state
        /// </summary>
        /// <param name="f">Function to map the success value with</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> MapAsync<B>(Func<A, Task<B>> f) =>
            MapAsync((_, a) => f(a));

        /// <summary>
        /// Maps the IO monad if it's in a success state
        /// </summary>
        /// <param name="f">Function to map the success value with</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> MapAsync<B>(Func<CancellationToken, A, Task<B>> f) =>
            new(Transducer.mapRight(Thunk, Transducer.liftIO(f)));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Bi-map
        //

        /// <summary>
        /// Mapping of either the Success state or the Failure state depending on what
        /// state this IO monad is in.  
        /// </summary>
        /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
        /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> BiMap<B>(Func<A, B> Succ, Func<E, E> Fail) =>
            BiMap(Transducer.lift(Succ), Transducer.lift(Fail));

        /// <summary>
        /// Mapping of either the Success state or the Failure state depending on what
        /// state this IO monad is in.  
        /// </summary>
        /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
        /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
        /// <returns>Mapped IO monad</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, B> BiMap<B>(Transducer<A, B> Succ, Transducer<E, E> Fail) =>
            new(Transducer.bimap(Thunk, Fail, Succ));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Matching
        //

        /// <summary>
        /// Pattern match the success or failure values and collapse them down to a success value
        /// </summary>
        /// <param name="Succ">Success value mapping</param>
        /// <param name="Fail">Failure value mapping</param>
        /// <returns>IO in a success state</returns>
        [Pure]
        public IO<RT, E, B> Match<B>(Func<A, B> Succ, Func<E, B> Fail) =>
            Match(Transducer.lift(Succ), Transducer.lift(Fail));

        /// <summary>
        /// Pattern match the success or failure values and collapse them down to a success value
        /// </summary>
        /// <param name="Succ">Success value mapping</param>
        /// <param name="Fail">Failure value mapping</param>
        /// <returns>IO in a success state</returns>
        [Pure]
        public IO<RT, E, B> Match<B>(Transducer<A, B> Succ, Transducer<E, B> Fail) =>
            new(Transducer.compose(
                    Transducer.bimap(Thunk, Fail, Succ),
                    Transducer.lift<Sum<B, B>, Sum<E, B>>(s => s switch
                    {
                        SumRight<B, B> r => Sum<E, B>.Right(r.Value),
                        SumLeft<B, B> l => Sum<E, B>.Right(l.Value),
                        _ => throw new BottomException()
                    })));

        /// <summary>
        /// Map the failure to a success value
        /// </summary>
        /// <param name="f">Function to map the fail value</param>
        /// <returns>IO in a success state</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, A> IfFail(Func<E, A> Fail) =>
            IfFail(Transducer.lift(Fail));

        /// <summary>
        /// Map the failure to a success value
        /// </summary>
        /// <param name="f">Function to map the fail value</param>
        /// <returns>IO in a success state</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, A> IfFail(Transducer<E, A> Fail) =>
            Match(Transducer.identity<A>(), Fail);

        /// <summary>
        /// Map the failure to a success value
        /// </summary>
        /// <param name="f">Function to map the fail value</param>
        /// <returns>IO in a success state</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, A> IfFail(A Fail) =>
            Match(Transducer.identity<A>(), Transducer.constant<E, A>(Fail));

        /// <summary>
        /// Map the failure to a new IO effect
        /// </summary>
        /// <param name="f">Function to map the fail value</param>
        /// <returns>IO that encapsulates that IfFail</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, A> IfFailIO(Func<E, IO<RT, E, A>> Fail) =>
            new(Transducer.bimap(
                    Thunk,
                    Transducer.lift((E e) => Fail(e).Thunk),
                    Transducer.lift((A x) => Transducer.constant<RT, Sum<E, A>>(Sum<E, A>.Right(x))))
                .Flatten());

        /// <summary>
        /// Map the failure to a new IO effect
        /// </summary>
        /// <param name="f">Function to map the fail value</param>
        /// <returns>IO that encapsulates that IfFail</returns>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, A> IfFailIO(IO<RT, E, A> Fail) =>
            IfFailIO(_ => Fail);
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  Filter
        //

        /// <summary>
        /// Only allow values through the effect if the predicate returns `true` for the bound value
        /// </summary>
        /// <param name="predicate">Predicate to apply to the bound value></param>
        /// <returns>Filtered IO</returns>
        public IO<RT, E, A> Filter(Func<A, bool> predicate) =>
            Filter(Transducer.lift(predicate));

        /// <summary>
        /// Only allow values through the effect if the predicate returns `true` for the bound value
        /// </summary>
        /// <param name="predicate">Predicate to apply to the bound value></param>
        /// <returns>Filtered IO</returns>
        public IO<RT, E, A> Filter(Transducer<A, bool> predicate) =>
            new(Transducer.filter(Thunk, predicate));

        /// <summary>
        /// Only allow values through the effect if the predicate returns `true` for the bound value
        /// </summary>
        /// <param name="predicate">Predicate to apply to the bound value></param>
        /// <returns>Filtered IO</returns>
        public IO<RT, E, A> Where(Func<A, bool> predicate) =>
            Filter(Transducer.lift(predicate));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  Monadic binding
        //

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="f">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, B> Bind<B>(Func<A, IO<RT, E, B>> f) =>
            Map(f).Flatten();

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// transducer provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="f">Bind operation</param>
        /// <returns>Composition of this monad and the result of the transducer provided</returns>
        public IO<RT, E, B> Bind<B>(Transducer<A, IO<RT, E, B>> f) =>
            Map(f).Flatten();

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="f">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, B> Bind<B>(Func<A, Pure<B>> f) =>
            Map(x => f(x).ToIO<RT, E>()).Flatten();

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="f">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, B> Bind<B>(Func<A, Fail<E>> f) =>
            Map(x => f(x).ToIO<RT, B>()).Flatten();

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="f">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, B> Bind<B>(Func<A, Use<B>> f) =>
            Map(x => f(x).ToIO<RT, E>()).Flatten();

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="f">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, Unit> Bind<B>(Func<A, Release<B>> f) =>
            Map(x => f(x).ToIO<RT, E>()).Flatten();

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="f">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, B> Bind<B>(Func<A, LiftIO<B>> f) =>
            Map(x => f(x).ToIO<RT, E>()).Flatten();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  Monadic binding and projection
        //
        
        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="bind">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, C> SelectMany<B, C>(Func<A, IO<RT, E, B>> bind, Func<A, B, C> project) =>
            Bind(x => bind(x).Map(y => project(x, y)));

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="bind">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
            Bind(x => bind(x).ToIO<RT, E>().Map(y => project(x, y)));

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="bind">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, C> SelectMany<B, C>(Func<A, Fail<E>> bind, Func<A, B, C> project) =>
            Bind(x => bind(x).ToIO<RT, B>().Map(y => project(x, y)));

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="bind">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, C> SelectMany<B, C>(Func<A, Use<B>> bind, Func<A, B, C> project) =>
            Bind(x => bind(x).ToIO<RT, E>().Map(y => project(x, y)));

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="bind">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, Unit, C> project) =>
            Bind(x => bind(x).ToIO<RT, E>().Map(y => project(x, y)));

        /// <summary>
        /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
        /// function provided; which in turn returns a new IO monad.  This can be thought of as
        /// chaining IO operations sequentially.
        /// </summary>
        /// <param name="bind">Bind operation</param>
        /// <returns>Composition of this monad and the result of the function provided</returns>
        public IO<RT, E, C> SelectMany<B, C>(Func<A, LiftIO<B>> bind, Func<A, B, C> project) =>
            Bind(x => bind(x).ToIO<RT, E>().Map(y => project(x, y)));
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Folding
        //

        /// <summary>
        /// Fold the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public IO<RT, E, S> Fold<S>(S initialState, Func<S, A, S> folder) =>
            new(Transducer.fold(Thunk, initialState, folder));
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Operators
        //
        
        /// <summary>
        /// Run the first IO operation; if it fails, run the second.  Otherwise return the
        /// result of the first without running the second.
        /// </summary>
        /// <param name="ma">First IO operation</param>
        /// <param name="mb">Alternative IO operation</param>
        /// <returns>Result of either the first or second operation</returns>
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> operator |(IO<RT, E, A> ma, IO<RT, E, A> mb) =>
            new(Transducer.choice(ma.Thunk, mb.Thunk));

        /// <summary>
        /// Convert to an IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static implicit operator IO<RT, E, A>(Use<A> ma) =>
            ma.ToIO<RT, E>();

        /// <summary>
        /// Convert to an IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static implicit operator IO<RT, E, A>(Pure<A> ma) =>
            ma.ToIO<RT, E>();

        /// <summary>
        /// Convert to an IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static implicit operator IO<RT, E, A>(Fail<E> ma) =>
            ma.ToIO<RT, A>();

        /// <summary>
        /// Convert to an IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static implicit operator IO<RT, E, A>(LiftIO<A> ma) =>
            ma.ToIO<RT, E>();

        /*
         
         TODO
         
        [Pure, MethodImpl(Opt.Default)]
        public static IO<RT, E, A> operator |(IO<RT, E, A> ma, TransducerCatch<RT, E, A> mb) =>
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
            IOectMaybe(_ => ma.Run());*/

    }
}
