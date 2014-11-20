using System;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Option<T> can be in two states:
    ///     1. Some(x) -- which means there is a value stored inside
    ///     2. None    -- which means there's no value stored inside
    /// To extract the value you must use the 'match' function.
    /// </summary>
    public struct Option<T> : IOptionalValue
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

        public Option() 
            : this(default(T), false)
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
                ? raise<U>(new ResultIsNullException("'\{location}' result is null.  Not allowed."))
                : value;

        public R Match<R>(Func<T, R> Some, Func<R> None) =>
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

        public T Failure(Func<T> None) => 
            Match(identity<T>(), None);

        public T Failure(T noneValue) => 
            Match(identity<T>(), () => noneValue);

        public SomeContext<T, R> Some<R>(Func<T, R> someHandler) =>
            new SomeContext<T, R>(this,someHandler);

        public override string ToString() =>
            IsSome
                ? Value == null 
                    ? "[null]"
                    :  Value.ToString()
                : "[None]";

        public override int GetHashCode() =>
            IsSome && Value != null
                ? Value.GetHashCode()
                : 0;

        public override bool Equals(object obj) =>
            IsSome && Value != null
                ? Value.Equals(obj)
                : false;
    }

    public struct SomeContext<T, R>
    {
        Option<T> option;
        Func<T, R> someHandler;

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
    public static Option<U> Select<T, U>(this Option<T> self, Func<T, U> map) =>
        match(self,
            Some: t => Option.Cast<U>(map(t)),
            None: () => Option<U>.None
            );

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
            None: () => LanguageExt.Option<V>.None
            );

    public static IEnumerable<T> AsEnumerable<T>(this LanguageExt.Option<T> self)
    {
        if (self.IsSome)
        {
            while (true)
            {
                yield return self.Value;
            }
        }
    }

    public static IEnumerable<T> AsEnumerableOne<T>(this LanguageExt.Option<T> self)
    {
        if (self.IsSome)
        {
            yield return self.Value;
        }
    }
}
