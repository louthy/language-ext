using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using LanguageExt.Common;
using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Try constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="f">Function to run when invoked</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static Try<A> Try<A>(Func<A> f) =>
            TryExtensions.Memo<A>(() => f());

        /// <summary>
        /// Try identity constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static Try<A> Try<A>(A v) =>
            () => v;

        /// <summary>
        /// Try identity constructor function
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="v">Bound value to return</param>
        /// <returns>A lifted operation that returns a value of A</returns>
        [Pure]
        public static Try<A> Try<A>(Exception ex) => () =>
            new Result<A>(ex);

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="ma">The first computation to run</param>
        /// <param name="tail">The rest of the computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static Try<A> choice<A>(Try<A> ma, params Try<A>[] tail) =>
            choice(Cons(ma, tail));

        /// <summary>
        /// Returns the first successful computation 
        /// </summary>
        /// <typeparam name="A">Bound value</typeparam>
        /// <param name="xs">Sequence of computations to run</param>
        /// <returns>The first computation that succeeds</returns>
        [Pure]
        public static Try<A> choice<A>(Seq<Try<A>> xs) =>
            xs.IsEmpty
                ? new Try<A>(() => Result<A>.Bottom)
                : xs.Head.BiBind(
                    Succ: x => xs.Head,
                    Fail: _ => choice(xs.Tail));

        /// <summary>
        /// Append the bound value of Try(x) to Try(y).  If either of the
        /// Trys are Fail then the result is Fail
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs ++ rhs</returns>
        [Pure]
        public static Try<A> append<SEMI, A>(Try<A> lhs, Try<A> rhs) where SEMI : struct, Semigroup<A> =>
            lhs.Append<SEMI, A>(rhs);

        /// <summary>
        /// Add the bound value of Try(x) to Try(y).  If either of the
        /// Trys are Fail then the result is Fail
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static Try<A> add<NUM, A>(Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
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
        public static Try<T> subtract<NUM, T>(Try<T> lhs, Try<T> rhs) where NUM : struct, Num<T> =>
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
        public static Try<T> product<NUM, T>(Try<T> lhs, Try<T> rhs) where NUM : struct, Num<T> =>
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
        public static Try<T> divide<NUM, T>(Try<T> lhs, Try<T> rhs) where NUM : struct, Num<T> =>
            lhs.Divide<NUM, T>(rhs);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static Try<B> apply<A, B>(Try<Func<A, B>> fab, Try<A> fa) =>
            ApplTry<A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static Try<B> apply<A, B>(Func<A, B> fab, Try<A> fa) =>
            ApplTry<A, B>.Inst.Apply(Try(fab), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static Try<C> apply<A, B, C>(Try<Func<A, B, C>> fabc, Try<A> fa, Try<B> fb) =>
            fabc.Bind(f => ApplTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(f)), fa, fb));

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static Try<C> apply<A, B, C>(Func<A, B, C> fabc, Try<A> fa, Try<B> fb) =>
            ApplTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(fabc)), fa, fb);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static Try<Func<B, C>> apply<A, B, C>(Try<Func<A, B, C>> fabc, Try<A> fa) =>
            fabc.Bind(f => ApplTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(f)), fa));

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static Try<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, Try<A> fa) =>
            ApplTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(fabc)), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static Try<Func<B, C>> apply<A, B, C>(Try<Func<A, Func<B, C>>> fabc, Try<A> fa) =>
            ApplTry<A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static Try<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, Try<A> fa) =>
            ApplTry<A, B, C>.Inst.Apply(Try(fabc), fa);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type Option<B></returns>
        [Pure]
        public static Try<B> action<A, B>(Try<A> fa, Try<B> fb) =>
            ApplTry<A, B>.Inst.Action(fa, fb);

        /// <summary>
        /// Test if the Try computation is successful
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation to test</param>
        /// <returns>True if successful</returns>
        [Pure]
        public static bool isSucc<T>(Try<T> self) =>
            !isFail(self);

        /// <summary>
        /// Test if the Try computation fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation to test</param>
        /// <returns>True if fails</returns>
        [Pure]
        public static bool isFail<T>(Try<T> self) =>
            self.Try().IsFaulted;

        /// <summary>
        /// Invoke a delegate if the Try returns a value successfully
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if successful</param>
        public static Unit ifSucc<T>(Try<T> self, Action<T> Succ) =>
            self.IfSucc(Succ);

        /// <summary>
        /// Invoke a delegate if the Try fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="value">Try computation</param>
        /// <param name="Fail">Delegate to invoke on failure</param>
        /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
        [Pure]
        public static T ifFail<T>(Try<T> self, Func<T> Fail) =>
            self.IfFail(Fail);

        /// <summary>
        /// Return a default value if the Try fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="failValue">Default value to use on failure</param>
        /// <returns>failValue on failure, the result of the Try otherwise</returns>
        [Pure]
        public static T ifFail<T>(Try<T> self, T failValue) =>
            self.IfFail(failValue);

        /// <summary>
        /// Provides a fluent exception matching interface which is invoked
        /// when the Try fails.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Fluent exception matcher</returns>
        [Pure]
        public static ExceptionMatch<T> ifFail<T>(Try<T> self) =>
            self.IfFail();

        /// <summary>
        /// Maps the bound value to the resulting exception (if the Try computation fails).  
        /// If the Try computation succeeds then a NotSupportedException is used.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Try of Exception</returns>
        [Pure]
        public static Try<Exception> failed<T>(Try<T> self) =>
            bimap(self, 
                Succ: _  => new NotSupportedException(),
                Fail: ex => ex
                );

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static Try<T> flatten<T>(Try<Try<T>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static Try<T> flatten<T>(Try<Try<Try<T>>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static Try<T> flatten<T>(Try<Try<Try<Try<T>>>> self) =>
            self.Flatten();

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ or Fail delegates</returns>
        [Pure]
        public static R match<T, R>(Try<T> self, Func<T, R> Succ, Func<Exception, R> Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Default value to use if the Try computation fails</param>
        /// <returns>The result of either the Succ delegate or the Fail value</returns>
        [Pure]
        public static R match<T, R>(Try<T> self, Func<T, R> Succ, R Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        public static Unit match<T>(Try<T> self, Action<T> Succ, Action<Exception> Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Invokes a delegate with the result of the Try computation if it is successful.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="action">Delegate to invoke on successful invocation of the Try computation</param>
        public static Unit iter<T>(Try<T> self, Action<T> action) =>
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
        public static S fold<S, T>(Try<T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        /// <summary>
        /// Folds the result of Try into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Succ">Fold function when Try succeeds</param>
        /// <param name="Fail">Fold function when Try fails</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(Try<T> self, S state, Func<S, T, S> Succ, Func<S, Exception, S> Fail) =>
            self.BiFold(state, Succ, Fail);

        /// <summary>
        /// Tests that a predicate holds for all values of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value, or if the Try computation
        /// fails.  False otherwise.</returns>
        [Pure]
        public static bool forall<T>(Try<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        /// <summary>
        /// Counts the number of bound values.  
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TrTry computation</param>
        /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
        [Pure]
        public static int count<T>(Try<T> self) =>
            self.Count();

        /// <summary>
        /// Tests that a predicate holds for any value of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
        [Pure]
        public static bool exists<T>(Try<T> self, Func<T, bool> pred) =>
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
        public static Try<R> map<T, R>(Try<T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to map the bound value</param>
        /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static Try<R> bimap<T, R>(Try<T> tryDel, Func<T, R> Succ, Func<Exception, R> Fail) =>
            tryDel.BiMap(Succ, Fail);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Try<Func<T2, R>> parmap<T1, T2, R>(Try<T1> self, Func<T1, T2, R> func) =>
            self.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Try<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(Try<T1> self, Func<T1, T2, T3, R> func) =>
            self.ParMap(func);

        [Pure]
        public static Try<T> filter<T>(Try<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        [Obsolete]
        public static Try<T> filter<T>(Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail) =>
            self.BiFilter(Succ, Fail);

        [Pure]
        public static Try<T> bifilter<T>(Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail) =>
            self.BiFilter(Succ, Fail);

        [Pure]
        public static Try<R> bind<T, R>(Try<T> tryDel, Func<T, Try<R>> binder) =>
            tryDel.Bind(binder);

        [Obsolete]
        public static Try<R> bind<T, R>(Try<T> self, Func<T, Try<R>> Succ, Func<Exception, Try<R>> Fail) =>
            self.BiBind(Succ, Fail);

        [Pure]
        public static Try<R> bibind<T, R>(Try<T> self, Func<T, Try<R>> Succ, Func<Exception, Try<R>> Fail) =>
            self.BiBind(Succ, Fail);

        [Pure]
        public static Lst<T> toList<T>(Try<T> tryDel) =>
            tryDel.ToList();

        [Pure]
        public static Arr<T> toArray<T>(Try<T> tryDel) =>
            tryDel.ToArray();

        [Pure]
        public static Try<T> tryfun<T>(Func<Try<T>> tryDel) => () => 
            tryDel()().Value;

        /// <summary>
        /// Partitions a list of 'Try' into two lists.
        /// All the 'Fail' elements are extracted, in order, to the first
        /// component of the output.  Similarly the 'Succ' elements are extracted
        /// to the second component of the output.
        /// </summary>
        /// <typeparam name="A">Succ</typeparam>
        /// <param name="self">Try list</param>
        /// <returns>A tuple containing the an enumerable of Exception and an enumerable of A</returns>
        [Pure]
        public static (IEnumerable<Exception> Fails, IEnumerable<A> Succs) partition<A>(IEnumerable<Try<A>> self) =>
            Choice.partition<MTry<A>, Try<A>, Exception, A>(self);

        /// <summary>
        /// Partitions a list of 'Try' into two lists.
        /// All the 'Fail' elements are extracted, in order, to the first
        /// component of the output.  Similarly the 'Succ' elements are extracted
        /// to the second component of the output.
        /// </summary>
        /// <typeparam name="A">Succ</typeparam>
        /// <param name="self">Try list</param>
        /// <returns>A tuple containing the an enumerable of Exception and an enumerable of A</returns>
        [Pure]
        public static (Seq<Exception> Fails, Seq<A> Succs) partition<A>(Seq<Try<A>> self) =>
            Choice.partition<MTry<A>, Try<A>, Exception, A>(self);

        [Pure]
        public static Seq<Exception> fails<A>(Seq<Try<A>> self) =>
            Choice.lefts<MTry<A>, Try<A>, Exception, A>(self);

        [Pure]
        public static Seq<A> succs<A>(Seq<Try<A>> self) =>
            Choice.rights<MTry<A>, Try<A>, Exception, A>(self);

        [Pure]
        public static IEnumerable<Exception> fails<A>(IEnumerable<Try<A>> self) =>
            Choice.lefts<MTry<A>, Try<A>, Exception, A>(self);

        [Pure]
        public static IEnumerable<A> succs<A>(IEnumerable<Try<A>> self) =>
            Choice.rights<MTry<A>, Try<A>, Exception, A>(self);
    }
}
