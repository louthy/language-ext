using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    public struct TString : Ord<string>, Monoid<string>
    {
        /// <summary>
        /// Append
        /// </summary>
        public string Append(string x, string y) =>
            x + y;

        /// <summary>
        /// Empty
        /// </summary>
        /// <returns></returns>
        public string Empty() => 
            "";

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(string x, string y) =>
            x == y;

        /// <summary>
        /// Returns true if x is greater than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than y
        public bool GreaterThan(string x, string y) =>
            x.CompareTo(y) > 0;

        /// <summary>
        /// Returns true if x is greater than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than or equal to y
        public bool GreaterOrEq(string x, string y) =>
            x.CompareTo(y) >= 0;

        /// <summary>
        /// Returns true if x is less than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than y
        public bool LessThan(string x, string y) =>
            x.CompareTo(y) < 0;

        /// <summary>
        /// Returns true if x is less than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than or equal to y
        public bool LessOrEq(string x, string y) =>
            x.CompareTo(y) <= 0;
    }
}
