using System;
using LanguageExt.Common;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        internal static CatchValue<A> matchError<A>(Func<Error, bool> predicate, Func<Error, A> Fail) =>
            new CatchValue<A>(predicate, Fail);

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        internal static CatchValue<A> matchError<A>(Func<Error, bool> predicate, A Fail) =>
            matchError(predicate, _ => Fail);
        
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        internal static CatchError matchError(Func<Error, bool> predicate, Func<Error, Error> Fail) =>
            new CatchError(predicate, Fail);

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        internal static CatchError matchError(Func<Error, bool> predicate, Error Fail) =>
            matchError(predicate, _ => Fail);

       
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static CatchValue<A> @catch<A>(Error error, Func<Error, A> Fail) =>
            matchError(e => e == error, Fail);
       
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static CatchError @catch(Error error, Func<Error, Error> Fail) =>
            matchError(e => e == error, Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static CatchValue<A> @catch<A>(Error error, A Fail) =>
            matchError(e => e == error, _ => Fail);        
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static CatchError @catch(Error error, Error Fail) =>
            matchError(e => e == error, _ => Fail);        

        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static CatchValue<A> @catch<A>(int errorCode, Func<Error, A> Fail) =>
            matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static CatchError @catch(int errorCode, Func<Error, Error> Fail) =>
            matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static CatchValue<A> @catch<A>(int errorCode, A Fail) =>
            matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static CatchError @catch(int errorCode, Error Fail) =>
            matchError(e => e.Code == errorCode, _ => Fail);

                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static CatchValue<A> @catch<A>(string errorText, Func<Error, A> Fail) =>
            matchError(e => e.Message == errorText, Fail);
                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static CatchError @catch(string errorText, Func<Error, Error> Fail) =>
            matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static CatchValue<A> @catch<A>(string errorText, A Fail) =>
            matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static CatchError @catch(string errorText, Error Fail) =>
            matchError(e => e.Message == errorText, _ => Fail);

                
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static CatchValue<A> @catch<A>(Func<Exception, bool> predicate, Func<Exception, A> Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e.ToException()));
                
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static CatchError @catch(Func<Exception, bool> predicate, Func<Exception, Error> Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e.ToException()));
                
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static CatchValue<A> @catch<A>(Func<Exception, bool> predicate, A Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);
                
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static CatchError @catch(Func<Exception, bool> predicate, Error Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);
                
        /// <summary>
        /// Catch all errors and return Fail 
        /// </summary>
        public static CatchError @catch(Error Fail) =>
            matchError(static _ => true, e => Fail);
                
        /// <summary>
        /// Catch all errors and return Fail 
        /// </summary>
        public static CatchValue<A> @catch<A>(A Fail) =>
            matchError(static _ => true, e => Fail);
    }
}
