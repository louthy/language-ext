using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans.Linq
{
    public static partial class EnumerableT
    {
        public static IEnumerable<IEnumerable<U>> Select<T, U>(this IEnumerable<IEnumerable<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<IEnumerable<V>> SelectMany<T, U, V>(this IEnumerable<IEnumerable<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<IEnumerable<V>> SelectMany<T, U, V>(this IEnumerable<IEnumerable<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y,bind(y))));

        public static IEnumerable<IEnumerable<T>> WhereT<T>(this IEnumerable<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        public static IEnumerable<Lst<U>> Select<T, U>(this IEnumerable<Lst<T>> self, Func<T, U> map) =>
            self.MapT(x => List.createRange(x.MapT(map)));

        public static IEnumerable<Lst<V>> SelectMany<T, U, V>(this IEnumerable<Lst<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => List.createRange(x.SelectMany(bind, project)));

        public static IEnumerable<Lst<V>> SelectMany<T, U, V>(this IEnumerable<Lst<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y, bind(y))));

        public static IEnumerable<Lst<T>> Where<T>(this IEnumerable<Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));


        public static IEnumerable<Option<U>> Select<T, U>(this IEnumerable<Option<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Option<V>> SelectMany<T, U, V>(this IEnumerable<Option<T>> self,
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<Option<V>> SelectMany<T, U, V>(this IEnumerable<Option<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y, bind(y))));

        public static IEnumerable<Option<T>> Where<T>(this IEnumerable<Option<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<TryOption<U>> Select<T, U>(this IEnumerable<TryOption<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<TryOption<V>> SelectMany<T, U, V>(this IEnumerable<TryOption<T>> self,
            Func<T, TryOption<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<TryOption<T>> Where<T>(this IEnumerable<TryOption<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<TryOption<V>> SelectMany<T, U, V>(this IEnumerable<TryOption<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y, bind(y))));

        public static IEnumerable<Try<U>> Select<T, U>(this IEnumerable<Try<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Try<V>> SelectMany<T, U, V>(this IEnumerable<Try<T>> self,
            Func<T, Try<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<Try<T>> Where<T>(this IEnumerable<Try<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Try<V>> SelectMany<T, U, V>(this IEnumerable<Try<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y, bind(y))));

        public static IEnumerable<Either<L, R2>> Select<L, R, R2>(this IEnumerable<Either<L, R>> self, Func<R, R2> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Either<L, R3>> SelectMany<L, R, R2, R3>(this IEnumerable<Either<L, R>> self,
            Func<R, Either<L, R2>> bind,
            Func<R, R2, R3> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<Either<L, V>> SelectMany<L, T, U, V>(this IEnumerable<Either<L, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y, bind(y))));

        public static IEnumerable<Either<Unit, R>> Where<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        public static IEnumerable<Writer<Out, U>> Select<Out, T, U>(this IEnumerable<Writer<Out, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Writer<Out, V>> SelectMany<Out, T, U, V>(this IEnumerable<Writer<Out, T>> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<Writer<Out, V>> SelectMany<Out, T, U, V>(this IEnumerable<Writer<Out, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y, bind(y))));


        public static IEnumerable<Reader<Env, U>> Select<Env, T, U>(this IEnumerable<Reader<Env, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Reader<Env, V>> SelectMany<Env, T, U, V>(this IEnumerable<Reader<Env, T>> self,
            Func<T, Reader<Env, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<Reader<Env, V>> SelectMany<Env, T, U, V>(this IEnumerable<Reader<Env, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y, bind(y))));


        public static IEnumerable<State<S, U>> Select<S, T, U>(this IEnumerable<State<S, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<State<S, V>> SelectMany<S, T, U, V>(this IEnumerable<State<S, T>> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<State<S, V>> SelectMany<S, T, U, V>(this IEnumerable<State<S, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(y => project(y, bind(y))));
    }
}
