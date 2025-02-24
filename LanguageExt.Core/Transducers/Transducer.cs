using System;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract record Transducer<A, B> : K<Transducer<A>, B>
{
    public abstract Reducer<A, S> Reduce<S>(Reducer<B, S> reducer);

    public virtual Transducer<A, C> Compose<C>(
        Transducer<B, C> tg) =>
        new ComposeTransducer<A, B, C>(this, tg);

    public Transducer<A, C> Map<C>(Func<B, C> f) =>
        Compose(new MapTransducer<B, C>(f));

    public Transducer<A, C> Bind<C>(Func<B, K<Transducer<A>, C>> f) =>
        new BindTransducer1<A, B, C>(this, f);

    public Transducer<A, C> Bind<C>(Func<B, Transducer<A, C>> f) =>
        new BindTransducer2<A, B, C>(this, f);

    public Transducer<A, D> SelectMany<C, D>(Func<B, K<Transducer<A>, C>> bind, Func<B, C, D> project) =>
        new SelectManyTransducer1<A, B, C, D>(this, bind, project);

    public Transducer<A, D> SelectMany<C, D>(Func<B, Transducer<A, C>> bind, Func<B, C, D> project) =>
        new SelectManyTransducer2<A, B, C, D>(this, bind, project);
}
