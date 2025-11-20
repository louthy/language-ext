using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Seq : 
    Monad<Seq>, 
    Alternative<Seq>, 
    Traversable<Seq>
{
    static K<Seq, B> Monad<Seq>.Bind<A, B>(K<Seq, A> ma, Func<A, K<Seq, B>> f)
    {
        return new Seq<B>(go());
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

    static K<Seq, B> Functor<Seq>.Map<A, B>(Func<A, B> f, K<Seq, A> ma) 
    {
        return new Seq<B>(go());
        IEnumerable<B> go()
        {
            foreach (var x in ma.As())
            {
                yield return f(x);
            }
        }
    }

    static K<Seq, A> Applicative<Seq>.Pure<A>(A value) =>
        singleton(value);

    static K<Seq, B> Applicative<Seq>.Apply<A, B>(K<Seq, Func<A, B>> mf, K<Seq, A> ma) 
    {
        return new Seq<B>(go());
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

    static K<Seq, B> Applicative<Seq>.Action<A, B>(K<Seq, A> ma, K<Seq, B> mb)
    {
        ignore(ma.As().Strict());
        return mb;
    }

    static K<Seq, A> MonoidK<Seq>.Empty<A>() =>
        Seq<A>.Empty;

    static K<Seq, A> Choice<Seq>.Choose<A>(K<Seq, A> ma, K<Seq, A> mb) => 
        ma.As().IsEmpty ? mb : ma;

    static K<Seq, A> Choice<Seq>.Choose<A>(K<Seq, A> ma, Func<K<Seq, A>> mb) => 
        ma.As().IsEmpty ? mb() : ma;

    static K<Seq, A> SemigroupK<Seq>.Combine<A>(K<Seq, A> ma, K<Seq, A> mb) =>
        ma.As() + mb.As();

    static int Foldable<Seq>.Count<A>(K<Seq, A> ta) =>
        ta.As().Count;

    static bool Foldable<Seq>.IsEmpty<A>(K<Seq, A> ta) =>
        ta.As().IsEmpty;

    static Option<A> Foldable<Seq>.At<A>(K<Seq, A> ta, Index index)
    {
        var list = ta.As();
        return index.Value >= 0 && index.Value < list.Count
                   ? Some(list[index])
                   : Option<A>.None;
    }

    static Option<A> Foldable<Seq>.Head<A>(K<Seq, A> ta) =>
        ta.As().Head;

    static Option<A> Foldable<Seq>.Last<A>(K<Seq, A> ta) =>
        ta.As().Last;
    
    static S Foldable<Seq>.FoldWhile<A, S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S state, K<Seq, A> ta)
    {
        foreach (var x in ta.As())
        {
            if (!predicate((state, x))) return state;
            state = f(x)(state);
        }
        return state;
    }

    static S Foldable<Seq>.FoldBackWhile<A, S>(Func<S, Func<A, S>> f, Func<(S State, A  Value), bool> predicate, S state, K<Seq, A> ta) 
    {
        foreach (var x in ta.As().Rev())
        {
            if (!predicate((state, x))) return state;
            state = f(state)(x);
        }
        return state;
    }
    
    static K<F, K<Seq, B>> Traversable<Seq>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Seq, A> ta)
    {
        return Foldable.fold(add, F.Pure(Seq<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        Func<K<F, Seq<B>>, K<F, Seq<B>>> add(A value) =>
            state =>
                Applicative.lift((bs, b) => bs.Add(b), state, f(value));                                            
    }

    static K<F, K<Seq, B>> Traversable<Seq>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Seq, A> ta) 
    {
        return Foldable.fold(add, F.Pure(Seq<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        Func<K<F, Seq<B>>, K<F, Seq<B>>> add(A value) =>
            state =>
                state.Bind(
                    bs => f(value).Bind(
                        b => F.Pure(bs.Add(b)))); 
    }
}
