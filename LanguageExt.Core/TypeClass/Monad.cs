using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    public interface Monad<A> : Applicative<A>
    {
        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="value">The bound monad value</param>
        /// <returns>Monad of A</returns>
        Monad<A> Return(A value);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="bind">Bind function</param>
        /// <returns>Monad of B</returns>
        Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> bind);

        /// <summary>
        /// Produce a failure value
        /// </summary>
        Monad<A> Fail();
    }

    public static class Extensions
    {
        public static Functor<U> Select<T, U>(
            this Functor<T> self,
            Func<T, U> map
            ) =>
            self.Map(self, map);

        public static Monad<V> SelectMany<T, U, V>(
            this Monad<T> self,
            Func<T, Monad<U>> bind,
            Func<T, U, V> project) =>
            self.Bind(self, t => (Monad<V>)bind(t).Select(u => project(t, u)));

        public static Monad<V> Join<L, T, U, K, V>(
            this Monad<T> self,
            Monad<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, U, V> project) =>
            from w in
                (from x in self
                 from y in inner
                 select EqualityComparer<K>.Default.Equals(outerKeyMap(x), innerKeyMap(y))
                     ? (Monad<V>)self.Select( _ => project(x,y))
                     : ((Monad<V>)self.Select( _ => default(V))).Fail())
            from res in w
            select res;
    }
}
