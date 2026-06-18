using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Stck :
    Monad<Stck>,
    MonoidK<Stck>,
    Countable<Stck>,
    Foldable<Stck, Stck.FoldState>
{
    static K<Stck, A> SemigroupK<Stck>.Combine<A>(K<Stck, A> lhs, K<Stck, A> rhs) => 
        +lhs + +rhs;

    static K<Stck, A> MonoidK<Stck>.Empty<A>() => 
        Stck<A>.Empty;

    static K<Stck, B> Functor<Stck>.Map<A, B>(Func<A, B> f, K<Stck, A> ma)
    {
        var state = ma.StepSetup<Stck, FoldState, A>();
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

    static FoldState IterableK<Stck, FoldState>.StepSetup<A>(K<Stck, A> ta)
    {
        var ma = +ta.As();
        return FoldState.Setup(ma);
    }

    static bool IterableK<Stck, FoldState>.Step<A>(K<Stck, A> ta, ref FoldState refState, out A value) =>
        FoldState.Step(ref refState, out value);

    static K<Stck, A> Applicative<Stck>.Pure<A>(A value) =>
        singleton(value);

    static K<Stck, B> Applicative<Stck>.Apply<A, B>(K<Stck, Func<A, B>> mf, K<Stck, A> ma)
    {
        var mfs   = mf.StepSetup<Stck, FoldState, Func<A, B>>();
        var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
        var top   = stack;
        while (mf.Step(ref mfs, out var f))
        {
            var mas = ma.StepSetup<Stck, FoldState, A>();
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
        var mfs   = mf.StepSetup<Stck, FoldState, Func<A, B>>();
        var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
        var top   = stack;
        while (mf.Step(ref mfs, out var f))
        {
            var       ka  = ma.Value.As();
            var mas = ka.StepSetup<Stck, FoldState, A>();
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
        var mas   = ma.StepSetup<Stck, FoldState, A>();
        var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
        var top   = stack;
        while (ma.Step(ref mas, out var a))
        {
            var mb  = f(a);
            var mbs = mb.StepSetup<Stck, FoldState, B>();
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
    
    static Iterator<A> IterableK<Stck>.ForwardIterator<A>(K<Stck, A> fa) =>
        fa switch
        {
            Stck<A>.Top(var t, var r) => Iterator.cons(t, () => r.ForwardIterator()),
            _                         => Iterator.empty<A>()
        };

    static long Countable<Stck>.Count<A>(K<Stck, A> fa) => 
        fa.As().Count;
}
