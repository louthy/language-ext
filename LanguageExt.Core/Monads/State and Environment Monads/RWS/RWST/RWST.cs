using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Reader / Write / State monad-transformer
/// </summary>
/// <typeparam name="R">Reader environment type</typeparam>
/// <typeparam name="W">Writer output type</typeparam>
/// <typeparam name="S">State type</typeparam>
/// <typeparam name="M">Lifted monad type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record RWST<R, W, S, M, A>(Func<(R Env, W Output, S State), K<M, (A Value, W Output, S State)>> runRWST): 
    K<RWST<R, W, S, M>, A>
    where M : Monad<M>
    where W : Monoid<W>
{
    public static RWST<R, W, S, M, A> Pure(A value) =>
        new (input => M.Pure((value, input.Output, input.State)));
    
    public static RWST<R, W, S, M, A> Lift(K<M, A> ma) =>
        new (input => ma.Map(a => (a, input.Output, input.State)));
    
    public static RWST<R, W, S, M, A> Lift(Pure<A> ma) =>
        new (input => M.Pure((ma.Value, input.Output, input.State)));
    
    public static RWST<R, W, S, M, A> LiftIO(K<IO, A> ma) =>
        new (input => M.LiftIOMaybe(ma).Map(a => (a, input.Output, input.State)));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Reader behaviours
    //
    
    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, A> Asks(Func<R, A> f) =>
        new(input => M.Pure((f(input.Env), input.Output, input.State)));

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, A> AsksM(Func<R, K<M, A>> f) =>
        new(input => f(input.Env).Map(a => (a, input.Output, input.State)));

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`RWST`</returns>
    public RWST<R1, W, S, M, A> With<R1>(Func<R1, R> f) =>
        new(input => runRWST((f(input.Env), input.Output, input.State)));

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, A> Local(Func<R, R> f) =>
        new(input => runRWST((f(input.Env), input.Output, input.State)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Writer behaviours
    //

    /// <summary>
    /// Construct a writer computation from a (result, output) pair.
    /// </summary>
    /// <remarks>
    /// The inverse of `Run()`
    /// </remarks>
    /// <param name="result">Result / Output pair</param>
    public static RWST<R, W, S, M, A> Write((A Value, W Output) result) =>
        Writable.write<W, RWST<R, W, S, M>, A>(result).As();

    /// <summary>
    /// Construct a writer computation from a (result, output) pair.
    /// </summary>
    /// <remarks>
    /// The inverse of `Run()`
    /// </remarks>
    /// <param name="result">Result / Output pair</param>
    public static RWST<R, W, S, M, A> Write(A value, W output) =>
        Writable.write<W, RWST<R, W, S, M>, A>(value, output).As();

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public RWST<R, W, S, M, (A Value, W Output)> Listen =>
        Writable.listen<W, RWST<R, W, S, M>, A>(this).As();

    /// <summary>
    /// `Listens` executes the action and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public RWST<R, W, S, M, (A Value, B Output)> Listens<B>(Func<W, B> f) =>
        Writable.listens(f, this).As();

    /// <summary>
    /// `Censor` executes the action and applies the function `f` to its output,
    /// leaving the return value unchanged.
    /// </summary>
    public RWST<R, W, S, M, A> Censor(Func<W, W> f) =>
        Writable.censor(f, this).As();
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  State behaviours
    //

    /// <summary>
    /// Extracts the state value, maps it, and then puts it back into
    /// the monadic state.
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, Unit> Modify(Func<S, S> f) =>
        new(input => M.Pure((unit, input.Output, f(input.State))));

    /// <summary>
    /// Extracts the state value, maps it, and then puts it back into
    /// the monadic state.
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, Unit> ModifyM(Func<S, K<M, S>> f) =>
        new(input => f(input.State).Map(s => (unit, input.Output, s)));

    /// <summary>
    /// Writes the value into the monadic state
    /// </summary>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, Unit> Put(S value) =>
        new(input => M.Pure((unit, input.Output, value)));

    /// <summary>
    /// Writes a value and state into the monad
    /// </summary>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, A> State(A value, S state) =>
        new(input => M.Pure((value, input.Output, state)));

    /// <summary>
    /// Writes a value and state into the monad
    /// </summary>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, A> State((A Value, S State) ma) =>
        new(input => M.Pure((ma.Value, input.Output, ma.State)));

    /// <summary>
    /// Extracts the state value and returns it as the bound value
    /// </summary>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, S> Get { get; } =
        new(input => M.Pure((input.State, input.Output, input.State)));

    /// <summary>
    /// Extracts the state value and maps it to the bound value
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, A> Gets(Func<S, A> f) =>
        new(input => M.Pure((f(input.State), input.Output, input.State)));

    /// <summary>
    /// Extracts the state value and maps it to the bound value
    /// </summary>
    /// <param name="f">State mapping function</param>
    /// <returns>`RWST`</returns>
    public static RWST<R, W, S, M, A> GetsM(Func<S, K<M, A>> f) =>
        new(input => M.Map(v => (v, input.Output, input.State), f(input.State)));
    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, B> Map<B>(Func<A, B> f) =>
        Functor.map(f, this).As();
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, B> Select<B>(Func<A, B> f) =>
        Functor.map(f, this).As();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, B> Bind<B>(Func<A, K<RWST<R, W, S, M>, B>> f) =>
        Monad.bind(this, f).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, B> Bind<B>(Func<A, RWST<R, W, S, M, B>> f) =>
        Monad.bind(this, f).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, B> Bind<B>(Func<A, Ask<R, B>> f) =>
        Bind(x => (RWST<R, W, S, M, B>)f(x));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => RWST<R, W, S, M, B>.LiftIO(f(x)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, B> Bind<B>(Func<A, K<IO, B>> f) =>
        Bind(x => RWST<R, W, S, M, B>.LiftIO(f(x)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<B, C>(Func<A, RWST<R, W, S, M, B>> bind, Func<A, B, C> project) =>
        new(input => runRWST(input).Bind(
                output1 => bind(output1.Value)
                          .runRWST((input.Env, output1.Output, output1.State))
                          .Map(output2 => (project(output1.Value, output2.Value), output2.Output, output2.State)))); 

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<B, C>(Func<A, K<RWST<R, W, S, M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => RWST<R, W, S, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => RWST<R, W, S, M, B>.LiftIO(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => RWST<R, W, S, M, B>.LiftIO(bind(x)), project);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<B, C>(Func<A, Ask<R, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).ToReadable<RWST<R, W, S, M>>(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<C>(Func<A, Put<S>> bind, Func<A, Unit, C> project) =>
        SelectMany(x => bind(x).ToStateful<RWST<R, W, S, M>>(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<B, C>(Func<A, Gets<S, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).ToStateful<RWST<R, W, S, M>>(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<C>(Func<A, Modify<S>> bind, Func<A, Unit, C> project) =>
        SelectMany(x => bind(x).ToStateful<RWST<R, W, S, M>>(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`RWST`</returns>
    public RWST<R, W, S, M, C> SelectMany<C>(Func<A, Tell<W>> bind, Func<A, Unit, C> project) =>
        SelectMany(x => bind(x).ToWritable<RWST<R, W, S, M>>(), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //
    
    public static implicit operator RWST<R, W, S, M, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator RWST<R, W, S, M, A>(Ask<R, A> ma) =>
        Asks(ma.F);

    public static implicit operator RWST<R, W, S, M, A>(IO<A> ma) =>
        LiftIO(ma);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the RWST
    //

    /// <summary>
    /// Run the monad 
    /// </summary>
    public K<M, (A Value, W Output, S State)> Run(R env, W output, S state) => 
        runRWST((env, output, state));
    
    /// <summary>
    /// Run the monad 
    /// </summary>
    public K<M, (A Value, W Output, S State)> Run(R env, S state) => 
        runRWST((env, W.Empty, state));
}
