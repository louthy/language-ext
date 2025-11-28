using System;
using LanguageExt.Traits;
using G = System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Iterator : 
    Monad<Iterator>,
    MonoidK<Iterator>,
    Alternative<Iterator>, 
    Traversable<Iterator>,
    Natural<Iterator, Arr>,
    Natural<Iterator, Seq>,
    Natural<Iterator, Lst>,
    Natural<Iterator, Set>,
    Natural<Iterator, Iterable>,
    Natural<Iterator, HashSet>
{
    static K<Iterator, B> Monad<Iterator>.Bind<A, B>(K<Iterator, A> ma, Func<A, K<Iterator, B>> f) =>
        ma.As().Bind(f);

    static K<Iterator, B> Functor<Iterator>.Map<A, B>(Func<A, B> f, K<Iterator, A> ma) => 
        ma.As().Map(f);

    static K<Iterator, A> Applicative<Iterator>.Pure<A>(A value) =>
        Cons(value, Nil<A>());

    static K<Iterator, B> Applicative<Iterator>.Apply<A, B>(K<Iterator, Func<A, B>> mf, K<Iterator, A> ma)
    {
        return go().GetIterator();
        G.IEnumerable<B> go()
        {
            for (var f = mf.As().Clone(); !f.IsEmpty; f = f.Tail)
            {
                for (var a = ma.As().Clone(); !a.IsEmpty; a = a.Tail)
                {
                    yield return f.Head(a.Head);
                }
            }
        }
    }

    static Memo<Iterator, B> Applicative<Iterator>.Apply<A, B>(K<Iterator, Func<A, B>> mf, Memo<Iterator, A> ma)
    {
        return memoF(go().GetIterator());
        G.IEnumerable<B> go()
        {
            for (var f = mf.As().Clone(); !f.IsEmpty; f = f.Tail)
            {
                for (var a = ma.Value.As().Clone(); !a.IsEmpty; a = a.Tail)
                {
                    yield return f.Head(a.Head);
                }
            }
        }
    }

    static K<Iterator, B> Applicative<Iterator>.Action<A, B>(K<Iterator, A> ma, K<Iterator, B> mb)
    {
        ignore(ma.As().Count);
        return mb;
    }

    static K<Iterator, A> MonoidK<Iterator>.Empty<A>() =>
        Iterator<A>.Empty;

    static K<Iterator, A> Alternative<Iterator>.Empty<A>() =>
        Iterator<A>.Empty;

    static K<Iterator, A> SemigroupK<Iterator>.Combine<A>(K<Iterator, A> ma, K<Iterator, A> mb) =>
        ma.As().Concat(mb.As());

    static K<Iterator, A> Choice<Iterator>.Choose<A>(K<Iterator, A> ma, K<Iterator, A> mb) => 
        ma.IsEmpty ? mb : ma;
    
    static Memo<Iterator, A> Choice<Iterator>.Choose<A>(K<Iterator, A> ma, Memo<Iterator, A> mb) => 
        ma.IsEmpty ? mb : memoF(ma);
    
    static K<F, K<Iterator, B>> Traversable<Iterator>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Iterator, A> ta)
    {
        return Foldable.foldBack(add, F.Pure(Iterator<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        Func<A, K<F, Iterator<B>>> add(K<F, Iterator<B>> state) =>
            value =>
              Applicative.lift((bs, b) => b.Cons(bs), state, f(value));                                            
    }

    static K<F, K<Iterator, B>> Traversable<Iterator>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Iterator, A> ta) 
    {
        return Foldable.foldBack(add, F.Pure(Iterator<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        Func<A, K<F, Iterator<B>>> add(K<F, Iterator<B>> state) =>
            value =>
                state.Bind(
                    bs => f(value).Bind(
                        b => F.Pure(b.Cons(bs)))); 
    }

    static S Foldable<Iterator>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Iterator, A> ta) =>
        ta.As().FoldWhile(state, f, predicate);
    
    static S Foldable<Iterator>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<Iterator, A> ta) =>
        ta.As().FoldBackWhile(state, f, predicate);
    
    static Arr<A> Foldable<Iterator>.ToArr<A>(K<Iterator, A> ta) =>
        new(ta.As());

    static Lst<A> Foldable<Iterator>.ToLst<A>(K<Iterator, A> ta) =>
        new(ta.As());

    static Iterable<A> Foldable<Iterator>.ToIterable<A>(K<Iterator, A> ta) =>
        Iterable.createRange(ta.As());
    
    static Seq<A> Foldable<Iterator>.ToSeq<A>(K<Iterator, A> ta) =>
        new(ta.As());

    static K<Seq, A> Natural<Iterator, Seq>.Transform<A>(K<Iterator, A> fa) => 
        toSeq(fa.As());

    static K<Arr, A> Natural<Iterator, Arr>.Transform<A>(K<Iterator, A> fa) => 
        toArray(fa.As());

    static K<Lst, A> Natural<Iterator, Lst>.Transform<A>(K<Iterator, A> fa) => 
        toList(fa.As());

    static K<Set, A> Natural<Iterator, Set>.Transform<A>(K<Iterator, A> fa) => 
        toSet(fa.As());

    static K<HashSet, A> Natural<Iterator, HashSet>.Transform<A>(K<Iterator, A> fa) => 
        toHashSet(fa.As());
    
    static K<Iterable, A> Natural<Iterator, Iterable>.Transform<A>(K<Iterator, A> fa) => 
        Iterable.createRange(fa.As());
}
