using System;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt;

public static class Release
{
    public static Release<A> New<A>(A resource) =>
        Release<A>.New(resource);
}

public readonly struct Release<A>
{
    readonly A resource;

    Release(A resource) =>
        this.resource = resource;

    public static Release<A> New(A release) =>
        new (release);

    public IO<RT, E, Unit> ToIO<RT, E>()
        where RT : struct, HasIO<RT, E> =>
        new (Transducer.compose(
                Transducer.constant<RT, A>(resource), 
                Transducer.release<A>(), 
                Transducer.mkRight<E, Unit>()));
}
