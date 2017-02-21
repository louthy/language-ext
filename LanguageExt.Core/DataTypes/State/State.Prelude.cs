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
            default(MState<SState<S, A>, State<S, A>, S, A>).Return(value);

        /// <summary>
        /// State monad constructor
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>State monad</returns>
        [Pure]
        public static State<S, A> State<S, A>(Func<S, (A, S)> f) =>
            default(MState<SState<S, A>, State<S, A>, S, A>).Return(s => f(s).Add(false));

        /// <summary>
        /// Get the state from monad into its wrapped value
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>State monad with state in the value</returns>
        [Pure]
        public static State<S, S> get<S>() =>
            default(MState<SState<S, S>, State<S, S>, S, S>).Get<SState<S, S>, State<S, S>>();

        /// <summary>
        /// Set the state 
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>State monad with state set and with a Unit value</returns>
        [Pure]
        public static State<S, Unit> put<S>(S state) =>
            default(MState<SState<S, Unit>, State<S, Unit>, S, Unit>).Put<SState<S, Unit>, State<S, Unit>>(state);

        /// <summary>
        /// modify::MonadState s m => (s -> s) -> m()
        /// 
        /// Monadic state transformer.
        /// 
        /// Maps an old state to a new state inside a state monad.The old state is thrown away.
        /// </summary>
        [Pure]
        public static State<S, Unit> modify<S>(Func<S, S> f) =>
            default(MState<SState<S, Unit>, State<S, Unit>, S, Unit>).State(s => (unit, f(s), false));

        /// <summary>
        /// gets :: MonadState s m => (s -> a) -> m a
        /// 
        /// Gets specific component of the state, using a projection function supplied.
        /// </summary>
        [Pure]
        public static State<S, A> gets<S, A>(Func<S, A> f) =>
            default(MState<SState<S, S>, State<S, S>, S, S>).Bind<MState<SState<S, A>, State<S, A>, S, A>, State<S, A>, A>(
                default(MState<SState<S, S>, State<S, S>, S, S>).Get<SState<S, S>, State<S, S>>(), s =>
                    default(MState<SState<S, A>, State<S, A>, S, A>).Return(f(s)));

        /// <summary>
        /// Chooses the first monad result that has a Some(x) for the value
        /// </summary>
        [Pure]
        public static State<S, Option<A>> choose<S, A>(params State<S, Option<A>>[] monads) =>
            default(MState<SState<S, Option<A>>, State<S, Option<A>>, S, Option<A>>).Return(state => 
            {
                foreach (var monad in monads)
                {
                    var (x, s, bottom) = monad.Eval(state);
                    if (!bottom && x.IsSome)
                    {
                        return (x, s, bottom);
                    }
                }
                return (default(A), state, true);
            });

        [Pure]
        public static State<S, A> trystate<S, A>(State<S, A> value) =>
            default(MState<SState<S, A>, State<S, A>, S, A>).Return(state =>
            {
                try
                {
                    return value.Eval(state);
                }
                catch
                {
                    return (default(A), state, true);
                }
            });

        [Pure]
        public static Try<State<S, A>> tryfun<S, A>(State<S, A> ma) => () =>
            from x in ma
            select x;
    }
}
