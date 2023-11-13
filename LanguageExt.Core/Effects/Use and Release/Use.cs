#nullable enable
using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt.Effects;

public static class Use
{
    public static Use<A> New<A>(Func<A> make, Func<A, Unit> dispose) =>
        Use<A>.New(make, dispose);

    public static Use<A> New<A>(Func<A> make) where A : IDisposable =>
        New(make, x => { x.Dispose(); return default;});
}

public readonly struct Use<A>
{
    readonly Func<A> make;
    readonly Func<A, Unit> dispose;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Construction
    //

    Use(Func<A> make, Func<A, Unit> dispose)
    {
        this.make = make;
        this.dispose = dispose;
    }

    public static Use<A> New(Func<A> make, Func<A, Unit> dispose) =>
        new (make, dispose);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //

    public Transducer<Unit, A> ToTransducer()
    {
        var mk = make;
        return Transducer.use(Transducer.lift<Unit, A>(_ => mk()), dispose);
    }
    
    public Transducer<E, A> ToTransducer<E>()
    {
        var mk = make;
        return Transducer.use(Transducer.lift<E, A>(_ => mk()), dispose);
    }

    public IO<RT, E, A> ToIO<RT, E>()
        where RT : struct, HasIO<RT, E> =>
        new (Transducer.compose(ToTransducer<RT>(), Transducer.mkRight<E, A>()));

    public Eff<RT, A> ToEff<RT>()
        where RT : struct, HasIO<RT, Error> =>
        throw new NotImplementedException("TODO");

    public Eff<A> ToEff() =>
        throw new NotImplementedException("TODO");
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    public IO<RT, E, B> Bind<RT, E, B>(Func<A, IO<RT, E, B>> bind)
        where RT : struct, HasIO<RT, E> =>
        ToIO<RT, E>().Bind(bind);

    public Eff<RT, B> Bind<RT, B>(Func<A, Eff<RT, B>> bind)
        where RT : struct, HasIO<RT, Error> =>
        ToEff<RT>().Bind(bind);

    public Eff<B> Bind<B>(Func<A, Eff<B>> bind) =>
        ToEff().Bind(bind);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    public IO<RT, E, C> SelectMany<RT, E, B, C>(Func<A, IO<RT, E, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, E> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<RT, C> SelectMany<RT, B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project)
        where RT : struct, HasIO<RT, Error> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Eff<C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
}
