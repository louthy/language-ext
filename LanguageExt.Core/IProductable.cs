using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IProductable
    {
    }

    /// <summary>
    /// Provides the Multiply method
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public interface IProductable<T> : IProductable
    {
        /// <summary>
        /// Find the product of this and rhs
        /// </summary>
        /// <param name="rhs">Right hand side of the product</param>
        /// <returns>this * rhs</returns>
        T Multiply(T rhs);
    }
}
