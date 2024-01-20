#nullable enable
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
    internal static EffCatch<A> matchError<A>(Func<Error, bool> predicate, Func<Error, Eff<A>> Fail) => 
        new (predicate, Fail);
   
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static EffCatch<A> @catch<A>(Error error, Func<Error, Eff<A>> Fail) =>
        matchError(e => default(EqDefault<Error>).Equals(e, error), Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static EffCatch<A> @catch<A>(Error error, Eff<A> Fail) => 
        matchError<A>(e => default(EqDefault<Error>).Equals(e, error), _ => Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static EffCatch<A> @catch<A>(Eff<A> Fail) =>
        matchError<A>(static _ => true, _ => Fail);

    /// <summary>
    /// Catch all errors
    /// </summary>
    public static EffCatch<A> @catch<A>(Func<Error, Eff<A>> Fail) => 
        matchError(static _ => true, Fail);

    /// <summary>
    /// Catch errors
    /// </summary>
    public static EffCatch<A> @catchOf<A, MATCH_ERROR>(Func<MATCH_ERROR, Eff<A>> Fail) 
        where MATCH_ERROR : Error =>
        matchError<A>(
            static e => e is MATCH_ERROR,
            e => Fail((MATCH_ERROR?)e ?? throw new InvalidCastException("casting the error can't result in null")));
}
