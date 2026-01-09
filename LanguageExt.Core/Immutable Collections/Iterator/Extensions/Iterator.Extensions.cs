using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IteratorExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    [Pure]
    public static Iterator<A> As<A>(this K<Iterator, A> ma) =>
        (Iterator<A>)ma;

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
        public Iterator<A> Cons(Iterator<A> tail) =>
            Iterator.ConsStrict(head, +tail);

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public Iterator<A> Cons(Func<Iterator<A>> tail) =>
            Iterator.Cons(head, tail);
    }

    /*
    /// <summary>
    /// Get an iterator for any `IEnumerable` 
    /// </summary>
    [Pure]
    public static Iterator<A> GetIterator<A>(this IEnumerable<A> enumerable) =>
        Iterator.from(enumerable);
        */


    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Iterator<A> Flatten<A>(this Iterator<Iterator<A>> ma) =>
        ma is (Exist<Iterator<A>> (var hs), var t)
            ? hs.Combine(t.Flatten())
            : Iterator.Nil<A>();
    
    extension<A, B>(Func<Iterator<A>> iter)
    {
        public Func<Iterator<B>> Map(Func<A, B> f) =>
            () => iter().Map(f);
    }
}

