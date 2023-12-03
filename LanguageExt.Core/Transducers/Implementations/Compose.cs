#nullable enable
namespace LanguageExt.Transducers;

record ComposeTransducer<TA, TB, TC>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G) : 
    Transducer<TA, TC>
{
    public override Reducer<TA, S> Transform<S>(Reducer<TC, S> reduce) =>
        F.Transform(
            G.Transform(reduce));

    public override string ToString() =>  
        "a → b → c";

    public override Transducer<TA, TD> Compose<TD>(Transducer<TC, TD> next) =>
        new ComposeTransducer<TA, TB, TC, TD>(F, G, next);
}

record ComposeTransducer<TA, TB, TC, TD>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G, 
    Transducer<TC, TD> H) : 
    Transducer<TA, TD>
{
    public override Reducer<TA, S> Transform<S>(Reducer<TD, S> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(reduce)));

    public override string ToString() =>  
        "a → b → c → d";

    public override Transducer<TA, TE> Compose<TE>(Transducer<TD, TE> next) =>
        new ComposeTransducer<TA, TB, TC, TD, TE>(F, G, H, next);
}

record ComposeTransducer<TA, TB, TC, TD, TE>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G, 
    Transducer<TC, TD> H, 
    Transducer<TD, TE> I) : 
    Transducer<TA, TE>
{
    public override Reducer<TA, S> Transform<S>(Reducer<TE, S> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(
                    I.Transform(reduce))));

    public override string ToString() =>  
        "a → b → c → d → e";

    public override Transducer<TA, TF> Compose<TF>(Transducer<TE, TF> next) =>
        new ComposeTransducer<TA, TB, TC, TD, TE, TF>(F, G, H, I, next);
}

record ComposeTransducer<TA, TB, TC, TD, TE, TF>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G, 
    Transducer<TC, TD> H, 
    Transducer<TD, TE> I, 
    Transducer<TE, TF> J) : 
    Transducer<TA, TF>
{
    public override Reducer<TA, S> Transform<S>(Reducer<TF, S> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(
                    I.Transform(
                        J.Transform(reduce)))));

    public override string ToString() =>  
        "a → b → c → d → e → f";

    public override Transducer<TA, TG> Compose<TG>(Transducer<TF, TG> next) =>
        new ComposeTransducer<TA, TB, TC, TD, TE, TF, TG>(F, G, H, I, J, next);
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
    public override Reducer<TA, S> Transform<S>(Reducer<TG, S> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(
                    I.Transform(
                        J.Transform(
                            K.Transform(reduce))))));

    public override string ToString() =>  
        "a → b → c → d → e → f → g";

    public override Transducer<TA, TH> Compose<TH>(Transducer<TG, TH> next) =>
        new ComposeTransducer<TA, TB, TC, TD, TE, TF, TG, TH>(F, G, H, I, J, K, next);
}

record ComposeTransducer<TA, TB, TC, TD, TE, TF, TG, TH>(
    Transducer<TA, TB> F, 
    Transducer<TB, TC> G, 
    Transducer<TC, TD> H, 
    Transducer<TD, TE> I, 
    Transducer<TE, TF> J, 
    Transducer<TF, TG> K,
    Transducer<TG, TH> L) : 
    Transducer<TA, TH>
{
    public override Reducer<TA, S> Transform<S>(Reducer<TH, S> reduce) =>
        F.Transform(
            G.Transform(
                H.Transform(
                    I.Transform(
                        J.Transform(
                            K.Transform(
                                L.Transform(reduce)))))));

    public override string ToString() =>  
        "a → b → c → d → e → f → g → h";

    public override Transducer<TA, TI> Compose<TI>(Transducer<TH, TI> next) =>
        new ComposeTransducer<TA, TB, TC, TD, TE, TF, TG, TI>(F, G, H, I, J, K, L.Compose(next));
}
