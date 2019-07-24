using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Sequentially composes two lenses
        /// </summary>
        public static Lens<A, C> lens<A, B, C>(Lens<A, B> la, Lens<B, C> lb) =>
            Lens<A, C>.New(
                Get: a => lb.Get(la.Get(a)),
                Set: v => la.Update(lb.SetF(v)));

        /// <summary>
        /// Sequentially composes three lenses
        /// </summary>
        public static Lens<A, D> lens<A, B, C, D>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc) =>
            Lens<A, D>.New(
                Get: a => lc.Get(lb.Get(la.Get(a))),
                Set: v => la.Update(lb.Update(lc.SetF(v))));

        /// <summary>
        /// Sequentially composes four lenses
        /// </summary>
        public static Lens<A, E> lens<A, B, C, D, E>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld) =>
            Lens<A, E>.New(
                Get: a => ld.Get(lc.Get(lb.Get(la.Get(a)))),
                Set: v => la.Update(lb.Update(lc.Update(ld.SetF(v)))));

        /// <summary>
        /// Sequentially composes five lenses
        /// </summary>
        public static Lens<A, F> lens<A, B, C, D, E, F>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le) =>
            Lens<A, F>.New(
                Get: a => le.Get(ld.Get(lc.Get(lb.Get(la.Get(a))))),
                Set: v => la.Update(lb.Update(lc.Update(ld.Update(le.SetF(v))))));

        /// <summary>
        /// Sequentially composes six lenses
        /// </summary>
        public static Lens<A, G> lens<A, B, C, D, E, F, G>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le, Lens<F, G> lf) =>
            Lens<A, G>.New(
                Get: a => lf.Get(le.Get(ld.Get(lc.Get(lb.Get(la.Get(a)))))),
                Set: v => la.Update(lb.Update(lc.Update(ld.Update(le.Update(lf.SetF(v)))))));

        /// <summary>
        /// Sequentially composes seven lenses
        /// </summary>
        public static Lens<A, H> lens<A, B, C, D, E, F, G, H>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le, Lens<F, G> lf, Lens<G, H> lg) =>
            Lens<A, H>.New(
                Get: a => lg.Get(lf.Get(le.Get(ld.Get(lc.Get(lb.Get(la.Get(a))))))),
                Set: v => la.Update(lb.Update(lc.Update(ld.Update(le.Update(lf.Update(lg.SetF(v))))))));

        /// <summary>
        /// Sequentially composes eight lenses
        /// </summary>
        public static Lens<A, I> lens<A, B, C, D, E, F, G, H, I>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le, Lens<F, G> lf, Lens<G, H> lg, Lens<H, I> lh) =>
            Lens<A, I>.New(
                Get: a => lh.Get(lg.Get(lf.Get(le.Get(ld.Get(lc.Get(lb.Get(la.Get(a)))))))),
                Set: v => la.Update(lb.Update(lc.Update(ld.Update(le.Update(lf.Update(lg.Update(lh.SetF(v)))))))));

        /// <summary>
        /// Sequentially composes nine lenses
        /// </summary>
        public static Lens<A, J> lens<A, B, C, D, E, F, G, H, I, J>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le, Lens<F, G> lf, Lens<G, H> lg, Lens<H, I> lh, Lens<I, J> li) =>
            Lens<A, J>.New(
                Get: a => li.Get(lh.Get(lg.Get(lf.Get(le.Get(ld.Get(lc.Get(lb.Get(la.Get(a))))))))),
                Set: v => la.Update(lb.Update(lc.Update(ld.Update(le.Update(lf.Update(lg.Update(lh.Update(li.SetF(v))))))))));

        /// <summary>
        /// Sequentially composes ten lenses
        /// </summary>
        public static Lens<A, K> lens<A, B, C, D, E, F, G, H, I, J, K>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le, Lens<F, G> lf, Lens<G, H> lg, Lens<H, I> lh, Lens<I, J> li, Lens<J, K> lj) =>
            Lens<A, K>.New(
                Get: a => lj.Get(li.Get(lh.Get(lg.Get(lf.Get(le.Get(ld.Get(lc.Get(lb.Get(la.Get(a)))))))))),
                Set: v => la.Update(lb.Update(lc.Update(ld.Update(le.Update(lf.Update(lg.Update(lh.Update(li.Update(lj.SetF(v)))))))))));
    }
}
