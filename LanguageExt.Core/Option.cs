using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClass;
using static LanguageExt.TypeClass.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Option T can be in two states:
    ///     1. Some(x) -- which means there is a value stored inside
    ///     2. None    -- which means there's no value stored inside
    /// To extract the value you must use the 'match' function.
    /// </summary>
#if !COREFX
    [TypeConverter(typeof(OptionalTypeConverter))]
    [Serializable]
#endif
    public struct Option<T> :
        IOptional,
        IComparable<Option<T>>,
        IComparable<T>,
        IEquatable<Option<T>>,
        IEquatable<T>,
        Monad<T>
    {
        readonly T value;

        private Option(T value)
        {
            if (isnull(value))
                throw new ValueIsNullException();

            this.value = value;
            this.IsSome = true;
        }

        /// <summary>
        /// Option Some(x) constructor
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Some(value) as Option T</returns>
        [Pure]
        public static Option<T> Some(T value) =>
            new Option<T>(value);

        /// <summary>
        /// Option None of T
        /// </summary>
        public static readonly Option<T> None =
            new Option<T>();

        /// <summary>
        /// true if the Option is in a Some(x) state
        /// </summary>
        [Pure]
        public bool IsSome { get; }

        /// <summary>
        /// true if the Option is in a None state
        /// </summary>
        [Pure]
        public bool IsNone =>
            !IsSome;

        [Pure]
        internal T Value =>
            IsSome
                ? value
                : raise<T>(new OptionIsNoneException());

        [Pure]
        public static implicit operator Option<T>(T value) =>
            isnull(value)
                ? None
                : Some(value);

        [Pure]
        public static implicit operator Option<T>(OptionNone none) =>
            None;

        [Pure]
        internal static U CheckNullReturn<U>(U value, string location) =>
            isnull(value)
                ? raise<U>(new ResultIsNullException($"'{location}' result is null.  Not allowed."))
                : value;

        [Pure]
        internal static U CheckNullNoneReturn<U>(U value) =>
            CheckNullReturn(value, "None");

        [Pure]
        internal static U CheckNullSomeReturn<U>(U value) =>
            CheckNullReturn(value, "Some");

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler.  Must not return null.</param>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A non-null R</returns>
        [Pure]
        public R Match<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? CheckNullSomeReturn(Some(Value))
                : CheckNullNoneReturn(None());

        /// <summary>
        /// Match the two states of the Option and return an R, which can be null.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler.  May return null.</param>
        /// <param name="None">None handler.  May return null.</param>
        /// <returns>R, or null</returns>
        [Pure]
        public R MatchUnsafe<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Match the two states of the Option and return a promise for a non-null R.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler.  Must not return null.</param>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A promise to return a non-null R</returns>
        public async Task<R> MatchAsync<R>(Func<T, Task<R>> Some, Func<R> None) =>
            IsSome
                ? CheckNullSomeReturn(await Some(Value))
                : CheckNullNoneReturn(None());

        /// <summary>
        /// Match the two states of the Option and return a promise for a non-null R.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler.  Must not return null.</param>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A promise to return a non-null R</returns>
        public async Task<R> MatchAsync<R>(Func<T, Task<R>> Some, Func<Task<R>> None) =>
            IsSome
                ? CheckNullSomeReturn(await Some(Value))
                : CheckNullNoneReturn(await None());

        /// <summary>
        /// Match the two states of the Option and return an observable stream of non-null Rs.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler.  Must not return null.</param>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A stream of non-null Rs</returns>
        [Pure]
        public IObservable<R> MatchObservable<R>(Func<T, IObservable<R>> Some, Func<R> None) =>
            IsSome
                ? Some(Value).Select(CheckNullSomeReturn)
                : Observable.Return(CheckNullNoneReturn(None()));

        /// <summary>
        /// Match the two states of the Option and return an observable stream of non-null Rs.
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Some handler.  Must not return null.</param>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A stream of non-null Rs</returns>
        [Pure]
        public IObservable<R> MatchObservable<R>(Func<T, IObservable<R>> Some, Func<IObservable<R>> None) =>
            IsSome
                ? Some(Value).Select(CheckNullSomeReturn)
                : None().Select(CheckNullNoneReturn);

        /// <summary>
        /// Match the two states of the Option and return an R, or null.
        /// </summary>
        /// <param name="Some">Some handler.  May return null.</param>
        /// <param name="None">None handler.  May return null.</param>
        /// <returns>An R, or null</returns>
        [Pure]
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
        public Unit IfSome(Func<T, Unit> someHandler)
        {
            if (IsSome)
            {
                someHandler(value);
            }
            return unit;
        }

        [Pure]
        public T IfNone(Func<T> None) =>
            Match(identity, None);

        [Pure]
        public T IfNone(T noneValue) =>
            Match(identity, () => noneValue);

        [Pure]
        public T IfNoneUnsafe(Func<T> None) =>
            MatchUnsafe(identity, None);

        [Pure]
        public T IfNoneUnsafe(T noneValue) =>
            MatchUnsafe(identity, () => noneValue);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'Failure' has been deprecated.  Please use 'IfNone' instead")]
        public T Failure(Func<T> None) =>
            Match(identity, None);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'Failure' has been deprecated.  Please use 'IfNone' instead")]
        public T Failure(T noneValue) =>
            Match(identity, () => noneValue);

        [Pure]
        public SomeUnitContext<T> Some(Action<T> someHandler) =>
            new SomeUnitContext<T>(this, someHandler);

        [Pure]
        public SomeContext<T, R> Some<R>(Func<T, R> someHandler) =>
            new SomeContext<T, R>(this, someHandler);

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

        [Pure]
        public Lst<T> ToList() =>
            toList(AsEnumerable());

        [Pure]
        public T[] ToArray() =>
            toArray(AsEnumerable());

        [Pure]
        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
            {
                yield return Value;
            }
        }

        [Pure]
        public Either<L, T> ToEither<L>(L defaultLeftValue) =>
            IsSome
                ? Right<L, T>(Value)
                : Left<L, T>(defaultLeftValue);

        [Pure]
        public Either<L, T> ToEither<L>(Func<L> Left) =>
            IsSome
                ? Right<L, T>(Value)
                : Left<L, T>(Left());

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
        public TryOption<T> ToTryOption<L>(L defaultLeftValue)
        {
            var self = this;
            return () => self;
        }

        [Pure]
        public static bool operator ==(Option<T> lhs, Option<T> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(Option<T> lhs, Option<T> rhs) =>
            !lhs.Equals(rhs);

        [Pure]
        public static Option<T> operator |(Option<T> lhs, Option<T> rhs) =>
            lhs.IsSome
                ? lhs
                : rhs;

        [Pure]
        public static bool operator true(Option<T> value) =>
            value.IsSome;

        [Pure]
        public static bool operator false(Option<T> value) =>
            value.IsNone;

        [Pure]
        public Type GetUnderlyingType() =>
            typeof(T);

        [Pure]
        public int CompareTo(Option<T> other) =>
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
        public bool Equals(Option<T> other) =>
            IsNone && other.IsNone
                ? true
                : IsSome && other.IsSome
                    ? EqualityComparer<T>.Default.Equals(Value, other.Value)
                    : false;

        public Monad<U> Bind<U>(Monad<T> ma, Func<T,Monad<U>> bind)
        {
            if (isnull(ma)) return Option<U>.None;
            var opt = (Option<T>)ma;
            return opt.IsSome
                ? bind(opt.Value)
                : Option<U>.None;
        }

        /// <summary>
        /// Produce a failure value
        /// </summary>
        public Monad<T> Fail() =>
            None;

        public Monad<T> Return(T value) =>
            Optional(value);

        public Functor<U> Map<U>(Functor<T> x, Func<T, U> f)
        {
            if (isnull(x)) return Option<U>.None;
            var opt = (Option<T>)x;
            return opt.IsSome
                ? Optional(f(opt.Value))
                : Option<U>.None;
        }

        public Applicative<T> Pure(T a) =>
            Optional(a);

        public Applicative<U> Apply<U>(Applicative<Func<T, U>> x, Applicative<T> y) =>
            from a in (Option<Func<T, U>>)x
            from b in (Option<T>)y
            select a(b);

        public Applicative<V> Apply<U, V>(Applicative<Func<T, U, V>> x, Applicative<T> y, Applicative<U> z) =>
            from a in (Option<Func<T, U, V>>)x
            from b in (Option<T>)y
            from c in (Option<U>)z
            select a(b, c);

        public Applicative<Func<U, V>> Apply<U, V>(Applicative<Func<T, Func<U, V>>> x, Applicative<T> y) =>
            from a in (Option<Func<T, Func<U, V>>>)x
            from b in (Option<T>)y
            select a(b);

        /// <summary>
        /// Sequential actions
        ///  f a -> f b -> f b
        /// </summary>
        public Applicative<U> Action<U>(Applicative<T> x, Applicative<U> y) =>
            from a in (Option<T>)x
            from b in (Option<U>)y
            select b;
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

        [Pure]
        public R None(Func<R> noneHandler) =>
            match(option, someHandler, noneHandler);

        [Pure]
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
}

public static class OptionExtensions
{
    [Pure]
    public static Option<T> Append<SEMI, T>(this Option<T> x, Option<T> y) where SEMI : struct, Semigroup<T> =>
        from a in x
        from b in y
        select append<SEMI, T>(a, b);

    [Pure]
    public static Option<T> Add<ADD, T>(this Option<T> x, Option<T> y) where ADD : struct, Add<T> =>
        from a in x
        from b in y
        select add<ADD, T>(a, b);

    [Pure]
    public static Option<T> Difference<DIFF, T>(this Option<T> x, Option<T> y) where DIFF : struct, Difference<T> =>
        from a in x
        from b in y
        select difference<DIFF, T>(a, b);

    [Pure]
    public static Option<T> Product<PROD, T>(this Option<T> x, Option<T> y) where PROD : struct, Product<T> =>
        from a in x
        from b in y
        select product<PROD, T>(a, b);

    [Pure]
    public static Option<T> Divide<DIV, T>(this Option<T> x, Option<T> y) where DIV : struct, Divide<T> =>
        from a in x
        from b in y
        select divide<DIV, T>(a, b);

    /// <summary>
    /// Extracts from a list of 'Option' all the 'Some' elements.
    /// All the 'Some' elements are extracted in order.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Somes<T>(this IEnumerable<Option<T>> self)
    {
        foreach (var item in self)
        {
            if (item.IsSome)
            {
                yield return item.Value;
            }
        }
    }

    [Pure]
    public static T? ToNullable<T>(this Option<T> self) where T : struct =>
        self.IsNone
            ? (T?)null
            : new T?(self.Value);

    public static Unit Iter<T>(this Option<T> self, Action<T> action) =>
        self.IfSome(action);

    [Pure]
    public static int Count<T>(this Option<T> self) =>
        self.IsSome
            ? 1
            : 0;

    [Pure]
    public static bool ForAll<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : true;

    [Pure]
    public static bool ForAll<T>(this Option<T> self, Func<T, bool> Some, Func<bool> None) =>
        self.IsSome
            ? Some(self.Value)
            : None();

    [Pure]
    public static bool Exists<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : false;

    [Pure]
    public static bool Exists<T>(this Option<T> self, Func<T, bool> Some, Func<bool> None) =>
        self.IsSome
            ? Some(self.Value)
            : None();

    /// <summary>
    /// Folds Option into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="tryDel">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<S, T>(this Option<T> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? folder(state, self.Value)
            : state;

    /// <summary>
    /// Folds Option into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="tryDel">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Some</param>
    /// <param name="None">Fold function for None</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<S, T>(this Option<T> self, S state, Func<S, T, S> Some, Func<S, S> None) =>
        self.IsSome
            ? Some(state, self.Value)
            : None(state);

    [Pure]
    public static Option<R> Map<T, R>(this Option<T> self, Func<T, R> mapper) =>
        isnull(self)
            ? Option<R>.None
            : (Option<R>)LanguageExt.TypeClass.Prelude.map<Option<T>, T, R>(self, mapper);

    [Pure]
    public static Option<R> Map<T, R>(this Option<T> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? Optional(Some(self.Value))
            : None();

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Option<Func<T2, R>> ParMap<T1, T2, R>(this Option<T1> opt, Func<T1, T2, R> func) =>
        opt.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Option<Func<T2, Func<T3, R>>> ParMap<T1, T2, T3, R>(this Option<T1> opt, Func<T1, T2, T3, R> func) =>
        opt.Map(curry(func));

    [Pure]
    public static Option<T> Filter<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
                ? self
                : None
            : self;

    [Pure]
    public static Option<T> Filter<T>(this Option<T> self, Func<T, bool> Some, Func<bool> None) =>
        self.IsSome
            ? Some(self.Value)
                ? self
                : Option<T>.None
            : None()
                ? self
                : Option<T>.None;

    [Pure]
    public static Option<R> Bind<T, R>(this Option<T> self, Func<T, Option<R>> binder) =>
        self.IsSome
            ? binder(self.Value)
            : None;

    [Pure]
    public static Option<R> Bind<T, R>(this Option<T> self, Func<T, Option<R>> Some, Func<Option<R>> None) =>
        self.IsSome
            ? Some(self.Value)
            : None();

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<U> Select<T, U>(this Option<T> self, Func<T, U> map) =>
        self.Map(map);

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<T> Where<T>(this Option<T> self, Func<T, bool> pred) =>
        self.Filter(pred);

    [Pure]
    public static int Sum(this Option<int> self) =>
        self.IsSome
            ? self.Value
            : 0;

    [Pure]
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

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<V> SelectMany<T, U, V>(this Option<T> self,
        Func<T, IEnumerable<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return new V[0];
        return bind(self.Value).Map(resU => project(self.Value, resU));
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Option<V> SelectMany<T, U, V>(this IEnumerable<T> self,
        Func<T, Option<U>> bind,
        Func<T, U, V> project
        )
    {
        var ta = self.Take(1).ToArray();
        if (ta.Length == 0) return None;
        var resU = bind(ta[0]);
        if (resU.IsNone) return None;
        return Optional(project(ta[0], resU.Value));
    }

    /// <summary>
    /// Match the two states of the Option and return a promise of a non-null R.
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler.  Cannot return null.</param>
    /// <param name="None">None handler.  Cannot return null.</param>
    /// <returns>A promise to return a non-null R</returns>
    public static async Task<R> MatchAsync<T, R>(this Option<Task<T>> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? Option<Task<T>>.CheckNullSomeReturn(Some(await self.Value))
            : Option<Task<T>>.CheckNullNoneReturn(None());

    /// <summary>
    /// Match the two states of the Option and return a stream of non-null Rs.
    /// </summary>
    /// <param name="Some">Some handler.  Cannot return null.</param>
    /// <param name="None">None handler.  Cannot return null.</param>
    /// <returns>A stream of non-null Rs</returns>
    [Pure]
    public static IObservable<R> MatchObservable<T, R>(this Option<IObservable<T>> self, Func<T, R> Some, Func<R> None) =>
        self.IsSome
            ? self.Value.Select(Some).Select(Option<R>.CheckNullSomeReturn)
            : Observable.Return(Option<R>.CheckNullNoneReturn(None()));

    /// <summary>
    /// Match the two states of the IObservable&lt;Option&lt;T&gt;&gt; and return a stream of non-null Rs.
    /// </summary>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="Some">Some handler.  Cannot return null.</param>
    /// <param name="None">None handler.  Cannot return null.</param>
    /// <returns>A stream of non-null Rs</returns>
    [Pure]
    public static IObservable<R> MatchObservable<T, R>(this IObservable<Option<T>> self, Func<T, R> Some, Func<R> None) =>
        self.Select(opt => match(opt, Some, None));

    public static async Task<Option<R>> MapAsync<T, R>(this Option<T> self, Func<T, Task<R>> map) =>
        self.IsSome
            ? Some(await map(self.Value))
            : None;

    public static async Task<Option<R>> MapAsync<T, R>(this Task<Option<T>> self, Func<T, Task<R>> map)
    {
        var val = await self;
        return val.IsSome
            ? Some(await map(val.Value))
            : None;
    }

    public static async Task<Option<R>> MapAsync<T, R>(this Task<Option<T>> self, Func<T, R> map)
    {
        var val = await self;
        return val.IsSome
            ? Some(map(val.Value))
            : None;
    }

    public static async Task<Option<R>> MapAsync<T, R>(this Option<Task<T>> self, Func<T, R> map) =>
        self.IsSome
            ? Some(map(await self.Value))
            : None;

    public static async Task<Option<R>> MapAsync<T, R>(this Option<Task<T>> self, Func<T, Task<R>> map) =>
        self.IsSome
            ? Some(await map(await self.Value))
            : None;


    public static async Task<Option<R>> BindAsync<T, R>(this Option<T> self, Func<T, Task<Option<R>>> bind) =>
        self.IsSome
            ? await bind(self.Value)
            : None;

    public static async Task<Option<R>> BindAsync<T, R>(this Task<Option<T>> self, Func<T, Task<Option<R>>> bind)
    {
        var val = await self;
        return val.IsSome
            ? await bind(val.Value)
            : None;
    }

    public static async Task<Option<R>> BindAsync<T, R>(this Task<Option<T>> self, Func<T, Option<R>> bind)
    {
        var val = await self;
        return val.IsSome
            ? bind(val.Value)
            : None;
    }

    public static async Task<Option<R>> BindAsync<T, R>(this Option<Task<T>> self, Func<T, Option<R>> bind) =>
        self.IsSome
            ? bind(await self.Value)
            : None;

    public static async Task<Option<R>> BindAsync<T, R>(this Option<Task<T>> self, Func<T, Task<Option<R>>> bind) =>
        self.IsSome
            ? await bind(await self.Value)
            : None;

    public static async Task<Unit> IterAsync<T>(this Task<Option<T>> self, Action<T> action)
    {
        var val = await self;
        if (val.IsSome) action(val.Value);
        return unit;
    }

    public static async Task<Unit> IterAsync<T>(this Option<Task<T>> self, Action<T> action)
    {
        if (self.IsSome) action(await self.Value);
        return unit;
    }

    public static async Task<int> CountAsync<T>(this Task<Option<T>> self) =>
        (await self).Count();

    public static async Task<int> SumAsync(this Task<Option<int>> self) =>
        (await self).Sum();

    public static async Task<int> SumAsync(this Option<Task<int>> self) =>
        self.IsSome
            ? await self.Value
            : 0;

    public static async Task<S> FoldAsync<T, S>(this Task<Option<T>> self, S state, Func<S, T, S> folder) =>
        (await self).Fold(state, folder);

    public static async Task<S> FoldAsync<T, S>(this Option<Task<T>> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? folder(state, await self.Value)
            : state;

    public static async Task<bool> ForAllAsync<T>(this Task<Option<T>> self, Func<T, bool> pred) =>
        (await self).ForAll(pred);

    public static async Task<bool> ForAllAsync<T>(this Option<Task<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : true;

    public static async Task<bool> ExistsAsync<T>(this Task<Option<T>> self, Func<T, bool> pred) =>
        (await self).Exists(pred);

    public static async Task<bool> ExistsAsync<T>(this Option<Task<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(await self.Value)
            : false;

    public static Option<V> Join<L, T, U, K, V>(
        this Option<T> self,
        Option<U> inner,
        Func<T, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<T, U, V> project)
    {
        if (self.IsNone) return None;
        if (inner.IsNone) return None;
        return EqualityComparer<K>.Default.Equals(outerKeyMap(self.Value), innerKeyMap(inner.Value))
            ? Some(project(self.Value, inner.Value))
            : None;
    }
}