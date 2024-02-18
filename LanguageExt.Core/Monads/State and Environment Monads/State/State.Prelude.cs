using System;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static State<S, A> flatten<S, A>(State<S, State<S, A>> ma) =>
        new(ma.state.Bind(mx => mx.state));

    /// <summary>
    /// State monad constructor
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="value">Value</param>
    /// <returns>State monad</returns>
    [Pure]
    public static State<S, A> State<S, A>(A value) =>
        new (StateT<S, Identity, A>.Pure(value));

    /// <summary>
    /// State monad constructor
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="value">Value</param>
    /// <returns>State monad</returns>
    [Pure]
    public static State<S, (A, S)> State<S, A>(Func<S, (A, S)> f) =>
        new (StateT<S, Identity, (A, S)>.Gets(f));
    
    /// <summary>
    /// Get the state from monad into its wrapped value
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>State monad with state in the value</returns>
    [Pure]
    public static State<S, S> get<S>() =>
        new (StateT<S, Identity, S>.Get);

    /// <summary>
    /// Applies a lens in the 'get' direction within a state monad
    /// </summary>
    [Pure]
    public static State<A, B> get<A, B>(Lens<A, B> la) =>
        new (StateT<A, Identity, B>.Gets(la.Get));

    /// <summary>
    /// Set the state
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>State monad with state set and with a Unit value</returns>
    [Pure]
    public static State<S, Unit> put<S>(S state) =>
        new (StateT<S, Identity, S>.Put(state));

    /// <summary>
    /// Applies a lens in the 'set' direction within a state monad
    /// </summary>
    [Pure]
    public static State<A, Unit> put<A, B>(Lens<A, B> la, B value) =>
        from a in get<A>()
        from _ in put(la.Set(value, a))
        select unit;

    /// <summary>
    /// modify::MonadState s m => (s -> s) -> m()
    ///
    /// Monadic state transformer.
    ///
    /// Maps an old state to a new state inside a state monad.The old state is thrown away.
    /// </summary>
    [Pure]
    public static State<S, Unit> modify<S>(Func<S, S> f) =>
        new (StateT<S, Identity, S>.Modify(f));

    /// <summary>
    /// Update through a lens within a state monad
    /// </summary>
    [Pure]
    public static State<A, Unit> modify<A, B>(Lens<A, B> la, Func<B, B> f) =>
        from b in get(la)
        from _ in put(la, f(b))
        select unit;


    /// <summary>
    /// gets :: MonadState s m => (s -> a) -> m a
    ///
    /// Gets specific component of the state, using a projection function supplied.
    /// </summary>
    [Pure]
    public static State<S, A> gets<S, A>(Func<S, A> f) =>
        new (StateT<S, Identity, A>.Gets(f));

    /// <summary>
    /// Chooses the first monad result that has a Some(x) for the value
    /// </summary>
    [Pure]
    public static State<S, Option<A>> choose<S, A>(Seq<State<S, Option<A>>> ms) =>
        ms switch
        {
            { IsEmpty: true } => Pure(Option<A>.None),
            var (x, xs)       => x.Bind(oa => oa.IsSome ? Pure(oa) : choose(xs))
        };

    /// <summary>
    /// Chooses the first monad result that has a Some(x) for the value
    /// </summary>
    [Pure]
    public static State<S, Option<A>> choose<S, A>(params State<S, Option<A>>[] monads) =>
        choose(toSeq(monads));
}
