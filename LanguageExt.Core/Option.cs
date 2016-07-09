#if false
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
        Monad<T>,
        Foldable<T>
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

        /// <summary>
        /// Produce a failure value
        /// </summary>
        [Pure]
        public Monad<T> Fail(string err = "") =>
            None;

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="value">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Monad<T> Return(T value) =>
            Optional(value);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="bind">Bind function</param>
        /// <returns>Monad of B</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Monad<U> Bind<U>(Monad<T> ma, Func<T, Monad<U>> bind)
        {
            var opt = (Option<T>)ma;
            return opt.IsSome
                ? bind(opt.Value)
                : Option<U>.None;
        }

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="bind">Bind function</param>
        /// <returns>Monad of B</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Applicative<U> Bind<U>(Applicative<T> ma, Func<T, Applicative<U>> bind)
        {
            var opt = (Option<T>)ma;
            return opt.IsSome
                ? bind(opt.Value)
                : Option<U>.None;
        }

        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="x">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Functor<U> Map<U>(Functor<T> x, Func<T, U> f)
        {
            var opt = (Option<T>)x;
            return opt.IsSome
                ? Optional(f(opt.Value))
                : Option<U>.None;
        }

        /// <summary>
        /// Applicative construction
        /// 
        ///     a -> f a
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Applicative<T> Pure(T a) =>
            Optional(a);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b) -> f a -> f b
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Applicative<U> Apply<U>(Applicative<Func<T, U>> x, Applicative<T> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f b -> f c
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Applicative<V> Apply<U, V>(Applicative<Func<T, U, V>> x, Applicative<T> y, Applicative<U> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        /// <summary>
        /// Sequential application
        /// 
        ///     f(a -> b -> c) -> f a -> f(b -> c)
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Applicative<Func<U, V>> Apply<U, V>(Applicative<Func<T, Func<U, V>>> x, Applicative<T> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Sequential actions
        /// 
        ///     f a -> f b -> f b
        /// </summary>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Applicative<U> Action<U>(Applicative<T> x, Applicative<U> y) =>
            from a in x
            from b in y
            select b;

        /// <summary>
        /// Iterate the bound value (or not if the option is in a Some state)
        /// </summary>
        /// <param name="action">Action to perform</param>
        public Unit Iter(Action<T> action) =>
            IfSome(action);

        /// <summary>
        /// Returns 0 if the option is in a None state, 1 otherwise
        /// </summary>
        /// <returns>0 or 1</returns>
        [Pure]
        public int Count() =>
            IsSome
                ? 1
                : 0;

        /// <summary>
        /// Runs a predicate against the bound value.  If the option is in a 
        /// None state then 'true' is returned (because the predicate holds 
        /// for-all values).
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>True if the predicate holds for all values</returns>
        [Pure]
        public bool ForAll(Func<T, bool> pred) =>
            IsSome
                ? pred(Value)
                : true;

        /// <summary>
        /// Runs a predicate against the bound value.  If the option is in a 
        /// None state then 'false' is returned (because the predicate doesn't
        /// hold for any value).
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>False if the predicate holds for any value</returns>
        [Pure]
        public bool Exists(Func<T, bool> pred) =>
            IsSome
                ? pred(Value)
                : false;

        /// <summary>
        /// Folds Option into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, T, S> folder) =>
            IsSome
                ? folder(state, Value)
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
        public S Fold<S>(S state, Func<S, T, S> Some, Func<S, S> None) =>
            IsSome
                ? Some(state, Value)
                : None(state);

        [Pure]
        public Option<R> Map<R>(Func<T, R> mapper) =>
            IsSome
                ? Optional(mapper(Value))
                : Option<R>.None;

        [Pure]
        public Option<R> BiMap<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? Optional(Some(Value))
                : None();

        [Pure]
        public Option<T> Filter(Func<T, bool> pred) =>
            IsSome
                ? pred(Value)
                    ? this
                    : None
                : this;


        [Pure]
        public Option<T> BiFilter(Func<T, bool> Some, Func<bool> None) =>
            IsSome
                ? Some(Value)
                    ? this
                    : Option<T>.None
                : None()
                    ? this
                    : Option<T>.None;

        [Pure]
        public Option<R> Bind<R>(Func<T, Option<R>> binder) =>
            IsSome
                ? binder(Value)
                : Option<R>.None;

        [Pure]
        public Option<R> BiBind<R>(Func<T, Option<R>> Some, Func<Option<R>> None) =>
            IsSome
                ? Some(Value)
                : None();

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Option<V> SelectMany<U, V>(
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            )
        {
            if (IsNone) return Option<V>.None;
            var resU = bind(Value);
            if (resU.IsNone) return Option<V>.None;

            return Optional(project(Value, resU.Value));
        }

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Option<U> Select<U>(Func<T, U> map) =>
            Map(map);

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Option<T> Where(Func<T, bool> pred) =>
            Filter(pred);

        public Option<V> Join<U, K, V>(
            Option<U> inner,
            Func<T, K> outerKeyMap,
            Func<U, K> innerKeyMap,
            Func<T, U, V> project)
        {
            if (IsNone) return Option<V>.None;
            if (inner.IsNone) return Option<V>.None;
            return EqualityComparer<K>.Default.Equals(outerKeyMap(Value), innerKeyMap(inner.Value))
                ? Option<V>.Some(project(Value, inner.Value))
                : Option<V>.None;
        }

        /// <summary>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// 
        /// Fold([x1, x2, ..., xn] == x1 `f` (x2 `f` ... (xn `f` z)...)
        /// 
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public S Fold<S>(Foldable<T> ma, S state, Func<S, T, S> f)
        {
            var opt = (Option<T>)ma;
            return opt.IsSome
                ? f(state, opt.Value)
                : state;
        }

        /// <summary>
        /// In the case of lists, 'FoldBack', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// 
        /// FoldBack( [x1, x2, ..., xn]) == (...((z `f` x1) `f` x2) `f`...) `f` xn
        /// 
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed. 
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Folder function, applied for each item in fa</param>
        /// <returns>The aggregate state</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public S FoldBack<S>(Foldable<T> ma, S state, Func<S, T, S> f) =>
            Fold(ma, state, f);

        /// <summary>
        /// Iterate the value(s) within the structure
        /// </summary>
        /// <param name="f">Operation to perform on the value(s)</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Unit Iter(Iterable<T> ia, Action<T> f) =>
            ((Option<T>)ia).Iter(f);
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

    [Pure]
    public static T? ToNullable<T>(this Option<T> self) where T : struct =>
        self.IsNone
            ? (T?)null
            : new T?(self.Value);

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
    public static long Sum(this Option<long> self) =>
        Sum<TLong, long>(self);

    [Pure]
    public static decimal Sum(this Option<decimal> self) =>
        Sum<TDecimal, decimal>(self);

    [Pure]
    public static double Sum(this Option<double> self) =>
        Sum<TDouble, double>(self);

    [Pure]
    public static int Sum(this Option<int> self) =>
        Sum<TInteger, int>(self);

    [Pure]
    public static A Sum<NUM, A>(this Option<A> self) where NUM : struct, Num<A> =>
        self.Fold(self, fromInteger<NUM, A>(0), (s, x) => x);

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
}
#endif