using System;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static class OptionAsyncExtensions
{
    /// <summary>
    /// Converts this Option to an OptionAsync
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="self">This</param>
    /// <returns>Asynchronous Try</returns>
    [Pure]
    [Obsolete(Change.UseEffMonadInstead)]
    public static OptionAsync<A> ToAsync<A>(this Option<A> self) =>
        self.IsSome ? OptionAsync<A>.Some(self.Value!) : default;
}
