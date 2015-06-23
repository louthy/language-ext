using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Wrapped type</typeparam>
    public delegate StateResult<S, T> State<S, T>(S state);

    /// <summary>
    /// State result.
    /// </summary>
    public struct StateResult<S, T>
    {
        readonly T value;
        public readonly S State;
        public readonly bool IsBottom;

        internal StateResult(S state, T value, bool isBottom = false)
        {
            this.value = value;
            State = state;
            IsBottom = isBottom;
        }

        public T Value =>
            IsBottom
                ? default(T)
                : value;

        public static implicit operator StateResult<S,T>(T value) =>
           new StateResult<S,T>(default(S),value);        // TODO:  Not a good idea

        public static implicit operator T(StateResult<S,T> value) =>
           value.Value;
    }

    public static class StateExt
    {
        public static State<S, IEnumerable<T>> AsEnumerable<S, T>(this State<S, T> self) =>
            from x in self
            select (new T[1] { x }).AsEnumerable();

        public static IEnumerable<T> AsEnumerable<S, T>(this State<S, T> self, S state)
        {
            var res = self(state);
            if (!res.IsBottom)
            {
                yield return self(state).Value;
            }
        }

        public static State<S, Unit> Iter<S, T>(this State<S, T> self, Action<T> action) =>
            s => bmap(self(s), action);

        public static State<S, int> Count<S, T>(this State<S, T> self) =>
            s => self(s).IsBottom
                ? 0
                : 1;

        public static State<S, bool> ForAll<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            from x in self
            select pred(x);

        public static State<S, bool> Exists<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            from x in self
            select pred(x);

        public static State<S, FState> Fold<S, T, FState>(this State<S, T> self, FState state, Func<FState, T, FState> folder) =>
            s => bmap(self(s), x => folder(state, x));

        public static State<S, S> Fold<S, T>(this State<S, T> self, Func<S, T, S> folder) =>
            s => bmap(self(s), x => folder(s, x));

        public static State<S, R> Map<S, T, R>(this State<S, T> self, Func<T, R> mapper) =>
            self.Select(mapper);

        public static State<S, R> Bind<S, T, R>(this State<S, T> self, Func<T, State<S, R>> binder) =>
            from x in self
            from y in binder(x)
            select y;

        public static State<S, U> Select<S, T, U>(this State<S, T> self, Func<T, U> map)
        {
            if (map == null) throw new ArgumentNullException("map");
            return (S state) =>
            {
                var resT = self(state);
                return resT.IsBottom
                    ? new StateResult<S, U>(resT.State, default(U), true)
                    : new StateResult<S, U>(resT.State, map(resT.Value));
            };
        }

        public static State<S, V> SelectMany<S, T, U, V>(
            this State<S, T> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            )
        {
            if (bind == null) throw new ArgumentNullException("bind");
            if (project == null) throw new ArgumentNullException("project");

            return (S state) =>
            {
                var resT = self(state);
                if (resT.IsBottom) return new StateResult<S, V>(resT.State, default(V), true);
                var resU = bind(resT.Value)(resT.State);
                if (resU.IsBottom) return new StateResult<S, V>(resU.State, default(V), true);
                var resV = project(resT.Value, resU.Value);
                return new StateResult<S, V>(resU.State, resV);
            };
        }

        public static State<S, T> Filter<S, T>(this State<S, T> self, Func<T, bool> pred) =>
            from x in self
            where pred(x)
            select x;

        public static State<S, T> Where<S, T>(this State<S, T> self, Func<T, bool> pred)
        {
            return state =>
            {
                var res = self(state);
                return new StateResult<S, T>(res.State, res.Value, !pred(res.Value));
            };
        }

        public static State<S, int> Sum<S>(this State<S, int> self) =>
            state => bmap(self(state), x => x);

        private static StateResult<S, R> bmap<S, T, R>(StateResult<S, T> r, Func<T, R> f) =>
            r.IsBottom
                ? new StateResult<S, R>(r.State, default(R), true)
                : new StateResult<S, R>(r.State, f(r.Value), false);

        private static StateResult<S, Unit> bmap<S, T>(StateResult<S, T> r, Action<T> f)
        {
            if (r.IsBottom)
            {
                return new StateResult<S, Unit>(r.State, unit, true);
            }
            else
            {
                f(r.Value);
                return new StateResult<S, Unit>(r.State, unit, false);
            }
        }
    }
}
