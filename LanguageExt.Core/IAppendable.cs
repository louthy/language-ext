using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IAppendable
    {
    }

    /// <summary>
    /// Provides the Append method
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public interface IAppendable<T> : IAppendable
    {
        /// <summary>
        /// Append rhs to this
        /// </summary>
        /// <param name="rhs">Right hand side to append</param>
        /// <returns>this with rhs appended</returns>
        T Append(T rhs);
    }
}
