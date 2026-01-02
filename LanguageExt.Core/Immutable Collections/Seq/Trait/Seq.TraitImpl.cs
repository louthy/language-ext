using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Seq : 
    Monad<Seq>, 
    MonoidK<Seq>,
    Alternative<Seq>, 
    Traversable<Seq>,
    Foldable<Seq, Seq.FoldState>
{
    static K<Seq, B> Monad<Seq>.Recur<A, B>(A value, Func<A, K<Seq, Next<A, B>>> f)
    {
        return toSeq(go());
        IEnumerable<B> go()
        {
            List<A> values = [value];
            List<A> next   = [];

            while (true)
            {
                foreach (var x in values)
                {
                    foreach (var mb in +f(x))
                    {
                        if (mb.IsDone)
                        {
                            yield return mb.Done;
                        }
                        else
                        {
                            next.Add(mb.Loop);
                        }
                    }
                }

                if (next.Count == 0)
                {
                    break;
                }
                else
                {
                    (values, next) = (next, values);
                    next.Clear();
                }
            }
        }
    }
    
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

    static K<Seq, B> Applicative<Seq>.Apply<A, B>(K<Seq, Func<A, B>> mf, Memo<Seq, A> ma) 
    {
        return new Seq<B>(go());
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

    static K<Seq, A> MonoidK<Seq>.Empty<A>() =>
        Seq<A>.Empty;

    static K<Seq, A> Alternative<Seq>.Empty<A>() =>
        Seq<A>.Empty;

    static K<Seq, A> Choice<Seq>.Choose<A>(K<Seq, A> ma, K<Seq, A> mb) => 
        ma.As().IsEmpty ? mb : ma;

    static K<Seq, A> Choice<Seq>.Choose<A>(K<Seq, A> ma, Memo<Seq, A> mb) => 
        ma.As().IsEmpty ? ~mb : ma;

    static K<Seq, A> SemigroupK<Seq>.Combine<A>(K<Seq, A> ma, K<Seq, A> mb) =>
        ma.As() + mb.As();

    static int Foldable<Seq>.Count<TA, A>(in TA ta) =>
        ta.As().Count;

    static void Foldable<Seq, FoldState>.FoldStepInit<TA, A>(in TA ta, ref FoldState refState) => 
        ta.As().InitFoldState(ref refState);

    static bool Foldable<Seq, FoldState>.FoldStep<TA, A>(in TA ta, ref FoldState refState, out A value)
    {
        if (FoldState.MoveNext<A>(ref refState, out var v))
        {
            value = v;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    static void Foldable<Seq, FoldState>.FoldStepBackInit<TA, A>(in TA ta, ref FoldState refState) => 
        ta.As().InitFoldBackState(ref refState);

    static bool Foldable<Seq, FoldState>.FoldStepBack<TA, A>(in TA ta, ref FoldState refState, out A value) 
    {
        if (FoldState.MovePrev<A>(ref refState, out var v))
        {
            value = v;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    static bool Foldable<Seq>.IsEmpty<TA, A>(in TA ta) =>
        ta.As().IsEmpty;

    static Option<A> Foldable<Seq>.At<TA, A>(Index index, in TA ta)
    {
        var list = ta.As();
        return index.Value >= 0 && index.Value < list.Count
                   ? Some(list[index])
                   : Option<A>.None;
    }

    static Option<A> Foldable<Seq>.Head<TA, A>(in TA ta) =>
        ta.As().Head;

    static Option<A> Foldable<Seq>.Last<TA, A>(in TA ta) =>
        ta.As().Last;
    
    static K<F, K<Seq, B>> Traversable<Seq>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Seq, A> ta)
    {
        return Foldable.fold(add, F.Pure(Seq<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, Seq<B>> add(K<F, Seq<B>> state, A value) =>
            Applicative.lift((bs, b) => bs.Add(b), state, f(value));                                            
    }

    static K<F, K<Seq, B>> Traversable<Seq>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Seq, A> ta) 
    {
        return Foldable.fold(add, F.Pure(Seq<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, Seq<B>> add(K<F, Seq<B>> state, A value) =>
            state.Bind(bs => f(value).Bind(b => F.Pure(bs.Add(b)))); 
    }
    
    static Fold<A, S> Foldable<Seq>.FoldStep<TA, A, S>(in TA ta, in S initialState)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        var iter = ta.As().GetEnumerator();
        return go(initialState);
        Fold<A, S> go(S state) =>
            iter.MoveNext()
                ? Fold.Loop(state, iter.Current, go)
                : Fold.Done<A, S>(state);
    }   
        
    static Fold<A, S> Foldable<Seq>.FoldStepBack<TA, A, S>(in TA ta, in S initialState)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        var iter = ta.As().Reverse().GetEnumerator();
        return go(initialState);
        Fold<A, S> go(S state) =>
            iter.MoveNext()
                ? Fold.Loop(state, iter.Current, go)
                : Fold.Done<A, S>(state);
    }
}
