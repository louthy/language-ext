using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `Writer` monad transformer, which adds a modifiable state to a given monad. 
/// </summary>
/// <param name="runState">Transducer that represents the transformer operation</param>
/// <typeparam name="S">State type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record Writer<W, A>(Func<W, (A Value, W Output)> runWriter) : K<Writer<W>, A>
    where W : Monoid<W>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`Writer`</returns>
    public static Writer<W, A> Pure(A value) =>
        new (w => (value, w));

    /// <summary>
    /// Construct a writer computation from a (result, output) pair.
    /// </summary>
    /// <remarks>
    /// The inverse of `Run()`
    /// </remarks>
    /// <param name="result">Result / Output pair</param>
    public Writer<W, A> Write((A Value, W Output) result) =>
        new(w => (result.Value, w.Combine(result.Output)));

    /// <summary>
    /// Construct a writer computation from a (result, output) pair.
    /// </summary>
    /// <remarks>
    /// The inverse of `Run()`
    /// </remarks>
    /// <param name="result">Result / Output pair</param>
    public Writer<W, A> Write(A value, W output) =>
        new(w => (value, w.Combine(output)));

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public Writer<W, (A Value, W Output)> Listen() =>
        Listens(x => x);

    /// <summary>
    /// `Listens` executes the action and adds the result of applying `f` to the
    /// output to the value of the computation.
    /// </summary>
    public Writer<W, (A Value, B Output)> Listens<B>(Func<W, B> f) =>
        new(w =>
            {
                var (a, w1) = Run();
                return ((a, f(w1)), w + w1);
            });

    /// <summary>
    /// `Censor` executes the action and applies the function `f` to its output,
    /// leaving the return value unchanged.
    /// </summary>
    public Writer<W, A> Censor(Func<W, W> f) =>
        new (w =>
             {
                 var (a, w1) = Run();
                 return (a, w + f(w1));
             });
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`Writer`</returns>
    public static Writer<W, A> Lift(Pure<A> monad) =>
        Pure(monad.Value);

    /// <summary>
    /// Lifts a function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`Writer`</returns>
    public static Writer<W, A> Lift(Func<A> f) =>
        new(w => (f(), w));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Writer`</returns>
    public Writer<W, B> Map<B>(Func<A, B> f) =>
        new(w => mapFirst(f, runWriter(w)));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Writer`</returns>
    public Writer<W, B> Select<B>(Func<A, B> f) =>
        new(w => mapFirst(f, runWriter(w)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Writer`</returns>
    public Writer<W, B> Bind<B>(Func<A, K<Writer<W>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Writer`</returns>
    public Writer<W, B> Bind<B>(Func<A, Writer<W, B>> f) =>
        new(w =>
            {
                var (a, w1) = runWriter(w);
                return f(a).runWriter(w1);
            });

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Writer`</returns>
    public Writer<W, Unit> Bind<B>(Func<A, Tell<W>> f) =>
        Bind(x => f(x).ToWriter());

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
    /// <returns>`Writer`</returns>
    public Writer<W, C> SelectMany<B, C>(Func<A, K<Writer<W>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Writer`</returns>
    public Writer<W, C> SelectMany<B, C>(Func<A, Writer<W, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Writer`</returns>
    public Writer<W, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Writer`</returns>
    public Writer<W, C> SelectMany<C>(Func<A, Tell<W>> bind, Func<A, Unit, C> project) =>
        SelectMany(x => bind(x).ToWriter(), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static Writer<W, A> operator >> (Writer<W, A> lhs, Writer<W, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static Writer<W, A> operator >> (Writer<W, A> lhs, K<Writer<W>, A> rhs) =>
        lhs.Bind(_ => rhs);

    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static Writer<W, A> operator >> (Writer<W, A> lhs, Writer<W, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static Writer<W, A> operator >> (Writer<W, A> lhs, K<Writer<W>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    public static implicit operator Writer<W, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the monad
    //

    /// <summary>
    /// Run the writer 
    /// </summary>
    /// <returns>Bound monad</returns>
    public (A Value, W Output) Run() =>
        runWriter(W.Empty);
}
