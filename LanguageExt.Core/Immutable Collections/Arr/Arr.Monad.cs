using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Arr : 
    Monad<Arr>, 
    Traversable<Arr>, 
    Alternative<Arr>,
    Natural<Arr, Seq>,
    Natural<Arr, Iterable>,
    Natural<Arr, Lst>,
    Natural<Arr, Set>,
    Natural<Arr, HashSet>
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
        var ff   = mf.As();
        var fa   = ma.As();
        var size = ff.Count * fa.Count;
        var bs   = new B[size];
        var ix   = 0;
        for (var i = 0; i < ff.Count; i++)
        {
            for (var j = 0; j < fa.Count; j++)
            {
                bs[ix] = ff[i](fa[j]);
            }
        }
        return new Arr<B>(bs);
    }    

    static K<Arr, B> Applicative<Arr>.Action<A, B>(K<Arr, A> ma, K<Arr, B> mb) => 
        mb;

    static K<Arr, A> MonoidK<Arr>.Empty<A>() =>
        Arr<A>.Empty;

    static K<Arr, A> SemigroupK<Arr>.Combine<A>(K<Arr, A> ma, K<Arr, A> mb) =>
        ma.As() + mb.As();
    
    static K<Arr, A> Choice<Arr>.Choose<A>(K<Arr, A> ma, K<Arr, A> mb) => 
        ma.IsEmpty() ? mb : ma;
    
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

    static Iterable<A> Foldable<Arr>.ToIterable<A>(K<Arr, A> ta) =>
        ta.As().AsIterable();
    
    static Seq<A> Foldable<Arr>.ToSeq<A>(K<Arr, A> ta) =>
        Seq.FromArray(ta.As().Value);
    
    static K<F, K<Arr, B>> Traversable<Arr>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Arr, A> ta)
    {
        return Foldable.fold(addItem, F.Pure(new SeqStrict<B>(new B[ta.As().Count], 0, 0, 0, 0)), ta)
                       .Map(bs => new Arr<B>(bs.data.AsSpan().Slice(bs.start, bs.Count)).Kind());

        Func<K<F, SeqStrict<B>>, K<F, SeqStrict<B>>> addItem(A value) =>
            state =>
                Applicative.lift((bs, b) => (SeqStrict<B>)bs.Add(b), state, f(value));                                            
    }

    static K<F, K<Arr, B>> Traversable<Arr>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Arr, A> ta) 
    {
        return Foldable.fold(addItem, F.Pure(new SeqStrict<B>(new B[ta.As().Count], 0, 0, 0, 0)), ta)
                       .Map(bs => new Arr<B>(bs.data.AsSpan().Slice(bs.start, bs.Count)).Kind());

        Func<K<F, SeqStrict<B>>, K<F, SeqStrict<B>>> addItem(A value) =>
            state =>
                state.Bind(
                    bs => f(value).Bind(
                        b => F.Pure((SeqStrict<B>)bs.Add(b)))); 
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
