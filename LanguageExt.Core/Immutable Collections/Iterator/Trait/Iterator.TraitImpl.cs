using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class Iterator : 
    Monad<Iterator>,
    MonoidK<Iterator>,
    Alternative<Iterator>, 
    Traversable<Iterator>,
    Natural<Iterator, Arr>,
    Natural<Iterator, Seq>,
    Natural<Iterator, Lst>,
    Natural<Iterator, Set>,
    Natural<Iterator, Iterable>,
    Natural<Iterator, HashSet>
{
    static K<Iterator, B> Monad<Iterator>.Recur<A, B>(A value, Func<A, K<Iterator, Next<A, B>>> f) =>
        Monad.unsafeRecur(value, f);
    
    static K<Iterator, B> Monad<Iterator>.Bind<A, B>(K<Iterator, A> ma, Func<A, K<Iterator, B>> f) =>
        ma.As().Bind(f);

    static K<Iterator, B> Functor<Iterator>.Map<A, B>(Func<A, B> f, K<Iterator, A> ma) => 
        ma.As().Map(f);

    static K<Iterator, A> Applicative<Iterator>.Pure<A>(A value) =>
        singleton(value);

    static K<Iterator, B> Applicative<Iterator>.Apply<A, B>(K<Iterator, Func<A, B>> mf, K<Iterator, A> ma) =>
        ma.As().ApplyBack(+mf);

    static K<Iterator, B> Applicative<Iterator>.Apply<A, B>(K<Iterator, Func<A, B>> mf, Memo<Iterator, A> ma) =>
        ma.Value.As().ApplyBack(+mf);

    static K<Iterator, A> MonoidK<Iterator>.Empty<A>() =>
        Iterator<A>.Empty;

    static K<Iterator, A> Alternative<Iterator>.Empty<A>() =>
        Iterator<A>.Empty;

    static K<Iterator, A> SemigroupK<Iterator>.Combine<A>(K<Iterator, A> ma, K<Iterator, A> mb) =>
        ma.As().Combine(mb.As());

    static K<Iterator, A> Choice<Iterator>.Choose<A>(K<Iterator, A> ma, K<Iterator, A> mb) =>
        new Iterator<A>.OpAlt(+ma, +mb);
    
    static K<Iterator, A> Choice<Iterator>.Choose<A>(K<Iterator, A> ma, Memo<Iterator, A> mb) => 
        new Iterator<A>.OpAltMemo(+ma, mb);
    
    static K<F, K<Iterator, B>> Traversable<Iterator>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Iterator, A> ta)
    {
        return Foldable.fold(add, F.Pure(Iterator<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, Iterator<B>> add(K<F, Iterator<B>> state, A value) =>
              Applicative.lift((bs, b) => bs + b, state, f(value));                                            
    }

    static K<F, K<Iterator, B>> Traversable<Iterator>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Iterator, A> ta) 
    {
        return Foldable.fold(add, F.Pure(Iterator<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, Iterator<B>> add(K<F, Iterator<B>> state, A value) =>
            state.Bind(bs => f(value).Bind(b => F.Pure(bs + b))); 
    }
    
    static Arr<A> Foldable<Iterator>.ToArr<A>(K<Iterator, A> ta) =>
        ta is Iterator<A>.IterArr arr 
            ? arr.Array
            : ta.As().ToArr();

    static Lst<A> Foldable<Iterator>.ToLst<A>(K<Iterator, A> ta) =>
        new(ta.As());

    static Iterable<A> Foldable<Iterator>.ToIterable<A>(K<Iterator, A> ta) =>
        ta.As().AsIterable();
    
    static Seq<A> Foldable<Iterator>.ToSeq<A>(K<Iterator, A> ta) =>
        new(ta.As());
        
    static Iterator<A> IterableK<Iterator>.ForwardIterator<A>(K<Iterator, A> fa) => 
        +fa;

    static K<Seq, A> Natural<Iterator, Seq>.Transform<A>(K<Iterator, A> fa) => 
        toSeq(fa.As());

    static K<Arr, A> Natural<Iterator, Arr>.Transform<A>(K<Iterator, A> fa) => 
        toArray(fa.As());

    static K<Lst, A> Natural<Iterator, Lst>.Transform<A>(K<Iterator, A> fa) => 
        toLst(fa.As());

    static K<Set, A> Natural<Iterator, Set>.Transform<A>(K<Iterator, A> fa) => 
        toSet(fa.As());

    static K<HashSet, A> Natural<Iterator, HashSet>.Transform<A>(K<Iterator, A> fa) => 
        toHashSet(fa.As());
    
    static K<Iterable, A> Natural<Iterator, Iterable>.Transform<A>(K<Iterator, A> fa) => 
        new IterableIterator<A>(fa.As());
}
