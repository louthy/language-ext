using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Observable extensions
    /// </summary>
    public static class ObservableExt
    {
        /// <summary>
        /// Executes an action post-subscription.  This is useful when the action is 
        /// going to publish to the observable.  A kind of request/response.
        /// </summary>
        [Pure]
        public static IObservable<T> PostSubscribe<T>(
            this IObservable<T> self,
            Action action
            )
        {
            return new ActionObservable<T>(action, self);
        }

        /// <summary>
        /// Executes an action post-subscription.  This is useful when the action is 
        /// going to publish to the observable.  A kind of request/response.
        /// </summary>
        [Pure]
        public static IObservable<T> PostSubscribe<T>(
            this IObservable<T> self,
            Func<Unit> action
            )
        {
            return new ActionObservable<T>(() => action(), self);
        }
    }
}
