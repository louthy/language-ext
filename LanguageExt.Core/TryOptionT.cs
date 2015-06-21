using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class TryOptionT
    {
        public static Unit IterT<T>(this TryOption<T> self, Action<T> action) =>
            self.Iter(action);

        public static int CountT<T>(this TryOption<T> self) =>
            self.Count();

        public static bool ForAllT<T>(this TryOption<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        public static S FoldT<S, T>(this TryOption<T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        public static bool ExistsT<T>(this TryOption<T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        public static TryOption<T> FilterT<T>(this TryOption<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        public static TryOption<R> MapT<T, R>(this TryOption<T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        public static TryOption<R> BindT<T, R>(this TryOption<T> self, Func<T, TryOption<R>> binder) =>
            self.Bind(binder);
    }
}
