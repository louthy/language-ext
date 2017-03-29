using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface Alternative<AltAB, A, B>
    {
        /// <summary>
        /// Identity
        /// </summary>
        AltAB Empty();

        /// <summary>
        /// As associative binary operation
        /// </summary>
        /// <param name="x">Left hand side</param>
        /// <param name="y">Right hand side</param>
        /// <returns>The result of the associative binary operation</returns>
        AltAB Append(AltAB x, AltAB y);
    }
}
