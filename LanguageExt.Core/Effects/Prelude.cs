#pragma warning disable LX_StreamT

using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Start a new source
    /// </summary>
    /// <returns>A source in an IO computation</returns>
    public static IO<Source<A>> Source<A>() =>
        LanguageExt.Source<A>.Start();

    /// <summary>
    /// Subscribe to the source and await the values
    /// </summary>
    /// <remarks>
    /// Each subscriber runs on the same thread as the event-distributor.  So, if you have multiple
    /// subscribers they will be processed serially for each event.  If you want the subscribers to
    /// run in parallel then you must lift the `IO` monad when calling `Subscribe` and `forkIO` the
    /// stream.  This gives fine-grained control over when to run events in parallel.
    /// </remarks>
    /// <typeparam name="M">Monad type lifted into the stream</typeparam>
    /// <returns>StreamT monad transformer that will get the values coming downstream</returns>

    public static StreamT<M, A> await<M, A>(Source<A> source)
        where M : Monad<M> =>
        source.Await<M>();

    /// <summary>
    /// Post a value to flow downstream
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>IO effect</returns>
    public static IO<Unit> post<A>(Source<A> source, A value) =>
        source.Post(value);

    /// <summary>
    /// Post a value to flow downstream (partially applied)
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>IO effect</returns>
    public static Func<A, IO<Unit>> post<A>(Source<A> source) => 
        source.Post;
}
