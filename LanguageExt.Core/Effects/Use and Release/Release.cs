/*
#nullable enable
using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt.Effects;

public static class Release
{
    public static Release<A> New<A>(A resource) =>
        Release<A>.New(resource);
}

public readonly struct Release<A>
{
    readonly A resource;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Construction
    //

    Release(A resource) =>
        this.resource = resource;

    public static Release<A> New(A release) =>
        new (release);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //

    public IO<RT, E, Unit> ToIO<RT, E>()
        where RT : HasIO<RT, E> =>
        new (Transducer.compose(
                Transducer.constant<RT, A>(resource), 
                Transducer.release<A>(), 
                Transducer.mkRight<E, Unit>()));
    
    public Eff<RT, Unit> ToEff<RT>()
        where RT : HasIO<RT, Error> =>
        throw new NotImplementedException("TODO");
    
    public Eff<Unit> ToEff() =>
        throw new NotImplementedException("TODO");

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    public IO<RT, E, B> Bind<RT, E, B>(Func<Unit, IO<RT, E, B>> bind)
        where RT : HasIO<RT, E> =>
        ToIO<RT, E>().Bind(bind);

    public Eff<RT, B> Bind<RT, B>(Func<Unit, Eff<RT, B>> bind)
        where RT : HasIO<RT, Error> =>
        ToEff<RT>().Bind(bind);

    public Eff<B> Bind<B>(Func<Unit, Eff<B>> bind) =>
        ToEff().Bind(bind);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    public IO<RT, E, C> SelectMany<RT, E, B, C>(Func<Unit, IO<RT, E, B>> bind, Func<Unit, B, C> project)
        where RT : HasIO<RT, E> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<RT, C> SelectMany<RT, B, C>(Func<Unit, Eff<RT, B>> bind, Func<Unit, B, C> project)
        where RT : HasIO<RT, Error> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<C> SelectMany<B, C>(Func<Unit, Eff<B>> bind, Func<Unit, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}
*/
