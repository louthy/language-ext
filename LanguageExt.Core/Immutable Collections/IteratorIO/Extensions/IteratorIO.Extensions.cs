using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IteratorIOExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    [Pure]
    public static IteratorIO<A> As<A>(this K<IteratorIO, A> ma) =>
        (IteratorIO<A>)ma;

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static IteratorIO<A> Flatten<A>(this IteratorIO<IteratorIO<A>> ma) =>
        new IteratorIO<A>.OpFlatten(ma);

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static IteratorIO<A> Flatten<A>(this IteratorIO<K<IteratorIO, A>> ma) =>
        new IteratorIO<A>.OpFlatten2(ma);

    /// <param name="head">Head item in the sequence</param>
    /// <typeparam name="A">Type of the items in the sequence</typeparam>
    extension<A>(A head)
    {
        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public IteratorIO<A> Cons(IteratorIO<A> tail) =>
            IteratorIO.cons(head, +tail);

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public IteratorIO<A> Cons(Func<IteratorIO<A>> tail) =>
            IteratorIO.cons(head, tail);
    }
    
    extension<A, B>(Func<IteratorIO<A>> iter)
    {
        public Func<IteratorIO<B>> Map(Func<A, B> f) =>
            () => iter().Map(f);
    }
}

