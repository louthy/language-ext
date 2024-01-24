using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct OrdTry<OrdA, A> : Ord<Try<A>> where OrdA : Ord<A>
{
    public static int Compare(Try<A> x, Try<A> y)
    {
        var dx = x.Try();
        var dy = y.Try();
        if (dx.IsBottom  && dy.IsBottom) return 0;
        if (dx.IsFaulted && dy.IsFaulted) return 0;
        if (dx.IsSuccess && dy.IsSuccess)
        {
            return OrdA.Compare(dx.Value, dy.Value);
        }

        if (dx.IsBottom  && !dy.IsBottom) return -1;
        if (!dx.IsBottom && dy.IsBottom) return 1;

        if (dx.IsFaulted  && !dy.IsFaulted) return -1;
        if (!dx.IsFaulted && dy.IsFaulted) return 1;

        if (dx.IsSuccess  && !dy.IsSuccess) return -1;
        if (!dx.IsSuccess && dy.IsSuccess) return 1;
        return 0;
    }

    public static bool Equals(Try<A> x, Try<A> y) =>
        EqTry<OrdA, A>.Equals(x, y);

    public static int GetHashCode(Try<A> x) =>
        HashableTry<OrdA, A>.GetHashCode(x);
}

public struct OrdTry<A> : Ord<Try<A>>
{
    public static int Compare(Try<A> x, Try<A> y) =>
        OrdTry<OrdDefault<A>, A>.Compare(x, y);
 
    public static bool Equals(Try<A> x, Try<A> y) =>
        EqTry<EqDefault<A>, A>.Equals(x, y);

    public static int GetHashCode(Try<A> x) =>
        HashableTry<A>.GetHashCode(x);
}
