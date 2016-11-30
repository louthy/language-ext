using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class __IEnumerableEitherTransExt
    {
        public static Unit IterT<L, R>(this IEnumerable<Either<L, R>> self, Action<L> action) =>
            self.Iter(x => x.Iter(action));

        public static Unit IterT<L, R>(this IEnumerable<Either<L, R>> self, Action<R> Right, Action<L> Left) =>
            self.Iter(x => x.Iter(Right,Left));

        [Pure]
        public static bool ForAllT<L, R>(this IEnumerable<Either<L, R>> self, Func<L, bool> pred) =>
            self.Map(x => x.ForAll(pred)).Any();

        [Pure]
        public static bool ForAllT<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.ForAll(Right, Left)).Any();

        [Pure]
        public static S FoldT<L, R, S>(this IEnumerable<Either<L, R>> self, S state, Func<S, L, S> folder) =>
            self.Fold(state, (s, e) => e.Fold(s,folder));

        [Pure]
        public static S FoldT<L, R, S>(this IEnumerable<Either<L, R>> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            self.Fold(state, (s, e) => e.Fold(s, Right, Left));

        [Pure]
        public static bool ExistsT<L, R>(this IEnumerable<Either<L, R>> self, Func<L, bool> pred) =>
            self.Exists(x => x.Exists(pred));

        [Pure]
        public static bool ExistsT<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Exists(x => x.Exists(Right, Left));

        [Pure]
        public static IEnumerable<Either<Ret, R>> MapLeftT<L, R, Ret>(this IEnumerable<Either<L, R>> self, Func<L, Ret> mapper) =>
            self.Map(x => x.MapLeft(mapper));

        [Pure]
        public static IEnumerable<Either<LRet, RRet>> BiMapT<L, R, LRet, RRet>(this IEnumerable<Either<L, R>> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
            self.Map(x => x.BiMap(Right, Left));

        [Pure]
        public static IEnumerable<Either<Ret, R>> BindT<L, R, Ret>(this IEnumerable<Either<L, R>> self, Func<L, Either<Ret, R>> binder) =>
            self.Map(x => x.Bind(binder));

        [Pure]
        public static IEnumerable<Either<LRet, RRet>> BindT<L, R, LRet, RRet>(this IEnumerable<Either<L, R>> self, Func<R, Either<LRet, RRet>> Right, Func<L, Either<LRet, RRet>> Left) =>
            self.Map(x => x.Bind(Right, Left));

        [Pure]
        public static IEnumerable<Either<L, R>> FilterT<L, R>(this IEnumerable<Either<L, R>> self, Func<L, bool> pred) =>
            self.Map(x => x.Filter(pred));

        [Pure]
        public static IEnumerable<Either<L, R>> FilterT<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.Filter(Right,Left));
    }

    public static partial class Prelude
    {
        public static Unit iterT<L, R>(IEnumerable<Either<L, R>> self, Action<L> action) =>
            self.Iter(x => x.Iter(action));

        public static Unit iterT<L, R>(IEnumerable<Either<L, R>> self, Action<R> Right, Action<L> Left) =>
            self.Iter(x => x.Iter(Right, Left));

        [Pure]
        public static bool forallT<L, R>(IEnumerable<Either<L, R>> self, Func<L, bool> pred) =>
            self.Map(x => x.ForAll(pred)).Any();

        [Pure]
        public static bool forallT<L, R>(IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.ForAll(Right, Left)).Any();

        [Pure]
        public static S foldT<L, R, S>(IEnumerable<Either<L, R>> self, S state, Func<S, L, S> folder) =>
            self.Fold(state, (s, e) => e.Fold(s, folder));

        [Pure]
        public static S foldT<L, R, S>(IEnumerable<Either<L, R>> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            self.Fold(state, (s, e) => e.Fold(s, Right, Left));

        [Pure]
        public static bool existsT<L, R>(IEnumerable<Either<L, R>> self, Func<L, bool> pred) =>
            self.Exists(x => x.Exists(pred));

        [Pure]
        public static bool existsT<L, R>(IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Exists(x => x.Exists(Right, Left));

        [Pure]
        public static IEnumerable<Either<Ret, R>> mapLeftT<L, R, Ret>(IEnumerable<Either<L, R>> self, Func<L, Ret> mapper) =>
            self.Map(x => x.MapLeft(mapper));

        [Pure]
        public static IEnumerable<Either<LRet, RRet>> bimapT<L, R, LRet, RRet>(IEnumerable<Either<L, R>> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
            self.Map(x => x.BiMap(Right, Left));

        [Pure]
        public static IEnumerable<Either<Ret, R>> bindT<L, R, Ret>(IEnumerable<Either<L, R>> self, Func<L, Either<Ret, R>> binder) =>
            self.Map(x => x.Bind(binder));

        [Pure]
        public static IEnumerable<Either<LRet, RRet>> bindT<L, R, LRet, RRet>(IEnumerable<Either<L, R>> self, Func<R, Either<LRet, RRet>> Right, Func<L, Either<LRet, RRet>> Left) =>
            self.Map(x => x.Bind(Right, Left));

        [Pure]
        public static IEnumerable<Either<L, R>> filterT<L, R>(IEnumerable<Either<L, R>> self, Func<L, bool> pred) =>
            self.Map(x => x.Filter(pred));

        [Pure]
        public static IEnumerable<Either<L, R>> filterT<L, R>(IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.Filter(Right, Left));
    }
}
