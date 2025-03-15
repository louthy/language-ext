namespace LanguageExt;

record ComposeTransducer<A, B, C>(Transducer<A, B> TF, Transducer<B, C> TG) : 
    Transducer<A, C> 
{
    public override Reducer<A, S> Reduce<S>(Reducer<C, S> reducer) =>
        (s, x) => TF.Reduce<S>((s1, y) =>  TG.Reduce(reducer)(s1, y))(s, x);

    public override ReducerM<M, A, S> ReduceM<M, S>(ReducerM<M, C, S> reducer) =>
        (s, x) => TF.ReduceM<M, S>((s1, y) => TG.ReduceM(reducer)(s1, y))(s, x);

    public override Transducer<A, D> Compose<D>(Transducer<C, D> t) =>
        new ComposeTransducer<A, B, C, D>(TF, TG, t);
}

record ComposeTransducer<A, B, C, D>(Transducer<A, B> TF, Transducer<B, C> TG, Transducer<C, D> TH) : 
    Transducer<A, D>
{
    public override Reducer<A, S> Reduce<S>(Reducer<D, S> reducer) =>
        (s, x) => TF.Reduce<S>((s1, y) => TG.Reduce<S>((s2, z) => TH.Reduce(reducer)(s2, z))(s1, y))(s, x);

    public override ReducerM<M, A, S> ReduceM<M, S>(ReducerM<M, D, S> reducer) => 
        (s, x) => TF.ReduceM<M, S>((s1, y) => TG.ReduceM<M, S>((s2, z) => TH.ReduceM(reducer)(s2, z))(s1, y))(s, x);

    public override Transducer<A, E> Compose<E>(Transducer<D, E> t) =>
        new ComposeTransducer<A, B, C, D, E>(TF, TG, TH, t);
}

record ComposeTransducer<A, B, C, D, E>(
    Transducer<A, B> TF, 
    Transducer<B, C> TG, 
    Transducer<C, D> TH, 
    Transducer<D, E> TI) : 
    Transducer<A, E>
{
    public override Reducer<A, S> Reduce<S>(Reducer<E, S> reducer) =>
        (s, w) => TF.Reduce<S>(
                (s1, x) => TG.Reduce<S>(
                        (s2, y) => TH.Reduce<S>(
                                (s3, z) => TI.Reduce(reducer)(s3, z))
                            (s2, y))
                    (s1, x))
            (s, w);

    public override ReducerM<M, A, S> ReduceM<M, S>(ReducerM<M, E, S> reducer) => 
        (s, w) => TF.ReduceM<M, S>(
                (s1, x) => TG.ReduceM<M, S>(
                        (s2, y) => TH.ReduceM<M, S>(
                                (s3, z) => TI.ReduceM(reducer)(s3, z))
                            (s2, y))
                    (s1, x))
            (s, w);

    public override Transducer<A, F> Compose<F>(Transducer<E, F> t) =>
        new ComposeTransducer<A, B, C, D, E, F>(TF, TG, TH, TI, t);
}

record ComposeTransducer<A, B, C, D, E, F>(
    Transducer<A, B> TF, 
    Transducer<B, C> TG, 
    Transducer<C, D> TH, 
    Transducer<D, E> TI, 
    Transducer<E, F> TJ) : 
    Transducer<A, F>
{
    public override Reducer<A, S> Reduce<S>(Reducer<F, S> reducer) =>
        (s, v) => TF.Reduce<S>(
                (s1, w) => TG.Reduce<S>(
                        (s2, x) => TH.Reduce<S>(
                                (s3, y) => TI.Reduce<S>(
                                        (s4, z) => TJ.Reduce(reducer)(s4, z))
                                    (s3, y))
                            (s2, x))
                    (s1, w))
            (s, v);

    public override ReducerM<M, A, S> ReduceM<M, S>(ReducerM<M, F, S> reducer) => 
        (s, v) => TF.ReduceM<M, S>(
                (s1, w) => TG.ReduceM<M, S>(
                        (s2, x) => TH.ReduceM<M, S>(
                                (s3, y) => TI.ReduceM<M, S>(
                                        (s4, z) => TJ.ReduceM(reducer)(s4, z))
                                    (s3, y))
                            (s2, x))
                    (s1, w))
            (s, v);

    public override Transducer<A, G> Compose<G>(Transducer<F, G> t) =>
        new ComposeTransducer<A, B, C, D, E, F, G>(TF, TG, TH, TI, TJ, t);
}

record ComposeTransducer<A, B, C, D, E, F, G>(
    Transducer<A, B> TF, 
    Transducer<B, C> TG, 
    Transducer<C, D> TH, 
    Transducer<D, E> TI, 
    Transducer<E, F> TJ, 
    Transducer<F, G> TK) : 
    Transducer<A, G>
{
    public override Reducer<A, S> Reduce<S>(Reducer<G, S> reducer) =>
        (s, u) => TF.Reduce<S>(
                (s1, v) => TG.Reduce<S>(
                        (s2, w) => TH.Reduce<S>(
                                (s3, x) => TI.Reduce<S>(
                                        (s4, y) => TJ.Reduce<S>(
                                                (s5, z) => TK.Reduce(reducer)(s5, z))
                                            (s4, y))
                                    (s3, x))
                            (s2, w))
                    (s1, v))
            (s, u);

    public override ReducerM<M, A, S> ReduceM<M, S>(ReducerM<M, G, S> reducer) => 
        (s, u) => TF.ReduceM<M, S>(
                (s1, v) => TG.ReduceM<M, S>(
                        (s2, w) => TH.ReduceM<M, S>(
                                (s3, x) => TI.ReduceM<M, S>(
                                        (s4, y) => TJ.ReduceM<M, S>(
                                                (s5, z) => TK.ReduceM(reducer)(s5, z))
                                            (s4, y))
                                    (s3, x))
                            (s2, w))
                    (s1, v))
            (s, u);

}
