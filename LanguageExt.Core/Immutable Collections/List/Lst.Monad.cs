using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public class Lst : Monad<Lst>, MonoidK<Lst>, Traversable<Lst>
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

    static K<F, K<Lst, B>> Traversable<Lst>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Lst, A> ta) 
    {
        return F.Map<Lst<B>, K<Lst, B>>(
            ks => ks, 
            Foldable.foldBack(cons, F.Pure(List.empty<B>()), ta));

        K<F, Lst<B>> cons(K<F, Lst<B>> ys, A x) =>
            Applicative.lift(Prelude.Cons, f(x), ys);
    }

    static K<F, K<Lst, B>> Traversable<Lst>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Lst, A> ta) 
    {
        return F.Map<Lst<B>, K<Lst, B>>(
            ks => ks, 
            Foldable.foldBack(cons, F.Pure(List.empty<B>()), ta));

        K<F, Lst<B>> cons(K<F, Lst<B>> fys, A x) =>
            fys.Bind(ys => f(x).Map(y => y.Cons(ys)));
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

    static EnumerableM<A> Foldable<Lst>.ToEnumerable<A>(K<Lst, A> ta) =>
        new (ta.As());
    
    static Seq<A> Foldable<Lst>.ToSeq<A>(K<Lst, A> ta) =>
        new (ta.As());

}
