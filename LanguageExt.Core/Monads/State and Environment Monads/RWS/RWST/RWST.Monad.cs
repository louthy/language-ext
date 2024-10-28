using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Reader / Write / State monad-transformer trait implementations
/// </summary>
/// <typeparam name="R">Reader environment type</typeparam>
/// <typeparam name="W">Writer output type</typeparam>
/// <typeparam name="S">State type</typeparam>
/// <typeparam name="M">Lifted monad type</typeparam>
public class RWST<R, W, S, M> :
    MonadT<RWST<R, W, S, M>, M>, 
    Choice<RWST<R, W, S, M>>,
    Readable<RWST<R, W, S, M>, R>,
    Writable<RWST<R, W, S, M>, W>,
    Stateful<RWST<R, W, S, M>, S>
    where M : Monad<M>, Choice<M>
    where W : Monoid<W>
{
    static K<RWST<R, W, S, M>, B> Monad<RWST<R, W, S, M>>.Bind<A, B>(
        K<RWST<R, W, S, M>, A> ma,
        Func<A, K<RWST<R, W, S, M>, B>> f) =>
        new RWST<R, W, S, M, B>(
            input => ma.As().runRWST(input)
                       .Bind(x => f(x.Value).As().runRWST((input.Env, x.Output, x.State))));

    static K<RWST<R, W, S, M>, B> Functor<RWST<R, W, S, M>>.Map<A, B>(
        Func<A, B> f, 
        K<RWST<R, W, S, M>, A> ma) => 
        new RWST<R, W, S, M, B>(
            input => ma.As().runRWST(input)
                       .Map(x => (f(x.Value), x.Output, x.State)));

    static K<RWST<R, W, S, M>, A> Applicative<RWST<R, W, S, M>>.Pure<A>(A value) => 
        new RWST<R, W, S, M, A>(input => M.Pure((value, input.Output, input.State)));

    static K<RWST<R, W, S, M>, B> Applicative<RWST<R, W, S, M>>.Apply<A, B>(
        K<RWST<R, W, S, M>, Func<A, B>> mf,
        K<RWST<R, W, S, M>, A> ma) =>
        new RWST<R, W, S, M, B>(
            input => from rf in mf.As().runRWST(input)
                     from ra in ma.As().runRWST((input.Env, rf.Output, rf.State))
                     select (rf.Value(ra.Value), ra.Output, ra.State));

    static K<RWST<R, W, S, M>, A> MonadT<RWST<R, W, S, M>, M>.Lift<A>(K<M, A> ma) => 
        new RWST<R, W, S, M, A>(input => ma.Map(value => (value, input.Output, input.State)));

    static K<RWST<R, W, S, M>, A> SemigroupK<RWST<R, W, S, M>>.Combine<A>(
        K<RWST<R, W, S, M>, A> lhs, 
        K<RWST<R, W, S, M>, A> rhs) => 
        new RWST<R, W, S, M, A>(input => lhs.As().runRWST(input).Combine(rhs.As().runRWST(input)));

    static K<RWST<R, W, S, M>, A> Choice<RWST<R, W, S, M>>.Choose<A>(
        K<RWST<R, W, S, M>, A> lhs, 
        K<RWST<R, W, S, M>, A> rhs) => 
        new RWST<R, W, S, M, A>(input => lhs.As().runRWST(input).Choose(rhs.As().runRWST(input)));

    static K<RWST<R, W, S, M>, A> Readable<RWST<R, W, S, M>, R>.Asks<A>(Func<R, A> f) => 
        new RWST<R, W, S, M, A>(input => M.Pure((f(input.Env), input.Output, input.State)));

    static K<RWST<R, W, S, M>, A> Readable<RWST<R, W, S, M>, R>.Local<A>(
        Func<R, R> f,
        K<RWST<R, W, S, M>, A> ma) =>
        new RWST<R, W, S, M, A>(
            input => ma.As().runRWST((f(input.Env), input.Output, input.State)));

    static K<RWST<R, W, S, M>, Unit> Writable<RWST<R, W, S, M>, W>.Tell(W item) => 
        new RWST<R, W, S, M, Unit>(
            input => M.Pure((unit, input.Output.Combine(item), input.State)));

    static K<RWST<R, W, S, M>, (A Value, W Output)> Writable<RWST<R, W, S, M>, W>.Listen<A>(
        K<RWST<R, W, S, M>, A> ma) =>
        new RWST<R, W, S, M, (A Value, W Output)>(
            input => ma.As()
                       .runRWST(input)
                       .Map(output => ((output.Value, output.Output), output.Output, output.State)));

    static K<RWST<R, W, S, M>, A> Writable<RWST<R, W, S, M>, W>.Pass<A>(
        K<RWST<R, W, S, M>, (A Value, Func<W, W> Function)> action) => 
        new RWST<R, W, S, M, A>(
            input => action.As()
                           .runRWST(input)
                           .Map(output => (output.Value.Value, 
                                           output.Output + output.Value.Function(output.Output),
                                           output.State)));

    static K<RWST<R, W, S, M>, Unit> Stateful<RWST<R, W, S, M>, S>.Put(S value) =>
        new RWST<R, W, S, M, Unit>(input => M.Pure((unit, input.Output, value)));

    static K<RWST<R, W, S, M>, Unit> Stateful<RWST<R, W, S, M>, S>.Modify(Func<S, S> f) => 
        new RWST<R, W, S, M, Unit>(input => M.Pure((unit, input.Output, f(input.State))));

    static K<RWST<R, W, S, M>, A> Stateful<RWST<R, W, S, M>, S>.Gets<A>(Func<S, A> f) => 
        new RWST<R, W, S, M, A>(input => M.Pure((f(input.State), input.Output, input.State)));

    public static K<RWST<R, W, S, M>, A> LiftIO<A>(K<IO, A> ma) => 
        new RWST<R, W, S, M, A>(input => M.LiftIO(ma).Map(a => (a, input.Output, input.State)));

    public static K<RWST<R, W, S, M>, A> LiftIO<A>(IO<A> ma) => 
        new RWST<R, W, S, M, A>(input => M.LiftIO(ma).Map(a => (a, input.Output, input.State)));
}
