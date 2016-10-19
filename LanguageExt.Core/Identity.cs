using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Identity monad
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct Identity<A> : Monad<A>
    {
        internal static Identity<A> Default = new Identity<A>(default(A));

        public readonly A value;
        public readonly bool IsBottom;

        Identity(A value)
        {
            this.value = value;
            IsBottom = false;
        }

        [Pure]
        public A Value
        {
            get
            {
                if (IsBottom) throw new BottomException();
                return value;
            }
        }

        [Pure]
        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f) =>
            f(Id(ma));

        [Pure]
        public MB Bind<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B> =>
            f(Id(ma));

        [Pure]
        public Monad<A> Fail(Exception err = null) => 
            default(Identity<A>);

        [Pure]
        public Monad<A> Fail<F>(F err = default(F)) =>
            default(Identity<A>);

        [Pure]
        private A Id(Functor<A> fa) =>
            ((Identity<A>)fa).value;

        [Pure]
        private A Id(Monad<A> fa) =>
            ((Identity<A>)fa).value;

        [Pure]
        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            new Identity<B>(f(Id(fa)));

        [Pure]
        public Monad<A> Return(A x, params A[] xs) =>
            new Identity<A>(x);

        [Pure]
        public Monad<A> Return(IEnumerable<A> xs) =>
            new Identity<A>(xs.First());

        [Pure]
        public S Fold<S>(Foldable<A> fa, S state, Func<S, A, S> f) =>
            f(state, value);

        [Pure]
        public S FoldBack<S>(Foldable<A> fa, S state, Func<S, A, S> f) =>
            f(state, value);
    }
}
