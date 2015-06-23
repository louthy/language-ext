using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.ComponentModel;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Option<T> can be in two states:
    ///     1. Some(x) -- which means there is a value stored inside
    ///     2. None    -- which means there's no value stored inside
    /// To extract the value you must use the 'match' function.
    /// </summary>
    [TypeConverter(typeof(OptionalTypeConverter))]
    public struct Option<T> : 
        IOptional, 
        IEnumerable<T>, 
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

        private U CheckNullReturn<U>(U value, string location) =>
            value == null
                ? raise<U>(new ResultIsNullException("'" + location + "' result is null.  Not allowed."))
                : value;

        public R Match<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? CheckNullReturn(Some(Value), "Some")
                : CheckNullReturn(None(), "None");

        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            IsSome
                ? CheckNullReturn(Some(Value), "Some")
                : CheckNullReturn(None(), "None");

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

        public IEnumerator<T> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        public int CompareTo(Option<T> other) =>
            IsNone && other.IsNone
                ? 0
                : IsNone
                    ? -1
                    : Comparer<T>.Default.Compare(Value, other.Value);

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

    public static Option<U> Select<T, U>(this Option<T> self, Func<T, U> map) =>
        self.Map(map);

    public static Option<T> Where<T>(this Option<T> self, Func<T, bool> pred) =>
        self.Filter(pred)
            ? self
            : None;

    public static int Sum(this Option<int> self) =>
        self.IsSome
            ? self.Value
            : 0;

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

    public static Option<IEnumerable<V>> SelectMany<T, U, V>(this Option<T> self,
        Func<T, IEnumerable<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return new V[0];
        var resU = bind(self.Value);
        return Some(resU.Select(x => project(self.Value, x)));
    }

    public static Option<Lst<V>> SelectMany<T, U, V>(this Option<T> self,
        Func<T, Lst<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return List<V>();
        var resU = bind(self.Value);
        return LanguageExt.List.createRange((resU.Select(x => project(self.Value, x))));
    }

    public static Option<Map<K, V>> SelectMany<K, T, U, V>(this Option<T> self,
        Func<T, Map<K, U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return Prelude.Map<K, V>();
        var resU = bind(self.Value);
        return resU.Select(x => project(self.Value, x));
    }

    public static Option<OptionUnsafe<V>> SelectMany<T, U, V>(this Option<T> self,
        Func<T, OptionUnsafe<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        var resU = bind(self.Value);
        if (resU.IsNone || resU == null) return None;
        return Option<OptionUnsafe<V>>.Some(project(self.Value, resU.Value));
    }

    public static Option<Either<L, V>> SelectMany<L, T, U, V>(this Option<T> self,
        Func<T, Either<L, U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        var resU = bind(self.Value);
        if (resU.IsLeft) return Either<L, V>.Left(resU.LeftValue);
        return Optional(Either<L, V>.Right(project(self.Value, resU.RightValue)));
    }

    public static Option<EitherUnsafe<L, V>> SelectMany<L, T, U, V>(this Option<T> self,
        Func<T, EitherUnsafe<L, U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        var resU = bind(self.Value);
        if (resU.IsLeft) return EitherUnsafe<L, V>.Left(resU.LeftValue);
        return Optional(EitherUnsafe<L, V>.Right(project(self.Value, resU.RightValue)));
    }

    public static Option<TryOption<V>> SelectMany<T, U, V>(this Option<T> self,
        Func<T, TryOption<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        return Some<TryOption<V>>(() =>
        {
            var resU = bind(self.Value).Try();
            if (resU.IsFaulted) return new TryOptionResult<V>(resU.Exception);
            if (resU.Value.IsNone) return new TryOptionResult<V>(None);
            return new TryOptionResult<V>(project(self.Value, resU.Value.Value));
        });
    }

    public static Option<Try<V>> SelectMany<T, U, V>(this Option<T> self,
        Func<T, Try<U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        return Some<Try<V>>(() =>
        {
            var resU = bind(self.Value).Try();
            if (resU.IsFaulted) return new TryResult<V>(resU.Exception);
            return new TryResult<V>(project(self.Value, resU.Value));
        });
    }

    public static Option<Reader<E, V>> SelectMany<E, T, U, V>(this Option<T> self,
        Func<T, Reader<E, U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        return Some<Reader<E, V>>((E env) =>
        {
            var resU = bind(self.Value)(env);
            if (resU.IsBottom) return new ReaderResult<V>(default(V), true); ;
            return new ReaderResult<V>(project(self.Value, resU.Value));
        });
    }

    public static Option<Writer<Out, V>> SelectMany<Out, T, U, V>(this Option<T> self,
        Func<T, Writer<Out, U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;
        return Some<Writer<Out, V>>(() =>
        {
            var resU = bind(self.Value)();
            if (resU.IsBottom) return new WriterResult<Out, V>(default(V), resU.Output, true);
            return new WriterResult<Out, V>(project(self.Value, resU.Value),resU.Output);
        });
    }

    public static Option<State<S,V>> SelectMany<S, T, U, V>(this Option<T> self,
        Func<T, State<S, U>> bind,
        Func<T, U, V> project
        )
    {
        if (self.IsNone) return None;

        return Some<State<S, V>>((S state) =>
        {
            var resU = bind(self.Value)(state);
            if (resU.IsBottom)
                return new StateResult<S, V>(state,default(V),true);
            else
                return new StateResult<S, V>(resU.State,project(self.Value, resU.Value));
        });
    }
}
