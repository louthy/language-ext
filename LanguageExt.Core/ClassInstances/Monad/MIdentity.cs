using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.Core.ClassInstances.MonadPlus
{
    public struct MIdentity<A> : Monad<Identity<A>, A>
    {
        public MB Bind<MONADB, MB, B>(Identity<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            f(ma.Value);

        public int Count(Identity<A> fa) =>
            1;

        public Identity<A> Fail(Exception err = null) =>
            Identity<A>.Bottom;

        public Identity<A> Fail(object err) =>
            Identity<A>.Bottom;

        public S Fold<S>(Identity<A> fa, S state, Func<S, A, S> f) =>
            f(state, fa.Value);

        public S FoldBack<S>(Identity<A> fa, S state, Func<S, A, S> f) =>
            f(state, fa.Value);

        public Identity<A> Plus(Identity<A> a, Identity<A> b) =>
            a.IsBottom
                ? b
                : a;

        public Identity<A> Return(A x) =>
            new Identity<A>(x);

        public Identity<A> Zero() =>
            Identity<A>.Bottom;
    }
}
