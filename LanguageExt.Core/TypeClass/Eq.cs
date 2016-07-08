using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.TypeClass.Prelude;

namespace LanguageExt.TypeClass
{
    /// <summary>
    /// Equality type-class
    /// </summary>
    /// <typeparam name="A">
    /// The type for which equality is defined
    /// </typeparam>
    public interface Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        bool Equals(A x, A y);
    }

    /// <summary>
    /// Integer equality
    /// </summary>
    public struct EqInt : Eq<int>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(int a, int b) { return a == b; }
    }

    /// <summary>
    /// Integer equality
    /// </summary>
    public struct EqShort : Eq<short>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(short a, short b) { return a == b; }
    }

    /// <summary>
    /// Integer equality
    /// </summary>
    public struct EqLong : Eq<long>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(long a, long b) { return a == b; }
    }

    /// <summary>
    /// Floating point equality
    /// </summary>
    public struct EqFloat : Eq<float>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(float a, float b) { return a == b; }
    }

    /// <summary>
    /// Floating point equality
    /// </summary>
    public struct EqDouble : Eq<double>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(double a, double b) { return a == b; }
    }

    /// <summary>
    /// Floating point equality
    /// </summary>
    public struct EqDecimal : Eq<decimal>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(decimal a, decimal b) { return a == b; }
    }

    /// <summary>
    /// String point equality
    /// </summary>
    public struct EqString : Eq<string>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(string a, string b) { return a == b; }
    }

    /// <summary>
    /// Array equality
    /// </summary>
    public struct EqArray<EQ, A> : Eq<A[]> where EQ : struct, Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(A[] x, A[] y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Length != y.Length) return false;
            for (int i = 0; i < x.Length; i++)
            {
                if (!eq<EQ, A>(x[i], y[i])) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Lst<T> equality
    /// </summary>
    public struct EqLst<A> : Eq<Lst<A>>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Lst<A> x, Lst<A> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Count != y.Count) return false;
            return x == y;
        }
    }

    /// <summary>
    /// Set<T> equality
    /// </summary>
    public struct EqSet<A> : Eq<Set<A>>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Set<A> x, Set<A> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Count != y.Count) return false;
            return x == y;
        }
    }

    /// <summary>
    /// Map<K,V> equality
    /// </summary>
    public struct EqMap<K, V> : Eq<Map<K, V>>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Map<K, V> x, Map<K, V> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Count != y.Count) return false;
            return x == y;
        }
    }

    /// <summary>
    /// Que<A> equality
    /// </summary>
    public struct EqQueue<A> : Eq<Que<A>>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Que<A> x, Que<A> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Count != y.Count) return false;
            return x == y;
        }
    }

    /// <summary>
    /// Option<A> equality
    /// </summary>
    public struct EqOption<EQ, A> : Eq<Option<A>> where EQ : struct, Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Option<A> x, Option<A> y) =>
            (from a in x
             from b in y
             select eq<EQ, A>(a, b)).IfNone(false);
    }

    /// <summary>
    /// Option<A> equality
    /// </summary>
    public struct EqOptionUnsafe<EQ, A> : Eq<OptionUnsafe<A>> where EQ : struct, Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            (from a in x
             from b in y
             select eq<EQ, A>(a, b)).IfNoneUnsafe(false);
    }
}
