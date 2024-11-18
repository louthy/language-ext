using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed;

public record StateEff<S, E, A>(Func<EnvIO, S, Either<E, (S State, A Value)>> runStateEff)
    : K<StateEff<S, E>, A>
    where E : Error;

public static class StateEff
{
    public static StateEff<S, E, A> As<S, E, A>(this K<StateEff<S, E>, A> ma) 
        where E : Error =>
        (StateEff<S, E, A>)ma;
    
    public static IO<Either<E, (S State, A Value)>> Run<S, E, A>(
        this K<StateEff<S, E>, A> ma, S initialState) 
        where E : Error =>
        IO.lift(envIO => ma.As().runStateEff(envIO, initialState));
}

public class StateEff<S, E> :
    Monad<StateEff<S, E>>,
    Stateful<StateEff<S, E>, S>,
    Choice<StateEff<S, E>>
    where E : Error 
{
    static K<StateEff<S, E>, B> Monad<StateEff<S, E>>.Bind<A, B>(
        K<StateEff<S, E>, A> ma,
        Func<A, K<StateEff<S, E>, B>> f) =>
        new StateEff<S, E, B>(
            (eio, state) =>
                ma.As().runStateEff(eio, state) switch
                {
                    Either.Right<E, (S State, A Value)>(var (s, v)) =>
                        f(v).As().runStateEff(eio, s),

                    Either.Left<E, (S State, A Value)>(var e) =>
                        Left(e)
                });

    static K<StateEff<S, E>, B> Functor<StateEff<S, E>>.Map<A, B>(Func<A, B> f, K<StateEff<S, E>, A> ma) =>
        new StateEff<S, E, B>(
            (eio, state) =>
                ma.As().runStateEff(eio, state) switch
                {
                    Either.Right<E, (S State, A Value)>(var (s, v)) =>
                        Right((s, f(v))),

                    Either.Left<E, (S State, A Value)>(var e) =>
                        Left(e)
                });

    static K<StateEff<S, E>, A> Applicative<StateEff<S, E>>.Pure<A>(A value) =>
        new StateEff<S, E, A>((_, s) => Right((s, value)));

    static K<StateEff<S, E>, B> Applicative<StateEff<S, E>>.Apply<A, B>(
        K<StateEff<S, E>, Func<A, B>> mf,
        K<StateEff<S, E>, A> ma) =>
        new StateEff<S, E, B>(
            (eio, state) =>
                mf.As().runStateEff(eio, state) switch
                {
                    Either.Right<E, (S State, Func<A, B> Value)>(var (s1, f)) =>
                        ma.As().runStateEff(eio, s1) switch
                        {
                            Either.Right<E, (S State, A Value)>(var (s2, x)) =>
                                Right((s2, f(x))),

                            Either.Left<E, (S State, A Value)>(var e) =>
                                Left(e)
                        },

                    Either.Left<E, (S State, Func<A, B> Value)>(var e) =>
                        Left(e)
                });

    static K<StateEff<S, E>, Unit> Stateful<StateEff<S, E>, S>.Put(S value) =>
        new StateEff<S, E, Unit>((_, _) => Right((value, unit)));

    static K<StateEff<S, E>, Unit> Stateful<StateEff<S, E>, S>.Modify(Func<S, S> modify) =>
        new StateEff<S, E, Unit>((_, state) => Right((modify(state), unit)));

    static K<StateEff<S, E>, A> Stateful<StateEff<S, E>, S>.Gets<A>(Func<S, A> f) =>
        new StateEff<S, E, A>((_, state) => Right((state, f(state))));

    static K<StateEff<S, E>, A> Choice<StateEff<S, E>>.Choose<A>(K<StateEff<S, E>, A> fa, K<StateEff<S, E>, A> fb) =>
        new StateEff<S, E, A>(
            (eio, state) =>
                fa.As().runStateEff(eio, state) switch
                {
                    Either.Right<E, (S State, A Value)> succ =>
                        succ,

                    Either.Left<E, (S State, A Value)> =>
                        fb.As().runStateEff(eio, state)
                });

    static K<StateEff<S, E>, IO<A>> MonadIO<StateEff<S, E>>.ToIO<A>(K<StateEff<S, E>, A> ma) =>
        new StateEff<S, E, IO<A>>(
            (eio, state) =>
                ma.As().runStateEff(eio, state) switch
                {
                    Either.Right<E, (S State, A Value)>(var (s, v)) =>
                        Right((s, IO.pure(v))),

                    Either.Left<E, (S State, A Value)>(var e) =>
                        Left(e)
                });

    static K<StateEff<S, E>, A> MonadIO<StateEff<S, E>>.LiftIO<A>(K<IO, A> ma) => 
        new StateEff<S, E, A>(
            (eio, state) => 
            {
                try
                {
                    return Right((state, ma.As().Run(eio)));
                }
                catch (Exception e)
                {
                    // TODO: Decide how to convert an Exception to `E` -- this is bespoke to your code
                    //       But if you derive your `E` type from `Error` then you'll find it's easier
                    throw new NotImplementedException();
                }
            }
        );
}
    
    
    
