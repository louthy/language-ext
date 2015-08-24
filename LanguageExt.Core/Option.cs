using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.ComponentModel;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LanguageExt
{
    /// <summary>
    /// Option T can be in two states:
    ///     1. Some(x) -- which means there is a value stored inside
    ///     2. None    -- which means there's no value stored inside
    /// To extract the value you must use the 'match' function.
    /// </summary>
    [TypeConverter(typeof(OptionalTypeConverter))]
    public struct Option<T> : 
        IOptional, 
        IComparable<Option<T>>, 
        IComparable<T>, 
        IEquatable<Option<T>>, 
        IEquatable<T>
    {
        readonly T value;

        private Option(T value, bool isSome)
        {
            this.IsSome = isSome;
            this.value = value;
        }

        private Option(T value) 
            : this (value,value != null)
            {}

        public static Option<T> Some(T value) => 
            new Option<T>(value);

        public static readonly Option<T> None = new Option<T>();

        public bool IsSome { get; }

        public bool IsNone => 
            !IsSome;

        internal T Value =>
            IsSome
                ? value
                : raise<T>(new OptionIsNoneException());

        public static implicit operator Option<T>(T value) =>
            value == null
                ? Option<T>.None
                : Option<T>.Some(value);

        public static implicit operator Option<T>(OptionNone none) => 
            Option<T>.None;

        internal static U CheckNullReturn<U>(U value, string location) =>
            value == null
                ? raise<U>(new ResultIsNullException("'" + location + "' result is null.  Not allowed."))
                : value;

        internal static U CheckNullNoneReturn<U>(U value) =>
            CheckNullReturn(value, "None");

        internal static U CheckNullSomeReturn<U>(U value) =>
            CheckNullReturn(value, "Some");

        public R Match<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? CheckNullReturn(Some(Value), "Some")
                : CheckNullReturn(None(), "None");

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
        public async Task<R> MatchAsync<R>(Func<T, Task<R>> Some, Func<R> None) =>
            IsSome
                ? CheckNullReturn(await Some(Value), "Some")
                : CheckNullReturn(None(),"None");

        /// <summary>
        /// Match the two states of the Option
        /// The Some and None can return a Task R and the None an R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an R</returns>
        public async Task<R> MatchAsync<R>(Func<T, Task<R>> Some, Func<Task<R>> None) =>
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
        public IObservable<R> MatchObservable<R>(Func<T, IObservable<R>> Some, Func<R> None) =>
            IsSome
                ? Some(Value).Select(CheckNullSomeReturn)
                : Observable.Return(CheckNullReturn(None(),"None"));

        /// <summary>
        /// Match the two states of the Option
        /// The Some and None can return an IObservable R
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler</param>
        /// <param name="None">None handler</param>
        /// <returns>A promise to return an stream of Rs</returns>
        public IObservable<R> MatchObservable<R>(Func<T, IObservable<R>> Some, Func<IObservable<R>> None) =>
            IsSome
                ? Some(Value).Select(CheckNullSomeReturn)
                : None().Select(CheckNullNoneReturn);

        /// <summary>
        /// Untyped check
        /// </summary>
        /// <remarks>May also return null</remarks>
        /// <returns>R</returns>
        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            IsSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Match the two states of the Option T
        /// </summary>
        /// <param name="Some">Some match</param>
        /// <param name="None">None match</param>
        /// <returns></returns>
        public Unit Match(Action<T> Some, Action None)
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
        /// Invokes the someHandler if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSome(Action<T> someHandler)
        {
            if (IsSome)
            {
                someHandler(value);
            }
            return unit;
        }

        /// <summary>
        /// Invokes the someHandler if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSome(Func<T,Unit> someHandler)
        {
            if (IsSome)
            {
                someHandler(value);
            }
            return unit;
        }

        public T IfNone(Func<T> None) =>
            Match(identity, None);

        public T IfNone(T noneValue) =>
            Match(identity, () => noneValue);

        public T IfNoneUnsafe(Func<T> None) =>
            MatchUnsafe(identity, None);

        public T IfNoneUnsafe(T noneValue) =>
            MatchUnsafe(identity, () => noneValue);

        [Obsolete("'Failure' has been deprecated.  Please use 'IfNone' instead")]
        public T Failure(Func<T> None) => 
            Match(identity, None);

        [Obsolete("'Failure' has been deprecated.  Please use 'IfNone' instead")]
        public T Failure(T noneValue) => 
            Match(identity, () => noneValue);

        public SomeUnitContext<T> Some<R>(Action<T> someHandler) =>
            new SomeUnitContext<T>(this, someHandler);

        public SomeContext<T, R> Some<R>(Func<T, R> someHandler) =>
            new SomeContext<T, R>(this,someHandler);

        public override string ToString() =>
            IsSome
                ? Value == null 
                    ? "Some(null)"
                    :  String.Format("Some({0})",Value)
                : "None";

        public override int GetHashCode() =>
            IsSome && Value != null
                ? Value.GetHashCode()
                : 0;

        public override bool Equals(object obj) =>
            obj is Option<T>
                ? map(this, (Option<T>)obj, (lhs, rhs) =>
                      lhs.IsNone && rhs.IsNone
                          ? true
                          : lhs.IsNone || rhs.IsNone
                              ? false
                              : lhs.Value.Equals(rhs.Value))
                : IsSome
                    ? Value.Equals(obj)
                    : false;

        public Lst<T> ToList() =>
            toList(AsEnumerable());

        public ImmutableArray<T> ToArray() =>
            toArray(AsEnumerable());

        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
            {
                yield return Value;
            }
        }

        public Either<L, T> ToEither<L>(L defaultLeftValue) =>
            IsSome
                ? Right<L, T>(Value)
                : Left<L, T>(defaultLeftValue);

        public Either<L, T> ToEither<L>(Func<L> Left) =>
            IsSome
                ? Right<L, T>(Value)
                : Left<L, T>(Left());

        public EitherUnsafe<L, T> ToEitherUnsafe<L>(L defaultLeftValue) =>
            IsSome
                ? RightUnsafe<L, T>(Value)
                : LeftUnsafe<L, T>(defaultLeftValue);

        public EitherUnsafe<L, T> ToEitherUnsafe<L>(Func<L> Left) =>
            IsSome
                ? RightUnsafe<L, T>(Value)
                : LeftUnsafe<L, T>(Left());

        public TryOption<T> ToTryOption<L>(L defaultLeftValue)
        {
            var self = this;
            return () => self;
        }

        public static bool operator ==(Option<T> lhs, Option<T> rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Option<T> lhs, Option<T> rhs) =>
            !lhs.Equals(rhs);

        public static Option<T> operator |(Option<T> lhs, Option<T> rhs) =>
            lhs.IsSome
                ? lhs
                : rhs;

        public static bool operator true(Option<T> value) =>
            value.IsSome;

        public static bool operator false(Option<T> value) =>
            value.IsNone;

        public Type GetUnderlyingType() =>
            typeof(T);

        public int CompareTo(Option<T> other) =>
            IsNone && other.IsNone
                ? 0
                : IsSome && other.IsSome
                    ? Comparer<T>.Default.Compare(Value, other.Value)
                    : IsSome
                        ? -1
                        : 1;

        public int CompareTo(T other) =>
            IsNone
                ? -1
                : Comparer<T>.Default.Compare(Value, other);

        public bool Equals(T other) =>
            IsNone
                ? false
                : EqualityComparer<T>.Default.Equals(Value, other);

        public bool Equals(Option<T> other) =>
            IsNone && other.IsNone
                ? true
                : IsSome && other.IsSome
                    ? EqualityComparer<T>.Default.Equals(Value, other.Value)
                    : false;
    }

    public struct SomeContext<T, R>
    {
        readonly Option<T> option;
        readonly Func<T, R> someHandler;

        internal SomeContext(Option<T> option, Func<T, R> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public R None(Func<R> noneHandler) =>
            match(option, someHandler, noneHandler);

        public R None(R noneValue) =>
            match(option, someHandler, () => noneValue);
    }

    public struct SomeUnitContext<T>
    {
        readonly Option<T> option;
        readonly Action<T> someHandler;

        internal SomeUnitContext(Option<T> option, Action<T> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public Unit None(Action noneHandler) =>
            match(option, someHandler, noneHandler);
    }

    public struct OptionNone
    {
        public static OptionNone Default = new OptionNone();
    }

    internal static class OptionCast
    {
        public static Option<T> Cast<T>(T value) =>
            value == null
                ? Option<T>.None
                : Option<T>.Some(value);


        public static Option<T> Cast<T>(Nullable<T> value) where T : struct =>
            value == null
                ? Option<T>.None
                : Option<T>.Some(value.Value);
    }
}

public static class __OptionExt
{
    public static T? ToNullable<T>(this Option<T> self) where T : struct =>
        self.IsNone
            ? (T?)null
            : new Nullable<T>(self.Value);

    // 
    // Option<T> extensions 
    // 
    public static Unit Iter<T>(this Option<T> self, Action<T> action) =>
        self.IfSome(action);

    public static int Count<T>(this Option<T> self) =>
        self.IsSome
            ? 1
            : 0;

    public static bool ForAll<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : true;

    public static bool Exists<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : false;

    public static S Fold<S, T>(this Option<T> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? folder(state, self.Value)
            : state;

    public static Option<R> Map<T, R>(this Option<T> self, Func<T, R> mapper) =>
        self.IsSome
            ? OptionCast.Cast(mapper(self.Value))
            : None;

    public static Option<T> Filter<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
                ? self
                : None
            : self;

    public static Option<R> Bind<T, R>(this Option<T> self, Func<T, Option<R>> binder) =>
        self.IsSome
            ? binder(self.Value)
            : None;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<U> Select<T, U>(this Option<T> self, Func<T, U> map) =>
        self.Map(map);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<T> Where<T>(this Option<T> self, Func<T, bool> pred) =>
        self.Filter(pred)
            ? self
            : None;

    public static int Sum(this Option<int> self) =>
        self.IsSome
            ? self.Value
            : 0;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<V> SelectMany<T, U, V>(this Option<T> self,
        Func<T, Option<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        var resU = bind(self.Value);
        if (resU.IsNone) return None;
        return Optional(project(self.Value, resU.Value));
    }

    /// <summary>
    /// Match the two states of the Option&lt;Task&lt;T&gt;&gt;
    /// 
    ///     If Some then the result of the Task is passed to Some and returned as a Task R.
    ///     If None then the result of None() is returned as a Task R
    ///     
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A promise to return an R</returns>
    public static async Task<R> MatchAsync<T, R>(this Option<Task<T>> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? Some(await self.Value)
            : None();

    /// <summary>
    /// Match the two states of the Option&lt;IObservable&lt;T&gt;&gt;
    /// 
    ///     If Some then the observable stream is mapped with Some (until the subscription ends)
    ///     If None the a single value observable is returned with the None result in
    /// 
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A stream of Rs</returns>
    public static IObservable<R> MatchObservable<T, R>(this Option<IObservable<T>> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? self.Value.Select(Some).Select(Option<R>.CheckNullSomeReturn)
            : Observable.Return(Option<R>.CheckNullReturn(None(), "None"));

    /// <summary>
    /// Match the two states of the IObservable&lt;Option&lt;T&gt;&gt;
    /// 
    ///     Matches a stream of options
    /// 
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler</param>
    /// <param name="None">None handler</param>
    /// <returns>A stream of Rs</returns>
    public static IObservable<R> MatchObservable<T, R>(this IObservable<Option<T>> self, Func<T, R> Some, Func<R> None) =>
        self.Select(opt => match(opt, Some, None));
}
