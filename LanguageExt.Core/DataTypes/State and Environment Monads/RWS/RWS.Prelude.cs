using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> flatten<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, RWS<MonoidW, R, W, S, A>> ma)
            where MonoidW : struct, Monoid<W> =>
            ma.Bind(identity);

        /// <summary>
        /// RWS monad constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWS<MonoidW, R, W, S, A>(A value) where MonoidW : struct, Monoid<W> => (_, state) =>
            RWSResult<MonoidW, R, W, S, A>.New(state, value);

        /// <summary>
        /// RWS monad constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWS<MonoidW, R, W, S, A>(S state, A value) where MonoidW : struct, Monoid<W> => (_, __) =>
            RWSResult<MonoidW, R, W, S, A>.New(state, value);

        /// <summary>
        /// RWS monad constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWS<MonoidW, R, W, S, A>(W output, S state, A value) where MonoidW : struct, Monoid<W> => (_, __) =>
            RWSResult<MonoidW, R, W, S, A>.New(output, state, value);

        /// <summary>
        /// RWS monad constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWS<MonoidW, R, W, S, A>(W output, A value) where MonoidW : struct, Monoid<W> => (_, state) =>
            RWSResult<MonoidW, R, W, S, A>.New(output, state, value);

        /// <summary>
        /// RWS monad constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWS<MonoidW, R, W, S, A>(Func<R, S, (A, W, S)> f) where MonoidW : struct, Monoid<W> =>
            (env, state) =>
            {
                var (a, w, s) = f(env, state);
                return RWSResult<MonoidW, R, W, S, A>.New(w, s, a);
            };

        /// <summary>
        /// RWS failure constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWSFail<MonoidW, R, W, S, A>(Error error) where MonoidW : struct, Monoid<W> => (_, state) =>
            RWSResult<MonoidW, R, W, S, A>.New(state, error);

        /// <summary>
        /// RWS failure constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWSFail<MonoidW, R, W, S, A>(Exception exception) where MonoidW : struct, Monoid<W> =>
            RWSFail<MonoidW, R, W, S, A>(Error.New(exception));

        /// <summary>
        /// RWS failure constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWSFail<MonoidW, R, W, S, A>(string message, Exception exception) where MonoidW : struct, Monoid<W> =>
            RWSFail<MonoidW, R, W, S, A>(Error.New(message, exception));

        /// <summary>
        /// RWS failure constructor
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>RWS monad</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> RWSFail<MonoidW, R, W, S, A>(string message) where MonoidW : struct, Monoid<W> =>
            RWSFail<MonoidW, R, W, S, A>(Error.New(message));

        /// <summary>
        /// Get the state from monad into its wrapped value
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with state in the value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, S> get<MonoidW, R, W, S, A>()
           where MonoidW : struct, Monoid<W> =>
            default(MRWS<MonoidW, R, W, S, S>).Get();

        /// <summary>
        /// Set the state from monad into its wrapped value
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with state set and with Unit value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, Unit> put<MonoidW, R, W, S, A>(S state)
           where MonoidW : struct, Monoid<W> =>
            default(MRWS<MonoidW, R, W, S, Unit>).Put(state);

        /// <summary>
        /// Monadic state transformer
        /// Maps an old state to a new state inside a RWS monad.The old state is thrown away.
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with state set and with Unit value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, Unit> modify<MonoidW, R, W, S, A>(Func<S, S> f)
        where MonoidW : struct, Monoid<W> => (env, state) =>
            RWSResult<MonoidW, R, W, S, Unit>.New(default(MonoidW).Empty(), f(state), unit);

        /// <summary>
        /// Gets a projection of the state
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with projected value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> gets<MonoidW, R, W, S, A>(Func<S, A> f)
           where MonoidW : struct, Monoid<W> => (env, state) =>
           default(MRWS<MonoidW, R, W, S, A>).Return(input => f(input.State))(env, state);

        /// <summary>
        /// Retrieves the reader monad environment.
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with the environment in as the bound value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, R> ask<MonoidW, R, W, S>()
            where MonoidW : struct, Monoid<W> => default(MRWS<MonoidW, R, W, S, R>).Ask();

        /// <summary>
        /// Retrieves a function of the current environment.
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with the mapped environment in as the bound value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> asks<MonoidW, R, W, S, A>(Func<R, A> f)
            where MonoidW : struct, Monoid<W> => default(MRWS<MonoidW, R, W, S, A>)
                .Return(input => f(input.Env));

        /// <summary>
        /// Executes a computation in a modified environment
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with the mapped environment in as the bound value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> local<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> ma, Func<R, R> f)
            where MonoidW : struct, Monoid<W> => default(MRWS<MonoidW, R, W, S, A>).Local(ma, f);

        /// <summary>
        /// Chooses the first monad result that has a Some(x) for the value
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with the mapped environment in as the bound value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, Option<A>> choose<MonoidW, R, W, S, A>(params RWS<MonoidW, R, W, S, Option<A>>[] monads)
            where MonoidW : struct, Monoid<W> => (env, state) =>
            {
                foreach (var monad in monads)
                {
                    var res = monad(env, state);
                    if (res.Value.IsSome && !res.IsFaulted)
                    {
                        return res;
                    }
                }
                return RWSResult<MonoidW, R, W, S, Option<A>>.New(default(MonoidW).Empty(), state, Option<A>.None);
            };

        /// <summary>
        /// Run the RWS and catch exceptions
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with the mapped environment in as the bound value</returns>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> tryrws<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> m)
            where MonoidW : struct, Monoid<W> => (env, state) =>
            {
                try
                {
                    return m(env, state);
                }
                catch(Exception e)
                {
                    return RWSResult<MonoidW, R, W, S, A>.New(default(MonoidW).Empty(), state, Error.New(e));
                }
            };

        /// <summary>
        /// Run the RWS and catch exceptions
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>RWS monad with the mapped environment in as the bound value</returns>
        [Pure]
        public static Try<RWS<MonoidW, R, W, S, A>> tryfun<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> ma)
            where MonoidW : struct, Monoid<W> => () =>
            from x in ma
            select x;

        [Pure]
        public static RWS<MonoidW, R, W, S, int>  sum<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, int> self)
            where MonoidW : struct, Monoid<W> =>
                self.Sum();

        [Pure]
        public static RWS<MonoidW, R, W, S, int>  count<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> self)
            where MonoidW : struct, Monoid<W> =>
                self.Count();

        [Pure]
        public static RWS<MonoidW, R, W, S, bool>  forall<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
            where MonoidW : struct, Monoid<W> =>
                self.ForAll(pred);

        [Pure]
        public static RWS<MonoidW, R, W, S, bool>  exists<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
            where MonoidW : struct, Monoid<W> =>
                self.Exists(pred);

        [Pure]
        public static RWS<MonoidW, R, W, S, FState> fold<MonoidW, R, W, S, A, FState>(RWS<MonoidW, R, W, S, A> self, FState initial, Func<FState, A, FState> f)
            where MonoidW : struct, Monoid<W> =>
                self.Fold(initial, f);

        [Pure]
        public static RWS<MonoidW, R, W, S, R> fold<MonoidW, R, W, S, A, FState>(RWS<MonoidW, R, W, S, A> self, Func<R, A, R> f)
            where MonoidW : struct, Monoid<W> =>
                self.Fold(f);

        /// <summary>
        /// Pass is an action that executes the monad, which
        /// returns a value and a function, and returns the value, applying
        /// the function to the output.
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        [Pure]
        public static RWS<MonoidW, R, W, S, A>  pass<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, (A, Func<W, W>)> self)
            where MonoidW : struct, Monoid<W> =>
                self.Pass();

        /// <summary>
        /// Listen is an action that executes the monad and adds
        /// its output to the value of the computation.
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        [Pure]
        public static RWS<MonoidW, R, W, S, (A, B)> listen<MonoidW, R, W, S, A, B>(RWS<MonoidW, R, W, S, A> self, Func<W, B> f)
            where MonoidW : struct, Monoid<W> =>
                self.Listen(f);

        /// <summary>
        /// Censor is an action that executes the writer monad and applies the function f 
        /// to its output, leaving the return value unchanged.
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        [Pure]
        public static RWS<MonoidW, R, W, S, A> censor<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> self, Func<W, W> f)
            where MonoidW : struct, Monoid<W> =>
                self.Censor(f);

        /// <summary>
        /// Censor is an action that executes the writer monad and applies the function f 
        /// to its output, leaving the return value unchanged.
        /// </summary>
        /// <typeparam name="R">Environment type</typeparam>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        [Pure]
        public static RWS<MonoidW, R, W, S, Unit> tell<MonoidW, R, W, S, A>(W what)
            where MonoidW : struct, Monoid<W> =>
                default(MRWS<MonoidW, R, W, S, A>).Tell(what);

        [Pure]
        public static RWS<MonoidW, R, W, S, B> bind<MonoidW, R, W, S, A, B>(RWS<MonoidW, R, W, S, A> self, Func<A, RWS<MonoidW, R, W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
                self.Bind(f);

        [Pure]
        public static RWS<MonoidW, R, W, S, B> map<MonoidW, R, W, S, A, B>(RWS<MonoidW, R, W, S, A> self, Func<A, B> f)
            where MonoidW : struct, Monoid<W> =>
                self.Map(f);

        [Pure]
        public static RWS<MonoidW, R, W, S, A> filter<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
            where MonoidW : struct, Monoid<W> =>
                self.Filter(pred);

        [Pure]
        public static RWS<MonoidW, R, W, S, Unit> iter<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> self, Action<A> action)
            where MonoidW : struct, Monoid<W> =>
                self.Iter(action);
    }
}
