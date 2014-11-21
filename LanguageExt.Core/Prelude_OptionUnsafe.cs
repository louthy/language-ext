using System;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Prelude' to your code.
    /// </summary>
    public static partial class Prelude
    {
        /// <summary>
        /// Create a Some of T (Option<T>).  Use the to wrap any-type without coercian.
        /// That means you can wrap null, Nullable<T>, or Option<T> to get Option<Option<T>>
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to make optional</param>
        /// <returns>Option<T> in a Some state</returns>
        public static OptionUnsafe<T> SomeUnsafe<T>(T value) =>
            OptionUnsafe<T>.Some(value);

        public static T failureUnsafe<T>(OptionUnsafe<T> option, Func<T> None) =>
            option.FailureUnsafe(None);

        public static T failure<T>(OptionUnsafe<T> option, T noneValue) =>
            option.FailureUnsafe(noneValue);

        public static R matchUnsafe<T, R>(OptionUnsafe<T> option, Func<T, R> Some, Func<R> None) =>
            option.MatchUnsafe(Some, None);

        public static Unit matchUnsafe<T>(OptionUnsafe<T> option, Action<T> Some, Action None) =>
            option.MatchUnsafe(Some, None);

        public static S foldUnsafe<S, T>(OptionUnsafe<T> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static int Count<T>(OptionUnsafe<T> option) =>
            option.Count;

        public static bool existsUnsafe<T>(OptionUnsafe<T> option, Predicate<T> pred) =>
            option.Exists(pred);

        public static OptionUnsafe<R> mapUnsafe<T, R>(OptionUnsafe<T> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static OptionUnsafe<R> bindUnsafe<T, R>(OptionUnsafe<T> option, Func<T, OptionUnsafe<R>> binder) =>
            option.Bind(binder);
    }
}
