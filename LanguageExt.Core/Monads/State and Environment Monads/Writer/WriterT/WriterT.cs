using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `WriterT` monad transformer, which adds a modifiable state to a given monad. 
/// </summary>
/// <param name="runState">Transducer that represents the transformer operation</param>
/// <typeparam name="S">State type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record WriterT<W, M, A>(Func<W, K<M, (A Value, W Output)>> runWriter) : K<WriterT<W, M>, A>
    where M : Monad<M>, SemiAlternative<M>
    where W : Monoid<W>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> Pure(A value) =>
        new (w => M.Pure((value, w)));

    /// <summary>
    /// Construct a writer computation from a (result, output) pair.
    /// </summary>
    /// <remarks>
    /// The inverse of `Run()`
    /// </remarks>
    /// <param name="result">Result / Output pair</param>
    public WriterT<W, M, A> Write((A Value, W Output) result) =>
        new(w => M.Pure((result.Value, w.Combine(result.Output))));

    /// <summary>
    /// Construct a writer computation from a (result, output) pair.
    /// </summary>
    /// <remarks>
    /// The inverse of `Run()`
    /// </remarks>
    /// <param name="result">Result / Output pair</param>
    public WriterT<W, M, A> Write(A value, W output) =>
        new(w => M.Pure((value, w.Combine(output))));

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public WriterT<W, M, (A Value, W Output)> Listen() =>
        Listens(x => x);

    /// <summary>
    /// `Listens` executes the action and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public WriterT<W, M, (A Value, B Output)> Listens<B>(Func<W, B> f) =>
        new(w => Run().Map(aw => ((aw.Value, f(aw.Output)), w + aw.Output)));

    /// <summary>
    /// `Censor` executes the action and applies the function `f` to its output,
    /// leaving the return value unchanged.
    /// </summary>
    public WriterT<W, M, A> Censor(Func<W, W> f) =>
        new(w => Run().Map(aw => (aw.Value, w + f(aw.Output)))); 
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> Lift(Pure<A> monad) =>
        Pure(monad.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> Lift(K<M, A> monad) =>
        new(w => M.Map(value => (value, w), monad));

    /// <summary>
    /// Lifts a function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> Lift(Func<A> f) =>
        new(w => M.Pure((f(), w)));
    
    /// <summary>
    /// Lifts a an IO monad into the monad 
    /// </summary>
    /// <remarks>NOTE: If the IO monad isn't the innermost monad of the transformer
    /// stack then this will throw an exception.</remarks>
    /// <param name="ma">IO monad to lift</param>
    /// <returns>`WriterT`</returns>
    public static WriterT<W, M, A> LiftIO(IO<A> ma) =>
        Lift(M.LiftIO(ma));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="M1">Trait of the monad to map to</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M1, B> MapT<M1, B>(Func<K<M, (A Value, W Output)>, K<M1, (B Value, W Output)>> f)
        where M1 : Monad<M1>, SemiAlternative<M1> =>
        new (w => f(runWriter(w)));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`StateT`</returns>
    public WriterT<W, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new(state =>
                runWriter(state)
                   .Bind(vs => f(M.Pure(vs.Value)).Map(x => (x, vs.Output))));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, B> Map<B>(Func<A, B> f) =>
        new(w => M.Map(x => (f(x.Value), x.Output), runWriter(w)));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, B> Select<B>(Func<A, B> f) =>
        new(w => M.Map(x => (f(x.Value), x.Output), runWriter(w)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, B> Bind<B>(Func<A, K<WriterT<W, M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, B> Bind<B>(Func<A, WriterT<W, M, B>> f) =>
        new(w => M.Bind(
                      runWriter(w), 
                      mx => f(mx.Value).runWriter(mx.Output)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, Unit> Bind<B>(Func<A, Tell<W>> f) =>
        Bind(x => f(x).ToWriterT<M>());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => WriterT<W, M, B>.LiftIO(f(x)));

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
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, C> SelectMany<B, C>(Func<A, K<WriterT<W, M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, C> SelectMany<B, C>(Func<A, WriterT<W, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => WriterT<W, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, C> SelectMany<C>(Func<A, Tell<W>> bind, Func<A, Unit, C> project) =>
        SelectMany(x => bind(x).ToWriterT<M>(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`WriterT`</returns>
    public WriterT<W, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    public static WriterT<W, M, A> operator >> (WriterT<W, M, A> lhs, WriterT<W, M, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    public static WriterT<W, M, A> operator >> (WriterT<W, M, A> lhs, K<WriterT<W, M>, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    public static implicit operator WriterT<W, M, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator WriterT<W, M, A>(IO<A> ma) =>
        LiftIO(ma);
    
    public static WriterT<W, M, A> operator |(WriterT<W, M, A> ma, WriterT<W, M, A> mb) =>
        new (w => M.Combine(ma.runWriter(w), mb.runWriter(w)));
    
    public static WriterT<W, M, A> operator |(WriterT<W, M, A> ma, Pure<A> mb) =>
        new (w => M.Combine(ma.runWriter(w), Pure(mb.Value).runWriter(w)));
    
    public static WriterT<W, M, A> operator |(Pure<A> ma,  WriterT<W, M, A>mb) =>
        new (w => M.Combine(Pure(ma.Value).runWriter(w), mb.runWriter(w)));
    
    public static WriterT<W, M, A> operator |(IO<A> ma, WriterT<W, M, A> mb) =>
        new (w => M.Combine(LiftIO(ma).runWriter(w), mb.runWriter(w)));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the monad
    //

    /// <summary>
    /// Run the writer 
    /// </summary>
    /// <returns>Bound monad</returns>
    public K<M, (A Value, W Output)> Run() =>
        runWriter(W.Empty);
}
