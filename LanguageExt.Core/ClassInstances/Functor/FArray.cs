using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FArray<A, B> :
        Functor<A[], B[], A, B>,
        Applicative<Func<A, B>[], A[], B[], A, B>
    {
        public static readonly FArray<A, B> Inst = default(FArray<A, B>);

        [Pure]
        public B[] Action(A[] fa, B[] fb) =>
            (from a in fa
             from b in fb
             select b).ToArray();

        [Pure]
        public B[] Apply(Func<A, B>[] fab, A[] fa) =>
            (from f in fab
             from a in fa
             select f(a)).ToArray();

        [Pure]
        public B[] Map(A[] ma, Func<A, B> f)
        {
            var bs = new B[ma.Length];
            var iter = ma.AsEnumerable().GetEnumerator();
            for (int i = 0; iter.MoveNext(); i++)
            {
                bs[i] = f(iter.Current);
            }
            return bs;
        }

        [Pure]
        public A[] Pure(A x) =>
            new A[] { x };
    }

    public struct FArray<A, B, C> :
        Applicative<Func<A, Func<B, C>>[], Func<B, C>[], A[], B[], C[], A, B, C>
    {
        public static readonly FArray<A, B, C> Inst = default(FArray<A, B, C>);

        [Pure]
        public Func<B, C>[] Apply(Func<A, Func<B, C>>[] fabc, A[] fa) =>
            (from f in fabc
             from a in fa
             select f(a)).ToArray();

        [Pure]
        public C[] Apply(Func<A, Func<B, C>>[] fabc, A[] fa, B[] fb) =>
            (from f in fabc
             from a in fa
             from b in fb
             select f(a)(b)).ToArray();

        [Pure]
        public A[] Pure(A x) =>
            new[] { x };
    }

    public struct FArray<A> :
        Applicative<Func<A, A>[], A[], A[], A, A>,
        Applicative<Func<A, Func<A, A>>[], Func<A, A>[], A[], A[], A[], A, A, A>
    {
        public static readonly FArray<A> Inst = default(FArray<A>);

        [Pure]
        public A[] Action(A[] fa, A[] fb) =>
            (from a in fa
             from b in fb
             select b).ToArray();

        [Pure]
        public A[] Apply(Func<A, A>[] fab, A[] fa) =>
            (from f in fab
             from a in fa
             select f(a)).ToArray();

        [Pure]
        public Func<A, A>[] Apply(Func<A, Func<A, A>>[] fabc, A[] fa) =>
            (from f in fabc
             from a in fa
             select f(a)).ToArray();

        [Pure]
        public A[] Apply(Func<A, Func<A, A>>[] fabc, A[] fa, A[] fb) =>
            (from f in fabc
             from a in fa
             from b in fb
             select f(a)(b)).ToArray();

        [Pure]
        public A[] Pure(A x) =>
            new[] { x };
    }
}
