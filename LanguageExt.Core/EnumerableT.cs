using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class EnumerableT
    {
        public static S FoldT<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
            list.Fold(state, folder);

        public static S FoldBackT<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
            list.FoldBack(state, folder);

        public static T ReduceT<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
            list.Reduce(reducer);

        public static T ReduceBackT<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
            list.ReduceBack(reducer);

        public static IEnumerable<S> ScanT<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
            list.Scan(state, folder);

        public static IEnumerable<S> ScanBackT<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
            list.ScanBack(state, folder);

        public static Option<T> FindT<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
            list.Find(pred);

        public static int CountT<T>(this IEnumerable<T> list) =>
            list.Count();

        public static Unit IterT<T>(this IEnumerable<T> list, Action<T> action) =>
            list.Iter(action);

        public static Unit IterT<T>(this IEnumerable<T> list, Action<int, T> action) =>
            list.Iter(action);

        public static bool ForAllT<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
            list.ForAll(pred);

        public static IEnumerable<T> DistinctT<T>(this IEnumerable<T> list, Func<T, T, bool> compare) =>
            list.Distinct(compare);

        public static bool ExistsT<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
            list.Exists(pred);

        public static IEnumerable<R> MapT<T, R>(this IEnumerable<T> list, Func<T, R> map) =>
            list.Map(map);

        public static IEnumerable<T> FilterT<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
            list.Filter(pred);

        public static IEnumerable<R> BindT<T, R>(this IEnumerable<T> self, Func<T, IEnumerable<R>> binder) =>
            self.Bind(binder);

        // 
        // IEnumerable<IEnumerable<T>> extensions 
        // 

        public static Unit IterT<T>(this IEnumerable<IEnumerable<T>> self, Action<T> action) =>
            (from x in self
             from y in x
             select y).IterT(action);

        public static int CountT<T>(this IEnumerable<IEnumerable<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this IEnumerable<IEnumerable<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this IEnumerable<IEnumerable<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this IEnumerable<IEnumerable<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static IEnumerable<IEnumerable<R>> MapT<T, R>(this IEnumerable<IEnumerable<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static IEnumerable<IEnumerable<T>> FilterT<T>(this IEnumerable<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<IEnumerable<R>> BindT<T, R>(this IEnumerable<IEnumerable<T>> self, Func<T, IEnumerable<R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static IEnumerable<IEnumerable<U>> Select<T, U>(this IEnumerable<IEnumerable<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<IEnumerable<V>> SelectMany<T, U, V>(this IEnumerable<IEnumerable<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<IEnumerable<T>> WhereT<T>(this IEnumerable<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        // 
        // IEnumerable<Lst<T>> extensions 
        // 

        public static Unit IterT<T>(this IEnumerable<Lst<T>> self, Action<T> action) =>
            (from x in self
             from y in x
             select y).IterT(action);

        public static int CountT<T>(this IEnumerable<Lst<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this IEnumerable<Lst<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this IEnumerable<Lst<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this IEnumerable<Lst<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static IEnumerable<Lst<R>> MapT<T, R>(this IEnumerable<Lst<T>> self, Func<T, R> mapper) =>
            self.MapT(x => List.createRange(x.Map(mapper)));

        public static IEnumerable<Lst<T>> FilterT<T>(this IEnumerable<Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));

        public static IEnumerable<Lst<R>> BindT<T, R>(this IEnumerable<Lst<T>> self, Func<T, Lst<R>> binder) =>
            self.MapT(x => List.createRange(x.BindT(binder)));

        public static IEnumerable<Lst<U>> Select<T, U>(this IEnumerable<Lst<T>> self, Func<T, U> map) =>
            self.MapT(x => List.createRange(x.MapT(map)));

        public static IEnumerable<Lst<V>> SelectMany<T, U, V>(this IEnumerable<Lst<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => List.createRange(x.SelectMany(bind, project)));

        public static IEnumerable<Lst<T>> Where<T>(this IEnumerable<Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));

        // 
        // IEnumerable<Map<T>> extensions 
        // 

        public static Unit IterT<K, V>(this IEnumerable<Map<K, V>> self, Action<V> action)
        {
            foreach (var x in self)
            {
                x.IterT(action);
            }
            return Unit.Default;
        }

        public static Unit IterT<K, V>(this IEnumerable<Map<K, V>> self, Action<K, V> action)
        {
            foreach (var x in self)
            {
                x.IterT(action);
            }
            return Unit.Default;
        }

        public static int CountT<K, V>(this IEnumerable<Map<K, V>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAll<K, V>(this IEnumerable<Map<K, V>> self, Func<K, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static bool ForAllT<K, V>(this IEnumerable<Map<K, V>> self, Func<V, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static bool ForAllT<K, V>(this IEnumerable<Map<K, V>> self, Func<K, V, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<K, V, S>(this IEnumerable<Map<K, V>> self, S state, Func<S, K, V, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static S FoldT<K, V, S>(this IEnumerable<Map<K, V>> self, S state, Func<S, V, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static S FoldT<K, V, S>(this IEnumerable<Map<K, V>> self, S state, Func<S, K, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<K, V>(this IEnumerable<Map<K, V>> self, Func<K, V, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static bool ExistsT<K, V>(this IEnumerable<Map<K, V>> self, Func<K, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static bool ExistsT<K, V>(this IEnumerable<Map<K, V>> self, Func<V, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static IEnumerable<Map<K, R>> MapT<K, V, R>(this IEnumerable<Map<K, V>> self, Func<K, V, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static IEnumerable<Map<K, R>> MapT<K, V, R>(this IEnumerable<Map<K, V>> self, Func<V, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static IEnumerable<Map<K, V>> FilterT<K, V>(this IEnumerable<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Map<K, V>> FilterT<K, V>(this IEnumerable<Map<K, V>> self, Func<V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Map<K, V>> FilterT<K, V>(this IEnumerable<Map<K, V>> self, Func<K, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Map<K, V>> Where<K, V>(this IEnumerable<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Map<K, V>> Where<K, V>(this IEnumerable<Map<K, V>> self, Func<K, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Map<K, V>> Where<K, V>(this IEnumerable<Map<K, V>> self, Func<V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        // 
        // IEnumerable<Option<T>> extensions 
        // 

        public static Unit IterT<T>(this IEnumerable<Option<T>> self, Action<T> action)
        {
            foreach (var item in self)
            {
                item.IterT(action);
            }
            return unit;
        }

        public static int CountT<T>(this IEnumerable<Option<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this IEnumerable<Option<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this IEnumerable<Option<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this IEnumerable<Option<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static IEnumerable<Option<R>> MapT<T, R>(this IEnumerable<Option<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static IEnumerable<Option<T>> FilterT<T>(this IEnumerable<Option<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Option<R>> BindT<T, R>(this IEnumerable<Option<T>> self, Func<T, Option<R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static IEnumerable<Option<U>> Select<T, U>(this IEnumerable<Option<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Option<V>> SelectMany<T, U, V>(this IEnumerable<Option<T>> self,
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<Option<T>> Where<T>(this IEnumerable<Option<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        // 
        // IEnumerable<TryOption<T>> extensions 
        // 

        public static Unit IterT<T>(this IEnumerable<TryOption<T>> self, Action<T> action)
        {
            foreach (var item in self)
            {
                item.IterT(action);
            }
            return unit;
        }

        public static int CountT<T>(this IEnumerable<TryOption<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this IEnumerable<TryOption<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this IEnumerable<TryOption<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this IEnumerable<TryOption<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static IEnumerable<TryOption<R>> MapT<T, R>(this IEnumerable<TryOption<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.Map(mapper));

        public static IEnumerable<TryOption<T>> FilterT<T>(this IEnumerable<TryOption<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.Filter(pred));

        public static IEnumerable<TryOption<R>> BindT<T, R>(this IEnumerable<TryOption<T>> self, Func<T, TryOption<R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static IEnumerable<TryOption<U>> Select<T, U>(this IEnumerable<TryOption<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<TryOption<V>> SelectMany<T, U, V>(this IEnumerable<TryOption<T>> self,
            Func<T, TryOption<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<TryOption<T>> Where<T>(this IEnumerable<TryOption<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        // 
        // IEnumerable<Try<T>> extensions 
        // 

        public static Unit IterT<T>(this IEnumerable<Try<T>> self, Action<T> action)
        {
            foreach (var item in self)
            {
                item.IterT(action);
            }
            return unit;
        }

        public static int CountT<T>(this IEnumerable<Try<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this IEnumerable<Try<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this IEnumerable<Try<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this IEnumerable<Try<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static IEnumerable<Try<R>> MapT<T, R>(this IEnumerable<Try<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static IEnumerable<Try<T>> FilterT<T>(this IEnumerable<Try<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Try<R>> BindT<T, R>(this IEnumerable<Try<T>> self, Func<T, Try<R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static IEnumerable<Try<U>> Select<T, U>(this IEnumerable<Try<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Try<V>> SelectMany<T, U, V>(this IEnumerable<Try<T>> self,
            Func<T, Try<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<Try<T>> Where<T>(this IEnumerable<Try<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        // 
        // IEnumerable<Either<L,R>> extensions 
        // 

        public static Unit IterT<L, R>(this IEnumerable<Either<L, R>> self, Action<R> action)
        {
            foreach (var item in self)
            {
                item.Iter(action);
            }
            return unit;
        }

        public static int CountT<L, R>(this IEnumerable<Either<L, R>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<L, R, S>(this IEnumerable<Either<L, R>> self, S state, Func<S, R, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static IEnumerable<Either<L, R2>> MapT<L, R, R2>(this IEnumerable<Either<L, R>> self, Func<R, R2> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static IEnumerable<Either<Unit, R>> FilterT<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static IEnumerable<Either<L, R2>> BindT<L, R, R2>(this IEnumerable<Either<L, R>> self, Func<R, Either<L, R2>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static IEnumerable<Either<L, R2>> Select<L, R, R2>(this IEnumerable<Either<L, R>> self, Func<R, R2> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Either<L, R3>> SelectMany<L, R, R2, R3>(this IEnumerable<Either<L, R>> self,
            Func<R, Either<L, R2>> bind,
            Func<R, R2, R3> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static IEnumerable<Either<Unit, R>> Where<L, R>(this IEnumerable<Either<L, R>> self, Func<R, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        // 
        // IEnumerable<Writer<Out,T>> extensions 
        // 

        public static Unit IterT<Out, T>(this IEnumerable<Writer<Out, T>> self, Action<T> action)
        {
            foreach (var x in self)
            {
                x.IterT(action);
            }
            return unit;
        }

        public static int CountT<Out, T>(this IEnumerable<Writer<Out, T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<Out, T>(this IEnumerable<Writer<Out, T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred))
                {
                    return false;
                }
            }
            return true;
        }

        public static S FoldT<Out, S, T>(this IEnumerable<Writer<Out, T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<Out, T>(this IEnumerable<Writer<Out, T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<Writer<Out, R>> MapT<Out, T, R>(this IEnumerable<Writer<Out, T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static IEnumerable<Writer<Out, R>> BindT<Out, T, R>(this IEnumerable<Writer<Out, T>> self, Func<T, Writer<Out, R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static IEnumerable<Writer<Out, U>> Select<Out, T, U>(this IEnumerable<Writer<Out, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Writer<Out, V>> SelectMany<Out, T, U, V>(this IEnumerable<Writer<Out, T>> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        // 
        // IEnumerable<Reader<Env,T>> extensions 
        // 

        public static Reader<Env, Unit> IterT<Env, T>(this IEnumerable<Reader<Env, T>> self, Action<T> action)
        {
            return env =>
            {
                foreach (var x in self)
                {
                    x.IterT(action);
                }
                return unit;
            };
        }

        public static int CountT<Env, T>(this IEnumerable<Reader<Env, T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static Reader<Env, bool> ForAllT<Env, T>(this IEnumerable<Reader<Env, T>> self, Func<T, bool> pred)
        {
            return env =>
            {
                foreach (var x in self)
                {
                    if (!x.ForAllT(pred)(env))
                    {
                        return false;
                    }
                }
                return true;
            };
        }

        public static Reader<Env, S> FoldT<Env, S, T>(this IEnumerable<Reader<Env, T>> self, S state, Func<S, T, S> folder)
        {
            return env =>
            {
                foreach (var x in self)
                {
                    state = x.FoldT(state, folder)(env);
                }
                return state;
            };
        }

        public static Reader<Env, bool> ExistsT<Env, T>(this IEnumerable<Reader<Env, T>> self, Func<T, bool> pred)
        {
            return env =>
            {
                foreach (var x in self)
                {
                    if (x.ExistsT(pred)(env))
                    {
                        return true;
                    }
                }
                return false;
            };
        }

        public static IEnumerable<Reader<Env, R>> MapT<Env, T, R>(this IEnumerable<Reader<Env, T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static IEnumerable<Reader<Env, R>> BindT<Env, T, R>(this IEnumerable<Reader<Env, T>> self, Func<T, Reader<Env, R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static IEnumerable<Reader<Env, U>> Select<Env, T, U>(this IEnumerable<Reader<Env, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<Reader<Env, V>> SelectMany<Env, T, U, V>(this IEnumerable<Reader<Env, T>> self,
            Func<T, Reader<Env, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));


        // 
        // IEnumerable<State<S,T>> extensions 
        // 

        public static State<S, Unit> IterT<S, T>(this IEnumerable<State<S, T>> self, Action<T> action)
        {
            return s =>
            {
                foreach (var x in self)
                {
                    x.Iter(action)(s);
                }
                return new StateResult<S, Unit>(s, unit);
            };
        }

        public static int CountT<S, T>(this IEnumerable<State<S, T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static State<S, bool> ForAllT<S, T>(this IEnumerable<State<S, T>> self, Func<T, bool> pred)
        {
            return s =>
            {
                foreach (var x in self)
                {
                    if (!x.ForAllT(pred)(s).Value)
                    {
                        return new StateResult<S, bool>(s, true);
                    }
                }
                return new StateResult<S, bool>(s, true);
            };
        }

        public static State<S, FState> FoldT<S, T, FState>(this IEnumerable<State<S, T>> self, FState state, Func<FState, T, FState> folder)
        {
            return s =>
            {
                foreach (var x in self)
                {
                    state = x.FoldT(state, folder)(s).Value;
                }
                return new StateResult<S, FState>(s, state);
            };
        }

        public static State<S, Unit> FoldT<S, T>(this IEnumerable<State<S, T>> self, Func<S, T, S> folder)
        {
            return s =>
            {
                foreach (var x in self)
                {
                    s = x.FoldT(folder)(s).State;
                }
                return new StateResult<S, Unit>(s, unit);
            };
        }

        public static State<S, bool> ExistsT<S, T>(this IEnumerable<State<S, T>> self, Func<T, bool> pred)
        {
            return s =>
            {
                foreach (var x in self)
                {
                    if (x.ExistsT(pred)(s).Value)
                    {
                        return new StateResult<S, bool>(s, true);
                    }
                }
                return new StateResult<S, bool>(s, false);
            };
        }

        public static IEnumerable<State<S, R>> MapT<S, T, R>(this IEnumerable<State<S, T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.Map(mapper));

        public static IEnumerable<State<S, R>> BindT<S, T, R>(this IEnumerable<State<S, T>> self, Func<T, State<S, R>> binder) =>
            self.MapT(x => x.Bind(binder));

        public static IEnumerable<State<S, U>> Select<S, T, U>(this IEnumerable<State<S, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static IEnumerable<State<S, V>> SelectMany<S, T, U, V>(this IEnumerable<State<S, T>> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));
    }
}