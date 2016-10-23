using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IEnumerableEitherTransExt
{
    [Pure]
    public static bool BiForAllT<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
        self.Map(x => x.BiForAll(Right, Left)).Any();

    [Pure]
    public static S BiFoldT<L, R, S>(this IEnumerable<Either<L, R>> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
        self.Fold(state, (s, e) => e.BiFold(s, Right, Left));

    [Pure]
    public static bool BiExistsT<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
        self.Exists(x => x.BiExists(Right, Left));

    [Pure]
    public static IEnumerable<Either<Ret, R>> MapLeftT<L, R, Ret>(this IEnumerable<Either<L, R>> self, Func<L, Ret> mapper) =>
        self.Map(x => x.MapLeft(mapper));

    [Pure]
    public static IEnumerable<Either<LRet, RRet>> BiMapT<L, R, LRet, RRet>(this IEnumerable<Either<L, R>> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
        self.Map(x => x.BiMap(Right, Left));
}

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Pure]
        public static bool biforallT<L, R>(IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Map(x => x.BiForAll(Right, Left)).Any();

        [Pure]
        public static S bifoldT<L, R, S>(IEnumerable<Either<L, R>> self, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            self.Fold(state, (s, e) => e.BiFold(s, Right, Left));

        [Pure]
        public static bool biexistsT<L, R>(IEnumerable<Either<L, R>> self, Func<R, bool> Right, Func<L, bool> Left) =>
            self.Exists(x => x.BiExists(Right, Left));

        [Pure]
        public static IEnumerable<Either<Ret, R>> mapLeftT<L, R, Ret>(IEnumerable<Either<L, R>> self, Func<L, Ret> mapper) =>
            self.Map(x => x.MapLeft(mapper));

        [Pure]
        public static IEnumerable<Either<LRet, RRet>> bimapT<L, R, LRet, RRet>(IEnumerable<Either<L, R>> self, Func<R, RRet> Right, Func<L, LRet> Left) =>
            self.Map(x => x.BiMap(Right, Left));
    }
}
