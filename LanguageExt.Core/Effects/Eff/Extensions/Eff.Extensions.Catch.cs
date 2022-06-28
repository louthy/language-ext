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
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Error error, Func<Error, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, Error error, Func<Error, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Error error, Func<Error, Eff<A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, Error error, Func<Error, Eff<A>> Fail) =>
            ma | matchError(e => e.Is(error), Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Error error, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), _ => Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, Error error, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Is(error), _ => Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Error error, Eff<A> Fail) where RT : struct, HasCancel<RT>  =>
            ma | matchError(e => e.Is(error), _ => Fail);
        
        /// <summary>
        /// Catch an error if the error matches the argument provided 
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, Error error, Eff<A> Fail) =>
            ma | matchError(e => e.Is(error), _ => Fail);

        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, int errorCode, Func<Error, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, int errorCode, Func<Error, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, int errorCode, Func<Error, Eff<A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, int errorCode, Func<Error, Eff<A>> Fail) =>
            ma | matchError(e => e.Code == errorCode, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, string errorText, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, string errorText, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, string errorText, Eff<A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, string errorText, Eff<A> Fail) =>
            ma | matchError(e => e.Message == errorText, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, int errorCode, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, int errorCode, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, int errorCode, Eff<A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Code == errorCode, _ => Fail);
        
        /// <summary>
        /// Catch an error if the error `Code` matches the `errorCode` argument provided 
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, int errorCode, Eff<A> Fail) =>
            ma | matchError(e => e.Code == errorCode, _ => Fail);

                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, string errorText, Func<Error, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, Fail);
                
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, string errorText, Func<Error, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, string errorText, Func<Error, Eff<A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Message == errorText, Fail);
        
        /// <summary>
        /// Catch an error if the error message matches the `errorText` argument provided 
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, string errorText, Func<Error, Eff<A>> Fail) =>
            ma | matchError(e => e.Message == errorText, Fail);

        
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Func<Exception, bool> predicate, Func<Exception, Eff<A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));
        
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, Func<Exception, bool> predicate, Func<Exception, Eff<A>> Fail) =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Func<Exception, bool> predicate, Eff<A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, Func<Exception, bool> predicate, Eff<A> Fail) =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Func<Exception, bool> predicate, Func<Exception, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, Func<Exception, bool> predicate, Func<Exception, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Func<Exception, bool> predicate, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, Func<Exception, bool> predicate, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, Eff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Eff<A> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, Eff<A> Fail) =>
            ma | matchError(static _ => true, e => Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Func<Error, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<A> ma, Func<Error, Eff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            ma | matchError(static _ => true, Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Eff<RT, A> Catch<RT, A>(this Eff<RT, A> ma, Func<Error, Eff<A>> Fail) where RT : struct, HasCancel<RT>  =>
            ma | matchError(static _ => true, Fail);

        /// <summary>
        /// Catch all errors
        /// </summary>
        public static Eff<A> Catch<A>(this Eff<A> ma, Func<Error, Eff<A>> Fail) =>
            ma | matchError(static _ => true, Fail);
    }
}
