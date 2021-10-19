using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Convert NullableT to OptionT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">Value to convert</param>
        /// <returns>OptionT with Some or None, depending on HasValue</returns>
        [Pure]
        public static Option<T> toOption<T>(T? self) where T : struct =>
            self.HasValue
                ? Some(self.Value)
                : None;

        /// <summary>
        /// Match the two states of the Nullable and return a non-null R.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A non-null R</returns>
        [Pure]
        public static R match<T, R>(T? self, Func<T, R> Some, Func<R> None) where T : struct =>
            self.HasValue
                ? Some(self.Value)
                : None();

        /// <summary>
        /// Match the two states of the Nullable and return a promise for an R.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an R</returns>
        public static async Task<R> matchAsync<T, R>(T? self, Func<T, Task<R>> Some, Func<R> None) where T : struct =>
            self.HasValue
                ? await Some(self.Value)
                : None();

        /// <summary>
        /// Match the two states of the Nullable and return a promise for an R.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an R</returns>
        public static async Task<R> matchAsync<T, R>(T? self, Func<T, Task<R>> Some, Func<Task<R>> None) where T : struct =>
            self.HasValue
                ? await Some(self.Value)
                : await None();

        /// <summary>
        /// Match the two states of the Nullable T
        /// </summary>
        /// <param name="Some">Some match</param>
        /// <param name="None">None match</param>
        public static Unit match<T>(T? self, Action<T> Some, Action None) where T : struct
        {
            if (self.HasValue)
            {
                Some(self.Value);
            }
            else
            {
                None();
            }
            return Unit.Default;
        }

        /// <summary>
        /// Invokes the someHandler if Nullable is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public static Unit ifSome<T>(T? self, Action<T> someHandler) where T : struct
        {
            if (self.HasValue)
            {
                someHandler(self.Value);
            }
            return unit;
        }

        /// <summary>
        /// Invokes the someHandler if Nullable is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public static Unit ifSome<T>(T? self, Func<T, Unit> someHandler) where T : struct
        {
            if (self.HasValue)
            {
                someHandler(self.Value);
            }
            return unit;
        }

        [Pure]
        public static T ifNone<T>(T? self, Func<T> None) where T : struct =>
            self.Match(identity, None);

        [Pure]
        public static T ifNone<T>(T? self, T noneValue) where T : struct =>
            self.Match(identity, () => noneValue);

        [Pure]
        public static Either<L, T> toEither<L, T>(T? self, L defaultLeftValue) where T : struct =>
            self.HasValue
                ? Right<L, T>(self.Value)
                : Left<L, T>(defaultLeftValue);

        [Pure]
        public static Either<L, T> toEither<L, T>(T? self, Func<L> Left) where T : struct =>
            self.HasValue
                ? Right<L, T>(self.Value)
                : Left<L, T>(Left());

        [Pure]
        public static EitherUnsafe<L, T> toEitherUnsafe<L, T>(T? self, L defaultLeftValue) where T : struct =>
            self.HasValue
                ? RightUnsafe<L, T>(self.Value)
                : LeftUnsafe<L, T>(defaultLeftValue);

        [Pure]
        public static EitherUnsafe<L, T> toEitherUnsafe<L, T>(T? self, Func<L> Left) where T : struct =>
            self.HasValue
                ? RightUnsafe<L, T>(self.Value)
                : LeftUnsafe<L, T>(Left());

        [Pure]
        public static TryOption<T> toTryOption<L, T>(T? self, L defaultLeftValue) where T : struct =>
            () => Optional(self);

        /// <summary>
        /// Append the Some(x) of one option to the Some(y) of another.
        /// For numeric values the behaviour is to sum the Somes (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the T type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static T? append<SEMI, T>(T? lhs, T? rhs)
            where SEMI : struct, Semigroup<T>
            where T : struct
        {
            if (!lhs.HasValue && !rhs.HasValue) return lhs;  // None  + None  = None
            if (!rhs.HasValue) return lhs;                   // Value + None  = Value
            if (!lhs.HasValue) return rhs;                   // None  + Value = Value
            return default(SEMI).Append(lhs.Value, rhs.Value);
        }

        /// <summary>
        /// Sum the Some(x) of one nullable from the Some(y) of another.
        /// For numeric values the behaviour is to find the subtract between the Somes (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the T type derives from ISubtractable then the behaviour
        /// is to call lhs.Plus(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static T? plus<NUM, T>(T? lhs, T? rhs)
            where T : struct
            where NUM : Num<T>
        {
            if (!lhs.HasValue) return rhs;
            if (!rhs.HasValue) return lhs;
            return default(NUM).Plus(lhs.Value, rhs.Value);
        }

        /// <summary>
        /// Subtract the Some(x) of one nullable from the Some(y) of another.
        /// For numeric values the behaviour is to find the subtract between the Somes (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the T type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static T? subtract<NUM, T>(T? lhs, T? rhs)
            where T : struct
            where NUM : Num<T>
        {
            if (!lhs.HasValue) return rhs;
            if (!rhs.HasValue) return lhs;
            return default(NUM).Subtract(lhs.Value, rhs.Value);
        }

        /// <summary>
        /// Find the product of the Somes.
        /// For numeric values the behaviour is to multiply the Somes (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the T type derives from IMultiplicable then the behaviour
        /// is to call lhs.Product(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public static T? product<NUM, T>(T? lhs, T? rhs)
            where T : struct
            where NUM : Num<T>
        {
            if (!lhs.HasValue) return lhs;  // zero * rhs = zero
            if (!rhs.HasValue) return rhs;  // lhs * zero = zero
            return default(NUM).Product(lhs.Value, rhs.Value);
        }

        /// <summary>
        /// Divide the Somes.
        /// For numeric values the behaviour is to divide the Somes (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the T type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static T? divide<NUM, T>(T? lhs, T? rhs)
            where T : struct
            where NUM : Num<T>
        {
            if (!lhs.HasValue) return lhs;  // zero / rhs  = zero
            if (!rhs.HasValue) return rhs;  // lhs  / zero = undefined: zero
            return default(NUM).Divide(lhs.Value, rhs.Value);
        }


        /// <summary>
        /// Extracts from a list of 'Option' all the 'Some' elements.
        /// All the 'Some' elements are extracted in order.
        /// </summary>
        [Pure]
        public static IEnumerable<T> somes<T>(IEnumerable<T?> self) where T : struct
        {
            foreach (var item in self)
            {
                if (item.HasValue)
                {
                    yield return item.Value;
                }
            }
        }

        /// <summary>
        /// Iterate Nullable.  Imagine the item has zero or one items depending on whether
        /// it's in a None state or not.
        /// </summary>
        /// <param name="action">Action to invoke with the value if not in None state</param>
        public static Unit iter<T>(T? self, Action<T> action) where T : struct =>
            self.IfSome(action);

        /// <summary>
        /// Returns 1 if there is a value, 0 otherwise
        /// </summary>
        /// <returns>1 if there is a value, 0 otherwise</returns>
        [Pure]
        public static int count<T>(T? self) where T : struct =>
            self.HasValue
                ? 1
                : 0;

        /// <summary>
        /// ForAll Nullable.  Imagine the item has zero or one items depending on whether
        /// it's in a None state or not.  This function runs a predicate against the value
        /// if it exists, returns true if it doesn't (because the predicate holds 'for all'
        /// items).
        /// </summary>
        /// <param name="pred">Predicate</param>
        [Pure]
        public static bool forall<T>(T? self, Func<T, bool> pred) where T : struct =>
            self.HasValue
                ? pred(self.Value)
                : true;

        /// <summary>
        /// ForAll Nullable.  Imagine the item has zero or one items depending on whether
        /// it's in a None state or not.  This function runs a predicate against the value
        /// if it exists, returns true if it doesn't (because the predicate holds 'for all'
        /// items).
        /// </summary>
        /// <param name="Some">Some predicate</param>
        /// <param name="None">None predicate</param>
        [Pure]
        public static bool forall<T>(T? self, Func<T, bool> Some, Func<bool> None) where T : struct =>
            self.HasValue
                ? Some(self.Value)
                : None();

        /// <summary>
        /// Exists Nullable.  Imagine the item has zero or one items depending on whether
        /// it's in a None state or not.  This function runs a predicate against the value
        /// if it exists, returns false if it doesn't.
        /// </summary>
        /// <param name="pred">Predicate</param>
        [Pure]
        public static bool exists<T>(T? self, Func<T, bool> pred) where T : struct =>
            self.HasValue
                ? pred(self.Value)
                : false;

        /// <summary>
        /// Exists Nullable.  Imagine the item has zero or one items depending on whether
        /// it's in a None state or not.  This function runs a predicate against the value
        /// if it exists, returns false if it doesn't.
        /// </summary>
        /// <param name="Some">Some predicate</param>
        /// <param name="None">None predicate</param>
        [Pure]
        public static bool exists<T>(T? self, Func<T, bool> Some, Func<bool> None) where T : struct =>
            self.HasValue
                ? Some(self.Value)
                : None();

        /// <summary>
        /// Folds Nullable into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(T? self, S state, Func<S, T, S> folder) where T : struct =>
            self.HasValue
                ? folder(state, self.Value)
                : state;

        /// <summary>
        /// Folds Nullable into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Fold function for Some</param>
        /// <param name="None">Fold function for None</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(T? self, S state, Func<S, T, S> Some, Func<S, S> None) where T : struct =>
            self.HasValue
                ? Some(state, self.Value)
                : None(state);

        [Pure]
        public static R? map<T, R>(T? self, Func<T, R> mapper)
            where T : struct
            where R : struct =>
            self.HasValue
                ? mapper(self.Value)
                : default(R?);

        [Pure]
        public static R? map<T, R>(T? self, Func<T, R> Some, Func<R> None)
            where T : struct
            where R : struct =>
            self.HasValue
                ? Some(self.Value)
                : default(R?);

        [Pure]
        public static T? filter<T>(T? self, Func<T, bool> pred) where T : struct =>
            self.HasValue
                ? pred(self.Value)
                    ? self
                    : default(T?)
                : self;

        [Pure]
        public static T? filter<T>(T? self, Func<T, bool> Some, Func<bool> None) where T : struct =>
            self.HasValue
                ? Some(self.Value)
                    ? self
                    : default(T?)
                : None()
                    ? self
                    : default(T?);

        [Pure]
        public static R? bind<T, R>(T? self, Func<T, R?> binder)
            where T : struct
            where R : struct =>
            self.HasValue
                ? binder(self.Value)
                : default(R?);

        [Pure]
        public static R? bind<T, R>(T? self, Func<T, R?> Some, Func<R?> None)
            where T : struct
            where R : struct =>
            self.HasValue
                ? Some(self.Value)
                : None();

        [Pure]
        public static int sum(int? self) =>
            self ?? 0;
    }
}
