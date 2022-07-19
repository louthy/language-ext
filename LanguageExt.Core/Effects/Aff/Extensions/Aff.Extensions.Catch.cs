using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Error error, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, Error error, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Error error, Func<Error, Aff<A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, Error error, Func<Error, Aff<A>> Fail) =>
            ma | matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Error error, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), _ => Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, Error error, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), _ => Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Error error, Aff<A> Fail) where RT : struct, HasCancel<RT>  =>
            ma | matchError(e => e.Is(error), _ => Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, Error error, Aff<A> Fail) =>
            ma | matchError(e => e.Is(error), _ => Fail);

        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, int errorCode, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, int errorCode, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, int errorCode, Func<Error, Aff<A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, int errorCode, Func<Error, Aff<A>> Fail) =>
            ma | matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, string errorText, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, string errorText, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, string errorText, Aff<A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, string errorText, Aff<A> Fail) =>
            ma | matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, int errorCode, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, int errorCode, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, int errorCode, Aff<A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, int errorCode, Aff<A> Fail) =>
            ma | matchError(e => e.Code == errorCode, _ => Fail);

                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, string errorText, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, Fail);
                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, string errorText, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, string errorText, Func<Error, Aff<A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, string errorText, Func<Error, Aff<A>> Fail) =>
            ma | matchError(e => e.Message == errorText, Fail);

        
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Func<Exception, bool> predicate, Func<Exception, Aff<A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));
        
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, Func<Exception, bool> predicate, Func<Exception, Aff<A>> Fail) =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Func<Exception, bool> predicate, Aff<A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, Func<Exception, bool> predicate, Aff<A> Fail) =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Func<Exception, bool> predicate, Func<Exception, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, Func<Exception, bool> predicate, Func<Exception, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Func<Exception, bool> predicate, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, Func<Exception, bool> predicate, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Aff<A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, Aff<A> Fail) =>
            ma | matchError(static _ => true, e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<A> ma, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Aff<RT, A> Catch<RT, A>(this Aff<RT, A> ma, Func<Error, Aff<A>> Fail) where RT : struct, HasCancel<RT>  =>
            ma | matchError(static _ => true, Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Aff<A> Catch<A>(this Aff<A> ma, Func<Error, Aff<A>> Fail) =>
            ma | matchError(static _ => true, Fail);
    }
}
