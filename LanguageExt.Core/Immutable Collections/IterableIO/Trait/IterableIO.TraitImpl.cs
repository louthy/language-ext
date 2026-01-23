using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class IterableIO : 
    Monad<IterableIO>, 
    MonoidK<IterableIO>,
    FoldableIO<IterableIO>,
    NaturalIso<IterableIO, IteratorIO>
{
    static K<IterableIO, B> Monad<IterableIO>.Recur<A, B>(A value, Func<A, K<IterableIO, Next<A, B>>> f) =>
        Monad.iterableRecurIO(value, f);
    
    static K<IterableIO, B> Monad<IterableIO>.Bind<A, B>(K<IterableIO, A> ma, Func<A, K<IterableIO, B>> f) =>
        ma.As().Bind(f);

    static K<IterableIO, B> Functor<IterableIO>.Map<A, B>(Func<A, B> f, K<IterableIO, A> ma) => 
        ma.As().Map(f);

    static K<IterableIO, A> Applicative<IterableIO>.Pure<A>(A value) =>
        singleton(value);

    static K<IterableIO, B> Applicative<IterableIO>.Apply<A, B>(K<IterableIO, Func<A, B>> mf, K<IterableIO, A> ma) =>
        mf >> ma.Map;

    static K<IterableIO, B> Applicative<IterableIO>.Apply<A, B>(K<IterableIO, Func<A, B>> mf, Memo<IterableIO, A> ma) =>
        mf >> ma.Map;

    static K<IterableIO, A> MonoidK<IterableIO>.Empty<A>() =>
        empty<A>();
    
    static K<IterableIO, A> SemigroupK<IterableIO>.Combine<A>(K<IterableIO, A> ma, K<IterableIO, A> mb) =>
        ma.As().Concat(+mb);

    static IteratorIO<A> IterableKIO<IterableIO>.ForwardIteratorIO<A>(K<IterableIO, A> fa) =>
        fa.As().iterator;

    /*
    static K<F, K<IterableIO, B>> Traversable<IterableIO>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<IterableIO, A> ta) =>
        ta.As().Traverse(f).Map(mb => mb.Kind());

    static K<F, K<IterableIO, B>> Traversable<IterableIO>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<IterableIO, A> ta) =>
        ta.As().TraverseM(f).Map(mb => mb.Kind());

    static IterableIO<A> Foldable<IterableIO>.ToIterableIO<A>(K<IterableIO, A> ta) =>
        ta.As();

    static K<Seq, A> Natural<IterableIO, Seq>.Transform<A>(K<IterableIO, A> fa) =>
        toSeq(fa.As());

    static K<Arr, A> Natural<IterableIO, Arr>.Transform<A>(K<IterableIO, A> fa) =>
        toArr(fa.As());

    static K<Lst, A> Natural<IterableIO, Lst>.Transform<A>(K<IterableIO, A> fa) =>
        toLst(fa.As());

    static K<Set, A> Natural<IterableIO, Set>.Transform<A>(K<IterableIO, A> fa) =>
        toSet(fa.As());

    static K<HashSet, A> Natural<IterableIO, HashSet>.Transform<A>(K<IterableIO, A> fa) =>
        toHashSet(fa.As());

    static K<IterableIO, A> Natural<IterableIO, IterableIO>.Transform<A>(K<IterableIO, A> fa) =>
        fa;

    static K<IterableIO, A> CoNatural<IterableIO, IterableIO>.CoTransform<A>(K<IterableIO, A> fa) =>
        fa;

    static Iterator<A> IterableIOK<IterableIO>.ForwardIterator<A>(K<IterableIO, A> fa) =>
        fa.As().ForwardIterator();*/

    static K<IteratorIO, A> Natural<IterableIO, IteratorIO>.Transform<A>(K<IterableIO, A> fa) => 
        fa.As().iterator;

    static K<IterableIO, A> CoNatural<IterableIO, IteratorIO>.CoTransform<A>(K<IteratorIO, A> fa) => 
        new IterableIO<A>(+fa);
}
