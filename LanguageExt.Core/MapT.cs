using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class MapTrans
    {
        public static Map<K, U> MapT<K, V, U>(this Map<K, V> self, Func<V, U> mapper) =>
            MapModule.Map(self, mapper);

        public static Map<K, U> MapT<K, V, U>(this Map<K, V> self, Func<K, V, U> mapper) =>
            MapModule.Map(self, mapper);

        public static int CountT<K, V>(this Map<K, V> self) =>
            self.Count;

        public static Map<K, V> FilterT<K, V>(this Map<K, V> self, Func<V, bool> pred) =>
            MapModule.Filter(self, pred);

        public static Map<K, V> FilterT<K, V>(this Map<K, V> self, Func<K, bool> pred) =>
            MapModule.Filter(self, pred);

        public static Map<K, V> FilterT<K, V>(this Map<K, V> self, Func<K, V, bool> pred) =>
            MapModule.Filter(self, pred);

        public static bool ForAllT<K, V>(this Map<K, V> self, Func<K, V, bool> pred) =>
            MapModule.ForAll(self, pred);

        public static bool ForAllT<K, V>(this Map<K, V> self, Func<Tuple<K, V>, bool> pred) =>
            MapModule.ForAll(self, (k, v) => pred(new Tuple<K, V>(k, v)));

        public static bool ForAllT<K, V>(this Map<K, V> self, Func<KeyValuePair<K, V>, bool> pred) =>
            MapModule.ForAll(self, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        public static bool ForAllT<K, V>(this Map<K, V> self, Func<K, bool> pred) =>
            MapModule.ForAll(self, (k, v) => pred(k));

        public static bool ForAllT<K, V>(this Map<K, V> self, Func<V, bool> pred) =>
            MapModule.ForAll(self, (k, v) => pred(v));

        public static bool ExistsT<K, V>(this Map<K, V> self, Func<K, V, bool> pred) =>
            MapModule.Exists(self, pred);

        public static bool ExistsT<K, V>(this Map<K, V> self, Func<Tuple<K, V>, bool> pred) =>
            MapModule.Exists(self, (k, v) => pred(new Tuple<K, V>(k, v)));

        public static bool ExistsT<K, V>(this Map<K, V> self, Func<KeyValuePair<K, V>, bool> pred) =>
            MapModule.Exists(self, (k, v) => pred(new KeyValuePair<K, V>(k, v)));

        public static bool ExistsT<K, V>(this Map<K, V> self, Func<K, bool> pred) =>
            MapModule.Exists(self, (k, _) => pred(k));

        public static bool ExistsT<K, V>(this Map<K, V> self, Func<V, bool> pred) =>
            MapModule.Exists(self, (_, v) => pred(v));

        public static Unit IterT<K, V>(this Map<K, V> self, Action<K, V> action) =>
            MapModule.Iter(self, action);

        public static Unit IterT<K, V>(this Map<K, V> self, Action<V> action) =>
            MapModule.Iter(self, action);

        public static Unit IterT<K, V>(this Map<K, V> self, Action<Tuple<K, V>> action) =>
            MapModule.Iter(self, (k, v) => action(new Tuple<K, V>(k, v)));

        public static Unit IterT<K, V>(this Map<K, V> self, Action<KeyValuePair<K, V>> action) =>
            MapModule.Iter(self, (k, v) => action(new KeyValuePair<K, V>(k, v)));

        public static Map<K, V> ChooseT<K, V>(this Map<K, V> self, Func<K, V, Option<V>> selector) =>
            MapModule.Choose(self, selector);

        public static Map<K, V> ChooseT<K, V>(this Map<K, V> self, Func<V, Option<V>> selector) =>
            MapModule.Choose(self, selector);

        public static S FoldT<K, V, S>(this Map<K, V> self, S state, Func<S, K, V, S> folder) =>
            MapModule.Fold(self, state, folder);

        public static S FoldT<K, V, S>(this Map<K, V> self, S state, Func<S, V, S> folder) =>
            MapModule.Fold(self, state, folder);

        public static S FoldT<K, V, S>(this Map<K, V> self, S state, Func<S, K, S> folder) =>
            MapModule.Fold(self, state, folder);


        //
        // Map<A<Map<B,C>>
        //

        public static Option<T> Find<A, B, T>(this Map<A, Map<B, T>> self, A outerKey, B innerKey) =>
            self.Find(outerKey, b => b.Find(innerKey), () => None);

        public static R Find<A, B, T, R>(this Map<A, Map<B, T>> self, A outerKey, B innerKey, Func<T, R> Some, Func<R> None) =>
            self.Find(outerKey, b => b.Find(innerKey, Some, None), None);

        public static Map<A, Map<B, T>> AddOrUpdate<A, B, T>(this Map<A, Map<B, T>> self, A outerKey, B innerKey, Func<T, T> Some, Func<T> None) =>
            self.AddOrUpdate(
                outerKey,
                b => b.AddOrUpdate(innerKey, Some, None),
                () => map(tuple(innerKey, None()))
            );

        public static Map<A, Map<B, T>> AddOrUpdate<A, B, T>(this Map<A, Map<B, T>> self, A outerKey, B innerKey, T value) =>
            self.AddOrUpdate(
                outerKey,
                b => b.AddOrUpdate(innerKey, _ => value, value),
                () => map(tuple(innerKey, value))
            );

        public static Map<A, Map<B, T>> Remove<A, B, T>(this Map<A, Map<B, T>> self, A outerKey, B innerKey)
        {
            var b = self.Find(outerKey);
            if (b.IsSome)
            {
                var bv = b.Value.Remove(innerKey);
                if (bv.Count() == 0)
                {
                    return self.Remove(outerKey);
                }
                else
                {
                    return self.SetItem(outerKey, bv);
                }
            }
            else
            {
                return self;
            }
        }

        // 
        // Map<K, IEnumerable<T>> extensions 
        // 

        public static Unit IterT<K, T>(this Map<K, IEnumerable<T>> self, Action<T> action) =>
            (from x in self.Values
             from y in x
             select y).IterT(action);

        public static int CountT<K, T>(this Map<K, IEnumerable<T>> self) =>
            (from x in self.Values
             select x.CountT()).Sum();

        public static bool ForAllT<K, T>(this Map<K, IEnumerable<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<K, T, S>(this Map<K, IEnumerable<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<K, T>(this Map<K, IEnumerable<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Map<K, IEnumerable<R>> MapT<K, T, R>(this Map<K, IEnumerable<T>> self, Func<T, R> mapper) =>
            self.MapT(x => x.Map(mapper));

        public static Map<K, IEnumerable<T>> FilterT<K, T>(this Map<K, IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.Filter(pred));

        public static Map<K, IEnumerable<R>> BindT<K, T, R>(this Map<K, IEnumerable<T>> self, Func<T, IEnumerable<R>> binder) =>
            self.MapT(x => x.Bind(binder));

        public static Map<K, IEnumerable<U>> Select<K, T, U>(this Map<K, IEnumerable<T>> self, Func<T, U> map) =>
            self.MapT(map);

        public static Map<K, IEnumerable<T>> Where<K, T>(this Map<K, IEnumerable<T>> self, Func<T, bool> pred) =>
            self.FilterT(pred);

        // 
        // Map<Lst<T>> extensions 
        // 

        public static Unit IterT<K, T>(this Map<K, Lst<T>> self, Action<T> action) =>
            (from x in self.Values
             from y in x
             select y).Iter(action);

        public static int CountT<K, T>(this Map<K, Lst<T>> self) =>
            (from x in self.Values
             select x.Count()).Sum();

        public static bool ForAllT<K, T>(this Map<K, Lst<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<K, T, S>(this Map<K, Lst<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<K, T>(this Map<K, Lst<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Map<K, Lst<R>> MapT<K, T, R>(this Map<K, Lst<T>> self, Func<T, R> mapper) =>
            self.MapT(x => List.createRange(x.MapT(mapper)));

        public static Map<K, Lst<T>> FilterT<K, T>(this Map<K, Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));

        public static Map<K, Lst<R>> BindT<K, T, R>(this Map<K, Lst<T>> self, Func<T, Lst<R>> binder) =>
            self.MapT(x => List.createRange(x.BindT(binder)));

        public static Map<K, Lst<U>> Select<K, T, U>(this Map<K, Lst<T>> self, Func<T, U> map) =>
            self.MapT(x => List.createRange(x.MapT(map)));

        public static Map<K, Lst<T>> Where<K, T>(this Map<K, Lst<T>> self, Func<T, bool> pred) =>
            self.MapT(x => List.createRange(x.FilterT(pred)));

        // 
        // Map<Map<T>> extensions 
        // 

        public static Unit IterT<K, J, V>(this Map<K, Map<J, V>> self, Action<V> action)
        {
            foreach (var x in self.Values)
            {
                x.IterT(action);
            }
            return Unit.Default;
        }

        public static Unit IterT<K, J, V>(this Map<K, Map<J, V>> self, Action<J, V> action)
        {
            foreach (var x in self.Values)
            {
                x.IterT(action);
            }
            return Unit.Default;
        }

        public static Unit IterT<K, J, V>(this Map<K, Map<J, V>> self, Action<K, J, V> action)
        {
            foreach (var x in self)
            {
                x.Item2.IterT((j, v) => action(x.Item1, j, v));
            }
            return Unit.Default;
        }

        public static int Countv<K, J, V>(this Map<K, Map<J, V>> self)
        {
            int sum = 0;
            foreach (var x in self.Values)
            {
                sum += x.CountT();
            }
            return sum;
        }

        public static bool ForAllT<K, J, V>(this Map<K, Map<J, V>> self, Func<J, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static bool ForAllT<K, J, V>(this Map<K, Map<J, V>> self, Func<V, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static bool ForAllT<K, J, V>(this Map<K, Map<J, V>> self, Func<J, V, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAllT(pred)) return false;
            }
            return true;
        }

        public static S FoldT<K, J, V, S>(this Map<K, Map<J, V>> self, S state, Func<S, J, V, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static S FoldT<K, V, J, S>(this Map<K, Map<J, V>> self, S state, Func<S, V, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.Fold(state, folder);
            }
            return state;
        }

        public static S FoldT<K, V, J, S>(this Map<K, Map<J, V>> self, S state, Func<S, J, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.FoldT(state, folder);
            }
            return state;
        }

        public static bool ExistsT<K, J, V>(this Map<K, Map<J, V>> self, Func<J, V, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static bool ExistsT<K, J, V>(this Map<K, Map<J, V>> self, Func<J, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static bool ExistsT<K, J, V>(this Map<K, Map<J, V>> self, Func<V, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.ExistsT(pred)) return true;
            }
            return false;
        }

        public static Map<K, Map<J, R>> MapT<K, J, V, R>(this Map<K, Map<J, V>> self, Func<J, V, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Map<K, Map<J, R>> MapT<K, J, V, R>(this Map<K, Map<J, V>> self, Func<V, R> mapper) =>
            self.MapT(x => x.MapT(mapper));

        public static Map<K, Map<J, V>> FilterT<K, J, V>(this Map<K, Map<J, V>> self, Func<J, V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Map<K, Map<J, V>> FilterT<K, J, V>(this Map<K, Map<J, V>> self, Func<V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Map<K, Map<J, V>> FilterT<K, J, V>(this Map<K, Map<J, V>> self, Func<J, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Map<K, Map<J, V>> Where<K, J, V>(this Map<K, Map<J, V>> self, Func<J, V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Map<K, Map<J, V>> Where<K, J, V>(this Map<K, Map<J, V>> self, Func<J, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        public static Map<K, Map<J, V>> Where<K, J, V>(this Map<K, Map<J, V>> self, Func<V, bool> pred) =>
            self.MapT(x => x.FilterT(pred));


        // 
        // Map<K, Option<T>> extensions 
        // 

        public static Unit Iter<K, T>(this Map<K, Option<T>> self, Action<T> action)
        {
            foreach (var item in self.Values)
            {
                item.Iter(action);
            }
            return unit;
        }

        public static int Count<K, T>(this Map<K, Option<T>> self) =>
            (from x in self.Values
             select x.Count()).Sum();

        public static bool ForAll<K, T>(this Map<K, Option<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAll(pred)) return false;
            }
            return true;
        }

        public static S Fold<K, T, S>(this Map<K, Option<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.Fold(state, folder);
            }
            return state;
        }

        public static bool Exists<K, T>(this Map<K, Option<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.Exists(pred)) return true;
            }
            return false;
        }

        public static Map<K, Option<R>> Map<K, T, R>(this Map<K, Option<T>> self, Func<T, R> mapper) =>
            self.Map(x => x.Map(mapper));

        public static Map<K, Option<T>> Filter<K, T>(this Map<K, Option<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));

        public static Map<K, Option<R>> Bind<K, T, R>(this Map<K, Option<T>> self, Func<T, Option<R>> binder) =>
            self.Map(x => x.Bind(binder));

        public static Map<K, Option<U>> Select<K, T, U>(this Map<K, Option<T>> self, Func<T, U> map) =>
            self.Map(x => x.Select(map));

        public static Map<K, Option<V>> SelectMany<K, T, U, V>(this Map<K, Option<T>> self,
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, Option<T>> Where<K, T>(this Map<K, Option<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));

        // 
        // IEnumerable<TryOption<T>> extensions 
        // 

        public static Unit Iter<K, T>(this Map<K, TryOption<T>> self, Action<T> action)
        {
            foreach (var item in self.Values)
            {
                item.Iter(action);
            }
            return unit;
        }

        public static int Count<K, T>(this Map<K, TryOption<T>> self) =>
            (from x in self.Values
             select x.Count()).Sum();

        public static bool ForAll<K, T>(this Map<K, TryOption<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAll(pred)) return false;
            }
            return true;
        }

        public static S Fold<K, T, S>(this Map<K, TryOption<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.Fold(state, folder);
            }
            return state;
        }

        public static bool Exists<K, T>(this Map<K, TryOption<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.Exists(pred)) return true;
            }
            return false;
        }

        public static Map<K, TryOption<R>> Map<K, T, R>(this Map<K, TryOption<T>> self, Func<T, R> mapper) =>
            self.Map(x => x.Map(mapper));

        public static Map<K, TryOption<T>> Filter<K, T>(this Map<K, TryOption<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));

        public static Map<K, TryOption<R>> Bind<K, T, R>(this Map<K, TryOption<T>> self, Func<T, TryOption<R>> binder) =>
            self.Map(x => x.Bind(binder));

        public static Map<K, TryOption<U>> Select<K, T, U>(this Map<K, TryOption<T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, TryOption<V>> SelectMany<K, T, U, V>(this Map<K, TryOption<T>> self,
            Func<T, TryOption<U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, TryOption<T>> Where<K, T>(this Map<K, TryOption<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));

        // 
        // IEnumerable<Try<T>> extensions 
        // 

        public static Unit Iter<K, T>(this Map<K, Try<T>> self, Action<T> action)
        {
            foreach (var item in self.Values)
            {
                item.Iter(action);
            }
            return unit;
        }

        public static int Count<K, T>(this Map<K, Try<T>> self) =>
            (from x in self.Values
             select x.Count()).Sum();

        public static bool ForAll<K, T>(this Map<K, Try<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAll(pred)) return false;
            }
            return true;
        }

        public static S Fold<K, T, S>(this Map<K, Try<T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.Fold(state, folder);
            }
            return state;
        }

        public static bool Exists<K, T>(this Map<K, Try<T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.Exists(pred)) return true;
            }
            return false;
        }

        public static Map<K, Try<R>> Map<K, T, R>(this Map<K, Try<T>> self, Func<T, R> mapper) =>
            self.Map(x => x.Map(mapper));

        public static Map<K, Try<T>> Filter<K, T>(this Map<K, Try<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));

        public static Map<K, Try<R>> Bind<K, T, R>(this Map<K, Try<T>> self, Func<T, Try<R>> binder) =>
            self.Map(x => x.Bind(binder));

        public static Map<K, Try<U>> Select<K, T, U>(this Map<K, Try<T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, Try<V>> SelectMany<K, T, U, V>(this Map<K, Try<T>> self,
            Func<T, Try<U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, Try<T>> Where<K, T>(this Map<K, Try<T>> self, Func<T, bool> pred) =>
            self.Map(x => x.Filter(pred));


        // 
        // IEnumerable<Either<L,R>> extensions 
        // 

        public static Unit Iter<K, L, R>(this Map<K, Either<L, R>> self, Action<R> action)
        {
            foreach (var item in self.Values)
            {
                item.Iter(action);
            }
            return unit;
        }

        public static int Count<K, L, R>(this Map<K, Either<L, R>> self) =>
            (from x in self.Values
             select x.Count()).Sum();

        public static bool ForAll<K, L, R>(this Map<K, Either<L, R>> self, Func<R, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAll(pred)) return false;
            }
            return true;
        }

        public static S Fold<K, L, R, S>(this Map<K, Either<L, R>> self, S state, Func<S, R, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.Fold(state, folder);
            }
            return state;
        }

        public static bool Exists<K, L, R>(this Map<K, Either<L, R>> self, Func<R, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.Exists(pred)) return true;
            }
            return false;
        }

        public static Map<K, Either<L, R2>> Map<K, L, R, R2>(this Map<K, Either<L, R>> self, Func<R, R2> mapper) =>
            self.Map(x => x.Map(mapper));

        public static Map<K, Either<Unit, R>> Filter<K, L, R>(this Map<K, Either<L, R>> self, Func<R, bool> pred) =>
            self.Map(x => x.Filter(pred));

        public static Map<K, Either<L, R2>> Bind<K, L, R, R2>(this Map<K, Either<L, R>> self, Func<R, Either<L, R2>> binder) =>
            self.Map(x => x.Bind(binder));

        public static Map<K, Either<L, R2>> Select<K, L, R, R2>(this Map<K, Either<L, R>> self, Func<R, R2> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, Either<L, R3>> SelectMany<K, L, R, R2, R3>(this Map<K, Either<L, R>> self,
            Func<R, Either<L, R2>> bind,
            Func<R, R2, R3> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        public static Map<K, Either<Unit, R>> Where<K, L, R>(this Map<K, Either<L, R>> self, Func<R, bool> pred) =>
            self.Map(x => x.Filter(pred));

        // 
        // Map<K, Writer<Out,T>> extensions 
        // 

        public static Unit Iter<K, Out, T>(this Map<K, Writer<Out, T>> self, Action<T> action)
        {
            foreach (var x in self.Values)
            {
                x.Iter(action);
            }
            return unit;
        }

        public static int Count<K, Out, T>(this Map<K, Writer<Out, T>> self) =>
            (from x in self.Values
             select x.Count()).Sum();

        public static bool ForAll<K, Out, T>(this Map<K, Writer<Out, T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (!x.ForAll(pred))
                {
                    return false;
                }
            }
            return true;
        }

        public static S Fold<K, Out, S, T>(this Map<K, Writer<Out, T>> self, S state, Func<S, T, S> folder)
        {
            foreach (var x in self.Values)
            {
                state = x.Fold(state, folder);
            }
            return state;
        }

        public static bool Exists<K, Out, T>(this Map<K, Writer<Out, T>> self, Func<T, bool> pred)
        {
            foreach (var x in self.Values)
            {
                if (x.Exists(pred))
                {
                    return true;
                }
            }
            return false;
        }

        public static Map<K, Writer<Out, R>> Map<K, Out, T, R>(this Map<K, Writer<Out, T>> self, Func<T, R> mapper) =>
            self.Map(x => x.Map(mapper));

        public static Map<K, Writer<Out, R>> Bind<K, Out, T, R>(this Map<K, Writer<Out, T>> self, Func<T, Writer<Out, R>> binder) =>
            self.Map(x => x.Bind(binder));

        public static Map<K, Writer<Out, U>> Select<K, Out, T, U>(this Map<K, Writer<Out, T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, Writer<Out, V>> SelectMany<K, Out, T, U, V>(this Map<K, Writer<Out, T>> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));

        // 
        // Map<K, Reader<Env,T>> extensions 
        // 

        public static Reader<Env, Unit> Iter<K, Env, T>(this Map<K, Reader<Env, T>> self, Action<T> action)
        {
            return env =>
            {
                foreach (var x in self.Values)
                {
                    x.Iter(action);
                }
                return unit;
            };
        }

        public static int Count<K, Env, T>(this Map<K, Reader<Env, T>> self) =>
            (from x in self.Values
             select x.Count()).Sum();

        public static Reader<Env, bool> ForAll<K, Env, T>(this Map<K, Reader<Env, T>> self, Func<T, bool> pred)
        {
            return env =>
            {
                foreach (var x in self.Values)
                {
                    if (!x.ForAll(pred)(env))
                    {
                        return false;
                    }
                }
                return true;
            };
        }

        public static Reader<Env, S> Fold<K, Env, S, T>(this Map<K, Reader<Env, T>> self, S state, Func<S, T, S> folder)
        {
            return env =>
            {
                foreach (var x in self.Values)
                {
                    state = x.Fold(state, folder)(env);
                }
                return state;
            };
        }

        public static Reader<Env, bool> Exists<K, Env, T>(this Map<K, Reader<Env, T>> self, Func<T, bool> pred)
        {
            return env =>
            {
                foreach (var x in self.Values)
                {
                    if (x.Exists(pred)(env))
                    {
                        return true;
                    }
                }
                return false;
            };
        }

        public static Map<K, Reader<Env, R>> Map<K, Env, T, R>(this Map<K, Reader<Env, T>> self, Func<T, R> mapper) =>
            self.Map(x => x.Map(mapper));

        public static Map<K, Reader<Env, R>> Bind<K, Env, T, R>(this Map<K, Reader<Env, T>> self, Func<T, Reader<Env, R>> binder) =>
            self.Map(x => x.Bind(binder));

        public static Map<K, Reader<Env, U>> Select<K, Env, T, U>(this Map<K, Reader<Env, T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, Reader<Env, V>> SelectMany<K, Env, T, U, V>(this Map<K, Reader<Env, T>> self,
            Func<T, Reader<Env, U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));


        // 
        // Map<K, <State<S,T>> extensions 
        // 

        public static State<S, Unit> Iter<K, S, T>(this Map<K, State<S, T>> self, Action<T> action)
        {
            return s =>
            {
                foreach (var x in self.Values)
                {
                    x.Iter(action)(s);
                }
                return new StateResult<S, Unit>(s, unit);
            };
        }

        public static int Count<K, S, T>(this Map<K, State<S, T>> self) =>
            (from x in self.Values
             select x.Count()).Sum();

        public static State<S, bool> ForAll<K, S, T>(this Map<K, State<S, T>> self, Func<T, bool> pred)
        {
            return s =>
            {
                foreach (var x in self.Values)
                {
                    if (!x.ForAll(pred)(s).Value)
                    {
                        return new StateResult<S, bool>(s, true);
                    }
                }
                return new StateResult<S, bool>(s, true);
            };
        }

        public static State<S, FState> Fold<K, S, T, FState>(this Map<K, State<S, T>> self, FState state, Func<FState, T, FState> folder)
        {
            return s =>
            {
                foreach (var x in self.Values)
                {
                    state = x.Fold(state, folder)(s).Value;
                }
                return new StateResult<S, FState>(s, state);
            };
        }

        public static State<S, Unit> Fold<K, S, T>(this Map<K, State<S, T>> self, Func<S, T, S> folder)
        {
            return s =>
            {
                foreach (var x in self.Values)
                {
                    s = x.Fold(folder)(s).State;
                }
                return new StateResult<S, Unit>(s, unit);
            };
        }

        public static State<S, bool> Exists<K, S, T>(this Map<K, State<S, T>> self, Func<T, bool> pred)
        {
            return s =>
            {
                foreach (var x in self.Values)
                {
                    if (x.Exists(pred)(s).Value)
                    {
                        return new StateResult<S, bool>(s, true);
                    }
                }
                return new StateResult<S, bool>(s, false);
            };
        }

        public static Map<K, State<S, R>> Map<K, S, T, R>(this Map<K, State<S, T>> self, Func<T, R> mapper) =>
            self.Map(x => x.Map(mapper));

        public static Map<K, State<S, R>> Bind<K, S, T, R>(this Map<K, State<S, T>> self, Func<T, State<S, R>> binder) =>
            self.Map(x => x.Bind(binder));

        public static Map<K, State<S, U>> Select<K, S, T, U>(this Map<K, State<S, T>> self, Func<T, U> map) =>
            self.Map(x => x.Map(map));

        public static Map<K, State<S, V>> SelectMany<K, S, T, U, V>(this Map<K, State<S, T>> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            ) =>
            self.Map(x => x.SelectMany(bind, project));
    }
}