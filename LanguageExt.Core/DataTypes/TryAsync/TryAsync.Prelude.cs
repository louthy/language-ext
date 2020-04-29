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
        /// TryAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="f">Function to run asynchronously</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryAsync<A> TryAsync<A>(Func<Task<A>> f) =>
            TryAsyncExtensions.Memo<A>(new LanguageExt.TryAsync<A>(async () => new Result<A>(await f())));

        /// <summary>
        /// TryAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Task to run asynchronously</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryAsync<A> TryAsync<A>(Task<A> v) =>
            TryAsyncExtensions.Memo<A>(new LanguageExt.TryAsync<A>(async () => new Result<A>(await v)));

        /// <summary>
        /// TryAsync identity constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryAsync<A> TryAsync<A>(A v) => () =>
            new Result<A>(v).AsTask();

        /// <summary>
        /// TryOptionAsync constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static TryAsync<A> TryAsync<A>(Exception ex) => () =>
            new Result<A>(ex).AsTask();

        /// <summary>
        /// Append the bound value of TryAsync(x) to TryAsync(y).  If either of the
        /// Trys are Fail then the result is Fail
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs ++ rhs</returns>
        [Pure]
        public static TryAsync<A> append<SEMI, A>(TryAsync<A> lhs, TryAsync<A> rhs) where SEMI : struct, Semigroup<A> =>
            lhs.Append<SEMI, A>(rhs);
    
        /// <summary>
        /// Add the bound value of TryAsync(x) to TryAsync(y).  If either of the
        /// Trys are Fail then the result is Fail
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static TryAsync<A> add<NUM, A>(TryAsync<A> lhs, TryAsync<A> rhs) where NUM : struct, Num<A> =>
            lhs.Add<NUM, A>(rhs);

        /// <summary>
        /// Subtract the TryAsync(x) from TryAsync(y).  If either of the Trys throw then the result is Fail
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
        public static TryAsync<T> subtract<NUM, T>(TryAsync<T> lhs, TryAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Subtract<NUM, T>(rhs);

        /// <summary>
        /// Find the product of TryAsync(x) and TryAsync(y).  If either of the Trys throw then the result is Fail
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
        public static TryAsync<T> product<NUM, T>(TryAsync<T> lhs, TryAsync<T> rhs) where NUM : struct, Num<T> =>
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
        public static TryAsync<T> divide<NUM, T>(TryAsync<T> lhs, TryAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Divide<NUM, T>(rhs);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static TryAsync<B> apply<A, B>(TryAsync<Func<A, B>> fab, TryAsync<A> fa) =>
            ApplTryAsync<A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static TryAsync<B> apply<A, B>(Func<A, B> fab, TryAsync<A> fa) =>
            ApplTryAsync<A, B>.Inst.Apply(TryAsync(fab), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static TryAsync<C> apply<A, B, C>(TryAsync<Func<A, B, C>> fabc, TryAsync<A> fa, TryAsync<B> fb) =>
            fabc.Bind(f => ApplTryAsync<A, B, C>.Inst.Apply(MTryAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(f).AsTask()), fa, fb));

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static TryAsync<C> apply<A, B, C>(Func<A, B, C> fabc, TryAsync<A> fa, TryAsync<B> fb) =>
            ApplTryAsync<A, B, C>.Inst.Apply(MTryAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(fabc).AsTask()), fa, fb);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryAsync<Func<B, C>> apply<A, B, C>(TryAsync<Func<A, B, C>> fabc, TryAsync<A> fa) =>
            fabc.Bind(f => ApplTryAsync<A, B, C>.Inst.Apply(MTryAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(f).AsTask()), fa));

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryAsync<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, TryAsync<A> fa) =>
            ApplTryAsync<A, B, C>.Inst.Apply(MTryAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(fabc).AsTask()), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryAsync<Func<B, C>> apply<A, B, C>(TryAsync<Func<A, Func<B, C>>> fabc, TryAsync<A> fa) =>
            ApplTryAsync<A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryAsync<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, TryAsync<A> fa) =>
            ApplTryAsync<A, B, C>.Inst.Apply(TryAsync(fabc), fa);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type Option<B></returns>
        [Pure]
        public static TryAsync<B> action<A, B>(TryAsync<A> fa, TryAsync<B> fb) =>
            ApplTryAsync<A, B>.Inst.Action(fa, fb);

        /// <summary>
        /// Test if the TryAsync computation is successful
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation to test</param>
        /// <returns>True if successful</returns>
        [Pure]
        public static TryAsync<bool> isSucc<T>(TryAsync<T> self) =>
            from x in isFail(self)
            select !x;

        /// <summary>
        /// Test if the TryAsync computation fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation to test</param>
        /// <returns>True if fails</returns>
        [Pure]
        public static TryAsync<bool> isFail<T>(TryAsync<T> self) => async () =>
            (await self.Try()).IsFaulted;

        /// <summary>
        /// Invoke a delegate if the TryAsync returns a value successfully
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <param name="Succ">Delegate to invoke if successful</param>
        public static Task<Unit> ifSucc<T>(TryAsync<T> self, Action<T> Succ) =>
            self.IfSucc(Succ);

        /// <summary>
        /// Invoke a delegate if the TryAsync fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="value">TryAsync computation</param>
        /// <param name="Fail">Delegate to invoke on failure</param>
        /// <returns>Result of the invocation of Fail on failure, the result of the TryAsync otherwise</returns>
        [Pure]
        public static Task<T> ifFail<T>(TryAsync<T> self, Func<T> Fail) =>
            self.IfFail(Fail);

        /// <summary>
        /// Return a default value if the TryAsync fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <param name="failValue">Default value to use on failure</param>
        /// <returns>failValue on failure, the result of the TryAsync otherwise</returns>
        [Pure]
        public static Task<T> ifFail<T>(TryAsync<T> self, T failValue) =>
            self.IfFail(failValue);

        /// <summary>
        /// Provides a fluent exception matching interface which is invoked
        /// when the Try fails.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <returns>Fluent exception matcher</returns>
        [Pure]
        public static ExceptionMatchAsync<T> ifFail<T>(TryAsync<T> self) =>
            self.IfFail();

        /// <summary>
        /// Maps the bound value to the resulting exception (if the TryAsync computation fails).  
        /// If the TryAsync computation succeeds then a NotSupportedException is used.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <returns>Try of Exception</returns>
        [Pure]
        public static TryAsync<Exception> failed<T>(TryAsync<T> self) =>
            bimap(self, 
                Succ: _  => new NotSupportedException(),
                Fail: ex => ex
                );

        /// <summary>
        /// Flattens nested TryAsync computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <returns>Flattened TryAsync computation</returns>
        [Pure]
        public static TryAsync<T> flatten<T>(TryAsync<TryAsync<T>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested TryAsync computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <returns>Flattened TryAsync computation</returns>
        [Pure]
        public static TryAsync<T> flatten<T>(TryAsync<TryAsync<TryAsync<T>>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested TryAsync computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <returns>Flattened TryAsync computation</returns>
        [Pure]
        public static TryAsync<T> flatten<T>(TryAsync<TryAsync<TryAsync<TryAsync<T>>>> self) =>
            self.Flatten();

        /// <summary>
        /// Pattern matches the two possible states of the TryAsync computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if the TryAsync computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the TryAsync computation fails</param>
        /// <returns>The result of either the Succ or Fail delegates</returns>
        [Pure]
        public static Task<R> match<T, R>(TryAsync<T> self, Func<T, R> Succ, Func<Exception, R> Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the TryAsync computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <param name="Succ">Delegate to invoke if the TryAsync computation completes successfully</param>
        /// <param name="Fail">Default value to use if the TryAsync computation fails</param>
        /// <returns>The result of either the Succ delegate or the Fail value</returns>
        [Pure]
        public static Task<R> match<T, R>(TryAsync<T> self, Func<T, R> Succ, R Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the TryAsync computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <param name="Succ">Delegate to invoke if the TryAsync computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the TryAsync computation fails</param>
        public static Task<Unit> match<T>(TryAsync<T> self, Action<T> Succ, Action<Exception> Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Invokes a delegate with the result of the TryAsync computation if it is successful.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <param name="action">Delegate to invoke on successful invocation of the TryAsync computation</param>
        public static Task<Unit> iter<T>(TryAsync<T> self, Action<T> action) =>
            self.Iter(action);

        /// <summary>
        /// Folds the value of TryAsync into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="self">TryAsync to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static Task<S> fold<S, T>(TryAsync<T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        /// <summary>
        /// Folds the result of TryAsync into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">TryAsync to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Succ">Fold function when TryAsync succeeds</param>
        /// <param name="Fail">Fold function when TryAsync fails</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static Task<S> fold<S, T>(TryAsync<T> self, S state, Func<S, T, S> Succ, Func<S, Exception, S> Fail) =>
            self.BiFold(state, Succ, Fail);

        /// <summary>
        /// Tests that a predicate holds for all values of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value, or if the TryAsync computation
        /// fails.  False otherwise.</returns>
        [Pure]
        public static Task<bool> forall<T>(TryAsync<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        /// <summary>
        /// Counts the number of bound values.  
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <returns>1 if the TryAsync computation is successful, 0 otherwise.</returns>
        [Pure]
        public static Task<int> count<T>(TryAsync<T> self) =>
            self.Count();

        /// <summary>
        /// Tests that a predicate holds for any value of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
        [Pure]
        public static Task<bool> exists<T>(TryAsync<T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <param name="mapper">Delegate to map the bound value</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static TryAsync<R> map<T, R>(TryAsync<T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">TryAsync computation</param>
        /// <param name="Succ">Delegate to map the bound value</param>
        /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static TryAsync<R> bimap<T, R>(TryAsync<T> tryDel, Func<T, R> Succ, Func<Exception, R> Fail) =>
            tryDel.BiMap(Succ, Fail);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static TryAsync<Func<T2, R>> parmap<T1, T2, R>(TryAsync<T1> self, Func<T1, T2, R> func) =>
            self.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static TryAsync<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(TryAsync<T1> self, Func<T1, T2, T3, R> func) =>
            self.ParMap(func);

        [Pure]
        public static TryAsync<T> filter<T>(TryAsync<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        [Pure]
        public static TryAsync<T> bifilter<T>(TryAsync<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail) =>
            self.BiFilter(Succ, Fail);

        [Pure]
        public static TryAsync<R> bind<T, R>(TryAsync<T> tryDel, Func<T, TryAsync<R>> binder) =>
            tryDel.Bind(binder);

        [Pure]
        public static TryAsync<R> bibind<T, R>(TryAsync<T> self, Func<T, TryAsync<R>> Succ, Func<Exception, TryAsync<R>> Fail) =>
            self.BiBind(Succ, Fail);

        [Pure]
        public static TryAsync<A> plus<A>(TryAsync<A> ma, TryAsync<A> mb) =>
            default(MTryAsync<A>).Plus(ma, mb);

        [Pure]
        public static TryAsync<A> plusFirst<A>(TryAsync<A> ma, TryAsync<A> mb) =>
            default(MTryFirstAsync<A>).Plus(ma, mb);

        [Pure]
        public static Task<Lst<T>> toList<T>(TryAsync<T> tryDel) =>
            tryDel.ToList();

        [Pure]
        public static Task<Arr<T>> toArray<T>(TryAsync<T> tryDel) =>
            tryDel.ToArray();

        [Pure]
        public static TryAsync<T> tryfun<T>(Func<TryAsync<T>> f) => () => 
            f().Try();

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="ma">The first computation to run</param>
        /// <param name="tail">The rest of the computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static TryAsync<A> choice<A>(TryAsync<A> ma, params TryAsync<A>[] tail) =>
            choice(Cons(ma, tail));

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="xs">Sequence of computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static TryAsync<A> choice<A>(Seq<TryAsync<A>> xs) =>
            xs.IsEmpty
                ? new TryAsync<A>(() => Result<A>.Bottom.AsTask())
                : xs.Head.BiBind(
                    Succ: x => xs.Head,
                    Fail: _ => choice(xs.Tail));
    }
}
