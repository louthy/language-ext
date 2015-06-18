using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.ComponentModel;

namespace LanguageExt
{
    /// <summary>
    /// Option<T> can be in two states:
    ///     1. Some(x) -- which means there is a value stored inside
    ///     2. None    -- which means there's no value stored inside
    /// To extract the value you must use the 'match' function.
    /// </summary>
    [TypeConverter(typeof(OptionalTypeConverter))]
    public struct Option<T> : IOptionalValue, IEnumerable<T>
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

        public object MatchUntyped(Func<object, object> Some, Func<object> None) =>
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

        public IImmutableList<T> ToList() =>
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

        public IEnumerator<T> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

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

    internal static class Option
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
    public static int Count<T>(this Option<T> self) =>
        self.IsSome 
            ? 1 
            : 0;

    public static bool ForAll<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : true;

    public static S Fold<T, S>(this Option<T> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? folder(state, self.Value)
            : state;

    public static bool Exists<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
            : false;

    public static Option<R> Map<T, R>(this Option<T> self, Func<T, R> mapper) =>
        self.IsSome
            ? Option.Cast(mapper(self.Value))
            : Option<R>.None;

    public static Option<T> Filter<T>(this Option<T> self, Func<T, bool> pred) =>
        self.IsSome
            ? pred(self.Value)
                ? self
                : None
            : self;

    public static Option<R> Bind<T, R>(this Option<T> self, Func<T, Option<R>> binder) =>
        self.IsSome
            ? binder(self.Value)
            : Option<R>.None;

    public static Option<U> Select<T, U>(this Option<T> self, Func<T, U> map) => 
        self.Map(map);

    public static Option<V> SelectMany<T, U, V>(this Option<T> self,
        Func<T, Option<U>> bind,
        Func<T, U, V> project
        ) =>
        match(self,
            Some: t =>
                match(bind(t),
                    Some: u => Option.Cast<V>(project(t, u)),
                    None: () => Option<V>.None
                ),
            None: () => Option<V>.None
            );

    public static Option<T> Where<T>(this Option<T> self, Func<T, bool> pred) =>
        self.Filter(pred)
            ? self
            : None;

    // 
    // Option<IEnumerable<T>> extensions 
    // 

    public static int Count<T>(this Option<IEnumerable<T>> self) =>
        self.IsSome
            ? List.length(self.Value)
            : 0;

    public static bool ForAll<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? List.forall(self.Value, pred)
            : true;

    public static S Fold<T, S>(this Option<IEnumerable<T>> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? List.fold(self.Value, state, folder)
            : state;

    public static bool Exists<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? List.exists(self.Value, pred)
            : false;

    public static Option<IEnumerable<R>> Map<T, R>(this Option<IEnumerable<T>> self, Func<T, R> mapper) =>
        self.IsSome
            ? Option.Cast(List.map(self.Value, mapper))
            : None;

    public static Option<IEnumerable<T>> Filter<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? Some(List.filter(self.Value, pred))
            : self;

    public static Option<IEnumerable<R>> Bind<T, R>(this Option<IEnumerable<T>> self, Func<T, Option<R>> binder) =>
        self.IsSome
            ? Some(from x in self.Value
                   let y = binder(x)
                   where y.IsSome
                   select y.Value)
            : None;

    public static Option<IEnumerable<U>> Select<T, U>(this Option<IEnumerable<T>> self, Func<T, U> map) =>
        self.IsSome
            ? Some(from x in self.Value
                   select map(x))
            : None;

    public static Option<IEnumerable<V>> SelectMany<T, U, V>(this Option<IEnumerable<T>> self,
        Func<T, Option<U>> bind,
        Func<T, U, V> project
        ) =>
        match(self,
            Some: tlist =>
                Some( from t in tlist
                      let x = match(bind(t),
                                  Some: u => Option.Cast<V>(project(t, u)),
                                  None: () => Option<V>.None )
                      where x.IsSome
                      select x.Value ),
            None: () => Option<IEnumerable<V>>.None
            );

    public static Option<IEnumerable<T>> Where<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
        self.Filter(pred);

    // 
    // Option<IImmutableList<T>> extensions 
    // 

    public static int Count<T>(this Option<IImmutableList<T>> self) =>
        self.IsSome
            ? List.length(self.Value)
            : 0;

    public static bool ForAll<T>(this Option<IImmutableList<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? List.forall(self.Value, pred)
            : true;

    public static S Fold<T, S>(this Option<IImmutableList<T>> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? List.fold(self.Value, state, folder)
            : state;

    public static bool Exists<T>(this Option<IImmutableList<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? List.exists(self.Value, pred)
            : false;

    public static Option<IImmutableList<R>> Map<T, R>(this Option<IImmutableList<T>> self, Func<T, R> mapper) =>
        self.IsSome
            ? Option.Cast(List.map(self.Value, mapper).Freeze())
            : None;

    public static Option<IImmutableList<T>> Filter<T>(this Option<IImmutableList<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? Some(List.filter(self.Value, pred).Freeze())
            : self;

    public static Option<IImmutableList<R>> Bind<T, R>(this Option<IImmutableList<T>> self, Func<T, Option<R>> binder) =>
        self.IsSome
            ? Some((from x in self.Value
                    let y = binder(x)
                    where y.IsSome
                    select y.Value).Freeze())
            : None;

    public static Option<IImmutableList<U>> Select<T, U>(this Option<IImmutableList<T>> self, Func<T, U> map) =>
        self.IsSome
            ? Some((from x in self.Value
                    select map(x)).Freeze())
            : None;

    public static Option<IImmutableList<V>> SelectMany<T, U, V>(this Option<IImmutableList<T>> self,
        Func<T, Option<U>> bind,
        Func<T, U, V> project
        ) =>
        match(self,
            Some: tlist =>
                Some((from t in tlist
                      let x = match(bind(t),
                                  Some: u => Option.Cast<V>(project(t, u)),
                                  None: () => Option<V>.None)
                      where x.IsSome
                      select x.Value).Freeze()),
            None: () => Option<IImmutableList<V>>.None
            );

    public static Option<IImmutableList<T>> Where<T>(this Option<IImmutableList<T>> self, Func<T, bool> pred) =>
        self.Filter(pred);


    // 
    // Option<Option<T>> extensions 
    // 

    public static int Count<T>(this Option<Option<T>> self) =>
        self.IsSome
            ? self.Value.Count()
            : 0;

    public static bool ForAll<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? self.Value.ForAll(pred)
            : true;

    public static S Fold<T, S>(this Option<Option<T>> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? self.Value.Fold(state, folder)
            : state;

    public static bool Exists<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? self.Value.Exists(pred)
            : false;

    public static Option<Option<R>> Map<T, R>(this Option<Option<T>> self, Func<T, R> mapper) =>
        self.IsSome
            ? Option.Cast(self.Value.Map(mapper))
            : None;

    public static Option<Option<T>> Filter<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? Some(self.Value.Filter(pred))
            : self;

    public static Option<Option<R>> Bind<T, R>(this Option<Option<T>> self, Func<T, Option<R>> binder) =>
        self.IsSome
            ? Some(self.Value.Bind(binder))
            : None;

    public static Option<Option<U>> Select<T, U>(this Option<Option<T>> self, Func<T, U> map) =>
        self.IsSome
            ? Some(self.Value.Map(map))
            : None;

    public static Option<Option<V>> SelectMany<T, U, V>(this Option<Option<T>> self,
        Func<T, Option<U>> bind,
        Func<T, U, V> project
        ) =>
        self.IsSome
            ? Some( match( self.Value.Bind(bind), 
                           Some: x =>  Optional(project(self.Value.Value,x)),
                           None: () => None ) )
            : None;

    public static Option<Option<T>> Where<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? Some(self.Value.Filter(pred))
            : None;

    // 
    // Option<TryOption<T>> extensions 
    // 

    public static int Count<T>(this Option<TryOption<T>> self) =>
        self.IsSome
            ? self.Value.Count()
            : 0;

    public static bool ForAll<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? self.Value.ForAll(pred)
            : true;

    public static S Fold<T, S>(this Option<TryOption<T>> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? self.Value.Fold(state, folder)
            : state;

    public static bool Exists<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? self.Value.Exists(pred)
            : false;

    public static Option<TryOption<R>> Map<T, R>(this Option<TryOption<T>> self, Func<T, R> mapper) =>
        self.IsSome
            ? Option.Cast(self.Value.Map(mapper))
            : None;

    public static Option<TryOption<T>> Filter<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? Some(self.Value.Filter(pred))
            : self;

    public static Option<TryOption<R>> Bind<T, R>(this Option<TryOption<T>> self, Func<T, TryOption<R>> binder) =>
        self.IsSome
            ? Some(self.Value.Bind(binder))
            : None;

    public static Option<TryOption<U>> Select<T, U>(this Option<TryOption<T>> self, Func<T, U> map) =>
        self.IsSome
            ? Some(self.Value.Map(map))
            : None;

    public static Option<TryOption<V>> SelectMany<T, U, V>(this Option<TryOption<T>> self,
        Func<T, TryOption<U>> bind,
        Func<T, U, V> project
        ) =>
            match( self,
                   tryopt => tryopt.Match(
                                    Some: t => bind(t).Match(
                                                Some: u  => Option<TryOption<V>>.Some(() => project(t,u)),
                                                None: () => Option<TryOption<V>>.None, 
                                                Fail: ex => Option<TryOption<V>>.None),
                                    None: () => Option<TryOption<V>>.None,
                                    Fail: ex => Option<TryOption<V>>.None
                                    ),
                   () => Option<TryOption<V>>.None );


    public static Option<TryOption<T>> Where<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? Some(self.Value.Filter(pred))
            : None;

    // 
    // Option<TryOption<T>> extensions 
    // 

    public static int Count<T>(this Option<Try<T>> self) =>
        self.IsSome
            ? self.Value.Count()
            : 0;

    public static bool ForAll<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? self.Value.ForAll(pred)
            : true;

    public static S Fold<T, S>(this Option<Try<T>> self, S state, Func<S, T, S> folder) =>
        self.IsSome
            ? self.Value.Fold(state, folder)
            : state;

    public static bool Exists<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? self.Value.Exists(pred)
            : false;

    public static Option<Try<R>> Map<T, R>(this Option<Try<T>> self, Func<T, R> mapper) =>
        self.IsSome
            ? Option.Cast(self.Value.Map(mapper))
            : None;

    public static Option<Try<T>> Filter<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? Some(self.Value.Filter(pred))
            : self;

    public static Option<Try<R>> Bind<T, R>(this Option<Try<T>> self, Func<T, Try<R>> binder) =>
        self.IsSome
            ? Some(self.Value.Bind(binder))
            : None;

    public static Option<Try<U>> Select<T, U>(this Option<Try<T>> self, Func<T, U> map) =>
        self.IsSome
            ? Some(self.Value.Map(map))
            : None;

    public static Option<Try<V>> SelectMany<T, U, V>(this Option<Try<T>> self,
        Func<T, Try<U>> bind,
        Func<T, U, V> project
        ) =>
            match(self,
                   tryopt => tryopt.Match(
                                    Succ: t => bind(t).Match(
                                                Succ: u => Option<Try<V>>.Some(() => project(t, u)),
                                                Fail: ex => Option<Try<V>>.None),
                                    Fail: ex => Option<Try<V>>.None
                                    ),
                   () => Option<Try<V>>.None);


    public static Option<Try<T>> Where<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
        self.IsSome
            ? Some(self.Value.Filter(pred))
            : None;

    // 
    // Option<Either<L,R>> extensions 
    // 

    public static int Count<L, R>(this Option<Either<L, R>> self) =>
        self.IsSome
            ? self.Value.Count()
            : 0;

    public static bool ForAll<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
        self.IsSome
            ? self.Value.ForAll(pred)
            : true;

    public static S Fold<L, R, S>(this Option<Either<L, R>> self, S state, Func<S, R, S> folder) =>
        self.IsSome
            ? self.Value.Fold(state, folder)
            : state;

    public static bool Exists<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
        self.IsSome
            ? self.Value.Exists(pred)
            : false;

    public static Option<Either<L, R2>> Map<L, R, R2>(this Option<Either<L, R>> self, Func<R, R2> mapper) =>
        self.IsSome
            ? Option.Cast(self.Value.Map(mapper))
            : None;

    public static Option<Either<Unit, R>> Filter<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
        self.IsSome
            ? Some(self.Value.Filter(pred))
            : None;

    public static Option<Either<L, R2>> Bind<L, R, R2>(this Option<Either<L, R>> self, Func<R, Either<L, R2>> binder) =>
        self.IsSome
            ? Some(self.Value.Bind(binder))
            : None;

    public static Option<Either<L, U>> Select<L, R, U>(this Option<Either<L, R>> self, Func<R, U> map) =>
        self.IsSome
            ? Some(self.Value.Map(map))
            : None;

    public static Option<Either<L, V>> SelectMany<L, R, U, V>(this Option<Either<L, R>> self,
        Func<R, Either<L, U>> bind,
        Func<R, U, V> project
        ) =>
        self.IsSome
            ? Some(self.Value.Bind(bind).Match(
                       Right: r => Right<L,V>(project(self.Value.RightValue, r)),
                       Left:  l => Left<L,V>(l)))
            : None;

    public static Option<Either<Unit, R>> Where<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
        self.IsSome
            ? Some(self.Value.Filter(pred))
            : None;
}
