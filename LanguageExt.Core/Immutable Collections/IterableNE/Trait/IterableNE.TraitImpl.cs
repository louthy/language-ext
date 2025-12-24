using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class IterableNE : 
    Monad<IterableNE>, 
    SemigroupK<IterableNE>,
    Foldable<IterableNE>,
    NaturalMono<IterableNE, Arr>,
    NaturalMono<IterableNE, Seq>,
    NaturalMono<IterableNE, Lst>,
    NaturalMono<IterableNE, Set>,
    NaturalMono<IterableNE, Iterable>,
    NaturalMono<IterableNE, HashSet>
{
    static K<IterableNE, B> Monad<IterableNE>.Recur<A, B>(A value, Func<A, K<IterableNE, Next<A, B>>> f) =>
        Monad.unsafeRecur(value, f);
    
    static K<IterableNE, B> Monad<IterableNE>.Bind<A, B>(K<IterableNE, A> ma, Func<A, K<IterableNE, B>> f) =>
        ma.As().Bind(f);

    static K<IterableNE, B> Functor<IterableNE>.Map<A, B>(Func<A, B> f, K<IterableNE, A> ma) => 
        ma.As().Map(f);

    static K<IterableNE, A> Applicative<IterableNE>.Pure<A>(A value) =>
        singleton(value);

    static K<IterableNE, B> Applicative<IterableNE>.Apply<A, B>(K<IterableNE, Func<A, B>> mf, K<IterableNE, A> ma) =>
        mf >> ma.Map;

    static K<IterableNE, B> Applicative<IterableNE>.Apply<A, B>(K<IterableNE, Func<A, B>> mf, Memo<IterableNE, A> ma) =>
        mf >> ma.Map;

    static K<IterableNE, A> SemigroupK<IterableNE>.Combine<A>(K<IterableNE, A> ma, K<IterableNE, A> mb) =>
        ma.As().Concat(mb.As());
    
    static S Foldable<IterableNE>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<IterableNE, A> ta) =>
        ta.As().FoldWhile(f, predicate, state);
    
    static S Foldable<IterableNE>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<IterableNE, A> ta) =>
        throw new NotSupportedException("FoldBack* is currently not supported for IterableNE");
    
    static Arr<A> Foldable<IterableNE>.ToArr<A>(K<IterableNE, A> ta) =>
        new(ta.As());

    static Lst<A> Foldable<IterableNE>.ToLst<A>(K<IterableNE, A> ta) =>
        new(ta.As());

    static Iterable<A> Foldable<IterableNE>.ToIterable<A>(K<IterableNE, A> ta) =>
        ta.As().AsIterable();
    
    static Seq<A> Foldable<IterableNE>.ToSeq<A>(K<IterableNE, A> ta) =>
        new(ta.As());

    static K<Seq, A> Natural<IterableNE, Seq>.Transform<A>(K<IterableNE, A> fa) => 
        toSeq(fa.As());

    static K<Arr, A> Natural<IterableNE, Arr>.Transform<A>(K<IterableNE, A> fa) => 
        toArray(fa.As());

    static K<Lst, A> Natural<IterableNE, Lst>.Transform<A>(K<IterableNE, A> fa) => 
        toList(fa.As());

    static K<Set, A> Natural<IterableNE, Set>.Transform<A>(K<IterableNE, A> fa) => 
        toSet(fa.As());

    static K<Iterable, A> Natural<IterableNE, Iterable>.Transform<A>(K<IterableNE, A> fa) => 
        fa.As().AsIterable();

    static K<HashSet, A> Natural<IterableNE, HashSet>.Transform<A>(K<IterableNE, A> fa) => 
        toHashSet(fa.As());
}
