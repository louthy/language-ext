using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Finds an appropriate Hashable from the loaded assemblies, if one can't be found then it
    /// falls back to the standard .NET Object.GetHashCode() method to provide a hash-code.
    /// </summary>
    public struct HashableDefault<A> : Hashable<A>
    {
        static readonly Func<A, int> hash;

        static HashableDefault()
        {
            if (Reflect.IsFunc(typeof(A)))
            {
                hash = x => x.IsNull() ? 0 : x.GetHashCode();
            }
            else if (Reflect.IsAnonymous(typeof(A)))
            {
                hash = IL.GetHashCode<A>(false);
            }
            else
            {
                var def = Class<Hashable<A>>.Default;
                if (def == null)
                {
                    hash = x => x.IsNull() ? 0 : x.GetHashCode();
                }
                else
                {
                    hash = def.GetHashCode;
                }
            }
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            hash(x);
    }
}
