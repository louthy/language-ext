using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans.Linq
{
    public static partial class MapTrans
    {
        // 
        // Map<K, IEnumerable<T>> extensions 
        // 
        public static Map<K, IEnumerable<U>> Select<K, T, U>(this Map<K, IEnumerable<T>> self, Func<T, U> map) =>
            self.MapT(map);

        public static Map<K, IEnumerable<V>> SelectMany<K, T, U, V>(this Map<K, IEnumerable<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Map<K, IEnumerable<V>> SelectMany<K, T, U, V>(this Map<K, IEnumerable<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x)));

        public static Map<K, IEnumerable<T>> Where<K, T>(this Map<K, IEnumerable<T>> self, Func<T, bool> pred) =>
            self.FilterT(pred);

        // 
        // Map<Lst<T>> extensions 
        // 
        public static Map<K, Lst<U>> Select<K, T, U>(this Map<K, Lst<T>> self, Func<T, U> map) =>
            self.MapT(x => List.createRange(x.MapT(map)));

        public static Map<K, Lst<V>> SelectMany<K, T, U, V>(this Map<K, Lst<T>> self,
            Func<T, Lst<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Map<K, Lst<V>> SelectMany<K, T, U, V>(this Map<K, Lst<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x)));

        public static Map<K, Lst<T>> Where<K, T>(this Map<K, Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));

        // 
        // Map<Map<T>> extensions 
        // 

        public static Map<K, Map<J, T>> Select<K, J, T>(this Map<K, Map<J, T>> self, Func<T, T> map) =>
            self.MapT(x => x.MapT(map));

        public static Map<K, Map<J, V>> SelectMany<K, J, T, U, V>(this Map<K, Map<J, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => project(x, bind(x)));

        public static Map<K, Map<J, V>> Where<K, J, V>(this Map<K, Map<J, V>> self, Func<J, V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Map<K, Map<J, V>> Where<K, J, V>(this Map<K, Map<J, V>> self, Func<J, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Map<K, Map<J, V>> Where<K, J, V>(this Map<K, Map<J, V>> self, Func<V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        // 
        // Map<K, Option<T>> extensions 
        // 
        public static Map<K, Option<U>> Select<K, T, U>(this Map<K, Option<T>> self, Func<T, U> map) =>
            self.Map(x => x.Select(map));

        public static Map<K, Option<V>> SelectMany<K, T, U, V>(this Map<K, Option<T>> self,
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, Option<V>> SelectMany<K, T, U, V>(this Map<K, Option<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(t => project(t, bind(t))));


        public static Map<K, Option<T>> Where<K, T>(this Map<K, Option<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));

        // 
        // IEnumerable<TryOption<T>> extensions 
        // 
        public static Map<K, TryOption<U>> Select<K, T, U>(this Map<K, TryOption<T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, TryOption<V>> SelectMany<K, T, U, V>(this Map<K, TryOption<T>> self,
            Func<T, TryOption<U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, TryOption<V>> SelectMany<K, T, U, V>(this Map<K, TryOption<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(t => project(t, bind(t))));

        public static Map<K, TryOption<T>> Where<K, T>(this Map<K, TryOption<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));

        // 
        // IEnumerable<Try<T>> extensions 
        // 
        public static Map<K, Try<U>> Select<K, T, U>(this Map<K, Try<T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, Try<V>> SelectMany<K, T, U, V>(this Map<K, Try<T>> self,
            Func<T, Try<U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, Try<V>> SelectMany<K, T, U, V>(this Map<K, Try<T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(t => project(t, bind(t))));

        public static Map<K, Try<T>> Where<K, T>(this Map<K, Try<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));


        // 
        // IEnumerable<Either<L,R>> extensions 
        // 
        public static Map<K, Either<L, R2>> Select<K, L, R, R2>(this Map<K, Either<L, R>> self, Func<R, R2> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, Either<L, R3>> SelectMany<K, L, R, R2, R3>(this Map<K, Either<L, R>> self,
            Func<R, Either<L, R2>> bind,
            Func<R, R2, R3> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, Either<L, V>> SelectMany<L, K, T, U, V>(this Map<K, Either<L, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(t => project(t, bind(t))));

        public static Map<K, Either<Unit, R>> Where<K, L, R>(this Map<K, Either<L, R>> self, Func<R, bool> pred) =>
            self.Map(x => x.Filter(pred));

        // 
        // Map<K, Writer<Out,T>> extensions 
        // 
        public static Map<K, Writer<Out, U>> Select<K, Out, T, U>(this Map<K, Writer<Out, T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, Writer<Out, V>> SelectMany<K, Out, T, U, V>(this Map<K, Writer<Out, T>> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, Writer<Out, V>> SelectMany<Out, K, T, U, V>(this Map<K, Writer<Out, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(t => project(t, bind(t))));

        // 
        // Map<K, Reader<Env,T>> extensions 
        // 
        public static Map<K, Reader<Env, U>> Select<K, Env, T, U>(this Map<K, Reader<Env, T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, Reader<Env, V>> SelectMany<K, Env, T, U, V>(this Map<K, Reader<Env, T>> self,
            Func<T, Reader<Env, U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, Reader<Env, V>> SelectMany<Env, K, T, U, V>(this Map<K, Reader<Env, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(t => project(t, bind(t))));

        // 
        // Map<K, <State<S,T>> extensions 
        // 
        public static Map<K, State<S, U>> Select<K, S, T, U>(this Map<K, State<S, T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, State<S, V>> SelectMany<K, S, T, U, V>(this Map<K, State<S, T>> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, State<S, V>> SelectMany<S, K, T, U, V>(this Map<K, State<S, T>> self,
            Func<T, U> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.MapT(t => project(t, bind(t))));
    }
}