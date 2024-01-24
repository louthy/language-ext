/*
#nullable enable
using System;
using LanguageExt.ClassInstances;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Catch an error if the predicate matches
    /// </summary>
    internal static IOCatch<RT, E, A> matchError<RT, E, A>(Func<E, bool> predicate, Func<E, IO<RT, E, A>> Fail) 
        where RT : HasIO<RT, E> =>
        new (predicate, Fail);
   
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static IOCatch<RT, E, A> @catch<RT, E, A>(E error, Func<E, IO<RT, E, A>> Fail)
        where RT : HasIO<RT, E> =>
        matchError(e => default(EqDefault<E>).Equals(e, error), Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static IOCatch<RT, E, A> @catch<RT, E, A>(E error, IO<RT, E, A> Fail) 
        where RT : HasIO<RT, E> =>
        matchError<RT, E, A>(e => default(EqDefault<E>).Equals(e, error), _ => Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static IOCatch<RT, E, A> @catch<RT, E, A>(IO<RT, E, A> Fail)
        where RT : HasIO<RT, E> =>
        matchError<RT, E, A>(static _ => true, _ => Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static IOCatch<RT, E, A> @catch<RT, E, A>(Func<E, IO<RT, E, A>> Fail) 
        where RT : HasIO<RT, E> =>
        matchError<RT, E, A>(static _ => true, Fail);

    /// <summary>
    /// Catch errors
    /// </summary>
    public static IOCatch<RT, E, A> @catchOf<RT, E, A, MATCH_ERROR>(Func<MATCH_ERROR, IO<RT, E, A>> Fail)
        where RT : HasIO<RT, E>
        where MATCH_ERROR : E =>
        matchError<RT, E, A>(
            static e => e is MATCH_ERROR,
            e => Fail((MATCH_ERROR?)e ?? throw new InvalidCastException("casting the error can't result in null")));
}
*/
