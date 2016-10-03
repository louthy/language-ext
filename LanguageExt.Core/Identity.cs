using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
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

        public A Value
        {
            get
            {
                if (IsBottom) throw new BottomException();
                return value;
            }
        }

        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f) =>
            f(Id(ma));

        public MB Bind<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B> =>
            f(Id(ma));

        public Monad<A> Fail(Exception err = null) => 
            default(Identity<A>);

        public Monad<A> Fail<F>(F err = default(F)) =>
            default(Identity<A>);

        private A Id(Functor<A> fa) =>
            ((Identity<A>)fa).value;

        private A Id(Monad<A> fa) =>
            ((Identity<A>)fa).value;

        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            new Identity<B>(f(Id(fa)));

        public Monad<A> Return(A x, params A[] xs) =>
            new Identity<A>(x);

        public Monad<A> Return(IEnumerable<A> xs) =>
            new Identity<A>(xs.First());

        public S Fold<S>(Foldable<A> fa, S state, Func<S, A, S> f) =>
            f(state, value);

        public S FoldBack<S>(Foldable<A> fa, S state, Func<S, A, S> f) =>
            f(state, value);
    }
}
