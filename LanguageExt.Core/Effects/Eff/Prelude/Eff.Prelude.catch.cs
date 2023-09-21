using System;
using LanguageExt.Common;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        internal static EffCatch<RT, A> matchError<RT, A>(Func<Error, bool> predicate, Func<Error, Eff<RT, A>> Fail) 
            where RT : struct =>
            new (predicate, Fail);
        
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        internal static EffCatch<A> matchError<A>(Func<Error, bool> predicate, Func<Error, Eff<A>> Fail) =>
            new (predicate, Fail);

       
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(Error error, Func<Error, Eff<RT, A>> Fail)
            where RT : struct =>
            matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static EffCatch<A> @catch<A>(Error error, Func<Error, Eff<A>> Fail) =>
            matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(Error error, Eff<RT, A> Fail) 
            where RT : struct =>
            matchError(e => e.Is(error), _ => Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static EffCatch<A> @catch<A>(Error error, Eff<A> Fail) =>
            matchError(e => e.Is(error), _ => Fail);
        
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(int errorCode, Func<Error, Eff<RT, A>> Fail) 
            where RT : struct =>
            matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static EffCatch<A> @catch<A>(int errorCode, Func<Error, Eff<A>> Fail) =>
            matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(int errorCode, Eff<RT, A> Fail)
            where RT : struct =>
            matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static EffCatch<A> @catch<A>(int errorCode, Eff<A> Fail) =>
            matchError(e => e.Code == errorCode, _ => Fail);
        
                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(string errorText, Func<Error, Eff<RT, A>> Fail) 
            where RT : struct =>
            matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static EffCatch<A> @catch<A>(string errorText, Func<Error, Eff<A>> Fail) =>
            matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(string errorText, Eff<RT, A> Fail)
            where RT : struct =>
            matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static EffCatch<A> @catch<A>(string errorText, Eff<A> Fail) =>
            matchError(e => e.Message == errorText, _ => Fail);
        
                
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<A> @catch<A>(Func<Exception, bool> predicate, Func<Exception, Eff<A>> Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<A> @catch<A>(Func<Exception, bool> predicate, Eff<A> Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), _ => Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(Func<Exception, bool> predicate, Func<Exception, Eff<RT, A>> Fail) 
            where RT : struct =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(Func<Exception, bool> predicate, Eff<RT, A> Fail) 
            where RT : struct =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), _ => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(Eff<RT, A> Fail)
            where RT : struct =>
            matchError(static _ => true, _ => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static EffCatch<A> @catch<A>(Eff<A> Fail) =>
            matchError(static _ => true, _ => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static EffCatch<RT, A> @catch<RT, A>(Func<Error, Eff<RT, A>> Fail) 
            where RT : struct =>
            matchError(static _ => true, Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static EffCatch<A> @catch<A>(Func<Error, Eff<A>> Fail) =>
            matchError(static _ => true, Fail);    
        
        /// <summary>
        /// Catch errors
        /// </summary>
        public static EffCatch<A> @catchOf<E, A>(Func<E, Eff<A>> Fail) where E : Error =>
            matchError(static e => e is E, e => Fail((E)e));

        /// <summary>
        /// Catch errors
        /// </summary>
        public static EffCatch<RT, A> @catchOf<RT, E, A>(Func<E, Eff<RT, A>> Fail)
            where RT : struct
            where E : Error =>
            matchError(static e => e is E, e => Fail((E)e));

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static EffCatch<A> @expected<A>(Func<Expected, Eff<A>> Fail) =>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static EffCatch<RT, A> @expected<RT, A>(Func<Expected, Eff<RT, A>> Fail) 
            where RT : struct =>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static EffCatch<A> @expectedOf<E, A>(Func<E, Eff<A>> Fail) where E : Expected =>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static EffCatch<RT, A> @expectedOf<RT, E, A>(Func<E, Eff<RT, A>> Fail)
            where RT : struct
            where E : Expected =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static EffCatch<A> @exceptional<A>(Func<Exceptional, Eff<A>> Fail) =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static EffCatch<RT, A> @exceptional<RT, A>(Func<Exceptional, Eff<RT, A>> Fail) 
            where RT : struct =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static EffCatch<A> @exceptionalOf<E, A>(Func<E, Eff<A>> Fail) where E : Exceptional =>
            @catchOf(Fail);
            
        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static EffCatch<RT, A> @exceptionalOf<RT, E, A>(Func<E, Eff<RT, A>> Fail)
            where RT : struct
            where E : Exceptional  =>
            @catchOf(Fail);
    }
}
