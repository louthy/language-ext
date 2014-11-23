using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Prelude;
using System.Collections;
using System.Collections.Immutable;

namespace LanguageExt
{
    /// <summary>
    /// OptionUnsafe<T> can be in two states:
    ///     1. Some(x) -- which means there is a value stored inside
    ///     2. None    -- which means there's no value stored inside
    /// To extract the value you must use the 'matchUnsafe' function.
    /// This differs from Option<T> in that it allows Some(null) which
    /// is expressly forbidden for Option<T>.  That is what makes this
    /// type 'unsafe'.  
    /// </summary>
    public struct OptionUnsafe<T> : IOptionalValue, IEnumerable<T>
    {
        readonly T value;

        private OptionUnsafe(T value, bool isSome)
        {
            this.IsSome = isSome;
            this.value = value;
        }

        private OptionUnsafe(T value)
            : this(value, value != null)
        { }

        public OptionUnsafe()
            : this(default(T), false)
        { }

        public static OptionUnsafe<T> Some(T value) =>
            new OptionUnsafe<T>(value);

        public static readonly OptionUnsafe<T> None = new OptionUnsafe<T>();

        public bool IsSome { get; }

        public bool IsNone =>
            !IsSome;

        internal T Value =>
            IsSome
                ? value
                : raise<T>(new OptionIsNoneException());

        public static implicit operator OptionUnsafe<T>(T value) =>
            value == null
                ? OptionUnsafe<T>.None
                : OptionUnsafe<T>.Some(value);

        public static implicit operator OptionUnsafe<T>(OptionNone none) =>
            OptionUnsafe<T>.None;

        public R MatchUnsafe<R>(Func<T, R> Some, Func<R> None) =>
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

        public T FailureUnsafe(Func<T> None) =>
            MatchUnsafe(identity<T>(), None);

        public T FailureUnsafe(T noneValue) =>
            MatchUnsafe(identity<T>(), () => noneValue);

        public SomeUnsafeContext<T, R> SomeUnsafe<R>(Func<T, R> someHandler) =>
            new SomeUnsafeContext<T, R>(this, someHandler);

        public override string ToString() =>
            IsSome
                ? Value == null
                    ? "[null]"
                    : Value.ToString()
                : "[None]";

        public override int GetHashCode() =>
            IsSome && Value != null
                ? Value.GetHashCode()
                : 0;

        public override bool Equals(object obj) =>
            IsSome && Value != null
                ? Value.Equals(obj)
                : false;

        public int Count =>
            IsSome ? 1 : 0;

        public bool ForAll(Func<T,bool> pred) =>
            IsSome
                ? pred(Value)
                : true;

        public S Fold<S>(S state, Func<S, T, S> folder) =>
            IsSome
                ? folder(state, Value)
                : state;

        public bool Exists(Func<T,bool> pred) =>
            IsSome
                ? pred(Value)
                : false;

        public OptionUnsafe<R> Map<R>(Func<T,R> mapper) =>
            IsSome
                ? OptionUnsafe<R>.Some(mapper(Value))
                : OptionUnsafe<R>.None;

        public OptionUnsafe<R> Bind<R>(Func<T, OptionUnsafe<R>> binder) =>
            IsSome
                ? binder(Value)
                : OptionUnsafe<R>.None;

        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
            {
                yield return Value;
            }
        }

        public IEnumerator<T> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        public IImmutableList<T> ToList() =>
            Prelude.toList(AsEnumerable());

        public ImmutableArray<T> ToArray() =>
            Prelude.toArray(AsEnumerable());
    }

    public struct SomeUnsafeContext<T, R>
    {
        OptionUnsafe<T> option;
        Func<T, R> someHandler;

        internal SomeUnsafeContext(OptionUnsafe<T> option, Func<T, R> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public R None(Func<R> noneHandler) =>
            matchUnsafe(option, someHandler, noneHandler);

        public R None(R noneValue) =>
            matchUnsafe(option, someHandler, () => noneValue);
    }

    internal static class OptionUnsafe
    {
        public static OptionUnsafe<T> Cast<T>(T value) =>
            value == null
                ? OptionUnsafe<T>.None
                : OptionUnsafe<T>.Some(value);


        public static OptionUnsafe<T> Cast<T>(Nullable<T> value) where T : struct =>
            value == null
                ? OptionUnsafe<T>.None
                : OptionUnsafe<T>.Some(value.Value);
    }
}

public static class __OptionUnsafeExt
{
    public static OptionUnsafe<U> Select<T, U>(this OptionUnsafe<T> self, Func<T, U> map) => 
        self.Map(map);

    public static OptionUnsafe<V> SelectMany<T, U, V>(this OptionUnsafe<T> self,
        Func<T, OptionUnsafe<U>> bind,
        Func<T, U, V> project
        ) =>
        matchUnsafe(self,
            Some: t =>
                matchUnsafe(bind(t),
                    Some: u => OptionUnsafe.Cast<V>(project(t, u)),
                    None: () => OptionUnsafe<V>.None
                ),
            None: () => OptionUnsafe<V>.None
            );

    public static bool Where<T>(this OptionUnsafe<T> self, Func<T, bool> pred) =>
        self.Exists(pred);
}
