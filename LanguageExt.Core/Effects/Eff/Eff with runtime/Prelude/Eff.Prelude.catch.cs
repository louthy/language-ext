using System;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Catch an error if the predicate matches
    /// </summary>
    internal static EffCatch<RT, A> matchErrorEff<RT, A>(Func<Error, bool> predicate, Func<Error, Eff<RT, A>> Fail) 
        where RT : HasIO<RT> =>
        new (predicate, Fail);
   
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static EffCatch<RT, A> @catch<RT, A>(Error error, Func<Error, Eff<RT, A>> Fail)
        where RT : HasIO<RT> =>
        matchErrorEff(e => EqDefault<Error>.Equals(e, error), Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static EffCatch<RT, A> @catch<RT, A>(Error error, Eff<RT, A> Fail) 
        where RT : HasIO<RT> =>
        matchErrorEff<RT, A>(e => EqDefault<Error>.Equals(e, error), _ => Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static EffCatch<RT, A> @catch<RT, A>(Eff<RT, A> Fail)
        where RT : HasIO<RT> =>
        matchErrorEff<RT, A>(static _ => true, _ => Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static EffCatch<RT, A> @catch<RT, A>(Func<Error, Eff<RT, A>> Fail) 
        where RT : HasIO<RT> =>
        matchErrorEff(static _ => true, Fail);

    /// <summary>
    /// Catch errors
    /// </summary>
    public static EffCatch<RT, A> @catchOf<RT, A, MATCH_ERROR>(Func<MATCH_ERROR, Eff<RT, A>> Fail)
        where RT : HasIO<RT>
        where MATCH_ERROR : Error =>
        matchErrorEff<RT, A>(
            static e => e is MATCH_ERROR,
            e => Fail((MATCH_ERROR?)e ?? throw new InvalidCastException("casting the error can't result in null")));
}
