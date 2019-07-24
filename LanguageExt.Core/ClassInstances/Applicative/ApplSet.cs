using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplSet<A, B> :
        Functor<Set<A>, Set<B>, A, B>,
        Applicative<Set<Func<A, B>>, Set<A>, Set<B>, A, B>
    {
        public static readonly ApplSet<A, B> Inst = default(ApplSet<A, B>);

        [Pure]
        public Set<B> Action(Set<A> fa, Set<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Set<B> Apply(Set<Func<A, B>> fab, Set<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Set<B> Map(Set<A> ma, Func<A, B> f) =>
            FSet<A, B>.Inst.Map(ma, f);

        [Pure]
        public Set<A> Pure(A x) =>
            Set(x);
    }

    public struct ApplSet<A, B, C> :
        Applicative<Set<Func<A, Func<B, C>>>, Set<Func<B, C>>, Set<A>, Set<B>, Set<C>, A, B, C>
    {
        public static readonly ApplSet<A, B, C> Inst = default(ApplSet<A, B, C>);

        [Pure]
        public Set<Func<B, C>> Apply(Set<Func<A, Func<B, C>>> fabc, Set<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Set<C> Apply(Set<Func<A, Func<B, C>>> fabc, Set<A> fa, Set<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Set<A> Pure(A x) =>
            Set(x);
    }

    public struct ApplSet<A> :
        Applicative<Set<Func<A, A>>, Set<A>, Set<A>, A, A>,
        Applicative<Set<Func<A, Func<A, A>>>, Set<Func<A, A>>, Set<A>, Set<A>, Set<A>, A, A, A>
    {
        public static readonly ApplSet<A> Inst = default(ApplSet<A>);

        [Pure]
        public Set<A> Action(Set<A> fa, Set<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public Set<A> Apply(Set<Func<A, A>> fab, Set<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public Set<Func<A, A>> Apply(Set<Func<A, Func<A, A>>> fabc, Set<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public Set<A> Apply(Set<Func<A, Func<A, A>>> fabc, Set<A> fa, Set<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public Set<A> Pure(A x) =>
            Set(x);
    }


    public struct ApplSet<OrdFAB, OrdA, OrdB, A, B> :
        Functor<Set<OrdA, A>, Set<OrdB, B>, A, B>,
        Applicative<Set<OrdFAB, Func<A, B>>, Set<OrdA, A>, Set<OrdB, B>, A, B>
        where OrdA : struct, Ord<A>
        where OrdB : struct, Ord<B>
        where OrdFAB : struct, Ord<Func<A,B>>
    {
        public static readonly ApplSet<OrdFAB, OrdA, OrdB, A, B> Inst = default(ApplSet<OrdFAB, OrdA, OrdB, A, B>);

        [Pure]
        public Set<OrdB, B> Action(Set<OrdA, A> fa, Set<OrdB, B> fb)
        {
            IEnumerable<B> Yield()
            {
                foreach(var a in fa)
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
        public Set<OrdB, B> Apply(Set<OrdFAB, Func<A, B>> fab, Set<OrdA, A> fa)
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
        public Set<OrdB, B> Map(Set<OrdA, A> ma, Func<A, B> f) =>
            FSet<OrdA, OrdB, A, B>.Inst.Map(ma, f);

        [Pure]
        public Set<OrdA, A> Pure(A x) =>
            Set<OrdA, A>(x);
    }

    public struct ApplSet<OrdFABC, OrdFBC, OrdA, OrdB, OrdC, A, B, C> :
        Applicative<Set<OrdFABC, Func<A, Func<B, C>>>, Set<OrdFBC, Func<B, C>>, Set<OrdA, A>, Set<OrdB, B>, Set<OrdC, C>, A, B, C>
        where OrdA : struct, Ord<A>
        where OrdB : struct, Ord<B>
        where OrdC : struct, Ord<C>
        where OrdFABC : struct, Ord<Func<A, Func<B, C>>>
        where OrdFBC : struct, Ord<Func<B, C>>
    {
        public static readonly ApplSet<OrdFABC, OrdFBC, OrdA, OrdB, OrdC, A, B, C> Inst = default(ApplSet<OrdFABC, OrdFBC, OrdA, OrdB, OrdC, A, B, C>);

        [Pure]
        public Set<OrdFBC, Func<B, C>> Apply(Set<OrdFABC, Func<A, Func<B, C>>> fabc, Set<OrdA, A> fa)
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
        public Set<OrdC, C> Apply(Set<OrdFABC, Func<A, Func<B, C>>> fabc, Set<OrdA, A> fa, Set<OrdB, B> fb)
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
        public Set<OrdA, A> Pure(A x) =>
            Set<OrdA, A>(x);
    }

    public struct ApplSet<OrdFAAA, OrdFAA, OrdA, A> :
        Applicative<Set<OrdFAA, Func<A, A>>, Set<OrdA, A>, Set<OrdA, A>, A, A>,
        Applicative<Set<OrdFAAA, Func<A, Func<A, A>>>, Set<OrdFAA, Func<A, A>>, Set<OrdA, A>, Set<OrdA, A>, Set<OrdA, A>, A, A, A>
        where OrdA : struct, Ord<A>
        where OrdFAAA : struct, Ord<Func<A, Func<A, A>>>
        where OrdFAA : struct, Ord<Func<A, A>>
    {
        public static readonly ApplSet<A> Inst = default(ApplSet<A>);

        [Pure]
        public Set<OrdA, A> Action(Set<OrdA, A> fa, Set<OrdA, A> fb)
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
            return new Set<OrdA, A>(Yield(), true);
        }

        [Pure]
        public Set<OrdA, A> Apply(Set<OrdFAA, Func<A, A>> fab, Set<OrdA, A> fa)
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
        public Set<OrdFAA, Func<A, A>> Apply(Set<OrdFAAA, Func<A, Func<A, A>>> fabc, Set<OrdA, A> fa)
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
        public Set<OrdA, A> Apply(Set<OrdFAAA, Func<A, Func<A, A>>> fabc, Set<OrdA, A> fa, Set<OrdA, A> fb)
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
        public Set<OrdA, A> Pure(A x) =>
            Set<OrdA, A>(x);
    }

}
