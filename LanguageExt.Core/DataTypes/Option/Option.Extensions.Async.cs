using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

public static partial class OptionAsyncExtensions
{
    /// <summary>
    /// Converts this Try to a TryAsync
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="self">This</param>
    /// <returns>Asynchronous Try</returns>
    [Pure]
    public static OptionAsync<A> ToAsync<A>(this Option<A> self) =>
        new OptionAsync<A>(self.IsSome ? OptionDataAsync.Some(self.Value) : OptionDataAsync<A>.None);
}
