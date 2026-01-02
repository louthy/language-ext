using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Lst : 
    Monad<Lst>, 
    MonoidK<Lst>,
    Alternative<Lst>, 
    Traversable<Lst>,
    Foldable<Lst, Lst.FoldState>
{
    static K<Lst, B> Monad<Lst>.Recur<A, B>(A value, Func<A, K<Lst, Next<A, B>>> f) =>
        List.createRange(Monad.enumerableRecur(value, x =>f(x).As().AsEnumerable()));
    
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

    static K<Lst, B> Applicative<Lst>.Apply<A, B>(K<Lst, Func<A, B>> mf, Memo<Lst, A> ma)
    {
        return new Lst<B>(go());
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

    static K<Lst, A> MonoidK<Lst>.Empty<A>() =>
        Lst<A>.Empty;

    static K<Lst, A> Alternative<Lst>.Empty<A>() =>
        Lst<A>.Empty;

    static K<Lst, A> SemigroupK<Lst>.Combine<A>(K<Lst, A> ma, K<Lst, A> mb) => 
        ma.As() + mb.As();

    static K<Lst, A> Choice<Lst>.Choose<A>(K<Lst, A> ma, K<Lst, A> mb) => 
        ma.IsEmpty ? mb : ma;

    static K<Lst, A> Choice<Lst>.Choose<A>(K<Lst, A> ma, Memo<Lst, A> mb) => 
        ma.IsEmpty ? mb.Value : ma;

    static K<F, K<Lst, B>> Traversable<Lst>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Lst, A> ta)
    {
        return Foldable.fold(add, F.Pure(Lst<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, Lst<B>> add(K<F, Lst<B>> state, A value) =>
            Applicative.lift((bs, b) => bs.Add(b), state, f(value));                                            
    }

    static K<F, K<Lst, B>> Traversable<Lst>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Lst, A> ta) 
    {
        return Foldable.fold(add, F.Pure(Lst<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, Lst<B>> add(K<F, Lst<B>> state, A value) =>
            state.Bind(bs => f(value).Bind(b => F.Pure(bs.Add(b)))); 
    }    

    static int Foldable<Lst>.Count<A>(K<Lst, A> ta) =>
        ta.As().Count;

    static bool Foldable<Lst>.IsEmpty<A>(K<Lst, A> ta) =>
        ta.As().IsEmpty;

    static Option<A> Foldable<Lst>.At<A>(Index index, K<Lst, A> ta)
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

    static void Foldable<Lst, FoldState>.FoldStepSetup<A>(K<Lst, A> ta, ref FoldState refState) => 
        FoldState.Init(ref refState, ta.As().Value.Root);

    static bool Foldable<Lst, FoldState>.FoldStep<A>(K<Lst, A> ta, ref FoldState refState, out A value)
    {
        if (FoldState.Step<A>(ref refState, out var item))
        {
            value = item.Key;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    static void Foldable<Lst, FoldState>.FoldStepBackSetup<A>(K<Lst, A> ta, ref FoldState refState) => 
        FoldState.Init(ref refState, ta.As().Value.Root);

    static bool Foldable<Lst, FoldState>.FoldStepBack<A>(K<Lst, A> ta, ref FoldState refState, out A value) 
    {
        if (FoldState.StepBack<A>(ref refState, out var item))
        {
            value = item.Key;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    static Seq<A> Foldable<Lst>.ToSeq<A>(K<Lst, A> ta) =>
        new (ta.As());
    
    static Fold<A, S> Foldable<Lst>.FoldStep<A, S>(K<Lst, A> ta, in S initialState) =>
        ta.As().FoldStep(initialState);
    
    static Fold<A, S> Foldable<Lst>.FoldStepBack<A, S>(K<Lst, A> ta, in S initialState) =>
        ta.As().FoldStepBack(initialState);
}
