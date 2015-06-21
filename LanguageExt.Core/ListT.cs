using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class ListT
    {
        public static S FoldT<S, T>(this Lst<T> list, S state, Func<S, T, S> folder) =>
            list.Fold(state, folder);

        public static S FoldBackT<S, T>(this Lst<T> list, S state, Func<S, T, S> folder) =>
            list.FoldBack(state, folder);

        public static T ReduceT<T>(this Lst<T> list, Func<T, T, T> reducer) =>
            list.Reduce(reducer);

        public static T ReduceBackT<T>(this Lst<T> list, Func<T, T, T> reducer) =>
            list.ReduceBack(reducer);

        public static Lst<S> ScanT<S, T>(this Lst<T> list, S state, Func<S, T, S> folder) =>
            list.Scan(state, folder).Freeze();

        public static IEnumerable<S> ScanBackT<S, T>(this Lst<T> list, S state, Func<S, T, S> folder) =>
            list.ScanBack(state, folder);

        public static Option<T> FindT<T>(this Lst<T> list, Func<T, bool> pred) =>
            list.Find(pred);

        public static int CountT<T>(this Lst<T> list) =>
            list.Count();

        public static Unit IterT<T>(this Lst<T> list, Action<T> action) =>
            list.Iter(action);

        public static Unit IterT<T>(this Lst<T> list, Action<int, T> action) =>
            list.Iter(action);

        public static bool ForAllT<T>(this Lst<T> list, Func<T, bool> pred) =>
            list.ForAll(pred);

        public static Lst<T> DistinctT<T>(this Lst<T> list, Func<T, T, bool> compare) =>
            list.Distinct(compare).Freeze();

        public static bool ExistsT<T>(this Lst<T> list, Func<T, bool> pred) =>
            list.Exists(pred);

        public static Lst<R> MapT<T, R>(this Lst<T> list, Func<T, R> map) =>
            list.Map(map).Freeze();

        public static Lst<T> FilterT<T>(this Lst<T> list, Func<T, bool> pred) =>
            list.Filter(pred).Freeze();

        public static Lst<R> BindT<T, R>(this Lst<T> self, Func<T, Lst<R>> binder) =>
            self.Bind(binder).Freeze();

        // 
        // IEnumerable<IEnumerable<T>> extensions 
        // 

        public static Unit IterT<T>(this Lst<IEnumerable<T>> self, Action<T> action) =>
            (from x in self
             from y in x
             select y).IterT(action);

        public static int CountT<T>(this Lst<IEnumerable<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this Lst<IEnumerable<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this Lst<IEnumerable<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this Lst<IEnumerable<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Lst<IEnumerable<R>> MapT<T, R>(this Lst<IEnumerable<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Lst<IEnumerable<T>> FilterT<T>(this Lst<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<IEnumerable<R>> BindT<T, R>(this Lst<IEnumerable<T>> self, Func<T, IEnumerable<R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static Lst<IEnumerable<U>> Select<T, U>(this Lst<IEnumerable<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<IEnumerable<V>> SelectMany<T, U, V>(this Lst<IEnumerable<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<IEnumerable<T>> WhereT<T>(this Lst<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        // 
        // IEnumerable<Lst<T>> extensions 
        // 

        public static Unit IterT<T>(this Lst<Lst<T>> self, Action<T> action) =>
            (from x in self
             from y in x
             select y).IterT(action);

        public static int CountT<T>(this Lst<Lst<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this Lst<Lst<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this Lst<Lst<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this Lst<Lst<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Lst<Lst<R>> MapT<T, R>(this Lst<Lst<T>> self, Func<T, R> mapper) =>
            self.MapT(x => List.createRange(x.Map(mapper)));

        public static Lst<Lst<T>> FilterT<T>(this Lst<Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));

        public static Lst<Lst<R>> BindT<T, R>(this Lst<Lst<T>> self, Func<T, Lst<R>> binder) =>
            self.MapT(x => List.createRange(x.BindT(binder)));

        public static Lst<Lst<U>> Select<T, U>(this Lst<Lst<T>> self, Func<T, U> map) =>
            self.MapT(x => List.createRange(x.MapT(map)));

        public static Lst<Lst<V>> SelectMany<T, U, V>(this Lst<Lst<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => List.createRange(x.SelectMany(bind, project)));

        public static Lst<Lst<T>> Where<T>(this Lst<Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));

        // 
        // IEnumerable<Map<T>> extensions 
        // 

        public static Unit IterT<K, V>(this Lst<Map<K, V>> self, Action<V> action)
        {
            foreach (var x in self)
            {
                x.IterT(action);
            }
            return Unit.Default;
        }

        public static Unit IterT<K, V>(this Lst<Map<K, V>> self, Action<K, V> action)
        {
            foreach (var x in self)
            {
                x.IterT(action);
            }
            return Unit.Default;
        }

        public static int CountT<K, V>(this Lst<Map<K, V>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAll<K, V>(this Lst<Map<K, V>> self, Func<K, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static bool ForAllT<K, V>(this Lst<Map<K, V>> self, Func<V, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static bool ForAllT<K, V>(this Lst<Map<K, V>> self, Func<K, V, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<K, V, S>(this Lst<Map<K, V>> self, S state, Func<S, K, V, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static S FoldT<K, V, S>(this Lst<Map<K, V>> self, S state, Func<S, V, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static S FoldT<K, V, S>(this Lst<Map<K, V>> self, S state, Func<S, K, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<K, V>(this Lst<Map<K, V>> self, Func<K, V, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static bool ExistsT<K, V>(this Lst<Map<K, V>> self, Func<K, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static bool ExistsT<K, V>(this Lst<Map<K, V>> self, Func<V, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Lst<Map<K, R>> MapT<K, V, R>(this Lst<Map<K, V>> self, Func<K, V, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Lst<Map<K, R>> MapT<K, V, R>(this Lst<Map<K, V>> self, Func<V, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Lst<Map<K, V>> FilterT<K, V>(this Lst<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<Map<K, V>> FilterT<K, V>(this Lst<Map<K, V>> self, Func<V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<Map<K, V>> FilterT<K, V>(this Lst<Map<K, V>> self, Func<K, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<Map<K, V>> Where<K, V>(this Lst<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<Map<K, V>> Where<K, V>(this Lst<Map<K, V>> self, Func<K, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<Map<K, V>> Where<K, V>(this Lst<Map<K, V>> self, Func<V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        // 
        // IEnumerable<Option<T>> extensions 
        // 

        public static Unit IterT<T>(this Lst<Option<T>> self, Action<T> action)
        {
            foreach (var item in self)
            {
                item.IterT(action);
            }
            return unit;
        }

        public static int CountT<T>(this Lst<Option<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this Lst<Option<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this Lst<Option<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this Lst<Option<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Lst<Option<R>> MapT<T, R>(this Lst<Option<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Lst<Option<T>> FilterT<T>(this Lst<Option<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<Option<R>> BindT<T, R>(this Lst<Option<T>> self, Func<T, Option<R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static Lst<Option<U>> Select<T, U>(this Lst<Option<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Option<V>> SelectMany<T, U, V>(this Lst<Option<T>> self,
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<Option<T>> Where<T>(this Lst<Option<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        // 
        // IEnumerable<TryOption<T>> extensions 
        // 

        public static Unit IterT<T>(this Lst<TryOption<T>> self, Action<T> action)
        {
            foreach (var item in self)
            {
                item.IterT(action);
            }
            return unit;
        }

        public static int CountT<T>(this Lst<TryOption<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this Lst<TryOption<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this Lst<TryOption<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this Lst<TryOption<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Lst<TryOption<R>> MapT<T, R>(this Lst<TryOption<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.Map(mapper));

        public static Lst<TryOption<T>> FilterT<T>(this Lst<TryOption<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.Filter(pred));

        public static Lst<TryOption<R>> BindT<T, R>(this Lst<TryOption<T>> self, Func<T, TryOption<R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static Lst<TryOption<U>> Select<T, U>(this Lst<TryOption<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<TryOption<V>> SelectMany<T, U, V>(this Lst<TryOption<T>> self,
            Func<T, TryOption<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<TryOption<T>> Where<T>(this Lst<TryOption<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        // 
        // IEnumerable<Try<T>> extensions 
        // 

        public static Unit IterT<T>(this Lst<Try<T>> self, Action<T> action)
        {
            foreach (var item in self)
            {
                item.IterT(action);
            }
            return unit;
        }

        public static int CountT<T>(this Lst<Try<T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<T>(this Lst<Try<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<T, S>(this Lst<Try<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<T>(this Lst<Try<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Lst<Try<R>> MapT<T, R>(this Lst<Try<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Lst<Try<T>> FilterT<T>(this Lst<Try<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<Try<R>> BindT<T, R>(this Lst<Try<T>> self, Func<T, Try<R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static Lst<Try<U>> Select<T, U>(this Lst<Try<T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Try<V>> SelectMany<T, U, V>(this Lst<Try<T>> self,
            Func<T, Try<U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<Try<T>> Where<T>(this Lst<Try<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        // 
        // IEnumerable<Either<L,R>> extensions 
        // 

        public static Unit IterT<L, R>(this Lst<Either<L, R>> self, Action<R> action)
        {
            foreach (var item in self)
            {
                item.Iter(action);
            }
            return unit;
        }

        public static int CountT<L, R>(this Lst<Either<L, R>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<L, R>(this Lst<Either<L, R>> self, Func<R, bool> pred)
        {
            foreach (var x in self)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<L, R, S>(this Lst<Either<L, R>> self, S state, Func<S, R, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<L, R>(this Lst<Either<L, R>> self, Func<R, bool> pred)
        {
            foreach (var x in self)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Lst<Either<L, R2>> MapT<L, R, R2>(this Lst<Either<L, R>> self, Func<R, R2> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Lst<Either<Unit, R>> FilterT<L, R>(this Lst<Either<L, R>> self, Func<R, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Lst<Either<L, R2>> BindT<L, R, R2>(this Lst<Either<L, R>> self, Func<R, Either<L, R2>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static Lst<Either<L, R2>> Select<L, R, R2>(this Lst<Either<L, R>> self, Func<R, R2> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Either<L, R3>> SelectMany<L, R, R2, R3>(this Lst<Either<L, R>> self,
            Func<R, Either<L, R2>> bind,
            Func<R, R2, R3> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        public static Lst<Either<Unit, R>> Where<L, R>(this Lst<Either<L, R>> self, Func<R, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        // 
        // IEnumerable<Writer<Out,T>> extensions 
        // 

        public static Unit IterT<Out, T>(this Lst<Writer<Out, T>> self, Action<T> action)
        {
            foreach (var x in self)
            {
                x.IterT(action);
            }
            return unit;
        }

        public static int CountT<Out, T>(this Lst<Writer<Out, T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static bool ForAllT<Out, T>(this Lst<Writer<Out, T>> self, Func<T, bool> pred)
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

        public static S FoldT<Out, S, T>(this Lst<Writer<Out, T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<Out, T>(this Lst<Writer<Out, T>> self, Func<T, bool> pred)
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

        public static Lst<Writer<Out, R>> MapT<Out, T, R>(this Lst<Writer<Out, T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Lst<Writer<Out, R>> BindT<Out, T, R>(this Lst<Writer<Out, T>> self, Func<T, Writer<Out, R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static Lst<Writer<Out, U>> Select<Out, T, U>(this Lst<Writer<Out, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Writer<Out, V>> SelectMany<Out, T, U, V>(this Lst<Writer<Out, T>> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));

        // 
        // IEnumerable<Reader<Env,T>> extensions 
        // 

        public static Reader<Env, Unit> IterT<Env, T>(this Lst<Reader<Env, T>> self, Action<T> action)
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

        public static int CountT<Env, T>(this Lst<Reader<Env, T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static Reader<Env, bool> ForAllT<Env, T>(this Lst<Reader<Env, T>> self, Func<T, bool> pred)
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

        public static Reader<Env, S> FoldT<Env, S, T>(this Lst<Reader<Env, T>> self, S state, Func<S, T, S> folder)
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

        public static Reader<Env, bool> ExistsT<Env, T>(this Lst<Reader<Env, T>> self, Func<T, bool> pred)
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

        public static Lst<Reader<Env, R>> MapT<Env, T, R>(this Lst<Reader<Env, T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Lst<Reader<Env, R>> BindT<Env, T, R>(this Lst<Reader<Env, T>> self, Func<T, Reader<Env, R>> binder) =>
            self.MapT(x => x.BindT(binder));

        public static Lst<Reader<Env, U>> Select<Env, T, U>(this Lst<Reader<Env, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<Reader<Env, V>> SelectMany<Env, T, U, V>(this Lst<Reader<Env, T>> self,
            Func<T, Reader<Env, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));


        // 
        // IEnumerable<State<S,T>> extensions 
        // 

        public static State<S, Unit> IterT<S, T>(this Lst<State<S, T>> self, Action<T> action)
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

        public static int CountT<S, T>(this Lst<State<S, T>> self) =>
            (from x in self
             select x.CountT()).Sum();

        public static State<S, bool> ForAllT<S, T>(this Lst<State<S, T>> self, Func<T, bool> pred)
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

        public static State<S, FState> FoldT<S, T, FState>(this Lst<State<S, T>> self, FState state, Func<FState, T, FState> folder)
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

        public static State<S, Unit> FoldT<S, T>(this Lst<State<S, T>> self, Func<S, T, S> folder)
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

        public static State<S, bool> ExistsT<S, T>(this Lst<State<S, T>> self, Func<T, bool> pred)
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

        public static Lst<State<S, R>> MapT<S, T, R>(this Lst<State<S, T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.Map(mapper));

        public static Lst<State<S, R>> BindT<S, T, R>(this Lst<State<S, T>> self, Func<T, State<S, R>> binder) =>
            self.MapT(x => x.Bind(binder));

        public static Lst<State<S, U>> Select<S, T, U>(this Lst<State<S, T>> self, Func<T, U> map) =>
            self.MapT(x => x.MapT(map));

        public static Lst<State<S, V>> SelectMany<S, T, U, V>(this Lst<State<S, T>> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            ) =>
            self.MapT(x => x.SelectMany(bind, project));
    }
}