using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// State monad constructor
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>State monad</returns>
        [Pure]
        public static State<S, A> State<S, A>(A value) =>
            default(MState<S, A>).Return(value);

        /// <summary>
        /// Get the state from monad into its wrapped value
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>State monad with state in the value</returns>
        [Pure]
        public static State<S, S> get<S>() =>
            default(MState<S, S>).Get();

        /// <summary>
        /// Set the state 
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>State monad with state set and with a Unit value</returns>
        [Pure]
        public static State<S, Unit> put<S>(S state) =>
            default(MState<S, Unit>).Put(state);

        /// <summary>
        /// modify::MonadState s m => (s -> s) -> m()
        /// 
        /// Monadic state transformer.
        /// 
        /// Maps an old state to a new state inside a state monad.The old state is thrown away.
        /// </summary>
        [Pure]
        public static State<S, Unit> modify<S>(Func<S, S> f) =>
            default(MState<S, Unit>).State(s => (unit, f(s), false));

        /// <summary>
        /// gets :: MonadState s m => (s -> a) -> m a
        /// 
        /// Gets specific component of the state, using a projection function supplied.
        /// </summary>
        [Pure]
        public static State<S, A> gets<S, A>(Func<S, A> f) =>
            default(MState<S, S>).Bind<MState<S, A>, State<S, A>, A>(
                default(MState<S, S>).Get(), s =>
                    default(MState<S, A>).Return(f(s)));

        /// <summary>
        /// Chooses the first monad result that has a Some(x) for the value
        /// </summary>
        [Pure]
        public static State<S, Option<T>> choose<S, T>(params State<S, Option<T>>[] monads) =>
            default(MState<S, Option<T>>).Return(state => 
            {
                foreach (var monad in monads)
                {
                    var (x, s, bottom) = monad.Eval(state);
                    if (!bottom && x.IsSome)
                    {
                        return (x, s, bottom);
                    }
                }
                return (default(T), state, true);
            });

        [Pure]
        public static State<S, T> trystate<S, T>(Func<State<S, T>> tryDel) =>
            default(MState<S, T>).Return(state =>
            {
                try
                {
                    return tryDel().Eval(state);
                }
                catch
                {
                    return (default(T), state, true);
                }
            });
    }
}
