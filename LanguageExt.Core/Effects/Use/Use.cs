using System;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt;

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

    Use(Func<A> make, Func<A, Unit> dispose)
    {
        this.make = make;
        this.dispose = dispose;
    }

    public static Use<A> New(Func<A> make, Func<A, Unit> dispose) =>
        new (make, dispose);

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
}
