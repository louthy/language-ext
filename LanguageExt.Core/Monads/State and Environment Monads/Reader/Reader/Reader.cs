using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `Reader` monad transformer, which adds a static environment to a given monad. 
/// </summary>
/// <param name="runReader">Transducer that represents the transformer operation</param>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record Reader<Env, A>(Func<Env, A> runReader) : K<Reader<Env>, A>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`Reader`</returns>
    public static Reader<Env, A> Pure(A value) =>
        new(_ => value);

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`Reader`</returns>
    public static Reader<Env, A> Asks(Func<Env, A> f) =>
        new(f);

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`Reader`</returns>
    public static Reader<Env, A> AsksM(Func<Env, Reader<Env, A>> f) =>
        Reader<Env, Reader<Env, A>>.Asks(f).Flatten();

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`Reader`</returns>
    public static Reader<Env, A> Lift(Pure<A> monad) =>
        Pure(monad.Value);
    
    /// <summary>
    /// Lifts a unit function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`Reader`</returns>
    public static Reader<Env, A> Lift(Func<A> f) =>
        new (_ => f());

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`Reader`</returns>
    public Reader<Env1, A> With<Env1>(Func<Env1, Env> f) =>
        new (e => runReader(f(e)));

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`Reader`</returns>
    public Reader<Env, A> Local(Func<Env, Env> f) =>
        new (e => runReader(f(e)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, B> Map<B>(Func<A, B> f) =>
        new(e => f(runReader(e)));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, B> Select<B>(Func<A, B> f) =>
        new(e => f(runReader(e)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, B> Bind<B>(Func<A, K<Reader<Env>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, B> Bind<B>(Func<A, Reader<Env, B>> f) =>
        new(e => f(runReader(e)).runReader(e));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, B> Bind<B>(Func<A, Ask<Env, B>> f) =>
        Bind(x => (Reader<Env, B>)f(x));

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
    /// <returns>`Reader`</returns>
    public Reader<Env, C> SelectMany<B, C>(Func<A, K<Reader<Env>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, C> SelectMany<B, C>(Func<A, Reader<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y))); 

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, C> SelectMany<B, C>(Func<A, Ask<Env, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).ToReader(), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    public static implicit operator Reader<Env, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator Reader<Env, A>(Ask<Env, A> ma) =>
        Asks(ma.F);

    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static Reader<Env, A> operator >> (Reader<Env, A> lhs, Reader<Env, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static Reader<Env, A> operator >> (Reader<Env, A> lhs, K<Reader<Env>, A> rhs) =>
        lhs.Bind(_ => rhs);

    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static Reader<Env, A> operator >> (Reader<Env, A> lhs, Reader<Env, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static Reader<Env, A> operator >> (Reader<Env, A> lhs, K<Reader<Env>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the reader
    //

    /// <summary>
    /// Run the reader monad 
    /// </summary>
    /// <param name="env">Input environment</param>
    /// <returns>Computed value</returns>
    public A Run(Env env) =>
        runReader(env);
}
