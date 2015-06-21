using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class TryT
    {
        public static Unit IterT<T>(this Try<T> self, Action<T> action) =>
            self.Iter(action);

        public static int CountT<T>(this Try<T> self) =>
            self.Count();

        public static bool ForAllT<T>(this Try<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        public static S FoldT<S, T>(this Try<T> self, S state, Func<S, T, S> folder) => 
            self.Fold(state, folder);

        public static bool ExistsT<T>(this Try<T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        public static Try<T> FilterT<T>(this Try<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        public static Try<R> MapT<T, R>(this Try<T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        public static Try<R> BindT<T, R>(this Try<T> self, Func<T, Try<R>> binder) =>
            self.Bind(binder);
    }
}
