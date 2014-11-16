using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

public static partial class LanguageExt
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
    public static LanguageExt.Option<U> Select<T, U>(this LanguageExt.Option<T> self, Func<T, U> map) =>
        LanguageExt.Match(self,
            Some: t => LanguageExt.Option<U>.Some(map(t)),
            None: () => LanguageExt.Option<U>.None
            );

    public static LanguageExt.Option<V> SelectMany<T, U, V>(this LanguageExt.Option<T> self,
        Func<T, LanguageExt.Option<U>> bind,
        Func<T, U, V> project
        ) =>
        LanguageExt.Match(self,
            Some: t =>
                LanguageExt.Match(bind(t),
                    Some: u => LanguageExt.Option<V>.Some(project(t, u)),
                    None: () => LanguageExt.Option<V>.None
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
