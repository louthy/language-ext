using System;
using LanguageExt.Traits;

namespace LanguageExt;

record IOBindMap<A, B, C>(A Value, Func<A, K<IO, B>> Ff, Func<B, C> Fg) : InvokeSyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOBindMap<A, B, C, D>(Value, Ff, Fg, f);

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOBindMap2<A, B, C, D>(Value, Ff, Fg, f);

    public override IO<C> Invoke(EnvIO envIO) =>
        Ff(Value).As().Map(Fg);
}

record IOBindBind<A, B, C>(A Value, Func<A, K<IO, B>> Ff, Func<B, K<IO, C>> Fg) : InvokeSyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOBindBindMap<A, B, C, D>(Value, Ff, Fg, f);

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOBindBind<A, B, D>(Value, Ff, x => Fg(x).Bind(f));

    public override IO<C> Invoke(EnvIO envIO) =>
        Ff(Value).As().Bind(Fg);
}

record IOBindBindMap<A, B, C, D>(A Value, Func<A, K<IO, B>> Ff, Func<B, K<IO, C>> Fg, Func<C, D> Fh) : InvokeSyncIO<D>
{
    public override IO<E> Map<E>(Func<D, E> f) => 
        new IOBindBindMap<A, B, C, E>(Value, Ff, Fg, x => f(Fh(x)));

    public override IO<E> Bind<E>(Func<D, K<IO, E>> f) => 
        new IOBindBindMapBind<A, B, C, D, E>(Value, Ff, Fg, Fh, f);

    public override IO<D> Invoke(EnvIO envIO) =>
        Ff(Value).As().Bind(Fg).Map(Fh);
}

record IOBindBindMapBind<A, B, C, D, E>(A Value, Func<A, K<IO, B>> Ff, Func<B, K<IO, C>> Fg, Func<C, D> Fh, Func<D, K<IO, E>> Fi) : InvokeSyncIO<E>
{
    public override IO<F> Map<F>(Func<E, F> f) => 
        new IOBindBindMapBind<A, B, C, D, F>(Value, Ff, Fg, Fh, x => Fi(x).Map(f));

    public override IO<F> Bind<F>(Func<E, K<IO, F>> f) => 
        new IOBindBindMapBind<A, B, C, D, F>(Value, Ff, Fg, Fh, x => Fi(x).Bind(f));

    public override IO<E> Invoke(EnvIO envIO) =>
        Ff(Value).As().Bind(Fg).Map(Fh).Bind(Fi);
}

record IOBindMap<A, B, C, D>(A Value, Func<A, K<IO, B>> Ff, Func<B, C> Fg, Func<C, D> Fh) : InvokeSyncIO<D>
{
    public override IO<E> Map<E>(Func<D, E> f) =>
        new IOBindMap<A, B, C, E>(Value, Ff, Fg, x => f(Fh(x)));

    public override IO<E> Bind<E>(Func<D, K<IO, E>> f) => 
        new IOBindMap2<A, B, C, E>(Value, Ff, Fg, x => f(Fh(x)));

    public override IO<D> Invoke(EnvIO envIO) =>
        Ff(Value).As().Map(Fg).Map(Fh);
}

record IOBindMap2<A, B, C, D>(A Value, Func<A, K<IO, B>> Ff, Func<B, C> Fg, Func<C, K<IO, D>> Fh) : InvokeSyncIO<D>
{
    public override IO<E> Map<E>(Func<D, E> f) =>
        new IOBindMap2<A, B, C, E>(Value, Ff, Fg, x => Fh(x).Map(f));

    public override IO<E> Bind<E>(Func<D, K<IO, E>> f) =>
        new IOBindMap2<A, B, C, E>(Value, Ff, Fg, x => Fh(x).As().Bind(f));

    public override IO<D> Invoke(EnvIO envIO) =>
        Ff(Value).As().Map(Fg).Bind(Fh);
}
