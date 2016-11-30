using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Append the Some(x) of one option to the Some(y) of another.  If either of the
        /// options are None then the result is None
        /// For numeric values the behaviour is to sum the Somes (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the R type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static Option<T> append<T>(Option<T> lhs, Option<T> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Subtract the Some(x) of one option from the Some(y) of another.  If either of the
        /// options are None then the result is None
        /// For numeric values the behaviour is to find the difference between the Somes (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the R type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static Option<T> subtract<T>(Option<T> lhs, Option<T> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Find the product of the Somes.  If either of the options are None then the result is None
        /// For numeric values the behaviour is to multiply the Somes (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public static Option<T> multiply<T>(Option<T> lhs, Option<T> rhs) =>
            lhs.Multiply(rhs);

        /// <summary>
        /// Divide the Somes.  If either of the options are None then the result is None
        /// For numeric values the behaviour is to divide the Somes (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static Option<T> divide<T>(Option<T> lhs, Option<T> rhs) =>
            lhs.Divide(rhs);

        /// <summary>
        /// Check if Option is in a Some state
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Option</param>
        /// <returns>True if value is in a Some state</returns>
        [Pure]
        public static bool isSome<T>(Option<T> value) =>
            value.IsSome;

        /// <summary>
        /// Check if Option is in a None state
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Option</param>
        /// <returns>True if value is in a None state</returns>
        [Pure]
        public static bool isNone<T>(Option<T> value) =>
            value.IsNone;

        /// <summary>
        /// Create a Some of T (Option<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>Option<T> in a Some state or throws ValueIsNullException
        /// if isnull(value).</returns>
        [Pure]
        public static Option<T> Some<T>(T value) =>
            isnull(value)
                ? raise<Option<T>>(new ValueIsNullException())
                : Option<T>.Some(value);

        /// <summary>
        /// Create a Some of T from a Nullable<T> (Option<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>Option<T> in a Some state or throws ValueIsNullException
        /// if isnull(value)</returns>
        [Pure]
        public static Option<T> Some<T>(T? value) where T : struct =>
            value.HasValue
                ? Option<T>.Some(value.Value)
                : raise<Option<T>>(new ValueIsNullException());

        /// <summary>
        /// Create an Option
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to be made optional, or null</param>
        /// <returns>If the value is null it will be None else Some(value)</returns>
        [Pure]
        public static Option<T> Optional<T>(T value) =>
            isnull(value)
                ? Option<T>.None
                : Option<T>.Some(value);

        /// <summary>
        /// Create an Option
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to be made optional, or null</param>
        /// <returns>If the value is null it will be None else Some(value)</returns>
        [Pure]
        public static Option<T> Optional<T>(T? value) where T : struct =>
            value.HasValue
                ? Option<T>.Some(value.Value)
                : Option<T>.None;

        public static Unit ifSome<T>(Option<T> option, Action<T> Some) => 
            option.IfSome(Some);

        [Pure]
        public static T ifNone<T>(Option<T> option, Func<T> None) =>
            option.IfNone(None);

        [Pure]
        public static T ifNone<T>(Option<T> option, T noneValue) =>
            option.IfNone(noneValue);

        [Pure]
        public static T ifNoneUnsafe<T>(Option<T> option, Func<T> None) =>
            option.IfNoneUnsafe(None);

        [Pure]
        public static T ifNoneUnsafe<T>(Option<T> option, T noneValue) =>
            option.IfNoneUnsafe(noneValue);

        [Pure]
        public static R match<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
            option.Match(Some, None);

        [Pure]
        public static R matchUnsafe<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
            option.MatchUnsafe(Some, None);

        public static Unit match<T>(Option<T> option, Action<T> Some, Action None) =>
            option.Match(Some, None);

        /// <summary>
        /// Apply an Optional value to an Optional function
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg">Optional argument</param>
        /// <returns>Returns the result of applying the optional argument to the optional function</returns>
        [Pure]
        public static Option<R> apply<T, R>(Option<Func<T, R>> option, Option<T> arg) =>
            option.Apply(arg);

        /// <summary>
        /// Apply an Optional value to an Optional function of arity 2
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg">Optional argument</param>
        /// <returns>Returns the result of applying the optional argument to the optional function:
        /// an optonal function of arity 1</returns>
        [Pure]
        public static Option<Func<T2, R>> apply<T1, T2, R>(Option<Func<T1, T2, R>> option, Option<T1> arg) =>
            option.Apply(arg);

        /// <summary>
        /// Apply Optional values to an Optional function of arity 2
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg1">Optional argument</param>
        /// <param name="arg2">Optional argument</param>
        /// <returns>Returns the result of applying the optional arguments to the optional function</returns>
        [Pure]
        public static Option<R> apply<T1, T2, R>(Option<Func<T1, T2, R>> option, Option<T1> arg1, Option<T2> arg2) =>
            option.Apply(arg1, arg2);

        /// <summary>
        /// Folds the option into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="option">Option to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(Option<T> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        /// <summary>
        /// Folds the option into an S
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="option">Option to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Fold function for Some</param>
        /// <param name="None">Fold function for None</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(Option<T> option, S state, Func<S, T, S> Some, Func<S, S> None) =>
            option.Fold(state, Some, None);

        [Pure]
        public static bool forall<T>(Option<T> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        [Pure]
        public static bool forall<T>(Option<T> option, Func<T, bool> Some, Func<bool> None) =>
            option.ForAll(Some, None);

        [Pure]
        public static int count<T>(Option<T> option) =>
            option.Count();

        [Pure]
        public static bool exists<T>(Option<T> option, Func<T, bool> pred) =>
            option.Exists(pred);

        [Pure]
        public static bool exists<T>(Option<T> option, Func<T, bool> Some, Func<bool> None) =>
            option.Exists(Some, None);

        [Pure]
        public static Option<R> map<T, R>(Option<T> option, Func<T, R> mapper) =>
            option.Map(mapper);

        [Pure]
        public static Option<R> map<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
            option.Map(Some, None);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Option<Func<T2, R>> parmap<T1, T2, R>(Option<T1> option, Func<T1, T2, R> mapper) =>
            option.ParMap(mapper);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Option<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(Option<T1> option, Func<T1, T2, T3, R> mapper) =>
            option.ParMap(mapper);

        [Pure]
        public static Option<T> filter<T>(Option<T> option, Func<T, bool> pred) =>
            option.Filter(pred);

        [Pure]
        public static Option<T> filter<T>(Option<T> option, Func<T, bool> Some, Func<bool> None) =>
            option.Filter(Some, None);

        [Pure]
        public static Option<R> bind<T, R>(Option<T> option, Func<T, Option<R>> binder) =>
            option.Bind(binder);

        [Pure]
        public static Option<R> bind<T, R>(Option<T> option, Func<T, Option<R>> Some, Func<Option<R>> None) =>
            option.Bind(Some, None);

        [Pure]
        public static IEnumerable<R> match<T, R>(IEnumerable<Option<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            list.Match(
                None,
                opt => opt.Some(v => Some(v)).None(None),
                (x, xs) => x.Some(v => Some(v)).None(None).Concat(match(xs, Some, None)) // TODO: Flatten recursion
            );

        [Pure]
        public static IEnumerable<R> match<T, R>(IEnumerable<Option<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            match(list, Some, () => None);

        [Pure]
        public static IEnumerable<R> Match<T, R>(this IEnumerable<Option<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            match(list, Some, None);

        [Pure]
        public static IEnumerable<R> Match<T, R>(this IEnumerable<Option<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            match(list, Some, () => None);

        /// <summary>
        /// Extracts from a list of 'Option' all the 'Some' elements.
        /// All the 'Some' elements are extracted in order.
        /// </summary>
        [Pure]
        public static IEnumerable<T> somes<T>(IEnumerable<Option<T>> list) =>
            list.Somes();

        [Pure]
        public static Lst<T> toList<T>(Option<T> option) =>
            option.ToList();

        [Pure]
        public static T[] toArray<T>(Option<T> option) =>
            option.ToArray();

        [Pure]
        public static IQueryable<T> toQuery<T>(Option<T> option) =>
            option.AsEnumerable().AsQueryable();

        public static Task<Option<R>> mapAsync<T, R>(Option<T> self, Func<T, Task<R>> map) =>
            self.MapAsync(map);

        public static Task<Option<R>> mapAsync<T, R>(Task<Option<T>> self, Func<T, Task<R>> map) =>
            self.MapAsync(map);

        public static Task<Option<R>> mapAsync<T, R>(Task<Option<T>> self, Func<T, R> map) =>
            self.MapAsync(map);

        public static Task<Option<R>> mapAsync<T, R>(Option<Task<T>> self, Func<T, R> map) =>
            self.MapAsync(map);

        public static Task<Option<R>> mapAsync<T, R>(Option<Task<T>> self, Func<T, Task<R>> map) =>
            self.MapAsync(map);

        public static Task<Option<R>> bindAsync<T, R>(Option<T> self, Func<T, Task<Option<R>>> bind) =>
            self.BindAsync(bind);

        public static Task<Option<R>> bindAsync<T, R>(Task<Option<T>> self, Func<T, Task<Option<R>>> bind) =>
            self.BindAsync(bind);

        public static Task<Option<R>> bindAsync<T, R>(Task<Option<T>> self, Func<T, Option<R>> bind) =>
            self.BindAsync(bind);

        public static Task<Option<R>> bindAsync<T, R>(Option<Task<T>> self, Func<T, Option<R>> bind) =>
            self.BindAsync(bind);

        public static Task<Option<R>> bindAsync<T, R>(Option<Task<T>> self, Func<T, Task<Option<R>>> bind) =>
            self.BindAsync(bind);

        public static Task<Unit> iterAsync<T>(Task<Option<T>> self, Action<T> action) =>
            self.IterAsync(action);

        public static Task<Unit> iterAsync<T>(Option<Task<T>> self, Action<T> action) =>
            self.IterAsync(action);

        public static Task<int> countAsync<T>(Task<Option<T>> self) =>
            self.CountAsync();

        public static Task<int> sumAsync(Task<Option<int>> self) =>
            self.SumAsync();

        public static Task<int> sumAsync(Option<Task<int>> self) =>
            self.SumAsync();

        public static Task<S> foldAsync<T, S>(Task<Option<T>> self, S state, Func<S, T, S> folder) =>
            self.FoldAsync(state, folder);

        public static Task<S> foldAsync<T, S>(Option<Task<T>> self, S state, Func<S, T, S> folder) =>
            self.FoldAsync(state, folder);

        public static Task<bool> forallAsync<T>(Task<Option<T>> self, Func<T, bool> pred) =>
            self.ForAllAsync(pred);

        public static Task<bool> forallAsync<T>(Option<Task<T>> self, Func<T, bool> pred) =>
            self.ForAllAsync(pred);

        public static Task<bool> existsAsync<T>(Task<Option<T>> self, Func<T, bool> pred) =>
            self.ExistsAsync(pred);

        public static Task<bool> existsAsync<T>(Option<Task<T>> self, Func<T, bool> pred) =>
            self.ExistsAsync(pred);

    }
}
