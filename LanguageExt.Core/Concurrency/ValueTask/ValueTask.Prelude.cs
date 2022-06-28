using LanguageExt;
using LanguageExt.ClassInstances;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Convert a value to a Task that completes immediately
        /// </summary>
        [Pure]
        public static ValueTask<A> ValueTaskSucc<A>(A self) =>
            new ValueTask<A>(self);

        /// <summary>
        /// Convert a value to a Task that completes immediately
        /// </summary>
        [Pure]
        public static ValueTask<A> ValueTaskFail<A>(Exception ex) =>
            Task.FromException<A>(ex).ToValue();

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static ValueTask<A> flatten<A>(ValueTask<ValueTask<A>> self) =>
            self.Flatten();

        /// <summary>
        /// Flatten the nested Task type
        /// </summary>
        [Pure]
        public static ValueTask<A> flatten<A>(ValueTask<ValueTask<ValueTask<A>>> self) =>
            self.Flatten();

        /// <summary>
        /// Get the Count of a Task T.  Returns either 1 or 0 if cancelled or faulted.
        /// </summary>
        [Pure]
        public static ValueTask<int> count<A>(ValueTask<A> self) =>
            self.Count();

        /// <summary>
        /// Monadic bind operation for Task
        /// </summary>
        [Pure]
        public static ValueTask<B> bind<A, B>(ValueTask<A> self, Func<A, ValueTask<B>> bind) =>
            self.Bind(bind);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static ValueTask<bool> exists<A>(ValueTask<A> self, Func<A, bool> pred) =>
            self.Exists(pred);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static ValueTask<bool> existsAsync<A>(ValueTask<A> self, Func<A, ValueTask<bool>> pred) =>
            self.ExistsAsync(pred);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static ValueTask<bool> forall<A>(ValueTask<A> self, Func<A, bool> pred) =>
            self.ForAll(pred);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        [Pure]
        public static ValueTask<bool> forallAsync<A>(ValueTask<A> self, Func<A, ValueTask<bool>> pred) =>
            self.ForAllAsync(pred);

        /// <summary>
        /// Filters the task.  This throws a BottomException when pred(Result)
        /// returns false
        /// </summary>
        [Pure]
        public static ValueTask<A> filter<A>(ValueTask<A> self, Func<A, bool> pred) =>
            self.Filter(pred);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static ValueTask<S> fold<S, A>(ValueTask<A> self, S state, Func<S, A, S> folder) =>
            self.Fold(state, folder);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        [Pure]
        public static ValueTask<S> foldAsync<S, A>(ValueTask<A> self, S state, Func<S, A, ValueTask<S>> folder) =>
            self.FoldAsync(state, folder);

        /// <summary>
        /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
        /// </summary>
        public static ValueTask<Unit> iter<A>(ValueTask<A> self, Action<A> f) =>
            self.Iter(f);

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static ValueTask<B> map<A, B>(ValueTask<A> self, Func<A, B> map) =>
            self.Map(map);

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        [Pure]
        public static ValueTask<B> mapAsync<A, B>(ValueTask<A> self, Func<A, ValueTask<B>> map) =>
            self.MapAsync(map);

        [Pure]
        public static ValueTask<A> plus<A>(this ValueTask<A> ma, ValueTask<A> mb) =>
            default(MValueTask<A>).Plus(ma, mb);

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="ma">The first computation to run</param>
        /// <param name="tail">The rest of the computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static ValueTask<A> choice<A>(ValueTask<A> ma, params ValueTask<A>[] tail) =>
            choice(Cons(ma, tail));

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="xs">Sequence of computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static async ValueTask<A> choice<A>(Seq<ValueTask<A>> xs) =>
            xs.IsEmpty
                ? await BottomException.Default.AsFailedTask<A>().ConfigureAwait(false)
                : await default(MValueTask<A>).MatchAsync(
                    xs.Head,
                    SomeAsync: async x  => await xs.Head,
                    NoneAsync: async () => await choice(xs.Tail))
                    .ConfigureAwait(false);
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static ValueTask<B> apply<A, B>(ValueTask<Func<A, B>> fab, ValueTask<A> fa) =>
            ApplValueTask<A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static ValueTask<B> apply<A, B>(Func<A, B> fab, ValueTask<A> fa) =>
            ApplValueTask<A, B>.Inst.Apply(fab.AsValueTask(), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static ValueTask<C> apply<A, B, C>(ValueTask<Func<A, B, C>> fabc, ValueTask<A> fa, ValueTask<B> fb) =>
            from x in fabc
            from y in ApplValueTask<A, B, C>.Inst.Apply(curry(x).AsValueTask(), fa, fb)
            select y;

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static ValueTask<C> apply<A, B, C>(Func<A, B, C> fabc, ValueTask<A> fa, ValueTask<B> fb) =>
            ApplValueTask<A, B, C>.Inst.Apply(curry(fabc).AsValueTask(), fa, fb);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static ValueTask<Func<B, C>> apply<A, B, C>(ValueTask<Func<A, B, C>> fabc, ValueTask<A> fa) =>
            from x in fabc
            from y in ApplValueTask<A, B, C>.Inst.Apply(curry(x).AsValueTask(), fa)
            select y;

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static ValueTask<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, ValueTask<A> fa) =>
            ApplValueTask<A, B, C>.Inst.Apply(curry(fabc).AsValueTask(), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static ValueTask<Func<B, C>> apply<A, B, C>(ValueTask<Func<A, Func<B, C>>> fabc, ValueTask<A> fa) =>
            ApplValueTask<A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static ValueTask<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, ValueTask<A> fa) =>
            ApplValueTask<A, B, C>.Inst.Apply(fabc.AsValueTask(), fa);
    }
}
