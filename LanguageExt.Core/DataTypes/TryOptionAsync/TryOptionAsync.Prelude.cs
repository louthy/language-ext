using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using LanguageExt.Common;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="f">Function to run asynchronously</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsync<A>(Func<Task<A>> f) =>
            TryOptionAsyncExtensions.Memo(new TryOptionAsync<A>(async () => new OptionalResult<A>(await f())));

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="f">Function to run asynchronously</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsync<A>(Func<Task<Option<A>>> f) =>
            TryOptionAsyncExtensions.Memo(new TryOptionAsync<A>(async () => new OptionalResult<A>(await f())));

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Task to run asynchronously</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsync<A>(Task<A> v) =>
            TryOptionAsyncExtensions.Memo(new TryOptionAsync<A>(async () => new OptionalResult<A>(await v)));

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Task to run asynchronously</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsync<A>(Task<Option<A>> v) =>
            TryOptionAsyncExtensions.Memo(new TryOptionAsync<A>(async () => new OptionalResult<A>(await v)));

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsync<A>(A v) => () =>
            new OptionalResult<A>(v).AsTask();

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsyncSucc<A>(A v) => () =>
            new OptionalResult<A>(v).AsTask();

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsync<A>(Option<A> v) => () =>
            new OptionalResult<A>(v).AsTask();

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionalAsync<A>(Option<A> v) => () =>
            new OptionalResult<A>(v).AsTask();

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsync<A>(Exception ex) => () =>
            new OptionalResult<A>(ex).AsTask();

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryOptionAsync<A> TryOptionAsyncFail<A>(Exception ex) => () =>
            new OptionalResult<A>(ex).AsTask();

        /// <summary>
        /// Append the bound value of TryOptionAsync(x) to TryOptionAsync(y).  If either of the
        /// Trys are Fail then the result is Fail
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs ++ rhs</returns>
        [Pure]
        public static TryOptionAsync<A> append<SEMI, A>(TryOptionAsync<A> lhs, TryOptionAsync<A> rhs) where SEMI : struct, Semigroup<A> =>
            lhs.Append<SEMI, A>(rhs);
    
        /// <summary>
        /// Add the bound value of TryOptionAsync(x) to TryOptionAsync(y).  If either of the
        /// Trys are Fail then the result is Fail
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static TryOptionAsync<A> add<NUM, A>(TryOptionAsync<A> lhs, TryOptionAsync<A> rhs) where NUM : struct, Num<A> =>
            lhs.Add<NUM, A>(rhs);

        /// <summary>
        /// Subtract the TryOptionAsync(x) from TryOptionAsync(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to find the subtract between the Trys (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the R type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static TryOptionAsync<T> subtract<NUM, T>(TryOptionAsync<T> lhs, TryOptionAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Subtract<NUM, T>(rhs);

        /// <summary>
        /// Find the product of TryOptionAsync(x) and TryOptionAsync(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to multiply the Trys (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public static TryOptionAsync<T> product<NUM, T>(TryOptionAsync<T> lhs, TryOptionAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Product<NUM, T>(rhs);

        /// <summary>
        /// Divide Try(x) by Try(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to divide the Trys (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static TryOptionAsync<T> divide<NUM, T>(TryOptionAsync<T> lhs, TryOptionAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Divide<NUM, T>(rhs);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static TryOptionAsync<B> apply<A, B>(TryOptionAsync<Func<A, B>> fab, TryOptionAsync<A> fa) =>
            ApplTryOptionAsync<A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static TryOptionAsync<B> apply<A, B>(Func<A, B> fab, TryOptionAsync<A> fa) =>
            ApplTryOptionAsync<A, B>.Inst.Apply(TryOptionAsync(fab), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static TryOptionAsync<C> apply<A, B, C>(TryOptionAsync<Func<A, B, C>> fabc, TryOptionAsync<A> fa, TryOptionAsync<B> fb) =>
            fabc.Bind(f => ApplTryOptionAsync<A, B, C>.Inst.Apply(MTryOptionAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(f).AsTask()), fa, fb));

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static TryOptionAsync<C> apply<A, B, C>(Func<A, B, C> fabc, TryOptionAsync<A> fa, TryOptionAsync<B> fb) =>
            ApplTryOptionAsync<A, B, C>.Inst.Apply(MTryOptionAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(fabc).AsTask()), fa, fb);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryOptionAsync<Func<B, C>> apply<A, B, C>(TryOptionAsync<Func<A, B, C>> fabc, TryOptionAsync<A> fa) =>
            fabc.Bind(f => ApplTryOptionAsync<A, B, C>.Inst.Apply(MTryOptionAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(f).AsTask()), fa));

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryOptionAsync<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, TryOptionAsync<A> fa) =>
            ApplTryOptionAsync<A, B, C>.Inst.Apply(MTryOptionAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(fabc).AsTask()), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryOptionAsync<Func<B, C>> apply<A, B, C>(TryOptionAsync<Func<A, Func<B, C>>> fabc, TryOptionAsync<A> fa) =>
            ApplTryOptionAsync<A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryOptionAsync<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, TryOptionAsync<A> fa) =>
            ApplTryOptionAsync<A, B, C>.Inst.Apply(TryOptionAsync(fabc), fa);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type Option<B></returns>
        [Pure]
        public static TryOptionAsync<B> action<A, B>(TryOptionAsync<A> fa, TryOptionAsync<B> fb) =>
            ApplTryOptionAsync<A, B>.Inst.Action(fa, fb);

        /// <summary>
        /// Test if the TryOptionAsync computation is successful
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation to test</param>
        /// <returns>True if successful</returns>
        [Pure]
        public static TryOptionAsync<bool> isSome<T>(TryOptionAsync<T> self) =>
            from x in isFail(self)
            select !x;

        /// <summary>
        /// Test if the TryOptionAsync computation fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation to test</param>
        /// <returns>True if fails</returns>
        [Pure]
        public static TryOptionAsync<bool> isFail<T>(TryOptionAsync<T> self) => async () =>
            (await self.Try()).IsFaultedOrNone;

        /// <summary>
        /// Invoke a delegate if the TryOptionAsync returns a value successfully
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <param name="Succ">Delegate to invoke if successful</param>
        public static Task<Unit> ifSome<T>(TryOptionAsync<T> self, Action<T> Succ) =>
            self.IfSome(Succ);

        /// <summary>
        /// Invoke a delegate if the TryOptionAsync fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="value">TryOptionAsync computation</param>
        /// <param name="Fail">Delegate to invoke on failure</param>
        /// <returns>Result of the invocation of Fail on failure, the result of the TryOptionAsync otherwise</returns>
        [Pure]
        public static Task<T> ifFailOrNone<T>(TryOptionAsync<T> self, Func<T> Fail) =>
            self.IfNoneOrFail(Fail);

        /// <summary>
        /// Return a default value if the TryOptionAsync fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <param name="failValue">Default value to use on failure</param>
        /// <returns>failValue on failure, the result of the TryOptionAsync otherwise</returns>
        [Pure]
        public static Task<T> ifNoneOrFail<T>(TryOptionAsync<T> self, T failValue) =>
            self.IfNoneOrFail(failValue);

        /// <summary>
        /// Provides a fluent exception matching interface which is invoked
        /// when the Try fails.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <returns>Fluent exception matcher</returns>
        [Pure]
        public static ExceptionMatchOptionalAsync<T> ifFail<T>(TryOptionAsync<T> self) =>
            self.IfFail();

        /// <summary>
        /// Flattens nested TryOptionAsync computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <returns>Flattened TryOptionAsync computation</returns>
        [Pure]
        public static TryOptionAsync<T> flatten<T>(TryOptionAsync<TryOptionAsync<T>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested TryOptionAsync computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <returns>Flattened TryOptionAsync computation</returns>
        [Pure]
        public static TryOptionAsync<T> flatten<T>(TryOptionAsync<TryOptionAsync<TryOptionAsync<T>>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested TryOptionAsync computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <returns>Flattened TryOptionAsync computation</returns>
        [Pure]
        public static TryOptionAsync<T> flatten<T>(TryOptionAsync<TryOptionAsync<TryOptionAsync<TryOptionAsync<T>>>> self) =>
            self.Flatten();

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            self.Match(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, R> Some, Func<Task<R>> None, Func<Exception, R> Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, Task<R>> Some, Func<R> None, Func<Exception, R> Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, Task<R>> Some, Func<Task<R>> None, Func<Exception, R> Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, R> Some, Func<R> None, Func<Exception, Task<R>> Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, R> Some, Func<Task<R>> None, Func<Exception, Task<R>> Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, Task<R>> Some, Func<R> None, Func<Exception, Task<R>> Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, Task<R>> Some, Func<Task<R>> None, Func<Exception, Task<R>> Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, R> Some, Func<R> None, R Fail) =>
            self.Match(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, R> Some, Func<Task<R>> None, R Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, Task<R>> Some, Func<R> None, R Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        [Pure]
        public static Task<R> match<A, R>(TryOptionAsync<A> self, Func<A, Task<R>> Some, Func<Task<R>> None, R Fail) =>
            self.MatchAsync(Some, None, Fail);

        /// <summary>
        /// Pattern matches the three possible states of the Try computation
        /// </summary>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="None">Delegate to invoke if the Try computation completes successfully but returns no value</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ, None, or Fail delegate</returns>
        public static Task<Unit> match<A>(TryOptionAsync<A> self, Action<A> Some, Action None, Action<Exception> Fail) =>
            self.Match(Some, None, Fail);

        /// <summary>
        /// Invokes a delegate with the result of the TryOptionAsync computation if it is successful.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <param name="action">Delegate to invoke on successful invocation of the TryOptionAsync computation</param>
        public static Task<Unit> iter<T>(TryOptionAsync<T> self, Action<T> action) =>
            self.Iter(action);

        /// <summary>
        /// Folds the value of TryOptionAsync into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="self">TryOptionAsync to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static Task<S> fold<S, T>(TryOptionAsync<T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        /// <summary>
        /// Tests that a predicate holds for all values of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value, or if the TryOptionAsync computation
        /// fails.  False otherwise.</returns>
        [Pure]
        public static Task<bool> forall<T>(TryOptionAsync<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        /// <summary>
        /// Counts the number of bound values.  
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <returns>1 if the TryOptionAsync computation is successful, 0 otherwise.</returns>
        [Pure]
        public static Task<int> count<T>(TryOptionAsync<T> self) =>
            self.Count();

        /// <summary>
        /// Tests that a predicate holds for any value of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
        [Pure]
        public static Task<bool> exists<T>(TryOptionAsync<T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <param name="mapper">Delegate to map the bound value</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static TryOptionAsync<R> map<T, R>(TryOptionAsync<T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">TryOptionAsync computation</param>
        /// <param name="Succ">Delegate to map the bound value</param>
        /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static TryOptionAsync<R> bimap<T, R>(TryOptionAsync<T> tryDel, Func<T, R> Succ, Func<R> Fail) =>
            tryDel.BiMap(Succ, Fail);

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public static TryOptionAsync<Func<T2, R>> parmap<T1, T2, R>(TryOptionAsync<T1> self, Func<T1, T2, R> func) =>
            self.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public static TryOptionAsync<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(TryOptionAsync<T1> self, Func<T1, T2, T3, R> func) =>
            self.ParMap(func);

        [Pure]
        public static TryOptionAsync<T> filter<T>(TryOptionAsync<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        [Pure]
        public static TryOptionAsync<T> bifilter<T>(TryOptionAsync<T> self, Func<T, bool> Succ, Func< bool> Fail) =>
            self.BiFilter(Succ, Fail);

        [Pure]
        public static TryOptionAsync<R> bind<T, R>(TryOptionAsync<T> tryDel, Func<T, TryOptionAsync<R>> binder) =>
            tryDel.Bind(binder);

        [Pure]
        public static TryOptionAsync<R> bibind<T, R>(TryOptionAsync<T> self, Func<T, TryOptionAsync<R>> Succ, Func<TryOptionAsync<R>> Fail) =>
            self.BiBind(Succ, Fail);

        [Pure]
        public static TryOptionAsync<A> plus<A>(TryOptionAsync<A> ma, TryOptionAsync<A> mb) =>
            default(MTryOptionAsync<A>).Plus(ma, mb);

        [Pure]
        public static TryOptionAsync<A> plusFirst<A>(TryOptionAsync<A> ma, TryOptionAsync<A> mb) =>
            default(MTryOptionFirstAsync<A>).Plus(ma, mb);

        [Pure]
        public static Task<Lst<T>> toList<T>(TryOptionAsync<T> tryDel) =>
            tryDel.ToList();

        [Pure]
        public static Task<Arr<T>> toArray<T>(TryOptionAsync<T> tryDel) =>
            tryDel.ToArray();

        [Pure]
        public static TryOptionAsync<T> tryfun<T>(Func<TryOptionAsync<T>> f) => () => 
            f().Try();

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="ma">The first computation to run</param>
        /// <param name="tail">The rest of the computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static TryOptionAsync<A> choice<A>(TryOptionAsync<A> ma, params TryOptionAsync<A>[] tail) =>
            choice(Cons(ma, tail));

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="xs">Sequence of computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static TryOptionAsync<A> choice<A>(Seq<TryOptionAsync<A>> xs) =>
            xs.IsEmpty
                ? new TryOptionAsync<A>(() => OptionalResult<A>.None.AsTask())
                : xs.Head.BiBind(
                    Succ: x => xs.Head,
                    Fail: () => choice(xs.Tail));
    }
}
