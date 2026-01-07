using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Stck :
    Monad<Stck>,
    Foldable<Stck, Stck.FoldState>,
    MonoidK<Stck>
{
    static Fold<A, S> Foldable<Stck>.FoldStep<A, S>(K<Stck, A> ta, in S initialState)
    {
        return go(ta.As())(initialState);

        Func<S, Fold<A, S>> go(Stck<A> stack) =>
            state =>
                stack switch
                {
                    Stck<A>.Nil               => Fold.Done<A, S>(state),
                    Stck<A>.Top(var t, var r) => Fold.Loop(state, t, go(r)),
                    _                         => throw new InvalidOperationException("Invalid stack state")
                };
    }

    static Fold<A, S> Foldable<Stck>.FoldStepBack<A, S>(K<Stck, A> ta, in S initialState) 
    {
        return go(ta.As().Reverse())(initialState);

        Func<S, Fold<A, S>> go(Stck<A> stack) =>
            state =>
                stack switch
                {
                    Stck<A>.Nil               => Fold.Done<A, S>(state),
                    Stck<A>.Top(var t, var r) => Fold.Loop(state, t, go(r)),
                    _                         => throw new InvalidOperationException("Invalid stack state")
                };
    }

    static K<Stck, A> SemigroupK<Stck>.Combine<A>(K<Stck, A> lhs, K<Stck, A> rhs) => 
        +lhs + +rhs;

    static K<Stck, A> MonoidK<Stck>.Empty<A>() => 
        Stck<A>.Empty;

    static K<Stck, B> Functor<Stck>.Map<A, B>(Func<A, B> f, K<Stck, A> ma)
    {
        FoldState state = default;
        ma.StepSetup(ref state);
        var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
        var top   = stack;
        while (ma.Step(ref state, out var x))
        {
            var nstack = new Stck<B>.Top(f(x), Stck<B>.Empty);
            stack.Rest = nstack;
            stack = nstack;
        }
        return top.Rest;
    }

    static void Foldable<Stck, FoldState>.FoldStepSetup<A>(K<Stck, A> ta, ref FoldState refState)
    {
        var ma = +ta.As();
        FoldState.Setup(ma, ref refState);
    }

    static bool Foldable<Stck, FoldState>.FoldStep<A>(K<Stck, A> ta, ref FoldState refState, out A value) =>
        FoldState.Step(ref refState, out value);

    static void Foldable<Stck, FoldState>.FoldStepBackSetup<A>(K<Stck, A> ta, ref FoldState refState)
    {
        var rs = ta.As().Reverse();
        FoldState.Setup(rs, ref refState);
    }

    static bool Foldable<Stck, FoldState>.FoldStepBack<A>(K<Stck, A> ta, ref FoldState refState, out A value) => 
        FoldState.Step(ref refState, out value);

    static K<Stck, A> Applicative<Stck>.Pure<A>(A value) =>
        singleton(value);

    static K<Stck, B> Applicative<Stck>.Apply<A, B>(K<Stck, Func<A, B>> mf, K<Stck, A> ma)
    {
        FoldState mfs = default;
        mf.StepSetup(ref mfs);
        var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
        var top   = stack;
        while (mf.Step(ref mfs, out var f))
        {
            FoldState mas = default;
            ma.StepSetup(ref mas);
            while (ma.Step(ref mas, out var a))
            {
                var nstack = new Stck<B>.Top(f(a), Stck<B>.Empty);
                stack.Rest = nstack;
                stack = nstack;
            }
        }
        return top.Rest;
    }

    static K<Stck, B> Applicative<Stck>.Apply<A, B>(K<Stck, Func<A, B>> mf, Memo<Stck, A> ma) 
    {
        FoldState mfs = default;
        mf.StepSetup(ref mfs);
        var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
        var top = stack;
        while (mf.Step(ref mfs, out var f))
        {
            var       ka  = ma.Value.As();
            FoldState mas = default;
            ka.StepSetup(ref mas);
            while (ka.Step(ref mas, out var a))
            {
                var nstack = new Stck<B>.Top(f(a), Stck<B>.Empty);
                stack.Rest = nstack;
                stack = nstack;
            }
        }
        return top.Rest;
    }

    static K<Stck, B> Monad<Stck>.Bind<A, B>(K<Stck, A> ma, Func<A, K<Stck, B>> f) 
    {
        FoldState mas = default;
        ma.StepSetup(ref mas);
        var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
        var top   = stack;
        while (ma.Step(ref mas, out var a))
        {
            var mb = f(a);
            FoldState mbs = default;
            mb.StepSetup(ref mbs);
            while (mb.Step(ref mbs, out var b))
            {
                var nstack = new Stck<B>.Top(b, Stck<B>.Empty);
                stack.Rest = nstack;
                stack = nstack;
            }
        }
        return top.Rest;
    }

    static K<Stck, B> Monad<Stck>.Recur<A, B>(A value, Func<A, K<Stck, Next<A, B>>> f) =>
        createRange(Monad.enumerableRecur(value, x => f(x).As().AsEnumerable()));
}
