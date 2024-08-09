using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Iterable : Monad<Iterable>, MonoidK<Iterable>, Traversable<Iterable>
{
    static K<Iterable, B> Monad<Iterable>.Bind<A, B>(K<Iterable, A> ma, Func<A, K<Iterable, B>> f) =>
        ma.As().Bind(f);

    static K<Iterable, B> Functor<Iterable>.Map<A, B>(Func<A, B> f, K<Iterable, A> ma) => 
        ma.As().Map(f);

    static K<Iterable, A> Applicative<Iterable>.Pure<A>(A value) =>
        singleton(value);

    static K<Iterable, B> Applicative<Iterable>.Apply<A, B>(K<Iterable, Func<A, B>> mf, K<Iterable, A> ma) =>
        throw new NotImplementedException();

    static K<Iterable, B> Applicative<Iterable>.Action<A, B>(K<Iterable, A> ma, K<Iterable, B> mb)
    {
        ma.As().Consume();
        return mb;
    }

    static K<Iterable, A> MonoidK<Iterable>.Empty<A>() =>
        Iterable<A>.Empty;

    static K<Iterable, A> SemigroupK<Iterable>.Combine<A>(K<Iterable, A> ma, K<Iterable, A> mb) =>
        throw new NotImplementedException();

    static K<F, K<Iterable, B>> Traversable<Iterable>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Iterable, A> ta) => 
        throw new NotImplementedException();

    static K<F, K<Iterable, B>> Traversable<Iterable>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Iterable, A> ta) => 
        throw new NotImplementedException();
    
    static S Foldable<Iterable>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Iterable, A> ta) =>
        throw new NotImplementedException();
    
    static S Foldable<Iterable>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<Iterable, A> ta) =>
        throw new NotImplementedException();
    
    static Arr<A> Foldable<Iterable>.ToArr<A>(K<Iterable, A> ta) =>
        new(ta.As());

    static Lst<A> Foldable<Iterable>.ToLst<A>(K<Iterable, A> ta) =>
        new(ta.As());

    static Iterable<A> Foldable<Iterable>.ToIterable<A>(K<Iterable, A> ta) =>
        ta.As();
    
    static Seq<A> Foldable<Iterable>.ToSeq<A>(K<Iterable, A> ta) =>
        throw new NotImplementedException();
}
