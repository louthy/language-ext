using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class WriterT
    {
        public static Unit IterT<Out, T>(this Writer<Out,T> self, Action<T> action) =>
            self.Iter(action);

        public static int CountT<Out, T>(this Writer<Out, T> self) =>
            self.Count();

        public static bool ForAllT<Out, T>(this Writer<Out, T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        public static S FoldT<Out, S, T>(this Writer<Out, T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        public static bool ExistsT<Out, T>(this Writer<Out, T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        public static Writer<Out, R> MapT<Out, T, R>(this Writer<Out, T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        public static Writer<Out, R> BindT<Out, T, R>(this Writer<Out, T> self, Func<T, Writer<Out, R>> binder) =>
            self.Bind(binder);
    }
}
