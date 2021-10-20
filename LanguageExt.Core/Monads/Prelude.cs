using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Construct identity monad
        /// </summary>
        public static Identity<A> Id<A>(A value) => 
            new Identity<A>(value);
    }
}
