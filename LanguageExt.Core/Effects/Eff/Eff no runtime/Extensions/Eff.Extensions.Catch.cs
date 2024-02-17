using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static Eff<A> Catch<A>(this Eff<A> ma, Error Error, Func<Error, Eff<A>> Fail) =>
        ma | @catch(Error, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static Eff<A> Catch<A>(this Eff<A> ma, Error Error, Eff<A> Fail) =>
        ma | @catch(Error, Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static Eff<A> Catch<A>(this Eff<A> ma, Eff<A> Fail) =>
        ma | @catch(Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static Eff<A> Catch<A>(this Eff<A> ma, Func<Error, Eff<A>> Fail) => 
        ma | @catch(Fail);

    /// <summary>
    /// Catch errors
    /// </summary>
    public static Eff<A> CatchOf<A, MATCH_ERROR>(
        this Eff<A> ma, 
        Func<MATCH_ERROR, Eff<A>> Fail)
        where MATCH_ERROR : Error =>
        ma | @catchOf(Fail);
}
