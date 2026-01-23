using System;
using System.Linq;
using LanguageExt.Traits;
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
    Foldable<Arr, Arr.FoldState>,
    FoldableBack<Arr, Arr.FoldState>
{
    static K<Arr, B> Monad<Arr>.Bind<A, B>(K<Arr, A> ma, Func<A, K<Arr, B>> f)
    {
        var writer = ArrayWriterRef<B>.Init();
        
        var astate = ma.StepSetup<Arr, FoldState, A>();
        while (ma.Step(ref astate, out var a))
        {
            var mb     = +f(a);
            var bstate = mb.StepSetup<Arr, FoldState, B>();
            while (mb.Step(ref bstate, out var b))
            {
                writer.Add(b);
            }
        }
        return writer.ToArr();
    }

    static K<Arr, B> Monad<Arr>.Recur<A, B>(A value, Func<A, K<Arr, Next<A, B>>> f) =>
        createRange(Monad.enumerableRecur(value, x => f(x).As().AsEnumerable()));

    static K<Arr, B> Functor<Arr>.Map<A, B>(Func<A, B> f, K<Arr, A> ma) => 
        ma.As().Map(f);

    static K<Arr, A> Applicative<Arr>.Pure<A>(A value) =>
        singleton(value);

    static K<Arr, B> Applicative<Arr>.Apply<A, B>(K<Arr, Func<A, B>> mf, K<Arr, A> ma)
    {
        var writer = ArrayWriterRef<B>.Init();
        
        var fstate = mf.StepSetup<Arr, FoldState, Func<A, B>>();
        while (mf.Step(ref fstate, out var f))
        {
            var astate = ma.StepSetup<Arr, FoldState, A>();
            while (ma.Step(ref astate, out var a))
            {
                writer.Add(f(a));
            }
        }
        return writer.ToArr();
    }    

    static K<Arr, B> Applicative<Arr>.Apply<A, B>(K<Arr, Func<A, B>> mf, Memo<Arr, A> ma)
    {
        var writer = ArrayWriterRef<B>.Init();
        
        var fstate = mf.StepSetup<Arr, FoldState, Func<A, B>>();
        while (mf.Step(ref fstate, out var f))
        {
            var fa     = ma.Value;
            var astate = fa.StepSetup<Arr, FoldState, A>();
            while (fa.Step(ref astate, out var a))
            {
                writer.Add(f(a));
            }
        }
        return writer.ToArr();
    }

    static K<Arr, A> MonoidK<Arr>.Empty<A>() =>
        Arr<A>.Empty;

    static K<Arr, A> Alternative<Arr>.Empty<A>() =>
        Arr<A>.Empty;

    static K<Arr, A> SemigroupK<Arr>.Combine<A>(K<Arr, A> ma, K<Arr, A> mb)
    {
        var fa     = +ma;
        var fb     = +mb;
        var writer = ArrayWriterRef<A>.Init(fa.Count + fb.Count);
        writer.AddRange(fa.AsSpan());
        writer.AddRange(fb.AsSpan());
        return writer.ToArr();
    }    
    
    static K<Arr, A> Choice<Arr>.Choose<A>(K<Arr, A> ma, K<Arr, A> mb) => 
        ma.IsEmpty ? mb : ma;

    static K<Arr, A> Choice<Arr>.Choose<A>(K<Arr, A> ma, Memo<Arr, A> mb) => 
        ma.IsEmpty ? mb.Value : ma;

    static long Foldable<Arr>.Count<A>(K<Arr, A> ta) =>
        ta.As().Count;

    static bool Foldable<Arr>.IsEmpty<A>(K<Arr, A> ta) =>
        ta.As().IsEmpty;

    static FoldState IterableK<Arr, FoldState>.StepSetup<A>(K<Arr, A> ta) =>
        FoldState.Setup(ta.As().AsSpan());

    static FoldState IterableBackK<Arr, FoldState>.StepBackSetup<A>(K<Arr, A> ta) =>
        FoldState.SetupBack(ta.As().AsSpan());

    static bool IterableK<Arr, FoldState>.Step<A>(K<Arr, A> ta, ref FoldState state, out A value) =>
        FoldState.MoveNext(ref state, out value);

    static bool IterableBackK<Arr, FoldState>.StepBack<A>(K<Arr, A> ta, ref FoldState state, out A value) =>
        FoldState.MovePrev(ref state, out value);

    static Option<A> Foldable<Arr>.At<A>(long index, K<Arr, A> ta)
    {
        var arr = ta.As();
        return index >= 0 && index < arr.Count
                   ? Some(arr[index])
                   : Option<A>.None;
    }

    static Option<A> FoldableBack<Arr>.AtBack<A>(long index, K<Arr, A> ta)
    {
        var arr = ta.As();
        return index > 0 && index <= arr.Count
                   ? Some(arr[arr.Count - 1 - index])
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
                       .Map(bs => new Arr<B>(bs.data, bs.start, bs.Count).Kind());

        K<F, SeqStrict<B>> addItem(K<F, SeqStrict<B>> state, A value) =>
            Applicative.lift((bs, b) => (SeqStrict<B>)bs.Add(b), state, f(value));                                            
    }

    static K<F, K<Arr, B>> Traversable<Arr>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Arr, A> ta) =>
        ta.FoldM((bs, a) => f(a).Map(bs.Add), Seq<B>.Empty)
          .Map(bs => create(bs.AsSpan()).Kind());

    static K<Seq, A> Natural<Arr, Seq>.Transform<A>(K<Arr, A> fa) => 
        toSeq(fa.As().ToSeq());

    static K<Iterable, A> Natural<Arr, Iterable>.Transform<A>(K<Arr, A> fa) => 
        fa.As().AsIterable();

    static K<Lst, A> Natural<Arr, Lst>.Transform<A>(K<Arr, A> fa) => 
        toLst(fa.As());

    static K<Set, A> Natural<Arr, Set>.Transform<A>(K<Arr, A> fa) => 
        toSet(fa.As());

    static K<HashSet, A> Natural<Arr, HashSet>.Transform<A>(K<Arr, A> fa) => 
        toHashSet(fa.As());

    public static Iterator<A> ForwardIterator<A>(K<Arr, A> fa)
    {
        var items = +fa;
        return new Iterator<A>.IterArr(items, 0, items.Count);
    }
    
    public static Iterator<A> BackwardIterator<A>(K<Arr, A> fa)
    {
        var items = +fa;
        return new Iterator<A>.IterArrBkwd(items, items.Count - 1, items.Count);
    }
}
