using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Identity : 
    Monad<Identity>, 
    Traversable<Identity>,
    Foldable<Identity, Identity.FoldState>
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
    
    static Fold<A, S> Foldable<Identity>.FoldStep<A, S>(K<Identity, A> ta, in S initialState) =>
        Fold.Loop(initialState, ta.As().Value, Fold.Done<A, S>);
        
    static Fold<A, S> Foldable<Identity>.FoldStepBack<A, S>(K<Identity, A> ta, in S initialState) =>
        ta.FoldStep(initialState);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Traversable
    //
    
    static K<F, K<Identity, B>> Traversable<Identity>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Identity, A> ta) =>
        F.Map(PureK, f(ta.As().Value));

    static void Foldable<Identity, FoldState>.FoldStepSetup<A>(K<Identity, A> ta, ref FoldState refState) =>
        refState = new FoldState(false);

    static bool Foldable<Identity, FoldState>.FoldStep<A>(K<Identity, A> ta, ref FoldState refState, out A value)
    {
        if (refState.HasRun)
        {
            value = default!;
            return false;
        }
        else
        {
            value = ta.As().Value;
            refState = new FoldState(true);
            return true;
        }
    }

    static void Foldable<Identity, FoldState>.FoldStepBackSetup<A>(K<Identity, A> ta, ref FoldState refState) =>
        refState = new FoldState(false);

    static bool Foldable<Identity, FoldState>.FoldStepBack<A>(K<Identity, A> ta, ref FoldState refState, out A value)
    {
        if (refState.HasRun)
        {
            value = default!;
            return false;
        }
        else
        {
            value = ta.As().Value;
            refState = new FoldState(true);
            return true;
        }
    }
}
