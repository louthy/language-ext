using System;
using System.Threading.Tasks;

namespace LanguageExt.DSL;

record IOMap<A, B>(A Value, Func<A, B> Ff) : DslInvokeIO<B>
{
    public override IODsl<C> Map<C>(Func<B, C> f) =>
        new IOMap<A, B, C>(Value, Ff, f);

    public override B Invoke(EnvIO envIO) =>
        Ff(Value);
}

record IOMap<A, B, C>(A Value, Func<A, B> Ff, Func<B, C> Fg) : DslInvokeIO<C>
{
    public override IODsl<D> Map<D>(Func<C, D> f) =>
        new IOMap<A, B, C, D>(Value, Ff, Fg, f);

    public override C Invoke(EnvIO envIO) =>
        Fg(Ff(Value));
}

record IOMap<A, B, C, D>(A Value, Func<A, B> Ff, Func<B, C> Fg, Func<C, D> Fh) : DslInvokeIO<D>
{
    public override IODsl<E> Map<E>(Func<D, E> f) =>
        new IOMap<A, B, C, D, E>(Value, Ff, Fg, Fh, f);

    public override D Invoke(EnvIO envIO) =>
        Fh(Fg(Ff(Value)));
}

record IOMap<A, B, C, D, E>(A Value, Func<A, B> Ff, Func<B, C> Fg, Func<C, D> Fh, Func<D, E> Fi) : DslInvokeIO<E>
{
    public override IODsl<F> Map<F>(Func<E, F> f) =>
        new IOMap<A, B, C, D, F>(Value, Ff, Fg, Fh, x => f(Fi(x)));

    public override E Invoke(EnvIO envIO) =>
        Fi(Fh(Fg(Ff(Value))));
}
