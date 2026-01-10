using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StckExtensions
{
    extension<A>(A top)
    {
        /// <summary>
        /// Construct a stack from a new top value (`this`) pushed to an existing stack (`rest`)
        /// </summary>
        /// <remarks>
        /// This is the exact same concept as singly linked `Cons` lists.
        /// </remarks>
        /// <param name="rest">The stack to push to</param>
        /// <returns>Constructed stack</returns>
        public Stck<A> Top(Stck<A> rest) =>
            new Stck<A>.Top(top, rest);
    }

    extension<A, B>(K<Stck, Func<A, B>> mf)
    {
        /// <summary>
        /// Applicatitve functor apply operator
        /// </summary>
        /// <param name="ma">Stack</param>
        /// <returns>Stack</returns>
        public Stck<B> Apply(K<Stck, A> ma) =>
            +Applicative.apply(mf, ma);

        /// <summary>
        /// Applicatitve functor apply operator
        /// </summary>
        /// <param name="ma">Stack</param>
        /// <returns>Stack</returns>
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
            
        /// <summary>
        /// Monoidal binary associative operator
        /// </summary>
        /// <param name="rhs">Right hand operand</param>
        /// <returns>Stack</returns>
        public Stck<A> Combine(K<Stck, A> rhs) => 
            ma.As() + rhs;

        /// <summary>
        /// Functor map operator
        /// </summary>
        /// <param name="f">Mapping function</param>
        /// <returns>Stack</returns>
        public Stck<B> Map<B>(Func<A, B> f) =>
            +Functor.map(f, ma);

        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="f">Bind function</param>
        /// <returns>Stack</returns>
        public Stck<B> Bind<B>(Func<A, K<Stck, B>> f) =>
            +Monad.bind(ma, f);

        /// <summary>
        /// Monad bind operator
        /// </summary>
        /// <param name="f">Bind function</param>
        /// <returns>Stack</returns>
        public Stck<B> Bind<B>(Func<A, Stck<B>> f) =>
            +Monad.bind(ma, f);

        /// <summary>
        /// Monad bind and project
        /// </summary>
        /// <param name="bind">Bind function</param>
        /// <param name="project">Projection function</param>
        /// <returns>Stack</returns>
        public Stck<C> SelectMany<B, C>(Func<A, K<Stck, B>> bind, Func<A, B, C> project) =>
            ma.Bind(a => bind(a).Map(b => project(a, b)));

        /// <summary>
        /// Filter the stack
        /// </summary>
        /// <param name="f">Predicate</param>
        /// <returns>Stack</returns>
        public Stck<A> Where(Func<A, bool> f) =>
            ma.Filter(f);

        /// <summary>
        /// Filter the stack
        /// </summary>
        /// <param name="f">Predicate</param>
        /// <returns>Stack</returns>
        public Stck<A> Filter(Func<A, bool> f)
        {
            var state = ma.StepSetup<Stck, Stck.FoldState, A>();
            var stack = new Stck<A>.Top(default!, Stck<A>.Empty);
            var top   = stack;
            while (ma.Step(ref state, out var x))
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
        
        /// <summary>
        /// Filter and map the stack
        /// </summary>
        /// <param name="f">Predicate</param>
        /// <returns>Stack</returns>
        public Stck<B> Choose<B>(Func<A, Option<B>> f)
        {
            var state = ma.StepSetup<Stck, Stck.FoldState, A>();
            var stack = new Stck<B>.Top(default!, Stck<B>.Empty);
            var top   = stack;
            while (ma.Step(ref state, out var x))
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
        
        /// <summary>
        /// Take `n` items from the stack
        /// </summary>
        /// <param name="amount">Number of items to take</param>
        /// <returns>Taken items</returns>
        public Stck<A> Take(int amount)
        {
            var state = ma.StepSetup<Stck, Stck.FoldState, A>();
            var stack = new Stck<A>.Top(default!, Stck<A>.Empty);
            var top   = stack;
            while (ma.Step(ref state, out var x))
            {
                if(amount == 0) return top.Rest;
                var nstack = new Stck<A>.Top(x, Stck<A>.Empty);
                stack.Rest = nstack;
                stack = nstack;
                amount--;
            }
            return top.Rest;
        }
        
        /// <summary>
        /// Skip `n` on the stack
        /// </summary>
        /// <param name="amount">Number of items to skip</param>
        /// <returns>Stack</returns>
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

        /// <summary>
        /// Reverse the stack
        /// </summary>
        /// <returns></returns>
        public Stck<A> Reverse() =>
            ma.As().Reverse();
    }
}
