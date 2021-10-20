using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplEnumerable<A, B> :
        Functor<IEnumerable<A>, IEnumerable<B>, A, B>,
        Applicative<IEnumerable<Func<A, B>>, IEnumerable<A>, IEnumerable<B>, A, B>
    {
        public static readonly ApplEnumerable<A, B> Inst = default(ApplEnumerable<A, B>);

        [Pure]
        public IEnumerable<B> Action(IEnumerable<A> fa, IEnumerable<B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public IEnumerable<B> Apply(IEnumerable<Func<A, B>> fab, IEnumerable<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public IEnumerable<B> Map(IEnumerable<A> ma, Func<A, B> f) =>
            FEnumerable<A, B>.Inst.Map(ma, f);

        [Pure]
        public IEnumerable<A> Pure(A x) =>
            List.create(x);
    }

    public struct ApplEnumerable<A, B, C> :
        Applicative<IEnumerable<Func<A, Func<B, C>>>, IEnumerable<Func<B, C>>, IEnumerable<A>, IEnumerable<B>, IEnumerable<C>, A, B, C>
    {
        public static readonly ApplEnumerable<A, B, C> Inst = default(ApplEnumerable<A, B, C>);

        [Pure]
        public IEnumerable<Func<B, C>> Apply(IEnumerable<Func<A, Func<B, C>>> fabc, IEnumerable<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public IEnumerable<C> Apply(IEnumerable<Func<A, Func<B, C>>> fabc, IEnumerable<A> fa, IEnumerable<B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public IEnumerable<A> Pure(A x) =>
            List.create(x);
    }

    public struct ApplEnumerable<A> :
        Applicative<IEnumerable<Func<A, A>>, IEnumerable<A>, IEnumerable<A>, A, A>,
        Applicative<IEnumerable<Func<A, Func<A, A>>>, IEnumerable<Func<A, A>>, IEnumerable<A>, IEnumerable<A>, IEnumerable<A>, A, A, A>
    {
        public static readonly ApplEnumerable<A> Inst = default(ApplEnumerable<A>);

        [Pure]
        public IEnumerable<A> Action(IEnumerable<A> fa, IEnumerable<A> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public IEnumerable<A> Apply(IEnumerable<Func<A, A>> fab, IEnumerable<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public IEnumerable<Func<A, A>> Apply(IEnumerable<Func<A, Func<A, A>>> fabc, IEnumerable<A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        [Pure]
        public IEnumerable<A> Apply(IEnumerable<Func<A, Func<A, A>>> fabc, IEnumerable<A> fa, IEnumerable<A> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public IEnumerable<A> Pure(A x) =>
            List.create(x);
    }
}
