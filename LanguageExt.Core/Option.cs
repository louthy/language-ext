using System;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExt
{
    public struct Option<T>
    {
        T value;

        public Option(T value)
        {
            this.IsSome = value != null;
            this.value = value;
        }

        public Option()
        {
            this.IsSome = false;
            this.value = default(T);
        }

        public static Option<T> Some(T value) => new Option<T>(value);
        public static readonly Option<T> None = new Option<T>();

        public bool IsSome { get; }
        public bool IsNone => !IsSome;

        internal T Value
        {
            get
            {
                if (IsSome)
                {
                    return value;
                }
                else
                {
                    throw new OptionIsNoneException();
                }
            }
        }

        public static implicit operator Option<T>(T value) =>
            value == null
                ? Option<T>.None
                : Option<T>.Some(value);

        public static implicit operator Option<T>(OptionNone none) => Option<T>.None;

        public R Match<R>(Func<T, R> Some, Func<R> None) =>
            IsSome
                ? Some(Value)
                : None();

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

        public T Failure(Func<T> None) => Match(identity<T>(), None);

        public T Failure(T noneValue) => Match(identity<T>(), () => noneValue);
    }

    [Serializable]
    public class OptionIsNoneException : Exception
    {
        public OptionIsNoneException()
            : base("Option isn't set.")
        {
        }

        public OptionIsNoneException(string message) : base(message)
        {
        }

        public OptionIsNoneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public struct OptionNone
    {
        public static OptionNone Default = new OptionNone();
    }
}

public static class __OptionExt
{
    public static Option<U> Select<T, U>(this Option<T> self, Func<T, U> map) =>
        match(self,
            Some: t => Option<U>.Some(map(t)),
            None: () => Option<U>.None
            );

    public static Option<V> SelectMany<T, U, V>(this Option<T> self,
        Func<T, Option<U>> bind,
        Func<T, U, V> project
        ) =>
        match(self,
            Some: t =>
                match(bind(t),
                    Some: u => Option<V>.Some(project(t, u)),
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
