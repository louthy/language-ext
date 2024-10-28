using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public class Lst : 
    Monad<Lst>, 
    Alternative<Lst>, 
    Traversable<Lst>
{
    static K<Lst, B> Monad<Lst>.Bind<A, B>(K<Lst, A> ma, Func<A, K<Lst, B>> f)
    {
        return new Lst<B>(go());
        IEnumerable<B> go()
        {
            foreach (var x in ma.As())
            {
                foreach (var y in f(x).As())
                {
                    yield return y;
                }
            }
        }
    }

    static K<Lst, B> Functor<Lst>.Map<A, B>(Func<A, B> f, K<Lst, A> ma)
    {
        return new Lst<B>(go());
        IEnumerable<B> go()
        {
            foreach (var x in ma.As())
            {
                yield return f(x);
            }
        }
    }

    static K<Lst, A> Applicative<Lst>.Pure<A>(A value) =>
        List.singleton(value);

    static K<Lst, B> Applicative<Lst>.Apply<A, B>(K<Lst, Func<A, B>> mf, K<Lst, A> ma)
    {
        return new Lst<B>(go());
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

    static K<Lst, B> Applicative<Lst>.Action<A, B>(K<Lst, A> ma, K<Lst, B> mb) =>
        mb;

    static K<Lst, A> MonoidK<Lst>.Empty<A>() =>
        Lst<A>.Empty;

    static K<Lst, A> SemigroupK<Lst>.Combine<A>(K<Lst, A> ma, K<Lst, A> mb) => 
        ma.As() + mb.As();

    static K<Lst, A> Choice<Lst>.Choose<A>(K<Lst, A> ma, K<Lst, A> mb) => 
        ma.IsEmpty() ? mb : ma;

    static K<F, K<Lst, B>> Traversable<Lst>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Lst, A> ta)
    {
        return Foldable.fold(add, F.Pure(Lst<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        Func<K<F, Lst<B>>, K<F, Lst<B>>> add(A value) =>
            state =>
                Applicative.lift((bs, b) => bs.Add(b), state, f(value));                                            
    }

    static K<F, K<Lst, B>> Traversable<Lst>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Lst, A> ta) 
    {
        return Foldable.fold(add, F.Pure(Lst<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        Func<K<F, Lst<B>>, K<F, Lst<B>>> add(A value) =>
            state =>
                state.Bind(
                    bs => f(value).Bind(
                        b => F.Pure(bs.Add(b)))); 
    }    
    
    static S Foldable<Lst>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state,
        K<Lst, A> ta)
    {
        foreach (var x in ta.As())
        {
            if (!predicate((state, x))) return state;
            state = f(x)(state);
        }
        return state;
    }
    
    static S Foldable<Lst>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S state, 
        K<Lst, A> ta)
    {
        foreach (var x in ta.As().Reverse())
        {
            if (!predicate((state, x))) return state;
            state = f(state)(x);
        }
        return state;
    }

    static int Foldable<Lst>.Count<A>(K<Lst, A> ta) =>
        ta.As().Count;

    static bool Foldable<Lst>.IsEmpty<A>(K<Lst, A> ta) =>
        ta.As().IsEmpty;

    static Option<A> Foldable<Lst>.At<A>(K<Lst, A> ta, Index index)
    {
        var list = ta.As().Value;
        return index.Value >= 0 && index.Value < list.Count
                   ? Some(list[index])
                   : Option<A>.None;
    }
        
    static Arr<A> Foldable<Lst>.ToArr<A>(K<Lst, A> ta) =>
        new(ta.As());

    static Lst<A> Foldable<Lst>.ToLst<A>(K<Lst, A> ta) =>
        ta.As();

    static Iterable<A> Foldable<Lst>.ToIterable<A>(K<Lst, A> ta) =>
        Iterable.createRange (ta.As());
    
    static Seq<A> Foldable<Lst>.ToSeq<A>(K<Lst, A> ta) =>
        new (ta.As());

}
