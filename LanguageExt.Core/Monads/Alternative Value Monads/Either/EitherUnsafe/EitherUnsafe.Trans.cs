using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IEnumerableEitherUnsafeTransExt
{
    [Pure]
    public static bool BiForAllT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
        self.Map(x => x.BiForAll(Right, Left)).Any();

    [Pure]
    public static S BiFoldT<L, R, S>(this IEnumerable<EitherUnsafe<L, R>> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
        self.Fold(state, (s, e) => e.BiFold(s, Right, Left));

    [Pure]
    public static bool BiExistsT<L, R>(this IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
        self.Exists(x => x.BiExists(Right, Left));

    [Pure]
    public static IEnumerable<EitherUnsafe<Ret, R>> MapLeftT<L, R, Ret>(this IEnumerable<EitherUnsafe<L, R>> self, Func<L, Ret> mapper) =>
        self.Map(x => x.MapLeft(mapper));

    [Pure]
    public static IEnumerable<EitherUnsafe<LRet, RRet>> BiMapT<L, R, LRet, RRet>(this IEnumerable<EitherUnsafe<L, R>> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
        self.Map(x => x.BiMap(Right, Left));
}

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Pure]
        public static bool biforallT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.BiForAll(Right, Left)).Any();

        [Pure]
        public static S bifoldT<L, R, S>(IEnumerable<EitherUnsafe<L, R>> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            self.Fold(state, (s, e) => e.BiFold(s, Right, Left));

        [Pure]
        public static bool biexistsT<L, R>(IEnumerable<EitherUnsafe<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Exists(x => x.BiExists(Right, Left));

        [Pure]
        public static IEnumerable<EitherUnsafe<Ret, R>> mapLeftT<L, R, Ret>(IEnumerable<EitherUnsafe<L, R>> self, Func<L, Ret> mapper) =>
            self.Map(x => x.MapLeft(mapper));

        [Pure]
        public static IEnumerable<EitherUnsafe<LRet, RRet>> bimapT<L, R, LRet, RRet>(IEnumerable<EitherUnsafe<L, R>> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
            self.Map(x => x.BiMap(Right, Left));
    }
}
