#nullable enable
using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct ApplHashSet<A, B> :
    Functor<HashSet<A>, HashSet<B>, A, B>,
    Applicative<HashSet<Func<A, B>>, HashSet<A>, HashSet<B>, A, B>
{
    [Pure]
    public static HashSet<B> Action(HashSet<A> fa, HashSet<B> fb) =>
        from a in fa
        from b in fb
        select b;

    [Pure]
    public static HashSet<B> Apply(HashSet<Func<A, B>> fab, HashSet<A> fa) =>
        from f in fab
        from a in fa
        select f(a);

    [Pure]
    public static HashSet<B> Map(HashSet<A> ma, Func<A, B> f) =>
        FHashSet<A, B>.Inst.Map(ma, f);

    [Pure]
    public static HashSet<A> Pure(A x) =>
        HashSet(x);
}

public struct ApplHashSet<A, B, C> :
    Applicative<HashSet<Func<A, Func<B, C>>>, HashSet<Func<B, C>>, HashSet<A>, HashSet<B>, HashSet<C>, A, B>
{
    [Pure]
    public static HashSet<Func<B, C>> Apply(HashSet<Func<A, Func<B, C>>> fabc, HashSet<A> fa) =>
        from f in fabc
        from a in fa
        select f(a);

    [Pure]
    public static HashSet<C> Apply(HashSet<Func<A, Func<B, C>>> fabc, HashSet<A> fa, HashSet<B> fb) =>
        from f in fabc
        from a in fa
        from b in fb
        select f(a)(b);

    [Pure]
    public static HashSet<A> Pure(A x) =>
        HashSet(x);
}

public struct ApplHashSet<A> :
    Applicative<HashSet<Func<A, A>>, HashSet<A>, HashSet<A>, A, A>,
    Applicative<HashSet<Func<A, Func<A, A>>>, HashSet<Func<A, A>>, HashSet<A>, HashSet<A>, HashSet<A>, A, A>
{
    [Pure]
    public static HashSet<A> Action(HashSet<A> fa, HashSet<A> fb) =>
        from a in fa
        from b in fb
        select b;

    [Pure]
    public static HashSet<A> Apply(HashSet<Func<A, A>> fab, HashSet<A> fa) =>
        from f in fab
        from a in fa
        select f(a);

    [Pure]
    public static HashSet<Func<A, A>> Apply(HashSet<Func<A, Func<A, A>>> fabc, HashSet<A> fa) =>
        from f in fabc
        from a in fa
        select f(a);

    [Pure]
    public static HashSet<A> Apply(HashSet<Func<A, Func<A, A>>> fabc, HashSet<A> fa, HashSet<A> fb) =>
        from f in fabc
        from a in fa
        from b in fb
        select f(a)(b);

    [Pure]
    public static HashSet<A> Pure(A x) =>
        HashSet(x);
}


public struct ApplHashSet<OrdFAB, OrdA, OrdB, A, B> :
    Functor<HashSet<OrdA, A>, HashSet<OrdB, B>, A, B>,
    Applicative<HashSet<OrdFAB, Func<A, B>>, HashSet<OrdA, A>, HashSet<OrdB, B>, A, B>
    where OrdA : Ord<A>
    where OrdB : Ord<B>
    where OrdFAB : Ord<Func<A, B>>
{
    [Pure]
    public static HashSet<OrdB, B> Action(HashSet<OrdA, A> fa, HashSet<OrdB, B> fb)
    {
        IEnumerable<B> Yield()
        {
            foreach (var _ in fa)
            {
                foreach (var b in fb)
                {
                    yield return b;
                }
            }
        }
        return new HashSet<OrdB, B>(Yield(), true);
    }

    [Pure]
    public static HashSet<OrdB, B> Apply(HashSet<OrdFAB, Func<A, B>> fab, HashSet<OrdA, A> fa)
    {
        IEnumerable<B> Yield()
        {
            foreach (var f in fab)
            {
                foreach (var a in fa)
                {
                    yield return f(a);
                }
            }
        }
        return new HashSet<OrdB, B>(Yield(), true);
    }

    [Pure]
    public static HashSet<OrdB, B> Map(HashSet<OrdA, A> ma, Func<A, B> f) =>
        ma.Map<OrdB, B>(f);

    [Pure]
    public static HashSet<OrdA, A> Pure(A x) =>
        HashSet<OrdA, A>(x);
}

public struct ApplHashSet<OrdFABC, OrdFBC, OrdA, OrdB, OrdC, A, B, C> :
    Applicative<HashSet<OrdFABC, Func<A, Func<B, C>>>, HashSet<OrdFBC, Func<B, C>>, HashSet<OrdA, A>, HashSet<OrdB, B>, HashSet<OrdC, C>, A, B>
    where OrdA : Ord<A>
    where OrdB : Ord<B>
    where OrdC : Ord<C>
    where OrdFABC : Ord<Func<A, Func<B, C>>>
    where OrdFBC : Ord<Func<B, C>>
{
    [Pure]
    public static HashSet<OrdFBC, Func<B, C>> Apply(HashSet<OrdFABC, Func<A, Func<B, C>>> fabc, HashSet<OrdA, A> fa)
    {
        IEnumerable<Func<B, C>> Yield()
        {
            foreach (var f in fabc)
            {
                foreach (var a in fa)
                {
                    yield return f(a);
                }
            }
        }
        return new HashSet<OrdFBC, Func<B, C>>(Yield(), true);
    }

    [Pure]
    public static HashSet<OrdC, C> Apply(HashSet<OrdFABC, Func<A, Func<B, C>>> fabc, HashSet<OrdA, A> fa, HashSet<OrdB, B> fb)
    {
        IEnumerable<C> Yield()
        {
            foreach (var f in fabc)
            {
                foreach (var a in fa)
                {
                    foreach (var b in fb)
                    {
                        yield return f(a)(b);
                    }
                }
            }
        }
        return new HashSet<OrdC, C>(Yield(), true);
    }

    [Pure]
    public static HashSet<OrdA, A> Pure(A x) =>
        HashSet<OrdA, A>(x);
}

public struct ApplHashSet<OrdFAAA, OrdFAA, OrdA, A> :
    Applicative<HashSet<OrdFAA, Func<A, A>>, HashSet<OrdA, A>, HashSet<OrdA, A>, A, A>,
    Applicative<HashSet<OrdFAAA, Func<A, Func<A, A>>>, HashSet<OrdFAA, Func<A, A>>, HashSet<OrdA, A>, HashSet<OrdA, A>, HashSet<OrdA, A>, A, A>
    where OrdA : Ord<A>
    where OrdFAAA : Ord<Func<A, Func<A, A>>>
    where OrdFAA : Ord<Func<A, A>>
{
    [Pure]
    public static HashSet<OrdA, A> Action(HashSet<OrdA, A> fa, HashSet<OrdA, A> fb)
    {
        IEnumerable<A> Yield()
        {
            foreach (var _ in fa)
            {
                foreach (var b in fb)
                {
                    yield return b;
                }
            }
        }
        return new HashSet<OrdA, A>(Yield(), true);
    }

    [Pure]
    public static HashSet<OrdA, A> Apply(HashSet<OrdFAA, Func<A, A>> fab, HashSet<OrdA, A> fa)
    {
        IEnumerable<A> Yield()
        {
            foreach (var f in fab)
            {
                foreach (var a in fa)
                {
                    yield return f(a);
                }
            }
        }
        return new HashSet<OrdA, A>(Yield(), true);
    }

    [Pure]
    public static HashSet<OrdFAA, Func<A, A>> Apply(HashSet<OrdFAAA, Func<A, Func<A, A>>> fabc, HashSet<OrdA, A> fa)
    {
        IEnumerable<Func<A, A>> Yield()
        {
            foreach (var f in fabc)
            {
                foreach (var a in fa)
                {
                    yield return f(a);
                }
            }
        }
        return new HashSet<OrdFAA, Func<A, A>>(Yield(), true);
    }
    [Pure]
    public static HashSet<OrdA, A> Apply(HashSet<OrdFAAA, Func<A, Func<A, A>>> fabc, HashSet<OrdA, A> fa, HashSet<OrdA, A> fb)
    {
        IEnumerable<A> Yield()
        {
            foreach (var f in fabc)
            {
                foreach (var a in fa)
                {
                    foreach (var b in fb)
                    {
                        yield return f(a)(b);
                    }
                }
            }
        }
        return new HashSet<OrdA, A>(Yield(), true);
    }

    [Pure]
    public static HashSet<OrdA, A> Pure(A x) =>
        HashSet<OrdA, A>(x);
}
