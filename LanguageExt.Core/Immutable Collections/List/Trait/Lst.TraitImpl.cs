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
    Foldable<Lst, Lst.FoldState>,
    FoldableBack<Lst, Lst.FoldState>
{
    static K<Lst, B> Monad<Lst>.Recur<A, B>(A value, Func<A, K<Lst, Next<A, B>>> f) =>
        createRange(Monad.enumerableRecur(value, x =>f(x).As().AsEnumerable()));
    
    static K<Lst, B> Monad<Lst>.Bind<A, B>(K<Lst, A> ma, Func<A, K<Lst, B>> f)
    {
        var       root     = ListItem<B>.EmptyM;
        var       subIndex = 0;
        
        var fsa = ma.StepSetup<Lst, FoldState, A>();
        while (ma.Step(ref fsa, out var a))
        {
            var mb = +f(a);
            var fsb = mb.StepSetup<Lst, FoldState, B>();
            while (mb.Step(ref fsb, out var b))
            {
                root = ListModuleM.Insert(root, new ListItem<B>(1, 1, ListItem<B>.Empty, b, ListItem<B>.Empty), subIndex);
                subIndex++;
            }
        }
        return new Lst<B>(root);
    }

    static K<Lst, B> Functor<Lst>.Map<A, B>(Func<A, B> f, K<Lst, A> ma)
    {
        var       root     = ListItem<B>.EmptyM;
        var       subIndex = 0;
        
        var fsa = ma.StepSetup<Lst, FoldState, A>();
        while (ma.Step(ref fsa, out var a))
        {
            var b = f(a);
            root = ListModuleM.Insert(root, new ListItem<B>(1, 1, ListItem<B>.Empty, b, ListItem<B>.Empty), subIndex);
            subIndex++;
        }
        return new Lst<B>(root);
    }

    static K<Lst, A> Applicative<Lst>.Pure<A>(A value) =>
        singleton(value);

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

    static Option<A> Foldable<Lst>.At<A>(int index, K<Lst, A> ta)
    {
        var list = ta.As().Value;
        return index >= 0 && index < list.Count
                   ? Some(list[index])
                   : Option<A>.None;
    }

    static Option<A> FoldableBack<Lst>.AtBack<A>(int index, K<Lst, A> ta)
    {
        var list = ta.As().Value;
        return index > 0 && index <= list.Count
                   ? Some(list[^index])
                   : Option<A>.None;
    }
        
    static Arr<A> Foldable<Lst>.ToArr<A>(K<Lst, A> ta) =>
        new(ta.As());

    static Lst<A> Foldable<Lst>.ToLst<A>(K<Lst, A> ta) =>
        ta.As();

    static Iterable<A> Foldable<Lst>.ToIterable<A>(K<Lst, A> ta) =>
        Iterable.createRange (ta.As());

    static FoldState IterableK<Lst, FoldState>.StepSetup<A>(K<Lst, A> ta) => 
        FoldState.Setup(ta.As().Value.Root);

    static bool IterableK<Lst, FoldState>.Step<A>(K<Lst, A> ta, ref FoldState refState, out A value)
    {
        if (FoldState.Step<A>(ref refState, out var item))
        {
            value = item;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    static FoldState IterableBackK<Lst, FoldState>.StepBackSetup<A>(K<Lst, A> ta) => 
        FoldState.Setup(ta.As().Value.Root);

    static bool IterableBackK<Lst, FoldState>.StepBack<A>(K<Lst, A> ta, ref FoldState refState, out A value) 
    {
        if (FoldState.StepBack<A>(ref refState, out var item))
        {
            value = item;
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

    static Iterator<A> IterableK<Lst>.ForwardIterator<A>(K<Lst, A> fa) => 
        throw new NotImplementedException();

    static Iterator<A> IterableBackK<Lst>.BackwardIterator<A>(K<Lst, A> fa) => 
        throw new NotImplementedException();
}
