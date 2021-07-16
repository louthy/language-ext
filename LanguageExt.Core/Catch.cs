using System;
using LanguageExt.Common;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static CatchValue<A> matchError<A>(Func<Error, bool> predicate, Func<Error, A> Fail) =>
            new CatchValue<A>(predicate, Fail);

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static CatchValue<A> matchError<A>(Func<Error, bool> predicate, A Fail) =>
            matchError(predicate, _ => Fail);
        
       
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static CatchValue<A> match<A>(Error error, Func<Error, A> Fail) =>
            matchError(e => e == error, Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static CatchValue<A> match<A>(Error error, A Fail) =>
            matchError(e => e == error, _ => Fail);        

        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static CatchValue<A> match<A>(int errorCode, Func<Error, A> Fail) =>
            matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static CatchValue<A> match<A>(int errorCode, A Fail) =>
            matchError(e => e.Code == errorCode, _ => Fail);

                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static CatchValue<A> match<A>(string errorText, Func<Error, A> Fail) =>
            matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static CatchValue<A> match<A>(string errorText, A Fail) =>
            matchError(e => e.Message == errorText, _ => Fail);

                
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static CatchValue<A> match<A>(Func<Exception, bool> predicate, Func<Exception, A> Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e.ToException()));
                
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static CatchValue<A> match<A>(Func<Exception, bool> predicate, A Fail) =>
            matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);
    }
}
