namespace LanguageExt;

record ComposeTransducerM<M, A, B, C>(TransducerM<M, A, B> TF, TransducerM<M, B, C> TG) : 
    TransducerM<M, A, C> 
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, C, S> reducer) =>
        (s, x) => TF.Reduce<S>((s1, y) => TG.Reduce(reducer)(s1, y))(s, x);

    public override TransducerM<M, A, D> Compose<D>(TransducerM<M, C, D> t) =>
        new ComposeTransducerM<M, A, B, C, D>(TF, TG, t);
}

record ComposeTransducerM<M, A, B, C, D>(TransducerM<M, A, B> TF, TransducerM<M, B, C> TG, TransducerM<M, C, D> TH) : 
    TransducerM<M, A, D>
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, D, S> reducer) => 
        (s, x) => TF.Reduce<S>((s1, y) => TG.Reduce<S>((s2, z) => TH.Reduce(reducer)(s2, z))(s1, y))(s, x);

    public override TransducerM<M, A, E> Compose<E>(TransducerM<M, D, E> t) =>
        new ComposeTransducerM<M, A, B, C, D, E>(TF, TG, TH, t);
}

record ComposeTransducerM<M, A, B, C, D, E>(
    TransducerM<M, A, B> TF, 
    TransducerM<M, B, C> TG, 
    TransducerM<M, C, D> TH, 
    TransducerM<M, D, E> TI) : 
    TransducerM<M, A, E>
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, E, S> reducer) => 
        (s, w) => TF.Reduce<S>(
                (s1, x) => TG.Reduce<S>(
                        (s2, y) => TH.Reduce<S>(
                                (s3, z) => TI.Reduce(reducer)(s3, z))
                            (s2, y))
                    (s1, x))
            (s, w);

    public override TransducerM<M, A, F> Compose<F>(TransducerM<M, E, F> t) =>
        new ComposeTransducerM<M, A, B, C, D, E, F>(TF, TG, TH, TI, t);
}

record ComposeTransducerM<M, A, B, C, D, E, F>(
    TransducerM<M, A, B> TF, 
    TransducerM<M, B, C> TG, 
    TransducerM<M, C, D> TH, 
    TransducerM<M, D, E> TI, 
    TransducerM<M, E, F> TJ) : 
    TransducerM<M, A, F>
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, F, S> reducer) => 
        (s, v) => TF.Reduce<S>(
                (s1, w) => TG.Reduce<S>(
                        (s2, x) => TH.Reduce<S>(
                                (s3, y) => TI.Reduce<S>(
                                        (s4, z) => TJ.Reduce(reducer)(s4, z))
                                    (s3, y))
                            (s2, x))
                    (s1, w))
            (s, v);

    public override TransducerM<M, A, G> Compose<G>(TransducerM<M, F, G> t) =>
        new ComposeTransducerM<M, A, B, C, D, E, F, G>(TF, TG, TH, TI, TJ, t);
}

record ComposeTransducerM<M, A, B, C, D, E, F, G>(
    TransducerM<M, A, B> TF, 
    TransducerM<M, B, C> TG, 
    TransducerM<M, C, D> TH, 
    TransducerM<M, D, E> TI, 
    TransducerM<M, E, F> TJ, 
    TransducerM<M, F, G> TK) : 
    TransducerM<M, A, G>
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, G, S> reducer) => 
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

}
