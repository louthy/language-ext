using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans.Linq
{
    public static partial class ListT
    {
        public static Lst<U> Select<T, U>(this Lst<T> self, Func<T, U> map) =>
            self.MapT(map);

        public static Lst<V> SelectMany<T, U, V>(this Lst<T> self,
            Func<T, Lst<U>> bind,
            Func<T, U, V> project
            )
        {
            // TODO: Really not efficient!
            var res = new List<V>();
            foreach (var t in self)
            {
                foreach (var u in bind(t))
                {
                    res.Add(project(t, u));
                }
            }
            return List.createRange(res);
        }

        public static Lst<V> SelectMany<T, U, V>(this Lst<T> self,
            Func<T, U> bind,
            Func<T, U, V> project
            )
        {
            // TODO: Really not efficient!
            var res = new List<V>();
            foreach (var t in self)
            {
                res.Add(project(t, bind(t)));
            }
            return List.createRange(res);
        }

        public static Lst<T> Where<T>(this Lst<T> self, Func<T, bool> pred) =>
            self.Filter(pred).Freeze();


        public static Lst<IEnumerable<U>> Select<T, U>(this Lst<IEnumerable<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<IEnumerable<V>> SelectMany<T, U, V>(this Lst<IEnumerable<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<IEnumerable<V>> SelectMany<T, U, V>(this Lst<IEnumerable<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x))).Freeze();

        public static Lst<IEnumerable<T>> Where<T>(this Lst<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        public static Lst<Lst<U>> Select<T, U>(this Lst<Lst<T>> self, Func<T, U> map) =>
            self.MapT(x => List.createRange(x.MapT(map)));

        public static Lst<Lst<V>> SelectMany<T, U, V>(this Lst<Lst<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => List.createRange(x.SelectMany(bind, project)));

        public static Lst<Lst<T>> Where<T>(this Lst<Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));


        public static Lst<Option<U>> Select<T, U>(this Lst<Option<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Option<V>> SelectMany<T, U, V>(this Lst<Option<T>> self,
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<Option<V>> SelectMany<T, U, V>(this Lst<Option<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x))).Freeze();

        public static Lst<Option<T>> Where<T>(this Lst<Option<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        public static Lst<TryOption<U>> Select<T, U>(this Lst<TryOption<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<TryOption<V>> SelectMany<T, U, V>(this Lst<TryOption<T>> self,
            Func<T, TryOption<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<TryOption<V>> SelectMany<T, U, V>(this Lst<TryOption<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x))).Freeze();

        public static Lst<TryOption<T>> Where<T>(this Lst<TryOption<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        public static Lst<Try<U>> Select<T, U>(this Lst<Try<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Try<V>> SelectMany<T, U, V>(this Lst<Try<T>> self,
            Func<T, Try<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<Try<V>> SelectMany<T, U, V>(this Lst<Try<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x))).Freeze();

        public static Lst<Try<T>> Where<T>(this Lst<Try<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        public static Lst<Either<L, R2>> Select<L, R, R2>(this Lst<Either<L, R>> self, Func<R, R2> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Either<L, R3>> SelectMany<L, R, R2, R3>(this Lst<Either<L, R>> self,
            Func<R, Either<L, R2>> bind,
            Func<R, R2, R3> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<Either<L, V>> SelectMany<L, T, U, V>(this Lst<Either<L, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x))).Freeze();


        public static Lst<Either<Unit, R>> Where<L, R>(this Lst<Either<L, R>> self, Func<R, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        public static Lst<Writer<Out, U>> Select<Out, T, U>(this Lst<Writer<Out, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Writer<Out, V>> SelectMany<Out, T, U, V>(this Lst<Writer<Out, T>> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<Writer<Out, V>> SelectMany<Out, T, U, V>(this Lst<Writer<Out, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x))).Freeze();

        public static Lst<Reader<Env, U>> Select<Env, T, U>(this Lst<Reader<Env, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Reader<Env, V>> SelectMany<Env, T, U, V>(this Lst<Reader<Env, T>> self,
            Func<T, Reader<Env, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<Reader<Env, V>> SelectMany<Env, T, U, V>(this Lst<Reader<Env, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x))).Freeze();

        public static Lst<State<S, U>> Select<S, T, U>(this Lst<State<S, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<State<S, V>> SelectMany<S, T, U, V>(this Lst<State<S, T>> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<State<S, V>> SelectMany<S, T, U, V>(this Lst<State<S, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x))).Freeze();
    }
}
