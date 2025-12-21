using System;
using System.Collections.Generic;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Iterable : 
    Monad<Iterable>, 
    MonoidK<Iterable>,
    Alternative<Iterable>, 
    Traversable<Iterable>,
    NaturalMono<Iterable, Arr>,
    NaturalMono<Iterable, Seq>,
    NaturalMono<Iterable, Lst>,
    NaturalMono<Iterable, Set>,
    NaturalMono<Iterable, HashSet>
{
    static K<Iterable, B> Monad<Iterable>.Recur<A, B>(A value, Func<A, K<Iterable, Next<A, B>>> f) =>
        Monad.unsafeRecur(value, f);
    
    static K<Iterable, B> Monad<Iterable>.Bind<A, B>(K<Iterable, A> ma, Func<A, K<Iterable, B>> f) =>
        ma.As().Bind(f);

    static K<Iterable, B> Functor<Iterable>.Map<A, B>(Func<A, B> f, K<Iterable, A> ma) => 
        ma.As().Map(f);

    static K<Iterable, A> Applicative<Iterable>.Pure<A>(A value) =>
        singleton(value);

    static K<Iterable, B> Applicative<Iterable>.Apply<A, B>(K<Iterable, Func<A, B>> mf, K<Iterable, A> ma)
    {
        return go().AsIterable();        
        IEnumerable<B> go()
        {
            foreach (var f in mf.As())
            {
                foreach (var a in ma.As())
                {
                    yield return f(a); 
                }
            }
        }
    }

    static K<Iterable, B> Applicative<Iterable>.Apply<A, B>(K<Iterable, Func<A, B>> mf, Memo<Iterable, A> ma)
    {
        return go().AsIterable();        
        IEnumerable<B> go()
        {
            foreach (var f in mf.As())
            {
                foreach (var a in ma.Value.As())
                {
                    yield return f(a); 
                }
            }
        }
    }

    static K<Iterable, A> MonoidK<Iterable>.Empty<A>() =>
        Iterable<A>.Empty;

    static K<Iterable, A> Alternative<Iterable>.Empty<A>() => 
        Iterable<A>.Empty;
    
    static K<Iterable, A> SemigroupK<Iterable>.Combine<A>(K<Iterable, A> ma, K<Iterable, A> mb) =>
        ma.As().Concat(mb.As());

    static K<Iterable, A> Choice<Iterable>.Choose<A>(K<Iterable, A> ma, K<Iterable, A> mb) => 
        ma.IsEmpty ? mb : ma;
    
    static K<Iterable, A> Choice<Iterable>.Choose<A>(K<Iterable, A> ma, Memo<Iterable, A> mb) => 
        ma.IsEmpty ? mb.Value : ma;
    
    static K<F, K<Iterable, B>> Traversable<Iterable>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Iterable, A> ta)
    {
        return Foldable.fold(add, F.Pure(Iterable<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        Func<K<F, Iterable<B>>, K<F, Iterable<B>>> add(A value) =>
            state =>
                Applicative.lift((bs, b) => bs.Add(b), state, f(value));                                            
    }

    static K<F, K<Iterable, B>> Traversable<Iterable>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Iterable, A> ta) 
    {
        return Foldable.fold(add, F.Pure(Iterable<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        Func<K<F, Iterable<B>>, K<F, Iterable<B>>> add(A value) =>
            state =>
                state.Bind(
                    bs => f(value).Bind(
                        b => F.Pure(bs.Add(b)))); 
    }

    static S Foldable<Iterable>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Iterable, A> ta) =>
        ta.As().FoldWhile(f, predicate, state);
    
    static S Foldable<Iterable>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<Iterable, A> ta) =>
        ta.As().FoldBackWhile(f, predicate, state);
    
    static Arr<A> Foldable<Iterable>.ToArr<A>(K<Iterable, A> ta) =>
        new(ta.As());

    static Lst<A> Foldable<Iterable>.ToLst<A>(K<Iterable, A> ta) =>
        new(ta.As());

    static Iterable<A> Foldable<Iterable>.ToIterable<A>(K<Iterable, A> ta) =>
        ta.As();
    
    static Seq<A> Foldable<Iterable>.ToSeq<A>(K<Iterable, A> ta) =>
        new(ta.As());

    static K<Seq, A> Natural<Iterable, Seq>.Transform<A>(K<Iterable, A> fa) => 
        toSeq(fa.As());

    static K<Arr, A> Natural<Iterable, Arr>.Transform<A>(K<Iterable, A> fa) => 
        toArray(fa.As());

    static K<Lst, A> Natural<Iterable, Lst>.Transform<A>(K<Iterable, A> fa) => 
        toList(fa.As());

    static K<Set, A> Natural<Iterable, Set>.Transform<A>(K<Iterable, A> fa) => 
        toSet(fa.As());

    static K<HashSet, A> Natural<Iterable, HashSet>.Transform<A>(K<Iterable, A> fa) => 
        toHashSet(fa.As());
}
