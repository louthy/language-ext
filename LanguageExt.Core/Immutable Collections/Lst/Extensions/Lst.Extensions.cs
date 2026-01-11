using System;
using System.Linq;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class LstExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Lst<A> Flatten<A>(this Lst<Lst<A>> ma) =>
        ma.Bind(identity);

    extension<A>(A head)
    {
        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="head">Head item in the sequence</param>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public Lst<A> Cons(Lst<A> tail) =>
            tail.Insert(0, head);
    }

    extension<A>(K<Lst, A> list)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public Lst<A> As() =>
            (Lst<A>)list;

        /// <summary>
        /// Provide a sorted Lst
        /// </summary>
        [Pure]
        public Lst<A> Sort<OrdA>() where OrdA : Ord<A> =>
            list.As().OrderBy(identity, OrdComparer<OrdA, A>.Default).AsIterable().ToLst();
        
        /// <summary>
        /// Reverse the list
        /// </summary>
        [Pure]
        public Lst<A> Reverse() =>
            list.As().Reverse();
    
        /// <summary>
        /// LINQ Select implementation for Lst
        /// </summary>
        [Pure]
        public Lst<B> Select<B>(Func<A, B> f)
        {
            var           root     = ListItem<B>.EmptyM;
            var           subIndex = 0;
        
            var fsa = list.StepSetup<Lst, Lst.FoldState, A>();
            while (list.Step(ref fsa, out var a))
            {
                var b = f(a);
                root = ListModuleM.Insert(root, new ListItem<B>(1, 1, ListItem<B>.Empty, b, ListItem<B>.Empty), subIndex);
                subIndex++;
            }
            return new Lst<B>(root);
        }

        /// <summary>
        /// Monadic bind function
        /// </summary>
        [Pure]
        public Lst<B> Bind<B>(Func<A, Lst<B>> f)
        {
            var           root     = ListItem<B>.EmptyM;
            var           subIndex = 0;
        
            var fsa = list.StepSetup<Lst, Lst.FoldState, A>();
            while (list.Step(ref fsa, out var a))
            {
                var mb  = +f(a);
                var fsb = mb.StepSetup<Lst, Lst.FoldState, B>();
                while (mb.Step(ref fsb, out var b))
                {
                    root = ListModuleM.Insert(root, new ListItem<B>(1, 1, ListItem<B>.Empty, b, ListItem<B>.Empty), subIndex);
                    subIndex++;
                }
            }
            return new Lst<B>(root);
        }

        /// <summary>
        /// Monadic bind function
        /// </summary>
        [Pure]
        public Lst<B> Bind<B>(Func<A, K<Lst, B>> f)
        {
            var           root     = ListItem<B>.EmptyM;
            var           subIndex = 0;
        
            var fsa = list.StepSetup<Lst, Lst.FoldState, A>();
            while (list.Step(ref fsa, out var a))
            {
                var mb  = +f(a);
                var fsb = mb.StepSetup<Lst, Lst.FoldState, B>();
                while (mb.Step(ref fsb, out var b))
                {
                    root = ListModuleM.Insert(root, new ListItem<B>(1, 1, ListItem<B>.Empty, b, ListItem<B>.Empty), subIndex);
                    subIndex++;
                }
            }
            return new Lst<B>(root);
        }

        /// <summary>
        /// Returns the number of items in the Lst T
        /// </summary>
        /// <returns>The number of items in the list</returns>
        [Pure]
        public int Count =>
            list.As().Count;

        /// <summary>
        /// LINQ bind implementation for Lst
        /// </summary>
        [Pure]
        public Lst<C> SelectMany<B, C>(Func<A, Lst<B>> bind, Func<A, B, C> project)
        {
            var           root     = ListItem<C>.EmptyM;
            var           subIndex = 0;
        
            var fsa = list.StepSetup<Lst, Lst.FoldState, A>();
            while (list.Step(ref fsa, out var a))
            {
                var mb = +bind(a);
                var fsb = mb.StepSetup<Lst, Lst.FoldState, B>();
                while (mb.Step(ref fsb, out var b))
                {
                    root = ListModuleM.Insert(root, new ListItem<C>(1, 1, ListItem<C>.Empty, project(a, b), ListItem<C>.Empty), subIndex);
                    subIndex++;
                }
            }
            return new Lst<C>(root);
        }

        /// <summary>
        /// Convert to a queryable 
        /// </summary>
        [Pure]
        public IQueryable<A> AsQueryable() =>
            // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
            list.As().Value.AsQueryable();
        
        [Obsolete("Use Lst.Reverse instead")]
        public Lst<A> Rev() =>
            list.Reverse();
    }
}
