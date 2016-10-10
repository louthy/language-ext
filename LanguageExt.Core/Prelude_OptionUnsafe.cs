using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        public static OptionUnsafe<T> append<T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
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
        public static OptionUnsafe<T> subtract<T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
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
        public static OptionUnsafe<T> multiply<T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
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
        public static OptionUnsafe<T> divide<T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Divide(rhs);

        [Pure]
        public static bool isSome<T>(OptionUnsafe<T> value) =>
            value.IsSome;

        [Pure]
        public static bool isNone<T>(OptionUnsafe<T> value) =>
            value.IsNone;

        /// <summary>
        /// Create a Some of T (OptionUnsafe<T>).  Use the to wrap any-type without coercian.
        /// That means you can wrap null, Nullable<T>, or Option<T> to get Option<Option<T>>
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to make optional</param>
        /// <returns>Option<T> in a Some state</returns>
        [Pure]
        public static OptionUnsafe<T> SomeUnsafe<T>(T value) =>
            OptionUnsafe<T>.Some(value);

        public static Unit ifSomeUnsafe<T>(OptionUnsafe<T> option, Action<T> Some) =>
            option.IfSomeUnsafe(Some);

        [Pure]
        public static T ifNoneUnsafe<T>(OptionUnsafe<T> option, Func<T> None) =>
            option.IfNoneUnsafe(None);

        [Pure]
        public static T ifNoneUnsafe<T>(OptionUnsafe<T> option, T noneValue) =>
            option.IfNoneUnsafe(noneValue);

        [Pure]
        public static R matchUnsafe<T, R>(OptionUnsafe<T> option, Func<T, R> Some, Func<R> None) =>
            option.MatchUnsafe(Some, None);

        public static Unit matchUnsafe<T>(OptionUnsafe<T> option, Action<T> Some, Action None) =>
            option.MatchUnsafe(Some, None);

        /// <summary>
        /// Folds the option into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="option">Option to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(OptionUnsafe<T> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        [Pure]
        public static bool forall<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        [Pure]
        public static int count<T>(OptionUnsafe<T> option) =>
            option.Count();

        [Pure]
        public static bool exists<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.Exists(pred);

        [Pure]
        public static OptionUnsafe<R> map<T, R>(OptionUnsafe<T> option, Func<T, R> mapper) =>
            option.Map(mapper);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static OptionUnsafe<Func<T2, R>> parmap<T1, T2, R>(OptionUnsafe<T1> option, Func<T1, T2, R> mapper) =>
            option.ParMap(mapper);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static OptionUnsafe<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(OptionUnsafe<T1> option, Func<T1, T2, T3, R> mapper) =>
            option.ParMap(mapper);

        [Pure]
        public static OptionUnsafe<T> filter<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.Filter(pred);

        [Pure]
        public static OptionUnsafe<R> bind<T, R>(OptionUnsafe<T> option, Func<T, OptionUnsafe<R>> binder) =>
            option.Bind(binder);

        [Pure]
        public static IEnumerable<R> matchUnsafe<T, R>(IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            list.Match(
                None,
                opt => opt.SomeUnsafe(v => Some(v)).None(None),
                (x, xs) => x.SomeUnsafe(v => Some(v)).None(None).Concat(matchUnsafe(xs, Some, None)) // TODO: Flatten recursion
            );

        [Pure]
        public static IEnumerable<R> matchUnsafe<T, R>(IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            matchUnsafe(list, Some, () => None);

        /// <summary>
        /// Apply an Optional value to an Optional function
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg">Optional argument</param>
        /// <returns>Returns the result of applying the optional argument to the optional function</returns>
        [Pure]
        public static OptionUnsafe<R> apply<T, R>(OptionUnsafe<Func<T, R>> option, OptionUnsafe<T> arg) =>
            option.Apply(arg);

        /// <summary>
        /// Apply an Optional value to an Optional function of arity 2
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg">Optional argument</param>
        /// <returns>Returns the result of applying the optional argument to the optional function:
        /// an optonal function of arity 1</returns>
        [Pure]
        public static OptionUnsafe<Func<T2, R>> apply<T1, T2, R>(OptionUnsafe<Func<T1, T2, R>> option, OptionUnsafe<T1> arg) =>
            option.Apply(arg);

        /// <summary>
        /// Apply Optional values to an Optional function of arity 2
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg1">Optional argument</param>
        /// <param name="arg2">Optional argument</param>
        /// <returns>Returns the result of applying the optional arguments to the optional function</returns>
        [Pure]
        public static OptionUnsafe<R> apply<T1, T2, R>(OptionUnsafe<Func<T1, T2, R>> option, OptionUnsafe<T1> arg1, OptionUnsafe<T2> arg2) =>
            option.Apply(arg1, arg2);

        [Pure]
        public static IEnumerable<R> MatchUnsafe<T, R>(this IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            matchUnsafe(list, Some, None);

        [Pure]
        public static IEnumerable<R> MatchUnsafe<T, R>(this IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            matchUnsafe(list, Some, () => None);

        [Pure]
        public static Lst<T> toList<T>(OptionUnsafe<T> option) =>
            option.ToList();

        [Pure]
        public static T[] toArray<T>(OptionUnsafe<T> option) =>
            option.ToArray();

        [Pure]
        public static IQueryable<T> toQuery<T>(OptionUnsafe<T> option) =>
            option.AsEnumerable().AsQueryable();

        /// <summary>
        /// Extracts from a list of 'Option' all the 'Some' elements.
        /// All the 'Some' elements are extracted in order.
        /// </summary>
        [Pure]
        public static IEnumerable<T> somesUnsafe<T>(IEnumerable<OptionUnsafe<T>> list) =>
            list.Somes();

        public static Task<OptionUnsafe<R>> mapAsync<T, R>(OptionUnsafe<T> self, Func<T, Task<R>> map) =>
            self.MapAsync(map);

        public static Task<OptionUnsafe<R>> mapAsync<T, R>(Task<OptionUnsafe<T>> self, Func<T, Task<R>> map) =>
            self.MapAsync(map);

        public static Task<OptionUnsafe<R>> mapAsync<T, R>(Task<OptionUnsafe<T>> self, Func<T, R> map) =>
            self.MapAsync(map);

        public static Task<OptionUnsafe<R>> mapAsync<T, R>(OptionUnsafe<Task<T>> self, Func<T, R> map) =>
            self.MapAsync(map);

        public static Task<OptionUnsafe<R>> mapAsync<T, R>(OptionUnsafe<Task<T>> self, Func<T, Task<R>> map) =>
            self.MapAsync(map);

        public static Task<OptionUnsafe<R>> bindAsync<T, R>(OptionUnsafe<T> self, Func<T, Task<OptionUnsafe<R>>> bind) =>
            self.BindAsync(bind);

        public static Task<OptionUnsafe<R>> bindAsync<T, R>(Task<OptionUnsafe<T>> self, Func<T, Task<OptionUnsafe<R>>> bind) =>
            self.BindAsync(bind);

        public static Task<OptionUnsafe<R>> bindAsync<T, R>(Task<OptionUnsafe<T>> self, Func<T, OptionUnsafe<R>> bind) =>
            self.BindAsync(bind);

        public static Task<OptionUnsafe<R>> bindAsync<T, R>(OptionUnsafe<Task<T>> self, Func<T, OptionUnsafe<R>> bind) =>
            self.BindAsync(bind);

        public static Task<OptionUnsafe<R>> bindAsync<T, R>(OptionUnsafe<Task<T>> self, Func<T, Task<OptionUnsafe<R>>> bind) =>
            self.BindAsync(bind);

        public static Task<Unit> iterAsync<T>(Task<OptionUnsafe<T>> self, Action<T> action) =>
            self.IterAsync(action);

        public static Task<Unit> iterAsync<T>(OptionUnsafe<Task<T>> self, Action<T> action) =>
            self.IterAsync(action);

        public static Task<int> countAsync<T>(Task<OptionUnsafe<T>> self) =>
            self.CountAsync();

        public static Task<int> sumAsync(Task<OptionUnsafe<int>> self) =>
            self.SumAsync();

        public static Task<int> sumAsync(OptionUnsafe<Task<int>> self) =>
            self.SumAsync();

        public static Task<S> foldAsync<T, S>(Task<OptionUnsafe<T>> self, S state, Func<S, T, S> folder) =>
            self.FoldAsync(state, folder);

        public static Task<S> foldAsync<T, S>(OptionUnsafe<Task<T>> self, S state, Func<S, T, S> folder) =>
            self.FoldAsync(state, folder);

        public static Task<bool> forallAsync<T>(Task<OptionUnsafe<T>> self, Func<T, bool> pred) =>
            self.ForAllAsync(pred);

        public static Task<bool> forallAsync<T>(OptionUnsafe<Task<T>> self, Func<T, bool> pred) =>
            self.ForAllAsync(pred);

        public static Task<bool> existsAsync<T>(Task<OptionUnsafe<T>> self, Func<T, bool> pred) =>
            self.ExistsAsync(pred);

        public static Task<bool> existsAsync<T>(OptionUnsafe<Task<T>> self, Func<T, bool> pred) =>
            self.ExistsAsync(pred);

    }
}
