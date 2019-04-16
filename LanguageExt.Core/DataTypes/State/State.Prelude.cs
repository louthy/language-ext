using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public static State<Env, A> flatten<Env, A>(State<Env, State<Env, A>> ma) =>
            ma.Bind(identity);

        /// <summary>
        /// State monad constructor
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>State monad</returns>
        [Pure]
        public static State<S, A> State<S, A>(A value) =>
            default(MState<S, A>).Return(_ => value);

        /// <summary>
        /// State monad constructor
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>State monad</returns>
        [Pure]
        public static State<S, A> State<S, A>(Func<S, (A, S)> f) =>
            state => f(state).Add(false);

        /// <summary>
        /// Get the state from monad into its wrapped value
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>State monad with state in the value</returns>
        [Pure]
        public static State<S, S> get<S>() =>
            default(MState<S, S>).Get();

        /// <summary>
        /// Applies a lens in the 'get' direction within a state monad   
        /// </summary>
        [Pure]
        public static State<A, B> get<A, B>(Lens<A, B> la) =>
            get<A>().Map(la.Get);

        /// <summary>
        /// Set the state 
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>State monad with state set and with a Unit value</returns>
        [Pure]
        public static State<S, Unit> put<S>(S state) =>
            default(MState<S, Unit>).Put(state);

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
            s => (unit, f(s), false);

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
            default(MState<S, A>).Return(s => f(s));

        /// <summary>
        /// Chooses the first monad result that has a Some(x) for the value
        /// </summary>
        [Pure]
        public static State<S, Option<A>> choose<S, A>(params State<S, Option<A>>[] monads) =>
            state => 
            {
                foreach (var monad in monads)
                {
                    var (x, s, bottom) = monad(state);
                    if (!bottom && x.IsSome)
                    {
                        return (x, s, bottom);
                    }
                }
                return (default(A), state, true);
            };

        [Pure]
        public static State<S, A> trystate<S, A>(State<S, A> value) =>
            state =>
            {
                try
                {
                    return value(state);
                }
                catch
                {
                    return (default(A), state, true);
                }
            };

        [Pure]
        public static Try<State<S, A>> tryfun<S, A>(State<S, A> ma) => () =>
            from x in ma
            select x;
    }
}
