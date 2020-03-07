using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
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
                hash = GetHashable("Try", typeof(HashableTry<>)) ?? 
                       GetHashable("TryOption", typeof(HashableTryOption<>)) ??
                       GetHashable("TryAsync", typeof(HashableTryAsync<>)) ??
                       GetHashable("TryOptionAsync", typeof(HashableTryOptionAsync<>)) ??
                       new Func<A, int>(x => x.IsNull() ? 0 : x.GetHashCode());
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
        
        static Func<A, int> GetHashable(string name, Type ordType)
        {
            if (typeof(A).FullName.StartsWith($"LanguageExt.{name}`"))
            {
                var arg = typeof(A).GenericTypeArguments[0];
                var genA = ordType.MakeGenericType(arg);
                var mthd = genA.GetMethod("GetHashCode", new Type[] {typeof(A)});
            
                var val = Expression.Parameter(typeof(A), "x");

                var lambda = Expression.Lambda<Func<A, int>>(Expression.Call(Expression.Default(genA), mthd, val), val);
                return lambda.Compile();
            }
            else
            {
                return null;
            }
        }
    }
}
