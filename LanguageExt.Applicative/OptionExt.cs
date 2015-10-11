using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Applicative
{
    public static class OptionExt
    {
        public static Option<R> Apply<T, R>(this Option<Func<T, R>> opt, Option<T> arg)
            => opt.IsSome && arg.IsSome
                ? Option<R>.Some(opt.Value(arg.Value))
                : Option<R>.None;

        public static Option<Func<T2, R>> Apply<T1, T2, R>(this Option<Func<T1, T2, R>> opt, Option<T1> arg)
            => Some(curry(opt.Value)).Apply(arg);

        public static Option<Func<T2, R>> Map<T1, T2, R>(this Option<T1> opt, Func<T1, T2, R> func)
            => opt.Map(curry(func));

        public static Option<Func<T2, Func<T3, R>>> Map<T1, T2, T3, R>(this Option<T1> opt, Func<T1, T2, T3, R> func)
            => opt.Map(curry(func));
    }
}
