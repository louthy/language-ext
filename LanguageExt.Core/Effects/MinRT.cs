#nullable enable
using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Effects;

public static class MinRTExtensions
{
    public static MinRT ToMin<RT>(this RT rt) where RT : struct, HasIO<RT, Error> =>
        new (rt.SynchronizationContext, rt.CancellationTokenSource, rt.CancellationToken);
    
    public static MinRT<E> ToMin<RT, E>(this RT rt) where RT : struct, HasIO<RT, E> =>
        new (rt.FromError, rt.SynchronizationContext, rt.CancellationTokenSource, rt.CancellationToken);
}

/// <summary>
/// Minimal runtime for running the non-runtime based IO monads
/// </summary>
public readonly struct MinRT : 
    HasIO<MinRT, Error>
{
    /// <summary>
    /// Get the transducer that converts from a `HasIO` supporting runtime to a `MinRT`
    /// </summary>
    public static Transducer<RT, MinRT> convert<RT>() 
        where RT : struct, HasIO<RT, Error> =>
        lift<RT, MinRT>(rt => rt.ToMin());
    
    public MinRT(
        SynchronizationContext syncContext,
        CancellationTokenSource cancellationTokenSource,
        CancellationToken cancellationToken) =>
        (SynchronizationContext, CancellationTokenSource, CancellationToken) = 
            (syncContext, cancellationTokenSource, cancellationToken);

    public MinRT(
        SynchronizationContext syncContext,
        CancellationTokenSource cancellationTokenSource) =>
        (SynchronizationContext, CancellationTokenSource, CancellationToken) = 
            (syncContext, cancellationTokenSource, cancellationTokenSource.Token);

    public MinRT()
    {
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;
        SynchronizationContext = SynchronizationContext.Current;
    }

    public MinRT LocalCancel =>
        new (SynchronizationContext, CancellationTokenSource);
    
    public SynchronizationContext SynchronizationContext { get; }
    public CancellationToken CancellationToken { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    
    public Error FromError(Error error) => 
        error;

    public MinRT WithSyncContext(SynchronizationContext syncContext) =>
        new (syncContext, CancellationTokenSource);
}

/// <summary>
/// Minimal runtime for running the non-runtime based IO monads
/// </summary>
public readonly struct MinRT<E> : 
    HasIO<MinRT<E>, E>
{
    /// <summary>
    /// Get the transducer that converts from a `HasIO` supporting runtime to a `MinRT`
    /// </summary>
    public static Transducer<RT, MinRT<E>> convert<RT>() 
        where RT : struct, HasIO<RT, E> =>
        lift<RT, MinRT<E>>(rt => rt.ToMin<RT, E>());
    
    public MinRT(
        Func<Error, E> errorMap,
        SynchronizationContext syncContext,
        CancellationTokenSource cancellationTokenSource,
        CancellationToken cancellationToken) =>
        (ErrorMap, SynchronizationContext, CancellationTokenSource, CancellationToken) = 
        (errorMap, syncContext, cancellationTokenSource, cancellationToken);

    public MinRT(
        Func<Error, E> errorMap,
        SynchronizationContext syncContext,
        CancellationTokenSource cancellationTokenSource) =>
        (ErrorMap, SynchronizationContext, CancellationTokenSource, CancellationToken) = 
        (errorMap, syncContext, cancellationTokenSource, cancellationTokenSource.Token);

    public MinRT(Func<Error, E> errorMap)
    {
        ErrorMap                = errorMap;
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken       = CancellationTokenSource.Token;
        SynchronizationContext  = SynchronizationContext.Current;
    }

    public MinRT<E> LocalCancel =>
        new (ErrorMap, SynchronizationContext, CancellationTokenSource);

    public readonly Func<Error, E> ErrorMap;
    public SynchronizationContext SynchronizationContext { get; }
    public CancellationToken CancellationToken { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    
    public E FromError(Error error) =>
        ErrorMap switch
        {
            null => throw new ValueIsNullException($"{nameof(MinRT)}.{nameof(ErrorMap)} isn't set.  This implies that the {nameof(MinRT)} runtime isn't initialised."),
            var f => f(error)
        };

    public MinRT<E> WithSyncContext(SynchronizationContext syncContext) =>
        new (ErrorMap, syncContext, CancellationTokenSource);
}
