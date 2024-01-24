using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct OrdTryOption<OrdA, A> : Ord<TryOption<A>> where OrdA : Ord<A>
{
    public static int Compare(TryOption<A> x, TryOption<A> y)
    {
        var dx = x.Try();
        var dy = y.Try();
        if (dx.IsBottom  && dy.IsBottom) return 0;
        if (dx.IsFaulted && dy.IsFaulted) return 0;
        if (dx.IsNone    && dy.IsNone) return 0;
        if (dx.IsSome && dy.IsSome)
        {
            return OrdA.Compare(dx.Value.Value ?? throw new ValueIsNullException(), dy.Value.Value ?? throw new ValueIsNullException());
        }

        if (dx.IsBottom  && !dy.IsBottom) return -1;
        if (!dx.IsBottom && dy.IsBottom) return 1;

        if (dx.IsFaulted  && !dy.IsFaulted) return -1;
        if (!dx.IsFaulted && dy.IsFaulted) return 1;

        if (dx.IsNone  && !dy.IsNone) return -1;
        if (!dx.IsNone && dy.IsNone) return 1;

        if (dx.IsSome  && !dy.IsSome) return -1;
        if (!dx.IsSome && dy.IsSome) return 1;
        return 0;
    }

    public static bool Equals(TryOption<A> x, TryOption<A> y) =>
        EqTryOption<OrdA, A>.Equals(x, y);

    public static int GetHashCode(TryOption<A> x) =>
        HashableTryOption<OrdA, A>.GetHashCode(x);
}

public struct OrdTryOption<A> : Ord<TryOption<A>>
{
    public static int Compare(TryOption<A> x, TryOption<A> y) =>
        OrdTryOption<OrdDefault<A>, A>.Compare(x, y);

    public static bool Equals(TryOption<A> x, TryOption<A> y) =>
        EqTryOption<EqDefault<A>, A>.Equals(x, y);

    public static int GetHashCode(TryOption<A> x) =>
        HashableTryOption<A>.GetHashCode(x);
}
