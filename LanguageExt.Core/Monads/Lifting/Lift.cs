
using System;
using System.Threading.Tasks;
using LanguageExt.HKT;

namespace LanguageExt;


/// <summary>
/// Lifts a function into a type that can be used with other
/// monadic types (in LINQ expressions, for example) and with implicit
/// conversions.
/// </summary>
public record Lift<A>(Func<A> Function)
{
    public ReaderT<Env, M, A> ToReaderT<Env, M>() 
        where M : Monad<M> =>
        new (_ => M.Pure(Function()));

    public AsyncT<M, A> ToAsyncT<M>() 
        where M : Monad<M> =>
        new (_ => ValueTask.FromResult(M.Pure(Function())));

    public Lift<B> Map<B>(Func<A, B> f) =>
        new (() => f(Function()));

    public Lift<B> Bind<B>(Func<A, Lift<B>> f) =>
        new(() => f(Function()).Function());

    public AsyncT<M, B> Bind<M, B>(Func<A, AsyncT<M, B>> f) where M : Monad<M> =>
        ToAsyncT<M>().Bind(f);

    public Lift<B> Bind<B>(Func<A, Pure<B>> f) =>
        new (() => f(Function()).Value);

    public Lift<B> Select<B>(Func<A, B> f) =>
        Map(f);

    public AsyncT<M, C> SelectMany<M, B, C>(Func<A, AsyncT<M, B>> bind, Func<A, B, C> project) 
        where M : Monad<M> =>
        ToAsyncT<M>().SelectMany(bind, project);

    public ReaderT<Env, M, C> SelectMany<Env, M, B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project) 
        where M : Monad<M> =>
        ToReaderT<Env, M>().SelectMany(bind, project);

    public Lift<C> SelectMany<B, C>(Func<A, Lift<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Lift<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}
