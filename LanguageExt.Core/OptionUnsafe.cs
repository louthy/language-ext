using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// OptionUnsafe T can be in two states:
    ///     1. Some(x) -- which means there is a value stored inside
    ///     2. None    -- which means there's no value stored inside
    /// To extract the value you must use the 'matchUnsafe' function.
    /// This differs from Option T  in that it allows Some(null) which
    /// is expressly forbidden for Option T.  That is what makes this
    /// type 'unsafe'.  
    /// </summary>
#if !COREFX
    [TypeConverter(typeof(OptionalTypeConverter))]
    [Serializable]
#endif
    public struct OptionUnsafe<T> : 
        IOptional, 
        IComparable<OptionUnsafe<T>>, 
        IComparable<T>, 
        IEquatable<OptionUnsafe<T>>, 
        IEquatable<T>,
        IAppendable<OptionUnsafe<T>>,
        ISubtractable<OptionUnsafe<T>>,
        IMultiplicable<OptionUnsafe<T>>,
        IDivisible<OptionUnsafe<T>>
    {
        readonly T value;

        private OptionUnsafe(T value, bool isSome)
        {
            this.IsSome = isSome;
            this.value = value;
        }

        private OptionUnsafe(T value)
        {
            this.IsSome = true;
            this.value = value;
        }

        [Pure]
        public static OptionUnsafe<T> Some(T value) =>
            new OptionUnsafe<T>(value, true);

        public static readonly OptionUnsafe<T> None = new OptionUnsafe<T>();

        [Pure]
        public bool IsSome { get; }

        [Pure]
        public bool IsNone =>
            !IsSome;

        [Pure]
        internal T Value =>
            IsSome
                ? value
                : raise<T>(new OptionIsNoneException());

        [Pure]
        public static implicit operator OptionUnsafe<T>(T value) =>
            Some(value);

        [Pure]
        public static implicit operator OptionUnsafe<T>(OptionNone none) =>
            None;

        [Pure]
        public R MatchUnsafe<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Match the two states of the Option
        /// The Some can return a Task R and the None an R.  The result is wrapped in a Task R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an R</returns>
        public async Task<R> MatchAsyncUnsafe<R>(Func<T, Task<R>> Some, Func<R> None) =>
            IsSome
                ? await Some(Value)
                : None();

        /// <summary>
        /// Match the two states of the Option
        /// The Some and None can return a Task R and the None an R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an R</returns>
        public async Task<R> MatchAsyncUnsafe<R>(Func<T, Task<R>> Some, Func<Task<R>> None) =>
            await (IsSome
                ? Some(Value)
                : None());

        /// <summary>
        /// Match the two states of the Option
        /// The Some can return an IObservable R and the None an R.  The result is wrapped in an IObservable R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an stream of Rs</returns>
        [Pure]
        public IObservable<R> MatchObservableUnsafe<R>(Func<T, IObservable<R>> Some, Func<R> None) =>
            IsSome
                ? Some(Value)
                : Observable.Return(None());

        /// <summary>
        /// Match the two states of the Option
        /// The Some and None can return an IObservable R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an stream of Rs</returns>
        [Pure]
        public IObservable<R> MatchObservableUnsafe<R>(Func<T, IObservable<R>> Some, Func<IObservable<R>> None) =>
            IsSome
                ? Some(Value)
                : None();

        [Pure]
        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            IsSome
                ? Some(Value)
                : None();

        public Unit MatchUnsafe(Action<T> Some, Action None)
        {
            if (IsSome)
            {
                Some(Value);
            }
            else
            {
                None();
            }
            return Unit.Default;
        }

       
        /// <summary>
        /// Invokes the someHandler if OptionUnsafe is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSomeUnsafe(Func<T, Unit> someHandler)
        {
            if (IsSome)
            {
                someHandler(value);
            }
            return unit;
        }

        /// <summary>
        /// Invokes the someHandler if OptionUnsafe is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSomeUnsafe(Action<T> someHandler)
        {
            if (IsSome)
            {
                someHandler(value);
            }
            return unit;
        }

        [Pure]
        public T IfNoneUnsafe(Func<T> None) =>
            MatchUnsafe(identity, None);

        [Pure]
        public T IfNoneUnsafe(T noneValue) =>
            MatchUnsafe(identity, () => noneValue);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'FailureUnsafe' has been deprecated.  Please use 'IfNoneUnsafe' instead")]
        public T FailureUnsafe(Func<T> None) =>
            MatchUnsafe(identity, None);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'FailureUnsafe' has been deprecated.  Please use 'IfNoneUnsafe' instead")]
        public T FailureUnsafe(T noneValue) =>
            MatchUnsafe(identity, () => noneValue);

        [Pure]
        public SomeUnsafeUnitContext<T> SomeUnsafe<R>(Action<T> someHandler) =>
            new SomeUnsafeUnitContext<T>(this, someHandler);

        [Pure]
        public SomeUnsafeContext<T, R> SomeUnsafe<R>(Func<T, R> someHandler) =>
            new SomeUnsafeContext<T, R>(this, someHandler);

        [Pure]
        public override string ToString() =>
            IsSome
                ? isnull(Value)
                    ? "Some(null)"
                    : $"Some({Value})"
                : "None";

        [Pure]
        public override int GetHashCode() =>
            IsSome && notnull(Value)
                ? Value.GetHashCode()
                : 0;

        [Pure]
        public override bool Equals(object obj) =>
            obj is OptionUnsafe<T>
                ? map(this, (OptionUnsafe<T>)obj, (lhs, rhs) =>
                    lhs.IsNone && rhs.IsNone
                        ? true
                        : lhs.IsNone || rhs.IsNone
                            ? false
                            : isnull(lhs.Value)
                                ? isnull(rhs.Value)
                                : lhs.Value.Equals(rhs.Value))
                : IsSome
                    ? Value.Equals(obj)
                    : false;

        [Pure]
        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
            {
                yield return Value;
            }
        }

        [Pure]
        public EitherUnsafe<L, T> ToEitherUnsafe<L>(L defaultLeftValue) =>
            IsSome
                ? RightUnsafe<L, T>(Value)
                : LeftUnsafe<L, T>(defaultLeftValue);

        [Pure]
        public EitherUnsafe<L, T> ToEitherUnsafe<L>(Func<L> Left) =>
            IsSome
                ? RightUnsafe<L, T>(Value)
                : LeftUnsafe<L, T>(Left());

        [Pure]
        public Lst<T> ToList() =>
            Prelude.toList(AsEnumerable());

        [Pure]
        public T[] ToArray() =>
            Prelude.toArray(AsEnumerable());

        [Pure]
        public static bool operator ==(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            !lhs.Equals(rhs);

        [Pure]
        public static OptionUnsafe<T> operator |(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.IsSome
                ? lhs
                : rhs;

        [Pure]
        public static bool operator true(OptionUnsafe<T> value) =>
            value.IsSome;

        [Pure]
        public static bool operator false(OptionUnsafe<T> value) =>
            value.IsNone;

        [Pure]
        public Type GetUnderlyingType() =>
            typeof(T);

        [Pure]
        public int CompareTo(OptionUnsafe<T> other) =>
            IsNone && other.IsNone
                ? 0
                : IsSome && other.IsSome
                    ? Comparer<T>.Default.Compare(Value, other.Value)
                    : IsSome
                        ? -1
                        : 1;

        [Pure]
        public int CompareTo(T other) =>
            IsNone
                ? -1
                : Comparer<T>.Default.Compare(Value, other);

        [Pure]
        public bool Equals(T other) =>
            IsNone
                ? false
                : EqualityComparer<T>.Default.Equals(Value, other);

        [Pure]
        public bool Equals(OptionUnsafe<T> other) =>
            IsNone && other.IsNone
                ? true
                : IsSome && other.IsSome
                    ? EqualityComparer<T>.Default.Equals(Value, other.Value)
                    : false;

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
        public static OptionUnsafe<T> operator +(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Append(rhs);

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
        public OptionUnsafe<T> Append(OptionUnsafe<T> rhs)
        {
            if (IsNone && rhs.IsNone) return this;  // None  + None  = None
            if (rhs.IsNone) return this;            // Value + None  = Value
            if (this.IsNone) return rhs;            // None  + Value = Value
            return TypeDesc.Append(Value, rhs.Value, TypeDesc<T>.Default);
        }

        /// <summary>
        /// Subtract the Some(x) of one option from the Some(y) of another.
        /// For numeric values the behaviour is to find the difference between the Somes (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the T type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static OptionUnsafe<T> operator -(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Subtract the Some(x) of one option from the Some(y) of another.
        /// For numeric values the behaviour is to find the difference between the Somes (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the T type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public OptionUnsafe<T> Subtract(OptionUnsafe<T> rhs)
        {
            var self = IsNone
                ? TypeDesc<T>.Default.HasZero
                    ? Some(TypeDesc<T>.Default.Zero<T>())
                    : this
                : this;
            if (self.IsNone) return this;  // zero - rhs = undefined (when HasZero == false)
            if (rhs.IsNone) return this;   // lhs + zero = lhs
            return TypeDesc.Subtract(self.Value, rhs.Value, TypeDesc<T>.Default);
        }

        /// <summary>
        /// Find the product of the Somes.
        /// For numeric values the behaviour is to multiply the Somes (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the T type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public static OptionUnsafe<T> operator *(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Multiply(rhs);

        /// <summary>
        /// Find the product of the Somes.
        /// For numeric values the behaviour is to multiply the Somes (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the T type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public OptionUnsafe<T> Multiply(OptionUnsafe<T> rhs)
        {
            if (IsNone) return this;      // zero * rhs = zero
            if (rhs.IsNone) return rhs;   // lhs * zero = zero
            return TypeDesc.Multiply(Value, rhs.Value, TypeDesc<T>.Default);
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
        public static OptionUnsafe<T> operator /(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Divide(rhs);

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
        public OptionUnsafe<T> Divide(OptionUnsafe<T> rhs)
        {
            if (IsNone) return this;      // zero / rhs  = zero
            if (rhs.IsNone) return rhs;   // lhs  / zero = undefined: zero
            return TypeDesc.Divide(Value, rhs.Value, TypeDesc<T>.Default);
        }
    }

    public struct SomeUnsafeContext<T, R>
    {
        readonly OptionUnsafe<T> option;
        readonly Func<T, R> someHandler;

        internal SomeUnsafeContext(OptionUnsafe<T> option, Func<T, R> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        [Pure]
        public R None(Func<R> noneHandler) =>
            matchUnsafe(option, someHandler, noneHandler);

        [Pure]
        public R None(R noneValue) =>
            matchUnsafe(option, someHandler, () => noneValue);
    }

    public struct SomeUnsafeUnitContext<T>
    {
        readonly OptionUnsafe<T> option;
        readonly Action<T> someHandler;

        internal SomeUnsafeUnitContext(OptionUnsafe<T> option, Action<T> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public Unit None(Action noneHandler) =>
            matchUnsafe(option, someHandler, noneHandler);
    }

    internal static class OptionUnsafeCast
    {
        [Pure]
        public static OptionUnsafe<T> Cast<T>(T value) =>
            isnull(value)
                ? OptionUnsafe<T>.None
                : OptionUnsafe<T>.Some(value);

        [Pure]
        public static OptionUnsafe<T> Cast<T>(T? value) where T : struct =>
            isnull(value)
                ? OptionUnsafe<T>.None
                : OptionUnsafe<T>.Some(value.Value);
    }
}

public static class __OptionUnsafeExt
{
    /// <summary>
    /// Extracts from a list of 'Option' all the 'Some' elements.
    /// All the 'Some' elements are extracted in order.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Somes<T>(this IEnumerable<OptionUnsafe<T>> self)
    {
        foreach (var item in self)
        {
            if (item.IsSome)
            {
                yield return item.Value;
            }
        }
    }

    /// <summary>
    /// Apply an Optional value to an Optional function
    /// </summary>
    /// <param name="opt">Optional function</param>
    /// <param name="arg">Optional argument</param>
    /// <returns>Returns the result of applying the optional argument to the optional function</returns>
    [Pure]
    public static OptionUnsafe<R> Apply<T, R>(this OptionUnsafe<Func<T, R>> opt, OptionUnsafe<T> arg) =>
        opt.IsSome && arg.IsSome
            ? SomeUnsafe(opt.Value(arg.Value))
            : None;

    /// <summary>
    /// Apply an Optional value to an Optional function of arity 2
    /// </summary>
    /// <param name="opt">Optional function</param>
    /// <param name="arg">Optional argument</param>
    /// <returns>Returns the result of applying the optional argument to the optional function:
    /// an optonal function of arity 1</returns>
    [Pure]
    public static OptionUnsafe<Func<T2, R>> Apply<T1, T2, R>(this OptionUnsafe<Func<T1, T2, R>> opt, OptionUnsafe<T1> arg) =>
        opt.IsSome && arg.IsSome
            ? SomeUnsafe(par(opt.Value,arg.Value))
            : None;

    /// <summary>
    /// Apply Optional values to an Optional function of arity 2
    /// </summary>
    /// <param name="opt">Optional function</param>
    /// <param name="arg1">Optional argument</param>
    /// <param name="arg2">Optional argument</param>
    /// <returns>Returns the result of applying the optional arguments to the optional function</returns>
    [Pure]
    public static OptionUnsafe<R> Apply<T1, T2, R>(this OptionUnsafe<Func<T1, T2, R>> opt, OptionUnsafe<T1> arg1, OptionUnsafe<T2> arg2) =>
        opt.IsSome && arg1.IsSome && arg2.IsSome
            ? SomeUnsafe(opt.Value(arg1.Value, arg2.Value))
            : None;

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static OptionUnsafe<U> Select<T, U>(this OptionUnsafe<T> self, Func<T, U> map) => 
        self.Map(map);

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static OptionUnsafe<T> Where<T>(this OptionUnsafe<T> self, Func<T, bool> pred) =>
        self.Filter(pred)
            ? self
            : None;

    public static Unit Iter<T>(this OptionUnsafe<T> self, Action<T> action) =>
        self.IfSomeUnsafe(action);

    [Pure]
    public static int Count<T>(this OptionUnsafe<T> self) =>
        self.IsSome
            ? 1
            : 0;

    [Pure]
    public static bool ForAll<T>(this OptionUnsafe<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : true;

    [Pure]
    public static bool Exists<T>(this OptionUnsafe<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : false;

    [Pure]
    public static S Fold<S, T>(this OptionUnsafe<T> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? folder(state, self.Value)
            : state;

    [Pure]
    public static OptionUnsafe<R> Map<T, R>(this OptionUnsafe<T> self, Func<T, R> mapper) =>
        self.IsSome
            ? OptionUnsafeCast.Cast(mapper(self.Value))
            : None;

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static OptionUnsafe<Func<T2, R>> ParMap<T1, T2, R>(this OptionUnsafe<T1> opt, Func<T1, T2, R> func) =>
        opt.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static OptionUnsafe<Func<T2, Func<T3, R>>> ParMap<T1, T2, T3, R>(this OptionUnsafe<T1> opt, Func<T1, T2, T3, R> func) =>
        opt.Map(curry(func));

    [Pure]
    public static OptionUnsafe<T> Filter<T>(this OptionUnsafe<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
                ? self
                : None
            : self;

    [Pure]
    public static OptionUnsafe<R> Bind<T, R>(this OptionUnsafe<T> self, Func<T, OptionUnsafe<R>> binder) =>
        self.IsSome
            ? binder(self.Value)
            : None;

    [Pure]
    public static int Sum(this OptionUnsafe<int> self) =>
        self.IsSome
            ? self.Value
            : 0;

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static OptionUnsafe<V> SelectMany<T, U, V>(this OptionUnsafe<T> self,
        Func<T, OptionUnsafe<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        var resU = bind(self.Value);
        if (resU.IsNone) return None;
        return project(self.Value, resU.Value);
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<V> SelectMany<T, U, V>(this OptionUnsafe<T> self,
        Func<T, IEnumerable<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return new V[0];
        return bind(self.Value).Map(resU => project(self.Value, resU));
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static OptionUnsafe<V> SelectMany<T, U, V>(this IEnumerable<T> self,
        Func<T, OptionUnsafe<U>> bind,
        Func<T, U, V> project
        )
    {
        var ta = self.Take(1).ToArray();
        if (ta.Length == 0) return None;
        var resU = bind(ta[0]);
        if (resU.IsNone) return None;
        return project(ta[0], resU.Value);
    }

    /// <summary>
    /// Match the two states of the OptionUnsafe&lt;Task&lt;T&gt;&gt;
    /// 
    ///     If Some then the result of the Task is passed to Some and returned as a Task R.
    ///     If None then the result of None() is returned as a Task R
    ///     
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A promise to return an R</returns>
    public static async Task<R> MatchAsync<T, R>(this OptionUnsafe<Task<T>> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? Some(await self.Value)
            : None();

    /// <summary>
    /// Match the two states of the OptionUnsafe&lt;IObservable&lt;T&gt;&gt;
    /// 
    ///     If Some then the observable stream is mapped with Some (until the subscription ends)
    ///     If None the a single value observable is returned with the None result in
    /// 
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A stream of Rs</returns>
    [Pure]
    public static IObservable<R> MatchObservable<T, R>(this OptionUnsafe<IObservable<T>> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? self.Value.Select(Some)
            : Observable.Return(None());

    /// <summary>
    /// Match the two states of the IObservable&lt;OptionUnsafe&lt;T&gt;&gt;
    /// 
    ///     Matches a stream of options
    /// 
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A stream of Rs</returns>
    [Pure]
    public static IObservable<R> MatchObservable<T, R>(this IObservable<OptionUnsafe<T>> self, Func<T, R> Some, Func<R> None) =>
        self.Select(opt => matchUnsafe(opt, Some, None));


    public static async Task<OptionUnsafe<R>> MapAsync<T, R>(this OptionUnsafe<T> self, Func<T, Task<R>> map) =>
        self.IsSome
            ? SomeUnsafe(await map(self.Value))
            : None;

    public static async Task<OptionUnsafe<R>> MapAsync<T, R>(this Task<OptionUnsafe<T>> self, Func<T, Task<R>> map)
    {
        var val = await self;
        return val.IsSome
            ? SomeUnsafe(await map(val.Value))
            : None;
    }

    public static async Task<OptionUnsafe<R>> MapAsync<T, R>(this Task<OptionUnsafe<T>> self, Func<T, R> map)
    {
        var val = await self;
        return val.IsSome
            ? SomeUnsafe(map(val.Value))
            : None;
    }

    public static async Task<OptionUnsafe<R>> MapAsync<T, R>(this OptionUnsafe<Task<T>> self, Func<T, R> map) =>
        self.IsSome
            ? SomeUnsafe(map(await self.Value))
            : None;

    public static async Task<OptionUnsafe<R>> MapAsync<T, R>(this OptionUnsafe<Task<T>> self, Func<T, Task<R>> map) =>
        self.IsSome
            ? SomeUnsafe(await map(await self.Value))
            : None;


    public static async Task<OptionUnsafe<R>> BindAsync<T, R>(this OptionUnsafe<T> self, Func<T, Task<OptionUnsafe<R>>> bind) =>
        self.IsSome
            ? await bind(self.Value)
            : None;

    public static async Task<OptionUnsafe<R>> BindAsync<T, R>(this Task<OptionUnsafe<T>> self, Func<T, Task<OptionUnsafe<R>>> bind)
    {
        var val = await self;
        return val.IsSome
            ? await bind(val.Value)
            : None;
    }

    public static async Task<OptionUnsafe<R>> BindAsync<T, R>(this Task<OptionUnsafe<T>> self, Func<T, OptionUnsafe<R>> bind)
    {
        var val = await self;
        return val.IsSome
            ? bind(val.Value)
            : None;
    }

    public static async Task<OptionUnsafe<R>> BindAsync<T, R>(this OptionUnsafe<Task<T>> self, Func<T, OptionUnsafe<R>> bind) =>
        self.IsSome
            ? bind(await self.Value)
            : None;

    public static async Task<OptionUnsafe<R>> BindAsync<T, R>(this OptionUnsafe<Task<T>> self, Func<T, Task<OptionUnsafe<R>>> bind) =>
        self.IsSome
            ? await bind(await self.Value)
            : None;

    public static async Task<Unit> IterAsync<T>(this Task<OptionUnsafe<T>> self, Action<T> action)
    {
        var val = await self;
        if (val.IsSome) action(val.Value);
        return unit;
    }

    public static async Task<Unit> IterAsync<T>(this OptionUnsafe<Task<T>> self, Action<T> action)
    {
        if (self.IsSome) action(await self.Value);
        return unit;
    }

    public static async Task<int> CountAsync<T>(this Task<OptionUnsafe<T>> self) =>
        (await self).Count();

    public static async Task<int> SumAsync(this Task<OptionUnsafe<int>> self) =>
        (await self).Sum();

    public static async Task<int> SumAsync(this OptionUnsafe<Task<int>> self) =>
        self.IsSome
            ? await self.Value
            : 0;

    public static async Task<S> FoldAsync<T, S>(this Task<OptionUnsafe<T>> self, S state, Func<S, T, S> folder) =>
        (await self).Fold(state, folder);

    public static async Task<S> FoldAsync<T, S>(this OptionUnsafe<Task<T>> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? folder(state, await self.Value)
            : state;

    public static async Task<bool> ForAllAsync<T>(this Task<OptionUnsafe<T>> self, Func<T, bool> pred) =>
        (await self).ForAll(pred);

    public static async Task<bool> ForAllAsync<T>(this OptionUnsafe<Task<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : true;

    public static async Task<bool> ExistsAsync<T>(this Task<OptionUnsafe<T>> self, Func<T, bool> pred) =>
        (await self).Exists(pred);

    public static async Task<bool> ExistsAsync<T>(this OptionUnsafe<Task<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : false;
}
