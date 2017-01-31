using System;
using System.Linq;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LanguageExt
{
    /// <summary>
    /// The Try monad captures exceptions and uses them to cancel the
    /// computation.  Primarily useful for expression based processing
    /// of errors.
    /// </summary>
    /// <remarks>To invoke directly, call x.Try()</remarks>
    /// <returns>A value that represents the outcome of the computation, either
    /// Success or Failure</returns>
    public delegate TryResult<A> Try<A>();
}
