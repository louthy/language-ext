using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// IterableT trait implementations
/// </summary>
public class IterableT<M> : 
    MonadT<IterableT<M>, M>, 
    Traversable<IterableT<M>>, 
    Alternative<IterableT<M>>
    where M : Monad<M>
{
    public static IterableT<M, A> pure<A>(A value) =>
        new IterableMainT<M, A>(M.Pure(MList<A>.Cons(value, M.Pure(MList<A>.Nil))));
    
    public static IterableT<M, A> lift<A>(IEnumerable<A> items) =>
        IterableT<M, A>.Lift(items);
    
    public static IterableT<M, A> lift<A>(IAsyncEnumerable<A> items) =>
        IterableT<M, A>.Lift(items);

    public static IterableT<M, A> lift<A>(Seq<A> items) =>
        IterableT<M, A>.Lift(items);

    public static IterableT<M, A> liftIO<A>(IO<A> ma) =>
        IterableT<M, A>.LiftIO(ma);
    
    static K<IterableT<M>, B> Monad<IterableT<M>>.Bind<A, B>(
        K<IterableT<M>, A> mma,
        Func<A, K<IterableT<M>, B>> f) =>
        mma.As().Map(f).Flatten();

    static K<IterableT<M>, B> Functor<IterableT<M>>.Map<A, B>(
        Func<A, B> f,
        K<IterableT<M>, A> mma) =>
        mma.As().Map(f);

    static K<IterableT<M>, A> Applicative<IterableT<M>>.Pure<A>(A value) => 
        new IterablePureT<M, A>(value);

    static K<IterableT<M>, B> Applicative<IterableT<M>>.Apply<A, B>(
        K<IterableT<M>, Func<A, B>> mf,
        K<IterableT<M>, A> ma) =>
        mf.Bind(f => ma.Map(f));

    static K<IterableT<M>, A> MonadT<IterableT<M>, M>.Lift<A>(K<M, A> ma) =>
        new IterableLiftM<M, A>(ma);

    static S Foldable<IterableT<M>>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<IterableT<M>, A> ta) => 
        throw new NotImplementedException();

    static S Foldable<IterableT<M>>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<IterableT<M>, A> ta) => 
        throw new NotImplementedException();

    static K<F, K<IterableT<M>, B>> Traversable<IterableT<M>>.Traverse<F, A, B>(
        Func<A, K<F, B>> f, 
        K<IterableT<M>, A> ta) => 
        throw new NotImplementedException();

    static K<IterableT<M>, A> SemigroupK<IterableT<M>>.Combine<A>(K<IterableT<M>, A> lhs, K<IterableT<M>, A> rhs) => 
        throw new NotImplementedException();

    static K<IterableT<M>, A> MonoidK<IterableT<M>>.Empty<A>() => 
        IterableMainT<M, A>.Empty;

    static K<IterableT<M>, B> MonadIO<IterableT<M>>.MapIO<A, B>(K<IterableT<M>, A> ma, Func<IO<A>, IO<B>> f) =>
        throw new NotImplementedException();

    static K<IterableT<M>, A> MonadIO<IterableT<M>>.LiftIO<A>(IO<A> ma) =>
        new IterableLiftM<M, A>(M.LiftIO(ma));
}
