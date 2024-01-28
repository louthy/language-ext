using System;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Catch an error if the predicate matches
    /// </summary>
    internal static IOCatch<E, A> matchErrorIO<E, A>(Func<E, bool> predicate, Func<E, IO<E, A>> Fail) => 
        new (predicate, Fail);
   
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static IOCatch<E, A> @catch<E, A>(E error, Func<E, IO<E, A>> Fail) =>
        matchErrorIO(e => EqDefault<E>.Equals(e, error), Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static IOCatch<E, A> @catch<E, A>(E error, IO<E, A> Fail) => 
        matchErrorIO<E, A>(e => EqDefault<E>.Equals(e, error), _ => Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static IOCatch<E, A> @catch<E, A>(IO<E, A> Fail) =>
        matchErrorIO<E, A>(static _ => true, _ => Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static IOCatch<E, A> @catch<E, A>(Func<E, IO<E, A>> Fail) => 
        matchErrorIO(static _ => true, Fail);

    /// <summary>
    /// Catch errors
    /// </summary>
    public static IOCatch<E, A> @catchOf<E, A, MATCH_ERROR>(Func<MATCH_ERROR, IO<E, A>> Fail)
        where MATCH_ERROR : E =>
        matchErrorIO<E, A>(
            static e => e is MATCH_ERROR,
            e => Fail((MATCH_ERROR?)e ?? throw new InvalidCastException("casting the error can't result in null")));
}
