using System;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.Pipes;

/// <summary>
/// Effects represent a 'fused' set of producer, pipes, and consumer into one type.
/// 
/// It neither can neither `yield` nor be `awaiting`, it represents an entirely closed effect system.
/// </summary>
/// <remarks>
///       Upstream | Downstream
///           +---------+
///           |         |
///     Void <==       <== Unit
///           |         |
///     Unit ==>       ==> Void
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public static class Effect
{
    [Pure]
    internal static Transducer<RT, Sum<Error, R>> RunEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) 
        where RT : HasIO<RT, Error> 
    {
        return Go(ma);
            
        Transducer<RT, Sum<Error, R>> Go(Proxy<RT, Void, Unit, Unit, Void, R> p) =>
            p.ToProxy() switch
            {
                M<RT, Void, Unit, Unit, Void, R> (var mx)         => mx.MapRight(Go).Flatten(),
                Pure<RT, Void, Unit, Unit, Void, R> (var r)       => Transducer.constant<RT, Sum<Error, R>>(Sum<Error, R>.Right(r)),                                                                                
                Request<RT, Void, Unit, Unit, Void, R> (var v, _) => closed<Transducer<RT, Sum<Error, R>>>(v),
                Respond<RT, Void, Unit, Unit, Void, R> (var v, _) => closed<Transducer<RT, Sum<Error, R>>>(v),
                _                                                 => throw new NotSupportedException()
            };
    }        
        
    [Pure, MethodImpl(mops)]
    public static Effect<RT, R> lift<RT, R>(Eff<R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

    [Pure, MethodImpl(mops)]
    public static Effect<RT, R> lift<RT, R>(Eff<RT, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

    [Pure, MethodImpl(mops)]
    public static Effect<RT, R> lift<RT, R>(Transducer<RT, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

    [Pure, MethodImpl(mops)]
    public static Effect<RT, R> lift<RT, R>(Transducer<RT, Sum<Error, R>> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

    [Pure, MethodImpl(mops)]
    public static Effect<RT, R> lift<RT, R>(Transducer<Unit, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

    [Pure, MethodImpl(mops)]
    public static Effect<RT, R> lift<RT, R>(Transducer<Unit, Sum<Error, R>> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

    [Obsolete(Change.UseEffMonadInsteadOfAff)]
    [Pure, MethodImpl(mops)]
    public static Effect<RT, R> lift<RT, R>(Aff<R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Void, Unit, Unit, Void, R>(ma.ToTransducer()).ToEffect();

    [Obsolete(Change.UseEffMonadInsteadOfAff)]
    [Pure, MethodImpl(mops)]
    public static Effect<RT, R> lift<RT, R>(Aff<RT, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Void, Unit, Unit, Void, R>(ma.ToTransducer()).ToEffect();
}
