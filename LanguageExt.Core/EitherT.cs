using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class EitherT
    {
        public static int CountT<L, R>(this Either<L, R> self) =>
            self.Count();

        public static bool ForAllT<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
            self.ForAll(pred);

        public static S FoldT<L, R, S>(this Either<L, R> self, S state, Func<S, R, S> folder) =>
            self.Fold(state,folder);

        public static bool ExistsT<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
            self.Exists(pred);

        public static Either<L, Ret> MapT<L, R, Ret>(this Either<L, R> self, Func<R, Ret> mapper) =>
            self.Map(mapper);

        public static Either<Unit, R> FilterT<L, R>(this Either<L, R> self, Func<R, bool> pred) =>
            self.Filter(pred);

        public static Either<L, Ret> BindT<L, R, Ret>(this Either<L, R> self, Func<R, Either<L, Ret>> binder) =>
            self.Bind(binder);
    }
}
