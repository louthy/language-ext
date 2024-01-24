/*
#nullable enable
using System;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IOExtensions
{
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static IO<RT, E, A> Catch<RT, E, A>(this IO<RT, E, A> ma, E Error, Func<E, IO<RT, E, A>> Fail)
        where RT : HasIO<RT, E> =>
        ma | @catch(Error, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static IO<RT, E, A> Catch<RT, E, A>(this IO<RT, E, A> ma, E Error, IO<RT, E, A> Fail) 
        where RT : HasIO<RT, E> =>
        ma | @catch(Error, Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static IO<RT, E, A> Catch<RT, E, A>(this IO<RT, E, A> ma, IO<RT, E, A> Fail)
        where RT : HasIO<RT, E> =>
        ma | @catch(Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static IO<RT, E, A> Catch<RT, E, A>(this IO<RT, E, A> ma, Func<E, IO<RT, E, A>> Fail) 
        where RT : HasIO<RT, E> =>
        ma | @catch(Fail);

    /// <summary>
    /// Catch errors
    /// </summary>
    public static IO<RT, E, A> CatchOf<RT, E, A, MATCH_ERROR>(
        this IO<RT, E, A> ma, 
        Func<MATCH_ERROR, IO<RT, E, A>> Fail)
        where RT : HasIO<RT, E>
        where MATCH_ERROR : E =>
        ma | @catchOf(Fail);
}
*/
