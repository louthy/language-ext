using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Arr : Monad<Arr>, MonoidK<Arr>, Traversable<Arr>
{
    static K<Arr, B> Monad<Arr>.Bind<A, B>(K<Arr, A> ma, Func<A, K<Arr, B>> f)
    {
        return new Arr<B>(Go());
        IEnumerable<B> Go()
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

    static K<Arr, B> Functor<Arr>.Map<A, B>(Func<A, B> f, K<Arr, A> ma) => 
        ma.As().Map(f);

    static K<Arr, A> Applicative<Arr>.Pure<A>(A value) =>
        singleton(value);

    static K<Arr, B> Applicative<Arr>.Apply<A, B>(K<Arr, Func<A, B>> mf, K<Arr, A> ma) 
    {
        return new Arr<B>(Go());
        IEnumerable<B> Go()
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

    static K<Arr, B> Applicative<Arr>.Action<A, B>(K<Arr, A> ma, K<Arr, B> mb) => 
        mb;

    static K<Arr, A> MonoidK<Arr>.Empty<A>() =>
        Arr<A>.Empty;

    static K<Arr, A> SemigroupK<Arr>.Combine<A>(K<Arr, A> ma, K<Arr, A> mb) =>
        ma.As() + mb.As();

    static int Foldable<Arr>.Count<A>(K<Arr, A> ta) =>
        ta.As().Count;

    static bool Foldable<Arr>.IsEmpty<A>(K<Arr, A> ta) =>
        ta.As().IsEmpty;

    static S Foldable<Arr>.FoldWhile<A, S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S state, K<Arr, A> ta)
    {
        var arr = ta.As().Value;
        for (var i = 0; i < arr.Length; i++)
        {
            var x = arr[i];
            if (!predicate((state, x))) return state;
            state = f(x)(state);
        }
        return state;
    }

    static S Foldable<Arr>.FoldBackWhile<A, S>(Func<S, Func<A, S>> f, Func<(S State, A Value), bool> predicate, S state, K<Arr, A> ta) 
    {
        var arr = ta.As().Value;
        for (var i = arr.Length - 1; i >= 0; i--)
        {
            var x = arr[i];
            if (!predicate((state, x))) return state;
            state = f(state)(x);
        }
        return state;
    }

    static Option<A> Foldable<Arr>.At<A>(K<Arr, A> ta, Index index)
    {
        var arr = ta.As().Value;
        return index.Value >= 0 && index.Value < arr.Length
                   ? Some(arr[index])
                   : Option<A>.None;
    }

    static Arr<A> Foldable<Arr>.ToArr<A>(K<Arr, A> ta) =>
        ta.As();

    static Lst<A> Foldable<Arr>.ToLst<A>(K<Arr, A> ta) =>
        new(ta.As());

    static EnumerableM<A> Foldable<Arr>.ToEnumerable<A>(K<Arr, A> ta) =>
        new(ta.As());
    
    static Seq<A> Foldable<Arr>.ToSeq<A>(K<Arr, A> ta) =>
        Seq.FromArray(ta.As().Value);
    
    static K<F, K<Arr, B>> Traversable<Arr>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Arr, A> ta) 
    {
        return F.Map<Arr<B>, K<Arr, B>>(
            ks => ks, 
            F.Map(s => s.ToArr(), 
                  Foldable.foldBack(cons, F.Pure(Seq.empty<B>()), ta)));

        K<F, Seq<B>> cons(K<F, Seq<B>> ys, A x) =>
            Applicative.lift(Prelude.Cons, f(x), ys);
    }
}
