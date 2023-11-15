#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //

    /// <summary>
    /// Lifts a lazy sequence into the IO monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> manyIO<RT, E, A>(IEnumerable<A> items)
        where RT : struct, HasIO<RT, E> =>
        new(Transducer.compose(
                Transducer.constant<RT, IEnumerable<A>>(items),
                Transducer.enumerable<A>(),
                Transducer.mkRight<E, A>()));

    /// <summary>
    /// Lifts a lazy sequence into the IO monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> manyIO<RT, E, A>(Seq<A> items)
        where RT : struct, HasIO<RT, E> =>
        new(Transducer.compose(
                Transducer.constant<RT, Seq<A>>(items),
                Transducer.seq<A>(),
                Transducer.mkRight<E, A>()));

    /// <summary>
    /// Lifts an asynchronous lazy sequence into the IO monad
    /// </summary>
    /// <param name="items">Items</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO monad that processes multiple items</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> manyIO<RT, E, A>(IAsyncEnumerable<A> items)
        where RT : struct, HasIO<RT, E> =>
        new(Transducer.compose(
                Transducer.constant<RT, IAsyncEnumerable<A>>(items),
                Transducer.asyncEnumerable<A>(),
                Transducer.mkRight<E, A>()));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic join
    //

    /// <summary>
    /// Monadic join operator 
    /// </summary>
    /// <remarks>
    /// Collapses a nested IO monad so there is no nesting.
    /// </remarks>
    /// <param name="mma">Nest IO monad to flatten</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Flattened IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> flatten<RT, E, A>(IO<RT, E, IO<RT, E, A>> mma)
        where RT : struct, HasIO<RT, E> =>
        new(mma.Thunk.Map(ma => ma.Map(r => r.Thunk)).Flatten());
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Zipping
    //

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, (A First, B Second)> zip<RT, E, A, B>(
         (IO<RT, E, A> First, IO<RT, E, B> Second) tuple)
         where RT : struct, HasIO<RT, E> =>
         new(Transducer.zip(tuple.First.Thunk, tuple.Second.Thunk));

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, (A First, B Second, C Third)> zip<RT, E, A, B, C>(
        (IO<RT, E, A> First, 
              IO<RT, E, B> Second, 
              IO<RT, E, C> Third) tuple)
        where RT : struct, HasIO<RT, E> =>
        new(Transducer.zip(tuple.First.Thunk, tuple.Second.Thunk, tuple.Third.Thunk));

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, (A First, B Second)> zip<RT, E, A, B>(
        IO<RT, E, A> First,
        IO<RT, E, B> Second)
        where RT : struct, HasIO<RT, E> =>
        (First, Second).Zip();

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, (A First, B Second, C Third)> zip<RT, E, A, B, C>(
        IO<RT, E, A> First, 
        IO<RT, E, B> Second, 
        IO<RT, E, C> Third)
        where RT : struct, HasIO<RT, E> =>
        (First, Second, Third).Zip();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Lifting
    //

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> liftIO<RT, E, A>(Func<RT, Either<E, A>> f)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> liftIO<RT, E, A>(Transducer<RT, Either<E, A>> f)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> liftIO<RT, E, A>(Func<RT, Sum<E, A>> f)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> liftIO<RT, E, A>(Transducer<RT, Sum<E, A>> f)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Lift(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> liftIO<RT, E, A>(Func<RT, Task<Sum<E, A>>> f)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.LiftIO(f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> liftIO<RT, E, A>(Func<RT, Task<Either<E, A>>> f)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.LiftIO(f);

    /// <summary>
    /// Memoise the result, so subsequent calls don't invoke the side-IOect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> memo<RT, E, A>(IO<RT, E, A> ma)
        where RT : struct, HasIO<RT, E> =>
        ma.Memo();
    
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
    public static IO<RT, E, B> map<RT, E, A, B>(IO<RT, E, A> ma, Func<A, B> f)
        where RT : struct, HasIO<RT, E> =>
        ma.Map(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> map<RT, E, A, B>(IO<RT, E, A> ma, Transducer<A, B> f)
        where RT : struct, HasIO<RT, E> =>
        ma.Map(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> mapFail<RT, E, A>(IO<RT, E, A> ma, Func<E, E> f)
        where RT : struct, HasIO<RT, E> =>
        ma.MapFail(f);

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> mapFail<RT, E, A>(IO<RT, E, A> ma, Transducer<E, E> f)
        where RT : struct, HasIO<RT, E> =>
        ma.MapFail(f);

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
    public static IO<RT, E, B> bimap<RT, E, A, B>(IO<RT, E, A> ma, Func<A, B> Succ, Func<E, E> Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.BiMap(Succ, Fail);

    /// <summary>
    /// Mapping of either the Success state or the Failure state depending on what
    /// state this IO monad is in.  
    /// </summary>
    /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
    /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> bimap<RT, E, A, B>(IO<RT, E, A> ma, Transducer<A, B> Succ, Transducer<E, E> Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.BiMap(Succ, Fail);

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
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> match<RT, E, A, B>(IO<RT, E, A> ma, Func<A, B> Succ, Func<E, B> Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.Match(Succ, Fail);

    /// <summary>
    /// Pattern match the success or failure values and collapse them down to a success value
    /// </summary>
    /// <param name="Succ">Success value mapping</param>
    /// <param name="Fail">Failure value mapping</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> match<RT, E, A, B>(IO<RT, E, A> ma, Transducer<A, B> Succ, Transducer<E, B> Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.Match(Succ, Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> ifFail<RT, E, A>(IO<RT, E, A> ma, Func<E, A> Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> ifFail<RT, E, A>(IO<RT, E, A> ma, Transducer<E, A> Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> ifFail<RT, E, A>(IO<RT, E, A> ma, A Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.IfFail(Fail);

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> ifFailIO<RT, E, A>(IO<RT, E, A> ma, Func<E, IO<RT, E, A>> Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.IfFailIO(Fail);

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> ifFailIO<RT, E, A>(IO<RT, E, A> ma, IO<RT, E, A> Fail)
        where RT : struct, HasIO<RT, E> =>
        ma.IfFailIO(Fail);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Filter
    //

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> filter<RT, E, A>(IO<RT, E, A> ma, Func<A, bool> predicate)
        where RT : struct, HasIO<RT, E> =>
        ma.Filter(predicate);

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> filter<RT, E, A>(IO<RT, E, A> ma, Transducer<A, bool> predicate)
        where RT : struct, HasIO<RT, E> =>
        ma.Filter(predicate);

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
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> bind<RT, E, A, B>(IO<RT, E, A> ma, Func<A, IO<RT, E, B>> f)
        where RT : struct, HasIO<RT, E> =>
        ma.Bind(f);

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// transducer provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the transducer provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> bind<RT, E, A, B>(IO<RT, E, A> ma, Transducer<A, IO<RT, E, B>> f)
        where RT : struct, HasIO<RT, E> =>
        ma.Bind(f);

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> bind<RT, E, A, B>(IO<RT, E, A> ma, Func<A, Pure<B>> f)
        where RT : struct, HasIO<RT, E> =>
        ma.Bind(f);

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> bind<RT, E, A>(IO<RT, E, A> ma, Func<A, Fail<E>> f)
        where RT : struct, HasIO<RT, E> =>
        ma.Bind(f);

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> bind<RT, E, A, B>(IO<RT, E, A> ma, Func<A, Use<B>> f) 
        where RT : struct, HasIO<RT, E> =>
        ma.Bind(f);

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, Unit> bind<RT, E, A, B>(IO<RT, E, A> ma, Func<A, Release<B>> f)
        where RT : struct, HasIO<RT, E> =>
        ma.Bind(f);

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, B> bind<RT, E, A, B>(IO<RT, E, A> ma, Func<A, LiftIO<B>> f)
        where RT : struct, HasIO<RT, E> =>
        ma.Bind(f);

    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Folding
    //

    /// <summary>
    /// Fold the effect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, S> fold<RT, E, A, S>(IO<RT, E, A> ma, S initialState, Func<S, A, S> folder)
        where RT : struct, HasIO<RT, E> =>
        ma.Fold(initialState, folder);

}
