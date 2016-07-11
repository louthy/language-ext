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

        public Applicative<B> Bind<B>(Applicative<A> ma, Func<A, Applicative<B>> f) =>
            f(Id(ma));

        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f) =>
            f(Id(ma));

        public Monad<A> Fail(string err = "") => 
            default(Identity<A>);

        private A Id(Functor<A> fa) =>
            ((Identity<A>)fa).value;

        private A Id(Monad<A> fa) =>
            ((Identity<A>)fa).value;

        private A Id(Applicative<A> fa) =>
            ((Identity<A>)fa).value;

        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            new Identity<B>(f(Id(fa)));

        public Applicative<A> Pure(A a) =>
            new Identity<A>(a);

        public Monad<A> Return(A a) =>
            new Identity<A>(a);
    }
}
