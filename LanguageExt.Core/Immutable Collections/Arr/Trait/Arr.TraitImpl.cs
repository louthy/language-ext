using System;
using System.Linq;
using LanguageExt.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Arr : 
    Monad<Arr>, 
    Traversable<Arr>, 
    Alternative<Arr>,
    MonoidK<Arr>,
    Natural<Arr, Seq>,
    Natural<Arr, Iterable>,
    Natural<Arr, Lst>,
    Natural<Arr, Set>,
    Natural<Arr, HashSet>,
    Foldable<Arr, Arr.FoldState>
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

    static K<Arr, B> Monad<Arr>.Recur<A, B>(A value, Func<A, K<Arr, Next<A, B>>> f) =>
        createRange(Monad.enumerableRecur(value, x => f(x).As().AsEnumerable()));

    static K<Arr, B> Functor<Arr>.Map<A, B>(Func<A, B> f, K<Arr, A> ma) => 
        ma.As().Map(f);

    static K<Arr, A> Applicative<Arr>.Pure<A>(A value) =>
        singleton(value);

    static K<Arr, B> Applicative<Arr>.Apply<A, B>(K<Arr, Func<A, B>> mf, K<Arr, A> ma) 
    {
        var ff   = mf.As();
        if(ff.IsEmpty) return Arr<B>.Empty;
        var fa   = ma.As();
        var size = ff.Count * fa.Count;
        var bs   = new B[size];
        var ix   = 0;
        foreach (var f in ff)
        {
            foreach (var a in fa)
            {
                bs[ix] = f(a);
                ix++;
            }
        }
        return new Arr<B>(bs);
    }    

    static K<Arr, B> Applicative<Arr>.Apply<A, B>(K<Arr, Func<A, B>> mf, Memo<Arr, A> ma)
    {
        var ff   = mf.As();
        if(ff.IsEmpty) return Arr<B>.Empty;
        var fa   = ma.Value.As();
        var size = ff.Count * fa.Count;
        var bs   = new B[size];
        var ix   = 0;
        foreach (var f in ff)
        {
            foreach (var a in fa)
            {
                bs[ix] = f(a);
                ix++;
            }
        }
        return new Arr<B>(bs);
    }    

    static K<Arr, A> MonoidK<Arr>.Empty<A>() =>
        Arr<A>.Empty;

    static K<Arr, A> Alternative<Arr>.Empty<A>() =>
        Arr<A>.Empty;

    static K<Arr, A> SemigroupK<Arr>.Combine<A>(K<Arr, A> ma, K<Arr, A> mb) =>
        ma.As() + mb.As();
    
    static K<Arr, A> Choice<Arr>.Choose<A>(K<Arr, A> ma, K<Arr, A> mb) => 
        ma.IsEmpty ? mb : ma;

    static K<Arr, A> Choice<Arr>.Choose<A>(K<Arr, A> ma, Memo<Arr, A> mb) => 
        ma.IsEmpty ? mb.Value : ma;

    static int Foldable<Arr>.Count<A>(K<Arr, A> ta) =>
        ta.As().Count;

    static bool Foldable<Arr>.IsEmpty<A>(K<Arr, A> ta) =>
        ta.As().IsEmpty;

    static void Foldable<Arr, FoldState>.FoldStepSetup<A>(K<Arr, A> ta, ref FoldState state) =>
        FoldState.Setup(ref state, ta.As().AsSpan());

    static void Foldable<Arr, FoldState>.FoldStepBackSetup<A>(K<Arr, A> ta, ref FoldState state) =>
        FoldState.SetupBack(ref state, ta.As().AsSpan());

    static bool Foldable<Arr, FoldState>.FoldStep<A>(K<Arr, A> ta, ref FoldState state, out A value) =>
        FoldState.MoveNext(ref state, out value);

    static bool Foldable<Arr, FoldState>.FoldStepBack<A>(K<Arr, A> ta, ref FoldState state, out A value) =>
        FoldState.MovePrev(ref state, out value);

    static Fold<A, S> Foldable<Arr>.FoldStep<A, S>(K<Arr, A> ta, in S initialState)
    {
        var array = ta.As();
        return go(array.GetIterator())(initialState);

        static Func<S, Fold<A, S>> go(Iterator<A> iterA) =>
            state =>
            {
                if (iterA.IsEmpty)
                {
                    return Fold.Done<A, S>(state);
                }
                else
                {
                    return Fold.Loop(state, iterA.Head, go(iterA.Tail.Clone()));
                }
            };
    }

    static Fold<A, S> Foldable<Arr>.FoldStepBack<A, S>(K<Arr, A> ta, in S initialState)
    {
        var array = ta.As();
        return go(array.Reverse().GetIterator())(initialState);

        static Func<S, Fold<A, S>> go(Iterator<A> iterA) =>
            state =>
            {
                if (iterA.IsEmpty)
                {
                    return Fold.Done<A, S>(state);
                }
                else
                {
                    return Fold.Loop(state, iterA.Head, go(iterA.Tail.Clone()));
                }
            };
    }

    static Option<A> Foldable<Arr>.At<A>(Index index, K<Arr, A> ta)
    {
        var arr = ta.As();
        return index.Value >= 0 && index.Value < arr.Length
                   ? Some(arr[index])
                   : Option<A>.None;
    }

    static Arr<A> Foldable<Arr>.ToArr<A>(K<Arr, A> ta) =>
        ta.As();

    static Lst<A> Foldable<Arr>.ToLst<A>(K<Arr, A> ta) =>
        new(ta.As());

    static Iterable<A> Foldable<Arr>.ToIterable<A>(K<Arr, A> ta) =>
        ta.As().AsIterable();
    
    static Seq<A> Foldable<Arr>.ToSeq<A>(K<Arr, A> ta) =>
        Seq.FromArray(ta.As().ToArray());
    
    static K<F, K<Arr, B>> Traversable<Arr>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Arr, A> ta)
    {
        return Foldable.fold(addItem, F.Pure(new SeqStrict<B>(new B[ta.As().Count], 0, 0, 0, 0)), ta)
                       .Map(bs => new Arr<B>(bs.data.AsSpan().Slice(bs.start, bs.Count)).Kind());

        K<F, SeqStrict<B>> addItem(K<F, SeqStrict<B>> state, A value) =>
            Applicative.lift((bs, b) => (SeqStrict<B>)bs.Add(b), state, f(value));                                            
    }

    static K<F, K<Arr, B>> Traversable<Arr>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Arr, A> ta)
    {
        return ta.FoldM((bs, a) => f(a).Map(bs.Add), Seq<B>.Empty)
                 .Map(bs => create(bs.AsSpan()).Kind());
        
        /*
        return Foldable.fold(addItem, F.Pure(new SeqStrict<B>(new B[ta.As().Count], 0, 0, 0, 0)), ta)
                       .Map(bs => new Arr<B>(bs.data.AsSpan().Slice(bs.start, bs.Count)).Kind());

        K<F, SeqStrict<B>> addItem(K<F, SeqStrict<B>> state, A value) =>
            state.Bind(bs => f(value).Map(b => (SeqStrict<B>)bs.Add(b))); 
    */
    }

    static K<Seq, A> Natural<Arr, Seq>.Transform<A>(K<Arr, A> fa) => 
        toSeq(fa.As().ToSeq());

    static K<Iterable, A> Natural<Arr, Iterable>.Transform<A>(K<Arr, A> fa) => 
        fa.As().AsIterable();

    static K<Lst, A> Natural<Arr, Lst>.Transform<A>(K<Arr, A> fa) => 
        toList(fa.As());

    static K<Set, A> Natural<Arr, Set>.Transform<A>(K<Arr, A> fa) => 
        toSet(fa.As());

    static K<HashSet, A> Natural<Arr, HashSet>.Transform<A>(K<Arr, A> fa) => 
        toHashSet(fa.As());
}
