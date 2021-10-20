using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplHashSet<A, B> :
        Functor<HashSet<A>, HashSet<B>, A, B>,
        Applicative<HashSet<Func<A, B>>, HashSet<A>, HashSet<B>, A, B>
    {
        public static readonly ApplHashSet<A, B> Inst = default(ApplHashSet<A, B>);

        [Pure]
        public HashSet<B> Action(HashSet<A> fa, HashSet<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public HashSet<B> Apply(HashSet<Func<A, B>> fab, HashSet<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public HashSet<B> Map(HashSet<A> ma, Func<A, B> f) =>
            FHashSet<A, B>.Inst.Map(ma, f);

        [Pure]
        public HashSet<A> Pure(A x) =>
            HashSet(x);
    }

    public struct ApplHashSet<A, B, C> :
        Applicative<HashSet<Func<A, Func<B, C>>>, HashSet<Func<B, C>>, HashSet<A>, HashSet<B>, HashSet<C>, A, B, C>
    {
        public static readonly ApplHashSet<A, B, C> Inst = default(ApplHashSet<A, B, C>);

        [Pure]
        public HashSet<Func<B, C>> Apply(HashSet<Func<A, Func<B, C>>> fabc, HashSet<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public HashSet<C> Apply(HashSet<Func<A, Func<B, C>>> fabc, HashSet<A> fa, HashSet<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public HashSet<A> Pure(A x) =>
            HashSet(x);
    }

    public struct ApplHashSet<A> :
        Applicative<HashSet<Func<A, A>>, HashSet<A>, HashSet<A>, A, A>,
        Applicative<HashSet<Func<A, Func<A, A>>>, HashSet<Func<A, A>>, HashSet<A>, HashSet<A>, HashSet<A>, A, A, A>
    {
        public static readonly ApplHashSet<A> Inst = default(ApplHashSet<A>);

        [Pure]
        public HashSet<A> Action(HashSet<A> fa, HashSet<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public HashSet<A> Apply(HashSet<Func<A, A>> fab, HashSet<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public HashSet<Func<A, A>> Apply(HashSet<Func<A, Func<A, A>>> fabc, HashSet<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public HashSet<A> Apply(HashSet<Func<A, Func<A, A>>> fabc, HashSet<A> fa, HashSet<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public HashSet<A> Pure(A x) =>
            HashSet(x);
    }


    public struct ApplHashSet<OrdFAB, OrdA, OrdB, A, B> :
        Functor<HashSet<OrdA, A>, HashSet<OrdB, B>, A, B>,
        Applicative<HashSet<OrdFAB, Func<A, B>>, HashSet<OrdA, A>, HashSet<OrdB, B>, A, B>
        where OrdA : struct, Ord<A>
        where OrdB : struct, Ord<B>
        where OrdFAB : struct, Ord<Func<A, B>>
    {
        public static readonly ApplHashSet<OrdFAB, OrdA, OrdB, A, B> Inst = default(ApplHashSet<OrdFAB, OrdA, OrdB, A, B>);

        [Pure]
        public HashSet<OrdB, B> Action(HashSet<OrdA, A> fa, HashSet<OrdB, B> fb)
        {
            IEnumerable<B> Yield()
            {
                foreach (var a in fa)
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
        public HashSet<OrdB, B> Apply(HashSet<OrdFAB, Func<A, B>> fab, HashSet<OrdA, A> fa)
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
        public HashSet<OrdB, B> Map(HashSet<OrdA, A> ma, Func<A, B> f) =>
            ma.Map<OrdB, B>(f);

        [Pure]
        public HashSet<OrdA, A> Pure(A x) =>
            HashSet<OrdA, A>(x);
    }

    public struct ApplHashSet<OrdFABC, OrdFBC, OrdA, OrdB, OrdC, A, B, C> :
        Applicative<HashSet<OrdFABC, Func<A, Func<B, C>>>, HashSet<OrdFBC, Func<B, C>>, HashSet<OrdA, A>, HashSet<OrdB, B>, HashSet<OrdC, C>, A, B, C>
        where OrdA : struct, Ord<A>
        where OrdB : struct, Ord<B>
        where OrdC : struct, Ord<C>
        where OrdFABC : struct, Ord<Func<A, Func<B, C>>>
        where OrdFBC : struct, Ord<Func<B, C>>
    {
        public static readonly ApplHashSet<OrdFABC, OrdFBC, OrdA, OrdB, OrdC, A, B, C> Inst = default(ApplHashSet<OrdFABC, OrdFBC, OrdA, OrdB, OrdC, A, B, C>);

        [Pure]
        public HashSet<OrdFBC, Func<B, C>> Apply(HashSet<OrdFABC, Func<A, Func<B, C>>> fabc, HashSet<OrdA, A> fa)
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
        public HashSet<OrdC, C> Apply(HashSet<OrdFABC, Func<A, Func<B, C>>> fabc, HashSet<OrdA, A> fa, HashSet<OrdB, B> fb)
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
        public HashSet<OrdA, A> Pure(A x) =>
            HashSet<OrdA, A>(x);
    }

    public struct ApplHashSet<OrdFAAA, OrdFAA, OrdA, A> :
        Applicative<HashSet<OrdFAA, Func<A, A>>, HashSet<OrdA, A>, HashSet<OrdA, A>, A, A>,
        Applicative<HashSet<OrdFAAA, Func<A, Func<A, A>>>, HashSet<OrdFAA, Func<A, A>>, HashSet<OrdA, A>, HashSet<OrdA, A>, HashSet<OrdA, A>, A, A, A>
        where OrdA : struct, Ord<A>
        where OrdFAAA : struct, Ord<Func<A, Func<A, A>>>
        where OrdFAA : struct, Ord<Func<A, A>>
    {
        public static readonly ApplHashSet<A> Inst = default(ApplHashSet<A>);

        [Pure]
        public HashSet<OrdA, A> Action(HashSet<OrdA, A> fa, HashSet<OrdA, A> fb)
        {
            IEnumerable<A> Yield()
            {
                foreach (var a in fa)
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
        public HashSet<OrdA, A> Apply(HashSet<OrdFAA, Func<A, A>> fab, HashSet<OrdA, A> fa)
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
        public HashSet<OrdFAA, Func<A, A>> Apply(HashSet<OrdFAAA, Func<A, Func<A, A>>> fabc, HashSet<OrdA, A> fa)
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
        public HashSet<OrdA, A> Apply(HashSet<OrdFAAA, Func<A, Func<A, A>>> fabc, HashSet<OrdA, A> fa, HashSet<OrdA, A> fb)
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
        public HashSet<OrdA, A> Pure(A x) =>
            HashSet<OrdA, A>(x);
    }
}
