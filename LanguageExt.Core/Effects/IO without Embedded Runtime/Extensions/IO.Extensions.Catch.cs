using System;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IOExtensions
{
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static IO<E, A> Catch<E, A>(this IO<E, A> ma, E Error, Func<E, IO<E, A>> Fail) =>
        ma | @catch(Error, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static IO<E, A> Catch<E, A>(this IO<E, A> ma, E Error, IO<E, A> Fail) => 
        ma | @catch(Error, Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static IO<E, A> Catch<E, A>(this IO<E, A> ma, IO<E, A> Fail) =>
        ma | @catch(Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static IO<E, A> Catch<E, A>(this IO<E, A> ma, Func<E, IO<E, A>> Fail) => 
        ma | @catch(Fail);

    /// <summary>
    /// Catch errors
    /// </summary>
    public static IO<E, A> CatchOf<E, A, MATCH_ERROR>(
        this IO<E, A> ma, 
        Func<MATCH_ERROR, IO<E, A>> Fail)
        where MATCH_ERROR : E =>
        ma | @catchOf(Fail);
}
