using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class __IEnumerableEitherUnsafeTransExt
    {
        public static Unit IterT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Action<L> action) =>
            self.Iter(x => x.Iter(action));

        public static Unit IterT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Action<R> Right, Action<L> Left) =>
            self.Iter(x => x.Iter(Right,Left));

        [Pure]
        public static bool ForAllT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Func<L, bool> pred) =>
            self.Map(x => x.ForAll(pred)).Any();

        [Pure]
        public static bool ForAllT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.ForAll(Right, Left)).Any();

        [Pure]
        public static S FoldT<L, R, S>(this IEnumerable<EitherUnsafe<L, R>> self, S state, Func<S, L, S> folder) =>
            self.Fold(state, (s, e) => e.Fold(s,folder));

        [Pure]
        public static S FoldT<L, R, S>(this IEnumerable<EitherUnsafe<L, R>> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            self.Fold(state, (s, e) => e.Fold(s, Right, Left));

        [Pure]
        public static bool ExistsT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Func<L, bool> pred) =>
            self.Exists(x => x.Exists(pred));

        [Pure]
        public static bool ExistsT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Exists(x => x.Exists(Right, Left));

        [Pure]
        public static IEnumerable<EitherUnsafe<Ret, R>> MapLeftT<L, R, Ret>(this IEnumerable<EitherUnsafe<L, R>> self, Func<L, Ret> mapper) =>
            self.Map(x => x.MapLeft(mapper));

        [Pure]
        public static IEnumerable<EitherUnsafe<LRet, RRet>> BiMapT<L, R, LRet, RRet>(this IEnumerable<EitherUnsafe<L, R>> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
            self.Map(x => x.BiMap(Right, Left));

        [Pure]
        public static IEnumerable<EitherUnsafe<Ret, R>> BindT<L, R, Ret>(this IEnumerable<EitherUnsafe<L, R>> self, Func<L, EitherUnsafe<Ret, R>> binder) =>
            self.Map(x => x.Bind(binder));

        [Pure]
        public static IEnumerable<EitherUnsafe<LRet, RRet>> BindT<L, R, LRet, RRet>(this IEnumerable<EitherUnsafe<L, R>> self, Func<R, EitherUnsafe<LRet, RRet>> Right, Func<L, EitherUnsafe<LRet, RRet>> Left) =>
            self.Map(x => x.Bind(Right, Left));

        [Pure]
        public static IEnumerable<EitherUnsafe<L, R>> FilterT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Func<L, bool> pred) =>
            self.Map(x => x.Filter(pred));

        [Pure]
        public static IEnumerable<EitherUnsafe<L, R>> FilterT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.Filter(Right,Left));
    }

    public static partial class Prelude
    {
        public static Unit iterT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Action<L> action) =>
            self.Iter(x => x.Iter(action));

        public static Unit iterT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Action<R> Right, Action<L> Left) =>
            self.Iter(x => x.Iter(Right, Left));

        [Pure]
        public static bool forallT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Func<L, bool> pred) =>
            self.Map(x => x.ForAll(pred)).Any();

        [Pure]
        public static bool forallT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.ForAll(Right, Left)).Any();

        [Pure]
        public static S foldT<L, R, S>(IEnumerable<EitherUnsafe<L, R>> self, S state, Func<S, L, S> folder) =>
            self.Fold(state, (s, e) => e.Fold(s, folder));

        [Pure]
        public static S foldT<L, R, S>(IEnumerable<EitherUnsafe<L, R>> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            self.Fold(state, (s, e) => e.Fold(s, Right, Left));

        [Pure]
        public static bool existsT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Func<L, bool> pred) =>
            self.Exists(x => x.Exists(pred));

        [Pure]
        public static bool existsT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Exists(x => x.Exists(Right, Left));

        [Pure]
        public static IEnumerable<EitherUnsafe<Ret, R>> mapLeftT<L, R, Ret>(IEnumerable<EitherUnsafe<L, R>> self, Func<L, Ret> mapper) =>
            self.Map(x => x.MapLeft(mapper));

        [Pure]
        public static IEnumerable<EitherUnsafe<LRet, RRet>> bimapT<L, R, LRet, RRet>(IEnumerable<EitherUnsafe<L, R>> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
            self.Map(x => x.BiMap(Right, Left));

        [Pure]
        public static IEnumerable<EitherUnsafe<Ret, R>> bindT<L, R, Ret>(IEnumerable<EitherUnsafe<L, R>> self, Func<L, EitherUnsafe<Ret, R>> binder) =>
            self.Map(x => x.Bind(binder));

        [Pure]
        public static IEnumerable<EitherUnsafe<LRet, RRet>> bindT<L, R, LRet, RRet>(IEnumerable<EitherUnsafe<L, R>> self, Func<R, EitherUnsafe<LRet, RRet>> Right, Func<L, EitherUnsafe<LRet, RRet>> Left) =>
            self.Map(x => x.Bind(Right, Left));

        [Pure]
        public static IEnumerable<EitherUnsafe<L, R>> filterT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Func<L, bool> pred) =>
            self.Map(x => x.Filter(pred));

        [Pure]
        public static IEnumerable<EitherUnsafe<L, R>> filterT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.Filter(Right, Left));
    }
}
