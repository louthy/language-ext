using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface ISubtractable
    {
    }

    /// <summary>
    /// Provides the Subtract method
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public interface ISubtractable<T> : ISubtractable
    {
        /// <summary>
        /// Subject rhs from lhs
        /// </summary>
        /// <param name="rhs">Right hand side to subtract</param>
        /// <returns>this with rhs subtracted</returns>
        T Subtract(T rhs);
    }
}
