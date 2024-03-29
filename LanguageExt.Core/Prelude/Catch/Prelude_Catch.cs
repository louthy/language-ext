using System;
using LanguageExt.Common;

namespace LanguageExt
{
    /// <summary>
    /// The `@catch` functions produce a `CatchError` or `CatchValue` type. These can be composed together with the
    /// `Aff` and `Eff` monads (and maybe more in the future), to create a functional-programming equivalent to
    /// exception catching and matching.
    /// </summary>
    /// <example>
    /// 
    ///      public class TimeoutExample<RT>
    ///          where RT : struct,
    ///          HasTime<RT>,
    ///          HasCancel<RT>,
    ///          HasConsole<RT>
    ///      {
    ///          public static Aff<RT, Unit> main =>
    ///              from _1 in timeout(60 * seconds, longRunning)
    ///                       | @catch(Errors.TimedOut, unit)
    ///              from _2 in Console<RT>.writeLine("done")
    ///              select unit;
    ///      
    ///          static Aff<RT, Unit> longRunning =>
    ///              (from tm in Time<RT>.now
    ///               from _1 in Console<RT>.writeLine(tm.ToLongTimeString())
    ///               select unit)
    ///             .ToAff()
    ///             .Repeat(Schedule.Fibonacci(1 * second));
    ///      }
    /// </example>
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
        
        /// <summary>
        /// Catch errors
        /// </summary>
        public static CatchValue<A> @catchOf<E, A>(Func<E, A> Fail) where E : Error =>
            matchError(static e => e is E, e => Fail((E)e));
        
        /// <summary>
        /// Catch errors
        /// </summary>
        public static CatchError @catchOf<E>(Func<E, Error> Fail) where E : Error =>
            matchError(static e => e is E, e => Fail((E)e));

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static CatchValue<A> @expected<A>(Func<Expected, A> Fail) =>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static CatchError @expected(Func<Expected, Error> Fail)=>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static CatchValue<A> @expectedOf<E, A>(Func<E, A> Fail) where E : Expected =>
            @catchOf(Fail);

        /// <summary>
        /// Catch expected errors
        /// </summary>
        public static CatchError @expectedOf<E>(Func<E, Error> Fail)
            where E : Expected =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static CatchValue<A> @exceptional<A>(Func<Exceptional, A> Fail) =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static CatchError @exceptional(Func<Exceptional, Error> Fail) =>
            @catchOf(Fail);

        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static CatchValue<A> @exceptionalOf<E, A>(Func<E, A> Fail) where E : Exceptional =>
            @catchOf(Fail);
            
        /// <summary>
        /// Catch exceptional errors
        /// </summary>
        public static CatchError @exceptionalOf<E>(Func<E, Error> Fail)
            where E : Exceptional  =>
            @catchOf(Fail);        
    }
}
