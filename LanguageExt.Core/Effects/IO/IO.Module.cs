using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class IO
{
    /// <summary>
    /// Put the IO into a failure state
    /// </summary>
    /// <param name="value">Error value</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IO in a failed state.  Always yields an error.</returns>
    public static IO<A> Fail<A>(Error value) =>
        IO<A>.Fail(value);
    
    public static IO<Unit> lift(Action f) =>
        lift(() =>
             {
                 f();
                 return unit;
             });
    
    public static IO<A> lift<A>(Func<A> f) => 
        IO<A>.Lift(f);
    
    public static IO<A> lift<A>(Func<EnvIO, A> f) => 
        IO<A>.Lift(f);

    public static IO<A> liftAsync<A>(Func<ValueTask<A>> f) => 
        IO<A>.LiftAsync(f);

    public static IO<A> liftAsync<A>(Func<EnvIO, ValueTask<A>> f) => 
        IO<A>.LiftAsync(f);

    public static readonly IO<CancellationToken> token = 
        lift(e => e.Token);
    
    public static readonly IO<CancellationTokenSource> source = 
        lift(e => e.Source);
    
    public static readonly IO<Option<SynchronizationContext>> syncContext = 
        lift(e => Optional(e.SyncContext));
    
    public static IO<B> bind<A, B>(K<IO, A> ma, Func<A, K<IO, B>> f) =>
        ma.As().Bind(f);

    public static IO<B> map<A, B>(Func<A, B> f, K<IO, A> ma) => 
        ma.As().Map(f);

    public static IO<B> apply<A, B>(K<IO, Func<A, B>> mf, K<IO, A> ma) => 
        mf.As().Bind(ma.As().Map);

    public static IO<B> action<A, B>(K<IO, A> ma, K<IO, B> mb) =>
        ma.As().Bind(_ => mb);

    public static IO<A> empty<A>() =>
        IO<A>.Empty;

    public static IO<A> or<A>(K<IO, A> ma, K<IO, A> mb) => 
        ma.As() | mb.As();
}
