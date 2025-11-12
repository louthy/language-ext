using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Represents a pure value.  Usually understood to be the 'success' value.
/// </summary>
/// <remarks>
/// On its own this doesn't do much, but  allows other monads to convert
/// from it and provide binding extensions that mean it can be lifted into
/// other monads without specifying lots of extra generic arguments.
/// </remarks>
/// <param name="Value">Bound value</param>
/// <typeparam name="A">Bound value type</typeparam>
public readonly record struct Pure<A>(A Value)
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Standard operators
    //

    /// <summary>
    /// Functor map
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Result bound value type</typeparam>
    /// <returns>Result of the applying the mapping function to the `Pure` value</returns>
    public Pure<B> Map<B>(Func<A, B> f) =>
        new (f(Value));

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="B">Result bound value type</typeparam>
    /// <returns>Result of the applying the bind function to the `Pure` value</returns>
    public Pure<B> Bind<B>(Func<A, Pure<B>> f) =>
        f(Value);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="L">Result bound value type</typeparam>
    /// <returns>Result of the applying the bind function to the `Pure` value</returns>
    public Either<L, A> Bind<L>(Func<A, Fail<L>> f) =>
        f(Value);

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <typeparam name="B">Result bound value type</typeparam>
    /// <returns>Result of the applying the bind function to the `Pure` value</returns>
    public IO<B> Bind<B>(Func<A, IO<B>> f) =>
        ToIO().Bind(f); 

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    /// <typeparam name="B">Result of the bind operation bound value type</typeparam>
    /// <typeparam name="C">Result of the mapping operation bound value type</typeparam>
    /// <returns>Result of the applying the bind and mapping function to the `Pure` value</returns>
    public Pure<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        new(project(Value, bind(Value).Value));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    /// <typeparam name="L">Result of the bind operation bound value type</typeparam>
    /// <typeparam name="C">Result of the mapping operation bound value type</typeparam>
    /// <returns>Result of the applying the bind and mapping function to the `Pure` value</returns>
    public Either<L, C> SelectMany<L, C>(Func<A, Fail<L>> bind, Func<A, Unit, C> project) =>
        bind(Value).Value;

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    /// <typeparam name="B">Result of the bind operation bound value type</typeparam>
    /// <typeparam name="C">Result of the mapping operation bound value type</typeparam>
    /// <returns>Result of the applying the bind and mapping function to the `Pure` value</returns>
    public IO<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        ToIO().SelectMany(bind, project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public Option<A> ToOption() =>
        Value is null 
            ? Option<A>.None 
            : Option.Some(Value);
    
    public Either<L, A> ToEither<L>() =>
        Either.Right<L, A>(Value);
    
    public Fin<A> ToFin() =>
        Fin.Succ(Value);
    
    public IO<A> ToIO() =>
        IO.pure(Value);
    
    public Eff<RT, A> ToEff<RT>() =>
        Eff<RT, A>.Pure(Value);
    
    public Eff<A> ToEff() =>
        Eff<A>.Pure(Value);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    public Either<L, B> Bind<L, B>(Func<A, Either<L, B>> bind) =>
        bind(Value);

    public Fin<B> Bind<B>(Func<A, Fin<B>> bind) =>
        bind(Value);

    public Eff<RT, B> Bind<RT, B>(Func<A, Eff<RT, B>> bind) =>
        bind(Value);

    public Eff<B> Bind<B>(Func<A, Eff<B>> bind) =>
        bind(Value);
    
    public K<M, B> Bind<M, B>(Func<A, K<M, B>> bind)
        where M : Monad<M> =>
        bind(Value);
    
    public Reader<Env, B> Bind<Env, B>(Func<A, Reader<Env, B>> bind) =>
        bind(Value);
    
    public ReaderT<Env, M, B> Bind<Env, M, B>(Func<A, ReaderT<Env, M, B>> bind)
        where M : Monad<M>, Alternative<M> =>
        bind(Value);
    
    public State<S, B> Bind<S, B>(Func<A, State<S, B>> bind) =>
        bind(Value);
    
    public StateT<S, M, B> Bind<S, M, B>(Func<A, StateT<S, M, B>> bind)
        where M : Monad<M>, Alternative<M> =>
        bind(Value);
    
    public OptionT<M, B> Bind<M, B>(Func<A, OptionT<M, B>> bind)
        where M : Monad<M> =>
        bind(Value);
    
    public Option<B> Bind<B>(Func<A, Option<B>> bind) =>
        bind(Value);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    public Either<L, C> SelectMany<L, B, C>(Func<A, Either<L, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Fin<C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<RT, C> SelectMany<RT, B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public K<M, C> SelectMany<M, B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project)
         where M : Monad<M> =>
         Bind(x => M.Map(y => project(x, y), bind(x)));
    
    public Reader<Env, C> SelectMany<Env, B, C>(Func<A, Reader<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public Reader<Env, C> SelectMany<Env, B, C>(Func<A, K<Reader<Env>, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).As().Map(y => project(x, y)));
    
    public ReaderT<Env, M, C> SelectMany<Env, M, B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public ReaderT<Env, M, C> SelectMany<Env, M, B, C>(Func<A, K<ReaderT<Env, M>, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        Bind(x => bind(x).As().Map(y => project(x, y)));
    
    public State<S, C> SelectMany<S, B, C>(Func<A, State<S, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public State<S, C> SelectMany<S, B, C>(Func<A, K<State<S>, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).As().Map(y => project(x, y)));
    
    public StateT<S, M, C> SelectMany<S, M, B, C>(Func<A, StateT<S, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public StateT<S, M, C> SelectMany<S, M, B, C>(Func<A, K<StateT<S, M>, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        Bind(x => bind(x).As().Map(y => project(x, y)));
    
    public OptionT<M, C> SelectMany<M, B, C>(Func<A, OptionT<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    public OptionT<M, C> SelectMany<M, B, C>(Func<A, K<OptionT<M>, B>> bind, Func<A, B, C> project)
        where M : Monad<M> =>
        Bind(x => bind(x).As().Map(y => project(x, y)));

    public Option<C> SelectMany<B, C>(Func<A, Option<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}

public static class PureExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    /// <param name="mma">Nested `Pure` monad</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Flattened monad</returns>
    public static Pure<A> Flatten<A>(this Pure<Pure<A>> mma) =>
        mma.Value;

    public static Validation<F, B> Bind<F, A, B>(this Pure<A> ma, Func<A, Validation<F, B>> bind)
        where F : Monoid<F> =>
        bind(ma.Value);

    public static Validation<F, A> ToValidation<F, A>(this Pure<A> ma)
        where F : Monoid<F> =>
        Validation.Success<F, A>(ma.Value);

    public static Validation<F, C> SelectMany<F, A, B, C>(
        this Pure<A> ma,
        Func<A, Validation<F, B>> bind,
        Func<A, B, C> project)
        where F : Monoid<F> =>
        bind(ma.Value).Map(y => project(ma.Value, y));
}
