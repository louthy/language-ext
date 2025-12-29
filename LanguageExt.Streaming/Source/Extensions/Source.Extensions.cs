using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Channels;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static Source<A> As<A>(this K<Source, A> ma) =>
        (Source<A>)ma;
    
    [Pure]
    public static Source<A> AsSource<A>(this Channel<A> items) =>
        Source.lift(items);

    [Pure]
    public static Source<A> AsSource<A>(this IEnumerable<A> items) =>
        Source.lift(items);
    
    [Pure]
    public static Source<A> AsSource<A>(this IAsyncEnumerable<A> items) =>
        Source.lift(items);
    
    [Pure]
    public static Source<A> AsSource<A>(this IObservable<A> items) =>
        Source.lift(items);
}
