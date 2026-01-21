using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class IteratorIO : 
    Monad<IteratorIO>,
    MonoidK<IteratorIO>,
    Alternative<IteratorIO>, 
  //Traversable<IteratorIO>,
    FoldableIO<IteratorIO>,
    NaturalEpi<IteratorIO, IteratorIO>
    /*Natural<IteratorIO, Arr>,
    Natural<IteratorIO, Seq>,
    Natural<IteratorIO, Lst>,
    Natural<IteratorIO, Set>,
    Natural<IteratorIO, Iterable>,
    Natural<IteratorIO, HashSet>*/
{
    static K<IteratorIO, B> Monad<IteratorIO>.Recur<A, B>(A value, Func<A, K<IteratorIO, Next<A, B>>> f) =>
        Monad.unsafeRecur(value, f);
    
    static K<IteratorIO, B> Monad<IteratorIO>.Bind<A, B>(K<IteratorIO, A> ma, Func<A, K<IteratorIO, B>> f) =>
        ma.As().Bind(f);

    static K<IteratorIO, B> Functor<IteratorIO>.Map<A, B>(Func<A, B> f, K<IteratorIO, A> ma) => 
        ma.As().Map(f);

    static K<IteratorIO, A> Applicative<IteratorIO>.Pure<A>(A value) =>
        singleton(value);

    static K<IteratorIO, B> Applicative<IteratorIO>.Apply<A, B>(K<IteratorIO, Func<A, B>> mf, K<IteratorIO, A> ma) =>
        ma.As().ApplyBack(+mf);

    static K<IteratorIO, B> Applicative<IteratorIO>.Apply<A, B>(K<IteratorIO, Func<A, B>> mf, Memo<IteratorIO, A> ma) =>
        ma.Value.As().ApplyBack(+mf);

    static K<IteratorIO, A> MonoidK<IteratorIO>.Empty<A>() =>
        IteratorIO<A>.Empty;

    static K<IteratorIO, A> Alternative<IteratorIO>.Empty<A>() =>
        IteratorIO<A>.Empty;

    static K<IteratorIO, A> SemigroupK<IteratorIO>.Combine<A>(K<IteratorIO, A> ma, K<IteratorIO, A> mb) =>
        ma.As().Combine(mb.As());

    static K<IteratorIO, A> Choice<IteratorIO>.Choose<A>(K<IteratorIO, A> ma, K<IteratorIO, A> mb) =>
        new IteratorIO<A>.OpAlt(+ma, +mb);
    
    static K<IteratorIO, A> Choice<IteratorIO>.Choose<A>(K<IteratorIO, A> ma, Memo<IteratorIO, A> mb) => 
        new IteratorIO<A>.OpAltMemo(+ma, mb);
    
    /*static K<F, K<IteratorIO, B>> Traversable<IteratorIO>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<IteratorIO, A> ta)
    {
        return Foldable.fold(add, F.Pure(IteratorIO<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, IteratorIO<B>> add(K<F, IteratorIO<B>> state, A value) =>
              Applicative.lift((bs, b) => bs + b, state, f(value));                                            
    }

    static K<F, K<IteratorIO, B>> Traversable<IteratorIO>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<IteratorIO, A> ta) 
    {
        return Foldable.fold(add, F.Pure(IteratorIO<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, IteratorIO<B>> add(K<F, IteratorIO<B>> state, A value) =>
            state.Bind(bs => f(value).Bind(b => F.Pure(bs + b))); 
    }*/
    
        
    static IteratorIO<A> IterableKIO<IteratorIO>.ForwardIteratorIO<A>(K<IteratorIO, A> fa) => 
        +fa;

    /*static K<Seq, A> Natural<IteratorIO, Seq>.Transform<A>(K<IteratorIO, A> fa) => 
        new Seq<A>(fa.As());

    static K<Arr, A> Natural<IteratorIO, Arr>.Transform<A>(K<IteratorIO, A> fa) => 
        fa.As().ToArr();

    static K<Lst, A> Natural<IteratorIO, Lst>.Transform<A>(K<IteratorIO, A> fa) => 
        toLst(fa.As());

    static K<Set, A> Natural<IteratorIO, Set>.Transform<A>(K<IteratorIO, A> fa) => 
        toSet(fa.As());

    static K<HashSet, A> Natural<IteratorIO, HashSet>.Transform<A>(K<IteratorIO, A> fa) => 
        toHashSet(fa.As());
    
    static K<Iterable, A> Natural<IteratorIO, Iterable>.Transform<A>(K<IteratorIO, A> fa) => 
        new IterableIteratorIO<A>(fa.As());*/
    static K<IteratorIO, A> Natural<IteratorIO, IteratorIO>.Transform<A>(K<IteratorIO, A> fa) => 
        fa;

    static K<IteratorIO, A> CoNatural<IteratorIO, IteratorIO>.CoTransform<A>(K<IteratorIO, A> fa) => 
        fa;
}
