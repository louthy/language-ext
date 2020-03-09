using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using LanguageExt.Common;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// TryOption constructor
        /// </summary>
        [Pure]
        public static TryOption<T> TryOption<T>(Func<Option<T>> f) =>
            TryOptionExtensions.Memo<T>(() => f());

        /// <summary>
        /// TryOption constructor
        /// </summary>
        [Pure]
        public static TryOption<T> TryOption<T>(Func<T> f) =>
            TryOptionExtensions.Memo<T>(() => Optional(f()));

        /// <summary>
        /// TryOption constructor
        /// </summary>
        [Pure]
        public static TryOption<T> TryOption<T>(T value) => () => 
            Optional(value);

        /// <summary>
        /// TryOption constructor
        /// </summary>
        [Pure]
        public static TryOption<T> TryOptionSucc<T>(T value) => () => 
            Optional(value);

        /// <summary>
        /// TryOption constructor
        /// </summary>
        [Pure]
        public static TryOption<T> TryOption<T>(Option<T> value) => () => 
            value;

        /// <summary>
        /// TryOption constructor
        /// </summary>
        [Pure]
        public static TryOption<T> TryOptional<T>(Option<T> value) => () => 
            value;

        /// <summary>
        /// TryOption constructor
        /// </summary>
        [Pure]
        public static TryOption<T> TryOption<T>(Exception ex) => () =>
            new OptionalResult<T>(ex);

        /// <summary>
        /// TryOption constructor
        /// </summary>
        [Pure]
        public static TryOption<T> TryOptionFail<T>(Exception ex) => () =>
            new OptionalResult<T>(ex);

        /// <summary>
        /// Add the bound value of Try(x) to Try(y).  If either of the
        /// Trys are Fail then the result is Fail
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static TryOption<A> add<NUM, A>(TryOption<A> lhs, TryOption<A> rhs) where NUM : struct, Num<A> =>
            lhs.Add<NUM, A>(rhs);
    
        /// <summary>
        /// Subtract the Try(x) from Try(y).  If either of the Trys throw then the result is Fail
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
        public static TryOption<T> subtract<NUM, T>(TryOption<T> lhs, TryOption<T> rhs) where NUM : struct, Num<T> =>
            lhs.Subtract<NUM, T>(rhs);

        /// <summary>
        /// Find the product of Try(x) and Try(y).  If either of the Trys throw then the result is Fail
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
        public static TryOption<T> product<NUM, T>(TryOption<T> lhs, TryOption<T> rhs) where NUM : struct, Num<T> =>
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
        public static TryOption<T> divide<NUM, T>(TryOption<T> lhs, TryOption<T> rhs) where NUM : struct, Num<T> =>
            lhs.Divide<NUM, T>(rhs);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static TryOption<B> apply<A, B>(TryOption<Func<A, B>> fab, TryOption<A> fa) =>
            ApplTryOption<A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static TryOption<B> apply<A, B>(Func<A, B> fab, TryOption<A> fa) =>
            ApplTryOption<A, B>.Inst.Apply(TryOption(fab), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static TryOption<C> apply<A, B, C>(TryOption<Func<A, B, C>> fabc, TryOption<A> fa, TryOption<B> fb) =>
            fabc.Bind(f => ApplTryOption<A, B, C>.Inst.Apply(MTryOption<Func<A, Func<B, C>>>.Inst.Return(curry(f)), fa, fb));

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static TryOption<C> apply<A, B, C>(Func<A, B, C> fabc, TryOption<A> fa, TryOption<B> fb) =>
            ApplTryOption<A, B, C>.Inst.Apply(MTryOption<Func<A, Func<B, C>>>.Inst.Return(curry(fabc)), fa, fb);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryOption<Func<B, C>> apply<A, B, C>( TryOption<Func<A, B, C>> fabc, TryOption<A> fa) =>
            fabc.Bind(f => ApplTryOption<A, B, C>.Inst.Apply(MTryOption<Func<A, Func<B, C>>>.Inst.Return(curry(f)), fa));

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryOption<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, TryOption<A> fa) =>
            ApplTryOption<A, B, C>.Inst.Apply(MTryOption<Func<A, Func<B, C>>>.Inst.Return(curry(fabc)), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryOption<Func<B, C>> apply<A, B, C>(TryOption<Func<A, Func<B, C>>> fabc, TryOption<A> fa) =>
            ApplTryOption<A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static TryOption<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, TryOption<A> fa) =>
            ApplTryOption<A, B, C>.Inst.Apply(TryOption(fabc), fa);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type Option<B></returns>
        [Pure]
        public static TryOption<B> action<A, B>(TryOption<A> fa, TryOption<B> fb) =>
            ApplTryOption<A, B>.Inst.Action(fa, fb);
        
        /// <summary>
        /// Test if the Try computation is successful
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation to test</param>
        /// <returns>True if successful</returns>
        [Pure]
        public static bool isSome<T>(TryOption<T> self) =>
            self.Try().Value.IsSome;

        /// <summary>
        /// Test if the Try computation is none
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation to test</param>
        /// <returns>True if result is a None</returns>
        [Pure]
        public static bool isNone<T>(TryOption<T> self) =>
            self.Try().Value.IsNone;

        /// <summary>
        /// Test if the Try computation fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation to test</param>
        /// <returns>True if fails</returns>
        [Pure]
        public static bool isFail<T>(TryOption<T> self) =>
            self.Try().IsFaulted;

        /// <summary>
        /// Invoke delegates based on None or Failed stateds
        /// </summary>
        /// <typeparam name="T">Bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="None">Delegate to invoke if the result is None</param>
        /// <param name="Fail">Delegate to invoke if the result is Fail</param>
        /// <returns>Success value, or the result of the None or Failed delegate</returns>
        [Pure]
        public static T ifNoneOrFail<T>(
            TryOption<T> self,
            Func<T> None,
            Func<Exception, T> Fail) => 
            self.IfNoneOrFail(None, Fail);

        /// <summary>
        /// Invoke a delegate if the Try returns a value successfully
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to invoke if successful</param>
        public static Unit ifSome<T>(TryOption<T> self, Action<T> Some) =>
            self.IfSome(Some);

        /// <summary>
        /// Invoke a delegate if the Try fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="value">Try computation</param>
        /// <param name="Fail">Delegate to invoke on failure</param>
        /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
        [Pure]
        public static T ifNoneOrFail<T>(TryOption<T> self, Func<T> Fail) =>
            self.IfNoneOrFail(Fail);

        /// <summary>
        /// Return a default value if the Try fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="defaultValue">Default value to use on failure</param>
        /// <returns>failValue on failure, the result of the Try otherwise</returns>
        [Pure]
        public static T ifNoneOrFail<T>(TryOption<T> self, T defaultValue) =>
            self.IfNoneOrFail(defaultValue);

        /// <summary>
        /// Provides a fluent exception matching interface which is invoked
        /// when the Try fails.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Fluent exception matcher</returns>
        [Pure]
        public static ExceptionMatch<Option<T>> ifFail<T>(TryOption<T> self) =>
            self.IfFail();

        /// <summary>
        /// Maps the bound value to the resulting exception (if the Try computation fails).  
        /// If the Try computation succeeds then a NotSupportedException is used.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Try of Exception</returns>
        [Pure]
        public static TryOption<Exception> failed<T>(TryOption<T> self) =>
            self.TriMap(
                Some: _   => new NotSupportedException(),
                None: ()  => new NotSupportedException(),
                Fail: ex  => ex
                );

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static TryOption<T> flatten<T>(TryOption<TryOption<T>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static TryOption<T> flatten<T>(TryOption<TryOption<TryOption<T>>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static TryOption<T> flatten<T>(TryOption<TryOption<TryOption<TryOption<T>>>> self) =>
            self.Flatten();

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Some or Fail delegates</returns>
        [Pure]
        public static R match<T, R>(TryOption<T> self, Func<T, R> Some, Func<R> Fail) =>
            self.Match(Some, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Some or Fail delegates</returns>
        [Pure]
        public static R match<T, R>(TryOption<T> self, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            self.Match(Some, None, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Default value to use if the Try computation fails</param>
        /// <returns>The result of either the Succ delegate or the Fail value</returns>
        [Pure]
        public static R match<T, R>(TryOption<T> self, Func<T, R> Some, R Fail) =>
            self.Match(Some, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        public static Unit match<T>(TryOption<T> self, Action<T> Some, Action Fail) =>
            self.Match(Some, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        public static Unit match<T>(TryOption<T> self, Action<T> Some, Action None, Action<Exception> Fail) =>
            self.Match(Some, None, Fail);

        /// <summary>
        /// Invokes a delegate with the result of the Try computation if it is successful.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="action">Delegate to invoke on successful invocation of the Try computation</param>
        public static Unit iter<T>(TryOption<T> self, Action<T> action) =>
            self.Iter(action);

        /// <summary>
        /// Folds the value of Try into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="self">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(TryOption<T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        /// <summary>
        /// Folds the result of Try into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Fold function when Try succeeds</param>
        /// <param name="Fail">Fold function when Try fails</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S bifold<S, T>(TryOption<T> self, S state, Func<S, T, S> Some, Func<S, S> Fail) =>
            self.BiFold(state, Some, Fail);

        /// <summary>
        /// Folds the result of Try into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Fold function when Try succeeds</param>
        /// <param name="Fail">Fold function when Try fails</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S trifold<S, T>(TryOption<T> self, S state, Func<S, T, S> Some, Func<S, S> None, Func<S, Exception, S> Fail) =>
            self.TriFold(state, Some, None, Fail);

        /// <summary>
        /// Tests that a predicate holds for all values of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value, or if the Try computation
        /// fails.  False otherwise.</returns>
        [Pure]
        public static bool forall<T>(TryOption<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        /// <summary>
        /// Counts the number of bound values.  
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TrTry computation</param>
        /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
        [Pure]
        public static int count<T>(TryOption<T> self) =>
            self.Count();

        /// <summary>
        /// Tests that a predicate holds for any value of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
        [Pure]
        public static bool exists<T>(TryOption<T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="mapper">Delegate to map the bound value</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static TryOption<R> map<T, R>(TryOption<T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to map the bound value</param>
        /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static TryOption<R> bimap<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> Fail) =>
            tryDel.BiMap(Some, Fail);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Some">Delegate to map the bound value</param>
        /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static TryOption<R> trimap<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            tryDel.TriMap(Some, None, Fail);

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public static TryOption<Func<T2, R>> parmap<T1, T2, R>(TryOption<T1> self, Func<T1, T2, R> func) =>
            self.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public static TryOption<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(TryOption<T1> self, Func<T1, T2, T3, R> func) =>
            self.ParMap(func);

        [Pure]
        public static TryOption<T> filter<T>(TryOption<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        [Pure]
        public static TryOption<R> bind<T, R>(TryOption<T> tryDel, Func<T, TryOption<R>> binder) =>
            tryDel.Bind(binder);

        [Pure]
        public static TryOption<R> bibind<T, R>(TryOption<T> self, Func<T, TryOption<R>> Some, Func<TryOption<R>> Fail) =>
            self.BiBind(Some, Fail);

        [Pure]
        public static TryOption<R> tribind<T, R>(TryOption<T> self, Func<T, TryOption<R>> Some, Func<TryOption<R>> None, Func<Exception, TryOption<R>> Fail) =>
            self.TriBind(Some, None, Fail);

        [Pure]
        public static Lst<T> toList<T>(TryOption<T> tryDel) =>
            tryDel.ToList();

        [Pure]
        public static Arr<T> toArray<T>(TryOption<T> tryDel) =>
            tryDel.ToArray();

        [Pure]
        public static TryOption<T> tryfun<T>(Func<TryOption<T>> tryDel) =>
            () => tryDel()();

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="ma">The first computation to run</param>
        /// <param name="tail">The rest of the computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static TryOption<A> choice<A>(TryOption<A> ma, params TryOption<A>[] tail) =>
            choice(Cons(ma, tail));

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="xs">Sequence of computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static TryOption<A> choice<A>(Seq<TryOption<A>> xs) =>
            xs.IsEmpty
                ? new TryOption<A>(() => OptionalResult<A>.None)
                : xs.Head.BiBind(
                    Some: x => xs.Head,
                    Fail: () => choice(xs.Tail));
    }
}
