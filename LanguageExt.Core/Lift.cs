using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Prelude
    {
        public static Option<Option<R>> lift<T, R>(Option<Option<T>> m, Func<T, R> f) =>
            m.Map(x => x.Map(f));

        public static Reader<E, Option<R>> lift<E, T, R>(Reader<E, Option<T>> m, Func<T, R> f) =>
            m.Map(x => x.Map(f));

        public static State<S, Reader<E, R>> lift<S, E, T, R>(State<S, Reader<E, T>> m, Func<T, R> f) =>
            m.Map(x => x.Map(f));

        public static State<S, Reader<E, R>> mapT<S, E, T, R>(State<S, Reader<E, T>> m, Func<T, R> f) =>
            lift(m, f);

        public static State<S, Reader<E, R>> bindT<S, E, T, R>(State<S, Reader<E, T>> m, Func<T, Reader<E, R>> f) =>
            m.Map(x => x.Bind(f));

        public static Reader<Env, Env> askr<Env>() =>
            env => new ReaderResult<Env>(env);

        public static State<S, Reader<E, E>> ask<S, E, T>(State<S, Reader<E, T>> m) =>
            bindT(m, _ => askr<E>());

    }
}
