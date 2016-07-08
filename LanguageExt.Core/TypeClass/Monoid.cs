using System;
using System.Collections.Generic;
using static LanguageExt.TypeClass.Prelude;


namespace LanguageExt.TypeClass
{
    /// <summary>
    /// Monoid type-class
    /// <para>
    /// A monoid is a type with an identity and an associative binary operation.
    /// </para>
    /// </summary>
    /// <typeparam name="A">The type being described as a monoid</typeparam>
    public interface Monoid<A> : Semigroup<A>
    {
        /// <summary>
        /// The identity of Append
        /// <summary>
        A Empty();

        // Concat is defined in Prelude_Monoid
    }

    /// <summary>
    /// Booleans form a monoid under conjunction.
    /// </summary>
    public struct All : Monoid<bool>
    {
        public bool Append(bool x, bool y) => x && y;
        public bool Empty() => true;
    }

    /// <summary>
    /// Booleans form a monoid under disjunctions.
    /// </summary>
    public struct Any : Monoid<bool>
    {
        public bool Append(bool x, bool y) => x || y;
        public bool Empty() => true;
    }

    /// <summary>
    /// Ordered values form a semigroup under minimum.
    /// </summary>
    /// <typeparam name="A">The type of the ordered values.</typeparam>
    public struct Min<ORD, A> : Semigroup<A> where ORD : struct, Ord<A>
    {
        public A Append(A x, A y) =>
            lessOrEq<ORD, A>(x, y) ? x : y;
    }

    /// <summary>
    /// Ordered values form a semigroup under maximum.
    /// </summary>
    /// <typeparam name="A">The type of the ordered values.</typeparam>
    public struct Max<ORD, A> : Semigroup<A> where ORD : struct, Ord<A>
    {
        public A Append(A x, A y) =>
            lessOrEq<ORD, A>(x, y) ? y : x;
    }

    /// <summary>
    /// Numbers form a monoid under addition.
    /// </summary>
    /// <typeparam name="A">The type of the number being added.</typeparam>
    public struct Sum<NUM, A> : Monoid<A> where NUM : struct, Num<A>
    {
        public A Append(A x, A y) =>
            add<NUM, A>(x, y);

        public A Empty() => 
            fromInteger<NUM, A>(0);
    }

    /// <summary>
    ///  Numbers form a monoid under multiplication.
    /// </summary>
    /// <typeparam name="A">The type of the number being multiplied.</typeparam>
    public struct Product<NUM, A> : Semigroup<A> where NUM : struct, Num<A>
    {
        public A Append(A x, A y) =>
            add<NUM, A>(x, y);

        public A Empty() =>
            fromInteger<NUM, A>(0);
    }
}
