#nullable enable
using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct ApplSet<A, B> :
    Functor<Set<A>, Set<B>, A, B>,
    Applicative<Set<Func<A, B>>, Set<A>, Set<B>, A, B>
{
    [Pure]
    public static Set<B> Action(Set<A> fa, Set<B> fb) =>
        from a in fa
        from b in fb
        select b;

    [Pure]
    public static Set<B> Apply(Set<Func<A, B>> fab, Set<A> fa) =>
        from f in fab
        from a in fa
        select f(a);

    [Pure]
    public static Set<B> Map(Set<A> ma, Func<A, B> f) =>
        FSet<A, B>.Inst.Map(ma, f);

    [Pure]
    public static Set<A> Pure(A x) =>
        Set(x);
}

public struct ApplSet<A, B, C> :
    Applicative<Set<Func<A, Func<B, C>>>, Set<Func<B, C>>, Set<A>, Set<B>, Set<C>, A, B>
{
    [Pure]
    public static Set<Func<B, C>> Apply(Set<Func<A, Func<B, C>>> fabc, Set<A> fa) =>
        from f in fabc
        from a in fa
        select f(a);

    [Pure]
    public static Set<C> Apply(Set<Func<A, Func<B, C>>> fabc, Set<A> fa, Set<B> fb) =>
        from f in fabc
        from a in fa
        from b in fb
        select f(a)(b);

    [Pure]
    public static Set<A> Pure(A x) =>
        Set(x);
}

public struct ApplSet<A> :
    Applicative<Set<Func<A, A>>, Set<A>, Set<A>, A, A>,
    Applicative<Set<Func<A, Func<A, A>>>, Set<Func<A, A>>, Set<A>, Set<A>, Set<A>, A, A>
{
    [Pure]
    public static Set<A> Action(Set<A> fa, Set<A> fb) =>
        from a in fa
        from b in fb
        select b;

    [Pure]
    public static Set<A> Apply(Set<Func<A, A>> fab, Set<A> fa) =>
        from f in fab
        from a in fa
        select f(a);

    [Pure]
    public static Set<Func<A, A>> Apply(Set<Func<A, Func<A, A>>> fabc, Set<A> fa) =>
        from f in fabc
        from a in fa
        select f(a);

    [Pure]
    public static Set<A> Apply(Set<Func<A, Func<A, A>>> fabc, Set<A> fa, Set<A> fb) =>
        from f in fabc
        from a in fa
        from b in fb
        select f(a)(b);

    [Pure]
    public static Set<A> Pure(A x) =>
        Set(x);
}


public struct ApplSet<OrdFAB, OrdA, OrdB, A, B> :
    Functor<Set<OrdA, A>, Set<OrdB, B>, A, B>,
    Applicative<Set<OrdFAB, Func<A, B>>, Set<OrdA, A>, Set<OrdB, B>, A, B>
    where OrdA : Ord<A>
    where OrdB : Ord<B>
    where OrdFAB : Ord<Func<A,B>>
{
    [Pure]
    public static Set<OrdB, B> Action(Set<OrdA, A> fa, Set<OrdB, B> fb)
    {
        IEnumerable<B> Yield()
        {
            foreach(var _ in fa)
            {
                foreach(var b in fb)
                {
                    yield return b;
                }
            }
        }
        return new Set<OrdB, B>(Yield(), true);
    }

    [Pure]
    public static Set<OrdB, B> Apply(Set<OrdFAB, Func<A, B>> fab, Set<OrdA, A> fa)
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
        return new Set<OrdB, B>(Yield(), true);
    }

    [Pure]
    public static Set<OrdB, B> Map(Set<OrdA, A> ma, Func<A, B> f) =>
        FSet<OrdA, OrdB, A, B>.Inst.Map(ma, f);

    [Pure]
    public static Set<OrdA, A> Pure(A x) =>
        Set<OrdA, A>(x);
}

public struct ApplSet<OrdFABC, OrdFBC, OrdA, OrdB, OrdC, A, B, C> :
    Applicative<Set<OrdFABC, Func<A, Func<B, C>>>, Set<OrdFBC, Func<B, C>>, Set<OrdA, A>, Set<OrdB, B>, Set<OrdC, C>, A, B>
    where OrdA : Ord<A>
    where OrdB : Ord<B>
    where OrdC : Ord<C>
    where OrdFABC : Ord<Func<A, Func<B, C>>>
    where OrdFBC : Ord<Func<B, C>>
{
    [Pure]
    public static Set<OrdFBC, Func<B, C>> Apply(Set<OrdFABC, Func<A, Func<B, C>>> fabc, Set<OrdA, A> fa)
    {
        IEnumerable<Func<B,C>> Yield()
        {
            foreach (var f in fabc)
            {
                foreach (var a in fa)
                {
                    yield return f(a);
                }
            }
        }
        return new Set<OrdFBC, Func<B, C>>(Yield(), true);
    }

    [Pure]
    public static Set<OrdC, C> Apply(Set<OrdFABC, Func<A, Func<B, C>>> fabc, Set<OrdA, A> fa, Set<OrdB, B> fb)
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
        return new Set<OrdC, C>(Yield(), true);
    }

    [Pure]
    public static Set<OrdA, A> Pure(A x) =>
        Set<OrdA, A>(x);
}

public struct ApplSet<OrdFAAA, OrdFAA, OrdA, A> :
    Applicative<Set<OrdFAA, Func<A, A>>, Set<OrdA, A>, Set<OrdA, A>, A, A>,
    Applicative<Set<OrdFAAA, Func<A, Func<A, A>>>, Set<OrdFAA, Func<A, A>>, Set<OrdA, A>, Set<OrdA, A>, Set<OrdA, A>, A, A>
    where OrdA : Ord<A>
    where OrdFAAA : Ord<Func<A, Func<A, A>>>
    where OrdFAA : Ord<Func<A, A>>
{
    [Pure]
    public static Set<OrdA, A> Action(Set<OrdA, A> fa, Set<OrdA, A> fb)
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
        return new Set<OrdA, A>(Yield(), true);
    }

    [Pure]
    public static Set<OrdA, A> Apply(Set<OrdFAA, Func<A, A>> fab, Set<OrdA, A> fa)
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
        return new Set<OrdA, A>(Yield(), true);
    }

    [Pure]
    public static Set<OrdFAA, Func<A, A>> Apply(Set<OrdFAAA, Func<A, Func<A, A>>> fabc, Set<OrdA, A> fa)
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
        return new Set<OrdFAA, Func<A, A>>(Yield(), true);
    }
    [Pure]
    public static Set<OrdA, A> Apply(Set<OrdFAAA, Func<A, Func<A, A>>> fabc, Set<OrdA, A> fa, Set<OrdA, A> fb)
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
        return new Set<OrdA, A>(Yield(), true);
    }

    [Pure]
    public static Set<OrdA, A> Pure(A x) =>
        Set<OrdA, A>(x);
}
