using System;
using System.Threading.Tasks;

namespace LanguageExt
{
    public partial class Prelude
    {
        /// <summary>
        /// Get the Sum of a Task int.  Returns either the wrapped value or 0 if cancelled or faulted.
        /// </summary>
        public static int sum(Task<int> self) =>
            self.Sum();

        /// <summary>
        /// Get the Count of a Task T.  Returns either 1 or 0 if cancelled or faulted.
        /// </summary>
        public static int count<T>(Task<T> self) => self.Count();

        /// <summary>
        /// Monadic bind operation for Task
        /// </summary>
        public static Task<U> bind<T, U>(Task<T> self, Func<T, Task<U>> bind) =>
            self.Bind(bind);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        public static bool exists<T>(Task<T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        /// <summary>
        /// Returns false if the Task is cancelled or faulted, otherwise
        /// it returns the result of pred(Result)
        /// </summary>
        public static bool forall<T>(Task<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        /// <summary>
        /// Filters the task.  This throws a BottomException when pred(Result)
        /// returns false
        /// </summary>
        public static Task<T> filter<T>(Task<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        /// <summary>
        /// Folds the Task.  Returns folder(state,Result) if not faulted or
        /// cancelled.  Returns state otherwise.
        /// </summary>
        public static S fold<T, S>(Task<T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        /// <summary>
        /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
        /// </summary>
        public static Unit iter<T>(Task<T> self, Action<T> f) =>
            self.Iter(f);

        /// <summary>
        /// Returns map(Result) if not faulted or cancelled.
        /// </summary>
        public static Task<U> map<T, U>(Task<T> self, Func<T, U> map) =>
            self.Select(map);

        /// <summary>
        /// Extracts the value from the Task - here for completeness so that
        /// the HKT work.
        /// </summary>
        public static T liftUnsafe<T>(Task<T> self) where T : class =>
            self.LiftUnsafe();

        /// <summary>
        /// Extracts the value from the Task - here for completeness so that
        /// the HKT work.
        /// </summary>
        public static T lift<T>(Task<T> self) where T : struct =>
            self.Lift();
    }
}
