#nullable enable
using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        internal static AffCatch<RT, A> matchError<RT, A>(Func<Error, bool> predicate, Func<Error, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            new (predicate, Fail);
        
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        internal static AffCatch<A> matchError<A>(Func<Error, bool> predicate, Func<Error, Aff<A>> Fail) =>
            new (predicate, Fail);

        
               
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(Error error, Func<Error, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static AffCatch<A> @catch<A>(Error error, Func<Error, Aff<A>> Fail) =>
            matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(Error error, Aff<RT, A> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            matchError(e => e.Is(error), _ => Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static AffCatch<A> @catch<A>(Error error, Aff<A> Fail) =>
            matchError(e => e.Is(error), _ => Fail);

        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(int errorCode, Func<Error, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static AffCatch<A> @catch<A>(int errorCode, Func<Error, Aff<A>> Fail) =>
            matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(string errorText, Aff<RT, A> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static AffCatch<A> @catch<A>(string errorText, Aff<A> Fail) =>
            matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(int errorCode, Aff<RT, A> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static AffCatch<A> @catch<A>(int errorCode, Aff<A> Fail) =>
            matchError(e => e.Code == errorCode, _ => Fail);

                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(string errorText, Func<Error, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static AffCatch<A> @catch<A>(string errorText, Func<Error, Aff<A>> Fail) =>
            matchError(e => e.Message == errorText, Fail);

        
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static AffCatch<A> @catch<A>(Func<Exception, bool> predicate, Func<Exception, Aff<A>> Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static AffCatch<A> @catch<A>(Func<Exception, bool> predicate, Aff<A> Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), _ => Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(Func<Exception, bool> predicate, Func<Exception, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(Func<Exception, bool> predicate, Aff<RT, A> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), _ => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(Aff<RT, A> Fail) where RT : struct, HasIO<RT, Error> =>
            matchError(static _ => true, _ => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static AffCatch<A> @catch<A>(Aff<A> Fail) =>
            matchError(static _ => true, _ => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static AffCatch<RT, A> @catch<RT, A>(Func<Error, Aff<RT, A>> Fail) where RT : struct, HasIO<RT, Error> =>
            matchError(static _ => true, Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static AffCatch<A> @catch<A>(Func<Error, Aff<A>> Fail) =>
            matchError(static _ => true, Fail);

        /// <summary>
        /// Catch errors
        /// </summary>
        public static AffCatch<A> @catchOf<E, A>(Func<E, Aff<A>> Fail) where E : Error =>
            matchError(static e => e is E, e => Fail((E)e));

        /// <summary>
        /// Catch errors
        /// </summary>
        public static AffCatch<RT, A> @catchOf<RT, E, A>(Func<E, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error>
            where E : Error =>
            matchError(static e => e is E, e => Fail((E)e));

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static AffCatch<A> @expected<A>(Func<Expected, Aff<A>> Fail) =>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static AffCatch<RT, A> @expected<RT, A>(Func<Expected, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static AffCatch<A> @expectedOf<E, A>(Func<E, Aff<A>> Fail) where E : Expected =>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static AffCatch<RT, A> @expectedOf<RT, E, A>(Func<E, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error>
            where E : Expected =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static AffCatch<A> @exceptional<A>(Func<Exceptional, Aff<A>> Fail) =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static AffCatch<RT, A> @exceptional<RT, A>(Func<Exceptional, Aff<RT, A>> Fail) 
            where RT : struct, HasIO<RT, Error> =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static AffCatch<A> @exceptionalOf<E, A>(Func<E, Aff<A>> Fail) where E : Exceptional  =>
            @catchOf(Fail);
            
        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static AffCatch<RT, A> @exceptionalOf<RT, E, A>(Func<E, Aff<RT, A>> Fail)
            where RT : struct, HasIO<RT, Error>
            where E : Exceptional  =>
            @catchOf(Fail);
    }
}
