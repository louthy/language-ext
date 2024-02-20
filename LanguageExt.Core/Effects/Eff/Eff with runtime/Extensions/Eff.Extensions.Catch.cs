using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Error Error, Func<Error, Eff<RT, A>> Fail)
        where RT : HasIO<RT> =>
        ma | @catch(Error, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Error Error, Eff<RT, A> Fail) 
        where RT : HasIO<RT> =>
        ma | @catch(Error, Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Eff<RT, A> Fail)
        where RT : HasIO<RT> =>
        ma | @catch(Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Func<Error, Eff<RT, A>> Fail) 
        where RT : HasIO<RT> =>
        ma | @catch(Fail);

    /// <summary>
    /// Catch errors
    /// </summary>
    public static Eff<RT, A> CatchOf<RT, A, MATCH_ERROR>(
        this Eff<RT, A> ma, 
        Func<MATCH_ERROR, Eff<RT, A>> Fail)
        where RT : HasIO<RT>
        where MATCH_ERROR : Error =>
        ma | @catchOf(Fail);
}
