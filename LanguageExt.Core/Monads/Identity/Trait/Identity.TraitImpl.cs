using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Identity : 
    Monad<Identity>, 
    Traversable<Identity>,
    Foldable<Identity, SingletonFoldState>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monad
    //
    
    static K<Identity, B> Monad<Identity>.Bind<A, B>(K<Identity, A> ma, Func<A, K<Identity, B>> f) =>
        ma.As().Bind(f);

    static K<Identity, B> Monad<Identity>.Recur<A, B>(A value, Func<A, K<Identity, Next<A, B>>> f)
    {
        while (true)
        {
            var mr = +f(value);
            if (mr.Value.IsDone) return new Identity<B>(mr.Value.Done);
            value = mr.Value.Loop;
        }
    }

    static K<Identity, B> Functor<Identity>.Map<A, B>(Func<A, B> f, K<Identity, A> ma) => 
        ma.As().Map(f);

    static K<Identity, A> Applicative<Identity>.Pure<A>(A value) =>
        new Identity<A>(value);

    static K<Identity, B> Applicative<Identity>.Apply<A, B>(K<Identity, Func<A, B>> mf, K<Identity, A> ma) =>
        mf.As().Bind(f => ma.As().Map(f));

    static K<Identity, B> Applicative<Identity>.Apply<A, B>(K<Identity, Func<A, B>> mf, Memo<Identity, A> ma) =>
        mf.As().Bind(f => ma.Value.As().Map(f));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Foldable
    //

    static void Foldable<Identity, SingletonFoldState>.FoldStepSetup<A>(K<Identity, A> ta, ref SingletonFoldState refState)
    {
        // Nothing to do
    }

    static bool Foldable<Identity, SingletonFoldState>.FoldStep<A>(K<Identity, A> ta, ref SingletonFoldState refState, out A value)
    {
        if (refState.ShouldYield())
        {
            value = ta.As().Value;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Traversable
    //
    
    static K<F, K<Identity, B>> Traversable<Identity>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Identity, A> ta) =>
        F.Map(PureK, f(ta.As().Value));


    static Iterator<A> IterableK<Identity>.ForwardIterator<A>(K<Identity, A> fa) =>
        Iterator.singleton(fa.As().Value);
}
