using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class ReaderT
    {
        public static Reader<Env, Unit> IterT<Env, T>(this Reader<Env, T> self, Action<T> action) =>
            self.Iter(action);

        public static int CountT<Env, T>(this Reader<Env, T> self) =>
            self.Count();

        public static Reader<Env,bool> ForAllT<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        public static Reader<Env, S> FoldT<Env, S, T>(this Reader<Env, T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        public static Reader<Env, bool> ExistsT<Env, T>(this Reader<Env, T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        public static Reader<Env, R> MapT<Env, T, R>(this Reader<Env, T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        public static Reader<Env, R> BindT<Env, T, R>(this Reader<Env, T> self, Func<T, Reader<Env, R>> binder) =>
            self.Bind(binder);
    }
}
