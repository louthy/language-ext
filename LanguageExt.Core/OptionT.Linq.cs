using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans.Linq
{
    public static partial class OptionT
    {
        // 
        // Option<T> extensions 
        // 

        public static Option<IEnumerable<U>> Select<T, U>(this Option<IEnumerable<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<IEnumerable<V>> SelectMany<T, U, V>(this Option<IEnumerable<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<IEnumerable<V>> SelectMany<T, U, V>(this Option<IEnumerable<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;

        public static Option<IEnumerable<T>> Where<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        // 
        // Option<Lst<T>> extensions 
        // 

        public static Option<Lst<U>> Select<T, U>(this Option<Lst<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map).Freeze())
                : None;

        public static Option<Lst<V>> SelectMany<T, U, V>(this Option<Lst<T>> self,
            Func<T, Lst<U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project).Freeze())
                : None;

        public static Option<Lst<V>> SelectMany<T, U, V>(this Option<Lst<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t,bind(t))).Freeze())
                : None;

        public static Option<Lst<T>> Where<T>(this Option<Lst<T>> self, Func<T, bool> pred) =>
            self.FilterT(pred);

        // 
        // Option<Map<T>> extensions 
        // 

        public static Option<Map<K, U>> Select<K, V, U>(this Option<Map<K, V>> self, Func<V, U> map) =>
            self.MapT(map);

        public static Option<Map<K, U>> Select<K, V, U>(this Option<Map<K, V>> self, Func<K, V, U> map) =>
            self.MapT(map);

        public static Option<Map<K, V>> SelectMany<K, T, U, V>(this Option<Map<K, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;

        public static Option<Map<K, V>> WhereT<K, V>(this Option<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.FilterT(pred);

        public static Option<Map<K, V>> WhereT<K, V>(this Option<Map<K, V>> self, Func<K, bool> pred) =>
            self.FilterT(pred);

        public static Option<Map<K, V>> WhereT<K, V>(this Option<Map<K, V>> self, Func<V, bool> pred) =>
            self.FilterT(pred);


        // 
        // Option<Option<T>> extensions 
        // 
        public static Option<Option<U>> Select<T, U>(this Option<Option<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Option<V>> SelectMany<T, U, V>(this Option<Option<T>> self,
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<Option<V>> SelectMany<T, U, V>(this Option<Option<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;

        public static Option<Option<T>> Where<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        // 
        // Option<TryOption<T>> extensions 
        // 

        public static Option<TryOption<U>> Select<T, U>(this Option<TryOption<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<TryOption<V>> SelectMany<T, U, V>(this Option<TryOption<T>> self,
            Func<T, TryOption<U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<TryOption<V>> SelectMany<T, U, V>(this Option<TryOption<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;

        public static Option<TryOption<T>> Where<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        // 
        // Option<Try<T>> extensions 
        // 

        public static Option<Try<V>> SelectMany<T, U, V>(this Option<Try<T>> self,
            Func<T, Try<U>> bind,
            Func<T, U, V> project
            ) =>
                self.IsSome
                    ? Some(self.Value.SelectMany(bind, project))
                    : None;

        public static Option<Try<V>> SelectMany<T, U, V>(this Option<Try<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;

        public static Option<Try<T>> Where<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        // 
        // Option<Either<L,R>> extensions 
        // 
        public static Option<Either<L, U>> Select<L, R, U>(this Option<Either<L, R>> self, Func<R, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Either<L, V>> SelectMany<L, R, U, V>(this Option<Either<L, R>> self,
            Func<R, Either<L, U>> bind,
            Func<R, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<Either<L,V>> SelectMany<L, T, U, V>(this Option<Either<L,T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;

        public static Option<Either<Unit, R>> Where<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        // 
        // Option<Reader<Env,T>> extensions 
        // 

        public static Option<Reader<Env, U>> Select<Env, T, U>(this Option<Reader<Env, T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Reader<Env, V>> SelectMany<Env, T, U, V>(this Option<Reader<Env, T>> self,
            Func<T, Reader<Env, U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<Reader<Env, V>> SelectMany<Env, T, U, V>(this Option<Reader<Env, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;


        // 
        // Option<Writer<Out,T>> extensions 
        // 
        public static Option<Writer<Out, U>> Select<Out, T, U>(this Option<Writer<Out, T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Writer<Out, V>> SelectMany<Out, T, U, V>(this Option<Writer<Out, T>> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<Writer<Out, V>> SelectMany<Out, T, U, V>(this Option<Writer<Out, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;

        // 
        // Option<State<S,T>> extensions 
        // 
        public static Option<State<S, U>> Select<S, T, U>(this Option<State<S, T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<State<S, V>> SelectMany<S, T, U, V>(this Option<State<S, T>> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;


        public static Option<State<S, V>> SelectMany<S, T, U, V>(this Option<State<S, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.Select(t => project(t, bind(t))))
                : None;
    }
}
