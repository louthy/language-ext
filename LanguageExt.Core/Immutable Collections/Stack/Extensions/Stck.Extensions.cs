using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StckExtensions
{
    extension<A>(A top)
    {
        public Stck<A> Top(Stck<A> tail) =>
            new Stck<A>.Top(top, tail);
    }

    extension<A, B>(K<Stck, Func<A, B>> mf)
    {
        public Stck<B> Apply(K<Stck, A> ma) =>
            +Applicative.apply(mf, ma);

        public Stck<B> Apply(Memo<Stck, A> ma) =>
            +Applicative.apply(mf, ma);
    }

    extension<A>(K<Stck, A> ma)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public Stck<A> As() =>
            (Stck<A>)ma;
            
        public Stck<A> Combine(K<Stck, A> rhs) => 
            ma.As() + rhs;

        public Stck<B> Map<B>(Func<A, B> f) =>
            +Functor.map(f, ma);

        public Stck<B> Bind<B>(Func<A, K<Stck, B>> f) =>
            +Monad.bind(ma, f);

        public Stck<B> Bind<B>(Func<A, Stck<B>> f) =>
            +Monad.bind(ma, f);

        public Stck<C> SelectMany<B, C>(Func<A, K<Stck, B>> bind, Func<A, B, C> project) =>
            ma.Bind(a => bind(a).Map(b => project(a, b)));

        public Stck<A> Where(Func<A, bool> f) =>
            ma.Filter(f);

        public Stck<A> Filter(Func<A, bool> f)
        {
            Stck.FoldState state = default;
            Foldable.stepSetup(ma, ref state);
            var stack = new Stck<A>.Top(default!, Stck<A>.Empty);
            var top   = stack;
            while (Foldable.step(ma, ref state, out var x))
            {
                if (f(x))
                {
                    var nstack = new Stck<A>.Top(x, Stck<A>.Empty);
                    stack.Rest = nstack;
                    stack = nstack;
                }
            }
            return top.Rest;
        }
        
        public Stck<B> Choose<B>(Func<A, Option<B>> f)
        {
            Stck.FoldState state = default;
            Foldable.stepSetup(ma, ref state);
            var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
            var top   = stack;
            while (Foldable.step(ma, ref state, out var x))
            {
                var ox = f(x);
                if (ox.IsSome)
                {
                    var nstack = new Stck<B>.Top((B)ox, Stck<B>.Empty);
                    stack.Rest = nstack;
                    stack = nstack;
                }
            }
            return top.Rest;
        }
        
        public Stck<A> Take(int amount)
        {
            Stck.FoldState state = default;
            Foldable.stepSetup(ma, ref state);
            var stack = new Stck<A>.Top(default!, Stck<A>.Empty);
            var top   = stack;
            while (Foldable.step(ma, ref state, out var x))
            {
                if(amount == 0) return top.Rest;
                var nstack = new Stck<A>.Top(x, Stck<A>.Empty);
                stack.Rest = nstack;
                stack = nstack;
                amount--;
            }
            return top.Rest;
        }
        
        public Stck<A> Skip(int amount)
        {
            var top = +ma;
            while (amount > 0)
            {
                if (top is Stck<A>.Top(_, var rest))
                {
                    top = rest;
                    amount--;
                }
                else
                {
                    return Stck<A>.Empty;
                }
            }
            return top;
        }

        public Stck<A> Reverse() =>
            ma.As().Reverse();
    }
}
