using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class StateT
    {
        public static State<S, Unit> IterT<S, T>(this State<S, T> self, Action<T> action) =>
            self.Iter(action);

        public static int CountT<S, T>(this State<S, T> self) =>
            self.Count();

        public static State<S, bool> ForAllT<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        public static State<S, FState> FoldT<S, T, FState>(this State<S, T> self, FState state, Func<FState, T, FState> folder) =>
            self.Fold(state, folder);

        public static State<S, Unit> FoldT<S, T>(this State<S, T> self, Func<S, T, S> folder) =>
            self.Fold(folder);

        public static State<S, bool> ExistsT<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        public static State<S, R> MapT<S, T, R>(this State<S, T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        public static State<S, R> BindT<S, T, R>(this State<S, T> self, Func<T, State<S, R>> binder) =>
            self.Bind(binder);
    }
}
