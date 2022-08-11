namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Convert a lens to a prism
        /// </summary>
        public static Prism<A, B> prism<A, B>(Lens<A, B> la) =>
            Prism<A, B>.New(la);

        /// <summary>
        /// Convert a lens to a prism
        /// </summary>
        public static Prism<A, B> prism<A, B>(Lens<A, Option<B>> la) => 
            Prism<A, B>.New(la);

        /// <summary>
        /// Sequentially composes two prisms
        /// </summary>
        public static Prism<A, C> prism<A, B, C>(Prism<A, B> pa, Prism<B, C> pb) => 
            Prism<A, C>.New(
                Get: a => pa.Get(a).Bind(pb.Get),
                Set: v => pa.Update(pb.SetF(v)));

        /// <summary>
        /// Sequentially composes three prisms
        /// </summary>
        public static Prism<A, D> prism<A, B, C, D>(Prism<A, B> pa, Prism<B, C> pb, Prism<C, D> pc) =>
            Prism<A, D>.New(
                Get: a => pa.Get(a).Bind(pb.Get).Bind(pc.Get),
                Set: v => pa.Update(pb.Update(pc.SetF(v))));

        /// <summary>
        /// Sequentially composes four prisms
        /// </summary>
        public static Prism<A, E> prism<A, B, C, D, E>(Prism<A, B> pa, Prism<B, C> pb, Prism<C, D> pc, Prism<D, E> pd) =>
            Prism<A, E>.New(
                Get: a => pa.Get(a).Bind(pb.Get).Bind(pc.Get).Bind(pd.Get),
                Set: v => pa.Update(pb.Update(pc.Update(pd.SetF(v)))));

        /// <summary>
        /// Sequentially composes five prisms
        /// </summary>
        public static Prism<A, F> prism<A, B, C, D, E, F>(Prism<A, B> pa, Prism<B, C> pb, Prism<C, D> pc, Prism<D, E> pd, Prism<E, F> pe) =>
            Prism<A, F>.New(
                Get: a => pa.Get(a).Bind(pb.Get).Bind(pc.Get).Bind(pd.Get).Bind(pe.Get),
                Set: v => pa.Update(pb.Update(pc.Update(pd.Update(pe.SetF(v))))));

        /// <summary>
        /// Sequentially composes six prisms
        /// </summary>
        public static Prism<A, G> prism<A, B, C, D, E, F, G>(Prism<A, B> pa, Prism<B, C> pb, Prism<C, D> pc, Prism<D, E> pd, Prism<E, F> pe, Prism<F, G> pf) =>
            Prism<A, G>.New(
                Get: a => pa.Get(a).Bind(pb.Get).Bind(pc.Get).Bind(pd.Get).Bind(pe.Get).Bind(pf.Get),
                Set: v => pa.Update(pb.Update(pc.Update(pd.Update(pe.Update(pf.SetF(v)))))));

        /// <summary>
        /// Sequentially composes seven prisms
        /// </summary>
        public static Prism<A, H> prism<A, B, C, D, E, F, G, H>(Prism<A, B> pa, Prism<B, C> pb, Prism<C, D> pc, Prism<D, E> pd, Prism<E, F> pe, Prism<F, G> pf, Prism<G, H> pg) =>
            Prism<A, H>.New(
                Get: a => pa.Get(a).Bind(pb.Get).Bind(pc.Get).Bind(pd.Get).Bind(pe.Get).Bind(pf.Get).Bind(pg.Get),
                Set: v => pa.Update(pb.Update(pc.Update(pd.Update(pe.Update(pf.Update(pg.SetF(v))))))));

        /// <summary>
        /// Sequentially composes eight prisms
        /// </summary>
        public static Prism<A, I> prism<A, B, C, D, E, F, G, H, I>(Prism<A, B> pa, Prism<B, C> pb, Prism<C, D> pc, Prism<D, E> pd, Prism<E, F> pe, Prism<F, G> pf, Prism<G, H> pg, Prism<H, I> ph) =>
            Prism<A, I>.New(
                Get: a => pa.Get(a).Bind(pb.Get).Bind(pc.Get).Bind(pd.Get).Bind(pe.Get).Bind(pf.Get).Bind(pg.Get).Bind(ph.Get),
                Set: v => pa.Update(pb.Update(pc.Update(pd.Update(pe.Update(pf.Update(pg.Update(ph.SetF(v)))))))));

        /// <summary>
        /// Sequentially composes nine prisms
        /// </summary>
        public static Prism<A, J> prism<A, B, C, D, E, F, G, H, I, J>(Prism<A, B> pa, Prism<B, C> pb, Prism<C, D> pc, Prism<D, E> pd, Prism<E, F> pe, Prism<F, G> pf, Prism<G, H> pg, Prism<H, I> ph, Prism<I, J> pi) =>
            Prism<A, J>.New(
                Get: a => pa.Get(a).Bind(pb.Get).Bind(pc.Get).Bind(pd.Get).Bind(pe.Get).Bind(pf.Get).Bind(pg.Get).Bind(ph.Get).Bind(pi.Get),
                Set: v => pa.Update(pb.Update(pc.Update(pd.Update(pe.Update(pf.Update(pg.Update(ph.Update(pi.SetF(v))))))))));

        /// <summary>
        /// Sequentially composes ten prisms
        /// </summary>
        public static Prism<A, K> prism<A, B, C, D, E, F, G, H, I, J, K>(Prism<A, B> pa, Prism<B, C> pb, Prism<C, D> pc, Prism<D, E> pd, Prism<E, F> pe, Prism<F, G> pf, Prism<G, H> pg, Prism<H, I> ph, Prism<I, J> pi, Prism<J, K> pj) =>
            Prism<A, K>.New(
                Get: a => pa.Get(a).Bind(pb.Get).Bind(pc.Get).Bind(pd.Get).Bind(pe.Get).Bind(pf.Get).Bind(pg.Get).Bind(ph.Get).Bind(pi.Get).Bind(pj.Get),
                Set: v => pa.Update(pb.Update(pc.Update(pd.Update(pe.Update(pf.Update(pg.Update(ph.Update(pi.Update(pj.SetF(v)))))))))));
    }
}
