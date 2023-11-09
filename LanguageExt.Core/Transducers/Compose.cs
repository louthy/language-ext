#nullable enable
namespace LanguageExt.Transducers;

record ComposeTransducer<TA, TB, TC>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G) : 
    Transducer<TA, TC>
{
    public Reducer<S, TA> Transform<S>(Reducer<S, TC> reduce) =>
        F.Transform(
            G.Transform(reduce));

    public Transducer<TA, TC> Morphism =>
        this;
}

record ComposeTransducer<TA, TB, TC, TD>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G, 
    Transducer<TC, TD> H) : 
    Transducer<TA, TD>
{
    public Reducer<S, TA> Transform<S>(Reducer<S, TD> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(reduce)));

    public Transducer<TA, TD> Morphism =>
        this;
}

record ComposeTransducer<TA, TB, TC, TD, TE>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G, 
    Transducer<TC, TD> H, 
    Transducer<TD, TE> I) : 
    Transducer<TA, TE>
{
    public Reducer<S, TA> Transform<S>(Reducer<S, TE> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(
                    I.Transform(reduce))));

    public Transducer<TA, TE> Morphism =>
        this;
}

record ComposeTransducer<TA, TB, TC, TD, TE, TF>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G, 
    Transducer<TC, TD> H, 
    Transducer<TD, TE> I, 
    Transducer<TE, TF> J) : 
    Transducer<TA, TF>
{
    public Reducer<S, TA> Transform<S>(Reducer<S, TF> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(
                    I.Transform(
                        J.Transform(reduce)))));

    public Transducer<TA, TF> Morphism =>
        this;
}

record ComposeTransducer<TA, TB, TC, TD, TE, TF, TG>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G, 
    Transducer<TC, TD> H, 
    Transducer<TD, TE> I, 
    Transducer<TE, TF> J, 
    Transducer<TF, TG> K) : 
    Transducer<TA, TG>
{
    public Reducer<S, TA> Transform<S>(Reducer<S, TG> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(
                    I.Transform(
                        J.Transform(
                            K.Transform(reduce))))));

    public Transducer<TA, TG> Morphism =>
        this;
}
