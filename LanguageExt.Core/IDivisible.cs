using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IDivisible
    {
    }

    /// <summary>
    /// Provides the Divide method
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public interface IDivisible<T> : IDivisible
    {
        /// <summary>
        /// Find this / rhs
        /// </summary>
        /// <param name="rhs">Right hand side of the divide</param>
        /// <returns>this / rhs</returns>
        T Divide(T rhs);
    }
}
